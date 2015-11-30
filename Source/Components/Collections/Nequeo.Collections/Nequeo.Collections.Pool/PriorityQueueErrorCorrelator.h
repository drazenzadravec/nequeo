/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          PriorityQueueErrorCorrelator.h
 *  Purpose :       
 *					Template class PriorityQueueErrorCorrelator
 *
 *					Provides Sample Error class with just a priority and a string error description.
 *					
 *					Single failures on a system can often cause multiple errors to be generated from different components. A
					good error-handling system uses error correlation to avoid processing duplicate errors and to process the
					most important errors first. You can use a priority_queue to write a very simple error correlator. This
					class simply sorts events according to their priority, so that the highest-priority errors are always processed
					first.

					Like the queue, the stack provides push() and pop(). The difference is that push() adds a new element
					to the top of the stack, “pushing down” all elements inserted earlier, and pop() removes the element
					from the top of the stack, which is the most recently inserted element. The top() method returns
					a const reference to the top element if called on a const object and a non-const reference if called on a
					non-const object.
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
			///	Provides Sample Error class with just a priority and a string error description.
			///	</summary>
			class PriorityQueueErrorCorrelator
			{
				public:
					PriorityQueueErrorCorrelator(int priority, std::string errMsg) : mPriority(priority), mError(errMsg) 
					{}

					int getPriority() const 
					{
						return mPriority; 
					}

					std::string getErrorString() const 
					{
						return mError; 
					}

					friend bool operator<(const PriorityQueueErrorCorrelator& lhs, const PriorityQueueErrorCorrelator& rhs);
					friend std::ostream& operator<<(std::ostream& str, const PriorityQueueErrorCorrelator& err);
					
				protected:
					int mPriority;
					std::string mError;
					
			};

			///	<summary>
			///	Simple ErrorCorrelator class that returns highest priority errors first.
			///	</summary>
			class ErrorCorrelator
			{
				public:
					ErrorCorrelator() 
					{}

					// Add an error to be correlated.
					void addError(const PriorityQueueErrorCorrelator& error);

					// Retrieve the next error to be processed.
					PriorityQueueErrorCorrelator getError() throw (std::out_of_range);

				protected:
					std::priority_queue<PriorityQueueErrorCorrelator> mErrors;
					std::stack<PriorityQueueErrorCorrelator> mStackErrors;

				private:
					// Prevent assignment and pass-by-reference.
					ErrorCorrelator(const ErrorCorrelator& src);
					ErrorCorrelator& operator=(const ErrorCorrelator& rhs);

			};
		}
	}
}