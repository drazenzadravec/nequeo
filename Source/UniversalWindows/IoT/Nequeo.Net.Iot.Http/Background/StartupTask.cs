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

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Background
{
    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral _serviceDeferral;
        AppServiceConnection _appServiceConnection;
        HttpServer _server = null;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Associate a cancellation handler with the background task. 
            taskInstance.Canceled += OnCanceled;

            // Get the deferral object from the task instance
            _serviceDeferral = taskInstance.GetDeferral();

            //Set a result to return to the caller
            _server = new HttpServer(81);
            IAsyncAction asyncAction = Windows.System.Threading.ThreadPool.RunAsync(
                (workItem) =>
                {
                    _server.StartServer();
                });

            _appServiceConnection = new AppServiceConnection();
            _appServiceConnection.AppServiceName = "NequeoNetIotHttpService";
            _appServiceConnection.RequestReceived += OnRequestReceived;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reason"></param>
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if(_server != null)
                _server.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var message = args.Request.Message;
            string command = message["Command"] as string;

            switch (command)
            {
                case "Initialize":
                    {
                        var messageDeferral = args.GetDeferral();
                        //Set a result to return to the caller
                        var returnMessage = new ValueSet();
                        returnMessage.Add("Status", "Success");
                        var responseStatus = await args.Request.SendResponseAsync(returnMessage);
                        messageDeferral.Complete();
                        break;
                    }

                case "Quit":
                    {
                        if (_server != null)
                            _server.Dispose();

                        //Service was asked to quit. Give us service deferral
                        //so platform can terminate the background task
                        _serviceDeferral.Complete();
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal sealed class HttpServer : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverPort"></param>
        public HttpServer(int serverPort)
        {
            _listener = new StreamSocketListener();
            _port = serverPort;
            _listener.ConnectionReceived += (s, e) => ProcessRequestAsync(e.Socket);
        }

        private const uint BufferSize = 8192;
        private int _port = 81;
        private readonly StreamSocketListener _listener;

        string offHtmlString = "<html><head><title>Blinky App</title></head><body><form action=\"blinky.html\" method=\"GET\"><input type=\"radio\" name=\"state\" value=\"on\" onclick=\"this.form.submit()\"> On<br><input type=\"radio\" name=\"state\" value=\"off\" checked onclick=\"this.form.submit()\"> Off</form></body></html>";
        string onHtmlString = "<html><head><title>Blinky App</title></head><body><form action=\"blinky.html\" method=\"GET\"><input type=\"radio\" name=\"state\" value=\"on\" checked onclick=\"this.form.submit()\"> On<br><input type=\"radio\" name=\"state\" value=\"off\" onclick=\"this.form.submit()\"> Off</form></body></html>";

        /// <summary>
        /// 
        /// </summary>
        public void StartServer()
        {
#pragma warning disable CS4014
            _listener.BindServiceNameAsync(_port.ToString());
#pragma warning restore CS4014
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _listener.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        private async void ProcessRequestAsync(StreamSocket socket)
        {
            // this works for text only
            StringBuilder request = new StringBuilder();
            using (IInputStream input = socket.InputStream)
            {
                byte[] data = new byte[BufferSize];
                IBuffer buffer = data.AsBuffer();
                uint dataRead = BufferSize;
                while (dataRead == BufferSize)
                {
                    await input.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);
                    request.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    dataRead = buffer.Length;
                }
            }

            using (IOutputStream output = socket.OutputStream)
            {
                string requestMethod = request.ToString().Split('\n')[0];
                string[] requestParts = requestMethod.Split(' ');

                if (requestParts[0] == "GET")
                    await WriteResponseAsync(requestParts[1], output);
                else
                    throw new InvalidDataException("HTTP method not supported: "
                                                   + requestParts[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="os"></param>
        /// <returns></returns>
        private async Task WriteResponseAsync(string request, IOutputStream os)
        {
            // See if the request is for blinky.html, if yes get the new state
            string state = "Unspecified";
            bool stateChanged = false;
            if (request.Contains("blinky.html?state=on"))
            {
                state = "On";
                stateChanged = true;
            }
            else if (request.Contains("blinky.html?state=off"))
            {
                state = "Off";
                stateChanged = true;
            }

            if (stateChanged)
            {
                var updateMessage = new ValueSet();
                updateMessage.Add("State", state);
            }

            string html = state == "On" ? onHtmlString : offHtmlString;
            // Show the html 
            using (Stream resp = os.AsStreamForWrite())
            {
                // Look in the Data subdirectory of the app package
                byte[] bodyArray = Encoding.UTF8.GetBytes(html);
                MemoryStream stream = new MemoryStream(bodyArray);
                string header = String.Format("HTTP/1.1 200 OK\r\n" +
                                  "Content-Length: {0}\r\n" +
                                  "Connection: close\r\n\r\n",
                                  stream.Length);

                byte[] headerArray = Encoding.UTF8.GetBytes(header);
                await resp.WriteAsync(headerArray, 0, headerArray.Length);
                await stream.CopyToAsync(resp);
                await resp.FlushAsync();
            }
        }
    }
}
