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
    /// Represents a collection of SkipListNodes.  This class differs from the base class - NodeList -
    /// in that it contains an internal method to increment or decrement the height of the SkipListNodeList. 
    /// Incrementing the height adds a new neighbor to the list, decrementing the height removes the
    /// top-most neighbor.
    /// </summary>
    /// <typeparam name="T">The type of data stored in the SkipListNode instances that are contained
    /// within this SkipListNodeList.</typeparam>
    public class SkipListNodeList<T> : NodeList<T>
    {
        #region Constructors
        /// <summary>
        /// Represents a collection of SkipListNodes.
        /// </summary>
        /// <param name="height">The hieght.</param>
        public SkipListNodeList(int height) : base(height) { }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Increases the size of the SkipListNodeList by one, adding a default SkipListNode.
        /// </summary>
        internal void IncrementHeight()
        {
            // add a dummy entry
            base.Add(default(Node<T>));
        }

        /// <summary>
        /// Decreases the size of the SkipListNodeList by one, removing the "top-most" SkipListNode.
        /// </summary>
        internal void DecrementHeight()
        {
            // delete the last entry
            base.RemoveAt(base.Count - 1);
        }
        #endregion
    }
}
