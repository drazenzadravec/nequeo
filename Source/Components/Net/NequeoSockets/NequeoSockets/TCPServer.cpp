/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TCPServer.cpp
*  Purpose :       TCPServer class.
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

#include "TCPServer.h"
#include "TCPServerDispatcher.h"
#include "TCPServerConnection.h"
#include "TCPServerConnectionFactory.h"
#include "Base\Timespan.h"
#include "Threading\ErrorHandler.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"

using Nequeo::Threading::ErrorHandler;
using Nequeo::Timespan;

namespace Nequeo {
	namespace Net {
		namespace Provider
		{
			TCPServer::TCPServer(TCPServerConnectionFactory::Ptr pFactory, const ServerSocket& socket, TCPServerParams::Ptr pParams) :
				_socket(socket),
				_pDispatcher(new TCPServerDispatcher(pFactory, Nequeo::Threading::ThreadPool::defaultPool(), pParams)),
				_thread(threadName(socket)),
				_stopped(true)
			{
			}

			TCPServer::TCPServer(TCPServerConnectionFactory::Ptr pFactory, Nequeo::Threading::ThreadPool& threadPool, const ServerSocket& socket, TCPServerParams::Ptr pParams) :
				_socket(socket),
				_pDispatcher(new TCPServerDispatcher(pFactory, threadPool, pParams)),
				_thread(threadName(socket)),
				_stopped(true)
			{
			}

			TCPServer::~TCPServer()
			{
				stop();
				_pDispatcher->release();
			}

			const TCPServerParams& TCPServer::params() const
			{
				return _pDispatcher->params();
			}


			void TCPServer::start()
			{
				_stopped = false;
				_thread.start(*this);
			}


			void TCPServer::stop()
			{
				if (!_stopped)
				{
					_stopped = true;
					_thread.join();
					_pDispatcher->stop();
				}
			}


			void TCPServer::run()
			{
				while (!_stopped)
				{
					Timespan timeout(250000);
					if (_socket.poll(timeout, Nequeo::Net::Sockets::SELECT_READ))
					{
						try
						{
							StreamSocket ss = _socket.acceptConnection();
							// enabe nodelay per default: OSX really needs that
							ss.setNoDelay(true);
							_pDispatcher->enqueue(ss);
						}
						catch (Nequeo::Exception& exc)
						{
							ErrorHandler::handle(exc);
						}
						catch (std::exception& exc)
						{
							ErrorHandler::handle(exc);
						}
						catch (...)
						{
							ErrorHandler::handle();
						}
					}
				}
			}


			int TCPServer::currentThreads() const
			{
				return _pDispatcher->currentThreads();
			}


			int TCPServer::totalConnections() const
			{
				return _pDispatcher->totalConnections();
			}


			int TCPServer::currentConnections() const
			{
				return _pDispatcher->currentConnections();
			}


			int TCPServer::maxConcurrentConnections() const
			{
				return _pDispatcher->maxConcurrentConnections();
			}


			int TCPServer::queuedConnections() const
			{
				return _pDispatcher->queuedConnections();
			}


			int TCPServer::refusedConnections() const
			{
				return _pDispatcher->refusedConnections();
			}


			std::string TCPServer::threadName(const ServerSocket& socket)
			{
				std::string name("TCPServer: ");
				name.append(socket.address().toString());
				return name;
			}
		}
	}
}