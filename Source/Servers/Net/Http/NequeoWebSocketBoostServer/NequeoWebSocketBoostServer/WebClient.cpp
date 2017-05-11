/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WebClient.cpp
*  Purpose :       WebSocket web client class.
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
#include "client_ws.hpp"
#include "client_wss.hpp"

using namespace Nequeo::Net::WebSocket;

static const char* WEBCLIENT_WS_CLIENT_TAG = "NequeoWsClient";
static const char* NETCONTEXT_WS_CLIENT_TAG = "NequeoWsNetContext";

std::atomic<int> clientWsCount;
concurrency::concurrent_unordered_map<int, std::shared_ptr<InternalWebSocketClient>> clientPtr;
concurrency::concurrent_unordered_map<int, std::shared_ptr<InternalSecureWebSocketClient>> clientSecurePtr;

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
	_executor = Nequeo::MakeShared<Nequeo::Threading::DefaultExecutor>(WEBCLIENT_WS_CLIENT_TAG);
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