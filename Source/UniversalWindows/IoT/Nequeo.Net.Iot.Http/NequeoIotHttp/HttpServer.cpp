/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          HttpServer.cpp
*  Purpose :       Http server.
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
#include "HttpServer.h"

using namespace NequeoIotHttp;

/// <summary>
/// Http server.
/// </summary>
/// <param name="serverPort">The server port number.</param>
HttpServer::HttpServer(int serverPort) : _port(serverPort), _disposed(false), _listener(nullptr), BufferSize(8192)
{
	// Create a stream socket listener.
	_listener = ref new StreamSocketListener();
	_listener->ConnectionReceived += ref new Windows::Foundation::TypedEventHandler<StreamSocketListener^, StreamSocketListenerConnectionReceivedEventArgs^>
		(this, &NequeoIotHttp::HttpServer::OnConnectionReceived);
}

/// <summary>
/// Http server deconstructor.
/// </summary>
HttpServer::~HttpServer()
{
	if (!_disposed)
	{
		_disposed = true;

		// Dispose of the listener.
		if (_listener != nullptr)
		{
			// Cleanup.
			delete _listener;
		}

		_listener = nullptr;
	}
}

/// <summary>
/// Start the server.
/// </summary>
Windows::Foundation::IAsyncAction^ HttpServer::StartServer()
{
	// Bind the server to the port.
	return _listener->BindServiceNameAsync(_port.ToString());
}

/// <summary>
/// On connection received.
/// </summary>
/// <param name="sender">The sender.</param>
/// <param name="args">The stream socket listener connection received event arguments.</param>
void NequeoIotHttp::HttpServer::OnConnectionReceived(StreamSocketListener ^sender, StreamSocketListenerConnectionReceivedEventArgs ^args)
{
	// Process the current request.
	concurrency::create_task(ProcessRequestAsync(args->Socket));
}

/// <summary>
/// Process a new request async.
/// </summary>
/// <param name="socket">The current socket stream.</param>
Windows::Foundation::IAsyncAction^ NequeoIotHttp::HttpServer::ProcessRequestAsync(StreamSocket^ socket)
{
	// Create the request async process.
	return concurrency::create_async([this, socket]
	{
		// Get the input stream.
		IInputStream^ input = socket->InputStream;

		// Setup the buffer.
		IBuffer^ buffer = ref new Buffer(BufferSize);
		unsigned int dataRead = BufferSize;
		std::vector<std::string> request;

		// While more data exists.
		while (dataRead == BufferSize)
		{
			IBuffer^ bufferResult = nullptr;

			// Read some data.
			concurrency::create_task(input->ReadAsync(buffer, BufferSize, InputStreamOptions::Partial))
				.then([&bufferResult](IBuffer^ responseBuffer)
			{
				// Get the response buffer.
				bufferResult = responseBuffer;

			}).wait();
			
			// If data exists.
			if (bufferResult != nullptr)
			{
				// Current read length from the received buffer.
				dataRead = bufferResult->Length;
				if (dataRead > 0)
				{
					// Get the byte array.
					std::vector<unsigned char> byteArray = GetData(bufferResult);

					// Load the bytes into the string array.
					request.push_back(std::string(byteArray.begin(), byteArray.end()));
				}
			}
			else
			{
				// Exit while loop.
				break;
			}
		}

		// If a request exists.
		if (request.size() > 0)
		{
			// Split the first line of data.
			std::vector<std::string> splitFirstRequest;
			Split(request[0], '\n', splitFirstRequest);

			// Split the request part.
			std::vector<std::string> splitRequest;
			Split(splitFirstRequest[0], ' ', splitRequest);

			// Convert to UTF-16 first request..
			std::wstring wstr(splitFirstRequest[0].length(), L' ');
			copy(splitFirstRequest[0].begin(), splitFirstRequest[0].end(), wstr.begin());

			// Get the ToUpper method.
			std::string toUpperMethod = splitRequest[0];
			std::transform(toUpperMethod.begin(), toUpperMethod.end(), toUpperMethod.begin(), ::toupper);

			// Convert to UTF-16 first request method.
			std::wstring wstrMethod(toUpperMethod.length(), L' ');
			copy(toUpperMethod.begin(), toUpperMethod.end(), wstrMethod.begin());

			// Convert to UTF-16 first request resource.
			std::wstring wstrResource(splitRequest[1].length(), L' ');
			copy(splitRequest[1].begin(), splitRequest[1].end(), wstrResource.begin());

			// Write the data back to the client.
			String^ requestPart = ref new String(wstr.c_str());
			String^ requestMethod = ref new String(wstrMethod.c_str());
			String^ requestResource = ref new String(wstrResource.c_str());
			
			// Get the socket output stream.
			IOutputStream^ output = socket->OutputStream;

			// Only accept GET method.
			if (requestMethod == "GET")
			{
				// Write response.
				WriteResponse(requestResource, output);
			}
			else
			{
				// Write response error.
				WriteResponseError(output);
			}
		}
	});
}

/// <summary>
/// Write response.
/// </summary>
/// <param name="request">The request (page and query).</param>
/// <param name="os">The current output stream.</param>
void NequeoIotHttp::HttpServer::WriteResponse(String^ request, IOutputStream^ os)
{
	// Html response.
	String^ html = "";

	// Request ping.   
	if (request == "ping" || request == "/ping")
	{
		auto calendar = ref new Calendar();
		String^ dateTimeFormat =
			calendar->Day.ToString() + "/" + calendar->Month.ToString() + "/" + calendar->Year.ToString() + " " +
			calendar->Hour.ToString() + ":" + calendar->Minute.ToString() + ":" + calendar->Second.ToString();
		
		// Get the utc time.
		html = CreateHtml("Ping", "UTC Date and Time: " + dateTimeFormat);
	}

	// Request alive.   
	if (request == "alive" || request == "/alive")
	{
		// Is alive OK.
		html = CreateText("OK");
	}

	// Request alive.   
	if (request == "sys" || request == "/sys")
	{
		// Is alive OK.
		html = CreateText("Internet of things - Windows 10 - Raspberry Pi");
	}
	
	// Get the body length.
	String^ length = html->Length().ToString();

	// Create the witter.
	DataWriter^ writer = ref new DataWriter();
	writer->WriteString(
		L"HTTP/1.1 200 OK\r\n" +
		L"Content-Length: " + length + "\r\n" +
		L"Connection: close\r\n\r\n"
	);

	// Write the body.
	writer->WriteString(html);

	// Write the data.
	os->WriteAsync(writer->DetachBuffer());
	os->FlushAsync();
}

/// <summary>
/// Write response error.
/// </summary>
/// <param name="os">The current output stream.</param>
void NequeoIotHttp::HttpServer::WriteResponseError(IOutputStream^ os)
{
	// Create the witter.
	DataWriter^ writer = ref new DataWriter();
	writer->WriteString(
		L"HTTP/1.1 500 Internal Server Error\r\n" +
		L"Content-Length: 0\r\n" +
		L"Connection: close\r\n\r\n"
	);

	// Write the data.
	os->WriteAsync(writer->DetachBuffer());
	os->FlushAsync();
}

/// <summary>
/// Create the response html.
/// </summary>
/// <param name="title">The page title.</param>
/// <param name="body">The page body.</param>
/// <returns>The html.</returns>
String^ NequeoIotHttp::HttpServer::CreateHtml(String^ title, String^ body)
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
String^ NequeoIotHttp::HttpServer::CreateText(String^ body)
{
	return body;
}

/// <summary>
/// Convert the buffer to an array of bytes.
/// </summary>
/// <param name="buffer">The buffer to convert.</param>
/// <return>The array of bytes.</return>
std::vector<unsigned char> NequeoIotHttp::HttpServer::GetData(IBuffer^ buffer)
{
	// Create the data reader.
	auto reader = DataReader::FromBuffer(buffer);

	// Create the data vector
	std::vector<unsigned char> data(reader->UnconsumedBufferLength);
	
	// If data exists.
	if (!data.empty())
	{
		// Read the bytes into the vector.
		reader->ReadBytes(::Platform::ArrayReference<unsigned char>(&data[0], data.size()));
	}

	// Return the array of bytes read.
	return data;
}

/// <summary>
/// Split the text.
/// </summary>
/// <param name="s">The text to split.</param>
/// <param name="delim">The delimeter.</param>
/// <param name="v">The array of split text.</param>
void NequeoIotHttp::HttpServer::Split(std::string &s, char delim, std::vector<std::string> &v)
{
	auto i = 0;
	auto pos = s.find(delim);

	while (pos != std::string::npos) 
	{
		v.push_back(s.substr(i, pos - i));
		i = ++pos;
		pos = s.find(delim, pos);

		if (pos == std::string::npos)
			v.push_back(s.substr(i, s.length()));
	}
}