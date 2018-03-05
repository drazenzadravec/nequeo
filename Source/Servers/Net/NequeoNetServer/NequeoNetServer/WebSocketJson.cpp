/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WebSocketJson.cpp
*  Purpose :        Web socket JSON class.
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

#include "WebSocketJson.h"

#include <json\json.h>

using namespace Nequeo::Net::WebSocket;

/// <summary>
/// Read the JSON data.
/// </summary>
/// <param name="document">The JSON document.</param>
/// <param name="currentData">The current data used to compare.</param>
/// <return>The JSON data.</return>
WebSocketJsonData WebSocketJson::ReadJson(const std::string& document, const WebSocketJsonData& currentData)
{
	WebSocketJsonData data;

	try
	{
		Json::Reader jsonReader;
		Json::Value jsonValue;

		// Parse the file.
		bool result = jsonReader.parse(document, jsonValue, false);
		if (result)
		{
			try
			{
				// Get the json values.
				std::string uniqueIDID = jsonValue["uniqueID"].asString();
				data.uniqueID = uniqueIDID;
				if (data.uniqueID != "" && currentData.uniqueID != data.uniqueID)
				{
					data.hasUniqueID = true;
					data.hasJsonData = true;
				}
			}
			catch (const std::exception&) { data.hasUniqueID = false; }

			try
			{
				std::string applicationIDID = jsonValue["applicationID"].asString();
				data.applicationID = applicationIDID;
				if (data.applicationID != "" && currentData.applicationID != data.applicationID)
				{
					data.hasApplicationID = true;
					data.hasJsonData = true;
				}
			}
			catch (const std::exception&) { data.hasApplicationID = false; }

			try
			{
				std::string availableID = jsonValue["available"].asString();
				if (availableID != "")
				{
					bool available = jsonValue["available"].asBool();
					data.available = available;
					if (data.available != currentData.available)
					{
						data.hasAvailable = true;
						data.hasJsonData = true;
					}
				}
			}
			catch (const std::exception&) { data.hasAvailable = false; }

			try
			{
				std::string broadcastID = jsonValue["broadcast"].asString();
				if (broadcastID != "")
				{
					bool broadcast = jsonValue["broadcast"].asBool();
					data.broadcast = broadcast;
					if (data.broadcast != currentData.broadcast)
					{
						data.hasBroadcast = true;
						data.hasJsonData = true;
					}
				}
			}
			catch (const std::exception&) { data.hasBroadcast = false; }

			try
			{
				std::string broadcastAppID = jsonValue["broadcastAppID"].asString();
				if (broadcastAppID != "")
				{
					bool broadcastAppID = jsonValue["broadcastAppID"].asBool();
					data.broadcastAppID = broadcastAppID;
					if (data.broadcastAppID != currentData.broadcastAppID)
					{
						data.hasBroadcastAppID = true;
						data.hasJsonData = true;
					}
				}
			}
			catch (const std::exception&) { data.hasBroadcastAppID = false; }

			try
			{
				// Get the json values.
				std::string contactUniqueIDID = jsonValue["contactUniqueID"].asString();
				data.contactUniqueID = contactUniqueIDID;
				if (data.contactUniqueID != "")
				{
					data.hasContactUniqueID = true;
					data.hasJsonData = true;
				}
			}
			catch (const std::exception&) { data.hasContactUniqueID = false; }

			try
			{
				std::string contactApplicationIDID = jsonValue["contactApplicationID"].asString();
				data.contactApplicationID = contactApplicationIDID;
				if (data.contactApplicationID != "")
				{
					data.hasContactApplicationID = true;
					data.hasJsonData = true;
				}
			}
			catch (const std::exception&) { data.hasContactApplicationID = false; }

			try
			{
				std::string responseID = jsonValue["response"].asString();
				data.response = responseID;
				if (data.response != "")
				{
					data.hasResponse = true;
					data.hasJsonData = true;
				}
			}
			catch (const std::exception&) { data.hasResponse = false; }

			try
			{
				std::string errorID = jsonValue["error"].asString();
				data.error = errorID;
				if (data.error != "")
				{
					data.hasError = true;
					data.hasJsonData = true;
				}
			}
			catch (const std::exception&) { data.hasError = false; }

			try
			{
				std::string serverIDID = jsonValue["serverID"].asString();
				data.serverID = serverIDID;
				if (data.serverID != "")
				{
					data.hasServerID = true;
					data.hasJsonData = true;
				}
			}
			catch (const std::exception&) { data.hasServerID = false; }

			try
			{
				std::string accessTokenID = jsonValue["accessToken"].asString();
				data.accessToken = accessTokenID;
				if (data.accessToken != "")
				{
					data.hasAccessToken = true;
					data.hasJsonData = true;
				}
			}
			catch (const std::exception&) { data.hasAccessToken = false; }
		}
	}
	catch (const std::exception&) {}
	return data;
}

/// <summary>
/// Read the JSON data.
/// </summary>
/// <param name="document">The JSON document.</param>
/// <return>The JSON data.</return>
WebSocketJsonAccessToken WebSocketJson::ReadJsonAccessToken(const std::string& document)
{
	WebSocketJsonAccessToken data;

	try
	{
		Json::Reader jsonReader;
		Json::Value jsonValue;

		// Parse the file.
		bool result = jsonReader.parse(document, jsonValue, false);
		if (result)
		{
			try
			{
				std::string grantID = jsonValue["grant"].asString();
				if (grantID != "")
				{
					bool grant = jsonValue["grant"].asBool();
					data.grant = grant;
					data.hasGrant = true;
					data.hasJsonData = true;
				}
			}
			catch (const std::exception&) 
			{ 
				data.hasGrant = false; 
				data.grant = false;
			}

			try
			{
				std::string uniqueIDID = jsonValue["uniqueid"].asString();
				data.uniqueID = uniqueIDID;
				if (data.uniqueID != "")
				{
					data.hasUniqueID = true;
					data.hasJsonData = true;
				}
			}
			catch (const std::exception&) { data.hasUniqueID = false; }

			try
			{
				std::string expiryID = jsonValue["expiry"].asString();
				if (expiryID != "")
				{
					unsigned int expiry = jsonValue["expiry"].asUInt();
					data.expiry = expiry;
					data.hasExpiry = true;
					data.hasJsonData = true;
				}
			}
			catch (const std::exception&) 
			{ 
				data.hasExpiry = false; 
				data.expiry = 1U;
			}

			try
			{
				std::string tokenIDID = jsonValue["tokenid"].asString();
				data.tokenID = tokenIDID;
				if (data.tokenID != "")
				{
					data.hasTokenID = true;
					data.hasJsonData = true;
				}
			}
			catch (const std::exception&) { data.hasTokenID = false; }

			try
			{
				std::string uniqueidMatchID = jsonValue["uniqueidmatch"].asString();
				if (uniqueidMatchID != "")
				{
					bool uniqueidmatch = jsonValue["uniqueidmatch"].asBool();
					data.uniqueidMatch = uniqueidmatch;
					data.hasUniqueidMatch = true;
					data.hasJsonData = true;
				}
			}
			catch (const std::exception&)
			{
				data.hasUniqueidMatch = false;
				data.uniqueidMatch = false;
			}
		}
	}
	catch (const std::exception&) {}
	return data;
}

/// <summary>
/// Read the JSON data.
/// </summary>
/// <param name="document">The JSON document.</param>
/// <return>The JSON data.</return>
WebSocketJsonClientLocation WebSocketJson::ReadJsonClientLocation(const std::string& document)
{
	WebSocketJsonClientLocation data;

	try
	{
		Json::Reader jsonReader;
		Json::Value jsonValue;

		// Parse the file.
		bool result = jsonReader.parse(document, jsonValue, false);
		if (result)
		{
			try
			{
				std::string uniqueIDID = jsonValue["uniqueid"].asString();
				data.uniqueID = uniqueIDID;
				if (data.uniqueID != "")
				{
					data.hasUniqueID = true;
					data.hasJsonData = true;
				}
			}
			catch (const std::exception&) { data.hasUniqueID = false; }

			try
			{
				std::string addressID = jsonValue["address"].asString();
				data.address = addressID;
				if (data.address != "")
				{
					data.hasAddress = true;
					data.hasJsonData = true;
				}
			}
			catch (const std::exception&) { data.hasAddress = false; }

			try
			{
				std::string maskID = jsonValue["mask"].asString();
				data.mask = maskID;
				if (data.mask != "")
				{
					data.hasMask = true;
					data.hasJsonData = true;
				}
			}
			catch (const std::exception&) { data.hasMask = false; }
		}
	}
	catch (const std::exception&) {}
	return data;
}

/// <summary>
/// Write the JSON data.
/// </summary>
/// <param name="document">The JSON document.</param>
/// <param name="currentData">The current data used to compare.</param>
/// <param name="avaliable">Is avaliable.</param>
/// <return>The JSON data.</return>
const std::string WebSocketJson::WriteJson(const std::string& document, const WebSocketJsonData& currentData, bool avaliable)
{
	std::string json = "";

	try
	{
		Json::StreamWriterBuilder jsonWriterBuilder;
		Json::Reader jsonReader;
		Json::Value jsonValue;

		// Parse the file.
		bool result = jsonReader.parse(document, jsonValue, false);
		if (result)
		{
			// Apply builder settings.
			jsonWriterBuilder.settings_["commentStyle"] = "None";
			jsonWriterBuilder.settings_["indentation"] = " ";

			// Create the writers.
			std::unique_ptr<Json::StreamWriter> jsonWriter(jsonWriterBuilder.newStreamWriter());
			std::unique_ptr<std::ostringstream> jsonStream(new std::ostringstream());

			// Add JSON elements.
			jsonValue["response"] = "ok";
			jsonValue["available"] = avaliable;
			jsonValue["contactUniqueID"] = currentData.uniqueID;
			jsonValue["contactApplicationID"] = currentData.applicationID;
			jsonWriter->write(jsonValue, jsonStream.get());

			// Create the json string from the stream.
			json = jsonStream->str();
		}
		else
		{
			json = "{\"response\":\"error\",\"error\":\"Unable to read request\"}";
		}
	}
	catch (const std::exception&)
	{
		json = "{\"response\":\"error\",\"error\":\"Unable to read request\"}";
	}

	// Return the JSON data.
	return json;
}

/// <summary>
/// Write the JSON data.
/// </summary>
/// <param name="document">The JSON document.</param>
/// <param name="currentData">The current data used to compare.</param>
/// <return>The JSON data.</return>
const std::string WebSocketJson::WriteJson(const std::string& document, const WebSocketJsonData& currentData)
{
	std::string json = "";

	try
	{
		Json::StreamWriterBuilder jsonWriterBuilder;
		Json::Reader jsonReader;
		Json::Value jsonValue;

		// Parse the file.
		bool result = jsonReader.parse(document, jsonValue, false);
		if (result)
		{
			// Apply builder settings.
			jsonWriterBuilder.settings_["commentStyle"] = "None";
			jsonWriterBuilder.settings_["indentation"] = " ";

			// Create the writers.
			std::unique_ptr<Json::StreamWriter> jsonWriter(jsonWriterBuilder.newStreamWriter());
			std::unique_ptr<std::ostringstream> jsonStream(new std::ostringstream());

			// Add JSON elements.
			jsonValue["response"] = currentData.response;
			jsonValue["error"] = currentData.error;
			jsonValue["serverID"] = currentData.serverID;
			jsonValue["available"] = currentData.available;
			jsonValue["uniqueID"] = currentData.uniqueID;
			jsonValue["applicationID"] = currentData.applicationID;
			jsonValue["contactUniqueID"] = currentData.contactUniqueID;
			jsonValue["contactApplicationID"] = currentData.contactApplicationID;
			jsonWriter->write(jsonValue, jsonStream.get());

			// Create the json string from the stream.
			json = jsonStream->str();
		}
		else
		{
			json = "{\"response\":\"error\",\"error\":\"Unable to read request\"}";
		}
	}
	catch (const std::exception&)
	{
		json = "{\"response\":\"error\",\"error\":\"Unable to read request\"}";
	}

	// Return the JSON data.
	return json;
}

/// <summary>
/// Write the JSON data.
/// </summary>
/// <param name="document">The JSON document.</param>
/// <param name="avaliable">Is avaliable.</param>
/// <return>The JSON data.</return>
const std::string WebSocketJson::WriteJson(const std::string& document, bool avaliable)
{
	std::string json = "";

	try
	{
		Json::StreamWriterBuilder jsonWriterBuilder;
		Json::Reader jsonReader;
		Json::Value jsonValue;

		// Parse the file.
		bool result = jsonReader.parse(document, jsonValue, false);
		if (result)
		{
			// Apply builder settings.
			jsonWriterBuilder.settings_["commentStyle"] = "None";
			jsonWriterBuilder.settings_["indentation"] = " ";

			// Create the writers.
			std::unique_ptr<Json::StreamWriter> jsonWriter(jsonWriterBuilder.newStreamWriter());
			std::unique_ptr<std::ostringstream> jsonStream(new std::ostringstream());

			// Add JSON elements.
			jsonValue["response"] = "ok";
			jsonValue["available"] = avaliable;
			jsonWriter->write(jsonValue, jsonStream.get());

			// Create the json string from the stream.
			json = jsonStream->str();
		}
		else
		{
			json = "{\"response\":\"error\",\"error\":\"Unable to read request\"}";
		}
	}
	catch (const std::exception&)
	{
		json = "{\"response\":\"error\",\"error\":\"Unable to read request\"}";
	}

	// Return the JSON data.
	return json;
}