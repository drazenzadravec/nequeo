/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WebContext.cpp
*  Purpose :       Http web context class.
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
#include "server_http.hpp"
#include "server_https.hpp"

using namespace Nequeo::Net::Http;

///	<summary>
///	Http web context.
///	</summary>
/// <param name="request">The web request.</param>
/// <param name="response">The web response.</param>
WebContext::WebContext(std::shared_ptr<WebRequest>& request, std::shared_ptr<WebResponse>& response) :
	_disposed(false), _isSecure(false), _port(80), _ipv(IPVersionType::IPv4), _request(request), _response(response)
{
}

///	<summary>
///	Http web context.
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
WebRequest& WebContext::Request()
{
	return *(_request.get());
}

/// <summary>
/// Get the web response.
/// </summary>
/// <return>The web response.</return>
WebResponse& WebContext::Response()
{
	return *(_response.get());
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
std::string WebContext::GetServerName() const
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