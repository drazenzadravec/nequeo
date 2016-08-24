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

namespace Nequeo.Net.FileTransfer.Common
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
    /// asynchronous thread state ftp information.
    /// </summary>
    internal class FileTransferStateAdapter
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public FileTransferStateAdapter()
        {
            // Create a new wait thread event
            // and set the initial sate to nonsignaled.
            waitUpload = new ManualResetEvent(false);
            waitDownload = new ManualResetEvent(false);
        }
        #endregion

        #region Private Fields
        /// <summary>
        /// The current network stream.
        /// </summary>
        private NetworkStream networkStream = null;
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
        /// The full path and file name of the file to upload.
        /// </summary>
        private string targetFile = string.Empty;
        /// <summary>
        /// The full path and file name where data should be written to.
        /// </summary>
        private string destinationFile = string.Empty;
        /// <summary>
        /// The upload file size.
        /// </summary>
        private long targetFileSize = 0;
        /// <summary>
        /// The download file size.
        /// </summary>
        private long destinationFileSize = 0;
        /// <summary>
        /// The current exception that has occured.
        /// </summary>
        private Exception operationException = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get set, the current network stream.
        /// </summary>
        public NetworkStream NetworkStream
        {
            get { return networkStream; }
            set { networkStream = value; }
        }

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

        /// <summary>
        /// Get set, the current exception that
        /// has occured.
        /// </summary>
        public Exception OperationException
        {
            get { return operationException; }
            set { operationException = value; }
        }
        #endregion
    }
}
