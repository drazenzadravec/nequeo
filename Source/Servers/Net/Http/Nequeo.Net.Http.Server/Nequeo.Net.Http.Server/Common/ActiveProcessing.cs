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

using Nequeo.Handler;

namespace Nequeo.Net.Common
{
    /// <summary>
    /// Active http context.
    /// </summary>
    [Serializable]
    [Logging(Handler.WriteTo.EventLog, Handler.LogType.Error)]
    public class ActiveHttpContext : MarshalByRefObject, Nequeo.Net.Http.IHttpListenerContext
    {
        private System.Net.HttpListenerRequest _request = null;
        private System.Net.HttpListenerResponse _response = null;
        private System.Security.Principal.IPrincipal _user = null;

        /// <summary>
        /// Gets or sets the http request.
        /// </summary>
        public System.Net.HttpListenerRequest Request
        {
            get { return _request; }
            set { _request = value; }
        }

        /// <summary>
        /// Gets or sets the http response.
        /// </summary>
        public System.Net.HttpListenerResponse Response
        {
            get { return _response; }
            set { _response = value; }
        }

        /// <summary>
        /// Gets or sets the http user principal.
        /// </summary>
        public System.Security.Principal.IPrincipal User
        {
            get { return _user; }
            set { _user = value; }
        }
    }

    /// <summary>
    /// Active processing.
    /// </summary>
    [Serializable]
    [Logging(Handler.WriteTo.EventLog, Handler.LogType.Error)]
    public class ActiveProcessing : MarshalByRefObject, Nequeo.Net.Http.IActiveProcessing
    {
        private NameValueCollection _form = null;
        private string[] _uploadFiles = null;
        private Nequeo.Net.Data.context _contextMimeType = null;
        private bool _isPostBack = false;

        /// <summary>
        /// Is the request a post back.
        /// </summary>
        public bool IsPostBack
        {
            get { return _isPostBack; }
            set { _isPostBack = value; }
        }

        /// <summary>
        /// Gets the post back form data; else empty collection.
        /// </summary>
        public NameValueCollection Form
        {
            get { return _form; }
            set { _form = value; }
        }

        /// <summary>
        /// Gets the collection of files that are in the postback data; else empty collection.
        /// </summary>
        public string[] UploadFiles
        {
            get { return _uploadFiles; }
            set { _uploadFiles = value; }
        }

        /// <summary>
        /// Gets the content mime type.
        /// </summary>
        public Nequeo.Net.Data.context MimeType
        {
            get { return _contextMimeType; }
            set { _contextMimeType = value; }
        }

        /// <summary>
        /// Start processing post back data from the request.
        /// </summary>
        /// <param name="request">The http request context.</param>
        /// <param name="uploadDirectory">The upload directory path where files are placed; else uploaded files are ingored.</param>
        public void ProcessPostBack(System.Net.HttpListenerRequest request, string uploadDirectory = null)
        {
            System.IO.Stream input = null;
            System.IO.FileStream localDestination = null;
            System.IO.MemoryStream memoryFormData = null;

            try
            {
                // If no directory path then only get the form post back data.
                if (String.IsNullOrEmpty(uploadDirectory))
                {
                    // Create a new memory stream that will contain the form data
                    using (memoryFormData = new System.IO.MemoryStream())
                    {
                        input = request.InputStream;

                        // Copy the request stream data to the file stream.
                        Nequeo.Net.Http.Utility.TransferData(input, memoryFormData);

                        // Flush the streams.
                        input.Flush();
                        memoryFormData.Flush();

                        // Close the local file.
                        memoryFormData.Close();
                        input.Close();
                    }

                    // Get the form data uploaded from the request stream
                    // within the memory stream
                    byte[] formByteData = memoryFormData.ToArray();
                    string formStringData = Encoding.ASCII.GetString(formByteData);

                    // Get the enumerable collection of for data lines.
                    IEnumerable<string> formLines = formStringData.Split(new string[] { "\r\n" }, StringSplitOptions.None).AsEnumerable();

                    // Get the form post back data.
                    _form = Nequeo.Net.Http.Utility.FormParser(formLines);
                    _uploadFiles = new string[0];
                }
                else
                {
                    string directory = uploadDirectory.TrimEnd('\\') + "\\";

                    // The request is a file uploader.
                    Nequeo.Net.Http.Utility.CreateDirectory(directory);
                    string localFileName = directory + Guid.NewGuid().ToString() + ".txt";

                    // Create the new file and start the transfer process.
                    using (localDestination = new System.IO.FileStream(localFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite))
                    {
                        input = request.InputStream;

                        // Copy the request stream data to the file stream.
                        Nequeo.Net.Http.Utility.TransferData(input, localDestination);

                        // Flush the streams.
                        input.Flush();
                        localDestination.Flush();

                        // Close the local file.
                        localDestination.Close();
                        input.Close();
                    }

                    // Get the form post back data.
                    _form = Nequeo.Net.Http.Utility.FormParser(System.IO.File.ReadLines(localFileName));

                    // Get the upload file collection.
                    _uploadFiles = Nequeo.Net.Http.Utility.ParseUploadedFileEx(localFileName);
                }
            }
            catch (Exception ex)
            {
                // Log the error.
                LogHandler.WriteTypeMessage(
                    ex.Message,
                    MethodInfo.GetCurrentMethod(),
                    Nequeo.Net.Common.Helper.EventApplicationName);

                throw;
            }
            finally
            {
                try
                {
                    if (input != null)
                        input.Close();
                }
                catch { }

                try
                {
                    if (memoryFormData != null)
                        memoryFormData.Close();
                }
                catch { }

                try
                {
                    if (localDestination != null)
                        localDestination.Close();
                }
                catch { }
            }
        }

        /// <summary>
        /// Get the response mime content type for the extension.
        /// </summary>
        /// <param name="extension">The extension name.</param>
        /// <returns>The mime content type.</returns>
        public string GetMimeContentType(string extension)
        {
            return ActiveProcessing.GetMimeContentType(_contextMimeType, extension);
        }

        /// <summary>
        /// Gets the list of allowed extensions.
        /// </summary>
        /// <returns>The list of extensions.</returns>
        public string[] AllowedExtensions()
        {
            return _contextMimeType.fileExtensionAllowList.Split(new char[] { ';' });
        }

        /// <summary>
        /// Parse a uploaded content file to the original uploaded data.
        /// </summary>
        /// <param name="uploadedFilePath">The current temp file with upload content.</param>
        internal static void ParseUploadedFile(string uploadedFilePath)
        {
            try
            {
                string[] ret = Nequeo.Net.Http.Utility.ParseUploadedFileEx(uploadedFilePath);
            }
            catch (Exception ex)
            {
                // Log the error.
                LogHandler.WriteTypeMessage(
                    ex.Message,
                    MethodInfo.GetCurrentMethod(),
                    Nequeo.Net.Common.Helper.EventApplicationName);
            }
        }

        /// <summary>
        /// Get a specific mime content type for the current extension.
        /// </summary>
        /// <param name="mimeTypeContext">The mime type context container..</param>
        /// <param name="currentExtension">The current extension.</param>
        /// <returns>The new mime content type.</returns>
        internal static string GetMimeContentType(Nequeo.Net.Data.context mimeTypeContext, string currentExtension)
        {
            // Find the mime type for the current extension.
            IEnumerable<Nequeo.Net.Data.contextMimeType> mimeTypes = 
                mimeTypeContext.mimeTypes.Where(u => u.ext.ToLower() == currentExtension.ToLower());

            if (mimeTypes != null)
                if (mimeTypes.Count() > 0)
                    return mimeTypes.First().mime;

            // Default if not found.
            return "text/plain";
        }

        /// <summary>
        /// Get the collection of save paths used.
        /// </summary>
        /// <param name="mimeTypeContext">The mime type context container..</param>
        /// <returns>The collection of base paths.</returns>
        internal static string[] GetSavePaths(Nequeo.Net.Data.context mimeTypeContext)
        {
            // Get the list.
            Nequeo.Net.Data.contextUploadFile[] uploaded = mimeTypeContext.uploadFiles;
            List<string> uploadedBasePaths = new List<string>();

            if (uploaded != null)
                if (uploaded.Count() > 0)
                    foreach (Nequeo.Net.Data.contextUploadFile item in uploaded)
                        uploadedBasePaths.Add(item.savePath.TrimEnd('\\') + "\\");

            // Return the collection of base paths.
            return uploadedBasePaths.ToArray();
        }

        /// <summary>
        /// Get the collection of base paths used.
        /// </summary>
        /// <param name="mimeTypeContext">The mime type context container..</param>
        /// <returns>The collection of base paths.</returns>
        internal static string[] GetBasePaths(Nequeo.Net.Data.context mimeTypeContext)
        {
            // Get the list.
            Nequeo.Net.Data.contextUploadFile[] uploaded = mimeTypeContext.uploadFiles;
            List<string> uploadedBasePaths = new List<string>();

            if (uploaded != null)
                if (uploaded.Count() > 0)
                    foreach (Nequeo.Net.Data.contextUploadFile item in uploaded)
                        uploadedBasePaths.Add(item.basePath.TrimEnd('\\') + "\\");

            // Return the collection of base paths.
            return uploadedBasePaths.ToArray();
        }

        /// <summary>
        /// Get the specific upload file save path.
        /// </summary>
        /// <param name="mimeTypeContext">The mime type context container..</param>
        /// <param name="fileName">The current file; if an uploader file.</param>
        /// <param name="directory">The directory to find a match for.</param>
        /// <returns>The local file directory where files are to be saved.</returns>
        internal static string UploaderSavePath(Nequeo.Net.Data.context mimeTypeContext, string fileName, string directory)
        {
            // Find the matching directory upload path.
            IEnumerable<Nequeo.Net.Data.contextUploadFile> uploadFiles =
                mimeTypeContext.uploadFiles.Where(u =>
                    (u.basePath.TrimEnd('\\').ToLower() == directory.TrimEnd('\\').ToLower()) &&
                    (u.fileUploader.ToLower() == fileName.ToLower()));

            if (uploadFiles != null)
                if (uploadFiles.Count() > 0)
                    return uploadFiles.First().savePath.TrimEnd('\\') + "\\";

            // Default is not found.
            return null;
        }

        /// <summary>
        /// Get the specific upload file save path.
        /// </summary>
        /// <param name="mimeTypeContext">The mime type context container..</param>
        /// <param name="fileName">The current file; if an uploader file.</param>
        /// <param name="directory">The directory to find a match for.</param>
        /// <returns>The local file directory where files are to be saved.</returns>
        internal static long UploaderMaxUploadFileZise(Nequeo.Net.Data.context mimeTypeContext, string fileName, string directory)
        {
            // Find the matching directory upload path.
            IEnumerable<Nequeo.Net.Data.contextUploadFile> uploadFiles =
                mimeTypeContext.uploadFiles.Where(u =>
                    (u.basePath.TrimEnd('\\').ToLower() == directory.TrimEnd('\\').ToLower()) &&
                    (u.fileUploader.ToLower() == fileName.ToLower()));

            if (uploadFiles != null)
                if (uploadFiles.Count() > 0)
                    return uploadFiles.First().maxUploadFileSize;

            // Default is not found.
            return 0;
        }

        /// <summary>
        /// Get the specific uploaded files list save path.
        /// </summary>
        /// <param name="mimeTypeContext">The mime type context container..</param>
        /// <param name="fileName">The current file; if an uploaded files list file.</param>
        /// <param name="directory">The directory to find a match for.</param>
        /// <returns>The local file directory where files are to be saved.</returns>
        internal static string UploadedFilesListSavePath(Nequeo.Net.Data.context mimeTypeContext, string fileName, string directory)
        {
            // Find the matching directory upload path.
            IEnumerable<Nequeo.Net.Data.contextUploadFile> uploadFiles =
                mimeTypeContext.uploadFiles.Where(u =>
                    (u.basePath.TrimEnd('\\').ToLower() == directory.TrimEnd('\\').ToLower()) &&
                    (u.uploadedFilesList.ToLower() == fileName.ToLower()));

            if (uploadFiles != null)
                if (uploadFiles.Count() > 0)
                    return uploadFiles.First().savePath.TrimEnd('\\') + "\\";

            // Default is not found.
            return null;
        }
    }
}
