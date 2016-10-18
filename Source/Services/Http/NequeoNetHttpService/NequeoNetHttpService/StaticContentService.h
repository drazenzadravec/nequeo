#pragma once

#include "stdafx.h"
#include "Base\ServiceBase.h"

#include "HttpServer.h"
#include "ConfigModel.h"

using namespace Nequeo::Net::Http;

// Static content service.
class StaticContentService : public Nequeo::CServiceBase
{
public:

	StaticContentService(
		const std::string& configPath,
		PWSTR pszServiceName,
		BOOL fCanStop = TRUE,
		BOOL fCanShutdown = TRUE,
		BOOL fCanPauseContinue = FALSE);

	virtual ~StaticContentService(void);
	void ServiceWorkerThread(void);

protected:

	virtual void OnStart(DWORD dwArgc, PWSTR *pszArgv) override;
	virtual void OnStop() override;

private:
	bool _disposed;
	HANDLE _stoppedEvent;
	std::string _configPath;

	std::unique_ptr<ConfigModel> _config;
	std::unique_ptr<HttpServer> _server;
};