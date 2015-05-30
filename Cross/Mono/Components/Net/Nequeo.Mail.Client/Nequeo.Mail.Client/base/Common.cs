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
using System.Threading;
using System.Diagnostics;
using System.Net.Security;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Security;

namespace Nequeo.Net.Mail
{
    /// <summary>
    /// Email message interface.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Get set, is multipart message, contains attacments.
        /// </summary>
        bool IsMultipart { get; set; }

        /// <summary>
        /// Get set, the message body type.
        /// </summary>
        Nequeo.Net.Mail.MessageType MessageType { get; set; }

        /// <summary>
        /// Get set, the priority of the message.
        /// </summary>
        string Priority { get; set; }

        /// <summary>
        /// Get set, the importance of the message.
        /// </summary>
        string Importance { get; set; }

        /// <summary>
        /// Get set, carbon copy email address.
        /// </summary>
        string Cc { get; set; }

        /// <summary>
        /// Get set, the blind carbon copy email address.
        /// </summary>
        string Bcc { get; set; }

        /// <summary>
        /// Get set, the return path of the email addrees.
        /// </summary>
        string ReturnPath { get; set; }

        /// <summary>
        /// Get set, the email address of who sent the message.
        /// </summary>
        string From { get; set; }

        /// <summary>
        /// Get set, the host who sent the message.
        /// </summary>
        string Received { get; set; }

        /// <summary>
        /// Get set, the email address to whom it was sent.
        /// </summary>
        string To { get; set; }

        /// <summary>
        /// Get set, the message subject.
        /// </summary>
        string Subject { get; set; }

        /// <summary>
        /// Get set, the date the message was sent.
        /// </summary>
        string Date { get; set; }

        /// <summary>
        /// Get set, the organization that sent the message.
        /// </summary>
        string Organization { get; set; }

        /// <summary>
        /// Get set, reply to email address.
        /// </summary>
        string ReplyTo { get; set; }

        /// <summary>
        /// Get set, the body of the message.
        /// </summary>
        string Body { get; set; }

        /// <summary>
        /// Get set, the message size.
        /// </summary>
        long Size { get; set; }
    }

    /// <summary>
    /// Email message attachment interface.
    /// </summary>
    public interface IAttachment
    {
        /// <summary>
        /// Get set, the content type.
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// Get set, the content type.
        /// </summary>
        string ContentDescription { get; set; }

        /// <summary>
        /// Get set, the content transfer encoding type.
        /// </summary>
        string ContentTransferEncoding { get; set; }

        /// <summary>
        /// Get set, has the attachment been decoded from
        /// the mine content transfer encoding type.
        /// </summary>
        bool Decoded { get; set; }

        /// <summary>
        /// Get set, the unique name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Get set, the file name.
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// Get set, the file extention.
        /// </summary>
        string FileExtention { get; set; }

        /// <summary>
        /// Get set, the file name without the extention.
        /// </summary>
        string FileNoExtention { get; set; }
    }

    /// <summary>
    /// Email message types.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// Text email message.
        /// </summary>
        Text = 0,
        /// <summary>
        /// Html email message.
        /// </summary>
        Html = 1,
        /// <summary>
        /// Rich text email message.
        /// </summary>
        RichText = 2,
        /// <summary>
        /// Text and html.
        /// </summary>
        TextAndHtml = 3,
        /// <summary>
        /// Text and rich text.
        /// </summary>
        TextAndRichText = 4,
        /// <summary>
        /// Html and rich text.
        /// </summary>
        HtmlAndRichText = 5,
        /// <summary>
        /// Xml email message.
        /// </summary>
        Xml = 6,
        /// <summary>
        /// All types.
        /// </summary>
        All = 7
    }

    /// <summary>
    /// Email message common header types.
    /// </summary>
    public enum MesssageHeaderType
    {
        /// <summary>
        /// Unknown header.
        /// </summary>
        UnKnown = -99,
        /// <summary>
        /// End of header.
        /// </summary>
        EndOfHeader = -98,
        /// <summary>
        /// Multipart message boundary.
        /// </summary>
        MultipartBoundaryFound = -97,
        /// <summary>
        /// Component boundary begin end.
        /// </summary>
        ComponentDone = -96,
        /// <summary>
        /// From header.
        /// </summary>
        From = 0,
        /// <summary>
        /// To header.
        /// </summary>
        To = 1,
        /// <summary>
        /// Subject header.
        /// </summary>
        Subject = 2,
        /// <summary>
        /// Content type header.
        /// </summary>
        ContentType = 3,
        /// <summary>
        /// Reply to header.
        /// </summary>
        ReplyTo = 4,
        /// <summary>
        /// Date header.
        /// </summary>
        Date = 5,
        /// <summary>
        /// Organization header.
        /// </summary>
        Organization = 6,
        /// <summary>
        /// Return path header.
        /// </summary>
        ReturnPath = 7,
        /// <summary>
        /// Received data header.
        /// </summary>
        Received = 8,
        /// <summary>
        /// Carbon copy header.
        /// </summary>
        Cc = 9,
        /// <summary>
        /// Blind carbon copy header
        /// </summary>
        Bcc = 10,
        /// <summary>
        /// Priority header.
        /// </summary>
        Priority = 11,
        /// <summary>
        /// Importance header.
        /// </summary>
        Importance = 12
    }

    /// <summary>
    /// Email message content mapping header types.
    /// </summary>
    public enum MesssageContentMappingType
    {
        /// <summary>
        /// Unknown header.
        /// </summary>
        UnKnown = -99,
        /// <summary>
        /// End of header.
        /// </summary>
        EndOfHeader = -98,
        /// <summary>
        /// Multipart message boundary.
        /// </summary>
        MultipartBoundaryFound = -97,
        /// <summary>
        /// Component boundary begin end.
        /// </summary>
        ComponentDone = -96,
        /// <summary>
        /// Common content type.
        /// </summary>
        ContentType = 0,
        /// <summary>
        /// Content transfer encoding.
        /// </summary>
        ContentTransferEncoding = 1,
        /// <summary>
        /// Content description.
        /// </summary>
        ContentDescription = 2,
        /// <summary>
        /// Content disposition.
        /// </summary>
        ContentDisposition = 3
    }

    /// <summary>
    /// Email message attachment header types.
    /// </summary>
    public enum MesssageAttachmentType
    {
        /// <summary>
        /// Unknown header.
        /// </summary>
        UnKnown = -99,
        /// <summary>
        /// End of header.
        /// </summary>
        EndOfHeader = -98,
        /// <summary>
        /// Multipart message boundary.
        /// </summary>
        MultipartBoundaryFound = -97,
        /// <summary>
        /// Component boundary begin end.
        /// </summary>
        ComponentDone = -96,
        /// <summary>
        /// Unique name of the attachment.
        /// </summary>
        Name = 0,
        /// <summary>
        /// The file name of the attachment.
        /// </summary>
        Filename = 1
    }

    /// <summary>
    /// Command argument class containing event handler
    /// information for the server command delegate.
    /// </summary>
    public class ClientCommandArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the command event argument.
        /// </summary>
        /// <param name="command">The command that is received from the server.</param>
        /// <param name="data">The data that is received from the server.</param>
        /// <param name="code">The code that is received from the server.</param>
        public ClientCommandArgs(string command, string data, long code)
        {
            this.command = command;
            this.data = data;
            this.code = code;
        }
        #endregion

        #region Private Fields
        // The command that is received from the server.
        private string command = string.Empty;
        // The data that is received from the server.
        private string data = string.Empty;
        // The code that is received from the server.
        private long code = 0;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the command that is received from the server.
        /// </summary>
        public string Command
        {
            get { return command; }
        }

        /// <summary>
        /// Contains the data that is received from the server.
        /// </summary>
        public string Data
        {
            get { return data; }
        }

        /// <summary>
        /// Contains the code that is received from the server.
        /// </summary>
        public long Code
        {
            get { return code; }
        }
        #endregion
    }

    /// <summary>
    /// Operation argument class containing event handler
    /// information for the server operation delegate.
    /// </summary>
    public class ClientOperationArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the operation event argument.
        /// </summary>
        /// <param name="localFile">The local file name.</param>
        /// <param name="remoteFile">The remote file name.</param>
        /// <param name="fileSize">The file size.</param>
        public ClientOperationArgs(string localFile, string remoteFile, long fileSize)
        {
            this.localFile = localFile;
            this.remoteFile = remoteFile;
            this.fileSize = fileSize;
        }
        #endregion

        #region Private Fields
        // The local file name.
        private string localFile = string.Empty;
        // The remote file name.
        private string remoteFile = string.Empty;
        // The file size.
        private long fileSize = 0;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the local file name.
        /// </summary>
        public string LocalFile
        {
            get { return localFile; }
        }

        /// <summary>
        /// Contains the remote file name.
        /// </summary>
        public string RemoteFile
        {
            get { return remoteFile; }
        }

        /// <summary>
        /// Contains the file size.
        /// </summary>
        public long FileSize
        {
            get { return fileSize; }
        }
        #endregion
    }

    /// <summary>
    /// Progress argument class containing event handler
    /// information for the server progress delegate.
    /// </summary>
    public class ClientProgressArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the progress event argument.
        /// </summary>
        /// <param name="size">The current number of 
        /// bytes sent or received.</param>
        public ClientProgressArgs(long size)
        {
            this.size = size;
        }
        #endregion

        #region Private Fields
        // The current number of bytes 
        // sent or received.
        private long size = 0;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains The current number of bytes 
        /// sent or received.
        /// </summary>
        public long Size
        {
            get { return size; }
        }
        #endregion
    }

    /// <summary>
    /// Thread error argument class containing event handler
    /// information for the server thread error delegate.
    /// </summary>
    public class ClientThreadErrorArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the thread error event argument.
        /// </summary>
        /// <param name="data">The current thread error.</param>
        /// <param name="code">The code that is received from the server.</param>
        public ClientThreadErrorArgs(string data, long code)
        {
            this.data = data;
            this.code = code;
        }
        #endregion

        #region Private Fields
        // current thread error.
        private string data = string.Empty;
        // The code that is received from the server.
        private long code = 0;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the current thread error. 
        /// sent or received.
        /// </summary>
        public string Data
        {
            get { return data; }
        }

        /// <summary>
        /// Contains the code that is received from the server.
        /// </summary>
        public long Code
        {
            get { return code; }
        }
        #endregion
    }
}
