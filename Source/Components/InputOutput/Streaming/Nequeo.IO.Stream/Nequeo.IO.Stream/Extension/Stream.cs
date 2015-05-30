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
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

using Nequeo.IO.Stream;
using Nequeo.Threading.Extension;
using Nequeo.Extension;
using Nequeo.IO.Compression;

namespace Nequeo.IO.Stream.Extension
{
    /// <summary>
    /// Extension methods for asynchronously working with streams.
    /// </summary>
    public static class StreamExtensions
    {
        private const int BUFFER_SIZE = 0x2000;

        /// <summary>
        /// Decompress the stream.
        /// </summary>
        /// <param name="source">The sources stream.</param>
        /// <param name="method">The compression method.</param>
        /// <returns>The bytes decompressed.</returns>
        public static byte[] Decompress(this System.IO.Stream source, Compression.CompressionAlgorithmStreaming method)
        {
            using (MemoryStream unzipped = new MemoryStream())
            {
                Compression.Compresss.Decompress(unzipped, source, method);
                unzipped.Close();
                return unzipped.ToArray();
            }
        }

        /// <summary>
        /// Compress the stream.
        /// </summary>
        /// <param name="source">The sources stream.</param>
        /// <param name="method">The compression method.</param>
        /// <returns>The bytes compressed.</returns>
        public static byte[] Compress(this System.IO.Stream source, Compression.CompressionAlgorithmStreaming method)
        {
            using (MemoryStream unzipped = new MemoryStream())
            {
                Compression.Compresss.Compress(source, unzipped, method);
                unzipped.Close();
                return unzipped.ToArray();
            }
        }

        /// <summary>
        /// Write the array of bytes to the current stream.
        /// </summary>
        /// <param name="stream">The current stream.</param>
        /// <param name="bytes">The array of bytes to write.</param>
        public static void WriteBytes(this System.IO.Stream stream, byte[] bytes)
        {
            using (MemoryStream input = new MemoryStream(bytes))
                input.CopyTo(stream);
        }

        /// <summary>
        /// Read the bytes from the stream.
        /// </summary>
        /// <param name="stream">The current stream.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>The bytes read.</returns>
        public static byte[] ReadBytes(this System.IO.Stream stream, int length)
        {
            return stream.ReadBytes(new byte[length], 0, length);
        }

        /// <summary>
        /// Read the bytes from the stream.
        /// </summary>
        /// <param name="source">The current stream.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <param name="completed">True if completed and did not timeout; else false.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely.</param>
        /// <returns>The bytes read.</returns>
        public static byte[] ReadBytesTimer(this System.IO.Stream source, int length, out bool completed, long timeout = -1)
        {
            int readBytes = 0;
            long totalBytesRead = 0;
            int bufferLength = length;
            byte[] buffer = new byte[bufferLength];

            // If completed.
            completed = true;

            using (MemoryStream destination = new MemoryStream())
            {
                // Only if data needs to be found.
                if (length > 0)
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
                        if (length <= bufferLength)
                        {
                            // Look for current total bytes read and number left.
                            long left = length - totalBytesRead;
                            if (left <= bufferLength)
                                bufferLength = (int)left;
                            else
                                bufferLength = (int)length;
                        }
                        else
                        {
                            // Look for current total bytes read and number left.
                            long left = length - totalBytesRead;
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

                        // If the timeout has been reached then
                        // break from the loop.
                        if (timeoutClock.IsComplete())
                        {
                            completed = false;
                            break;
                        }
                    }
                    while (totalBytesRead < length);
                }

                // Return the data as bytes.
                destination.Close();
                return destination.ToArray();
            }
        }

        /// <summary>
        /// Read the bytes from the stream.
        /// </summary>
        /// <param name="stream">The current stream.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <param name="bufferLength">The buffer length.</param>
        /// <returns>The bytes read.</returns>
        public static byte[] ReadBytes(this System.IO.Stream stream, int length, int bufferLength)
        {
            using (MemoryStream res = new MemoryStream())
            {
                var cnt = length / bufferLength;
                var rem = (int)(length % bufferLength);

                var buff = new byte[bufferLength];
                var end = false;
                for (long i = 0; i < cnt; i++)
                {
                    if (!stream.ReadBytes(buff, 0, bufferLength, res))
                    {
                        end = true;
                        break;
                    }
                }

                if (!end && rem > 0)
                    stream.ReadBytes(new byte[rem], 0, rem, res);

                res.Close();
                return res.ToArray();
            }
        }

        /// <summary>
        /// Read the bytes from the stream.
        /// </summary>
        /// <param name="stream">The current stream.</param>
        /// <param name="buffer">The buffer of bytes.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <param name="destination">The destination stream.</param>
        /// <returns>The bytes read.</returns>
        public static bool ReadBytes(this System.IO.Stream stream, byte[] buffer, int offset, int length, System.IO.Stream destination)
        {
            var bytes = stream.ReadBytes(buffer, offset, length);
            var len = bytes.Length;
            destination.Write(bytes, 0, len);

            return len == offset + length;
        }

        /// <summary>
        /// Read the bytes from the stream.
        /// </summary>
        /// <param name="stream">The current stream.</param>
        /// <param name="buffer">An array of bytes to be filled by the read operation.</param>
        /// <param name="offset">The offset at which data should be stored.</param>
        /// <param name="length">The number of bytes to be read.</param>
        /// <returns>The bytes read.</returns>
        public static byte[] ReadBytes(this System.IO.Stream stream, byte[] buffer, int offset, int length)
        {
            var len = 0;
            try
            {
                len = stream.Read(buffer, offset, length);
                if (len < 1)
                    return buffer.SubArray(0, offset);

                while (len < length)
                {
                    var readLen = stream.Read(buffer, offset + len, length - len);
                    if (readLen < 1)
                        break;

                    len += readLen;
                }
            }
            catch
            {
            }

            return len < length
                   ? buffer.SubArray(0, offset + len)
                   : buffer;
        }

        /// <summary>
        /// Read from a stream asynchronously.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <param name="completed">The completed action.</param>
        /// <param name="error">The error action.</param>
        public static void ReadAsync(this System.IO.Stream stream, int length, Action<byte[]> completed, Action<Exception> error)
        {
            var buff = new byte[length];
            stream.BeginRead(
              buff,
              0,
              length,
              ar =>
              {
                  try
                  {
                      byte[] bytes = null;
                      try
                      {
                          var len = stream.EndRead(ar);
                          bytes = len < 1
                                  ? new byte[0]
                                  : len < length
                                    ? stream.ReadBytes(buff, len, length - len)
                                    : buff;
                      }
                      catch
                      {
                          bytes = new byte[0];
                      }

                      if (completed != null)
                          completed(bytes);
                  }
                  catch (Exception ex)
                  {
                      if (error != null)
                          error(ex);
                  }
              },
              null);
        }

        /// <summary>
        /// Read from a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <param name="completed">The completed action.</param>
        /// <param name="error">The error action.</param>
        public static void Read(this System.IO.Stream stream, int length, Action<byte[]> completed, Action<Exception> error)
        {
            var buff = new byte[length];

            try
            {
                byte[] bytes = null;

                try
                {
                    // Read the data from the stream.
                    int len = stream.Read(buff, 0, length);
                    bytes = len < 1
                                  ? new byte[0]
                                  : len < length
                                    ? stream.ReadBytes(buff, len, length - len)
                                    : buff;
                }
                catch
                {
                    bytes = new byte[0];
                }

                if (completed != null)
                    completed(bytes);
            }
            catch (Exception ex)
            {
                if (error != null)
                    error(ex);
            }
        }

        /// <summary>Read from a stream asynchronously.</summary>
        /// <param name="stream">The stream.</param>
        /// <param name="buffer">An array of bytes to be filled by the read operation.</param>
        /// <param name="offset">The offset at which data should be stored.</param>
        /// <param name="count">The number of bytes to be read.</param>
        /// <returns>A Task containing the number of bytes read.</returns>
        public static Task<int> ReadAsync(
            this System.IO.Stream stream, byte[] buffer, int offset, int count)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            return Task<int>.Factory.FromAsync(
                stream.BeginRead, stream.EndRead,
                buffer, offset, count, stream /* object state */);
        }

        /// <summary>Write to a stream asynchronously.</summary>
        /// <param name="stream">The stream.</param>
        /// <param name="buffer">An array of bytes to be written.</param>
        /// <param name="offset">The offset from which data should be read to be written.</param>
        /// <param name="count">The number of bytes to be written.</param>
        /// <returns>A Task representing the completion of the asynchronous operation.</returns>
        public static Task WriteAsync(
            this System.IO.Stream stream, byte[] buffer, int offset, int count)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            return Task.Factory.FromAsync(
                stream.BeginWrite, stream.EndWrite,
                buffer, offset, count, stream);
        }

        /// <summary>Reads the contents of the stream asynchronously.</summary>
        /// <param name="stream">The stream.</param>
        /// <returns>A Task representing the contents of the file in bytes.</returns>
        public static Task<byte[]> ReadAllBytesAsync(this System.IO.Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            // Create a MemoryStream to store the data read from the input stream
            int initialCapacity = stream.CanSeek ? (int)stream.Length : 0;
            var readData = new MemoryStream(initialCapacity);

            // Copy from the source stream to the memory stream and return the copied data
            return stream.CopyStreamToStreamAsync(readData).ContinueWith(t =>
            {
                t.PropagateExceptions();
                return readData.ToArray();
            });
        }

        /// <summary>Read the content of the stream, yielding its data in buffers to the provided delegate.</summary>
        /// <param name="stream">The stream.</param>
        /// <param name="bufferSize">The size of the buffers to use.</param>
        /// <param name="bufferAvailable">The delegate to be called when a new buffer is available.</param>
        /// <returns>A Task that represents the completion of the asynchronous operation.</returns>
        public static Task ReadBuffersAsync(this System.IO.Stream stream, int bufferSize, Action<byte[], int> bufferAvailable)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (bufferSize < 1) throw new ArgumentOutOfRangeException("bufferSize");
            if (bufferAvailable == null) throw new ArgumentNullException("bufferAvailable");

            // Read from the stream over and over, handing the buffers off to the bufferAvailable delegate
            // as they're available.  Delegate invocation will be serialized.
            return Task.Factory.Iterate(
                ReadIterator(stream, bufferSize, bufferAvailable));
        }

        /// <summary>
        /// Creates an enumerable to be used with TaskFactoryExtensions.Iterate that reads data
        /// from an input stream and passes it to a user-provided delegate.
        /// </summary>
        /// <param name="input">The source stream.</param>
        /// <param name="bufferSize">The size of the buffers to be used.</param>
        /// <param name="bufferAvailable">
        /// A delegate to be invoked when a buffer is available (provided the
        /// buffer and the number of bytes in the buffer starting at offset 0.
        /// </param>
        /// <returns>An enumerable containing yielded tasks from the operation.</returns>
        private static IEnumerable<Task> ReadIterator(System.IO.Stream input, int bufferSize, Action<byte[], int> bufferAvailable)
        {
            // Create a buffer that will be used over and over
            var buffer = new byte[bufferSize];

            // Until there's no more data
            while (true)
            {
                // Asynchronously read a buffer and yield until the operation completes
                var readTask = input.ReadAsync(buffer, 0, buffer.Length);
                yield return readTask;

                // If there's no more data in the stream, we're done.
                if (readTask.Result <= 0) break;

                // Otherwise, hand the data off to the delegate
                bufferAvailable(buffer, readTask.Result);
            }
        }

        /// <summary>Copies the contents of a stream to a file, asynchronously.</summary>
        /// <param name="source">The source stream.</param>
        /// <param name="destinationPath">The path to the destination file.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        public static Task CopyStreamToFileAsync(this System.IO.Stream source, string destinationPath)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (destinationPath == null) throw new ArgumentNullException("destinationPath");

            // Open the output file for writing
            var destinationStream = FileAsynchronous.OpenWrite(destinationPath);

            // Copy the source to the destination stream, then close the output file.
            return CopyStreamToStreamAsync(source, destinationStream).ContinueWith(t =>
            {
                var e = t.Exception;
                destinationStream.Close();
                if (e != null) throw e;
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>Copies the contents of one stream to another, asynchronously.</summary>
        /// <param name="source">The source stream.</param>
        /// <param name="destination">The destination stream.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <returns>A Task that represents the completion of the asynchronous operation.</returns>
        public static Task CopyStreamToStreamAsync(this System.IO.Stream source, System.IO.Stream destination, long timeout = -1)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (destination == null) throw new ArgumentNullException("destination");
            return Task.Factory.Iterate(
                CopyStreamIterator(source, destination, timeout));
        }

        /// <summary>
        /// Creates an enumerable to be used with TaskFactoryExtensions.Iterate that copies data from one
        /// stream to another.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <returns>An enumerable containing yielded tasks from the copy operation.</returns>
        private static IEnumerable<Task> CopyStreamIterator(System.IO.Stream input, System.IO.Stream output, long timeout = -1)
        {
            // Create two buffers.  One will be used for the current read operation and one for the current
            // write operation.  We'll continually swap back and forth between them.
            byte[][] buffers = new byte[2][] { new byte[BUFFER_SIZE], new byte[BUFFER_SIZE] };
            int filledBufferNum = 0;
            Task writeTask = null;

            // Until there's no more data to be read
            while (true)
            {
                // Read from the input asynchronously
                var readTask = input.ReadAsync(buffers[filledBufferNum], 0, buffers[filledBufferNum].Length);

                // If we have no pending write operations, just yield until the read operation has
                // completed.  If we have both a pending read and a pending write, yield until both the read
                // and the write have completed.
                if (writeTask == null)
                {
                    yield return readTask;
                    readTask.Wait(); // propagate any exception that may have occurred
                }
                else
                {
                    var tasks = new[] { readTask, writeTask };
                    yield return Task.Factory.WhenAll(tasks);
                    Task.WaitAll(tasks); // propagate any exceptions that may have occurred
                }

                // If no data was read, nothing more to do.
                if (readTask.Result <= 0) break;

                // Otherwise, write the written data out to the file
                writeTask = output.WriteAsync(buffers[filledBufferNum], 0, readTask.Result);

                // Swap buffers
                filledBufferNum ^= 1;
            }
        }

        /// <summary>
        /// Creates an enumerable to be used with TaskFactoryExtensions.Iterate that copies data from one
        /// stream to another.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <returns>An enumerable containing yielded tasks from the copy operation.</returns>
        private static IEnumerable<Task> CopyStreamIteratorUnicodeToUtf8Encoding(System.IO.Stream input, System.IO.Stream output)
        {
            // Create two buffers.  One will be used for the current read operation and one for the current
            // write operation.  We'll continually swap back and forth between them.
            byte[][] buffers = new byte[2][] { new byte[BUFFER_SIZE], new byte[BUFFER_SIZE] };
            int filledBufferNum = 0;
            Task writeTask = null;

            // Until there's no more data to be read
            while (true)
            {
                // Read from the input asynchronously
                var readTask = input.ReadAsync(buffers[filledBufferNum], 0, buffers[filledBufferNum].Length);

                // If we have no pending write operations, just yield until the read operation has
                // completed.  If we have both a pending read and a pending write, yield until both the read
                // and the write have completed.
                if (writeTask == null)
                {
                    yield return readTask;
                    readTask.Wait(); // propagate any exception that may have occurred
                }
                else
                {
                    var tasks = new[] { readTask, writeTask };
                    yield return Task.Factory.WhenAll(tasks);
                    Task.WaitAll(tasks); // propagate any exceptions that may have occurred
                }

                // If no data was read, nothing more to do.
                if (readTask.Result <= 0) break;

                // Otherwise, write the written data out to the file
                writeTask = output.WriteAsyncUnicodeToUtf8Encoding(buffers[filledBufferNum], 0, readTask.Result);

                // Swap buffers
                filledBufferNum ^= 1;
            }
        }

        /// <summary>Write to a stream asynchronously.</summary>
        /// <param name="stream">The stream.</param>
        /// <param name="buffer">An array of bytes to be written.</param>
        /// <param name="offset">The offset from which data should be read to be written.</param>
        /// <param name="count">The number of bytes to be written.</param>
        /// <returns>A Task representing the completion of the asynchronous operation.</returns>
        private static Task WriteAsyncUnicodeToUtf8Encoding(
            this System.IO.Stream stream, byte[] buffer, int offset, int count)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            // Create two different encodings.
            System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
            System.Text.Encoding unicode = System.Text.Encoding.Unicode;

            // Convert the encoded types
            byte[] convertedBytes = System.Text.Encoding.Convert(unicode, utf8, buffer, offset, count);

            // Return the asyn result.
            return Task.Factory.FromAsync(
                stream.BeginWrite, stream.EndWrite,
                convertedBytes, offset, convertedBytes.Length, stream);
        }

        /// <summary>Copies the contents of one stream to another, asynchronously, Unicode to Utf8 encoding.</summary>
        /// <param name="source">The source stream.</param>
        /// <param name="destination">The destination stream.</param>
        /// <returns>A Task that represents the completion of the asynchronous operation.</returns>
        public static Task CopyStreamToStreamAsyncUnicodeToUtf8(this System.IO.Stream source, System.IO.Stream destination)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (destination == null) throw new ArgumentNullException("destination");
            return Task.Factory.Iterate(
                CopyStreamIteratorUnicodeToUtf8Encoding(source, destination));
        }

        /// <summary>
        /// Drain the stream.
        /// </summary>
        /// <param name="inStr">The stream to drain.</param>
        public static void Drain(System.IO.Stream inStr)
        {
            byte[] bs = new byte[BUFFER_SIZE];
            while (inStr.Read(bs, 0, bs.Length) > 0)
            {
            }
        }

        /// <summary>
        /// Read all data.
        /// </summary>
        /// <param name="inStr">The stream to read.</param>
        /// <returns>The bytes read.</returns>
        public static byte[] ReadAll(System.IO.Stream inStr)
        {
            MemoryStream buf = new MemoryStream();
            PipeAll(inStr, buf);
            return buf.ToArray();
        }

        /// <summary>
        /// Read the limited data to the input stream.
        /// </summary>
        /// <param name="inStr">The input stream.</param>
        /// <param name="limit">The number to read.</param>
        /// <returns>The bytes read.</returns>
        public static byte[] ReadAllLimited(System.IO.Stream inStr, int limit)
        {
            MemoryStream buf = new MemoryStream();
            PipeAllLimited(inStr, limit, buf);
            return buf.ToArray();
        }

        /// <summary>
        /// Read all data to the input stream.
        /// </summary>
        /// <param name="inStr">The input stream.</param>
        /// <param name="buf">The buffer data to add to the input stream.</param>
        /// <returns>The number of bytes read.</returns>
        public static int ReadFully(System.IO.Stream inStr, byte[] buf)
        {
            return ReadFully(inStr, buf, 0, buf.Length);
        }

        /// <summary>
        /// Read all data to the input stream.
        /// </summary>
        /// <param name="inStr">The input stream.</param>
        /// <param name="buf">The buffer data to add to the input stream.</param>
        /// <param name="off">The buffer offset.</param>
        /// <param name="len">The length of data to read.</param>
        /// <returns>The number of bytes read.</returns>
        public static int ReadFully(System.IO.Stream inStr, byte[] buf, int off, int len)
        {
            int totalRead = 0;
            while (totalRead < len)
            {
                int numRead = inStr.Read(buf, off + totalRead, len - totalRead);
                if (numRead < 1)
                    break;
                totalRead += numRead;
            }
            return totalRead;
        }

        /// <summary>
        /// Pipe all data from one stream to another.
        /// </summary>
        /// <param name="inStr">The input stream.</param>
        /// <param name="outStr">The output stream.</param>
        public static void PipeAll(System.IO.Stream inStr, System.IO.Stream outStr)
        {
            byte[] bs = new byte[BUFFER_SIZE];
            int numRead;
            while ((numRead = inStr.Read(bs, 0, bs.Length)) > 0)
            {
                outStr.Write(bs, 0, numRead);
            }
        }

        /// <summary>
        /// Pipe all bytes from <c>inStr</c> to <c>outStr</c>, throwing <c>StreamFlowException</c> if greater
        /// than <c>limit</c> bytes in <c>inStr</c>.
        /// </summary>
        /// <param name="inStr">
        /// A <see cref="Stream"/>
        /// </param>
        /// <param name="limit">
        /// A <see cref="System.Int64"/>
        /// </param>
        /// <param name="outStr">
        /// A <see cref="Stream"/>
        /// </param>
        /// <returns>The number of bytes actually transferred, if not greater than <c>limit</c></returns>
        /// <exception cref="IOException"></exception>
        public static long PipeAllLimited(System.IO.Stream inStr, long limit, System.IO.Stream outStr)
        {
            byte[] bs = new byte[BUFFER_SIZE];
            long total = 0;
            int numRead;
            while ((numRead = inStr.Read(bs, 0, bs.Length)) > 0)
            {
                total += numRead;
                if (total > limit)
                    throw new System.Exception("Data Overflow");
                outStr.Write(bs, 0, numRead);
            }
            return total;
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
    }
}
