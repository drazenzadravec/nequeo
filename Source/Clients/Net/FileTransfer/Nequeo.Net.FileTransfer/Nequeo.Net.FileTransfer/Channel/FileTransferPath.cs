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
    /// The file transfer file path data.
    /// </summary>
    public sealed class FileTransferPath
    {
        #region Private Fields
        private string remoteFile = string.Empty;
        private string targetFile = string.Empty;
        private string destinationFile = string.Empty;
        private string remoteDirectoryPath = string.Empty;
        private long targetFileSize = 0;
        private long destinationFileSize = 0;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get set, the upload or download remote file.
        /// </summary>
        public string RemoteFile
        {
            get { return remoteFile; }
            set { remoteFile = value; }
        }

        /// <summary>
        /// Get set, the directory path at the remote host.
        /// </summary>
        /// <remarks>Empty string indicates root path.</remarks>
        public string RemoteDirectoryPath
        {
            get { return remoteDirectoryPath; }
            set { remoteDirectoryPath = value; }
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
        /// Get set, the upload file size.
        /// </summary>
        public long TargetFileSize
        {
            get { return targetFileSize; }
            set { targetFileSize = value; }
        }

        /// <summary>
        /// Get set, the download file size.
        /// </summary>
        public long DestinationFileSize
        {
            get { return destinationFileSize; }
            set { destinationFileSize = value; }
        }
        #endregion
    }

    /// <summary>
    /// Class contains the current certificate.
    /// </summary>
    public sealed class X509Certificate2Info
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        /// <param name="chain">The certificate chain.</param>
        /// <param name="sslPolicyErrors">The policy error.</param>
        public X509Certificate2Info(X509Certificate2 certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            _certificate = certificate;
            _chain = chain;
            _sslPolicyErrors = sslPolicyErrors;
        }
        #endregion

        #region Private Fields
        private X509Certificate2 _certificate = null;
        private X509Chain _chain = null;
        private SslPolicyErrors _sslPolicyErrors = SslPolicyErrors.None;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get, the certificate.
        /// </summary>
        public X509Certificate2 Certificate
        {
            get { return _certificate; }
        }

        /// <summary>
        /// Get, certificate chain.
        /// </summary>
        public X509Chain Chain
        {
            get { return _chain; }
        }

        /// <summary>
        /// Get, policy error.
        /// </summary>
        public SslPolicyErrors SslPolicyErrors
        {
            get { return _sslPolicyErrors; }
        }
        #endregion
    }
}
