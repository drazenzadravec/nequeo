/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          HttpClient.cpp
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

#pragma once

#include "stdafx.h"

#include "HttpClient.h"

using namespace Nequeo::Net::Http;

/// <summary>
/// Http web client.
/// </summary>
/// <param name="url">The URL.</param>
/// <param name="ipv">The IP version to use.</param>
HttpClient::HttpClient(const std::string& url, IPVersionType ipv) :
	_disposed(false)
{
	_client = std::make_shared<WebClient>(url, ipv);
}

///	<summary>
///	Http web client.
///	</summary>
HttpClient::HttpClient(const std::string& host, unsigned short port, bool isSecure, IPVersionType ipv) :
	_disposed(false)
{
	_client = std::make_shared<WebClient>(host, port, isSecure, ipv);
}

///	<summary>
///	Http web client.
///	</summary>
HttpClient::~HttpClient()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Make a connection.
/// </summary>
void HttpClient::Connect()
{
	_client->Connect();
}

/// <summary>
/// Make a request.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <return>The net response.</return>
NetResponse& HttpClient::Request(const std::string& method, const std::string& path)
{
	return _client->Request(method, path);
}

/// <summary>
/// Make a request.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="content">The request content.</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <return>The net response.</return>
NetResponse& HttpClient::Request(const std::string& method, const std::string& content, const std::string& path)
{
	return _client->Request(method, content, path);
}

/// <summary>
/// Make a request.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="content">The request content.</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <return>The net response.</return>
NetResponse& HttpClient::Request(const std::string& method, std::iostream& content, const std::string& path)
{
	return _client->Request(method, content, path);
}

/// <summary>
/// Make a request.
/// </summary>
/// <param name="request">The request.</param>
/// <return>The net response.</return>
NetResponse& HttpClient::Request(const NetRequest& request)
{
	return _client->Request(request);
}

/// <summary>
/// Make a request.
/// </summary>
/// <param name="request">The request.</param>
/// <param name="content">The request content.</param>
/// <return>The net response.</return>
NetResponse& HttpClient::Request(const NetRequest& request, const std::string& content)
{
	return _client->Request(request, content);
}

/// <summary>
/// Make a request.
/// </summary>
/// <param name="request">The request.</param>
/// <param name="content">The request content.</param>
/// <return>The net response.</return>
NetResponse& HttpClient::Request(const NetRequest& request, std::iostream& content)
{
	return _client->Request(request, content);
}

/// <summary>
/// Make a request async.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <param name="handler">The async function handler.</param>
/// <param name="context">The client specific content.</param>
void HttpClient::RequestAsync(const std::string& method, const std::string& path,
	const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	_client->RequestAsync(method, path, handler, context);
}

/// <summary>
/// Make a request async.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="content">The request content.</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <param name="handler">The async function handler.</param>
/// <param name="context">The client specific content.</param>
void HttpClient::RequestAsync(const std::string& method, const std::string& content, const std::string& path,
	const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	_client->RequestAsync(method, content, path, handler, context);
}

/// <summary>
/// Make a request async.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="content">The request content.</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <param name="handler">The async function handler.</param>
/// <param name="context">The client specific content.</param>
void HttpClient::RequestAsync(const std::string& method, std::iostream& content, const std::string& path,
	const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	_client->RequestAsync(method, content, path, handler, context);
}

/// <summary>
/// Make a request async.
/// </summary>
/// <param name="request">The request.</param>
/// <param name="handler">The async function handler.</param>
/// <param name="context">The client specific content.</param>
void HttpClient::RequestAsync(const NetRequest& request,
	const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	_client->RequestAsync(request, handler, context);
}

/// <summary>
/// Make a request async.
/// </summary>
/// <param name="request">The request.</param>
/// <param name="content">The request content.</param>
/// <param name="handler">The async function handler.</param>
/// <param name="context">The client specific content.</param>
void HttpClient::RequestAsync(const NetRequest& request, const std::string& content,
	const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	_client->RequestAsync(request, content, handler, context);
}

/// <summary>
/// Make a request async.
/// </summary>
/// <param name="request">The request.</param>
/// <param name="content">The request content.</param>
/// <param name="handler">The async function handler.</param>
/// <param name="context">The client specific content.</param>
void HttpClient::RequestAsync(const NetRequest& request, std::iostream& content,
	const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	_client->RequestAsync(request, content, handler, context);
}

/// <summary>
/// Make a request callable.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <return>The callable future.</return>
NetResponseCallable HttpClient::RequestCallable(const std::string& method, const std::string& path)
{
	return _client->RequestCallable(method, path);
}

/// <summary>
/// Make a request callable.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="content">The request content.</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <return>The callable future.</return>
NetResponseCallable HttpClient::RequestCallable(const std::string& method, const std::string& content, const std::string& path)
{
	return _client->RequestCallable(method, content, path);
}

/// <summary>
/// Make a request callable.
/// </summary>
/// <param name="method">The request method (e.g GET, POST).</param>
/// <param name="content">The request content.</param>
/// <param name="path">The resource path (e.g. '/').</param>
/// <return>The callable future.</return>
NetResponseCallable HttpClient::RequestCallable(const std::string& method, std::iostream& content, const std::string& path)
{
	return _client->RequestCallable(method, content, path);
}

/// <summary>
/// Make a request callable.
/// </summary>
/// <param name="request">The request.</param>
/// <return>The callable future.</return>
NetResponseCallable HttpClient::RequestCallable(const NetRequest& request)
{
	return _client->RequestCallable(request);
}

/// <summary>
/// Make a request callable.
/// </summary>
/// <param name="request">The request.</param>
/// <param name="content">The request content.</param>
/// <return>The callable future.</return>
NetResponseCallable HttpClient::RequestCallable(const NetRequest& request, const std::string& content)
{
	return _client->RequestCallable(request, content);
}

/// <summary>
/// Make a request callable.
/// </summary>
/// <param name="request">The request.</param>
/// <param name="content">The request content.</param>
/// <return>The callable future.</return>
NetResponseCallable HttpClient::RequestCallable(const NetRequest& request, std::iostream& content)
{
	return _client->RequestCallable(request, content);
}

/// <summary>
/// Get the host of the URL.
/// </summary>
/// <return>The URL host.</return>
std::string HttpClient::GetURLHost()
{
	return _client->GetURLHost();
}

/// <summary>
/// Get the path of the URL.
/// </summary>
/// <return>The URL path.</return>
std::string HttpClient::GetURLPath()
{
	return _client->GetURLPath();
}

/// <summary>
/// Get the query of the URL.
/// </summary>
/// <return>The URL query.</return>
std::string HttpClient::GetURLQuery()
{
	return _client->GetURLQuery();
}

/// <summary>
/// Get the port of the URL.
/// </summary>
/// <return>The URL port.</return>
unsigned short HttpClient::GetURLPort()
{
	return _client->GetURLPort();
}

/// <summary>
/// Get the is secure of the URL.
/// </summary>
/// <return>The URL is secure.</return>
bool HttpClient::GetURLIsSecure()
{
	return _client->GetURLIsSecure();
}

/// <summary>
/// Get the ip type of the URL.
/// </summary>
/// <param name="ipAddress">The IP address.</param>
/// <return>The URL ip type.</return>
IPVersionType HttpClient::GetIPVersionType(const std::string& ipAddress)
{
	return _client->GetIPVersionType(ipAddress);
}
void HttpClient::SetIPVersionType(IPVersionType ipVersionType)
{
	_client->SetIPVersionType(ipVersionType);
}

/// <summary>
/// Get the list of resolved IP address of the host of the URL.
/// </summary>
/// <return>The URL ip address.</return>
std::vector<std::string> HttpClient::GetResolvedHosts()
{
	return _client->GetResolvedHosts();
}