/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          StartupTask.cpp
*  Purpose :       Startup task.
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

#include "pch.h"
#include "StartupTask.h"

using namespace NequeoIotHttp;

/// <summary>
/// Run the background task, entry-point.
/// </summary>
/// <param name="taskInstance">The background task instance.</param>
void StartupTask::Run(IBackgroundTaskInstance^ taskInstance)
{
	// Associate a cancellation handler with the background task. 
	taskInstance->Canceled += ref new Windows::ApplicationModel::Background::BackgroundTaskCanceledEventHandler(this, &NequeoIotHttp::StartupTask::OnCanceled);

	// Get the deferral object from the task instance.
	_serviceDeferral = taskInstance->GetDeferral();

	// Set a result to return to the caller.
	_server = ref new HttpServer(83);
	Windows::System::Threading::ThreadPool::RunAsync(
		ref new Windows::System::Threading::WorkItemHandler(this, &NequeoIotHttp::StartupTask::OnWorkItemHandler));
}

/// <summary>
/// On start server work item handler.
/// </summary>
/// <param name="operation">The async action.</param>
void StartupTask::OnWorkItemHandler(Windows::Foundation::IAsyncAction^ operation)
{
	// If the server has been created.
	if (_server != nullptr)
	{
		// Start the server.
		auto start = concurrency::create_task(_server->StartServer());
	}
}

/// <summary>
/// On cancel background task.
/// </summary>
/// <param name="sender">The sender.</param>
/// <param name="reason">The reson for cancellation.</param>
void StartupTask::OnCanceled(IBackgroundTaskInstance^ sender, BackgroundTaskCancellationReason reason)
{
	// Clean up and get ready to exit.
	if (_server != nullptr)
		delete _server;

	// Service was asked to quit. Give us service deferral
	// so platform can terminate the background task
	if (_serviceDeferral != nullptr)
	{
		_serviceDeferral->Complete();
		_serviceDeferral.Release();
	}

	_server = nullptr;
	_serviceDeferral = nullptr;
}
