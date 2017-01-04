/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MediaServer.cpp
*  Purpose :       Media Server class.
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

#include "MediaServer.h"

using namespace Live::Media;

/// <summary>
/// Live media server.
/// </summary>
MediaServer::MediaServer() :
	_scheduler(nullptr),
	_env(nullptr),
	_rtspServer(nullptr),
	_authDB(nullptr),
	_rtspServerPortNumber(554),
	_upTunnelingOverHttpPortNumber(80),
	_initialised(false),
	_started(false),
	_disposed(false)
{
	// Begin by setting up our usage environment:
	_scheduler = std::unique_ptr<TaskScheduler>(BasicTaskScheduler::createNew());
	_env = std::unique_ptr<BasicUsageEnvironment>(BasicUsageEnvironment::createNew(*_scheduler));
}

/// <summary>
/// This destructor. Call release to cleanup resources.
/// </summary>
MediaServer::~MediaServer()
{
	if (!_disposed)
	{
		_disposed = true;
		_initialised = false;
		_started = false;

		_env = nullptr;
		_authDB = nullptr;
		_scheduler = nullptr;
		_rtspServer = nullptr;
	}
}

/// <summary>
/// Add a user.
/// </summary>
/// <param name="username">The user username.</param>
/// <param name="password">The user password.</param>
void MediaServer::AddUser(char const* username, char const* password)
{
	// If not exists.
	if (_authDB == nullptr)
	{
		// Create the auth DB.
		_authDB = std::unique_ptr<UserAuthenticationDatabase>(new UserAuthenticationDatabase());
	}
	
	// If exists.
	if (_authDB != nullptr)
	{
		// Add the user.
		_authDB->addUserRecord(username, password);
	}
}

/// <summary>
/// Remove a user.
/// </summary>
/// <param name="username">The user username.</param>
void MediaServer::RemoveUser(const char* username)
{
	// If exists.
	if (_authDB != nullptr)
	{
		// Remove the user.
		_authDB->removeUserRecord(username);
	}
}

/// <summary>
/// Gets or sets the rtsp server port number.
/// </summary>
void MediaServer::SetRtspServerPortNumber(unsigned int port)
{
	_rtspServerPortNumber = port;
}
unsigned int MediaServer::GetRtspServerPortNumber() const
{
	return _rtspServerPortNumber;
}

/// <summary>
/// Gets or sets the up tunneling over http port number.
/// </summary>
void MediaServer::SetUpTunnelingOverHttpPortNumber(unsigned int port)
{
	_upTunnelingOverHttpPortNumber = port;
}
unsigned int MediaServer::GetUpTunnelingOverHttpPortNumber() const
{
	return _upTunnelingOverHttpPortNumber;
}

/// <summary>
/// Has the server been initialised.
/// </summary>
/// <returns>True if initialised; else false.</returns>
bool MediaServer::IsInitialised()
{
	return _initialised;
}

/// <summary>
/// Has the server been started.
/// </summary>
/// <returns>True if started; else false.</returns>
bool MediaServer::HasStarted()
{
	return _started;
}

/// <summary>
/// Initialise the server.
/// </summary>
void MediaServer::Initialise()
{
	// If not initialised.
	if (!_initialised)
	{
		// If not created.
		if (_rtspServer == nullptr)
		{
			// Create the server.
			_rtspServer = std::unique_ptr<DynamicRTSPServer>(DynamicRTSPServer::createNew(*_env, (portNumBits)_rtspServerPortNumber, _authDB.get()));

			// If not created.
			if (_rtspServer == nullptr)
				_initialised = false;
			else
			{
				// Setup the server for optional RTSP-over-HTTP tunneling,
				// or for HTTP live streaming (for indexed Transport Stream files only).
				_rtspServer->setUpTunnelingOverHTTP((portNumBits)_upTunnelingOverHttpPortNumber);

				_initialised = true;

			}
		}
	}
}

/// <summary>
/// Start the server.
/// </summary>
void MediaServer::Start()
{
	// If initialised.
	if (_initialised)
	{
		// If not started.
		if (!_started)
		{
			// Start the event loop.
			_env->taskScheduler().stopEventLoop(false);
			_started = true;

			// Start a new thread.
			_thread = std::thread(std::bind(&MediaServer::StartThread, this));
			_thread.detach();
		}
	}
}

/// <summary>
/// Stop the server.
/// </summary>
void MediaServer::Stop()
{
	// If initialised.
	if (_initialised)
	{
		// If started.
		if (_started)
		{
			// Stop the event loop.
			_env->taskScheduler().stopEventLoop(true);
			_started = false;

			try
			{
				_thread.join();
			}
			catch (...) {}
		}
	}
}

/// <summary>
/// Start the server thread.
/// </summary>
void MediaServer::StartThread()
{
	_env->taskScheduler().doEventLoop();
}