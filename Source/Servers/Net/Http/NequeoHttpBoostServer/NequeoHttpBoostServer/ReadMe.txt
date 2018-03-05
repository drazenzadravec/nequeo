========================================================================
    STATIC LIBRARY : NequeoHttpBoostServer Project Overview
========================================================================

AppWizard has created this NequeoHttpBoostServer library project for you.

This file contains a summary of what you will find in each of the files that
make up your NequeoHttpBoostServer application.


NequeoHttpBoostServer.vcxproj
    This is the main project file for VC++ projects generated using an Application Wizard.
    It contains information about the version of Visual C++ that generated the file, and
    information about the platforms, configurations, and project features selected with the
    Application Wizard.

NequeoHttpBoostServer.vcxproj.filters
    This is the filters file for VC++ projects generated using an Application Wizard. 
    It contains information about the association between the files in your project 
    and the filters. This association is used in the IDE to show grouping of files with
    similar extensions under a specific node (for e.g. ".cpp" files are associated with the
    "Source Files" filter).


/////////////////////////////////////////////////////////////////////////////

StdAfx.h, StdAfx.cpp
    These files are used to build a precompiled header (PCH) file
    named NequeoHttpBoostServer.pch and a precompiled types file named StdAfx.obj.

/////////////////////////////////////////////////////////////////////////////
Other notes:

AppWizard uses "TODO:" comments to indicate parts of the source code you
should add to or customize.

/////////////////////////////////////////////////////////////////////////////

// TestHttpServer.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <iostream>
#include <sstream>
#include <string>
#include <thread>

#include "WebClient.h"
#include "WebServer.h"
#include "WebContext.h"
#include "IPVersionType.h"
#include "NetResponse.h"
#include "MimeType.h"

using namespace Nequeo::Net::Http;
using namespace Nequeo::Net::Mime;

void OnWebContext(const WebContext*);
void AsyncClient(const WebClient*, const NetResponse&, const std::shared_ptr<const Nequeo::AsyncCallerContext>&);

int main()
{
	WebClient client("www.google.com", 8800, false, IPVersionType::IPv6);
	client.RequestAsync("GET", "/", AsyncClient);

	WebServer server(333, IPVersionType::IPv6);
	server.OnWebContext(OnWebContext);
	server.StartThread();
	std::cout << "Server started" << std::endl;

    return 0;
}

void AsyncClient(const WebClient* client, const NetResponse& response, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	std::cout << "In Here." << std::endl;
	std::cout << response.Content->rdbuf() << std::endl;
	std::cout << response.GetContentLength() << std::endl;
}

void OnWebContext(const WebContext* context)
{
	
	std::cout << "GOT a request." << std::endl;
	std::cout << context->Request().GetMethod().c_str() << std::endl;
	std::cout << context->Request().GetPath().c_str() << std::endl;
	std::cout << context->GetPort() << std::endl;
	std::cout << context->IsSecure() << std::endl;
	std::cout << context->GetServerName().c_str() << std::endl;
	std::cout << context->Request().GetProtocolVersion().c_str() << std::endl;
	std::cout << context->Request().GetRemoteEndpointAddress().c_str() << std::endl;
	std::cout << context->Request().GetRemoteEndpointPort() << std::endl;
	std::cout << context->Request().GetAcceptEncoding().c_str() << std::endl;
	std::cout << context->Request().GetContentLength() << std::endl;
	std::cout << context->Request().GetContentType().c_str() << std::endl;
	
	// For each header.
	for (auto& h : context->Request().GetHeaders())
	{
		// Write header name and value.
		std::cout << h.first.c_str() << ": " << h.second.c_str() << "\r\n";
	}

	if (context->Request().Content != nullptr && context->Request().GetContentLength() > 0)
	{
		std::cout << context->Request().Content << std::endl;
	}

	// Write response.
	WebResponse* webResponse = &context->Response();
	webResponse->SetContentType("text/html");
	webResponse->SetContentLength(50);

	webResponse->WriteHeaders();

	std::istringstream input("01234567890123456789012345678901234567890123456789");
	webResponse->WriteContent(input.rdbuf());
	
}