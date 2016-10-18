/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MemorySystemInterface.h
*  Purpose :       Memory System Interface.
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

#ifndef _MEMORYSYSTEMINTERFACE_H
#define _MEMORYSYSTEMINTERFACE_H

#include "Global.h"
#include <cstdlib>

namespace Nequeo
{
	namespace Memory
	{
		/**
		* Central interface for memory management customizations. To create a custom memory manager.
		*/
		class MemorySystemInterface
		{
		public:
			virtual ~MemorySystemInterface() = default;

			/**
			* This is for initializing your memory manager in a static context. This can be empty if you don't need to do that.
			*/
			virtual void Begin() = 0;
			/**
			* This is for cleaning up your memory manager in a static context. This can be empty if you don't need to do that.
			*/
			virtual void End() = 0;

			/**
			* Allocate your memory inside this method. blocksize and alignment are exactly the same as the std::alocators interfaces.
			* The allocationTag parameter is for memory tracking; you don't have to handle it.
			*/
			virtual void* AllocateMemory(std::size_t blockSize, std::size_t alignment, const char *allocationTag = nullptr) = 0;

			/**
			* Free the memory pointed to by memoryPtr.
			*/
			virtual void FreeMemory(void* memoryPtr) = 0;
		};
	}
}
#endif