/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TCPServerDispatcher.cpp
*  Purpose :       TCPServerDispatcher class.
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

#include "TCPServerDispatcher.h"
#include "TCPServerConnectionFactory.h"
#include "Threading\Notification.h"
#include "Base\AutoPtr.h"
#include <memory>

using Nequeo::Threading::Notifications::Notification;
using Nequeo::Threading::FastMutex;
using Nequeo::AutoPtr;

namespace Nequeo {
	namespace Net {
		namespace Provider
		{
			class TCPConnectionNotification : public Notification
			{
			public:
				TCPConnectionNotification(const StreamSocket& socket) :
					_socket(socket)
				{
				}

				~TCPConnectionNotification()
				{
				}

				const StreamSocket& socket() const
				{
					return _socket;
				}

			private:
				StreamSocket _socket;
			};


			TCPServerDispatcher::TCPServerDispatcher(TCPServerConnectionFactory::Ptr pFactory, Nequeo::Threading::ThreadPool& threadPool, TCPServerParams::Ptr pParams) :
				_rc(1),
				_pParams(pParams),
				_currentThreads(0),
				_totalConnections(0),
				_currentConnections(0),
				_maxConcurrentConnections(0),
				_refusedConnections(0),
				_stopped(false),
				_pConnectionFactory(pFactory),
				_threadPool(threadPool)
			{
				if (!_pParams)
					_pParams = new TCPServerParams;

				if (_pParams->getMaxThreads() == 0)
					_pParams->setMaxThreads(threadPool.capacity());
			}


			TCPServerDispatcher::~TCPServerDispatcher()
			{
			}


			void TCPServerDispatcher::duplicate()
			{
				_mutex.lock();
				++_rc;
				_mutex.unlock();
			}


			void TCPServerDispatcher::release()
			{
				_mutex.lock();
				int rc = --_rc;
				_mutex.unlock();
				if (rc == 0) delete this;
			}


			void TCPServerDispatcher::run()
			{
				AutoPtr<TCPServerDispatcher> guard(this, true); // ensure object stays alive

				int idleTime = (int)_pParams->getThreadIdleTime().totalMilliseconds();

				for (;;)
				{
					AutoPtr<Notification> pNf = _queue.waitDequeueNotification(idleTime);
					if (pNf)
					{
						TCPConnectionNotification* pCNf = dynamic_cast<TCPConnectionNotification*>(pNf.get());
						if (pCNf)
						{
							std::auto_ptr<TCPServerConnection> pConnection(_pConnectionFactory->createConnection(pCNf->socket()));
							
							beginConnection();
							pConnection->start();
							endConnection();
						}
					}

					FastMutex::ScopedLock lock(_mutex);
					if (_stopped || (_currentThreads > 1 && _queue.empty()))
					{
						--_currentThreads;
						break;
					}
				}
			}


			namespace
			{
				static const std::string threadName("TCPServerConnection");
			}


			void TCPServerDispatcher::enqueue(const StreamSocket& socket)
			{
				FastMutex::ScopedLock lock(_mutex);

				if (_queue.size() < _pParams->getMaxQueued())
				{
					_queue.enqueueNotification(new TCPConnectionNotification(socket));
					if (!_queue.hasIdleThreads() && _currentThreads < _pParams->getMaxThreads())
					{
						try
						{
							_threadPool.startWithPriority(_pParams->getThreadPriority(), *this, threadName);
							++_currentThreads;
						}
						catch (Nequeo::Exception&)
						{
							// no problem here, connection is already queued
							// and a new thread might be available later.
						}
					}
				}
				else
				{
					++_refusedConnections;
				}
			}


			void TCPServerDispatcher::stop()
			{
				_stopped = true;
				_queue.clear();
				_queue.wakeUpAll();
			}


			int TCPServerDispatcher::currentThreads() const
			{
				FastMutex::ScopedLock lock(_mutex);

				return _currentThreads;
			}


			int TCPServerDispatcher::totalConnections() const
			{
				FastMutex::ScopedLock lock(_mutex);

				return _totalConnections;
			}


			int TCPServerDispatcher::currentConnections() const
			{
				FastMutex::ScopedLock lock(_mutex);

				return _currentConnections;
			}


			int TCPServerDispatcher::maxConcurrentConnections() const
			{
				FastMutex::ScopedLock lock(_mutex);

				return _maxConcurrentConnections;
			}


			int TCPServerDispatcher::queuedConnections() const
			{
				return _queue.size();
			}


			int TCPServerDispatcher::refusedConnections() const
			{
				FastMutex::ScopedLock lock(_mutex);

				return _refusedConnections;
			}


			void TCPServerDispatcher::beginConnection()
			{
				FastMutex::ScopedLock lock(_mutex);

				++_totalConnections;
				++_currentConnections;
				if (_currentConnections > _maxConcurrentConnections)
					_maxConcurrentConnections = _currentConnections;
			}


			void TCPServerDispatcher::endConnection()
			{
				FastMutex::ScopedLock lock(_mutex);

				--_currentConnections;
			}
		}
	}
}