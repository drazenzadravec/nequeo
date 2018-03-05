/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WebSocketJson.h
*  Purpose :       Web socket JSON class.
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

namespace Nequeo {
	namespace Net {
		namespace WebSocket
		{
			/// <summary>
			/// JSON data.
			/// </summary>
			class WebSocketJsonData
			{
			public:
				/// <summary>
				/// JSON data.
				/// </summary>
				WebSocketJsonData() : _disposed(false), hasJsonData(false),
					uniqueID(""), applicationID(""), available(false), broadcast(false), broadcastAppID(false),
					hasUniqueID(false), hasApplicationID(false), hasAvailable(false), hasBroadcast(false), hasBroadcastAppID(false),
					contactUniqueID(""), contactApplicationID(""), hasContactUniqueID(false), hasContactApplicationID(false),
					hasResponse(false), response(""), hasError(false), error(""), hasServerID(false), serverID(""),
					hasAccessToken(false), accessToken("") {}

				/// <summary>
				/// JSON data.
				/// </summary>
				~WebSocketJsonData() {}

				bool hasJsonData;

				bool hasUniqueID;
				std::string uniqueID;

				bool hasApplicationID;
				std::string applicationID;

				bool hasAvailable;
				bool available;

				bool hasBroadcast;
				bool broadcast;

				bool hasBroadcastAppID;
				bool broadcastAppID;

				bool hasContactUniqueID;
				std::string contactUniqueID;

				bool hasContactApplicationID;
				std::string contactApplicationID;

				bool hasResponse;
				std::string response;

				bool hasError;
				std::string error;

				bool hasServerID;
				std::string serverID;

				bool hasAccessToken;
				std::string accessToken;

			private:
				bool _disposed;

			};

			/// <summary>
			/// JSON data.
			/// </summary>
			class WebSocketJsonAccessToken
			{
			public:
				/// <summary>
				/// JSON data.
				/// </summary>
				WebSocketJsonAccessToken() : _disposed(false), hasJsonData(false),
					uniqueID(""), hasUniqueID(false), grant(false), hasGrant(false),
					hasExpiry(false), expiry(1), hasTokenID(false), tokenID(""),
					hasUniqueidMatch(false), uniqueidMatch(false) {}

				/// <summary>
				/// JSON data.
				/// </summary>
				~WebSocketJsonAccessToken() {}

				bool hasJsonData;

				bool hasUniqueID;
				std::string uniqueID;

				bool hasGrant;
				bool grant;

				bool hasExpiry;
				unsigned int expiry;

				bool hasTokenID;
				std::string tokenID;

				bool hasUniqueidMatch;
				bool uniqueidMatch;

			private:
				bool _disposed;

			};

			/// <summary>
			/// JSON data.
			/// </summary>
			class WebSocketJsonClientLocation
			{
			public:
				/// <summary>
				/// JSON data.
				/// </summary>
				WebSocketJsonClientLocation() : _disposed(false), hasJsonData(false),
					uniqueID(""), hasUniqueID(false), address(""), hasAddress(false),
					hasMask(false), mask("") {}

				/// <summary>
				/// JSON data.
				/// </summary>
				~WebSocketJsonClientLocation() {}

				bool hasJsonData;

				bool hasUniqueID;
				std::string uniqueID;

				bool hasAddress;
				std::string address;

				bool hasMask;
				std::string mask;

			private:
				bool _disposed;

			};

			/// <summary>
			/// JSON reader.
			/// </summary>
			class WebSocketJson
			{
			public:
				/// <summary>
				/// JSON reader.
				/// </summary>
				WebSocketJson() : _disposed(false) {}

				/// <summary>
				/// JSON reader.
				/// </summary>
				~WebSocketJson() {}

				/// <summary>
				/// Read the JSON data.
				/// </summary>
				/// <param name="document">The JSON document.</param>
				/// <param name="currentData">The current data used to compare.</param>
				/// <return>The JSON data.</return>
				WebSocketJsonData ReadJson(const std::string& document, const WebSocketJsonData& currentData);

				/// <summary>
				/// Read the JSON data.
				/// </summary>
				/// <param name="document">The JSON document.</param>
				/// <return>The JSON data.</return>
				WebSocketJsonAccessToken ReadJsonAccessToken(const std::string& document);

				/// <summary>
				/// Read the JSON data.
				/// </summary>
				/// <param name="document">The JSON document.</param>
				/// <return>The JSON data.</return>
				WebSocketJsonClientLocation ReadJsonClientLocation(const std::string& document);

				/// <summary>
				/// Write the JSON data.
				/// </summary>
				/// <param name="document">The JSON document.</param>
				/// <param name="currentData">The current data used to compare.</param>
				/// <param name="avaliable">Is avaliable.</param>
				/// <return>The JSON data.</return>
				const std::string WriteJson(const std::string& document, const WebSocketJsonData& currentData, bool avaliable);

				/// <summary>
				/// Write the JSON data.
				/// </summary>
				/// <param name="document">The JSON document.</param>
				/// <param name="currentData">The current data used to compare.</param>
				/// <return>The JSON data.</return>
				const std::string WriteJson(const std::string& document, const WebSocketJsonData& currentData);

				/// <summary>
				/// Write the JSON data.
				/// </summary>
				/// <param name="document">The JSON document.</param>
				/// <param name="avaliable">Is avaliable.</param>
				/// <return>The JSON data.</return>
				const std::string WriteJson(const std::string& document, bool avaliable);

			private:
				bool _disposed;

			};
		}
	}
}