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
using System.Security;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Nequeo.Azure.Document.DocumentDB
{
    /// <summary>
    /// Cloud client provider.
    /// </summary>
    public class CloudClient : IDisposable
    {
        /// <summary>
        /// Cloud client provider.
        /// </summary>
        /// <param name="serviceEndpoint">The service endpoint URL.</param>
        /// <param name="authKeyOrResourceToken">Authentication key or resource token.</param>
        /// <param name="connectionPolicy">Connection policy.</param>
        /// <param name="desiredConsistencyLevel">Consistency level.</param>
        public CloudClient(Uri serviceEndpoint, string authKeyOrResourceToken, 
            ConnectionPolicy connectionPolicy = null, 
            ConsistencyLevel? desiredConsistencyLevel = default(ConsistencyLevel?))
        {
            _client = new DocumentClient(serviceEndpoint, authKeyOrResourceToken, connectionPolicy, desiredConsistencyLevel);
        }

        /// <summary>
        /// Cloud client provider.
        /// </summary>
        /// <param name="serviceEndpoint">The service endpoint URL.</param>
        /// <param name="authKey">Authentication key.</param>
        /// <param name="connectionPolicy">Connection policy.</param>
        /// <param name="desiredConsistencyLevel">Consistency level.</param>
        public CloudClient(Uri serviceEndpoint, SecureString authKey, 
            ConnectionPolicy connectionPolicy = null, 
            ConsistencyLevel? desiredConsistencyLevel = default(ConsistencyLevel?))
        {
            _client = new DocumentClient(serviceEndpoint, authKey, connectionPolicy, desiredConsistencyLevel);
        }

        /// <summary>
        /// Cloud client provider.
        /// </summary>
        /// <param name="serviceEndpoint">The service endpoint URL.</param>
        /// <param name="resourceTokens">Resource tokens.</param>
        /// <param name="connectionPolicy">Connection policy.</param>
        /// <param name="desiredConsistencyLevel">Consistency level.</param>
        public CloudClient(Uri serviceEndpoint, IDictionary<string, string> resourceTokens, 
            ConnectionPolicy connectionPolicy = null, 
            ConsistencyLevel? desiredConsistencyLevel = default(ConsistencyLevel?))
        {
            _client = new DocumentClient(serviceEndpoint, resourceTokens, connectionPolicy, desiredConsistencyLevel);
        }

        /// <summary>
        /// Cloud client provider.
        /// </summary>
        /// <param name="serviceEndpoint">The service endpoint URL.</param>
        /// <param name="permissionFeed">Permission feed list.</param>
        /// <param name="connectionPolicy">Connection policy.</param>
        /// <param name="desiredConsistencyLevel">Consistency level.</param>
        public CloudClient(Uri serviceEndpoint, IList<Permission> permissionFeed, 
            ConnectionPolicy connectionPolicy = null, 
            ConsistencyLevel? desiredConsistencyLevel = default(ConsistencyLevel?))
        {
            _client = new DocumentClient(serviceEndpoint, permissionFeed, connectionPolicy, desiredConsistencyLevel);
        }

        private DocumentClient _client = null;

        /// <summary>
        /// Gets the cloud document client.
        /// </summary>
        public DocumentClient DocumentClient
        {
            get { return _client; }
        }

        /// <summary>
        /// Create a new database if it does not exist.
        /// </summary>
        /// <param name="databaseId">The name of the database to create.</param>
        /// <returns>The database reference.</returns>
        public Database CreateDatabase(string databaseId)
        {
            // Look for the database.
            Database db = _client.CreateDatabaseQuery()
                            .Where(d => d.Id == databaseId)
                            .AsEnumerable()
                            .FirstOrDefault();

            // In case there was no database matching, go ahead and create it. 
            if (db == null)
            {
                // Create the database.
                db = _client.CreateDatabaseAsync(new Database { Id = databaseId }).Result;
            }

            // Did not create, may already exist.
            return db;
        }

        /// <summary>
        /// Get the database if it exists.
        /// </summary>
        /// <param name="databaseId">The name of the database to get.</param>
        /// <returns>The database instance if exists; else null if does not exist.</returns>
        public Database GetDatabase(string databaseId)
        {
            // Look for the database.
            Database db = _client.CreateDatabaseQuery()
                            .Where(d => d.Id == databaseId)
                            .AsEnumerable()
                            .FirstOrDefault();

            // Return the database.
            return db;
        }

        /// <summary>
        /// Delete the database if it exist.
        /// </summary>
        /// <param name="databaseId">The name of the database to delete.</param>
        /// <returns>True if the database was deleted; else false.</returns>
        public bool DeleteDatabase(string databaseId)
        {
            // Look for the database.
            Database db = _client.CreateDatabaseQuery()
                            .Where(d => d.Id == databaseId)
                            .AsEnumerable()
                            .FirstOrDefault();

            // In case there was database matching, go ahead and delete it. 
            if (db != null)
            {
                // Delete the database.
                _client.DeleteDatabaseAsync(db.SelfLink).Wait();
                return true;
            }

            // Did not delete, may not exist.
            return false;
        }

        #region Dispose Object Methods

        private bool _disposed = false; // To detect redundant calls

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
        /// </summary>
        /// <param name="disposing">Is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects).
                    if (_client != null)
                        _client.Dispose();
                }

                // Free unmanaged resources (unmanaged objects) and override a finalizer below.
                
                _disposed = true;
                _client = null;
            }
        }

        /// <summary>
        /// /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~CloudClient()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
