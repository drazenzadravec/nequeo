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
using System.Collections.Generic;
using System.Text;

namespace Nequeo.Collections.SkipLists
{
    /// <summary>
    /// Represnts a SkipList.  A SkipList is a combination of a BST and a sorted link list, providing
    /// sub-linear access, insert, and deletion running times.  It is a randomized data structure, randomly
    /// choosing the heights of the nodes in the SkipList.
    /// </summary>
    /// <typeparam name="T">Type type of elements contained within the SkipList.</typeparam>
    public class SkipList<T> : ICollection<T>, IEnumerable<T>
    {
        #region Private Member Variables
        SkipListNode<T> _head;      // a reference to the head of the SkipList
        private long _comparisons;      // an internal counter used for performance testing of the SkipList
        private int _count;
        private Random _rndNum;
        private IComparer<T> comparer = Comparer<T>.Default;

        /// <summary>
        /// the probability used in determining the heights of the SkipListNodes
        /// </summary>
        protected readonly double Prob = 0.5;
        #endregion

        #region Constructors
        /// <summary>
        /// Represnts a SkipList.
        /// </summary>
        public SkipList() : this(-1, null) { }

        /// <summary>
        /// Represnts a SkipList.
        /// </summary>
        /// <param name="randomSeed">The random seed.</param>
        public SkipList(int randomSeed) : this(randomSeed, null) { }

        /// <summary>
        /// Represnts a SkipList.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public SkipList(IComparer<T> comparer) : this(-1, comparer) { }

        /// <summary>
        /// Represnts a SkipList.
        /// </summary>
        /// <param name="randomSeed">The random seed.</param>
        /// <param name="comparer">The comparer.</param>
        public SkipList(int randomSeed, IComparer<T> comparer)
        {
            _head = new SkipListNode<T>(1);
            _comparisons = 0;
            _count = 0;
            _count++;
            if (randomSeed < 0)
                _rndNum = new Random();
            else
                _rndNum = new Random(randomSeed);

            if (comparer != null) this.comparer = comparer;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Selects a height for a new SkipListNode using the "loaded dice" technique.
        /// The value selected is between 1 and maxLevel.
        /// </summary>
        /// <param name="maxLevel">The maximum value ChooseRandomHeight can return.</param>
        /// <returns>A randomly chosen integer value between 1 and maxLevel.</returns>
        protected virtual int ChooseRandomHeight(int maxLevel)
        {
            int level = 1;

            while (_rndNum.NextDouble() < Prob && level < maxLevel)
                level++;

            return level;
        }

        #region ICollection Methods and Related Helpers
        #region Clear

        /// <summary>
        /// Clears out the contents of the SkipList and creates a new head, with height 1.
        /// </summary>
        public void Clear()
        {
            // create a new head
            _head = null;
            _head = new SkipListNode<T>(1);
            _count = 0;
        }
        #endregion

        #region Contains
        /// <summary>
        /// Determines if a particular element is contained within the SkipList.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <returns>True if value is found in the SkipList; false otherwise.</returns>
        public bool Contains(T value)
        {
            SkipListNode<T> current = _head;

            // first, determine the nodes that need to be updated at each level
            for (int i = _head.Height - 1; i >= 0; i--)
            {
                while (current[i] != null)
                {
                    _comparisons++;
                    int results = comparer.Compare(current[i].Value, value);
                    if (results == 0)
                        return true;        // we found the item
                    else if (results < 0)
                        current = current[i];   // move on to the next neighbor
                    else
                        break;	// exit while loop, we need to move down the height of the current node
                }
            }

            // if we reach here, we searched to the end of the list without finding the element
            return false;
        }
        #endregion

        #region Add
        /// <summary>
        /// Adds a new element to the SkipList.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <remarks>This SkipList implementation does not allow for duplicates.  Attempting to add a
        /// duplicate value will not raise an exception, it will simply exit the method without
        /// changing the SkipList.</remarks>
        public void Add(T value)
        {
            SkipListNode<T>[] updates = BuildUpdateTable(value);
            SkipListNode<T> current = updates[0];

            // see if a duplicate is being inserted
            if (current[0] != null && comparer.Compare(current[0].Value, value) == 0)
                // cannot enter a duplicate, handle this case by either just returning or by throwing an exception
                return;

            // create a new node
            SkipListNode<T> n = new SkipListNode<T>(value, ChooseRandomHeight(_head.Height + 1));
            _count++;

            // if the node's level is greater than the head's level, increase the head's level
            if (n.Height > _head.Height)
            {
                _head.IncrementHeight();
                _head[_head.Height - 1] = n;
            }

            // splice the new node into the list
            for (int i = 0; i < n.Height; i++)
            {
                if (i < updates.Length)
                {
                    n[i] = updates[i][i];
                    updates[i][i] = n;
                }
            }
        }
        #endregion

        #region Remove
        /// <summary>
        /// Attempts to remove a value from the SkipList.
        /// </summary>
        /// <param name="value">The value to remove from the SkipList.</param>
        /// <returns>True if the value is found and removed; false if the value is not found
        /// in the SkipList.</returns>
        public bool Remove(T value)
        {
            SkipListNode<T>[] updates = BuildUpdateTable(value);
            SkipListNode<T> current = updates[0][0];

            if (current != null && comparer.Compare(current.Value, value) == 0)
            {
                _count--;

                // We found the data to delete
                for (int i = 0; i < _head.Height; i++)
                {
                    if (updates[i][i] != current)
                        break;
                    else
                        updates[i][i] = current[i];
                }

                // finally, see if we need to trim the height of the list
                if (_head[_head.Height - 1] == null)
                    // we removed the single, tallest item... reduce the list height
                    _head.DecrementHeight();

                current = null;

                return true;        // the item was successfully removed
            }
            else
                // the data to delete wasn't found.
                return false;
        }
        #endregion

        #region BuildUpdateTable
        /// <summary>
        /// Creates a table of the SkipListNode instances that will need to be updated when an item is
        /// added or removed from the SkipList.
        /// </summary>
        /// <param name="value">The value to be added or removed.</param>
        /// <returns>An array of SkipListNode instances, as many as the height of the head node.
        /// A SkipListNode instance in array index k represents the SkipListNode at height k that must
        /// be updated following the addition/deletion.</returns>
        protected SkipListNode<T>[] BuildUpdateTable(T value)
        {
            SkipListNode<T>[] updates = new SkipListNode<T>[_head.Height];
            SkipListNode<T> current = _head;

            // determine the nodes that need to be updated at each level
            for (int i = _head.Height - 1; i >= 0; i--)
            {
                if (!(current[i] != null && comparer.Compare(current[i].Value, value) < 0))
                    _comparisons++;

                while (current[i] != null && comparer.Compare(current[i].Value, value) < 0)
                {
                    current = current[i];
                    _comparisons++;
                }

                updates[i] = current;
            }

            return updates;
        }
        #endregion

        #region CopyTo
        /// <summary>
        /// Copies the contents of the SkipList to the passed-in array.
        /// </summary>
        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }

        /// <summary>
        /// Copies the contents of the SkipList to the passed-in array.
        /// </summary>
        public void CopyTo(T[] array, int index)
        {
            // copy the values from the skip list to array
            if (array == null)
                throw new ArgumentNullException("array is null");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index is less than 0");

            if (index >= array.Length)
                throw new ArithmeticException("index is greater than the length of array");

            if (array.Length - index <= _count)
                throw new ArgumentException("insufficient space in array to store skip list starting at specified index");

            SkipListNode<T> current = _head[0];
            int i = 0;
            while (current != null)
            {
                array[i + index] = current.Value;
                i++;
            }
        }
        #endregion
        #endregion

        #region GetEnumerator
        /// <summary>
        /// Returns an enumerator to access the contents of the SkipList.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            // enumerate through the skip list one element at a time
            SkipListNode<T> current = _head[0];
            while (current != null)
            {
                yield return current.Value;
                current = current[0];
            }
        }
        #endregion

        #region ToString
        /// <summary>
        /// This overridden form of ToString() is simply for displaying detailed information
        /// about the contents of the SkipList, used by SkipListTester - feel free to remove it.
        /// </summary>
        public override string ToString()
        {
            SkipListNode<T> current = _head[0];
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            while (current != null)
            {
                sb.Append(current.Value.ToString());
                sb.Append(" [ H=").Append(current.Height);
                for (int i = current.Height - 1; i >= 0; i--)
                {
                    sb.Append(" | ");
                    if (current[i] == null)
                        sb.Append("NULL");
                    else
                        sb.Append(current[i].Value.ToString());
                }
                sb.Append(" ] ; ");

                current = current[0];
            }

            return sb.ToString();
        }
        #endregion

        #region ResetComparisons
        /// <summary>
        /// Resets the internal comparison counter back to zero.  Used for performance testing (can be removed).
        /// </summary>
        public void ResetComparisons()
        {
            _comparisons = 0;
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            // enumerate through the skip list one element at a time
            SkipListNode<T> current = _head[0];
            while (current != null)
            {
                yield return current.Value;
                current = current[0];
            }
        }
        #endregion
        #endregion

        #region Properties
        /// <summary>
        /// Gets the height of the tallest SkipListNode in the SkipList.
        /// </summary>
        public int Height
        {
            get { return _head.Height; }
        }

        /// <summary>
        /// Gets the number of total comparisons made - used for perf. testing.
        /// </summary>
        /// <value></value>
        public long Comparisons
        {
            get { return _comparisons; }
        }

        #region ICollection Properties
        /// <summary>
        /// Gets the number of elements in the SkipList
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Gets is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
        #endregion
        #endregion
    }
}
