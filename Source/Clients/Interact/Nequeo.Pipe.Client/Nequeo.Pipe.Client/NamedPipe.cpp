/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          NamedPipe.cpp
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

#include "stdafx.h"

#include "NamedPipe.h"

using namespace Nequeo::Pipe::Client;

HANDLE NamedPipe::_hPipe = INVALID_HANDLE_VALUE;
OnReply_Function NamedPipe::_onReplyFunction = NULL;

///	<summary>
///	Named pipe client.
///	</summary>
NamedPipe::NamedPipe() :
	_disposed(false), _init(false), _namedPipeName("\\\\.\\pipe\\nequeopipeservernamedpipe")
{
}

///	<summary>
///	Named pipe client destructor.
///	</summary>
NamedPipe::~NamedPipe()
{
	// If not disposed.
	if (!_disposed)
	{
		_disposed = true;

		// If the pipe handle is valid
		if (NamedPipe::_hPipe != INVALID_HANDLE_VALUE)
		{
			// Close the pipe handle.
			CloseHandle(NamedPipe::_hPipe);
		}
	}
}

/// <summary>
/// Initialize the name pipe.
/// </summary>
/// <param name="namedPipeName">The named pipe name.</param>
/// <param name="onRequestFunction">The request call back function handler.</param>
void NamedPipe::Initialize(std::string namedPipeName, OnReply_Function onReplyFunction)
{
	_namedPipeName = namedPipeName;
	NamedPipe::_onReplyFunction = onReplyFunction;
	_init = true;
}

/// <summary>
/// Initialize the name pipe.
/// </summary>
/// <param name="lpvMessage">The message to send.</param>
/// <param name="timeout">The time (miliseconds, default is 10 seconds) to wait for a named pipe instance, when named pipe is busy.</param>
/// <returns>The result, -1 if failed.</returns>
int NamedPipe::Send(LPTSTR lpvMessage, DWORD timeout)
{
	// If initialized.
	if (_init)
	{
		// Get the named pipe name.
		std::wstring namedPipeName = StringToWstring(_namedPipeName);

		HANDLE hPipe = INVALID_HANDLE_VALUE;
		TCHAR  chBuf[BUFSIZE];
		BOOL   fSuccess = FALSE;
		DWORD  cbRead, cbToWrite, cbWritten, dwMode;

		// Wait for a message.
		while (1)
		{
			// Open a handle for the named pipe.
			hPipe = CreateFile(
				namedPipeName.c_str(),  // pipe name 
				GENERIC_READ |			// read and write access 
				GENERIC_WRITE,
				0,						// no sharing 
				NULL,					// default security attributes
				OPEN_EXISTING,			// opens existing pipe 
				0,						// default attributes 
				NULL);					// no template file

			// If invalid handle.
			if (hPipe != INVALID_HANDLE_VALUE)
			{
				// Stop the process.
				break;
			}
			else
			{
				// Assign the global handle.
				NamedPipe::_hPipe = hPipe;
			}

			// Exit if an error other than ERROR_PIPE_BUSY occurs. 
			if (GetLastError() != ERROR_PIPE_BUSY)
			{
				// Could not open pipe.
				return -1;
			}

			// All pipe instances are busy. 
			if (!WaitNamedPipe(namedPipeName.c_str(), timeout))
			{
				// Could not open pipe: 20 second wait timed out.
				return -1;
			}
		}

		// The pipe connected; change to message-read mode. 
		dwMode = PIPE_READMODE_MESSAGE;

		// Set the named pipe handle state to
		fSuccess = SetNamedPipeHandleState(
			hPipe,    // pipe handle 
			&dwMode,  // new pipe mode 
			NULL,     // don't set maximum bytes 
			NULL);    // don't set maximum time 

		if (!fSuccess)
		{
			// Unable to set the state.
			return -1;
		}

		// Get the message size.
		cbToWrite = (lstrlen(lpvMessage) + 1)*sizeof(TCHAR);
		
		// Send a message to the pipe server. 
		fSuccess = WriteFile(
			hPipe,                  // pipe handle 
			lpvMessage,             // message 
			cbToWrite,              // message length 
			&cbWritten,             // bytes written 
			NULL);                  // not overlapped 

		// Could not write to the pipe stream.
		if (!fSuccess)
		{
			// WriteFile to pipe failed.
			return -1;
		}

		// Repeat loop if ERROR_MORE_DATA.
		do
		{
			// Read from the pipe. 
			fSuccess = ReadFile(
				hPipe,					// pipe handle 
				chBuf,					// buffer to receive reply 
				BUFSIZE*sizeof(TCHAR),  // size of buffer 
				&cbRead,				// number of bytes read 
				NULL);					// not overlapped 

			// If more data exists, but a read error occured.
			if (!fSuccess && GetLastError() != ERROR_MORE_DATA)
				break;

		} while (!fSuccess);

		//  Could not read to the pipe stream.
		if (!fSuccess)
		{
			// ReadFile from pipe failed.
			return -1;
		}

		// Call the function handler.
		NamedPipe::_onReplyFunction(chBuf, cbRead);

		// Close the pipe handle.
		CloseHandle(hPipe);

		// If the handle was closed.
		NamedPipe::_hPipe = INVALID_HANDLE_VALUE;

		// Return the result.
		return 0;
	}
	else
	{
		// Failed.
		return -1;
	}
}

///	<summary>
///	Convert from wide string to string.
///	</summary>
/// <param name="wstr">The wide string.</param>
/// <returns>The result.</returns>
std::string NamedPipe::WstringToString(std::wstring wstr)
{
	std::string str(wstr.length(), ' ');
	copy(wstr.begin(), wstr.end(), str.begin());
	return str;
}

///	<summary>
///	Convert from string to wide string.
///	</summary>
/// <param name="str">The string.</param>
/// <returns>The result.</returns>
std::wstring NamedPipe::StringToWstring(std::string str)
{
	std::wstring wstr(str.length(), L' ');
	copy(str.begin(), str.end(), wstr.begin());
	return wstr;
}