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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using System.Runtime.Serialization;

using Nequeo.Invention;

namespace Nequeo.Net.Mail.Imap
{
    /// <summary>
    /// The imap4 client class to the server.
    /// </summary>
    public class Imap4Client : IDisposable, IImap4Client
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Imap4Client()
        {
        }

        /// <summary>
        /// Connection adapter constructor.
        /// </summary>
        /// <param name="connection">The connection adapter used to connect to the server.</param>
        public Imap4Client(Imap4ConnectionAdapter connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Connection adapter constructor.
        /// </summary>
        /// <param name="connection">The connection adapter used to connect to the server.</param>
        /// <param name="writeAttachmentToFile">Write the attachemnt to the file.</param>
        public Imap4Client(Imap4ConnectionAdapter connection,
            bool writeAttachmentToFile)
        {
            _connection = connection;
            _writeAttachmentToFile = writeAttachmentToFile;
        }
        #endregion

        #region Private Fields
        private Socket _socket = null;
        private TcpClient _client = null;
        private Message _message = null;
        private SslStream _sslStream = null;
        private Imap4ConnectionAdapter _connection = null;
        private Nequeo.Threading.ActionHandler<long> _callbackHandler = null;
        private Nequeo.Security.X509Certificate2Info _sslCertificate = null;

        private byte[] _buffer = new byte[EmailMessageParse.MAX_BUFFER_READ_SIZE];

        private bool _userConnected = false;
        private bool _clientConnected = false;
        private bool _writeAttachmentToFile = false;
        private bool _rawEmailMessageOnly = false;

        private string _prefixNumber = string.Empty;

        private bool _disposed = false;
        private bool _startTLS = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get set, the connection adapter.
        /// </summary>
        public Imap4ConnectionAdapter Connection
        {
            set { _connection = value; }
            get { return _connection; }
        }

        /// <summary>
        /// Get set, the message retreival progress call back handler.
        /// </summary>
        public Nequeo.Threading.ActionHandler<long> MessageReturnProgress
        {
            set { _callbackHandler = value; }
            get { return _callbackHandler; }
        }

        /// <summary>
        /// Get, the secure certificate..
        /// </summary>
        public Nequeo.Security.X509Certificate2Info Certificate
        {
            get { return _sslCertificate; }
        }

        /// <summary>
        /// Get, the currrent message data.
        /// </summary>
        public Message Message
        {
            get { return _message; }
        }

        /// <summary>
        /// Get set, the write attachment to file indicator.
        /// </summary>
        public bool WriteAttachmentToFile
        {
            set { _writeAttachmentToFile = value; }
            get { return _writeAttachmentToFile; }
        }

        /// <summary>
        /// Get set, the complete raw email message only.
        /// </summary>
        public bool RawEmailMessageOnly
        {
            set { _rawEmailMessageOnly = value; }
            get { return _rawEmailMessageOnly; }
        }

        /// <summary>
        /// Get, the current connection state.
        /// </summary>
        public bool Connected
        {
            get { return _userConnected; }
        }

        /// <summary>
        /// Get, an indicator for secure TLS connection.
        /// </summary>
        public bool SecurelyConnected
        {
            get { return _startTLS; }
        }
        #endregion

        #region Public Events
        /// <summary>
        /// On command return error event.
        /// </summary>
        public event Nequeo.Threading.EventHandler<ClientCommandArgs> OnCommandError;

        /// <summary>
        /// On validation return error event.
        /// </summary>
        public event Nequeo.Threading.EventHandler<ClientThreadErrorArgs> OnValidationError;

        /// <summary>
        /// On command return error event.
        /// </summary>
        public event Nequeo.Threading.EventHandler<ClientCommandArgs> OnCommandConnected;

        /// <summary>
        /// When an error occures on an asynchronous threa.
        /// </summary>
        public event Nequeo.Threading.EventHandler<ClientThreadErrorArgs> OnAsyncThreadError;
        #endregion

        #region Public Methods
        /// <summary>
        /// Open a new connection.
        /// </summary>
        public virtual void Open()
        {
            // Validate all the data.
            ClientValidation();

            _socket = null;
            _client = null;
            _sslStream = null;
            _startTLS = false;

            // If a secure connection is required.
            if (_connection.UseSSLConnection &&
                (_connection.ServerType == Nequeo.Net.ServerType.SslIMAP4 ||
                _connection.ServerType == Nequeo.Net.ServerType.SslProxyIMAP4))
            {
                // Get the tcp client socket to
                // the server.
                _client = GetTcpClient();
                _startTLS = true;
            }
            else
                // Get the client socket to
                // the server.
                _socket = GetSocket();

            // Has connection been made to
            // the server.
            if (_socket == null && _client == null)
                throw new Exception("A connection could not be established.");
            else
            {
                // Get the response from the server.
                string header = GetServerResponse();

                // If an invalid resonse was received
                // then throw a new exception.
                if (!header.ToUpper().Substring(0, 2).Equals("OK"))
                    throw new Exception("Invalid initial server response.");

                // If a secure connection is required.
                if (_connection.UseSSLConnection &&
                    _connection.ServerType == Nequeo.Net.ServerType.StartTslProxyIMAP4)
                {
                    // Send a start TLS command to the server.
                    SendCommand("A999 STARTTLS");

                    // Get the response from the server.
                    string returned = GetServerResponse();

                    // If an invalid resonse was received
                    // then throw a new exception.
                    if (!header.ToUpper().Substring(0, 2).Equals("OK"))
                        throw new Exception("Invalid initial server response.");

                    // Get the client and the ssl
                    // client socket for TLS connection.
                    _client = GetTcpClient();

                    // Make sure that a secure connection
                    // was negotiated.
                    if (_sslStream != null)
                        if (_sslStream.IsAuthenticated)
                            _startTLS = true;

                    // Make sure a secure connection has been
                    // established with the server.
                    if (_startTLS)
                    {
                        // Set the connection status to true
                        // the client is connected.
                        _clientConnected = true;

                        // Login the current client.
                        Login();
                    }
                }
                else
                {
                    // Set the connection status to true
                    // the client is connected.
                    _clientConnected = true;

                    // Login the current client.
                    Login();
                }
            }
        }

        /// <summary>
        /// Close the current connection.
        /// </summary>
        public virtual void Close()
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            if (_socket != null)
            {
                if (_clientConnected)
                {
                    // Send a quit command to the server.
                    SendCommand(_prefixNumber + " LOGOUT");
                    _socket.Close();

                    // Set the client to no connection.
                    _clientConnected = false;
                }

                _socket = null;
            }

            if (_client != null)
            {
                if (_clientConnected)
                {
                    // Send a quit command to the server.
                    SendCommand(_prefixNumber + " LOGOUT");
                    _client.Close();

                    // Set the client to no connection.
                    _clientConnected = false;
                }

                if (_sslStream != null)
                    _sslStream.Close();

                // Clear from memory.
                _client = null;
                _sslStream = null;
            }
        }

        /// <summary>
        /// Creates a new folder on the server.
        /// </summary>
        /// <param name="folderName">The folder to create.</param>
        /// <returns>True if the operation was successful else false.</returns>
        public virtual bool CreateFolder(string folderName)
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            bool valid = true;
            bool ret = false;

            // If no connection exists
            // then thrown an exception.
            if ((_socket == null && _client == null) || (!_clientConnected))
            {
                if (OnValidationError != null)
                    OnValidationError(this, new ClientThreadErrorArgs(
                        "Not connected to a server.", 500));

                // Not connected.
                valid = false;
            }

            // If valid.
            if (valid)
            {
                // If the folderName is empty than
                // throw exception.
                if (String.IsNullOrEmpty(folderName))
                {
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "Folder name can not be empty or null.", 503));

                    // Not valid message position.
                    valid = false;
                }

                // If valid.
                if (valid)
                {
                    // If user connected.
                    if (_userConnected)
                    {
                        string returned;

                        // Send the create folder command.
                        SendCommand(_prefixNumber + " CREATE " + folderName + "/");

                        // Get the server response.
                        returned = GetServerResponse();

                        // If the server do not send
                        // a comfirmation response.
                        if (!returned.ToUpper().Substring(0, 2).Equals("OK"))
                        {
                            if (OnCommandError != null)
                                OnCommandError(this, new ClientCommandArgs("CREATE",
                                    "Server response : " + returned, 402));
                        }
                        else 
                            ret = true;
                    }
                    else
                        if (OnValidationError != null)
                            OnValidationError(this, new ClientThreadErrorArgs(
                                "User has not logged in.", 502));
                }
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Deletes the folder on the server.
        /// </summary>
        /// <param name="folderName">The folder to delete.</param>
        /// <returns>True if the operation was successful else false.</returns>
        public virtual bool DeleteFolder(string folderName)
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            bool valid = true;
            bool ret = false;

            // If no connection exists
            // then thrown an exception.
            if ((_socket == null && _client == null) || (!_clientConnected))
            {
                if (OnValidationError != null)
                    OnValidationError(this, new ClientThreadErrorArgs(
                        "Not connected to a server.", 500));

                // Not connected.
                valid = false;
            }

            // If valid.
            if (valid)
            {
                // If the folderName is empty than
                // throw exception.
                if (String.IsNullOrEmpty(folderName))
                {
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "Folder name can not be empty or null.", 503));

                    // Not valid message position.
                    valid = false;
                }

                // If valid.
                if (valid)
                {
                    // If user connected.
                    if (_userConnected)
                    {
                        string returned;

                        // Send the create folder command.
                        SendCommand(_prefixNumber + " DELETE " + folderName);

                        // Get the server response.
                        returned = GetServerResponse();

                        // If the server do not send
                        // a comfirmation response.
                        if (!returned.ToUpper().Substring(0, 2).Equals("OK"))
                        {
                            if (OnCommandError != null)
                                OnCommandError(this, new ClientCommandArgs("DELETE",
                                    "Server response : " + returned, 403));
                        }
                        else
                            ret = true;
                    }
                    else
                        if (OnValidationError != null)
                            OnValidationError(this, new ClientThreadErrorArgs(
                                "User has not logged in.", 502));
                }
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Rename the folder on the server.
        /// </summary>
        /// <param name="sourceFolderName">The source folder.</param>
        /// <param name="destinationFolderName">The destination folder.</param>
        /// <returns>True if the operation was successful else false.</returns>
        public virtual bool RenameFolder(string sourceFolderName, string destinationFolderName)
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            bool valid = true;
            bool ret = false;

            // If no connection exists
            // then thrown an exception.
            if ((_socket == null && _client == null) || (!_clientConnected))
            {
                if (OnValidationError != null)
                    OnValidationError(this, new ClientThreadErrorArgs(
                        "Not connected to a server.", 500));

                // Not connected.
                valid = false;
            }

            // If valid.
            if (valid)
            {
                // If the folderName is empty than
                // throw exception.
                if (String.IsNullOrEmpty(sourceFolderName) || String.IsNullOrEmpty(destinationFolderName))
                {
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "Folder name can not be empty or null.", 503));

                    // Not valid message position.
                    valid = false;
                }

                // If valid.
                if (valid)
                {
                    // If user connected.
                    if (_userConnected)
                    {
                        string returned;

                        // Send the create folder command.
                        SendCommand(_prefixNumber + " RENAME " + sourceFolderName + 
                            " " + destinationFolderName);

                        // Get the server response.
                        returned = GetServerResponse();

                        // If the server do not send
                        // a comfirmation response.
                        if (!returned.ToUpper().Substring(0, 2).Equals("OK"))
                        {
                            if (OnCommandError != null)
                                OnCommandError(this, new ClientCommandArgs("RENAME",
                                    "Server response : " + returned, 404));
                        }
                        else
                            ret = true;
                    }
                    else
                        if (OnValidationError != null)
                            OnValidationError(this, new ClientThreadErrorArgs(
                                "User has not logged in.", 502));
                }
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Get all folders for the account.
        /// </summary>
        /// <returns>The collection of all folders.</returns>
        public virtual List<string> GetFolders()
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            bool valid = true;
            List<string> ret = null;

            // If no connection exists
            // then thrown an exception.
            if ((_socket == null && _client == null) || (!_clientConnected))
            {
                if (OnValidationError != null)
                    OnValidationError(this, new ClientThreadErrorArgs(
                        "Not connected to a server.", 500));

                // Not connected.
                valid = false;
            }

            // If valid.
            if (valid)
            {
                // If user connected.
                if (_userConnected)
                {
                    ManualResetEvent waitObject = null;

                    // Create a new state object.
                    Imap4StateAdapter state = new Imap4StateAdapter();
                    state.SslClient = _sslStream;
                    state.Client = _client;
                    state.Socket = _socket;

                    // Get the event to wait on.
                    waitObject = state.ReceiveComplete;

                    // Send the create folder command.
                    SendCommand(_prefixNumber + " LIST \"\" \"*\"");

                    // If using a secure connection.
                    if (_connection.UseSSLConnection)
                    {
                        lock (_sslStream)
                            // This starts the asynchronous read thread. 
                            // The data will be saved into readBuffer.
                            _sslStream.BeginRead(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                new AsyncCallback(ReceiveCallback), state);
                    }
                    else
                    {
                        // Start receiving data asynchrounusly.
                        lock (_socket)
                            _socket.BeginReceive(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
                    }

                    // Block the current thread until all 
                    // operations are complete.
                    waitObject.WaitOne();

                    // The operations either completed or threw an exception.
                    if (state.OperationException != null)
                        throw state.OperationException;

                    // Get the message data.
                    string messageData = state.EmailMessage.ToString();

                    // If the server sent
                    // a comfirmation response.
                    if (messageData.ToUpper().IndexOf(_prefixNumber + " OK") > -1)
                    {
                        // Search for null characters '\r\n', '\n'.
                        // Split with current delimeter the data returned
                        // from the server. Create the new file list.
                        char[] delimeter = new char[] { '\r' };
                        string[] splitList = messageData.Split(delimeter,
                            StringSplitOptions.RemoveEmptyEntries);

                        // Create a new instance of the list
                        // generic collection.
                        ret = new List<string>();

                        // For each folder in the split list
                        // add the folder into the list collection.
                        foreach (string folder in splitList)
                            if (!String.IsNullOrEmpty(folder.Trim()))
                            {
                                // Filter line of data to
                                // the specified folder add
                                // the folder to the collection.
                                string folderStart = folder.Substring(folder.IndexOf("/") + 4).Trim().Replace("\"", "");
                                ret.Add(folderStart);
                            }

                        // Remove the last element.
                        ret.RemoveAt(ret.Count - 1);
                    }
                    else
                        if (OnCommandError != null)
                            OnCommandError(this, new ClientCommandArgs("FOLDERS",
                                "Server response : " + messageData, 404));
                }
                else
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "User has not logged in.", 502));
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Begin get all folders for the account.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginGetFolders(
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncGetAccountFolders(this, callback, state);
        }

        /// <summary>
        /// End get all folders for the account.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the message was returned else false.</returns>
        public virtual List<string> EndGetFolders(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return AsyncGetAccountFolders.End(ar);
        }

        /// <summary>
        /// Get all subscribed folders for the account.
        /// </summary>
        /// <returns>The collection of all folders.</returns>
        public virtual List<string> GetSubscribedFolders()
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            bool valid = true;
            List<string> ret = null;

            // If no connection exists
            // then thrown an exception.
            if ((_socket == null && _client == null) || (!_clientConnected))
            {
                if (OnValidationError != null)
                    OnValidationError(this, new ClientThreadErrorArgs(
                        "Not connected to a server.", 500));

                // Not connected.
                valid = false;
            }

            // If valid.
            if (valid)
            {
                // If user connected.
                if (_userConnected)
                {
                    ManualResetEvent waitObject = null;

                    // Create a new state object.
                    Imap4StateAdapter state = new Imap4StateAdapter();
                    state.SslClient = _sslStream;
                    state.Client = _client;
                    state.Socket = _socket;

                    // Get the event to wait on.
                    waitObject = state.ReceiveComplete;

                    // Send the create folder command.
                    SendCommand(_prefixNumber + " LSUB \"\" \"*\"");

                    // If using a secure connection.
                    if (_connection.UseSSLConnection)
                    {
                        lock (_sslStream)
                            // This starts the asynchronous read thread. 
                            // The data will be saved into readBuffer.
                            _sslStream.BeginRead(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                new AsyncCallback(ReceiveCallback), state);
                    }
                    else
                    {
                        // Start receiving data asynchrounusly.
                        lock (_socket)
                            _socket.BeginReceive(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
                    }

                    // Block the current thread until all 
                    // operations are complete.
                    waitObject.WaitOne();

                    // The operations either completed or threw an exception.
                    if (state.OperationException != null)
                        throw state.OperationException;

                    // Get the message data.
                    string messageData = state.EmailMessage.ToString();

                    // If the server sent
                    // a comfirmation response.
                    if (messageData.ToUpper().IndexOf(_prefixNumber + " OK") > -1)
                    {
                        // Search for null characters '\r\n', '\n'.
                        // Split with current delimeter the data returned
                        // from the server. Create the new file list.
                        char[] delimeter = new char[] { '\r' };
                        string[] splitList = messageData.Split(delimeter,
                            StringSplitOptions.RemoveEmptyEntries);

                        // Create a new instance of the list
                        // generic collection.
                        ret = new List<string>();

                        // For each folder in the split list
                        // add the folder into the list collection.
                        foreach (string folder in splitList)
                            if (!String.IsNullOrEmpty(folder.Trim()))
                            {
                                // Filter line of data to
                                // the specified folder add
                                // the folder to the collection.
                                string folderStart = folder.Substring(folder.IndexOf("/") + 4).Trim().Replace("\"", "");
                                ret.Add(folderStart);
                            }

                        // Remove the last element.
                        ret.RemoveAt(ret.Count - 1);
                    }
                    else
                        if (OnCommandError != null)
                            OnCommandError(this, new ClientCommandArgs("SUBSCRIBEDFOLDER",
                                "Server response : " + messageData, 404));
                }
                else
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "User has not logged in.", 502));
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Begin get all folders for the account.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginGetSubscribedFolders(
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncGetAccountFolders(true, this, callback, state);
        }

        /// <summary>
        /// End get all folders for the account.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the message was returned else false.</returns>
        public virtual  List<string> EndGetSubscribedFolders(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return AsyncGetAccountFolders.End(ar);
        }

        /// <summary>
        /// Subscribe to the specific folder.
        /// </summary>
        /// <param name="folderName">The folder to subscribe to.</param>
        /// <returns>True if the operation was successful else false.</returns>
        public virtual bool SubscribeToFolder(string folderName)
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            bool valid = true;
            bool ret = false;

            // If no connection exists
            // then thrown an exception.
            if ((_socket == null && _client == null) || (!_clientConnected))
            {
                if (OnValidationError != null)
                    OnValidationError(this, new ClientThreadErrorArgs(
                        "Not connected to a server.", 500));

                // Not connected.
                valid = false;
            }

            // If valid.
            if (valid)
            {
                // If the folderName is empty than
                // throw exception.
                if (String.IsNullOrEmpty(folderName))
                {
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "Folder name can not be empty or null.", 503));

                    // Not valid message position.
                    valid = false;
                }

                // If valid.
                if (valid)
                {
                    // If user connected.
                    if (_userConnected)
                    {
                        string returned;

                        // Send the create folder command.
                        SendCommand(_prefixNumber + " SUBSCRIBE " + folderName);

                        // Get the server response.
                        returned = GetServerResponse();

                        // If the server do not send
                        // a comfirmation response.
                        if (!returned.ToUpper().Substring(0, 2).Equals("OK"))
                        {
                            if (OnCommandError != null)
                                OnCommandError(this, new ClientCommandArgs("SUBSCRIBE",
                                    "Server response : " + returned, 404));
                        }
                        else
                            ret = true;
                    }
                    else
                        if (OnValidationError != null)
                            OnValidationError(this, new ClientThreadErrorArgs(
                                "User has not logged in.", 502));
                }
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Un subscribe from the specific folder.
        /// </summary>
        /// <param name="folderName">The folder to unsubscribe from.</param>
        /// <returns>True if the operation was successful else false.</returns>
        public virtual bool UnSubscribeFromFolder(string folderName)
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            bool valid = true;
            bool ret = false;

            // If no connection exists
            // then thrown an exception.
            if ((_socket == null && _client == null) || (!_clientConnected))
            {
                if (OnValidationError != null)
                    OnValidationError(this, new ClientThreadErrorArgs(
                        "Not connected to a server.", 500));

                // Not connected.
                valid = false;
            }

            // If valid.
            if (valid)
            {
                // If the folderName is empty than
                // throw exception.
                if (String.IsNullOrEmpty(folderName))
                {
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "Folder name can not be empty or null.", 503));

                    // Not valid message position.
                    valid = false;
                }

                // If valid.
                if (valid)
                {
                    // If user connected.
                    if (_userConnected)
                    {
                        string returned;

                        // Send the create folder command.
                        SendCommand(_prefixNumber + " UNSUBSCRIBE " + folderName);

                        // Get the server response.
                        returned = GetServerResponse();

                        // If the server do not send
                        // a comfirmation response.
                        if (!returned.ToUpper().Substring(0, 2).Equals("OK"))
                        {
                            if (OnCommandError != null)
                                OnCommandError(this, new ClientCommandArgs("UNSUBSCRIBE",
                                    "Server response : " + returned, 405));
                        }
                        else
                            ret = true;
                    }
                    else
                        if (OnValidationError != null)
                            OnValidationError(this, new ClientThreadErrorArgs(
                                "User has not logged in.", 502));
                }
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Sets the current folder specified.
        /// </summary>
        /// <param name="folderName">The folder to view details on.</param>
        /// <returns>The details of the folder.</returns>
        public virtual List<string> SetFolder(string folderName)
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            bool valid = true;
            List<string> ret = null;

            // If no connection exists
            // then thrown an exception.
            if ((_socket == null && _client == null) || (!_clientConnected))
            {
                if (OnValidationError != null)
                    OnValidationError(this, new ClientThreadErrorArgs(
                        "Not connected to a server.", 500));

                // Not connected.
                valid = false;
            }

            // If valid.
            if (valid)
            {
                // If the folderName is empty than
                // throw exception.
                if (String.IsNullOrEmpty(folderName))
                {
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "Folder name can not be empty or null.", 503));

                    // Not valid message position.
                    valid = false;
                }

                // If valid.
                if (valid)
                {
                    // If user connected.
                    if (_userConnected)
                    {
                        ManualResetEvent waitObject = null;

                        // Create a new state object.
                        Imap4StateAdapter state = new Imap4StateAdapter();
                        state.SslClient = _sslStream;
                        state.Client = _client;
                        state.Socket = _socket;

                        // Get the event to wait on.
                        waitObject = state.ReceiveComplete;

                        // Send the create folder command.
                        SendCommand(_prefixNumber + " SELECT " + folderName);

                        // If using a secure connection.
                        if (_connection.UseSSLConnection)
                        {
                            lock (_sslStream)
                                // This starts the asynchronous read thread. 
                                // The data will be saved into readBuffer.
                                _sslStream.BeginRead(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                    new AsyncCallback(ReceiveCallback), state);
                        }
                        else
                        {
                            // Start receiving data asynchrounusly.
                            lock (_socket)
                                _socket.BeginReceive(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                    SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
                        }

                        // Block the current thread until all 
                        // operations are complete.
                        waitObject.WaitOne();

                        // The operations either completed or threw an exception.
                        if (state.OperationException != null)
                            throw state.OperationException;

                        // Get the message data.
                        string messageData = state.EmailMessage.ToString();

                        // If the server sent
                        // a comfirmation response.
                        if (messageData.ToUpper().IndexOf(_prefixNumber + " OK") > -1)
                        {
                            // Search for null characters '\r\n', '\n'.
                            // Split with current delimeter the data returned
                            // from the server. Create the new file list.
                            char[] delimeter = new char[] { '\r' };
                            string[] splitList = messageData.Split(delimeter,
                                StringSplitOptions.RemoveEmptyEntries);

                            // Create a new instance of the list
                            // generic collection.
                            ret = new List<string>();

                            // For each folder in the split list
                            // add the folder into the list collection.
                            foreach (string folder in splitList)
                                if (!String.IsNullOrEmpty(folder.Trim()))
                                {
                                    // Filter line of data to
                                    // the specified folder add
                                    // the folder to the collection.
                                    string folderStart = folder.Substring(folder.IndexOf("*") + 2).
                                        Trim().Replace("\"", "").Replace("OK ", "");
                                    ret.Add(folderStart);
                                }

                            // Remove the last element.
                            ret.RemoveAt(ret.Count - 1);
                        }
                        else
                            if (OnCommandError != null)
                                OnCommandError(this, new ClientCommandArgs("SETFOLDER",
                                    "Server response : " + messageData, 405));
                    }
                    else
                        if (OnValidationError != null)
                            OnValidationError(this, new ClientThreadErrorArgs(
                                "User has not logged in.", 502));
                }
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Begin set the specified folder details.
        /// </summary>
        /// <param name="folderName">The folder to view details on.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginSetFolder(string folderName,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncGetAccountFolders(folderName, this, callback, state);
        }

        /// <summary>
        /// End set the specified folder details.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the message was returned else false.</returns>
        public virtual List<string> EndSetFolder(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return AsyncGetAccountFolders.End(ar);
        }

        /// <summary>
        /// Copy one messsage to the specified folder.
        /// </summary>
        /// <param name="messageNumber">The message number to copy.</param>
        /// <param name="destinationFolder">The destination folder to copy to.</param>
        /// <param name="useUID">Use the unique identifier for messsage number.</param>
        /// <returns>True if the message was returned else false.</returns>
        public virtual bool CopyMessage(long messageNumber, string destinationFolder, bool useUID)
        {
            return CopyMessages(messageNumber, messageNumber, destinationFolder, useUID);
        }

        /// <summary>
        /// Copy a group of messages to the specified folder.
        /// </summary>
        /// <param name="startMessageNumber">The start message to copy.</param>
        /// <param name="endMessageNumber">The end message to copy.</param>
        /// <param name="destinationFolder">The destination folder to copy to.</param>
        /// <param name="useUID">Use the unique identifier for messsage number.</param>
        /// <returns>True if the message was returned else false.</returns>
        /// <remarks>Set the 'endMessageNumber' variable to '0' for '*'.</remarks>
        public virtual bool CopyMessages(long startMessageNumber, long endMessageNumber, 
            string destinationFolder, bool useUID)
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            bool valid = true;
            bool ret = false;

            // If no connection exists
            // then thrown an exception.
            if ((_socket == null && _client == null) || (!_clientConnected))
            {
                if (OnValidationError != null)
                    OnValidationError(this, new ClientThreadErrorArgs(
                        "Not connected to a server.", 500));

                // Not connected.
                valid = false;
            }

            // If valid.
            if (valid)
            {
                // If the folderName is empty than
                // throw exception.
                if (String.IsNullOrEmpty(destinationFolder))
                {
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "Folder name can not be empty or null.", 503));

                    // Not valid message position.
                    valid = false;
                }
                else if (startMessageNumber < 1)
                {
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "Message start number must be greater than zero", 503));

                    // Not valid message position.
                    valid = false;
                }

                // If valid.
                if (valid)
                {
                    // If user connected.
                    if (_userConnected)
                    {
                        string returned;
                        string useMessageUID = string.Empty;
                        string endMessageNo = endMessageNumber.ToString();

                        // If the end message number is zero
                        // then the message number is the
                        // wild card '*';
                        if (endMessageNumber == 0)
                            endMessageNo = "*";

                        // Should the unique message identifier be
                        // used as the messsage numbers.
                        if(useUID)
                            useMessageUID = " UID";

                        // Send the create folder command.
                        SendCommand(_prefixNumber + useMessageUID + " COPY " + startMessageNumber.ToString() + ":" +
                            endMessageNo + " " + destinationFolder);

                        // Get the server response.
                        returned = GetServerResponse();

                        // If the server do not send
                        // a comfirmation response.
                        if (!returned.ToUpper().Substring(0, 2).Equals("OK"))
                        {
                            if (OnCommandError != null)
                                OnCommandError(this, new ClientCommandArgs("COPY",
                                    "Server response : " + returned, 406));
                        }
                        else
                            ret = true;
                    }
                    else
                        if (OnValidationError != null)
                            OnValidationError(this, new ClientThreadErrorArgs(
                                "User has not logged in.", 502));
                }
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Delete one messsage from the account.
        /// </summary>
        /// <param name="messageNumber">The message number to delete.</param>
        /// <param name="useUID">Use the unique identifier for messsage number.</param>
        /// <returns>True if the message was returned else false.</returns>
        public virtual bool DeleteMessage(long messageNumber, bool useUID)
        {
            return DeleteMessages(messageNumber, messageNumber, useUID);
        }

        /// <summary>
        /// Delete a group of messages from the account.
        /// </summary>
        /// <param name="startMessageNumber">The start message to delete.</param>
        /// <param name="endMessageNumber">The end message to delete.</param>
        /// <param name="useUID">Use the unique identifier for messsage number.</param>
        /// <returns>True if the message was returned else false.</returns>
        /// <remarks>Set the 'endMessageNumber' variable to '0' for '*'.</remarks>
        public virtual bool DeleteMessages(long startMessageNumber,
            long endMessageNumber, bool useUID)
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            bool valid = true;
            bool ret = false;

            // If no connection exists
            // then thrown an exception.
            if ((_socket == null && _client == null) || (!_clientConnected))
            {
                if (OnValidationError != null)
                    OnValidationError(this, new ClientThreadErrorArgs(
                        "Not connected to a server.", 500));

                // Not connected.
                valid = false;
            }

            // If valid.
            if (valid)
            {
                if (startMessageNumber < 1)
                {
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "Message start number must be greater than zero", 503));

                    // Not valid message position.
                    valid = false;
                }

                // If valid.
                if (valid)
                {
                    // If user connected.
                    if (_userConnected)
                    {
                        string returned;
                        string useMessageUID = string.Empty;
                        string endMessageNo = endMessageNumber.ToString();

                        // If the end message number is zero
                        // then the message number is the
                        // wild card '*';
                        if (endMessageNumber == 0)
                            endMessageNo = "*";

                        // Should the unique message identifier be
                        // used as the messsage numbers.
                        if (useUID)
                            useMessageUID = " UID";

                        // Send the create folder command.
                        // Set the delete flag on the messages.
                        SendCommand(_prefixNumber + useMessageUID + " STORE " + startMessageNumber.ToString() + ":" +
                            endMessageNo + " +FLAGS.SILENT (\\Deleted)");

                        // Get the server response.
                        returned = GetServerResponse();

                        // If the server do not send
                        // a comfirmation response.
                        if (!returned.ToUpper().Substring(0, 2).Equals("OK"))
                        {
                            if (OnCommandError != null)
                                OnCommandError(this, new ClientCommandArgs("DELETE",
                                    "Server response : " + returned, 407));
                        }
                        else
                        {
                            ManualResetEvent waitObject = null;

                            // Create a new state object.
                            Imap4StateAdapter state = new Imap4StateAdapter();
                            state.SslClient = _sslStream;
                            state.Client = _client;
                            state.Socket = _socket;

                            // Get the event to wait on.
                            waitObject = state.ReceiveComplete;

                            // Send the create folder command.
                            // Delete all the messages flaged as
                            // 'delete' with the EXPUNGE command.
                            SendCommand(_prefixNumber + " EXPUNGE");

                            // If using a secure connection.
                            if (_connection.UseSSLConnection)
                            {
                                lock (_sslStream)
                                    // This starts the asynchronous read thread. 
                                    // The data will be saved into readBuffer.
                                    _sslStream.BeginRead(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                        new AsyncCallback(ReceiveCallback), state);
                            }
                            else
                            {
                                // Start receiving data asynchrounusly.
                                lock (_socket)
                                    _socket.BeginReceive(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                        SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
                            }

                            // Block the current thread until all 
                            // operations are complete.
                            waitObject.WaitOne();

                            // The operations either completed or threw an exception.
                            if (state.OperationException != null)
                                throw state.OperationException;

                            // Get the message data.
                            string messageData = state.EmailMessage.ToString();

                            // If the server sent
                            // a comfirmation response.
                            if (messageData.ToUpper().IndexOf(_prefixNumber + " OK") > -1)
                                ret = true;
                            else
                                if (OnCommandError != null)
                                    OnCommandError(this, new ClientCommandArgs("EXPUNGE",
                                        "Server response : " + returned, 408));
                        }
                    }
                    else
                        if (OnValidationError != null)
                            OnValidationError(this, new ClientThreadErrorArgs(
                                "User has not logged in.", 502));
                }
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Move one messsage to the specified folder.
        /// </summary>
        /// <param name="messageNumber">The message number to move.</param>
        /// <param name="destinationFolder">The destination folder to move to.</param>
        /// <param name="useUID">Use the unique identifier for messsage number.</param>
        /// <returns>True if the message was returned else false.</returns>
        public virtual bool MoveMessage(long messageNumber, string destinationFolder, bool useUID)
        {
            return MoveMessages(messageNumber, messageNumber, destinationFolder, useUID);
        }

        /// <summary>
        /// Move a group of messages to the specified folder.
        /// </summary>
        /// <param name="startMessageNumber">The start message to move.</param>
        /// <param name="endMessageNumber">The end message to move.</param>
        /// <param name="destinationFolder">The destination folder to move to.</param>
        /// <param name="useUID">Use the unique identifier for messsage number.</param>
        /// <returns>True if the message was returned else false.</returns>
        /// <remarks>Set the 'endMessageNumber' variable to '0' for '*'.</remarks>
        public virtual bool MoveMessages(long startMessageNumber, long endMessageNumber, 
            string destinationFolder, bool useUID)
        {
            bool ret = false;

            // Copy the messages from one location
            // to another, then delete the messages
            // from the original location.
            ret = CopyMessages(startMessageNumber, endMessageNumber, destinationFolder, useUID);
            if (ret)
                ret = DeleteMessages(startMessageNumber, endMessageNumber, useUID);

            // return true if succeeded else
            // return false.
            return ret;
        }

        /// <summary>
        /// Gets the size of the specified message.
        /// </summary>
        /// <param name="messageNumber">The message to get information on.</param>
        /// <returns>The size of the message number.</returns>
        public virtual List<string> GetMessageSize(long messageNumber)
        {
            return GetSizeOfMessages(messageNumber, messageNumber);
        }

        /// <summary>
        /// Gets the size of all messages in the account.
        /// </summary>
        /// <returns>The collection of message sizes.</returns>
        public virtual List<string> GetSizeOfMessages()
        {
            return GetSizeOfMessages(1, 0);
        }

        /// <summary>
        /// Gets the size of the messages between the interval.
        /// </summary>
        /// <param name="startMessageNumber">The start message number.</param>
        /// <param name="endMessageNumber">The end message number.</param>
        /// <returns>The collection of message sizes.</returns>
        /// <remarks>Set the 'endMessageNumber' variable to '0' for '*'.</remarks>
        public virtual List<string> GetSizeOfMessages(long startMessageNumber, long endMessageNumber)
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            bool valid = true;
            List<string> ret = null;

            // If no connection exists
            // then thrown an exception.
            if ((_socket == null && _client == null) || (!_clientConnected))
            {
                if (OnValidationError != null)
                    OnValidationError(this, new ClientThreadErrorArgs(
                        "Not connected to a server.", 500));

                // Not connected.
                valid = false;
            }

            // If valid.
            if (valid)
            {
                if (startMessageNumber < 1)
                {
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "Message start number must be greater than zero", 503));

                    // Not valid message position.
                    valid = false;
                }

                // If valid.
                if (valid)
                {
                    // If user connected.
                    if (_userConnected)
                    {
                        string endMessasgeIndex = endMessageNumber.ToString();
                        ManualResetEvent waitObject = null;

                        // Create a new state object.
                        Imap4StateAdapter state = new Imap4StateAdapter();
                        state.SslClient = _sslStream;
                        state.Client = _client;
                        state.Socket = _socket;

                        // Get the event to wait on.
                        waitObject = state.ReceiveComplete;

                        // If the end message index is zero
                        // then get all messages.
                        if (endMessageNumber == 0)
                            endMessasgeIndex = "*";

                        // Send the create fetch command.
                        SendCommand(_prefixNumber + " FETCH " + startMessageNumber.ToString()
                            + ":" + endMessasgeIndex + " (RFC822.SIZE)");

                        // If using a secure connection.
                        if (_connection.UseSSLConnection)
                        {
                            lock (_sslStream)
                                // This starts the asynchronous read thread. 
                                // The data will be saved into readBuffer.
                                _sslStream.BeginRead(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                    new AsyncCallback(ReceiveCallback), state);
                        }
                        else
                        {
                            // Start receiving data asynchrounusly.
                            lock (_socket)
                                _socket.BeginReceive(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                    SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
                        }

                        // Block the current thread until all 
                        // operations are complete.
                        waitObject.WaitOne();

                        // The operations either completed or threw an exception.
                        if (state.OperationException != null)
                            throw state.OperationException;

                        // Get the message data.
                        string messageData = state.EmailMessage.ToString();

                        // If the server sent
                        // a comfirmation response.
                        if (messageData.ToUpper().IndexOf(_prefixNumber + " OK") > -1)
                        {
                            // Search for null characters '\r\n', '\n'.
                            // Split with current delimeter the data returned
                            // from the server. Create the new file list.
                            char[] delimeter = new char[] { '\r' };
                            string[] splitList = messageData.Split(delimeter,
                                StringSplitOptions.RemoveEmptyEntries);

                            // Create a new instance of the list
                            // generic collection.
                            ret = new List<string>();

                            // For each folder in the split list
                            // add the folder into the list collection.
                            foreach (string message in splitList)
                                if (!String.IsNullOrEmpty(message.Trim()))
                                {
                                    // Make sure that it is
                                    // a message fetch item not
                                    // a command response.
                                    if (message.IndexOf("*") > -1)
                                    {
                                        // Extract the length of the message
                                        // from the first element.
                                        int start = message.IndexOf("(RFC822.SIZE") + 12;
                                        int end = message.LastIndexOf(")");
                                        string length = message.Substring(start, end - start).Trim();
                                        long size = Convert.ToInt64(length);

                                        // Extract the message number
                                        // for the current line.
                                        int startMesNo = message.IndexOf("*") + 1;
                                        int endMesNo = message.LastIndexOf("FETCH");
                                        string messageNo = message.Substring(startMesNo, endMesNo - startMesNo).Trim();

                                        // Add the size of the current message.
                                        ret.Add("Message No : " + messageNo + "; Size : " + size.ToString());
                                    }
                                }
                        }
                        else
                            if (OnCommandError != null)
                                OnCommandError(this, new ClientCommandArgs("FETCH",
                                    "Server response : " + messageData, 408));
                    }
                    else
                        if (OnValidationError != null)
                            OnValidationError(this, new ClientThreadErrorArgs(
                                "User has not logged in.", 502));
                }
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Begin gets the size of the messages between the interval.
        /// </summary>
        /// <param name="startMessageNumber">The start message number.</param>
        /// <param name="endMessageNumber">The end message number.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginGetSizeOfMessages(long startMessageNumber, 
            long endMessageNumber, AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncGetMessageSize(startMessageNumber,
                endMessageNumber, this, callback, state);
        }

        /// <summary>
        /// Begin gets the size of all messages in the account.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginGetSizeOfMessages(AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncGetMessageSize(this, callback, state);
        }

        /// <summary>
        /// End gets the size of all messages in the account.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the message was returned else false.</returns>
        public virtual List<string> EndGetSizeOfMessages(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return AsyncGetMessageSize.End(ar);
        }

        /// <summary>
        /// Begin gets the size of the specified message.
        /// </summary>
        /// <param name="messageNumber">The message to get information on.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginGetMessageSize(long messageNumber,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncGetMessageSize(messageNumber, this, callback, state);
        }

        /// <summary>
        /// End gets the size of the specified message.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the message was returned else false.</returns>
        public virtual List<string> EndGetMessageSize(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return AsyncGetMessageSize.End(ar);
        }

        /// <summary>
        /// Gets the specified message.
        /// </summary>
        /// <param name="messageNumber">The message number to return.</param>
        /// <returns>True if the message was returned else false.</returns>
        public virtual bool GetEmail(long messageNumber)
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            bool valid = true;
            bool ret = false;

            // If no connection exists
            // then thrown an exception.
            if ((_socket == null && _client == null) || (!_clientConnected))
            {
                if (OnValidationError != null)
                    OnValidationError(this, new ClientThreadErrorArgs(
                        "Not connected to a server.", 500));

                // Not connected.
                valid = false;
            }

            // If valid.
            if (valid)
            {
                // If the messageNumber is not valid
                // throw exception.
                if (messageNumber < 1)
                {
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "Message number must be greater than zero.", 504));

                    // Not valid message position.
                    valid = false;
                }

                // If valid.
                if (valid)
                {
                    // If user connected.
                    if (_userConnected)
                    {
                        // Create a new attachment list and
                        // an new message class that will contain
                        // the current message information.
                        // Assign the attachment object to the
                        // attachment property in the message object.
                        List<Attachment> attachments = new List<Attachment>();
                        _message = new Message();
                        _message.Attachments = attachments;

                        // Get the size of the current email message.
                        long size = GetTotalMessageSize(messageNumber);

                        try
                        {
                            // Create a new email message class,
                            // starts the retreival of the specified
                            // email message.
                            EmailMessage emailMessage =
                                new EmailMessage(messageNumber, size, false, 0, _socket,
                                _sslStream, _message, _writeAttachmentToFile, _rawEmailMessageOnly,
                                _connection, _callbackHandler);

                            // Email retreival succeeded.
                            ret = true;
                        }
                        catch (Exception e)
                        {
                            if (OnAsyncThreadError != null)
                                OnAsyncThreadError(this, new ClientThreadErrorArgs(
                                    e.Message, 804));
                        }
                    }
                    else
                        if (OnValidationError != null)
                            OnValidationError(this, new ClientThreadErrorArgs(
                                "User has not logged in.", 502));
                }
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Begin get the specified email message.
        /// </summary>
        /// <param name="messageNumber">The email message number.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginGetEmail(long messageNumber,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncGetEmailMessage(messageNumber, this, callback, state);
        }

        /// <summary>
        /// End get the specified email message.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the message was returned else false.</returns>
        public virtual bool EndGetEmail(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return AsyncGetEmailMessage.End(ar);
        }

        /// <summary>
        /// Gets the specified message headers.
        /// </summary>
        /// <param name="messageNumber">The message number to return.</param>
        /// <returns>True if the message was returned else false.</returns>
        public virtual bool GetEmailHeaders(long messageNumber)
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            bool valid = true;
            bool ret = false;

            // If no connection exists
            // then thrown an exception.
            if ((_socket == null && _client == null) || (!_clientConnected))
            {
                if (OnValidationError != null)
                    OnValidationError(this, new ClientThreadErrorArgs(
                        "Not connected to a server.", 500));

                // Not connected.
                valid = false;
            }

            // If valid.
            if (valid)
            {
                // If the messageNumber is not valid
                // throw exception.
                if (messageNumber < 1)
                {
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "Message number must be greater than zero.", 504));

                    // Not valid message position.
                    valid = false;
                }

                // If valid.
                if (valid)
                {
                    // If user connected.
                    if (_userConnected)
                    {
                        // Create a new attachment list and
                        // an new message class that will contain
                        // the current message information.
                        // Assign the attachment object to the
                        // attachment property in the message object.
                        List<Attachment> attachments = new List<Attachment>();
                        _message = new Message();
                        _message.Attachments = attachments;

                        // Get the size of the current email message.
                        long size = GetTotalMessageSize(messageNumber);

                        try
                        {
                            // Create a new email message class,
                            // starts the retreival of the specified
                            // email message.
                            EmailMessage emailMessage =
                                new EmailMessage(messageNumber, size, true, 0, _socket,
                                _sslStream, _message, _writeAttachmentToFile, _rawEmailMessageOnly,
                                _connection, _callbackHandler);

                            // Email retreival succeeded.
                            ret = true;
                        }
                        catch (Exception e)
                        {
                            if (OnAsyncThreadError != null)
                                OnAsyncThreadError(this, new ClientThreadErrorArgs(
                                    e.Message, 804));
                        }
                    }
                    else
                        if (OnValidationError != null)
                            OnValidationError(this, new ClientThreadErrorArgs(
                                "User has not logged in.", 502));
                }
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Begin get the specified email message headers.
        /// </summary>
        /// <param name="messageNumber">The email message number.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginGetEmailHeaders(long messageNumber,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncGetEmailMessage(messageNumber, 0, this, callback, state);
        }

        /// <summary>
        /// End get the specified email message headers.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the message was returned else false.</returns>
        public virtual bool EndGetEmailHeaders(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return AsyncGetEmailMessage.End(ar);
        }

        /// <summary>
        /// Gets the total number of messages in the account.
        /// </summary>
        /// <returns>The number of messages.</returns>
        public virtual long GetMessageCount()
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            bool valid = true;
            long ret = 0;

            // If no connection exists
            // then thrown an exception.
            if ((_socket == null && _client == null) || (!_clientConnected))
            {
                if (OnValidationError != null)
                    OnValidationError(this, new ClientThreadErrorArgs(
                        "Not connected to a server.", 500));

                // Not connected.
                valid = false;
            }

            // If valid.
            if (valid)
            {
                // If user connected.
                if (_userConnected)
                {
                    ManualResetEvent waitObject = null;

                    // Create a new state object.
                    Imap4StateAdapter state = new Imap4StateAdapter();
                    state.SslClient = _sslStream;
                    state.Client = _client;
                    state.Socket = _socket;

                    // Get the event to wait on.
                    waitObject = state.ReceiveComplete;

                    // Send the create fetch command.
                    SendCommand(_prefixNumber + " FETCH 1:* (RFC822.SIZE)");

                    // If using a secure connection.
                    if (_connection.UseSSLConnection)
                    {
                        lock (_sslStream)
                            // This starts the asynchronous read thread. 
                            // The data will be saved into readBuffer.
                            _sslStream.BeginRead(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                new AsyncCallback(ReceiveCallback), state);
                    }
                    else
                    {
                        // Start receiving data asynchrounusly.
                        lock (_socket)
                            _socket.BeginReceive(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
                    }

                    // Block the current thread until all 
                    // operations are complete.
                    waitObject.WaitOne();

                    // The operations either completed or threw an exception.
                    if (state.OperationException != null)
                        throw state.OperationException;

                    // Get the message data.
                    string messageData = state.EmailMessage.ToString();

                    // If the server sent
                    // a comfirmation response.
                    if (messageData.ToUpper().IndexOf(_prefixNumber + " OK") > -1)
                    {
                        // Search for null characters '\r\n', '\n'.
                        // Split with current delimeter the data returned
                        // from the server. Create the new file list.
                        char[] delimeter = new char[] { '\r' };
                        string[] splitList = messageData.Split(delimeter,
                            StringSplitOptions.RemoveEmptyEntries);

                        // Initial count messages.
                        long count = 0;

                        // For each folder in the split list
                        // add the folder into the list collection.
                        foreach (string message in splitList)
                            if (!String.IsNullOrEmpty(message.Trim()))
                                // Make sure that it is
                                // a message fetch item not
                                // a command response.
                                if (message.IndexOf("*") > -1)
                                    count++;

                        // Return the number of messages.
                        ret = count;
                    }
                    else
                        if (OnCommandError != null)
                            OnCommandError(this, new ClientCommandArgs("FETCH",
                                "Server response : " + messageData, 408));
                }
                else
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "User has not logged in.", 502));
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Gets the collection of all un-seen messages for the account.
        /// </summary>
        /// <returns>Collection of un-seen messages.</returns>
        public virtual List<string> GetUnSeenMessages()
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            bool valid = true;
            List<string> ret = null;

            // If no connection exists
            // then thrown an exception.
            if ((_socket == null && _client == null) || (!_clientConnected))
            {
                if (OnValidationError != null)
                    OnValidationError(this, new ClientThreadErrorArgs(
                        "Not connected to a server.", 500));

                // Not connected.
                valid = false;
            }

            // If valid.
            if (valid)
            {
                // If user connected.
                if (_userConnected)
                {
                    ManualResetEvent waitObject = null;

                    // Create a new state object.
                    Imap4StateAdapter state = new Imap4StateAdapter();
                    state.SslClient = _sslStream;
                    state.Client = _client;
                    state.Socket = _socket;

                    // Get the event to wait on.
                    waitObject = state.ReceiveComplete;

                    // Send the create fetch command.
                    SendCommand(_prefixNumber + " FETCH 1:* FLAGS");

                    // If using a secure connection.
                    if (_connection.UseSSLConnection)
                    {
                        lock (_sslStream)
                            // This starts the asynchronous read thread. 
                            // The data will be saved into readBuffer.
                            _sslStream.BeginRead(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                new AsyncCallback(ReceiveCallback), state);
                    }
                    else
                    {
                        // Start receiving data asynchrounusly.
                        lock (_socket)
                            _socket.BeginReceive(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
                    }

                    // Block the current thread until all 
                    // operations are complete.
                    waitObject.WaitOne();

                    // The operations either completed or threw an exception.
                    if (state.OperationException != null)
                        throw state.OperationException;

                    // Get the message data.
                    string messageData = state.EmailMessage.ToString();

                    // If the server sent
                    // a comfirmation response.
                    if (messageData.ToUpper().IndexOf(_prefixNumber + " OK") > -1)
                    {
                        // Search for null characters '\r\n', '\n'.
                        // Split with current delimeter the data returned
                        // from the server. Create the new file list.
                        char[] delimeter = new char[] { '\r' };
                        string[] splitList = messageData.Split(delimeter,
                            StringSplitOptions.RemoveEmptyEntries);

                        // Create a new instance of the list
                        // generic collection.
                        ret = new List<string>();

                        // For each folder in the split list
                        // add the folder into the list collection.
                        foreach (string message in splitList)
                            if (!String.IsNullOrEmpty(message.Trim()))
                            {
                                // Make sure that it is
                                // a message fetch item not
                                // a command response.
                                if (message.IndexOf("*") > -1)
                                {
                                    if (message.ToUpper().IndexOf("\\SEEN") == -1)
                                    {
                                        // Extract the message number
                                        // for the current line.
                                        int startMesNo = message.IndexOf("*") + 1;
                                        int endMesNo = message.LastIndexOf("FETCH");
                                        string messageNo = message.Substring(startMesNo, endMesNo - startMesNo).Trim();

                                        // Add the size of the current message.
                                        ret.Add("Message No : " + messageNo);
                                    }
                                }
                            }
                    }
                    else
                        if (OnCommandError != null)
                            OnCommandError(this, new ClientCommandArgs("FETCH",
                                "Server response : " + messageData, 408));
                }
                else
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "User has not logged in.", 502));
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Get the collection of attachment file names found in the message.
        /// </summary>
        /// <param name="messageNumber">The message number.</param>
        /// <returns>The collection of attachment file names found.</returns>
        public virtual List<string> GetAttachmentFileNames(long messageNumber)
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            bool valid = true;
            List<string> ret = null;

            // If no connection exists
            // then thrown an exception.
            if ((_socket == null && _client == null) || (!_clientConnected))
            {
                if (OnValidationError != null)
                    OnValidationError(this, new ClientThreadErrorArgs(
                        "Not connected to a server.", 500));

                // Not connected.
                valid = false;
            }

            // If valid.
            if (valid)
            {
                if (messageNumber < 1)
                {
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "Message number must be greater than zero", 503));

                    // Not valid message position.
                    valid = false;
                }

                // If valid.
                if (valid)
                {
                    // If user connected.
                    if (_userConnected)
                    {
                        ManualResetEvent waitObject = null;

                        // Create a new state object.
                        Imap4StateAdapter state = new Imap4StateAdapter();
                        state.SslClient = _sslStream;
                        state.Client = _client;
                        state.Socket = _socket;

                        // Get the event to wait on.
                        waitObject = state.ReceiveComplete;

                        // Send the create fetch command.
                        SendCommand(_prefixNumber + " FETCH " + messageNumber +
                            ":" + messageNumber + " FULL");

                        // If using a secure connection.
                        if (_connection.UseSSLConnection)
                        {
                            lock (_sslStream)
                                // This starts the asynchronous read thread. 
                                // The data will be saved into readBuffer.
                                _sslStream.BeginRead(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                    new AsyncCallback(ReceiveCallback), state);
                        }
                        else
                        {
                            // Start receiving data asynchrounusly.
                            lock (_socket)
                                _socket.BeginReceive(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                    SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
                        }

                        // Block the current thread until all 
                        // operations are complete.
                        waitObject.WaitOne();

                        // The operations either completed or threw an exception.
                        if (state.OperationException != null)
                            throw state.OperationException;

                        // Get the message data.
                        string messageData = state.EmailMessage.ToString();

                        // If the server sent
                        // a comfirmation response.
                        if (messageData.ToUpper().IndexOf(_prefixNumber + " OK") > -1)
                        {
                            // Search for null characters '\r\n', '\n'.
                            // Split with current delimeter the data returned
                            // from the server. Create the new file list.
                            char[] delimeter = new char[] { '\r' };
                            string[] splitList = messageData.Split(delimeter,
                                StringSplitOptions.RemoveEmptyEntries);

                            // Create a new instance of the list
                            // generic collection.
                            ret = new List<string>();
                            int startIndex = 0;
                            int endIndex = 0;

                            // For each folder in the split list
                            // add the folder into the list collection.
                            foreach (string message in splitList)
                                if (!String.IsNullOrEmpty(message.Trim()))
                                {
                                    // Make sure that it is
                                    // a message fetch item not
                                    // a command response.
                                    if (message.IndexOf("*") > -1)
                                    {
                                        // While attachment file name
                                        // has been found.
                                        while (startIndex > -1)
                                        {
                                            // Get the starting index and the ending index
                                            // of the attachment header file name found.
                                            startIndex = message.ToUpper().IndexOf("NAME", startIndex, StringComparison.CurrentCultureIgnoreCase);
                                            endIndex = message.ToUpper().IndexOf(")", (startIndex + 4), StringComparison.CurrentCultureIgnoreCase);

                                            // If no more attachment header data
                                            // is found then break out of the loop.
                                            if (startIndex < 0)
                                                break;

                                            // Add the attachment file name
                                            // to the collection.
                                            ret.Add(message.Substring(startIndex + 4, endIndex - (startIndex + 4)).Replace("\"", "").Trim());

                                            // Set the new starting point
                                            // of the index search.
                                            startIndex = endIndex;
                                        }
                                    }
                                }
                        }
                        else
                            if (OnCommandError != null)
                                OnCommandError(this, new ClientCommandArgs("FETCH",
                                    "Server response : " + messageData, 408));
                    }
                    else
                        if (OnValidationError != null)
                            OnValidationError(this, new ClientThreadErrorArgs(
                                "User has not logged in.", 502));
                }
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Get the email message body.
        /// </summary>
        /// <param name="messageNumber">The email message number.</param>
        /// <returns>The message body of the email.</returns>
        public virtual string GetEmailBody(long messageNumber)
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            bool valid = true;
            string ret = null;

            // If no connection exists
            // then thrown an exception.
            if ((_socket == null && _client == null) || (!_clientConnected))
            {
                if (OnValidationError != null)
                    OnValidationError(this, new ClientThreadErrorArgs(
                        "Not connected to a server.", 500));

                // Not connected.
                valid = false;
            }

            // If valid.
            if (valid)
            {
                if (messageNumber < 1)
                {
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "Message number must be greater than zero", 503));

                    // Not valid message position.
                    valid = false;
                }

                // If valid.
                if (valid)
                {
                    // If user connected.
                    if (_userConnected)
                    {
                        ManualResetEvent waitObject = null;

                        // Create a new state object.
                        Imap4StateAdapter state = new Imap4StateAdapter();
                        state.SslClient = _sslStream;
                        state.Client = _client;
                        state.Socket = _socket;

                        // Get the event to wait on.
                        waitObject = state.ReceiveComplete;

                        // Send the create fetch command.
                        SendCommand(_prefixNumber + " FETCH " + messageNumber +
                            ":" + messageNumber + " BODY[1]");

                        // If using a secure connection.
                        if (_connection.UseSSLConnection)
                        {
                            lock (_sslStream)
                                // This starts the asynchronous read thread. 
                                // The data will be saved into readBuffer.
                                _sslStream.BeginRead(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                    new AsyncCallback(ReceiveCallback), state);
                        }
                        else
                        {
                            // Start receiving data asynchrounusly.
                            lock (_socket)
                                _socket.BeginReceive(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                    SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
                        }

                        // Block the current thread until all 
                        // operations are complete.
                        waitObject.WaitOne();

                        // The operations either completed or threw an exception.
                        if (state.OperationException != null)
                            throw state.OperationException;

                        // Get the message data.
                        string messageData = state.EmailMessage.ToString();

                        // If the server sent
                        // a comfirmation response.
                        if (messageData.ToUpper().IndexOf(_prefixNumber + " OK") > -1)
                        {
                            // If the message body is a html message.
                            if (messageData.IndexOf("<html") > -1)
                            {
                                // Get the starting index of the string
                                // and the ending index of the string.
                                int start = messageData.IndexOf("<html");
                                int end = messageData.LastIndexOf("</html>") + 7;

                                // Extract only the html part of the message body.
                                ret = messageData.Substring(start, end - start);
                            }
                            else if (messageData.IndexOf("<HTML") > -1)
                            {
                                // Get the starting index of the string
                                // and the ending index of the string.
                                int start = messageData.IndexOf("<HTML");
                                int end = messageData.LastIndexOf("</HTML>") + 7;

                                // Extract only the html part of the message body.
                                ret = messageData.Substring(start, end - start);
                            }
                            else if (messageData.ToLower().IndexOf("<xml") > -1)
                            {
                                // Get the starting index of the string
                                // and the ending index of the string.
                                int start = messageData.ToLower().IndexOf("<xml");
                                int end = messageData.ToLower().LastIndexOf("</xml>") + 6;

                                // Extract only the html part of the message body.
                                ret = messageData.Substring(start, end - start);
                            }
                            else
                            {
                                // A normal text message has been sent.
                                int start = messageData.IndexOf("}") + 3;
                                int end = messageData.LastIndexOf(")");
                                ret = messageData.Substring(start, end - start);
                            }
                        }
                        else
                            if (OnCommandError != null)
                                OnCommandError(this, new ClientCommandArgs("FETCH",
                                    "Server response : " + messageData, 408));
                    }
                    else
                        if (OnValidationError != null)
                            OnValidationError(this, new ClientThreadErrorArgs(
                                "User has not logged in.", 502));
                }
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Begin get the specified email message body.
        /// </summary>
        /// <param name="messageNumber">The email message number.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginGetEmailBody(long messageNumber,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncGetEmailBody(messageNumber, this, callback, state);
        }

        /// <summary>
        /// End get the specified email message body.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the message was returned else false.</returns>
        public virtual string EndGetEmailBody(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return AsyncGetEmailBody.End(ar);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Get the client connection socket.
        /// </summary>
        /// <returns>The client connection socket.</returns>
        private Socket GetSocket()
        {
            Socket socket = null;

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
                        socket = tempSocket;
                        break;
                    }
                    else continue;
                }

                // Return the client socket.
                return socket;
            }
            catch (Exception e)
            {
                if (OnAsyncThreadError != null)
                    OnAsyncThreadError(this, new ClientThreadErrorArgs(
                        e.Message, 803));

                // Return null.
                return null;
            }
        }

        /// <summary>
        /// Get the client tcp connection socket.
        /// </summary>
        /// <returns>The client connection socket.</returns>
        private TcpClient GetTcpClient()
        {
            TcpClient client = null;

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
                    TcpClient tempSocket = new TcpClient(endPoint.AddressFamily);

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
                        client = tempSocket;

                        // Remote certificate validation call back.
                        RemoteCertificateValidationCallback callback =
                            new RemoteCertificateValidationCallback(OnCertificateValidation);

                        // Get the current ssl stream
                        // from the socket.
                        _sslStream = new SslStream(client.GetStream(), true, callback);
                        _sslStream.AuthenticateAsClient(_connection.Server);
                        break;
                    }
                    else continue;
                }

                // Return the client socket.
                return client;
            }
            catch (Exception e)
            {
                if (OnAsyncThreadError != null)
                    OnAsyncThreadError(this, new ClientThreadErrorArgs(
                        e.Message, 804));

                // Return null.
                return null;
            }
        }

        /// <summary>
        /// Get the total size of the specified message.
        /// </summary>
        /// <param name="messageNumber">The current message number.</param>
        /// <returns>The size of the message.</returns>
        private long GetTotalMessageSize(long messageNumber)
        {
            try
            {
                NumberGenerator randomNumber = new NumberGenerator();
                UpperCaseGenerator randomUpper = new UpperCaseGenerator();
                _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

                // Send the create fetch command.
                SendCommand(_prefixNumber + " FETCH " + messageNumber.ToString()
                    + ":" + messageNumber.ToString() + " (RFC822.SIZE)");

                // Get the server response.
                string returned = GetServerResponse();

                // Search for null characters '\r\n', '\n'.
                // Split with current delimeter the data returned
                // from the server. Create the new file list.
                char[] delimeter = new char[] { '\r' };
                string[] splitList = returned.Split(delimeter,
                    StringSplitOptions.RemoveEmptyEntries);

                // Extract the length of the message
                // from the first element.
                int start = splitList[0].IndexOf("(RFC822.SIZE") + 12;
                int end = splitList[0].LastIndexOf(")");
                string length = splitList[0].Substring(start, end - start).Trim();
                long size = Convert.ToInt64(length);

                // Return the totoal size of the message.
                return size;
            }
            catch { return 0; }
        }

        /// <summary>
        /// Login the current user.
        /// </summary>
        private void Login()
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            string returned;
            bool valid = true;

            // Send the username as password.
            SendCommand(_prefixNumber + " LOGIN " + _connection.UserName +
                " " + _connection.Password);

            // Get the server response.
            returned = GetServerResponse();

            // If the server do not send
            // a comfirmation response.
            if (!returned.ToUpper().Substring(0, 2).Equals("OK"))
            {
                if (OnCommandError != null)
                    OnCommandError(this, new ClientCommandArgs("LOGIN",
                        "The user name and or password are not valid.", 401));

                // Invalid login;
                valid = false;
            }

            // If valid password then
            // continue.
            if (valid)
            {
                // Set the user as connected.
                _userConnected = true;

                // indicate to the user that the
                // client has been validated.
                if (OnCommandConnected != null)
                    OnCommandConnected(this, new ClientCommandArgs("CONN",
                        "The user has been logged in.", 200));
            }
        }

        /// <summary>
        /// Send a command to the server.
        /// </summary>
        /// <param name="data">The command data to send to the server.</param>
        private void SendCommand(String data)
        {
            try
            {
                // Convert the string data to byte data 
                // using ASCII encoding.
                byte[] byteData = Encoding.ASCII.GetBytes(data + "\r\n");

                // If a secure connection is required.
                if (_connection.UseSSLConnection && _startTLS)
                    // Send the command to the server.
                    _sslStream.Write(byteData);
                else
                    // Send the command to the server.
                    _socket.Send(byteData);
            }
            catch (Exception e)
            {
                if (OnAsyncThreadError != null)
                    OnAsyncThreadError(this, new ClientThreadErrorArgs(
                        e.Message, 802));
            }
        }

        /// <summary>
        /// Get the current server response.
        /// </summary>
        /// <returns>The server response data.</returns>
        private string GetServerResponse()
        {
            try
            {
                // Create a new buffer byte array, stores the response.
                byte[] readBuffer = new byte[EmailMessageParse.MAX_BUFFER_READ_SIZE];
                int byteCount = 0;

                // If a secure connection is required.
                if (_connection.UseSSLConnection && _startTLS)
                    // Get the data from the server placed it in the buffer.
                    byteCount = _sslStream.Read(readBuffer, 0, readBuffer.Length);
                else
                    // Get the data from the server placed it in the buffer.
                    byteCount = _socket.Receive(readBuffer, readBuffer.Length, 0);

                // Decode the data in the buffer to a string.
                string line = Encoding.ASCII.GetString(readBuffer, 0, byteCount);

                // Remove the command prefix number.
                string reply = line.Substring(line.IndexOf(" ")).Trim();

                // Return the response from the server.
                return reply;
            }
            catch (Exception e)
            {
                if (OnAsyncThreadError != null)
                    OnAsyncThreadError(this, new ClientThreadErrorArgs(
                        e.Message, 801));

                return null;
            }
        }

        /// <summary>
        /// The end of the asynchronous request stream callback.
        /// </summary>
        /// <param name="ar">The status of the asynchronous operation.</param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            // Get the state adapter from the
            // async result object.
            Imap4StateAdapter state = (Imap4StateAdapter)ar.AsyncState;

            try
            {
                // Get the socket from the
                // state adapter.
                Socket socket = state.Socket;
                TcpClient client = state.Client;
                SslStream sslStream = state.SslClient;

                // Read data from the remote device.
                int bytesRead = 0;

                // If a secure connection is required.
                if (_connection.UseSSLConnection)
                    lock (sslStream)
                        // End the current asynchrounus
                        // read operation.
                        bytesRead = sslStream.EndRead(ar);
                else
                    lock (socket)
                        // End the current asynchrounus
                        // read operation.
                        bytesRead = socket.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // Decode the current buffer and add the new data
                    // to the message string builder.
                    state.EmailMessage.Append(Encoding.ASCII.GetString(_buffer, 0, bytesRead));

                    // If the progress call back handler
                    // is not null then send to the client
                    // the number of bytes read.
                    if (_callbackHandler != null)
                        _callbackHandler((long)bytesRead);
                }

                // Receive more data if we expect more.
                // note: a literal "." (or more) followed by
                // "OK" in an email is prefixed a value
                if (!state.EmailMessage.ToString().Contains(_prefixNumber + " OK") &&
                    !state.EmailMessage.ToString().Contains(_prefixNumber + " NO") &&
                    !state.EmailMessage.ToString().Contains(_prefixNumber + " BAD"))
                {
                    // If a secure connection is required.
                    if (_connection.UseSSLConnection)
                    {
                        lock (sslStream)
                            sslStream.BeginRead(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                new AsyncCallback(ReceiveCallback), state);
                    }
                    else
                    {
                        lock (socket)
                            socket.BeginReceive(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
                    }
                }
                else
                    // The end of the message has been reached
                    // indicate by the ManualResetEvent thread
                    // handler the that operation is complete
                    // indicated to the ManualResetEvent handler
                    // to stop blocking.
                    state.ReceiveComplete.Set();
            }
            catch (Exception e)
            {
                // An exception has occurred, assign the
                // current operation exception and set
                // the recieved ManualResetEvent thread
                // handler to set indicating that the
                // operation must stop.
                state.OperationException = e;
                state.ReceiveComplete.Set();
            }
        }

        /// <summary>
        /// Validate all objects used by the client before connection.
        /// </summary>
        private void ClientValidation()
        {
            // Make sure that the Connection type has been created.
            if (_connection == null)
                throw new System.ArgumentNullException("Connection can not be null.",
                    new System.Exception("The Connection reference has not been set."));

            // Make sure that a valid username has been specified.
            if (String.IsNullOrEmpty(_connection.UserName))
                throw new System.ArgumentNullException("A valid username has not been specified.",
                    new System.Exception("No valid user credentials are set, set a valid username."));

            // Make sure that a valid password has been specified.
            if (String.IsNullOrEmpty(_connection.Password))
                throw new System.ArgumentNullException("A valid password has not been specified.",
                    new System.Exception("No valid user password was set, set a valid password."));

            // Make sure that a valid host has been specified.
            if (String.IsNullOrEmpty(_connection.Server))
                throw new System.ArgumentNullException("A valid host name has not been specified.",
                    new System.Exception("No valid remote host was specified to send data to."));

            // Make sure the the port number is greater than one.
            if (_connection.Port < 1)
                throw new System.ArgumentOutOfRangeException("The port number must be greater than zero.",
                    new System.Exception("No valid port number was set, set a valid port number."));

            // Make sure the the port number is greater than one.
            if (_connection.ServerType != Nequeo.Net.ServerType.SslIMAP4 &&
                _connection.ServerType != Nequeo.Net.ServerType.SslProxyIMAP4 &&
                _connection.ServerType != Nequeo.Net.ServerType.StartTslProxyIMAP4)
                throw new System.ArgumentOutOfRangeException("The server type must be set to an IMAP4 server type.",
                    new System.Exception("The server type has not been set."));
        }

        /// <summary>
        /// Certificate validator.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="certificate">The certificate</param>
        /// <param name="chain">The certificate chain.</param>
        /// <param name="sslPolicyErrors">The policy error.</param>
        /// <returns>True if the certificate is valid else false.</returns>
        private bool OnCertificateValidation(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Certificate should be validated.
            if (_connection.ValidateCertificate)
            {
                // If the certificate is valid
                // return true.
                if (sslPolicyErrors == SslPolicyErrors.None)
                    return true;
                else
                {
                    // Create a new certificate collection
                    // instance and return false.
                    _sslCertificate = new Nequeo.Security.X509Certificate2Info(
                        certificate as X509Certificate2, chain, sslPolicyErrors);
                    return false;
                }
            }
            else
                // Return true.
                return true;
        }
        #endregion

        #region Dispose Object Methods
        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_sslStream != null)
                        _sslStream.Dispose();

                    if (_socket != null)
                        _socket.Dispose();

                    if (_connection != null)
                        _connection.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _socket = null;
                _client = null;
                _sslStream = null;
                _connection = null;

                // Note disposing has been done.
                _disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Imap4Client()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// The imap4 client interface to the server.
    /// </summary>
    public interface IImap4Client
    {
        #region Public Properties
        /// <summary>
        /// Get set, the connection adapter.
        /// </summary>
        Imap4ConnectionAdapter Connection { get; set; }

        /// <summary>
        /// Get set, the message retreival progress call back handler.
        /// </summary>
        Nequeo.Threading.ActionHandler<long> MessageReturnProgress { get; set; }

        /// <summary>
        /// Get, the secure certificate..
        /// </summary>
        Nequeo.Security.X509Certificate2Info Certificate { get; }

        /// <summary>
        /// Get, the currrent message data.
        /// </summary>
        Message Message { get; }

        /// <summary>
        /// Get set, the write attachment to file indicator.
        /// </summary>
        bool WriteAttachmentToFile { get; set; }

        /// <summary>
        /// Get set, the complete raw email message only.
        /// </summary>
        bool RawEmailMessageOnly { get; set; }

        /// <summary>
        /// Get, the current connection state.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Get, an indicator for secure TLS connection.
        /// </summary>
        bool SecurelyConnected { get; }
        #endregion

        #region Public Events
        /// <summary>
        /// On command return error event.
        /// </summary>
        event Nequeo.Threading.EventHandler<ClientCommandArgs> OnCommandError;

        /// <summary>
        /// On validation return error event.
        /// </summary>
        event Nequeo.Threading.EventHandler<ClientThreadErrorArgs> OnValidationError;

        /// <summary>
        /// On command return error event.
        /// </summary>
        event Nequeo.Threading.EventHandler<ClientCommandArgs> OnCommandConnected;

        /// <summary>
        /// When an error occures on an asynchronous threa.
        /// </summary>
        event Nequeo.Threading.EventHandler<ClientThreadErrorArgs> OnAsyncThreadError;
        #endregion

        #region Public Methods
        /// <summary>
        /// Open a new connection.
        /// </summary>
        void Open();

        /// <summary>
        /// Close the current connection.
        /// </summary>
        void Close();

        /// <summary>
        /// Creates a new folder on the server.
        /// </summary>
        /// <param name="folderName">The folder to create.</param>
        /// <returns>True if the operation was successful else false.</returns>
        bool CreateFolder(string folderName);

        /// <summary>
        /// Deletes the folder on the server.
        /// </summary>
        /// <param name="folderName">The folder to delete.</param>
        /// <returns>True if the operation was successful else false.</returns>
        bool DeleteFolder(string folderName);

        /// <summary>
        /// Rename the folder on the server.
        /// </summary>
        /// <param name="sourceFolderName">The source folder.</param>
        /// <param name="destinationFolderName">The destination folder.</param>
        /// <returns>True if the operation was successful else false.</returns>
        bool RenameFolder(string sourceFolderName, string destinationFolderName);

        /// <summary>
        /// Get all folders for the account.
        /// </summary>
        /// <returns>The collection of all folders.</returns>
        List<string> GetFolders();

        /// <summary>
        /// Begin get all folders for the account.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginGetFolders(
            AsyncCallback callback, object state);

        /// <summary>
        /// End get all folders for the account.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the message was returned else false.</returns>
        List<string> EndGetFolders(IAsyncResult ar);

        /// <summary>
        /// Get all subscribed folders for the account.
        /// </summary>
        /// <returns>The collection of all folders.</returns>
        List<string> GetSubscribedFolders();

        /// <summary>
        /// Begin get all folders for the account.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginGetSubscribedFolders(
            AsyncCallback callback, object state);

        /// <summary>
        /// End get all folders for the account.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the message was returned else false.</returns>
        List<string> EndGetSubscribedFolders(IAsyncResult ar);

        /// <summary>
        /// Subscribe to the specific folder.
        /// </summary>
        /// <param name="folderName">The folder to subscribe to.</param>
        /// <returns>True if the operation was successful else false.</returns>
        bool SubscribeToFolder(string folderName);

        /// <summary>
        /// Un subscribe from the specific folder.
        /// </summary>
        /// <param name="folderName">The folder to unsubscribe from.</param>
        /// <returns>True if the operation was successful else false.</returns>
        bool UnSubscribeFromFolder(string folderName);

        /// <summary>
        /// Sets the current folder specified.
        /// </summary>
        /// <param name="folderName">The folder to view details on.</param>
        /// <returns>The details of the folder.</returns>
        List<string> SetFolder(string folderName);

        /// <summary>
        /// Begin set the specified folder details.
        /// </summary>
        /// <param name="folderName">The folder to view details on.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSetFolder(string folderName,
            AsyncCallback callback, object state);

        /// <summary>
        /// End set the specified folder details.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the message was returned else false.</returns>
        List<string> EndSetFolder(IAsyncResult ar);

        /// <summary>
        /// Copy one messsage to the specified folder.
        /// </summary>
        /// <param name="messageNumber">The message number to copy.</param>
        /// <param name="destinationFolder">The destination folder to copy to.</param>
        /// <param name="useUID">Use the unique identifier for messsage number.</param>
        /// <returns>True if the message was returned else false.</returns>
        bool CopyMessage(long messageNumber, string destinationFolder, bool useUID);

        /// <summary>
        /// Copy a group of messages to the specified folder.
        /// </summary>
        /// <param name="startMessageNumber">The start message to copy.</param>
        /// <param name="endMessageNumber">The end message to copy.</param>
        /// <param name="destinationFolder">The destination folder to copy to.</param>
        /// <param name="useUID">Use the unique identifier for messsage number.</param>
        /// <returns>True if the message was returned else false.</returns>
        /// <remarks>Set the 'endMessageNumber' variable to '0' for '*'.</remarks>
        bool CopyMessages(long startMessageNumber, long endMessageNumber, 
            string destinationFolder, bool useUID);

        /// <summary>
        /// Delete one messsage from the account.
        /// </summary>
        /// <param name="messageNumber">The message number to delete.</param>
        /// <param name="useUID">Use the unique identifier for messsage number.</param>
        /// <returns>True if the message was returned else false.</returns>
        bool DeleteMessage(long messageNumber, bool useUID);

        /// <summary>
        /// Delete a group of messages from the account.
        /// </summary>
        /// <param name="startMessageNumber">The start message to delete.</param>
        /// <param name="endMessageNumber">The end message to delete.</param>
        /// <param name="useUID">Use the unique identifier for messsage number.</param>
        /// <returns>True if the message was returned else false.</returns>
        /// <remarks>Set the 'endMessageNumber' variable to '0' for '*'.</remarks>
        bool DeleteMessages(long startMessageNumber,
            long endMessageNumber, bool useUID);

        /// <summary>
        /// Move one messsage to the specified folder.
        /// </summary>
        /// <param name="messageNumber">The message number to move.</param>
        /// <param name="destinationFolder">The destination folder to move to.</param>
        /// <param name="useUID">Use the unique identifier for messsage number.</param>
        /// <returns>True if the message was returned else false.</returns>
        bool MoveMessage(long messageNumber, string destinationFolder, bool useUID);

        /// <summary>
        /// Move a group of messages to the specified folder.
        /// </summary>
        /// <param name="startMessageNumber">The start message to move.</param>
        /// <param name="endMessageNumber">The end message to move.</param>
        /// <param name="destinationFolder">The destination folder to move to.</param>
        /// <param name="useUID">Use the unique identifier for messsage number.</param>
        /// <returns>True if the message was returned else false.</returns>
        /// <remarks>Set the 'endMessageNumber' variable to '0' for '*'.</remarks>
        bool MoveMessages(long startMessageNumber, long endMessageNumber, 
            string destinationFolder, bool useUID);

        /// <summary>
        /// Gets the size of the specified message.
        /// </summary>
        /// <param name="messageNumber">The message to get information on.</param>
        /// <returns>The size of the message number.</returns>
        List<string> GetMessageSize(long messageNumber);

        /// <summary>
        /// Gets the size of all messages in the account.
        /// </summary>
        /// <returns>The collection of message sizes.</returns>
        List<string> GetSizeOfMessages();

        /// <summary>
        /// Gets the size of the messages between the interval.
        /// </summary>
        /// <param name="startMessageNumber">The start message number.</param>
        /// <param name="endMessageNumber">The end message number.</param>
        /// <returns>The collection of message sizes.</returns>
        /// <remarks>Set the 'endMessageNumber' variable to '0' for '*'.</remarks>
        List<string> GetSizeOfMessages(long startMessageNumber, long endMessageNumber);

        /// <summary>
        /// Begin gets the size of the messages between the interval.
        /// </summary>
        /// <param name="startMessageNumber">The start message number.</param>
        /// <param name="endMessageNumber">The end message number.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginGetSizeOfMessages(long startMessageNumber, 
            long endMessageNumber, AsyncCallback callback, object state);

        /// <summary>
        /// Begin gets the size of all messages in the account.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginGetSizeOfMessages(AsyncCallback callback, object state);

        /// <summary>
        /// End gets the size of all messages in the account.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the message was returned else false.</returns>
        List<string> EndGetSizeOfMessages(IAsyncResult ar);

        /// <summary>
        /// Begin gets the size of the specified message.
        /// </summary>
        /// <param name="messageNumber">The message to get information on.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginGetMessageSize(long messageNumber,
            AsyncCallback callback, object state);

        /// <summary>
        /// End gets the size of the specified message.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the message was returned else false.</returns>
        List<string> EndGetMessageSize(IAsyncResult ar);

        /// <summary>
        /// Gets the specified message.
        /// </summary>
        /// <param name="messageNumber">The message number to return.</param>
        /// <returns>True if the message was returned else false.</returns>
        bool GetEmail(long messageNumber);

        /// <summary>
        /// Begin get the specified email message.
        /// </summary>
        /// <param name="messageNumber">The email message number.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginGetEmail(long messageNumber,
            AsyncCallback callback, object state);

        /// <summary>
        /// End get the specified email message.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the message was returned else false.</returns>
        bool EndGetEmail(IAsyncResult ar);

        /// <summary>
        /// Gets the specified message headers.
        /// </summary>
        /// <param name="messageNumber">The message number to return.</param>
        /// <returns>True if the message was returned else false.</returns>
        bool GetEmailHeaders(long messageNumber);

        /// <summary>
        /// Begin get the specified email message headers.
        /// </summary>
        /// <param name="messageNumber">The email message number.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginGetEmailHeaders(long messageNumber,
            AsyncCallback callback, object state);

        /// <summary>
        /// End get the specified email message headers.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the message was returned else false.</returns>
        bool EndGetEmailHeaders(IAsyncResult ar);

        /// <summary>
        /// Gets the total number of messages in the account.
        /// </summary>
        /// <returns>The number of messages.</returns>
        long GetMessageCount();

        /// <summary>
        /// Gets the collection of all un-seen messages for the account.
        /// </summary>
        /// <returns>Collection of un-seen messages.</returns>
        List<string> GetUnSeenMessages();

        /// <summary>
        /// Get the collection of attachment file names found in the message.
        /// </summary>
        /// <param name="messageNumber">The message number.</param>
        /// <returns>The collection of attachment file names found.</returns>
        List<string> GetAttachmentFileNames(long messageNumber);

        /// <summary>
        /// Get the email message body.
        /// </summary>
        /// <param name="messageNumber">The email message number.</param>
        /// <returns>The message body of the email.</returns>
        string GetEmailBody(long messageNumber);

        /// <summary>
        /// Begin get the specified email message body.
        /// </summary>
        /// <param name="messageNumber">The email message number.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginGetEmailBody(long messageNumber,
            AsyncCallback callback, object state);

        /// <summary>
        /// End get the specified email message body.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the message was returned else false.</returns>
        string EndGetEmailBody(IAsyncResult ar);

        #endregion
    }

    /// <summary>
    /// Class contains properties that hold all the
    /// connection collection for the specified Imap4 server.
    /// </summary>
    [Serializable]
    public class Imap4ConnectionAdapter : IImap4ConnectionAdapter, IDisposable
    {
        #region Constructors
        /// <summary>
        /// The imap4 connection adapter for emailing information.
        /// </summary>
        /// <param name="server">The imap4 mail server.</param>
        /// <param name="port">The imap4 mail server port.</param>
        /// <param name="userName">The imap4 account username.</param>
        /// <param name="password">The imap4 account password.</param>
        /// <param name="timeOut">The time out for connection (-1 for infinity).</param>
        /// <param name="useSSLConnection">Use a secure socket layer connection.</param>
        /// <param name="attachmentDirectory">The directory where attachments are stored.</param>
        public Imap4ConnectionAdapter(string server, int port,
            string userName, string password, int timeOut,
            bool useSSLConnection, string attachmentDirectory)
        {
            this.server = server;
            this.port = port;
            this.userName = userName;
            this.password = password;
            this.timeOut = timeOut;
            this.useSSLConnection = useSSLConnection;
            this.attachmentDirectory = attachmentDirectory;
        }

        /// <summary>
        /// The imap4 connection adapter for emailing information.
        /// </summary>
        /// <param name="server">The imap4 mail server.</param>
        /// <param name="port">The imap4 mail server port.</param>
        /// <param name="userName">The imap4 account username.</param>
        /// <param name="password">The imap4 account password.</param>
        /// <param name="timeOut">The time out for connection (-1 for infinity).</param>
        /// <param name="attachmentDirectory">The directory where attachments are stored.</param>
        public Imap4ConnectionAdapter(string server, int port,
            string userName, string password, int timeOut,
            string attachmentDirectory)
        {
            this.server = server;
            this.port = port;
            this.userName = userName;
            this.password = password;
            this.timeOut = timeOut;
            this.attachmentDirectory = attachmentDirectory;
        }

        /// <summary>
        /// The imap4 connection adapter for emailing information.
        /// </summary>
        /// <param name="server">The imap4 mail server.</param>
        /// <param name="userName">The imap4 account username.</param>
        /// <param name="password">The imap4 account password.</param>
        /// <param name="timeOut">The time out for connection (-1 for infinity).</param>
        /// <param name="useSSLConnection">Use a secure socket layer connection.</param>
        /// <param name="attachmentDirectory">The directory where attachments are stored.</param>
        public Imap4ConnectionAdapter(string server, string userName,
            string password, int timeOut, bool useSSLConnection,
            string attachmentDirectory)
        {
            this.server = server;
            this.userName = userName;
            this.password = password;
            this.timeOut = timeOut;
            this.useSSLConnection = useSSLConnection;
            this.attachmentDirectory = attachmentDirectory;
        }

        /// <summary>
        /// The imap4 connection adapter for emailing information.
        /// </summary>
        /// <param name="server">The imap4 mail server.</param>
        /// <param name="userName">The imap4 account username.</param>
        /// <param name="password">The imap4 account password.</param>
        /// <param name="timeOut">The time out for connection (-1 for infinity).</param>
        /// <param name="attachmentDirectory">The directory where attachments are stored.</param>
        public Imap4ConnectionAdapter(string server, string userName,
            string password, int timeOut, string attachmentDirectory)
        {
            this.server = server;
            this.userName = userName;
            this.password = password;
            this.timeOut = timeOut;
            this.attachmentDirectory = attachmentDirectory;
        }

        /// <summary>
        /// The imap4 connection adapter for emailing information.
        /// </summary>
        /// <param name="server">The imap4 mail server.</param>
        /// <param name="userName">The imap4 account username.</param>
        /// <param name="password">The imap4 account password.</param>
        /// <param name="timeOut">The time out for connection (-1 for infinity).</param>
        public Imap4ConnectionAdapter(string server, string userName,
            string password, int timeOut = -1)
        {
            this.server = server;
            this.userName = userName;
            this.password = password;
            this.timeOut = timeOut;
        }

        /// <summary>
        /// The imap4 connection adapter for emailing information.
        /// </summary>
        /// <param name="server">The imap4 mail server.</param>
        /// <param name="userName">The imap4 account username.</param>
        /// <param name="password">The imap4 account password.</param>
        public Imap4ConnectionAdapter(string server, string userName,
            string password)
        {
            this.server = server;
            this.userName = userName;
            this.password = password;
        }

        /// <summary>
        /// The imap4 connection adapter for emailing information.
        /// </summary>
        public Imap4ConnectionAdapter()
        {
        }
        #endregion

        #region Private Fields
        private string userName = string.Empty;
        private string password = string.Empty;
        private string server = string.Empty;
        private string domain = string.Empty;
        private string attachmentDirectory = @"C:\Temp";
        private int port = 143;
        private int timeOut = -1;
        private bool useSSLConnection = false;
        private bool disposed = false;
        private bool validateCertificate = false;
        private Nequeo.Net.ServerType serverType =
            Nequeo.Net.ServerType.SslIMAP4;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get set, should the ssl/tsl certificate be veryfied
        /// when making a secure connection.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public bool ValidateCertificate
        {
            get { return validateCertificate; }
            set { validateCertificate = value; }
        }

        /// <summary>
        /// Get set, the IMAP4 server type.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public Nequeo.Net.ServerType ServerType
        {
            get { return serverType; }
            set { serverType = value; }
        }

        /// <summary>
        /// Get Set, the user name.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// Get Set, the password.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        /// <summary>
        /// Get Set, the domain.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string Domain
        {
            get { return domain; }
            set { domain = value; }
        }

        /// <summary>
        /// Get Set, the imap4 server.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string Server
        {
            get { return server; }
            set { server = value; }
        }

        /// <summary>
        /// Get Set, the attachment directory.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string AttachmentDirectory
        {
            get { return attachmentDirectory; }
            set { attachmentDirectory = value; }
        }

        /// <summary>
        /// Get Set, the imap4 port.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        /// <summary>
        /// Get Set, the time out request.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public int TimeOut
        {
            get { return timeOut; }
            set { timeOut = value; }
        }

        /// <summary>
        /// Get Set, use ssl encryption transfer.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public bool UseSSLConnection
        {
            get { return useSSLConnection; }
            set { useSSLConnection = value; }
        }
        #endregion

        #region Dispose Object Methods
        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                userName = null;
                password = null;
                server = null;

                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Imap4ConnectionAdapter()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// Imap4 connection interface.
    /// </summary>
    public interface IImap4ConnectionAdapter
    {
        #region Public Properties

        /// <summary>
        /// Get Set, the user name.
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// Get Set, the password.
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Get Set, the imap4 server.
        /// </summary>
        string Server { get; set; }

        /// <summary>
        /// Get Set, the attachment directory.
        /// </summary>
        string AttachmentDirectory { get; set; }

        /// <summary>
        /// Get Set, the imap4 port.
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// Get Set, the time out request.
        /// </summary>
        int TimeOut { get; set; }

        /// <summary>
        /// Get Set, use ssl encryption transfer.
        /// </summary>
        bool UseSSLConnection { get; set; }

        #endregion
    }
}
