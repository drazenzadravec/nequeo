/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Memory.cpp
*  Purpose :       Memory.
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

#include "Memory.h"

#include <atomic>

using namespace Nequeo;
using namespace Nequeo::Memory;

static MemorySystemInterface* MemorySystem(nullptr);

void Nequeo::Memory::InitializeMemorySystem(MemorySystemInterface& memorySystem)
{
	if (MemorySystem != nullptr)
	{
		MemorySystem->End();
	}

	MemorySystem = &memorySystem;
	MemorySystem->Begin();
}

void Nequeo::Memory::ShutdownMemorySystem(void)
{
	if (MemorySystem != nullptr)
	{
		MemorySystem->End();
	}
	MemorySystem = nullptr;
}

MemorySystemInterface* Nequeo::Memory::GetMemorySystem()
{
	return MemorySystem;
}

void* Nequeo::Malloc(const char* allocationTag, size_t allocationSize)
{
	Nequeo::Memory::MemorySystemInterface* memorySystem = Nequeo::Memory::GetMemorySystem();

	void* rawMemory = nullptr;
	if (memorySystem != nullptr)
	{
		rawMemory = memorySystem->AllocateMemory(allocationSize, 1, allocationTag);
	}
	else
	{
		rawMemory = malloc(allocationSize);
	}

	return rawMemory;
}


void Nequeo::Free(void* memoryPtr)
{
	if (memoryPtr == nullptr)
	{
		return;
	}

	Nequeo::Memory::MemorySystemInterface* memorySystem = Nequeo::Memory::GetMemorySystem();
	if (memorySystem != nullptr)
	{
		memorySystem->FreeMemory(memoryPtr);
	}
	else
	{
		free(memoryPtr);
	}
}