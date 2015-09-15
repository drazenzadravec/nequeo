/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
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
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Net.Security;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ComponentModel.Composition;

using Nequeo.ComponentModel.Composition;

namespace Nequeo.Security
{
    /// <summary>
    /// Password encoder.
    /// </summary>
    [Export(typeof(Nequeo.Cryptography.IPasswordEncryption))]
    [ContentMetadata(Name = "NequeoPasswordEncoding", Index = 0, Description = "Nequeo password default encoder.")]
    public sealed class PasswordEncoding : Nequeo.Cryptography.IPasswordEncryption
    {
        /// <summary>
        /// Password encoder.
        /// </summary>
        public PasswordEncoding() { }

        private string _authorisationCode = null;
        private string _authorisationKey = "Y/t01cC?-8eWS!6m4o%QE9i&$T3bG7q{";

        /// <summary>
        /// Internal key container.
        /// </summary>
        internal class KeyContainer
        {
            /// <summary>
            /// Salt length.
            /// </summary>
            public static int _saltLength = 32;

            /// <summary>
            /// Password cryptography key.
            /// </summary>
            public static string _passwordKey = "b/0ZRt$13Hf%L&8ze7D!6-dSP9_ayG4}";

            /// <summary>
            /// Authorisation cryptography key.
            /// </summary>
            public static string _authorisationCryptoKey = "i?7QJ0w+2*Gd$Cz5q=6M4sZ}_3mAb&H1";

            /// <summary>
            /// Number of derived iterations.
            /// </summary>
            public static int _numberOfIterations = 5000;

            /// <summary>
            /// The collection of passwords.
            /// </summary>
            public static List<Nequeo.Model.NameValue> _passwords = null;
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
                    // Convert the HEX string to bytes.
                    byte[] bytes = Nequeo.Conversion.Context.HexStringToByteArray(_authorisationCode);

                    // Decrypt the data
                    byte[] decryptedData = new Nequeo.Cryptography.AdvancedAES().DecryptFromMemory(bytes, KeyContainer._authorisationCryptoKey);
                    authorisationKey = Encoding.Default.GetString(decryptedData).Replace("\0", "");
                }
                catch { }

                // If the keys match then authorise.
                if (_authorisationKey.Equals(authorisationKey))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Get the encrypted authorisation code.
        /// </summary>
        /// <returns>The encrypted base64 authorisation code used for access.</returns>
        public string GetAuthorisationCode()
        {
            byte[] data = Encoding.Default.GetBytes(_authorisationKey);
            byte[] encryptedData = new Nequeo.Cryptography.AdvancedAES().EncryptToMemory(data, KeyContainer._authorisationCryptoKey);
            string encryptedBase64AuthorisationCode = Nequeo.Conversion.Context.ByteArrayToHexString(encryptedData);

            // Current encrypted Base64 authorisation code.
            return encryptedBase64AuthorisationCode; // 46638A1D3B7F9502B8460824FB75841E1DF38537EBAACA5163DB7529D38063AE
        }

        /// <summary>
        /// Gets or sets the code used to authorise encoding and decoding (this is used along with AuthorisationKey).
        /// </summary>
        public string AuthorisationCode
        {
            get { return _authorisationCode; }
            set { _authorisationCode = value; }
        }

        /// <summary>
        /// Gets or sets the key used to authorise encoding and decoding (this is used along with AuthorisationCode).
        /// </summary>
        public string AuthorisationKey
        {
            get { return _authorisationKey; }
            set { _authorisationKey = value; }
        }

        /// <summary>
        /// Gets or sets the password format.
        /// </summary>
        public Nequeo.Cryptography.PasswordFormat PasswordFormat { get; set; }

        /// <summary>
        /// Gets or sets the hashcode type.
        /// </summary>
        public Nequeo.Cryptography.HashcodeType HashcodeType { get; set; }

        /// <summary>
        /// Decode the password.
        /// </summary>
        /// <param name="password">The password to decode.</param>
        /// <param name="passwordFormat">The password format type to decode with.</param>
        /// <param name="originalPassword">The original password (used when format is Hashed).</param>
        /// <param name="hashcodeType">The the hash code type (used when format is Hashed).</param>
        /// <returns>The decoded password (if format is Hashed and the hash has been verified to be 
        /// the same then the original password is returned; else the password is returned).</returns>
        public string Decode(string password, Cryptography.PasswordFormat passwordFormat, string originalPassword = "",
            Nequeo.Cryptography.HashcodeType hashcodeType = Cryptography.HashcodeType.SHA512)
        {
            // If authorised.
            if (IsAuthorised())
            {
                // Select the format.
                switch (passwordFormat)
                {
                    case Cryptography.PasswordFormat.Encrypted:
                        // Convert the HEX string to bytes.
                        byte[] bytes = Nequeo.Conversion.Context.HexStringToByteArray(password);

                        // Decrypt the data
                        byte[] decryptedData = new Nequeo.Cryptography.AdvancedAES().DecryptFromMemory(bytes, KeyContainer._passwordKey);
                        return Encoding.Default.GetString(decryptedData).Replace("\0", "");

                    case Cryptography.PasswordFormat.Hashed:
                        // Get the salt.
                        string saltBase = password.Substring(0, KeyContainer._saltLength * 2);
                        byte[] saltBaseBytes = Nequeo.Conversion.Context.HexStringToByteArray(saltBase);
                        string salt = Encoding.Default.GetString(saltBaseBytes);

                        // Password - based Key Derivation Functions.
                        Rfc2898DeriveBytes rfcDerive = new Rfc2898DeriveBytes(originalPassword, saltBaseBytes, KeyContainer._numberOfIterations);
                        string derivedPassword = Encoding.Default.GetString(rfcDerive.GetBytes(KeyContainer._saltLength * 2));

                        // Return the salt for the hash.
                        string hash = Nequeo.Cryptography.Hashcode.GetHashcode(derivedPassword + _authorisationKey + salt, hashcodeType);

                        // Get the hex salt.
                        byte[] saltBase64 = Encoding.Default.GetBytes(salt);
                        string hashed = Nequeo.Conversion.Context.ByteArrayToHexString(saltBase64) + hash;

                        // If the hash is the password.
                        if (hashed.Equals(password))
                            return originalPassword;
                        else
                            return password;

                    case Cryptography.PasswordFormat.Clear:
                    default:
                        return password;
                }
            }
            else
            {
                if (passwordFormat == Cryptography.PasswordFormat.Clear)
                    return password;
                else
                    throw new Exception("Authorisation Failed.");
            }
        }

        /// <summary>
        /// Decode the password.
        /// </summary>
        /// <param name="password">The encoded password to decode.</param>
        /// <param name="passwordFormat">The password format type to decode with.</param>
        /// <param name="originalPassword">The original password (used when format is Hashed); can be null.</param>
        /// <param name="hashcodeType">The the hash code type (used when format is Hashed).</param>
        /// <returns>The decoded password.</returns>
        public string Decode(SecureString password, Nequeo.Cryptography.PasswordFormat passwordFormat, SecureString originalPassword,
            Nequeo.Cryptography.HashcodeType hashcodeType = Cryptography.HashcodeType.SHA512)
        {
            return Decode(
                new Nequeo.Security.SecureText().GetText(password), 
                passwordFormat,
                (originalPassword != null ? new Nequeo.Security.SecureText().GetText(originalPassword) : ""), 
                hashcodeType);
        }

        /// <summary>
        /// Encode the password.
        /// </summary>
        /// <param name="password">The password to encode.</param>
        /// <param name="passwordFormat">The password format type to encode with.</param>
        /// <param name="hashcodeType">The the hash code type (used when format is Hashed).</param>
        /// <returns>The encode password.</returns>
        public string Encode(string password, Cryptography.PasswordFormat passwordFormat,
            Nequeo.Cryptography.HashcodeType hashcodeType = Cryptography.HashcodeType.SHA512)
        {
            // If authorised.
            if (IsAuthorised())
            {
                // Select the format.
                switch (passwordFormat)
                {
                    case Cryptography.PasswordFormat.Encrypted:
                        // Encrypt the data.
                        byte[] data = Encoding.Default.GetBytes(password);
                        byte[] encryptedData = new Nequeo.Cryptography.AdvancedAES().EncryptToMemory(data, KeyContainer._passwordKey);
                        return Nequeo.Conversion.Context.ByteArrayToHexString(encryptedData);

                    case Cryptography.PasswordFormat.Hashed:
                        // Encode the password to a hash.
                        string salt = Nequeo.Cryptography.Hashcode.GenerateSalt(KeyContainer._saltLength, KeyContainer._saltLength);
                        byte[] saltBase = Encoding.Default.GetBytes(salt);

                        // Password - based Key Derivation Functions.
                        Rfc2898DeriveBytes rfcDerive = new Rfc2898DeriveBytes(password, saltBase, KeyContainer._numberOfIterations);
                        string derivedPassword = Encoding.Default.GetString(rfcDerive.GetBytes(KeyContainer._saltLength * 2));

                        // Get the hex salt.
                        string hash = Nequeo.Cryptography.Hashcode.GetHashcode(derivedPassword + _authorisationKey + salt, hashcodeType);
                        return Nequeo.Conversion.Context.ByteArrayToHexString(saltBase) + hash;

                    case Cryptography.PasswordFormat.Clear:
                    default:
                        return password;
                }
            }
            else
            {
                if (passwordFormat == Cryptography.PasswordFormat.Clear)
                    return password;
                else
                    throw new Exception("Authorisation Failed.");
            }
        }

        /// <summary>
        /// Encode the password.
        /// </summary>
        /// <param name="password">The password to encode.</param>
        /// <param name="passwordFormat">The password format type to encode with.</param>
        /// <param name="hashcodeType">The the hash code type (used when format is Hashed).</param>
        /// <returns>The encode password.</returns>
        public string Encode(SecureString password, Nequeo.Cryptography.PasswordFormat passwordFormat,
            Nequeo.Cryptography.HashcodeType hashcodeType = Cryptography.HashcodeType.SHA512)
        {
            return Encode(new Nequeo.Security.SecureText().GetText(password), passwordFormat, hashcodeType);
        }

        /// <summary>
        /// Decrypt the password.
        /// </summary>
        /// <param name="password">The encrypted password.</param>
        /// <returns>The decrypted password.</returns>
        public string Decrypt(string password)
        {
            return Decrypt(password, KeyContainer._passwordKey);
        }

        /// <summary>
        /// Decrypt the password.
        /// </summary>
        /// <param name="password">The encrypted password.</param>
        /// <param name="key">The key used to decrypt the password.</param>
        /// <returns>The decrypted password.</returns>
        public string Decrypt(string password, string key)
        {
            // If authorised.
            if (IsAuthorised())
            {
                // Decrypt the data.
                byte[] bytes = Nequeo.Conversion.Context.HexStringToByteArray(password);
                byte[] decryptedData = new Nequeo.Cryptography.AdvancedAES().DecryptFromMemory(bytes, key);
                return Encoding.Default.GetString(decryptedData).Replace("\0", "");
            }
            else
                throw new Exception("Authorisation Failed.");
        }

        /// <summary>
        /// Decrypt the password.
        /// </summary>
        /// <param name="password">The encrypted password.</param>
        /// <returns>The decrypted password.</returns>
        public string Decrypt(SecureString password)
        {
            return Decrypt(new Nequeo.Security.SecureText().GetText(password));
        }

        /// <summary>
        /// Decrypt the password.
        /// </summary>
        /// <param name="password">The encrypted password.</param>
        /// <param name="key">The key used to decrypt the password.</param>
        /// <returns>The decrypted password.</returns>
        public string Decrypt(SecureString password, SecureString key)
        {
            return Decrypt(new Nequeo.Security.SecureText().GetText(password), new Nequeo.Security.SecureText().GetText(key));
        }

        /// <summary>
        /// Encrypt the password.
        /// </summary>
        /// <param name="password">The password to encrypt.</param>
        /// <returns>The encrypted password.</returns>
        public string Encrypt(string password)
        {
            return Encrypt(password, KeyContainer._passwordKey);
        }

        /// <summary>
        /// Encrypt the password.
        /// </summary>
        /// <param name="password">The password to encrypt.</param>
        /// <param name="key">The key used to encrypt the password.</param>
        /// <returns>The encrypted password.</returns>
        public string Encrypt(string password, string key)
        {
            // If authorised.
            if (IsAuthorised())
            {
                // Encrypt the data.
                byte[] data = Encoding.Default.GetBytes(password);
                byte[] encryptedData = new Nequeo.Cryptography.AdvancedAES().EncryptToMemory(data, key);
                return Nequeo.Conversion.Context.ByteArrayToHexString(encryptedData);
            }
            else
                throw new Exception("Authorisation Failed.");
        }

        /// <summary>
        /// Encrypt the password.
        /// </summary>
        /// <param name="password">The password to encrypt.</param>
        /// <returns>The encrypted password.</returns>
        public string Encrypt(SecureString password)
        {
            return Encrypt(new Nequeo.Security.SecureText().GetText(password));
        }

        /// <summary>
        /// Encrypt the password.
        /// </summary>
        /// <param name="password">The password to encrypt.</param>
        /// <param name="key">The key used to encrypt the password.</param>
        /// <returns>The encrypted password.</returns>
        public string Encrypt(SecureString password, SecureString key)
        {
            return Encrypt(new Nequeo.Security.SecureText().GetText(password), new Nequeo.Security.SecureText().GetText(key));
        }

        /// <summary>
        /// Store the password in a secure string.
        /// </summary>
        /// <param name="password">The password to store.</param>
        /// <returns>The secure password.</returns>
        public SecureString StorePassword(string password)
        {
            return new Nequeo.Security.SecureText().GetSecureText(password);
        }

        /// <summary>
        /// Store the original password in a secure string.
        /// </summary>
        /// <param name="password">The password to store.</param>
        /// <returns>The secure password.</returns>
        public SecureString StoreOriginalPassword(string password)
        {
            return new Nequeo.Security.SecureText().GetSecureText(password);
        }

        /// <summary>
        /// Store the key in a secure string.
        /// </summary>
        /// <param name="key">The key used to encrypt the password.</param>
        /// <returns>The secure key.</returns>
        public SecureString StoreKey(string key)
        {
            return new Nequeo.Security.SecureText().GetSecureText(key);
        }

        /// <summary>
        /// Load the passwords from the stream.
        /// </summary>
        /// <param name="store">The stream where the password store is read from.</param>
        public void LoadPasswordStore(Stream store)
        {
            // If the store contains data.
            if (store != null && store.Length > 0)
            {
                // Go to the start of the stream.
                store.Position = 0;

                // Decrypt the password store.
                byte[] passwordStore = new Nequeo.Cryptography.AdvancedAES().DecryptStream(store, KeyContainer._authorisationCryptoKey);

                // Load the password data.
                string passwordData = Encoding.Unicode.GetString(passwordStore);
                string[] names = passwordData.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                // Load into the name value collection.
                KeyContainer._passwords = new List<Model.NameValue>(names.Length);
                for (int i = 0; i < names.Length; i++)
                {
                    // Get the name and value.
                    string[] nameValue = names[i].Split(new char[] { ':' }, StringSplitOptions.None);
                    string name = nameValue[0];
                    string value = nameValue[1].Replace("\0", "");

                    // Add the name and password.
                    KeyContainer._passwords.Add(new Model.NameValue() { Name = name, Value = value });
                }
            }
            else
            {
                // Create an empty password store.
                KeyContainer._passwords = new List<Model.NameValue>();
            }
        }

        /// <summary>
        /// Save the password store data to the stream.
        /// </summary>
        /// <param name="store">The stream where the password store is written to.</param>
        public void SavePasswordStore(Stream store)
        {
            // If authorised.
            if (IsAuthorised())
            {
                // If the stream is null.
                if (store == null) throw new ArgumentNullException("store");

                // Make sure data exists.
                if (KeyContainer._passwords == null)
                    // Create an empty password store.
                    KeyContainer._passwords = new List<Model.NameValue>();

                MemoryStream memoryStream = null;
                try
                {
                    // Create a new MemoryStream using the passed 
                    // array of encrypted data.
                    using (memoryStream = new MemoryStream())
                    {
                        // For each password.
                        for (int i = 0; i < KeyContainer._passwords.Count; i++)
                        {
                            // Assing the values.
                            string name = KeyContainer._passwords[i].Name;
                            string value = KeyContainer._passwords[i].Value;
                            string end = (i == KeyContainer._passwords.Count - 1 ? "" : "\r\n");

                            // Create the buffer.
                            byte[] buffer = Encoding.Unicode.GetBytes(name + ":" + value + end);
                            memoryStream.Write(buffer, 0, buffer.Length);
                        }

                        // Reset the position of the memory stream.
                        memoryStream.Position = 0;

                        // Encrypt the password store and write
                        // the data to the user stream.
                        byte[] passwordStore = new Nequeo.Cryptography.AdvancedAES().EncryptStream(memoryStream, KeyContainer._authorisationCryptoKey);
                        store.Write(passwordStore, 0, passwordStore.Length);
                    }
                }
                catch { }
                finally
                {
                    // Release all resources.
                    if (memoryStream != null)
                        memoryStream.Close();
                }
            }
            else
                throw new Exception("Authorisation Failed.");
        }

        /// <summary>
        /// Get the password for the specified name from the store.
        /// </summary>
        /// <param name="storeName">The name of the password loaded from the store.</param>
        /// <returns>The password; else null.</returns>
        public string GetPassword(string storeName)
        {
            // Make sure data exists.
            if (KeyContainer._passwords != null)
            {
                // Find all.
                IEnumerable<Nequeo.Model.NameValue> pass = KeyContainer._passwords.Where(u => u.Name.Equals(storeName));
                if (pass != null && pass.Count() > 0)
                {
                    // Decrypt the stored password.
                    byte[] data = Convert.FromBase64String(pass.First().Value);
                    byte[] password = new Nequeo.Cryptography.AdvancedAES().DecryptFromMemory(data, KeyContainer._passwordKey);
                    return Encoding.Unicode.GetString(password);
                }
                else
                    return null;
            }
            else
                throw new Exception("Load the password store data first.");
        }

        /// <summary>
        /// Get the password for the specified name from the store.
        /// </summary>
        /// <param name="storeName">The name of the password loaded from the store.</param>
        /// <returns>The password; else null.</returns>
        public SecureString GetPasswordSecure(string storeName)
        {
            string password = GetPassword(storeName);
            if (!String.IsNullOrEmpty(password))
                return new Nequeo.Security.SecureText().GetSecureText(password);
            else
                return null;
        }

        /// <summary>
        /// Set the password if it exists else create new entry.
        /// </summary>
        /// <param name="storeName">The name of the password loaded from the store.</param>
        /// <param name="storePassword">The password to store for the name.</param>
        public void SetPassword(string storeName, string storePassword)
        {
            // Make sure data exists.
            if (KeyContainer._passwords != null)
            {
                // Find all.
                IEnumerable<Nequeo.Model.NameValue> pass = KeyContainer._passwords.Where(u => u.Name.Equals(storeName));
                if (pass != null && pass.Count() > 0)
                {
                    // Update the password.
                    byte[] data = Encoding.Unicode.GetBytes(storePassword);
                    byte[] password = new Nequeo.Cryptography.AdvancedAES().EncryptToMemory(data, KeyContainer._passwordKey);
                    pass.First().Value = Convert.ToBase64String(password);
                }
                else
                {
                    // Create a new password.
                    byte[] data = Encoding.Unicode.GetBytes(storePassword);
                    byte[] password = new Nequeo.Cryptography.AdvancedAES().EncryptToMemory(data, KeyContainer._passwordKey);
                    string value = Convert.ToBase64String(password);

                    // Add the password.
                    KeyContainer._passwords.Add(new Model.NameValue() { Name = storeName, Value = value });
                }
            }
            else
                throw new Exception("Load the password store data first.");
        }

        /// <summary>
        /// Set the password if it exists else create new entry.
        /// </summary>
        /// <param name="storeName">The name of the password loaded from the store.</param>
        /// <param name="storePassword">The password to store for the name.</param>
        public void SetPasswordSecure(string storeName, SecureString storePassword)
        {
            string password = new Nequeo.Security.SecureText().GetText(storePassword);
            SetPassword(storeName, password);
        }

        /// <summary>
        /// Delete the password if it exists.
        /// </summary>
        /// <param name="storeName">The name of the password loaded from the store.</param>
        public void DeletePassword(string storeName)
        {
            // If authorised.
            if (IsAuthorised())
            {
                // Make sure data exists.
                if (KeyContainer._passwords != null)
                {
                    // Find all.
                    IEnumerable<Nequeo.Model.NameValue> pass = KeyContainer._passwords.Where(u => u.Name.Equals(storeName));
                    if (pass != null && pass.Count() > 0)
                    {
                        // Remove the item.
                        KeyContainer._passwords.Remove(pass.First());
                    }
                }
                else
                    throw new Exception("Load the password store data first.");
            }
            else
                throw new Exception("Authorisation Failed.");
        }
    }
}
