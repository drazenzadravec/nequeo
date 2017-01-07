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

#pragma once

#include "GlobalWebSockets.h"

namespace Nequeo {
	namespace Net {
		namespace WebSocket
		{
			// Forward declare server.
			class Server;

			/// <summary>
			/// On client connected handler.
			/// </summary>
			/// <param name="server">The current web socket server.</param>
			/// <param name="serverName">The unique web socket server name.</param>
			/// <param name="client">The client web socket instance.</param>
			typedef std::function<void(Server*, const QString&, QWebSocket*)> OnClientConnectedHandler;

			/// <summary>
			/// On client disconnected handler.
			/// </summary>
			/// <param name="server">The current web socket server.</param>
			/// <param name="serverName">The unique web socket server name.</param>
			/// <param name="client">The client web socket instance.</param>
			typedef std::function<void(Server*, const QString&, QWebSocket*)> OnClientDisconnectedHandler;

			/// <summary>
			/// On ssl error handler.
			/// </summary>
			/// <param name="server">The current web socket server.</param>
			/// <param name="serverName">The unique web socket server name.</param>
			/// <param name="error">The ssl error.</param>
			typedef std::function<void(Server*, const QString&, const QList<QSslError>&)> OnSslErrorsServerHandler;

			/// <summary>
			/// On socket error handler.
			/// </summary>
			/// <param name="server">The current web socket server.</param>
			/// <param name="serverName">The unique web socket server name.</param>
			/// <param name="error">The ssl error.</param>
			/// <param name="client">The client web socket instance.</param>
			typedef std::function<void(Server*, const QString&, QWebSocket*, QAbstractSocket::SocketError)> OnClientSocketErrorsHandler;

			/// <summary>
			/// On text message received handler.
			/// </summary>
			/// <param name="server">The current web socket server.</param>
			/// <param name="serverName">The unique web socket server name.</param>
			/// <param name="message">The text message.</param>
			/// <param name="client">The client web socket instance.</param>
			typedef std::function<void(Server*, const QString&, QString, QWebSocket*)> OnClientTextMessageReceivedHandler;

			/// <summary>
			/// On binary message received handler.
			/// </summary>
			/// <param name="server">The current web socket server.</param>
			/// <param name="serverName">The unique web socket server name.</param>
			/// <param name="message">The binary message.</param>
			/// <param name="client">The client web socket instance.</param>
			typedef std::function<void(Server*, const QString&, QByteArray, QWebSocket*)> OnClientBinaryMessageReceivedHandler;

			/// <summary>
			/// On pong handler.
			/// </summary>
			/// <param name="server">The current web socket server.</param>
			/// <param name="serverName">The unique web socket server name.</param>
			/// <param name="client">The client web socket instance.</param>
			/// <param name="elapsedTime">The elapsed time.</param>
			/// <param name="payload">The payload.</param>
			typedef std::function<void(Server*, const QString&, QWebSocket*, quint64 elapsedTime, QByteArray payload)> OnClientPongHandler;

			/// <summary>
			/// On state changed handler.
			/// </summary>
			/// <param name="server">The current web socket server.</param>
			/// <param name="serverName">The unique web socket server name.</param>
			/// <param name="client">The client web socket instance.</param>
			/// <param name="state">The new state.</param>
			typedef std::function<void(Server*, const QString&, QWebSocket*, QAbstractSocket::SocketState)> OnClientStateChangedHandler;

			/// <summary>
			/// Web socket server.
			/// </summary>
			class EXPORT_NEQUEO_WEBSOCKETS_QT_API Server : public QObject
			{
				Q_OBJECT

			public:
				/// <summary>
				/// Web socket server.
				/// </summary>
				/// <param name="secureMode">The secure mode.</param>
				/// <param name="serverName">The server name.</param>
				Server(
					QWebSocketServer::SslMode secureMode = QWebSocketServer::SecureMode,
					const QString &serverName = "WebSocket Server");

				/// <summary>
				/// This destructor. Call release to cleanup resources.
				/// </summary>
				virtual ~Server();

				// <summary>
				/// Set on client connected handler.
				/// </summary>
				/// <param name="handler">The on client connected call back handler.</param>
				void SetClientConnectedHandler(OnClientConnectedHandler handler);

				// <summary>
				/// Set on client disconnected handler.
				/// </summary>
				/// <param name="handler">The on client disconnected call back handler.</param>
				void SetClientDisconnectedHandler(OnClientDisconnectedHandler handler);

				/// <summary>
				/// Set on ssl error handler.
				/// </summary>
				/// <param name="handler">The on ssl error call back handler.</param>
				void SetOnSslErrorsHandler(OnSslErrorsServerHandler handler);

				/// <summary>
				/// Set on text message received handler.
				/// </summary>
				/// <param name="handler">The on text message received call back handler.</param>
				void SetOnTextMessageReceivedHandler(OnClientTextMessageReceivedHandler handler);

				/// <summary>
				/// Set on binary message received handler.
				/// </summary>
				/// <param name="handler">The on binary message received call back handler.</param>
				void SetOnBinaryMessageReceivedHandler(OnClientBinaryMessageReceivedHandler handler);

				/// <summary>
				/// Set on socket error handler.
				/// </summary>
				/// <param name="handler">The on socket error call back handler.</param>
				void SetOnClientSocketErrorsHandler(OnClientSocketErrorsHandler handler);

				/// <summary>
				/// Set on pong handler.
				/// </summary>
				/// <param name="handler">The on pong call back handler.</param>
				void SetOnClientPongHandler(OnClientPongHandler handler);

				/// <summary>
				/// Set on state changed handler.
				/// </summary>
				/// <param name="handler">The on state changed call back handler.</param>
				void SetOnClientStateChangedHandler(OnClientStateChangedHandler handler);

				/// <summary>
				/// Create the ssl configuration for the server.
				/// </summary>
				/// <param name="publicKey">The public certificate key filename.</param>
				/// <param name="privateKey">The private key filename.</param>
				void SslCertificate(QFile publicKey, QFile privateKey);

				/// <summary>
				/// Create the ssl configuration for the server.
				/// </summary>
				/// <param name="sslConfiguration">The ssl configuration.</param>
				void SslCertificate(const QSslConfiguration& sslConfiguration);

				/// <summary>
				/// Has the server been started.
				/// </summary>
				/// <returns>True if started; else false.</returns>
				bool HasStarted();

				/// <summary>
				/// Is the server listening.
				/// </summary>
				/// <returns>True if listening; else false.</returns>
				bool IsListening();

				/// <summary>
				/// Start the server.
				/// </summary>
				void Start(quint16 port = 443, QHostAddress address = QHostAddress::Any);

				/// <summary>
				/// Stop the server.
				/// </summary>
				void Stop();

				/// <summary>
				/// Gets the server name.
				/// </summary>
				QString GetServerName() const;

				/// <summary>
				/// Gets the secure mode.
				/// </summary>
				QWebSocketServer::SslMode GetSecureMode() const;

				/// <summary>
				/// Gets the ssl configuration.
				/// </summary>
				QSslConfiguration GetSslConfiguration() const;

				/// <summary>
				/// Gets the port.
				/// </summary>
				quint16 GetPort() const;

				/// <summary>
				/// Gets the endpoint address.
				/// </summary>
				QHostAddress GetAddress() const;

				/// <summary>
				/// Gets the list of connected clients.
				/// </summary>
				QList<QWebSocket*> GetClients() const;

			private Q_SLOTS:
				/// <summary>
				/// On client connected handle.
				/// </summary>
				void OnClientConnectedHandle();

				/// <summary>
				/// On client disconnected handle.
				/// </summary>
				void OnClientDisconnectedHandle();

				/// <summary>
				/// On ssl error handle.
				/// </summary>
				/// <param name="errors">The ssl error.</param>
				void OnSslErrorsHandle(const QList<QSslError> &errors);

				/// <summary>
				/// On socket error handle.
				/// </summary>
				/// <param name="errors">The socket error.</param>
				void OnClientSocketErrorHandle(QAbstractSocket::SocketError error);

				/// <summary>
				/// On text message received handle.
				/// </summary>
				/// <param name="message">The text message.</param>
				void OnTextMessageReceivedHandle(QString message);

				/// <summary>
				/// On binary message received handle.
				/// </summary>
				/// <param name="message">The binary message.</param>
				void OnBinaryMessageReceivedHandle(QByteArray message);

				/// <summary>
				/// On pong handle.
				/// </summary>
				/// <param name="elapsedTime">The elapsed time.</param>
				/// <param name="payload">The payload.</param>
				void OnClientPongHandle(quint64 elapsedTime, QByteArray payload);

				/// <summary>
				/// On state changed handle.
				/// </summary>
				/// <param name="state">The new state.</param>
				void OnClientStateChangedHandle(QAbstractSocket::SocketState state);

			private:
				bool _disposed;
				bool _started;
				bool _listening;

				QString _serverName;
				QWebSocketServer::SslMode _secureMode;
				QSslConfiguration _sslConfiguration;

				quint16 _port;
				QHostAddress _address;

				QList<QWebSocket*> _clients;
				std::shared_ptr<QWebSocketServer> _qWebSocketServer;

				OnClientConnectedHandler _onClientConnectedHandler;
				OnClientDisconnectedHandler _onClientDisconnectedHandler;
				OnSslErrorsServerHandler _onSslErrorsServerHandler;
				OnClientSocketErrorsHandler _onClientSocketErrorsHandler;
				OnClientTextMessageReceivedHandler _onClientTextMessageReceivedHandler;
				OnClientBinaryMessageReceivedHandler _onClientBinaryMessageReceivedHandler;
				OnClientPongHandler _onClientPongHandler;
				OnClientStateChangedHandler _onClientStateChangedHandler;
			};
		}
	}
}