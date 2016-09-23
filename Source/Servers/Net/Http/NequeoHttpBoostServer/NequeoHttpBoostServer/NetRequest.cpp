/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          NetRequest.cpp
*  Purpose :       Http net request class.
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

#include "NetRequest.h"

using namespace Nequeo::Net::Http;

void ExWriteRequest(NetRequest*, InternalHttpClient*);
void ExWriteRequest(NetRequest*, InternalHttpClient*, const std::string&);
void ExWriteRequest(NetRequest*, InternalHttpClient*, std::iostream&);

void ExWriteRequestSecure(NetRequest*, InternalSecureHttpClient*);
void ExWriteRequestSecure(NetRequest*, InternalSecureHttpClient*, const std::string&);
void ExWriteRequestSecure(NetRequest*, InternalSecureHttpClient*, std::iostream&);

///	<summary>
///	Http net request.
///	</summary>
NetRequest::NetRequest() :
	_disposed(false)
{

}

///	<summary>
///	Http net request.
///	</summary>
NetRequest::~NetRequest()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	Write the request.
///	</summary>
void NetRequest::WriteRequest()
{
	//	Write the request headers.
	WriteHeaders();

	// If not secure.
	if (!isSecure)
	{
		//	Write the request.
		ExWriteRequest(this, static_cast<InternalHttpClient*>(internalRequest));
	}
	else
	{
		//	Write the request.
		ExWriteRequestSecure(this, static_cast<InternalSecureHttpClient*>(internalRequest));
	}
}

///	<summary>
///	Write the request with content.
///	</summary>
/// <param name="content">The content to write.</param>
void NetRequest::WriteRequest(const std::string& content)
{
	//	Write the request headers.
	WriteHeaders();

	// If not secure.
	if (!isSecure)
	{
		//	Write the request.
		ExWriteRequest(this, static_cast<InternalHttpClient*>(internalRequest), content);
	}
	else
	{
		//	Write the request.
		ExWriteRequestSecure(this, static_cast<InternalSecureHttpClient*>(internalRequest), content);
	}
}

///	<summary>
///	Write the request with content.
///	</summary>
/// <param name="content">The content to write.</param>
void NetRequest::WriteRequest(std::iostream& content)
{
	//	Write the request headers.
	WriteHeaders();

	// If not secure.
	if (!isSecure)
	{
		//	Write the request.
		ExWriteRequest(this, static_cast<InternalHttpClient*>(internalRequest), content);
	}
	else
	{
		//	Write the request.
		ExWriteRequestSecure(this, static_cast<InternalSecureHttpClient*>(internalRequest), content);
	}
}

///	<summary>
///	Write the request headers.
///	</summary>
void NetRequest::WriteHeaders()
{
	// If exists.
	if (GetAcceptEncoding().length() > 0)
	{
		// Add the header.
		AddHeader("Accept-Encoding", GetAcceptEncoding());
	}

	// If exists.
	if (GetContentLength() > 0)
	{
		// Add the header.
		AddHeader("Content-Length", std::to_string(GetContentLength()));
	}

	// If exists.
	if (GetContentType().length() > 0)
	{
		// Add the header.
		AddHeader("Content-Type", GetContentType());
	}
}

///	<summary>
///	Write the request content.
///	</summary>
/// <param name="request">The net request.</param>
/// <param name="client">The net client.</param>
void ExWriteRequest(NetRequest* request, InternalHttpClient* client)
{
	client->request(request->GetMethod(), request->GetPath(), "", request->GetHeaders());
}

///	<summary>
///	Write the request content.
///	</summary>
/// <param name="request">The net request.</param>
/// <param name="client">The net client.</param>
/// <param name="content">The request content.</param>
void ExWriteRequest(NetRequest* request, InternalHttpClient* client, const std::string& content)
{
	client->request(request->GetMethod(), request->GetPath(), content, request->GetHeaders());
}

///	<summary>
///	Write the request content.
///	</summary>
/// <param name="request">The net request.</param>
/// <param name="client">The net client.</param>
/// <param name="content">The request content.</param>
void ExWriteRequest(NetRequest* request, InternalHttpClient* client, std::iostream& content)
{
	client->request(request->GetMethod(), request->GetPath(), content, request->GetHeaders());
}

///	<summary>
///	Write the request content.
///	</summary>
/// <param name="request">The net request.</param>
/// <param name="client">The net client.</param>
void ExWriteRequestSecure(NetRequest* request, InternalSecureHttpClient* client)
{
	client->request(request->GetMethod(), request->GetPath(), "", request->GetHeaders());
}

///	<summary>
///	Write the request content.
///	</summary>
/// <param name="request">The net request.</param>
/// <param name="client">The net client.</param>
/// <param name="content">The request content.</param>
void ExWriteRequestSecure(NetRequest* request, InternalSecureHttpClient* client, const std::string& content)
{
	client->request(request->GetMethod(), request->GetPath(), content, request->GetHeaders());
}

///	<summary>
///	Write the request content.
///	</summary>
/// <param name="request">The net request.</param>
/// <param name="client">The net client.</param>
/// <param name="content">The request content.</param>
void ExWriteRequestSecure(NetRequest* request, InternalSecureHttpClient* client, std::iostream& content)
{
	client->request(request->GetMethod(), request->GetPath(), content, request->GetHeaders());
}