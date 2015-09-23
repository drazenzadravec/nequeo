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
using System.Runtime.CompilerServices;
using System.Threading;

namespace Nequeo.Wpf.Threading
{
    /// <summary>
    /// Work dispatcher object.
    /// </summary>
    public abstract class WorkDispatcherObject
    {
        /// <summary>
        /// Work dispatcher object.
        /// </summary>
        protected WorkDispatcherObject()
        {
            Dispatcher = new WorkDispatcher();
            Dispatcher.ShutdownFinished += Dispatcher_ShutdownFinished;
        }

        private bool _restartThread;

        /// <summary>
        /// Gets the current dispatcher.
        /// </summary>
        public WorkDispatcher Dispatcher
        {
            get;
            private set;
        }

        /// <summary>
        /// Check if the current thread is the same as the dispatcher thread.
        /// </summary>
        /// <returns>True if the same; else false.</returns>
        public bool CheckAccess()
        {
            lock(Dispatcher)
            {
                if (Dispatcher.DispatcherThread == Thread.CurrentThread)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Verfiy if the same thread. Throws exception if not the same thread.
        /// </summary>
        public void VerifyAccess()
        {
            lock (Dispatcher)
            {
                if (Dispatcher.DispatcherThread != Thread.CurrentThread)
                    throw new InvalidOperationException("Not the same thread");
            }
        }

        /// <summary>
        /// Shutdown the thread if dispatcher is shutting down, else start a new dispatcher thread from the aparment state.
        /// </summary>
        /// <param name="apartmentState">The spartment state.</param>
        public void EnsureThread(ApartmentState apartmentState)
        {
            lock(Dispatcher)
            {
                if (Dispatcher.ShuttingDown)
                {
                    _restartThread = true;
                    return;
                }

                if (Dispatcher.DispatcherThread == null)
                {
                    StartNewDispatcherThread(apartmentState);
                }
            }
        }

        /// <summary>
        /// Start a new dispatcher thread.
        /// </summary>
        /// <param name="apartmentState">The spartment state.</param>
        private void StartNewDispatcherThread(ApartmentState apartmentState)
        {
            var reset = new ManualResetEvent(false);

            var t = new Thread((ThreadStart)delegate
            {
                Thread.CurrentThread.Name = string.Format("WorkDispatcherThread");

                Dispatcher.Run(reset);
            })
            {
                IsBackground = true
            };

            t.SetApartmentState(apartmentState);

            t.Priority = ThreadPriority.Normal;

            /* Starts the thread and creates the object */
            t.Start();
           
            /* We wait until our dispatcher is initialized and
             * the new Dispatcher is running */
            reset.WaitOne();
        }

        /// <summary>
        /// Shutdown worker thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dispatcher_ShutdownFinished(object sender, ShutdownFinishedEventArgs e)
        {
            lock(Dispatcher)
            {
                /* If our restart thread flag is set,
                 * then we want to cancel our dispatcher's
                 * shutdown and have it continue it's
                 * processing of messages and delegates */
                e.CancelShutdown = _restartThread;
                _restartThread = false;
            }
        }
    }
}