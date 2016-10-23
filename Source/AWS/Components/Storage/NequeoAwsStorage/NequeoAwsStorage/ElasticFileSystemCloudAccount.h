/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          ElasticFileSystemCloudAccount.h
*  Purpose :       Elastic File System Cloud account provider class.
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
#include "AwsAccount.h"

#undef IN
#undef GetObject

#include <aws\elasticfilesystem\EFSClient.h>
#include <aws\elasticfilesystem\EFSEndpoint.h>
#include <aws\elasticfilesystem\model\CreateFileSystemRequest.h>
#include <aws\elasticfilesystem\model\CreateMountTargetRequest.h>
#include <aws\elasticfilesystem\model\CreateTagsRequest.h>
#include <aws\elasticfilesystem\model\DeleteFileSystemRequest.h>
#include <aws\elasticfilesystem\model\DeleteMountTargetRequest.h>
#include <aws\elasticfilesystem\model\DeleteTagsRequest.h>
#include <aws\elasticfilesystem\model\DescribeFileSystemsRequest.h>

namespace Nequeo {
	namespace AWS {
		namespace Storage
		{
			///	<summary>
			///	Cloud account provider.
			///	</summary>
			class EXPORT_NEQUEO_AWS_STORAGE_API ElasticFileSystemCloudAccount
			{
			public:
				///	<summary>
				///	Cloud account provider.
				///	</summary>
				/// <param name="account">The AWS services account.</param>
				ElasticFileSystemCloudAccount(const AwsAccount& account);

				///	<summary>
				///	Cloud account provider destructor.
				///	</summary>
				~ElasticFileSystemCloudAccount();

				/// <summary>
				/// Gets the Elastic File System client.
				/// </summary>
				/// <return>The Elastic File System client.</return>
				const Aws::EFS::EFSClient& GetClient() const;

				///	<summary>
				///	Get the service URI.
				///	</summary>
				///	<return>The service URI.</return>
				std::string GetServiceUri();

				///	<summary>
				///	Create the file system asynchronously.
				///	</summary>
				/// <param name="request">The file system request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void CreateFileSystemAsync(
					const Aws::EFS::Model::CreateFileSystemRequest& request,
					const Aws::EFS::CreateFileSystemResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Create the file system asynchronously.
				///	</summary>
				/// <param name="creationToken">The file system creation token.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void CreateFileSystemAsync(
					const std::string& creationToken,
					const Aws::EFS::CreateFileSystemResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Create the mount target asynchronously.
				///	</summary>
				/// <param name="request">The mount target request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void CreateMountTargetAsync(
					const Aws::EFS::Model::CreateMountTargetRequest& request,
					const Aws::EFS::CreateMountTargetResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Create the tags asynchronously.
				///	</summary>
				/// <param name="request">The tags request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void CreateTagsAsync(
					const Aws::EFS::Model::CreateTagsRequest& request,
					const Aws::EFS::CreateTagsResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Delete the file system asynchronously.
				///	</summary>
				/// <param name="request">The file system request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void DeleteFileSystemAsync(
					const Aws::EFS::Model::DeleteFileSystemRequest& request,
					const Aws::EFS::DeleteFileSystemResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Delete the mount target asynchronously.
				///	</summary>
				/// <param name="request">The mount target request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void DeleteMountTargetAsync(
					const Aws::EFS::Model::DeleteMountTargetRequest& request,
					const Aws::EFS::DeleteMountTargetResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Delete the tags asynchronously.
				///	</summary>
				/// <param name="request">The tags request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void DeleteTagsAsync(
					const Aws::EFS::Model::DeleteTagsRequest& request,
					const Aws::EFS::DeleteTagsResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Describe the file system asynchronously.
				///	</summary>
				/// <param name="request">The file system request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void DescribeFileSystemsAsync(
					const Aws::EFS::Model::DescribeFileSystemsRequest& request,
					const Aws::EFS::DescribeFileSystemsResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

			private:
				bool _disposed;
				AwsAccount _account;
				Aws::UniquePtr<Aws::EFS::EFSClient> _client;
			};
		}
	}
}