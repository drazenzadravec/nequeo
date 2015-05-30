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
using System.Net;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Cryptography.X509Certificates;

namespace Nequeo.Net.Ftp
{
    /// <summary>
    /// This class contains methods that control the
    /// client access to an Ftp server through uploading
    /// or downloading files from the Ftp connection
    /// adapter information.
    /// </summary>
    public class FTPClientConnection : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public FTPClientConnection()
        {
            ftpClient = new FTPSocket();
            ftpAsycClient = new AsynchronousFTPSocket();
        }

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="callbackHandler">Processing callback delegate.</param>
        public FTPClientConnection(FTPWebSocketCallbackHandler callbackHandler)
        {
            ftpClient = new FTPSocket(callbackHandler);
            ftpAsycClient = new AsynchronousFTPSocket(callbackHandler);
        }

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete 
        /// connection information.</param>
        public FTPClientConnection(FTPConnectionAdapter ftpAdapter)
        {
            this.ftpAdapter = ftpAdapter;
            ftpClient = new FTPSocket();
            ftpAsycClient = new AsynchronousFTPSocket();
        }

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete 
        /// connection information.</param>
        /// <param name="callbackHandler">Processing callback delegate.</param>
        public FTPClientConnection(FTPConnectionAdapter ftpAdapter,
            FTPWebSocketCallbackHandler callbackHandler)
        {
            this.ftpAdapter = ftpAdapter;
            ftpClient = new FTPSocket(callbackHandler);
            ftpAsycClient = new AsynchronousFTPSocket(callbackHandler);
        }
        #endregion

        #region Private Fields
        private FTPSocket ftpClient = null;
        private AsynchronousFTPSocket ftpAsycClient = null;
        private FTPConnectionAdapter ftpAdapter = null;
        private string uploadTarget = string.Empty;
        private string downloadDestination = string.Empty;
        private string ftpUploadSourcePath = string.Empty;
        private string ftpDownloadSourcePath = string.Empty;
        private string ftpDeleteFileSourcePath = string.Empty;
        private string ftpDirectoryListPath = string.Empty;
        private List<string> directoryList = null;
        private bool asynchronous = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets the ftp adapter for connecting to the server.
        /// </summary>
        public FTPConnectionAdapter FTPAdapter
        {
            get { return ftpAdapter; }
            set { ftpAdapter = value; }
        }

        /// <summary>
        /// Gets sets the ftp delete file path on the ftp server.
        /// </summary>
        public string FtpDeleteFileSourcePath
        {
            get { return ftpDeleteFileSourcePath; }
            set { ftpDeleteFileSourcePath = value; }
        }

        /// <summary>
        /// Gets sets the upload target file on the local machine.
        /// </summary>
        public string UploadTarget
        {
            get { return uploadTarget; }
            set { uploadTarget = value; }
        }

        /// <summary>
        /// Gets sets the download destination file on the local machine.
        /// </summary>
        public string DownloadDestination
        {
            get { return downloadDestination; }
            set { downloadDestination = value; }
        }

        /// <summary>
        /// Gets sets the ftp upload path on the ftp server.
        /// </summary>
        public string FTPUploadSourcePath
        {
            get { return ftpUploadSourcePath; }
            set { ftpUploadSourcePath = value; }
        }

        /// <summary>
        /// Gets sets the ftp download path on the ftp server.
        /// </summary>
        public string FTPDownloadSourcePath
        {
            get { return ftpDownloadSourcePath; }
            set { ftpDownloadSourcePath = value; }
        }

        /// <summary>
        /// Gets sets the ftp directory list path on the ftp server.
        /// </summary>
        public string FTPDirectoryListPath
        {
            get { return ftpDirectoryListPath; }
            set { ftpDirectoryListPath = value; }
        }

        /// <summary>
        /// Gets the file list.
        /// </summary>
        public List<string> DirectoryList
        {
            get { return directoryList; }
        }

        /// <summary>
        /// Gets sets, should asynchronous ftp operations be used.
        /// </summary>
        public bool Asynchronous
        {
            get { return asynchronous; }
            set { asynchronous = value; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// The Ftp validation operation to perform.
        /// </summary>
        /// <param name="socketDirection">Socket operation to perform.</param>
        private void ValidateProperties(Nequeo.Net.Ftp.SocketTransferDirection socketDirection)
        {
            // Make sure that the ftp adapter exists.
            if (this.ftpAdapter == null)
                throw new FTPClientConnectionException(
                    new NullReferenceException("The Ftp Connection " +
                        "Adapter has not been set."));

            switch (socketDirection)
            {
                case Nequeo.Net.Ftp.SocketTransferDirection.Upload:
                    // Make sure that an upload target exists.
                    if (this.uploadTarget == string.Empty)
                        throw new FTPClientConnectionException("The local upload " +
                            "target path has not been set.");

                    // Make sure that an upload source exists.
                    if (this.ftpUploadSourcePath == string.Empty)
                        throw new FTPClientConnectionException("The ftp server upload " +
                            "path has not been set.");
                    break;

                case Nequeo.Net.Ftp.SocketTransferDirection.Download:
                    // Make sure that a download destination exists.
                    if (this.downloadDestination == string.Empty)
                        throw new FTPClientConnectionException("The local download " +
                            "destination path has not been set.");

                    // Make sure that a download source exists.
                    if (this.ftpDownloadSourcePath == string.Empty)
                        throw new FTPClientConnectionException("The ftp server download " +
                            "path has not been set.");
                    break;

                case Nequeo.Net.Ftp.SocketTransferDirection.DeleteFile:
                    // Make sure that the delete file exists.
                    if (this.ftpDeleteFileSourcePath == string.Empty)
                        throw new FTPClientConnectionException("The ftp server delete file " +
                            "path has not been set.");
                    break;

                case Nequeo.Net.Ftp.SocketTransferDirection.DirectoryList:
                    // Make sure that a download directory list exists.
                    if (this.ftpDirectoryListPath == string.Empty)
                        throw new FTPClientConnectionException("The ftp server directory list " +
                            "path has not been set.");
                    break;
            }
        }

        /// <summary>
        /// The ftp operation to perform.
        /// </summary>
        /// <param name="socketDirection">Socket operation to perform.</param>
        /// <returns>True if the ftp operation succeed else false.</returns>
        private bool FtpOperation(Nequeo.Net.Ftp.SocketTransferDirection socketDirection)
        {
            // Set the initial return to false.
            bool ret = false;

            // Validate the properties before any
            // Ftp operation.
            ValidateProperties(socketDirection);

            switch (socketDirection)
            {
                case Nequeo.Net.Ftp.SocketTransferDirection.Upload:
                    // Start uploading the local target file to the
                    // ftp server source path file.
                    ret = ftpClient.Upload(this.ftpAdapter, this.uploadTarget,
                        new Uri("ftp://" + this.FTPAdapter.FTPServer + (this.FTPAdapter.Port != 21 ? ":" + this.FTPAdapter.Port.ToString() : "") + "/" +
                            this.ftpUploadSourcePath));
                    break;

                case Nequeo.Net.Ftp.SocketTransferDirection.Download:
                    // Start downloading the ftp server source path file
                    // to the local destination path.
                    ret = ftpClient.Download(this.ftpAdapter, this.downloadDestination,
                        new Uri("ftp://" + this.FTPAdapter.FTPServer + (this.FTPAdapter.Port != 21 ? ":" + this.FTPAdapter.Port.ToString() : "") + "/" +
                            this.ftpDownloadSourcePath));
                    break;

                case Nequeo.Net.Ftp.SocketTransferDirection.DeleteFile:
                    // Start delete file the ftp server source path file
                    // to the local destination path.
                    ret = ftpClient.DeleteFile(this.ftpAdapter,
                        new Uri("ftp://" + this.FTPAdapter.FTPServer + (this.FTPAdapter.Port != 21 ? ":" + this.FTPAdapter.Port.ToString() : "") + "/" +
                            this.FtpDeleteFileSourcePath));
                    break;

                case Nequeo.Net.Ftp.SocketTransferDirection.DirectoryList:
                    // List of all the files in the directory.
                    List<string> fileList = null;

                    // Start downloading the directory list from the
                    // ftp server source path.
                    ret = ftpClient.DirectoryList(this.ftpAdapter,
                        new Uri("ftp://" + this.FTPAdapter.FTPServer + (this.FTPAdapter.Port != 21 ? ":" + this.FTPAdapter.Port.ToString() : "") + "/" +
                            this.ftpDirectoryListPath), out fileList);

                    // Assign the file list to the
                    // property value, returns the
                    // list of files.
                    directoryList = fileList;
                    break;
            }

            // Return true if the process
            // completed successfully.
            return ret;
        }

        /// <summary>
        /// The asynchronous ftp operation to perform.
        /// </summary>
        /// <param name="socketDirection">Socket operation to perform.</param>
        /// <returns>True if the ftp operation succeed else false.</returns>
        private bool AsynchronousFtpOperation(Nequeo.Net.Ftp.SocketTransferDirection socketDirection)
        {
            // Set the initial return to false.
            bool ret = false;

            // Validate the properties before any
            // Ftp operation.
            ValidateProperties(socketDirection);

            switch (socketDirection)
            {
                case Nequeo.Net.Ftp.SocketTransferDirection.Upload:
                    // Start uploading the local target file to the
                    // ftp server source path file.
                    ret = ftpAsycClient.Upload(this.ftpAdapter, this.uploadTarget,
                        new Uri("ftp://" + this.FTPAdapter.FTPServer + (this.FTPAdapter.Port != 21 ? ":" + this.FTPAdapter.Port.ToString() : "") + "/" +
                            this.ftpUploadSourcePath));
                    break;

                case Nequeo.Net.Ftp.SocketTransferDirection.Download:
                    // Start downloading the ftp server source path file
                    // to the local destination path.
                    ret = ftpAsycClient.Download(this.ftpAdapter, this.downloadDestination,
                        new Uri("ftp://" + this.FTPAdapter.FTPServer + (this.FTPAdapter.Port != 21 ? ":" + this.FTPAdapter.Port.ToString() : "") + "/" +
                            this.ftpDownloadSourcePath));
                    break;
            }

            // Return true if the process
            // completed successfully.
            return ret;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start uploading the file to the Ftp server.
        /// </summary>
        /// <returns>True if the upload was successful else false.</returns>
        public virtual bool StartUpload()
        {
            if (!asynchronous)
                return FtpOperation(Nequeo.Net.Ftp.SocketTransferDirection.Upload);
            else
                return AsynchronousFtpOperation(Nequeo.Net.Ftp.SocketTransferDirection.Upload);
        }

        /// <summary>
        /// Start downloading the file from the Ftp server.
        /// </summary>
        /// <returns>True if the download was successful else false.</returns>
        public virtual bool StartDownload()
        {
            if (!asynchronous)
                return FtpOperation(Nequeo.Net.Ftp.SocketTransferDirection.Download);
            else
                return AsynchronousFtpOperation(Nequeo.Net.Ftp.SocketTransferDirection.Download);
        }

        /// <summary>
        /// Start deleting the file from the Ftp server.
        /// </summary>
        /// <returns>True if the file was deleted was successful else false.</returns>
        public virtual bool StartDeleteFile()
        {
            if (!asynchronous)
                return FtpOperation(Nequeo.Net.Ftp.SocketTransferDirection.DeleteFile);
            else
                return AsynchronousFtpOperation(Nequeo.Net.Ftp.SocketTransferDirection.DeleteFile);
        }

        /// <summary>
        /// Start directory list download from the Ftp server.
        /// </summary>
        /// <returns>True if the directory list download was successful else false.</returns>
        public virtual bool StartDirectoryList()
        {
            return FtpOperation(Nequeo.Net.Ftp.SocketTransferDirection.DirectoryList);
        }

        /// <summary>
        /// Start uploading the file to the Ftp server.
        /// </summary>
        /// <returns>True if the upload was successful else false.</returns>
        public virtual void StartUploadThread()
        {
            if (!asynchronous)
                FtpOperation(Nequeo.Net.Ftp.SocketTransferDirection.Upload);
            else
                AsynchronousFtpOperation(Nequeo.Net.Ftp.SocketTransferDirection.Upload);
        }

        /// <summary>
        /// Start downloading the file from the Ftp server.
        /// </summary>
        /// <returns>True if the download was successful else false.</returns>
        public virtual void StartDownloadThread()
        {
            if (!asynchronous)
                FtpOperation(Nequeo.Net.Ftp.SocketTransferDirection.Download);
            else
                AsynchronousFtpOperation(Nequeo.Net.Ftp.SocketTransferDirection.Download);
        }

        /// <summary>
        /// Start directory list download from the Ftp server.
        /// </summary>
        /// <returns>True if the directory list download was successful else false.</returns>
        public virtual void StartDirectoryListThread()
        {
            FtpOperation(Nequeo.Net.Ftp.SocketTransferDirection.DirectoryList);
        }
        #endregion

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool disposed = false;

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
                    if(ftpClient != null)
                        ftpClient.Dispose();

                    if (ftpAsycClient != null)
                        ftpAsycClient.Dispose();

                    if (ftpAdapter != null)
                        ftpAdapter.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                ftpAdapter = null;
                ftpClient = null;
                ftpAsycClient = null;

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
        ~FTPClientConnection()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// Class contains properties that hold all the
    /// connection collection for the specified FTP.
    /// </summary>
    [Serializable]
    public class FTPConnectionAdapter : IFTPConnectionAdapter, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public FTPConnectionAdapter()
        {
        }

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="ftpServer">The FTP server.</param>
        /// <param name="userName">The username for the connection.</param>
        /// <param name="password">The password for the connection.</param>
        /// <param name="isAnonymousUser">Is the connection to the FTP server anonymous.</param>
        /// <param name="usePassive">The behavior of a client application's data transfer process.</param>
        /// <param name="useBinary">The value that specifies the data type for file transfers.</param>
        /// <param name="timeOut">The number of milliseconds to wait for a request.</param>
        /// <param name="useSSLConnection">Should the connection use ssl encryption.</param>
        public FTPConnectionAdapter(string ftpServer, string userName,
            string password, bool isAnonymousUser = false, bool usePassive = false,
            bool useBinary = true, int timeOut = -1, bool useSSLConnection = false)
        {
            this.ftpServer = ftpServer;
            this.userName = userName;
            this.password = password;
            this.isAnonymousUser = isAnonymousUser;
            this.usePassive = usePassive;
            this.useBinary = useBinary;
            this.timeOut = timeOut;
            this.useSSLConnection = useSSLConnection;
        }
        #endregion

        #region Private Fields
        private bool isAnonymousUser = false;
        private string userName = string.Empty;
        private string password = string.Empty;
        private string ftpServer = string.Empty;
        private string domain = string.Empty;
        private bool usePassive = false;
        private bool useBinary = true;
        private bool useSSLConnection = false;
        private bool sslCertificateOverride = true;
        private int timeOut = -1;
        private int port = 21;
        private bool disposed = false;
        private IWebProxy _proxy = null;
        private X509Certificate2 _clientCertificate = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get set, the client certificate.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public X509Certificate2 ClientCertificate
        {
            get { return _clientCertificate; }
            set { _clientCertificate = value; }
        }

        /// <summary>
        /// Get set, the web client proxy.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public IWebProxy Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        /// <summary>
        /// Get Set, the anonymous user indicator.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public bool IsAnonymousUser
        {
            get { return isAnonymousUser; }
            set { isAnonymousUser = value; }
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
        /// Get Set, the ftp server.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string FTPServer
        {
            get { return ftpServer; }
            set { ftpServer = value; }
        }

        /// <summary>
        /// Get Set, the ftp port.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        /// <summary>
        /// Get Set, use passive mode.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public bool UsePassive
        {
            get { return usePassive; }
            set { usePassive = value; }
        }

        /// <summary>
        /// Get Set, use binay transfer.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public bool UseBinary
        {
            get { return useBinary; }
            set { useBinary = value; }
        }

        /// <summary>
        /// Get Set, the time out request.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public int TimeOut
        {
            get { return timeOut; }
            set { timeOut = value; }
        }

        /// <summary>
        /// Get Set, use ssl encryption transfer.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public bool UseSSLConnection
        {
            get { return useSSLConnection; }
            set { useSSLConnection = value; }
        }

        /// <summary>
        /// Get Set, override server certificate validation to
        /// always true no matter if the certificate is not valid.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public bool SslCertificateOverride
        {
            get { return sslCertificateOverride; }
            set { sslCertificateOverride = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Load the client certificate.
        /// </summary>
        /// <param name="fileName">The path to the certificate.</param>
        public void LoadClientCertificate(string fileName)
        {
            _clientCertificate = (X509Certificate2)X509Certificate2.CreateFromCertFile(fileName);
        }

        /// <summary>
        /// Load the client certificate.
        /// </summary>
        /// <param name="storeName">The store name to search in.</param>
        /// <param name="storeLocation">The store location to search in.</param>
        /// <param name="x509FindType">The type of data to find on.</param>
        /// <param name="findValue">The value to find in the certificate.</param>
        /// <param name="validOnly">Search for only valid certificates.</param>
        public void LoadClientCertificate(
            StoreName storeName,
            StoreLocation storeLocation,
            X509FindType x509FindType,
            object findValue,
            bool validOnly)
        {
            _clientCertificate = Nequeo.Security.X509Certificate2Store.GetCertificate(
                storeName, storeLocation, x509FindType, findValue, validOnly);
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
                ftpServer = null;

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
        ~FTPConnectionAdapter()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// Class contains properties that hold all the
    /// transmit collection for the specified FTP.
    /// </summary>
    [Serializable]
    public class FTPTransmitAdapter : IFTPTransmitAdapter, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Constructor for the class.
        /// </summary>
        /// <param name="uploadPath">The upload path.</param>
        /// <param name="downloadPath">The download path.</param>
        public FTPTransmitAdapter(string uploadPath, string downloadPath)
        {
            this.uploadPath = uploadPath;
            this.downloadPath = downloadPath;
        }
        #endregion

        #region Private Fields
        private bool disposed = false;
        private string uploadPath = string.Empty;
        private string downloadPath = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get Set, the upload ftp transmit path.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string UploadPath
        {
            get { return uploadPath; }
            set { uploadPath = value; }
        }

        /// <summary>
        /// Get Set, the download ftp transmit path.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string DownloadPath
        {
            get { return downloadPath; }
            set { downloadPath = value; }
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
        ~FTPTransmitAdapter()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// This class will contain the ftp connection and ftp transmit
    /// adapter integration data.
    /// </summary>
    [Serializable]
    public class FTPIntegrationAdapter
    {
        #region Constructors
        /// <summary>
        /// Constructor for the class.
        /// </summary>
        /// <param name="ftpAdapter">The current ftp connection adapter.</param>
        /// <param name="ftpTransmit">The current ftp transmit adapter.</param>
        public FTPIntegrationAdapter(FTPConnectionAdapter ftpAdapter,
            FTPTransmitAdapter ftpTransmit)
        {
            this.ftpAdapter = ftpAdapter;
            this.ftpTransmit = ftpTransmit;
        }
        #endregion

        #region Private Fields
        private bool disposed = false;
        private FTPConnectionAdapter ftpAdapter = null;
        private FTPTransmitAdapter ftpTransmit = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get Set, the current ftp connection adapter.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public FTPConnectionAdapter FTPConnectionAdapter
        {
            get { return ftpAdapter; }
            set { ftpAdapter = value; }
        }

        /// <summary>
        /// Get Set, the current ftp transmit adapter.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public FTPTransmitAdapter FTPTransmitAdapter
        {
            get { return ftpTransmit; }
            set { ftpTransmit = value; }
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
                    this.ftpAdapter = null;
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
        ~FTPIntegrationAdapter()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// Class contains properties that hold all the
    /// asynchronous thread state ftp information.
    /// </summary>
    internal class FTPStateAdapter : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public FTPStateAdapter()
        {
            // Create a new wait thread event
            // and set the initial sate to nonsignaled.
            waitUpload = new ManualResetEvent(false);
            waitDownload = new ManualResetEvent(false);
        }
        #endregion

        #region Private Fields
        /// <summary>
        /// Notifies one or more waiting threads that an
        /// event has occured.
        /// </summary>
        private ManualResetEvent waitUpload = null;
        /// <summary>
        /// Notifies one or more waiting threads that an
        /// event has occured.
        /// </summary>
        private ManualResetEvent waitDownload = null;
        /// <summary>
        /// The ftp request object.
        /// </summary>
        private FtpWebRequest ftpRequest = null;
        /// <summary>
        /// The ftp response object.
        /// </summary>
        private FtpWebResponse ftpResponse = null;
        /// <summary>
        /// The full path and file name of the file to upload.
        /// </summary>
        private string targetFile = string.Empty;
        /// <summary>
        /// The full path and file name where data should be written to.
        /// </summary>
        private string destinationFile = string.Empty;
        /// <summary>
        /// The current exception that has occured.
        /// </summary>
        private Exception operationException = null;
        /// <summary>
        /// The most recent ftp status code from the server.
        /// </summary>
        private FtpStatusCode ftpStatusCode = FtpStatusCode.Undefined;
        /// <summary>
        /// The most recent ftp status description from the server.
        /// </summary>
        private string ftpStatusDescription = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get, the waiting thread upload complete
        /// event signal.
        /// </summary>
        public ManualResetEvent UploadComplete
        {
            get { return waitUpload; }
        }

        /// <summary>
        /// Get, the waiting thread download complete
        /// event signal.
        /// </summary>
        public ManualResetEvent DownloadComplete
        {
            get { return waitDownload; }
        }

        /// <summary>
        /// Get set, the current ftp request object.
        /// </summary>
        public FtpWebRequest FtpRequest
        {
            get { return ftpRequest; }
            set { ftpRequest = value; }
        }

        /// <summary>
        /// Get set, the current ftp response object.
        /// </summary>
        public FtpWebResponse FtpResponse
        {
            get { return ftpResponse; }
            set { ftpResponse = value; }
        }

        /// <summary>
        /// Get set, the target file on the local
        /// machine to upload.
        /// </summary>
        public string TargetFile
        {
            get { return targetFile; }
            set { targetFile = value; }
        }

        /// <summary>
        /// Get set, the destination file on the local
        /// machine to download to.
        /// </summary>
        public string DestinationFile
        {
            get { return destinationFile; }
            set { destinationFile = value; }
        }

        /// <summary>
        /// Get set, the current exception that
        /// has occured.
        /// </summary>
        public Exception OperationException
        {
            get { return operationException; }
            set { operationException = value; }
        }

        /// <summary>
        /// Get, the most recent ftp status code from the server.
        /// </summary>
        public FtpStatusCode FtpStatusCode
        {
            get { return ftpStatusCode; }
            set { ftpStatusCode = value; }
        }

        /// <summary>
        /// Get, the most recent ftp status description from the server.
        /// </summary>
        public string FtpStatusDescription
        {
            get { return ftpStatusDescription; }
            set { ftpStatusDescription = value; }
        }
        #endregion

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool disposed = false;

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
                    if(waitUpload != null)
                        waitUpload.Dispose();

                    if(waitDownload != null)
                        waitDownload.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                waitUpload = null;
                waitDownload = null;

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
        ~FTPStateAdapter()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// This custom exception class can handle all exceptions
    /// related to any Ftp client connections.
    /// </summary>
    public class FTPClientConnectionException : System.Exception
    {
        #region Constructors
        /// <summary>
        /// Constructor for message arguments.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public FTPClientConnectionException(string message)
            : base(message)
        {
            this.customMessage = message;
        }

        /// <summary>
        /// Constructor for inner exception argument.
        /// </summary>
        /// <param name="innerException">The inner exception object to pass.</param>
        public FTPClientConnectionException(System.Exception innerException)
            : base(string.Empty, innerException)
        {
            this.customInnerException = innerException;
        }

        #endregion

        #region Private Fields
        private string customMessage = string.Empty;
        private System.Exception customInnerException = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the current exception message.
        /// </summary>
        public string CustomMessage
        {
            get { return customMessage; }
        }

        /// <summary>
        /// Contains the current inner exception object.
        /// </summary>
        public System.Exception CustomInnerException
        {
            get { return customInnerException; }
        }
        #endregion
    }
}
