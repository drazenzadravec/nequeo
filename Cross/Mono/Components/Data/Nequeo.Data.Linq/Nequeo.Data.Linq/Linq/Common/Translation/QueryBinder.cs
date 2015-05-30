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
    /// Converts LINQ query operators to into custom DbExpression's
    /// </summary>
    public class QueryBinder : DbExpressionVisitor
    {
        QueryMapping mapping;
        Dictionary<ParameterExpression, Expression> map;
        Dictionary<Expression, GroupByInfo> groupByMap;
        Expression root;

        private QueryBinder(QueryMapping mapping, Expression root)
        {
            this.mapping = mapping;
            this.map = new Dictionary<ParameterExpression, Expression>();
            this.groupByMap = new Dictionary<Expression, GroupByInfo>();
            this.root = root;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Expression Bind(QueryMapping mapping, Expression expression)
        {
            return new QueryBinder(mapping, expression).Visit(expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal TableAlias GetNextAlias()
        {
            return new TableAlias();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="newAlias"></param>
        /// <param name="existingAliases"></param>
        /// <returns></returns>
        private ProjectedColumns ProjectColumns(Expression expression, TableAlias newAlias, params TableAlias[] existingAliases)
        {
            return ColumnProjector.ProjectColumns(this.mapping.Language.CanBeColumn, expression, null, newAlias, existingAliases);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) || m.Method.DeclaringType == typeof(Enumerable))
            {
                switch (m.Method.Name)
                {
                    case "Where":
                        return this.BindWhere(m.Type, m.Arguments[0], (LambdaExpression)StripQuotes(m.Arguments[1]));
                    case "Select":
                        return this.BindSelect(m.Type, m.Arguments[0], (LambdaExpression)StripQuotes(m.Arguments[1]));
                    case "SelectMany":
                        if (m.Arguments.Count == 2)
                        {
                            return this.BindSelectMany(
                                m.Type, m.Arguments[0],
                                (LambdaExpression)StripQuotes(m.Arguments[1]),
                                null
                                );
                        }
                        else if (m.Arguments.Count == 3)
                        {
                            return this.BindSelectMany(
                                m.Type, m.Arguments[0],
                                (LambdaExpression)StripQuotes(m.Arguments[1]),
                                (LambdaExpression)StripQuotes(m.Arguments[2])
                                );
                        }
                        break;
                    case "Join":
                        return this.BindJoin(
                            m.Type, m.Arguments[0], m.Arguments[1],
                            (LambdaExpression)StripQuotes(m.Arguments[2]),
                            (LambdaExpression)StripQuotes(m.Arguments[3]),
                            (LambdaExpression)StripQuotes(m.Arguments[4])
                            );
                    case "OrderBy":
                        return this.BindOrderBy(m.Type, m.Arguments[0], (LambdaExpression)StripQuotes(m.Arguments[1]), OrderType.Ascending);
                    case "OrderByDescending":
                        return this.BindOrderBy(m.Type, m.Arguments[0], (LambdaExpression)StripQuotes(m.Arguments[1]), OrderType.Descending);
                    case "ThenBy":
                        return this.BindThenBy(m.Arguments[0], (LambdaExpression)StripQuotes(m.Arguments[1]), OrderType.Ascending);
                    case "ThenByDescending":
                        return this.BindThenBy(m.Arguments[0], (LambdaExpression)StripQuotes(m.Arguments[1]), OrderType.Descending);
                    case "GroupBy":
                        if (m.Arguments.Count == 2)
                        {
                            return this.BindGroupBy(
                                m.Arguments[0],
                                (LambdaExpression)StripQuotes(m.Arguments[1]),
                                null,
                                null
                                );
                        }
                        else if (m.Arguments.Count == 3)
                        {
                            LambdaExpression lambda1 = (LambdaExpression)StripQuotes(m.Arguments[1]);
                            LambdaExpression lambda2 = (LambdaExpression)StripQuotes(m.Arguments[2]);
                            if (lambda2.Parameters.Count == 1)
                            {
                                // second lambda is element selector
                                return this.BindGroupBy(m.Arguments[0], lambda1, lambda2, null);
                            }
                            else if (lambda2.Parameters.Count == 2)
                            {
                                // second lambda is result selector
                                return this.BindGroupBy(m.Arguments[0], lambda1, null, lambda2);
                            }
                        }
                        else if (m.Arguments.Count == 4)
                        {
                            return this.BindGroupBy(
                                m.Arguments[0],
                                (LambdaExpression)StripQuotes(m.Arguments[1]),
                                (LambdaExpression)StripQuotes(m.Arguments[2]),
                                (LambdaExpression)StripQuotes(m.Arguments[3])
                                );
                        }
                        break;
                    case "Count":
                    case "Min":
                    case "Max":
                    case "Sum":
                    case "Average":
                        if (m.Arguments.Count == 1)
                        {
                            return this.BindAggregate(m.Arguments[0], m.Method, null, m == this.root);
                        }
                        else if (m.Arguments.Count == 2)
                        {
                            LambdaExpression selector = (LambdaExpression)StripQuotes(m.Arguments[1]);
                            return this.BindAggregate(m.Arguments[0], m.Method, selector, m == this.root);
                        }
                        break;
                    case "Distinct":
                        if (m.Arguments.Count == 1)
                        {
                            return this.BindDistinct(m.Arguments[0]);
                        }
                        break;
                    case "Skip":
                        if (m.Arguments.Count == 2)
                        {
                            return this.BindSkip(m.Arguments[0], m.Arguments[1]);
                        }
                        break;
                    case "Take":
                        if (m.Arguments.Count == 2)
                        {
                            return this.BindTake(m.Arguments[0], m.Arguments[1]);
                        }
                        break;
                    case "First":
                    case "FirstOrDefault":
                    case "Single":
                    case "SingleOrDefault":
                        if (m.Arguments.Count == 1)
                        {
                            return this.BindFirst(m.Arguments[0], null, m.Method.Name, m == this.root);
                        }
                        else if (m.Arguments.Count == 2)
                        {
                            LambdaExpression predicate = (LambdaExpression)StripQuotes(m.Arguments[1]);
                            return this.BindFirst(m.Arguments[0], predicate, m.Method.Name, m == this.root);
                        }
                        break;
                    case "Any":
                        if (m.Arguments.Count == 1)
                        {
                            return this.BindAnyAll(m.Arguments[0], m.Method, null, m == this.root);
                        }
                        else if (m.Arguments.Count == 2)
                        {
                            LambdaExpression predicate = (LambdaExpression)StripQuotes(m.Arguments[1]);
                            return this.BindAnyAll(m.Arguments[0], m.Method, predicate, m == this.root);
                        }
                        break;
                    case "All":
                        if (m.Arguments.Count == 2)
                        {
                            LambdaExpression predicate = (LambdaExpression)StripQuotes(m.Arguments[1]);
                            return this.BindAnyAll(m.Arguments[0], m.Method, predicate, m == this.root);
                        }
                        break;
                    case "Contains":
                        if (m.Arguments.Count == 2)
                        {
                            return this.BindContains(m.Arguments[0], m.Arguments[1], m == this.root);
                        }
                        break;
                }
            }
            return base.VisitMethodCall(m);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private ProjectionExpression VisitSequence(Expression source)
        {
            // sure to call base.Visit in order to skip my override
            return this.ConvertToSequence(base.Visit(source));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private ProjectionExpression ConvertToSequence(Expression expr)
        {
            switch (expr.NodeType)
            {
                case (ExpressionType)DbExpressionType.Projection:
                    return (ProjectionExpression)expr;
                case ExpressionType.New:
                    NewExpression nex = (NewExpression)expr;
                    if (expr.Type.IsGenericType && expr.Type.GetGenericTypeDefinition() == typeof(Grouping<,>))
                    {
                        return (ProjectionExpression)nex.Arguments[1];
                    }
                    goto default;
                case ExpressionType.MemberAccess:
                    return ConvertToSequence(this.BindRelationshipProperty((MemberExpression)expr));
                default:
                    throw new Exception(string.Format("The expression of type '{0}' is not a sequence", expr.Type));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mex"></param>
        /// <returns></returns>
        private Expression BindRelationshipProperty(MemberExpression mex)
        {
            if (this.mapping.IsRelationship(mex.Member))
            {
                return this.mapping.GetMemberExpression(mex.Expression, mex.Member);
            }
            return mex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected override Expression Visit(Expression exp)
        {
            Expression result = base.Visit(exp);

            if (result != null)
            {
                // bindings that expect projections should have called VisitSequence, the rest will probably get annoyed if
                // the projection does not have the expected type.
                Type expectedType = exp.Type;
                ProjectionExpression projection = result as ProjectionExpression;
                if (projection != null && projection.Aggregator == null && !expectedType.IsAssignableFrom(projection.Type))
                {
                    LambdaExpression aggregator = this.mapping.GetAggregator(expectedType, projection.Type);
                    if (aggregator != null)
                    {
                        return new ProjectionExpression(projection.Source, projection.Projector, aggregator);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resultType"></param>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private Expression BindWhere(Type resultType, Expression source, LambdaExpression predicate)
        {
            ProjectionExpression projection = this.VisitSequence(source);
            this.map[predicate.Parameters[0]] = projection.Projector;
            Expression where = this.Visit(predicate.Body);
            var alias = this.GetNextAlias();
            ProjectedColumns pc = this.ProjectColumns(projection.Projector, alias, projection.Source.Alias);
            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, projection.Source, where),
                pc.Projector
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resultType"></param>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        private Expression BindSelect(Type resultType, Expression source, LambdaExpression selector)
        {
            ProjectionExpression projection = this.VisitSequence(source);
            this.map[selector.Parameters[0]] = projection.Projector;
            Expression expression = this.Visit(selector.Body);
            var alias = this.GetNextAlias();
            ProjectedColumns pc = this.ProjectColumns(expression, alias, projection.Source.Alias);
            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, projection.Source, null),
                pc.Projector
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resultType"></param>
        /// <param name="source"></param>
        /// <param name="collectionSelector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        protected virtual Expression BindSelectMany(Type resultType, Expression source, LambdaExpression collectionSelector, LambdaExpression resultSelector)
        {
            ProjectionExpression projection = this.VisitSequence(source);
            this.map[collectionSelector.Parameters[0]] = projection.Projector;

            Expression collection = collectionSelector.Body;

            // check for DefaultIfEmpty
            bool defaultIfEmpty = false;
            MethodCallExpression mcs = collection as MethodCallExpression;
            if (mcs != null && mcs.Method.Name == "DefaultIfEmpty" && mcs.Arguments.Count == 1 &&
                (mcs.Method.DeclaringType == typeof(Queryable) || mcs.Method.DeclaringType == typeof(Enumerable)))
            {
                collection = mcs.Arguments[0];
                defaultIfEmpty = true;
            }

            ProjectionExpression collectionProjection = (ProjectionExpression)this.VisitSequence(collection);
            bool isTable = collectionProjection.Source.From is TableExpression;
            JoinType joinType = isTable ? JoinType.CrossJoin : defaultIfEmpty ? JoinType.OuterApply : JoinType.CrossApply;
            if (joinType == JoinType.OuterApply)
            {
                collectionProjection = collectionProjection.AddOuterJoinTest();
            }
            JoinExpression join = new JoinExpression(joinType, projection.Source, collectionProjection.Source, null);

            var alias = this.GetNextAlias();
            ProjectedColumns pc;
            if (resultSelector == null)
            {
                pc = this.ProjectColumns(collectionProjection.Projector, alias, projection.Source.Alias, collectionProjection.Source.Alias);
            }
            else
            {
                this.map[resultSelector.Parameters[0]] = projection.Projector;
                this.map[resultSelector.Parameters[1]] = collectionProjection.Projector;
                Expression result = this.Visit(resultSelector.Body);
                pc = this.ProjectColumns(result, alias, projection.Source.Alias, collectionProjection.Source.Alias);
            }
            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, join, null),
                pc.Projector
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resultType"></param>
        /// <param name="outerSource"></param>
        /// <param name="innerSource"></param>
        /// <param name="outerKey"></param>
        /// <param name="innerKey"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        protected virtual Expression BindJoin(Type resultType, Expression outerSource, Expression innerSource, LambdaExpression outerKey, LambdaExpression innerKey, LambdaExpression resultSelector)
        {
            ProjectionExpression outerProjection = this.VisitSequence(outerSource);
            ProjectionExpression innerProjection = this.VisitSequence(innerSource);
            this.map[outerKey.Parameters[0]] = outerProjection.Projector;
            Expression outerKeyExpr = this.Visit(outerKey.Body);
            this.map[innerKey.Parameters[0]] = innerProjection.Projector;
            Expression innerKeyExpr = this.Visit(innerKey.Body);
            this.map[resultSelector.Parameters[0]] = outerProjection.Projector;
            this.map[resultSelector.Parameters[1]] = innerProjection.Projector;
            Expression resultExpr = this.Visit(resultSelector.Body);
            JoinExpression join = new JoinExpression(JoinType.InnerJoin, outerProjection.Source, innerProjection.Source, Expression.Equal(outerKeyExpr, innerKeyExpr));
            var alias = this.GetNextAlias();
            ProjectedColumns pc = this.ProjectColumns(resultExpr, alias, outerProjection.Source.Alias, innerProjection.Source.Alias);
            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, join, null),
                pc.Projector
                );
        }

        List<OrderExpression> thenBys;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resultType"></param>
        /// <param name="source"></param>
        /// <param name="orderSelector"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        protected virtual Expression BindOrderBy(Type resultType, Expression source, LambdaExpression orderSelector, OrderType orderType)
        {
            List<OrderExpression> myThenBys = this.thenBys;
            this.thenBys = null;
            ProjectionExpression projection = this.VisitSequence(source);

            this.map[orderSelector.Parameters[0]] = projection.Projector;
            List<OrderExpression> orderings = new List<OrderExpression>();
            orderings.Add(new OrderExpression(orderType, this.Visit(orderSelector.Body)));

            if (myThenBys != null)
            {
                for (int i = myThenBys.Count - 1; i >= 0; i--)
                {
                    OrderExpression tb = myThenBys[i];
                    LambdaExpression lambda = (LambdaExpression)tb.Expression;
                    this.map[lambda.Parameters[0]] = projection.Projector;
                    orderings.Add(new OrderExpression(tb.OrderType, this.Visit(lambda.Body)));
                }
            }

            var alias = this.GetNextAlias();
            ProjectedColumns pc = this.ProjectColumns(projection.Projector, alias, projection.Source.Alias);
            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, projection.Source, null, orderings.AsReadOnly(), null),
                pc.Projector
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="orderSelector"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        protected virtual Expression BindThenBy(Expression source, LambdaExpression orderSelector, OrderType orderType)
        {
            if (this.thenBys == null)
            {
                this.thenBys = new List<OrderExpression>();
            }
            this.thenBys.Add(new OrderExpression(orderType, orderSelector));
            return this.Visit(source);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="elementSelector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        protected virtual Expression BindGroupBy(Expression source, LambdaExpression keySelector, LambdaExpression elementSelector, LambdaExpression resultSelector)
        {
            ProjectionExpression projection = this.VisitSequence(source);

            this.map[keySelector.Parameters[0]] = projection.Projector;
            Expression keyExpr = this.Visit(keySelector.Body);

            Expression elemExpr = projection.Projector;
            if (elementSelector != null)
            {
                this.map[elementSelector.Parameters[0]] = projection.Projector;
                elemExpr = this.Visit(elementSelector.Body);
            }

            // Use ProjectColumns to get group-by expressions from key expression
            ProjectedColumns keyProjection = this.ProjectColumns(keyExpr, projection.Source.Alias, projection.Source.Alias);
            IEnumerable<Expression> groupExprs = keyProjection.Columns.Select(c => c.Expression);

            // make duplicate of source query as basis of element subquery by visiting the source again
            ProjectionExpression subqueryBasis = this.VisitSequence(source);

            // recompute key columns for group expressions relative to subquery (need these for doing the correlation predicate)
            this.map[keySelector.Parameters[0]] = subqueryBasis.Projector;
            Expression subqueryKey = this.Visit(keySelector.Body);

            // use same projection trick to get group-by expressions based on subquery
            ProjectedColumns subqueryKeyPC = this.ProjectColumns(subqueryKey, subqueryBasis.Source.Alias, subqueryBasis.Source.Alias);
            IEnumerable<Expression> subqueryGroupExprs = subqueryKeyPC.Columns.Select(c => c.Expression);
            Expression subqueryCorrelation = this.BuildPredicateWithNullsEqual(subqueryGroupExprs, groupExprs);

            // compute element based on duplicated subquery
            Expression subqueryElemExpr = subqueryBasis.Projector;
            if (elementSelector != null)
            {
                this.map[elementSelector.Parameters[0]] = subqueryBasis.Projector;
                subqueryElemExpr = this.Visit(elementSelector.Body);
            }

            // build subquery that projects the desired element
            var elementAlias = this.GetNextAlias();
            ProjectedColumns elementPC = this.ProjectColumns(subqueryElemExpr, elementAlias, subqueryBasis.Source.Alias);
            ProjectionExpression elementSubquery =
                new ProjectionExpression(
                    new SelectExpression(elementAlias, elementPC.Columns, subqueryBasis.Source, subqueryCorrelation),
                    elementPC.Projector
                    );

            var alias = this.GetNextAlias();

            // make it possible to tie aggregates back to this group-by
            GroupByInfo info = new GroupByInfo(alias, elemExpr);
            this.groupByMap.Add(elementSubquery, info);

            Expression resultExpr;
            if (resultSelector != null)
            {
                Expression saveGroupElement = this.currentGroupElement;
                this.currentGroupElement = elementSubquery;
                // compute result expression based on key & element-subquery
                this.map[resultSelector.Parameters[0]] = keyProjection.Projector;
                this.map[resultSelector.Parameters[1]] = elementSubquery;
                resultExpr = this.Visit(resultSelector.Body);
                this.currentGroupElement = saveGroupElement;
            }
            else
            {
                // result must be IGrouping<K,E>
                resultExpr = 
                    Expression.New(
                        typeof(Grouping<,>).MakeGenericType(keyExpr.Type, subqueryElemExpr.Type).GetConstructors()[0],
                        new Expression[] { keyExpr, elementSubquery }
                        );
            }

            ProjectedColumns pc = this.ProjectColumns(resultExpr, alias, projection.Source.Alias);

            // make it possible to tie aggregates back to this group-by
            Expression projectedElementSubquery = ((NewExpression)pc.Projector).Arguments[1];
            this.groupByMap.Add(projectedElementSubquery, info);

            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, projection.Source, null, null, groupExprs),
                pc.Projector
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source1"></param>
        /// <param name="source2"></param>
        /// <returns></returns>
        private Expression BuildPredicateWithNullsEqual(IEnumerable<Expression> source1, IEnumerable<Expression> source2)
        {
            IEnumerator<Expression> en1 = source1.GetEnumerator();
            IEnumerator<Expression> en2 = source2.GetEnumerator();
            Expression result = null;
            while (en1.MoveNext() && en2.MoveNext())
            {
                Expression compare =
                    Expression.Or(
                        Expression.And(new IsNullExpression(en1.Current), new IsNullExpression(en2.Current)),
                        Expression.Equal(en1.Current, en2.Current)
                        );
                result = (result == null) ? compare : Expression.And(result, compare);
            }
            return result;
        }

        Expression currentGroupElement;

        /// <summary>
        /// 
        /// </summary>
        class GroupByInfo
        {
            internal TableAlias Alias { get; private set; }
            internal Expression Element { get; private set; }
            internal GroupByInfo(TableAlias alias, Expression element)
            {
                this.Alias = alias;
                this.Element = element;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private AggregateType GetAggregateType(string methodName)
        {
            switch (methodName)
            {
                case "Count": return AggregateType.Count;
                case "Min": return AggregateType.Min;
                case "Max": return AggregateType.Max;
                case "Sum": return AggregateType.Sum;
                case "Average": return AggregateType.Average;
                default: throw new Exception(string.Format("Unknown aggregate type: {0}", methodName));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aggregateType"></param>
        /// <returns></returns>
        private bool HasPredicateArg(AggregateType aggregateType)
        {
            return aggregateType == AggregateType.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="method"></param>
        /// <param name="argument"></param>
        /// <param name="isRoot"></param>
        /// <returns></returns>
        private Expression BindAggregate(Expression source, MethodInfo method, LambdaExpression argument, bool isRoot)
        {
            Type returnType = method.ReturnType;
            AggregateType aggType = this.GetAggregateType(method.Name);
            bool hasPredicateArg = this.HasPredicateArg(aggType);
            bool isDistinct = false;
            bool argumentWasPredicate = false;
            bool useAlternateArg = false;

            // check for distinct
            MethodCallExpression mcs = source as MethodCallExpression;
            if (mcs != null && !hasPredicateArg && argument == null)
            {
                if (mcs.Method.Name == "Distinct" && mcs.Arguments.Count == 1 &&
                    (mcs.Method.DeclaringType == typeof(Queryable) || mcs.Method.DeclaringType == typeof(Enumerable)))
                {
                    source = mcs.Arguments[0];
                    isDistinct = true;
                }
            }

            if (argument != null && hasPredicateArg)
            {
                // convert query.Count(predicate) into query.Where(predicate).Count()
                source = Expression.Call(typeof(Queryable), "Where", method.GetGenericArguments(), source, argument);
                argument = null;
                argumentWasPredicate = true;
            }

            ProjectionExpression projection = this.VisitSequence(source);

            Expression argExpr = null;
            if (argument != null)
            {
                this.map[argument.Parameters[0]] = projection.Projector;
                argExpr = this.Visit(argument.Body);
            }
            else if (!hasPredicateArg || useAlternateArg)
            {
                argExpr = projection.Projector;
            }

            var alias = this.GetNextAlias();
            var pc = this.ProjectColumns(projection.Projector, alias, projection.Source.Alias);
            Expression aggExpr = new AggregateExpression(returnType, aggType, argExpr, isDistinct);
            SelectExpression select = new SelectExpression(alias, new ColumnDeclaration[] { new ColumnDeclaration("", aggExpr) }, projection.Source, null);

            if (isRoot)
            {
                ParameterExpression p = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(aggExpr.Type), "p");
                LambdaExpression gator = Expression.Lambda(Expression.Call(typeof(Enumerable), "Single", new Type[] { returnType }, p), p);
                return new ProjectionExpression(select, new ColumnExpression(returnType, alias, ""), gator);
            }

            ScalarExpression subquery = new ScalarExpression(returnType, select);

            // if we can find the corresponding group-info we can build a special AggregateSubquery node that will enable us to 
            // optimize the aggregate expression later using AggregateRewriter
            GroupByInfo info;
            if (!argumentWasPredicate && this.groupByMap.TryGetValue(projection, out info))
            {
                // use the element expression from the group-by info to rebind the argument so the resulting expression is one that 
                // would be legal to add to the columns in the select expression that has the corresponding group-by clause.
                if (argument != null)
                {
                    this.map[argument.Parameters[0]] = info.Element;
                    argExpr = this.Visit(argument.Body);
                }
                else if (!hasPredicateArg || useAlternateArg)
                {
                    argExpr = info.Element;
                }
                aggExpr = new AggregateExpression(returnType, aggType, argExpr, isDistinct);

                // check for easy to optimize case.  If the projection that our aggregate is based on is really the 'group' argument from
                // the query.GroupBy(xxx, (key, group) => yyy) method then whatever expression we return here will automatically
                // become part of the select expression that has the group-by clause, so just return the simple aggregate expression.
                if (projection == this.currentGroupElement)
                    return aggExpr;

                return new AggregateSubqueryExpression(info.Alias, aggExpr, subquery);
            }

            return subquery;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private Expression BindDistinct(Expression source)
        {
            ProjectionExpression projection = this.VisitSequence(source);
            SelectExpression select = projection.Source;
            var alias = this.GetNextAlias();
            ProjectedColumns pc = this.ProjectColumns(projection.Projector, alias, projection.Source.Alias);
            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, projection.Source, null, null, null, true, null, null),
                pc.Projector
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        private Expression BindTake(Expression source, Expression take)
        {
            ProjectionExpression projection = this.VisitSequence(source);
            take = this.Visit(take);
            SelectExpression select = projection.Source;
            var alias = this.GetNextAlias();
            ProjectedColumns pc = this.ProjectColumns(projection.Projector, alias, projection.Source.Alias);
            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, projection.Source, null, null, null, false, null, take),
                pc.Projector
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        private Expression BindSkip(Expression source, Expression skip)
        {
            ProjectionExpression projection = this.VisitSequence(source);
            skip = this.Visit(skip);
            SelectExpression select = projection.Source;
            var alias = this.GetNextAlias();
            ProjectedColumns pc = this.ProjectColumns(projection.Projector, alias, projection.Source.Alias);
            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, projection.Source, null, null, null, false, skip, null),
                pc.Projector
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <param name="kind"></param>
        /// <param name="isRoot"></param>
        /// <returns></returns>
        private Expression BindFirst(Expression source, LambdaExpression predicate, string kind, bool isRoot)
        {
            ProjectionExpression projection = this.VisitSequence(source);
            Expression where = null;
            if (predicate != null)
            {
                this.map[predicate.Parameters[0]] = projection.Projector;
                where = this.Visit(predicate.Body);
            }
            Expression take = kind.StartsWith("First") ? Expression.Constant(1) : null;
            if (take != null || where != null)
            {
                var alias = this.GetNextAlias();
                ProjectedColumns pc = this.ProjectColumns(projection.Projector, alias, projection.Source.Alias);
                projection = new ProjectionExpression(
                    new SelectExpression(alias, pc.Columns, projection.Source, where, null, null, false, null, take),
                    pc.Projector
                    );
            }
            if (isRoot)
            {
                Type elementType = projection.Projector.Type;
                ParameterExpression p = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(elementType), "p");
                LambdaExpression gator = Expression.Lambda(Expression.Call(typeof(Enumerable), kind, new Type[] { elementType }, p), p);
                return new ProjectionExpression(projection.Source, projection.Projector, gator);
            }
            return projection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="method"></param>
        /// <param name="predicate"></param>
        /// <param name="isRoot"></param>
        /// <returns></returns>
        private Expression BindAnyAll(Expression source, MethodInfo method, LambdaExpression predicate, bool isRoot)
        {
            bool isAll = method.Name == "All";
            ConstantExpression constSource = source as ConstantExpression;
            if (constSource != null && !IsQuery(constSource))
            {
                System.Diagnostics.Debug.Assert(!isRoot);
                Expression where = null;
                foreach (object value in (IEnumerable)constSource.Value)
                {
                    Expression expr = Expression.Invoke(predicate, Expression.Constant(value, predicate.Parameters[0].Type));
                    if (where == null)
                    {
                        where = expr;
                    }
                    else if (isAll)
                    {
                        where = Expression.And(where, expr);
                    }
                    else
                    {
                        where = Expression.Or(where, expr);
                    }
                }
                return this.Visit(where);
            }
            else
            {
                if (isAll)
                {
                    predicate = Expression.Lambda(Expression.Not(predicate.Body), predicate.Parameters.ToArray());
                }
                if (predicate != null)
                {
                    source = Expression.Call(typeof(Queryable), "Where", method.GetGenericArguments(), source, predicate);
                }
                ProjectionExpression projection = this.VisitSequence(source);
                Expression result = new ExistsExpression(projection.Source);
                if (isAll)
                {
                    result = Expression.Not(result);
                }
                if (isRoot)
                {
                    return GetSingletonSequence(result, "SingleOrDefault");
                }
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="match"></param>
        /// <param name="isRoot"></param>
        /// <returns></returns>
        private Expression BindContains(Expression source, Expression match, bool isRoot)
        {
            ConstantExpression constSource = source as ConstantExpression;
            if (constSource != null && !IsQuery(constSource))
            {
                System.Diagnostics.Debug.Assert(!isRoot);
                List<Expression> values = new List<Expression>();
                foreach (object value in (IEnumerable)constSource.Value)
                {
                    values.Add(Expression.Constant(Convert.ChangeType(value, match.Type), match.Type));
                }
                match = this.Visit(match);
                return new InExpression(match, values);
            }
            else
            {
                ProjectionExpression projection = this.VisitSequence(source);
                match = this.Visit(match);
                Expression result = new InExpression(match, projection.Source);
                if (isRoot)
                {
                    return this.GetSingletonSequence(result, "SingleOrDefault");
                }
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="aggregator"></param>
        /// <returns></returns>
        private Expression GetSingletonSequence(Expression expr, string aggregator)
        {
            ParameterExpression p = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(expr.Type), "p");
            LambdaExpression gator = null;
            if (aggregator != null)
            {
                gator = Expression.Lambda(Expression.Call(typeof(Enumerable), aggregator, new Type[] { expr.Type }, p), p);
            }
            var alias = this.GetNextAlias();
            SelectExpression select = new SelectExpression(alias, new[] { new ColumnDeclaration("value", expr) }, null, null);
            return new ProjectionExpression(select, new ColumnExpression(expr.Type, alias, "value"), gator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private bool IsQuery(Expression expression)
        {
            return
                expression.Type.IsGenericType
                && typeof(IQueryable<>).MakeGenericType(expression.Type.GetGenericArguments()).IsAssignableFrom(expression.Type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (this.IsQuery(c))
            {
                return this.VisitSequence(this.mapping.GetTableQuery(TypeHelper.GetElementType(c.Type)));
            }
            return c;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            Expression e;
            if (this.map.TryGetValue(p, out e))
            {
                return e;
            }
            return p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iv"></param>
        /// <returns></returns>
        protected override Expression VisitInvocation(InvocationExpression iv)
        {
            LambdaExpression lambda = iv.Expression as LambdaExpression;
            if (lambda != null)
            {
                for (int i = 0, n = lambda.Parameters.Count; i < n; i++)
                {
                    this.map[lambda.Parameters[i]] = iv.Arguments[i];
                }
                return this.Visit(lambda.Body);
            }
            return base.VisitInvocation(iv);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter && this.IsQuery(m))
            {
                return this.VisitSequence(this.mapping.GetTableQuery(TypeHelper.GetElementType(m.Type)));
            }
            Expression source = this.Visit(m.Expression);

            Expression result = BindMember(source, m.Member);
            MemberExpression mex = result as MemberExpression;
            if (mex != null && mex.Member == m.Member && mex.Expression == m.Expression)
            {
                return m;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        internal static Expression BindMember(Expression source, MemberInfo member)
        {
            switch (source.NodeType)
            {
                case ExpressionType.MemberInit:
                    MemberInitExpression min = (MemberInitExpression)source;
                    for (int i = 0, n = min.Bindings.Count; i < n; i++)
                    {
                        MemberAssignment assign = min.Bindings[i] as MemberAssignment;
                        if (assign != null && MembersMatch(assign.Member, member))
                        {
                            return assign.Expression;
                        }
                    }
                    break;

                case ExpressionType.New:
                    NewExpression nex = (NewExpression)source;
                    if (nex.Members != null)
                    {
                        for (int i = 0, n = nex.Members.Count; i < n; i++)
                        {
                            if (MembersMatch(nex.Members[i], member))
                            {
                                return nex.Arguments[i];
                            }
                        }
                    }
                    else if (nex.Type.IsGenericType && nex.Type.GetGenericTypeDefinition() == typeof(Grouping<,>))
                    {
                        if (member.Name == "Key")
                        {
                            return nex.Arguments[0];
                        }
                    }
                    break;

                case (ExpressionType)DbExpressionType.Projection:
                    // member access on a projection turns into a new projection w/ member access applied
                    ProjectionExpression proj = (ProjectionExpression)source;
                    Expression newProjector = BindMember(proj.Projector, member);
                    return new ProjectionExpression(proj.Source, newProjector);

                case (ExpressionType)DbExpressionType.OuterJoined:
                    OuterJoinedExpression oj = (OuterJoinedExpression)source;
                    Expression em = BindMember(oj.Expression, member);
                    if (em is ColumnExpression)
                    {
                        return em;
                    }
                    return new OuterJoinedExpression(oj.Test, em);

                case ExpressionType.Conditional:
                    ConditionalExpression cex = (ConditionalExpression)source;
                    return Expression.Condition(cex.Test, BindMember(cex.IfTrue, member), BindMember(cex.IfFalse, member));

                case ExpressionType.Constant:
                    ConstantExpression con = (ConstantExpression)source;
                    if (con.Value == null)
                    {
                        Type memberType = TypeHelper.GetMemberType(member);
                        return Expression.Constant(GetDefault(memberType), memberType);
                    }
                    break;
            }
            return Expression.MakeMemberAccess(source, member);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object GetDefault(Type type)
        {
            if (!type.IsValueType || TypeHelper.IsNullableType(type))
            {
                return null;
            }
            else
            {
                return Activator.CreateInstance(type);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool MembersMatch(MemberInfo a, MemberInfo b)
        {
            if (a == b)
            {
                return true;
            }
            if (a is MethodInfo && b is PropertyInfo)
            {
                return a == ((PropertyInfo)b).GetGetMethod();
            }
            else if (a is PropertyInfo && b is MethodInfo)
            {
                return ((PropertyInfo)a).GetGetMethod() == b;
            }
            return false;
        }
    }
}
