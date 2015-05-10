/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          UTF16Encoding.h
*  Purpose :       UTF16Encoding header.
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

#ifndef _UTF16ENCODING_H
#define _UTF16ENCODING_H

#include "GlobalText.h"
#include "TextEncoding.h"

namespace Nequeo{
	namespace IO{
		namespace Text
		{
			class UTF16Encoding : public TextEncoding
				/// UTF-16 text encoding, as defined in RFC 2781.
				///
				/// When converting from UTF-16 to Unicode, surrogates are
				/// reported as they are - in other words, surrogate pairs
				/// are not combined into one Unicode character. 
				/// When converting from Unicode to UTF-16, however, characters
				/// outside the 16-bit range are converted into a low and
				/// high surrogate.
			{
			public:
				enum ByteOrderType
				{
					BIG_ENDIAN_BYTE_ORDER,
					LITTLE_ENDIAN_BYTE_ORDER,
					NATIVE_BYTE_ORDER
				};

				UTF16Encoding(ByteOrderType byteOrder = NATIVE_BYTE_ORDER);
				/// Creates and initializes the encoding for the given byte order.

				UTF16Encoding(int byteOrderMark);
				/// Creates and initializes the encoding for the byte-order
				/// indicated by the given byte-order mark, which is the Unicode
				/// character 0xFEFF.

				~UTF16Encoding();

				ByteOrderType getByteOrder() const;
				/// Returns the byte-order currently in use.

				void setByteOrder(ByteOrderType byteOrder);
				/// Sets the byte order.

				void setByteOrder(int byteOrderMark);
				/// Sets the byte order according to the given
				/// byte order mark, which is the Unicode
				/// character 0xFEFF.

				const char* canonicalName() const;
				bool isA(const std::string& encodingName) const;
				const CharacterMap& characterMap() const;
				int convert(const unsigned char* bytes) const;
				int convert(int ch, unsigned char* bytes, int length) const;
				int queryConvert(const unsigned char* bytes, int length) const;
				int sequenceLength(const unsigned char* bytes, int length) const;

			private:
				bool _flipBytes;
				static const char* _names[];
				static const CharacterMap _charMap;
			};
		}
	}
}
#endif
