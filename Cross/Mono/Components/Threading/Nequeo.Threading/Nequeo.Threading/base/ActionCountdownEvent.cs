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

namespace Nequeo.Threading
{
    /// <summary>Runs an action when the CountdownEvent reaches zero.</summary>
    public class ActionCountdownEvent : IDisposable
    {
        private readonly CountdownEvent _event;
        private readonly Action _action;
        private readonly ExecutionContext _context;

        /// <summary>Initializes the ActionCountdownEvent.</summary>
        /// <param name="initialCount">The number of signals required to set the CountdownEvent.</param>
        /// <param name="action">The delegate to be invoked when the count reaches zero.</param>
        public ActionCountdownEvent(int initialCount, Action action)
        {
            // Validate arguments
            if (initialCount < 0) throw new ArgumentOutOfRangeException("initialCount");
            if (action == null) throw new ArgumentNullException("action");

            // Store the action and create the event from the initial count. If the initial count forces the
            // event to be set, run the action immediately. Otherwise, capture the current execution context
            // so we can run the action in the right context later on.
            _action = action;
            _event = new CountdownEvent(initialCount);
            if (initialCount == 0) action();
            else _context = ExecutionContext.Capture();
        }

        /// <summary>Increments the current count by one.</summary>
        public void AddCount() 
        {
            // Just delegate to the underlying event
            _event.AddCount(); 
        }

        /// <summary>Registers a signal with the event, decrementing its count.</summary>
        public void Signal()
        {
            // If signaling the event causes it to become set
            if (_event.Signal())
            {
                // Execute the action.  If we were able to capture a context
                // at instantiation time, use that context to execute the action.
                // Otherwise, just run the action.
                if (_context != null)
                {
                    ExecutionContext.Run(_context, _ => _action(), null);
                }
                else _action();
            }
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if(_event != null)
                        _event.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.

                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~ActionCountdownEvent()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}