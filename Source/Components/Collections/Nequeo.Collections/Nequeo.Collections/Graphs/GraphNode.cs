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

namespace Nequeo.Collections.Graphs
{
    /// <summary>
    /// Represents a node in a graph.  A graph node contains some piece of data, along with a set of
    /// neighbors.  There can be an optional cost between a graph node and each of its neighbors.
    /// </summary>
    /// <typeparam name="T">The type of data stored in the graph node.</typeparam>
    public class GraphNode<T> : Node<T>
    {
        #region Private Member Variables
        private List<int> costs;        // the cost associated with each edge
        #endregion

        #region Constructors
        /// <summary>
        /// Represents a node in a graph.
        /// </summary>
        public GraphNode() : base() { }

        /// <summary>
        /// Represents a node in a graph.
        /// </summary>
        /// <param name="value">The data.</param>
        public GraphNode(T value) : base(value) { }

        /// <summary>
        /// Represents a node in a graph.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="neighbors">The value neighbors.</param>
        public GraphNode(T value, NodeList<T> neighbors) : base(value, neighbors) { }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the set of neighbors for this graph node.
        /// </summary>
        new public NodeList<T> Neighbors
        {
            get
            {
                if (base.Neighbors == null)
                    base.Neighbors = new NodeList<T>();

                return base.Neighbors;
            }            
        }

        /// <summary>
        /// Gets the set of costs for the edges eminating from this graph node.
        /// The k<sup>th</sup> cost (Cost[k]) represents the cost from the graph node to the node
        /// represented by its k<sup>th</sup> neighbor (Neighbors[k]).
        /// </summary>
        /// <value></value>
        public List<int> Costs
        {
            get
            {
                if (costs == null)
                    costs = new List<int>();

                return costs;
            }
        }
        #endregion
    }
}
