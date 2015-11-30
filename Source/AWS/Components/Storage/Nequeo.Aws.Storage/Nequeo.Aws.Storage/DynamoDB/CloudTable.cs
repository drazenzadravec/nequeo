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

using Amazon.Runtime;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace Nequeo.Aws.Storage.DynamoDB
{
    /// <summary>
    /// Represents the base object type for a table entity in the Table service.
    /// </summary>
    /// <typeparam name="T">The table entity type.</typeparam>
    public class CloudTable<T> where T : new()
    {
        /// <summary>
        /// Represents the base object type for a table entity in the Table service.
        /// </summary>
        /// <param name="cloudAccount">Cloud account provider.</param>
        public CloudTable(CloudAccount cloudAccount)
        {
            _account = cloudAccount;
            _client = cloudAccount.AmazonDynamoDBClient;
            _context = new DynamoDBContext(_client);
        }

        private CloudAccount _account = null;
        private AmazonDynamoDBClient _client = null;
        private DynamoDBContext _context = null;

        /// <summary>
        /// Gets the DynamoDB context.
        /// </summary>
        public DynamoDBContext DynamoDBContext
        {
            get { return _context; }
        }

        /// <summary>
        /// Delete the entity.
        /// </summary>
        /// <param name="value">The table specifiec value.</param>
        /// <returns>The async task.</returns>
        public async Task DeleteAsync(T value)
        {
            await _context.DeleteAsync<T>(value);
        }

        /// <summary>
        /// Delete the entity.
        /// </summary>
        /// <param name="value">The table specifiec value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task DeleteAsync(T value, CancellationToken cancellationToken)
        {
            await _context.DeleteAsync<T>(value, cancellationToken);
        }

        /// <summary>
        /// Insert entity async.
        /// </summary>
        /// <param name="value">The table specifiec value.</param>
        /// <returns>The async task.</returns>
        public async Task InsertAsync(T value)
        {
            await _context.SaveAsync<T>(value);
        }

        /// <summary>
        /// Insert entity async.
        /// </summary>
        /// <param name="value">The table specifiec value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task InsertAsync(T value, CancellationToken cancellationToken)
        {
            await _context.SaveAsync<T>(value, cancellationToken);
        }

        /// <summary>
        /// Update entity async.
        /// </summary>
        /// <param name="value">The table specifiec value.</param>
        /// <returns>The async task.</returns>
        public async Task UpdateAsync(T value)
        {
            await _context.SaveAsync<T>(value);
        }

        /// <summary>
        /// Update entity async.
        /// </summary>
        /// <param name="value">The table specifiec value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task UpdateAsync(T value, CancellationToken cancellationToken)
        {
            await _context.SaveAsync<T>(value, cancellationToken);
        }

        /// <summary>
        /// Serializes an object to a Document.
        /// </summary>
        /// <param name="value">The table specifiec value.</param>
        /// <returns>The document defentition for the table type.</returns>
        public Document ToDocument(T value)
        {
            return _context.ToDocument<T>(value);
        }

        /// <summary>
        /// Deserializes a document to an instance of type T.
        /// </summary>
        /// <param name="document">The document defentition for the table type.</param>
        /// <returns>The table specifiec type.</returns>
        public T FromDocument(Document document)
        {
            return _context.FromDocument<T>(document);
        }

        /// <summary>
        /// Execution of the Load operation.
        /// </summary>
        /// <param name="keyObject">Key of the target item.</param>
        /// <returns>The async task.</returns>
        public async Task<T> LoadAsync(T keyObject)
        {
            return await _context.LoadAsync<T>(keyObject);
        }

        /// <summary>
        /// Execution of the Load operation.
        /// </summary>
        /// <param name="keyObject">Key of the target item.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<T> LoadAsync(T keyObject, CancellationToken cancellationToken)
        {
            return await _context.LoadAsync<T>(keyObject, cancellationToken);
        }

        /// <summary>
        /// Configures an async Query operation against DynamoDB, finding items that match the specified hash primary key.
        /// </summary>
        /// <param name="hashKeyValue">Hash key of the items to query.</param>
        /// <returns>AsyncSearch which can be used to retrieve DynamoDB data.</returns>
        public AsyncSearch<T> QueryAsync(object hashKeyValue)
        {
            AsyncSearch<T> result = _context.QueryAsync<T>(hashKeyValue);
            return result;
        }

        /// <summary>
        /// Configures an async Query operation against DynamoDB, finding items that match the specified hash primary key.
        /// </summary>
        /// <param name="hashKeyValue">Hash key of the items to query.</param>
        /// <param name="operationConfig">Config object which can be used to override the table used.</param>
        /// <returns>AsyncSearch which can be used to retrieve DynamoDB data.</returns>
        public AsyncSearch<T> QueryAsync(object hashKeyValue, DynamoDBOperationConfig operationConfig)
        {
            AsyncSearch<T> result = _context.QueryAsync<T>(hashKeyValue, operationConfig);
            return result;
        }

        /// <summary>
        /// Configures an async Query operation against DynamoDB, finding items that match the specified hash primary key.
        /// </summary>
        /// <param name="hashKeyValue">Hash key of the items to query.</param>
        /// <param name="take">The number of items to take.</param>
        /// <returns>The async task.</returns>
        public async Task<List<T>> QueryAsync(object hashKeyValue, int take = 100)
        {
            return await QueryAsyncEx(hashKeyValue, CancellationToken.None, take);
        }

        /// <summary>
        /// Configures an async Query operation against DynamoDB, finding items that match the specified hash primary key.
        /// </summary>
        /// <param name="hashKeyValue">Hash key of the items to query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="take">The number of items to take.</param>
        /// <returns>The async task.</returns>
        public async Task<List<T>> QueryAsync(object hashKeyValue, CancellationToken cancellationToken, int take = 100)
        {
            return await QueryAsyncEx(hashKeyValue, cancellationToken, take);
        }
        
        /// <summary>
        /// Configures an async Query operation against DynamoDB, finding items that match the specified hash primary key.
        /// </summary>
        /// <param name="hashKeyValue">Hash key of the items to query.</param>
        /// <param name="queryFilter">Query filter for the Query operation operation.</param>
        /// <param name="take">The number of items to take.</param>
        /// <returns>The async task.</returns>
        public async Task<List<T>> QueryAsync(object hashKeyValue, List<ScanCondition> queryFilter, int take = 100)
        {
            if (queryFilter != null)
            {
                return await QueryAsyncEx(hashKeyValue, CancellationToken.None, take, queryFilter);
            }
            else
                return await QueryAsyncEx(hashKeyValue, CancellationToken.None, take);
        }

        /// <summary>
        /// Configures an async Query operation against DynamoDB, finding items that match the specified hash primary key.
        /// </summary>
        /// <param name="hashKeyValue">Hash key of the items to query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="queryFilter">Query filter for the Query operation operation.</param>
        /// <param name="take">The number of items to take.</param>
        /// <returns>The async task.</returns>
        public async Task<List<T>> QueryAsync(object hashKeyValue, CancellationToken cancellationToken, List<ScanCondition> queryFilter, int take = 100)
        {
            if (queryFilter != null)
            {
                return await QueryAsyncEx(hashKeyValue, cancellationToken, take, queryFilter);
            }
            else
                return await QueryAsyncEx(hashKeyValue, cancellationToken, take);
        }
        
        /// <summary>
        /// Configures an async Query operation against DynamoDB, finding items that match the specified hash primary key.
        /// </summary>
        /// <param name="hashKeyValue">Hash key of the items to query.</param>
        /// <param name="take">The number of items to take.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>The async task.</returns>
        public async Task<List<T>> QueryAsync<ST>(object hashKeyValue, int take = 100, SkipCount<ST> skip = null)
        {
            if (skip != null)
            {
                List<ScanCondition> queryFilter = new List<ScanCondition>();
                queryFilter.Add(skip.GetSkipQuery());
                return await QueryAsyncEx(hashKeyValue, CancellationToken.None, take, queryFilter);
            }
            else
                return await QueryAsyncEx(hashKeyValue, CancellationToken.None, take);
        }

        /// <summary>
        /// Configures an async Query operation against DynamoDB, finding items that match the specified hash primary key.
        /// </summary>
        /// <param name="hashKeyValue">Hash key of the items to query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="take">The number of items to take.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>The async task.</returns>
        public async Task<List<T>> QueryAsync<ST>(object hashKeyValue, CancellationToken cancellationToken, int take = 100, SkipCount<ST> skip = null)
        {
            if (skip != null)
            {
                List<ScanCondition> queryFilter = new List<ScanCondition>();
                queryFilter.Add(skip.GetSkipQuery());
                return await QueryAsyncEx(hashKeyValue, cancellationToken, take, queryFilter);
            }
            else
                return await QueryAsyncEx(hashKeyValue, cancellationToken, take);
        }
        
        /// <summary>
        /// Configures an async Query operation against DynamoDB, finding items that match the specified hash primary key.
        /// </summary>
        /// <param name="hashKeyValue">Hash key of the items to query.</param>
        /// <param name="queryFilter">Query filter for the Query operation operation.</param>
        /// <param name="take">The number of items to take.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>The async task.</returns>
        public async Task<List<T>> QueryAsync<ST>(object hashKeyValue, List<ScanCondition> queryFilter, int take = 100, SkipCount<ST> skip = null)
        {
            if (skip != null)
            {
                List<ScanCondition> queryFilterInternal = new List<ScanCondition>();
                queryFilterInternal.Add(skip.GetSkipQuery());
                if (queryFilter != null)
                {
                    // Add the rest of the queries.
                    queryFilterInternal.AddRange(queryFilter);
                }
                return await QueryAsyncEx(hashKeyValue, CancellationToken.None, take, queryFilterInternal);
            }
            else
            {
                return await QueryAsyncEx(hashKeyValue, CancellationToken.None, take, queryFilter);
            }
        }

        /// <summary>
        /// Configures an async Query operation against DynamoDB, finding items that match the specified hash primary key.
        /// </summary>
        /// <param name="hashKeyValue">Hash key of the items to query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="queryFilter">Query filter for the Query operation operation.</param>
        /// <param name="take">The number of items to take.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>The async task.</returns>
        public async Task<List<T>> QueryAsync<ST>(object hashKeyValue, CancellationToken cancellationToken, List<ScanCondition> queryFilter, int take = 100, SkipCount<ST> skip = null)
        {
            if (skip != null)
            {
                List<ScanCondition> queryFilterInternal = new List<ScanCondition>();
                queryFilterInternal.Add(skip.GetSkipQuery());
                if (queryFilter != null)
                {
                    // Add the rest of the queries.
                    queryFilterInternal.AddRange(queryFilter);
                }
                return await QueryAsyncEx(hashKeyValue, cancellationToken, take, queryFilterInternal);
            }
            else
            {
                return await QueryAsyncEx(hashKeyValue, cancellationToken, take, queryFilter);
            }
        }

        /// <summary>
        /// Configures an async Scan operation against DynamoDB, finding items that match the specified conditions.
        /// </summary>
        /// <param name="take">The number of items to take.</param>
        /// <returns>The async task.</returns>
        public async Task<List<T>> ScanAsync(int take = 100)
        {
            return await ScanAsyncEx(new List<ScanCondition>(), CancellationToken.None, take);
        }

        /// <summary>
        /// Configures an async Scan operation against DynamoDB, finding items that match the specified conditions.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="take">The number of items to take.</param>
        /// <returns>The async task.</returns>
        public async Task<List<T>> ScanAsync(CancellationToken cancellationToken, int take = 100)
        {
            return await ScanAsyncEx(new List<ScanCondition>(), cancellationToken, take);
        }

        /// <summary>
        /// Configures an async Scan operation against DynamoDB, finding items that match the specified conditions.
        /// </summary>
        /// <param name="conditions">The scan conditions.</param>
        /// <param name="take">The number of items to take.</param>
        /// <returns>The async task.</returns>
        public async Task<List<T>> ScanAsync(List<ScanCondition> conditions, int take = 100)
        {
            return await ScanAsyncEx(conditions, CancellationToken.None, take);
        }

        /// <summary>
        /// Configures an async Scan operation against DynamoDB, finding items that match the specified conditions.
        /// </summary>
        /// <param name="conditions">The scan conditions.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="take">The number of items to take.</param>
        /// <returns>The async task.</returns>
        public async Task<List<T>> ScanAsync(List<ScanCondition> conditions, CancellationToken cancellationToken, int take = 100)
        {
            return await ScanAsyncEx(conditions, cancellationToken, take);
        }
        
        /// <summary>
        /// Configures an async Scan operation against DynamoDB, finding items that match the specified conditions.
        /// </summary>
        /// <param name="take">The number of items to take.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>The async task.</returns>
        public async Task<List<T>> ScanAsync<ST>(int take = 100, SkipCount<ST> skip = null)
        {
            if (skip != null)
            {
                List<ScanCondition> queryFilterInternal = new List<ScanCondition>();
                queryFilterInternal.Add(skip.GetSkipQuery());
                return await ScanAsyncEx(queryFilterInternal, CancellationToken.None, take);
            }
            else
                return await ScanAsyncEx(new List<ScanCondition>(), CancellationToken.None, take);
        }

        /// <summary>
        /// Configures an async Scan operation against DynamoDB, finding items that match the specified conditions.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="take">The number of items to take.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>The async task.</returns>
        public async Task<List<T>> ScanAsync<ST>(CancellationToken cancellationToken, int take = 100, SkipCount<ST> skip = null)
        {
            if (skip != null)
            {
                List<ScanCondition> queryFilterInternal = new List<ScanCondition>();
                queryFilterInternal.Add(skip.GetSkipQuery());
                return await ScanAsyncEx(queryFilterInternal, cancellationToken, take);
            }
            else
                return await ScanAsyncEx(new List<ScanCondition>(), cancellationToken, take);
        }

        /// <summary>
        /// Configures an async Scan operation against DynamoDB, finding items that match the specified conditions.
        /// </summary>
        /// <param name="conditions">The scan conditions.</param>
        /// <param name="take">The number of items to take.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>The async task.</returns>
        public async Task<List<T>> ScanAsync<ST>(List<ScanCondition> conditions, int take = 100, SkipCount<ST> skip = null)
        {
            if (skip != null)
            {
                List<ScanCondition> queryFilterInternal = new List<ScanCondition>();
                queryFilterInternal.Add(skip.GetSkipQuery());
                if (conditions != null)
                {
                    queryFilterInternal.AddRange(conditions);
                }
                return await ScanAsyncEx(queryFilterInternal, CancellationToken.None, take);
            }
            else
                return await ScanAsyncEx(conditions, CancellationToken.None, take);
        }

        /// <summary>
        /// Configures an async Scan operation against DynamoDB, finding items that match the specified conditions.
        /// </summary>
        /// <param name="conditions">The scan conditions.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="take">The number of items to take.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>The async task.</returns>
        public async Task<List<T>> ScanAsync<ST>(List<ScanCondition> conditions, CancellationToken cancellationToken, int take = 100, SkipCount<ST> skip = null)
        {
            if (skip != null)
            {
                List<ScanCondition> queryFilterInternal = new List<ScanCondition>();
                queryFilterInternal.Add(skip.GetSkipQuery());
                if (conditions != null)
                {
                    queryFilterInternal.AddRange(conditions);
                }
                return await ScanAsyncEx(queryFilterInternal, cancellationToken, take);
            }
            else
                return await ScanAsyncEx(conditions, cancellationToken, take);
        }
        
        /// <summary>
        /// Configures an async Query operation against DynamoDB, finding items that match the specified hash primary key.
        /// </summary>
        /// <param name="hashKeyValue">Hash key of the items to query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="take">The number of items to take.</param>
        /// <param name="queryFilter">Query filter for the Query operation operation.</param>
        /// <returns>The async task.</returns>
        private async Task<List<T>> QueryAsyncEx(object hashKeyValue, CancellationToken cancellationToken, int take = 100, List<ScanCondition> queryFilter = null)
        {
            List<T> items = new List<T>();
            AsyncSearch<T> result = null;
            bool foundEnought = false;

            // Add the filter.
            if (queryFilter != null)
            {
                // Add the query filter.
                DynamoDBOperationConfig operationConfig = new DynamoDBOperationConfig();
                operationConfig.QueryFilter = queryFilter;
                result = _context.QueryAsync<T>(hashKeyValue, operationConfig);
            }
            else
            {
                // Set the query.
                result = _context.QueryAsync<T>(hashKeyValue);
            }

            do
            {
                // Get the set of results.
                List<T> data = await result.GetNextSetAsync(cancellationToken);
                foreach (T entity in data)
                {
                    // Add the item found.
                    items.Add(entity);

                    // If we have enought.
                    if (items.Count >= take)
                        break;
                }

                // No more data.
                foundEnought = result.IsDone;

                // If we have enought.
                if (items.Count >= take)
                    break;
            }
            while (!foundEnought);

            // Return the list.
            return items;
        }

        /// <summary>
        /// Configures an async Scan operation against DynamoDB, finding items that match the specified conditions.
        /// </summary>
        /// <param name="conditions">The scan conditions.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="take">The number of items to take.</param>
        /// <returns>The async task.</returns>
        private async Task<List<T>> ScanAsyncEx(List<ScanCondition> conditions, CancellationToken cancellationToken, int take = 100)
        {
            List<T> items = new List<T>();
            bool foundEnought = false;

            // Set the query.
            AsyncSearch<T> result = _context.ScanAsync<T>(conditions);

            do
            {
                // Get the set of results.
                List<T> data = await result.GetNextSetAsync(cancellationToken);
                foreach (T entity in data)
                {
                    // Add the item found.
                    items.Add(entity);

                    // If we have enought.
                    if (items.Count >= take)
                        break;
                }

                // No more data.
                foundEnought = result.IsDone;

                // If we have enought.
                if (items.Count >= take)
                    break;
            }
            while (!foundEnought);

            // Return the list.
            return items;
        }
    }
}
