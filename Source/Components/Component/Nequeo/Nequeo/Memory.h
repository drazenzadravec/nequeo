/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Memory.h
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

#pragma once

#ifndef _MEMORY_H
#define _MEMORY_H

#include <memory>
#include <cstdlib>

#include "Global.h"
#include "MemorySystemInterface.h"

namespace Nequeo
{
	namespace Memory
	{
		/**
		*InitializeMemory should be called at the very start of your program
		*/
		void InitializeMemorySystem(MemorySystemInterface& memorySystem);

		/**
		* ShutdownMemory should be called the very end of your program
		*/
		void ShutdownMemorySystem(void);

		/**
		* Get the globally install memory system, if it has been installed.
		*/
		MemorySystemInterface* GetMemorySystem();

	}

	/**
	* ::new, ::delete, ::malloc, ::free, std::make_shared, and std::make_unique should not be used in SDK code
	*  use these functions instead or Nequeo::MakeShared
	*/
	void* Malloc(const char* allocationTag, size_t allocationSize);

	/**
	* ::new, ::delete, ::malloc, ::free, std::make_shared, and std::make_unique should not be used in SDK code
	*  use these functions instead or Nequeo::MakeShared
	*/
	void Free(void* memoryPtr);

	/**
	* ::new, ::delete, ::malloc, ::free, std::make_shared, and std::make_unique should not be used in SDK code
	*  use these functions instead or Nequeo::MakeShared
	*/
	template<typename T, typename ...ArgTypes>
	T* New(const char* allocationTag, ArgTypes&&... args)
	{
		void *rawMemory = Malloc(allocationTag, sizeof(T));

		T *constructedMemory = new (rawMemory) T(std::forward<ArgTypes>(args)...);
		return constructedMemory;
	}

	/**
	* ::new, ::delete, ::malloc, ::free, std::make_shared, and std::make_unique should not be used in SDK code
	*  use these functions instead or Nequeo::MakeShared
	*/
	template<typename T>
	void Delete(T* pointerToT)
	{
		if (pointerToT == nullptr)
		{
			return;
		}

		pointerToT->~T();
		Free(pointerToT);
	}

	template<typename T>
	bool ShouldConstructArrayMembers()
	{
		return std::is_class<T>::value;
	}

	template<typename T>
	bool ShouldDestroyArrayMembers()
	{
		return !std::is_trivially_destructible<T>::value;
	}

	/**
	* ::new, ::delete, ::malloc, ::free, std::make_shared, and std::make_unique should not be used in SDK code
	*  use these functions instead or Nequeo::MakeShared
	*/
	template<typename T>
	T* NewArray(std::size_t amount, const char* allocationTag)
	{
		if (amount > 0)
		{
			bool constructMembers = ShouldConstructArrayMembers<T>();
			bool trackMemberCount = ShouldDestroyArrayMembers<T>();

			// if we need to remember the # of items in the array (because we need to call their destructors) then allocate extra memory and keep the # of items in the extra slot
			std::size_t allocationSize = amount * sizeof(T);
			if (trackMemberCount)
			{
				allocationSize += sizeof(std::size_t);
			}

			void* rawMemory = Malloc(allocationTag, allocationSize);
			T* pointerToT = nullptr;

			if (trackMemberCount)
			{
				std::size_t* pointerToAmount = reinterpret_cast<std::size_t*>(rawMemory);
				*pointerToAmount = amount;
				pointerToT = reinterpret_cast<T*>(reinterpret_cast<void*>(pointerToAmount + 1));
			}
			else
			{
				pointerToT = reinterpret_cast<T*>(rawMemory);
			}

			if (constructMembers)
			{
				for (std::size_t i = 0; i < amount; ++i)
				{
					new (pointerToT + i) T;
				}
			}

			return pointerToT;
		}

		return nullptr;
	}

	/**
	* ::new, ::delete, ::malloc, ::free, std::make_shared, and std::make_unique should not be used in SDK code
	*  use these functions instead or Nequeo::MakeShared
	*/
	template<typename T>
	void DeleteArray(T* pointerToTArray)
	{
		if (pointerToTArray == nullptr)
		{
			return;
		}

		bool destroyMembers = ShouldDestroyArrayMembers<T>();
		void* rawMemory = nullptr;

		if (destroyMembers)
		{
			std::size_t *pointerToAmount = reinterpret_cast<std::size_t *>(reinterpret_cast<void *>(pointerToTArray)) - 1;
			std::size_t amount = *pointerToAmount;

			for (std::size_t i = amount; i > 0; --i)
			{
				(pointerToTArray + i - 1)->~T();
			}
			rawMemory = reinterpret_cast<void *>(pointerToAmount);
		}
		else
		{
			rawMemory = reinterpret_cast<void *>(pointerToTArray);
		}

		Free(rawMemory);
	}

	/**
	* modeled from std::default_delete
	*/
	template<typename T>
	struct Deleter
	{
		Deleter() {}

		template<class U, class = typename std::enable_if<std::is_convertible<U *, T *>::value, void>::type>
		Deleter(const Deleter<U>&)
		{
		}

		void operator()(T *pointerToT) const
		{
			static_assert(0 < sizeof(T), "can't delete an incomplete type");
			Nequeo::Delete(pointerToT);
		}
	};

	template< typename T > using UniquePtr = std::unique_ptr< T, Deleter< T > >;

	/**
	* ::new, ::delete, ::malloc, ::free, std::make_shared, and std::make_unique should not be used in SDK code
	*  use these functions instead or Nequeo::MakeShared
	*/
	template<typename T, typename ...ArgTypes>
	UniquePtr<T> MakeUnique(const char* allocationTag, ArgTypes&&... args)
	{
		return UniquePtr<T>(Nequeo::New<T>(allocationTag, std::forward<ArgTypes>(args)...));
	}

	template<typename T>
	struct ArrayDeleter
	{
		ArrayDeleter() {}

		template<class U, class = typename std::enable_if<std::is_convertible<U *, T *>::value, void>::type>
		ArrayDeleter(const ArrayDeleter<U>&)
		{
		}

		void operator()(T *pointerToTArray) const
		{
			static_assert(0 < sizeof(T), "can't delete an incomplete type");
			Nequeo::DeleteArray(pointerToTArray);
		}
	};

	template< typename T > using UniqueArrayPtr = std::unique_ptr< T, ArrayDeleter< T > >;

	/**
	* ::new, ::delete, ::malloc, ::free, std::make_shared, and std::make_unique should not be used in SDK code
	*  use these functions instead or Nequeo::MakeShared
	*/
	template<typename T, typename ...ArgTypes>
	UniqueArrayPtr<T> MakeUniqueArray(std::size_t amount, const char* allocationTag, ArgTypes&&... args)
	{
		return UniqueArrayPtr<T>(Nequeo::NewArray<T>(amount, allocationTag, std::forward<ArgTypes>(args)...));
	}
}
#endif