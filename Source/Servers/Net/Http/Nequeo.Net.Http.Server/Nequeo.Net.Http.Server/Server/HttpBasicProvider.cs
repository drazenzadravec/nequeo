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
using System.Security.Principal;

using Nequeo.Net.Common;
using Nequeo.Handler;
using Nequeo.Net.Configuration;
using Nequeo.ComponentModel.Composition;
using Nequeo.Composite.Configuration;

namespace Nequeo.Net.Server
{
    /// <summary>
    /// Basic http server.
    /// </summary>
    [Logging(Handler.WriteTo.EventLog, Handler.LogType.Error)]
    public class HttpBasicProvider : ServiceBase, IDisposable
    {
        /// <summary>
        /// Http html server
        /// </summary>
        /// <param name="urlBaseAddress">The Url base this sample will listen for. The URL must not have a query element.</param>
        /// <param name="localBaseDirectory">The local directory to map incoming requested Url to. Note that this path should not include a trailing slash.</param>
        /// <param name="providerName">The provider name (host="htmlprovider") in the configuration file.</param>
        public HttpBasicProvider(String urlBaseAddress = null, String localBaseDirectory = null, string providerName = "htmlbasicprovider")
        {
            _urlBaseAddress = urlBaseAddress;
            _localBaseDirectory = localBaseDirectory;
            _providerName = providerName;
        }

        private bool _initialized = false;
        private bool _running = false;
        private Nequeo.Net.Server.HttpServer _listener;
        private Nequeo.Net.Data.context _contextMimeType = null;

        private string _providerName = "htmlbasicprovider";
        private string _urlBaseAddress = null;
        private string _localBaseDirectory = null;

        /// <summary>
        /// Gets the state of the server.
        /// </summary>
        public bool IsRunning
        {
            get { return _running; }
        }

        /// <summary>
        /// Gets sets the provider name, (host="htmlbasicprovider") in the configuration file.
        /// </summary>
        public string ProviderName
        {
            get { return _providerName; }
            set { _providerName = value; }
        }

        /// <summary>
        /// Gets sets the Url base this sample will listen for. The URL must not have a query element.
        /// </summary>
        public string UrlBaseAddress
        {
            get { return _urlBaseAddress; }
            set { _urlBaseAddress = value; }
        }

        /// <summary>
        /// Gets sets the local directory to map incoming requested Url to. Note that this path should not include a trailing slash.
        /// </summary>
        public string LocalBaseDirectory
        {
            get { return _localBaseDirectory; }
            set { _localBaseDirectory = value; }
        }

        /// <summary>
        /// Start the http listener.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when the urlBaseAddress parameter is missing.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when the localBaseDirectory parameter is missing.</exception>
        public void Start()
        {
            // If the server is already running.
            if (_running)
                return;

            bool ret = false;

            try
            {
                // Make sure the url base address is set.
                if (System.String.IsNullOrEmpty(_urlBaseAddress))
                    throw new ArgumentNullException("urlBaseAddress");

                // Make sure the local base directory is set.
                if (System.String.IsNullOrEmpty(_localBaseDirectory))
                    throw new ArgumentNullException("localBaseDirectory");

                // Create a new http listener
                _listener = new Nequeo.Net.Server.HttpServer(_urlBaseAddress, _localBaseDirectory);

                // Get the mime types
                _contextMimeType = ReaderHttp.GetMimeType();

                // Initialise the listener
                if (!_initialized)
                {
                    ret = _listener.Initialize();
                    _initialized = ret;
                }

                // Start the listener
                if (ret)
                    ret = _listener.Start();

                // Assign the running indicator.
                if (ret)
                    _running = true;
            }
            catch (Exception ex)
            {
                // Log the error.
                LogHandler.WriteTypeMessage(
                    ex.Message,
                    MethodInfo.GetCurrentMethod(),
                    Nequeo.Net.Common.Helper.EventApplicationName);

                // Stop the http listener.
                Stop();
            }
        }

        /// <summary>
        /// Stop the http listener.
        /// </summary>
        public void Stop()
        {
            try
            {
                // Stop the service.
                _running = false;

                if (_listener != null)
                    _listener.Stop();
            }
            catch (Exception ex)
            {
                // Log the error.
                LogHandler.WriteTypeMessage(
                    ex.Message,
                    MethodInfo.GetCurrentMethod(),
                    Nequeo.Net.Common.Helper.EventApplicationName);
            }
            finally
            {
                _listener = null;
            }
        }

        #region Dispose Object Methods

        private bool _disposed = false;

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

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
                    if (_listener != null)
                        _listener.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _listener = null;

            }
        }
        #endregion
    }
}
