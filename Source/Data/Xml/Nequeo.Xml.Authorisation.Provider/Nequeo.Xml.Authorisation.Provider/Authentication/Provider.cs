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

namespace Nequeo.Xml.Authorisation.Authentication
{
    /// <summary>
    /// Xml authentication provider.
    /// </summary>
    public class Provider : IAuthenticationProvider, Nequeo.Security.IAuthorisationProvider
    {
        /// <summary>
        /// Xml authentication provider.
        /// </summary>
        public Provider()
        {
        }

        /// <summary>
        /// Xml authentication provider.
        /// </summary>
        /// <param name="authenticationXmlPath">The authentication xml file path.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public Provider(string authenticationXmlPath)
        {
            if (String.IsNullOrEmpty(authenticationXmlPath)) throw new ArgumentNullException("authenticationXmlPath");

            // Load the authentication data.
            AuthenticationReader.AuthenticationXmlPath = authenticationXmlPath;
        }

        private Action<string, Exception> _callback_Exception = null;
        private Nequeo.Xml.Authorisation.Authentication.Data.context _authenticationData = null;

        /// <summary>
        /// Gets or sets the error callbak handler.
        /// </summary>
        public Action<string, Exception> ExceptionCallback
        {
            get { return _callback_Exception; }
            set { _callback_Exception = value; }
        }

        /// <summary>
        /// Authenticate user credentials.
        /// </summary>
        /// <param name="userCredentials">The user credentials.</param>
        /// <returns>True if authenticated; else false.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual bool AuthenticateUser(Security.UserCredentials userCredentials)
        {
            string ret = null;
            AutoResetEvent eventHandler = new AutoResetEvent(false);

            try
            {
                // Load provider data from the store.
                if(_authenticationData == null)
                    Load();

                ValidateUser(
                    userCredentials.Username,
                    userCredentials.Password,
                    (uid, state) =>
                    {
                        ret = uid;
                        eventHandler.Set();
                    },
                    userCredentials.Domain,
                    userCredentials.ApplicationName);

                // Wait for 30 seconds for the task to complete.
                eventHandler.WaitOne(30000);
            }
            catch { }
            finally
            {
                if (eventHandler != null)
                    eventHandler.Dispose();
            }

            // Get the result.
            if (String.IsNullOrEmpty(ret))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Commit provider data to the store.
        /// </summary>
        public void Commit()
        {
            // Save the data to the store.
            AuthenticationReader.SaveAuthenticationData(_authenticationData);
        }

        /// <summary>
        /// Load provider data from the store.
        /// </summary>
        public void Load()
        {
            // Load the authentication data.
            _authenticationData = AuthenticationReader.LoadAuthenticationData();
        }

        /// <summary>
        /// Add a new contact for this client.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="uniqueClientContactIdentifier">The unique client contact identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void AddClientContact(string uniqueClientIdentifier, string uniqueClientContactIdentifier, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return AuthenticationReader.AddClientContact(uniqueClientIdentifier, uniqueClientContactIdentifier, _authenticationData);
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
                    _callback_Exception("AddClientContact", exec);
            }
            if (callback != null)
                callback(authenticationData, state);
        }

        /// <summary>
        /// Find a person that could potentally be a contact.
        /// </summary>
        /// <param name="query">The query data to search for.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void FindPerson(string query, Action<string[], object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string[]>(() =>
                {
                    try
                    {
                        return AuthenticationReader.FindPerson(query, _authenticationData);
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
                    _callback_Exception("FindPerson", exec);
            }
            if (callback != null)
                callback(authenticationData, state);
        }

        /// <summary>
        /// Get all the client contacts.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetClientContacts(string uniqueClientIdentifier, Action<string[], object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string[]>(() =>
                {
                    try
                    {
                        return AuthenticationReader.GetClientContacts(uniqueClientIdentifier, _authenticationData);
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
                    _callback_Exception("GetClientContacts", exec);
            }
            if (callback != null)
                callback(authenticationData, state);
        }

        /// <summary>
        /// Get all the client contacts that are online and available.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetClientContactsOnline(string uniqueClientIdentifier, Action<string[], object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string[]>(() =>
                {
                    try
                    {
                        return AuthenticationReader.GetClientContactsOnline(uniqueClientIdentifier, _authenticationData);
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
                    _callback_Exception("GetClientContactsOnline", exec);
            }
            if (callback != null)
                callback(authenticationData, state);
        }

        /// <summary>
        /// Get all the client contacts that are online and logged on.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetClientContactsLoggedOn(string uniqueClientIdentifier, Action<string[], object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string[]>(() =>
                {
                    try
                    {
                        return AuthenticationReader.GetClientContactsLoggedOn(uniqueClientIdentifier, _authenticationData);
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
                    _callback_Exception("GetClientContactsLoggedOn", exec);
            }
            if (callback != null)
                callback(authenticationData, state);
        }

        /// <summary>
        /// Get the client online and available status.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetClientOnlineStatus(string uniqueClientIdentifier, Action<OnlineStatus, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<OnlineStatus>(() =>
                {
                    try
                    {
                        return AuthenticationReader.GetClientOnlineStatus(uniqueClientIdentifier, _authenticationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return OnlineStatus.Invisible;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("GetClientOnlineStatus", exec);
            }
            if (callback != null)
                callback(authenticationData, state);
        }

        /// <summary>
        /// Is the client contact online and available.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="uniqueClientContactIdentifier">The unique client contact identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void IsClientContactOnline(string uniqueClientIdentifier, string uniqueClientContactIdentifier, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return AuthenticationReader.IsClientContactOnline(uniqueClientIdentifier, uniqueClientContactIdentifier, _authenticationData);
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
                    _callback_Exception("IsClientContactOnline", exec);
            }
            if (callback != null)
                callback(authenticationData, state);
        }

        /// <summary>
        /// Is the client currently suspended, which means that the credentials are valid but the client can not log-in.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void IsClientSuspended(string uniqueClientIdentifier, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return AuthenticationReader.IsClientSuspended(uniqueClientIdentifier, _authenticationData);
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
                    _callback_Exception("IsClientSuspended", exec);
            }
            if (callback != null)
                callback(authenticationData, state);
        }

        /// <summary>
        /// Set the client suspended state.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="suspended">True if suspended, false if not suspended.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void SetClientSuspendedState(string uniqueClientIdentifier, bool suspended, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        AuthenticationReader.SetClientSuspendedState(uniqueClientIdentifier, suspended, _authenticationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("SetClientSuspendedState", exec);
            }
            if (callback != null)
                callback(true, state);
        }

        /// <summary>
        /// Remove a contact from this client.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="uniqueClientContactIdentifier">The unique client contact identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void RemoveClientContact(string uniqueClientIdentifier, string uniqueClientContactIdentifier, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return AuthenticationReader.RemoveClientContact(uniqueClientIdentifier, uniqueClientContactIdentifier, _authenticationData);
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
                    _callback_Exception("RemoveClientContact", exec);
            }
            if (callback != null)
                callback(authenticationData, state);
        }

        /// <summary>
        /// Set the online status of the client.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="status">The status of the client.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void SetClientOnlineStatus(string uniqueClientIdentifier, OnlineStatus status, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        AuthenticationReader.SetClientOnlineStatus(uniqueClientIdentifier, status, _authenticationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("SetClientOnlineStatus", exec);
            }
            if (callback != null)
                callback(true, state);
        }

        /// <summary>
        /// Valiadate the user credentials.
        /// </summary>
        /// <param name="username">The client user name.</param>
        /// <param name="password">The client password.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="domain">The client domain.</param>
        /// <param name="applicationName">The application name the client belongs to.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void ValidateUser(string username, string password, Action<string, object> callback, string domain = null, string applicationName = "All", string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string>(() =>
                {
                    try
                    {
                        return AuthenticationReader.ValidateUser(username, password, applicationName, domain, _authenticationData);
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
                    _callback_Exception("ValidateUser", exec);
            }
            if (callback != null)
                callback(authenticationData, state);
        }

        /// <summary>
        /// Is the client currently logged-on.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void IsClientLoggedOn(string uniqueClientIdentifier, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return AuthenticationReader.IsClientLoggedOn(uniqueClientIdentifier, _authenticationData);
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
                    _callback_Exception("IsClientLoggedOn", exec);
            }
            if (callback != null)
                callback(authenticationData, state);
        }

        /// <summary>
        /// Set the client logged-on state.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="loggedOn">True if logged-on, false if logged-off.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void SetClientLoggedOnState(string uniqueClientIdentifier, bool loggedOn, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        AuthenticationReader.SetClientLoggedOnState(uniqueClientIdentifier, loggedOn, _authenticationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("SetClientLoggedOnState", exec);
            }
            if (callback != null)
                callback(true, state);
        }

        /// <summary>
        /// Does the client exist.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void ClientExists(string uniqueClientIdentifier, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return AuthenticationReader.ClientExists(uniqueClientIdentifier, _authenticationData);
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
                callback(authenticationData, state);
        }

        /// <summary>
        /// Set the client name.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="name">The client full name.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void SetClientName(string uniqueClientIdentifier, string name, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        AuthenticationReader.SetClientName(uniqueClientIdentifier, name, _authenticationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("SetClientName", exec);
            }
            if (callback != null)
                callback(true, state);
        }

        /// <summary>
        /// Set the client email address.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="emailAddress">The client email address.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void SetClientEmailAddress(string uniqueClientIdentifier, string emailAddress, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        AuthenticationReader.SetClientEmailAddress(uniqueClientIdentifier, emailAddress, _authenticationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("SetClientEmailAddress", exec);
            }
            if (callback != null)
                callback(true, state);
        }

        /// <summary>
        /// Set the client username.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="username">The client username.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void SetClientUsername(string uniqueClientIdentifier, string username, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        AuthenticationReader.SetClientUsername(uniqueClientIdentifier, username, _authenticationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("SetClientUsername", exec);
            }
            if (callback != null)
                callback(true, state);
        }

        /// <summary>
        /// Set the client password.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="password">The client password.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void SetClientPassword(string uniqueClientIdentifier, string password, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        AuthenticationReader.SetClientPassword(uniqueClientIdentifier, password, _authenticationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("SetClientPassword", exec);
            }
            if (callback != null)
                callback(true, state);
        }

        /// <summary>
        /// Set the client active connections count for the same client.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="activeConnections">The client active connections count.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void SetClientActiveConnections(string uniqueClientIdentifier, int activeConnections, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        AuthenticationReader.SetClientActiveConnections(uniqueClientIdentifier, activeConnections, _authenticationData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("SetClientActiveConnections", exec);
            }
            if (callback != null)
                callback(true, state);
        }

        /// <summary>
        /// Set the client application name.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="applicationName">The client application name (All - indicates all applications).</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void SetClientApplicationName(string uniqueClientIdentifier, Action<bool, object> callback, string applicationName = "All", string actionName = "", object state = null)
        {
            Exception exec = null;
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        AuthenticationReader.SetClientApplicationName(uniqueClientIdentifier, _authenticationData, applicationName);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("SetClientApplicationName", exec);
            }
            if (callback != null)
                callback(true, state);
        }

        /// <summary>
        /// Add a new client.
        /// </summary>
        /// <param name="name">The full client name.</param>
        /// <param name="emailAddress">The client email address.</param>
        /// <param name="username">The logon username.</param>
        /// <param name="password">The password username.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="applicationName">The application name the client will belong to (All - indicates all applications).</param>
        /// <param name="clientContacts">The client contact list, unique identifiers.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void AddClient(string name, string emailAddress, string username, string password, Action<bool, object> callback, string applicationName = "All", string[] clientContacts = null, string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return AuthenticationReader.AddClient(name, emailAddress, username, password, _authenticationData, applicationName, clientContacts);
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
                callback(authenticationData, state);
        }

        /// <summary>
        /// Remove an existing client.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void RemoveClient(string uniqueClientIdentifier, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return AuthenticationReader.RemoveClient(uniqueClientIdentifier, _authenticationData);
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
                callback(authenticationData, state);
        }

        /// <summary>
        /// Get the client name.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetClientName(string uniqueClientIdentifier, Action<string, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null; 
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string>(() =>
                {
                    try
                    {
                        return AuthenticationReader.GetClientName(uniqueClientIdentifier, _authenticationData);
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
                    _callback_Exception("GetClientName", exec);
            }
            if (callback != null)
                callback(authenticationData, state);
        }

        /// <summary>
        /// Get the client email address.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetClientEmailAddress(string uniqueClientIdentifier, Action<string, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string>(() =>
                {
                    try
                    {
                        return AuthenticationReader.GetClientEmailAddress(uniqueClientIdentifier, _authenticationData);
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
                    _callback_Exception("GetClientEmailAddress", exec);
            }
            if (callback != null)
                callback(authenticationData, state);
        }

        /// <summary>
        /// Get the client username.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetClientUsername(string uniqueClientIdentifier, Action<string, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string>(() =>
                {
                    try
                    {
                        return AuthenticationReader.GetClientUsername(uniqueClientIdentifier, _authenticationData);
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
                    _callback_Exception("GetClientUsername", exec);
            }
            if (callback != null)
                callback(authenticationData, state);
        }

        /// <summary>
        /// Get the client application name.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetClientApplicationName(string uniqueClientIdentifier, Action<string, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null; 
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string>(() =>
                {
                    try
                    {
                        return AuthenticationReader.GetClientApplicationName(uniqueClientIdentifier, _authenticationData);
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
                    _callback_Exception("GetClientApplicationName", exec);
            }
            if (callback != null)
                callback(authenticationData, state);
        }

        /// <summary>
        /// Get the client active connections count.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetClientActiveConnections(string uniqueClientIdentifier, Action<int, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<int>(() =>
                {
                    try
                    {
                        return AuthenticationReader.GetClientActiveConnections(uniqueClientIdentifier, _authenticationData);
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
                    _callback_Exception("GetClientActiveConnections", exec);
            }
            if (callback != null)
                callback(authenticationData, state);
        }

        /// <summary>
        /// Logoff the client.
        /// </summary>
        /// <param name="uniqueClientIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void LogOff(string uniqueClientIdentifier, Action<int, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<int>(() =>
                {
                    try
                    {
                        return AuthenticationReader.LogOff(uniqueClientIdentifier, _authenticationData);
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
                    _callback_Exception("LogOff", exec);
            }
            if (callback != null)
                callback(authenticationData, state);
        }

        /// <summary>
        /// Get all the clients.
        /// </summary>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetClients(Action<string[], object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string[]>(() =>
                {
                    try
                    {
                        return AuthenticationReader.GetClients(_authenticationData);
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
                    _callback_Exception("GetClients", exec);
            }
            if (callback != null)
                callback(authenticationData, state);
        }

        /// <summary>
        /// Get all the clients online.
        /// </summary>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void GetClientsOnline(Action<string[], object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var authenticationData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string[]>(() =>
                {
                    try
                    {
                        return AuthenticationReader.GetClientsOnline(_authenticationData);
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
                    _callback_Exception("GetClientsOnline", exec);
            }
            if (callback != null)
                callback(authenticationData, state);
        }
    }
}
