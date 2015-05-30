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

using Nequeo.Extension;
using Nequeo.Custom;
using Nequeo.Net.WebSockets;

namespace Nequeo.Net.WebSockets.Protocol
{
    /// <summary>
    /// Represents well known WebSocket close codes as defined in section 11.7 of the WebSocket protocol spec.
    /// </summary>
    public enum CloseCodeStatus : ushort
    {
        /// <summary>
        /// The connection has closed after the request was fulfilled.
        /// </summary>
        NormalClosure = 1000,
        /// <summary>
        /// Indicates an endpoint is being removed. Either the server or client will become unavailable.
        /// </summary>
        GoingAway = 1001,
        /// <summary>
        /// The client or server is terminating the connection because of a protocol error.
        /// </summary>
        ProtocolError = 1002,
        /// <summary>
        /// The client or server is terminating the connection because it cannot accept the data type it received.
        /// </summary>
        UnsupportedData = 1003,
        /// <summary>
        /// Equivalent to close status 1004.
        /// Still undefined. A Reserved value.
        /// </summary>
        Undefined = 1004,
        /// <summary>
        /// No error specified.
        /// </summary>
        NoStatusReceived = 1005,
        /// <summary>
        /// The connection has closed abnormally. Either the server or client has forcibly closed the connection.
        /// </summary>
        AbnormalClosure = 1006,
        /// <summary>
        /// The client or server is terminating the connection because it has received data inconsistent with the message type.
        /// </summary>
        InvalidFramePayloadData = 1007,
        /// <summary>
        /// The connection will be closed because an endpoint has received a message that violates its policy.
        /// </summary>
        PolicyViolation = 1008,
        /// <summary>
        /// The message sent is bigger than specified.
        /// </summary>
        MessageTooBig = 1009,
        /// <summary>
        /// The client is terminating the connection because it expected the server to negotiate an extension.
        /// </summary>
        MandatoryExtension = 1010,
        /// <summary>
        /// The connection will be closed by the server because of an error on the server.
        /// </summary>
        InternalServerError = 1011,
        /// <summary>
        /// The client and server expect a TLS handshake before the connection can continue.
        /// </summary>
        TLSHandshake = 1015,
    }

    /// <summary>
    /// Internal extensions.
    /// </summary>
    internal static class InternalExtensions
    {
        /// <summary>
        /// Check if open.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>A messsage; else null.</returns>
        public static string CheckIfOpen(this WebSocketConnectionState state)
        {
            return state == WebSocketConnectionState.Connecting
                   ? "A WebSocket connection isn't established."
                   : state == WebSocketConnectionState.Closing
                     ? "While closing the WebSocket connection."
                     : state == WebSocketConnectionState.Closed
                       ? "The WebSocket connection has already been closed."
                       : null;
        }

        /// <summary>
        /// Check if valid control data.
        /// </summary>
        /// <param name="data">The array of bytes.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <returns>A message; else null.</returns>
        public static string CheckIfValidControlData(this byte[] data, string paramName)
        {
            return data.Length > 125
                   ? String.Format("'{0}' has greater than the allowable max size.", paramName)
                   : null;
        }

        /// <summary>
        /// Check if valid send data.
        /// </summary>
        /// <param name="data">The array of bytes.</param>
        /// <returns>A message; else null.</returns>
        public static string CheckIfValidSendData(this byte[] data)
        {
            return data == null
                   ? "'data' is null."
                   : null;
        }

        /// <summary>
        /// Is reserved.
        /// </summary>
        /// <param name="code">The CloseCodeStatus code.</param>
        /// <returns>True if reserved.</returns>
        public static bool IsReserved(this ushort code)
        {
            return code == (ushort)CloseCodeStatus.Undefined ||
                   code == (ushort)CloseCodeStatus.NoStatusReceived ||
                   code == (ushort)CloseCodeStatus.AbnormalClosure ||
                   code == (ushort)CloseCodeStatus.TLSHandshake;
        }

        /// <summary>
        /// Is reserved.
        /// </summary>
        /// <param name="code">The CloseCodeStatus code.</param>
        /// <returns>True if reserved.</returns>
        public static bool IsReserved(this CloseCodeStatus code)
        {
            return code == CloseCodeStatus.Undefined ||
                   code == CloseCodeStatus.NoStatusReceived ||
                   code == CloseCodeStatus.AbnormalClosure ||
                   code == CloseCodeStatus.TLSHandshake;
        }

        /// <summary>
        /// Convert to byte array.
        /// </summary>
        /// <param name="value">The current value.</param>
        /// <param name="order">The byte order.</param>
        /// <returns>The byte array.</returns>
        public static byte[] InternalToByteArray(this ushort value, Nequeo.Custom.ByteOrder order)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!order.IsHostOrder())
                Array.Reverse(bytes);

            return bytes;
        }

        /// <summary>
        /// Convert to byte array.
        /// </summary>
        /// <param name="value">The current value.</param>
        /// <param name="order">The byte order.</param>
        /// <returns>The byte array.</returns>
        public static byte[] InternalToByteArray(this ulong value, Nequeo.Custom.ByteOrder order)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!order.IsHostOrder())
                Array.Reverse(bytes);

            return bytes;
        }

        /// <summary>
        /// Append bytes.
        /// </summary>
        /// <param name="code">The close code.</param>
        /// <param name="reason">The message.</param>
        /// <returns>The appended bytes.</returns>
        public static byte[] Append(this ushort code, string reason)
        {
            using (var buff = new System.IO.MemoryStream())
            {
                var bytes = code.InternalToByteArray(Nequeo.Custom.ByteOrder.Big);
                buff.Write(bytes, 0, 2);
                if (reason != null && reason.Length > 0)
                {
                    bytes = Encoding.UTF8.GetBytes(reason);
                    buff.Write(bytes, 0, bytes.Length);
                }

                buff.Close();
                return buff.ToArray();
            }
        }

        /// <summary>
        /// Get a close code status messsage.
        /// </summary>
        /// <param name="code">Close code status.</param>
        /// <returns>The message.</returns>
        public static string GetMessage(this CloseCodeStatus code)
        {
            return code == CloseCodeStatus.ProtocolError
                   ? "A WebSocket protocol error has occurred."
                   : code == CloseCodeStatus.UnsupportedData
                     ? "An incorrect data has been received."
                     : code == CloseCodeStatus.AbnormalClosure
                       ? "An exception has occurred."
                       : code == CloseCodeStatus.InvalidFramePayloadData
                         ? "An inconsistent data has been received."
                         : code == CloseCodeStatus.PolicyViolation
                           ? "A policy violation has occurred."
                           : code == CloseCodeStatus.MessageTooBig
                             ? "A too big data has been received."
                             : code == CloseCodeStatus.MandatoryExtension
                               ? "WebSocket client didn't receive expected extension(s)."
                               : code == CloseCodeStatus.InternalServerError
                                 ? "WebSocket server got an internal error."
                                 : code == CloseCodeStatus.TLSHandshake
                                   ? "An error has occurred while handshaking."
                                   : String.Empty;
        }

        /// <summary>
        /// Check if valid close parameters.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="reason">The reason.</param>
        /// <returns>The message.</returns>
        public static string CheckIfValidCloseParameters(this CloseCodeStatus code, string reason)
        {
            return code.IsNoStatusCode() && !reason.IsNullOrEmpty()
                   ? "NoStatusCode cannot have a reason."
                   : !reason.IsNullOrEmpty() && Encoding.UTF8.GetBytes(reason).Length > 123
                     ? "A reason has greater than the allowable max size."
                     : null;
        }

        /// <summary>
        /// Is no status received.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>True if no status; else false.</returns>
        public static bool IsNoStatusCode(this CloseCodeStatus code)
        {
            return code == CloseCodeStatus.NoStatusReceived;
        }

        /// <summary>
        /// Check if closable.
        /// </summary>
        /// <param name="state">The web socket state.</param>
        /// <returns>The reason.</returns>
        public static string CheckIfClosable(this WebSocketConnectionState state)
        {
            return state == WebSocketConnectionState.Closing
                   ? "While closing the WebSocket connection."
                   : state == WebSocketConnectionState.Closed
                     ? "The WebSocket connection has already been closed."
                     : null;
        }
    }
}
