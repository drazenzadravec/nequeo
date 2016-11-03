/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          AzureAccount.cpp
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

#include "AzureAccount.h"

using namespace Nequeo::Azure::Storage;

///	<summary>
///	Azure account client provider.
///	</summary>
/// <param name="credentials">The storage credentials to use.</param>
/// <param name="endpoint_suffix">The DNS endpoint suffix for the storage services, e.g., &quot;core.windows.net&quot;.</param>
/// <param name="useHttps"><c>true</c> to use HTTPS to connect to storage service endpoints; otherwise, <c>false</c>.</param>
AzureAccount::AzureAccount(const azure::storage::storage_credentials& credentials, const utility::string_t& endpoint_suffix, bool useHttps)
	: _disposed(false), _credentials(credentials), _endpoint_suffix(endpoint_suffix), _useHttps(useHttps), 
	_account(credentials, endpoint_suffix, useHttps)
{
}

/// <summary>
/// Initializes a new instance of the <see cref="azure::storage::cloud_storage_account" /> class using the specified
/// credentials and service endpoints.
/// </summary>
/// <param name="credentials">The <see cref="azure::storage::storage_credentials" /> to use.</param>
/// <param name="blob_endpoint">The Blob service endpoint.</param>
/// <param name="queue_endpoint">The Queue service endpoint.</param>
/// <param name="table_endpoint">The Table service endpoint.</param>
AzureAccount::AzureAccount(
	const azure::storage::storage_credentials& credentials,
	const azure::storage::storage_uri& blob_endpoint,
	const azure::storage::storage_uri& queue_endpoint,
	const azure::storage::storage_uri& table_endpoint)
	: _disposed(false), _credentials(credentials), _blob_endpoint(blob_endpoint), _queue_endpoint(queue_endpoint), _table_endpoint(table_endpoint),
	_account(credentials, blob_endpoint, queue_endpoint, table_endpoint)
{
}

/// <summary>
/// Initializes a new instance of the <see cref="azure::storage::cloud_storage_account" /> class using the specified
/// credentials and service endpoints.
/// </summary>
/// <param name="credentials">The <see cref="azure::storage::storage_credentials" /> to use.</param>
/// <param name="blob_endpoint">The Blob service endpoint.</param>
/// <param name="queue_endpoint">The Queue service endpoint.</param>
/// <param name="table_endpoint">The Table service endpoint.</param>
/// <param name="file_endpoint">The File service endpoint.</param>
AzureAccount::AzureAccount(
	const azure::storage::storage_credentials& credentials,
	const azure::storage::storage_uri& blob_endpoint,
	const azure::storage::storage_uri& queue_endpoint,
	const azure::storage::storage_uri& table_endpoint,
	const azure::storage::storage_uri& file_endpoint)
	: _disposed(false), _credentials(credentials), _blob_endpoint(blob_endpoint), _queue_endpoint(queue_endpoint), _table_endpoint(table_endpoint), _file_endpoint(file_endpoint),
	_account(credentials, blob_endpoint, queue_endpoint, table_endpoint, file_endpoint)
{

}

///	<summary>
///	Azure account client provider.
///	</summary>
/// <param name="connection_string">The account connection string.</param>
AzureAccount::AzureAccount(const utility::string_t& connection_string) : _disposed(false)
{
	// Retrieve the storage account from the connection string.
	_account = azure::storage::cloud_storage_account::parse(connection_string);
}

///	<summary>
///	Azure account client provider.
///	</summary>
AzureAccount::~AzureAccount()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}