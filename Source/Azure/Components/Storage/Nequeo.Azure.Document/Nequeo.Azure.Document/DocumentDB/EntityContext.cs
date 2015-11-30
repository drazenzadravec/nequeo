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
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Nequeo.Azure.Document.DocumentDB
{
    /// <summary>
    /// Represents the base object type for a document entity in the Database service.
    /// </summary>
    /// <typeparam name="T">The table entity type.</typeparam>
    public class EntityContext<T> where T : Resource, new()
    {
        /// <summary>
        /// Represents the base object type for a document entity in the Database service.
        /// </summary>
        /// <param name="cloudClient">Represents a Windows Azure cloud account.</param>
        /// <param name="database">Represents a Windows Azure database.</param>
        public EntityContext(CloudClient cloudClient, Database database)
        {
            _database = database;
            _cloudClient = cloudClient;
            _client = cloudClient.DocumentClient;
        }

        private CloudClient _cloudClient = null;
        private DocumentClient _client = null;
        private Database _database = null;

        /// <summary>
        /// Gets the cloud client.
        /// </summary>
        public CloudClient CloudClient
        {
            get { return _cloudClient; }
        }

        /// <summary>
        /// Gets the cloud database.
        /// </summary>
        public Database Database
        {
            get { return _database; }
        }

        /// <summary>
        /// Gets the cloud document client.
        /// </summary>
        public DocumentClient DocumentClient
        {
            get { return _client; }
        }

        /// <summary>
        /// Get the document collection.
        /// </summary>
        /// <param name="collectionId">The document collection id.</param>
        /// <returns>The collection instance if exists; else null if does not exist.</returns>
        public DocumentCollection GetDocumentCollection(string collectionId)
        {
            // Get the collection.
            DocumentCollection col = 
                _client.CreateDocumentCollectionQuery(_database.SelfLink)
                              .Where(c => c.Id == collectionId)
                              .AsEnumerable()
                              .FirstOrDefault();

            // Return the collection
            return col;
        }

        /// <summary>
        /// Create a new collection if it does not exist.
        /// </summary>
        /// <param name="collectionId">The document collection id to create.</param>
        /// <returns>The collection instance.</returns>
        public DocumentCollection CreateDocumentCollection(string collectionId)
        {
            // Get the collection.
            DocumentCollection col =
                _client.CreateDocumentCollectionQuery(_database.SelfLink)
                              .Where(c => c.Id == collectionId)
                              .AsEnumerable()
                              .FirstOrDefault();

            // If it does not exist.
            if (col == null)
            {
                // Create the collection.
                col = _client.CreateDocumentCollectionAsync(_database.SelfLink, new DocumentCollection { Id = collectionId }).Result;
            }

            // Return the collection.
            return col;
        }

        /// <summary>
        /// Delete the collection if it exist.
        /// </summary>
        /// <param name="collectionId">The document collection id to delete.</param>
        /// <returns>True if the collection was deleted; else false.</returns>
        public bool DeleteDocumentCollection(string collectionId)
        {
            // Get the collection.
            DocumentCollection col =
                _client.CreateDocumentCollectionQuery(_database.SelfLink)
                              .Where(c => c.Id == collectionId)
                              .AsEnumerable()
                              .FirstOrDefault();

            // In case there was collection matching, go ahead and delete it. 
            if (col != null)
            {
                // Delete the database.
                _client.DeleteDocumentCollectionAsync(col.SelfLink).Wait();
                return true;
            }

            // Did not delete, may not exist.
            return false;
        }

        /// <summary>
        /// Create a new document.
        /// </summary>
        /// <param name="collection">The document collection the entity belongs to.</param>
        /// <param name="entity">The document entity to create.</param>
        /// <returns>The async task.</returns>
        public async Task<ResourceResponse<Microsoft.Azure.Documents.Document>> CreateDocumentAsync(DocumentCollection collection, T entity)
        {
            return await _client.CreateDocumentAsync(collection.SelfLink, entity);
        }

        /// <summary>
        /// Delete the current document
        /// </summary>
        /// <param name="document">The document entity to delete.</param>
        /// <returns>The async task.</returns>
        public async Task<ResourceResponse<Microsoft.Azure.Documents.Document>> DeleteDocumentAsync(Microsoft.Azure.Documents.Document document)
        {
            return await _client.DeleteDocumentAsync(document.SelfLink);
        }

        /// <summary>
        /// Query the database for documents.
        /// </summary>
        /// <param name="collection">The document collection that contains the documents.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="take">The number of items to take; -1 indicates get all.</param>
        /// <returns>The array of entities.</returns>
        public Task<T[]> QueryAsync(DocumentCollection collection, CancellationToken token, int take = 100)
        {
            // Create a new task.
            return Task<T[]>.Factory.StartNew(() =>
            {
                // Wait until data has been sent.
                return Query(collection, take);

            }, token);
        }

        /// <summary>
        /// Query the database for documents.
        /// </summary>
        /// <param name="collection">The document collection that contains the documents.</param>
        /// <param name="take">The number of items to take; -1 indicates get all.</param>
        /// <returns>The array of entities.</returns>
        public T[] Query(DocumentCollection collection, int take = 100)
        {
            bool foundEnought = false;
            List<T> items = new List<T>();
            IDocumentQuery<T> feed = null;

            // If take is positive.
            if (take > -1)
            {
                // Get the list of items.
                feed = _client.CreateDocumentQuery<T>(collection.SelfLink, new FeedOptions() { MaxItemCount = take }).AsDocumentQuery();
            }
            else
            {
                // Get the list of items.
                feed = _client.CreateDocumentQuery<T>(collection.SelfLink).AsDocumentQuery();
            }

            // Get the next set of items.
            while (feed.HasMoreResults && !foundEnought)
            {
                // For each item in this segment.
                foreach (T entity in feed.ExecuteNextAsync().Result)
                {
                    // Add the entity.
                    items.Add(entity);

                    // If we have enought.
                    if (items.Count >= take)
                        break;
                }

                // If we have enought.
                if (items.Count >= take)
                    foundEnought = true;
            }

            // Return the collection.
            return items.ToArray();
        }

        /// <summary>
        /// Query the database for documents.
        /// </summary>
        /// <param name="collection">The document collection that contains the documents.</param>
        /// <param name="where">The where clause predicate.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="take">The number of items to take; -1 indicates get all.</param>
        /// <returns>The array of entities.</returns>
        public Task<T[]> QueryAsync(DocumentCollection collection, System.Linq.Expressions.Expression<Func<T, bool>> where, CancellationToken token, int take = 100)
        {
            // Create a new task.
            return Task<T[]>.Factory.StartNew(() =>
            {
                // Wait until data has been sent.
                return Query(collection, where, take);

            }, token);
        }

        /// <summary>
        /// Query the database for documents.
        /// </summary>
        /// <param name="collection">The document collection that contains the documents.</param>
        /// <param name="where">The where clause predicate.</param>
        /// <param name="take">The number of items to take; -1 indicates get all.</param>
        /// <returns>The array of entities.</returns>
        public T[] Query(DocumentCollection collection, System.Linq.Expressions.Expression<Func<T, bool>> where, int take = 100)
        {
            bool foundEnought = false;
            List<T> items = new List<T>();
            IDocumentQuery<T> feed = null;

            // If take is positive.
            if (take > -1)
            {
                // Get the list of items.
                feed = _client.CreateDocumentQuery<T>(collection.SelfLink, new FeedOptions() { MaxItemCount = take }).Where(where).AsDocumentQuery();
            }
            else
            {
                // Get the list of items.
                feed = _client.CreateDocumentQuery<T>(collection.SelfLink).Where(where).AsDocumentQuery();
            }

            // Get the next set of items.
            while (feed.HasMoreResults && !foundEnought)
            {
                // For each item in this segment.
                foreach (T entity in feed.ExecuteNextAsync().Result)
                {
                    // Add the entity.
                    items.Add(entity);

                    // If we have enought.
                    if (items.Count >= take)
                        break;
                }

                // If we have enought.
                if (items.Count >= take)
                    foundEnought = true;
            }

            // Return the collection.
            return items.ToArray();
        }
        
        /// <summary>
        /// Query the database for documents.
        /// </summary>
        /// <param name="collection">The document collection that contains the documents.</param>
        /// <param name="querySpec">The sql query specification.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="take">The number of items to take; -1 indicates get all.</param>
        /// <returns>The array of entities.</returns>
        public Task<T[]> QueryAsync(DocumentCollection collection, SqlQuerySpec querySpec, CancellationToken token, int take = 100)
        {
            // Create a new task.
            return Task<T[]>.Factory.StartNew(() =>
            {
                // Wait until data has been sent.
                return Query(collection, querySpec, take);

            }, token);
        }

        /// <summary>
        /// Query the database for documents.
        /// </summary>
        /// <param name="collection">The document collection that contains the documents.</param>
        /// <param name="querySpec">The sql query specification.</param>
        /// <param name="take">The number of items to take; -1 indicates get all.</param>
        /// <returns>The array of entities.</returns>
        public T[] Query(DocumentCollection collection, SqlQuerySpec querySpec, int take = 100)
        {
            bool foundEnought = false;
            List<T> items = new List<T>();
            IDocumentQuery<T> feed = null;

            // If take is positive.
            if (take > -1)
            {
                // Get the list of items.
                feed = _client.CreateDocumentQuery<T>(collection.SelfLink, querySpec, new FeedOptions() { MaxItemCount = take }).AsDocumentQuery();
            }
            else
            {
                // Get the list of items.
                feed = _client.CreateDocumentQuery<T>(collection.SelfLink, querySpec).AsDocumentQuery();
            }

            // Get the next set of items.
            while (feed.HasMoreResults && !foundEnought)
            {
                // For each item in this segment.
                foreach (T entity in feed.ExecuteNextAsync().Result)
                {
                    // Add the entity.
                    items.Add(entity);

                    // If we have enought.
                    if (items.Count >= take)
                        break;
                }

                // If we have enought.
                if (items.Count >= take)
                    foundEnought = true;
            }

            // Return the collection.
            return items.ToArray();
        }
    }
}
