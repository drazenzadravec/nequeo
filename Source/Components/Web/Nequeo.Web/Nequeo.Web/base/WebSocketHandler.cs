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
using System.Net;
using System.Web;
using System.Threading.Tasks;

namespace Nequeo.Web
{
    /// <summary>
    /// Http web socket handler.
    /// </summary>
    public abstract class WebSocketHandler : Nequeo.Web.WebServiceBase, IHttpHandler
    {
        /// <summary>
        /// Http web socket handler.
        /// </summary>
        protected WebSocketHandler()
        {
        }

        /// <summary>
        /// Called when a new web socket connection has been established.
        /// </summary>
        /// <param name="webSocketContext">The web socket context.</param>
        public abstract void WebSocketContext(System.Web.WebSockets.AspNetWebSocketContext webSocketContext);

        /// <summary>
        /// Return resuse state option
        /// </summary>
        public virtual bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        /// <summary>
        /// Process request method.
        /// </summary>
        /// <param name="context">The current http context.</param>
        public void ProcessRequest(System.Web.HttpContext context)
        {
            HttpResponse response = null;

            // If the context exists.
            if (context != null)
            {
                // Get the request and response context.
                response = context.Response;

                // If the request is a web socket protocol
                if (context.IsWebSocketRequest)
                {
                    // Process the request.
                    ProcessWebSocketRequest(context);
                }
                else
                {
                    try
                    {
                        if (response != null)
                        {
                            // Get the response OutputStream and write the response to it.
                            response.AddHeader("Content-Length", (0).ToString());
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
                        response.AddHeader("Content-Length", (0).ToString());
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        response.StatusDescription = "Internal Server Error";
                        response.Close();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Transfer the stream data from the source to the destination.
        /// </summary>
        /// <param name="source">The source stream to read from.</param>
        /// <param name="destination">The destination stream to write to.</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of source bytes). Waits until all bytes are read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        protected bool TransferData(System.IO.Stream source, System.IO.Stream destination, long byteLength, long timeout = -1)
        {
            return Nequeo.IO.Stream.Operation.CopyStream(source, destination, byteLength, timeout);
        }

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <param name="httpContext">The http context.</param>
        private void ProcessWebSocketRequest(System.Web.HttpContext httpContext)
        {
            HttpResponse response = null;

            try
            {
                // Get the request and response context.
                response = httpContext.Response;

                // Process the request asynchronously.
                httpContext.AcceptWebSocketRequest(ProcessWebSocketRequestAsync);
            }
            catch (Exception)
            {
                try
                {
                    if (response != null)
                    {
                        // Get the response OutputStream and write the response to it.
                        response.AddHeader("Content-Length", (0).ToString());
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        response.StatusDescription = "Internal Server Error";
                        response.Close();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Process the request asynchronously.
        /// </summary>
        /// <param name="webSocketContext">The web socket context.</param>
        /// <returns>The task to execute.</returns>
        private async Task ProcessWebSocketRequestAsync(System.Web.WebSockets.AspNetWebSocketContext webSocketContext)
        {
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    // Process the request.
                    WebSocketContext(webSocketContext);
                });
        }
    }
}
