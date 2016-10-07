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