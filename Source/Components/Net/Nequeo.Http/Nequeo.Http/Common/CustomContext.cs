﻿/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
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
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;
using System.Reflection;

using Nequeo.Net;
using Nequeo.Net.Provider;

namespace Nequeo.Net.Http
{
    /// <summary>
    /// Custom server context base provider.
    /// </summary>
    public abstract class CustomContext : ContextBase
    {
        /// <summary>
        /// Custom server context base provider.
        /// </summary>
        public CustomContext() { }

        private Action<Context> _onReceivedHandler = null;

        /// <summary>
        /// On http context received.
        /// </summary>
        /// <param name="context">The http context.</param>
        protected abstract void OnHttpContext(HttpContext context);

        /// <summary>
        /// Gets or sets the on received handler.
        /// </summary>
        protected override Action<Context> OnReceivedHandler
        {
            get
            {
                _onReceivedHandler = (context) => OnReceived(context);
                return _onReceivedHandler;
            }
        }

        /// <summary>
        /// On received.
        /// </summary>
        /// <param name="context">The base context.</param>
        private void OnReceived(Context context)
        {
            // Create the context.
            WebContext webContext = base.CreateWebContext(context);

            // Create the new http context from the web context.
            Nequeo.Net.Http.HttpContext httpContext = Nequeo.Net.Http.HttpContext.CreateFrom(webContext);

            // Assign response and request data.
            httpContext.HttpResponse = new HttpResponse();
            httpContext.HttpRequest = new HttpRequest();
            httpContext.HttpResponse.Output = webContext.WebResponse.Output;
            httpContext.HttpRequest.Input = webContext.WebRequest.Input;

            // Pass the http context.
            OnHttpContext(httpContext);
        }
    }
}
