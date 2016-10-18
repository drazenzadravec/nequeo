/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nequeo.Threading.Tasks
{
    /// <summary>
    /// Provides basic implementation of the IAsyncResult interface.
    /// </summary>
    public class AsyncResultQueue : IAsyncResult
    {
        // The owner of this AsyncResult object.
        private object owner;

        // The callback to be invoked when the operation completes.
        private AsyncCallback callback;

        // User state information.
        private object state;

        // For signaling when the operation has completed.
        private ManualResetEvent waitHandle = new ManualResetEvent(false);

        // A value indicating whether the operation completed synchronously.
        private bool completedSynchronously;

        // A value indicating whether the operation has completed.
        private bool isCompleted = false;

        // The ID of the thread this AsyncResult object originated on.
        private int threadId;

        /// <summary>
        /// Initializes a new instance of the AsyncResult object with the
        /// specified owner of the AsyncResult object, the optional callback
        /// delegate, and optional state object.
        /// </summary>
        /// <param name="owner">
        /// The owner of the AsyncResult object.
        /// </param>
        /// <param name="callback">
        /// An optional asynchronous callback, to be called when the 
        /// operation is complete. 
        /// </param>
        /// <param name="state">
        /// A user-provided object that distinguishes this particular 
        /// asynchronous request from other requests. 
        /// </param>
        public AsyncResultQueue(object owner, AsyncCallback callback, object state)
        {
            this.owner = owner;
            this.callback = callback;
            this.state = state;

            // Get the current thread ID. This will be used later to determine
            // if the operation completed synchronously.
            threadId = Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        /// Signals that the operation has completed.
        /// </summary>
        public void Signal()
        {
            isCompleted = true;
            completedSynchronously = threadId == Thread.CurrentThread.ManagedThreadId;
            waitHandle.Set();

            if (callback != null)
            {
                callback(this);
            }
        }

        /// <summary>
        /// Gets the owner of this AsyncResult object.
        /// </summary>
        public object Owner
        {
            get
            {
                return owner;
            }
        }

        /// <summary>
        /// Gets the async state.
        /// </summary>
        public object AsyncState
        {
            get
            {
                return state;
            }
        }

        /// <summary>
        /// Gets the async wait handle.
        /// </summary>
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                return waitHandle;
            }
        }

        /// <summary>
        /// Get the completed synchronously indicator.
        /// </summary>
        public bool CompletedSynchronously
        {
            get
            {
                return completedSynchronously;
            }
        }

        /// <summary>
        /// Get the is operation complete indicator.
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                return isCompleted;
            }
        }
    }
}
