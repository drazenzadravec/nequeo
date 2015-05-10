/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          AtomicCounter.h
*  Purpose :       Atomic counter class.
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

#ifndef _ATOMICCOUNTER_H
#define _ATOMICCOUNTER_H

#include "Global.h"

namespace Nequeo
{
	/// This class implements a simple counter, which
	/// provides atomic operations that are safe to
	/// use in a multithreaded environment.
	///
	/// Typical usage of AtomicCounter is for implementing
	/// reference counting and similar things.
	class AtomicCounter
	{
	public:
		// AtomicCounter class.
		AtomicCounter();

		// Destroys the AtomicCounter and releases resources.
		virtual ~AtomicCounter();

		// Copy.
		AtomicCounter(const AtomicCounter& counter);

		/// The underlying integer type.
		typedef int ValueType;

		// Creates a new AtomicCounter and initializes it with
		// the given value.
		explicit AtomicCounter(ValueType initialValue);

		// Assigns the value of another AtomicCounter.
		AtomicCounter& operator = (const AtomicCounter& counter);

		// Assigns a value to the counter.
		AtomicCounter& operator = (ValueType value);

		// Returns the value of the counter.
		operator ValueType () const;

		// Returns the value of the counter.
		ValueType value() const;

		// Increments the counter and returns the result.
		ValueType operator ++ (); // prefix

		// Increments the counter and returns the previous value.
		ValueType operator ++ (int); // postfix

		// Decrements the counter and returns the result.
		ValueType operator -- (); // prefix

		// Decrements the counter and returns the previous value.
		ValueType operator -- (int); // postfix

		// Returns true if the counter is zero, false otherwise.
		bool operator ! () const;

	private:
		bool _disposed;

		typedef volatile LONG ImplType;
		ImplType _counter;
	};
}
#endif