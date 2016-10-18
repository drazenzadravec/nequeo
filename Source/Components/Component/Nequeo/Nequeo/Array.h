/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Array.h
*  Purpose :       Array class.
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
#include "Memory.h"
#include "Allocator.h"

#include <memory>
#include <cassert>
#include <cstring>
#include <algorithm>
#include <iterator>

namespace Nequeo
{
	static const char* ARRAY_ALLOCATION_TAG = "Nequeo::Array";

	/**
	* Safe array class with move and copy semantics.
	*/
	template<typename T>
	class Array
	{
	public:
		/**
		* Create new empty array of size arraySize. Default argument is 0. If it is empty then no allocation happens.
		*/
		Array(size_t arraySize = 0) :
			m_size(arraySize),
			m_data(arraySize > 0 ? Nequeo::MakeUniqueArray<T>(arraySize, ARRAY_ALLOCATION_TAG) : nullptr)
		{
		}

		/**
		* Create new array and initialize it to a raw array
		*/
		Array(const T* arrayToCopy, size_t arraySize) :
			m_size(arraySize),
			m_data(nullptr)
		{
			if (arrayToCopy != nullptr && m_size > 0)
			{
				m_data.reset(Nequeo::NewArray<T>(m_size, ARRAY_ALLOCATION_TAG));

#ifdef _WIN32
				std::copy(arrayToCopy, arrayToCopy + arraySize, stdext::checked_array_iterator< T * >(m_data.get(), m_size));
#else
				std::copy(arrayToCopy, arrayToCopy + arraySize, m_data.get());
#endif // MSVC
			}
		}

		/**
		* Merge multiple arrays into one
		*/
		Array(Nequeo::Vector<Array*>&& toMerge)
		{
			size_t totalSize = 0;
			for (auto& array : toMerge)
			{
				totalSize += array->m_size;
			}

			m_size = totalSize;
			m_data.reset(Nequeo::NewArray<T>(m_size, ARRAY_ALLOCATION_TAG));

			size_t location = 0;
			for (auto& arr : toMerge)
			{
				if (arr->m_size > 0 && arr->m_data)
				{
					size_t arraySize = arr->m_size;
#ifdef _WIN32
					std::copy(arr->m_data.get(), arr->m_data.get() + arraySize, stdext::checked_array_iterator< T * >(m_data.get() + location, m_size));
#else
					std::copy(arr->m_data.get(), arr->m_data.get() + arraySize, m_data.get() + location);
#endif // MSVC
					location += arraySize;
				}
			}
		}

		Array(const Array& other)
		{
			m_size = other.m_size;
			m_data = nullptr;

			if (m_size > 0)
			{
				m_data.reset(Nequeo::NewArray<T>(m_size, ARRAY_ALLOCATION_TAG));

#ifdef _WIN32
				std::copy(other.m_data.get(), other.m_data.get() + other.m_size, stdext::checked_array_iterator< T * >(m_data.get(), m_size));
#else
				std::copy(other.m_data.get(), other.m_data.get() + other.m_size, m_data.get());
#endif // MSVC
			}
		}

		//move c_tor
		Array(Array&& other) :
			m_size(other.m_size),
			m_data(std::move(other.m_data))
		{
			other.m_size = 0;
			other.m_data = nullptr;
		}

		virtual ~Array() = default;

		Array& operator=(const Array& other)
		{
			if (this == &other)
			{
				return *this;
			}

			m_size = other.m_size;
			m_data = nullptr;

			if (m_size > 0)
			{
				m_data.reset(Nequeo::NewArray<T>(m_size, ARRAY_ALLOCATION_TAG));

#ifdef _WIN32
				std::copy(other.m_data.get(), other.m_data.get() + other.m_size, stdext::checked_array_iterator< T * >(m_data.get(), m_size));
#else
				std::copy(other.m_data.get(), other.m_data.get() + other.m_size, m_data.get());
#endif // MSVC
			}

			return *this;
		}

		Array& operator=(Array&& other)
		{
			m_size = other.m_size;
			m_data = std::move(other.m_data);

			return *this;
		}

		bool operator==(const Array& other) const
		{
			if (this == &other)
				return true;

			if (m_size == 0 && other.m_size == 0)
			{
				return true;
			}

			if (m_size == other.m_size && m_data && other.m_data)
			{
				for (unsigned i = 0; i < m_size; ++i)
				{
					if (m_data.get()[i] != other.m_data.get()[i])
						return false;
				}

				return true;
			}

			return false;
		}

		bool operator!=(const Array& other) const
		{
			return !(*this == other);
		}

		T const& GetItem(size_t index) const
		{
			assert(index < m_size);
			return m_data.get()[index];
		}

		T& GetItem(size_t index)
		{
			assert(index < m_size);
			return m_data.get()[index];
		}

		T& operator[](size_t index)
		{
			return GetItem(index);
		}

		T const& operator[](size_t index) const
		{
			return GetItem(index);
		}

		inline size_t GetLength() const
		{
			return m_size;
		}

		inline T* GetUnderlyingData() const
		{
			return m_data.get();
		}

	protected:
		size_t m_size;

		Nequeo::UniqueArrayPtr<T> m_data;
	};

	typedef Array<unsigned char> ByteBuffer;

	/**
	* Buffer for cryptographic operations. It zeroes itself back out upon deletion. Everything else is identical
	* to byte buffer.
	*/
	class CryptoBuffer : public ByteBuffer
	{
	public:
		CryptoBuffer(size_t arraySize = 0) : ByteBuffer(arraySize) {}
		CryptoBuffer(const unsigned char* arrayToCopy, size_t arraySize) : ByteBuffer(arrayToCopy, arraySize) {}
		CryptoBuffer(Nequeo::Vector<ByteBuffer*>&& toMerge) : ByteBuffer(std::move(toMerge)) {}
		CryptoBuffer(const ByteBuffer& other) : ByteBuffer(other) {}
		CryptoBuffer(const CryptoBuffer& other) : ByteBuffer(other) {}
		CryptoBuffer(CryptoBuffer&& other) : ByteBuffer(std::move(other)) {}
		CryptoBuffer& operator=(const CryptoBuffer&) = default;
		CryptoBuffer& operator=(CryptoBuffer&& other) { ByteBuffer::operator=(std::move(other)); return *this; }
		bool operator==(const CryptoBuffer& other) const { return ByteBuffer::operator==(other); }
		bool operator!=(const CryptoBuffer& other) const { return ByteBuffer::operator!=(other); }

		~CryptoBuffer() { Zero(); }

		Array<CryptoBuffer> Slice(size_t sizeOfSlice) const;
		CryptoBuffer& operator^(const CryptoBuffer& operand);
		void Zero();
	};
}