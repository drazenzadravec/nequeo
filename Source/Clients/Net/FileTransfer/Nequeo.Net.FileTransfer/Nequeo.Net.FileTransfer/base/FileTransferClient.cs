/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 *                  
 *                  
 *                  
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Net.Security;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Net.FileTransfer.Channel;

namespace Nequeo.Net.FileTransfer
{
    /// <summary>
    /// Welcome call back delegate.
    /// </summary>
    /// <param name="welcomeCommand">The welcome command.</param>
    internal delegate void WelcomeCallback(string welcomeCommand);

    /// <summary>
    /// Operation complete call back delegate.
    /// </summary>
    /// <param name="sendQuitCommand">Send the quit command.</param>
    internal delegate void OperationCompleteCallback(bool sendQuitCommand);

    /// <summary>
    /// The file transfer client socket.
    /// </summary>
    public class Client
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Client()
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="credentials">Current credentials.</param>
        /// <param name="connection">Current connection information.</param>
        public Client(NetworkCredential credentials,
            FileTransferConnection connection)
        {
            this.credentials = credentials;
            this.connection = connection;
        }
        #endregion

        #region Constants
        private const int READ_BUFFER_SIZE = 8192;
        private const int WRITE_BUFFER_SIZE = 8192;
        #endregion

        #region Private Fields
        private TcpClient client = null;
        private NetworkStream networkStream = null;
        private NetworkCredential credentials = null;
        private FileTransferConnection connection = null;
        private FileTransferPath filePath = null;
        private FileTransferDataChannel _dataChannel = null;
        private int timeout = -1;

        private UploadProgressCallback _uploadProgressCallback = null;
        private DownloadProgressCallback _downloadProgressCallback = null;
        private UploadCompleteCallback _uploadCompleteCallback = null;
        private DownloadCompleteCallback _downloadCompleteCallback = null;

        private byte[] readBuffer = new byte[READ_BUFFER_SIZE];
        private byte[] writeBuffer = new byte[WRITE_BUFFER_SIZE];

        private string readCommand = string.Empty;
        private long readCode = 0;

        private string directDataReceived = string.Empty;
        private long directDataLength = 0;
        private bool directDataRead = false;
        private bool downloadingFile = false;
        private bool clientConnected = false;
        private bool userConnected = false;
        private bool stopSending = false;

        private long readLoopCount = 0;
        private long readDownLoadLoopCount = 0;

        // Upload and download event signaler.
        private WaitHandle[] _waitOnEvents = new WaitHandle[]
        {
            // Download signal.
            new AutoResetEvent(false),
            // Upload signal.
            new AutoResetEvent(false)
        };

        private Channel.Operation operation = Channel.Operation.None;

        private WelcomeCallback _welcomeCallback = null;
        private OperationCompleteCallback _operationCompleteCallback = null;
        private FileTransferPath _internalFilePath = new FileTransferPath();
        #endregion

        #region Public Properties
        /// <summary>
        /// Get set, internal opertaion file paths.
        /// </summary>
        internal FileTransferPath InternalFilePath
        {
            get { return _internalFilePath; }
            set { _internalFilePath = value; }
        }

        /// <summary>
        /// Get set, operation complete call back delegate.
        /// </summary>
        internal OperationCompleteCallback OperationCompleteCallback
        {
            set { _operationCompleteCallback = value; }
            get { return _operationCompleteCallback; }
        }

        /// <summary>
        /// Get set, upload progress call back delegate.
        /// </summary>
        public UploadProgressCallback UploadProgressCallback
        {
            set { _uploadProgressCallback = value; }
            get { return _uploadProgressCallback; }
        }

        /// <summary>
        /// Get set, download progress call back delegate.
        /// </summary>
        public DownloadProgressCallback DownloadProgressCallback
        {
            set { _downloadProgressCallback = value; }
            get { return _downloadProgressCallback; }
        }

        /// <summary>
        /// Get set, upload complete call back delegate.
        /// </summary>
        public UploadCompleteCallback UploadCompleteCallback
        {
            set { _uploadCompleteCallback = value; }
            get { return _uploadCompleteCallback; }
        }

        /// <summary>
        /// Get set, download complete call back delegate.
        /// </summary>
        public DownloadCompleteCallback DownloadCompleteCallback
        {
            set { _downloadCompleteCallback = value; }
            get { return _downloadCompleteCallback; }
        }

        /// <summary>
        /// Get, the server response command.
        /// </summary>
        public string ResponseCommand
        {
            get { return readCommand; }
        }

        /// <summary>
        /// Get, the server response code.
        /// </summary>
        public long ResponseCode
        {
            get { return readCode; }
        }

        /// <summary>
        /// Get set, the client credentials.
        /// </summary>
        public NetworkCredential Credentials
        {
            get { return credentials; }
            set { credentials = value; }
        }

        /// <summary>
        /// Get set, the operation time out.
        /// </summary>
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        /// <summary>
        /// Gets sets, stop sending the file.
        /// </summary>
        public bool StopSending
        {
            get { return stopSending; }
            set { stopSending = value; }
        }

        /// <summary>
        /// Get set, the client connection.
        /// </summary>
        public FileTransferConnection Connection
        {
            get { return connection; }
            set { connection = value; }
        }

        /// <summary>
        /// Get set, the opertaion file paths.
        /// </summary>
        public FileTransferPath FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        /// <summary>
        /// Get set, the operation direction.
        /// </summary>
        public Channel.Operation Operation
        {
            get { return operation; }
            set { operation = value; }
        }

        /// <summary>
        /// Get, the current connection state.
        /// </summary>
        public bool Connected
        {
            get { return userConnected; }
        }
        #endregion

        #region Public Events
        /// <summary>
        /// On process complete event.
        /// </summary>
        public event ClientCommandHandler OnProcessComplete;

        /// <summary>
        /// On command return error event.
        /// </summary>
        public event ClientCommandHandler OnCommandError;

        /// <summary>
        /// On command return error event.
        /// </summary>
        public event ClientCommandHandler OnCommandConnected;

        /// <summary>
        /// On direct read data complete event.
        /// </summary>
        public event ClientCommandHandler OnReadDataComplete;

        /// <summary>
        /// When an error occures on an asynchronous thread.
        /// </summary>
        public event ClientThreadErrorHandler OnAsyncThreadError;
        #endregion

        #region Public Methods
        /// <summary>
        /// Abort the current asynchronous upload/download operation.
        /// This method also closes the connection.
        /// </summary>
        public void Abort()
        {
            if (_dataChannel != null)
                _dataChannel.Abort();

            // Get each auto reset event
            // from the wait handle collection.
            AutoResetEvent stopDownload = (AutoResetEvent)_waitOnEvents[0];
            AutoResetEvent stopUpload = (AutoResetEvent)_waitOnEvents[1];

            // Stop any download or upload
            // from executing.
            stopDownload.Set();
            stopUpload.Set();
        }

        /// <summary>
        /// Starts the client operation.
        /// </summary>
        public void Open()
        {
            Connect();
        }

        /// <summary>
        /// Closes the client connection.
        /// </summary>
        /// <param name="sendCloseCommand">Send a close command to the
        /// server, only use when not stopping an operation.</param>
        public void Close(bool sendCloseCommand)
        {
            if (client != null)
            {
                if (clientConnected)
                {
                    // Send a close command to the server.
                    if (sendCloseCommand)
                        SendCommand("CLOS");

                    // Set the client to no connection.
                    clientConnected = false;
                }

                // Close the tcp channel.
                client.Client.Shutdown(SocketShutdown.Both);
                client.Client.Disconnect(false);
                client.Close();
                networkStream.Close();

                // Clear from memory.
                client = null;
                networkStream = null;
            }
        }

        /// <summary>
        /// Execute the client operation.
        /// </summary>
        public void Execute()
        {
            // Validate all the data.
            DataValidation();

            // Start the specified operation.
            if (userConnected)
                StartOperation();
            else
                throw new Exception("No connection has been opened.");
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Make a connection to the server.
        /// </summary>
        private void Connect()
        {
            // Validate all the data.
            ClientValidation();

            // Create anew instance of the
            // TCP client class.
            client = null;
            networkStream = null;
            client = new TcpClient();

            // Connect to the server with the
            // specified connection data.
            // Get the network channel.
            client.Connect(connection.Host, connection.Port);
            networkStream = client.GetStream();

            // Create a new file transfer state
            // object and assign the network stream.
            FileTransferState state = new FileTransferState();
            state.NetworkStream = networkStream;

            // Start a new read thread and block
            // until data is received from the server.
            networkStream.BeginRead(readBuffer, 0, READ_BUFFER_SIZE,
                new AsyncCallback(DataReceiver), state);
        }

        /// <summary>
        /// Get the welcome command from the server.
        /// </summary>
        /// <param name="welcomeCommand">The welcome command.</param>
        private void ServerWelcome(string welcomeCommand)
        {
            // Indicate that the client is now connected.
            clientConnected = true;

            // Attempt to login the client.
            Login();
        }

        /// <summary>
        /// Login the current client with
        /// the specified credentials.
        /// </summary>
        private void Login()
        {
            if (clientConnected)
                // Send a connect command to the server
                // along with the client credentials.
                SendCommand("CONN " + credentials.UserName + ";" +
                    credentials.Password + ";" + credentials.Domain);
        }

        /// <summary>
        /// Start the specified operation.
        /// </summary>
        private void StartOperation()
        {
            switch (operation)
            {
                case Channel.Operation.Upload:
                    // Upload the file.
                    Upload();
                    break;

                case Channel.Operation.Download:
                    // Download the file.
                    Download();
                    break;

                case Channel.Operation.GetFileList:
                    // File list.
                    FileList();
                    break;

                case Channel.Operation.GetDirectoryList:
                    // Directory list.
                    DirectoryList();
                    break;

                case Channel.Operation.GetPath:
                    // Get directory path.
                    DirectoryPath(0);
                    break;

                case Channel.Operation.SetPath:
                    // Set directory path.
                    DirectoryPath(1);
                    break;

                case Channel.Operation.GetFileSize:
                    // Get file size.
                    FileSize();
                    break;

                case Channel.Operation.DeleteFile:
                    // Delete the file.
                    DeleteFile();
                    break;

                case Channel.Operation.DeleteDirectory:
                    // Delete the directory.
                    DeleteDirectory();
                    break;

                case Channel.Operation.CreateDirectory:
                    // Create a new directory.
                    CreateDirectory();
                    break;
            }
        }

        /// <summary>
        /// Start the download operation.
        /// </summary>
        private void Download()
        {
            if (userConnected)
                // Send a download command to the server
                // along with the remote file to get.
                if (!connection.UseDataChannel)
                    SendCommand("DOWL " + filePath.RemoteFile.Replace(';', 'c'));
                else
                {
                    // Create a new connection channel
                    // to download the file.
                    _dataChannel = new FileTransferDataChannel(_uploadProgressCallback,
                        _downloadProgressCallback, _uploadCompleteCallback,
                        _downloadCompleteCallback, OnCommandError, OnAsyncThreadError,
                        credentials, connection, filePath, _internalFilePath,
                        operation, false, timeout);

                    // Start the operation.
                    _dataChannel.StartOperation();
                }
        }

        /// <summary>
        /// Start the upload operation.
        /// </summary>
        private void Upload()
        {
            if (userConnected)
                // Send an upload command to the server
                // along with the remote file to set
                // and the file size.
                if (!connection.UseDataChannel)
                    SendCommand("UPLO " + filePath.RemoteFile.Replace(';', 'c') + ";" +
                        filePath.TargetFileSize.ToString());
                else
                {
                    // Create a new connection channel
                    // to upload the file.
                    _dataChannel = new FileTransferDataChannel(_uploadProgressCallback,
                        _downloadProgressCallback, _uploadCompleteCallback,
                        _downloadCompleteCallback, OnCommandError, OnAsyncThreadError,
                        credentials, connection, filePath, _internalFilePath, 
                        operation, false, timeout);

                    // Start the operation.
                    _dataChannel.StartOperation();
                }
        }

        /// <summary>
        /// Start the list operation.
        /// </summary>
        private void FileList()
        {
            if (userConnected)
                // Send an list command to the
                // server, returns the list of files.
                SendCommand("LIST");
        }

        /// <summary>
        /// Start the list operation.
        /// </summary>
        private void DirectoryList()
        {
            if (userConnected)
                // Send an directory list command to the
                // server, returns the list of files.
                SendCommand("DLST");
        }

        /// <summary>
        /// Start the directory path operation.
        /// </summary>
        private void DirectoryPath(int operation)
        {
            if (userConnected)
                // Send an directory path command to the
                // server, returns the list of files.
                if (operation == 0)
                    SendCommand("GCDI");
                else
                {
                    if (filePath.RemoteDirectoryPath == string.Empty)
                        SendCommand("SCDI /");
                    else
                        SendCommand("SCDI " +
                            filePath.RemoteDirectoryPath.Trim().Replace(';', 'c'));
                }
        }

        /// <summary>
        /// Start the get file size operation.
        /// </summary>
        private void FileSize()
        {
            if (userConnected)
                // Send a get file command
                // along with the remote file to get.
                SendCommand("GFSZ " + filePath.RemoteFile.Replace(';', 'c'));
        }

        /// <summary>
        /// Delete a file.
        /// </summary>
        private void DeleteFile()
        {
            if (userConnected)
                // Send a delete file command
                // along with the remote file to get.
                SendCommand("DEFL " + filePath.RemoteFile.Replace(';', 'c'));
        }

        /// <summary>
        /// Delete a directory.
        /// </summary>
        private void DeleteDirectory()
        {
            if (userConnected)
                // Send a delete directory command
                // along with the remote file to get.
                SendCommand("DEDI " +
                    filePath.RemoteDirectoryPath.Trim().Replace(';', 'c'));
        }

        /// <summary>
        /// Create a new directory.
        /// </summary>
        private void CreateDirectory()
        {
            if (userConnected)
                // Send a delete directory command
                // along with the remote file to get.
                SendCommand("CRDI " +
                    filePath.RemoteDirectoryPath.Trim().Replace(';', 'c'));
        }

        /// <summary>
        /// Processes all in-comming client command requests.
        /// </summary>
        /// <param name="dataReceived">The data receved from the server.</param>
        private void CommandReceived(string dataReceived)
        {
            try
            {
                // Decrypt the data recived from the client.
                string receivedData = dataReceived;

                // If diectly reading data then
                // get all data.
                if (directDataRead)
                {
                    GetData(receivedData);
                    return;
                }

                // Get the current command and
                // the data send by the server.
                string command = receivedData.Substring(0, 4).Trim();
                string data = receivedData.Substring(4).Trim();

                // Message parts are divided by ";"  
                // Break the string into an array accordingly.
                string[] dataArray = data.Split(new char[] { ';' },
                    StringSplitOptions.RemoveEmptyEntries);

                // Get the current command code.
                long code = (long)Convert.ToInt64(dataArray[0]);

                // Assign the command and code
                // for a initial read process.
                readCommand = command;
                readCode = code;

                // Delay.
                System.Threading.Thread.Sleep(200);

                // Process the command.
                switch (command.ToUpper())
                {
                    case "WELC":
                        // Get the welcome data from the server,
                        // Create a new welcome delegate and
                        // invoke the call back.
                        _welcomeCallback = new WelcomeCallback(ServerWelcome);
                        _welcomeCallback.Invoke(command);
                        break;

                    case "JOIN":
                        // The login was successful
                        userConnected = true;

                        // When a connection has been made
                        // and the client is validated.
                        if (OnCommandConnected != null)
                            OnCommandConnected(this, new ClientCommandArgs(command,
                                Channel.FileTransferProtocol.GetFileTransferServerResponseDescription(command, code), code));
                        break;

                    case "UPOK":
                        // Upload has been validated
                        // start the upload.
                        stopSending = false;
                        UploadEx();
                        break;

                    case "CDDN":
                        // Uplaod has completed
                        // send a message the client
                        // indicating that the upload
                        // is finished.
                        if (_uploadCompleteCallback != null)
                            _uploadCompleteCallback(filePath.RemoteFile);

                        //Send upload complete to the data channel connection.
                        if(_operationCompleteCallback != null)
                            _operationCompleteCallback.Invoke(true);
                        break;

                    case "FAEX":
                        // Server sent a command indicating that
                        // the file being uploaded
                        // already exits. Send to the client
                        // the command error.
                        if (OnCommandError != null)
                            OnCommandError(this, new ClientCommandArgs(command,
                                Channel.FileTransferProtocol.GetFileTransferServerResponseDescription(command, code) +
                                " " + filePath.RemoteFile, code));
                        break;

                    case "REJE":
                        // Send to the client the current
                        // command error.
                        if (OnCommandError != null)
                            OnCommandError(this, new ClientCommandArgs(command,
                                Channel.FileTransferProtocol.GetFileTransferServerResponseDescription(command, code), code));
                        break;

                    case "FNEX":
                        // Server sent a command indicating that
                        // the file being downloaded does not exist.
                        // Send to the client the command error.
                        if (OnCommandError != null)
                            OnCommandError(this, new ClientCommandArgs(command,
                                Channel.FileTransferProtocol.GetFileTransferServerResponseDescription(command, code) +
                                " " + filePath.RemoteFile, code));
                        break;

                    case "UCMD":
                        // Server sent an unknown command error,
                        // the server did not understand the command.
                        // Send to the client the command error.
                        if (OnCommandError != null)
                            OnCommandError(this, new ClientCommandArgs(command,
                                Channel.FileTransferProtocol.GetFileTransferServerResponseDescription(command, code), code));
                        break;

                    case "ARCO":
                        // Server sent a command indicating that
                        // the login account is already in use.
                        // Send to the client the command error.
                        if (OnCommandError != null)
                            OnCommandError(this, new ClientCommandArgs(command,
                                Channel.FileTransferProtocol.GetFileTransferServerResponseDescription(command, code), code));
                        break;

                    case "DMOK":
                        // Server sent a command that upload is valid
                        // and the server is ready to sent the file,
                        // get the file size being sent.
                        downloadingFile = true;
                        filePath.DestinationFileSize = (long)Convert.ToInt64(dataArray[1]);

                        // Send a command to the server to sent
                        // the file.
                        SendCommand("GETF");
                        break;

                    case "ERRO":
                        // Send to the client the current
                        // command error.
                        if (OnCommandError != null)
                            OnCommandError(this, new ClientCommandArgs(command,
                                Channel.FileTransferProtocol.GetFileTransferServerResponseDescription(command, code), code));
                        break;

                    case "LIST":
                        // Gets the list of files from the specified directory.
                        directDataRead = true;
                        directDataLength = (long)Convert.ToInt64(dataArray[1]);
                        GetData(dataArray[2]);
                        break;

                    case "DLST":
                        // Gets the list of directories from the specified directory.
                        directDataRead = true;
                        directDataLength = (long)Convert.ToInt64(dataArray[1]);
                        GetData(dataArray[2]);
                        break;

                    case "GCDI":
                        // Gets the current directory.
                        directDataRead = true;
                        directDataLength = (long)Convert.ToInt64(dataArray[1]);
                        GetData(dataArray[2]);
                        break;

                    case "SCDI":
                        // Server sent a command indicating that
                        // the directory path was set.
                        _internalFilePath.RemoteDirectoryPath = filePath.RemoteDirectoryPath;
                        if (OnProcessComplete != null)
                            OnProcessComplete(this, new ClientCommandArgs(command,
                                Channel.FileTransferProtocol.GetFileTransferServerResponseDescription(command, code) +
                                " " + filePath.RemoteDirectoryPath, code));
                        break;

                    case "FNFD":
                        // Server sent a command indicating that
                        // the file list found zero files.
                        // Send to the client the command error.
                        if (OnCommandError != null)
                            OnCommandError(this, new ClientCommandArgs(command,
                                Channel.FileTransferProtocol.GetFileTransferServerResponseDescription(command, code), code));
                        break;

                    case "DNFD":
                        // Server sent a command indicating that
                        // the directory list found zero directories.
                        // Send to the client the command error.
                        if (OnCommandError != null)
                            OnCommandError(this, new ClientCommandArgs(command,
                                Channel.FileTransferProtocol.GetFileTransferServerResponseDescription(command, code), code));
                        break;

                    case "DNEX":
                        // Server sent a command indicating that
                        // the directory dose not exits for the path.
                        if (OnCommandError != null)
                            OnCommandError(this, new ClientCommandArgs(command,
                                Channel.FileTransferProtocol.GetFileTransferServerResponseDescription(command, code), code));
                        break;

                    case "FDEX":
                        // File does not exist.
                        if (OnCommandError != null)
                            OnCommandError(this, new ClientCommandArgs(command,
                                Channel.FileTransferProtocol.GetFileTransferServerResponseDescription(command, code), code));
                        break;

                    case "GFSZ":
                        // Gets file size.
                        directDataRead = true;
                        directDataLength = (long)Convert.ToInt64(dataArray[1]);
                        GetData(dataArray[2]);
                        break;

                    case "DAEX":
                        // Directory already exists.
                        if (OnCommandError != null)
                            OnCommandError(this, new ClientCommandArgs(command,
                                Channel.FileTransferProtocol.GetFileTransferServerResponseDescription(command, code), code));
                        break;

                    case "CRDI":
                        // Directory was created.
                        if (OnProcessComplete != null)
                            OnProcessComplete(this, new ClientCommandArgs(command,
                                Channel.FileTransferProtocol.GetFileTransferServerResponseDescription(command, code) +
                                " " + filePath.RemoteDirectoryPath, code));
                        break;

                    case "DNEM":
                        // Director is not empty.
                        if (OnCommandError != null)
                            OnCommandError(this, new ClientCommandArgs(command,
                                Channel.FileTransferProtocol.GetFileTransferServerResponseDescription(command, code), code));
                        break;

                    case "DDEX":
                        // Director does not exists.
                        if (OnCommandError != null)
                            OnCommandError(this, new ClientCommandArgs(command,
                                Channel.FileTransferProtocol.GetFileTransferServerResponseDescription(command, code), code));
                        break;

                    case "DEDI":
                        // Directory was deleted.
                        if (OnProcessComplete != null)
                            OnProcessComplete(this, new ClientCommandArgs(command,
                                Channel.FileTransferProtocol.GetFileTransferServerResponseDescription(command, code) +
                                " " + filePath.RemoteDirectoryPath, code));
                        break;

                    case "DEFL":
                        // File was deleted.
                        if (OnProcessComplete != null)
                            OnProcessComplete(this, new ClientCommandArgs(command,
                                Channel.FileTransferProtocol.GetFileTransferServerResponseDescription(command, code) +
                                " " + filePath.RemoteFile, code));
                        break;

                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                if (OnAsyncThreadError != null)
                    OnAsyncThreadError(this, new ClientThreadErrorArgs(
                        e.Message, 800));
            }
        }

        /// <summary>
        /// Clean up the current state object.
        /// </summary>
        /// <param name="state">The current state object.</param>
        private void StateCleanUp(FileTransferState state)
        {
            if (state != null)
            {
                try
                {
                    // Get the current network stream from the
                    // asynchronus result state object.
                    FileTransferState stateObject = (FileTransferState)state;

                    // Get the current file stream.
                    FileStream fileStream = stateObject.FileStream;

                    // Close the file and and dispose
                    // of the object.
                    if (fileStream != null)
                    {
                        // Close the file.
                        fileStream.Close();

                        // Delete the current download file.
                        File.Delete(filePath.DestinationFile);

                        // Release all resources
                        fileStream.Dispose();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Clean up the files.
        /// </summary>
        /// <param name="state">The current state object.</param>
        private void FileCleanUp(FileTransferState state)
        {
            try
            {
                // If the download file exists.
                if(File.Exists(filePath.DestinationFile))
                {
                    // Get the current download
                    // file information.
                    FileInfo fileInfo = new FileInfo(filePath.DestinationFile);

                    // If the downloaded file has not
                    // been completely downloaded.
                    if(fileInfo.Length < filePath.DestinationFileSize)
                        // Delete the current download file.
                        File.Delete(filePath.DestinationFile);
                }
            }
            catch { }
        }

        /// <summary>
        /// Data received asynchronus result method, all client commands
        /// are processed through this asynchronus result method.
        /// </summary>
        /// <param name="ar">The current asynchronus result.</param>
        private void DataReceiver(IAsyncResult ar)
        {
            int bytesRead;

            // Get the current network stream from the
            // asynchronus result state object.
            FileTransferState state = (FileTransferState)ar.AsyncState;
            NetworkStream nStream = state.NetworkStream;

            try
            {
                // If the current operation is downloading data.
                if (downloadingFile)
                {
                    // Reset download loop read count.
                    readDownLoadLoopCount = 0;

                    // Start the asynchronus download operation.
                    DownloadFile(ar);
                }
                else
                {
                    // Close the file.
                    if (state.FileStream != null)
                        state.FileStream.Close();

                    // Clean up the file stream.
                    state.FileStream = null;

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
                            string command = Encoding.ASCII.GetString(readBuffer, 0, bytesRead);
                            CommandReceived(command);
                        }
                        catch { }
                    }
                    else
                    {
                        // If the connection is closed
                        // then throw a new exception.
                        if (!client.Connected)
                            throw new Exception("Connection has closed.");

                        // Start counting loops.
                        readLoopCount++;

                        // If count greater than
                        // 10 then throw exception
                        // and close the connection.
                        if (readLoopCount > 4)
                            throw new Exception("Connection has been disconnected, read loop detected. " +
                                "Connection has timed out or, may occur when client calls Shutdown(SocketShutdown.Both)");
                    }

                    // Start a new asynchronous read into readBuffer.
                    // more data maybe present. This will wait for more
                    // data from the client.
                    lock (nStream)
                        nStream.BeginRead(readBuffer, 0, READ_BUFFER_SIZE,
                            new AsyncCallback(DataReceiver), state);
                }
            }
            catch (System.InvalidOperationException ioe)
            {
                string error = ioe.Message;

                // Close the file.
                if (state.FileStream != null)
                    state.FileStream.Close();

                // Clean up the file stream.
                StateCleanUp(state);
            }
            catch (Exception e)
            {
                if (OnAsyncThreadError != null)
                    OnAsyncThreadError(this, new ClientThreadErrorArgs(
                        e.Message, 801));

                // Clean up the file stream.
                StateCleanUp(state);
            }
        }

        /// <summary>
        /// This method handles the download asynchronus result
        /// when the server receives a file from the client.
        /// </summary>
        /// <param name="ar">The current asynchronus result</param>
        private void DownloadFile(IAsyncResult ar)
        {
            // Get the current network stream from the
            // asynchronus result state object.
            FileTransferState state = (FileTransferState)ar.AsyncState;
            NetworkStream nStream = state.NetworkStream;

            long totalBytesRead = 0;
            FileStream fileStream = null;

            try
            {
                // If the file state file stream has
                // been created then get the stream
                // from the state object.
                if (state.FileStream != null)
                {
                    fileStream = state.FileStream;
                    totalBytesRead = state.TotalBytesRead;
                }
                else
                {
                    // Create a new download file stream.
                    fileStream = new FileStream(filePath.DestinationFile,
                        FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

                    // Get the network stream and file stream
                    // form the state object.
                    state.NetworkStream = nStream;
                    state.FileStream = fileStream;
                    state.TotalBytesRead = 0;
                }
            }
            catch (Exception e)
            {
                // Stop the download.
                downloadingFile = false;

                if (OnAsyncThreadError != null)
                    OnAsyncThreadError(this, new ClientThreadErrorArgs(
                        e.Message, 806));
            }

            try
            {
                // End the current read process.
                int bytesRead = nStream.EndRead(ar);

                if (bytesRead < 1)
                {
                    // If the connection is closed
                    // then throw a new exception.
                    if (!client.Connected)
                        throw new Exception("Connection has closed.");

                    // Start counting loops.
                    readDownLoadLoopCount++;

                    // If count greater than
                    // 10 then throw exception
                    // and close the connection.
                    if (readDownLoadLoopCount > 4)
                        throw new Exception("Connection has been disconnected, read loop detected. " +
                            "Connection has timed out or, may occur when client calls Shutdown(SocketShutdown.Both)");
                }

                // If not the first time reading
                // data from the network stream
                // the load the data received
                // to the file.
                fileStream.Write(readBuffer, 0, bytesRead);
                totalBytesRead += (long)bytesRead;

                // Send to the client the current bytes that
                // have been read.
                if (_downloadProgressCallback != null)
                    _downloadProgressCallback((long)bytesRead);

                // Download the current data.
                totalBytesRead += DownloadFileEx(fileStream, nStream, totalBytesRead);
            }
            catch (Exception e)
            {
                // Stop the download.
                downloadingFile = false;

                if (OnAsyncThreadError != null)
                    OnAsyncThreadError(this, new ClientThreadErrorArgs(
                        e.Message, 802));
            }

            try
            {
                // If the download file is the same size
                // as the file current being written then
                // the download has completed and data has
                // been received.
                if (filePath.DestinationFileSize <= totalBytesRead)
                    downloadingFile = false;

                if (!downloadingFile)
                {
                    // Close the file.
                    if (fileStream != null)
                        fileStream.Close();

                    // Release the object.
                    fileStream = null;
                    state.FileStream = null;

                    // Send to the client an indicator
                    // that the download has completed.
                    if (_downloadCompleteCallback != null)
                        _downloadCompleteCallback(filePath.RemoteFile);

                    //Send download complete to the data channel connection.
                    if (_operationCompleteCallback != null)
                        _operationCompleteCallback.Invoke(true);

                    // Make sure that the file
                    // has been completely downloaded
                    // if not then delete the file.
                    FileCleanUp(state);

                    // Start a new asynchronous read into readBuffer.
                    // more data maybe present. This will wait for more
                    // data from the client.
                    System.Threading.Thread.Sleep(10);
                    lock (nStream)
                        nStream.BeginRead(readBuffer, 0, READ_BUFFER_SIZE,
                            new AsyncCallback(DataReceiver), state);
                }
                else
                {
                    // The current bytes read in this async threa.
                    state.TotalBytesRead = totalBytesRead;

                    // Start a new asynchronous read into, more
                    // of the file being download exists so read
                    // from the network stream until all data 
                    // has been received.
                    lock (nStream)
                        nStream.BeginRead(readBuffer, 0, READ_BUFFER_SIZE,
                            new AsyncCallback(DownloadFile), state);
                }
            }
            catch (Exception e)
            {
                if (OnAsyncThreadError != null)
                    OnAsyncThreadError(this, new ClientThreadErrorArgs(
                        e.Message, 803));

                // Clean up the file stream.
                StateCleanUp(state);
            }
        }

        /// <summary>
        /// Load the data into the file.
        /// </summary>
        /// <param name="fileStream">The current file stream object.</param>
        /// <param name="nStream">The current network client stream.</param>
        /// <param name="numberBytesRead">The current number of bytes read.</param>
        private long DownloadFileEx(FileStream fileStream, NetworkStream nStream, long numberBytesRead)
        {
            int readBytes = 0;
            long totalBytesRead = 0;
            const int bufferLength = READ_BUFFER_SIZE;
            byte[] buffer = new byte[READ_BUFFER_SIZE];

            // The read time out.
            nStream.ReadTimeout = timeout;

            // If client is connected and the number of bytes read
            // is less than the total size then read all bytes.
            while ((client.Connected) &&
                (totalBytesRead + numberBytesRead < filePath.DestinationFileSize))
            {
                // Read all the data available in the network stream
                // and write the data to the file stream.
                while (nStream.DataAvailable)
                {
                    readBytes = nStream.Read(buffer, 0, bufferLength);
                    fileStream.Write(buffer, 0, readBytes);
                    totalBytesRead += (long)readBytes;

                    // Send to the client the current bytes that
                    // have been read.
                    if (_downloadProgressCallback != null)
                        _downloadProgressCallback((long)readBytes);
                }

                // Wait for the stop download signal
                // for a specified time before continuing.
                int index = WaitHandle.WaitAny(_waitOnEvents, 0, false);
                if (index == 0)
                    // Return the number of bytes to
                    // indicate that the transfer
                    // has completed.
                    return filePath.DestinationFileSize;

                // Allow any other threads to work by
                // changing context.
                //System.Threading.Thread.Sleep(2);
            }

            // Return the number of bytes read.
            return totalBytesRead;
        }

        /// <summary>
        /// Starts the upload process.
        /// </summary>
        private void UploadEx()
        {
            FileStream fileStream = null;
            FileTransferState state = new FileTransferState();

            try
            {
                // Open the upload file.
                fileStream = new FileStream(filePath.TargetFile,
                    FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // Get the current network stream
                // from the socket.
                NetworkStream netStream = client.GetStream();

                // Create a new file state object
                // assign the current network stream
                // and the upload file stream.
                state.NetworkStream = netStream;
                state.FileStream = fileStream;

                // Get the first set of bytes from
                // the upload file.
                int readBytes = fileStream.Read(writeBuffer, 0, WRITE_BUFFER_SIZE);

                // Send to the client the current bytes that
                // have been read.
                if (_uploadProgressCallback != null)
                    _uploadProgressCallback((long)readBytes);

                // Begin a new asynochronus write operation
                // to send the client the requested file.
                lock (netStream)
                    netStream.BeginWrite(writeBuffer, 0, readBytes,
                        new AsyncCallback(UploadFile), state);
            }
            catch (Exception e)
            {
                if (fileStream != null)
                    fileStream.Close();

                if (OnAsyncThreadError != null)
                    OnAsyncThreadError(this, new ClientThreadErrorArgs(
                        e.Message, 807));

                // Clean up the file stream.
                state.FileStream = null;
                StateCleanUp(state);
            }
        }

        /// <summary>
        /// This method handles the upload asynchronus result
        /// when the client sends a file to the server.
        /// </summary>
        /// <param name="ar">The current asynchronus result</param>
        private void UploadFile(IAsyncResult ar)
        {
            // Get the current network stream from the
            // asynchronus result state object.
            FileTransferState state = (FileTransferState)ar.AsyncState;
            NetworkStream nStream = state.NetworkStream;
            FileStream fileStream = state.FileStream;

            lock (nStream)
                // End the current asynchronus write
                // operation.
                nStream.EndWrite(ar);

            try
            {
                int readBytes = 0;
                const int bufferLength = WRITE_BUFFER_SIZE;
                byte[] buffer = new byte[bufferLength];

                // The write time out.
                nStream.WriteTimeout = timeout;

                // Read all the data in the upload file and
                // send the data from the file to the client 
                // through the current network stream.
                do
                {
                    readBytes = fileStream.Read(buffer, 0, bufferLength);
                    nStream.Write(buffer, 0, readBytes);

                    // Send to the client the current bytes that
                    // have been read.
                    if (_uploadProgressCallback != null)
                        _uploadProgressCallback((long)readBytes);

                    // Wait for the stop download signal
                    // for a specified time before continuing.
                    int index = WaitHandle.WaitAny(_waitOnEvents, 0, false);
                    if (index == 1)
                        // Set the read bytes to
                        // zero indicating exit
                        // the loop.
                        readBytes = 0;
                }
                while (readBytes != 0);

                // Close the current upload file.
                fileStream.Close();
            }
            catch (Exception e)
            {
                if (OnAsyncThreadError != null)
                    OnAsyncThreadError(this, new ClientThreadErrorArgs(
                        e.Message, 804));

                // Clean up the file stream.
                state.FileStream = null;
                StateCleanUp(state);
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }
        }

        /// <summary>
        /// Get the direct read data from the server.
        /// This is used for all non upload and download
        /// read operations.
        /// </summary>
        /// <param name="receivedData">The data returned by the server.</param>
        private void GetData(string receivedData)
        {
            // Get the concantanted
            // data from the server.
            directDataReceived += receivedData;

            // If all the data was returned
            // then end the read operation.
            if (directDataLength <= (long)directDataReceived.Length)
                directDataRead = false;

            // If the read operation has ended.
            if (!directDataRead)
            {
                // Get the current command.
                switch (readCommand)
                {
                    case "GCDI":
                        // Get the directory path;
                        filePath.RemoteDirectoryPath = directDataReceived.Trim();
                        _internalFilePath.RemoteDirectoryPath = directDataReceived.Trim();
                        break;
                }

                // Send to the client the
                // data from the server.
                if (OnReadDataComplete != null)
                    OnReadDataComplete(this, new ClientCommandArgs(readCommand,
                        directDataReceived, readCode));

                // Reset the initialising data.
                directDataLength = 0;
                directDataReceived = string.Empty;
            }
        }

        /// <summary>
        /// Sends a command and data to the current server.
        /// </summary>
        /// <param name="command">The command and data to write.</param>
        private void SendCommand(string command)
        {
            StreamWriter writer = null;

            try
            {
                // Write the command to the client.
                lock (networkStream)
                {
                    writer = new StreamWriter(networkStream);
                    writer.Write(command);
                    writer.Flush();
                    writer = null;
                }
            }
            catch (Exception e)
            {
                if (OnAsyncThreadError != null)
                    OnAsyncThreadError(this, new ClientThreadErrorArgs(
                        e.Message, 805));
            }
            finally
            {
                if (writer != null)
                    writer = null;
            }
        }

        /// <summary>
        /// Validate all objects used by the client before connection.
        /// </summary>
        private void DataValidation()
        {
            // Make sure that a connection has been
            // established before execution.
            if (!userConnected)
                throw new System.Exception("No connection has been established.", null);

            // Make sure that the a valid TransferDirection
            // has been chosen.
            if (Operation == Channel.Operation.None)
                throw new System.Exception("No Transfer Direction has been set.", null);

            // The current transfer operation
            // required.
            switch (operation)
            {
                case Channel.Operation.Download:
                    // Make sure that the FilePath type has been created.
                    if (filePath == null)
                        throw new System.ArgumentNullException("FilePath can not be null.",
                            new System.Exception("The FilePath reference has not been set."));

                    // Make sure the download destination file has
                    // been specidied.
                    if (String.IsNullOrEmpty(filePath.DestinationFile))
                        throw new System.ArgumentNullException("A local destination file must be specified.",
                            new System.Exception("No valid local destination file has been specified."));

                    // Extract the directory path.
                    string sFolderPath = System.IO.Path.GetDirectoryName(filePath.DestinationFile);

                    // If the directory does not exists create it.
                    if (!Directory.Exists(sFolderPath))
                        Directory.CreateDirectory(sFolderPath);

                    // Make sure that a valid remote file has been specified.
                    if (String.IsNullOrEmpty(filePath.RemoteFile))
                        throw new System.ArgumentNullException("A remote file has not been specified.",
                            new System.Exception("No valid remote file was specified to the server."));
                    break;

                case Channel.Operation.Upload:
                    // Make sure that the FilePath type has been created.
                    if (filePath == null)
                        throw new System.ArgumentNullException("FilePath can not be null.",
                            new System.Exception("The FilePath reference has not been set."));

                    // Make sure the upload target file has
                    // been specidied.
                    if (String.IsNullOrEmpty(filePath.TargetFile))
                        throw new System.ArgumentNullException("A local target file must be specified.",
                            new System.Exception("No valid local target file has been specified."));

                    // Make sure the upload target file exists.
                    if (!File.Exists(filePath.TargetFile))
                        throw new System.Exception("The local target file does not exist.");

                    // Get the upload target file size.
                    FileInfo fileInfo = new FileInfo(filePath.TargetFile);
                    filePath.TargetFileSize = fileInfo.Length;

                    // Make sure that a valid remote file has been specified.
                    if (String.IsNullOrEmpty(filePath.RemoteFile))
                        throw new System.ArgumentNullException("A remote file has not been specified.",
                            new System.Exception("No valid remote file was specified to the server."));
                    break;
            }
        }

        /// <summary>
        /// Validate all objects used by the client before connection.
        /// </summary>
        private void ClientValidation()
        {
            // Make sure that the Connection type has been created.
            if (connection == null)
                throw new System.ArgumentNullException("Connection can not be null.",
                    new System.Exception("The Connection reference has not been set."));

            // Make sure that the Credentials type has been created.
            if (credentials == null)
                throw new System.ArgumentNullException("Credentials can not be null.",
                    new System.Exception("The Credentials reference has not been set."));

            // Make sure that a valid username has been specified.
            if (String.IsNullOrEmpty(credentials.UserName))
                throw new System.ArgumentNullException("A valid username has not been specified.",
                    new System.Exception("No valid user credentials are set, set a valid username."));

            // Make sure that a valid password has been specified.
            if (String.IsNullOrEmpty(credentials.Password))
                throw new System.ArgumentNullException("A valid password has not been specified.",
                    new System.Exception("No valid user password was set, set a valid password."));

            // Make sure that a valid host has been specified.
            if (String.IsNullOrEmpty(connection.Host))
                throw new System.ArgumentNullException("A valid host name has not been specified.",
                    new System.Exception("No valid remote host was specified to send data to."));

            // Make sure the the port number is greater than one.
            if (connection.Port < 1)
                throw new System.ArgumentOutOfRangeException("The port number must be greater than zero.",
                    new System.Exception("No valid port number was set, set a valid port number."));
        }
        #endregion
    }
}
