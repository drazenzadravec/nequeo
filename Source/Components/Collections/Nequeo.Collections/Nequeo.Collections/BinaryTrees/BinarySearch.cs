/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
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
using System.Threading.Tasks;

namespace Nequeo.Collections.BinaryTrees
{
    /// <summary>
    /// Binary search implementation.
    /// </summary>
    /// <typeparam name="T">The type to search for.</typeparam>
    public class BinarySearch<T> where T : IComparable<T>
    {
        /// <summary>
        /// Find the index of the value in the sorted collection.
        /// </summary>
        /// <param name="items">The sorted collection.</param>
        /// <param name="value">The value to find the index for.</param>
        /// <returns>The index of the value in the collection; else -1 if not found.</returns>
        public int FindIndex(T[] items, T value)
        {
            return FindIndex(items, value);
        }

        /// <summary>
        /// Find the index of the value in the sorted collection.
        /// </summary>
        /// <param name="items">The sorted collection.</param>
        /// <param name="value">The value to find the index for.</param>
        /// <returns>The index of the value in the collection; else -1 if not found.</returns>
        public int FindIndex(IEnumerable<T> items, T value)
        {
            return FindIndex(items, value);
        }

        /// <summary>
        /// Find the index of the value in the sorted collection.
        /// </summary>
        /// <param name="items">The sorted collection.</param>
        /// <param name="value">The value to find the index for.</param>
        /// <returns>The index of the value in the collection; else -1 if not found.</returns>
        public int FindIndex(Nequeo.Collections.Collection<T> items, T value)
        {
            return FindIndex(items, value);
        }

        /// <summary>
        /// Find the index of the value in the sorted collection.
        /// </summary>
        /// <param name="items">The sorted collection.</param>
        /// <param name="value">The value to find the index for.</param>
        /// <returns>The index of the value in the collection; else -1 if not found.</returns>
        public int FindIndex(IList<T> items, T value)
        {
            int centre = 0;
            int itemCount = 0;
            return FindIndexEx(items, value, out centre, out itemCount);
        }

        /// <summary>
        /// Find the index of the value in the sorted collection.
        /// </summary>
        /// <param name="items">The sorted collection.</param>
        /// <param name="value">The value to find the index for.</param>
        /// <param name="lowerNearest">The nearest lower item to the value.</param>
        /// <param name="upperNearest">The nearest upper item to the value.</param>
        /// <returns>The index of the value in the collection; else -1 if not found.</returns>
        public int FindIndex(T[] items, T value, out T lowerNearest, out T upperNearest)
        {
            return FindIndex(items, value, out lowerNearest, out upperNearest);
        }

        /// <summary>
        /// Find the index of the value in the sorted collection.
        /// </summary>
        /// <param name="items">The sorted collection.</param>
        /// <param name="value">The value to find the index for.</param>
        /// <param name="lowerNearest">The nearest lower item to the value.</param>
        /// <param name="upperNearest">The nearest upper item to the value.</param>
        /// <returns>The index of the value in the collection; else -1 if not found.</returns>
        public int FindIndex(IEnumerable<T> items, T value, out T lowerNearest, out T upperNearest)
        {
            return FindIndex(items, value, out lowerNearest, out upperNearest);
        }

        /// <summary>
        /// Find the index of the value in the sorted collection.
        /// </summary>
        /// <param name="items">The sorted collection.</param>
        /// <param name="value">The value to find the index for.</param>
        /// <param name="lowerNearest">The nearest lower item to the value.</param>
        /// <param name="upperNearest">The nearest upper item to the value.</param>
        /// <returns>The index of the value in the collection; else -1 if not found.</returns>
        public int FindIndex(Nequeo.Collections.Collection<T> items, T value, out T lowerNearest, out T upperNearest)
        {
            return FindIndex(items, value, out lowerNearest, out upperNearest);
        }

        /// <summary>
        /// Find the index of the value in the sorted collection.
        /// </summary>
        /// <param name="items">The sorted collection.</param>
        /// <param name="value">The value to find the index for.</param>
        /// <param name="lowerNearest">The nearest lower item to the value.</param>
        /// <param name="upperNearest">The nearest upper item to the value.</param>
        /// <returns>The index of the value in the collection; else -1 if not found.</returns>
        public int FindIndex(IList<T> items, T value, out T lowerNearest, out T upperNearest)
        {
            int centre = 0;
            int itemCount = 0;
            int index = FindIndexEx(items, value, out centre, out itemCount);

            // Set the nearest.
            lowerNearest = (centre > 0 && centre < itemCount - 1 ? (index > 0 && index < itemCount - 1 ? items[index - 1] : items[centre - 1]) : items[0]);
            upperNearest = (centre > 0 && centre < itemCount - 1 ? (index > 0 && index < itemCount - 1 ? items[index + 1] : items[centre + 1]) : items[itemCount - 1]);

            // Return the item index.
            return index;
        }

        /// <summary>
        /// Find the index of the value in the sorted collection.
        /// </summary>
        /// <param name="items">The sorted collection.</param>
        /// <param name="value">The value to find the index for.</param>
        /// <param name="centreIndex">The the last centre value.</param>
        /// <param name="itemCountValue">The number of items.</param>
        /// <returns>The index of the value in the collection; else -1 if not found.</returns>
        private int FindIndexEx(IList<T> items, T value, out int centreIndex, out int itemCountValue)
        {
            int index = -1;
            bool found = false;
            int itemCount = items.Count;
            T currentItem = default(T);

            // Go to the centre of the list.
            int centre = itemCount / 2;

            // Can not find indicators.
            int top = 0;
            int botom = 0;
            int barrier = centre;
            int lastCentre = 0;

            // While not found.
            while (!found)
            {
                // If within length.
                if (centre >= 0 && centre < itemCount)
                {
                    // Get curent item.
                    currentItem = items[centre];

                    // If item is less than value.
                    if (currentItem.CompareTo(value) < 0)
                    {
                        // Last centre.
                        lastCentre = centre;

                        // Move to the top half centre.
                        centre = centre + ((itemCount - barrier) / 2);
                        barrier = centre;

                        // This is an indicator that the value can not be found.
                        if (top == centre)
                            break;

                        // Keep going.
                        top = centre;
                    }
                    else if (currentItem.CompareTo(value) > 0)
                    {
                        // Move to the bottom half centre.
                        centre = centre - ((barrier - lastCentre) / 2);

                        // Last centre.
                        lastCentre = centre;

                        // This is an indicator that the value can not be found.
                        if (botom == centre)
                            break;

                        // Keep going.
                        botom = centre;
                    }
                    else if (currentItem.CompareTo(value) == 0)
                    {
                        // Item are equal, found
                        found = true;
                        index = centre;
                    }
                    else
                    {
                        // Cannot be found
                        index = -1;
                        break;
                    }
                }
                else
                    break;
            }

            // Set result values.
            centreIndex = centre;
            itemCountValue = itemCount;

            // Return the item index.
            return index;
        }
    }
}
