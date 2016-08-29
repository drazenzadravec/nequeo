/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          HttpServer.h
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

#pragma once

#include "pch.h"

using namespace Platform;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Networking::Sockets;
using namespace Windows::Storage::Streams;
using namespace Windows::Foundation;
using namespace Windows::Globalization;

namespace NequeoIotHttp
{
	/// <summary>
	/// Http server.
	/// </summary>
	public ref class HttpServer sealed
	{
	public:
		/// <summary>
		/// Http server.
		/// </summary>
		/// <param name="serverPort">The server port number.</param>
		HttpServer(int serverPort);

		/// <summary>
		/// Http server deconstructor.
		/// </summary>
		virtual ~HttpServer();

		/// <summary>
		/// Start the server.
		/// </summary>
		Windows::Foundation::IAsyncAction^ StartServer();

	private:
		int _port;
		bool _disposed;
		const unsigned int BufferSize;
		StreamSocketListener^ _listener;

		/// <summary>
		/// On connection received.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The stream socket listener connection received event arguments.</param>
		void OnConnectionReceived(StreamSocketListener ^sender, StreamSocketListenerConnectionReceivedEventArgs ^args);

		/// <summary>
		/// Process a new request async.
		/// </summary>
		/// <param name="socket">The current socket stream.</param>
		Windows::Foundation::IAsyncAction^ ProcessRequestAsync(StreamSocket^ socket);

		/// <summary>
		/// Write response.
		/// </summary>
		/// <param name="request">The request (page and query).</param>
		/// <param name="os">The current output stream.</param>
		void WriteResponse(String^ request, IOutputStream^ os);

		/// <summary>
		/// Write response error.
		/// </summary>
		/// <param name="os">The current output stream.</param>
		void WriteResponseError(IOutputStream^ os);

		/// <summary>
		/// Create the response html.
		/// </summary>
		/// <param name="title">The page title.</param>
		/// <param name="body">The page body.</param>
		/// <returns>The html.</returns>
		String^ CreateHtml(String^ title, String^ body);

		/// <summary>
		/// Create the response text.
		/// </summary>
		/// <param name="body">The page body.</param>
		/// <returns>The text.</returns>
		String^ CreateText(String^ body);

		/// <summary>
		/// Convert the buffer to an array of bytes.
		/// </summary>
		/// <param name="buffer">The buffer to convert.</param>
		/// <return>The array of bytes.</return>
		std::vector<unsigned char> GetData(IBuffer^ buffer);

		/// <summary>
		/// Split the text.
		/// </summary>
		/// <param name="s">The text to split.</param>
		/// <param name="delim">The delimeter.</param>
		/// <param name="v">The array of split text.</param>
		void Split(std::string &s, char delim, std::vector<std::string> &v);
		
	};
}