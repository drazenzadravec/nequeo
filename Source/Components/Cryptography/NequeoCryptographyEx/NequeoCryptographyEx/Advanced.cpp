/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          Advanced.cpp
*  Purpose :       Advanced class.
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

#include "Advanced.h"

#include <bcrypt.h>

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		///	Advanced cryptography using cryptography next generation (CNG).
		/// </summary>
		AdvancedCNG::AdvancedCNG()
		{
		}

		/// <summary>
		/// This destructor.
		/// </summary>
		AdvancedCNG::~AdvancedCNG()
		{
		}

		/// <summary>
		/// Create a hash code.
		/// </summary>
		/// <param name="data">The data to hash.</param>
		/// <returns>The resulting hash code.</returns>
		vector<BYTE> AdvancedCNG::CreateHashCode(BYTE data[])
		{
			BCRYPT_ALG_HANDLE       hAlg = NULL;
			BCRYPT_HASH_HANDLE      hHash = NULL;
			NTSTATUS                status = STATUS_UNSUCCESSFUL;
			DWORD                   cbData = 0,
									cbHash = 0,
									cbHashObject = 0;
			PBYTE                   pbHashObject = NULL;
			PBYTE                   hashCode = NULL;
			vector<BYTE>			result;

			// Open an algorithm handle
			if (!NT_SUCCESS(status = BCryptOpenAlgorithmProvider(
				&hAlg,
				BCRYPT_SHA512_ALGORITHM,
				NULL,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptOpenAlgorithmProvider\n", status);
				goto Cleanup;
			}

			// Calculate the size of the buffer to hold the hash object
			if (!NT_SUCCESS(status = BCryptGetProperty(
				hAlg,
				BCRYPT_OBJECT_LENGTH,
				(PBYTE)&cbHashObject,
				sizeof(DWORD),
				&cbData,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptGetProperty\n", status);
				goto Cleanup;
			}

			// allocate the hash object on the heap
			pbHashObject = (PBYTE)HeapAlloc(GetProcessHeap(), 0, cbHashObject);
			if (NULL == pbHashObject)
			{
				// L"**** memory allocation failed\n");
				goto Cleanup;
			}

			// calculate the length of the hash
			if (!NT_SUCCESS(status = BCryptGetProperty(
				hAlg,
				BCRYPT_HASH_LENGTH,
				(PBYTE)&cbHash,
				sizeof(DWORD),
				&cbData,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptGetProperty\n", status);
				goto Cleanup;
			}

			// allocate the hash buffer on the heap
			hashCode = (PBYTE)HeapAlloc(GetProcessHeap(), 0, cbHash);
			if (NULL == hashCode)
			{
				// L"**** memory allocation failed\n");
				goto Cleanup;
			}

			// create a hash
			if (!NT_SUCCESS(status = BCryptCreateHash(
				hAlg,
				&hHash,
				pbHashObject,
				cbHashObject,
				NULL,
				0,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptCreateHash\n", status);
				goto Cleanup;
			}

			// hash some data
			if (!NT_SUCCESS(status = BCryptHashData(
				hHash,
				(PBYTE)data,
				sizeof(data),
				0)))
			{
				// L"**** Error 0x%x returned by BCryptHashData\n", status);
				goto Cleanup;
			}

			// close the hash
			if (!NT_SUCCESS(status = BCryptFinishHash(
				hHash,
				hashCode,
				cbHash,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptFinishHash\n", status);
				goto Cleanup;
			}

			// Add the bytes to the vector.
			for (unsigned long i = 0; i < cbHash; i++)
			{
				result.push_back(hashCode[i]);
			}

		Cleanup:

			if (hAlg)
			{
				BCryptCloseAlgorithmProvider(hAlg, 0);
			}

			if (hHash)
			{
				BCryptDestroyHash(hHash);
			}

			if (pbHashObject)
			{
				HeapFree(GetProcessHeap(), 0, pbHashObject);
			}

			if (hashCode)
			{
				// Release the hash code.
				HeapFree(GetProcessHeap(), 0, hashCode);
			}

			// Success.
			return result;
		}

		/// <summary>
		/// Create the signature for the data.
		/// </summary>
		/// <param name="data">The data to sign.</param>
		/// <returns>The resulting signature.</returns>
		vector<BYTE> AdvancedCNG::SignData(BYTE data[])
		{
			NCRYPT_PROV_HANDLE      hProv = NULL;
			NCRYPT_KEY_HANDLE       hKey = NULL;
			BCRYPT_KEY_HANDLE       hTmpKey = NULL;
			SECURITY_STATUS         secStatus = ERROR_SUCCESS;
			BCRYPT_ALG_HANDLE       hHashAlg = NULL, 
									hSignAlg = NULL;
			BCRYPT_HASH_HANDLE      hHash = NULL;
			NTSTATUS                status = STATUS_UNSUCCESSFUL;
			DWORD                   cbData = 0,
									cbHash = 0,
									cbBlob = 0,
									cbSignature = 0,
									cbHashObject = 0;
			PBYTE                   pbHashObject = NULL;
			PBYTE                   pbHash = NULL,
									pbBlob = NULL;
			PBYTE                   signedData = NULL;
			vector<BYTE>			result;

			// open an algorithm handle
			if (!NT_SUCCESS(status = BCryptOpenAlgorithmProvider(
				&hHashAlg,
				BCRYPT_SHA512_ALGORITHM,
				NULL,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptOpenAlgorithmProvider\n", status);
				goto Cleanup;
			}

			if (!NT_SUCCESS(status = BCryptOpenAlgorithmProvider(
				&hSignAlg,
				BCRYPT_ECDSA_P521_ALGORITHM,
				NULL,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptOpenAlgorithmProvider\n", status);
				goto Cleanup;
			}

			// calculate the size of the buffer to hold the hash object
			if (!NT_SUCCESS(status = BCryptGetProperty(
				hHashAlg,
				BCRYPT_OBJECT_LENGTH,
				(PBYTE)&cbHashObject,
				sizeof(DWORD),
				&cbData,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptGetProperty\n", status);
				goto Cleanup;
			}

			// allocate the hash object on the heap
			pbHashObject = (PBYTE)HeapAlloc(GetProcessHeap(), 0, cbHashObject);
			if (NULL == pbHashObject)
			{
				// L"**** memory allocation failed\n");
				goto Cleanup;
			}

			// calculate the length of the hash
			if (!NT_SUCCESS(status = BCryptGetProperty(
				hHashAlg,
				BCRYPT_HASH_LENGTH,
				(PBYTE)&cbHash,
				sizeof(DWORD),
				&cbData,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptGetProperty\n", status);
				goto Cleanup;
			}

			// allocate the hash buffer on the heap
			pbHash = (PBYTE)HeapAlloc(GetProcessHeap(), 0, cbHash);
			if (NULL == pbHash)
			{
				// L"**** memory allocation failed\n");
				goto Cleanup;
			}

			// create a hash
			if (!NT_SUCCESS(status = BCryptCreateHash(
				hHashAlg,
				&hHash,
				pbHashObject,
				cbHashObject,
				NULL,
				0,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptCreateHash\n", status);
				goto Cleanup;
			}

			// hash some data
			if (!NT_SUCCESS(status = BCryptHashData(
				hHash,
				(PBYTE)data,
				sizeof(data),
				0)))
			{
				// L"**** Error 0x%x returned by BCryptHashData\n", status);
				goto Cleanup;
			}

			// close the hash
			if (!NT_SUCCESS(status = BCryptFinishHash(
				hHash,
				pbHash,
				cbHash,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptFinishHash\n", status);
				goto Cleanup;
			}

			// open handle to KSP
			if (FAILED(secStatus = NCryptOpenStorageProvider(
				&hProv,
				MS_KEY_STORAGE_PROVIDER,
				0)))
			{
				// L"**** Error 0x%x returned by NCryptOpenStorageProvider\n", secStatus);
				goto Cleanup;
			}

			// create a persisted key
			if (FAILED(secStatus = NCryptCreatePersistedKey(
				hProv,
				&hKey,
				NCRYPT_ECDSA_P521_ALGORITHM,
				L"my ecc key",
				0,
				0)))
			{
				// L"**** Error 0x%x returned by NCryptCreatePersistedKey\n", secStatus);
				goto Cleanup;
			}

			// create key on disk
			if (FAILED(secStatus = NCryptFinalizeKey(hKey, 0)))
			{
				// L"**** Error 0x%x returned by NCryptFinalizeKey\n", secStatus);
				goto Cleanup;
			}

			// sign the hash
			if (FAILED(secStatus = NCryptSignHash(
				hKey,
				NULL,
				pbHash,
				cbHash,
				NULL,
				0,
				&cbSignature,
				0)))
			{
				// L"**** Error 0x%x returned by NCryptSignHash\n", secStatus);
				goto Cleanup;
			}


			// allocate the signature buffer
			signedData = (PBYTE)HeapAlloc(GetProcessHeap(), 0, cbSignature);
			if (NULL == signedData)
			{
				// L"**** memory allocation failed\n");
				goto Cleanup;
			}

			if (FAILED(secStatus = NCryptSignHash(
				hKey,
				NULL,
				pbHash,
				cbHash,
				signedData,
				cbSignature,
				&cbSignature,
				0)))
			{
				// L"**** Error 0x%x returned by NCryptSignHash\n", secStatus);
				goto Cleanup;
			}

			if (FAILED(secStatus = NCryptExportKey(
				hKey,
				NULL,
				BCRYPT_ECCPUBLIC_BLOB,
				NULL,
				NULL,
				0,
				&cbBlob,
				0)))
			{
				// L"**** Error 0x%x returned by NCryptExportKey\n", secStatus);
				goto Cleanup;
			}

			pbBlob = (PBYTE)HeapAlloc(GetProcessHeap(), 0, cbBlob);
			if (NULL == pbBlob)
			{
				// L"**** memory allocation failed\n");
				goto Cleanup;
			}

			if (FAILED(secStatus = NCryptExportKey(
				hKey,
				NULL,
				BCRYPT_ECCPUBLIC_BLOB,
				NULL,
				pbBlob,
				cbBlob,
				&cbBlob,
				0)))
			{
				// L"**** Error 0x%x returned by NCryptExportKey\n", secStatus);
				goto Cleanup;
			}

			if (!NT_SUCCESS(status = BCryptImportKeyPair(
				hSignAlg,
				NULL,
				BCRYPT_ECCPUBLIC_BLOB,
				&hTmpKey,
				pbBlob,
				cbBlob,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptImportKeyPair\n", status);
				goto Cleanup;
			}

			if (!NT_SUCCESS(status = BCryptVerifySignature(
				hTmpKey,
				NULL,
				pbHash,
				cbHash,
				signedData,
				cbSignature,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptVerifySignature\n", status);
				goto Cleanup;
			}

			// Add the bytes to the vector.
			for (unsigned long i = 0; i < cbSignature; i++)
			{
				result.push_back(signedData[i]);
			}

		Cleanup:

			if (hHashAlg)
			{
				BCryptCloseAlgorithmProvider(hHashAlg, 0);
			}

			if (hSignAlg)
			{
				BCryptCloseAlgorithmProvider(hSignAlg, 0);
			}

			if (hHash)
			{
				BCryptDestroyHash(hHash);
			}

			if (pbHashObject)
			{
				HeapFree(GetProcessHeap(), 0, pbHashObject);
			}

			if (pbHash)
			{
				HeapFree(GetProcessHeap(), 0, pbHash);
			}

			if (pbBlob)
			{
				HeapFree(GetProcessHeap(), 0, pbBlob);
			}

			if (hTmpKey)
			{
				BCryptDestroyKey(hTmpKey);
			}

			if (hKey)
			{
				NCryptDeleteKey(hKey, 0);
			}

			if (hProv)
			{
				NCryptFreeObject(hProv);
			}

			if (signedData)
			{
				// Release the signature.
				HeapFree(GetProcessHeap(), 0, signedData);
			}

			// Success.
			return result;
		}

		/// <summary>
		/// Encrypt the data.
		/// </summary>
		/// <param name="data">The data to encrypt.</param>
		/// <param name="key">The key used to encrypt the data.</param>
		/// <param name="iv">The vector used in encryption.</param>
		/// <returns>The resulting encryption.</returns>
		vector<BYTE> AdvancedCNG::Encrypt(BYTE data[], BYTE key[], BYTE iv[])
		{
			BCRYPT_ALG_HANDLE       hAesAlg = NULL;
			BCRYPT_KEY_HANDLE       hKey = NULL;
			NTSTATUS                status = STATUS_UNSUCCESSFUL;
			DWORD                   cbCipherText = 0,
									cbPlainText = 0,
									cbData = 0,
									cbKeyObject = 0,
									cbBlockLen = 0,
									cbBlob = 0;
			PBYTE                   pbPlainText = NULL,
									pbKeyObject = NULL,
									pbIV = NULL,
									pbBlob = NULL;
			PBYTE                   encryptData = NULL;
			vector<BYTE>			result;

			// Open an algorithm handle.
			if (!NT_SUCCESS(status = BCryptOpenAlgorithmProvider(
				&hAesAlg,
				BCRYPT_AES_ALGORITHM,
				NULL,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptOpenAlgorithmProvider\n", status);
				goto Cleanup;
			}

			// Calculate the size of the buffer to hold the KeyObject.
			if (!NT_SUCCESS(status = BCryptGetProperty(
				hAesAlg,
				BCRYPT_OBJECT_LENGTH,
				(PBYTE)&cbKeyObject,
				sizeof(DWORD),
				&cbData,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptGetProperty\n", status);
				goto Cleanup;
			}

			// Allocate the key object on the heap.
			pbKeyObject = (PBYTE)HeapAlloc(GetProcessHeap(), 0, cbKeyObject);
			if (NULL == pbKeyObject)
			{
				// L"**** memory allocation failed\n");
				goto Cleanup;
			}

			// Calculate the block length for the IV.
			if (!NT_SUCCESS(status = BCryptGetProperty(
				hAesAlg,
				BCRYPT_BLOCK_LENGTH,
				(PBYTE)&cbBlockLen,
				sizeof(DWORD),
				&cbData,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptGetProperty\n", status);
				goto Cleanup;
			}

			// Determine whether the cbBlockLen is not longer than the IV length.
			if (cbBlockLen > sizeof(iv))
			{
				// L"**** block length is longer than the provided IV length\n");
				goto Cleanup;
			}

			// Allocate a buffer for the IV. The buffer is consumed during the 
			// encrypt/decrypt process.
			pbIV = (PBYTE)HeapAlloc(GetProcessHeap(), 0, cbBlockLen);
			if (NULL == pbIV)
			{
				// L"**** memory allocation failed\n");
				goto Cleanup;
			}

			memcpy(pbIV, iv, cbBlockLen);

			if (!NT_SUCCESS(status = BCryptSetProperty(
				hAesAlg,
				BCRYPT_CHAINING_MODE,
				(PBYTE)BCRYPT_CHAIN_MODE_CBC,
				sizeof(BCRYPT_CHAIN_MODE_CBC),
				0)))
			{
				// L"**** Error 0x%x returned by BCryptSetProperty\n", status);
				goto Cleanup;
			}

			// Generate the key from supplied input key bytes.
			if (!NT_SUCCESS(status = BCryptGenerateSymmetricKey(
				hAesAlg,
				&hKey,
				pbKeyObject,
				cbKeyObject,
				(PBYTE)key,
				sizeof(key),
				0)))
			{
				// L"**** Error 0x%x returned by BCryptGenerateSymmetricKey\n", status);
				goto Cleanup;
			}

			// Save another copy of the key for later.
			if (!NT_SUCCESS(status = BCryptExportKey(
				hKey,
				NULL,
				BCRYPT_OPAQUE_KEY_BLOB,
				NULL,
				0,
				&cbBlob,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptExportKey\n", status);
				goto Cleanup;
			}

			// Allocate the buffer to hold the BLOB.
			pbBlob = (PBYTE)HeapAlloc(GetProcessHeap(), 0, cbBlob);
			if (NULL == pbBlob)
			{
				// L"**** memory allocation failed\n");
				goto Cleanup;
			}

			if (!NT_SUCCESS(status = BCryptExportKey(
				hKey,
				NULL,
				BCRYPT_OPAQUE_KEY_BLOB,
				pbBlob,
				cbBlob,
				&cbBlob,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptExportKey\n", status);
				goto Cleanup;
			}

			cbPlainText = sizeof(data);
			pbPlainText = (PBYTE)HeapAlloc(GetProcessHeap(), 0, cbPlainText);
			if (NULL == pbPlainText)
			{
				// L"**** memory allocation failed\n");
				goto Cleanup;
			}

			memcpy(pbPlainText, data, sizeof(data));

			// Get the output buffer size.
			if (!NT_SUCCESS(status = BCryptEncrypt(
				hKey,
				pbPlainText,
				cbPlainText,
				NULL,
				pbIV,
				cbBlockLen,
				NULL,
				0,
				&cbCipherText,
				BCRYPT_BLOCK_PADDING)))
			{
				// L"**** Error 0x%x returned by BCryptEncrypt\n", status);
				goto Cleanup;
			}

			encryptData = (PBYTE)HeapAlloc(GetProcessHeap(), 0, cbCipherText);
			if (NULL == encryptData)
			{
				// L"**** memory allocation failed\n");
				goto Cleanup;
			}

			// Use the key to encrypt the plaintext buffer.
			// For block sized messages, block padding will add an extra block.
			if (!NT_SUCCESS(status = BCryptEncrypt(
				hKey,
				pbPlainText,
				cbPlainText,
				NULL,
				pbIV,
				cbBlockLen,
				encryptData,
				cbCipherText,
				&cbData,
				BCRYPT_BLOCK_PADDING)))
			{
				// L"**** Error 0x%x returned by BCryptEncrypt\n", status);
				goto Cleanup;
			}

			// Add the bytes to the vector.
			for (unsigned long i = 0; i < cbCipherText; i++)
			{
				result.push_back(encryptData[i]);
			}

		Cleanup:

			if (hAesAlg)
			{
				BCryptCloseAlgorithmProvider(hAesAlg, 0);
			}

			if (hKey)
			{
				BCryptDestroyKey(hKey);
			}

			if (pbPlainText)
			{
				HeapFree(GetProcessHeap(), 0, pbPlainText);
			}

			if (pbKeyObject)
			{
				HeapFree(GetProcessHeap(), 0, pbKeyObject);
			}

			if (pbIV)
			{
				HeapFree(GetProcessHeap(), 0, pbIV);
			}

			if (encryptData)
			{
				// Release the encrypted data.
				HeapFree(GetProcessHeap(), 0, encryptData);
			}

			// Success.
			return result;
		}

		/// <summary>
		/// Decrypt the data.
		/// </summary>
		/// <param name="data">The data to decrypt.</param>
		/// <param name="key">The key used to decrypt the data.</param>
		/// <param name="iv">The vector used in decryption.</param>
		/// <returns>The resulting decryption.</returns>
		vector<BYTE> AdvancedCNG::Decrypt(BYTE data[], BYTE key[], BYTE iv[])
		{
			BCRYPT_ALG_HANDLE       hAesAlg = NULL;
			BCRYPT_KEY_HANDLE       hKey = NULL;
			NTSTATUS                status = STATUS_UNSUCCESSFUL;
			DWORD					cbPlainText = 0,
									cbData = 0,
									cbKeyObject = 0,
									cbBlockLen = 0,
									cbBlob = 0;
			PBYTE                   pbKeyObject = NULL,
									pbIV = NULL,
									pbBlob = NULL;
			DWORD                   cbCipherText = sizeof(data);
			PBYTE                   decryptData = NULL;
			vector<BYTE>			result;

			// Open an algorithm handle.
			if (!NT_SUCCESS(status = BCryptOpenAlgorithmProvider(
				&hAesAlg,
				BCRYPT_AES_ALGORITHM,
				NULL,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptOpenAlgorithmProvider\n", status);
				goto Cleanup;
			}

			// Calculate the size of the buffer to hold the KeyObject.
			if (!NT_SUCCESS(status = BCryptGetProperty(
				hAesAlg,
				BCRYPT_OBJECT_LENGTH,
				(PBYTE)&cbKeyObject,
				sizeof(DWORD),
				&cbData,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptGetProperty\n", status);
				goto Cleanup;
			}

			// Allocate the key object on the heap.
			pbKeyObject = (PBYTE)HeapAlloc(GetProcessHeap(), 0, cbKeyObject);
			if (NULL == pbKeyObject)
			{
				// L"**** memory allocation failed\n");
				goto Cleanup;
			}

			// Calculate the block length for the IV.
			if (!NT_SUCCESS(status = BCryptGetProperty(
				hAesAlg,
				BCRYPT_BLOCK_LENGTH,
				(PBYTE)&cbBlockLen,
				sizeof(DWORD),
				&cbData,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptGetProperty\n", status);
				goto Cleanup;
			}

			// Determine whether the cbBlockLen is not longer than the IV length.
			if (cbBlockLen > sizeof(iv))
			{
				// L"**** block length is longer than the provided IV length\n");
				goto Cleanup;
			}

			// Allocate a buffer for the IV. The buffer is consumed during the 
			// encrypt/decrypt process.
			pbIV = (PBYTE)HeapAlloc(GetProcessHeap(), 0, cbBlockLen);
			if (NULL == pbIV)
			{
				// L"**** memory allocation failed\n");
				goto Cleanup;
			}

			memcpy(pbIV, iv, cbBlockLen);

			if (!NT_SUCCESS(status = BCryptSetProperty(
				hAesAlg,
				BCRYPT_CHAINING_MODE,
				(PBYTE)BCRYPT_CHAIN_MODE_CBC,
				sizeof(BCRYPT_CHAIN_MODE_CBC),
				0)))
			{
				// L"**** Error 0x%x returned by BCryptSetProperty\n", status);
				goto Cleanup;
			}

			// Generate the key from supplied input key bytes.
			if (!NT_SUCCESS(status = BCryptGenerateSymmetricKey(
				hAesAlg,
				&hKey,
				pbKeyObject,
				cbKeyObject,
				(PBYTE)key,
				sizeof(key),
				0)))
			{
				// L"**** Error 0x%x returned by BCryptGenerateSymmetricKey\n", status);
				goto Cleanup;
			}

			// Save another copy of the key for later.
			if (!NT_SUCCESS(status = BCryptExportKey(
				hKey,
				NULL,
				BCRYPT_OPAQUE_KEY_BLOB,
				NULL,
				0,
				&cbBlob,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptExportKey\n", status);
				goto Cleanup;
			}

			// Allocate the buffer to hold the BLOB.
			pbBlob = (PBYTE)HeapAlloc(GetProcessHeap(), 0, cbBlob);
			if (NULL == pbBlob)
			{
				// L"**** memory allocation failed\n");
				goto Cleanup;
			}

			if (!NT_SUCCESS(status = BCryptExportKey(
				hKey,
				NULL,
				BCRYPT_OPAQUE_KEY_BLOB,
				pbBlob,
				cbBlob,
				&cbBlob,
				0)))
			{
				// L"**** Error 0x%x returned by BCryptExportKey\n", status);
				goto Cleanup;
			}

			// Get the output buffer size.
			if (!NT_SUCCESS(status = BCryptDecrypt(
				hKey,
				data,
				cbCipherText,
				NULL,
				pbIV,
				cbBlockLen,
				NULL,
				0,
				&cbPlainText,
				BCRYPT_BLOCK_PADDING)))
			{
				// L"**** Error 0x%x returned by BCryptDecrypt\n", status);
				goto Cleanup;
			}

			decryptData = (PBYTE)HeapAlloc(GetProcessHeap(), 0, cbPlainText);
			if (NULL == decryptData)
			{
				// L"**** memory allocation failed\n");
				goto Cleanup;
			}

			if (!NT_SUCCESS(status = BCryptDecrypt(
				hKey,
				data,
				cbCipherText,
				NULL,
				pbIV,
				cbBlockLen,
				decryptData,
				cbPlainText,
				&cbPlainText,
				BCRYPT_BLOCK_PADDING)))
			{
				// L"**** Error 0x%x returned by BCryptDecrypt\n", status);
				goto Cleanup;
			}

			// Add the bytes to the vector.
			for (unsigned long i = 0; i < cbPlainText; i++)
			{
				result.push_back(decryptData[i]);
			}

		Cleanup:

			if (hAesAlg)
			{
				BCryptCloseAlgorithmProvider(hAesAlg, 0);
			}

			if (hKey)
			{
				BCryptDestroyKey(hKey);
			}

			if (pbKeyObject)
			{
				HeapFree(GetProcessHeap(), 0, pbKeyObject);
			}

			if (pbIV)
			{
				HeapFree(GetProcessHeap(), 0, pbIV);
			}

			if (decryptData)
			{
				// Release the decrypted data.
				HeapFree(GetProcessHeap(), 0, decryptData);
			}

			// Success.
			return result;
		}
	}
}