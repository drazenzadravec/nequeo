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
using System.Collections.Generic;
using System.Diagnostics;

namespace Nequeo.Threading.Tasks
{
    /// <summary>Represents a queue of tasks to be started and executed serially.</summary>
    public class SerialTaskQueue
    {
        /// <summary>The ordered queue of tasks to be executed. Also serves as a lock protecting all shared state.</summary>
        private Queue<object> _tasks = new Queue<object>();
        /// <summary>The task currently executing, or null if there is none.</summary>
        private Task _taskInFlight;

        /// <summary>Enqueues the task to be processed serially and in order.</summary>
        /// <param name="taskGenerator">The function that generates a non-started task.</param>
        public void Enqueue(Func<Task> taskGenerator) { EnqueueInternal(taskGenerator); }

        /// <summary>Enqueues the non-started task to be processed serially and in order.</summary>
        /// <param name="task">The task.</param>
        public Task Enqueue(Task task) { EnqueueInternal(task); return task; }

        /// <summary>Gets a Task that represents the completion of all previously queued tasks.</summary>
        public Task Completed() { return Enqueue(new Task(() => { })); }

        /// <summary>Enqueues the task to be processed serially and in order.</summary>
        /// <param name="taskOrFunction">The task or functino that generates a task.</param>
        /// <remarks>The task must not be started and must only be started by this instance.</remarks>
        private void EnqueueInternal(object taskOrFunction)
        {
            // Validate the task
            if (taskOrFunction == null) throw new ArgumentNullException("task");
            lock(_tasks)
            {
                // If there is currently no task in flight, we'll start this one
                if (_taskInFlight == null) StartTask_CallUnderLock(taskOrFunction);
                    // Otherwise, just queue the task to be started later
                else _tasks.Enqueue(taskOrFunction);
            }
        }

        /// <summary>Called when a Task completes to potentially start the next in the queue.</summary>
        /// <param name="ignored">The task that completed.</param>
        private void OnTaskCompletion(Task ignored)
        {
            lock (_tasks)
            {
                // The task completed, so nothing is currently in flight.
                // If there are any tasks in the queue, start the next one.
                _taskInFlight = null;
                if (_tasks.Count > 0) StartTask_CallUnderLock(_tasks.Dequeue());
            }
        }

        /// <summary>Starts the provided task (or function that returns a task).</summary>
        /// <param name="nextItem">The next task or function that returns a task.</param>
        private void StartTask_CallUnderLock(object nextItem)
        {
            Task next = nextItem as Task;
            if (next == null) next = ((Func<Task>)nextItem)();

            if (next.Status == TaskStatus.Created) next.Start();
            _taskInFlight = next;
            next.ContinueWith(OnTaskCompletion);
        }
    }
}