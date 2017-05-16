/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WebSocketServer.cpp
*  Purpose :       WebSocket web server class.
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
#include "Shlwapi.h"
#include <time.h>

#include "WebSocketServer.h"
#include "Base\StringEx.h"
#include "Base\StringUtils.h"

using namespace Nequeo::Net::WebSocket;

std::atomic<const char*> rootWsPath;
void OnWebSocketContext(const WebContext*);

/// <summary>
/// WebSocket web server.
/// </summary>
/// <param name="path">The root folder path (this is where all files are located).</param>
WebSocketServer::WebSocketServer(const std::string& path) :
	_disposed(false), _path(path), _initialised(false)
{
}

///	<summary>
///	WebSocket web server.
///	</summary>
WebSocketServer::~WebSocketServer()
{
	if (!_disposed)
	{
		_disposed = true;
		_initialised = false;
	}
}

/// <summary>
/// Initialise the servers.
/// </summary>
void WebSocketServer::Initialise()
{
	// If not initialised.
	if (!_initialised)
	{
		// Create the new array of servers.
		_servers = std::make_shared<MultiWebServer>(_containers);
		_servers->OnWebContext(OnWebSocketContext);
		_initialised = true;

		// Set the root path.
		rootWsPath = _path.c_str();
	}
}

///	<summary>
///	Start the servers.
///	</summary>
void WebSocketServer::Start()
{
	// If initialised.
	if (_initialised)
	{
		_servers->Start();
	}
}

///	<summary>
///	Stop the servers.
///	</summary>
void WebSocketServer::Stop()
{
	// If initialised.
	if (_initialised)
	{
		_servers->Stop();
	}
}

/// <summary>
/// On http context request.
/// </summary>
/// <param name="context">The web context.</param>
void OnWebSocketContext(const WebContext* context)
{
	// All requests will come into this function.
	try
	{
		// Get the request and response.
		WebRequest request = context->Request();

		// Cast away the const.
		WebContext* internalWebContext = const_cast<WebContext*>(context);
		
		// On close.
		internalWebContext->OnClose = [](int status, const std::string& reason)
		{
			
		};

		// On message.
		internalWebContext->OnMessage = [](MessageType messageType, size_t length, std::shared_ptr<Message>& message)
		{
			// If message
			if (length > 0)
			{
				// If message.
				if (messageType == MessageType::Binary || messageType == MessageType::Text)
				{
					// If text message.
					if (messageType == MessageType::Text)
					{
						std::string messageText = message->Get();

						// If requesting local time.
						if (messageText == "time")
						{
							// Time buffer
							struct tm newtime;
							char am_pm[] = "AM";
							__time64_t long_time;
							char timebuf[26];
							errno_t err;

							// Get time as 64-bit integer.  
							_time64(&long_time);

							// Convert to local time.  
							err = _localtime64_s(&newtime, &long_time);
							if (!err)
							{
								if (newtime.tm_hour > 12)					// Set up extension.   
									strcpy_s(am_pm, sizeof(am_pm), "PM");

								if (newtime.tm_hour > 12)					// Convert from 24-hour   
									newtime.tm_hour -= 12;					// to 12-hour clock.   

								if (newtime.tm_hour == 0)					// Set hour to 12 if midnight.  
									newtime.tm_hour = 12;

								// Convert to an ASCII representation.   
								err = asctime_s(timebuf, 26, &newtime);
							}

							if (!err)
							{
								// Time text.
								std::istringstream input(timebuf);
								message->Send(messageType, input.rdbuf());
							}
							else
							{
								// Echo text.
								std::istringstream input(messageText);
								message->Send(messageType, input.rdbuf());
							}
						}
						else
						{
							// Echo text.
							std::istringstream input(messageText);
							message->Send(messageType, input.rdbuf());
						}
					}

					// If binary message.
					if (messageType == MessageType::Binary)
					{
						// Echo binary.
						std::istream input(message->Received);
						message->Send(messageType, input.rdbuf());
					}
				}
				else
				{
					// Send back message type.
					if (messageType == MessageType::Close)
					{
						std::istringstream input("Close");
						message->Send(messageType, input.rdbuf());
					}

					// Send back message type.
					if (messageType == MessageType::Ping)
					{
						std::istringstream input("Ping");
						message->Send(messageType, input.rdbuf());
					}
				}
			}
		};

		// On error.
		internalWebContext->OnError = [](const std::string& error)
		{
			
		};
	}
	catch (...) {}
}