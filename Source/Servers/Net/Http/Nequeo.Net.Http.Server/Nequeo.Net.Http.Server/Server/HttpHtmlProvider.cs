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
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Reflection;
using System.Web;

using Nequeo.Net.Common;
using Nequeo.Handler;
using Nequeo.Net.Configuration;

namespace Nequeo.Net.Server
{
    /// <summary>
    /// Used to over ride the ClientValidation method
    /// hehavior, when set the ClientValidation method default behavior is over written.
    /// </summary>
    /// <param name="user">Defines the basic functionality of a principal object.</param>
    /// <param name="authenticationSchemes">Specifies protocols for authentication.</param>
    /// <returns>True if the client has been validated; else false.</returns>
    public delegate Boolean ClientAuthenticationValidator(System.Security.Principal.IPrincipal user, AuthenticationSchemes authenticationSchemes);

    /// <summary>
    /// Http html server
    /// </summary>
    [Logging(Handler.WriteTo.EventLog, Handler.LogType.Error)]
    public class HttpHtmlProvider : ServiceBase, IDisposable
    {
        /// <summary>
        /// Http html server
        /// </summary>
        /// <param name="uriList">The url prefix listening list</param>
        /// <param name="providerName">The provider name (host="htmlprovider") in the configuration file.</param>
        public HttpHtmlProvider(string[] uriList = null, string providerName = "htmlprovider")
        {
            _uriList = uriList;
            _providerName = providerName;
        }

        private AuthenticationSchemes _authenticationSchemes = AuthenticationSchemes.Anonymous;
        private AuthenticationSchemeSelector _authenticationSchemeSelectorDelegate = null;
        private ClientAuthenticationValidator _clientAuthenticationValidator = null;

        private bool _running = true;
        private HttpListener _listener = null;
        private string[] _uriList = null;
        private Object _threadObject = new object();
        private string _providerName = "htmlprovider";
        private Nequeo.Net.Data.context _contextMimeType = null;

        /// <summary>
        /// Gets or sets the Uniform Resource Identifier (URI) prefixes handled by this System.Net.HttpListener object.
        /// </summary>
        public string[] UriList
        {
            get { return _uriList; }
            set { _uriList = value; }
        }

        /// <summary>
        /// Gets sets the provider name, (host="htmlprovider") in the configuration file.
        /// </summary>
        public string ProviderName
        {
            get { return _providerName; }
            set { _providerName = value; }
        }

        /// <summary>
        /// Gets sets the client authentication validator. Used to over ride the ClientValidation method
        /// hehavior, when set the ClientValidation method default behavior is over written.
        /// </summary>
        public ClientAuthenticationValidator ClientAuthenticationValidator
        {
            get { return _clientAuthenticationValidator; }
            set { _clientAuthenticationValidator = value; }
        }

        /// <summary>
        /// Gets or sets the authentication schemes
        /// </summary>
        /// <remarks>
        /// The default value is System.Net.AuthenticationSchemes.Anonymous.
        /// </remarks>
        protected AuthenticationSchemes AuthenticationSchemes
        {
            get { return _authenticationSchemes; }
            set { _authenticationSchemes = value; }
        }

        /// <summary>
        /// Gets or sets the scheme used to authenticate clients.
        /// </summary>
        /// <remarks>
        /// The default value is Null.
        /// </remarks>
        protected AuthenticationSchemeSelector AuthenticationSchemeSelector
        {
            get { return _authenticationSchemeSelectorDelegate; }
            set { _authenticationSchemeSelectorDelegate = value; }
        }

        /// <summary>
        /// Validate the client. Client is only validated if IsAuthenticated = true; all other is false.
        /// Use the ClientAuthenticationValidator delegate to over ride this behavior. Or override this
        /// method in a derived class to change the behavior
        /// </summary>
        /// <param name="user">Defines the basic functionality of a principal object.</param>
        /// <param name="authenticationSchemes">Specifies protocols for authentication.</param>
        /// <returns>True if the client has been validated; else false.</returns>
        protected virtual Boolean ClientValidation(System.Security.Principal.IPrincipal user, AuthenticationSchemes authenticationSchemes)
        {
            if (_clientAuthenticationValidator != null)
            {
                // Custom client authentication valiador.
                return _clientAuthenticationValidator(user, authenticationSchemes);
            }
            else
            {
                // Does the user priciple exist.
                if (user != null)
                {
                    // Does the user identity exist.
                    if (user.Identity != null)
                    {
                        // If the client was validated.
                        if (!user.Identity.IsAuthenticated)
                            return false;
                        else
                            return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Start the http listener.
        /// </summary>
        public void Start()
        {
            try
            {
                // Create a new http listener
                _listener = new HttpListener();

                // Add URI prefixes to listen for.
                foreach (string uri in _uriList)
                    _listener.Prefixes.Add(uri);

                // Set the Authentication Schemes.
                _listener.AuthenticationSchemes = _authenticationSchemes;
                if (_authenticationSchemeSelectorDelegate != null)
                    _listener.AuthenticationSchemeSelectorDelegate = _authenticationSchemeSelectorDelegate;

                // Get the mime types
                _contextMimeType = ReaderHttp.GetMimeType();

                // Start the listener
                _listener.Start();

                // Keep the service in the running start
                // listen for in-comming requests.
                while (_running)
                {
                    // Start a new listening thread for the
                    // current connection request.
                    IAsyncResult result = _listener.BeginGetContext(new AsyncCallback(AsynchronousListenerCallback), _listener);

                    // Wait until the current context is made and processed before continuing.
                    result.AsyncWaitHandle.WaitOne();
                }

                // If the runServer flag gets set to false, 
                // stop the server and close the listener.
                _listener.Close();
            }
            catch (Exception ex)
            {
                // Log the error.
                LogHandler.WriteTypeMessage(
                    ex.Message,
                    MethodInfo.GetCurrentMethod(),
                    Nequeo.Net.Common.Helper.EventApplicationName);
            }
            finally
            {
                if (_listener != null)
                    _listener.Close();
            }
        }

        /// <summary>
        /// Stop the http listener.
        /// </summary>
        public void Stop()
        {
            try
            {
                // Stop the service.
                _running = false;

                if (_listener != null)
                    _listener.Abort();
            }
            catch (Exception ex)
            {
                // Log the error.
                LogHandler.WriteTypeMessage(
                    ex.Message,
                    MethodInfo.GetCurrentMethod(),
                    Nequeo.Net.Common.Helper.EventApplicationName);
            }
            finally
            {
                if (_listener != null)
                    _listener.Close();
            }
        }

        /// <summary>
        /// Asynchronous listener callback result.
        /// </summary>
        /// <param name="result">The async result for the current connection.</param>
        private void AsynchronousListenerCallback(IAsyncResult result)
        {
            System.IO.Stream output = null;
            System.IO.Stream input = null;
            System.IO.FileStream localDestination = null;

            HttpListenerRequest request = null;
            HttpListenerResponse response = null;

            try
            {
                // Get the callback state.
                HttpListener listener = (HttpListener)result.AsyncState;

                // Call EndGetContext to signal the completion of the asynchronous operation.
                HttpListenerContext context = null;

                try
                {
                    // If we have aborted the server while waiting, catch the exception and terminate
                    context = listener.EndGetContext(result);
                }
                catch (ObjectDisposedException)
                {
                    return;
                }

                // If the context is not null.
                if (context != null)
                {
                    bool isClientValid = true;

                    // Attempt to validate the client.
                    if (context.User != null)
                        isClientValid = ClientValidation(context.User, _authenticationSchemes);

                    // Get the request and response context.
                    request = context.Request;
                    response = context.Response;

                    // If the user has not been validated.
                    if (!isClientValid)
                    {
                        // Construct a minimal response string.
                        string responseString = Nequeo.Net.Http.Common.HttpPageContent.Html401();
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                        // Get the response OutputStream and write the response to it.
                        response.ContentLength64 = buffer.Length;
                        response.ContentType = "text/html; charset=utf-8";
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        response.AddHeader("Content-Language", "en-au");
                        response.AddHeader("Server", "Nequeo/2011.26 (Windows)");
                        response.AddHeader("WWW-Authenticate", _authenticationSchemes.ToString());

                        // Get the current response output stream
                        // and write the response to the client.
                        output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);

                        // Properly flush and close the output stream
                        output.Flush();
                        output.Close();
                    }
                    else
                    {
                        // Get the local file path for the resource request.
                        string absolutePath = HttpUtility.UrlDecode(request.Url.AbsolutePath.TrimStart('/').Replace("/", "\\"));
                        string urlFilePath = ReaderHttp.GetBaseDirectoryPath() + absolutePath;
                        string authMode = ReaderHttp.GetProviderAuthentication(request.Url, _providerName);
                        bool fileExists = System.IO.File.Exists(urlFilePath);

                        string uploadFilePath = null;
                        bool foundInDownload = false;

                        // Look in the base upload path for the file.
                        if (!fileExists)
                        {
                            // Get the save paths.
                            string[] savePaths = ActiveProcessing.GetSavePaths(_contextMimeType);

                            // If an upload path has been supplied.
                            if (savePaths.Count() > 0)
                            {
                                // For each path found.
                                foreach (string path in savePaths)
                                {
                                    // Get the upload directory.
                                    string uploadDirectory = path.TrimEnd('\\') + "\\";
                                    string[] directories = System.IO.Path.GetDirectoryName(absolutePath).Split(new char[] { '\\' });

                                    // For each possible url prefix.
                                    foreach (string prefix in listener.Prefixes)
                                    {
                                        Uri url = new Uri(prefix);
                                        string prefixAbaolutePath = HttpUtility.UrlDecode(url.AbsolutePath.TrimStart('/').Replace("/", "\\"));

                                        // Get the directory query string.
                                        string directory = "";
                                        foreach (string item in directories)
                                            directory += item + "\\";

                                        // Get the download file path.
                                        uploadFilePath = uploadDirectory +
                                            (string.IsNullOrEmpty(directory) ? "" : directory.Replace(prefixAbaolutePath, "").TrimEnd('\\') + "\\") +
                                            System.IO.Path.GetFileName(urlFilePath);

                                        // Does the file exist.
                                        fileExists = System.IO.File.Exists(uploadFilePath);

                                        // If the file is found in the download path.
                                        if (fileExists)
                                        {
                                            foundInDownload = true;
                                            break;
                                        }
                                    }
                                    if (foundInDownload)
                                        break;
                                }
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
                            // Get the extension allow list.
                            string[] extensions = _contextMimeType.fileExtensionAllowList.Split(new char[] { ';' });
                            string extension = System.IO.Path.GetExtension(urlFilePath).TrimStart(new char[] { '.' });
                            string directory = System.IO.Path.GetDirectoryName(urlFilePath);
                            string fileName = System.IO.Path.GetFileName(urlFilePath);

                            // Extension is allowed.
                            if (extensions.Count(u => u.Contains(extension)) > 0 && !foundInDownload)
                            {
                                // Get the specific upload file save paths.
                                string uploaderSavePath = ActiveProcessing.UploaderSavePath(_contextMimeType, fileName, directory);
                                string uploadedFilesListSavePath = ActiveProcessing.UploadedFilesListSavePath(_contextMimeType, fileName, directory);

                                // If the client is posting back.
                                if (!String.IsNullOrEmpty(request.HttpMethod))
                                {
                                    // If method is anything other then POST, PUT then null.
                                    if ((!request.HttpMethod.ToLower().Contains("post")) && (!request.HttpMethod.ToLower().Contains("put")))
                                        // If not posting back then not an uploader operation.
                                        uploaderSavePath = null;
                                }
                                else
                                    // If no request method then no uploader operation.
                                    uploaderSavePath = null;

                                // If the request is a file uploader.
                                if (!String.IsNullOrEmpty(uploaderSavePath))
                                {
                                    string localFileName = null;
                                    try
                                    {
                                        // Get the maximum upload file size.
                                        long maxUploadFileSize = ActiveProcessing.UploaderMaxUploadFileZise(_contextMimeType, fileName, directory);

                                        // If the file is not to large.
                                        if (request.ContentLength64 <= maxUploadFileSize)
                                        {
                                            // The request is a file uploader.
                                            Nequeo.Net.Http.Utility.CreateDirectory(uploaderSavePath);
                                            localFileName = uploaderSavePath + Guid.NewGuid().ToString() + ".txt";

                                            // Create the new file and start the transfer process.
                                            localDestination = new System.IO.FileStream(localFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
                                            input = request.InputStream;

                                            // Copy the request stream data to the file stream.
                                            Nequeo.Net.Http.Utility.TransferData(input, localDestination);

                                            // Flush the streams.
                                            input.Flush();
                                            localDestination.Flush();

                                            // Close the local file.
                                            localDestination.Close();
                                            input.Close();

                                            // Construct a minimal response string.
                                            string responseString = Nequeo.Net.Http.Common.HttpPageContent.Html001();
                                            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                                            // Get the response OutputStream and write the response to it.
                                            response.ContentLength64 = buffer.Length;
                                            response.ContentType = "text/html; charset=utf-8";
                                            response.AddHeader("Content-Language", "en-au");
                                            response.AddHeader("Server", "Nequeo/2011.26 (Windows)");
                                            response.AddHeader("WWW-Authenticate", (String.IsNullOrEmpty(authMode) ? "none" : authMode.ToLower()));

                                            // Get the current response output stream
                                            // and write the response to the client.
                                            output = response.OutputStream;
                                            output.Write(buffer, 0, buffer.Length);

                                            // Properly flush and close the output stream
                                            output.Flush();
                                            output.Close();

                                            // Start a async uploaded file parser.
                                            Action<string> fileParserHandler = new Action<string>(ActiveProcessing.ParseUploadedFile);
                                            IAsyncResult ar = fileParserHandler.BeginInvoke(localFileName, null, null);
                                        }
                                        else
                                        {
                                            // Construct a minimal response string.
                                            string responseString = Nequeo.Net.Http.Common.HttpPageContent.Html003(maxUploadFileSize);
                                            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                                            // Get the response OutputStream and write the response to it.
                                            response.ContentLength64 = buffer.Length;
                                            response.StatusCode = (int)HttpStatusCode.RequestEntityTooLarge;
                                            response.ContentType = "text/html; charset=utf-8";
                                            response.AddHeader("Content-Language", "en-au");
                                            response.AddHeader("Server", "Nequeo/2011.26 (Windows)");
                                            response.AddHeader("WWW-Authenticate", (String.IsNullOrEmpty(authMode) ? "none" : authMode.ToLower()));

                                            // Get the current response output stream
                                            // and write the response to the client.
                                            output = response.OutputStream;
                                            output.Write(buffer, 0, buffer.Length);

                                            // Properly flush and close the output stream
                                            output.Flush();
                                            output.Close();
                                        }
                                    }
                                    catch (Exception upex)
                                    {
                                        // Log the error.
                                        LogHandler.WriteTypeMessage(
                                            upex.Message,
                                            MethodInfo.GetCurrentMethod(),
                                            Nequeo.Net.Common.Helper.EventApplicationName);

                                        // Close the local file.
                                        if (localDestination != null)
                                            localDestination.Close();

                                        // If the local file exits the delete it.
                                        if (!String.IsNullOrEmpty(localFileName))
                                            if (System.IO.File.Exists(localFileName))
                                                System.IO.File.Delete(localFileName);

                                        // Throw the exception.
                                        throw;
                                    }
                                }
                                else
                                {
                                    // If the request is a uploaded file list.
                                    if (!String.IsNullOrEmpty(uploadedFilesListSavePath))
                                    {
                                        // Lock the current thread.
                                        lock (_threadObject)
                                        {
                                            string directoryQuery = "";
                                            try
                                            {
                                                // Get the query string.
                                                NameValueCollection queryString = request.QueryString;

                                                // Delete the file file if requested.
                                                if (queryString != null)
                                                {
                                                    // If the delete file query exists
                                                    if (!String.IsNullOrEmpty(queryString["deletefile"]))
                                                    {
                                                        // Get the file to delete path.
                                                        string fileNameToDelete = uploadedFilesListSavePath.TrimEnd('\\') + "\\" + queryString["deletefile"].Replace("/", "\\");

                                                        // If the file exists then delete the file.
                                                        if (System.IO.File.Exists(fileNameToDelete))
                                                            System.IO.File.Delete(fileNameToDelete);
                                                    }

                                                    // If the delete directory query exists
                                                    if (!String.IsNullOrEmpty(queryString["deletedirectory"]))
                                                    {
                                                        // Get the directory to delete path.
                                                        string directoryToDelete = uploadedFilesListSavePath.TrimEnd('\\') + "\\" + queryString["deletedirectory"].Replace("/", "\\").TrimStart('\\') + "\\";

                                                        // If the directory exists then delete the directory.
                                                        if (System.IO.Directory.Exists(directoryToDelete))
                                                            System.IO.Directory.Delete(directoryToDelete, true);
                                                    }

                                                    // If the directory query exists.
                                                    if (!String.IsNullOrEmpty(queryString["directory"]))
                                                    {
                                                        // Set the directory query string.
                                                        directoryQuery = queryString["directory"];
                                                    }
                                                }
                                            }
                                            catch (Exception delupfex)
                                            {
                                                // Log the error.
                                                LogHandler.WriteTypeMessage(
                                                    delupfex.Message,
                                                    MethodInfo.GetCurrentMethod(),
                                                    Nequeo.Net.Common.Helper.EventApplicationName);
                                            }

                                            // Run the uploaded files list Run-Time Text Templating File Preprocessor
                                            // and write the resulting text to the file (uploadfilelist.htm).
                                            string preUploadedFilesList = Nequeo.Net.Http.Common.HttpResponseContent.GetUploadedFileListHtmlEx(fileName, uploadedFilesListSavePath.TrimEnd('\\') + "\\", directoryQuery);
                                            System.IO.File.WriteAllText(urlFilePath, preUploadedFilesList);
                                        }
                                    }

                                    // Construct a response string.
                                    byte[] buffer = System.IO.File.ReadAllBytes(urlFilePath);
                                    string extensionBase = ActiveProcessing.GetMimeContentType(_contextMimeType, extension);

                                    // Get the response OutputStream and write the response to it.
                                    response.ContentLength64 = buffer.Length;
                                    response.ContentType = extensionBase;
                                    response.AddHeader("Allow", "POST, PUT, GET, HEAD");
                                    response.AddHeader("Content-Language", "en-au");
                                    response.AddHeader("Server", "Nequeo/2011.26 (Windows)");
                                    response.AddHeader("WWW-Authenticate", (String.IsNullOrEmpty(authMode) ? "none" : authMode.ToLower()));

                                    // Closes the connection 'response.OutputStream' becomes null
                                    // and no data is sent to the client at all. This should only 
                                    // be used to abort a connection if the client IP is not allowed.
                                    //response.AddHeader("Connection", "close");

                                    // Get the current response output stream
                                    // and write the response to the client.
                                    output = response.OutputStream;
                                    output.Write(buffer, 0, buffer.Length);

                                    // Properly flush and close the output stream
                                    output.Flush();
                                    output.Close();
                                }
                            }
                            else
                            {
                                // Construct a response string.
                                byte[] buffer = System.IO.File.ReadAllBytes(uploadFilePath);

                                // Get the response OutputStream and write the response to it.
                                response.ContentLength64 = buffer.Length;
                                response.ContentType = "application/" + extension;
                                response.AddHeader("content-disposition", "attachment; filename=\"" + fileName + "\"");
                                response.AddHeader("Content-Language", "en-au");
                                response.AddHeader("Server", "Nequeo/2011.26 (Windows)");
                                response.AddHeader("WWW-Authenticate", (String.IsNullOrEmpty(authMode) ? "none" : authMode.ToLower()));

                                // Get the current response output stream
                                // and write the response to the client.
                                output = response.OutputStream;
                                output.Write(buffer, 0, buffer.Length);

                                // Properly flush and close the output stream
                                output.Flush();
                                output.Close();
                            }
                        }
                        else
                        {
                            // Construct a minimal response string.
                            string responseString = Nequeo.Net.Http.Common.HttpPageContent.Html404();
                            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                            // Get the response OutputStream and write the response to it.
                            response.ContentLength64 = buffer.Length;
                            response.ContentType = "text/html; charset=utf-8";
                            response.StatusCode = (int)HttpStatusCode.NotFound;
                            response.AddHeader("Allow", "POST, PUT, GET, HEAD");
                            response.AddHeader("Content-Language", "en-au");
                            response.AddHeader("Server", "Nequeo/2011.26 (Windows)");
                            response.AddHeader("WWW-Authenticate", (String.IsNullOrEmpty(authMode) ? "none" : authMode.ToLower()));

                            // Get the current response output stream
                            // and write the response to the client.
                            output = response.OutputStream;
                            output.Write(buffer, 0, buffer.Length);

                            // Properly flush and close the output stream
                            output.Flush();
                            output.Close();
                        }
                    }
                }
                else
                    throw new Exception("No http context: HttpListenerContext");
            }
            catch (Exception ex)
            {
                try
                {
                    if (response != null)
                    {
                        // Construct a minimal response string.
                        string responseString = Nequeo.Net.Http.Common.HttpPageContent.Html500();
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                        // Get the response OutputStream and write the response to it.
                        response.ContentLength64 = buffer.Length;
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        response.ContentType = "text/html; charset=utf-8";
                        response.AddHeader("Content-Language", "en-au");
                        response.AddHeader("Server", "Nequeo/2011.26 (Windows)");

                        // If the response stream has already been activated.
                        if (output == null)
                        {
                            // Get the current response output stream
                            // and write the response to the client.
                            output = response.OutputStream;
                            output.Write(buffer, 0, buffer.Length);
                        }
                        else
                            output.Write(buffer, 0, buffer.Length);

                        // Properly flush and close the output stream
                        output.Flush();
                        output.Close();
                    }
                }
                catch (Exception iex)
                {
                    // Log the error.
                    LogHandler.WriteTypeMessage(
                        iex.Message,
                        MethodInfo.GetCurrentMethod(),
                        Nequeo.Net.Common.Helper.EventApplicationName);
                }

                // Log the error.
                LogHandler.WriteTypeMessage(
                    ex.Message,
                    MethodInfo.GetCurrentMethod(),
                    Nequeo.Net.Common.Helper.EventApplicationName);
            }
            finally
            {
                try
                {
                    if (output != null)
                        output.Close();
                }
                catch (Exception ex)
                {
                    // Log the error.
                    LogHandler.WriteTypeMessage(
                        ex.Message,
                        MethodInfo.GetCurrentMethod(),
                        Nequeo.Net.Common.Helper.EventApplicationName);
                }

                try
                {
                    if (input != null)
                        input.Close();
                }
                catch (Exception ex)
                {
                    // Log the error.
                    LogHandler.WriteTypeMessage(
                        ex.Message,
                        MethodInfo.GetCurrentMethod(),
                        Nequeo.Net.Common.Helper.EventApplicationName);
                }

                try
                {
                    if (localDestination != null)
                        localDestination.Close();
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
        }
    }
}
