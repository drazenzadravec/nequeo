/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TextBufferIterator.h
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

#pragma once

#ifndef _TEXTBUFFERITERATOR_H
#define _TEXTBUFFERITERATOR_H

#include "GlobalText.h"
#include <cstdlib>

namespace Nequeo{
	namespace IO{
		namespace Text
		{
			class TextEncoding;


			class TextBufferIterator
				/// An unidirectional iterator for iterating over characters in a buffer.
				/// The TextBufferIterator uses a TextEncoding object to
				/// work with multi-byte character encodings like UTF-8.
				/// Characters are reported in Unicode.
				///
				/// Example: Count the number of UTF-8 characters in a buffer.
				///
				///     UTF8Encoding utf8Encoding;
				///     char buffer[] = "...";
				///     TextBufferIterator it(buffer, utf8Encoding);
				///     TextBufferIterator end(it.end());
				///     int n = 0;
				///     while (it != end) { ++n; ++it; }
				///
				/// NOTE: When an UTF-16 encoding is used, surrogate pairs will be
				/// reported as two separate characters, due to restrictions of
				/// the TextEncoding class.
				///
				/// For iterating over the characters in a std::string, see the
				/// TextIterator class.
			{
			public:
				TextBufferIterator();
				/// Creates an uninitialized TextBufferIterator.

				TextBufferIterator(const char* begin, const TextEncoding& encoding);
				/// Creates a TextBufferIterator for the given buffer, which must be 0-terminated.
				/// The encoding object must not be deleted as long as the iterator
				/// is in use.

				TextBufferIterator(const char* begin, std::size_t size, const TextEncoding& encoding);
				/// Creates a TextBufferIterator for the given buffer with the given size.
				/// The encoding object must not be deleted as long as the iterator
				/// is in use.

				TextBufferIterator(const char* begin, const char* end, const TextEncoding& encoding);
				/// Creates a TextBufferIterator for the given range.
				/// The encoding object must not be deleted as long as the iterator
				/// is in use.

				TextBufferIterator(const char* end);
				/// Creates an end TextBufferIterator for the given buffer.

				~TextBufferIterator();
				/// Destroys the TextBufferIterator.

				TextBufferIterator(const TextBufferIterator& it);
				/// Copy constructor.

				TextBufferIterator& operator = (const TextBufferIterator& it);
				/// Assignment operator.

				void swap(TextBufferIterator& it);
				/// Swaps the iterator with another one.

				int operator * () const;
				/// Returns the Unicode value of the current character.
				/// If there is no valid character at the current position,
				/// -1 is returned.

				TextBufferIterator& operator ++ ();
				/// Prefix increment operator.

				TextBufferIterator operator ++ (int);
				/// Postfix increment operator.

				bool operator == (const TextBufferIterator& it) const;
				/// Compares two iterators for equality.

				bool operator != (const TextBufferIterator& it) const;
				/// Compares two iterators for inequality.

				TextBufferIterator end() const;
				/// Returns the end iterator for the range handled
				/// by the iterator.

			private:
				const TextEncoding* _pEncoding;
				const char* _it;
				const char* _end;
			};


			//
			// inlines
			//
			inline bool TextBufferIterator::operator == (const TextBufferIterator& it) const
			{
				return _it == it._it;
			}


			inline bool TextBufferIterator::operator != (const TextBufferIterator& it) const
			{
				return _it != it._it;
			}


			inline void swap(TextBufferIterator& it1, TextBufferIterator& it2)
			{
				it1.swap(it2);
			}


			inline TextBufferIterator TextBufferIterator::end() const
			{
				return TextBufferIterator(_end);
			}
		}
	}
}
#endif
