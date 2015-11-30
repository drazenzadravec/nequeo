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
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Configuration;
using System.Net;
using System.IO;

namespace Nequeo.Net.Http.Common
{
    /// <summary>
    /// Http response content loaded.
    /// </summary>
    public partial class HttpResponseContent
    {
        /// <summary>
        /// Create a new memory stream from the resposne data.
        /// </summary>
        /// <param name="responseData">The resposne data.</param>
        /// <returns>The new resposne memeory stream.</returns>
        public static MemoryStream CreateResponseStream(byte[] responseData)
        {
            return new MemoryStream(responseData);
        }

        /// <summary>
        /// Transfer the response data.
        /// </summary>
        /// <param name="input">The input memory stream.</param>
        /// <param name="output">The output stream.</param>
        public static void TransferResponse(System.IO.Stream input, System.IO.Stream output)
        {
            Exception exc = null;
            try
            {
                // Read the request stream data and write
                // to the memory stream.
                TransferData(input, output);
            }
            catch (Exception ex) { exc = ex; }

            try
            {
                // Properly flush and close the output stream
                input.Flush();
                input.Close();
            }
            catch (Exception ex) { exc = ex; }

            try
            {
                // Properly flush and close the output stream
                output.Flush();
                output.Close();
            }
            catch (Exception ex) { exc = ex; }

            if (exc != null)
                throw new Exception(exc.Message);
        }

        /// <summary>
        /// Transfer the stream data from the source to the destination.
        /// </summary>
        /// <param name="source">The source stream to read from.</param>
        /// <param name="destination">The destination stream to write to.</param>
        public static void TransferData(System.IO.Stream source, System.IO.Stream destination)
        {
            Nequeo.IO.Stream.Operation.CopyStream(source, destination);
        }

        /// <summary>
        /// Maximum content length.
        /// </summary>
        /// <param name="httpResponse">The current http response.</param>
        /// <param name="maxFileSize">The maximum upload file content.</param>
        /// <returns>The byte array of data to send.</returns>
        public static byte[] MaxContentLength(HttpListenerResponse httpResponse, int maxFileSize)
        {
            // Construct a minimal response string.
            string responseString = Nequeo.Net.Http.Common.HttpPageContent.Html003(maxFileSize);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            // Get the response OutputStream and write the response to it.
            httpResponse.ContentLength64 = buffer.Length;
            httpResponse.StatusCode = (int)System.Net.HttpStatusCode.RequestEntityTooLarge;
            httpResponse.ContentType = "text/html; charset=utf-8";
            httpResponse.AddHeader("Content-Language", "en-au");
            httpResponse.AddHeader("Server", "Nequeo/2011.26 (Windows)");

            // Get the response data.
            return buffer;
        }

        /// <summary>
        /// File not found response.
        /// </summary>
        /// <param name="httpResponse">The current http response.</param>
        /// <returns>The byte array of data to send.</returns>
        public static byte[] FileNotFound(HttpListenerResponse httpResponse)
        {
            // Construct a minimal response string.
            string responseString = Nequeo.Net.Http.Common.HttpPageContent.Html404();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            // Get the response OutputStream and write the response to it.
            httpResponse.ContentLength64 = buffer.Length;
            httpResponse.ContentType = "text/html; charset=utf-8";
            httpResponse.StatusCode = (int)System.Net.HttpStatusCode.NotFound;
            httpResponse.AddHeader("Allow", "POST, PUT, GET, HEAD");
            httpResponse.AddHeader("Content-Language", "en-au");
            httpResponse.AddHeader("Server", "Nequeo/2011.26 (Windows)");

            // Get the response data.
            return buffer;
        }

        /// <summary>
        /// Get the resource data.
        /// </summary>
        /// <param name="httpResponse">The current http response.</param>
        /// <param name="process">The processing instnace.</param>
        /// <param name="urlFilePath">The current resource.</param>
        /// <param name="extension">The extension of the resource.</param>
        /// <returns>The byte array of data to send.</returns>
        public static byte[] ResourceFound(HttpListenerResponse httpResponse, Nequeo.Net.Http.IActiveProcessing process, string urlFilePath, string extension)
        {
            // Construct a response string.
            byte[] buffer = System.IO.File.ReadAllBytes(urlFilePath);
            string extensionBase = process.GetMimeContentType(extension);

            // Get the response OutputStream and write the response to it.
            httpResponse.ContentLength64 = buffer.Length;
            httpResponse.ContentType = extensionBase;
            httpResponse.AddHeader("Allow", "POST, PUT, GET, HEAD");
            httpResponse.AddHeader("Content-Language", "en-au");
            httpResponse.AddHeader("Server", "Nequeo/2011.26 (Windows)");

            // Get the response data.
            return buffer;
        }

        /// <summary>
        /// Get the resource data.
        /// </summary>
        /// <param name="httpResponse">The current http response.</param>
        /// <param name="content">The content length.</param>
        /// <param name="process">The processing instnace.</param>
        /// <param name="urlFilePath">The current resource.</param>
        /// <param name="extension">The extension of the resource.</param>
        public static void ResourceFound(HttpListenerResponse httpResponse, long content, Nequeo.Net.Http.IActiveProcessing process, string urlFilePath, string extension)
        {
            // Construct a response string.
            string extensionBase = process.GetMimeContentType(extension);

            // Get the response OutputStream and write the response to it.
            httpResponse.ContentLength64 = content;
            httpResponse.ContentType = extensionBase;
            httpResponse.AddHeader("Allow", "POST, PUT, GET, HEAD");
            httpResponse.AddHeader("Content-Language", "en-au");
            httpResponse.AddHeader("Server", "Nequeo/2011.26 (Windows)");
        }

        /// <summary>
        /// Get the resource attachment data.
        /// </summary>
        /// <param name="httpResponse">The current http response.</param>
        /// <param name="urlFilePath">The current resource location.</param>
        /// <param name="extension">The extension of the resource.</param>
        /// <param name="fileName">The file name of the resource.</param>
        /// <returns>The byte array of data to send.</returns>
        public static byte[] ResourceAttachment(HttpListenerResponse httpResponse, string urlFilePath, string extension, string fileName)
        {
            // Construct a response string.
            byte[] buffer = System.IO.File.ReadAllBytes(urlFilePath);

            // Get the response OutputStream and write the response to it.
            httpResponse.ContentLength64 = buffer.Length;
            httpResponse.ContentType = "application/" + extension;
            httpResponse.AddHeader("content-disposition", "attachment; filename=\"" + fileName + "\"");
            httpResponse.AddHeader("Content-Language", "en-au");
            httpResponse.AddHeader("Server", "Nequeo/2011.26 (Windows)");

            // Get the response data.
            return buffer;
        }

        /// <summary>
        /// Gets the error response.
        /// </summary>
        /// <param name="httpResponse">The current http response.</param>
        /// <param name="exception">The current exception.</param>
        /// <returns>The byte array of data to send.</returns>
        public static byte[] OnError(HttpListenerResponse httpResponse, Exception exception)
        {
            // Construct a minimal response string.
            string responseString = Nequeo.Net.Http.Common.HttpPageContent.Html500();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            // Get the response OutputStream and write the response to it.
            httpResponse.ContentLength64 = buffer.Length;
            httpResponse.ContentType = "text/html; charset=utf-8";
            httpResponse.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            httpResponse.AddHeader("Content-Language", "en-au");
            httpResponse.AddHeader("Server", "Nequeo/2011.26 (Windows)");

            // Get the response data.
            return buffer;
        }

        /// <summary>
        /// Get the uploaded file list response.
        /// </summary>
        /// <param name="httpResponse">The current http response.</param>
        /// <param name="baseDirectoryPath">The base directory.</param>
        /// <param name="uploadFileListName">The html page upload file name list handler.</param>
        /// <param name="directoryQuery">The directory query value.</param>
        /// <param name="deleteFilesEnable">Enable deletion of files.</param>
        /// <param name="deleteDirectoryEnable">Enable deletion of directories.</param>
        /// <returns>The byte array of data to send.</returns>
        public static byte[] UploadFileList(HttpListenerResponse httpResponse, string baseDirectoryPath,
            string uploadFileListName, string directoryQuery, Boolean deleteFilesEnable = false, Boolean deleteDirectoryEnable = false)
        {
            // Construct a minimal response string.
            byte[] buffer = GetUploadedFileListHtml(baseDirectoryPath, uploadFileListName, directoryQuery, deleteFilesEnable, deleteDirectoryEnable);

            // Get the response OutputStream and write the response to it.
            httpResponse.ContentLength64 = buffer.Length;
            httpResponse.ContentType = "text/html; charset=utf-8";
            httpResponse.AddHeader("Content-Language", "en-au");
            httpResponse.AddHeader("Server", "Nequeo/2011.26 (Windows)");

            // Get the response data.
            return buffer;
        }

        /// <summary>
        /// Set the json invalid http response data.
        /// </summary>
        /// <param name="httpResponse">The current http response.</param>
        public static byte[] SetJsonInvalidHttpResponse(HttpListenerResponse httpResponse)
        {
            // Construct a minimal response string.
            string responseString = Nequeo.Net.Http.Common.HttpPageContent.Html702();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            // Get the response OutputStream and write the response to it.
            httpResponse.ContentLength64 = buffer.Length;
            httpResponse.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            httpResponse.ContentType = "text/html; charset=utf-8";
            httpResponse.AddHeader("Content-Language", "en-au");
            httpResponse.AddHeader("Server", "Nequeo/2011.26 (Windows)");

            // Get the response data.
            return buffer;
        }

        /// <summary>
        /// Set the xml invalid http response data.
        /// </summary>
        /// <param name="httpResponse">The current http response.</param>
        public static byte[] SetXmlInvalidHttpResponse(HttpListenerResponse httpResponse)
        {
            // Construct a minimal response string.
            string responseString = Nequeo.Net.Http.Common.HttpPageContent.Html701();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            // Get the response OutputStream and write the response to it.
            httpResponse.ContentLength64 = buffer.Length;
            httpResponse.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            httpResponse.ContentType = "text/html; charset=utf-8";
            httpResponse.AddHeader("Content-Language", "en-au");
            httpResponse.AddHeader("Server", "Nequeo/2011.26 (Windows)");

            // Get the response data.
            return buffer;
        }

        /// <summary>
        /// Set the http response data.
        /// </summary>
        /// <param name="httpResponse">The current http response.</param>
        /// <param name="responseData">The current resposne data.</param>
        public static void SetXmlContentTypeResponse(HttpListenerResponse httpResponse, byte[] responseData)
        {
            // Get the response OutputStream and write the response to it.
            httpResponse.ContentLength64 = responseData.Length;
            httpResponse.ContentType = "text/xml";
            httpResponse.AddHeader("Content-Language", "en-au");
            httpResponse.AddHeader("Server", "Nequeo/2011.26 (Windows)");
        }

        /// <summary>
        /// Set the http response data.
        /// </summary>
        /// <param name="httpResponse">The current http response.</param>
        /// <param name="responseData">The current resposne data.</param>
        public static void SetJsonContentTypeResponse(HttpListenerResponse httpResponse, byte[] responseData)
        {
            // Get the response OutputStream and write the response to it.
            httpResponse.ContentLength64 = responseData.Length;
            httpResponse.ContentType = "text/json";
            httpResponse.AddHeader("Content-Language", "en-au");
            httpResponse.AddHeader("Server", "Nequeo/2011.26 (Windows)");
        }

        /// <summary>
        /// Get the file list.
        /// </summary>
        /// <param name="baseDirectoryPath">The base directory.</param>
        /// <param name="uploadFileListName">The html page upload file name list handler.</param>
        /// <param name="directoryQuery">The directory query value.</param>
        /// <param name="deleteFilesEnable">Enable deletion of files.</param>
        /// <param name="deleteDirectoryEnable">Enable deletion of directories.</param>
        /// <returns>The new html page with the list of files.</returns>
        public static byte[] GetUploadedFileListHtml(string baseDirectoryPath, string uploadFileListName, string directoryQuery, 
            Boolean deleteFilesEnable = false, Boolean deleteDirectoryEnable = false)
        {
            byte[] buffer = null;

            try
            {
                // Get the base path directory
                string directoryPath = baseDirectoryPath.TrimEnd('\\') + "\\";
                StringBuilder fileList = new StringBuilder();

                // Build the html page.
                fileList.Append("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">");
                fileList.Append("<html>");
                fileList.Append("<head>");
                fileList.Append("<title>Uploaded File List</title>");
                fileList.Append("</head>");
                fileList.Append("<body>");
                fileList.Append("<div style=\"display:block;width:800px;\">");

                // If the directory query is not null or empty.
                if (!string.IsNullOrEmpty(directoryQuery))
                {
                    // Get the current absolut directory.
                    // Get all the individual directories
                    string absoluteDirectory = directoryQuery;
                    string[] absoluteDirectories = absoluteDirectory.Split(new char[] { '\\' });
                    string directoryParent = null;

                    // Build the top level directory for the current directory
                    for (int i = 0; i < absoluteDirectories.Length - 1; i++)
                        directoryParent += absoluteDirectories[i] + "\\";

                    fileList.Append("<div style=\"display:inline-block;width:790px;height:30px;\">");
                    fileList.Append("<a href=\"" + uploadFileListName + "\">To Root Directory</a>");
                    fileList.Append("</div>");

                    // If a parent directory exists then display the html
                    if (!string.IsNullOrEmpty(directoryParent))
                    {
                        fileList.Append("<div style=\"display:inline-block;width:790px;height:30px;\">");
                        fileList.Append("<a href=\"" + uploadFileListName + "?directory=" + HttpUtility.UrlPathEncode(HttpUtility.UrlPathEncode(directoryParent.TrimEnd('\\'))) + "\">To Parent Directory</a>");
                        fileList.Append("</div>");
                    }
                    else
                    {
                        // A top level directory does not exist.
                        fileList.Append("<div style=\"display:inline-block;width:790px;height:30px;\">");
                        fileList.Append("<a href=\"" + uploadFileListName + "\">To Parent Directory</a>");
                        fileList.Append("</div>");
                    }

                    // Build the top level file system.
                    BuildHtmlFileSystem(fileList, directoryPath, uploadFileListName, directoryQuery, deleteFilesEnable, deleteDirectoryEnable);
                }
                else
                {
                    // Build the top level file system.
                    BuildHtmlFileSystem(fileList, directoryPath, uploadFileListName, "", deleteFilesEnable, deleteDirectoryEnable);
                }

                fileList.Append("</div>");
                fileList.Append("</body>");
                fileList.Append("</html>");

                // Return the html page.
                buffer = System.Text.Encoding.UTF8.GetBytes(fileList.ToString());
                return buffer;
            }
            catch (Exception)
            {
                // Return no files found.
                string html = Nequeo.Net.Http.Common.HttpPageContent.Html002();
                buffer = System.Text.Encoding.UTF8.GetBytes(html);
                return buffer;
            }
        }

        /// <summary>
        /// Build the current file system html.
        /// </summary>
        /// <param name="fileList">The string builder.</param>
        /// <param name="baseDirectoryPath">The base upload path.</param>
        /// <param name="uploadFileListName">The html page upload file name list handler.</param>
        /// <param name="directoryQuery">The directory query value.</param>
        /// <param name="deleteFilesEnable">Enable deletion of files.</param>
        /// <param name="deleteDirectoryEnable">Enable deletion of directories.</param>
        private static void BuildHtmlFileSystem(StringBuilder fileList, string baseDirectoryPath, string uploadFileListName, 
            string directoryQuery, Boolean deleteFilesEnable = false, Boolean deleteDirectoryEnable = false)
        {
            // Get all the current directory list.
            string[] directories = Directory.GetDirectories(baseDirectoryPath + (!string.IsNullOrEmpty(directoryQuery) ? directoryQuery + "\\" : ""));

            // Create the list of directories.
            foreach (string item in directories)
            {
                // Get specific directory names.
                string directory = item.Replace(baseDirectoryPath, "");
                string[] directoryNames = directory.Split(new char[] { '\\' });

                // Build the directory list.
                fileList.Append("<div style=\"display:inline-block;width:790px;height:30px;\">");

                // Enable deletion of directories.
                if (deleteDirectoryEnable)
                    fileList.Append("<a style=\"display:inline-block;width:200px;\" href=\"" + uploadFileListName + "?deletedirectory=" + HttpUtility.UrlPathEncode(directory.TrimEnd('\\').Replace("\\", "/")) + "\">Delete Directory</a>");
                
                fileList.Append("<a href=\"" + uploadFileListName + "?directory=" + HttpUtility.UrlPathEncode(directory.TrimEnd('\\')) + "\">" + (!string.IsNullOrEmpty(directoryQuery) ? directoryNames[directoryNames.Length - 1] : directory) + "</a>");
                fileList.Append("</div>");
            }

            // Get all the files in the directory
            string[] files = Directory.GetFiles(baseDirectoryPath + (!string.IsNullOrEmpty(directoryQuery) ? directoryQuery + "\\" : ""));

            // Create the list of files.
            foreach (string item in files)
            {
                // Get the file url.
                string urlPath = (!string.IsNullOrEmpty(directoryQuery) ? directoryQuery.Replace("\\", "/").TrimEnd('/') + "/" : "") + System.IO.Path.GetFileName(item);

                fileList.Append("<div style=\"display:inline-block;width:790px;height:30px;\">");

                // Enable deletion of files.
                if (deleteFilesEnable)
                    fileList.Append("<a style=\"display:inline-block;width:200px;\" href=\"" + uploadFileListName + "?deletefile=" + HttpUtility.UrlPathEncode(urlPath) + "\">Delete File</a>");

                fileList.Append("<span style=\"display:inline-block;width:160px;\">" + String.Format("{0:#,#} Bytes", new System.IO.FileInfo(item).Length) + "</span>");
                fileList.Append("<a href=\"" + HttpUtility.UrlPathEncode(urlPath) + "\">" + System.IO.Path.GetFileName(item) + "</a>");
                fileList.Append("</div>");
            }
        }

        /// <summary>
        /// Get the file list.
        /// </summary>
        /// <param name="uploadFileListName">The html page upload file name list handler.</param>
        /// <param name="baseDirectoryPath">The base upload path.</param>
        /// <param name="directoryQuery">The directory query value.</param>
        /// <returns>The new html page with the list of files.</returns>
        public static string GetUploadedFileListHtmlEx(string uploadFileListName, string baseDirectoryPath, string directoryQuery)
        {
            try
            {
                // Get the base path directory
                string directoryPath = baseDirectoryPath.TrimEnd('\\') + "\\";
                StringBuilder fileList = new StringBuilder();

                // Build the html page.
                fileList.Append("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">");
                fileList.Append("<html>");
                fileList.Append("<head>");
                fileList.Append("<title>Uploaded File List</title>");
                fileList.Append("</head>");
                fileList.Append("<body>");
                fileList.Append("<div style=\"display:block;width:800px;\">");

                // If the directory query is not null or empty.
                if (!string.IsNullOrEmpty(directoryQuery))
                {
                    // Get the current absolut directory.
                    // Get all the individual directories
                    string absoluteDirectory = directoryQuery;
                    string[] absoluteDirectories = absoluteDirectory.Split(new char[] { '\\' });
                    string directoryParent = null;

                    // Build the top level directory for the current directory
                    for (int i = 0; i < absoluteDirectories.Length - 1; i++)
                        directoryParent += absoluteDirectories[i] + "\\";

                    fileList.Append("<div style=\"display:inline-block;width:790px;height:30px;\">");
                    fileList.Append("<a href=\"" + uploadFileListName + "\">To Root Directory</a>");
                    fileList.Append("</div>");

                    // If a parent directory exists then display the html
                    if (!string.IsNullOrEmpty(directoryParent))
                    {
                        fileList.Append("<div style=\"display:inline-block;width:790px;height:30px;\">");
                        fileList.Append("<a href=\"" + uploadFileListName + "?directory=" + HttpUtility.UrlPathEncode(HttpUtility.UrlPathEncode(directoryParent.TrimEnd('\\'))) + "\">To Parent Directory</a>");
                        fileList.Append("</div>");
                    }
                    else
                    {
                        // A top level directory does not exist.
                        fileList.Append("<div style=\"display:inline-block;width:790px;height:30px;\">");
                        fileList.Append("<a href=\"" + uploadFileListName + "\">To Parent Directory</a>");
                        fileList.Append("</div>");
                    }

                    // Build the top level file system.
                    BuildHtmlFileSystem(fileList, directoryPath, uploadFileListName, directoryQuery);
                }
                else
                {
                    // Build the top level file system.
                    BuildHtmlFileSystem(fileList, directoryPath, uploadFileListName, "");
                }

                fileList.Append("</div>");
                fileList.Append("</body>");
                fileList.Append("</html>");

                // Return the html page.
                return fileList.ToString();
            }
            catch (Exception)
            {
                // Return no files found.
                return Nequeo.Net.Http.Common.HttpPageContent.Html002();
            }
        }

        /// <summary>
        /// Build the current file system html.
        /// </summary>
        /// <param name="fileList">The string builder.</param>
        /// <param name="baseDirectoryPath">The base upload path.</param>
        /// <param name="uploadFileListName">The html page upload file name list handler.</param>
        /// <param name="directoryQuery">The directory query value.</param>
        public static void BuildHtmlFileSystemEx(StringBuilder fileList, string baseDirectoryPath, string uploadFileListName, string directoryQuery)
        {
            // Get all the current directory list.
            string[] directories = System.IO.Directory.GetDirectories(baseDirectoryPath + (!string.IsNullOrEmpty(directoryQuery) ? directoryQuery + "\\" : ""));

            // Create the list of directories.
            foreach (string item in directories)
            {
                // Get specific directory names.
                string directory = item.Replace(baseDirectoryPath, "");
                string[] directoryNames = directory.Split(new char[] { '\\' });

                // Build the directory list.
                fileList.Append("<div style=\"display:inline-block;width:790px;height:30px;\">");
                fileList.Append("<a style=\"display:inline-block;width:200px;\" href=\"" + uploadFileListName + "?deletedirectory=" + HttpUtility.UrlPathEncode(directory.TrimEnd('\\').Replace("\\", "/")) + "\">Delete Directory</a>");
                fileList.Append("<a href=\"" + uploadFileListName + "?directory=" + HttpUtility.UrlPathEncode(directory.TrimEnd('\\')) + "\">" + (!string.IsNullOrEmpty(directoryQuery) ? directoryNames[directoryNames.Length - 1] : directory) + "</a>");
                fileList.Append("</div>");
            }

            // Get all the files in the directory
            string[] files = System.IO.Directory.GetFiles(baseDirectoryPath + (!string.IsNullOrEmpty(directoryQuery) ? directoryQuery + "\\" : ""));

            // Create the list of files.
            foreach (string item in files)
            {
                // Get the file url.
                string urlPath = (!string.IsNullOrEmpty(directoryQuery) ? directoryQuery.Replace("\\", "/").TrimEnd('/') + "/" : "") + System.IO.Path.GetFileName(item);

                fileList.Append("<div style=\"display:inline-block;width:790px;height:30px;\">");
                fileList.Append("<a style=\"display:inline-block;width:200px;\" href=\"" + uploadFileListName + "?deletefile=" + HttpUtility.UrlPathEncode(urlPath) + "\">Delete File</a>");
                fileList.Append("<span style=\"display:inline-block;width:160px;\">" + String.Format("{0:#,#} Bytes", new System.IO.FileInfo(item).Length) + "</span>");
                fileList.Append("<a href=\"" + HttpUtility.UrlPathEncode(urlPath) + "\">" + System.IO.Path.GetFileName(item) + "</a>");
                fileList.Append("</div>");
            }
        }
    }
}
