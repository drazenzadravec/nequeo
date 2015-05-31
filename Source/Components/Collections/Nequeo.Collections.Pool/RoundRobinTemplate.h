/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          RoundRobinTemplate.h
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
			///	Provides simple round-robin semantics for a list of elements.
			///	</summary>
			template <typename T>
			class RoundRobinTemplate
			{
				public:
					
					// Client can give a hint as to the number of expected elements for
					// increased efficiency.
					RoundRobinTemplate(int numExpected = 0);
					virtual ~RoundRobinTemplate();

					// Appends elem to the end of the list. May be called
					// between calls to getNext().
					void add(const T& elem);

					// Removes the first (and only the first) element
					// in the list that is equal (with operator==) to elem.
					// May be called between calls to getNext().
					void remove(const T& elem);

					// Returns the next element in the list, starting from 0 and continuously
					// cycling, taking into account elements that are added or removed.
					T& getNext() throw(std::out_of_range);

				protected:
			
					vector<T> mElems;
					typename std::vector<T>::iterator mCurElem;

				private:
					// Prevent assignment and pass-by-reference.
					RoundRobinTemplate(const RoundRobinTemplate& src);
					RoundRobinTemplate& operator=(const RoundRobinTemplate& rhs);

			};
		}
	}
}