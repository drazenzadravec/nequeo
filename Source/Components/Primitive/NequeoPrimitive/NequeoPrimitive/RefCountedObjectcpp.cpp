/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          RefCountedObject.cpp
*  Purpose :       Ref Counted Object class.
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

#include "RefCountedObject.h"

/// Constructor.
Nequeo::Primitive::RefCountedObject::RefCountedObject() : _disposed(false), _counter(1)
{
}

/// Destructor.
Nequeo::Primitive::RefCountedObject::~RefCountedObject()
{
	// If not disposed.
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// Get the current count.
inline int Nequeo::Primitive::RefCountedObject::referenceCount() const
{
	return _counter.value();
}

/// Duplicate the counter.
inline void Nequeo::Primitive::RefCountedObject::duplicate() const
{
	++_counter;
}

/// Release the current counter.
inline void Nequeo::Primitive::RefCountedObject::release() const
{
	if (--_counter == 0) delete this;
}
