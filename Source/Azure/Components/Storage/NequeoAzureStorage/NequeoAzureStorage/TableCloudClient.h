/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          TableCloudClient.h
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

#pragma once

#include "stdafx.h"

#include "Global.h"
#include "AzureAccount.h"

#include <was\table.h>

namespace Nequeo {
	namespace Azure {
		namespace Storage
		{
			///	<summary>
			///	Cloud client provider.
			///	</summary>
			class EXPORT_NEQUEO_AZURE_STORAGE_API TableCloudClient
			{
			public:
				///	<summary>
				///	Cloud client provider.
				///	</summary>
				/// <param name="account">The Azure account.</param>
				TableCloudClient(const AzureAccount& account);

				///	<summary>
				///	Cloud client provider destructor.
				///	</summary>
				~TableCloudClient();

				/// <summary>
				/// Has the client been initialised.
				/// </summary>
				/// <returns>True if is initialised; else false.</returns>
				inline bool IsInitialised() const
				{
					return _isInitialised;
				}

				///	<summary>
				///	Get the table client.
				///	</summary>
				/// <returns>The table client.</returns>
				const azure::storage::cloud_table_client& TableClient() const;

				///	<summary>
				///	Initialise the Table client.
				///	</summary>
				void Initialise();

				///	<summary>
				///	Initialise the Table client.
				///	</summary>
				/// <param name="default_request_options">Default table request options.</param>
				void Initialise(const azure::storage::table_request_options& default_request_options);

				///	<summary>
				///	Get the list if table items.
				///	</summary>
				/// <returns>The list of table items.</returns>
				const std::vector<azure::storage::cloud_table> ListTable() const;

				///	<summary>
				///	Get the list if table items.
				///	</summary>
				/// <param name="prefix">The table name prefix.</param>
				/// <returns>The list of table items.</returns>
				const std::vector<azure::storage::cloud_table> ListTable(const utility::string_t& prefix) const;

				/// <summary>
				/// Intitiates an asynchronous operation that returns an <see cref="azure::storage::table_result_segment"/> containing an enumerable collection of tables that begin with the specified prefix.
				/// </summary>
				/// <param name="prefix">The table name prefix.</param>
				/// <param name="token">An <see cref="azure::storage::continuation_token" /> returned by a previous listing operation.</param>
				/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::table_result_segment"/> that represents the current operation.</returns>
				Concurrency::task<azure::storage::table_result_segment> ListTableSegmentedAsync(
					const utility::string_t& prefix, const azure::storage::continuation_token& token) const;

				/// <summary>
				/// Intitiates an asynchronous operation to set the service properties for the Table service client.
				/// </summary>
				/// <param name="properties">The <see cref="azure::storage::service_properties"/> for the Table service client.</param>
				/// <param name="includes">An <see cref="azure::storage::service_properties_includes /> enumeration describing which items to include when setting service properties.</param>
				/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
				Concurrency::task<void> UploadServicePropertiesAsync(
					const azure::storage::service_properties& properties,
					const azure::storage::service_properties_includes& includes) const;

				/// <summary>
				/// Intitiates an asynchronous operation to get the properties of the service.
				/// </summary>
				/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::service_properties"/> that represents the current operation.</returns>
				Concurrency::task<azure::storage::service_properties> DownloadServicePropertiesAsync() const;

				/// <summary>
				/// Intitiates an asynchronous operation to get the properties of the service.
				/// </summary>
				/// <param name="options">An <see cref="azure::storage::table_request_options"/> object that specifies additional options for the request.</param>
				/// <param name="context">An <see cref="azure::storage::operation_context"/> object that represents the context for the current operation.</param>
				/// <returns>A <see cref="Concurrency::task"/> object of type <see cref="azure::storage::service_properties"/> that represents the current operation.</returns>
				Concurrency::task<azure::storage::service_properties> DownloadServicePropertiesAsync(
					const azure::storage::table_request_options& options, azure::storage::operation_context context) const;

				///	<summary>
				///	Create a Table.
				///	</summary>
				/// <param name="tableName">The Table name to create.</param>
				/// <returns>True if created; else false.</returns>
				bool CreateTable(const utility::string_t& tableName) const;

				///	<summary>
				///	Delete a Table.
				///	</summary>
				/// <param name="tableName">The Table name to delete.</param>
				/// <returns>True if deleted; else false.</returns>
				bool DeleteTable(const utility::string_t& tableName) const;

				///	<summary>
				///	Add an entity to the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="entity">The entity to add.</param>
				/// <returns>The add entity result.</returns>
				azure::storage::table_result AddEntity(const utility::string_t& tableName, azure::storage::table_entity& entity);

				///	<summary>
				///	Add an entity to the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="entity">The entity to add.</param>
				/// <returns>The add entity result <see cref="Concurrency::task"/>.</returns>
				Concurrency::task<azure::storage::table_result> AddEntityAsync(const utility::string_t& tableName, azure::storage::table_entity& entity);

				///	<summary>
				///	Add entities to the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="entities">The entities to add.</param>
				/// <returns>The add entities result.</returns>
				std::vector<azure::storage::table_result> AddEntities(const utility::string_t& tableName, const std::vector<azure::storage::table_entity>& entities);

				///	<summary>
				///	Add entities to the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="entities">The entities to add.</param>
				/// <returns>The add entities result <see cref="Concurrency::task"/>.</returns>
				Concurrency::task<std::vector<azure::storage::table_result>> AddEntitiesAsync(
					const utility::string_t& tableName, const std::vector<azure::storage::table_entity>& entities);

				///	<summary>
				///	Retrieve entities in the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="query">The query.</param>
				/// <returns>The retrieved entities result.</returns>
				azure::storage::table_query_iterator RetrieveEntities(
					const utility::string_t& tableName, azure::storage::table_query& query);

				///	<summary>
				///	Retrieve entities in the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="query">The query.</param>
				/// <returns>The retrieved entities result.</returns>
				std::vector<azure::storage::table_entity> RetrieveEntitiesEx(
					const utility::string_t& tableName, azure::storage::table_query& query);

				///	<summary>
				///	Retrieve entities in the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="query">The query.</param>
				/// <returns>The retrieved entities result <see cref="Concurrency::task"/>.</returns>
				Concurrency::task<azure::storage::table_query_iterator> RetrieveEntitiesAsync(
					const utility::string_t& tableName, azure::storage::table_query& query);

				///	<summary>
				///	Retrieve entities in the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="query">The query.</param>
				/// <returns>The retrieved entities result <see cref="Concurrency::task"/>.</returns>
				Concurrency::task<std::vector<azure::storage::table_entity>> RetrieveEntitiesExAsync(
					const utility::string_t& tableName, azure::storage::table_query& query);

				///	<summary>
				///	Retrieve entity in the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="retrieveOperation">The retrieve operation.</param>
				/// <returns>The retrieved entity result.</returns>
				azure::storage::table_entity RetrieveEntity(
					const utility::string_t& tableName, azure::storage::table_operation& retrieveOperation);

				///	<summary>
				///	Retrieve entity in the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="retrieveOperation">The retrieve operation.</param>
				/// <returns>The retrieved entity result <see cref="Concurrency::task"/>.</returns>
				Concurrency::task<azure::storage::table_entity> RetrieveEntityAsync(
					const utility::string_t& tableName, azure::storage::table_operation& retrieveOperation);

				///	<summary>
				///	Replace an entity to the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="entity">The entity to add.</param>
				/// <returns>The add entity result.</returns>
				azure::storage::table_result ReplaceEntity(const utility::string_t& tableName, azure::storage::table_entity& entity);

				///	<summary>
				///	Replace an entity to the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="entity">The entity to add.</param>
				/// <returns>The add entity result <see cref="Concurrency::task"/>.</returns>
				Concurrency::task<azure::storage::table_result> ReplaceEntityAsync(const utility::string_t& tableName, azure::storage::table_entity& entity);

				///	<summary>
				///	Add or replace an entity to the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="entity">The entity to add.</param>
				/// <returns>The add entity result.</returns>
				azure::storage::table_result AddReplaceEntity(const utility::string_t& tableName, azure::storage::table_entity& entity);

				///	<summary>
				///	Add or replace an entity to the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="entity">The entity to add.</param>
				/// <returns>The add entity result <see cref="Concurrency::task"/>.</returns>
				Concurrency::task<azure::storage::table_result> AddReplaceEntityAsync(const utility::string_t& tableName, azure::storage::table_entity& entity);

				///	<summary>
				///	Retrieve entity properties in the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="columns">The columns within the entity.</param>
				/// <returns>The retrieved entity properties result.</returns>
				azure::storage::table_query_iterator QuerySubset(const utility::string_t& tableName, std::vector<utility::string_t>& columns);

				///	<summary>
				///	Retrieve entity properties in the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="columns">The columns within the entity.</param>
				/// <returns>The retrieved entity properties result <see cref="Concurrency::task"/>.</returns>
				Concurrency::task<azure::storage::table_query_iterator> QuerySubsetAsync(const utility::string_t& tableName, std::vector<utility::string_t>& columns);

				///	<summary>
				///	Retrieve entity properties in the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="columns">The columns within the entity.</param>
				/// <returns>The retrieved entity properties result.</returns>
				std::vector<azure::storage::table_entity> QuerySubsetEx(const utility::string_t& tableName, std::vector<utility::string_t>& columns);

				///	<summary>
				///	Retrieve entity properties in the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="columns">The columns within the entity.</param>
				/// <returns>The retrieved entity properties result <see cref="Concurrency::task"/>.</returns>
				Concurrency::task<std::vector<azure::storage::table_entity>> QuerySubsetExAsync(const utility::string_t& tableName, std::vector<utility::string_t>& columns);

				///	<summary>
				///	Delete entity in the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="retrieveOperation">The retrieve operation.</param>
				/// <returns>The deleted entity result.</returns>
				azure::storage::table_result DeleteEntity(
					const utility::string_t& tableName, azure::storage::table_operation& retrieveOperation);

				///	<summary>
				///	Delete entity in the Table.
				///	</summary>
				/// <param name="tableName">The Table name.</param>
				/// <param name="retrieveOperation">The retrieve operation.</param>
				/// <returns>The deleted entity result <see cref="Concurrency::task"/>.</returns>
				Concurrency::task<azure::storage::table_result> DeleteEntityAsync(
					const utility::string_t& tableName, azure::storage::table_operation& retrieveOperation);

			private:
				bool _disposed;
				bool _isInitialised;

				AzureAccount _account;
				azure::storage::cloud_table_client _client;
			};
		}
	}
}