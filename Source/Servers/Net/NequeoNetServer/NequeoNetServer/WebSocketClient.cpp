/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WebSocketClient.cpp
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

#pragma once

#include "stdafx.h"

#include "WebSocketClient.h"

using namespace Nequeo::Net::WebSocket;

///	<summary>
///	WebSocket web client.
///	</summary>
WebSocketClient::WebSocketClient(const std::string& host, unsigned short port, bool isSecure, IPVersionType ipv) :
	_disposed(false)
{
	_client = std::make_shared<WebClient>(host, port, isSecure, ipv);
}

///	<summary>
///	WebSocket web client.
///	</summary>
WebSocketClient::~WebSocketClient()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	Start a connection.
///	</summary>
void WebSocketClient::Connect()
{
	_client->Connect();
}

///	<summary>
///	Disconnect.
///	</summary>
void WebSocketClient::Disconnect()
{
	_client->Disconnect();
}

/// <summary>
/// Make a request async.
/// </summary>
/// <param name="handler">The async function handler.</param>
/// <param name="context">The client specific content.</param>
void WebSocketClient::RequestAsync(const ResponseHandler& handler, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	_client->RequestAsync(handler, context);
}