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
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;
using System.Reflection;

namespace Nequeo.Net.Provider
{
    /// <summary>
    /// Client side request stream handler.
    /// </summary>
    public class NetRequestStream : Nequeo.Collections.StreamBuffer
    {
        /// <summary>
        /// Request stream handler.
        /// </summary>
        /// <param name="buffer">The internal data buffer handler.</param>
        public NetRequestStream(Nequeo.Collections.CircularBuffer<byte> buffer)
            : base(buffer)
        {
        }

        private Action _readStreamActionHandler = null;
        private Action _closeActionHandler = null;

        /// <summary>
        /// Gets sets the read stream action handler.
        /// </summary>
        internal Action CloseActionHandler
        {
            get { return _closeActionHandler; }
            set { _closeActionHandler = value; }
        }

        /// <summary>
        /// Gets sets the read stream action handler.
        /// </summary>
        internal Action ReadStreamActionHandler
        {
            get { return _readStreamActionHandler; }
            set { _readStreamActionHandler = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead
        {
            get { return false; }
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered 
        /// data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            // Trigger the read handler.
            if (_readStreamActionHandler != null)
                _readStreamActionHandler();
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances 
        /// the current position within this stream by the number of bytes 
        /// written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies 
        /// count bytes from buffer to the currentstream.</param>
        /// <param name="offset">The zero-based byte offset in buffer 
        /// at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            base.Write(buffer, offset, count);

            // Trigger the read handler.
            if (_readStreamActionHandler != null)
                _readStreamActionHandler();
        }

        /// <summary>
        /// This method is not supported in request stream.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, 
        /// the buffer contains the specified byte array with the values between 
        /// offset and (offset + count - 1) replaced by the bytes read from the 
        /// current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which 
        /// to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from 
        /// the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be 
        /// less than the number of bytes requested if that many bytes are not 
        /// currently available, or zero (0) if the end of the stream has been 
        /// reached.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return 0;
        }

        /// <summary>
        /// Closes the current connection and releases any resources (such as sockets and
        /// file handles) associated with the current connection.
        /// </summary>
        public override void Close()
        {
            // Call the close action handler.
            if (_closeActionHandler != null)
                _closeActionHandler();
        }

        /// <summary>
        /// Clear the output buffer.
        /// </summary>
        public virtual void Clear()
        {
            base.Buffer.Clear();
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream 
        /// and advances the position within the stream by the 
        /// number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, 
        /// the buffer contains the specified byte array with the values between 
        /// offset and (offset + count - 1) replaced by the bytes read from the 
        /// current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which 
        /// to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from 
        /// the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be 
        /// less than the number of bytes requested if that many bytes are not 
        /// currently available, or zero (0) if the end of the stream has been 
        /// reached.</returns>
        internal int ReadFromStream(byte[] buffer, int offset, int count)
        {
            return base.Read(buffer, offset, count);
        }
    }
}
