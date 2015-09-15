/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          AbstractEvent.h
*  Purpose :       AbstractEvent class.
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

#ifndef _ABSTRACTEVENT_H
#define _ABSTRACTEVENT_H

#include "GlobalThreading.h"

#include "SingletonHolder.h"
#include "Base\SharedPtr.h"
#include "ActiveResult.h"
#include "ActiveMethod.h"
#include "Mutex.h"

namespace Nequeo {
	namespace Threading {
		namespace Events
		{
			/// An AbstractEvent is the base class of all events. 
			/// It works similar to the way C# handles notifications (aka events in C#).
			template <class TArgs, class TStrategy, class TDelegate, class TMutex = FastMutex>
			class AbstractEvent
				/// Events can be used to send information to a set of delegates
				/// which are registered with the event. The type of the data is specified with
				/// the template parameter TArgs. The TStrategy parameter must be a subclass 
				/// of NotificationStrategy. The parameter TDelegate can either be a subclass of AbstractDelegate
				/// or of AbstractPriorityDelegate. 
				///
				/// Note that AbstractEvent should never be used directly. One ought to use
				/// one of its subclasses which set the TStrategy and TDelegate template parameters
				/// to fixed values. For most use-cases the BasicEvent template will be sufficient:
				///
				///     #include "BasicEvent.h"
				///     #include "Delegate.h"
				///
				/// Note that as of release 1.4.2, the behavior of BasicEvent equals that of FIFOEvent,
				/// so the FIFOEvent class is no longer necessary and provided for backwards compatibility
				/// only.
				///
				/// BasicEvent works with a standard delegate. They allow one object to register
				/// onr or more delegates with an event. In contrast, a PriorityDelegate comes with an attached priority value
				/// and allows one object to register for one priority value one or more delegates. Note that PriorityDelegates
				/// only work with PriorityEvents:
				///
				///     #include "PriorityEvent.h"
				///     #include "PriorityDelegate.h"
				///
				/// Use events by adding them as public members to the object which is throwing notifications:
				///
				///     class MyData
				///     {
				///     public:
				///         Poco::BasicEvent<int> dataChanged;
				///         
				///         MyData();
				///         ...
				///         void setData(int i);
				///         ...
				///     private:
				///         int _data;
				///     };
				///
				/// Firing the event is done either by calling the event's notify() or notifyAsync() method:
				///
				///     void MyData::setData(int i)
				///     {
				///         this->_data = i;
				///         dataChanged.notify(this, this->_data);
				///     }
				///
				/// Alternatively, instead of notify(), operator () can be used.
				///
				///     void MyData::setData(int i)
				///     {
				///         this->_data = i;
				///         dataChanged(this, this->_data);
				///     }
				///
				/// Note that operator (), notify() and notifyAsync() do not catch exceptions, i.e. in case a  
				/// delegate throws an exception, notifying is immediately aborted and the exception is propagated
				/// back to the caller.
				///
				/// Delegates can register methods at the event. In the case of a BasicEvent
				/// the Delegate template is used, in case of an PriorityEvent a PriorityDelegate is used.
				/// Mixing of delegates, e.g. using a PriorityDelegate with a BasicEvent is not allowed and
				/// can lead to compile-time and/or run-time errors. The standalone delegate() functions
				/// can be used to construct Delegate objects.
				///
				/// Events require the observers to have one of the following method signatures:
				///
				///     void onEvent(const void* pSender, TArgs& args);
				///     void onEvent(TArgs& args);
				///     static void onEvent(const void* pSender, TArgs& args);
				///     static void onEvent(void* pSender, TArgs& args);
				///     static void onEvent(TArgs& args);
				///
				/// For performance reasons arguments are always sent by reference. This also allows observers
				/// to modify the event argument. To prevent that, use <[const TArg]> as template
				/// parameter. A non-conformant method signature leads to compile errors.
				///
				/// Assuming that the observer meets the method signature requirement, it can register
				/// this method with the += operator:
				///
				///     class MyController
				///     {
				///     protected:
				///         MyData _data;
				///         
				///         void onDataChanged(void* pSender, int& data);
				///         ...
				///     };
				///         
				///     MyController::MyController()
				///     {
				///         _data.dataChanged += delegate(this, &MyController::onDataChanged);
				///     }
				///
				/// In some cases it might be desirable to work with automatically expiring registrations. Simply add
				/// to delegate as 3rd parameter a expireValue (in milliseconds):
				///
				///     _data.dataChanged += delegate(this, &MyController::onDataChanged, 1000);
				///
				/// This will add a delegate to the event which will automatically be removed in 1000 millisecs.
				///
				/// Unregistering happens via the -= operator. Forgetting to unregister a method will lead to
				/// segmentation faults later, when one tries to send a notify to a no longer existing object.
				///
				///     MyController::~MyController()
				///     {
				///         _data.dataChanged -= delegate(this, &MyController::onDataChanged);
				///     }
				///
				/// Working with PriorityDelegate's as similar to working with BasicEvent.
				/// Instead of delegate(), the priorityDelegate() function must be used
				/// to create the PriorityDelegate.
			{
			public:
				typedef TArgs Args;

				AbstractEvent() :
					_executeAsync(this, &AbstractEvent::executeAsyncImpl),
					_enabled(true)
				{
				}

				AbstractEvent(const TStrategy& strat) :
					_executeAsync(this, &AbstractEvent::executeAsyncImpl),
					_strategy(strat),
					_enabled(true)
				{
				}

				virtual ~AbstractEvent()
				{
				}

				void operator += (const TDelegate& aDelegate)
					/// Adds a delegate to the event. 
					///
					/// Exact behavior is determined by the TStrategy.
				{
					typename TMutex::ScopedLock lock(_mutex);
					_strategy.add(aDelegate);
				}

				void operator -= (const TDelegate& aDelegate)
					/// Removes a delegate from the event.
					///
					/// If the delegate is not found, this function does nothing.
				{
					typename TMutex::ScopedLock lock(_mutex);
					_strategy.remove(aDelegate);
				}

				void operator () (const void* pSender, TArgs& args)
					/// Shortcut for notify(pSender, args);
				{
					notify(pSender, args);
				}

				void operator () (TArgs& args)
					/// Shortcut for notify(args).
				{
					notify(0, args);
				}

				void notify(const void* pSender, TArgs& args)
					/// Sends a notification to all registered delegates. The order is 
					/// determined by the TStrategy. This method is blocking. While executing,
					/// the list of delegates may be modified. These changes don't
					/// influence the current active notifications but are activated with
					/// the next notify. If a delegate is removed during a notify(), the
					/// delegate will no longer be invoked (unless it has already been
					/// invoked prior to removal). If one of the delegates throws an exception, 
					/// the notify method is immediately aborted and the exception is propagated
					/// to the caller.
				{
					ScopedLockWithUnlock<TMutex> lock(_mutex);

					if (!_enabled) return;

					// thread-safeness: 
					// copy should be faster and safer than blocking until
					// execution ends
					TStrategy strategy(_strategy);
					lock.unlock();
					strategy.notify(pSender, args);
				}

				ActiveResult<TArgs> notifyAsync(const void* pSender, const TArgs& args)
					/// Sends a notification to all registered delegates. The order is 
					/// determined by the TStrategy. This method is not blocking and will
					/// immediately return. The delegates are invoked in a seperate thread.
					/// Call activeResult.wait() to wait until the notification has ended.
					/// While executing, other objects can change the delegate list. These changes don't
					/// influence the current active notifications but are activated with
					/// the next notify. If a delegate is removed during a notify(), the
					/// delegate will no longer be invoked (unless it has already been
					/// invoked prior to removal). If one of the delegates throws an exception, 
					/// the execution is aborted and the exception is propagated to the caller.
				{
					NotifyAsyncParams params(pSender, args);
					{
						typename TMutex::ScopedLock lock(_mutex);

						// thread-safeness: 
						// copy should be faster and safer than blocking until
						// execution ends
						// make a copy of the strategy here to guarantee that
						// between notifyAsync and the execution of the method no changes can occur

						params.ptrStrat = SharedPtr<TStrategy>(new TStrategy(_strategy));
						params.enabled = _enabled;
					}
					ActiveResult<TArgs> result = _executeAsync(params);
					return result;
				}

				void enable()
					/// Enables the event.
				{
					typename TMutex::ScopedLock lock(_mutex);
					_enabled = true;
				}

				void disable()
					/// Disables the event. notify and notifyAsnyc will be ignored,
					/// but adding/removing delegates is still allowed.
				{
					typename TMutex::ScopedLock lock(_mutex);
					_enabled = false;
				}

				bool isEnabled() const
				{
					typename TMutex::ScopedLock lock(_mutex);
					return _enabled;
				}

				void clear()
					/// Removes all delegates.
				{
					typename TMutex::ScopedLock lock(_mutex);
					_strategy.clear();
				}

				bool empty() const
					/// Checks if any delegates are registered at the delegate.
				{
					typename TMutex::ScopedLock lock(_mutex);
					return _strategy.empty();
				}

			protected:
				struct NotifyAsyncParams
				{
					SharedPtr<TStrategy> ptrStrat;
					const void* pSender;
					TArgs       args;
					bool        enabled;

					NotifyAsyncParams(const void* pSend, const TArgs& a) :ptrStrat(), pSender(pSend), args(a), enabled(true)
						/// Default constructor reduces the need for TArgs to have an empty constructor, only copy constructor is needed.
					{
					}
				};

				ActiveMethod<TArgs, NotifyAsyncParams, AbstractEvent> _executeAsync;

				TArgs executeAsyncImpl(const NotifyAsyncParams& par)
				{
					if (!par.enabled)
					{
						return par.args;
					}

					NotifyAsyncParams params = par;
					TArgs retArgs(params.args);
					params.ptrStrat->notify(params.pSender, retArgs);
					return retArgs;
				}

				TStrategy _strategy; /// The strategy used to notify observers.
				bool      _enabled;  /// Stores if an event is enabled. Notfies on disabled events have no effect
				/// but it is possible to change the observers.
				mutable TMutex _mutex;

			private:
				AbstractEvent(const AbstractEvent& other);
				AbstractEvent& operator = (const AbstractEvent& other);
			};
		}
	}
}
#endif
