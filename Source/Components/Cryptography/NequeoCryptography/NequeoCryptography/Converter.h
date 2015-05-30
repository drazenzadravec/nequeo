/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Converter.h
*  Purpose :       Converter class.
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

#ifndef _CONVERTER_H
#define _CONVERTER_H

#include "GlobalCryptography.h"
#include "CipherMode.h"
#include "PaddingMode.h"

#include "aes.h"
#include "modes.h"
#include "files.h"
#include "filters.h"
#include "cryptlib.h"

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// Conversion class
		/// </summary>
		class Converter
		{
		public:
			Converter();
			~Converter();

			/// <summary>
			/// This method will create a new padded password
			/// with the current password included.
			/// </summary>
			/// <param name="password">The current password.</param>
			/// <returns>The new padded password.</returns>
			unsigned char* GeneratePasswordBytes(string password);

			/// <summary>
			/// This method will encrypt a decrypted.
			/// </summary>
			/// <param name="decryptedData">The decrypted data.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="cipherMode">The cipher mode.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>The array of encrypted data.</returns>
			string EncryptToMemory(string decryptedData, unsigned char *key, size_t length, unsigned char *iv, CipherMode cipherMode, PaddingMode paddingMode);

			/// <summary>
			/// This method will decrypt an encrypted byte array.
			/// </summary>
			/// <param name="encryptedData">The array of encrypted bytes.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="cipherMode">The cipher mode.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>The decrypted data.</returns>
			string DecryptFromMemory(string encryptedData, unsigned char *key, size_t length, unsigned char *iv, CipherMode cipherMode, PaddingMode paddingMode);

			/// <summary>
			/// This method will encrypt a file to another file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
			/// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="cipherMode">The cipher mode.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool EncryptFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile,
				unsigned char *key, size_t length, unsigned char *iv, CipherMode cipherMode, PaddingMode paddingMode);

			/// <summary>
			/// This method will decrypt a file from an encrypted file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
			/// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="cipherMode">The cipher mode.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool DecryptFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile,
				unsigned char *key, size_t length, unsigned char *iv, CipherMode cipherMode, PaddingMode paddingMode);

			/// <summary>
			/// This method will encrypt a decrypted.
			/// </summary>
			/// <param name="decryptedData">The decrypted data.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>The array of encrypted data.</returns>
			string CbcEncryptor(string decryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// This method will encrypt a decrypted.
			/// </summary>
			/// <param name="decryptedData">The decrypted data.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>The array of encrypted data.</returns>
			string EcbEncryptor(string decryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// This method will encrypt a decrypted.
			/// </summary>
			/// <param name="decryptedData">The decrypted data.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>The array of encrypted data.</returns>
			string OfbEncryptor(string decryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// This method will encrypt a decrypted.
			/// </summary>
			/// <param name="decryptedData">The decrypted data.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>The array of encrypted data.</returns>
			string CfbEncryptor(string decryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// This method will encrypt a decrypted.
			/// </summary>
			/// <param name="decryptedData">The decrypted data.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>The array of encrypted data.</returns>
			string CtsEncryptor(string decryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// This method will decrypt an encrypted byte array.
			/// </summary>
			/// <param name="encryptedData">The array of encrypted bytes.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>The decrypted data.</returns>
			string CbcDecryptor(string encryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// This method will decrypt an encrypted byte array.
			/// </summary>
			/// <param name="encryptedData">The array of encrypted bytes.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>The decrypted data.</returns>
			string EcbDecryptor(string encryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// This method will decrypt an encrypted byte array.
			/// </summary>
			/// <param name="encryptedData">The array of encrypted bytes.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>The decrypted data.</returns>
			string OfbDecryptor(string encryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// This method will decrypt an encrypted byte array.
			/// </summary>
			/// <param name="encryptedData">The array of encrypted bytes.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>The decrypted data.</returns>
			string CfbDecryptor(string encryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// This method will decrypt an encrypted byte array.
			/// </summary>
			/// <param name="encryptedData">The array of encrypted bytes.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>The decrypted data.</returns>
			string CtsDecryptor(string encryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// This method will encrypt a file to another file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
			/// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool CbcEncryptorFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile,
				unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// This method will encrypt a file to another file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
			/// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool EcbEncryptorFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile,
				unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// This method will encrypt a file to another file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
			/// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool OfbEncryptorFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile,
				unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// This method will encrypt a file to another file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
			/// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool CfbEncryptorFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile,
				unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// This method will encrypt a file to another file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
			/// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool CtsEncryptorFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile,
				unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// This method will decrypt a file from an encrypted file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
			/// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool CbcDecryptorFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile,
				unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// This method will decrypt a file from an encrypted file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
			/// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool EcbDecryptorFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile,
				unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// This method will decrypt a file from an encrypted file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
			/// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool OfbDecryptorFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile,
				unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// This method will decrypt a file from an encrypted file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
			/// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool CfbDecryptorFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile,
				unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// This method will decrypt a file from an encrypted file.
			/// </summary>
			/// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
			/// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
			/// <param name="key">The key.</param>
			/// <param name="length">The the key length.</param>
			/// <param name="iv">The block vector.</param>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>True if no error has occurred else false.</returns>
			bool CtsDecryptorFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile,
				unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode);

			/// <summary>
			/// Get the block padding scheme.
			/// </summary>
			/// <param name="paddingMode">The padding mode.</param>
			/// <returns>The block padding scheme.</returns>
			CryptoPP::StreamTransformationFilter::BlockPaddingScheme PaddingScheme(PaddingMode paddingMode);

		private:
			bool _disposed;

			unsigned char _passwordKey[32];
			int _numberOfBytes;
			int _validKeyLength;
			int _validVectorLength;
		};
	}
}
#endif
