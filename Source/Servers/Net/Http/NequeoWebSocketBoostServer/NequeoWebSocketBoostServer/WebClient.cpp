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
	_disposed(false), _active(false), _connected(false), _isSecure(isSecure), _clientIndex(-1), _ipv(ipv), _port(port), _host(host)
{
	// Create a new executor.
	_executor = Nequeo::MakeShared<Nequeo::Threading::DefaultExecutor>(WEBCLIENT_WS_CLIENT_TAG);

	// Create a new context.
	CreateNetContext();
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
	_connected = false;
	_clientIndex = -1;
}

/// <summary>
/// Get the net context.
/// </summary>
/// <return>The net context.</return>
NetContext& WebClient::Context() const
{
	return *(_context.get());
}

///	<summary>
///	Disconnect.
///	</summary>
void WebClient::Disconnect()
{
	// If active.
	if (_active && _connected)
	{
		// If not secure.
		if (!_isSecure)
		{
			// Get this client.
			std::shared_ptr<InternalWebSocketClient> client = clientPtr[_clientIndex];
			client->stop();
		}
		else
		{
			// Get this client.
			std::shared_ptr<InternalSecureWebSocketClient> client = clientSecurePtr[_clientIndex];
			client->stop();
		}

		// disconnection called.
		_connected = false;
	}
}

///	<summary>
///	Close the client.
///	</summary>
/// <param name="status">The current status.</param>
/// <param name="reason">The close reason.</param>
void WebClient::Close(int status, const std::string& reason)
{
	// If active.
	if (_active && _connected)
	{
		// If not secure.
		if (!_isSecure)
		{
			// Get this client.
			std::shared_ptr<InternalWebSocketClient> client = clientPtr[_clientIndex];
			client->send_close(status, reason);
		}
		else
		{
			// Get this client.
			std::shared_ptr<InternalSecureWebSocketClient> client = clientSecurePtr[_clientIndex];
			client->send_close(status, reason);
		}
	}
}

///	<summary>
///	Start a connection.
///	</summary>
void WebClient::Connect()
{
	// If active.
	if (_active && !_connected)
	{
		// Connection called.
		_connected = true;

		// Get client context.
		std::shared_ptr<NetContext> context = _context;

		// If not secure.
		if (!_isSecure)
		{
			// Get this client.
			std::shared_ptr<InternalWebSocketClient> client = clientPtr[_clientIndex];
			client->onopen = [this, &client, &context]()
			{
				// Assign the context.
				client->connection->netContext = context;

				// Get the request and response.
				NetResponse* netResponse = &context->Response();
				
				// Assign values.
				netResponse->SetRemoteEndpointAddress(client->connection->remote_endpoint_address);
				netResponse->SetRemoteEndpointPort(client->connection->remote_endpoint_port);
				netResponse->SetProtocolVersion("http 1.1");

				// Assign the headers.
				std::map<std::string, std::string> headers;
				typedef std::pair<std::string, std::string> headerPair;

				// For each header.
				for (auto& h : client->connection->header)
				{
					// Write header name and value.
					headers.insert(headerPair(h.first.c_str(), h.second.c_str()));
				}

				// Assign the header and content.
				netResponse->SetHeaders(headers);

				// If on open connection.
				if (this->OnOpen)
				{
					// Call open.
					this->OnOpen();
				}
			};

			// Start a connection.
			client->start();
		}
		else
		{
			// Get this client.
			std::shared_ptr<InternalSecureWebSocketClient> client = clientSecurePtr[_clientIndex];
			client->onopen = [this, &client, &context]()
			{
				// Assign the context.
				client->connection->netContext = context;

				// Get the request and response.
				NetResponse* netResponse = &context->Response();

				// Assign values.
				netResponse->SetRemoteEndpointAddress(client->connection->remote_endpoint_address);
				netResponse->SetRemoteEndpointPort(client->connection->remote_endpoint_port);
				netResponse->SetProtocolVersion("http 1.1");

				// Assign the headers.
				std::map<std::string, std::string> headers;
				typedef std::pair<std::string, std::string> headerPair;

				// For each header.
				for (auto& h : client->connection->header)
				{
					// Write header name and value.
					headers.insert(headerPair(h.first.c_str(), h.second.c_str()));
				}

				// Assign the header and content.
				netResponse->SetHeaders(headers);

				// If on open connection.
				if (this->OnOpen)
				{
					// Call open.
					this->OnOpen();
				}
			};

			// Start a connection.
			client->start();
		}
	}
}

///	<summary>
///	Create a new context.
///	</summary>
void WebClient::CreateNetContext()
{
	// If not active.
	if (!_active)
	{
		// Create a new context.
		_context = std::make_shared<NetContext>();
		_context->SetPort(_port);
		_context->SetIsSecure(_isSecure);
		_context->SetIPVersionType(_ipv);

		// If not secure.
		if (!_isSecure)
		{
			// If not created.
			if (_clientIndex < 0)
			{
				++clientWsCount;
				_clientIndex = clientWsCount;

				// WebSocket client
				clientPtr.insert(std::make_pair(_clientIndex, std::make_shared<InternalWebSocketClient>(_host + ":" + std::to_string(_port), _ipv)));

				// Get this client.
				std::shared_ptr<InternalWebSocketClient> client = clientPtr[_clientIndex];

				// On close.
				client->onclose = [this](int status, const string& reason)
				{
					// If handler exists.
					if (this->OnClose)
					{
						// Call close.
						this->OnClose(status, reason);
					}
				};

				// On error.
				client->onerror = [this](const boost::system::error_code& ec) 
				{
					// If handler exists.
					if (this->OnError)
					{
						// Call error.
						this->OnError(ec.message());
					}
				};

				// On message.
				client->onmessage = [this](shared_ptr<InternalWebSocketClient::Message> message)
				{
					// If handler exists.
					if (this->OnMessage)
					{
						MessageType messageType = MessageType::Text;

						// Text
						if ((message->fin_rsv_opcode & 0x0f) == 1)
							messageType = MessageType::Text;

						// Binary
						if ((message->fin_rsv_opcode & 0x0f) == 2)
							messageType = MessageType::Binary;

						// Close
						if ((message->fin_rsv_opcode & 0x0f) == 8)
							messageType = MessageType::Close;

						// Ping
						if ((message->fin_rsv_opcode & 0x0f) == 9)
							messageType = MessageType::Ping;

						// Make message.
						auto webSocketMessage = std::make_shared<Message>();
						webSocketMessage->Received = message->rdbuf();

						// Call message.
						this->OnMessage(messageType, message->size(), webSocketMessage);
					}
				};
			}
		}
		else
		{
			// If not created.
			if (_clientIndex < 0)
			{
				++clientWsCount;
				_clientIndex = clientWsCount;

				// WebSocket client
				clientSecurePtr.insert(std::make_pair(_clientIndex, std::make_shared<InternalSecureWebSocketClient>(_host + ":" + std::to_string(_port), _ipv)));

				// Get this client.
				std::shared_ptr<InternalSecureWebSocketClient> client = clientSecurePtr[_clientIndex];

				// On close.
				client->onclose = [this](int status, const string& reason)
				{
					// If handler exists.
					if (this->OnClose)
					{
						// Call close.
						this->OnClose(status, reason);
					}
				};

				// On error.
				client->onerror = [this](const boost::system::error_code& ec)
				{
					// If handler exists.
					if (this->OnError)
					{
						// Call error.
						this->OnError(ec.message());
					}
				};

				// On message.
				client->onmessage = [this](shared_ptr<InternalSecureWebSocketClient::Message> message)
				{
					// If handler exists.
					if (this->OnMessage)
					{
						MessageType messageType = MessageType::Text;

						// Text
						if ((message->fin_rsv_opcode & 0x0f) == 1)
							messageType = MessageType::Text;

						// Binary
						if ((message->fin_rsv_opcode & 0x0f) == 2)
							messageType = MessageType::Binary;

						// Close
						if ((message->fin_rsv_opcode & 0x0f) == 8)
							messageType = MessageType::Close;

						// Ping
						if ((message->fin_rsv_opcode & 0x0f) == 9)
							messageType = MessageType::Ping;

						// Make message.
						auto webSocketMessage = std::make_shared<Message>();
						webSocketMessage->Received = message->rdbuf();

						// Call message.
						this->OnMessage(messageType, message->size(), webSocketMessage);
					}
				};
			}
		}

		// Active context.
		_active = true;
	}
}

///	<summary>
///	Send message.
///	</summary>
/// <param name="messageType">The message type.</param>
/// <param name="message">The message to send.</param>
void WebClient::Send(MessageType messageType, std::streambuf* message)
{
	// If active.
	if (_active && _connected)
	{
		// If not secure.
		if (!_isSecure)
		{
			// Get this client.
			std::shared_ptr<InternalWebSocketClient> client = clientPtr[_clientIndex];
			
			// Create send stream.
			auto sendStream = std::make_shared<InternalWebSocketClient::SendStream>();
			*sendStream << message;

			unsigned char opcode = 129;

			// Set opcode
			switch (messageType)
			{
			case Nequeo::Net::WebSocket::MessageType::Text:
				opcode = 129;
				break;
			case Nequeo::Net::WebSocket::MessageType::Binary:
				opcode = 130;
				break;
			case Nequeo::Net::WebSocket::MessageType::Close:
				opcode = 136;
				break;
			case Nequeo::Net::WebSocket::MessageType::Ping:
				opcode = 137;
				break;
			default:
				break;
			}

			// Send the message.
			client->send(sendStream,
				[](const boost::system::error_code& ec)
			{
				try
				{
					//// If handler exists.
					//if (connection->webContext && connection->webContext->OnError)
					//{
					//	// Call error.
					//	connection->webContext->OnError(ec.message());
					//}
				}
				catch (...) {}

			}, opcode);
		}
		else
		{
			// Get this client.
			std::shared_ptr<InternalSecureWebSocketClient> client = clientSecurePtr[_clientIndex];
			
			// Create send stream.
			auto sendStream = std::make_shared<InternalSecureWebSocketClient::SendStream>();
			*sendStream << message;

			unsigned char opcode = 129;

			// Set opcode
			switch (messageType)
			{
			case Nequeo::Net::WebSocket::MessageType::Text:
				opcode = 129;
				break;
			case Nequeo::Net::WebSocket::MessageType::Binary:
				opcode = 130;
				break;
			case Nequeo::Net::WebSocket::MessageType::Close:
				opcode = 136;
				break;
			case Nequeo::Net::WebSocket::MessageType::Ping:
				opcode = 137;
				break;
			default:
				break;
			}

			// Send the message.
			client->send(sendStream,
				[](const boost::system::error_code& ec)
			{
				try
				{
					//// If handler exists.
					//if (connection->webContext && connection->webContext->OnError)
					//{
					//	// Call error.
					//	connection->webContext->OnError(ec.message());
					//}
				}
				catch (...) {}

			}, opcode);
		}
	}
}