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

namespace Nequeo.Data.Linq.Common.Expressions
{
    /// <summary>
    /// Determines if two expressions are equivalent. Supports DbExpression nodes.
    /// </summary>
    public class DbExpressionComparer : ExpressionComparer
    {
        ScopedDictionary<TableAlias, TableAlias> aliasScope;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterScope"></param>
        /// <param name="aliasScope"></param>
        protected DbExpressionComparer(ScopedDictionary<ParameterExpression, ParameterExpression> parameterScope, ScopedDictionary<TableAlias, TableAlias> aliasScope)
            : base(parameterScope)
        {
            this.aliasScope = aliasScope;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public new static bool AreEqual(Expression a, Expression b)
        {
            return AreEqual(null, null, a, b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterScope"></param>
        /// <param name="aliasScope"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool AreEqual(ScopedDictionary<ParameterExpression, ParameterExpression> parameterScope, ScopedDictionary<TableAlias, TableAlias> aliasScope, Expression a, Expression b)
        {
            return new DbExpressionComparer(parameterScope, aliasScope).Compare(a, b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override bool Compare(Expression a, Expression b)
        {
            if (a == b)
                return true;
            if (a == null || b == null)
                return false;
            if (a.NodeType != b.NodeType)
                return false;
            if (a.Type != b.Type)
                return false;
            switch ((DbExpressionType)a.NodeType)
            {
                case DbExpressionType.Table:
                    return this.CompareTable((TableExpression)a, (TableExpression)b);
                case DbExpressionType.Column:
                    return this.CompareColumn((ColumnExpression)a, (ColumnExpression)b);
                case DbExpressionType.Select:
                    return this.CompareSelect((SelectExpression)a, (SelectExpression)b);
                case DbExpressionType.Join:
                    return this.CompareJoin((JoinExpression)a, (JoinExpression)b);
                case DbExpressionType.Aggregate:
                    return this.CompareAggregate((AggregateExpression)a, (AggregateExpression)b);
                case DbExpressionType.Scalar:
                case DbExpressionType.Exists:
                case DbExpressionType.In:
                    return this.CompareSubquery((SubqueryExpression)a, (SubqueryExpression)b);
                case DbExpressionType.AggregateSubquery:
                    return this.CompareAggregateSubquery((AggregateSubqueryExpression)a, (AggregateSubqueryExpression)b);
                case DbExpressionType.IsNull:
                    return this.CompareIsNull((IsNullExpression)a, (IsNullExpression)b);
                case DbExpressionType.Between:
                    return this.CompareBetween((BetweenExpression)a, (BetweenExpression)b);
                case DbExpressionType.RowCount:
                    return this.CompareRowNumber((RowNumberExpression)a, (RowNumberExpression)b);
                case DbExpressionType.Projection:
                    return this.CompareProjection((ProjectionExpression)a, (ProjectionExpression)b);
                case DbExpressionType.NamedValue:
                    return this.CompareNamedValue((NamedValueExpression)a, (NamedValueExpression)b);
                default:
                    return base.Compare(a, b);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CompareTable(TableExpression a, TableExpression b)
        {
            return a.Name == b.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CompareColumn(ColumnExpression a, ColumnExpression b)
        {
            return this.CompareAlias(a.Alias, b.Alias) && a.Name == b.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CompareAlias(TableAlias a, TableAlias b)        
        {
            if (this.aliasScope != null)
            {
                TableAlias mapped;
                if (this.aliasScope.TryGetValue(a, out mapped))
                    return mapped == b;
            }
            return a == b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CompareSelect(SelectExpression a, SelectExpression b)
        {
            var save = this.aliasScope;
            try
            {
                if (!this.Compare(a.From, b.From))
                    return false;

                this.aliasScope = new ScopedDictionary<TableAlias, TableAlias>(save);
                this.MapAliases(a.From, b.From);

                return this.Compare(a.Where, b.Where)
                    && this.CompareOrderList(a.OrderBy, b.OrderBy)
                    && this.CompareExpressionList(a.GroupBy, b.GroupBy)
                    && this.Compare(a.Skip, b.Skip)
                    && this.Compare(a.Take, b.Take)
                    && a.IsDistinct == b.IsDistinct
                    && this.CompareColumnDeclarations(a.Columns, b.Columns);
            }
            finally
            {
                this.aliasScope = save;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void MapAliases(Expression a, Expression b)
        {
            TableAlias[] prodA = DeclaredAliasGatherer.Gather(a).ToArray();
            TableAlias[] prodB = DeclaredAliasGatherer.Gather(b).ToArray();
            for (int i = 0, n = prodA.Length; i < n; i++) 
            {
                this.aliasScope.Add(prodA[i], prodB[i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CompareOrderList(ReadOnlyCollection<OrderExpression> a, ReadOnlyCollection<OrderExpression> b)
        {
            if (a == b)
                return true;
            if (a == null || b == null)
                return false;
            if (a.Count != b.Count)
                return false;
            for (int i = 0, n = a.Count; i < n; i++)
            {
                if (a[i].OrderType != b[i].OrderType ||
                    !this.Compare(a[i].Expression, b[i].Expression))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CompareColumnDeclarations(ReadOnlyCollection<ColumnDeclaration> a, ReadOnlyCollection<ColumnDeclaration> b)
        {
            if (a == b)
                return true;
            if (a == null || b == null)
                return false;
            if (a.Count != b.Count)
                return false;
            for (int i = 0, n = a.Count; i < n; i++)
            {
                if (!this.CompareColumnDeclaration(a[i], b[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CompareColumnDeclaration(ColumnDeclaration a, ColumnDeclaration b)
        {
            return a.Name == b.Name && this.Compare(a.Expression, b.Expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CompareJoin(JoinExpression a, JoinExpression b)
        {
            if (a.Join != b.Join || !this.Compare(a.Left, b.Left))
                return false;

            if (a.Join == JoinType.CrossApply || a.Join == JoinType.OuterApply)
            {
                var save = this.aliasScope;
                try
                {
                    this.aliasScope = new ScopedDictionary<TableAlias, TableAlias>(this.aliasScope);
                    this.MapAliases(a.Left, b.Left);

                    return this.Compare(a.Right, b.Right)
                        && this.Compare(a.Condition, b.Condition);
                }
                finally
                {
                    this.aliasScope = save;
                }
            }
            else
            {
                return this.Compare(a.Right, b.Right)
                    && this.Compare(a.Condition, b.Condition);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CompareAggregate(AggregateExpression a, AggregateExpression b)
        {
            return a.AggregateType == b.AggregateType && this.Compare(a.Argument, b.Argument);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CompareIsNull(IsNullExpression a, IsNullExpression b)
        {
            return this.Compare(a.Expression, b.Expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CompareBetween(BetweenExpression a, BetweenExpression b)
        {
            return this.Compare(a.Expression, b.Expression)
                && this.Compare(a.Lower, b.Lower)
                && this.Compare(a.Upper, b.Upper);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CompareRowNumber(RowNumberExpression a, RowNumberExpression b)
        {
            return this.CompareOrderList(a.OrderBy, b.OrderBy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CompareNamedValue(NamedValueExpression a, NamedValueExpression b)
        {
            return a.Name == b.Name && this.Compare(a.Value, b.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CompareSubquery(SubqueryExpression a, SubqueryExpression b)
        {
            if (a.NodeType != b.NodeType)
                return false;
            switch ((DbExpressionType)a.NodeType)
            {
                case DbExpressionType.Scalar:
                    return this.CompareScalar((ScalarExpression)a, (ScalarExpression)b);
                case DbExpressionType.Exists:
                    return this.CompareExists((ExistsExpression)a, (ExistsExpression)b);
                case DbExpressionType.In:
                    return this.CompareIn((InExpression)a, (InExpression)b);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CompareScalar(ScalarExpression a, ScalarExpression b)
        {
            return this.Compare(a.Select, b.Select);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CompareExists(ExistsExpression a, ExistsExpression b)
        {
            return this.Compare(a.Select, b.Select);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CompareIn(InExpression a, InExpression b)
        {
            return this.Compare(a.Expression, b.Expression)
                && this.Compare(a.Select, b.Select)
                && this.CompareExpressionList(a.Values, b.Values);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CompareAggregateSubquery(AggregateSubqueryExpression a, AggregateSubqueryExpression b)
        {
            return this.Compare(a.AggregateAsSubquery, b.AggregateAsSubquery)
                && this.Compare(a.AggregateInGroupSelect, b.AggregateInGroupSelect)
                && a.GroupByAlias == b.GroupByAlias;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CompareProjection(ProjectionExpression a, ProjectionExpression b)
        {
            if (!this.Compare(a.Source, b.Source))
                return false;

            var save = this.aliasScope;
            try
            {
                this.aliasScope = new ScopedDictionary<TableAlias, TableAlias>(this.aliasScope);
                this.aliasScope.Add(a.Source.Alias, b.Source.Alias);

                return this.Compare(a.Projector, b.Projector)
                    && this.Compare(a.Aggregator, b.Aggregator)
                    && a.IsSingleton == b.IsSingleton;
            }
            finally
            {
                this.aliasScope = save;
            }
        }
    }
}