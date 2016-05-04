/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          Hashcode.cpp
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

#include "Hashcode.h"
#include "HashcodeType.h"
#include "Conversion.h"
#include "CHashcode.h"

///	<summary>
///	Construct the hashcode.
///	</summary>
Nequeo::Cryptography::Hashcode::Hashcode() : m_disposed(false)
{
}

///	<summary>
///	Deconstruct the hashcode.
///	</summary>
Nequeo::Cryptography::Hashcode::~Hashcode()
{
	// If not disposed.
    if (!m_disposed)
    {
        m_disposed = true;
    }
}

/// <summary>
/// Get the hashcode from the value.
/// </summary>
/// <param name="value">The value to generate the hash code for.</param>
/// <param name="hashcodeType">The hash name.</param>
/// <returns>The generated hash code.</returns>
String^ Nequeo::Cryptography::Hashcode::GetHashcode(String^ value, HashcodeType hashcodeType)
{
	int i = 0;
	
	// Get ascii encoding.
	Encoding^ asciiEncoding = Encoding::ASCII;

    // Generate the hash code
    HashAlgorithm^ alg = HashAlgorithm::Create(hashcodeType.ToString());
    array<Byte>^ byteValue = asciiEncoding->GetBytes(value);
    array<Byte>^ hashValue = alg->ComputeHash(byteValue);

    // Get the string value of hashcode.
    array<String^>^ octetArrayByte = gcnew array<String^>(hashValue->Length);
    for each (Byte item in hashValue)
        octetArrayByte[i++] = item.ToString("X2");

    // Create the octet string of bytes.
    String^ octetValue = String::Join("", octetArrayByte);

	// Cleanup memory.
	if(octetArrayByte != nullptr)
		delete[] octetArrayByte;
	
    return octetValue;
}

/// <summary>
/// Gets the MD5 hashcode from the value.
/// </summary>
/// <param name="value">The value to generate the hash code for.</param>
/// <returns>The generated hash code.</returns>
String^ Nequeo::Cryptography::Hashcode::GetHashcodeMD5(String^ value)
{
	int i = 0;

	// Get ascii encoding.
	Encoding^ asciiEncoding = Encoding::ASCII;

    // Generate the hash code
    MD5^ md5 = MD5::Create();
    array<Byte>^ byteValue = asciiEncoding->GetBytes(value);
    array<Byte>^ hashValue = md5->ComputeHash(byteValue);

    // Get the string value of hashcode.
    array<String^>^ octetArrayByte = gcnew array<String^>(hashValue->Length);
    for each (Byte item in hashValue)
        octetArrayByte[i++] = item.ToString("X2");

    // Create the octet string of bytes.
    String^ octetValue = String::Join("", octetArrayByte);

	// Cleanup memory.
	if(octetArrayByte != nullptr)
		delete[] octetArrayByte;

    return octetValue;
}

/// <summary>
/// Gets the SHA1 hashcode from the value.
/// </summary>
/// <param name="value">The value to generate the hash code for.</param>
/// <returns>The generated hash code.</returns>
String^ Nequeo::Cryptography::Hashcode::GetHashcodeSHA1(String^ value)
{
	int i = 0;

	// Get ascii encoding.
	Encoding^ asciiEncoding = Encoding::ASCII;

    // Generate the hash code
    SHA1^ sha1 = SHA1::Create();
    array<Byte>^ byteValue = asciiEncoding->GetBytes(value);
    array<Byte>^ hashValue = sha1->ComputeHash(byteValue);

    // Get the string value of hashcode.
    array<String^>^ octetArrayByte = gcnew array<String^>(hashValue->Length);
    for each (Byte item in hashValue)
        octetArrayByte[i++] = item.ToString("X2");

    // Create the octet string of bytes.
    String^ octetValue = String::Join("", octetArrayByte);

	// Cleanup memory.
	if(octetArrayByte != nullptr)
		delete[] octetArrayByte;

    return octetValue;
}