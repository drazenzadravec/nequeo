/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using Nequeo.Data.Linq;
using Nequeo.Data.Linq.Common;
using Nequeo.Data.Linq.Language;
using Nequeo.Data.Linq.Provider;
using Nequeo.Data.Linq.Common.Expressions;
using Nequeo.Data.Linq.Common.Translation;

namespace Nequeo.Data.Linq.Common.Translation
{
    /// <summary>
    /// rewrites nested projections into client-side joins
    /// </summary>
    public class ClientJoinedProjectionRewriter : DbExpressionVisitor
    {
        QueryLanguage language;
        bool isTopLevel = true;
        SelectExpression currentSelect;

        private ClientJoinedProjectionRewriter(QueryLanguage language)
        {
            this.language = language;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="language"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Expression Rewrite(QueryLanguage language, Expression expression)
        {
            return new ClientJoinedProjectionRewriter(language).Visit(expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proj"></param>
        /// <returns></returns>
        protected override Expression VisitProjection(ProjectionExpression proj)
        {
            SelectExpression save = this.currentSelect;
            this.currentSelect = proj.Source;
            try
            {
                if (!this.isTopLevel)
                {
                    if (this.CanJoinOnClient(this.currentSelect))
                    {
                        // make a query that combines all the constraints from the outer queries into a single select
                        SelectExpression newOuterSelect = (SelectExpression)QueryDuplicator.Duplicate(save);

                        // remap any references to the outer select to the new alias;
                        SelectExpression newInnerSelect = (SelectExpression)ColumnMapper.Map(proj.Source, newOuterSelect.Alias, save.Alias);
                        // add outer-join test
                        ProjectionExpression newInnerProjection = new ProjectionExpression(newInnerSelect, proj.Projector).AddOuterJoinTest();
                        newInnerSelect = newInnerProjection.Source;
                        Expression newProjector = newInnerProjection.Projector;

                        TableAlias newAlias = new TableAlias();
                        var pc = ColumnProjector.ProjectColumns(this.language.CanBeColumn, newProjector, newOuterSelect.Columns, newAlias, newOuterSelect.Alias, newInnerSelect.Alias);
                        JoinExpression join = new JoinExpression(JoinType.OuterApply, newOuterSelect, newInnerSelect, null);
                        SelectExpression joinedSelect = new SelectExpression(newAlias, pc.Columns, join, null, null, null, proj.IsSingleton, null, null);

                        // apply client-join treatment recursively
                        this.currentSelect = joinedSelect;
                        newProjector = this.Visit(pc.Projector); 

                        // compute keys (this only works if join condition was a single column comparison)
                        List<Expression> outerKeys = new List<Expression>();
                        List<Expression> innerKeys = new List<Expression>();
                        if (this.GetEquiJoinKeyExpressions(newInnerSelect.Where, newOuterSelect.Alias, outerKeys, innerKeys))
                        {
                            // outerKey needs to refer to the outer-scope's alias
                            var outerKey = outerKeys.Select(k => ColumnMapper.Map(k, save.Alias, newOuterSelect.Alias));
                            // innerKey needs to refer to the new alias for the select with the new join
                            var innerKey = innerKeys.Select(k => ColumnMapper.Map(k, joinedSelect.Alias, ((ColumnExpression)k).Alias));
                            ProjectionExpression newProjection = new ProjectionExpression(joinedSelect, newProjector, proj.Aggregator);
                            return new ClientJoinExpression(newProjection, outerKey, innerKey);
                        }
                    }
                }
                else
                {
                    this.isTopLevel = false;
                }

                return base.VisitProjection(proj);
            }
            finally 
            {
                this.currentSelect = save;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        private bool CanJoinOnClient(SelectExpression select)
        {
            // can add singleton (1:0,1) join if no grouping/aggregates or distinct
            return !select.IsDistinct
                && (select.GroupBy == null || select.GroupBy.Count == 0)
                && !AggregateChecker.HasAggregates(select);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="outerAlias"></param>
        /// <param name="outerExpressions"></param>
        /// <param name="innerExpressions"></param>
        /// <returns></returns>
        private bool GetEquiJoinKeyExpressions(Expression predicate, TableAlias outerAlias, List<Expression> outerExpressions, List<Expression> innerExpressions)
        {
            // predicate can be AND's and EQUAL's between columns
            BinaryExpression b = predicate as BinaryExpression;
            if (b != null)
            {
                switch (predicate.NodeType)
                {
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                        return this.GetEquiJoinKeyExpressions(b.Left, outerAlias, outerExpressions, innerExpressions)
                            && this.GetEquiJoinKeyExpressions(b.Right, outerAlias, outerExpressions, innerExpressions);
                    case ExpressionType.Equal:
                        ColumnExpression left = b.Left as ColumnExpression;
                        ColumnExpression right = b.Right as ColumnExpression;
                        if (left != null && right != null)
                        {
                            if (left.Alias == outerAlias)
                            {
                                outerExpressions.Add(left);
                                innerExpressions.Add(right);
                                return true;
                            }
                            else if (right.Alias == outerAlias)
                            {
                                innerExpressions.Add(left);
                                outerExpressions.Add(right);
                                return true;
                            }
                        }
                        break;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subquery"></param>
        /// <returns></returns>
        protected override Expression VisitSubquery(SubqueryExpression subquery)
        {
            return subquery;
        }
    }
}