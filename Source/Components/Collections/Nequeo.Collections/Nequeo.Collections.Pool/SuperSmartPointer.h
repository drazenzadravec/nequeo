/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          SuperSmartPointer.h
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

#pragma once

#include "stdafx.h"

using namespace System;

namespace Nequeo 
{
	namespace Collections 
	{
		namespace Pool 
		{
			///	<summary>
			///	The implementation of SuperSmartPointer that follows is based on the smart pointer
			///	</summary>
			template <typename T>
			class SuperSmartPointer
			{
				public:
					explicit SuperSmartPointer(T* inPtr);
					~SuperSmartPointer();

					SuperSmartPointer(const SuperSmartPointer<T>& src);
					SuperSmartPointer<T>& operator=(const SuperSmartPointer<T>& rhs);

					const T& operator*() const;
					const T* operator->() const;
					T& operator*();
					T* operator->();
					operator void*() const { return mPtr; }

				protected:
					T* mPtr;
					static std::map<T*, int> sRefCountMap;
					void finalizePointer();
					void initPointer(T* inPtr);

			};
		}
	}
}