/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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

namespace Nequeo.Net.Core.Messaging.Bindings
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;

    using Nequeo.Net.Core.Messaging.Bindings;
    using Nequeo.Net.Core.Messaging;

    /// <summary>
    /// Code contract for the <see cref="ICryptoKeyStore"/> interface.
    /// </summary>
    [ContractClassFor(typeof(ICryptoKeyStore))]
    public abstract class ICryptoKeyStoreContract : ICryptoKeyStore
    {
        /// <summary>
        /// Gets the key in a given bucket and handle.
        /// </summary>
        /// <param name="bucket">The bucket name.  Case sensitive.</param>
        /// <param name="handle">The key handle.  Case sensitive.</param>
        /// <returns>
        /// The cryptographic key, or <c>null</c> if no matching key was found.
        /// </returns>
        CryptoKey ICryptoKeyStore.GetKey(string bucket, string handle)
        {
            Requires.NotNullOrEmpty(bucket, "bucket");
            Requires.NotNullOrEmpty(handle, "handle");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a sequence of existing keys within a given bucket.
        /// </summary>
        /// <param name="bucket">The bucket name.  Case sensitive.</param>
        /// <returns>
        /// A sequence of handles and keys, ordered by descending <see cref="CryptoKey.ExpiresUtc"/>.
        /// </returns>
        IEnumerable<KeyValuePair<string, CryptoKey>> ICryptoKeyStore.GetKeys(string bucket)
        {
            Requires.NotNullOrEmpty(bucket, "bucket");
            Contract.Ensures(Contract.Result<IEnumerable<KeyValuePair<string, CryptoKey>>>() != null);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stores a cryptographic key.
        /// </summary>
        /// <param name="bucket">The name of the bucket to store the key in.  Case sensitive.</param>
        /// <param name="handle">The handle to the key, unique within the bucket.  Case sensitive.</param>
        /// <param name="key">The key to store.</param>
        /// <exception cref="CryptoKeyCollisionException">Thrown in the event of a conflict with an existing key in the same bucket and with the same handle.</exception>
        void ICryptoKeyStore.StoreKey(string bucket, string handle, CryptoKey key)
        {
            Requires.NotNullOrEmpty(bucket, "bucket");
            Requires.NotNullOrEmpty(handle, "handle");
            Requires.NotNull(key, "key");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the key.
        /// </summary>
        /// <param name="bucket">The bucket name.  Case sensitive.</param>
        /// <param name="handle">The key handle.  Case sensitive.</param>
        void ICryptoKeyStore.RemoveKey(string bucket, string handle)
        {
            Requires.NotNullOrEmpty(bucket, "bucket");
            Requires.NotNullOrEmpty(handle, "handle");
            throw new NotImplementedException();
        }
    }
}
