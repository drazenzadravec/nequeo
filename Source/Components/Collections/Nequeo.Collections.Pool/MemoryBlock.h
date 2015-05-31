/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          MemoryBlock.h
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
			///	Provides memory buffer managment.
			///	</summary>
			template <typename T>
			class MemoryBlockImp
			{
				public:
					explicit MemoryBlockImp(size_t length);
					~MemoryBlockImp();

					// Copy constructor (copy semantics).
					MemoryBlockImp(const MemoryBlockImp<T>& other);
					MemoryBlockImp<T>& operator=(const MemoryBlockImp<T>& other);

					// Move constructor (move semantics).
					MemoryBlockImp(MemoryBlockImp<T>&& other);
					MemoryBlockImp<T>& operator=(MemoryBlockImp<T>&& other);

					T* GetData();
					T GetElement(int&& index);
					size_t Length() const;

				private:
					bool _disposed;
					size_t _length;		// The length of the resource.
					T* _data;			// The resource.

			};
		}
	}
}