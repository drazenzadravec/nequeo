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
using System.IO;
using System.Diagnostics;
using System.Globalization;

using Nequeo.Net;
using Nequeo.Net.FileTransfer.Common;
using Nequeo.Net.FileTransfer.Connection;
using Nequeo.Handler;
using Nequeo.Net.FileTransfer.Configuration;
using Nequeo.Invention;
using Nequeo.Security;

namespace Nequeo.Net.FileTransfer.Server
{
    /// <summary>
    /// Class that controls the file transfer host server.
    /// </summary>
    public class FileTransferServer : ServiceBase, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public FileTransferServer()
        {
        }

        /// <summary>
        /// Constructor for the current class, with port parameter.
        /// </summary>
        /// <param name="listeningPort">Contains the port to listen on.</param>
        public FileTransferServer(int listeningPort)
        {
            listenPort = listeningPort;
        }
        #endregion

        #region Private Fields
        private int listenPort = 2766;
        private string hostName = string.Empty;
        //private string remoteHost = "localhost";
        private TcpListener listener;

        private int _timeOut = -1;
        private int _clientIndex = -1;
        private int _clientCount = 0;
        private int _maxNumClients = 30;
        private FileTransferConnection[] _client = null;

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
            get { return hostName; }
            set { hostName = value; }
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
                listener = new TcpListener(System.Net.IPAddress.Any, listenPort);
                listener.Start();

                // While the server is alive accept in-comming client
                // connections.
                do
                {
                    // Do not allow any more clients
                    // if maximum is reached.
                    if (_clientCount < _maxNumClients)
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
                            new FileTransferConnection(listener.AcceptTcpClient());

                        // Assign the current index.
                        _client[_clientIndex].ClientIndex = _clientIndex;

                        // if a time out has been set.
                        if (_timeOut > 0)
                            _client[_clientIndex].TimeOut = _timeOut;

                        // Create a new client data receive handler, this event
                        // handles commands from the current client.
                        _client[_clientIndex].OnDataReceived +=
                            new FileTransferReceiveHandler(client_OnDataReceived);

                        // Increment the count.
                        Interlocked.Increment(ref _clientCount);
                    }
                    else
                    {
                        base.Write("FileTransferServer", "StartListen", "Maximum number of client connections has been reached.",
                            120, WriteTo.EventLog, LogType.Error);

                        // Blocks the current thread until a
                        // connection becomes available.
                        _connAvailable.WaitOne();
                    }

                } while (true);
            }
            catch (SocketException see)
            {
                base.Write("FileTransferServer", "StartListen", see.Message,
                    121, WriteTo.EventLog, LogType.Error);
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("FileTransferServer", "StartListen", e.Message,
                    121, WriteTo.EventLog, LogType.Error);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();

                listener = null;
            }
        }

        /// <summary>
        /// Stops listening.
        /// </summary>
        public void StopListen()
        {
            try
            {
                if (listener != null)
                    listener.Stop();
            }
            catch (SocketException see)
            {
                base.Write("FileTransferServer", "StopListen", see.Message,
                    158, WriteTo.EventLog, LogType.Error);
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("FileTransferServer", "StopListen", e.Message,
                    158, WriteTo.EventLog, LogType.Error);
            }
            finally
            {
                listener = null;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Get the client idle time out from the configuration
        /// file, if no value is specified then default or
        /// the current value will be used.
        /// </summary>
        private void GetClientTimeOut()
        {
            try
            {
                // If no host name set then
                // use the defualt values.
                if (String.IsNullOrEmpty(hostName))
                {
                    // Create a new default host type
                    // an load the values from the configuration
                    // file into the default host type.
                    FileTransferServerDefaultHost defaultHost =
                        (FileTransferServerDefaultHost)System.Configuration.ConfigurationManager.GetSection(
                            "FileTransferServerGroup/FileTransferServerDefaultHost");

                    // If the value is greater than zero then
                    // assign the value.
                    if (defaultHost.HostSection.ClientTimeOutAttribute > 0)
                        _timeOut = defaultHost.HostSection.ClientTimeOutAttribute;
                }
                else
                {
                    // Create a new host type
                    // an load the values from the configuration
                    // file into the host type.
                    FileTransferServerHosts hosts =
                        (FileTransferServerHosts)System.Configuration.ConfigurationManager.GetSection(
                            "FileTransferServerGroup/FileTransferServerHosts");

                    // If the value is greater than zero then
                    // assign the value.
                    if (hosts.HostSection[hostName].ClientTimeOutAttribute > 0)
                        _timeOut = hosts.HostSection[hostName].ClientTimeOutAttribute;
                }
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("FileTransferServer", "GetClientTimeOut", e.Message,
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
                if (String.IsNullOrEmpty(hostName))
                {
                    // Create a new default host type
                    // an load the values from the configuration
                    // file into the default host type.
                    FileTransferServerDefaultHost defaultHost =
                        (FileTransferServerDefaultHost)System.Configuration.ConfigurationManager.GetSection(
                            "FileTransferServerGroup/FileTransferServerDefaultHost");

                    // If the value is greater than zero then
                    // assign the value.
                    if (defaultHost.HostSection.MaxNumClientsAttribute > 0)
                        _maxNumClients = defaultHost.HostSection.MaxNumClientsAttribute;
                }
                else
                {
                    // Create a new host type
                    // an load the values from the configuration
                    // file into the host type.
                    FileTransferServerHosts hosts =
                        (FileTransferServerHosts)System.Configuration.ConfigurationManager.GetSection(
                            "FileTransferServerGroup/FileTransferServerHosts");

                    // If the value is greater than zero then
                    // assign the value.
                    if (hosts.HostSection[hostName].MaxNumClientsAttribute > 0)
                        _maxNumClients = hosts.HostSection[hostName].MaxNumClientsAttribute;
                }

                // Create a new client array.
                _client = new FileTransferConnection[_maxNumClients];
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("FileTransferServer", "GetMaxNumClients", e.Message,
                    246, WriteTo.EventLog, LogType.Error);
            }
        }

        /// <summary>
        /// Get the current user root path.
        /// </summary>
        /// <param name="username">The user name.</param>
        /// <returns>The root path.</returns>
        private string GetDirectoryPaths(string username)
        {
            try
            {
                // Create a new path type
                // an load the values from the configuration
                // file into the path type.
                FileTransferServerPaths paths =
                    (FileTransferServerPaths)System.Configuration.ConfigurationManager.GetSection(
                        "FileTransferServerDirectoryGroup/FileTransferServerPaths");

                // Get the root directory path.
                return paths.DirectorySection[username].RootDirectoryAttribute;
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("FileTransferServer", "GetDirectoryPaths", e.Message,
                    197, WriteTo.EventLog, LogType.Error);

                return string.Empty;
            }
        }

        /// <summary>
        /// Get the user authentication type.
        /// </summary>
        /// <param name="username">The user name.</param>
        /// <returns>The authentication type.</returns>
        private AuthenticationType GetAuthenticationType(string username)
        {
            try
            {
                AuthenticationType authType = AuthenticationType.None;

                // Create a new path type
                // an load the values from the configuration
                // file into the path type.
                FileTransferServerPaths paths =
                    (FileTransferServerPaths)System.Configuration.ConfigurationManager.GetSection(
                        "FileTransferServerDirectoryGroup/FileTransferServerPaths");

                // Return the authentication type.
                string auth = paths.DirectorySection[username].AuthenticationTypeAttribute;

                switch (auth.ToLower())
                {
                    case "anonymous":
                        authType = AuthenticationType.Anonymous;
                        break;

                    case "integrated":
                        authType = AuthenticationType.Integrated;
                        break;

                    case "sql":
                        authType = AuthenticationType.SQL;
                        break;
                }

                return authType;
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("FileTransferServer", "GetAuthenticationType", e.Message,
                    197, WriteTo.EventLog, LogType.Error);

                // Return no type.
                return AuthenticationType.None;
            }
        }

        /// <summary>
        /// Get the domain of the user.
        /// </summary>
        /// <param name="username">The user name.</param>
        /// <returns>The user domain.</returns>
        private string GetDomain(string username)
        {
            try
            {
                // Create a new path type
                // an load the values from the configuration
                // file into the path type.
                FileTransferServerPaths paths =
                    (FileTransferServerPaths)System.Configuration.ConfigurationManager.GetSection(
                        "FileTransferServerDirectoryGroup/FileTransferServerPaths");

                // Get the domian of the username login.
                return paths.DirectorySection[username].DomianAttribute;
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("FileTransferServer", "GetAuthenticationType", e.Message,
                    197, WriteTo.EventLog, LogType.Error);

                return string.Empty;
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
                if (String.IsNullOrEmpty(hostName))
                {
                    // Create a new default host type
                    // an load the values from the configuration
                    // file into the default host type.
                    FileTransferServerDefaultHost defaultHost =
                        (FileTransferServerDefaultHost)System.Configuration.ConfigurationManager.GetSection(
                            "FileTransferServerGroup/FileTransferServerDefaultHost");

                    // If the port is greater than zero then
                    // assign the port number.
                    if (defaultHost.HostSection.PortAttribute > 0)
                        listenPort = defaultHost.HostSection.PortAttribute;
                }
                else
                {
                    // Create a new host type
                    // an load the values from the configuration
                    // file into the host type.
                    FileTransferServerHosts hosts =
                        (FileTransferServerHosts)System.Configuration.ConfigurationManager.GetSection(
                            "FileTransferServerGroup/FileTransferServerHosts");

                    // If the port is greater than zero then
                    // assign the port number.
                    if (hosts.HostSection[hostName].PortAttribute > 0)
                        listenPort = hosts.HostSection[hostName].PortAttribute;
                }
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("FileTransferServer", "GetListeningPort", e.Message,
                    246, WriteTo.EventLog, LogType.Error);
            }
        }

        /// <summary>
        /// Processes all in-comming client command requests.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="dataReceived">The data received from the client.</param>
        private void client_OnDataReceived(FileTransferConnection sender, string dataReceived)
        {
            try
            {
                // Decrypt the data recived from the client.
                string receivedData = dataReceived;
                string command = string.Empty;
                string data = string.Empty;

                // Get the command from the client and
                // any data the client has sent.
                if (receivedData.Length < 4)
                    command = receivedData.Substring(0, 3).Trim();
                else if (receivedData.Length < 5)
                    command = receivedData.Substring(0, 4).Trim();
                else
                {
                    command = receivedData.Substring(0, 4).Trim();
                    data = receivedData.Substring(4).Trim();
                }

                // Delay.
                System.Threading.Thread.Sleep(10);

                // Process the command.
                switch (command.ToUpper())
                {
                    case "CONN":
                        // Connect the client.
                        Connect(sender, data);
                        break;

                    case "CLOS":
                        // Close the client connection.
                        Disconnect(sender);
                        break;

                    case "UPLO":
                        // Request a file upload.
                        RecieveFile(sender, data);
                        break;

                    case "DOWL":
                        // Request a file download.
                        ConfirmSendFile(sender, data);
                        break;

                    case "GETF":
                        // Upload the file to the client.
                        SendFile(sender);
                        break;

                    case "LIST":
                        // Get the file list in the directory.
                        GetFileList(sender, data);
                        break;

                    case "DLST":
                        // Get the directory list in the directory.
                        GetDirectoryList(sender, data);
                        break;

                    case "STOP":
                        // Stop data interaction.
                        StopData(sender, data);
                        break;

                    case "GCDI":
                        // Get currrent directory.
                        GetDirectory(sender, data);
                        break;

                    case "SCDI":
                        // Set current directory.
                        SetDirectory(sender, data);
                        break;

                    case "DEFL":
                        // Delete current file.
                        DeleteFile(sender, data);
                        break;

                    case "DEDI":
                        // Delete current directory.
                        DeleteDirectory(sender, data);
                        break;

                    case "CRDI":
                        // Create a new directory.
                        CreateDirectory(sender, data);
                        break;

                    case "GFSZ":
                        // Get the size of the file.
                        GetFileSize(sender, data);
                        break;

                    case "ENDC":
                        // End the client connection.
                        EndConnection(sender);
                        break;

                    default:
                        // An unknown command sent.
                        Unknown(sender);
                        break;
                }
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("FileTransferServer", "client_OnDataReceived", e.Message,
                    292, WriteTo.EventLog, LogType.Error);
            }
        }

        /// <summary>
        /// Reply to the current client.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="command">The command to send to the client.</param>
        private void ReplyToSender(FileTransferConnection sender, string command)
        {
            sender.SendCommand(command);
        }

        /// <summary>
        /// Unknown command received.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        private void Unknown(FileTransferConnection sender)
        {
            if (sender.Connected)
                // Reply to client with unknown command.
                ReplyToSender(sender, "UCMD 204");
            else
                // If an unknown command is sent when the channel is opened
                // but the user has not supplied valid credentials then close
                // the socket channel to the client. This is needed when a
                // client opens a socket channel and starts sending a DOS
                // attack.
                Disconnect(sender);
        }

        /// <summary>
        /// Get the file size.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="data">The data from the client.</param>
        private void GetFileSize(FileTransferConnection sender, string data)
        {
            if (sender.Connected)
            {
                try
                {
                    // If the directory exists.
                    if (Directory.Exists(sender.RootDirectory + "\\" + sender.CurrentDirectory))
                    {
                        // Get the file name to delete.
                        string fileName = data.Trim();

                        // Get the current download full path.
                        string filePath = sender.RootDirectory + "\\" +
                            sender.CurrentDirectory + "\\" + fileName.Trim();

                        // If the file exist then get the size.
                        if (File.Exists(filePath))
                        {
                            FileInfo oFileInfo = new FileInfo(filePath);
                            string size = oFileInfo.Length.ToString();

                            // Reply to client ready to receive the file.
                            ReplyToSender(sender, "GFSZ 201;" + size.Length.ToString() + ";" + size);
                        }
                        else
                            // Reply to client file does not exists error command.
                            ReplyToSender(sender, "FDEX 409");
                    }
                    else
                        // Reply to client directory does not exist.
                        ReplyToSender(sender, "DNEX 408");
                }
                catch (Exception e)
                {
                    // Detect a thread abort exception.
                    if (e is ThreadAbortException)
                        Thread.ResetAbort();

                    base.Write("FileTransferServer", "GetFileSize", e.Message,
                        382, WriteTo.EventLog, LogType.Error);

                    // Reply to client internal server error command.
                    ReplyToSender(sender, "ERRO 510");
                }
            }
        }

        /// <summary>
        /// Create a new directory.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="data">The data from the client.</param>
        private void CreateDirectory(FileTransferConnection sender, string data)
        {
            if (sender.Connected)
            {
                try
                {
                    // If the directory exists.
                    if (Directory.Exists(sender.RootDirectory + "\\" + sender.CurrentDirectory))
                    {
                        // Get the file name to delete.
                        string directoryName = data.Trim();

                        // Get the current download full path.
                        string directoryPath = sender.RootDirectory + "\\" +
                            sender.CurrentDirectory + "\\" + directoryName.Trim();

                        // If the directory does not exist then create
                        // the directory from the current directory.
                        if (!Directory.Exists(directoryPath))
                        {
                            // Create the directory.
                            Directory.CreateDirectory(directoryPath);

                            // Reply to client ready to receive the file.
                            ReplyToSender(sender, "CRDI 201");
                        }
                        else
                            // Reply to client directory exists error command.
                            ReplyToSender(sender, "DAEX 413");
                    }
                    else
                        // Reply to client directory does not exist.
                        ReplyToSender(sender, "DNEX 408");
                }
                catch (Exception e)
                {
                    // Detect a thread abort exception.
                    if (e is ThreadAbortException)
                        Thread.ResetAbort();

                    base.Write("FileTransferServer", "CreateDirectory", e.Message,
                        382, WriteTo.EventLog, LogType.Error);

                    // Reply to client internal server error command.
                    ReplyToSender(sender, "ERRO 509");
                }
            }
        }

        /// <summary>
        /// Delete the current directory.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="data">The data from the client.</param>
        private void DeleteDirectory(FileTransferConnection sender, string data)
        {
            if (sender.Connected)
            {
                try
                {
                    // If the directory exists.
                    if (Directory.Exists(sender.RootDirectory + "\\" + sender.CurrentDirectory))
                    {
                        // Get the file name to delete.
                        string directoryName = data.Trim();

                        // Get the current download full path.
                        string directoryPath = sender.RootDirectory + "\\" +
                            sender.CurrentDirectory + "\\" + directoryName.Trim();

                        // If the directory exist then delete
                        // the directory from the current directory.
                        if (Directory.Exists(directoryPath))
                        {
                            // Get all the files and folders
                            // in the current directory to be deleted.
                            string[] files = Directory.GetFiles(directoryPath);
                            string[] directories = Directory.GetDirectories(directoryPath);

                            if (files.Length > 0 || directories.Length > 0)
                                // Reply to client indicating
                                // that the directory is not empty.
                                ReplyToSender(sender, "DNEM 411");
                            else
                            {
                                // Delete the directory.
                                Directory.Delete(directoryPath, false);

                                // Reply to client ready to receive the file.
                                ReplyToSender(sender, "DEDI 201");
                            }
                        }
                        else
                            // Reply to client directory does not exists error command.
                            ReplyToSender(sender, "DDEX 410");
                    }
                    else
                        // Reply to client directory does not exist.
                        ReplyToSender(sender, "DNEX 408");
                }
                catch (Exception e)
                {
                    // Detect a thread abort exception.
                    if (e is ThreadAbortException)
                        Thread.ResetAbort();

                    base.Write("FileTransferServer", "DeleteDirectory", e.Message,
                        382, WriteTo.EventLog, LogType.Error);

                    // Reply to client internal server error command.
                    ReplyToSender(sender, "ERRO 508");
                }
            }
        }

        /// <summary>
        /// Delete the current file.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="data">The data from the client.</param>
        private void DeleteFile(FileTransferConnection sender, string data)
        {
            if (sender.Connected)
            {
                try
                {
                    // If the directory exists.
                    if (Directory.Exists(sender.RootDirectory + "\\" + sender.CurrentDirectory))
                    {
                        // Get the file name to delete.
                        string fileName = data.Trim();

                        // Get the current download full path.
                        string deletePath = sender.RootDirectory + "\\" +
                            sender.CurrentDirectory + "\\" + fileName.Trim();

                        // If the file exist then delete
                        // the file from the current directory.
                        if (File.Exists(deletePath))
                        {
                            // Delete the file.
                            File.Delete(deletePath);

                            // Reply to client ready to receive the file.
                            ReplyToSender(sender, "DEFL 201");
                        }
                        else
                            // Reply to client file does not exists error command.
                            ReplyToSender(sender, "FDEX 409");
                    }
                    else
                        // Reply to client directory does not exist.
                        ReplyToSender(sender, "DNEX 408");
                }
                catch (Exception e)
                {
                    // Detect a thread abort exception.
                    if (e is ThreadAbortException)
                        Thread.ResetAbort();

                    base.Write("FileTransferServer", "DeleteFile", e.Message,
                        382, WriteTo.EventLog, LogType.Error);

                    // Reply to client internal server error command.
                    ReplyToSender(sender, "ERRO 507");
                }
            }
        }

        /// <summary>
        /// Get the current directory path.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="data">The data from the client.</param>
        private void GetDirectory(FileTransferConnection sender, string data)
        {
            if (sender.Connected)
            {
                try
                {
                    // Gets the command which indicates from where
                    // to get the list from.
                    string directory = "GCDI 206;";
                    string path = sender.CurrentDirectory;

                    // If the string id empty
                    // then set as root path.
                    if (path == string.Empty)
                        path = @"\";

                    // Reply to the client with the
                    // list of files with the directory.
                    // Remove the length by two CR+LF.
                    ReplyToSender(sender, directory + (path.Length).ToString() +
                        ";" + path.Replace(@"\", "/"));
                }
                catch (Exception e)
                {
                    // Detect a thread abort exception.
                    if (e is ThreadAbortException)
                        Thread.ResetAbort();

                    base.Write("FileTransferServer", "GetDirectory", e.Message,
                        382, WriteTo.EventLog, LogType.Error);

                    // Reply to client internal server error command.
                    ReplyToSender(sender, "ERRO 506");
                }
            }
        }

        /// <summary>
        /// Set the current directory path.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="data">The data from the client.</param>
        private void SetDirectory(FileTransferConnection sender, string data)
        {
            if (sender.Connected)
            {
                try
                {
                    if (Directory.Exists(sender.RootDirectory + "\\" + data.Trim().Replace("/", @"\")))
                    {
                        if ((data.Trim() == "/") || (data.Trim() == @"\"))
                            sender.CurrentDirectory = string.Empty;
                        else
                            sender.CurrentDirectory = data.Trim().Replace("/", @"\");

                        // Reply to the client that
                        // the directory was set.
                        ReplyToSender(sender, "SCDI 205");
                    }
                    else
                        // Reply to client directory does not exist.
                        ReplyToSender(sender, "DNEX 408");
                }
                catch (Exception e)
                {
                    // Detect a thread abort exception.
                    if (e is ThreadAbortException)
                        Thread.ResetAbort();

                    base.Write("FileTransferServer", "SetDirectory", e.Message,
                        382, WriteTo.EventLog, LogType.Error);

                    // Reply to client internal server error command.
                    ReplyToSender(sender, "ERRO 505");
                }
            }
        }

        /// <summary>
        /// Connect the client to the server.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="data">The data from the client.</param>
        private void Connect(FileTransferConnection sender, string data)
        {
            try
            {
                bool authResult = false;

                // Message parts are divided by ";"  
                // Break the string into an array accordingly.
                string[] dataArray = data.Split((char)59);

                // Get the user name and password
                // to valid the client.
                string username = dataArray[0].Trim().ToLower();
                string password = dataArray[1].Trim();
                string domain = dataArray[2].Trim();

                // If the client is already connected.
                if (!String.IsNullOrEmpty(sender.ConnectionName))
                    // Reply to client user already connected command.
                    ReplyToSender(sender, "ARCO 203");
                else
                {
                    // Get the authentication type.
                    AuthenticationType authType = GetAuthenticationType(username);

                    switch (authType)
                    {
                        case AuthenticationType.None:
                            // Reply to client user suspended error command.
                            ReplyToSender(sender, "REJE 401");
                            break;

                        case AuthenticationType.Anonymous:
                            // Anonymous access.
                            authResult = true;
                            break;

                        case AuthenticationType.Integrated:
                            // Integrated authentication.
                            authResult = IntegratedAuthentication(sender, username, password);
                            break;

                        case AuthenticationType.SQL:
                            // Look in the database of authentication.
                            authResult = SQLAuthentication(sender, username, password);
                            break;
                    }

                    // If the user has been validated
                    // and autharised then allow connection.
                    if (authResult)
                    {
                        // Get the root path for the
                        // current user.
                        string rootPath = GetDirectoryPaths(username);

                        // Assign the user name
                        // and connected to true.
                        // Reply to client a valid connection.
                        sender.ConnectionName = username;
                        sender.RootDirectory = rootPath;
                        sender.Connected = true;
                        ReplyToSender(sender, "JOIN 200");
                    }
                }
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("FileTransferServer", "Connect", e.Message,
                    382, WriteTo.EventLog, LogType.Error);

                // Reply to client internal server error command.
                ReplyToSender(sender, "ERRO 500");
            }
        }

        /// <summary>
        /// Validate the current user through an SQL server database.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="username">The user name.</param>
        /// <param name="password">The user password.</param>
        /// <returns>True if the user has been validated.</returns>
        private bool SQLAuthentication(FileTransferConnection sender,
            string username, string password)
        {
            bool ret = false;
            bool retInt = false;
            Nequeo.Security.IAuthorisationProvider authenticate = Common.Helper.Authenticate();

            // Validate the credentials of the
            // client with the data data.
            Nequeo.Security.UserCredentials user = new UserCredentials();
            user.Username = username;
            user.Password = password;

            if (user != null)
                retInt = true;

            try
            {
                // If no internal database error
                // then proceed.
                if (retInt)
                {
                    // Make sure the authenticator has been created.
                    if (authenticate != null)
                    {
                        // If the user has been authenticated.
                        if (authenticate.AuthenticateUser(user))
                            ret = true;
                        else
                            // Reply to client user suspended error command.
                            ReplyToSender(sender, "REJE 401");
                    }
                    else
                        // Reply to client user not valid error command.
                        ReplyToSender(sender, "REJE 402");
                }
                else
                    // Reply to client user not valid error command.
                    ReplyToSender(sender, "REJE 402");
            }
            finally { }

            // Return the validation result.
            return ret;
        }

        /// <summary>
        /// Validate the current user through windows integration.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="username">The user name.</param>
        /// <param name="password">The user password.</param>
        /// <returns>True if the user has been validated.</returns>
        private bool IntegratedAuthentication(FileTransferConnection sender,
            string username, string password)
        {
            bool ret = false;

            WindowsAuthentication windowsAuth = new WindowsAuthentication();
            windowsAuth.OnError += new AuthenticateErrorHandler(windowsAuth_OnError);
            windowsAuth.OnAuthenticate +=
                delegate(Object s, AuthenticateArgs e)
                {
                    // Has the user been authenticated
                    ret = e.IsAuthenticated;
                };

            // Get the current domian for the user.
            string domain = GetDomain(username);

            // Authenticate the user.
            bool result = windowsAuth.AuthenticateUser(username, domain, password);

            // User not authenticated.
            if (!ret)
                // Reply to client user not valid error command.
                ReplyToSender(sender, "REJE 402");

            // Return the validation result.
            return ret;
        }

        /// <summary>
        /// Windows authenticate error.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="e">The event argument.</param>
        private void windowsAuth_OnError(object sender, AuthenticateErrorArgs e)
        {
            base.Write("FileTransferServer", "OnError",
                "Integrated authentication error." + " " +
                "Attempt Username : " + e.UserName + " Password : " +
                e.Password + " Domain : " + e.Domain + ". Message : " + e.ErrorMessage,
                472, WriteTo.EventLog, LogType.Error);
        }

        /// <summary>
        /// Disconnect the client from the server, close the channel.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        private void Disconnect(FileTransferConnection sender)
        {
            sender.Connected = false;
            sender.Disconnect(null);
            sender = null;
        }

        /// <summary>
        /// Ends the current client connection.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        private void EndConnection(FileTransferConnection sender)
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
        /// A request from the client to upload a file, server receives the file.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="data">The data sent by the client.</param>
        private void RecieveFile(FileTransferConnection sender, string data)
        {
            if (sender.Connected)
            {
                try
                {
                    // Message parts are divided by ";"  
                    // Break the string into an array accordingly.
                    string[] dataArray = data.Split((char)59);

                    // Get the file to download.
                    // and the size of the file.
                    string fileName = dataArray[0].Trim();
                    long fileSize = (long)Convert.ToInt64(dataArray[1].Trim());

                    // If the directory exists.
                    if (Directory.Exists(sender.RootDirectory + "\\" + sender.CurrentDirectory))
                    {
                        // Get the current download full path.
                        string downloadPath = sender.RootDirectory + "\\" +
                            sender.CurrentDirectory + "\\" + fileName.Trim();

                        // If the file does not exist then add
                        // the file to the current directory.
                        if (!File.Exists(downloadPath))
                        {
                            // Assign the downlaod path
                            // the file size and the
                            // downloading indicator
                            // to true.
                            sender.DestinationFile = downloadPath;
                            sender.DestinationFileSize = fileSize;
                            sender.DownloadingFile = true;

                            // Reply to client ready to receive the file.
                            ReplyToSender(sender, "UPOK 201");
                        }
                        else
                            // Reply to client file already exists error command.
                            ReplyToSender(sender, "FAEX 404");
                    }
                    else
                        // Reply to client directory does not exist.
                        ReplyToSender(sender, "DNEX 408");
                }
                catch (Exception e)
                {
                    // Detect a thread abort exception.
                    if (e is ThreadAbortException)
                        Thread.ResetAbort();

                    base.Write("FileTransferServer", "RecieveFile", e.Message,
                        472, WriteTo.EventLog, LogType.Error);

                    // Reply to client internal server error command.
                    ReplyToSender(sender, "ERRO 501");
                }
            }
        }

        /// <summary>
        /// A request from the client to download a file, server sends the file.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="data">The data sent by the client.</param>
        private void ConfirmSendFile(FileTransferConnection sender, string data)
        {
            if (sender.Connected)
            {
                try
                {
                    // If the directory exists.
                    if (Directory.Exists(sender.RootDirectory + "\\" + sender.CurrentDirectory))
                    {
                        // Get the file name to upload and
                        // the full path of the upload file.
                        string fileName = data.Trim();
                        string uploadPath = sender.RootDirectory + "\\" +
                            sender.CurrentDirectory + "\\" + fileName.Trim();

                        // If the file exists then confirm upload.
                        if (File.Exists(uploadPath))
                        {
                            // Get the upload file size
                            // and assign the client channel
                            // with the file to upload
                            // and the size.
                            FileInfo fileInfo = new FileInfo(uploadPath);
                            long fileSize = fileInfo.Length;
                            sender.TargetFile = uploadPath;
                            sender.TargetFileSize = fileSize;
                            sender.StopSending = false;

                            // Reply to the client ready to send the
                            // file and also send the size of the file.
                            ReplyToSender(sender, "DMOK 202;" + fileSize.ToString());
                        }
                        else
                            // Reply to client file does not exist error command.
                            ReplyToSender(sender, "FNEX 405");
                    }
                    else
                        // Reply to client directory does not exist.
                        ReplyToSender(sender, "DNEX 408");
                }
                catch (Exception e)
                {
                    // Detect a thread abort exception.
                    if (e is ThreadAbortException)
                        Thread.ResetAbort();

                    base.Write("FileTransferServer", "ConfirmSendFile", e.Message,
                        539, WriteTo.EventLog, LogType.Error);

                    // Reply to client internal server error command.
                    ReplyToSender(sender, "ERRO 502");
                }
            }
        }

        /// <summary>
        /// Begin sending the file to the client.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        private void SendFile(FileTransferConnection sender)
        {
            sender.Upload();
        }

        /// <summary>
        /// Get the current file list within the specified directory.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="data">The data sent by the client.</param>
        private void GetFileList(FileTransferConnection sender, string data)
        {
            if (sender.Connected)
            {
                try
                {
                    // Gets the command which indicates from where
                    // to get the list from.
                    string list = "LIST 207;";
                    string fileList = string.Empty;
                    string path = string.Empty;

                    // Get the collection of files within the folder.
                    string[] files = null;

                    // If the directory exists.
                    if (Directory.Exists(sender.RootDirectory + "\\" + sender.CurrentDirectory))
                    {
                        // Get all the files in the current
                        // directory.
                        files = System.IO.Directory.GetFiles(sender.RootDirectory + "\\" + sender.CurrentDirectory);
                        path = sender.RootDirectory + "\\" + sender.CurrentDirectory;

                        // Does the folder contain files.
                        if ((files != null) && (files.Length > 0))
                        {
                            // For each file within the folder
                            // concatenate the text.
                            for (int j = 0; j < files.Length; j++)
                            {
                                FileInfo oFileInfo = new FileInfo(files[j].Trim());
                                fileList = fileList + files[j].Trim().Replace(path, "").Replace(";", "c").Replace(@"\", "") +
                                " Size: " + oFileInfo.Length.ToString("#,#", CultureInfo.InvariantCulture) + " bytes Updated: " +
                                oFileInfo.LastWriteTime.ToShortDateString() + " " +
                                oFileInfo.LastWriteTime.ToShortTimeString() + (char)13 + (char)10;
                            }

                            // Reply to the client with the
                            // list of files with the directory.
                            // Remove the length by two CR+LF.
                            ReplyToSender(sender, list + (fileList.Length - 2).ToString() + ";" + fileList);
                        }
                        else
                            // Reply to client no files found error command.
                            ReplyToSender(sender, "FNFD 406");
                    }
                    else
                        // Reply to client directory does not exist.
                        ReplyToSender(sender, "DNEX 408");
                }
                catch (Exception e)
                {
                    // Detect a thread abort exception.
                    if (e is ThreadAbortException)
                        Thread.ResetAbort();

                    base.Write("FileTransferServer", "GetFileList", e.Message,
                        599, WriteTo.EventLog, LogType.Error);

                    // Reply to client internal server error command.
                    ReplyToSender(sender, "ERRO 503");
                }
            }
        }

        /// <summary>
        /// Get the current directory list within the specified directory.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="data">The data sent by the client.</param>
        private void GetDirectoryList(FileTransferConnection sender, string data)
        {
            if (sender.Connected)
            {
                try
                {
                    // Gets the command which indicates from where
                    // to get the list from.
                    string directory = "DLST 208;";
                    string directoryList = string.Empty;
                    string path = string.Empty;

                    // Get the collection of files within the folder.
                    string[] directories = null;

                    // If the directory exists.
                    if (Directory.Exists(sender.RootDirectory + "\\" + sender.CurrentDirectory))
                    {
                        // Get all upload directories.
                        directories = System.IO.Directory.GetDirectories(sender.RootDirectory + "\\" + sender.CurrentDirectory);
                        path = sender.RootDirectory + "\\" + sender.CurrentDirectory;

                        // Does the folder contain files.
                        if ((directories != null) && (directories.Length > 0))
                        {
                            // For each directory within the folder
                            // concatenate the text.
                            for (int j = 0; j < directories.Length; j++)
                            {
                                // Get all the files and folders
                                // in the current directory to be deleted.
                                string[] files = Directory.GetFiles(directories[j]);
                                string[] dir = Directory.GetDirectories(directories[j]);

                                DirectoryInfo oDirectoryInfo = new DirectoryInfo(directories[j].Trim());
                                directoryList = directoryList +
                                directories[j].Trim().Replace(path, "").Replace(";", "c").Replace(@"\", "") +
                                " D: " + dir.Length.ToString() + " directories F: " + files.Length.ToString() +
                                " files Updated: " + oDirectoryInfo.LastWriteTime.ToShortDateString() + " " +
                                oDirectoryInfo.LastWriteTime.ToShortTimeString() + (char)13 + (char)10;
                            }

                            // Reply to the client with the
                            // list of directories with the directory.
                            // Remove the length by two CR+LF.
                            ReplyToSender(sender, directory + (directoryList.Length - 2).ToString() + ";" + directoryList);
                        }
                        else
                            // Reply to client no directories found error command.
                            ReplyToSender(sender, "DNFD 407");
                    }
                    else
                        // Reply to client directory does not exist.
                        ReplyToSender(sender, "DNEX 408");
                }
                catch (Exception e)
                {
                    // Detect a thread abort exception.
                    if (e is ThreadAbortException)
                        Thread.ResetAbort();

                    base.Write("FileTransferServer", "GetDirectoryList", e.Message,
                        599, WriteTo.EventLog, LogType.Error);

                    // Reply to client internal server error command.
                    ReplyToSender(sender, "ERRO 504");
                }
            }
        }

        /// <summary>
        /// Stop data transfer.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="data">The data sent by the client.</param>
        private void StopData(FileTransferConnection sender, string data)
        {
            if (sender.Connected)
            {
                // Gets the command which indicates from where
                // to get the list from.
                string listCommand = data.Trim();

                // Get the list command.
                switch (listCommand.ToUpper())
                {
                    case "UP":
                    case "DN":
                        // Stop uploading data.
                        sender.StopSending = true;

                        // Stop waiting for data.
                        sender.DownloadingFile = false;
                        break;
                }
            }
        }
        #endregion
    }
}
