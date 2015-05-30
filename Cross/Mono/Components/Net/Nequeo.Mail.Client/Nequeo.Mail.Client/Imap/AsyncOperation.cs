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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nequeo.Net.Mail.Imap
{
    /// <summary>
    /// Class for asynchronous email message operations.
    /// </summary>
    internal class AsyncGetAccountFolders : Nequeo.Threading.AsynchronousResult<List<string>>
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous email message operation.
        /// </summary>
        /// <param name="client">The current imap4 client.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncGetAccountFolders(Imap4Client client,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _client = client;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncGetAccountFoldersThread1));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous email message operation.
        /// </summary>
        /// <param name="subscribed">The subscribed folder list.</param>
        /// <param name="client">The current imap4 client.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncGetAccountFolders(bool subscribed, Imap4Client client,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _client = client;
            _subscribed = subscribed;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncGetAccountSubFoldersThread2));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous email message operation.
        /// </summary>
        /// <param name="folderName">The folder to view details on.</param>
        /// <param name="client">The current imap4 client.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncGetAccountFolders(string folderName, Imap4Client client,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _client = client;
            _folderName = folderName;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncGetAccountFolderDetailsThread3));
            Thread.Sleep(20);
        }

        private string _folderName = string.Empty;
        private bool _subscribed = false;
        private Imap4Client _client = null;

        /// <summary>
        /// The async email folders method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncGetAccountFoldersThread1(Object stateInfo)
        {
            // Get the folders.
            List<string> data = _client.GetFolders();

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }

        /// <summary>
        /// The async email sub folders method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncGetAccountSubFoldersThread2(Object stateInfo)
        {
            // Get the sub folders.
            List<string> data = _client.GetSubscribedFolders();

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }

        /// <summary>
        /// The async email folder details method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncGetAccountFolderDetailsThread3(Object stateInfo)
        {
            // Get the sub folders.
            List<string> data = _client.SetFolder(_folderName);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }
        #endregion
    }

    /// <summary>
    /// Class for asynchronous email message operations.
    /// </summary>
    internal class AsyncGetMessageSize : Nequeo.Threading.AsynchronousResult<List<string>>
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous email message operation.
        /// </summary>
        /// <param name="startMessageNumber">The start message number.</param>
        /// <param name="endMessageNumber">The end message number.</param>
        /// <param name="client">The current imap4 client.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncGetMessageSize(long startMessageNumber, long endMessageNumber,
            Imap4Client client, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _client = client;
            _startMessageNumber = startMessageNumber;
            _endMessageNumber = endMessageNumber;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncGetMessageSizeThread1));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous email message operation.
        /// </summary>
        /// <param name="client">The current imap4 client.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncGetMessageSize(Imap4Client client, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _client = client;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncGetMessageSizeThread2));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous email message operation.
        /// </summary>
        /// <param name="messageNumber">The message to get information on.</param>
        /// <param name="client">The current imap4 client.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncGetMessageSize(long messageNumber, Imap4Client client, 
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _client = client;
            _messageNumber = messageNumber;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncGetMessageSizeThread3));
            Thread.Sleep(20);
        }

        private long _startMessageNumber = 0;
        private long _endMessageNumber = 0;
        private long _messageNumber = 0;
        private Imap4Client _client = null;

        /// <summary>
        /// The async email size method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncGetMessageSizeThread1(Object stateInfo)
        {
            // Get the size.
            List<string> data = _client.GetSizeOfMessages(
                _startMessageNumber, _endMessageNumber);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }

        /// <summary>
        /// The async email size method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncGetMessageSizeThread2(Object stateInfo)
        {
            // Get the size.
            List<string> data = _client.GetSizeOfMessages();

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }

        /// <summary>
        /// The async email size method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncGetMessageSizeThread3(Object stateInfo)
        {
            // Get the size.
            List<string> data = _client.GetMessageSize(_messageNumber);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }
        #endregion
    }

    /// <summary>
    /// Class for asynchronous email message operations.
    /// </summary>
    internal class AsyncGetEmailMessage : Nequeo.Threading.AsynchronousResult<bool>
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous email message operation.
        /// </summary>
        /// <param name="messageNumber">The email message number.</param>
        /// <param name="client">The current imap4 client.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncGetEmailMessage(long messageNumber, Imap4Client client,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _client = client;
            _messageNumber = messageNumber;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncGetEmailMessageThread1));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous email message operation.
        /// </summary>
        /// <param name="messageNumber">The email message number.</param>
        /// <param name="headerLineCount">The number of message lines to get.</param>
        /// <param name="client">The current imap4 client.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncGetEmailMessage(long messageNumber, long headerLineCount, Imap4Client client,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _client = client;
            _messageNumber = messageNumber;
            _headerLineCount = headerLineCount;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncGetEmailMessageThread2));
            Thread.Sleep(20);
        }

        private long _messageNumber = 0;
        private Imap4Client _client = null;
        private long _headerLineCount = 0;

        /// <summary>
        /// The async email messsage method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncGetEmailMessageThread1(Object stateInfo)
        {
            // Get the email messasge.
            bool data = _client.GetEmail(_messageNumber);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data)
                base.Complete(data, true);
            else
                base.Complete(false);
        }

        /// <summary>
        /// The async email messsage header method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncGetEmailMessageThread2(Object stateInfo)
        {
            // Get the email messasge header.
            bool data = _client.GetEmailHeaders(_messageNumber);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data)
                base.Complete(data, true);
            else
                base.Complete(false);
        }
        #endregion
    }

    /// <summary>
    /// Class for asynchronous email message operations.
    /// </summary>
    internal class AsyncGetEmailBody : Nequeo.Threading.AsynchronousResult<string>
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous email message operation.
        /// </summary>
        /// <param name="messageNumber">The email message number.</param>
        /// <param name="client">The current imap4 client.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncGetEmailBody(long messageNumber, Imap4Client client,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _client = client;
            _messageNumber = messageNumber;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncGetEmailBodyThread1));
            Thread.Sleep(20);
        }

        private long _messageNumber = 0;
        private Imap4Client _client = null;

        /// <summary>
        /// The async email messsage method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncGetEmailBodyThread1(Object stateInfo)
        {
            // Get the email messasge.
            string data = _client.GetEmailBody(_messageNumber);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }
        #endregion
    }
}
