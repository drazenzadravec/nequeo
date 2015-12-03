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
using System.Linq;
using System.IO;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;
using System.Reflection;
using System.Management;
using Microsoft.Win32;

using Nequeo.Handler;
using Nequeo.Extension;
using Nequeo.Server;
using Nequeo.Net;
using Nequeo.Model;

namespace Nequeo.Net.Download
{
    /// <summary>
    /// File download manager server.
    /// </summary>
    [Logging(Handler.WriteTo.EventLog, Handler.LogType.Error)]
    public sealed partial class ManagerServer : Nequeo.Net.WebServer
    {
        #region Constructors
        /// <summary>
        /// File download manager server.
        /// </summary>
        /// <param name="downloadBasePath">The base download path.</param>
        public ManagerServer(string downloadBasePath)
        {
            Common.Helper.DownloadFileBasePath = downloadBasePath;
            AssignOnConnectedActionHandler();
        }

        /// <summary>
        /// File download manager server.
        /// </summary>
        /// <param name="downloadBasePath">The base download path.</param>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public ManagerServer(string downloadBasePath, Net.Sockets.MultiEndpointModel[] multiEndpointModels, int maxNumClients = Int32.MaxValue)
            : base(multiEndpointModels, maxNumClients)
        {
            Common.Helper.DownloadFileBasePath = downloadBasePath;
            AssignOnConnectedActionHandler();
        }
        #endregion

        #region Private Fields
        private Nequeo.Data.Provider.IToken _token = null;
        private Nequeo.Net.IStateContextManager _state = null;
        private Nequeo.Net.IStateContextManager _permission = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the token provider.
        /// </summary>
        public Nequeo.Data.Provider.IToken TokenProvider
        {
            get { return _token; }
            set { _token = value; }
        }
        #endregion

        #region Protected Properties
        /// <summary>
        /// Remove the member from the context manager.
        /// </summary>
        /// <param name="context">The member context to remove.</param>
        protected override void RemoveMember(Sockets.IServerContext context)
        {
            base.RemoveMember(context);

            try
            {
                // Remove the client from the
                // permission state as each client
                // disconnects.
                _permission.Remove(context.UniqueIdentifier);
            }
            catch { }

            try
            {
                // Remove the client from the
                // state state as each client
                // disconnects.
                _state.Remove(context.UniqueIdentifier);
            }
            catch { }
        }
        #endregion

        #region Private Base server initialiser members
        /// <summary>
        /// Assign the on connected action handler.
        /// </summary>
        private void AssignOnConnectedActionHandler()
        {
            // Create a new state context manager.
            _state = new Nequeo.Server.StateContextManager();
            _permission = new Nequeo.Server.StateContextManager();

            // Assign the on connect action handler.
            base.Timeout = 60;
            base.HeaderTimeout = 30000;
            base.RequestTimeout = 30000;
            base.ResponseTimeout = 30000;
            base.ReadBufferSize = 32768;
            base.WriteBufferSize = 32768;
            base.ResponseBufferCapacity = 10000000;
            base.RequestBufferCapacity = 10000000;
            base.MaximumReadLength = 1000000;
            base.Name = "Nequeo Download Manager Server";
            base.ServiceName = "DownloadManagerServer";
            base.SocketProviderHostPrefix = "DownloadManagerServer_";
            base.OnWebContext += ManagerServer_OnWebContext;
        }

        /// <summary>
        /// On web context receive handler.
        /// </summary>
        /// <param name="sender">The application sender.</param>
        /// <param name="context">The current web context.</param>
        private void ManagerServer_OnWebContext(object sender, Net.WebContext context)
        {
            Net.WebRequest request = null;
            Net.WebResponse response = null;
            bool keepAlive = true;
            bool isServerError = false;
            bool isError = true;
            
            try
            {
                string resource = "";
                string executionMember = "";
                request = context.WebRequest;
                response = context.WebResponse;

                // Get the request headers, and set the request headers.
                List<NameValue> headers = base.ParseHeaders(request.Input, out resource, base.HeaderTimeout, base.MaximumReadLength);
                if (headers != null)
                {
                    // Set the request headers.
                    request.ReadWebRequestHeaders(headers, resource);

                    // Should the connection be kept alive.
                    keepAlive = request.KeepAlive;

                    // Get the execution member.
                    if (!String.IsNullOrEmpty(request.Headers["Member"]))
                    {
                        // Get the execution member.
                        // Set the calling member.
                        executionMember = request.Headers["Member"].Trim();
                        response.AddHeader("Member", executionMember);
                        response.AddHeader("ActionName", request.Headers["ActionName"]);

                        // Select the execution member.
                        isError = ExecutionMember(context, executionMember);
                    }
                    else
                    {
                        // Look for a query.
                        if (request.Query != null)
                        {
                            executionMember = request.Query["Member"];
                            string actionName = request.Query["ActionName"];

                            // Set the headers.
                            response.AddHeader("Member", executionMember);
                            response.AddHeader("ActionName", actionName);

                            // Select the execution member.
                            isError = Validate(context, executionMember, true);
                        }
                        else
                        {
                            // Do not allow access.
                            keepAlive = false;
                            throw new Exception("No query has been found.");
                        }
                    }
                }
                else
                {
                    // No headers have been found.
                    keepAlive = false;
                    throw new Exception("No headers have been found.");
                }

                // If error has occured.
                if (isError)
                {
                    // Send an error response.
                    response.KeepAlive = keepAlive;
                    response.StatusCode = 400;
                    response.StatusDescription = "Bad Request";
                    response.ContentLength = 0;
                    response.WriteWebResponseHeaders();
                }
            }
            catch (Exception) { isServerError = true; }

            // If a server error has occured.
            if (isServerError)
            {
                // Make sure the response exists.
                if (response != null)
                {
                    try
                    {
                        // Send an error response.
                        response.KeepAlive = keepAlive;
                        response.StatusCode = 500;
                        response.StatusDescription = "Internal server error";
                        response.ContentLength = 0;
                        response.WriteWebResponseHeaders();
                    }
                    catch { keepAlive = false; }
                }
            }

            // If do not keep alive.
            if (!keepAlive)
            {
                // Close the connection.
                if (response != null)
                {
                    try
                    {
                        // Close the connection.
                        response.Close();
                    }
                    catch { }
                }
            }
        }
        #endregion

        #region Private Execution Members
        /// <summary>
        /// Select the execution member.
        /// </summary>
        /// <param name="context">The web context.</param>
        /// <param name="executionMember">The execution member.</param>
        /// <param name="isQueryRequest">Is the request from a query string.</param>
        /// <returns>True if error; else false.</returns>
        private bool ExecutionMember(Net.WebContext context, string executionMember, bool isQueryRequest = false)
        {
            bool isError = false;

            // Select the member to execute.
            switch (executionMember.ToUpper())
            {
                case "VALIDATE":
                    // Validate.
                    isError = Validate(context, executionMember, isQueryRequest);
                    break;

                case "DOWNLOADFILE":
                    // Download the file.
                    if (!isQueryRequest)
                        isError = DownloadFile(context);
                    else
                        DownloadFileQuery(context);
                    break;

                case "UPLOADFILE":
                    // Upload the file.
                    if (!isQueryRequest)
                        isError = UploadFile(context);
                    else
                        UploadFileQuery(context);
                    break;

                case "FILELIST":
                    // File directory list.
                    if (!isQueryRequest)
                        isError = FileList(context);
                    else
                        FileListQuery(context);
                    break;

                case "STOPDOWNLOADINGFILE":
                    // Stop downloading file.
                    isError = false;
                    break;

                case "DELETEFILE":
                    // Delete file.
                    if (!isQueryRequest)
                        isError = DeleteFile(context);
                    else
                        DeleteFileQuery(context);
                    break;

                case "RENAMEFILE":
                    // Rename file.
                    if (!isQueryRequest)
                        isError = RenameFile(context);
                    else
                        RenameFileQuery(context);
                    break;

                case "COPYFILE":
                    // Copy file.
                    if (!isQueryRequest)
                        isError = CopyFile(context);
                    else
                        CopyFileQuery(context);
                    break;

                case "MOVEFILE":
                    // Move file.
                    if (!isQueryRequest)
                        isError = MoveFile(context);
                    else
                        MoveFileQuery(context);
                    break;

                case "FILEDETAILS":
                    // File details.
                    if (!isQueryRequest)
                        isError = FileDetails(context);
                    else
                        FileDetailsQuery(context);
                    break;

                case "DELETEDIRECTORY":
                    // Delete directory.
                    if (!isQueryRequest)
                        isError = DeleteDirectory(context);
                    else
                        DeleteDirectoryQuery(context);
                    break;

                case "CREATEDIRECTORY":
                    // Create directory.
                    if (!isQueryRequest)
                        isError = CreateDirectory(context);
                    else
                        CreateDirectoryQuery(context);
                    break;

                default:
                    throw new Exception("Command not recognised.");
            }

            // Return error state.
            return isError;
        }
        #endregion

        #region Private Action Members
        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="context">The web context.</param>
        /// <param name="executionMember">The execution member.</param>
        /// <param name="isQueryRequest">Is the request from a query string.</param>
        /// <returns>True if error; else false.</returns>
        private bool Validate(Net.WebContext context, string executionMember, bool isQueryRequest = false)
        {
            bool isError = false;

            // If not authenticated.
            if (!context.IsAuthenticated)
            {
                // Create a new uid.
                context.UniqueIdentifier = Guid.NewGuid().ToString();

                // Get the user unique id and the current token issued.
                string uniqueIdentifier = "";
                string token = "";

                // If not a query request.
                if (!isQueryRequest)
                {
                    // Get the user unique id and the current token issued.
                    uniqueIdentifier = context.WebRequest.Headers["UniqueIdentifier"];
                    token = context.WebRequest.Headers["Token"];
                }
                else
                {
                    // Get the user unique id and the current token issued.
                    uniqueIdentifier = context.WebRequest.Query["UniqueIdentifier"];
                    token = context.WebRequest.Query["Token"];
                }

                // Is token valid.
                _token.IsValid(uniqueIdentifier, base.ServiceName, token, (result, permission, state) =>
                {
                    Net.WebContext currState = null;

                    try
                    {
                        // Get the current state.
                        currState = (Net.WebContext)state;

                        // If the token is valid.
                        if (result)
                        {
                            // Set the uid.
                            currState.UniqueIdentifier = uniqueIdentifier;

                            // If the permission does not exist.
                            if (_permission.Find(uniqueIdentifier) == null)
                                _permission.Add(uniqueIdentifier, permission);

                            // If not a query request.
                            if (!isQueryRequest)
                            {
                                // The client has been authenticated.
                                currState.IsAuthenticated = true;

                                // Send valid token response.
                                currState.WebResponse.KeepAlive = true;
                                currState.WebResponse.AddHeader("MemberResult", true.ToString());
                                CreateOkHeaders(currState.WebResponse);
                            }
                            else
                            {
                                // Select the execution member query.
                                ExecutionMember(currState, executionMember, true);
                            }
                        }
                        else
                        {
                            // Send an error response.
                            currState.WebResponse.KeepAlive = false;
                            currState.WebResponse.AddHeader("MemberResult", false.ToString());
                            CreateBadRequestHeaders(currState.WebResponse);
                        }
                    }
                    catch
                    {
                        try
                        {
                            // Send an error response.
                            currState.WebResponse.KeepAlive = false;
                            currState.WebResponse.AddHeader("MemberResult", false.ToString());
                            CreateInternalServerErrorHeaders(currState.WebResponse);
                        }
                        catch { }
                    }

                }, uniqueIdentifier, context);
            }
            else
            {
                // If not a query request.
                if (!isQueryRequest)
                {
                    // Send valid token response.
                    context.WebResponse.KeepAlive = true;
                    context.WebResponse.AddHeader("MemberResult", true.ToString());
                    CreateOkHeaders(context.WebResponse);
                }
                else
                {
                    // Select the execution member query.
                    ExecutionMember(context, executionMember, true);
                }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// DownloadFile
        /// </summary>
        /// <param name="context">The web context.</param>
        /// <returns>True if error; else false.</returns>
        private bool DownloadFile(Net.WebContext context)
        {
            bool isError = false;

            // If not authenticated.
            if (!context.IsAuthenticated)
            {
                // Send an error response.
                context.WebResponse.KeepAlive = false;
                context.WebResponse.AddHeader("MemberResult", false.ToString());
                CreateBadRequestHeaders(context.WebResponse);
            }
            else
            {
                // Authenticated.
                FileStream requestStream = null;
                DownloadState stateData = null;

                try
                {
                    // Attempt to find the permission.
                    Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                    // If download permission is denied.
                    if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Download))
                        throw new Nequeo.Exceptions.PermissionException("Permission denied");

                    // Get the file location.
                    string fileNameQuery = context.WebRequest.Headers["FileName"];
                    string fileSubDirectoryQuery = context.WebRequest.Headers["Directory"];
                    string fileNamePath = Common.Helper.GetDownloadFile(fileNameQuery, fileSubDirectoryQuery);
                    string fileName = System.IO.Path.GetFileName(fileNamePath);
                    string extension = System.IO.Path.GetExtension(fileNamePath);
                    FileInfo fileInfo = new FileInfo(fileNamePath);

                    // Get the file read position.
                    long fileReadPosition = 0;
                    long fileReadSize = 0;
                    long contentLength = fileInfo.Length;

                    // Get the read position of the file.
                    if (context.WebRequest.Headers["FileReadPosition"] != null)
                        fileReadPosition = Int64.Parse(context.WebRequest.Headers["FileReadPosition"]);

                    // Get the number of bytes to read.
                    if (context.WebRequest.Headers["FileReadSize"] != null)
                        fileReadSize = Int64.Parse(context.WebRequest.Headers["FileReadSize"]);

                    // If the file read position is too large.
                    if (fileReadPosition > fileInfo.Length)
                        // Throw position too large.
                        throw new Nequeo.Exceptions.InvalidLengthException("File read position invalid.");
                    else
                    {
                        // Open and read the file.
                        requestStream = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                        // Set the position to start reading from.
                        requestStream.Seek(fileReadPosition, SeekOrigin.Begin);
                    }

                    // If read size has been set.
                    if (fileReadPosition > 0 && fileReadSize > 0)
                    {
                        // Get the number of bytes left.
                        long left = fileInfo.Length - fileReadPosition;

                        // If the read size is less or same
                        // as what is left to download.
                        if (fileReadSize <= left)
                            contentLength = fileReadSize;
                        else
                            contentLength = left;
                    }
                    else if (fileReadPosition > 0)
                    {
                        // Get the difference.
                        contentLength = fileInfo.Length - fileReadPosition;
                    }
                    else if (fileReadSize > 0)
                    {
                        // If the read size is less or same
                        // as the file length.
                        if (fileReadSize <= fileInfo.Length)
                            contentLength = fileReadSize;
                    }

                    // Attempt to find the current state data.
                    stateData = new DownloadState() 
                    { 
                        CancellationToken = new CancellationTokenSource(), 
                        IsDownloadComplete = false,
                        File = requestStream,
                        ContentLength = contentLength
                    };

                    // Start the async mode.
                    context.IsAsyncMode = true;

                    // Listen for a cancellation request from the client.
                    ProcessDownloadCancellation(context, stateData);

                    // Start the download.
                    ProcessDownload(context, stateData);
                }
                catch (Nequeo.Exceptions.InvalidPathException)
                {
                    try
                    {
                        // Send an error response.
                        context.WebResponse.KeepAlive = false;
                        context.WebResponse.AddHeader("MemberResult", false.ToString());
                        CreateInternalServerErrorHeaders(context.WebResponse);
                    }
                    catch { }
                }
                catch (Nequeo.Exceptions.InvalidLengthException)
                {
                    try
                    {
                        // Send an error response.
                        context.WebResponse.KeepAlive = false;
                        context.WebResponse.AddHeader("MemberResult", false.ToString());
                        CreateInternalServerErrorHeaders(context.WebResponse);
                    }
                    catch { }
                }
                catch (Nequeo.Exceptions.PermissionException)
                {
                    try
                    {
                        // Send an error response.
                        context.WebResponse.KeepAlive = false;
                        context.WebResponse.AddHeader("MemberResult", false.ToString());
                        CreateInternalServerErrorHeaders(context.WebResponse);
                    }
                    catch { }
                }
                catch { }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// UploadFile
        /// </summary>
        /// <param name="context">The web context.</param>
        /// <returns>True if error; else false.</returns>
        private bool UploadFile(Net.WebContext context)
        {
            bool isError = false;

            // If not authenticated.
            if (!context.IsAuthenticated)
            {
                // Send an error response.
                context.WebResponse.KeepAlive = false;
                context.WebResponse.AddHeader("MemberResult", false.ToString());
                CreateBadRequestHeaders(context.WebResponse);
            }
            else
            {
                // Authenticated.
                bool containsUploadData = false;

                // If context type exits.
                if (!String.IsNullOrEmpty(context.WebRequest.ContentType))
                {
                    // If upload data exists.
                    if(context.WebRequest.ContentType.ToLower().Contains("multipart/form-data"))
                        containsUploadData = true;
                }

                // Upload form data exists.
                if (containsUploadData)
                {
                    System.IO.BinaryReader reader = null;
                    try
                    {
                        // Attempt to find the permission.
                        Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                        // If upload permission is denied.
                        if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Upload))
                            throw new Nequeo.Exceptions.PermissionException("Permission denied");

                        // Create the reader.
                        reader = new System.IO.BinaryReader(context.WebRequest.Input, Encoding.Default, true);

                        // Return the result.
                        Http.Common.HttpFormModel fileData = Nequeo.Net.Http.Utility.ParseForm(reader, base.RequestTimeout);

                        // If files exist.
                        if (fileData != null && fileData.UploadedFiles != null && fileData.UploadedFiles.Count > 0)
                        {
                            // For each file uploaded.
                            foreach (Http.Common.HttpUploadFileModel fileModel in fileData.UploadedFiles)
                            {
                                bool copied = false;
                                string fileNamePath = "";
                                FileStream requestStream = null;

                                try
                                {
                                    // If this file contains data.
                                    if (fileModel.UploadFile != null && fileModel.UploadFile.Length > 0)
                                    {
                                        // Get the file path.
                                        fileNamePath = Common.Helper.GetUploadFile(fileModel.FileName, context.UniqueIdentifier);

                                        // Open and write to the file.
                                        requestStream = new FileStream(fileNamePath, FileMode.Create, FileAccess.Write, FileShare.Write);
                                        fileModel.Save(requestStream);
                                        fileModel.UploadFile.Dispose();
                                        requestStream.Close();
                                        copied = true;
                                    }
                                }
                                catch { }
                                finally
                                {
                                    // Dispose of the buffer.
                                    if (requestStream != null)
                                        requestStream.Dispose();

                                    // If all data was not copied.
                                    if (!copied)
                                    {
                                        try
                                        {
                                            if (File.Exists(fileNamePath))
                                                File.Delete(fileNamePath);
                                        }
                                        catch { }
                                    }
                                }
                            }

                            // Send the result.
                            context.WebResponse.ContentLength = 0;
                            context.WebResponse.ContentLanguage = "en-au";
                            context.WebResponse.Server = base.Name;
                            context.WebResponse.AddHeader("MemberResult", true.ToString());
                            context.WebResponse.WriteWebResponseHeaders();
                        }
                        else
                            throw new Exception();
                    }
                    catch (Exception)
                    {
                        try
                        {
                            // Send an error response.
                            context.WebResponse.KeepAlive = false;
                            context.WebResponse.AddHeader("MemberResult", false.ToString());
                            CreateInternalServerErrorHeaders(context.WebResponse);
                        }
                        catch { }
                    }
                    finally
                    {
                        if (reader != null)
                            reader.Dispose();
                    }
                }
                else
                {
                    bool copied = false;
                    string fileNamePath = "";
                    string fileNamePathCopied = "";
                    FileStream requestStream = null;

                    try
                    {
                        // Attempt to find the permission.
                        Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                        // If upload permission is denied.
                        if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Upload))
                            throw new Nequeo.Exceptions.PermissionException("Permission denied");

                        // Get the user unique id.
                        string fileNameQuery = context.WebRequest.Headers["FileName"];
                        string fileSubDirectoryQuery = context.WebRequest.Headers["Directory"];

                        fileNamePathCopied = Common.Helper.GetUploadFile(fileNameQuery, fileSubDirectoryQuery);
                        fileNamePath = fileNamePathCopied + ".partial";

                        // Get the file write position.
                        long fileWritePosition = 0;

                        // Get the write position of the file.
                        if (context.WebRequest.Headers["FileWritePosition"] != null)
                            fileWritePosition = Int64.Parse(context.WebRequest.Headers["FileWritePosition"]);

                        // If appending to the file.
                        if(fileWritePosition > 0)
                            // Append to the file.
                            requestStream = new FileStream(fileNamePath, FileMode.Append, FileAccess.Write, FileShare.Write);
                        else
                            // Open and write to the file.
                            requestStream = new FileStream(fileNamePath, FileMode.Create, FileAccess.Write, FileShare.Write);

                        // Send the file to the stream.
                        copied = Nequeo.IO.Stream.Operation.CopyStream(context.WebRequest.Input, requestStream, context.WebRequest.ContentLength, base.RequestTimeout, base.ReadBufferSize);
                        requestStream.Close();

                        // Send the result.
                        context.WebResponse.ContentLength = 0;
                        context.WebResponse.ContentLanguage = "en-au";
                        context.WebResponse.Server = base.Name;
                        context.WebResponse.AddHeader("MemberResult", true.ToString());
                        context.WebResponse.WriteWebResponseHeaders();
                    }
                    catch (Nequeo.Exceptions.InvalidPathException)
                    {
                        try
                        {
                            // Send an error response.
                            context.WebResponse.KeepAlive = false;
                            context.WebResponse.AddHeader("MemberResult", false.ToString());
                            CreateInternalServerErrorHeaders(context.WebResponse);
                        }
                        catch { }
                    }
                    catch (Nequeo.Exceptions.PermissionException)
                    {
                        try
                        {
                            // Send an error response.
                            context.WebResponse.KeepAlive = false;
                            context.WebResponse.AddHeader("MemberResult", false.ToString());
                            CreateInternalServerErrorHeaders(context.WebResponse);
                        }
                        catch { }
                    }
                    catch { }
                    finally
                    {
                        // Dispose of the buffer.
                        if (requestStream != null)
                            requestStream.Dispose();

                        // If all data was copied.
                        if (copied)
                        {
                            try
                            {
                                if (File.Exists(fileNamePath))
                                    File.Move(fileNamePath, fileNamePathCopied);
                            }
                            catch { }
                        }
                    }
                }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// FileList
        /// </summary>
        /// <param name="context">The web context.</param>
        /// <returns>True if error; else false.</returns>
        private bool FileList(Net.WebContext context)
        {
            bool isError = false;

            // If not authenticated.
            if (!context.IsAuthenticated)
            {
                // Send an error response.
                context.WebResponse.KeepAlive = false;
                context.WebResponse.AddHeader("MemberResult", false.ToString());
                CreateBadRequestHeaders(context.WebResponse);
            }
            else
            {
                try
                {
                    // Attempt to find the permission.
                    Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                    // If download permission is denied.
                    if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.List))
                        throw new Nequeo.Exceptions.PermissionException("Permission denied");

                    // Get the file location.
                    string fileSubDirectoryQuery = context.WebRequest.Headers["Directory"];
                    string[] paths = Common.Helper.GetFileList(fileSubDirectoryQuery);

                    // Convert the paths to byte array.
                    byte[] buffer = String.Join("\r\n", paths).ToByteArray();

                    // Send the result.
                    context.WebResponse.ContentLength = buffer.Length;
                    context.WebResponse.ContentLanguage = "en-au";
                    context.WebResponse.Server = base.Name;
                    context.WebResponse.AddHeader("MemberResult", true.ToString());
                    context.WebResponse.WriteWebResponseHeaders();

                    // Write the data to the stream.
                    context.WebResponse.Write(buffer);
                }
                catch (Nequeo.Exceptions.InvalidPathException)
                {
                    try
                    {
                        // Send an error response.
                        context.WebResponse.KeepAlive = false;
                        context.WebResponse.AddHeader("MemberResult", false.ToString());
                        CreateInternalServerErrorHeaders(context.WebResponse);
                    }
                    catch { }
                }
                catch (Nequeo.Exceptions.PermissionException)
                {
                    try
                    {
                        // Send an error response.
                        context.WebResponse.KeepAlive = false;
                        context.WebResponse.AddHeader("MemberResult", false.ToString());
                        CreateInternalServerErrorHeaders(context.WebResponse);
                    }
                    catch { }
                }
                catch { }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// DeleteFile
        /// </summary>
        /// <param name="context">The web context.</param>
        /// <returns>True if error; else false.</returns>
        private bool DeleteFile(Net.WebContext context)
        {
            bool isError = false;

            // If not authenticated.
            if (!context.IsAuthenticated)
            {
                // Send an error response.
                context.WebResponse.KeepAlive = false;
                context.WebResponse.AddHeader("MemberResult", false.ToString());
                CreateBadRequestHeaders(context.WebResponse);
            }
            else
            {
                try
                {
                    // Attempt to find the permission.
                    Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                    // If upload permission is denied.
                    if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Delete))
                        throw new Nequeo.Exceptions.PermissionException("Permission denied");

                    // Get the user unique id.
                    string fileNameQuery = context.WebRequest.Headers["FileName"];
                    string fileSubDirectoryQuery = context.WebRequest.Headers["Directory"];
                    string fileNamePath = Common.Helper.GetFilePath(fileNameQuery, fileSubDirectoryQuery);

                    // If the file exists then delete the file.
                    if (File.Exists(fileNamePath))
                        File.Delete(fileNamePath);

                    // Send the result.
                    context.WebResponse.ContentLength = 0;
                    context.WebResponse.ContentLanguage = "en-au";
                    context.WebResponse.Server = base.Name;
                    context.WebResponse.AddHeader("MemberResult", true.ToString());
                    context.WebResponse.WriteWebResponseHeaders();
                }
                catch (Nequeo.Exceptions.InvalidPathException)
                {
                    try
                    {
                        // Send an error response.
                        context.WebResponse.KeepAlive = false;
                        context.WebResponse.AddHeader("MemberResult", false.ToString());
                        CreateInternalServerErrorHeaders(context.WebResponse);
                    }
                    catch { }
                }
                catch (Nequeo.Exceptions.PermissionException)
                {
                    try
                    {
                        // Send an error response.
                        context.WebResponse.KeepAlive = false;
                        context.WebResponse.AddHeader("MemberResult", false.ToString());
                        CreateInternalServerErrorHeaders(context.WebResponse);
                    }
                    catch { }
                }
                catch { }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// RenameFile
        /// </summary>
        /// <param name="context">The web context.</param>
        /// <returns>True if error; else false.</returns>
        private bool RenameFile(Net.WebContext context)
        {
            bool isError = false;

            // If not authenticated.
            if (!context.IsAuthenticated)
            {
                // Send an error response.
                context.WebResponse.KeepAlive = false;
                context.WebResponse.AddHeader("MemberResult", false.ToString());
                CreateBadRequestHeaders(context.WebResponse);
            }
            else
            {
                try
                {
                    // Attempt to find the permission.
                    Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                    // If upload permission is denied.
                    if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Rename))
                        throw new Nequeo.Exceptions.PermissionException("Permission denied");

                    // Get the user unique id.
                    string fileNameCuurentQuery = context.WebRequest.Headers["CurrentFileName"];
                    string fileNameNewQuery = context.WebRequest.Headers["NewFileName"];
                    string fileSubDirectoryQuery = context.WebRequest.Headers["Directory"];
                    string fileNameCurrrentPath = Common.Helper.GetFilePath(fileNameCuurentQuery, fileSubDirectoryQuery);
                    string fileNameNewPath = Common.Helper.GetFilePath(fileNameNewQuery, fileSubDirectoryQuery);

                    // If the file exists then remane the file.
                    if (File.Exists(fileNameCurrrentPath))
                        File.Move(fileNameCurrrentPath, fileNameNewPath);

                    // Send the result.
                    context.WebResponse.ContentLength = 0;
                    context.WebResponse.ContentLanguage = "en-au";
                    context.WebResponse.Server = base.Name;
                    context.WebResponse.AddHeader("MemberResult", true.ToString());
                    context.WebResponse.WriteWebResponseHeaders();
                }
                catch (Nequeo.Exceptions.InvalidPathException)
                {
                    try
                    {
                        // Send an error response.
                        context.WebResponse.KeepAlive = false;
                        context.WebResponse.AddHeader("MemberResult", false.ToString());
                        CreateInternalServerErrorHeaders(context.WebResponse);
                    }
                    catch { }
                }
                catch (Nequeo.Exceptions.PermissionException)
                {
                    try
                    {
                        // Send an error response.
                        context.WebResponse.KeepAlive = false;
                        context.WebResponse.AddHeader("MemberResult", false.ToString());
                        CreateInternalServerErrorHeaders(context.WebResponse);
                    }
                    catch { }
                }
                catch { }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// CopyFile
        /// </summary>
        /// <param name="context">The web context.</param>
        /// <returns>True if error; else false.</returns>
        private bool CopyFile(Net.WebContext context)
        {
            bool isError = false;

            // If not authenticated.
            if (!context.IsAuthenticated)
            {
                // Send an error response.
                context.WebResponse.KeepAlive = false;
                context.WebResponse.AddHeader("MemberResult", false.ToString());
                CreateBadRequestHeaders(context.WebResponse);
            }
            else
            {
                try
                {
                    // Attempt to find the permission.
                    Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                    // If upload permission is denied.
                    if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Copy))
                        throw new Nequeo.Exceptions.PermissionException("Permission denied");

                    // Get the user unique id.
                    string sourcePathFileNameQuery = context.WebRequest.Headers["SourcePathFileName"];
                    string destinationPathFileNameQuery = context.WebRequest.Headers["DestinationPathFileName"];
                    string sourcePathFileName = Common.Helper.GetFilePath(sourcePathFileNameQuery);
                    string destinationPathFileName = Common.Helper.GetFilePath(destinationPathFileNameQuery);

                    // If the file exists then remane the file.
                    if (File.Exists(sourcePathFileNameQuery))
                        File.Copy(sourcePathFileNameQuery, destinationPathFileName, true);

                    // Send the result.
                    context.WebResponse.ContentLength = 0;
                    context.WebResponse.ContentLanguage = "en-au";
                    context.WebResponse.Server = base.Name;
                    context.WebResponse.AddHeader("MemberResult", true.ToString());
                    context.WebResponse.WriteWebResponseHeaders();
                }
                catch (Nequeo.Exceptions.InvalidPathException)
                {
                    try
                    {
                        // Send an error response.
                        context.WebResponse.KeepAlive = false;
                        context.WebResponse.AddHeader("MemberResult", false.ToString());
                        CreateInternalServerErrorHeaders(context.WebResponse);
                    }
                    catch { }
                }
                catch (Nequeo.Exceptions.PermissionException)
                {
                    try
                    {
                        // Send an error response.
                        context.WebResponse.KeepAlive = false;
                        context.WebResponse.AddHeader("MemberResult", false.ToString());
                        CreateInternalServerErrorHeaders(context.WebResponse);
                    }
                    catch { }
                }
                catch { }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// MoveFile
        /// </summary>
        /// <param name="context">The web context.</param>
        /// <returns>True if error; else false.</returns>
        private bool MoveFile(Net.WebContext context)
        {
            bool isError = false;

            // If not authenticated.
            if (!context.IsAuthenticated)
            {
                // Send an error response.
                context.WebResponse.KeepAlive = false;
                context.WebResponse.AddHeader("MemberResult", false.ToString());
                CreateBadRequestHeaders(context.WebResponse);
            }
            else
            {
                try
                {
                    // Attempt to find the permission.
                    Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                    // If upload permission is denied.
                    if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Move))
                        throw new Nequeo.Exceptions.PermissionException("Permission denied");

                    // Get the user unique id.
                    string sourcePathFileNameQuery = context.WebRequest.Headers["SourcePathFileName"];
                    string destinationPathFileNameQuery = context.WebRequest.Headers["DestinationPathFileName"];
                    string sourcePathFileName = Common.Helper.GetFilePath(sourcePathFileNameQuery);
                    string destinationPathFileName = Common.Helper.GetFilePath(destinationPathFileNameQuery);

                    // If the file exists then remane the file.
                    if (File.Exists(sourcePathFileNameQuery))
                        File.Move(sourcePathFileNameQuery, destinationPathFileName);

                    // Send the result.
                    context.WebResponse.ContentLength = 0;
                    context.WebResponse.ContentLanguage = "en-au";
                    context.WebResponse.Server = base.Name;
                    context.WebResponse.AddHeader("MemberResult", true.ToString());
                    context.WebResponse.WriteWebResponseHeaders();
                }
                catch (Nequeo.Exceptions.InvalidPathException)
                {
                    try
                    {
                        // Send an error response.
                        context.WebResponse.KeepAlive = false;
                        context.WebResponse.AddHeader("MemberResult", false.ToString());
                        CreateInternalServerErrorHeaders(context.WebResponse);
                    }
                    catch { }
                }
                catch (Nequeo.Exceptions.PermissionException)
                {
                    try
                    {
                        // Send an error response.
                        context.WebResponse.KeepAlive = false;
                        context.WebResponse.AddHeader("MemberResult", false.ToString());
                        CreateInternalServerErrorHeaders(context.WebResponse);
                    }
                    catch { }
                }
                catch { }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// FileDetails
        /// </summary>
        /// <param name="context">The web context.</param>
        /// <returns>True if error; else false.</returns>
        private bool FileDetails(Net.WebContext context)
        {
            bool isError = false;

            // If not authenticated.
            if (!context.IsAuthenticated)
            {
                // Send an error response.
                context.WebResponse.KeepAlive = false;
                context.WebResponse.AddHeader("MemberResult", false.ToString());
                CreateBadRequestHeaders(context.WebResponse);
            }
            else
            {
                try
                {
                    // Attempt to find the permission.
                    Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                    // If upload permission is denied.
                    if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.List))
                        throw new Nequeo.Exceptions.PermissionException("Permission denied");

                    // Get the user unique id.
                    string fileNameQuery = context.WebRequest.Headers["FileName"];
                    string fileSubDirectoryQuery = context.WebRequest.Headers["Directory"];
                    string fileNamePath = Common.Helper.GetFilePath(fileNameQuery, fileSubDirectoryQuery);
                    string[] details = new string[15];

                    // Get the file information.
                    FileInfo fileInfo = new FileInfo(fileNamePath);
                    details[0] = fileInfo.Attributes.ToString();
                    details[1] = fileInfo.CreationTime.ToString();
                    details[2] = fileInfo.CreationTimeUtc.ToString();
                    details[3] = Common.Helper.GetRelativePath(fileInfo.Directory.FullName);
                    details[4] = Common.Helper.GetRelativePath(fileInfo.DirectoryName);
                    details[5] = fileInfo.Exists.ToString();
                    details[6] = fileInfo.Extension.ToString();
                    details[7] = Common.Helper.GetRelativePath(fileInfo.FullName);
                    details[8] = fileInfo.IsReadOnly.ToString();
                    details[9] = fileInfo.LastAccessTime.ToString();
                    details[10] = fileInfo.LastAccessTimeUtc.ToString();
                    details[11] = fileInfo.LastWriteTime.ToString();
                    details[12] = fileInfo.LastWriteTimeUtc.ToString();
                    details[13] = fileInfo.Length.ToString();
                    details[14] = fileInfo.Name.ToString();

                    // Convert the paths to byte array.
                    byte[] buffer = String.Join("\r\n", details).ToByteArray();

                    // Send the result.
                    context.WebResponse.ContentLength = buffer.Length;
                    context.WebResponse.ContentLanguage = "en-au";
                    context.WebResponse.Server = base.Name;
                    context.WebResponse.AddHeader("MemberResult", true.ToString());
                    context.WebResponse.WriteWebResponseHeaders();

                    // Write the data to the stream.
                    context.WebResponse.Write(buffer);
                }
                catch (Nequeo.Exceptions.InvalidPathException)
                {
                    try
                    {
                        // Send an error response.
                        context.WebResponse.KeepAlive = false;
                        context.WebResponse.AddHeader("MemberResult", false.ToString());
                        CreateInternalServerErrorHeaders(context.WebResponse);
                    }
                    catch { }
                }
                catch (Nequeo.Exceptions.PermissionException)
                {
                    try
                    {
                        // Send an error response.
                        context.WebResponse.KeepAlive = false;
                        context.WebResponse.AddHeader("MemberResult", false.ToString());
                        CreateInternalServerErrorHeaders(context.WebResponse);
                    }
                    catch { }
                }
                catch { }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// DeleteDirectory
        /// </summary>
        /// <param name="context">The web context.</param>
        /// <returns>True if error; else false.</returns>
        private bool DeleteDirectory(Net.WebContext context)
        {
            bool isError = false;

            // If not authenticated.
            if (!context.IsAuthenticated)
            {
                // Send an error response.
                context.WebResponse.KeepAlive = false;
                context.WebResponse.AddHeader("MemberResult", false.ToString());
                CreateBadRequestHeaders(context.WebResponse);
            }
            else
            {
                try
                {
                    // Attempt to find the permission.
                    Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                    // If upload permission is denied.
                    if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Delete))
                        throw new Nequeo.Exceptions.PermissionException("Permission denied");

                    // Get the user unique id.
                    string fileSubDirectoryQuery = context.WebRequest.Headers["Directory"];
                    string directoryPath = Common.Helper.GetDirectoryPath(fileSubDirectoryQuery);

                    // If the directory exists then delete the directory.
                    if (Directory.Exists(directoryPath))
                    {
                        // Do not delete if not empty.
                        Directory.Delete(directoryPath, false);
                    }

                    // Send the result.
                    context.WebResponse.ContentLength = 0;
                    context.WebResponse.ContentLanguage = "en-au";
                    context.WebResponse.Server = base.Name;
                    context.WebResponse.AddHeader("MemberResult", true.ToString());
                    context.WebResponse.WriteWebResponseHeaders();
                }
                catch (Nequeo.Exceptions.InvalidPathException)
                {
                    try
                    {
                        // Send an error response.
                        context.WebResponse.KeepAlive = false;
                        context.WebResponse.AddHeader("MemberResult", false.ToString());
                        CreateInternalServerErrorHeaders(context.WebResponse);
                    }
                    catch { }
                }
                catch (Nequeo.Exceptions.PermissionException)
                {
                    try
                    {
                        // Send an error response.
                        context.WebResponse.KeepAlive = false;
                        context.WebResponse.AddHeader("MemberResult", false.ToString());
                        CreateInternalServerErrorHeaders(context.WebResponse);
                    }
                    catch { }
                }
                catch { }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// CreateDirectory
        /// </summary>
        /// <param name="context">The web context.</param>
        /// <returns>True if error; else false.</returns>
        private bool CreateDirectory(Net.WebContext context)
        {
            bool isError = false;

            // If not authenticated.
            if (!context.IsAuthenticated)
            {
                // Send an error response.
                context.WebResponse.KeepAlive = false;
                context.WebResponse.AddHeader("MemberResult", false.ToString());
                CreateBadRequestHeaders(context.WebResponse);
            }
            else
            {
                try
                {
                    // Attempt to find the permission.
                    Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                    // If upload permission is denied.
                    if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Create))
                        throw new Nequeo.Exceptions.PermissionException("Permission denied");

                    // Get the user unique id.
                    string fileSubDirectoryQuery = context.WebRequest.Headers["Directory"];
                    bool created = Common.Helper.CreateDirectory(fileSubDirectoryQuery);

                    // Send the result.
                    context.WebResponse.ContentLength = 0;
                    context.WebResponse.ContentLanguage = "en-au";
                    context.WebResponse.Server = base.Name;
                    context.WebResponse.AddHeader("MemberResult", created.ToString());
                    context.WebResponse.WriteWebResponseHeaders();
                }
                catch (Nequeo.Exceptions.InvalidPathException)
                {
                    try
                    {
                        // Send an error response.
                        context.WebResponse.KeepAlive = false;
                        context.WebResponse.AddHeader("MemberResult", false.ToString());
                        CreateInternalServerErrorHeaders(context.WebResponse);
                    }
                    catch { }
                }
                catch (Nequeo.Exceptions.PermissionException)
                {
                    try
                    {
                        // Send an error response.
                        context.WebResponse.KeepAlive = false;
                        context.WebResponse.AddHeader("MemberResult", false.ToString());
                        CreateInternalServerErrorHeaders(context.WebResponse);
                    }
                    catch { }
                }
                catch { }
            }

            // Return the result.
            return isError;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Download the file.
        /// </summary>
        /// <param name="context">The web context.</param>
        /// <param name="stateData">The current download state.</param>
        private async void ProcessDownload(Net.WebContext context, DownloadState stateData)
        {
            // Start the task.
            await Nequeo.Threading.AsyncOperationResult<bool>.RunTask(() =>
                {
                    try
                    {
                        // Send the file.
                        context.WebResponse.ContentLength = stateData.ContentLength;
                        context.WebResponse.ContentLanguage = "en-au";
                        context.WebResponse.Server = base.Name;
                        context.WebResponse.AddHeader("MemberResult", true.ToString());
                        context.WebResponse.WriteWebResponseHeaders();

                        // Send the file to the stream.
                        Nequeo.IO.Stream.Operation.CopyStream(stateData.File, context.WebResponse.Output, stateData.CancellationToken.Token,
                            context.WebResponse.ContentLength, base.ResponseTimeout, base.WriteBufferSize);

                        // Download has completed.
                        stateData.IsDownloadComplete = true;

                        // Close the file stream.
                        stateData.File.Close();

                        // If cancellaction has not been set,
                        // then set the cancellation request.
                        if (!stateData.CancellationToken.IsCancellationRequested)
                            stateData.CancellationToken.Cancel();
                    }
                    catch (Exception) 
                    {
                        try
                        {
                            // Send an error response.
                            context.WebResponse.KeepAlive = false;
                            context.WebResponse.AddHeader("MemberResult", false.ToString());
                            CreateInternalServerErrorHeaders(context.WebResponse);
                        }
                        catch { }
                    }
                    finally
                    {
                        // Cancel the async mode.
                        context.IsAsyncMode = false;

                        // Dispose of the buffer.
                        if (stateData.File != null)
                            stateData.File.Dispose();

                        // Cleaup the state.
                        if (stateData != null)
                        {
                            if (stateData.CancellationToken != null)
                                stateData.CancellationToken.Dispose();

                            stateData.File = null;
                            stateData = null;
                        }
                    }
                });
        }

        /// <summary>
        /// Listen for a cancellation request from the client.
        /// </summary>
        /// <param name="context">The web context.</param>
        /// <param name="stateData">The current download state.</param>
        private async void ProcessDownloadCancellation(Net.WebContext context, DownloadState stateData)
        {
            bool hasBeenFound = false;
            bool stopDownloading = false;

            // Setup the buffers.
            byte[] temp = null;
            byte[] store = new byte[0];
            byte[] receiveBuffer = new byte[8192];
            ArraySegment<byte> arrayBuffer = new ArraySegment<byte>(receiveBuffer);

            try
            {
                // While the end of the headers have not been found.
                while (!hasBeenFound && (context.SocketState == SocketState.Open) && !stateData.IsDownloadComplete)
                {
                    // Wait for data.
                    await context.ReceiveAsync(arrayBuffer, stateData.CancellationToken.Token);

                    // If the end of the headers has not been found.
                    if (!hasBeenFound && !stateData.IsDownloadComplete)
                    {
                        // Combine the received data with the existing.
                        temp = store.CombineParallel(receiveBuffer);

                        // Find the end of the header data.
                        hasBeenFound = Nequeo.Net.Utility.IsParse2CRLF(temp);

                        // Store the data until the end.
                        store = temp;
                        temp = null;
                    }

                    // If end of headers.
                    if (hasBeenFound)
                    {
                        // Get the request headers, and set the request headers.
                        List<NameValue> headers = base.ParseHeadersOnly(store, base.RequestTimeout);

                        // If headers exist.
                        if (headers != null)
                        {
                            // If the stop downloading header was sent.
                            if (headers.Count(u => u.Name.ToLower() == "STOPDOWNLOADINGFILE".ToLower()) > 0)
                            {
                                stopDownloading = true;

                                // Cancel the download operation.
                                stateData.CancellationToken.Cancel();
                            }
                        }
                    }

                    // If stop downloading.
                    if (stopDownloading)
                    {
                        // The way aync await works, it never goes back to
                        // where it left of, so if it gets to this point then
                        // the copying thread will stop. Send the cancellation
                        // request anyway.
                        if (!stateData.CancellationToken.IsCancellationRequested)
                            stateData.CancellationToken.Cancel();

                        // Cleaup the state.
                        if (stateData != null)
                        {
                            if (stateData.CancellationToken != null)
                                stateData.CancellationToken.Dispose();

                            // Dispose of the buffer.
                            if (stateData.File != null)
                                stateData.File.Dispose();

                            stateData.File = null;
                            stateData = null;
                        }
                        break;
                    }
                }
            }
            catch { }
            finally
            {
                temp = null;
                store = null;
                receiveBuffer = null;

                // Cancel the async mode.
                context.IsAsyncMode = false;
            }
        }

        /// <summary>
        /// Download file from query request.
        /// </summary>
        /// <param name="context">The web context.</param>
        private void DownloadFileQuery(Net.WebContext context)
        {
            FileStream requestStream = null;
            try
            {
                // Attempt to find the permission.
                Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                // If download permission is denied.
                if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Download))
                    throw new Nequeo.Exceptions.PermissionException("Permission denied");

                // Get the file location.
                string fileNameQuery = context.WebRequest.Query["FileName"];
                string fileSubDirectoryQuery = context.WebRequest.Query["Directory"];
                string fileNamePath = Common.Helper.GetDownloadFile(fileNameQuery, fileSubDirectoryQuery);
                string fileName = System.IO.Path.GetFileName(fileNamePath);
                string extension = System.IO.Path.GetExtension(fileNamePath);
                FileInfo fileInfo = new FileInfo(fileNamePath);

                // Get the file read position.
                long fileReadPosition = 0;
                long contentLength = fileInfo.Length;

                // Get the read position of the file.
                if (context.WebRequest.Query["FileReadPosition"] != null)
                    fileReadPosition = Int64.Parse(context.WebRequest.Query["FileReadPosition"]);

                // If the file read position is too large.
                if (fileReadPosition > fileInfo.Length)
                    // Throw position too large.
                    throw new Nequeo.Exceptions.InvalidLengthException("File read position invalid.");
                else
                {
                    // Open and read the file.
                    requestStream = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                    // Set the position to start reading from.
                    requestStream.Seek(fileReadPosition, SeekOrigin.Begin);
                }

                // If the read position has been set.
                if (fileReadPosition > 0)
                {
                    // Get the difference.
                    contentLength = fileInfo.Length - fileReadPosition;
                }

                // Send the file.
                context.WebResponse.ContentLength = contentLength;
                context.WebResponse.ContentType = "application/" + extension;
                context.WebResponse.AddHeader("content-disposition", "attachment; filename=\"" + fileName + "\"");
                context.WebResponse.ContentLanguage = "en-au";
                context.WebResponse.Server = base.Name;
                context.WebResponse.AddHeader("MemberResult", true.ToString());
                context.WebResponse.WriteWebResponseHeaders();

                // Send the file to the stream.
                Nequeo.IO.Stream.Operation.CopyStream(requestStream, context.WebResponse.Output, context.WebResponse.ContentLength, base.ResponseTimeout, base.WriteBufferSize);
                requestStream.Close();
            }
            catch (Nequeo.Exceptions.InvalidPathException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch (Nequeo.Exceptions.PermissionException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch { }
            finally
            {
                // Dispose of the buffer.
                if (requestStream != null)
                    requestStream.Dispose();
            }
        }

        /// <summary>
        /// Upload file from query request.
        /// </summary>
        /// <param name="context">The web context.</param>
        private void UploadFileQuery(Net.WebContext context)
        {
            bool copied = false;
            string fileNamePath = "";
            FileStream requestStream = null;

            try
            {
                // Attempt to find the permission.
                Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                // If upload permission is denied.
                if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Upload))
                    throw new Nequeo.Exceptions.PermissionException("Permission denied");

                // Get the user unique id.
                string fileNameQuery = context.WebRequest.Query["FileName"];
                string fileSubDirectoryQuery = context.WebRequest.Query["Directory"];
                fileNamePath = Common.Helper.GetUploadFile(fileNameQuery, fileSubDirectoryQuery);

                // Open and write to the file.
                requestStream = new FileStream(fileNamePath, FileMode.Create, FileAccess.Write, FileShare.Write);

                // Send the file to the stream.
                copied = Nequeo.IO.Stream.Operation.CopyStream(context.WebRequest.Input, requestStream, context.WebRequest.ContentLength, base.RequestTimeout, base.ReadBufferSize);
                requestStream.Close();

                // Send the result.
                context.WebResponse.ContentLength = 0;
                context.WebResponse.ContentLanguage = "en-au";
                context.WebResponse.Server = base.Name;
                context.WebResponse.AddHeader("MemberResult", true.ToString());
                context.WebResponse.WriteWebResponseHeaders();
            }
            catch (Nequeo.Exceptions.InvalidPathException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch (Nequeo.Exceptions.PermissionException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch { }
            finally
            {
                // Dispose of the buffer.
                if (requestStream != null)
                    requestStream.Dispose();

                // If all data was not copied.
                if (!copied)
                {
                    try
                    {
                        if (File.Exists(fileNamePath))
                            File.Delete(fileNamePath);
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// File list from query request.
        /// </summary>
        /// <param name="context">The web context.</param>
        private void FileListQuery(Net.WebContext context)
        {
            try
            {
                // Attempt to find the permission.
                Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                // If download permission is denied.
                if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.List))
                    throw new Nequeo.Exceptions.PermissionException("Permission denied");

                // Get the file location.
                string fileSubDirectoryQuery = context.WebRequest.Query["Directory"];
                string[] paths = Common.Helper.GetFileList(fileSubDirectoryQuery);

                // Convert the paths to byte array.
                byte[] buffer = String.Join("\r\n", paths).ToByteArray();

                // Send the result.
                context.WebResponse.ContentLength = buffer.Length;
                context.WebResponse.ContentLanguage = "en-au";
                context.WebResponse.Server = base.Name;
                context.WebResponse.AddHeader("MemberResult", true.ToString());
                context.WebResponse.WriteWebResponseHeaders();

                // Write the data to the stream.
                context.WebResponse.Write(buffer);
            }
            catch (Nequeo.Exceptions.InvalidPathException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch (Nequeo.Exceptions.PermissionException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch { }
        }

        /// <summary>
        /// Delete file from query request.
        /// </summary>
        /// <param name="context">The web context.</param>
        private void DeleteFileQuery(Net.WebContext context)
        {
            try
            {
                // Attempt to find the permission.
                Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                // If upload permission is denied.
                if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Delete))
                    throw new Nequeo.Exceptions.PermissionException("Permission denied");

                // Get the user unique id.
                string fileNameQuery = context.WebRequest.Query["FileName"];
                string fileSubDirectoryQuery = context.WebRequest.Query["Directory"];
                string fileNamePath = Common.Helper.GetFilePath(fileNameQuery, fileSubDirectoryQuery);

                // If the file exists then delete the file.
                if (File.Exists(fileNamePath))
                    File.Delete(fileNamePath);

                // Send the result.
                context.WebResponse.ContentLength = 0;
                context.WebResponse.ContentLanguage = "en-au";
                context.WebResponse.Server = base.Name;
                context.WebResponse.AddHeader("MemberResult", true.ToString());
                context.WebResponse.WriteWebResponseHeaders();
            }
            catch (Nequeo.Exceptions.InvalidPathException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch (Nequeo.Exceptions.PermissionException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch { }
        }

        /// <summary>
        /// Rename file from query request.
        /// </summary>
        /// <param name="context">The web context.</param>
        private void RenameFileQuery(Net.WebContext context)
        {
            try
            {
                // Attempt to find the permission.
                Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                // If upload permission is denied.
                if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Rename))
                    throw new Nequeo.Exceptions.PermissionException("Permission denied");

                // Get the user unique id.
                string fileNameCuurentQuery = context.WebRequest.Query["CurrentFileName"];
                string fileNameNewQuery = context.WebRequest.Query["NewFileName"];
                string fileSubDirectoryQuery = context.WebRequest.Query["Directory"];
                string fileNameCurrrentPath = Common.Helper.GetFilePath(fileNameCuurentQuery, fileSubDirectoryQuery);
                string fileNameNewPath = Common.Helper.GetFilePath(fileNameNewQuery, fileSubDirectoryQuery);

                // If the file exists then remane the file.
                if (File.Exists(fileNameCurrrentPath))
                    File.Move(fileNameCurrrentPath, fileNameNewPath);

                // Send the result.
                context.WebResponse.ContentLength = 0;
                context.WebResponse.ContentLanguage = "en-au";
                context.WebResponse.Server = base.Name;
                context.WebResponse.AddHeader("MemberResult", true.ToString());
                context.WebResponse.WriteWebResponseHeaders();
            }
            catch (Nequeo.Exceptions.InvalidPathException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch (Nequeo.Exceptions.PermissionException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch { }
        }

        /// <summary>
        /// Copy file from query request.
        /// </summary>
        /// <param name="context">The web context.</param>
        private void CopyFileQuery(Net.WebContext context)
        {
            try
            {
                // Attempt to find the permission.
                Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                // If upload permission is denied.
                if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Copy))
                    throw new Nequeo.Exceptions.PermissionException("Permission denied");

                // Get the user unique id.
                string sourcePathFileNameQuery = context.WebRequest.Query["SourcePathFileName"];
                string destinationPathFileNameQuery = context.WebRequest.Query["DestinationPathFileName"];
                string sourcePathFileName = Common.Helper.GetFilePath(sourcePathFileNameQuery);
                string destinationPathFileName = Common.Helper.GetFilePath(destinationPathFileNameQuery);

                // If the file exists then remane the file.
                if (File.Exists(sourcePathFileNameQuery))
                    File.Copy(sourcePathFileNameQuery, destinationPathFileName, true);

                // Send the result.
                context.WebResponse.ContentLength = 0;
                context.WebResponse.ContentLanguage = "en-au";
                context.WebResponse.Server = base.Name;
                context.WebResponse.AddHeader("MemberResult", true.ToString());
                context.WebResponse.WriteWebResponseHeaders();
            }
            catch (Nequeo.Exceptions.InvalidPathException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch (Nequeo.Exceptions.PermissionException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch { }
        }

        /// <summary>
        /// Move file from query request.
        /// </summary>
        /// <param name="context">The web context.</param>
        private void MoveFileQuery(Net.WebContext context)
        {
            try
            {
                // Attempt to find the permission.
                Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                // If upload permission is denied.
                if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Move))
                    throw new Nequeo.Exceptions.PermissionException("Permission denied");

                // Get the user unique id.
                string sourcePathFileNameQuery = context.WebRequest.Query["SourcePathFileName"];
                string destinationPathFileNameQuery = context.WebRequest.Query["DestinationPathFileName"];
                string sourcePathFileName = Common.Helper.GetFilePath(sourcePathFileNameQuery);
                string destinationPathFileName = Common.Helper.GetFilePath(destinationPathFileNameQuery);

                // If the file exists then remane the file.
                if (File.Exists(sourcePathFileNameQuery))
                    File.Move(sourcePathFileNameQuery, destinationPathFileName);

                // Send the result.
                context.WebResponse.ContentLength = 0;
                context.WebResponse.ContentLanguage = "en-au";
                context.WebResponse.Server = base.Name;
                context.WebResponse.AddHeader("MemberResult", true.ToString());
                context.WebResponse.WriteWebResponseHeaders();
            }
            catch (Nequeo.Exceptions.InvalidPathException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch (Nequeo.Exceptions.PermissionException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch { }
        }

        /// <summary>
        /// File details from query request.
        /// </summary>
        /// <param name="context">The web context.</param>
        private void FileDetailsQuery(Net.WebContext context)
        {
            try
            {
                // Attempt to find the permission.
                Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                // If upload permission is denied.
                if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.List))
                    throw new Nequeo.Exceptions.PermissionException("Permission denied");

                // Get the user unique id.
                string fileNameQuery = context.WebRequest.Query["FileName"];
                string fileSubDirectoryQuery = context.WebRequest.Query["Directory"];
                string fileNamePath = Common.Helper.GetFilePath(fileNameQuery, fileSubDirectoryQuery);
                string[] details = new string[15];

                // Get the file information.
                FileInfo fileInfo = new FileInfo(fileNamePath);
                details[0] = fileInfo.Attributes.ToString();
                details[1] = fileInfo.CreationTime.ToString();
                details[2] = fileInfo.CreationTimeUtc.ToString();
                details[3] = Common.Helper.GetRelativePath(fileInfo.Directory.FullName);
                details[4] = Common.Helper.GetRelativePath(fileInfo.DirectoryName);
                details[5] = fileInfo.Exists.ToString();
                details[6] = fileInfo.Extension.ToString();
                details[7] = Common.Helper.GetRelativePath(fileInfo.FullName);
                details[8] = fileInfo.IsReadOnly.ToString();
                details[9] = fileInfo.LastAccessTime.ToString();
                details[10] = fileInfo.LastAccessTimeUtc.ToString();
                details[11] = fileInfo.LastWriteTime.ToString();
                details[12] = fileInfo.LastWriteTimeUtc.ToString();
                details[13] = fileInfo.Length.ToString();
                details[14] = fileInfo.Name.ToString();

                // Convert the paths to byte array.
                byte[] buffer = String.Join("\r\n", details).ToByteArray();

                // Send the result.
                context.WebResponse.ContentLength = buffer.Length;
                context.WebResponse.ContentLanguage = "en-au";
                context.WebResponse.Server = base.Name;
                context.WebResponse.AddHeader("MemberResult", true.ToString());
                context.WebResponse.WriteWebResponseHeaders();

                // Write the data to the stream.
                context.WebResponse.Write(buffer);
            }
            catch (Nequeo.Exceptions.InvalidPathException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch (Nequeo.Exceptions.PermissionException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch { }
        }

        /// <summary>
        /// Delete directory from query request.
        /// </summary>
        /// <param name="context">The web context.</param>
        private void DeleteDirectoryQuery(Net.WebContext context)
        {
            try
            {
                // Attempt to find the permission.
                Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                // If upload permission is denied.
                if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Delete))
                    throw new Nequeo.Exceptions.PermissionException("Permission denied");

                // Get the user unique id.
                string fileSubDirectoryQuery = context.WebRequest.Query["Directory"];
                string directoryPath = Common.Helper.GetDirectoryPath(fileSubDirectoryQuery);

                // If the directory exists then delete the directory.
                if (Directory.Exists(directoryPath))
                {
                    // Do not delete if not empty.
                    Directory.Delete(directoryPath, false);
                }

                // Send the result.
                context.WebResponse.ContentLength = 0;
                context.WebResponse.ContentLanguage = "en-au";
                context.WebResponse.Server = base.Name;
                context.WebResponse.AddHeader("MemberResult", true.ToString());
                context.WebResponse.WriteWebResponseHeaders();
            }
            catch (Nequeo.Exceptions.InvalidPathException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch (Nequeo.Exceptions.PermissionException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch { }
        }

        /// <summary>
        /// Create directory from query request.
        /// </summary>
        /// <param name="context">The web context.</param>
        private void CreateDirectoryQuery(Net.WebContext context)
        {
            try
            {
                // Attempt to find the permission.
                Nequeo.Security.IPermission perState = (Nequeo.Security.IPermission)_permission.Find(context.UniqueIdentifier);

                // If upload permission is denied.
                if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Create))
                    throw new Nequeo.Exceptions.PermissionException("Permission denied");

                // Get the user unique id.
                string fileSubDirectoryQuery = context.WebRequest.Query["Directory"];
                bool created = Common.Helper.CreateDirectory(fileSubDirectoryQuery);

                // Send the result.
                context.WebResponse.ContentLength = 0;
                context.WebResponse.ContentLanguage = "en-au";
                context.WebResponse.Server = base.Name;
                context.WebResponse.AddHeader("MemberResult", created.ToString());
                context.WebResponse.WriteWebResponseHeaders();
            }
            catch (Nequeo.Exceptions.InvalidPathException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch (Nequeo.Exceptions.PermissionException)
            {
                try
                {
                    // Send an error response.
                    context.WebResponse.KeepAlive = false;
                    context.WebResponse.AddHeader("MemberResult", false.ToString());
                    CreateInternalServerErrorHeaders(context.WebResponse);
                }
                catch { }
            }
            catch { }
        }

        /// <summary>
        /// Create OK headers.
        /// </summary>
        /// <param name="response">The web response.</param>
        /// <param name="contentLength">The content length.</param>
        private void CreateOkHeaders(Net.WebResponse response, long contentLength = 0)
        {
            // Send the response.
            response.StatusCode = 200;
            response.StatusDescription = "OK";
            response.Server = base.Name;
            response.ContentLength = contentLength;
            response.WriteWebResponseHeaders();
        }

        /// <summary>
        /// Create Bad Request headers.
        /// </summary>
        /// <param name="response">The web response.</param>
        /// <param name="contentLength">The content length.</param>
        private void CreateBadRequestHeaders(Net.WebResponse response, long contentLength = 0)
        {
            // Send the response.
            response.StatusCode = 400;
            response.StatusDescription = "Bad Request";
            response.Server = base.Name;
            response.ContentLength = contentLength;
            response.WriteWebResponseHeaders();
        }

        /// <summary>
        /// Create Internal Server Error headers.
        /// </summary>
        /// <param name="response">The web response.</param>
        /// <param name="contentLength">The content length.</param>
        private void CreateInternalServerErrorHeaders(Net.WebResponse response, long contentLength = 0)
        {
            // Send the response.
            response.StatusCode = 500;
            response.StatusDescription = "Internal Server Error";
            response.Server = base.Name;
            response.ContentLength = contentLength;
            response.WriteWebResponseHeaders();
        }
        #endregion

        #region Dispose Object Methods
        private bool _disposed = false;

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // Dispose of the base objects.
                base.Dispose();

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_state != null)
                        _state.Dispose();

                    if (_permission != null)
                        _permission.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _state = null;
                _permission = null;
            }
        }
        #endregion
    }
}
