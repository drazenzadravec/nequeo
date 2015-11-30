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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nequeo.Net.Download
{
    /// <summary>
    /// Download manager server.
    /// </summary>
    public class Manager : IDisposable
    {
        /// <summary>
        /// Download manager server.
        /// </summary>
        /// <param name="downloadBasePath">The base download path.</param>
        public Manager(string downloadBasePath) 
        {
            _basePath = downloadBasePath;

            // Initialise the server.
            Init();
        }

        private Nequeo.Net.Download.ManagerServer _server = null;
        private string _basePath = string.Empty;

        /// <summary>
        /// Gets the http server.
        /// </summary>
        public Nequeo.Net.Download.ManagerServer Server
        {
            get { return _server; }
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        public void Start()
        {
            try
            {
                // Start the server.
                if (_server != null)
                    _server.Start();
            }
            catch (Exception)
            {
                if (_server != null)
                    _server.Dispose();

                _server = null;
                throw;
            }
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop()
        {
            try
            {
                // Stop the server.
                if (_server != null)
                    _server.Stop();
            }
            catch { }
            finally
            {
                if (_server != null)
                    _server.Dispose();

                _server = null;
            }
        }

        /// <summary>
        /// Initialise the server.
        /// </summary>
        private void Init()
        {
            try
            {
                string socketProviderHostPrefix = "DownloadManagerServer_";
                string hostProviderFullName = socketProviderHostPrefix + "SocketProviderV6";
                string hostProviderFullNameSecure = socketProviderHostPrefix + "SocketProviderV6Ssl";

                // Get the certificate reader.
                Nequeo.Security.Configuration.Reader certificateReader = new Nequeo.Security.Configuration.Reader();
                Nequeo.Net.Configuration.Reader hostReader = new Nequeo.Net.Configuration.Reader();

                // Create the server endpoint.
                Nequeo.Net.Sockets.MultiEndpointModel[] model = new Nequeo.Net.Sockets.MultiEndpointModel[]
                {
                    // None secure.
                    new Nequeo.Net.Sockets.MultiEndpointModel()
                    {
                        Port = hostReader.GetServerHost(hostProviderFullName).Port,
                        Addresses = new System.Net.IPAddress[]
                        {
                            System.Net.IPAddress.IPv6Any,
                            System.Net.IPAddress.Any
                        }
                    },
                    // Secure.
                    new Nequeo.Net.Sockets.MultiEndpointModel()
                    {
                        Port = hostReader.GetServerHost(hostProviderFullNameSecure).Port,
                        Addresses = new System.Net.IPAddress[]
                        {
                            System.Net.IPAddress.IPv6Any,
                            System.Net.IPAddress.Any
                        }
                    }
                };

                // Start the server.
                _server = new Nequeo.Net.Download.ManagerServer(_basePath, model, hostReader.GetServerHost(hostProviderFullName).MaxNumClients);
                _server.Timeout = hostReader.GetServerHost(hostProviderFullName).ClientTimeOut;

                // Set the token provider.
                TokenProvider tokenProvider = new TokenProvider();
                tokenProvider.ServiceName = _server.ServiceName;
                _server.TokenProvider = tokenProvider;

                // Inititalise.
                _server.Initialisation();

                try
                {
                    // Look for the certificate information in the configuration file.

                    // Get the certificate if any.
                    X509Certificate2 serverCertificate = certificateReader.GetServerCredentials();

                    // If a certificate exists.
                    if (serverCertificate != null)
                    {
                        // Get the secure servers.
                        _server.Server[2].UseSslConnection = true;
                        _server.Server[2].X509Certificate = serverCertificate;
                        _server.Server[3].UseSslConnection = true;
                        _server.Server[3].X509Certificate = serverCertificate;
                    }
                }
                catch { }
            }
            catch (Exception)
            {
                if (_server != null)
                    _server.Dispose();

                _server = null;
                throw;
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
                    // Dispose managed resources.
                    if (_server != null)
                        _server.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _server = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Manager()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// Token provider
    /// </summary>
    internal class TokenProvider : Nequeo.Data.ITokenProvider
    {
        /// <summary>
        /// Gets or sets the service name.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Validate the token
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <param name="serviceName">The service name.</param>
        /// <param name="token">The token.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="actionName">The unique action name.</param>
        /// <param name="state">The object state.</param>
        public async void IsValid(string uniqueIdentifier, string serviceName, string token, Action<bool, Nequeo.Security.IPermission, object> callback, string actionName = "", object state = null)
        {
            await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
            {
                return false;
            });

            // Check the credentials.
            bool allow = false;
            if (!String.IsNullOrEmpty(uniqueIdentifier) && !String.IsNullOrEmpty(serviceName) && !String.IsNullOrEmpty(token))
            {
                // Make sure the credentials match.
                if (uniqueIdentifier.ToLower().Equals("drazen") && serviceName.ToLower().Equals(ServiceName.ToLower()) && token.ToLower().Equals("allow"))
                {
                    // Allow access.
                    allow = true;
                }
            }

            // Send the result and permission to the client.
            if (callback != null)
                callback(allow,
                    new Nequeo.Security.PermissionSource(
                        Nequeo.Security.PermissionType.Download |
                        Nequeo.Security.PermissionType.Upload |
                        Nequeo.Security.PermissionType.List |
                        Nequeo.Security.PermissionType.Delete |
                        Nequeo.Security.PermissionType.Move |
                        Nequeo.Security.PermissionType.Rename |
                        Nequeo.Security.PermissionType.Create |
                        Nequeo.Security.PermissionType.Copy),
                        state);
        }

        #region Private Action Members
        /// <summary>
        /// 
        /// </summary>
        public Action<string, Exception> ExceptionCallback
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Commit()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Load()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="serviceName"></param>
        /// <param name="callback"></param>
        /// <param name="actionName"></param>
        /// <param name="state"></param>
        public void Create(string uniqueIdentifier, string serviceName, Action<string, object> callback, string actionName = "", object state = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="serviceName"></param>
        /// <param name="callback"></param>
        /// <param name="actionName"></param>
        /// <param name="state"></param>
        public void Delete(string uniqueIdentifier, string serviceName, Action<bool, object> callback, string actionName = "", object state = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="serviceName"></param>
        /// <param name="callback"></param>
        /// <param name="actionName"></param>
        /// <param name="state"></param>
        public void Get(string uniqueIdentifier, string serviceName, Action<string, Nequeo.Security.IPermission, object> callback, string actionName = "", object state = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="serviceName"></param>
        /// <param name="token"></param>
        /// <param name="callback"></param>
        /// <param name="actionName"></param>
        /// <param name="state"></param>
        public void Update(string uniqueIdentifier, string serviceName, string token, Action<bool, object> callback, string actionName = "", object state = null)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
