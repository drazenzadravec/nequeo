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