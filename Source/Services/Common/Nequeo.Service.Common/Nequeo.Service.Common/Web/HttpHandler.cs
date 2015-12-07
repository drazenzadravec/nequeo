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
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Net;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Web.Hosting;
using System.Web;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Nequeo.Service.Web
{
    /// <summary>
    /// Http handler.
    /// </summary>
    public abstract class HttpHandler : Nequeo.Web.WebServiceBase, IHttpAsyncHandler
    {
        /// <summary>
        /// Http handler.
        /// </summary>
        protected HttpHandler()
        {
        }

        /// <summary>
        /// Http context.
        /// </summary>
        /// <param name="context">The current http context.</param>
        public abstract void HttpContext(System.Web.HttpContext context);

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
        /// Begin process request
        /// </summary>
        /// <param name="context">The current http context.</param>
        /// <param name="cb">The call back handler</param>
        /// <param name="extraData">Additional data to pass.</param>
        /// <returns>The async result.</returns>
        public IAsyncResult BeginProcessRequest(System.Web.HttpContext context, AsyncCallback cb, object extraData)
        {
            // Process the task.
            return Nequeo.Threading.AsyncOperationResult<Boolean>.
                RunTask(() => ProcessRequest(context)).ContinueWith(t => cb(t));
        }

        /// <summary>
        /// End the process request
        /// </summary>
        /// <param name="result">The async result.</param>
        public void EndProcessRequest(IAsyncResult result)
        {
            if (result != null)
                ((Task)result).Dispose();
        }

        /// <summary>
        /// Process request method.
        /// </summary>
        /// <param name="context">The current http context.</param>
        public void ProcessRequest(System.Web.HttpContext context)
        {
            HttpContext(context);
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
    }
}
