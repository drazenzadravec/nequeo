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
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

using Nequeo.Net.Common;
using Nequeo.Handler;
using Nequeo.Security;
using Nequeo.Net.Configuration;

namespace Nequeo.Net.Connection
{
    /// <summary>
    /// Delegate that handles commands received from the client.
    /// </summary>
    /// <param name="sender">The current object sending the data.</param>
    /// <param name="dataReceived">The command data send by the client.</param>
    internal delegate void SslProxyPop3ReceiveHandler(Pop3SslProxyConnection sender, string dataReceived);

    /// <summary>
    /// Class that contains the data for the current tcp client 
    /// connected to the server.
    /// </summary>
    internal class Pop3SslProxyConnection : ServiceBase, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="tcpClient">The tcp client channel connection from server to client.</param>
        /// <param name="connection">The connection adapter used to connect to the server.</param>
        public Pop3SslProxyConnection(TcpClient tcpClient, Pop3ConnectionAdapter connection)
        {
            try
            {
                // The tcp client to server channel.
                // Assign the network stream from the client
                // channel stream.
                _tcpClient = tcpClient;
                _connection = connection;
                _sslStream = new SslStream(tcpClient.GetStream());

                // Certificate file containing a public key
                // and private key for secure data transfer.
                X509Certificate2 certificate = GetServerCredentials();

                // Load the certificate into the
                // secure stream used for secure communication.
                _sslStream.AuthenticateAsServer(certificate, false, SslProtocols.Ssl3, false);

                // Create a new state object.
                ServerSocketState state = new ServerSocketState();
                state.SslStream = _sslStream;

                // This starts the asynchronous read thread. 
                // The data will be saved into readBuffer.
                _sslStream.BeginRead(_readBuffer, 0, READ_BUFFER_SIZE,
                    new AsyncCallback(DataReceiver), state);

                // Create a new pop3 client and 
                // attempt to make a connection
                // get the response from the server.
                bool ret = GetSocket();

                // If return false.
                if (!ret)
                    // Write an error response back to
                    // the pop3 client.
                    _sslStream.Write(Encoding.ASCII.GetBytes("-ERR" + "\r\n"));
                else
                {
                    // Create a new state object.
                    ServerSocketState stateSocket = new ServerSocketState();
                    stateSocket.EmailMessage = new StringBuilder();
                    stateSocket.SslStream = _sslStream;
                    stateSocket.Socket = _socket;

                    // Start receiving data asynchrounusly.
                    _socket.BeginReceive(_receiveBuffer, 0, READ_BUFFER_SIZE,
                        SocketFlags.None, new AsyncCallback(ReceiveCallback), stateSocket);
                }
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("Pop3SslProxyConnection", "Constructor", e.Message,
                    65, WriteTo.EventLog, LogType.Error);
            }
        }
        #endregion

        #region Constants
        private const int READ_BUFFER_SIZE = 8192;
        private const int WRITE_BUFFER_SIZE = 8192;
        #endregion

        #region Private Fields
        private TcpClient _tcpClient;
        private SslStream _sslStream;
        private byte[] _readBuffer = new byte[READ_BUFFER_SIZE];
        private byte[] _writeBuffer = new byte[WRITE_BUFFER_SIZE];

        private Socket _socket = null;
        private Pop3ConnectionAdapter _connection = null;
        private byte[] _receiveBuffer = new byte[READ_BUFFER_SIZE];
        private byte[] _sendBuffer = new byte[WRITE_BUFFER_SIZE];

        private Timer _timeOut = null;
        private int _timeOutInterval = -1;

        private string _connectionName = string.Empty;
        private bool _clientConnected = false;

        private long _readLoopCount = 0;
        private int _clientIndex = 0;
        #endregion

        #region Public Events
        /// <summary>
        /// The on data received event handler.
        /// </summary>
        public event SslProxyPop3ReceiveHandler OnDataReceived;
        #endregion

        #region Public Properties
        /// <summary>
        /// Sets, the client time out interval.
        /// </summary>
        public int TimeOut
        {
            set
            {
                _timeOutInterval = value;
                if (_timeOutInterval > -1)
                {
                    _timeOut = new Timer(ClientTimedOut, null,
                        new TimeSpan(0, _timeOutInterval, 0),
                        new TimeSpan(0, _timeOutInterval, 0));
                }
            }
        }

        /// <summary>
        /// Gets sets, the current client index.
        /// </summary>
        public int ClientIndex
        {
            get { return _clientIndex; }
            set { _clientIndex = value; }
        }

        /// <summary>
        /// Gets sets, the unique user name
        /// currenlty connected.
        /// </summary>
        public string ConnectionName
        {
            get { return _connectionName; }
            set { _connectionName = value; }
        }

        /// <summary>
        /// Gets sets, current connection status
        /// for the current user.
        /// </summary>
        public bool Connected
        {
            get { return _clientConnected; }
            set { _clientConnected = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Disconnects the current client.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (_tcpClient != null)
                {
                    _tcpClient.Client.Shutdown(SocketShutdown.Both);
                    _tcpClient.Client.Disconnect(false);
                    _tcpClient.Close();
                    _sslStream.Close();
                }

                _tcpClient = null;
                _sslStream = null;

                if (_socket != null)
                {
                    // Send a quit command to the server.
                    SendSocketCommand("QUIT");
                    _socket.Close();
                    _socket = null;
                }
            }
            catch { }

            // Send a single to end the connection.
            if (OnDataReceived != null)
                OnDataReceived(this, "ENDC");
        }

        /// <summary>
        /// Sends a command to the pop3 server.
        /// </summary>
        /// <param name="data"></param>
        public void SendSocketCommand(string data)
        {
            try
            {
                if (_socket != null)
                    // Send the command to the server.
                    _socket.Send(Encoding.ASCII.GetBytes(data + "\r\n"));
            }
            catch { }
        }

        /// <summary>
        /// Sends a command to the client.
        /// </summary>
        /// <param name="data">The command and data to send to the client.</param>
        public void SendCommand(string data)
        {
            WriteCommand(data, _sslStream);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Disconnects the current client after the time out
        /// interval has elapsed.
        /// </summary>
        /// <param name="state">A passed object state.</param>
        private void ClientTimedOut(object state)
        {
            if (_timeOut != null)
                _timeOut.Dispose();

            Disconnect();
        }

        /// <summary>
        /// Data received asynchronus result method, all client commands
        /// are processed through this asynchronus result method.
        /// </summary>
        /// <param name="ar">The current asynchronus result.</param>
        private void DataReceiver(IAsyncResult ar)
        {
            int bytesRead = 0;

            // Get the current network stream from the
            // asynchronus result state object.
            ServerSocketState state = (ServerSocketState)ar.AsyncState;
            SslStream nStream = state.SslStream;

            try
            {
                // Finish asynchronous read into readBuffer 
                // and get number of bytes read.
                lock (nStream)
                    bytesRead = nStream.EndRead(ar);

                if (bytesRead > 0)
                {
                    // Convert the byte array the message command.
                    // Trigger the data receiver delegate and send
                    // the command and data to the host.
                    try
                    {
                        string command = Encoding.ASCII.GetString(_readBuffer, 0, bytesRead);
                        OnDataReceived(this, command);

                        // If the time out control has been created
                        // then reset the timer.
                        if (_timeOut != null)
                            _timeOut.Change(
                                new TimeSpan(0, _timeOutInterval, 0),
                                new TimeSpan(0, _timeOutInterval, 0));
                    }
                    catch { }
                }
                else
                {
                    // If the connection is closed
                    // then throw a new exception.
                    if (!_tcpClient.Connected)
                        throw new Exception("Connection has closed.");

                    // Start counting loops.
                    _readLoopCount++;

                    // If count greater than
                    // 10 then throw exception
                    // and close the connection.
                    if (_readLoopCount > 4)
                        throw new Exception("Connection has been disconnected, read loop detected. " +
                            "Connection has timed out or, may occur when client calls Shutdown(SocketShutdown.Both)");
                }

                // Start a new asynchronous read into readBuffer.
                // more data maybe present. This will wait for more
                // data from the client.
                lock (nStream)
                    nStream.BeginRead(_readBuffer, 0, READ_BUFFER_SIZE,
                        new AsyncCallback(DataReceiver), state);
            }
            catch (IOException ioe)
            {
                // Handles the forced disconnection such as
                // exiting a client program without closing
                // the connection.
                string error = ioe.Message;

                // On any error close the connection.
                Disconnect();
            }
            catch (ObjectDisposedException ode)
            {
                // Handles the exception when the client
                // closes the current network channel
                // and disposes of the current connection
                // to client class, this object.
                string error = ode.Message;

                // On any error close the connection.
                Disconnect();
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("Pop3SslProxyConnection", "DataReceiver", e.Message,
                    243, WriteTo.EventLog, LogType.Error);

                // On any error close the connection.
                Disconnect();
            }
        }

        /// <summary>
        /// Receive call back asynchrounus result.
        /// Handles the receiving of the email message.
        /// </summary>
        /// <param name="ar">The current asynchronus result.</param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            int bytesRead = 0;

            // Get the current network stream from the
            // asynchronus result state object.
            ServerSocketState stateSocket = (ServerSocketState)ar.AsyncState;
            SslStream sslStream = stateSocket.SslStream;
            Socket nStream = stateSocket.Socket;

            try
            {
                lock (nStream)
                    // End the current asynchrounus
                    // read operation.
                    bytesRead = nStream.EndReceive(ar);

                if (bytesRead > 0)
                    // Decode the current buffer and add the new data
                    // to the message string builder.
                    sslStream.Write(_receiveBuffer, 0, bytesRead);

                // Start a new asynchronous read into readBuffer.
                // more data maybe present. This will wait for more
                // data from the client.
                lock (nStream)
                    nStream.BeginReceive(_receiveBuffer, 0, READ_BUFFER_SIZE,
                        SocketFlags.None, new AsyncCallback(ReceiveCallback), stateSocket);
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("Pop3SslProxyConnection", "ReceiveCallback", e.Message,
                    333, WriteTo.EventLog, LogType.Error);
            }
        }

        /// <summary>
        /// Sends a command and data to the current client.
        /// </summary>
        /// <param name="command">The command and data to write.</param>
        /// <param name="stream">The current client ssl stream.</param>
        private void WriteCommand(string command, SslStream stream)
        {
            StreamWriter writer = null;

            try
            {
                // Write the command to the client.
                lock (stream)
                {
                    writer = new StreamWriter(stream);
                    writer.Write(command + "\r\n");
                    writer.Flush();
                    writer = null;
                }
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("Pop3SslProxyConnection", "WriteCommand", e.Message,
                    367, WriteTo.EventLog, LogType.Error);
            }
            finally
            {
                if (writer != null)
                    writer = null;
            }
        }

        /// <summary>
        /// Get the pop3 client socket stream.
        /// </summary>
        /// <returns>True if connected else false.</returns>
        private bool GetSocket()
        {
            bool ret = false;

            try
            {
                IPHostEntry hostEntry = null;

                // Get host related information.
                hostEntry = Dns.GetHostEntry(_connection.Server);

                // Loop through the AddressList to obtain the supported 
                // AddressFamily. This is to avoid an exception that 
                // occurs when the host IP Address is not compatible 
                // with the address family 
                // (typical in the IPv6 case).
                foreach (IPAddress address in hostEntry.AddressList)
                {
                    // Get the current server endpoint for
                    // the current address.
                    IPEndPoint endPoint = new IPEndPoint(address, _connection.Port);

                    // Create a new client socket for the
                    // current endpoint.
                    Socket tempSocket = new Socket(endPoint.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);

                    // Connect to the server with the
                    // current end point.
                    try
                    {
                        tempSocket.Connect(endPoint);
                    }
                    catch { }

                    // If this connection succeeded then
                    // asiign the client socket and
                    // break put of the loop.
                    if (tempSocket.Connected)
                    {
                        // A client connection has been found.
                        // Break out of the loop.
                        _socket = tempSocket;
                        ret = true;
                        break;
                    }
                    else continue;
                }

                return ret;
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("Pop3SslProxyConnection", "GetSocket", e.Message,
                    422, WriteTo.EventLog, LogType.Error);

                return false;
            }
        }

        /// <summary>
        /// Get the server credentials from the certificate store or file.
        /// </summary>
        /// <returns>The x509 certificate else null.</returns>
        private X509Certificate2 GetServerCredentials()
        {
            X509Certificate2 certificate = null;

            try
            {
                // Create a new default host type
                // an load the values from the configuration
                // file into the default host type.
                ProxyPop3ServerDefaultHost defaultHost =
                    (ProxyPop3ServerDefaultHost)System.Configuration.ConfigurationManager.GetSection(
                        "ProxyPop3ServerGroup/ProxyPop3ServerDefaultHost");

                // Make sure the section is defined.
                if (defaultHost == null)
                    throw new Exception("Configuration section ProxyPop3ServerDefaultHost has not been defined.");

                // Get the server credetials element.
                ProxyPop3ServerCredentialsElement serverCredentials = defaultHost.ServerCredentialsSection;
                if (serverCredentials == null)
                    throw new Exception("Configuration element ServerCredentials has not been defined.");

                // If using the certificate store.
                if (serverCredentials.UseCertificateStore)
                {
                    // Get the certificate from the store.
                    ProxyPop3ServerCredentialsStoreElement certificateStore = serverCredentials.CertificateStore;
                    if (certificateStore == null)
                        throw new Exception("Configuration element CertificateStore has not been defined.");

                    // Get the certificate refrence details from the certificate store.
                    certificate = X509Certificate2Store.GetCertificate(
                        certificateStore.StoreName,
                        certificateStore.StoreLocation,
                        certificateStore.X509FindType,
                        certificateStore.FindValue,
                        false);
                }
                else
                {
                    // Get the certificate path
                    ProxyPop3ServerCredentialsPathElement certificatePath = serverCredentials.CertificatePath;
                    if (certificatePath == null)
                        throw new Exception("Configuration element CertificatePath has not been defined.");

                    // Get the certificate path details and create
                    // the x509 certificate reference.
                    certificate = X509Certificate2Store.GetCertificate(certificatePath.Path, certificatePath.Password);
                }
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("Pop3SslProxyConnection", "GetServerCredentials", e.Message,
                    553, WriteTo.EventLog, LogType.Error);
            }
            
            // Return the certificate.
            return certificate;
        }
        #endregion
    }
}
