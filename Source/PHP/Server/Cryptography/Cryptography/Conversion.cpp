/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          Conversion.cpp
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

///	<summary>
///	Construct the conversion.
///	</summary>
Nequeo::Conversion::Conversion() : m_disposed(false)
{
}

///	<summary>
///	Deconstruct the conversion.
///	</summary>
Nequeo::Conversion::~Conversion()
{
	// If not disposed.
    if (!m_disposed)
    {
        m_disposed = true;
    }
}

/// <summary>
/// Convert a managed string to a c++ string.
/// </summary>
/// <param name="value">The managed string.</param>
/// <returns>The string.</returns>
std::string Nequeo::Conversion::ConvertString(String^ value)
{
    std::string standardString = marshal_as<std::string>(value);
	return standardString;
}

/// <summary>
/// Convert a c++ string to a managed string.
/// </summary>
/// <param name="value">The string.</param>
/// <returns>The managed string.</returns>
String^ Nequeo::Conversion::ConvertStdString(std::string value)
{
	const char* standardChar = value.c_str();
	return gcnew String(standardChar);
}

/// <summary>
/// Convert a c++ string to a managed string.
/// </summary>
/// <param name="value">The string.</param>
/// <returns>The managed string.</returns>
String^ Nequeo::Conversion::ConvertCharArray(char* value)
{
	String^ managedString = Marshal::PtrToStringAnsi((IntPtr)value);
	return managedString;
}

/// <summary>
/// Convert a managed string to a c++ string.
/// </summary>
/// <param name="value">The managed string.</param>
/// <returns>The string.</returns>
char* Nequeo::Conversion::ConvertToCharArray(String^ value)
{
	char* standardChar = (char*)Marshal::StringToHGlobalAnsi(value).ToPointer();
	return standardChar;

	/*
	// The memory allocated by StringToHGlobalAnsi must be deallocated by calling either FreeHGlobal or GlobalFree.
	// Now retrieve the rows and display their contents.
    char *values[MAXCOLS];
    for (int i = 0; i < len; i++)
    {
        // Deallocate the memory allocated using
        // Marshal::StringToHGlobalAnsi.
        GlobalFree(values[i]);
    }
	*/
}