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
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Web.Hosting;
using System.Web;

using Nequeo.Handler;
using Nequeo.Composite.Configuration;

namespace Nequeo.Service.Transfer
{
    /// <summary>
    /// Simple file transfer streaming.
    /// </summary>
    public class Stream : IStream
    {
        private const int FILENAME_STRUCTURE_BUFFER_SIZE = 100;
        private const int FILE_SIZE_STRUCTURE_BUFFER_SIZE = 12;
        private const int
            STRUCTURE_BUFFER_SIZE = FILENAME_STRUCTURE_BUFFER_SIZE +
            FILE_SIZE_STRUCTURE_BUFFER_SIZE;

        private int _timeout = 30000;

        /// <summary>
        /// Gets or sets the transfer time.
        /// </summary>
        public int Timeout { get { return _timeout; } set { _timeout = value; } }

        /// <summary>
        /// Upload a structured format file.
        /// </summary>
        /// <param name="stream">The uploaded file stream.</param>
        /// <returns>True if uploaded; else false.</returns>
        public bool UploadStructuredFile(System.IO.Stream stream)
        {
            System.IO.Stream localDestination = null;

            try
            {
                // Get the base path directory
                string baseDirectoryPath = Nequeo.Service.Properties.Settings.Default.BaseDirectoryPath.TrimEnd('\\') + "\\";

                // If the directory does not exist then create it.
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(baseDirectoryPath)))
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(baseDirectoryPath));

                // Set the initial file data
                string fileName = "temp.data";
                long fileSize = 0;

                // Read the file data structure information
                // for the structure format.
                ReadUploadStructure(stream, out fileName, out fileSize);

                // Create the new file and start the transfer process.
                localDestination = new FileStream(baseDirectoryPath + fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                TransferData(stream, localDestination, stream.Length, _timeout);

                // Close the local file.
                localDestination.Flush();
                localDestination.Close();
                return true;
            }
            catch (Exception ex)
            {
                // Get the current exception.
                return false;
            }
            finally
            {
                // Clean-up
                if (localDestination != null)
                    localDestination.Close();
            }
        }

        /// <summary>
        /// Upload a structured format file.
        /// </summary>
        /// <param name="stream">The uploaded file stream.</param>
        /// <returns>True if uploaded; else false.</returns>
        public bool UploadFile(System.IO.Stream stream)
        {
            System.IO.Stream localDestination = null;

            try
            {
                // Get the base path directory
                string baseDirectoryPath = Nequeo.Service.Properties.Settings.Default.BaseDirectoryPath.TrimEnd('\\') + "\\";

                // If the directory does not exist then create it.
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(baseDirectoryPath)))
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(baseDirectoryPath));

                // Create a new unique file name.
                string fileName = Guid.NewGuid().ToString() + ".upload";

                // Create the new file and start the transfer process.
                localDestination = new FileStream(baseDirectoryPath + fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                TransferData(stream, localDestination, stream.Length, _timeout);

                // Close the local file.
                localDestination.Flush();
                localDestination.Close();
                return true;
            }
            catch (Exception ex)
            {
                // Get the current exception.
                return false;
            }
            finally
            {
                // Clean-up
                if (localDestination != null)
                    localDestination.Close();
            }
        }

        /// <summary>
        /// Download file.
        /// </summary>
        /// <param name="fileName">The file name to download.</param>
        /// <returns>The download file stream.</returns>
        public System.IO.Stream DownloadFile(string fileName)
        {
            System.IO.Stream localSource = null;

            try
            {
                // Get the base path directory
                string baseDirectoryPath = Nequeo.Service.Properties.Settings.Default.BaseDirectoryPath.TrimEnd('\\') + "\\";

                // Open the file and create a new file stream.
                localSource = new FileStream(baseDirectoryPath + fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // Return the stream instance.
                return localSource;
            }
            catch (Exception ex)
            {
                // Clean-up
                if (localSource != null)
                    localSource.Close();

                // Get the current exception.
                return null;
            }
        }

        /// <summary>
        /// Gets the file size.
        /// </summary>
        /// <param name="fileName">The name if the file</param>
        /// <returns>The size in bytes of the file; else -1 (operation failed).</returns>
        public long GetFileSize(string fileName)
        {
            try
            {
                // Get the base path directory
                string baseDirectoryPath = Nequeo.Service.Properties.Settings.Default.BaseDirectoryPath.TrimEnd('\\') + "\\";

                // Get the file info from the specified file
                FileInfo fileInfo = new FileInfo(baseDirectoryPath + fileName);

                // Return the length of the file.
                return fileInfo.Length;
            }
            catch (Exception ex)
            {
                // Get the current exception.
                return -1;
            }
        }

        /// <summary>
        /// Read the structured data from the source stream, the structured data at the top of the
        /// file contains the file name to use when uploading the file and the file size.
        /// </summary>
        /// <param name="source">The source stream that contains the structure data.</param>
        /// <param name="fileName">Sets the filename from the structured data.</param>
        /// <param name="fileSize">Sets the file size from the struactured data.</param>
        private void ReadUploadStructure(System.IO.Stream source, out string fileName, out long fileSize)
        {
            int readBytes = 0;
            const int bufferLength = 1;
            byte[] buffer = new byte[bufferLength];

            // Create a memmory stream which will store
            // the structured file data.
            int totalBytesRead = 0;
            MemoryStream structureData = new MemoryStream();

            // Read all the data in the source stream and
            // write the data to the destination stream.
            // Only read the number of bytes that are structured
            // data and no more (STRUCTURE_BUFFER_SIZE).
            do
            {
                // Read the data and then write the data.
                readBytes = source.Read(buffer, 0, bufferLength);
                structureData.Write(buffer, 0, readBytes);

                // Only read the structured data within the file
                // this is located at the top of the file.
                totalBytesRead += readBytes;
            }
            while ((totalBytesRead < STRUCTURE_BUFFER_SIZE) && readBytes != 0);

            // Convert the array of bytes in the structured data
            // to a string format
            structureData.Flush();
            string fileData = ASCIIEncoding.UTF8.GetString(structureData.ToArray());

            // Extract the file name and the file size.
            fileName = fileData.Substring(0, FILENAME_STRUCTURE_BUFFER_SIZE).TrimEnd();
            fileSize = Int64.Parse(fileData.Substring(FILENAME_STRUCTURE_BUFFER_SIZE, FILE_SIZE_STRUCTURE_BUFFER_SIZE).TrimEnd());

            // Cloase the memory stream that contains
            // the structured data.
            structureData.Close();
        }

        /// <summary>
        /// Transfer the stream data from the source to the destination.
        /// </summary>
        /// <param name="source">The source stream to read from.</param>
        /// <param name="destination">The destination stream to write to.</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of source bytes). Waits until all bytes are read.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <returns>True if all data has been copied; else false.</returns>
        public bool TransferData(System.IO.Stream source, System.IO.Stream destination, long byteLength, long timeout = -1)
        {
            return Nequeo.IO.Stream.Operation.CopyStream(source, destination, byteLength, timeout);
        }
    }

    /// <summary>
    /// Simple file transfer streaming.
    /// </summary>
    [Logging(Handler.WriteTo.EventLog, Handler.LogType.Error)]
    public class ByteStream : IByteStream
    {
        private const int FILENAME_STRUCTURE_BUFFER_SIZE = 100;
        private const int FILE_SIZE_STRUCTURE_BUFFER_SIZE = 12;
        private const int
            STRUCTURE_BUFFER_SIZE = FILENAME_STRUCTURE_BUFFER_SIZE +
            FILE_SIZE_STRUCTURE_BUFFER_SIZE;

        /// <summary>
        /// Upload file.
        /// </summary>
        /// <param name="stream">The uploaded file stream.</param>
        /// <param name="fileName">The file name to upload.</param>
        /// <param name="fileSize">The file to upload size.</param>
        /// <returns>True if uploaded; else false.</returns>
        public bool UploadFile(byte[] stream, string fileName, long fileSize)
        {
            System.IO.Stream localDestination = null;

            try
            {
                // Get the base path directory
                string baseDirectoryPath = Nequeo.Service.Properties.Settings.Default.BaseDirectoryPath.TrimEnd('\\') + "\\";

                // If the directory does not exist then create it.
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(baseDirectoryPath)))
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(baseDirectoryPath));

                // Create the new file and start the transfer process.
                localDestination = new FileStream(baseDirectoryPath + fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                localDestination.Write(stream, 0, Convert.ToInt32(fileSize));

                // Close the local file.
                localDestination.Flush();
                localDestination.Close();
                return true;
            }
            catch (Exception ex)
            {
                // Get the current exception.
                return false;
            }
            finally
            {
                // Clean-up
                if (localDestination != null)
                    localDestination.Close();
            }
        }

        /// <summary>
        /// Download file.
        /// </summary>
        /// <param name="fileName">The file name to download.</param>
        /// <returns>The download file stream.</returns>
        public byte[] DownloadFile(string fileName)
        {
            System.IO.Stream localSource = null;
            System.IO.StreamReader localReader = null;

            try
            {
                // Get the base path directory
                string baseDirectoryPath = Nequeo.Service.Properties.Settings.Default.BaseDirectoryPath.TrimEnd('\\') + "\\";

                // Open the file and create a new file stream.
                localSource = new FileStream(baseDirectoryPath + fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                localReader = new StreamReader(localSource);

                // Return the stream instance.
                byte[] data = ASCIIEncoding.Default.GetBytes(localReader.ReadToEnd());

                localReader.Close();
                localSource.Close();

                // Return the data.
                return data;
            }
            catch (Exception ex)
            {
                // Get the current exception.
                return null;
            }
            finally
            {
                // Clean-up
                if (localReader != null)
                    localReader.Close();

                // Clean-up
                if (localSource != null)
                    localSource.Close();
            }
        }

        /// <summary>
        /// Gets the file size.
        /// </summary>
        /// <param name="fileName">The name if the file</param>
        /// <returns>The size in bytes of the file; else -1 (operation failed).</returns>
        public long GetFileSize(string fileName)
        {
            try
            {
                // Get the base path directory
                string baseDirectoryPath = Nequeo.Service.Properties.Settings.Default.BaseDirectoryPath.TrimEnd('\\') + "\\";

                // Get the file info from the specified file
                FileInfo fileInfo = new FileInfo(baseDirectoryPath + fileName);

                // Return the length of the file.
                return fileInfo.Length;
            }
            catch (Exception ex)
            {
                // Get the current exception.
                return -1;
            }
        }
    }
}
