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
    /// Rewrites nested singleton projection into server-side joins
    /// </summary>
    public class SingletonProjectionRewriter : DbExpressionVisitor
    {
        QueryLanguage language;
        bool isTopLevel = true;
        SelectExpression currentSelect;

        private SingletonProjectionRewriter(QueryLanguage language)
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
            return new SingletonProjectionRewriter(language).Visit(expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="join"></param>
        /// <returns></returns>
        protected override Expression VisitClientJoin(ClientJoinExpression join)
        {
            // treat client joins as new top level
            var saveTop = this.isTopLevel;
            var saveSelect = this.currentSelect;
            this.isTopLevel = true;
            this.currentSelect = null;
            Expression result = base.VisitClientJoin(join);
            this.isTopLevel = saveTop;
            this.currentSelect = saveSelect;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proj"></param>
        /// <returns></returns>
        protected override Expression VisitProjection(ProjectionExpression proj)
        {
            if (isTopLevel)
            {
                isTopLevel = false;
                this.currentSelect = proj.Source;
                Expression projector = this.Visit(proj.Projector);
                if (projector != proj.Projector || this.currentSelect != proj.Source)
                {
                    return new ProjectionExpression(this.currentSelect, projector, proj.Aggregator);
                }
                return proj;
            }

            if (proj.IsSingleton && this.CanJoinOnServer(this.currentSelect))
            {
                TableAlias newAlias = new TableAlias();
                this.currentSelect = this.currentSelect.AddRedundantSelect(newAlias);

                // remap any references to the outer select to the new alias;
                SelectExpression source =(SelectExpression)ColumnMapper.Map(proj.Source, newAlias, this.currentSelect.Alias);

                // add outer-join test
                ProjectionExpression pex = new ProjectionExpression(source, proj.Projector).AddOuterJoinTest();

                var pc = ColumnProjector.ProjectColumns(this.language.CanBeColumn, pex.Projector, this.currentSelect.Columns, this.currentSelect.Alias, newAlias, proj.Source.Alias);

                JoinExpression join = new JoinExpression(JoinType.OuterApply, this.currentSelect.From, pex.Source, null);

                this.currentSelect = new SelectExpression(this.currentSelect.Alias, pc.Columns, join, null);
                return this.Visit(pc.Projector);
            }

            var saveTop = this.isTopLevel;
            var saveSelect = this.currentSelect;
            this.isTopLevel = true;
            this.currentSelect = null;
            Expression result = base.VisitProjection(proj);
            this.isTopLevel = saveTop;
            this.currentSelect = saveSelect;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        private bool CanJoinOnServer(SelectExpression select)
        {
            // can add singleton (1:0,1) join if no grouping/aggregates or distinct
            return !select.IsDistinct
                && (select.GroupBy == null || select.GroupBy.Count == 0)
                && !AggregateChecker.HasAggregates(select);
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