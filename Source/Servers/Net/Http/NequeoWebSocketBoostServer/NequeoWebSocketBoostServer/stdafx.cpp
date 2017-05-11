// stdafx.cpp : source file that includes just the standard includes
// NequeoWebSocketBoostServer.pch will be the pre-compiled header
// stdafx.obj will contain the pre-compiled type information

#include "stdafx.h"

#include "server_ws.hpp"
#include "server_wss.hpp"
#include "client_ws.hpp"
#include "client_wss.hpp"

typedef Nequeo::Net::WebSocket::Boost::SocketServer<Nequeo::Net::WebSocket::Boost::WS> InternalWebSocketServer;
typedef Nequeo::Net::WebSocket::Boost::SocketServer<Nequeo::Net::WebSocket::Boost::WSS> InternalSecureWebSocketServer;

typedef Nequeo::Net::WebSocket::Boost::SocketClient<Nequeo::Net::WebSocket::Boost::WS> InternalWebSocketClient;
typedef Nequeo::Net::WebSocket::Boost::SocketClient<Nequeo::Net::WebSocket::Boost::WSS> InternalSecureWebSocketClient;
