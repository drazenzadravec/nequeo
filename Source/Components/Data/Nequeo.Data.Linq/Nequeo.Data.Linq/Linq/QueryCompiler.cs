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

using Nequeo.Data.Linq;
using Nequeo.Data.Linq.Common;
using Nequeo.Data.Linq.Language;
using Nequeo.Data.Linq.Provider;
using Nequeo.Data.Linq.Common.Expressions;
using Nequeo.Data.Linq.Common.Translation;

namespace Nequeo.Data.Linq
{
    /// <summary>
    /// Creates a reusable, parameterized representation of a query that caches the execution plan
    /// </summary>
    public static class QueryCompiler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static D Compile<D>(Expression<D> query)
        {
            return (D)(object)Compile((LambdaExpression)query);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Func<TResult> Compile<TResult>(Expression<Func<TResult>> query)
        {
            return new CompiledQuery(query).Invoke<TResult>;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Func<T1, TResult> Compile<T1, TResult>(Expression<Func<T1, TResult>> query)
        {
            return new CompiledQuery(query).Invoke<T1, TResult>;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Func<T1, T2, TResult> Compile<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> query)
        {
            return new CompiledQuery(query).Invoke<T1, T2, TResult>;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Func<T1, T2, T3, TResult> Compile<T1, T2, T3, TResult>(Expression<Func<T1, T2, T3, TResult>> query)
        {
            return new CompiledQuery(query).Invoke<T1, T2, T3, TResult>;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Func<T1, T2, T3, T4, TResult> Compile<T1, T2, T3, T4, TResult>(Expression<Func<T1, T2, T3, T4, TResult>> query)
        {
            return new CompiledQuery(query).Invoke<T1, T2, T3, T4, TResult>;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Func<IEnumerable<T>> Compile<T>(this IQueryable<T> source)
        {
            return Compile<IEnumerable<T>>(
                Expression.Lambda<Func<IEnumerable<T>>>(((IQueryable)source).Expression)
                );
        }

        /// <summary>
        /// 
        /// </summary>
        public class CompiledQuery
        {
            LambdaExpression query;
            Delegate fnQuery;

            internal CompiledQuery(LambdaExpression query)
            {
                this.query = query;
            }

            internal void Compile(params object[] args)
            {
                if (this.fnQuery == null)
                {
                    // first identify the query provider being used
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
                    // ask the query provider to compile the query by 'executing' the lambda expression
                    IQueryProvider provider = ((IQueryable)root.Value).Provider;
                    Delegate result = (Delegate)provider.Execute(this.query);
                    System.Threading.Interlocked.CompareExchange(ref this.fnQuery, result, null);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="args"></param>
            /// <returns></returns>
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

            internal TResult Invoke<TResult>()
            {
                this.Compile(null);
                return ((Func<TResult>)this.fnQuery)();
            }

            internal TResult Invoke<T1, TResult>(T1 arg)
            {
                this.Compile(arg);
                return ((Func<T1, TResult>)this.fnQuery)(arg);
            }

            internal TResult Invoke<T1, T2, TResult>(T1 arg1, T2 arg2)
            {
                this.Compile(arg1, arg2);
                return ((Func<T1, T2, TResult>)this.fnQuery)(arg1, arg2);
            }

            internal TResult Invoke<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3)
            {
                this.Compile(arg1, arg2, arg3);
                return ((Func<T1, T2, T3, TResult>)this.fnQuery)(arg1, arg2, arg3);
            }

            internal TResult Invoke<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            {
                this.Compile(arg1, arg2, arg3, arg4);
                return ((Func<T1, T2, T3, T4, TResult>)this.fnQuery)(arg1, arg2, arg3, arg4);
            }
        }
    }

    /// <summary>
    /// Creates a reusable, parameterized representation of a query that caches the execution plan,
    /// this class replaces all parameters with their coresponding values.
    /// </summary>
    public static class QueryCompilerReplaceParam
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Delegate Compile(LambdaExpression query)
        {
            CompiledQueryReplaceParam cq = new CompiledQueryReplaceParam(query);
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
        /// 
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static D Compile<D>(Expression<D> query)
        {
            return (D)(object)Compile((LambdaExpression)query);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Func<TResult> Compile<TResult>(Expression<Func<TResult>> query)
        {
            return new CompiledQueryReplaceParam(query).Invoke<TResult>;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Func<T1, TResult> Compile<T1, TResult>(Expression<Func<T1, TResult>> query)
        {
            return new CompiledQueryReplaceParam(query).Invoke<T1, TResult>;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Func<T1, T2, TResult> Compile<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> query)
        {
            return new CompiledQueryReplaceParam(query).Invoke<T1, T2, TResult>;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Func<T1, T2, T3, TResult> Compile<T1, T2, T3, TResult>(Expression<Func<T1, T2, T3, TResult>> query)
        {
            return new CompiledQueryReplaceParam(query).Invoke<T1, T2, T3, TResult>;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Func<T1, T2, T3, T4, TResult> Compile<T1, T2, T3, T4, TResult>(Expression<Func<T1, T2, T3, T4, TResult>> query)
        {
            return new CompiledQueryReplaceParam(query).Invoke<T1, T2, T3, T4, TResult>;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Func<IEnumerable<T>> Compile<T>(this IQueryable<T> source)
        {
            return Compile<IEnumerable<T>>(
                Expression.Lambda<Func<IEnumerable<T>>>(((IQueryable)source).Expression)
                );
        }

        /// <summary>
        /// 
        /// </summary>
        public class CompiledQueryReplaceParam
        {
            LambdaExpression query;
            Delegate fnQuery;

            internal CompiledQueryReplaceParam(LambdaExpression query)
            {
                this.query = query;
            }

            internal void Compile(params object[] args)
            {
                if (this.fnQuery == null)
                {
                    // first identify the query provider being used
                    Expression replaced = null;
                    Expression body = this.query.Body;
                    ConstantExpression root = RootQueryableFinder.Find(body) as ConstantExpression;
                    if (root == null && args != null && args.Length > 0)
                    {
                        replaced = ExpressionReplacer.ReplaceAll(
                            this.query,
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
                    // ask the query provider to compile the query by 'executing' the lambda expression
                    IQueryProvider provider = ((IQueryable)root.Value).Provider;
                    Delegate result = null;

                    if (replaced != null)
                        result = (Delegate)provider.Execute(replaced);
                    else
                        result = (Delegate)provider.Execute(this.query);

                    System.Threading.Interlocked.CompareExchange(ref this.fnQuery, result, null);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="args"></param>
            /// <returns></returns>
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

            internal TResult Invoke<TResult>()
            {
                this.Compile(null);
                return ((Func<TResult>)this.fnQuery)();
            }

            internal TResult Invoke<T1, TResult>(T1 arg)
            {
                this.Compile(arg);
                return ((Func<T1, TResult>)this.fnQuery)(arg);
            }

            internal TResult Invoke<T1, T2, TResult>(T1 arg1, T2 arg2)
            {
                this.Compile(arg1, arg2);
                return ((Func<T1, T2, TResult>)this.fnQuery)(arg1, arg2);
            }

            internal TResult Invoke<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3)
            {
                this.Compile(arg1, arg2, arg3);
                return ((Func<T1, T2, T3, TResult>)this.fnQuery)(arg1, arg2, arg3);
            }

            internal TResult Invoke<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            {
                this.Compile(arg1, arg2, arg3, arg4);
                return ((Func<T1, T2, T3, T4, TResult>)this.fnQuery)(arg1, arg2, arg3, arg4);
            }
        }
    }
}