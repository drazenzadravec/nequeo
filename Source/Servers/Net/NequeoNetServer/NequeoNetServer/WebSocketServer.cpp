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

#include "HttpClient.h"
#include "WebSocketServer.h"
#include "WebSocketJson.h"

#include "Base\Base64.h"
#include "Base\StringEx.h"
#include "Base\StringUtils.h"
#include "Cryptography\AdvancedAES.h"

#include <boost/uuid/uuid.hpp>            // uuid class
#include <boost/uuid/uuid_generators.hpp> // generators
#include <boost/uuid/uuid_io.hpp>         // streaming operators etc.

using namespace Nequeo::Net::WebSocket;
using namespace Nequeo::Net::Http;

std::atomic<const char*> rootWsPath;
static const char* EXECUTOR_WS_SERVER_TAG = "Executor_WS_Server";
//void OnWebSocketContext(const WebContext*);

/// <summary>
/// WebSocket web server.
/// </summary>
/// <param name="path">The root folder path (this is where all files are located).</param>
WebSocketServer::WebSocketServer(const std::string& path) :
	_disposed(false), _path(path), _initialised(false), _udpBroadcastEnabled(false), 
	_udpBroadcastPort(0), _udpBroadcastCallbackPort(0), _serverID(""), _accessTokenVerifyURL(""),
	_clientLocationRequestURL(""), _clientLocationRequestEnabled(false)
{
	// Create a new executor.
	_executor = Nequeo::MakeShared<Nequeo::Threading::DefaultExecutor>(EXECUTOR_WS_SERVER_TAG);
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
		_servers->OnWebContext([this](const WebContext* context)
		{
			this->OnWebSocketContext(context);
		});

		// If UDP server is enabled.
		if (_udpBroadcastEnabled && _udpBroadcastPort > 0)
		{
			// Init UDP server.
			_udpServer = std::make_shared<UdpBroadcastServer>(_udpBroadcastPort);
			_udpServer->OnReceivedData([this](const std::string& data, const std::string& ipAddressSender, unsigned int portSender)
			{
				this->OnUdpReceiveData(data, ipAddressSender, portSender);
			});
			_udpServer->Initialise();
		}

		// If UDP server is enabled.
		if (_udpBroadcastEnabled && _udpBroadcastCallbackPort > 0)
		{
			// Init UDP server.
			_udpServerCallback = std::make_shared<UdpBroadcastServer>(_udpBroadcastCallbackPort);
			_udpServerCallback->OnReceivedData([this](const std::string& data, const std::string& ipAddressSender, unsigned int portSender)
			{
				this->OnUdpReceiveDataCallback(data, ipAddressSender, portSender);
			});
			_udpServerCallback->Initialise();
		}

		_initialised = true;

		// Set the root path.
		rootWsPath = _path.c_str();

		// Create a uniqueid.
		boost::uuids::random_generator generator;
		boost::uuids::uuid uuid = generator();
		_serverID = boost::uuids::to_string(uuid);
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

		// If UDP server is enabled.
		if (_udpBroadcastEnabled && _udpBroadcastPort > 0)
		{
			_udpServer->Start();
		}

		// If UDP server is enabled.
		if (_udpBroadcastEnabled && _udpBroadcastCallbackPort > 0)
		{
			_udpServerCallback->Start();
		}
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

		// If UDP server is enabled.
		if (_udpBroadcastEnabled && _udpBroadcastPort > 0)
		{
			_udpServer->Stop();
		}

		// If UDP server is enabled.
		if (_udpBroadcastEnabled && _udpBroadcastCallbackPort > 0)
		{
			_udpServerCallback->Stop();
		}
	}
}

/// <summary>
/// On web socket context request.
/// </summary>
/// <param name="context">The web context.</param>
void WebSocketServer::OnWebSocketContext(const WebContext* context)
{
	// All requests will come into this function.
	try
	{
		// Cast away the const.
		WebContext* internalWebContext = const_cast<WebContext*>(context);

		// Create a uniqueid.
		boost::uuids::random_generator generator;
		boost::uuids::uuid uuid = generator();
		const std::string tmp = boost::uuids::to_string(uuid);

		// Set the defaults for this client.
		internalWebContext->SetUniqueID(tmp);
		internalWebContext->SetApplicationID(tmp);
		internalWebContext->SetAvailable(false);
		internalWebContext->SetBroadcast(false);
		internalWebContext->SetBroadcastAppID(false);
		internalWebContext->SetHasAccess(false);
		internalWebContext->SetClientToken("");
		
		// On close.
		internalWebContext->OnClose = [internalWebContext](int status, const std::string& reason)
		{
			// Client is un-available.
			internalWebContext->SetAvailable(false);
		};

		// On error.
		internalWebContext->OnError = [internalWebContext](const std::string& error)
		{
			// Client is un-available.
			internalWebContext->SetAvailable(false);
		};

		// On access expiry.
		internalWebContext->OnAccessExpiry = [internalWebContext](std::shared_ptr<WebMessage>& message)
		{
			try
			{
				std::istringstream inputClose("Access expiry timeout");
				message->Send(MessageType::Close, inputClose.rdbuf());
			}
			catch (const std::exception&) {}
		};

		// On message.
		internalWebContext->OnMessage = [this, internalWebContext](MessageType messageType, size_t length, std::shared_ptr<WebMessage>& message)
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
							// If no access.
							if (!internalWebContext->HasAccess())
							{
								// Send message.
								std::istringstream input("{\"response\":\"error\",\"error\":\"Access Denied\"}");
								message->Send(messageType, input.rdbuf());
								return;
							}

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
								// Create the response string.
								std::string responseBegin = "{\"response\":\"ok\",\"time\":\"";
								std::string responseMiddle(timebuf);
								std::string responseEnd = "\"}";
								std::string response = responseBegin + responseMiddle + responseEnd;

								// Time text.
								std::istringstream input(response);
								message->Send(messageType, input.rdbuf());
							}
							else
							{
								// Echo text.
								std::istringstream input("{\"response\":\"error\",\"error\":\"No Time\"}");
								message->Send(messageType, input.rdbuf());
							}
						}
						else if (messageText == "applicationids")
						{
							// If no access.
							if (!internalWebContext->HasAccess())
							{
								// Send message.
								std::istringstream input("{\"response\":\"error\",\"error\":\"Access Denied\"}");
								message->Send(messageType, input.rdbuf());
								return;
							}

							// Get the list of application ids
							std::string listText = "";

							// If requesting list of all connections.
							for (auto& server : this->_servers->GetWebServers())
							{
								// For each connection on each server.
								for (auto connection : server->GetConnections())
								{
									// Can the application id be broadcast.
									if (connection->BroadcastAppID())
									{
										// Add each client.
										listText += "{\"application\":\"" + connection->GetApplicationID() + "\"},";
									}
								}
							}

							// Trim end.
							std::string trimChars(",");
							std::string listTextTrimed(Nequeo::StringUtils::RTrim(listText.c_str(), trimChars.c_str()).c_str());

							// Send the list.
							std::istringstream input("{\"response\":\"ok\",\"applications\":[" + listTextTrimed + "]}");
							message->Send(messageType, input.rdbuf());
						}
						else if (messageText == "uniqueids")
						{
							// If no access.
							if (!internalWebContext->HasAccess())
							{
								// Send message.
								std::istringstream input("{\"response\":\"error\",\"error\":\"Access Denied\"}");
								message->Send(messageType, input.rdbuf());
								return;
							}

							// Get the list of unique ids
							std::string listText = "";

							// If requesting list of all connections.
							for (auto& server : this->_servers->GetWebServers())
							{
								// For each connection on each server.
								for (auto connection : server->GetConnections())
								{
									// Can the unique id be broadcast.
									if (connection->Broadcast())
									{
										// Add each client.
										listText += "{\"unique\":\"" + connection->GetUniqueID() + "\"},";
									}
								}
							}

							// Trim end.
							std::string trimChars(",");
							std::string listTextTrimed(Nequeo::StringUtils::RTrim(listText.c_str(), trimChars.c_str()).c_str());

							// Send the list.
							std::istringstream input("{\"response\":\"ok\",\"uniques\":[" + listTextTrimed + "]}");
							message->Send(messageType, input.rdbuf());
						}
						else if (messageText == "uniqueapplication")
						{
							// If no access.
							if (!internalWebContext->HasAccess())
							{
								// Send message.
								std::istringstream input("{\"response\":\"error\",\"error\":\"Access Denied\"}");
								message->Send(messageType, input.rdbuf());
								return;
							}

							// Get the list of group ids
							std::string listText = "";

							// If requesting list of all connections.
							for (auto& server : this->_servers->GetWebServers())
							{
								// For each connection on each server.
								for (auto connection : server->GetConnections())
								{
									// Can the unique id be broadcast and can the application id be broadcast.
									if (connection->Broadcast() && connection->BroadcastAppID())
									{
										// Add each client.
										listText += "{\"unique\":\"" + connection->GetUniqueID() + "\",\"application\":\"" + connection->GetApplicationID() + "\"},";
									}
								}
							}

							// Trim end.
							std::string trimChars(",");
							std::string listTextTrimed(Nequeo::StringUtils::RTrim(listText.c_str(), trimChars.c_str()).c_str());

							// Send the list.
							std::istringstream input("{\"response\":\"ok\",\"groups\":[" + listTextTrimed + "]}");
							message->Send(messageType, input.rdbuf());
						}
						else
						{
							// Look for internal JSON data.
							WebSocketJson json;

							// Assign the current data.
							WebSocketJsonData dataCurr;
							dataCurr.uniqueID = internalWebContext->GetUniqueID();
							dataCurr.applicationID = internalWebContext->GetApplicationID();
							dataCurr.available = internalWebContext->Available();
							dataCurr.broadcast = internalWebContext->Broadcast();
							dataCurr.broadcastAppID = internalWebContext->BroadcastAppID();

							// Get the data.
							WebSocketJsonData data = json.ReadJson(messageText, dataCurr);

							// Does JSON data exist.
							if (data.hasJsonData)
							{
								// Assign the changes.
								if (data.hasUniqueID)
									internalWebContext->SetUniqueID(data.uniqueID);

								if (data.hasApplicationID)
									internalWebContext->SetApplicationID(data.applicationID);

								if (data.hasAvailable)
									internalWebContext->SetAvailable(data.available);

								if (data.hasBroadcast)
									internalWebContext->SetBroadcast(data.broadcast);

								if (data.hasBroadcastAppID)
									internalWebContext->SetBroadcastAppID(data.broadcastAppID);

								// If no access.
								if (data.hasAccessToken && data.hasUniqueID && data.hasApplicationID)
								{
									// Attempt to grant access.
									try
									{
										// Make a request to the access token authority.
										HttpClient client(_accessTokenVerifyURL);

										// Create the request.
										Nequeo::Net::Http::NetRequest request;
										request.SetMethod("GET");
										request.SetContentLength(0);
										request.SetPath(client.GetURLPath() + client.GetURLQuery());

										// Send the token.
										std::string authValueBearer("Bearer ");
										std::string authValueToken(data.accessToken);
										std::string authValue = authValueBearer + authValueToken;
										request.AddHeader("Authorization", authValue);
										request.AddHeader("UniqueID", internalWebContext->GetUniqueID());
										request.AddHeader("ApplicationID", internalWebContext->GetApplicationID());

										// Open and send the request.
										client.Connect();
										Nequeo::Net::Http::NetResponse& response = client.Request(request);

										// Get the response.
										std::istream accessTokenJSONResponse(response.Content->rdbuf());
										size_t dataSize(static_cast<size_t>(accessTokenJSONResponse.rdbuf()->in_avail()));

										// Allocate the char size.
										auto bytes = std::unique_ptr<char[]>(new char[dataSize]);
										accessTokenJSONResponse.read(bytes.get(), dataSize);
										std::string accessTokenJSON(bytes.get(), dataSize);

										// Read the JSON response.
										WebSocketJsonAccessToken tokenResponse = json.ReadJsonAccessToken(accessTokenJSON);

										// If response.
										if (tokenResponse.hasJsonData && tokenResponse.hasGrant)
										{
											bool hasAccess = false;
											hasAccess = tokenResponse.grant;

											// If allowed.
											if (hasAccess)
											{
												// Should the uniqueid be matched with what is in the returned token
												// and what was entered by the user.
												if (tokenResponse.hasUniqueidMatch && tokenResponse.hasUniqueID)
												{
													try
													{
														// If a unique id match must be performed.
														if (tokenResponse.uniqueidMatch)
														{
															// If not the same.
															if (tokenResponse.uniqueID != data.uniqueID)
															{
																// No access is allowed.
																hasAccess = false;
															}
														}
													}
													catch (const std::exception&)
													{
														// No access.
														hasAccess = false;
													}
												}
											}

											// Grant or not.
											internalWebContext->SetHasAccess(hasAccess);
											if (hasAccess)
											{
												// Assign the access token.
												internalWebContext->SetClientToken(data.accessToken);

												// If access has been granted then set the expiry timeout.
												internalWebContext->StartAccessExpiry(tokenResponse.expiry);

												// Cancel the timeout connect timer.
												internalWebContext->CancelTimeoutConnect(hasAccess);
											}
										}
										else
										{
											// Do not grant.
											internalWebContext->SetHasAccess(false);
										}
									}
									catch (const std::exception&)
									{
										std::istringstream input("{\"response\":\"error\",\"error\":\"Unable to grant access\"}");
										std::istringstream inputClose("Access Error");
										message->Send(messageType, input.rdbuf());
										message->Send(MessageType::Close, inputClose.rdbuf());
									}
								}
							}

							// If no access.
							if (!internalWebContext->HasAccess())
							{
								// Send message.
								std::istringstream input("{\"response\":\"error\",\"error\":\"Access Denied\"}");
								message->Send(messageType, input.rdbuf());
								return;
							}

							// Contact request.
							if (data.hasJsonData && data.hasContactUniqueID && data.hasContactApplicationID)
							{
								bool foundContact = false;

								// Find the contact.
								for (auto& server : this->_servers->GetWebServers())
								{
									// For each connection on each server.
									for (auto connection : server->GetConnections())
									{
										// Get the contact details.
										std::string uniqueID = connection->GetUniqueID();
										std::string applicationID = connection->GetApplicationID();

										// Find the contact.
										if (uniqueID == data.contactUniqueID && applicationID == data.contactApplicationID)
										{
											// Found the contact.
											foundContact = true;
										}

										// Complete.
										if (foundContact)
										{
											try
											{
												// If available.
												if (connection->Available())
												{
													// Add the JSON data to the stream.
													std::istringstream inputBack("{\"response\":\"ok\",\"available\":true}");
													message->Send(messageType, inputBack.rdbuf());

													// Add the JSON data to the stream.
													std::istringstream input(json.WriteJson(messageText, dataCurr, true));

													// Send the message.
													auto contact = connection->Message();
													contact->Send(messageType, input.rdbuf());
												}
												else
												{
													// Add the JSON data to the stream.
													std::istringstream input("{\"response\":\"ok\",\"available\":false}");
													message->Send(messageType, input.rdbuf());
												}
											}
											catch (const std::exception&)
											{
												std::istringstream input("{\"response\":\"error\",\"error\":\"Unable to send message to contact\"}");
												message->Send(messageType, input.rdbuf());
											}

											break;
										}
									}

									// Complete.
									if (foundContact)
										break;
								}

								// Not found.
								if (!foundContact)
								{
									std::istringstream input("{\"response\":\"error\",\"error\":\"No contacts, searching\"}");
									message->Send(messageType, input.rdbuf());

									// If UDP server is enabled.
									if (_udpBroadcastPort > 0 && (_udpBroadcastEnabled || _clientLocationRequestEnabled))
									{
										// Broadcast to all servers, ask to search for contact
										// that was not found on this server.
										WebSocketJsonData broadcastData;
										broadcastData.response = "ok";
										broadcastData.error = "";
										broadcastData.serverID = _serverID;
										broadcastData.available = false;
										broadcastData.uniqueID = internalWebContext->GetUniqueID();
										broadcastData.applicationID = internalWebContext->GetApplicationID();
										broadcastData.contactUniqueID = data.contactUniqueID;
										broadcastData.contactApplicationID = data.contactApplicationID;

										// Get the broadcast data.
										unsigned int udpBroadcastPort = _udpBroadcastPort;
										std::string udpBroadcastAddress = _udpBroadcastAddress;
										std::string udpBroadcastMask = _udpBroadcastMask;
										bool udpBroadcastEnabled = _udpBroadcastEnabled;
										bool clientLocationRequestEnabled = _clientLocationRequestEnabled;
										std::string clientLocationRequestURL = _clientLocationRequestURL;
										std::string clientToken = internalWebContext->GetClientToken();
										std::string contactUniqueID = data.contactUniqueID;
										std::string contactApplicationID = data.contactApplicationID;

										// Get the data and sent.
										std::string broadcast = json.WriteJson(messageText, broadcastData);
										_executor->Submit([this, broadcast, udpBroadcastPort, udpBroadcastAddress, udpBroadcastMask, 
											udpBroadcastEnabled, clientLocationRequestEnabled, clientLocationRequestURL, clientToken,
											contactUniqueID, contactApplicationID]
										{ 
											this->BroadcastDataAsync(broadcast, udpBroadcastPort, udpBroadcastAddress, udpBroadcastMask, 
												udpBroadcastEnabled, clientLocationRequestEnabled, clientLocationRequestURL, clientToken,
												contactUniqueID, contactApplicationID);
										});
									}
								}
							}
							else
							{
								std::istringstream input("{\"response\":\"ok\",\"settings\":true}");
								message->Send(messageType, input.rdbuf());
							}
						}
					}

					// If binary message.
					if (messageType == MessageType::Binary)
					{
						// Echo binary.
						std::istringstream inputString("{\"response\":\"ok\"}");
						std::istream input(inputString.rdbuf());
						message->Send(messageType, input.rdbuf());
					}
				}
				else
				{
					// Send back message type.
					if (messageType == MessageType::Close)
					{
						std::istringstream input("{\"response\":\"close\"}");
						message->Send(messageType, input.rdbuf());
					}

					// Send back message type.
					if (messageType == MessageType::Ping)
					{
						std::istringstream input("{\"response\":\"ping\"}");
						message->Send(messageType, input.rdbuf());
					}
				}
			}
		};
	}
	catch (...) {}
}

/// <summary>
/// Broadcast data to all websocket servers.
/// </summary>
/// <param name="data">The sent data.</param>
/// <param name="port">The UDP broadcast port.</param>
/// <param name="address">The UDP broadcast address.</param>
/// <param name="mask">The UDP broadcast mask.</param>
/// <param name="udpBroadcastEnabled">The UDP broadcast enabled.</param>
/// <param name="clientLocationRequestEnabled">The client location request enabled.</param>
/// <param name="clientLocationRequestURL">The client location request URL.</param>
/// <param name="clientToken">The client loken.</param>
/// <param name="contactUniqueID">The contact uniqueID.</param>
/// <param name="contactApplicationID">The contact applicationID.</param>
void WebSocketServer::BroadcastDataAsync(const std::string& data, unsigned int port, const std::string& address, const std::string& mask, 
	bool udpBroadcastEnabled, bool clientLocationRequestEnabled, const std::string& clientLocationRequestURL, const std::string& clientToken,
	const std::string& contactUniqueID, const std::string& contactApplicationID)
{
	try
	{
		// Encrypt.
		Nequeo::Cryptography::AdvancedAES aes;
		std::string encryptedData = aes.EncryptToMemory(data, "Zy0+-Tq5G8}bn!H2Ws3{7C*oAm1$4/kF");

		// Encrypt convert.
		vector<char> vectorBaseEnc(encryptedData.begin(), encryptedData.end());
		char* encryptedDataChar = &vectorBaseEnc[0];
		const unsigned char* encrypted = (unsigned char*)encryptedDataChar;

		// Base64.
		Nequeo::Base64 base64;
		Nequeo::ByteBuffer bufferEncode(encrypted, vectorBaseEnc.size());
		std::string encryptedBase64 = std::string(base64.Encode(bufferEncode).c_str());

		// Get broadcast.
		std::string udpAddress = address;
		std::string udpMask = mask;

		// If client location request enabled.
		if (clientLocationRequestEnabled)
		{
			try
			{
				// Make a request to the client location request server.
				HttpClient client(clientLocationRequestURL);

				// Create the request.
				Nequeo::Net::Http::NetRequest request;
				request.SetMethod("GET");
				request.SetContentLength(0);
				request.SetPath(client.GetURLPath() + client.GetURLQuery());

				// Send the token.
				std::string authValueBearer("Bearer ");
				std::string authValueToken(clientToken);
				std::string authValue = authValueBearer + authValueToken;
				request.AddHeader("Authorization", authValue);
				request.AddHeader("ContactUniqueID", contactUniqueID);
				request.AddHeader("ContactApplicationID", contactApplicationID);

				// Open and send the request.
				client.Connect();
				Nequeo::Net::Http::NetResponse& response = client.Request(request);

				// Get the response.
				std::istream clientLocationJSONResponse(response.Content->rdbuf());
				size_t dataSize(static_cast<size_t>(clientLocationJSONResponse.rdbuf()->in_avail()));

				// Allocate the char size.
				auto bytes = std::unique_ptr<char[]>(new char[dataSize]);
				clientLocationJSONResponse.read(bytes.get(), dataSize);
				std::string clientLocationJSON(bytes.get(), dataSize);

				// Read the JSON response.
				WebSocketJson json;
				WebSocketJsonClientLocation locationResponse = json.ReadJsonClientLocation(clientLocationJSON);

				// If the client contact unique id match.
				if (locationResponse.hasJsonData && contactUniqueID == locationResponse.uniqueID)
				{
					// Assgin the address and mask.
					udpAddress = locationResponse.address;
					udpMask = locationResponse.mask;
				}
			}
			catch (const std::exception&) {}
		}

		// Broadcast the data.
		UdpBroadcastClient udpClient(0);
		udpClient.InitialiseDirect();

		if (udpAddress == "" || udpMask == "")
		{
			// Broadcast to all.
			udpClient.SendToBroadcastDirect(encryptedBase64, port);
		}
		else
		{
			// Broadcast to subnet.
			udpClient.SendToBroadcastDirect(encryptedBase64, port, udpAddress, udpMask);
		}
	}
	catch (...) {}
}

/// <summary>
/// Send data to host endpoint.
/// </summary>
/// <param name="data">The data to send.</param>
/// <param name="host">The host address.</param>
/// <param name="port">The host port.</param>
void WebSocketServer::SendCallbackDataAsync(const std::string& data, const std::string& host, unsigned int port)
{
	try
	{
		// Encrypt.
		Nequeo::Cryptography::AdvancedAES aes;
		std::string encryptedData = aes.EncryptToMemory(data, "Zy0+-Tq5G8}bn!H2Ws3{7C*oAm1$4/kF");

		// Encrypt convert.
		vector<char> vectorBaseEnc(encryptedData.begin(), encryptedData.end());
		char* encryptedDataChar = &vectorBaseEnc[0];
		const unsigned char* encrypted = (unsigned char*)encryptedDataChar;

		// Base64.
		Nequeo::Base64 base64;
		Nequeo::ByteBuffer bufferEncode(encrypted, vectorBaseEnc.size());
		std::string encryptedBase64 = std::string(base64.Encode(bufferEncode).c_str());

		// Broadcast the data.
		UdpBroadcastClient udpClient(0);
		udpClient.InitialiseDirect();
		udpClient.SendToDirect(encryptedBase64, host, port);
	}
	catch (...) {}
}

/// <summary>
/// On receive data.
/// </summary>
/// <param name="data">The sent data.</param>
/// <param name="ipAddressSender">The sender IP address.</param>
/// <param name="portSender">The sender port.</param>
void WebSocketServer::OnUdpReceiveData(const std::string& data, const std::string& ipAddressSender, unsigned int portSender)
{
	try
	{
		// Send back nothing.
		_udpServer->SendTo(ipAddressSender);
	}
	catch (...) {}

	// If data and.
	if (data.size() > 0)
	{
		try
		{
			// Look for internal JSON data.
			WebSocketJson json;

			// Assign the current data.
			WebSocketJsonData dataCurr;
			dataCurr.uniqueID = "";
			dataCurr.applicationID = "";
			dataCurr.available = false;
			dataCurr.broadcast = false;
			dataCurr.broadcastAppID = false;

			// Base64
			Nequeo::Base64 base64;
			Nequeo::String encryptedBase64(data.c_str());
			Nequeo::ByteBuffer bufferDecode = base64.Decode(encryptedBase64);
			unsigned char* encryptedData = bufferDecode.GetUnderlyingData();
			std::string encrypted = (char*)encryptedData;

			// Decrypt.
			Nequeo::Cryptography::AdvancedAES aes;
			std::string decryptedData = aes.DecryptFromMemory(encrypted, "Zy0+-Tq5G8}bn!H2Ws3{7C*oAm1$4/kF");

			// Get the data.
			std::string recData = decryptedData;
			WebSocketJsonData jsonData = json.ReadJson(recData, dataCurr);

			// If contact exists and not this server.
			if (jsonData.hasJsonData && jsonData.hasContactUniqueID && jsonData.hasContactApplicationID && 
				(jsonData.serverID != _serverID))
			{
				bool foundContact = false;

				// Find the contact.
				for (auto& server : this->_servers->GetWebServers())
				{
					// For each connection on each server.
					for (auto connection : server->GetConnections())
					{
						// Get the contact details.
						std::string uniqueID = connection->GetUniqueID();
						std::string applicationID = connection->GetApplicationID();

						// Find the contact.
						if (uniqueID == jsonData.contactUniqueID && applicationID == jsonData.contactApplicationID)
						{
							// Found the contact.
							foundContact = true;
						}

						// Complete.
						if (foundContact)
						{
							try
							{
								// If available.
								if (connection->Available())
								{
									// Add the JSON data to the stream.
									std::istringstream input(json.WriteJson(recData, jsonData, true));

									// Send the message.
									auto contact = connection->Message();
									contact->Send(MessageType::Text, input.rdbuf());

									// If UDP server is enabled.
									if (_udpBroadcastEnabled && _udpBroadcastCallbackPort > 0)
									{
										// Assign the current data.
										WebSocketJsonData dataCallback;
										dataCallback.response = "ok";
										dataCallback.error = "";
										dataCallback.serverID = _serverID;
										dataCallback.available = true;
										dataCallback.uniqueID = jsonData.uniqueID;
										dataCallback.applicationID = jsonData.applicationID;
										dataCallback.contactUniqueID = jsonData.contactUniqueID;
										dataCallback.contactApplicationID = jsonData.contactApplicationID;

										// Get the broadcast data.
										unsigned int udpBroadcastPort = _udpBroadcastCallbackPort;
										std::string inputCallback = json.WriteJson("{\"response\":\"ok\",\"available\":true}", dataCallback);
										_executor->Submit([this, inputCallback, ipAddressSender, udpBroadcastPort]
										{
											this->SendCallbackDataAsync(inputCallback, ipAddressSender, udpBroadcastPort);
										});
									}
								}
								else
								{
									// If UDP server is enabled.
									if (_udpBroadcastEnabled && _udpBroadcastCallbackPort > 0)
									{
										// Assign the current data.
										WebSocketJsonData dataCallback;
										dataCallback.response = "ok";
										dataCallback.error = "";
										dataCallback.serverID = _serverID;
										dataCallback.available = false;
										dataCallback.uniqueID = jsonData.uniqueID;
										dataCallback.applicationID = jsonData.applicationID;
										dataCallback.contactUniqueID = jsonData.contactUniqueID;
										dataCallback.contactApplicationID = jsonData.contactApplicationID;

										// Get the broadcast data.
										unsigned int udpBroadcastPort = _udpBroadcastCallbackPort;
										std::string inputCallback = json.WriteJson("{\"response\":\"ok\",\"available\":false}", dataCallback);
										_executor->Submit([this, inputCallback, ipAddressSender, udpBroadcastPort]
										{
											this->SendCallbackDataAsync(inputCallback, ipAddressSender, udpBroadcastPort);
										});
									}
								}
							}
							catch (const std::exception&)
							{
								bool foundContact = false;
							}

							break;
						}
					}

					// Complete.
					if (foundContact)
						break;
				}

				// Not found.
				if (!foundContact)
				{
					// If UDP server is enabled.
					if (_udpBroadcastEnabled && _udpBroadcastCallbackPort > 0)
					{
						// Assign the current data.
						WebSocketJsonData dataCallback;
						dataCallback.response = "error";
						dataCallback.error = "No contacts, searching";
						dataCallback.serverID = _serverID;
						dataCallback.available = false;
						dataCallback.uniqueID = jsonData.uniqueID;
						dataCallback.applicationID = jsonData.applicationID;
						dataCallback.contactUniqueID = jsonData.contactUniqueID;
						dataCallback.contactApplicationID = jsonData.contactApplicationID;

						// Get the broadcast data.
						unsigned int udpBroadcastPort = _udpBroadcastCallbackPort;
						std::string inputCallback = json.WriteJson("{\"response\":\"error\",\"error\":\"No contacts, searching\"}", dataCallback);
						_executor->Submit([this, inputCallback, ipAddressSender, udpBroadcastPort] 
						{ 
							this->SendCallbackDataAsync(inputCallback, ipAddressSender, udpBroadcastPort); 
						});
					}
				}
			}
		}
		catch (...) {}
	}
}

/// <summary>
/// On receive data.
/// </summary>
/// <param name="data">The sent data.</param>
/// <param name="ipAddressSender">The sender IP address.</param>
/// <param name="portSender">The sender port.</param>
void WebSocketServer::OnUdpReceiveDataCallback(const std::string& data, const std::string& ipAddressSender, unsigned int portSender)
{
	try
	{
		// Send back nothing.
		_udpServerCallback->SendTo(ipAddressSender);
	}
	catch (...) {}
	
	// If data and.
	if (data.size() > 0)
	{
		try
		{
			// Look for internal JSON data.
			WebSocketJson json;

			// Assign the current data.
			WebSocketJsonData dataCurr;
			dataCurr.uniqueID = "";
			dataCurr.applicationID = "";
			dataCurr.available = false;
			dataCurr.broadcast = false;
			dataCurr.broadcastAppID = false;

			// Base64
			Nequeo::Base64 base64;
			Nequeo::String encryptedBase64(data.c_str());
			Nequeo::ByteBuffer bufferDecode = base64.Decode(encryptedBase64);
			unsigned char* encryptedData = bufferDecode.GetUnderlyingData();
			std::string encrypted = (char*)encryptedData;

			// Decrypt.
			Nequeo::Cryptography::AdvancedAES aes;
			std::string decryptedData = aes.DecryptFromMemory(encrypted, "Zy0+-Tq5G8}bn!H2Ws3{7C*oAm1$4/kF");

			// Get the data.
			std::string recData = decryptedData;
			WebSocketJsonData jsonData = json.ReadJson(recData, dataCurr);

			// If contact exists and not this server.
			if (jsonData.hasJsonData && jsonData.hasUniqueID && jsonData.hasApplicationID)
			{
				bool foundContact = false;

				// Find the contact.
				for (auto& server : this->_servers->GetWebServers())
				{
					// For each connection on each server.
					for (auto connection : server->GetConnections())
					{
						// Get the contact details.
						std::string uniqueID = connection->GetUniqueID();
						std::string applicationID = connection->GetApplicationID();

						// Find the contact.
						if (uniqueID == jsonData.uniqueID && applicationID == jsonData.applicationID)
						{
							// Found the contact.
							foundContact = true;
						}

						// Complete.
						if (foundContact)
						{
							try
							{
								// Add the JSON data to the stream.
								std::istringstream input(json.WriteJson(recData, jsonData));

								// Send the message.
								auto contact = connection->Message();
								contact->Send(MessageType::Text, input.rdbuf());
							}
							catch (const std::exception&)
							{
								bool foundContact = false;
							}

							break;
						}
					}

					// Complete.
					if (foundContact)
						break;
				}
			}
		}
		catch (...) {}
	}
}