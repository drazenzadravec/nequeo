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
using System.Configuration;
using System.Messaging;
using System.Transactions;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.MsmqIntegration;

namespace Nequeo.Management.MessageQueue
{
    /// <summary>
    /// Message queue client implementation
    /// </summary>
	public partial class Client
	{
        /// <summary>
        /// Send the message to the message queue
        /// </summary>
        /// <param name="message">The message data to send.</param>
        public void SendMessage(Message message)
        {
            // Connect to the queue
            System.Messaging.MessageQueue serverQueue = 
                new System.Messaging.MessageQueue(
                    Nequeo.Management.MessageQueue.Properties.Settings.Default.ServerQueueName +
                    Nequeo.Management.MessageQueue.Properties.Settings.Default.QueueName);

            // Submit the message
            System.Messaging.Message msg = new System.Messaging.Message();
            msg.Body = message;
            msg.Label = "SendMessage";

            // Create a transaction scope.
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                // Send the message.
                serverQueue.Send(msg, MessageQueueTransactionType.Automatic);

                // Complete the transaction.
                scope.Complete();
            }
        }
	}
}
