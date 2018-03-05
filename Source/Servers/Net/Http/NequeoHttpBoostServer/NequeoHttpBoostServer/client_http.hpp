/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          client_http.h
*  Purpose :       Http client class.
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

#ifndef _CLIENT_HTTP_HPP
#define	_CLIENT_HTTP_HPP

#include "stdafx.h"
#include "IPVersionType.h"

#include <boost/asio.hpp>
#include <boost/utility/string_ref.hpp>
#include <boost/algorithm/string/predicate.hpp>
#include <boost/functional/hash.hpp>

#include <unordered_map>
#include <map>
#include <random>

namespace Nequeo {
	namespace Net {
		namespace Http {
			namespace Boost
			{
				/// <summary>
				/// Http client base.
				/// </summary>
				template <class socket_type>
				class ClientBase 
				{
				public:
					/// <summary>
					/// Http client base destructor.
					/// </summary>
					virtual ~ClientBase() {}

					/// <summary>
					/// Http client response.
					/// </summary>
					class Response 
					{
						/// <summary>
						/// Allow the client base to use private members.
						/// </summary>
						friend class ClientBase<socket_type>;

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
						std::string http_version, status_code;
						std::istream content;
						std::unordered_multimap<std::string, std::string, ihash, iequal_to> header;

					private:
						boost::asio::streambuf content_buffer;

						/// <summary>
						/// Http client response.
						/// </summary>
						Response() : content(&content_buffer) {}
					};

					///	<summary>
					///	Make a request.
					///	</summary>
					/// <param name="request_type">The request type (e.g. GET, POST).</param>
					/// <param name="path">The resource path.</param>
					/// <param name="content">The content to send.</param>
					/// <param name="header">The collection of header name value pairs.</param>
					void request
					(
						const std::string& request_type, const std::string& path = "/", boost::string_ref content = "",
						const std::map<std::string, std::string>& header = std::map<std::string, std::string>()) 
					{
						std::string corrected_path = path;

						// Add the '/' if does not exist.
						if (corrected_path == "")
							corrected_path = "/";

						// Create a new buffer and stream writer.
						boost::asio::streambuf write_buffer;
						std::ostream write_stream(&write_buffer);

						// Write the resource data.
						write_stream << request_type << " " << corrected_path << " HTTP/1.1\r\n";
						write_stream << "Host: " << host << "\r\n";

						// For each header.
						for (auto& h : header) 
						{
							// Write header name and value.
							write_stream << h.first << ": " << h.second << "\r\n";
						}

						// If content exists
						if (content.size() > 0)
							// Write content length.
							write_stream << "Content-Length: " << content.size() << "\r\n";

						// Write the end of all header new line.
						write_stream << "\r\n";

						try 
						{
							// Make a connection.
							connect();

							// Write the data from the buffer written to by the write stream.
							boost::asio::write(*socket, write_buffer);

							// If content exists.
							if (content.size() > 0)
								// Write all the content to the socket stream.
								boost::asio::write(*socket, boost::asio::buffer(content.data(), content.size()));

						}
						catch (const std::exception& e) 
						{
							// Get the error.
							socket_error = true;
							throw std::invalid_argument(e.what());
						}
					}

					///	<summary>
					///	Make a request.
					///	</summary>
					/// <param name="request_type">The request type (e.g. GET, POST).</param>
					/// <param name="path">The resource path.</param>
					/// <param name="content">The content to send.</param>
					/// <param name="header">The collection of header name value pairs.</param>
					void request
					(
						const std::string& request_type, const std::string& path, std::iostream& content,
						const std::map<std::string, std::string>& header = std::map<std::string, std::string>()) 
					{
						std::string corrected_path = path;

						// Add the '/' if does not exist.
						if (corrected_path == "")
							corrected_path = "/";

						// Seek to the begining of the content stream.
						content.seekp(0, std::ios::end);
						auto content_length = content.tellp();
						content.seekp(0, std::ios::beg);

						// Create a new buffer and stream writer.
						boost::asio::streambuf write_buffer;
						std::ostream write_stream(&write_buffer);

						// Write the resource data.
						write_stream << request_type << " " << corrected_path << " HTTP/1.1\r\n";
						write_stream << "Host: " << host << "\r\n";

						// For each header.
						for (auto& h : header) 
						{
							// Write header name and value.
							write_stream << h.first << ": " << h.second << "\r\n";
						}

						// If content exists
						if (content_length > 0)
							// Write content length.
							write_stream << "Content-Length: " << content_length << "\r\n";

						// Write the end of all header new line.
						write_stream << "\r\n";

						// If content exists.
						if (content_length > 0)
							// Write the content stream data to the write stream.
							write_stream << content.rdbuf();

						try 
						{
							// Make a connection.
							connect();

							// Write the data from the buffer written to by the write stream.
							boost::asio::write(*socket, write_buffer);
						}
						catch (const std::exception& e) 
						{
							// Get the error.
							socket_error = true;
							throw std::invalid_argument(e.what());
						}
					}

					///	<summary>
					///	Read the response.
					///	</summary>
					///	<return>The response.</return>
					std::shared_ptr<Response> request_read()
					{
						// Create a new response.
						std::shared_ptr<Response> response(new Response());

						try
						{
							// Read all the data from the socket
							// to the response buffer until a '\r\n\r\n' is found.
							size_t bytes_transferred = boost::asio::read_until(*socket, response->content_buffer, "\r\n\r\n");

							// The number of additional bytes, content data may
							// exist after the header data.
							size_t num_additional_bytes = response->content_buffer.size() - bytes_transferred;

							// Get all header data, the response content buffer
							// at this time only contains the header information.
							parse_response_header(response, response->content);

							// Find the 'Content-Length' header.
							auto header_it = response->header.find("Content-Length");

							// If the 'Content-Length' exists.
							if (header_it != response->header.end())
							{
								// Get the 'Content-Length' header value.
								auto content_length = stoull(header_it->second);

								// If 'Content-Length' value is greater then additional bytes
								// then content data exists start reading the rest of the
								// data from the socket to the content buffer in the response.
								if (content_length > num_additional_bytes)
								{
									// Read from the socket the rest of the content length data.
									boost::asio::read(*socket, response->content_buffer, boost::asio::transfer_exactly(content_length - num_additional_bytes));
								}
							}
							else if ((header_it = response->header.find("Transfer-Encoding")) != response->header.end() && header_it->second == "chunked")
							{
								// If the 'Transfer-Encoding' exists and the value is 'chunked'
								// then the content data is sent chunked, read the data using
								// the chunked alogorithm.

								// Create the chunked content buffer and write stream.
								boost::asio::streambuf streambuf;
								std::ostream content(&streambuf);

								std::streamsize length;
								std::string buffer;

								do
								{
									// Read all the data until '\r\n' is found.
									size_t bytes_transferred = boost::asio::read_until(*socket, response->content_buffer, "\r\n");

									std::string line;

									// Get the next line of data.
									getline(response->content, line);
									bytes_transferred -= line.size() + 1;

									// Get the line
									line.pop_back();

									// Get the line length.
									length = stol(line, 0, 16);

									// The number of bytes to
									// read for this chunk.
									auto num_additional_bytes = static_cast<std::streamsize>(response->content_buffer.size() - bytes_transferred);

									// If more chunks exist.
									if ((2 + length) > num_additional_bytes)
									{
										// Read the chunk from the socket.
										boost::asio::read(*socket, response->content_buffer, boost::asio::transfer_exactly(2 + length - num_additional_bytes));
									}

									// Write the content to the stream.
									buffer.resize(static_cast<size_t>(length));
									response->content.read(&buffer[0], length);
									content.write(&buffer[0], length);

									//Remove "\r\n"
									response->content.get();
									response->content.get();

								} while (length > 0);

								// Create the response stream from the response content buffer.
								std::ostream response_content_output_stream(&response->content_buffer);

								// Read the response content to the stream.
								response_content_output_stream << content.rdbuf();
							}
						}
						catch (const std::exception& e)
						{
							// Get the error.
							socket_error = true;
							throw std::invalid_argument(e.what());
						}

						// Return the response.
						return response;
					}

				protected:
					boost::asio::io_service io_service;
					boost::asio::ip::tcp::endpoint endpoint;
					boost::asio::ip::tcp::resolver resolver;

					IPVersionType _ipv;
					std::shared_ptr<socket_type> socket;
					bool socket_error;

					std::string host;
					unsigned short port;

					///	<summary>
					///	Http client base.
					///	</summary>
					/// <param name="host_port">The server name or IP with specific port (10.1.1.1:444 port is optional).</param>
					/// <param name="default_port">The defualt port number.</param>
					/// <param name="ipv">The ip version.</param>
					ClientBase(const std::string& host_port, unsigned short default_port, IPVersionType ipv) :
						resolver(io_service), socket_error(false), _ipv(ipv)
					{
						// If a port number on on the host then find the position.
						size_t host_end = host_port.find(':');

						// If no port has been found at the end.
						if (host_end == std::string::npos) 
						{
							// Use host and port
							host = host_port;
							port = default_port;
						}
						else 
						{
							// Get the host excluding the port number, and get the port without the host.
							host = host_port.substr(0, host_end);
							port = static_cast<unsigned short>(stoul(host_port.substr(host_end + 1)));
						}

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
					///	Read the response headers.
					///	</summary>
					/// <param name="response">The response data.</param>
					/// <param name="stream">The input response stream.</param>
					void parse_response_header(std::shared_ptr<Response> response, std::istream& stream) const 
					{
						std::string line;

						// Read the first line.
						getline(stream, line);

						// Find the spaces.
						size_t version_end = line.find(' ');

						// If a space exists.
						if (version_end != std::string::npos) 
						{
							// If less than four lines.
							if (5 < line.size())
								// Get the version number.
								response->http_version = line.substr(5, version_end - 5);

							// Find the response status.
							if ((version_end + 1) < line.size())
								// Get the response status.
								response->status_code = line.substr(version_end + 1, line.size() - (version_end + 1) - 1);

							// Read the next header line.
							getline(stream, line);
							size_t param_end;

							// While headers still exist, if no other name value pair exists
							// seperated with the ':'.
							while ((param_end = line.find(':')) != std::string::npos) 
							{
								size_t value_start = param_end + 1;

								// If more header data exists.
								if ((value_start) < line.size()) 
								{
									if (line[value_start] == ' ')
										value_start++;

									if (value_start < line.size())
										// Add the header name value pair to the list.
										response->header.insert(std::make_pair(line.substr(0, param_end), line.substr(value_start, line.size() - value_start - 1)));
								}

								// Read the next header line.
								getline(stream, line);
							}
						}
					}
				};

				///	<summary>
				///	Socket client.
				///	</summary>
				template<class socket_type>
				class Client : public ClientBase<socket_type> {};

				///	<summary>
				///	Http TCP socket.
				///	</summary>
				typedef boost::asio::ip::tcp::socket HTTP;

				///	<summary>
				///	Http client.
				///	</summary>
				template<>
				class Client<HTTP> : public ClientBase<HTTP> 
				{
				public:
					///	<summary>
					///	Http client.
					///	</summary>
					/// <param name="server_port_path">The server name or IP with specific port (10.1.1.1:88) if port is not specified to defualt port 80 is used.</param>
					/// <param name="ipv">The ip version.</param>
					Client(const std::string& server_port_path, IPVersionType ipv = IPVersionType::IPv4) : ClientBase<HTTP>::ClientBase(server_port_path, 80, ipv)
					{
						// Create the secure TCP socket.
						socket = std::make_shared<HTTP>(io_service);
					}

				protected:
					///	<summary>
					///	Override the connect method.
					///	</summary>
					void connect() override
					{
						// If no socet error and is open.
						if (socket_error || !socket->is_open()) 
						{
							// Create a query and make a connection.
							boost::asio::ip::tcp::resolver::query query(host, std::to_string(port));
							boost::asio::connect(*socket, resolver.resolve(query));

							// Set the delay options.
							boost::asio::ip::tcp::no_delay option(true);
							socket->set_option(option);

							// No error.
							socket_error = false;
						}
					}
				};
			}
		}
	}
}
#endif	/* CLIENT_HTTP_HPP */
