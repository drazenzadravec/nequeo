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
using System.Linq;
using System.Text;
using System.Net.Mime;
using System.Text.RegularExpressions;

namespace Nequeo.Net.Mail.Mime
{
    /// <summary>
    /// Mime message parser class.
    /// </summary>
    public class Parser
    {
        #region Private Fields
        private MessageBody _messageBody = null;
        private Nequeo.Threading.FunctionHandler<List<Attachment>, string> _asyncAttachments = null;

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
        private bool _isMultipart = false;
        #endregion

        #region Private Properites
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

        #region Public Methods
        /// <summary>
        /// Get the message content from the mime data.
        /// </summary>
        /// <param name="mimeMessage">The mime data to parse.</param>
        /// <returns>The message parsed mime data.</returns>
        public virtual Message GetEmailMessage(string mimeMessage)
        {
            return GetEmailMessageEx(mimeMessage);
        }

        /// <summary>
        /// Begin get the message content from the mime data.
        /// </summary>
        /// <param name="mimeMessage">The mime data to parse.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginGetEmailMessage(string mimeMessage,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncParserEmailMessage(mimeMessage, this, callback, state);
        }

        /// <summary>
        /// End get the message content from the mime data.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the message was returned else false.</returns>
        public virtual Message EndGetEmailMessage(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return AsyncParserEmailMessage.End(ar);
        }

        /// <summary>
        /// Gets the collection of attachments within the mime data.
        /// </summary>
        /// <param name="mimeMessage">The mime data to parse.</param>
        /// <returns>A collection of attachments.</returns>
        public virtual List<Attachment> GetAttachments(string mimeMessage)
        {
            return GetEmailMessageEx(mimeMessage).Attachments;
        }

        /// <summary>
        /// Begin get the collection of attachments within the mime data.
        /// </summary>
        /// <param name="mimeMessage">The mime data to parse.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginGetAttachments(string mimeMessage,
            AsyncCallback callback, object state)
        {
            if (_asyncAttachments == null)
                _asyncAttachments = new Nequeo.Threading.FunctionHandler<List<Attachment>, string>(GetAttachments);

            return _asyncAttachments.BeginInvoke(mimeMessage, callback, state);
        }

        /// <summary>
        /// End get the collection of attachments within the mime data.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>A collection of attachments.</returns>
        public virtual List<Attachment> EndGetAttachments(IAsyncResult ar)
        {
            if (_asyncAttachments == null)
                throw new System.InvalidOperationException("End of asynchronous" +
                    " operation attempted when one has not yet begun.");

            // Use the AsyncResult to complete that async operation.
            return _asyncAttachments.EndInvoke(ar);
        }

        /// <summary>
        /// Gets the message headers within the mime data.
        /// </summary>
        /// <param name="mimeMessage">The mime data to parse.</param>
        /// <returns>The messsage headers of the mime data.</returns>
        public virtual IMessage GetEmailMessageHeaders(string mimeMessage)
        {
            IMessage message = GetEmailMessageEx(mimeMessage);
            return message;
        }

        /// <summary>
        /// Gets the collection of attachment headers within the mime data.
        /// </summary>
        /// <param name="mimeMessage">The mime data to parse.</param>
        /// <returns>A collection of attachment headers.</returns>
        public virtual List<IAttachment> GetAttachmentHeaders(string mimeMessage)
        {
            // Get all attachments in the message
            // and create a new attachment interface
            // collection.
            List<Attachment> attachments = GetEmailMessageEx(mimeMessage).Attachments;
            List<IAttachment> attach = new List<IAttachment>();

            // For each attachment found
            // add the attachment headers
            // to the interface collection.
            foreach (Attachment attachment in attachments)
                attach.Add(attachment);

            // Return the collection of
            // attachment headers.
            return attach;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Get the message content from the mime data.
        /// </summary>
        /// <param name="mimeMessage">The mime data to parse.</param>
        /// <returns>The message parsed mime data.</returns>
        private Message GetEmailMessageEx(string mimeMessage)
        {
            // Create a new attachment and message class.
            List<Attachment> attachments = new List<Attachment>();
            Message message = new Message();
            message.Attachments = attachments;

            // Parse the mime message.
            ParseEmail(mimeMessage.ToString().Split(new char[] { '\r' }));

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

            // Return the mime message
            // as a complete message.
            return message;
        }

        /// <summary>
        /// Parse the email message through each header required.
        /// </summary>
        /// <param name="lines">The array of email message lines.</param>
        private void ParseEmail(string[] lines)
        {
            try{
                // Includes the number of emails and
                // the size of the message.
                string[] elements = lines[0].Split(new char[] { ' ' });
                _size = long.Parse(elements[1]);}
            catch { }

            // Parse the message headers and find the
            // start of the body.
            long startOfBody = ParseHeader(lines);
            long numberOfLines = lines.Length;

            // Create a new message body.
            _messageBody = new MessageBody(lines, startOfBody, _multipartBoundary,
                _contentType);
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
    }
}
