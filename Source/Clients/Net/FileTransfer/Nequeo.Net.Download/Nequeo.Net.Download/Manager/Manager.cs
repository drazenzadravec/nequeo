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
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using Nequeo.Model;
using Nequeo.Extension;

namespace Nequeo.Net.Download
{
    /// <summary>
    /// Download manager client.
    /// </summary>
    public sealed partial class ManagerClient : Nequeo.Net.NetClient, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Download manager client.
        /// </summary>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public ManagerClient()
        {
            Initialise();
        }

        /// <summary>
        /// Download manager client.
        /// </summary>
        /// <param name="address">An IP address.</param>
        /// <param name="port">The port to connect to.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public ManagerClient(IPAddress address, int port)
            : base(address, port)
        {
            Initialise();
        }

        /// <summary>
        /// Download manager client.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
        /// <param name="port">The port to connect to.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public ManagerClient(string hostNameOrAddress, int port)
            : base(hostNameOrAddress, port)
        {
            Initialise();
        }
        #endregion

        #region Private Fields
        private object _lockConnectObject = new object();
        private object _lockRequestObject = new object();

        private bool _reconnectWhenNoConnection = false;

        private Dictionary<string, object> _callback = new Dictionary<string, object>();
        private Dictionary<string, object> _state = new Dictionary<string, object>();
        #endregion

        #region Public Events
        /// <summary>
        /// The on error event handler, triggered when data received from the server is any type of error.
        /// </summary>
        public event Nequeo.Threading.EventHandler<string, string, string> OnError;

        #endregion

        #region Public Methods
        /// <summary>
        /// Validate the token.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier of the client.</param>
        /// <param name="token">The token issued to the client.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Validate(string uniqueIdentifier, string token, Action<bool, object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "VALIDATE" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "VALIDATE" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.AddHeader("Member", "VALIDATE");
                    request.AddHeader("ActionName", (String.IsNullOrEmpty(actionName) ? "VALIDATE" : actionName));
                    request.AddHeader("UniqueIdentifier", uniqueIdentifier);
                    request.AddHeader("Token", token);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// Download a file.
        /// </summary>
        /// <param name="filename">The name of the file to download.</param>
        /// <param name="directory">The directory on the remote system (root '' or '/' or '\').</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="fileReadPosition">The position in the file to start reading from.</param>
        /// <param name="fileReadSize">The number of bytes to read.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void DownloadFile(string filename, string directory, Action<Nequeo.Net.NetResponse, object> callback, long fileReadPosition = 0, long fileReadSize = 0, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "DOWNLOADFILE" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "DOWNLOADFILE" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.AddHeader("Member", "DOWNLOADFILE");
                    request.AddHeader("ActionName", (String.IsNullOrEmpty(actionName) ? "DOWNLOADFILE" : actionName));
                    request.AddHeader("FileName", filename);
                    request.AddHeader("Directory", String.IsNullOrEmpty(directory) ? "" : directory);

                    // Position in the file to start reading from.
                    if (fileReadPosition > 0)
                        request.AddHeader("FileReadPosition", fileReadPosition.ToString());

                    // Number of bytes to read.
                    if (fileReadSize > 0)
                        request.AddHeader("FileReadSize", fileReadSize.ToString());

                    // Write the headers.
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// Download a file from a query request.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier of the client.</param>
        /// <param name="token">The token issued to the client.</param>
        /// <param name="filename">The name of the file to download.</param>
        /// <param name="directory">The directory on the remote system (root '' or '/' or '\').</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="fileReadPosition">The position in the file to start reading from.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void DownloadFile(string uniqueIdentifier, string token, string filename, string directory, Action<Nequeo.Net.NetResponse, object> callback, long fileReadPosition = 0, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "DOWNLOADFILE" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "DOWNLOADFILE" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.Path = 
                        "/?UniqueIdentifier=" + uniqueIdentifier + 
                        "&Token=" + token + 
                        "&FileName=" + filename +
                        "&Directory=" + (String.IsNullOrEmpty(directory) ? "" : directory) +
                        "&FileReadPosition=" + (fileReadPosition > 0 ? fileReadPosition.ToString() : (0).ToString()) +
                        "&Member=DOWNLOADFILE" + 
                        "&ActionName=" + (String.IsNullOrEmpty(actionName) ? "DOWNLOADFILE" : actionName);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// Download a file from a query request.
        /// </summary>
        /// <param name="path">The path url to the resource (/api/transfer.ashx).</param>
        /// <param name="uniqueIdentifier">The unique identifier of the client.</param>
        /// <param name="token">The token issued to the client.</param>
        /// <param name="filename">The name of the file to download.</param>
        /// <param name="directory">The directory on the remote system (root '' or '/' or '\').</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="fileReadPosition">The position in the file to start reading from.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void DownloadFile(string path, string uniqueIdentifier, string token, string filename, string directory, Action<Nequeo.Net.NetResponse, object> callback, long fileReadPosition = 0, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "DOWNLOADFILE" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "DOWNLOADFILE" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.Path = "/" + path.Replace("/", "\\").Trim(new char[] { '\\' }) +
                        "?UniqueIdentifier=" + uniqueIdentifier +
                        "&Token=" + token +
                        "&FileName=" + filename +
                        "&Directory=" + (String.IsNullOrEmpty(directory) ? "" : directory) +
                        "&FileReadPosition=" + (fileReadPosition > 0 ? fileReadPosition.ToString() : (0).ToString()) +
                        "&Member=DOWNLOADFILE" +
                        "&ActionName=" + (String.IsNullOrEmpty(actionName) ? "DOWNLOADFILE" : actionName);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// Stop downloading file.
        /// </summary>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void StopDownloadingFile(Action<bool, object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "STOPDOWNLOADINGFILE" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "STOPDOWNLOADINGFILE" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.AddHeader("Member", "STOPDOWNLOADINGFILE");
                    request.AddHeader("ActionName", (String.IsNullOrEmpty(actionName) ? "STOPDOWNLOADINGFILE" : actionName));
                    request.AddHeader("StopDownloadingFile", "");
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// Upload a file.
        /// </summary>
        /// <param name="data">The stream containing the data to send.</param>
        /// <param name="filename">The name of the file to create.</param>
        /// <param name="fileSize">The total file size.</param>
        /// <param name="directory">The directory on the remote system where the file is to be stored (root '' or '/' or '\').</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="fileWritePosition">The position in the file to start writing from.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <returns>The task to execute.</returns>
        public Task<bool> UploadFile(Stream data, string filename, long fileSize, string directory, CancellationToken cancellationToken, Action<bool, object> callback, long fileWritePosition = 0, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Create a new task.
                return Task<bool>.Factory.StartNew(() =>
                {
                    // Only allow one request at a time.
                    lock (_lockRequestObject)
                    {
                        bool ret = false;
                        try
                        {
                            // Assign the call back.
                            _callback[(String.IsNullOrEmpty(actionName) ? "UPLOADFILE" : actionName)] = callback;
                            _state[(String.IsNullOrEmpty(actionName) ? "UPLOADFILE" : actionName)] = state;

                            // Write the request to the server.
                            Nequeo.Net.NetRequest request = base.GetRequest();
                            request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                            request.Method = "POST";
                            request.ContentLength = fileSize;
                            request.KeepAlive = true;
                            request.AddHeader("Member", "UPLOADFILE");
                            request.AddHeader("ActionName", (String.IsNullOrEmpty(actionName) ? "UPLOADFILE" : actionName));
                            request.AddHeader("FileName", filename);
                            request.AddHeader("Directory", String.IsNullOrEmpty(directory) ? "" : directory);

                            // Position in the file to start writing from.
                            if (fileWritePosition > 0)
                                request.AddHeader("FileWritePosition", fileWritePosition.ToString());

                            // Write the headers.
                            request.WriteNetRequestHeaders();

                            // Send the data.
                            ret = Nequeo.IO.Stream.Operation.CopyStream(data, request.Output, cancellationToken, fileSize, base.RequestTimeout, base.WriteBufferSize);
                        }
                        catch
                        {
                            try
                            {
                                // Call the callback action.
                                callback(false, state);
                            }
                            catch { }
                        }

                        // return the result.
                        return ret;
                    }

                }, cancellationToken);
            }
            else
            {
                // Create a new task.
                return Task<bool>.Factory.StartNew(() =>
                {
                    // No action was performed.
                    return false;

                }, cancellationToken);
            }
        }

        /// <summary>
        /// Upload a file from a query request.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier of the client.</param>
        /// <param name="token">The token issued to the client.</param>
        /// <param name="data">The stream containing the data to send.</param>
        /// <param name="filename">The name of the file to create.</param>
        /// <param name="fileSize">The total file size.</param>
        /// <param name="directory">The directory on the remote system where the file is to be stored (root '' or '/' or '\').</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <returns>The task to execute.</returns>
        public Task<bool> UploadFile(string uniqueIdentifier, string token, Stream data, string filename, long fileSize, string directory, 
            CancellationToken cancellationToken, Action<bool, object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Create a new task.
                return Task<bool>.Factory.StartNew(() =>
                {
                    // Only allow one request at a time.
                    lock (_lockRequestObject)
                    {
                        bool ret = false;
                        try
                        {
                            // Assign the call back.
                            _callback[(String.IsNullOrEmpty(actionName) ? "UPLOADFILE" : actionName)] = callback;
                            _state[(String.IsNullOrEmpty(actionName) ? "UPLOADFILE" : actionName)] = state;

                            // Write the request to the server.
                            Nequeo.Net.NetRequest request = base.GetRequest();
                            request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                            request.Method = "POST";
                            request.ContentLength = fileSize;
                            request.KeepAlive = true;
                            request.Path =
                                "/?UniqueIdentifier=" + uniqueIdentifier +
                                "&Token=" + token +
                                "&FileName=" + filename +
                                "&Directory=" + (String.IsNullOrEmpty(directory) ? "" : directory) +
                                "&Member=UPLOADFILE" +
                                "&ActionName=" + (String.IsNullOrEmpty(actionName) ? "UPLOADFILE" : actionName);
                            request.WriteNetRequestHeaders();

                            // Send the data.
                            ret = Nequeo.IO.Stream.Operation.CopyStream(data, request.Output, cancellationToken, fileSize, base.RequestTimeout, base.WriteBufferSize);
                        }
                        catch
                        {
                            try
                            {
                                // Call the callback action.
                                callback(false, state);
                            }
                            catch { }
                        }

                        // return the result.
                        return ret;
                    }

                }, cancellationToken);
            }
            else
            {
                // Create a new task.
                return Task<bool>.Factory.StartNew(() =>
                {
                    // No action was performed.
                    return false;

                }, cancellationToken);
            }
        }

        /// <summary>
        /// File list.
        /// </summary>
        /// <param name="directory">The directory on the remote system (root '' or '/' or '\').</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void FileList(string directory, Action<string[], object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "FILELIST" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "FILELIST" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.AddHeader("Member", "FILELIST");
                    request.AddHeader("ActionName", (String.IsNullOrEmpty(actionName) ? "FILELIST" : actionName));
                    request.AddHeader("Directory", String.IsNullOrEmpty(directory) ? "" : directory);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// File list from a query request.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier of the client.</param>
        /// <param name="token">The token issued to the client.</param>
        /// <param name="directory">The directory on the remote system (root '' or '/' or '\').</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void FileList(string uniqueIdentifier, string token, string directory, Action<string[], object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "FILELIST" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "FILELIST" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.Path =
                        "/?UniqueIdentifier=" + uniqueIdentifier +
                        "&Token=" + token +
                        "&Directory=" + (String.IsNullOrEmpty(directory) ? "" : directory) +
                        "&Member=FILELIST" +
                        "&ActionName=" + (String.IsNullOrEmpty(actionName) ? "FILELIST" : actionName);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// File list from a query request.
        /// </summary>
        /// <param name="path">The path url to the resource (/api/transfer.ashx).</param>
        /// <param name="uniqueIdentifier">The unique identifier of the client.</param>
        /// <param name="token">The token issued to the client.</param>
        /// <param name="directory">The directory on the remote system (root '' or '/' or '\').</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void FileList(string path, string uniqueIdentifier, string token, string directory, Action<string[], object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "FILELIST" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "FILELIST" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.Path = "/" + path.Replace("/", "\\").Trim(new char[] { '\\' }) +
                        "?UniqueIdentifier=" + uniqueIdentifier +
                        "&Token=" + token +
                        "&Directory=" + (String.IsNullOrEmpty(directory) ? "" : directory) +
                        "&Member=FILELIST" +
                        "&ActionName=" + (String.IsNullOrEmpty(actionName) ? "FILELIST" : actionName);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// File details.
        /// </summary>
        /// <param name="filename">The name of the file to get details for.</param>
        /// <param name="directory">The directory on the remote system (root '' or '/' or '\').</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void FileDetails(string filename, string directory, Action<Nequeo.Model.FileDetails, object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "FILEDETAILS" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "FILEDETAILS" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.AddHeader("Member", "FILEDETAILS");
                    request.AddHeader("ActionName", (String.IsNullOrEmpty(actionName) ? "FILEDETAILS" : actionName));
                    request.AddHeader("FileName", filename);
                    request.AddHeader("Directory", String.IsNullOrEmpty(directory) ? "" : directory);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// File details from a query request.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier of the client.</param>
        /// <param name="token">The token issued to the client.</param>
        /// <param name="filename">The name of the file to get details for.</param>
        /// <param name="directory">The directory on the remote system (root '' or '/' or '\').</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void FileDetails(string uniqueIdentifier, string token, string filename, string directory, Action<Nequeo.Model.FileDetails, object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "FILEDETAILS" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "FILEDETAILS" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.Path =
                        "/?UniqueIdentifier=" + uniqueIdentifier +
                        "&Token=" + token +
                        "&FileName=" + filename +
                        "&Directory=" + (String.IsNullOrEmpty(directory) ? "" : directory) +
                        "&Member=FILEDETAILS" +
                        "&ActionName=" + (String.IsNullOrEmpty(actionName) ? "FILEDETAILS" : actionName);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// File details from a query request.
        /// </summary>
        /// <param name="path">The path url to the resource (/api/transfer.ashx).</param>
        /// <param name="uniqueIdentifier">The unique identifier of the client.</param>
        /// <param name="token">The token issued to the client.</param>
        /// <param name="filename">The name of the file to get details for.</param>
        /// <param name="directory">The directory on the remote system (root '' or '/' or '\').</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void FileDetails(string path, string uniqueIdentifier, string token, string filename, string directory, Action<Nequeo.Model.FileDetails, object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "FILEDETAILS" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "FILEDETAILS" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.Path = "/" + path.Replace("/", "\\").Trim(new char[] { '\\' }) +
                        "?UniqueIdentifier=" + uniqueIdentifier +
                        "&Token=" + token +
                        "&FileName=" + filename +
                        "&Directory=" + (String.IsNullOrEmpty(directory) ? "" : directory) +
                        "&Member=FILEDETAILS" +
                        "&ActionName=" + (String.IsNullOrEmpty(actionName) ? "FILEDETAILS" : actionName);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// Delete file.
        /// </summary>
        /// <param name="filename">The name of the file to delete.</param>
        /// <param name="directory">The directory on the remote system (root '' or '/' or '\').</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void DeleteFile(string filename, string directory, Action<bool, object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "DELETEFILE" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "DELETEFILE" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.AddHeader("Member", "DELETEFILE");
                    request.AddHeader("ActionName", (String.IsNullOrEmpty(actionName) ? "DELETEFILE" : actionName));
                    request.AddHeader("FileName", filename);
                    request.AddHeader("Directory", String.IsNullOrEmpty(directory) ? "" : directory);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// Delete file from a query request.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier of the client.</param>
        /// <param name="token">The token issued to the client.</param>
        /// <param name="filename">The name of the file to delete.</param>
        /// <param name="directory">The directory on the remote system (root '' or '/' or '\').</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void DeleteFile(string uniqueIdentifier, string token, string filename, string directory, Action<bool, object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "DELETEFILE" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "DELETEFILE" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.Path =
                        "/?UniqueIdentifier=" + uniqueIdentifier +
                        "&Token=" + token +
                        "&FileName=" + filename +
                        "&Directory=" + (String.IsNullOrEmpty(directory) ? "" : directory) +
                        "&Member=DELETEFILE" +
                        "&ActionName=" + (String.IsNullOrEmpty(actionName) ? "DELETEFILE" : actionName);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// Rename file.
        /// </summary>
        /// <param name="currentFilename">The current name of the file to rename.</param>
        /// <param name="newFilename">The new name of the file.</param>
        /// <param name="directory">The directory on the remote system (root '' or '/' or '\').</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void RenameFile(string currentFilename,string newFilename, string directory, Action<bool, object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "RENAMEFILE" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "RENAMEFILE" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.AddHeader("Member", "RENAMEFILE");
                    request.AddHeader("ActionName", (String.IsNullOrEmpty(actionName) ? "RENAMEFILE" : actionName));
                    request.AddHeader("CurrentFileName", currentFilename);
                    request.AddHeader("NewFileName", newFilename);
                    request.AddHeader("Directory", String.IsNullOrEmpty(directory) ? "" : directory);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// Rename file from a query request.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier of the client.</param>
        /// <param name="token">The token issued to the client.</param>
        /// <param name="currentFilename">The current name of the file to rename.</param>
        /// <param name="newFilename">The new name of the file.</param>
        /// <param name="directory">The directory on the remote system (root '' or '/' or '\').</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void RenameFile(string uniqueIdentifier, string token, string currentFilename, string newFilename, string directory, Action<bool, object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "RENAMEFILE" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "RENAMEFILE" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.Path =
                        "/?UniqueIdentifier=" + uniqueIdentifier +
                        "&Token=" + token +
                        "&CurrentFileName=" + currentFilename +
                        "&NewFileName=" + newFilename +
                        "&Directory=" + (String.IsNullOrEmpty(directory) ? "" : directory) +
                        "&Member=RENAMEFILE" +
                        "&ActionName=" + (String.IsNullOrEmpty(actionName) ? "RENAMEFILE" : actionName);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// Copy file.
        /// </summary>
        /// <param name="sourcePathFilename">The source path and filename.</param>
        /// <param name="destinationPathFilename">The destination path and filename.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void CopyFile(string sourcePathFilename, string destinationPathFilename, Action<bool, object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "COPYFILE" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "COPYFILE" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.AddHeader("Member", "COPYFILE");
                    request.AddHeader("ActionName", (String.IsNullOrEmpty(actionName) ? "COPYFILE" : actionName));
                    request.AddHeader("SourcePathFileName", sourcePathFilename);
                    request.AddHeader("DestinationPathFileName", destinationPathFilename);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// Copy file from a query request.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier of the client.</param>
        /// <param name="token">The token issued to the client.</param>
        /// <param name="sourcePathFilename">The source path and filename.</param>
        /// <param name="destinationPathFilename">The destination path and filename.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void CopyFile(string uniqueIdentifier, string token, string sourcePathFilename, string destinationPathFilename, Action<bool, object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "COPYFILE" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "COPYFILE" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.Path =
                        "/?UniqueIdentifier=" + uniqueIdentifier +
                        "&Token=" + token +
                        "&SourcePathFileName=" + sourcePathFilename +
                        "&DestinationPathFileName=" + destinationPathFilename +
                        "&Member=COPYFILE" +
                        "&ActionName=" + (String.IsNullOrEmpty(actionName) ? "COPYFILE" : actionName);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// Move file.
        /// </summary>
        /// <param name="sourcePathFilename">The source path and filename.</param>
        /// <param name="destinationPathFilename">The destination path and filename.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void MoveFile(string sourcePathFilename, string destinationPathFilename, Action<bool, object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "MOVEFILE" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "MOVEFILE" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.AddHeader("Member", "MOVEFILE");
                    request.AddHeader("ActionName", (String.IsNullOrEmpty(actionName) ? "MOVEFILE" : actionName));
                    request.AddHeader("SourcePathFileName", sourcePathFilename);
                    request.AddHeader("DestinationPathFileName", destinationPathFilename);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// Move file from a query request.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier of the client.</param>
        /// <param name="token">The token issued to the client.</param>
        /// <param name="sourcePathFilename">The source path and filename.</param>
        /// <param name="destinationPathFilename">The destination path and filename.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void MoveFile(string uniqueIdentifier, string token, string sourcePathFilename, string destinationPathFilename, Action<bool, object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "MOVEFILE" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "MOVEFILE" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.Path =
                        "/?UniqueIdentifier=" + uniqueIdentifier +
                        "&Token=" + token +
                        "&SourcePathFileName=" + sourcePathFilename +
                        "&DestinationPathFileName=" + destinationPathFilename +
                        "&Member=MOVEFILE" +
                        "&ActionName=" + (String.IsNullOrEmpty(actionName) ? "MOVEFILE" : actionName);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// Delete directory.
        /// </summary>
        /// <param name="directory">The full path to the directory on the remote system to delete (root '' or '/' or '\').</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void DeleteDirectory(string directory, Action<bool, object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "DELETEDIRECTORY" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "DELETEDIRECTORY" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.AddHeader("Member", "DELETEDIRECTORY");
                    request.AddHeader("ActionName", (String.IsNullOrEmpty(actionName) ? "DELETEDIRECTORY" : actionName));
                    request.AddHeader("Directory", String.IsNullOrEmpty(directory) ? "" : directory);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// Delete directory from a query request.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier of the client.</param>
        /// <param name="token">The token issued to the client.</param>
        /// <param name="directory">The full path to the directory on the remote system to delete (root '' or '/' or '\').</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void DeleteDirectory(string uniqueIdentifier, string token, string directory, Action<bool, object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "DELETEDIRECTORY" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "DELETEDIRECTORY" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.Path =
                        "/?UniqueIdentifier=" + uniqueIdentifier +
                        "&Token=" + token +
                        "&Directory=" + (String.IsNullOrEmpty(directory) ? "" : directory) +
                        "&Member=DELETEDIRECTORY" +
                        "&ActionName=" + (String.IsNullOrEmpty(actionName) ? "DELETEDIRECTORY" : actionName);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// Create directory.
        /// </summary>
        /// <param name="directory">The full path to the directory on the remote system to create (root '' or '/' or '\').</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void CreateDirectory(string directory, Action<bool, object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "CREATEDIRECTORY" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "CREATEDIRECTORY" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.AddHeader("Member", "CREATEDIRECTORY");
                    request.AddHeader("ActionName", (String.IsNullOrEmpty(actionName) ? "CREATEDIRECTORY" : actionName));
                    request.AddHeader("Directory", String.IsNullOrEmpty(directory) ? "" : directory);
                    request.WriteNetRequestHeaders();
                }
            }
        }

        /// <summary>
        /// Create directory from a query request.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier of the client.</param>
        /// <param name="token">The token issued to the client.</param>
        /// <param name="directory">The full path to the directory on the remote system to create (root '' or '/' or '\').</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void CreateDirectory(string uniqueIdentifier, string token, string directory, Action<bool, object> callback, string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Assign the call back.
                    _callback[(String.IsNullOrEmpty(actionName) ? "CREATEDIRECTORY" : actionName)] = callback;
                    _state[(String.IsNullOrEmpty(actionName) ? "CREATEDIRECTORY" : actionName)] = state;

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Method = "GET";
                    request.ContentLength = 0;
                    request.KeepAlive = true;
                    request.Path =
                        "/?UniqueIdentifier=" + uniqueIdentifier +
                        "&Token=" + token +
                        "&Directory=" + (String.IsNullOrEmpty(directory) ? "" : directory) +
                        "&Member=CREATEDIRECTORY" +
                        "&ActionName=" + (String.IsNullOrEmpty(actionName) ? "CREATEDIRECTORY" : actionName);
                    request.WriteNetRequestHeaders();
                }
            }
        }
        #endregion

        #region Private Receive Methods
        /// <summary>
        /// On net context receive handler.
        /// </summary>
        /// <param name="sender">The application sender.</param>
        /// <param name="context">The current net context.</param>
        private void ManagerClient_OnNetContext(object sender, Net.NetContext context)
        {
            Net.NetRequest request = null;
            Net.NetResponse response = null;
            bool keepAlive = true;
            bool isError = true;

            try
            {
                string resource = "";
                string executionMember = "";
                string statusCode = "";
                string statusDescription = "";

                request = context.NetRequest;
                response = context.NetResponse;

                // Get the response headers, and set the response headers.
                List<NameValue> headers = base.ParseHeaders(response.Input, out resource, base.HeaderTimeout, base.MaximumReadLength);
                if (headers != null)
                {
                    // Set the response headers.
                    response.ReadNetResponseHeaders(headers, resource);

                    // Should the connection be kept alive.
                    keepAlive = response.KeepAlive;
                    statusCode = response.StatusCode.ToString();
                    statusDescription = response.StatusDescription;

                    // Get the execution member.
                    if (!String.IsNullOrEmpty(response.Headers["Member"]))
                    {
                        // Get the execution member.
                        executionMember = response.Headers["Member"].Trim();
                        switch (executionMember.ToUpper())
                        {
                            case "VALIDATE":
                                // Validate
                                isError = Validate(response);
                                break;

                            case "DOWNLOADFILE":
                                // DownloadFile
                                isError = DownloadFile(response);
                                break;

                            case "UPLOADFILE":
                                // UploadFile
                                isError = UploadFile(response);
                                break;

                            case "STOPDOWNLOADINGFILE":
                                // StopDownloadingFile
                                isError = StopDownloadingFile(response);
                                break;

                            case "FILELIST":
                                // File directory list.
                                isError = FileList(response);
                                break;

                            case "DELETEFILE":
                                // Delete file.
                                isError = DeleteFile(response);
                                break;

                            case "RENAMEFILE":
                                // Rename file.
                                isError = RenameFile(response);
                                break;

                            case "COPYFILE":
                                // Copy file.
                                isError = CopyFile(response);
                                break;

                            case "MOVEFILE":
                                // Move file.
                                isError = MoveFile(response);
                                break;

                            case "FILEDETAILS":
                                // File details.
                                isError = FileDetails(response);
                                break;

                            case "DELETEDIRECTORY":
                                // Delete directory.
                                isError = DeleteDirectory(response);
                                break;

                            case "CREATEDIRECTORY":
                                // Create directory.
                                isError = CreateDirectory(response);
                                break;

                            default:
                                keepAlive = false;
                                throw new Exception("Command not recognised.");
                        }
                    }
                }
                else
                {
                    // No headers have been found.
                    keepAlive = false;
                    throw new Exception("No headers have been found.");
                }

                // If error.
                if (isError)
                    // An internal client error.
                    AnyError(executionMember, statusCode, statusDescription);
            }
            catch (Exception ex)
            {
                try
                {
                    // An internal client error.
                    AnyError("Error", "500", ex.Message);
                }
                catch { }
            }
        }
        #endregion

        #region Private Action Members
        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="response">The current response stream.</param>
        /// <returns>True if error; else false.</returns>
        private bool Validate(Net.NetResponse response)
        {
            bool isError = true;
            bool result = false;
            string actionName = "";

            try
            {
                actionName = response.Headers["ActionName"];

                // Get the execution member result.
                if (!String.IsNullOrEmpty(response.Headers["MemberResult"]))
                {
                    // Get the data.
                    result = Boolean.Parse(response.Headers["MemberResult"]);
                    isError = false;
                }
            }
            catch { isError = true; }

            // Call the handler.
            object callback = null;
            object state = null;
            if (_callback.TryGetValue(actionName, out callback))
            {
                _state.TryGetValue(actionName, out state);
                if (callback != null)
                {
                    Action<bool, object> callbackAction = (Action<bool, object>)callback;
                    callbackAction(result, state);
                }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// DownloadFile
        /// </summary>
        /// <param name="response">The current response stream.</param>
        /// <returns>True if error; else false.</returns>
        private bool DownloadFile(Net.NetResponse response)
        {
            bool isError = true;
            bool result = false;
            string actionName = "";

            try
            {
                actionName = response.Headers["ActionName"];

                // Get the execution member result.
                if (!String.IsNullOrEmpty(response.Headers["MemberResult"]))
                {
                    // Get the data.
                    result = Boolean.Parse(response.Headers["MemberResult"]);
                    isError = false;
                }
            }
            catch { isError = true; }

            // Call the handler.
            object callback = null;
            object state = null;
            if (_callback.TryGetValue(actionName, out callback))
            {
                _state.TryGetValue(actionName, out state);
                if (callback != null)
                {
                    Action<Nequeo.Net.NetResponse, object> callbackAction = (Action<Nequeo.Net.NetResponse, object>)callback;

                    // If the result is true.
                    if(result)
                        callbackAction(response, state);
                    else
                        callbackAction(null, state);
                }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// UploadFile
        /// </summary>
        /// <param name="response">The current response stream.</param>
        /// <returns>True if error; else false.</returns>
        private bool UploadFile(Net.NetResponse response)
        {
            bool isError = true;
            bool result = false;
            string actionName = "";

            try
            {
                actionName = response.Headers["ActionName"];

                // Get the execution member result.
                if (!String.IsNullOrEmpty(response.Headers["MemberResult"]))
                {
                    // Get the data.
                    result = Boolean.Parse(response.Headers["MemberResult"]);
                    isError = false;
                }
            }
            catch { isError = true; }

            // Call the handler.
            object callback = null;
            object state = null;
            if (_callback.TryGetValue(actionName, out callback))
            {
                _state.TryGetValue(actionName, out state);
                if (callback != null)
                {
                    Action<bool, object> callbackAction = (Action<bool, object>)callback;
                    callbackAction(result, state);
                }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// StopDownloadingFile
        /// </summary>
        /// <param name="response">The current response stream.</param>
        /// <returns>True if error; else false.</returns>
        private bool StopDownloadingFile(Net.NetResponse response)
        {
            bool isError = true;
            bool result = true;
            string actionName = "";

            try
            {
                actionName = response.Headers["ActionName"];

                // Get the execution member result.
                if (!String.IsNullOrEmpty(response.Headers["MemberResult"]))
                {
                    // Get the data.
                    result = Boolean.Parse(response.Headers["MemberResult"]);
                    isError = false;
                }
            }
            catch { isError = true; }

            // Call the handler.
            object callback = null;
            object state = null;
            if (_callback.TryGetValue(actionName, out callback))
            {
                _state.TryGetValue(actionName, out state);
                if (callback != null)
                {
                    Action<bool, object> callbackAction = (Action<bool, object>)callback;
                    callbackAction(result, state);
                }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// FileList
        /// </summary>
        /// <param name="response">The current response stream.</param>
        /// <returns>True if error; else false.</returns>
        private bool FileList(Net.NetResponse response)
        {
            bool isError = true;
            string[] result = null;
            string actionName = "";
            bool ret = false;

            MemoryStream memory = null;
            try
            {
                actionName = response.Headers["ActionName"];

                // Get the execution member result.
                if (!String.IsNullOrEmpty(response.Headers["MemberResult"]))
                {
                    // Get the data.
                    ret = Boolean.Parse(response.Headers["MemberResult"]);
                    isError = false;
                }

                // Create the store stream.
                memory = new MemoryStream();

                // Read the data from the response.
                bool copied = Nequeo.IO.Stream.Operation.CopyStream(response.Input, memory, response.ContentLength, base.ResponseTimeout, base.ReadBufferSize);
                if (copied)
                {
                    // Extract the data.
                    string data = Encoding.Default.GetString(memory.ToArray());
                    result = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                }
            }
            catch { isError = true; }
            finally
            {
                if (memory != null)
                    memory.Dispose();
            }

            // Call the handler.
            object callback = null;
            object state = null;
            if (_callback.TryGetValue(actionName, out callback))
            {
                _state.TryGetValue(actionName, out state);
                if (callback != null)
                {
                    Action<string[], object> callbackAction = (Action<string[], object>)callback;
                    callbackAction(result, state);
                }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// DeleteFile
        /// </summary>
        /// <param name="response">The current response stream.</param>
        /// <returns>True if error; else false.</returns>
        private bool DeleteFile(Net.NetResponse response)
        {
            bool isError = true;
            bool result = false;
            string actionName = "";

            try
            {
                actionName = response.Headers["ActionName"];

                // Get the execution member result.
                if (!String.IsNullOrEmpty(response.Headers["MemberResult"]))
                {
                    // Get the data.
                    result = Boolean.Parse(response.Headers["MemberResult"]);
                    isError = false;
                }
            }
            catch { isError = true; }

            // Call the handler.
            object callback = null;
            object state = null;
            if (_callback.TryGetValue(actionName, out callback))
            {
                _state.TryGetValue(actionName, out state);
                if (callback != null)
                {
                    Action<bool, object> callbackAction = (Action<bool, object>)callback;
                    callbackAction(result, state);
                }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// RenameFile
        /// </summary>
        /// <param name="response">The current response stream.</param>
        /// <returns>True if error; else false.</returns>
        private bool RenameFile(Net.NetResponse response)
        {
            bool isError = true;
            bool result = false;
            string actionName = "";

            try
            {
                actionName = response.Headers["ActionName"];

                // Get the execution member result.
                if (!String.IsNullOrEmpty(response.Headers["MemberResult"]))
                {
                    // Get the data.
                    result = Boolean.Parse(response.Headers["MemberResult"]);
                    isError = false;
                }
            }
            catch { isError = true; }

            // Call the handler.
            object callback = null;
            object state = null;
            if (_callback.TryGetValue(actionName, out callback))
            {
                _state.TryGetValue(actionName, out state);
                if (callback != null)
                {
                    Action<bool, object> callbackAction = (Action<bool, object>)callback;
                    callbackAction(result, state);
                }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// CopyFile
        /// </summary>
        /// <param name="response">The current response stream.</param>
        /// <returns>True if error; else false.</returns>
        private bool CopyFile(Net.NetResponse response)
        {
            bool isError = true;
            bool result = false;
            string actionName = "";

            try
            {
                actionName = response.Headers["ActionName"];

                // Get the execution member result.
                if (!String.IsNullOrEmpty(response.Headers["MemberResult"]))
                {
                    // Get the data.
                    result = Boolean.Parse(response.Headers["MemberResult"]);
                    isError = false;
                }
            }
            catch { isError = true; }

            // Call the handler.
            object callback = null;
            object state = null;
            if (_callback.TryGetValue(actionName, out callback))
            {
                _state.TryGetValue(actionName, out state);
                if (callback != null)
                {
                    Action<bool, object> callbackAction = (Action<bool, object>)callback;
                    callbackAction(result, state);
                }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// MoveFile
        /// </summary>
        /// <param name="response">The current response stream.</param>
        /// <returns>True if error; else false.</returns>
        private bool MoveFile(Net.NetResponse response)
        {
            bool isError = true;
            bool result = false;
            string actionName = "";

            try
            {
                actionName = response.Headers["ActionName"];

                // Get the execution member result.
                if (!String.IsNullOrEmpty(response.Headers["MemberResult"]))
                {
                    // Get the data.
                    result = Boolean.Parse(response.Headers["MemberResult"]);
                    isError = false;
                }
            }
            catch { isError = true; }

            // Call the handler.
            object callback = null;
            object state = null;
            if (_callback.TryGetValue(actionName, out callback))
            {
                _state.TryGetValue(actionName, out state);
                if (callback != null)
                {
                    Action<bool, object> callbackAction = (Action<bool, object>)callback;
                    callbackAction(result, state);
                }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// FileDetails
        /// </summary>
        /// <param name="response">The current response stream.</param>
        /// <returns>True if error; else false.</returns>
        private bool FileDetails(Net.NetResponse response)
        {
            bool isError = true;
            bool result = false;
            Nequeo.Model.FileDetails fileDetails = null;
            string actionName = "";

            MemoryStream memory = null;
            try
            {
                actionName = response.Headers["ActionName"];

                // Get the execution member result.
                if (!String.IsNullOrEmpty(response.Headers["MemberResult"]))
                {
                    // Get the data.
                    result = Boolean.Parse(response.Headers["MemberResult"]);
                    isError = false;
                }

                // Create the store stream.
                memory = new MemoryStream();

                // Read the data from the response.
                bool copied = Nequeo.IO.Stream.Operation.CopyStream(response.Input, memory, response.ContentLength, base.ResponseTimeout, base.ReadBufferSize);
                if (copied)
                {
                    // Extract the data.
                    string data = Encoding.Default.GetString(memory.ToArray());
                    string[] details = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                    // If details exist.
                    if (details != null && details.Length > 0)
                    {
                        // Assign the details.
                        fileDetails = new Model.FileDetails();
                        fileDetails.Attributes = (FileAttributes)Enum.Parse(typeof(FileAttributes), details[0]);
                        fileDetails.CreationTime = DateTime.Parse(details[1]);
                        fileDetails.CreationTimeUtc = DateTime.Parse(details[2]);
                        fileDetails.Directory = details[3];
                        fileDetails.DirectoryName = details[4];
                        fileDetails.Exists = Boolean.Parse(details[5]);
                        fileDetails.Extension = details[6];
                        fileDetails.FullName = details[7];
                        fileDetails.IsReadOnly = Boolean.Parse(details[8]);
                        fileDetails.LastAccessTime = DateTime.Parse(details[9]);
                        fileDetails.LastAccessTimeUtc = DateTime.Parse(details[10]);
                        fileDetails.LastWriteTime = DateTime.Parse(details[11]);
                        fileDetails.LastWriteTimeUtc = DateTime.Parse(details[12]);
                        fileDetails.Length = Int64.Parse(details[13]);
                        fileDetails.Name = details[14];
                    }
                }
            }
            catch { isError = true; }
            finally
            {
                if (memory != null)
                    memory.Dispose();
            }

            // Call the handler.
            object callback = null;
            object state = null;
            if (_callback.TryGetValue(actionName, out callback))
            {
                _state.TryGetValue(actionName, out state);
                if (callback != null)
                {
                    Action<Nequeo.Model.FileDetails, object> callbackAction = (Action<Nequeo.Model.FileDetails, object>)callback;
                    callbackAction(fileDetails, state);
                }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// DeleteDirectory
        /// </summary>
        /// <param name="response">The current response stream.</param>
        /// <returns>True if error; else false.</returns>
        private bool DeleteDirectory(Net.NetResponse response)
        {
            bool isError = true;
            bool result = false;
            string actionName = "";

            try
            {
                actionName = response.Headers["ActionName"];

                // Get the execution member result.
                if (!String.IsNullOrEmpty(response.Headers["MemberResult"]))
                {
                    // Get the data.
                    result = Boolean.Parse(response.Headers["MemberResult"]);
                    isError = false;
                }
            }
            catch { isError = true; }

            // Call the handler.
            object callback = null;
            object state = null;
            if (_callback.TryGetValue(actionName, out callback))
            {
                _state.TryGetValue(actionName, out state);
                if (callback != null)
                {
                    Action<bool, object> callbackAction = (Action<bool, object>)callback;
                    callbackAction(result, state);
                }
            }

            // Return the result.
            return isError;
        }

        /// <summary>
        /// CreateDirectory
        /// </summary>
        /// <param name="response">The current response stream.</param>
        /// <returns>True if error; else false.</returns>
        private bool CreateDirectory(Net.NetResponse response)
        {
            bool isError = true;
            bool result = false;
            string actionName = "";

            try
            {
                actionName = response.Headers["ActionName"];

                // Get the execution member result.
                if (!String.IsNullOrEmpty(response.Headers["MemberResult"]))
                {
                    // Get the data.
                    result = Boolean.Parse(response.Headers["MemberResult"]);
                    isError = false;
                }
            }
            catch { isError = true; }

            // Call the handler.
            object callback = null;
            object state = null;
            if (_callback.TryGetValue(actionName, out callback))
            {
                _state.TryGetValue(actionName, out state);
                if (callback != null)
                {
                    Action<bool, object> callbackAction = (Action<bool, object>)callback;
                    callbackAction(result, state);
                }
            }

            // Return the result.
            return isError;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Send to the client any type of error.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="code">The code.</param>
        /// <param name="message">The message</param>
        private void AnyError(string command, string code, string message)
        {
            // Send any type of error.
            if (OnError != null)
                OnError(this, command, code, message);
        }

        /// <summary>
        /// Initialise.
        /// </summary>
        private void Initialise()
        {
            // Assign the on connect action handler.
            base.Timeout = 60;
            base.HeaderTimeout = 30000;
            base.RequestTimeout = 30000;
            base.ResponseTimeout = 30000;
            base.RemoteHostPrefix = "DownloadManagerClient_";
            base.OnNetContext += ManagerClient_OnNetContext;
            base.OnDisconnected += new EventHandler(Client_OnDisconnected);
            base.OnTimedOut += new EventHandler(Client_OnDisconnected);
            base.OnInternalError += new Threading.EventHandler<Exception, string>(Client_OnInternalError);
        }

        /// <summary>
        /// An internal error
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="ex">The exception that occured.</param>
        /// <param name="method">The method name the exception occured in.</param>
        private void Client_OnInternalError(object sender, Exception ex, string method)
        {
            // Attempt to reconnect to the server.
            ReConnect();
        }

        /// <summary>
        /// Client has been disconnected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Client_OnDisconnected(object sender, EventArgs e)
        {
            // Attempt to reconnect to the server.
            ReConnect();
        }

        /// <summary>
        /// Attempt to reconnect to the server.
        /// </summary>
        private void ReConnect()
        {
            lock (_lockConnectObject)
            {
                // If no longer connected.
                if (!base.Connected)
                {
                    // Close the connection.
                    base.Close();

                    try
                    {
                        // Attempt to reconnect.
                        if (_reconnectWhenNoConnection)
                        {
                            // Attempt to reconnect.
                            Initialisation();
                            Connect();
                        }
                    }
                    catch { }
                }
            }
        }
        #endregion

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.  If disposing
        /// equals true, the method has been called directly or indirectly by a user's
        /// code. Managed and unmanaged resources can be disposed.  If disposing equals
        /// false, the method has been called by the runtime from inside the finalizer
        /// and you should not reference other objects. Only unmanaged resources can
        /// be disposed.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_callback != null)
                        _callback.Clear();

                    if (_state != null)
                        _state.Clear();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _lockConnectObject = null;
                _lockRequestObject = null;

                _callback = null;
                _state = null;
            }
        }
        #endregion
    }
}
