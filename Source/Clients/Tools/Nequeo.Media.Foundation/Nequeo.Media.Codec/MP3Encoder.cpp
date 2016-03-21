/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MP3Encoder.cpp
*  Purpose :       MP3Encoder class.
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

#include "MP3Encoder.h"
#include "VideoCodec.h"

#include <vcclr.h>
#include <msclr\auto_handle.h>

using namespace Nequeo::Media;

///	<summary>
///	MP3 encoder.
///	</summary>
MP3Encoder::MP3Encoder() : _disposed(false)
{
}

///	<summary>
///	MP3 encoder deconstructor.
///	</summary>
MP3Encoder::~MP3Encoder()
{
	if (!_disposed)
	{
		// Cleanup the native classes.
		this->!MP3Encoder();

		_disposed = true;
	}
}

///	<summary>
///	MP3 encoder finalizer.
///	</summary>
MP3Encoder::!MP3Encoder()
{
	if (!_disposed)
	{

	}
}

/// <summary>
/// Encode the file.
/// </summary>
/// <param name="inputFilename">The input file to convert.</param>
/// <param name="outputFilename">The mp4 encoded file.</param>
void MP3Encoder::Encode(String^ inputFilename, String^ outputFilename)
{
	int ret;

	std::string inputFilenameN;
	MarshalString(inputFilename, inputFilenameN);

	std::string outputFilenameN;
	MarshalString(outputFilename, outputFilenameN);

	try
	{
		// Start encoding
		ret = LameMp3EncodeFile(inputFilenameN.c_str(), outputFilenameN.c_str());
		if (ret != 0)
		{
			// Throw exception.
			throw std::exception();
		}

		// Get the vector size of the error list.
		unsigned int vectorSize = (unsigned int)GetErrorList().size();

		// If devices exist.
		if (vectorSize > 0)
		{
			// Errors exist.
			// Throw exception.
			throw std::exception();
		}
	}
	catch (const std::exception&)
	{
		// Get the error list.
		StringBuilder^ errorMessage = gcnew StringBuilder();

		// Get the vector size.
		std::vector<char*> errors = GetErrorList();
		unsigned int vectorSize = (unsigned int)errors.size();

		// If devices exist.
		if (vectorSize > 0)
		{
			// For each error found.
			for (int i = 0; i < vectorSize; i++)
			{
				// Get the current error.
				String^ currentError = gcnew String(errors[i]);
				errorMessage->Append(currentError);
			}
		}

		// Throw a general exception.
		System::Exception^ innerException = gcnew System::Exception(errorMessage->ToString());
		throw gcnew System::Exception("Encoding error.", innerException);
	}
	finally {}
}

///	<summary>
///	MarshalString
///	</summary>
/// <param name="s">The string.</param>
/// <param name="os">The native string.</param>
void MP3Encoder::MarshalString(String^ s, std::string& os)
{
	if (!String::IsNullOrEmpty(s))
	{
		using namespace Runtime::InteropServices;
		const char* chars = (const char*)(Marshal::StringToHGlobalAnsi(s)).ToPointer();
		os = chars;
		Marshal::FreeHGlobal(IntPtr((void*)chars));
	}
}

///	<summary>
///	MarshalString
///	</summary>
/// <param name="s">The string.</param>
/// <param name="os">The native string.</param>
void MP3Encoder::MarshalString(String^ s, std::wstring& os)
{
	if (!String::IsNullOrEmpty(s))
	{
		using namespace Runtime::InteropServices;
		const wchar_t* chars = (const wchar_t*)(Marshal::StringToHGlobalUni(s)).ToPointer();
		os = chars;
		Marshal::FreeHGlobal(IntPtr((void*)chars));
	}
}

///	<summary>
///	Convert from wide string to string.
///	</summary>
/// <param name="wstr">The wide string.</param>
/// <returns>The result.</returns>
std::string MP3Encoder::WstringToString(std::wstring wstr)
{
	std::string str(wstr.length(), ' ');
	copy(wstr.begin(), wstr.end(), str.begin());
	return str;
}

///	<summary>
///	Convert from string to wide string.
///	</summary>
/// <param name="str">The string.</param>
/// <returns>The result.</returns>
std::wstring MP3Encoder::StringToWstring(std::string str)
{
	std::wstring wstr(str.length(), L' ');
	copy(str.begin(), str.end(), wstr.begin());
	return wstr;
}