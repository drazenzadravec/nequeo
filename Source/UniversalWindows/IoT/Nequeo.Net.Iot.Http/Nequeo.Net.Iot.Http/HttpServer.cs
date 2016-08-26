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
using Windows.Storage;

namespace Nequeo.Net.Iot.Http
{
    /// <summary>
    /// Http server.
    /// </summary>
    internal sealed class HttpServer : IDisposable
    {
        /// <summary>
        /// Http server.
        /// </summary>
        /// <param name="serverPort">The server port number.</param>
        public HttpServer(int serverPort)
        {
            _port = serverPort;
            _listener = new StreamSocketListener();
            _listener.ConnectionReceived += (s, e) => ProcessRequestAsync(e.Socket);
        }

        private int _port = 80;
        private bool _disposed = false;
        private const uint BufferSize = 8192;
        private readonly StreamSocketListener _listener = null;

        /// <summary>
        /// Start the server.
        /// </summary>
        public async void StartServer()
        {
            // Bind the server to the port.
            await _listener.BindServiceNameAsync(_port.ToString());
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            // If not disposed.
            if (!_disposed)
            {
                _disposed = true;

                // Dispose of the listener.
                if (_listener != null)
                {
                    _listener.Dispose();
                }
            }
        }

        /// <summary>
        /// Process a new request async.
        /// </summary>
        /// <param name="socket">The current socket stream.</param>
        private async void ProcessRequestAsync(StreamSocket socket)
        {
            // This works for text only.
            StringBuilder request = new StringBuilder();

            // Get the input stream.
            using (IInputStream input = socket.InputStream)
            {
                // Setup the buffer.
                byte[] data = new byte[BufferSize];
                IBuffer buffer = data.AsBuffer();
                uint dataRead = BufferSize;

                // While more data exists.
                while (dataRead == BufferSize)
                {
                    // Read the next set of data.
                    await input.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);
                    request.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    dataRead = buffer.Length;
                }
            }

            // Get the output stream.
            using (IOutputStream output = socket.OutputStream)
            {
                // Get the request parts.
                string requestMethod = request.ToString().Split('\n')[0];
                string[] requestParts = requestMethod.Split(' ');

                // Only accept GET method.
                if (requestParts[0].ToUpper() == "GET")
                    await WriteResponseAsync(requestParts[1], output);
                else
                    await WriteResponseErrorAsync(output);
            }
        }

        /// <summary>
        /// Write response.
        /// </summary>
        /// <param name="request">The request (page and query).</param>
        /// <param name="os">The current output stream.</param>
        /// <returns>The async task.</returns>
        private async Task WriteResponseAsync(string request, IOutputStream os)
        {
            // Html response.
            string html = "";

            // Request ping.   
            if (request.Contains("ping"))
            {
                // Get the utc time.
                html = CreateHtml("Ping", "UTC Date and Time: " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"));
            }
            // Request alive.   
            if (request.Contains("alive"))
            {
                // Is alive OK.
                html = CreateText("OK");
            }
            // Request alive.   
            if (request.Contains("sys"))
            {
                // Is alive OK.
                html = CreateText("Internet of things - Windows 10 - Raspberry Pi");
            }

            // Get the output stream.
            using (Stream resp = os.AsStreamForWrite())
            {
                // Set the body of the response.
                byte[] bodyArray = Encoding.UTF8.GetBytes(html);
                MemoryStream stream = new MemoryStream(bodyArray);

                // Set the headers.
                string header = String.Format(
                    "HTTP/1.1 200 OK\r\n" +
                    "Content-Length: {0}\r\n" +
                    "Connection: close\r\n\r\n",
                    stream.Length);

                // Write the headers.
                byte[] headerArray = Encoding.UTF8.GetBytes(header);
                await resp.WriteAsync(headerArray, 0, headerArray.Length);

                // Write the body to the output stream.
                await stream.CopyToAsync(resp);
                await resp.FlushAsync();
            }
        }

        /// <summary>
        /// Write response error.
        /// </summary>
        /// <param name="os">The current output stream.</param>
        /// <returns>The async task.</returns>
        private async Task WriteResponseErrorAsync(IOutputStream os)
        {
            // Get the output stream.
            using (Stream resp = os.AsStreamForWrite())
            {
                // Set the body of the response.
                byte[] bodyArray = Encoding.UTF8.GetBytes("");
                MemoryStream stream = new MemoryStream(bodyArray);

                // Set the headers.
                string header = String.Format(
                    "HTTP/1.1 500 Internal Server Error\r\n" +
                    "Content-Length: {0}\r\n" +
                    "Connection: close\r\n\r\n",
                    stream.Length);

                // Write the headers.
                byte[] headerArray = Encoding.UTF8.GetBytes(header);
                await resp.WriteAsync(headerArray, 0, headerArray.Length);

                // Write the body to the output stream.
                await stream.CopyToAsync(resp);
                await resp.FlushAsync();
            }
        }

        /// <summary>
        /// Create the response html.
        /// </summary>
        /// <param name="title">The page title.</param>
        /// <param name="body">The page body.</param>
        /// <returns>The html.</returns>
        private string CreateHtml(string title, string body)
        {
            return
            "<html>" +
                "<head>" +
                    "<title>" + title + "</title>" +
                "</head>" +
                "<body>" +
                    body +
                "</body>" +
            "</html>";
        }

        /// <summary>
        /// Create the response text.
        /// </summary>
        /// <param name="body">The page body.</param>
        /// <returns>The text.</returns>
        private string CreateText(string body)
        {
            return body;
        }
    }
}
