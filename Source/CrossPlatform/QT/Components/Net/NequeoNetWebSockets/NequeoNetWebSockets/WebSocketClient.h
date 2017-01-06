/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2017 http://www.nequeo.com.au/
*
*  File :          WebSocketClient.h
*  Purpose :       WebSocketClient class.
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
			// Forward declare client.
			class Client;

			/// <summary>
			/// On connected handler.
			/// </summary>
			/// <param name="client">The current web socket client.</param>
			/// <param name="id">The unique web socket client id.</param>
			typedef std::function<void(Client*, const std::string&)> OnConnectedHandler;

			/// <summary>
			/// On disconnected handler.
			/// </summary>
			/// <param name="client">The current web socket client.</param>
			/// <param name="id">The unique web socket client id.</param>
			typedef std::function<void(Client*, const std::string&)> OnDisconnectedHandler;

			/// <summary>
			/// On text message received handler.
			/// </summary>
			/// <param name="client">The current web socket client.</param>
			/// <param name="id">The unique web socket client id.</param>
			/// <param name="message">The text message.</param>
			typedef std::function<void(Client*, const std::string&, QString)> OnTextMessageReceivedHandler;

			/// <summary>
			/// On binary message received handler.
			/// </summary>
			/// <param name="client">The current web socket client.</param>
			/// <param name="id">The unique web socket client id.</param>
			/// <param name="message">The binary message.</param>
			typedef std::function<void(Client*, const std::string&, QByteArray)> OnBinaryMessageReceivedHandler;

			/// <summary>
			/// On ssl error handler.
			/// </summary>
			/// <param name="client">The current web socket client.</param>
			/// <param name="id">The unique web socket client id.</param>
			/// <param name="error">The ssl error.</param>
			typedef std::function<void(Client*, const std::string&, const QList<QSslError>&)> OnSslErrorsHandler;

			/// <summary>
			/// On socket error handler.
			/// </summary>
			/// <param name="client">The current web socket client.</param>
			/// <param name="id">The unique web socket client id.</param>
			/// <param name="error">The ssl error.</param>
			typedef std::function<void(Client*, const std::string&, QAbstractSocket::SocketError)> OnSocketErrorsHandler;

			/// <summary>
			/// On pong handler.
			/// </summary>
			/// <param name="client">The current web socket client.</param>
			/// <param name="id">The unique web socket client id.</param>
			/// <param name="elapsedTime">The elapsed time.</param>
			/// <param name="payload">The payload.</param>
			typedef std::function<void(Client*, const std::string&, quint64 elapsedTime, QByteArray payload)> OnPongHandler;

			/// <summary>
			/// On state changed handler.
			/// </summary>
			/// <param name="client">The current web socket client.</param>
			/// <param name="id">The unique web socket client id.</param>
			/// <param name="state">The new state.</param>
			typedef std::function<void(Client*, const std::string&, QAbstractSocket::SocketState)> OnStateChangedHandler;

			/// <summary>
			/// On proxy required handler.
			/// </summary>
			/// <param name="client">The current web socket client.</param>
			/// <param name="id">The unique web socket client id.</param>
			/// <param name="proxy">The proxy.</param>
			/// <param name="authenticator">The authenticator.</param>
			typedef std::function<void(Client*, const std::string&, const QNetworkProxy&, QAuthenticator*)> OnProxyAuthenticationRequiredHandler;

			/// <summary>
			/// On read channel finished handler.
			/// </summary>
			/// <param name="client">The current web socket client.</param>
			/// <param name="id">The unique web socket client id.</param>
			typedef std::function<void(Client*, const std::string&)> OnReadChannelFinishedHandler;

			/// <summary>
			/// On text frame received handler.
			/// </summary>
			/// <param name="client">The current web socket client.</param>
			/// <param name="id">The unique web socket client id.</param>
			/// <param name="frame">The text frame.</param>
			/// <param name="isLastFrame">Is this the last frame.</param>
			typedef std::function<void(Client*, const std::string&, QString, bool)> OnTextFrameReceivedHandler;

			/// <summary>
			/// On binary frame received handler.
			/// </summary>
			/// <param name="client">The current web socket client.</param>
			/// <param name="id">The unique web socket client id.</param>
			/// <param name="frame">The binary frame.</param>
			/// <param name="isLastFrame">Is this the last frame.</param>
			typedef std::function<void(Client*, const std::string&, QByteArray, bool)> OnBinaryFrameReceivedHandler;

			/// <summary>
			/// Web socket client.
			/// </summary>
			class EXPORT_NEQUEO_WEBSOCKETS_QT_API Client : public QObject
			{
				Q_OBJECT

			public:
				/// <summary>
				/// Web socket client.
				/// </summary>
				Client();

				/// <summary>
				/// This destructor. Call release to cleanup resources.
				/// </summary>
				virtual ~Client();

				/// <summary>
				/// On connected handler.
				/// </summary>
				/// <param name="url">The URL to make the connection to.</param>
				/// <param name="handler">The on connected call back handler.</param>
				void Connect(const QUrl &url, OnConnectedHandler handler);

				/// <summary>
				/// Make a connection.
				/// </summary>
				/// <param name="request">The network request containing the connection details.</param>
				/// <param name="handler">The on connected call back handler.</param>
				void Connect(const QNetworkRequest &request, OnConnectedHandler handler);

				/// <summary>
				/// Close the connection.
				/// </summary>
				/// <param name="closeCode">The close code.</param>
				/// <param name="reason">The close reason.</param>
				void Close(
					QWebSocketProtocol::CloseCode closeCode = QWebSocketProtocol::CloseCodeNormal,
					const QString &reason = QString());

				/// <summary>
				/// Abort communication.
				/// </summary>
				void Abort();

				/// <summary>
				/// Send text message.
				/// </summary>
				/// <param name="message">The text message.</param>
				/// <returns>The number of bytes sent.</returns>
				qint64 SendTextMessage(const QString &message);

				/// <summary>
				/// Send binary message.
				/// </summary>
				/// <param name="message">The binary message.</param>
				/// <returns>The number of bytes sent.</returns>
				qint64 SendBinaryMessage(const QByteArray &message);

				/// <summary>
				/// Send ping.
				/// </summary>
				/// <param name="payload">The payload.</param>
				void Ping(const QByteArray &payload = QByteArray());

				/// <summary>
				/// Ignore ssl errors.
				/// </summary>
				void IgnoreSslErrors();

				/// <summary>
				/// Ignore ssl errors.
				/// </summary>
				/// <param name="errors">The the list of errors to ignore.</param>
				void IgnoreSslErrors(const QList<QSslError> &errors);

				/// <summary>
				/// Resume from pause.
				/// </summary>
				void Resume();

				/// <summary>
				/// Set on disconnected handler.
				/// </summary>
				/// <param name="handler">The on disconnected call back handler.</param>
				void SetOnDisconnectedHandler(OnDisconnectedHandler handler);

				/// <summary>
				/// Set on text message received handler.
				/// </summary>
				/// <param name="handler">The on text message received call back handler.</param>
				void SetOnTextMessageReceivedHandler(OnTextMessageReceivedHandler handler);

				/// <summary>
				/// Set on binary message received handler.
				/// </summary>
				/// <param name="handler">The on binary message received call back handler.</param>
				void SetOnBinaryMessageReceivedHandler(OnBinaryMessageReceivedHandler handler);

				/// <summary>
				/// Set on ssl error handler.
				/// </summary>
				/// <param name="handler">The on ssl error call back handler.</param>
				void SetOnSslErrorsHandler(OnSslErrorsHandler handler);

				/// <summary>
				/// Set on socket error handler.
				/// </summary>
				/// <param name="handler">The on socket error call back handler.</param>
				void SetOnSocketErrorsHandler(OnSocketErrorsHandler handler);

				/// <summary>
				/// Set on pong handler.
				/// </summary>
				/// <param name="handler">The on pong call back handler.</param>
				void SetOnPongHandler(OnPongHandler handler);

				/// <summary>
				/// Set on state changed handler.
				/// </summary>
				/// <param name="handler">The on state changed call back handler.</param>
				void SetOnStateChangedHandler(OnStateChangedHandler handler);

				/// <summary>
				/// Set on proxy authentication required handler.
				/// </summary>
				/// <param name="handler">The on proxy authentication required call back handler.</param>
				void SetOnProxyAuthenticationRequiredHandler(OnProxyAuthenticationRequiredHandler handler);

				/// <summary>
				/// Set on read channel finished handler.
				/// </summary>
				/// <param name="handler">The on read channel finished call back handler.</param>
				void SetOnReadChannelFinishedHandler(OnReadChannelFinishedHandler handler);

				/// <summary>
				/// Set on text frame received handler.
				/// </summary>
				/// <param name="handler">The on text frame received call back handler.</param>
				void SetOnTextFrameReceivedHandler(OnTextFrameReceivedHandler handler);

				/// <summary>
				/// Set on binary frame received handler.
				/// </summary>
				/// <param name="handler">The on binary frame received call back handler.</param>
				void SetOnBinaryFrameReceivedHandler(OnBinaryFrameReceivedHandler handler);

				/// <summary>
				/// Gets or sets the client unique id.
				/// </summary>
				void SetUniqueID(const std::string &id);
				std::string GetUniqueID() const;

				/// <summary>
				/// Gets or sets the client ssl configuration.
				/// </summary>
				void SetSslConfiguration(const QSslConfiguration &sslConfiguration);
				QSslConfiguration GetSslConfiguration() const;

				/// <summary>
				/// Gets the error string if any.
				/// </summary>
				QString ErrorString() const;

				/// <summary>
				/// Gets the socket state.
				/// </summary>
				QAbstractSocket::SocketState State() const;

				/// <summary>
				/// Gets the origin.
				/// </summary>
				QString Origin() const;

				/// <summary>
				/// Gets the close code.
				/// </summary>
				QWebSocketProtocol::CloseCode CloseCode() const;

				/// <summary>
				/// Gets the close reason.
				/// </summary>
				QString CloseReason() const;

				/// <summary>
				/// Gets the is valid indicator.
				/// </summary>
				bool IsValid() const;

				/// <summary>
				/// Gets the local ip address.
				/// </summary>
				QHostAddress LocalAddress() const;

				/// <summary>
				/// Gets the local port.
				/// </summary>
				quint16 LocalPort() const;
				
				/// <summary>
				/// Gets or sets the pause mode.
				/// </summary>
				void SetPauseMode(QAbstractSocket::PauseModes pauseMode);
				QAbstractSocket::PauseModes GetPauseMode() const;

				/// <summary>
				/// Gets the remote ip address.
				/// </summary>
				QHostAddress PeerAddress() const;

				/// <summary>
				/// Gets the remote name.
				/// </summary>
				QString PeerName() const;

				/// <summary>
				/// Gets the remote port.
				/// </summary>
				quint16 PeerPort() const;

				/// <summary>
				/// Gets or sets the read buffer size.
				/// </summary>
				void SetReadBufferSize(qint64 size);
				qint64 GetReadBufferSize() const;

				/// <summary>
				/// Gets the resource name.
				/// </summary>
				QString ResourceName() const;

				/// <summary>
				/// Gets the request URL.
				/// </summary>
				QUrl RequestUrl() const;

				/// <summary>
				/// Gets the request.
				/// </summary>
				QNetworkRequest Request() const;

				/// <summary>
				/// Gets or sets the proxy.
				/// </summary>
				void SetProxy(const QNetworkProxy &networkProxy);
				QNetworkProxy GetProxy() const;

			private Q_SLOTS:
				/// <summary>
				/// On connected handle.
				/// </summary>
				void OnConnectedHandle();

				/// <summary>
				/// On disconnected handle.
				/// </summary>
				void OnDisonnectedHandle();

				/// <summary>
				/// On about to close handle.
				/// </summary>
				void OnAboutToClose();

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
				/// On ssl error handle.
				/// </summary>
				/// <param name="errors">The ssl error.</param>
				void OnSslErrorsHandle(const QList<QSslError> &errors);

				/// <summary>
				/// On socket error handle.
				/// </summary>
				/// <param name="errors">The socket error.</param>
				void OnSocketErrorHandle(QAbstractSocket::SocketError error);

				/// <summary>
				/// On pong handle.
				/// </summary>
				/// <param name="elapsedTime">The elapsed time.</param>
				/// <param name="payload">The payload.</param>
				void OnPongHandle(quint64 elapsedTime, QByteArray payload);

				/// <summary>
				/// On state changed handle.
				/// </summary>
				/// <param name="state">The new state.</param>
				void OnStateChangedHandle(QAbstractSocket::SocketState state);

				/// <summary>
				/// On proxy authentication required handle.
				/// </summary>
				/// <param name="proxy">The proxy.</param>
				/// <param name="authenticator">The authenticator.</param>
				void OnProxyAuthenticationRequiredHandle(const QNetworkProxy &proxy, QAuthenticator *authenticator);

				/// <summary>
				/// On read channel finished handle.
				/// </summary>
				void OnReadChannelFinishedHandle();

				/// <summary>
				/// On text frame received handle.
				/// </summary>
				/// <param name="frame">The text frame.</param>
				/// <param name="isLastFrame">Is this the last frame.</param>
				void OnTextFrameReceivedHandle(QString frame, bool isLastFrame);

				/// <summary>
				/// On binary frame received handle.
				/// </summary>
				/// <param name="frame">The binary frame.</param>
				/// <param name="isLastFrame">Is this the last frame.</param>
				void OnBinaryFrameReceivedHandle(QByteArray frame, bool isLastFrame);

			private:
				bool _disposed;
				bool _closed;

				std::string _uniqueID;
				QSslConfiguration _sslConfiguration;

				std::shared_ptr<QWebSocket> _qWebSocket;

				OnConnectedHandler _onConnectedHandler;
				OnDisconnectedHandler _onDisconnectedHandler;
				OnTextMessageReceivedHandler _onTextMessageReceivedHandler;
				OnBinaryMessageReceivedHandler _onBinaryMessageReceivedHandler;
				OnSslErrorsHandler _onSslErrorsHandler;
				OnSocketErrorsHandler _onSocketErrorsHandler;
				OnPongHandler _onPongHandler;
				OnStateChangedHandler _onStateChangedHandler;
				OnProxyAuthenticationRequiredHandler _onProxyAuthenticationRequiredHandler;
				OnReadChannelFinishedHandler _onReadChannelFinishedHandler;
				OnTextFrameReceivedHandler _onTextFrameReceivedHandler;
				OnBinaryFrameReceivedHandler _onBinaryFrameReceivedHandler;
			};
		}
	}
}