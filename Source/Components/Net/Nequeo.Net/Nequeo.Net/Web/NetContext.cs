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
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;

using Nequeo.Handler;
using Nequeo.Extension;
using Nequeo.Net.Configuration;

namespace Nequeo.Net
{
    /// <summary>
    /// Web client context.
    /// </summary>
    public class NetContext
    {
        /// <summary>
        /// Web client context.
        /// </summary>
        public NetContext() { }

        /// <summary>
        /// Inject the current context provider.
        /// </summary>
        /// <param name="context">The provider context.</param>
        public NetContext(Nequeo.Net.Provider.ClientContext context)
        {
            _context = context;
        }

        private string _connectionID = null;
        private bool _isAsyncMode = false;
        private SocketState _socketState = SocketState.None;
        private Nequeo.Net.Provider.ClientContext _context = null;

        /// <summary>
        /// Gets or sets, true if in async mode, then the context is only generated once.
        /// All client processing is done in async and within the current context.
        /// </summary>
        public bool IsAsyncMode
        {
            get
            {
                if (_context != null)
                    _isAsyncMode = _context.IsAsyncMode;

                return _isAsyncMode;
            }
            set 
            {
                _isAsyncMode = value;
                if (_context != null)
                    _context.IsAsyncMode = _isAsyncMode;
            }
        }

        /// <summary>
        /// Gets the web request.
        /// </summary>
        public NetRequest NetRequest
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the web response.
        /// </summary>
        public NetResponse NetResponse
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a System.Boolean value that indicates whether the TCP connection used
        /// to send the request is using the Secure Sockets Layer (SSL) protocol.
        /// </summary>
        public bool IsSecureConnection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets, has a secure negotiation and server authentication 
        /// been established with the client. Also is the data encrypted.
        /// </summary>
        public bool IsSslAuthenticated
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the communication port number.
        /// </summary>
        public int Port
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the common service name.
        /// </summary>
        public string ServiceName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the unique identifier for this connection.
        /// </summary>
        public string UniqueIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets, true if the context has been authenticated; else false.
        /// </summary>
        public bool IsAuthenticated
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current web socket state.
        /// </summary>
        public SocketState SocketState
        {
            get
            {
                if (_context != null)
                    _socketState = _context.SocketState;

                return _socketState;
            }
            set { _socketState = value; }
        }

        /// <summary>
        /// Gets or sets the current unique connection identifier.
        /// </summary>
        public string ConnectionID
        {
            get
            {
                if (_context != null)
                    _connectionID = _context.ConnectionID;

                return _connectionID;
            }
            set { _connectionID = value; }
        }

        /// <summary>
        /// Gets or sets, false if the connection was opened previous and has been kept alive, true if the connection is new and started.
        /// </summary>
        public bool IsStartOfConnection
        {
            get;
            set;
        }

        /// <summary>
        /// Clears all content input from the buffer stream.
        /// </summary>
        public void ClearResponse()
        {
            if (_context != null)
                _context.ResponseStream.Clear();
        }

        /// <summary>
        /// Clears all content output from the buffer stream.
        /// </summary>
        public void ClearRequest()
        {
            if (_context != null)
                _context.RequestStream.Clear();
        }

        /// <summary>
        /// Start a new async receive operation.
        /// </summary>
        /// <param name="buffer">The buffer to receive the request.</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <returns>The operation to perform.</returns>
        public Task<int> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken token)
        {
            // Create a new task.
            return Task<int>.Factory.StartNew(() =>
            {
                // Wait until data has been received.
                return ReceiveWait(buffer, token);

            }, token);
        }

        /// <summary>
        /// Wait until data has been received.
        /// </summary>
        /// <param name="buffer">The buffer to receive the request.</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <returns>The number of bytes read from the request stream.</returns>
        public int ReceiveWait(ArraySegment<byte> buffer, CancellationToken token)
        {
            int bytesRead = 0;

            // Make sure the buffer exists.
            if (buffer != null)
            {
                bool exitIndicator = false;
                byte[] store = new byte[buffer.Array.Length];

                // Create the tasks.
                Task[] tasks = new Task[1];

                // Poller task.
                Task poller = Task.Factory.StartNew(() =>
                {
                    // Create a new spin wait.
                    SpinWait sw = new SpinWait();

                    // Action to perform.
                    while (!exitIndicator)
                    {
                        try
                        {
                            // Get data from the input stream.
                            bytesRead = NetResponse.Input.Read(store, 0, store.Length);
                        }
                        catch { exitIndicator = true; }
                        
                        // The NextSpinWillYield property returns true if 
                        // calling sw.SpinOnce() will result in yielding the 
                        // processor instead of simply spinning. 
                        if (sw.NextSpinWillYield)
                        {
                            if (token.IsCancellationRequested || bytesRead > 0) exitIndicator = true;
                        }
                        sw.SpinOnce();
                    }
                });

                // Assign the listener task.
                tasks[0] = poller;

                // Wait for all tasks to complete.
                Task.WaitAll(tasks);

                // For each task.
                foreach (Task item in tasks)
                {
                    try
                    {
                        // Release the resources.
                        item.Dispose();
                    }
                    catch { }
                }
                tasks = null;

                // Copy the buffer.
                store.CopyTo(buffer.Array, 0);
                store = null;
            }

            // Return the number of bytes read.
            return bytesRead;
        }

        /// <summary>
        /// Start a new async send operation.
        /// </summary>
        /// <param name="buffer">The buffer containing the data to send.</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <returns>The operation to perform.</returns>
        public Task<int> SendAsync(ArraySegment<byte> buffer, CancellationToken token)
        {
            // Create a new task.
            return Task<int>.Factory.StartNew(() =>
            {
                // Wait until data has been sent.
                return SendWait(buffer, token);

            }, token);
        }

        /// <summary>
        /// Wait until data has been sent.
        /// </summary>
        /// <param name="buffer">The buffer containing the data to send.</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <returns>The number of bytes sent to the response stream.</returns>
        public int SendWait(ArraySegment<byte> buffer, CancellationToken token)
        {
            int bytesRead = 0;

            // Make sure the buffer exists.
            if (buffer != null)
            {
                bool exitIndicator = false;
                byte[] store = buffer.Array.ToArray();

                // Create the tasks.
                Task[] tasks = new Task[1];

                // Poller task.
                Task poller = Task.Factory.StartNew(() =>
                {
                    // Create a new spin wait.
                    SpinWait sw = new SpinWait();

                    // Action to perform.
                    while (!exitIndicator)
                    {
                        exitIndicator = true;

                        try
                        {
                            // Send data to the output stream.
                            NetRequest.Output.Write(store, 0, store.Length);
                            bytesRead = store.Length;
                        }
                        catch { exitIndicator = true; }

                        // The NextSpinWillYield property returns true if 
                        // calling sw.SpinOnce() will result in yielding the 
                        // processor instead of simply spinning. 
                        if (sw.NextSpinWillYield)
                        {
                            if (token.IsCancellationRequested || bytesRead > 0) exitIndicator = true;
                        }
                        sw.SpinOnce();
                    }
                });

                // Assign the listener task.
                tasks[0] = poller;

                // Wait for all tasks to complete.
                Task.WaitAll(tasks);

                // For each task.
                foreach (Task item in tasks)
                {
                    try
                    {
                        // Release the resources.
                        item.Dispose();
                    }
                    catch { }
                }
                tasks = null;
            }

            // Return the number of bytes read.
            return bytesRead;
        }

        /// <summary>
        /// Close and release all resources.
        /// </summary>
        public void Close()
        {
            // Close the connection.
            if (_context != null)
                _context.Close();
        }
    }
}
