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
using System.Security.Principal;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Configuration;
using System.Net;
using System.IO;

using Nequeo.Net.Http.Common;

namespace Nequeo.Net.Http
{
    /// <summary>
    /// Http service base.
    /// </summary>
    [Serializable]
    public abstract class HttpServiceBase : MarshalByRefObject, IDisposable
	{
        /// <summary>
        /// Http service base.
        /// </summary>
        /// <param name="serviceVirtualName">The service virtual name.</param>
        protected HttpServiceBase(string serviceVirtualName) 
        {
            if (string.IsNullOrEmpty(serviceVirtualName)) throw new ArgumentNullException("serviceVirtualName");

            _serviceVirtualName = serviceVirtualName;
        }

        private string _serviceVirtualName = null;

        /// <summary>
        /// Gets or sets the base service path
        /// </summary>
        public abstract string BaseServicePath { get; set; }

        /// <summary>
        /// Gets or sets the base maximun post back content length
        /// </summary>
        public abstract int MaxBaseContentSize { get; set; }

        /// <summary>
        /// Gets or sets the base upload path.
        /// </summary>
        public virtual string BaseUploadPath { get; set; }

        /// <summary>
        /// Gets or sets the base service namespace.
        /// </summary>
        public virtual string BaseNameSpace { get; set; }

        /// <summary>
        /// Gets or sets the service assembly name.
        /// </summary>
        public virtual string AssemblyName { get; set; }

        /// <summary>
        /// Sens the response data to the client.
        /// </summary>
        /// <param name="context">Provides access to the request and response objects.</param>
        /// <param name="response">The array of response bytes.</param>
        public virtual void SendResponseData(IHttpContext context, byte[] response)
        {
            System.IO.Stream outputResponse = null;
            System.IO.MemoryStream memoryInput = null;

            HttpListenerResponse httpResponse = null;

            try
            {
                // Get the request and response context.
                httpResponse = context.HttpContext.Response;

                // Create the response.
                memoryInput = HttpResponseContent.CreateResponseStream(response);

                // Transfer the data.
                outputResponse = httpResponse.OutputStream;
                HttpResponseContent.TransferResponse(memoryInput, outputResponse);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (outputResponse != null)
                        outputResponse.Close();
                }
                catch { }

                try
                {
                    if (memoryInput != null)
                        memoryInput.Close();
                }
                catch { }
            }
        }

        /// <summary>
        /// Gets the request data from the client.
        /// </summary>
        /// <param name="context">Provides access to the request and response objects.</param>
        /// <returns>The array of request bytes</returns>
        public virtual byte[] GetRequestData(IHttpContext context)
        {
            System.IO.Stream inputRequest = null;
            System.IO.MemoryStream memoryOutput = null;

            HttpListenerRequest httpRequest = null;

            try
            {
                // Get the request and response context.
                httpRequest = context.HttpContext.Request;

                // Read the request stream data and write
                // to the memory stream.
                inputRequest = httpRequest.InputStream;
                memoryOutput = new System.IO.MemoryStream();
                HttpResponseContent.TransferData(inputRequest, memoryOutput);

                // Properly flush and close the output stream
                inputRequest.Flush();
                memoryOutput.Flush();
                inputRequest.Close();
                memoryOutput.Close();

                // Get the request data.
                byte[] requestData = memoryOutput.ToArray();

                // Return the request data.
                return requestData;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (inputRequest != null)
                        inputRequest.Close();
                }
                catch { }

                try
                {
                    if (memoryOutput != null)
                        memoryOutput.Close();
                }
                catch { }
            }
        }

        /// <summary>
        /// Process the http server resource request.
        /// </summary>
        /// <param name="context">Provides access to the request and response objects.</param>
        public virtual void HttpServiceProcessRequest(IHttpContext context)
        {
            System.IO.Stream outputResponse = null;
            System.IO.Stream inputRequest = null;
            System.IO.MemoryStream memoryOutput = null;
            System.IO.MemoryStream memoryInput = null;

            HttpListenerRequest httpRequest = null;
            HttpListenerResponse httpResponse = null;

            try
            {
                // Get the upload file directory.
                string serviceDirectory = BaseServicePath.TrimEnd('\\') + "\\";
                string uploadFilePath = null;
                bool foundInDownload = false;

                // Get the request and response context.
                httpRequest = context.HttpContext.Request;
                httpResponse = context.HttpContext.Response;

                // Get the current resposne stream.
                outputResponse = httpResponse.OutputStream;

                // Get the current request filename and directory information.
                string absolutePath = HttpUtility.UrlDecode(httpRequest.Url.AbsolutePath.TrimStart('/').Replace("/", "\\"));
                string urlFilePath = serviceDirectory + absolutePath;
                bool fileExists = System.IO.File.Exists(urlFilePath);

                // Look in the base download path for the file.
                if (!fileExists)
                {
                    // If an upload path has been supplied.
                    if (!string.IsNullOrEmpty(BaseUploadPath))
                    {
                        // Get the upload directory.
                        string uploadDirectory = BaseUploadPath.TrimEnd('\\') + "\\";
                        string[] directories = Path.GetDirectoryName(absolutePath).Split(new char[] { '\\' });

                        // Get the directory query string.
                        string directory = "";
                        bool foundStart = false;
                        foreach (string item in directories)
                        {
                            // Find the starting virtual directory name associated
                            // with the current service name.
                            if (item.ToLower() == _serviceVirtualName.ToLower())
                                foundStart = true;

                            // Build the directory.
                            if (foundStart)
                                directory += item + "\\";
                        }

                        // Get the download file path.
                        uploadFilePath = uploadDirectory +
                            (string.IsNullOrEmpty(directory) ? "" : directory.ToLower().Replace(_serviceVirtualName.ToLower() + "\\", "").TrimEnd('\\') + "\\") +
                            System.IO.Path.GetFileName(urlFilePath);

                        // Does the file exist.
                        fileExists = System.IO.File.Exists(uploadFilePath);

                        // If the file is found in the download path.
                        if (fileExists)
                            foundInDownload = true;
                    }
                }

                // If the file does not exists then try to load
                // the default.htm file.
                if (!fileExists)
                {
                    string newUrlFilePath = urlFilePath.TrimEnd('\\') + "\\";
                    string newFileName = System.IO.Path.GetFileName(newUrlFilePath);

                    // Create the new default url file name.
                    if (String.IsNullOrEmpty(newFileName))
                    {
                        urlFilePath = newUrlFilePath + "default.htm";
                        fileExists = System.IO.File.Exists(urlFilePath);
                    }
                }

                // Does the resource exits on the server.
                if (fileExists)
                {
                    // Get the extension of the resource.
                    string extension = System.IO.Path.GetExtension(urlFilePath).TrimStart(new char[] { '.' });
                    string fileName = System.IO.Path.GetFileName(urlFilePath);
                    string[] extensions = context.ActiveProcess.AllowedExtensions();

                    // Get the maximum upload file size.
                    int maxUploadFileSize = MaxBaseContentSize;

                    // If the file is not too large.
                    if (httpRequest.ContentLength64 >= maxUploadFileSize)
                    {
                        // Get the response html. Create the response. Transfer the data.
                        byte[] responseMaxHtmlData = HttpResponseContent.MaxContentLength(httpResponse, maxUploadFileSize);
                        HttpResponseContent.TransferResponse(HttpResponseContent.CreateResponseStream(responseMaxHtmlData), outputResponse);
                    }
                    else
                    {
                        // Extension is allowed and the resource is
                        // not a download file.
                        if (extensions.Count(u => u.Contains(extension)) > 0 && !foundInDownload)
                        {
                            // Execute the correct page code behind.
                            bool ret = HttpPageBase.PageInstance(BaseNameSpace, AssemblyName, urlFilePath, context);

                            // If the resource code behind does not exist
                            // then load only the content data.
                            if (!ret)
                            {
                                // If no code behind then load the resource.
                                byte[] responseResourceHtmlData = HttpResponseContent.ResourceFound(httpResponse, context.ActiveProcess, urlFilePath, extension);
                                HttpResponseContent.TransferResponse(HttpResponseContent.CreateResponseStream(responseResourceHtmlData), outputResponse);
                            }
                        }
                        else
                        {
                            // Get the response html. Create the response. Transfer the data.
                            byte[] responseFoundAttachmentHtmlData = HttpResponseContent.ResourceAttachment(httpResponse, uploadFilePath, extension, fileName);
                            HttpResponseContent.TransferResponse(HttpResponseContent.CreateResponseStream(responseFoundAttachmentHtmlData), outputResponse);
                        }
                    }
                }
                else
                {
                    // Get the response html. Create the response. Transfer the data.
                    byte[] responseNotFoundHtmlData = HttpResponseContent.FileNotFound(httpResponse);
                    HttpResponseContent.TransferResponse(HttpResponseContent.CreateResponseStream(responseNotFoundHtmlData), outputResponse);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    if (httpResponse != null)
                    {
                        // Get the response html. Create the response. Transfer the data.
                        byte[] responseErrorHtmlData = HttpResponseContent.OnError(httpResponse, ex);
                        memoryInput = HttpResponseContent.CreateResponseStream(responseErrorHtmlData);

                        // If the response stream has already been activated.
                        if (outputResponse == null)
                        {
                            // Transfer the data.
                            outputResponse = httpResponse.OutputStream;
                            HttpResponseContent.TransferResponse(memoryInput, outputResponse);
                        }
                        else
                            // Send the response.
                            HttpResponseContent.TransferResponse(memoryInput, outputResponse);
                    }
                }
                catch { }

                throw;
            }
            finally
            {
                try
                {
                    if (inputRequest != null)
                        inputRequest.Close();
                }
                catch { }

                try
                {
                    if (outputResponse != null)
                        outputResponse.Close();
                }
                catch { }

                try
                {
                    if (memoryOutput != null)
                        memoryOutput.Close();
                }
                catch { }

                try
                {
                    if (memoryInput != null)
                        memoryInput.Close();
                }
                catch { }
            }
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.

                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~HttpServiceBase()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
	}
}
