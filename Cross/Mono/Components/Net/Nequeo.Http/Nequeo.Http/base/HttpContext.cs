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

namespace Nequeo.Net.Http
{
    /// <summary>
    /// Http server context app domain host.
    /// </summary>
    public sealed class HttpContextHost : MarshalByRefObject
    {
        /// <summary>
        /// Http server context app domain host.
        /// </summary>
        /// <param name="context">The http context.</param>
        public HttpContextHost(HttpContext context)
        {
            _context = context;
        }

        private HttpContext _context = null;

        /// <summary>
        /// Gets the http context.
        /// </summary>
        public HttpContext Context
        {
            get { return _context; }
        }
    }

    /// <summary>
    /// Http server context.
    /// </summary>
    public sealed class HttpContext : Nequeo.Net.WebContext
    {
        /// <summary>
        /// Create the http context from the web context.
        /// </summary>
        /// <param name="webContext">The web context to create from.</param>
        /// <returns>The http server context.</returns>
        public static HttpContext CreateFrom(Nequeo.Net.WebContext webContext)
        {
            Nequeo.Net.Http.HttpContext httpContext = new Nequeo.Net.Http.HttpContext();
            httpContext.Context = webContext.Context;
            httpContext.IsStartOfConnection = webContext.IsStartOfConnection;
            httpContext.IsAuthenticated = webContext.IsAuthenticated;
            httpContext.IsSecureConnection = webContext.IsSecureConnection;
            httpContext.Name = webContext.Name;
            httpContext.NumberOfClients = webContext.NumberOfClients;
            httpContext.Port = webContext.Port;
            httpContext.RemoteEndPoint = webContext.RemoteEndPoint;
            httpContext.ServerEndPoint = webContext.ServerEndPoint;
            httpContext.ServiceName = webContext.ServiceName;
            httpContext.UniqueIdentifier = webContext.UniqueIdentifier;
            httpContext.ConnectionID = webContext.ConnectionID;
            httpContext.SessionID = webContext.SessionID;
            httpContext.User = webContext.User;
            httpContext.SocketState = webContext.SocketState;
            httpContext.IsAsyncMode = webContext.IsAsyncMode;
            return httpContext;
        }

        /// <summary>
        /// Gets the http request.
        /// </summary>
        public Nequeo.Net.Http.HttpRequest HttpRequest
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the http response.
        /// </summary>
        public Nequeo.Net.Http.HttpResponse HttpResponse
        {
            get;
            set;
        }
    }
}
