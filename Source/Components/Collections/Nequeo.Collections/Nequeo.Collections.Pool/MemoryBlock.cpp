/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          MemoryBlock.cpp
 *  Purpose :       Manages a memory buffer.
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

#include "MemoryBlock.h"

template <typename T>
Nequeo::Collections::Pool::MemoryBlockImp<T>::MemoryBlockImp(size_t length) : _disposed(false), _length(length), _data(new T[length])
{
}

template <typename T>
Nequeo::Collections::Pool::MemoryBlockImp<T>::~MemoryBlockImp()
{
	// If not disposed.
    if (!_disposed)
    {
		// Deleting resource.
		if (_data != NULL)
		{
			delete[] _data;
		}

        _disposed = true;
    }
}

template <typename T>
Nequeo::Collections::Pool::MemoryBlockImp<T>::MemoryBlockImp(const MemoryBlockImp<T>& other) : _disposed(false), _length(other._length), _data(new T[other._length])
{
	std::copy(other._data, other._data + _length, _data);
}

template <typename T>
Nequeo::Collections::Pool::MemoryBlockImp<T>& Nequeo::Collections::Pool::MemoryBlockImp<T>::operator=(const MemoryBlockImp<T>& other)
{
	if (this != &other)
      {
         // Free the existing resource.
         delete[] _data;

         _length = other._length;
         _data = new T[_length];
         std::copy(other._data, other._data + _length, _data);
      }

      return *this;
}

template <typename T>
Nequeo::Collections::Pool::MemoryBlockImp<T>::MemoryBlockImp(MemoryBlockImp<T>&& other) : _disposed(false), _length(0), _data(NULL)
{
	// If you provide both a move constructor and a move assignment operator for your class, 
	// you can eliminate redundant code by writing the move constructor to call the move 
	// assignment operator. The following example shows a revised version of the move constructor 
	// that calls the move assignment operator:
	*this = std::move(other);
}

template <typename T>
Nequeo::Collections::Pool::MemoryBlockImp<T>& Nequeo::Collections::Pool::MemoryBlockImp<T>::operator=(Nequeo::Collections::Pool::MemoryBlockImp<T>&& other)
{
	if (this != &other)
	{
		// Free the existing resource.
		delete[] _data;

		// Copy the data pointer and its length from the 
		// source object.
		_data = other._data;
		_length = other._length;

		// Release the data pointer from the source object so that
		// the destructor does not free the memory multiple times.
		other._data = NULL;
		other._length = 0;
   }

   return *this;
}

template <typename T>
T* Nequeo::Collections::Pool::MemoryBlockImp<T>::GetData()
{
	return _data;
}

template <typename T>
T Nequeo::Collections::Pool::MemoryBlockImp<T>::GetElement(int&& index)
{
	int indexValue = std::forward<int>(index);
	return _data[indexValue];
}

template <typename T>
size_t Nequeo::Collections::Pool::MemoryBlockImp<T>::Length() const
{
	return _length;
}