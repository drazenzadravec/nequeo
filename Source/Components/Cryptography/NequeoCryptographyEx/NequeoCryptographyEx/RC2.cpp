/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          RC2.cpp
*  Purpose :       RC2 class.
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

#include "RC2.h"

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		///	All of the Cryptography API initialisation is done in the 
		///	constructor, so constructing this object is expensive.
		/// </summary>
		RC2::RC2() : _hCryptProv(NULL), _hKey(NULL), _hHash(NULL)
		{
			//	Create the Crypt context.
			if (!::CryptAcquireContext(&_hCryptProv, NULL, NULL, PROV_RSA_FULL, 0))  {
				if (::GetLastError() == NTE_BAD_KEYSET)
				{
					if (!::CryptAcquireContext(&_hCryptProv,
						NULL,
						NULL,
						PROV_RSA_FULL,
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
			if (!::CryptCreateHash(_hCryptProv, CALG_SHA, 0, 0, &_hHash))
				return;

			//	Memory files are opened automatically on construction, we don't
			//	explcitly call open.
		}

		/// <summary>
		/// This destructor.
		/// </summary>
		RC2::~RC2()
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
		/// Derive a key from a password.
		/// </summary>
		/// <param name="strPassword">The password.</param>
		/// <returns>True if derived key; else false.</returns>
		/// <remarks>
		///	These functions are essential to using the crypto object- you must
		///	have a key from some source or other.
		/// </remarks>
		bool RC2::DeriveKey(CString strPassword)
		{
			//	Return failure if we don't have a context or hash.
			if (_hCryptProv == NULL || _hHash == NULL)
				return false;

			//	If we already have a hash, trash it.
			if (_hHash)
			{
				CryptDestroyHash(_hHash);
				_hHash = NULL;
				if (!CryptCreateHash(_hCryptProv, CALG_SHA, 0, 0, &_hHash))
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
			if (!CryptDeriveKey(_hCryptProv, CALG_RC2, _hHash, CRYPT_EXPORTABLE, &_hKey))
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
		bool RC2::Encrypt(const CObject& serializable, CByteArray& arData)
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
		bool RC2::Decrypt(const CByteArray& arData, CObject& serializable)
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
		bool RC2::Encrypt(const CString& str, CByteArray& arData)
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
		/// Decrypt the encrypted data into the string.
		/// </summary>
		/// <param name="arData">The byte array of encrypted data.</param>
		/// <param name="str">The string that will contains the decrypt data.</param>
		/// <returns>True if decrypted; else false.</returns>
		bool RC2::Decrypt(const CByteArray& arData, CString& str)
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
		/// Encrypt the contents of the memory file and store in the passed array.
		/// </summary>
		/// <param name="arDestination">The array of bytes to encrypt.</param>
		/// <returns>True if encrypted; else false.</returns>
		bool RC2::InternalEncrypt(CByteArray& arDestination)
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
		bool RC2::InternalDecrypt(const CByteArray& arSource)
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
