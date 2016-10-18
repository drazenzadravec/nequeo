/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WebServer.cpp
*  Purpose :       Http web server class.
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
#include "stdafx.cpp"

#include "WebServer.h"
#include "server_http.hpp"
#include "server_https.hpp"

using namespace Nequeo::Net::Http;

std::atomic<int> serverCount;
concurrency::concurrent_unordered_map<int, std::shared_ptr<InternalHttpServer>> serverPtr;
concurrency::concurrent_unordered_map<int, std::shared_ptr<InternalSecureHttpServer>> serverSecurePtr;

void Accept(WebServer*, std::shared_ptr<InternalHttpServer>, std::function<void(const WebContext*)>);
void AcceptSecure(WebServer*, std::shared_ptr<InternalSecureHttpServer>, std::function<void(const WebContext*)>);

void StopAccept(std::shared_ptr<InternalHttpServer>);
void StopAcceptSecure(std::shared_ptr<InternalSecureHttpServer>);

void MakeWebContext(std::shared_ptr<WebContext>, std::shared_ptr<InternalHttpServer::Response>, std::shared_ptr<InternalHttpServer::Request>);
void MakeSecureWebContext(std::shared_ptr<WebContext>, std::shared_ptr<InternalSecureHttpServer::Response>, std::shared_ptr<InternalSecureHttpServer::Request>);


///	<summary>
///	Http web server.
///	</summary>
/// <param name="port">The listening port number.</param>
/// <param name="ipv">The IP version to use.</param>
/// <param name="isSecure">Is the server secure (must set the public and private key files).</param>
WebServer::WebServer(unsigned short port, IPVersionType ipv, bool isSecure) :
	_disposed(false), _listening(false), _isSecure(isSecure), _port(port),
	_internalThread(false), _ipv(ipv), _hasEndpoint(false), _serverName("Nequeo Web Server 16.26.1.1"),
	_serverIndex(-1), _endpoint("")
{
}

/// <summary>
/// Http web server.
/// </summary>
/// <param name="port">The listening port number.</param>
/// <param name="endpoint">The endpoint address to listen on.</param>
/// <param name="isSecure">Is the server secure (must set the public and private key files).</param>
WebServer::WebServer(unsigned short port, const std::string& endpoint, bool isSecure) :
	_disposed(false), _listening(false), _isSecure(isSecure), _port(port),
	_internalThread(false), _endpoint(endpoint), _hasEndpoint(true), _serverName("Nequeo Web Server 16.26.1.1"),
	_serverIndex(-1)
{
	_ipv = IPVersionType::IPv4;
}

///	<summary>
///	Http web server.
///	</summary>
WebServer::~WebServer()
{
	if (!_disposed)
	{
		_disposed = true;

		// Stop listening.
		Stop();
		StopThread();

		if (_serverIndex >= 0)
		{
			serverPtr[_serverIndex] = nullptr;
		}

		if (_serverIndex >= 0)
		{
			serverSecurePtr[_serverIndex] = nullptr;
		}
	}

	_listening = false;
	_serverIndex = -1;
}

/// <summary>
/// On web context request.
/// </summary>
/// <param name="webContext">The web context callback function.</param>
void WebServer::OnWebContext(const WebContextHandler& webContext)
{
	_onWebContext = webContext;
}

///	<summary>
///	Stop the server.
///	</summary>
void WebServer::Stop()
{
	// If listening.
	if (_listening)
	{
		// If not secure.
		if (!_isSecure)
		{
			// Stop listening.
			if (_serverIndex >= 0)
				StopAccept(serverPtr[_serverIndex]);
		}
		else
		{
			// Stop listening.
			if (_serverIndex >= 0)
				StopAcceptSecure(serverSecurePtr[_serverIndex]);
		}

		_listening = false;
	}
}

///	<summary>
///	Stop the server.
///	</summary>
void WebServer::StopThread()
{
	// Stop the server.
	Stop();

	// If an internal thread was created.
	if (_internalThread)
	{
		try
		{
			_internalThread = false;
			_thread.join();
		}
		catch (...) {}
	}
}

///	<summary>
///	Start the server.
///	</summary>
void WebServer::StartThread()
{
	// If an internal thread has not been created.
	if (!_internalThread)
	{
		// Move-assign threads
		_internalThread = true;
		_thread = std::thread(std::bind(&WebServer::Start, this));
	}
}

///	<summary>
///	Start the server.
///	</summary>
void WebServer::Start()
{
	// If not listening.
	if (!_listening)
	{
		// If not secure.
		if (!_isSecure)
		{
			// If not created.
			if (_serverIndex < 0)
			{
				++serverCount;
				_serverIndex = serverCount;

				// HTTP-server at port using 1 thread
				// Unless you do more heavy non-threaded processing in the resources,
				// 1 thread is usually faster than several threads
				serverPtr.insert(std::make_pair(_serverIndex, std::make_shared<InternalHttpServer>(_port, 1, _ipv)));

				// If an enpoint exists.
				if (_hasEndpoint)
					serverPtr[_serverIndex]->config.address = _endpoint;
			}

			// Start accepting;
			Accept(this, serverPtr[_serverIndex], _onWebContext);
		}
		else
		{
			// If not created.
			if (_serverIndex < 0)
			{
				++serverCount;
				_serverIndex = serverCount;

				// HTTPS-server at port using 1 thread
				// Unless you do more heavy non-threaded processing in the resources,
				// 1 thread is usually faster than several threads
				serverSecurePtr.insert(std::make_pair(_serverIndex, std::make_shared<InternalSecureHttpServer>(_port, 1, _publicKeyFile, _privateKeyFile, _ipv)));

				// If an enpoint exists.
				if (_hasEndpoint)
					serverSecurePtr[_serverIndex]->config.address = _endpoint;
			}

			// Start accepting;
			AcceptSecure(this, serverSecurePtr[_serverIndex], _onWebContext);
		}

		_listening = true;
	}
}

/// <summary>
/// On web context request.
/// </summary>
/// <param name="publicKeyFile">The public certificate file path.</param>
/// <param name="privateKeyFile">The private (un-encrypted) key file.</param>
void WebServer::SetSecurePublicPrivateKeys(const std::string& publicKeyFile, const std::string& privateKeyFile)
{
	_isSecure = true;
	_publicKeyFile = publicKeyFile;
	_privateKeyFile = privateKeyFile;
}

/// <summary>
/// Is the server listening.
/// </summary>
/// <return>True if the server is listening; else false.</return>
bool WebServer::IsListening() const
{
	return _listening;
}

/// <summary>
/// Is the server secure.
/// </summary>
/// <return>True if the server is secure; else false.</return>
bool WebServer::IsSecure() const
{
	return _isSecure;
}

/// <summary>
/// The IP version type.
/// </summary>
/// <return>The IP version type.</return>
IPVersionType WebServer::IPVersion() const
{
	return _ipv;
}

/// <summary>
/// Gets or sets the server name.
/// </summary>
/// <param name="serverName">The server name.</param>
/// <return>The servername.</return>
const std::string& WebServer::GetServerName() const
{
	return _serverName;
}
void WebServer::SetServerName(const std::string& serverName)
{
	_serverName = serverName;
}

/// <summary>
/// Get the port number.
/// </summary>
/// <return>The port number.</return>
unsigned short WebServer::Port() const
{
	return _port;
}

/// <summary>
/// Get the endpoint.
/// </summary>
/// <return>The endpoint.</return>
const std::string& WebServer::GetEndpoint() const
{
	return _endpoint;
}

///	<summary>
///	Accept connections for the server.
///	</summary>
/// <param name="webServer">The web server instance.</param>
/// <param name="server">The server instance.</param>
/// <param name="handler">The web context callback function.</param>
void Accept(WebServer* webServer, std::shared_ptr<InternalHttpServer> server, std::function<void(const WebContext*)> handler)
{
	// Default GET-DEFAULT_METHOD. If no other matches, this anonymous function will be called. 
	server->default_resource["DEFAULT_METHOD"] = [&webServer, &server, &handler](std::shared_ptr<InternalHttpServer::Response> response, std::shared_ptr<InternalHttpServer::Request> request)
	{
		// Create the web request and response.
		auto webRequest = std::make_shared<WebRequest>();
		auto webResponse = std::make_shared<WebResponse>();

		// Create a new web context.
		auto webContext = std::make_shared<WebContext>(webRequest, webResponse);
		
		webContext->SetIsSecure(webServer->IsSecure());
		webContext->SetIPVersionType(webServer->IPVersion());
		webContext->SetPort(webServer->Port());
		webContext->SetServerName(webServer->GetServerName());
		
		// Make the web context.
		MakeWebContext(webContext, response, request);

		// Execute the web context handler.
		handler(webContext.get());
	};

	// Start the server.
	server->start();
}

///	<summary>
///	Accept connections for the server.
///	</summary>
/// <param name="webServer">The web server instance.</param>
/// <param name="server">The server instance.</param>
/// <param name="handler">The web context callback function.</param>
void AcceptSecure(WebServer* webServer, std::shared_ptr<InternalSecureHttpServer> server, std::function<void(const WebContext*)> handler)
{
	// Default GET-DEFAULT_METHOD. If no other matches, this anonymous function will be called. 
	server->default_resource["DEFAULT_METHOD"] = [&webServer, &server, &handler](std::shared_ptr<InternalSecureHttpServer::Response> response, std::shared_ptr<InternalSecureHttpServer::Request> request)
	{
		// Create the web request and response.
		auto webRequest = std::make_shared<WebRequest>();
		auto webResponse = std::make_shared<WebResponse>();

		// Create a new web context.
		auto webContext = std::make_shared<WebContext>(webRequest, webResponse);

		webContext->SetIsSecure(webServer->IsSecure());
		webContext->SetIPVersionType(webServer->IPVersion());
		webContext->SetPort(webServer->Port());
		webContext->SetServerName(webServer->GetServerName());

		// Make the web context.
		MakeSecureWebContext(webContext, response, request);

		// Execute the web context handler.
		handler(webContext.get());
	};

	// Start the server.
	server->start();
}

///	<summary>
///	Stop the server.
///	</summary>
/// <param name="server">The server instance.</param>
void StopAccept(std::shared_ptr<InternalHttpServer> server)
{
	server->stop();
}

///	<summary>
///	Stop the server.
///	</summary>
/// <param name="server">The server instance.</param>
void StopAcceptSecure(std::shared_ptr<InternalSecureHttpServer> server)
{
	server->stop();
}

///	<summary>
///	Create the web context from the response and request.
///	</summary>
/// <param name="context">The web context.</param>
/// <param name="response">The response stream.</param>
/// <param name="request">The request stream.</param>
void MakeWebContext(std::shared_ptr<WebContext> context, std::shared_ptr<InternalHttpServer::Response> response, std::shared_ptr<InternalHttpServer::Request> request)
{
	// Get the request and response.
	WebRequest* webRequest = &context->Request();
	WebResponse* webResponse = &context->Response();

	// Set the internal request and response.
	webResponse->internalResponse = response.get();
	webResponse->isSecure = context->IsSecure();

	// Assign values.
	webRequest->SetMethod(request->method);
	webRequest->SetPath(request->path);
	webRequest->SetProtocolVersion(request->http_version);
	webRequest->SetRemoteEndpointAddress(request->remote_endpoint_address);
	webRequest->SetRemoteEndpointPort(request->remote_endpoint_port);

	// Assign the headers.
	std::map<std::string, std::string> headers;
	typedef std::pair<std::string, std::string> headerPair;

	// For each header.
	for (auto& h : request->header)
	{
		// Write header name and value.
		headers.insert(headerPair(h.first.c_str(), h.second.c_str()));
	}
	
	// Assign the header and content.
	webRequest->SetHeaders(headers);
	webRequest->Content = request->content.rdbuf();

	// Find the 'Content-Length' header.
	auto content_length_it = request->header.find("Content-Length");
	if (content_length_it != request->header.end())
	{
		// Get the 'Content-Length' header value.
		auto content_length = std::stoull(content_length_it->second);
		webRequest->SetContentLength(content_length);
	}

	// Find the 'Content-Type' header.
	auto content_type_it = request->header.find("Content-Type");
	if (content_type_it != request->header.end())
	{
		// Get the 'Content-Type' header value.
		auto content_type = content_type_it->second;
		webRequest->SetContentType(content_type);
	}

	// Find the 'Accept-Encoding' header.
	auto accept_encoding_it = request->header.find("Accept-Encoding");
	if (accept_encoding_it != request->header.end())
	{
		// Get the 'Accept-Encoding' header value.
		auto accept_encoding = accept_encoding_it->second;
		webRequest->SetAcceptEncoding(accept_encoding);
	}
}

///	<summary>
///	Create the web context from the response and request.
///	</summary>
/// <param name="context">The web context.</param>
/// <param name="response">The response stream.</param>
/// <param name="request">The request stream.</param>
void MakeSecureWebContext(std::shared_ptr<WebContext> context, std::shared_ptr<InternalSecureHttpServer::Response> response, std::shared_ptr<InternalSecureHttpServer::Request> request)
{
	// Get the request and response.
	WebRequest* webRequest = &context->Request();
	WebResponse* webResponse = &context->Response();

	// Set the internal request and response.
	webResponse->internalResponse = response.get();
	webResponse->isSecure = context->IsSecure();

	// Assign values.
	webRequest->SetMethod(request->method);
	webRequest->SetPath(request->path);
	webRequest->SetProtocolVersion(request->http_version);
	webRequest->SetRemoteEndpointAddress(request->remote_endpoint_address);
	webRequest->SetRemoteEndpointPort(request->remote_endpoint_port);

	// Assign the headers.
	std::map<std::string, std::string> headers;
	typedef std::pair<std::string, std::string> headerPair;

	// For each header.
	for (auto& h : request->header)
	{
		// Write header name and value.
		headers.insert(headerPair(h.first.c_str(), h.second.c_str()));
	}

	// Assign the header and content.
	webRequest->SetHeaders(headers);
	webRequest->Content = request->content.rdbuf();

	// Find the 'Content-Length' header.
	auto content_length_it = request->header.find("Content-Length");
	if (content_length_it != request->header.end())
	{
		// Get the 'Content-Length' header value.
		auto content_length = std::stoull(content_length_it->second);
		webRequest->SetContentLength(content_length);
	}

	// Find the 'Content-Type' header.
	auto content_type_it = request->header.find("Content-Type");
	if (content_type_it != request->header.end())
	{
		// Get the 'Content-Type' header value.
		auto content_type = content_type_it->second;
		webRequest->SetContentType(content_type);
	}

	// Find the 'Accept-Encoding' header.
	auto accept_encoding_it = request->header.find("Accept-Encoding");
	if (accept_encoding_it != request->header.end())
	{
		// Get the 'Accept-Encoding' header value.
		auto accept_encoding = accept_encoding_it->second;
		webRequest->SetAcceptEncoding(accept_encoding);
	}
}
