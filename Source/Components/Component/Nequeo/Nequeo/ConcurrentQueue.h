/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          ConcurrentQueue.h
*  Purpose :       ConcurrentQueue class.
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

#include "Global.h"

#include <queue>

#include <boost/noncopyable.hpp>

#pragma warning( push )
#pragma warning( disable : 4100 )

#include <boost/thread.hpp>

#pragma warning( pop )

namespace Nequeo {
	namespace Collections
	{
		/// <summary>
		/// A ConcurrentQueue class implements a thread-safe first in-first out (FIFO) collection. 
		/// </summary>
		template <typename T>
		class ConcurrentQueue : private boost::noncopyable
		{
		public:
			/// <summary>
			/// Initializes a new instance of the ConcurrentQueue class. 
			/// </summary>
			ConcurrentQueue() : _stopRequested(false) { }

			/// <summary>
			/// Adds an object to the end of the ConcurrentQueue. 
			/// </summary>
			void Push(T value)
			{
				{
					boost::unique_lock<boost::mutex> lock(_mutex);
					_queue.push(value);
				}

				_queueNotEmpty.notify_one();
			}

			/// <summary>
			/// Tries to remove and retrieve the object at the beginning of the ConcurrentQueue. 
			/// </summary>
			bool TryPop(T& value)
			{
				boost::unique_lock<boost::mutex> lock(_mutex);
				if (_queue.empty())
				{
					return false;
				}

				value = _queue.front();
				_queue.pop();

				return true;
			}

			/// <summary>
			/// Removes and retrieves the object at the beginning of the ConcurrentQueue,
			/// waiting if necessary until an object becomes available.
			/// </summary>
			bool WaitAndPop(T &value)
			{
				boost::unique_lock<boost::mutex> lock(_mutex);
				_queueNotEmpty.wait(lock,
					[this] { return !_queue.empty() || _stopRequested; });

				if (_stopRequested)
					return false;

				value = _queue.front();
				_queue.pop();

				return true;
			}

			/// <summary>
			/// Interrupts waiting for a new object. 
			/// </summary>
			void StopWait()
			{
				{
					boost::unique_lock<boost::mutex> lock(_mutex);
					_stopRequested = true;
				}

				_queueNotEmpty.notify_all();
			}

		private:
			bool _stopRequested;
			std::queue<T> _queue;
			mutable boost::mutex _mutex;
			boost::condition_variable _queueNotEmpty;
		};
	}
}