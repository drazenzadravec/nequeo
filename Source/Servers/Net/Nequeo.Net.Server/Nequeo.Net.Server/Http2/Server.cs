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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nequeo.Net.Http2
{
    /// <summary>
    /// Http2 static content server.
    /// </summary>
    public class Server : IDisposable
    {
        /// <summary>
        /// Http2 static content server.
        /// </summary>
        /// <param name="basePath">The base document path.</param>
        public Server(string basePath)
        {
            _basePath = basePath;

            // Initialise the server.
            Init();
        }

        private ConcurrentDictionary<string, HttpServerContext> _serverList = null;
        private Nequeo.Net.Http2.HttpServer _httpServer = null;
        private string _basePath = string.Empty;
        private long _headerTimeout = 30000;

        /// <summary>
        /// Gets the http2 server.
        /// </summary>
        public Nequeo.Net.Http2.HttpServer HttpServer
        {
            get { return _httpServer; }
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        public void Start()
        {
            try
            {
                // Start the server.
                if (_httpServer != null)
                    _httpServer.Start();
            }
            catch (Exception)
            {
                if (_httpServer != null)
                    _httpServer.Dispose();

                _httpServer = null;
                throw;
            }
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop()
        {
            try
            {
                // Stop the server.
                if (_httpServer != null)
                    _httpServer.Stop();
            }
            catch { }
            finally
            {
                if (_httpServer != null)
                    _httpServer.Dispose();

                _httpServer = null;
            }
        }

        /// <summary>
        /// Initialise the server.
        /// </summary>
        private void Init()
        {
            try
            {
                // Create the http server context list.
                _serverList = new ConcurrentDictionary<string, HttpServerContext>();

                // Create the server endpoint.
                Nequeo.Net.Sockets.MultiEndpointModel[] model = new Nequeo.Net.Sockets.MultiEndpointModel[]
                {
                    // None secure.
                    new Nequeo.Net.Sockets.MultiEndpointModel()
                    {
                        Port = Nequeo.Net.Properties.Settings.Default.HttpStaticServerListeningPort,
                        Addresses = new System.Net.IPAddress[]
                        {
                            System.Net.IPAddress.IPv6Any,
                            System.Net.IPAddress.Any
                        }
                    },
                    // Secure.
                    new Nequeo.Net.Sockets.MultiEndpointModel()
                    {
                        Port = Nequeo.Net.Properties.Settings.Default.HttpStaticServerListeningPortSecure,
                        Addresses = new System.Net.IPAddress[]
                        {
                            System.Net.IPAddress.IPv6Any,
                            System.Net.IPAddress.Any
                        }
                    }
                };

                // Start the server.
                _httpServer = new Nequeo.Net.Http2.HttpServer(model);
                _httpServer.Name = "Application Server";
                _httpServer.OnHttpContext += HttpContext;
                _httpServer.Timeout = 30;
                _httpServer.ReadBufferSize = 32768;
                _httpServer.WriteBufferSize = 32768;
                _httpServer.OnClientConnected = (server, context) => Server_OnConnected(server, context);
                _httpServer.OnClientDisconnected = (context) => Server_OnDisconnected(context);

                // Set the header readtimeout.
                _headerTimeout = _httpServer.HeaderTimeout;

                // Get the certificate reader.
                Nequeo.Security.Configuration.Reader certificateReader = new Nequeo.Security.Configuration.Reader();

                // Inititalise.
                _httpServer.Initialisation();

                try
                {
                    // Look for the certificate information in the configuration file.

                    // Get the certificate if any.
                    X509Certificate2 serverCertificate = certificateReader.GetServerCredentials();

                    // If a certificate exists.
                    if (serverCertificate != null)
                    {
                        // Get the secure servers.
                        _httpServer.Server[2].UseSslConnection = true;
                        _httpServer.Server[2].X509Certificate = serverCertificate;
                        _httpServer.Server[3].UseSslConnection = true;
                        _httpServer.Server[3].X509Certificate = serverCertificate;
                    }
                }
                catch { }
            }
            catch (Exception)
            {
                if (_httpServer != null)
                    _httpServer.Dispose();

                _httpServer = null;
                throw;
            }
        }

        /// <summary>
        /// Disconnected client.
        /// </summary>
        /// <param name="context">The server context.</param>
        private void Server_OnDisconnected(Sockets.IServerContext context)
        {
            HttpServerContext httpContext = null;
            bool ret = _serverList.TryRemove(context.ConnectionID, out httpContext);

            // If found then release.
            if (ret)
            {
                // Release all resources.
                if (httpContext != null)
                    httpContext.Dispose();

                httpContext = null;
            }
        }

        /// <summary>
        /// Connected client.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="context">The server context.</param>
        private void Server_OnConnected(Sockets.IServer server, Sockets.IServerContext context) { }

        /// <summary>
        /// Http context.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="context"></param>
        private void HttpContext(object sender, Nequeo.Net.Http2.HttpContext context)
        {
            context.IsAsyncMode = true;
            HttpServerContext httpContext = new HttpServerContext(context, _headerTimeout);
            _serverList.TryAdd(context.ConnectionID, httpContext);
        }

        /// <summary>
        /// Http server context handler.
        /// </summary>
        internal class HttpServerContext : IDisposable
        {
            /// <summary>
            /// Http2 context handler.
            /// </summary>
            /// <param name="context">The currrent client http2 context.</param>
            /// <param name="frameReadTimeout">The frame read timeout.</param>
            public HttpServerContext(Nequeo.Net.Http2.HttpContext context, long frameReadTimeout)
            {
                _context = context;
                _isStartOfContext = true;
                _frameReadTimeout = frameReadTimeout;
                _useStreamPriority = _context.UsePriorities;
                _streamRequestQueue = new ConcurrentQueue<int>();
                _context.Context.ReceivedAsyncMode = () => AsyncReceived();

                try
                {
                    // Create new task and start the processing.
                    _processTask = Task.Factory.StartNew(() => ProcessRequestTask());
                    _processTask.Start();
                }
                catch (Exception)
                {
                    _errorOrCloseConnection = true;
                    _context.Close();
                }
            }

            private Task _processTask = null;
            private object _lockContext = new object();
            private ConcurrentQueue<int> _streamRequestQueue = null;

            private long _frameReadTimeout = 30000;
            private int _hasProcessingFrame = 0;
            private volatile bool _isStartOfContext = false;
            private volatile bool _errorOrCloseConnection = false;

            private bool _useStreamPriority = false;
            private Nequeo.Net.Http2.HttpContext _context = null;

            /// <summary>
            /// Receives data from the client in async mode.
            /// </summary>
            private void AsyncReceived()
            {
                // Make sure only one thread at a time is processing.
                lock (_lockContext)
                {
                    // If active.
                    if (_context != null && _context.SocketState == SocketState.Open)
                    {
                        // If not the start of the context.
                        if (!_isStartOfContext)
                        {
                            // Process a new frame.
                            bool canPassContext = false;
                            bool frameExist = Nequeo.Net.Http2.Utility.ProcessFrameRequest(_context, out canPassContext, _frameReadTimeout);

                            // If frame exists.
                            if (frameExist && canPassContext)
                            {
                                // Process the request.
                                _streamRequestQueue.Enqueue(_context.StreamId);
                                Interlocked.Exchange(ref _hasProcessingFrame, 1);
                            }
                            else
                            {
                                // If some error has occured.
                                if (!frameExist)
                                {
                                    // Close the connection and release all resources used for communication.
                                    _errorOrCloseConnection = true;
                                    _context.Close();
                                }
                            }
                        }
                        else
                        {
                            // Start of new context.
                            // Process the request.
                            _streamRequestQueue.Enqueue(_context.StreamId);
                            Interlocked.Exchange(ref _hasProcessingFrame, 1);
                        }
                    }
                    else
                    {
                        // Close the connection and release all resources used for communication.
                        _errorOrCloseConnection = true;
                    }

                    // End of initial context.
                    _isStartOfContext = false;
                }
            }

            /// <summary>
            /// Process the request.
            /// </summary>
            private void ProcessRequestTask()
            {
                // Create a new spin wait.
                SpinWait sw = new SpinWait();

                // While no error and connection is open.
                while (!_errorOrCloseConnection)
                {
                    // The NextSpinWillYield property returns true if 
                    // calling sw.SpinOnce() will result in yielding the 
                    // processor instead of simply spinning. 
                    if (sw.NextSpinWillYield)
                    {
                        // If the location is 1 then continue and replace with 0.
                        if (Interlocked.CompareExchange(ref _hasProcessingFrame, 0, 1) == 1)
                        {
                            // Process the request.
                            ProcessRequest();
                        }
                    }

                    // Performs a single spin.
                    sw.SpinOnce();
                }
            }

            /// <summary>
            /// Process the request.
            /// </summary>
            private void ProcessRequest()
            {
                // If not using stream priority then process as requests come in.
                if (!_useStreamPriority)
                {
                    int streamID = 0;

                    // While requests exist.
                    while (_streamRequestQueue.TryDequeue(out streamID))
                    {
                        HttpStream httpStream = null;

                        // Make sure only one thread at a time is processing.
                        lock (_lockContext)
                        {
                            // Get the current stream.
                            httpStream = _context.GetHttpStream(streamID);
                        }

                        // If stream is active.
                        if (httpStream != null && httpStream.HttpRequest != null)
                        {
                            // Process the request.
                            ProcessStreamRequest(httpStream);
                        }
                    }
                }
                else
                {
                    // Use stream priority.
                }
            }

            /// <summary>
            /// Process the stream request.
            /// </summary>
            /// <param name="httpStream">The current stream.</param>
            private void ProcessStreamRequest(HttpStream httpStream)
            {
                // Select the frame type.
                switch (httpStream.FrameType)
                {
                    case Protocol.OpCodeFrame.Continuation:
                    case Protocol.OpCodeFrame.Headers:
                        // If end of headers.
                        if (httpStream.HttpRequest.IsEndOfData)
                        {
                            // Process the stream request.
                            ProcessStreamRequestEx(httpStream);
                        }
                        break;

                    case Protocol.OpCodeFrame.Data:
                        // This server does not allow POST of data.
                        break;
                }
            }

            /// <summary>
            /// Process the stream request.
            /// </summary>
            /// <param name="httpStream">The current stream.</param>
            private void ProcessStreamRequestEx(HttpStream httpStream)
            {
                System.IO.MemoryStream unzip = null;
                System.IO.MemoryStream zip = null;

                Nequeo.Net.Http2.HttpRequest request = null;
                Nequeo.Net.Http2.HttpResponse response = null;

                byte[] responseData = null;
                byte[] requestBuffer = null;

                try
                {
                    // Get the context data.
                    request = httpStream.HttpRequest;
                    response = httpStream.HttpResponse;

                }
                catch { }
            }

            #region Dispose Object Methods
            /// <summary>
            /// Track whether Dispose has been called.
            /// </summary>
            private bool _disposed = false;

            /// <summary>
            /// Implement IDisposable.
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
            /// Dispose(bool disposing) executes in two distinct scenarios.  If disposing
            /// equals true, the method has been called directly or indirectly by a user's
            /// code. Managed and unmanaged resources can be disposed.  If disposing equals
            /// false, the method has been called by the runtime from inside the finalizer
            /// and you should not reference other objects. Only unmanaged resources can
            /// be disposed.
            /// </summary>
            protected virtual void Dispose(bool disposing)
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
                        // Close the connection.
                        _errorOrCloseConnection = true;

                        try
                        {
                            // Release the resources.
                            _processTask.Dispose();
                        }
                        catch { }
                    }

                    // Call the appropriate methods to clean up
                    // unmanaged resources here.
                    _lockContext = null;
                    _processTask = null;
                    _streamRequestQueue = null;
                }
            }

            /// <summary>
            /// Use C# destructor syntax for finalization code.
            /// This destructor will run only if the Dispose method
            /// does not get called.
            /// It gives your base class the opportunity to finalize.
            /// Do not provide destructors in types derived from this class.
            /// </summary>
            ~HttpServerContext()
            {
                // Do not re-create Dispose clean-up code here.
                // Calling Dispose(false) is optimal in terms of
                // readability and maintainability.
                Dispose(false);
            }
            #endregion
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
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
        /// Dispose(bool disposing) executes in two distinct scenarios.  If disposing
        /// equals true, the method has been called directly or indirectly by a user's
        /// code. Managed and unmanaged resources can be disposed.  If disposing equals
        /// false, the method has been called by the runtime from inside the finalizer
        /// and you should not reference other objects. Only unmanaged resources can
        /// be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
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
                    if (_httpServer != null)
                        _httpServer.Dispose();

                    if (_serverList != null)
                        _serverList.Clear();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _httpServer = null;
                _serverList = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Server()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
