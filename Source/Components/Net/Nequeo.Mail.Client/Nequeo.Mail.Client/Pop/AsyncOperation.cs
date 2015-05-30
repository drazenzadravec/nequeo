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

namespace Nequeo.Net.Mail.Pop
{
    /// <summary>
    /// Class for asynchronous email message operations.
    /// </summary>
    internal class AsyncGetEmailMessage : Nequeo.Threading.AsynchronousResult<bool>
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous email message operation.
        /// </summary>
        /// <param name="position">The email message position.</param>
        /// <param name="client">The current pop3 client.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncGetEmailMessage(long position, Pop3Client client,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _client = client;
            _position = position;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncGetEmailMessageThread1));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous email message operation.
        /// </summary>
        /// <param name="position">The email message position.</param>
        /// <param name="headerLineCount">The number of message lines to get.</param>
        /// <param name="client">The current pop3 client.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncGetEmailMessage(long position, long headerLineCount, Pop3Client client,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _client = client;
            _position = position;
            _headerLineCount = headerLineCount;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncGetEmailMessageThread2));
            Thread.Sleep(20);
        }

        private long _position = 0;
        private Pop3Client _client = null;
        private long _headerLineCount = 0;

        /// <summary>
        /// The async email messsage method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncGetEmailMessageThread1(Object stateInfo)
        {
            // Get the email messasge.
            bool data = _client.GetEmail(_position);

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
            bool data = _client.GetEmailHeaders(_position, _headerLineCount);

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
}
