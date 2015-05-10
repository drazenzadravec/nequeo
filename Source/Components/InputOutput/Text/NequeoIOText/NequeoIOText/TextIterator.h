/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TextIterator.h
*  Purpose :       TextIterator header.
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

#ifndef _TEXTITERATOR_H
#define _TEXTITERATOR_H

#include "GlobalText.h"

namespace Nequeo{
	namespace IO{
		namespace Text
		{
			class TextEncoding;

			class TextIterator
				/// An unidirectional iterator for iterating over characters in a string.
				/// The TextIterator uses a TextEncoding object to
				/// work with multi-byte character encodings like UTF-8.
				/// Characters are reported in Unicode.
				///
				/// Example: Count the number of UTF-8 characters in a string.
				///
				///     UTF8Encoding utf8Encoding;
				///     std::string utf8String("....");
				///     TextIterator it(utf8String, utf8Encoding);
				///     TextIterator end(utf8String);
				///     int n = 0;
				///     while (it != end) { ++n; ++it; }
				///
				/// NOTE: When an UTF-16 encoding is used, surrogate pairs will be
				/// reported as two separate characters, due to restrictions of
				/// the TextEncoding class.
				///
				/// For iterating over char buffers, see the TextBufferIterator class.
			{
			public:
				TextIterator();
				/// Creates an uninitialized TextIterator.

				TextIterator(const std::string& str, const TextEncoding& encoding);
				/// Creates a TextIterator for the given string.
				/// The encoding object must not be deleted as long as the iterator
				/// is in use.

				TextIterator(const std::string::const_iterator& begin, const std::string::const_iterator& end, const TextEncoding& encoding);
				/// Creates a TextIterator for the given range.
				/// The encoding object must not be deleted as long as the iterator
				/// is in use.

				TextIterator(const std::string& str);
				/// Creates an end TextIterator for the given string.

				TextIterator(const std::string::const_iterator& end);
				/// Creates an end TextIterator.

				~TextIterator();
				/// Destroys the TextIterator.

				TextIterator(const TextIterator& it);
				/// Copy constructor.

				TextIterator& operator = (const TextIterator& it);
				/// Assignment operator.

				void swap(TextIterator& it);
				/// Swaps the iterator with another one.

				int operator * () const;
				/// Returns the Unicode value of the current character.
				/// If there is no valid character at the current position,
				/// -1 is returned.

				TextIterator& operator ++ ();
				/// Prefix increment operator.

				TextIterator operator ++ (int);
				/// Postfix increment operator.

				bool operator == (const TextIterator& it) const;
				/// Compares two iterators for equality.

				bool operator != (const TextIterator& it) const;
				/// Compares two iterators for inequality.

				TextIterator end() const;
				/// Returns the end iterator for the range handled
				/// by the iterator.

			private:
				const TextEncoding*         _pEncoding;
				std::string::const_iterator _it;
				std::string::const_iterator _end;
			};


			//
			// inlines
			//
			inline bool TextIterator::operator == (const TextIterator& it) const
			{
				return _it == it._it;
			}


			inline bool TextIterator::operator != (const TextIterator& it) const
			{
				return _it != it._it;
			}


			inline void swap(TextIterator& it1, TextIterator& it2)
			{
				it1.swap(it2);
			}


			inline TextIterator TextIterator::end() const
			{
				return TextIterator(_end);
			}
		}
	}
}
#endif
