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
    /// 
    /// </summary>
    public class SkipRewriter : DbExpressionVisitor
    {
        private Nequeo.Data.DataType.ConnectionContext.ConnectionDataType _dataType = Nequeo.Data.DataType.ConnectionContext.ConnectionDataType.None;

        /// <summary>
        /// 
        /// </summary>
        private SkipRewriter()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        internal Nequeo.Data.DataType.ConnectionContext.ConnectionDataType MappingDataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public static Expression Rewrite(Expression expression, Nequeo.Data.DataType.ConnectionContext.ConnectionDataType dataType)
        {
            SkipRewriter skipRewriter = new SkipRewriter();
            skipRewriter.MappingDataType = dataType;
            return skipRewriter.Visit(expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        protected override Expression VisitSelect(SelectExpression select)
        {
            switch (_dataType)
            {
                case Nequeo.Data.DataType.ConnectionContext.ConnectionDataType.OracleDataType:
                case Nequeo.Data.DataType.ConnectionContext.ConnectionDataType.SqlDataType:

                    select = (SelectExpression)base.VisitSelect(select);
                    if (select.Skip != null)
                    {
                        SelectExpression newSelect = select.SetSkip(null).SetTake(null);
                        bool canAddColumn = !select.IsDistinct && (select.GroupBy == null || select.GroupBy.Count == 0);
                        if (!canAddColumn)
                        {
                            newSelect = newSelect.AddRedundantSelect(new TableAlias());
                        }
                        newSelect = newSelect.AddColumn(new ColumnDeclaration("ROW_NUM", new RowNumberExpression(select.OrderBy)));

                        // add layer for WHERE clause that references new rownum column
                        newSelect = newSelect.AddRedundantSelect(new TableAlias());
                        newSelect = newSelect.RemoveColumn(newSelect.Columns[newSelect.Columns.Count - 1]);

                        var newAlias = ((SelectExpression)newSelect.From).Alias;
                        ColumnExpression rnCol = new ColumnExpression(typeof(int), newAlias, "ROW_NUM");
                        Expression where;
                        if (select.Take != null)
                        {
                            where = new BetweenExpression(rnCol, Expression.Add(select.Skip, Expression.Constant(1)), Expression.Add(select.Skip, select.Take));
                        }
                        else
                        {
                            where = Expression.GreaterThan(rnCol, select.Skip);
                        }
                        if (newSelect.Where != null)
                        {
                            where = Expression.And(newSelect.Where, where);
                        }
                        newSelect = newSelect.SetWhere(where);

                        select = newSelect;
                    }
                    return select;

                default:
                    select = (SelectExpression)base.VisitSelect(select);
                    if (select.Skip != null)
                    {
                        SelectExpression newSelect = select.SetSkip(select.Skip).SetTake(select.Take);
                        select = newSelect;
                    }
                    return select;
            }
        }
    }
}