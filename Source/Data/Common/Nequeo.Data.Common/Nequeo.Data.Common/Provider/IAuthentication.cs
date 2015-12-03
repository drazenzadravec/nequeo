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
using System.Threading.Tasks;

using Nequeo.Data.Enum;

namespace Nequeo.Data.Provider
{
    /// <summary>
    /// Authentication provider interface.
    /// </summary>
    public interface IAuthentication
    {
        /// <summary>
        /// Add a new contact for this client.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="uniqueContactIdentifier">The unique client contact identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void AddClientContact(string uniqueIdentifier, string uniqueContactIdentifier, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Find a person that could potentally be a contact.
        /// </summary>
        /// <param name="query">The query data to search for.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void FindPerson(string query, Action<string[], object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get all the client contacts.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetClientContacts(string uniqueIdentifier, Action<string[], object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get all the client contacts that are online and available.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetClientContactsOnline(string uniqueIdentifier, Action<string[], object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get the client online and available status.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetClientOnlineStatus(string uniqueIdentifier, Action<OnlineStatus, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Is the client contact online and available.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="uniqueContactIdentifier">The unique client contact identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void IsClientContactOnline(string uniqueIdentifier, string uniqueContactIdentifier, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Is the client currently suspended, which means that the credentials are valid but the client can not log-in.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void IsClientSuspended(string uniqueIdentifier, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Set the client suspended state.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="suspended">True if suspended, false if not suspended.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void SetClientSuspendedState(string uniqueIdentifier, bool suspended, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Remove a contact from this client.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="uniqueContactIdentifier">The unique client contact identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void RemoveClientContact(string uniqueIdentifier, string uniqueContactIdentifier, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Set the online status of the client.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="status">The status of the client.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void SetClientOnlineStatus(string uniqueIdentifier, OnlineStatus status, Action<bool, object> callback, string actionName = "", object state = null);

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
        void ValidateUser(string username, string password, Action<string, object> callback, string domain = null, string applicationName = "All", string actionName = "", object state = null);

        /// <summary>
        /// Is the client currently logged-on.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void IsClientLoggedOn(string uniqueIdentifier, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Set the client logged-on state.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="loggedOn">True if logged-on, false if logged-off.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void SetClientLoggedOnState(string uniqueIdentifier, bool loggedOn, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Does the client exist.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void ClientExists(string uniqueIdentifier, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Set the client name.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="name">The client full name.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void SetClientName(string uniqueIdentifier, string name, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Set the client email address.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="emailAddress">The client email address.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void SetClientEmailAddress(string uniqueIdentifier, string emailAddress, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Set the client username.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="username">The client username.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void SetClientUsername(string uniqueIdentifier, string username, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Set the client password.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="password">The client password.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void SetClientPassword(string uniqueIdentifier, string password, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Set the client active connections count for the same client.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="activeConnections">The client active connections count.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void SetClientActiveConnections(string uniqueIdentifier, int activeConnections, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Set the client application name.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="applicationName">The client application name (All - indicates all applications).</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void SetClientApplicationName(string uniqueIdentifier, Action<bool, object> callback, string applicationName = "All", string actionName = "", object state = null);

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
        void AddClient(string name, string emailAddress, string username, string password, Action<bool, object> callback, string applicationName = "All", string[] clientContacts = null, string actionName = "", object state = null);

        /// <summary>
        /// Remove an existing client.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void RemoveClient(string uniqueIdentifier, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get the client name.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetClientName(string uniqueIdentifier, Action<string, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get the client email address.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetClientEmailAddress(string uniqueIdentifier, Action<string, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get the client username.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetClientUsername(string uniqueIdentifier, Action<string, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get the client application name.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetClientApplicationName(string uniqueIdentifier, Action<string, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get the client active connections count.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetClientActiveConnections(string uniqueIdentifier, Action<int, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Logoff the client.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void LogOff(string uniqueIdentifier, Action<int, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get all the clients.
        /// </summary>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetClients(Action<string[], object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get all the clients online.
        /// </summary>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetClientsOnline(Action<string[], object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get all the client contacts that are online and logged on.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetClientContactsLoggedOn(string uniqueIdentifier, Action<string[], object> callback, string actionName = "", object state = null);

    }
}
