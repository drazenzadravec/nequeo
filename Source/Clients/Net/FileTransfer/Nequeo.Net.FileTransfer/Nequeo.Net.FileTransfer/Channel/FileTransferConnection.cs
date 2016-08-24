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

using Nequeo.Net.FileTransfer.Configuration;

namespace Nequeo.Net.FileTransfer.Channel
{
    /// <summary>
    /// The file transfer connection data.
    /// </summary>
    public sealed class FileTransferConnection
    {
        #region Private Fields
        private int _remotePort = 2766;
        private string _remoteHost = "localhost";
        private string _hostName = string.Empty;
        private bool _validateCertificate = false;
        private bool _useDataChannel = false;
        private bool _useSSLConnection = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get set, the remote host IP address or host  name.
        /// </summary>
        public string Host
        {
            get { return _remoteHost; }
            set { _remoteHost = value; }
        }

        /// <summary>
        /// Get set, the remote host port number.
        /// </summary>
        public int Port
        {
            get { return _remotePort; }
            set { _remotePort = value; }
        }

        /// <summary>
        /// Gets the name of the host to use.
        /// This host name is from the application configuration file.
        /// This property is case sensitive.
        /// </summary>
        public string HostName
        {
            get { return _hostName; }
        }

        /// <summary>
        /// Get set, should the ssl/tsl certificate be veryfied
        /// when making a secure connection.
        /// </summary>
        public bool ValidateCertificate
        {
            get { return _validateCertificate; }
            set { _validateCertificate = value; }
        }

        /// <summary>
        /// Get set, use a secondary connection channel when
        /// uploading and downloading files.
        /// </summary>
        public bool UseDataChannel
        {
            get { return _useDataChannel; }
            set { _useDataChannel = value; }
        }

        /// <summary>
        /// Get Set, use ssl encryption transfer.
        /// </summary>
        public bool UseSSLConnection
        {
            get { return _useSSLConnection; }
            set { _useSSLConnection = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Loads the remote host data from the configuration file.
        /// </summary>
        /// <param name="hostName">The host name from the configuration file.</param>
        public void GetRemoteConnection(string hostName)
        {
            // If the host name is null then
            // throw an exception.
            if (String.IsNullOrEmpty(hostName))
                throw new ArgumentNullException("HostName can not be null.",
                    new System.Exception("HostName must be a valid configuration name " +
                        "where host and port information is retreived."));

            // Assign the host name.
            _hostName = hostName;

            // Create a new host type
            // an load the values from the configuration
            // file into the host type.
            FileTransferServerHosts hosts =
                (FileTransferServerHosts)System.Configuration.ConfigurationManager.GetSection(
                    "FileTransferServerGroup/FileTransferServerHosts");

            // If the port is greater than zero then
            // assign the port number.
            if (hosts.HostSection[hostName].PortAttribute > 0)
                _remotePort = hosts.HostSection[hostName].PortAttribute;

            // Assign the remote host address.
            _remoteHost = hosts.HostSection[hostName].HostAttribute;
        }
        #endregion
    }

    /// <summary>
    /// This enum holds the server transfer.
    /// </summary>
    public enum ServerTransfer
    {
        /// <summary>
        /// No server set.
        /// </summary>
        None = -1,
        /// <summary>
        /// Default transfer type.
        /// </summary>
        FileTransfer = 0,
        /// <summary>
        /// Secure transfer type.
        /// </summary>
        SslFileTransfer = 1
    }

    /// <summary>
    /// This enum holds the transfer direction.
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
    /// This enum holds the operation.
    /// </summary>
    public enum Operation
    {
        /// <summary>
        /// No direction set.
        /// </summary>
        None = -1,
        /// <summary>
        /// Downloading a file.
        /// </summary>
        Download = 0,
        /// <summary>
        /// Uploading a file.
        /// </summary>
        Upload = 1,
        /// <summary>
        /// File list.
        /// </summary>
        GetFileList = 2,
        /// <summary>
        /// Directory list.
        /// </summary>
        GetDirectoryList = 3,
        /// <summary>
        /// Get current directory path.
        /// </summary>
        GetPath = 4,
        /// <summary>
        /// Set directory path.
        /// </summary>
        SetPath = 5,
        /// <summary>
        /// Create a new directory.
        /// </summary>
        CreateDirectory = 6,
        /// <summary>
        /// Delete a directory.
        /// </summary>
        DeleteDirectory = 7,
        /// <summary>
        /// Delete a file.
        /// </summary>
        DeleteFile = 8,
        /// <summary>
        /// Get the file size.
        /// </summary>
        GetFileSize = 9
    }

    /// <summary>
    /// Common enumerator class.
    /// </summary>
    public class FileTransferProtocol
    {
        #region Public File Transfer Server Methods
        /// <summary>
        /// Get all the server codes.
        /// </summary>
        /// <returns>The server code array</returns>
        public static long[] GetFileTransferServerCodes()
        {
            return new long[] { 100, 101, 200, 201, 202, 203, 204, 205,
                206, 207, 208, 400, 401, 402, 403, 404, 405, 406, 407, 
                408, 409, 410, 411, 412, 413, 500, 501, 502, 503, 504, 
                505, 506, 507, 508, 509, 510};
        }

        /// <summary>
        /// Get the file transfer response description.
        /// </summary>
        /// <param name="command">The command received.</param>
        /// <param name="code">The code received.</param>
        /// <returns>The response description.</returns>
        /// <remarks>set code to 0 for a command description.</remarks>
        public static string GetFileTransferServerResponseDescription(string command, long code)
        {
            string response = string.Empty;

            switch (code)
            {
                // Direct server responses.
                case 100:
                    response = GetServerDescription(command, "");
                    break;

                case 101:
                    response = GetServerDescription(command, "");
                    break;



                //Confirm responses.
                case 200:
                    response = GetServerDescription(command, "");
                    break;

                case 201:
                    response = GetServerDescription(command, "");
                    break;

                case 202:
                    response = GetServerDescription(command, "");
                    break;

                case 203:
                    response = GetServerDescription(command, "");
                    break;

                case 204:
                    response = GetServerDescription(command, "");
                    break;

                case 205:
                    response = GetServerDescription(command, "");
                    break;

                case 206:
                    response = GetServerDescription(command, "");
                    break;

                case 207:
                    response = GetServerDescription(command, "");
                    break;

                case 208:
                    response = GetServerDescription(command, "");
                    break;




                // Operation invalid responses.
                case 400:
                    response = GetServerDescription(command, "General response.");
                    break;

                case 401:
                    response = GetServerDescription(command, "No authentication, user suspended.");
                    break;

                case 402:
                    response = GetServerDescription(command, "User not validated.");
                    break;

                case 403:
                    response = GetServerDescription(command, "SQL authentication error.");
                    break;

                case 404:
                    response = GetServerDescription(command, "");
                    break;

                case 405:
                    response = GetServerDescription(command, "");
                    break;

                case 406:
                    response = GetServerDescription(command, "");
                    break;

                case 407:
                    response = GetServerDescription(command, "");
                    break;

                case 408:
                    response = GetServerDescription(command, "");
                    break;

                case 409:
                    response = GetServerDescription(command, "");
                    break;

                case 410:
                    response = GetServerDescription(command, "");
                    break;

                case 411:
                    response = GetServerDescription(command, "");
                    break;

                case 412:
                    response = GetServerDescription(command, "");
                    break;

                case 413:
                    response = GetServerDescription(command, "");
                    break;



                // Internal error responses.
                case 500:
                    response = GetServerDescription(command, "Connection attempt.");
                    break;

                case 501:
                    response = GetServerDescription(command, "Receiving file.");
                    break;

                case 502:
                    response = GetServerDescription(command, "Sending file.");
                    break;

                case 503:
                    response = GetServerDescription(command, "File list.");
                    break;

                case 504:
                    response = GetServerDescription(command, "Directory list.");
                    break;

                case 505:
                    response = GetServerDescription(command, "Set directory path.");
                    break;

                case 506:
                    response = GetServerDescription(command, "Get directory path.");
                    break;

                case 507:
                    response = GetServerDescription(command, "Delete file.");
                    break;

                case 508:
                    response = GetServerDescription(command, "Delete directory.");
                    break;

                case 509:
                    response = GetServerDescription(command, "Create new directory.");
                    break;

                case 510:
                    response = GetServerDescription(command, "Get file size.");
                    break;

                default:
                    response = GetServerDescription(command, "");
                    break;
            }

            // Return the response.
            return response;
        }

        /// <summary>
        /// Get all the server commands.
        /// </summary>
        /// <returns>The server command array</returns>
        public static string[] GetFileTransferServerCommands()
        {
            return new string[] { "WELC", "UCMD", "ERRO", "DNEX", "GFSZ", "CRDI", "DAEX", "DNEM",
                "DEDI", "DDEX", "DEFL", "FDEX", "GCDI", "SCDI", "ARCO", "REJE", "JOIN",
                "UPOK", "FAEX", "DMOK", "FNEX", "FNFD", "DNFD", "LIST", "DLST", "CDDN" };
        }

        /// <summary>
        /// Get the file transfer response description.
        /// </summary>
        /// <param name="command">The command received.</param>
        /// <param name="conc">Add the extra string to the description.</param>
        /// <returns>The response description.</returns>
        private static string GetServerDescription(string command, string conc)
        {
            string response = string.Empty;

            switch (command)
            {
                // Server response descriptions.
                case "WELC":
                    response = "Welcome to File Transfer Server - Authorized Accounts Only." + " " + conc;
                    break;

                case "UCMD":
                    response = "Unknown command sent." + " " + conc;
                    break;

                case "ERRO":
                    response = "Internal server error." + " " + conc;
                    break;

                case "DNEX":
                    response = "The directory does not exist. Directory validation exception." + " " + conc;
                    break;

                case "GFSZ":
                    response = "Get the file size." + " " + conc;
                    break;

                case "CRDI":
                    response = "Create a new directory." + " " + conc;
                    break;

                case "DAEX":
                    response = "Directory already exists." + " " + conc;
                    break;

                case "DNEM":
                    response = "Directory is not empty." + " " + conc;
                    break;

                case "DEDI":
                    response = "Directory has been deleted." + " " + conc;
                    break;

                case "DDEX":
                    response = "Directory does not exist." + " " + conc;
                    break;

                case "DEFL":
                    response = "File has been deleted." + " " + conc;
                    break;

                case "FDEX":
                    response = "File does not exist." + " " + conc;
                    break;

                case "GCDI":
                    response = "Get current directory path." + " " + conc;
                    break;

                case "SCDI":
                    response = "Set current directory path." + " " + conc;
                    break;

                case "ARCO":
                    response = "User already connected." + " " + conc;
                    break;

                case "REJE":
                    response = "Rejected login attempt." + " " + conc;
                    break;

                case "JOIN":
                    response = "User connection accepted." + " " + conc;
                    break;

                case "UPOK":
                    response = "Ready to receive file." + " " + conc;
                    break;

                case "FAEX":
                    response = "File already exists." + " " + conc;
                    break;

                case "DMOK":
                    response = "Ready to send file." + " " + conc;
                    break;

                case "FNEX":
                    response = "File does not exist." + " " + conc;
                    break;

                case "FNFD":
                    response = "No files found in the directory." + " " + conc;
                    break;

                case "DNFD":
                    response = "No directories found in the directory." + " " + conc;
                    break;

                case "LIST":
                    response = "List all files in the directory." + " " + conc;
                    break;

                case "DLST":
                    response = "List all directories in the directory." + " " + conc;
                    break;

                case "CDDN":
                    response = "Receive file from client completed." + " " + conc;
                    break;

                default:
                    break;
            }

            // Return the response.
            return response;
        }

        /// <summary>
        /// Get all the client commands.
        /// </summary>
        /// <returns>The client command array</returns>
        public static string[] GetFileTransferClientCommands()
        {
            return new string[] { "CONN", "CLOS", "UPLO", "DOWL", "GETF", "LIST", "DLST",
                "STOP", "GCDI", "SCDI", "DEFL", "DEDI", "CRDI", "GFSZ"};
        }

        /// <summary>
        /// Get the file transfer response description.
        /// </summary>
        /// <param name="command">The command received.</param>
        /// <returns>The response description.</returns>
        public static string GetFileTransferClientCommandDescription(string command)
        {
            string response = string.Empty;

            switch (command)
            {
                // Server request commands
                case "CONN":
                    response = "Attempt a connection." + " Usage [CONN <username>;<password>]";
                    break;

                case "CLOS":
                    response = "Close a connection." + " Usage [CLOS]";
                    break;

                case "UPLO":
                    response = "Request a file upload." + " Usage [UPLO <remotefile>;<size>]";
                    break;

                case "DOWL":
                    response = "Request a file download." + " Usage [DOWL <remotefile>]";
                    break;

                case "GETF":
                    response = "Start sending the file." + " Usage [GETF]";
                    break;

                case "LIST":
                    response = "List all files in the directory." + " Usage [LIST]";
                    break;

                case "DLST":
                    response = "List all directories in the directory." + " Usage [DLST]";
                    break;

                case "STOP":
                    response = "Stop the current download." + " Usage [STOP <UP/DN>]";
                    break;

                case "GCDI":
                    response = "Get the current directory." + " Usage [GCDI]";
                    break;

                case "SCDI":
                    response = "Set the current directory." + " Usage [SCDI <directorypath>]";
                    break;

                case "DEFL":
                    response = "Delete the file." + " Usage [DEFL <remotefile>]";
                    break;

                case "DEDI":
                    response = "Delete the directory." + " Usage [DEDI <directory>]";
                    break;

                case "CRDI":
                    response = "Create a new directory." + " Usage [CRDI <directory>]";
                    break;

                case "GFSZ":
                    response = "Get file size." + " Usage [GFSZ <remotefile>]";
                    break;

                default:
                    break;
            }

            // Return the response.
            return response;
        }
        #endregion
    }
}
