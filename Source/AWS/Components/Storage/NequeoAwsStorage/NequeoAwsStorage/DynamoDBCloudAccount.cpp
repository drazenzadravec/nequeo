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

using namespace Nequeo::AWS::Storage;

static const char* DYNAMODB_CLIENT_TAG = "NequeoDynamoDBClient";

///	<summary>
///	Cloud account provider.
///	</summary>
/// <param name="account">The AWS services account.</param>
DynamoDBCloudAccount::DynamoDBCloudAccount(const AwsAccount& account) : _disposed(false), _account(account)
{
	// Create the client.
	_client = Aws::MakeUnique<Aws::DynamoDB::DynamoDBClient>(DYNAMODB_CLIENT_TAG, _account._credentials, _account._clientConfiguration);
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

/// <summary>
/// Gets the DynamoDB client.
/// </summary>
/// <return>The DynamoDB client.</return>
const Aws::DynamoDB::DynamoDBClient& DynamoDBCloudAccount::GetClient() const
{
	return *(_client.get());
}

///	<summary>
///	Get the service URI.
///	</summary>
///	<return>The service URI.</return>
std::string DynamoDBCloudAccount::GetServiceUri()
{
	return std::string(Aws::DynamoDB::DynamoDBEndpoint::ForRegion(_account._clientConfiguration.region).c_str());
}

///	<summary>
///	Create the table asynchronously.
///	</summary>
/// <param name="request">The table request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void DynamoDBCloudAccount::CreateTableAsync(
	const Aws::DynamoDB::Model::CreateTableRequest& request,
	const Aws::DynamoDB::CreateTableResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context)
{
	// Create the table.
	_client->CreateTableAsync(request, handler, context);
}

///	<summary>
///	Delete the table asynchronously.
///	</summary>
/// <param name="request">The table request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void DynamoDBCloudAccount::DeleteTableAsync(
	const Aws::DynamoDB::Model::DeleteTableRequest& request,
	const Aws::DynamoDB::DeleteTableResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context)
{
	_client->DeleteTableAsync(request, handler, context);
}

///	<summary>
///	Delete the table asynchronously.
///	</summary>
/// <param name="tableName">The table name.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void DynamoDBCloudAccount::DeleteTableAsync(
	const std::string& tableName,
	const Aws::DynamoDB::DeleteTableResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context)
{
	Aws::DynamoDB::Model::DeleteTableRequest request;
	request.SetTableName(Aws::String(tableName.c_str()));
	_client->DeleteTableAsync(request, handler, context);
}

///	<summary>
///	List table items asynchronously.
///	</summary>
/// <param name="request">The list request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void DynamoDBCloudAccount::ListTablesAsync(
	const Aws::DynamoDB::Model::ListTablesRequest& request,
	const Aws::DynamoDB::ListTablesResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context)
{
	_client->ListTablesAsync(request, handler, context);
}

///	<summary>
///	Scan for items asynchronously.
///	</summary>
/// <param name="request">The scan request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void DynamoDBCloudAccount::ScanAsync(
	const Aws::DynamoDB::Model::ScanRequest& request,
	const Aws::DynamoDB::ScanResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context)
{
	_client->ScanAsync(request, handler, context);
}

///	<summary>
///	Get all scanned items after request.
///	</summary>
/// <param name="client">The DynamoDB Client.</param>
/// <param name="results">The scan result.</param>
/// <param name="take">The maximum number of results to return (set take = -1 to return all results found).</param>
///	<return>The array of scanned results.</return>
const Aws::Vector<Aws::Map<Aws::String, Aws::DynamoDB::Model::AttributeValue>>
DynamoDBCloudAccount::GetScanedItems(const Aws::DynamoDB::DynamoDBClient* client, const Aws::DynamoDB::Model::ScanResult& results, long long take) const
{
	long long count = 0;
	bool foundEnought = false;
	bool getAll = (take < 0 ? true : false);
	Aws::Vector<Aws::Map<Aws::String, Aws::DynamoDB::Model::AttributeValue>> completeItems;

	// Get the first set of items.
	Aws::Vector<Aws::Map<Aws::String, Aws::DynamoDB::Model::AttributeValue>> items = results.GetItems();

	// Get the vector size.
	size_t vectorSize = items.size();

	// If items exist.
	if (vectorSize > 0)
	{
		// For each item found.
		for (int i = 0; i < vectorSize; i++)
		{
			// Add the item.
			auto item = items[i];
			completeItems.push_back(item);

			// If not getting all.
			if (!getAll)
			{
				++count;
				if (count >= take)
				{
					foundEnought = true;
					break;
				}
			}
		}
	}
	else
	{
		// No items.
		foundEnought = true;
	}

	// Find more items.
	if (!foundEnought)
	{
		Aws::DynamoDB::Model::ScanResult scanResult = results;

		// Keep getting more items until limit reached.
		do
		{
			// Create a new request from to current key data.
			Aws::DynamoDB::Model::ScanRequest request;
			request.SetExclusiveStartKey(scanResult.GetLastEvaluatedKey());

			// Make the request.
			Aws::DynamoDB::Model::ScanOutcome outcome = client->Scan(request);
			
			// If success.
			if (outcome.IsSuccess())
			{
				// Get the result.
				scanResult = outcome.GetResult();
				auto scanItems = scanResult.GetItems();

				// Get the vector size.
				vectorSize = scanItems.size();

				// If items exist.
				if (vectorSize > 0)
				{
					// For each item found.
					for (int i = 0; i < vectorSize; i++)
					{
						// Add the item.
						auto item = scanItems[i];
						completeItems.push_back(item);

						// If not getting all.
						if (!getAll)
						{
							++count;
							if (count >= take)
							{
								foundEnought = true;
								break;
							}
						}
					}
				}
				else
				{
					// No items.
					foundEnought = true;
				}
			}
			else
			{
				// Do not continue.
				break;
			}

		} while (!foundEnought);
	}

	// Return the complete list.
	return completeItems;
}

///	<summary>
///	Query for items asynchronously.
///	</summary>
/// <param name="request">The query request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void DynamoDBCloudAccount::QueryAsync(
	const Aws::DynamoDB::Model::QueryRequest& request,
	const Aws::DynamoDB::QueryResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context)
{
	_client->QueryAsync(request, handler, context);
}

///	<summary>
///	Get all query items after request.
///	</summary>
/// <param name="client">The current client.</param>
/// <param name="results">The query result.</param>
/// <param name="take">The maximum number of results to return (set take = -1 to return all results found).</param>
///	<return>The array of query results.</return>
const Aws::Vector<Aws::Map<Aws::String, Aws::DynamoDB::Model::AttributeValue>>
DynamoDBCloudAccount::GetQueryItems(const Aws::DynamoDB::DynamoDBClient* client, const Aws::DynamoDB::Model::QueryResult& results, long long take) const
{
	long long count = 0;
	bool foundEnought = false;
	bool getAll = (take < 0 ? true : false);
	Aws::Vector<Aws::Map<Aws::String, Aws::DynamoDB::Model::AttributeValue>> completeItems;

	// Get the first set of items.
	Aws::Vector<Aws::Map<Aws::String, Aws::DynamoDB::Model::AttributeValue>> items = results.GetItems();

	// Get the vector size.
	size_t vectorSize = items.size();

	// If items exist.
	if (vectorSize > 0)
	{
		// For each item found.
		for (int i = 0; i < vectorSize; i++)
		{
			// Add the item.
			auto item = items[i];
			completeItems.push_back(item);

			// If not getting all.
			if (!getAll)
			{
				++count;
				if (count >= take)
				{
					foundEnought = true;
					break;
				}
			}
		}
	}
	else
	{
		// No items.
		foundEnought = true;
	}

	// Find more items.
	if (!foundEnought)
	{
		Aws::DynamoDB::Model::QueryResult queryResult = results;

		// Keep getting more items until limit reached.
		do
		{
			// Create a new request from to current key data.
			Aws::DynamoDB::Model::QueryRequest request;
			request.SetExclusiveStartKey(queryResult.GetLastEvaluatedKey());

			// Make the request.
			Aws::DynamoDB::Model::QueryOutcome outcome = client->Query(request);

			// If success.
			if (outcome.IsSuccess())
			{
				// Get the result.
				queryResult = outcome.GetResult();
				auto queryItems = queryResult.GetItems();

				// Get the vector size.
				vectorSize = queryItems.size();

				// If items exist.
				if (vectorSize > 0)
				{
					// For each item found.
					for (int i = 0; i < vectorSize; i++)
					{
						// Add the item.
						auto item = queryItems[i];
						completeItems.push_back(item);

						// If not getting all.
						if (!getAll)
						{
							++count;
							if (count >= take)
							{
								foundEnought = true;
								break;
							}
						}
					}
				}
				else
				{
					// No items.
					foundEnought = true;
				}
			}
			else
			{
				// Do not continue.
				break;
			}

		} while (!foundEnought);
	}

	// Return the complete list.
	return completeItems;
}

///	<summary>
/// Get item asynchronously.
///	</summary>
/// <param name="request">The item request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void DynamoDBCloudAccount::GetItemAsync(
	const Aws::DynamoDB::Model::GetItemRequest& request,
	const Aws::DynamoDB::GetItemResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->GetItemAsync(request, handler, context);
}

///	<summary>
/// Get item asynchronously.
///	</summary>
/// <param name="tableName">The table name.</param>
/// <param name="keyName">The primary partition key name.</param>
/// <param name="attribute">The attribute value.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void DynamoDBCloudAccount::GetItemAsync(
	const std::string& tableName,
	const std::string& keyName,
	const std::string& attribute,
	const Aws::DynamoDB::GetItemResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	Aws::DynamoDB::Model::GetItemRequest request;
	request.SetTableName(Aws::String(tableName.c_str()));

	Aws::DynamoDB::Model::AttributeValue att(Aws::String(attribute.c_str()));
	request.AddKey(Aws::String(keyName.c_str()), att);

	_client->GetItemAsync(request, handler, context);
}

///	<summary>
/// Get item asynchronously.
///	</summary>
/// <param name="tableName">The table name.</param>
/// <param name="keyName">The primary partition key name.</param>
/// <param name="attribute">The attribute value.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void DynamoDBCloudAccount::GetItemAsync(
	const std::string& tableName,
	const std::string& keyName,
	long long attribute,
	const Aws::DynamoDB::GetItemResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	Aws::DynamoDB::Model::GetItemRequest request;
	request.SetTableName(Aws::String(tableName.c_str()));

	Aws::DynamoDB::Model::AttributeValue att;
	att.SetN(std::to_string(attribute).c_str());
	request.AddKey(Aws::String(keyName.c_str()), att);

	_client->GetItemAsync(request, handler, context);
}

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
void DynamoDBCloudAccount::GetItemAsync(
	const std::string& tableName,
	const std::string& keyName,
	const std::string& sortKeyName,
	const std::string& attributeKey,
	const std::string& attributeSort,
	const Aws::DynamoDB::GetItemResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	Aws::DynamoDB::Model::GetItemRequest request;
	request.SetTableName(Aws::String(tableName.c_str()));

	Aws::DynamoDB::Model::AttributeValue attKey(Aws::String(attributeKey.c_str()));
	request.AddKey(Aws::String(keyName.c_str()), attKey);

	Aws::DynamoDB::Model::AttributeValue attSort(Aws::String(attributeSort.c_str()));
	request.AddKey(Aws::String(sortKeyName.c_str()), attSort);

	_client->GetItemAsync(request, handler, context);
}

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
void DynamoDBCloudAccount::GetItemAsync(
	const std::string& tableName,
	const std::string& keyName,
	const std::string& sortKeyName,
	long long attributeKey,
	long long attributeSort,
	const Aws::DynamoDB::GetItemResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	Aws::DynamoDB::Model::GetItemRequest request;
	request.SetTableName(Aws::String(tableName.c_str()));

	Aws::DynamoDB::Model::AttributeValue attKey;
	attKey.SetN(std::to_string(attributeKey).c_str());
	request.AddKey(Aws::String(keyName.c_str()), attKey);

	Aws::DynamoDB::Model::AttributeValue attSort;
	attSort.SetN(std::to_string(attributeSort).c_str());
	request.AddKey(Aws::String(sortKeyName.c_str()), attSort);

	_client->GetItemAsync(request, handler, context);
}

///	<summary>
/// Put item asynchronously.
///	</summary>
/// <param name="request">The item request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void DynamoDBCloudAccount::PutItemAsync(
	const Aws::DynamoDB::Model::PutItemRequest& request,
	const Aws::DynamoDB::PutItemResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->PutItemAsync(request, handler, context);
}

///	<summary>
/// Put item asynchronously.
///	</summary>
/// <param name="tableName">The table name.</param>
/// <param name="keyName">The primary partition key name.</param>
/// <param name="attribute">The attribute value.</param>
/// <param name="values">The attribute values.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void DynamoDBCloudAccount::PutItemAsync(
	const std::string& tableName,
	const std::string& keyName,
	const std::string& attribute,
	Aws::Map<Aws::String, Aws::DynamoDB::Model::AttributeValue>&& values,
	const Aws::DynamoDB::PutItemResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	Aws::DynamoDB::Model::PutItemRequest request;
	request.SetTableName(Aws::String(tableName.c_str()));

	Aws::DynamoDB::Model::AttributeValue att(Aws::String(attribute.c_str()));
	request.AddItem(Aws::String(keyName.c_str()), att);

	// For each attribute.
	for (auto& i : values)
	{
		// Get the key/value pair.
		request.AddItem(i.first, i.second);
	}

	_client->PutItemAsync(request, handler, context);
}

///	<summary>
/// Put item asynchronously.
///	</summary>
/// <param name="tableName">The table name.</param>
/// <param name="keyName">The primary partition key name.</param>
/// <param name="attribute">The attribute value.</param>
/// <param name="values">The attribute values.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void DynamoDBCloudAccount::PutItemAsync(
	const std::string& tableName,
	const std::string& keyName,
	long long attribute,
	Aws::Map<Aws::String, Aws::DynamoDB::Model::AttributeValue>&& values,
	const Aws::DynamoDB::PutItemResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	Aws::DynamoDB::Model::PutItemRequest request;
	request.SetTableName(Aws::String(tableName.c_str()));

	Aws::DynamoDB::Model::AttributeValue att;
	att.SetN(std::to_string(attribute).c_str());
	request.AddItem(Aws::String(keyName.c_str()), att);

	// For each attribute.
	for (auto& i : values)
	{
		// Get the key/value pair.
		request.AddItem(i.first, i.second);
	}

	_client->PutItemAsync(request, handler, context);
}

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
void DynamoDBCloudAccount::PutItemAsync(
	const std::string& tableName,
	const std::string& keyName,
	const std::string& sortKeyName,
	const std::string& attributeKey,
	const std::string& attributeSort,
	Aws::Map<Aws::String, Aws::DynamoDB::Model::AttributeValue>&& values,
	const Aws::DynamoDB::PutItemResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	Aws::DynamoDB::Model::PutItemRequest request;
	request.SetTableName(Aws::String(tableName.c_str()));

	Aws::DynamoDB::Model::AttributeValue attKey(Aws::String(attributeKey.c_str()));
	request.AddItem(Aws::String(keyName.c_str()), attKey);

	Aws::DynamoDB::Model::AttributeValue attSort(Aws::String(attributeSort.c_str()));
	request.AddItem(Aws::String(sortKeyName.c_str()), attSort);

	// For each attribute.
	for (auto& i : values)
	{
		// Get the key/value pair.
		request.AddItem(i.first, i.second);
	}

	_client->PutItemAsync(request, handler, context);
}

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
void DynamoDBCloudAccount::PutItemAsync(
	const std::string& tableName,
	const std::string& keyName,
	const std::string& sortKeyName,
	long long attributeKey,
	long long attributeSort,
	Aws::Map<Aws::String, Aws::DynamoDB::Model::AttributeValue>&& values,
	const Aws::DynamoDB::PutItemResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	Aws::DynamoDB::Model::PutItemRequest request;
	request.SetTableName(Aws::String(tableName.c_str()));

	Aws::DynamoDB::Model::AttributeValue attKey;
	attKey.SetN(std::to_string(attributeKey).c_str());
	request.AddItem(Aws::String(keyName.c_str()), attKey);

	Aws::DynamoDB::Model::AttributeValue attSort;
	attSort.SetN(std::to_string(attributeSort).c_str());
	request.AddItem(Aws::String(sortKeyName.c_str()), attSort);

	// For each attribute.
	for (auto& i : values)
	{
		// Get the key/value pair.
		request.AddItem(i.first, i.second);
	}

	_client->PutItemAsync(request, handler, context);
}