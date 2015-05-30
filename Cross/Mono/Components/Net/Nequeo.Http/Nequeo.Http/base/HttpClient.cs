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
using System.ComponentModel.Composition;

namespace Nequeo.Net.Http
{
    /// <summary>
    /// Encapsulates http web functions.
    /// </summary>
    [Export(typeof(IHttpClient))]
    [Export(typeof(IHttpWebClient))]
    public partial class HttpClient : Nequeo.Net.NetClient, IHttpClient, IHttpWebClient
    {
        #region Costructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public HttpClient()
        {
            OnCreated();
            Initialise();
            _connection = new WebConnectionAdapter();
        }

        /// <summary>
        /// Interact Integration client.
        /// </summary>
        /// <param name="address">An IP address.</param>
        /// <param name="port">The port to connect to.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public HttpClient(IPAddress address, int port)
            : base(address, port)
        {
            OnCreated();
            Initialise();
            _connection = new WebConnectionAdapter();
            _connection.Port = port;
            _connection.HttpHost = address.ToString();
        }

        /// <summary>
        /// Interact Integration client.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
        /// <param name="port">The port to connect to.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public HttpClient(string hostNameOrAddress, int port)
            : base(hostNameOrAddress, port)
        {
            OnCreated();
            Initialise();
            _connection = new WebConnectionAdapter();
            _connection.Port = port;
            _connection.HttpHost = hostNameOrAddress;
        }
        #endregion

        #region Private Fields
        private Nequeo.Security.X509Certificate2Info _sslCertificate = null;
        private WebConnectionAdapter _connection = null;
        
        private string _baseUri;
        private string _previousUri;
        private CookieContainer _cookieContainer = new CookieContainer();
        private string _userAgent = @"HttpReader";
        private string _accept = @"image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-excel, application/vnd.ms-powerpoint, */*";
        private Uri _requestUri;
        private string _contentType = string.Empty;
        
        private readonly Hashtable _headers = new Hashtable();
        private string _html;
        private string _location;
        private bool _sendReferer = true;
        private HttpStatusCode _statusCode;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets, the base uri address.
        /// </summary>
        public string BaseUri
        {
            get { return _baseUri; }
        }

        /// <summary>
        /// Gets, the previous uri address called.
        /// </summary>
        public string PreviousUri
        {
            get { return _previousUri; }
        }

        /// <summary>
        /// Gets sets, the cookie container for the request.
        /// </summary>
        public CookieContainer CookieContainer
        {
            get { return _cookieContainer; }
            set { _cookieContainer = value; }
        }

        /// <summary>
        ///  Gets or sets, the value of the User-agent HTTP header.
        /// </summary>
        public string UserAgent
        {
            get { return _userAgent; }
            set { _userAgent = value; }
        }

        /// <summary>
        /// Gets or sets the value of the Accept HTTP header.
        /// </summary>
        public string Accept
        {
            get { return _accept; }
            set { _accept = value; }
        }

        /// <summary>
        /// Gets, the final request uri.
        /// </summary>
        public Uri RequestUri
        {
            get { return _requestUri; }
        }

        /// <summary>
        /// Gets or sets the value of the Content-type HTTP header.
        /// </summary>
        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        /// <summary>
        /// Gets sets, the web connection adapter.
        /// </summary>
        public WebConnectionAdapter Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        /// <summary>
        /// Gets, the client credentials.
        /// </summary>
        public NetworkCredential Credentials
        {
            get
            {
                return new NetworkCredential(_connection.UserName,
                    _connection.Password, _connection.Domain);
            }
        }

        /// <summary>
        /// Gets, the secure server certificate.
        /// </summary>
        public Nequeo.Security.X509Certificate2Info Certificate
        {
            get { return _sslCertificate; }
        }

        /// <summary>
        /// Gets, the resulting html string returned for the request.
        /// </summary>
        public string Html
        {
            get { return _html; }
        }

        /// <summary>
        /// Gets, the header options for the request.
        /// </summary>
        public Hashtable Headers
        {
            get { return _headers; }
        }

        /// <summary>
        /// Gets, the current web location from the response.
        /// </summary>
        public string Location
        {
            get { return _location; }
        }

        /// <summary>
        /// Gets or sets the value of the Referer HTTP header.
        /// </summary>
        public bool SendReferer
        {
            get { return _sendReferer; }
            set { _sendReferer = value; }
        }

        /// <summary>
        /// Gets, the http status code for the request.
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get { return _statusCode; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Certificate validator.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="certificate">The certificate</param>
        /// <param name="chain">The certificate chain.</param>
        /// <param name="sslPolicyErrors">The policy error.</param>
        /// <returns>True if the certificate is valid else false.</returns>
        private bool OnCertificateValidation(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Certificate should be validated.
            if (_connection.ValidateCertificate)
            {
                // If the certificate is valid
                // return true.
                if (sslPolicyErrors == SslPolicyErrors.None)
                    return true;
                else
                {
                    // Create a new certificate collection
                    // instance and return false.
                    _sslCertificate = new Nequeo.Security.X509Certificate2Info(
                        certificate as X509Certificate2, chain, sslPolicyErrors);
                    return false;
                }
            }
            else
                // Return true.
                return true;
        }
        #endregion

        #region Internal Classes
        /// <summary>
        /// Default request process class.
        /// </summary>
        internal class DefaultRequestStreamProcessor
        {
            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="data">The data to send to the http server.</param>
            /// <param name="connection">The web connection adapter used for http connection.</param>
            public DefaultRequestStreamProcessor(string data, WebConnectionAdapter connection)
            {
                _data = data;
                _connection = connection;
            }

            private WebConnectionAdapter _connection = null;
            private readonly string _data;

            /// <summary>
            /// The request process method.
            /// </summary>
            /// <param name="stream">The stream used to write the data.</param>
            public void Process(Stream stream)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(_data);

                // If the client certificate is to be used to
                // encrypt the data before sending.
                if (_connection.EncryptWithClientCertificate)
                    bytes = Nequeo.Cryptography.AdvancedRSA.EncryptData(Encoding.ASCII.GetBytes(_data), _connection.ClientCertificate);

                stream.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// Default response process class.
        /// </summary>
        internal class DefaultResponseStreamProcessor
        {
            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="reader">The current http client.</param>
            /// <param name="connection">The web connection adapter used for http connection.</param>
            public DefaultResponseStreamProcessor(HttpClient reader, WebConnectionAdapter connection)
            {
                _reader = reader;
                _connection = connection;
            }

            private WebConnectionAdapter _connection = null;
            private readonly HttpClient _reader;

            /// <summary>
            /// The response process method.
            /// </summary>
            /// <param name="stream">The stream used to read the data.</param>
            public void Process(Stream stream)
            {
                using (StreamReader sr = new StreamReader(stream, Encoding.Default))
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(sr.ReadToEnd());

                    // If the client certificate is to be used to
                    // dencrypt the data before receiving.
                    if (_connection.EncryptWithClientCertificate)
                        bytes = Nequeo.Cryptography.AdvancedRSA.DecryptData(Encoding.ASCII.GetBytes(sr.ReadToEnd()), _connection.ClientCertificate);

                    _reader._html = Encoding.ASCII.GetString(bytes);
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initiate the http request.
        /// </summary>
        /// <param name="requestUri">The request uri.</param>
        /// <param name="method">The http request method.</param>
        /// <param name="requestStreamProcessor">The request delegate method.</param>
        /// <param name="responseStreamProcessor">The response delegate method.</param>
        /// <returns>The http status code.</returns>
        public virtual HttpStatusCode Request(string requestUri, string method,
            Nequeo.Threading.ActionHandler<Stream> requestStreamProcessor,
            Nequeo.Threading.ActionHandler<Stream> responseStreamProcessor)
        {
            // Get the base uri from the adapter
            _html = "";
            _baseUri = _connection.HttpHost;
            string uri = _baseUri;

            // If the request is not a SOAP call
            // then append the request uri to the base uri
            if (method != "SOAP")
                uri += requestUri;

            Uri requestRes = new Uri(uri);
            // Check if the URI is valid HTTP scheme.
            if ((requestRes.Scheme != Uri.UriSchemeHttp) && (requestRes.Scheme != Uri.UriSchemeHttps))
                throw new ArgumentException("Uri is not a valid HTTP scheme");

            // Create a new web request call.
            HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(uri);

            if (_connection.Proxy != null) 
                request.Proxy = _connection.Proxy;

            // If the http connection is not
            // anonymous then create a new
            // network credential.
            if (_connection.Authentication != Nequeo.Net.Http.AuthenticationType.None)
            {
                if (_connection.Authentication != Nequeo.Net.Http.AuthenticationType.Anonymous)
                    request.Credentials = Credentials;
                else
                    request.UseDefaultCredentials = true;
            }

            // Assign the request properties.
            request.CookieContainer = CookieContainer;
            request.UserAgent = UserAgent;
            request.Accept = Accept;
            request.Method = method == "SOAP" ? "POST" : method;
            request.KeepAlive = true;

            // Create a new service point manger to
            // validate the certificate.
            ServicePointManager.ServerCertificateValidationCallback = new
                RemoteCertificateValidationCallback(OnCertificateValidation);

            // If referer http header is set
            // then get the prevoius uri if null.
            if (SendReferer)
                request.Referer = PreviousUri ?? uri;

            // For each type of header request.
            foreach (string key in Headers.Keys)
                request.Headers.Add(key, Headers[key].ToString());

            // If the request method is post.
            if (method == "POST")
            {
                // Assign the content type as post back content.
                request.ContentType = "application/x-www-form-urlencoded";
                request.AllowAutoRedirect = false;
            }
            else if (method == "SOAP")
            {
                // Assign SOAP content type
                request.ContentType = "text/xml; charset=utf-8";
                request.AllowAutoRedirect = false;

                // Set the SOAP action and request uri.
                request.Headers.Add("SOAPAction", requestUri);
            }
            else
            {
                // Assign the default content type.
                request.ContentType = ContentType;
                request.AllowAutoRedirect = true;
            }

            // Set the previous uri to the current
            // and the request uri to the current.
            _previousUri = uri;
            _requestUri = request.RequestUri;

            // If a client certificate is to be used
            // add the client certificate.
            if (_connection.ClientCertificate != null)
                request.ClientCertificates.Add(_connection.ClientCertificate);

            // If a time out has been set
            if (_connection.TimeOut > -1)
                request.Timeout = _connection.TimeOut;

            // Get the request stream, initialise a new request,
            // invoke the request delegate method.
            if (requestStreamProcessor != null)
                using (Stream st = request.GetRequestStream())
                    requestStreamProcessor(st);

            // Initialise a new response stream from the request.
            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            using (Stream sm = resp.GetResponseStream())
            {
                _statusCode = resp.StatusCode;
                _location = resp.Headers["Location"];

                // Set the base uri
                if (resp.ResponseUri.AbsoluteUri.StartsWith(BaseUri) == false)
                    _baseUri = resp.ResponseUri.Scheme + "://" + resp.ResponseUri.Host;

                // Get the collection of cookies
                CookieCollection cc = request.CookieContainer.GetCookies(request.RequestUri);

                // This code fixes the situation when a server sets a cookie without the 'path'.
                // IE takes this as the root ('/') value, the HttpWebRequest class as the 
                // RequestUri.AbsolutePath value.
                foreach (Cookie c in cc)
                    if (c.Path == request.RequestUri.AbsolutePath)
                        CookieContainer.Add(new Cookie(c.Name, c.Value, "/", c.Domain));

                // Invoke the response delegate method.
                if (responseStreamProcessor != null)
                    responseStreamProcessor(sm);
            }

            // Return the http status code.
            return StatusCode;
        }

        /// <summary>
        /// Get the specified http request.
        /// </summary>
        /// <param name="requestUri">The request uri.</param>
        /// <param name="requestStreamProcessor">The request delegate method.</param>
        /// <param name="responseStreamProcessor">The response delegate method.</param>
        /// <returns>The http status code.</returns>
        public virtual HttpStatusCode Get(string requestUri,
            Nequeo.Threading.ActionHandler<Stream> requestStreamProcessor,
            Nequeo.Threading.ActionHandler<Stream> responseStreamProcessor)
        {
            return Request(requestUri, "GET", requestStreamProcessor, responseStreamProcessor);
        }

        /// <summary>
        /// Get the specified http request.
        /// </summary>
        /// <param name="requestUri">The request uri.</param>
        /// <returns>The http status code.</returns>
        public virtual HttpStatusCode Get(string requestUri)
        {
            DefaultResponseStreamProcessor rp = new DefaultResponseStreamProcessor(this, _connection);
            return Request(requestUri, "GET", null, new Nequeo.Threading.ActionHandler<Stream>(rp.Process));
        }

        /// <summary>
        /// Get the specified http request.
        /// </summary>
        /// <param name="requestUri">The request uri.</param>
        /// <param name="responseStreamProcessor">The response delegate method.</param>
        /// <returns>The http status code.</returns>
        public virtual HttpStatusCode Get(string requestUri, Nequeo.Threading.ActionHandler<Stream> responseStreamProcessor)
        {
            return Request(requestUri, "GET", null, responseStreamProcessor);
        }

        /// <summary>
        /// Post back the http data to the specified request.
        /// </summary>
        /// <param name="requestUri">The request uri.</param>
        /// <param name="postData">The http data to post back.</param>
        /// <returns>The http status code.</returns>
        public virtual HttpStatusCode Post(string requestUri, string postData)
        {
            return Post(requestUri,
                new DefaultRequestStreamProcessor(postData, _connection).Process,
                new DefaultResponseStreamProcessor(this, _connection).Process);
        }

        /// <summary>
        /// Post back the http data to the specified request.
        /// </summary>
        /// <param name="requestUri">The request uri.</param>
        /// <param name="requestStreamProcessor">The request delegate method.</param>
        /// <returns>The http status code.</returns>
        public virtual HttpStatusCode Post(string requestUri, Nequeo.Threading.ActionHandler<Stream> requestStreamProcessor)
        {
            return Post(requestUri, requestStreamProcessor,
                new DefaultResponseStreamProcessor(this, _connection).Process);
        }

        /// <summary>
        /// Post back the http data to the specified request.
        /// </summary>
        /// <param name="requestUri">The request uri.</param>
        /// <param name="postData">The http data to post back.</param>
        /// <param name="responseStreamProcessor">The response delegate method.</param>
        /// <returns>The http status code.</returns>
        public virtual HttpStatusCode Post(string requestUri, string postData,
            Nequeo.Threading.ActionHandler<Stream> responseStreamProcessor)
        {
            return Post(requestUri,
                new DefaultRequestStreamProcessor(postData, _connection).Process,
                responseStreamProcessor);
        }

        /// <summary>
        /// Post back the http data to the specified request.
        /// </summary>
        /// <param name="requestUri">The request uri.</param>
        /// <param name="requestStreamProcessor">The request delegate method.</param>
        /// <param name="responseStreamProcessor">The response delegate method.</param>
        /// <returns>The http status code.</returns>
        public virtual HttpStatusCode Post(
            string requestUri,
            Nequeo.Threading.ActionHandler<Stream> requestStreamProcessor,
            Nequeo.Threading.ActionHandler<Stream> responseStreamProcessor)
        {
            // Make an initial post to the request uri
            Request(requestUri, "POST", requestStreamProcessor, responseStreamProcessor);

            // Loop making requests until
            // the correct response is made
            // by the http server.
            for (int i = 0; i < 10; i++)
            {
                bool post = false;

                // Get the response status code
                switch (StatusCode)
                {
                    case HttpStatusCode.MultipleChoices:   // 300
                    case HttpStatusCode.MovedPermanently:  // 301
                    case HttpStatusCode.Found:             // 302
                    case HttpStatusCode.SeeOther:          // 303
                        break;

                    case HttpStatusCode.TemporaryRedirect: // 307
                        post = true;
                        break;

                    default:
                        return StatusCode;
                }

                // If a null location is sent back
                // then exit the loop.
                if (_location == null)
                    break;

                // Create the new request uri
                Uri uri = new Uri(new Uri(PreviousUri), _location);

                // Construct the new request uri for the
                // redirection post back.
                _baseUri = uri.Scheme + "://" + uri.Host;
                requestUri = uri.AbsolutePath + uri.Query;

                // If the redirection was sent back
                // then get the http request and on the
                // next loop post the data to the new http 
                // request uri.
                Request(
                    requestUri,
                    post ? "POST" : "GET",
                    post ? requestStreamProcessor : null,
                    responseStreamProcessor);
            }

            // Return the http status code.
            return StatusCode;
        }

        /// <summary>
        /// Makes a SOAP request to the base uri.
        /// </summary>
        /// <param name="soapAction">The SOAP method acction.</param>
        /// <param name="postData">The SOAP encoded data to send to the SOAP method.</param>
        /// <returns>The http status code.</returns>
        public virtual HttpStatusCode Soap(string soapAction, string postData)
        {
            return Request("\"" + soapAction + "\"", "SOAP",
                new DefaultRequestStreamProcessor(postData, _connection).Process,
                new DefaultResponseStreamProcessor(this, _connection).Process);
        }

        #endregion

        #region Public Transfer Methods
        /// <summary>
        /// Downloads a request uri to a local file.
        /// </summary>
        /// <param name="requestUri">The request uri to download.</param>
        /// <param name="destinationFile">The local destination file to copy the data to.</param>
        public virtual void DownloadFile(string requestUri, string destinationFile)
        {
            // Create the complete uri
            string uri = _baseUri + requestUri;
            Uri requestRes = new Uri(uri);

            using (WebClient request = new WebClient())
            {
                // Check if the URI is valid HTTP scheme.
                if ((requestRes.Scheme != Uri.UriSchemeHttp) && (requestRes.Scheme != Uri.UriSchemeHttps))
                    throw new ArgumentException("Uri is not a valid HTTP scheme");

                // Extract the directory path.
                string sFolderPath = System.IO.Path.GetDirectoryName(destinationFile);

                // If the directory does not exists create it.
                if (!Directory.Exists(sFolderPath))
                    Directory.CreateDirectory(sFolderPath);

                // If a proxy server has been specified.
                if (_connection.Proxy != null) 
                    request.Proxy = _connection.Proxy;

                // If the http connection is not
                // anonymous then create a new
                // network credential.
                if (_connection.Authentication != Nequeo.Net.Http.AuthenticationType.None)
                {
                    if (_connection.Authentication != Nequeo.Net.Http.AuthenticationType.Anonymous)
                        request.Credentials = Credentials;
                    else
                        request.UseDefaultCredentials = true;
                }

                // Create a new service point manger to
                // validate the certificate.
                ServicePointManager.ServerCertificateValidationCallback = new
                    RemoteCertificateValidationCallback(OnCertificateValidation);

                foreach (string key in Headers.Keys)
                    request.Headers.Add(key, Headers[key].ToString());

                // Download the request item to the file.
                request.DownloadFile(requestRes, destinationFile);
            }
        }

        /// <summary>
        /// Downloads data from the request uri.
        /// </summary>
        /// <param name="requestUri">The request uri to download.</param>
        /// <returns>The returned request uri content.</returns>
        public virtual string DownloadString(string requestUri)
        {
            // Create the complete uri
            string uri = _baseUri + requestUri;
            Uri requestRes = new Uri(uri);

            using (WebClient request = new WebClient())
            {
                // Check if the URI is valid HTTP scheme.
                if ((requestRes.Scheme != Uri.UriSchemeHttp) && (requestRes.Scheme != Uri.UriSchemeHttps))
                    throw new ArgumentException("Uri is not a valid HTTP scheme");

                // If a proxy server has been specified.
                if (_connection.Proxy != null)
                    request.Proxy = _connection.Proxy;

                // If the http connection is not
                // anonymous then create a new
                // network credential.
                if (_connection.Authentication != Nequeo.Net.Http.AuthenticationType.None)
                {
                    if (_connection.Authentication != Nequeo.Net.Http.AuthenticationType.Anonymous)
                        request.Credentials = Credentials;
                    else
                        request.UseDefaultCredentials = true;
                }

                // Create a new service point manger to
                // validate the certificate.
                ServicePointManager.ServerCertificateValidationCallback = new
                    RemoteCertificateValidationCallback(OnCertificateValidation);

                foreach (string key in Headers.Keys)
                    request.Headers.Add(key, Headers[key].ToString());

                // Download the request item to the file.
                return request.DownloadString(requestRes);
            }
        }

        /// <summary>
        /// Upload a local file to the request uri.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="sourceFile">The local source file to upload data from.</param>
        /// <returns>A System.Byte array containing the body of the response from the resource.</returns>
        public virtual byte[] UploadFile(string requestUri, string sourceFile)
        {
            return UploadFile(requestUri, sourceFile, null);
        }

        /// <summary>
        /// Upload a local file to the request uri.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="sourceFile">The local source file to upload data from.</param>
        /// <param name="method">The HTTP method used to send the file to the resource. If null, the default is POST for http.</param>
        /// <returns>A System.Byte array containing the body of the response from the resource.</returns>
        public virtual byte[] UploadFile(string requestUri, string sourceFile, string method)
        {
            // Create the complete uri
            string uri = _baseUri + requestUri;
            Uri requestRes = new Uri(uri);

            using (WebClient request = new WebClient())
            {
                // Check if the URI is valid HTTP scheme.
                if ((requestRes.Scheme != Uri.UriSchemeHttp) && (requestRes.Scheme != Uri.UriSchemeHttps))
                    throw new ArgumentException("Uri is not a valid HTTP scheme");

                // If a proxy server has been specified.
                if (_connection.Proxy != null)
                    request.Proxy = _connection.Proxy;

                // If the http connection is not
                // anonymous then create a new
                // network credential.
                if (_connection.Authentication != Nequeo.Net.Http.AuthenticationType.None)
                {
                    if (_connection.Authentication != Nequeo.Net.Http.AuthenticationType.Anonymous)
                        request.Credentials = Credentials;
                    else
                        request.UseDefaultCredentials = true;
                }

                // Create a new service point manger to
                // validate the certificate.
                ServicePointManager.ServerCertificateValidationCallback = new
                    RemoteCertificateValidationCallback(OnCertificateValidation);

                foreach (string key in Headers.Keys)
                    request.Headers.Add(key, Headers[key].ToString());

                // Upload the request item from the file.
                return request.UploadFile(requestRes, method, sourceFile);
            }
        }

        /// <summary>
        /// Uploads the specified string to the specified resource, using the POST method.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="data">The data to upload.</param>
        /// <returns>A System.String containing the response sent by the server.</returns>
        public virtual string UploadString(string requestUri, string data)
        {
            return UploadString(requestUri, data, null);
        }

        /// <summary>
        /// Uploads the specified string to the specified resource, using the POST method.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="data">The data to upload.</param>
        /// <param name="method">The HTTP method used to send the string to the resource. If null, the default is POST for http.</param>
        /// <returns>A System.String containing the response sent by the server.</returns>
        public virtual string UploadString(string requestUri, string data, string method)
        {
            // Create the complete uri
            string uri = _baseUri + requestUri;
            Uri requestRes = new Uri(uri);

            using (WebClient request = new WebClient())
            {
                // Check if the URI is valid HTTP scheme.
                if ((requestRes.Scheme != Uri.UriSchemeHttp) && (requestRes.Scheme != Uri.UriSchemeHttps))
                    throw new ArgumentException("Uri is not a valid HTTP scheme");

                // If a proxy server has been specified.
                if (_connection.Proxy != null)
                    request.Proxy = _connection.Proxy;

                // If the http connection is not
                // anonymous then create a new
                // network credential.
                if (_connection.Authentication != Nequeo.Net.Http.AuthenticationType.None)
                {
                    if (_connection.Authentication != Nequeo.Net.Http.AuthenticationType.Anonymous)
                        request.Credentials = Credentials;
                    else
                        request.UseDefaultCredentials = true;
                }

                // Create a new service point manger to
                // validate the certificate.
                ServicePointManager.ServerCertificateValidationCallback = new
                    RemoteCertificateValidationCallback(OnCertificateValidation);

                foreach (string key in Headers.Keys)
                    request.Headers.Add(key, Headers[key].ToString());

                // Upload the request item from the file.
                return request.UploadString(requestRes, method, data);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initialise.
        /// </summary>
        private void Initialise()
        {
            // Assign the on connect action handler.
            base.Timeout = 60;
            base.HeaderTimeout = 30000;
            base.RequestTimeout = 30000;
            base.ResponseTimeout = 30000;
            base.RemoteHostPrefix = "HttpClient_";
        }
        #endregion

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
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
            }
        }
        #endregion
    }

    /// <summary>
    /// Encapsulates http web functions.
    /// </summary>
    public interface IHttpClient
    {
        #region Public Properties
        /// <summary>
        /// Gets, the base uri address.
        /// </summary>
        string BaseUri { get; }

        /// <summary>
        /// Gets, the previous uri address called.
        /// </summary>
        string PreviousUri { get; }

        /// <summary>
        /// Gets sets, the cookie container for the request.
        /// </summary>
        CookieContainer CookieContainer { get; set; }

        /// <summary>
        ///  Gets or sets, the value of the User-agent HTTP header.
        /// </summary>
        string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the value of the Accept HTTP header.
        /// </summary>
        string Accept { get; set; }

        /// <summary>
        /// Gets, the final request uri.
        /// </summary>
        Uri RequestUri { get; }

        /// <summary>
        /// Gets or sets the value of the Content-type HTTP header.
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// Gets sets, the web connection adapter.
        /// </summary>
        WebConnectionAdapter Connection { get; set; }

        /// <summary>
        /// Gets, the client credentials.
        /// </summary>
        NetworkCredential Credentials { get; }

        /// <summary>
        /// Gets, the secure server certificate.
        /// </summary>
        Nequeo.Security.X509Certificate2Info Certificate { get; }

        /// <summary>
        /// Gets, the resulting html string returned for the request.
        /// </summary>
        string Html { get; }

        /// <summary>
        /// Gets, the header options for the request.
        /// </summary>
        Hashtable Headers { get; }

        /// <summary>
        /// Gets, the current web location from the response.
        /// </summary>
        string Location { get; }

        /// <summary>
        /// Gets or sets the value of the Referer HTTP header.
        /// </summary>
        bool SendReferer { get; set; }

        /// <summary>
        /// Gets, the http status code for the request.
        /// </summary>
        HttpStatusCode StatusCode { get; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initiate the http request.
        /// </summary>
        /// <param name="requestUri">The request uri.</param>
        /// <param name="method">The http request method.</param>
        /// <param name="requestStreamProcessor">The request delegate method.</param>
        /// <param name="responseStreamProcessor">The response delegate method.</param>
        /// <returns>The http status code.</returns>
        HttpStatusCode Request(string requestUri, string method,
            Nequeo.Threading.ActionHandler<Stream> requestStreamProcessor,
            Nequeo.Threading.ActionHandler<Stream> responseStreamProcessor);

        /// <summary>
        /// Get the specified http request.
        /// </summary>
        /// <param name="requestUri">The request uri.</param>
        /// <param name="requestStreamProcessor">The request delegate method.</param>
        /// <param name="responseStreamProcessor">The response delegate method.</param>
        /// <returns>The http status code.</returns>
        HttpStatusCode Get(string requestUri,
            Nequeo.Threading.ActionHandler<Stream> requestStreamProcessor,
            Nequeo.Threading.ActionHandler<Stream> responseStreamProcessor);

        /// <summary>
        /// Get the specified http request.
        /// </summary>
        /// <param name="requestUri">The request uri.</param>
        /// <returns>The http status code.</returns>
        HttpStatusCode Get(string requestUri);

        /// <summary>
        /// Get the specified http request.
        /// </summary>
        /// <param name="requestUri">The request uri.</param>
        /// <param name="responseStreamProcessor">The response delegate method.</param>
        /// <returns>The http status code.</returns>
        HttpStatusCode Get(string requestUri, Nequeo.Threading.ActionHandler<Stream> responseStreamProcessor);

        /// <summary>
        /// Post back the http data to the specified request.
        /// </summary>
        /// <param name="requestUri">The request uri.</param>
        /// <param name="postData">The http data to post back.</param>
        /// <returns>The http status code.</returns>
        HttpStatusCode Post(string requestUri, string postData);

        /// <summary>
        /// Post back the http data to the specified request.
        /// </summary>
        /// <param name="requestUri">The request uri.</param>
        /// <param name="requestStreamProcessor">The request delegate method.</param>
        /// <returns>The http status code.</returns>
        HttpStatusCode Post(string requestUri, Nequeo.Threading.ActionHandler<Stream> requestStreamProcessor);

        /// <summary>
        /// Post back the http data to the specified request.
        /// </summary>
        /// <param name="requestUri">The request uri.</param>
        /// <param name="postData">The http data to post back.</param>
        /// <param name="responseStreamProcessor">The response delegate method.</param>
        /// <returns>The http status code.</returns>
        HttpStatusCode Post(string requestUri, string postData,
            Nequeo.Threading.ActionHandler<Stream> responseStreamProcessor);

        /// <summary>
        /// Post back the http data to the specified request.
        /// </summary>
        /// <param name="requestUri">The request uri.</param>
        /// <param name="requestStreamProcessor">The request delegate method.</param>
        /// <param name="responseStreamProcessor">The response delegate method.</param>
        /// <returns>The http status code.</returns>
        HttpStatusCode Post(
            string requestUri,
            Nequeo.Threading.ActionHandler<Stream> requestStreamProcessor,
            Nequeo.Threading.ActionHandler<Stream> responseStreamProcessor);

        /// <summary>
        /// Makes a SOAP request to the base uri.
        /// </summary>
        /// <param name="soapAction">The SOAP method acction.</param>
        /// <param name="postData">The SOAP encoded data to send to the SOAP method.</param>
        /// <returns>The http status code.</returns>
        HttpStatusCode Soap(string soapAction, string postData);

        #endregion
    }

    /// <summary>
    /// Encapsulates http web functions.
    /// </summary>
    public interface IHttpWebClient
    {
        #region Public Transfer Methods
        /// <summary>
        /// Downloads a request uri to a local file.
        /// </summary>
        /// <param name="requestUri">The request uri to download.</param>
        /// <param name="destinationFile">The local destination file to copy the data to.</param>
        void DownloadFile(string requestUri, string destinationFile);

        /// <summary>
        /// Downloads data from the request uri.
        /// </summary>
        /// <param name="requestUri">The request uri to download.</param>
        /// <returns>The returned request uri content.</returns>
        string DownloadString(string requestUri);

        /// <summary>
        /// Upload a local file to the request uri.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="sourceFile">The local source file to upload data from.</param>
        /// <returns>A System.Byte array containing the body of the response from the resource.</returns>
        byte[] UploadFile(string requestUri, string sourceFile);

        /// <summary>
        /// Upload a local file to the request uri.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="sourceFile">The local source file to upload data from.</param>
        /// <param name="method">The HTTP method used to send the file to the resource. If null, the default is POST for http.</param>
        /// <returns>A System.Byte array containing the body of the response from the resource.</returns>
        byte[] UploadFile(string requestUri, string sourceFile, string method);

        /// <summary>
        /// Uploads data to the request uri.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="data">The data to upload.</param>
        /// <returns>A System.String containing the response sent by the server.</returns>
        string UploadString(string requestUri, string data);

        /// <summary>
        /// Uploads data to the request uri.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="data">The data to upload.</param>
        /// <param name="method">The HTTP method used to send the file to the resource. If null, the default is POST for http.</param>
        /// <returns>A System.String containing the response sent by the server.</returns>
        string UploadString(string requestUri, string data, string method);

        #endregion
    }
}
