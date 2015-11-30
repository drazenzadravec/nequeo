/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Threading.Tasks;

using Nequeo.Invention;
using Nequeo.Serialisation;

namespace Nequeo.Net.Data
{
    /// <summary>
    /// Client connection context.
    /// </summary>
    internal class ConnectionContext
    {
        /// <summary>
        /// Gets or sets, the client.
        /// </summary>
        public Nequeo.Net.NetClient Client { get; set; }

        /// <summary>
        /// Gets or sets the name of the load balance server the client is connected to.
        /// </summary>
        public string LoadBalanceServer { get; set; }
    }

    /// <summary>
    /// Helper.
    /// </summary>
    internal class Helper
    {
        /// <summary>
        /// File cache provider.
        /// </summary>
        public static Nequeo.IO.File.Cache FileCache = new IO.File.Cache();

        /// <summary>
        /// Application hosting provider.
        /// </summary>
        public static Nequeo.Web.Hosting.Application Application = new Nequeo.Web.Hosting.Application();

        /// <summary>
        /// Load balance server xml path.
        /// </summary>
        public static string LoadBalanceServerXmlPath = Nequeo.Net.Properties.Settings.Default.LoadBalanceServerXmlPath;

        /// <summary>
        /// Get the load balance server data.
        /// </summary>
        /// <returns></returns>
        public static Nequeo.Net.Data.context GetLoadBalanceServer()
        {
            try
            {
                string xmlValidationMessage = string.Empty;

                // Get the xml file location and
                // the xsd file schema.
                string xml = Helper.LoadBalanceServerXmlPath;
                string xsd = Nequeo.Net.Properties.Resources.LoadBalanceServer;

                // Validate the filter xml file.
                if (!Validation.IsXmlValidEx(xsd, xml, out xmlValidationMessage))
                    throw new Exception("Xml validation. " + xmlValidationMessage);

                // Deserialise the xml file into
                // the log directory list object
                GeneralSerialisation serial = new GeneralSerialisation();
                Nequeo.Net.Data.context loadData =
                    ((Nequeo.Net.Data.context)serial.Deserialise(typeof(Nequeo.Net.Data.context), xml));

                // Return the load balance data.
                return loadData;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Application host domain helper.
    /// </summary>
    internal class AppHostDomain : IDisposable
    {
        /// <summary>
        /// Application host domain.
        /// </summary>
        /// <param name="basePath">The base document path.</param>
        /// <param name="virtualDir">The virtual path to the application directory; for example, "/app".</param>
        /// <param name="configurationFile">The application configuration file (e.g. "[application].config").</param>
        public AppHostDomain(string basePath, string virtualDir = "/", string configurationFile = null)
        {
            _basePath = basePath;
            _virtualDir = virtualDir;
            _configurationFile = configurationFile;

            // Create the application host.
            _applicationHost = new Web.Hosting.ApplicationHost();
        }

        private object _lockObject = new object();
        private Nequeo.Web.Hosting.ApplicationHost _applicationHost = null;

        // Make sure that the 'Host' Nequeo.Web.Hosting.dll is placed in the 'bin' folder where the basePath files are placed.
        // basePath = C:\Temp\Html\ then C:\Temp\Html\bin should contain Nequeo.Web.Hosting.dll.
        private Nequeo.Web.Hosting.Host _host = null;

        private string _configurationFile = null;
        private string _basePath = string.Empty;
        private string _virtualDir = "/";

        /// <summary>
        /// Create the application host.
        /// </summary>
        public void CreateApplicationHost()
        {
            lock (_lockObject)
            {
                // If null.
                if (_host == null)
                {
                    // Create the application host.
                    _host = (Nequeo.Web.Hosting.Host)_applicationHost.CreateApplicationHost(typeof(Nequeo.Web.Hosting.Host), _virtualDir, _basePath, configurationFile: _configurationFile);
                }
            }
        }

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <param name="page">The page to load.</param>
        /// <param name="request">The current http request.</param>
        /// <param name="streamWriter">The output stream writer.</param>
        public void ProcessRequest(string page, Nequeo.Net.Http.HttpRequest request, System.IO.StreamWriter streamWriter)
        {
            lock(_lockObject)
            {
                try
                {
                    // Execute the host.
                    _host.ProcessRequest(page, streamWriter, (String.IsNullOrEmpty(request.Url.Query) ? null : request.Url.Query));
                }
                catch (System.Runtime.Remoting.RemotingException)
                {
                    // Create a new host instance when this one is released.
                    _host = null;

                    // Create the application host.
                    CreateApplicationHost();
                    System.Threading.Thread.Sleep(400);

                    try
                    {
                        // Execute the host.
                        _host.ProcessRequest(page, streamWriter, (String.IsNullOrEmpty(request.Url.Query) ? null : request.Url.Query));
                    }
                    catch { }
                }
            }
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
                    if (_applicationHost != null)
                        _applicationHost.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _host = null;
                _applicationHost = null;
                _lockObject = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~AppHostDomain()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
