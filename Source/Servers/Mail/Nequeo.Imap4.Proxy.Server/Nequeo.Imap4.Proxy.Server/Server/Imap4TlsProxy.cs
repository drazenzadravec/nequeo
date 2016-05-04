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
using System.Net.Sockets;
using System.Threading;

using Nequeo.Net.Common;
using Nequeo.Net.Connection;
using Nequeo.Handler;
using Nequeo.Net.Configuration;

namespace Nequeo.Net.Server
{
    /// <summary>
    /// Class that controls the proxy imap4 host server.
    /// </summary>
    public class Imap4TlsProxyServer : ServiceBase, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public Imap4TlsProxyServer()
        {
        }

        /// <summary>
        /// Constructor for the current class, with port parameter.
        /// </summary>
        /// <param name="listeningPort">Contains the port to listen on.</param>
        public Imap4TlsProxyServer(int listeningPort)
        {
            _listenPort = listeningPort;
        }
        #endregion

        #region Private Fields
        private int _listenPort = 9936;
        private string _hostName = string.Empty;
        private string _imap4ServerName = string.Empty;
        //private string _remoteHost = "localhost";
        private TcpListener _listener;

        private Imap4ConnectionAdapter _connection = null;

        private int _timeOut = -1;
        private int _clientIndex = -1;
        private int _clientCount = 0;
        private int _maxNumClients = 30;
        private Imap4TlsProxyConnection[] _client = null;

        // Looks for connection avaliability.
        private AutoResetEvent _connAvailable =
            new AutoResetEvent(false);
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the name of the host to use.
        /// This host name is from the application configuration file.
        /// This property is case sensitive.
        /// </summary>
        public String HostName
        {
            get { return _hostName; }
            set { _hostName = value; }
        }

        /// <summary>
        /// Gets sets, the name of the imap4 server to use.
        /// This host name is from the application configuration file.
        /// This property is case sensitive.
        /// </summary>
        public String Imap4ServerName
        {
            get { return _imap4ServerName; }
            set { _imap4ServerName = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Starts listening on the specified port.
        /// </summary>
        public void StartListen()
        {
            try
            {
                // Get the imap4 server information from
                // the configuration file, the host and
                // port of the imap4 server is located in
                // the default section, this data is used
                // to connect to the imap4 server.
                GetImap4ServerHost();

                // Get the listening port from the configuration
                // file, if no port specified then default or
                // the current port set will be used.
                GetListeningPort();

                // Get the maximum number of clients from the configuration
                // file, if no value is specified then default or
                // the current value will be used.
                GetMaxNumClients();

                // Get the client idle time out from the configuration
                // file, if no value is specified then default or
                // the current value will be used.
                GetClientTimeOut();

                // Get the local end point, the server will listen
                // on only the first IP address assigned.
                //IPEndPoint endPoint = new IPEndPoint(Dns.GetHostEntry(remoteHost).AddressList[0], listenPort);

                // Create a new tcp listener server,
                // and start the server.
                _listener = new TcpListener(System.Net.IPAddress.Any, _listenPort);
                _listener.Start();

                // While the server is alive accept in-comming client
                // connections.
                do
                {
                    // Do not allow any more clients
                    // if maximum is reached.
                    if (_clientCount < (_maxNumClients - 1))
                    {
                        // Find the next available
                        // connection within the list.
                        for (int i = 0; i < _client.Count(); i++)
                        {
                            if (_client[i] == null)
                            {
                                _clientIndex = i;
                                break;
                            }
                        }

                        // Create a new client connection handler for the current
                        // tcp client attempting to connect to the server. Creates
                        // a new channel from the client to the server.
                        _client[_clientIndex] =
                            new Imap4TlsProxyConnection(_listener.AcceptTcpClient(), _connection);

                        // Assign the current index.
                        _client[_clientIndex].ClientIndex = _clientIndex;

                        // if a time out has been set.
                        if (_timeOut > 0)
                            _client[_clientIndex].TimeOut = _timeOut;

                        // Create a new client data receive handler, this event
                        // handles commands from the current client.
                        _client[_clientIndex].OnDataReceived +=
                            new TlsProxyImap4ReceiveHandler(client_OnDataReceived);

                        // Increment the count.
                        Interlocked.Increment(ref _clientCount);
                    }
                    else
                    {
                        base.Write("Imap4TlsProxyServer", "StartListen", "Maximum number of client connections has been reached.",
                            120, WriteTo.EventLog, LogType.Error);

                        // Blocks the current thread until a
                        // connection becomes available.
                        _connAvailable.WaitOne();
                    }

                } while (true);
            }
            catch (SocketException see)
            {
                base.Write("Imap4TlsProxyServer", "StartListen", see.Message,
                    121, WriteTo.EventLog, LogType.Error);
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("Imap4TlsProxyServer", "StartListen", e.Message,
                    121, WriteTo.EventLog, LogType.Error);
            }
            finally
            {
                if (_listener != null)
                    _listener.Stop();

                _listener = null;
            }
        }

        /// <summary>
        /// Stops listening.
        /// </summary>
        public void StopListen()
        {
            try
            {
                if (_listener != null)
                    _listener.Stop();
            }
            catch (SocketException see)
            {
                base.Write("Imap4TlsProxyServer", "StopListen", see.Message,
                    158, WriteTo.EventLog, LogType.Error);
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("Imap4TlsProxyServer", "StopListen", e.Message,
                    158, WriteTo.EventLog, LogType.Error);
            }
            finally
            {
                _listener = null;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Get the client time out from the configuration
        /// file, if no value is specified then default or
        /// the current value will be used.
        /// </summary>
        private void GetClientTimeOut()
        {
            try
            {
                // If no host name set then
                // use the defualt values.
                if (String.IsNullOrEmpty(_hostName))
                {
                    // Create a new default host type
                    // an load the values from the configuration
                    // file into the default host type.
                    ProxyImap4ServerDefaultHost defaultHost =
                        (ProxyImap4ServerDefaultHost)System.Configuration.ConfigurationManager.GetSection(
                            "ProxyImap4ServerGroup/ProxyImap4ServerDefaultHost");

                    // If the port is greater than zero then
                    // assign the port number.
                    if (defaultHost.HostSection.ClientTimeOutAttribute > 0)
                        _timeOut = defaultHost.HostSection.ClientTimeOutAttribute;
                }
                else
                {
                    // Create a new host type
                    // an load the values from the configuration
                    // file into the host type.
                    ProxyImap4ServerHosts hosts =
                        (ProxyImap4ServerHosts)System.Configuration.ConfigurationManager.GetSection(
                            "ProxyImap4ServerGroup/ProxyImap4ServerHosts");

                    // If the port is greater than zero then
                    // assign the port number.
                    if (hosts.HostSection[_hostName].ClientTimeOutAttribute > 0)
                        _timeOut = hosts.HostSection[_hostName].ClientTimeOutAttribute;
                }
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("Imap4TlsProxyServer", "GetClientTimeOut", e.Message,
                    246, WriteTo.EventLog, LogType.Error);
            }
        }

        /// <summary>
        /// Get the maximum number of clients from the configuration
        /// file, if no value is specified then default or
        /// the current value will be used.
        /// </summary>
        private void GetMaxNumClients()
        {
            try
            {
                // If no host name set then
                // use the defualt values.
                if (String.IsNullOrEmpty(_hostName))
                {
                    // Create a new default host type
                    // an load the values from the configuration
                    // file into the default host type.
                    ProxyImap4ServerDefaultHost defaultHost =
                        (ProxyImap4ServerDefaultHost)System.Configuration.ConfigurationManager.GetSection(
                            "ProxyImap4ServerGroup/ProxyImap4ServerDefaultHost");

                    // If the port is greater than zero then
                    // assign the port number.
                    if (defaultHost.HostSection.MaxNumClientsAttribute > 0)
                        _maxNumClients = defaultHost.HostSection.MaxNumClientsAttribute;
                }
                else
                {
                    // Create a new host type
                    // an load the values from the configuration
                    // file into the host type.
                    ProxyImap4ServerHosts hosts =
                        (ProxyImap4ServerHosts)System.Configuration.ConfigurationManager.GetSection(
                            "ProxyImap4ServerGroup/ProxyImap4ServerHosts");

                    // If the port is greater than zero then
                    // assign the port number.
                    if (hosts.HostSection[_hostName].MaxNumClientsAttribute > 0)
                        _maxNumClients = hosts.HostSection[_hostName].MaxNumClientsAttribute;
                }

                // Create a new client array.
                _client = new Imap4TlsProxyConnection[_maxNumClients];
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("Imap4TlsProxyServer", "GetMaxNumClients", e.Message,
                    246, WriteTo.EventLog, LogType.Error);
            }
        }

        /// <summary>
        /// Get the imap4 server information from
        /// the configuration file, the host and
        /// port of the imap4 server is located in
        /// the default section, this data is used
        /// to connect to the imap4 server.
        /// </summary>
        private void GetImap4ServerHost()
        {
            try
            {
                // Create a new connection adapter.
                _connection = new Imap4ConnectionAdapter();

                // If no host name set then
                // use the defualt values.
                if (String.IsNullOrEmpty(_imap4ServerName))
                {
                    // Create a new default host type
                    // an load the values from the configuration
                    // file into the default host type.
                    ProxyImap4ServerDefaultHost defaultHost =
                        (ProxyImap4ServerDefaultHost)System.Configuration.ConfigurationManager.GetSection(
                            "ProxyImap4ServerGroup/ProxyImap4ServerDefaultHost");

                    // If the port is greater than zero then
                    // assign the port number.
                    if (defaultHost.HostSection.PortAttribute > 0)
                        _connection.Port = defaultHost.HostSection.PortAttribute;

                    // Get the imap4 server.
                    _connection.Server = defaultHost.HostSection.HostAttribute;
                }
                else
                {
                    // Create a new host type
                    // an load the values from the configuration
                    // file into the host type.
                    ProxyImap4ServerHosts hosts =
                        (ProxyImap4ServerHosts)System.Configuration.ConfigurationManager.GetSection(
                            "ProxyImap4ServerGroup/ProxyImap4ServerHosts");

                    // If the port is greater than zero then
                    // assign the port number.
                    if (hosts.HostSection[_imap4ServerName].PortAttribute > 0)
                        _connection.Port = hosts.HostSection[_imap4ServerName].PortAttribute;

                    // Get the imap4 server.
                    _connection.Server = hosts.HostSection[_imap4ServerName].HostAttribute;
                }
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("Imap4TlsProxyServer", "GetListeningPort", e.Message,
                    246, WriteTo.EventLog, LogType.Error);
            }
        }

        /// <summary>
        /// Get the listening port from the configuration
        /// file, if no port specified then default or
        /// the current port will be used.
        /// </summary>
        private void GetListeningPort()
        {
            try
            {
                // If no host name set then
                // use the defualt values.
                if (String.IsNullOrEmpty(_hostName))
                {
                    // Create a new default host type
                    // an load the values from the configuration
                    // file into the default host type.
                    ProxyImap4ServerDefaultHost defaultHost =
                        (ProxyImap4ServerDefaultHost)System.Configuration.ConfigurationManager.GetSection(
                            "ProxyImap4ServerGroup/ProxyImap4ServerDefaultHost");

                    // If the port is greater than zero then
                    // assign the port number.
                    if (defaultHost.HostSection.PortAttribute > 0)
                        _listenPort = defaultHost.HostSection.PortAttribute;
                }
                else
                {
                    // Create a new host type
                    // an load the values from the configuration
                    // file into the host type.
                    ProxyImap4ServerHosts hosts =
                        (ProxyImap4ServerHosts)System.Configuration.ConfigurationManager.GetSection(
                            "ProxyImap4ServerGroup/ProxyImap4ServerHosts");

                    // If the port is greater than zero then
                    // assign the port number.
                    if (hosts.HostSection[_hostName].PortAttribute > 0)
                        _listenPort = hosts.HostSection[_hostName].PortAttribute;
                }
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("Imap4TlsProxyServer", "GetListeningPort", e.Message,
                    246, WriteTo.EventLog, LogType.Error);
            }
        }

        /// <summary>
        /// Processes all in-comming client command requests.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="dataReceived">The data received from the client.</param>
        private void client_OnDataReceived(Imap4TlsProxyConnection sender, string dataReceived)
        {
            try
            {
                // Decrypt the data recived from the client.
                string receivedData = dataReceived.Trim();
                string command = string.Empty;

                // Get specific commands from the client.
                if (receivedData.ToUpper().IndexOf("LOGOUT") > -1)
                    command = "LOGOUT";
                else if (receivedData.ToUpper().IndexOf("LOGIN") > -1)
                    command = "LOGIN";
                else if (receivedData.ToUpper().IndexOf("STARTTLS") > -1)
                    command = "STARTTLS";
                else if (receivedData.ToUpper().IndexOf("CAPABILITY") > -1)
                    command = "CAPABILITY";
                else if (receivedData.ToUpper().IndexOf("AUTHENTICATE") > -1)
                    command = "AUTHENTICATE";
                else if (receivedData.ToUpper().IndexOf("ENDC") > -1)
                    command = "ENDC";
                else
                    command = receivedData;

                // Delay.
                System.Threading.Thread.Sleep(10);

                // Process the command.
                switch (command.ToUpper())
                {
                    case "LOGOUT":
                        // Close the client connection.
                        Disconnect(sender, receivedData);
                        break;

                    case "LOGIN":
                        // User name.
                        Connect(sender, receivedData);
                        break;

                    case "CAPABILITY":
                        if (!sender.TlsNegotiated)
                        {
                            // Send TLS capable.
                            string dataArrayCapable = receivedData.Trim();
                            string prefixCapable = string.Empty;

                            // Get the prefix of the current 
                            // message request.
                            int endPrefixCapable = dataArrayCapable.IndexOf(" ");
                            prefixCapable = dataArrayCapable.Substring(0, endPrefixCapable).Trim();

                            sender.SendNonSSLClientCommand("* CAPABILITY IMAP4 IMAP4rev1 CHILDREN IDLE QUOTA SORT ACL NAMESPACE STARTTLS LOGINDISABLED");
                            sender.SendNonSSLClientCommand(prefixCapable + " OK CAPABILITY completed");
                        }
                        else
                            // Send the command to the imap4 server.
                            sender.SendSocketCommand(receivedData);
                        
                        break;

                    case "STARTTLS":
                        // Start a TLS connection.
                        SslConnection(sender, receivedData);
                        break;

                    case "AUTHENTICATE":
                        // Start a SASL connection
                        string dataArray = receivedData.Trim();
                        string prefix = string.Empty;

                        // Get the prefix of the current 
                        // message request.
                        int endPrefix = dataArray.IndexOf(" ");
                        prefix = dataArray.Substring(0, endPrefix).Trim();

                        // Sent to the client that this
                        // command is not supported.
                        sender.SendNonSSLClientCommand(prefix + " NO Not supported");
                        break;

                    case "ENDC":
                        // End the client connection.
                        EndConnection(sender);
                        break;

                    default:
                        // An unknown command sent.
                        //Unknown(sender);
                        sender.SendSocketCommand(receivedData);
                        break;
                }
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("Imap4TlsProxyServer", "client_OnDataReceived", e.Message,
                    292, WriteTo.EventLog, LogType.Error);

                // Reply to client internal server error command.
                ReplyToSender(sender, "500 Error");
            }
        }

        /// <summary>
        /// Reply to the current client.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="command">The command to send to the client.</param>
        private void ReplyToSender(Imap4TlsProxyConnection sender, string command)
        {
            sender.SendCommand(command);
        }

        /// <summary>
        /// Unknown command received.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        private void Unknown(Imap4TlsProxyConnection sender)
        {
            if (sender.Connected)
                // Reply to client with unknown command.
                ReplyToSender(sender, "-ERR Unknown command");
            else
                // If an unknown command is sent when the channel is opened
                // but the user has not supplied valid credentials then close
                // the socket channel to the client. This is needed when a
                // client opens a socket channel and starts sending a DOS
                // attack.
                Disconnect(sender, sender.Prefix + " LOGOUT");
        }

        /// <summary>
        /// Disconnect the client from the server, close the channel.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="data">The data from the client.</param>
        private void Disconnect(Imap4TlsProxyConnection sender, string data)
        {
            // Add custom validation here.
            // Get the current data.
            string dataArray = data.Trim();
            string prefix = string.Empty;

            // Get the prefix of the current 
            // message request.
            int endPrefix = dataArray.IndexOf(" ");
            prefix = dataArray.Substring(0, endPrefix).Trim();
            sender.Prefix = prefix;

            // Disconnect the client.
            sender.Connected = false;
            sender.Disconnect(data);
            sender = null;
        }

        /// <summary>
        /// Ends the current client connection.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        private void EndConnection(Imap4TlsProxyConnection sender)
        {
            // Release the current client connection
            // resources and make avaliable a new connection.
            if (_client[sender.ClientIndex] != null)
            {
                _client[sender.ClientIndex].Dispose();
                _client[sender.ClientIndex] = null;

                // Decrement the count.
                Interlocked.Decrement(ref _clientCount);

                // Signal to the blocking handler
                // to un-block.
                _connAvailable.Set();
            }
        }

        /// <summary>
        /// Connect the client to the server.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="data">The data from the client.</param>
        private void Connect(Imap4TlsProxyConnection sender, string data)
        {
            try
            {
                // Add custom validation here.
                // Get the current data.
                string dataArray = data.Trim();
                string userPassword = string.Empty;
                string prefix = string.Empty;

                // Start and end of the login index.
                int start = dataArray.ToUpper().IndexOf("LOGIN") + 5;
                userPassword = dataArray.Substring(start);

                // Get the prefix of the current 
                // message request.
                int endPrefix = dataArray.IndexOf(" ");
                prefix = dataArray.Substring(0, endPrefix).Trim();

                // Get the user name and passord
                // of the client creadentials.
                sender.ConnectionName = userPassword;
                sender.Prefix = prefix;
                sender.Connected = true;

                // Send the command to the imap4 server.
                sender.SendSocketCommand(data);
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("Imap4TlsProxyServer", "Connect", e.Message,
                    382, WriteTo.EventLog, LogType.Error);

                // Reply to client internal server error command.
                ReplyToSender(sender, "500 Error");
            }
        }

        /// <summary>
        /// Request an ssl connection to the server.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="data">The data from the client.</param>
        private void SslConnection(Imap4TlsProxyConnection sender, string data)
        {
            try
            {
                // Add custom validation here.
                // Get the current data.
                string dataArray = data.Trim();
                string prefix = string.Empty;

                // Get the prefix of the current 
                // message request.
                int endPrefix = dataArray.IndexOf(" ");
                prefix = dataArray.Substring(0, endPrefix).Trim();
                sender.Prefix = prefix;

                // Create an ssl connection from the socket.
                // Reply to the client that a ssl connection
                // is being created.
                sender.InitializingSSLConnection(prefix + " OK Begin TLS negotiation now");
                sender.TlsNegotiated = true;
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("Imap4TlsProxyServer", "SslConnection", e.Message,
                    382, WriteTo.EventLog, LogType.Error);

                // Reply to client internal server error command.
                ReplyToSender(sender, "500 -ERROR");
            }
        }
        #endregion
    }
}
