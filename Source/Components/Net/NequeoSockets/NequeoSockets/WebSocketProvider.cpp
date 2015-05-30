/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          WebSocketProvider.cpp
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

#include "stdafx.h"

#include "WebSocketProvider.h"
#include "WebSocketType.h"
#include "Base\Timespan.h"
#include "Base\Types.h"
#include "Primitive\Format.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"
#include "IO\MemoryOutputStream.h"
#include "IO\MemoryInputStream.h"
#include "IO\Buffer.h"
#include "IO\BinaryWriter.h"
#include "IO\BinaryReader.h"

using Nequeo::Timespan;
using Nequeo::UInt8;
using Nequeo::UInt16;
using Nequeo::UInt32;
using Nequeo::UInt64;

using Nequeo::Net::Sockets::StreamSocketProvider;
using Nequeo::Net::Sockets::SocketAddress;
using Nequeo::Net::Sockets::SocketProvider;

using Nequeo::Exceptions::Net::WebSocketException;
using Nequeo::Exceptions::InvalidAccessException;

namespace Nequeo {
	namespace Net {
		namespace Provider
		{
			WebSocketProvider::WebSocketProvider(StreamSocketProvider* pStreamSocketImpl, bool mustMaskPayload) :
				StreamSocketProvider(pStreamSocketImpl->sockfd()),
				_pStreamSocketImpl(pStreamSocketImpl),
				_frameFlags(0),
				_mustMaskPayload(mustMaskPayload)
			{
				_pStreamSocketImpl->duplicate();
			}


			WebSocketProvider::~WebSocketProvider()
			{
				_pStreamSocketImpl->release();
				reset();
			}


			int WebSocketProvider::sendBytes(const void* buffer, int length, int flags)
			{
				Nequeo::IO::Buffer<char> frame(length + MAX_HEADER_LENGTH);
				Nequeo::IO::MemoryOutputStream ostr(frame.begin(), frame.size());
				Nequeo::IO::BinaryWriter writer(ostr, Nequeo::IO::BinaryWriter::NETWORK_BYTE_ORDER);

				writer << static_cast<UInt8>(flags);
				UInt8 lengthByte(0);
				if (_mustMaskPayload)
				{
					lengthByte |= FRAME_FLAG_MASK;
				}
				if (length < 126)
				{
					lengthByte |= static_cast<UInt8>(length);
					writer << lengthByte;
				}
				else if (length < 65536)
				{
					lengthByte |= 126;
					writer << lengthByte << static_cast<UInt16>(length);
				}
				else
				{
					lengthByte |= 127;
					writer << lengthByte << static_cast<UInt64>(length);
				}
				if (_mustMaskPayload)
				{
					const UInt32 mask = _rnd.next();
					const char* m = reinterpret_cast<const char*>(&mask);
					const char* b = reinterpret_cast<const char*>(buffer);
					writer.writeRaw(m, 4);
					char* p = frame.begin() + ostr.charsWritten();
					for (int i = 0; i < length; i++)
					{
						p[i] = b[i] ^ m[i % 4];
					}
				}
				else
				{
					std::memcpy(frame.begin() + ostr.charsWritten(), buffer, length);
				}
				_pStreamSocketImpl->sendBytes(frame.begin(), length + static_cast<int>(ostr.charsWritten()));
				return length;
			}


			int WebSocketProvider::receiveBytes(void* buffer, int length, int)
			{
				char header[MAX_HEADER_LENGTH];
				int n = receiveNBytes(header, 2);
				
				UInt8 lengthByte = static_cast<UInt8>(header[1]);
				int maskOffset = 0;
				if (lengthByte & FRAME_FLAG_MASK) maskOffset += 4;
				lengthByte &= 0x7f;
				if (lengthByte + 2 + maskOffset < MAX_HEADER_LENGTH)
				{
					n = receiveNBytes(header + 2, lengthByte + maskOffset);
				}
				else
				{
					n = receiveNBytes(header + 2, MAX_HEADER_LENGTH - 2);
				}

				
				n += 2;
				Nequeo::IO::MemoryInputStream istr(header, n);
				Nequeo::IO::BinaryReader reader(istr, Nequeo::IO::BinaryReader::NETWORK_BYTE_ORDER);
				UInt8 flags;
				char mask[4];
				reader >> flags >> lengthByte;
				_frameFlags = flags;
				int payloadLength = 0;
				int payloadOffset = 2;
				if ((lengthByte & 0x7f) == 127)
				{
					UInt64 l;
					reader >> l;
					if (l > length) throw WebSocketException(Nequeo::Primitive::Format("Insufficient buffer for payload size %Lu", l), Nequeo::Net::Provider::WS_ERR_PAYLOAD_TOO_BIG);
					payloadLength = static_cast<int>(l);
					payloadOffset += 8;
				}
				else if ((lengthByte & 0x7f) == 126)
				{
					UInt16 l;
					reader >> l;
					if (l > length) throw WebSocketException(Nequeo::Primitive::Format("Insufficient buffer for payload size %hu", l), Nequeo::Net::Provider::WS_ERR_PAYLOAD_TOO_BIG);
					payloadLength = static_cast<int>(l);
					payloadOffset += 2;
				}
				else
				{
					UInt8 l = lengthByte & 0x7f;
					if (l > length) throw WebSocketException(Nequeo::Primitive::Format("Insufficient buffer for payload size %u", unsigned(l)), Nequeo::Net::Provider::WS_ERR_PAYLOAD_TOO_BIG);
					payloadLength = static_cast<int>(l);
				}
				if (lengthByte & FRAME_FLAG_MASK)
				{
					reader.readRaw(mask, 4);
					payloadOffset += 4;
				}
				int received = 0;
				if (payloadOffset < n)
				{
					std::memcpy(buffer, header + payloadOffset, n - payloadOffset);
					received = n - payloadOffset;
				}
				if (received < payloadLength)
				{
					received += receiveNBytes(reinterpret_cast<char*>(buffer)+received, payloadLength - received);
				}
				if (lengthByte & FRAME_FLAG_MASK)
				{
					char* p = reinterpret_cast<char*>(buffer);
					for (int i = 0; i < received; i++)
					{
						p[i] ^= mask[i % 4];
					}
				}
				return received;
			}


			int WebSocketProvider::receiveNBytes(void* buffer, int bytes)
			{
				int received = _pStreamSocketImpl->receiveBytes(reinterpret_cast<char*>(buffer), bytes);
				while (received < bytes)
				{
					int n = _pStreamSocketImpl->receiveBytes(reinterpret_cast<char*>(buffer)+received, bytes - received);
					if (n > 0)
						received += n;
					else
						throw WebSocketException("Incomplete frame received", Nequeo::Net::Provider::WS_ERR_INCOMPLETE_FRAME);
				}
				return received;
			}


			SocketProvider* WebSocketProvider::acceptConnection(SocketAddress& clientAddr)
			{
				throw InvalidAccessException("Cannot acceptConnection() on a WebSocketImpl");
			}


			void WebSocketProvider::connect(const SocketAddress& address)
			{
				throw InvalidAccessException("Cannot connect() a WebSocketImpl");
			}


			void WebSocketProvider::connect(const SocketAddress& address, const Timespan& timeout)
			{
				throw InvalidAccessException("Cannot connect() a WebSocketImpl");
			}


			void WebSocketProvider::connectNB(const SocketAddress& address)
			{
				throw InvalidAccessException("Cannot connectNB() a WebSocketImpl");
			}


			void WebSocketProvider::bind(const SocketAddress& address, bool reuseAddress)
			{
				throw InvalidAccessException("Cannot bind() a WebSocketImpl");
			}


			void WebSocketProvider::bind6(const SocketAddress& address, bool reuseAddress, bool ipV6Only)
			{
				throw InvalidAccessException("Cannot bind6() a WebSocketImpl");
			}


			void WebSocketProvider::listen(int backlog)
			{
				throw InvalidAccessException("Cannot listen() on a WebSocketImpl");
			}


			void WebSocketProvider::close()
			{
				_pStreamSocketImpl->close();
				reset();
			}


			void WebSocketProvider::shutdownReceive()
			{
				_pStreamSocketImpl->shutdownReceive();
			}


			void WebSocketProvider::shutdownSend()
			{
				_pStreamSocketImpl->shutdownSend();
			}


			void WebSocketProvider::shutdown()
			{
				_pStreamSocketImpl->shutdown();
			}


			int WebSocketProvider::sendTo(const void* buffer, int length, const SocketAddress& address, int flags)
			{
				throw InvalidAccessException("Cannot sendTo() on a WebSocketImpl");
			}


			int WebSocketProvider::receiveFrom(void* buffer, int length, SocketAddress& address, int flags)
			{
				throw InvalidAccessException("Cannot receiveFrom() on a WebSocketImpl");
			}


			void WebSocketProvider::sendUrgent(unsigned char data)
			{
				throw InvalidAccessException("Cannot sendUrgent() on a WebSocketImpl");
			}


			bool WebSocketProvider::secure() const
			{
				return _pStreamSocketImpl->secure();
			}


			void WebSocketProvider::setSendTimeout(const Timespan& timeout)
			{
				_pStreamSocketImpl->setSendTimeout(timeout);
			}


			Timespan WebSocketProvider::getSendTimeout()
			{
				return _pStreamSocketImpl->getSendTimeout();
			}


			void WebSocketProvider::setReceiveTimeout(const Timespan& timeout)
			{
				_pStreamSocketImpl->setReceiveTimeout(timeout);
			}


			Timespan WebSocketProvider::getReceiveTimeout()
			{
				return _pStreamSocketImpl->getReceiveTimeout();
			}
		}
	}
}