/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          ConfigModel.cpp
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

#include "ConfigModel.h"

#include <json\json.h>

using namespace Nequeo::Net::Http;

/// <summary>
/// Read the condifuration file contents.
/// </summary>
void ConfigModel::ReadConfigFile()
{
	// Open the file.
	std::ifstream configFileStream(_path);

	// If the file could be opened.
	if (!configFileStream.bad())
	{
		try
		{
			Json::Reader jsonReader;
			Json::Value jsonValue;

			// Seek to the begining of the file
			// and get the file size.
			configFileStream.seekg(0, configFileStream.end);
			int length = configFileStream.tellg();

			// Seek back to the begining of the file.
			configFileStream.seekg(0, configFileStream.beg);

			// Parse the file.
			bool result = jsonReader.parse(configFileStream, jsonValue, false);
			if (result)
			{
				// Get the json values.
				_rootPath = jsonValue["rootpath"].asString();

				// Read the array of web servers.
				const Json::Value& webservers = jsonValue["webservers"];

				// For each web server.
				for (int i = 0; i < webservers.size(); i++)
				{
					// Assign each server details.
					MultiServerContainer server;
					server.SetNumberOfThreads(webservers[i]["numberofthreads"].asInt());
					server.SetPort(webservers[i]["port"].asUInt());
					server.SetEndpoint(webservers[i]["endpoint"].asString());
					server.SetIsSecure(webservers[i]["issecure"].asBool());
					server.SetPublicKeyFile(webservers[i]["publickeyfile"].asString());
					server.SetPrivateKeyFile(webservers[i]["privatekeyfile"].asString());
					server.SetPrivateKeyPassword(webservers[i]["privatekeypassword"].asString());
					server.SetTimeoutRequest(webservers[i]["timeoutrequest"].asInt64());
					server.SetTimeoutContent(webservers[i]["timeoutcontent"].asInt64());

					// Get the ip version.
					std::string ipv;
					std::string ipvSource = webservers[i]["ipversion"].asString();

					// Make the ipv source to lower case.
					const char* source = ipvSource.c_str();
					size_t sourceLength = strlen(source);
					ipv.resize(sourceLength);
					std::transform(source, source + sourceLength, ipv.begin(), ::tolower);

					// If IPv4.
					if (ipv == "ipv4")
					{
						// Set to IPv4.
						server.SetIPversion(Nequeo::Net::Http::IPVersionType::IPv4);
					}

					// If IPv6.
					if (ipv == "ipv6")
					{
						// Set to IPv6.
						server.SetIPversion(Nequeo::Net::Http::IPVersionType::IPv6);
					}

					// Add the server details to the container.
					_containers.push_back(server);
				}
			}
		}
		catch (const std::exception&) {}
		
		// Close the file stream.
		configFileStream.close();
	}
}