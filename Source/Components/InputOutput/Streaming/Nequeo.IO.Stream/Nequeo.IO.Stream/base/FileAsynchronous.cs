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
using System.Text;
using System.Threading.Tasks;

using Nequeo.Threading.Extension;
using Nequeo.IO.Stream.Extension;

namespace Nequeo.IO.Stream
{
    /// <summary>
    /// Provides asynchronous counterparts to members of the File class.
    /// </summary>
    public static class FileAsynchronous
    {
        private const int BUFFER_SIZE = 0x2000;

        /// <summary>
        /// Copies a unicode encoded file to an ansi encoded file.
        /// </summary>
        /// <param name="unicodeSourcePath">The unicode encoded source file</param>
        /// <param name="ansiDestinationPath">The ansi encoded source file</param>
        /// <returns>The task object reference.</returns>
        public static Task CopyUnicodeToUtf8(string unicodeSourcePath, string ansiDestinationPath)
        {
            // Open the output file for writing
            var destinationStream = FileAsynchronous.OpenWrite(ansiDestinationPath);
            var sourceStream = FileAsynchronous.OpenRead(unicodeSourcePath);

            // Copy the unicode data to ansi.
            return sourceStream.CopyStreamToStreamAsyncUnicodeToUtf8(destinationStream);
        }

        /// <summary>Opens an existing file for asynchronous reading.</summary>
        /// <param name="path">The path to the file to be opened for reading.</param>
        /// <returns>A read-only FileStream on the specified path.</returns>
        public static FileStream OpenRead(string path)
        {
            // Open a file stream for reading and that supports asynchronous I/O
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE, true);
        }

        /// <summary>Opens an existing file for asynchronous writing.</summary>
        /// <param name="path">The path to the file to be opened for writing.</param>
        /// <returns>An unshared FileStream on the specified path with access for writing.</returns>
        public static FileStream OpenWrite(string path)
        {
            // Open a file stream for writing and that supports asynchronous I/O
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, BUFFER_SIZE, true);
        }

        /// <summary>
        /// Opens a binary file for asynchronosu operation, reads the contents of the file into a byte array, and then closes the file.
        /// </summary>
        /// <param name="path">The path to the file to be read.</param>
        /// <returns>A task that will contain the contents of the file.</returns>
        public static Task<byte[]> ReadAllBytes(string path)
        {
            // Open the file for reading
            var fs = OpenRead(path);

            // Read all of its contents
            var asyncRead = fs.ReadAllBytesAsync();

            // When we're done reading its contents, close the file and propagate the file's contents
            var closedFile = asyncRead.ContinueWith(t =>
            {
                fs.Close();
                return t.Result;
            }, TaskContinuationOptions.ExecuteSynchronously);

            // Return the task that represents the entire operation being complete and that returns the
            // file's contents
            return closedFile;
        }

        /// <summary>
        /// Opens a binary file for asynchronous operation, writes the contents of the byte array into the file, and then closes the file.
        /// </summary>
        /// <param name="path">The path to the file to be written.</param>
        /// <param name="bytes">The data that is to be written.</param>
        /// <returns>A task that will signal the completion of the operation.</returns>
        public static Task WriteAllBytes(string path, byte[] bytes)
        {
            // Open the file for writing
            var fs = OpenWrite(path);

            // Write the contents to the file
            var asyncWrite = fs.WriteAsync(bytes, 0, bytes.Length);

            // When complete, close the file and propagate any exceptions
            var closedFile = asyncWrite.ContinueWith(t =>
            {
                var e = t.Exception;
                fs.Close();
                if (e != null) throw e;
            }, TaskContinuationOptions.ExecuteSynchronously);

            // Return a task that represents the operation having completed
            return closedFile;
        }

        /// <summary>
        /// Opens a text file for asynchronosu operation, reads the contents of the file into a string, and then closes the file.
        /// </summary>
        /// <param name="path">The path to the file to be read.</param>
        /// <returns>A task that will contain the contents of the file.</returns>
        public static Task<string> ReadAllText(string path)
        {
            // Create a StringBuilder to store the text from the file and an encoding object to decode the
            // contents of the file
            var text = new StringBuilder();
            var encoding = new UTF8Encoding();

            // Open the file for reading
            var fs = OpenRead(path);

            // Continually read buffers from the file, decoding them and storing the results into the StringBuilder
            var asyncRead = fs.ReadBuffersAsync(BUFFER_SIZE, (buffer, count) => text.Append(encoding.GetString(buffer, 0, count)));

            // When done, close the file, propagate any exceptions, and return the decoded text
            return asyncRead.ContinueWith(t =>
            {
                var e = t.Exception;
                fs.Close();
                if (e != null) throw e;
                return text.ToString();
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// Opens a text file for asynchronosu operation, writes a string into the file, and then closes the file.
        /// </summary>
        /// <param name="path">The path to the file to be written.</param>
        /// <param name="contents">The contents that are to be written.</param>
        /// <returns>A task that will signal the completion of the operation.</returns>
        public static Task WriteAllText(string path, string contents)
        {
            // First encode the string contents into a byte array
            var encoded = Task.Factory.StartNew(
                state => Encoding.UTF8.GetBytes((string)state),
                contents);

            // When encoding is done, write all of the contents to the file.  Return
            // a task that represents the completion of that write.
            return encoded.ContinueWith(t => WriteAllBytes(path, t.Result)).Unwrap();
        }
    }
}
