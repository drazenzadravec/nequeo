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
				WebContext(std::shared_ptr<WebRequest>& request);

				/// <summary>
				/// WebSocket web context.
				/// </summary>
				~WebContext();

				/// <summary>
				/// Get the web request.
				/// </summary>
				/// <return>The web request.</return>
				WebRequest& Request() const;

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

			public:
				/// <summary>
				/// On message received function handler.
				/// </summary>
				/// <param name="messageType">The message type.</param>
				/// <param name="length">The length of the message.</param>
				/// <param name="messsage">The message.</param>
				std::function<void(MessageType, size_t, std::shared_ptr<Message>&)> OnMessage;

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

			private:
				bool _disposed;
				bool _isSecure;

				unsigned short _port;
				IPVersionType _ipv;
				std::string _serverName;

				std::shared_ptr<WebRequest> _request;
			};
		}
	}
}