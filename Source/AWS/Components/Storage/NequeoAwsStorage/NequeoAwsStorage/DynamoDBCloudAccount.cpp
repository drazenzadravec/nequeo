/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          DynamoDBCloudAccount.cpp
*  Purpose :       DynamoDB Cloud account provider class.
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

#include "DynamoDBCloudAccount.h"

using namespace Nequeo::Aws::Storage;

static const char* DYNAMODB_CLIENT_TAG = "NequeoDynamoDBClient";

///	<summary>
///	Cloud account provider.
///	</summary>
/// <param name="account">The AWS services account.</param>
DynamoDBCloudAccount::DynamoDBCloudAccount(const AwsAccount& account) : _disposed(false), _account(account)
{
	// Create the client.
	_client = AWS::MakeUnique<AWS::DynamoDB::DynamoDBClient>(DYNAMODB_CLIENT_TAG, _account._credentials, _account._clientConfiguration);
}

///	<summary>
///	Cloud account provider.
///	</summary>
DynamoDBCloudAccount::~DynamoDBCloudAccount()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	Get the service URI.
///	</summary>
///	<return>The service URI.</return>
std::string DynamoDBCloudAccount::GetServiceUri()
{
	return std::string(AWS::DynamoDB::DynamoDBEndpoint::ForRegion(_account._clientConfiguration.region).c_str());
}

///	<summary>
///	Create the table asynchronously.
///	</summary>
/// <param name="request">The table request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void DynamoDBCloudAccount::CreateTableAsync(
	const AWS::DynamoDB::Model::CreateTableRequest& request,
	const AWS::DynamoDB::CreateTableResponseReceivedHandler& handler,
	const std::shared_ptr<const AWS::Client::AsyncCallerContext>& context)
{
	// Create the table.
	_client->CreateTableAsync(request, handler, context);
}

///	<summary>
///	Scan for items asynchronously.
///	</summary>
/// <param name="request">The scan request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void DynamoDBCloudAccount::ScanAsync(
	const AWS::DynamoDB::Model::ScanRequest& request,
	const AWS::DynamoDB::ScanResponseReceivedHandler& handler,
	const std::shared_ptr<const AWS::Client::AsyncCallerContext>& context)
{
	_client->ScanAsync(request, handler, context);
}