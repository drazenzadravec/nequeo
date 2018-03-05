/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          server_ws.h
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

#ifndef SERVER_WS_HPP
#define	SERVER_WS_HPP

#include "stdafx.h"
#include "WebContext.h"
#include "IPVersionType.h"

#include "crypto.hpp"

#include <boost/asio.hpp>
#include <boost/asio/spawn.hpp>
#include <boost/regex.hpp>

#include <unordered_map>
#include <thread>
#include <mutex>
#include <set>
#include <list>
#include <memory>
#include <atomic>
#include <iostream>

namespace Nequeo {
	namespace Net {
		namespace WebSocket {
			namespace Boost
			{
				/// <summary>
				/// WebSocket server base.
				/// </summary>
				template <class socket_type>
				class SocketServer;

				/// <summary>
				/// WebSocket server base.
				/// </summary>
				template <class socket_type>
				class SocketServerBase 
				{
				public:
					/// <summary>
					/// WebSocket server base destructor.
					/// </summary>
					virtual ~SocketServerBase() {}

					/// <summary>
					/// WebSocket server response.
					/// </summary>
					class SendStream : public std::ostream 
					{
						/// <summary>
						/// Allow the server base to use private members.
						/// </summary>
						friend class SocketServerBase<socket_type>;

					private:
						boost::asio::streambuf streambuf;

					public:
						/// <summary>
						/// WebSocket server response.
						/// </summary>
						SendStream() : std::ostream(&streambuf) {}

						/// <summary>
						/// Get the stream buffer size.
						/// </summary>
						/// <return>The stream buffer size.</return>
						size_t size() 
						{
							return streambuf.size();
						}
					};

					/// <summary>
					/// WebSocket server connection.
					/// </summary>
					class Connection 
					{
						/// <summary>
						/// Allow the client base to use private members.
						/// </summary>
						friend class SocketServerBase<socket_type>;
						friend class SocketServer<socket_type>;

					public:
						std::string method, path, http_version;
						std::unordered_map<std::string, std::string> header;
						boost::smatch path_match;
						std::string remote_endpoint_address;
						unsigned short remote_endpoint_port;
						bool timeout_access_expiry_async_started;

						// Store web context.
						std::shared_ptr<WebContext> webContext;
						SocketServerBase<socket_type>* socketServer;

						std::unique_ptr<boost::asio::deadline_timer> timer_access_expiry;
						std::unique_ptr<boost::asio::deadline_timer> timer_connect;

						/// <summary>
						/// Cancel the time out connect timer.
						/// </summary>
						void CancelTimeoutConnect(bool cancel)
						{
							// If cancel and not timeout
							// then call cancel.
							if (cancel && !timeout_connect_cancelled)
							{
								try
								{
									// Cancel connect timeout.
									timeout_connect_cancelled = cancel;
									timer_connect->cancel();
								}
								catch (const std::exception&) {}
							}
						}

						/// <summary>
						/// Start the access expiry timeout.
						/// </summary>
						void StartAccessExpiry(unsigned int accessExpiry)
						{
							// If access expiry exists.
							if (accessExpiry > 0 && !timeout_access_expiry_started)
							{
								timeout_access_expiry_started = true;
								timer_access_expiry->expires_from_now(boost::posix_time::minutes(static_cast<unsigned long>(accessExpiry)));
							}
						}

					private:
						/// <summary>
						/// WebSocket server connection.
						/// </summary>
						/// <param name="socket">The socket type.</param>
						Connection(socket_type *socket) : socket(socket), strand(socket->get_io_service()), closed(false), 
							timeout_connect_cancelled(false), timeout_access_expiry_started(false), timeout_access_expiry_async_started(false) {}

						/// <summary>
						/// WebSocket server send data.
						/// </summary>
						class SendData 
						{
						public:
							/// <summary>
							/// WebSocket server send data.
							/// </summary>
							/// <param name="header_stream">The header stream.</param>
							/// <param name="message_stream">The message stream.</param>
							/// <param name="callback">The error callback.</param>
							SendData(std::shared_ptr<SendStream> header_stream, std::shared_ptr<SendStream> message_stream,
								const std::function<void(const boost::system::error_code)> &callback) :
								header_stream(header_stream), message_stream(message_stream), callback(callback) {}

							std::shared_ptr<SendStream> header_stream;
							std::shared_ptr<SendStream> message_stream;
							std::function<void(const boost::system::error_code)> callback;
						};

						// boost::asio::ssl::stream constructor needs move, until then we store socket as unique_ptr
						std::unique_ptr<socket_type> socket;
						boost::asio::strand strand;
						std::list<SendData> send_queue;
						bool timeout_connect_cancelled;
						bool timeout_access_expiry_started;

						///	<summary>
						///	Send data from queue.
						///	</summary>
						/// <param name="connection">The connection.</param>
						void send_from_queue(std::shared_ptr<Connection> connection) 
						{
							strand.post([this, connection]() 
							{
								// Write send buffer.
								boost::asio::async_write(*socket, send_queue.begin()->header_stream->streambuf,
									strand.wrap([this, connection](const boost::system::error_code& ec, size_t /*bytes_transferred*/) 
								{
									// If no error.
									if (!ec) 
									{
										// Write send buffer.
										boost::asio::async_write(*socket, send_queue.begin()->message_stream->streambuf,strand.wrap([this, connection]
											(const boost::system::error_code& ec, size_t /*bytes_transferred*/)
										{
											// Get the start of the queue.
											auto send_queued = send_queue.begin();

											// If callback set then send a callback.
											if (send_queued->callback)
												send_queued->callback(ec);

											// If no error.
											if (!ec) 
											{
												// Remove the queue item.
												send_queue.erase(send_queued);

												// If move data exists.
												if (send_queue.size() > 0)
													// Send the next item in the queue.
													send_from_queue(connection);
											}
											else
												send_queue.clear();
										}));
									}
									else 
									{
										// Get the start of the queue.
										auto send_queued = send_queue.begin();

										// If callback set then send a callback.
										if (send_queued->callback)
											send_queued->callback(ec);

										send_queue.clear();
									}
								}));
							});
						}

						std::atomic<bool> closed;
						std::unique_ptr<boost::asio::deadline_timer> timer_idle;

						///	<summary>
						///	Read the remote endpoint information.
						///	</summary>
						void read_remote_endpoint_data() 
						{
							try 
							{
								// Get remote endpoint address and port.
								remote_endpoint_address = socket->lowest_layer().remote_endpoint().address().to_string();
								remote_endpoint_port = socket->lowest_layer().remote_endpoint().port();
							}
							catch (const std::exception& e) 
							{
								std::cerr << e.what() << std::endl;
							}
						}
					};

					/// <summary>
					/// WebSocket server message.
					/// </summary>
					class Message : public std::istream 
					{
						friend class SocketServerBase<socket_type>;

					public:
						unsigned char fin_rsv_opcode;

						///	<summary>
						///	Get the stream buffer size.
						///	</summary>
						///	<return>The size.</return>
						size_t size() 
						{
							return length;
						}

						///	<summary>
						///	Get the stream buffer.
						///	</summary>
						///	<return>The string.</return>
						std::string string() 
						{
							std::stringstream ss;
							ss << rdbuf();
							return ss.str();
						}
					private:
						/// <summary>
						/// WebSocket server message.
						/// </summary>
						Message() : std::istream(&streambuf) {}

						size_t length;
						boost::asio::streambuf streambuf;
					};

					/// <summary>
					/// WebSocket server endpoint.
					/// </summary>
					class Endpoint 
					{
						friend class SocketServerBase<socket_type>;

					private:
						std::set<std::shared_ptr<Connection> > connections;
						std::mutex connections_mutex;

					public:
						std::function<void(std::shared_ptr<Connection>)> onopen;
						std::function<void(std::shared_ptr<Connection>, std::shared_ptr<Message>)> onmessage;
						std::function<void(std::shared_ptr<Connection>, const boost::system::error_code&)> onerror;
						std::function<void(std::shared_ptr<Connection>, int, const std::string&)> onclose;
						std::function<void(std::shared_ptr<Connection>, std::shared_ptr<Message>)> onaccessexpiry;

						///	<summary>
						///	Get all connections.
						///	</summary>
						///	<return>The connections.</return>
						std::set<std::shared_ptr<Connection> > get_connections() 
						{
							connections_mutex.lock();
							auto copy = connections;
							connections_mutex.unlock();
							return copy;
						}
					};

					/// <summary>
					/// WebSocket server config.
					/// </summary>
					class Config 
					{
						friend class SocketServerBase<socket_type>;

					private:
						/// <summary>
						/// WebSocket server config.
						/// </summary>
						/// <param name="port">The port number.</param>
						/// <param name="num_threads">The number of threads.</param>
						Config(unsigned short port, size_t num_threads) : num_threads(num_threads), port(port), reuse_address(true) {}

						size_t num_threads;

					public:
						unsigned short port;
						/// IPv4 address in dotted decimal form or IPv6 address in hexadecimal notation.
						/// If empty, the address will be any address.
						std::string address;
						/// Set to false to avoid binding the socket to an address that is already in use.
						bool reuse_address;
					};

					// Set before calling start().
					Config config;

					std::map<std::string, Endpoint> endpoint;
					std::string default_resource_method;

				private:
					///	<summary>
					///	Client content option resource.
					///	</summary>
					std::vector<std::pair<boost::regex, Endpoint*> > opt_endpoint;

				public:
					///	<summary>
					///	Start the server.
					///	</summary>
					void start() 
					{
						// Copy the resources to opt_resource for more efficient request processing.
						opt_endpoint.clear();

						// For each resource context.
						for (auto& endp : endpoint) 
						{
							// Set the request method options
							opt_endpoint.emplace_back(boost::regex(endp.first), &endp.second);
						}

						// If the service has been stopped.
						if (io_service.stopped())
							io_service.reset();

						boost::asio::ip::tcp::endpoint endpoint;

						// If the endpoint listening address was set.
						if (config.address.size() > 0)
							// Set the configured endpoint.
							endpoint = boost::asio::ip::tcp::endpoint(boost::asio::ip::address::from_string(config.address), config.port);
						else
						{
							// Select the ip version.
							switch (_ipv)
							{
							case IPVersionType::IPv6:
								endpoint = boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v6(), config.port);
								break;

							default:
								// Set the default all endpoints for the IPv4 TCP.
								endpoint = boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v4(), config.port);
								break;
							}
						}

						// Open the listening endpoint
						acceptor.open(endpoint.protocol());
						acceptor.set_option(boost::asio::socket_base::reuse_address(config.reuse_address));

						// Bind to the endpoint and start listening on the port and endpoint.
						acceptor.bind(endpoint);
						acceptor.listen();

						// Start accepting connections.
						accept();

						// If num_threads>1, start m_io_service.run() in (num_threads-1) threads for thread-pooling
						threads.clear();
						for (size_t c = 1; c < config.num_threads; c++) 
						{
							threads.emplace_back([this]() 
							{
								// Start a new service.
								io_service.run();
							});
						}

						// Main thread
						io_service.run();

						//Wait for the rest of the threads, if any, to finish as well
						for (auto& t : threads) 
						{
							t.join();
						}
					}

					///	<summary>
					///	Stop the server.
					///	</summary>
					void stop() 
					{
						try
						{
							// Close acceptor.
							acceptor.close();
						}
						catch (...) {}

						try
						{
							// Close service.
							io_service.stop();
						}
						catch (...) {}

						try
						{
							// Disconnect each endpoint.
							for (auto& p : endpoint)
								p.second.connections.clear();
						}
						catch (...) {}
					}

					/// <summary>
					/// Send the data.
					/// </summary>
					/// <param name="connection">The connection.</param>
					/// <param name="message_stream">The message stream.</param>
					/// <param name="callback">The error callback.</param>
					/// <param name="fin_rsv_opcode">The op code.</param>
					///	<remarks>
					/// fin_rsv_opcode: 129=one fragment, text, 130=one fragment, binary, 136=close connection.
					/// See http://tools.ietf.org/html/rfc6455#section-5.2 for more information
					///	</remarks>
					void send(
						std::shared_ptr<Connection> connection, 
						std::shared_ptr<SendStream> message_stream,
						const std::function<void(const boost::system::error_code&)>& callback = nullptr,
						unsigned char fin_rsv_opcode = 129) const 
					{
						// If opcode is 136 then reset connection.
						if (fin_rsv_opcode != 136)
							timer_idle_reset(connection);

						// Create header stream pointer.
						auto header_stream = std::make_shared<SendStream>();
						size_t length = message_stream->size();

						// Add the current op code to the header stream.
						header_stream->put(fin_rsv_opcode);

						// Unmasked (first length byte<128)
						if (length >= 126) 
						{
							int num_bytes;

							if (length > 0xffff) 
							{
								num_bytes = 8;
								header_stream->put(127);
							}
							else 
							{
								num_bytes = 2;
								header_stream->put(126);
							}

							for (int c = num_bytes - 1; c >= 0; c--) 
							{
								// Add header stream data.
								header_stream->put((length >> (8 * c)) % 256);
							}
						}
						else
							header_stream->put(static_cast<unsigned char>(length));

						// Post a new connection.
						connection->strand.post([this, connection, header_stream, message_stream, callback]() 
						{
							// Add to connection queue data.
							connection->send_queue.emplace_back(header_stream, message_stream, callback);

							// If data exists in connection queue.
							if (connection->send_queue.size() == 1)
								// Send the next message in the connection queue.
								connection->send_from_queue(connection);
						});
					}

					/// <summary>
					/// Send the close event.
					/// </summary>
					/// <param name="connection">The connection.</param>
					/// <param name="status">The connection status.</param>
					/// <param name="reason">The close reason.</param>
					/// <param name="callback">The error callback.</param>
					void send_close(std::shared_ptr<Connection> connection, int status, const std::string& reason = "",
						const std::function<void(const boost::system::error_code&)>& callback = nullptr) const 
					{
						// Send close only once (in case close is initiated by server)
						if (connection->closed.load()) 
						{
							return;
						}

						// Sent atomic operation to true
						// Not called again.
						connection->closed.store(true);

						// Make send stream.
						auto send_stream = std::make_shared<SendStream>();

						send_stream->put(status >> 8);
						send_stream->put(status % 256);
						*send_stream << reason;

						try
						{
							// fin_rsv_opcode=136: message close
							send(connection, send_stream, callback, 136);
						}
						catch (const std::exception&) {}
					}

					///	<summary>
					///	Get all connections.
					///	</summary>
					///	<return>The connections.</return>
					std::set<std::shared_ptr<Connection> > get_connections() 
					{
						std::set<std::shared_ptr<Connection> > all_connections;

						// For each endpoint.
						for (auto& e : endpoint) 
						{
							e.second.connections_mutex.lock();
							all_connections.insert(e.second.connections.begin(), e.second.connections.end());
							e.second.connections_mutex.unlock();
						}

						// Return the connections.
						return all_connections;
					}

				protected:
					const std::string ws_magic_string = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

					IPVersionType _ipv;
					boost::asio::io_service io_service;
					boost::asio::ip::tcp::acceptor acceptor;
					std::vector<std::thread> threads;

					size_t timeout_request;
					size_t timeout_idle;
					size_t timeout_connect;

					///	<summary>
					///	WebSocket server.
					///	</summary>
					/// <param name="port">The port number the server should listen on.</param>
					/// <param name="num_threads">The number of threads to use (set to 1 is more than statisfactory).</param>
					/// <param name="timeout_request">The request time out.</param>
					/// <param name="timeout_idle">The idle time out.</param>
					/// <param name="timeout_connect">The time out (seconds) to connect.</param>
					/// <param name="ipv">The ip version.</param>
					SocketServerBase(
						unsigned short port, size_t num_threads, size_t timeout_request, size_t timeout_idle, size_t timeout_connect, IPVersionType ipv) :
						config(port, num_threads), acceptor(io_service), _ipv(ipv), default_resource_method("DEFAULT_METHOD"), 
						timeout_request(timeout_request), timeout_idle(timeout_idle), timeout_connect(timeout_connect) {}

					///	<summary>
					///	Virtual accept method must be overriden.
					///	</summary>
					virtual void accept() = 0;

					///	<summary>
					///	Set the timeout on socket.
					///	</summary>
					/// <param name="socket">The socket context.</param>
					/// <param name="seconds">The timeout.</param>
					/// <return>The timer.</return>
					std::shared_ptr<boost::asio::deadline_timer> set_timeout_on_connection(std::shared_ptr<Connection> connection, size_t seconds) 
					{
						std::shared_ptr<boost::asio::deadline_timer> timer(new boost::asio::deadline_timer(io_service));
						timer->expires_from_now(boost::posix_time::seconds(static_cast<long>(seconds)));

						// If the timeout is triggered then close the socket.
						timer->async_wait([connection](const boost::system::error_code& ec) 
						{
							// If no error.
							if (!ec) 
							{
								try
								{
									// Shutdown and close the client socket context.
									connection->socket->lowest_layer().shutdown(boost::asio::ip::tcp::socket::shutdown_both);
									connection->socket->lowest_layer().close();
								}
								catch (const std::exception&) {}
							}
						});

						// Return the timer.
						return timer;
					}

					///	<summary>
					///	Read the handshaking data.
					///	</summary>
					/// <param name="connection">The connection.</param>
					void read_handshake(std::shared_ptr<Connection> connection) 
					{
						connection->read_remote_endpoint_data();

						// Create new read_buffer for async_read_until()
						// Shared_ptr is used to pass temporary objects to the asynchronous functions
						std::shared_ptr<boost::asio::streambuf> read_buffer(new boost::asio::streambuf);

						// Set timeout on the following boost::asio::async-read or write function
						std::shared_ptr<boost::asio::deadline_timer> timer;

						if (timeout_request > 0)
							// Set the time out.
							timer = set_timeout_on_connection(connection, timeout_request);

						// Find path- and method-match, and generate response
						for (auto& endp : opt_endpoint)
						{
							// Timer idle initialise connection.
							timer_idle_init(connection, *endp.second);
							timer_connect_init(connection, *endp.second);
							timer_access_expiry_init(connection, *endp.second);
						}

						// Read all the data from the socket
						// to the response buffer until a '\r\n\r\n' is found.
						boost::asio::async_read_until(*connection->socket, *read_buffer, "\r\n\r\n", [this, connection, read_buffer, timer]
						(const boost::system::error_code& ec, size_t /*bytes_transferred*/) 
						{
							if (timeout_request > 0)
								// Cancle time out if time out as been reset.
								timer->cancel();

							// If no error.
							if (!ec) 
							{
								// Convert to istream to extract string-lines
								std::istream stream(read_buffer.get());
								parse_handshake(connection, stream);
								write_handshake(connection, read_buffer);
							}
							else
							{
								try
								{
									// Close socket.
									close_socket_connection(connection);
								}
								catch (const std::exception&) {}
							}
						});
					}

					///	<summary>
					///	Parse the handshaking data.
					///	</summary>
					/// <param name="connection">The connection.</param>
					/// <param name="stream">The stream.</param>
					void parse_handshake(std::shared_ptr<Connection> connection, std::istream& stream) const 
					{
						std::string line;
						getline(stream, line);
						size_t method_end;

						// Find the space.
						if ((method_end = line.find(' ')) != std::string::npos) 
						{
							size_t path_end;

							// Find the space.
							if ((path_end = line.find(' ', method_end + 1)) != std::string::npos) 
							{
								// Set the method and path.
								connection->method = line.substr(0, method_end);
								connection->path = line.substr(method_end + 1, path_end - method_end - 1);

								// Get the http version.
								if ((path_end + 6) < line.size())
									connection->http_version = line.substr(path_end + 6, line.size() - (path_end + 6) - 1);
								else
									connection->http_version = "1.1";

								// Get the next line.
								getline(stream, line);
								size_t param_end;

								// While ":" not found.
								while ((param_end = line.find(':')) != std::string::npos) 
								{
									size_t value_start = param_end + 1;

									// Line size.
									if ((value_start) < line.size()) 
									{
										if (line[value_start] == ' ')
											value_start++;

										// Add header.
										if (value_start < line.size())
											connection->header.insert(std::make_pair(line.substr(0, param_end), line.substr(value_start, line.size() - value_start - 1)));
									}

									// Get the next line.
									getline(stream, line);
								}
							}
						}
					}

					///	<summary>
					///	Write the handshaking data.
					///	</summary>
					/// <param name="connection">The connection.</param>
					/// <param name="read_buffer">The read stream buffer.</param>
					void write_handshake(std::shared_ptr<Connection> connection, std::shared_ptr<boost::asio::streambuf> read_buffer) 
					{
						// Find path- and method-match, and generate response
						for (auto& endp : opt_endpoint) 
						{
							boost::smatch path_match;

							// Match
							if (boost::regex_match(connection->path, path_match, endp.first)) 
							{
								// Write to the buffer.
								std::shared_ptr<boost::asio::streambuf> write_buffer(new boost::asio::streambuf);
								std::ostream handshake(write_buffer.get());

								// Generate response handshake.
								if (generate_handshake(connection, handshake)) 
								{
									connection->path_match = std::move(path_match);

									// Capture write_buffer in lambda so it is not destroyed before async_write is finished
									boost::asio::async_write(*connection->socket, *write_buffer, [this, connection, write_buffer, read_buffer, &endp]
									(const boost::system::error_code& ec, size_t /*bytes_transferred*/)
									{
										// If no error.
										if (!ec)
										{
											// Open connection and read.
											connection_open(connection, *endp.second);
											read_message(connection, read_buffer, *endp.second);
										}
										else
										{
											//connection_error(connection, *endp.second, ec);
											try
											{
												// Close socket.
												close_socket_connection(connection);
											}
											catch (const std::exception&) {}
										}
									});
								}
								return;
							}
						}
					}

					///	<summary>
					///	Generate the handshaking response.
					///	</summary>
					/// <param name="connection">The connection.</param>
					/// <param name="handshake">The handshake stream buffer.</param>
					/// <return>True if handshaking; else false.</return>
					bool generate_handshake(std::shared_ptr<Connection> connection, std::ostream& handshake) const 
					{
						// If "Sec-WebSocket-Key" does not exist.
						if (connection->header.count("Sec-WebSocket-Key") == 0)
							return 0;

						// Get hash of "Sec-WebSocket-Key"
						auto sha1 = Crypto::SHA1(connection->header["Sec-WebSocket-Key"] + ws_magic_string);

						// Write the handshaking response.
						handshake << "HTTP/1.1 101 Web Socket Protocol Handshake\r\n";
						handshake << "Upgrade: websocket\r\n";
						handshake << "Connection: Upgrade\r\n";
						handshake << "Sec-WebSocket-Accept: " << Crypto::Base64::encode(sha1) << "\r\n";
						handshake << "\r\n";

						return 1;
					}

					///	<summary>
					///	Read messsage.
					///	</summary>
					/// <param name="connection">The connection.</param>
					/// <param name="read_buffer">The read stream buffer.</param>
					/// <param name="endpoint">The endpoint.</param>
					void read_message(std::shared_ptr<Connection> connection,
						std::shared_ptr<boost::asio::streambuf> read_buffer, Endpoint& endpoint) const 
					{
						// Read message async.
						boost::asio::async_read(*connection->socket, *read_buffer, boost::asio::transfer_exactly(2), [this, connection, read_buffer, &endpoint]
						(const boost::system::error_code& ec, size_t bytes_transferred) 
						{
							// If no error.
							if (!ec) 
							{
								if (bytes_transferred == 0) 
								{ 
									//TODO: why does this happen sometimes?
									read_message(connection, read_buffer, endpoint);
									return;
								}

								std::istream stream(read_buffer.get());
								std::vector<unsigned char> first_bytes;
								first_bytes.resize(2);

								// Read some data.
								stream.read((char*)&first_bytes[0], 2);

								unsigned char fin_rsv_opcode = first_bytes[0];

								// Close connection if unmasked message from client (protocol error)
								if (first_bytes[1] < 128) 
								{
									const std::string reason("message from client not masked");
									send_close(connection, 1002, reason, [this, connection](const boost::system::error_code& /*ec*/) {});
									connection_close(connection, endpoint, 1002, reason);
									return;
								}

								size_t length = (first_bytes[1] & 127);

								if (length == 126) 
								{
									// 2 next bytes is the size of content
									boost::asio::async_read(*connection->socket, *read_buffer, boost::asio::transfer_exactly(2), [this, connection, read_buffer, &endpoint, fin_rsv_opcode]
									(const boost::system::error_code& ec, size_t /*bytes_transferred*/) 
									{
										// If no error.
										if (!ec) 
										{
											std::istream stream(read_buffer.get());
											std::vector<unsigned char> length_bytes;
											length_bytes.resize(2);

											// Read some data.
											stream.read((char*)&length_bytes[0], 2);

											size_t length = 0;
											int num_bytes = 2;

											for (int c = 0; c < num_bytes; c++)
												length += length_bytes[c] << (8 * (num_bytes - 1 - c));

											// Read messsage content.
											read_message_content(connection, read_buffer, length, endpoint, fin_rsv_opcode);
										}
										else
											connection_error(connection, endpoint, ec);
									});
								}
								else if (length == 127) 
								{
									// 8 next bytes is the size of content
									boost::asio::async_read(*connection->socket, *read_buffer, boost::asio::transfer_exactly(8), [this, connection, read_buffer, &endpoint, fin_rsv_opcode]
									(const boost::system::error_code& ec, size_t /*bytes_transferred*/) 
									{
										// If no error.
										if (!ec) 
										{
											std::istream stream(read_buffer.get());
											std::vector<unsigned char> length_bytes;
											length_bytes.resize(8);

											// Read some data.
											stream.read((char*)&length_bytes[0], 8);

											size_t length = 0;
											int num_bytes = 8;

											for (int c = 0; c < num_bytes; c++)
												length += length_bytes[c] << (8 * (num_bytes - 1 - c));

											// Read messsage content.
											read_message_content(connection, read_buffer, length, endpoint, fin_rsv_opcode);
										}
										else
											connection_error(connection, endpoint, ec);
									});
								}
								else
									// Read messsage content.
									read_message_content(connection, read_buffer, length, endpoint, fin_rsv_opcode);
							}
							else
								connection_error(connection, endpoint, ec);
						});
					}

					///	<summary>
					///	Read message content.
					///	</summary>
					/// <param name="connection">The connection.</param>
					/// <param name="read_buffer">The read stream buffer.</param>
					/// <param name="length">The buffer length.</param>
					/// <param name="endpoint">The connection endpoint.</param>
					/// <param name="fin_rsv_opcode">The operation op code.</param>
					void read_message_content(
						std::shared_ptr<Connection> connection,
						std::shared_ptr<boost::asio::streambuf> read_buffer,
						size_t length, Endpoint& endpoint, unsigned char fin_rsv_opcode) const 
					{
						// Read message async.
						boost::asio::async_read(*connection->socket, *read_buffer, boost::asio::transfer_exactly(4 + length), [this, connection, read_buffer, length, &endpoint, fin_rsv_opcode]
						(const boost::system::error_code& ec, size_t /*bytes_transferred*/) 
						{
							// If no error.
							if (!ec) 
							{
								std::istream raw_message_data(read_buffer.get());

								// Read mask
								std::vector<unsigned char> mask;
								mask.resize(4);
								raw_message_data.read((char*)&mask[0], 4);

								// New message.
								std::shared_ptr<Message> message(new Message());
								message->length = length;
								message->fin_rsv_opcode = fin_rsv_opcode;

								std::ostream message_data_out_stream(&message->streambuf);

								for (size_t c = 0; c < length; c++) 
								{
									// Add message to output stream.
									message_data_out_stream.put(raw_message_data.get() ^ mask[c % 4]);
								}

								// If connection close
								if ((fin_rsv_opcode & 0x0f) == 8) 
								{
									int status = 0;

									if (length >= 2) 
									{
										unsigned char byte1 = message->get();
										unsigned char byte2 = message->get();
										status = (byte1 << 8) + byte2;
									}

									// Close the connection op code.
									auto reason = message->string();
									send_close(connection, status, reason, [this, connection](const boost::system::error_code& /*ec*/) {});
									connection_close(connection, endpoint, status, reason);
									return;
								}
								else 
								{
									// If ping
									if ((fin_rsv_opcode & 0x0f) == 9) 
									{
										// Send pong
										auto empty_send_stream = std::make_shared<SendStream>();
										send(connection, empty_send_stream, nullptr, fin_rsv_opcode + 1);
									}
									else if (endpoint.onmessage) 
									{
										// Send message to enpoint.
										timer_idle_reset(connection);
										endpoint.onmessage(connection, message);

										// Set up access expiry handler.
										timer_access_expired_function(connection, message, endpoint);
									}

									// Next message
									read_message(connection, read_buffer, endpoint);
								}
							}
							else
								connection_error(connection, endpoint, ec);
						});
					}

					///	<summary>
					///	Open the connection.
					///	</summary>
					/// <param name="connection">The connection.</param>
					/// <param name="endpoint">The connection endpoint.</param>
					void connection_open(std::shared_ptr<Connection> connection, Endpoint& endpoint) 
					{
						// Add to collection.
						endpoint.connections_mutex.lock();
						endpoint.connections.insert(connection);
						endpoint.connections_mutex.unlock();

						// If on open function handler.
						if (endpoint.onopen)
							// Call on open handler.
							endpoint.onopen(connection);
					}

					///	<summary>
					///	Close the connection.
					///	</summary>
					/// <param name="connection">The connection.</param>
					/// <param name="endpoint">The connection endpoint.</param>
					/// <param name="status">The close status.</param>
					/// <param name="reason">The close reason.</param>
					void connection_close(std::shared_ptr<Connection> connection, Endpoint& endpoint, int status, const std::string& reason) const 
					{
						timer_idle_cancel(connection);
						timer_connect_cancel(connection);
						timer_access_expiry_cancel(connection);

						try
						{
							endpoint.connections_mutex.lock();
							endpoint.connections.erase(connection);
							endpoint.connections_mutex.unlock();
						}
						catch (const std::exception&) {}

						// If on close function handler.
						if (endpoint.onclose)
						{
							// Call on close handler.
							endpoint.onclose(connection, status, reason);
						}

						// Shutdown and close the client socket context.
						close_socket_connection(connection);
					}

					///	<summary>
					///	Close the connection.
					///	</summary>
					/// <param name="connection">The connection.</param>
					/// <param name="endpoint">The connection endpoint.</param>
					void connection_close(std::shared_ptr<Connection> connection, Endpoint& endpoint) const
					{
						timer_idle_cancel(connection);
						timer_connect_cancel(connection);
						timer_access_expiry_cancel(connection);

						try
						{
							// Remove
							endpoint.connections_mutex.lock();
							endpoint.connections.erase(connection);
							endpoint.connections_mutex.unlock();
						}
						catch (const std::exception&) {}

						// Shutdown and close the client socket context.
						close_socket_connection(connection);
					}

					///	<summary>
					///	Error the connection.
					///	</summary>
					/// <param name="connection">The connection.</param>
					/// <param name="endpoint">The connection endpoint.</param>
					/// <param name="ec">The error code.</param>
					void connection_error(std::shared_ptr<Connection> connection, Endpoint& endpoint, const boost::system::error_code& ec) const 
					{
						timer_idle_cancel(connection);
						timer_connect_cancel(connection);
						timer_access_expiry_cancel(connection);

						try
						{
							endpoint.connections_mutex.lock();
							endpoint.connections.erase(connection);
							endpoint.connections_mutex.unlock();
						}
						catch (const std::exception&) {}

						// If on error function handler.
						if (endpoint.onerror) 
						{
							// Call on error handler.
							boost::system::error_code ec_tmp = ec;
							endpoint.onerror(connection, ec_tmp);
						}
					}

					///	<summary>
					///	Timer idle initialise connection.
					///	</summary>
					/// <param name="connection">The connection.</param>
					/// <param name="endpoint">The connection endpoint.</param>
					void timer_idle_init(std::shared_ptr<Connection> connection, Endpoint& endpoint)
					{
						if (timeout_idle > 0) 
						{
							connection->timer_idle = std::unique_ptr<boost::asio::deadline_timer>(new boost::asio::deadline_timer(io_service));
							connection->timer_idle->expires_from_now(boost::posix_time::seconds(static_cast<unsigned long>(timeout_idle)));
							timer_idle_expired_function(connection, endpoint);
						}
					}

					///	<summary>
					///	Timer idle initialise connection.
					///	</summary>
					/// <param name="connection">The connection.</param>
					/// <param name="endpoint">The connection endpoint.</param>
					void timer_connect_init(std::shared_ptr<Connection> connection, Endpoint& endpoint)
					{
						if (timeout_connect > 0)
						{
							connection->timer_connect = std::unique_ptr<boost::asio::deadline_timer>(new boost::asio::deadline_timer(io_service));
							connection->timer_connect->expires_from_now(boost::posix_time::seconds(static_cast<unsigned long>(timeout_connect)));
							timer_connect_expired_function(connection, endpoint);
						}
					}

					///	<summary>
					///	Timer access expiry timeout.
					///	</summary>
					/// <param name="connection">The connection.</param>
					/// <param name="endpoint">The connection endpoint.</param>
					void timer_access_expiry_init(std::shared_ptr<Connection> connection, Endpoint& endpoint)
					{
						connection->timer_access_expiry = std::unique_ptr<boost::asio::deadline_timer>(new boost::asio::deadline_timer(io_service));
					}

					///	<summary>
					///	Timer idle reset connection.
					///	</summary>
					/// <param name="connection">The connection.</param>
					void timer_idle_reset(std::shared_ptr<Connection> connection) const
					{
						if (timeout_idle > 0 && connection->timer_idle->expires_from_now(boost::posix_time::seconds(static_cast<unsigned long>(timeout_idle))) > 0) 
						{
							timer_idle_expired_function_reset(connection);
						}
					}

					///	<summary>
					///	Timer idle cancel connection.
					///	</summary>
					/// <param name="connection">The connection.</param>
					void timer_idle_cancel(std::shared_ptr<Connection> connection) const 
					{
						try
						{
							if (timeout_idle > 0)
								connection->timer_idle->cancel();
						}
						catch (const std::exception&) {}
					}

					///	<summary>
					///	Timer connect cancel connection.
					///	</summary>
					/// <param name="connection">The connection.</param>
					void timer_connect_cancel(std::shared_ptr<Connection> connection) const
					{
						try
						{
							if (!connection->timeout_connect_cancelled)
								connection->timer_connect->cancel();
						}
						catch (const std::exception&) {}
					}

					///	<summary>
					///	Timer access expiry cancel connection.
					///	</summary>
					/// <param name="connection">The connection.</param>
					void timer_access_expiry_cancel(std::shared_ptr<Connection> connection) const
					{
						try
						{
							if (!connection->timeout_access_expiry_started)
								connection->timer_access_expiry->cancel();
						}
						catch (const std::exception&) {}	
					}

					///	<summary>
					///	Timer idle expired function connection.
					///	</summary>
					/// <param name="connection">The connection.</param>
					void timer_idle_expired_function_reset(std::shared_ptr<Connection> connection) const
					{
						connection->timer_idle->async_wait([this, connection](const boost::system::error_code& ec)
						{
							// If no error
							if (!ec)
							{
								// 1000=normal closure
								send_close(connection, 1000, "idle timeout");
							}
						});
					}

					///	<summary>
					///	Timer idle expired function connection.
					///	</summary>
					/// <param name="connection">The connection.</param>
					/// <param name="endpoint">The connection endpoint.</param>
					void timer_idle_expired_function(std::shared_ptr<Connection> connection, Endpoint& endpoint) const
					{
						connection->timer_idle->async_wait([this, connection, &endpoint](const boost::system::error_code& ec)
						{
							// If no error
							if (!ec) 
							{
								// 1000=normal closure
								send_close(connection, 1000, "idle timeout");

								try
								{
									endpoint.connections_mutex.lock();
									endpoint.connections.erase(connection);
									endpoint.connections_mutex.unlock();

									// Shutdown and close the client socket context.
									close_socket_connection(connection);
								}
								catch (const std::exception&) {}
							}
						});
					}

					///	<summary>
					///	Timer connect expired function connection.
					///	</summary>
					/// <param name="connection">The connection.</param>
					/// <param name="endpoint">The connection endpoint.</param>
					void timer_connect_expired_function(std::shared_ptr<Connection> connection, Endpoint& endpoint) const
					{
						connection->timer_connect->async_wait([this, connection, &endpoint](const boost::system::error_code& ec)
						{
							// If no error
							if (!ec)
							{
								// 1000=normal closure
								send_close(connection, 1000, "idle timeout");

								try
								{
									endpoint.connections_mutex.lock();
									endpoint.connections.erase(connection);
									endpoint.connections_mutex.unlock();

									// Shutdown and close the client socket context.
									close_socket_connection(connection);
								}
								catch (const std::exception&) {}
							}
						});
					}

					///	<summary>
					///	Timer access expired function connection.
					///	</summary>
					/// <param name="connection">The connection.</param>
					/// <param name="message">The message.</param>
					/// <param name="endpoint">The connection endpoint.</param>
					void timer_access_expired_function(std::shared_ptr<Connection> connection, std::shared_ptr<Message> message, Endpoint& endpoint) const
					{
						// If not init.
						if (!connection->timeout_access_expiry_async_started)
						{
							connection->timeout_access_expiry_async_started = true;
							connection->timer_access_expiry->async_wait([this, connection, message, &endpoint](const boost::system::error_code& ec)
							{
								// If no error
								if (!ec)
								{
									try
									{
										// If valid.
										if (connection != nullptr && message != nullptr && endpoint.onaccessexpiry)
										{
											// Send access expiry.
											endpoint.onaccessexpiry(connection, message);
										}
									}
									catch (const std::exception&) {}
								}
							});
						}
					}

					///	<summary>
					///	Close the current socket connection.
					///	</summary>
					/// <param name="connection">The connection.</param>
					void close_socket_connection(std::shared_ptr<Connection> connection) const
					{
						try
						{
							// Shutdown and close the client socket context.
							connection->socket->lowest_layer().shutdown(boost::asio::ip::tcp::socket::shutdown_both);
							connection->socket->lowest_layer().close();
							connection = nullptr;
						}
						catch (const std::exception&) {}
					}
				};

				///	<summary>
				///	WebSocket server.
				///	</summary>
				template<class socket_type>
				class SocketServer : public SocketServerBase<socket_type> {};

				///	<summary>
				///	WebSocket TCP socket.
				///	</summary>
				typedef boost::asio::ip::tcp::socket WS;

				///	<summary>
				///	WebSocket server.
				///	</summary>
				template<>
				class SocketServer<WS> : public SocketServerBase<WS> 
				{
				public:
					///	<summary>
					///	WebSocket server.
					///	</summary>
					/// <param name="port">The port number the server should listen on.</param>
					/// <param name="num_threads">The number of threads to use (set to 1 is more than statisfactory).</param>
					/// <param name="timeout_request">The request time out.</param>
					/// <param name="timeout_idle">The send and receive time out (600 seconds = 10 minutes).</param>
					/// <param name="timeout_connect">The time out (seconds) to connect.</param>
					/// <param name="ipv">The ip version.</param>
					SocketServer(unsigned short port, size_t num_threads = 1, IPVersionType ipv = IPVersionType::IPv4, 
						size_t timeout_request = 5, size_t timeout_idle = 0, size_t timeout_connect = 0) :
						SocketServerBase<WS>::SocketServerBase(port, num_threads, timeout_request, timeout_idle, timeout_connect, ipv) {};

				protected:
					///	<summary>
					///	Override the accept method.
					///	</summary>
					void accept() override
					{
						// Create new socket for this connection (stored in Connection::socket)
						// Shared_ptr is used to pass temporary objects to the asynchronous functions
						std::shared_ptr<Connection> connection(new Connection(new WS(io_service)));

						// Accept a new client connection, and using the new socket client created above.
						acceptor.async_accept(*connection->socket, [this, connection](const boost::system::error_code& ec) 
						{
							// Immediately start accepting a new connection
							accept();

							// If no error.
							if (!ec) 
							{
								// Set socket options.
								boost::asio::ip::tcp::no_delay option(true);
								connection->socket->set_option(option);

								// Create the web request and web context.
								auto webRequest = std::make_shared<WebRequest>();
								auto webMessage = std::make_shared<WebMessage>();

								// Assign the connection.
								connection->socketServer = this;
								webMessage->connectionHandler = std::static_pointer_cast<void>(connection);
								connection->webContext = std::make_shared<WebContext>(webRequest, webMessage);

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
				};
			}
		}
	}
}
#endif	/* SERVER_WS_HPP */
