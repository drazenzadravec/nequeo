/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          UdpBroadcastClient.h
*  Purpose :       Udp Broadcast Client class.
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
#include "NequeoWebSocketBoostServer\IPVersionType.h"

namespace Nequeo {
	namespace Net {
		namespace UDP
		{
			class UdpClient;
			class UdpSenderEndpoint;

			typedef std::function<void(const std::string&, const std::string&, unsigned int)> UdpBroadcastReceiveHandler;

			/// <summary>
			///  UDP Broadcast web client.
			/// </summary>
			class EXPORT_NEQUEO_NET_SERVER_API UdpBroadcastClient
			{
			public:
				/// <summary>
				/// UDP Broadcast web client.
				/// </summary>
				/// <param name="port">The UDP listening port.</param>
				UdpBroadcastClient(unsigned int port);

				/// <summary>
				/// UDP Broadcast web client.
				/// </summary>
				~UdpBroadcastClient();

				/// <summary>
				/// Is initialise.
				/// </summary>
				/// <return>True if initialise; else false.</return>
				inline bool IsInitialise() const
				{
					return _initialised;
				}

				/// <summary>
				/// Initialise the clients.
				/// </summary>
				void Initialise();

				/// <summary>
				/// Initialise the clients for direct call methods.
				/// </summary>
				void InitialiseDirect();

				/// <summary>
				/// On received data.
				/// </summary>
				/// <param name="received">The received data handler.</param>
				void OnReceivedData(const UdpBroadcastReceiveHandler& received);

				/// <summary>
				/// Broadcast data on the port (only IPv4).
				/// </summary>
				/// <param name="data">The data to send.</param>
				/// <param name="port">The port to broadcast on.</param>
				void Broadcast(const std::string& data, unsigned int port);

				/// <summary>
				/// Send data to host endpoint.
				/// </summary>
				/// <param name="data">The data to send.</param>
				/// <param name="host">The host address.</param>
				/// <param name="port">The host port.</param>
				void SendToConnection(const std::string& data, const std::string& host, unsigned int port);

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
				///	Open the connection.
				///	</summary>
				void Open();

				///	<summary>
				///	Close the connection.
				///	</summary>
				void Close();

				/// <summary>
				/// Send data to the endpoint created after calling connect.
				/// </summary>
				/// <param name="data">The data to send.</param>
				/// <param name="host">The UDP host name or IP.</param>
				/// <param name="port">The UDP host port.</param>
				void SendToDirect(const std::string& data, const std::string& host, unsigned int port);

				/// <summary>
				/// Broadcast data on the port (only IPv4).
				/// </summary>
				/// <param name="data">The data to send.</param>
				/// <param name="port">The port to broadcast on.</param>
				void SendToBroadcastDirect(const std::string& data, unsigned int port);

				/// <summary>
				/// Broadcast data on the port (only IPv4).
				/// </summary>
				/// <param name="data">The data to send.</param>
				/// <param name="port">The port to broadcast on.</param>
				/// <param name="ip">The port to broadcast on.</param>
				/// <param name="ipMask">The port to broadcast on.</param>
				void SendToBroadcastDirect(const std::string& data, unsigned int port, const std::string& ip, const std::string& ipMask);

			private:
				std::set<std::shared_ptr<UdpSenderEndpoint>> GetSenderEndpoints();

			private:
				bool _disposed;
				bool _initialised;

				unsigned int _port;
				std::set<std::shared_ptr<UdpSenderEndpoint>> _senderEndpoints;

				std::shared_ptr<UdpClient> _udpClientV4;
				std::shared_ptr<UdpClient> _udpClientV6;

				std::mutex _endpointMutex;
				UdpBroadcastReceiveHandler _onReceivedData;
			};
		}
	}
}