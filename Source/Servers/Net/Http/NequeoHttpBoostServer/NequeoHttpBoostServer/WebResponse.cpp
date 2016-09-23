/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WebResponse.cpp
*  Purpose :       Http web response class.
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

#include "WebResponse.h"

using namespace Nequeo::Net::Http;

void ExWriteHeaders(WebResponse*, InternalHttpServer::Response*);
void ExWriteHeadersSecure(WebResponse*, InternalSecureHttpServer::Response*);

void ExWriteContent(WebResponse*, InternalHttpServer::Response*, std::streambuf*);
void ExWriteContentSecure(WebResponse*, InternalSecureHttpServer::Response*, std::streambuf*);

///	<summary>
///	Http web response.
///	</summary>
WebResponse::WebResponse() :
	_disposed(false)
{
}

///	<summary>
///	Http web response.
///	</summary>
WebResponse::~WebResponse()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	Write the response headers.
///	</summary>
void WebResponse::WriteHeaders()
{
	// If not secure.
	if (!isSecure)
	{
		//	Write the response headers.
		ExWriteHeaders(this, static_cast<InternalHttpServer::Response*>(internalResponse));
	}
	else
	{
		//	Write the response headers.
		ExWriteHeadersSecure(this, static_cast<InternalSecureHttpServer::Response*>(internalResponse));
	}
}

///	<summary>
///	Write the response headers.
///	</summary>
/// <param name="content">The content to write.</param>
void WebResponse::WriteContent(std::streambuf* content)
{
	// If not secure.
	if (!isSecure)
	{
		//	Write the response content.
		ExWriteContent(this, static_cast<InternalHttpServer::Response*>(internalResponse), content);
	}
	else
	{
		//	Write the response content.
		ExWriteContentSecure(this, static_cast<InternalSecureHttpServer::Response*>(internalResponse), content);
	}
}

///	<summary>
///	Write the response headers.
///	</summary>
/// <param name="webResponse">The web response.</param>
/// <param name="response">The response stream.</param>
void ExWriteHeaders(WebResponse* webResponse, InternalHttpServer::Response* response)
{
	// Get headers.
	typedef std::pair<std::string, std::string> headerPair;
	std::map<std::string, std::string>* headers = &webResponse->GetHeaders();

	// If exists.
	if (webResponse->GetContentEncoding().length() > 0)
	{
		// Add the header.
		webResponse->AddHeader("Content-Encoding", webResponse->GetContentEncoding());
	}

	// If exists.
	if (webResponse->GetContentLength() > 0)
	{
		// Add the header.
		webResponse->AddHeader("Content-Length", std::to_string(webResponse->GetContentLength()));
	}

	// If exists.
	if (webResponse->GetContentType().length() > 0)
	{
		// Add the header.
		webResponse->AddHeader("Content-Type", webResponse->GetContentType());
	}

	// Write the response.
	*response << webResponse->GetProtocolVersion() << " " << webResponse->GetStatusCode() << (webResponse->GetStatusSubCode() > 0 ? "." + webResponse->GetStatusSubCode() : "") << " " << webResponse->GetStatusDescription() << "\r\n";

	// For each header in the list.
	// For each header.
	for (auto& h : *headers)
	{
		// Write header name and value.
		*response << h.first << ": " << h.second << "\r\n";
	}

	// Write the last '\r\n'
	*response << "\r\n";
}

///	<summary>
///	Write the response headers.
///	</summary>
/// <param name="webResponse">The web response.</param>
/// <param name="response">The response stream.</param>
void ExWriteHeadersSecure(WebResponse* webResponse, InternalSecureHttpServer::Response* response)
{
	// Get headers.
	typedef std::pair<std::string, std::string> headerPair;
	std::map<std::string, std::string>* headers = &webResponse->GetHeaders();

	// If exists.
	if (webResponse->GetContentEncoding().length() > 0)
	{
		// Add the header.
		webResponse->AddHeader("Content-Encoding", webResponse->GetContentEncoding());
	}

	// If exists.
	if (webResponse->GetContentLength() > 0)
	{
		// Add the header.
		webResponse->AddHeader("Content-Length", std::to_string(webResponse->GetContentLength()));
	}

	// If exists.
	if (webResponse->GetContentType().length() > 0)
	{
		// Add the header.
		webResponse->AddHeader("Content-Type", webResponse->GetContentType());
	}

	// Write the response.
	*response << webResponse->GetProtocolVersion() << " " << webResponse->GetStatusCode() << (webResponse->GetStatusSubCode() > 0 ? "." + webResponse->GetStatusSubCode() : "") << " " << webResponse->GetStatusDescription() << "\r\n";

	// For each header in the list.
	// For each header.
	for (auto& h : *headers)
	{
		// Write header name and value.
		*response << h.first << ": " << h.second << "\r\n";
	}

	// Write the last '\r\n'
	*response << "\r\n";
}

///	<summary>
///	Write the response content.
///	</summary>
/// <param name="webResponse">The web response.</param>
/// <param name="response">The response stream.</param>
/// <param name="content">The response content.</param>
void ExWriteContent(WebResponse* webResponse, InternalHttpServer::Response* response, std::streambuf* content)
{
	// If content exists.
	if (content != nullptr)
	{
		// Write the content.
		*response << content;
	}
}

///	<summary>
///	Write the response content.
///	</summary>
/// <param name="webResponse">The web response.</param>
/// <param name="response">The response stream.</param>
/// <param name="content">The response content.</param>
void ExWriteContentSecure(WebResponse* webResponse, InternalSecureHttpServer::Response* response, std::streambuf* content)
{
	// If content exists.
	if (content != nullptr)
	{
		// Write the content.
		*response << content;
	}
}