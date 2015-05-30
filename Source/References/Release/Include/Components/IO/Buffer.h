/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Buffer.h
*  Purpose :       Buffer class.
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

#ifndef _BUFFER_H
#define _BUFFER_H

#include "GlobalStreaming.h"

namespace Nequeo {
	namespace IO
	{
		/// A very simple buffer class that allocates a buffer of
		/// a given type and size in the constructor and
		/// deallocates the buffer in the destructor.
		///
		/// This class is useful everywhere where a temporary buffer
		/// is needed.
		template <class T>
		class Buffer
		{
		public:
			Buffer(std::size_t size) :
				_size(size),
				_ptr(new T[size])
				/// Creates and allocates the Buffer.
			{
			}

			~Buffer()
				/// Destroys the Buffer.
			{
				delete[] _ptr;
			}

			void resize(std::size_t newSize, bool preserveContent = true)
				/// Resizes the buffer. If preserveContent is true,
				/// the content of the old buffer is copied over to the
				/// new buffer. NewSize can be larger or smaller than
				/// the current size, but it must not be 0.
			{
				T* ptr = new T[newSize];
				if (preserveContent)
				{
					std::size_t n = newSize > _size ? _size : newSize;
					std::memcpy(ptr, _ptr, n);
				}
				delete[] _ptr;
				_ptr = ptr;
				_size = newSize;
			}

			std::size_t size() const
				/// Returns the size of the buffer.
			{
				return _size;
			}

			T* begin()
				/// Returns a pointer to the beginning of the buffer.
			{
				return _ptr;
			}

			const T* begin() const
				/// Returns a pointer to the beginning of the buffer.
			{
				return _ptr;
			}

			T* end()
				/// Returns a pointer to end of the buffer.
			{
				return _ptr + _size;
			}

			const T* end() const
				/// Returns a pointer to the end of the buffer.
			{
				return _ptr + _size;
			}

			T& operator [] (std::size_t index)
			{
				return _ptr[index];
			}

			const T& operator [] (std::size_t index) const
			{
				return _ptr[index];
			}

		private:
			Buffer();
			Buffer(const Buffer&);
			Buffer& operator = (const Buffer&);

			std::size_t _size;
			T* _ptr;
		};
	}
}
#endif