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
using System.Text;
using System.Reflection;

using Nequeo.Threading;

namespace Nequeo.Linq
{
    /// <summary>
    /// Creates a reusable, parameterized representation of a query that caches the execution plan
    /// </summary>
    public static class QueryCompiler
    {
        /// <summary>
        /// Complie the current expression.
        /// </summary>
        /// <param name="query">The lambda expression to complie.</param>
        /// <returns>The delegate function to execute.</returns>
        public static Delegate Compile(LambdaExpression query)
        {
            CompiledQuery cq = new CompiledQuery(query);
            Type dt = query.Type;
            MethodInfo method = dt.GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public);
            ParameterInfo[] parameters = method.GetParameters();
            ParameterExpression[] pexprs = parameters.Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();
            var args = Expression.NewArrayInit(typeof(object), pexprs.Select(p => Expression.Convert(p, typeof(object))).ToArray());
            Expression body = Expression.Convert(Expression.Call(Expression.Constant(cq), "Invoke", Type.EmptyTypes, args), method.ReturnType);
            LambdaExpression e = Expression.Lambda(dt, body, pexprs);
            return e.Compile();
        }

        /// <summary>
        /// Complie the current expression.
        /// </summary>
        /// <typeparam name="D">The delegate function handler type.</typeparam>
        /// <param name="query">The expression to complie.</param>
        /// <returns>The delegate function type.</returns>
        public static D Compile<D>(Expression<D> query)
        {
            return (D)(object)Compile((LambdaExpression)query);
        }

        /// <summary>
        /// Complie the current expression.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="query">The expresion to compile.</param>
        /// <returns>The result delegate function.</returns>
        public static FunctionHandler<TResult> Compile<TResult>(Expression<FunctionHandler<TResult>> query)
        {
            return new CompiledQuery(query).Invoke<TResult>;
        }

        /// <summary>
        /// Complie the current expression.
        /// </summary>
        /// <typeparam name="T1">The first type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="query">The expresion to compile.</param>
        /// <returns>The result delegate function.</returns>
        public static FunctionHandler<TResult, T1> Compile<T1, TResult>(Expression<FunctionHandler<TResult, T1>> query)
        {
            return new CompiledQuery(query).Invoke<T1, TResult>;
        }

        /// <summary>
        /// Complie the current expression.
        /// </summary>
        /// <typeparam name="T1">The first type.</typeparam>
        /// <typeparam name="T2">The second type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="query">The expresion to compile.</param>
        /// <returns>The result delegate function.</returns>
        public static FunctionHandler<TResult, T1, T2> Compile<T1, T2, TResult>(Expression<FunctionHandler<TResult, T1, T2>> query)
        {
            return new CompiledQuery(query).Invoke<T1, T2, TResult>;
        }

        /// <summary>
        /// Complie the current expression.
        /// </summary>
        /// <typeparam name="T1">The first type.</typeparam>
        /// <typeparam name="T2">The second type.</typeparam>
        /// <typeparam name="T3">The third type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="query">The expresion to compile.</param>
        /// <returns>The result delegate function.</returns>
        public static FunctionHandler<TResult, T1, T2, T3> Compile<T1, T2, T3, TResult>(Expression<FunctionHandler<TResult, T1, T2, T3>> query)
        {
            return new CompiledQuery(query).Invoke<T1, T2, T3, TResult>;
        }

        /// <summary>
        /// Complie the current expression.
        /// </summary>
        /// <typeparam name="T1">The first type.</typeparam>
        /// <typeparam name="T2">The second type.</typeparam>
        /// <typeparam name="T3">The third type.</typeparam>
        /// <typeparam name="T4">The fourth type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="query">The expresion to compile.</param>
        /// <returns>The result delegate function.</returns>
        public static FunctionHandler<TResult, T1, T2, T3, T4> Compile<T1, T2, T3, T4, TResult>(Expression<FunctionHandler<TResult, T1, T2, T3, T4>> query)
        {
            return new CompiledQuery(query).Invoke<T1, T2, T3, T4, TResult>;
        }

        /// <summary>
        /// Complie the current expression.
        /// </summary>
        /// <typeparam name="T1">The first type.</typeparam>
        /// <typeparam name="T2">The second type.</typeparam>
        /// <typeparam name="T3">The third type.</typeparam>
        /// <typeparam name="T4">The fourth type.</typeparam>
        /// <typeparam name="T5">The fifth type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="query">The expresion to compile.</param>
        /// <returns>The result delegate function.</returns>
        public static FunctionHandler<TResult, T1, T2, T3, T4, T5> Compile<T1, T2, T3, T4, T5, TResult>(Expression<FunctionHandler<TResult, T1, T2, T3, T4, T5>> query)
        {
            return new CompiledQuery(query).Invoke<T1, T2, T3, T4, T5, TResult>;
        }
    }

    /// <summary>
    /// Compiles any query expression.
    /// </summary>
    internal class CompiledQuery
    {
        LambdaExpression query;
        Delegate fnQuery;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="query">The lambda function expression.</param>
        internal CompiledQuery(LambdaExpression query)
        {
            this.query = query;
        }

        /// <summary>
        /// Compiles the expression through the delegate.
        /// </summary>
        /// <param name="args">The arguments for the method to execute.</param>
        internal void Compile(params object[] args)
        {
            if (this.fnQuery == null)
            {
                // First identify the query provider being used
                Expression body = this.query.Body;
                ConstantExpression root = RootQueryableFinder.Find(body) as ConstantExpression;
                if (root == null && args != null && args.Length > 0)
                {
                    Expression replaced = ExpressionReplacer.ReplaceAll(
                        body,
                        this.query.Parameters.ToArray(),
                        args.Select((a, i) => Expression.Constant(a, this.query.Parameters[i].Type)).ToArray()
                        );
                    body = PartialEvaluator.Eval(replaced);
                    root = RootQueryableFinder.Find(body) as ConstantExpression;
                }
                if (root == null)
                {
                    throw new InvalidOperationException("Could not find query provider");
                }

                // Ask the query provider to compile the query by 'executing' the lambda expression
                IQueryProvider provider = ((IQueryable)root.Value).Provider;
                Delegate result = (Delegate)provider.Execute(this.query);
                System.Threading.Interlocked.CompareExchange(ref this.fnQuery, result, null);
            }
        }

        /// <summary>
        /// Invoke the collection of arguments.
        /// </summary>
        /// <param name="args">The arguments to invoke.</param>
        /// <returns>The function result.</returns>
        public object Invoke(object[] args)
        {
            this.Compile(args);
            try
            {
                return this.fnQuery.DynamicInvoke(args);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        /// Invoke the function, compile the arguments and return the result.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <returns>The result of the function evaluation.</returns>
        internal TResult Invoke<TResult>()
        {
            this.Compile(null);
            return ((FunctionHandler<TResult>)this.fnQuery)();
        }

        /// <summary>
        /// Invoke the function, compile the arguments and return the result.
        /// </summary>
        /// <typeparam name="T1">The first type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="arg">The first argument.</param>
        /// <returns>The result of the function evaluation.</returns>
        internal TResult Invoke<T1, TResult>(T1 arg)
        {
            this.Compile(arg);
            return ((FunctionHandler<TResult, T1>)this.fnQuery)(arg);
        }

        /// <summary>
        /// Invoke the function, compile the arguments and return the result.
        /// </summary>
        /// <typeparam name="T1">The first type.</typeparam>
        /// <typeparam name="T2">The second type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        /// <returns>The result of the function evaluation.</returns>
        internal TResult Invoke<T1, T2, TResult>(T1 arg1, T2 arg2)
        {
            this.Compile(arg1, arg2);
            return ((FunctionHandler<TResult, T1, T2>)this.fnQuery)(arg1, arg2);
        }

        /// <summary>
        /// Invoke the function, compile the arguments and return the result.
        /// </summary>
        /// <typeparam name="T1">The first type.</typeparam>
        /// <typeparam name="T2">The second type.</typeparam>
        /// <typeparam name="T3">The third type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        /// <param name="arg3">The third argument.</param>
        /// <returns>The result of the function evaluation.</returns>
        internal TResult Invoke<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3)
        {
            this.Compile(arg1, arg2, arg3);
            return ((FunctionHandler<TResult, T1, T2, T3>)this.fnQuery)(arg1, arg2, arg3);
        }

        /// <summary>
        /// Invoke the function, compile the arguments and return the result.
        /// </summary>
        /// <typeparam name="T1">The first type.</typeparam>
        /// <typeparam name="T2">The second type.</typeparam>
        /// <typeparam name="T3">The third type.</typeparam>
        /// <typeparam name="T4">The fourth type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        /// <param name="arg3">The third argument.</param>
        /// <param name="arg4">The fourth argument.</param>
        /// <returns>The result of the function evaluation.</returns>
        internal TResult Invoke<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            this.Compile(arg1, arg2, arg3, arg4);
            return ((FunctionHandler<TResult, T1, T2, T3, T4>)this.fnQuery)(arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Invoke the function, compile the arguments and return the result.
        /// </summary>
        /// <typeparam name="T1">The first type.</typeparam>
        /// <typeparam name="T2">The second type.</typeparam>
        /// <typeparam name="T3">The third type.</typeparam>
        /// <typeparam name="T4">The fourth type.</typeparam>
        /// <typeparam name="T5">The fifth type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        /// <param name="arg3">The third argument.</param>
        /// <param name="arg4">The fourth argument.</param>
        /// <param name="arg5">The fifth argument.</param>
        /// <returns>The result of the function evaluation.</returns>
        internal TResult Invoke<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            this.Compile(arg1, arg2, arg3, arg4, arg5);
            return ((FunctionHandler<TResult, T1, T2, T3, T4, T5>)this.fnQuery)(arg1, arg2, arg3, arg4, arg5);
        }
    }

    /// <summary>
    /// Finds the first sub-expression that accesses a Query generic type object.
    /// Finds the first IQueryable generic type expression function.
    /// </summary>
    internal class RootQueryableFinder : ExpressionVisitor
    {
        Expression root;

        /// <summary>
        /// Find the first IQueryable expression.
        /// </summary>
        /// <param name="expression">The current expression</param>
        /// <returns>The iqueryable expression.</returns>
        public static Expression Find(Expression expression)
        {
            RootQueryableFinder finder = new RootQueryableFinder();
            finder.Visit(expression);
            return finder.root;
        }

        /// <summary>
        /// Visit the current expression type.
        /// </summary>
        /// <param name="exp">The expression to examine.</param>
        /// <returns>The iqueryable expression.</returns>
        protected override Expression Visit(Expression exp)
        {
            Expression result = base.Visit(exp);

            // Remember the first sub-expression that produces an IQueryable
            if (this.root == null && result != null && typeof(IQueryable).IsAssignableFrom(result.Type))
            {
                this.root = result;
            }

            return result;
        }
    }

    /// <summary>
    /// Replaces references to one specific instance of an 
    /// expression node with another node
    /// </summary>
    internal class ExpressionReplacer : ExpressionVisitor
    {
        Expression searchFor;
        Expression replaceWith;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="searchFor">The expression to search for.</param>
        /// <param name="replaceWith">The expression to replace with.</param>
        private ExpressionReplacer(Expression searchFor, Expression replaceWith)
        {
            this.searchFor = searchFor;
            this.replaceWith = replaceWith;
        }

        /// <summary>
        /// Replace the expression.
        /// </summary>
        /// <param name="expression">The current expression.</param>
        /// <param name="searchFor">The expression to search for.</param>
        /// <param name="replaceWith">The expression to replace with.</param>
        /// <returns>The expression visited.</returns>
        public static Expression Replace(Expression expression, Expression searchFor, Expression replaceWith)
        {
            return new ExpressionReplacer(searchFor, replaceWith).Visit(expression);
        }

        /// <summary>
        /// Replace all the expression.
        /// </summary>
        /// <param name="expression">The current expression.</param>
        /// <param name="searchFor">The expression to search for.</param>
        /// <param name="replaceWith">The expression to replace with.</param>
        /// <returns>The expression visited.</returns>
        public static Expression ReplaceAll(Expression expression, Expression[] searchFor, Expression[] replaceWith)
        {
            for (int i = 0, n = searchFor.Length; i < n; i++)
            {
                expression = Replace(expression, searchFor[i], replaceWith[i]);
            }
            return expression;
        }

        /// <summary>
        /// Replace expression.
        /// </summary>
        /// <param name="exp">The current expression to match and replace.</param>
        /// <returns>The replaced expression.</returns>
        protected override Expression Visit(Expression exp)
        {
            if (exp == this.searchFor)
            {
                return this.replaceWith;
            }
            return base.Visit(exp);
        }
    }

    /// <summary>
    /// Rewrites an expression tree so that locally isolatable 
    /// sub-expressions are evaluated and converted into 
    /// ConstantExpression nodes.
    /// </summary>
    internal static class PartialEvaluator
    {
        /// <summary>
        /// Performs evaluation and replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <param name="fnCanBeEvaluated">A function that decides whether a given expression node can be part of the local function.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression Eval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
        {
            return SubtreeEvaluator.Eval(Nominator.Nominate(fnCanBeEvaluated, expression), expression);
        }

        /// <summary>
        /// Performs evaluation and replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression Eval(Expression expression)
        {
            return Eval(expression, PartialEvaluator.CanBeEvaluatedLocally);
        }

        /// <summary>
        /// Can the expression be evaluated locally, that is, is the expression
        /// node type not a parameter type. Function contains no parameters, it
        /// can be called out right.
        /// </summary>
        /// <param name="expression">The current expression.</param>
        /// <returns>Can the expression be evaluated locally.</returns>
        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter;
        }

        /// <summary>
        /// Evaluates and replaces sub-trees when first candidate is reached (top-down)
        /// evaluates each expression in the function found.
        /// </summary>
        internal class SubtreeEvaluator : ExpressionVisitor
        {
            HashSet<Expression> candidates;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="candidates"> The collection of sub- expressions.</param>
            private SubtreeEvaluator(HashSet<Expression> candidates)
            {
                this.candidates = candidates;
            }

            /// <summary>
            /// Evaluate all the candiate expression functions in the collection, and
            /// turn each expression into a constant node. Evaluate each function one at
            /// a time until all sub-expression functions have been evaluated an turned
            /// into evaluated constants.
            /// </summary>
            /// <param name="candidates">The collection of all function candidates to evaluate.</param>
            /// <param name="exp">The expression to evaluate.</param>
            /// <returns>The constant expression node.</returns>
            internal static Expression Eval(HashSet<Expression> candidates, Expression exp)
            {
                return new SubtreeEvaluator(candidates).Visit(exp);
            }

            /// <summary>
            /// Visit the current expression.
            /// </summary>
            /// <param name="exp">The current expression.</param>
            /// <returns>The new expression node.</returns>
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
            /// Evaluate the current expression.
            /// </summary>
            /// <param name="e">The expression to evaluate.</param>
            /// <returns>The complied lambda expression containg the function.</returns>
            private Expression Evaluate(Expression e)
            {
                if (e.NodeType == ExpressionType.Constant)
                {
                    return e;
                }
                Type type = e.Type;
                if (type.IsValueType)
                {
                    e = Expression.Convert(e, typeof(object));
                }
                Expression<Func<object>> lambda = Expression.Lambda<Func<object>>(e);
                Func<object> fn = lambda.Compile();
                return Expression.Constant(fn(), type);
            }
        }

        /// <summary>
        /// Performs bottom-up analysis to determine which nodes can possibly
        /// be part of an evaluated sub-tree. Internal collection class.
        /// </summary>
        internal class Nominator : ExpressionVisitor
        {
            Func<Expression, bool> fnCanBeEvaluated;
            HashSet<Expression> candidates;
            bool cannotBeEvaluated;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="fnCanBeEvaluated">The delegate containing the expression.</param>
            private Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                this.candidates = new HashSet<Expression>();
                this.fnCanBeEvaluated = fnCanBeEvaluated;
            }

            /// <summary>
            /// Adds the current function to the collection.
            /// </summary>
            /// <param name="fnCanBeEvaluated">The delegate containing the expression.</param>
            /// <param name="expression">The current expression.</param>
            /// <returns>The expression values.</returns>
            internal static HashSet<Expression> Nominate(Func<Expression, bool> fnCanBeEvaluated, Expression expression)
            {
                Nominator nominator = new Nominator(fnCanBeEvaluated);
                nominator.Visit(expression);
                return nominator.candidates;
            }

            /// <summary>
            /// Visit the expression to test the expression.
            /// </summary>
            /// <param name="expression">The original expression.</param>
            /// <returns>The expression that can be evaluated.</returns>
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
        }
    }
}
