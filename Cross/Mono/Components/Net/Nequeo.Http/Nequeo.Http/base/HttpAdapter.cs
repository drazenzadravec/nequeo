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
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;

namespace Nequeo.Net.Http
{
    /// <summary>
    /// Class contains properties that hold all the
    /// connection collection for the specified web server.
    /// </summary>
    [Serializable]
    public class WebConnectionAdapter : IDisposable
    {
        #region Constructors
        /// <summary>
        /// The web connection adapter for emailing information.
        /// </summary>
        /// <param name="httpHost">The web server base host uri.</param>
        /// <param name="port">The web server port.</param>
        /// <param name="userName">The web account username.</param>
        /// <param name="password">The web account password.</param>
        /// <param name="domain">The web account domain.</param>
        /// <param name="timeOut">The time out for connection (-1 for infinity).</param>
        /// <param name="useSSLConnection">Use a secure socket layer connection.</param>
        /// <param name="authentication">The authentication type to use for connection.</param>
        public WebConnectionAdapter(string httpHost, int port, string userName, string password,
            string domain, int timeOut, bool useSSLConnection,
            Nequeo.Net.Http.AuthenticationType authentication = Nequeo.Net.Http.AuthenticationType.Basic)
        {
            _httpHost = httpHost;
            _port = port;
            _userName = userName;
            _password = password;
            _domain = domain;
            _timeOut = timeOut;
            _useSSLConnection = useSSLConnection;
            _authentication = authentication;
        }

        /// <summary>
        /// The web connection adapter for emailing information.
        /// </summary>
        /// <param name="httpHost">The web server base host uri.</param>
        /// <param name="userName">The web account username.</param>
        /// <param name="password">The web account password.</param>
        /// <param name="domain">The web account domain.</param>
        /// <param name="timeOut">The time out for connection (-1 for infinity).</param>
        /// <param name="useSSLConnection">Use a secure socket layer connection.</param>
        public WebConnectionAdapter(string httpHost, string userName,
            string password, string domain = "", int timeOut = -1, bool useSSLConnection = false)
        {
            _httpHost = httpHost;
            _userName = userName;
            _password = password;
            _domain = domain;
            _timeOut = timeOut;
            _useSSLConnection = useSSLConnection;
        }

        /// <summary>
        /// The web connection adapter for emailing information.
        /// </summary>
        /// <param name="httpHost">The web server base host uri.</param>
        /// <param name="userName">The web account username.</param>
        /// <param name="password">The web account password.</param>
        /// <param name="domain">The web account domain.</param>
        /// <param name="timeOut">The time out for connection (-1 for infinity).</param>
        public WebConnectionAdapter(string httpHost, string userName,
            string password, string domain = "", int timeOut = -1)
        {
            _httpHost = httpHost;
            _userName = userName;
            _password = password;
            _domain = domain;
            _timeOut = timeOut;
        }

        /// <summary>
        /// The web connection adapter for emailing information.
        /// </summary>
        /// <param name="httpHost">The web server base host uri.</param>
        /// <param name="userName">The web account username.</param>
        /// <param name="password">The web account password.</param>
        /// <param name="domain">The web account domain.</param>
        public WebConnectionAdapter(string httpHost, string userName,
            string password, string domain = "")
        {
            _httpHost = httpHost;
            _userName = userName;
            _password = password;
            _domain = domain;
        }

        /// <summary>
        /// The web connection adapter for emailing information.
        /// </summary>
        /// <param name="httpHost">The web server base host uri.</param>
        /// <param name="userName">The web account username.</param>
        /// <param name="password">The web account password.</param>
        public WebConnectionAdapter(string httpHost, string userName,
            string password)
        {
            _httpHost = httpHost;
            _userName = userName;
            _password = password;
        }

        /// <summary>
        /// The web connection adapter for emailing information.
        /// </summary>
        public WebConnectionAdapter()
        {
        }
        #endregion

        #region Private Fields
        private string _userName = string.Empty;
        private string _password = string.Empty;
        private string _httpHost = string.Empty;
        private string _domain = string.Empty;
        private int _port = 80;
        private int _timeOut = -1;
        private bool _useSSLConnection = false;
        private bool _disposed = false;
        private bool _validateCertificate = false;
        private Nequeo.Net.Http.AuthenticationType _authentication =
             Nequeo.Net.Http.AuthenticationType.None;
        private X509Certificate2 _clientCertificate = null;
        private bool _encryptWithClientCertificate = false;
        private IWebProxy _proxy = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get set, the client certificate.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public X509Certificate2 ClientCertificate
        {
            get { return _clientCertificate; }
            set { _clientCertificate = value; }
        }

        /// <summary>
        /// Get set, should the data that is sent be encrypted 
        /// and decrypted using the client certificate.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public bool EncryptWithClientCertificate
        {
            get { return _encryptWithClientCertificate; }
            set { _encryptWithClientCertificate = value; }
        }

        /// <summary>
        /// Get set, the web client proxy.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public IWebProxy Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        /// <summary>
        /// Get set, the authentication type for the connection.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public Nequeo.Net.Http.AuthenticationType Authentication
        {
            get { return _authentication; }
            set { _authentication = value; }
        }

        /// <summary>
        /// Get set, should the ssl/tsl certificate be veryfied
        /// when making a secure connection.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public bool ValidateCertificate
        {
            get { return _validateCertificate; }
            set { _validateCertificate = value; }
        }

        /// <summary>
        /// Get Set, the user name.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        /// <summary>
        /// Get Set, the password.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        /// <summary>
        /// Get Set, the web http host.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string HttpHost
        {
            get { return _httpHost; }
            set { _httpHost = value; }
        }

        /// <summary>
        /// Get Set, the domain.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string Domain
        {
            get { return _domain; }
            set { _domain = value; }
        }

        /// <summary>
        /// Get Set, the web port.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        /// Get Set, the time out request.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public int TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }

        /// <summary>
        /// Get Set, use ssl encryption transfer.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public bool UseSSLConnection
        {
            get { return _useSSLConnection; }
            set { _useSSLConnection = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Load the client certificate.
        /// </summary>
        /// <param name="fileName">The path to the certificate.</param>
        public void LoadClientCertificate(string fileName)
        {
            _clientCertificate = (X509Certificate2)X509Certificate2.CreateFromCertFile(fileName);
        }

        /// <summary>
        /// Load the client certificate.
        /// </summary>
        /// <param name="storeName">The store name to search in.</param>
        /// <param name="storeLocation">The store location to search in.</param>
        /// <param name="x509FindType">The type of data to find on.</param>
        /// <param name="findValue">The value to find in the certificate.</param>
        /// <param name="validOnly">Search for only valid certificates.</param>
        public void LoadClientCertificate(
            StoreName storeName,
            StoreLocation storeLocation, 
            X509FindType x509FindType,
            object findValue, 
            bool validOnly)
        {
            _clientCertificate = Nequeo.Security.X509Certificate2Store.GetCertificate(
                storeName, storeLocation, x509FindType, findValue, validOnly);
        }
        #endregion

        #region Dispose Object Methods
        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
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
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _userName = null;
                _password = null;
                _httpHost = null;
                _domain = null;

                // Note disposing has been done.
                _disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~WebConnectionAdapter()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// This enum holds the authentication type.
    /// </summary>
    public enum AuthenticationType
    {
        /// <summary>
        /// No authentication.
        /// </summary>
        None = -1,
        /// <summary>
        /// Anonymous type authentication.
        /// </summary>
        Anonymous = 0,
        /// <summary>
        /// Integrated type authentication.
        /// </summary>
        Integrated = 1,
        /// <summary>
        /// SQL type authentication.
        /// </summary>
        SQL = 2,
        /// <summary>
        /// Basic type authentication.
        /// </summary>
        Basic = 3,
        /// <summary>
        /// Digest type authentication.
        /// </summary>
        Digest = 4,
        /// <summary>
        /// NTLM type authentication.
        /// </summary>
        NTLM = 5,
        /// <summary>
        /// DotNetPassport type authentication.
        /// </summary>
        DotNetPassport = 6,
        /// <summary>
        /// Kerberos type authentication.
        /// </summary>
        Kerberos = 7
    }
}
