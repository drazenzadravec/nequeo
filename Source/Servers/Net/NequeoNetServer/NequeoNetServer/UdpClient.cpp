/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          UdpClient.cpp
*  Purpose :       Udp Client class.
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
#include "UdpServer.cpp"

#include <boost/asio.hpp>

using boost::asio::ip::udp;

namespace Nequeo {
	namespace Net {
		namespace UDP
		{
			/// <summary>
			///  UDP web client.
			/// </summary>
			class UdpClient
			{
			public:
				/// <summary>
				/// UDP web client.
				/// </summary>
				/// <param name="port">The UDP listening port.</param>
				/// <param name="protocol">The internet protocol.</param>
				UdpClient(unsigned int port, const boost::asio::ip::udp& protocol) :
					_disposed(false), _port(port), _initialised(false), _connected(false), _protocol(protocol)
				{
				}

				/// <summary>
				/// UDP web client.
				/// </summary>
				~UdpClient()
				{
					if (!_disposed)
					{
						_disposed = true;
						_initialised = false;
						_connected = false;
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
						//// Create the new array of servers.
						_socket = std::make_shared<udp::socket>(_ioService, udp::endpoint(_protocol, _port));
						_resolver = std::make_shared<udp::resolver>(_ioService);
						_initialised = true;
						_connected = false;

						// Start receiving data.
						StartReceive();
					}
				}

				///	<summary>
				///	Connect to an endpoint.
				///	</summary>
				/// <param name="host">The UDP host name or IP.</param>
				/// <param name="port">The UDP host port.</param>
				void Connect(const std::string& host, unsigned int port)
				{
					// If initialised.
					if (_initialised)
					{
						udp::resolver::query query(_protocol, host, std::to_string(port));
						_endpoint = *_resolver->resolve(query);
						_connected = true;
					}
				}

				/// <summary>
				/// Send data to the endpoint created after calling connect.
				/// </summary>
				/// <param name="data">The data to send.</param>
				void SendToConnect(const std::string& data)
				{
					// If connected.
					if (_connected)
					{
						// Send to endpoint.
						_socket->send_to(boost::asio::buffer(data, data.size()), _endpoint);
					}
				}

				/// <summary>
				/// Send data to the endpoint created after calling connect.
				/// </summary>
				/// <param name="data">The data to send.</param>
				/// <param name="host">The UDP host name or IP.</param>
				/// <param name="port">The UDP host port.</param>
				void SendToDirect(const std::string& data, const std::string& host, unsigned int port)
				{
					boost::asio::io_service io_service;
					udp::socket socket(io_service, udp::endpoint(_protocol, 0));

					// Resolver.
					udp::resolver resolver(io_service);
					udp::endpoint endpoint = *_resolver->resolve({ _protocol, host, std::to_string(port) });

					// Send the data.
					socket.send_to(boost::asio::buffer(data, data.size()), endpoint);
				}

				/// <summary>
				/// Broadcast data on the port (only IPv4).
				/// </summary>
				/// <param name="data">The data to send.</param>
				/// <param name="port">The port to broadcast on.</param>
				void SendToBroadcast(const std::string& data, unsigned int port)
				{
					// If connected.
					if (_initialised)
					{
						// Set socket options.
						_socket->set_option(udp::socket::reuse_address(true));
						_socket->set_option(boost::asio::socket_base::broadcast(true));

						// Send to endpoints.
						udp::endpoint endpoint(boost::asio::ip::address_v4::broadcast(), port);

						// Send to all hosts listening on the port.
						_socket->send_to(boost::asio::buffer(data, data.size()), endpoint);
					}
				}

				/// <summary>
				/// Broadcast data on the port (only IPv4).
				/// </summary>
				/// <param name="data">The data to send.</param>
				/// <param name="port">The port to broadcast on.</param>
				void SendToBroadcastDirect(const std::string& data, unsigned int port)
				{
					boost::asio::io_service io_service;
					udp::socket socket(io_service, udp::endpoint(udp::v4(), 0));

					// Set socket options.
					socket.set_option(udp::socket::reuse_address(true));
					socket.set_option(boost::asio::socket_base::broadcast(true));

					// Send to endpoints.
					udp::endpoint endpoint(boost::asio::ip::address_v4::broadcast(), port);

					// Send the data.
					socket.send_to(boost::asio::buffer(data, data.size()), endpoint);
				}

				/// <summary>
				/// Broadcast data on the port (only IPv4).
				/// </summary>
				/// <param name="data">The data to send.</param>
				/// <param name="port">The port to broadcast on.</param>
				/// <param name="ip">The port to broadcast on.</param>
				/// <param name="ipMask">The port to broadcast on.</param>
				void SendToBroadcastDirect(const std::string& data, unsigned int port, const std::string& ip, const std::string& ipMask)
				{
					boost::asio::io_service io_service;
					udp::socket socket(io_service, udp::endpoint(udp::v4(), 0));

					// Set socket options.
					socket.set_option(udp::socket::reuse_address(true));
					socket.set_option(boost::asio::socket_base::broadcast(true));

					// Send to endpoints.
					boost::asio::ip::address_v4 addr = boost::asio::ip::address_v4::from_string(ip);
					boost::asio::ip::address_v4 mask= boost::asio::ip::address_v4::from_string(ipMask);
					udp::endpoint endpoint(boost::asio::ip::address_v4::broadcast(addr, mask), port);

					// Send the data.
					socket.send_to(boost::asio::buffer(data, data.size()), endpoint);
				}

				/// <summary>
				/// Receive data from the endpoint created after calling connect and then calling sendto.
				/// </summary>
				/// <return>The data received from the endpoint.</return>
				std::string ReceiveFrom()
				{
					udp::endpoint senderEndpoint;
					enum { max_length = 262144};
					char data[max_length];

					// Get the reply.
					size_t replyLength = _socket->receive_from(boost::asio::buffer(data, max_length), senderEndpoint);
					return std::string(data, replyLength);
				}

				///	<summary>
				///	Open the connection.
				///	</summary>
				void Open()
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
				///	Close the connection.
				///	</summary>
				void Close()
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
						enum { max_length = 65536 };
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

							// Start receiving data.
							StartReceive();
						});
					}

			private:
				bool _disposed;
				bool _initialised;
				bool _connected;

				unsigned int _port;
				boost::asio::ip::udp _protocol;
				udp::endpoint _endpoint;

				std::shared_ptr<udp::socket> _socket;
				std::shared_ptr<udp::resolver> _resolver;
				boost::asio::io_service _ioService;

				UdpReceiveHandler _onReceivedData;
			};
		}
	}
}

