/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          AdvancedAES.cpp
 *  Purpose :       
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

#include "Conversion.h"
#include "CHashcode.h"
#include "AdvancedAES.h"

///	<summary>
///	Construct the AES.
///	</summary>
Nequeo::Cryptography::AdvancedAES::AdvancedAES() : m_disposed(false)
{
	// Initialise vectors.
	InitialiseVectors();
}

///	<summary>
///	Deconstruct the AES.
///	</summary>
Nequeo::Cryptography::AdvancedAES::~AdvancedAES()
{
	// If not disposed.
    if (!m_disposed)
    {
        m_disposed = true;

		if(m_internalKey != nullptr)
			delete[] m_internalKey;

		if(m_internalIV != nullptr)
			delete[] m_internalIV;
    }
}

/// <summary>
/// Initialise vectors.
/// </summary>
void Nequeo::Cryptography::AdvancedAES::InitialiseVectors()
{
	// The cryptography key.
	m_internalKey = gcnew array<Byte> { 
		23, 80, 90, 34, 200, 215, 167, 97, 
		132, 104, 67, 34, 99, 235, 240, 57, 
		25, 79, 67, 113, 147, 156, 167, 251,
		20, 39, 69, 125, 149, 214, 202, 53};

	// The initializations vector.
	m_internalIV = gcnew array<Byte> { 
		18, 43, 63, 126, 169, 214, 232, 47,
		19, 41, 70, 127, 159, 224, 222, 58};
}

/// <summary>
/// This method will encrypt a decrypted string.
/// </summary>
/// <param name="value">The string of decrypted data.</param>
/// <returns>The array of encrypted data.</returns>
String^ Nequeo::Cryptography::AdvancedAES::Encrypt(String^ value)
{
	MemoryStream^ memoryStream = nullptr;
    CryptoStream^ cryptoStream = nullptr;
	ASCIIEncoding^ byteEncoding = nullptr;
	AesCryptoServiceProvider^ provider = nullptr;

	try	
	{
		// Will contain the encrypted data
        // from the memory stream.
        array<Byte>^ memEncryptedData = nullptr;

		// Convert the passed string to a byte array.
		ASCIIEncoding^ byteEncoding = gcnew ASCIIEncoding();
        array<Byte>^ encryptedData = byteEncoding->GetBytes(value);

		// Create a new MemoryStream using the passed 
        // array of encrypted data.
        memoryStream = gcnew MemoryStream();

		// Create a new AES provider.
        provider = gcnew AesCryptoServiceProvider();
        provider->Mode = System::Security::Cryptography::CipherMode::CBC;
        provider->Padding = System::Security::Cryptography::PaddingMode::Zeros;
        provider->BlockSize = 128;
        provider->KeySize = 256;

		// Create a CryptoStream using the MemoryStream 
        // and the passed key and initialization vector (IV).
        cryptoStream = gcnew CryptoStream(memoryStream, provider->CreateEncryptor(this->m_internalKey, this->m_internalIV), 
			System::Security::Cryptography::CryptoStreamMode::Write);

		// Write the byte array to the 
        // crypto stream and flush it.
        cryptoStream->Write(encryptedData, 0, encryptedData->Length);
        cryptoStream->FlushFinalBlock();

		// Close the CryptoStream object 
        // release resources.
        cryptoStream->Close();

		// Get an array of bytes from the MemoryStream 
        // that holds the encrypted data.
        memEncryptedData = memoryStream->ToArray();

		// Close the MemoryStream object 
        // release resources.
        memoryStream->Close();

		// Return the encrypted data buffer.
        return byteEncoding->GetString(memEncryptedData);
	}
    catch (System::Exception^ ex)
    {
		String^ errorMessage = ex->Message;
		return nullptr;
	}
	finally
    {
        // Release all resources.
        if (memoryStream != nullptr)
            memoryStream->Close();

        // Release all resources.
        if (cryptoStream != nullptr)
        {
            try
            {
				cryptoStream->Close();
                cryptoStream->Clear();
            }
			catch(System::Exception^) {}
        }

		if (memoryStream != nullptr)
			delete memoryStream;

		if (cryptoStream != nullptr)
			delete cryptoStream;

		if (byteEncoding != nullptr)
			delete byteEncoding;

		if (provider != nullptr)
			delete provider;
    }
}

/// <summary>
/// This method will decrypt an encrypted string.
/// </summary>
/// <param name="value">The string of encrypted data.</param>
/// <returns>The string of decrypted data.</returns>
String^ Nequeo::Cryptography::AdvancedAES::Decrypt(String^ value)
{
	MemoryStream^ memoryStream = nullptr;
    CryptoStream^ cryptoStream = nullptr;
	ASCIIEncoding^ byteEncoding = nullptr;
	AesCryptoServiceProvider^ provider = nullptr;
	array<Byte>^ memDecryptedData = nullptr;

	try	
	{
		// Convert the passed string to a byte array.
		ASCIIEncoding^ byteEncoding = gcnew ASCIIEncoding();
        array<Byte>^ encryptedData = byteEncoding->GetBytes(value);

		// Will contain the encrypted data
        // from the memory stream.
        memDecryptedData = gcnew array<Byte>(encryptedData->Length);

		// Create a new MemoryStream using the passed 
        // array of encrypted data.
        memoryStream = gcnew MemoryStream(encryptedData);

		// Create a new AES provider.
        provider = gcnew AesCryptoServiceProvider();
        provider->Mode = System::Security::Cryptography::CipherMode::CBC;
        provider->Padding = System::Security::Cryptography::PaddingMode::Zeros;
        provider->BlockSize = 128;
        provider->KeySize = 256;

		// Create a CryptoStream using the MemoryStream 
        // and the passed key and initialization vector (IV).
        cryptoStream = gcnew CryptoStream(memoryStream, provider->CreateDecryptor(this->m_internalKey, this->m_internalIV), 
			System::Security::Cryptography::CryptoStreamMode::Read);

		// Read the decrypted data out of the crypto stream
        // and place it into the temporary buffer.
        cryptoStream->Read(memDecryptedData, 0, memDecryptedData->Length);

		// Close the CryptoStream object 
        // release resources.
        cryptoStream->Close();

		// Close the MemoryStream object 
        // release resources.
        memoryStream->Close();

		// Return the encrypted data buffer.
        return byteEncoding->GetString(memDecryptedData)->Replace("\0", "");
	}
    catch (System::Exception^ ex)
    {
		String^ errorMessage = ex->Message;
		return nullptr;
	}
	finally
    {
        // Release all resources.
        if (memoryStream != nullptr)
            memoryStream->Close();

        // Release all resources.
        if (cryptoStream != nullptr)
        {
            try
            {
				cryptoStream->Close();
                cryptoStream->Clear();
            }
			catch(System::Exception^) {}
        }

		if(memDecryptedData != nullptr)
			delete memDecryptedData;

		if (memoryStream != nullptr)
			delete memoryStream;

		if (cryptoStream != nullptr)
			delete cryptoStream;

		if (byteEncoding != nullptr)
			delete byteEncoding;

		if (provider != nullptr)
			delete provider;
    }
}