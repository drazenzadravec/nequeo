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
using System.Net.WebSockets;
using System.Web;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;

namespace Nequeo.Net.WebSockets
{
    /// <summary>
    /// Web socket server uses the base System.Net.HttpListener provider for http context.
    /// </summary>
    public class WebSocketServer : Nequeo.Net.Http.HttpListenerBase
    {
        /// <summary>
        /// Web socket server uses the base System.Net.HttpListener provider for http context.
        /// </summary>
        public WebSocketServer() 
        {
            // Assign the on connect action handler.
            base.HttpContext = (HttpListenerContext context) => OnHttpContextHandler(context);
        }

        /// <summary>
        /// Web socket server uses the base System.Net.HttpListener provider for http context.
        /// </summary>
        /// <param name="uriList">The url prefix listening list.</param>
        public WebSocketServer(string[] uriList)
            : base(uriList)
        {
            // Assign the on connect action handler.
            base.HttpContext = (HttpListenerContext context) => OnHttpContextHandler(context);
        }

        /// <summary>
        /// The on web socket context event handler, triggered when a new connection is establised or data is present. Should be used when implementing a new connection.
        /// </summary>
        public event Nequeo.Threading.EventHandler<System.Net.WebSockets.HttpListenerWebSocketContext> OnWebSocketContext;

        /// <summary>
        /// On http context action handler.
        /// </summary>
        /// <param name="context">The current http context.</param>
        private void OnHttpContextHandler(HttpListenerContext context)
        {
            HttpListenerRequest request = null;
            HttpListenerResponse response = null;

            // If the context exists.
            if (context != null)
            {
                // Get the request and response context.
                request = context.Request;
                response = context.Response;

                // If the request is a web socket protocol
                if (request.IsWebSocketRequest)
                {
                    // Process the web socket request.
                    ProcessWebSocketRequest(context);
                }
                else
                {
                    try
                    {
                        if (response != null)
                        {
                            // Get the response OutputStream and write the response to it.
                            response.ContentLength64 = 0;
                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                            response.StatusDescription = "Bad Request";
                            response.Close();
                        }
                    }
                    catch { }
                }
            }
            else
            {
                try
                {
                    if (response != null)
                    {
                        // Get the response OutputStream and write the response to it.
                        response.ContentLength64 = 0;
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        response.StatusDescription = "Internal Server Error";
                        response.Close();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Process the web socket protocol request.
        /// </summary>
        /// <param name="context">The current http context.</param>
        private async void ProcessWebSocketRequest(HttpListenerContext context)
        {
            HttpListenerResponse response = null;
            
            try
            {
                // Get the request and response context.
                response = context.Response;

                // When calling `AcceptWebSocketAsync` the negotiated subprotocol 
                // must be specified. This sample assumes that no subprotocol  was requested. 
                HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
                
                // Send the web socket to the handler.
                if (OnWebSocketContext != null)
                    OnWebSocketContext(this, webSocketContext);
            }
            catch (Exception)
            {
                try
                {
                    if (response != null)
                    {
                        // Get the response OutputStream and write the response to it.
                        response.ContentLength64 = 0;
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        response.StatusDescription = "Internal Server Error";
                        response.Close();
                    }
                }
                catch { }
            }
        }
    }
}
