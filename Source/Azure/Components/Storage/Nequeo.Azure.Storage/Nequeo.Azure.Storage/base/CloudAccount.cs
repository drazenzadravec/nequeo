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
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Nequeo.Azure.Storage
{
    /// <summary>
    /// Cloud account provider.
    /// </summary>
    public class CloudAccount
    {
        /// <summary>
        /// Cloud account provider.
        /// </summary>
        /// <param name="storageConnectionString">The storage connection string.</param>
        public CloudAccount(string storageConnectionString)
        {
            _storageAccount = CreateStorageAccountFromConnectionString(storageConnectionString);
        }

        /// <summary>
        /// Cloud account provider.
        /// </summary>
        /// <param name="storageCredentials">A Microsoft.WindowsAzure.Storage.Auth.StorageCredentials object.</param>
        /// <param name="useHttps">True to use HTTPS to connect to storage service endpoints; otherwise, false.</param>
        public CloudAccount(StorageCredentials storageCredentials, bool useHttps)
        {
            _storageAccount = new CloudStorageAccount(storageCredentials, useHttps);
        }

        /// <summary>
        /// Cloud account provider.
        /// </summary>
        /// <param name="storageCredentials">A Microsoft.WindowsAzure.Storage.Auth.StorageCredentials object.</param>
        /// <param name="endpointSuffix">The DNS endpoint suffix for all storage services, e.g. "core.windows.net".</param>
        /// <param name="useHttps">True to use HTTPS to connect to storage service endpoints; otherwise, false.</param>
        public CloudAccount(StorageCredentials storageCredentials, string endpointSuffix, bool useHttps)
        {
            _storageAccount = new CloudStorageAccount(storageCredentials, endpointSuffix, useHttps);
        }

        /// <summary>
        /// Cloud account provider.
        /// </summary>
        /// <param name="storageCredentials">A Microsoft.WindowsAzure.Storage.Auth.StorageCredentials object.</param>
        /// <param name="blobEndpoint">A System.Uri specifying the primary Blob service endpoint.</param>
        /// <param name="queueEndpoint">A System.Uri specifying the primary Queue service endpoint.</param>
        /// <param name="tableEndpoint">A System.Uri specifying the primary Table service endpoint.</param>
        /// <param name="fileEndpoint">A System.Uri specifying the primary File service endpoint.</param>
        public CloudAccount(StorageCredentials storageCredentials, Uri blobEndpoint, Uri queueEndpoint, Uri tableEndpoint, Uri fileEndpoint)
        {
            _storageAccount = new CloudStorageAccount(storageCredentials, blobEndpoint, queueEndpoint, tableEndpoint, fileEndpoint);
        }

        /// <summary>
        /// Cloud account provider.
        /// </summary>
        /// <param name="storageCredentials">A Microsoft.WindowsAzure.Storage.Auth.StorageCredentials object.</param>
        /// <param name="blobStorageUri">A Microsoft.WindowsAzure.Storage.StorageUri specifying the Blob service endpoint or endpoints.</param>
        /// <param name="queueStorageUri">A Microsoft.WindowsAzure.Storage.StorageUri specifying the Queue service endpoint or endpoints.</param>
        /// <param name="tableStorageUri">A Microsoft.WindowsAzure.Storage.StorageUri specifying the Table service endpoint or endpoints.</param>
        /// <param name="fileStorageUri">A Microsoft.WindowsAzure.Storage.StorageUri specifying the File service endpoint or endpoints.</param>
        public CloudAccount(StorageCredentials storageCredentials, StorageUri blobStorageUri, StorageUri queueStorageUri, StorageUri tableStorageUri, StorageUri fileStorageUri)
        {
            _storageAccount = new CloudStorageAccount(storageCredentials, blobStorageUri, queueStorageUri, tableStorageUri, fileStorageUri);
        }

        public CloudStorageAccount _storageAccount = null;

        /// <summary>
        /// Gets the cloud storgae account.
        /// </summary>
        public CloudStorageAccount CloudStorageAccount
        {
            get { return _storageAccount; }
        }

        /// <summary>
        /// Validate the connection string information in app.config and throws an exception if it looks like 
        /// the user hasn't updated this to valid values. 
        /// </summary>
        /// <param name="storageConnectionString">Connection string for the storage service or the emulator</param>
        /// <returns>CloudStorageAccount object</returns>
        private CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                // Parse the connection string.
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                throw;
            }
            catch (ArgumentException)
            {
                throw;
            }

            // Return the cloud storage account.
            return storageAccount;
        }
    }
}
