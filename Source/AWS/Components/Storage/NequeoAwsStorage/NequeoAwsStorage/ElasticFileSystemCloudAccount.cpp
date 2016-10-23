/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          ElasticFileSystemCloudAccount.cpp
*  Purpose :       Cloud Front Cloud account provider class.
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

#include "ElasticFileSystemCloudAccount.h"

using namespace Nequeo::AWS::Storage;

static const char* ELASTICFILESYSTEM_CLIENT_TAG = "NequeoElasticFileSystemClient";

///	<summary>
///	Cloud account provider.
///	</summary>
/// <param name="account">The AWS services account.</param>
ElasticFileSystemCloudAccount::ElasticFileSystemCloudAccount(const AwsAccount& account) : _disposed(false), _account(account)
{
	// Create the client.
	_client = Aws::MakeUnique<Aws::EFS::EFSClient>(ELASTICFILESYSTEM_CLIENT_TAG, _account._credentials, _account._clientConfiguration);
}

///	<summary>
///	Cloud account provider.
///	</summary>
ElasticFileSystemCloudAccount::~ElasticFileSystemCloudAccount()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Gets the Elastic File System client.
/// </summary>
/// <return>The Elastic File System client.</return>
const Aws::EFS::EFSClient& ElasticFileSystemCloudAccount::GetClient() const
{
	return *(_client.get());
}

///	<summary>
///	Get the service URI.
///	</summary>
///	<return>The service URI.</return>
std::string ElasticFileSystemCloudAccount::GetServiceUri()
{
	return std::string(Aws::EFS::EFSEndpoint::ForRegion(_account._clientConfiguration.region).c_str());
}

///	<summary>
///	Create the file system asynchronously.
///	</summary>
/// <param name="request">The file system request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void ElasticFileSystemCloudAccount::CreateFileSystemAsync(
	const Aws::EFS::Model::CreateFileSystemRequest& request,
	const Aws::EFS::CreateFileSystemResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->CreateFileSystemAsync(request, handler, context);
}

///	<summary>
///	Create the file system asynchronously.
///	</summary>
/// <param name="creationToken">The file system creation token.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void ElasticFileSystemCloudAccount::CreateFileSystemAsync(
	const std::string& creationToken,
	const Aws::EFS::CreateFileSystemResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	Aws::EFS::Model::CreateFileSystemRequest request;
	request.SetCreationToken(Aws::String(creationToken.c_str()));
	_client->CreateFileSystemAsync(request, handler, context);
}

///	<summary>
///	Create the mount target asynchronously.
///	</summary>
/// <param name="request">The mount target request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void ElasticFileSystemCloudAccount::CreateMountTargetAsync(
	const Aws::EFS::Model::CreateMountTargetRequest& request,
	const Aws::EFS::CreateMountTargetResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->CreateMountTargetAsync(request, handler, context);
}

///	<summary>
///	Create the tags asynchronously.
///	</summary>
/// <param name="request">The rags request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void ElasticFileSystemCloudAccount::CreateTagsAsync(
	const Aws::EFS::Model::CreateTagsRequest& request,
	const Aws::EFS::CreateTagsResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->CreateTagsAsync(request, handler, context);
}

///	<summary>
///	Delete the file system asynchronously.
///	</summary>
/// <param name="request">The file system request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void ElasticFileSystemCloudAccount::DeleteFileSystemAsync(
	const Aws::EFS::Model::DeleteFileSystemRequest& request,
	const Aws::EFS::DeleteFileSystemResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->DeleteFileSystemAsync(request, handler, context);
}

///	<summary>
///	Delete the mount target asynchronously.
///	</summary>
/// <param name="request">The mount target request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void ElasticFileSystemCloudAccount::DeleteMountTargetAsync(
	const Aws::EFS::Model::DeleteMountTargetRequest& request,
	const Aws::EFS::DeleteMountTargetResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->DeleteMountTargetAsync(request, handler, context);
}

///	<summary>
///	Delete the tags asynchronously.
///	</summary>
/// <param name="request">The rags request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void ElasticFileSystemCloudAccount::DeleteTagsAsync(
	const Aws::EFS::Model::DeleteTagsRequest& request,
	const Aws::EFS::DeleteTagsResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->DeleteTagsAsync(request, handler, context);
}

///	<summary>
///	Describe the file system asynchronously.
///	</summary>
/// <param name="request">The file system request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void ElasticFileSystemCloudAccount::DescribeFileSystemsAsync(
	const Aws::EFS::Model::DescribeFileSystemsRequest& request,
	const Aws::EFS::DescribeFileSystemsResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->DescribeFileSystemsAsync(request, handler, context);
}