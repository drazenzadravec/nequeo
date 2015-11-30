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
using Nequeo.Data.Provider;

namespace Nequeo.Server.Authorise
{
    /// <summary>
    /// Common authentication server and client base helper.
    /// </summary>
    public class AuthenticateBase
    {
        /// <summary>
        /// Common authentication server helper.
        /// </summary>
        /// <param name="authenticationProvider">The type used to authenticate requests from applications.</param>
        public AuthenticateBase(IAuthentication authenticationProvider)
        {
            if (authenticationProvider == null) throw new ArgumentNullException("authenticationProvider");
            _authenticationProvider = authenticationProvider;
        }

        private IAuthentication _authenticationProvider = null;
        private bool _isAuthenticationServer = true;

        /// <summary>
        /// Gets sets, is authentication server.
        /// </summary>
        protected bool IsAuthenticationServer
        {
            get { return _isAuthenticationServer; }
            set { _isAuthenticationServer = value; }
        }

        /// <summary>
        /// Data sent from the server context to the server.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data sent from the server context to the server.</param>
        /// <returns>True if the command was found and executed; else false.</returns>
        public bool Receiver(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            // Delay.
            System.Threading.Thread.Sleep(10);

            // Process the command.
            switch (command.ToUpper())
            {
                case "AUTH":
                    // Attempt to autherticate the credentials.
                    Authenticate(server, serverContext, command, data);
                    return true;

                case "A1CO":
                case "A2CO":
                case "ACOO":
                case "AXCO":
                    // Attempt to get all the current client contacts
                    // that are currently online.
                    GetClientContactsOnline(server, serverContext, command, data);
                    return true;

                case "ACLD":
                    // Get all the client contacts that are online and logged on
                    GetClientContactsLoggedOn(server, serverContext, command, data);
                    return true;

                case "ASUS":
                    // Client suspended.
                    IsClientSuspended(server, serverContext, command, data);
                    return true;

                case "AACO":
                    // Add a new contact for this client.
                    AddClientContact(server, serverContext, command, data);
                    return true;

                case "AACL":
                    // Add a new client.
                    AddClient(server, serverContext, command, data);
                    return true;

                case "ACEX":
                    // Does the client exist.
                    ClientExists(server, serverContext, command, data);
                    return true;

                case "AFDP":
                    // Find a person that could potentally be a contact.
                    FindPerson(server, serverContext, command, data);
                    return true;

                case "ACAC":
                    // Get the client active connections count.
                    GetClientActiveConnections(server, serverContext, command, data);
                    return true;

                case "ACAN":
                    // Get the client application name.
                    GetClientApplicationName(server, serverContext, command, data);
                    return true;

                case "ACLC":
                case "AXCL":
                    // Get all the client contacts.
                    GetClientContacts(server, serverContext, command, data);
                    return true;

                case "ACEA":
                    // Get the client email address.
                    GetClientEmailAddress(server, serverContext, command, data);
                    return true;

                case "ACNM":
                    // Get the client name.
                    GetClientName(server, serverContext, command, data);
                    return true;

                case "ACOS":
                    // Get the client online and available status.
                    GetClientOnlineStatus(server, serverContext, command, data);
                    return true;

                case "ACUN":
                    // Get the client username.
                    GetClientUsername(server, serverContext, command, data);
                    return true;

                case "AICO":
                    // Is the client contact online and available.
                    IsClientContactOnline(server, serverContext, command, data);
                    return true;

                case "AICL":
                    // Is the client currently logged-on.
                    IsClientLoggedOn(server, serverContext, command, data);
                    return true;

                case "ALOF":
                case "AXLO":
                    // Logoff the client.
                    LogOff(server, serverContext, command, data);
                    return true;

                case "ARCL":
                    // Remove an existing client.
                    RemoveClient(server, serverContext, command, data);
                    return true;

                case "ARCC":
                    // Remove a contact from this client.
                    RemoveClientContact(server, serverContext, command, data);
                    return true;

                case "ASAC":
                    // Set the client active connections count for the same client.
                    SetClientActiveConnections(server, serverContext, command, data);
                    return true;

                case "ASAN":
                    // Set the client application name.
                    SetClientApplicationName(server, serverContext, command, data);
                    return true;

                case "ASED":
                    // Set the client email address.
                    SetClientEmailAddress(server, serverContext, command, data);
                    return true;

                case "ASLS":
                    // Set the client logged-on state.
                    SetClientLoggedOnState(server, serverContext, command, data);
                    return true;

                case "ASCN":
                    // Set the client name.
                    SetClientName(server, serverContext, command, data);
                    return true;

                case "AXXS":
                case "ASOS":
                case "AXSO":
                    // Set the online status of the client.
                    SetClientOnlineStatus(server, serverContext, command, data);
                    return true;

                case "ASCP":
                    // Set the client password.
                    SetClientPassword(server, serverContext, command, data);
                    return true;

                case "ASSS":
                    // Set the client suspended state.
                    SetClientSuspendedState(server, serverContext, command, data);
                    return true;

                case "ASUN":
                    // Set the client username.
                    SetClientUsername(server, serverContext, command, data);
                    return true;

                case "AGCA":
                    // Get all the clients.
                    GetClients(server, serverContext, command, data);
                    return true;

                case "AGOC":
                    // Get all the clients online.
                    GetClientsOnline(server, serverContext, command, data);
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Authenticate the client to the server.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void Authenticate(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                // Message parts are divided by "|"  
                // Break the string into an array accordingly.
                string[] dataArray = data.Split('|');

                // Get the user name and password
                // to validate the client.
                string username = dataArray[0].Trim();
                string password = dataArray[1].Trim();
                string domain = dataArray[2].Trim();
                string applicationName = dataArray[3].Trim();

                // Validate the client credentails.
                string result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string>(() =>
                    {
                        return _authenticationProvider.ValidateUser(username, password, domain, applicationName);
                    });

                // If the credentails are valid.
                if (!String.IsNullOrEmpty(result))
                {
                    // Send a message back to the context
                    // indicating that the credentials have
                    // been authenticated.
                    serverContext.SentFromServer(
                        Encoding.ASCII.GetBytes(command +
                            (String.IsNullOrEmpty(result) ? "" : " " + result) + "\r\n"));
                }
                else
                    // Send a message back to the context
                    // indicating that the credentials have
                    // not been authenticated.
                    serverContext.SentFromServer(Encoding.ASCII.GetBytes("AREJ" + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Get all the client contacts online.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetClientContactsOnline(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string identifier = serverContext.UniqueIdentifier;

            if (_isAuthenticationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Get all the client contacts online and available.
                string[] result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string[]>(() =>
                    {
                        return _authenticationProvider.GetClientContactsOnline((_isAuthenticationServer ? data.Trim() : identifier));
                    });

                // Send to the context all online contacts.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command +
                        ((result == null || result.Length < 1) ? "" : " " + String.Join("|", result)) + "\r\n"));
            }
        }

        /// <summary>
        /// Get all the client contacts that are online and logged on.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetClientContactsLoggedOn(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string identifier = serverContext.UniqueIdentifier;

            if (_isAuthenticationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Get all the client contacts online and available.
                string[] result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string[]>(() =>
                    {
                        return _authenticationProvider.GetClientContactsLoggedOn((_isAuthenticationServer ? data.Trim() : identifier));
                    });

                // Send to the context all online contacts.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command +
                        ((result == null || result.Length < 1) ? "" : " " + String.Join("|", result)) + "\r\n"));
            }
        }

        /// <summary>
        /// Get the client suspended state.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void IsClientSuspended(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string identifier = serverContext.UniqueIdentifier;

            if (_isAuthenticationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Is the client suspended.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _authenticationProvider.IsClientSuspended((_isAuthenticationServer ? data.Trim() : identifier));
                    });

                // Send to the context suspended state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
        }

        /// <summary>
        /// Add a new contact for this client.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void AddClientContact(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string identifier = serverContext.UniqueIdentifier;
                string identifierContact = data.Trim();

                if (_isAuthenticationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get the details from the data.
                    identifier = dataArray[0].Trim();
                    identifierContact = dataArray[1].Trim();
                }

                // Result of the operation.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _authenticationProvider.AddClientContact(identifier, identifierContact);
                    });

                // Send to the context result state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Add a new client.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void AddClient(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string identifier = serverContext.UniqueIdentifier;

                // Message parts are divided by "|"  
                // Break the string into an array accordingly.
                string[] dataArray = data.Split('|');

                // Get the details from the data.
                string name = dataArray[0].Trim();
                string emailAddress = dataArray[1].Trim();
                string username = dataArray[2].Trim();
                string password = dataArray[3].Trim();
                string applicationName = dataArray[4].Trim();
                string[] clientContacts = String.IsNullOrEmpty(dataArray[5].Trim()) ? null : dataArray[5].Trim().Split(';');

                // Result of the operation.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _authenticationProvider.AddClient(name, emailAddress, username, password, applicationName, clientContacts);
                    });

                // Send to the context result state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Does the client exist.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void ClientExists(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string identifier = serverContext.UniqueIdentifier;

            if (_isAuthenticationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Result of the operation.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _authenticationProvider.ClientExists((_isAuthenticationServer ? data.Trim() : identifier));
                    });

                // Send to the context suspended state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
        }

        /// <summary>
        /// Find a person that could potentally be a contact.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void FindPerson(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string identifier = serverContext.UniqueIdentifier;

                // Result of the operation.
                string[] result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string[]>(() =>
                    {
                        return _authenticationProvider.FindPerson(data.Trim());
                    });

                // Send to the context suspended state.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command +
                        ((result == null || result.Length < 1) ? "" : " " + String.Join("|", result)) + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Get the client active connections count.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetClientActiveConnections(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string identifier = serverContext.UniqueIdentifier;

            if (_isAuthenticationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Result of the operation.
                int result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<int>(() =>
                    {
                        return _authenticationProvider.GetClientActiveConnections((_isAuthenticationServer ? data.Trim() : identifier));
                    });

                // Send to the context suspended state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
        }

        /// <summary>
        /// Get the client application name.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetClientApplicationName(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string identifier = serverContext.UniqueIdentifier;

            if (_isAuthenticationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Result of the operation.
                string result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string>(() =>
                    {
                        return _authenticationProvider.GetClientApplicationName((_isAuthenticationServer ? data.Trim() : identifier));
                    });

                // Send to the context suspended state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + (String.IsNullOrEmpty(result) ? "" : " " + result) + "\r\n"));
            }
        }

        /// <summary>
        /// Get all the client contacts.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetClientContacts(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string identifier = serverContext.UniqueIdentifier;

            if (_isAuthenticationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Result of the operation.
                string[] result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string[]>(() =>
                    {
                        return _authenticationProvider.GetClientContacts((_isAuthenticationServer ? data.Trim() : identifier));
                    });

                // Send to the context suspended state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + ((result == null || result.Length < 1) ? "" : " " + String.Join("|", result)) + "\r\n"));
            }
        }

        /// <summary>
        /// Get the client application name.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetClientEmailAddress(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string identifier = serverContext.UniqueIdentifier;

            if (_isAuthenticationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Result of the operation.
                string result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string>(() =>
                    {
                        return _authenticationProvider.GetClientEmailAddress((_isAuthenticationServer ? data.Trim() : identifier));
                    });

                // Send to the context suspended state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + (String.IsNullOrEmpty(result) ? "" : " " + result.ToString()) + "\r\n"));
            }
        }

        /// <summary>
        /// Get the client name.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetClientName(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string identifier = serverContext.UniqueIdentifier;

            if (_isAuthenticationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Result of the operation.
                string result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string>(() =>
                    {
                        return _authenticationProvider.GetClientName((_isAuthenticationServer ? data.Trim() : identifier));
                    });

                // Send to the context suspended state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + (String.IsNullOrEmpty(result) ? "" : " " + result.ToString()) + "\r\n"));
            }
        }

        /// <summary>
        /// Get the client online and available status.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetClientOnlineStatus(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string identifier = serverContext.UniqueIdentifier;

            if (_isAuthenticationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Result of the operation.
                OnlineStatus result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<OnlineStatus>(() =>
                    {
                        return _authenticationProvider.GetClientOnlineStatus((_isAuthenticationServer ? data.Trim() : identifier));
                    });

                // Send to the context suspended state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
        }

        /// <summary>
        /// Get the client name.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetClientUsername(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string identifier = serverContext.UniqueIdentifier;

            if (_isAuthenticationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Result of the operation.
                string result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string>(() =>
                    {
                        return _authenticationProvider.GetClientUsername((_isAuthenticationServer ? data.Trim() : identifier));
                    });

                // Send to the context suspended state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + (String.IsNullOrEmpty(result) ? "" : " " + result.ToString()) + "\r\n"));
            }
        }

        /// <summary>
        /// Is the client contact online and available.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void IsClientContactOnline(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string identifier = serverContext.UniqueIdentifier;
                string identifierContact = data.Trim();

                if (_isAuthenticationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get the details from the data.
                    identifier = dataArray[0].Trim();
                    identifierContact = dataArray[1].Trim();
                }

                // Result of the operation.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _authenticationProvider.IsClientContactOnline(identifier, identifierContact);
                    });

                // Send to the context result state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Is the client currently logged-on.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void IsClientLoggedOn(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string identifier = serverContext.UniqueIdentifier;

            if (_isAuthenticationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Result of the operation.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _authenticationProvider.IsClientLoggedOn((_isAuthenticationServer ? data.Trim() : identifier));
                    });

                // Send to the context suspended state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
        }

        /// <summary>
        /// Logoff the client.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void LogOff(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string identifier = serverContext.UniqueIdentifier;

            if (_isAuthenticationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Result of the operation.
                int result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<int>(() =>
                    {
                        return _authenticationProvider.LogOff((_isAuthenticationServer ? data.Trim() : identifier));
                    });

                // Send to the context suspended state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
        }

        /// <summary>
        /// Remove an existing client.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void RemoveClient(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string identifier = serverContext.UniqueIdentifier;

            if (_isAuthenticationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Result of the operation.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _authenticationProvider.RemoveClient((_isAuthenticationServer ? data.Trim() : identifier));
                    });

                // Send to the context suspended state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
        }

        /// <summary>
        /// Is the client contact online and available.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void RemoveClientContact(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string identifier = serverContext.UniqueIdentifier;
                string identifierContact = data.Trim();

                if (_isAuthenticationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get the details from the data.
                    identifier = dataArray[0].Trim();
                    identifierContact = dataArray[1].Trim();
                }

                // Result of the operation.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _authenticationProvider.RemoveClientContact(identifier, identifierContact);
                    });

                // Send to the context result state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Set the client active connections count for the same client.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void SetClientActiveConnections(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string identifier = serverContext.UniqueIdentifier;
                string activeConnections = data.Trim();

                if (_isAuthenticationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get the details from the data.
                    identifier = dataArray[0].Trim();
                    activeConnections = dataArray[1].Trim();
                }

                // Result of the operation.
                await Nequeo.Threading.AsyncOperationResult<bool>.RunTask(() =>
                    {
                        _authenticationProvider.SetClientActiveConnections(identifier, Int32.Parse(activeConnections));
                    });

                // Send to the context result state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Set the client application name.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void SetClientApplicationName(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string identifier = serverContext.UniqueIdentifier;
                string applicationName = data.Trim();

                if (_isAuthenticationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get the details from the data.
                    identifier = dataArray[0].Trim();
                    applicationName = dataArray[1].Trim();
                }

                // Result of the operation.
                await Nequeo.Threading.AsyncOperationResult<bool>.RunTask(() =>
                    {
                        _authenticationProvider.SetClientApplicationName(identifier, applicationName);
                    });

                // Send to the context result state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Set the client email address.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void SetClientEmailAddress(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string identifier = serverContext.UniqueIdentifier;
                string emailAddress = data.Trim();

                if (_isAuthenticationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get the details from the data.
                    identifier = dataArray[0].Trim();
                    emailAddress = dataArray[1].Trim();
                }

                // Result of the operation.
                await Nequeo.Threading.AsyncOperationResult<bool>.RunTask(() =>
                    {
                        _authenticationProvider.SetClientEmailAddress(identifier, emailAddress);
                    });

                // Send to the context result state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Set the client logged-on state.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void SetClientLoggedOnState(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string identifier = serverContext.UniqueIdentifier;
                string loggedOn = data.Trim();

                if (_isAuthenticationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get the details from the data.
                    identifier = dataArray[0].Trim();
                    loggedOn = dataArray[1].Trim();
                }

                // Result of the operation.
                await Nequeo.Threading.AsyncOperationResult<bool>.RunTask(() =>
                    {
                        _authenticationProvider.SetClientLoggedOnState(identifier, Boolean.Parse(loggedOn));
                    });

                // Send to the context result state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Set the client name.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void SetClientName(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string identifier = serverContext.UniqueIdentifier;
                string name = data.Trim();

                if (_isAuthenticationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get the details from the data.
                    identifier = dataArray[0].Trim();
                    name = dataArray[1].Trim();
                }

                // Result of the operation.
                await Nequeo.Threading.AsyncOperationResult<bool>.RunTask(() =>
                    {
                        _authenticationProvider.SetClientName(identifier, name);
                    });

                // Send to the context result state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Set the online status of the client.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void SetClientOnlineStatus(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string identifier = serverContext.UniqueIdentifier;
                string status = data.Trim();

                if (_isAuthenticationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get the details from the data.
                    identifier = dataArray[0].Trim();
                    status = dataArray[1].Trim();
                }

                OnlineStatus statusVal = OnlineStatus.Invisible;
                switch (status.ToUpper())
                {
                    case "AVAILABLE":
                        statusVal = OnlineStatus.Available;
                        break;
                    case "INVISIBLE":
                    default:
                        statusVal = OnlineStatus.Invisible;
                        break;
                }

                // Result of the operation.
                await Nequeo.Threading.AsyncOperationResult<bool>.RunTask(() =>
                    {
                        _authenticationProvider.SetClientOnlineStatus(identifier, statusVal);
                    });

                // Send to the context result state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Set the client name.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void SetClientPassword(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string identifier = serverContext.UniqueIdentifier;
                string password = data.Trim();

                if (_isAuthenticationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get the details from the data.
                    identifier = dataArray[0].Trim();
                    password = dataArray[1].Trim();
                }

                // Result of the operation.
                await Nequeo.Threading.AsyncOperationResult<bool>.RunTask(() =>
                    {
                        _authenticationProvider.SetClientPassword(identifier, password);
                    });

                // Send to the context result state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Set the client suspended state.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void SetClientSuspendedState(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string identifier = serverContext.UniqueIdentifier;
                string suspended = data.Trim();

                if (_isAuthenticationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get the details from the data.
                    identifier = dataArray[0].Trim();
                    suspended = dataArray[1].Trim();
                }

                // Result of the operation.
                await Nequeo.Threading.AsyncOperationResult<bool>.RunTask(() =>
                    {
                        _authenticationProvider.SetClientSuspendedState(identifier, Boolean.Parse(suspended));
                    });

                // Send to the context result state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Set the client username.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void SetClientUsername(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string identifier = serverContext.UniqueIdentifier;
                string username = data.Trim();

                if (_isAuthenticationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get the details from the data.
                    identifier = dataArray[0].Trim();
                    username = dataArray[1].Trim();
                }

                // Result of the operation.
                await Nequeo.Threading.AsyncOperationResult<bool>.RunTask(() =>
                    {
                        _authenticationProvider.SetClientUsername(identifier, username);
                    });

                // Send to the context result state.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes(command + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Get all the clients.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetClients(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            // Get all the client contacts online and available.
            string[] result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string[]>(() =>
                    {
                        return _authenticationProvider.GetClients();
                    });

            // Send to the context all online contacts.
            serverContext.SentFromServer(
                Encoding.ASCII.GetBytes(command +
                    ((result == null || result.Length < 1) ? "" : " " + String.Join("|", result)) + "\r\n"));
        }

        /// <summary>
        /// Get all the clients online.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetClientsOnline(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            // Get all the client contacts online and available.
            string[] result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string[]>(() =>
                    {
                        return _authenticationProvider.GetClientsOnline();
                    });

            // Send to the context all online contacts.
            serverContext.SentFromServer(
                Encoding.ASCII.GetBytes(command +
                    ((result == null || result.Length < 1) ? "" : " " + String.Join("|", result)) + "\r\n"));
        }
    }
}
