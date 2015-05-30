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
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Nequeo.Net.Mail.Extension
{
    /// <summary>
    /// Extension methods for working with SmtpClient asynchronously.
    /// </summary>
    public static class SmtpClientExtensions
    {
        /// <summary>Sends an e-mail message asynchronously.</summary>
        /// <param name="smtpClient">The client.</param>
        /// <param name="message">A MailMessage that contains the message to send.</param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A Task that represents the asynchronous send.</returns>
        public static Task SendTask(this SmtpClient smtpClient, MailMessage message, object userToken)
        {
            return SendTaskCore(smtpClient, userToken, tcs => smtpClient.SendAsync(message, tcs));
        }

        /// <summary>Sends an e-mail message asynchronously.</summary>
        /// <param name="smtpClient">The client.</param>
        /// <param name="from">A String that contains the address information of the message sender.</param>
        /// <param name="recipients">A String that contains the address that the message is sent to.</param>
        /// <param name="subject">A String that contains the subject line for the message.</param>
        /// <param name="body">A String that contains the message body.</param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A Task that represents the asynchronous send.</returns>
        public static Task SendTask(this SmtpClient smtpClient, string from, string recipients, string subject, string body, object userToken)
        {
            return SendTaskCore(smtpClient, userToken, tcs => smtpClient.SendAsync(from, recipients, subject, body, tcs));
        }

        /// <summary>The core implementation of SendTask.</summary>
        /// <param name="smtpClient">The client.</param>
        /// <param name="userToken">The user-supplied state.</param>
        /// <param name="sendAsync">
        /// A delegate that initiates the asynchronous send.
        /// The provided TaskCompletionSource must be passed as the user-supplied state to the actual SmtpClient.SendAsync method.
        /// </param>
        /// <returns></returns>
        private static Task SendTaskCore(SmtpClient smtpClient, object userToken, Action<TaskCompletionSource<object>> sendAsync)
        {
            // Validate we're being used with a real smtpClient.  The rest of the arg validation
            // will happen in the call to sendAsync.
            if (smtpClient == null) throw new ArgumentNullException("smtpClient");

            // Create a TaskCompletionSource to represent the operation
            var tcs = new TaskCompletionSource<object>(userToken);

            // Register a handler that will transfer completion results to the TCS Task
            SendCompletedEventHandler handler = null;
            handler = (sender, e) => Nequeo.Threading.Common.EAPCommon.HandleCompletion(tcs, e, () => null, () => smtpClient.SendCompleted -= handler);
            smtpClient.SendCompleted += handler;

            // Try to start the async operation.  If starting it fails (due to parameter validation)
            // unregister the handler before allowing the exception to propagate.
            try
            {
                sendAsync(tcs);
            }
            catch (Exception exc)
            {
                smtpClient.SendCompleted -= handler;
                tcs.TrySetException(exc);
            }

            // Return the task to represent the asynchronous operation
            return tcs.Task;
        }
    }
}
