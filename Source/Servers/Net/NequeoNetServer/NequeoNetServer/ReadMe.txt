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

WebSocket:
	Client request JSON elements:
		uniqueID:				A unique id for the client.
		applicationID:			An application id (room).
		available:				True if the client is available for contact.
		broadcast:				True if the uniqueID is visible to clients.
		broadcastAppID:			True if the applicationID is visible to clients.
		contactUniqueID:		The unique id of the client contact details.
		contactApplicationID:	The application id of the client contact details.

	Client request single text:
		time:					The current time.
		uniqueids:				Get the list of unique ids of all clients (The client is added only if the 'broadcast' is true).
		applicationids:			Get the list of application ids of all clients (The client is added only if the 'broadcastAppID' is true).
		uniqueapplication:		Get the list of unique id and application id groupings.

	Client response JSON elements:
		response:				Response ok, error, close, ping.
		error:					An error message if response is error.
		available:				True if the contact is available.
		contactUniqueID:		The unique id of the client contact details.
		contactApplicationID:	The application id of the client contact details.
		time:					The current time.
		uniques:				An array of unique ids.
		applications:			An array of application ids.

	Example of changing client details:
		{ "uniqueID" : "Drazen", "applicationID" : "ChatRoom", "available" : true, "broadcast" : true, "broadcastAppID" : true}

	Example of sendind data to a client contact (The client is contactable only if they are available = true):
		{ "contactUniqueID" : "Holly", "contactApplicationID" : "ChatRoom", "sentData" : "stuff" }

	Example of requesting the time: just sent the text 'time'
	Example of requesting unique ids: just sent the text 'uniqueids'	
	Example of requesting application ids: just sent the text 'applicationids'	

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