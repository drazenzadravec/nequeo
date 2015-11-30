/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          DefaultHash.h
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
			///	This hashmap implementation uses chained hashing (also called open hashing) 
			/// and does not attempt to provide advanced features like rehashing.
			///	</summary>
			template <typename T>
			class DefaultHash
			{
				public:
					// Throws invalid_argument if numBuckets is nonpositive
					DefaultHash(int numBuckets = 101) throw (invalid_argument);

					int hash(const T& key) const;
					int numBuckets() const { return mNumBuckets; }
					
				protected:
					int mNumBuckets;

			};

			///	<summary>
			/// Unfortunately, the preceding method doesn’t work on strings because different string objects can
			/// contain the same string value. Thus, the same string value could hash to different buckets. Therefore,
			/// it’s also a good idea to provide a partial specialization of the DefaultHash class for strings:
			///	</summary>
			template <>
			class DefaultHash<string>
			{
				public:
					// Throws invalid_argument if numBuckets is nonpositive
					DefaultHash(int numBuckets = 101) throw (invalid_argument);

					int hash(const string& key) const;
					int numBuckets() const { return mNumBuckets; }

				protected:
					int mNumBuckets;

			};

			///	<summary>
			/// A hashmap supports three basic operations: insertion, deletion, and lookup. Of course, it provides a constructor,
			/// destructor, copy constructor, and assignment operator as well. Here is the public portion of the
			/// hashmap class template:
			///	</summary>
			template <typename Key, typename T, typename Compare = std::equal_to<Key>, typename Hash = DefaultHash<Key> >
			class hashmap
			{
				public:
					// The iterator class needs access to protected members of the hashmap.
					//friend class HashIterator<Key, T, Compare, Hash>;

					typedef Key key_type;
					typedef T mapped_type;
					typedef pair<const Key, T> value_type;
					typedef Compare key_compare;

					// STL Typedef Container Requirements.
					typedef pair<const Key, T>& reference;
					typedef const pair<const Key, T>& const_reference;
					//typedef HashIterator<Key, T, Compare, Hash> iterator_hash;
					//typedef HashIterator<Key, T, Compare, Hash> const_iterator_hash;
					typedef size_t size_type;
					typedef ptrdiff_t difference_type;

					// Required class definition for associative containers
					class value_compare : public std::binary_function<value_type, value_type, bool>
					{
						friend class hashmap<Key, T, Compare, Hash>;
						public:
							bool operator() (const value_type& x, const value_type& y) const
							{
								return comp(x.first, y.first);
							}
						protected:
							Compare comp;
							value_compare(Compare c) : comp(c) {}
					};

					// Constructors
					// Throws invalid_argument if the hash object specifies a nonpositive
					// number of buckets
					explicit hashmap(const Compare& comp = Compare(), const Hash& hash = Hash()) throw(invalid_argument);

					template <class InputIterator>
					hashmap(InputIterator first, InputIterator last, const Compare& comp = Compare(), const Hash& hash = Hash()) throw(invalid_argument);

					// destructor, copy constructor, assignment operator
					~hashmap();
					hashmap(const hashmap<Key, T, Compare, Hash>& src);
					hashmap<Key, T, Compare, Hash>& operator=(const hashmap<Key, T, Compare, Hash>& rhs);

					// Element insert
					// Inserts the key/value pair x
					void insert(const value_type& x);

					// Element delete
					// Removes the element with key x, if it exists
					void erase(const key_type& x);

					// Element lookup
					// find returns a pointer to the element with key x.
					// Returns NULL if no element with that key exists.
					value_type* find(const key_type& x);

					// operator[] finds the element with key x or inserts an
					// element with that key if none exists yet. Returns a reference to the
					// value corresponding to that key.
					T& operator[] (const key_type& x);

					// STL Method Container Requirements.
					bool empty() const;
					size_type size() const;
					size_type max_size() const;
					// Other modifying utilities
					void swap(hashmap<Key, T, Compare, Hash>& hashIn);

					// Iterator methods
					//iterator_hash begin();
					//iterator_hash end();
					//const_iterator_hash begin() const;
					//const_iterator_hash end() const;

				protected:
					typedef list<value_type> ListType;

					typename list<pair<const Key, T> >::iterator findElement(const key_type& x, int& bucket) const;

					// In this first implementation, it would be easier to use a vector
					// instead of a pointer to a vector, which requires dynamic allocation.
					// However, we use a ptr to a vector so that, in the final
					// implementation, swap() can be implemented in constant time.
					vector<ListType>* mElems;
					int mSize;
					Compare mComp;
					Hash mHash;

			};

			///	<summary>
			/// HashIterator class definition
			///	</summary>
			template<typename Key, typename T, typename Compare, typename Hash>
			class HashIterator : public std::iterator<std::bidirectional_iterator_tag, pair<const Key, T> >
			{
				public:
					// Bidirectional iterators must supply default ctors.
					HashIterator(); 
					HashIterator(int bucket, typename list<pair<const Key, T> >::iterator listIt, const hashmap<Key, T, Compare, Hash>* inHashmap);
			
					pair<const Key, T>& operator*() const;

					// Return type must be something to which -> can be applied.
					// Return a pointer to a pair<const Key, T>, to which the compiler will
					// apply -> again.
					pair<const Key, T>* operator->() const;

					HashIterator<Key, T, Compare, Hash>& operator++();
					const HashIterator<Key, T, Compare, Hash> operator++(int);
					HashIterator<Key, T, Compare, Hash>& operator--();
					const HashIterator<Key, T, Compare, Hash> operator--(int);

					// Don’t need to define a copy constructor or operator= because the
					// default behavior is what we want
					// Don’t need destructor because the default behavior
					// (not deleting mHashmap) is what we want.
					// These are ok as member functions because we don’t support
					// comparisons of different types to this one.
					bool operator==(const HashIterator& rhs) const;
					bool operator!=(const HashIterator& rhs) const;

				protected:
					int mBucket;
					typename list<pair<const Key, T> >::iterator mIt;
					const hashmap<Key, T, Compare, Hash>* mHashmap;

					// Helper methods for operator++ and operator--
					void increment();
					void decrement();

			};
		}
	}
}