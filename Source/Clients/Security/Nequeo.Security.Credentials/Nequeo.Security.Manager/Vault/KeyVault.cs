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
using System.Security;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Security;
using Nequeo.Cryptography;
using Nequeo.Serialisation;

using Nequeo.Security.Manager.Vault;

namespace Nequeo.Security.Manager
{
	/// <summary>
	/// Key vault is used to store and manage cryptographic keys and secrets.
	/// </summary>
	public sealed class KeyVault : IDisposable
    {
		/// <summary>
		/// Key vault is used to store and manage cryptographic keys and secrets.
		/// </summary>
		public KeyVault() { }

        /// <summary>
        /// Contains the authorisation key.
        /// </summary>
        private byte[] _authorisationKey = System.Text.Encoding.Default.GetBytes("k_5EPc?4+1iM2L&se$8G0gT!qC*7R9w%");
        private string _authorisationCode = null;
        private bool _isAuthorised = false;

        /// <summary>
        /// Gets or sets the code used to authorise encoding and decoding (this is used along with AuthorisationKey).
        /// </summary>
        public string AuthorisationCode
        {
            get { return _authorisationCode; }
        }

        /// <summary>
        /// Gets or sets the key used to authorise encoding and decoding (this is used along with AuthorisationCode).
        /// </summary>
        public byte[] AuthorisationKey
        {
            get { return _authorisationKey; }
            set { _authorisationKey = value; }
        }

        /// <summary>
        /// Backup the vault data.
        /// </summary>
        /// <param name="key">The key used to encrypt the backup data.</param>
        /// <param name="stream">The stream to write the backup data to.</param>
        public void Backup(byte[] key, Stream stream)
		{
            if (_isAuthorised)
            {
                // Write the data to the stream.
                using (Nequeo.Cryptography.AdvancedAES aes = new AdvancedAES())
                {
                    byte[] data = aes.EncryptToMemory(KeyStore._unprotectedData, key);
                    stream.Write(data, 0, data.Length);
                }
            }
            else
                throw new Exception("Authorisation has not been granted.");
        }

        /// <summary>
        /// Restore the backup data to the vault.
        /// </summary>
        /// <param name="key">The key used to decrypt the backup data.</param>
        /// <param name="stream">The stream to read the backup data from.</param>
        /// <param name="loadRestoredData">Load the restored data back to memory, this will replace existing data.</param>
        public void Restore(byte[] key, Stream stream, bool loadRestoredData = false)
		{
            if (_isAuthorised)
            {
                using (Nequeo.Cryptography.AdvancedAES aes = new AdvancedAES())
                {
                    KeyStore._unprotectedData = aes.DecryptStream(stream, key);

                    // If the restored data should be loaded.
                    if (loadRestoredData)
                    {
                        KeyStore.Decrypt();
                        KeyStore.Deserialise();
                    }
                }
            }
            else
                throw new Exception("Authorisation has not been granted.");
        }

        /// <summary>
        /// Create a new vault.
        /// </summary>
        /// <param name="stream">The stream to write the new vault data to.</param>
        /// <returns>True if the vault was created; else false.</returns>
        public bool CreateVault(Stream stream)
        {
            if (!String.IsNullOrEmpty(_authorisationCode))
            {
                // The stream must be empty.
                if (stream.Length > 0)
                    throw new Exception("The stream must be empty.");

                // Create the stream to read from.
                using (MemoryStream memory = new MemoryStream())
                {
                    // Convert the HEX string to bytes.
                    byte[] bytes = Encoding.Default.GetBytes(_authorisationCode);

                    // Protect the data.
                    KeyStore.CreateVault(bytes);
                    KeyStore.Serialise();
                    KeyStore.Encrypt();
                    KeyStore.Protect(memory);
                    memory.Position = 0;

                    // Copy the stream.
                    bool ret = Nequeo.IO.Stream.Operation.CopyStream(memory, stream);

                    // Close the stream.
                    stream.Close();

                    // If not copied.
                    if (!ret)
                    {
                        throw new Exception("The stream can not be copied.");
                    }
                    return true;
                }
            }
            else
                throw new Exception("Authorisation code has not been specified.");
        }

		/// <summary>
		/// Load the vault data.
		/// </summary>
		/// <param name="stream">The stream to read the vault data from.</param>
		/// <param name="closeStream">Close the stream after reading.</param>
		public void Load(Stream stream, bool closeStream = true)
		{
            if (!String.IsNullOrEmpty(_authorisationCode))
            {
                // Create the stream to write to.
                using (MemoryStream memory = new MemoryStream())
                {
                    // Copy the stream.
                    bool ret = Nequeo.IO.Stream.Operation.CopyStream(stream, memory);

                    // If the source stream should be closed.
                    if (closeStream)
                        stream.Close();

                    // If not copied.
                    if (!ret)
                    {
                        throw new Exception("The stream can not be copied.");
                    }

                    // Get the data.
                    KeyStore.Unprotect(memory.ToArray());
                    KeyStore.Decrypt();
                    KeyStore.Deserialise();

                    // Validate the auth code and key.
                    _isAuthorised = IsAuthorised();
                    if(!_isAuthorised)
                    {
                        if (!closeStream)
                            // Close the stream.
                            stream.Close();

                        // Not authorised.
                        throw new Exception("Authorisation has not been granted.");
                    }
                }
            }
            else
                throw new Exception("Authorisation code has not been specified.");
        }

        /// <summary>
        /// Save the vault data.
        /// </summary>
        /// <param name="stream">The stream to write the vault data to.</param>
        /// <param name="closeStream">Close the stream after writing.</param>
        public void Save(Stream stream, bool closeStream = true)
		{
            if (_isAuthorised)
            {
                // Create the stream to read from.
                using (MemoryStream memory = new MemoryStream())
                {
                    // Protect the data.
                    KeyStore.Serialise();
                    KeyStore.Encrypt();
                    KeyStore.Protect(memory);
                    memory.Position = 0;

                    // Copy the stream.
                    bool ret = Nequeo.IO.Stream.Operation.CopyStream(memory, stream);

                    // If the source stream should be closed.
                    if (closeStream)
                        stream.Close();

                    // If not copied.
                    if (!ret)
                    {
                        throw new Exception("The stream can not be copied.");
                    }
                }
            }
            else
                throw new Exception("Authorisation has not been granted.");
        }

        /// <summary>
        /// Add or Update the key to the vault async.
        /// </summary>
        /// <param name="name">The name of the key.</param>
        /// <param name="version">The version of the key.</param>
        /// <param name="keyType">The specified key type stored.</param>
        /// <param name="key">The key.</param>
        /// <returns>True if addedd or updated; else false.</returns>
        public async Task<bool> AddOrUpdateActiveStoreKeyAsync(string name, string version, string keyType, byte[] key)
        {
            if (_isAuthorised)
            {
                if (String.IsNullOrEmpty(name)) throw new NullReferenceException(nameof(name));
                if (String.IsNullOrEmpty(version)) throw new NullReferenceException(nameof(version));
                if (String.IsNullOrEmpty(keyType)) throw new NullReferenceException(nameof(keyType));

                // If vault store is ActiveStore.
                if (key == null) throw new NullReferenceException(nameof(key));

                // Create a new async process.
                using (KeyProcessor process = new KeyProcessor())
                {
                    // Return the processed value.
                    return await process.AddOrUpdateKey(name, version, keyType, key, KeyVaultType.ActiveStore);
                }
            }
            else
                throw new Exception("Authorisation has not been granted.");
        }

        /// <summary>
        /// Add or Update the key to the vault async.
        /// </summary>
        /// <param name="name">The name of the key.</param>
        /// <param name="version">The version of the key.</param>
        /// <param name="keyType">The specified key type stored.</param>
        /// <param name="key">The key.</param>
        /// <param name="cypherType">The cypher type used for security type is <see cref="Nequeo.Cryptography.SecurityType.Certificate"/>.</param>
        /// <returns>True if addedd or updated; else false.</returns>
        public async Task<bool> AddOrUpdateCryptographyKeyAsync(string name, string version, string keyType, byte[] key, Nequeo.Cryptography.CypherType cypherType = CypherType.Aes256)
        {
            if (_isAuthorised)
            {
                if (String.IsNullOrEmpty(name)) throw new NullReferenceException(nameof(name));
                if (String.IsNullOrEmpty(version)) throw new NullReferenceException(nameof(version));
                if (String.IsNullOrEmpty(keyType)) throw new NullReferenceException(nameof(keyType));

                // If vault store is ActiveStore.
                if (key == null) throw new NullReferenceException(nameof(key));

                // Make sure that the SecurityType and CypherType are set.
                if (cypherType == CypherType.None)
                    throw new Exception("The CypherType must be set.");

                // CypherType RSA and PGP can only be used with SecurityType.Certificate.
                if (cypherType == CypherType.RSA || cypherType == CypherType.PGP)
                {
                    throw new Exception("The CypherType.RSA and CypherType.PGP can only be used with SecurityType.Certificate.");
                }

                // Create a new async process.
                using (KeyProcessor process = new KeyProcessor())
                {
                    // Return the processed value.
                    return await process.AddOrUpdateKey(name, version, keyType, key, KeyVaultType.Cryptography, SecurityType.Passphrase, cypherType);
                }
            }
            else
                throw new Exception("Authorisation has not been granted.");
        }

        /// <summary>
        /// Add or Update the key to the vault async.
        /// </summary>
        /// <param name="name">The name of the key.</param>
        /// <param name="version">The version of the key.</param>
        /// <param name="keyType">The specified key type stored.</param>
        /// <param name="publicKey">The public key.</param>
        /// <param name="privateKey">The private key.</param>
        /// <returns>True if addedd or updated; else false.</returns>
        public async Task<bool> AddOrUpdateRSACryptographyKeyAsync(string name, string version, string keyType, byte[] publicKey, byte[] privateKey)
        {
            if (_isAuthorised)
            {
                if (String.IsNullOrEmpty(name)) throw new NullReferenceException(nameof(name));
                if (String.IsNullOrEmpty(version)) throw new NullReferenceException(nameof(version));
                if (String.IsNullOrEmpty(keyType)) throw new NullReferenceException(nameof(keyType));
                if (publicKey == null) throw new NullReferenceException(nameof(publicKey));
                if (privateKey == null) throw new NullReferenceException(nameof(privateKey));

                // Create a new async process.
                using (KeyProcessor process = new KeyProcessor())
                {
                    // Return the processed value.
                    return await process.AddOrUpdateKey(name, version, keyType, null, KeyVaultType.Cryptography, SecurityType.Certificate, CypherType.RSA, publicKey, privateKey);
                }
            }
            else
                throw new Exception("Authorisation has not been granted.");
        }

        /// <summary>
        /// Add or Update the key to the vault async.
        /// </summary>
        /// <param name="name">The name of the key.</param>
        /// <param name="version">The version of the key.</param>
        /// <param name="keyType">The specified key type stored.</param>
        /// <param name="publicKey">The public key.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="keyID">The cryptograph key ID.</param>
        /// <returns>True if addedd or updated; else false.</returns>
        public async Task<bool> AddOrUpdatePGPCryptographyKeyAsync(string name, string version, string keyType, byte[] publicKey, byte[] secretKey, long keyID)
        {
            if (_isAuthorised)
            {
                if (String.IsNullOrEmpty(name)) throw new NullReferenceException(nameof(name));
                if (String.IsNullOrEmpty(version)) throw new NullReferenceException(nameof(version));
                if (String.IsNullOrEmpty(keyType)) throw new NullReferenceException(nameof(keyType));
                if (publicKey == null) throw new NullReferenceException(nameof(publicKey));
                if (secretKey == null) throw new NullReferenceException(nameof(secretKey));
                if (keyID <= 0) throw new Exception("The KeyID must be set to a valid value.");

                // Create a new async process.
                using (KeyProcessor process = new KeyProcessor())
                {
                    // Return the processed value.
                    return await process.AddOrUpdateKey(name, version, keyType, null, KeyVaultType.Cryptography, SecurityType.Certificate, CypherType.PGP, publicKey, secretKey, keyID);
                }
            }
            else
                throw new Exception("Authorisation has not been granted.");
        }

        /// <summary>
        /// Delete the key async.
        /// </summary>
        /// <param name="keyVaultType">The key vault type stored.</param>
        /// <param name="name">The name of the key.</param>
        /// <param name="version">The version of the key.</param>
        /// <param name="keyType">The specified key type stored.</param>
        /// <returns>True if the key was deleted; else false.</returns>
        public async Task<bool> DeleteKeyAsync(KeyVaultType keyVaultType, string name, string version, string keyType)
        {
            if (_isAuthorised)
            {
                if (String.IsNullOrEmpty(name)) throw new NullReferenceException(nameof(name));
                if (String.IsNullOrEmpty(version)) throw new NullReferenceException(nameof(version));
                if (String.IsNullOrEmpty(keyType)) throw new NullReferenceException(nameof(keyType));

                // Create a new async process.
                using (KeyProcessor process = new KeyProcessor())
                {
                    // Return the processed value.
                    return await process.DeleteKey(keyVaultType, name, version, keyType);
                }
            }
            else
                throw new Exception("Authorisation has not been granted.");
        }

        /// <summary>
        /// Delete the key async.
        /// </summary>
        /// <param name="name">The name of the key.</param>
        /// <param name="version">The version of the key.</param>
        /// <param name="keyType">The specified key type stored.</param>
        /// <returns>True if the key was deleted; else false.</returns>
        public async Task<bool> DeleteActiveStoreKeyAsync(string name, string version, string keyType)
        {
            return await DeleteKeyAsync(KeyVaultType.ActiveStore, name, version, keyType);
        }

        /// <summary>
        /// Delete the key async.
        /// </summary>
        /// <param name="name">The name of the key.</param>
        /// <param name="version">The version of the key.</param>
        /// <param name="keyType">The specified key type stored.</param>
        /// <returns>True if the key was deleted; else false.</returns>
        public async Task<bool> DeleteCryptographyKeyAsync(string name, string version, string keyType)
        {
            return await DeleteKeyAsync(KeyVaultType.Cryptography, name, version, keyType);
        }

        /// <summary>
        /// Get the value that has been processed async.
        /// </summary>
        /// <param name="keyVaultType">The key vault type stored.</param>
        /// <param name="name">The name of the key.</param>
        /// <param name="version">The version of the key.</param>
        /// <param name="keyType">The specified key type stored.</param>
        /// <param name="cryptographyType">The cryptography type process if not <see cref="Nequeo.Cryptography.CryptographyType.None"/>.</param>
        /// <param name="data">The data to process if not <see cref="Nequeo.Cryptography.CryptographyType.None"/>.</param>
        /// <returns>The byte array of processed data; else null.</returns>
        public async Task<byte[]> GetValueAsync(KeyVaultType keyVaultType, string name, string version, string keyType,
            Nequeo.Cryptography.CryptographyType cryptographyType = CryptographyType.None, byte[] data = null)
        {
            if (_isAuthorised)
            {
                if (String.IsNullOrEmpty(name)) throw new NullReferenceException(nameof(name));
                if (String.IsNullOrEmpty(version)) throw new NullReferenceException(nameof(version));
                if (String.IsNullOrEmpty(keyType)) throw new NullReferenceException(nameof(keyType));

                if (keyVaultType == KeyVaultType.Cryptography && cryptographyType == CryptographyType.None)
                    throw new Exception("Specify a CryptographyType.");

                if (keyVaultType == KeyVaultType.Cryptography)
                {
                    // If vault store is ActiveStore.
                    if (data == null) throw new NullReferenceException(nameof(data));
                }

                // Create a new async process.
                using (KeyProcessor process = new KeyProcessor())
                {
                    // Return the processed value.
                    return await process.GetValue(keyVaultType, name, version, keyType, cryptographyType, data);
                }
            }
            else
                throw new Exception("Authorisation has not been granted.");
        }

        /// <summary>
        /// Get the value that has been processed async.
        /// </summary>
        /// <param name="name">The name of the key.</param>
        /// <param name="version">The version of the key.</param>
        /// <param name="keyType">The specified key type stored.</param>
        /// <returns>The byte array of processed data; else null.</returns>
        public async Task<byte[]> GetActiveStoreValueAsync(string name, string version, string keyType)
        {
            return await GetValueAsync(KeyVaultType.ActiveStore, name, version, keyType);
        }

        /// <summary>
        /// Get the value that has been processed async.
        /// </summary>
        /// <param name="name">The name of the key.</param>
        /// <param name="version">The version of the key.</param>
        /// <param name="keyType">The specified key type stored.</param>
        /// <param name="data">The data to decrypt.</param>
        /// <returns>>The byte array of processed data; else null.</returns>
        public async Task<byte[]> GetCryptographyDecryptValueAsync(string name, string version, string keyType, byte[] data)
        {
            return await GetValueAsync(KeyVaultType.Cryptography, name, version, keyType, CryptographyType.Decrypt, data);
        }

        /// <summary>
        /// Get the value that has been processed async.
        /// </summary>
        /// <param name="name">The name of the key.</param>
        /// <param name="version">The version of the key.</param>
        /// <param name="keyType">The specified key type stored.</param>
        /// <param name="data">The data to encrypt.</param>
        /// <returns>>The byte array of processed data; else null.</returns>
        public async Task<byte[]> GetCryptographyEncryptValueAsync(string name, string version, string keyType, byte[] data)
        {
            return await GetValueAsync(KeyVaultType.Cryptography, name, version, keyType, CryptographyType.Encrypt, data);
        }

        /// <summary>
        /// Assign the authorisation code used to access the vault.
        /// </summary>
        /// <returns>The HEX string authorisation code.</returns>
        public string AssignAuthorisationCode()
        {
            using (Nequeo.Cryptography.AdvancedAES aes = new Nequeo.Cryptography.AdvancedAES())
            {
                byte[] encryptedData = aes.EncryptToMemory(_authorisationKey, KeyStore._vaultAuthKey);
                string encryptedBase64AuthorisationCode = Nequeo.Conversion.Context.ByteArrayToHexString(encryptedData);

                // Current encrypted Base64 authorisation code.
                _authorisationCode = encryptedBase64AuthorisationCode;
                return _authorisationCode;
            }
        }

        /// <summary>
        /// Is the code authorised.
        /// </summary>
        /// <returns>True if authorised; else false.</returns>
        private bool IsAuthorised()
        {
            if (!String.IsNullOrEmpty(_authorisationCode))
            {
                string authorisationKey = "";

                try
                {
                    // Create a new async process.
                    using (KeyProcessor process = new KeyProcessor())
                    {
                        // Get the internal AuthorisationCode.
                        Task<byte[]> authCodeTask = process.GetValue(KeyVaultType.ActiveStore, "AuthorisationCode", "1", "Internal", CryptographyType.None);
                        byte[] authCode = authCodeTask.Result;

                        // Get the string.
                        string authCodeString = Encoding.Default.GetString(authCode);
                        byte[] bytes = Nequeo.Conversion.Context.HexStringToByteArray(authCodeString);

                        // Decrypt the auth code.
                        using (Nequeo.Cryptography.AdvancedAES aes = new Nequeo.Cryptography.AdvancedAES())
                        {
                            // Decrypt the data
                            byte[] decryptedData = aes.DecryptFromMemory(bytes, KeyStore._vaultAuthKey);
                            authorisationKey = Encoding.Default.GetString(decryptedData).Replace("\0", "");
                        }
                    }
                }
                catch { }

                // Key must be set.
                if (_authorisationKey != null && _authorisationKey.Length > 0)
                {
                    // If the keys match then authorise.
                    if (Encoding.Default.GetString(_authorisationKey).Equals(authorisationKey))
                        return true;
                    else
                        return false;
                }
                else
                    throw new Exception("Authorisation key has not been specified.");
            }
            else
                throw new Exception("Authorisation code has not been specified.");
        }

        /// <summary>
		/// Internal key store.
		/// </summary>
		internal sealed class KeyStore
        {
            private static int _publicKeyLengthIndicator = 8;
            internal static object _syncObject = new object();

            /// <summary>
            /// Contains the vault key.
            /// </summary>
            internal static byte[] _vaultAuthKey = System.Text.Encoding.Default.GetBytes("3m}S_Bf1g$6Q{8LtP%o2+9Ha4i!T*7Xw");
            internal static byte[] _vaultKey = System.Text.Encoding.Default.GetBytes("9Ro_Ct{3$T5wy*L7J2?s8/nMkW=60fD!");

            internal static byte[] _data = null;
            internal static KeyVaultDataModel _keyVaultDataModel = null;

            /// <summary>
            /// Unprotected encypted data.
            /// </summary>
            internal static byte[] _unprotectedData = null;

            /// <summary>
            /// Create a new vault.
            /// </summary>
            /// /// <param name="authorisationCode">The authorisation code.</param>
            internal static void CreateVault(byte[] authorisationCode)
            {
                byte[] authorisationCodeEnc = null;

                _keyVaultDataModel = new KeyVaultDataModel();

                // Create a random key.
                byte[] keyActiveStore = Encoding.Default.GetBytes(new Nequeo.Cryptography.RandomPassword().Generate(32));
                byte[] keyActiveStoreEnc = null;

                // Create a random key.
                byte[] keyCryptography = Encoding.Default.GetBytes(new Nequeo.Cryptography.RandomPassword().Generate(32));
                byte[] keyCryptographyEnc = null;

                // Create the encrypter.
                using (Nequeo.Cryptography.AdvancedAES aes = new AdvancedAES())
                {
                    // Encrypt the key.
                    keyActiveStoreEnc = aes.EncryptToMemory(keyActiveStore, _vaultKey);
                    keyCryptographyEnc = aes.EncryptToMemory(keyCryptography, _vaultKey);
                    authorisationCodeEnc = aes.EncryptToMemory(authorisationCode, _vaultKey);
                }

                // Create the initial active store key.
                _keyVaultDataModel.ActiveStore = new ActiveStoreKeyVaultDataModel[]
                {
                    new ActiveStoreKeyVaultDataModel()
                    {
                        Name = "Key",
                        Version = "1",
                        KeyType = "Internal",
                        Key = keyActiveStoreEnc
                    },
                    new ActiveStoreKeyVaultDataModel()
                    {
                        Name = "AuthorisationCode",
                        Version = "1",
                        KeyType = "Internal",
                        Key = authorisationCodeEnc
                    }
                };

                // Create the initial cryptography key.
                _keyVaultDataModel.Cryptography = new CryptographyKeyVaultDataModel[]
                {
                    new CryptographyKeyVaultDataModel()
                    {
                        Name = "Key",
                        Version = "1",
                        KeyType = "Internal",
                        Key = keyCryptographyEnc,
                        KeyID = 0,
                        CypherType = CypherType.None,
                        SecurityType = SecurityType.Passphrase,
                    }
                };
            }

            /// <summary>
            /// Decrypt the data.
            /// </summary>
            internal static void Decrypt()
            {
                using (Nequeo.Cryptography.AdvancedAES aes = new AdvancedAES())
                {
                    _data = aes.DecryptFromMemory(_unprotectedData, _vaultKey);
                }
            }

            /// <summary>
            /// Encrypt the data.
            /// </summary>
            internal static void Encrypt()
            {
                using (Nequeo.Cryptography.AdvancedAES aes = new AdvancedAES())
                {
                    _unprotectedData = aes.EncryptToMemory(_data, _vaultKey);
                }
            }

            /// <summary>
            /// Delete the key.
            /// </summary>
            /// <param name="keyVaultType">The key vault type stored.</param>
            /// <param name="name">The name of the key.</param>
            /// <param name="version">The version of the key.</param>
            /// <param name="keyType">The specified key type stored.</param>
            /// <returns>True if the key was deleted; else false.</returns>
            internal static bool DeleteKey(KeyVaultType keyVaultType, string name, string version, string keyType)
            {
                if (String.IsNullOrEmpty(name)) throw new NullReferenceException(nameof(name));
                if (String.IsNullOrEmpty(version)) throw new NullReferenceException(nameof(version));
                if (String.IsNullOrEmpty(keyType)) throw new NullReferenceException(nameof(keyType));

                // Lock the code segment.
                lock (_syncObject)
                {
                    bool result = false;
                    bool found = false;

                    ActiveStoreKeyVaultDataModel activeStoreKeyVaultDataModel = null;
                    CryptographyKeyVaultDataModel cryptographyKeyVaultDataModel = null;

                    // Select the vault type.
                    switch (keyVaultType)
                    {
                        case KeyVaultType.ActiveStore:
                            // Find the key.
                            IEnumerable<ActiveStoreKeyVaultDataModel> activeStoreKeys =
                                _keyVaultDataModel.ActiveStore.Where(u => u.Name.Equals(name) && u.Version.Equals(version) && u.KeyType.Equals(keyType));

                            // If keys have been found.
                            if (activeStoreKeys != null && activeStoreKeys.Count() > 0)
                            {
                                // Take the first.
                                activeStoreKeyVaultDataModel = activeStoreKeys.First();
                                found = true;
                            }
                            break;

                        case KeyVaultType.Cryptography:
                            // Find the key.
                            IEnumerable<CryptographyKeyVaultDataModel> cryptographyKeys =
                                _keyVaultDataModel.Cryptography.Where(u => u.Name.Equals(name) && u.Version.Equals(version) && u.KeyType.Equals(keyType));

                            // If keys have been found.
                            if (cryptographyKeys != null && cryptographyKeys.Count() > 0)
                            {
                                // Take the first.
                                cryptographyKeyVaultDataModel = cryptographyKeys.First();
                                found = true;
                            }
                            break;
                    }

                    // Key has been found.
                    if (found)
                    {
                        // Select the vault type.
                        switch (keyVaultType)
                        {
                            case KeyVaultType.ActiveStore:
                                // Add the current list.
                                List<ActiveStoreKeyVaultDataModel> activeStoreKeys = new List<ActiveStoreKeyVaultDataModel>(_keyVaultDataModel.ActiveStore);
                                result = activeStoreKeys.Remove(activeStoreKeyVaultDataModel);

                                // If removed.
                                if (result)
                                {
                                    // Assign the list with the new key.
                                    _keyVaultDataModel.ActiveStore = activeStoreKeys.ToArray();
                                }
                                break;

                            case KeyVaultType.Cryptography:
                                // Add the current list.
                                List<CryptographyKeyVaultDataModel> cryptographyKeys = new List<CryptographyKeyVaultDataModel>(_keyVaultDataModel.Cryptography);
                                result = cryptographyKeys.Remove(cryptographyKeyVaultDataModel);

                                // If removed.
                                if (result)
                                {
                                    // Assign the list with the new key.
                                    _keyVaultDataModel.Cryptography = cryptographyKeys.ToArray();
                                }
                                break;
                        }
                    }

                    // Return the result.
                    return result;
                }
            }

            /// <summary>
            /// Get the vault key.
            /// </summary>
            /// <param name="keyVaultType">The key vault type stored.</param>
            /// <param name="name">The name of the key.</param>
            /// <param name="version">The version of the key.</param>
            /// <param name="keyType">The specified key type stored.</param>
            /// <param name="securityType">The security type to store.</param>
            /// <param name="cypherType">The cypher type used for security type is <see cref="Nequeo.Cryptography.SecurityType.Certificate"/>.</param>
            /// <param name="publicKey">The public key when CypherType.RSA or CypherType.PGP.</param>
            /// <param name="privateKey">The private key when CypherType.RSA or CypherType.PGP.</param>
            /// <param name="keyID">The cryptograph key ID; used specifically for CypherType.PGP.</param>
            /// <returns>The decrypted key; else null.</returns>
            internal static byte[] GetKey(KeyVaultType keyVaultType, string name, string version, string keyType,
                out Nequeo.Cryptography.SecurityType securityType, out Nequeo.Cryptography.CypherType cypherType,
                out byte[] publicKey, out byte[] privateKey, out long keyID)
            {
                if (String.IsNullOrEmpty(name)) throw new NullReferenceException(nameof(name));
                if (String.IsNullOrEmpty(version)) throw new NullReferenceException(nameof(version));
                if (String.IsNullOrEmpty(keyType)) throw new NullReferenceException(nameof(keyType));

                // Lock the code segment.
                lock (_syncObject)
                {
                    byte[] key = null;
                    byte[] keyDec = null;

                    ActiveStoreKeyVaultDataModel activeStoreKeyVaultDataModel = null;
                    CryptographyKeyVaultDataModel cryptographyKeyVaultDataModel = null;

                    // Select the vault type.
                    switch (keyVaultType)
                    {
                        case KeyVaultType.ActiveStore:
                            // Find the key.
                            IEnumerable<ActiveStoreKeyVaultDataModel> activeStoreKeys = 
                                _keyVaultDataModel.ActiveStore.Where(u => u.Name.Equals(name) && u.Version.Equals(version) && u.KeyType.Equals(keyType));

                            // If keys have been found.
                            if(activeStoreKeys != null && activeStoreKeys.Count() > 0)
                            {
                                // Take the first.
                                activeStoreKeyVaultDataModel = activeStoreKeys.First();
                                key = activeStoreKeyVaultDataModel.Key;
                            }
                            break;

                        case KeyVaultType.Cryptography:
                            // Find the key.
                            IEnumerable<CryptographyKeyVaultDataModel> cryptographyKeys =
                                _keyVaultDataModel.Cryptography.Where(u => u.Name.Equals(name) && u.Version.Equals(version) && u.KeyType.Equals(keyType));

                            // If keys have been found.
                            if (cryptographyKeys != null && cryptographyKeys.Count() > 0)
                            {
                                // Take the first.
                                cryptographyKeyVaultDataModel = cryptographyKeys.First();
                                key = cryptographyKeyVaultDataModel.Key;
                            }
                            break;
                    }

                    // If a key has been found.
                    if (key != null)
                    {
                        using (Nequeo.Cryptography.AdvancedAES aes = new AdvancedAES())
                        {
                            // Decrypt the key.
                            keyDec = aes.DecryptFromMemory(key, _vaultKey);

                            // Select the vault type.
                            switch (keyVaultType)
                            {
                                case KeyVaultType.ActiveStore:
                                    publicKey = null;
                                    privateKey = null;
                                    securityType = SecurityType.None;
                                    cypherType = CypherType.None;
                                    keyID = 0;
                                    return keyDec;

                                case KeyVaultType.Cryptography:
                                    // If using SecurityType.Certificate.
                                    if (cryptographyKeyVaultDataModel.SecurityType == SecurityType.Certificate)
                                    {
                                        // If the cyper type is certificate
                                        if ((cryptographyKeyVaultDataModel.CypherType == CypherType.RSA || cryptographyKeyVaultDataModel.CypherType == CypherType.PGP))
                                        {
                                            // Get the private and public key.
                                            byte[] publicKeyLength = keyDec.Take(_publicKeyLengthIndicator).ToArray();
                                            string publicKeySize = Encoding.Default.GetString(publicKeyLength);
                                            int publicKeyInt = Int32.Parse(publicKeySize);

                                            // Assign the public and private keys.
                                            publicKey = keyDec.Skip(_publicKeyLengthIndicator).Take(publicKeyInt).ToArray();
                                            privateKey = keyDec.Skip(_publicKeyLengthIndicator + publicKeyInt).ToArray();
                                            securityType = cryptographyKeyVaultDataModel.SecurityType;
                                            cypherType = cryptographyKeyVaultDataModel.CypherType;
                                            keyID = cryptographyKeyVaultDataModel.KeyID;
                                            return keyDec;
                                        }
                                        else
                                        {
                                            publicKey = null;
                                            privateKey = null;
                                            securityType = cryptographyKeyVaultDataModel.SecurityType;
                                            cypherType = cryptographyKeyVaultDataModel.CypherType;
                                            keyID = cryptographyKeyVaultDataModel.KeyID;
                                            return keyDec;
                                        }
                                    }
                                    else
                                    {
                                        publicKey = null;
                                        privateKey = null;
                                        securityType = SecurityType.None;
                                        cypherType = CypherType.None;
                                        keyID = 0;
                                        return keyDec;
                                    }

                                default:
                                    publicKey = null;
                                    privateKey = null;
                                    securityType = SecurityType.None;
                                    cypherType = CypherType.None;
                                    keyID = 0;
                                    return null;
                            }
                        }
                    }
                    else
                    {
                        publicKey = null;
                        privateKey = null;
                        securityType = SecurityType.None;
                        cypherType = CypherType.None;
                        keyID = 0;
                        return null;
                    }
                }
            }

            /// <summary>
            /// Add or Update the key to the vault.
            /// </summary>
            /// <param name="name">The name of the key.</param>
            /// <param name="version">The version of the key.</param>
            /// <param name="keyType">The specified key type stored.</param>
            /// <param name="key">The key.</param>
            /// <param name="keyVaultType">The key vault type stored.</param>
            /// <param name="securityType">The security type to store.</param>
            /// <param name="cypherType">The cypher type used for security type is <see cref="Nequeo.Cryptography.SecurityType.Certificate"/>.</param>
            /// <param name="publicKey">The public key when CypherType.RSA or CypherType.PGP.</param>
            /// <param name="privateKey">The private key when CypherType.RSA or CypherType.PGP.</param>
            /// <param name="keyID">The cryptograph key ID; used specifically for CypherType.PGP.</param>
            /// <returns>True if addedd or updated; else false.</returns>
            internal static bool AddOrUpdateKey(string name, string version, string keyType, byte[] key, KeyVaultType keyVaultType = KeyVaultType.ActiveStore,
                Nequeo.Cryptography.SecurityType securityType = SecurityType.None, Nequeo.Cryptography.CypherType cypherType = CypherType.None,
                byte[] publicKey = null, byte[] privateKey = null, long keyID = 0)
            {
                if (String.IsNullOrEmpty(name)) throw new NullReferenceException(nameof(name));
                if (String.IsNullOrEmpty(version)) throw new NullReferenceException(nameof(version));
                if (String.IsNullOrEmpty(keyType)) throw new NullReferenceException(nameof(keyType));

                // If the store type is cryptography.
                if (keyVaultType == KeyVaultType.Cryptography)
                {
                    // Make sure that the SecurityType and CypherType are set.
                    if (securityType == SecurityType.None || cypherType == CypherType.None)
                        throw new Exception("The SecurityType and CypherType must be set.");

                    // CypherType RSA and PGP can only be used with SecurityType.Certificate.
                    if (securityType == SecurityType.Passphrase && (cypherType == CypherType.RSA || cypherType == CypherType.PGP))
                    {
                        throw new Exception("The CypherType.RSA and CypherType.PGP can only be used with SecurityType.Certificate.");
                    }

                    // If Passphrase and key is null.
                    if (securityType == SecurityType.Passphrase && (key == null))
                        throw new NullReferenceException(nameof(key));

                    // If SecurityType.Certificate and certificate keys are null.
                    if (securityType == SecurityType.Certificate && (publicKey == null || privateKey == null))
                    {
                        if (publicKey == null) throw new NullReferenceException(nameof(publicKey));
                        if (privateKey == null) throw new NullReferenceException(nameof(privateKey));
                    }

                    // If cypher type is PGP.
                    if (cypherType == CypherType.PGP)
                    {
                        if (keyID <= 0) throw new Exception("The KeyID must be set to a valid value.");
                    }
                }
                else
                {
                    // If vault store is ActiveStore.
                    if (key == null) throw new NullReferenceException(nameof(key));
                }

                // Lock the code segment.
                lock (_syncObject)
                {
                    bool result = false;
                    bool found = false;
                    byte[] keyEnc = null;

                    ActiveStoreKeyVaultDataModel activeStoreKeyVaultDataModel = null;
                    CryptographyKeyVaultDataModel cryptographyKeyVaultDataModel = null;

                    // Select the vault type.
                    switch (keyVaultType)
                    {
                        case KeyVaultType.ActiveStore:
                            // Find the key.
                            IEnumerable<ActiveStoreKeyVaultDataModel> activeStoreKeys =
                                _keyVaultDataModel.ActiveStore.Where(u => u.Name.Equals(name) && u.Version.Equals(version) && u.KeyType.Equals(keyType));

                            // If keys have been found.
                            if (activeStoreKeys != null && activeStoreKeys.Count() > 0)
                            {
                                // Take the first.
                                activeStoreKeyVaultDataModel = activeStoreKeys.First();
                                found = true;
                            }
                            break;

                        case KeyVaultType.Cryptography:
                            // Find the key.
                            IEnumerable<CryptographyKeyVaultDataModel> cryptographyKeys =
                                _keyVaultDataModel.Cryptography.Where(u => u.Name.Equals(name) && u.Version.Equals(version) && u.KeyType.Equals(keyType));

                            // If keys have been found.
                            if (cryptographyKeys != null && cryptographyKeys.Count() > 0)
                            {
                                // Take the first.
                                cryptographyKeyVaultDataModel = cryptographyKeys.First();
                                found = true;
                            }
                            break;
                    }

                    // Create the encrypter.
                    using (Nequeo.Cryptography.AdvancedAES aes = new AdvancedAES())
                    {
                        // Select the vault type.
                        switch (keyVaultType)
                        {
                            case KeyVaultType.ActiveStore:
                                // Encrypt the key.
                                keyEnc = aes.EncryptToMemory(key, _vaultKey);

                                // If encrypted key.
                                if (keyEnc != null)
                                {
                                    // Add the current list.
                                    List<ActiveStoreKeyVaultDataModel> activeStoreKeys = new List<ActiveStoreKeyVaultDataModel>(_keyVaultDataModel.ActiveStore);

                                    // If found.
                                    if (found)
                                    {
                                        // Assign the new key.
                                        activeStoreKeyVaultDataModel.Key = keyEnc;
                                    }
                                    else
                                    {
                                        // Add the item.
                                        activeStoreKeys.Add(new ActiveStoreKeyVaultDataModel()
                                        {
                                            Name = name,
                                            Version = version,
                                            KeyType = keyType,
                                            Key = keyEnc
                                        });
                                    }

                                    // Assign the list with the new key.
                                    _keyVaultDataModel.ActiveStore = activeStoreKeys.ToArray();
                                    result = true;
                                }
                                break;

                            case KeyVaultType.Cryptography:
                                // If using SecurityType.Passphrase.
                                if (securityType == SecurityType.Passphrase)
                                {
                                    // Encrypt the key.
                                    keyEnc = aes.EncryptToMemory(key, _vaultKey);
                                }
                                else
                                {
                                    // If the cyper type is certificate
                                    if ((cypherType == CypherType.RSA || cypherType == CypherType.PGP))
                                    {
                                        // Create the public and private key store.
                                        string publicKeySize = publicKey.Length.ToString().PadLeft(_publicKeyLengthIndicator, '0');
                                        byte[] publicKeyLength = Encoding.Default.GetBytes(publicKeySize);
                                        List<byte> cryptoKey = new List<byte>();

                                        // Add the keys.
                                        cryptoKey.AddRange(publicKeyLength);
                                        cryptoKey.AddRange(publicKey);
                                        cryptoKey.AddRange(privateKey);

                                        // Encrypt the key.
                                        keyEnc = aes.EncryptToMemory(cryptoKey.ToArray(), _vaultKey);
                                    }
                                }

                                // If encrypted key.
                                if (keyEnc != null)
                                {
                                    // Add the current list.
                                    List<CryptographyKeyVaultDataModel> cryptographyKeys = new List<CryptographyKeyVaultDataModel>(_keyVaultDataModel.Cryptography);

                                    // If found.
                                    if (found)
                                    {
                                        // Assign the new key.
                                        cryptographyKeyVaultDataModel.Key = keyEnc;
                                        cryptographyKeyVaultDataModel.KeyID = keyID;
                                        cryptographyKeyVaultDataModel.SecurityType = securityType;
                                        cryptographyKeyVaultDataModel.CypherType = cypherType;
                                    }
                                    else
                                    {
                                        // Add the current list.
                                        cryptographyKeys.Add(new CryptographyKeyVaultDataModel()
                                        {
                                            Name = name,
                                            Version = version,
                                            KeyType = keyType,
                                            Key = keyEnc,
                                            SecurityType = securityType,
                                            CypherType = cypherType,
                                            KeyID = keyID,
                                        });
                                    }

                                    // Assign the list with the new key.
                                    _keyVaultDataModel.Cryptography = cryptographyKeys.ToArray();
                                    result = true;
                                }
                                break;
                        }

                        // Return the result.
                        return result;
                    }
                }
            }

            /// <summary>
            /// Serialise the data.
            /// </summary>
            internal static void Serialise()
            {
                Nequeo.Serialisation.GenericSerialisation<KeyVaultDataModel> ser = new GenericSerialisation<KeyVaultDataModel>();
                _data = ser.Serialise(_keyVaultDataModel);
            }

            /// <summary>
            /// Deserialise the data.
            /// </summary>
            internal static void Deserialise()
            {
                Nequeo.Serialisation.GenericSerialisation<KeyVaultDataModel> ser = new GenericSerialisation<KeyVaultDataModel>();
                _keyVaultDataModel = ser.Deserialise(_data);
            }

            /// <summary>
            /// Get the protected vault key.
            /// </summary>
            /// <returns>The protected key.</returns>
            internal static byte[] GetProtectedVaultKey()
            {
                return new Nequeo.Security.Protection().SameProcessProtect(_vaultKey);
            }

            /// <summary>
            /// Get the un-protected vault key.
            /// </summary>
            /// <param name="data">The protected data.</param>
            /// <returns>The un-protected key.</returns>
            internal static byte[] GetUnprotectedVaultKey(byte[] data)
            {
                return new Nequeo.Security.Protection().SameProcessUnprotect(data);
            }

            /// <summary>
            /// Unprotect the protect data.
            /// </summary>
            /// <param name="data">The protected data.</param>
            internal static void Unprotect(byte[] data)
            {
                // Get the memory stream.
                using (MemoryStream memory = new Nequeo.Security.Protection().MachineUnprotect(data))
                {
                    _unprotectedData = memory.ToArray();
                }
            }

            /// <summary>
            /// Protect the unprotected data.
            /// </summary>
            /// <param name="stream">The stream to write the protected data to.</param>
            internal static void Protect(Stream stream)
            {
                // Get the memory stream.
                using (MemoryStream memory = new Nequeo.Security.Protection().MachineProtect(_unprotectedData))
                {
                    // Write the protected data to the stream.
                    stream.Write(memory.ToArray(), 0, (int)memory.Length);
                }
            }
        }

        /// <summary>
		/// Internal key processor.
		/// </summary>
		internal sealed class KeyProcessor : IDisposable
        {
            private object _syncObject = new object();

            /// <summary>
            /// Delete the key.
            /// </summary>
            /// <param name="keyVaultType">The key vault type stored.</param>
            /// <param name="name">The name of the key.</param>
            /// <param name="version">The version of the key.</param>
            /// <param name="keyType">The specified key type stored.</param>
            /// <returns>True if the key was deleted; else false.</returns>
            public Task<bool> DeleteKey(KeyVaultType keyVaultType, string name, string version, string keyType)
            {
                // Start a new task.
                return Task.Factory.StartNew<bool>(() =>
                {
                    bool result = false;

                    try
                    {
                        // Delete the key.
                        result = KeyStore.DeleteKey(keyVaultType, name, version, keyType);
                    }
                    catch { }

                    // Return the result.
                    return result;
                });
            }

            /// <summary>
            /// Add or Update the key to the vault.
            /// </summary>
            /// <param name="name">The name of the key.</param>
            /// <param name="version">The version of the key.</param>
            /// <param name="keyType">The specified key type stored.</param>
            /// <param name="key">The key.</param>
            /// <param name="keyVaultType">The key vault type stored.</param>
            /// <param name="securityType">The security type to store.</param>
            /// <param name="cypherType">The cypher type used for security type is <see cref="Nequeo.Cryptography.SecurityType.Certificate"/>.</param>
            /// <param name="publicKey">The public key when CypherType.RSA or CypherType.PGP.</param>
            /// <param name="privateKey">The private key when CypherType.RSA or CypherType.PGP.</param>
            /// <param name="keyID">The cryptograph key ID; used specifically for CypherType.PGP.</param>
            /// <returns>True if addedd or updated; else false.</returns>
            public Task<bool> AddOrUpdateKey(string name, string version, string keyType, byte[] key, KeyVaultType keyVaultType = KeyVaultType.ActiveStore,
                Nequeo.Cryptography.SecurityType securityType = SecurityType.None, Nequeo.Cryptography.CypherType cypherType = CypherType.None,
                byte[] publicKey = null, byte[] privateKey = null, long keyID = 0)
            {
                // Start a new task.
                return Task.Factory.StartNew<bool>(() =>
                {
                    bool result = false;

                    try
                    {
                        // Delete the key.
                        result = KeyStore.AddOrUpdateKey(name, version, keyType, key, keyVaultType, securityType, cypherType, publicKey, privateKey, keyID);
                    }
                    catch { }

                    // Return the result.
                    return result;
                });
            }

            /// <summary>
            /// Get the value that has been processed async.
            /// </summary>
            /// <param name="keyVaultType">The key vault type stored.</param>
            /// <param name="name">The name of the key.</param>
            /// <param name="version">The version of the key.</param>
            /// <param name="keyType">The specified key type stored.</param>
            /// <param name="cryptographyType">The cryptography type process if not <see cref="Nequeo.Cryptography.CryptographyType.None"/>.</param>
            /// <param name="data">The data to process if not <see cref="Nequeo.Cryptography.CryptographyType.None"/>.</param>
            /// <returns>The byte array of processed data; else null.</returns>
            public Task<byte[]> GetValue(KeyVaultType keyVaultType, string name, string version, string keyType,
                Nequeo.Cryptography.CryptographyType cryptographyType = CryptographyType.None, byte[] data = null)
            {
                // Start a new task.
                return Task.Factory.StartNew<byte[]>(() =>
                {
                    SecurityType securityType = SecurityType.None;
                    CypherType cypherType = CypherType.None;

                    byte[] key = null;
                    byte[] keyResult = null;
                    byte[] publicKey = null;
                    byte[] privateKey = null;
                    long keyID = 0;

                    try
                    {
                        // Get the key.
                        key = KeyStore.GetKey(keyVaultType, name, version, keyType,
                            out securityType, out cypherType, out publicKey, out privateKey, out keyID);

                        // Select the vault type.
                        switch (keyVaultType)
                        {
                            case KeyVaultType.ActiveStore:
                                keyResult = key;
                                break;

                            case KeyVaultType.Cryptography:
                                // If security type is certificate.
                                if (securityType == SecurityType.Certificate)
                                {
                                    // If the cypher type is certificate.
                                    if ((cypherType == CypherType.RSA || cypherType == CypherType.PGP))
                                    {
                                        // Select the cypher type.
                                        switch (cypherType)
                                        {
                                            case CypherType.RSA:
                                                // Create the RSA certificate cryptography provider.
                                                Nequeo.Cryptography.Openssl.PublicPrivateKey rsa = new Cryptography.Openssl.PublicPrivateKey();

                                                // Select the cryptography type.
                                                switch (cryptographyType)
                                                {
                                                    case CryptographyType.Decrypt:
                                                        // Load the private key.
                                                        using (MemoryStream memoryDec = new MemoryStream(privateKey))
                                                        using (StreamReader streamReaderDec = new StreamReader(memoryDec))
                                                        {
                                                            // Get the private key decrypter.
                                                            RSACryptoServiceProvider decryptProvider = rsa.PrivateKeyDecryptionProvider(streamReaderDec);
                                                            keyResult = rsa.Decrypt(data, decryptProvider);
                                                        }
                                                        break;
                                                    case CryptographyType.Encrypt:
                                                        // Load the public key.
                                                        using (MemoryStream memoryEnc = new MemoryStream(publicKey))
                                                        using (StreamReader streamReaderEnc = new StreamReader(memoryEnc))
                                                        {
                                                            // Get the public key encrypter.
                                                            RSACryptoServiceProvider encyptProvider = rsa.PublicKeyEncryptionProvider(streamReaderEnc);
                                                            keyResult = rsa.Encrypt(data, encyptProvider);
                                                        }
                                                        break;
                                                    default:
                                                        keyResult = null;
                                                        break;
                                                }
                                                break;

                                            case CypherType.PGP:
                                                // Create the PGP certificate cryptography provider.
                                                Nequeo.Cryptography.Openpgp.PublicPrivateKey pgp = new Nequeo.Cryptography.Openpgp.PublicPrivateKey();

                                                // Load the keys
                                                using (MemoryStream publicKeyStream = new MemoryStream(publicKey))
                                                using (MemoryStream privateKeyStream = new MemoryStream(privateKey))
                                                {
                                                    // Get the provider.
                                                    RSACryptoServiceProvider provider = pgp.PublicKeySecretKeyProvider(publicKeyStream, privateKeyStream, keyID, "");

                                                    // Select the cryptography type.
                                                    switch (cryptographyType)
                                                    {
                                                        case CryptographyType.Decrypt:
                                                            keyResult = pgp.Decrypt(data, provider);
                                                            break;
                                                        case CryptographyType.Encrypt:
                                                            keyResult = pgp.Encrypt(data, provider);
                                                            break;
                                                        default:
                                                            keyResult = null;
                                                            break;
                                                    }
                                                }
                                                break;

                                            default:
                                                keyResult = null;
                                                break;
                                        }
                                    }
                                    else
                                        keyResult = null;
                                }
                                else
                                {
                                    // Select the cypher type.
                                    switch (cypherType)
                                    {
                                        case CypherType.Aes256:
                                            // Select the cryptography type.
                                            switch (cryptographyType)
                                            {
                                                case CryptographyType.Decrypt:
                                                    keyResult = new Nequeo.Cryptography.AdvancedAES().DecryptFromMemory(data, key);
                                                    break;
                                                case CryptographyType.Encrypt:
                                                    keyResult = new Nequeo.Cryptography.AdvancedAES().EncryptToMemory(data, key);
                                                    break;
                                                default:
                                                    keyResult = null;
                                                    break;
                                            }
                                            break;

                                        case CypherType.Blowfish:
                                            // Select the cryptography type.
                                            switch (cryptographyType)
                                            {
                                                case CryptographyType.Decrypt:
                                                    keyResult = new Nequeo.Cryptography.Blowfish().Decrypt(data, Encoding.Default.GetString(key));
                                                    break;
                                                case CryptographyType.Encrypt:
                                                    keyResult = new Nequeo.Cryptography.Blowfish().Encrypt(data, Encoding.Default.GetString(key));
                                                    break;
                                                default:
                                                    keyResult = null;
                                                    break;
                                            }
                                            break;

                                        case CypherType.RC6:
                                            // Select the cryptography type.
                                            switch (cryptographyType)
                                            {
                                                case CryptographyType.Decrypt:
                                                    keyResult = new Nequeo.Cryptography.RC6().Decrypt(data, Encoding.Default.GetString(key));
                                                    break;
                                                case CryptographyType.Encrypt:
                                                    keyResult = new Nequeo.Cryptography.RC6().Encrypt(data, Encoding.Default.GetString(key));
                                                    break;
                                                default:
                                                    keyResult = null;
                                                    break;
                                            }
                                            break;

                                        case CypherType.Rijndael:
                                            // Select the cryptography type.
                                            switch (cryptographyType)
                                            {
                                                case CryptographyType.Decrypt:
                                                    keyResult = new Nequeo.Cryptography.Rijndael().Decrypt(data, Encoding.Default.GetString(key));
                                                    break;
                                                case CryptographyType.Encrypt:
                                                    keyResult = new Nequeo.Cryptography.Rijndael().Encrypt(data, Encoding.Default.GetString(key));
                                                    break;
                                                default:
                                                    keyResult = null;
                                                    break;
                                            }
                                            break;

                                        case CypherType.Serpent:
                                            // Select the cryptography type.
                                            switch (cryptographyType)
                                            {
                                                case CryptographyType.Decrypt:
                                                    keyResult = new Nequeo.Cryptography.Serpent().Decrypt(data, Encoding.Default.GetString(key));
                                                    break;
                                                case CryptographyType.Encrypt:
                                                    keyResult = new Nequeo.Cryptography.Serpent().Encrypt(data, Encoding.Default.GetString(key));
                                                    break;
                                                default:
                                                    keyResult = null;
                                                    break;
                                            }
                                            break;

                                        case CypherType.TripleDes:
                                            // Select the cryptography type.
                                            switch (cryptographyType)
                                            {
                                                case CryptographyType.Decrypt:
                                                    keyResult = new Nequeo.Cryptography.AdvancedTripleDES().DecryptFromMemory(data, key);
                                                    break;
                                                case CryptographyType.Encrypt:
                                                    keyResult = new Nequeo.Cryptography.AdvancedTripleDES().EncryptToMemory(data, key);
                                                    break;
                                                default:
                                                    keyResult = null;
                                                    break;
                                            }
                                            break;

                                        case CypherType.Twofish:
                                            // Select the cryptography type.
                                            switch (cryptographyType)
                                            {
                                                case CryptographyType.Decrypt:
                                                    keyResult = new Nequeo.Cryptography.Twofish().Decrypt(data, Encoding.Default.GetString(key));
                                                    break;
                                                case CryptographyType.Encrypt:
                                                    keyResult = new Nequeo.Cryptography.Twofish().Encrypt(data, Encoding.Default.GetString(key));
                                                    break;
                                                default:
                                                    keyResult = null;
                                                    break;
                                            }
                                            break;

                                        default:
                                            keyResult = null;
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    catch { }

                    // Return the key.
                    return keyResult;
                });
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
            private void Dispose(bool disposing)
            {
                // Check to see if Dispose has already been called.
                if (!_disposed)
                {
                    if (disposing)
                    {
                        // Dispose managed state (managed objects).
                    }

                    // Free unmanaged resources (unmanaged objects) and override a finalizer below.
                    _syncObject = null;
                }
            }

            /// <summary>
            /// /// Use C# destructor syntax for finalization code.
            /// This destructor will run only if the Dispose method
            /// does not get called.
            /// It gives your base class the opportunity to finalize.
            /// Do not provide destructors in types derived from this class.
            /// </summary>
            ~KeyProcessor()
            {
                // Do not re-create Dispose clean-up code here.
                // Calling Dispose(false) is optimal in terms of
                // readability and maintainability.
                Dispose(false);
            }
            #endregion
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
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects).
                }

                // Free unmanaged resources (unmanaged objects) and override a finalizer below.
                _authorisationKey = null;

                KeyStore._syncObject = null;
                KeyStore._vaultKey = null;
                KeyStore._unprotectedData = null;
                KeyStore._data = null;
                KeyStore._keyVaultDataModel = null;
            }
        }

        /// <summary>
        /// /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~KeyVault()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
