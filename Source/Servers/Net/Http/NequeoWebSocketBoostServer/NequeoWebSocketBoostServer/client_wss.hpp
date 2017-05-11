/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          client_wss.h
*  Purpose :       Web socket client class.
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

#ifndef CLIENT_WSS_HPP
#define	CLIENT_WSS_HPP

#include "stdafx.h"
#include "IPVersionType.h"

#include "client_ws.hpp"
#include <boost/asio/ssl.hpp>

namespace Nequeo {
	namespace Net {
		namespace WebSocket {
			namespace Boost
			{
				///	<summary>
				///	Secure websocket TCP socket.
				///	</summary>
				typedef boost::asio::ssl::stream<boost::asio::ip::tcp::socket> WSS;

				///	<summary>
				///	Secure websocket client.
				///	</summary>
				template<>
				class SocketClient<WSS> : public SocketClientBase<WSS> 
				{
				public:
					///	<summary>
					///	Secure websocket client.
					///	</summary>
					/// <param name="server_port_path">The server name or IP with specific port (10.1.1.1:444) if port is not specified to defualt port 443 is used.</param>
					/// <param name="verify_certificate">Should the certificate be verified.</param>
					/// <param name="ipv">The ip version.</param>
					/// <param name="cert_file">The certificate file path.</param>
					/// <param name="private_key_file">The private (un-encrypted) key file.</param>
					/// <param name="verify_file">The verificate file path.</param>
					SocketClient(
						const std::string& server_port_path, IPVersionType ipv = IPVersionType::IPv4, bool verify_certificate = false,
						const std::string& cert_file = std::string(), const std::string& private_key_file = std::string(),
						const std::string& verify_file = std::string()) :
						SocketClientBase<WSS>::SocketClientBase(server_port_path, 443, ipv), context(boost::asio::ssl::context::tlsv12)
					{
						// If certicate file is to be verified.
						if (verify_certificate) 
						{
							context.set_verify_mode(boost::asio::ssl::verify_peer);
							context.set_default_verify_paths();
						}
						else
							context.set_verify_mode(boost::asio::ssl::verify_none);

						// If a public and private certificate exists.
						if (cert_file.size() > 0 && private_key_file.size() > 0) 
						{
							context.use_certificate_chain_file(cert_file);
							context.use_private_key_file(private_key_file, boost::asio::ssl::context::pem);
						}

						// If a vertifcation file exists.
						if (verify_file.size() > 0)
							context.load_verify_file(verify_file);

					};

				protected:
					// Current ssl context.
					boost::asio::ssl::context context;

					///	<summary>
					///	Override the connect method.
					///	</summary>
					void connect() override
					{
						// Query resolver.
						boost::asio::ip::tcp::resolver::query query(host, std::to_string(port));

						// Resolver the IP or Name.
						resolver.async_resolve(query, [this](const boost::system::error_code &ec, boost::asio::ip::tcp::resolver::iterator it) 
						{
							// If no error.
							if (!ec) 
							{
								// New connection.
								connection = std::shared_ptr<Connection>(new Connection(new WSS(io_service, context)));

								// New async connection.
								boost::asio::async_connect(connection->socket->lowest_layer(), it, [this](const boost::system::error_code &ec, boost::asio::ip::tcp::resolver::iterator /*it*/) 
								{
									// If no error.
									if (!ec) 
									{
										// Set the delay options.
										boost::asio::ip::tcp::no_delay option(true);
										connection->socket->lowest_layer().set_option(option);

										// Create socket handshake.
										connection->socket->async_handshake(boost::asio::ssl::stream_base::client, [this](const boost::system::error_code& ec) 
										{
											// If no error.
											if (!ec)
												// Handshake.
												handshake();
											else
												throw std::invalid_argument(ec.message());
										});
									}
									else
										throw std::invalid_argument(ec.message());
								});
							}
							else
								throw std::invalid_argument(ec.message());
						});
					}
				};
			}
		}
	}
}
#endif	/* CLIENT_WSS_HPP */