/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Nequeo.IO
{
    /// <summary>
    /// Stream to IStream wrapper.
    /// </summary>
    public class StreamWrapper : IStream
    {
        /// <summary>
        /// Stream to IStream wrapper.
        /// </summary>
        /// <param name="stream">The stream to wrap.</param>
        public StreamWrapper(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream", "Can't wrap null stream.");

            _stream = stream;
        }

        private Stream _stream = null;

        /// <summary>
        /// Creates a new stream object with its own seek pointer that references the
        /// same bytes as the original stream.
        /// </summary>
        /// <param name="ppstm">When this method returns, contains the new stream object. This parameter
        /// is passed uninitialized.</param>
        public void Clone(out IStream ppstm)
        {
            ppstm = new StreamWrapper(_stream);
        }

        /// <summary>
        /// Ensures that any changes made to a stream object that is open in transacted
        /// mode are reflected in the parent storage.
        /// </summary>
        /// <param name="grfCommitFlags">A value that controls how the changes for the stream object are committed.</param>
        public void Commit(int grfCommitFlags)
        {
            if (grfCommitFlags == 0)
                _stream.Close();
        }

        /// <summary>
        /// Copies a specified number of bytes from the current seek pointer in the stream
        /// to the current seek pointer in another stream.
        /// </summary>
        /// <param name="pstm">A reference to the destination stream.</param>
        /// <param name="cb">The number of bytes to copy from the source stream.</param>
        /// <param name="pcbRead">On successful return, contains the actual number of bytes read from the source.</param>
        /// <param name="pcbWritten">On successful return, contains the actual number of bytes written to the
        /// destination.</param>
        public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
        {
        }

        /// <summary>
        /// Restricts access to a specified range of bytes in the stream.
        /// </summary>
        /// <param name="libOffset">The byte offset for the beginning of the range.</param>
        /// <param name="cb">The length of the range, in bytes, to restrict.</param>
        /// <param name="dwLockType">The requested restrictions on accessing the range.</param>
        public void LockRegion(long libOffset, long cb, int dwLockType)
        {
        }

        /// <summary>
        /// Reads a specified number of bytes from the stream object into memory starting
        /// at the current seek pointer.
        /// </summary>
        /// <param name="pv">When this method returns, contains the data read from the stream. This parameter
        /// is passed uninitialized.</param>
        /// <param name="cb">The number of bytes to read from the stream object.</param>
        /// <param name="pcbRead">A pointer to a ULONG variable that receives the actual number of bytes read
        /// from the stream object.</param>
        public void Read(byte[] pv, int cb, IntPtr pcbRead)
        {
            if (_stream.CanRead)
                Marshal.WriteInt32(pcbRead, (Int32)_stream.Read(pv, 0, cb));
        }

        /// <summary>
        /// Discards all changes that have been made to a transacted stream since the
        /// last System.Runtime.InteropServices.ComTypes.IStream.Commit(System.Int32) call.
        /// </summary>
        public void Revert()
        {
        }

        /// <summary>
        /// Changes the seek pointer to a new location relative to the beginning of the
        /// stream, to the end of the stream, or to the current seek pointer.
        /// </summary>
        /// <param name="dlibMove">The displacement to add to dwOrigin.</param>
        /// <param name="dwOrigin">The origin of the seek. The origin can be the beginning of the file, the
        /// current seek pointer, or the end of the file.</param>
        /// <param name="plibNewPosition">On successful return, contains the offset of the seek pointer from the beginning
        /// of the stream.</param>
        public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
        {
            // If can seek and no origin.
            if (_stream.CanSeek && dwOrigin > 0)
            {
                Marshal.WriteInt32(plibNewPosition, (int)_stream.Seek(dlibMove, (SeekOrigin)dwOrigin));
            }
        }

        /// <summary>
        /// Changes the size of the stream object.
        /// </summary>
        /// <param name="libNewSize">The new size of the stream as a number of bytes.</param>
        public void SetSize(long libNewSize)
        {
            _stream.SetLength(libNewSize);
        }

        /// <summary>
        /// Retrieves the System.Runtime.InteropServices.STATSTG structure for this stream.
        /// </summary>
        /// <param name="pstatstg">When this method returns, contains a STATSTG structure that describes this
        /// stream object. This parameter is passed uninitialized.</param>
        /// <param name="grfStatFlag">Members in the STATSTG structure that this method does not return, thus saving
        /// some memory allocation operations.</param>
        public void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
        {
            pstatstg = new System.Runtime.InteropServices.ComTypes.STATSTG();
            pstatstg.pwcsName = "Stream";
            pstatstg.cbSize = _stream.Length;
        }

        /// <summary>
        /// Removes the access restriction on a range of bytes previously restricted
        /// with the System.Runtime.InteropServices.ComTypes.IStream.LockRegion(System.Int64,System.Int64,System.Int32)
        /// method.
        /// </summary>
        /// <param name="libOffset">The byte offset for the beginning of the range.</param>
        /// <param name="cb">The length, in bytes, of the range to restrict.</param>
        /// <param name="dwLockType">The access restrictions previously placed on the range.</param>
        public void UnlockRegion(long libOffset, long cb, int dwLockType)
        {
        }

        /// <summary>
        /// Writes a specified number of bytes into the stream object starting at the
        /// current seek pointer.
        /// </summary>
        /// <param name="pv">The buffer to write this stream to.</param>
        /// <param name="cb">The number of bytes to write to the stream.</param>
        /// <param name="pcbWritten">On successful return, contains the actual number of bytes written to the
        /// stream object. If the caller sets this pointer to System.IntPtr.Zero, this
        /// method does not provide the actual number of bytes written.</param>
        public void Write(byte[] pv, int cb, IntPtr pcbWritten)
        {
            if (_stream.CanWrite)
            {
                int written = Marshal.ReadInt32(pcbWritten);
                _stream.Write(pv, 0, written);
            }
        }
    }

    /// <summary>
    /// IStream to Stream wrapper.
    /// </summary>
    public class IStreamWrapper : Stream
    {
        IStream _stream = null;

        /// <summary>
        /// IStream to Stream wrapper.
        /// </summary>
        /// <param name="stream">The stream object.</param>
        public IStreamWrapper(IStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            _stream = stream;
        }

        /// <summary>
        /// Deconstructor.
        /// </summary>
        ~IStreamWrapper()
        {
            Close();
        }

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current
        /// stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified
        /// byte array with the values between offset and (offset + count - 1) replaced
        /// by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read
        /// from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the
        /// number of bytes requested if that many bytes are not currently available,
        /// or zero (0) if the end of the stream has been reached.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (offset != 0)
                throw new NotSupportedException("only 0 offset is supported");
            if (buffer.Length < count)
                throw new NotSupportedException("buffer is not large enough");

            IntPtr bytesRead = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)));
            try
            {
                _stream.Read(buffer, count, bytesRead);
                return Marshal.ReadInt32(bytesRead);
            }
            finally
            {
                Marshal.FreeCoTaskMem(bytesRead);
            }
        }

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current
        /// stream and advances the current position within this stream by the number
        /// of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current
        /// stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the
        /// current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (offset != 0)
                throw new NotSupportedException("only 0 offset is supported");
            _stream.Write(buffer, count, IntPtr.Zero);
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type System.IO.SeekOrigin indicating the reference point used
        /// to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            IntPtr address = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)));
            try
            {
                _stream.Seek(offset, (int)origin, address);
                return Marshal.ReadInt32(address);
            }
            finally
            {
                Marshal.FreeCoTaskMem(address);
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        public override long Length
        {
            get
            {
                System.Runtime.InteropServices.ComTypes.STATSTG statstg;
                _stream.Stat(out statstg, 1 /* STATSFLAG_NONAME*/ );
                return statstg.cbSize;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the
        /// current stream.
        /// </summary>
        public override long Position
        {
            get { return Seek(0, SeekOrigin.Current); }
            set { Seek(value, SeekOrigin.Begin); }
        }

        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public override void SetLength(long value)
        {
            _stream.SetSize(value);
        }

        /// <summary>
        /// Closes the current stream and releases any resources (such as sockets and
        /// file handles) associated with the current stream. Instead of calling this
        /// method, ensure that the stream is properly disposed.
        /// </summary>
        public override void Close()
        {
            _stream.Commit(0);
            // Marshal.ReleaseComObject(stream);
            _stream = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream and
        /// causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            _stream.Commit(-1);
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current
        /// stream supports reading.
        /// </summary>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current
        /// stream supports writing.
        /// </summary>
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current
        /// stream supports seeking.
        /// </summary>
        public override bool CanSeek
        {
            get { return true; }
        }
    }

}
