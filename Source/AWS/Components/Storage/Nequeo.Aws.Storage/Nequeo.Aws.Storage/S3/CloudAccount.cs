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
using Amazon.S3;
using Amazon.S3.Model;

namespace Nequeo.Aws.Storage.S3
{
    /// <summary>
    /// Cloud account provider (Simple Storage Service).
    /// </summary>
    public class CloudAccount : IDisposable
    {
        /// <summary>
        /// Cloud account provider.
        /// </summary>
        /// <param name="credentials">The credentials object for AWS services.</param>
        public CloudAccount(AWSCredentials credentials)
        {
            _client = new AmazonS3Client(credentials);
        }

        /// <summary>
        /// Cloud account provider.
        /// </summary>
        /// <param name="credentials">credentials object for AWS services.</param>
        /// <param name="clientConfig">Configuration for accessing Amazon DynamoDB service.</param>
        public CloudAccount(AWSCredentials credentials, AmazonS3Config clientConfig)
        {
            _client = new AmazonS3Client(credentials, clientConfig);
        }

        /// <summary>
        /// Cloud account provider.
        /// </summary>
        /// <param name="awsAccessKeyId">AWS Access Key ID.</param>
        /// <param name="awsSecretAccessKey">AWS Secret Access Key.</param>
        /// <param name="serviceUrl">AWS service URL.</param>
        public CloudAccount(string awsAccessKeyId, string awsSecretAccessKey, string serviceUrl)
        {
            AmazonS3Config clientConfig = new AmazonS3Config();
            clientConfig.ServiceURL = serviceUrl;
            _client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, clientConfig);
        }

        /// <summary>
        /// Cloud account provider.
        /// </summary>
        /// <param name="awsAccessKeyId">AWS Access Key ID.</param>
        /// <param name="awsSecretAccessKey">AWS Secret Access Key.</param>
        /// <param name="clientConfig">Configuration for accessing Amazon DynamoDB service</param>
        public CloudAccount(string awsAccessKeyId, string awsSecretAccessKey, AmazonS3Config clientConfig)
        {
            _client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, clientConfig);
        }

        private AmazonS3Client _client = null;

        /// <summary>
        /// Gets the S3 client.
        /// </summary>
        public AmazonS3Client AmazonS3Client
        {
            get { return _client; }
        }

        /// <summary>
        /// Creates a new bucket.
        /// </summary>
        /// <param name="bucketName">The bucket name.</param>
        /// <returns>The async task.</returns>
        public async Task<PutBucketResponse> CreateBucketAsync(string bucketName)
        {
            return await _client.PutBucketAsync(bucketName);
        }

        /// <summary>
        /// Creates a new bucket.
        /// </summary>
        /// <param name="bucketName">The bucket name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<PutBucketResponse> CreateBucketAsync(string bucketName, CancellationToken cancellationToken)
        {
            return await _client.PutBucketAsync(bucketName, cancellationToken);
        }


        /// <summary>
        /// Delete a bucket.
        /// </summary>
        /// <param name="bucketName">The bucket name.</param>
        /// <returns>The async task.</returns>
        public async Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName)
        {
            return await _client.DeleteBucketAsync(bucketName);
        }

        /// <summary>
        /// Delete a bucket.
        /// </summary>
        /// <param name="bucketName">The bucket name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, CancellationToken cancellationToken)
        {
            return await _client.DeleteBucketAsync(bucketName, cancellationToken);
        }

        /// <summary>
        /// Returns a list of all buckets owned by the authenticated sender of the request.
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task<ListBucketsResponse> ListBucketsAsync()
        {
            return await _client.ListBucketsAsync();
        }

        /// <summary>
        /// Returns a list of all buckets owned by the authenticated sender of the request.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<ListBucketsResponse> ListBucketsAsync(CancellationToken cancellationToken)
        {
            return await _client.ListBucketsAsync(cancellationToken);
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
