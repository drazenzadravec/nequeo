/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          TableCloudClient.cpp
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

#include "TableCloudClient.h"

using namespace Nequeo::Azure::Storage;

///	<summary>
///	Cloud client provider.
///	</summary>
/// <param name="account">The Azure account.</param>
TableCloudClient::TableCloudClient(const AzureAccount& account) :  _disposed(false), _isInitialised(false), _account(account)
{
}

///	<summary>
///	Cloud client provider.
///	</summary>
TableCloudClient::~TableCloudClient()
{
	if (!_disposed)
	{
		_disposed = true;
		_isInitialised = false;
	}
}

///	<summary>
///	Initialise the Table client.
///	</summary>
void TableCloudClient::Initialise()
{
	// If not initialised;
	if (!_isInitialised)
	{
		_client = _account._account.create_cloud_table_client();
		_isInitialised = true;
	}
}

///	<summary>
///	Initialise the Table client.
///	</summary>
/// <param name="default_request_options">Default table request options.</param>
void TableCloudClient::Initialise(const azure::storage::table_request_options& default_request_options)
{
	// If not initialised;
	if (!_isInitialised)
	{
		_client = _account._account.create_cloud_table_client(default_request_options);
		_isInitialised = true;
	}
}