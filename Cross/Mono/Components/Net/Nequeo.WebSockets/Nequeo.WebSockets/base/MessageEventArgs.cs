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
using Nequeo.IO.Compression;
using Nequeo.Net.WebSockets.Protocol;

namespace Nequeo.Net.WebSockets
{
    /// <summary>
    /// Message event.
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        #region Private Fields

        private string _data;
        private OpCodeFrame _opcode;
        private byte[] _rawData;

        #endregion

        #region Internal Constructors
        /// <summary>
        /// Message event.
        /// </summary>
        /// <param name="frame">The frame.</param>
        internal MessageEventArgs(Frame frame)
        {
            _opcode = frame.Opcode;
            _rawData = frame.PayloadData.ApplicationData;
            _data = ConvertToString(_opcode, _rawData);
        }

        /// <summary>
        /// Message event.
        /// </summary>
        /// <param name="opcode">The opcode.</param>
        /// <param name="rawData">The frame data.</param>
        internal MessageEventArgs(OpCodeFrame opcode, byte[] rawData)
        {
            if ((ulong)rawData.LongLength > Payload.MaxLength)
                throw new WebSocketException(CloseCodeStatus.MessageTooBig);

            _opcode = opcode;
            _rawData = rawData;
            _data = ConvertToString(opcode, rawData);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the message data as a <see cref="string"/>.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///   If the message data is empty, this property returns <see cref="String.Empty"/>.
        ///   </para>
        ///   <para>
        ///   Or if the message is a binary message, this property returns <c>"Binary"</c>.
        ///   </para>
        /// </remarks>
        /// <value>
        /// A <see cref="string"/> that represents the message data.
        /// </value>
        public string Data
        {
            get { return _data; }
        }

        /// <summary>
        /// Gets the message data as an array of <see cref="byte"/>.
        /// </summary>
        /// <value>
        /// An array of <see cref="byte"/> that represents the message data.
        /// </value>
        public byte[] RawData
        {
            get { return _rawData; }
        }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        public OpCodeFrame Type
        {
            get { return _opcode; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Convert to string.
        /// </summary>
        /// <param name="opcode">The opcode.</param>
        /// <param name="rawData">The frame data.</param>
        /// <returns>The string value.</returns>
        private static string ConvertToString(OpCodeFrame opcode, byte[] rawData)
        {
            return rawData.LongLength == 0
                   ? String.Empty
                   : opcode == OpCodeFrame.Text
                     ? Encoding.UTF8.GetString(rawData)
                     : opcode.ToString();
        }
        #endregion
    }
}
