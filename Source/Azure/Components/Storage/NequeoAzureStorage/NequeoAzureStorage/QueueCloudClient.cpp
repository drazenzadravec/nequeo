/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          QueueCloudClient.cpp
*  Purpose :       Cloud client provider class.
*
*/

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

#include "stdafx.h"

#include "QueueCloudClient.h"

using namespace Nequeo::Azure::Storage;

///	<summary>
///	Cloud client provider.
///	</summary>
/// <param name="account">The Azure account.</param>
QueueCloudClient::QueueCloudClient(const AzureAccount& account) : _disposed(false), _isInitialised(false), _account(account)
{
}

///	<summary>
///	Cloud client provider.
///	</summary>
QueueCloudClient::~QueueCloudClient()
{
	if (!_disposed)
	{
		_disposed = true;
		_isInitialised = false;
	}
}

///	<summary>
///	Get the queue client.
///	</summary>
/// <returns>The queue client.</returns>
const azure::storage::cloud_queue_client& QueueCloudClient::QueueClient() const
{
	return _client;
}

///	<summary>
///	Initialise the Queue client.
///	</summary>
void QueueCloudClient::Initialise()
{
	// If not initialised;
	if (!_isInitialised)
	{
		_client = _account._account.create_cloud_queue_client();
		_isInitialised = true;
	}
}

///	<summary>
///	Initialise the Queue client.
///	</summary>
/// <param name="default_request_options">Default queue request options.</param>
void QueueCloudClient::Initialise(const azure::storage::queue_request_options& default_request_options)
{
	// If not initialised;
	if (!_isInitialised)
	{
		_client = _account._account.create_cloud_queue_client(default_request_options);
		_isInitialised = true;
	}
}

///	<summary>
///	Get the list if queues items.
///	</summary>
/// <returns>The list of queues items.</returns>
const std::vector<azure::storage::cloud_queue> QueueCloudClient::ListQueues() const
{
	std::vector<azure::storage::cloud_queue> items;

	// Get the list.
	auto itemIterator = _client.list_queues();

	// Iterate through the list.
	for (azure::storage::cloud_queue item : itemIterator)
	{
		// Add the item.
		items.push_back(item);
	}

	// Return the items.
	return items;
}

///	<summary>
///	Get the list if queues items.
///	</summary>
/// <param name="prefix">The queues name prefix.</param>
/// <returns>The list of queues items.</returns>
const std::vector<azure::storage::cloud_queue> QueueCloudClient::ListQueues(const utility::string_t& prefix) const
{
	std::vector<azure::storage::cloud_queue> items;

	// Get the list.
	auto itemIterator = _client.list_queues(prefix);

	// Iterate through the list.
	for (azure::storage::cloud_queue item : itemIterator)
	{
		// Add the item.
		items.push_back(item);
	}

	// Return the items.
	return items;
}

/// <summary>
/// Intitiates an asynchronous operation to return a result segment containing a collection of queue items.
/// </summary>
/// <param name="token">A continuation token returned by a previous listing operation.</param>
/// <param name="prefix">The queue name prefix.</param>
/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::queue_result_segment"/> that represents the current operation.</returns>
Concurrency::task<azure::storage::queue_result_segment> QueueCloudClient::ListQueuesSegmentedAsync(
	const utility::string_t& prefix, const azure::storage::continuation_token& token) const
{
	return _client.list_queues_segmented_async(prefix, token);
}

/// <summary>
/// Intitiates an asynchronous operation to set the service properties for the Queue service client.
/// </summary>
/// <param name="properties">The <see cref="azure::storage::service_properties"/> for the Queue service client.</param>
/// <param name="includes">An <see cref="azure::storage::service_properties_includes /> enumeration describing which items to include when setting service properties.</param>
/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
Concurrency::task<void> QueueCloudClient::UploadServicePropertiesAsync(
	const azure::storage::service_properties& properties,
	const azure::storage::service_properties_includes& includes) const
{
	return _client.upload_service_properties_async(properties, includes);
}

/// <summary>
/// Intitiates an asynchronous operation to get the properties of the service.
/// </summary>
/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::service_properties"/> that represents the current operation.</returns>
Concurrency::task<azure::storage::service_properties> QueueCloudClient::DownloadServicePropertiesAsync() const
{
	return _client.download_service_properties_async();
}

/// <summary>
/// Intitiates an asynchronous operation to get the properties of the service.
/// </summary>
/// <param name="options">An <see cref="azure::storage::queue_request_options"/> object that specifies additional options for the request.</param>
/// <param name="context">An <see cref="azure::storage::operation_context"/> object that represents the context for the current operation.</param>
/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::service_properties"/> that represents the current operation.</returns>
Concurrency::task<azure::storage::service_properties> QueueCloudClient::DownloadServicePropertiesAsync(
	const azure::storage::queue_request_options& options, azure::storage::operation_context context) const
{
	return _client.download_service_properties_async(options, context);
}

///	<summary>
///	Create a Queue.
///	</summary>
/// <param name="queueName">The Queue name to create.</param>
/// <returns>True if created; else false.</returns>
bool QueueCloudClient::CreateQueue(const utility::string_t& queueName)
{
	// Retrieve a reference to a queue.
	azure::storage::cloud_queue queue = _client.get_queue_reference(queueName);

	// Create the queue if it doesn't already exist.
	return queue.create_if_not_exists();
}

///	<summary>
///	Delete a Queue.
///	</summary>
/// <param name="queueName">The queue name to delete.</param>
/// <returns>True if deleted; else false.</returns>
bool QueueCloudClient::DeleteQueue(const utility::string_t& queueName)
{
	// Retrieve a reference to a queue.
	azure::storage::cloud_queue queue = _client.get_queue_reference(queueName);

	// Delete the queue if it already exist.
	return queue.delete_queue_if_exists();
}

///	<summary>
///	Create a Message.
///	</summary>
/// <param name="queueName">The queue name.</param>
/// <param name="message">The message to create.</param>
void QueueCloudClient::CreateMessage(const utility::string_t& queueName, const utility::string_t& message)
{
	// Retrieve a reference to a queue.
	azure::storage::cloud_queue queue = _client.get_queue_reference(queueName);

	// Create the queue if it doesn't already exist.
	queue.create_if_not_exists();

	// Create a message and add it to the queue.
	azure::storage::cloud_queue_message message1(message);
	queue.add_message(message1);
}

///	<summary>
///	Create a Message.
///	</summary>
/// <param name="queueName">The queue name.</param>
/// <param name="content">The content of the message as raw data.</param>
void QueueCloudClient::CreateMessage(const utility::string_t& queueName, const std::vector<uint8_t>& content)
{
	// Retrieve a reference to a queue.
	azure::storage::cloud_queue queue = _client.get_queue_reference(queueName);

	// Create the queue if it doesn't already exist.
	queue.create_if_not_exists();

	// Create a message and add it to the queue.
	azure::storage::cloud_queue_message message1(content);
	queue.add_message(message1);
}

///	<summary>
///	Create a Message.
///	</summary>
/// <param name="queueName">The queue name.</param>
/// <param name="message">The message to create.</param>
/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::service_properties"/> that represents the current operation.</returns>
Concurrency::task<void> QueueCloudClient::CreateMessageAsync(const utility::string_t& queueName, const utility::string_t& message)
{
	// Retrieve a reference to a queue.
	azure::storage::cloud_queue queue = _client.get_queue_reference(queueName);

	// Create the queue if it doesn't already exist.
	queue.create_if_not_exists();

	// Create a message and add it to the queue.
	azure::storage::cloud_queue_message message1(message);
	return queue.add_message_async(message1);
}

///	<summary>
///	Create a Message.
///	</summary>
/// <param name="queueName">The queue name.</param>
/// <param name="content">The content of the message as raw data.</param>
/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::service_properties"/> that represents the current operation.</returns>
Concurrency::task<void> QueueCloudClient::CreateMessageAsync(const utility::string_t& queueName, const std::vector<uint8_t>& content)
{
	// Retrieve a reference to a queue.
	azure::storage::cloud_queue queue = _client.get_queue_reference(queueName);

	// Create the queue if it doesn't already exist.
	queue.create_if_not_exists();

	// Create a message and add it to the queue.
	azure::storage::cloud_queue_message message1(content);
	return queue.add_message_async(message1);
}

/// <summary>
/// Peeks a message from the front of the queue
/// </summary>
/// <param name="queueName">The queue name.</param>
/// <returns>An <see cref="azure::storage::cloud_queue_message"/> object.</returns>
azure::storage::cloud_queue_message QueueCloudClient::PeekMessage(const utility::string_t& queueName) const
{
	// Retrieve a reference to a queue.
	azure::storage::cloud_queue queue = _client.get_queue_reference(queueName);

	// Peek at the next message.
	azure::storage::cloud_queue_message peeked_message = queue.peek_message();
	return peeked_message;
}

/// <summary>
/// Peeks a message from the front of the queue.
/// </summary>
/// <param name="queueName">The queue name.</param>
/// <returns>A <see cref="concurrency::task"/> object of type <see cref="azure::storage::cloud_queue_message" /> that represents the current operation.</returns>
Concurrency::task<azure::storage::cloud_queue_message> QueueCloudClient::PeekMessageAsync(const utility::string_t& queueName) const
{
	// Retrieve a reference to a queue.
	azure::storage::cloud_queue queue = _client.get_queue_reference(queueName);

	// Peek at the next message.
	return queue.peek_message_async();
}

/// <summary>
/// Peeks a message from the front of the queue.
/// </summary>
/// <param name="queueName">The queue name.</param>
/// <param name="options">An <see cref="azure::storage::queue_request_options"/> object that specifies additional options for the request.</param>
/// <param name="context">An <see cref="azure::storage::operation_context"/> object that represents the context for the current operation.</param>
/// <returns>An <see cref="azure::storage::cloud_queue_message"/> object.</returns>
azure::storage::cloud_queue_message QueueCloudClient::PeekMessage(
	const utility::string_t& queueName,
	const azure::storage::queue_request_options& options,
	azure::storage::operation_context context) const
{
	// Retrieve a reference to a queue.
	azure::storage::cloud_queue queue = _client.get_queue_reference(queueName);

	// Peek at the next message.
	azure::storage::cloud_queue_message peeked_message = queue.peek_message(options, context);
	return peeked_message;
}

/// <summary>
/// Peeks a message from the front of the queue.
/// </summary>
/// <param name="queueName">The queue name.</param>
/// <param name="options">An <see cref="azure::storage::queue_request_options"/> object that specifies additional options for the request.</param>
/// <param name="context">An <see cref="azure::storage::operation_context"/> object that represents the context for the current operation.</param>
/// <returns>A <see cref="concurrency::task"/> object of type <see cref="azure::storage::cloud_queue_message" /> that represents the current operation.</returns>
Concurrency::task<azure::storage::cloud_queue_message> QueueCloudClient::PeekMessageAsync(
	const utility::string_t& queueName,
	const azure::storage::queue_request_options& options,
	azure::storage::operation_context context) const
{
	// Retrieve a reference to a queue.
	azure::storage::cloud_queue queue = _client.get_queue_reference(queueName);

	// Peek at the next message.
	return queue.peek_message_async(options, context);
}

///	<summary>
///	Change a message from the front of the queue.
///	</summary>
/// <param name="queueName">The queue name.</param>
/// <param name="message">The message to assign.</param>
/// <param name="visibilityTimeout">The time interval, in seconds, after which the message becomes visible again, unless it has been deleted.</param>
void QueueCloudClient::ChangeMessage(const utility::string_t& queueName, const utility::string_t& message, long long visibilityTimeout)
{
	// Retrieve a reference to a queue.
	azure::storage::cloud_queue queue = _client.get_queue_reference(queueName);

	// Get the message from the queue and update the message contents.
	// The visibility timeout "0" means make it visible immediately.
	// The visibility timeout "60" means the client can get another minute to continue
	// working on the message.
	azure::storage::cloud_queue_message changed_message = queue.get_message();

	changed_message.set_content(message);
	queue.update_message(changed_message, std::chrono::seconds(visibilityTimeout), true);
}

///	<summary>
///	Change a message from the front of the queue.
///	</summary>
/// <param name="queueName">The queue name.</param>
/// <param name="message">The message to assign.</param>
/// <param name="visibilityTimeout">The time interval, in seconds, after which the message becomes visible again, unless it has been deleted.</param>
/// <returns>A <see cref="concurrency::task"/> object that represents the current operation.</returns>
Concurrency::task<void> QueueCloudClient::ChangeMessageAsync(const utility::string_t& queueName, const utility::string_t& message, long long visibilityTimeout)
{
	// Create a new internal task.
	return Concurrency::create_task([this, queueName, message, visibilityTimeout]() -> void
	{
		// Retrieve a reference to a queue.
		azure::storage::cloud_queue queue = _client.get_queue_reference(queueName);
		
		// Get the message from the queue and update the message contents.
		// The visibility timeout "0" means make it visible immediately.
		// The visibility timeout "60" means the client can get another minute to continue
		// working on the message.
		azure::storage::cloud_queue_message changed_message = queue.get_message_async().get();

		changed_message.set_content(message);
		queue.update_message_async(changed_message, std::chrono::seconds(visibilityTimeout), true).get();
	});
}

///	<summary>
///	De-Queue a message from the front of the queue.
///	</summary>
/// <param name="queueName">The queue name.</param>
/// <returns>An <see cref="azure::storage::cloud_queue_message"/> object.</returns>
azure::storage::cloud_queue_message QueueCloudClient::DequeueMessage(const utility::string_t& queueName)
{
	// Retrieve a reference to a queue.
	azure::storage::cloud_queue queue = _client.get_queue_reference(queueName);

	// Get the next message.
	azure::storage::cloud_queue_message dequeued_message = queue.get_message();
	
	// Delete the message.
	queue.delete_message(dequeued_message);
	return dequeued_message;
}

///	<summary>
///	De-Queue a message from the front of the queue.
///	</summary>
/// <param name="queueName">The queue name.</param>
/// <returns>A <see cref="concurrency::task"/> object of type <see cref="azure::storage::cloud_queue_message"/> that represents the current operation.</returns>
Concurrency::task<azure::storage::cloud_queue_message> QueueCloudClient::DequeueMessageAsync(const utility::string_t& queueName)
{
	// Create a new internal task.
	return Concurrency::create_task([this, queueName]() -> azure::storage::cloud_queue_message
	{
		// Retrieve a reference to a queue.
		azure::storage::cloud_queue queue = _client.get_queue_reference(queueName);

		// Get the next message.
		azure::storage::cloud_queue_message dequeued_message = queue.get_message_async().get();

		// Delete the message.
		queue.delete_message_async(dequeued_message).get();
		return dequeued_message;
	});
}

///	<summary>
///	De-Queue a message from the front of the queue.
///	</summary>
/// <param name="queueName">The queue name.</param>
/// <param name="messageCount">The number of messages to return.</param>
/// <param name="visibilityTimeout">The time interval, in seconds, after which the message becomes visible again, unless it has been deleted.</param>
/// <returns>The collection of messages.</returns>
std::vector<azure::storage::cloud_queue_message> QueueCloudClient::DequeueMessageList(const utility::string_t& queueName, long long messageCount, long long visibilityTimeout)
{
	// Retrieve a reference to a queue.
	azure::storage::cloud_queue queue = _client.get_queue_reference(queueName);

	// Dequeue some queue messages (maximum 32 at a time) and set their visibility timeout.
	azure::storage::queue_request_options options;
	azure::storage::operation_context context;

	// Retrieve messages from the queue with a visibility timeout.
	std::vector<azure::storage::cloud_queue_message> messages = queue.get_messages(messageCount, std::chrono::seconds(visibilityTimeout), options, context);
	return messages;
}

///	<summary>
///	De-Queue a message from the front of the queue.
///	</summary>
/// <param name="queueName">The queue name.</param>
/// <param name="messageCount">The number of messages to return.</param>
/// <param name="visibilityTimeout">The time interval, in seconds, after which the message becomes visible again, unless it has been deleted.</param>
/// <returns>The collection of messages <see cref="concurrency::task"/>.</returns>
Concurrency::task<std::vector<azure::storage::cloud_queue_message>> QueueCloudClient::DequeueMessageListAsync(
	const utility::string_t& queueName, long long messageCount, long long visibilityTimeout)
{
	// Retrieve a reference to a queue.
	azure::storage::cloud_queue queue = _client.get_queue_reference(queueName);

	// Dequeue some queue messages (maximum 32 at a time) and set their visibility timeout.
	azure::storage::queue_request_options options;
	azure::storage::operation_context context;

	// Retrieve messages from the queue with a visibility timeout.
	return queue.get_messages_async(messageCount, std::chrono::seconds(visibilityTimeout), options, context);
}

///	<summary>
///	Get the approximate message count.
///	</summary>
/// <param name="queueName">The queue name.</param>
/// <returns>The approximate message count.</returns>
int QueueCloudClient::GetMessageCount(const utility::string_t& queueName) const
{
	// Retrieve a reference to a queue.
	azure::storage::cloud_queue queue = _client.get_queue_reference(queueName);

	// Fetch the queue attributes.
	queue.download_attributes();

	// Retrieve the cached approximate message count.
	return queue.approximate_message_count();
}

///	<summary>
///	Get the approximate message count.
///	</summary>
/// <param name="queueName">The queue name.</param>
/// <returns>The approximate message count <see cref="concurrency::task"/>.</returns>
Concurrency::task<int> QueueCloudClient::GetMessageCountAsync(const utility::string_t& queueName)
{
	// Create a new internal task.
	return Concurrency::create_task([this, queueName]() -> int
	{
		// Retrieve a reference to a queue.
		azure::storage::cloud_queue queue = _client.get_queue_reference(queueName);

		// Fetch the queue attributes.
		queue.download_attributes_async().get();
		
		// Retrieve the cached approximate message count.
		return queue.approximate_message_count();
	});
}