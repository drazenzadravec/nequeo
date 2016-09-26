/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          HttpServer.cpp
*  Purpose :       Http web server class.
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

#include "HttpServer.h"
#include "NequeoHttpBoostServer\MimeType.h"
#include "Base\StringEx.h"
#include "Base\StringUtils.h"

using namespace Nequeo::Net::Http;
using namespace Nequeo::Net::Mime;

std::atomic<const char*> rootPath;
void OnHttpContext(const WebContext*);
bool OpenFile(const std::string&, WebResponse&, const std::string&);

/// <summary>
/// Http web server.
/// </summary>
/// <param name="path">The root folder path (this is where all files are located).</param>
HttpServer::HttpServer(const std::string& path) :
	_disposed(false), _path(path), _initialised(false)
{
}

///	<summary>
///	Http web server.
///	</summary>
HttpServer::~HttpServer()
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
void HttpServer::Initialise()
{
	// If not initialised.
	if (!_initialised)
	{
		// Create the new array of servers.
		_servers = std::make_shared<MultiWebServer>(_containers);
		_servers->OnWebContext(OnHttpContext);
		_initialised = true;

		// Set the root path.
		rootPath = _path.c_str();
	}
}

///	<summary>
///	Start the servers.
///	</summary>
void HttpServer::Start()
{
	// If initialised.
	if (_initialised)
	{
		_servers->Start();
	}
}

///	<summary>
///	Stop the servers.
///	</summary>
void HttpServer::Stop()
{
	// If initialised.
	if (_initialised)
	{
		_servers->Stop();
	}
}

/// <summary>
/// On http context request.
/// </summary>
/// <param name="context">The web context.</param>
void OnHttpContext(const WebContext* context)
{
	// All requests will come into this function.
	try
	{
		// Get the request and response.
		WebRequest request = context->Request();
		WebResponse response = context->Response();

		// Get the method.
		std::string method;
		std::string methodSource = request.GetMethod();

		// Make the method to lower case.
		const char* source = methodSource.c_str();
		size_t sourceLength = strlen(source);
		method.resize(sourceLength);
		std::transform(source, source + sourceLength, method.begin(), ::tolower);

		// Only accept GET.
		if (method == "get")
		{
			// Get the root path.
			std::string root = rootPath;

			// Get the request values.
			std::string path = request.GetPath();
			std::string contentType = request.GetContentType();
			std::string acceptEncoding = request.GetAcceptEncoding();
			unsigned long long contentLength = request.GetContentLength();

			// Split the query if any.
			Nequeo::String splitPath(path.c_str());
			Nequeo::Vector<Nequeo::String> pathAndQuery = Nequeo::StringUtils::Split(splitPath, '?');

			// If the starting "/" exists.
			std::string requestPathNoBackSlash = pathAndQuery[0].c_str();
			std::string requestPath(pathAndQuery[0].c_str());

			// Find the index.
			std::basic_string <char>::size_type index;
			index = requestPath.find("/");

			// If a "/" has been found.
			if (index != std::string::npos)
			{
				// If the "/" is at position 0.
				if (index == 0)
				{
					// Move to the second char, this is index 0 based.
					requestPathNoBackSlash = requestPath.substr(1);
				}
			}

			// Replace the string.
			Nequeo::String currentPath(requestPathNoBackSlash.c_str());
			std::string search = "/";
			std::string replace = "\\";
			Nequeo::StringUtils::Replace(currentPath, search.c_str(), replace.c_str());
			std::string replacedPath(currentPath.c_str());

			// Get the file file path.
			std::string file = root + "\\" + replacedPath;
			std::string extension = ".html";

			// If an extension has been found.
			if (file.find_last_of(".") != std::string::npos)
			{
				// Get the extension.
				extension = file.substr(file.find_last_of(".") + 1);
			}
			
			// Open the file.
			std::ifstream fileStream;
			std::string backSlash = "\\";

			// Attempt to open the file.
			bool found = OpenFile(file, response, extension);

			// Could not open file.
			if (!found)
			{
				// Try default.htm.
				std::string file1Ex(Nequeo::StringUtils::RTrim(replacedPath.c_str(), backSlash.c_str()).c_str());
				std::string file1 = root + "\\" + (file1Ex.length() > 0 ? file1Ex + "\\" : "") + "default.htm";
				bool found = OpenFile(file1, response, extension);

				// Could not open file.
				if (!found)
				{
					// Try default.html.
					std::string file2Ex(Nequeo::StringUtils::RTrim(replacedPath.c_str(), backSlash.c_str()).c_str());
					std::string file2 = root + "\\" + (file2Ex.length() > 0 ? file2Ex + "\\" : "") + "default.html";
					bool found = OpenFile(file2, response, extension);

					// Could not open file.
					if (!found)
					{
						// Try index.htm.
						std::string file3Ex(Nequeo::StringUtils::RTrim(replacedPath.c_str(), backSlash.c_str()).c_str());
						std::string file3 = root + "\\" + (file3Ex.length() > 0 ? file3Ex + "\\" : "") + "index.htm";
						bool found = OpenFile(file3, response, extension);

						// Could not open file.
						if (!found)
						{
							// Try index.html.
							std::string file4Ex(Nequeo::StringUtils::RTrim(replacedPath.c_str(), backSlash.c_str()).c_str());
							std::string file4 = root + "\\" + (file4Ex.length() > 0 ? file4Ex + "\\" : "") + "index.html";
							bool found = OpenFile(file4, response, extension);
						}
					}
				}
			}

			// If the file could be opened.
			if (!found)
			{
				// Send error.
				response.SetStatusCode(401);
				response.SetStatusDescription("Can not find resource");
				response.SetContentType("text/html");
				response.SetContentLength(0);
				response.WriteHeaders();
			}
		}
		else
		{
			// Send error.
			response.SetStatusCode(500);
			response.SetStatusDescription("Internal Server Error");
			response.SetContentType("text/html");
			response.SetContentLength(0);
			response.WriteHeaders();
		}
	}
	catch (...) {}
}

/// <summary>
/// Open the file.
/// </summary>
/// <param name="file">The file stream.</param>
/// <param name="response">The response.</param>
/// <param name="extension">The extension.</param>
bool OpenFile(const std::string& file, WebResponse& response, const std::string& extension)
{
	bool ret = false;

	try
	{
		// Open the file.
		std::ifstream fileStream(file, std::istream::binary);

		// If file exists.
		if (fileStream.good())
		{
			// If the file could be opened.
			if (!fileStream.bad())
			{
				try
				{
					MimeType mime;

					// Seek to the begining of the file
					// and get the file size.
					fileStream.seekg(0, fileStream.end);
					long long length = fileStream.tellg();

					// Seek back to the begining of the file.
					fileStream.seekg(0, fileStream.beg);

					// Write response.
					response.SetContentType(mime.GetMimeType(extension));
					response.SetContentLength(length);

					// Write the headers.
					response.WriteHeaders();

					// Write the content.
					response.WriteContent(fileStream.rdbuf());
					ret = true;
				}
				catch (...) { ret = false; }

				// Close the file stream.
				fileStream.close();
				return ret;
			}
			else
			{
				// Could not open the file.
				return ret;
			}
		}
		else
		{
			// Could not open the file.
			return ret;
		}
	}
	catch (...) { ret = false; }

	// Could not open the file.
	return ret;
}