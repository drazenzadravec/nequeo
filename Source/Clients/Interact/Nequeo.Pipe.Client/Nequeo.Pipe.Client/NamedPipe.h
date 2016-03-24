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
		namespace Client
		{
			/// <summary>
			/// On new reply call back function.
			/// </summary>
			/// <param name="pchReply">The reply message.</param>
			/// <param name="pchBytes">The reply size.</param>
			typedef void(*OnReply_Function)(TCHAR pchReply[BUFSIZE], DWORD pchBytes);

			/// <summary>
			/// Named pipe client, blocking.
			/// </summary>
			class NamedPipe
			{
			public:
				/// <summary>
				/// Named pipe client.
				/// </summary>
				NamedPipe();

				/// <summary>
				/// Named pipe client destructor.
				/// </summary>
				virtual ~NamedPipe();

				/// <summary>
				/// Initialize the name pipe.
				/// </summary>
				/// <param name="namedPipeName">The named pipe name.</param>
				/// <param name="onReplyFunction">The reply call back function handler.</param>
				void Initialize(std::string namedPipeName, OnReply_Function onReplyFunction);

				/// <summary>
				/// Initialize the name pipe.
				/// </summary>
				/// <param name="lpvMessage">The message to send.</param>
				/// <param name="timeout">The time (miliseconds, default is 10 seconds) to wait for a named pipe instance, when named pipe is busy.</param>
				/// <returns>The result, -1 if failed.</returns>
				int Send(LPTSTR lpvMessage, DWORD timeout = 10000);

			private:
				bool _disposed;
				bool _init;

				std::string _namedPipeName;

				static HANDLE _hPipe;
				static OnReply_Function _onReplyFunction;

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