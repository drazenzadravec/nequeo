/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          BlobCloudClient.h
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

#include <was\blob.h>

namespace Nequeo {
	namespace Azure {
		namespace Storage
		{
			///	<summary>
			///	Cloud client provider.
			///	</summary>
			class EXPORT_NEQUEO_AZURE_STORAGE_API BlobCloudClient
			{
			public:
				///	<summary>
				///	Cloud client provider.
				///	</summary>
				/// <param name="account">The Azure account.</param>
				BlobCloudClient(const AzureAccount& account);

				///	<summary>
				///	Cloud client provider destructor.
				///	</summary>
				~BlobCloudClient();

				/// <summary>
				/// Has the client been initialised.
				/// </summary>
				/// <returns>True if is initialised; else false.</returns>
				inline bool IsInitialised() const
				{
					return _isInitialised;
				}

				///	<summary>
				///	Get the blob client.
				///	</summary>
				/// <returns>The blob client.</returns>
				const azure::storage::cloud_blob_client& BlobClient() const;

				///	<summary>
				///	Initialise the Blob client.
				///	</summary>
				void Initialise();

				///	<summary>
				///	Initialise the Blob client.
				///	</summary>
				/// <param name="default_request_options">Default blob request options.</param>
				void Initialise(const azure::storage::blob_request_options& default_request_options);

				///	<summary>
				///	Create a container.
				///	</summary>
				/// <param name="containerName">The container name to create.</param>
				/// <returns>True if created; else false.</returns>
				bool CreateContainer(const utility::string_t& containerName);

				///	<summary>
				///	Delete a container.
				///	</summary>
				/// <param name="containerName">The container name to delete.</param>
				/// <returns>True if deleted; else false.</returns>
				bool DeleteContainer(const utility::string_t& containerName);

				///	<summary>
				///	Delete a blob.
				///	</summary>
				/// <param name="containerName">The container name to delete.</param>
				/// <param name="blobName">The blob name to delete.</param>
				/// <returns>True if deleted; else false.</returns>
				bool DeleteBlob(const utility::string_t& containerName, const utility::string_t& blobName);

				///	<summary>
				///	Upload a blob to the container.
				///	</summary>
				/// <param name="containerName">The container name.</param>
				/// <param name="blobName">The blob name.</param>
				/// <param name="localFileName">The path and name of the local file to upload.</param>
				void UploadFileBlob(const utility::string_t& containerName, const utility::string_t& blobName, const utility::string_t& localFileName);

				///	<summary>
				///	Upload a blob to the container.
				///	</summary>
				/// <param name="containerName">The container name.</param>
				/// <param name="blobName">The blob name.</param>
				/// <param name="inputStream">The input stream containing the file to upload.</param>
				/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
				Concurrency::task<void> UploadFileBlobAsync(const utility::string_t& containerName, const utility::string_t& blobName, Concurrency::streams::istream& inputStream);

				///	<summary>
				///	Upload a blob to the container.
				///	</summary>
				/// <param name="containerName">The container name.</param>
				/// <param name="blobName">The blob name.</param>
				/// <param name="text">The text to upload.</param>
				void UploadTextBlob(const utility::string_t& containerName, const utility::string_t& blobName, const utility::string_t& text);

				///	<summary>
				///	Upload a blob to the container.
				///	</summary>
				/// <param name="containerName">The container name.</param>
				/// <param name="blobName">The blob name.</param>
				/// <param name="text">The text to upload.</param>
				/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
				Concurrency::task<void> UploadTextBlobAsync(const utility::string_t& containerName, const utility::string_t& blobName, const utility::string_t& text);

				///	<summary>
				///	Download a blob from the container.
				///	</summary>
				/// <param name="containerName">The container name.</param>
				/// <param name="blobName">The blob name.</param>
				/// <returns>The text within the blob.</returns>
				utility::string_t DownloadTextBlob(const utility::string_t& containerName, const utility::string_t& blobName);

				///	<summary>
				///	Download a blob from the container.
				///	</summary>
				/// <param name="containerName">The container name.</param>
				/// <param name="blobName">The blob name.</param>
				/// <returns>The text within the blob.</returns>
				Concurrency::task<utility::string_t> DownloadTextBlobAsync(const utility::string_t& containerName, const utility::string_t& blobName);

				///	<summary>
				///	Download a blob from the container.
				///	</summary>
				/// <param name="containerName">The container name.</param>
				/// <param name="blobName">The blob name.</param>
				/// <param name="localFileName">The path and name of the local file to download to.</param>
				void DownloadFileBlob(const utility::string_t& containerName, const utility::string_t& blobName, const utility::string_t& localFileName);

				///	<summary>
				///	Download a blob from the container.
				///	</summary>
				/// <param name="containerName">The container name.</param>
				/// <param name="blobName">The blob name.</param>
				/// <param name="outputStream">The output stream containing the file to download.</param>
				void DownloadFileBlob(const utility::string_t& containerName, const utility::string_t& blobName, Concurrency::streams::ostream& outputStream);

				///	<summary>
				///	Download a blob from the container.
				///	</summary>
				/// <param name="containerName">The container name.</param>
				/// <param name="blobName">The blob name.</param>
				/// <param name="outputStream">The output stream containing the file to download.</param>
				/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
				Concurrency::task<void> DownloadFileBlobAsync(const utility::string_t& containerName, const utility::string_t& blobName, Concurrency::streams::ostream& outputStream);

				///	<summary>
				///	Download a blob from the container.
				///	</summary>
				/// <param name="containerName">The container name.</param>
				/// <param name="blobName">The blob name.</param>
				/// <param name="localFileName">The path and name of the local file to download to.</param>
				/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
				Concurrency::task<void> DownloadFileBlobAsync(const utility::string_t& containerName, const utility::string_t& blobName, const utility::string_t& localFileName);

				///	<summary>
				///	Get the list if blob items.
				///	</summary>
				/// <param name="prefix">The blob name prefix.</param>
				/// <returns>The list of blob items.</returns>
				const std::vector<azure::storage::list_blob_item> ListBlobs(const utility::string_t& prefix) const;

				///	<summary>
				///	Get the list if blob items.
				///	</summary>
				/// <param name="containerName">The container name the blobs are in.</param>
				/// <returns>The list of blob items.</returns>
				const std::vector<utility::string_t> ListBlobsInContainer(const utility::string_t& containerName) const;

				///	<summary>
				///	Get the list if directory items.
				///	</summary>
				/// <param name="containerName">The container name the directories are in.</param>
				/// <returns>The list of directory items.</returns>
				const std::vector<utility::string_t> ListDirectoriesInContainer(const utility::string_t& containerName) const;

				/// <summary>
				/// Intitiates an asynchronous operation to return an <see cref="azure::storage::list_blob_item_segment"/> containing a collection of blob items in the container.
				/// </summary>
				/// <param name="prefix">The blob name prefix.</param>
				/// <param name="token">An <see cref="azure::storage::continuation_token" /> returned by a previous listing operation.</param>
				/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::list_blob_item_segment"/> that represents the current operation.</returns>
				Concurrency::task<azure::storage::list_blob_item_segment> ListBlobsSegmentedAsync(
					const utility::string_t& prefix, const azure::storage::continuation_token& token) const;

				///	<summary>
				///	Get the list if container items.
				///	</summary>
				/// <param name="prefix">The container name prefix.</param>
				/// <returns>The list of container items.</returns>
				const std::vector<azure::storage::cloud_blob_container> ListContainers(const utility::string_t& prefix) const;

				/// <summary>
				/// Intitiates an asynchronous operation to return a result segment containing a collection of <see cref="azure::storage::cloud_blob_container"/> objects.
				/// </summary>
				/// <param name="prefix">The container name prefix.</param>
				/// <param name="token">An <see cref="azure::storage::continuation_token"/> returned by a previous listing operation.</param>
				/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::container_result_segment"/> that represents the current operation.</returns>
				Concurrency::task<azure::storage::container_result_segment> ListContainersSegmentedAsync(
					const utility::string_t& prefix, const azure::storage::continuation_token& token) const;

				/// <summary>
				/// Intitiates an asynchronous operation to set the service properties for the Blob service client.
				/// </summary>
				/// <param name="properties">The <see cref="azure::storage::service_properties"/> for the Blob service client.</param>
				/// <param name="includes">An <see cref="azure::storage::service_properties_includes"/> enumeration describing which items to include when setting service properties.</param>
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
				/// <param name="options">An <see cref="azure::storage::blob_request_options"/> object that specifies additional options for the request.</param>
				/// <param name="context">An <see cref="azure::storage::operation_context"/> object that represents the context for the current operation.</param>
				/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::service_properties"/> that represents the current operation.</returns>
				Concurrency::task<azure::storage::service_properties> DownloadServicePropertiesAsync(
					const azure::storage::blob_request_options& options, azure::storage::operation_context context) const;

			private:
				bool _disposed;
				bool _isInitialised;

				AzureAccount _account;
				azure::storage::cloud_blob_client _client;
			};
		}
	}
}