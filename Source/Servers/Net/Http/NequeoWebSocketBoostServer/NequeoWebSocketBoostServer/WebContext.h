/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WebContext.h
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

#pragma once

#include "stdafx.h"
#include "Global.h"

#include "IPVersionType.h"
#include "MessageType.h"
#include "WebRequest.h"
#include "Message.h"

namespace Nequeo {
	namespace Net {
		namespace WebSocket
		{
			class WebContextExtender;

			/// <summary>
			/// WebSocket web context.
			/// </summary>
			class EXPORT_NEQUEO_WEBSOCKET_BOOST_SERVER_API WebContext
			{
			public:
				/// <summary>
				/// WebSocket web context.
				/// </summary>
				/// <param name="request">The web request.</param>
				/// <param name="message">The web message.</param>
				WebContext(std::shared_ptr<WebRequest>& request, std::shared_ptr<WebMessage>& message);

				/// <summary>
				/// WebSocket web context.
				/// </summary>
				~WebContext();

				/// <summary>
				/// Get the web request.
				/// </summary>
				/// <return>The web request.</return>
				std::shared_ptr<WebRequest> Request() const;

				/// <summary>
				/// Get the web message.
				/// </summary>
				/// <return>The web message.</return>
				std::shared_ptr<WebMessage> Message() const;

				/// <summary>
				/// Is the server secure.
				/// </summary>
				/// <return>True if the server is secure; else false.</return>
				bool IsSecure() const;

				/// <summary>
				/// The IP version type.
				/// </summary>
				/// <return>The IP version type.</return>
				IPVersionType IPVersion() const;

				/// <summary>
				/// Get the server name.
				/// </summary>
				/// <return>The servername.</return>
				const std::string& GetServerName() const;

				/// <summary>
				/// Get the port number.
				/// </summary>
				/// <return>The port number.</return>
				unsigned short GetPort() const;

				/// <summary>
				/// Get the context unique id.
				/// </summary>
				/// <return>The unique id.</return>
				const std::string& GetUniqueID() const;

				/// <summary>
				/// Get the context application id.
				/// </summary>
				/// <return>The application id.</return>
				const std::string& GetApplicationID() const;

				/// <summary>
				/// Get the context client token.
				/// </summary>
				/// <return>The client token.</return>
				const std::string& GetClientToken() const;

				/// <summary>
				/// Is the context available.
				/// </summary>
				/// <return>True if the context is available; else false.</return>
				bool Available() const;

				/// <summary>
				/// Should the context be broadcast.
				/// </summary>
				/// <return>True if the contaxt should be broadcast; else false.</return>
				bool Broadcast() const;

				/// <summary>
				/// Should the context broadcast application id.
				/// </summary>
				/// <return>True if the contaxt should broadcast application id; else false.</return>
				bool BroadcastAppID() const;

				/// <summary>
				/// Has the client got access.
				/// </summary>
				/// <return>True if the contex has access; else false.</return>
				bool HasAccess() const;

				/// <summary>
				/// Get the access exipry timeout.
				/// </summary>
				/// <return>The access expiry timeout.</return>
				unsigned int AccessExpiry() const;

				/// <summary>
				/// Has the time out connect been cancelled.
				/// </summary>
				/// <return>True if the time out connect has been cancelled; else false.</return>
				bool TimeoutConnectCancelled() const;

				/// <summary>
				/// Set is server secure.
				/// </summary>
				/// <param name="isSecure">Is secure.</param>
				inline void SetIsSecure(bool isSecure)
				{
					_isSecure = isSecure;
				}
				/// <summary>
				/// Set ip version.
				/// </summary>
				/// <param name="ipv">IP version.</param>
				inline void SetIPVersionType(IPVersionType ipv)
				{
					_ipv = ipv;
				}

				/// <summary>
				/// Set server name.
				/// </summary>
				/// <param name="serverName">Server name.</param>
				inline void SetServerName(const std::string& serverName)
				{
					_serverName = serverName;
				}

				/// <summary>
				/// Set port.
				/// </summary>
				/// <param name="port">Port.</param>
				inline void SetPort(unsigned short port)
				{
					_port = port;
				}

				/// <summary>
				/// Set context unique id.
				/// </summary>
				/// <param name="uniqueID">Unique id.</param>
				inline void SetUniqueID(const std::string& uniqueID)
				{
					_uniqueID = uniqueID;
				}

				/// <summary>
				/// Set context application id.
				/// </summary>
				/// <param name="applicationID">Application id.</param>
				inline void SetApplicationID(const std::string& applicationID)
				{
					_applicationID = applicationID;
				}

				/// <summary>
				/// Set context client token.
				/// </summary>
				/// <param name="clientToken">Client token.</param>
				inline void SetClientToken(const std::string& clientToken)
				{
					_clientToken = clientToken;
				}

				/// <summary>
				/// Set available.
				/// </summary>
				/// <param name="available">Available.</param>
				inline void SetAvailable(bool available)
				{
					_available = available;
				}

				/// <summary>
				/// Set broadcast.
				/// </summary>
				/// <param name="broadcast">Broadcast.</param>
				inline void SetBroadcast(bool broadcast)
				{
					_broadcast = broadcast;
				}

				/// <summary>
				/// Set broadcast application id.
				/// </summary>
				/// <param name="broadcastAppID">Broadcast application id.</param>
				inline void SetBroadcastAppID(bool broadcastAppID)
				{
					_broadcastAppID = broadcastAppID;
				}

				/// <summary>
				/// Set the has access indicator.
				/// </summary>
				/// <param name="hasAccess">Has the client access.</param>
				inline void SetHasAccess(bool hasAccess)
				{
					_hasAccess = hasAccess;
				}

				/// <summary>
				/// Start the access expiry timeout.
				/// </summary>
				/// <param name="accessExpiry">The access expiry timeout.</param>
				void StartAccessExpiry(unsigned int accessExpiry);

				///	<summary>
				///	Cancel the time out connect timer.
				///	</summary>
				/// <param name="cancel">True to cancel, once this is set to true, it stays true.</param>
				void CancelTimeoutConnect(bool cancel);

			public:
				/// <summary>
				/// On message received function handler.
				/// </summary>
				/// <param name="messageType">The message type.</param>
				/// <param name="length">The length of the message.</param>
				/// <param name="messsage">The message.</param>
				std::function<void(MessageType, size_t, std::shared_ptr<WebMessage>&)> OnMessage;

				/// <summary>
				/// On connection error function handler.
				/// </summary>
				/// <param name="error">The error message.</param>
				std::function<void(const std::string&)> OnError;

				/// <summary>
				/// On connection closed function handler.
				/// </summary>
				/// <param name="status">The current status.</param>
				/// <param name="reason">The close reason.</param>
				std::function<void(int, const std::string&)> OnClose;

				/// <summary>
				/// On connection access expiry function handler.
				/// </summary>
				/// <param name="messsage">The message.</param>
				std::function<void(std::shared_ptr<WebMessage>&)> OnAccessExpiry;

			private:
				bool _disposed;
				bool _isSecure;
				bool _available;
				bool _broadcast;
				bool _broadcastAppID;

				unsigned short _port;
				IPVersionType _ipv;
				std::string _serverName;
				std::string _uniqueID;
				std::string _applicationID;
				std::string _clientToken;
				bool _hasAccess;
				bool _timeoutConnectCancelled;
				unsigned int _accessExpiry;

				std::shared_ptr<WebRequest> _request;
				std::shared_ptr<WebMessage> _message;

				std::shared_ptr<WebContextExtender> _extender;
			};
		}
	}
}