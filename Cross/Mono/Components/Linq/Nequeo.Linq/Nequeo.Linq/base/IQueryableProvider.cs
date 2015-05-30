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
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Nequeo.Linq
{
    /// <summary>
    /// Server query context interface.
    /// </summary>
    public interface IQueryContext
    {
        #region IServerQueryContext
        /// <summary>
        /// The base execution method for all query results.
        /// </summary>
        /// <param name="expression">The expression tree</param>
        /// <param name="IsEnumerable">Is the result enumerable.</param>
        /// <returns>The enumerable result.</returns>
        object Execute(Expression expression, bool IsEnumerable);

        #endregion
    }

    /// <summary>
    /// The generic queryable provider.
    /// </summary>
    /// <typeparam name="T">The querable type to return.</typeparam>
    public class QueryableProvider<T> : IOrderedQueryable<T>
    {
        #region QueryableServer

        #region Constructors
        /// <summary>
        /// This constructor is called by the client to create the data source.
        /// </summary>
        /// <param name="context">The server query context interface.</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public QueryableProvider(IQueryContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            Provider = new QueryProvider(context);
            Expression = Expression.Constant(this);
            Context = context;
        }

        /// <summary>
        /// This constructor is called by the client to create the data source.
        /// </summary>
        /// <param name="context">The server query context interface.</param>
        /// <param name="provider">The query provider to use.</param>
        /// <param name="expression">The expression to evaluate.</param>
        /// <exception cref="ArgumentNullException">context</exception>
        /// <exception cref="ArgumentNullException">provider</exception>
        /// <exception cref="ArgumentNullException">expression</exception>
        public QueryableProvider(IQueryContext context, IQueryProvider provider, Expression expression)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (provider == null)
                throw new ArgumentNullException("provider");

            if (expression == null)
                throw new ArgumentNullException("expression");

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
                throw new ArgumentOutOfRangeException("expression");

            Provider = provider;
            Expression = expression;
            Context = context;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets, the query provider.
        /// </summary>
        public IQueryProvider Provider { get; private set; }

        /// <summary>
        /// Gets, the expression tree.
        /// </summary>
        public Expression Expression { get; private set; }

        /// <summary>
        /// Gets, the queryable server return type.
        /// </summary>
        public Type ElementType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Gets, the server query context.
        /// </summary>
        public IQueryContext Context { get; private set; }

        #endregion

        #region Enumerators
        /// <summary>
        /// Gets the enumertor of the queryable provider.
        /// </summary>
        /// <returns>The enumarator of the queryable provider.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return (Provider.Execute<IEnumerable<T>>(Expression)).GetEnumerator();
        }

        /// <summary>
        /// Gets the non generic enumertor of the queryable provider.
        /// </summary>
        /// <returns>The enumarator of the queryable provider.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (Provider.Execute<System.Collections.IEnumerable>(Expression)).GetEnumerator();
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// The general query provider.
    /// </summary>
    public class QueryProvider : IQueryProvider
    {
        #region ServerQueryProvider

        #region Constructors
        /// <summary>
        /// This constructor is called by the client to create the data source.
        /// </summary>
        public QueryProvider() {}

        /// <summary>
        /// This constructor is called by the client to create the data source.
        /// </summary>
        /// <param name="context">The server query context interface.</param>
        public QueryProvider(IQueryContext context)
        {
            _context = context;
        }
        #endregion

        #region Fields
        private IQueryContext _context = null;
        #endregion

        #region Properites
        /// <summary>
        /// Gets, the server query context.
        /// </summary>
        public IQueryContext Context
        {
            get { return _context; }
            set { _context = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Create the query from the expression.
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The queryable result collection.</returns>
        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(
                    typeof(QueryableProvider<>).MakeGenericType(elementType), new object[] { _context, this, expression });
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        /// Queryable's collection-returning standard query operators call this method.
        /// </summary>
        /// <typeparam name="TResult">The type to return.</typeparam>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The queryable type result collection.</returns>
        public IQueryable<TResult> CreateQuery<TResult>(Expression expression)
        {
            return new QueryableProvider<TResult>(_context, this, expression);
        }

        /// <summary>
        /// The queryable result
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The result.</returns>
        /// <remarks>The execute method is called in the The server query context interface. The
        /// code in this custom method is used to evaluate the expression.</remarks>
        public object Execute(Expression expression)
        {
            return _context.Execute(expression, false);
        }

        /// <summary>
        /// Queryable's "single value" standard query operators call this method.
        /// It is also called from QueryableServer.GetEnumerator().
        /// </summary>
        /// <typeparam name="TResult">The type to return.</typeparam>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The result type.</returns>
        /// <remarks>The execute method is called in the The server query context interface. The
        /// code in this custom method is used to evaluate the expression.</remarks>
        public TResult Execute<TResult>(Expression expression)
        {
            bool IsEnumerable = (typeof(TResult).Name == "IEnumerable`1");
            return (TResult)_context.Execute(expression, IsEnumerable);
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Inner most skip finder
    /// </summary>
    public class InnermostSkipFinder : ExpressionVisitor
    {
        #region InnermostSkipFinder

        #region Fields
        private MethodCallExpression innermostSkipExpression;
        #endregion

        #region Methods
        /// <summary>
        /// Get the inner most skip.
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The method call expression.</returns>
        public MethodCallExpression GetInnermostSkip(Expression expression)
        {
            Visit(expression);
            return innermostSkipExpression;
        }

        /// <summary>
        /// Protected override vist method call expression.
        /// </summary>
        /// <param name="expression">The method call expression.</param>
        /// <returns>The new expression.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression.Method.Name == "Skip")
                innermostSkipExpression = expression;

            Visit(expression.Arguments[0]);

            return expression;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Inner most take finder
    /// </summary>
    public class InnermostTakeFinder : ExpressionVisitor
    {
        #region InnermostTakeFinder

        #region Fields
        private MethodCallExpression innermostTakeExpression;
        #endregion

        #region Methods
        /// <summary>
        /// Get the inner most skip.
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The method call expression.</returns>
        public MethodCallExpression GetInnermostTake(Expression expression)
        {
            Visit(expression);
            return innermostTakeExpression;
        }

        /// <summary>
        /// Protected override vist method call expression.
        /// </summary>
        /// <param name="expression">The method call expression.</param>
        /// <returns>The new expression.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression.Method.Name == "Take")
                innermostTakeExpression = expression;

            Visit(expression.Arguments[0]);

            return expression;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Inner most where finder
    /// </summary>
    public class InnermostWhereFinder : ExpressionVisitor
    {
        #region InnermostWhereFinder

        #region Fields
        private MethodCallExpression innermostWhereExpression;
        #endregion

        #region Methods
        /// <summary>
        /// Get the inner most where.
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The method call expression.</returns>
        public MethodCallExpression GetInnermostWhere(Expression expression)
        {
            Visit(expression);
            return innermostWhereExpression;
        }

        /// <summary>
        /// Protected override vist method call expression.
        /// </summary>
        /// <param name="expression">The method call expression.</param>
        /// <returns>The new expression.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression.Method.Name == "Where")
                innermostWhereExpression = expression;

            Visit(expression.Arguments[0]);

            return expression;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Inner most count finder
    /// </summary>
    public class InnermostCountFinder : ExpressionVisitor
    {
        #region InnermostCountFinder

        #region Fields
        private MethodCallExpression innermostCountExpression;
        #endregion

        #region Methods
        /// <summary>
        /// Get the inner most count.
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The method call expression.</returns>
        public MethodCallExpression GetInnermostCount(Expression expression)
        {
            Visit(expression);
            return innermostCountExpression;
        }

        /// <summary>
        /// Protected override vist method call expression.
        /// </summary>
        /// <param name="expression">The method call expression.</param>
        /// <returns>The new expression.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression.Method.Name == "Count")
                innermostCountExpression = expression;

            Visit(expression.Arguments[0]);

            return expression;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Inner most any finder
    /// </summary>
    public class InnermostAnyFinder : ExpressionVisitor
    {
        #region InnermostAnyFinder

        #region Fields
        private MethodCallExpression innermostAnyExpression;
        #endregion

        #region Methods
        /// <summary>
        /// Get the inner most count.
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The method call expression.</returns>
        public MethodCallExpression GetInnermostAny(Expression expression)
        {
            Visit(expression);
            return innermostAnyExpression;
        }

        /// <summary>
        /// Protected override vist method call expression.
        /// </summary>
        /// <param name="expression">The method call expression.</param>
        /// <returns>The new expression.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression.Method.Name == "Any")
                innermostAnyExpression = expression;

            Visit(expression.Arguments[0]);

            return expression;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Inner most first finder
    /// </summary>
    public class InnermostFirstFinder : ExpressionVisitor
    {
        #region InnermostWhereFinder

        #region Fields
        private MethodCallExpression innermostFirstExpression;
        #endregion

        #region Methods
        /// <summary>
        /// Get the inner most first.
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The method call expression.</returns>
        public MethodCallExpression GetInnermostFirst(Expression expression)
        {
            Visit(expression);
            return innermostFirstExpression;
        }

        /// <summary>
        /// Protected override vist method call expression.
        /// </summary>
        /// <param name="expression">The method call expression.</param>
        /// <returns>The new expression.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression.Method.Name == "First")
                innermostFirstExpression = expression;

            Visit(expression.Arguments[0]);

            return expression;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Inner most select finder
    /// </summary>
    public class InnermostSelectFinder : ExpressionVisitor
    {
        #region InnermostSelectFinder

        #region Fields
        private MethodCallExpression innermostSelectExpression;
        #endregion

        #region Methods
        /// <summary>
        /// Get the inner most select.
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The method call expression.</returns>
        public MethodCallExpression GetInnermostSelect(Expression expression)
        {
            Visit(expression);
            return innermostSelectExpression;
        }

        /// <summary>
        /// Protected override vist method call expression.
        /// </summary>
        /// <param name="expression">The method call expression.</param>
        /// <returns>The new expression.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression.Method.Name == "Select")
                innermostSelectExpression = expression;

            Visit(expression.Arguments[0]);

            return expression;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Inner most orderby finder
    /// </summary>
    public class InnermostOrderByFinder : ExpressionVisitor
    {
        #region InnermostOrderByFinder

        #region Fields
        private MethodCallExpression innermostOrderByExpression;
        #endregion

        #region Methods
        /// <summary>
        /// Get the inner most orderby.
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The method call expression.</returns>
        public MethodCallExpression GetInnermostOrderBy(Expression expression)
        {
            Visit(expression);
            return innermostOrderByExpression;
        }

        /// <summary>
        /// Protected override vist method call expression.
        /// </summary>
        /// <param name="expression">The method call expression.</param>
        /// <returns>The new expression.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression.Method.Name == "OrderBy")
                innermostOrderByExpression = expression;

            Visit(expression.Arguments[0]);

            return expression;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Inner most thenby finder
    /// </summary>
    public class InnermostThenByFinder : ExpressionVisitor
    {
        #region InnermostThenByFinder

        #region Fields
        private MethodCallExpression innermostThenByExpression;
        #endregion

        #region Methods
        /// <summary>
        /// Get the inner most orderby.
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The method call expression.</returns>
        public MethodCallExpression GetInnermostThenBy(Expression expression)
        {
            Visit(expression);
            return innermostThenByExpression;
        }

        /// <summary>
        /// Protected override vist method call expression.
        /// </summary>
        /// <param name="expression">The method call expression.</param>
        /// <returns>The new expression.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression.Method.Name == "ThenBy")
                innermostThenByExpression = expression;

            Visit(expression.Arguments[0]);

            return expression;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Inner most orderby descending finder
    /// </summary>
    public class InnermostOrderByDescendingFinder : ExpressionVisitor
    {
        #region InnermostOrderByDescendingFinder

        #region Fields
        private MethodCallExpression innermostOrderByDescendingExpression;
        #endregion

        #region Methods
        /// <summary>
        /// Get the inner most orderby.
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The method call expression.</returns>
        public MethodCallExpression GetInnermostOrderByDescending(Expression expression)
        {
            Visit(expression);
            return innermostOrderByDescendingExpression;
        }

        /// <summary>
        /// Protected override vist method call expression.
        /// </summary>
        /// <param name="expression">The method call expression.</param>
        /// <returns>The new expression.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression.Method.Name == "OrderByDescending")
                innermostOrderByDescendingExpression = expression;

            Visit(expression.Arguments[0]);

            return expression;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Inner most thenby descending finder
    /// </summary>
    public class InnermostThenByDescendingFinder : ExpressionVisitor
    {
        #region InnermostThenByDescendingFinder

        #region Fields
        private MethodCallExpression innermostThenByDescendingExpression;
        #endregion

        #region Methods
        /// <summary>
        /// Get the inner most orderby.
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The method call expression.</returns>
        public MethodCallExpression GetInnermostThenByDescending(Expression expression)
        {
            Visit(expression);
            return innermostThenByDescendingExpression;
        }

        /// <summary>
        /// Protected override vist method call expression.
        /// </summary>
        /// <param name="expression">The method call expression.</param>
        /// <returns>The new expression.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression.Method.Name == "ThenByDescending")
                innermostThenByDescendingExpression = expression;

            Visit(expression.Arguments[0]);

            return expression;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Expression tree modifier.
    /// </summary>
    /// <typeparam name="TData">The return data type.</typeparam>
    public class ExpressionTreeModifier<TData> : ExpressionVisitor
    {
        #region ExpressionTreeModifier

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="query">The query expression.</param>
        public ExpressionTreeModifier(IQueryable<TData> query)
        {
            this._query = query;
        }
        #endregion

        #region Fields
        private IQueryable<TData> _query;
        #endregion

        #region Methods
        /// <summary>
        /// Copy and modify the expression.
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The new expression.</returns>
        public Expression CopyAndModify(Expression expression)
        {
            return this.Visit(expression);
        }

        /// <summary>
        /// Protected override visit contant expression.
        /// </summary>
        /// <param name="constant">The constant expression.</param>
        /// <returns>The new expression.</returns>
        protected override Expression VisitConstant(ConstantExpression constant)
        {
            // Replace the constant QueryableTerraServerData arg with the queryable Place collection.
            if (constant.Type == typeof(QueryableProvider<TData>))
                return Expression.Constant(this._query);
            else
                return constant;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Custome invalid query exeception.
    /// </summary>
    public class InvalidQueryException : System.Exception
    {
        #region InvalidQueryException

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="message">The exception message</param>
        public InvalidQueryException(string message)
        {
            this.message = message + " ";
        }
        #endregion

        #region Fields
        private string message;
        #endregion

        #region Methods
        /// <summary>
        /// Public override the message.
        /// </summary>
        public override string Message
        {
            get
            {
                return "The client query is invalid: " + message;
            }
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Expression evaluator.
    /// </summary>
    public static class Evaluator
    {
        #region Evaluator

        #region Static Methods
        /// <summary>
        /// Performs evaluation and replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <param name="fnCanBeEvaluated">A function that decides whether a given expression node can be part of the local function.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
        {
            return new SubtreeEvaluator(new Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
        }

        /// <summary>
        /// Performs evaluation and replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression)
        {
            return PartialEval(expression, Evaluator.CanBeEvaluatedLocally);
        }

        /// <summary>
        /// Can the expression be evaluated locally.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Treu if it can else false.</returns>
        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter;
        }
        #endregion

        /// <summary>
        /// Evaluates and replaces sub-trees when first candidate is reached (top-down)
        /// </summary>
        class SubtreeEvaluator : ExpressionVisitor
        {
            #region SubtreeEvaluator

            #region Constructors
            /// <summary>
            /// Internal constructor.
            /// </summary>
            /// <param name="candidates">Hash set expression values.</param>
            internal SubtreeEvaluator(HashSet<Expression> candidates)
            {
                this.candidates = candidates;
            }
            #endregion

            #region Fields
            HashSet<Expression> candidates;
            #endregion

            #region Methods
            /// <summary>
            /// Evalulate the expression to the visit method.
            /// </summary>
            /// <param name="exp">The expression.</param>
            /// <returns>The new expression.</returns>
            internal Expression Eval(Expression exp)
            {
                return this.Visit(exp);
            }

            /// <summary>
            /// Protected override vist expression
            /// </summary>
            /// <param name="exp">The expression.</param>
            /// <returns>The new expression.</returns>
            protected override Expression Visit(Expression exp)
            {
                if (exp == null)
                {
                    return null;
                }
                if (this.candidates.Contains(exp))
                {
                    return this.Evaluate(exp);
                }
                return base.Visit(exp);
            }

            /// <summary>
            /// Evalute the expression.
            /// </summary>
            /// <param name="e">The expression.</param>
            /// <returns>The complied expression.</returns>
            private Expression Evaluate(Expression e)
            {
                if (e.NodeType == ExpressionType.Constant)
                {
                    return e;
                }
                LambdaExpression lambda = Expression.Lambda(e);
                Delegate fn = lambda.Compile();
                return Expression.Constant(fn.DynamicInvoke(null), e.Type);
            }
            #endregion

            #endregion
        }

        /// <summary>
        /// Performs bottom-up analysis to determine which nodes can possibly
        /// be part of an evaluated sub-tree.
        /// </summary>
        class Nominator : ExpressionVisitor
        {
            #region Nominator

            #region Constructors
            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="fnCanBeEvaluated">The eveluation delegate.</param>
            internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                this.fnCanBeEvaluated = fnCanBeEvaluated;
            }
            #endregion

            #region Fields
            Func<Expression, bool> fnCanBeEvaluated;
            HashSet<Expression> candidates;
            bool cannotBeEvaluated;
            #endregion

            #region Methods
            /// <summary>
            /// Nominate the expression.
            /// </summary>
            /// <param name="expression">The expression tree.</param>
            /// <returns>The hash set of expression values.</returns>
            internal HashSet<Expression> Nominate(Expression expression)
            {
                this.candidates = new HashSet<Expression>();
                this.Visit(expression);
                return this.candidates;
            }

            /// <summary>
            /// Protected override visit method.
            /// </summary>
            /// <param name="expression">The expression tree.</param>
            /// <returns>The new expression tree.</returns>
            protected override Expression Visit(Expression expression)
            {
                if (expression != null)
                {
                    bool saveCannotBeEvaluated = this.cannotBeEvaluated;
                    this.cannotBeEvaluated = false;
                    base.Visit(expression);
                    if (!this.cannotBeEvaluated)
                    {
                        if (this.fnCanBeEvaluated(expression))
                        {
                            this.candidates.Add(expression);
                        }
                        else
                        {
                            this.cannotBeEvaluated = true;
                        }
                    }
                    this.cannotBeEvaluated |= saveCannotBeEvaluated;
                }
                return expression;
            }
            #endregion

            #endregion
        }
        #endregion
    }

    /// <summary>
    /// Result type system evaluator.
    /// </summary>
    public static class TypeSystem
    {
        #region TypeSystem

        #region Methods
        /// <summary>
        /// Get the generic IEnumerable object.
        /// </summary>
        /// <param name="seqType">The current sequance type.</param>
        /// <returns>The type of result.</returns>
        public static Type GetElementType(Type seqType)
        {
            Type ienum = FindIEnumerable(seqType);
            if (ienum == null) return seqType;
            return ienum.GetGenericArguments()[0];
        }

        /// <summary>
        /// Find the generic IEnumerable object.
        /// </summary>
        /// <param name="seqType">The current sequance type.</param>
        /// <returns>The type of result.</returns>
        private static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;

            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());

            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }

            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces != null && ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }

            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }

            return null;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Expression tree evaluation helper.
    /// </summary>
    public class ExpressionTreeHelpers
    {
        #region TypeSystem

        #region Methods
        /// <summary>
        /// Is the member equal value to the expression.
        /// </summary>
        /// <param name="exp">The expression tree.</param>
        /// <param name="declaringType">The type to evaluate to.</param>
        /// <param name="memberName">The member name in the declaring type.</param>
        /// <returns>True else false.</returns>
        public static bool IsMemberEqualsValueExpression(Expression exp, Type declaringType, string memberName)
        {
            if (exp.NodeType != ExpressionType.Equal)
                return false;

            BinaryExpression be = (BinaryExpression)exp;

            // Assert.
            if (ExpressionTreeHelpers.IsSpecificMemberExpression(be.Left, declaringType, memberName) &&
                ExpressionTreeHelpers.IsSpecificMemberExpression(be.Right, declaringType, memberName))
                throw new Exception("Cannot have 'member' == 'member' in an expression!");

            return (ExpressionTreeHelpers.IsSpecificMemberExpression(be.Left, declaringType, memberName) ||
                ExpressionTreeHelpers.IsSpecificMemberExpression(be.Right, declaringType, memberName));
        }

        /// <summary>
        /// Is the specific member an expression.
        /// </summary>
        /// <param name="exp">The expression tree.</param>
        /// <param name="declaringType">The type to evaluate to.</param>
        /// <param name="memberName">The member name in the declaring type.</param>
        /// <returns>True else false.</returns>
        public static bool IsSpecificMemberExpression(Expression exp, Type declaringType, string memberName)
        {
            return ((exp is MemberExpression) &&
                (((MemberExpression)exp).Member.DeclaringType == declaringType) &&
                (((MemberExpression)exp).Member.Name == memberName));
        }

        /// <summary>
        /// Get the value from the binary expression.
        /// </summary>
        /// <param name="exp">The binary expression tree.</param>
        /// <param name="declaringType">The type to evaluate to.</param>
        /// <param name="memberName">The member name in the declaring type.</param>
        /// <returns>True else false.</returns>
        public static string GetValueFromEqualsExpression(BinaryExpression exp, Type declaringType, string memberName)
        {
            if (exp.NodeType != ExpressionType.Equal)
                throw new Exception("The binary expression node type is not the expression type 'Equal'.");

            if (exp.Left.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression me = (MemberExpression)exp.Left;

                if (me.Member.DeclaringType == declaringType && me.Member.Name == memberName)
                {
                    return GetValueFromExpression(exp.Right);
                }
            }
            else if (exp.Right.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression me = (MemberExpression)exp.Right;

                if (me.Member.DeclaringType == declaringType && me.Member.Name == memberName)
                {
                    return GetValueFromExpression(exp.Left);
                }
            }

            // We should have returned by now.
            throw new Exception("Could not get the constant expression value.");
        }

        /// <summary>
        /// Get the constant value of the expression.
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The constant value of the expression.</returns>
        public static string GetValueFromExpression(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Constant)
                return (string)(((ConstantExpression)expression).Value);
            else
                throw new InvalidQueryException(
                    String.Format("The expression type {0} is not supported to obtain a value.", expression.NodeType));
        }
        #endregion

        #endregion
    }
}

