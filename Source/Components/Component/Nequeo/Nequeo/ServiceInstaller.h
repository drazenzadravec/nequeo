/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          ServiceInstaller.h
*  Purpose :       ServiceInstaller class.
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

#ifndef _SERVICEINSTALLER_H
#define _SERVICEINSTALLER_H

#include "Global.h"

namespace Nequeo
{
	//
	//   FUNCTION: InstallService
	//
	//   PURPOSE: Install the current application as a service to the local 
	//   service control manager database.
	//
	//   PARAMETERS:
	//   * pszServiceName - the name of the service to be installed
	//   * pszDisplayName - the display name of the service
	//   * dwStartType - the service start option. This parameter can be one of 
	//     the following values: SERVICE_AUTO_START, SERVICE_BOOT_START, 
	//     SERVICE_DEMAND_START, SERVICE_DISABLED, SERVICE_SYSTEM_START.
	//   * pszDependencies - a pointer to a double null-terminated array of null-
	//     separated names of services or load ordering groups that the system 
	//     must start before this service.
	//   * pszAccount - the name of the account under which the service runs.
	//   * pszPassword - the password to the account name.
	//
	//   NOTE: If the function fails to install the service, it prints the error 
	//   in the standard output stream for users to diagnose the problem.
	//
	void InstallService(PWSTR pszServiceName,
		PWSTR pszDisplayName,
		DWORD dwStartType,
		PWSTR pszDependencies,
		PWSTR pszAccount,
		PWSTR pszPassword);


	//
	//   FUNCTION: UninstallService
	//
	//   PURPOSE: Stop and remove the service from the local service control 
	//   manager database.
	//
	//   PARAMETERS: 
	//   * pszServiceName - the name of the service to be removed.
	//
	//   NOTE: If the function fails to uninstall the service, it prints the 
	//   error in the standard output stream for users to diagnose the problem.
	//
	void UninstallService(PWSTR pszServiceName);
}
#endif