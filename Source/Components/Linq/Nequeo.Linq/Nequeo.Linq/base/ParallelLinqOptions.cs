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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nequeo.Linq
{
    /// <summary>Provides a grouping for common Parallel LINQ options.</summary>
    public sealed class ParallelLinqOptions : ParallelOptions
    {
        private ParallelExecutionMode _executionMode = ParallelExecutionMode.Default;
        private ParallelMergeOptions _mergeOptions = ParallelMergeOptions.Default;
        private bool _ordered = false;

        /// <summary>Gets or sets the execution mode.</summary>
        public ParallelExecutionMode ExecutionMode
        {
            get { return _executionMode; }
            set
            {
                if (value != ParallelExecutionMode.Default &&
                    value != ParallelExecutionMode.ForceParallelism) throw new ArgumentOutOfRangeException("ExecutionMode");
                _executionMode = value;
            }
        }

        /// <summary>Gets or sets the merge options.</summary>
        public ParallelMergeOptions MergeOptions
        {
            get { return _mergeOptions; }
            set
            {
                if (value != ParallelMergeOptions.AutoBuffered &&
                    value != ParallelMergeOptions.Default &&
                    value != ParallelMergeOptions.FullyBuffered &&
                    value != ParallelMergeOptions.NotBuffered) throw new ArgumentOutOfRangeException("MergeOptions");
                _mergeOptions = value;
            }
        }

        /// <summary>Gets or sets whether the query should retain ordering.</summary>
        public bool Ordered
        {
            get { return _ordered; }
            set { _ordered = value; }
        }
    }
}
