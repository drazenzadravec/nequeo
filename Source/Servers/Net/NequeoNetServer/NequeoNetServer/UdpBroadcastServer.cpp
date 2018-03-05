/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          UdpBroadcastServer.cpp
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
#include "Shlwapi.h"
#include <time.h>

#include "UdpServer.cpp"
#include "UdpBroadcastServer.h"
#include "Base\StringEx.h"
#include "Base\StringUtils.h"

#include <boost/asio.hpp>

using boost::asio::ip::udp;
using namespace Nequeo::Net::UDP;

/// <summary>
/// UDP Broadcast web server.
/// </summary>
/// <param name="port">The UDP listening port.</param>
UdpBroadcastServer::UdpBroadcastServer(unsigned int port) :
	_disposed(false), _port(port), _initialised(false)
{
}

/// <summary>
/// UDP Broadcast web server.
/// </summary>
UdpBroadcastServer::~UdpBroadcastServer()
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
void UdpBroadcastServer::Initialise()
{
	// If not initialised.
	if (!_initialised)
	{
		// Create the new array of servers.
		_udpServerV4 = std::make_shared<UdpServer>(_port, udp::v4());
		_udpServerV6 = std::make_shared<UdpServer>(_port, udp::v6());

		// Initialise the server.
		_udpServerV4->OnReceivedData([this](const std::string& data, const udp::endpoint& sender)
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
		_udpServerV4->Initialise();

		// Initialise the server.
		_udpServerV6->OnReceivedData([this](const std::string& data, const udp::endpoint& sender)
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
		_udpServerV6->Initialise();
		_initialised = true;
	}
}

///	<summary>
///	Start the servers.
///	</summary>
void UdpBroadcastServer::Start()
{
	// If initialised.
	if (_initialised)
	{
		_threadV4 = std::thread(std::bind(&UdpBroadcastServer::StartServerV4, this));
		_threadV4.detach();

		_threadV6 = std::thread(std::bind(&UdpBroadcastServer::StartServerV6, this));
		_threadV6.detach();
	}
}

///	<summary>
///	Start IPv4 server.
///	</summary>
void UdpBroadcastServer::StartServerV4()
{
	try
	{
		// Main thread
		_udpServerV4->Start();
	}
	catch (...) {}
}

///	<summary>
///	Start IPv6 server.
///	</summary>
void UdpBroadcastServer::StartServerV6()
{
	try
	{
		// Main thread
		_udpServerV6->Start();
	}
	catch (...) {}
}

///	<summary>
///	Stop the servers.
///	</summary>
void UdpBroadcastServer::Stop()
{
	// If initialised.
	if (_initialised)
	{
		try
		{
			// Close.
			_udpServerV4->Stop();
			_udpServerV6->Stop();
		}
		catch (...) {}

		try
		{
			// Clear sender endpoints.
			_senderEndpoints.clear();
		}
		catch (...) {}

		try
		{
			_threadV4.join();
			_threadV6.join();
		}
		catch (...) {}
	}
}

/// <summary>
/// Get a copy of all sender endpoints.
/// </summary>
/// <return>The collection of sender endpoints.</return>
std::set<std::shared_ptr<UdpSenderEndpoint>> UdpBroadcastServer::GetSenderEndpoints()
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
void UdpBroadcastServer::OnReceivedData(const UdpBroadcastReceiveHandler& received)
{
	_onReceivedData = received;
}

/// <summary>
/// Send data to sender endpoint.
/// </summary>
/// <param name="data">The data to send.</param>
/// <param name="senderAddress">The sender address.</param>
/// <param name="senderPort">The sender port.</param>
void UdpBroadcastServer::SendTo(const std::string& data, const std::string& senderAddress, unsigned int senderPort)
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
						_udpServerV4->SendToSender(data, endpoint->senderEndpoint);
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
						_udpServerV6->SendToSender(data, endpoint->senderEndpoint);
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
void UdpBroadcastServer::SendTo(const std::string& senderAddress)
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
				_udpServerV4->SendToSender(data, sender);
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
				_udpServerV6->SendToSender(data, sender);
			}
		}
		catch (const std::exception&) {}
	}
}