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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Reflection;
using System.Runtime.Remoting;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;

namespace Nequeo.Net.Http
{
    /// <summary>
    /// Used to over ride the ClientValidation method
    /// hehavior, when set the ClientValidation method default behavior is over written.
    /// </summary>
    /// <param name="user">Defines the basic functionality of a principal object.</param>
    /// <param name="authenticationSchemes">Specifies protocols for authentication.</param>
    /// <returns>True if the client has been validated; else false.</returns>
    public delegate Boolean ClientAuthenticationValidator(System.Security.Principal.IPrincipal user, AuthenticationSchemes authenticationSchemes);

    /// <summary>
    /// Http listener base uses the base System.Net.HttpListener provider for http context.
    /// </summary>
    public abstract class HttpListenerBase : Nequeo.Net.IInteractionContext, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Http listener base uses the base System.Net.HttpListener provider for http context.
        /// </summary>
        protected HttpListenerBase()
        {
        }

        /// <summary>
        /// Http listener base uses the base System.Net.HttpListener provider for http context.
        /// </summary>
        /// <param name="uriList">The url prefix listening list.</param>
        protected HttpListenerBase(string[] uriList)
        {
            _uriList = uriList;
        }
        #endregion

        #region Private Fields
        private int READ_BUFFER_SIZE = 8192;
        private int WRITE_BUFFER_SIZE = 8192;

        private AuthenticationSchemes _authenticationSchemes = AuthenticationSchemes.Anonymous;
        private AuthenticationSchemeSelector _authenticationSchemeSelectorDelegate = null;
        private ClientAuthenticationValidator _clientAuthenticationValidator = null;

        private int _timeout = -1;
        private long _headerTimeout = -1;
        private int _requestTimeout = -1;
        private int _responseTimeout = -1;

        private string _serverName = string.Empty;
        private string _serviceName = string.Empty;

        private int _maxReadLength = 0;
        private int _requestBufferCapacity = 10000000;
        private int _responseBufferCapacity = 10000000;

        private bool _running = false;
        private System.Net.HttpListener _listener = null;
        private string[] _uriList = null;

        private ConcurrentQueue<Nequeo.Net.Provider.SendToContainer> _queue = null;
        private Action<HttpListenerContext> _httpContextHandler = null;

        private Nequeo.Net.IMemberContextManager _timeoutManager = null;
        private Nequeo.Net.IMemberContextManager _memberContextManager = null;
        private Nequeo.Net.IInteractionContext _integrationContext = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the read buffer size.
        /// </summary>
        public int ReadBufferSize
        {
            get { return READ_BUFFER_SIZE; }
            set { READ_BUFFER_SIZE = value; }
        }

        /// <summary>
        /// Gets sets, the write buffer size.
        /// </summary>
        public int WriteBufferSize
        {
            get { return WRITE_BUFFER_SIZE; }
            set { WRITE_BUFFER_SIZE = value; }
        }

        /// <summary>
        /// Gets or sets the member context timeout manager.
        /// </summary>
        public Nequeo.Net.IMemberContextManager TimeoutManager
        {
            get { return _timeoutManager; }
            set { _timeoutManager = value; }
        }

        /// <summary>
        /// Gets or sets the member context manager.
        /// </summary>
        public Nequeo.Net.IMemberContextManager MemberContextManager
        {
            get { return _memberContextManager; }
            set { _memberContextManager = value; }
        }

        /// <summary>
        /// Gets or sets the interact context.
        /// </summary>
        public virtual Nequeo.Net.IInteractionContext IntegrationContext
        {
            get { return _integrationContext; }
            set { _integrationContext = value; }
        }

        /// <summary>
        /// Gets or sets the Uniform Resource Identifier (URI) prefixes handled by this System.Net.HttpListener object.
        /// </summary>
        public string[] UriList
        {
            get { return _uriList; }
            set { _uriList = value; }
        }

        /// <summary>
        /// Gets a value indicating if the server is listening to incomming connections.
        /// </summary>
        public bool IsListening
        {
            get { return _running; }
        }

        /// <summary>
        /// Gets or sets the timeout (minutes) for each client connection when in-active.
        /// Disconnects the client when this time out is triggered.
        /// </summary>
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of bytes to read. This is used when reading initial headers,
        /// this prevents an infinite read of data. This is a DOS security measure.
        /// </summary>
        public int MaximumReadLength
        {
            get { return _maxReadLength; }
            set { _maxReadLength = value; }
        }

        /// <summary>
        /// Gets or sets the maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely.
        /// </summary>
        public long HeaderTimeout
        {
            get { return _headerTimeout; }
            set { _headerTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the length of time, in milliseconds, before the request times out; -1 wait indefinitely.
        /// </summary>
        public int RequestTimeout
        {
            get { return _requestTimeout; }
            set { _requestTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the length of time, in milliseconds, before the response times out; -1 wait indefinitely.
        /// </summary>
        public int ResponseTimeout
        {
            get { return _responseTimeout; }
            set { _responseTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the current server name.
        /// </summary>
        public string Name
        {
            get { return _serverName; }
            set { _serverName = value; }
        }

        /// <summary>
        /// Gets or sets the common service name.
        /// </summary>
        public string ServiceName
        {
            get { return _serviceName; }
            set { _serviceName = value; }
        }

        /// <summary>
        /// Gets or sets then maximum request buffer capacity when using buffered stream.
        /// </summary>
        public int RequestBufferCapacity
        {
            get { return _requestBufferCapacity; }
            set { _requestBufferCapacity = value; }
        }

        /// <summary>
        /// Gets or sets then maximum response buffer capacity when using buffered stream.
        /// </summary>
        public int ResponseBufferCapacity
        {
            get { return _responseBufferCapacity; }
            set { _responseBufferCapacity = value; }
        }

        /// <summary>
        /// Gets or sets the http context handler, used to receive new connections.
        /// </summary>
        protected Action<HttpListenerContext> HttpContext
        {
            get { return _httpContextHandler; }
            set { _httpContextHandler = value; }
        }
        #endregion

        #region Protected Properties
        /// <summary>
        /// Gets sets the client authentication validator. Used to over ride the ClientValidation method
        /// hehavior, when set the ClientValidation method default behavior is over written.
        /// </summary>
        protected ClientAuthenticationValidator ClientAuthenticationValidator
        {
            get { return _clientAuthenticationValidator; }
            set { _clientAuthenticationValidator = value; }
        }

        /// <summary>
        /// Gets or sets the authentication schemes
        /// </summary>
        /// <remarks>
        /// The default value is System.Net.AuthenticationSchemes.Anonymous.
        /// </remarks>
        protected AuthenticationSchemes AuthenticationSchemes
        {
            get { return _authenticationSchemes; }
            set { _authenticationSchemes = value; }
        }

        /// <summary>
        /// Gets or sets the scheme used to authenticate clients.
        /// </summary>
        /// <remarks>
        /// The default value is Null.
        /// </remarks>
        protected AuthenticationSchemeSelector AuthenticationSchemeSelector
        {
            get { return _authenticationSchemeSelectorDelegate; }
            set { _authenticationSchemeSelectorDelegate = value; }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Validate the client. Client is only validated if IsAuthenticated = true; all other is false.
        /// Use the ClientAuthenticationValidator delegate to over ride this behavior. Or override this
        /// method in a derived class to change the behavior
        /// </summary>
        /// <param name="user">Defines the basic functionality of a principal object.</param>
        /// <param name="authenticationSchemes">Specifies protocols for authentication.</param>
        /// <returns>True if the client has been validated; else false.</returns>
        protected virtual Boolean ClientValidation(System.Security.Principal.IPrincipal user, AuthenticationSchemes authenticationSchemes)
        {
            if (_clientAuthenticationValidator != null)
            {
                // Custom client authentication valiador.
                return _clientAuthenticationValidator(user, authenticationSchemes);
            }
            else
            {
                // Does the user priciple exist.
                if (user != null)
                {
                    // Does the user identity exist.
                    if (user.Identity != null)
                    {
                        // If the client was validated.
                        if (!user.Identity.IsAuthenticated)
                            return false;
                        else
                            return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Add the member to the member manager.
        /// </summary>
        /// <param name="context">The member context to add.</param>
        protected virtual void AddMember(Nequeo.Net.IMemberContext context)
        {
            // Remove the client.
            if (_timeoutManager != null)
                _timeoutManager.Add(context);

            if (_memberContextManager != null)
                _memberContextManager.Add(context);
        }

        /// <summary>
        /// Remove the member from the member manager.
        /// </summary>
        /// <param name="context">The member context to remove.</param>
        protected virtual void RemoveMember(Nequeo.Net.IMemberContext context)
        {
            // Remove the client.
            if (_timeoutManager != null)
                _timeoutManager.Remove(context);

            if (_memberContextManager != null)
                _memberContextManager.Remove(context);
        }

        /// <summary>
        /// Parse all header data.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="resource">Get the request or response with the supplied data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        protected virtual List<Nequeo.Model.NameValue> ParseHeaders(System.IO.Stream input, out string resource, long timeout = -1, int maxReadLength = 0)
        {
            return Nequeo.Net.Utility.ParseHeaders(input, out resource, timeout, maxReadLength);
        }

        /// <summary>
        /// Parse all header data only, no initial resources data is present.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        protected virtual List<Nequeo.Model.NameValue> ParseHeadersOnly(byte[] input, long timeout = -1, int maxReadLength = 0)
        {
            return Nequeo.Net.Utility.ParseHeadersOnly(input, timeout, maxReadLength);
        }

        /// <summary>
        /// Parse the input data until then end of the data is found (\r\n cariage return line feed).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The bytes of data; else null.</returns>
        protected virtual byte[] ParseCRLF(byte[] input, long timeout = -1, int maxReadLength = 0)
        {
            return Nequeo.Net.Utility.ParseCRLF(input, timeout, maxReadLength);
        }

        /// <summary>
        /// Parse the input data until then end of the data is found (\r\n\r\n cariage return line feed repeat once).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The bytes of data; else null.</returns>
        protected virtual byte[] Parse2CRLF(byte[] input, long timeout = -1, int maxReadLength = 0)
        {
            return Nequeo.Net.Utility.Parse2CRLF(input, timeout, maxReadLength);
        }

        /// <summary>
        /// Parse all header data only, no initial resources data is present.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        protected virtual List<Nequeo.Model.NameValue> ParseHeadersOnly(System.IO.Stream input, long timeout = -1, int maxReadLength = 0)
        {
            return Nequeo.Net.Utility.ParseHeadersOnly(input, timeout, maxReadLength);
        }

        /// <summary>
        /// Parse the input data until then end of the data is found (\r\n cariage return line feed).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The bytes of data; else null.</returns>
        protected virtual byte[] ParseCRLF(System.IO.Stream input, long timeout = -1, int maxReadLength = 0)
        {
            return Nequeo.Net.Utility.ParseCRLF(input, timeout, maxReadLength);
        }

        /// <summary>
        /// Parse the input data until then end of the data is found (\r\n\r\n cariage return line feed repeat once).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The bytes of data; else null.</returns>
        protected virtual byte[] Parse2CRLF(System.IO.Stream input, long timeout = -1, int maxReadLength = 0)
        {
            return Nequeo.Net.Utility.Parse2CRLF(input, timeout, maxReadLength);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start the http listener.
        /// </summary>
        public async void Start()
        {
            // If not running.
            if (!_running)
            {
                try
                {
                    // Create a new http listener
                    if (_listener == null)
                    {
                        _listener = new System.Net.HttpListener();

                        // Add URI prefixes to listen for.
                        foreach (string uri in _uriList)
                            _listener.Prefixes.Add(uri);
                    }

                    // Set the Authentication Schemes.
                    _listener.AuthenticationSchemes = _authenticationSchemes;
                    if (_authenticationSchemeSelectorDelegate != null)
                        _listener.AuthenticationSchemeSelectorDelegate = _authenticationSchemeSelectorDelegate;

                    // Start the listener
                    _listener.Start();
                    _running = true;

                    // Keep the service in the running start
                    // listen for in-comming requests.
                    while (_running)
                    {
                        // Wait for the next request.
                        HttpListenerContext listenerContext = await _listener.GetContextAsync();

                        // Send the http context to the handler.
                        if (listenerContext != null)
                            AsynchronousListener(listenerContext);
                    }
                }
                catch (Exception)
                {
                    if (_listener != null)
                        _listener.Close();
                }
            }
        }

        /// <summary>
        /// Stop the http listener.
        /// </summary>
        public void Stop()
        {
            // If not running.
            if (_running)
            {
                try
                {
                    // Stop the service.
                    _running = false;

                    if (_listener != null)
                        _listener.Stop();
                }
                catch (Exception)
                {
                    if (_listener != null)
                        _listener.Close();

                    throw;
                }
            }
        }

        /// <summary>
        /// Shuts down the System.Net.HttpListener object immediately, discarding all currently queued requests.
        /// </summary>
        public void Abort()
        {
            // If not running.
            if (_running)
            {
                try
                {
                    // Stop the service.
                    _running = false;

                    if (_listener != null)
                        _listener.Abort();
                }
                catch (Exception)
                {
                    if (_listener != null)
                        _listener.Close();

                    throw;
                }
            }
        }

        /// <summary>
        /// Shuts down the System.Net.HttpListener.
        /// </summary>
        public void Close()
        {
            // If not running.
            if (_running)
            {
                try
                {
                    // Stop the service.
                    _running = false;

                    if (_listener != null)
                        _listener.Close();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Send the data to the clients connected to this system.
        /// </summary>
        /// <param name="data">The data to send to each server context client.</param>
        /// <remarks>This method does not send the data to the current context client, only
        /// to all the specified clients through the corresponding server context connections.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual void SendToClients(byte[] data)
        {
            if (_memberContextManager != null)
            {
                try
                {
                    // Find all the connected clients.
                    Nequeo.Net.IMemberContext[] clients = _memberContextManager.FindAllMember();

                    // Send the data to each client on this server.
                    SendToClients(data, clients);
                }
                catch { }
            }
        }

        /// <summary>
        /// Send the data to the clients connected to this system.
        /// </summary>
        /// <param name="data">The data to send to each server context client.</param>
        /// <param name="memberContexts">The collection of member context clients.</param>
        /// <remarks>This method does not send the data to the current context client, only
        /// to all the specified clients through the corresponding server context connections.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual void SendToClients(byte[] data, Nequeo.Net.IMemberContext[] memberContexts)
        {
            // Make sure references exists.
            if (memberContexts == null || data == null)
                return;

            try
            {
                // Send the data.
                SendDataToClients(data, memberContexts);
            }
            catch { }
        }

        /// <summary>
        /// Send the data to the clients connected to this system.
        /// </summary>
        /// <param name="data">The data to send to each server context client.</param>
        /// <param name="uniqueIdentifiers">The collection of clients.</param>
        /// <returns>True if all the unique identifiers exist on this server; else false.</returns>
        public virtual bool SendToClients(byte[] data, string[] uniqueIdentifiers)
        {
            if (_memberContextManager != null)
            {
                try
                {
                    // Find all the clients on the current server.
                    Nequeo.Net.IMemberContext[] clients = _memberContextManager.FindMember(uniqueIdentifiers);
                    if (clients != null && clients.Length > 0)
                    {
                        // Send the data to each client on this server.
                        SendToClients(data, clients);

                        // All clients exist on this server.
                        if (clients.Length == uniqueIdentifiers.Length)
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
                catch { return false; }
            }
            else
                return false;
        }

        /// <summary>
        /// Send the data to the client connected to this system.
        /// </summary>
        /// <param name="data">The data to send to each server context client.</param>
        /// <param name="uniqueIdentifier">The client to send the data to.</param>
        /// <returns>True if all the unique identifiers exist on this server; else false.</returns>
        public virtual bool SendToClient(byte[] data, string uniqueIdentifier)
        {
            if (_memberContextManager != null)
            {
                try
                {
                    // Find all the clients on the current server.
                    Nequeo.Net.IMemberContext[] clients = _memberContextManager.FindMember(new string[] { uniqueIdentifier });
                    if (clients != null && clients.Length > 0)
                    {
                        // Send the data to each client on this server.
                        SendToClients(data, clients);

                        // All clients exist on this server.
                        if (clients.Length == 1)
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
                catch { return false; }
            }
            else
                return false;
        }

        /// <summary>
        /// Send data to all identities and maintain an active connection (receivers that are on hosts).
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier that is sending the data.</param>
        /// <param name="serviceName">The service name the unique identifier is connected to.</param>
        /// <param name="serviceNameUniqueIdentifiers">The service name the unique identifiers are connected to.</param>
        /// <param name="uniqueIdentifiers">The list of unique identities to send the data to.</param>
        /// <param name="data">The data to send to the receivers.</param>
        public virtual void SendToReceivers(string uniqueIdentifier, string serviceName, string serviceNameUniqueIdentifiers, string[] uniqueIdentifiers, byte[] data)
        {
            // Send to clients on other machines.
            if (_integrationContext != null)
                _integrationContext.SendToReceivers(uniqueIdentifier, serviceName, serviceNameUniqueIdentifiers, uniqueIdentifiers, data);
        }

        /// <summary>
        /// Send data to all identities and maintain an active connection (receivers that are on hosts). 
        /// Hosts and ports are included for an open channel to the remote nost.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier that is sending the data.</param>
        /// <param name="serviceName">The service name the unique identifier is connected to.</param>
        /// <param name="serviceNameUniqueIdentifiers">The service name the unique identifiers are connected to.</param>
        /// <param name="uniqueIdentifiers">The list of unique identities to send the data to.</param>
        /// <param name="data">The data to send to the receivers.</param>
        /// <param name="hosts">The remote hosts that receivers (unique identities) are connected to.</param>
        /// <param name="ports">The remote host ports, each port must have a matching host.</param>
        public virtual void SendToReceivers(string uniqueIdentifier, string serviceName, string serviceNameUniqueIdentifiers, string[] uniqueIdentifiers, byte[] data, string[] hosts, string[] ports)
        {
            // Send to clients on other machines.
            if (_integrationContext != null)
                _integrationContext.SendToReceivers(uniqueIdentifier, serviceName, serviceNameUniqueIdentifiers, uniqueIdentifiers, data, hosts, ports);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Asynchronous listener callback result.
        /// </summary>
        /// <param name="context">The current http context</param>
        private void AsynchronousListener(HttpListenerContext context)
        {
            HttpListenerResponse response = null;

            try
            {
                // If the context is not null.
                if (context != null)
                    response = context.Response;

                // Send the context to the contect handler.
                if (_httpContextHandler != null)
                    _httpContextHandler(context);
            }
            catch (Exception)
            {
                try
                {
                    if (response != null)
                    {
                        // Get the response OutputStream and write the response to it.
                        response.ContentLength64 = 0;
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        response.StatusDescription = "Internal Server Error";
                        response.Close();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Send the data to the server context clients.
        /// </summary>
        /// <param name="data">The data to send to each server context client.</param>
        /// <param name="memberContexts">The collection of member context clients.</param>
        private void SendDataToClients(byte[] data, Nequeo.Net.IMemberContext[] memberContexts)
        {
            try
            {
                // Create a new queue.
                if (_queue == null)
                    _queue = new ConcurrentQueue<Nequeo.Net.Provider.SendToContainer>();

                // Create the container.
                Nequeo.Net.Provider.SendToContainer container = new Nequeo.Net.Provider.SendToContainer()
                {
                    Data = data,
                    MemberContexts = memberContexts
                };

                // Queue an new container.
                _queue.Enqueue(container);

                // Send the data to the server context clients.
                SendDataToClientsAsync();
            }
            catch { }
        }

        /// <summary>
        /// Send the data to the server context clients.
        /// </summary>
        private async void SendDataToClientsAsync()
        {
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        // Send to each server context the specified data.
                        SendDataToClientsParallel();
                    }
                    catch { }
                });
        }

        /// <summary>
        /// Send to each server context the specified data.
        /// </summary>
        private void SendDataToClientsParallel()
        {
            // Error message.
            StringBuilder errorMessage = new StringBuilder();

            // Get the next item in the queue.
            Nequeo.Net.Provider.SendToContainer container = null;
            while (_queue.TryDequeue(out container))
            {
                // Get the data and list.
                byte[] data = container.Data;
                Nequeo.Net.IMemberContext[] memberContexts = container.MemberContexts;

                // If conenction exist.
                if (memberContexts != null)
                {
                    // If more than zero connections exist.
                    if (memberContexts.Count() > 0)
                    {
                        // Start a new parallel operation for each connection.
                        Parallel.ForEach<Nequeo.Net.IMemberContext>(memberContexts, u =>
                        {
                            try
                            {
                                // Make sure the current object has not be disposed.
                                if (u != null)
                                {
                                    if (data.Length > 0)
                                    {
                                        // Send the data to the current composite service.
                                        u.SentFromServer(data);

                                        // Get the current error from the server.
                                        Exception exception = u.GetLastError();
                                        if (exception != null)
                                            errorMessage.Append(exception.Message + "\r\n");
                                    }
                                }
                            }
                            catch { }
                        });
                    }
                }
            }
        }
        #endregion

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

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
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_listener != null)
                        _listener.Close();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _listener = null;
                _queue = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~HttpListenerBase()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
