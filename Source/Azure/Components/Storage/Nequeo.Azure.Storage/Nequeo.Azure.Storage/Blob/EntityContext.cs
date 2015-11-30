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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Nequeo.Azure.Storage.Blob
{
    /// <summary>
    /// Represents the base object type for a blob entity in the blob service.
    /// </summary>
    public class EntityContext
    {
        /// <summary>
        /// Represents the base object type for a blob entity in the blob service.
        /// </summary>
        /// <param name="cloudBlob">Represents a Windows Azure blob.</param>
        public EntityContext(CloudBlobContainer cloudBlob)
        {
            _cloudBlob = cloudBlob;
        }

        private CloudBlobContainer _cloudBlob = null;

        /// <summary>
        /// Gets the cloud blob.
        /// </summary>
        public CloudBlobContainer CloudBlobContainer
        {
            get { return _cloudBlob; }
        }

        /// <summary>
        /// Delete blob async.
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task<bool> DeleteBlobAsync()
        {
            return await _cloudBlob.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Delete blob async.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> DeleteBlobAsync(CancellationToken cancellationToken)
        {
            return await _cloudBlob.DeleteIfExistsAsync(cancellationToken);
        }

        /// <summary>
        /// Create a blob if it does not exist.
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task<bool> CreateBlobAsync()
        {
            bool result = await _cloudBlob.CreateIfNotExistsAsync();
            return result;
        }

        /// <summary>
        /// Create a blob if it does not exist.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> CreateBlobAsync(CancellationToken cancellationToken)
        {
            bool result = await _cloudBlob.CreateIfNotExistsAsync(cancellationToken);
            return result;
        }

        /// <summary>
        /// Get all the cloud blob.
        /// </summary>
        /// <returns>The list of cloud blob.</returns>
        public IEnumerable<IListBlobItem> GetBlobs()
        {
            return _cloudBlob.ListBlobs();
        }

        /// <summary>
        /// Get the block blob.
        /// </summary>
        /// <param name="blobName">The blob name.</param>
        /// <returns>The block blob.</returns>
        public CloudBlockBlob GetBlockBlob(string blobName)
        {
            return _cloudBlob.GetBlockBlobReference(blobName);
        }

        /// <summary>
        /// Get the block blob.
        /// </summary>
        /// <param name="blobName">The blob name.</param>
        /// <param name="snapshotTime">A System.DateTimeOffset specifying the snapshot timestamp, if the blob is a snapshot.</param>
        /// <returns>The block blob.</returns>
        public CloudBlockBlob GetBlockBlob(string blobName, DateTimeOffset snapshotTime)
        {
            return _cloudBlob.GetBlockBlobReference(blobName, snapshotTime);
        }

        /// <summary>
        /// Get the block blob.
        /// </summary>
        /// <param name="blobName">The blob name.</param>
        /// <returns>The block blob.</returns>
        public CloudPageBlob GetPageBlob(string blobName)
        {
            return _cloudBlob.GetPageBlobReference(blobName);
        }

        /// <summary>
        /// Get the block blob.
        /// </summary>
        /// <param name="blobName">The blob name.</param>
        /// <param name="snapshotTime">A System.DateTimeOffset specifying the snapshot timestamp, if the blob is a snapshot.</param>
        /// <returns>The block blob.</returns>
        public CloudPageBlob GetPageBlob(string blobName, DateTimeOffset snapshotTime)
        {
            return _cloudBlob.GetPageBlobReference(blobName, snapshotTime);
        }

        /// <summary>
        /// Delete the blob async.
        /// </summary>
        /// <param name="blob">The cloud blob to delete.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> DeleteBlobAsync(CloudBlockBlob blob)
        {
            return await blob.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Delete the blob async.
        /// </summary>
        /// <param name="blob">The cloud blob to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> DeleteBlobAsync(CloudBlockBlob blob, CancellationToken cancellationToken)
        {
            return await blob.DeleteIfExistsAsync(cancellationToken);
        }

        /// <summary>
        /// Delete the blob async.
        /// </summary>
        /// <param name="blob">The cloud blob to delete.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> DeleteBlobAsync(CloudPageBlob blob)
        {
            return await blob.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Delete the blob async.
        /// </summary>
        /// <param name="blob">The cloud blob to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> DeleteBlobAsync(CloudPageBlob blob, CancellationToken cancellationToken)
        {
            return await blob.DeleteIfExistsAsync(cancellationToken);
        }

        /// <summary>
        /// Create a page blob.
        /// </summary>
        /// <param name="blob">The page blob.</param>
        /// <param name="size">The maximum size of the blob, in bytes.</param>
        /// <returns>The async task.</returns>
        public async Task CreateAsync(CloudPageBlob blob, long size)
        {
            await blob.CreateAsync(size);
        }

        /// <summary>
        /// Create a page blob.
        /// </summary>
        /// <param name="blob">The page blob.</param>
        /// <param name="size">The maximum size of the blob, in bytes.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task CreateAsync(CloudPageBlob blob, long size, CancellationToken cancellationToken)
        {
            await blob.CreateAsync(size, cancellationToken);
        }

        /// <summary>
        /// Resize a page blob.
        /// </summary>
        /// <param name="blob">The page blob.</param>
        /// <param name="size">The maximum size of the blob, in bytes.</param>
        /// <returns>The async task.</returns>
        public async Task ResizeAsync(CloudPageBlob blob, long size)
        {
            await blob.ResizeAsync(size);
        }

        /// <summary>
        /// Resize a page blob.
        /// </summary>
        /// <param name="blob">The page blob.</param>
        /// <param name="size">The maximum size of the blob, in bytes.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task ResizeAsync(CloudPageBlob blob, long size, CancellationToken cancellationToken)
        {
            await blob.ResizeAsync(size, cancellationToken);
        }

        /// <summary>
        /// Upload the local blob.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="path">The local path and file name.</param>
        /// <returns>The async task.</returns>
        public async Task UploadAsync(CloudBlockBlob blob, string path)
        {
            await blob.UploadFromFileAsync(path, FileMode.Open);
        }

        /// <summary>
        /// Upload the local blob.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="path">The local path and file name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task UploadAsync(CloudBlockBlob blob, string path, CancellationToken cancellationToken)
        {
            await blob.UploadFromFileAsync(path, FileMode.Open, cancellationToken);
        }

        /// <summary>
        /// Upload the stream data.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="stream">The stream containing the data.</param>
        /// <returns>The async task.</returns>
        public async Task UploadAsync(CloudBlockBlob blob, Stream stream)
        {
            await blob.UploadFromStreamAsync(stream);
        }

        /// <summary>
        /// Upload the stream data.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="stream">The stream containing the data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task UploadAsync(CloudBlockBlob blob, Stream stream, CancellationToken cancellationToken)
        {
            await blob.UploadFromStreamAsync(stream, cancellationToken);
        }

        /// <summary>
        /// Upload the byte array.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="buffer">The data to upload.</param>
        /// <param name="index">The buffer offset index.</param>
        /// <param name="count">The number of bytes in the buffer to read.</param>
        /// <returns>The async task.</returns>
        public async Task UploadAsync(CloudBlockBlob blob, byte[] buffer, int index, int count)
        {
            await blob.UploadFromByteArrayAsync(buffer, index, count);
        }

        /// <summary>
        /// Upload the byte array.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="buffer">The data to upload.</param>
        /// <param name="index">The buffer offset index.</param>
        /// <param name="count">The number of bytes in the buffer to read.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task UploadAsync(CloudBlockBlob blob, byte[] buffer, int index, int count, CancellationToken cancellationToken)
        {
            await blob.UploadFromByteArrayAsync(buffer, index, count, cancellationToken);
        }

        /// <summary>
        /// Upload the text data.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="text">The text containing the data.</param>
        /// <returns>The async task.</returns>
        public async Task UploadTextAsync(CloudBlockBlob blob, string text)
        {
            await blob.UploadTextAsync(text);
        }

        /// <summary>
        /// Upload the text data.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="text">The text containing the data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task UploadTextAsync(CloudBlockBlob blob, string text, CancellationToken cancellationToken)
        {
            await blob.UploadTextAsync(text, cancellationToken);
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="stream">The stream to write to.</param>
        /// <returns>The async task.</returns>
        public async Task DownloadAsync(CloudBlockBlob blob, Stream stream)
        {
            await blob.DownloadToStreamAsync(stream);
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task DownloadAsync(CloudBlockBlob blob, Stream stream, CancellationToken cancellationToken)
        {
            await blob.DownloadToStreamAsync(stream, cancellationToken);
        }

        /// <summary>
        /// Download data to the text.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <returns>The async task.</returns>
        public async Task<string> DownloadAsync(CloudBlockBlob blob)
        {
            return await blob.DownloadTextAsync();
        }

        /// <summary>
        /// Download data from text.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<string> DownloadAsync(CloudBlockBlob blob, CancellationToken cancellationToken)
        {
            return await blob.DownloadTextAsync(cancellationToken);
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="target">The byte array where to write to.</param>
        /// <param name="index">The offset index.</param>
        /// <returns>The async task.</returns>
        public async Task<int> DownloadAsync(CloudBlockBlob blob, byte[] target, int index)
        {
            return await blob.DownloadToByteArrayAsync(target, index);
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="target">The byte array where to write to.</param>
        /// <param name="index">The offset index.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<int> DownloadAsync(CloudBlockBlob blob, byte[] target, int index, CancellationToken cancellationToken)
        {
            return await blob.DownloadToByteArrayAsync(target, index, cancellationToken);
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="path">The path and file name to write to.</param>
        /// <returns>The async task.</returns>
        public async Task DownloadAsync(CloudBlockBlob blob, string path)
        {
            await blob.DownloadToFileAsync(path, FileMode.Create);
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="path">The path and file name to write to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task DownloadAsync(CloudBlockBlob blob, string path, CancellationToken cancellationToken)
        {
            await blob.DownloadToFileAsync(path, FileMode.Create, cancellationToken);
        }

        /// <summary>
        /// Upload the local blob.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="path">The local path and file name.</param>
        /// <returns>The async task.</returns>
        public async Task UploadAsync(CloudPageBlob blob, string path)
        {
            await blob.UploadFromFileAsync(path, FileMode.Open);
        }

        /// <summary>
        /// Upload the local blob.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="path">The local path and file name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task UploadAsync(CloudPageBlob blob, string path, CancellationToken cancellationToken)
        {
            await blob.UploadFromFileAsync(path, FileMode.Open, cancellationToken);
        }

        /// <summary>
        /// Upload the stream data.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="stream">The stream containing the data.</param>
        /// <returns>The async task.</returns>
        public async Task UploadAsync(CloudPageBlob blob, Stream stream)
        {
            await blob.UploadFromStreamAsync(stream);
        }

        /// <summary>
        /// Upload the stream data.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="stream">The stream containing the data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task UploadAsync(CloudPageBlob blob, Stream stream, CancellationToken cancellationToken)
        {
            await blob.UploadFromStreamAsync(stream, cancellationToken);
        }

        /// <summary>
        /// Upload the byte array.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="buffer">The data to upload.</param>
        /// <param name="index">The buffer offset index.</param>
        /// <param name="count">The number of bytes in the buffer to read.</param>
        /// <returns>The async task.</returns>
        public async Task UploadAsync(CloudPageBlob blob, byte[] buffer, int index, int count)
        {
            await blob.UploadFromByteArrayAsync(buffer, index, count);
        }

        /// <summary>
        /// Upload the byte array.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="buffer">The data to upload.</param>
        /// <param name="index">The buffer offset index.</param>
        /// <param name="count">The number of bytes in the buffer to read.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task UploadAsync(CloudPageBlob blob, byte[] buffer, int index, int count, CancellationToken cancellationToken)
        {
            await blob.UploadFromByteArrayAsync(buffer, index, count, cancellationToken);
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="stream">The stream to write to.</param>
        /// <returns>The async task.</returns>
        public async Task DownloadAsync(CloudPageBlob blob, Stream stream)
        {
            await blob.DownloadToStreamAsync(stream);
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task DownloadAsync(CloudPageBlob blob, Stream stream, CancellationToken cancellationToken)
        {
            await blob.DownloadToStreamAsync(stream, cancellationToken);
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="target">The byte array where to write to.</param>
        /// <param name="index">The offset index.</param>
        /// <returns>The async task.</returns>
        public async Task<int> DownloadAsync(CloudPageBlob blob, byte[] target, int index)
        {
            return await blob.DownloadToByteArrayAsync(target, index);
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="target">The byte array where to write to.</param>
        /// <param name="index">The offset index.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<int> DownloadAsync(CloudPageBlob blob, byte[] target, int index, CancellationToken cancellationToken)
        {
            return await blob.DownloadToByteArrayAsync(target, index, cancellationToken);
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="path">The path and file name to write to.</param>
        /// <returns>The async task.</returns>
        public async Task DownloadAsync(CloudPageBlob blob, string path)
        {
            await blob.DownloadToFileAsync(path, FileMode.Create);
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="path">The path and file name to write to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task DownloadAsync(CloudPageBlob blob, string path, CancellationToken cancellationToken)
        {
            await blob.DownloadToFileAsync(path, FileMode.Create, cancellationToken);
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="target">The byte array where to write to.</param>
        /// <param name="index">The offset index.</param>
        /// <param name="blobOffset">The starting offset of the data range, in bytes.</param>
        /// <param name="length">The length of the data to download from the blob, in bytes.</param>
        /// <returns>The async task.</returns>
        public async Task<int> DownloadAsync(CloudPageBlob blob, byte[] target, int index, long blobOffset, long length)
        {
            return await blob.DownloadRangeToByteArrayAsync(target, index, blobOffset, length);
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="blob">The cloud blob.</param>
        /// <param name="target">The byte array where to write to.</param>
        /// <param name="index">The offset index.</param>
        /// <param name="blobOffset">The starting offset of the data range, in bytes.</param>
        /// <param name="length">The length of the data to download from the blob, in bytes.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<int> DownloadAsync(CloudPageBlob blob, byte[] target, int index, long blobOffset, long length, CancellationToken cancellationToken)
        {
            return await blob.DownloadRangeToByteArrayAsync(target, index, blobOffset, length, cancellationToken);
        }
    }
}
