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
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Nequeo.Net.Ftp
{
    /// <summary>
    /// FTP socket argument class containing event handler
    /// information for the error FTP socket process delegate.
    /// </summary>
    public class FTPSocketErrorArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the ftp process event argument.
        /// </summary>
        /// <param name="exceptionMessage">The error message within the ftp class.</param>
        /// <param name="ftpServer">The ftp server connecting to.</param>
        /// <param name="ftpProcess">The original command that was to be processed.</param>
        /// <param name="fileInfo">The original upload target file, download destination file or
        /// the list of files within a directory.</param>
        /// <param name="ftpStatusCode">The current ftp status code from the server.</param>
        /// <param name="ftpStatusDescription">The current ftp status description from the server.</param>
        /// <param name="transferDirection">The current ftp transfer direction.</param>
        public FTPSocketErrorArgs(string exceptionMessage, string ftpServer, string ftpProcess,
            string fileInfo, FtpStatusCode ftpStatusCode, string ftpStatusDescription,
            Nequeo.Net.Ftp.SocketTransferDirection transferDirection)
        {
            this.fileInfo = fileInfo;
            this.ftpServer = ftpServer;
            this.ftpProcess = ftpProcess;
            this.ftpStatusCode = ftpStatusCode;
            this.exceptionMessage = exceptionMessage;
            this.transferDirection = transferDirection;
            this.ftpStatusDescription = ftpStatusDescription;
        }
        #endregion

        #region Private Fields
        // The error message within the ftp class.
        private string exceptionMessage = string.Empty;
        // The ftp server connecting to.
        private string ftpServer = string.Empty;
        // The original command that was to be processed.
        private string ftpProcess = string.Empty;
        // The current ftp status code from the server.
        private FtpStatusCode ftpStatusCode;
        // The current ftp status description from the server.
        private string ftpStatusDescription = string.Empty;
        // The original upload target file, download destination file or
        // the list of files within a directory.
        private string fileInfo = string.Empty;
        // The current ftp transfer direction.
        private Nequeo.Net.Ftp.SocketTransferDirection transferDirection;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the error message within the ftp class.
        /// </summary>
        public string ExceptionMessage
        {
            get { return exceptionMessage; }
        }

        /// <summary>
        /// Contains ftp server connecting to.
        /// </summary>
        public string FTPServer
        {
            get { return ftpServer; }
        }

        /// <summary>
        /// Contains the original command that was to be processed.
        /// </summary>
        public string FTPProcess
        {
            get { return ftpProcess; }
        }

        /// <summary>
        /// Contains the most recent ftp status code from the server.
        /// </summary>
        public FtpStatusCode FtpStatusCode
        {
            get { return ftpStatusCode; }
        }

        /// <summary>
        /// Contains the current ftp status description from the server.
        /// </summary>
        public string FTPStatusDescription
        {
            get { return ftpStatusDescription; }
        }

        /// <summary>
        /// Contains the original upload target file, download 
        /// destination file or the list of files within a directory.
        /// </summary>
        public string FileInfo
        {
            get { return fileInfo; }
        }

        /// <summary>
        /// Contains the current ftp transfer direction.
        /// </summary>
        public Nequeo.Net.Ftp.SocketTransferDirection TransferDirection
        {
            get { return transferDirection; }
        }
        #endregion
    }

    /// <summary>
    /// FTP socket argument class containing event handler
    /// information for the FTP socket process delegate.
    /// </summary>
    public class FTPSocketArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the ftp process event argument.
        /// </summary>
        /// <param name="ftpServer">The ftp server connecting to.</param>
        /// <param name="ftpProcess">The original command that was to be processed.</param>
        /// <param name="fileInfo">The original upload target file, download destination file or
        /// the list of files within a directory.</param>
        /// <param name="ftpStatusCode">The current ftp status code from the server.</param>
        /// <param name="ftpStatusDescription">The current ftp status description from the server.</param>
        /// <param name="transferDirection">The current ftp transfer direction.</param>
        public FTPSocketArgs(string ftpServer, string ftpProcess, string fileInfo,
            FtpStatusCode ftpStatusCode, string ftpStatusDescription,
            Nequeo.Net.Ftp.SocketTransferDirection transferDirection)
        {
            this.fileInfo = fileInfo;
            this.ftpServer = ftpServer;
            this.ftpProcess = ftpProcess;
            this.ftpStatusCode = ftpStatusCode;
            this.transferDirection = transferDirection;
            this.ftpStatusDescription = ftpStatusDescription;
        }
        #endregion

        #region Private Fields
        // The ftp server connecting to.
        private string ftpServer = string.Empty;
        // The original command that was to be processed.
        private string ftpProcess = string.Empty;
        // The current ftp status code from the server.
        private FtpStatusCode ftpStatusCode;
        // The current ftp status description from the server.
        private string ftpStatusDescription = string.Empty;
        // The original upload target file, download destination file or
        // the list of files within a directory.
        private string fileInfo = string.Empty;
        // The current ftp transfer direction.
        private Nequeo.Net.Ftp.SocketTransferDirection transferDirection;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains ftp server connecting to.
        /// </summary>
        public string FTPServer
        {
            get { return ftpServer; }
        }

        /// <summary>
        /// Contains the original command that was to be processed.
        /// </summary>
        public string FTPProcess
        {
            get { return ftpProcess; }
        }

        /// <summary>
        /// Contains the most recent ftp status code from the server.
        /// </summary>
        public FtpStatusCode FtpStatusCode
        {
            get { return ftpStatusCode; }
        }

        /// <summary>
        /// Contains the current ftp status description from the server.
        /// </summary>
        public string FTPStatusDescription
        {
            get { return ftpStatusDescription; }
        }

        /// <summary>
        /// Contains the original upload target file, download 
        /// destination file or the list of files within a directory.
        /// </summary>
        public string FileInfo
        {
            get { return fileInfo; }
        }

        /// <summary>
        /// Contains the current ftp transfer direction.
        /// </summary>
        public Nequeo.Net.Ftp.SocketTransferDirection TransferDirection
        {
            get { return transferDirection; }
        }
        #endregion
    }

    /// <summary>
    /// This enum holds the ftp transfer direction.
    /// </summary>
    public enum TransferDirection
    {
        /// <summary>
        /// Uploading a file.
        /// </summary>
        Uploading = 0,
        /// <summary>
        /// Downloading a file.
        /// </summary>
        Downloading = 1
    }

    /// <summary>
    /// This enum holds the ftp transfer direction.
    /// </summary>
    public enum SocketTransferDirection
    {
        /// <summary>
        /// Downloading a file.
        /// </summary>
        Download = 0,
        /// <summary>
        /// Uploading a file.
        /// </summary>
        Upload = 1,
        /// <summary>
        /// Removed a directory.
        /// </summary>
        RemoveDirectory = 2,
        /// <summary>
        /// Deleted a file.
        /// </summary>
        DeleteFile = 3,
        /// <summary>
        /// Create a new directory.
        /// </summary>
        MakeDirectory = 4,
        /// <summary>
        /// Renamed a file.
        /// </summary>
        RenameFile = 5,
        /// <summary>
        /// List of files within a directory.
        /// </summary>
        DirectoryList = 6,
        /// <summary>
        /// The size of the file.
        /// </summary>
        FileSize = 7
    }

    /// <summary>
    /// Configuration connection interface.
    /// </summary>
    public interface IFTPConnection
    {
        /// <summary>
        /// Method to retrieve the specified ftp server configuration data.
        /// </summary>
        /// <param name="ftpServerKey">The ftp server configuration key.</param>
        /// <param name="userNameKey">The user name configuration key.</param>
        /// <param name="passwordKey">The password configuration key.</param>
        /// <param name="isAnonymousUserKey">The is anonymous user configuration key.</param>
        /// <param name="usePassiveKey">The behavior of a client application's data transfer process.</param>
        /// <param name="useBinaryKey">The value that specifies the data type for file transfers.</param>
        /// <param name="timeOutKey">The number of milliseconds to wait for a request.</param>
        /// <param name="useSSLConnectionKey">Should the connection use ssl encryption.</param>
        /// <param name="classOwner">The class that owns the current instance.</param>
        /// <param name="method">The method calling the log file location.</param>
        /// <param name="message">The message to include on error.</param>
        /// <returns>True if the configuration was returned.</returns>
        bool FTPConnection(string ftpServerKey, string userNameKey, string passwordKey,
            string isAnonymousUserKey, string usePassiveKey, string useBinaryKey, string timeOutKey,
            string useSSLConnectionKey, string classOwner, string method, string message);

        /// <summary>
        /// Method to retrieve the specified ftp transmit configuration data.
        /// </summary>
        /// <param name="uploadPathKey">The upload path configuration key.</param>
        /// <param name="downloadPathKey">The download path configuration key.</param>
        /// <param name="classOwner">The class that owns the current instance.</param>
        /// <param name="method">The method calling the log file location.</param>
        /// <param name="message">The message to include on error.</param>
        /// <returns>True if the configuration was returned.</returns>
        bool FTPTransmit(string uploadPathKey, string downloadPathKey,
            string classOwner, string method, string message);
    }

    /// <summary>
    /// FTP connection interface.
    /// </summary>
    public interface IFTPConnectionAdapter
    {
        /// <summary>
        /// Get Set, the anonymous user indicator.
        /// </summary>
        bool IsAnonymousUser { get; set; }

        /// <summary>
        /// Get Set, the user name.
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// Get Set, the password.
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Get Set, the ftp server.
        /// </summary>
        string FTPServer { get; set; }

        /// <summary>
        /// Get Set, use passive mode.
        /// </summary>
        bool UsePassive { get; set; }

        /// <summary>
        /// Get Set, use binay transfer.
        /// </summary>
        bool UseBinary { get; set; }

        /// <summary>
        /// Get Set, the time out request.
        /// </summary>
        int TimeOut { get; set; }

        /// <summary>
        /// Get, use ssl encryption transfer.
        /// </summary>
        bool UseSSLConnection { get; set; }
    }

    /// <summary>
    /// FTP transmit interface.
    /// </summary>
    public interface IFTPTransmitAdapter
    {
        /// <summary>
        /// Get Set, the upload ftp transmit path.
        /// </summary>
        string UploadPath { get; set; }

        /// <summary>
        /// Get Set, the download ftp transmit path.
        /// </summary>
        string DownloadPath { get; set; }
    }
}
