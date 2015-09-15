/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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
using System.Timers;
using System.Threading;

namespace Nequeo.Maintenance.Timing
{
    /// <summary>
    /// Timing interval controller.
    /// </summary>
	public class IntervalControl : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="interval">The specified timer interval for the timer.</param>
        public IntervalControl(double interval)
        {
            if (interval < (double)0.0) throw new ArgumentOutOfRangeException("interval");

            _interval = interval;
        }
        #endregion

        #region Fields

        // 1 * 24 * 60 * 60 * 1000 (1 day) default.
        private double _interval = (double)86400000.0;
        private System.Timers.Timer _timer;

        private Action<object> _actionHandler = null;
        private Object _state = null;

        // This is the synchronization point that prevents events
        // from running concurrently, and prevents the main thread 
        // from executing code after the Stop method until any 
        // event handlers are done executing.
        private int _syncPoint = 0;

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the time interval.
        /// </summary>
        public double Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start the timer.
        /// </summary>
        /// <param name="actionHandler">The interval action.</param>
        /// <param name="state">The state object.</param>
        public void Start(Action<object> actionHandler, Object state = null)
        {
            if (actionHandler == null) throw new ArgumentNullException("actionHandler");

            _state = state;
            _actionHandler = actionHandler;

            // Set _syncPoint to zero before 
            // starting the timer. 
            _syncPoint = 0;

            _timer = new System.Timers.Timer(_interval);
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            // Only raise the event the first time Interval elapses set AutoReset = False
            // Start the timer, start collecting file logging information.
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        /// <summary>
        /// Stop the timer.
        /// </summary>
        public void Stop()
        {
            try
            {
                // Stop the server timer.
                if (_timer != null)
                    _timer.Enabled = false;
            }
            catch { }
            finally
            {
                // Dispose of the timer object.
                if (_timer != null)
                    _timer.Dispose();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// This event occures each time the time interval has elapsed.
        /// The control process is initialed.
        /// </summary>
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            // Start the control process.
            this.InitialiseControl();
        }

        /// <summary>
        /// Initialises the control process.
        /// </summary>
        private void InitialiseControl()
        {
            // The Interlocked.CompareExchange(Int32,Int32,Int32) method overload 
            // is used to avoid reentrancy and to prevent the control thread from 
            // continuing until an executing event ends. The event handler uses the 
            // CompareExchange(Int32,Int32,Int32) method to set a control variable 
            // to 1, but only if the value is currently zero. This is an atomic 
            // operation. If the return value is zero, the control variable has been 
            // set to 1 and the event handler proceeds. If the return value is non-zero, 
            // the event is simply discarded to avoid reentrancy
            int sync = Interlocked.CompareExchange(ref _syncPoint, 1, 0);
            
            // If the sync is zero then no other event handler
            // has started, so execute the code, else exit the
            // event handler.
            if (sync == 0)
            {
                // Do not allow any waiting timer threads
                // to immediatly execute after this thread,
                // otherwise the action is executed to early.
                _timer.Enabled = false;

                if (_actionHandler != null)
                    _actionHandler(_state);

                // Allow any new threads to
                // execute after the interval.
                _timer.Enabled = true;

                // Release control of _syncPoint.
                _syncPoint = 0;
            }
        }
        #endregion

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
                    if(_timer != null)
                        _timer.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _timer = null;

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
        ~IntervalControl()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// Timing interval controller.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    public class IntervalControl<T> : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="interval">The specified timer interval for the timer.</param>
        public IntervalControl(double interval)
        {
            if (interval < (double)0.0) throw new ArgumentOutOfRangeException("interval");

            _interval = interval;
        }
        #endregion

        #region Fields

        // 1 * 24 * 60 * 60 * 1000 (1 day) default.
        private double _interval = (double)86400000.0;
        private System.Timers.Timer _timer;

        private Action<T> _returnAction = null;
        private Func<object, T> _actionHandler = null;
        private Object _state = null;

        // This is the synchronization point that prevents events
        // from running concurrently, and prevents the main thread 
        // from executing code after the Stop method until any 
        // event handlers are done executing.
        private int _syncPoint = 0;

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the time interval.
        /// </summary>
        public double Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start the timer.
        /// </summary>
        /// <param name="actionHandler">The interval action.</param>
        /// <param name="state">The state object.</param>
        /// <param name="returnAction">The return action.</param>
        public void Start(Func<object, T> actionHandler, Object state = null, Action<T> returnAction = null)
        {
            if (actionHandler == null) throw new ArgumentNullException("actionHandler");

            _state = state;
            _actionHandler = actionHandler;
            _returnAction = returnAction;

            // Set _syncPoint to zero before 
            // starting the timer. 
            _syncPoint = 0;

            _timer = new System.Timers.Timer(_interval);
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            // Only raise the event the first time Interval elapses set AutoReset = False
            // Start the timer, start collecting file logging information.
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        /// <summary>
        /// Stop the timer.
        /// </summary>
        public void Stop()
        {
            try
            {
                // Stop the server timer.
                if (_timer != null)
                    _timer.Enabled = false;
            }
            catch { }
            finally
            {
                // Dispose of the timer object.
                if (_timer != null)
                    _timer.Dispose();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// This event occures each time the time interval has elapsed.
        /// The control process is initialed.
        /// </summary>
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            // Start the control process.
            this.InitialiseControl();
        }

        /// <summary>
        /// Initialises the control process.
        /// </summary>
        private void InitialiseControl()
        {
            // The Interlocked.CompareExchange(Int32,Int32,Int32) method overload 
            // is used to avoid reentrancy and to prevent the control thread from 
            // continuing until an executing event ends. The event handler uses the 
            // CompareExchange(Int32,Int32,Int32) method to set a control variable 
            // to 1, but only if the value is currently zero. This is an atomic 
            // operation. If the return value is zero, the control variable has been 
            // set to 1 and the event handler proceeds. If the return value is non-zero, 
            // the event is simply discarded to avoid reentrancy
            int sync = Interlocked.CompareExchange(ref _syncPoint, 1, 0);

            // If the sync is zero then no other event handler
            // has started, so execute the code, else exit the
            // event handler.
            if (sync == 0)
            {
                // Do not allow any waiting timer threads
                // to immediatly execute after this thread,
                // otherwise the action is executed to early.
                _timer.Enabled = false;

                if (_actionHandler != null)
                {
                    // If return action is null.
                    if (_returnAction == null)
                    {
                        _actionHandler(_state);
                    }
                    else
                    {
                        // Execute the return action.
                        T ret = _actionHandler(_state);
                        _returnAction(ret);
                    }
                }

                // Allow any new threads to
                // execute after the interval.
                _timer.Enabled = true;

                // Release control of _syncPoint.
                _syncPoint = 0;
            }
        }
        #endregion

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
                    if (_timer != null)
                        _timer.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _timer = null;

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
        ~IntervalControl()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
