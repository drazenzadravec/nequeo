/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          NetResponse.cpp
*  Purpose :       Http net response class.
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

#include "stdafx.h"
#include "stdafx.cpp"

#include "NetResponse.h"

using namespace Nequeo::Net::Http;

void ExReadResponse(NetResponse*, InternalHttpClient*);
void ExReadResponseSecure(NetResponse*, InternalSecureHttpClient*);

///	<summary>
///	Http net response.
///	</summary>
NetResponse::NetResponse() :
	_disposed(false)
{

}

///	<summary>
///	Http net response.
///	</summary>
NetResponse::~NetResponse()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Read the response.
/// </summary>
void NetResponse::ReadResponse()
{
	// If not secure.
	if (!isSecure)
	{
		//	Read the response.
		ExReadResponse(this, static_cast<InternalHttpClient*>(internalResponse));
	}
	else
	{
		//	Read the response.
		ExReadResponseSecure(this, static_cast<InternalSecureHttpClient*>(internalResponse));
	}
}

///	<summary>
///	Read the response content.
///	</summary>
/// <param name="netResponse">The net response.</param>
/// <param name="client">The net client.</param>
void ExReadResponse(NetResponse* netResponse, InternalHttpClient* client)
{
	// Read the response.
	std::shared_ptr<InternalHttpClient::Response> response = client->request_read();
	netResponse->SetProtocolVersion(response->http_version);
	netResponse->SetStatusCode(std::stoi(response->status_code));
	
	// Assign the headers.
	std::map<std::string, std::string> headers;
	typedef std::pair<std::string, std::string> headerPair;

	// For each header.
	for (auto& h : response->header)
	{
		// Write header name and value.
		headers.insert(headerPair(h.first.c_str(), h.second.c_str()));
	}

	// Assign the header and content.
	netResponse->SetHeaders(headers);
	netResponse->Content = &response->content;

	// Find the 'Content-Length' header.
	auto content_length_it = headers.find("Content-Length");
	if (content_length_it != headers.end())
	{
		// Get the 'Content-Length' header value.
		auto content_length = std::stoull(content_length_it->second);
		netResponse->SetContentLength(content_length);
	}

	// Find the 'Content-Type' header.
	auto content_type_it = headers.find("Content-Type");
	if (content_type_it != headers.end())
	{
		// Get the 'Content-Type' header value.
		auto content_type = content_type_it->second;
		netResponse->SetContentType(content_type);
	}

	// Find the 'Content-Encoding' header.
	auto content_encoding_it = headers.find("Content-Encoding");
	if (content_encoding_it != headers.end())
	{
		// Get the 'Content-Encoding' header value.
		auto content_encoding = content_encoding_it->second;
		netResponse->SetContentEncoding(content_encoding);
	}
}

//	<summary>
///	Read the response content.
///	</summary>
/// <param name="netResponse">The net response.</param>
/// <param name="client">The net client.</param>
void ExReadResponseSecure(NetResponse* netResponse, InternalSecureHttpClient* client)
{
	// Read the response.
	std::shared_ptr<InternalSecureHttpClient::Response> response = client->request_read();
	netResponse->SetProtocolVersion(response->http_version);
	netResponse->SetStatusCode(std::stoi(response->status_code));

	// Assign the headers.
	std::map<std::string, std::string> headers;
	typedef std::pair<std::string, std::string> headerPair;

	// For each header.
	for (auto& h : response->header)
	{
		// Write header name and value.
		headers.insert(headerPair(h.first.c_str(), h.second.c_str()));
	}

	// Assign the header and content.
	netResponse->SetHeaders(headers);
	netResponse->Content = &response->content;

	// Find the 'Content-Length' header.
	auto content_length_it = headers.find("Content-Length");
	if (content_length_it != headers.end())
	{
		// Get the 'Content-Length' header value.
		auto content_length = std::stoull(content_length_it->second);
		netResponse->SetContentLength(content_length);
	}

	// Find the 'Content-Type' header.
	auto content_type_it = headers.find("Content-Type");
	if (content_type_it != headers.end())
	{
		// Get the 'Content-Type' header value.
		auto content_type = content_type_it->second;
		netResponse->SetContentType(content_type);
	}

	// Find the 'Content-Encoding' header.
	auto content_encoding_it = headers.find("Content-Encoding");
	if (content_encoding_it != headers.end())
	{
		// Get the 'Content-Encoding' header value.
		auto content_encoding = content_encoding_it->second;
		netResponse->SetContentEncoding(content_encoding);
	}
}