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

namespace Nequeo.Net
{
    /// <summary>
    /// Internal helper
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Application event source name when logging to the event log.
        /// </summary>
        public static string EventApplicationName = Nequeo.Net.Properties.Settings.Default.EventApplicationName;
    }

    /// <summary>
    /// The current web socket state.
    /// </summary>
    public enum SocketState
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,
        /// <summary>
        /// Open.
        /// </summary>
        Open = 1,
        /// <summary>
        /// Closed.
        /// </summary>
        Closed = 2,
    }

    /// <summary>
    /// This enum holds the index action type to perform.
    /// </summary>
    public enum IndexAction
    {
        /// <summary>
        /// Increment.
        /// </summary>
        Increment = 0,
        /// <summary>
        /// Decrement.
        /// </summary>
        Decrement = 1,
    }

    /// <summary>
    /// This enum holds the socket action type.
    /// </summary>
    public enum SocketServerActionType
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,
        /// <summary>
        /// Streaming.
        /// </summary>
        Stream = 1,
        /// <summary>
        /// Action handler.
        /// </summary>
        ActionHandler = 2,
        /// <summary>
        /// Abstract method call.
        /// </summary>
        Method = 3,
    }

    /// <summary>
    /// This enum holds the socket action type.
    /// </summary>
    public enum SocketClientActionType
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,
        /// <summary>
        /// Streaming.
        /// </summary>
        Stream = 1,
        /// <summary>
        /// Action handler.
        /// </summary>
        ActionHandler = 2,
    }

    /// <summary>
    /// The send data request mode.
    /// </summary>
    public enum RequestMode : int
    {
        /// <summary>
        /// Normal open send close.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Active open send maintain connection.
        /// </summary>
        Active = 1,
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
    /// This enum holds the server type.
    /// </summary>
    public enum ServerType
    {
        /// <summary>
        /// No server type set.
        /// </summary>
        None = 0,
        /// <summary>
        /// Custom file transfer server type set.
        /// </summary>
        FileTransfer = 1,
        /// <summary>
        /// Custom secure socket layer file transfer server type set.
        /// </summary>
        SslFileTransfer = 2,
        /// <summary>
        /// FTP server type set.
        /// </summary>
        FTP = 3,
        /// <summary>
        /// Secure FTP server type set.
        /// </summary>
        FTPS = 4,
        /// <summary>
        /// POP3 server type set.
        /// </summary>
        POP3 = 5,
        /// <summary>
        /// Secure POP3 server type set.
        /// </summary>
        SslPOP3 = 6,
        /// <summary>
        /// Secure Proxy POP3 server type set.
        /// </summary>
        SslProxyPOP3 = 7,
        /// <summary>
        /// IMAP4 server type set.
        /// </summary>
        IMAP4 = 8,
        /// <summary>
        /// Secure IMAP4 server type set.
        /// </summary>
        SslIMAP4 = 9,
        /// <summary>
        /// Secure Proxy IMAP4 server type set.
        /// </summary>
        SslProxyIMAP4 = 10,
        /// <summary>
        /// Secure Start TLS command Proxy IMAP4 server type set.
        /// </summary>
        StartTslProxyIMAP4 = 11,
        /// <summary>
        /// Domain name server type set.
        /// </summary>
        DNS = 12,
        /// <summary>
        /// Whois type set.
        /// </summary>
        Whois = 13,
        /// <summary>
        /// SMTP type set.
        /// </summary>
        SMTP = 14,
        /// <summary>
        /// HTTP type set.
        /// </summary>
        HTTP = 15,
        /// <summary>
        /// Secure HTTP type set.
        /// </summary>
        HTTPS = 16,
        /// <summary>
        /// System log server.
        /// </summary>
        Syslog = 17,
        /// <summary>
        /// Session Initiation Protocol is a communications protocol for signaling and controlling multimedia communication sessions.
        /// </summary>
        SIP = 18,
        /// <summary>
        /// Real-time Transport Protocol is a network protocol for delivering audio and video over IP networks.
        /// </summary>
        RTP = 19,
        /// <summary>
        /// Secure Real-time Transport Protocol is a network protocol for delivering audio and video over IP networks.
        /// </summary>
        SRTP = 20,
        /// <summary>
        /// SSH File Transfer Protocol (also Secure File Transfer Protocol, or SFTP) is a network protocol 
        /// that provides file access, file transfer, and file management functionalities over any reliable data stream.
        /// </summary>
        SFTP = 21,
        /// <summary>
        /// Any proxy server.
        /// </summary>
        Proxy = 22,
        /// <summary>
        /// WebSocket is a protocol providing full-duplex communications channels over a single TCP connection.
        /// </summary>
        WS = 23,
        /// <summary>
        /// Secure WebSocket is a protocol providing full-duplex communications channels over a single TCP connection.
        /// </summary>
        WSS = 24,
        /// <summary>
        /// Local machine directory services.
        /// </summary>
        LMD = 25,
        /// <summary>
        /// Lightweight Directory Access Protocol (LDAP),
        /// application directory services (AD LDS).
        /// </summary>
        ADLDS = 26,
        /// <summary>
        /// Lightweight Directory Access Protocol (LDAP),
        /// domain machine directory services.
        /// </summary>
        LDAP = 27,
        /// <summary>
        /// Exchange Server Protocol.
        /// </summary>
        ESP = 28,
        /// <summary>
        /// Structured Query Language is a special-purpose programming language designed for managing data held in a 
        /// relational database management system (RDBMS), or for stream processing in a relational data 
        /// stream management system (RDSMS).
        /// </summary>
        SQL = 29,
        /// <summary>
        /// Not only SQL database provides a mechanism for storage and retrieval of data that is 
        /// modeled in means other than the tabular relations used in relational databases.
        /// </summary>
        NoSQL = 30,
        /// <summary>
        /// Git  is a distributed revision control system with an emphasis on speed, 
        /// data integrity, and support for distributed, non-linear workflows.
        /// </summary>
        GIT = 31,
        /// <summary>
        /// Team foundation server which provides source code management.
        /// </summary>
        TFS = 32,
        /// <summary>
        /// Virtual private network extends a private network across a public network, such as the Internet. 
        /// It enables a computer or network-enabled device to send and receive data across shared or public 
        /// networks as if it were directly connected to the private network, while benefiting from the 
        /// functionality, security and management policies of the public network.[1] A VPN is created by 
        /// establishing a virtual point-to-point connection through the use of dedicated connections, 
        /// virtual tunneling protocols, or traffic encryption.
        /// </summary>
        VPN = 33,
    }
}
