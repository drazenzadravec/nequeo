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
using System.Threading;
using System.Threading.Tasks;

using Nequeo.IO.Stream.Extension;

namespace Nequeo.IO.Stream
{
    /// <summary>
    /// Stream operation handler
    /// </summary>
	public class Operation
	{
        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="source">The source stream to copy from (read).</param>
        /// <param name="destination">The destination stream to copy to (write).</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsync(System.IO.Stream source, System.IO.Stream destination, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStream(source, destination, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="source">The source stream to copy from (read).</param>
        /// <param name="destination">The destination stream to copy to (write).</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStream(System.IO.Stream source, System.IO.Stream destination, int BUFFER_SIZE = 8192)
        {
            return CopyStream(source, destination, CancellationToken.None, BUFFER_SIZE);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="source">The source stream to copy from (read).</param>
        /// <param name="destination">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsync(System.IO.Stream source, System.IO.Stream destination, CancellationToken token, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStream(source, destination, token, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="source">The source stream to copy from (read).</param>
        /// <param name="destination">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStream(System.IO.Stream source, System.IO.Stream destination, CancellationToken token, int BUFFER_SIZE = 8192)
        {
            bool foundAll = true;
            int readBytes = 0;
            int bufferLength = BUFFER_SIZE;
            byte[] buffer = new byte[bufferLength];

            // Read all the data in the source stream and
            // write the data to the destination stream.
            do
            {
                // Read the data and then write the data.
                readBytes = source.Read(buffer, 0, bufferLength);

                // If data exists.
                if (readBytes > 0)
                {
                    // Each time data is read reset the timeout.
                    destination.Write(buffer, 0, readBytes);
                }

                // Cancellation has been requested for this token.
                if (token.IsCancellationRequested)
                {
                    foundAll = false;
                    break;
                }
            }
            while (readBytes != 0);

            // True if all data has been found.
            return foundAll;
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="source">The source stream to copy from (read).</param>
        /// <param name="destination">The destination stream to copy to (write).</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of source bytes). Waits until all bytes are read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsync(System.IO.Stream source, System.IO.Stream destination, long byteLength, long timeout = -1, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStream(source, destination, byteLength, timeout, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="source">The source stream to copy from (read).</param>
        /// <param name="destination">The destination stream to copy to (write).</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of source bytes). Waits until all bytes are read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStream(System.IO.Stream source, System.IO.Stream destination, long byteLength, long timeout = -1, int BUFFER_SIZE = 8192)
        {
            return CopyStream(source, destination, CancellationToken.None, byteLength, timeout, BUFFER_SIZE);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="source">The source stream to copy from (read).</param>
        /// <param name="destination">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of source bytes). Waits until all bytes are read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsync(System.IO.Stream source, System.IO.Stream destination, CancellationToken token, long byteLength, long timeout = -1, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStream(source, destination, token, byteLength, timeout, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="source">The source stream to copy from (read).</param>
        /// <param name="destination">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of source bytes). Waits until all bytes are read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStream(System.IO.Stream source, System.IO.Stream destination, CancellationToken token, long byteLength, long timeout = -1, int BUFFER_SIZE = 8192)
        {
            bool foundAll = true;
            int readBytes = 0;
            long totalBytesRead = 0;
            int bufferLength = BUFFER_SIZE;
            byte[] buffer = new byte[bufferLength];

            // Only if data needs to be found.
            if (byteLength > 0)
            {
                // Start a new timeout clock.
                Custom.TimeoutClock timeoutClock = new Custom.TimeoutClock((int)timeout);

                // Read all the data in the source stream and
                // write the data to the destination stream.
                do
                {
                    // Determine the number of bytes to read
                    // from the byteLength and buffer_size.
                    // Can not read more then byteLength.
                    if (byteLength <= bufferLength)
                    {
                        // Look for current total bytes read and number left.
                        long left = byteLength - totalBytesRead;
                        if (left <= bufferLength)
                            bufferLength = (int)left;
                        else
                            bufferLength = (int)byteLength;
                    }
                    else
                    {
                        // Look for current total bytes read and number left.
                        long left = byteLength - totalBytesRead;
                        if (left <= bufferLength)
                            bufferLength = (int)left;
                    }

                    // Read the data and then write the data.
                    readBytes = source.Read(buffer, 0, bufferLength);

                    // If data exists.
                    if (readBytes > 0)
                    {
                        // Each time data is read reset the timeout.
                        timeoutClock.Reset();
                        totalBytesRead += readBytes;
                        destination.Write(buffer, 0, readBytes);
                    }
                    else
                        SpinWaitHandler(source, timeoutClock);

                    // If the timeout has bee reached then
                    // break from the loop.
                    if (timeoutClock.IsComplete())
                    {
                        foundAll = false;
                        break;
                    }

                    // Cancellation has been requested for this token.
                    if (token.IsCancellationRequested)
                    {
                        foundAll = false;
                        break;
                    }
                }
                while (totalBytesRead < byteLength);
            }

            // True if all data has been found.
            return foundAll;
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="source">The source stream to copy from (read).</param>
        /// <param name="destination">The destination stream to copy to (write).</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of source bytes). Waits until all bytes are read.</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="offsetEnd">The number of bytes to skip from the end of the stream (number of bytes not to read from the bottom).</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsync(System.IO.Stream source, System.IO.Stream destination, long byteLength, long offsetStart, long offsetEnd, long timeout = -1, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStream(source, destination, byteLength, offsetStart, offsetEnd, timeout, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="source">The source stream to copy from (read).</param>
        /// <param name="destination">The destination stream to copy to (write).</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of source bytes). Waits until all bytes are read.</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="offsetEnd">The number of bytes to skip from the end of the stream (number of bytes not to read from the bottom).</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStream(System.IO.Stream source, System.IO.Stream destination, long byteLength, long offsetStart, long offsetEnd, long timeout = -1, int BUFFER_SIZE = 8192)
        {
            return CopyStream(source, destination, CancellationToken.None, byteLength, offsetStart, offsetEnd, timeout, BUFFER_SIZE);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="source">The source stream to copy from (read).</param>
        /// <param name="destination">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of source bytes). Waits until all bytes are read.</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="offsetEnd">The number of bytes to skip from the end of the stream (number of bytes not to read from the bottom).</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsync(System.IO.Stream source, System.IO.Stream destination, CancellationToken token, long byteLength, long offsetStart, long offsetEnd, long timeout = -1, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStream(source, destination, token, byteLength, offsetStart, offsetEnd, timeout, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="source">The source stream to copy from (read).</param>
        /// <param name="destination">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of source bytes). Waits until all bytes are read.</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="offsetEnd">The number of bytes to skip from the end of the stream (number of bytes not to read from the bottom).</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStream(System.IO.Stream source, System.IO.Stream destination, CancellationToken token, long byteLength, long offsetStart, long offsetEnd, long timeout = -1, int BUFFER_SIZE = 8192)
        {
            bool foundAll = true;
            int readBytes = 0;
            long totalBytesRead = 0;
            int bufferLength = BUFFER_SIZE;
            byte[] buffer = new byte[bufferLength];

            long totalBytesNotToRead = offsetStart + offsetEnd;
            long totalBytesToRead = byteLength - totalBytesNotToRead;

            // Only if data needs to be found.
            if (byteLength > 0)
            {
                // Start a new timeout clock.
                Custom.TimeoutClock timeoutClock = new Custom.TimeoutClock((int)timeout);

                // Offset starting point within the stream.
                source.Seek(offsetStart, System.IO.SeekOrigin.Begin);

                // Read all the data in the source stream and
                // write the data to the destination stream.
                do
                {
                    // Determine the number of bytes to read
                    // from the byteLength and buffer_size.
                    // Can not read more then byteLength.
                    if (byteLength <= bufferLength)
                    {
                        // Look for current total bytes read and number left.
                        long left = byteLength - totalBytesRead;
                        if (left <= bufferLength)
                            bufferLength = (int)left;
                        else
                            bufferLength = (int)byteLength;
                    }
                    else
                    {
                        // Look for current total bytes read and number left.
                        long left = byteLength - totalBytesRead;
                        if (left <= bufferLength)
                            bufferLength = (int)left;
                    }

                    // Read the data and then write the data.
                    readBytes = source.Read(buffer, 0, bufferLength);
                    totalBytesRead += readBytes;

                    // If data exists.
                    if (readBytes > 0)
                    {
                        // Each time data is read reset the timeout.
                        timeoutClock.Reset();

                        // If all the bytes have been read in the first iteration.
                        if (readBytes >= totalBytesToRead)
                            destination.Write(buffer, 0, (int)totalBytesToRead);
                        else
                        {
                            // If the total number of bytes has been read
                            if (totalBytesRead >= totalBytesToRead)
                            {
                                // Calculate the last set of bytes to read.
                                long extraBytesNotToRead = totalBytesRead - totalBytesToRead;
                                long lastBytesToRead = readBytes - extraBytesNotToRead;

                                // If no more bytes are to be read.
                                if ((totalBytesRead - totalBytesToRead) > 0)
                                    destination.Write(buffer, 0, (int)lastBytesToRead);
                                else
                                    // Keep on reading more bytes.
                                    destination.Write(buffer, 0, readBytes);
                            }
                            else
                                // Keep on reading more bytes.
                                destination.Write(buffer, 0, readBytes);
                        }
                    }
                    else
                        SpinWaitHandler(source, timeoutClock);

                    // If the timeout has bee reached then
                    // break from the loop.
                    if (timeoutClock.IsComplete())
                    {
                        foundAll = false;
                        break;
                    }

                    // Cancellation has been requested for this token.
                    if (token.IsCancellationRequested)
                    {
                        foundAll = false;
                        break;
                    }
                }
                while (totalBytesRead < totalBytesToRead);
            }

            // True if all data has been found.
            return foundAll;
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="source">The source stream to copy from (read).</param>
        /// <param name="destination">The destination stream to copy to (write).</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="numberToRead">The total number of bytes to read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsyncEx(System.IO.Stream source, System.IO.Stream destination, long offsetStart, long numberToRead, long timeout = -1, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStreamEx(source, destination, offsetStart, numberToRead, timeout, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="source">The source stream to copy from (read).</param>
        /// <param name="destination">The destination stream to copy to (write).</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="numberToRead">The total number of bytes to read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStreamEx(System.IO.Stream source, System.IO.Stream destination, long offsetStart, long numberToRead, long timeout = -1, int BUFFER_SIZE = 8192)
        {
            return CopyStreamEx(source, destination, CancellationToken.None, offsetStart, numberToRead, timeout, BUFFER_SIZE);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="source">The source stream to copy from (read).</param>
        /// <param name="destination">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="numberToRead">The total number of bytes to read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsyncEx(System.IO.Stream source, System.IO.Stream destination, CancellationToken token, long offsetStart, long numberToRead, long timeout = -1, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStreamEx(source, destination, token, offsetStart, numberToRead, timeout, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="source">The source stream to copy from (read).</param>
        /// <param name="destination">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="numberToRead">The total number of bytes to read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStreamEx(System.IO.Stream source, System.IO.Stream destination, CancellationToken token, long offsetStart, long numberToRead, long timeout = -1, int BUFFER_SIZE = 8192)
        {
            bool foundAll = true;
            int readBytes = 0;
            long totalBytesRead = 0;
            int bufferLength = BUFFER_SIZE;
            byte[] buffer = new byte[bufferLength];

            long totalBytesToRead = numberToRead;

            // Only if data needs to be found.
            if (numberToRead > 0)
            {
                // Start a new timeout clock.
                Custom.TimeoutClock timeoutClock = new Custom.TimeoutClock((int)timeout);

                // Offset starting point within the stream.
                source.Seek(offsetStart, System.IO.SeekOrigin.Begin);

                // Read all the data in the source stream and
                // write the data to the destination stream.
                do
                {
                    // Determine the number of bytes to read
                    // from the byteLength and buffer_size.
                    // Can not read more then byteLength.
                    if (numberToRead <= bufferLength)
                    {
                        // Look for current total bytes read and number left.
                        long left = numberToRead - totalBytesRead;
                        if (left <= bufferLength)
                            bufferLength = (int)left;
                        else
                            bufferLength = (int)numberToRead;
                    }
                    else
                    {
                        // Look for current total bytes read and number left.
                        long left = numberToRead - totalBytesRead;
                        if (left <= bufferLength)
                            bufferLength = (int)left;
                    }

                    // Read the data and then write the data.
                    readBytes = source.Read(buffer, 0, bufferLength);
                    totalBytesRead += readBytes;

                    // If data exists.
                    if (readBytes > 0)
                    {
                        // Each time data is read reset the timeout.
                        timeoutClock.Reset();

                        // If all the bytes have been read in the first iteration.
                        if (readBytes >= totalBytesToRead)
                            destination.Write(buffer, 0, (int)totalBytesToRead);
                        else
                        {
                            // If the total number of bytes has been read
                            if (totalBytesRead >= totalBytesToRead)
                            {
                                // Calculate the last set of bytes to read.
                                long extraBytesNotToRead = totalBytesRead - totalBytesToRead;
                                long lastBytesToRead = readBytes - extraBytesNotToRead;

                                // If no more bytes are to be read.
                                if ((totalBytesRead - totalBytesToRead) > 0)
                                    destination.Write(buffer, 0, (int)lastBytesToRead);
                                else
                                    // Keep on reading more bytes.
                                    destination.Write(buffer, 0, readBytes);
                            }
                            else
                                // Keep on reading more bytes.
                                destination.Write(buffer, 0, readBytes);
                        }
                    }
                    else
                        SpinWaitHandler(source, timeoutClock);

                    // If the timeout has bee reached then
                    // break from the loop.
                    if (timeoutClock.IsComplete())
                    {
                        foundAll = false;
                        break;
                    }

                    // Cancellation has been requested for this token.
                    if (token.IsCancellationRequested)
                    {
                        foundAll = false;
                        break;
                    }
                }
                while (totalBytesRead < totalBytesToRead);
            }

            // True if all data has been found.
            return foundAll;
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsync(System.IO.BinaryReader reader, System.IO.BinaryWriter writer, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStream(reader, writer, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStream(System.IO.BinaryReader reader, System.IO.BinaryWriter writer, int BUFFER_SIZE = 8192)
        {
            return CopyStream(reader, writer, CancellationToken.None, BUFFER_SIZE);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsync(System.IO.BinaryReader reader, System.IO.BinaryWriter writer, CancellationToken token, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStream(reader, writer, token, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStream(System.IO.BinaryReader reader, System.IO.BinaryWriter writer, CancellationToken token, int BUFFER_SIZE = 8192)
        {
            bool foundAll = true;
            int readBytes = 0;
            int bufferLength = BUFFER_SIZE;
            byte[] buffer = new byte[bufferLength];

            // Read all the data in the source stream and
            // write the data to the destination stream.
            do
            {
                // Read the data and then write the data.
                readBytes = reader.Read(buffer, 0, bufferLength);

                // If data exists.
                if (readBytes > 0)
                {
                    // Each time data is read reset the timeout.
                    writer.Write(buffer, 0, readBytes);
                }

                // Cancellation has been requested for this token.
                if (token.IsCancellationRequested)
                {
                    foundAll = false;
                    break;
                }
            }
            while (readBytes != 0);

            // True if all data has been found.
            return foundAll;
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of reader bytes). Waits until all bytes are read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsync(System.IO.BinaryReader reader, System.IO.BinaryWriter writer, long byteLength, long timeout = -1, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStream(reader, writer, byteLength, timeout, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of reader bytes). Waits until all bytes are read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStream(System.IO.BinaryReader reader, System.IO.BinaryWriter writer, long byteLength, long timeout = -1, int BUFFER_SIZE = 8192)
        {
            return CopyStream(reader, writer, CancellationToken.None, byteLength, timeout, BUFFER_SIZE);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of reader bytes). Waits until all bytes are read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsync(System.IO.BinaryReader reader, System.IO.BinaryWriter writer, CancellationToken token, long byteLength, long timeout = -1, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStream(reader, writer, token, byteLength, timeout, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of reader bytes). Waits until all bytes are read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStream(System.IO.BinaryReader reader, System.IO.BinaryWriter writer, CancellationToken token, long byteLength, long timeout = -1, int BUFFER_SIZE = 8192)
        {
            bool foundAll = true;
            int readBytes = 0;
            long totalBytesRead = 0;
            int bufferLength = BUFFER_SIZE;
            byte[] buffer = new byte[bufferLength];

            // Only if data needs to be found.
            if (byteLength > 0)
            {
                // Start a new timeout clock.
                Custom.TimeoutClock timeoutClock = new Custom.TimeoutClock((int)timeout);

                // Read all the data in the source stream and
                // write the data to the destination stream.
                do
                {
                    // Determine the number of bytes to read
                    // from the byteLength and buffer_size.
                    // Can not read more then byteLength.
                    if (byteLength <= bufferLength)
                    {
                        // Look for current total bytes read and number left.
                        long left = byteLength - totalBytesRead;
                        if (left <= bufferLength)
                            bufferLength = (int)left;
                        else
                            bufferLength = (int)byteLength;
                    }
                    else
                    {
                        // Look for current total bytes read and number left.
                        long left = byteLength - totalBytesRead;
                        if (left <= bufferLength)
                            bufferLength = (int)left;
                    }

                    // Read the data and then write the data.
                    readBytes = reader.Read(buffer, 0, bufferLength);

                    // If data exists.
                    if (readBytes > 0)
                    {
                        // Each time data is read reset the timeout.
                        timeoutClock.Reset();
                        totalBytesRead += readBytes;
                        writer.Write(buffer, 0, readBytes);
                    }
                    else
                        SpinWaitHandler(reader, timeoutClock);

                    // If the timeout has bee reached then
                    // break from the loop.
                    if (timeoutClock.IsComplete())
                    {
                        foundAll = false;
                        break;
                    }

                    // Cancellation has been requested for this token.
                    if (token.IsCancellationRequested)
                    {
                        foundAll = false;
                        break;
                    }
                }
                while (totalBytesRead < byteLength);
            }

            // True if all data has been found.
            return foundAll;
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of reader bytes). Waits until all bytes are read.</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="offsetEnd">The number of bytes to skip from the end of the stream (number of bytes not to read from the bottom).</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsync(System.IO.BinaryReader reader, System.IO.BinaryWriter writer, long byteLength, long offsetStart, long offsetEnd, long timeout = -1, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStream(reader, writer, byteLength, offsetStart, offsetEnd, timeout, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of reader bytes). Waits until all bytes are read.</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="offsetEnd">The number of bytes to skip from the end of the stream (number of bytes not to read from the bottom).</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStream(System.IO.BinaryReader reader, System.IO.BinaryWriter writer, long byteLength, long offsetStart, long offsetEnd, long timeout = -1, int BUFFER_SIZE = 8192)
        {
            return CopyStream(reader, writer, CancellationToken.None, byteLength, offsetStart, offsetEnd, timeout, BUFFER_SIZE);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of reader bytes). Waits until all bytes are read.</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="offsetEnd">The number of bytes to skip from the end of the stream (number of bytes not to read from the bottom).</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsync(System.IO.BinaryReader reader, System.IO.BinaryWriter writer, CancellationToken token, long byteLength, long offsetStart, long offsetEnd, long timeout = -1, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStream(reader, writer, token, byteLength, offsetStart, offsetEnd, timeout, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of reader bytes). Waits until all bytes are read.</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="offsetEnd">The number of bytes to skip from the end of the stream (number of bytes not to read from the bottom).</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStream(System.IO.BinaryReader reader, System.IO.BinaryWriter writer, CancellationToken token, long byteLength, long offsetStart, long offsetEnd, long timeout = -1, int BUFFER_SIZE = 8192)
        {
            bool foundAll = true;
            int readBytes = 0;
            long totalBytesRead = 0;
            int bufferLength = BUFFER_SIZE;
            byte[] buffer = new byte[bufferLength];

            long totalBytesNotToRead = offsetStart + offsetEnd;
            long totalBytesToRead = byteLength - totalBytesNotToRead;

            // Only if data needs to be found.
            if (byteLength > 0)
            {
                // Start a new timeout clock.
                Custom.TimeoutClock timeoutClock = new Custom.TimeoutClock((int)timeout);

                // Offset starting point within the stream.
                reader.BaseStream.Seek(offsetStart, System.IO.SeekOrigin.Begin);

                // Read all the data in the source stream and
                // write the data to the destination stream.
                do
                {
                    // Determine the number of bytes to read
                    // from the byteLength and buffer_size.
                    // Can not read more then byteLength.
                    if (byteLength <= bufferLength)
                    {
                        // Look for current total bytes read and number left.
                        long left = byteLength - totalBytesRead;
                        if (left <= bufferLength)
                            bufferLength = (int)left;
                        else
                            bufferLength = (int)byteLength;
                    }
                    else
                    {
                        // Look for current total bytes read and number left.
                        long left = byteLength - totalBytesRead;
                        if (left <= bufferLength)
                            bufferLength = (int)left;
                    }

                    // Read the data and then write the data.
                    readBytes = reader.Read(buffer, 0, bufferLength);
                    totalBytesRead += readBytes;

                    // If data exists.
                    if (readBytes > 0)
                    {
                        // Each time data is read reset the timeout.
                        timeoutClock.Reset();

                        // If all the bytes have been read in the first iteration.
                        if (readBytes >= totalBytesToRead)
                            writer.Write(buffer, 0, (int)totalBytesToRead);
                        else
                        {
                            // If the total number of bytes has been read
                            if (totalBytesRead >= totalBytesToRead)
                            {
                                // Calculate the last set of bytes to read.
                                long extraBytesNotToRead = totalBytesRead - totalBytesToRead;
                                long lastBytesToRead = readBytes - extraBytesNotToRead;

                                // If no more bytes are to be read.
                                if ((totalBytesRead - totalBytesToRead) > 0)
                                    writer.Write(buffer, 0, (int)lastBytesToRead);
                                else
                                    // Keep on reading more bytes.
                                    writer.Write(buffer, 0, readBytes);
                            }
                            else
                                // Keep on reading more bytes.
                                writer.Write(buffer, 0, readBytes);
                        }
                    }
                    else
                        SpinWaitHandler(reader, timeoutClock);

                    // If the timeout has bee reached then
                    // break from the loop.
                    if (timeoutClock.IsComplete())
                    {
                        foundAll = false;
                        break;
                    }

                    // Cancellation has been requested for this token.
                    if (token.IsCancellationRequested)
                    {
                        foundAll = false;
                        break;
                    }
                }
                while (totalBytesRead < totalBytesToRead);
            }

            // True if all data has been found.
            return foundAll;
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="numberToRead">The total number of bytes to read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsyncEx(System.IO.BinaryReader reader, System.IO.BinaryWriter writer, long offsetStart, long numberToRead, long timeout = -1, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStreamEx(reader, writer, offsetStart, numberToRead, timeout, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="numberToRead">The total number of bytes to read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStreamEx(System.IO.BinaryReader reader, System.IO.BinaryWriter writer, long offsetStart, long numberToRead, long timeout = -1, int BUFFER_SIZE = 8192)
        {
            return CopyStreamEx(reader, writer, CancellationToken.None, offsetStart, numberToRead, timeout, BUFFER_SIZE);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="numberToRead">The total number of bytes to read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsyncEx(System.IO.BinaryReader reader, System.IO.BinaryWriter writer, CancellationToken token, long offsetStart, long numberToRead, long timeout = -1, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStreamEx(reader, writer, token, offsetStart, numberToRead, timeout, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="numberToRead">The total number of bytes to read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStreamEx(System.IO.BinaryReader reader, System.IO.BinaryWriter writer, CancellationToken token, long offsetStart, long numberToRead, long timeout = -1, int BUFFER_SIZE = 8192)
        {
            bool foundAll = true;
            int readBytes = 0;
            long totalBytesRead = 0;
            int bufferLength = BUFFER_SIZE;
            byte[] buffer = new byte[bufferLength];

            long totalBytesToRead = numberToRead;

            // Only if data needs to be found.
            if (numberToRead > 0)
            {
                // Start a new timeout clock.
                Custom.TimeoutClock timeoutClock = new Custom.TimeoutClock((int)timeout);

                // Offset starting point within the stream.
                reader.BaseStream.Seek(offsetStart, System.IO.SeekOrigin.Begin);

                // Read all the data in the source stream and
                // write the data to the destination stream.
                do
                {
                    // Determine the number of bytes to read
                    // from the byteLength and buffer_size.
                    // Can not read more then byteLength.
                    if (numberToRead <= bufferLength)
                    {
                        // Look for current total bytes read and number left.
                        long left = numberToRead - totalBytesRead;
                        if (left <= bufferLength)
                            bufferLength = (int)left;
                        else
                            bufferLength = (int)numberToRead;
                    }
                    else
                    {
                        // Look for current total bytes read and number left.
                        long left = numberToRead - totalBytesRead;
                        if (left <= bufferLength)
                            bufferLength = (int)left;
                    }

                    // Read the data and then write the data.
                    readBytes = reader.Read(buffer, 0, bufferLength);
                    totalBytesRead += readBytes;

                    // If data exists.
                    if (readBytes > 0)
                    {
                        // Each time data is read reset the timeout.
                        timeoutClock.Reset();

                        // If all the bytes have been read in the first iteration.
                        if (readBytes >= totalBytesToRead)
                            writer.Write(buffer, 0, (int)totalBytesToRead);
                        else
                        {
                            // If the total number of bytes has been read
                            if (totalBytesRead >= totalBytesToRead)
                            {
                                // Calculate the last set of bytes to read.
                                long extraBytesNotToRead = totalBytesRead - totalBytesToRead;
                                long lastBytesToRead = readBytes - extraBytesNotToRead;

                                // If no more bytes are to be read.
                                if ((totalBytesRead - totalBytesToRead) > 0)
                                    writer.Write(buffer, 0, (int)lastBytesToRead);
                                else
                                    // Keep on reading more bytes.
                                    writer.Write(buffer, 0, readBytes);
                            }
                            else
                                // Keep on reading more bytes.
                                writer.Write(buffer, 0, readBytes);
                        }
                    }
                    else
                        SpinWaitHandler(reader, timeoutClock);

                    // If the timeout has bee reached then
                    // break from the loop.
                    if (timeoutClock.IsComplete())
                    {
                        foundAll = false;
                        break;
                    }

                    // Cancellation has been requested for this token.
                    if (token.IsCancellationRequested)
                    {
                        foundAll = false;
                        break;
                    }
                }
                while (totalBytesRead < totalBytesToRead);
            }

            // True if all data has been found.
            return foundAll;
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsync(System.IO.TextReader reader, System.IO.TextWriter writer, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStream(reader, writer, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStream(System.IO.TextReader reader, System.IO.TextWriter writer, int BUFFER_SIZE = 8192)
        {
            return CopyStream(reader, writer, CancellationToken.None, BUFFER_SIZE);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsync(System.IO.TextReader reader, System.IO.TextWriter writer, CancellationToken token, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStream(reader, writer, token, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStream(System.IO.TextReader reader, System.IO.TextWriter writer, CancellationToken token, int BUFFER_SIZE = 8192)
        {
            bool foundAll = true;
            int readBytes = 0;
            int bufferLength = BUFFER_SIZE;
            char[] buffer = new char[bufferLength];

            // Read all the data in the source stream and
            // write the data to the destination stream.
            do
            {
                // Read the data and then write the data.
                readBytes = reader.Read(buffer, 0, bufferLength);

                // If data exists.
                if (readBytes > 0)
                {
                    // Each time data is read reset the timeout.
                    writer.Write(buffer, 0, readBytes);
                }

                // Cancellation has been requested for this token.
                if (token.IsCancellationRequested)
                {
                    foundAll = false;
                    break;
                }
            }
            while (readBytes != 0);

            // True if all data has been found.
            return foundAll;
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of reader bytes). Waits until all bytes are read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsync(System.IO.TextReader reader, System.IO.TextWriter writer, long byteLength, long timeout = -1, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStream(reader, writer, byteLength, timeout, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of reader bytes). Waits until all bytes are read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStream(System.IO.TextReader reader, System.IO.TextWriter writer, long byteLength, long timeout = -1, int BUFFER_SIZE = 8192)
        {
            return CopyStream(reader, writer, CancellationToken.None, byteLength, timeout, BUFFER_SIZE);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of reader bytes). Waits until all bytes are read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsync(System.IO.TextReader reader, System.IO.TextWriter writer, CancellationToken token, long byteLength, long timeout = -1, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStream(reader, writer, token, byteLength, timeout, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of reader bytes). Waits until all bytes are read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStream(System.IO.TextReader reader, System.IO.TextWriter writer, CancellationToken token, long byteLength, long timeout = -1, int BUFFER_SIZE = 8192)
        {
            bool foundAll = true;
            int readBytes = 0;
            long totalBytesRead = 0;
            int bufferLength = BUFFER_SIZE;
            char[] buffer = new char[bufferLength];

            // Only if data needs to be found.
            if (byteLength > 0)
            {
                // Start a new timeout clock.
                Custom.TimeoutClock timeoutClock = new Custom.TimeoutClock((int)timeout);

                // Read all the data in the source stream and
                // write the data to the destination stream.
                do
                {
                    // Determine the number of bytes to read
                    // from the byteLength and buffer_size.
                    // Can not read more then byteLength.
                    if (byteLength <= bufferLength)
                    {
                        // Look for current total bytes read and number left.
                        long left = byteLength - totalBytesRead;
                        if (left <= bufferLength)
                            bufferLength = (int)left;
                        else
                            bufferLength = (int)byteLength;
                    }
                    else
                    {
                        // Look for current total bytes read and number left.
                        long left = byteLength - totalBytesRead;
                        if (left <= bufferLength)
                            bufferLength = (int)left;
                    }

                    // Read the data and then write the data.
                    readBytes = reader.Read(buffer, 0, bufferLength);

                    // If data exists.
                    if (readBytes > 0)
                    {
                        // Each time data is read reset the timeout.
                        timeoutClock.Reset();
                        totalBytesRead += readBytes;
                        writer.Write(buffer, 0, readBytes);
                    }

                    // If the timeout has bee reached then
                    // break from the loop.
                    if (timeoutClock.IsComplete())
                    {
                        foundAll = false;
                        break;
                    }

                    // Cancellation has been requested for this token.
                    if (token.IsCancellationRequested)
                    {
                        foundAll = false;
                        break;
                    }
                }
                while (totalBytesRead < byteLength);
            }

            // True if all data has been found.
            return foundAll;
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of reader bytes). Waits until all bytes are read.</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="offsetEnd">The number of bytes to skip from the end of the stream (number of bytes not to read from the bottom).</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsync(System.IO.TextReader reader, System.IO.TextWriter writer, long byteLength, long offsetStart, long offsetEnd, long timeout = -1, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStream(reader, writer, byteLength, offsetStart, offsetEnd, timeout, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of reader bytes). Waits until all bytes are read.</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="offsetEnd">The number of bytes to skip from the end of the stream (number of bytes not to read from the bottom).</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStream(System.IO.TextReader reader, System.IO.TextWriter writer, long byteLength, long offsetStart, long offsetEnd, long timeout = -1, int BUFFER_SIZE = 8192)
        {
            return CopyStream(reader, writer, CancellationToken.None, byteLength, offsetStart, offsetEnd, timeout, BUFFER_SIZE);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of reader bytes). Waits until all bytes are read.</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="offsetEnd">The number of bytes to skip from the end of the stream (number of bytes not to read from the bottom).</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsync(System.IO.TextReader reader, System.IO.TextWriter writer, CancellationToken token, long byteLength, long offsetStart, long offsetEnd, long timeout = -1, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStream(reader, writer, token, byteLength, offsetStart, offsetEnd, timeout, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of reader bytes). Waits until all bytes are read.</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="offsetEnd">The number of bytes to skip from the end of the stream (number of bytes not to read from the bottom).</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStream(System.IO.TextReader reader, System.IO.TextWriter writer, CancellationToken token, long byteLength, long offsetStart, long offsetEnd, long timeout = -1, int BUFFER_SIZE = 8192)
        {
            bool foundAll = true;
            int readBytes = 0;
            long totalBytesRead = 0;
            int bufferLength = BUFFER_SIZE;
            char[] buffer = new char[bufferLength];

            long totalBytesNotToRead = offsetStart + offsetEnd;
            long totalBytesToRead = byteLength - totalBytesNotToRead;

            // Only if data needs to be found.
            if (byteLength > 0)
            {
                // Start a new timeout clock.
                Custom.TimeoutClock timeoutClock = new Custom.TimeoutClock((int)timeout);

                // Read all the data in the source stream and
                // write the data to the destination stream.
                do
                {
                    // Determine the number of bytes to read
                    // from the byteLength and buffer_size.
                    // Can not read more then byteLength.
                    if (byteLength <= bufferLength)
                    {
                        // Look for current total bytes read and number left.
                        long left = byteLength - totalBytesRead;
                        if (left <= bufferLength)
                            bufferLength = (int)left;
                        else
                            bufferLength = (int)byteLength;
                    }
                    else
                    {
                        // Look for current total bytes read and number left.
                        long left = byteLength - totalBytesRead;
                        if (left <= bufferLength)
                            bufferLength = (int)left;
                    }

                    // Read the data and then write the data.
                    readBytes = reader.Read(buffer, 0, bufferLength);
                    totalBytesRead += readBytes;

                    // If data exists.
                    if (readBytes > 0)
                    {
                        // Each time data is read reset the timeout.
                        timeoutClock.Reset();

                        // If all the bytes have been read in the first iteration.
                        if (readBytes >= totalBytesToRead)
                            writer.Write(buffer, 0, (int)totalBytesToRead);
                        else
                        {
                            // If the total number of bytes has been read
                            if (totalBytesRead >= totalBytesToRead)
                            {
                                // Calculate the last set of bytes to read.
                                long extraBytesNotToRead = totalBytesRead - totalBytesToRead;
                                long lastBytesToRead = readBytes - extraBytesNotToRead;

                                // If no more bytes are to be read.
                                if ((totalBytesRead - totalBytesToRead) > 0)
                                    writer.Write(buffer, 0, (int)lastBytesToRead);
                                else
                                    // Keep on reading more bytes.
                                    writer.Write(buffer, 0, readBytes);
                            }
                            else
                                // Keep on reading more bytes.
                                writer.Write(buffer, 0, readBytes);
                        }
                    }

                    // If the timeout has bee reached then
                    // break from the loop.
                    if (timeoutClock.IsComplete())
                    {
                        foundAll = false;
                        break;
                    }

                    // Cancellation has been requested for this token.
                    if (token.IsCancellationRequested)
                    {
                        foundAll = false;
                        break;
                    }
                }
                while (totalBytesRead < totalBytesToRead);
            }

            // True if all data has been found.
            return foundAll;
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="numberToRead">The total number of bytes to read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsyncEx(System.IO.TextReader reader, System.IO.TextWriter writer, long offsetStart, long numberToRead, long timeout = -1, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStreamEx(reader, writer, offsetStart, numberToRead, timeout, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="numberToRead">The total number of bytes to read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStreamEx(System.IO.TextReader reader, System.IO.TextWriter writer, long offsetStart, long numberToRead, long timeout = -1, int BUFFER_SIZE = 8192)
        {
            return CopyStreamEx(reader, writer, CancellationToken.None, offsetStart, numberToRead, timeout, BUFFER_SIZE);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="numberToRead">The total number of bytes to read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="exceptionCallback">The exception callback.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>The operational task.</returns>
        public static Task<bool> CopyStreamAsyncEx(System.IO.TextReader reader, System.IO.TextWriter writer, CancellationToken token, long offsetStart, long numberToRead, long timeout = -1, Action<Exception> exceptionCallback = null, int BUFFER_SIZE = 8192)
        {
            // Create a new task.
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    return CopyStreamEx(reader, writer, token, offsetStart, numberToRead, timeout, BUFFER_SIZE);
                }
                catch (Exception ex)
                {
                    if (exceptionCallback != null)
                        exceptionCallback(ex);
                    return false;
                }

            }, CancellationToken.None);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="numberToRead">The total number of bytes to read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="BUFFER_SIZE">The copy buffer size.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public static bool CopyStreamEx(System.IO.TextReader reader, System.IO.TextWriter writer, CancellationToken token, long offsetStart, long numberToRead, long timeout = -1, int BUFFER_SIZE = 8192)
        {
            bool foundAll = true;
            int readBytes = 0;
            long totalBytesRead = 0;
            int totalSkippedBytes = 0;
            int bufferLength = BUFFER_SIZE;
            char[] buffer = new char[bufferLength];

            long totalBytesToRead = numberToRead;

            // Only if data needs to be found.
            if (numberToRead > 0)
            {
                // Start a new timeout clock.
                Custom.TimeoutClock timeoutClock = new Custom.TimeoutClock((int)timeout);

                // Offset starting point within the stream.
                // read the first set of bytes.
                do
                {
                    // Determine the number of bytes to read
                    // from the byteLength and buffer_size.
                    // Can not read more then byteLength.
                    if (numberToRead <= bufferLength)
                    {
                        // Look for current total bytes read and number left.
                        long left = numberToRead - totalBytesRead;
                        if (left <= bufferLength)
                            bufferLength = (int)left;
                        else
                            bufferLength = (int)numberToRead;
                    }
                    else
                    {
                        // Look for current total bytes read and number left.
                        long left = numberToRead - totalBytesRead;
                        if (left <= bufferLength)
                            bufferLength = (int)left;
                    }

                    // Read the data and then write the data.
                    readBytes = reader.Read(buffer, 0, 1);

                    // If data exists.
                    if (readBytes > 0)
                    {
                        // Each time data is read reset the timeout.
                        timeoutClock.Reset();
                        totalSkippedBytes += readBytes;
                    }

                    // If the timeout has bee reached then
                    // break from the loop.
                    if (timeoutClock.IsComplete())
                    {
                        foundAll = false;
                        break;
                    }
                }
                while (totalSkippedBytes < offsetStart);

                // Each time data is read reset the timeout.
                timeoutClock.Reset();

                // Read all the data in the source stream and
                // write the data to the destination stream.
                do
                {
                    // Read the data and then write the data.
                    readBytes = reader.Read(buffer, 0, bufferLength);
                    totalBytesRead += readBytes;

                    // If data exists.
                    if (readBytes > 0)
                    {
                        // Each time data is read reset the timeout.
                        timeoutClock.Reset();

                        // If all the bytes have been read in the first iteration.
                        if (readBytes >= totalBytesToRead)
                            writer.Write(buffer, 0, (int)totalBytesToRead);
                        else
                        {
                            // If the total number of bytes has been read
                            if (totalBytesRead >= totalBytesToRead)
                            {
                                // Calculate the last set of bytes to read.
                                long extraBytesNotToRead = totalBytesRead - totalBytesToRead;
                                long lastBytesToRead = readBytes - extraBytesNotToRead;

                                // If no more bytes are to be read.
                                if ((totalBytesRead - totalBytesToRead) > 0)
                                    writer.Write(buffer, 0, (int)lastBytesToRead);
                                else
                                    // Keep on reading more bytes.
                                    writer.Write(buffer, 0, readBytes);
                            }
                            else
                                // Keep on reading more bytes.
                                writer.Write(buffer, 0, readBytes);
                        }
                    }

                    // If the timeout has bee reached then
                    // break from the loop.
                    if (timeoutClock.IsComplete())
                    {
                        foundAll = false;
                        break;
                    }

                    // Cancellation has been requested for this token.
                    if (token.IsCancellationRequested)
                    {
                        foundAll = false;
                        break;
                    }
                }
                while (totalBytesRead < totalBytesToRead);
            }

            // True if all data has been found.
            return foundAll;
        }

        /// <summary>
        /// Spin wait until data is avaiable or timed out.
        /// </summary>
        /// <param name="source">The source stream to check.</param>
        /// <param name="timeoutClock">The time to check.</param>
        private static void SpinWaitHandler(System.IO.Stream source, Custom.TimeoutClock timeoutClock)
        {
            bool exitIndicator = false;

            // Create the tasks.
            Task[] tasks = new Task[1];

            // Poller task.
            Task poller = Task.Factory.StartNew(() =>
            {
                // Create a new spin wait.
                SpinWait sw = new SpinWait();

                // Action to perform.
                while (!exitIndicator)
                {
                    // The NextSpinWillYield property returns true if 
                    // calling sw.SpinOnce() will result in yielding the 
                    // processor instead of simply spinning. 
                    if (sw.NextSpinWillYield)
                    {
                        if (timeoutClock.IsComplete() || source.Length > 0) exitIndicator = true;
                    }
                    sw.SpinOnce();
                }
            });

            // Assign the listener task.
            tasks[0] = poller;

            // Wait for all tasks to complete.
            Task.WaitAll(tasks);

            // For each task.
            foreach (Task item in tasks)
            {
                try
                {
                    // Release the resources.
                    item.Dispose();
                }
                catch { }
            }
            tasks = null;
        }

        /// <summary>
        /// Spin wait until data is avaiable or timed out.
        /// </summary>
        /// <param name="source">The source stream to check.</param>
        /// <param name="timeoutClock">The time to check.</param>
        private static void SpinWaitHandler(System.IO.BinaryReader source, Custom.TimeoutClock timeoutClock)
        {
            bool exitIndicator = false;

            // Create the tasks.
            Task[] tasks = new Task[1];

            // Poller task.
            Task poller = Task.Factory.StartNew(() =>
            {
                // Create a new spin wait.
                SpinWait sw = new SpinWait();

                // Action to perform.
                while (!exitIndicator)
                {
                    // The NextSpinWillYield property returns true if 
                    // calling sw.SpinOnce() will result in yielding the 
                    // processor instead of simply spinning. 
                    if (sw.NextSpinWillYield)
                    {
                        if (timeoutClock.IsComplete() || source.BaseStream.Length > 0) exitIndicator = true;
                    }
                    sw.SpinOnce();
                }
            });

            // Assign the listener task.
            tasks[0] = poller;

            // Wait for all tasks to complete.
            Task.WaitAll(tasks);

            // For each task.
            foreach (Task item in tasks)
            {
                try
                {
                    // Release the resources.
                    item.Dispose();
                }
                catch { }
            }
            tasks = null;
        }
	}
}
