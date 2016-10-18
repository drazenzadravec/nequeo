/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          AzureAccount.h
*  Purpose :       Azure account client provider class.
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

namespace Nequeo {
	namespace Azure {
		namespace Storage
		{
			///	<summary>
			///	Azure account client provider.
			///	</summary>
			class EXPORT_NEQUEO_AZURE_STORAGE_API AzureAccount
			{
			public:
				///	<summary>
				///	Azure account client provider.
				///	</summary>
				/// <param name="credentials">The storage credentials to use.</param>
				/// <param name="endpoint_suffix">The DNS endpoint suffix for the storage services, e.g., &quot;core.windows.net&quot;.</param>
				/// <param name="useHttps"><c>true</c> to use HTTPS to connect to storage service endpoints; otherwise, <c>false</c>.</param>
				AzureAccount(
					const azure::storage::storage_credentials& credentials, 
					const utility::string_t& endpoint_suffix, 
					bool useHttps = true);

				/// <summary>
				/// Initializes a new instance of the <see cref="azure::storage::cloud_storage_account" /> class using the specified
				/// credentials and service endpoints.
				/// </summary>
				/// <param name="credentials">The <see cref="azure::storage::storage_credentials" /> to use.</param>
				/// <param name="blob_endpoint">The Blob service endpoint.</param>
				/// <param name="queue_endpoint">The Queue service endpoint.</param>
				/// <param name="table_endpoint">The Table service endpoint.</param>
				AzureAccount(
					const azure::storage::storage_credentials& credentials,
					const azure::storage::storage_uri& blob_endpoint, 
					const azure::storage::storage_uri& queue_endpoint, 
					const azure::storage::storage_uri& table_endpoint);

				/// <summary>
				/// Initializes a new instance of the <see cref="azure::storage::cloud_storage_account" /> class using the specified
				/// credentials and service endpoints.
				/// </summary>
				/// <param name="credentials">The <see cref="azure::storage::storage_credentials" /> to use.</param>
				/// <param name="blob_endpoint">The Blob service endpoint.</param>
				/// <param name="queue_endpoint">The Queue service endpoint.</param>
				/// <param name="table_endpoint">The Table service endpoint.</param>
				/// <param name="file_endpoint">The File service endpoint.</param>
				AzureAccount(
					const azure::storage::storage_credentials& credentials,
					const azure::storage::storage_uri& blob_endpoint,
					const azure::storage::storage_uri& queue_endpoint,
					const azure::storage::storage_uri& table_endpoint,
					const azure::storage::storage_uri& file_endpoint);

				///	<summary>
				///	Azure account client provider.
				///	</summary>
				/// <param name="connection_string">The account connection string.</param>
				AzureAccount(
					const utility::string_t& connection_string);

				///	<summary>
				///	Azure account client provider destructor.
				///	</summary>
				~AzureAccount();

				/// <summary>
				/// Gets the endpoint for the Blob service for all location.
				/// </summary>
				/// <returns>An <see cref="azure::storage::storage_uri" /> object containing the Blob service endpoint for all locations.</returns>
				inline const azure::storage::storage_uri& BlobEndpoint() const
				{
					return _blob_endpoint;
				}

				/// <summary>
				/// Gets the endpoint for the Queue service for all location.
				/// </summary>
				/// <returns>An <see cref="azure::storage::storage_uri" /> object containing the Queue service endpoint for all locations.</returns>
				inline const azure::storage::storage_uri& QueueEndpoint() const
				{
					return _queue_endpoint;
				}

				/// <summary>
				/// Gets the endpoint for the Table service for all location.
				/// </summary>
				/// <returns>An <see cref="azure::storage::storage_uri" /> object containing the Table service endpoint for all locations.</returns>
				inline const azure::storage::storage_uri& TableEndpoint() const
				{
					return _table_endpoint;
				}

				/// <summary>
				/// Gets the endpoint for the File service for all location.
				/// </summary>
				/// <returns>An <see cref="azure::storage::storage_uri" /> object containing the File service endpoint for all locations.</returns>
				inline const azure::storage::storage_uri& FileEndpoint() const
				{
					return _file_endpoint;
				}

				/// <summary>
				/// Gets the credentials used to create this object.
				/// </summary>
				/// <returns>The credentials used to create the object.</returns>
				inline const azure::storage::storage_credentials& Credentials() const
				{
					return _credentials;
				}

				/// <summary>
				/// Gets the endpoint suffix.
				/// </summary>
				/// <returns>The DNS endpoint suffix for the storage services, e.g., &quot;core.windows.net&quot;.</returns>
				inline const utility::string_t& EndpointSuffix() const
				{
					return _endpoint_suffix;
				}

				/// <summary>
				/// Gets the use HTTPS indicator.
				/// </summary>
				/// <returns><c>true</c> to use HTTPS to connect to storage service endpoints; otherwise, <c>false</c>.</returns>
				inline bool UseHttps() const
				{
					return _useHttps;
				}

			private:
				bool _disposed;
				bool _useHttps;

				utility::string_t _endpoint_suffix;
				azure::storage::storage_credentials _credentials;
				azure::storage::cloud_storage_account _account;

				azure::storage::storage_uri _blob_endpoint;
				azure::storage::storage_uri _queue_endpoint;
				azure::storage::storage_uri _table_endpoint;
				azure::storage::storage_uri _file_endpoint;

				friend class BlobCloudClient;
				friend class QueueCloudClient;
				friend class TableCloudClient;
				friend class FileCloudClient;
			};
		}
	}
}