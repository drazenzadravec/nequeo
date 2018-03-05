/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          UdpBroadcastServer.h
*  Purpose :       Udp Broadcast Server class.
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

namespace Nequeo {
	namespace Net {
		namespace UDP
		{
			class UdpServer;
			class UdpSenderEndpoint;

			typedef std::function<void(const std::string&, const std::string&, unsigned int)> UdpBroadcastReceiveHandler;

			/// <summary>
			///  UDP Broadcast web server.
			/// </summary>
			class EXPORT_NEQUEO_NET_SERVER_API UdpBroadcastServer
			{
			public:
				/// <summary>
				/// UDP Broadcast web server.
				/// </summary>
				/// <param name="port">The UDP listening port.</param>
				UdpBroadcastServer(unsigned int port);

				/// <summary>
				/// UDP Broadcast web server.
				/// </summary>
				~UdpBroadcastServer();

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

				/// <summary>
				/// On received data.
				/// </summary>
				/// <param name="received">The received data handler.</param>
				void OnReceivedData(const UdpBroadcastReceiveHandler& received);

				/// <summary>
				/// Send data to sender endpoint.
				/// </summary>
				/// <param name="data">The data to send.</param>
				/// <param name="senderAddress">The sender address.</param>
				/// <param name="senderPort">The sender port.</param>
				void SendTo(const std::string& data, const std::string& senderAddress, unsigned int senderPort);

				/// <summary>
				/// Send no response to complete the async call.
				/// </summary>
				/// <param name="senderAddress">The sender address.</param>
				void SendTo(const std::string& senderAddress);

				///	<summary>
				///	Start IPv4 server.
				///	</summary>
				void StartServerV4();

				///	<summary>
				///	Start IPv6 server.
				///	</summary>
				void StartServerV6();

			private:
				std::set<std::shared_ptr<UdpSenderEndpoint>> GetSenderEndpoints();

			private:
				bool _disposed;
				bool _initialised;
				std::thread _threadV4;
				std::thread _threadV6;

				unsigned int _port;
				std::set<std::shared_ptr<UdpSenderEndpoint>> _senderEndpoints;

				std::shared_ptr<UdpServer> _udpServerV4;
				std::shared_ptr<UdpServer> _udpServerV6;

				std::mutex _endpointMutex;
				UdpBroadcastReceiveHandler _onReceivedData;
			};
		}
	}
}