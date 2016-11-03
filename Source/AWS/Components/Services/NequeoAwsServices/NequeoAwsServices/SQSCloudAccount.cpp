/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          SQSCloudAccount.cpp
*  Purpose :       Message Queue Service Cloud account provider class.
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

#include "SQSCloudAccount.h"

using namespace Nequeo::AWS::Services;

static const char* SQS_CLIENT_TAG = "NequeoSQSClient";

///	<summary>
///	Cloud account provider.
///	</summary>
/// <param name="account">The AWS services account.</param>
SQSCloudAccount::SQSCloudAccount(const AwsAccount& account) : _disposed(false), _account(account)
{
	// Create the client.
	_client = Aws::MakeUnique<Aws::SQS::SQSClient>(SQS_CLIENT_TAG, _account._credentials, _account._clientConfiguration);
}

///	<summary>
///	Cloud account provider.
///	</summary>
SQSCloudAccount::~SQSCloudAccount()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Gets the Message Queue Service client.
/// </summary>
/// <return>The Message Queue Service client.</return>
const Aws::SQS::SQSClient& SQSCloudAccount::GetClient() const
{
	return *(_client.get());
}

///	<summary>
///	Get the service URI.
///	</summary>
///	<return>The service URI.</return>
std::string SQSCloudAccount::GetServiceUri()
{
	return std::string(Aws::SQS::SQSEndpoint::ForRegion(_account._clientConfiguration.region).c_str());
}
///	<summary>
///	Create the queue asynchronously.
///	</summary>
/// <param name="request">The queue request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SQSCloudAccount::CreateQueueAsync(
	const Aws::SQS::Model::CreateQueueRequest& request,
	const Aws::SQS::CreateQueueResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->CreateQueueAsync(request, handler, context);
}

///	<summary>
///	Create the queue asynchronously.
///	</summary>
/// <param name="queueName">The queue name.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SQSCloudAccount::CreateQueueAsync(
	const std::string& queueName,
	const Aws::SQS::CreateQueueResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	Aws::SQS::Model::CreateQueueRequest request;
	request.SetQueueName(Aws::String(queueName.c_str()));
	_client->CreateQueueAsync(request, handler, context);
}

///	<summary>
///	List the queues asynchronously.
///	</summary>
/// <param name="request">The queues request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SQSCloudAccount::ListQueuesAsync(
	const Aws::SQS::Model::ListQueuesRequest& request,
	const Aws::SQS::ListQueuesResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->ListQueuesAsync(request, handler, context);
}

///	<summary>
///	Deletes the messages in a queue asynchronously.
///	</summary>
/// <param name="request">The purge request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SQSCloudAccount::PurgeQueueAsync(
	const Aws::SQS::Model::PurgeQueueRequest& request,
	const Aws::SQS::PurgeQueueResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->PurgeQueueAsync(request, handler, context);
}

///	<summary>
///	Deletes the messages in a queue asynchronously.
///	</summary>
/// <param name="queueUrl">The queue URL of the queue to delete the messages from.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SQSCloudAccount::PurgeQueueAsync(
	const std::string& queueUrl,
	const Aws::SQS::PurgeQueueResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	Aws::SQS::Model::PurgeQueueRequest request;
	request.SetQueueUrl(Aws::String(queueUrl.c_str()));
	_client->PurgeQueueAsync(request, handler, context);
}

///	<summary>
///	Retrieves one or more messages asynchronously.
///	</summary>
/// <param name="request">The receive message request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SQSCloudAccount::ReceiveMessageAsync(
	const Aws::SQS::Model::ReceiveMessageRequest& request,
	const Aws::SQS::ReceiveMessageResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->ReceiveMessageAsync(request, handler, context);
}

///	<summary>
///	Retrieves one or more messages asynchronously.
///	</summary>
/// <param name="queueUrl">The queue URL of the queue to return.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SQSCloudAccount::ReceiveMessageAsync(
	const std::string& queueUrl,
	const Aws::SQS::ReceiveMessageResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	Aws::SQS::Model::ReceiveMessageRequest request;
	request.SetQueueUrl(Aws::String(queueUrl.c_str()));
	_client->ReceiveMessageAsync(request, handler, context);
}

///	<summary>
///	Sends a message to the specified queue asynchronously.
///	</summary>
/// <param name="request">The send message request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SQSCloudAccount::SendMessageAsync(
	const Aws::SQS::Model::SendMessageRequest& request,
	const Aws::SQS::SendMessageResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->SendMessageAsync(request, handler, context);
}

///	<summary>
///	Sends a message to the specified queue asynchronously.
///	</summary>
/// <param name="queueUrl">The queue URL of the queue to send.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SQSCloudAccount::SendMessageAsync(
	const std::string& queueUrl,
	const Aws::SQS::SendMessageResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	Aws::SQS::Model::SendMessageRequest request;
	request.SetQueueUrl(Aws::String(queueUrl.c_str()));
	_client->SendMessageAsync(request, handler, context);
}

///	<summary>
///	Sends up to ten messages to the specified queue asynchronously.
///	</summary>
/// <param name="request">The send message batch request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SQSCloudAccount::SendMessageBatchAsync(
	const Aws::SQS::Model::SendMessageBatchRequest& request,
	const Aws::SQS::SendMessageBatchResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->SendMessageBatchAsync(request, handler, context);
}

///	<summary>
///	Adds a permission to a queue asynchronously.
///	</summary>
/// <param name="request">The add permission request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SQSCloudAccount::AddPermissionAsync(
	const Aws::SQS::Model::AddPermissionRequest& request,
	const Aws::SQS::AddPermissionResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->AddPermissionAsync(request, handler, context);
}

///	<summary>
///	Changes the visibility timeout of a specified message in a queue to a new
/// value. The maximum allowed timeout value you can set the value to is 12 hours.
/// This means you can't extend the timeout of a message in an existing queue to
/// more than a total visibility timeout of 12 hours.
///	</summary>
/// <param name="request">The change message visibility request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SQSCloudAccount::ChangeMessageVisibilityAsync(
	const Aws::SQS::Model::ChangeMessageVisibilityRequest& request,
	const Aws::SQS::ChangeMessageVisibilityResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->ChangeMessageVisibilityAsync(request, handler, context);
}
///	<summary>
///	This is a batch version
/// of <a>ChangeMessageVisibility</a>.The result of the action on each message is
/// reported individually in the response.You can send up to 10
/// <a>ChangeMessageVisibility</a> requests with each
/// <code>ChangeMessageVisibilityBatch</code> action.
///	</summary>
/// <param name="request">The change message visibility batch request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SQSCloudAccount::ChangeMessageVisibilityBatchAsync(
	const Aws::SQS::Model::ChangeMessageVisibilityBatchRequest& request,
	const Aws::SQS::ChangeMessageVisibilityBatchResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->ChangeMessageVisibilityBatchAsync(request, handler, context);
}
