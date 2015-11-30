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
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;

namespace Nequeo.Aws.Storage.SimpleDB
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
            _client = new AmazonSimpleDBClient(credentials);
        }

        /// <summary>
        /// Cloud account provider.
        /// </summary>
        /// <param name="credentials">credentials object for AWS services.</param>
        /// <param name="clientConfig">Configuration for accessing Amazon SimpleDB service.</param>
        public CloudAccount(AWSCredentials credentials, AmazonSimpleDBConfig clientConfig)
        {
            _client = new AmazonSimpleDBClient(credentials, clientConfig);
        }

        /// <summary>
        /// Cloud account provider.
        /// </summary>
        /// <param name="awsAccessKeyId">AWS Access Key ID.</param>
        /// <param name="awsSecretAccessKey">AWS Secret Access Key.</param>
        /// <param name="serviceUrl">AWS service URL.</param>
        public CloudAccount(string awsAccessKeyId, string awsSecretAccessKey, string serviceUrl)
        {
            AmazonSimpleDBConfig clientConfig = new AmazonSimpleDBConfig();
            clientConfig.ServiceURL = serviceUrl;
            _client = new AmazonSimpleDBClient(awsAccessKeyId, awsSecretAccessKey, clientConfig);
        }

        /// <summary>
        /// Cloud account provider.
        /// </summary>
        /// <param name="awsAccessKeyId">AWS Access Key ID.</param>
        /// <param name="awsSecretAccessKey">AWS Secret Access Key.</param>
        /// <param name="clientConfig">Configuration for accessing Amazon SimpleDB service</param>
        public CloudAccount(string awsAccessKeyId, string awsSecretAccessKey, AmazonSimpleDBConfig clientConfig)
        {
            _client = new AmazonSimpleDBClient(awsAccessKeyId, awsSecretAccessKey, clientConfig);
        }

        private AmazonSimpleDBClient _client = null;

        /// <summary>
        /// Gets the SimpleDB client.
        /// </summary>
        public AmazonSimpleDBClient AmazonSimpleDBClient
        {
            get { return _client; }
        }

        /// <summary>
        /// Create a new domain (table).
        /// </summary>
        /// <param name="domainName">The name of the domain to create. The name can range between 3 and 255 characters
        /// and can contain the following characters: a-z, A-Z, 0-9, '_', '-', and '.'.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task<CreateDomainResponse> CreateDomainAsync(string domainName)
        {
            CreateDomainRequest domainRequest = new CreateDomainRequest(domainName);
            return await _client.CreateDomainAsync(domainRequest);
        }

        /// <summary>
        /// Create a new domain (table).
        /// </summary>
        /// <param name="domainName">The name of the domain to create. The name can range between 3 and 255 characters
        /// and can contain the following characters: a-z, A-Z, 0-9, '_', '-', and '.'.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task<CreateDomainResponse> CreateDomainAsync(string domainName, CancellationToken cancellationToken)
        {
            CreateDomainRequest domainRequest = new CreateDomainRequest(domainName);
            return await _client.CreateDomainAsync(domainRequest, cancellationToken);
        }

        /// <summary>
        /// Delete a domain (table).
        /// </summary>
        /// <param name="domainName">The name of the domain.</param>
        /// <returns>The async task.</returns>
        public async Task<DeleteDomainResponse> DeleteDomainAsync(string domainName)
        {
            return await _client.DeleteDomainAsync(new DeleteDomainRequest(domainName));
        }

        /// <summary>
        /// Delete a domain (table).
        /// </summary>
        /// <param name="domainName">The name of the domain.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<DeleteDomainResponse> DeleteDomainAsync(string domainName, CancellationToken cancellationToken)
        {
            return await _client.DeleteDomainAsync(new DeleteDomainRequest(domainName), cancellationToken);
        }

        /// <summary>
        /// List domains.
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task<ListDomainsResponse> ListDomainsAsync()
        {
            return await _client.ListDomainsAsync();
        }

        /// <summary>
        /// List domains.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<ListDomainsResponse> ListDomainsAsync(CancellationToken cancellationToken)
        {
            return await _client.ListDomainsAsync(cancellationToken);
        }

        /// <summary>
        /// Add data to domain (table) and row (item).
        /// </summary>
        /// <param name="attributes">The domain and item names, with the attribute (column) and value.</param>
        /// <returns>The async task.</returns>
        public async Task<PutAttributesResponse> PutAttributesAsync(PutAttributesRequest attributes)
        {
            return await _client.PutAttributesAsync(attributes);
        }

        /// <summary>
        /// Add data to domain (table) and row (item).
        /// </summary>
        /// <param name="attributes">The domain and item names, with the attribute (column) and value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<PutAttributesResponse> PutAttributesAsync(PutAttributesRequest attributes, CancellationToken cancellationToken)
        {
            return await _client.PutAttributesAsync(attributes, cancellationToken);
        }

        /// <summary>
        /// Delete data from domain (table) and row (item).
        /// </summary>
        /// <param name="attributes">The domain and item names, with the attribute (column) and value.</param>
        /// <returns>The async task.</returns>
        public async Task<DeleteAttributesResponse> DeleteAttributesAsync(DeleteAttributesRequest attributes)
        {
            return await _client.DeleteAttributesAsync(attributes);
        }

        /// <summary>
        /// Delete data from domain (table) and row (item).
        /// </summary>
        /// <param name="attributes">The domain and item names, with the attribute (column) and value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<DeleteAttributesResponse> DeleteAttributesAsync(DeleteAttributesRequest attributes, CancellationToken cancellationToken)
        {
            return await _client.DeleteAttributesAsync(attributes, cancellationToken);
        }

        /// <summary>
        /// Batch add data to domain (table) and row (item).
        /// </summary>
        /// <param name="batchAttributes">Batch put attributes request.</param>
        /// <returns>The async task.</returns>
        public async Task<BatchPutAttributesResponse> BatchPutAttributesAsync(BatchPutAttributesRequest batchAttributes)
        {
            return await _client.BatchPutAttributesAsync(batchAttributes);
        }

        /// <summary>
        /// Batch add data to domain (table) and row (item).
        /// </summary>
        /// <param name="batchAttributes">Batch put attributes request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<BatchPutAttributesResponse> BatchPutAttributesAsync(BatchPutAttributesRequest batchAttributes, CancellationToken cancellationToken)
        {
            return await _client.BatchPutAttributesAsync(batchAttributes, cancellationToken);
        }
        
        /// <summary>
        /// Batch delete data from domain (table) and row (item).
        /// </summary>
        /// <param name="batchAttributes">Batch delete attributes request.</param>
        /// <returns>The async task.</returns>
        public async Task<BatchDeleteAttributesResponse> BatchDeleteAttributesAsync(BatchDeleteAttributesRequest batchAttributes)
        {
            return await _client.BatchDeleteAttributesAsync(batchAttributes);
        }

        /// <summary>
        /// Batch delete data from domain (table) and row (item).
        /// </summary>
        /// <param name="batchAttributes">Batch delete attributes request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<BatchDeleteAttributesResponse> BatchDeleteAttributesAsync(BatchDeleteAttributesRequest batchAttributes, CancellationToken cancellationToken)
        {
            return await _client.BatchDeleteAttributesAsync(batchAttributes, cancellationToken);
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
