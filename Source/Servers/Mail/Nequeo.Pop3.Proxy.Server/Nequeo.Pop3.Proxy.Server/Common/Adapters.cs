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
using System.Threading;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;
using System.Xml.Serialization;

namespace Nequeo.Net.Common
{
    /// <summary>
    /// The current server socket state.
    /// </summary>
    internal sealed class ServerSocketState
    {
        #region Private Fields
        /// <summary>
        /// The current network stream.
        /// </summary>
        private NetworkStream _networkStream = null;
        /// <summary>
        /// The current file stream.
        /// </summary>
        private FileStream _fileStream = null;
        /// <summary>
        /// The current secure socket layer stream.
        /// </summary>
        private SslStream _sslStream = null;
        /// <summary>
        /// The current socket stream.
        /// </summary>
        private Socket _socket = null;
        /// <summary>
        /// The current tcp client stream.
        /// </summary>
        private TcpClient _tcpClient = null;
        /// <summary>
        /// Total number of bytes read.
        /// </summary>
        private long _totalBytesRead = 0;
        /// <summary>
        /// The current email message string builder.
        /// </summary>
        private StringBuilder _emailMessage = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get set, the current network stream.
        /// </summary>
        public NetworkStream NetworkStream
        {
            get { return _networkStream; }
            set { _networkStream = value; }
        }

        /// <summary>
        /// Get set, the current file stream.
        /// </summary>
        public FileStream FileStream
        {
            get { return _fileStream; }
            set { _fileStream = value; }
        }

        /// <summary>
        /// Get set, the current secure socket layer stream.
        /// </summary>
        public SslStream SslStream
        {
            get { return _sslStream; }
            set { _sslStream = value; }
        }

        /// <summary>
        /// Get set, the current tcp socket stream.
        /// </summary>
        public TcpClient TcpClient
        {
            get { return _tcpClient; }
            set { _tcpClient = value; }
        }

        /// <summary>
        /// Get set, the current socket stream.
        /// </summary>
        public Socket Socket
        {
            get { return _socket; }
            set { _socket = value; }
        }

        /// <summary>
        /// Get set, the total bytes read.
        /// </summary>
        public long TotalBytesRead
        {
            get { return _totalBytesRead; }
            set { _totalBytesRead = value; }
        }

        /// <summary>
        /// Get set, the current email message.
        /// </summary>
        public StringBuilder EmailMessage
        {
            get { return _emailMessage; }
            set { _emailMessage = value; }
        }
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

        #region Dispose Object Methods.
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
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    userName = string.Empty;
                    password = string.Empty;
                    server = string.Empty;
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.

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
