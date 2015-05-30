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

using Nequeo.Net.WebSockets.Common;
using Nequeo.Extension;

namespace Nequeo.Net.WebSockets.Protocol
{
    /// <summary>
    /// The web socket payload.
    /// </summary>
    internal class Payload : IEnumerable<byte>
    {
        /// <summary>
        /// The web socket payload.
        /// </summary>
        public Payload()
        {
            _data = new byte[0];
        }

        /// <summary>
        /// The web socket payload.
        /// </summary>
        /// <param name="data">The array of payload data.</param>
        public Payload(byte[] data)
            : this(data, false)
        {
        }

        /// <summary>
        /// The web socket payload.
        /// </summary>
        /// <param name="data">The array of payload data.</param>
        /// <param name="masked">True if the data is masked.</param>
        public Payload(byte[] data, bool masked)
        {
            _data = data;
            _masked = masked;
            _length = data.LongLength;
        }

        private byte[] _data;
        private long _extDataLength;
        private long _length;
        private bool _masked;

        public const ulong MaxLength = Int64.MaxValue;

        /// <summary>
        /// Gets or sets the extension data length.
        /// </summary>
        public long ExtensionDataLength
        {
            get { return _extDataLength; }
            set { _extDataLength = value; }
        }

        /// <summary>
        /// Gets the includes reserved close status code.
        /// </summary>
        public bool IncludesReservedCloseStatusCode
        {
            get
            {
                return _length > 1 && _data.SubArray(0, 2).ToUInt16(Nequeo.Custom.ByteOrder.Big).IsReserved();
            }
        }

        /// <summary>
        /// Gets the application data.
        /// </summary>
        public byte[] ApplicationData
        {
            get
            {
                return _extDataLength > 0
                       ? _data.SubArray(_extDataLength, _length - _extDataLength)
                       : _data;
            }
        }

        /// <summary>
        /// Gets the extension data if any.
        /// </summary>
        public byte[] ExtensionData
        {
            get
            {
                return _extDataLength > 0
                       ? _data.SubArray(0, _extDataLength)
                       : new byte[0];
            }
        }

        /// <summary>
        /// Gets an indicator specifying if the data is masked.
        /// </summary>
        public bool IsMasked
        {
            get { return _masked; }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public ulong Length
        {
            get { return (ulong)_length; }
        }

        /// <summary>
        /// Mask the data.
        /// </summary>
        /// <param name="key">The key array of bytes.</param>
        public void Mask(byte[] key)
        {
            for (long i = 0; i < _length; i++)
                _data[i] = (byte)(_data[i] ^ key[i % 4]);

            _masked = !_masked;
        }

        /// <summary>
        /// Get the byte array of the message frame.
        /// </summary>
        /// <returns>The byte array.</returns>
        public byte[] ToByteArray()
        {
            return _data;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A System.Collections.Generic.IEnumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<byte> GetEnumerator()
        {
            foreach (var data in _data)
                yield return data;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return BitConverter.ToString(_data);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An System.Collections.IEnumerator object that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
