/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          client_ws.h
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

#ifndef CLIENT_WS_HPP
#define	CLIENT_WS_HPP

#include "stdafx.h"
#include "IPVersionType.h"
#include "crypto.hpp"

#include <boost/asio.hpp>

#include <unordered_map>
#include <iostream>
#include <random>
#include <atomic>
#include <list>

namespace Nequeo {
	namespace Net {
		namespace WebSocket {
			namespace Boost
			{
				/// <summary>
				/// WebSocket client.
				/// </summary>
				template <class socket_type>
				class SocketClient;

				/// <summary>
				/// WebSocket client base.
				/// </summary>
				template <class socket_type>
				class SocketClientBase 
				{
				public:
					/// <summary>
					/// WebSocket client base destructor.
					/// </summary>
					virtual ~SocketClientBase() { connection.reset(); }

					/// <summary>
					/// WebSocket client response.
					/// </summary>
					class SendStream : public std::iostream 
					{
						/// <summary>
						/// Allow the client base to use private members.
						/// </summary>
						friend class SocketClientBase<socket_type>;

					private:
						boost::asio::streambuf streambuf;

					public:
						/// <summary>
						/// WebSocket client response.
						/// </summary>
						SendStream() : std::iostream(&streambuf) {}

						///	<summary>
						///	Get the stream buffer size.
						///	</summary>
						///	<return>The size.</return>
						size_t size() 
						{
							return streambuf.size();
						}
					};

					/// <summary>
					/// WebSocket client connection.
					/// </summary>
					class Connection 
					{
						/// <summary>
						/// Allow the client base to use private members.
						/// </summary>
						friend class SocketClientBase<socket_type>;
						friend class SocketClient<socket_type>;

					public:
						std::unordered_map<std::string, std::string> header;
						std::string remote_endpoint_address;
						unsigned short remote_endpoint_port;

					private:
						/// <summary>
						/// WebSocket client connection.
						/// </summary>
						/// <param name="socket">The socket type.</param>
						Connection(socket_type* socket) : socket(socket), strand(socket->get_io_service()), closed(false) {}

						/// <summary>
						/// WebSocket client send data.
						/// </summary>
						class SendData 
						{
						public:
							/// <summary>
							/// WebSocket client send data.
							/// </summary>
							/// <param name="send_stream">The send stream.</param>
							/// <param name="callback">The error callback.</param>
							SendData(std::shared_ptr<SendStream> send_stream, const std::function<void(const boost::system::error_code)> &callback) :
								send_stream(send_stream), callback(callback) {}

							std::shared_ptr<SendStream> send_stream;
							std::function<void(const boost::system::error_code)> callback;
						};

						std::unique_ptr<socket_type> socket;
						boost::asio::strand strand;
						std::list<SendData> send_queue;

						///	<summary>
						///	Send data from queue.
						///	</summary>
						void send_from_queue() 
						{
							strand.post([this]() 
							{
								// Write send buffer.
								boost::asio::async_write(*socket, send_queue.begin()->send_stream->streambuf,
									strand.wrap([this](const boost::system::error_code& ec, size_t /*bytes_transferred*/) 
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
											send_from_queue();
									}
									else
										send_queue.clear();
								}));
							});
						}

						std::atomic<bool> closed;

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

					std::shared_ptr<Connection> connection;

					/// <summary>
					/// WebSocket client message.
					/// </summary>
					class Message : public std::istream 
					{
						friend class SocketClientBase<socket_type>;

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
						///	Get the sream buffer.
						///	</summary>
						///	<return>The size.</return>
						std::string string() 
						{
							std::stringstream ss;
							ss << rdbuf();
							return ss.str();
						}
					private:
						/// <summary>
						/// WebSocket client message.
						/// </summary>
						Message() : std::istream(&streambuf) {}

						size_t length;
						boost::asio::streambuf streambuf;
					};

					std::function<void(void)> onopen;
					std::function<void(std::shared_ptr<Message>)> onmessage;
					std::function<void(const boost::system::error_code&)> onerror;
					std::function<void(int, const std::string&)> onclose;

					///	<summary>
					///	Start a connection.
					///	</summary>
					void start() 
					{
						// Connect.
						connect();

						// Run the service.
						io_service.run();
					}

					///	<summary>
					///	Stop a connection.
					///	</summary>
					void stop() 
					{
						// Stop the service.
						io_service.stop();
					}

					/// <summary>
					/// Send the data.
					/// </summary>
					/// <param name="message_stream">The message stream.</param>
					/// <param name="callback">The error callback.</param>
					/// <param name="fin_rsv_opcode">The op code.</param>
					///	<remarks>
					/// fin_rsv_opcode: 129=one fragment, text, 130=one fragment, binary, 136=close connection.
					/// See http://tools.ietf.org/html/rfc6455#section-5.2 for more information
					///	</remarks>
					void send(
						std::shared_ptr<SendStream> message_stream, 
						const std::function<void(const boost::system::error_code&)>& callback = nullptr,
						unsigned char fin_rsv_opcode = 129) 
					{
						// Create mask
						std::vector<unsigned char> mask;
						mask.resize(4);
						std::uniform_int_distribution<unsigned short> dist(0, 255);
						std::random_device rd;

						// For the first chars.
						for (int c = 0; c < 4; c++) 
						{
							// Add the mask.
							mask[c] = static_cast<unsigned char>(dist(rd));
						}

						// Make shared pointer.
						auto send_stream = std::make_shared<SendStream>();

						// Message length.
						size_t length = message_stream->size();

						// Sent the op code.
						send_stream->put(fin_rsv_opcode);

						// Masked (first length byte>=128)
						if (length >= 126) 
						{
							int num_bytes;
							if (length > 0xffff) 
							{
								// Op code.
								num_bytes = 8;
								send_stream->put(static_cast<unsigned char>(127 + 128));
							}
							else 
							{
								// Op code.
								num_bytes = 2;
								send_stream->put(static_cast<unsigned char>(126 + 128));
							}

							for (int c = num_bytes - 1; c >= 0; c--) 
							{
								// Op code.
								send_stream->put((length >> (8 * c)) % 256);
							}
						}
						else
							// Op code.
							send_stream->put(static_cast<unsigned char>(length + 128));

						for (int c = 0; c < 4; c++) 
						{
							// Op code.
							send_stream->put(mask[c]);
						}

						for (size_t c = 0; c < length; c++) 
						{
							// Op code.
							send_stream->put(message_stream->get() ^ mask[c % 4]);
						}

						connection->strand.post([this, send_stream, callback]() 
						{
							// Send the queued data.
							connection->send_queue.emplace_back(send_stream, callback);

							// If data exists in queue.
							if (connection->send_queue.size() == 1)
								// Send the next queued data.
								connection->send_from_queue();
						});
					}

					/// <summary>
					/// Send close connection data.
					/// </summary>
					/// <param name="status">The close status.</param>
					/// <param name="reason">The close reason.</param>
					/// <param name="callback">The callback.</param>
					void send_close(
						int status, 
						const std::string& reason = "", 
						const std::function<void(const boost::system::error_code&)>& callback = nullptr) 
					{
						// Send close only once (in case close is initiated by client)
						if (connection->closed.load()) 
						{
							return;
						}

						// Close the connection.
						connection->closed.store(true);

						// Make shared pointer.
						auto send_stream = std::make_shared<SendStream>();

						send_stream->put(status >> 8);
						send_stream->put(status % 256);
						*send_stream << reason;

						// fin_rsv_opcode=136: message close
						send(send_stream, callback, 136);
					}

				protected:
					const std::string ws_magic_string = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

					boost::asio::io_service io_service;
					boost::asio::ip::tcp::endpoint endpoint;
					boost::asio::ip::tcp::resolver resolver;

					std::string host;
					unsigned short port;
					std::string path;
					IPVersionType _ipv;

					///	<summary>
					///	WebSocket client base.
					///	</summary>
					/// <param name="host_port_path">The server name or IP with specific port (10.1.1.1:444 port is optional).</param>
					/// <param name="default_port">The defualt port number.</param>
					/// <param name="ipv">The ip version.</param>
					SocketClientBase(
						const std::string& host_port_path, 
						unsigned short default_port, 
						IPVersionType ipv) :
						resolver(io_service), _ipv(ipv)
					{
						// If a port number on on the host then find the position.
						size_t host_end = host_port_path.find(':');
						size_t host_port_end = host_port_path.find('/');

						// If no port has been found at the end.
						if (host_end == std::string::npos) 
						{
							host_end = host_port_end;
							port = default_port;
						}
						else 
						{
							// Get the port.
							if (host_port_end == std::string::npos)
								port = (unsigned short)stoul(host_port_path.substr(host_end + 1));
							else
								port = (unsigned short)stoul(host_port_path.substr(host_end + 1, host_port_end - (host_end + 1)));
						}

						// Get the path.
						if (host_port_end == std::string::npos) 
						{
							path = "/";
						}
						else 
						{
							path = host_port_path.substr(host_port_end);
						}

						// Get the host.
						if (host_end == std::string::npos)
							host = host_port_path;
						else
							host = host_port_path.substr(0, host_end);

						// Select the ip version.
						switch (_ipv)
						{
						case IPVersionType::IPv6:
							endpoint = boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v6(), port);
							break;

						default:
							// Create a IPv4 TCP endpoint.
							endpoint = boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v4(), port);
							break;
						}
					}

					///	<summary>
					///	Virtual connect method must be overriden.
					///	</summary>
					virtual void connect() = 0;

					///	<summary>
					///	Start handshaking.
					///	</summary>
					void handshake() 
					{
						connection->read_remote_endpoint_data();

						// Create write buffer pointer.
						std::shared_ptr<boost::asio::streambuf> write_buffer(new boost::asio::streambuf);

						// Create write stream.
						std::ostream request(write_buffer.get());

						// Write the request data.
						request << "GET " << path << " HTTP/1.1" << "\r\n";
						request << "Host: " << host << "\r\n";
						request << "Upgrade: websocket\r\n";
						request << "Connection: Upgrade\r\n";

						// Make random 16-byte nonce
						std::string nonce;
						nonce.resize(16);
						std::uniform_int_distribution<unsigned short> dist(0, 255);
						std::random_device rd;

						for (int c = 0; c < 16; c++)
							nonce[c] = static_cast<unsigned char>(dist(rd));

						// Write the request web socket headers.
						std::string nonce_base64 = Crypto::Base64::encode(nonce);
						request << "Sec-WebSocket-Key: " << nonce_base64 << "\r\n";
						request << "Sec-WebSocket-Version: 13\r\n";
						request << "\r\n";

						// Test this to base64::decode(Sec-WebSocket-Accept)
						std::shared_ptr<std::string> accept_sha1(new std::string(Crypto::SHA1(nonce_base64 + ws_magic_string)));

						// Write the request async.
						boost::asio::async_write(*connection->socket, *write_buffer, [this, write_buffer, accept_sha1]
						(const boost::system::error_code& ec, size_t /*bytes_transferred*/) 
						{
							// If no error.
							if (!ec) 
							{
								// Create a message pointer.
								std::shared_ptr<Message> message(new Message());

								// Read all the data from the socket
								// to the response buffer until a '\r\n\r\n' is found.
								boost::asio::async_read_until(*connection->socket, message->streambuf, "\r\n\r\n", [this, message, accept_sha1]
								(const boost::system::error_code& ec, size_t /*bytes_transferred*/) 
								{
									// If no error.
									if (!ec) 
									{
										// Parse the message returned.
										parse_handshake(*message);

										// Decode the message.
										if (Crypto::Base64::decode(connection->header["Sec-WebSocket-Accept"]) == *accept_sha1) 
										{
											// Open the connection.
											if (onopen)
												onopen();

											// Read the complete message.
											read_message(message);
										}
										else
											throw std::invalid_argument("WebSocket handshake failed");
									}
								});
							}
							else
								throw std::invalid_argument("Failed sending handshake");
						});
					}

					///	<summary>
					///	Parse the handshaking data.
					///	</summary>
					/// <param name="stream">The stream.</param>
					void parse_handshake(std::istream& stream) const 
					{
						std::string line;
						getline(stream, line);
						// Not parsing the first line

						getline(stream, line);
						size_t param_end;

						// Read until the end of the message.
						while ((param_end = line.find(':')) != std::string::npos) 
						{
							size_t value_start = param_end + 1;

							if ((value_start) < line.size()) 
							{
								if (line[value_start] == ' ')
									value_start++;

								if (value_start < line.size())
									connection->header.insert(std::make_pair(line.substr(0, param_end), line.substr(value_start, line.size() - value_start - 1)));
							}

							// Get the next line of the message.
							getline(stream, line);
						}
					}

					///	<summary>
					///	Read message.
					///	</summary>
					/// <param name="message">The message pointer.</param>
					void read_message(std::shared_ptr<Message> message) 
					{
						// Start async read.
						boost::asio::async_read(*connection->socket, message->streambuf, boost::asio::transfer_exactly(2),
							[this, message](const boost::system::error_code& ec, size_t bytes_transferred) 
						{
							// If no error.
							if (!ec) 
							{
								// If no data transfered.
								if (bytes_transferred == 0) 
								{ 
									//TODO: This might happen on server at least, might also happen here.
									read_message(message);
									return;
								}

								std::vector<unsigned char> first_bytes;
								first_bytes.resize(2);

								// Read the message opcode.
								message->read((char*)&first_bytes[0], 2);
								message->fin_rsv_opcode = first_bytes[0];

								// Close connection if masked message from server (protocol error)
								if (first_bytes[1] >= 128) 
								{
									const std::string reason("message from server masked");
									auto kept_connection = connection;

									// Send close connection.
									send_close(1002, reason, [this, kept_connection](const boost::system::error_code& /*ec*/) {});

									// If on close function handler.
									if (onclose)
										// Call on close.
										onclose(1002, reason);

									return;
								}

								// Get the message length.
								size_t length = (first_bytes[1] & 127);

								if (length == 126) 
								{
									// 2 next bytes is the size of content
									boost::asio::async_read(*connection->socket, message->streambuf, boost::asio::transfer_exactly(2), [this, message]
									(const boost::system::error_code& ec, size_t /*bytes_transferred*/) 
									{
										if (!ec) 
										{
											std::vector<unsigned char> length_bytes;
											length_bytes.resize(2);
											message->read((char*)&length_bytes[0], 2);

											size_t length = 0;
											int num_bytes = 2;

											// Get payload length.
											for (int c = 0; c < num_bytes; c++)
												length += length_bytes[c] << (8 * (num_bytes - 1 - c));

											// Read the message.
											message->length = length;
											read_message_content(message);
										}
										else 
										{
											// If on error function handler.
											if (onerror)
												// Call on error.
												onerror(ec);
										}
									});
								}
								else if (length == 127) 
								{
									// 8 next bytes is the size of content
									boost::asio::async_read(*connection->socket, message->streambuf, boost::asio::transfer_exactly(8), [this, message]
									(const boost::system::error_code& ec, size_t /*bytes_transferred*/) 
									{
										if (!ec) 
										{
											std::vector<unsigned char> length_bytes;
											length_bytes.resize(8);
											message->read((char*)&length_bytes[0], 8);

											size_t length = 0;
											int num_bytes = 8;

											// Get payload length.
											for (int c = 0; c < num_bytes; c++)
												length += length_bytes[c] << (8 * (num_bytes - 1 - c));

											// Read the message.
											message->length = length;
											read_message_content(message);
										}
										else 
										{
											// If on error function handler.
											if (onerror)
												// Call on error.
												onerror(ec);
										}
									});
								}
								else 
								{
									// Read the message.
									message->length = length;
									read_message_content(message);
								}
							}
							else 
							{
								// If on error function handler.
								if (onerror)
									// Call on error.
									onerror(ec);
							}
						});
					}

					///	<summary>
					///	Read message content.
					///	</summary>
					/// <param name="message">The message pointer.</param>
					void read_message_content(std::shared_ptr<Message> message) 
					{
						// Start async read.
						boost::asio::async_read(*connection->socket, message->streambuf, boost::asio::transfer_exactly(message->length), [this, message]
						(const boost::system::error_code& ec, size_t /*bytes_transferred*/) 
						{
							// If no error.
							if (!ec) 
							{
								// If connection close
								if ((message->fin_rsv_opcode & 0x0f) == 8) 
								{
									int status = 0;

									// If message exists.
									if (message->length >= 2) 
									{
										unsigned char byte1 = message->get();
										unsigned char byte2 = message->get();
										status = (byte1 << 8) + byte2;
									}

									auto reason = message->string();
									auto kept_connection = connection;

									// Send close message.
									send_close(status, reason, [this, kept_connection](const boost::system::error_code& /*ec*/) {});

									// If on close function handler.
									if (onclose)
										// Call on close.
										onclose(status, reason);

									return;
								}
								// If ping
								else if ((message->fin_rsv_opcode & 0x0f) == 9)
								{
									// Send pong.
									auto empty_send_stream = std::make_shared<SendStream>();
									send(empty_send_stream, nullptr, message->fin_rsv_opcode + 1);
								}
								else if (onmessage) 
								{
									// Call on message.
									onmessage(message);
								}

								// Next message.
								std::shared_ptr<Message> next_message(new Message());
								read_message(next_message);
							}
							else 
							{
								// If on error function handler.
								if (onerror)
									// Call on error.
									onerror(ec);
							}
						});
					}
				};

				///	<summary>
				///	Socket client.
				///	</summary>
				template<class socket_type>
				class SocketClient : public SocketClientBase<socket_type> {};

				///	<summary>
				///	WebSocket TCP socket.
				///	</summary>
				typedef boost::asio::ip::tcp::socket WS;

				///	<summary>
				///	WebSocket client.
				///	</summary>
				template<>
				class SocketClient<WS> : public SocketClientBase<WS> 
				{
				public:
					///	<summary>
					///	WebSocket client.
					///	</summary>
					/// <param name="server_port_path">The server name or IP with specific port (10.1.1.1:88) if port is not specified to defualt port 80 is used.</param>
					/// <param name="ipv">The ip version.</param>
					SocketClient(const std::string& server_port_path, IPVersionType ipv = IPVersionType::IPv4) : SocketClientBase<WS>::SocketClientBase(server_port_path, 80, ipv) {};

				protected:
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
								connection = std::shared_ptr<Connection>(new Connection(new WS(io_service)));

								// New async connection.
								boost::asio::async_connect(*connection->socket, it, [this](const boost::system::error_code &ec, boost::asio::ip::tcp::resolver::iterator /*it*/) 
								{
									// If no error.
									if (!ec) 
									{
										// Set the delay options.
										boost::asio::ip::tcp::no_delay option(true);
										connection->socket->set_option(option);

										// Handshake.
										handshake();
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
#endif	/* CLIENT_WS_HPP */
