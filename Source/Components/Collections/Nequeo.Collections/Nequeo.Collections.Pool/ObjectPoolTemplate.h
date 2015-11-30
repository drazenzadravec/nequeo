/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          ObjectPoolTemplate.h
 *  Purpose :       
 *					Template class ObjectPoolTemplate
 *
 *					Provides an object pool that can be used with any class that provides a
 *					default constructor
 *
 *					The object pool constructor creates a pool of objects, which it hands out
 *					to clients when requested via the acquireObject() method. When a client is
 *					finished with the object it calls releaseObject() to put the object back
 *					into the object pool.
 *
 *					The constructor and destructor on each object in the pool will be called only
 *					once each for the lifetime of the program, not once per acquisition and release.
 *
 *					The primary use of an object pool is to avoid creating and deleting objects
 *					repeatedly. The object pool is most suited to applications that use large
 *					numbers of objects for short periods of time.
 *
 *					For efficiency, the object pool doesn’t perform sanity checks.
 *					It expects the user to release every acquired object exactly once.
 *					It expects the user to avoid using any objects that he or she has released.
 *
 *					It expects the user not to delete the object pool until every object
 *					that was acquired has been released. Deleting the object pool invalidates
 *					any objects that the user has acquired, even if they have not yet been released.
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
			///	Provides an object pool that can be used with any class that provides a default constructor
			///	</summary>
			template <typename T>
			class ObjectPoolTemplate
			{
				public:
					
					// Creates an object pool with chunkSize objects.
					// Whenever the object pool runs out of objects, chunkSize
					// more objects will be added to the pool. The pool only grows:
					// objects are never removed from the pool (freed), until
					// the pool is destroyed.
					//
					// Throws invalid_argument if chunkSize is <= 0
					ObjectPoolTemplate(int chunkSize = kDefaultChunkSize) throw(std::invalid_argument, std::bad_alloc);

					// Frees all the allocated objects. Invalidates any objects that have
					// been acquired for use
					virtual ~ObjectPoolTemplate();

					// Reserve an object for use. The reference to the object is invalidated
					// if the object pool itself is freed.
					//
					// Clients must not free the object!
					T& acquireObject();

					// Return the object to the pool. Clients must not use the object after
					// it has been returned to the pool.
					void releaseObject(T& obj);

				protected:
			
					// mFreeList stores the objects that are not currently in use
					// by clients.

					queue<T*> mFreeList;
				
					// mAllObjects stores pointers to all the objects, in use
					// or not. This vector is needed in order to ensure that all
					// objects are freed properly in the destructor.
					vector<T*> mAllObjects;

					int mChunkSize;
					static const int kDefaultChunkSize = 10;
					
					// Allocates mChunkSize new objects and adds them
					// to the mFreeList
					void allocateChunk();

					static void arrayDeleteObject(T* obj);

				private:
					// Prevent assignment and pass-by-value.
					ObjectPoolTemplate(const ObjectPoolTemplate<T>& src);
					ObjectPoolTemplate<T>& operator=(const ObjectPoolTemplate<T>& rhs);

			};
		}
	}
}