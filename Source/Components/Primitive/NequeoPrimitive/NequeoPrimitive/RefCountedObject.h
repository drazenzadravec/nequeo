/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          RefCountedObject.h
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

#pragma once

#ifndef _REFCOUNTEDOBJECT_H
#define _REFCOUNTEDOBJECT_H

#include "GlobalPrimitive.h"

#include "AtomicCounter.h"

namespace Nequeo {
	namespace Primitive
	{
		/// A base class for objects that employ
		/// reference counting based garbage collection.
		///
		/// Reference-counted objects inhibit construction
		/// by copying and assignment.
		class RefCountedObject
		{
		public:
			// RefCountedObject class.
			RefCountedObject();

			/// Increments the object's reference count.
			void duplicate() const;
			
			/// Decrements the object's reference count
			/// and deletes the object if the count
			/// reaches zero.
			void release() const;
			
			/// Returns the reference count.
			int referenceCount() const;
			
		protected:
			// Destroys the RefCountedObject and releases resources.
			virtual ~RefCountedObject();

		private:
			bool _disposed;

			RefCountedObject(const RefCountedObject&);
			RefCountedObject& operator = (const RefCountedObject&);

			// Counter.
			mutable AtomicCounter _counter;
		};
	}
}
#endif