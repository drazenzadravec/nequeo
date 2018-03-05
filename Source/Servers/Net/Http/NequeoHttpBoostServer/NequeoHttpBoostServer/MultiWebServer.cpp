/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MultiWebServer.cpp
*  Purpose :       Http net context class.
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

#include "MultiWebServer.h"

using namespace Nequeo::Net::Http;

/// <summary>
/// Http multi web server.
/// </summary>
/// <param name="containers">The list of server arguments.</param>
MultiWebServer::MultiWebServer(std::vector<MultiServerContainer>& containers) :
	_disposed(false), _containers(containers), _started(false)
{
	// Get the vector size.
	size_t vectorSize = containers.size();

	// If servers exist.
	if (vectorSize > 0)
	{
		// For each server found.
		for (int i = 0; i < vectorSize; i++)
		{
			// Get the container.
			MultiServerContainer container = containers[i];

			// If endpoint.
			if (container.GetEndpoint().length() > 0)
			{
				// Construct the endpoint server.
				std::shared_ptr<WebServer> serverEndpoint = std::make_shared<WebServer>(
					container.GetPort(), container.GetEndpoint(), container.GetIsSecure(), 
					container.GetTimeoutRequest(), container.GetTimeoutContent(),
					container.GetNumberOfThreads());

				// If secure.
				if (serverEndpoint->IsSecure())
				{
					// Set the public and private keys.
					serverEndpoint->SetSecurePublicPrivateKeys(
						container.GetPublicKeyFile(), container.GetPrivateKeyFile(), container.GetPrivateKeyPassword());
				}

				// Add the server.
				_webServers.push_back(serverEndpoint);
			}
			else
			{
				// Construct the ipv server.
				std::shared_ptr<WebServer> serverIPversion = std::make_shared<WebServer>(
					container.GetPort(), container.GetIPversion(), container.GetIsSecure(),
					container.GetTimeoutRequest(), container.GetTimeoutContent(),
					container.GetNumberOfThreads());

				// If secure.
				if (serverIPversion->IsSecure())
				{
					// Set the public and private keys.
					serverIPversion->SetSecurePublicPrivateKeys(
						container.GetPublicKeyFile(), container.GetPrivateKeyFile(), container.GetPrivateKeyPassword());
				}

				// Add the server.
				_webServers.push_back(serverIPversion);
			}
		}
	}
}

/// <summary>
/// Http multi web server.
/// </summary>
MultiWebServer::~MultiWebServer()
{
	if (!_disposed)
	{
		_disposed = true;
		_started = false;
	}
}

/// <summary>
/// On web context request.
/// </summary>
/// <param name="webContext">The web context callback function.</param>
void MultiWebServer::OnWebContext(const WebContextHandler& webContext)
{
	_onWebContext = webContext;
}

///	<summary>
///	Start the servers.
///	</summary>
void MultiWebServer::Start()
{
	// If not stared.
	if (!_started)
	{
		// Get the vector size.
		size_t vectorSize = _webServers.size();

		// If servers exist.
		if (vectorSize > 0)
		{
			// For each server found.
			for (int i = 0; i < vectorSize; i++)
			{
				try
				{
					// Get the server.
					WebServer* server = _webServers[i].get();
					server->OnWebContext(_onWebContext);

					// Start a new thread.
					server->StartThread();
				}
				catch (...) {}
			}
			_started = true;
		}
	}
}

///	<summary>
///	Stop the servers.
///	</summary>
void MultiWebServer::Stop()
{
	// If stared.
	if (_started)
	{
		// Get the vector size.
		size_t vectorSize = _webServers.size();

		// If servers exist.
		if (vectorSize > 0)
		{
			// For each server found.
			for (int i = 0; i < vectorSize; i++)
			{
				try
				{
					// Get the server.
					WebServer* server = _webServers[i].get();
					server->StopThread();
				}
				catch (...) {}
			}
			_started = false;
		}
	}
}

/// <summary>
/// Get the web server list.
/// </summary>
/// <return>The web servers.</return>
const std::vector<std::shared_ptr<WebServer>>& MultiWebServer::GetWebServers() const
{
	return _webServers;
}