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

using Nequeo.Invention;

namespace Nequeo.Net.Mail.Imap
{
    /// <summary>
    /// Email messsage class contains methods that return
    /// a complete imap4 email messsage.
    /// </summary>
    internal class EmailMessage
    {
        #region Constructors
        /// <summary>
        /// Email message constructor.
        /// </summary>
        /// <param name="position">The position of the message.</param>
        /// <param name="size">The size of the complete message.</param>
        /// <param name="headersOnly">The should only header data be returned.</param>
        /// <param name="headerLineCount">The number of header data lines to return.</param>
        /// <param name="socket">The current client socket.</param>
        /// <param name="sslStream">The current secure socket layer client.</param>
        /// <param name="message">The message object that will contain the message data.</param>
        /// <param name="writeAttachmentToFile">Write the attachment to file.</param>
        /// <param name="rawEmailMessageOnly">Get the complete raw email message only.</param>
        /// <param name="connection">The current connection object.</param>
        /// <param name="callbackHandler">The progress call back handler.</param>
        public EmailMessage(long position, long size, bool headersOnly, long headerLineCount,
            Socket socket, SslStream sslStream, Message message, bool writeAttachmentToFile,
            bool rawEmailMessageOnly, Imap4ConnectionAdapter connection,
            Nequeo.Threading.ActionHandler<long> callbackHandler)
        {
            try
            {
                _size = size;
                _socket = socket;
                _sslStream = sslStream;
                _connection = connection;
                _inboxPosition = position;
                _headersOnly = headersOnly;
                _headerLineCount = headerLineCount;
                _callbackHandler = callbackHandler;
                _rawEmailMessageOnly = rawEmailMessageOnly;
                _writeAttachmentToFile = writeAttachmentToFile;


                // Load the email asynchronously.
                // This point is blocked until the
                // receive is complete.
                LoadEmail(message);

                // If the decoded email message is
                // to be returned.
                if (!rawEmailMessageOnly)
                {
                    // Get body (if it exists)
                    IEnumerator multipartEnumerator =
                        MultipartEnumerator;

                    // For each multipart content, including
                    // attachments iterate through the array.
                    while (multipartEnumerator.MoveNext())
                    {
                        // Get the multipart object index.
                        MessageAttachment multipart = (MessageAttachment)
                            multipartEnumerator.Current;

                        // If this multipart object is a body type
                        // then get the data stored.
                        if (multipart.IsBody)
                            _body = multipart.Data;

                        // If this multipart object is an attachment type
                        // then add the file name to the collection.
                        if (multipart.IsAttachment)
                        {
                            // Create a new attachment class
                            // and add the file data.
                            Attachment attachment =
                                new Attachment()
                                {
                                    FileName = multipart.Filename,
                                    File = multipart.FileData,
                                    FileExtention = multipart.FileExtension,
                                    FileNoExtention = multipart.FileNoExtension,
                                    Name = multipart.Name,
                                    ContentTransferEncoding = multipart.ContentTransferEncoding,
                                    Decoded = multipart.Decoded,
                                    ContentDescription = multipart.ContentDescription,
                                    ContentType = multipart.ContentType
                                };

                            // Add the attachment to the list.
                            message.Attachments.Add(attachment);
                        }
                    }

                    // Load the message data into the
                    // message class.
                    message.To = _to;
                    message.Cc = _cc;
                    message.Bcc = _bcc;
                    message.Size = _size;
                    message.Date = _date;
                    message.From = _from;
                    message.Subject = _subject;
                    message.ReplyTo = _replyTo;
                    message.Priority = _priority;
                    message.Received = _received;
                    message.ReturnPath = _returnPath;
                    message.Importance = _importance;
                    message.IsMultipart = _isMultipart;
                    message.Organization = _organization;
                    message.InboxPosition = _inboxPosition;

                    // If the message body is a html message.
                    if (_body.IndexOf("<html") > -1)
                    {
                        // A html message has been sent.
                        message.MessageType = Nequeo.Net.Mail.MessageType.Html;

                        // Get the starting index of the string
                        // and the ending index of the string.
                        int start = _body.IndexOf("<html");
                        int end = _body.LastIndexOf("</html>") + 7;

                        // Extract only the html part of the message body.
                        message.Body = _body.Substring(start, end - start);
                    }
                    else if (_body.IndexOf("<HTML") > -1)
                    {
                        // A rich text or html message has been sent.
                        message.MessageType = Nequeo.Net.Mail.MessageType.RichText;

                        // Get the starting index of the string
                        // and the ending index of the string.
                        int start = _body.IndexOf("<HTML");
                        int end = _body.LastIndexOf("</HTML>") + 7;

                        // Extract only the html part of the message body.
                        message.Body = _body.Substring(start, end - start);
                    }
                    else if (_body.ToLower().IndexOf("<xml") > -1)
                    {
                        // A xml message has been sent.
                        message.MessageType = Nequeo.Net.Mail.MessageType.Xml;

                        // Get the starting index of the string
                        // and the ending index of the string.
                        int start = _body.ToLower().IndexOf("<xml");
                        int end = _body.ToLower().LastIndexOf("</xml>") + 6;

                        // Extract only the xml part of the message body.
                        message.Body = _body.Substring(start, end - start);
                    }
                    else
                    {
                        // A normal text message has been sent.
                        message.MessageType = Nequeo.Net.Mail.MessageType.Text;
                        message.Body = _body;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Fields
        private Socket _socket = null;
        private SslStream _sslStream = null;
        private MessageBody _messageBody = null;
        private bool _writeAttachmentToFile = false;
        private Imap4ConnectionAdapter _connection = null;
        private Nequeo.Threading.ActionHandler<long> _callbackHandler = null;
        private byte[] _buffer = new byte[EmailMessageParse.MAX_BUFFER_READ_SIZE];

        private string _from = null;
        private string _to = null;
        private string _subject = null;
        private string _contentType = null;
        private string _body = null;
        private string _replyTo = null;
        private string _date = null;
        private string _organization = null;
        private string _returnPath = null;
        private string _received = null;
        private string _cc = null;
        private string _bcc = null;
        private string _priority = null;
        private string _importance = null;
        private string _multipartBoundary = null;

        private long _size = 0;
        private long _inboxPosition = 0;
        private long _headerLineCount = 0;

        private bool _headersOnly = false;
        private bool _isMultipart = false;
        private string _prefixNumber = string.Empty;

        private bool _rawEmailMessageOnly = false;
        #endregion

        #region Properites
        /// <summary>
        /// Get Message body component enumerator. The message body class
        /// also contains a collection of all the attachments sent with
        /// the message.
        /// </summary>
        private IEnumerator MultipartEnumerator
        {
            get { return _messageBody.ComponentEnumerator; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Start loading the current requested message.
        /// </summary>
        /// <param name="message">The message object that will contain the message data.</param>
        private void LoadEmail(Message message)
        {
            NumberGenerator randomNumber = new NumberGenerator();
            UpperCaseGenerator randomUpper = new UpperCaseGenerator();
            _prefixNumber = randomUpper.Random(1) + randomNumber.Random(3);

            // If only message header data is required.
            if (_headersOnly)
                if (_headerLineCount > 0)
                {
                    // Send a command to the server indicating that
                    // only body data is required. Send the message
                    // position and the number of the body header,
                    // '1' indicates only body text..
                    SendCommand(_prefixNumber + " FETCH " + _inboxPosition + 
                        ":" + _inboxPosition + " BODY[" + _headerLineCount + "]");
                }
                else
                {
                    // Send a command to the server indicating that
                    // only header data is required. Send the message
                    // position and the number of message lines to return.
                    SendCommand(_prefixNumber + " FETCH " + _inboxPosition + 
                        ":" + _inboxPosition + " BODY[HEADER]");
                }
            else
                // Send a command to return the entire
                // message. Send the message position.
                SendCommand(_prefixNumber + " FETCH " + _inboxPosition +
                    ":" + _inboxPosition + " BODY[]");

            // Start the receive process
            // at this point, and return the state
            // object that contains the email message
            // built from the string builder.
            Imap4StateAdapter state = StartReceive();

            // Return the complete raw email message only
            if (_rawEmailMessageOnly)
                // Assign the complete email message.
                message.RawEmailMessage = state.EmailMessage.ToString();
            else
                // Parse the email message for header processing
                // of the line within the email message.
                ParseEmail(state.EmailMessage.ToString().Split(new char[] { '\r' }));
        }

        /// <summary>
        /// Start receiving the email message.
        /// </summary>
        /// <returns>The current state object.</returns>
        private Imap4StateAdapter StartReceive()
        {
            // The manual reset event handler user to
            // block the current thread until the operation
            // completes the task, and create a new state
            // object to hold all state data.
            ManualResetEvent waitObject = null;
            Imap4StateAdapter state = new Imap4StateAdapter();

            // Assign the current client
            // socket object.
            state.Socket = _socket;
            state.SslClient = _sslStream;

            // Get the event to wait on.
            waitObject = state.ReceiveComplete;

            // If a secure connection is required.
            if (_connection.UseSSLConnection)
            {
                // Start receiving data asynchrounusly.
                lock (_sslStream)
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

            // Return the state object.
            return state;
        }

        /// <summary>
        /// Receive call back asynchrounus result.
        /// Handles the receiving of the email message.
        /// </summary>
        /// <param name="ar">The current asynchronus result.</param>
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
                        lock (sslStream)
                            sslStream.BeginRead(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                new AsyncCallback(ReceiveCallback), state);
                    else
                        lock (socket)
                            socket.BeginReceive(_buffer, 0, EmailMessageParse.MAX_BUFFER_READ_SIZE,
                                SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
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
        /// Send the command and data to the server.
        /// </summary>
        /// <param name="data">The data to sent.</param>
        private void SendCommand(String data)
        {
            // Convert the string data to byte data 
            // using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data + "\r\n");

            // If a secure connection is required.
            if (_connection.UseSSLConnection)
                // Send the command to the server.
                _sslStream.Write(byteData);
            else
                // Sending the data to the remote device.
                _socket.Send(byteData);
        }

        /// <summary>
        /// Parse the email message through each header required.
        /// </summary>
        /// <param name="lines">The array of email message lines.</param>
        private void ParseEmail(string[] lines)
        {
            // Parse the message headers and find the
            // start of the body.
            long startOfBody = ParseHeader(lines);
            long numberOfLines = lines.Length;

            // Create a new message body.
            _messageBody = new MessageBody(lines, startOfBody, _multipartBoundary,
                _contentType, _writeAttachmentToFile, _connection);
        }

        /// <summary>
        /// Parse each header type for each line in the email message.
        /// </summary>
        /// <param name="lines">The array of lines in the email message.</param>
        /// <returns>The start of the message body.</returns>
        private long ParseHeader(string[] lines)
        {
            // Get the total number of lines
            // and set the body start count to
            // zero.
            int numberOfLines = lines.Length;
            long bodyStart = 0;

            // For each line in the email message
            // parse all headers required.
            for (int i = 0; i < numberOfLines; i++)
            {
                // Get the current email line.
                string currentLine = lines[i].Replace("\n", "");

                // Get the current line header type.
                Nequeo.Net.Mail.MesssageHeaderType lineType = GetHeaderType(currentLine);

                // Get each haeder type.
                switch (lineType)
                {
                    case Nequeo.Net.Mail.MesssageHeaderType.From:
                        // From email message.
                        _from = EmailMessageParse.From(currentLine);
                        break;

                    case Nequeo.Net.Mail.MesssageHeaderType.Subject:
                        // Subject of the message.
                        _subject = EmailMessageParse.Subject(currentLine);
                        break;

                    case Nequeo.Net.Mail.MesssageHeaderType.To:
                        // To email message.
                        _to = EmailMessageParse.To(currentLine);
                        break;

                    case Nequeo.Net.Mail.MesssageHeaderType.ReplyTo:
                        // Reply to email message.
                        _replyTo = EmailMessageParse.ReplyTo(currentLine);
                        break;

                    case Nequeo.Net.Mail.MesssageHeaderType.Date:
                        // Date received message.
                        _date = EmailMessageParse.Date(currentLine);
                        break;

                    case Nequeo.Net.Mail.MesssageHeaderType.Organization:
                        // The orginization that sent the message.
                        _organization = EmailMessageParse.Organization(currentLine);
                        break;

                    case Nequeo.Net.Mail.MesssageHeaderType.ReturnPath:
                        // The email address return path.
                        _returnPath = EmailMessageParse.ReturnPath(currentLine);
                        break;

                    case Nequeo.Net.Mail.MesssageHeaderType.Received:
                        // The computers that sent and recevied the message.
                        _received += EmailMessageParse.Received(currentLine);
                        break;

                    case Nequeo.Net.Mail.MesssageHeaderType.Cc:
                        // The carbon copy email address.
                        _cc = EmailMessageParse.Cc(currentLine);
                        break;

                    case Nequeo.Net.Mail.MesssageHeaderType.Bcc:
                        // The blind carbon copy email address.
                        _bcc = EmailMessageParse.Bcc(currentLine);
                        break;

                    case Nequeo.Net.Mail.MesssageHeaderType.Priority:
                        // The email message priority.
                        _priority = EmailMessageParse.Priority(currentLine);
                        break;

                    case Nequeo.Net.Mail.MesssageHeaderType.Importance:
                        // The importance of the message.
                        _importance = EmailMessageParse.Importance(currentLine);
                        break;

                    case Nequeo.Net.Mail.MesssageHeaderType.ContentType:
                        // The content type of the message
                        // that has been specified (text, html etc..).
                        _contentType = EmailMessageParse.ContentType(currentLine);

                        // Is multi part, that is are
                        // attachments included.
                        _isMultipart = EmailMessageParse.IsMultipart(_contentType);

                        // If multipart.
                        if (_isMultipart)
                        {
                            // Boundary has been found
                            // for the attachments.
                            if (_contentType.Substring(_contentType.Length - 1, 1).Equals(";"))
                            {
                                // Increament the line index by one
                                // move to the next line.
                                ++i;

                                // Get the boundary data from the message.
                                _multipartBoundary = EmailMessageParse.MultipartBoundary(lines[i].Replace("\n", ""));
                            }
                            else
                            {
                                // Boundary definition is on same
                                // line as Content-Type.
                                _multipartBoundary = EmailMessageParse.MultipartBoundary(_contentType);
                            }
                        }

                        break;

                    case Nequeo.Net.Mail.MesssageHeaderType.EndOfHeader:
                        // The end of all message headers has been
                        // found so the next data is the body of the
                        // message increment the body start index.
                        bodyStart = i + 1;
                        break;
                }

                // If the body index is greater than
                // zero then end of headers has been
                // for and the body of the message
                // has started, break out of the loop.
                if (bodyStart > 0)
                    break;
            }

            // Return the start of the body.
            return bodyStart;
        }

        /// <summary>
        /// Get the header type for the current line.
        /// </summary>
        /// <param name="line">The current header line.</param>
        /// <returns>The current message header type match.</returns>
        private Nequeo.Net.Mail.MesssageHeaderType GetHeaderType(string line)
        {
            // Initialise the message header to
            // an unknown header type.
            Nequeo.Net.Mail.MesssageHeaderType lineType = Nequeo.Net.Mail.MesssageHeaderType.UnKnown;

            // For each message header string.
            for (int i = 0; i < EmailMessageParse.HeaderType.Length; i++)
            {
                // Get the current header string.
                string match = EmailMessageParse.HeaderType[i];

                // Find a header match in the current line.
                if (Regex.Match(line, "^" + match + ":" + ".*$").Success)
                {
                    // A header match has been found for
                    // the header string, return the header type.
                    lineType = (Nequeo.Net.Mail.MesssageHeaderType)i;
                    break;
                }
                else
                    if (line.Length == 0)
                    {
                        // No match has been found, the
                        // end of the header types has
                        // been reached.
                        lineType = Nequeo.Net.Mail.MesssageHeaderType.EndOfHeader;
                        break;
                    }
            }

            // Return the matching header type,
            return lineType;
        }
        #endregion

        #region Override Public Methods
        /// <summary>
        /// Override the ToString method.
        /// </summary>
        /// <returns>The header of the email message only.</returns>
        public override string ToString()
        {
            // The override string includes
            // the header of the message only.
            string ret =
                "From    : " + _from + "\r\n" +
                "To      : " + _to + "\r\n" +
                "Subject : " + _subject + "\r\n";

            // Return the new string.
            return ret;
        }
        #endregion
    }
}
