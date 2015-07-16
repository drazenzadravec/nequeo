/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          Aes.cpp
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

#include "stdafx.h"

#include "Aes.h"

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		///	All of the Cryptography API initialisation is done in the 
		///	constructor, so constructing this object is expensive.
		/// </summary>
		Aes::Aes() : _hCryptProv(NULL), _hKey(NULL), _hHash(NULL)
		{
			_cipherMode = CryptCipherMode::CBC;
			_padding = CryptPaddingMode::PKCS5;

			//	Create the Crypt context.
			if (!::CryptAcquireContext(&_hCryptProv, NULL, NULL, PROV_RSA_AES, 0))  {
				if (::GetLastError() == NTE_BAD_KEYSET)
				{
					if (!::CryptAcquireContext(&_hCryptProv,
						NULL,
						NULL,
						PROV_RSA_AES,
						CRYPT_NEWKEYSET))
					{
						return;
					}
				}
				else {
					return;
				}
			}

			//	Create an empty hash object.
			if (!::CryptCreateHash(_hCryptProv, CALG_SHA_512, 0, 0, &_hHash))
				return;

			//	Memory files are opened automatically on construction, we don't
			//	explcitly call open.
		}

		/// <summary>
		/// This destructor.
		/// </summary>
		Aes::~Aes()
		{
			//	Close the file.
			_file.Close();

			// Clean up.
			if (_hHash)
				::CryptDestroyHash(_hHash);

			if (_hKey)
				::CryptDestroyKey(_hKey);

			if (_hCryptProv)
				::CryptReleaseContext(_hCryptProv, 0);
		}

		/// <summary>
		/// Sets the cipher mode.
		/// </summary>
		void Aes::setCipherMode(CryptCipherMode cipherMode)
		{
			_cipherMode = cipherMode;

			// Set the default CBC
			DWORD dwMode = CRYPT_MODE_CBC;

			// Select the cipher mode.
			switch (_cipherMode)
			{
			case Nequeo::Cryptography::CFB:
				dwMode = CRYPT_MODE_CFB;
				break;

			case Nequeo::Cryptography::CTS:
				dwMode = CRYPT_MODE_CTS;
				break;

			case Nequeo::Cryptography::ECB:
				dwMode = CRYPT_MODE_ECB;
				break;

			case Nequeo::Cryptography::OFB:
				dwMode = CRYPT_MODE_OFB;
				break;

			default:
			case Nequeo::Cryptography::CBC:
				dwMode = CRYPT_MODE_CBC;
				break;
			}
			
			//	Return failure if we don't have a context or key.
			if (_hCryptProv == NULL || _hKey == NULL)
				return;

			// Attempt to set the cipher mode.
			if (!CryptSetKeyParam(_hKey, KP_MODE, (BYTE*)&dwMode, 0))
			{
				throw exception("Unable to set the cipher mode.");
			}
		}

		/// <summary>
		/// Gets the cipher mode.
		/// </summary>
		CryptCipherMode Aes::getCipherMode()
		{
			DWORD dwCount = sizeof(DWORD);
			DWORD dwMode;

			// Attempt to get the cipher mode.
			if (!CryptGetKeyParam(_hKey, KP_MODE, (PBYTE)&dwMode, &dwCount, 0))
			{
				throw exception("Unable to get the cipher mode.");
			}
			else
			{
				// Select the cipher mode.
				switch (dwMode)
				{
				case CRYPT_MODE_CFB:
					_cipherMode = CryptCipherMode::CFB;
					break;

				case CRYPT_MODE_CTS:
					_cipherMode = CryptCipherMode::CTS;
					break;

				case CRYPT_MODE_ECB:
					_cipherMode = CryptCipherMode::ECB;
					break;

				case CRYPT_MODE_OFB:
					_cipherMode = CryptCipherMode::OFB;
					break;

				default:
				case CRYPT_MODE_CBC:
					_cipherMode = CryptCipherMode::CBC;
					break;

				}
			}

			// Return the cipher mode.
			return _cipherMode;
		}

		/// <summary>
		/// Sets the padding mode.
		/// </summary>
		void Aes::setPaddingMode(CryptPaddingMode paddingMode)
		{
			_padding = paddingMode;

			// Set the default pkcs5.
			DWORD dwPadding = PKCS5_PADDING;

			// Select the padding mode.
			switch (_padding)
			{
			case Nequeo::Cryptography::Zeros:
				dwPadding = ZERO_PADDING;
				break;

			case Nequeo::Cryptography::Random:
				dwPadding = RANDOM_PADDING;
				break;

			default:
			case Nequeo::Cryptography::PKCS5:
				dwPadding = PKCS5_PADDING;
				break;
			}

			//	Return failure if we don't have a context or key.
			if (_hCryptProv == NULL || _hKey == NULL)
				return;

			// Attempt to set the padding mode.
			if (!CryptSetKeyParam(_hKey, KP_PADDING, (BYTE*)&dwPadding, 0))
			{
				throw exception("Unable to set the padding mode.");
			}
		}

		/// <summary>
		/// Gets the padding mode.
		/// </summary>
		CryptPaddingMode Aes::getPaddingMode()
		{
			DWORD dwCount = sizeof(DWORD);
			DWORD dwPadding;

			// Attempt to get the cipher mode.
			if (!CryptGetKeyParam(_hKey, KP_PADDING, (PBYTE)&dwPadding, &dwCount, 0))
			{
				throw exception("Unable to get the padding mode.");
			}
			else
			{
				// Select the padding mode.
				switch (dwPadding)
				{
				case RANDOM_PADDING:
					_padding = CryptPaddingMode::Random;
					break;

				case ZERO_PADDING:
					_padding = CryptPaddingMode::Zeros;
					break;

				default:
				case PKCS5_PADDING:
					_padding = CryptPaddingMode::PKCS5;
					break;
				}
			}

			// Return the padding mode.
			return _padding;
		}

		/// <summary>
		/// Sets the initialization vector.
		/// </summary>
		void Aes::setInitializationVector(BYTE iv[16])
		{
			// Attempt to set the initialization vector.
			if (!CryptSetKeyParam(_hKey, KP_IV, iv, 0))
			{
				throw exception("Unable to set the initialization vector.");
			}
		}

		/// <summary>
		/// Gets the initialization vector.
		/// </summary>
		array<BYTE, 16> Aes::getInitializationVector()
		{
			DWORD dwCount = sizeof(DWORD);
			BYTE iv[16];

			// Attempt to get the initialization vector.
			if (!CryptGetKeyParam(_hKey, KP_IV, iv, &dwCount, 0))
			{
				throw exception("Unable to get the initialization vector.");
			}

			// Assign the vector.
			array<BYTE, 16> aIV;
			for (int i = 0; i < 16; i++)
			{
				// Assign the vector values.
				aIV[i] = iv[i];
			}

			// Return the vector.
			return aIV;
		}

		/// <summary>
		/// Derive a key from a password.
		/// </summary>
		/// <param name="strPassword">The password (must be 32 bytes in length).</param>
		/// <returns>True if derived key; else false.</returns>
		/// <remarks>
		///	These functions are essential to using the crypto object- you must
		///	have a key from some source or other.
		/// </remarks>
		bool Aes::DeriveKey(CString strPassword)
		{
			//	Return failure if we don't have a context or hash.
			if (_hCryptProv == NULL || _hHash == NULL)
				return false;

			//	If we already have a hash, trash it.
			if (_hHash)
			{
				CryptDestroyHash(_hHash);
				_hHash = NULL;
				if (!CryptCreateHash(_hCryptProv, CALG_SHA_512, 0, 0, &_hHash))
					return false;
			}

			//	If we already have a key, destroy it.
			if (_hKey)
			{
				::CryptDestroyKey(_hKey);
				_hKey = NULL;
			}

			//	Hash the password. This will have a different result in UNICODE mode, as it
			//	will hash the UNICODE string (this is by design, allowing for UNICODE passwords, but
			//	it's important to be aware of this behaviour.
			if (!CryptHashData(_hHash, (const BYTE*)strPassword.GetString(), strPassword.GetLength() * sizeof(TCHAR), 0))
				return false;

			//	Create a session key based on the hash of the password.
			if (!CryptDeriveKey(_hCryptProv, CALG_AES_256, _hHash, CRYPT_EXPORTABLE, &_hKey))
				return false;

			//	And we're done.
			return true;
		}

		/// <summary>
		/// Encrypt the serializable object.
		/// </summary>
		/// <param name="serializable">The serializable object that contains the data to encrypt.</param>
		/// <param name="arData">The byte array where the encrypted data will be stored.</param>
		/// <returns>True if encrypted; else false.</returns>
		bool Aes::Encrypt(const CObject& serializable, CByteArray& arData)
		{
			//	Return failure if we don't have a context or key.
			if (_hCryptProv == NULL || _hKey == NULL)
				return false;

			//	Return failure if the object is not serializable.
			if (serializable.IsSerializable() == FALSE)
				return false;

			//	Before we write to the file, trash it.
			_file.SetLength(0);

			//	Create a storing archive from the memory file.
			CArchive ar(&_file, CArchive::store);

			//	We know that serialzing an object will not change it's data, as we can
			//	safely use a const cast here.

			//	Write the data to the memory file.
			const_cast<CObject&>(serializable).Serialize(ar);

			//	Close the archive, flushing the write.
			ar.Close();

			//	Encrypt the contents of the memory file and store the result in the array.
			return InternalEncrypt(arData);
		}

		/// <summary>
		/// Decrypt the encrypted data into the object.
		/// </summary>
		/// <param name="arData">The byte array of encrypted data.</param>
		/// <param name="serializable">The serializable object that will contains the decrypt data.</param>
		/// <returns>True if decrypted; else false.</returns>
		bool Aes::Decrypt(const CByteArray& arData, CObject& serializable)
		{
			//	Return failure if we don't have a context or key.
			if (_hCryptProv == NULL || _hKey == NULL)
				return false;

			//	Return failure if the object is not serializable.
			if (serializable.IsSerializable() == FALSE)
				return false;

			//	Decrypt the contents of the array to the memory file.
			if (InternalDecrypt(arData) == false)
				return false;

			//	Create a reading archive from the memory file.
			CArchive ar(&_file, CArchive::load);

			//	Read the data from the memory file.
			serializable.Serialize(ar);

			//	Close the archive.
			ar.Close();

			//	And we're done.
			return true;
		}

		/// <summary>
		/// Encrypt the string.
		/// </summary>
		/// <param name="str">The string that contains the data to encrypt.</param>
		/// <param name="arData">The byte array where the encrypted data will be stored.</param>
		/// <returns>True if encrypted; else false.</returns>
		bool Aes::Encrypt(const CString& str, CByteArray& arData)
		{
			//	Return failure if we don't have a context or key.
			if (_hCryptProv == NULL || _hKey == NULL)
				return false;

			//	Before we write to the file, trash it.
			_file.SetLength(0);

			//	Create a storing archive from the memory file.
			CArchive ar(&_file, CArchive::store);

			//	Write the string to the memory file.
			ar << str;

			//	Close the archive, flushing the write.
			ar.Close();

			//	Encrypt the contents of the memory file and store the result in the array.
			return InternalEncrypt(arData);
		}

		/// <summary>
		/// Encrypt the decrypted file.
		/// </summary>
		/// <param name="szSource">The decrypted file.</param>
		/// <param name="szDestination">The encrypted file.</param>
		/// <param name="szPassword">The password used to encrypt the file.</param>
		/// <returns>True if encrypted; else false.</returns>
		bool Aes::Encrypt(PCHAR szSource, PCHAR szDestination, PCHAR szPassword)
		{
			FILE *hSource = NULL;
			FILE *hDestination = NULL;
			errno_t err;
			INT eof = 0;

			HCRYPTPROV hProv = 0;
			HCRYPTKEY hKey = 0;
			HCRYPTKEY hXchgKey = 0;
			HCRYPTHASH hHash = 0;

			PBYTE pbKeyBlob = NULL;
			DWORD dwKeyBlobLen;

			PBYTE pbBuffer = NULL;
			DWORD dwBlockLen;
			DWORD dwBufferLen;
			DWORD dwCount;

			bool status = false;

			// Open source file.
			err = fopen_s(&hSource, szSource, "rb");
			if (err != 0) {
				goto done;
			}

			// Open destination file.
			err = fopen_s(&hDestination, szDestination, "wb");
			if (err != 0){
				goto done;
			}

			// Get handle to the CSP. In order to be used with different OSs 
			// with different default provides, the CSP is explicitly set. 
			// If the Microsoft Enhanced Provider is not installed, set parameter
			// three to MS_DEF_PROV 

			if (!CryptAcquireContext(&hProv, NULL, MS_ENHANCED_PROV, PROV_RSA_AES, 0)) {
				goto done;
			}

			if (szPassword == NULL) {
				// Encrypt the file with a random session key.

				// Create a random session key.
				if (!CryptGenKey(hProv, CALG_AES_256, CRYPT_EXPORTABLE, &hKey)) {
					goto done;
				}

				// Get handle to key exchange public key.
				if (!CryptGetUserKey(hProv, AT_KEYEXCHANGE, &hXchgKey)) {
					goto done;
				}

				// Determine size of the key blob and allocate memory.
				if (!CryptExportKey(hKey, hXchgKey, SIMPLEBLOB, 0, NULL, &dwKeyBlobLen)) {
					goto done;
				}
				if ((pbKeyBlob = (unsigned char *)malloc(dwKeyBlobLen)) == NULL) {
					goto done;
				}

				// Export session key into a simple key blob.
				if (!CryptExportKey(hKey, hXchgKey, SIMPLEBLOB, 0, pbKeyBlob, &dwKeyBlobLen)) {
					goto done;
				}

				// Release key exchange key handle.
				CryptDestroyKey(hXchgKey);
				hXchgKey = 0;

				// Write size of key blob to destination file.
				fwrite(&dwKeyBlobLen, sizeof(DWORD), 1, hDestination);
				if (ferror(hDestination)) {
					goto done;
				}

				// Write key blob to destination file.
				fwrite(pbKeyBlob, 1, dwKeyBlobLen, hDestination);
				if (ferror(hDestination)) {
					goto done;
				}

			}
			else {
				// Encrypt the file with a session key derived from a password.

				// Create a hash object.
				if (!CryptCreateHash(hProv, CALG_SHA_512, 0, 0, &hHash)) {
					goto done;
				}

				// Hash in the password data.
				if (!CryptHashData(hHash, (const unsigned char *)szPassword, (DWORD)strlen(szPassword), 0)) {
					goto done;
				}

				// Derive a session key from the hash object.
				if (!CryptDeriveKey(hProv, CALG_AES_256, hHash, CRYPT_EXPORTABLE, &hKey)) {
					goto done;
				}

				// Destroy the hash object.
				CryptDestroyHash(hHash);
				hHash = 0;
			}

			// Determine number of bytes to encrypt at a time. This must be a multiple
			// of ENCRYPT_BLOCK_SIZE.
			dwBlockLen = 1000 - 1000 % 1;

			// Determine the block size. If a block cipher is used this must have
			// room for an extra block.
#ifdef USE_BLOCK_CIPHER
			dwBufferLen = dwBlockLen + ENCRYPT_BLOCK_SIZE;
#else
			dwBufferLen = dwBlockLen;
#endif

			// Allocate memory.
			if ((pbBuffer = (unsigned char *)malloc(dwBufferLen)) == NULL) {
				goto done;
			}

			// Encrypt source file and write to Source file.
			do {
				// Read up to 'dwBlockLen' bytes from source file.
				dwCount = (DWORD)fread(pbBuffer, 1, dwBlockLen, hSource);
				if (ferror(hSource)) {
					goto done;
				}
				eof = feof(hSource);

				// Encrypt data
				if (!CryptEncrypt(hKey, 0, eof, 0, pbBuffer, &dwCount, dwBufferLen)) {
					goto done;
				}

				// Write data to destination file.
				fwrite(pbBuffer, 1, dwCount, hDestination);
				if (ferror(hDestination)) {
					goto done;
				}
			} while (!feof(hSource));

			status = true;

		done:

			// Close files.
			if (hSource) fclose(hSource);
			if (hDestination) fclose(hDestination);

			// Free memory.
			if (pbKeyBlob) free(pbKeyBlob);
			if (pbBuffer) free(pbBuffer);

			// Destroy session key.
			if (hKey) CryptDestroyKey(hKey);

			// Release key exchange key handle.
			if (hXchgKey) CryptDestroyKey(hXchgKey);

			// Destroy hash object.
			if (hHash) CryptDestroyHash(hHash);

			// Release provider handle.
			if (hProv) CryptReleaseContext(hProv, 0);

			return status;
		}

		/// <summary>
		/// Decrypt the encrypted data into the string.
		/// </summary>
		/// <param name="arData">The byte array of encrypted data.</param>
		/// <param name="str">The string that will contains the decrypt data.</param>
		/// <returns>True if decrypted; else false.</returns>
		bool Aes::Decrypt(const CByteArray& arData, CString& str)
		{
			//	Return failure if we don't have a context or key.
			if (_hCryptProv == NULL || _hKey == NULL)
				return false;

			//	Decrypt the contents of the array to the memory file.
			if (InternalDecrypt(arData) == false)
				return false;

			//	Create a reading archive from the memory file.
			CArchive ar(&_file, CArchive::load);

			//	Read the data from the memory file.
			ar >> str;

			//	Close the archive.
			ar.Close();

			//	And we're done.
			return true;
		}

		/// <summary>
		/// Decrypt the encrypted file.
		/// </summary>
		/// <param name="szSource">The encrypted file.</param>
		/// <param name="szDestination">The decrypted file.</param>
		/// <param name="szPassword">The password used to decrypt the file.</param>
		/// <returns>True if decrypted; else false.</returns>
		bool Aes::Decrypt(PCHAR szSource, PCHAR szDestination, PCHAR szPassword)
		{
			FILE *hSource = NULL;
			FILE *hDestination = NULL;
			errno_t err;
			INT eof = 0;

			HCRYPTPROV hProv = 0;
			HCRYPTKEY hKey = 0;
			HCRYPTHASH hHash = 0;

			PBYTE pbKeyBlob = NULL;
			DWORD dwKeyBlobLen;

			PBYTE pbBuffer = NULL;
			DWORD dwBlockLen;
			DWORD dwBufferLen;
			DWORD dwCount;

			bool status = false;

			// Open source file.
			err = fopen_s(&hSource, szSource, "rb");
			if (err != 0) {
				goto done;
			}

			// Open destination file.
			err = fopen_s(&hDestination, szDestination, "wb");
			if (err != 0) {
				goto done;
			}

			// Get handle to the default provider.
			if (!CryptAcquireContext(&hProv, NULL, MS_ENHANCED_PROV, PROV_RSA_AES, 0)) {
				goto done;
			}

			if (szPassword == NULL) {
				// Decrypt the file with the saved session key.

				// Read key blob length from source file and allocate memory.
				fread(&dwKeyBlobLen, sizeof(DWORD), 1, hSource);
				if (ferror(hSource) || feof(hSource)) {
					goto done;
				}
				if ((pbKeyBlob == malloc(dwKeyBlobLen)) == NULL) {
					goto done;
				}

				// Read key blob from source file.
				fread(pbKeyBlob, 1, dwKeyBlobLen, hSource);
				if (ferror(hSource) || feof(hSource)) {
					goto done;
				}

				// Import key blob into CSP.
				if (!CryptImportKey(hProv, pbKeyBlob, dwKeyBlobLen, 0, 0, &hKey)) {
					goto done;
				}
			}
			else {
				// Decrypt the file with a session key derived from a password.

				// Create a hash object.
				if (!CryptCreateHash(hProv, CALG_SHA_512, 0, 0, &hHash)) {
					goto done;
				}

				// Hash in the password data.
				if (!CryptHashData(hHash, (PBYTE)szPassword, (DWORD)strlen(szPassword), 0)) {
					goto done;
				}

				// Derive a session key from the hash object.
				if (!CryptDeriveKey(hProv, CALG_AES_256, hHash, CRYPT_EXPORTABLE, &hKey)) {
					goto done;
				}

				// Destroy the hash object.
				CryptDestroyHash(hHash);
				hHash = 0;
			}

			// Determine number of bytes to decrypt at a time. This must be a multiple
			// of ENCRYPT_BLOCK_SIZE.
			dwBlockLen = 1000 - 1000 % 1;
			dwBufferLen = dwBlockLen;

			// Allocate memory.
			if ((pbBuffer == malloc(dwBufferLen)) == NULL) {
				goto done;
			}

			// Decrypt source file and write to destination file.
			do {
				// Read up to 'dwBlockLen' bytes from source file.
				dwCount = (DWORD)fread(pbBuffer, 1, dwBlockLen, hSource);
				if (ferror(hSource)) {
					goto done;
				}
				eof = feof(hSource);

				// Decrypt data
				if (!CryptDecrypt(hKey, 0, eof, 0, pbBuffer, &dwCount)) {
					goto done;
				}

				// Write data to destination file.
				fwrite(pbBuffer, 1, dwCount, hDestination);
				if (ferror(hDestination)) {
					goto done;
				}
			} while (!feof(hSource));

			status = true;

		done:

			// Close files.
			if (hSource) fclose(hSource);
			if (hDestination) fclose(hDestination);

			// Free memory.
			if (pbKeyBlob) free(pbKeyBlob);
			if (pbBuffer) free(pbBuffer);

			// Destroy session key.
			if (hKey) CryptDestroyKey(hKey);

			// Destroy hash object.
			if (hHash) CryptDestroyHash(hHash);

			// Release provider handle.
			if (hProv) CryptReleaseContext(hProv, 0);

			return status;
		}

		/// <summary>
		/// Encrypt the contents of the memory file and store in the passed array.
		/// </summary>
		/// <param name="arDestination">The array of bytes to encrypt.</param>
		/// <returns>True if encrypted; else false.</returns>
		bool Aes::InternalEncrypt(CByteArray& arDestination)
		{
			//	Get the length of the data in memory. Increase the capacity to handle the size of the encrypted data.
			ULONGLONG uLength = _file.GetLength();
			ULONGLONG uCapacity = uLength * 2;
			_file.SetLength(uCapacity);

			//	Acquire direct access to the memory.
			BYTE* pData = _file.Detach();

			//	We need a DWORD to tell encrypt how much data we're encrypting.
			DWORD dwDataLength = static_cast<DWORD>(uLength);

			//	Now encrypt the memory file.
			if (!::CryptEncrypt(_hKey, NULL, TRUE, 0, pData, &dwDataLength, static_cast<DWORD>(uCapacity)))
			{
				//	Free the memory we release from the memory file.
				delete[] pData;

				return false;
			}

			//	Assign all of the data we have encrypted to the byte array- make sure anything 
			//	already in the array is trashed first.
			arDestination.RemoveAll();
			arDestination.SetSize(static_cast<INT_PTR>(dwDataLength));
			memcpy(arDestination.GetData(), pData, dwDataLength);

			//	Free the memory we release from the memory file.
			delete[] pData;

			return true;
		}

		/// <summary>
		/// Decrypt the contents of the passed array and store in the memory file.
		/// </summary>
		/// <param name="arSource">The array of bytes to decrypt.</param>
		/// <returns>True if decrypted; else false.</returns>
		bool Aes::InternalDecrypt(const CByteArray& arSource)
		{
			//	Trash the file.
			_file.SetLength(0);

			//	Write the contents of the byte array to the memory file.
			_file.Write(arSource.GetData(), static_cast<UINT>(arSource.GetCount()));
			_file.Flush();

			//	Acquire direct access to the memory file buffer.
			BYTE* pData = _file.Detach();

			//	We need a DWORD to tell decrpyt how much data we're encrypting.
			DWORD dwDataLength = static_cast<DWORD>(arSource.GetCount());
			DWORD dwOldDataLength = dwDataLength;

			//	Now decrypt the data.
			if (!::CryptDecrypt(_hKey, NULL, TRUE, 0, pData, &dwDataLength))
			{
				//	Free the memory we release from the memory file.
				delete[] pData;

				return false;
			}

			//	Set the length of the data file, write the decrypted data to it.
			_file.SetLength(dwDataLength);
			_file.Write(pData, dwDataLength);
			_file.Flush();
			_file.SeekToBegin();

			//	Free the memory we release from the memory file.
			delete[] pData;

			return true;
		}
	}
}