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
using Microsoft.WindowsAzure.Storage.File;

namespace Nequeo.Azure.Storage.File
{
    /// <summary>
    /// Represents the base object type for a file entity in the file share service.
    /// </summary>
    public class EntityContext
    {
        /// <summary>
        /// Represents the base object type for a file entity in the file share service.
        /// </summary>
        /// <param name="cloudFileShare">Represents a Windows Azure file.</param>
        public EntityContext(CloudFileShare cloudFileShare)
        {
            _cloudFileShare = cloudFileShare;
        }

        private CloudFileShare _cloudFileShare = null;

        /// <summary>
        /// Gets the cloud table.
        /// </summary>
        public CloudFileShare CloudFileShare
        {
            get { return _cloudFileShare; }
        }

        /// <summary>
        /// Get the list of all files and directories under the root directory.
        /// </summary>
        /// <param name="root">The share root directory.</param>
        /// <returns>The async task.</returns>
        public async Task<IListFileItem[]> GetItems(CloudFileDirectory root)
        {
            List<IListFileItem> results = new List<IListFileItem>();
            FileContinuationToken token = null;

            do
            {
                // Get the set of data.
                FileResultSegment resultSegment = await root.ListFilesAndDirectoriesSegmentedAsync(token);
                results.AddRange(resultSegment.Results);
                token = resultSegment.ContinuationToken;
            }
            while (token != null);

            // Return the list.
            return results.ToArray();
        }

        /// <summary>
        /// Get the list of all files and directories under the root directory.
        /// </summary>
        /// <param name="root">The share root directory.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<IListFileItem[]> GetItems(CloudFileDirectory root, CancellationToken cancellationToken)
        {
            List<IListFileItem> results = new List<IListFileItem>();
            FileContinuationToken token = null;

            do
            {
                // Get the set of data.
                FileResultSegment resultSegment = await root.ListFilesAndDirectoriesSegmentedAsync(token, cancellationToken);
                results.AddRange(resultSegment.Results);
                token = resultSegment.ContinuationToken;
            }
            while (token != null);

            // Return the list.
            return results.ToArray();
        }

        /// <summary>
        /// Upload the text data.
        /// </summary>
        /// <param name="file">The cloud file.</param>
        /// <param name="text">The text containing the data.</param>
        /// <returns>The async task.</returns>
        public async Task UploadTextAsync(CloudFile file, string text)
        {
            await file.UploadTextAsync(text);
        }

        /// <summary>
        /// Upload the text data.
        /// </summary>
        /// <param name="file">The cloud file.</param>
        /// <param name="text">The text containing the data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task UploadTextAsync(CloudFile file, string text, CancellationToken cancellationToken)
        {
            await file.UploadTextAsync(text, cancellationToken);
        }

        /// <summary>
        /// Upload the stream data.
        /// </summary>
        /// <param name="file">The cloud file.</param>
        /// <param name="stream">The stream containing the data.</param>
        /// <returns>The async task.</returns>
        public async Task UploadAsync(CloudFile file, Stream stream)
        {
            await file.UploadFromStreamAsync(stream);
        }

        /// <summary>
        /// Upload the stream data.
        /// </summary>
        /// <param name="file">The cloud file.</param>
        /// <param name="stream">The stream containing the data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task UploadAsync(CloudFile file, Stream stream, CancellationToken cancellationToken)
        {
            await file.UploadFromStreamAsync(stream, cancellationToken);
        }

        /// <summary>
        /// Upload the byte array.
        /// </summary>
        /// <param name="file">The cloud file.</param>
        /// <param name="buffer">The data to upload.</param>
        /// <param name="index">The buffer offset index.</param>
        /// <param name="count">The number of bytes in the buffer to read.</param>
        /// <returns>The async task.</returns>
        public async Task UploadAsync(CloudFile file, byte[] buffer, int index, int count)
        {
            await file.UploadFromByteArrayAsync(buffer, index, count);
        }

        /// <summary>
        /// Upload the byte array.
        /// </summary>
        /// <param name="file">The cloud file.</param>
        /// <param name="buffer">The data to upload.</param>
        /// <param name="index">The buffer offset index.</param>
        /// <param name="count">The number of bytes in the buffer to read.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task UploadAsync(CloudFile file, byte[] buffer, int index, int count, CancellationToken cancellationToken)
        {
            await file.UploadFromByteArrayAsync(buffer, index, count, cancellationToken);
        }

        /// <summary>
        /// Upload the local file.
        /// </summary>
        /// <param name="file">The cloud file.</param>
        /// <param name="path">The local path and file name.</param>
        /// <returns>The async task.</returns>
        public async Task UploadAsync(CloudFile file, string path)
        {
            await file.UploadFromFileAsync(path, FileMode.Open);
        }

        /// <summary>
        /// Upload the local file.
        /// </summary>
        /// <param name="file">The cloud file.</param>
        /// <param name="path">The local path and file name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task UploadAsync(CloudFile file, string path, CancellationToken cancellationToken)
        {
            await file.UploadFromFileAsync(path, FileMode.Open, cancellationToken);
        }

        /// <summary>
        /// Get the file within the directory.
        /// </summary>
        /// <param name="directory">The cloud file directory.</param>
        /// <param name="fileName">The name of the file in the directory.</param>
        /// <returns>The file.</returns>
        public CloudFile GetFile(CloudFileDirectory directory, string fileName)
        {
            return directory.GetFileReference(fileName);
        }

        /// <summary>
        /// Delete the file async.
        /// </summary>
        /// <param name="file">The cloud file to delete.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> DeleteFileAsync(CloudFile file)
        {
            return await file.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Delete the file async.
        /// </summary>
        /// <param name="file">The cloud file to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> DeleteFileAsync(CloudFile file, CancellationToken cancellationToken)
        {
            return await file.DeleteIfExistsAsync(cancellationToken);
        }

        /// <summary>
        /// Get the root directory.
        /// </summary>
        /// <returns>The root directory.</returns>
        public CloudFileDirectory GetRootDirectory()
        {
            return _cloudFileShare.GetRootDirectoryReference();
        }

        /// <summary>
        /// Get the reference directory under the root directory.
        /// </summary>
        /// <param name="root">The root directory.</param>
        /// <param name="directoryName">The directory name under the root directory.</param>
        /// <returns>The directory.</returns>
        public CloudFileDirectory GetDirectoryReference(CloudFileDirectory root, string directoryName)
        {
            return root.GetDirectoryReference(directoryName);
        }

        /// <summary>
        /// Create a new directory under the root.
        /// </summary>
        /// <param name="directory">The cloud file directory.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> CreateDirectory(CloudFileDirectory directory)
        {
            return await directory.CreateIfNotExistsAsync();
        }

        /// <summary>
        /// Create a new directory under the root.
        /// </summary>
        /// <param name="directory">The cloud file directory.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> CreateDirectory(CloudFileDirectory directory, CancellationToken cancellationToken)
        {
            return await directory.CreateIfNotExistsAsync(cancellationToken);
        }

        /// <summary>
        /// Delete the directory async.
        /// </summary>
        /// <param name="directory">The cloud file directory.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> DeleteDirectory(CloudFileDirectory directory)
        {
            return await directory.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Delete the directory async.
        /// </summary>
        /// <param name="directory">The cloud file directory.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> DeleteDirectory(CloudFileDirectory directory, CancellationToken cancellationToken)
        {
            return await directory.DeleteIfExistsAsync(cancellationToken);
        }

        /// <summary>
        /// Delete file share async.
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task<bool> DeleteFileShareAsync()
        {
            return await _cloudFileShare.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Delete file share async.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> DeleteFileShareAsync(CancellationToken cancellationToken)
        {
            return await _cloudFileShare.DeleteIfExistsAsync(cancellationToken);
        }

        /// <summary>
        /// Create a file share if it does not exist.
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task<bool> CreateFileShareAsync()
        {
            bool result = await _cloudFileShare.CreateIfNotExistsAsync();
            return result;
        }

        /// <summary>
        /// Create a file share if it does not exist.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> CreateFileShareAsync(CancellationToken cancellationToken)
        {
            bool result = await _cloudFileShare.CreateIfNotExistsAsync(cancellationToken);
            return result;
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="file">The cloud file.</param>
        /// <param name="stream">The stream to write to.</param>
        /// <returns>The async task.</returns>
        public async Task DownloadAsync(CloudFile file, Stream stream)
        {
            await file.DownloadToStreamAsync(stream);
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="file">The cloud file.</param>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task DownloadAsync(CloudFile file, Stream stream, CancellationToken cancellationToken)
        {
            await file.DownloadToStreamAsync(stream, cancellationToken);
        }

        /// <summary>
        /// Download data to the text.
        /// </summary>
        /// <param name="file">The cloud file.</param>
        /// <returns>The async task.</returns>
        public async Task<string> DownloadAsync(CloudFile file)
        {
            return await file.DownloadTextAsync();
        }

        /// <summary>
        /// Download data from text.
        /// </summary>
        /// <param name="file">The cloud file.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<string> DownloadAsync(CloudFile file, CancellationToken cancellationToken)
        {
            return await file.DownloadTextAsync(cancellationToken);
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="file">The cloud file.</param>
        /// <param name="target">The byte array where to write to.</param>
        /// <param name="index">The offset index.</param>
        /// <returns>The async task.</returns>
        public async Task<int> DownloadAsync(CloudFile file, byte[] target, int index)
        {
            return await file.DownloadToByteArrayAsync(target, index);
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="file">The cloud file.</param>
        /// <param name="target">The byte array where to write to.</param>
        /// <param name="index">The offset index.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<int> DownloadAsync(CloudFile file, byte[] target, int index, CancellationToken cancellationToken)
        {
            return await file.DownloadToByteArrayAsync(target, index, cancellationToken);
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="file">The cloud file.</param>
        /// <param name="path">The path and file name to write to.</param>
        /// <returns>The async task.</returns>
        public async Task DownloadAsync(CloudFile file, string path)
        {
            await file.DownloadToFileAsync(path, FileMode.Create);
        }

        /// <summary>
        /// Download data to the stream.
        /// </summary>
        /// <param name="file">The cloud file.</param>
        /// <param name="path">The path and file name to write to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task DownloadAsync(CloudFile file, string path, CancellationToken cancellationToken)
        {
            await file.DownloadToFileAsync(path, FileMode.Create, cancellationToken);
        }
    }
}
