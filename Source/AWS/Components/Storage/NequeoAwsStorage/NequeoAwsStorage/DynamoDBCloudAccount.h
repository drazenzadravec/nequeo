/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          DynamoDBCloudAccount.h
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

#pragma once

#include "stdafx.h"

#include "Global.h"
#include "AwsAccount.h"

#undef IN

#include <aws\dynamodb\DynamoDBClient.h>
#include <aws\dynamodb\DynamoDBEndpoint.h>
#include <aws\dynamodb\model\CreateTableRequest.h>
#include <aws\dynamodb\model\ScanRequest.h>

namespace Nequeo {
	namespace Aws {
		namespace Storage 
		{
			///	<summary>
			///	Cloud account provider.
			///	</summary>
			class EXPORT_NEQUEO_AWS_STORAGE_API DynamoDBCloudAccount
			{
			public:
				///	<summary>
				///	Cloud account provider.
				///	</summary>
				/// <param name="account">The AWS services account.</param>
				DynamoDBCloudAccount(const AwsAccount& account);

				///	<summary>
				///	Cloud account provider destructor.
				///	</summary>
				~DynamoDBCloudAccount();

				///	<summary>
				///	Get the service URI.
				///	</summary>
				///	<return>The service URI.</return>
				std::string GetServiceUri();

				///	<summary>
				///	Create the table asynchronously.
				///	</summary>
				/// <param name="request">The table request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void CreateTableAsync(
					const AWS::DynamoDB::Model::CreateTableRequest& request, 
					const AWS::DynamoDB::CreateTableResponseReceivedHandler& handler,
					const std::shared_ptr<const AWS::Client::AsyncCallerContext>& context = nullptr);

				///	<summary>
				///	Scan for items asynchronously.
				///	</summary>
				/// <param name="request">The scan request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void ScanAsync(
					const AWS::DynamoDB::Model::ScanRequest& request,
					const AWS::DynamoDB::ScanResponseReceivedHandler& handler,
					const std::shared_ptr<const AWS::Client::AsyncCallerContext>& context = nullptr);

			private:
				bool _disposed;
				AwsAccount _account;
				AWS::UniquePtr<AWS::DynamoDB::DynamoDBClient> _client;
			};
		}
	}
}