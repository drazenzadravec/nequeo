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
using System.Net;
using System.Web;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;

using Nequeo.Extension;

namespace Nequeo.Net.Http2
{
    /// <summary>
    /// Http server.
    /// </summary>
    public class HttpServer : Nequeo.Net.WebServer
    {
        #region Constructors
        /// <summary>
        /// Http server.
        /// </summary>
        public HttpServer()
        {
            AssignOnConnectedActionHandler();
        }

        /// <summary>
        /// Http server.
        /// </summary>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public HttpServer(Net.Sockets.MultiEndpointModel[] multiEndpointModels, int maxNumClients = Int32.MaxValue)
            : base(multiEndpointModels, maxNumClients)
        {
            AssignOnConnectedActionHandler();
        }
        #endregion

        #region Private Fields
        private bool _usePriorities = false;
        private bool _useFlowControl = false;
        #endregion

        #region Public Events
        /// <summary>
        /// The on http context event handler, triggered when a new connection is establised or data is present. Should be used when implementing a new connection.
        /// </summary>
        public event Nequeo.Threading.EventHandler<Nequeo.Net.Http2.HttpContext> OnHttpContext;

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets an indicator specifying if priorities should be used.
        /// </summary>
        public bool UsePriorities
        {
            get { return _usePriorities; }
            set { _usePriorities = value; }
        }

        /// <summary>
        /// Gets or sets an indicator specifying if flow control should be used.
        /// </summary>
        public bool UseFlowControl
        {
            get { return _useFlowControl; }
            set { _useFlowControl = value; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Assign the on connected action handler.
        /// </summary>
        private void AssignOnConnectedActionHandler()
        {
            // Assign the on connect action handler.
            base.Timeout = 30;
            base.HeaderTimeout = 30000;
            base.RequestTimeout = 30000;
            base.ResponseTimeout = 30000;
            base.Name = "Nequeo Http2 Server";
            base.ServiceName = "Http2Server";
            base.SocketProviderHostPrefix = "Http2Server_";
            base.OnReceivedHandler = (Nequeo.Net.Provider.Context context) => OnReceivedActionHandler(context);
        }

        /// <summary>
        /// On received action handler.
        /// </summary>
        /// <param name="context">The current client context.</param>
        private void OnReceivedActionHandler(Nequeo.Net.Provider.Context context)
        {
            try
            {
                // Create the http context and set the headers.
                Nequeo.Net.Http2.HttpContext httpContext = CreateHttpContext(context);

                // Get the headers from the stream and assign the request data.
                bool canPassContext = false;
                bool frameExist = Nequeo.Net.Http2.Utility.ProcessFrameRequest(httpContext, out canPassContext, 
                    base.HeaderTimeout, base.MaximumReadLength);

                // If frame exists.
                if (frameExist && canPassContext)
                {
                    // If the event has been assigned.
                    if (OnHttpContext != null)
                        OnHttpContext(this, httpContext);

                    // Save the web context state objects.
                    SaveWebContext(context, httpContext);
                }
                else
                {
                    // If some error has occured.
                    if (!frameExist)
                        throw new Exception("Frame not found.");
                }
            }
            catch (Exception)
            {
                // Close the connection and release all resources used for communication.
                context.Close();
            }
        }

        /// <summary>
        /// Create the http context.
        /// </summary>
        /// <param name="context">The current client context.</param>
        /// <returns>The http context.</returns>
        private Nequeo.Net.Http2.HttpContext CreateHttpContext(Nequeo.Net.Provider.Context context)
        {
            // If the http context has not been assign to the state.
            if (context.State == null)
            {
                // Get the underlying web context.
                Nequeo.Net.WebContext webContext = CreateWebContext(context);

                // Create the new http context from the web context.
                Nequeo.Net.Http2.HttpContext httpContext = 
                    Nequeo.Net.Http2.HttpContext.CreateFrom(webContext, context.Request.Input, context.Response.Output);

                httpContext.UsePriorities = _usePriorities;
                httpContext.UseFlowControl = _useFlowControl;

                // Return the request context.
                context.State = httpContext;
                return httpContext;
            }
            else
            {
                // Get the saved context.
                Nequeo.Net.Http2.HttpContext httpContext = (Nequeo.Net.Http2.HttpContext)context.State;
                return httpContext;
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Get a new http response stream.
        /// </summary>
        /// <param name="output">The http response output stream.</param>
        /// <returns>The http response stream.</returns>
        protected virtual Nequeo.Net.Http2.HttpResponse GetHttpResponse(System.IO.Stream output)
        {
            Nequeo.Net.Http2.HttpResponse response = new Nequeo.Net.Http2.HttpResponse();
            return response;
        }

        /// <summary>
        /// Assign the request input stream and response output stream.
        /// </summary>
        /// <param name="context">The current client context.</param>
        /// <param name="webContext">The current web context.</param>
        protected override void AssignRequestResponseStreams(Provider.Context context, WebContext webContext)
        {
            // bypass this do not create the request and response in the web context.
        }

        /// <summary>
        /// Parse all header data.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="resource">Get the request or response with the supplied data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        protected override List<Model.NameValue> ParseHeaders(System.IO.Stream input, out string resource, long timeout = -1, int maxReadLength = 0)
        {
            throw new NotSupportedException("This method can not be used in this context.");
        }

        /// <summary>
        /// Parse all header data only, no initial resources data is present.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        protected override List<Model.NameValue> ParseHeadersOnly(byte[] input, long timeout = -1, int maxReadLength = 0)
        {
            throw new NotSupportedException("This method can not be used in this context.");
        }

        /// <summary>
        /// Parse all header data only, no initial resources data is present.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        protected override List<Model.NameValue> ParseHeadersOnly(System.IO.Stream input, long timeout = -1, int maxReadLength = 0)
        {
            throw new NotSupportedException("This method can not be used in this context.");
        }
        #endregion
    }

    /// <summary>
    /// Http server single.
    /// </summary>
    public class HttpServerSingle : Nequeo.Net.Provider.ServerSingleSocket
    {
        #region Constructors
        /// <summary>
        /// Http server.
        /// </summary>
        public HttpServerSingle()
        {
            AssignOnConnectedActionHandler();
        }

        /// <summary>
        /// Http server.
        /// </summary>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public HttpServerSingle(Net.Sockets.MultiEndpointModel[] multiEndpointModels, int maxNumClients = 10000)
            : base(multiEndpointModels, maxNumClients)
        {
            AssignOnConnectedActionHandler();
        }
        #endregion

        #region Private Fields
        private int _maxFrameBufferStore = 20000;
        private bool _usePriorities = false;
        private bool _useFlowControl = false;
        #endregion

        #region Public Events
        /// <summary>
        /// The on http context event handler, triggered when a new connection is establised or data is present. Should be used when implementing a new connection.
        /// </summary>
        public event Nequeo.Threading.EventHandler<Nequeo.Net.Http2.HttpContext> OnHttpContext;

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the maximum frame request buffer store read size before closing the connection. 
        /// </summary>
        public int MaxFrameBufferStore
        {
            get { return _maxFrameBufferStore; }
            set { _maxFrameBufferStore = value; }
        }

        /// <summary>
        /// Gets or sets an indicator specifying if priorities should be used.
        /// </summary>
        public bool UsePriorities
        {
            get { return _usePriorities; }
            set { _usePriorities = value; }
        }

        /// <summary>
        /// Gets or sets an indicator specifying if flow control should be used.
        /// </summary>
        public bool UseFlowControl
        {
            get { return _useFlowControl; }
            set { _useFlowControl = value; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Assign the on connected action handler.
        /// </summary>
        private void AssignOnConnectedActionHandler()
        {
            // Assign the on connect action handler.
            base.Timeout = 30;
            base.HeaderTimeout = 30000;
            base.RequestTimeout = 30000;
            base.ResponseTimeout = 30000;
            base.Name = "Nequeo Http2 Server";
            base.ServiceName = "Http2Server";
            base.SocketProviderHostPrefix = "Http2Server_";
            base.OnReceivedHandler = (Nequeo.Net.Provider.Context context) => OnReceivedActionHandler(context);
        }

        /// <summary>
        /// On received action handler.
        /// </summary>
        /// <param name="context">The current client context.</param>
        private void OnReceivedActionHandler(Nequeo.Net.Provider.Context context)
        {
            try
            {
                // Store data until all frame data has been read.
                if (Nequeo.Net.Http2.Utility.IsParseInitialFramePreamble(context.Request.Input, context.RequestBufferStore, _maxFrameBufferStore))
                {
                    // Create the http context and set the headers.
                    Nequeo.Net.Http2.HttpContext httpContext = CreateHttpContext(context);

                    // Get the headers from the stream and assign the request data.
                    bool canPassContext = false;
                    bool frameExist = Nequeo.Net.Http2.Utility.ProcessFrameRequest(httpContext, out canPassContext,
                        base.HeaderTimeout, base.MaximumReadLength, context.RequestBufferStore);

                    // If frame exeist.
                    if (frameExist && canPassContext)
                    {
                        // If the event has been assigned.
                        if (OnHttpContext != null)
                            OnHttpContext(this, httpContext);

                        // Save the web context state objects.
                        SaveWebContext(context, httpContext);
                    }
                    else
                    {
                        // If some error has occured.
                        if (!frameExist)
                            throw new Exception("Frame not found.");
                    }
                }

                // If the maximum request buffer store has been reached then close the connection.
                if (context.RequestBufferStore.Length > _maxFrameBufferStore)
                    throw new Exception("Maximum request buffer store has been reached.");
            }
            catch (Exception)
            {
                // Close the connection and release all resources used for communication.
                context.Close();
            }
        }

        /// <summary>
        /// Create the http context.
        /// </summary>
        /// <param name="context">The current client context.</param>
        /// <returns>The http context.</returns>
        private Nequeo.Net.Http2.HttpContext CreateHttpContext(Nequeo.Net.Provider.Context context)
        {
            // If the http context has not been assign to the state.
            if (context.State == null)
            {
                // Get the underlying web context.
                Nequeo.Net.WebContext webContext = CreateWebContext(context);

                // Create the new http context from the web context.
                Nequeo.Net.Http2.HttpContext httpContext = 
                    Nequeo.Net.Http2.HttpContext.CreateFrom(webContext, context.Request.Input, context.Response.Output);

                httpContext.UsePriorities = _usePriorities;
                httpContext.UseFlowControl = _useFlowControl;

                // Return the request context.
                context.State = httpContext;
                return httpContext;
            }
            else
            {
                // Get the saved context.
                Nequeo.Net.Http2.HttpContext httpContext = (Nequeo.Net.Http2.HttpContext)context.State;
                return httpContext;
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Get a new http response stream.
        /// </summary>
        /// <param name="output">The http response output stream.</param>
        /// <returns>The http response stream.</returns>
        protected virtual Nequeo.Net.Http2.HttpResponse GetHttpResponse(System.IO.Stream output)
        {
            Nequeo.Net.Http2.HttpResponse response = new Nequeo.Net.Http2.HttpResponse();
            return response;
        }

        /// <summary>
        /// Assign the request input stream and response output stream.
        /// </summary>
        /// <param name="context">The current client context.</param>
        /// <param name="webContext">The current web context.</param>
        protected override void AssignRequestResponseStreams(Provider.Context context, WebContext webContext)
        {
            // bypass this do not create the request and response in the web context.
        }

        /// <summary>
        /// Parse all header data.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="resource">Get the request or response with the supplied data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        protected override List<Model.NameValue> ParseHeaders(System.IO.Stream input, out string resource, long timeout = -1, int maxReadLength = 0)
        {
            throw new NotSupportedException("This method can not be used in this context.");
        }

        /// <summary>
        /// Parse all header data only, no initial resources data is present.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        protected override List<Model.NameValue> ParseHeadersOnly(byte[] input, long timeout = -1, int maxReadLength = 0)
        {
            throw new NotSupportedException("This method can not be used in this context.");
        }

        /// <summary>
        /// Parse all header data only, no initial resources data is present.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        protected override List<Model.NameValue> ParseHeadersOnly(System.IO.Stream input, long timeout = -1, int maxReadLength = 0)
        {
            throw new NotSupportedException("This method can not be used in this context.");
        }
        #endregion
    }
}
