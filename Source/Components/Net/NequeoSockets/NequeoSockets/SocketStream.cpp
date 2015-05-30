/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          SocketStream.cpp
*  Purpose :       SocketStream class.
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

#include "SocketStream.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			SocketStreamBuffer::SocketStreamBuffer(const Socket& socket) :
				Nequeo::IO::BufferedBidirectionalStream(STREAM_BUFFER_SIZE, std::ios::in | std::ios::out),
				_pImpl(dynamic_cast<StreamSocketProvider*>(socket.impl()))
			{
				if (_pImpl)
					_pImpl->duplicate();
				else
					throw Nequeo::Exceptions::InvalidArgumentException("Invalid or null SocketProvider passed to SocketStreamBuffer");
			}


			SocketStreamBuffer::~SocketStreamBuffer()
			{
				_pImpl->release();
			}


			int SocketStreamBuffer::readFromDevice(char* buffer, std::streamsize length)
			{
				return _pImpl->receiveBytes(buffer, (int)length);
			}


			int SocketStreamBuffer::writeToDevice(const char* buffer, std::streamsize length)
			{
				return _pImpl->sendBytes(buffer, (int)length);
			}


			SocketIOS::SocketIOS(const Socket& socket) :
				_buf(socket)
			{
				nequeo_ios_init(&_buf);
			}


			SocketIOS::~SocketIOS()
			{
				try
				{
					_buf.sync();
				}
				catch (...)
				{
				}
			}


			SocketStreamBuffer* SocketIOS::rdbuf()
			{
				return &_buf;
			}


			void SocketIOS::close()
			{
				_buf.sync();
				_buf.socketImpl()->close();
			}


			StreamSocket SocketIOS::socket() const
			{
				return StreamSocket(_buf.socketImpl());
			}


			SocketOutputStream::SocketOutputStream(const Socket& socket) :
				SocketIOS(socket),
				std::ostream(&_buf)
			{
			}


			SocketOutputStream::~SocketOutputStream()
			{
			}

			SocketInputStream::SocketInputStream(const Socket& socket) :
				SocketIOS(socket),
				std::istream(&_buf)
			{
			}


			SocketInputStream::~SocketInputStream()
			{
			}


			SocketStream::SocketStream(const Socket& socket) :
				SocketIOS(socket),
				std::iostream(&_buf)
			{
			}

			SocketStream::~SocketStream()
			{
			}
		}
	}
}