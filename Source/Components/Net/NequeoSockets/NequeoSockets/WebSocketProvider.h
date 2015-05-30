/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          WebSocketProvider.h
*  Purpose :       WebSocketProvider class.
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

#ifndef _WEBSOCKETPROVIDER_H
#define _WEBSOCKETPROVIDER_H

#include "GlobalSocket.h"
#include "StreamSocketProvider.h"
#include "Base\Timespan.h"
#include "Primitive\Random.h"

using Nequeo::Net::Sockets::StreamSocketProvider;
using Nequeo::Net::Sockets::SocketAddress;
using Nequeo::Net::Sockets::SocketProvider;

using Nequeo::Timespan;

namespace Nequeo {
	namespace Net {
		namespace Provider
		{
			/// This class implements a WebSocket, according
			/// to the WebSocket protocol described in RFC 6455.
			class WebSocketProvider : public StreamSocketProvider
			{
			public:
				WebSocketProvider(StreamSocketProvider* pStreamSocketImpl, bool mustMaskPayload);
				/// Creates a StreamSocketImpl using the given native socket.

				// StreamSocketImpl
				virtual int sendBytes(const void* buffer, int length, int flags);
				/// Sends a WebSocket protocol frame.

				virtual int receiveBytes(void* buffer, int length, int flags);
				/// Receives a WebSocket protocol frame.

				virtual SocketProvider* acceptConnection(SocketAddress& clientAddr);
				virtual void connect(const SocketAddress& address);
				virtual void connect(const SocketAddress& address, const Timespan& timeout);
				virtual void connectNB(const SocketAddress& address);
				virtual void bind(const SocketAddress& address, bool reuseAddress = false);
				virtual void bind6(const SocketAddress& address, bool reuseAddress = false, bool ipV6Only = false);
				virtual void listen(int backlog = 64);
				virtual void close();
				virtual void shutdownReceive();
				virtual void shutdownSend();
				virtual void shutdown();
				virtual int sendTo(const void* buffer, int length, const SocketAddress& address, int flags = 0);
				virtual int receiveFrom(void* buffer, int length, SocketAddress& address, int flags = 0);
				virtual void sendUrgent(unsigned char data);
				virtual bool secure() const;
				virtual void setSendTimeout(const Timespan& timeout);
				virtual Timespan getSendTimeout();
				virtual void setReceiveTimeout(const Timespan& timeout);
				virtual Timespan getReceiveTimeout();

				// Internal
				int frameFlags() const;
				/// Returns the frame flags of the most recently received frame.

				bool mustMaskPayload() const;
				/// Returns true if the payload must be masked.

			protected:
				enum
				{
					FRAME_FLAG_MASK = 0x80,
					MAX_HEADER_LENGTH = 14
				};

				int receiveNBytes(void* buffer, int bytes);
				virtual ~WebSocketProvider();

			private:
				WebSocketProvider();

				StreamSocketProvider* _pStreamSocketImpl;
				int _frameFlags;
				bool _mustMaskPayload;
				Nequeo::Primitive::Random _rnd;
			};


			//
			// inlines
			//
			inline int WebSocketProvider::frameFlags() const
			{
				return _frameFlags;
			}


			inline bool WebSocketProvider::mustMaskPayload() const
			{
				return _mustMaskPayload;
			}
		}
	}
}
#endif