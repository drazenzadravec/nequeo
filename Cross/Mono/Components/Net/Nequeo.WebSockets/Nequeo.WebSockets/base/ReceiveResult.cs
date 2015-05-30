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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.WebSockets;
using System.Web;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;

using Nequeo.Model;
using Nequeo.Extension;
using Nequeo.IO.Compression;
using Nequeo.Net.WebSockets.Protocol;
using Nequeo.IO.Stream.Extension;
using Nequeo.Net.WebSockets.Common;

namespace Nequeo.Net.WebSockets
{
    /// <summary>
    /// An instance of this class represents the result of performing a single ReceiveAsync operation on a WebSocket.
    /// </summary>
    public class ReceiveResult
    {
        /// <summary>
        /// An instance of this class represents the result of performing a single ReceiveAsync operation on a WebSocket.
        /// </summary>
        /// <param name="count">The number of bytes received.</param>
        /// <param name="messageType">The type of message that was received.</param>
        /// <param name="endOfMessage">Indicates whether this is the final message.</param>
        public ReceiveResult(int count, MessageType messageType, bool endOfMessage)
        {
            _count = count;
            _messageType = messageType;
            _endOfMessage = endOfMessage;
        }

        /// <summary>
        /// An instance of this class represents the result of performing a single ReceiveAsync operation on a WebSocket.
        /// </summary>
        /// <param name="count">The number of bytes received.</param>
        /// <param name="messageType">The type of message that was received.</param>
        /// <param name="endOfMessage">Indicates whether this is the final message.</param>
        /// <param name="closeStatus">Indicates the CloseStatus of the connection.</param>
        /// <param name="closeStatusDescription">The description of closeStatus.</param>
        public ReceiveResult(int count, MessageType messageType, bool endOfMessage, CloseCodeStatus? closeStatus, string closeStatusDescription)
        {
            _count = count;
            _messageType = messageType;
            _endOfMessage = endOfMessage;
            _closeStatus = closeStatus;
            _closeStatusDescription = closeStatusDescription;
        }

        private int _count = 0;
        private bool _endOfMessage = false;
        private MessageType _messageType = MessageType.Close;
        private CloseCodeStatus? _closeStatus = null;
        private string _closeStatusDescription = null;

        /// <summary>
        /// Gets an indication of the CloseStatus of the connection.
        /// </summary>
        public CloseCodeStatus? CloseStatus 
        {
            get { return _closeStatus; }
        }

        /// <summary>
        /// Gets the description of closeStatus.
        /// </summary>
        public string CloseStatusDescription 
        {
            get { return _closeStatusDescription; }
        }

        /// <summary>
        /// Gets the number of bytes received.
        /// </summary>
        public int Count 
        {
            get { return _count; }
        }

        /// <summary>
        /// Gets an indication whether this is the final message.
        /// </summary>
        public bool EndOfMessage 
        {
            get { return _endOfMessage; }
        }

        /// <summary>
        /// Gets the type of message that was received.
        /// </summary>
        public MessageType MessageType 
        {
            get { return _messageType; }
        }
    }

    /// <summary>
    /// An instance of this class represents the result of performing a single ReceiveAsync operation on a WebSocket.
    /// </summary>
    internal class ReceiveResultWebContext
    {
        /// <summary>
        /// Gets or sets the receive result.
        /// </summary>
        public ReceiveResult Result { get; set; }

        /// <summary>
        /// Gets or sets the receive data.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets and indicator specifying if all data has been returned to the call.
        /// </summary>
        public bool ReturnComplete { get; set; }

        /// <summary>
        /// Gets or sets the data read offset.
        /// </summary>
        public int Offset { get; set; }
    }
}
