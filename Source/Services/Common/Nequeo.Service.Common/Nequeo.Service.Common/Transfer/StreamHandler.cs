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
    /// Stream HTTP context handler.
    /// </summary>
    public class StreamHandler : IHttpHandler
    {
        private const int OPERATION_STRUCTURE_BUFFER_SIZE = 2;
        private const int FILENAME_STRUCTURE_BUFFER_SIZE = 98;
        private const int FILE_SIZE_STRUCTURE_BUFFER_SIZE = 12;
        private const int FILE_POSITION_STRUCTURE_BUFFER_SIZE = 12;
        private const int STRUCTURE_BUFFER_SIZE =
                            OPERATION_STRUCTURE_BUFFER_SIZE +
                            FILENAME_STRUCTURE_BUFFER_SIZE +
                            FILE_SIZE_STRUCTURE_BUFFER_SIZE +
                            FILE_POSITION_STRUCTURE_BUFFER_SIZE;

        private int _timeout = 30000;

        /// <summary>
        /// Gets or sets the transfer time.
        /// </summary>
        public int Timeout { get { return _timeout; } set { _timeout = value; } }

        /// <summary>
        /// Return resuse state option
        /// </summary>
        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        /// <summary>
        /// Process request method.
        /// </summary>
        /// <param name="context">The current http context.</param>
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                // Get the base path directory
                string baseDirectoryPath = Nequeo.Service.Properties.Settings.Default.BaseDirectoryPath.TrimEnd('\\') + "\\";

                // If the directory does not exist then create it.
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(baseDirectoryPath)))
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(baseDirectoryPath));

                // Set the initial file data
                string operation = "ER";
                string fileName = string.Empty;
                long fileSize = (long)0;
                long position = (long)0;

                // Read the file data structure information
                // for the structure format.
                ReadStructure(context.Request.InputStream, out operation, out fileName, out fileSize, out position);

                // Get the current operation to perform.
                switch (operation.ToUpper())
                {
                    case "ER":
                        throw new Exception("Unable to process request, IP:" + context.Request.UserHostAddress);

                    case "UP":
                        // Attempt to upload the file.
                        bool uploadResult = UploadFile(context.Request, baseDirectoryPath + fileName, fileSize, position);

                        // If failed.
                        if (!uploadResult)
                            throw new Exception("Unable to upload file");
                        break;

                    case "DN":
                        // Attempt to download the file.
                        bool downloadResult = DownloadFile(context.Response, baseDirectoryPath + fileName, position);

                        // If failed.
                        if (!downloadResult)
                            throw new Exception("Unable to download file");
                        break;

                    case "SZ":
                        // Get the file size for the request.
                        long requestFileSize = GetFileSize(fileName);

                        // If success the write the data.
                        if (requestFileSize > -1)
                            WriteFileSizeResponse(context.Response, requestFileSize);
                        else
                            throw new Exception("Unable to return file size");
                        break;

                    default:
                        throw new Exception("The operation '" + operation + "' is not supported");
                }
            }
            catch (Exception ex)
            {
                try
                {
                    // Respond with the error.
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("ERROR: " + ex.Message);
                }
                catch { }
            }
        }

        /// <summary>
        /// Upload the file from the client.
        /// </summary>
        /// <param name="request">The http request object</param>
        /// <param name="filename">The file name to create</param>
        /// <param name="size">The size of the file.</param>
        /// <param name="position">The position in the file to start writing from.</param>
        /// <returns>>True if download was complete; else false</returns>
        private bool UploadFile(HttpRequest request, string filename, long size, long position)
        {
            System.IO.Stream localDestination = null;

            try
            {
                // Create the new file and start the transfer process.
                localDestination = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

                // If an upload position exists.
                if (position > 0)
                    localDestination.Position = position;

                // Transfer the data from the client.
                TransferData(request.InputStream, localDestination, (long)request.ContentLength, _timeout);

                // Close the local file.
                localDestination.Flush();
                localDestination.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                // Clean-up
                if (localDestination != null)
                    localDestination.Close();

                // Clean-up
                if (request != null)
                    if (request.InputStream != null)
                        request.InputStream.Close();
            }
        }

        /// <summary>
        /// Download the file request to the client.
        /// </summary>
        /// <param name="response">The http response object</param>
        /// <param name="filename">The file name to open</param>
        /// <param name="position">The position in the file to start reading from.</param>
        /// <returns>True if download was complete; else false</returns>
        private bool DownloadFile(HttpResponse response, string filename, long position)
        {
            System.IO.Stream localSource = null;

            try
            {
                // Open the file and create a new file stream.
                localSource = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // If a download position exists.
                if (position > 0)
                    localSource.Position = position;

                // Transfer the data to the client.
                response.AddHeader("Content-Length", localSource.Length.ToString());
                TransferData(localSource, response.OutputStream, localSource.Length, _timeout);

                // Close the local file.
                localSource.Flush();
                localSource.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                // Clean-up
                if (localSource != null)
                    localSource.Close();

                // Clean-up
                if (response != null)
                    if (response.OutputStream != null)
                        response.OutputStream.Close();
            }
        }

        /// <summary>
        /// Write the file size
        /// </summary>
        /// <param name="response">The response object</param>
        /// <param name="size">The size of file to write</param>
        private void WriteFileSizeResponse(HttpResponse response, long size)
        {
            // Respond with the error.
            response.ContentType = "text/plain";
            response.AddHeader("Content-Length", size.ToString().Length.ToString());
            response.Write(size.ToString());
        }

        /// <summary>
        /// Gets the file size.
        /// </summary>
        /// <param name="fileName">The name if the file</param>
        /// <returns>The size in bytes of the file; else -1 (operation failed).</returns>
        private long GetFileSize(string fileName)
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
                return -1;
            }
        }

        /// <summary>
        /// Read the data request structured data.
        /// </summary>
        /// <param name="source">The source request stream</param>
        /// <param name="operation">The operation to perform</param>
        /// <param name="fileName">The name of the file in the data</param>
        /// <param name="fileSize">The size of the file</param>
        /// <param name="position">The position to start reading/writing from.</param>
        private void ReadStructure(System.IO.Stream source, out string operation, out string fileName, out long fileSize, out long position)
        {
            int readBytes = 0;
            const int bufferLength = 1;
            byte[] buffer = new byte[bufferLength];

            // Create a memmory stream which will store
            // the structured file data.
            int totalBytesRead = 0;
            MemoryStream structureData = new MemoryStream();

            try
            {
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
                operation = fileData.Substring(0, OPERATION_STRUCTURE_BUFFER_SIZE).TrimEnd();
                fileName = fileData.Substring(OPERATION_STRUCTURE_BUFFER_SIZE, FILENAME_STRUCTURE_BUFFER_SIZE).TrimEnd();
                fileSize = Int64.Parse(fileData.Substring(
                    OPERATION_STRUCTURE_BUFFER_SIZE + FILENAME_STRUCTURE_BUFFER_SIZE, FILE_SIZE_STRUCTURE_BUFFER_SIZE).TrimEnd());
                position = Int64.Parse(fileData.Substring(
                    OPERATION_STRUCTURE_BUFFER_SIZE + FILENAME_STRUCTURE_BUFFER_SIZE + FILE_SIZE_STRUCTURE_BUFFER_SIZE, FILE_POSITION_STRUCTURE_BUFFER_SIZE).TrimEnd());

                // Cloase the memory stream that contains
                // the structured data.
                structureData.Close();
            }
            catch (Exception ex)
            {
                // Assign the default out values.
                operation = "ER";
                fileName = string.Empty;
                fileSize = (long)0;
                position = (long)0;
            }
            finally
            {
                // Clean-up
                if (structureData != null)
                    structureData.Close();
            }
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
}
