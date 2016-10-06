/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          AccessManagementCloudAccount.cpp
*  Purpose :       Manage User Access and Encryption Keys account provider class.
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

#include "AccessManagementCloudAccount.h"

using namespace Nequeo::AWS::Management;

static const char* ACCESSMANAGEMENT_CLIENT_TAG = "NequeoAccessManagementClient";

///	<summary>
///	Cloud account provider.
///	</summary>
/// <param name="account">The AWS services account.</param>
/// <param name="iamClient">The AWS IAM client.</param>
/// <param name="cognitoClient">The AWS Cognito Identity Client.</param>
AccessManagementCloudAccount::AccessManagementCloudAccount(
	const AwsAccount& account, 
	std::shared_ptr<Aws::IAM::IAMClient>& iamClient,
	std::shared_ptr<Aws::CognitoIdentity::CognitoIdentityClient>& cognitoClient) : _disposed(false), _account(account)
{
	// Create the client.
	_client = Aws::MakeUnique<Aws::AccessManagement::AccessManagementClient>(ACCESSMANAGEMENT_CLIENT_TAG, iamClient, cognitoClient);
}

///	<summary>
///	Cloud account provider.
///	</summary>
AccessManagementCloudAccount::~AccessManagementCloudAccount()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Gets the Manage User Access and Encryption Keys client.
/// </summary>
/// <return>The Manage User Access and Encryption Keys client.</return>
const Aws::AccessManagement::AccessManagementClient& AccessManagementCloudAccount::GetClient() const
{
	return *(_client.get());
}