using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation.Collections;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.AppService;
using Windows.System.Threading;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Foundation;

namespace Nequeo.Net.Iot.Http
{
    /// <summary>
    /// Start a background task.
    /// </summary>
    public sealed class StartupTask : IBackgroundTask
    {
        HttpServer _server = null;
        BackgroundTaskDeferral _serviceDeferral = null;

        /// <summary>
        /// Run the background task, entry-point.
        /// </summary>
        /// <param name="taskInstance">The background task instance.</param>
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Associate a cancellation handler with the background task. 
            taskInstance.Canceled += OnCanceled;

            // Get the deferral object from the task instance.
            _serviceDeferral = taskInstance.GetDeferral();

            // Set a result to return to the caller.
            _server = new HttpServer(82);
            IAsyncAction asyncAction = Windows.System.Threading.ThreadPool.RunAsync(
                (workItem) =>
                {
                    // Start the server.
                    _server.StartServer();
                });
        }

        /// <summary>
        /// On cancel background task.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="reason">The reson for cancellation.</param>
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // Clean up and get ready to exit.
            if (_server != null)
                _server.Dispose();

            // Service was asked to quit. Give us service deferral
            // so platform can terminate the background task
            if (_serviceDeferral != null)
                _serviceDeferral.Complete();
        }
    }
}
