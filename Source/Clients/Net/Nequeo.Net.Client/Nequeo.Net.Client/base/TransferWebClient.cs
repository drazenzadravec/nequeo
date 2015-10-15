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
using System.Text;

namespace Nequeo.Net
{
    /// <summary>
    /// Generic HTTP web transfer client
    /// </summary>
    public class TransferWebClient : Nequeo.Net.Http.IHttpWebClient
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="connection">The web connection adapter used for http connection.</param>
        public TransferWebClient(Nequeo.Net.Http.WebConnectionAdapter connection)
        {
            _connection = connection;

            // Start the async control.
            _asyncAccount = new Nequeo.Threading.AsyncExecutionHandler<TransferWebClient>();
            _asyncAccount.AsyncError += new Threading.EventHandler<Exception>(_asyncAccount_AsyncError);
            _asyncAccount.AsyncComplete += new Threading.EventHandler<object, string>(_asyncAccount_AsyncComplete);
            _asyncAccount.InitiliseAsyncInstance(this);
        }

        private Nequeo.Threading.AsyncExecutionHandler<TransferWebClient> _asyncAccount = null;
        private Nequeo.Net.Http.WebConnectionAdapter _connection = null;
        private Exception _exception = null;

        private Dictionary<object, object> _callback = new Dictionary<object, object>();
        private Dictionary<object, object> _state = new Dictionary<object, object>();

        /// <summary>
        /// Gets the current async exception; else null;
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }

        /// <summary>
        /// Downloads a request uri to a local file.
        /// </summary>
        /// <param name="requestUri">The request uri to download.</param>
        /// <param name="destinationFile">The local destination file to copy the data to.</param>
        public void DownloadFile(string requestUri, string destinationFile)
        {
            Nequeo.Net.Http.HttpClient http = new Http.HttpClient();
            http.Connection = _connection;
            http.DownloadFile(requestUri, destinationFile);
        }

        /// <summary>
        /// Downloads a request uri to a local file.
        /// </summary>
        /// <param name="requestUri">The request uri to download.</param>
        /// <param name="destinationFile">The local destination file to copy the data to.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public void DownloadFile(string requestUri, string destinationFile, Action<Nequeo.Threading.AsyncOperationResult<bool>> callback, object state = null)
        {
            string keyName = "DownloadFile";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute(u => u.DownloadFile(requestUri, destinationFile), keyName);
        }

        /// <summary>
        /// Downloads data from the request uri.
        /// </summary>
        /// <param name="requestUri">The request uri to download.</param>
        /// <returns>The returned request uri content.</returns>
        public string DownloadString(string requestUri)
        {
            Nequeo.Net.Http.HttpClient http = new Http.HttpClient();
            http.Connection = _connection;
            return http.DownloadString(requestUri);
        }

        /// <summary>
        /// Downloads data from the request uri.
        /// </summary>
        /// <param name="requestUri">The request uri to download.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public void DownloadString(string requestUri, Action<Nequeo.Threading.AsyncOperationResult<string>> callback, object state = null)
        {
            string keyName = "DownloadString";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<string>(u => u.DownloadString(requestUri), keyName);
        }

        /// <summary>
        /// Upload a local file to the request uri.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="sourceFile">The local source file to upload data from.</param>
        /// <returns>A System.Byte array containing the body of the response from the resource.</returns>
        public byte[] UploadFile(string requestUri, string sourceFile)
        {
            Nequeo.Net.Http.HttpClient http = new Http.HttpClient();
            http.Connection = _connection;
            return http.UploadFile(requestUri, sourceFile);
        }

        /// <summary>
        /// Upload a local file to the request uri.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="sourceFile">The local source file to upload data from.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public void UploadFile(string requestUri, string sourceFile, Action<Nequeo.Threading.AsyncOperationResult<byte[]>> callback, object state = null)
        {
            string keyName = "UploadFile";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<byte[]>(u => u.UploadFile(requestUri, sourceFile), keyName);
        }

        /// <summary>
        /// Upload a local file to the request uri.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="sourceFile">The local source file to upload data from.</param>
        /// <param name="method">The HTTP method used to send the file to the resource. If null, the default is POST for http.</param>
        /// <returns>A System.Byte array containing the body of the response from the resource.</returns>
        public byte[] UploadFile(string requestUri, string sourceFile, string method)
        {
            Nequeo.Net.Http.HttpClient http = new Http.HttpClient();
            http.Connection = _connection;
            return http.UploadFile(requestUri, sourceFile, method);
        }

        /// <summary>
        /// Upload a local file to the request uri.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="sourceFile">The local source file to upload data from.</param>
        /// <param name="method">The HTTP method used to send the file to the resource. If null, the default is POST for http.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public void UploadFile(string requestUri, string sourceFile, string method, Action<Nequeo.Threading.AsyncOperationResult<byte[]>> callback, object state = null)
        {
            string keyName = "UploadFileEx";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<byte[]>(u => u.UploadFile(requestUri, sourceFile, method), keyName);
        }

        /// <summary>
        /// Uploads data to the request uri.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="data">The data to upload.</param>
        /// <returns>A System.String containing the response sent by the server.</returns>
        public string UploadString(string requestUri, string data)
        {
            Nequeo.Net.Http.HttpClient http = new Http.HttpClient();
            http.Connection = _connection;
            return http.UploadString(requestUri, data);
        }

        /// <summary>
        /// Uploads data to the request uri.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="data">The data to upload.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public void UploadString(string requestUri, string data, Action<Nequeo.Threading.AsyncOperationResult<string>> callback, object state = null)
        {
            string keyName = "UploadString";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<string>(u => u.UploadString(requestUri, data), keyName);
        }

        /// <summary>
        /// Uploads data to the request uri.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="data">The data to upload.</param>
        /// <param name="method">The HTTP method used to send the file to the resource. If null, the default is POST for http.</param>
        /// <returns>A System.String containing the response sent by the server.</returns>
        public string UploadString(string requestUri, string data, string method)
        {
            Nequeo.Net.Http.HttpClient http = new Http.HttpClient();
            http.Connection = _connection;
            return http.UploadString(requestUri, data, method);
        }

        /// <summary>
        /// Uploads data to the request uri.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="data">The data to upload.</param>
        /// <param name="method">The HTTP method used to send the file to the resource. If null, the default is POST for http.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public void UploadString(string requestUri, string data, string method, Action<Nequeo.Threading.AsyncOperationResult<string>> callback, object state = null)
        {
            string keyName = "UploadStringEx";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<string>(u => u.UploadString(requestUri, data, method), keyName);
        }

        /// <summary>
        /// Async complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        private void _asyncAccount_AsyncComplete(object sender, object e1, string e2)
        {
            switch (e2)
            {
                case "DownloadFile":
                    Action<Nequeo.Threading.AsyncOperationResult<bool>> callbackDownloadFile = (Action<Nequeo.Threading.AsyncOperationResult<bool>>)_callback[e2];
                    callbackDownloadFile(new Nequeo.Threading.AsyncOperationResult<bool>(true, _state[e2], e2));
                    break;
                case "DownloadString":
                    Action<Nequeo.Threading.AsyncOperationResult<string>> callbackDownloadString = (Action<Nequeo.Threading.AsyncOperationResult<string>>)_callback[e2];
                    callbackDownloadString(new Nequeo.Threading.AsyncOperationResult<string>(e1.ToString(), _state[e2], e2));
                    break;
                case "UploadFile":
                    Action<Nequeo.Threading.AsyncOperationResult<byte[]>> callbackUploadFile = (Action<Nequeo.Threading.AsyncOperationResult<byte[]>>)_callback[e2];
                    callbackUploadFile(new Nequeo.Threading.AsyncOperationResult<byte[]>(((byte[])e1), _state[e2], e2));
                    break;
                case "UploadString":
                    Action<Nequeo.Threading.AsyncOperationResult<string>> callbackUploadString = (Action<Nequeo.Threading.AsyncOperationResult<string>>)_callback[e2];
                    callbackUploadString(new Nequeo.Threading.AsyncOperationResult<string>(e1.ToString(), _state[e2], e2));
                    break;
                case "UploadFileEx":
                    Action<Nequeo.Threading.AsyncOperationResult<byte[]>> callbackUploadFileEx = (Action<Nequeo.Threading.AsyncOperationResult<byte[]>>)_callback[e2];
                    callbackUploadFileEx(new Nequeo.Threading.AsyncOperationResult<byte[]>(((byte[])e1), _state[e2], e2));
                    break;
                case "UploadStringEx":
                    Action<Nequeo.Threading.AsyncOperationResult<string>> callbackUploadStringEx = (Action<Nequeo.Threading.AsyncOperationResult<string>>)_callback[e2];
                    callbackUploadStringEx(new Nequeo.Threading.AsyncOperationResult<string>(e1.ToString(), _state[e2], e2));
                    break;
                default:
                    _exception = new Exception("The async operation is not supported.");
                    break;
            }
        }

        /// <summary>
        /// Async error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e1"></param>
        private void _asyncAccount_AsyncError(object sender, Exception e1)
        {
            _exception = e1;
        }
    }
}
