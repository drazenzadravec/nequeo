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
    /// Close connection event.
    /// </summary>
    public class CloseEventArgs : EventArgs
    {
        #region Private Fields

        private bool _clean;
        private ushort _code;
        private Payload _payloadData;
        private byte[] _rawData;
        private string _reason;

        #endregion

        #region Internal Constructors
        /// <summary>
        /// Close connection.
        /// </summary>
        internal CloseEventArgs()
        {
            _payloadData = new Payload();
            _rawData = _payloadData.ApplicationData;

            _code = (ushort)CloseCodeStatus.NoStatusReceived;
            _reason = String.Empty;
        }

        /// <summary>
        /// Close connection.
        /// </summary>
        /// <param name="code">The close code.</param>
        internal CloseEventArgs(ushort code)
        {
            _code = code;
            _reason = String.Empty;
            _rawData = code.InternalToByteArray(Nequeo.Custom.ByteOrder.Big);
        }

        /// <summary>
        /// Close connection.
        /// </summary>
        /// <param name="code">The close code.</param>
        internal CloseEventArgs(CloseCodeStatus code)
            : this((ushort)code)
        {
        }

        /// <summary>
        /// Close connection.
        /// </summary>
        /// <param name="payloadData">The payload.</param>
        internal CloseEventArgs(Payload payloadData)
        {
            _payloadData = payloadData;
            _rawData = payloadData.ApplicationData;

            var len = _rawData.Length;
            _code = len > 1
                    ? _rawData.SubArray(0, 2).ToUInt16(Nequeo.Custom.ByteOrder.Big)
                    : (ushort)CloseCodeStatus.NoStatusReceived;

            _reason = len > 2
                      ? Encoding.UTF8.GetString(_rawData.SubArray(2, len - 2))
                      : String.Empty;
        }

        /// <summary>
        /// Close connection.
        /// </summary>
        /// <param name="code">The close code.</param>
        /// <param name="reason">The message.</param>
        internal CloseEventArgs(ushort code, string reason)
        {
            _code = code;
            _reason = reason ?? String.Empty;
            _rawData = code.Append(reason);
        }

        /// <summary>
        /// Close connection.
        /// </summary>
        /// <param name="code">The close code.</param>
        /// <param name="reason">The message.</param>
        internal CloseEventArgs(CloseCodeStatus code, string reason)
            : this((ushort)code, reason)
        {
        }
        #endregion

        #region Internal Properties
        /// <summary>
        /// Gets the payload.
        /// </summary>
        internal Payload PayloadData
        {
            get { return _payloadData ?? (_payloadData = new Payload(_rawData)); }
        }

        /// <summary>
        /// Gets the raw data.
        /// </summary>
        internal byte[] RawData
        {
            get { return _rawData; }
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the status code for the close.
        /// </summary>
        /// <value>
        /// A <see cref="ushort"/> that represents the status code for the close if any.
        /// </value>
        public ushort Code
        {
            get { return _code; }
        }

        /// <summary>
        /// Gets the reason for the close.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> that represents the reason for the close if any.
        /// </value>
        public string Reason
        {
            get { return _reason; }
        }

        /// <summary>
        /// Gets a value indicating whether the WebSocket connection has been closed cleanly.
        /// </summary>
        /// <value>
        /// <c>true</c> if the WebSocket connection has been closed cleanly; otherwise, <c>false</c>.
        /// </value>
        public bool WasClean
        {
            get { return _clean; }
            internal set { _clean = value; }
        }
        #endregion
    }
}
