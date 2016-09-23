/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          HttpClient.h
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
#include "GlobalNetServer.h"

#include "NequeoHttpBoostServer\WebClient.h"

namespace Nequeo {
	namespace Net {
		namespace Http
		{
			/// <summary>
			/// Http web client.
			/// </summary>
			class EXPORT_NEQUEO_NET_SERVER_API HttpClient
			{
			public:
				/// <summary>
				/// Http web client.
				/// </summary>
				HttpClient(const std::string& host, unsigned short port = 80, bool isSecure = false, IPVersionType ipv = IPVersionType::IPv4);

				/// <summary>
				/// Http web client.
				/// </summary>
				~HttpClient();

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

			private:
				bool _disposed;
				std::shared_ptr<WebClient> _client;
				
			};
		}
	}
}