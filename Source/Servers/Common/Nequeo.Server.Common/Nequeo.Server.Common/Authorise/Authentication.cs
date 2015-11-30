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

namespace Nequeo.Server.Authorise
{
    /// <summary>
    /// Authentication helper.
    /// </summary>
    public class Authentication
    {
        /// <summary>
        /// Authentication direct server request command.
        /// </summary>
        public enum DirectServerRequestCommand
        {
            /// <summary>
            /// Logoff the client.
            /// </summary>
            AXLO = 0,
            /// <summary>
            /// Get all the current client contacts that are currently online.
            /// </summary>
            AXCO = 1,
            /// <summary>
            /// Get all the client contacts.
            /// </summary>
            AXCL = 2,
            /// <summary>
            /// Set the online status of the client.
            /// </summary>
            AXSO = 3,
        }

        /// <summary>
        /// Common authentication helper.
        /// </summary>
        public Authentication()
        {
            _contactsOnline = new Dictionary<string, string[]>();
            AllowCommandList = new List<string>();

            AllowCommandList.Add("AUTH");
            AllowCommandList.Add("ACOO");
            AllowCommandList.Add("ACLC");
            AllowCommandList.Add("ALOF");
            AllowCommandList.Add("ASOS");
            AllowCommandList.Add("ARCC");
            AllowCommandList.Add("AACO");
            AllowCommandList.Add("AFDP");
            AllowCommandList.Add("ACOS");
            AllowCommandList.Add("AICO");
        }

        private object _lockContactsOnlineStore = new object();
        private bool _isAuthenticated = false;
        private Dictionary<string, string[]> _contactsOnline = null;

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
            _lockContactsOnlineStore = null;
            AllowCommandList.Clear();

            if (_contactsOnline != null)
            {
                foreach (KeyValuePair<string, string[]> item in _contactsOnline)
                {
                    if (item.Value != null)
                    {
                        string[] value = item.Value;
                        value = null;
                    }
                }
                _contactsOnline.Clear();
            }
            _contactsOnline = null;
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
                case "AUTH":
                    // Send a message to the client
                    // indicating that the credentials
                    // are valid.
                    Authenticate(context, false, data);
                    return true;

                case "AREJ":
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
        /// Authenticate the client to the server.
        /// </summary>
        /// <param name="context">The server context sending the data.</param>
        /// <param name="request">Is in request mode.</param>
        /// <param name="data">The data from the client.</param>
        protected virtual void Authenticate(Nequeo.Net.Sockets.ServerContext context, bool request, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                // Get the intentity.
                string uniqueClientIdentifier = data.Trim();

                // Assign the client data.
                context.UniqueIdentifier = uniqueClientIdentifier;

                // If an identifier exists.
                if (!String.IsNullOrEmpty(context.UniqueIdentifier))
                {
                    // The client has been authenticated.
                    context.SetAuthenticated();

                    // Send a message to the client
                    // indicating that the client
                    // is allowed to join the conversation.
                    context.Send("JOIN 200 " + uniqueClientIdentifier + "\r\n");
                }
                else
                {
                    // Send a message to the client
                    // indicating that the credentials
                    // are invalid.
                    context.Send("REJD 402 Credentials have been suspended or invalid." + "\r\n");
                }
            }
            else
                // Send an error to the client.
                context.Send("ERRO 500 No data" + "\r\n");
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
                case "AXCO":
                    // Attempt to get all the current client contacts
                    // that are currently online.
                    ExecuteRequest(context, command, request, data, true);
                    return true;

                case "ACOO":
                    // Attempt to get all the current client contacts
                    // that are currently online.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ACLD":
                    // Get all the client contacts that are online and logged on
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ASUS":
                    // Client suspended.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "AACO":
                    // Add a new contact for this client.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "AACL":
                    // Add a new client.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ACEX":
                    // Does the client exist.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "AFDP":
                    // Find a person that could potentally be a contact.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ACAC":
                    // Get the client active connections count.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ACAN":
                    // Get the client application name.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "AXCL":
                    // Get all the client contacts.
                    ExecuteRequest(context, command, request, data, true);
                    return true;

                case "ACLC":
                    // Get all the client contacts.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ACEA":
                    // Get the client email address.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ACNM":
                    // Get the client name.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ACOS":
                    // Get the client online and available status.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ACUN":
                    // Get the client username.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "AICO":
                    // Is the client contact online and available.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "AICL":
                    // Is the client currently logged-on.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "AXLO":
                    // Logoff the client, direct server request.
                    ExecuteRequest(context, command, request, data, true);
                    return true;

                case "ALOF":
                    // Logoff the client.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ARCL":
                    // Remove an existing client.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ARCC":
                    // Remove a contact from this client.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ASAC":
                    // Set the client active connections count for the same client.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ASAN":
                    // Set the client application name.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ASED":
                    // Set the client application name.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ASLS":
                    // Set the client logged-on state.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ASCN":
                    // Set the client name.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "AXXS":
                case "AXSO":
                    // Set the online status of the client.
                    ExecuteRequest(context, command, request, data, true);
                    return true;

                case "ASOS":
                    // Set the online status of the client.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ASCP":
                    // Set the client password.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ASSS":
                    // Set the client suspended state.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "ASUN":
                    // Set the client username.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "AGCA":
                    // Get all the clients.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "AGOC":
                    // Get all the clients online.
                    ExecuteRequest(context, command, request, data);
                    return true;

                case "A1CO":
                    // Attempt to get all the current client contacts
                    // that are currently online.
                    ExecuteRequest(context, command, request, data, true);

                    // If result mode.
                    if (!request)
                        // Store the result.
                        StoreContactsOnline(ActionStoreKey.ASK1, data.Split('|'));
                    return true;

                case "A2CO":
                    // Attempt to get all the current client contacts
                    // that are currently online.
                    ExecuteRequest(context, command, request, data, true);

                    // If result mode.
                    if (!request)
                        // Store the result.
                        StoreContactsOnline(ActionStoreKey.ASK2, data.Split('|'));
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
                    if(!isDirectServerRequest)
                        // Send a message to the client.
                        context.Send(command + " 200" + (String.IsNullOrEmpty(data) ? "" : " " + data) + "\r\n");
                }
            }
        }

        /// <summary>
        /// Authenticate the user.
        /// </summary>
        /// <param name="context">The server context sending the data.</param>
        /// <param name="username">The user name.</param>
        /// <param name="password">The password.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="applicationName">The application name.</param>
        public void ValidateUser(Nequeo.Net.Sockets.ServerContext context, string username, string password, string domain = null, string applicationName = "All")
        {
            ExecuteRequest(context, "AUTH", true,
                username + "|" + password + "|" +
                (String.IsNullOrEmpty(domain) ? "" : domain) + "|" +
                (String.IsNullOrEmpty(applicationName) ? "All" : applicationName));
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
                case "ALOF":
                case "AXLO":
                    directCommand = "AXLO";
                    return true;
                case "ACOO":
                case "AXCO":
                    directCommand = "AXCO";
                    return true;
                case "ACLC":
                case "AXCL":
                    directCommand = "AXCL";
                    return true;
                case "ASOS":
                case "AXSO":
                    directCommand = "AXSO";
                    return true;
                default:
                    directCommand = string.Empty;
                    return false;
            }
        }

        /// <summary>
        /// Get all the client contacts.
        /// </summary>
        /// <param name="context">The server context sending the data.</param>
        /// <remarks>This does not relay back to the client.</remarks>
        public void GetClientContacts(Nequeo.Net.Sockets.ServerContext context)
        {
            ExecuteRequest(context, "AXCL", true, "");
        }

        /// <summary>
        /// Get all the current client contacts that are currently online.
        /// </summary>
        /// <param name="context">The server context sending the data.</param>
        /// <remarks>This does not relay back to the client.</remarks>
        public void GetClientContactsOnline(Nequeo.Net.Sockets.ServerContext context)
        {
            ExecuteRequest(context, "AXCO", true, "");
        }

        /// <summary>
        /// Logoff the client.
        /// </summary>
        /// <param name="context">The server context sending the data.</param>
        /// <remarks>This does not relay back to the client.</remarks>
        public void LogOff(Nequeo.Net.Sockets.ServerContext context)
        {
            ExecuteRequest(context, "AXLO", true, "");
        }

        /// <summary>
        /// Set the online status of the client.
        /// </summary>
        /// <param name="context">The server context sending the data.</param>
        /// <param name="status">The status of the client.</param>
        public void SetClientOnlineStatus(Nequeo.Net.Sockets.ServerContext context, OnlineStatus status)
        {
            ExecuteRequest(context, "AXSO", true, status.ToString());
        }

        /// <summary>
        /// Set the online status of the client, no callback.
        /// </summary>
        /// <param name="context">The server context sending the data.</param>
        /// <param name="status">The status of the client.</param>
        public void SetClientOnlineStatusNoCallback(Nequeo.Net.Sockets.ServerContext context, string status)
        {
            ExecuteRequest(context, "AXXS", true, status);
        }

        /// <summary>
        /// Store the contacts online.
        /// </summary>
        /// <param name="storeKey">The store key value.</param>
        /// <param name="contactsOnline">The contacts online list; else null to get contacts online.</param>
        /// <param name="context">The server context sending the data.</param>
        public void StoreContactsOnline(ActionStoreKey storeKey, string[] contactsOnline, 
            Nequeo.Net.Sockets.ServerContext context = null)
        {
            lock (_lockContactsOnlineStore)
            {
                if (contactsOnline != null)
                {
                    // Assign the list.
                    _contactsOnline[storeKey.ToString()] = contactsOnline;
                }
                else
                {
                    switch (storeKey)
                    {
                        case ActionStoreKey.ASK1:
                            ExecuteRequest(context, "A1CO", true, "");
                            break;
                        case ActionStoreKey.ASK2:
                            ExecuteRequest(context, "A2CO", true, "");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Get the stored items.
        /// </summary>
        /// <param name="storeKey">The store key value.</param>
        /// <returns>The collection of stored items; else null.</returns>
        public string[] GetStoredContactsOnline(ActionStoreKey storeKey)
        {
            lock (_lockContactsOnlineStore)
            {
                try
                {
                    return _contactsOnline[storeKey.ToString()];
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
        public void RemoveStoredContactsOnline(ActionStoreKey storeKey)
        {
            lock (_lockContactsOnlineStore)
            {
                try
                {
                    string[] item = _contactsOnline[storeKey.ToString()];
                    item = null;
                    _contactsOnline.Remove(storeKey.ToString());
                }
                catch { }
            }
        }
    }
}
