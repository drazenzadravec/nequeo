/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          SuperSmartPointer.cpp
 *  Purpose :       
					The approach for SuperSmartPointer, a reference-counting smart pointer implementation is to keep a
					static map for reference counts. Each key in the map is the memory address of a traditional pointer that is
					referred to by one or more SuperSmartPointers. The corresponding value is the number of
					SuperSmartPointers that refer to that object.
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

#include "SuperSmartPointer.h"

template <typename T>
std::map<T*, int> Nequeo::Collections::Pool::SuperSmartPointer<T>::sRefCountMap;

template <typename T>
Nequeo::Collections::Pool::SuperSmartPointer<T>::SuperSmartPointer(T* inPtr)
{
	initPointer(inPtr);
}

template <typename T>
Nequeo::Collections::Pool::SuperSmartPointer<T>::SuperSmartPointer(const SuperSmartPointer<T>& src)
{
	initPointer(src.mPtr);
}

template <typename T>
Nequeo::Collections::Pool::SuperSmartPointer<T>& Nequeo::Collections::Pool::SuperSmartPointer<T>::operator=(const SuperSmartPointer<T>& rhs)
{
	if (this == &rhs) 
	{
		return (*this);
	}

	finalizePointer();
	initPointer(rhs.mPtr);
	return (*this);
}

template <typename T>
Nequeo::Collections::Pool::SuperSmartPointer<T>::~SuperSmartPointer()
{
	finalizePointer();
}

template<typename T>
void Nequeo::Collections::Pool::SuperSmartPointer<T>::initPointer(T* inPtr)
{
	mPtr = inPtr;
	if (sRefCountMap.find(mPtr) == sRefCountMap.end()) 
	{
		sRefCountMap[mPtr] = 1;
	} 
	else 
	{
		sRefCountMap[mPtr]++;
	}
}

template<typename T>
void Nequeo::Collections::Pool::SuperSmartPointer<T>::finalizePointer()
{
	if (sRefCountMap.find(mPtr) == sRefCountMap.end()) 
	{
		std::cerr << "ERROR: Missing entry in map!" << std::endl;
		return;
	}

	sRefCountMap[mPtr]--;

	if (sRefCountMap[mPtr] == 0) 
	{
		// No more references to this object--delete it and remove from map
		sRefCountMap.erase(mPtr);
		delete mPtr;
	}
}

template <typename T>
const T* Nequeo::Collections::Pool::SuperSmartPointer<T>::operator->() const
{
	return (mPtr);
}

template <typename T>
const T& Nequeo::Collections::Pool::SuperSmartPointer<T>::operator*() const
{
	return (*mPtr);
}

template <typename T>
T* Nequeo::Collections::Pool::SuperSmartPointer<T>::operator->()
{
	return (mPtr);
}

template <typename T>
T& Nequeo::Collections::Pool::SuperSmartPointer<T>::operator*()
{
	return (*mPtr);
}