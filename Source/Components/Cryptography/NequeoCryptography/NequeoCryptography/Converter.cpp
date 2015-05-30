/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Converter.cpp
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

#include "stdafx.h"

#include "Converter.h"

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// Constructor for the current class.
		/// </summary>
		Converter::Converter() : _disposed(false)
		{
			_numberOfBytes = 1024;
			_validKeyLength = 32;
			_validVectorLength = 16;
		}

		/// <summary>
		/// This destructor.
		/// </summary>
		Converter::~Converter()
		{
			// If not disposed.
			if (!_disposed)
			{
				// Indicate that dispose has been called.
				_disposed = true;
			}
		}

		/// <summary>
		/// This method will create a new padded password
		/// with the current password included.
		/// </summary>
		/// <param name="password">The current password.</param>
		/// <returns>The new padded password.</returns>
		unsigned char* Converter::GeneratePasswordBytes(string password)
		{
			// Get the byte array equivalent 
			// of the current password.
			const char *currentPassword = password.c_str();
			int length = strlen(currentPassword);

			// If the current password is greater
			// then the maximum key size then return null.
			if (length > _validKeyLength)
				return nullptr;

			// For each byte in the array
			// create a new password.
			for (int i = 0; i < _validKeyLength; i++)
			{
				// if the current index is less than the current password
				// length then use the current password at the beginning.
				// else pad the remaining bytes with a valid byte.
				if (i < length)
					_passwordKey[i] = currentPassword[i];
				else
					_passwordKey[i] = i + 63;
			}

			// Return the new padded password.
			return _passwordKey;
		}

		/// <summary>
		/// Get the block padding scheme.
		/// </summary>
		/// <param name="paddingMode">The padding mode.</param>
		/// <returns>The block padding scheme.</returns>
		CryptoPP::StreamTransformationFilter::BlockPaddingScheme Converter::PaddingScheme(PaddingMode paddingMode)
		{
			// Default is zero padding.
			CryptoPP::StreamTransformationFilter::BlockPaddingScheme paddingScheme = CryptoPP::StreamTransformationFilter::BlockPaddingScheme::ZEROS_PADDING;

			// Select the corrent padding.
			switch (paddingMode)
			{
			case Nequeo::Cryptography::No:
				paddingScheme = CryptoPP::StreamTransformationFilter::BlockPaddingScheme::NO_PADDING;
				break;
			case Nequeo::Cryptography::PKCS7:
				paddingScheme = CryptoPP::StreamTransformationFilter::BlockPaddingScheme::PKCS_PADDING;
				break;
			case Nequeo::Cryptography::Zeros:
				paddingScheme = CryptoPP::StreamTransformationFilter::BlockPaddingScheme::ZEROS_PADDING;
				break;
			case Nequeo::Cryptography::ANSIX923:
				paddingScheme = CryptoPP::StreamTransformationFilter::BlockPaddingScheme::ONE_AND_ZEROS_PADDING;
				break;
			case Nequeo::Cryptography::ISO10126:
				paddingScheme = CryptoPP::StreamTransformationFilter::BlockPaddingScheme::ONE_AND_ZEROS_PADDING;
				break;
			default:
				paddingScheme = CryptoPP::StreamTransformationFilter::BlockPaddingScheme::DEFAULT_PADDING;
				break;
			}

			// return the padding
			return paddingScheme;
		}

		/// <summary>
		/// This method will encrypt a decrypted byte array.
		/// </summary>
		/// <param name="decryptedData">The decrypted data.</param>
		/// <param name="key">The key.</param>
		/// <param name="length">The the key length.</param>
		/// <param name="iv">The block vector.</param>
		/// <param name="cipherMode">The cipher mode.</param>
		/// <param name="paddingMode">The padding mode.</param>
		/// <returns>The array of encrypted data.</returns>
		string Converter::EncryptToMemory(string decryptedData, unsigned char *key, size_t length, unsigned char *iv, CipherMode cipherMode, PaddingMode paddingMode)
		{
			// Select the correct cipher.
			switch (cipherMode)
			{
			case Nequeo::Cryptography::CBC:
				return CbcEncryptor(decryptedData, key, length, iv, paddingMode);

			case Nequeo::Cryptography::ECB:
				return EcbEncryptor(decryptedData, key, length, iv, paddingMode);

			case Nequeo::Cryptography::OFB:
				return OfbEncryptor(decryptedData, key, length, iv, paddingMode);

			case Nequeo::Cryptography::CFB:
				return CfbEncryptor(decryptedData, key, length, iv, paddingMode);
				
			case Nequeo::Cryptography::CTS:
				return CtsEncryptor(decryptedData, key, length, iv, paddingMode);

			default:
				return CbcEncryptor(decryptedData, key, length, iv, paddingMode);
			}
		}

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
		string Converter::DecryptFromMemory(string encryptedData, unsigned char *key, size_t length, unsigned char *iv, CipherMode cipherMode, PaddingMode paddingMode)
		{
			// Select the correct cipher.
			switch (cipherMode)
			{
			case Nequeo::Cryptography::CBC:
				return CbcDecryptor(encryptedData, key, length, iv, paddingMode);

			case Nequeo::Cryptography::ECB:
				return EcbDecryptor(encryptedData, key, length, iv, paddingMode);

			case Nequeo::Cryptography::OFB:
				return OfbDecryptor(encryptedData, key, length, iv, paddingMode);

			case Nequeo::Cryptography::CFB:
				return CfbDecryptor(encryptedData, key, length, iv, paddingMode);

			case Nequeo::Cryptography::CTS:
				return CtsDecryptor(encryptedData, key, length, iv, paddingMode);

			default:
				return CbcDecryptor(encryptedData, key, length, iv, paddingMode);
			}
		}

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
		bool Converter::EncryptFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile, 
			unsigned char *key, size_t length, unsigned char *iv, CipherMode cipherMode, PaddingMode paddingMode)
		{
			// Select the correct cipher.
			switch (cipherMode)
			{
			case Nequeo::Cryptography::CBC:
				return CbcEncryptorFile(pathToDecryptedFile, pathToEncryptedFile, key, length, iv, paddingMode);

			case Nequeo::Cryptography::ECB:
				return EcbEncryptorFile(pathToDecryptedFile, pathToEncryptedFile, key, length, iv, paddingMode);

			case Nequeo::Cryptography::OFB:
				return OfbEncryptorFile(pathToDecryptedFile, pathToEncryptedFile, key, length, iv, paddingMode);

			case Nequeo::Cryptography::CFB:
				return CfbEncryptorFile(pathToDecryptedFile, pathToEncryptedFile, key, length, iv, paddingMode);

			case Nequeo::Cryptography::CTS:
				return CtsEncryptorFile(pathToDecryptedFile, pathToEncryptedFile, key, length, iv, paddingMode);

			default:
				return CbcEncryptorFile(pathToDecryptedFile, pathToEncryptedFile, key, length, iv, paddingMode);
			}
		}

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
		bool Converter::CbcEncryptorFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile,
			unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// Encryptor.
			CryptoPP::CBC_Mode<CryptoPP::AES>::Encryption encryptor(key, length, iv);

			// Encryption.
			CryptoPP::FileSource(pathToDecryptedFile, true,
				new CryptoPP::StreamTransformationFilter(encryptor,
				new CryptoPP::FileSink(pathToEncryptedFile),
				PaddingScheme(paddingMode)));

			// Return.
			return true;
		}

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
		bool Converter::EcbEncryptorFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile,
			unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// Encryptor.
			CryptoPP::ECB_Mode<CryptoPP::AES>::Encryption encryptor(key, length, iv);

			// Encryption.
			CryptoPP::FileSource(pathToDecryptedFile, true,
				new CryptoPP::StreamTransformationFilter(encryptor,
				new CryptoPP::FileSink(pathToEncryptedFile),
				PaddingScheme(paddingMode)));

			// Return.
			return true;
		}

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
		bool Converter::OfbEncryptorFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile,
			unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// Encryptor.
			CryptoPP::OFB_Mode<CryptoPP::AES>::Encryption encryptor(key, length, iv);

			// Encryption.
			CryptoPP::FileSource(pathToDecryptedFile, true,
				new CryptoPP::StreamTransformationFilter(encryptor,
				new CryptoPP::FileSink(pathToEncryptedFile),
				PaddingScheme(paddingMode)));

			// Return.
			return true;
		}

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
		bool Converter::CfbEncryptorFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile,
			unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// Encryptor.
			CryptoPP::CFB_Mode<CryptoPP::AES>::Encryption encryptor(key, length, iv);

			// Encryption.
			CryptoPP::FileSource(pathToDecryptedFile, true,
				new CryptoPP::StreamTransformationFilter(encryptor,
				new CryptoPP::FileSink(pathToEncryptedFile),
				PaddingScheme(paddingMode)));

			// Return.
			return true;
		}

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
		bool Converter::CtsEncryptorFile(const char *pathToDecryptedFile, const char * pathToEncryptedFile,
			unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// Encryptor.
			CryptoPP::CBC_CTS_Mode<CryptoPP::AES>::Encryption encryptor(key, length, iv);

			// Encryption.
			CryptoPP::FileSource(pathToDecryptedFile, true,
				new CryptoPP::StreamTransformationFilter(encryptor,
				new CryptoPP::FileSink(pathToEncryptedFile),
				PaddingScheme(paddingMode)));

			// Return.
			return true;
		}

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
		bool Converter::DecryptFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile,
			unsigned char *key, size_t length, unsigned char *iv, CipherMode cipherMode, PaddingMode paddingMode)
		{
			// Select the correct cipher.
			switch (cipherMode)
			{
			case Nequeo::Cryptography::CBC:
				return CbcDecryptorFile(pathToDecryptedFile, pathToEncryptedFile, key, length, iv, paddingMode);

			case Nequeo::Cryptography::ECB:
				return EcbDecryptorFile(pathToDecryptedFile, pathToEncryptedFile, key, length, iv, paddingMode);

			case Nequeo::Cryptography::OFB:
				return OfbDecryptorFile(pathToDecryptedFile, pathToEncryptedFile, key, length, iv, paddingMode);

			case Nequeo::Cryptography::CFB:
				return CfbDecryptorFile(pathToDecryptedFile, pathToEncryptedFile, key, length, iv, paddingMode);

			case Nequeo::Cryptography::CTS:
				return CtsDecryptorFile(pathToDecryptedFile, pathToEncryptedFile, key, length, iv, paddingMode);

			default:
				return CbcDecryptorFile(pathToDecryptedFile, pathToEncryptedFile, key, length, iv, paddingMode);
			}
		}

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
		bool Converter::CbcDecryptorFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile,
			unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// Decryptor.
			CryptoPP::CBC_Mode<CryptoPP::AES>::Decryption decryptor(key, length, iv);

			// Decryption.
			CryptoPP::FileSource(pathToEncryptedFile, true,
				new CryptoPP::StreamTransformationFilter(decryptor,
				new CryptoPP::FileSink(pathToDecryptedFile),
				PaddingScheme(paddingMode)));

			// Return.
			return true;
		}

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
		bool Converter::EcbDecryptorFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile,
			unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// Decryptor.
			CryptoPP::ECB_Mode<CryptoPP::AES>::Decryption decryptor(key, length, iv);

			// Decryption.
			CryptoPP::FileSource(pathToEncryptedFile, true,
				new CryptoPP::StreamTransformationFilter(decryptor,
				new CryptoPP::FileSink(pathToDecryptedFile),
				PaddingScheme(paddingMode)));

			// Return.
			return true;
		}

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
		bool Converter::OfbDecryptorFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile,
			unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// Decryptor.
			CryptoPP::OFB_Mode<CryptoPP::AES>::Decryption decryptor(key, length, iv);

			// Decryption.
			CryptoPP::FileSource(pathToEncryptedFile, true,
				new CryptoPP::StreamTransformationFilter(decryptor,
				new CryptoPP::FileSink(pathToDecryptedFile),
				PaddingScheme(paddingMode)));

			// Return.
			return true;
		}

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
		bool Converter::CfbDecryptorFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile,
			unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// Decryptor.
			CryptoPP::CFB_Mode<CryptoPP::AES>::Decryption decryptor(key, length, iv);

			// Decryption.
			CryptoPP::FileSource(pathToEncryptedFile, true,
				new CryptoPP::StreamTransformationFilter(decryptor,
				new CryptoPP::FileSink(pathToDecryptedFile),
				PaddingScheme(paddingMode)));

			// Return.
			return true;
		}

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
		bool Converter::CtsDecryptorFile(const char *pathToDecryptedFile, const char *pathToEncryptedFile,
			unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// Decryptor.
			CryptoPP::CBC_CTS_Mode<CryptoPP::AES>::Decryption decryptor(key, length, iv);

			// Decryption.
			CryptoPP::FileSource(pathToEncryptedFile, true,
				new CryptoPP::StreamTransformationFilter(decryptor,
				new CryptoPP::FileSink(pathToDecryptedFile),
				PaddingScheme(paddingMode)));

			// Return.
			return true;
		}

		/// <summary>
		/// This method will encrypt a decrypted.
		/// </summary>
		/// <param name="decryptedData">The decrypted data.</param>
		/// <param name="key">The key.</param>
		/// <param name="length">The the key length.</param>
		/// <param name="iv">The block vector.</param>
		/// <param name="paddingMode">The padding mode.</param>
		/// <returns>The array of encrypted data.</returns>
		string Converter::CbcEncryptor(string decryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// crypto data store.
			std::string decryptedText = std::string(decryptedData);
			std::string encryptedText;

			// Encryptor.
			CryptoPP::CBC_Mode<CryptoPP::AES>::Encryption encryptor(key, length, iv);

			// Encryption.
			CryptoPP::StringSource(decryptedText, true,
				new CryptoPP::StreamTransformationFilter(encryptor,
				new CryptoPP::StringSink(encryptedText),
				PaddingScheme(paddingMode)));

			// Return the encrypted data.
			return encryptedText;
		}

		/// <summary>
		/// This method will encrypt a decrypted.
		/// </summary>
		/// <param name="decryptedData">The decrypted data.</param>
		/// <param name="key">The key.</param>
		/// <param name="length">The the key length.</param>
		/// <param name="iv">The block vector.</param>
		/// <param name="paddingMode">The padding mode.</param>
		/// <returns>The array of encrypted data.</returns>
		string Converter::EcbEncryptor(string decryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// crypto data store.
			std::string decryptedText = std::string(decryptedData);
			std::string encryptedText;

			// Encryptor.
			CryptoPP::ECB_Mode<CryptoPP::AES>::Encryption encryptor(key, length, iv);

			// Encryption.
			CryptoPP::StringSource(decryptedText, true,
				new CryptoPP::StreamTransformationFilter(encryptor,
				new CryptoPP::StringSink(encryptedText),
				PaddingScheme(paddingMode)));

			// Return the encrypted data.
			return encryptedText;
		}

		/// <summary>
		/// This method will encrypt a decrypted.
		/// </summary>
		/// <param name="decryptedData">The decrypted data.</param>
		/// <param name="key">The key.</param>
		/// <param name="length">The the key length.</param>
		/// <param name="iv">The block vector.</param>
		/// <param name="paddingMode">The padding mode.</param>
		/// <returns>The array of encrypted data.</returns>
		string Converter::OfbEncryptor(string decryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// crypto data store.
			std::string decryptedText = std::string(decryptedData);
			std::string encryptedText;

			// Encryptor.
			CryptoPP::OFB_Mode<CryptoPP::AES>::Encryption encryptor(key, length, iv);

			// Encryption.
			CryptoPP::StringSource(decryptedText, true,
				new CryptoPP::StreamTransformationFilter(encryptor,
				new CryptoPP::StringSink(encryptedText),
				PaddingScheme(paddingMode)));

			// Return the encrypted data.
			return encryptedText;
		}

		/// <summary>
		/// This method will encrypt a decrypted.
		/// </summary>
		/// <param name="decryptedData">The decrypted data.</param>
		/// <param name="key">The key.</param>
		/// <param name="length">The the key length.</param>
		/// <param name="iv">The block vector.</param>
		/// <param name="paddingMode">The padding mode.</param>
		/// <returns>The array of encrypted data.</returns>
		string Converter::CfbEncryptor(string decryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// crypto data store.
			std::string decryptedText = std::string(decryptedData);
			std::string encryptedText;

			// Encryptor.
			CryptoPP::CFB_Mode<CryptoPP::AES>::Encryption encryptor(key, length, iv);

			// Encryption.
			CryptoPP::StringSource(decryptedText, true,
				new CryptoPP::StreamTransformationFilter(encryptor,
				new CryptoPP::StringSink(encryptedText),
				PaddingScheme(paddingMode)));

			// Return the encrypted data.
			return encryptedText;
		}

		/// <summary>
		/// This method will encrypt a decrypted.
		/// </summary>
		/// <param name="decryptedData">The decrypted data.</param>
		/// <param name="key">The key.</param>
		/// <param name="length">The the key length.</param>
		/// <param name="iv">The block vector.</param>
		/// <param name="paddingMode">The padding mode.</param>
		/// <returns>The array of encrypted data.</returns>
		string Converter::CtsEncryptor(string decryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// crypto data store.
			std::string decryptedText = std::string(decryptedData);
			std::string encryptedText;

			// Encryptor.
			CryptoPP::CBC_CTS_Mode<CryptoPP::AES>::Encryption encryptor(key, length, iv);

			// Encryption.
			CryptoPP::StringSource(decryptedText, true,
				new CryptoPP::StreamTransformationFilter(encryptor,
				new CryptoPP::StringSink(encryptedText),
				PaddingScheme(paddingMode)));

			// Return the encrypted data.
			return encryptedText;
		}

		/// <summary>
		/// This method will decrypt an encrypted byte array.
		/// </summary>
		/// <param name="encryptedData">The array of encrypted bytes.</param>
		/// <param name="key">The key.</param>
		/// <param name="length">The the key length.</param>
		/// <param name="iv">The block vector.</param>
		/// <param name="paddingMode">The padding mode.</param>
		/// <returns>The decrypted data.</returns>
		string Converter::CbcDecryptor(string encryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// crypto data store.
			std::string encryptedText = std::string(encryptedData);
			std::string decryptedText;

			// Decryptor.
			CryptoPP::CBC_Mode<CryptoPP::AES>::Decryption decryptor(key, length, iv);

			// Decryption.
			CryptoPP::StringSource(encryptedText, true,
				new CryptoPP::StreamTransformationFilter(decryptor,
				new CryptoPP::StringSink(decryptedText),
				PaddingScheme(paddingMode)));

			// Return the decrypted data.
			return decryptedText;
		}

		/// <summary>
		/// This method will decrypt an encrypted byte array.
		/// </summary>
		/// <param name="encryptedData">The array of encrypted bytes.</param>
		/// <param name="key">The key.</param>
		/// <param name="length">The the key length.</param>
		/// <param name="iv">The block vector.</param>
		/// <param name="paddingMode">The padding mode.</param>
		/// <returns>The decrypted data.</returns>
		string Converter::EcbDecryptor(string encryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// crypto data store.
			std::string encryptedText = std::string(encryptedData);
			std::string decryptedText;

			// Decryptor.
			CryptoPP::ECB_Mode<CryptoPP::AES>::Decryption decryptor(key, length, iv);

			// Decryption.
			CryptoPP::StringSource(encryptedText, true,
				new CryptoPP::StreamTransformationFilter(decryptor,
				new CryptoPP::StringSink(decryptedText),
				PaddingScheme(paddingMode)));

			// Return the decrypted data.
			return decryptedText;
		}

		/// <summary>
		/// This method will decrypt an encrypted byte array.
		/// </summary>
		/// <param name="encryptedData">The array of encrypted bytes.</param>
		/// <param name="key">The key.</param>
		/// <param name="length">The the key length.</param>
		/// <param name="iv">The block vector.</param>
		/// <param name="paddingMode">The padding mode.</param>
		/// <returns>The decrypted data.</returns>
		string Converter::OfbDecryptor(string encryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// crypto data store.
			std::string encryptedText = std::string(encryptedData);
			std::string decryptedText;

			// Decryptor.
			CryptoPP::OFB_Mode<CryptoPP::AES>::Decryption decryptor(key, length, iv);

			// Decryption.
			CryptoPP::StringSource(encryptedText, true,
				new CryptoPP::StreamTransformationFilter(decryptor,
				new CryptoPP::StringSink(decryptedText),
				PaddingScheme(paddingMode)));

			// Return the decrypted data.
			return decryptedText;
		}

		/// <summary>
		/// This method will decrypt an encrypted byte array.
		/// </summary>
		/// <param name="encryptedData">The array of encrypted bytes.</param>
		/// <param name="key">The key.</param>
		/// <param name="length">The the key length.</param>
		/// <param name="iv">The block vector.</param>
		/// <param name="paddingMode">The padding mode.</param>
		/// <returns>The decrypted data.</returns>
		string Converter::CfbDecryptor(string encryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// crypto data store.
			std::string encryptedText = std::string(encryptedData);
			std::string decryptedText;

			// Decryptor.
			CryptoPP::CFB_Mode<CryptoPP::AES>::Decryption decryptor(key, length, iv);

			// Decryption.
			CryptoPP::StringSource(encryptedText, true,
				new CryptoPP::StreamTransformationFilter(decryptor,
				new CryptoPP::StringSink(decryptedText),
				PaddingScheme(paddingMode)));

			// Return the decrypted data.
			return decryptedText;
		}

		/// <summary>
		/// This method will decrypt an encrypted byte array.
		/// </summary>
		/// <param name="encryptedData">The array of encrypted bytes.</param>
		/// <param name="key">The key.</param>
		/// <param name="length">The the key length.</param>
		/// <param name="iv">The block vector.</param>
		/// <param name="paddingMode">The padding mode.</param>
		/// <returns>The decrypted data.</returns>
		string Converter::CtsDecryptor(string encryptedData, unsigned char *key, size_t length, unsigned char *iv, PaddingMode paddingMode)
		{
			// crypto data store.
			std::string encryptedText = std::string(encryptedData);
			std::string decryptedText;

			// Decryptor.
			CryptoPP::CBC_CTS_Mode<CryptoPP::AES>::Decryption decryptor(key, length, iv);

			// Decryption.
			CryptoPP::StringSource(encryptedText, true,
				new CryptoPP::StreamTransformationFilter(decryptor,
				new CryptoPP::StringSink(decryptedText),
				PaddingScheme(paddingMode)));

			// Return the decrypted data.
			return decryptedText;
		}
	}
}
