/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Message.h
*  Purpose :       WebSocket message container.
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
#include "MessageType.h"

#include <iostream>
#include <sstream>

namespace Nequeo {
	namespace Net {
		namespace WebSocket
		{
			/// <summary>
			/// WebSocket message container.
			/// </summary>
			class EXPORT_NEQUEO_WEBSOCKET_BOOST_SERVER_API WebMessage
			{
			public:
				/// <summary>
				/// WebSocket message container.
				/// </summary>
				WebMessage();

				/// <summary>
				/// WebSocket message container.
				/// </summary>
				virtual ~WebMessage();

				///	<summary>
				///	Get the message.
				///	</summary>
				///	<return>The message.</return>
				std::string Get();

				/// <summary>
				/// The messsage received if any.
				/// </summary>
				std::streambuf* Received;

				/// <summary>
				/// Send message.
				/// </summary>
				/// <param name="messageType">The message type.</param>
				/// <param name="message">The message to send.</param>
				void Send(MessageType messageType, std::streambuf* message);

				/// <summary>
				/// On send message function handler (internal use only).
				/// </summary>
				/// <param name="messageType">The message type.</param>
				/// <param name="messsage">The message to send.</param>
				std::function<void(MessageType, std::streambuf*, std::shared_ptr<void>)> OnSend;

				/// <summary>
				/// Internal use only.
				/// </summary>
				std::shared_ptr<void> connectionHandler;

			private:
				bool _disposed;

			};
		}
	}
}