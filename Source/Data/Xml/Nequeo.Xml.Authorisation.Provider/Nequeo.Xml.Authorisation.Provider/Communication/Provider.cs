/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2013 http://www.nequeo.com.au/
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
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;

using Nequeo.Data.Enum;
using Nequeo.Data;
using Nequeo.Data.Provider;
using Nequeo.Xml.Authorisation.Configuration;

namespace Nequeo.Xml.Authorisation.Communication
{
    /// <summary>
    /// Xml Communication provider.
    /// </summary>
    public class Provider : ICommunicationProvider
    {
        /// <summary>
        /// Xml Communication provider.
        /// </summary>
        public Provider()
        {
        }

        /// <summary>
        /// Xml Communication provider.
        /// </summary>
        /// <param name="communicationXmlPath">The communication xml file path.</param>
        /// <param name="clientServiceXmlPath">The client service xml file path.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public Provider(string communicationXmlPath, string clientServiceXmlPath)
        {
            if (String.IsNullOrEmpty(communicationXmlPath)) throw new ArgumentNullException("communicationXmlPath");
            if (String.IsNullOrEmpty(clientServiceXmlPath)) throw new ArgumentNullException("clientServiceXmlPath");

            // Load the Communication data.
            CommunicationReader.CommunicationXmlPath = communicationXmlPath;

            // Load the client service data.
            ClientServiceReader.ClientServiceXmlPath = clientServiceXmlPath;
        }

        private Action<string, Exception> _callback_Exception = null;
        private Nequeo.Xml.Authorisation.Communication.Data.context _communicationData = null;
        private Nequeo.Xml.Authorisation.Communication.Data.contextService _clientServiceData = null;

        /// <summary>
        /// Gets or sets the error callbak handler.
        /// </summary>
        public Action<string, Exception> ExceptionCallback
        {
            get { return _callback_Exception; }
            set { _callback_Exception = value; }
        }

        /// <summary>
        /// Commit provider data to the store.
        /// </summary>
        public void Commit()
        {
            // Save the data to the store.
            CommunicationReader.SaveCommunicationData(_communicationData);
            ClientServiceReader.SaveClientServiceData(_clientServiceData);
        }

        /// <summary>
        /// Load provider data from the store.
        /// </summary>
        public void Load()
        {
            // Load the Communication data.
            _communicationData = CommunicationReader.LoadCommunicationData();
            _clientServiceData = ClientServiceReader.LoadClientServiceData();
        }

        /// <summary>
        /// Add a new client.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="hostName">The host machine the client must connect to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="active">Is the communication currenlty active.</param>
        /// <param name="communicationToken">The common token used for communication.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void AddClient(string uniqueClientIdentifier, string serviceName, string hostName, Action<bool, object> callback, bool active = false, string communicationToken = "", string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        bool result = ClientServiceReader.AddClient(uniqueClientIdentifier, serviceName, hostName, _clientServiceData, active, communicationToken);
                        CommunicationReader.IncrementHostActiveConnections(hostName, _communicationData);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return false;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("AddClient", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Add a new host
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="index">The index number of the host.</param>
        /// <param name="domain">The domain of the host.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="type">The host type.</param>
        /// <param name="activeConnections">The number of active connection on the host.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public async void AddHost(string hostName, int index, string domain, Action<bool, object> callback, string type = "", int activeConnections = 0, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return CommunicationReader.AddHost(hostName, index, domain, type, _communicationData, activeConnections);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return false;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("AddHost", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Does the client exist.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void ClientExists(string uniqueClientIdentifier, string serviceName, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return ClientServiceReader.ClientExists(uniqueClientIdentifier, serviceName, _clientServiceData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return false;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("ClientExists", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Get the communication active state.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetClientActive(string uniqueClientIdentifier, string serviceName, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return ClientServiceReader.GetClientActive(uniqueClientIdentifier, serviceName, _clientServiceData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return false;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("GetClientActive", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Get the communication token.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetClientCommunicationToken(string uniqueClientIdentifier, string serviceName, Action<string, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string>(() =>
                {
                    try
                    {
                        return ClientServiceReader.GetClientCommunicationToken(uniqueClientIdentifier, serviceName, _clientServiceData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return null;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("GetClientCommunicationToken", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Get the client host name.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetClientHost(string uniqueClientIdentifier, string serviceName, Action<string, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string>(() =>
                {
                    try
                    {
                        return ClientServiceReader.GetClientHost(uniqueClientIdentifier, serviceName, _clientServiceData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return null;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("GetClientHost", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Get the number of active connections.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetHostActiveConnections(string hostName, Action<int, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<int>(() =>
                {
                    try
                    {
                        return CommunicationReader.GetHostActiveConnections(hostName, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return 0;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("GetHostActiveConnections", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Get the host index number.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetHostIndex(string hostName, Action<int, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<int>(() =>
                {
                    try
                    {
                        return CommunicationReader.GetHostIndex(hostName, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return 0;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("GetHostIndex", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Does the host exist.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void HostExists(string hostName, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return CommunicationReader.HostExists(hostName, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return false;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("HostExists", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Remove the client.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="hostName">The host machine the client must connect to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="activeConnections">The number of active connection using this client.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void RemoveClient(string uniqueClientIdentifier, string serviceName, string hostName, Action<bool, object> callback, int activeConnections = 0, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        bool result = ClientServiceReader.RemoveClient(uniqueClientIdentifier, serviceName, activeConnections, _clientServiceData);
                        CommunicationReader.DecrementHostActiveConnections(hostName, _communicationData);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return false;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("RemoveClient", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Remove the host.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void RemoveHost(string hostName, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return CommunicationReader.RemoveHost(hostName, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return false;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("RemoveHost", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Set the client communication active state.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="active">The communication active state.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void SetClientActive(string uniqueClientIdentifier, string serviceName, bool active, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        ClientServiceReader.SetClientActive(uniqueClientIdentifier, serviceName, active, _clientServiceData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("SetClientActive", exec);
            }
            if (callback != null)
                callback(true, state);
        }

        /// <summary>
        /// Set the communication token.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="communicationToken">The client communication token.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void SetClientCommunicationToken(string uniqueClientIdentifier, string serviceName, string communicationToken, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        ClientServiceReader.SetClientCommunicationToken(uniqueClientIdentifier, serviceName, communicationToken, _clientServiceData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("SetClientCommunicationToken", exec);
            }
            if (callback != null)
                callback(true, state);
        }

        /// <summary>
        /// Set the client host name.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void SetClientHost(string uniqueClientIdentifier, string serviceName, string hostName, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        ClientServiceReader.SetClientHost(uniqueClientIdentifier, serviceName, hostName, _clientServiceData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("SetClientHost", exec);
            }
            if (callback != null)
                callback(true, state);
        }

        /// <summary>
        /// Set the number of active connections.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="activeConnections">The number of active connections on the host.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void SetHostActiveConnections(string hostName, int activeConnections, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        CommunicationReader.SetHostActiveConnections(hostName, activeConnections, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("SetHostActiveConnections", exec);
            }
            if (callback != null)
                callback(true, state);
        }

        /// <summary>
        /// Set the host index number.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="index">The host index number.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public async void SetHostIndex(string hostName, int index, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        CommunicationReader.SetHostIndex(hostName, index, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("SetHostIndex", exec);
            }
            if (callback != null)
                callback(true, state);
        }

        /// <summary>
        /// Get the host details for the identities.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetHost(string uniqueClientIdentifier, string serviceName, Action<string, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string>(() =>
                {
                    try
                    {
                        return ClientServiceReader.GetHost(uniqueClientIdentifier, serviceName, _clientServiceData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return null;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("GetHost", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Get the host details for the identities.
        /// </summary>
        /// <param name="uniqueClientIdentifiers">The unique client identifiers.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetHosts(string[] uniqueClientIdentifiers, string serviceName, Action<string[], string[], object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            string[] communicationDataHosts = null;
            string[] communicationDataPorts = null;
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        communicationDataHosts = CommunicationReader.GetHosts(uniqueClientIdentifiers, serviceName, _communicationData, _clientServiceData);
                        communicationDataPorts = CommunicationReader.GetPorts(serviceName, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("GetHosts", exec);
            }
            if (callback != null)
                callback(communicationDataHosts, communicationDataPorts, state);
        }

        /// <summary>
        /// Set the host domain.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="domain">The host domain.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void SetHostDomain(string hostName, string domain, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        CommunicationReader.SetHostDomain(hostName, domain, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("SetHostDomain", exec);
            }
            if (callback != null)
                callback(true, state);
        }

        /// <summary>
        /// Get the host domain.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetHostDomain(string hostName, Action<string, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null; 
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string>(() =>
                {
                    try
                    {
                        return CommunicationReader.GetHostDomain(hostName, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return null;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("GetHostDomain", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Add a new port range.
        /// </summary>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="portTypeName">The list of application port names</param>
        /// <param name="portTypeNumber">The list of application port numbers.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void AddPort(string serviceName, string[] portTypeName, int[] portTypeNumber, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return CommunicationReader.AddPort(serviceName, portTypeName, portTypeNumber, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return false;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("AddPort", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Remove all the port ranges.
        /// </summary>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="portTypeName">The application port name.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void RemovePort(string serviceName, string portTypeName, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return CommunicationReader.RemovePort(serviceName, portTypeName, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return false;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("RemovePort", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Sets the port number.
        /// </summary>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="portTypeName">The application port name.</param>
        /// <param name="portTypeNumber">The application port number.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void SetPortNumber(string serviceName, string portTypeName, int portTypeNumber, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return CommunicationReader.SetPortNumber(serviceName, portTypeName, portTypeNumber, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return false;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("SetPortNumber", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Remove the port.
        /// </summary>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void RemovePort(string serviceName, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return CommunicationReader.RemovePort(serviceName, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return false;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("RemovePort", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Get the ports.
        /// </summary>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetPorts(string serviceName, Action<string[], object> callback, string actionName = "", object state = null)
        {
            Exception exec = null; 
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string[]>(() =>
                {
                    try
                    {
                        return CommunicationReader.GetPorts(serviceName, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return null;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("GetPorts", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Get the host type.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetHostType(string hostName, Action<string, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string>(() =>
                {
                    try
                    {
                        return CommunicationReader.GetHostType(hostName, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return null;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("GetHostType", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Set the host type.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="type">The host type.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void SetHostType(string hostName, string type, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        CommunicationReader.SetHostType(hostName, type, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("SetHostType", exec);
            }
            if (callback != null)
                callback(true, state);
        }

        /// <summary>
        /// Get the host manage URLs for a type.
        /// </summary>
        /// <param name="type">The host type.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetManageURLs(string type, Action<string[], object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string[]>(() =>
                {
                    try
                    {
                        return CommunicationReader.GetManageURLs(type, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return null;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("GetManageURLs", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Adds new manage URLs.
        /// </summary>
        /// <param name="type">The manage type.</param>
        /// <param name="urls">The list of service urls.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void AddManageURL(string type, string[] urls, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return CommunicationReader.AddManageURL(type, urls, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return false;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("AddManageURL", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Removes a manage URLs.
        /// </summary>
        /// <param name="type">The manage type.</param>
        /// <param name="url">The service url to remove.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void RemoveManageURL(string type, string url, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return CommunicationReader.RemoveManageURL(type, url, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return false;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("RemoveManageURL", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }

        /// <summary>
        /// Removes all manage URLs.
        /// </summary>
        /// <param name="type">The manage type.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void RemoveManageURL(string type, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var communicationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return CommunicationReader.RemoveManageURL(type, _communicationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return false;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("RemoveManageURL", exec);
            }
            if (callback != null)
                callback(communicationData, state);
        }
    }
}
