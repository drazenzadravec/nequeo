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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nequeo.Custom;

namespace Nequeo.IO
{
    /// <summary>
    /// Inherites a System.IO.StreamReader that reads characters from a byte stream in a particular encoding.
    /// </summary>
    public class StreamReader : System.IO.StreamReader
    {
        /// <summary>
        /// Create a stream reader.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        public StreamReader(string path)
            : base(path)
        {
        }

        /// <summary>
        /// Create a stream reader.
        /// </summary>
        /// <param name="stream">The stream to be read.</param>
        public StreamReader(Stream stream)
            : base(stream)
        {
        }

        /// <summary>
        /// Read one line at time, only read a line if it ends with \r\n.
        /// Reads a line of characters from the current stream and returns the data as a string.
        /// </summary>
        /// <returns>The next line from the input stream, or null if the end of the input stream is reached.</returns>
        public override string ReadLine()
        {
            string line = "";
            int bytesRead = 0;
            bool foundEndOfData = false;
            char[] buffer = new char[1];
            List<char> store = new List<char>();
            int position = 0;

            // Start a new timeout clock.
            TimeoutClock timeoutClock = new TimeoutClock(5000);

            // While the end of the header data
            // has not been found.
            while (!foundEndOfData)
            {
                // Read a single byte from the stream
                // add the data to the store and re-assign.
                bytesRead = base.Read(buffer, 0, 1);

                // If data exists.
                if (bytesRead > 0)
                {
                    // Each time data is read reset the timeout.
                    timeoutClock.Reset();
                    store.Add(buffer[0]);

                    // If the store contains the right
                    // amount of data.
                    if (store.Count > 1)
                    {
                        // If the end of the header data has been found
                        // \r\n (13 10).
                        if (store[position] == 10 &&
                            store[position - 1] == 13)
                        {
                            // The end of the header data has been found.
                            foundEndOfData = true;
                            break;
                        }
                    }

                    // Increment the position.
                    position++;
                }

                // If the timeout has bee reached then
                // break from the loop.
                if (timeoutClock.IsComplete())
                    break;
            }

            line = new string(store.ToArray());
            return line;
        }

        /// <summary>
        /// Read one line at time, only read a line if it ends with \r\n.
        /// Reads a line of characters asynchronously from the current stream and returns the data as a string.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation. The value of the
        /// TResult parameter contains the next line from the stream, or is null if all
        /// the characters have been read.</returns>
        public override Task<string> ReadLineAsync()
        {
            return Nequeo.Threading.AsyncOperationResult<bool>.RunTask<string>(
                () =>
                {
                    // Read the new line.
                    return ReadLine();
                });
        }
    }
}
