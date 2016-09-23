/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          server_http.h
*  Purpose :       Http server class.
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

#ifndef _SERVER_HTTP_HPP
#define	_SERVER_HTTP_HPP

#include "stdafx.h"
#include "IPVersionType.h"

#include <boost/asio.hpp>
#include <boost/regex.hpp>
#include <boost/algorithm/string/predicate.hpp>
#include <boost/functional/hash.hpp>

#include <unordered_map>
#include <thread>
#include <functional>
#include <iostream>
#include <sstream>

namespace Nequeo {
	namespace Net {
		namespace Http {
			namespace Boost
			{
				/// <summary>
				/// Http server base.
				/// </summary>
				template <class socket_type>
				class ServerBase {
				public:
					/// <summary>
					/// Http server base destructor.
					/// </summary>
					virtual ~ServerBase() {}

					/// <summary>
					/// Http server response.
					/// </summary>
					class Response : public std::ostream 
					{
						/// <summary>
						/// Allow the server base to use private members.
						/// </summary>
						friend class ServerBase<socket_type>;

						boost::asio::streambuf streambuf;
						std::shared_ptr<socket_type> socket;

						/// <summary>
						/// Http server response.
						/// </summary>
						/// <param name="socket">The accepted client socket.</param>
						Response(std::shared_ptr<socket_type> socket) : std::ostream(&streambuf), socket(socket) {}

					public:
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
					/// Http server content.
					/// </summary>
					class Content : public std::istream 
					{
						/// <summary>
						/// Allow the server base to use private members.
						/// </summary>
						friend class ServerBase<socket_type>;

					public:
						/// <summary>
						/// Get the stream buffer size.
						/// </summary>
						/// <return>The stream buffer size.</return>
						size_t size() 
						{
							return streambuf.size();
						}

						/// <summary>
						/// Get the stream buffer data as a string.
						/// </summary>
						/// <return>The stream buffer.</return>
						std::string string() 
						{
							std::stringstream ss;
							ss << rdbuf();
							return ss.str();
						}

					private:
						boost::asio::streambuf &streambuf;

						/// <summary>
						/// Http server content.
						/// </summary>
						/// <param name="streambuf">The stream buffer.</param>
						Content(boost::asio::streambuf &streambuf) : std::istream(&streambuf), streambuf(streambuf) {}
					};

					/// <summary>
					/// Http server request.
					/// </summary>
					class Request 
					{
						/// <summary>
						/// Allow the server base to use private members.
						/// </summary>
						friend class ServerBase<socket_type>;

						/// <summary>
						/// Equal to class. Based on http://www.boost.org/doc/libs/1_60_0/doc/html/unordered/hash_equality.html
						/// </summary>
						class iequal_to 
						{
						public:
							/// <summary>
							/// () operator on two keys.
							/// </summary>
							bool operator()(const std::string &key1, const std::string &key2) const 
							{
								return boost::algorithm::iequals(key1, key2);
							}
						};

						/// <summary>
						/// Hashing class.
						/// </summary>
						class ihash 
						{
						public:
							/// <summary>
							/// () operator on a key to return seed.
							/// </summary>
							size_t operator()(const std::string &key) const 
							{
								std::size_t seed = 0;
								for (auto &c : key)
									boost::hash_combine(seed, std::tolower(c));

								return seed;
							}
						};

					public:
						std::string method, path, http_version;
						Content content;
						std::unordered_multimap<std::string, std::string, ihash, iequal_to> header;
						boost::smatch path_match;
						std::string remote_endpoint_address;
						unsigned short remote_endpoint_port;

					private:
						/// <summary>
						/// Http client response.
						/// </summary>
						Request() : content(streambuf) {}

						boost::asio::streambuf streambuf;
					};

					/// <summary>
					/// Http server configuration.
					/// </summary>
					class Config 
					{
						/// <summary>
						/// Allow the server base to use private members.
						/// </summary>
						friend class ServerBase<socket_type>;

						///	<summary>
						///	Http server configuration.
						///	</summary>
						/// <param name="port">The port number.</param>
						/// <param name="num_threads">The number of threads.</param>
						Config(unsigned short port, size_t num_threads) : num_threads(num_threads), port(port), reuse_address(true) {}

						size_t num_threads;

					public:
						unsigned short port;

						// IPv4 address in dotted decimal form or IPv6 address in hexadecimal notation.
						// If empty, the address will be any address.
						std::string address;

						// Set to false to avoid binding the socket to an address that is already in use.
						bool reuse_address;
					};

					// Set before calling start().
					Config config;

					///	<summary>
					///	Client context resource.
					///	</summary>
					std::unordered_map<std::string, std::unordered_map<std::string,
						std::function<void(std::shared_ptr<typename ServerBase<socket_type>::Response>, std::shared_ptr<typename ServerBase<socket_type>::Request>)> > >  resource;

					///	<summary>
					///	Client context default resource.
					///	</summary>
					std::unordered_map<std::string,
						std::function<void(std::shared_ptr<typename ServerBase<socket_type>::Response>, std::shared_ptr<typename ServerBase<socket_type>::Request>)> > default_resource;

					///	<summary>
					///	Client exception function handler.
					///	</summary>
					std::function<void(const std::exception&)> exception_handler;

					std::string default_resource_method;

				private:
					///	<summary>
					///	Client context option resource.
					///	</summary>
					std::vector<std::pair<std::string, std::vector<std::pair<boost::regex,
						std::function<void(std::shared_ptr<typename ServerBase<socket_type>::Response>, std::shared_ptr<typename ServerBase<socket_type>::Request>)> > > > > opt_resource;

				public:
					///	<summary>
					///	Start the server.
					///	</summary>
					void start() 
					{
						// Copy the resources to opt_resource for more efficient request processing.
						opt_resource.clear();

						// For each resource context.
						for (auto& res : resource) 
						{
							// For each resource context method.
							for (auto& res_method : res.second) 
							{
								// End of resource.
								auto it = opt_resource.end();

								// For each resource option context.
								for (auto opt_it = opt_resource.begin(); opt_it != opt_resource.end(); opt_it++) 
								{
									// Find the match.
									if (res_method.first == opt_it->first) 
									{
										it = opt_it;
										break;
									}
								}

								// If end.
								if (it == opt_resource.end()) 
								{
									// Set the request method options
									opt_resource.emplace_back();
									it = opt_resource.begin() + (opt_resource.size() - 1);
									it->first = res_method.first;
								}

								// Set the request path option.
								it->second.emplace_back(boost::regex(res.first), res_method.second);
							}
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

						// If num_threads > 1, start m_io_service.run() in (num_threads-1) threads for thread-pooling.
						threads.clear();
						for (size_t c = 1; c < config.num_threads; c++) 
						{
							threads.emplace_back([this]() 
							{
								// Start a new service.
								io_service.run();
							});
						}

						// Main thread.
						io_service.run();

						// Wait for the rest of the threads, if any, to finish as well.
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
							acceptor.close();
							io_service.stop();
						}
						catch (...) {}
					}

					///	<summary>
					///	Send data to the client from the response. Use this function if you need to recursively send parts of a longer message.
					///	</summary>
					/// <param name="response">The client context response to send.</param>
					/// <param name="callback">The callback with error if any.</param>
					void send(std::shared_ptr<Response> response, const std::function<void(const boost::system::error_code&)>& callback = nullptr) const
					{
						// Send the response data back to the context client.
						boost::asio::async_write(*response->socket, response->streambuf, [this, response, callback](const boost::system::error_code& ec, size_t /*bytes_transferred*/)
						{
							// If a callback has been set.
							if (callback)
								// Execute the callback, send to the calling context.
								callback(ec);
						});
					}

				protected:
					boost::asio::io_service io_service;
					boost::asio::ip::tcp::acceptor acceptor;
					std::vector<std::thread> threads;
					IPVersionType _ipv;

					long timeout_request;
					long timeout_content;

					/// <summary>
					/// Http server base.
					/// </summary>
					/// <param name="port">The port number the server should listen on.</param>
					/// <param name="num_threads">The number of threads to use (set to 1 is more than statisfactory).</param>
					/// <param name="timeout_request">The request time out.</param>
					/// <param name="timeout_send_or_receive">The send and receive time out.</param>
					/// <param name="ipv">The ip version.</param>
					ServerBase(unsigned short port, size_t num_threads, long timeout_request, long timeout_send_or_receive, IPVersionType ipv) :
						config(port, num_threads), acceptor(io_service), _ipv(ipv), default_resource_method("DEFAULT_METHOD"),
						timeout_request(timeout_request), timeout_content(timeout_send_or_receive) {}

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
					std::shared_ptr<boost::asio::deadline_timer> set_timeout_on_socket(std::shared_ptr<socket_type> socket, long seconds) 
					{
						std::shared_ptr<boost::asio::deadline_timer> timer(new boost::asio::deadline_timer(io_service));
						timer->expires_from_now(boost::posix_time::seconds(seconds));

						// If the timeout is triggered then close the socket.
						timer->async_wait([socket](const boost::system::error_code& ec) 
						{
							// If no error.
							if (!ec) 
							{
								boost::system::error_code ec;

								// Shutdown and close the client socket context.
								socket->lowest_layer().shutdown(boost::asio::ip::tcp::socket::shutdown_both, ec);
								socket->lowest_layer().close();
							}
						});

						// Return the timer.
						return timer;
					}

					///	<summary>
					///	Read the request and content.
					///	</summary>
					/// <param name="socket">The socket context.</param>
					void read_request_and_content(std::shared_ptr<socket_type> socket) 
					{
						// Create new streambuf (Request::streambuf) for async_read_until()
						// shared_ptr is used to pass temporary objects to the asynchronous functions
						std::shared_ptr<Request> request(new Request());

						try 
						{
							// Set the request client remote endpoint.
							request->remote_endpoint_address = socket->lowest_layer().remote_endpoint().address().to_string();
							request->remote_endpoint_port = socket->lowest_layer().remote_endpoint().port();
						}
						catch (const std::exception &e) 
						{
							// Send the error for the context.
							if (exception_handler)
								exception_handler(e);
						}

						// Set timeout on the following boost::asio::async-read or write function
						std::shared_ptr<boost::asio::deadline_timer> timer;
						if (timeout_request > 0)
							// Set the timer.
							timer = set_timeout_on_socket(socket, timeout_request);

						// Start reading async all the data from the socket until '\r\n\r\n' is found
						// this will read all the headers
						boost::asio::async_read_until(*socket, request->streambuf, "\r\n\r\n",
							[this, socket, request, timer](const boost::system::error_code& ec, size_t bytes_transferred) 
						{
							if (timeout_request > 0)
								// Cancel the timer if data has come in on time.
								timer->cancel();

							// If no error.
							if (!ec) 
							{
								// request->streambuf.size() is not necessarily the same as bytes_transferred, from Boost-docs:
								// "After a successful async_read_until operation, the streambuf may contain additional data beyond the delimiter"
								// The chosen solution is to extract lines from the stream directly when parsing the header. What is left of the
								// streambuf (maybe some bytes of the content) is appended to in the async_read-function below (for retrieving content).
								size_t num_additional_bytes = request->streambuf.size() - bytes_transferred;

								// If the request data has not been passed.
								if (!parse_request(request, request->content))
									return;

								// If content, read that as well.
								auto it = request->header.find("Content-Length");

								// Read the content if exists.
								if (it != request->header.end()) 
								{
									// Set timeout on the following boost::asio::async-read or write function
									std::shared_ptr<boost::asio::deadline_timer> timer;
									if (timeout_content > 0)
										// Set the timeout.
										timer = set_timeout_on_socket(socket, timeout_content);

									unsigned long long content_length;

									try 
									{
										// Get the content length.
										content_length = std::stoull(it->second);
									}
									catch (const std::exception &e) 
									{
										// If error.
										if (exception_handler)
											exception_handler(e);
										return;
									}

									// If content data exists read it.
									if (content_length > num_additional_bytes) 
									{
										// Start reading the content data to the exact length.
										boost::asio::async_read(*socket, request->streambuf,
											boost::asio::transfer_exactly(content_length - num_additional_bytes),
											[this, socket, request, timer]
										(const boost::system::error_code& ec, size_t /*bytes_transferred*/) 
										{
											if (timeout_content > 0)
												timer->cancel();

											// If no error find the resource.
											if (!ec)
												find_resource(socket, request);
										});
									}
									else 
									{
										if (timeout_content > 0)
											timer->cancel();

										// Find the resource.
										find_resource(socket, request);
									}
								}
								else 
								{
									// Find the resource.
									find_resource(socket, request);
								}
							}
						});
					}

					///	<summary>
					///	Parse the request and content.
					///	</summary>
					/// <param name="request">The current request.</param>
					/// <param name="stream">The request stream containing the request.</param>
					/// <return>True if parsed; else false..</return>
					bool parse_request(std::shared_ptr<Request> request, std::istream& stream) const 
					{
						std::string line;

						// Get the first line.
						getline(stream, line);
						size_t method_end;

						// Find the request method.
						if ((method_end = line.find(' ')) != std::string::npos) 
						{
							size_t path_end;

							// Find the end of the method.
							if ((path_end = line.find(' ', method_end + 1)) != std::string::npos) 
							{
								// Set the request method and the resource request path.
								request->method = line.substr(0, method_end);
								request->path = line.substr(method_end + 1, path_end - method_end - 1);

								size_t protocol_end;

								// Find the protocol.
								if ((protocol_end = line.find('/', path_end + 1)) != std::string::npos) 
								{
									// If the protocol can not be found the end the parser.
									if (line.substr(path_end + 1, protocol_end - path_end - 1) != "HTTP")
										return false;

									// Extract the protocol.
									request->http_version = line.substr(protocol_end + 1, line.size() - protocol_end - 2);
								}
								else
									return false;

								// Get the next line.
								getline(stream, line);
								size_t param_end;

								// Get each line until no more.
								while ((param_end = line.find(':')) != std::string::npos) 
								{
									size_t value_start = param_end + 1;

									if ((value_start) < line.size()) 
									{
										if (line[value_start] == ' ')
											value_start++;

										if (value_start < line.size())
											// Add the request header to the request.
											request->header.insert(std::make_pair(line.substr(0, param_end), line.substr(value_start, line.size() - value_start - 1)));
									}

									// Get the next line.
									getline(stream, line);
								}
							}
							else
								return false;
						}
						else
							return false;

						return true;
					}

					///	<summary>
					///	Find the resource.
					///	</summary>
					/// <param name="socket">The current socket context.</param>
					/// <param name="request">The request context.</param>
					void find_resource(std::shared_ptr<socket_type> socket, std::shared_ptr<Request> request) 
					{
						// Find path- and method-match, and call write_response
						for (auto& res : opt_resource) 
						{
							if (request->method == res.first) 
							{
								for (auto& res_path : res.second) 
								{
									boost::smatch sm_res;

									// Find the match for the rquest.
									if (boost::regex_match(request->path, sm_res, res_path.first)) 
									{
										// Move the data.
										request->path_match = std::move(sm_res);

										// Write the response to the socket context.
										write_response(socket, request, res_path.second);
										return;
									}
								}
							}
						}

						// Find the request method.
						auto it_method = default_resource.find(request->method);

						// If found.
						if (it_method != default_resource.end()) 
						{
							// Write the response to the socket context.
							write_response(socket, request, it_method->second);
						}
						else
						{
							// Find the default of all methods, no matter wat the request method.
							auto it_method_default = default_resource.find(default_resource_method);

							// If found.
							if (it_method_default != default_resource.end())
							{
								// Write the response to the socket context.
								write_response(socket, request, it_method_default->second);
							}
						}
					}

					///	<summary>
					///	Write the response.
					///	</summary>
					/// <param name="socket">The current socket context.</param>
					/// <param name="request">The request context.</param>
					/// <param name="resource_function">The resource function to call.</param>
					void write_response(
						std::shared_ptr<socket_type> socket, std::shared_ptr<Request> request,
						std::function<void(std::shared_ptr<typename ServerBase<socket_type>::Response>, std::shared_ptr<typename ServerBase<socket_type>::Request>)>& resource_function) 
					{
						// Set timeout on the following boost::asio::async-read or write function
						std::shared_ptr<boost::asio::deadline_timer> timer;
						if (timeout_content > 0)
							// Create a timer.
							timer = set_timeout_on_socket(socket, timeout_content);

						// Create the response.
						auto response = std::shared_ptr<Response>(new Response(socket), [this, request, timer](Response *response_ptr) 
						{
							// Get the current response context.
							auto response = std::shared_ptr<Response>(response_ptr);

							// Send the response to the client.
							send(response, [this, response, request, timer](const boost::system::error_code& ec) 
							{
								// If no error.
								if (!ec) 
								{
									if (timeout_content > 0)
										timer->cancel();

									float http_version;

									try 
									{
										// Get the http version.
										http_version = stof(request->http_version);
									}
									catch (const std::exception &e) 
									{
										// Call the error handler.
										if (exception_handler)
											exception_handler(e);
										return;
									}

									// Get the 'Connection' header.
									auto range = request->header.equal_range("Connection");

									// For the 'Connection' header get the name and value.
									for (auto it = range.first; it != range.second; it++) 
									{
										// If the connection is 'close'
										// the return a close the connection.
										if (boost::iequals(it->second, "close"))
											return;
									}

									// If http protocol 1.05.
									if (http_version > 1.05)
										// Read more data.
										read_request_and_content(response->socket);
								}
							});
						});

						try 
						{
							// Call the resource function.
							resource_function(response, request);
						}
						catch (const std::exception &e) 
						{
							if (exception_handler)
								exception_handler(e);
							return;
						}
					}
				};

				///	<summary>
				///	Socket server.
				///	</summary>
				template<class socket_type>
				class Server : public ServerBase<socket_type> {};

				///	<summary>
				///	Http TCP socket.
				///	</summary>
				typedef boost::asio::ip::tcp::socket HTTP;

				///	<summary>
				///	Http server.
				///	</summary>
				template<>
				class Server<HTTP> : public ServerBase<HTTP> 
				{
				public:
					///	<summary>
					///	Http server.
					///	</summary>
					/// <param name="port">The port number the server should listen on.</param>
					/// <param name="num_threads">The number of threads to use (set to 1 is more than statisfactory).</param>
					/// <param name="timeout_request">The request time out.</param>
					/// <param name="timeout_content">The send and receive time out.</param>
					/// <param name="ipv">The ip version.</param>
					Server(unsigned short port, size_t num_threads = 1, IPVersionType ipv = IPVersionType::IPv4, long timeout_request = 5, long timeout_content = 300) :
						ServerBase<HTTP>::ServerBase(port, num_threads, timeout_request, timeout_content, ipv) {}

				protected:
					///	<summary>
					///	Override the accept method.
					///	</summary>
					void accept() override 
					{
						// Create new socket for this connection.
						// Shared_ptr is used to pass temporary objects to the asynchronous functions.
						std::shared_ptr<HTTP> socket(new HTTP(io_service));

						// Accept a new client connection, and using the new socket client created above.
						acceptor.async_accept(*socket, [this, socket](const boost::system::error_code& ec) 
						{
							// Immediately start accepting a new connection.
							accept();

							// If no error.
							if (!ec) 
							{
								// Set socket options.
								boost::asio::ip::tcp::no_delay option(true);
								socket->set_option(option);

								// Read the request data from the client.
								read_request_and_content(socket);
							}
						});
					}
				};
			}
		}
	}
}
#endif	/* SERVER_HTTP_HPP */
