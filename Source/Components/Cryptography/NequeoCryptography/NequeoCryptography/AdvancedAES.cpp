/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          AdvancedAES.cpp
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

#include "stdafx.h"

#include "AdvancedAES.h"
#include "Converter.h"

using Nequeo::Cryptography::Converter;

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// Constructor for the current class.
		/// </summary>
		AdvancedAES::AdvancedAES() : _disposed(false)
		{
			_cipherMode = CipherMode::CFB;
			_padding = PaddingMode::Zeros;
			_blockSize = 128;
			_keySize = 256;

			_numberOfBytes = 1024;
			_validKeyLength = 32;
			_validVectorLength = 16;

			// Assign the default key.
			_internalKey[0] = 23;
			_internalKey[1] = 80;
			_internalKey[2] = 90;
			_internalKey[3] = 34;
			_internalKey[4] = 200;
			_internalKey[5] = 215;
			_internalKey[6] = 167;
			_internalKey[7] = 97;
			_internalKey[8] = 132;
			_internalKey[9] = 104;
			_internalKey[10] = 67;
			_internalKey[11] = 34;
			_internalKey[12] = 99;
			_internalKey[13] = 235;
			_internalKey[14] = 240;
			_internalKey[15] = 57;
			_internalKey[16] = 25;
			_internalKey[17] = 79;
			_internalKey[18] = 67;
			_internalKey[19] = 113;
			_internalKey[20] = 147;
			_internalKey[21] = 156;
			_internalKey[22] = 167;
			_internalKey[23] = 251;
			_internalKey[24] = 20;
			_internalKey[25] = 39;
			_internalKey[26] = 69;
			_internalKey[27] = 125;
			_internalKey[28] = 149;
			_internalKey[29] = 214;
			_internalKey[30] = 202;
			_internalKey[31] = 53;

			// Assign the default IV.
			_internalIV[0] = 18;
			_internalIV[1] = 43;
			_internalIV[2] = 63;
			_internalIV[3] = 126;
			_internalIV[4] = 169;
			_internalIV[5] = 214;
			_internalIV[6] = 232;
			_internalIV[7] = 47;
			_internalIV[8] = 19;
			_internalIV[9] = 41;
			_internalIV[10] = 70;
			_internalIV[11] = 127;
			_internalIV[12] = 159;
			_internalIV[13] = 224;
			_internalIV[14] = 222;
			_internalIV[15] = 58;
		}

		/// <summary>
		/// This destructor.
		/// </summary>
		AdvancedAES::~AdvancedAES()
		{
			// If not disposed.
			if (!_disposed)
			{
				// Indicate that dispose has been called.
				_disposed = true;
			}
		}

		/// <summary>
		/// Sets the cipher mode.
		/// </summary>
		void AdvancedAES::setCipherMode(CipherMode cipherMode)
		{
			_cipherMode = cipherMode;
		}

		/// <summary>
		/// Gets the cipher mode.
		/// </summary>
		CipherMode AdvancedAES::getCipherMode() const
		{
			return _cipherMode;
		}

		/// <summary>
		/// Sets the padding mode.
		/// </summary>
		void AdvancedAES::setPaddingMode(PaddingMode paddingMode)
		{
			_padding = paddingMode;
		}

		/// <summary>
		/// Gets the padding mode.
		/// </summary>
		PaddingMode AdvancedAES::getPaddingMode() const
		{
			return _padding;
		}

		/// <summary>
		/// Sets the block size.
		/// </summary>
		void AdvancedAES::setBlockSize(int blockSize)
		{
			_blockSize = blockSize;
		}

		/// <summary>
		/// Gets the block size.
		/// </summary>
		int AdvancedAES::getBlockSize() const
		{
			return _blockSize;
		}

		/// <summary>
		/// Sets the key size.
		/// </summary>
		void AdvancedAES::setKeySize(int keySize)
		{
			_keySize = keySize;
		}

		/// <summary>
		/// Gets the key size.
		/// </summary>
		int AdvancedAES::getKeySize() const
		{
			return _keySize;
		}

		/// <summary>
		/// This method will encrypt a decrypted.
		/// </summary>
		/// <param name="decryptedData">The decrypted data.</param>
		/// <returns>The array of encrypted data.</returns>
		string AdvancedAES::EncryptToMemory(string decryptedData)
		{
			Converter conv;

			// Return the encrypted data.
			return conv.EncryptToMemory(decryptedData, _internalKey, _validKeyLength, _internalIV, _cipherMode, _padding);
		}

		/// <summary>
		/// This method will encrypt a decrypted.
		/// </summary>
		/// <param name="decryptedData">The decrypted data.</param>
		/// <param name ="key">The current key for cryptography.</param>
		/// <returns>The array of encrypted data.</returns>
		string AdvancedAES::EncryptToMemory(string decryptedData, unsigned char *key)
		{
			Converter conv;

			// Return the encrypted data.
			return conv.EncryptToMemory(decryptedData, key, _validKeyLength, _internalIV, _cipherMode, _padding);
		}

		/// <summary>
		/// This method will encrypt a decrypted.
		/// </summary>
		/// <param name="decryptedData">The decrypted data.</param>
		/// <param name="password">The password used for cryptography.</param>
		/// <returns>The array of encrypted data.</returns>
		string AdvancedAES::EncryptToMemory(string decryptedData, string password)
		{
			Converter conv;
			unsigned char* key = conv.GeneratePasswordBytes(password);

			// Return the encrypted data.
			return conv.EncryptToMemory(decryptedData, key, _validKeyLength, _internalIV, _cipherMode, _padding);
		}

		/// <summary>
		/// This method will encrypt a decrypted.
		/// </summary>
		/// <param name="decryptedData">The decrypted data.</param>
		/// <param name="key">The current key for cryptography.</param>
		/// <param name="iv">The current initialising vector.</param>
		/// <returns>The array of encrypted data.</returns>
		string AdvancedAES::EncryptToMemory(string decryptedData, unsigned char *key, unsigned char *iv)
		{
			Converter conv;

			// Return the encrypted data.
			return conv.EncryptToMemory(decryptedData, key, _validKeyLength, iv, _cipherMode, _padding);
		}

		/// <summary>
		/// This method will decrypt an encrypted byte array.
		/// </summary>
		/// <param name="encryptedData">The array of encrypted bytes.</param>
		/// <returns>The decrypted data.</returns>
		string AdvancedAES::DecryptFromMemory(string encryptedData)
		{
			Converter conv;

			// Return the decrypted data.
			return conv.DecryptFromMemory(encryptedData, _internalKey, _validKeyLength, _internalIV, _cipherMode, _padding);
		}

		/// <summary>
		/// This method will decrypt an encrypted byte array.
		/// </summary>
		/// <param name="encryptedData">The array of encrypted bytes.</param>
		/// <param name="key">The current key for cryptography.</param>
		/// <returns>The decrypted data.</returns>
		string AdvancedAES::DecryptFromMemory(string encryptedData, unsigned char *key)
		{
			Converter conv;

			// Return the decrypted data.
			return conv.DecryptFromMemory(encryptedData, key, _validKeyLength, _internalIV, _cipherMode, _padding);
		}

		/// <summary>
		/// This method will decrypt an encrypted byte array.
		/// </summary>
		/// <param name="encryptedData">The array of encrypted bytes.</param>
		/// <param name="password">The password used for cryptography.</param>
		/// <returns>The decrypted data.</returns>
		string AdvancedAES::DecryptFromMemory(string encryptedData, string password)
		{
			Converter conv;
			unsigned char* key = conv.GeneratePasswordBytes(password);

			// Return the decrypted data.
			return conv.DecryptFromMemory(encryptedData, key, _validKeyLength, _internalIV, _cipherMode, _padding);
		}

		/// <summary>
		/// This method will decrypt an encrypted byte array.
		/// </summary>
		/// <param name="encryptedData">The array of encrypted bytes.</param>
		/// <param name="key">The current key for cryptography.</param>
		/// <param name="iv">The current initialising vector.</param>
		/// <returns>The decrypted data.</returns>
		string AdvancedAES::DecryptFromMemory(string encryptedData, unsigned char *key, unsigned char *iv)
		{
			Converter conv;

			// Return the decrypted data.
			return conv.DecryptFromMemory(encryptedData, key, _validKeyLength, iv, _cipherMode, _padding);
		}

		/// <summary>
		/// This method will encrypt a file to another file.
		/// </summary>
		/// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
		/// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
		/// <returns>True if no error has occurred else false.</returns>
		bool AdvancedAES::EncryptFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile)
		{
			Converter conv;
			return conv.EncryptFile(pathToDecryptedFile, pathToEncryptedFile, _internalKey, _validKeyLength, _internalIV, _cipherMode, _padding);
		}

		/// <summary>
		/// This method will encrypt a file to another file.
		/// </summary>
		/// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
		/// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
		/// <param name="password">The password used for cryptography.</param>
		/// <returns>True if no error has occurred else false.</returns>
		bool AdvancedAES::EncryptFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile, string password)
		{
			Converter conv;
			unsigned char* key = conv.GeneratePasswordBytes(password);
			return conv.EncryptFile(pathToDecryptedFile, pathToEncryptedFile, key, _validKeyLength, _internalIV, _cipherMode, _padding);
		}

		/// <summary>
		/// This method will encrypt a file to another file.
		/// </summary>
		/// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
		/// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
		/// <param name="key">The current key for cryptography.</param>
		/// <returns>True if no error has occurred else false.</returns>
		bool AdvancedAES::EncryptFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile, unsigned char *key)
		{
			Converter conv;
			return conv.EncryptFile(pathToDecryptedFile, pathToEncryptedFile, key, _validKeyLength, _internalIV, _cipherMode, _padding);
		}

		/// <summary>
		/// This method will encrypt a file to another file.
		/// </summary>
		/// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
		/// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
		/// <param name="key">The current key for cryptography.</param>
		/// <param name="iv">The current initialising vector.</param>
		/// <returns>True if no error has occurred else false.</returns>
		bool AdvancedAES::EncryptFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile, unsigned char *key, unsigned char *iv)
		{
			Converter conv;
			return conv.EncryptFile(pathToDecryptedFile, pathToEncryptedFile, key, _validKeyLength, iv, _cipherMode, _padding);
		}

		/// <summary>
		/// This method will decrypt a file from an encrypted file.
		/// </summary>
		/// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
		/// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
		/// <returns>True if no error has occurred else false.</returns>
		bool AdvancedAES::DecryptFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile)
		{
			Converter conv;
			return conv.DecryptFile(pathToDecryptedFile, pathToEncryptedFile, _internalKey, _validKeyLength, _internalIV, _cipherMode, _padding);
		}

		/// <summary>
		/// This method will decrypt a file from an encrypted file.
		/// </summary>
		/// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
		/// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
		/// <param name="password">The password used for cryptography.</param>
		/// <returns>True if no error has occurred else false.</returns>
		bool AdvancedAES::DecryptFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile, string password)
		{
			Converter conv;
			unsigned char* key = conv.GeneratePasswordBytes(password);
			return conv.DecryptFile(pathToDecryptedFile, pathToEncryptedFile, key, _validKeyLength, _internalIV, _cipherMode, _padding);
		}

		/// <summary>
		/// This method will decrypt a file from an encrypted file.
		/// </summary>
		/// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
		/// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
		/// <param name="key">The current key for cryptography.</param>
		/// <returns>True if no error has occurred else false.</returns>
		bool AdvancedAES::DecryptFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile, unsigned char *key)
		{
			Converter conv;
			return conv.DecryptFile(pathToDecryptedFile, pathToEncryptedFile, key, _validKeyLength, _internalIV, _cipherMode, _padding);
		}

		/// <summary>
		/// This method will decrypt a file from an encrypted file.
		/// </summary>
		/// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
		/// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
		/// <param name="key">The current key for cryptography.</param>
		/// <param name="iv">The current initialising vector.</param>
		/// <returns>True if no error has occurred else false.</returns>
		bool AdvancedAES::DecryptFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile, unsigned char *key, unsigned char *iv)
		{
			Converter conv;
			return conv.DecryptFile(pathToDecryptedFile, pathToEncryptedFile, key, _validKeyLength, iv, _cipherMode, _padding);
		}
	}
}