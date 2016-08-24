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
    /// The current file transfer state.
    /// </summary>
    internal sealed class FileTransferState
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
        /// The current ssl stream.
        /// </summary>
        private SslStream _sslStream = null;
        /// <summary>
        /// Total number of bytes read.
        /// </summary>
        private long _totalBytesRead = 0;
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
        /// Get set, the current ssl stream.
        /// </summary>
        public SslStream SslStream
        {
            get { return _sslStream; }
            set { _sslStream = value; }
        }

        /// <summary>
        /// Get set, the total bytes read.
        /// </summary>
        public long TotalBytesRead
        {
            get { return _totalBytesRead; }
            set { _totalBytesRead = value; }
        }
        #endregion
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
