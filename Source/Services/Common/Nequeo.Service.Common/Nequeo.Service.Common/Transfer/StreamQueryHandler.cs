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

using Nequeo.Net;
using Nequeo.Net.Http;
using Nequeo.Handler;

namespace Nequeo.Service.Transfer
{
    /// <summary>
    /// Stream HTTP context handler.
    /// </summary>
    public class StreamQueryHandler : IHttpHandler
    {
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
        public void ProcessRequest(System.Web.HttpContext context)
        {
            try
            {
                // Get the collection of querys.
                NameValueCollection query = context.Request.QueryString;

                // Get the base path directory
                string baseDirectoryPath = Nequeo.Service.Properties.Settings.Default.BaseDirectoryPath.TrimEnd('\\') + "\\";

                // If the directory does not exist then create it.
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(baseDirectoryPath)))
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(baseDirectoryPath));

                // Set the initial file data
                string operation = "ER";
                string fileName = string.Empty;
                long fileSize = (long)0;
                string resourceUri = string.Empty;
                string location = string.Empty;
                long position = (long)0;

                // Read the file data structure information
                // for the structure format.
                ReadStructure(query, out operation, out fileName, out fileSize, out resourceUri, out location, out position);

                // Get the current operation to perform.
                switch (operation.ToUpper())
                {
                    case "ER":
                        throw new Exception("Unable to process request, IP:" + context.Request.UserHostAddress);

                    case "UP":
                        // Attempt to upload the file from the client.
                        CreateDirectory(baseDirectoryPath + location);
                        bool uploadResult = UploadFile(context.Request, baseDirectoryPath + location + fileName, fileSize, position);

                        // If failed.
                        if (!uploadResult)
                            throw new Exception("Unable to upload file");
                        break;

                    case "DN":
                        // Attempt to download the file to the client.
                        bool downloadResult = DownloadFile(context.Response, baseDirectoryPath + location + fileName, position);

                        // If failed.
                        if (!downloadResult)
                            throw new Exception("Unable to download file");
                        break;

                    case "DL":
                        // Attempt to delete the file.
                        bool deleteResult = DeleteFile(context.Response, baseDirectoryPath + location + fileName);

                        // If failed.
                        if (deleteResult)
                            WriteDeleteFileResponse(context.Response);
                        else
                            throw new Exception("Unable to delete file");
                        break;

                    case "DR":
                        // Attempt to download the file from the URL to the server.
                        CreateDirectory(baseDirectoryPath + location);
                        bool downloadUrlResult = DownloadUrlFile(resourceUri, baseDirectoryPath + location + fileName);

                        // If failed.
                        if (!downloadUrlResult)
                            throw new Exception("Unable to download the URL file");
                        else
                            WriteDownloadUrlFileResponse(context.Response);
                        break;

                    case "UR":
                        // Attempt to upload the server file to the URL.
                        Byte[] result = null;
                        bool uploadUrlResult = UploadUrlFile(resourceUri, baseDirectoryPath + location + fileName, out result);

                        // If failed.
                        if (!uploadUrlResult)
                            throw new Exception("Unable to upload the URL file");
                        else
                            WriteUploadUrlFileResponse(context.Response, result);
                        break;

                    case "SZ":
                        // Get the file size for the request.
                        long requestFileSize = GetFileSize(baseDirectoryPath + location + fileName);

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
        private bool UploadFile(System.Web.HttpRequest request, string filename, long size, long position)
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
        private bool DownloadFile(System.Web.HttpResponse response, string filename, long position)
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
        /// Delete the file request to the client.
        /// </summary>
        /// <param name="response">The http response object</param>
        /// <param name="filename">The file name to open</param>
        /// <returns>True if delete was complete; else false</returns>
        private bool DeleteFile(System.Web.HttpResponse response, string filename)
        {
            try
            {
                // Does the file exist.
                if (System.IO.File.Exists(filename))
                    System.IO.File.Delete(filename);
                else
                    throw new Exception("File '" + filename + "' does not exist.");

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                // Clean-up
                if (response != null)
                    if (response.OutputStream != null)
                        response.OutputStream.Close();
            }
        }

        /// <summary>
        /// Download the URI file request to the server.
        /// </summary>
        /// <param name="url">The URI to download.</param>
        /// <param name="filename">The location to download the file.</param>
        /// <returns>True if download was complete; else false</returns>
        private bool DownloadUrlFile(string url, string filename)
        {
            try
            {
                // Make sure the file has been specifed.
                if (String.IsNullOrEmpty(filename))
                    throw new Exception("The file does not exist.");

                // Make sure the file has been specifed.
                if (String.IsNullOrEmpty(url))
                    throw new Exception("The remote URI has not been specified.");

                // Attempt to download the URL resource.
                // Create a new client connection.
                WebConnectionAdapter connection = new WebConnectionAdapter();
                TransferWebClient target = new TransferWebClient(connection);
                target.DownloadFile(url, filename);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
            }
        }

        /// <summary>
        /// Upload the server local file to the remote URI.
        /// </summary>
        /// <param name="url">The URI to upload.</param>
        /// <param name="filename">The location to upload the file.</param>
        /// <param name="result">The response from the server after upload.</param>
        /// <returns>True if upload was complete; else false</returns>
        private bool UploadUrlFile(string url, string filename, out Byte[] result)
        {
            try
            {
                // Make sure the file has been specifed.
                if (String.IsNullOrEmpty(filename))
                    throw new Exception("The file does not exist.");

                // Make sure the file has been specifed.
                if (String.IsNullOrEmpty(url))
                    throw new Exception("The remote URI has not been specified.");

                // Attempt to upload the URL resource.
                // Create a new client connection.
                WebConnectionAdapter connection = new WebConnectionAdapter();
                TransferWebClient target = new TransferWebClient(connection);
                result = target.UploadFile(url, filename);

                return true;
            }
            catch (Exception ex)
            {
                result = null;
                return false;
            }
            finally
            {
            }
        }

        /// <summary>
        /// Write the delete file
        /// </summary>
        /// <param name="response">The response object</param>
        private void WriteDeleteFileResponse(System.Web.HttpResponse response)
        {
            // Respond with the error.
            response.ContentType = "text/plain";
            response.AddHeader("Content-Length", "Deleted".Length.ToString());
            response.Write("Deleted");
        }

        /// <summary>
        /// Write the file size
        /// </summary>
        /// <param name="response">The response object</param>
        /// <param name="size">The size of file to write</param>
        private void WriteFileSizeResponse(System.Web.HttpResponse response, long size)
        {
            // Respond with the error.
            response.ContentType = "text/plain";
            response.AddHeader("Content-Length", size.ToString().Length.ToString());
            response.Write(size.ToString());
        }

        /// <summary>
        /// Write the download URI file
        /// </summary>
        /// <param name="response">The response object</param>
        private void WriteDownloadUrlFileResponse(System.Web.HttpResponse response)
        {
            // Respond with the error.
            response.ContentType = "text/plain";
            response.AddHeader("Content-Length", "Completed".Length.ToString());
            response.Write("Completed");
        }

        /// <summary>
        /// Write the download URI file
        /// </summary>
        /// <param name="response">The response object</param>
        /// <param name="result">The response from the server</param>
        private void WriteUploadUrlFileResponse(System.Web.HttpResponse response, byte[] result)
        {
            // Respond with the error.
            response.ContentType = "text/plain";

            if (result != null)
            {
                string data = "Completed. " + Encoding.Default.GetString(result);
                response.AddHeader("Content-Length", data.Length.ToString());
                response.Write(data);
            }
            else
            {
                response.AddHeader("Content-Length", "Completed".Length.ToString());
                response.Write("Completed");
            }
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
                // Get the file info from the specified file
                FileInfo fileInfo = new FileInfo(fileName);

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
        /// <param name="query">The query collection</param>
        /// <param name="operation">The operation to perform</param>
        /// <param name="fileName">The name of the file in the data</param>
        /// <param name="fileSize">The size of the file</param>
        /// <param name="uri">The resourse to search for</param>
        /// <param name="location">The file location to upload/download from/to.</param>
        /// <param name="position">The position to start reading/writing from.</param>
        private void ReadStructure(NameValueCollection query, out string operation, out string fileName, out long fileSize, out string uri, out string location, out long position)
        {
            try
            {
                // If no query data has been supplied.
                if (query.Count < 1)
                    throw new Exception("Invalid query structured data.");

                // Extract the file name and the file size.
                operation = query["O"].ToString();
                fileName = query["F"].ToString();

                if (query["S"] != null)
                    fileSize = Int64.Parse(query["S"].ToString());
                else
                    fileSize = (long)0;

                if (query["R"] != null)
                    uri = query["R"].ToString();
                else
                    uri = string.Empty;

                if (query["L"] != null)
                    location = query["L"].ToString().Replace('|', '\\').TrimStart('\\').TrimEnd('\\') + "\\";
                else
                    location = string.Empty;

                if (query["P"] != null)
                    position = Int64.Parse(query["P"].ToString());
                else
                    position = (long)0;
            }
            catch (Exception ex)
            {
                // Assign the default out values.
                operation = "ER";
                fileName = string.Empty;
                fileSize = (long)0;
                uri = string.Empty;
                location = string.Empty;
                position = (long)0;
            }
        }

        /// <summary>
        /// Create the directory on the server.
        /// </summary>
        /// <param name="directory">The directory to create.</param>
        private void CreateDirectory(string directory)
        {
            // If the directory does not exist then create it.
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(directory)))
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(directory));
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
