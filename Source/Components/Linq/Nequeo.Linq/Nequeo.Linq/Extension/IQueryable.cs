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
using System.Linq;
using System.Text;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Nequeo.Linq.Extension
{
    /// <summary>
    /// Class that extends the System.Ling.IQueryable type.
    /// </summary>
    public static class IQueryableExtensions
    {
        #region Public Methods
        /// <summary>
        /// Converts the System.Ling.IQueryable type
        /// to a collection of objects.
        /// </summary>
        /// <typeparam name="TSource">The source type within the collection.</typeparam>
        /// <param name="source">The current IQueryable type.</param>
        /// <returns>The array of objects for the source type.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static Object[] ToObjectArray<TSource>(this IQueryable<TSource> source)
        {
            // If the source object is null.
            if (source == null)
                throw new System.ArgumentNullException();

            int i = 0;

            // Create a new object collection.
            Object[] objectArray = new Object[source.Count()];

            // Get the current source enumerator.
            IEnumerator<TSource> data = source.GetEnumerator();

            // Fore each type in the collection
            // assign the object array with the type.
            while (data.MoveNext())
                objectArray[i++] = (Object)data.Current;

            // Return the object collection.
            return objectArray;
        }

        /// <summary>
        /// Converts the System.Ling.IQueryable type
        /// to a collection of objects.
        /// </summary>
        /// <typeparam name="TSource">The source type within the collection.</typeparam>
        /// <param name="source">The current IQueryable type.</param>
        /// <returns>The array of objects for the source type.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static TSource[] ToSourceArray<TSource>(this IQueryable<TSource> source)
        {
            // If the source object is null.
            if (source == null)
                throw new System.ArgumentNullException();

            int i = 0;

            // Create a new object collection.
            TSource[] objectArray = new TSource[source.Count()];

            // Get the current source enumerator.
            IEnumerator<TSource> data = source.GetEnumerator();

            // Fore each type in the collection
            // assign the object array with the type.
            while (data.MoveNext())
                objectArray[i++] = data.Current;

            // Return the object collection.
            return objectArray;
        }

        /// <summary>
        /// Gets all the items by dynamically creates a predicate query on the specified criteria.
        /// </summary>
        /// <typeparam name="T">The source type within the collection.</typeparam>
        /// <param name="source">The current IQueryable type.</param>
        /// <param name="predicate">The predicate query to use for the call.</param>
        /// <param name="values">The array of query values to apply to the predicate query.</param>
        /// <returns>A collection of queryable items.</returns>
        public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, params object[] values)
        {
            return (IQueryable<T>)Where((IQueryable)source, predicate, values);
        }

        /// <summary>
        /// Gets all the items by dynamically creates a predicate query on the specified criteria.
        /// </summary>
        /// <param name="source">The current IQueryable type.</param>
        /// <param name="predicate">The predicate query to use for the call.</param>
        /// <param name="values">The array of query values to apply to the predicate query.</param>
        /// <returns>A collection of queryable items.</returns>
        public static IQueryable Where(this IQueryable source, string predicate, params object[] values)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");
            LambdaExpression lambda = Nequeo.Linq.DynamicExpression.ParseLambda(source.ElementType, typeof(bool), predicate, values);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Where",
                    new Type[] { source.ElementType },
                    source.Expression, Expression.Quote(lambda)));
        }

        /// <summary>
        /// Gets the first item by dynamically creates a predicate query on the specified criteria.
        /// </summary>
        /// <typeparam name="T">The source type within the collection.</typeparam>
        /// <param name="source">The current IQueryable type.</param>
        /// <param name="predicate">The predicate query to use for the call.</param>
        /// <param name="values">The array of query values to apply to the predicate query.</param>
        /// <returns>One item of the specified type.</returns>
        public static T First<T>(this IQueryable<T> source, string predicate, params object[] values)
        {
            // Get the first item found.
            IQueryable query = First((IQueryable)source, predicate, values);
            System.Collections.IEnumerator dataEnum = query.GetEnumerator();

            // Assign the current type.
            T sourceType = default(T);

            // For each item found.
            while (dataEnum.MoveNext())
            {
                // Get the first item
                // and exit the loop.
                sourceType = (T)dataEnum.Current;
                break;
            }

            // Return the first item found.
            return sourceType;
        }

        /// <summary>
        /// Gets the first item by dynamically creates a predicate query on the specified criteria.
        /// </summary>
        /// <param name="source">The current IQueryable type.</param>
        /// <param name="predicate">The predicate query to use for the call.</param>
        /// <param name="values">The array of query values to apply to the predicate query.</param>
        /// <returns>One item of the specified type.</returns>
        private static IQueryable First(this IQueryable source, string predicate, params object[] values)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");
            LambdaExpression lambda = Nequeo.Linq.DynamicExpression.ParseLambda(source.ElementType, typeof(bool), predicate, values);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Where",
                    new Type[] { source.ElementType },
                    source.Expression, Expression.Quote(lambda)));
        }

        /// <summary>
        /// Gets all items by dynamically creates a selector query on the specified criteria.
        /// </summary>
        /// <param name="source">The current IQueryable type.</param>
        /// <param name="selector">The selector query to use for the call.</param>
        /// <param name="values">The array of query values to apply to the selector query.</param>
        /// <returns>The IQueryable query of items.</returns>
        public static IQueryable Select(this IQueryable source, string selector, params object[] values)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            LambdaExpression lambda = Nequeo.Linq.DynamicExpression.ParseLambda(source.ElementType, null, selector, values);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Select",
                    new Type[] { source.ElementType, lambda.Body.Type },
                    source.Expression, Expression.Quote(lambda)));
        }

        /// <summary>
        /// Gets the ordering expression by dynamically creates a selector query on the specified criteria.
        /// </summary>
        /// <typeparam name="T">The source type within the collection.</typeparam>
        /// <param name="source">The current IQueryable type.</param>
        /// <param name="ordering">The order by fields for the expression.</param>
        /// <param name="values">The array of query values to apply to the ordering query.</param>
        /// <returns>The query expression from the dynamic creation.</returns>
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, params object[] values)
        {
            return (IQueryable<T>)OrderBy((IQueryable)source, ordering, values);
        }

        /// <summary>
        /// Gets the ordering expression by dynamically creates a selector query on the specified criteria.
        /// </summary>
        /// <param name="source">The current IQueryable type.</param>
        /// <param name="ordering">The order by fields for the expression.</param>
        /// <param name="values">The array of query values to apply to the ordering query.</param>
        /// <returns>The query expression from the dynamic creation.</returns>
        public static IQueryable OrderBy(this IQueryable source, string ordering, params object[] values)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (ordering == null) throw new ArgumentNullException("ordering");
            ParameterExpression[] parameters = new ParameterExpression[] {
                Expression.Parameter(source.ElementType, "") };
            Nequeo.Linq.ExpressionParser parser = new ExpressionParser(parameters, ordering, values);
            IEnumerable<DynamicOrdering> orderings = parser.ParseOrdering();
            Expression queryExpr = source.Expression;
            string methodAsc = "OrderBy";
            string methodDesc = "OrderByDescending";
            foreach (DynamicOrdering o in orderings)
            {
                queryExpr = Expression.Call(
                    typeof(Queryable), o.Ascending ? methodAsc : methodDesc,
                    new Type[] { source.ElementType, o.Selector.Type },
                    queryExpr, Expression.Quote(Expression.Lambda(o.Selector, parameters)));
                methodAsc = "ThenBy";
                methodDesc = "ThenByDescending";
            }
            return source.Provider.CreateQuery(queryExpr);
        }

        /// <summary>
        /// Gets the count expression by dynamically creates a count query on the specified criteria.
        /// </summary>
        /// <param name="source">The current IQueryable type.</param>
        /// <param name="count">The count fields for the expression.</param>
        /// <returns>The query expression from the dynamic creation.</returns>
        public static IQueryable Take(this IQueryable source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Take",
                    new Type[] { source.ElementType },
                    source.Expression, Expression.Constant(count)));
        }

        /// <summary>
        /// Gets the count expression by dynamically creates a count query on the specified criteria.
        /// </summary>
        /// <param name="source">The current IQueryable type.</param>
        /// <param name="count">The count fields for the expression.</param>
        /// <returns>The query expression from the dynamic creation.</returns>
        public static IQueryable Skip(this IQueryable source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Skip",
                    new Type[] { source.ElementType },
                    source.Expression, Expression.Constant(count)));
        }

        /// <summary>
        /// Gets the group by expression by dynamically creates a group by query on the specified criteria.
        /// </summary>
        /// <param name="source">The current IQueryable type.</param>
        /// <param name="keySelector">The key selector within the expression.</param>
        /// <param name="elementSelector">The element selector within the expression.</param>
        /// <param name="values">The array of query values to apply to the group by query</param>
        /// <returns>The query expression from the dynamic creation.</returns>
        public static IQueryable GroupBy(this IQueryable source, string keySelector, string elementSelector, params object[] values)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (elementSelector == null) throw new ArgumentNullException("elementSelector");
            LambdaExpression keyLambda = Nequeo.Linq.DynamicExpression.ParseLambda(source.ElementType, null, keySelector, values);
            LambdaExpression elementLambda = Nequeo.Linq.DynamicExpression.ParseLambda(source.ElementType, null, elementSelector, values);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "GroupBy",
                    new Type[] { source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type },
                    source.Expression, Expression.Quote(keyLambda), Expression.Quote(elementLambda)));
        }

        /// <summary>
        /// Gets the any expression by dynamically creates a any query on the specified criteria.
        /// </summary>
        /// <param name="source">The current IQueryable type.</param>
        /// <returns>The query expression from the dynamic creation.</returns>
        public static bool Any(this IQueryable source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return (bool)source.Provider.Execute(
                Expression.Call(
                    typeof(Queryable), "Any",
                    new Type[] { source.ElementType }, source.Expression));
        }

        /// <summary>
        /// Gets the count expression by dynamically creates a count query on the specified criteria.
        /// </summary>
        /// <param name="source">The current IQueryable type.</param>
        /// <returns>The query expression from the dynamic creation.</returns>
        public static int Count(this IQueryable source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return (int)source.Provider.Execute(
                Expression.Call(
                    typeof(Queryable), "Count",
                    new Type[] { source.ElementType }, source.Expression));
        }

        /// <summary>
        /// Compiles the current queryable type.
        /// </summary>
        /// <typeparam name="T">The source type within the collection.</typeparam>
        /// <param name="source">The current IQueryable type.</param>
        /// <returns>The enumrable function type.</returns>
        public static Nequeo.Threading.FunctionHandler<IEnumerable<T>> Compile<T>(this IQueryable<T> source)
        {
            return QueryCompiler.Compile<IEnumerable<T>>(
                Expression.Lambda<Nequeo.Threading.FunctionHandler<IEnumerable<T>>>(((IQueryable)source).Expression));
        }
        #endregion
    }
}
