/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          QueueCloudClient.h
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

#pragma once

#include "stdafx.h"

#include "Global.h"
#include "AzureAccount.h"

#include <was\queue.h>

#undef PeekMessage

namespace Nequeo {
	namespace Azure {
		namespace Storage
		{
			///	<summary>
			///	Cloud client provider.
			///	</summary>
			class EXPORT_NEQUEO_AZURE_STORAGE_API QueueCloudClient
			{
			public:
				///	<summary>
				///	Cloud client provider.
				///	</summary>
				/// <param name="account">The Azure account.</param>
				QueueCloudClient(const AzureAccount& account);

				///	<summary>
				///	Cloud client provider destructor.
				///	</summary>
				~QueueCloudClient();

				/// <summary>
				/// Has the client been initialised.
				/// </summary>
				/// <returns>True if is initialised; else false.</returns>
				inline bool IsInitialised() const
				{
					return _isInitialised;
				}

				///	<summary>
				///	Get the queue client.
				///	</summary>
				/// <returns>The queue client.</returns>
				const azure::storage::cloud_queue_client& QueueClient() const;

				///	<summary>
				///	Initialise the Queue client.
				///	</summary>
				void Initialise();

				///	<summary>
				///	Initialise the Queue client.
				///	</summary>
				/// <param name="default_request_options">Default queue request options.</param>
				void Initialise(const azure::storage::queue_request_options& default_request_options);

				///	<summary>
				///	Get the list if queues items.
				///	</summary>
				/// <returns>The list of queues items.</returns>
				const std::vector<azure::storage::cloud_queue> ListQueues() const;

				///	<summary>
				///	Get the list if queues items.
				///	</summary>
				/// <param name="prefix">The queues name prefix.</param>
				/// <returns>The list of queues items.</returns>
				const std::vector<azure::storage::cloud_queue> ListQueues(const utility::string_t& prefix) const;

				/// <summary>
				/// Intitiates an asynchronous operation to return a result segment containing a collection of queue items.
				/// </summary>
				/// <param name="token">A continuation token returned by a previous listing operation.</param>
				/// <param name="prefix">The queue name prefix.</param>
				/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::queue_result_segment"/> that represents the current operation.</returns>
				Concurrency::task<azure::storage::queue_result_segment> ListQueuesSegmentedAsync(
					const utility::string_t& prefix, const azure::storage::continuation_token& token) const;

				/// <summary>
				/// Intitiates an asynchronous operation to set the service properties for the Queue service client.
				/// </summary>
				/// <param name="properties">The <see cref="azure::storage::service_properties"/> for the Queue service client.</param>
				/// <param name="includes">An <see cref="azure::storage::service_properties_includes /> enumeration describing which items to include when setting service properties.</param>
				/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
				Concurrency::task<void> UploadServicePropertiesAsync(
					const azure::storage::service_properties& properties,
					const azure::storage::service_properties_includes& includes) const;

				/// <summary>
				/// Intitiates an asynchronous operation to get the properties of the service.
				/// </summary>
				/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::service_properties"/> that represents the current operation.</returns>
				Concurrency::task<azure::storage::service_properties> DownloadServicePropertiesAsync() const;

				/// <summary>
				/// Intitiates an asynchronous operation to get the properties of the service.
				/// </summary>
				/// <param name="options">An <see cref="azure::storage::queue_request_options"/> object that specifies additional options for the request.</param>
				/// <param name="context">An <see cref="azure::storage::operation_context"/> object that represents the context for the current operation.</param>
				/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::service_properties"/> that represents the current operation.</returns>
				Concurrency::task<azure::storage::service_properties> DownloadServicePropertiesAsync(
					const azure::storage::queue_request_options& options, azure::storage::operation_context context) const;

				///	<summary>
				///	Create a Queue.
				///	</summary>
				/// <param name="queueName">The queue name to create.</param>
				/// <returns>True if created; else false.</returns>
				bool CreateQueue(const utility::string_t& queueName);

				///	<summary>
				///	Delete a Queue.
				///	</summary>
				/// <param name="queueName">The queue name to delete.</param>
				/// <returns>True if deleted; else false.</returns>
				bool DeleteQueue(const utility::string_t& queueName);

				///	<summary>
				///	Create a Message.
				///	</summary>
				/// <param name="queueName">The queue name.</param>
				/// <param name="message">The message to create.</param>
				void CreateMessage(const utility::string_t& queueName, const utility::string_t& message);

				///	<summary>
				///	Create a Message.
				///	</summary>
				/// <param name="queueName">The queue name.</param>
				/// <param name="content">The content of the message as raw data.</param>
				void CreateMessage(const utility::string_t& queueName, const std::vector<uint8_t>& content);

				///	<summary>
				///	Create a Message.
				///	</summary>
				/// <param name="queueName">The queue name.</param>
				/// <param name="message">The message to create.</param>
				/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::service_properties"/> that represents the current operation.</returns>
				Concurrency::task<void> CreateMessageAsync(const utility::string_t& queueName, const utility::string_t& message);

				///	<summary>
				///	Create a Message.
				///	</summary>
				/// <param name="queueName">The queue name.</param>
				/// <param name="content">The content of the message as raw data.</param>
				/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::service_properties"/> that represents the current operation.</returns>
				Concurrency::task<void> CreateMessageAsync(const utility::string_t& queueName, const std::vector<uint8_t>& content);

				/// <summary>
				/// Peeks a message from the front of the queue.
				/// </summary>
				/// <param name="queueName">The queue name.</param>
				/// <returns>An <see cref="azure::storage::cloud_queue_message"/> object.</returns>
				azure::storage::cloud_queue_message PeekMessage(const utility::string_t& queueName) const;

				/// <summary>
				/// Peeks a message from the front of the queue.
				/// </summary>
				/// <param name="queueName">The queue name.</param>
				/// <returns>A <see cref="concurrency::task"/> object of type <see cref="azure::storage::cloud_queue_message"/> that represents the current operation.</returns>
				Concurrency::task<azure::storage::cloud_queue_message> PeekMessageAsync(const utility::string_t& queueName) const;

				/// <summary>
				/// Peeks a message from the front of the queue.
				/// </summary>
				/// <param name="queueName">The queue name.</param>
				/// <param name="options">An <see cref="azure::storage::queue_request_options"/> object that specifies additional options for the request.</param>
				/// <param name="context">An <see cref="azure::storage::operation_context"/> object that represents the context for the current operation.</param>
				/// <returns>An <see cref="azure::storage::cloud_queue_message"/> object.</returns>
				azure::storage::cloud_queue_message PeekMessage(
					const utility::string_t& queueName, 
					const azure::storage::queue_request_options& options,
					azure::storage::operation_context context) const;

				/// <summary>
				/// Peeks a message from the front of the queue.
				/// </summary>
				/// <param name="queueName">The queue name.</param>
				/// <param name="options">An <see cref="azure::storage::queue_request_options"/> object that specifies additional options for the request.</param>
				/// <param name="context">An <see cref="azure::storage::operation_context"/> object that represents the context for the current operation.</param>
				/// <returns>A <see cref="concurrency::task"/> object of type <see cref="azure::storage::cloud_queue_message"/> that represents the current operation.</returns>
				Concurrency::task<azure::storage::cloud_queue_message> PeekMessageAsync(
					const utility::string_t& queueName,
					const azure::storage::queue_request_options& options,
					azure::storage::operation_context context) const;

				///	<summary>
				///	Change a message from the front of the queue.
				///	</summary>
				/// <param name="queueName">The queue name.</param>
				/// <param name="message">The message to assign.</param>
				/// <param name="visibilityTimeout">The time interval, in seconds, after which the message becomes visible again, unless it has been deleted.</param>
				void ChangeMessage(const utility::string_t& queueName, const utility::string_t& message, long long visibilityTimeout = 60);

				///	<summary>
				///	Change a message from the front of the queue.
				///	</summary>
				/// <param name="queueName">The queue name.</param>
				/// <param name="message">The message to assign.</param>
				/// <param name="visibilityTimeout">The time interval, in seconds, after which the message becomes visible again, unless it has been deleted.</param>
				/// <returns>A <see cref="concurrency::task"/> object that represents the current operation.</returns>
				Concurrency::task<void> ChangeMessageAsync(const utility::string_t& queueName, const utility::string_t& message, long long visibilityTimeout = 60);

				///	<summary>
				///	De-Queue a message from the front of the queue.
				///	</summary>
				/// <param name="queueName">The queue name.</param>
				/// <returns>An <see cref="azure::storage::cloud_queue_message"/> object.</returns>
				azure::storage::cloud_queue_message DequeueMessage(const utility::string_t& queueName);

				///	<summary>
				///	De-Queue a message from the front of the queue.
				///	</summary>
				/// <param name="queueName">The queue name.</param>
				/// <returns>A <see cref="concurrency::task"/> object of type <see cref="azure::storage::cloud_queue_message"/> that represents the current operation.</returns>
				Concurrency::task<azure::storage::cloud_queue_message> DequeueMessageAsync(const utility::string_t& queueName);

				///	<summary>
				///	De-Queue a message from the front of the queue.
				///	</summary>
				/// <param name="queueName">The queue name.</param>
				/// <param name="messageCount">The number of messages to return.</param>
				/// <param name="visibilityTimeout">The time interval, in seconds, after which the message becomes visible again, unless it has been deleted.</param>
				/// <returns>The collection of messages.</returns>
				std::vector<azure::storage::cloud_queue_message> DequeueMessageList(const utility::string_t& queueName, long long messageCount = 20, long long visibilityTimeout = 300);

				///	<summary>
				///	De-Queue a message from the front of the queue.
				///	</summary>
				/// <param name="queueName">The queue name.</param>
				/// <param name="messageCount">The number of messages to return.</param>
				/// <param name="visibilityTimeout">The time interval, in seconds, after which the message becomes visible again, unless it has been deleted.</param>
				/// <returns>The collection of messages <see cref="concurrency::task"/>.</returns>
				Concurrency::task<std::vector<azure::storage::cloud_queue_message>> DequeueMessageListAsync(
					const utility::string_t& queueName, long long messageCount = 20, long long visibilityTimeout = 300);

				///	<summary>
				///	Get the approximate message count.
				///	</summary>
				/// <param name="queueName">The queue name.</param>
				/// <returns>The approximate message count.</returns>
				int GetMessageCount(const utility::string_t& queueName) const;

				///	<summary>
				///	Get the approximate message count.
				///	</summary>
				/// <param name="queueName">The queue name.</param>
				/// <returns>The approximate message count <see cref="concurrency::task"/>.</returns>
				Concurrency::task<int> GetMessageCountAsync(const utility::string_t& queueName);

			private:
				bool _disposed;
				bool _isInitialised;

				AzureAccount _account;
				azure::storage::cloud_queue_client _client;
			};
		}
	}
}