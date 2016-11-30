/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Collections;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Nequeo.Collections
{
    /// <summary>
    /// Streamed buffer storage.
    /// </summary>
    public abstract class StreamBuffer : System.IO.Stream
    {
        /// <summary>
        /// Streamed buffer storage.
        /// </summary>
        /// <param name="buffer">The buffer storage.</param>
        protected StreamBuffer(CircularBuffer<byte> buffer)
		{
            _buffer = buffer;
		}

        private CircularBuffer<byte> _buffer = null;
        private long _position = 0;

        /// <summary>
        /// Gets the underlying buffer.
        /// </summary>
        protected CircularBuffer<byte> Buffer
        {
            get { return _buffer; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        public override long Length
        {
            get { return _buffer.Count; }
        }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        public override long Position
        {
            get { return _position; }
            set 
            { 
                // Only if not removing items.
                if (!_buffer.RemoveItemsRead)
                {
                    if (value >= _buffer.Count)
                        _position = _buffer.Count;
                    else
                        _position = value;
                }
                else
                {
                    // If removing data when reading then
                    // position is zero.
                    _position = 0;
                }
            }
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered 
        /// data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
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
            if (null == buffer)
                throw new ArgumentNullException("buffer");
            if (0 > offset)
                throw new ArgumentOutOfRangeException("offset");
            if (0 > count)
                throw new ArgumentOutOfRangeException("count");
            if (offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset");
            if (offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException();

            if ((buffer.Length == 0) || (0 == count))
                return 0;

            // Copy the bytes read into the buffer.
            byte[] data = _buffer.Read((int)_position, count);
            Array.Copy(data, 0, buffer, offset, data.Length);

            // Only if not removing items.
            if (!_buffer.RemoveItemsRead)
            {
                // Set the position in the stream after reading.
                _position += (long)data.Length;

                // Position can not be graeter than count.
                if (_position >= _buffer.Count)
                    _position = _buffer.Count;
            }

            // The number of bytes read.
            return data.Length;
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type System.IO.SeekOrigin indicating the 
        /// reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            // Only if not removing items.
            if (!_buffer.RemoveItemsRead)
            {
                switch (origin)
                {
                    case System.IO.SeekOrigin.Current:
                        // If the buffer is less or same.
                        if ((offset + _position) < _buffer.Count)
                        {
                            // Less or same.
                            _position += offset;
                            return _position;
                        }
                        else
                        {
                            // Greater than then end of stream.
                            _position = _buffer.Count;
                            return _position;
                        }

                    case System.IO.SeekOrigin.End:
                        // If the buffer is less or same.
                        if (offset >= _buffer.Count)
                        {
                            // Less or same.
                            _position = 0;
                            return _position;
                        }
                        else
                        {
                            // Greater than then end of stream.
                            _position = _buffer.Count - offset;
                            return _position;
                        }

                    case System.IO.SeekOrigin.Begin:
                    default:
                        // If the buffer is less or same.
                        if (offset <_buffer.Count)
                        {
                            // Less or same.
                            _position = offset;
                            return _position;
                        }
                        else
                        {
                            // Greater than then end of stream.
                            _position = _buffer.Count;
                            return _position;
                        }
                }
            }
            else
                return _position;
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public override void SetLength(long value)
        {
            _buffer.Capacity = (int)value;
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
            if (null == buffer)
                throw new ArgumentNullException("buffer");
            if (0 > offset)
                throw new ArgumentOutOfRangeException("offset");
            if (0 > count)
                throw new ArgumentOutOfRangeException("count");
            if (offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset");
            if (offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException();

            if ((buffer.Length == 0) || (0 == count))
                return;

            // Only if not removing items.
            if (!_buffer.RemoveItemsRead)
            {
                // Set the position in the stream after reading.
                _position += (long)count;

                // Position can not be greater than count.
                if (_position >= _buffer.Count)
                    _position = _buffer.Count;
            }

            // If the position is positive.
            if (_position > 0)
            {
                // Add the buffer data to the storage.
                _buffer.Write(buffer, offset, count, _position);
            }
            else
            {
                // Add the buffer data to the storage.
                _buffer.Write(buffer, offset, count);
            }
        }
    }
}
