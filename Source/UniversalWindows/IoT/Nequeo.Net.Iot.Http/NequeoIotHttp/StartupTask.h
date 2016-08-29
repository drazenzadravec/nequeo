/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          StartupTask.h
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

#pragma once

#include "pch.h"

#include "HttpServer.h"

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::ApplicationModel::Background;

namespace NequeoIotHttp
{
	/// <summary>
	/// Start a background task.
	/// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class StartupTask sealed : public IBackgroundTask
    {
    public:
		/// <summary>
		/// Run the background task, entry-point.
		/// </summary>
		/// <param name="taskInstance">The background task instance.</param>
        virtual void Run(IBackgroundTaskInstance^ taskInstance);

	private:
		HttpServer^ _server;
		Platform::Agile<BackgroundTaskDeferral^> _serviceDeferral;

		/// <summary>
		/// On cancel background task.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="reason">The reason for cancellation.</param>
		void OnCanceled(IBackgroundTaskInstance^ sender, BackgroundTaskCancellationReason reason);

		/// <summary>
		/// On start server work item handler.
		/// </summary>
		/// <param name="operation">The async action.</param>
		void OnWorkItemHandler(Windows::Foundation::IAsyncAction^ operation);

    };
}
