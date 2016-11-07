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
    /// Implements a minimum binary heap sort.
    /// </summary>
    /// <typeparam name="T">The type in the collection.</typeparam>
    public class MinimumBinaryHeap<T> where T : IComparable<T>
    {
        /// <summary>
        /// Implements a minimum binary heap sort.
        /// </summary>
        public MinimumBinaryHeap()
        {
            _items = new List<T>();
        }

        /// <summary>
        /// Implements a minimum binary heap sort.
        /// </summary>
        /// <param name="heapToCopy">An existing heap to coppy.</param>
        public MinimumBinaryHeap(MinimumBinaryHeap<T> heapToCopy)
        {
            _items = new List<T>(heapToCopy.Items);
        }

        private readonly List<T> _items;

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count { get { return _items.Count; } }

        /// <summary>
        /// Gets the item list
        /// </summary>
        public List<T> Items { get { return _items; } }

        /// <summary>
        /// Clear all the items.
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Add an item to the collection.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void Add(T value)
        {
            // Add the item to the list, making sure to keep track of where it was added.
            _items.Add(value);
            int pos = _items.Count - 1;

            // If the new item is the only item, we're done.
            if (pos == 0) return;

            // Otherwise, perform log(n) operations, walking up the tree, swapping
            // where necessary based on key values
            while (pos > 0)
            {
                // Get the next position to check
                int nextPos = pos / 2;

                // Extract the entry at the next position
                var toCheck = _items[nextPos];

                // Compare that entry to our new one.  If our entry has a smaller key, move it up.
                // Otherwise, we're done.
                if (value.CompareTo(toCheck) < 0)
                {
                    _items[pos] = toCheck;
                    pos = nextPos;
                }
                else break;
            }

            // Make sure we put this entry back in, just in case
            _items[pos] = value;
        }

        /// <summary>
        /// Remove an item from the collection.
        /// </summary>
        /// <param name="value">The value to remove.</param>
        public void Remove(T value)
        {
            // Get the index of the item to 
            int index = _items.IndexOf(value);

            // Remove the first item if there will only be 0 or 1 items left after doing so.  
            if (_items.Count <= 2) _items.RemoveAt(index);
            // A reheapify will be required for the removal
            else
            {
                // Remove the item and move the last item to the index.
                _items[index] = _items[_items.Count - 1];
                _items.RemoveAt(_items.Count - 1);

                // Start reheapify
                int current = index, possibleSwap = 0;

                // Keep going until the tree is a heap
                while (true)
                {
                    // Get the positions of the node's children
                    int leftChildPos = 2 * current + 1;
                    int rightChildPos = leftChildPos + 1;

                    // Should we swap with the left child?
                    if (leftChildPos < _items.Count)
                    {
                        // Get the two entries to compare (node and its left child)
                        var entry1 = _items[current];
                        var entry2 = _items[leftChildPos];

                        // If the child has a lower key than the parent, set that as a possible swap
                        if (entry2.CompareTo(entry1) < 0) possibleSwap = leftChildPos;
                    }
                    else break; // if can't swap this, we're done

                    // Should we swap with the right child?  Note that now we check with the possible swap
                    // position (which might be current and might be left child).
                    if (rightChildPos < _items.Count)
                    {
                        // Get the two entries to compare (node and its left child)
                        var entry1 = _items[possibleSwap];
                        var entry2 = _items[rightChildPos];

                        // If the child has a lower key than the parent, set that as a possible swap
                        if (entry2.CompareTo(entry1) < 0) possibleSwap = rightChildPos;
                    }

                    // Now swap current and possible swap if necessary
                    if (current != possibleSwap)
                    {
                        var temp = _items[current];
                        _items[current] = _items[possibleSwap];
                        _items[possibleSwap] = temp;
                    }
                    else break; // if nothing to swap, we're done

                    // Update current to the location of the swap
                    current = possibleSwap;
                }
            }
        }
    }
}
