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
#include "WebContextExtender.cpp"

using namespace Nequeo::Net::WebSocket;

///	<summary>
///	WebSocket web context.
///	</summary>
/// <param name="request">The web request.</param>
/// <param name="message">The web message.</param>
WebContext::WebContext(std::shared_ptr<WebRequest>& request, std::shared_ptr<WebMessage>& message) :
	_disposed(false), _isSecure(false), _port(80), _ipv(IPVersionType::IPv4), _request(request), _message(message),
	_uniqueID(""), _applicationID(""), _available(false), _broadcast(false), _broadcastAppID(false), 
	_hasAccess(false), _accessExpiry(0), _timeoutConnectCancelled(false), _clientToken("")
{
	_extender = std::make_shared<WebContextExtender>();
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
std::shared_ptr<WebRequest> WebContext::Request() const
{
	return _request;
}

/// <summary>
/// Get the web message.
/// </summary>
/// <return>The web message.</return>
std::shared_ptr<WebMessage> WebContext::Message() const
{
	return _message;
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

/// <summary>
/// Get the context unique id.
/// </summary>
/// <return>The unique id.</return>
const std::string& WebContext::GetUniqueID() const
{
	return _uniqueID;
}

/// <summary>
/// Get the context application id.
/// </summary>
/// <return>The application id.</return>
const std::string& WebContext::GetApplicationID() const
{
	return _applicationID;
}

/// <summary>
/// Is the context available.
/// </summary>
/// <return>True if the context is available; else false.</return>
bool WebContext::Available() const
{
	return _available;
}

/// <summary>
/// Should the context be broadcast.
/// </summary>
/// <return>True if the contaxt should be broadcast; else false.</return>
bool WebContext::Broadcast() const
{
	return _broadcast;
}

/// <summary>
/// Should the context broadcast application id.
/// </summary>
/// <return>True if the contaxt should broadcast application id; else false.</return>
bool WebContext::BroadcastAppID() const
{
	return _broadcastAppID;
}

/// <summary>
/// Has the client got access.
/// </summary>
/// <return>True if the contaxt has access; else false.</return>
bool WebContext::HasAccess() const
{
	return _hasAccess;
}

/// <summary>
/// Get the access exipry timeout.
/// </summary>
/// <return>The access expiry timeout.</return>
unsigned int WebContext::AccessExpiry() const
{
	return _accessExpiry;
}

/// <summary>
/// Has the time out connect been cancelled.
/// </summary>
/// <return>True if the time out connect has been cancelled; else false.</return>
bool WebContext::TimeoutConnectCancelled() const
{
	return _timeoutConnectCancelled;
}

/// <summary>
/// Get the context client token.
/// </summary>
/// <return>The client token.</return>
const std::string& WebContext::GetClientToken() const
{
	return _clientToken;
}

/// <summary>
/// Start the access expiry timeout.
/// </summary>
/// <param name="accessExpiry">The access expiry timeout.</param>
/// <param name="callback">The access expiry timeout callback.</param>
void WebContext::StartAccessExpiry(unsigned int accessExpiry)
{
	_accessExpiry = accessExpiry;
}

///	<summary>
///	Cancel the time out connect timer.
///	</summary>
/// <param name="cancel">True to cancel, once this is set to true, it stays true.</param>
void WebContext::CancelTimeoutConnect(bool cancel)
{
	_timeoutConnectCancelled = cancel;
}