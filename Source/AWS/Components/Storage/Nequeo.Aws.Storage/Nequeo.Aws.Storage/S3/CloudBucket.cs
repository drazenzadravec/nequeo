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
    /// Represents the base object type for a bucket entity in the S3 service.
    /// </summary>
    public class CloudBucket
    {
        /// <summary>
        /// Represents the base object type for a bucket entity in the S3 service.
        /// </summary>
        /// <param name="cloudAccount">Cloud account provider.</param>
        /// <param name="bucketName">The name of the bucket used in this cloud bucket.</param>
        public CloudBucket(CloudAccount cloudAccount, string bucketName)
        {
            if (String.IsNullOrEmpty(bucketName)) throw new ArgumentNullException(nameof(bucketName));

            _account = cloudAccount;
            _bucketName = bucketName;
            _client = cloudAccount.AmazonS3Client;
        }

        private string _bucketName = null;
        private CloudAccount _account = null;
        private AmazonS3Client _client = null;

        /// <summary>
        /// Get object async.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns>The async task.</returns>
        public async Task<GetObjectResponse> GetObjectAsync(string key)
        {
            return await _client.GetObjectAsync(_bucketName, key);
        }

        /// <summary>
        /// Get object async.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<GetObjectResponse> GetObjectAsync(string key, CancellationToken cancellationToken)
        {
            return await _client.GetObjectAsync(_bucketName, key, cancellationToken);
        }

        /// <summary>
        /// Get object async.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="versionId">The version id of the object.</param>
        /// <returns>The async task.</returns>
        public async Task<GetObjectResponse> GetObjectAsync(string key, string versionId)
        {
            return await _client.GetObjectAsync(_bucketName, key, versionId);
        }

        /// <summary>
        /// Get object async.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="versionId">The version id of the object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<GetObjectResponse> GetObjectAsync(string key, string versionId, CancellationToken cancellationToken)
        {
            return await _client.GetObjectAsync(_bucketName, key, versionId, cancellationToken);
        }

        /// <summary>
        /// List all objects in the bucket async.
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task<ListObjectsResponse> ListObjectsAsync()
        {
            return await _client.ListObjectsAsync(_bucketName);
        }

        /// <summary>
        /// List all objects in the bucket async.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<ListObjectsResponse> ListObjectsAsync(CancellationToken cancellationToken)
        {
            return await _client.ListObjectsAsync(_bucketName, cancellationToken);
        }

        /// <summary>
        /// Get the object metadata async.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns>The async task.</returns>
        public async Task<GetObjectMetadataResponse> GetObjectMetadataAsync(string key)
        {
            return await _client.GetObjectMetadataAsync(_bucketName, key);
        }

        /// <summary>
        /// Get the object metadata async.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<GetObjectMetadataResponse> GetObjectMetadataAsync(string key, CancellationToken cancellationToken)
        {
            return await _client.GetObjectMetadataAsync(_bucketName, key, cancellationToken);
        }

        /// <summary>
        /// Get the object metadata async.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="versionId">The version id of the object.</param>
        /// <returns>The async task.</returns>
        public async Task<GetObjectMetadataResponse> GetObjectMetadataAsync(string key, string versionId)
        {
            return await _client.GetObjectMetadataAsync(_bucketName, key, versionId);
        }

        /// <summary>
        /// Get the object metadata async.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="versionId">The version id of the object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<GetObjectMetadataResponse> GetObjectMetadataAsync(string key, string versionId, CancellationToken cancellationToken)
        {
            return await _client.GetObjectMetadataAsync(_bucketName, key, versionId, cancellationToken);
        }

        /// <summary>
        /// Add the object to the store.
        /// </summary>
        /// <param name="inputStream">The stream containing the data to upload.</param>
        /// <param name="key">The key of the object.</param>
        /// <returns>The async task.</returns>
        public async Task<PutObjectResponse> AddObjectAsync(System.IO.Stream inputStream, string key)
        {
            PutObjectRequest request = new PutObjectRequest();
            request.BucketName = _bucketName;
            request.Key = key;
            request.InputStream = inputStream;
            return await _client.PutObjectAsync(request);
        }

        /// <summary>
        /// Add the object to the store.
        /// </summary>
        /// <param name="inputStream">The stream containing the data to upload.</param>
        /// <param name="key">The key of the object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<PutObjectResponse> AddObjectAsync(System.IO.Stream inputStream, string key, CancellationToken cancellationToken)
        {
            PutObjectRequest request = new PutObjectRequest();
            request.BucketName = _bucketName;
            request.Key = key;
            request.InputStream = inputStream;
            return await _client.PutObjectAsync(request, cancellationToken);
        }

        /// <summary>
        /// Add the object to the store.
        /// </summary>
        /// <param name="input">The string containing the data to upload.</param>
        /// <param name="key">The key of the object.</param>
        /// <returns>The async task.</returns>
        public async Task<PutObjectResponse> AddObjectAsync(string input, string key)
        {
            PutObjectRequest request = new PutObjectRequest();
            request.BucketName = _bucketName;
            request.Key = key;
            request.ContentBody = input;
            return await _client.PutObjectAsync(request);
        }

        /// <summary>
        /// Add the object to the store.
        /// </summary>
        /// <param name="input">The string containing the data to upload.</param>
        /// <param name="key">The key of the object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<PutObjectResponse> AddObjectAsync(string input, string key, CancellationToken cancellationToken)
        {
            PutObjectRequest request = new PutObjectRequest();
            request.BucketName = _bucketName;
            request.Key = key;
            request.ContentBody = input;
            return await _client.PutObjectAsync(request, cancellationToken);
        }

        /// <summary>
        /// Add the object to the store.
        /// </summary>
        /// <param name="fileName">The path and name of the file to upload.</param>
        /// <param name="key">The key of the object.</param>
        /// <returns>The async task.</returns>
        public async Task<PutObjectResponse> AddFileObjectAsync(string fileName, string key)
        {
            PutObjectRequest request = new PutObjectRequest();
            request.BucketName = _bucketName;
            request.Key = key;
            request.FilePath = fileName;
            return await _client.PutObjectAsync(request);
        }

        /// <summary>
        /// Add the object to the store.
        /// </summary>
        /// <param name="fileName">The path and name of the file to upload.</param>
        /// <param name="key">The key of the object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<PutObjectResponse> AddFileObjectAsync(string fileName, string key, CancellationToken cancellationToken)
        {
            PutObjectRequest request = new PutObjectRequest();
            request.BucketName = _bucketName;
            request.Key = key;
            request.FilePath = fileName;
            return await _client.PutObjectAsync(request, cancellationToken);
        }

        /// <summary>
        /// Get the object from the store.
        /// </summary>
        /// <param name="fileName">The path and name of the file to download to.</param>
        /// <param name="key">The key of the object.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> GetFileObjectAsync(string fileName, string key)
        {
            GetObjectResponse response = await _client.GetObjectAsync(_bucketName, key);
            long size = response.ContentLength;

            // If data exists.
            if (size > 0)
            {
                // Write the data to the file.
                await response.WriteResponseStreamToFileAsync(fileName, false, CancellationToken.None);
                return true;
            }
            else
            {
                // No data.
                return false;
            }
        }

        /// <summary>
        /// Get the object from the store.
        /// </summary>
        /// <param name="fileName">The path and name of the file to download to.</param>
        /// <param name="key">The key of the object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> GetFileObjectAsync(string fileName, string key, CancellationToken cancellationToken)
        {
            GetObjectResponse response = await _client.GetObjectAsync(_bucketName, key, cancellationToken);
            long size = response.ContentLength;

            // If data exists.
            if (size > 0)
            {
                // Write the data to the file.
                await response.WriteResponseStreamToFileAsync(fileName, false, CancellationToken.None);
                return true;
            }
            else
            {
                // No data.
                return false;
            }
        }

        /// <summary>
        /// Get the object from the store.
        /// </summary>
        /// <param name="fileName">The path and name of the file to download to.</param>
        /// <param name="key">The key of the object.</param>
        /// <param name="versionId">The version id of the object.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> GetFileObjectAsync(string fileName, string key, string versionId)
        {
            GetObjectResponse response = await _client.GetObjectAsync(_bucketName, key, versionId);
            long size = response.ContentLength;

            // If data exists.
            if (size > 0)
            {
                // Write the data to the file.
                await response.WriteResponseStreamToFileAsync(fileName, false, CancellationToken.None);
                return true;
            }
            else
            {
                // No data.
                return false;
            }
        }

        /// <summary>
        /// Get the object from the store.
        /// </summary>
        /// <param name="fileName">The path and name of the file to download to.</param>
        /// <param name="key">The key of the object.</param>
        /// <param name="versionId">The version id of the object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> GetFileObjectAsync(string fileName, string key, string versionId, CancellationToken cancellationToken)
        {
            GetObjectResponse response = await _client.GetObjectAsync(_bucketName, key, versionId, cancellationToken);
            long size = response.ContentLength;

            // If data exists.
            if (size > 0)
            {
                // Write the data to the file.
                await response.WriteResponseStreamToFileAsync(fileName, false, CancellationToken.None);
                return true;
            }
            else
            {
                // No data.
                return false;
            }
        }

        /// <summary>
        /// Get the object from the store.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns>The async task.</returns>
        public async Task<string> GetTextObjectAsync(string key)
        {
            GetObjectResponse response = await _client.GetObjectAsync(_bucketName, key);
            long size = response.ContentLength;
            bool result = false;

            // Create a local memory stream.
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                // Copy the stream data.
                result = Nequeo.IO.Stream.Operation.CopyStream(response.ResponseStream, stream, size, 30000);

                // If data has been copies.
                if(stream.Length > 0 && result)
                {
                    // Get the string data.
                    string data = Encoding.Default.GetString(stream.ToArray());
                    return data;
                }
                else
                {
                    // Return null no data.
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the object from the store.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<string> GetTextObjectAsync(string key, CancellationToken cancellationToken)
        {
            GetObjectResponse response = await _client.GetObjectAsync(_bucketName, key, cancellationToken);
            long size = response.ContentLength;
            bool result = false;

            // Create a local memory stream.
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                // Copy the stream data.
                result = Nequeo.IO.Stream.Operation.CopyStream(response.ResponseStream, stream, size, 30000);

                // If data has been copies.
                if (stream.Length > 0 && result)
                {
                    // Get the string data.
                    string data = Encoding.Default.GetString(stream.ToArray());
                    return data;
                }
                else
                {
                    // Return null no data.
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the object from the store.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="versionId">The version id of the object.</param>
        /// <returns>The async task.</returns>
        public async Task<string> GetTextObjectAsync(string key, string versionId)
        {
            GetObjectResponse response = await _client.GetObjectAsync(_bucketName, key, versionId);
            long size = response.ContentLength;
            bool result = false;

            // Create a local memory stream.
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                // Copy the stream data.
                result = Nequeo.IO.Stream.Operation.CopyStream(response.ResponseStream, stream, size, 30000);

                // If data has been copies.
                if (stream.Length > 0 && result)
                {
                    // Get the string data.
                    string data = Encoding.Default.GetString(stream.ToArray());
                    return data;
                }
                else
                {
                    // Return null no data.
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the object from the store.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="versionId">The version id of the object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<string> GetTextObjectAsync(string key, string versionId, CancellationToken cancellationToken)
        {
            GetObjectResponse response = await _client.GetObjectAsync(_bucketName, key, versionId, cancellationToken);
            long size = response.ContentLength;
            bool result = false;

            // Create a local memory stream.
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                // Copy the stream data.
                result = Nequeo.IO.Stream.Operation.CopyStream(response.ResponseStream, stream, size, 30000);

                // If data has been copies.
                if (stream.Length > 0 && result)
                {
                    // Get the string data.
                    string data = Encoding.Default.GetString(stream.ToArray());
                    return data;
                }
                else
                {
                    // Return null no data.
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the object from the store.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns>The async task.</returns>
        public async Task<byte[]> GetBytesObjectAsync(string key)
        {
            GetObjectResponse response = await _client.GetObjectAsync(_bucketName, key);
            long size = response.ContentLength;
            bool result = false;

            // Create a local memory stream.
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                // Copy the stream data.
                result = Nequeo.IO.Stream.Operation.CopyStream(response.ResponseStream, stream, size, 30000);

                // If data has been copies.
                if (stream.Length > 0 && result)
                {
                    // Get the bytes data.
                    return stream.ToArray();
                }
                else
                {
                    // Return null no data.
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the object from the store.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<byte[]> GetBytesObjectAsync(string key, CancellationToken cancellationToken)
        {
            GetObjectResponse response = await _client.GetObjectAsync(_bucketName, key, cancellationToken);
            long size = response.ContentLength;
            bool result = false;

            // Create a local memory stream.
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                // Copy the stream data.
                result = Nequeo.IO.Stream.Operation.CopyStream(response.ResponseStream, stream, size, 30000);

                // If data has been copies.
                if (stream.Length > 0 && result)
                {
                    // Get the bytes data.
                    return stream.ToArray();
                }
                else
                {
                    // Return null no data.
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the object from the store.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="versionId">The version id of the object.</param>
        /// <returns>The async task.</returns>
        public async Task<byte[]> GetBytesObjectAsync(string key, string versionId)
        {
            GetObjectResponse response = await _client.GetObjectAsync(_bucketName, key, versionId);
            long size = response.ContentLength;
            bool result = false;

            // Create a local memory stream.
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                // Copy the stream data.
                result = Nequeo.IO.Stream.Operation.CopyStream(response.ResponseStream, stream, size, 30000);

                // If data has been copies.
                if (stream.Length > 0 && result)
                {
                    // Get the bytes data.
                    return stream.ToArray();
                }
                else
                {
                    // Return null no data.
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the object from the store.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="versionId">The version id of the object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<byte[]> GetBytesObjectAsync(string key, string versionId, CancellationToken cancellationToken)
        {
            GetObjectResponse response = await _client.GetObjectAsync(_bucketName, key, versionId, cancellationToken);
            long size = response.ContentLength;
            bool result = false;

            // Create a local memory stream.
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                // Copy the stream data.
                result = Nequeo.IO.Stream.Operation.CopyStream(response.ResponseStream, stream, size, 30000);

                // If data has been copies.
                if (stream.Length > 0 && result)
                {
                    // Get the bytes data.
                    return stream.ToArray();
                }
                else
                {
                    // Return null no data.
                    return null;
                }
            }
        }
    }
}
