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
    /// Web socket server request.
    /// </summary>
    public sealed class WebSocketRequest : Nequeo.Net.WebRequest
    {
        private bool _isWebSocketRequest = false;

        /// <summary>
        /// Gets an indicator that specifies if the request is a valid WebSocket request.
        /// </summary>
        public bool IsWebSocketRequest
        {
            get { return _isWebSocketRequest; }
        }

        /// <summary>
        /// Gets or sets the WebSocket security key.
        /// </summary>
        public string SecWebSocketKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the WebSocket protocols request.
        /// </summary>
        public string[] SecWebSocketProtocols
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the WebSocket standard version.
        /// </summary>
        public string SecWebSocketVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the WebSocket extensions.
        /// </summary>
        public string[] SecWebSocketExtensions
        {
            get;
            set;
        }

        /// <summary>
        /// Read the request headers.
        /// </summary>
        /// <param name="headers">The header collection.</param>
        /// <param name="request">The request header.</param>
        public void ReadWebSocketHeaders(List<NameValue> headers, string request)
        {
            // If headers exist.
            if (headers != null)
            {
                // Set the request headers.
                ReadWebRequestHeaders(headers, request);

                try
                {
                    // Get the Host
                    if (!String.IsNullOrEmpty(Headers["Host"]))
                    {
                        // Get the query details.
                        Uri uri = new Uri("ws://" + Headers["Host"] + "/" + Path.TrimStart('/'));
                        Url = uri;
                    }
                }
                catch { }

                // If the request is a WebSocket request.
                if (!String.IsNullOrEmpty(Upgrade) && Upgrade.Trim().ToLower().Contains("websocket"))
                {
                    _isWebSocketRequest = true;

                    // Get the SecWebSocketKey
                    if (!String.IsNullOrEmpty(Headers["Sec-WebSocket-Key"]))
                    {
                        // Get the SecWebSocketKey.
                        SecWebSocketKey = Headers["Sec-WebSocket-Key"].Trim();
                    }

                    // Get the SecWebSocketProtocol
                    if (!String.IsNullOrEmpty(Headers["Sec-WebSocket-Protocol"]))
                    {
                        // Get the SecWebSocketProtocol.
                        SecWebSocketProtocols = Headers["Sec-WebSocket-Protocol"].Split(new string[] { "," }, StringSplitOptions.None).Trim();
                    }

                    // Get the SecWebSocketVersion
                    if (!String.IsNullOrEmpty(Headers["Sec-WebSocket-Version"]))
                    {
                        // Get the SecWebSocketVersion.
                        SecWebSocketVersion = Headers["Sec-WebSocket-Version"].Trim();
                    }

                    // Get the SecWebSocketExtensions
                    if (!String.IsNullOrEmpty(Headers["Sec-WebSocket-Extensions"]))
                    {
                        // Get the SecWebSocketExtensions.
                        SecWebSocketExtensions = Headers["Sec-WebSocket-Extensions"].Split(new string[] { "," }, StringSplitOptions.None).Trim();
                    }
                }
            }
        }
    }
}
