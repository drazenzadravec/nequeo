/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          UdpBroadcastClient.cpp
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
#include "Shlwapi.h"
#include <time.h>

#include "UdpClient.cpp"
#include "UdpBroadcastClient.h"
#include "Base\StringEx.h"
#include "Base\StringUtils.h"

#include <boost/asio.hpp>

using boost::asio::ip::udp;
using namespace Nequeo::Net::UDP;
using namespace Nequeo::Net::WebSocket;

/// <summary>
/// UDP Broadcast web client.
/// </summary>
/// <param name="port">The UDP listening port.</param>
UdpBroadcastClient::UdpBroadcastClient(unsigned int port) :
	_disposed(false), _port(port), _initialised(false)
{
}

/// <summary>
/// UDP Broadcast web client.
/// </summary>
UdpBroadcastClient::~UdpBroadcastClient()
{
	if (!_disposed)
	{
		_disposed = true;
		_initialised = false;
	}
}

/// <summary>
/// Initialise the clients.
/// </summary>
void UdpBroadcastClient::Initialise()
{
	// If not initialised.
	if (!_initialised)
	{
		// Create the new array of clients.
		_udpClientV4 = std::make_shared<UdpClient>(_port, udp::v4());
		_udpClientV6 = std::make_shared<UdpClient>(_port, udp::v6());

		// Initialise the server.
		_udpClientV4->OnReceivedData([this](const std::string& data, const udp::endpoint& sender)
		{
			// Create a new endpint.
			auto endpoint = std::make_shared<UdpSenderEndpoint>();
			endpoint->senderAddress = sender.address().to_string();
			endpoint->senderEndpoint = sender;
			endpoint->senderPort = sender.port();

			// Add to collection.
			_endpointMutex.lock();
			_senderEndpoints.insert(endpoint);
			_endpointMutex.unlock();

			// Call receive handler.
			_onReceivedData(data, endpoint->senderAddress, endpoint->senderPort);

			// Remove the endpoint.
			_endpointMutex.lock();
			_senderEndpoints.erase(endpoint);
			_endpointMutex.unlock();

		});
		_udpClientV4->Initialise();

		// Initialise the server.
		_udpClientV6->OnReceivedData([this](const std::string& data, const udp::endpoint& sender)
		{
			// Create a new endpint.
			auto endpoint = std::make_shared<UdpSenderEndpoint>();
			endpoint->senderAddress = sender.address().to_string();
			endpoint->senderEndpoint = sender;
			endpoint->senderPort = sender.port();

			// Add to collection.
			_endpointMutex.lock();
			_senderEndpoints.insert(endpoint);
			_endpointMutex.unlock();

			// Call receive handler.
			_onReceivedData(data, endpoint->senderAddress, endpoint->senderPort);

			// Remove the endpoint.
			_endpointMutex.lock();
			_senderEndpoints.erase(endpoint);
			_endpointMutex.unlock();

		});
		_udpClientV6->Initialise();
		_initialised = true;
	}
}

/// <summary>
/// Initialise the clients for direct call methods.
/// </summary>
void UdpBroadcastClient::InitialiseDirect()
{
	// If not initialised.
	if (!_initialised)
	{
		// Create the new array of clients.
		_udpClientV4 = std::make_shared<UdpClient>(_port, udp::v4());
		_udpClientV6 = std::make_shared<UdpClient>(_port, udp::v6());

		_initialised = true;
	}
}

/// <summary>
/// Get a copy of all sender endpoints.
/// </summary>
/// <return>The collection of sender endpoints.</return>
std::set<std::shared_ptr<UdpSenderEndpoint>> UdpBroadcastClient::GetSenderEndpoints()
{
	// Return a copy of all sender endpoints.
	_endpointMutex.lock();
	auto copy = _senderEndpoints;
	_endpointMutex.unlock();
	return copy;
}

/// <summary>
/// On received data.
/// </summary>
/// <param name="received">The received data handler.</param>
void UdpBroadcastClient::OnReceivedData(const UdpBroadcastReceiveHandler& received)
{
	_onReceivedData = received;
}

/// <summary>
/// Broadcast data on the port (only IPv4).
/// </summary>
/// <param name="data">The data to send.</param>
/// <param name="port">The port to broadcast on.</param>
void UdpBroadcastClient::Broadcast(const std::string& data, unsigned int port)
{
	// If initialised.
	if (_initialised)
	{
		// Broadcast on IPv4 subnet.
		_udpClientV4->SendToBroadcast(data, port);
	}
}

/// <summary>
/// Send data to host endpoint.
/// </summary>
/// <param name="data">The data to send.</param>
/// <param name="host">The host address.</param>
/// <param name="port">The host port.</param>
void UdpBroadcastClient::SendToConnection(const std::string& data, const std::string& host, unsigned int port)
{
	// If initialised.
	if (_initialised)
	{
		try
		{
			// Create the sender end point.
			boost::asio::ip::address addr4(boost::asio::ip::address_v4::from_string(host));

			// If IPv4.
			if (addr4.is_v4())
			{
				// Send the data.
				_udpClientV4->Connect(host, port);
				_udpClientV4->SendToConnect(data);
			}
		}
		catch (const std::exception&) {}
		
		try
		{
			// Resolver the host.
			boost::asio::ip::address addr6(boost::asio::ip::address_v6::from_string(host));

			// If IPv6.
			if (addr6.is_v6())
			{
				// Send the data.
				_udpClientV6->Connect(host, port);
				_udpClientV6->SendToConnect(data);
			}
		}
		catch (const std::exception&) {}
	}
}

/// <summary>
/// Send data to sender endpoint.
/// </summary>
/// <param name="data">The data to send.</param>
/// <param name="senderAddress">The sender address.</param>
/// <param name="senderPort">The sender port.</param>
void UdpBroadcastClient::SendTo(const std::string& data, const std::string& senderAddress, unsigned int senderPort)
{
	// If initialised.
	if (_initialised)
	{
		// For each endpoint.
		for (auto endpoint : GetSenderEndpoints())
		{
			// Get the sender details.
			std::string address = endpoint->senderAddress;
			unsigned int port = endpoint->senderPort;

			// Find the sender.
			if (address == senderAddress && port == senderPort)
			{
				try
				{
					// Create the sender end point.
					boost::asio::ip::address addr4(boost::asio::ip::address_v4::from_string(senderAddress));

					// If IPv4.
					if (addr4.is_v4())
					{
						// Send the data.
						_udpClientV4->SendToSender(data, endpoint->senderEndpoint);
					}
				}
				catch (const std::exception&) {}
				
				try
				{
					// Resolver the host.
					boost::asio::ip::address addr6(boost::asio::ip::address_v6::from_string(senderAddress));

					// If IPv6.
					if (addr6.is_v6())
					{
						// Send the data.
						_udpClientV6->SendToSender(data, endpoint->senderEndpoint);
					}
				}
				catch (const std::exception&) {}
			}
		}
	}
}

/// <summary>
/// Send no response to complete the async call.
/// </summary>
/// <param name="senderAddress">The sender address.</param>
void UdpBroadcastClient::SendTo(const std::string& senderAddress)
{
	// If initialised.
	if (_initialised)
	{
		// Create the sender end point.
		std::string data = "";

		try
		{
			// Create the sender end point.
			boost::asio::ip::address addr4(boost::asio::ip::address_v4::from_string(senderAddress));

			// If IPv4.
			if (addr4.is_v4())
			{
				// Send the data.
				udp::endpoint sender(addr4, 0);
				_udpClientV4->SendToSender(data, sender);
			}
		}
		catch (const std::exception&) {}
		
		try
		{
			// Resolver the host.
			boost::asio::ip::address addr6(boost::asio::ip::address_v6::from_string(senderAddress));

			// If IPv6.
			if (addr6.is_v6())
			{
				// Send the data.
				udp::endpoint sender(addr6, 0);
				_udpClientV6->SendToSender(data, sender);
			}
		}
		catch (const std::exception&) {}
	}
}

///	<summary>
///	Open the connection.
///	</summary>
void UdpBroadcastClient::Open()
{
	// If initialised.
	if (_initialised)
	{
		_udpClientV4->Open();
		_udpClientV6->Open();
	}
}

///	<summary>
///	Close the connection.
///	</summary>
void UdpBroadcastClient::Close()
{
	// If initialised.
	if (_initialised)
	{
		_udpClientV4->Close();
		_udpClientV6->Close();
	}
}

/// <summary>
/// Send data to the endpoint created after calling connect.
/// </summary>
/// <param name="data">The data to send.</param>
/// <param name="host">The UDP host name or IP.</param>
/// <param name="port">The UDP host port.</param>
void UdpBroadcastClient::SendToDirect(const std::string& data, const std::string& host, unsigned int port)
{
	// If initialised.
	if (_initialised)
	{
		try
		{
			// Create the sender end point.
			boost::asio::ip::address addr4(boost::asio::ip::address_v4::from_string(host));

			// If IPv4.
			if (addr4.is_v4())
			{
				// Send the data.
				_udpClientV4->SendToDirect(data, host, port);
			}
		}
		catch (const std::exception&) {}
		
		try
		{
			// Resolver the host.
			boost::asio::ip::address addr6(boost::asio::ip::address_v6::from_string(host));

			// If IPv6.
			if (addr6.is_v6())
			{
				// Send the data.
				_udpClientV6->SendToDirect(data, host, port);
			}
		}
		catch (const std::exception&) {}
	}
}

/// <summary>
/// Broadcast data on the port (only IPv4).
/// </summary>
/// <param name="data">The data to send.</param>
/// <param name="port">The port to broadcast on.</param>
void UdpBroadcastClient::SendToBroadcastDirect(const std::string& data, unsigned int port)
{
	// If initialised.
	if (_initialised)
	{
		// Broadcast on IPv4 subnet.
		_udpClientV4->SendToBroadcastDirect(data, port);
	}
}

/// <summary>
/// Broadcast data on the port (only IPv4).
/// </summary>
/// <param name="data">The data to send.</param>
/// <param name="port">The port to broadcast on.</param>
/// <param name="ip">The port to broadcast on.</param>
/// <param name="ipMask">The port to broadcast on.</param>
void UdpBroadcastClient::SendToBroadcastDirect(const std::string& data, unsigned int port, const std::string& ip, const std::string& ipMask)
{
	// If initialised.
	if (_initialised)
	{
		// Broadcast on IPv4 subnet.
		_udpClientV4->SendToBroadcastDirect(data, port, ip, ipMask);
	}
}
