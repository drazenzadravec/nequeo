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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Net.WebSockets;

namespace Nequeo.Net.Http
{
    /// <summary>
    /// Web socket member.
    /// </summary>
    public class WebSocketMember : Nequeo.Net.IMemberContext
    {
        /// <summary>
        /// Web socket member.
        /// </summary>
        /// <param name="webSocket">The WebSocket class allows applications to 
        /// send and receive data after the WebSocket upgrade has completed.</param>
        public WebSocketMember(System.Net.WebSockets.WebSocket webSocket)
        {
            _webSocket = webSocket;

            // Set the initial timeout time.
            _initialTimeOutTime = DateTime.Now;
        }

        private System.Net.WebSockets.WebSocket _webSocket = null;
        private WebSocketReceiveResult _receiveResult = null;

        private DateTime _initialTimeOutTime;
        private Exception _lastError = null;
        private string _uniqueIdentifier = "";
        private string _connectionID = "";
        private string _serviceName = "";

        private object _lockObject = new object();

        /// <summary>
        /// Gets or sets the service name.
        /// </summary>
        public string ServiceName
        {
            get { return _serviceName; }
            set { _serviceName = value; }
        }

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public string UniqueIdentifier
        {
            get { return _uniqueIdentifier; }
            set { _uniqueIdentifier = value; }
        }

        /// <summary>
        /// Gets or sets the current unique connection identifier.
        /// </summary>
        public string ConnectionID
        {
            get { return _connectionID; }
            set { _connectionID = value; }
        }

        /// <summary>
        /// Gets or sets the web socket receive result.
        /// </summary>
        public WebSocketReceiveResult ReceiveResult
        {
            get { return _receiveResult; }
            set { _receiveResult = value; }
        }

        /// <summary>
        /// Sets the web socket receive result.
        /// </summary>
        public DateTime TimeoutTime
        {
            set { _initialTimeOutTime = value; }
        }

        /// <summary>
        /// Close the connection.
        /// </summary>
        public async void Close()
        {
            // Close the connection.
            if (_webSocket != null)
            {
                try
                {
                    // Close the connection.
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                catch (Exception ex) { _lastError = ex; }
            }
        }

        /// <summary>
        /// Get the last error that occured.
        /// </summary>
        /// <returns>The last exception.</returns>
        public Exception GetLastError()
        {
            return _lastError;
        }

        /// <summary>
        /// Has the current context timed out.
        /// </summary>
        /// <param name="timeout">The time out (minutes) set for the context; -1 wait indefinitely.</param>
        /// <returns>True if the context has timed out; else false.</returns>
        public bool HasTimedOut(int timeout)
        {
            bool ret = false;

            // If a timeout has been set.
            if (timeout > -1)
            {
                // Get the current time, subtract the initial time from the current time
                // to get the difference and assign the time span from the timeout.
                DateTime now = DateTime.Now;
                TimeSpan lapsed = now.Subtract(_initialTimeOutTime);
                TimeSpan timeoutTime = new TimeSpan(0, timeout, 0);

                // If the lapsed time is greater than then timeout span
                // than the timeout has been reached.
                if (lapsed.TotalMilliseconds >= timeoutTime.TotalMilliseconds)
                    ret = true;
            }

            // Return true if context has times out; else false.
            return ret;
        }

        /// <summary>
        /// Send data to the client through the member context from the server.
        /// </summary>
        /// <param name="data">The data received from the server.</param>
        public void SentFromServer(byte[] data)
        {
            // Only allow one thread at a time.
            lock (_lockObject)
                SentFromServerEx(data);
        }

        /// <summary>
        /// Send data to the client through the member context from the server.
        /// </summary>
        /// <param name="data">The data received from the server.</param>
        private async void SentFromServerEx(byte[] data)
        {
            if (_webSocket != null)
            {
                try
                {
                    // Reset the initial timeout time.
                    _initialTimeOutTime = DateTime.Now;

                    // Send data back to the client.
                    await _webSocket.SendAsync(new ArraySegment<byte>(data, 0, _receiveResult.Count),
                        WebSocketMessageType.Binary, _receiveResult.EndOfMessage, CancellationToken.None);
                }
                catch (Exception ex) { _lastError = ex; }
            }
        }
    }
}
