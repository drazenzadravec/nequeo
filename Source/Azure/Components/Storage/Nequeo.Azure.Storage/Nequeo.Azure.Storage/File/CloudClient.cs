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
using Microsoft.WindowsAzure.Storage.File;

namespace Nequeo.Azure.Storage.File
{
    /// <summary>
    /// Cloud client provider.
    /// </summary>
    public class CloudClient
    {
        /// <summary>
        /// Cloud client provider
        /// </summary>
        /// <param name="storageAccount">The cloud storage account.</param>
        public CloudClient(CloudAccount storageAccount)
        {
            _cloudFileClient = storageAccount.CloudStorageAccount.CreateCloudFileClient();
        }

        private CloudFileClient _cloudFileClient = null;

        /// <summary>
        /// Gets the cloud file client.
        /// </summary>
        public CloudFileClient CloudFileClient
        {
            get { return _cloudFileClient; }
        }

        /// <summary>
        /// Get the cloud file share.
        /// </summary>
        /// <param name="shareName">The name of the file share.</param>
        /// <returns>The cloud file share.</returns>
        public CloudFileShare GetCloudFileShare(string shareName)
        {
            return _cloudFileClient.GetShareReference(shareName);
        }

        /// <summary>
        /// Get all the cloud file shares.
        /// </summary>
        /// <returns>The list of cloud file shares.</returns>
        public IEnumerable<CloudFileShare> GetFileShares()
        {
            return _cloudFileClient.ListShares();
        }
    }
}
