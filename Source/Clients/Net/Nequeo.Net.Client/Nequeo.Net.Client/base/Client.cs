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

namespace Nequeo.Net
{
    /// <summary>
    /// Generic net client.
    /// </summary>
    public class Client : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Generic net client.
        /// </summary>
        /// <param name="address">An IP address.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="isSecureConnection">Is the connection secure.</param>
        /// <param name="validateSslCertificate">Validate the x.509 certificate..</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public Client(IPAddress address, int port, bool isSecureConnection = false, bool validateSslCertificate = false)
        {
            _isHttpProtocol = false;
            _connectionLevel = 1;
            _port = port;
            _address = address;
            _isSecureConnection = isSecureConnection;
            _validateSslCertificate = validateSslCertificate;
            Initialise();
        }

        /// <summary>
        /// Generic net client.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="isSecureConnection">Is the connection secure.</param>
        /// <param name="validateSslCertificate">Validate the x.509 certificate..</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public Client(string hostNameOrAddress, int port, bool isSecureConnection = false, bool validateSslCertificate = false)
        {
            _isHttpProtocol = false;
            _connectionLevel = 0;
            _port = port;
            _hostNameOrAddress = hostNameOrAddress;
            _isSecureConnection = isSecureConnection;
            _validateSslCertificate = validateSslCertificate;
            Initialise();
        }

        /// <summary>
        /// Generic net client.
        /// </summary>
        /// <param name="requestUri">The request uri.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="isSecureConnection">Is the connection secure.</param>
        /// <param name="validateSslCertificate">Validate the x.509 certificate..</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public Client(Uri requestUri, int port = -1, bool isSecureConnection = false, bool validateSslCertificate = false)
        {
            _connectionLevel = 2;
            _requestUri = requestUri;
            _port = port;
            _isSecureConnection = isSecureConnection;
            _validateSslCertificate = validateSslCertificate;
            Initialise();
        }
        #endregion

        #region Private Fields
        private object _lockConnectObject = new object();
        private object _lockRequestObject = new object();

        private Nequeo.Net.NetClient _client = null;
        private int _connectionLevel = 0;

        private IPAddress _address = null;
        private int _port = -1;
        private string _hostNameOrAddress = null;
        private Uri _requestUri = null;
        private bool _isSecureConnection = false;
        private bool _validateSslCertificate = false;
        private bool _isHttpProtocol = true;
        private RemoteCertificateValidationCallback _certificateValidationCallback = null;

        private Dictionary<string, object> _callback = new Dictionary<string, object>();
        private Dictionary<string, object> _state = new Dictionary<string, object>();
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the base net client.
        /// </summary>
        public Nequeo.Net.NetClient NetClient
        {
            get { return _client; }
        }

        /// <summary>
        /// Gets, a value that indicates whether a connection to a remote host exits.
        /// </summary>
        public bool Connected
        {
            get 
            {
                if (_client != null)
                    return _client.Connected;
                else
                    return false;
            }
        }

        /// <summary>
        /// Gets or sets, is this request a http protocol request (uses headers).
        /// </summary>
        public bool IsHttpProtocol
        {
            get { return _isHttpProtocol; }
            set { _isHttpProtocol = value; }
        }

        /// <summary>
        /// Gets or sets, verifies the remote Secure Sockets Layer (SSL) certificate used for authentication.
        /// </summary>
        public RemoteCertificateValidationCallback RemoteCertificateValidationCallback
        {
            get { return _certificateValidationCallback; }
            set { _certificateValidationCallback = value; }
        }
        #endregion

        #region Public Events
        /// <summary>
        /// The on error event handler, triggered when data received from the server is any type of error.
        /// </summary>
        public event Nequeo.Threading.EventHandler<string, string, string> OnError;

        #endregion

        #region Public Methods
        /// <summary>
        /// Transfer the data.
        /// </summary>
        /// <param name="request">The request (the request output stream must be set).</param>
        /// <param name="input">The stream containing the data to send (can be null).</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        public virtual void Transfer(Nequeo.Net.NetRequest request, System.IO.Stream input, CancellationToken cancellationToken)
        {
            if (_client == null)
                throw new Exception("A connection does not exist.");

            // If not connected.
            if (!_client.Connected)
                _client.Connect();

            // If connected.
            if (_client.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Is http protocol (uses headers).
                    if (_isHttpProtocol)
                    {
                        // If no host has been set.
                        if (String.IsNullOrEmpty(request.Host))
                            request.Host = _client.HostNameOrAddress + ":" + _client.Port.ToString();

                        // If no path has been set.
                        if (String.IsNullOrEmpty(request.Path))
                            request.Path = (_requestUri != null ? _requestUri.PathAndQuery : "/");
                        else
                        {
                            // If path is at root.
                            if (request.Path.Equals("/"))
                                request.Path = (_requestUri != null ? _requestUri.PathAndQuery : "/");
                        }

                        // Write the headers.
                        request.WriteNetRequestHeaders();
                    }

                    // If data exists.
                    if (input != null)
                    {
                        // Write data.
                        if (input.Length > 0 && request.ContentLength > 0)
                        {
                            // Send the data.
                            Nequeo.IO.Stream.Operation.CopyStream(input, request.Output, cancellationToken, request.ContentLength, _client.RequestTimeout, _client.WriteBufferSize);
                        }
                        else
                        {
                            // If one or the other is zero.
                            if(input.Length == 0 || request.ContentLength == 0)
                                throw new Exception("The request content length is zero or the input length is zero.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Transfer the data.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="input">The stream containing the data to send (can be null).</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="state">The state callback handler object.</param>
        public virtual void Transfer(Nequeo.Net.NetRequest request, System.IO.Stream input,
            CancellationToken cancellationToken, Action<Nequeo.Net.NetContext, object> callback, object state = null)
        {
            if (_client == null)
                throw new Exception("A connection does not exist.");

            // If not connected.
            if (!_client.Connected)
                _client.Connect();

            // If connected.
            if (_client.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback["Transfer"] = callback;
                    _state["Transfer"] = state;

                    // Is http protocol (uses headers).
                    if (_isHttpProtocol)
                    {
                        // If no host has been set.
                        if (String.IsNullOrEmpty(request.Host))
                            request.Host = _client.HostNameOrAddress + ":" + _client.Port.ToString();

                        // If no path has been set.
                        if (String.IsNullOrEmpty(request.Path))
                            request.Path = (_requestUri != null ? _requestUri.PathAndQuery : "/");
                        else
                        {
                            // If path is at root.
                            if (request.Path.Equals("/"))
                                request.Path = (_requestUri != null ? _requestUri.PathAndQuery : "/");
                        }

                        // Write the headers.
                        request.WriteNetRequestHeaders();
                    }

                    // If data exists.
                    if (input != null)
                    {
                        // Write data.
                        if (input.Length > 0 && request.ContentLength > 0)
                        {
                            // Send the data.
                            Nequeo.IO.Stream.Operation.CopyStream(input, request.Output, cancellationToken, request.ContentLength, _client.RequestTimeout, _client.WriteBufferSize);
                        }
                        else
                        {
                            // If one or the other is zero.
                            if (input.Length == 0 || request.ContentLength == 0)
                                throw new Exception("The request content length is zero or the input length is zero.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Close the current socket connection and disposes of all resources.
        /// </summary>
        public virtual void Close()
        {
            if (_client != null)
                _client.Close();
        }

        /// <summary>
        /// Begin the secure negotiation and server authentication.
        /// </summary>
        /// <param name="continueNegotiation">Continue the negotiation handler. After sending the command, wait for a 
        /// response. If the response is correct then continue the negotiation process.</param>
        /// <param name="tlsNegotiationCommand">Send a TLS negotiation command (e.g. STARTTLS) if not null.</param>
        /// <returns>True if authentication has started; else false.</returns>
        /// <remarks>This is generally used for TLS protcol.</remarks>
        public virtual bool BeginTlsNegotiation(Func<bool> continueNegotiation, string tlsNegotiationCommand = "STARTTLS\r\n")
        {
            if (_client != null)
            {
                // If not connected.
                if (!_client.Connected)
                    _client.Connect();

                // If connected.
                if (_client.Connected)
                {
                    // Begin the secure negotiation and server authentication.
                    return _client.BeginTlsNegotiation(continueNegotiation, tlsNegotiationCommand);
                }
                else
                    throw new Exception("A connection does not exist.");
            }
            else
                throw new Exception("A connection does not exist.");
        }

        /// <summary>
        /// Certificate validator.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="certificate">The certificate</param>
        /// <param name="chain">The certificate chain.</param>
        /// <param name="sslPolicyErrors">The policy error.</param>
        /// <returns>True if the certificate is valid else false.</returns>
        public virtual bool CertificateValidation(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // If a validated exists.
            if (_certificateValidationCallback != null)
            {
                // Certificate validator.
                if (_client != null)
                    return _client.CertificateValidation(sender, certificate, chain, sslPolicyErrors);
                else
                    return true;
            }
            else
                return true;
        }

        /// <summary>
        /// Get the current web context.
        /// </summary>
        /// <returns>The current web context.</returns>
        public virtual NetContext GetContext()
        {
            if (_client != null)
            {
                // If not connected.
                if (!_client.Connected)
                    _client.Connect();

                // If connected.
                if (_client.Connected)
                {
                    return _client.GetContext();
                }
                else
                    throw new Exception("A connection does not exist.");
            }
            else
                throw new Exception("A connection does not exist.");
        }

        /// <summary>
        /// Get a new web request stream.
        /// </summary>
        /// <returns>The web request stream.</returns>
        public virtual NetRequest GetRequest()
        {
            if (_client != null)
            {
                // If not connected.
                if (!_client.Connected)
                    _client.Connect();

                // If connected.
                if (_client.Connected)
                {
                    return _client.GetRequest();
                }
                else
                    throw new Exception("A connection does not exist.");
            }
            else
                throw new Exception("A connection does not exist.");
        }

        /// <summary>
        /// Get a new web response stream.
        /// </summary>
        /// <returns>The web response stream.</returns>
        public virtual NetResponse GetResponse()
        {
            if (_client != null)
            {
                // If not connected.
                if (!_client.Connected)
                    _client.Connect();

                // If connected.
                if (_client.Connected)
                {
                    return _client.GetResponse();
                }
                else
                    throw new Exception("A connection does not exist.");
            }
            else
                throw new Exception("A connection does not exist.");
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Process a http request.
        /// </summary>
        /// <param name="serviceUri">The service name to call.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="method">The request method.</param>
        /// <param name="input">The stream containing the data to send (can be null).</param>
        /// <param name="credentials">The request network credentials.</param>
        public static void HttpRequestAsync(Uri serviceUri,
            Action<Nequeo.Net.NetContext, object> callback, string method = "GET",
            System.IO.Stream input = null, NetworkCredential credentials = null)
        {
            Nequeo.Net.HttpDataClient.RequestAsync(serviceUri, callback, method, input, credentials);
        }

        /// <summary>
        /// Process a http request.
        /// </summary>
        /// <param name="serviceUri">The service name to call.</param>
        /// <param name="request">The request provider used to send the data.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="input">The stream containing the data to send (can be null).</param>
        public static void HttpRequestAsync(Uri serviceUri, Nequeo.Net.NetRequest request,
            Action<Nequeo.Net.NetContext, object> callback, System.IO.Stream input = null)
        {
            Nequeo.Net.HttpDataClient.RequestAsync(serviceUri, request, callback, input);
        }

        /// <summary>
        /// Process a http response.
        /// </summary>
        /// <param name="context">The client context response.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <returns>The array of bytes; else null.</returns>
        public static byte[] HttpResponseAsync(Nequeo.Net.NetContext context, long timeout = -1)
        {
            return Nequeo.Net.HttpDataClient.ResponseAsync(context, timeout);
        }

        /// <summary>
        /// Process a http response.
        /// </summary>
        /// <param name="context">The client context response.</param>
        /// <param name="headerList">The array of response headers.</param>
        /// <param name="response">The response message.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <returns>The array of bytes; else null.</returns>
        public static byte[] HttpResponseAsync(Nequeo.Net.NetContext context,
            out Model.NameValue[] headerList, out Nequeo.Model.Message.ResponseResource response,
            long timeout = -1)
        {
            return Nequeo.Net.HttpDataClient.ResponseAsync(context, out headerList, out response, timeout);
        }

        /// <summary>
        /// Process a general data request.
        /// </summary>
        /// <param name="serviceUri">The service name to call.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="input">The stream containing the data to send (can be null).</param>
        /// <param name="byteLength">The total number of bytes that need to be read (must be the same as the number of input bytes).</param>
        /// <param name="isSecure">True if the connection is secure.</param>
        public static void RequestAsync(Uri serviceUri, Action<Nequeo.Net.NetContext, object> callback, 
            System.IO.Stream input, long byteLength, bool isSecure = false)
        {
            // Open a new connection.
            Nequeo.Net.Client client = new Client(serviceUri, isSecureConnection: isSecure);
            client.IsHttpProtocol = false;

            // Create a new request.
            Nequeo.Net.NetRequest request = client.GetRequest();
            request.ContentLength = byteLength;

            // Send the data.
            client.Transfer(request, input, CancellationToken.None, callback, client);
        }

        /// <summary>
        /// Process a general data response.
        /// </summary>
        /// <param name="context">The client context response.</param>
        /// <param name="byteLength">The total number of bytes that need to be read (must be the same as the number of source bytes). Waits until all bytes are read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <returns>The array of bytes; else null.</returns>
        public static byte[] ResponseAsync(Nequeo.Net.NetContext context, long byteLength, long timeout = -1)
        {
            byte[] data = null;

            // Open a stream.
            using (MemoryStream stream = new MemoryStream())
            {
                // Copy the response stream.
                Nequeo.IO.Stream.Operation.CopyStream(context.NetResponse.Input, stream, byteLength, timeout);

                // Create the object from the data.
                data = stream.ToArray();
            }

            // Data.
            return data;
        }

        /// <summary>
        /// Create the URI from the parameters.
        /// </summary>
        /// <param name="serviceRoot">The service root.</param>
        /// <param name="serviceEntityName">The service entity name.</param>
        /// <returns>Return the constructed Uri.</returns>
        public static Uri CreateUri(string serviceRoot, string serviceEntityName)
        {
            return CreateUri(serviceRoot, serviceEntityName, null);
        }

        /// <summary>
        /// Create the URI from the parameters.
        /// </summary>
        /// <param name="serviceRoot">The service root.</param>
        /// <param name="serviceEntityName">The service entity name.</param>
        /// <param name="queries">The array of name and value query pairs.</param>
        /// <returns>Return the constructed Uri.</returns>
        public static Uri CreateUri(string serviceRoot, string serviceEntityName, Nequeo.Model.NameObject[] queries)
        {
            string query = "";

            // If queries exist.
            if (queries != null && queries.Length > 0)
            {
                // Create the query.
                query = Nequeo.Net.Utility.CreateQueryString(queries);
            }

            // Return the URI
            return new Uri(serviceRoot.TrimEnd('/') + "/" + serviceEntityName + (String.IsNullOrEmpty(query) ? "" : query));
        }
        #endregion

        #region Private Receive Methods
        /// <summary>
        /// On net context receive handler.
        /// </summary>
        /// <param name="sender">The application sender.</param>
        /// <param name="context">The current net context.</param>
        private void ManagerClient_OnNetContext(object sender, Net.NetContext context)
        {
            try
            {
                // Call the handler.
                object callback = null;
                object state = null;
                if (_callback.TryGetValue("Transfer", out callback))
                {
                    _state.TryGetValue("Transfer", out state);
                    if (callback != null)
                    {
                        Action<Nequeo.Net.NetContext, object> callbackAction = (Action<Nequeo.Net.NetContext, object>)callback;
                        callbackAction(context, state);
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    // An internal client error.
                    AnyError("Error", "500", ex.Message);
                }
                catch { }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Send to the client any type of error.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="code">The code.</param>
        /// <param name="message">The message</param>
        private void AnyError(string command, string code, string message)
        {
            // Send any type of error.
            if (OnError != null)
                OnError(this, command, code, message);
        }

        /// <summary>
        /// Initialise.
        /// </summary>
        private void Initialise()
        {
            // Select the connection level.
            switch (_connectionLevel)
            {
                case 2:
                    // Assign the on connect action handler.
                    _client = new Nequeo.Net.NetClient(_requestUri.Host, (_port > -1 ? _port : _requestUri.Port));
                    break;
                case 1:
                    // Assign the on connect action handler.
                    _client = new Nequeo.Net.NetClient(_address, _port);
                    break;
                case 0:
                default:
                    // Assign the on connect action handler.
                    _client = new Nequeo.Net.NetClient(_hostNameOrAddress, _port);
                    break;
            }
            
            _client.Timeout = 10;
            _client.HeaderTimeout = 30000;
            _client.RequestTimeout = 30000;
            _client.ResponseTimeout = 30000;
            _client.ReadBufferSize = 32768;
            _client.WriteBufferSize = 32768;
            _client.ResponseBufferCapacity = 100000000;
            _client.RequestBufferCapacity = 100000000;
            _client.UseSslConnection = _isSecureConnection;
            _client.ValidateSslCertificate = _validateSslCertificate;
            _client.OnNetContext += ManagerClient_OnNetContext;
        }
        #endregion

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
                    if (_callback != null)
                        _callback.Clear();

                    if (_state != null)
                        _state.Clear();

                    if (_client != null)
                        _client.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _lockConnectObject = null;
                _lockRequestObject = null;

                _callback = null;
                _state = null;
                _client = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Client()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
