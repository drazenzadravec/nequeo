/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TextConverter.cpp
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

#include "stdafx.h"

#include "TextConverter.h"
#include "TextIterator.h"
#include "TextEncoding.h"

namespace Nequeo{
	namespace IO{
		namespace Text
		{
			int nullTransform(int ch)
			{
				return ch;
			}
		}
	}
}

namespace Nequeo{
	namespace IO{
		namespace Text
		{
			TextConverter::TextConverter(const TextEncoding& inEncoding, const TextEncoding& outEncoding, int defaultChar) :
				_inEncoding(inEncoding),
				_outEncoding(outEncoding),
				_defaultChar(defaultChar)
			{
			}


			TextConverter::~TextConverter()
			{
			}


			int TextConverter::convert(const std::string& source, std::string& destination, Transform trans)
			{
				int errors = 0;
				TextIterator it(source, _inEncoding);
				TextIterator end(source);
				unsigned char buffer[TextEncoding::MAX_SEQUENCE_LENGTH];

				while (it != end)
				{
					int c = *it;
					if (c == -1) { ++errors; c = _defaultChar; }
					c = trans(c);
					int n = _outEncoding.convert(c, buffer, sizeof(buffer));
					if (n == 0) n = _outEncoding.convert(_defaultChar, buffer, sizeof(buffer));

					destination.append((const char*)buffer, n);
					++it;
				}
				return errors;
			}


			int TextConverter::convert(const void* source, int length, std::string& destination, Transform trans)
			{

				int errors = 0;
				const unsigned char* it = (const unsigned char*)source;
				const unsigned char* end = (const unsigned char*)source + length;
				unsigned char buffer[TextEncoding::MAX_SEQUENCE_LENGTH];

				while (it < end)
				{
					int n = _inEncoding.queryConvert(it, 1);
					int uc;
					int read = 1;

					while (-1 > n && (end - it) >= -n)
					{
						read = -n;
						n = _inEncoding.queryConvert(it, read);
					}

					if (-1 > n)
					{
						it = end;
					}
					else
					{
						it += read;
					}

					if (-1 >= n)
					{
						uc = _defaultChar;
						++errors;
					}
					else
					{
						uc = n;
					}

					uc = trans(uc);
					n = _outEncoding.convert(uc, buffer, sizeof(buffer));
					if (n == 0) n = _outEncoding.convert(_defaultChar, buffer, sizeof(buffer));

					destination.append((const char*)buffer, n);
				}
				return errors;
			}


			int TextConverter::convert(const std::string& source, std::string& destination)
			{
				return convert(source, destination, nullTransform);
			}


			int TextConverter::convert(const void* source, int length, std::string& destination)
			{
				return convert(source, length, destination, nullTransform);
			}
		}
	}
}