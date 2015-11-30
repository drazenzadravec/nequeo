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

namespace Nequeo.Aws.Storage.DynamoDB
{
    /// <summary>
    /// Cloud account provider.
    /// </summary>
    public class CloudAccount : IDisposable
    {
        /// <summary>
        /// Cloud account provider.
        /// </summary>
        /// <param name="credentials">The credentials object for AWS services.</param>
        public CloudAccount(AWSCredentials credentials)
        {
            _client = new AmazonDynamoDBClient(credentials);
        }

        /// <summary>
        /// Cloud account provider.
        /// </summary>
        /// <param name="credentials">credentials object for AWS services.</param>
        /// <param name="clientConfig">Configuration for accessing Amazon DynamoDB service.</param>
        public CloudAccount(AWSCredentials credentials, AmazonDynamoDBConfig clientConfig)
        {
            _client = new AmazonDynamoDBClient(credentials, clientConfig);
        }

        /// <summary>
        /// Cloud account provider.
        /// </summary>
        /// <param name="awsAccessKeyId">AWS Access Key ID.</param>
        /// <param name="awsSecretAccessKey">AWS Secret Access Key.</param>
        /// <param name="serviceUrl">AWS service URL.</param>
        public CloudAccount(string awsAccessKeyId, string awsSecretAccessKey, string serviceUrl)
        {
            AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
            clientConfig.ServiceURL = serviceUrl;
            _client = new AmazonDynamoDBClient(awsAccessKeyId, awsSecretAccessKey, clientConfig);
        }

        /// <summary>
        /// Cloud account provider.
        /// </summary>
        /// <param name="awsAccessKeyId">AWS Access Key ID.</param>
        /// <param name="awsSecretAccessKey">AWS Secret Access Key.</param>
        /// <param name="clientConfig">Configuration for accessing Amazon DynamoDB service</param>
        public CloudAccount(string awsAccessKeyId, string awsSecretAccessKey, AmazonDynamoDBConfig clientConfig)
        {
            _client = new AmazonDynamoDBClient(awsAccessKeyId, awsSecretAccessKey, clientConfig);
        }

        private AmazonDynamoDBClient _client = null;

        /// <summary>
        /// Gets the DynamoDB client.
        /// </summary>
        public AmazonDynamoDBClient AmazonDynamoDBClient
        {
            get { return _client; }
        }

        /// <summary>
        /// Create a table async.
        /// </summary>
        /// <param name="request">The create table request.</param>
        /// <returns>The async task.</returns>
        public async Task<CreateTableResponse> CreateTableAsync(CreateTableRequest request)
        {
            return await _client.CreateTableAsync(request);
        }

        /// <summary>
        /// Create a table async.
        /// </summary>
        /// <param name="request">The create table request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<CreateTableResponse> CreateTableAsync(CreateTableRequest request, CancellationToken cancellationToken)
        {
            return await _client.CreateTableAsync(request, cancellationToken);
        }

        /// <summary>
        /// Delete a table async.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The async task.</returns>
        public async Task<DeleteTableResponse> DeleteTableAsync(string tableName)
        {
            return await _client.DeleteTableAsync(new DeleteTableRequest(tableName));
        }

        /// <summary>
        /// Delete a table async.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<DeleteTableResponse> DeleteTableAsync(string tableName, CancellationToken cancellationToken)
        {
            return await _client.DeleteTableAsync(new DeleteTableRequest(tableName), cancellationToken);
        }

        /// <summary>
        /// List all tables.
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task<ListTablesResponse> ListTablesAsync()
        {
            return await _client.ListTablesAsync();
        }

        /// <summary>
        /// List all tables.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<ListTablesResponse> ListTablesAsync(CancellationToken cancellationToken)
        {
            return await _client.ListTablesAsync(cancellationToken);
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
        ~CloudAccount()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
