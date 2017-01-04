/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          ProxyServer.cpp
*  Purpose :       Proxy Server class.
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

#include "ProxyServer.h"

using namespace Live::Media;

/// <summary>
/// Live media proxy server.
/// </summary>
ProxyServer::ProxyServer() :
	_scheduler(nullptr),
	_env(nullptr),
	_rtspServer(nullptr),
	_authDB(nullptr),
	_authDBRegister(nullptr),
	_rtspServerPortNumber(554),
	_upTunnelingOverHttpPortNumber(80),
	_verbosityLevel(1),
	_streamRTPOverTCP(false),
	_proxyREGISTERRequests(false),
	_initialised(false),
	_started(false),
	_disposed(false)
{
	// Increase the maximum size of video frames that we can 'proxy' without truncation.
	// (Such frames are unreasonably large; the back-end servers should really not be sending frames this large!)
	OutPacketBuffer::maxSize = 100000;

	// Begin by setting up our usage environment:
	_scheduler = std::unique_ptr<TaskScheduler>(BasicTaskScheduler::createNew());
	_env = std::unique_ptr<BasicUsageEnvironment>(BasicUsageEnvironment::createNew(*_scheduler));
}

/// <summary>
/// This destructor. Call release to cleanup resources.
/// </summary>
ProxyServer::~ProxyServer()
{
	if (!_disposed)
	{
		_disposed = true;
		_initialised = false;
		_started = false;

		_env = nullptr;
		_authDB = nullptr;
		_authDBRegister = nullptr;
		_scheduler = nullptr;
		_rtspServer = nullptr;
	}
}

/// <summary>
/// Add a user.
/// </summary>
/// <param name="username">The user username.</param>
/// <param name="password">The user password.</param>
void ProxyServer::AddUser(char const* username, char const* password)
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
void ProxyServer::RemoveUser(const char* username)
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
void ProxyServer::SetRtspServerPortNumber(unsigned int port)
{
	_rtspServerPortNumber = port;
}
unsigned int ProxyServer::GetRtspServerPortNumber() const
{
	return _rtspServerPortNumber;
}

/// <summary>
/// Gets or sets the up tunneling over http port number.
/// </summary>
void ProxyServer::SetUpTunnelingOverHttpPortNumber(unsigned int port)
{
	_upTunnelingOverHttpPortNumber = port;
}
unsigned int ProxyServer::GetUpTunnelingOverHttpPortNumber() const
{
	return _upTunnelingOverHttpPortNumber;
}

/// <summary>
/// Gets or sets the _verbosity level (1, 2).
/// </summary>
void ProxyServer::SetVerbosityLevel(int level)
{
	_verbosityLevel = level;
}
int ProxyServer::GetVerbosityLevel() const
{
	return _verbosityLevel;
}

/// <summary>
/// Gets or sets the stream RTP and RTCP over the TCP 'control' connection. (This is for the 'back end' (i.e., proxied) stream only.)
/// </summary>
void ProxyServer::SetStreamRTPOverTCP(bool streamed)
{
	_streamRTPOverTCP = streamed;
}
bool ProxyServer::GetStreamRTPOverTCP() const
{
	return _streamRTPOverTCP;
}

/// <summary>
/// Gets or sets handle incoming "REGISTER" requests by proxying the specified stream:
/// </summary>
/// <param name="registerRequest">True to register request; else false.</param>
/// <param name="username">The register request username.</param>
/// <param name="password">The register request password.</param>
void ProxyServer::SetProxyREGISTERRequests(bool registerRequest, const char* username, const char* password)
{
	_proxyREGISTERRequests = registerRequest;

	// If not exists.
	if (_authDBRegister == nullptr)
	{
		// Create the auth DB.
		_authDBRegister = std::unique_ptr<UserAuthenticationDatabase>(new UserAuthenticationDatabase());
	}

	// If not exists.
	if (_authDBRegister != nullptr)
	{
		// If true.
		if (_proxyREGISTERRequests)
		{
			// Add the user.
			_authDBRegister->addUserRecord(username, password);
		}
		else
		{
			// Remove the user.
			_authDBRegister->removeUserRecord(username);
		}
	}
}
bool ProxyServer::GetProxyREGISTERRequests() const
{
	return _proxyREGISTERRequests;
}

/// <summary>
/// Sets the RTSP server URLs, used by the proxy server.
/// </summary>
/// <param name="urls">The collection of URLs.</param>
void ProxyServer::SetRtspServerURLs(std::vector<RtspServerURLs> urls)
{
	_rtspServerURLs = urls;
}

/// <summary>
/// Has the server been initialised.
/// </summary>
/// <returns>True if initialised; else false.</returns>
bool ProxyServer::IsInitialised()
{
	return _initialised;
}

/// <summary>
/// Has the server been started.
/// </summary>
/// <returns>True if started; else false.</returns>
bool ProxyServer::HasStarted()
{
	return _started;
}

/// <summary>
/// Initialise the server.
/// </summary>
void ProxyServer::Initialise()
{
	// If not initialised.
	if (!_initialised)
	{
		// If not created.
		if (_rtspServer == nullptr)
		{
			// If proxy register.
			if (_proxyREGISTERRequests)
			{
				// Create the server.
				_rtspServer = std::unique_ptr<RTSPServer>(RTSPServerWithREGISTERProxying::createNew(
					*_env, (portNumBits)_rtspServerPortNumber, _authDB.get(), _authDBRegister.get(), 65, _streamRTPOverTCP, _verbosityLevel));
			}
			else
			{
				// Create the server.
				_rtspServer = std::unique_ptr<RTSPServer>(RTSPServer::createNew(*_env, (portNumBits)_rtspServerPortNumber, _authDB.get()));
			}
			
			// If not created.
			if (_rtspServer == nullptr)
				_initialised = false;
			else
			{
				// get the vector url size.
				size_t vectorSize = _rtspServerURLs.size();

				// If devices exist.
				if (vectorSize > 0)
				{
					// For each server found.
					for (int i = 0; i < vectorSize; i++)
					{
						// Create each session.
						RtspServerURLs serverURL = _rtspServerURLs[i];

						// Create a session
						ServerMediaSession* sms = ProxyServerMediaSession::createNew(*_env, _rtspServer.get(), serverURL.URL, 
							serverURL.StreamName, serverURL.Username, serverURL.Password, 
							(portNumBits)serverURL.UpTunnelingOverHttpPortNumber, serverURL.VerbosityLevel);

						// Add the server session.
						_rtspServer->addServerMediaSession(sms);

						// Add the server session.
						_rtspServerSession.push_back(std::shared_ptr<ServerMediaSession>(sms));
					}

					// Setup the server for optional RTSP-over-HTTP tunneling,
					// or for HTTP live streaming (for indexed Transport Stream files only).
					_rtspServer->setUpTunnelingOverHTTP((portNumBits)_upTunnelingOverHttpPortNumber);

					_initialised = true;
				}
			}
		}
	}
}

/// <summary>
/// Start the server.
/// </summary>
void ProxyServer::Start()
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
			_thread = std::thread(std::bind(&ProxyServer::StartThread, this));
			_thread.detach();
		}
	}
}

/// <summary>
/// Stop the server.
/// </summary>
void ProxyServer::Stop()
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
void ProxyServer::StartThread()
{
	_env->taskScheduler().doEventLoop();
}