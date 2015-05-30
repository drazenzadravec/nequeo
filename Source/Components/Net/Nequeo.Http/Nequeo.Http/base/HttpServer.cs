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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;

using Nequeo.Extension;

namespace Nequeo.Net.Http
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

        #region Public Events
        /// <summary>
        /// The on http context event handler, triggered when a new connection is establised or data is present. Should be used when implementing a new connection.
        /// </summary>
        public event Nequeo.Threading.EventHandler<Nequeo.Net.Http.HttpContext> OnHttpContext;

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
            base.Name = "Nequeo Http Server";
            base.ServiceName = "HttpServer";
            base.SocketProviderHostPrefix = "HttpServer_";
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
                Nequeo.Net.Http.HttpContext httpContext = CreateHttpContext(context);

                // Get the headers from the stream and assign the request data.
                bool headersExist = Nequeo.Net.Http.Utility.SetRequestHeaders(httpContext, base.HeaderTimeout, base.MaximumReadLength);

                // If the event has been assigned.
                if (OnHttpContext != null)
                    OnHttpContext(this, httpContext);

                // Save the web context state objects.
                SaveWebContext(context, httpContext);
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
        private Nequeo.Net.Http.HttpContext CreateHttpContext(Nequeo.Net.Provider.Context context)
        {
            // Get the underlying web context.
            Nequeo.Net.WebContext webContext = CreateWebContext(context);

            // Create the new http context from the web context.
            Nequeo.Net.Http.HttpContext httpContext = Nequeo.Net.Http.HttpContext.CreateFrom(webContext);

            // Assign response and request data.
            httpContext.HttpResponse = new HttpResponse();
            httpContext.HttpRequest = new HttpRequest();
            httpContext.HttpResponse.Output = webContext.WebResponse.Output;
            httpContext.HttpRequest.Input = webContext.WebRequest.Input;

            // Return the request context.
            return httpContext;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Get a new http response stream.
        /// </summary>
        /// <param name="output">The http response output stream.</param>
        /// <returns>The http response stream.</returns>
        protected virtual Nequeo.Net.Http.HttpResponse GetHttpResponse(System.IO.Stream output)
        {
            Nequeo.Net.Http.HttpResponse response = new Nequeo.Net.Http.HttpResponse();
            response.Output = output;
            return response;
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
        private int _maxHeaderBufferStore = 20000;
        #endregion

        #region Public Events
        /// <summary>
        /// The on http context event handler, triggered when a new connection is establised or data is present. Should be used when implementing a new connection.
        /// </summary>
        public event Nequeo.Threading.EventHandler<Nequeo.Net.Http.HttpContext> OnHttpContext;

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the maximum header request buffer store read size before closing the connection. 
        /// </summary>
        public int MaxHeaderBufferStore
        {
            get { return _maxHeaderBufferStore; }
            set { _maxHeaderBufferStore = value; }
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
            base.Name = "Nequeo Http Server";
            base.ServiceName = "HttpServer";
            base.SocketProviderHostPrefix = "HttpServer_";
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
                // Store data until all headers have been read.
                if (Nequeo.Net.Utility.IsParse2CRLF(context.Request.Input, context.RequestBufferStore, _maxHeaderBufferStore))
                {
                    // Create the http context and set the headers.
                    Nequeo.Net.Http.HttpContext httpContext = CreateHttpContext(context);

                    // Get the headers from the stream and assign the request data.
                    bool headersExist = Nequeo.Net.Http.Utility.SetRequestHeaders(httpContext, 
                        base.HeaderTimeout, base.MaximumReadLength, context.RequestBufferStore);

                    // If the event has been assigned.
                    if (OnHttpContext != null)
                        OnHttpContext(this, httpContext);

                    // Save the web context state objects.
                    SaveWebContext(context, httpContext);
                }

                // If the maximum request buffer store has been reached then close the connection.
                if (context.RequestBufferStore.Length > _maxHeaderBufferStore)
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
        private Nequeo.Net.Http.HttpContext CreateHttpContext(Nequeo.Net.Provider.Context context)
        {
            // Get the underlying web context.
            Nequeo.Net.WebContext webContext = CreateWebContext(context);

            // Create the new http context from the web context.
            Nequeo.Net.Http.HttpContext httpContext = Nequeo.Net.Http.HttpContext.CreateFrom(webContext);

            // Assign response and request data.
            httpContext.HttpResponse = new HttpResponse();
            httpContext.HttpRequest = new HttpRequest();
            httpContext.HttpResponse.Output = webContext.WebResponse.Output;
            httpContext.HttpRequest.Input = webContext.WebRequest.Input;

            // Return the request context.
            return httpContext;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Get a new http response stream.
        /// </summary>
        /// <param name="output">The http response output stream.</param>
        /// <returns>The http response stream.</returns>
        protected virtual Nequeo.Net.Http.HttpResponse GetHttpResponse(System.IO.Stream output)
        {
            Nequeo.Net.Http.HttpResponse response = new Nequeo.Net.Http.HttpResponse();
            response.Output = output;
            return response;
        }
        #endregion
    }
}
