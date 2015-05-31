/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          RoundRobinTemplate.cpp
 *  Purpose :       
 *					Template class RoundRobinTemplate
 *
 *					Provides simple round-robin semantics for a list of elements.
 *					Clients add elements to the end of the list with add().
 *
 *					getNext() returns the next element in the list, starting with the first,
 *					and cycling back to the first when the end of the list is reached.
 *
 *					remove() removes the element matching the argument.
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

#include "RoundRobinTemplate.h"

template <typename T>
Nequeo::Collections::Pool::RoundRobinTemplate<T>::RoundRobinTemplate(int numExpected)
{
	// If the client gave a guideline, reserve that much space.
	mElems.reserve(numExpected);

	// Initialize mCurElem even though it isn’t used until
	// there’s at least one element.
	mCurElem = mElems.begin();
}

template <typename T>
Nequeo::Collections::Pool::RoundRobinTemplate<T>::~RoundRobinTemplate()
{
	// Nothing to do here--the vector will delete all the elements
}

// Always add the new element at the end.
template <typename T>
void Nequeo::Collections::Pool::RoundRobinTemplate<T>::add(const T& elem)
{
	// Even though we add the element at the end,
	// the vector could reallocate and invalidate the iterator.
	// Take advantage of the random access iterator features to save our
	// spot.
	int pos = mCurElem - mElems.begin();

	// Add the element.
	mElems.push_back(elem);

	// If it’s the first element, initialize the iterator to the beginning.
	if (mElems.size() == 1) 
	{
		mCurElem = mElems.begin();
	} 
	else 
	{
		// Set it back to our spot.
		mCurElem = mElems.begin() + pos;
	}
}

template <typename T>
void Nequeo::Collections::Pool::RoundRobinTemplate<T>::remove(const T& elem)
{
	for (typename std::vector<T>::iterator it = mElems.begin(); it != mElems.end(); ++it) 
	{
		if (*it == elem) 
		{
			// Removing an element will invalidate our mCurElem iterator if
			// it refers to an element past the point of the removal.
			// Take advantage of the random access features of the iterator
			// to track the position of the current element after the removal.
			int newPos;

			// If the current iterator is before or at the one we’re removing,
			// the new position is the same as before.
			if (mCurElem <= it) 
			{
				newPos = mCurElem - mElems.begin();
			} 
			else 
			{
				// Otherwise, it’s one less than before.
				newPos = mCurElem - mElems.begin() - 1;
			}

			// Erase the element (and ignore the return value).
			mElems.erase(it);

			// Now reset our iterator.
			mCurElem = mElems.begin() + newPos;

			// If we were pointing to the last element and it was removed,
			// we need to loop back to the first.
			if (mCurElem == mElems.end()) 
			{
				mCurElem = mElems.begin();
			}

			return;
		}
	}
}

template <typename T>
T& Nequeo::Collections::Pool::RoundRobinTemplate<T>::getNext()throw(std::out_of_range)
{
	// First, make sure there are any elements.
	if (mElems.empty()) 
	{
		throw std::out_of_range(“No elements in the list”);
	}

	// Retrieve a reference to return.
	T& retVal = *mCurElem;

	// Increment the iterator modulo the number of elements.
	++mCurElem;

	if (mCurElem == mElems.end()) 
	{
		mCurElem = mElems.begin();
	}

	// Return the reference.
	return (retVal);
}