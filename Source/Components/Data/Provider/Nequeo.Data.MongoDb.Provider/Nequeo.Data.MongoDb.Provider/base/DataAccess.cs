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
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
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
        public virtual MongoDatabase CreateDatabase(string databaseName)
        {
            return _connection.Server.GetDatabase(databaseName);
        }

        /// <summary>
        /// Drops a database.
        /// </summary>
        /// <param name="database">The existing instance of MongoDatabase.</param>
        public virtual void DropDatabase(MongoDatabase database)
        {
            database.Drop();
        }

        /// <summary>
        /// Gets a MongoDatabase instance representing a database on this server. Only
        /// one instance is created for each combination of database settings.
        /// </summary>
        /// <param name="databaseName">The name of the database.</param>
        /// <returns>A new or existing instance of MongoDatabase.</returns>
        public virtual MongoDatabase GetDatabase(string databaseName)
        {
            return _connection.Server.GetDatabase(databaseName);
        }

        /// <summary>
        /// Gets the names of the databases on this server.
        /// </summary>
        /// <returns>A list of database names.</returns>
        public virtual IEnumerable<string> GetDatabaseNames()
        {
            return _connection.Server.GetDatabaseNames();
        }

        /// <summary>
        /// Creates a collection. MongoDB creates collections automatically when they
        /// are first used, so this command is mainly here for frameworks.
        /// </summary>
        /// <param name="database">The existing instance of MongoDatabase.</param>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>A CommandResult.</returns>
        public virtual CommandResult CreateCollection(MongoDatabase database, string collectionName)
        {
            return database.CreateCollection(collectionName);
        }

        /// <summary>
        /// Drops a collection.
        /// </summary>
        /// <param name="database">The existing instance of MongoDatabase.</param>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>A CommandResult.</returns>
        public virtual CommandResult DropCollection(MongoDatabase database, string collectionName)
        {
            return database.DropCollection(collectionName);
        }

        /// <summary>
        /// Gets a MongoCollection instance representing a collection on this database
        /// with a default document type of T.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="database">The existing instance of MongoDatabase.</param>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>An instance of MongoCollection.</returns>
        public virtual MongoCollection<T> GetCollection<T>(MongoDatabase database, string collectionName)
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
        public virtual MongoCollection<BsonDocument> GetCollection(MongoDatabase database, string collectionName)
        {
            return database.GetCollection(collectionName);
        }

        /// <summary>
        /// Gets a list of the names of all the collections in this database.
        /// </summary>
        /// <param name="database">The existing instance of MongoDatabase.</param>
        /// <returns>A list of collection names.</returns>
        public virtual IEnumerable<string> GetCollectionNames(MongoDatabase database)
        {
            return database.GetCollectionNames();
        }

        /// <summary>
        /// Gets a MongoCollection instance representing a collection on this database
        /// with a default document type of BsonDocument.
        /// </summary>
        /// <param name="database">The existing instance of MongoDatabase.</param>
        /// <param name="defaultDocumentType">The default document type.</param>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>An instance of MongoCollection.</returns>
        public virtual MongoCollection GetCollection(MongoDatabase database, Type defaultDocumentType, string collectionName)
        {
            return database.GetCollection(defaultDocumentType, collectionName);
        }

        /// <summary>
        /// Inserts a document into this collection (see also InsertBatch to insert multiple documents at once).
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="document">The document type to insert.</param>
        /// <returns>A WriteConcernResult (or null if WriteConcern is disabled).</returns>
        public virtual WriteConcernResult Insert<T>(MongoCollection<T> collection, T document)
        {
            lock(_lockObject)
                return collection.Insert<T>(document);
        }

        /// <summary>
        /// Inserts multiple documents at once into this collection (see also Insert to insert a single document).
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="documents">The documents to insert.</param>
        /// <returns>A list of WriteConcernResults (or null if WriteConcern is disabled).</returns>
        public virtual IEnumerable<WriteConcernResult> InsertBatch<T>(MongoCollection<T> collection, IEnumerable<T> documents)
        {
            lock (_lockObject)
                return collection.InsertBatch(documents);
        }

        /// <summary>
        /// Updates one matching document in this collection.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="query">The query (usually a QueryDocument or constructed using the Query builder).</param>
        /// <param name="update">The update to perform on the matching document.</param>
        /// <returns>A WriteConcernResult (or null if WriteConcern is disabled).</returns>
        public virtual WriteConcernResult Update<T>(MongoCollection<T> collection, IMongoQuery query, IMongoUpdate update)
        {
            lock (_lockObject)
                return collection.Update(query, update);
        }

        /// <summary>
        /// Deletes documents from this collection that match a query.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="query">The query (usually a QueryDocument or constructed using the Query builder).</param>
        /// <returns>A WriteConcernResult (or null if WriteConcern is disabled).</returns>
        public virtual WriteConcernResult Delete<T>(MongoCollection<T> collection, IMongoQuery query)
        {
            lock (_lockObject)
                return collection.Remove(query);
        }

        /// <summary>
        /// Saves a document to this collection. The document must have an identifiable
        /// Id field. Based on the value of the Id field Save will perform either an
        /// Insert or an Update.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="document">The document to save.</param>
        /// <returns>A WriteConcernResult (or null if WriteConcern is disabled).</returns>
        public virtual WriteConcernResult Save<T>(MongoCollection<T> collection, T document)
        {
            lock (_lockObject)
                return collection.Save(document);
        }

        /// <summary>
        /// Counts the number of documents in this collection.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <returns>The number of documents in this collection.</returns>
        public virtual long Count<T>(MongoCollection<T> collection)
        {
            lock (_lockObject)
                return collection.Count();
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
        public virtual long Count<T>(MongoCollection<T> collection, IMongoQuery query, int limit = -1, int skip = 0)
        {
            lock (_lockObject)
            {
                if (limit > -1 && skip > 0)
                    return collection.Count(new CountArgs() { Limit = (long)limit, Skip = (long)skip, Query = query });
                else if (skip > 0)
                    return collection.Count(new CountArgs() { Skip = (long)skip, Query = query });
                else if (limit > -1)
                    return collection.Count(new CountArgs() { Limit = (long)limit, Query = query });
                else
                    return collection.Count(new CountArgs() { Query = query });
            }
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
        public virtual IEnumerable<T> Find<T>(MongoCollection<T> collection, IMongoQuery query, IMongoSortBy sort = null, int limit = -1, int skip = 0)
        {
            lock (_lockObject)
            {
                // If no sort has been specified.
                if (sort == null)
                {
                    if (limit > -1 && skip > 0)
                        return collection.Find(query).SetSkip(skip).SetLimit(limit);
                    else if (skip > 0)
                        return collection.Find(query).SetSkip(skip);
                    else if (limit > -1)
                        return collection.Find(query).SetLimit(limit);
                    else
                        return collection.Find(query);
                }
                else
                {
                    if (limit > -1 && skip > 0)
                        return collection.Find(query).SetSortOrder(sort).SetSkip(skip).SetLimit(limit);
                    else if (skip > 0)
                        return collection.Find(query).SetSortOrder(sort).SetSkip(skip);
                    else if (limit > -1)
                        return collection.Find(query).SetSortOrder(sort).SetLimit(limit);
                    else
                        return collection.Find(query).SetSortOrder(sort);
                }
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
        public virtual IEnumerable<T> FindAll<T>(MongoCollection<T> collection, IMongoSortBy sort = null, int limit = -1, int skip = 0)
        {
            lock (_lockObject)
            {
                // If no sort has been specified.
                if (sort == null)
                {
                    if (limit > -1 && skip > 0)
                        return collection.FindAll().SetSkip(skip).SetLimit(limit);
                    else if (skip > 0)
                        return collection.FindAll().SetSkip(skip);
                    else if (limit > -1)
                        return collection.FindAll().SetLimit(limit);
                    else
                        return collection.FindAll();
                }
                else
                {
                    if (limit > -1 && skip > 0)
                        return collection.FindAll().SetSortOrder(sort).SetSkip(skip).SetLimit(limit);
                    else if (skip > 0)
                        return collection.FindAll().SetSortOrder(sort).SetSkip(skip);
                    else if (limit > -1)
                        return collection.FindAll().SetSortOrder(sort).SetLimit(limit);
                    else
                        return collection.FindAll().SetSortOrder(sort);
                }
            }
        }

        /// <summary>
        /// Find one document in this collection that matches a query as a TDefaultDocument.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="query">The query to execute.</param>
        /// <returns>The document; else null.</returns>
        public virtual T FindOne<T>(MongoCollection<T> collection, IMongoQuery query)
        {
            lock (_lockObject)
            {
                return collection.FindOne(query);
            }
        }

        /// <summary>
        /// Find one document in this collection by its _id value as a TDefaultDocument.
        /// </summary>
        /// <typeparam name="T">The default document type for this collection.</typeparam>
        /// <param name="collection">An instance of MongoCollection.</param>
        /// <param name="id">The id of the document.</param>
        /// <returns>The document; else null.</returns>
        public virtual T FindOneById<T>(MongoCollection<T> collection, BsonValue id)
        {
            lock (_lockObject)
            {
                return collection.FindOneById(id);
            }
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
            MongoCollection<T> collection, IMongoQuery query, Action<T[], object> callback,
            IMongoSortBy sort = null, int limit = -1, int skip = 0, object state = null)
        {
            // Set up a new task.
            T[] result = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<T[]>(() =>
                {
                    List<T> col = new List<T>();

                    try
                    {
                        // For each item found.
                        foreach (T item in Find<T>(collection, query, sort, limit, skip))
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
            callback(result, state);
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
            MongoCollection<T> collection, Action<T[], object> callback, 
            IMongoSortBy sort = null, int limit = -1, int skip = 0, object state = null)
        {
            // Set up a new task.
            T[] result = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<T[]>(() =>
                {
                    List<T> col = new List<T>();

                    try
                    {
                        // For each item found.
                        foreach (T item in FindAll<T>(collection, sort, limit, skip))
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
            callback(result, state);
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
            MongoCollection<T> collection, IMongoQuery query, Action<T, object> callback, object state = null)
        {
            // Set up a new task.
            T result = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<T>(() =>
                {
                    T col = default(T);

                    try
                    {
                        // Find one.
                        col = FindOne<T>(collection, query);
                    }
                    catch { }

                    // Return the the result.
                    return col;
                });

            // Call the callback action sending
            // the result and state.
            callback(result, state);
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
            MongoCollection<T> collection, BsonValue id, Action<T, object> callback, object state = null)
        {
            // Set up a new task.
            T result = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<T>(() =>
                {
                    T col = default(T);

                    try
                    {
                        // Find one.
                        col = FindOneById<T>(collection, id);
                    }
                    catch { }

                    // Return the the result.
                    return col;
                });

            // Call the callback action sending
            // the result and state.
            callback(result, state);
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
