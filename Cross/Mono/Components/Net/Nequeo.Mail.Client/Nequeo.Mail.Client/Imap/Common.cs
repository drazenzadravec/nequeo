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
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Net.Security;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Threading;

namespace Nequeo.Net.Mail.Imap
{
    /// <summary>
    /// The message class that will contain 
    /// the current email message.
    /// </summary>
    [Serializable]
    [DataContract]
    public class Message : IMessage
    {
        #region Private Fields
        private string _from = null;
        private string _to = null;
        private string _subject = null;
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
        private bool _isMultipart = false;
        private long _messageSize = 0;
        private long _inboxPosition = 0;
        private List<Attachment> _attachments = null;
        private Nequeo.Net.Mail.MessageType _messageType = Nequeo.Net.Mail.MessageType.Text;

        private String _rawEmailMessage = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get set, the complete raw email message.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public String RawEmailMessage
        {
            get { return _rawEmailMessage; }
            set { _rawEmailMessage = value; }
        }

        /// <summary>
        /// Get set, is multipart message, contains attacments.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public bool IsMultipart
        {
            get { return _isMultipart; }
            set { _isMultipart = value; }
        }

        /// <summary>
        /// Get set, the message body type.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Nequeo.Net.Mail.MessageType MessageType
        {
            get { return _messageType; }
            set { _messageType = value; }
        }

        /// <summary>
        /// Get set, the priority of the message.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        /// <summary>
        /// Get set, the importance of the message.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string Importance
        {
            get { return _importance; }
            set { _importance = value; }
        }

        /// <summary>
        /// Get set, carbon copy email address.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string Cc
        {
            get { return _cc; }
            set { _cc = value; }
        }

        /// <summary>
        /// Get set, the blind carbon copy email address.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string Bcc
        {
            get { return _bcc; }
            set { _bcc = value; }
        }

        /// <summary>
        /// Get set, the return path of the email addrees.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string ReturnPath
        {
            get { return _returnPath; }
            set { _returnPath = value; }
        }

        /// <summary>
        /// Get set, the email address of who sent the message.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string From
        {
            get { return _from; }
            set { _from = value; }
        }

        /// <summary>
        /// Get set, the host who sent the message.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string Received
        {
            get { return _received; }
            set { _received = value; }
        }

        /// <summary>
        /// Get set, the email address to whom it was sent.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string To
        {
            get { return _to; }
            set { _to = value; }
        }

        /// <summary>
        /// Get set, the message subject.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        /// <summary>
        /// Get set, the date the message was sent.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string Date
        {
            get { return _date; }
            set { _date = value; }
        }

        /// <summary>
        /// Get set, the organization that sent the message.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string Organization
        {
            get { return _organization; }
            set { _organization = value; }
        }

        /// <summary>
        /// Get set, reply to email address.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string ReplyTo
        {
            get { return _replyTo; }
            set { _replyTo = value; }
        }

        /// <summary>
        /// Get set, the body of the message.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        /// <summary>
        /// Get set, the message position.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public long InboxPosition
        {
            get { return _inboxPosition; }
            set { _inboxPosition = value; }
        }

        /// <summary>
        /// Get set, the message size.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public long Size
        {
            get { return _messageSize; }
            set { _messageSize = value; }
        }

        /// <summary>
        /// Get set, the file attachments.
        /// </summary>
        [DataMember]
        [XmlArray("Attachments", IsNullable = true)]
        public List<Attachment> Attachments
        {
            get { return _attachments; }
            set { _attachments = value; }
        }
        #endregion
    }

    /// <summary>
    /// The attachment class that will contain 
    /// the current email message attachment.
    /// </summary>
    [Serializable]
    [DataContract]
    public class Attachment : IAttachment
    {
        #region Private Fields
        private string _name = null;
        private string _fileName = null;
        private string _fileExtention = null;
        private string _fileNoExtention = null;
        private Byte[] _file = null;
        private bool _decoded = false;
        private string _contentTransferEncoding = null;
        private string _contentType = null;
        private string _contentDescription = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get set, the content type.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        /// <summary>
        /// Get set, the content type.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string ContentDescription
        {
            get { return _contentDescription; }
            set { _contentDescription = value; }
        }

        /// <summary>
        /// Get set, the content transfer encoding type.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string ContentTransferEncoding
        {
            get { return _contentTransferEncoding; }
            set { _contentTransferEncoding = value; }
        }

        /// <summary>
        /// Get set, has the attachment been decoded from
        /// the mine content transfer encoding type.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public bool Decoded
        {
            get { return _decoded; }
            set { _decoded = value; }
        }

        /// <summary>
        /// Get set, the unique name.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Get set, the file name.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        /// <summary>
        /// Get set, the file extention.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string FileExtention
        {
            get { return _fileExtention; }
            set { _fileExtention = value; }
        }

        /// <summary>
        /// Get set, the file name without the extention.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public string FileNoExtention
        {
            get { return _fileNoExtention; }
            set { _fileNoExtention = value; }
        }

        /// <summary>
        /// Get set, the file.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public Byte[] File
        {
            get { return _file; }
            set { _file = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Write the attachment to the file path.
        /// </summary>
        /// <param name="directoryPath">The directory path to write the file.</param>
        /// <returns>True if the attachment was written else false.</returns>
        public bool WriteAttachment(string directoryPath)
        {
            try
            {
                // Write the current attachment
                // to the current file.
                EmailMessageParse.WriteAttachment(_file,
                    directoryPath, _fileName);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Write the attachment to the file path.
        /// </summary>
        /// <param name="directoryPath">The directory path to write the file.</param>
        /// <param name="fileName">The name of the file to write.</param>
        /// <returns>True if the attachment was written else false.</returns>
        public bool WriteAttachment(string directoryPath, string fileName)
        {
            try
            {
                // Write the current attachment
                // to the specified file.
                EmailMessageParse.WriteAttachment(_file,
                    directoryPath, fileName);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }

    /// <summary>
    /// Class that will contain the current state of the
    /// messasge when in an asynchrounus operation.
    /// </summary>
    internal class Imap4StateAdapter : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public Imap4StateAdapter()
        {
            // Create a new wait thread event
            // and set the initial sate to nonsignaled.
            _waitReceiveMessage = new ManualResetEvent(false);

            // Create a new string builder that will contain
            // the current email address.
            _emailMessage = new StringBuilder();
        }
        #endregion

        #region Fields
        /// <summary>
        /// Notifies one or more waiting threads that an
        /// event has occured.
        /// </summary>
        private ManualResetEvent _waitReceiveMessage = null;

        /// <summary>
        /// The current client secure socket layer.
        /// </summary>
        private SslStream _sslStream = null;

        /// <summary>
        /// The current client socket.
        /// </summary>
        private Socket _socket = null;

        /// <summary>
        /// The current client socket.
        /// </summary>
        private TcpClient _client = null;

        /// <summary>
        /// The current email message string builder.
        /// </summary>
        private StringBuilder _emailMessage = null;

        /// <summary>
        /// The current exception that has occured.
        /// </summary>
        private Exception _operationException = null;
        #endregion

        #region Properties
        /// <summary>
        /// Get, the waiting thread upload complete
        /// event signal.
        /// </summary>
        public ManualResetEvent ReceiveComplete
        {
            get { return _waitReceiveMessage; }
        }

        /// <summary>
        /// Get set, the current email message.
        /// </summary>
        public StringBuilder EmailMessage
        {
            get { return _emailMessage; }
            set { _emailMessage = value; }
        }

        /// <summary>
        /// Get set, the current client socket.
        /// </summary>
        public Socket Socket
        {
            get { return _socket; }
            set { _socket = value; }
        }

        /// <summary>
        /// Get set, the current tcp client socket.
        /// </summary>
        public TcpClient Client
        {
            get { return _client; }
            set { _client = value; }
        }

        /// <summary>
        /// Get set, the current secure socket layer client.
        /// </summary>
        public SslStream SslClient
        {
            get { return _sslStream; }
            set { _sslStream = value; }
        }

        /// <summary>
        /// Get set, the current exception that
        /// has occured.
        /// </summary>
        public Exception OperationException
        {
            get { return _operationException; }
            set { _operationException = value; }
        }
        #endregion

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

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

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if(_waitReceiveMessage != null)
                        _waitReceiveMessage.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _waitReceiveMessage = null;

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
        ~Imap4StateAdapter()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// Email message parser class, parses email 
    /// messsage header data.
    /// </summary>
    internal class EmailMessageParse
    {
        #region Public Constants
        public const int MAX_BUFFER_READ_SIZE = 8192;
        #endregion

        #region Private Header Type Fields
        /// <summary>
        /// General email message header types.
        /// </summary>
        private static string[] _headerType =
		{ 
			"From",
			"To",
			"Subject",
			"Content-Type",
            "Reply-To",
            "Date",
            "Organization",
            "Return-Path",
            "Received",
            "Cc",
            "Bcc",
            "X-Priority",
            "Importance"
		};

        /// <summary>
        /// Content mapping email message header types.
        /// </summary>
        private static string[] _contentMappingHeader =
		{
			"Content-Type",
			"Content-Transfer-Encoding",
			"Content-Description",
			"Content-Disposition"
		};

        /// <summary>
        /// Attachment email message header types.
        /// </summary>
        private static string[] _attachmenHeaderType =
		{
			"name",
			"filename"
		};
        #endregion

        #region Public Header Type Properties
        /// <summary>
        /// Get, general email message header types.
        /// </summary>
        public static string[] HeaderType
        {
            get { return _headerType; }
        }

        /// <summary>
        /// Get, content mapping email message header types.
        /// </summary>
        public static string[] ContentMappingHeader
        {
            get { return _contentMappingHeader; }
        }

        /// <summary>
        /// Get, attachment email message header types.
        /// </summary>
        public static string[] AttachmenHeaderType
        {
            get { return _attachmenHeaderType; }
        }
        #endregion

        #region Public Header Parser Methods
        /// <summary>
        /// Parse the from header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static string From(string line)
        {
            // Find the from header in the message.
            // Replace the from header with empty string.
            string fromLine = Regex.Replace(line, "^From: \"(.*)$", "$1");
            return fromLine.Replace("\"", "").Replace("<", "[").Replace(">", "]");
        }

        /// <summary>
        /// Parse the subject header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static string Subject(string line)
        {
            // Find the subject header in the message.
            // Replace the subject header with empty string.
            return Regex.Replace(line, @"^Subject: (.*)$", "$1");
        }

        /// <summary>
        /// Parse the return path header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static string ReturnPath(string line)
        {
            // Find the return path header in the message.
            // Replace the return path header with empty string.
            return Regex.Replace(line, @"^Return-Path: (.*)$", "$1");
        }

        /// <summary>
        /// Parse the received header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static string Received(string line)
        {
            // Find the received header in the message.
            // Replace the received header with empty string.
            return Regex.Replace(line, @"^Received: (.*)$", "$1");
        }

        /// <summary>
        /// Parse the priority header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static string Priority(string line)
        {
            // Find the priority header in the message.
            // Replace the priority header with empty string.
            return Regex.Replace(line, @"^X-Priority: (.*)$", "$1");
        }

        /// <summary>
        /// Parse the importance header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static string Importance(string line)
        {
            // Find the importance header in the message.
            // Replace the importance header with empty string.
            return Regex.Replace(line, @"^Importance: (.*)$", "$1");
        }

        /// <summary>
        /// Parse the reply to header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static string ReplyTo(string line)
        {
            // Find the reply to header in the message.
            // Replace the reply to header with empty string.
            return Regex.Replace(line,
                @"^Reply-To:.*[ |<]([a-z|A-Z|0-9|\.|\-|_]+@[a-z|A-Z|0-9|\.|\-|_]+).*$",
                "$1");
        }

        /// <summary>
        /// Parse the date header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static string Date(string line)
        {
            // Find the date header in the message.
            // Replace the date header with empty string.
            return Regex.Replace(line, @"^Date: (.*)$", "$1");
        }

        /// <summary>
        /// Parse the organization header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static string Organization(string line)
        {
            // Find the organization header in the message.
            // Replace the organization header with empty string.
            return Regex.Replace(line, @"^Organization: (.*)$", "$1");
        }

        /// <summary>
        /// Parse the to header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static string To(string line)
        {
            // Find the to header in the message.
            // Replace the to header with empty string.
            return Regex.Replace(line,
                @"^To:.*[ |<]([a-z|A-Z|0-9|\.|\-|_]+@[a-z|A-Z|0-9|\.|\-|_]+).*$",
                "$1");
        }

        /// <summary>
        /// Parse the cc header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static string Cc(string line)
        {
            // Find the cc header in the message.
            // Replace the cc header with empty string.
            return Regex.Replace(line,
                @"^Cc:.*[ |<]([a-z|A-Z|0-9|\.|\-|_]+@[a-z|A-Z|0-9|\.|\-|_]+).*$",
                "$1");
        }

        /// <summary>
        /// Parse the bcc header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static string Bcc(string line)
        {
            // Find the bcc header in the message.
            // Replace the bcc header with empty string.
            return Regex.Replace(line,
                @"^Bcc:.*[ |<]([a-z|A-Z|0-9|\.|\-|_]+@[a-z|A-Z|0-9|\.|\-|_]+).*$",
                "$1");
        }

        /// <summary>
        /// Parse the content type header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static string ContentType(string line)
        {
            // Find the content type header in the message.
            // Replace the content type header with empty string.
            return Regex.Replace(line, @"^Content-Type: (.*)$", "$1");
        }

        /// <summary>
        /// Parse the content transfer encoding header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static string ContentTransferEncoding(string line)
        {
            // Find the content transfer encoding header in the message.
            // Replace the content transfer encoding header with empty string.
            return Regex.Replace(line,
                @"^Content-Transfer-Encoding: (.*)$",
                "$1");
        }

        /// <summary>
        /// Parse the content description header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static string ContentDescription(string line)
        {
            // Find the content description header in the message.
            // Replace the content description header with empty string.
            return Regex.Replace(line,
                @"^Content-Description: (.*)$",
                "$1");
        }

        /// <summary>
        /// Parse the content disposition header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static string ContentDisposition(string line)
        {
            // Find the content disposition header in the message.
            // Replace the content disposition header with empty string.
            return Regex.Replace(line,
                @"^Content-Disposition: (.*)$",
                "$1");
        }

        /// <summary>
        /// Parse the multipart header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static bool IsMultipart(string line)
        {
            // Find the multipart header in the message.
            // Replace the multipart header with empty string.
            return Regex.Match(line, "^multipart/.*").Success;
        }

        /// <summary>
        /// Parse the boundary header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static string MultipartBoundary(string line)
        {
            // Find the boundary header in the message.
            // Replace the boundary header with empty string.
            return Regex.Replace(line,
                "^.*boundary=[\"]*([^\"]*).*$",
                "$1");
        }

        /// <summary>
        /// Parse the attachment name header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static string Name(string line)
        {
            // Find the attachment name header in the message.
            // Replace the attachment name header with empty string.
            return Regex.Replace(line, "^[ |	]+name=[\"]*([^\"]*).*$", "$1");
        }

        /// <summary>
        /// Parse the attachment file name header in the message.
        /// </summary>
        /// <param name="line">The current line in the message.</param>
        /// <returns>The filtered line.</returns>
        public static string Filename(string line)
        {
            // Find the attachment file name header in the message.
            // Replace the attachment file name header with empty string.
            return Regex.Replace(line, "^[ |	]+filename=[\"]*([^\"]*).*$", "$1");
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get the current attachment header type for the current line
        /// </summary>
        /// <param name="line">The current message line.</param>
        /// <returns>The attachment header type.</returns>
        public static Nequeo.Net.Mail.MesssageAttachmentType GetAttachmenHeaderType(string line)
        {
            // Set the initial attachment header typ the unknown.
            Nequeo.Net.Mail.MesssageAttachmentType lineType = Nequeo.Net.Mail.MesssageAttachmentType.UnKnown;

            // For each attachment header type string.
            for (int i = 0; i < EmailMessageParse.AttachmenHeaderType.Length; i++)
            {
                // Get the current attachment header type string.
                string match = EmailMessageParse.AttachmenHeaderType[i];

                // Find a match in the current message line
                // with the current attachment haeder type.
                if (Regex.Match(line, "^[ |	]+" + match + "=" + ".*$").Success)
                {
                    // Cast the matching attachment header type
                    // with the current array of attachment header type.
                    // Break out of the loop.
                    lineType = (Nequeo.Net.Mail.MesssageAttachmentType)i;
                    break;
                }

                // If the current line length is zero.
                if (line.Length == 0)
                {
                    // Set the attachment header type to end
                    // of header and break out of the loop.
                    lineType = Nequeo.Net.Mail.MesssageAttachmentType.EndOfHeader;
                    break;
                }
            }

            // Return the attachment header type
            // that was found.
            return lineType;
        }

        /// <summary>
        /// Get the current content mapping header type for the current line
        /// </summary>
        /// <param name="line">The current message line.</param>
        /// <param name="boundary">The boundary unique string for the current conten.</param>
        /// <returns>The content mapping header type.</returns>
        public static Nequeo.Net.Mail.MesssageContentMappingType GetContentMappingHeaderType(string line,
            string boundary)
        {
            // Set the initial content mapping header typ the unknown.
            Nequeo.Net.Mail.MesssageContentMappingType lineType = Nequeo.Net.Mail.MesssageContentMappingType.UnKnown;

            // For each content mapping header type string.
            for (int i = 0; i < EmailMessageParse.ContentMappingHeader.Length; i++)
            {
                // Get the current content mapping header type string.
                string match = EmailMessageParse.ContentMappingHeader[i];

                // Find a match in the current message line
                // with the current content mapping haeder type.
                if (Regex.Match(line, "^" + match + ":" + ".*$").Success)
                {
                    // Cast the matching content mapping header type
                    // with the current array of content mapping header type.
                    // Break out of the loop.
                    lineType = (Nequeo.Net.Mail.MesssageContentMappingType)i;
                    break;
                }
                else
                {
                    // If the current line is a start boundary unique string
                    // for body or attachment content mapping header type.
                    if (line.Equals("--" + boundary))
                    {
                        // Set the current content mapping header type
                        // to start of multipart boundary found, indicating that the
                        // message is a multipart message with a starting
                        // boundary unique string. Break out of the loop.
                        lineType = Nequeo.Net.Mail.MesssageContentMappingType.MultipartBoundaryFound;
                        break;
                    }
                    else
                    {
                        // If the current line is an end boundary unique string
                        // for body or attachment conrent mapping header type.
                        if (line.Equals("--" + boundary + "--"))
                        {
                            // Set the current content mapping header type
                            // to end of multipart boundary found. all the
                            // body or attachment data has been collected
                            // in a multipart messsage.
                            lineType = Nequeo.Net.Mail.MesssageContentMappingType.ComponentDone;
                            break;
                        }
                    }
                }

                // If the current line length is zero.
                if (line.Length == 0)
                {
                    // Set the content mapping header type to end
                    // of header and break out of the loop.
                    lineType = Nequeo.Net.Mail.MesssageContentMappingType.EndOfHeader;
                    break;
                }
            }

            // Return the content mapping header type
            // that was found.
            return lineType;
        }

        /// <summary>
        /// If the message its Mime Transfer Encoding set to
        /// QuotedPrintable then decode the message data to
        /// a character string format.
        /// </summary>
        /// <param name="data">The data to decode.</param>
        /// <returns>The decoded data.</returns>
        public static string FromQuotedPrintable(string data)
        {
            // The decode data initally set to null.
            // Remove the '=\n' characters fron the string.
            string output = null;
            string input = data.Replace("=\n", "");

            // If the input string contains data
            if (input.Length > 3)
            {
                // Initialise output string.
                output = String.Empty;

                // For each input string found
                // decode the data.
                for (int i = 0; i < input.Length; )
                {
                    // Get the current substring character.
                    string sub = input.Substring(i, 1);

                    // If the sub string is '=' character and
                    // data remains but less than the length.
                    if ((sub.Equals("=")) && ((i + 2) < input.Length))
                    {
                        // Get the sub string of two
                        // characters of data.
                        string hex = input.Substring(i + 1, 2);

                        // If the sub string is a hex type
                        // of data then convert to a character
                        // printerble string.
                        if (Regex.Match(hex.ToUpper(), @"^[A-F|0-9]+[A-F|0-9]+$").Success)
                        {
                            // Convert the hex sub string to
                            // a printerble string.
                            output += System.Text.Encoding.ASCII.GetString(
                                new byte[] { System.Convert.ToByte(hex, 16) });

                            // Increment the count by three, get
                            // the next two hex string data from the
                            // input string.
                            i += 3;
                        }
                        else
                        {
                            // If not hex value then keep
                            // original data and move to
                            // the next character.
                            output += sub;
                            ++i;
                        }
                    }
                    else
                    {
                        // If not hex value then keep
                        // original data and move to
                        // the next character.
                        output += sub;
                        ++i;
                    }
                }
            }
            else
                // No valid data, return
                // the input string.
                output = input;

            // Return the decode data.
            return output.Replace("\n", "\r\n");
        }

        /// <summary>
        /// Write the quoted printable data to the file.
        /// </summary>
        /// <param name="data">The decoded quoted printable data.</param>
        /// <param name="directoryPath">The directory path of the file.</param>
        /// <param name="fileName">The file name to write.</param>
        public static void WriteQuotedPrintable(string data,
            string directoryPath, string fileName)
        {
            // Use a stream writer to write
            // the data.
            StreamWriter streamWriter = null;

            try
            {
                // Get the full file path.
                string filePath = directoryPath + @"\" + fileName;

                // If the directory does not exist
                // then create the directory.
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                // Create a new instance of the stream writer.
                using (streamWriter = File.CreateText(filePath))
                {
                    // Write the data to the file.
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (streamWriter != null)
                    streamWriter.Close();
            }
        }

        /// <summary>
        /// Write the attachment data to the file.
        /// </summary>
        /// <param name="data">The decoded attachment data.</param>
        /// <param name="directoryPath">The directory path of the file.</param>
        /// <param name="fileName">The file name to write.</param>
        public static void WriteAttachment(byte[] data,
            string directoryPath, string fileName)
        {
            // Use a binary writer to write
            // the data.
            BinaryWriter binaryWriter = null;

            try
            {
                // Get the full file path.
                string filePath = directoryPath + @"\" + fileName;

                // If the directory does not exist
                // then create the directory.
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                // Create a new binary writer.
                binaryWriter = new BinaryWriter(
                    new FileStream(filePath, FileMode.Create));

                // Write the data to the file.
                binaryWriter.Write(data);
                binaryWriter.Flush();
                binaryWriter.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (binaryWriter != null)
                    binaryWriter.Close();
            }
        }
        #endregion
    }
}
