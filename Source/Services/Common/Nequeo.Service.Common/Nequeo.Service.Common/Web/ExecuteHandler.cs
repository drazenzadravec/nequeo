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
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Web.Hosting;
using System.Web;

using Nequeo.Net.ServiceModel;
using Nequeo.Net.Http;
using Nequeo.Handler;

namespace Nequeo.Service.Web
{
    /// <summary>
    /// Web page execution handler.
    /// </summary>
    public abstract class ExecuteHandler : IHttpHandler
    {
        /// <summary>
        /// Compose the MEF instance.
        /// </summary>
        public abstract void Compose();

        /// <summary>
        /// Get the collection of virtual paths that are to be executed in sequence.
        /// </summary>
        /// <param name="context">The current http context.</param>
        /// <param name="serviceName">The unique service method name.</param>
        /// <returns>The collection of virtual paths.</returns>
        public abstract string[] VirtualPageUriCollection(System.Web.HttpContext context, string serviceName);

        /// <summary>
        /// Gets the unique service name for the current operation.
        /// </summary>
        /// <param name="context">The current http context.</param>
        /// <returns>The unique service name</returns>
        public abstract string ServiceName(System.Web.HttpContext context);

        /// <summary>
        /// Gets sets the current error.
        /// </summary>
        public abstract Exception Exception { get; set; }

        private int _timeout = 30000;

        /// <summary>
        /// Gets or sets the transfer time.
        /// </summary>
        protected int Timeout { get { return _timeout; } set { _timeout = value; } }

        /// <summary>
        /// Return resuse state option
        /// </summary>
        public bool IsReusable
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
            MemoryStream receivedMessageData = null;
            MemoryStream sendMessageData = null;
            string serviceName = string.Empty;

            try
            {
                // Initialise the composition assembly collection.
                Compose();

                // Get the current service name request.
                serviceName = ServiceName(context);

                // Get the collection of virtual pages to load.
                string[] virtualPages = VirtualPageUriCollection(context, serviceName);

                try
                {
                    // Execute each of the virtual pages, into
                    // one single response handler.
                    if (virtualPages != null)
                        foreach (string item in virtualPages)
                            context.Server.Execute(item);
                }
                catch (Exception exVirtualUri)
                {
                    // Get the current exception.
                    Exception = exVirtualUri;
                }
            }
            catch (System.Threading.ThreadAbortException)
            { }
            catch (Exception ex)
            {
                // Get the current exception.
                Exception = ex;
            }
            finally
            {
                // Clean-up
                if (receivedMessageData != null)
                    receivedMessageData.Close();

                // Clean-up
                if (sendMessageData != null)
                    sendMessageData.Close();

                // Clean-up
                if (context.Response != null)
                    if (context.Response.OutputStream != null)
                        context.Response.OutputStream.Close();

                // Clean-up
                if (context.Request != null)
                    if (context.Request.InputStream != null)
                        context.Request.InputStream.Close();
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
    }
}
