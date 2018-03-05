/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          UdpServer.cpp
*  Purpose :       Udp Server class.
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

#include "stdafx.h"
#include "GlobalNetServer.h"

#include <boost/asio.hpp>

using boost::asio::ip::udp;

namespace Nequeo {
	namespace Net {
		namespace UDP
		{
			typedef std::function<void(const std::string&, const udp::endpoint&)> UdpReceiveHandler;

			/// <summary>
			///  UDP web server.
			/// </summary>
			class UdpServer
			{
			public:
				/// <summary>
				/// UDP web server.
				/// </summary>
				/// <param name="port">The UDP listening port.</param>
				/// <param name="protocol">The internet protocol.</param>
				UdpServer(unsigned int port, const boost::asio::ip::udp& protocol) :
					_disposed(false), _port(port), _initialised(false), _protocol(protocol)
				{
				}

				/// <summary>
				/// UDP web server.
				/// </summary>
				~UdpServer()
				{
					if (!_disposed)
					{
						_disposed = true;
						_initialised = false;
					}
				}

				/// <summary>
				/// Is initialise.
				/// </summary>
				/// <return>True if initialise; else false.</return>
				inline bool IsInitialise() const
				{
					return _initialised;
				}

				/// <summary>
				/// On received data.
				/// </summary>
				/// <param name="received">The received data handler.</param>
				void OnReceivedData(const UdpReceiveHandler& received)
				{
					_onReceivedData = received;
				}

				/// <summary>
				/// Initialise the servers.
				/// </summary>
				void Initialise()
				{
					// If not initialised.
					if (!_initialised)
					{
						// Create the new array of servers.
						_socket = std::make_shared<udp::socket>(_ioService, udp::endpoint(_protocol, _port));
						_initialised = true;

						// Start receiving data.
						StartReceive();
					}
				}

				///	<summary>
				///	Start the server.
				///	</summary>
				void Start()
				{
					// If initialised.
					if (_initialised)
					{
						try
						{
							// Main thread
							_ioService.run();
						}
						catch (...) {}
					}
				}

				///	<summary>
				///	Stop the server.
				///	</summary>
				void Stop()
				{
					// If initialised.
					if (_initialised)
					{
						try
						{
							// Close service.
							_ioService.stop();
						}
						catch (...) {}

						try
						{
							// Shutdown and close the client socket context.
							_socket->lowest_layer().shutdown(boost::asio::ip::tcp::socket::shutdown_both);
							_socket->lowest_layer().close();
							_socket->close();
						}
						catch (...) {}
					}
				}

				/// <summary>
				/// Send data to sender endpoint.
				/// </summary>
				/// <param name="data">The data to send.</param>
				/// <param name="sender">The sender endpoint.</param>
				void SendToSender(const std::string& data, const udp::endpoint& sender)
				{
					// Start sending data.
					_socket->async_send_to(boost::asio::buffer(data, data.size()), sender,
						[this](boost::system::error_code ec, std::size_t bytes_sent)
					{
						// Start receiving data.
						StartReceive();
					});
				}

			private:
				///	<summary>
				///	Start receiving data.
				///	</summary>
				void StartReceive()
				{
					udp::endpoint senderEndpoint;
					enum { max_length = 262144};
					char data[max_length];

					// Start receiving data.
					_socket->async_receive_from(boost::asio::buffer(data, max_length), senderEndpoint,
						[this, &senderEndpoint, &data](boost::system::error_code ec, std::size_t bytes_recvd)
					{
						// If data received.
						if (!ec && bytes_recvd > 0)
						{
							// Call the handler.
							_onReceivedData(std::string(data, bytes_recvd), senderEndpoint);
						}
						else
						{
							// Start receiving data.
							StartReceive();
						}
					});
				}

			private:
				bool _disposed;
				bool _initialised;

				unsigned int _port;
				boost::asio::ip::udp _protocol;
				std::shared_ptr<udp::socket> _socket;
				boost::asio::io_service _ioService;

				UdpReceiveHandler _onReceivedData;
			};

			/// <summary>
			///  UDP sender endpoint.
			/// </summary>
			class UdpSenderEndpoint
			{
			public:
				/// <summary>
				/// UDP sender endpoint.
				/// <param name="protocol">The internet protocol.</param>
				UdpSenderEndpoint() {}
				
				/// <summary>
				/// UDP sender endpoint.
				/// </summary>
				~UdpSenderEndpoint()
				{
					if (!_disposed)
					{
						_disposed = true;
					}
				}

				udp::endpoint senderEndpoint;
				std::string senderAddress;
				unsigned int senderPort;

			private:
				bool _disposed;
			};
		}
	}
}

