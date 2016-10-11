/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          BlobCloudClient.cpp
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

#include "BlobCloudClient.h"

using namespace Nequeo::Azure::Storage;

///	<summary>
///	Cloud client provider.
///	</summary>
/// <param name="account">The Azure account.</param>
BlobCloudClient::BlobCloudClient(const AzureAccount& account) : _disposed(false), _isInitialised(false), _account(account)
{
}

///	<summary>
///	Cloud client provider.
///	</summary>
BlobCloudClient::~BlobCloudClient()
{
	if (!_disposed)
	{
		_disposed = true;
		_isInitialised = false;
	}
}

///	<summary>
///	Get the blob client.
///	</summary>
/// <returns>The blob client.</returns>
const azure::storage::cloud_blob_client& BlobCloudClient::BlobClient() const
{
	return _client;
}

///	<summary>
///	Initialise the Blob client.
///	</summary>
void BlobCloudClient::Initialise()
{
	// If not initialised;
	if (!_isInitialised)
	{
		_client = _account._account.create_cloud_blob_client();
		_isInitialised = true;
	}
}

///	<summary>
///	Initialise the Blob client.
///	</summary>
/// <param name="default_request_options">Default blob request options.</param>
void BlobCloudClient::Initialise(const azure::storage::blob_request_options& default_request_options)
{
	// If not initialised;
	if (!_isInitialised)
	{
		_client = _account._account.create_cloud_blob_client(default_request_options);
		_isInitialised = true;
	}
}

///	<summary>
///	Create a container.
///	</summary>
/// <param name="containerName">The container name to create.</param>
/// <returns>True if created; else false.</returns>
bool BlobCloudClient::CreateContainer(const utility::string_t& containerName)
{
	// Retrieve a reference to a container.
	azure::storage::cloud_blob_container container = _client.get_container_reference(containerName);

	// Create the container if it doesn't already exist.
	return container.create_if_not_exists();
}

///	<summary>
///	Delete a container.
///	</summary>
/// <param name="containerName">The container name to delete.</param>
/// <returns>True if deleted; else false.</returns>
bool BlobCloudClient::DeleteContainer(const utility::string_t& containerName)
{
	// Retrieve a reference to a container.
	azure::storage::cloud_blob_container container = _client.get_container_reference(containerName);

	// Delete the container if it already exist.
	return container.delete_container_if_exists();
}

///	<summary>
///	Delete a blob.
///	</summary>
/// <param name="containerName">The container name to delete.</param>
/// <param name="blobName">The blob name to delete.</param>
/// <returns>True if deleted; else false.</returns>
bool BlobCloudClient::DeleteBlob(const utility::string_t& containerName, const utility::string_t& blobName)
{
	// Retrieve a reference to a previously created container.
	azure::storage::cloud_blob_container container = _client.get_container_reference(containerName);

	// Retrieve reference to a blob named "my-blob-1".
	azure::storage::cloud_block_blob blockBlob = container.get_block_blob_reference(blobName);

	// Delete the blob if it exists.
	return blockBlob.delete_blob_if_exists();
}

///	<summary>
///	Upload a blob to the container.
///	</summary>
/// <param name="containerName">The container name.</param>
/// <param name="blobName">The blob name.</param>
/// <param name="localFileName">The path and name of the local file to upload.</param>
void BlobCloudClient::UploadFileBlob(const utility::string_t& containerName, const utility::string_t& blobName, const utility::string_t& localFileName)
{
	// Retrieve a reference to a previously created container.
	azure::storage::cloud_blob_container container = _client.get_container_reference(containerName);

	// Retrieve reference to a blob named "my-blob-1".
	azure::storage::cloud_block_blob blockBlob = container.get_block_blob_reference(blobName);

	// Create or overwrite the "my-blob-1" blob with contents from a local file.
	Concurrency::streams::istream input_stream = Concurrency::streams::file_stream<uint8_t>::open_istream(localFileName).get();
	blockBlob.upload_from_stream(input_stream);
	input_stream.close().wait();
}

///	<summary>
///	Upload a blob to the container.
///	</summary>
/// <param name="containerName">The container name.</param>
/// <param name="blobName">The blob name.</param>
/// <param name="inputStream">The input stream containing the file to upload.</param>
/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
Concurrency::task<void> BlobCloudClient::UploadFileBlobAsync(const utility::string_t& containerName, const utility::string_t& blobName, Concurrency::streams::istream& inputStream)
{
	// Retrieve a reference to a previously created container.
	azure::storage::cloud_blob_container container = _client.get_container_reference(containerName);

	// Retrieve reference to a blob named "my-blob-1".
	azure::storage::cloud_block_blob blockBlob = container.get_block_blob_reference(blobName);

	// Create or overwrite the "my-blob-1" blob with contents from a local file.
	return blockBlob.upload_from_stream_async(inputStream);
}

///	<summary>
///	Upload a blob to the container.
///	</summary>
/// <param name="containerName">The container name.</param>
/// <param name="blobName">The blob name.</param>
/// <param name="text">The text to upload.</param>
void BlobCloudClient::UploadTextBlob(const utility::string_t& containerName, const utility::string_t& blobName, const utility::string_t& text)
{
	// Retrieve a reference to a previously created container.
	azure::storage::cloud_blob_container container = _client.get_container_reference(containerName);

	// Retrieve reference to a blob named "my-blob-1".
	azure::storage::cloud_block_blob blockBlob = container.get_block_blob_reference(blobName);
	blockBlob.upload_text(text);
}
///	<summary>
///	Upload a blob to the container.
///	</summary>
/// <param name="containerName">The container name.</param>
/// <param name="blobName">The blob name.</param>
/// <param name="text">The text to upload.</param>
/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
Concurrency::task<void> BlobCloudClient::UploadTextBlobAsync(const utility::string_t& containerName, const utility::string_t& blobName, const utility::string_t& text)
{
	// Retrieve a reference to a previously created container.
	azure::storage::cloud_blob_container container = _client.get_container_reference(containerName);

	// Retrieve reference to a blob named "my-blob-1".
	azure::storage::cloud_block_blob blockBlob = container.get_block_blob_reference(blobName);
	return blockBlob.upload_text_async(text);
}

///	<summary>
///	Download a blob from the container.
///	</summary>
/// <param name="containerName">The container name.</param>
/// <param name="blobName">The blob name.</param>
/// <returns>The text within the blob.</returns>
utility::string_t BlobCloudClient::DownloadTextBlob(const utility::string_t& containerName, const utility::string_t& blobName)
{
	// Retrieve a reference to a previously created container.
	azure::storage::cloud_blob_container container = _client.get_container_reference(containerName);

	// Retrieve reference to a blob named "my-blob-1".
	azure::storage::cloud_block_blob blockBlob = container.get_block_blob_reference(blobName);
	return blockBlob.download_text();
}

///	<summary>
///	Download a blob from the container.
///	</summary>
/// <param name="containerName">The container name.</param>
/// <param name="blobName">The blob name.</param>
/// <returns>The text within the blob.</returns>
Concurrency::task<utility::string_t> BlobCloudClient::DownloadTextBlobAsync(const utility::string_t& containerName, const utility::string_t& blobName)
{
	// Retrieve a reference to a previously created container.
	azure::storage::cloud_blob_container container = _client.get_container_reference(containerName);

	// Retrieve reference to a blob named "my-blob-1".
	azure::storage::cloud_block_blob blockBlob = container.get_block_blob_reference(blobName);
	return blockBlob.download_text_async();
}

///	<summary>
///	Download a blob from the container.
///	</summary>
/// <param name="containerName">The container name.</param>
/// <param name="blobName">The blob name.</param>
/// <param name="localFileName">The path and name of the local file to download to.</param>
void BlobCloudClient::DownloadFileBlob(const utility::string_t& containerName, const utility::string_t& blobName, const utility::string_t& localFileName)
{
	// Retrieve a reference to a previously created container.
	azure::storage::cloud_blob_container container = _client.get_container_reference(containerName);

	// Retrieve reference to a blob named "my-blob-1".
	azure::storage::cloud_block_blob blockBlob = container.get_block_blob_reference(blobName);

	// Save blob contents to a file.
	concurrency::streams::container_buffer<std::vector<uint8_t>> buffer;
	concurrency::streams::ostream output_stream(buffer);
	blockBlob.download_to_stream(output_stream);

	std::ofstream outfile(localFileName, std::ofstream::binary);
	std::vector<unsigned char>& data = buffer.collection();

	outfile.write((char *)&data[0], buffer.size());
	outfile.close();
}

///	<summary>
///	Download a blob from the container.
///	</summary>
/// <param name="containerName">The container name.</param>
/// <param name="blobName">The blob name.</param>
/// <param name="outputStream">The output stream containing the file to download.</param>
void BlobCloudClient::DownloadFileBlob(const utility::string_t& containerName, const utility::string_t& blobName, Concurrency::streams::ostream& outputStream)
{
	// Retrieve a reference to a previously created container.
	azure::storage::cloud_blob_container container = _client.get_container_reference(containerName);

	// Retrieve reference to a blob named "my-blob-1".
	azure::storage::cloud_block_blob blockBlob = container.get_block_blob_reference(blobName);

	// Save blob contents to a file.
	blockBlob.download_to_stream(outputStream);
}

///	<summary>
///	Download a blob from the container.
///	</summary>
/// <param name="containerName">The container name.</param>
/// <param name="blobName">The blob name.</param>
/// <param name="outputStream">The output stream containing the file to download.</param>
/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
Concurrency::task<void> BlobCloudClient::DownloadFileBlobAsync(const utility::string_t& containerName, const utility::string_t& blobName, Concurrency::streams::ostream& outputStream)
{
	// Retrieve a reference to a previously created container.
	azure::storage::cloud_blob_container container = _client.get_container_reference(containerName);

	// Retrieve reference to a blob named "my-blob-1".
	azure::storage::cloud_block_blob blockBlob = container.get_block_blob_reference(blobName);

	// Save blob contents to a file.
	return blockBlob.download_to_stream_async(outputStream);
}

///	<summary>
///	Download a blob from the container.
///	</summary>
/// <param name="containerName">The container name.</param>
/// <param name="blobName">The blob name.</param>
/// <param name="localFileName">The path and name of the local file to download to.</param>
/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
Concurrency::task<void> BlobCloudClient::DownloadFileBlobAsync(const utility::string_t& containerName, const utility::string_t& blobName, const utility::string_t& localFileName)
{
	// Retrieve a reference to a previously created container.
	azure::storage::cloud_blob_container container = _client.get_container_reference(containerName);

	// Retrieve reference to a blob named "my-blob-1".
	azure::storage::cloud_block_blob blockBlob = container.get_block_blob_reference(blobName);

	// Save blob contents to a file.
	return blockBlob.download_to_file_async(localFileName);
}

///	<summary>
///	Get the list if blob items.
///	</summary>
/// <param name="prefix">The blob name prefix.</param>
/// <returns>The list of blob items.</returns>
const std::vector<azure::storage::list_blob_item> BlobCloudClient::ListBlobs(const utility::string_t& prefix) const
{
	std::vector<azure::storage::list_blob_item> items;

	// Get the list.
	auto itemIterator = _client.list_blobs(prefix);
	
	// Iterate through the list.
	for (azure::storage::list_blob_item item : itemIterator)
	{
		// Add the item.
		items.push_back(item);
	}

	// Return the items.
	return items;
}

///	<summary>
///	Get the list if blob items.
///	</summary>
/// <param name="containerName">The container name the blobs are in.</param>
/// <returns>The list of blob items.</returns>
const std::vector<utility::string_t> BlobCloudClient::ListBlobsInContainer(const utility::string_t& containerName) const
{
	std::vector<utility::string_t> items;

	// Retrieve a reference to a previously created container.
	azure::storage::cloud_blob_container container = _client.get_container_reference(containerName);

	// Output URI of each item.
	azure::storage::list_blob_item_iterator end_of_results;

	// For each blob.
	for (auto it = container.list_blobs(); it != end_of_results; ++it)
	{
		// If is blob.
		if (it->is_blob())
		{
			// Add the blob.
			items.push_back(it->as_blob().uri().primary_uri().to_string());
		}
	}

	// Return the items.
	return items;
}

///	<summary>
///	Get the list if directory items.
///	</summary>
/// <param name="containerName">The container name the directories are in.</param>
/// <returns>The list of directory items.</returns>
const std::vector<utility::string_t> BlobCloudClient::ListDirectoriesInContainer(const utility::string_t& containerName) const
{
	std::vector<utility::string_t> items;

	// Retrieve a reference to a previously created container.
	azure::storage::cloud_blob_container container = _client.get_container_reference(containerName);

	// Output URI of each item.
	azure::storage::list_blob_item_iterator end_of_results;

	// For each blob.
	for (auto it = container.list_blobs(); it != end_of_results; ++it)
	{
		// If is not blob.
		if (!it->is_blob())
		{
			// Add the directory.
			items.push_back(it->as_directory().uri().primary_uri().to_string());
		}
	}

	// Return the items.
	return items;
}

/// <summary>
/// Intitiates an asynchronous operation to return an <see cref="azure::storage::list_blob_item_segment"/> containing a collection of blob items in the container.
/// </summary>
/// <param name="prefix">The blob name prefix.</param>
/// <param name="token">An <see cref="azure::storage::continuation_token" /> returned by a previous listing operation.</param>
/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::list_blob_item_segment"/> that represents the current operation.</returns>
Concurrency::task<azure::storage::list_blob_item_segment> BlobCloudClient::ListBlobsSegmentedAsync(
	const utility::string_t& prefix, const azure::storage::continuation_token& token) const
{
	return _client.list_blobs_segmented_async(prefix, token);
}

///	<summary>
///	Get the list if container items.
///	</summary>
/// <param name="prefix">The container name prefix.</param>
/// <returns>The list of container items.</returns>
const std::vector<azure::storage::cloud_blob_container> BlobCloudClient::ListContainers(const utility::string_t& prefix) const
{
	std::vector<azure::storage::cloud_blob_container> items;

	// Get the list.
	auto itemIterator = _client.list_containers(prefix);

	// Iterate through the list.
	for (azure::storage::cloud_blob_container item : itemIterator)
	{
		// Add the item.
		items.push_back(item);
	}
	
	// Return the items.
	return items;
}

/// <summary>
/// Intitiates an asynchronous operation to return a result segment containing a collection of <see cref="azure::storage::cloud_blob_container"/> objects.
/// </summary>
/// <param name="prefix">The container name prefix.</param>
/// <param name="token">An <see cref="azure::storage::continuation_token"/> returned by a previous listing operation.</param>
/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::container_result_segment"/> that represents the current operation.</returns>
Concurrency::task<azure::storage::container_result_segment> BlobCloudClient::ListContainersSegmentedAsync(
	const utility::string_t& prefix, const azure::storage::continuation_token& token) const
{
	return _client.list_containers_segmented_async(prefix, token);
}

/// <summary>
/// Intitiates an asynchronous operation to set the service properties for the Blob service client.
/// </summary>
/// <param name="properties">The <see cref="azure::storage::service_properties"/> for the Blob service client.</param>
/// <param name="includes">An <see cref="azure::storage::service_properties_includes"/> enumeration describing which items to include when setting service properties.</param>
/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
Concurrency::task<void> BlobCloudClient::UploadServicePropertiesAsync(
	const azure::storage::service_properties& properties,
	const azure::storage::service_properties_includes& includes) const
{
	return _client.upload_service_properties_async(properties, includes);
}

/// <summary>
/// Intitiates an asynchronous operation to get the properties of the service.
/// </summary>
/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::service_properties"/> that represents the current operation.</returns>
Concurrency::task<azure::storage::service_properties> BlobCloudClient::DownloadServicePropertiesAsync() const
{
	return _client.download_service_properties_async();
}

/// <summary>
/// Intitiates an asynchronous operation to get the properties of the service.
/// </summary>
/// <param name="options">An <see cref="azure::storage::blob_request_options"/> object that specifies additional options for the request.</param>
/// <param name="context">An <see cref="azure::storage::operation_context"/> object that represents the context for the current operation.</param>
/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::service_properties"/> that represents the current operation.</returns>
Concurrency::task<azure::storage::service_properties> BlobCloudClient::DownloadServicePropertiesAsync(
	const azure::storage::blob_request_options& options, azure::storage::operation_context context) const
{
	return _client.download_service_properties_async(options, context);
}