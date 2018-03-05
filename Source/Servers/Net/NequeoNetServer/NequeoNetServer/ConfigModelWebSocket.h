/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          ConfigModelWebSocket.h
*  Purpose :       Configuration Model class.
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

#include "NequeoWebSocketBoostServer\MultiWebServer.h"

namespace Nequeo {
	namespace Net {
		namespace WebSocket
		{
			/// <summary>
			/// Configuration model.
			/// </summary>
			class EXPORT_NEQUEO_NET_SERVER_API ConfigModelWebSocket
			{
			public:
				/// <summary>
				/// Configuration model.
				/// </summary>
				/// <param name="path">The path and file name of the configuration file (json format).</param>
				ConfigModelWebSocket(const std::string& path) : _disposed(false), _path(path), 
					_udpBroadcastEnabled(false), _udpBroadcastPort(0), _udpBroadcastCallbackPort(0), 
					_udpBroadcastAddress(""), _udpBroadcastMask(""), _accessTokenVerifyURL(""),
					_clientLocationRequestEnabled(false), _clientLocationRequestURL("") {}

				/// <summary>
				/// Configuration model.
				/// </summary>
				~ConfigModelWebSocket() {}

				/// <summary>
				/// Read the condifuration file contents.
				/// </summary>
				void ReadConfigFile();

				/// <summary>
				/// Get the muilti server contatiner list.
				/// </summary>
				/// <return>The muilti server contatiner list.</return>
				inline const std::vector<MultiServerContainer>& GetMultiServerContainer() const
				{
					return _containers;
				}

				/// <summary>
				/// Get the root path.
				/// </summary>
				/// <return>The root path.</return>
				inline const std::string& GetRootPath() const
				{
					return _rootPath;
				}

				/// <summary>
				/// Get the UDP broadcast port.
				/// </summary>
				/// <return>The UDP broadcast port.</return>
				inline const unsigned int GetUdpBroadcastPort() const
				{
					return _udpBroadcastPort;
				}

				/// <summary>
				/// Get the UDP broadcast callback port.
				/// </summary>
				/// <return>The UDP broadcast callback port.</return>
				inline const unsigned int GetUdpBroadcastCallbackPort() const
				{
					return _udpBroadcastCallbackPort;
				}

				/// <summary>
				/// Get the UDP broadcast enabled.
				/// </summary>
				/// <return>The UDP broadcast enabled.</return>
				inline const bool GetUdpBroadcastEnabled() const
				{
					return _udpBroadcastEnabled;
				}

				/// <summary>
				/// Get the broadcast address.
				/// </summary>
				/// <return>The broadcast address.</return>
				inline const std::string& GetUdpBroadcastAddress() const
				{
					return _udpBroadcastAddress;
				}

				/// <summary>
				/// Get the broadcast mask.
				/// </summary>
				/// <return>The broadcast mask.</return>
				inline const std::string& GetUdpBroadcastMask() const
				{
					return _udpBroadcastMask;
				}

				/// <summary>
				/// Get the access token verify URL.
				/// </summary>
				/// <return>The access token verify URL.</return>
				inline const std::string& GetAccessTokenVerifyURL() const
				{
					return _accessTokenVerifyURL;
				}

				/// <summary>
				/// Get the client location request enabled.
				/// </summary>
				/// <return>The client location request enabled.</return>
				inline const bool GetClientLocationRequestEnabled() const
				{
					return _clientLocationRequestEnabled;
				}

				/// <summary>
				/// Get the client location request URL.
				/// </summary>
				/// <return>The client location request URL.</return>
				inline const std::string& GetClientLocationRequestURL() const
				{
					return _clientLocationRequestURL;
				}

			private:
				bool _disposed;

				std::string _path;
				std::string _rootPath;

				bool _udpBroadcastEnabled;
				unsigned int _udpBroadcastPort;
				std::string _udpBroadcastAddress;
				std::string _udpBroadcastMask;
				unsigned int _udpBroadcastCallbackPort;

				bool _clientLocationRequestEnabled;
				std::string _accessTokenVerifyURL;
				std::string _clientLocationRequestURL;

				std::vector<MultiServerContainer> _containers;
			};
		}
	}
}