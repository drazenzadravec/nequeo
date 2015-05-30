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

namespace Nequeo.Net.WebSockets.Protocol
{
    /// <summary>
    /// Represents the frame type of the WebSocket frame as defined in section 11.8 of the WebSocket protocol spec.
    /// </summary>
    public enum MessageType : int
    {
        /// <summary>
        /// Text frame.
        /// </summary>
        Text = 0,
        /// <summary>
        /// Binary frame.
        /// </summary>
        Binary = 1,
        /// <summary>
        /// A receive has completed because a close message was received.
        /// </summary>
        Close = 2,
        /// <summary>
        /// Ping frame.
        /// </summary>
        Ping = 3,
    }

    /// <summary>
    /// Represents the frame type of the WebSocket frame as defined in section 11.8 of the WebSocket protocol spec.
    /// </summary>
    public enum OpCodeFrame : byte
    {
        /// <summary>
        /// Continuation frame.
        /// </summary>
        Continuation = 0x0,
        /// <summary>
        /// Text frame.
        /// </summary>
        Text = 0x1,
        /// <summary>
        /// Binary frame.
        /// </summary>
        Binary = 0x2,
        /// <summary>
        /// A receive has completed because a close message was received.
        /// </summary>
        Close = 0x8,
        /// <summary>
        /// Ping frame.
        /// </summary>
        Ping = 0x9,
        /// <summary>
        /// Pong frame.
        /// </summary>
        Pong = 0xa,
    }

    /// <summary>
    /// Message type to opcode frame converter.
    /// </summary>
    internal class OpCodeTypeHelper
    {
        /// <summary>
        /// Get the message type from the opcode frame type.
        /// </summary>
        /// <param name="opCode">The opcode frame.</param>
        /// <returns>The message type.</returns>
        public static MessageType GetMessageType(OpCodeFrame opCode)
        {
            switch (opCode)
            {
                case OpCodeFrame.Binary:
                    return MessageType.Binary;
                case OpCodeFrame.Close:
                    return MessageType.Close;
                case OpCodeFrame.Ping:
                    return MessageType.Ping;
                case OpCodeFrame.Text:
                    return MessageType.Text;
                default:
                    return MessageType.Close;
            }
        }

        /// <summary>
        /// Get the opcode frame from the message type.
        /// </summary>
        /// <param name="opCode">The message type.</param>
        /// <returns>The opcode frame.</returns>
        public static OpCodeFrame GetOpCodeFrame(MessageType opCode)
        {
            switch (opCode)
            {
                case MessageType.Binary:
                    return OpCodeFrame.Binary;
                case MessageType.Close:
                    return OpCodeFrame.Close;
                case MessageType.Ping:
                    return OpCodeFrame.Ping;
                case MessageType.Text:
                    return OpCodeFrame.Text;
                default:
                    return OpCodeFrame.Close;
            }
        }
    }
}
