/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          CTransit.h
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

#include "CTransit.h"
#include "CHashcode.h"
#include "CAdvancedAES.h"

///	<summary>
///	Construct the transit.
///	</summary>
CTransit::CTransit() : m_disposed(false)
{
}

///	<summary>
///	Deconstruct the transit.
///	</summary>
CTransit::~CTransit()
{
	// If not disposed.
    if (!m_disposed)
    {
        m_disposed = true;
    }
}

/// <summary>
/// Encrypt the value.
/// </summary>
/// <param name="value">The value to encrypt.</param>
/// <returns>The encrypted value.</returns>
char* CTransit::Encrypt(const char* value)
{
	// Create a cpp hashcode class.
	Nequeo::Cryptography::CAdvancedAES* aes = new Nequeo::Cryptography::CAdvancedAES();
	std::string dec = aes->Encrypt(std::string(value));

	// Delete the resource.
	delete aes;

	// Copy the result.
	int length = dec.length() + 1;
	char* result = new char[length];
	strncpy_s(result, length, dec.c_str(), length);

	// Return the result.
	return result;
}

/// <summary>
/// Decrypt the value.
/// </summary>
/// <param name="value">The value to decrypt.</param>
/// <returns>The decrypted value.</returns>
char* CTransit::Decrypt(const char* value)
{
	// Create a cpp hashcode class.
	Nequeo::Cryptography::CAdvancedAES* aes = new Nequeo::Cryptography::CAdvancedAES();
	std::string dec = aes->Decrypt(std::string(value));

	// Delete the resource.
	delete aes;

	// Copy the result.
	int length = dec.length() + 1;
	char* result = new char[length];
	strncpy_s(result, length, dec.c_str(), length);

	// Return the result.
	return result;
}

/// <summary>
/// Get the hashcode from the value.
/// </summary>
/// <param name="value">The value to generate the hash code for.</param>
/// <param name="hashcodeType">The hash name.</param>
/// <returns>The generated hash code.</returns>
char* CTransit::GetHashcode(const char* value, long hashcodeType)
{
	// Create a cpp hashcode class.
	Nequeo::Cryptography::CHashcode* hashcode = new Nequeo::Cryptography::CHashcode();
	Nequeo::Cryptography::HashcodeType codeType = Nequeo::Cryptography::HashcodeType::SHA1;

	// Select the hash code type.
	switch (hashcodeType)
	{ 
		case 0:
			codeType = Nequeo::Cryptography::HashcodeType::MD5;
			break;
		case 1:
			codeType = Nequeo::Cryptography::HashcodeType::SHA1;
			break;
		case 2:
			codeType = Nequeo::Cryptography::HashcodeType::SHA256;
			break;
		case 3:
			codeType = Nequeo::Cryptography::HashcodeType::SHA384;
			break;
		case 4:
			codeType = Nequeo::Cryptography::HashcodeType::SHA512;
			break;
		default:
			codeType = Nequeo::Cryptography::HashcodeType::SHA1;
			break;
	}

	// Get the code type.
	std::string dec = hashcode->GetHashcode(std::string(value), codeType);

	// Delete the resource.
	delete hashcode;

	// Copy the result.
	int length = dec.length() + 1;
	char* result = new char[length];
	strncpy_s(result, length, dec.c_str(), length);

	// Return the result.
	return result;
}

/// <summary>
/// Gets the MD5 hashcode from the value.
/// </summary>
/// <param name="value">The value to generate the hash code for.</param>
/// <returns>The generated hash code.</returns>
char* CTransit::GetHashcodeMD5(const char* value)
{
	// Create a cpp hashcode class.
	Nequeo::Cryptography::CHashcode* hashcode = new Nequeo::Cryptography::CHashcode();
	std::string dec = hashcode->GetHashcodeMD5(std::string(value));

	// Delete the resource.
	delete hashcode;

	// Copy the result.
	int length = dec.length() + 1;
	char* result = new char[length];
	strncpy_s(result, length, dec.c_str(), length);

	// Return the result.
	return result;
}

/// <summary>
/// Gets the SHA1 hashcode from the value.
/// </summary>
/// <param name="value">The value to generate the hash code for.</param>
/// <returns>The generated hash code.</returns>
char* CTransit::GetHashcodeSHA1(const char* value)
{
	// Create a cpp hashcode class.
	Nequeo::Cryptography::CHashcode* hashcode = new Nequeo::Cryptography::CHashcode();
	std::string dec = hashcode->GetHashcodeSHA1(std::string(value));

	// Delete the resource.
	delete hashcode;

	// Copy the result.
	int length = dec.length() + 1;
	char* result = new char[length];
	strncpy_s(result, length, dec.c_str(), length);

	// Return the result.
	return result;
}