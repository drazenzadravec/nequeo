/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          ObjectPoolTemplate.cpp
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

#include "stdafx.h"

#include "ObjectPoolTemplate.h"

///	<summary>
///	Construct the ObjectPoolTemplate.
///	</summary>
/// <param name='chunkSize'>The size of the object pool.</param>
template <typename T>
Nequeo::Collections::Pool::ObjectPoolTemplate<T>::ObjectPoolTemplate(int chunkSize) throw(std::invalid_argument, std::bad_alloc) : mChunkSize(chunkSize)
{
	if (mChunkSize <= 0) 
	{
		throw std::invalid_argument(“chunk size must be positive”);
	}

	// Create mChunkSize objects to start.
	allocateChunk();
}

///	<summary>
///	Deconstruct the ObjectPoolTemplate.
///	</summary>
template <typename T>
Nequeo::Collections::Pool::ObjectPoolTemplate<T>::~ObjectPoolTemplate()
{
	// Free each of the allocation chunks.
	for_each(mAllObjects.begin(), mAllObjects.end(), arrayDeleteObject);
}

///	<summary>
/// Reserve an object for use. The reference to the object is invalidated
/// if the object pool itself is freed. Clients must not free the object.
///	</summary>
template <typename T>
T& Nequeo::Collections::Pool::ObjectPoolTemplate<T>::acquireObject()
{
	if (mFreeList.empty()) 
	{
		allocateChunk();
	}

	T* obj = mFreeList.front();
	mFreeList.pop();
	return (*obj);
}

///	<summary>
/// Return the object to the pool. Clients must not use the object after
/// it has been returned to the pool.
///	</summary>
template <typename T>
void Nequeo::Collections::Pool::ObjectPoolTemplate<T>::releaseObject(T& obj)
{
	mFreeList.push(&obj);
}

///	<summary>
///	The default object pool size.
///	</summary>
template<typename T>
const int Nequeo::Collections::Pool::ObjectPoolTemplate<T>::kDefaultChunkSize;

///	<summary>
/// Allocates an array of mChunkSize objects because that’s
/// more efficient than allocating each of them individually.
/// Stores a pointer to the first element of the array in the mAllObjects
/// vector. Adds a pointer to each new object to the mFreeList.
///	</summary>
template <typename T>
void Nequeo::Collections::Pool::ObjectPoolTemplate<T>::allocateChunk()
{
	T* newObjects = new T[mChunkSize];
	mAllObjects.push_back(newObjects);

	for (int i = 0; i < mChunkSize; i++) 
	{
		mFreeList.push(&newObjects[i]);
	}
}

///	<summary>
/// Freeing function for use in the for_each algorithm in the destructor
///	</summary>
/// <param name='T'>The object type within the pool.</param>
template<typename T>
void Nequeo::Collections::Pool::ObjectPoolTemplate<T>::arrayDeleteObject(T* obj)
{
	delete [] obj;
}