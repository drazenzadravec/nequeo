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
using System.Collections;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace Nequeo.Net.Webdav
{
    /// <summary>
    /// Class contains properties that hold all the
    /// connection collection for the specified Web DAV.
    /// </summary>
    [Serializable]
    public class WebdavConnectionAdapter
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public WebdavConnectionAdapter()
        {
        }

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="webdavServer">The Web Dav server (e.g. https://webdav.company.com/) "</param>
        /// <param name="userName">The username for the connection.</param>
        /// <param name="password">The password for the connection.</param>
        /// <param name="isAnonymousUser">Is the connection to the Web Dav server anonymous.</param>
        /// <param name="timeOut">The number of milliseconds to wait for a request.</param>
        /// <param name="useSSLConnection">Should the connection use ssl encryption.</param>
        public WebdavConnectionAdapter(string webdavServer, string userName,
            string password, bool isAnonymousUser = false, int timeOut = -1, 
            bool useSSLConnection = false)
        {
            _webdavServer = webdavServer;
            _userName = userName;
            _password = password;
            _isAnonymousUser = isAnonymousUser;
            _timeOut = timeOut;
            _useSSLConnection = useSSLConnection;

            if (useSSLConnection)
                _port = 443;
            else
                _port = 80;
        }
        #endregion

        #region Private Fields
        private bool _isAnonymousUser = false;
        private string _userName = string.Empty;
        private string _password = string.Empty;
        private string _webdavServer = string.Empty;
        private string _domain = string.Empty;
        private bool _useSSLConnection = false;
        private bool _sslCertificateOverride = true;
        private int _timeOut = -1;
        private int _port = 80;
        private IWebProxy _proxy = null;
        private X509Certificate2 _clientCertificate = null;
        private int _responseTimeout = 30000;
        private int _requestTimeout = 30000;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get Set, the response time out.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public int ResponseTimeout
        {
            get { return _responseTimeout; }
            set { _responseTimeout = value; }
        }

        /// <summary>
        /// Get Set, the request time out.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public int RequestTimeout
        {
            get { return _requestTimeout; }
            set { _requestTimeout = value; }
        }

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
        /// Get set, the web client proxy.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public IWebProxy Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        /// <summary>
        /// Get Set, the anonymous user indicator.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public bool IsAnonymousUser
        {
            get { return _isAnonymousUser; }
            set { _isAnonymousUser = value; }
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
        /// Get Set, the domain.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string Domain
        {
            get { return _domain; }
            set { _domain = value; }
        }

        /// <summary>
        /// Get Set, the web dav server (e.g. https://webdav.company.com/).
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string WebdavServer
        {
            get { return _webdavServer; }
            set { _webdavServer = value; }
        }

        /// <summary>
        /// Get Set, the web dav port.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        /// Get Set, the time out.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public int TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }

        /// <summary>
        /// Get Set, use ssl encryption transfer.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public bool UseSSLConnection
        {
            get { return _useSSLConnection; }
            set { _useSSLConnection = value; }
        }

        /// <summary>
        /// Get Set, override server certificate validation to
        /// always true no matter if the certificate is not valid.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public bool SslCertificateOverride
        {
            get { return _sslCertificateOverride; }
            set { _sslCertificateOverride = value; }
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
    }
}
