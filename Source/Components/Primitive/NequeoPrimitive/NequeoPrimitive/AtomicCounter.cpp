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
Nequeo::Primitive::AtomicCounter::AtomicCounter() : _disposed(false), _counter(0)
{
}

/// Destructor.
Nequeo::Primitive::AtomicCounter::~AtomicCounter()
{
	// If not disposed.
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// Copy
Nequeo::Primitive::AtomicCounter::AtomicCounter(const Nequeo::Primitive::AtomicCounter& counter) : _disposed(false), _counter(counter.value())
{
}

/// Creates a new AtomicCounter and initializes it with the given value.
Nequeo::Primitive::AtomicCounter::AtomicCounter(Nequeo::Primitive::AtomicCounter::ValueType initialValue) : _disposed(false), _counter(initialValue)
{
}

/// Get the current counter.
Nequeo::Primitive::AtomicCounter& Nequeo::Primitive::AtomicCounter::operator = (const Nequeo::Primitive::AtomicCounter& counter)
{
	InterlockedExchange(&_counter, counter.value());
	return *this;
}

/// Get the current counter.
Nequeo::Primitive::AtomicCounter& Nequeo::Primitive::AtomicCounter::operator = (Nequeo::Primitive::AtomicCounter::ValueType value)
{
	InterlockedExchange(&_counter, value);
	return *this;
}

/// Get the counter.
inline Nequeo::Primitive::AtomicCounter::operator Nequeo::Primitive::AtomicCounter::ValueType() const
{
	return _counter;
}

/// Get the value.
inline Nequeo::Primitive::AtomicCounter::ValueType Nequeo::Primitive::AtomicCounter::value() const
{
	return _counter;
}

/// Increment
inline Nequeo::Primitive::AtomicCounter::ValueType Nequeo::Primitive::AtomicCounter::operator ++ ()
{
	return InterlockedIncrement(&_counter);
}

/// Increment
inline Nequeo::Primitive::AtomicCounter::ValueType Nequeo::Primitive::AtomicCounter::operator ++ (int)
{
	ValueType result = InterlockedIncrement(&_counter);
	return --result;
}

/// Decrement.
inline Nequeo::Primitive::AtomicCounter::ValueType Nequeo::Primitive::AtomicCounter::operator -- ()
{
	return InterlockedDecrement(&_counter);
}

/// Decrement.
inline Nequeo::Primitive::AtomicCounter::ValueType Nequeo::Primitive::AtomicCounter::operator -- (int)
{
	ValueType result = InterlockedDecrement(&_counter);
	return ++result;
}

/// Not.
inline bool Nequeo::Primitive::AtomicCounter::operator ! () const
{
	return _counter == 0;
}