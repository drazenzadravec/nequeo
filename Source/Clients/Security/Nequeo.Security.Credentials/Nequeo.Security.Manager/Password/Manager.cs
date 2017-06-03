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

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

namespace Nequeo.Security
{
	/// <summary>
	/// Password manager provider.
	/// </summary>
	public sealed class PasswordManager
	{
		/// <summary>
		/// Password manager provider.
		/// </summary>
		public PasswordManager()
		{
		}

		/// <summary>
		/// Password manager provider.
		/// </summary>
		/// <param name="securityPassword">A password security encryption implementation.</param>
		public PasswordManager(Nequeo.Cryptography.IPasswordEncryption securityPassword)
		{
			_securityPassword = securityPassword;
		}

		private Nequeo.Cryptography.IPasswordEncryption _securityPassword = null;

		/// <summary>
		/// Gets or sets the security password implementation.
		/// </summary>
		public Nequeo.Cryptography.IPasswordEncryption SecurityPassword
		{
			get { return _securityPassword; }
			set { _securityPassword = value; }
		}

		/// <summary>
		/// Loads from the configuration file the security password encoder.
		/// </summary>
		/// <param name="section">The config section group and section name.</param>
		public void LoadSecurityPasswordEncoder(string section = "NequeoSecurityGroup/NequeoSecurityPassword")
		{
			_securityPassword = new Nequeo.Security.Configuration.Reader().GetEncoder(section);
		}

		/// <summary>
		/// Load the passwords from the stream.
		/// </summary>
		/// <param name="store">The stream where the password store is read from.</param>
		/// <param name="storeName">The authorisation key store name. Use this to automatically set the authorisation key and code.</param>
		public void LoadPasswordStore(Stream store, string storeName = "AuthorisationKey")
		{
			_securityPassword.LoadPasswordStore(store);
			GetAuthorisationKey(storeName);
		}

		/// <summary>
		/// Save the password store data to the stream.
		/// </summary>
		/// <param name="store">The stream where the password store is written to.</param>
		public void SavePasswordStore(Stream store)
		{
			_securityPassword.SavePasswordStore(store);
		}

		/// <summary>
		/// Get the authorisation code from the authorisation key.
		/// </summary>
		/// <returns>The authorisation code.</returns>
		public string GetAuthorisationCode()
		{
			return _securityPassword.GetAuthorisationCode();
		}

		/// <summary>
		/// Get the authorisation key.
		/// </summary>
		/// <param name="storeName">The authorisation key store name.</param>
		/// <returns>The authorisation key; else null.</returns>
		public string GetAuthorisationKey(string storeName = "AuthorisationKey")
		{
			string pass = _securityPassword.GetPassword(storeName);
			if (!String.IsNullOrEmpty(pass))
			{
				_securityPassword.AuthorisationKey = pass;
				_securityPassword.AuthorisationCode = _securityPassword.GetAuthorisationCode();
				return pass;
			}
			else
				return pass;
		}

		/// <summary>
		/// Set the authorisation key.
		/// </summary>
		/// <param name="key">The authorisation key.</param>
		/// <param name="storeName">The authorisation key store name.</param>
		public void SetAuthorisationKey(string key, string storeName = "AuthorisationKey")
		{
			_securityPassword.SetPassword(storeName, key);
			_securityPassword.AuthorisationKey = key;
			_securityPassword.AuthorisationCode = _securityPassword.GetAuthorisationCode();
		}

		/// <summary>
		/// Get the password for the specified name from the store.
		/// </summary>
		/// <param name="storeName">The name of the password loaded from the store.</param>
		/// <returns>The password; else null.</returns>
		public string GetPassword(string storeName)
		{
			return _securityPassword.GetPassword(storeName);
		}

		/// <summary>
		/// Set the password if it exists else create new entry.
		/// </summary>
		/// <param name="storeName">The name of the password loaded from the store.</param>
		/// <param name="storePassword">The password to store for the name.</param>
		public void SetPassword(string storeName, string storePassword)
		{
			_securityPassword.SetPassword(storeName, storePassword);
		}

        /// <summary>
		/// Set the password if it exists else create new entry. Creates a random password.
		/// </summary>
		/// <param name="storeName">The name of the password loaded from the store.</param>
		public void SetPassword(string storeName)
        {
            byte[] randomPasswordData = Encoding.Default.GetBytes(new Nequeo.Cryptography.RandomPassword().Generate(32));
            string storePassword = Encoding.Default.GetString(randomPasswordData);
            _securityPassword.SetPassword(storeName, storePassword);
        }

        /// <summary>
        /// Delete the password if it exists.
        /// </summary>
        /// <param name="storeName">The name of the password loaded from the store.</param>
        public void DeletePassword(string storeName)
		{
			_securityPassword.DeletePassword(storeName);
		}

		/// <summary>
		/// Encrypt the password.
		/// </summary>
		/// <param name="password">The password to encrypt.</param>
		/// <param name="key">The key used to encrypt the password.</param>
		/// <returns>The encrypted password.</returns>
		public string Encrypt(string password, string key)
		{
			return _securityPassword.Encrypt(password, key);
		}

		/// <summary>
		/// Decrypt the password.
		/// </summary>
		/// <param name="password">The encrypted password.</param>
		/// <param name="key">The key used to decrypt the password.</param>
		/// <returns>The decrypted password.</returns>
		public string Decrypt(string password, string key)
		{
			return _securityPassword.Decrypt(password, key);
		}

		/// <summary>
		/// Encrypts the data in a specified byte array and returns a byte array that contains the encrypted data.
		/// This method uses the current user account.
		/// </summary>
		/// <param name="data">The data to protect.</param>
		/// <returns>The stream containing the protected data.</returns>
		public MemoryStream UserProtect(byte[] data)
		{
			return new Nequeo.Security.Protection().UserProtect(data);
		}

		/// <summary>
		/// Decrypts the data in a specified byte array and returns a byte array that contains the decrypted data.
		/// This method uses the current user account.
		/// </summary>
		/// <param name="data">The data to unprotect.</param>
		/// <returns>The stream containing the unprotected data.</returns>
		public MemoryStream UserUnprotect(byte[] data)
		{
			return new Nequeo.Security.Protection().UserUnprotect(data);
		}

		/// <summary>
		/// Encrypts the data in a specified byte array and returns a byte array that contains the encrypted data.
		/// This method uses the local machine account.
		/// </summary>
		/// <param name="data">The data to protect.</param>
		/// <returns>The stream containing the protected data.</returns>
		public MemoryStream MachineProtect(byte[] data)
		{
			return new Nequeo.Security.Protection().MachineProtect(data);
		}

		/// <summary>
		/// Decrypts the data in a specified byte array and returns a byte array that contains the decrypted data.
		/// This method uses the local machine account.
		/// </summary>
		/// <param name="data">The data to unprotect.</param>
		/// <returns>The stream containing the unprotected data.</returns>
		public MemoryStream MachineUnprotect(byte[] data)
		{
			return new Nequeo.Security.Protection().MachineUnprotect(data);
		}

		/// <summary>
		/// Encrypts the data in a specified byte array and returns a byte array that contains the encrypted data.
		/// This method uses the current user account.
		/// </summary>
		/// <param name="reader">The stream that contains the data to protect.</param>
		/// <returns>The stream containing the protected data.</returns>
		public MemoryStream UserProtect(Stream reader)
		{
			long length = reader.Length;
			byte[] buffer = new byte[length];
			int read = reader.Read(buffer, 0, (int)length);
			return new Nequeo.Security.Protection().UserProtect(buffer);
		}

		/// <summary>
		/// Decrypts the data in a specified byte array and returns a byte array that contains the decrypted data.
		/// This method uses the current user account.
		/// </summary>
		/// <param name="reader">The stream that contains the data to unprotect.</param>
		/// <returns>The stream containing the unprotected data.</returns>
		public MemoryStream UserUnprotect(Stream reader)
		{
			long length = reader.Length;
			byte[] buffer = new byte[length];
			int read = reader.Read(buffer, 0, (int)length);
			return new Nequeo.Security.Protection().UserUnprotect(buffer);
		}

		/// <summary>
		/// Encrypts the data in a specified byte array and returns a byte array that contains the encrypted data.
		/// This method uses the local machine account.
		/// </summary>
		/// <param name="reader">The stream that contains the data to protect.</param>
		/// <returns>The stream containing the protected data.</returns>
		public MemoryStream MachineProtect(Stream reader)
		{
			long length = reader.Length;
			byte[] buffer = new byte[length];
			int read = reader.Read(buffer, 0, (int)length);
			return new Nequeo.Security.Protection().MachineProtect(buffer);
		}

		/// <summary>
		/// Decrypts the data in a specified byte array and returns a byte array that contains the decrypted data.
		/// This method uses the local machine account.
		/// </summary>
		/// <param name="reader">The stream that contains the data to unprotect.</param>
		/// <returns>The stream containing the unprotected data.</returns>
		public MemoryStream MachineUnprotect(Stream reader)
		{
			long length = reader.Length;
			byte[] buffer = new byte[length];
			int read = reader.Read(buffer, 0, (int)length);
			return new Nequeo.Security.Protection().MachineUnprotect(buffer);
		}
	}
}
