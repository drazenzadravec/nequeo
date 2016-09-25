/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WebClient.cpp
*  Purpose :       Http web client class.
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

#include "WebClient.h"
#include "client_http.hpp"
#include "client_https.hpp"

using namespace Nequeo::Net::Http;

static const char* WEBCLIENT_CLIENT_TAG = "NequeoWebClient";
static const char* NETCONTEXT_CLIENT_TAG = "NequeoNetContext";

std::atomic<int> clientCount;
concurrency::concurrent_unordered_map<int, std::shared_ptr<InternalHttpClient>> clientPtr;
concurrency::concurrent_unordered_map<int, std::shared_ptr<InternalSecureHttpClient>> clientSecurePtr;

/// <summary>
/// Http web client.
/// </summary>
/// <param name="host">The host (name or IP).</param>
/// <param name="port">The host port number.</param>
/// <param name="isSecure">Is the connection secure.</param>
/// <param name="ipv">The IP version to use.</param>
WebClient::WebClient(const std::string& host, unsigned short port, bool isSecure, IPVersionType ipv) :
	_disposed(false), _active(false), _isSecure(isSecure), _clientIndex(-1), _ipv(ipv), _port(port), _host(host)
{
	// Create a new executor.
	_executor = Nequeo::MakeShared<Nequeo::Threading::DefaultExecutor>(WEBCLIENT_CLIENT_TAG);

	// Create a new context.
	CreateNetContext();
}

///	<summary>
///	Http web client.
///	</summary>
WebClient::~WebClient()
{
	if (!_disposed)
	{
		_disposed = true;

		if (_clientIndex >= 0)
		{
			clientPtr[_clientIndex] = nullptr;
		}

		if (_clientIndex >= 0)
		{
			clientSecurePtr[_clientIndex] = nullptr;
		}
	}

	_active = false;
	_clientIndex = -1;
}

///	<summary>
///	Create a new context.
///	</summary>
void WebClient::CreateNetContext()
{
	// If not active.
	if (!_active)
	{
		// Create a new context.
		_context = Nequeo::MakeShared<NetContext>(NETCONTEXT_CLIENT_TAG);
		_context->SetPort(_port);
		_context->SetIsSecure(_isSecure);
		_context->SetIPVersionType(_ipv);

		// Get the request and response.
		NetRequest* netRequest = &_context->Request();
		NetResponse* netResponse = &_context->Response();

		// If not secure.
		if (!_isSecure)
		{
			// If not created.
			if (_clientIndex < 0)
			{
				++clientCount;
				_clientIndex = clientCount;

				// HTTP client
				clientPtr.insert(std::make_pair(_clientIndex, std::make_shared<InternalHttpClient>(_host + ":" + std::to_string(_port), _ipv)));

				// Assign the current client to the request and response.
				netRequest->isSecure = _isSecure;
				netRequest->internalRequest = clientPtr[_clientIndex].get();
				netResponse->isSecure = _isSecure;
				netResponse->internalResponse = clientPtr[_clientIndex].get();
			}
		}
		else
		{
			// If not created.
			if (_clientIndex < 0)
			{
				++clientCount;
				_clientIndex = clientCount;

				// HTTP client
				clientSecurePtr.insert(std::make_pair(_clientIndex, std::make_shared<InternalSecureHttpClient>(_host + ":" + std::to_string(_port), _ipv)));

				// Assign the current client to the request and response.
				netRequest->isSecure = _isSecure;
				netRequest->internalRequest = clientSecurePtr[_clientIndex].get();
				netResponse->isSecure = _isSecure;
				netResponse->internalResponse = clientSecurePtr[_clientIndex].get();
			}
		}

		// Active context.
		_active = true;
	}
}

/// <summary>
/// Make a request.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <return>The net response.</return>
NetResponse& WebClient::Request(const std::string& method, const std::string& path)
{
	// Get the request and response.
	NetRequest* netRequest = &_context->Request();
	NetResponse* netResponse = &_context->Response();

	// Assign the headers.
	netRequest->SetMethod(method);
	netRequest->SetPath(path);
	netRequest->SetContentLength(0);

	// Write the request.
	netRequest->WriteRequest();

	// Get the response from the server.
	netResponse->ReadResponse();
	return *netResponse;
}

/// <summary>
/// Make a request.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="content">The request content.</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <return>The net response.</return>
NetResponse& WebClient::Request(const std::string& method, const std::string& content, const std::string& path)
{
	// Get the request and response.
	NetRequest* netRequest = &_context->Request();
	NetResponse* netResponse = &_context->Response();

	// Assign the headers.
	netRequest->SetMethod(method);
	netRequest->SetPath(path);
	netRequest->SetContentLength(content.length());

	// Write the request.
	netRequest->WriteRequest(content);

	// Get the response from the server.
	netResponse->ReadResponse();
	return *netResponse;
}

/// <summary>
/// Make a request.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="content">The request content.</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <return>The net response.</return>
NetResponse& WebClient::Request(const std::string& method, std::iostream& content, const std::string& path)
{
	// Get the request and response.
	NetRequest* netRequest = &_context->Request();
	NetResponse* netResponse = &_context->Response();

	// Assign the headers.
	netRequest->SetMethod(method);
	netRequest->SetPath(path);

	// Go to the start of the stream.
	content.seekp(0, std::ios::end);

	// Get the length of the stream.
	auto content_length = content.tellp();
	netRequest->SetContentLength(content_length);

	// Write the request.
	netRequest->WriteRequest(content);

	// Get the response from the server.
	netResponse->ReadResponse();
	return *netResponse;
}

/// <summary>
/// Make a request.
/// </summary>
/// <param name="request">The request.</param>
/// <return>The net response.</return>
NetResponse& WebClient::Request(const NetRequest& request)
{
	// Get the request and response.
	NetRequest* netRequest = &_context->Request();
	NetResponse* netResponse = &_context->Response();

	// Assign the headers.
	netRequest->SetMethod(request.GetMethod());
	netRequest->SetPath(request.GetPath());
	netRequest->SetHost(request.GetHost());
	netRequest->SetProtocolVersion(request.GetProtocolVersion());
	netRequest->SetAcceptEncoding(request.GetAcceptEncoding());
	netRequest->SetContentLength(0);
	netRequest->SetContentType(request.GetContentType());

	// Assign the headers.
	std::map<std::string, std::string> headers = const_cast<NetRequest&>(request).GetHeaders();
	typedef std::pair<std::string, std::string> headerPair;

	// For each header.
	for (auto& h : headers)
	{
		// Write header name and value.
		netRequest->AddHeader(h.first.c_str(), h.second.c_str());
	}

	// Write the request.
	netRequest->WriteRequest();

	// Get the response from the server.
	netResponse->ReadResponse();
	return *netResponse;
}

/// <summary>
/// Make a request.
/// </summary>
/// <param name="request">The request.</param>
/// <param name="content">The request content.</param>
/// <return>The net response.</return>
NetResponse& WebClient::Request(const NetRequest& request, const std::string& content)
{
	// Get the request and response.
	NetRequest* netRequest = &_context->Request();
	NetResponse* netResponse = &_context->Response();

	// Assign the headers.
	netRequest->SetMethod(request.GetMethod());
	netRequest->SetPath(request.GetPath());
	netRequest->SetHost(request.GetHost());
	netRequest->SetProtocolVersion(request.GetProtocolVersion());
	netRequest->SetAcceptEncoding(request.GetAcceptEncoding());
	netRequest->SetContentLength(content.length());
	netRequest->SetContentType(request.GetContentType());

	// Assign the headers.
	std::map<std::string, std::string> headers = const_cast<NetRequest&>(request).GetHeaders();
	typedef std::pair<std::string, std::string> headerPair;

	// For each header.
	for (auto& h : headers)
	{
		// Write header name and value.
		netRequest->AddHeader(h.first.c_str(), h.second.c_str());
	}

	// Write the request.
	netRequest->WriteRequest(content);

	// Get the response from the server.
	netResponse->ReadResponse();
	return *netResponse;
}

/// <summary>
/// Make a request.
/// </summary>
/// <param name="request">The request.</param>
/// <param name="content">The request content.</param>
/// <return>The net response.</return>
NetResponse& WebClient::Request(const NetRequest& request, std::iostream& content)
{
	// Get the request and response.
	NetRequest* netRequest = &_context->Request();
	NetResponse* netResponse = &_context->Response();

	// Assign the headers.
	netRequest->SetMethod(request.GetMethod());
	netRequest->SetPath(request.GetPath());
	netRequest->SetHost(request.GetHost());
	netRequest->SetProtocolVersion(request.GetProtocolVersion());
	netRequest->SetAcceptEncoding(request.GetAcceptEncoding());
	netRequest->SetContentType(request.GetContentType());

	// Go to the start of the stream.
	content.seekp(0, std::ios::end);

	// Get the length of the stream.
	auto content_length = content.tellp();
	netRequest->SetContentLength(content_length);

	// Assign the headers.
	std::map<std::string, std::string> headers = const_cast<NetRequest&>(request).GetHeaders();
	typedef std::pair<std::string, std::string> headerPair;

	// For each header.
	for (auto& h : headers)
	{
		// Write header name and value.
		netRequest->AddHeader(h.first.c_str(), h.second.c_str());
	}

	// Write the request.
	netRequest->WriteRequest(content);

	// Get the response from the server.
	netResponse->ReadResponse();
	return *netResponse;
}

/// <summary>
/// Make a request async.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <param name="handler">The async function handler.</param>
/// <param name="context">The client specific content.</param>
void WebClient::RequestAsync(const std::string& method, const std::string& path,
	const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	// Submit to the executor as new thread.
	_executor->Submit([this, method, path, handler, context] { this->RequestAsyncInternal(method, path, handler, context); });
}

/// <summary>
/// Make a request async.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <param name="handler">The async function handler.</param>
/// <param name="context">The client specific content.</param>
void WebClient::RequestAsyncInternal(const std::string& method, const std::string& path,
	const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	// Call the function handler.
	handler(this, Request(method, path), context);
}

/// <summary>
/// Make a request async.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="content">The request content.</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <param name="handler">The async function handler.</param>
/// <param name="context">The client specific content.</param>
void WebClient::RequestAsync(const std::string& method, const std::string& content, const std::string& path,
	const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	// Submit to the executor as new thread.
	_executor->Submit([this, method, content, path, handler, context] { this->RequestAsyncInternal(method, content, path, handler, context); });
}

/// <summary>
/// Make a request async.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="content">The request content.</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <param name="handler">The async function handler.</param>
/// <param name="context">The client specific content.</param>
void WebClient::RequestAsyncInternal(const std::string& method, const std::string& content, const std::string& path,
	const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	// Call the function handler.
	handler(this, Request(method, content, path), context);
}

/// <summary>
/// Make a request async.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="content">The request content.</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <param name="handler">The async function handler.</param>
/// <param name="context">The client specific content.</param>
void WebClient::RequestAsync(const std::string& method, std::iostream& content, const std::string& path,
	const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	// Submit to the executor as new thread.
	_executor->Submit([this, method, &content, path, handler, context] { this->RequestAsyncInternal(method, content, path, handler, context); });
}

/// <summary>
/// Make a request async.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="content">The request content.</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <param name="handler">The async function handler.</param>
/// <param name="context">The client specific content.</param>
void WebClient::RequestAsyncInternal(const std::string& method, std::iostream& content, const std::string& path,
	const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	// Call the function handler.
	handler(this, Request(method, content, path), context);
}

/// <summary>
/// Make a request async.
/// </summary>
/// <param name="request">The request.</param>
/// <param name="handler">The async function handler.</param>
/// <param name="context">The client specific content.</param>
void WebClient::RequestAsync(const NetRequest& request,
	const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	// Submit to the executor as new thread.
	_executor->Submit([this, request, handler, context] { this->RequestAsyncInternal(request, handler, context); });
}

/// <summary>
/// Make a request async.
/// </summary>
/// <param name="request">The request.</param>
/// <param name="handler">The async function handler.</param>
/// <param name="context">The client specific content.</param>
void WebClient::RequestAsyncInternal(const NetRequest& request,
	const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	// Call the function handler.
	handler(this, Request(request), context);
}

/// <summary>
/// Make a request async.
/// </summary>
/// <param name="request">The request.</param>
/// <param name="content">The request content.</param>
/// <param name="handler">The async function handler.</param>
/// <param name="context">The client specific content.</param>
void WebClient::RequestAsync(const NetRequest& request, const std::string& content,
	const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	// Submit to the executor as new thread.
	_executor->Submit([this, request, content, handler, context] { this->RequestAsyncInternal(request, content, handler, context); });
}

/// <summary>
/// Make a request async.
/// </summary>
/// <param name="request">The request.</param>
/// <param name="content">The request content.</param>
/// <param name="handler">The async function handler.</param>
/// <param name="context">The client specific content.</param>
void WebClient::RequestAsyncInternal(const NetRequest& request, const std::string& content,
	const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	// Call the function handler.
	handler(this, Request(request, content), context);
}

/// <summary>
/// Make a request async.
/// </summary>
/// <param name="request">The request.</param>
/// <param name="content">The request content.</param>
/// <param name="handler">The async function handler.</param>
/// <param name="context">The client specific content.</param>
void WebClient::RequestAsync(const NetRequest& request, std::iostream& content,
	const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	// Submit to the executor as new thread.
	_executor->Submit([this, request, &content, handler, context] { this->RequestAsyncInternal(request, content, handler, context); });
}

/// <summary>
/// Make a request async.
/// </summary>
/// <param name="request">The request.</param>
/// <param name="content">The request content.</param>
/// <param name="handler">The async function handler.</param>
/// <param name="context">The client specific content.</param>
void WebClient::RequestAsyncInternal(const NetRequest& request, std::iostream& content,
	const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	// Call the function handler.
	handler(this, Request(request, content), context);
}

/// <summary>
/// Make a request callable.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <return>The callable future.</return>
NetResponseCallable WebClient::RequestCallable(const std::string& method, const std::string& path)
{
	return std::async(std::launch::async, [this, method, path]() { return this->Request(method, path); });
}

/// <summary>
/// Make a request callable.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="content">The request content.</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <return>The callable future.</return>
NetResponseCallable WebClient::RequestCallable(const std::string& method, const std::string& content, const std::string& path)
{
	return std::async(std::launch::async, [this, method, content, path]() { return this->Request(method, content, path); });
}

/// <summary>
/// Make a request callable.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="content">The request content.</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <return>The callable future.</return>
NetResponseCallable WebClient::RequestCallable(const std::string& method, std::iostream& content, const std::string& path)
{
	return std::async(std::launch::async, [this, method, &content, path]() { return this->Request(method, content, path); });
}

/// <summary>
/// Make a request callable.
/// </summary>
/// <param name="request">The request.</param>
/// <return>The callable future.</return>
NetResponseCallable WebClient::RequestCallable(const NetRequest& request)
{
	return std::async(std::launch::async, [this, request]() { return this->Request(request); });
}

/// <summary>
/// Make a request callable.
/// </summary>
/// <param name="request">The request.</param>
/// <param name="content">The request content.</param>
/// <return>The callable future.</return>
NetResponseCallable WebClient::RequestCallable(const NetRequest& request, const std::string& content)
{
	return std::async(std::launch::async, [this, request, content]() { return this->Request(request, content); });
}

/// <summary>
/// Make a request callable.
/// </summary>
/// <param name="request">The request.</param>
/// <param name="content">The request content.</param>
/// <return>The callable future.</return>
NetResponseCallable WebClient::RequestCallable(const NetRequest& request, std::iostream& content)
{
	return std::async(std::launch::async, [this, request, &content]() { return this->Request(request, content); });
}