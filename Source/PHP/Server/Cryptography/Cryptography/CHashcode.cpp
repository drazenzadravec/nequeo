/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          CHashcode.cpp
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
#include "Conversion.h"
#include "CHashcode.h"

///	<summary>
///	Construct the hashcode.
///	</summary>
Nequeo::Cryptography::CHashcode::CHashcode() : m_disposed(false)
{
}

///	<summary>
///	Deconstruct the hashcode.
///	</summary>
Nequeo::Cryptography::CHashcode::~CHashcode()
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
std::string Nequeo::Cryptography::CHashcode::GetHashcode(std::string value, HashcodeType hashcodeType)
{
	// Create a new managed hash code.
	Nequeo::Cryptography::Hashcode^ hashcode = gcnew Nequeo::Cryptography::Hashcode();
	Nequeo::Conversion^ convert = gcnew Nequeo::Conversion();

	// Get the hash code.
	String^ data = convert->ConvertStdString(value);
	String^ sha1 = hashcode->GetHashcode(data, hashcodeType);
	std::string result = convert->ConvertString(sha1);

	// Delete the managed class.
	delete hashcode;
	delete convert;

	// Return the hash code.
	return result;
}

/// <summary>
/// Gets the MD5 hashcode from the value.
/// </summary>
/// <param name="value">The value to generate the hash code for.</param>
/// <returns>The generated hash code.</returns>
std::string Nequeo::Cryptography::CHashcode::GetHashcodeMD5(std::string value)
{
	// Create a new managed hash code.
	Nequeo::Cryptography::Hashcode^ hashcode = gcnew Nequeo::Cryptography::Hashcode();
	Nequeo::Conversion^ convert = gcnew Nequeo::Conversion();

	// Get the hash code.
	String^ data = convert->ConvertStdString(value);
	String^ sha1 = hashcode->GetHashcodeMD5(data);
	std::string result = convert->ConvertString(sha1);

	// Delete the managed class.
	delete hashcode;
	delete convert;

	// Return the hash code.
	return result;
}

/// <summary>
/// Gets the SHA1 hashcode from the value.
/// </summary>
/// <param name="value">The value to generate the hash code for.</param>
/// <returns>The generated hash code.</returns>
std::string Nequeo::Cryptography::CHashcode::GetHashcodeSHA1(std::string value)
{
	// Create a new managed hash code.
	Nequeo::Cryptography::Hashcode^ hashcode = gcnew Nequeo::Cryptography::Hashcode();
	Nequeo::Conversion^ convert = gcnew Nequeo::Conversion();

	// Get the hash code.
	String^ data = convert->ConvertStdString(value);
	String^ sha1 = hashcode->GetHashcodeSHA1(data);
	std::string result = convert->ConvertString(sha1);

	// Delete the managed class.
	delete hashcode;
	delete convert;

	// Return the hash code.
	return result;
}