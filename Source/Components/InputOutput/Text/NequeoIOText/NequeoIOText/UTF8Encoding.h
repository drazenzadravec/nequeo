/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          UTF8Encoding.h
*  Purpose :       UTF8Encoding header.
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

#ifndef _UTF8ENCODING_H
#define _UTF8ENCODING_H

#include "GlobalText.h"
#include "TextEncoding.h"

namespace Nequeo{
	namespace IO{
		namespace Text
		{
			class UTF8Encoding : public TextEncoding
				/// UTF-8 text encoding, as defined in RFC 2279.
			{
			public:
				UTF8Encoding();
				~UTF8Encoding();
				const char* canonicalName() const;
				bool isA(const std::string& encodingName) const;
				const CharacterMap& characterMap() const;
				int convert(const unsigned char* bytes) const;
				int convert(int ch, unsigned char* bytes, int length) const;
				int queryConvert(const unsigned char* bytes, int length) const;
				int sequenceLength(const unsigned char* bytes, int length) const;

				static bool isLegal(const unsigned char *bytes, int length);
				/// Utility routine to tell whether a sequence of bytes is legal UTF-8.
				/// This must be called with the length pre-determined by the first byte.
				/// The sequence is illegal right away if there aren't enough bytes 
				/// available. If presented with a length > 4, this function returns false.
				/// The Unicode definition of UTF-8 goes up to 4-byte sequences.
				/// 
				/// Adapted from ftp://ftp.unicode.org/Public/PROGRAMS/CVTUTF/ConvertUTF.c
				/// Copyright 2001-2004 Unicode, Inc.

			private:
				static const char* _names[];
				static const CharacterMap _charMap;
			};
		}
	}
}
#endif
