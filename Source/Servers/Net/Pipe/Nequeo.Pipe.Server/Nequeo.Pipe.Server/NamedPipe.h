/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          NamedPipe.h
*  Purpose :       NamedPipe class.
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

#ifndef _NAMEDPIPE_H
#define _NAMEDPIPE_H

#include "stdafx.h"

namespace Nequeo
{
	namespace Pipe
	{
		namespace Server
		{
			/// <summary>
			/// On new request call back function.
			/// </summary>
			/// <param name="pchRequest">The request message.</param>
			/// <param name="pchReply">The reply message.</param>
			/// <param name="pchBytes">The reply size.</param>
			typedef void(*OnRequest_Function)(LPTSTR pchRequest, LPTSTR pchReply, LPDWORD pchBytes);

			/// <summary>
			/// Named pipe server, multi threaded, blocking.
			/// </summary>
			class NamedPipe
			{
			public:
				/// <summary>
				/// Named pipe server.
				/// </summary>
				NamedPipe();

				/// <summary>
				/// Named pipe server destructor.
				/// </summary>
				virtual ~NamedPipe();

				/// <summary>
				/// Initialize the name pipe.
				/// </summary>
				/// <param name="namedPipeName">The named pipe name.</param>
				/// <param name="onRequestFunction">The request call back function handler.</param>
				void Initialize(std::string namedPipeName, OnRequest_Function onRequestFunction);

				/// <summary>
				/// Start the named pipe server.
				/// </summary>
				void Start();

				/// <summary>
				/// Stop the named pipe server.
				/// </summary>
				void Stop();

			private:
				bool _disposed;
				bool _isRunning;
				bool _started;
				bool _init;

				std::string _namedPipeName;

				static HANDLE _hPipe;
				static OnRequest_Function _onRequestFunction;

				/// <summary>
				/// Initialize the name pipe.
				/// </summary>
				/// <param name="namedPipeName">A state passed parameter.</param>
				/// <return>The result.</return>
				static DWORD WINAPI InstanceThread(LPVOID lpvParam);

				/// <summary>
				/// Get answer to request.
				/// </summary>
				/// <param name="pchRequest">The request message.</param>
				/// <param name="pchReply">The reply message.</param>
				/// <param name="pchBytes">The reply size.</param>
				/// <return>The result.</return>
				static VOID GetAnswerToRequest(LPTSTR pchRequest, LPTSTR pchReply, LPDWORD pchBytes);

				///	<summary>
				///	Convert from wide string to string.
				///	</summary>
				/// <param name="wstr">The wide string.</param>
				/// <returns>The result.</returns>
				std::string WstringToString(std::wstring wstr);

				///	<summary>
				///	Convert from string to wide string.
				///	</summary>
				/// <param name="str">The string.</param>
				/// <returns>The result.</returns>
				std::wstring StringToWstring(std::string str);

			};
		}
	}
}
#endif