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

namespace Nequeo.Server.Authorise
{
    /// <summary>
    /// Communication helper.
    /// </summary>
    public class Communication
    {
        /// <summary>
        /// Communication direct server request command.
        /// </summary>
        public enum DirectServerRequestCommand
        {
            /// <summary>
            /// Get the host details for the identity.
            /// </summary>
            ZXGH = 0,
            /// <summary>
            /// Get the host details for the identities.
            /// </summary>
            ZXGS = 1,
            /// <summary>
            /// Add a new client.
            /// </summary>
            ZXAC = 2,
            /// <summary>
            /// Remove a new client.
            /// </summary>
            ZXCR = 3,
        }

        /// <summary>
        /// Common communication helper.
        /// </summary>
        public Communication()
        {
            _contactsOnlineHosts = new Dictionary<string, string[]>();
            AllowCommandList = new List<string>();
        }

        private object _lockContactsOnlineHostsStore = new object();
        private bool _isAuthenticated = false;
        private Dictionary<string, string[]> _contactsOnlineHosts = null;

        /// <summary>
        /// The list of command that the client can access.
        /// </summary>
        public List<string> AllowCommandList = null;

        /// <summary>
        /// Gets sets, is authenticated.
        /// </summary>
        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
            set { _isAuthenticated = value; }
        }

        /// <summary>
        /// Disposes of all resources.
        /// </summary>
        public void Dispose()
        {
            _lockContactsOnlineHostsStore = null;
            AllowCommandList.Clear();

            if (_contactsOnlineHosts != null)
            {
                foreach (KeyValuePair<string, string[]> item in _contactsOnlineHosts)
                {
                    if (item.Value != null)
                    {
                        string[] value = item.Value;
                        value = null;
                    }
                }
                _contactsOnlineHosts.Clear();
            }
            _contactsOnlineHosts = null;
            AllowCommandList = null;
        }

        /// <summary>
        /// Data sent from the server context to the server.
        /// </summary>
        /// <param name="context">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data sent from the server context to the server.</param>
        /// <returns>True if the command was found and executed; else false.</returns>
        public bool Receiver(Nequeo.Net.Sockets.ServerContext context, string command, string data)
        {
            return ExecuteCommand(context, command, true, data);
        }

        /// <summary>
        /// Data sent from the server context to the server.
        /// </summary>
        /// <param name="context">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data sent from the server context to the server.</param>
        /// <returns>True if the command was found and executed; else false.</returns>
        public bool ReceiveFromServer(Nequeo.Net.Sockets.ServerContext context, string command, string data)
        {
            // Delay.
            System.Threading.Thread.Sleep(10);

            // Process the command.
            switch (command.ToUpper())
            {
                case "ZREJ":
                    // Send a message to the client
                    // indicating that the credentials
                    // are invalid.
                    context.Send("REJD 401 Credentials are invalid." + "\r\n");
                    return true;

                case "ERRO":
                    // Send an error to the client.
                    context.Send("ERRO 500" + (String.IsNullOrEmpty(data) ? "" : " " + data) + "\r\n");
                    return true;

                default:
                    return ExecuteCommand(context, command, false, data);
            }
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="context">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="request">Is in request mode.</param>
        /// <param name="data">The data from the client.</param>
        /// <returns>True if the command was found and executed; else false.</returns>
        private bool ExecuteCommand(Nequeo.Net.Sockets.ServerContext context, string command, bool request, string data)
        {
            // Delay.
            System.Threading.Thread.Sleep(10);

            // Process the command.
            switch (command.ToUpper())
            {
                case "ZURL":
                    // Get the host manage URL for a type.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZAMU":
                    // Adds new manage URLs.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZRME":
                    // Removes a manage URLs.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZRMU":
                    // Removes a manage URLs.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZHHT":
                    // Get the host type.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZHTS":
                    // Set the host type.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZXAC":
                    // Add a new client.
                    ExecuteRequest(context, command, request, data, true);
                    return true;

                case "ZACL":
                    // Add a new client.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZAHN":
                    // Add a new host.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZCEX":
                    // Does the client exist.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZCAC":
                    // Get the communication active state.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZCCT":
                    // Get the communication token.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZCHN":
                    // Get the client host name.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZHAC":
                    // Get the number of active connections.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZHPT":
                    // Get the host index number.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZHEX":
                    // Does the host exist.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZXCR":
                    // Remove the client.
                    ExecuteRequest(context, command, request, data, true);
                    return true;

                case "ZCRM":
                    // Remove the client.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZHRM":
                    // Remove the host.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZXGH":
                    // Get the host details for the identity.
                    ExecuteRequest(context, command, request, data, true);
                    return true;

                case "ZHST":
                    // Get the host details for the identity.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZXGS":
                    // Get the host details for the identities.
                    ExecuteRequest(context, command, request, data, true);
                    return true;

                case "ZHSS":
                    // Get the host details for the identities.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZCSA":
                    // Set the client communication active state.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZCST":
                    // Set the communication token.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZCSH":
                    // Set the client host name.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZHSC":
                    // Set the number of active connections.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZHSP":
                    // Set the host index number.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZHSD":
                    // Set the host domain.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZHGD":
                    // Get the host domain.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZAPT":
                    // Add a new port range.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZRPT":
                    // Remove the port.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZRPX":
                    // Remove the port.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZRSN":
                    // Sets the port number.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ZGPT":
                    // Get the ports.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "Z1GS":
                    // Get the host details for the identities.
                    ExecuteRequest(context, command, request, data, true);

                    // If result mode.
                    if (!request)
                        // Store the result.
                        StoreContactsOnlineHosts(ActionStoreKey.ASK1, data.Split('|'));
                    return true;

                case "Z2GS":
                    // Get the host details for the identities.
                    ExecuteRequest(context, command, request, data, true);

                    // If result mode.
                    if (!request)
                        // Store the result.
                        StoreContactsOnlineHosts(ActionStoreKey.ASK2, data.Split('|'));
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Execute the request.
        /// </summary>
        /// <param name="context">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="request">Is in request mode; else returning data.</param>
        /// <param name="data">The data from the client.</param>
        /// <param name="isDirectServerRequest">Is the request direct from the server.</param>
        private void ExecuteRequest(Nequeo.Net.Sockets.ServerContext context, string command, bool request, string data, bool isDirectServerRequest = false)
        {
            // Has the credentials been authenticated.
            if (_isAuthenticated)
            {
                // If in request mode.
                if (request)
                {
                    // Send a message to the server.
                    context.SendToServer(Encoding.ASCII.GetBytes(command + (String.IsNullOrEmpty(data) ? "" : " " + data) + "\r\n"));
                }
                else
                {
                    // If not direct from server.
                    if (!isDirectServerRequest)
                        // Send a message to the client.
                        context.Send(command + " 200" + (String.IsNullOrEmpty(data) ? "" : " " + data) + "\r\n");
                }
            }
        }

        /// <summary>
        /// Is the command a direct server request.
        /// </summary>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="directCommand">The direct server request command.</param>
        /// <returns>True if the command is a direct server request; else false.</returns>
        public bool IsDirectServerRequestCommand(string command, out string directCommand)
        {
            switch (command.ToUpper())
            {
                case "ZHST":
                case "ZXGH":
                    directCommand = "ZXGH";
                    return true;
                case "ZHSS":
                case "ZXGS":
                    directCommand = "ZXGS";
                    return true;
                case "ZACL":
                case "ZXAC":
                    directCommand = "ZXAC";
                    return true;
                case "ZCRM":
                case "ZXCR":
                    directCommand = "ZXCR";
                    return true;
                default:
                    directCommand = string.Empty;
                    return false;
            }
        }

        /// <summary>
        /// Get the host details for the identity.
        /// </summary>
        /// <param name="context">The server context sending the data.</param>
        /// <remarks>This does not relay back to the client.</remarks>
        public void GetHost(Nequeo.Net.Sockets.ServerContext context)
        {
            ExecuteRequest(context, "ZXGH", true, "");
        }

        /// <summary>
        /// Get the host details for the identities.
        /// </summary>
        /// <param name="context">The server context sending the data.</param>
        /// <param name="uniqueIdentifiers">The unique client identifiers.</param>
        /// <remarks>This does not relay back to the client.</remarks>
        public void GetHosts(Nequeo.Net.Sockets.ServerContext context, string[] uniqueIdentifiers)
        {
            ExecuteRequest(context, "ZXGS", true, String.Join(";", uniqueIdentifiers));
        }

        /// <summary>
        /// Add a new client.
        /// </summary>
        /// <param name="context">The server context sending the data.</param>
        /// <param name="hostName">The host machine the client must connect to.</param>
        /// <param name="active">Is the communication currenlty active.</param>
        /// <param name="communicationToken">The common token used for communication.</param>
        /// <remarks>This does not relay back to the client.</remarks>
        public void AddClient(Nequeo.Net.Sockets.ServerContext context, string hostName, bool active = false, string communicationToken = "")
        {
            ExecuteRequest(context, "ZXAC", true, 
                hostName + "|" + 
                active.ToString() + "|" +
                (String.IsNullOrEmpty(communicationToken) ? "" : communicationToken));
        }

        /// <summary>
        /// Remove a new client.
        /// </summary>
        /// <param name="context">The server context sending the data.</param>
        /// <param name="hostName">The host machine the client must connect to.</param>
        /// <param name="activeConnections">The number of active connection using this client.</param>
        /// <remarks>This does not relay back to the client.</remarks>
        public void RemoveClient(Nequeo.Net.Sockets.ServerContext context, string hostName, int activeConnections = 0)
        {
            ExecuteRequest(context, "ZXCR", true,
                hostName + "|" +
                activeConnections);
        }

        /// <summary>
        /// Store the contacts online hosts.
        /// </summary>
        /// <param name="storeKey">The store key value.</param>
        /// <param name="contactsOnlineHosts">The contacts online list; else null to get contacts online.</param>
        /// <param name="context">The server context sending the data.</param>
        /// <param name="uniqueIdentifiers">The unique client identifiers.</param>
        public void StoreContactsOnlineHosts(ActionStoreKey storeKey, string[] contactsOnlineHosts, 
            Nequeo.Net.Sockets.ServerContext context = null, string[] uniqueIdentifiers = null)
        {
            lock (_lockContactsOnlineHostsStore)
            {
                if (contactsOnlineHosts != null)
                {
                    // Assign the list.
                    _contactsOnlineHosts[storeKey.ToString()] = contactsOnlineHosts;
                }
                else
                {
                    switch (storeKey)
                    {
                        case ActionStoreKey.ASK1:
                            ExecuteRequest(context, "Z1GS", true, String.Join(";", uniqueIdentifiers));
                            break;
                        case ActionStoreKey.ASK2:
                            ExecuteRequest(context, "Z2GS", true, String.Join(";", uniqueIdentifiers));
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Get the stored items.
        /// </summary>
        /// <param name="storeKey">The store key value.</param>
        /// <returns>The collection of stored items.</returns>
        public string[] GetStoredContactsOnlineHosts(ActionStoreKey storeKey)
        {
            lock (_lockContactsOnlineHostsStore)
            {
                try
                {
                    return _contactsOnlineHosts[storeKey.ToString()];
                }
                catch
                {
                    return null;
                }
            } 
        }

        /// <summary>
        /// Remove the stored items.
        /// </summary>
        /// <param name="storeKey">The store key value.</param>
        public void RemoveStoredContactsOnlineHosts(ActionStoreKey storeKey)
        {
            lock (_lockContactsOnlineHostsStore)
            {
                try
                {
                    string[] item = _contactsOnlineHosts[storeKey.ToString()];
                    item = null;
                    _contactsOnlineHosts.Remove(storeKey.ToString());
                }
                catch { }
            }
        }
    }
}
