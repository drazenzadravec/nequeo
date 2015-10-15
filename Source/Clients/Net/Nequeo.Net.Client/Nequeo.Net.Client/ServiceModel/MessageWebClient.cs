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
using System.IO;

using Nequeo.IO.Stream.Extension;

namespace Nequeo.Net.ServiceModel
{
    /// <summary>
    /// Generic HTTP web message client
    /// </summary>
    public class MessageWebClient
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="connection">The web connection adapter used for http connection.</param>
        public MessageWebClient(Nequeo.Net.Http.WebConnectionAdapter connection)
        {
            _connection = connection;

            // Start the async control.
            _asyncAccount = new Nequeo.Threading.AsyncExecutionHandler<MessageWebClient>();
            _asyncAccount.AsyncError += new Threading.EventHandler<Exception>(_asyncAccount_AsyncError);
            _asyncAccount.AsyncComplete += new Threading.EventHandler<object, string>(_asyncAccount_AsyncComplete);
            _asyncAccount.InitiliseAsyncInstance(this);
        }

        private Nequeo.Threading.AsyncExecutionHandler<MessageWebClient> _asyncAccount = null;
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
        /// Send and recieve string message.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="message">The received message</param>
        /// <returns>The response message</returns>
        public string MessageString(string requestUri, string message)
        {
            try
            {
                Nequeo.Net.Http.HttpClient http = new Http.HttpClient();
                http.Connection = _connection;
                return http.UploadString(requestUri, message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Send and recieve string message.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="message">The received message</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        /// <returns>The response message</returns>
        public void MessageString(string requestUri, string message, Action<Nequeo.Threading.AsyncOperationResult<string>> callback, object state = null)
        {
            string keyName = "MessageString";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<string>(u => u.MessageString(requestUri, message), keyName);
        }

        /// <summary>
        /// Send and recieve byte message.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="message">The received message</param>
        /// <returns>The response message</returns>
        public byte[] MessageByte(string requestUri, byte[] message)
        {
            try
            {
                Nequeo.Net.Http.HttpClient http = new Http.HttpClient();
                http.Connection = _connection;
                return Encoding.Default.GetBytes(http.UploadString(requestUri, Encoding.Default.GetString(message)));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Send and recieve byte message.
        /// </summary>
        /// <param name="requestUri">The request uri to upload to.</param>
        /// <param name="message">The received message</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        /// <returns>The response message</returns>
        public void MessageByte(string requestUri, byte[] message, Action<Nequeo.Threading.AsyncOperationResult<byte[]>> callback, object state = null)
        {
            string keyName = "MessageByte";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<byte[]>(u => u.MessageByte(requestUri, message), keyName);
        }

        /// <summary>
        /// Async complete action handler
        /// </summary>
        /// <param name="sender">The current object handler</param>
        /// <param name="e1">The action execution result</param>
        /// <param name="e2">The unique action name.</param>
        private void _asyncAccount_AsyncComplete(object sender, object e1, string e2)
        {
            switch (e2)
            {
                case "MessageString":
                    Action<Nequeo.Threading.AsyncOperationResult<string>> callbackMessageString = (Action<Nequeo.Threading.AsyncOperationResult<string>>)_callback[e2];
                    callbackMessageString(new Nequeo.Threading.AsyncOperationResult<string>(((string)e1), _state[e2], e2));
                    break;

                case "MessageByte":
                    Action<Nequeo.Threading.AsyncOperationResult<byte[]>> callbackMessageByte = (Action<Nequeo.Threading.AsyncOperationResult<byte[]>>)_callback[e2];
                    callbackMessageByte(new Nequeo.Threading.AsyncOperationResult<byte[]>(((byte[])e1), _state[e2], e2));
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
