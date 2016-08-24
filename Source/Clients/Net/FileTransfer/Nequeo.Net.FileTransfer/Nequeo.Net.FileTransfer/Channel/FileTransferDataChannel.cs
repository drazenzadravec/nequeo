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

namespace Nequeo.Net.FileTransfer.Channel
{
    /// <summary>
    /// Upload progress call back handler.
    /// </summary>
    /// <param name="bytesRead">Number of bytes read.</param>
    public delegate void UploadProgressCallback(long bytesRead);

    /// <summary>
    /// Download progress call back handler.
    /// </summary>
    /// <param name="bytesRead">Number of bytes read.</param>
    public delegate void DownloadProgressCallback(long bytesRead);

    /// <summary>
    /// Upload complete call back handler.
    /// </summary>
    /// <param name="remoteFile">The remote file being uploaded.</param>
    public delegate void UploadCompleteCallback(string remoteFile);

    /// <summary>
    /// Download complete call back handler.
    /// </summary>
    /// <param name="remoteFile">The remote file being downloaded.</param>
    public delegate void DownloadCompleteCallback(string remoteFile);

    /// <summary>
    /// Client command handler.
    /// </summary>
    /// <param name="sender">The object sender.</param>
    /// <param name="e">The object containing the operation data.</param>
    public delegate void ClientThreadErrorHandler(object sender, ClientThreadErrorArgs e);

    /// <summary>
    /// Client command handler.
    /// </summary>
    /// <param name="sender">The object sender.</param>
    /// <param name="e">The object containing the operation data.</param>
    public delegate void ClientCommandHandler(object sender, ClientCommandArgs e);

    /// <summary>
    /// Class contains a data channel connection
    /// to the server with passed data.
    /// </summary>
    internal sealed class FileTransferDataChannel
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="uploadProgressCallback">Upload progress call back delegate.</param>
        /// <param name="downloadProgressCallback">Download progress call back delegate.</param>
        /// <param name="uploadCompleteCallback">Upload complete call back delegate.</param>
        /// <param name="downloadCompleteCallback">Download complete call back delegate.</param>
        /// <param name="onCommandError">On error delegate event handler.</param>
        /// <param name="onAsyncThreadError">On async thread error delegate event handler.</param>
        /// <param name="credentials">The user credentials.</param>
        /// <param name="connection">The server connection data.</param>
        /// <param name="filePath">The file path transfer data.</param>
        /// <param name="internalFilePath">The internal file path transfer data.</param>
        /// <param name="operation">The current operation.</param>
        /// <param name="secureConnection">Is the connection secure.</param>
        /// <param name="timeout">The time out for the operation.</param>
        public FileTransferDataChannel(
            UploadProgressCallback uploadProgressCallback, DownloadProgressCallback downloadProgressCallback,
            UploadCompleteCallback uploadCompleteCallback, DownloadCompleteCallback downloadCompleteCallback,
            ClientCommandHandler onCommandError, ClientThreadErrorHandler onAsyncThreadError,
            NetworkCredential credentials, FileTransferConnection connection, FileTransferPath filePath,
            FileTransferPath internalFilePath, Operation operation, bool secureConnection, int timeout)
        {
            _connection = new FileTransferConnection();
            _connection.Host = connection.Host;
            _connection.Port = connection.Port;
            _connection.ValidateCertificate = false;
            _connection.UseDataChannel = false;

            _secureConnection = secureConnection;
            _credentials = credentials;
            _filePath = filePath;
            _operation = operation;
            _timeout = timeout;

            _internalFilePath = internalFilePath;

            _onCommandError = onCommandError;
            _onAsyncThreadError = onAsyncThreadError;

            _uploadProgressCallback = uploadProgressCallback;
            _downloadProgressCallback = downloadProgressCallback;
            _uploadCompleteCallback = uploadCompleteCallback;
            _downloadCompleteCallback = downloadCompleteCallback;
        }
        #endregion

        #region Private Fields
        private Client _socket = null;
        private SslClient _sslSocket = null;

        private ClientCommandHandler _onCommandError = null;
        private ClientThreadErrorHandler _onAsyncThreadError = null;

        private NetworkCredential _credentials = null;
        private FileTransferConnection _connection = null;
        private FileTransferPath _filePath = null;
        private FileTransferPath _internalFilePath = null;

        private int _timeout = -1;
        private bool _secureConnection = false;

        private UploadProgressCallback _uploadProgressCallback = null;
        private DownloadProgressCallback _downloadProgressCallback = null;
        private UploadCompleteCallback _uploadCompleteCallback = null;
        private DownloadCompleteCallback _downloadCompleteCallback = null;

        private Channel.Operation _operation = Channel.Operation.None;

        #endregion

        #region Public Methods
        /// <summary>
        /// Abort the current operation.
        /// </summary>
        public void Abort()
        {
            if (_secureConnection)
            {
                _sslSocket.Close(false);
            }
            else
            {
                _socket.Close(false);
            }
        }

        /// <summary>
        /// Start the async operation.
        /// </summary>
        public void StartOperation()
        {
            if (_secureConnection)
                SecureDataChannel();
            else
                NonSecureDataChannel();
        }

        /// <summary>
        /// Close the connection.
        /// </summary>
        /// <param name="sendQuitCommand">Send a quit command.</param>
        public void Close(bool sendQuitCommand)
        {
            if (_secureConnection)
                _sslSocket.Close(sendQuitCommand);
            else
                _socket.Close(sendQuitCommand);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Secure data channel connection.
        /// </summary>
        private void SecureDataChannel()
        {
            // Create a new instance and
            // assign the properties.
            _sslSocket = new SslClient()
            {
                Connection = _connection,
                Credentials = _credentials,
                FilePath = _filePath,
                Operation = _operation,
                Timeout = _timeout,
                InternalFilePath = _internalFilePath,
                UploadProgressCallback = _uploadProgressCallback,
                DownloadProgressCallback = _downloadProgressCallback,
                UploadCompleteCallback = _uploadCompleteCallback,
                DownloadCompleteCallback = _downloadCompleteCallback
            };

            // Set the event handlers.
            _sslSocket.OnCommandError += _onCommandError;
            _sslSocket.OnAsyncThreadError += _onAsyncThreadError;
            _sslSocket.OnCommandConnected += new ClientCommandHandler(_sslSocket_OnCommandConnected);
            _sslSocket.OperationCompleteCallback = new OperationCompleteCallback(Close);

            // Open a new connection.
            _sslSocket.Open();
        }

        /// <summary>
        /// Non secure data channel connection.
        /// </summary>
        private void NonSecureDataChannel()
        {
            // Create a new instance and
            // assign the properties.
            _socket = new Client()
            {
                Connection = _connection,
                Credentials = _credentials,
                FilePath = _filePath,
                Operation = _operation,
                Timeout = _timeout,
                InternalFilePath = _internalFilePath,
                UploadProgressCallback = _uploadProgressCallback,
                DownloadProgressCallback = _downloadProgressCallback,
                UploadCompleteCallback = _uploadCompleteCallback,
                DownloadCompleteCallback = _downloadCompleteCallback
            };

            // Set the event handlers.
            _socket.OnCommandError += _onCommandError;
            _socket.OnAsyncThreadError += _onAsyncThreadError;
            _socket.OnCommandConnected += new ClientCommandHandler(_socket_OnCommandConnected);
            _socket.OperationCompleteCallback = new OperationCompleteCallback(Close);

            // Open a new connection.
            _socket.Open();
        }

        /// <summary>
        /// When a secure connection has been made.
        /// </summary>
        /// <param name="sender">The sender of the data.</param>
        /// <param name="e">The argument data.</param>
        private void _sslSocket_OnCommandConnected(object sender, ClientCommandArgs e)
        {
            // Set the server path and execute.
            // The path is now set.
            _sslSocket.Operation = Channel.Operation.SetPath;
            _sslSocket.FilePath.RemoteDirectoryPath = _internalFilePath.RemoteDirectoryPath;
            _sslSocket.Execute();

            // Execute the operation.
            _sslSocket.Operation = _operation;
            _sslSocket.FilePath = _filePath;
            _sslSocket.Execute();
        }

        /// <summary>
        /// When a connection has been made.
        /// </summary>
        /// <param name="sender">The sender of the data.</param>
        /// <param name="e">The argument data.</param>
        private void _socket_OnCommandConnected(object sender, ClientCommandArgs e)
        {
            // Set the server path and execute.
            // The path is now set.
            _socket.Operation = Channel.Operation.SetPath;
            _socket.FilePath.RemoteDirectoryPath = _internalFilePath.RemoteDirectoryPath;
            _socket.Execute();

            // Execute the operation.
            _socket.Operation = _operation;
            _socket.FilePath = _filePath;
            _socket.Execute();
        }
        #endregion
    }
}
