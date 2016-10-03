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
#include <aws\dynamodb\model\DeleteTableRequest.h>
#include <aws\dynamodb\model\ListTablesRequest.h>
#include <aws\dynamodb\model\ScanRequest.h>
#include <aws\dynamodb\model\QueryRequest.h>
#include <aws\dynamodb\model\AttributeValue.h>
#include <aws\dynamodb\model\GetItemRequest.h>
#include <aws\dynamodb\model\PutItemRequest.h>

namespace Nequeo {
	namespace AWS {
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

				/// <summary>
				/// Gets the DynamoDB client.
				/// </summary>
				/// <return>The DynamoDB client.</return>
				const Aws::DynamoDB::DynamoDBClient& GetClient() const;

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
					const Aws::DynamoDB::Model::CreateTableRequest& request,
					const Aws::DynamoDB::CreateTableResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr);

				///	<summary>
				///	Delete the table asynchronously.
				///	</summary>
				/// <param name="request">The table request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void DeleteTableAsync(
					const Aws::DynamoDB::Model::DeleteTableRequest& request,
					const Aws::DynamoDB::DeleteTableResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr);

				///	<summary>
				///	Delete the table asynchronously.
				///	</summary>
				/// <param name="tableName">The table name.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void DeleteTableAsync(
					const std::string& tableName,
					const Aws::DynamoDB::DeleteTableResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr);

				///	<summary>
				///	List table items asynchronously.
				///	</summary>
				/// <param name="request">The list request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void ListTablesAsync(
					const Aws::DynamoDB::Model::ListTablesRequest& request,
					const Aws::DynamoDB::ListTablesResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr);

				///	<summary>
				///	Scan for items asynchronously.
				///	</summary>
				/// <param name="request">The scan request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void ScanAsync(
					const Aws::DynamoDB::Model::ScanRequest& request,
					const Aws::DynamoDB::ScanResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr);

				///	<summary>
				///	Get all scanned items after request.
				///	</summary>
				/// <param name="client">The current client.</param>
				/// <param name="results">The scan result.</param>
				/// <param name="take">The maximum number of results to return (set take = -1 to return all results found).</param>
				///	<return>The array of scanned results.</return>
				const Aws::Vector<Aws::Map<Aws::String, Aws::DynamoDB::Model::AttributeValue>>
					GetScanedItems(const Aws::DynamoDB::DynamoDBClient* client, const Aws::DynamoDB::Model::ScanResult& results, long long take = 100) const;

				///	<summary>
				///	Query for items asynchronously.
				///	</summary>
				/// <param name="request">The query request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void QueryAsync(
					const Aws::DynamoDB::Model::QueryRequest& request,
					const Aws::DynamoDB::QueryResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr);

				///	<summary>
				///	Get all query items after request.
				///	</summary>
				/// <param name="client">The current client.</param>
				/// <param name="results">The query result.</param>
				/// <param name="take">The maximum number of results to return (set take = -1 to return all results found).</param>
				///	<return>The array of query results.</return>
				const Aws::Vector<Aws::Map<Aws::String, Aws::DynamoDB::Model::AttributeValue>>
					GetQueryItems(const Aws::DynamoDB::DynamoDBClient* client, const Aws::DynamoDB::Model::QueryResult& results, long long take = 100) const;

				///	<summary>
				/// Get item asynchronously.
				///	</summary>
				/// <param name="request">The item request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void GetItemAsync(
					const Aws::DynamoDB::Model::GetItemRequest& request,
					const Aws::DynamoDB::GetItemResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				/// Get item asynchronously.
				///	</summary>
				/// <param name="tableName">The table name.</param>
				/// <param name="keyName">The primary partition key name.</param>
				/// <param name="attribute">The attribute value.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void GetItemAsync(
					const std::string& tableName,
					const std::string& keyName,
					const std::string& attribute,
					const Aws::DynamoDB::GetItemResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				/// Get item asynchronously.
				///	</summary>
				/// <param name="tableName">The table name.</param>
				/// <param name="keyName">The primary partition key name.</param>
				/// <param name="attribute">The attribute value.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void GetItemAsync(
					const std::string& tableName,
					const std::string& keyName,
					long long attribute,
					const Aws::DynamoDB::GetItemResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				/// Get item asynchronously.
				///	</summary>
				/// <param name="tableName">The table name.</param>
				/// <param name="keyName">The primary partition key name.</param>
				/// <param name="sortKeyName">The primary sort key name.</param>
				/// <param name="attributeKey">The attribute value.</param>
				/// <param name="attributeSort">The attribute sort value.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void GetItemAsync(
					const std::string& tableName,
					const std::string& keyName,
					const std::string& sortKeyName,
					const std::string& attributeKey,
					const std::string& attributeSort,
					const Aws::DynamoDB::GetItemResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				/// Get item asynchronously.
				///	</summary>
				/// <param name="tableName">The table name.</param>
				/// <param name="keyName">The primary partition key name.</param>
				/// <param name="sortKeyName">The primary sort key name.</param>
				/// <param name="attributeKey">The attribute value.</param>
				/// <param name="attributeSort">The attribute sort value.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void GetItemAsync(
					const std::string& tableName,
					const std::string& keyName,
					const std::string& sortKeyName,
					long long attributeKey,
					long long attributeSort,
					const Aws::DynamoDB::GetItemResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				/// Put item asynchronously.
				///	</summary>
				/// <param name="request">The item request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void PutItemAsync(
					const Aws::DynamoDB::Model::PutItemRequest& request,
					const Aws::DynamoDB::PutItemResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				/// Put item asynchronously.
				///	</summary>
				/// <param name="tableName">The table name.</param>
				/// <param name="keyName">The primary partition key name.</param>
				/// <param name="attribute">The attribute value.</param>
				/// <param name="values">The attribute values.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void PutItemAsync(
					const std::string& tableName,
					const std::string& keyName,
					const std::string& attribute,
					Aws::Map<Aws::String, Aws::DynamoDB::Model::AttributeValue>&& values,
					const Aws::DynamoDB::PutItemResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				/// Put item asynchronously.
				///	</summary>
				/// <param name="tableName">The table name.</param>
				/// <param name="keyName">The primary partition key name.</param>
				/// <param name="attribute">The attribute value.</param>
				/// <param name="values">The attribute values.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void PutItemAsync(
					const std::string& tableName,
					const std::string& keyName,
					long long attribute,
					Aws::Map<Aws::String, Aws::DynamoDB::Model::AttributeValue>&& values,
					const Aws::DynamoDB::PutItemResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				/// Put item asynchronously.
				///	</summary>
				/// <param name="tableName">The table name.</param>
				/// <param name="keyName">The primary partition key name.</param>
				/// <param name="sortKeyName">The primary sort key name.</param>
				/// <param name="attributeKey">The attribute value.</param>
				/// <param name="attributeSort">The attribute sort value.</param>
				/// <param name="values">The attribute values.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void PutItemAsync(
					const std::string& tableName,
					const std::string& keyName,
					const std::string& sortKeyName,
					const std::string& attributeKey,
					const std::string& attributeSort,
					Aws::Map<Aws::String, Aws::DynamoDB::Model::AttributeValue>&& values,
					const Aws::DynamoDB::PutItemResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				/// Put item asynchronously.
				///	</summary>
				/// <param name="tableName">The table name.</param>
				/// <param name="keyName">The primary partition key name.</param>
				/// <param name="sortKeyName">The primary sort key name.</param>
				/// <param name="attributeKey">The attribute value.</param>
				/// <param name="attributeSort">The attribute sort value.</param>
				/// <param name="values">The attribute values.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void PutItemAsync(
					const std::string& tableName,
					const std::string& keyName,
					const std::string& sortKeyName,
					long long attributeKey,
					long long attributeSort,
					Aws::Map<Aws::String, Aws::DynamoDB::Model::AttributeValue>&& values,
					const Aws::DynamoDB::PutItemResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

			private:
				bool _disposed;
				AwsAccount _account;
				Aws::UniquePtr<Aws::DynamoDB::DynamoDBClient> _client;
			};
		}
	}
}