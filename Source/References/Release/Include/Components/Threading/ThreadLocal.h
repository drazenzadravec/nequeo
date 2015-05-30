/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TLSAbstractSlot.h
*  Purpose :       TLSAbstractSlot class.
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

#ifndef _TLSABSTRACTSLOT_H
#define _TLSABSTRACTSLOT_H

#include "GlobalThreading.h"

#include <map>

namespace Nequeo {
	namespace Threading
	{
		/// This is the base class for all objects
		/// that the ThreadLocalStorage class manages.
		class TLSAbstractSlot
		{
		public:
			TLSAbstractSlot();
			virtual ~TLSAbstractSlot();
		};

		/// The Slot template wraps another class
		/// so that it can be stored in a ThreadLocalStorage
		/// object. This class is used internally, and you
		/// must not create instances of it yourself.
		template <class C>
		class TLSSlot : public TLSAbstractSlot
		{
		public:
			TLSSlot() :
				_value()
			{
			}

			~TLSSlot()
			{
			}

			C& value()
			{
				return _value;
			}

		private:
			TLSSlot(const TLSSlot&);
			TLSSlot& operator = (const TLSSlot&);

			C _value;
		};

		/// This class manages the local storage for each thread.
		/// Never use this class directly, always use the
		/// ThreadLocal template for managing thread local storage.
		class ThreadLocalStorage
		{
		public:
			ThreadLocalStorage();
			/// Creates the TLS.

			~ThreadLocalStorage();
			/// Deletes the TLS.

			TLSAbstractSlot*& get(const void* key);
			/// Returns the slot for the given key.

			static ThreadLocalStorage& current();
			/// Returns the TLS object for the current thread
			/// (which may also be the main thread).

			static void clear();
			/// Clears the current thread's TLS object.
			/// Does nothing in the main thread.

		private:
			typedef std::map<const void*, TLSAbstractSlot*> TLSMap;

			TLSMap _map;

			friend class Thread;
		};

		/// This template is used to declare type safe thread
		/// local variables. It can basically be used like
		/// a smart pointer class with the special feature
		/// that it references a different object
		/// in every thread. The underlying object will
		/// be created when it is referenced for the first
		/// time.
		/// See the NestedDiagnosticContext class for an
		/// example how to use this template.
		/// Every thread only has access to its own
		/// thread local data. There is no way for a thread
		/// to access another thread's local data.
		template <class C>
		class ThreadLocal
		{
			typedef TLSSlot<C> Slot;

		public:
			ThreadLocal()
			{
			}

			~ThreadLocal()
			{
			}

			C* operator -> ()
			{
				return &get();
			}

			C& operator * ()
				/// "Dereferences" the smart pointer and returns a reference
				/// to the underlying data object. The reference can be used
				/// to modify the object.
			{
				return get();
			}

			C& get()
				/// Returns a reference to the underlying data object.
				/// The reference can be used to modify the object.
			{
				TLSAbstractSlot*& p = ThreadLocalStorage::current().get(this);
				if (!p) p = new Slot;
				return static_cast<Slot*>(p)->value();
			}

		private:
			ThreadLocal(const ThreadLocal&);
			ThreadLocal& operator = (const ThreadLocal&);
		};
	}
}
#endif
