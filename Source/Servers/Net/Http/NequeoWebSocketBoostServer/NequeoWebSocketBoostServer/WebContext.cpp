/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WebContext.cpp
*  Purpose :       WebSocket web context class.
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

#include "WebContext.h"
#include "server_ws.hpp"
#include "server_wss.hpp"

using namespace Nequeo::Net::WebSocket;

///	<summary>
///	WebSocket web context.
///	</summary>
/// <param name="request">The web request.</param>
WebContext::WebContext(std::shared_ptr<WebRequest>& request) :
	_disposed(false), _isSecure(false), _port(80), _ipv(IPVersionType::IPv4), _request(request)
{
}

///	<summary>
///	WebSocket web context.
///	</summary>
WebContext::~WebContext()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Get the web request.
/// </summary>
/// <return>The web request.</return>
WebRequest& WebContext::Request() const
{
	return *(_request.get());
}

/// <summary>
/// Is the server secure.
/// </summary>
/// <return>True if the server is secure; else false.</return>
bool WebContext::IsSecure() const
{
	return _isSecure;
}

/// <summary>
/// The IP version type.
/// </summary>
/// <return>The IP version type.</return>
IPVersionType WebContext::IPVersion() const
{
	return _ipv;
}

/// <summary>
/// Get the server name.
/// </summary>
/// <return>The servername.</return>
const std::string& WebContext::GetServerName() const
{
	return _serverName;
}

/// <summary>
/// Get the port number.
/// </summary>
/// <return>The port number.</return>
unsigned short WebContext::GetPort() const
{
	return _port;
}