// NequeoNetHttpService.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <stdio.h>
#include <windows.h>

#include "StaticContentService.h"

#include "Base\ServiceInstaller.h"
#include "Base\ServiceBase.h"

// Internal name of the service
#define SERVICE_NAME             L"NequeoNetHttpService"

// Displayed name of the service
#define SERVICE_DISPLAY_NAME     L"Nequeo Net Http Service"

// Service start options.
#define SERVICE_START_TYPE       SERVICE_AUTO_START

// List of service dependencies - "dep1\0dep2\0\0"
#define SERVICE_DEPENDENCIES     L""

// The name of the account under which the service should run
#define SERVICE_ACCOUNT          L"NT AUTHORITY\\LocalService"

// The password to the service account name
#define SERVICE_PASSWORD         NULL

#define SERVICE_DESCRIPTION      L"Http version 1.1 C++ static content server."

std::string wstring2string(std::wstring wstr)
{
	std::string str(wstr.length(), ' ');
	copy(wstr.begin(), wstr.end(), str.begin());
	return str;
}

std::wstring string2wstring(std::string str)
{
	std::wstring wstr(str.length(), L' ');
	copy(str.begin(), str.end(), wstr.begin());
	return wstr;
}

// Main entry point.
void __cdecl _tmain(int argc, wchar_t *argv[])
{
	if ((argc > 1) && ((*argv[1] == L'-' || (*argv[1] == L'/'))))
	{
		// Install the service.
		if (_wcsicmp(L"install", argv[1] + 1) == 0)
		{
			// Install the service when the command is 
			// "-install" or "/install".
			Nequeo::InstallService(
				SERVICE_NAME,               // Name of service
				SERVICE_DISPLAY_NAME,       // Name to display
				SERVICE_START_TYPE,         // Service start type
				SERVICE_DEPENDENCIES,       // Dependencies
				SERVICE_ACCOUNT,            // Service running account
				SERVICE_PASSWORD            // Password of the account
			);

			// Set the service description.
			Nequeo::ServiceDescription(SERVICE_NAME, SERVICE_DESCRIPTION);
		}
		else if (_wcsicmp(L"remove", argv[1] + 1) == 0)
		{
			// Uninstall the service when the command is 
			// "-remove" or "/remove".
			Nequeo::UninstallService(SERVICE_NAME);
		}
	}
	else
	{
		wprintf(L"Parameters:\n");
		wprintf(L" -install  to install the service.\n");
		wprintf(L" -remove   to remove the service.\n");

		wchar_t szPath[MAX_PATH];

		// Get the file path.
		if (GetModuleFileName(NULL, szPath, MAX_PATH))
		{
			// Find the last.
			auto pos = std::wstring(szPath).find_last_of(L"\\/");
			std::wstring directory = std::wstring(szPath).substr(0, pos);

			// Get config file.
			std::string configPath = wstring2string(directory) + "\\Configuration.json";

			// Start the service.
			StaticContentService service(configPath, SERVICE_NAME);
			if (!Nequeo::CRegisterService::Run(service))
			{
				wprintf(L"Service failed to run w/err 0x%08lx\n", GetLastError());
			}
		}
	}
}

