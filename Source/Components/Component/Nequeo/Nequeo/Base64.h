/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Base64.h
*  Purpose :       Base64 class.
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

#pragma once

#include "Global.h"
#include "Array.h"
#include "Allocator.h"

namespace Nequeo
{
	/**
	* interface for platform specific Base64 encoding/decoding.
	*/
	class Base64
	{
	public:
		Base64(const char *encodingTable = nullptr);

		/**
		* Encode a byte buffer into a base64 stream.
		*
		* throws Base64Exception if encoding fails.
		*/
		Nequeo::String Encode(const ByteBuffer&) const;

		/**
		* Decode a base64 string into a byte buffer.
		*/
		ByteBuffer Decode(const Nequeo::String&) const;

		/**
		* Calculates the required length of a base64 buffer after decoding the
		* input string.
		*/
		static size_t CalculateBase64DecodedLength(const Nequeo::String& b64input);

		/**
		* Calculates the length of an encoded base64 string based on the buffer being encoded
		*/
		static size_t CalculateBase64EncodedLength(const ByteBuffer& buffer);

	private:
		char m_mimeBase64EncodingTable[64];
		uint8_t m_mimeBase64DecodingTable[256];

	};
}