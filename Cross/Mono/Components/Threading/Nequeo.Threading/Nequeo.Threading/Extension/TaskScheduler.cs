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
using System.Threading;
using System.Threading.Tasks;

namespace Nequeo.Threading.Extension
{
    /// <summary>Extension methods for TaskScheduler.</summary>
    public static class TaskSchedulerExtensions
    {
        /// <summary>Gets a SynchronizationContext that targets this TaskScheduler.</summary>
        /// <param name="scheduler">The target scheduler.</param>
        /// <returns>A SynchronizationContext that targets this scheduler.</returns>
        public static SynchronizationContext ToSynchronizationContext(this TaskScheduler scheduler)
        {
            return new TaskSchedulerSynchronizationContext(scheduler);
        }

        /// <summary>Provides a SynchronizationContext wrapper for a TaskScheduler.</summary>
        private sealed class TaskSchedulerSynchronizationContext : SynchronizationContext
        {
            /// <summary>The scheduler.</summary>
            private TaskScheduler _scheduler;

            /// <summary>Initializes the context with the specified scheduler.</summary>
            /// <param name="scheduler">The scheduler to target.</param>
            internal TaskSchedulerSynchronizationContext(TaskScheduler scheduler)
            {
                if (scheduler == null) throw new ArgumentNullException("scheduler");
                _scheduler = scheduler;
            }

            /// <summary>Dispatches an asynchronous message to the synchronization context.</summary>
            /// <param name="d">The System.Threading.SendOrPostCallback delegate to call.</param>
            /// <param name="state">The object passed to the delegate.</param>
            public override void Post(SendOrPostCallback d, object state)
            {
                Task.Factory.StartNew(() => d(state), CancellationToken.None, TaskCreationOptions.None, _scheduler);
            }

            /// <summary>Dispatches a synchronous message to the synchronization context.</summary>
            /// <param name="d">The System.Threading.SendOrPostCallback delegate to call.</param>
            /// <param name="state">The object passed to the delegate.</param>
            public override void Send(SendOrPostCallback d, object state)
            {
                Task t = new Task(() => d(state));
                t.RunSynchronously(_scheduler);
                t.Wait();
            }
        }
    }
}