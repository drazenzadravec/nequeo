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

namespace Nequeo.Net.OAuth2.Storage.Basic
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;

    using Nequeo.Net.Core.Messaging.Bindings;
    using Nequeo.Net.Core.Messaging;

    /// <summary>
    /// Internal cryto key store; this is not implemeted not necessary.
    /// </summary>
    internal class CryptoKeyStoreTranslate : ICryptographyKeyStore
    {
        /// <summary>
        /// Internal cryto key store; this is not implemeted not necessary.
        /// </summary>
        /// <param name="tokenStore">The token store</param>
        /// <param name="consumerStore">The consumer store</param>
        /// <param name="nonceStore">The nonce store.</param>
        public CryptoKeyStoreTranslate(ITokenStore tokenStore, IConsumerStore consumerStore, INonceStore nonceStore)
        {
            _tokenStore = tokenStore;
            _consumerStore = consumerStore;
            _nonceStore = nonceStore;
        }

        private ITokenStore _tokenStore = null;
        private IConsumerStore _consumerStore = null;
        private INonceStore _nonceStore = null;
        private string _clientIndetifier = null;
        private DateTime _expiryDateTime;
        private string _nonce = null;
        private string _codeKey = null;
        private bool _getCodeKey = false;

        /// <summary>
        /// Gets or sets the client identifier
        /// </summary>
        public string ClientIndetifier
        {
            get { return _clientIndetifier; }
            set { _clientIndetifier = value; }
        }

        /// <summary>
        /// Gets or sets the expiry date time.
        /// </summary>
        public DateTime ExpiryDateTime
        {
            get { return _expiryDateTime; }
            set { _expiryDateTime = value; }
        }

        /// <summary>
        /// Gets or sets the nonce.
        /// </summary>
        public string Nonce
        {
            get { return _nonce; }
            set { _nonce = value; }
        }

        /// <summary>
        /// Gets or sets the code key.
        /// </summary>
        public string CodeKey
        {
            get { return _codeKey; }
            set { _codeKey = value; }
        }

        /// <summary>
        /// Gets or sets the get code key indicator.
        /// </summary>
        public bool GetCodeKey
        {
            get { return _getCodeKey; }
            set { _getCodeKey = value; }
        }

        /// <summary>
        /// Gets the key in a given bucket and handle.
        /// </summary>
        /// <param name="bucket">The bucket name.  Case sensitive.</param>
        /// <param name="handle">The key handle.  Case sensitive.</param>
        /// <returns>The cryptographic key, or <c>null</c> if no matching key was found.</returns>
        public CryptoKey GetKey(string bucket, string handle)
        {
            X509Certificate2 certificate = null;

            if(GetCodeKey)
                certificate = _consumerStore.GetConsumerCertificate(handle);
            else
                certificate = _consumerStore.GetConsumerCertificate(_clientIndetifier);


            Byte[] publicKey = certificate.GetPublicKey();
            Byte[] getSigningKey = publicKey.Take(32).ToArray();
            return new CryptoKey(getSigningKey, ExpiryDateTime);
        }

        /// <summary>
        /// Gets a sequence of existing keys within a given bucket.
        /// </summary>
        /// <param name="bucket">The bucket name.  Case sensitive.</param>
        /// <returns>A sequence of handles and keys, ordered by descending <see cref="CryptoKey.ExpiresUtc"/>.</returns>
        public IEnumerable<KeyValuePair<string, CryptoKey>> GetKeys(string bucket)
        {
            X509Certificate2 certificate = _consumerStore.GetConsumerCertificate(_clientIndetifier);
            Byte[] publicKey = certificate.GetPublicKey();
            Byte[] getSigningKey = publicKey.Take(32).ToArray();
            return new List<KeyValuePair<string, CryptoKey>>()
            {
                new KeyValuePair<string, CryptoKey>(_clientIndetifier, new CryptoKey(getSigningKey, ExpiryDateTime))
            };
        }

        /// <summary>
        /// Removes the key.
        /// </summary>
        /// <param name="bucket">The bucket name.  Case sensitive.</param>
        /// <param name="handle">The key handle.  Case sensitive.</param>
        public void RemoveKey(string bucket, string handle)
        {
        }

        /// <summary>
        /// Stores a cryptographic key.
        /// </summary>
        /// <param name="bucket">The name of the bucket to store the key in.  Case sensitive.</param>
        /// <param name="handle">The handle to the key, unique within the bucket.  Case sensitive.</param>
        /// <param name="key">The key to store.</param>
        /// <exception cref="CryptoKeyCollisionException">Thrown in the event of a conflict with an existing key in the same bucket and with the same handle.</exception>
        public void StoreKey(string bucket, string handle, CryptoKey key)
        {
        }
    }
}
