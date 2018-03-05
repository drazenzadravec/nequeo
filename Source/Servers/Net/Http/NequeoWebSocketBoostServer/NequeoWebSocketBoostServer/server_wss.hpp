/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          server_wss.h
*  Purpose :       Web socket server class.
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

#ifndef SERVER_WSS_HPP
#define	SERVER_WSS_HPP

#include "stdafx.h"
#include "IPVersionType.h"

#include "server_ws.hpp"
#include <boost/asio/ssl.hpp>
#include <boost/bind.hpp>

namespace Nequeo {
	namespace Net {
		namespace WebSocket {
			namespace Boost
			{
				///	<summary>
				///	Secure Web Socket TCP socket.
				///	</summary>
				typedef boost::asio::ssl::stream<boost::asio::ip::tcp::socket> WSS;

				///	<summary>
				///	Secure websocket server.
				///	</summary>
				template<>
				class SocketServer<WSS> : public SocketServerBase<WSS> 
				{
				public:
					///	<summary>
					///	Secure websocket server.
					///	</summary>
					/// <param name="port">The port number the server should listen on.</param>
					/// <param name="num_threads">The number of threads to use (set to 1 is more than statisfactory).</param>
					/// <param name="cert_file">The certificate file path.</param>
					/// <param name="private_key_file">The private (un-encrypted, encrypted - use password) key file.</param>
					/// <param name="timeout_request">The request time out.</param>
					/// <param name="timeout_idle">The send and receive time out (600 seconds = 10 minutes).</param>
					/// <param name="timeout_connect">The time out (seconds) to connect.</param>
					/// <param name="ipv">The ip version.</param>
					/// <param name="private_key_password">The private key password (decrypt).</param>
					/// <param name="verify_file">The verificate file path.</param>
					SocketServer(
						unsigned short port, size_t num_threads, const std::string& cert_file, const std::string& private_key_file,
						IPVersionType ipv = IPVersionType::IPv4, size_t timeout_request = 5, size_t timeout_idle = 0, size_t timeout_connect = 0,
						const std::string& private_key_password = std::string(), const std::string& verify_file = std::string()) :
						SocketServerBase<WSS>::SocketServerBase(port, num_threads, timeout_request, timeout_idle, timeout_connect, ipv),
						context(boost::asio::ssl::context::tlsv12), _private_key_password(private_key_password)
					{
						// 2016/08/13 only use tls12, see https://www.ssllabs.com/ssltest
						context.use_certificate_chain_file(cert_file);
						context.use_private_key_file(private_key_file, boost::asio::ssl::context::pem);

						// If a vertifcation file exists.
						if (verify_file.size() > 0)
							context.load_verify_file(verify_file);

						// If a private key certificate password exists.
						if (private_key_password.size() > 0)
							context.set_password_callback(boost::bind(&SocketServer<WSS>::GetCertificatePassword, this));
					}

				protected:
					// Current ssl context.
					boost::asio::ssl::context context;

					///	<summary>
					///	Override the accept method.
					///	</summary>
					void accept() override
					{
						// Create new socket for this connection (stored in Connection::socket)
						// Shared_ptr is used to pass temporary objects to the asynchronous functions
						std::shared_ptr<Connection> connection(new Connection(new WSS(io_service, context)));

						// Accept a new client connection, and using the new socket client created above.
						acceptor.async_accept(connection->socket->lowest_layer(), [this, connection](const boost::system::error_code& ec)
						{
							// Immediately start accepting a new connection
							accept();

							// If no error.
							if (!ec) 
							{
								// Set socket options.
								boost::asio::ip::tcp::no_delay option(true);
								connection->socket->lowest_layer().set_option(option);

								// Create the web request and web context.
								auto webRequest = std::make_shared<WebRequest>();
								auto webMessage = std::make_shared<WebMessage>();

								// Assign the connection.
								connection->socketServer = this;
								webMessage->connectionHandler = std::static_pointer_cast<void>(connection);
								connection->webContext = std::make_shared<WebContext>(webRequest, webMessage);
								
								// Set timeout on the following boost::asio::ssl::stream::async_handshake
								std::shared_ptr<boost::asio::deadline_timer> timer;

								// If timeout request.
								if (timeout_request > 0)
									// Set the timeout for the current client socket.
									timer = set_timeout_on_connection(connection, timeout_request);

								// Start the asyn authenticate as the server, start the handshake and negosiation.
								connection->socket->async_handshake(boost::asio::ssl::stream_base::server, [this, connection, timer](const boost::system::error_code& ec)
								{
									// If timeout.
									if (timeout_request > 0)
										// Cancel the timeout and start reading.
										timer->cancel();

									// If no error.
									if (!ec) 
									{
										// Read the request data from the client.
										read_handshake(connection);
									}
									else
									{
										try
										{
											// Shutdown and close the client socket context.
											connection->socket->lowest_layer().shutdown(boost::asio::ip::tcp::socket::shutdown_both);
											connection->socket->lowest_layer().close();
											connection->webContext = nullptr;
										}
										catch (const std::exception&) {}
									}
								});
							}
							else
							{
								try
								{
									// Shutdown and close the client socket context.
									connection->socket->lowest_layer().shutdown(boost::asio::ip::tcp::socket::shutdown_both);
									connection->socket->lowest_layer().close();
									connection->webContext = nullptr;
								}
								catch (const std::exception&) {}
							}
						});
					}

				private:
					std::string _private_key_password;

					///	<summary>
					///	Get the certificate password.
					///	</summary>
					std::string GetCertificatePassword() const
					{
						return _private_key_password;
					}

				};
			}
		}
	}
}
#endif	/* SERVER_WSS_HPP */

