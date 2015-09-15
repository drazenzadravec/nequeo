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
using System.Threading.Tasks;

namespace Nequeo.IO.Stream
{
    /// <summary>
    /// Stream buffer base handler.
    /// </summary>
    public class StreamBufferBase : Nequeo.Collections.StreamBuffer
    {
        /// <summary>
        /// Request stream handler.
        /// </summary>
        public StreamBufferBase()
            : base(new Nequeo.Collections.CircularBuffer<byte>())
        {
            _internalBufferInstance = true;
        }

        /// <summary>
        /// Request stream handler.
        /// </summary>
        /// <param name="buffer">The internal data buffer handler.</param>
        public StreamBufferBase(Nequeo.Collections.CircularBuffer<byte> buffer)
            : base(buffer)
        {
            _internalBufferInstance = false;
        }

        private bool _internalBufferInstance = false;

        /// <summary>
        /// Releases the unmanaged resources used by the System.IO.Stream and optionally
        /// releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            // Only dispose if created internally.
            if (_internalBufferInstance)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (base.Buffer != null)
                        base.Buffer.Dispose();
                }
            }

            // Call the base dispose method.
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Stream buffer trigger handler.
    /// </summary>
    public class StreamBufferTrigger : Nequeo.Collections.StreamBuffer
    {
        /// <summary>
        /// Request stream handler.
        /// </summary>
        public StreamBufferTrigger()
            : base(new Nequeo.Collections.CircularBuffer<byte>())
        {
            _internalBufferInstance = true;
        }

        /// <summary>
        /// Request stream handler.
        /// </summary>
        /// <param name="buffer">The internal data buffer handler.</param>
        public StreamBufferTrigger(Nequeo.Collections.CircularBuffer<byte> buffer)
            : base(buffer)
        {
            _internalBufferInstance = false;
        }

        private bool _internalBufferInstance = false;

        private Action<int> _writeComplete = null;
        private Action<int> _readComplete = null;

        /// <summary>
        /// Gets or sets the action when the write is complete.
        /// </summary>
        public Action<int> WriteComplete
        {
            get { return _writeComplete; }
            set { _writeComplete = value; }
        }

        /// <summary>
        /// Gets or sets the action when the read is complete.
        /// </summary>
        public Action<int> ReadComplete
        {
            get { return _readComplete; }
            set { _readComplete = value; }
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances 
        /// the current position within this stream by the number of bytes 
        /// written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies 
        /// count bytes from buffer to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer 
        /// at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            base.Write(buffer, offset, count);

            // Trigger the write complete action.
            _writeComplete?.Invoke(count);
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
        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = base.Read(buffer, offset, count);

            // Trigger the write complete action.
            _readComplete?.Invoke(bytesRead);

            // Return the bytes read.
            return bytesRead;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the System.IO.Stream and optionally
        /// releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            // Only dispose if created internally.
            if (_internalBufferInstance)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (base.Buffer != null)
                        base.Buffer.Dispose();
                }
            }

            // Call the base dispose method.
            base.Dispose(disposing);
        }
    }
}
