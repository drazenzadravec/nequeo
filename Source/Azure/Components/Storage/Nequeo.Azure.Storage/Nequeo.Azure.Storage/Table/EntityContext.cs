/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.WindowsAzure.Storage.Table;

namespace Nequeo.Azure.Storage.Table
{
    /// <summary>
    /// Represents the base object type for a table entity in the Table service.
    /// </summary>
    /// <typeparam name="T">The table entity type.</typeparam>
    public class EntityContext<T> where T : TableEntity, new()
    {
        /// <summary>
        /// Represents the base object type for a table entity in the Table service.
        /// </summary>
        /// <param name="cloudTable">Represents a Windows Azure table.</param>
        public EntityContext(CloudTable cloudTable)
        {
            _cloudTable = cloudTable;
        }

        private CloudTable _cloudTable = null;

        /// <summary>
        /// Gets the cloud table.
        /// </summary>
        public CloudTable CloudTable
        {
            get { return _cloudTable; }
        }

        /// <summary>
        /// Insert entity async.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The inserted entity.</returns>
        public async Task<T> InsertAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Create the InsertOrReplace  TableOperation
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

            // Execute the operation.
            TableResult result = await _cloudTable.ExecuteAsync(insertOrMergeOperation);
            T inserted = result.Result as T;
            return inserted;
        }

        /// <summary>
        /// Insert entity async.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The inserted entity.</returns>
        public async Task<T> InsertAsync(T entity, CancellationToken cancellationToken)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Create the InsertOrReplace  TableOperation
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

            // Execute the operation.
            TableResult result = await _cloudTable.ExecuteAsync(insertOrMergeOperation, cancellationToken);
            T inserted = result.Result as T;
            return inserted;
        }

        /// <summary>
        /// Insert entities async.
        /// </summary>
        /// <param name="entities">The array of entities.</param>
        /// <returns>The array of inserted entities.</returns>
        public async Task<T[]> InsertAsync(T[] entities)
        {
            List<T> items = new List<T>();

            // Create the batch operation. 
            TableBatchOperation batchOperation = new TableBatchOperation();

            // For each entity.
            for (int i = 0; i < entities.Length; i++)
            {
                // Insert the entity.
                batchOperation.InsertOrMerge(entities[i]);
            }

            // Execute the batch operation.
            IList<TableResult> results = await _cloudTable.ExecuteBatchAsync(batchOperation);

            // For each item.
            foreach (var item in results)
            {
                T inserted = item.Result as T;
                items.Add(inserted);
            }

            // Return the array
            return items.ToArray();
        }

        /// <summary>
        /// Insert entities async.
        /// </summary>
        /// <param name="entities">The array of entities.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The array of inserted entities.</returns>
        public async Task<T[]> InsertAsync(T[] entities, CancellationToken cancellationToken)
        {
            List<T> items = new List<T>();

            // Create the batch operation. 
            TableBatchOperation batchOperation = new TableBatchOperation();

            // For each entity.
            for (int i = 0; i < entities.Length; i++)
            {
                // Insert the entity.
                batchOperation.InsertOrMerge(entities[i]);
            }

            // Execute the batch operation.
            IList<TableResult> results = await _cloudTable.ExecuteBatchAsync(batchOperation, cancellationToken);

            // For each item.
            foreach (var item in results)
            {
                T inserted = item.Result as T;
                items.Add(inserted);
            }

            // Return the array
            return items.ToArray();
        }

        /// <summary>
        /// Get the entity async.
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        /// <returns>The entity.</returns>
        public async Task<T> GetEntityAsync(string partitionKey, string rowKey)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            TableResult result = await _cloudTable.ExecuteAsync(retrieveOperation);
            T entity = result.Result as T;
            return entity;
        }

        /// <summary>
        /// Get the entity async.
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The entity.</returns>
        public async Task<T> GetEntityAsync(string partitionKey, string rowKey, CancellationToken cancellationToken)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            TableResult result = await _cloudTable.ExecuteAsync(retrieveOperation);
            T entity = result.Result as T;
            return entity;
        }

        /// <summary>
        /// Get async.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="take">The number of items to return.</param>
        /// <returns>The array or items.</returns>
        public async Task<T[]> GetAsync(TableQuery<T> query, int take = 100)
        {
            // Return the array
            return await ScanAsyncEx(query, take);
        }

        /// <summary>
        /// Get async.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="take">The number of items to return.</param>
        /// <returns>The array or items.</returns>
        public async Task<T[]> GetAsync(string value, string propertyName = "PartitionKey",
            string operation = QueryComparisons.Equal, int take = 100)
        {
            // Create the query.
            TableQuery<T> scanQuery = new TableQuery<T>().Where
                (TableQuery.GenerateFilterCondition(propertyName, operation, value));

            // Return the array
            return await ScanAsyncEx(scanQuery, take);
        }

        /// <summary>
        /// Get async.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="take">The number of items to return.</param>
        /// <returns>The array or items.</returns>
        public async Task<T[]> GetAsync(byte[] value, string propertyName = "PartitionKey",
            string operation = QueryComparisons.Equal, int take = 100)
        {
            // Create the query.
            TableQuery<T> scanQuery = new TableQuery<T>().Where
                (TableQuery.GenerateFilterConditionForBinary(propertyName, operation, value));

            // Return the array
            return await ScanAsyncEx(scanQuery, take);
        }

        /// <summary>
        /// Get async.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="take">The number of items to return.</param>
        /// <returns>The array or items.</returns>
        public async Task<T[]> GetAsync(bool value, string propertyName = "PartitionKey",
            string operation = QueryComparisons.Equal, int take = 100)
        {
            // Create the query.
            TableQuery<T> scanQuery = new TableQuery<T>().Where
                (TableQuery.GenerateFilterConditionForBool(propertyName, operation, value));

            // Return the array
            return await ScanAsyncEx(scanQuery, take);
        }

        /// <summary>
        /// Get async.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="take">The number of items to return.</param>
        /// <returns>The array or items.</returns>
        public async Task<T[]> GetAsync(DateTimeOffset value, string propertyName = "PartitionKey",
            string operation = QueryComparisons.Equal, int take = 100)
        {
            // Create the query.
            TableQuery<T> scanQuery = new TableQuery<T>().Where
                (TableQuery.GenerateFilterConditionForDate(propertyName, operation, value));

            // Return the array
            return await ScanAsyncEx(scanQuery, take);
        }

        /// <summary>
        /// Get async.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="take">The number of items to return.</param>
        /// <returns>The array or items.</returns>
        public async Task<T[]> GetAsync(double value, string propertyName = "PartitionKey",
            string operation = QueryComparisons.Equal, int take = 100)
        {
            // Create the query.
            TableQuery<T> scanQuery = new TableQuery<T>().Where
                (TableQuery.GenerateFilterConditionForDouble(propertyName, operation, value));

            // Return the array
            return await ScanAsyncEx(scanQuery, take);
        }

        /// <summary>
        /// Get async.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="take">The number of items to return.</param>
        /// <returns>The array or items.</returns>
        public async Task<T[]> GetAsync(Guid value, string propertyName = "PartitionKey",
            string operation = QueryComparisons.Equal, int take = 100)
        {
            // Create the query.
            TableQuery<T> scanQuery = new TableQuery<T>().Where
                (TableQuery.GenerateFilterConditionForGuid(propertyName, operation, value));

            // Return the array
            return await ScanAsyncEx(scanQuery, take);
        }

        /// <summary>
        /// Get async.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="take">The number of items to return.</param>
        /// <returns>The array or items.</returns>
        public async Task<T[]> GetAsync(int value, string propertyName = "PartitionKey",
            string operation = QueryComparisons.Equal, int take = 100)
        {
            // Create the query.
            TableQuery<T> scanQuery = new TableQuery<T>().Where
                (TableQuery.GenerateFilterConditionForInt(propertyName, operation, value));

            // Return the array
            return await ScanAsyncEx(scanQuery, take);
        }

        /// <summary>
        /// Get async.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="take">The number of items to return.</param>
        /// <returns>The array or items.</returns>
        public async Task<T[]> GetAsync(long value, string propertyName = "PartitionKey",
            string operation = QueryComparisons.Equal, int take = 100)
        {
            // Create the query.
            TableQuery<T> scanQuery = new TableQuery<T>().Where
                (TableQuery.GenerateFilterConditionForLong(propertyName, operation, value));

            // Return the array
            return await ScanAsyncEx(scanQuery, take);
        }

        /// <summary>
        /// Get async.
        /// </summary>
        /// <typeparam name="ST">The skip type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="take">The number of items to return.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>The array or items.</returns>
        public async Task<T[]> GetAsync<ST>(string value, string propertyName = "PartitionKey", 
            string operation = QueryComparisons.Equal, int take = 100, SkipCount<ST> skip = null)
        {
            TableQuery<T> scanQuery = null;
            if (skip == null)
            {
                // Create the query.
                scanQuery = new TableQuery<T>().Where
                    (TableQuery.GenerateFilterCondition(propertyName, operation, value));
            }
            else
            {
                // Create the query.
                scanQuery = new TableQuery<T>().Where
                    (TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition(propertyName, operation, value),
                        TableOperators.And,
                        skip.GetSkipQuery()
                    ));
            }

            // Return the array
            return await ScanAsyncEx(scanQuery, take);
        }

        /// <summary>
        /// Get async.
        /// </summary>
        /// <typeparam name="ST">The skip type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="take">The number of items to return.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>The array or items.</returns>
        public async Task<T[]> GetAsync<ST>(byte[] value, string propertyName = "PartitionKey", 
            string operation = QueryComparisons.Equal, int take = 100, SkipCount<ST> skip = null)
        {
            TableQuery<T> scanQuery = null;
            if (skip == null)
            {
                // Create the query.
                scanQuery = new TableQuery<T>().Where
                    (TableQuery.GenerateFilterConditionForBinary(propertyName, operation, value));
            }
            else
            {
                // Create the query.
                scanQuery = new TableQuery<T>().Where
                    (TableQuery.CombineFilters(
                        TableQuery.GenerateFilterConditionForBinary(propertyName, operation, value),
                        TableOperators.And,
                        skip.GetSkipQuery()
                    ));
            }

            // Return the array
            return await ScanAsyncEx(scanQuery, take);
        }

        /// <summary>
        /// Get async.
        /// </summary>
        /// <typeparam name="ST">The skip type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="take">The number of items to return.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>The array or items.</returns>
        public async Task<T[]> GetAsync<ST>(bool value, string propertyName = "PartitionKey", 
            string operation = QueryComparisons.Equal, int take = 100, SkipCount<ST> skip = null)
        {
            TableQuery<T> scanQuery = null;
            if (skip == null)
            {
                // Create the query.
                scanQuery = new TableQuery<T>().Where
                    (TableQuery.GenerateFilterConditionForBool(propertyName, operation, value));
            }
            else
            {
                // Create the query.
                scanQuery = new TableQuery<T>().Where
                    (TableQuery.CombineFilters(
                        TableQuery.GenerateFilterConditionForBool(propertyName, operation, value),
                        TableOperators.And,
                        skip.GetSkipQuery()
                    ));
            }

            // Return the array
            return await ScanAsyncEx(scanQuery, take);
        }

        /// <summary>
        /// Get async.
        /// </summary>
        /// <typeparam name="ST">The skip type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="take">The number of items to return.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>The array or items.</returns>
        public async Task<T[]> GetAsync<ST>(DateTimeOffset value, string propertyName = "PartitionKey", 
            string operation = QueryComparisons.Equal, int take = 100, SkipCount<ST> skip = null)
        {
            TableQuery<T> scanQuery = null;
            if (skip == null)
            {
                // Create the query.
                scanQuery = new TableQuery<T>().Where
                    (TableQuery.GenerateFilterConditionForDate(propertyName, operation, value));
            }
            else
            {
                // Create the query.
                scanQuery = new TableQuery<T>().Where
                    (TableQuery.CombineFilters(
                        TableQuery.GenerateFilterConditionForDate(propertyName, operation, value),
                        TableOperators.And,
                        skip.GetSkipQuery()
                    ));
            }

            // Return the array
            return await ScanAsyncEx(scanQuery, take);
        }

        /// <summary>
        /// Get async.
        /// </summary>
        /// <typeparam name="ST">The skip type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="take">The number of items to return.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>The array or items.</returns>
        public async Task<T[]> GetAsync<ST>(double value, string propertyName = "PartitionKey", 
            string operation = QueryComparisons.Equal, int take = 100, SkipCount<ST> skip = null)
        {
            TableQuery<T> scanQuery = null;
            if (skip == null)
            {
                // Create the query.
                scanQuery = new TableQuery<T>().Where
                    (TableQuery.GenerateFilterConditionForDouble(propertyName, operation, value));
            }
            else
            {
                // Create the query.
                scanQuery = new TableQuery<T>().Where
                    (TableQuery.CombineFilters(
                        TableQuery.GenerateFilterConditionForDouble(propertyName, operation, value),
                        TableOperators.And,
                        skip.GetSkipQuery()
                    ));
            }

            // Return the array
            return await ScanAsyncEx(scanQuery, take);
        }

        /// <summary>
        /// Get async.
        /// </summary>
        /// <typeparam name="ST">The skip type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="take">The number of items to return.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>The array or items.</returns>
        public async Task<T[]> GetAsync<ST>(Guid value, string propertyName = "PartitionKey", 
            string operation = QueryComparisons.Equal, int take = 100, SkipCount<ST> skip = null)
        {
            TableQuery<T> scanQuery = null;
            if (skip == null)
            {
                // Create the query.
                scanQuery = new TableQuery<T>().Where
                    (TableQuery.GenerateFilterConditionForGuid(propertyName, operation, value));
            }
            else
            {
                // Create the query.
                scanQuery = new TableQuery<T>().Where
                    (TableQuery.CombineFilters(
                        TableQuery.GenerateFilterConditionForGuid(propertyName, operation, value),
                        TableOperators.And,
                        skip.GetSkipQuery()
                    ));
            }

            // Return the array
            return await ScanAsyncEx(scanQuery, take);
        }

        /// <summary>
        /// Get async.
        /// </summary>
        /// <typeparam name="ST">The skip type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="take">The number of items to return.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>The array or items.</returns>
        public async Task<T[]> GetAsync<ST>(int value, string propertyName = "PartitionKey", 
            string operation = QueryComparisons.Equal, int take = 100, SkipCount<ST> skip = null)
        {
            TableQuery<T> scanQuery = null;
            if (skip == null)
            {
                // Create the query.
                scanQuery = new TableQuery<T>().Where
                    (TableQuery.GenerateFilterConditionForInt(propertyName, operation, value));
            }
            else
            {
                // Create the query.
                scanQuery = new TableQuery<T>().Where
                    (TableQuery.CombineFilters(
                        TableQuery.GenerateFilterConditionForInt(propertyName, operation, value),
                        TableOperators.And,
                        skip.GetSkipQuery()
                    ));
            }

            // Return the array
            return await ScanAsyncEx(scanQuery, take);
        }

        /// <summary>
        /// Get async.
        /// </summary>
        /// <typeparam name="ST">The skip type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="take">The number of items to return.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>The array or items.</returns>
        public async Task<T[]> GetAsync<ST>(long value, string propertyName = "PartitionKey", 
            string operation = QueryComparisons.Equal, int take = 100, SkipCount<ST> skip = null)
        {
            TableQuery<T> scanQuery = null;
            if (skip == null)
            {
                // Create the query.
                scanQuery = new TableQuery<T>().Where
                    (TableQuery.GenerateFilterConditionForLong(propertyName, operation, value));
            }
            else
            {
                // Create the query.
                scanQuery = new TableQuery<T>().Where
                    (TableQuery.CombineFilters(
                        TableQuery.GenerateFilterConditionForLong(propertyName, operation, value),
                        TableOperators.And,
                        skip.GetSkipQuery()
                    ));
            }

            // Return the array
            return await ScanAsyncEx(scanQuery, take);
        }

        /// <summary>
        /// Delete the entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>The async task.</returns>
        public async Task DeleteEntityAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // The operation to perform.
            TableOperation deleteOperation = TableOperation.Delete(entity);
            await _cloudTable.ExecuteAsync(deleteOperation);
        }

        /// <summary>
        /// Delete the entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task DeleteEntityAsync(T entity, CancellationToken cancellationToken)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // The operation to perform.
            TableOperation deleteOperation = TableOperation.Delete(entity);
            await _cloudTable.ExecuteAsync(deleteOperation, cancellationToken);
        }

        /// <summary>
        /// Delete table async.
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task<bool> DeleteTableAsync()
        {
            return await _cloudTable.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Delete table async.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> DeleteTableAsync(CancellationToken cancellationToken)
        {
            return await _cloudTable.DeleteIfExistsAsync(cancellationToken);
        }

        /// <summary>
        /// Create a table if it does not exist.
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task<bool> CreateTableAsync()
        {
            bool result = await _cloudTable.CreateIfNotExistsAsync();
            return result;
        }

        /// <summary>
        /// Create a table if it does not exist.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> CreateTableAsync(CancellationToken cancellationToken)
        {
            bool result = await _cloudTable.CreateIfNotExistsAsync(cancellationToken);
            return result;
        }

        /// <summary>
        /// Scan async.
        /// </summary>
        /// <param name="scanQuery">The scan query.</param>
        /// <param name="take">The number of items to return.</param>
        /// <returns>The array or items.</returns>
        private async Task<T[]> ScanAsyncEx(TableQuery<T> scanQuery, int take = 100)
        {
            List<T> items = new List<T>();
            TableContinuationToken token = null;
            bool foundEnought = false;

            // Page through the results
            scanQuery.TakeCount = take;
            
            do
            {
                TableQuerySegment<T> segment = await _cloudTable.ExecuteQuerySegmentedAsync(scanQuery, token);
                token = segment.ContinuationToken;
                foreach (T entity in segment)
                {
                    // Add the item found.
                    items.Add(entity);

                    // If we have enought.
                    if (items.Count >= take)
                        break;
                }

                // If we have enought.
                if (items.Count >= take)
                    foundEnought = true;
            }
            while (token != null && !foundEnought);

            // Return the array
            return items.ToArray();
        }
    }
}
