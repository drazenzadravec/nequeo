/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Shared;
using MongoDB.Driver.Linq;

namespace Nequeo.Data.MongoDb
{
    /// <summary>
    /// MongoDB data access provider.
    /// </summary>
    public partial class DataAccess : IDisposable
    {
        /// <summary>
        /// MongoDB data access provider.
        /// </summary>
        public DataAccess()
        {
        }

        /// <summary>
        /// MongoDB data access provider.
        /// </summary>
        /// <param name="connection">MongoDB client connection provider.</param>
        public DataAccess(Connection connection)
        {
            _connection = connection;
        }

        private Connection _connection = null;
        private object _lockObject = new object();

        /// <summary>
        /// MongoDB client connection provider.
        /// </summary>
        public Connection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        /// <summary>
        /// Create a MongoDatabase instance representing a database on this server. Only
        /// one instance is created for each combination of database settings.
        /// </summary>
        /// <param name="databaseName">The name of the database.</param>
        /// <returns>A new or existing instance of MongoDatabase.</returns>
        public virtual IMongoDatabase CreateDatabase(string databaseName)
        {
            return _connection.Client.GetDatabase(databaseName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="databaseName"></param>
        public virtual void DropDatabase(MongoDB.Driver.MongoClient client, string databaseName)
        {
            client.DropDatabaseAsync(databaseName);
        }

        /// <summary>
        /// Gets a MongoDatabase instance representing a database on this server. Only
        /// one instance is created for each combination of database settings.
        /// </summary>
        /// <param name="databaseName">The name of the database.</param>
        /// <returns>A new or existing instance of MongoDatabase.</returns>
        public virtual IMongoDatabase GetDatabase(string databaseName)
        {
            return _connection.Client.GetDatabase(databaseName);
        }

        /// <summary>
        /// Gets the names of the databases on this server.
        /// </summary>
        /// <returns>A list of database names.</returns>
        public async virtual Task<IEnumerable<string>> GetDatabaseNames()
        {
            IAsyncCursor<MongoDB.Bson.BsonDocument> collections = await _connection.Client.ListDatabasesAsync();
            bool moved = await collections.MoveNextAsync();
            IEnumerable<BsonDocument> dbs = collections.Current;
            List<string> names = new List<string>();

            // Add the names.
            foreach (BsonDocument db in dbs)
            {
                BsonElement nameElement = db.GetElement("name");
                BsonValue nameValue = nameElement.Value;
                string name = nameValue.AsString;

                // Add the name.
                names.Add(name);
            }

            // Return the list.
            return names;
        }

        /// <summary>
        /// Creates a collection. MongoDB creates collections automatically when they
        /// are first used, so this command is mainly here for frameworks.
        /// </summary>
        /// <param name="database">The existing instance of MongoDatabase.</param>
        /// <param name="collectionName">The name of the collection.</param>
        public virtual void CreateCollection(IMongoDatabase database, string collectionName)
        {
            database.CreateCollectionAsync(collectionName);
        }

        /// <summary>
        /// Drops a collection.
        /// </summary>
        /// <param name="database">The existing instance of MongoDatabase.</param>
        /// <param name="collectionName">The name of the collection.</param>
        public virtual void DropCollection(IMongoDatabase database, string collectionName)
        {
            database.DropCollectionAsync(collectionName);
        }

        /// <summary>
        /// Gets a MongoCollection instance representing a collection on this database
        /// with a default document type of T.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="database">The existing instance of MongoDatabase.</param>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>An instance of MongoCollection.</returns>
        public virtual IMongoCollection<T> GetCollection<T>(IMongoDatabase database, string collectionName)
        {
            return database.GetCollection<T>(collectionName);
        }

        /// <summary>
        /// Gets a MongoCollection instance representing a collection on this database
        /// with a default document type of BsonDocument.
        /// </summary>
        /// <param name="database">The existing instance of MongoDatabase.</param>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>An instance of MongoCollection.</returns>
        public virtual IMongoCollection<BsonDocument> GetCollection(IMongoDatabase database, string collectionName)
        {
            return database.GetCollection<BsonDocument>(collectionName);
        }

        /// <summary>
        /// Gets a list of the names of all the collections in this database.
        /// </summary>
        /// <param name="database">The existing instance of MongoDatabase.</param>
        /// <returns>A list of collection names.</returns>
        public async virtual Task<IEnumerable<string>> GetCollectionNames(IMongoDatabase database)
        {
            IAsyncCursor<MongoDB.Bson.BsonDocument> collections = await database.ListCollectionsAsync();
            bool moved = await collections.MoveNextAsync();
            IEnumerable<BsonDocument> col = collections.Current;
            List<string> names = new List<string>();

            // Add the names.
            foreach (BsonDocument document in col)
            {
                BsonElement nameElement = document.GetElement("name");
                BsonValue nameValue = nameElement.Value;
                string name = nameValue.AsString;

                // Add the name.
                names.Add(name);
            }

            // Return the list.
            return names;
        }

        /// <summary>
        /// Gets a MongoCollection instance representing a collection on this database
        /// with a collection settings.
        /// </summary>
        /// <param name="database">The existing instance of MongoDatabase.</param>
        /// <param name="setting">The collection settings.</param>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>An instance of MongoCollection.</returns>
        public virtual IMongoCollection<BsonDocument> GetCollection(IMongoDatabase database, MongoCollectionSettings setting, string collectionName)
        {
            return database.GetCollection<BsonDocument>(collectionName, setting);
        }

        /// <summary>
        /// Inserts a document into this collection (see also InsertBatch to insert multiple documents at once).
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="document">The document type to insert.</param>
        public virtual void Insert<T>(IMongoCollection<T> collection, T document)
        {
            lock(_lockObject)
                collection.InsertOneAsync(document);
        }

        /// <summary>
        /// Inserts multiple documents at once into this collection (see also Insert to insert a single document).
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="documents">The documents to insert.</param>
        /// <returns>A list of WriteConcernResults (or null if WriteConcern is disabled).</returns>
        public virtual void InsertBatch<T>(IMongoCollection<T> collection, IEnumerable<T> documents)
        {
            lock (_lockObject)
                collection.InsertManyAsync(documents);
        }

        /// <summary>
        /// Updates one matching document in this collection.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="query">The query (usually a QueryDocument or constructed using the Query builder).</param>
        /// <param name="update">The update to perform on the matching document.</param>
        /// <returns>A WriteConcernResult (or null if WriteConcern is disabled).</returns>
        public async virtual Task<UpdateResult> Update<T>(IMongoCollection<T> collection, FilterDefinition<T> query, UpdateDefinition<T> update)
        {
            return await collection.UpdateOneAsync(query, update); 
        }

        /// <summary>
        /// Deletes documents from this collection that match a query.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="query">The query (usually a QueryDocument or constructed using the Query builder).</param>
        /// <returns>A WriteConcernResult (or null if WriteConcern is disabled).</returns>
        public async virtual Task<DeleteResult> Delete<T>(IMongoCollection<T> collection, FilterDefinition<T> query)
        {
            return await collection.DeleteManyAsync(query);
        }

        /// <summary>
        /// Counts the number of documents in this collection.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <returns>The number of documents in this collection.</returns>
        public async virtual Task<long> Count<T>(IMongoCollection<T> collection)
        {
            return await collection.CountAsync(new BsonDocument());
        }

        /// <summary>
        /// Counts the number of documents in this collection that match a query.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="query">The query to execute.</param>
        /// <param name="limit">The number of items to return.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>The number of documents in this collection that match the query.</returns>
        public async virtual Task<long> Count<T>(IMongoCollection<T> collection, FilterDefinition<T> query, int limit = -1, int skip = 0)
        {
            if (limit > -1 && skip > 0)
                return await collection.CountAsync(query, new CountOptions() { Limit = (long)limit, Skip = (long)skip });
            else if (skip > 0)
                return await collection.CountAsync(query, new CountOptions() { Skip = (long)skip });
            else if (limit > -1)
                return await collection.CountAsync(query, new CountOptions() { Limit = (long)limit });
            else
                return await collection.CountAsync(query);
        }

        /// <summary>
        /// Find all documents in this collection that match the query as TDefaultDocuments.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="query">The query to execute.</param>
        /// <param name="sort">The sort by clause</param>
        /// <param name="limit">The number of items to return.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>The collection of documents.</returns>
        public async virtual Task<IEnumerable<T>> Find<T>(IMongoCollection<T> collection, FilterDefinition<T> query, SortDefinition<T> sort = null, int limit = -1, int skip = 0)
        {
            // If no sort has been specified.
            if (sort == null)
            {
                if (limit > -1 && skip > 0)
                    return await collection.Find(query).Skip(skip).Limit(limit).ToListAsync();
                else if (skip > 0)
                    return await collection.Find(query).Skip(skip).ToListAsync();
                else if (limit > -1)
                    return await collection.Find(query).Limit(limit).ToListAsync();
                else
                    return await collection.Find(query).ToListAsync();
            }
            else
            {
                if (limit > -1 && skip > 0)
                    return await collection.Find(query).Sort(sort).Skip(skip).Limit(limit).ToListAsync();
                else if (skip > 0)
                    return await collection.Find(query).Sort(sort).Skip(skip).ToListAsync();
                else if (limit > -1)
                    return await collection.Find(query).Sort(sort).Limit(limit).ToListAsync();
                else
                    return await collection.Find(query).Sort(sort).ToListAsync();
            }
        }

        /// <summary>
        /// Find all documents in this collection as TDefaultDocuments.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="sort">The sort by clause</param>
        /// <param name="limit">The number of items to return.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>The collection of documents.</returns>
        public async virtual Task<IEnumerable<T>> FindAll<T>(IMongoCollection<T> collection, SortDefinition<T> sort = null, int limit = -1, int skip = 0)
        {
            // If no sort has been specified.
            if (sort == null)
            {
                if (limit > -1 && skip > 0)
                    return await collection.Find(new BsonDocument()).Skip(skip).Limit(limit).ToListAsync();
                else if (skip > 0)
                    return await collection.Find(new BsonDocument()).Skip(skip).ToListAsync();
                else if (limit > -1)
                    return await collection.Find(new BsonDocument()).Limit(limit).ToListAsync();
                else
                    return await collection.Find(new BsonDocument()).ToListAsync();
            }
            else
            {
                if (limit > -1 && skip > 0)
                    return await collection.Find(new BsonDocument()).Sort(sort).Skip(skip).Limit(limit).ToListAsync();
                else if (skip > 0)
                    return await collection.Find(new BsonDocument()).Sort(sort).Skip(skip).ToListAsync();
                else if (limit > -1)
                    return await collection.Find(new BsonDocument()).Sort(sort).Limit(limit).ToListAsync();
                else
                    return await collection.Find(new BsonDocument()).Sort(sort).ToListAsync();
            }
        }

        /// <summary>
        /// Find one document in this collection that matches a query as a TDefaultDocument.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="query">The query to execute.</param>
        /// <returns>The document; else null.</returns>
        public async virtual Task<T> FindOne<T>(IMongoCollection<T> collection, FilterDefinition<T> query)
        {
            IEnumerable<T> result = await collection.Find(query).Limit(1).ToListAsync();
            return result.Count() > 0 ? result.First() : default(T);
        }

        /// <summary>
        /// Find one document in this collection by its _id value as a TDefaultDocument.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="id">The id of the document.</param>
        /// <returns>The document; else null.</returns>
        public async virtual Task<T> FindOneById<T>(IMongoCollection<T> collection, FilterDefinition<T> id)
        {
            IEnumerable<T> result = await collection.Find(id).Limit(1).ToListAsync();
            return result.Count() > 0 ? result.First() : default(T);
        }

        /// <summary>
        /// Find all documents in this collection that match the query as TDefaultDocuments.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="query">The query to execute.</param>
        /// <param name="callback">The action callback handler.</param>
        /// <param name="sort">The sort by clause</param>
        /// <param name="limit">The number of items to return.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <param name="state">The action callback state.</param>
        public virtual async void FindAsync<T>(
            IMongoCollection<T> collection, FilterDefinition<T> query, Action<T[], object> callback,
            SortDefinition<T> sort = null, int limit = -1, int skip = 0, object state = null)
        {
            // Set up a new task.
            var result = Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<Task<T[]>>(async () =>
                {
                    List<T> col = new List<T>();

                    try
                    {
                        // Get the data.
                        IEnumerable<T> ret = await Find<T>(collection, query, sort, limit, skip);

                        // For each item found.
                        foreach (T item in ret)
                        {
                            // Add to the collection.
                            col.Add(item);
                        }
                    }
                    catch { }
                    
                    // Return the result.
                    return col.ToArray();
                });

            // Call the callback action sending
            // the result and state.
            var resultList = await result;
            var finalList = await resultList;
            callback(finalList, state);
        }

        /// <summary>
        /// Find all documents in this collection as TDefaultDocuments.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="callback">The action callback handler.</param>
        /// <param name="sort">The sort by clause</param>
        /// <param name="limit">The number of items to return.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <param name="state">The action callback state.</param>
        public virtual async void FindAllAsync<T>(
            IMongoCollection<T> collection, Action<T[], object> callback,
            SortDefinition<T> sort = null, int limit = -1, int skip = 0, object state = null)
        {
            // Set up a new task.
            var result = Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<Task<T[]>>(async () =>
                {
                    List<T> col = new List<T>();

                    try
                    {
                        // Get the data.
                        IEnumerable<T> ret = await FindAll<T>(collection, sort, limit, skip);

                        // For each item found.
                        foreach (T item in ret)
                        {
                            // Add to the collection.
                            col.Add(item);
                        }
                    }
                    catch { }

                    // Return the the result.
                    return col.ToArray();
                });

            // Call the callback action sending
            // the result and state.
            var resultList = await result;
            var finalList = await resultList;
            callback(finalList, state);
        }

        /// <summary>
        /// Find one document in this collection that matches a query as a TDefaultDocument.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="query">The query to execute.</param>
        /// <param name="callback">The action callback handler.</param>
        /// <param name="state">The action callback state.</param>
        public virtual async void FindOneAsync<T>(
            IMongoCollection<T> collection, FilterDefinition<T> query, Action<T, object> callback, object state = null)
        {
            // Set up a new task.
            var result = Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<Task<T>>(async () =>
                {
                    T col = default(T);

                    try
                    {
                        // Find one.
                        col = await FindOne<T>(collection, query);
                    }
                    catch { }

                    // Return the the result.
                    return col;
                });

            // Call the callback action sending
            // the result and state.
            var resultList = await result;
            var finalList = await resultList;
            callback(finalList, state);
        }

        /// <summary>
        /// Find one document in this collection by its _id value as a TDefaultDocument.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="id">The id of the document.</param>
        /// <param name="callback">The action callback handler.</param>
        /// <param name="state">The action callback state.</param>
        public virtual async void FindOneByIdAsync<T>(
            IMongoCollection<T> collection, FilterDefinition<T> id, Action<T, object> callback, object state = null)
        {
            // Set up a new task.
            var result = Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<Task<T>>(async () =>
                {
                    T col = default(T);

                    try
                    {
                        // Find one.
                        col = await FindOneById<T>(collection, id);
                    }
                    catch { }

                    // Return the the result.
                    return col;
                });

            // Call the callback action sending
            // the result and state.
            var resultList = await result;
            var finalList = await resultList;
            callback(finalList, state);
        }

        /// <summary>
        /// Create a dynamic collection of Bson documents.
        /// </summary>
        /// <param name="documents">The collection of Bson documents.</param>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>The collection of dynamic models.</returns>
        public virtual object[] CreateDynamicModel(IEnumerable<BsonDocument> documents, string collectionName = "MongoDBCollection")
        {
            List<object> result = new List<object>();

            // Make sure documents exits.
            if (documents != null && documents.Count() > 0)
            {
                // Create the dynamic type builder.
                Nequeo.Reflection.DynamicTypeBuilder builder = new Reflection.DynamicTypeBuilder("DynamicBsonDocumentModule");

                // For each document.
                foreach (BsonDocument item in documents)
                {
                    int i = 0;

                    // Get the current row.
                    Dictionary<string, object> row = item.ToDictionary();

                    // Get all document property names.
                    List<string> propertyName = new List<string>();
                    foreach (KeyValuePair<string, object> id in row)
                    {
                        // Get the name.
                        string name = id.Key;
                        if (name.ToLower().Contains("_id"))
                            name = "Id";

                        // Add the name.
                        propertyName.Add(name);
                    }

                    // Create the dynamic type.
                    Nequeo.Reflection.DynamicPropertyValue[] dynamicTypeProperties = new Nequeo.Reflection.DynamicPropertyValue[row.Count()];

                    // Get the document values.
                    IEnumerable<BsonValue> values = item.Values;

                    // For each Bson value.
                    foreach(BsonValue bsonValue in values)
                    {
                        // Map the Bson type tp .Net type.
                        object netValue = BsonTypeMapper.MapToDotNetValue(bsonValue);
                        Nequeo.Reflection.DynamicPropertyValue propertyValue = new Reflection.DynamicPropertyValue(propertyName[i], netValue.GetType(), netValue);
                        
                        // Add the value to the list of dynamic types.
                        dynamicTypeProperties[i] = propertyValue;
                        i++;
                    }

                    // Create the model from the document.
                    object model = builder.Create(collectionName, dynamicTypeProperties);
                    result.Add(model);
                }
            }

            // Return the dynamic array.
            return result.ToArray();
        }

        /// <summary>
        /// Create a data table from the bson documents.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="data">The collection of data.</param>
        /// <param name="collectionName">The name of the table to create (Collection Name).</param>
        /// <returns>The data table containing the documents data.</returns>
        public virtual System.Data.DataTable CreateDataTable<T>(T[] data, string collectionName = "MongoDBCollection")
        {
            // Create a new data table instance.
            DataTable dataTable = new DataTable(collectionName);

            // Make sure documents exits.
            if (data != null && data.Count() > 0)
            {
                // If data was returned.
                if (data != null && data.Length > 0)
                {
                    // Get the dynamic type
                    Type type = data[0].GetType();

                    // Get all the properties within the type.
                    PropertyInfo[] properties = type.GetProperties();

                    // For each property found add the
                    // property information.
                    foreach (PropertyInfo propertyItem in properties)
                    {
                        try
                        {
                            // Find nullable types
                            switch (propertyItem.PropertyType.Name.ToLower())
                            {
                                case "nullable`1":
                                    // Get the array of generic
                                    // type parameters.
                                    Type[] genericArguments = propertyItem.PropertyType.GetGenericArguments();

                                    // Create a new column and assign
                                    // each of the properties on the column.
                                    DataColumn columnGen = new DataColumn();
                                    columnGen.DataType = genericArguments[0];
                                    columnGen.ColumnName = propertyItem.Name;
                                    dataTable.Columns.Add(columnGen);
                                    break;

                                default:
                                    // Create a new column and assign
                                    // each of the properties on the column.
                                    DataColumn column = new DataColumn();
                                    column.DataType = propertyItem.PropertyType;
                                    column.ColumnName = propertyItem.Name;
                                    dataTable.Columns.Add(column);
                                    break;
                            }
                        }
                        catch { }
                    }

                    // For each item in the collection.
                    foreach (var result in data)
                    {
                        // Create a new data row.
                        DataRow row = null;
                        row = dataTable.NewRow();

                        // For each property found add the
                        // property information.
                        foreach (PropertyInfo propertyItem in properties)
                        {
                            // Get the current value from
                            // linq entity property.
                            object value = propertyItem.GetValue(result, null);

                            try
                            {
                                // Assign the current row column
                                // value for the property.
                                row[propertyItem.Name] = value;
                            }
                            catch { }
                        }

                        // Add the current row to the table.
                        dataTable.Rows.Add(row);
                    }
                }
            }

            // Return the data table.
            return dataTable;
        }

        #region Dispose Object Methods

        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_connection != null)
                        _connection.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _connection = null;
                _lockObject = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~DataAccess()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
