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

using Nequeo.Model;
using Nequeo.Extension;

namespace Nequeo.Net.WebSockets
{
    /// <summary>
    /// WebSocket utility provider.
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Set the request headers from the input stream within the current http context.
        /// </summary>
        /// <param name="webSocketContext">The current web socket context.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <param name="requestBufferStore">The request buffer store stream.</param>
        /// <exception cref="System.Exception"></exception>
        /// <returns>True if the headers have been found; else false.</returns>
        public static bool SetRequestHeaders(Nequeo.Net.WebSockets.WebSocketContext webSocketContext, long timeout = -1, int maxReadLength = 0, System.IO.Stream requestBufferStore = null)
        {
            // Header has not been found at this point.
            string requestMethod = "";
            string protocolVersion = "";
            byte[] rawData = null;
            List<NameValue> headers = null;

            // Web socket context is null.
            if (webSocketContext == null)
                return false;

            // Web socket request context is null.
            if (webSocketContext.WebSocketRequest == null)
                return false;

            // Web socket request context stream is null.
            if (webSocketContext.WebSocketRequest.Input == null)
                return false;

            // If not using the buffer store.
            if (requestBufferStore == null)
            {
                // We need to wait until we get all the header
                // data then send the context to the server.
                headers = Nequeo.Net.Utility.
                    ParseHeaders(webSocketContext.WebSocketRequest.Input, out requestMethod, ref rawData, timeout, maxReadLength);
            }
            else
            {
                // We need to wait until we get all the header
                // data then send the context to the server.
                headers = Nequeo.Net.Utility.
                    ParseHeaders(webSocketContext.RequestBufferStore, out requestMethod, ref rawData, timeout, maxReadLength);
            }

            // If headers exist then all has been found.
            if (headers != null)
            {
                // Set all the request headers.
                webSocketContext.WebSocketRequest.ReadWebSocketHeaders(headers, requestMethod);
                protocolVersion = webSocketContext.WebSocketRequest.ProtocolVersion;
                webSocketContext.WebSocketRequest.HeadersFound = true;

                // If the client is using protocol version "HTTP/1.1"
                if (protocolVersion.ToUpper().Trim().Replace(" ", "").Contains("HTTP/1.1"))
                {
                    // Do nothing.
                }

                // If the client is using protocol version "HTTP/2.0".
                if (protocolVersion.ToUpper().Trim().Replace(" ", "").Contains("HTTP/2"))
                {
                    // Do nothing.
                }

                // Set the user principle if credentials
                // have been passed.
                if (webSocketContext.WebSocketRequest.Credentials != null)
                {
                    // Add the credentials.
                    Nequeo.Security.IdentityMember identity =
                        new Nequeo.Security.IdentityMember(
                            webSocketContext.WebSocketRequest.Credentials.UserName,
                            webSocketContext.WebSocketRequest.Credentials.Password,
                            webSocketContext.WebSocketRequest.Credentials.Domain);

                    Nequeo.Security.AuthenticationType authType = Nequeo.Security.AuthenticationType.None;
                    try
                    {
                        // Attempt to get the authentication type.
                        authType = (Nequeo.Security.AuthenticationType)
                            Enum.Parse(typeof(Nequeo.Security.AuthenticationType), webSocketContext.WebSocketRequest.AuthorizationType);
                    }
                    catch { }

                    // Set the cuurent authentication schema.
                    identity.AuthenticationSchemes = authType;

                    // Create the principal.
                    Nequeo.Security.PrincipalMember principal = new Nequeo.Security.PrincipalMember(identity, null);

                    // Assign the principal
                    webSocketContext.User = principal;
                }

                return true;
            }
            else
                return false;
        }
    }
}
