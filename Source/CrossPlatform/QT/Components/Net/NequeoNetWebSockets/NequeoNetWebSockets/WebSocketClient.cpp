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

#include "stdafx.h"

#include "WebSocketClient.h"

using namespace Nequeo::Net::WebSocket;

// SSL error signal function handler.
typedef void (QWebSocket::*sslErrorsSignal)(const QList<QSslError> &);

// Socket error signal function handler.
typedef void (QWebSocket::*socketErrorSignal)(QAbstractSocket::SocketError);

/// <summary>
/// Web socket client.
/// </summary>
Client::Client() : QObject(nullptr),
	_qWebSocket(nullptr),
	_uniqueID("default"),
	_closed(false),
	_disposed(false)
{
	// Create the web socket.
	_qWebSocket = std::make_shared<QWebSocket>();

	// Attach the web socket events to this class.
	connect(_qWebSocket.get(), &QWebSocket::connected, this, &Client::OnConnectedHandle);
	connect(_qWebSocket.get(), &QWebSocket::disconnected, this, &Client::OnDisonnectedHandle);
	connect(_qWebSocket.get(), &QWebSocket::aboutToClose, this, &Client::OnAboutToClose);
	connect(_qWebSocket.get(), &QWebSocket::pong, this, &Client::OnPongHandle);
	connect(_qWebSocket.get(), &QWebSocket::stateChanged, this, &Client::OnStateChangedHandle);
	connect(_qWebSocket.get(), &QWebSocket::proxyAuthenticationRequired, this, &Client::OnProxyAuthenticationRequiredHandle);
	connect(_qWebSocket.get(), static_cast<socketErrorSignal>(&QWebSocket::error), this, &Client::OnSocketErrorHandle);
	connect(_qWebSocket.get(), static_cast<sslErrorsSignal>(&QWebSocket::sslErrors), this, &Client::OnSslErrorsHandle);
}

/// <summary>
/// This destructor. Call release to cleanup resources.
/// </summary>
Client::~Client()
{
	if (!_disposed)
	{
		_disposed = true;

		// if not closed.
		if (!_closed)
		{
			_closed = true;
			_qWebSocket->close();
		}
	}
}

/// <summary>
/// On connected handler.
/// </summary>
/// <param name="url">The URL to make the connection to.</param>
/// <param name="handler">The on connected call back handler.</param>
void Client::Connect(const QUrl &url, OnConnectedHandler handler)
{
	_onConnectedHandler = handler;
	_qWebSocket->open(QUrl(url));
}

/// <summary>
/// Make a connection.
/// </summary>
/// <param name="request">The network request containing the connection details.</param>
/// <param name="handler">The on connected call back handler.</param>
void Client::Connect(const QNetworkRequest &request, OnConnectedHandler handler)
{
	_onConnectedHandler = handler;
	_qWebSocket->open(request);
}

/// <summary>
/// Set on disconnected handler.
/// </summary>
/// <param name="handler">The on disconnected call back handler.</param>
void Client::SetOnDisconnectedHandler(OnDisconnectedHandler handler)
{
	_onDisconnectedHandler = handler;
}

/// <summary>
/// Set on text message received handler.
/// </summary>
/// <param name="handler">The on text message received call back handler.</param>
void Client::SetOnTextMessageReceivedHandler(OnTextMessageReceivedHandler handler)
{
	_onTextMessageReceivedHandler = handler;
}

/// <summary>
/// Set on binary message received handler.
/// </summary>
/// <param name="handler">The on binary message received call back handler.</param>
void Client::SetOnBinaryMessageReceivedHandler(OnBinaryMessageReceivedHandler handler)
{
	_onBinaryMessageReceivedHandler = handler;
}

/// <summary>
/// Set on ssl error handler.
/// </summary>
/// <param name="handler">The on ssl error call back handler.</param>
void Client::SetOnSslErrorsHandler(OnSslErrorsHandler handler)
{
	_onSslErrorsHandler = handler;
}

/// <summary>
/// Set on socket error handler.
/// </summary>
/// <param name="handler">The on socket error call back handler.</param>
void Client::SetOnSocketErrorsHandler(OnSocketErrorsHandler handler)
{
	_onSocketErrorsHandler = handler;
}

/// <summary>
/// Set on pong handler.
/// </summary>
/// <param name="handler">The on pong call back handler.</param>
void Client::SetOnPongHandler(OnPongHandler handler)
{
	_onPongHandler = handler;
}

/// <summary>
/// Set on state changed handler.
/// </summary>
/// <param name="handler">The on state changed call back handler.</param>
void Client::SetOnStateChangedHandler(OnStateChangedHandler handler)
{
	_onStateChangedHandler = handler;
}

/// <summary>
/// Set on proxy authentication required handler.
/// </summary>
/// <param name="handler">The on proxy authentication required call back handler.</param>
void Client::SetOnProxyAuthenticationRequiredHandler(OnProxyAuthenticationRequiredHandler handler)
{
	_onProxyAuthenticationRequiredHandler = handler;
}

/// <summary>
/// Set on read channel finished handler.
/// </summary>
/// <param name="handler">The on read channel finished call back handler.</param>
void Client::SetOnReadChannelFinishedHandler(OnReadChannelFinishedHandler handler)
{
	_onReadChannelFinishedHandler = handler;
}

/// <summary>
/// Set on text frame received handler.
/// </summary>
/// <param name="handler">The on text frame received call back handler.</param>
void Client::SetOnTextFrameReceivedHandler(OnTextFrameReceivedHandler handler)
{
	_onTextFrameReceivedHandler = handler;
}

/// <summary>
/// Set on binary frame received handler.
/// </summary>
/// <param name="handler">The on binary frame received call back handler.</param>
void Client::SetOnBinaryFrameReceivedHandler(OnBinaryFrameReceivedHandler handler)
{
	_onBinaryFrameReceivedHandler = handler;
}

/// <summary>
/// On connected handle.
/// </summary>
void Client::OnConnectedHandle()
{
	connect(_qWebSocket.get(), &QWebSocket::textMessageReceived, this, &Client::OnTextMessageReceivedHandle);
	connect(_qWebSocket.get(), &QWebSocket::binaryMessageReceived, this, &Client::OnBinaryMessageReceivedHandle);
	connect(_qWebSocket.get(), &QWebSocket::readChannelFinished, this, &Client::OnReadChannelFinishedHandle);
	connect(_qWebSocket.get(), &QWebSocket::textFrameReceived, this, &Client::OnTextFrameReceivedHandle);
	connect(_qWebSocket.get(), &QWebSocket::binaryFrameReceived, this, &Client::OnBinaryFrameReceivedHandle);

	// Call the handler.
	_onConnectedHandler(this, _uniqueID);
}

/// <summary>
/// Close the connection.
/// </summary>
/// <param name="closeCode">The close code.</param>
/// <param name="reason">The close reason.</param>
void Client::Close(QWebSocketProtocol::CloseCode closeCode, const QString &reason)
{
	_closed = true;
	_qWebSocket->close(closeCode, reason);
}

/// <summary>
/// Abort communication.
/// </summary>
void Client::Abort()
{
	_qWebSocket->abort();
}

/// <summary>
/// Ignore ssl errors.
/// </summary>
void Client::IgnoreSslErrors()
{
	_qWebSocket->ignoreSslErrors();
}

/// <summary>
/// Ignore ssl errors.
/// </summary>
/// <param name="errors">The the list of errors to ignore.</param>
void Client::IgnoreSslErrors(const QList<QSslError> &errors)
{
	_qWebSocket->ignoreSslErrors(errors);
}

/// <summary>
/// Resume from pause.
/// </summary>
void Client::Resume()
{
	_qWebSocket->resume();
}

/// <summary>
/// Send text message.
/// </summary>
/// <param name="message">The text message.</param>
/// <returns>The number of bytes sent.</returns>
qint64 Client::SendTextMessage(const QString &message)
{
	return _qWebSocket->sendTextMessage(message);
}

/// <summary>
/// Send binary message.
/// </summary>
/// <param name="message">The binary message.</param>
/// <returns>The number of bytes sent.</returns>
qint64 Client::SendBinaryMessage(const QByteArray &message)
{
	return _qWebSocket->sendBinaryMessage(message);
}

/// <summary>
/// Send ping.
/// </summary>
/// <param name="payload">The payload.</param>
void Client::Ping(const QByteArray &payload)
{
	_qWebSocket->ping(payload);
}

/// <summary>
/// On disconnected handle.
/// </summary>
void Client::OnDisonnectedHandle()
{
	_onDisconnectedHandler(this, _uniqueID);
}

/// <summary>
/// On about to close handle.
/// </summary>
void Client::OnAboutToClose()
{
	_closed = true;
}

/// <summary>
/// On text message received handle.
/// </summary>
/// <param name="message">The text message.</param>
void Client::OnTextMessageReceivedHandle(QString message)
{
	_onTextMessageReceivedHandler(this, _uniqueID, message);
}

/// <summary>
/// On binary message received handle.
/// </summary>
/// <param name="message">The binary message.</param>
void Client::OnBinaryMessageReceivedHandle(QByteArray message)
{
	_onBinaryMessageReceivedHandler(this, _uniqueID, message);
}

/// <summary>
/// On ssl error handle.
/// </summary>
/// <param name="errors">The ssl error.</param>
void Client::OnSslErrorsHandle(const QList<QSslError> &errors)
{
	_onSslErrorsHandler(this, _uniqueID, errors);
}

/// <summary>
/// On socket error handle.
/// </summary>
/// <param name="errors">The socket error.</param>
void Client::OnSocketErrorHandle(QAbstractSocket::SocketError error)
{
	_onSocketErrorsHandler(this, _uniqueID, error);
}

/// <summary>
/// On pong handle.
/// </summary>
/// <param name="elapsedTime">The elapsed time.</param>
/// <param name="payload">The payload.</param>
void Client::OnPongHandle(quint64 elapsedTime, QByteArray payload)
{
	_onPongHandler(this, _uniqueID, elapsedTime, payload);
}

/// <summary>
/// On state changed handle.
/// </summary>
/// <param name="state">The new state.</param>
void Client::OnStateChangedHandle(QAbstractSocket::SocketState state)
{
	_onStateChangedHandler(this, _uniqueID, state);
}

/// <summary>
/// On proxy authentication required handle.
/// </summary>
/// <param name="proxy">The proxy.</param>
/// <param name="authenticator">The authenticator.</param>
void Client::OnProxyAuthenticationRequiredHandle(const QNetworkProxy &proxy, QAuthenticator *authenticator)
{
	_onProxyAuthenticationRequiredHandler(this, _uniqueID, proxy, authenticator);
}

/// <summary>
/// On read channel finished handle.
/// </summary>
void Client::OnReadChannelFinishedHandle()
{
	_onReadChannelFinishedHandler(this, _uniqueID);
}

/// <summary>
/// On text frame received handle.
/// </summary>
/// <param name="frame">The text frame.</param>
/// <param name="isLastFrame">Is this the last frame.</param>
void Client::OnTextFrameReceivedHandle(QString frame, bool isLastFrame)
{
	_onTextFrameReceivedHandler(this, _uniqueID, frame, isLastFrame);
}

/// <summary>
/// On binary frame received handle.
/// </summary>
/// <param name="frame">The binary frame.</param>
/// <param name="isLastFrame">Is this the last frame.</param>
void Client::OnBinaryFrameReceivedHandle(QByteArray frame, bool isLastFrame)
{
	_onBinaryFrameReceivedHandler(this, _uniqueID, frame, isLastFrame);
}

/// <summary>
/// Gets or sets the client unique id.
/// </summary>
void Client::SetUniqueID(const std::string &id)
{
	_uniqueID = id;
}
std::string Client::GetUniqueID() const
{
	return _uniqueID;
}

/// <summary>
/// Gets or sets the client ssl configuration.
/// </summary>
void Client::SetSslConfiguration(const QSslConfiguration &sslConfiguration)
{
	_sslConfiguration = sslConfiguration;
}
QSslConfiguration Client::GetSslConfiguration() const
{
	return _sslConfiguration;
}

/// <summary>
/// Gets the error string if any.
/// </summary>
QString Client::ErrorString() const
{
	return _qWebSocket->errorString();
}

/// <summary>
/// Gets the socket state.
/// </summary>
QAbstractSocket::SocketState Client::State() const
{
	return _qWebSocket->state();
}

/// <summary>
/// Gets the origin.
/// </summary>
QString Client::Origin() const
{
	return _qWebSocket->origin();
}

/// <summary>
/// Gets the close code.
/// </summary>
QWebSocketProtocol::CloseCode Client::CloseCode() const
{
	return _qWebSocket->closeCode();
}

/// <summary>
/// Gets the close reason.
/// </summary>
QString Client::CloseReason() const
{
	return _qWebSocket->closeReason();
}

/// <summary>
/// Gets the is valid indicator.
/// </summary>
bool Client::IsValid() const
{
	return _qWebSocket->isValid();
}

/// <summary>
/// Gets the local ip address.
/// </summary>
QHostAddress Client::LocalAddress() const
{
	return _qWebSocket->localAddress();
}

/// <summary>
/// Gets the local port.
/// </summary>
quint16 Client::LocalPort() const
{
	return _qWebSocket->localPort();
}

/// <summary>
/// Gets or sets the pause mode.
/// </summary>
void Client::SetPauseMode(QAbstractSocket::PauseModes pauseMode)
{
	_qWebSocket->setPauseMode(pauseMode);
}
QAbstractSocket::PauseModes Client::GetPauseMode() const
{
	return _qWebSocket->pauseMode();
}

/// <summary>
/// Gets the remote ip address.
/// </summary>
QHostAddress Client::PeerAddress() const
{
	return _qWebSocket->peerAddress();
}

/// <summary>
/// Gets the remote name.
/// </summary>
QString Client::PeerName() const
{
	return _qWebSocket->peerName();
}

/// <summary>
/// Gets the remote port.
/// </summary>
quint16 Client::PeerPort() const
{
	return _qWebSocket->peerPort();
}

/// <summary>
/// Gets or sets the read buffer size.
/// </summary>
void Client::SetReadBufferSize(qint64 size)
{
	_qWebSocket->setReadBufferSize(size);
}
qint64 Client::GetReadBufferSize() const
{
	return _qWebSocket->readBufferSize();
}

/// <summary>
/// Gets the resource name.
/// </summary>
QString Client::ResourceName() const
{
	return _qWebSocket->resourceName();
}

/// <summary>
/// Gets the request URL.
/// </summary>
QUrl Client::RequestUrl() const
{
	return _qWebSocket->requestUrl();
}

/// <summary>
/// Gets the request.
/// </summary>
QNetworkRequest Client::Request() const
{
	return _qWebSocket->request();
}

/// <summary>
/// Gets or sets the proxy.
/// </summary>
void Client::SetProxy(const QNetworkProxy &networkProxy)
{
	_qWebSocket->setProxy(networkProxy);
}
QNetworkProxy Client::GetProxy() const
{
	return _qWebSocket->proxy();
}