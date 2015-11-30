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
    /// Common communication server and client base helper.
    /// </summary>
    public class CommunicateBase
    {
        /// <summary>
        /// Common communication server helper.
        /// </summary>
        /// <param name="communicationProvider">The type used to communicate requests from applications.</param>
        public CommunicateBase(ICommunication communicationProvider)
        {
            if (communicationProvider == null) throw new ArgumentNullException("communicationProvider");
            _communicationProvider = communicationProvider;
        }

        private ICommunication _communicationProvider = null;
        private bool _isCommunicationServer = true;

        /// <summary>
        /// Gets sets, is communication server.
        /// </summary>
        protected bool IsCommunicationServer
        {
            get { return _isCommunicationServer; }
            set { _isCommunicationServer = value; }
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
                case "ZURL":
                    // Get the host manage URLs for a type.
                    GetManageURLs(server, serverContext, command, data);
                    return true;

                case "ZAMU":
                    // Adds new manage URLs.
                    AddManageURL(server, serverContext, command, data);
                    return true;

                case "ZRME":
                    // Removes a manage URLs.
                    RemoveManageURLEx(server, serverContext, command, data);
                    return true;

                case "ZRMU":
                    // Removes a manage URLs.
                    RemoveManageURL(server, serverContext, command, data);
                    return true;

                case "ZXAC":
                case "ZACL":
                    // Add a new client.
                    AddClient(server, serverContext, command, data);
                    return true;

                case "ZAHN":
                    // Add a new host.
                    AddHost(server, serverContext, command, data);
                    return true;

                case "ZCEX":
                    // Does the client exist.
                    ClientExists(server, serverContext, command, data);
                    return true;

                case "ZCAC":
                    // Get the communication active state.
                    GetClientActive(server, serverContext, command, data);
                    return true;

                case "ZCCT":
                    // Get the communication token.
                    GetClientCommunicationToken(server, serverContext, command, data);
                    return true;

                case "ZCHN":
                    // Get the client host name.
                    GetClientHost(server, serverContext, command, data);
                    return true;

                case "ZHAC":
                    // Get the number of active connections.
                    GetHostActiveConnections(server, serverContext, command, data);
                    return true;

                case "ZHPT":
                    // Get the host index number.
                    GetHostIndex(server, serverContext, command, data);
                    return true;

                case "ZHEX":
                    // Does the host exist.
                    HostExists(server, serverContext, command, data);
                    return true;

                case "ZXCR":
                case "ZCRM":
                    // Remove the client.
                    RemoveClient(server, serverContext, command, data);
                    return true;

                case "ZHRM":
                    // Remove the host.
                    RemoveHost(server, serverContext, command, data);
                    return true;

                case "ZXGH":
                case "ZHST":
                    // Get the host details for the identity.
                    GetHost(server, serverContext, command, data);
                    return true;

                case "Z1GS":
                case "Z2GS":
                case "ZXGS":
                case "ZHSS":
                    // Get the host details for the identities.
                    GetHosts(server, serverContext, command, data);
                    return true;

                case "ZCSA":
                    // Set the client communication active state.
                    SetClientActive(server, serverContext, command, data);
                    return true;

                case "ZCST":
                    // Set the communication token.
                    SetClientCommunicationToken(server, serverContext, command, data);
                    return true;

                case "ZCSH":
                    // Set the client host name.
                    SetClientHost(server, serverContext, command, data);
                    return true;

                case "ZHSC":
                    // Set the number of active connections.
                    SetHostActiveConnections(server, serverContext, command, data);
                    return true;

                case "ZHSP":
                    // Set the host index number.
                    SetHostIndex(server, serverContext, command, data);
                    return true;

                case "ZHSD":
                    // Set the host domain.
                    SetHostDomain(server, serverContext, command, data);
                    return true;

                case "ZHGD":
                    // Get the host domain.
                    GetHostDomain(server, serverContext, command, data);
                    return true;

                case "ZAPT":
                    // Add a new port range.
                    AddPort(server, serverContext, command, data);
                    return true;

                case "ZRPT":
                    // Remove the port.
                    RemovePort(server, serverContext, command, data);
                    return true;

                case "ZRPX":
                    // Remove the port.
                    RemovePortEx(server, serverContext, command, data);
                    return true;

                case "ZRSN":
                    // Sets the port number.
                    SetPortNumber(server, serverContext, command, data);
                    return true;

                case "ZGPT":
                    // Get the ports.
                    GetPorts(server, serverContext, command, data);
                    return true;

                case "ZHHT":
                    // Get the host type.
                    GetHostType(server, serverContext, command, data);
                    return true;

                case "ZHTS":
                    // Set the host type.
                    SetHostType(server, serverContext, command, data);
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Get the host manage URLs for a type.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetManageURLs(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string type = data.Trim();

                // Execute.
                string[] result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string[]>(() =>
                    {
                        return _communicationProvider.GetManageURLs(type);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command +
                        ((result == null || result.Length < 1) ? "" : " " + String.Join("|", result)) + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Get the host manage URLs for a type.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void AddManageURL(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                // Message parts are divided by "|"  
                // Break the string into an array accordingly.
                string[] dataArray = data.Split('|');

                string type = dataArray[0].Trim();
                string urls = dataArray[1].Trim();

                // Execute.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _communicationProvider.AddManageURL(
                            type, (!String.IsNullOrEmpty(urls) ? urls.Split(';') : null));
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Get the host manage URLs for a type.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void RemoveManageURLEx(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                // Message parts are divided by "|"  
                // Break the string into an array accordingly.
                string[] dataArray = data.Split('|');

                string type = dataArray[0].Trim();
                string url = dataArray[1].Trim();

                // Execute.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _communicationProvider.RemoveManageURL(type, url);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Get the host manage URLs for a type.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void RemoveManageURL(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string type = data.Trim();

                // Execute.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _communicationProvider.RemoveManageURL(type);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
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
                string serviceName = server.ServiceName;
                string hostName = string.Empty;
                string active = "false";
                string communicationToken = "";

                if (_isCommunicationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get client data.
                    identifier = dataArray[0].Trim();
                    serviceName = dataArray[1].Trim();
                    hostName = dataArray[2].Trim();
                    active = dataArray[3].Trim();
                    communicationToken = dataArray[4].Trim();
                }
                else
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get client data.
                    hostName = dataArray[0].Trim();
                    active = dataArray[1].Trim();
                    communicationToken = dataArray[2].Trim();
                }

                // Execute.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _communicationProvider.AddClient(identifier, serviceName, hostName,
                            (String.IsNullOrEmpty(active) ? false : Boolean.Parse(active)),
                            (String.IsNullOrEmpty(communicationToken) ? "" : communicationToken));
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Add a new host.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void AddHost(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                // Message parts are divided by "|"  
                // Break the string into an array accordingly.
                string[] dataArray = data.Split('|');

                // Get client data.
                string hostName = dataArray[0].Trim();
                string index = dataArray[1].Trim();
                string domain = dataArray[2].Trim();
                string type = dataArray[3].Trim();
                string activeConnections = dataArray[4].Trim();

                // Execute.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _communicationProvider.AddHost(hostName,
                            (String.IsNullOrEmpty(index) ? 0 : Int32.Parse(index)),
                            (String.IsNullOrEmpty(domain) ? "All" : domain),
                            (String.IsNullOrEmpty(type) ? "" : type),
                            (String.IsNullOrEmpty(activeConnections) ? 0 : Int32.Parse(activeConnections)));
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
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
            string serviceName = server.ServiceName;

            if (_isCommunicationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                if (_isCommunicationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get client data.
                    identifier = dataArray[0].Trim();
                    serviceName = dataArray[1].Trim();
                }

                // Execute.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _communicationProvider.ClientExists(identifier, serviceName);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
        }

        /// <summary>
        /// Get the communication active state.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetClientActive(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string identifier = serverContext.UniqueIdentifier;
            string serviceName = server.ServiceName;

            if (_isCommunicationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                if (_isCommunicationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get client data.
                    identifier = dataArray[0].Trim();
                    serviceName = dataArray[1].Trim();
                }

                // Execute.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _communicationProvider.GetClientActive(identifier, serviceName);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
        }

        /// <summary>
        /// Get the communication token.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetClientCommunicationToken(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string identifier = serverContext.UniqueIdentifier;
            string serviceName = server.ServiceName;

            if (_isCommunicationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                if (_isCommunicationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get client data.
                    identifier = dataArray[0].Trim();
                    serviceName = dataArray[1].Trim();
                }

                // Execute.
                string result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string>(() =>
                    {
                        return _communicationProvider.GetClientCommunicationToken(identifier, serviceName);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command +
                            (String.IsNullOrEmpty(result) ? "" : " " + result) + "\r\n"));
            }
        }

        /// <summary>
        /// Get the client host name.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetClientHost(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string identifier = serverContext.UniqueIdentifier;
            string serviceName = server.ServiceName;

            if (_isCommunicationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                if (_isCommunicationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get client data.
                    identifier = dataArray[0].Trim();
                    serviceName = dataArray[1].Trim();
                }

                // Execute.
                string result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string>(() =>
                    {
                        return _communicationProvider.GetClientHost(identifier, serviceName);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command +
                            (String.IsNullOrEmpty(result) ? "" : " " + result) + "\r\n"));
            }
        }

        /// <summary>
        /// Get the number of active connections.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetHostActiveConnections(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string hostName = data.Trim();

            if (_isCommunicationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Execute.
                int result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<int>(() =>
                    {
                        return _communicationProvider.GetHostActiveConnections(hostName);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
        }

        /// <summary>
        /// Get the host index number.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetHostIndex(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string hostName = data.Trim();

            if (_isCommunicationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Execute.
                int result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<int>(() =>
                    {
                        return _communicationProvider.GetHostIndex(hostName);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
        }

        /// <summary>
        /// Does the host exist.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void HostExists(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string hostName = data.Trim();

            if (_isCommunicationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Execute.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _communicationProvider.HostExists(hostName);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
        }

        /// <summary>
        /// Remove the client.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void RemoveClient(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string identifier = serverContext.UniqueIdentifier;
            string serviceName = server.ServiceName;
            string activeConnections = string.Empty;
            string hostName = string.Empty;

            if (_isCommunicationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                if (_isCommunicationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get client data.
                    identifier = dataArray[0].Trim();
                    serviceName = dataArray[1].Trim();
                    hostName = dataArray[2].Trim();
                    activeConnections = dataArray[3].Trim();
                }
                else
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    hostName = dataArray[0].Trim();
                    activeConnections = dataArray[1].Trim();
                }

                // Execute.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _communicationProvider.RemoveClient(identifier, serviceName, hostName, Int32.Parse(activeConnections));
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
        }

        /// <summary>
        /// Remove the host.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void RemoveHost(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string hostName = data.Trim();

            if (_isCommunicationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Execute.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _communicationProvider.RemoveHost(hostName);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
        }

        /// <summary>
        /// Get the host details for the identities.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetHost(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string identifier = serverContext.UniqueIdentifier;
            string serviceName = server.ServiceName;

            if (_isCommunicationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                if (_isCommunicationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get client data.
                    identifier = dataArray[0].Trim();
                    serviceName = dataArray[1].Trim();
                }

                // Execute.
                string result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string>(() =>
                    {
                        return _communicationProvider.GetHost(identifier, serviceName);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command +
                            (String.IsNullOrEmpty(result) ? "" : " " + result) + "\r\n"));
            }
        }

        /// <summary>
        /// Get the host details for the identities.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetHosts(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string serviceName = server.ServiceName;
                string identifiers = data.Trim();

                if (_isCommunicationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get client data.
                    serviceName = dataArray[0].Trim();
                    identifiers = dataArray[1].Trim();
                }

                // Execute.
                string[] result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string[]>(() =>
                    {
                        return _communicationProvider.GetHosts(identifiers.Split(';'), serviceName);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command +
                        ((result == null || result.Length < 1) ? "" : " " + String.Join("|", result)) + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Set the client communication active state.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void SetClientActive(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string identifier = serverContext.UniqueIdentifier;
                string serviceName = server.ServiceName;
                string active = data.Trim();

                if (_isCommunicationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get client data.
                    identifier = dataArray[0].Trim();
                    serviceName = dataArray[1].Trim();
                    active = dataArray[2].Trim();
                }

                // Execute.
                await Nequeo.Threading.AsyncOperationResult<bool>.RunTask(() =>
                    {
                        _communicationProvider.SetClientActive(identifier, serviceName, Boolean.Parse(active));
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Set the communication token.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void SetClientCommunicationToken(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string identifier = serverContext.UniqueIdentifier;
                string serviceName = server.ServiceName;
                string communicationToken = data.Trim();

                if (_isCommunicationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get client data.
                    identifier = dataArray[0].Trim();
                    serviceName = dataArray[1].Trim();
                    communicationToken = dataArray[2].Trim();
                }

                // Execute.
                await Nequeo.Threading.AsyncOperationResult<bool>.RunTask(() =>
                    {
                        _communicationProvider.SetClientCommunicationToken(identifier, serviceName, communicationToken);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Set the client host name.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void SetClientHost(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string identifier = serverContext.UniqueIdentifier;
                string serviceName = server.ServiceName;
                string hostName = data.Trim();

                if (_isCommunicationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get client data.
                    identifier = dataArray[0].Trim();
                    serviceName = dataArray[1].Trim();
                    hostName = dataArray[2].Trim();
                }

                // Execute.
                await Nequeo.Threading.AsyncOperationResult<bool>.RunTask(() =>
                    {
                        _communicationProvider.SetClientHost(identifier, serviceName, hostName);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Set the number of active connections.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void SetHostActiveConnections(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                // Message parts are divided by "|"  
                // Break the string into an array accordingly.
                string[] dataArray = data.Split('|');

                // Get client data.
                string hostName = dataArray[0].Trim();
                string activeConnections = dataArray[1].Trim();

                // Execute.
                await Nequeo.Threading.AsyncOperationResult<bool>.RunTask(() =>
                    {
                        _communicationProvider.SetHostActiveConnections(hostName,
                            (String.IsNullOrEmpty(activeConnections) ? 0 : Int32.Parse(activeConnections)));
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Set the host index number.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void SetHostIndex(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                // Message parts are divided by "|"  
                // Break the string into an array accordingly.
                string[] dataArray = data.Split('|');

                // Get client data.
                string hostName = dataArray[0].Trim();
                string index = dataArray[1].Trim();

                // Execute.
                await Nequeo.Threading.AsyncOperationResult<bool>.RunTask(() =>
                    {
                        _communicationProvider.SetHostIndex(hostName,
                            (String.IsNullOrEmpty(index) ? 0 : Int32.Parse(index)));
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Set the host domain.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void SetHostDomain(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                // Message parts are divided by "|"  
                // Break the string into an array accordingly.
                string[] dataArray = data.Split('|');

                // Get client data.
                string hostName = dataArray[0].Trim();
                string domain = dataArray[1].Trim();

                // Execute.
                await Nequeo.Threading.AsyncOperationResult<bool>.RunTask(() =>
                    {
                        _communicationProvider.SetHostDomain(hostName,
                            (String.IsNullOrEmpty(domain) ? "All" : domain));
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Get the host domain.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetHostDomain(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string hostName = data.Trim();

            if (_isCommunicationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Execute.
                string result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string>(() =>
                    {
                        return _communicationProvider.GetHostDomain(hostName);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command +
                            (String.IsNullOrEmpty(result) ? "All" : " " + result) + "\r\n"));
            }
        }

        /// <summary>
        /// Add a new port range.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void AddPort(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string serviceName = server.ServiceName;
                string portTypeName = string.Empty;
                string portTypeNumber = string.Empty;

                if (_isCommunicationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get client data.
                    serviceName = dataArray[0].Trim();
                    portTypeName = dataArray[1].Trim();
                    portTypeNumber = dataArray[2].Trim();
                }
                else
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get client data.
                    portTypeName = dataArray[0].Trim();
                    portTypeNumber = dataArray[1].Trim();
                }

                // Assign the port numbers.
                string[] portTypeNumberStr = portTypeNumber.Split(';');
                int[] portTypeNumberInt = new int[portTypeNumberStr.Length];
                for (int i = 0; i < portTypeNumberStr.Length; i++)
                    portTypeNumberInt[i] = Int32.Parse(portTypeNumberStr[i]);

                // Execute.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _communicationProvider.AddPort(serviceName, portTypeName.Split(';'), portTypeNumberInt);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Remove the port.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void RemovePort(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string serviceName = server.ServiceName;

                if (_isCommunicationServer)
                {
                    // Get client data.
                    serviceName = data.Trim();
                }

                // Execute.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _communicationProvider.RemovePort(serviceName);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Remove the port.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void RemovePortEx(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string serviceName = server.ServiceName;
                string portTypeName = data.Trim();

                if (_isCommunicationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get client data.
                    serviceName = dataArray[0].Trim();
                    portTypeName = dataArray[1].Trim();
                }

                // Execute.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _communicationProvider.RemovePort(serviceName, portTypeName);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Sets the port number.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void SetPortNumber(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string serviceName = server.ServiceName;
                string portTypeName = string.Empty;
                string portTypeNumber = string.Empty;

                if (_isCommunicationServer)
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get client data.
                    serviceName = dataArray[0].Trim();
                    portTypeName = dataArray[1].Trim();
                    portTypeNumber = dataArray[2].Trim();
                }
                else
                {
                    // Message parts are divided by "|"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split('|');

                    // Get client data.
                    portTypeName = dataArray[0].Trim();
                    portTypeNumber = dataArray[1].Trim();
                }

                // Execute.
                bool result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
                    {
                        return _communicationProvider.SetPortNumber(serviceName, portTypeName, Int32.Parse(portTypeNumber));
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + " " + result.ToString() + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Get the ports.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetPorts(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                string serviceName = server.ServiceName;
                
                if (_isCommunicationServer)
                {
                    // Get client data.
                    serviceName = data.Trim();
                }

                // Execute.
                string[] result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string[]>(() =>
                    {
                        return _communicationProvider.GetPorts(serviceName);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command +
                        ((result == null || result.Length < 1) ? "" : " " + String.Join("|", result)) + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }

        /// <summary>
        /// Get the host type.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void GetHostType(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            string hostName = data.Trim();

            if (_isCommunicationServer && String.IsNullOrEmpty(data))
            {
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
            }
            else
            {
                // Execute.
                string result = await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string>(() =>
                    {
                        return _communicationProvider.GetHostType(hostName);
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command +
                            (String.IsNullOrEmpty(result) ? "" : " " + result) + "\r\n"));
            }
        }

        /// <summary>
        /// Set the host manage URL.
        /// </summary>
        /// <param name="server">The server sending the data.</param>
        /// <param name="serverContext">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data from the client.</param>
        private async void SetHostType(Net.Sockets.IServer server, Net.Sockets.IServerContext serverContext, string command, string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                // Message parts are divided by "|"  
                // Break the string into an array accordingly.
                string[] dataArray = data.Split('|');

                // Get client data.
                string hostName = dataArray[0].Trim();
                string type = dataArray[1].Trim();

                // Execute.
                await Nequeo.Threading.AsyncOperationResult<bool>.RunTask(() =>
                    {
                        _communicationProvider.SetHostType(hostName,
                            (String.IsNullOrEmpty(type) ? "" : type));
                    });

                // Send to the data.
                serverContext.SentFromServer(
                    Encoding.ASCII.GetBytes(command + "\r\n"));
            }
            else
                // Send an error to the server.
                serverContext.SentFromServer(Encoding.ASCII.GetBytes("ERRO No data" + "\r\n"));
        }
    }
}
