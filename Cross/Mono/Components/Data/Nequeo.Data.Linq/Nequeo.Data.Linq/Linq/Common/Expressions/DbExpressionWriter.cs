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
using System.IO;

using Nequeo.Data.Linq;
using Nequeo.Data.Linq.Common;
using Nequeo.Data.Linq.Language;
using Nequeo.Data.Linq.Provider;
using Nequeo.Data.Linq.Common.Expressions;
using Nequeo.Data.Linq.Common.Translation;

namespace Nequeo.Data.Linq.Common.Expressions
{
    /// <summary>
    /// Writes out an expression tree (including DbExpression nodes) in a C#-ish syntax
    /// </summary>
    public class DbExpressionWriter : ExpressionWriter
    {
        Dictionary<TableAlias, int> aliasMap = new Dictionary<TableAlias, int>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected DbExpressionWriter(TextWriter writer)
            : base(writer)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="expression"></param>
        public new static void Write(TextWriter writer, Expression expression)
        {
            new DbExpressionWriter(writer).Visit(expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public new static string WriteToString(Expression expression)
        {
            StringWriter sw = new StringWriter();
            Write(sw, expression);
            return sw.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected override Expression Visit(Expression exp)
        {
            if (exp == null)
                return null;

            switch ((DbExpressionType)exp.NodeType)
            {
                case DbExpressionType.Projection:
                    return this.VisitProjection((ProjectionExpression)exp);
                case DbExpressionType.ClientJoin:
                    return this.VisitClientJoin((ClientJoinExpression)exp);
                case DbExpressionType.Select:
                    return this.VisitSelect((SelectExpression)exp);
                case DbExpressionType.OuterJoined:
                    return this.VisitOuterJoined((OuterJoinedExpression)exp);
                case DbExpressionType.Column:
                    return this.VisitColumn((ColumnExpression)exp);
                default:
                    if (exp is DbExpression)
                    {
                        this.Write(TSqlFormatter.Format(exp));
                        return exp;
                    }
                    else
                    {
                        return base.Visit(exp);
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        protected void AddAlias(TableAlias alias)
        {
            if (!this.aliasMap.ContainsKey(alias))
            {
                this.aliasMap.Add(alias, this.aliasMap.Count);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projection"></param>
        /// <returns></returns>
        protected virtual Expression VisitProjection(ProjectionExpression projection)
        {
            this.AddAlias(projection.Source.Alias);
            this.Write("Project(");
            this.WriteLine(Indentation.Inner);
            this.Write("@\"");
            this.Visit(projection.Source);
            this.Write("\",");
            this.WriteLine(Indentation.Same);
            this.Visit(projection.Projector);
            this.Write(",");
            this.WriteLine(Indentation.Same);
            this.Visit(projection.Aggregator);
            this.WriteLine(Indentation.Outer);
            this.Write(")");
            return projection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="join"></param>
        /// <returns></returns>
        protected virtual Expression VisitClientJoin(ClientJoinExpression join)
        {
            this.AddAlias(join.Projection.Source.Alias);
            this.Write("ClientJoin(");
            this.WriteLine(Indentation.Inner);
            this.Write("OuterKey(");
            this.VisitExpressionList(join.OuterKey);
            this.Write("),");
            this.WriteLine(Indentation.Same);
            this.Write("InnerKey(");
            this.VisitExpressionList(join.InnerKey);
            this.Write("),");
            this.WriteLine(Indentation.Same);
            this.Visit(join.Projection);
            this.WriteLine(Indentation.Outer);
            this.Write(")");
            return join;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outer"></param>
        /// <returns></returns>
        protected virtual Expression VisitOuterJoined(OuterJoinedExpression outer)
        {
            this.Write("Outer(");
            this.WriteLine(Indentation.Inner);
            this.Visit(outer.Test);
            this.Write(", ");
            this.WriteLine(Indentation.Same);
            this.Visit(outer.Expression);
            this.WriteLine(Indentation.Outer);
            this.Write(")");
            return outer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        protected virtual Expression VisitSelect(SelectExpression select)
        {
            this.Write(select.QueryText);
            return select;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        protected virtual Expression VisitColumn(ColumnExpression column)
        {
            int iAlias;
            string aliasName = 
                this.aliasMap.TryGetValue(column.Alias, out iAlias)
                ? "A" + iAlias
                : "A?";

            this.Write(aliasName);
            this.Write(".");
            this.Write("Column(\"");
            this.Write(column.Name);
            this.Write("\")");
            return column;
        }
    }
}
 