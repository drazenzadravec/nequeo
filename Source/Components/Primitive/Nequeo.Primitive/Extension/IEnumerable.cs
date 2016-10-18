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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

using Nequeo.Invention;
using Nequeo.Reflection;
using Nequeo.Extension;

namespace Nequeo.Extension
{
    /// <summary>
    /// Class that extends the System.Collections.Generic.IEnumerable type.
    /// </summary>
    public static class IEnumerableExtensions
    {
        #region Public Methods
        /// <summary>
        /// Converts the System.Collections.Generic.IEnumerable type
        /// to a collection of strings.
        /// </summary>
        /// <typeparam name="TSource">The source type within the collection.</typeparam>
        /// <param name="source">The current IEnumerable type.</param>
        /// <returns>The array of strings for the source type.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static String[] ToStringArray<TSource>(this IEnumerable<TSource> source)
        {
            // If the source object is null.
            if (source == null)
                throw new System.ArgumentNullException();

            int i = 0;

            // Create a new object collection.
            String[] objectArray = new String[source.Count()];

            // Get the current source enumerator.
            IEnumerator<TSource> data = source.GetEnumerator();

            // For each type in the collection
            // assign the object array with the type.
            while (data.MoveNext())
                objectArray[i++] = data.Current.ToString();

            // Return the object collection.
            return objectArray;
        }

        /// <summary>
        /// Converts the System.Collections.Generic.IEnumerable type
        /// to a collection of objects.
        /// </summary>
        /// <typeparam name="TSource">The source type within the collection.</typeparam>
        /// <param name="source">The current IEnumerable type.</param>
        /// <returns>The array of objects for the source type.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static Object[] ToObjectArray<TSource>(this IEnumerable<TSource> source)
        {
            // If the source object is null.
            if (source == null)
                throw new System.ArgumentNullException();

            int i = 0;

            // Create a new object collection.
            Object[] objectArray = new Object[source.Count()];

            // Get the current source enumerator.
            IEnumerator<TSource> data = source.GetEnumerator();

            // For each type in the collection
            // assign the object array with the type.
            while (data.MoveNext())
                objectArray[i++] = (Object)data.Current;

            // Return the object collection.
            return objectArray;
        }

        /// <summary>
        /// Converts the object type
        /// to the specific type.
        /// </summary>
        /// <typeparam name="TSource">The source type within the collection.</typeparam>
        /// <param name="source">The source collection array.</param>
        /// <returns>The array of generic type objects for the source type.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static TSource[] ToTSourceArray<TSource>(this IEnumerable<TSource> source)
        {
            // If the source object is null.
            if (source == null)
                throw new System.ArgumentNullException();

            int i = 0;

            // Create a new object collection.
            TSource[] objectArray = new TSource[source.Count()];

            // Get the current source enumerator.
            IEnumerator<TSource> data = source.GetEnumerator();

            // For each type in the collection
            // assign the object array with the type.
            while (data.MoveNext())
                objectArray[i++] = (TSource)data.Current;

            // Return the object collection.
            return objectArray;
        }

        /// <summary>
        /// Converts the object type
        /// to the specific type.
        /// </summary>
        /// <typeparam name="TResult">The result type within the collection.</typeparam>
        /// <param name="source">The source collection array.</param>
        /// <returns>The array of generic type objects for the source type.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static TResult[] ToTSourceArray<TResult>(this IEnumerable<Object> source)
        {
            // If the source object is null.
            if (source == null)
                throw new System.ArgumentNullException();

            int i = 0;

            // Create a new object collection.
            TResult[] objectArray = new TResult[source.Count()];

            // Get the current source enumerator.
            IEnumerator<Object> data = source.GetEnumerator();

            // For each type in the collection
            // assign the object array with the type.
            while (data.MoveNext())
                objectArray[i++] = (TResult)data.Current;

            // Return the object collection.
            return objectArray;
        }

        /// <summary>
        /// Converts the System.Collections.Generic.IEnumerable type
        /// to a list object with generic array.
        /// </summary>
        /// <typeparam name="TResult">The source type within the collection.</typeparam>
        /// <param name="source">The source collection array.</param>
        /// <returns>The array of generic type in the list object for the source type.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static List<TResult> ToListTSourceArray<TResult>(this IEnumerable<Object> source)
        {
            // If the source object is null.
            if (source == null)
                throw new System.ArgumentNullException();

            // Create a new object collection.
            List<TResult> objectArray = new List<TResult>();

            // Get the current source enumerator.
            IEnumerator<Object> data = source.GetEnumerator();

            // For each type in the collection
            // assign the object array with the type.
            while (data.MoveNext())
                objectArray.Add((TResult)data.Current);

            // Return the object collection.
            return objectArray;
        }

        /// <summary>
        /// Converts the System.Collections.Generic.IEnumerable type
        /// to a list object with generic array.
        /// </summary>
        /// <typeparam name="TSource">The source type within the collection.</typeparam>
        /// <param name="source">The source collection array.</param>
        /// <returns>The array of generic type in the list object for the source type.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static List<TSource> ToListTSourceArray<TSource>(this IEnumerable<TSource> source)
        {
            // If the source object is null.
            if (source == null)
                throw new System.ArgumentNullException();

            // Create a new object collection.
            List<TSource> objectArray = new List<TSource>(source);

            // Return the object collection.
            return objectArray;
        }

        /// <summary>
        /// Finds the intersection of two arrays
        /// </summary>
        /// <typeparam name="TSource">The source type within the collection.</typeparam>
        /// <param name="source">The source collection array.</param>
        /// <param name="arrayB">The array to compare with.</param>
        /// <param name="isEachArraySorted">Are each of the arrays sorted.</param>
        /// <returns>The intersection collection of the arrays.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static TSource[] Intersection<TSource>(
            this IEnumerable<TSource> source, TSource[] arrayB, bool isEachArraySorted)
        {
            // If the source object is null.
            if (source == null)
                throw new System.ArgumentNullException();

            int i = 0;

            // Create a new object collection.
            TSource[] objectArray = new TSource[source.Count()];

            // Get the current source enumerator.
            IEnumerator<TSource> data = source.GetEnumerator();

            // For each type in the collection
            // assign the object array with the type.
            while (data.MoveNext())
                objectArray[i++] = (TSource)data.Current;

            // Get the intersection of the arrays.
            return ArrayComparer.Intersection<TSource>(objectArray, arrayB, isEachArraySorted);
        }

        /// <summary>
        /// Does an intersection of two arrays exist.
        /// </summary>
        /// <typeparam name="TSource">The source type within the collection.</typeparam>
        /// <param name="source">The source collection array.</param>
        /// <param name="arrayB">The array to compare with.</param>
        /// <param name="isEachArraySorted">Are each of the arrays sorted.</param>
        /// <returns>True if an intersection exists; else false.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static bool IntersectionExists<TSource>(
            this IEnumerable<TSource> source, TSource[] arrayB, bool isEachArraySorted)
        {
            // If the source object is null.
            if (source == null)
                throw new System.ArgumentNullException();

            int i = 0;

            // Create a new object collection.
            TSource[] objectArray = new TSource[source.Count()];

            // Get the current source enumerator.
            IEnumerator<TSource> data = source.GetEnumerator();

            // For each type in the collection
            // assign the object array with the type.
            while (data.MoveNext())
                objectArray[i++] = (TSource)data.Current;

            // Get the intersection of the arrays.
            TSource[] result = ArrayComparer.Intersection<TSource>(objectArray, arrayB, isEachArraySorted);

            if (result != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Sorts an array of elements, descending or ascending.
        /// </summary>
        /// <typeparam name="TSource">The source type within the collection.</typeparam>
        /// <param name="source">The source collection array.</param>
        /// <param name="descending">Sort in decending order else ascending.</param>
        /// <returns>The sorted array of elements.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static TSource[] Sort<TSource>(
            this IEnumerable<TSource> source, bool descending)
        {
            // If the source object is null.
            if (source == null)
                throw new System.ArgumentNullException();

            int i = 0;

            // Create a new object collection.
            TSource[] objectArray = new TSource[source.Count()];

            // Get the current source enumerator.
            IEnumerator<TSource> data = source.GetEnumerator();

            // For each type in the collection
            // assign the object array with the type.
            while (data.MoveNext())
                objectArray[i++] = (TSource)data.Current;

            // Get the sorted array.
            return ArrayComparer.Sort<TSource>(objectArray, descending);
        }

        /// <summary>
        /// Finds the index of the first item matching an expression in an enumerable.
        /// </summary> 
        /// <typeparam name="TSource">The source type within the collection.</typeparam>
        /// <param name="source">The enumerable to search.</param> 
        /// <param name="predicate">The expression to test the items against.</param> 
        /// <returns>The index of the first matching item, or -1 if no items match.</returns> 
        public static int FindIndex<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            int retVal = 0;
            foreach (var item in source)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }
            return -1;
        }

        /// <summary>
        /// Finds the indexes of the item matching an expression in an enumerable.
        /// </summary> 
        /// <typeparam name="TSource">The source type within the collection.</typeparam>
        /// <param name="source">The enumerable to search.</param> 
        /// <param name="predicate">The expression to test the items against.</param> 
        /// <returns>The indexes of the matching items, or null if no items match.</returns> 
        public static int[] FindIndexes<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            int retVal = 0;
            List<int> retVals = new List<int>();
            foreach (var item in source)
            {
                if (predicate(item))
                    retVals.Add(retVal);

                retVal++;
            }
            return (retVals.Count > 0 ? retVals.ToArray() : null);
        }

        /// <summary>
        /// Remove an item from the array.
        /// </summary>
        /// <typeparam name="TSource">The source type within the collection.</typeparam>
        /// <param name="source">The enumerable to search.</param>
        /// <param name="index">The index of the item in the array to remove.</param>
        /// <returns>The new array of items.</returns>
        public static TSource[] Remove<TSource>(this IEnumerable<TSource> source, int index)
        {
            if (source == null) throw new ArgumentNullException("source");

            int count = 0;
            List<TSource> retVals = new List<TSource>();
            foreach (var item in source)
            {
                // If the count is not the index then add.
                if (count != index)
                {
                    retVals.Add(item);
                }
                count++;
            }
            return retVals.ToArray();
        }

        /// <summary>
        /// Remove item(s) from the array.
        /// </summary>
        /// <typeparam name="TSource">The source type within the collection.</typeparam>
        /// <param name="source">The enumerable to search.</param>
        /// <param name="indexes">The indexes of the item in the array to remove.</param>
        /// <returns>The new array of items.</returns>
        public static TSource[] Remove<TSource>(this IEnumerable<TSource> source, int[] indexes)
        {
            if (source == null) throw new ArgumentNullException("source");

            int count = 0;
            List<TSource> retVals = new List<TSource>();
            foreach (var item in source)
            {
                // If the count is not the index then add.
                if (!indexes.Contains(count))
                {
                    retVals.Add(item);
                }
                count++;
            }
            return retVals.ToArray();
        }

        /// <summary>
        /// Remove item(s) from the array.
        /// </summary>
        /// <typeparam name="TSource">The source type within the collection.</typeparam>
        /// <param name="source">The enumerable to search.</param>
        /// <param name="predicate">The expression to test the items against.</param> 
        /// <returns>The new array of items.</returns>
        public static TSource[] Remove<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            List<TSource> retVals = new List<TSource>();
            foreach (var item in source)
            {
                bool removeItem = false;

                // Find the index to remove.
                if (predicate(item))
                    removeItem = true;

                // If not removing then add.
                if (!removeItem)
                {
                    retVals.Add(item);
                }
            }
            return retVals.ToArray();
        }

        /// <summary>
        /// Keep item(s) in the array.
        /// </summary>
        /// <typeparam name="TSource">The source type within the collection.</typeparam>
        /// <param name="source">The enumerable to search.</param>
        /// <param name="indexes">The indexes of the item in the array to keep.</param>
        /// <returns>The new array of items.</returns>
        public static TSource[] Keep<TSource>(this IEnumerable<TSource> source, int[] indexes)
        {
            if (source == null) throw new ArgumentNullException("source");

            int count = 0;
            List<TSource> retVals = new List<TSource>();
            foreach (var item in source)
            {
                // If the count is the index then add.
                if (indexes.Contains(count))
                {
                    retVals.Add(item);
                }
                count++;
            }
            return retVals.ToArray();
        }

        /// <summary>
        /// Keep item(s) in the array.
        /// </summary>
        /// <typeparam name="TSource">The source type within the collection.</typeparam>
        /// <param name="source">The enumerable to search.</param>
        /// <param name="predicate">The expression to test the items against.</param> 
        /// <returns>The new array of items.</returns>
        public static TSource[] Keep<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            List<TSource> retVals = new List<TSource>();
            foreach (var item in source)
            {
                bool keepItem = false;

                // Find the index to remove.
                if (predicate(item))
                    keepItem = true;

                // If not keeping then add.
                if (keepItem)
                {
                    retVals.Add(item);
                }
            }
            return retVals.ToArray();
        }

        /// <summary>
        /// Add the items to the sources if they do not exist in the source collection.
        /// </summary>
        /// <typeparam name="TSource">The source type within the collection.</typeparam>
        /// <typeparam name="TAdd">The add type within the collection.</typeparam>
        /// <param name="source">The enumerable to search.</param>
        /// <param name="add">The list to add to the sources if any item does not exist in the source.</param>
        /// <param name="predicate">The expression to test the items against.</param>
        /// <param name="addAction">The expression used to add the data.</param>
        /// <returns>The new array of items.</returns>
        public static TSource[] AddIfNotExists<TSource, TAdd>(this IEnumerable<TSource> source, TAdd[] add, Func<TSource, TAdd, bool> predicate, Func<TAdd, TSource> addAction)
        {
            if (add == null) throw new ArgumentNullException("add");
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            List<TAdd> foundItems = new List<TAdd>();
            object monitor = new object();
            int numberToFind = add.Length;
            bool[] found = new bool[numberToFind];

            // For each source item.
            foreach (var item in source)
            {
                int numberFound = 0;

                // For each add item.
                for (int i = 0; i < numberToFind; i++)
                {
                    // If predicate is true then do not add.
                    if (predicate(item, add[i]))
                        found[i] = true;

                    // If the current identifier
                    // has been found then stop the
                    // search for the current identifier.
                    if (found[i])
                    {
                        // Add to the collection.
                        foundItems.Add(add[i]);
                        break;
                    }
                }

                // Count the number of items found.
                Parallel.For(0, numberToFind, () => 0, (j, state, local) =>
                {
                    // If found then increment the count.
                    if (found[j])
                        return local = 1;
                    else
                        return local = 0;

                }, local =>
                {
                    // Add one to the count.
                    lock (monitor)
                        numberFound += local;
                });

                // If all the machine names have been found
                // then stop the search.
                if (numberFound >= numberToFind)
                    break;
            }

            // If not all the add itema are in the source.
            if (foundItems.Count != add.Length)
            {
                List<TSource> retVals = new List<TSource>(source);

                // For each add item
                foreach (TAdd item in add)
                {
                    bool foundIt = false;

                    // For each item found.
                    foreach (TAdd item1 in foundItems)
                    {
                        // If items are equal then found.
                        if(item.Equals(item1))
                        {
                            foundIt = true;
                            break;
                        }
                    }

                    // If not found then add to collection.
                    if(!foundIt)
                    {
                        // Add the new item with the action.
                        retVals.Add(addAction(item));
                    }
                }

                return retVals.ToArray();
            }
            else
                // If all items already exist then return origin source.
                return source.ToArray();
        }

        /// <summary>
        /// Add the items to the sources if they do not exist in the source collection.
        /// </summary>
        /// <typeparam name="TSource">The source type within the collection.</typeparam>
        /// <typeparam name="TAdd">The add type within the collection.</typeparam>
        /// <typeparam name="TAddSecond">The second add type to include.</typeparam>
        /// <param name="source">The enumerable to search.</param>
        /// <param name="add">The list to add to the sources if any item does not exist in the source.</param>
        /// <param name="addSecond">The second list to add.</param>
        /// <param name="predicate">The expression to test the items against.</param>
        /// <param name="addAction">The expression used to add the data.</param>
        /// <returns>The new array of items.</returns>
        public static TSource[] AddIfNotExists<TSource, TAdd, TAddSecond>(this IEnumerable<TSource> source,
            TAdd[] add, TAddSecond[] addSecond, Func<TSource, TAdd, bool> predicate, Func<TAdd, TAddSecond, TSource> addAction)
        {
            if (add == null) throw new ArgumentNullException("add");
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            List<TAdd> foundItems = new List<TAdd>();
            object monitor = new object();
            int numberToFind = add.Length;
            bool[] found = new bool[numberToFind];

            // For each source item.
            foreach (var item in source)
            {
                int numberFound = 0;

                // For each add item.
                for (int i = 0; i < numberToFind; i++)
                {
                    // If predicate is true then do not add.
                    if (predicate(item, add[i]))
                        found[i] = true;

                    // If the current identifier
                    // has been found then stop the
                    // search for the current identifier.
                    if (found[i])
                    {
                        // Add to the collection.
                        foundItems.Add(add[i]);
                        break;
                    }
                }

                // Count the number of items found.
                Parallel.For(0, numberToFind, () => 0, (j, state, local) =>
                {
                    // If found then increment the count.
                    if (found[j])
                        return local = 1;
                    else
                        return local = 0;

                }, local =>
                {
                    // Add one to the count.
                    lock (monitor)
                        numberFound += local;
                });

                // If all the machine names have been found
                // then stop the search.
                if (numberFound >= numberToFind)
                    break;
            }

            // If not all the add itema are in the source.
            if (foundItems.Count != add.Length)
            {
                List<TSource> retVals = new List<TSource>(source);

                // For each add item
                for(int i = 0; i < add.Length; i++)
                {
                    bool foundIt = false;

                    // For each item found.
                    foreach (TAdd item1 in foundItems)
                    {
                        // If items are equal then found.
                        if (add[i].Equals(item1))
                        {
                            foundIt = true;
                            break;
                        }
                    }

                    // If not found then add to collection.
                    if (!foundIt)
                    {
                        // Add the new item with the action.
                        retVals.Add(addAction(add[i], addSecond[i]));
                    }
                }

                return retVals.ToArray();
            }
            else
                // If all items already exist then return origin source.
                return source.ToArray();
        }

        /// <summary>
        /// Executes the provided delegate for each item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="action">The action to be applied.</param>
        public static void Each<T>(this IEnumerable<T> instance, Action<T, int> action)
        {
            int index = 0;
            foreach (T item in instance)
                action(item, index++);
        }

        /// <summary>
        /// Executes the provided delegate for each item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="action">The action to be applied.</param>
        public static void Each<T>(this IEnumerable<T> instance, Action<T> action)
        {
            foreach (T item in instance)
                action(item);
        }

        /// <summary>
        /// Convert to generic object enumerable instance.
        /// </summary>
        /// <param name="source">The current enumerable.</param>
        /// <returns>The generic enumerable instance.</returns>
        public static IEnumerable AsGenericEnumerable(this IEnumerable source)
        {
            Type elementType = typeof(Object);

            Type type = source.GetType().FindGenericType(typeof(IEnumerable<>));
            if (type != null)
            {
                return source;
            }

            IEnumerator enumerator = source.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator.Current != null)
                {
                    elementType = enumerator.Current.GetType();
                    try
                    {
                        enumerator.Reset();
                    }
                    catch
                    {
                    }
                    break;
                }
            }

            Type genericType = typeof(GenericEnumerable<>).MakeGenericType(elementType);
            object[] constructorParameters = new object[] { source };

            return (IEnumerable)Activator.CreateInstance(genericType, constructorParameters);
        }

        /// <summary>
        /// Get the index of the item.
        /// </summary>
        /// <param name="source">The current enumerable.</param>
        /// <param name="item">The item.</param>
        /// <returns>The index of the item.</returns>
        public static int IndexOf(this IEnumerable source, object item)
        {
            int index = 0;
            foreach (object i in source)
            {
                if (Equals(i, item))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        /// <summary>
        /// Get the element at index.
        /// </summary>
        /// <param name="source">The current enumerable.</param>
        /// <param name="index">The index.</param>
        /// <returns>The object at the index; else null.</returns>
        public static object ElementAt(this IEnumerable source, int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            var list = source as IList;
            if (list != null && list.Count > 0)
            {
                return list[index];
            }

            foreach (var item in source)
            {
                if (index == 0)
                {
                    return item;
                }

                index--;
            }

            return null;
        }

        /// <summary>
        /// Select recursively.
        /// </summary>
        /// <typeparam name="TSource">The enumerable type.</typeparam>
        /// <param name="source">The enum source.</param>
        /// <param name="recursiveSelector">The recursive selector.</param>
        /// <returns>The enumerable type result.</returns>
        public static IEnumerable<TSource> SelectRecursive<TSource>(this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TSource>> recursiveSelector)
        {
            Stack<IEnumerator<TSource>> stack = new Stack<IEnumerator<TSource>>();
            stack.Push(source.GetEnumerator());

            try
            {
                while (stack.Count > 0)
                {
                    if (stack.Peek().MoveNext())
                    {
                        TSource current = stack.Peek().Current;

                        yield return current;

                        IEnumerable<TSource> children = recursiveSelector(current);
                        if (children != null)
                        {
                            stack.Push(children.GetEnumerator());
                        }
                    }
                    else
                    {
                        stack.Pop().Dispose();
                    }
                }
            }
            finally
            {
                while (stack.Count > 0)
                {
                    stack.Pop().Dispose();
                }
            }
        }

        /// <summary>
        /// Zip iterator. 
        /// </summary>
        /// <typeparam name="TFirst">The first type.</typeparam>
        /// <typeparam name="TSecond">The second type.</typeparam>
        /// <typeparam name="TResult">The result ype.</typeparam>
        /// <param name="first">The first collection.</param>
        /// <param name="second">The second collection.</param>
        /// <param name="resultSelector">The result selector function.</param>
        /// <returns>The combined type collection.</returns>
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");

            return ZipIterator(first, second, resultSelector);
        }

        /// <summary>
        /// Zip iterator. 
        /// </summary>
        /// <typeparam name="TFirst">The first type.</typeparam>
        /// <typeparam name="TSecond">The second type.</typeparam>
        /// <typeparam name="TResult">The result ype.</typeparam>
        /// <param name="first">The first collection.</param>
        /// <param name="second">The second collection.</param>
        /// <param name="resultSelector">The result selector function.</param>
        /// <returns>The combined type collection.</returns>
        private static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TResult>(
            IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            using (IEnumerator<TFirst> e1 = first.GetEnumerator())
            using (IEnumerator<TSecond> e2 = second.GetEnumerator())
                while (e1.MoveNext() && e2.MoveNext())
                    yield return resultSelector(e1.Current, e2.Current);
        }

        /// <summary>
        /// To readOnly collection.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="sequence">The current sequence.</param>
        /// <returns>The readOnly collection.</returns>
        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> sequence)
        {
            if (sequence == null)
            {
                return DefaultReadOnlyCollection<T>.Empty;
            }
            ReadOnlyCollection<T> onlys = sequence as ReadOnlyCollection<T>;
            if (onlys != null)
            {
                return onlys;
            }
            return new ReadOnlyCollection<T>(sequence.ToArray());
        }

        /// <summary>
        /// Default ReadOnly Collection.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        private static class DefaultReadOnlyCollection<T>
        {
            private static ReadOnlyCollection<T> defaultCollection;

            /// <summary>
            /// Gets the Empty collection.
            /// </summary>
            internal static ReadOnlyCollection<T> Empty
            {
                get
                {
                    if (defaultCollection == null)
                    {
                        defaultCollection = new ReadOnlyCollection<T>(new T[0]);
                    }
                    return defaultCollection;
                }
            }
        }

        /// <summary>
        /// Generic enumerable.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        private class GenericEnumerable<T> : IEnumerable<T>
        {
            private readonly IEnumerable source;

            /// <summary>
            /// Initializes a new instance of the <see cref="GenericEnumerable{T}"/> class.
            /// </summary>
            /// <param name="source">The source.</param>
            public GenericEnumerable(IEnumerable source)
            {
                this.source = source;
            }

            /// <summary>
            /// Get the implicit enumerable.
            /// </summary>
            /// <returns>The implicit enumerable.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.source.GetEnumerator();
            }

            /// <summary>
            /// Get the implicit generic enumerable.
            /// </summary>
            /// <returns>The implicit generic enumerable.</returns>
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                foreach (T item in this.source)
                {
                    yield return item;
                }
            }
        }
        #endregion
    }
}
