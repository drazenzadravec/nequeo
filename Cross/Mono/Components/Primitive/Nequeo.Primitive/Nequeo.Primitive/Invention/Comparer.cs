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
using System.Threading;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Net.Mail;
using System.Net;
using System.Windows.Forms;

namespace Nequeo.Invention
{
    /// <summary>
    /// This class will implement the IComparer interface, The implementation
    /// will compare and sort a column for all ListViewItem dat in a ListView.
    /// </summary>
    public class ListViewItemColumn : IComparer<ListViewItem>
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="column">The column to compare.</param>
        /// <param name="descending">Sort into descending order.</param>
        /// <remarks>Sorting into descebding order default is true else
        /// sort into ascending or is false.</remarks>
        public ListViewItemColumn(int column, bool descending)
        {
            this.column = column;
            this.descending = descending;
        }
        #endregion

        #region Private Fields
        private int column = 0;
        private bool descending = true;
        #endregion

        #region Public IComparer Implementation Methods
        /// <summary>
        /// Compares to list view item columns with each other,
        /// returns the specified sort direction results.
        /// </summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>The compared index. Positive or negative.</returns>
        public virtual int Compare(ListViewItem x, ListViewItem y)
        {
            // If the data should be set to descending order
            // ascending order, return positive integer for
            // decending and negative for ascending.
            if(descending)
                return string.Compare(x.SubItems[column].Text, y.SubItems[column].Text);
            else
                return -(string.Compare(x.SubItems[column].Text, y.SubItems[column].Text));
        }
        #endregion
    }

    /// <summary>
    /// To upper comparer.
    /// </summary>
    public class ToUpperComparer : IEqualityComparer<string>
    {
        #region Public Comparer Methods
        /// <summary>
        /// Compares the table name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(string x, string y)
        {
            return x.ToUpper() == y.ToUpper();
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(string obj)
        {
            return obj.ToUpper().GetHashCode();
        }
        #endregion
    }

    /// <summary>
    /// To lower comparer.
    /// </summary>
    public class ToLowerComparer : IEqualityComparer<string>
    {
        #region Public Comparer Methods
        /// <summary>
        /// Compares the table name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(string x, string y)
        {
            return x.ToLower() == y.ToLower();
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(string obj)
        {
            return obj.ToLower().GetHashCode();
        }
        #endregion
    }

    /// <summary>
    /// String comparer.
    /// </summary>
    public class StringComparer : IEqualityComparer<string>
    {
        #region Public Comparer Methods
        /// <summary>
        /// Compares the table name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(string x, string y)
        {
            return x == y;
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
        #endregion
    }

    /// <summary>
    /// Integer comparer.
    /// </summary>
    public class IntegerComparer : IEqualityComparer<int>
    {
        #region Public Comparer Methods
        /// <summary>
        /// Compares the table name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(int x, int y)
        {
            return x == y;
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(int obj)
        {
            return obj.GetHashCode();
        }
        #endregion
    }

    /// <summary>
    /// LongInteger comparer.
    /// </summary>
    public class LongIntegerComparer : IEqualityComparer<long>
    {
        #region Public Comparer Methods
        /// <summary>
        /// Compares the table name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(long x, long y)
        {
            return x == y;
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(long obj)
        {
            return obj.GetHashCode();
        }
        #endregion
    }

    /// <summary>
    /// Single comparer.
    /// </summary>
    public class SingleComparer : IEqualityComparer<float>
    {
        #region Public Comparer Methods
        /// <summary>
        /// Compares the table name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(float x, float y)
        {
            return x == y;
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
        #endregion
    }

    /// <summary>
    /// Double comparer.
    /// </summary>
    public class DoubleComparer : IEqualityComparer<double>
    {
        #region Public Comparer Methods
        /// <summary>
        /// Compares the table name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(double x, double y)
        {
            return x == y;
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(double obj)
        {
            return obj.GetHashCode();
        }
        #endregion
    }

    /// <summary>
    /// Decimal comparer.
    /// </summary>
    public class DecimalComparer : IEqualityComparer<decimal>
    {
        #region Public Comparer Methods
        /// <summary>
        /// Compares the table name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(decimal x, decimal y)
        {
            return x == y;
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(decimal obj)
        {
            return obj.GetHashCode();
        }
        #endregion
    }

    /// <summary>
    /// DateTime comparer.
    /// </summary>
    public class DateTimeComparer : IEqualityComparer<DateTime>
    {
        #region Public Comparer Methods
        /// <summary>
        /// Compares the table name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(DateTime x, DateTime y)
        {
            return x == y;
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(DateTime obj)
        {
            return obj.GetHashCode();
        }
        #endregion
    }

    /// <summary>
    /// Array manipulation class.
    /// </summary>
    public sealed class ArrayComparer
    {
        #region Public Intersection Of Arrays Methods
        /// <summary>
        /// Finds the intersection of two arrays.
        /// </summary>
        /// <typeparam name="TSource">The array type to compare.</typeparam>
        /// <param name="arrayA">First array of elements to match.</param>
        /// <param name="arrayB">Second array of elements to match.</param>
        /// <param name="isEachArraySorted">Are the arrays sorted.</param>
        /// <returns>An array of intersecting elements else null.</returns>
        /// <remarks>If the arrays are sorted they must be sorted in the same direction.</remarks>
        public static TSource[] Intersection<TSource>(TSource[] arrayA, TSource[] arrayB, bool isEachArraySorted)
        {
            if (isEachArraySorted)
                return IntersectionSorted<TSource>(arrayA, arrayB);
            else
                return IntersectionUnSorted<TSource>(arrayA, arrayB);
        }

        /// <summary>
        /// Does an intersection exist of two arrays.
        /// </summary>
        /// <typeparam name="TSource">The array type to compare.</typeparam>
        /// <param name="arrayA">First array of elements to match.</param>
        /// <param name="arrayB">Second array of elements to match.</param>
        /// <param name="isEachArraySorted">Are the arrays sorted.</param>
        /// <returns>True if an intersection exists; else false.</returns>
        /// <remarks>If the arrays are sorted they must be sorted in the same direction.</remarks>
        public static bool IntersectionExists<TSource>(TSource[] arrayA, TSource[] arrayB, bool isEachArraySorted)
        {
            if (isEachArraySorted)
            {
                return (IntersectionSorted<TSource>(arrayA, arrayB) != null ? true : false);
            }
            else
            {
                return (IntersectionUnSorted<TSource>(arrayA, arrayB) != null ? true : false);
            }
        }

        /// <summary>
        /// Sorts an array of elements, descending or ascending.
        /// </summary>
        /// <typeparam name="TSource">The array type to sort.</typeparam>
        /// <param name="array">The array of elements to sort.</param>
        /// <param name="descending">Sort in decending order else ascending.</param>
        /// <returns>The sorted array of elements.</returns>
        public static TSource[] Sort<TSource>(TSource[] array, bool descending)
        {
            // Make sure that each array
            // instance has been created.
            if ((array != null))
            {
                TSource[] arrayX = array;
                TSource[] arrayZ = null;

                // Sort the array in descending order.
                if (descending)
                {
                    // Sort the array, in ascending order.
                    Array.Sort<TSource>(arrayX);

                    // Create a new array of elements,
                    // set the initial index to the
                    // maximum value.
                    arrayZ = new TSource[arrayX.Length];
                    int i = arrayX.Length - 1;

                    // For each element in the array.
                    foreach (TSource element in arrayX)
                        // Assign the new array
                        arrayZ[i--] = element;

                    // Descending sorted array.
                    return arrayZ;
                }
                else
                    // Ascending sorted array.
                    Array.Sort<TSource>(arrayX);

                // Return the new sorted array.
                return arrayX;
            }
            else
                return null;
        }

        /// <summary>
        /// Gets the array of unique items and number within a sorted list.
        /// </summary>
        /// <typeparam name="TSource">The array type to compare.</typeparam>
        /// <param name="array">The array of a sorted type to extract unique items from.</param>
        /// <param name="uniquenessCount">The array of unique item numbers.</param>
        /// <returns>The array of unique items.</returns>
        public static TSource[] Uniqueness<TSource>(TSource[] array, ref int[] uniquenessCount)
        {
            // Make sure that each array
            // instance has been created.
            if ((array != null))
            {
                int currentUniqueCounter = 0;
                int numberOfItems = array.Length;
                int lastItemIndex = numberOfItems - 1;

                // Create a new generic list object
                // with integer elements.
                List<TSource> elements = new List<TSource>();
                List<int> uniquenessCountElements = new List<int>();

                // For each item extact unique items.
                for (int i = 0; i < array.Length; i++)
                {
                    // For the first item.
                    if (i < 1)
                    {
                        // Add the first item to the list
                        // and increment the current item count.
                        elements.Add(array[i]);
                        currentUniqueCounter += 1;
                    }
                    else
                    {
                        // If the current item is equal to the previous item
                        // then icement the current item count.
                        if (array[i - 1].Equals(array[i]))
                        {
                            // Increment the current item count.
                            currentUniqueCounter += 1;
                        }
                        else
                        {
                            // Add the current count of elements
                            // and reset the current item count to zero.
                            uniquenessCountElements.Add(currentUniqueCounter);
                            currentUniqueCounter = 0;

                            // Add the first item to the list
                            // and increment the current item count.
                            elements.Add(array[i]);
                            currentUniqueCounter += 1;
                        }
                    }

                    // For the last item.
                    if (i == lastItemIndex)
                    {
                        // Add the current count of elements
                        // and reset the current item count to zero.
                        uniquenessCountElements.Add(currentUniqueCounter);
                        currentUniqueCounter = 0;
                    }
                }

                // If no elements have been created.
                if (elements.ToArray().Length > 0)
                    uniquenessCount = uniquenessCountElements.ToArray();
                else
                    uniquenessCount = new int[0];

                // If the number of elements found is greater than
                // zero then return the match else return an array
                // with no elements.
                return elements.ToArray().Length > 0 ? elements.ToArray() : null;
            }
            else
            {
                uniquenessCount = null;
                return null;
            }
        }

        /// <summary>
        /// Finds the intersection of two un-sorted arrays.
        /// </summary>
        /// <typeparam name="TSource">The array type to compare.</typeparam>
        /// <param name="arrayA">First array of elements to match.</param>
        /// <param name="arrayB">Second array of elements to match.</param>
        /// <returns>An array of intersecting elements else null.</returns>
        private static TSource[] IntersectionUnSorted<TSource>(TSource[] arrayA, TSource[] arrayB)
        {
            // Make sure that each array
            // instance has been created.
            if ((arrayA != null) && (arrayB != null))
            {
                // For each element in array A,
                // and each element in array B
                // make sure than there are elements
                // to compare.
                if ((arrayA.Length > 0) && (arrayB.Length > 0))
                {
                    // Create a new generic list object
                    // with integer elements.
                    List<TSource> elements = new List<TSource>();

                    TSource[] arrayX = null;
                    TSource[] arrayZ = null;

                    // If array A contains less elements
                    // than array B then the top loop
                    // is always the smaller loop.
                    if (arrayA.Length < arrayB.Length)
                    {
                        arrayX = arrayA;
                        arrayZ = arrayB;
                    }
                    else
                    {
                        // Reverse assign the smaller loop
                        // to the top level nested loop.
                        arrayX = arrayB;
                        arrayZ = arrayA;
                    }

                    // Forach each element in the smallest
                    // array.
                    foreach (TSource elementX in arrayX)
                        foreach (TSource elementZ in arrayZ)
                            // If a match has been found then
                            // add the element into list collection,
                            // break out of the inner loop.
                            if (elementX.Equals(elementZ))
                            {
                                elements.Add(elementX);
                                break;
                            }

                    // If the number of elements found is greater than
                    // zero then return the match else return an array
                    // with no elements.
                    return elements.ToArray().Length > 0 ? elements.ToArray() : null;
                }
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// Finds the intersection of two sorted arrays.
        /// </summary>
        /// <typeparam name="TSource">The array type to compare.</typeparam>
        /// <param name="arrayA">First array of elements to match.</param>
        /// <param name="arrayB">Second array of elements to match.</param>
        /// <returns>An array of intersecting elements else null.</returns>
        private static TSource[] IntersectionSorted<TSource>(TSource[] arrayA, TSource[] arrayB)
        {
            // Make sure that each array
            // instance has been created.
            if ((arrayA != null) && (arrayB != null))
            {
                // For each element in array A,
                // and each element in array B
                // make sure than there are elements
                // to compare.
                if ((arrayA.Length > 0) && (arrayB.Length > 0))
                {
                    // Create a new generic list object
                    // with integer elements.
                    List<TSource> elements = new List<TSource>();

                    // Applying the index j to the inner
                    // loop only works when each array
                    // has been sorted. If a match has 
                    // been found then move to position
                    // 'j' no need to process proceeding
                    // values.
                    int j = 0;

                    TSource[] arrayX = null;
                    TSource[] arrayZ = null;

                    // If array A contains less elements
                    // than array B then the top loop
                    // is always the smaller loop.
                    if (arrayA.Length < arrayB.Length)
                    {
                        arrayX = arrayA;
                        arrayZ = arrayB;
                    }
                    else
                    {
                        // Reverse assign the smaller loop
                        // to the top level nested loop.
                        arrayX = arrayB;
                        arrayZ = arrayA;
                    }

                    // Forach each element in the smallest
                    // array.
                    foreach (TSource elementX in arrayX)
                        // This loop should only be
                        // applied when all array elements 
                        // have been sorted.
                        for (int i = j; i < arrayZ.Length; i++)
                            // If a match has been found then
                            // add the element into list collection,
                            // break out of the inner loop.
                            if (elementX.Equals(arrayZ[i]))
                            {
                                elements.Add(elementX);
                                j = i + 1;
                                break;
                            }

                    // If the number of elements found is greater than
                    // zero then return the match else return an array
                    // with no elements.
                    return elements.ToArray().Length > 0 ? elements.ToArray() : null;
                }
                else
                    return null;
            }
            else
                return null;
        }
        #endregion
    }

    /// <summary>
    /// Custom array provider.
    /// </summary>
    public sealed class Arrays
    {
        #region Public Custom array provider. Methods
        /// <summary>
        /// Custom array provider.
        /// </summary>
        public Arrays()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool AreEqual(
            bool[] a,
            bool[] b)
        {
            if (a == b)
                return true;

            if (a == null || b == null)
                return false;

            return HaveSameContents(a, b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool AreEqual(
            char[] a,
            char[] b)
        {
            if (a == b)
                return true;

            if (a == null || b == null)
                return false;

            return HaveSameContents(a, b);
        }

        /// <summary>
        /// Are two arrays equal.
        /// </summary>
        /// <param name="a">Left side.</param>
        /// <param name="b">Right side.</param>
        /// <returns>True if equal.</returns>
        public static bool AreEqual(
            byte[] a,
            byte[] b)
        {
            if (a == b)
                return true;

            if (a == null || b == null)
                return false;

            return HaveSameContents(a, b);
        }

        /// <summary>
        /// A constant time equals comparison - does not terminate early if
        /// test will fail.
        /// </summary>
        /// <param name="a">first array</param>
        /// <param name="b">second array</param>
        /// <returns>true if arrays equal, false otherwise.</returns>
        public static bool ConstantTimeAreEqual(
            byte[] a,
            byte[] b)
        {
            int i = a.Length;
            if (i != b.Length)
                return false;
            int cmp = 0;
            while (i != 0)
            {
                --i;
                cmp |= (a[i] ^ b[i]);
            }
            return cmp == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool AreEqual(
            int[] a,
            int[] b)
        {
            if (a == b)
                return true;

            if (a == null || b == null)
                return false;

            return HaveSameContents(a, b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool HaveSameContents(
            bool[] a,
            bool[] b)
        {
            int i = a.Length;
            if (i != b.Length)
                return false;
            while (i != 0)
            {
                --i;
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool HaveSameContents(
            char[] a,
            char[] b)
        {
            int i = a.Length;
            if (i != b.Length)
                return false;
            while (i != 0)
            {
                --i;
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool HaveSameContents(
            byte[] a,
            byte[] b)
        {
            int i = a.Length;
            if (i != b.Length)
                return false;
            while (i != 0)
            {
                --i;
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool HaveSameContents(
            int[] a,
            int[] b)
        {
            int i = a.Length;
            if (i != b.Length)
                return false;
            while (i != 0)
            {
                --i;
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string ToString(
            object[] a)
        {
            StringBuilder sb = new StringBuilder('[');
            if (a.Length > 0)
            {
                sb.Append(a[0]);
                for (int index = 1; index < a.Length; ++index)
                {
                    sb.Append(", ").Append(a[index]);
                }
            }
            sb.Append(']');
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int GetHashCode(
            byte[] data)
        {
            if (data == null)
            {
                return 0;
            }

            int i = data.Length;
            int hc = i + 1;

            while (--i >= 0)
            {
                hc *= 257;
                hc ^= data[i];
            }

            return hc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Clone(
            byte[] data)
        {
            return data == null ? null : (byte[])data.Clone();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int[] Clone(
            int[] data)
        {
            return data == null ? null : (int[])data.Clone();
        }
        #endregion
    }
}
