========================================================================
    DYNAMIC LINK LIBRARY : NequeoNetServer Project Overview
========================================================================

AppWizard has created this NequeoNetServer DLL for you.

This file contains a summary of what you will find in each of the files that
make up your NequeoNetServer application.


NequeoNetServer.vcxproj
    This is the main project file for VC++ projects generated using an Application Wizard.
    It contains information about the version of Visual C++ that generated the file, and
    information about the platforms, configurations, and project features selected with the
    Application Wizard.

NequeoNetServer.vcxproj.filters
    This is the filters file for VC++ projects generated using an Application Wizard. 
    It contains information about the association between the files in your project 
    and the filters. This association is used in the IDE to show grouping of files with
    similar extensions under a specific node (for e.g. ".cpp" files are associated with the
    "Source Files" filter).

NequeoNetServer.cpp
    This is the main DLL source file.

	When created, this DLL does not export any symbols. As a result, it
	will not produce a .lib file when it is built. If you wish this project
	to be a project dependency of some other project, you will either need to
	add code to export some symbols from the DLL so that an export library
	will be produced, or you can set the Ignore Input Library property to Yes
	on the General propert page of the Linker folder in the project's Property
	Pages dialog box.

/////////////////////////////////////////////////////////////////////////////
Other standard files:

StdAfx.h, StdAfx.cpp
    These files are used to build a precompiled header (PCH) file
    named NequeoNetServer.pch and a precompiled types file named StdAfx.obj.

/////////////////////////////////////////////////////////////////////////////
Other notes:

AppWizard uses "TODO:" comments to indicate parts of the source code you
should add to or customize.

/////////////////////////////////////////////////////////////////////////////


// TestNetServer.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <iostream>
#include <sstream>
#include <string>
#include <thread>

#include "HttpClient.h"
#include "HttpServer.h"
#include "ConfigModel.h"

using namespace Nequeo::Net::Http;

void AsyncClient(const Nequeo::Net::Http::WebClient*, const Nequeo::Net::Http::NetResponse&, const std::shared_ptr<const Nequeo::AsyncCallerContext>&);

int main()
{
	

	HttpClient client("www.google.com", 80, false, IPVersionType::IPv6);
	client.RequestAsync("GET", "/", AsyncClient);

	ConfigModel config("configuration.json");
	config.ReadConfigFile();
	
	HttpServer server(config.GetRootPath());
	server.SetMultiServerContainer(config.GetMultiServerContainer());
	server.Initialise();
	server.Start();

	return 0;
}

void AsyncClient(const Nequeo::Net::Http::WebClient* client, const Nequeo::Net::Http::NetResponse& response, const std::shared_ptr<const Nequeo::AsyncCallerContext>& context)
{
	std::cout << "In Here." << std::endl;
	std::cout << response.Content->rdbuf() << std::endl;
	std::cout << response.GetContentLength() << std::endl;
}