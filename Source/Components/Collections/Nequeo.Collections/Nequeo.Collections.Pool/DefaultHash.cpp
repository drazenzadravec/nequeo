/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          DefaultHash.cpp
 *  Purpose :       
 *					Class DefaultHash
 *
 *					This hashmap implementation uses chained hashing (also called open hashing) and does not attempt to
					provide advanced features like rehashing.
 
 					The first choice when writing a hashmap is how to handle hash functions. Recalling the adage that a
					good abstraction makes the easy case easy and the hard case possible, a good hashmap interface allows
					clients to specify their own hash function and number of buckets in order to customize the hashing
					behavior for their particular workload. On the other hand, clients that do not have the desire, or ability,
					to write a good hash function and choose a number of buckets should be able to use the container without
					doing so. One solution is to allow clients to provide a hash function and number of buckets in the
					hashmap constructor, but also to provide defaults values. It also makes sense to package the hash function
					and the number of buckets into a hashing class. Our default hash class definition looks like this:
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

#include "DefaultHash.h"

// Throws invalid_argument if numBuckets is nonpositive
template <typename T>
Nequeo::Collections::Pool::DefaultHash<T>::DefaultHash(int numBuckets) throw (invalid_argument)
{
	if (numBuckets <= 0) 
	{
		throw (invalid_argument("numBuckets must be > 0"));
	}
	mNumBuckets = numBuckets;
}

// Uses the division method for hashing.
// Treats the key as a sequence of bytes, sums the ASCII
// values of the bytes, and mods the total by the number
// of buckets.
template <typename T>
int Nequeo::Collections::Pool::DefaultHash<T>::hash(const T& key) const
{
	int bytes = sizeof(key);
	unsigned long res = 0;

	for (int i = 0; i < bytes; ++i) 
	{
		res += *((char*)&key + i);
	}

	return (res % mNumBuckets);
}

// Throws invalid_argument if numBuckets is nonpositive
Nequeo::Collections::Pool::DefaultHash<string>::DefaultHash(int numBuckets) throw (invalid_argument)
{
	if (numBuckets <= 0) 
	{
		throw (invalid_argument("numBuckets must be > 0"));
	}
	mNumBuckets = numBuckets;
}

// Uses the division method for hashing after summing the
// ASCII values of all the characters in key.
int Nequeo::Collections::Pool::DefaultHash<string>::hash(const string& key) const
{
	int sum = 0;

	for (size_t i = 0; i < key.size(); i++) 
	{
		sum += key[i];
	}

	return (sum % mNumBuckets);
}

// Make a call to insert() to actually insert the elements.
template <typename Key, typename T, typename Compare, typename Hash>
template <class InputIterator>
Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::hashmap(InputIterator first, InputIterator last, const Compare& comp, const Hash& hash) throw(invalid_argument) : mSize(0), mComp(comp), mHash(hash)
{
	if (mHash.numBuckets() <= 0) 
	{
		throw (invalid_argument("Number of buckets must be positive"));
	}

	mElems = new vector<list<value_type> >(mHash.numBuckets());
	insert(first, last);
}

/// <summary>
/// A hashmap supports three basic operations: insertion, deletion, and lookup. Of course, it provides a constructor,
/// destructor, copy constructor, and assignment operator as well. Here is the public portion of the
/// hashmap class template:
///	</summary>
template <typename Key, typename T, typename Compare, typename Hash> 
Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::hashmap(const Compare& comp, const Hash& hash) throw(invalid_argument) : mSize(0), mComp(comp), mHash(hash)
{
	if (mHash.numBuckets() <= 0) 
	{
		throw (invalid_argument(“Number of buckets must be positive”));
	}
	mElems = new vector<list<value_type> >(mHash.numBuckets());
}

template <typename Key, typename T, typename Compare, typename Hash>
Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::~hashmap()
{
	delete mElems;
}

template <typename Key, typename T, typename Compare, typename Hash>
Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::hashmap(const hashmap<Key, T, Compare, Hash>& src) : mSize(src.mSize), mComp(src.mComp), mHash(src.mHash)
{
	// Don’t need to bother checking if numBuckets is positive, because
	// we know src checked
	// Use the vector copy constructor.
	mElems = new vector<list<value_type> >(*(src.mElems));
}

template <typename Key, typename T, typename Compare, typename Hash>
Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>& Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::operator=(const hashmap<Key, T, Compare, Hash>& rhs)
{
	// Check for self-assignment.
	if (this != &rhs) 
	{
		delete mElems;
		mSize = rhs.mSize;
		mComp = rhs.mComp;
		mHash = rhs.mHash;

		// Don’t need to bother checking if numBuckets is positive, because
		// we know rhs checked
		// Use the vector copy constructor.
		mElems = new vector<list<value_type> >(*(rhs.mElems));
	}
	return (*this);
}

template <typename Key, typename T, typename Compare, typename Hash>
typename list<pair<const Key, T> >::iterator Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::findElement(const key_type& x, int& bucket) const
{
	// Hash the key to get the bucket.
	bucket = mHash.hash(x);

	// Look for the key in the bucket.
	for (typename ListType::iterator it = (*mElems)[bucket].begin(); it != (*mElems)[bucket].end(); ++it) 
	{
		if (mComp(it->first, x)) 
		{
			return (it);
		}
	}
	return ((*mElems)[bucket].end());
}

template <typename Key, typename T, typename Compare, typename Hash>
typename Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::value_type* Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::find(const key_type& x)
{
	int bucket;
	// Use the findElement() helper.

	typename ListType::iterator it = findElement(x, bucket);

	if (it == (*mElems)[bucket].end()) 
	{
		// We didn’t find the element--return NULL.
		return (NULL);
	}

	// We found the element. Return a pointer to it.
	return (&(*it));
}

template <typename Key, typename T, typename Compare, typename Hash>
T& Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::operator[] (const key_type& x)
{
	// Try to find the element.
	// If it doesn’t exist, add a new element.
	value_type* found = find(x);

	if (found == NULL) 
	{
		insert(make_pair(x, T()));
		found = find(x);
	}
	return (found->second);
}

template <typename Key, typename T, typename Compare, typename Hash>
void Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::insert(const value_type& x)
{
	int bucket;

	// Try to find the element.
	typename ListType::iterator it = findElement(x.first, bucket);

	if (it != (*mElems)[bucket].end()) 
	{
		// The element already exists.
		return;
	} 
	else 
	{
		// We didn’t find the element, so insert a new one.
		mSize++;
		(*mElems)[bucket].insert((*mElems)[bucket].end(), x);
	}
}

template <typename Key, typename T, typename Compare, typename Hash>
void Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::erase(const key_type& x)
{
	int bucket;

	// First, try to find the element.
	typename ListType::iterator it = findElement(x, bucket);

	if (it != (*mElems)[bucket].end()) 
	{
		// The element already exists--erase it.
		(*mElems)[bucket].erase(it);
		mSize--;
	}
}

template <typename Key, typename T, typename Compare, typename Hash>
bool Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::empty() const
{
	return (mSize == 0);
}

template <typename Key, typename T, typename Compare, typename Hash>
typename Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::size_type Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::size() const
{
	return (mSize);
}

template <typename Key, typename T, typename Compare, typename Hash>
typename Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::size_type Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::max_size() const
{
	// In the worst case, all the elements hash to the
	// same list, so the max_size is the max_size of a single
	// list. This code assumes that all the lists have the same
	// max_size.
	return ((*mElems)[0].max_size());
}

// Just swap the four data members.
// Use the generic swap template.
template <typename Key, typename T, typename Compare, typename Hash>
void Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::swap(hashmap<Key, T, Compare, Hash>& hashIn)
{
	// Explicitly qualify with std:: so the compiler doesn’t think
	// it’s a recursive call.
	std::swap(*this, hashIn);
}

//template <typename Key, typename T, typename Compare, typename Hash>
//typename Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::iterator_hash Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::begin()
//{
//	if (mSize == 0) 
//	{
//		// Special case: there are no elements, so return the end iterator
//		return (end());
//	}
//
//	// We know there is at least one element. Find the first element.
//	for (size_t i = 0; i < mElems->size(); ++i) 
//	{
//		if (!((*mElems)[i].empty())) 
//		{
//			return (HashIterator<Key, T, Compare, Hash>(i, (*mElems)[i].begin(), this));
//		}
//	}
//
//	// Should never reach here, but if we do, return the end iterator
//	return (end());
//}
//
//template <typename Key, typename T, typename Compare, typename Hash>
//typename Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::iterator_hash Nequeo::Collections::Pool::hashmap<Key, T, Compare, Hash>::end()
//{
//	// The end iterator is just the end iterator of the list in last bucket.
//	return (HashIterator<Key, T, Compare, Hash>(mElems->size() - 1, (*mElems)[mElems->size() - 1].end(), this));
//}

// Dereferencing or incrementing an iterator constructed with the
// default ctor is undefined, so it doesn’t matter what values we give
// here.
template<typename Key, typename T, typename Compare, typename Hash>
Nequeo::Collections::Pool::HashIterator<Key, T, Compare, Hash>::HashIterator()
{
	mBucket = -1;
	mIt = list<pair<const Key, T> >::iterator();
	mHashmap = NULL;
}

template<typename Key, typename T, typename Compare, typename Hash>
Nequeo::Collections::Pool::HashIterator<Key, T, Compare, Hash>::HashIterator(int bucket, typename list<pair<const Key, T> >::iterator listIt, const hashmap<Key, T, Compare, Hash>* inHashmap) : mBucket(bucket), mIt(listIt), mHashmap(inHashmap)
{
}

// Return the actual element
template<typename Key, typename T, typename Compare, typename Hash>
pair<const Key, T>& Nequeo::Collections::Pool::HashIterator<Key, T, Compare, Hash>::operator*() const
{
	return (*mIt);
}

// Return the iterator, so the compiler can apply -> to it to access
// the actual desired field.
template<typename Key, typename T, typename Compare, typename Hash>
pair<const Key, T>* Nequeo::Collections::Pool::HashIterator<Key, T, Compare, Hash>::operator->() const
{
	return (&(*mIt));
}

// Defer the details to the increment() helper.
template<typename Key, typename T, typename Compare, typename Hash>
Nequeo::Collections::Pool::HashIterator<Key, T, Compare, Hash>& Nequeo::Collections::Pool::HashIterator<Key, T, Compare, Hash>::operator++()
{
	increment();
	return (*this);
}

// Defer the details to the increment() helper.
template<typename Key, typename T, typename Compare, typename Hash>
const Nequeo::Collections::Pool::HashIterator<Key, T, Compare, Hash> Nequeo::Collections::Pool::HashIterator<Key, T, Compare, Hash>::operator++(int)
{
	Nequeo::Collections::Pool::HashIterator<Key, T, Compare, Hash> oldIt = *this;
	increment();
	return (oldIt);
}

// Defer the details to the decrement() helper.
template<typename Key, typename T, typename Compare, typename Hash>
Nequeo::Collections::Pool::HashIterator<Key, T, Compare, Hash>& Nequeo::Collections::Pool::HashIterator<Key, T, Compare, Hash>::operator--()
{
	decrement();
	return (*this);
}

// Defer the details to the decrement() helper.
template<typename Key, typename T, typename Compare, typename Hash>
const Nequeo::Collections::Pool::HashIterator<Key, T, Compare, Hash> Nequeo::Collections::Pool::HashIterator<Key, T, Compare, Hash>::operator--(int)
{
	Nequeo::Collections::Pool::HashIterator<Key, T, Compare, Hash> newIt = *this;
	decrement();
	return (newIt);
}

// Behavior is undefined if mIt already refers to the past-the-end
// element in the table, or is otherwise invalid.
template<typename Key, typename T, typename Compare, typename Hash>
void Nequeo::Collections::Pool::HashIterator<Key, T, Compare, Hash>::increment()
{
	// mIt is an iterator into a single bucket.
	// Increment it.
	++mIt;

	// If we’re at the end of the current bucket,
	// find the next bucket with elements.
	if (mIt == (*mHashmap->mElems)[mBucket].end()) 
	{
		for (int i = mBucket + 1; i < (*mHashmap->mElems).size(); i++) 
		{
			if (!((*mHashmap->mElems)[i].empty())) 
			{
				// We found a nonempty bucket.
				// Make mIt refer to the first element in it.
				mIt = (*mHashmap->mElems)[i].begin();
				mBucket = i;
				return;
			}
		}

		// No more empty buckets. Assign mIt to refer to the end
		// iterator of the last list.
		mBucket = (*mHashmap->mElems).size() - 1;
		mIt = (*mHashmap->mElems)[mBucket].end();
	}
}

// Behavior is undefined if mIt already refers to the first element
// in the table, or is otherwise invalid.
template<typename Key, typename T, typename Compare, typename Hash>
void Nequeo::Collections::Pool::HashIterator<Key, T, Compare, Hash>::decrement()
{
	// mIt is an iterator into a single bucket.
	// If it’s at the beginning of the current bucket, don’t decrement it.
	// Instead, try to find a nonempty bucket ahead of the current one.
	if (mIt == (*mHashmap->mElems)[mBucket].begin()) 
	{
		for (int i = mBucket - 1; i >= 0; --i) 
		{
			if (!((*mHashmap->mElems)[i].empty())) 
			{
				mIt = (*mHashmap->mElems)[i].end();
				--mIt;
				mBucket = i;
				return;
			}
		}

		// No more nonempty buckets. This is an invalid decrement.
		// Assign mIt to refer to one before the start element of the first
		// list (an invalid position).
		mIt = (*mHashmap->mElems)[0].begin();
		--mIt;
		mBucket = 0;
	} 
	else 
	{
		// We’re not at the beginning of the bucket, so
		// just move down.
		--mIt;
	}
}

template<typename Key, typename T, typename Compare, typename Hash>
bool Nequeo::Collections::Pool::HashIterator<Key, T, Compare, Hash>::operator==(const HashIterator& rhs) const
{
	// All fields, including the hashmap to which the iterators refer,
	// must be equal.
	return (mHashmap == rhs.mHashmap && mBucket == rhs.mBucket && mIt == rhs.mIt);
}

template<typename Key, typename T, typename Compare, typename Hash>
bool Nequeo::Collections::Pool::HashIterator<Key, T, Compare, Hash>::operator!=(const HashIterator& rhs) const
{
	return (!operator==(rhs));
}