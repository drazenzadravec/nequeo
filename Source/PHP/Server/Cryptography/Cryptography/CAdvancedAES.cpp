/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          CAdvancedAES.cpp
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

#include "AdvancedAES.h"
#include "Conversion.h"
#include "CAdvancedAES.h"

///	<summary>
///	Construct the hashcode.
///	</summary>
Nequeo::Cryptography::CAdvancedAES::CAdvancedAES() : m_disposed(false)
{
}

///	<summary>
///	Deconstruct the hashcode.
///	</summary>
Nequeo::Cryptography::CAdvancedAES::~CAdvancedAES()
{
	// If not disposed.
    if (!m_disposed)
    {
        m_disposed = true;
    }
}

/// <summary>
/// This method will encrypt a decrypted string.
/// </summary>
/// <param name="value">The string of decrypted data.</param>
/// <returns>The array of encrypted data.</returns>
std::string Nequeo::Cryptography::CAdvancedAES::Encrypt(std::string value)
{
	// Create a new managed hash code.
	Nequeo::Cryptography::AdvancedAES^ aes = gcnew Nequeo::Cryptography::AdvancedAES();
	Nequeo::Conversion^ convert = gcnew Nequeo::Conversion();

	// Get the hash code.
	String^ data = convert->ConvertStdString(value);
	String^ sha1 = aes->Encrypt(data);
	std::string result = "";

	// If data exists.
	if(sha1 != nullptr)
		result = convert->ConvertString(sha1);

	// Delete the managed class.
	delete aes;
	delete convert;

	// Return the hash code.
	return result;
}

/// <summary>
/// This method will decrypt an encrypted string.
/// </summary>
/// <param name="value">The string of encrypted data.</param>
/// <returns>The string of decrypted data.</returns>
std::string Nequeo::Cryptography::CAdvancedAES::Decrypt(std::string value)
{
	// Create a new managed hash code.
	Nequeo::Cryptography::AdvancedAES^ aes = gcnew Nequeo::Cryptography::AdvancedAES();
	Nequeo::Conversion^ convert = gcnew Nequeo::Conversion();

	// Get the hash code.
	String^ data = convert->ConvertStdString(value);
	String^ sha1 = aes->Decrypt(data);
	std::string result = "";

	// If data exists.
	if(sha1 != nullptr)
		result = convert->ConvertString(sha1);

	// Delete the managed class.
	delete aes;
	delete convert;

	// Return the hash code.
	return result;
}