/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          AtomicCounter.cpp
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

#include "stdafx.h"

#include "AtomicCounter.h"

/// Constructor.
Nequeo::AtomicCounter::AtomicCounter() : _disposed(false), _counter(0)
{
}

/// Destructor.
Nequeo::AtomicCounter::~AtomicCounter()
{
	// If not disposed.
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// Copy
Nequeo::AtomicCounter::AtomicCounter(const Nequeo::AtomicCounter& counter) : _disposed(false), _counter(counter.value())
{
}

/// Creates a new AtomicCounter and initializes it with the given value.
Nequeo::AtomicCounter::AtomicCounter(Nequeo::AtomicCounter::ValueType initialValue) : _disposed(false), _counter(initialValue)
{
}

/// Get the current counter.
Nequeo::AtomicCounter& Nequeo::AtomicCounter::operator = (const Nequeo::AtomicCounter& counter)
{
	InterlockedExchange(&_counter, counter.value());
	return *this;
}

/// Get the current counter.
Nequeo::AtomicCounter& Nequeo::AtomicCounter::operator = (Nequeo::AtomicCounter::ValueType value)
{
	InterlockedExchange(&_counter, value);
	return *this;
}

/// Get the counter.
inline Nequeo::AtomicCounter::operator Nequeo::AtomicCounter::ValueType() const
{
	return _counter;
}

/// Get the value.
inline Nequeo::AtomicCounter::ValueType Nequeo::AtomicCounter::value() const
{
	return _counter;
}

/// Increment
inline Nequeo::AtomicCounter::ValueType Nequeo::AtomicCounter::operator ++ ()
{
	return InterlockedIncrement(&_counter);
}

/// Increment
inline Nequeo::AtomicCounter::ValueType Nequeo::AtomicCounter::operator ++ (int)
{
	ValueType result = InterlockedIncrement(&_counter);
	return --result;
}

/// Decrement.
inline Nequeo::AtomicCounter::ValueType Nequeo::AtomicCounter::operator -- ()
{
	return InterlockedDecrement(&_counter);
}

/// Decrement.
inline Nequeo::AtomicCounter::ValueType Nequeo::AtomicCounter::operator -- (int)
{
	ValueType result = InterlockedDecrement(&_counter);
	return ++result;
}

/// Not.
inline bool Nequeo::AtomicCounter::operator ! () const
{
	return _counter == 0;
}