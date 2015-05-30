/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          SocketStream.h
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

#pragma once

#ifndef _SOCKETSTREAM_H
#define _SOCKETSTREAM_H

#include "GlobalSocket.h"
#include "Socket.h"
#include "StreamSocket.h"
#include "SocketProvider.h"
#include "StreamSocketProvider.h"
#include "IO\BufferedBidirectionalStream.h"

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			/// This is the streambuf class used for reading from and writing to a socket.
			class SocketStreamBuffer : public Nequeo::IO::BufferedBidirectionalStream
			{
			public:
				SocketStreamBuffer(const Socket& socket);
				/// Creates a SocketStreamBuf with the given socket.
				///
				/// The socket's SocketImpl must be a StreamSocketImpl,
				/// otherwise an InvalidArgumentException is thrown.

				~SocketStreamBuffer();
				/// Destroys the SocketStreamBuf.

				StreamSocketProvider* socketImpl() const;
				/// Returns the internal SocketImpl.

			protected:
				int readFromDevice(char* buffer, std::streamsize length);
				int writeToDevice(const char* buffer, std::streamsize length);

			private:
				StreamSocketProvider* _pImpl;
			};

			/// The base class for SocketStream, SocketInputStream and
			/// SocketOutputStream.
			///
			/// This class is needed to ensure the correct initialization
			/// order of the stream buffer and base classes.
			class SocketIOS : public virtual std::ios
			{
			public:
				SocketIOS(const Socket& socket);
				/// Creates the SocketIOS with the given socket.
				///
				/// The socket's SocketImpl must be a StreamSocketImpl,
				/// otherwise an InvalidArgumentException is thrown.

				~SocketIOS();
				/// Destroys the SocketIOS.
				///
				/// Flushes the buffer, but does not close the socket.

				SocketStreamBuffer* rdbuf();
				/// Returns a pointer to the internal SocketStreamBuf.

				void close();
				/// Flushes the stream and closes the socket.

				StreamSocket socket() const;
				/// Returns the underlying socket.

			protected:
				SocketStreamBuffer _buf;
			};


			class SocketOutputStream : public SocketIOS, public std::ostream
				/// An output stream for writing to a socket.
			{
			public:
				explicit SocketOutputStream(const Socket& socket);
				/// Creates the SocketOutputStream with the given socket.
				///
				/// The socket's SocketImpl must be a StreamSocketImpl,
				/// otherwise an InvalidArgumentException is thrown.

				~SocketOutputStream();
				/// Destroys the SocketOutputStream.
				///
				/// Flushes the buffer, but does not close the socket.
			};


			class SocketInputStream : public SocketIOS, public std::istream
				/// An input stream for reading from a socket.
				///
				/// When using formatted input from a SocketInputStream,
				/// always ensure that a receive timeout is set for the
				/// socket. Otherwise your program might unexpectedly
				/// hang.
				///
				/// However, using formatted input from a SocketInputStream
				/// is not recommended, due to the read-ahead behavior of
				/// istream with formatted reads.
			{
			public:
				explicit SocketInputStream(const Socket& socket);
				/// Creates the SocketInputStream with the given socket.
				///
				/// The socket's SocketImpl must be a StreamSocketImpl,
				/// otherwise an InvalidArgumentException is thrown.

				~SocketInputStream();
				/// Destroys the SocketInputStream.
			};


			class SocketStream : public SocketIOS, public std::iostream
				/// An bidirectional stream for reading from and writing to a socket.
				///
				/// When using formatted input from a SocketStream,
				/// always ensure that a receive timeout is set for the
				/// socket. Otherwise your program might unexpectedly
				/// hang.
				///
				/// However, using formatted input from a SocketStream
				/// is not recommended, due to the read-ahead behavior of
				/// istream with formatted reads.
			{
			public:
				explicit SocketStream(const Socket& socket);
				/// Creates the SocketStream with the given socket.
				///
				/// The socket's SocketImpl must be a StreamSocketImpl,
				/// otherwise an InvalidArgumentException is thrown.

				~SocketStream();
				/// Destroys the SocketStream.
				///
				/// Flushes the buffer, but does not close the socket.
			};


			///
			inline StreamSocketProvider* SocketStreamBuffer::socketImpl() const
			{
				return _pImpl;
			}
		}
	}
}
#endif