#include "stdafx.h"

#include "Base\ThreadPool.h"
#include "StaticContentService.h"

// Constructor.
StaticContentService::StaticContentService(
	const std::string& configPath,
	PWSTR pszServiceName,
	BOOL fCanStop,
	BOOL fCanShutdown,
	BOOL fCanPauseContinue) : 
	CServiceBase(pszServiceName, fCanStop, fCanShutdown, fCanPauseContinue), _disposed(false), _configPath(configPath)
{

	// Load the configuration file and read it.
	_config = std::make_unique<ConfigModel>(_configPath);
	_config->ReadConfigFile();

	_server = std::make_unique<HttpServer>(_config->GetRootPath());
	_server->SetMultiServerContainer(_config->GetMultiServerContainer());
	_server->Initialise();

	// Create a manual-reset event that is not signaled at first to indicate 
	// the stopped signal of the service.
	_stoppedEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
}

// destructor.
StaticContentService::~StaticContentService(void)
{
	if (!_disposed)
	{
		_disposed = true;
	}

	if (_stoppedEvent)
	{
		CloseHandle(_stoppedEvent);
		_stoppedEvent = NULL;
	}
}

//
//   FUNCTION: StaticContentService::OnStart(DWORD, LPWSTR *)
//
//   PURPOSE: The function is executed when a Start command is sent to the 
//   service by the SCM or when the operating system starts (for a service 
//   that starts automatically). It specifies actions to take when the 
//   service starts. In this code sample, OnStart logs a service-start 
//   message to the Application log, and queues the main service function for 
//   execution in a thread pool worker thread.
//
//   PARAMETERS:
//   * dwArgc   - number of command line arguments
//   * lpszArgv - array of command line arguments
//
//   NOTE: A service application is designed to be long running. Therefore, 
//   it usually polls or monitors something in the system. The monitoring is 
//   set up in the OnStart method. However, OnStart does not actually do the 
//   monitoring. The OnStart method must return to the operating system after 
//   the service's operation has begun. It must not loop forever or block. To 
//   set up a simple monitoring mechanism, one general solution is to create 
//   a timer in OnStart. The timer would then raise events in your code 
//   periodically, at which time your service could do its monitoring. The 
//   other solution is to spawn a new thread to perform the main service 
//   functions, which is demonstrated in this code sample.
//
void StaticContentService::OnStart(DWORD dwArgc, PWSTR *lpszArgv)
{
	// Log a service start message to the Application log.
	WriteEventLogEntry(L"Nequeo net http service has started.",
		EVENTLOG_INFORMATION_TYPE);

	// Queue the main service function for execution in a worker thread.
	Nequeo::CThreadPool::QueueUserWorkItem(&StaticContentService::ServiceWorkerThread, this);
}

//
//   FUNCTION: StaticContentService::OnStop(void)
//
//   PURPOSE: The function is executed when a Stop command is sent to the 
//   service by SCM. It specifies actions to take when a service stops 
//   running. In this code sample, OnStop logs a service-stop message to the 
//   Application log, and waits for the finish of the main service function.
//
//   COMMENTS:
//   Be sure to periodically call ReportServiceStatus() with 
//   SERVICE_STOP_PENDING if the procedure is going to take long time. 
//
void StaticContentService::OnStop()
{
	// Log a service stop message to the Application log.
	WriteEventLogEntry(L"Nequeo net http service has stopped.",
		EVENTLOG_INFORMATION_TYPE);

	// Signal the stopped event.
	SetEvent(_stoppedEvent);

	// Stop the server.
	_server->Stop();
}

//
//   FUNCTION: CSampleService::ServiceWorkerThread(void)
//
//   PURPOSE: The method performs the main function of the service. It runs 
//   on a thread pool worker thread.
//
void StaticContentService::ServiceWorkerThread(void)
{
	// Start the server.
	_server->Start();

	// Wait for stop
	while (1)
	{
		// Check whether to stop the service.
		WaitForSingleObject(_stoppedEvent, INFINITE);
		return;
	}
}