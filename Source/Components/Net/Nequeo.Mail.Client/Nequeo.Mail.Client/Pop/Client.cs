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

namespace Nequeo.Net.Mail.Pop
{
    /// <summary>
    /// The pop3 client class to the server.
    /// </summary>
    public class Pop3Client : IDisposable, IPop3Client
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Pop3Client()
        {
        }

        /// <summary>
        /// Connection adapter constructor.
        /// </summary>
        /// <param name="connection">The connection adapter used to connect to the server.</param>
        public Pop3Client(Pop3ConnectionAdapter connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Connection adapter constructor.
        /// </summary>
        /// <param name="connection">The connection adapter used to connect to the server.</param>
        /// <param name="writeAttachmentToFile">Write the attachemnt to the file.</param>
        public Pop3Client(Pop3ConnectionAdapter connection,
            bool writeAttachmentToFile)
        {
            _connection = connection;
            _writeAttachmentToFile = writeAttachmentToFile;
        }
        #endregion

        #region Private Fields
        private Socket _socket = null;
        private Message _message = null;
        private TcpClient _client = null;
        private SslStream _sslStream = null;
        private Pop3ConnectionAdapter _connection = null;
        private Nequeo.Threading.ActionHandler<long> _callbackHandler = null;
        private Nequeo.Security.X509Certificate2Info _sslCertificate = null;
       
        private bool _userConnected = false;
        private bool _clientConnected = false;
        private bool _writeAttachmentToFile = false;
        private bool _rawEmailMessageOnly = false;

        private bool _disposed = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get set, the connection adapter.
        /// </summary>
        public Pop3ConnectionAdapter Connection
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
        /// Get, the currrent message data.
        /// </summary>
        public Message Message
        {
            get { return _message; }
        }

        /// <summary>
        /// Get, the current connection state.
        /// </summary>
        public bool Connected
        {
            get { return _userConnected; }
        }

        /// <summary>
        /// Get, the number of email messages for
        /// the current connection.
        /// </summary>
        public long Count
        {
            get
            {
                // If no connection exists
                // then thrown an exception.
                if ((_socket == null && _client == null) || (!_clientConnected))
                    throw new Exception("Not connected to a server.");

                // If user connected
                if (_userConnected)
                {
                    try
                    {
                        // Set the inital count.
                        long count = 0;

                        // Send to the server the status command.
                        SendCommand("STAT");

                        // Get the server response.
                        string returned = GetServerResponse();

                        // If data is return matches the regex
                        // then a valid response.
                        if (Regex.Match(returned, @"^.*\+OK[ |	]+([0-9]+)[ |	]+.*$").Success)
                        {
                            // Get the number of email messages.
                            count = long.Parse(Regex.Replace(returned.Replace("\r\n", ""),
                                @"^.*\+OK[ |	]+([0-9]+)[ |	]+.*$", "$1"));
                        }
                        else
                            if (OnCommandError != null)
                                OnCommandError(this, new ClientCommandArgs("STAT",
                                    "Could not return the email count.", 400));

                        // Get the email count.
                        return count;
                    }
                    catch (Exception e)
                    {
                        if (OnAsyncThreadError != null)
                            OnAsyncThreadError(this, new ClientThreadErrorArgs(
                                e.Message, 800));

                        return 0;
                    }
                }
                else
                    throw new Exception("User has not logged in.");
            }
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
            _message = null;
            _client = null;
            _sslStream = null;

            // If a secure connection is required.
            if (_connection.UseSSLConnection)
                // Get the tcp client socket to
                // the server.
                _client = GetTcpClient();
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
                if (!header.Substring(0, 3).Equals("+OK"))
                    throw new Exception("Invalid initial server response.");

                // Set the connection status to true
                // the client is connected.
                _clientConnected = true;

                // Login the current client.
                Login();
            }
        }

        /// <summary>
        /// Close the current connection.
        /// </summary>
        public virtual void Close()
        {
            if (_socket != null)
            {
                if (_clientConnected)
                {
                    // Send a quit command to the server.
                    SendCommand("QUIT");
                    _socket.Close();

                    // Set the client to no connection.
                    _clientConnected = false;
                }

                _socket = null;
                _message = null;
            }

            if (_client != null)
            {
                if (_clientConnected)
                {
                    // Send a quit command to the server.
                    SendCommand("QUIT");
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
        /// Deletes the specified email message.
        /// </summary>
        /// <param name="position">The email message position.</param>
        /// <returns>True if the message was deleted else false.</returns>
        public virtual bool DeleteEmail(long position)
        {
            // If no connection exists
            // then thrown an exception.
            if ((_socket == null && _client == null) || (!_clientConnected))
                throw new Exception("Not connected to a server.");

            // If the position is less than
            // one then throw exception.
            if (position < 1)
                throw new Exception("Position must be greater than zero.");

            bool ret = false;

            // If user connected.
            if (_userConnected)
            {
                // Send a delete command to
                // the server.
                SendCommand("DELE " + position.ToString());

                // Get the response from the server.
                string returned = GetServerResponse();

                if (Regex.Match(returned, @"^.*\+OK.*$").Success)
                    ret = true;
                else
                    if (OnCommandError != null)
                        OnCommandError(this, new ClientCommandArgs("DELE",
                            "Could not delete the email messsage.", 403));
            }
            else
                throw new Exception("User has not logged in.");

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Get the specified email message.
        /// </summary>
        /// <param name="position">The email message position.</param>
        /// <returns>True if the message was returned else false.</returns>
        public virtual bool GetEmail(long position)
        {
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
                // If the position is less than
                // one then throw exception.
                if (position < 1)
                {
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "Position must be greater than zero.", 501));

                    // Not valid message position.
                    valid = false;
                }

                // If valid.
                if (valid)
                {
                    // If user connected.
                    if (_userConnected)
                        ret = GetCurrentEmail(position, false, 0);
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
        /// <param name="position">The email message position.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginGetEmail(long position,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncGetEmailMessage(position, this, callback, state);
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
        /// Get the specified email message header data.
        /// </summary>
        /// <param name="position">The email message position.</param>
        /// <param name="headerLineCount">The number of message lines to get.</param>
        /// <returns>True if the message headers have been returned else false.</returns>
        public virtual bool GetEmailHeaders(long position, long headerLineCount)
        {
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
                // If the position is less than
                // one then throw exception.
                if (position < 1)
                {
                    if (OnValidationError != null)
                        OnValidationError(this, new ClientThreadErrorArgs(
                            "Position must be greater than zero.", 501));

                    // Not valid message position.
                    valid = false;
                }
               
                // If valid.
                if (valid)
                {
                    // If user connected.
                    if (_userConnected)
                        ret = GetCurrentEmail(position, true, headerLineCount);
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
        /// <param name="position">The email message position.</param>
        /// <param name="headerLineCount">The number of message lines to get.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginGetEmailHeaders(long position, long headerLineCount,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncGetEmailMessage(position, headerLineCount, this, callback, state);
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
        /// Gets the size of the specified email message.
        /// </summary>
        /// <param name="position">The email message position.</param>
        /// <returns>The size of the message.</returns>
        public virtual long GetEmailSize(long position)
        {
            // If no connection exists
            // then thrown an exception.
            if ((_socket == null && _client == null) || (!_clientConnected))
                throw new Exception("Not connected to a server.");

            // If the position is less than
            // one then throw exception.
            if (position < 1)
                throw new Exception("Position must be greater than zero.");

            long ret = 0;

            // If user connected.
            if (_userConnected)
                ret = GetCurrentEmailSize(position);
            else
                throw new Exception("User has not logged in.");

            // Return the result.
            return ret;
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
				foreach(IPAddress address in hostEntry.AddressList)
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
			catch(Exception e)
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
                if (_connection.UseSSLConnection)
                    // Send the command to the server.
                    _sslStream.Write(byteData);
                else
				    // Send the command to the server.
				    _socket.Send(byteData);
			}
			catch(Exception e)
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
                byte[] buffer = new byte[EmailMessageParse.MAX_BUFFER_READ_SIZE];
                int byteCount = 0;

                // If a secure connection is required.
                if (_connection.UseSSLConnection)
                    // Get the data from the server placed it in the buffer.
                    byteCount = _sslStream.Read(buffer, 0, buffer.Length);
                else
                    // Get the data from the server placed it in the buffer.
                    byteCount = _socket.Receive(buffer, buffer.Length, 0);

                // Decode the data in the buffer to a string.
				string line = Encoding.ASCII.GetString(buffer, 0, byteCount);

                // Return the response from the server.
                return line;
			}
			catch(Exception e)
			{
                if (OnAsyncThreadError != null)
                    OnAsyncThreadError(this, new ClientThreadErrorArgs(
                        e.Message, 801));

                return null;
			}
		}

        /// <summary>
        /// Login the current user.
        /// </summary>
		private void Login()
		{
			string returned;
            bool valid = true;

			// Send the user name.
            SendCommand("USER " + _connection.UserName);

            // Get the server response.
            returned = GetServerResponse();

            // If the server do not send
            // a comfirmation response.
            if (!returned.Substring(0, 3).Equals("+OK"))
            {
                if (OnCommandError != null)
                    OnCommandError(this, new ClientCommandArgs("USER",
                        "The user name and or password are not valid.", 401));

                // Invalid login;
                valid = false;
            }

            // If valid user name then
            // continue.
            if (valid)
            {
                // Send the password.
                SendCommand("PASS " + _connection.Password);

                // Get the server response.
                returned = GetServerResponse();

                // If the server do not send
                // a comfirmation response.
                if (!returned.Substring(0, 3).Equals("+OK"))
                {
                    if (OnCommandError != null)
                        OnCommandError(this, new ClientCommandArgs("PASS",
                            "The user name and or password are not valid.", 402));

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
		}

        /// <summary>
        /// Gets the size of the specified email message.
        /// </summary>
        /// <param name="position">The email message position.</param>
        /// <returns>The size of the message.</returns>
        private long GetCurrentEmailSize(long position)
        {
            string returned;
            bool valid = true;
            long size = 0;

            // Send a list request command.
            SendCommand("LIST " + position.ToString());

            // Get the server response.
            returned = GetServerResponse();

            // If some error occurred
            if (returned.Substring(0, 4).Equals("-ERR"))
            {
                if (OnCommandError != null)
                    OnCommandError(this, new ClientCommandArgs("LIST",
                        "The server sent an error response.", 403));

                // List request failed
                valid = false;
            }

            // If valid list then
            // continue.
            if (valid)
            {
                // Strip out CRLF and get the
                // list elements from the returned message
                // includes the number of emails and
                // the size of the message.
                string[] noCr = returned.Split(new char[] { '\r' });
                string[] elements = noCr[0].Split(new char[] { ' ' });

                // Element two contains the size.
                size = long.Parse(elements[2]);
            }

            // Return the email size.
            return size;
        }
		
        /// <summary>
        /// Get the specified email from the server.
        /// </summary>
        /// <param name="position">The email position number.</param>
        /// <param name="headerOnly">Get header data only.</param>
        /// <param name="headerLineCount">The header line count limit.</param>
        /// <returns>True if the message was returned else false.</returns>
        private bool GetCurrentEmail(long position, bool headerOnly, 
            long headerLineCount)
		{
            string returned;
            bool valid = true;
            long size = 0;
            bool ret = false;

            // Send a list request command.
            SendCommand("LIST " + position.ToString());

            // Get the server response.
            returned = GetServerResponse();

            // If some error occurred
            if (returned.Substring(0, 4).Equals("-ERR"))
            {
                if (OnCommandError != null)
                    OnCommandError(this, new ClientCommandArgs("LIST",
                        "The server sent an error response.", 404));

                // List request failed
                valid = false;
            }

            // If valid list then
            // continue.
            if (valid)
            {
                // Strip out CRLF and get the
                // list elements from the returned message
                // includes the number of emails and
                // the size of the message.
                string[] noCr = returned.Split(new char[] { '\r' });
                string[] elements = noCr[0].Split(new char[] { ' ' });

                // Element two contains the size.
                size = long.Parse(elements[2]);

                // Create a new attachment list and
                // an new message class that will contain
                // the current message information.
                // Assign the attachment object to the
                // attachment property in the message object.
                List<Attachment> attachments = new List<Attachment>();
                _message = new Message();
                _message.Attachments = attachments;

			    try
                {
                    // Create a new email message class,
                    // starts the retreival of the specified
                    // email message.
                    EmailMessage emailMessage =
                        new EmailMessage(position, size, headerOnly, headerLineCount, _socket,
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

            // Return the result.
            return ret;
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
        ~Pop3Client()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
	}

    /// <summary>
    /// The pop3 client interface to the server.
    /// </summary>
    public interface IPop3Client
    {
        #region Public Properties
        /// <summary>
        /// Get set, the connection adapter.
        /// </summary>
        Pop3ConnectionAdapter Connection { get; set; }

        /// <summary>
        /// Get set, the message retreival progress call back handler.
        /// </summary>
        Nequeo.Threading.ActionHandler<long> MessageReturnProgress { get; set; }

        /// <summary>
        /// Get, the secure certificate..
        /// </summary>
        Nequeo.Security.X509Certificate2Info Certificate { get; }

        /// <summary>
        /// Get set, the write attachment to file indicator.
        /// </summary>
        bool WriteAttachmentToFile { get; set; }

        /// <summary>
        /// Get set, the complete raw email message only.
        /// </summary>
        bool RawEmailMessageOnly { get; set; }

        /// <summary>
        /// Get, the currrent message data.
        /// </summary>
        Message Message { get; }

        /// <summary>
        /// Get, the current connection state.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Get, the number of email messages for
        /// the current connection.
        /// </summary>
        long Count { get; }

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
        /// Deletes the specified email message.
        /// </summary>
        /// <param name="position">The email message position.</param>
        /// <returns>True if the message was deleted else false.</returns>
        bool DeleteEmail(long position);

        /// <summary>
        /// Get the specified email message.
        /// </summary>
        /// <param name="position">The email message position.</param>
        /// <returns>True if the message was returned else false.</returns>
        bool GetEmail(long position);

        /// <summary>
        /// Begin get the specified email message.
        /// </summary>
        /// <param name="position">The email message position.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginGetEmail(long position,
            AsyncCallback callback, object state);

        /// <summary>
        /// End get the specified email message.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the message was returned else false.</returns>
        bool EndGetEmail(IAsyncResult ar);

        /// <summary>
        /// Get the specified email message header data.
        /// </summary>
        /// <param name="position">The email message position.</param>
        /// <param name="headerLineCount">The number of message lines to get.</param>
        /// <returns>True if the message headers have been returned else false.</returns>
        bool GetEmailHeaders(long position, long headerLineCount);

        /// <summary>
        /// Begin get the specified email message headers.
        /// </summary>
        /// <param name="position">The email message position.</param>
        /// <param name="headerLineCount">The number of message lines to get.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginGetEmailHeaders(long position, long headerLineCount,
            AsyncCallback callback, object state);

        /// <summary>
        /// End get the specified email message headers.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the message was returned else false.</returns>
        bool EndGetEmailHeaders(IAsyncResult ar);

        /// <summary>
        /// Gets the size of the specified email message.
        /// </summary>
        /// <param name="position">The email message position.</param>
        /// <returns>The size of the message.</returns>
        long GetEmailSize(long position);

        #endregion
    }

    /// <summary>
    /// Class contains properties that hold all the
    /// connection collection for the specified Pop3 server.
    /// </summary>
    [Serializable]
    public class Pop3ConnectionAdapter : IPop3ConnectionAdapter, IDisposable
    {
        #region Constructors
        /// <summary>
        /// The pop3 connection adapter for emailing information.
        /// </summary>
        /// <param name="server">The pop3 mail server.</param>
        /// <param name="port">The pop3 mail server port.</param>
        /// <param name="userName">The pop3 account username.</param>
        /// <param name="password">The pop3 account password.</param>
        /// <param name="timeOut">The time out for connection (-1 for infinity).</param>
        /// <param name="useSSLConnection">Use a secure socket layer connection.</param>
        /// <param name="attachmentDirectory">The directory where attachments are stored.</param>
        public Pop3ConnectionAdapter(string server, int port,
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
        /// The pop3 connection adapter for emailing information.
        /// </summary>
        /// <param name="server">The pop3 mail server.</param>
        /// <param name="port">The pop3 mail server port.</param>
        /// <param name="userName">The pop3 account username.</param>
        /// <param name="password">The pop3 account password.</param>
        /// <param name="timeOut">The time out for connection (-1 for infinity).</param>
        /// <param name="attachmentDirectory">The directory where attachments are stored.</param>
        public Pop3ConnectionAdapter(string server, int port,
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
        /// The pop3 connection adapter for emailing information.
        /// </summary>
        /// <param name="server">The pop3 mail server.</param>
        /// <param name="userName">The pop3 account username.</param>
        /// <param name="password">The pop3 account password.</param>
        /// <param name="timeOut">The time out for connection (-1 for infinity).</param>
        /// <param name="useSSLConnection">Use a secure socket layer connection.</param>
        /// <param name="attachmentDirectory">The directory where attachments are stored.</param>
        public Pop3ConnectionAdapter(string server, string userName,
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
        /// The pop3 connection adapter for emailing information.
        /// </summary>
        /// <param name="server">The pop3 mail server.</param>
        /// <param name="userName">The pop3 account username.</param>
        /// <param name="password">The pop3 account password.</param>
        /// <param name="timeOut">The time out for connection (-1 for infinity).</param>
        /// <param name="attachmentDirectory">The directory where attachments are stored.</param>
        public Pop3ConnectionAdapter(string server, string userName,
            string password, int timeOut, string attachmentDirectory)
        {
            this.server = server;
            this.userName = userName;
            this.password = password;
            this.timeOut = timeOut;
            this.attachmentDirectory = attachmentDirectory;
        }

        /// <summary>
        /// The pop3 connection adapter for emailing information.
        /// </summary>
        /// <param name="server">The pop3 mail server.</param>
        /// <param name="userName">The pop3 account username.</param>
        /// <param name="password">The pop3 account password.</param>
        /// <param name="timeOut">The time out for connection (-1 for infinity).</param>
        public Pop3ConnectionAdapter(string server, string userName,
            string password, int timeOut = -1)
        {
            this.server = server;
            this.userName = userName;
            this.password = password;
            this.timeOut = timeOut;
        }

        /// <summary>
        /// The pop3 connection adapter for emailing information.
        /// </summary>
        /// <param name="server">The pop3 mail server.</param>
        /// <param name="userName">The pop3 account username.</param>
        /// <param name="password">The pop3 account password.</param>
        public Pop3ConnectionAdapter(string server, string userName,
            string password)
        {
            this.server = server;
            this.userName = userName;
            this.password = password;
        }

        /// <summary>
        /// The pop3 connection adapter for emailing information.
        /// </summary>
        public Pop3ConnectionAdapter()
        {
        }
        #endregion

        #region Private Fields
        private string userName = string.Empty;
        private string password = string.Empty;
        private string server = string.Empty;
        private string domain = string.Empty;
        private string attachmentDirectory = @"C:\Temp";
        private int port = 110;
        private int timeOut = -1;
        private bool useSSLConnection = false;
        private bool disposed = false;
        private bool validateCertificate = false;
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
        /// Get Set, the pop3 server.
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
        /// Get Set, the pop3 port.
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
        ~Pop3ConnectionAdapter()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// Pop3 connection interface.
    /// </summary>
    public interface IPop3ConnectionAdapter
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
        /// Get Set, the pop3 server.
        /// </summary>
        string Server { get; set; }

        /// <summary>
        /// Get Set, the attachment directory.
        /// </summary>
        string AttachmentDirectory { get; set; }

        /// <summary>
        /// Get Set, the pop3 port.
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
