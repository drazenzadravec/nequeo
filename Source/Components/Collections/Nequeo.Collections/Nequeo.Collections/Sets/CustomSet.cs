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
using System.Collections;
using System.Text;

namespace Nequeo.Collections.Sets
{
    /// <summary>
    /// Custom implemention set class that uses the efficient bit-wise operators the set's universe must be known.
    /// </summary>
    public class CustomSet : ICloneable, ICollection, IEnumerable
    {
        // Private member variables
        private int lowerBound, upperBound;
        private BitArray data;

        #region Constructors
        /// <summary>
        /// Creates a new CustomSet instance with a specified lower and upper bound.
        /// </summary>
        /// <param name="lowerBound">The lower bound for the set.  Can be any legal character value.</param>
        /// <param name="upperBound">The upper bound for the set.  Can be any legal character value.</param>
        public CustomSet(char lowerBound, char upperBound) : this((int) lowerBound, (int) upperBound) { }

        /// <summary>
        /// Creates a new CustomSet instance with a specified lower and upper bound.
        /// </summary>
        /// <param name="lowerBound">The lower bound for the set.  Can be any legal integer value.</param>
        /// <param name="upperBound">The upper bound for the set.  Can be any legal integer value.</param>
        public CustomSet(int lowerBound, int upperBound)
        {
            // make sure lowerbound is less than or equal to upperbound
            if (lowerBound > upperBound)
                throw new ArgumentException("The set's lower bound cannot be greater than its upper bound.");

            this.lowerBound = lowerBound;
            this.upperBound = upperBound;

            // Create the BitArray
            data = new BitArray(upperBound - lowerBound + 1);
        }

        /// <summary>
        /// Creates a new CustomSet instance whose initial values are assigned from an integer array.
        /// </summary>
        /// <param name="lowerBound">The lower bound for the set.  Can be any legal integer value.</param>
        /// <param name="upperBound">The upper bound for the set.  Can be any legal integer value.</param>
        /// <param name="initialData">An integer array that is used as the initial values of the array.</param>
        public CustomSet(int lowerBound, int upperBound, int[] initialData)
        {
            // make sure lowerbound is less than or equal to upperbound
            if (lowerBound > upperBound)
                throw new ArgumentException("The set's lower bound cannot be greater than its upper bound.");

            this.lowerBound = lowerBound;
            this.upperBound = upperBound;

            // Create the BitArray
            data = new BitArray(upperBound - lowerBound + 1);

            // Populuate the BitArray with the passed-in initialData array.
            for (int i = 0; i < initialData.Length; i++)
            {
                int val = initialData[i];
                if (val >= this.lowerBound && val <= this.upperBound)
                    data.Set(val - this.lowerBound, true);
                else
                    throw new ArgumentException("Attempting to add an element with value " + val.ToString() + " that is outside of the set's universe.  Value must be between " + this.lowerBound.ToString() + " and " + this.upperBound.ToString());
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Determines if two CustomSets are "compatible."  Specifically, it checks to ensure that the CustomSets
        /// share the same lower and upper bounds.
        /// </summary>
        /// <param name="s">The custom set to compare.</param>
        /// <returns><b>True</b> if the CustomSets share the same bounds, <b>False</b> otherwise.</returns>
        protected virtual bool AreSimilar(CustomSet s)
        {
            return this.lowerBound == s.lowerBound && this.upperBound == s.upperBound;
        }

        #region Union
        /// <summary>
        /// Unions a set of integers with the current CustomSet.
        /// </summary>
        /// <param name="list">An variable number of integers.</param>
        /// <returns>A new CustomSet, which is the union of the <b>this</b> CustomSet and the passed-in integers.</returns>
        public virtual CustomSet Union(params int[] list)
        {
            // create a deep copy of this
            CustomSet result = (CustomSet)Clone();

            // For each integer passed in, if it's within the bounds add it to the results's BitArray.
            for (int i = 0; i < list.Length; i++)
            {
                int val = list[i];
                if (val >= this.lowerBound && val <= this.upperBound)
                    result.data.Set(val - this.lowerBound, true);
                else
                    throw new ArgumentException("Attempting to add an element with value " + val.ToString() + " that is outside of the set's universe.  Value must be between " + this.lowerBound.ToString() + " and " + this.upperBound.ToString());
            }

            return result;		// return the new CustomSet
        }

        /// <summary>
        /// Unions a set of characters with the current CustomSet.
        /// </summary>
        /// <param name="list">A variable number of characters.</param>
        /// <returns>A new CustomSet, which is the union of the <b>this</b> CustomSet and the passed-in characters.</returns>
        public virtual CustomSet Union(params char[] list)
        {
            int[] intForm = new int[list.Length];
            Array.Copy(list, intForm, list.Length);
            return Union(intForm);
        }

        /// <summary>
        /// Unions a passed-in CustomSet with the current CustomSet.
        /// </summary>
        /// <param name="s">A CustomSet.</param>
        /// <returns>A new CustomSet whose elements are the union of <b>s</b> and <b>this</b>.</returns>
        /// <remarks><b>s</b> and <b>this</b> must be "similar" CustomSets.</remarks>
        public virtual CustomSet Union(CustomSet s)
        {
            if (!AreSimilar(s))
                throw new ArgumentException("Attempting to union two dissimilar sets.  Union can only occur between two sets with the same universe.");

            // do a bit-wise OR to union together this.data and s.data
            CustomSet result = (CustomSet)Clone();
            result.data.Or(s.data);

            return result;
        }

        /// <summary>
        /// Overloaded + operator for union...
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static CustomSet operator +(CustomSet s, CustomSet t)
        {
            return s.Union(t);
        }
        #endregion

        #region Intersection
        /// <summary>
        /// Intersects a set of integers with the current CustomSet.
        /// </summary>
        /// <param name="list">An variable number of integers.</param>
        /// <returns>A new CustomSet, which is the intersection of the <b>this</b> CustomSet and the passed-in integers.</returns>
        public virtual CustomSet Intersection(params int[] list)
        {
            CustomSet result = new CustomSet(this.lowerBound, this.upperBound);

            for (int i = 0; i < list.Length; i++)
            {
                // only add the element to result if its in this.data
                int val = list[i];
                if (val >= this.lowerBound && val <= this.upperBound)
                    if (this.data.Get(val - this.lowerBound))
                        result.data.Set(val - this.lowerBound, true);
            }

            return result;
        }

        /// <summary>
        /// Intersects a set of characters with the current CustomSet.
        /// </summary>
        /// <param name="list">A variable number of characters.</param>
        /// <returns>A new CustomSet, which is the intersection of the <b>this</b> CustomSet and the passed-in characters.</returns>
        public virtual CustomSet Intersection(params char[] list)
        {
            int[] intForm = new int[list.Length];
            Array.Copy(list, intForm, list.Length);
            return Intersection(intForm);
        }

        /// <summary>
        /// Intersects a passed-in CustomSet with the current CustomSet.
        /// </summary>
        /// <param name="s">A CustomSet.</param>
        /// <returns>A new CustomSet whose elements are the intersection of <b>s</b> and <b>this</b>.</returns>
        /// <remarks><b>s</b> and <b>this</b> must be "similar" CustomSets.</remarks>
        public virtual CustomSet Intersection(CustomSet s)
        {
            if (!AreSimilar(s))
                throw new ArgumentException("Attempting to intersect two dissimilar sets.  Intersection can only occur between two sets with the same universe.");

            // do a bit-wise AND to intersect this.data and s.data
            CustomSet result = (CustomSet)Clone();
            result.data.And(s.data);

            return result;
        }

        /// <summary>
        /// Overloaded * operator for intersection
        /// </summary>
        /// <param name="s">CustomSet</param>
        /// <param name="t">CustomSet</param>
        /// <returns>CustomSet</returns>
        public static CustomSet operator *(CustomSet s, CustomSet t)
        {
            return s.Intersection(t);
        }
        #endregion

        #region Difference
        /// <summary>
        /// Set differences a set of integers with the current CustomSet.
        /// </summary>
        /// <param name="list">An variable number of integers.</param>
        /// <returns>A new CustomSet, which is the set difference of the <b>this</b> CustomSet and the passed-in integers.</returns>
        public virtual CustomSet Difference(params int[] list)
        {
            CustomSet result = new CustomSet(this.lowerBound, this.upperBound, list);
            return Difference(result);
        }

        /// <summary>
        /// Set differences a set of characters with the current CustomSet.
        /// </summary>
        /// <param name="list">A variable number of characters.</param>
        /// <returns>A new CustomSet, which is the set difference of the <b>this</b> CustomSet and the passed-in characters.</returns>
        public virtual CustomSet Difference(params char[] list)
        {
            int[] intForm = new int[list.Length];
            Array.Copy(list, intForm, list.Length);
            return Difference(intForm);
        }

        /// <summary>
        /// Set differences a passed-in CustomSet with the current CustomSet.
        /// </summary>
        /// <param name="s">A CustomSet.</param>
        /// <returns>A new CustomSet whose elements are the set difference of <b>s</b> and <b>this</b>.</returns>
        /// <remarks><b>s</b> and <b>this</b> must be "similar" CustomSets.</remarks>
        public virtual CustomSet Difference(CustomSet s)
        {
            if (!AreSimilar(s))
                throw new ArgumentException("Attempting to apply set difference to two dissimilar sets.  Set difference can only occur between two sets with the same universe.");

            // do a bit-wise XOR and then an AND to achieve set difference
            CustomSet result = (CustomSet)Clone();
            result.data.Xor(s.data).And(this.data);

            return result;
        }

        /// <summary>
        /// Overloaded - operator for set difference
        /// </summary>
        /// <param name="s">CustomSet</param>
        /// <param name="t">CustomSet</param>
        /// <returns>CustomSet</returns>
        public static CustomSet operator -(CustomSet s, CustomSet t)
        {
            return s.Difference(t);
        }
        #endregion

        #region Complement
        /// <summary>
        /// Complements a CustomSet.
        /// </summary>
        /// <returns>A new CustomSet that is the complement of <b>this</b>.</returns>
        public virtual CustomSet Complement()
        {
            CustomSet result = (CustomSet)Clone();
            result.data.Not();
            return result;
        }
        #endregion

        #region Element Of
        /// <summary>
        /// Determines if a passed-in value is an element of the CustomSet.
        /// </summary>
        /// <param name="x">The integer to check if it exists in the set.</param>
        /// <returns><b>True</b> is <b>x</b> is in the set, <b>False</b> otherwise</returns>
        public virtual bool ContainsElement(int x)
        {
            if (x < lowerBound || x > upperBound)
                return false;

            return this.data.Get(x - lowerBound);
        }

        /// <summary>
        /// Determines if a passed-in value is an element of the CustomSet.
        /// </summary>
        /// <param name="x">The character to check if it exists in the set.</param>
        /// <returns><b>True</b> is <b>x</b> is in the set, <b>False</b> otherwise</returns>
        public virtual bool ContainsElement(char x)
        {
            return ContainsElement((int)x);
        }
        #endregion

        #region Subset
        /// <summary>
        /// Determins if this set is a subset of the integers passed-in.
        /// </summary>
        /// <param name="list">A variable number of integers.</param>
        /// <returns><b>True</b> if <b>this</b> is a subset of the passed-in integers; <b>False</b> otherwise.</returns>
        public virtual bool Subset(params int[] list)
        {
            CustomSet temp = new CustomSet(this.lowerBound, this.upperBound, list);
            return Subset(temp);
        }

        /// <summary>
        /// Determins if this set is a subset of the characters passed-in.
        /// </summary>
        /// <param name="list">A variable number of characters.</param>
        /// <returns><b>True</b> if <b>this</b> is a subset of the passed-in characters; <b>False</b> otherwise.</returns>
        public virtual bool Subset(params char[] list)
        {
            int[] intForm = new int[list.Length];
            Array.Copy(list, intForm, list.Length);
            return Subset(intForm);
        }

        /// <summary>
        /// Determins if this set is a subset of the passed-in CustomSet.
        /// </summary>
        /// <param name="s">A CustomSet that is "similar" to <b>this</b>.</param>
        /// <returns><b>True</b> if <b>this</b> is a subset of <b>s</b>; <b>False</b> otherwise.</returns>
        public virtual bool Subset(CustomSet s)
        {
            if (!AreSimilar(s))
                throw new ArgumentException("Attempting to compare two dissimilar sets.  Subset comparisons can only occur between two sets with the same universe.");

            // Get the BitArray's underlying array
			const int INT_SIZE = 32;
            int arraySize = (data.Length + INT_SIZE - 1) / INT_SIZE;
            int[] thisBits = new int[arraySize];
            int[] sBits = new int[arraySize];
            data.CopyTo(thisBits, 0);
            s.data.CopyTo(sBits, 0);

            // now, enumerate through the int array elements
            for (int i = 0; i < thisBits.Length; i++)
            {
                // do a bitwise AND between thisBits[i] and sBits[i];
                int result = thisBits[i] & sBits[i];

                // see if result == thisBits[i] - if it doesn't, then not a subset
                if (result != thisBits[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determins if this set is a proper subset of the integers passed-in.
        /// </summary>
        /// <param name="list">A variable number of integers.</param>
        /// <returns><b>True</b> if <b>this</b> is a proper subset of the passed-in integers; <b>False</b> otherwise.</returns>
        public virtual bool ProperSubset(params int[] list)
        {
            CustomSet temp = new CustomSet(this.lowerBound, this.upperBound, list);
            return ProperSubset(temp);
        }

        /// <summary>
        /// Determins if this set is a proper subset of the characters passed-in.
        /// </summary>
        /// <param name="list">A variable number of characters.</param>
        /// <returns><b>True</b> if <b>this</b> is a proper subset of the passed-in characters; <b>False</b> otherwise.</returns>
        public virtual bool ProperSubset(params char[] list)
        {
            int[] intForm = new int[list.Length];
            Array.Copy(list, intForm, list.Length);
            return ProperSubset(intForm);
        }

        /// <summary>
        /// Determins if this set is a proper subset of the passed-in CustomSet.
        /// </summary>
        /// <param name="s">A CustomSet that is "similar" to <b>this</b>.</param>
        /// <returns><b>True</b> if <b>this</b> is a proper subset of <b>s</b>; <b>False</b> otherwise.</returns>
        public virtual bool ProperSubset(CustomSet s)
        {
            if (!AreSimilar(s))
                throw new ArgumentException("Attempting to compare two dissimilar sets.  Subset comparisons can only occur between two sets with the same universe.");

            return Subset(s) && !s.Subset(this);
        }
        #endregion

        #region Superset
        /// <summary>
        /// Determins if this set is a superset of the integers passed-in.
        /// </summary>
        /// <param name="list">A variable number of integers.</param>
        /// <returns><b>True</b> if <b>this</b> is a superset of the passed-in integers; <b>False</b> otherwise.</returns>
        public virtual bool Superset(params int[] list)
        {
            CustomSet temp = new CustomSet(this.lowerBound, this.upperBound, list);
            return Superset(temp);
        }

        /// <summary>
        /// Determins if this set is a superset of the characters passed-in.
        /// </summary>
        /// <param name="list">A variable number of characters.</param>
        /// <returns><b>True</b> if <b>this</b> is a superset of the passed-in characters; <b>False</b> otherwise.</returns>
        public virtual bool Superset(params char[] list)
        {
            int[] intForm = new int[list.Length];
            Array.Copy(list, intForm, list.Length);
            return Superset(intForm);
        }

        /// <summary>
        /// Determins if this set is a superset of the passed-in CustomSet.
        /// </summary>
        /// <param name="s">A CustomSet that is "similar" to <b>this</b>.</param>
        /// <returns><b>True</b> if <b>this</b> is a superset of <b>s</b>; <b>False</b> otherwise.</returns>
        public virtual bool Superset(CustomSet s)
        {
            if (!AreSimilar(s))
                throw new ArgumentException("Attempting to compare two dissimilar sets.  Superset comparisons can only occur between two sets with the same universe.");

            return s.Subset(this);
        }

        /// <summary>
        /// Determins if this set is a proper superset of the integers passed-in.
        /// </summary>
        /// <param name="list">A variable number of integers.</param>
        /// <returns><b>True</b> if <b>this</b> is a proper superset of the passed-in integers; <b>False</b> otherwise.</returns>
        public virtual bool ProperSuperset(params int[] list)
        {
            CustomSet temp = new CustomSet(this.lowerBound, this.upperBound, list);
            return ProperSuperset(temp);
        }

        /// <summary>
        /// Determins if this set is a proper superset of the characters passed-in.
        /// </summary>
        /// <param name="list">A variable number of characters.</param>
        /// <returns><b>True</b> if <b>this</b> is a proper superset of the passed-in characters; <b>False</b> otherwise.</returns>
        public virtual bool ProperSuperset(params char[] list)
        {
            int[] intForm = new int[list.Length];
            Array.Copy(list, intForm, list.Length);
            return ProperSuperset(intForm);
        }

        /// <summary>
        /// Determins if this set is a proper superset of the passed-in CustomSet.
        /// </summary>
        /// <param name="s">A CustomSet that is "similar" to <b>this</b>.</param>
        /// <returns><b>True</b> if <b>this</b> is a proper superset of <b>s</b>; <b>False</b> otherwise.</returns>
        public virtual bool ProperSuperset(CustomSet s)
        {
            if (!AreSimilar(s))
                throw new ArgumentException("Attempting to compare two dissimilar sets.  Superset comparisons can only occur between two sets with the same universe.");

            return Superset(s) && !s.Superset(this);
        }
        #endregion
        #endregion

        #region CustomSet Properties
        /// <summary>
        /// Gets the lower bound of the set.
        /// </summary>
        public virtual int LowerBound
        {
            get
            {
                return this.lowerBound;
            }
        }

        /// <summary>
        /// Gets the upper bound of the set.
        /// </summary>
        public virtual int UpperBound
        {
            get
            {
                return this.upperBound;
            }
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Clones the CustomSet, performing a deep copy.
        /// </summary>
        /// <returns>A new instance of a CustomSet, using a deep copy.</returns>
        public object Clone()
        {
            CustomSet p = new CustomSet(lowerBound, upperBound);
            p.data = new BitArray(this.data);
            return p;
        }
        #endregion

        #region ICollection Members

        /// <summary>
        /// Gets a value indicating whether access to the ICollection is synchronized (thread-safe).
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the cardinality of the set.
        /// </summary>
        public int Count
        {
            get
            {
                int elements = 0;
                for (int i = 0; i < data.Length; i++)
                    if (data.Get(i)) elements++;

                return elements;
            }
        }

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
        /// </summary>
        public void CopyTo(Array array, int index)
        {
            data.CopyTo(array, index);
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the ICollection.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns an IEnumerator to enumerate through the set.
        /// </summary>
        /// <returns>An IEnumerator instance.</returns>
        public IEnumerator GetEnumerator()
        {
            int totalElements = Count;
            int itemsReturned = 0;
            for (int i = 0; i < this.data.Length; i++)
            {
                if (itemsReturned >= totalElements)
                    break;
                else if (this.data.Get(i))
                    yield return i + this.lowerBound;
            }
        }
        #endregion
    }
}
