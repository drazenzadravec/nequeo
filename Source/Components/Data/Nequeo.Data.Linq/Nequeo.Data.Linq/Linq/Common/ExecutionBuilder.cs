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
using System.Data.Common;
using System.IO;
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

namespace Nequeo.Data.Linq.Common
{
    /// <summary>
    /// Builds an execution plan for a query expression
    /// </summary>
    public class ExecutionBuilder : DbExpressionVisitor
    {
        QueryPolicy policy;
        Expression provider;
        Scope scope;
        bool isTop = true;
        MemberInfo receivingMember;
        int nReaders = 0;
        List<ParameterExpression> variables = new List<ParameterExpression>();
        List<Expression> initializers = new List<Expression>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="provider"></param>
        private ExecutionBuilder(QueryPolicy policy, Expression provider)
        {
            this.policy = policy;
            this.provider = provider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="expression"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Expression Build(QueryPolicy policy, Expression expression, Expression provider)
        {
            return new ExecutionBuilder(policy, provider).Build(expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private Expression Build(Expression expression)
        {
            expression = this.Visit(expression);
            expression = this.AddVariables(expression);
            return expression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private Expression AddVariables(Expression expression)
        {
            // add variable assignments up front
            if (this.variables.Count > 0)
            {
                List<Expression> exprs = new List<Expression>();
                for (int i = 0, n = this.variables.Count; i < n; i++)
                {
                    exprs.Add(MakeAssign(this.variables[i], this.initializers[i]));
                }
                exprs.Add(expression);
                Expression sequence = MakeSequence(exprs);  // yields last expression value

                // use invoke/lambda to create variables via parameters in scope
                Expression[] nulls = this.variables.Select(v => Expression.Constant(null, v.Type)).ToArray();
                expression = Expression.Invoke(Expression.Lambda(sequence, this.variables.ToArray()), nulls);
            }

            return expression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        private static Expression MakeSequence(IList<Expression> expressions)
        {
            Expression last = expressions[expressions.Count - 1];
            return Expression.Convert(Expression.Call(typeof(ExecutionBuilder), "Sequence", null, Expression.NewArrayInit(typeof(object), expressions)), last.Type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static object Sequence(params object[] values) 
        {
            return values[values.Length - 1];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Expression MakeAssign(ParameterExpression variable, Expression value)
        {
            return Expression.Call(typeof(ExecutionBuilder), "Assign", new Type[] { variable.Type }, variable, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Assign<T>(ref T variable, T value)
        {
            variable = value;
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private Expression BuildInner(Expression expression)
        {
            var eb = new ExecutionBuilder(this.policy, this.provider);
            eb.scope = this.scope;
            eb.receivingMember = this.receivingMember;
            eb.nReaders = this.nReaders;
            eb.nLookup = this.nLookup;
            return eb.Build(expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        protected override MemberBinding VisitBinding(MemberBinding binding)
        {
            var save = this.receivingMember;
            this.receivingMember = binding.Member;
            var result = base.VisitBinding(binding);
            this.receivingMember = save;
            return result;
        }

        int nLookup = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private Expression MakeJoinKey(IList<Expression> key)
        {
            if (key.Count == 1)
            {
                return key[0];
            }
            else
            {
                return Expression.New(
                    typeof(CompoundKey).GetConstructors()[0],
                    Expression.NewArrayInit(typeof(object), key.Select(k => (Expression)Expression.Convert(k, typeof(object))))
                    );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="join"></param>
        /// <returns></returns>
        protected override Expression VisitClientJoin(ClientJoinExpression join)
        {
            // convert client join into a up-front lookup table builder & replace client-join in tree with lookup accessor

            // 1) lookup = query.Select(e => new KVP(key: inner, value: e)).ToLookup(kvp => kvp.Key, kvp => kvp.Value)
            Expression innerKey = MakeJoinKey(join.InnerKey);
            Expression outerKey = MakeJoinKey(join.OuterKey);

            ConstructorInfo kvpConstructor = typeof(KeyValuePair<,>).MakeGenericType(innerKey.Type, join.Projection.Projector.Type).GetConstructor(new Type[] { innerKey.Type, join.Projection.Projector.Type });
            Expression constructKVPair = Expression.New(kvpConstructor, innerKey, join.Projection.Projector);
            ProjectionExpression newProjection = new ProjectionExpression(join.Projection.Source, constructKVPair);

            int iLookup = ++nLookup;
            Expression execution = this.ExecuteProjection(newProjection, false);

            ParameterExpression kvp = Expression.Parameter(constructKVPair.Type, "kvp");

            // filter out nulls
            if (join.Projection.Projector.NodeType == (ExpressionType)DbExpressionType.OuterJoined)
            {
                LambdaExpression pred = Expression.Lambda(
                    Expression.NotEqual(
                        Expression.PropertyOrField(kvp, "Value"),
                        Expression.Constant(null, join.Projection.Projector.Type)
                        ),
                    kvp
                    );
                execution = Expression.Call(typeof(Enumerable), "Where", new Type[] { kvp.Type }, execution, pred);
            }

            // make lookup
            LambdaExpression keySelector = Expression.Lambda(Expression.PropertyOrField(kvp, "Key"), kvp);
            LambdaExpression elementSelector = Expression.Lambda(Expression.PropertyOrField(kvp, "Value"), kvp);
            Expression toLookup = Expression.Call(typeof(Enumerable), "ToLookup", new Type[] { kvp.Type, outerKey.Type, join.Projection.Projector.Type }, execution, keySelector, elementSelector);

            // 2) agg(lookup[outer])
            ParameterExpression lookup = Expression.Parameter(toLookup.Type, "lookup" + iLookup);
            PropertyInfo property = lookup.Type.GetProperty("Item");
            Expression access = Expression.Call(lookup, property.GetGetMethod(), this.Visit(outerKey));
            if (join.Projection.Aggregator != null)
            {
                // apply aggregator
                access = DbExpressionReplacer.Replace(join.Projection.Aggregator.Body, join.Projection.Aggregator.Parameters[0], access);
            }

            this.variables.Add(lookup);
            this.initializers.Add(toLookup);

            return access;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projection"></param>
        /// <returns></returns>
        protected override Expression VisitProjection(ProjectionExpression projection)
        {
            if (this.isTop)
            {
                this.isTop = false;
                return this.ExecuteProjection(projection, this.scope != null);
            }
            else
            {
                return this.BuildInner(projection);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projection"></param>
        /// <param name="okayToDefer"></param>
        /// <returns></returns>
        private Expression ExecuteProjection(ProjectionExpression projection, bool okayToDefer)
        {
            okayToDefer &= (this.receivingMember != null && this.policy.IsDeferLoaded(this.receivingMember));

            // parameterize query
            projection = (ProjectionExpression)Parameterizer.Parameterize(projection);

            if (this.scope != null)
            {
                // also convert references to outer alias to named values!  these become SQL parameters too
                projection = (ProjectionExpression)OuterParameterizer.Parameterize(this.scope.Alias, projection);
            }

            var saveScope = this.scope;
            ParameterExpression reader = Expression.Parameter(typeof(DbDataReader), "r" + nReaders++);
            this.scope = new Scope(this.scope, reader, projection.Source.Alias, projection.Source.Columns);
            LambdaExpression projector = Expression.Lambda(this.Visit(projection.Projector), reader);
            this.scope = saveScope;

            string commandText = this.policy.Mapping.Language.Format(projection.Source);
            ReadOnlyCollection<NamedValueExpression> namedValues = NamedValueGatherer.Gather(projection.Source);
            string[] names = namedValues.Select(v => v.Name).ToArray();
            Expression[] values = namedValues.Select(v => Expression.Convert(this.Visit(v.Value), typeof(object))).ToArray();

            string methExecute = okayToDefer 
                ? "ExecuteDeferred" 
                : "Execute";

            if (okayToDefer)
            {
            }

            // call low-level execute directly on supplied DbQueryProvider
            Expression result = Expression.Call(this.provider, methExecute, new Type[] { projector.Body.Type },
                Expression.New(
                    typeof(QueryCommand<>).MakeGenericType(projector.Body.Type).GetConstructors()[0],
                    Expression.Constant(commandText),
                    Expression.Constant(names),
                    projector
                    ),
                Expression.NewArrayInit(typeof(object), values)
                );

            if (projection.Aggregator != null)
            {
                // apply aggregator
                result = DbExpressionReplacer.Replace(projection.Aggregator.Body, projection.Aggregator.Parameters[0], result);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outer"></param>
        /// <returns></returns>
        protected override Expression VisitOuterJoined(OuterJoinedExpression outer)
        {
            Expression expr = this.Visit(outer.Expression);
            ColumnExpression column = (ColumnExpression)outer.Test;
            ParameterExpression reader;
            int iOrdinal;
            if (this.scope.TryGetValue(column, out reader, out iOrdinal))
            {
                return Expression.Condition(
                    Expression.Call(reader, "IsDbNull", null, Expression.Constant(iOrdinal)),
                    Expression.Constant(null, outer.Type),
                    expr
                    );
            }
            return expr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        protected override Expression VisitColumn(ColumnExpression column)
        {
            ParameterExpression reader;
            int iOrdinal;

            if (this.scope.TryGetValue(column, out reader, out iOrdinal))
            {
                Expression defvalue;
                if (!column.Type.IsValueType || TypeHelper.IsNullableType(column.Type))
                {
                    defvalue = Expression.Constant(null, column.Type);
                }
                else
                {
                    defvalue = Expression.Constant(Activator.CreateInstance(column.Type), column.Type);
                }

                // this sucks, but since we don't track true SQL types through the query, and ADO throws exception if you
                // call the wrong accessor, the best we can do is call GetValue and Convert.ChangeType
                Expression value = Expression.Convert(
                    Expression.Call(typeof(System.Convert), "ChangeType", null,
                        Expression.Call(reader, "GetValue", null, Expression.Constant(iOrdinal)),
                        Expression.Constant(TypeHelper.GetNonNullableType(column.Type))
                        ),
                        column.Type
                    );

                return Expression.Condition(
                    Expression.Call(reader, "IsDbNull", null, Expression.Constant(iOrdinal)),
                    defvalue, value
                    );
            }
            else
            {
                System.Diagnostics.Debug.Fail(string.Format("column not in scope: {0}", column));
            }
            return column;
        }

        /// <summary>
        /// 
        /// </summary>
        class Scope
        {
            Scope outer;
            ParameterExpression dbDataReader;
            internal TableAlias Alias { get; private set; }
            Dictionary<string, int> nameMap;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="outer"></param>
            /// <param name="dbDataReaderParam"></param>
            /// <param name="alias"></param>
            /// <param name="columns"></param>
            internal Scope(Scope outer, ParameterExpression dbDataReaderParam, TableAlias alias, IEnumerable<ColumnDeclaration> columns)
            {
                this.outer = outer;
                this.dbDataReader = dbDataReaderParam;
                this.Alias = alias;
                this.nameMap = columns.Select((c, i) => new { c, i }).ToDictionary(x => x.c.Name, x => x.i);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="column"></param>
            /// <param name="dbDataReader"></param>
            /// <param name="ordinal"></param>
            /// <returns></returns>
            internal bool TryGetValue(ColumnExpression column, out ParameterExpression dbDataReader, out int ordinal)
            {
                for (Scope s = this; s != null; s = s.outer)
                {
                    if (column.Alias == s.Alias && this.nameMap.TryGetValue(column.Name, out ordinal))
                    {
                        dbDataReader = this.dbDataReader;
                        return true;
                    }
                }
                dbDataReader = null;
                ordinal = 0;
                return false;
            }
        }

        /// <summary>
        /// columns referencing the outer alias are turned into special named-value parameters
        /// </summary>
        class OuterParameterizer : DbExpressionVisitor
        {
            int iParam;
            TableAlias outerAlias;
            Dictionary<ColumnExpression, NamedValueExpression> map = new Dictionary<ColumnExpression, NamedValueExpression>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="outerAlias"></param>
            /// <param name="expr"></param>
            /// <returns></returns>
            internal static Expression Parameterize(TableAlias outerAlias, Expression expr)
            {
                OuterParameterizer op = new OuterParameterizer();
                op.outerAlias = outerAlias;
                return op.Visit(expr);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="proj"></param>
            /// <returns></returns>
            protected override Expression VisitProjection(ProjectionExpression proj)
            {
                SelectExpression select = (SelectExpression)this.Visit(proj.Source);
                if (select != proj.Source)
                {
                    return new ProjectionExpression(select, proj.Projector, proj.Aggregator);
                }
                return proj;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="column"></param>
            /// <returns></returns>
            protected override Expression VisitColumn(ColumnExpression column)
            {
                if (column.Alias == this.outerAlias)
                {
                    NamedValueExpression nv;
                    if (!this.map.TryGetValue(column, out nv)) 
                    {
                        nv = new NamedValueExpression("n" + (iParam++), column);
                        this.map.Add(column, nv);
                    }
                    return nv;
                }
                return column;
            }
        }
    }
}