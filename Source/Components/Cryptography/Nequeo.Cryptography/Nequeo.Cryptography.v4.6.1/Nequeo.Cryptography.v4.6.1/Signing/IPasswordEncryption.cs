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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Threading.Tasks;

namespace Nequeo.Cryptography
{
    /// <summary>
    /// Password encryption provider
    /// </summary>
    public interface IPasswordEncryption
    {
        /// <summary>
        /// Gets or sets the code used to authorise encoding and decoding (this is used along with AuthorisationKey).
        /// </summary>
        string AuthorisationCode { get; set; }

        /// <summary>
        /// Gets or sets the key used to authorise encoding and decoding (this is used along with AuthorisationCode).
        /// </summary>
        string AuthorisationKey { get; set; }

        /// <summary>
        /// Gets or sets the password format.
        /// </summary>
        Nequeo.Cryptography.PasswordFormat PasswordFormat { get; set; }

        /// <summary>
        /// Gets or sets the hashcode type.
        /// </summary>
        Nequeo.Cryptography.HashcodeType HashcodeType { get; set; }

        /// <summary>
        /// Get the authorisation code.
        /// </summary>
        /// <returns>The authorisation code used for access.</returns>
        string GetAuthorisationCode();

        /// <summary>
        /// Encode the password.
        /// </summary>
        /// <param name="password">The password to encode.</param>
        /// <param name="passwordFormat">The password format type to encode with.</param>
        /// <param name="hashcodeType">The the hash code type (used when format is Hashed).</param>
        /// <returns>The encode password.</returns>
        string Encode(string password, Nequeo.Cryptography.PasswordFormat passwordFormat, 
            Nequeo.Cryptography.HashcodeType hashcodeType = Cryptography.HashcodeType.SHA512);

        /// <summary>
        /// Encode the password.
        /// </summary>
        /// <param name="password">The password to encode.</param>
        /// <param name="passwordFormat">The password format type to encode with.</param>
        /// <param name="hashcodeType">The the hash code type (used when format is Hashed).</param>
        /// <returns>The encode password.</returns>
        string Encode(SecureString password, Nequeo.Cryptography.PasswordFormat passwordFormat,
            Nequeo.Cryptography.HashcodeType hashcodeType = Cryptography.HashcodeType.SHA512);

        /// <summary>
        /// Decode the password.
        /// </summary>
        /// <param name="password">The encoded password to decode.</param>
        /// <param name="passwordFormat">The password format type to decode with.</param>
        /// <param name="originalPassword">The original password (used when format is Hashed).</param>
        /// <param name="hashcodeType">The the hash code type (used when format is Hashed).</param>
        /// <returns>The decoded password.</returns>
        string Decode(string password, Nequeo.Cryptography.PasswordFormat passwordFormat, string originalPassword = "",
            Nequeo.Cryptography.HashcodeType hashcodeType = Cryptography.HashcodeType.SHA512);

        /// <summary>
        /// Decode the password.
        /// </summary>
        /// <param name="password">The encoded password to decode.</param>
        /// <param name="passwordFormat">The password format type to decode with.</param>
        /// <param name="originalPassword">The original password (used when format is Hashed).</param>
        /// <param name="hashcodeType">The the hash code type (used when format is Hashed).</param>
        /// <returns>The decoded password.</returns>
        string Decode(SecureString password, Nequeo.Cryptography.PasswordFormat passwordFormat, SecureString originalPassword,
            Nequeo.Cryptography.HashcodeType hashcodeType = Cryptography.HashcodeType.SHA512);

        /// <summary>
        /// Decrypt the password.
        /// </summary>
        /// <param name="password">The encrypted password.</param>
        /// <returns>The decrypted password.</returns>
        string Decrypt(string password);

        /// <summary>
        /// Decrypt the password.
        /// </summary>
        /// <param name="password">The encrypted password.</param>
        /// <param name="key">The key used to decrypt the password.</param>
        /// <returns>The decrypted password.</returns>
        string Decrypt(string password, string key);

        /// <summary>
        /// Decrypt the password.
        /// </summary>
        /// <param name="password">The encrypted password.</param>
        /// <returns>The decrypted password.</returns>
        string Decrypt(SecureString password);

        /// <summary>
        /// Decrypt the password.
        /// </summary>
        /// <param name="password">The encrypted password.</param>
        /// <param name="key">The key used to decrypt the password.</param>
        /// <returns>The decrypted password.</returns>
        string Decrypt(SecureString password, SecureString key);

        /// <summary>
        /// Encrypt the password.
        /// </summary>
        /// <param name="password">The password to encrypt.</param>
        /// <returns>The encrypted password.</returns>
        string Encrypt(string password);

        /// <summary>
        /// Encrypt the password.
        /// </summary>
        /// <param name="password">The password to encrypt.</param>
        /// <param name="key">The key used to encrypt the password.</param>
        /// <returns>The encrypted password.</returns>
        string Encrypt(string password, string key);

        /// <summary>
        /// Encrypt the password.
        /// </summary>
        /// <param name="password">The password to encrypt.</param>
        /// <returns>The encrypted password.</returns>
        string Encrypt(SecureString password);

        /// <summary>
        /// Encrypt the password.
        /// </summary>
        /// <param name="password">The password to encrypt.</param>
        /// <param name="key">The key used to encrypt the password.</param>
        /// <returns>The encrypted password.</returns>
        string Encrypt(SecureString password, SecureString key);

        /// <summary>
        /// Store the password in a secure string.
        /// </summary>
        /// <param name="password">The password to store.</param>
        /// <returns>The secure password.</returns>
        SecureString StorePassword(string password);

        /// <summary>
        /// Store the original password in a secure string.
        /// </summary>
        /// <param name="password">The password to store.</param>
        /// <returns>The secure password.</returns>
        SecureString StoreOriginalPassword(string password);

        /// <summary>
        /// Store the key in a secure string.
        /// </summary>
        /// <param name="key">The key used to encrypt the password.</param>
        /// <returns>The secure key.</returns>
        SecureString StoreKey(string key);

        /// <summary>
        /// Load the passwords from the stream.
        /// </summary>
        /// <param name="store">The stream where the password store is read from.</param>
        void LoadPasswordStore(Stream store);

        /// <summary>
        /// Save the password store data to the stream.
        /// </summary>
        /// <param name="store">The stream where the password store is written to.</param>
        void SavePasswordStore(Stream store);

        /// <summary>
        /// Get the password for the specified name from the store.
        /// </summary>
        /// <param name="storeName">The name of the password loaded from the store.</param>
        /// <returns>The password; else null.</returns>
        string GetPassword(string storeName);

        /// <summary>
        /// Get the password for the specified name from the store.
        /// </summary>
        /// <param name="storeName">The name of the password loaded from the store.</param>
        /// <returns>The password; else null.</returns>
        SecureString GetPasswordSecure(string storeName);

        /// <summary>
        /// Set the password if it exists else create new entry.
        /// </summary>
        /// <param name="storeName">The name of the password loaded from the store.</param>
        /// <param name="storePassword">The password to store for the name.</param>
        void SetPassword(string storeName, string storePassword);

        /// <summary>
        /// Set the password if it exists else create new entry.
        /// </summary>
        /// <param name="storeName">The name of the password loaded from the store.</param>
        /// <param name="storePassword">The password to store for the name.</param>
        void SetPasswordSecure(string storeName, SecureString storePassword);

        /// <summary>
        /// Delete the password if it exists.
        /// </summary>
        /// <param name="storeName">The name of the password loaded from the store.</param>
        void DeletePassword(string storeName);
    }
}
