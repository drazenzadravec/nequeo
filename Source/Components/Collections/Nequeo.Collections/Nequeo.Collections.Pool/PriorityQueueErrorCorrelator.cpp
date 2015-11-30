/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          PriorityQueueErrorCorrelator.cpp
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

#include "PriorityQueueErrorCorrelator.h"

bool Nequeo::Collections::Pool::operator<(const PriorityQueueErrorCorrelator& lhs, const PriorityQueueErrorCorrelator& rhs)
{
	return (lhs.mPriority < rhs.mPriority);
}

ostream& Nequeo::Collections::Pool::operator<<(ostream& str, const PriorityQueueErrorCorrelator& err)
{
	str << err.mError << " (priority " << err.mPriority << ")";
	return (str);
}

void Nequeo::Collections::Pool::ErrorCorrelator::addError(const PriorityQueueErrorCorrelator& error)
{
	mErrors.push(error);
}

Nequeo::Collections::Pool::PriorityQueueErrorCorrelator Nequeo::Collections::Pool::ErrorCorrelator::getError() throw (out_of_range)
{
	// If there are no more errors, throw an exception.
	if (mErrors.empty()) 
	{
		throw (out_of_range("No elements!"));
	}

	// Save the top element.
	PriorityQueueErrorCorrelator top = mErrors.top();

	// Remove the top element.
	mErrors.pop();

	// Return the saved element.
	return (top);
}