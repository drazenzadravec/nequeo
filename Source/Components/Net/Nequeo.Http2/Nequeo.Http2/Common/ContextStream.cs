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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Threading.Tasks;
using System.Security.Principal;

using Nequeo.Net;
using Nequeo.Extension;
using Nequeo.Model;
using Nequeo.Model.Message;
using Nequeo.Net.Http2.Protocol;

namespace Nequeo.Net.Http2
{
    /// <summary>
    /// Http context stream.
    /// </summary>
    internal class ContextStream : HttpStream, IDisposable
    {
        /// <summary>
        /// Http context stream.
        /// </summary>
        /// <param name="streamId">The context stream id.</param>
        /// <param name="httpContext">The http context that controls this context stream.</param>
        public ContextStream(int streamId, HttpContext httpContext)
            : base(streamId, httpContext)
        { 
        }

        private StreamState _state = StreamState.Idle;
        private readonly FlowControlManager _flowCrtlManager = null;

        /// <summary>
        /// Gets the flow control manager.
        /// </summary>
        internal FlowControlManager FlowControlManager
        {
            get { return _flowCrtlManager; }
        }

        /// <summary>
        /// Gets or sets the window size.
        /// </summary>
        public int WindowSize { get; set; }

        /// <summary>
        /// Gets or sets the frames sent.
        /// </summary>
        public int FramesSent { get; set; }

        /// <summary>
        /// Gets or sets the frames received.
        /// </summary>
        public int FramesReceived { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets an indicator specifying that the reset stream frame was sent.
        /// </summary>
        public bool WasResetSent { get; set; }

        /// <summary>
        /// Gets or sets an indicator specifying if the stream is opened.
        /// </summary>
        public bool Opened
        {
            get { return _state == StreamState.Opened; }
            set { Contract.Assert(value); _state = StreamState.Opened; }
        }

        /// <summary>
        /// Gets or sets an indicator specifying if the stream is idle.
        /// </summary>
        public bool Idle
        {
            get { return _state == StreamState.Idle; }
            set { Contract.Assert(value); _state = StreamState.Idle; }
        }

        /// <summary>
        /// Gets or sets an indicator specifying if the stream is half closed remote.
        /// </summary>
        public bool HalfClosedRemote
        {
            get { return _state == StreamState.HalfClosedRemote; }
            set
            {
                Contract.Assert(value);
                _state = HalfClosedLocal ? StreamState.Closed : StreamState.HalfClosedRemote;

                if (Closed)
                {
                    Utility.ProcessClose(HttpContext, ErrorCodeRegistry.No_Error, this);
                    Closed = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets an indicator specifying if the stream is half closed local.
        /// </summary>
        public bool HalfClosedLocal
        {
            get { return _state == StreamState.HalfClosedLocal; }
            set
            {
                Contract.Assert(value);
                _state = HalfClosedRemote ? StreamState.Closed : StreamState.HalfClosedLocal;

                if (Closed)
                {
                    Utility.ProcessClose(HttpContext, ErrorCodeRegistry.No_Error, this);
                    Closed = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets an indicator specifying if the stream is reserved local.
        /// </summary>
        public bool ReservedLocal
        {
            get { return _state == StreamState.ReservedLocal; }
            set
            {
                Contract.Assert(value);
                _state = ReservedRemote ? StreamState.Closed : StreamState.ReservedLocal;

                if (Closed)
                {
                    Utility.ProcessClose(HttpContext, ErrorCodeRegistry.No_Error, this);
                    Closed = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets an indicator specifying if the stream is reserved remote.
        /// </summary>
        public bool ReservedRemote
        {
            get { return _state == StreamState.ReservedRemote; }
            set
            {
                Contract.Assert(value);
                _state = HalfClosedLocal ? StreamState.Closed : StreamState.ReservedRemote;

                if (Closed)
                {
                    Utility.ProcessClose(HttpContext, ErrorCodeRegistry.No_Error, this);
                    Closed = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets an indicator specifying if the stream is closed.
        /// </summary>
        public bool Closed
        {
            get { return _state == StreamState.Closed; }
            set { Contract.Assert(value); _state = StreamState.Closed; }
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

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
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (HttpRequest != null)
                        HttpRequest.Dispose();

                    if (HttpResponse != null)
                        HttpResponse.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                HttpRequest = null;
                HttpResponse = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~ContextStream()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
