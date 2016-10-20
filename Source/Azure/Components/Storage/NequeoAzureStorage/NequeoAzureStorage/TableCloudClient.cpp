/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          TableCloudClient.cpp
*  Purpose :       Cloud client provider class.
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

#include "TableCloudClient.h"

using namespace Nequeo::Azure::Storage;

///	<summary>
///	Cloud client provider.
///	</summary>
/// <param name="account">The Azure account.</param>
TableCloudClient::TableCloudClient(const AzureAccount& account) :  _disposed(false), _isInitialised(false), _account(account)
{
}

///	<summary>
///	Cloud client provider.
///	</summary>
TableCloudClient::~TableCloudClient()
{
	if (!_disposed)
	{
		_disposed = true;
		_isInitialised = false;
	}
}

///	<summary>
///	Get the table client.
///	</summary>
/// <returns>The table client.</returns>
const azure::storage::cloud_table_client& TableCloudClient::TableClient() const
{
	return _client;
}

///	<summary>
///	Initialise the Table client.
///	</summary>
void TableCloudClient::Initialise()
{
	// If not initialised;
	if (!_isInitialised)
	{
		_client = _account._account.create_cloud_table_client();
		_isInitialised = true;
	}
}

///	<summary>
///	Initialise the Table client.
///	</summary>
/// <param name="default_request_options">Default table request options.</param>
void TableCloudClient::Initialise(const azure::storage::table_request_options& default_request_options)
{
	// If not initialised;
	if (!_isInitialised)
	{
		_client = _account._account.create_cloud_table_client(default_request_options);
		_isInitialised = true;
	}
}

///	<summary>
///	Get the list if table items.
///	</summary>
/// <returns>The list of table items.</returns>
const std::vector<azure::storage::cloud_table> TableCloudClient::ListTable() const
{
	std::vector<azure::storage::cloud_table> items;

	// Get the list.
	auto itemIterator = _client.list_tables();

	// Iterate through the list.
	for (azure::storage::cloud_table item : itemIterator)
	{
		// Add the item.
		items.push_back(item);
	}

	// Return the items.
	return items;
}

///	<summary>
///	Get the list if table items.
///	</summary>
/// <param name="prefix">The table name prefix.</param>
/// <returns>The list of table items.</returns>
const std::vector<azure::storage::cloud_table> TableCloudClient::ListTable(const utility::string_t& prefix) const
{
	std::vector<azure::storage::cloud_table> items;

	// Get the list.
	auto itemIterator = _client.list_tables(prefix);
	
	// Iterate through the list.
	for (azure::storage::cloud_table item : itemIterator)
	{
		// Add the item.
		items.push_back(item);
	}

	// Return the items.
	return items;
}

/// <summary>
/// Intitiates an asynchronous operation that returns an <see cref="azure::storage::table_result_segment"/> containing an enumerable collection of tables that begin with the specified prefix.
/// </summary>
/// <param name="prefix">The table name prefix.</param>
/// <param name="token">An <see cref="azure::storage::continuation_token" /> returned by a previous listing operation.</param>
/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::table_result_segment"/> that represents the current operation.</returns>
Concurrency::task<azure::storage::table_result_segment> TableCloudClient::ListTableSegmentedAsync(
	const utility::string_t& prefix, const azure::storage::continuation_token& token) const
{
	return _client.list_tables_segmented_async(prefix, token);
}

/// <summary>
/// Intitiates an asynchronous operation to set the service properties for the Table service client.
/// </summary>
/// <param name="properties">The <see cref="azure::storage::service_properties"/> for the Table service client.</param>
/// <param name="includes">An <see cref="azure::storage::service_properties_includes /> enumeration describing which items to include when setting service properties.</param>
/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
Concurrency::task<void> TableCloudClient::UploadServicePropertiesAsync(
	const azure::storage::service_properties& properties,
	const azure::storage::service_properties_includes& includes) const
{
	return _client.upload_service_properties_async(properties, includes);
}

/// <summary>
/// Intitiates an asynchronous operation to get the properties of the service.
/// </summary>
/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::service_properties"/> that represents the current operation.</returns>
Concurrency::task<azure::storage::service_properties> TableCloudClient::DownloadServicePropertiesAsync() const
{
	return _client.download_service_properties_async();
}

/// <summary>
/// Intitiates an asynchronous operation to get the properties of the service.
/// </summary>
/// <param name="options">An <see cref="azure::storage::table_request_options"/> object that specifies additional options for the request.</param>
/// <param name="context">An <see cref="azure::storage::operation_context"/> object that represents the context for the current operation.</param>
/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::service_properties"/> that represents the current operation.</returns>
Concurrency::task<azure::storage::service_properties> TableCloudClient::DownloadServicePropertiesAsync(
	const azure::storage::table_request_options& options, azure::storage::operation_context context) const
{
	return _client.download_service_properties_async(options, context);
}

///	<summary>
///	Create a Table.
///	</summary>
/// <param name="tableName">The Table name to create.</param>
/// <returns>True if created; else false.</returns>
bool TableCloudClient::CreateTable(const utility::string_t& tableName) const
{
	// Retrieve a reference to a table.
	azure::storage::cloud_table table = _client.get_table_reference(tableName);

	// Create the table if it doesn't exist.
	return table.create_if_not_exists();
}

///	<summary>
///	Delete a Table.
///	</summary>
/// <param name="tableName">The Table name to delete.</param>
/// <returns>True if deleted; else false.</returns>
bool TableCloudClient::DeleteTable(const utility::string_t& tableName) const
{
	// Retrieve a reference to a table.
	azure::storage::cloud_table table = _client.get_table_reference(tableName);

	// Delete the table if it exists.
	return table.delete_table_if_exists();
}

///	<summary>
///	Add an entity to the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="entity">The entity to add.</param>
/// <returns>The add entity result.</returns>
azure::storage::table_result TableCloudClient::AddEntity(const utility::string_t& tableName, azure::storage::table_entity& entity)
{
	// Retrieve a reference to a table.
	azure::storage::cloud_table table = _client.get_table_reference(tableName);

	// Create the table operation that inserts the customer entity.
	azure::storage::table_operation insert_operation = azure::storage::table_operation::insert_entity(entity);

	// Execute the insert operation.
	azure::storage::table_result insert_result = table.execute(insert_operation);
	return insert_result;
}

///	<summary>
///	Add an entity to the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="entity">The entity to add.</param>
/// <returns>The add entity result <see cref="Concurrency::task"/>.</returns>
Concurrency::task<azure::storage::table_result> TableCloudClient::AddEntityAsync(const utility::string_t& tableName, azure::storage::table_entity& entity)
{
	// Retrieve a reference to a table.
	azure::storage::cloud_table table = _client.get_table_reference(tableName);

	// Create the table operation that inserts the customer entity.
	azure::storage::table_operation insert_operation = azure::storage::table_operation::insert_entity(entity);

	// Execute the insert operation.
	return table.execute_async(insert_operation);
}

///	<summary>
///	Add entities to the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="entities">The entities to add.</param>
/// <returns>The add entities result.</returns>
std::vector<azure::storage::table_result> TableCloudClient::AddEntities(const utility::string_t& tableName, const std::vector<azure::storage::table_entity>& entities)
{
	// Retrieve a reference to a table.
	azure::storage::cloud_table table = _client.get_table_reference(tableName);

	// Define a batch operation.
	azure::storage::table_batch_operation batch_operation;

	// For each entity.
	for (auto it = entities.cbegin(); it != entities.cend(); ++it)
	{
		// Add customer entities to the batch insert operation.
		batch_operation.insert_or_replace_entity(*it);
	}
	
	// Execute the batch operation.
	std::vector<azure::storage::table_result> results = table.execute_batch(batch_operation);
	return results;
}

///	<summary>
///	Add entities to the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="entities">The entities to add.</param>
/// <returns>The add entities result <see cref="Concurrency::task"/>.</returns>
Concurrency::task<std::vector<azure::storage::table_result>> TableCloudClient::AddEntitiesAsync(
	const utility::string_t& tableName, const std::vector<azure::storage::table_entity>& entities)
{
	// Retrieve a reference to a table.
	azure::storage::cloud_table table = _client.get_table_reference(tableName);

	// Define a batch operation.
	azure::storage::table_batch_operation batch_operation;

	// For each entity.
	for (auto it = entities.cbegin(); it != entities.cend(); ++it)
	{
		// Add customer entities to the batch insert operation.
		batch_operation.insert_or_replace_entity(*it);
	}

	// Execute the batch operation.
	return table.execute_batch_async(batch_operation);
}

///	<summary>
///	Retrieve entities in the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="query">The query.</param>
/// <returns>The retrieved entities result.</returns>
azure::storage::table_query_iterator TableCloudClient::RetrieveEntities(
	const utility::string_t& tableName, azure::storage::table_query& query)
{
	// Retrieve a reference to a table.
	azure::storage::cloud_table table = _client.get_table_reference(tableName);

	// Execute the query.
	azure::storage::table_query_iterator it = table.execute_query(query);
	return it;
}

///	<summary>
///	Retrieve entities in the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="query">The query.</param>
/// <returns>The retrieved entities result.</returns>
std::vector<azure::storage::table_entity> TableCloudClient::RetrieveEntitiesEx(
	const utility::string_t& tableName, azure::storage::table_query& query)
{
	std::vector<azure::storage::table_entity> entities;

	// Retrieve a reference to a table.
	azure::storage::cloud_table table = _client.get_table_reference(tableName);

	// Execute the query.
	azure::storage::table_query_iterator it = table.execute_query(query);

	// For each item found.
	azure::storage::table_query_iterator end_of_results;
	for (; it != end_of_results; ++it)
	{
		// Add the table entity.
		entities.push_back(*it);
	}

	// Return the entities.
	return entities;
}

///	<summary>
///	Retrieve entities in the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="query">The query.</param>
/// <returns>The retrieved entities result <see cref="Concurrency::task"/>.</returns>
Concurrency::task<azure::storage::table_query_iterator> TableCloudClient::RetrieveEntitiesAsync(
	const utility::string_t& tableName, azure::storage::table_query& query)
{
	// Create a new internal task.
	return Concurrency::create_task([this, tableName, query]() -> azure::storage::table_query_iterator
	{
		// Retrieve a reference to a table.
		azure::storage::cloud_table table = _client.get_table_reference(tableName);

		// Execute the query.
		azure::storage::table_query_iterator it = table.execute_query(query);
		return it;
	});
}

///	<summary>
///	Retrieve entities in the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="query">The query.</param>
/// <returns>The retrieved entities result <see cref="Concurrency::task"/>.</returns>
Concurrency::task<std::vector<azure::storage::table_entity>> TableCloudClient::RetrieveEntitiesExAsync(
	const utility::string_t& tableName, azure::storage::table_query& query)
{
	// Create a new internal task.
	return Concurrency::create_task([this, tableName, query]() -> std::vector<azure::storage::table_entity>
	{
		std::vector<azure::storage::table_entity> entities;

		// Retrieve a reference to a table.
		azure::storage::cloud_table table = _client.get_table_reference(tableName);

		// Execute the query.
		azure::storage::table_query_iterator it = table.execute_query(query);

		// For each item found.
		azure::storage::table_query_iterator end_of_results;
		for (; it != end_of_results; ++it)
		{
			// Add the table entity.
			entities.push_back(*it);
		}

		// Return the entities.
		return entities;
	});
}

///	<summary>
///	Retrieve entity in the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="retrieveOperation">The retrieve operation.</param>
/// <returns>The retrieved entity result.</returns>
azure::storage::table_entity TableCloudClient::RetrieveEntity(
	const utility::string_t& tableName, azure::storage::table_operation& retrieveOperation)
{
	// Retrieve a reference to a table.
	azure::storage::cloud_table table = _client.get_table_reference(tableName);

	// Retrieve the entity with partition key of "Smith" and row key of "Jeff".
	azure::storage::table_result retrieve_result = table.execute(retrieveOperation);

	// Output the entity.
	azure::storage::table_entity entity = retrieve_result.entity();
	return entity;
}

///	<summary>
///	Retrieve entity in the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="retrieveOperation">The retrieve operation.</param>
/// <returns>The retrieved entity result <see cref="Concurrency::task"/>.</returns>
Concurrency::task<azure::storage::table_entity> TableCloudClient::RetrieveEntityAsync(
	const utility::string_t& tableName, azure::storage::table_operation& retrieveOperation)
{
	// Create a new internal task.
	return Concurrency::create_task([this, tableName, retrieveOperation]() -> azure::storage::table_entity
	{
		// Retrieve a reference to a table.
		azure::storage::cloud_table table = _client.get_table_reference(tableName);

		// Retrieve the entity with partition key of "Smith" and row key of "Jeff".
		azure::storage::table_result retrieve_result = table.execute_async(retrieveOperation).get();

		// Output the entity.
		azure::storage::table_entity entity = retrieve_result.entity();
		return entity;
	});
}

///	<summary>
///	Replace an entity to the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="entity">The entity to add.</param>
/// <returns>The add entity result.</returns>
azure::storage::table_result TableCloudClient::ReplaceEntity(const utility::string_t& tableName, azure::storage::table_entity& entity)
{
	// Retrieve a reference to a table.
	azure::storage::cloud_table table = _client.get_table_reference(tableName);

	// Create an operation to replace the entity.
	azure::storage::table_operation replace_operation = azure::storage::table_operation::replace_entity(entity);

	// Submit the operation to the Table service.
	azure::storage::table_result replace_result = table.execute(replace_operation);
	return replace_result;
}

///	<summary>
///	Replace an entity to the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="entity">The entity to add.</param>
/// <returns>The add entity result <see cref="Concurrency::task"/>.</returns>
Concurrency::task<azure::storage::table_result> TableCloudClient::ReplaceEntityAsync(const utility::string_t& tableName, azure::storage::table_entity& entity)
{
	// Create a new internal task.
	return Concurrency::create_task([this, tableName, entity]() -> azure::storage::table_result
	{
		// Retrieve a reference to a table.
		azure::storage::cloud_table table = _client.get_table_reference(tableName);

		// Create an operation to replace the entity.
		azure::storage::table_operation replace_operation = azure::storage::table_operation::replace_entity(entity);

		// Submit the operation to the Table service.
		azure::storage::table_result replace_result = table.execute(replace_operation);
		return replace_result;
	});
}

///	<summary>
///	Add or replace an entity to the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="entity">The entity to add.</param>
/// <returns>The add entity result.</returns>
azure::storage::table_result TableCloudClient::AddReplaceEntity(const utility::string_t& tableName, azure::storage::table_entity& entity)
{
	// Retrieve a reference to a table.
	azure::storage::cloud_table table = _client.get_table_reference(tableName);

	// Create an operation to insert-or-replace the entity.
	azure::storage::table_operation insert_or_replace_operation = azure::storage::table_operation::insert_or_replace_entity(entity);

	// Submit the operation to the Table service.
	azure::storage::table_result insert_or_replace_result = table.execute(insert_or_replace_operation);
	return insert_or_replace_result;
}

///	<summary>
///	Add or replace an entity to the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="entity">The entity to add.</param>
/// <returns>The add entity result <see cref="Concurrency::task"/>.</returns>
Concurrency::task<azure::storage::table_result> TableCloudClient::AddReplaceEntityAsync(const utility::string_t& tableName, azure::storage::table_entity& entity)
{
	// Create a new internal task.
	return Concurrency::create_task([this, tableName, entity]() -> azure::storage::table_result
	{
		// Retrieve a reference to a table.
		azure::storage::cloud_table table = _client.get_table_reference(tableName);

		// Create an operation to insert-or-replace the entity.
		azure::storage::table_operation insert_or_replace_operation = azure::storage::table_operation::insert_or_replace_entity(entity);

		// Submit the operation to the Table service.
		azure::storage::table_result insert_or_replace_result = table.execute(insert_or_replace_operation);
		return insert_or_replace_result;
	});
}

///	<summary>
///	Retrieve entity properties in the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="columns">The columns within the entity.</param>
/// <returns>The retrieved entity properties result.</returns>
azure::storage::table_query_iterator TableCloudClient::QuerySubset(const utility::string_t& tableName, std::vector<utility::string_t>& columns)
{
	// Retrieve a reference to a table.
	azure::storage::cloud_table table = _client.get_table_reference(tableName);

	// Define the query, and select only the Email property.
	azure::storage::table_query query;
	query.set_select_columns(columns);

	// Execute the query.
	azure::storage::table_query_iterator it = table.execute_query(query);
	return it;
}

///	<summary>
///	Retrieve entity properties in the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="columns">The columns within the entity.</param>
/// <returns>The retrieved entity properties result <see cref="Concurrency::task"/>.</returns>
Concurrency::task<azure::storage::table_query_iterator> TableCloudClient::QuerySubsetAsync(const utility::string_t& tableName, std::vector<utility::string_t>& columns)
{
	// Create a new internal task.
	return Concurrency::create_task([this, tableName, columns]() -> azure::storage::table_query_iterator
	{
		// Retrieve a reference to a table.
		azure::storage::cloud_table table = _client.get_table_reference(tableName);

		// Define the query, and select only the Email property.
		azure::storage::table_query query;
		query.set_select_columns(columns);

		// Execute the query.
		azure::storage::table_query_iterator it = table.execute_query(query);
		return it;
	});
}

///	<summary>
///	Retrieve entity properties in the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="columns">The columns within the entity.</param>
/// <returns>The retrieved entity properties result.</returns>
std::vector<azure::storage::table_entity> TableCloudClient::QuerySubsetEx(const utility::string_t& tableName, std::vector<utility::string_t>& columns)
{
	std::vector<azure::storage::table_entity> entities;

	// Retrieve a reference to a table.
	azure::storage::cloud_table table = _client.get_table_reference(tableName);

	// Define the query, and select only the Email property.
	azure::storage::table_query query;
	query.set_select_columns(columns);

	// Execute the query.
	azure::storage::table_query_iterator it = table.execute_query(query);

	// For each item found.
	azure::storage::table_query_iterator end_of_results;
	for (; it != end_of_results; ++it)
	{
		// Add the table entity.
		entities.push_back(*it);
	}

	// Return the entities.
	return entities;
}

///	<summary>
///	Retrieve entity properties in the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="columns">The columns within the entity.</param>
/// <returns>The retrieved entity properties result <see cref="Concurrency::task"/>.</returns>
Concurrency::task<std::vector<azure::storage::table_entity>> TableCloudClient::QuerySubsetExAsync(const utility::string_t& tableName, std::vector<utility::string_t>& columns)
{
	// Create a new internal task.
	return Concurrency::create_task([this, tableName, columns]() -> std::vector<azure::storage::table_entity>
	{
		std::vector<azure::storage::table_entity> entities;

		// Retrieve a reference to a table.
		azure::storage::cloud_table table = _client.get_table_reference(tableName);

		// Define the query, and select only the Email property.
		azure::storage::table_query query;
		query.set_select_columns(columns);

		// Execute the query.
		azure::storage::table_query_iterator it = table.execute_query(query);

		// For each item found.
		azure::storage::table_query_iterator end_of_results;
		for (; it != end_of_results; ++it)
		{
			// Add the table entity.
			entities.push_back(*it);
		}

		// Return the entities.
		return entities;
	});
}

///	<summary>
///	Delete entity in the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="retrieveOperation">The retrieve operation.</param>
/// <returns>The deleted entity result.</returns>
azure::storage::table_result TableCloudClient::DeleteEntity(
	const utility::string_t& tableName, azure::storage::table_operation& retrieveOperation)
{
	// Retrieve a reference to a table.
	azure::storage::cloud_table table = _client.get_table_reference(tableName);

	// Get the table result.
	azure::storage::table_result retrieve_result = table.execute(retrieveOperation);

	// Create an operation to delete the entity.
	azure::storage::table_operation delete_operation = azure::storage::table_operation::delete_entity(retrieve_result.entity());

	// Submit the delete operation to the Table service.
	azure::storage::table_result delete_result = table.execute(delete_operation);
	return delete_result;
}

///	<summary>
///	Delete entity in the Table.
///	</summary>
/// <param name="tableName">The Table name.</param>
/// <param name="retrieveOperation">The retrieve operation.</param>
/// <returns>The deleted entity result <see cref="Concurrency::task"/>.</returns>
Concurrency::task<azure::storage::table_result> TableCloudClient::DeleteEntityAsync(
	const utility::string_t& tableName, azure::storage::table_operation& retrieveOperation)
{
	// Create a new internal task.
	return Concurrency::create_task([this, tableName, retrieveOperation]() -> azure::storage::table_result
	{
		// Retrieve a reference to a table.
		azure::storage::cloud_table table = _client.get_table_reference(tableName);

		// Get the table result.
		azure::storage::table_result retrieve_result = table.execute(retrieveOperation);

		// Create an operation to delete the entity.
		azure::storage::table_operation delete_operation = azure::storage::table_operation::delete_entity(retrieve_result.entity());

		// Submit the delete operation to the Table service.
		azure::storage::table_result delete_result = table.execute(delete_operation);
		return delete_result;
	});
}