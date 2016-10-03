/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          S3CloudAccount.h
*  Purpose :       S3 Cloud account provider class.
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

#include <aws\s3\S3Client.h>
#include <aws\s3\S3Endpoint.h>
#include <aws\s3\model\CreateBucketRequest.h>
#include <aws\s3\model\DeleteBucketRequest.h>
#include <aws\s3\model\GetObjectRequest.h>
#include <aws\s3\model\ListObjectsRequest.h>
#include <aws\s3\model\DeleteObjectRequest.h>
#include <aws\s3\model\PutObjectRequest.h>
#include <aws\s3\model\GetObjectResult.h>
#include <aws\s3\model\PutObjectResult.h>

namespace Nequeo {
	namespace AWS {
		namespace Storage
		{
			///	<summary>
			///	Cloud account provider.
			///	</summary>
			class EXPORT_NEQUEO_AWS_STORAGE_API S3CloudAccount
			{
			public:
				///	<summary>
				///	Cloud account provider.
				///	</summary>
				/// <param name="account">The AWS services account.</param>
				S3CloudAccount(const AwsAccount& account);

				///	<summary>
				///	Cloud account provider destructor.
				///	</summary>
				~S3CloudAccount();

				/// <summary>
				/// Gets the S3 client.
				/// </summary>
				/// <return>The S3 client.</return>
				const Aws::S3::S3Client& GetClient() const;

				///	<summary>
				///	Get the service URI.
				///	</summary>
				///	<return>The service URI.</return>
				std::string GetServiceUri();

				///	<summary>
				///	Create the bucket asynchronously.
				///	</summary>
				/// <param name="request">The bucket request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void CreateBucketAsync(
					const Aws::S3::Model::CreateBucketRequest& request,
					const Aws::S3::CreateBucketResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Delete the bucket asynchronously.
				///	</summary>
				/// <param name="request">The delete request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void DeleteBucketAsync(
					const Aws::S3::Model::DeleteBucketRequest& request,
					const Aws::S3::DeleteBucketResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Delete the object asynchronously.
				///	</summary>
				/// <param name="request">The object request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void DeleteObjectAsync(
					const Aws::S3::Model::DeleteObjectRequest& request,
					const Aws::S3::DeleteObjectResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	List all buckets asynchronously.
				///	</summary>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void ListBucketsAsync(
					const Aws::S3::ListBucketsResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	List the object asynchronously.
				///	</summary>
				/// <param name="request">The list request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void ListObjectsAsync(
					const Aws::S3::Model::ListObjectsRequest& request,
					const Aws::S3::ListObjectsResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Add the object asynchronously.
				///	</summary>
				/// <param name="request">The put request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void PutObjectAsync(
					const Aws::S3::Model::PutObjectRequest& request,
					const Aws::S3::PutObjectResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Add the object asynchronously.
				///	</summary>
				/// <param name="bucketName">The bucket name object request.</param>
				/// <param name="key">The key object request.</param>
				/// <param name="body">The content for the request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void PutObjectAsync(
					const std::string& bucketName,
					const std::string& key,
					const std::shared_ptr<Aws::IOStream>& body,
					const Aws::S3::PutObjectResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Get the object asynchronously.
				///	</summary>
				/// <param name="request">The object request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void GetObjectAsync(
					const Aws::S3::Model::GetObjectRequest& request,
					const Aws::S3::GetObjectResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Get the object asynchronously.
				///	</summary>
				/// <param name="bucketName">The bucket name object request.</param>
				/// <param name="key">The key object request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void GetObjectAsync(
					const std::string& bucketName,
					const std::string& key,
					const Aws::S3::GetObjectResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Get the object asynchronously.
				///	</summary>
				/// <param name="bucketName">The bucket name object request.</param>
				/// <param name="key">The key object request.</param>
				/// <param name="versionId">The version Id object request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void GetObjectAsync(
					const std::string& bucketName,
					const std::string& key,
					const std::string& versionId,
					const Aws::S3::GetObjectResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Write the object result to the file.
				///	</summary>
				/// <param name="result">The object result.</param>
				/// <param name="fileName">The path to the file to write the result to.</param>
				void GetFileObjectResult(Aws::S3::Model::GetObjectResult& result, const std::string& fileName) const;

				///	<summary>
				///	Write the object result to the stream.
				///	</summary>
				/// <param name="result">The object result.</param>
				/// <param name="content">The stream to write the result to.</param>
				void GetTextObjectResult(Aws::S3::Model::GetObjectResult& result, std::stringstream& content) const;

				///	<summary>
				///	Write the object result to the stream.
				///	</summary>
				/// <param name="result">The object result.</param>
				/// <param name="content">The stream to write the result to.</param>
				void GetDataObjectResult(Aws::S3::Model::GetObjectResult& result, std::ostream& content) const;

			private:
				bool _disposed;
				AwsAccount _account;
				Aws::UniquePtr<Aws::S3::S3Client> _client;
			};
		}
	}
}