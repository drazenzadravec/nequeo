/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          NetContext.cpp
*  Purpose :       WebSocket net context class.
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

#include "NetContext.h"

using namespace Nequeo::Net::WebSocket;

static const char* NETREQUEST_WS_CLIENT_TAG = "NequeoWsRequest";
static const char* NETRESPONSE_WS_CLIENT_TAG = "NequeoWsResponse";

///	<summary>
///	Http net context.
///	</summary>
NetContext::NetContext() :
	_disposed(false), _isSecure(false), _port(80), _ipv(IPVersionType::IPv4)
{
	_response = Nequeo::MakeShared<NetResponse>(NETRESPONSE_WS_CLIENT_TAG);
}

///	<summary>
///	Http net context.
///	</summary>
NetContext::~NetContext()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Get the net response.
/// </summary>
/// <return>The net response.</return>
NetResponse& NetContext::Response() const
{
	return *(_response.get());
}

/// <summary>
/// Is the server secure.
/// </summary>
/// <return>True if the server is secure; else false.</return>
bool NetContext::IsSecure() const
{
	return _isSecure;
}

/// <summary>
/// The IP version type.
/// </summary>
/// <return>The IP version type.</return>
IPVersionType NetContext::IPVersion() const
{
	return _ipv;
}

/// <summary>
/// Get the port number.
/// </summary>
/// <return>The port number.</return>
unsigned short NetContext::GetPort() const
{
	return _port;
}