/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          AdvancedAES.h
*  Purpose :       AdvancedAES class.
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

#ifndef _ADVANCEDAES_H
#define _ADVANCEDAES_H

#include "GlobalCryptography.h"
#include "CipherMode.h"
#include "PaddingMode.h"

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// Advanced encryption decryption of data using AES.
		/// </summary>
		class AdvancedAES
		{
		public:
			AdvancedAES();
			~AdvancedAES();

			/// <summary>
			/// Gets or sets the cipher mode.
			/// </summary>
			void setCipherMode(CipherMode cipherMode);
			CipherMode getCipherMode() const;

			/// <summary>
			/// Gets or sets the padding mode.
			/// </summary>
			void setPaddingMode(PaddingMode paddingMode);
			PaddingMode getPaddingMode() const;

			/// <summary>
			/// Gets or sets the block size.
			/// </summary>
			void setBlockSize(int blockSize);
			int getBlockSize() const;

			/// <summary>
			/// Gets or sets the key size.
			/// </summary>
			void setKeySize(int keySize);
			int getKeySize() const;

			/// <summary>
			/// This method will encrypt a decrypted.
			/// </summary>
			/// <param name="decryptedData">The decrypted data.</param>
			/// <returns>The array of encrypted data.</returns>
			string EncryptToMemory(string decryptedData);

			/// <summary>
			/// This method will encrypt a decrypted.
			/// </summary>
			/// <param name="decryptedData">The decrypted data.</param>
			/// <param name="key">The current key for cryptography.</param>
			/// <returns>The array of encrypted data.</returns>
			string EncryptToMemory(string decryptedData, unsigned char *key);

			/// <summary>
			/// This method will encrypt a decrypted.
			/// </summary>
			/// <param name="decryptedData">The decrypted data.</param>
			/// <param name="password">The password used for cryptography.</param>
			/// <returns>The array of encrypted data.</returns>
			string EncryptToMemory(string decryptedData, string password);

			/// <summary>
			/// This method will encrypt a decrypted.
			/// </summary>
			/// <param name="decryptedData">The decrypted data.</param>
			/// <param name="key">The current key for cryptography.</param>
			/// <param name="iv">The current initialising vector.</param>
			/// <returns>The array of encrypted data.</returns>
			string EncryptToMemory(string decryptedData, unsigned char *key, unsigned char *iv);

			/// <summary>
			/// This method will decrypt an encrypted byte array.
			/// </summary>
			/// <param name="encryptedData">The array of encrypted bytes.</param>
			/// <returns>The decrypted data.</returns>
			string DecryptFromMemory(string encryptedData);

			/// <summary>
			/// This method will decrypt an encrypted byte array.
			/// </summary>
			/// <param name="encryptedData">The array of encrypted bytes.</param>
			/// <param name="key">The current key for cryptography.</param>
			/// <returns>The decrypted data.</returns>
			string DecryptFromMemory(string encryptedData, unsigned char *key);

			/// <summary>
			/// This method will decrypt an encrypted byte array.
			/// </summary>
			/// <param name="encryptedData">The array of encrypted bytes.</param>
			/// <param name="password">The password used for cryptography.</param>
			/// <returns>The decrypted data.</returns>
			string DecryptFromMemory(string encryptedData, string password);

			/// <summary>
			/// This method will decrypt an encrypted byte array.
			/// </summary>
			/// <param name="encryptedData">The array of encrypted bytes.</param>
			/// <param name="key">The current key for cryptography.</param>
			/// <param name="iv">The current initialising vector.</param>
			/// <returns>The decrypted data.</returns>
			string DecryptFromMemory(string encryptedData, unsigned char *key, unsigned char *iv);

			/// <summary>
			/// This method will encrypt a file to another file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
			/// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool EncryptFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile);

			/// <summary>
			/// This method will encrypt a file to another file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
			/// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
			/// <param name="password">The password used for cryptography.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool EncryptFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile, string password);

			/// <summary>
			/// This method will encrypt a file to another file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
			/// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
			/// <param name="key">The current key for cryptography.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool EncryptFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile, unsigned char *key);

			/// <summary>
			/// This method will encrypt a file to another file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
			/// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
			/// <param name="key">The current key for cryptography.</param>
			/// <param name="iv">The current initialising vector.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool EncryptFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile, unsigned char *key, unsigned char *iv);

			/// <summary>
			/// This method will decrypt a file from an encrypted file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
			/// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool DecryptFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile);

			/// <summary>
			/// This method will decrypt a file from an encrypted file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
			/// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
			/// <param name="password">The password used for cryptography.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool DecryptFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile, string password);

			/// <summary>
			/// This method will decrypt a file from an encrypted file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
			/// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
			/// <param name="key">The current key for cryptography.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool DecryptFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile, unsigned char *key);

			/// <summary>
			/// This method will decrypt a file from an encrypted file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
			/// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
			/// <param name="key">The current key for cryptography.</param>
			/// <param name="iv">The current initialising vector.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool DecryptFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile, unsigned char *key, unsigned char *iv);

		private:
			bool _disposed;

			CipherMode _cipherMode;
			PaddingMode _padding;
			int _blockSize;
			int _keySize;

			unsigned char _internalKey[32];
			unsigned char _internalIV[16];

			int _numberOfBytes;
			int _validKeyLength;
			int _validVectorLength;
		};
	}
}
#endif