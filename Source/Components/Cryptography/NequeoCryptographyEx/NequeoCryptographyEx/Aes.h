/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          Aes.h
*  Purpose :       Aes class.
*
*/

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

#pragma once

#ifndef _AES_H
#define _AES_H

#include "CryptoGlobal.h"
#include "CipherMode.h"
#include "PaddingMode.h"

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// Advanced cryptography using windows Crypto API.
		/// </summary>
		class Aes
		{
		public:
			/// <summary>
			///	All of the Cryptography API initialisation is done in the 
			///	constructor, so constructing this object is expensive.
			/// </summary>
			Aes();

			/// <summary>
			/// This destructor.
			/// </summary>
			virtual ~Aes();

			/// <summary>
			/// Gets or sets the cipher mode.
			/// </summary>
			void setCipherMode(CryptCipherMode cipherMode);
			CryptCipherMode getCipherMode();

			/// <summary>
			/// Gets or sets the padding mode.
			/// </summary>
			void setPaddingMode(CryptPaddingMode paddingMode);
			CryptPaddingMode getPaddingMode();

			/// <summary>
			/// Gets or sets the initialization vector.
			/// </summary>
			/// <remarks>Gets an array of 16 bytes.</remarks>
			void setInitializationVector(BYTE iv[16]);
			array<BYTE, 16> getInitializationVector();

			/// <summary>
			/// Derive a key from a password.
			/// </summary>
			/// <param name="strPassword">The password (must be 32 bytes in length).</param>
			/// <returns>True if derived key; else false.</returns>
			/// <remarks>
			///	These functions are essential to using the crypto object- you must
			///	have a key from some source or other.
			/// </remarks>
			bool DeriveKey(CString strPassword);

			/// <summary>
			/// Encrypt the serializable object.
			/// </summary>
			/// <param name="serializable">The serializable object that contains the data to encrypt.</param>
			/// <param name="arData">The byte array where the encrypted data will be stored.</param>
			/// <returns>True if encrypted; else false.</returns>
			bool Encrypt(const CObject& serializable, CByteArray& arData);

			/// <summary>
			/// Encrypt the string.
			/// </summary>
			/// <param name="str">The string that contains the data to encrypt.</param>
			/// <param name="arData">The byte array where the encrypted data will be stored.</param>
			/// <returns>True if encrypted; else false.</returns>
			bool Encrypt(const CString& str, CByteArray& arData);

			/// <summary>
			/// Encrypt the decrypted file.
			/// </summary>
			/// <param name="szSource">The decrypted file.</param>
			/// <param name="szDestination">The encrypted file.</param>
			/// <param name="szPassword">The password used to encrypt the file.</param>
			/// <returns>True if encrypted; else false.</returns>
			bool Encrypt(PCHAR szSource, PCHAR szDestination, PCHAR szPassword);

			/// <summary>
			/// Decrypt the encrypted data into the object.
			/// </summary>
			/// <param name="arData">The byte array of encrypted data.</param>
			/// <param name="serializable">The serializable object that will contains the decrypt data.</param>
			/// <returns>True if decrypted; else false.</returns>
			bool Decrypt(const CByteArray& arData, CObject& serializable);

			/// <summary>
			/// Decrypt the encrypted data into the string.
			/// </summary>
			/// <param name="arData">The byte array of encrypted data.</param>
			/// <param name="str">The string that will contains the decrypt data.</param>
			/// <returns>True if decrypted; else false.</returns>
			bool Decrypt(const CByteArray& arData, CString& str);

			/// <summary>
			/// Decrypt the encrypted file.
			/// </summary>
			/// <param name="szSource">The encrypted file.</param>
			/// <param name="szDestination">The decrypted file.</param>
			/// <param name="szPassword">The password used to decrypt the file.</param>
			/// <returns>True if decrypted; else false.</returns>
			bool Decrypt(PCHAR szSource, PCHAR szDestination, PCHAR szPassword);

		protected:
			/// <summary>
			/// Encrypt the contents of the memory file and store in the passed array.
			/// </summary>
			/// <param name="arDestination">The array of bytes to encrypt.</param>
			/// <returns>True if encrypted; else false.</returns>
			virtual bool InternalEncrypt(CByteArray& arDestination);

			/// <summary>
			/// Decrypt the contents of the passed array and store in the memory file.
			/// </summary>
			/// <param name="arSource">The array of bytes to decrypt.</param>
			/// <returns>True if decrypted; else false.</returns>
			virtual bool InternalDecrypt(const CByteArray& arSource);

			// Handle to the cryptography provider.
			HCRYPTPROV _hCryptProv;

			// Handle to the cryptography key.
			HCRYPTKEY _hKey;

			// Handle to the hash object.
			HCRYPTHASH _hHash;

			// Internally, the encryption and decryption of data is done with
			// a CMemFile intermediate.
			CMemFile _file;

		private:
			CryptCipherMode _cipherMode;
			CryptPaddingMode _padding;
		};
	}
}
#endif