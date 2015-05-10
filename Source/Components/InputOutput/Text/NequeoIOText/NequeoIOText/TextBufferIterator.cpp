/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TextBufferIterator.cpp
*  Purpose :       TextBufferIterator header.
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

#include "TextBufferIterator.h"
#include "TextEncoding.h"
#include <algorithm>
#include <cstring>

namespace Nequeo{
	namespace IO{
		namespace Text
		{
			TextBufferIterator::TextBufferIterator() :
				_pEncoding(0),
				_it(0),
				_end(0)
			{
			}


			TextBufferIterator::TextBufferIterator(const char* begin, const TextEncoding& encoding) :
				_pEncoding(&encoding),
				_it(begin),
				_end(begin + std::strlen(begin))
			{
			}


			TextBufferIterator::TextBufferIterator(const char* begin, std::size_t size, const TextEncoding& encoding) :
				_pEncoding(&encoding),
				_it(begin),
				_end(begin + size)
			{
			}


			TextBufferIterator::TextBufferIterator(const char* begin, const char* end, const TextEncoding& encoding) :
				_pEncoding(&encoding),
				_it(begin),
				_end(end)
			{
			}


			TextBufferIterator::TextBufferIterator(const char* end) :
				_pEncoding(0),
				_it(end),
				_end(end)
			{
			}


			TextBufferIterator::~TextBufferIterator()
			{
			}


			TextBufferIterator::TextBufferIterator(const TextBufferIterator& it) :
				_pEncoding(it._pEncoding),
				_it(it._it),
				_end(it._end)
			{
			}


			TextBufferIterator& TextBufferIterator::operator = (const TextBufferIterator& it)
			{
				if (&it != this)
				{
					_pEncoding = it._pEncoding;
					_it = it._it;
					_end = it._end;
				}
				return *this;
			}


			void TextBufferIterator::swap(TextBufferIterator& it)
			{
				std::swap(_pEncoding, it._pEncoding);
				std::swap(_it, it._it);
				std::swap(_end, it._end);
			}


			int TextBufferIterator::operator * () const
			{
				const char* it = _it;

				unsigned char buffer[TextEncoding::MAX_SEQUENCE_LENGTH];
				unsigned char* p = buffer;

				if (it != _end)
					*p++ = *it++;
				else
					*p++ = 0;

				int read = 1;
				int n = _pEncoding->queryConvert(buffer, 1);

				while (-1 > n && (_end - it) >= -n - read)
				{
					while (read < -n && it != _end)
					{
						*p++ = *it++;
						read++;
					}
					n = _pEncoding->queryConvert(buffer, read);
				}

				if (-1 > n)
				{
					return -1;
				}
				else
				{
					return n;
				}
			}


			TextBufferIterator& TextBufferIterator::operator ++ ()
			{
				unsigned char buffer[TextEncoding::MAX_SEQUENCE_LENGTH];
				unsigned char* p = buffer;

				if (_it != _end)
					*p++ = *_it++;
				else
					*p++ = 0;

				int read = 1;
				int n = _pEncoding->sequenceLength(buffer, 1);

				while (-1 > n && (_end - _it) >= -n - read)
				{
					while (read < -n && _it != _end)
					{
						*p++ = *_it++;
						read++;
					}
					n = _pEncoding->sequenceLength(buffer, read);
				}
				while (read < n && _it != _end)
				{
					_it++;
					read++;
				}

				return *this;
			}


			TextBufferIterator TextBufferIterator::operator ++ (int)
			{
				TextBufferIterator prev(*this);
				operator ++ ();
				return prev;
			}
		}
	}
}