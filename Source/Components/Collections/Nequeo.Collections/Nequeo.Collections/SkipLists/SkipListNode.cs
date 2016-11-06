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
using System.Text;

using Nequeo.Collections.Common;

namespace Nequeo.Collections.SkipLists
{
    /// <summary>
    /// Represents a node in a SkipList.  A SkipListNode has a Height and a set of neighboring
    /// SkipListNodes (precisely as many neighbor references as its Height, although some neighbor 
    /// references may be null).  Also, a SkipListNode contains some piece of data associated with it.
    /// </summary>
    /// <typeparam name="T">The type of the data stored in the SkipListNode.</typeparam>
    public class SkipListNode<T> : Node<T>
    {
        #region Constructors
        /// <summary>
        /// Represents a node in a SkipList.
        /// </summary>
        private SkipListNode() {}   // no default constructor available, must supply height

        /// <summary>
        /// Represents a node in a SkipList.
        /// </summary>
        /// <param name="height">The height.</param>
        public SkipListNode(int height)
        {
            base.Neighbors = new SkipListNodeList<T>(height);
        }

        /// <summary>
        /// Represents a node in a SkipList.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="height">The height.</param>
        public SkipListNode(T value, int height) : base(value)
        {
            base.Neighbors = new SkipListNodeList<T>(height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Increases the height of the SkipListNode by 1.
        /// </summary>
        internal void IncrementHeight()
        {
            // Increase height by 1
            ((SkipListNodeList<T>) base.Neighbors).IncrementHeight();
        }

        /// <summary>
        /// Decreases the height of the SkipListNode by 1.
        /// </summary>
        internal void DecrementHeight()
        {
            // Decrease height by 1
            ((SkipListNodeList<T>) base.Neighbors).DecrementHeight();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the height of the SkipListNode
        /// </summary>
        public int Height
        {
            get { return base.Neighbors.Count; }
        }

        /// <summary>
        /// Provides ordinally-indexed access to the neighbors of the SkipListNode.
        /// </summary>
        public SkipListNode<T> this[int index]
        {
            get { return (SkipListNode<T>) base.Neighbors[index]; }
            set { base.Neighbors[index] = value; }
        }
        #endregion
    }
}
