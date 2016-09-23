/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          client_https.h
*  Purpose :       Secure Http client class.
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

#ifndef _CLIENT_HTTPS_HPP
#define	_CLIENT_HTTPS_HPP

#include "stdafx.h"
#include "client_http.hpp"

#include <boost/asio/ssl.hpp>

namespace Nequeo {
	namespace Net {
		namespace Http {
			namespace Boost
			{
				///	<summary>
				///	Secure Http TCP socket.
				///	</summary>
				typedef boost::asio::ssl::stream<boost::asio::ip::tcp::socket> HTTPS;

				///	<summary>
				///	Secure Http client.
				///	</summary>
				template<>
				class Client<HTTPS> : public ClientBase<HTTPS> 
				{
				public:
					///	<summary>
					///	Secure Http client.
					///	</summary>
					/// <param name="server_port_path">The server name or IP with specific port (10.1.1.1:444) if port is not specified to defualt port 443 is used.</param>
					/// <param name="verify_certificate">Should the certificate be verified.</param>
					/// <param name="ipv">The ip version.</param>
					/// <param name="cert_file">The certificate file path.</param>
					/// <param name="private_key_file">The private (un-encrypted) key file.</param>
					/// <param name="verify_file">The verificate file path.</param>
					Client(const std::string& server_port_path, IPVersionType ipv = IPVersionType::IPv4, bool verify_certificate = false,
						const std::string& cert_file = std::string(), const std::string& private_key_file = std::string(),
						const std::string& verify_file = std::string()) :
						ClientBase<HTTPS>::ClientBase(server_port_path, 443, ipv), context(boost::asio::ssl::context::tlsv12)
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

						// Create the secure TCP socket.
						socket = std::make_shared<HTTPS>(io_service, context);
					}

				protected:
					// Current ssl context.
					boost::asio::ssl::context context;

					///	<summary>
					///	Override the connect method.
					///	</summary>
					void connect() override
					{
						// If no socet error and is open.
						if (socket_error || !socket->lowest_layer().is_open()) 
						{
							// Create a query and make a connection.
							boost::asio::ip::tcp::resolver::query query(host, std::to_string(port));
							boost::asio::connect(socket->lowest_layer(), resolver.resolve(query));

							// Set the delay options.
							boost::asio::ip::tcp::no_delay option(true);
							socket->lowest_layer().set_option(option);

							// Authenticate as the client, start the handshake and negosiation.
							socket->handshake(boost::asio::ssl::stream_base::client);

							// No error.
							socket_error = false;
						}
					}
				};
			}
		}
	}
}
#endif	/* CLIENT_HTTPS_HPP */
