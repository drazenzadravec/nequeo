/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2017 http://www.nequeo.com.au/
*
*  File :          WebSocketServer.h
*  Purpose :       WebSocketServer class.
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

#include "WebSocketServer.h"

using namespace Nequeo::Net::WebSocket;

// Socket error signal function handler.
typedef void (QWebSocket::*socketErrorSignalServer)(QAbstractSocket::SocketError);

/// <summary>
/// Web socket server.
/// </summary>
/// <param name="secureMode">The secure mode.</param>
/// <param name="serverName">The server name.</param>
Server::Server(QWebSocketServer::SslMode secureMode, const QString &serverName) : QObject(nullptr),
	_qWebSocketServer(nullptr),
	_clients(),
	_serverName(serverName),
	_secureMode(secureMode),
	_address(QHostAddress::Any),
	_port(443),
	_started(false),
	_listening(false),
	_disposed(false)
{
	// Create the web socket server.
	_qWebSocketServer = std::make_shared<QWebSocketServer>(serverName, secureMode, this);

	// Attach the web socket events to this class.
	connect(_qWebSocketServer.get(), &QWebSocketServer::newConnection, this, &Server::OnClientConnectedHandle);
	connect(_qWebSocketServer.get(), &QWebSocketServer::sslErrors, this, &Server::OnSslErrorsHandle);
}

/// <summary>
/// This destructor. Call release to cleanup resources.
/// </summary>
Server::~Server()
{
	if (!_disposed)
	{
		_disposed = true;

		// if started.
		if (_started)
		{
			_started = false;
			_qWebSocketServer->close();
		}

		// Call delete on each web socket client.
		qDeleteAll(_clients.begin(), _clients.end());
	}
}

/// <summary>
/// Is the server listening.
/// </summary>
/// <returns>True if listening; else false.</returns>
bool Server::IsListening()
{
	_listening = _qWebSocketServer->isListening();
	return _listening;
}

/// <summary>
/// Has the server been started.
/// </summary>
/// <returns>True if started; else false.</returns>
bool Server::HasStarted()
{
	return _started;
}

/// <summary>
/// Start the server.
/// </summary>
/// <param name="port">The port number.</param>
/// <param name="address">The endpoint address.</param>
void Server::Start(quint16 port, QHostAddress address)
{
	// if not started.
	if (!_started)
	{
		_port = port;
		_address = address;

		// If server is listening.
		if (_qWebSocketServer->listen(_address, _port))
		{
			_started = true;
			_listening = true;
		}
	}
}

/// <summary>
/// Stop the server.
/// </summary>
void Server::Stop()
{
	// if started.
	if (_started)
	{
		_started = false;
		_qWebSocketServer->close();

		// Call delete on each web socket client.
		qDeleteAll(_clients.begin(), _clients.end());
	}
}

// <summary>
/// Set on client connected handler.
/// </summary>
/// <param name="handler">The on client connected call back handler.</param>
void Server::SetClientConnectedHandler(OnClientConnectedHandler handler)
{
	_onClientConnectedHandler = handler;
}

// <summary>
/// Set on client disconnected handler.
/// </summary>
/// <param name="handler">The on client disconnected call back handler.</param>
void Server::SetClientDisconnectedHandler(OnClientDisconnectedHandler handler)
{
	_onClientDisconnectedHandler = handler;
}

/// <summary>
/// Set on ssl error handler.
/// </summary>
/// <param name="handler">The on ssl error call back handler.</param>
void Server::SetOnSslErrorsHandler(OnSslErrorsServerHandler handler)
{
	_onSslErrorsServerHandler = handler;
}

/// <summary>
/// Set on socket error handler.
/// </summary>
/// <param name="handler">The on socket error call back handler.</param>
void Server::SetOnClientSocketErrorsHandler(OnClientSocketErrorsHandler handler)
{
	_onClientSocketErrorsHandler = handler;
}

/// <summary>
/// Set on text message received handler.
/// </summary>
/// <param name="handler">The on text message received call back handler.</param>
void Server::SetOnTextMessageReceivedHandler(OnClientTextMessageReceivedHandler handler)
{
	_onClientTextMessageReceivedHandler = handler;
}

/// <summary>
/// Set on binary message received handler.
/// </summary>
/// <param name="handler">The on binary message received call back handler.</param>
void Server::SetOnBinaryMessageReceivedHandler(OnClientBinaryMessageReceivedHandler handler)
{
	_onClientBinaryMessageReceivedHandler = handler;
}

/// <summary>
/// Set on pong handler.
/// </summary>
/// <param name="handler">The on pong call back handler.</param>
void Server::SetOnClientPongHandler(OnClientPongHandler handler)
{
	_onClientPongHandler = handler;
}

/// <summary>
/// Set on state changed handler.
/// </summary>
/// <param name="handler">The on state changed call back handler.</param>
void Server::SetOnClientStateChangedHandler(OnClientStateChangedHandler handler)
{
	_onClientStateChangedHandler = handler;
}

/// <summary>
/// Create the ssl configuration for the server.
/// </summary>
/// <param name="publicKey">The public certificate key filename.</param>
/// <param name="privateKey">The private key filename.</param>
void Server::SslCertificate(QFile publicKey, QFile privateKey)
{
	QSslConfiguration sslConfiguration;
	
	// Open the files.
	publicKey.open(QIODevice::ReadOnly);
	privateKey.open(QIODevice::ReadOnly);

	// Create the certificate.
	QSslCertificate certificate(&publicKey, QSsl::Pem);
	QSslKey sslKey(&privateKey, QSsl::Rsa, QSsl::Pem);

	// Close the files.
	publicKey.close();
	privateKey.close();

	// Assign the ssl configuration.
	sslConfiguration.setPeerVerifyMode(QSslSocket::VerifyNone);
	sslConfiguration.setLocalCertificate(certificate);
	sslConfiguration.setPrivateKey(sslKey);
	sslConfiguration.setProtocol(QSsl::TlsV1_2);

	// Add the ssl configuration to the server.
	_qWebSocketServer->setSslConfiguration(sslConfiguration);
}

/// <summary>
/// Create the ssl configuration for the server.
/// </summary>
/// <param name="sslConfiguration">The ssl configuration.</param>
void Server::SslCertificate(const QSslConfiguration& sslConfiguration)
{
	_sslConfiguration = sslConfiguration;
	
	// Add the ssl configuration to the server.
	_qWebSocketServer->setSslConfiguration(sslConfiguration);
}

/// <summary>
/// Gets the server name.
/// </summary>
QString Server::GetServerName() const
{
	return _serverName;
}

/// <summary>
/// Gets the secure mode.
/// </summary>
QWebSocketServer::SslMode Server::GetSecureMode() const
{
	return _secureMode;
}

/// <summary>
/// Gets the ssl configuration.
/// </summary>
QSslConfiguration Server::GetSslConfiguration() const
{
	return _sslConfiguration;
}

/// <summary>
/// Gets the port.
/// </summary>
quint16 Server::GetPort() const
{
	return _port;
}

/// <summary>
/// Gets the endpoint address.
/// </summary>
QHostAddress Server::GetAddress() const
{
	return _address;
}

/// <summary>
/// Gets the list of connected clients.
/// </summary>
QList<QWebSocket*> Server::GetClients() const
{
	return _clients;
}

/// <summary>
/// On client connected handle.
/// </summary>
void Server::OnClientConnectedHandle()
{
	// Get the next client connection.
	QWebSocket *pWebSocket = _qWebSocketServer->nextPendingConnection();

	// Assign client events.
	connect(pWebSocket, &QWebSocket::textMessageReceived, this, &Server::OnTextMessageReceivedHandle);
	connect(pWebSocket, &QWebSocket::binaryMessageReceived, this, &Server::OnBinaryMessageReceivedHandle);
	connect(pWebSocket, &QWebSocket::disconnected, this, &Server::OnClientDisconnectedHandle);
	connect(pWebSocket, static_cast<socketErrorSignalServer>(&QWebSocket::error), this, &Server::OnClientSocketErrorHandle);
	connect(pWebSocket, &QWebSocket::pong, this, &Server::OnClientPongHandle);
	connect(pWebSocket, &QWebSocket::stateChanged, this, &Server::OnClientStateChangedHandle);

	// Add the client to the list.
	_clients << pWebSocket;

	// Call the handler.
	_onClientConnectedHandler(this, _serverName, pWebSocket);
}

/// <summary>
/// On client disconnected handle.
/// </summary>
void Server::OnClientDisconnectedHandle()
{
	// Get the current client.
	QWebSocket *pWebSocket = qobject_cast<QWebSocket*>(sender());

	// If exists
	if (pWebSocket)
	{
		// Call the handler.
		_onClientDisconnectedHandler(this, _serverName, pWebSocket);

		// Remove from the list.
		_clients.removeAll(pWebSocket);

		// Delete the instance.
		pWebSocket->deleteLater();
	}
}

/// <summary>
/// On ssl error handle.
/// </summary>
/// <param name="errors">The ssl error.</param>
void Server::OnSslErrorsHandle(const QList<QSslError> &errors)
{
	_onSslErrorsServerHandler(this, _serverName, errors);
}

/// <summary>
/// On socket error handle.
/// </summary>
/// <param name="errors">The socket error.</param>
void Server::OnClientSocketErrorHandle(QAbstractSocket::SocketError error)
{
	// Get the current client.
	QWebSocket *pWebSocket = qobject_cast<QWebSocket*>(sender());

	// If exists
	if (pWebSocket)
	{
		// Call the handler.
		_onClientSocketErrorsHandler(this, _serverName, pWebSocket, error);
	}
}

/// <summary>
/// On text message received handle.
/// </summary>
/// <param name="message">The text message.</param>
void Server::OnTextMessageReceivedHandle(QString message)
{
	// Get the current client.
	QWebSocket *pWebSocket = qobject_cast<QWebSocket*>(sender());

	// If exists
	if (pWebSocket)
	{
		// Call the handler.
		_onClientTextMessageReceivedHandler(this, _serverName, message, pWebSocket);
	}
}

/// <summary>
/// On binary message received handle.
/// </summary>
/// <param name="message">The binary message.</param>
void Server::OnBinaryMessageReceivedHandle(QByteArray message)
{
	// Get the current client.
	QWebSocket *pWebSocket = qobject_cast<QWebSocket*>(sender());

	// If exists
	if (pWebSocket)
	{
		// Call the handler.
		_onClientBinaryMessageReceivedHandler(this, _serverName, message, pWebSocket);
	}
}

/// <summary>
/// On pong handle.
/// </summary>
/// <param name="elapsedTime">The elapsed time.</param>
/// <param name="payload">The payload.</param>
void Server::OnClientPongHandle(quint64 elapsedTime, QByteArray payload)
{
	// Get the current client.
	QWebSocket *pWebSocket = qobject_cast<QWebSocket*>(sender());

	// If exists
	if (pWebSocket)
	{
		// Call the handler.
		_onClientPongHandler(this, _serverName, pWebSocket, elapsedTime, payload);
	}
}

/// <summary>
/// On state changed handle.
/// </summary>
/// <param name="state">The new state.</param>
void Server::OnClientStateChangedHandle(QAbstractSocket::SocketState state)
{
	// Get the current client.
	QWebSocket *pWebSocket = qobject_cast<QWebSocket*>(sender());
	
	// If exists
	if (pWebSocket)
	{
		// Call the handler.
		_onClientStateChangedHandler(this, _serverName, pWebSocket, state);
	}
}