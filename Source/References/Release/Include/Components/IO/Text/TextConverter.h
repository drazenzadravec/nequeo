/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TextConverter.h
*  Purpose :       TextConverter header.
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

#ifndef _TEXTCONVERTER_H
#define _TEXTCONVERTER_H

#include "GlobalText.h"

namespace Nequeo{
	namespace IO{
		namespace Text
		{
			class TextEncoding;


			class TextConverter
				/// A TextConverter converts strings from one encoding
				/// into another.
			{
			public:
				typedef int(*Transform)(int);
				/// Transform function for convert.

				TextConverter(const TextEncoding& inEncoding, const TextEncoding& outEncoding, int defaultChar = '?');
				/// Creates the TextConverter. The encoding objects must not be deleted while the
				/// TextConverter is in use.

				~TextConverter();
				/// Destroys the TextConverter.

				int convert(const std::string& source, std::string& destination, Transform trans);
				/// Converts the source string from inEncoding to outEncoding
				/// and appends the result to destination. Every character is
				/// passed to the transform function.
				/// If a character cannot be represented in outEncoding, defaultChar
				/// is used instead.
				/// Returns the number of encoding errors (invalid byte sequences
				/// in source).

				int convert(const void* source, int length, std::string& destination, Transform trans);
				/// Converts the source buffer from inEncoding to outEncoding
				/// and appends the result to destination. Every character is
				/// passed to the transform function.
				/// If a character cannot be represented in outEncoding, defaultChar
				/// is used instead.
				/// Returns the number of encoding errors (invalid byte sequences
				/// in source).

				int convert(const std::string& source, std::string& destination);
				/// Converts the source string from inEncoding to outEncoding
				/// and appends the result to destination.
				/// If a character cannot be represented in outEncoding, defaultChar
				/// is used instead.
				/// Returns the number of encoding errors (invalid byte sequences
				/// in source).

				int convert(const void* source, int length, std::string& destination);
				/// Converts the source buffer from inEncoding to outEncoding
				/// and appends the result to destination.
				/// If a character cannot be represented in outEncoding, defaultChar
				/// is used instead.
				/// Returns the number of encoding errors (invalid byte sequences
				/// in source).

			private:
				TextConverter();
				TextConverter(const TextConverter&);
				TextConverter& operator = (const TextConverter&);

				const TextEncoding& _inEncoding;
				const TextEncoding& _outEncoding;
				int                 _defaultChar;
			};
		}
	}
}
#endif
