/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WebClient.h
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
#include "Global.h"

#include "NetContext.h"
#include "IPVersionType.h"

#include "Threading\Executor.h"
#include "Threading\ThreadTask.h"
#include "Base\AsyncCallerContext.h"

namespace Nequeo {
	namespace Net {
		namespace WebSocket
		{
			/// <summary>
			/// WebSocket web client.
			/// </summary>
			class EXPORT_NEQUEO_WEBSOCKET_BOOST_SERVER_API WebClient
			{
			public:
				/// <summary>
				/// WebSocket web client.
				/// </summary>
				/// <param name="host">The host (name or IP).</param>
				/// <param name="port">The host port number.</param>
				/// <param name="isSecure">Is the connection secure.</param>
				/// <param name="ipv">The IP version to use.</param>
				WebClient(const std::string& host, unsigned short port = 80, bool isSecure = false, IPVersionType ipv = IPVersionType::IPv4);

				/// <summary>
				/// WebSocket web client.
				/// </summary>
				virtual ~WebClient();

				/// <summary>
				/// Get the net context.
				/// </summary>
				/// <return>The net context.</return>
				NetContext& Context() const;

				///	<summary>
				///	Start a connection.
				///	</summary>
				void Connect();

				///	<summary>
				///	Disconnect.
				///	</summary>
				void Disconnect();

				///	<summary>
				///	Close the client.
				///	</summary>
				/// <param name="status">The current status.</param>
				/// <param name="reason">The close reason.</param>
				void Close(int status = 1000, const std::string& reason = "");

				///	<summary>
				///	Send message.
				///	</summary>
				/// <param name="messageType">The message type.</param>
				/// <param name="message">The message to send.</param>
				void Send(MessageType messageType, std::streambuf* message);

			public:
				/// <summary>
				/// On message received function handler.
				/// </summary>
				/// <param name="messageType">The message type.</param>
				/// <param name="length">The length of the message.</param>
				/// <param name="message">The message.</param>
				std::function<void(MessageType, size_t, std::shared_ptr<Message>&)> OnMessage;

				/// <summary>
				/// On open connection.
				/// </summary>
				std::function<void(void)> OnOpen;

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
				bool _active;
				bool _connected;
				bool _isSecure;

				int _clientIndex;
				IPVersionType _ipv;
				std::string _host;
				unsigned short _port;

				std::shared_ptr<NetContext> _context;
				std::shared_ptr<Nequeo::Threading::Executor> _executor;

				void CreateNetContext();
			};
		}
	}
}