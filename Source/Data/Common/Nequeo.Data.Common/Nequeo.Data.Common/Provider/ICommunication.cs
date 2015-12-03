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
    /// Communication provider interface.
    /// </summary>
    public interface ICommunication
    {
        /// <summary>
        /// Add a new client.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="hostName">The host machine the client must connect to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="active">Is the communication currenlty active.</param>
        /// <param name="communicationToken">The common token used for communication.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void AddClient(string uniqueIdentifier, string serviceName, string hostName, Action<bool, object> callback, bool active = false, string communicationToken = "", string actionName = "", object state = null);

        /// <summary>
        /// Remove the client.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="hostName">The host machine the client must connect to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="activeConnections">The number of active connection using this client.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void RemoveClient(string uniqueIdentifier, string serviceName, string hostName, Action<bool, object> callback, int activeConnections = 0, string actionName = "", object state = null);

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
        void AddHost(string hostName, int index, string domain, Action<bool, object> callback, string type = "", int activeConnections = 0, string actionName = "", object state = null);

        /// <summary>
        /// Remove the host.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void RemoveHost(string hostName, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Set the client host name.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void SetClientHost(string uniqueIdentifier, string serviceName, string hostName, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Set the client communication active state.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="active">The communication active state.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void SetClientActive(string uniqueIdentifier, string serviceName, bool active, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Set the communication token.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="communicationToken">The client communication token.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void SetClientCommunicationToken(string uniqueIdentifier, string serviceName, string communicationToken, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get the client host name.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetClientHost(string uniqueIdentifier, string serviceName, Action<string, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get the communication active state.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetClientActive(string uniqueIdentifier, string serviceName, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get the communication token.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetClientCommunicationToken(string uniqueIdentifier, string serviceName, Action<string, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Set the host index number.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="index">The host index number.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void SetHostIndex(string hostName, int index, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Set the number of active connections.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="activeConnections">The number of active connections on the host.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void SetHostActiveConnections(string hostName, int activeConnections, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Set the host domain.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="domain">The host domain.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void SetHostDomain(string hostName, string domain, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get the host index number.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetHostIndex(string hostName, Action<int, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get the number of active connections.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetHostActiveConnections(string hostName, Action<int, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get the host domain.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetHostDomain(string hostName, Action<string, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Does the client exist.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void ClientExists(string uniqueIdentifier, string serviceName, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Does the host exist.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void HostExists(string hostName, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get the host details for the identities.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetHost(string uniqueIdentifier, string serviceName, Action<string, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Adds new or replaces all the port ranges.
        /// </summary>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="portTypeName">The list of application port names.</param>
        /// <param name="portTypeNumber">The list of application port numbers.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void AddPort(string serviceName, string[] portTypeName, int[] portTypeNumber, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Sets the port number.
        /// </summary>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="portTypeName">The application port name.</param>
        /// <param name="portTypeNumber">The application port number.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void SetPortNumber(string serviceName, string portTypeName, int portTypeNumber, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Remove all the port ranges.
        /// </summary>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="portTypeName">The application port name.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void RemovePort(string serviceName, string portTypeName, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Remove all the port ranges.
        /// </summary>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void RemovePort(string serviceName, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Set the host type.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="type">The host type.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void SetHostType(string hostName, string type, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get the host type.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetHostType(string hostName, Action<string, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Adds new manage URLs.
        /// </summary>
        /// <param name="type">The manage type.</param>
        /// <param name="urls">The list of service urls.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void AddManageURL(string type, string[] urls, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Removes all manage URLs.
        /// </summary>
        /// <param name="type">The manage type.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void RemoveManageURL(string type, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Removes a manage URLs.
        /// </summary>
        /// <param name="type">The manage type.</param>
        /// <param name="url">The service url to remove.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void RemoveManageURL(string type, string url, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get the manage URLs for a type.
        /// </summary>
        /// <param name="type">The manage type.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetManageURLs(string type, Action<string[], object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get the host details for the identities.
        /// </summary>
        /// <param name="uniqueIdentifiers">The unique client identifiers.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetHosts(string[] uniqueIdentifiers, string serviceName, Action<string[], string[], object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get the ports.
        /// </summary>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void GetPorts(string serviceName, Action<string[], object> callback, string actionName = "", object state = null);

    }
}
