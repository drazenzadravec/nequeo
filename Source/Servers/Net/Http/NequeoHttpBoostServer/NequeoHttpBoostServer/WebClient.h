/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WebClient.h
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
#include "Global.h"

#include "NetContext.h"
#include "IPVersionType.h"

#include "Threading\Executor.h"
#include "Threading\ThreadTask.h"
#include "Base\AsyncCallerContext.h"

namespace Nequeo {
	namespace Net {
		namespace Http
		{
			class WebClient;

			typedef std::future<NetResponse> NetResponseCallable;
			typedef std::function<void(const WebClient*, const NetResponse&, const std::shared_ptr<const Nequeo::AsyncCallerContext>&)> ResponseHandler;

			/// <summary>
			/// Http web client.
			/// </summary>
			class EXPORT_NEQUEO_NET_BOOST_SERVER_API WebClient
			{
			public:
				/// <summary>
				/// Http web client.
				/// </summary>
				/// <param name="url">The URL.</param>
				WebClient(const std::string& url);

				/// <summary>
				/// Http web client.
				/// </summary>
				/// <param name="url">The URL.</param>
				/// <param name="ipv">The IP version to use.</param>
				WebClient(const std::string& url, IPVersionType ipv);

				/// <summary>
				/// Http web client.
				/// </summary>
				/// <param name="host">The host (name or IP).</param>
				/// <param name="port">The host port number.</param>
				/// <param name="isSecure">Is the connection secure.</param>
				/// <param name="ipv">The IP version to use.</param>
				WebClient(const std::string& host, unsigned short port, bool isSecure = false, IPVersionType ipv = IPVersionType::IPv4);

				/// <summary>
				/// Http web client.
				/// </summary>
				virtual ~WebClient();

				/// <summary>
				/// Make a connection.
				/// </summary>
				void Connect();

				/// <summary>
				/// Make a request.
				/// </summary>
				/// <param name="method">The request method (e.g GET, POST).</param>
				/// <param name="path">The resource path (e.g. '/').</param>
				/// <return>The net response.</return>
				NetResponse& Request(const std::string& method, const std::string& path = "/");

				/// <summary>
				/// Make a request.
				/// </summary>
				/// <param name="method">The request method (e.g GET, POST).</param>
				/// <param name="content">The request content.</param>
				/// <param name="path">The resource path (e.g. '/').</param>
				/// <return>The net response.</return>
				NetResponse& Request(const std::string& method, const std::string& content, const std::string& path);

				/// <summary>
				/// Make a request.
				/// </summary>
				/// <param name="method">The request method (e.g GET, POST).</param>
				/// <param name="content">The request content.</param>
				/// <param name="path">The resource path (e.g. '/').</param>
				/// <return>The net response.</return>
				NetResponse& Request(const std::string& method, std::iostream& content, const std::string& path);

				/// <summary>
				/// Make a request.
				/// </summary>
				/// <param name="request">The request.</param>
				/// <return>The net response.</return>
				NetResponse& Request(const NetRequest& request);

				/// <summary>
				/// Make a request.
				/// </summary>
				/// <param name="request">The request.</param>
				/// <param name="content">The request content.</param>
				/// <return>The net response.</return>
				NetResponse& Request(const NetRequest& request, const std::string& content);

				/// <summary>
				/// Make a request.
				/// </summary>
				/// <param name="request">The request.</param>
				/// <param name="content">The request content.</param>
				/// <return>The net response.</return>
				NetResponse& Request(const NetRequest& request, std::iostream& content);

				/// <summary>
				/// Make a request async.
				/// </summary>
				/// <param name="method">The request method (e.g GET, POST).</param>
				/// <param name="path">The resource path (e.g. '/').</param>
				/// <param name="handler">The async function handler.</param>
				/// <param name="context">The client specific content.</param>
				void RequestAsync(const std::string& method, const std::string& path,
					const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context = nullptr);

				/// <summary>
				/// Make a request async.
				/// </summary>
				/// <param name="method">The request method (e.g GET, POST).</param>
				/// <param name="content">The request content.</param>
				/// <param name="path">The resource path (e.g. '/').</param>
				/// <param name="handler">The async function handler.</param>
				/// <param name="context">The client specific content.</param>
				void RequestAsync(const std::string& method, const std::string& content, const std::string& path, 
					const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context = nullptr);

				/// <summary>
				/// Make a request async.
				/// </summary>
				/// <param name="method">The request method (e.g GET, POST).</param>
				/// <param name="content">The request content.</param>
				/// <param name="path">The resource path (e.g. '/').</param>
				/// <param name="handler">The async function handler.</param>
				/// <param name="context">The client specific content.</param>
				void RequestAsync(const std::string& method, std::iostream& content, const std::string& path, 
					const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context = nullptr);

				/// <summary>
				/// Make a request async.
				/// </summary>
				/// <param name="request">The request.</param>
				/// <param name="handler">The async function handler.</param>
				/// <param name="context">The client specific content.</param>
				void RequestAsync(const NetRequest& request,
					const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context = nullptr);

				/// <summary>
				/// Make a request async.
				/// </summary>
				/// <param name="request">The request.</param>
				/// <param name="content">The request content.</param>
				/// <param name="handler">The async function handler.</param>
				/// <param name="context">The client specific content.</param>
				void RequestAsync(const NetRequest& request, const std::string& content,
					const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context = nullptr);

				/// <summary>
				/// Make a request async.
				/// </summary>
				/// <param name="request">The request.</param>
				/// <param name="content">The request content.</param>
				/// <param name="handler">The async function handler.</param>
				/// <param name="context">The client specific content.</param>
				void RequestAsync(const NetRequest& request, std::iostream& content,
					const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context = nullptr);

				/// <summary>
				/// Make a request callable.
				/// </summary>
				/// <param name="method">The request method (e.g GET, POST).</param>
				/// <param name="path">The resource path (e.g. '/').</param>
				/// <return>The callable future.</return>
				NetResponseCallable RequestCallable(const std::string& method, const std::string& path = "/");

				/// <summary>
				/// Make a request callable.
				/// </summary>
				/// <param name="method">The request method (e.g GET, POST).</param>
				/// <param name="content">The request content.</param>
				/// <param name="path">The resource path (e.g. '/').</param>
				/// <return>The callable future.</return>
				NetResponseCallable RequestCallable(const std::string& method, const std::string& content, const std::string& path);

				/// <summary>
				/// Make a request callable.
				/// </summary>
				/// <param name="method">The request method (e.g GET, POST).</param>
				/// <param name="content">The request content.</param>
				/// <param name="path">The resource path (e.g. '/').</param>
				/// <return>The callable future.</return>
				NetResponseCallable RequestCallable(const std::string& method, std::iostream& content, const std::string& path);

				/// <summary>
				/// Make a request callable.
				/// </summary>
				/// <param name="request">The request.</param>
				/// <return>The callable future.</return>
				NetResponseCallable RequestCallable(const NetRequest& request);

				/// <summary>
				/// Make a request callable.
				/// </summary>
				/// <param name="request">The request.</param>
				/// <param name="content">The request content.</param>
				/// <return>The callable future.</return>
				NetResponseCallable RequestCallable(const NetRequest& request, const std::string& content);

				/// <summary>
				/// Make a request callable.
				/// </summary>
				/// <param name="request">The request.</param>
				/// <param name="content">The request content.</param>
				/// <return>The callable future.</return>
				NetResponseCallable RequestCallable(const NetRequest& request, std::iostream& content);

				/// <summary>
				/// Get the host of the URL.
				/// </summary>
				/// <return>The URL host.</return>
				std::string GetURLHost();

				/// <summary>
				/// Get the path of the URL.
				/// </summary>
				/// <return>The URL path.</return>
				std::string GetURLPath();

				/// <summary>
				/// Get the query of the URL.
				/// </summary>
				/// <return>The URL query.</return>
				std::string GetURLQuery();

				/// <summary>
				/// Get the port of the URL.
				/// </summary>
				/// <return>The URL port.</return>
				unsigned short GetURLPort();

				/// <summary>
				/// Get the is secure of the URL.
				/// </summary>
				/// <return>The URL is secure.</return>
				bool GetURLIsSecure();

				/// <summary>
				/// Get the ip type of the URL.
				/// </summary>
				/// <param name="ipAddress">The IP address.</param>
				/// <return>The URL ip type.</return>
				IPVersionType GetIPVersionType(const std::string& ipAddress);
				void SetIPVersionType(IPVersionType ipVersionType);

				/// <summary>
				/// Get the list of resolved IP address of the host of the URL.
				/// </summary>
				/// <return>The URL ip address.</return>
				std::vector<std::string> GetResolvedHosts();

			private:
				bool _disposed;
				bool _active;
				bool _isSecure;
				bool _connected;

				int _clientIndex;
				IPVersionType _ipv;
				std::string _host;
				unsigned short _port;
				std::string _url;

				std::shared_ptr<NetContext> _context;
				std::shared_ptr<Nequeo::Threading::Executor> _executor;

				void CreateNetContext();

				void RequestAsyncInternal(const std::string& method, const std::string& path,
					const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context = nullptr);

				void RequestAsyncInternal(const std::string& method, const std::string& content, const std::string& path, 
					const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context = nullptr);

				void RequestAsyncInternal(const std::string& method, std::iostream& content, const std::string& path, 
					const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context = nullptr);

				void RequestAsyncInternal(const NetRequest& request,
					const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context = nullptr);

				void RequestAsyncInternal(const NetRequest& request, const std::string& content,
					const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context = nullptr);

				void RequestAsyncInternal(const NetRequest& request, std::iostream& content,
					const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context = nullptr);

			};
		}
	}
}