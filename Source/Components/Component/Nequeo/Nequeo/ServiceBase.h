/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          ServiceBase.h
*  Purpose :       ServiceBase class.
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

#ifndef _SERVICEBASE_H
#define _SERVICEBASE_H

#include "Global.h"
#include <assert.h>

namespace Nequeo
{
	/// Service base class.
	class CServiceBase
	{
	public:
		// Service object constructor. The optional parameters (fCanStop, 
		// fCanShutdown and fCanPauseContinue) allow you to specify whether the 
		// service can be stopped, paused and continued, or be notified when 
		// system shutdown occurs.
		CServiceBase(PWSTR pszServiceName,
			BOOL fCanStop = TRUE,
			BOOL fCanShutdown = TRUE,
			BOOL fCanPauseContinue = FALSE);

		// Service object destructor. 
		virtual ~CServiceBase(void);

		// Stop the service.
		void Stop();

		// Start the service.
		void Start(DWORD dwArgc, PWSTR *pszArgv);

		// Pause the service.
		void Pause();

		// Resume the service after being paused.
		void Continue();

		// Execute when the system is shutting down.
		void Shutdown();

		// The name of the service
		PWSTR m_name;

		// The status of the service
		SERVICE_STATUS m_status;

		// The service status handle
		SERVICE_STATUS_HANDLE m_statusHandle;

	protected:

		// When implemented in a derived class, executes when a Start command is 
		// sent to the service by the SCM or when the operating system starts 
		// (for a service that starts automatically). Specifies actions to take 
		// when the service starts.
		virtual void OnStart(DWORD dwArgc, PWSTR *pszArgv);

		// When implemented in a derived class, executes when a Stop command is 
		// sent to the service by the SCM. Specifies actions to take when a 
		// service stops running.
		virtual void OnStop();

		// When implemented in a derived class, executes when a Pause command is 
		// sent to the service by the SCM. Specifies actions to take when a 
		// service pauses.
		virtual void OnPause();

		// When implemented in a derived class, OnContinue runs when a Continue 
		// command is sent to the service by the SCM. Specifies actions to take 
		// when a service resumes normal functioning after being paused.
		virtual void OnContinue();

		// When implemented in a derived class, executes when the system is 
		// shutting down. Specifies what should occur immediately prior to the 
		// system shutting down.
		virtual void OnShutdown();

		// Set the service status and report the status to the SCM.
		void SetServiceStatus(DWORD dwCurrentState,
			DWORD dwWin32ExitCode = NO_ERROR,
			DWORD dwWaitHint = 0);

		// Log a message to the Application event log.
		void WriteEventLogEntry(PWSTR pszMessage, WORD wType);

		// Log an error message to the Application event log.
		void WriteErrorLogEntry(PWSTR pszFunction,
			DWORD dwError = GetLastError());
		
	};

	/// Register a singleton service.
	class CRegisterService
	{
	public:

		// Register the executable for a service with the Service Control Manager 
		// (SCM). After you call Run(ServiceBase), the SCM issues a Start command, 
		// which results in a call to the OnStart method in the service. This 
		// method blocks until the service has stopped.
		static BOOL Run(CServiceBase &service1);

		// Register the executable for a service with the Service Control Manager 
		// (SCM). After you call Run(ServiceBase), the SCM issues a Start command, 
		// which results in a call to the OnStart method in the service. This 
		// method blocks until the service has stopped.
		static BOOL Run(CServiceBase &service1, CServiceBase &service2);

		// Register the executable for a service with the Service Control Manager 
		// (SCM). After you call Run(ServiceBase), the SCM issues a Start command, 
		// which results in a call to the OnStart method in the service. This 
		// method blocks until the service has stopped.
		static BOOL Run(CServiceBase &service1, CServiceBase &service2, CServiceBase &service3);

		// Register the executable for a service with the Service Control Manager 
		// (SCM). After you call Run(ServiceBase), the SCM issues a Start command, 
		// which results in a call to the OnStart method in the service. This 
		// method blocks until the service has stopped.
		static BOOL Run(CServiceBase &service1, CServiceBase &service2, CServiceBase &service3, CServiceBase &service4);

		// Register the executable for a service with the Service Control Manager 
		// (SCM). After you call Run(ServiceBase), the SCM issues a Start command, 
		// which results in a call to the OnStart method in the service. This 
		// method blocks until the service has stopped.
		static BOOL Run(CServiceBase &service1, CServiceBase &service2, CServiceBase &service3, CServiceBase &service4, CServiceBase &service5);

		// Register the executable for a service with the Service Control Manager 
		// (SCM). After you call Run(ServiceBase), the SCM issues a Start command, 
		// which results in a call to the OnStart method in the service. This 
		// method blocks until the service has stopped.
		static BOOL Run(CServiceBase &service1, CServiceBase &service2, CServiceBase &service3, CServiceBase &service4, CServiceBase &service5, CServiceBase &service6);

	private:

		// Entry point for the service. It registers the handler function for the 
		// service and starts the service.
		static void WINAPI ServiceMain1(DWORD, LPWSTR*);

		// Entry point for the service. It registers the handler function for the 
		// service and starts the service.
		static void WINAPI ServiceMain2(DWORD, LPWSTR*);

		// Entry point for the service. It registers the handler function for the 
		// service and starts the service.
		static void WINAPI ServiceMain3(DWORD, LPWSTR*);

		// Entry point for the service. It registers the handler function for the 
		// service and starts the service.
		static void WINAPI ServiceMain4(DWORD, LPWSTR*);

		// Entry point for the service. It registers the handler function for the 
		// service and starts the service.
		static void WINAPI ServiceMain5(DWORD, LPWSTR*);

		// Entry point for the service. It registers the handler function for the 
		// service and starts the service.
		static void WINAPI ServiceMain6(DWORD, LPWSTR*);

		// The function is called by the SCM whenever a control code is sent to 
		// the service.
		static void WINAPI ServiceCtrlHandler1(DWORD dwCtrl);

		// The function is called by the SCM whenever a control code is sent to 
		// the service.
		static void WINAPI ServiceCtrlHandler2(DWORD dwCtrl);

		// The function is called by the SCM whenever a control code is sent to 
		// the service.
		static void WINAPI ServiceCtrlHandler3(DWORD dwCtrl);

		// The function is called by the SCM whenever a control code is sent to 
		// the service.
		static void WINAPI ServiceCtrlHandler4(DWORD dwCtrl);

		// The function is called by the SCM whenever a control code is sent to 
		// the service.
		static void WINAPI ServiceCtrlHandler5(DWORD dwCtrl);

		// The function is called by the SCM whenever a control code is sent to 
		// the service.
		static void WINAPI ServiceCtrlHandler6(DWORD dwCtrl);

		// The singleton service instance.
		static CServiceBase *s_service1;
		static CServiceBase *s_service2;
		static CServiceBase *s_service3;
		static CServiceBase *s_service4;
		static CServiceBase *s_service5;
		static CServiceBase *s_service6;
	};
}
#endif