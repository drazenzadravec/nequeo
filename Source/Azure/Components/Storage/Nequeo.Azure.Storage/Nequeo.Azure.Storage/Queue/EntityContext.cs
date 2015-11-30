/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Nequeo.Azure.Storage.Queue
{
    /// <summary>
    /// Represents the base object type for a queue entity in the queue service.
    /// </summary>
    public class EntityContext
    {
        /// <summary>
        /// Represents the base object type for a queue entity in the queue service.
        /// </summary>
        /// <param name="cloudQueue">Represents a Windows Azure queue.</param>
        public EntityContext(CloudQueue cloudQueue)
        {
            _cloudQueue = cloudQueue;
        }

        private CloudQueue _cloudQueue = null;

        /// <summary>
        /// Gets the cloud queue.
        /// </summary>
        public CloudQueue CloudQueue
        {
            get { return _cloudQueue; }
        }

        /// <summary>
        /// Clear all messages from the queue.
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task ClearAsync()
        {
            await _cloudQueue.ClearAsync();
        }

        /// <summary>
        /// Clear all messages from the queue.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task ClearAsync(CancellationToken cancellationToken)
        {
            await _cloudQueue.ClearAsync(cancellationToken);
        }

        /// <summary>
        /// Check the existence of the queue.
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task<bool> ExistsAsync()
        {
            return await _cloudQueue.ExistsAsync();
        }

        /// <summary>
        /// Check the existence of the queue.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> ExistsAsync(CancellationToken cancellationToken)
        {
            return await _cloudQueue.ExistsAsync(cancellationToken);
        }

        /// <summary>
        /// Delete queue async.
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task<bool> DeleteFileShareAsync()
        {
            return await _cloudQueue.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Delete queue async.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> DeleteFileShareAsync(CancellationToken cancellationToken)
        {
            return await _cloudQueue.DeleteIfExistsAsync(cancellationToken);
        }

        /// <summary>
        /// Create a queue if it does not exist.
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task<bool> CreateFileShareAsync()
        {
            bool result = await _cloudQueue.CreateIfNotExistsAsync();
            return result;
        }

        /// <summary>
        /// Create a queue if it does not exist.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> CreateFileShareAsync(CancellationToken cancellationToken)
        {
            bool result = await _cloudQueue.CreateIfNotExistsAsync(cancellationToken);
            return result;
        }

        /// <summary>
        /// Add a message to the queue async.
        /// </summary>
        /// <param name="content">The content to add.</param>
        /// <returns>The async task.</returns>
        public async Task AddMessageAsync(string content)
        {
            await _cloudQueue.AddMessageAsync(new CloudQueueMessage(content));
        }

        /// <summary>
        /// Add a message to the queue async.
        /// </summary>
        /// <param name="content">The content to add.</param>
        /// <returns>The async task.</returns>
        public async Task AddMessageAsync(byte[] content)
        {
            await _cloudQueue.AddMessageAsync(new CloudQueueMessage(content));
        }

        /// <summary>
        /// Add a message to the queue async.
        /// </summary>
        /// <param name="messageID">A string specifying the message ID.</param>
        /// <param name="popReceipt">A string containing the pop receipt token.</param>
        /// <returns>The async task.</returns>
        public async Task AddMessageAsync(string messageID, string popReceipt)
        {
            await _cloudQueue.AddMessageAsync(new CloudQueueMessage(messageID, popReceipt));
        }

        /// <summary>
        /// Add a message to the queue async.
        /// </summary>
        /// <param name="content">The content to add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task AddMessageAsync(string content, CancellationToken cancellationToken)
        {
            await _cloudQueue.AddMessageAsync(new CloudQueueMessage(content), cancellationToken);
        }

        /// <summary>
        /// Add a message to the queue async.
        /// </summary>
        /// <param name="content">The content to add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task AddMessageAsync(byte[] content, CancellationToken cancellationToken)
        {
            await _cloudQueue.AddMessageAsync(new CloudQueueMessage(content), cancellationToken);
        }

        /// <summary>
        /// Add a message to the queue async.
        /// </summary>
        /// <param name="messageID">A string specifying the message ID.</param>
        /// <param name="popReceipt">A string containing the pop receipt token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task AddMessageAsync(string messageID, string popReceipt, CancellationToken cancellationToken)
        {
            await _cloudQueue.AddMessageAsync(new CloudQueueMessage(messageID, popReceipt), cancellationToken);
        }

        /// <summary>
        /// Delete a message to the queue async.
        /// </summary>
        /// <param name="messageID">A string specifying the message ID.</param>
        /// <param name="popReceipt">A string containing the pop receipt token.</param>
        /// <returns>The async task.</returns>
        public async Task DeleteMessageAsync(string messageID, string popReceipt)
        {
            await _cloudQueue.DeleteMessageAsync(messageID, popReceipt);
        }

        /// <summary>
        /// Delete a message to the queue async.
        /// </summary>
        /// <param name="messageID">A string specifying the message ID.</param>
        /// <param name="popReceipt">A string containing the pop receipt token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task DeleteMessageAsync(string messageID, string popReceipt, CancellationToken cancellationToken)
        {
            await _cloudQueue.DeleteMessageAsync(messageID, popReceipt, cancellationToken);
        }

        /// <summary>
        /// Get a single message from the queue.
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task<CloudQueueMessage> GetMessageAsync()
        {
            return await _cloudQueue.GetMessageAsync();
        }

        /// <summary>
        /// Get a single message from the queue.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<CloudQueueMessage> GetMessageAsync(CancellationToken cancellationToken)
        {
            return await _cloudQueue.GetMessageAsync(cancellationToken);
        }

        /// <summary>
        /// Get messages from the queue.
        /// </summary>
        /// <param name="messageCount">The number of messages to retrieve.</param>
        /// <returns>The async task.</returns>
        public async Task<IEnumerable<CloudQueueMessage>> GetMessagesAsync(int messageCount)
        {
            return await _cloudQueue.GetMessagesAsync(messageCount);
        }

        /// <summary>
        /// Get messages from the queue.
        /// </summary>
        /// <param name="messageCount">The number of messages to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<IEnumerable<CloudQueueMessage>> GetMessagesAsync(int messageCount, CancellationToken cancellationToken)
        {
            return await _cloudQueue.GetMessagesAsync(messageCount, cancellationToken);
        }

        /// <summary>
        /// Get a single message from the queue.
        /// </summary>
        /// <param name="visibilityTimeout">A System.TimeSpan specifying the visibility timeout interval.</param>
        /// <returns>The async task.</returns>
        public async Task<CloudQueueMessage> GetMessageAsync(TimeSpan visibilityTimeout)
        {
            return await _cloudQueue.GetMessageAsync(visibilityTimeout, null, null);
        }

        /// <summary>
        /// Get a single message from the queue.
        /// </summary>
        /// <param name="visibilityTimeout">A System.TimeSpan specifying the visibility timeout interval.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<CloudQueueMessage> GetMessageAsync(TimeSpan visibilityTimeout, CancellationToken cancellationToken)
        {
            return await _cloudQueue.GetMessageAsync(visibilityTimeout, null, null, cancellationToken);
        }

        /// <summary>
        /// Get messages from the queue.
        /// </summary>
        /// <param name="messageCount">The number of messages to retrieve.</param>
        /// <param name="visibilityTimeout">A System.TimeSpan specifying the visibility timeout interval.</param>
        /// <returns>The async task.</returns>
        public async Task<IEnumerable<CloudQueueMessage>> GetMessagesAsync(int messageCount, TimeSpan visibilityTimeout)
        {
            return await _cloudQueue.GetMessagesAsync(messageCount, visibilityTimeout, null, null);
        }

        /// <summary>
        /// Get messages from the queue.
        /// </summary>
        /// <param name="messageCount">The number of messages to retrieve.</param>
        /// <param name="visibilityTimeout">A System.TimeSpan specifying the visibility timeout interval.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<IEnumerable<CloudQueueMessage>> GetMessagesAsync(int messageCount, TimeSpan visibilityTimeout, CancellationToken cancellationToken)
        {
            return await _cloudQueue.GetMessagesAsync(messageCount, visibilityTimeout, null, null, cancellationToken);
        }

        /// <summary>
        /// Fetch the queue's attributes.
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task FetchAttributesAsync()
        {
            await _cloudQueue.FetchAttributesAsync();
        }

        /// <summary>
        /// Fetch the queue's attributes.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task FetchAttributesAsync(CancellationToken cancellationToken)
        {
            await _cloudQueue.FetchAttributesAsync(cancellationToken);
        }

        /// <summary>
        /// Get a single message from the queue.
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task<CloudQueueMessage> PeekMessageAsync()
        {
            return await _cloudQueue.PeekMessageAsync();
        }

        /// <summary>
        /// Get a single message from the queue.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<CloudQueueMessage> PeekMessageAsync(CancellationToken cancellationToken)
        {
            return await _cloudQueue.PeekMessageAsync(cancellationToken);
        }

        /// <summary>
        /// Get messages from the queue.
        /// </summary>
        /// <param name="messageCount">The number of messages to retrieve.</param>
        /// <returns>The async task.</returns>
        public async Task<IEnumerable<CloudQueueMessage>> PeekMessagesAsync(int messageCount)
        {
            return await _cloudQueue.PeekMessagesAsync(messageCount);
        }

        /// <summary>
        /// Get messages from the queue.
        /// </summary>
        /// <param name="messageCount">The number of messages to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<IEnumerable<CloudQueueMessage>> PeekMessagesAsync(int messageCount, CancellationToken cancellationToken)
        {
            return await _cloudQueue.PeekMessagesAsync(messageCount, cancellationToken);
        }

        /// <summary>
        /// Update the visibility timeout and optionally the content of a message.
        /// </summary>
        /// <param name="cloudQueueMessage">A Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage object.</param>
        /// <param name="visibilityTimeout">A System.TimeSpan specifying the visibility timeout interval.</param>
        /// <param name="messageUpdateFields">A set of Microsoft.WindowsAzure.Storage.Queue.MessageUpdateFields values that specify which parts of the message are to be updated.</param>
        /// <returns>The async task.</returns>
        public async Task UpdateMessageAsync(CloudQueueMessage cloudQueueMessage, TimeSpan visibilityTimeout, MessageUpdateFields messageUpdateFields)
        {
            await _cloudQueue.UpdateMessageAsync(cloudQueueMessage, visibilityTimeout, messageUpdateFields);
        }

        /// <summary>
        /// Update the visibility timeout and optionally the content of a message.
        /// </summary>
        /// <param name="cloudQueueMessage">A Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage object.</param>
        /// <param name="visibilityTimeout">A System.TimeSpan specifying the visibility timeout interval.</param>
        /// <param name="messageUpdateFields">A set of Microsoft.WindowsAzure.Storage.Queue.MessageUpdateFields values that specify which parts of the message are to be updated.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task UpdateMessageAsync(CloudQueueMessage cloudQueueMessage, TimeSpan visibilityTimeout, MessageUpdateFields messageUpdateFields, CancellationToken cancellationToken)
        {
            await _cloudQueue.UpdateMessageAsync(cloudQueueMessage, visibilityTimeout, messageUpdateFields, cancellationToken);
        }
    }
}
