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
using System.Net.Sockets;
using System.Threading;
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

namespace Nequeo.Net.WebSockets
{
    /// <summary>
    /// Web socket server response.
    /// </summary>
    public sealed class WebSocketResponse : Nequeo.Net.WebResponse
    {
        private string _deli = "\r\n";

        /// <summary>
        /// Gets or sets the WebSocket security acceptance.
        /// </summary>
        public string SecWebSocketAccept
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the WebSocket protocol request.
        /// </summary>
        public string SecWebSocketProtocol
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the WebSocket extensions.
        /// </summary>
        public string SecWebSocketExtensions
        {
            get;
            set;
        }

        /// <summary>
        /// Write the response status to the stream.
        /// </summary>
        public void WriteWebSocketHeaders()
        {
            byte[] buffer = null;
            string data = "";

            // Set the upgrade value.
            Upgrade = "websocket";
            ProtocolVersion = "HTTP/1.1";

            // Get the status code.
            Nequeo.Net.Http.Common.HttpStatusCode statusCode = Nequeo.Net.Http.Utility.GetStatusCode(101);
            StatusCode = statusCode.Code;
            StatusDescription = statusCode.Description;

            // If the SecWebSocketProtocol has been specified.
            if (!String.IsNullOrEmpty(SecWebSocketProtocol))
            {
                AddHeader("Sec-WebSocket-Protocol", SecWebSocketProtocol);
            }

            // If the SecWebSocketAccept has been specified.
            if (!String.IsNullOrEmpty(SecWebSocketAccept))
            {
                AddHeader("Sec-WebSocket-Accept", SecWebSocketAccept);
            }

            // If the SecWebSocketExtensions has been specified.
            if (!String.IsNullOrEmpty(SecWebSocketExtensions))
            {
                AddHeader("Sec-WebSocket-Extensions", SecWebSocketExtensions);
            }

            // Write the response status to the stream.
            WriteWebResponseHeaders(false);

            // Send the header end space.
            data = _deli;
            buffer = Encoding.Default.GetBytes(data);
            Write(buffer, 0, buffer.Length);
        }
    }
}
