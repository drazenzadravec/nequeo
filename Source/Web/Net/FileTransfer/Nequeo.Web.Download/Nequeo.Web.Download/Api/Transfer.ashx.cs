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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Nequeo.Extension;

namespace Nequeo.Web.Download.Api
{
    /// <summary>
    /// Transfer file http web handler.
    /// </summary>
    public class Transfer : Nequeo.Service.Web.HttpHandlerEx
    {
        #region Constructor
        /// <summary>
        /// Transfer file http web handler.
        /// </summary>
        public Transfer()
        {
            // Assign the token provider.
            _token = Common.Helper.TokenProvider;
        }
        #endregion

        #region Private Fields
        private Nequeo.Data.Provider.IToken _token = null;
        private string _serviceName = Common.Helper.ServiceName;
        private long _requestTimeout = 30000;
        private long _responseTimeout = 30000;
        private int _writeBufferSize = 1000000;
        private int _readBufferSize = 1000000;
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

        #region Private Base server initialiser members
        /// <summary>
        /// Http context handler.
        /// </summary>
        /// <param name="context">The current http context.</param>
        public override void HttpContext(HttpContext context)
        {
            HttpRequest request = null;
            HttpResponse response = null;
            bool keepAlive = true;
            bool isServerError = false;
            bool isError = true;

            try
            {
                // If the physical path has not been set.
                if (String.IsNullOrEmpty(Common.Helper.PhysicalPath))
                    Common.Helper.PhysicalPath = context.Server.MapPath("/");

                // Assign the context.
                request = context.Request;
                response = context.Response;
                string executionMember = "";

                // Should the connection be kept alive.
                keepAlive = (String.IsNullOrEmpty(request.Headers["keep-alive"]) ? false : true);

                // Look for a query.
                if (request.QueryString != null && !String.IsNullOrEmpty(request.QueryString["Member"]))
                {
                    executionMember = request.QueryString["Member"];
                    string actionName = request.QueryString["ActionName"];

                    // Set the headers.
                    response.AddHeader("Member", executionMember);
                    response.AddHeader("ActionName", actionName);

                    // Select the execution member.
                    isError = ExecutionMember(context, executionMember);
                }
                else
                {
                    // Do not allow access.
                    keepAlive = false;
                    throw new Exception("No query has been found.");
                }

                // If error has occured.
                if (isError)
                {
                    // Send an error response.
                    response.StatusCode = 400;
                    response.StatusDescription = "Bad Request";
                    response.AddHeader("Content-Length", "0");
                    response.Write("");
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
                        response.StatusCode = 500;
                        response.StatusDescription = "Internal server error";
                        response.AddHeader("Content-Length", "0");
                        response.Write("");
                    }
                    catch { keepAlive = false; }
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
        /// <returns>True if error; else false.</returns>
        private bool ExecutionMember(HttpContext context, string executionMember)
        {
            bool isError = false;

            // Select the member to execute.
            switch (executionMember.ToUpper())
            {
                case "DOWNLOADFILE":
                    // Download the file.
                    isError = DownloadFile(context);
                    break;

                case "FILELIST":
                    // File directory list.
                    isError = FileList(context);
                    break;

                case "FILEDETAILS":
                    // File details.
                    isError = FileDetails(context);
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
        /// DownloadFile
        /// </summary>
        /// <param name="context">The web context.</param>
        /// <returns>True if error; else false.</returns>
        private bool DownloadFile(HttpContext context)
        {
            bool isError = false;

            AutoResetEvent waitEvent = new AutoResetEvent(false);
            FileStream requestStream = null;
            try
            {
                // Get the user unique id and the current token issued.
                string uniqueIdentifier = context.Request.QueryString["UniqueIdentifier"];
                string token = context.Request.QueryString["Token"];

                // State object.
                Common.TokenState tokenState = new Common.TokenState();
                tokenState.IsValid = false;
                tokenState.Permission = null;

                // Is token valid.
                _token.IsValid(uniqueIdentifier, _serviceName, token, (result, permission, state) =>
                {
                    try
                    {
                        // Get the token validation data.
                        Common.TokenState stateToken = (Common.TokenState)state;
                        stateToken.IsValid = result;
                        stateToken.Permission = permission;
                    }
                    catch { }

                    // Validation has ended.
                    waitEvent.Set();

                }, uniqueIdentifier, tokenState);

                // Wait until the token validation.
                waitEvent.WaitOne((int)(_requestTimeout + 10000));

                // If not valid vredentails
                if (!tokenState.IsValid)
                    throw new Nequeo.Exceptions.InvalidCredentailsException("Invalid credentails.");

                // Attempt to find the permission.
                Nequeo.Security.IPermission perState = tokenState.Permission;

                // If download permission is denied.
                if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Download))
                    throw new Nequeo.Exceptions.PermissionException("Permission denied");

                // Get the file location.
                string fileNameQuery = context.Request.QueryString["FileName"];
                string fileSubDirectoryQuery = context.Request.QueryString["Directory"];
                string fileNamePath = Common.Helper.GetDownloadFile(fileNameQuery, fileSubDirectoryQuery);
                string fileName = System.IO.Path.GetFileName(fileNamePath);
                string extension = System.IO.Path.GetExtension(fileNamePath);
                FileInfo fileInfo = new FileInfo(fileNamePath);

                // Get the file read position.
                long fileReadPosition = 0;
                long contentLength = fileInfo.Length;

                // Get the read position of the file.
                if (context.Request.QueryString["FileReadPosition"] != null)
                    fileReadPosition = Int64.Parse(context.Request.QueryString["FileReadPosition"]);

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
                context.Response.AddHeader("Content-Length", contentLength.ToString());
                context.Response.ContentType = "application/" + extension;
                context.Response.AddHeader("content-disposition", "attachment; filename=\"" + fileName + "\"");
                context.Response.AddHeader("MemberResult", true.ToString());

                // Send the file to the stream.
                Nequeo.IO.Stream.Operation.CopyStream(requestStream, context.Response.OutputStream, contentLength, _responseTimeout, _writeBufferSize);
                requestStream.Close();
            }
            catch (Nequeo.Exceptions.InvalidPathException)
            {
                try
                {
                    // Send an error response.
                    context.Response.StatusCode = 500;
                    context.Response.StatusDescription = "Internal server error";
                    context.Response.AddHeader("Content-Length", "0");
                    context.Response.AddHeader("MemberResult", false.ToString());
                    context.Response.Write("");
                }
                catch { }
            }
            catch (Nequeo.Exceptions.PermissionException)
            {
                try
                {
                    // Send an error response.
                    context.Response.StatusCode = 500;
                    context.Response.StatusDescription = "Internal server error";
                    context.Response.AddHeader("Content-Length", "0");
                    context.Response.AddHeader("MemberResult", false.ToString());
                    context.Response.Write("");
                }
                catch { }
            }
            catch (Nequeo.Exceptions.InvalidCredentailsException)
            {
                try
                {
                    // Send an error response.
                    context.Response.StatusCode = 500;
                    context.Response.StatusDescription = "Internal server error";
                    context.Response.AddHeader("Content-Length", "0");
                    context.Response.AddHeader("MemberResult", false.ToString());
                    context.Response.Write("");
                }
                catch { }
            }
            catch { }
            finally
            {
                // Dispose of the buffer.
                if (requestStream != null)
                    requestStream.Dispose();

                if (waitEvent != null)
                    waitEvent.Dispose();
            }
            
            // Return the result.
            return isError;
        }

        /// <summary>
        /// FileList
        /// </summary>
        /// <param name="context">The web context.</param>
        /// <returns>True if error; else false.</returns>
        private bool FileList(HttpContext context)
        {
            bool isError = false;

            AutoResetEvent waitEvent = new AutoResetEvent(false);
            try
            {
                // Get the user unique id and the current token issued.
                string uniqueIdentifier = context.Request.QueryString["UniqueIdentifier"];
                string token = context.Request.QueryString["Token"];

                // State object.
                Common.TokenState tokenState = new Common.TokenState();
                tokenState.IsValid = false;
                tokenState.Permission = null;

                // Is token valid.
                _token.IsValid(uniqueIdentifier, _serviceName, token, (result, permission, state) =>
                {
                    try
                    {
                        // Get the token validation data.
                        Common.TokenState stateToken = (Common.TokenState)state;
                        stateToken.IsValid = result;
                        stateToken.Permission = permission;
                    }
                    catch { }

                    // Validation has ended.
                    waitEvent.Set();

                }, uniqueIdentifier, tokenState);

                // Wait until the token validation.
                waitEvent.WaitOne((int)(_requestTimeout + 10000));

                // If not valid vredentails
                if (!tokenState.IsValid)
                    throw new Nequeo.Exceptions.InvalidCredentailsException("Invalid credentails.");

                // Attempt to find the permission.
                Nequeo.Security.IPermission perState = tokenState.Permission;

                // If download permission is denied.
                if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Download))
                    throw new Nequeo.Exceptions.PermissionException("Permission denied");

                // Get the file location.
                string fileSubDirectoryQuery = context.Request.QueryString["Directory"];
                string[] paths = Common.Helper.GetFileList(fileSubDirectoryQuery);

                // Convert the paths to byte array.
                string buffer = String.Join("\r\n", paths);

                // Send the file.
                context.Response.AddHeader("Content-Length", buffer.Length.ToString());
                context.Response.ContentType = "text/txt";
                context.Response.AddHeader("MemberResult", true.ToString());

                // Write the data to the stream.
                context.Response.Write(buffer);
            }
            catch (Nequeo.Exceptions.InvalidPathException)
            {
                try
                {
                    // Send an error response.
                    context.Response.StatusCode = 500;
                    context.Response.StatusDescription = "Internal server error";
                    context.Response.AddHeader("Content-Length", "0");
                    context.Response.AddHeader("MemberResult", false.ToString());
                    context.Response.Write("");
                }
                catch { }
            }
            catch (Nequeo.Exceptions.PermissionException)
            {
                try
                {
                    // Send an error response.
                    context.Response.StatusCode = 500;
                    context.Response.StatusDescription = "Internal server error";
                    context.Response.AddHeader("Content-Length", "0");
                    context.Response.AddHeader("MemberResult", false.ToString());
                    context.Response.Write("");
                }
                catch { }
            }
            catch (Nequeo.Exceptions.InvalidCredentailsException)
            {
                try
                {
                    // Send an error response.
                    context.Response.StatusCode = 500;
                    context.Response.StatusDescription = "Internal server error";
                    context.Response.AddHeader("Content-Length", "0");
                    context.Response.AddHeader("MemberResult", false.ToString());
                    context.Response.Write("");
                }
                catch { }
            }
            catch { }
            finally
            {
                if (waitEvent != null)
                    waitEvent.Dispose();
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// FileDetails
        /// </summary>
        /// <param name="context">The web context.</param>
        /// <returns>True if error; else false.</returns>
        private bool FileDetails(HttpContext context)
        {
            bool isError = false;

            AutoResetEvent waitEvent = new AutoResetEvent(false);
            try
            {
                // Get the user unique id and the current token issued.
                string uniqueIdentifier = context.Request.QueryString["UniqueIdentifier"];
                string token = context.Request.QueryString["Token"];

                // State object.
                Common.TokenState tokenState = new Common.TokenState();
                tokenState.IsValid = false;
                tokenState.Permission = null;

                // Is token valid.
                _token.IsValid(uniqueIdentifier, _serviceName, token, (result, permission, state) =>
                {
                    try
                    {
                        // Get the token validation data.
                        Common.TokenState stateToken = (Common.TokenState)state;
                        stateToken.IsValid = result;
                        stateToken.Permission = permission;
                    }
                    catch { }

                    // Validation has ended.
                    waitEvent.Set();

                }, uniqueIdentifier, tokenState);

                // Wait until the token validation.
                waitEvent.WaitOne((int)(_requestTimeout + 10000));

                // If not valid vredentails
                if (!tokenState.IsValid)
                    throw new Nequeo.Exceptions.InvalidCredentailsException("Invalid credentails.");

                // Attempt to find the permission.
                Nequeo.Security.IPermission perState = tokenState.Permission;

                // If download permission is denied.
                if (perState == null || !perState.Access() || !perState.Permission.HasFlag(Nequeo.Security.PermissionType.Download))
                    throw new Nequeo.Exceptions.PermissionException("Permission denied");

                // Get the user unique id.
                string fileNameQuery = context.Request.QueryString["FileName"];
                string fileSubDirectoryQuery = context.Request.QueryString["Directory"];
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
                string buffer = String.Join("\r\n", details);

                // Send the file.
                context.Response.AddHeader("Content-Length", buffer.Length.ToString());
                context.Response.ContentType = "text/txt";
                context.Response.AddHeader("MemberResult", true.ToString());

                // Write the data to the stream.
                context.Response.Write(buffer);
            }
            catch (Nequeo.Exceptions.InvalidPathException)
            {
                try
                {
                    // Send an error response.
                    context.Response.StatusCode = 500;
                    context.Response.StatusDescription = "Internal server error";
                    context.Response.AddHeader("Content-Length", "0");
                    context.Response.AddHeader("MemberResult", false.ToString());
                    context.Response.Write("");
                }
                catch { }
            }
            catch (Nequeo.Exceptions.PermissionException)
            {
                try
                {
                    // Send an error response.
                    context.Response.StatusCode = 500;
                    context.Response.StatusDescription = "Internal server error";
                    context.Response.AddHeader("Content-Length", "0");
                    context.Response.AddHeader("MemberResult", false.ToString());
                    context.Response.Write("");
                }
                catch { }
            }
            catch (Nequeo.Exceptions.InvalidCredentailsException)
            {
                try
                {
                    // Send an error response.
                    context.Response.StatusCode = 500;
                    context.Response.StatusDescription = "Internal server error";
                    context.Response.AddHeader("Content-Length", "0");
                    context.Response.AddHeader("MemberResult", false.ToString());
                    context.Response.Write("");
                }
                catch { }
            }
            catch { }
            finally
            {
                if (waitEvent != null)
                    waitEvent.Dispose();
            }

            // Return the result.
            return isError;
        }
        #endregion
    }
}