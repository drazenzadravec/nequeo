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
using System.Linq;
using System.Linq.Expressions;

using Nequeo.Data.Linq;
using Nequeo.Data.Linq.Common;
using Nequeo.Data.Linq.Language;
using Nequeo.Data.Linq.Provider;
using Nequeo.Data.Linq.Common.Expressions;
using Nequeo.Data.Linq.Common.Translation;

namespace Nequeo.Data.Linq.Common.Translation
{
    /// <summary>
    /// Rewrite aggregate expressions, moving them into same select expression that has the group-by clause
    /// </summary>
    public class AggregateRewriter : DbExpressionVisitor
    {
        ILookup<TableAlias, AggregateSubqueryExpression> lookup;
        Dictionary<AggregateSubqueryExpression, Expression> map;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expr"></param>
        private AggregateRewriter(Expression expr)
        {
            this.map = new Dictionary<AggregateSubqueryExpression, Expression>();
            this.lookup = AggregateGatherer.Gather(expr).ToLookup(a => a.GroupByAlias);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Expression Rewrite(Expression expr)
        {
            return new AggregateRewriter(expr).Visit(expr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        protected override Expression VisitSelect(SelectExpression select)
        {
            select = (SelectExpression)base.VisitSelect(select);
            if (lookup.Contains(select.Alias))
            {
                List<ColumnDeclaration> aggColumns = new List<ColumnDeclaration>(select.Columns);
                foreach (AggregateSubqueryExpression ae in lookup[select.Alias])
                {
                    string name = "agg" + aggColumns.Count;
                    ColumnDeclaration cd = new ColumnDeclaration(name, ae.AggregateInGroupSelect);
                    this.map.Add(ae, new ColumnExpression(ae.Type, ae.GroupByAlias, name));
                    aggColumns.Add(cd);
                }
                return new SelectExpression(select.Alias, aggColumns, select.From, select.Where, select.OrderBy, select.GroupBy, select.IsDistinct, select.Skip, select.Take);
            }
            return select;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aggregate"></param>
        /// <returns></returns>
        protected override Expression VisitAggregateSubquery(AggregateSubqueryExpression aggregate)
        {
            Expression mapped;
            if (this.map.TryGetValue(aggregate, out mapped))
            {
                return mapped;
            }
            return this.Visit(aggregate.AggregateAsSubquery);
        }

        /// <summary>
        /// 
        /// </summary>
        class AggregateGatherer : DbExpressionVisitor
        {
            List<AggregateSubqueryExpression> aggregates = new List<AggregateSubqueryExpression>();
            private AggregateGatherer()
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            internal static List<AggregateSubqueryExpression> Gather(Expression expression)
            {
                AggregateGatherer gatherer = new AggregateGatherer();
                gatherer.Visit(expression);
                return gatherer.aggregates;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="aggregate"></param>
            /// <returns></returns>
            protected override Expression VisitAggregateSubquery(AggregateSubqueryExpression aggregate)
            {
                this.aggregates.Add(aggregate);
                return base.VisitAggregateSubquery(aggregate);
            }
        }
    }
}