/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          S3CloudAccount.cpp
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

#include "stdafx.h"

#include "S3CloudAccount.h"

using namespace Nequeo::AWS::Storage;

static const char* S3_CLIENT_TAG = "NequeoS3Client";

///	<summary>
///	Cloud account provider.
///	</summary>
/// <param name="account">The AWS services account.</param>
S3CloudAccount::S3CloudAccount(const AwsAccount& account) : _disposed(false), _account(account)
{
	// Create the client.
	_client = Aws::MakeUnique<Aws::S3::S3Client>(S3_CLIENT_TAG, _account._credentials, _account._clientConfiguration);
}

///	<summary>
///	Cloud account provider.
///	</summary>
S3CloudAccount::~S3CloudAccount()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Gets the S3 client.
/// </summary>
/// <return>The S3 client.</return>
const Aws::S3::S3Client& S3CloudAccount::GetClient() const
{
	return *(_client.get());
}

///	<summary>
///	Get the service URI.
///	</summary>
///	<return>The service URI.</return>
std::string S3CloudAccount::GetServiceUri()
{
	return std::string(Aws::S3::S3Endpoint::ForRegion(_account._clientConfiguration.region).c_str());
}

///	<summary>
///	Create the bucket asynchronously.
///	</summary>
/// <param name="request">The bucket request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void S3CloudAccount::CreateBucketAsync(
	const Aws::S3::Model::CreateBucketRequest& request,
	const Aws::S3::CreateBucketResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->CreateBucketAsync(request, handler, context);
}

///	<summary>
///	Delete the bucket asynchronously.
///	</summary>
/// <param name="request">The delete request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void S3CloudAccount::DeleteBucketAsync(
	const Aws::S3::Model::DeleteBucketRequest& request,
	const Aws::S3::DeleteBucketResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->DeleteBucketAsync(request, handler, context);
}

///	<summary>
///	Delete the object asynchronously.
///	</summary>
/// <param name="request">The object request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void S3CloudAccount::DeleteObjectAsync(
	const Aws::S3::Model::DeleteObjectRequest& request,
	const Aws::S3::DeleteObjectResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->DeleteObjectAsync(request, handler, context);
}

///	<summary>
///	List all buckets asynchronously.
///	</summary>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void S3CloudAccount::ListBucketsAsync(
	const Aws::S3::ListBucketsResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->ListBucketsAsync(handler, context);
}

///	<summary>
///	List the object asynchronously.
///	</summary>
/// <param name="request">The list request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void S3CloudAccount::ListObjectsAsync(
	const Aws::S3::Model::ListObjectsRequest& request,
	const Aws::S3::ListObjectsResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->ListObjectsAsync(request, handler, context);
}

///	<summary>
///	Add the object asynchronously.
///	</summary>
/// <param name="request">The put request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void S3CloudAccount::PutObjectAsync(
	const Aws::S3::Model::PutObjectRequest& request,
	const Aws::S3::PutObjectResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->PutObjectAsync(request, handler, context);
}

///	<summary>
///	Add the object asynchronously.
///	</summary>
/// <param name="bucketName">The bucket name object request.</param>
/// <param name="key">The key object request.</param>
/// <param name="body">The content for the request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void S3CloudAccount::PutObjectAsync(
	const std::string& bucketName,
	const std::string& key,
	const std::shared_ptr<Aws::IOStream>& body,
	const Aws::S3::PutObjectResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	Aws::S3::Model::PutObjectRequest request;
	request.SetBucket(Aws::String(bucketName.c_str()));
	request.SetKey(Aws::String(key.c_str()));
	request.SetBody(body);
	_client->PutObjectAsync(request, handler, context);
}

///	<summary>
///	Get the object asynchronously.
///	</summary>
/// <param name="request">The object request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void S3CloudAccount::GetObjectAsync(
	const Aws::S3::Model::GetObjectRequest& request,
	const Aws::S3::GetObjectResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->GetObjectAsync(request, handler, context);
}

///	<summary>
///	Get the object asynchronously.
///	</summary>
/// <param name="bucketName">The bucket name object request.</param>
/// <param name="key">The key object request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void S3CloudAccount::GetObjectAsync(
	const std::string& bucketName,
	const std::string& key,
	const Aws::S3::GetObjectResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	Aws::S3::Model::GetObjectRequest request;
	request.SetBucket(Aws::String(bucketName.c_str()));
	request.SetKey(Aws::String(key.c_str()));
	_client->GetObjectAsync(request, handler, context);
}

///	<summary>
///	Get the object asynchronously.
///	</summary>
/// <param name="bucketName">The bucket name object request.</param>
/// <param name="key">The key object request.</param>
/// <param name="versionId">The version Id object request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void S3CloudAccount::GetObjectAsync(
	const std::string& bucketName,
	const std::string& key,
	const std::string& versionId,
	const Aws::S3::GetObjectResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	Aws::S3::Model::GetObjectRequest request;
	request.SetBucket(Aws::String(bucketName.c_str()));
	request.SetKey(Aws::String(key.c_str()));
	request.SetVersionId(Aws::String(versionId.c_str()));
	_client->GetObjectAsync(request, handler, context);
}

///	<summary>
///	Write the object result to the file.
///	</summary>
/// <param name="result">The object result.</param>
/// <param name="fileName">The path to the file to write the result to.</param>
void S3CloudAccount::GetFileObjectResult(Aws::S3::Model::GetObjectResult& result, const std::string& fileName) const
{
	// Get the body of the result stream.
	Aws::IOStream& stream = result.GetBody();

	// Create a new file or truncate an existing file.
	std::ofstream write(fileName, std::istream::binary | std::istream::out | std::istream::trunc);

	// If the file could be created.
	if (!write.bad())
	{
		// If the file is open.
		if (!write.is_open())
		{
			// Create the buffer.
			const unsigned int read_write_size = 8192;
			char* buffer = new char[read_write_size];

			try
			{
				// Get data size.
				long long dataSize = result.GetContentLength();

				// Set read write size.
				std::streamsize count = read_write_size;
				std::streamsize read = read_write_size;

				// Read until end of data.
				while (!stream.eof())
				{
					// Read some data, and get the number of data read.
					stream.read(buffer, count);
					read = stream.gcount();

					// Write the data to the file.
					write.write(buffer, read);
				}
			}
			catch (...) {}

			// Clean up the buffer.
			if (buffer != nullptr)
				delete[] buffer;

			// Close the file.
			write.close();
		}
	}
}

///	<summary>
///	Write the object result to the stream.
///	</summary>
/// <param name="result">The object result.</param>
/// <param name="content">The stream to write the result to.</param>
void S3CloudAccount::GetTextObjectResult(Aws::S3::Model::GetObjectResult& result, std::stringstream& content) const
{
	// Get the body of the result stream.
	Aws::IOStream& stream = result.GetBody();

	// Create the buffer.
	const unsigned int read_write_size = 8192;
	char* buffer = new char[read_write_size];

	try
	{
		// Get data size.
		long long dataSize = result.GetContentLength();

		// Set read write size.
		std::streamsize count = read_write_size;
		std::streamsize read = read_write_size;

		// Read until end of data.
		while (!stream.eof())
		{
			// Read some data, and get the number of data read.
			stream.read(buffer, count);
			read = stream.gcount();

			// Write the data to the file.
			content.write(buffer, read);
		}

		// Seek back to the begining of the stream.
		content.seekg(0, content.beg);
	}
	catch (...) {}

	// Clean up the buffer.
	if (buffer != nullptr)
		delete[] buffer;
}

///	<summary>
///	Write the object result to the stream.
///	</summary>
/// <param name="result">The object result.</param>
/// <param name="content">The stream to write the result to.</param>
void S3CloudAccount::GetDataObjectResult(Aws::S3::Model::GetObjectResult& result, std::ostream& content) const
{
	// Get the body of the result stream.
	Aws::IOStream& stream = result.GetBody();

	// Create the buffer.
	const unsigned int read_write_size = 8192;
	char* buffer = new char[read_write_size];
	
	try
	{
		// Get data size.
		long long dataSize = result.GetContentLength();

		// Set read write size.
		std::streamsize count = read_write_size;
		std::streamsize read = read_write_size;

		// Read until end of data.
		while (!stream.eof())
		{
			// Read some data, and get the number of data read.
			stream.read(buffer, count);
			read = stream.gcount();

			// Write the data to the file.
			content.write(buffer, read);
		}

		// Seek back to the begining of the stream.
		content.seekp(0, content.beg);
	}
	catch (...) {}

	// Clean up the buffer.
	if (buffer != nullptr)
		delete[] buffer;
}