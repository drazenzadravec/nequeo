/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WebSocketServer.h
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
#include "GlobalNetServer.h"
#include "UdpBroadcastServer.h"
#include "UdpBroadcastClient.h"

#include "NequeoWebSocketBoostServer\MultiWebServer.h"

#include "Threading\Executor.h"
#include "Threading\ThreadTask.h"
#include "Base\AsyncCallerContext.h"

using namespace Nequeo::Net::UDP;

namespace Nequeo {
	namespace Net {
		namespace WebSocket
		{
			/// <summary>
			/// WebSocket web server.
			/// </summary>
			class EXPORT_NEQUEO_NET_SERVER_API WebSocketServer
			{
			public:
				/// <summary>
				/// WebSocket web server.
				/// </summary>
				/// <param name="path">The root folder path (this is where all files are located).</param>
				WebSocketServer(const std::string& path);

				/// <summary>
				/// WebSocket web server.
				/// </summary>
				~WebSocketServer();

				/// <summary>
				/// Set the muilti server contatiner list.
				/// </summary>
				/// <param name="path">The muilti server contatiner list.</param>
				inline void SetMultiServerContainer(const std::vector<MultiServerContainer>& containers)
				{
					_containers = containers;
				}

				/// <summary>
				/// Set the UDP broadcast details.
				/// </summary>
				/// <param name="udpBroadcastEnabled">The UDP broadcast enabled.</param>
				/// <param name="udpBroadcastPort">The UDP broadcast port.</param>
				/// <param name="udpBroadcastCallbackPort">The UDP broadcast callback port.</param>
				/// <param name="udpBroadcastAddress">The UDP broadcast address.</param>
				/// <param name="udpBroadcastMask">The UDP broadcast mask.</param>
				inline void SetUdpBroadcast(
					bool udpBroadcastEnabled, 
					unsigned int udpBroadcastPort, 
					unsigned int udpBroadcastCallbackPort,
					const std::string& udpBroadcastAddress,
					const std::string& udpBroadcastMask)
				{
					_udpBroadcastEnabled = udpBroadcastEnabled;
					_udpBroadcastPort = udpBroadcastPort;
					_udpBroadcastCallbackPort = udpBroadcastCallbackPort;
					_udpBroadcastAddress = udpBroadcastAddress;
					_udpBroadcastMask = udpBroadcastMask;
				}

				/// <summary>
				/// Set the access token verify URL and client location request URL.
				/// </summary>
				/// <param name="accessTokenVerifyURL">The access token verify URL.</param>
				/// <param name="clientLocationRequestURL">The client location request URL.</param>
				/// <param name="clientLocationRequestEnabled">The client location request enabled.</param>
				inline void SetURL(
					const std::string& accessTokenVerifyURL,
					const std::string& clientLocationRequestURL,
					bool clientLocationRequestEnabled)
				{
					_accessTokenVerifyURL = accessTokenVerifyURL;
					_clientLocationRequestURL = clientLocationRequestURL;
					_clientLocationRequestEnabled = clientLocationRequestEnabled;
				}

				/// <summary>
				/// Is initialise.
				/// </summary>
				/// <return>True if initialise; else false.</return>
				inline bool IsInitialise() const
				{
					return _initialised;
				}

				/// <summary>
				/// Initialise the servers.
				/// </summary>
				void Initialise();

				///	<summary>
				///	Start the servers.
				///	</summary>
				void Start();

				///	<summary>
				///	Stop the servers.
				///	</summary>
				void Stop();

			private:
				void OnWebSocketContext(const WebContext*);
				void OnUdpReceiveData(const std::string& data, const std::string& ipAddressSender, unsigned int portSender);
				void OnUdpReceiveDataCallback(const std::string& data, const std::string& ipAddressSender, unsigned int portSender);
				void SendCallbackDataAsync(const std::string& data, const std::string& host, unsigned int port);
				void BroadcastDataAsync(
					const std::string& data, unsigned int port, const std::string& address, const std::string& mask, 
					bool udpBroadcastEnabled, bool clientLocationRequestEnabled, const std::string& clientLocationRequestURL,
					const std::string& clientToken, const std::string& contactUniqueID);

			private:
				bool _disposed;
				bool _initialised;

				std::string _path;
				std::string _serverID;
				std::shared_ptr<MultiWebServer> _servers;
				std::vector<MultiServerContainer> _containers;
				std::shared_ptr<UdpBroadcastServer> _udpServer;
				std::shared_ptr<UdpBroadcastServer> _udpServerCallback;

				std::shared_ptr<Nequeo::Threading::Executor> _executor;

				bool _udpBroadcastEnabled;
				unsigned int _udpBroadcastPort;
				unsigned int _udpBroadcastCallbackPort;
				std::string _udpBroadcastAddress;
				std::string _udpBroadcastMask;

				bool _clientLocationRequestEnabled;
				std::string _accessTokenVerifyURL;
				std::string _clientLocationRequestURL;
			};
		}
	}
}