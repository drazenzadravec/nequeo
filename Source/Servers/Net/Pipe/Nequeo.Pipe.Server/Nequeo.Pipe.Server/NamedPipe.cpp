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

using namespace Nequeo::Pipe::Server;

HANDLE NamedPipe::_hPipe = INVALID_HANDLE_VALUE;
OnRequest_Function NamedPipe::_onRequestFunction = NULL;

///	<summary>
///	Named pipe server.
///	</summary>
NamedPipe::NamedPipe() :
	_disposed(false), _isRunning(true), _started(false), _init(false),
	_namedPipeName("\\\\.\\pipe\\nequeopipeservernamedpipe")
{
}

///	<summary>
///	Named pipe server destructor.
///	</summary>
NamedPipe::~NamedPipe()
{
	// If not disposed.
	if (!_disposed)
	{
		_disposed = true;
		_started = false;
		_isRunning = false;

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
void NamedPipe::Initialize(std::string namedPipeName, OnRequest_Function onRequestFunction)
{
	_namedPipeName = namedPipeName;
	NamedPipe::_onRequestFunction = onRequestFunction;
	_init = true;
}

/// <summary>
/// Start the named pipe server.
/// </summary>
void NamedPipe::Start()
{
	// If not started and initialized.
	if (!_started && _init)
	{
		// Get the named pipe name.
		std::wstring namedPipeName = StringToWstring(_namedPipeName);

		HANDLE hPipe = INVALID_HANDLE_VALUE;
		HANDLE hThread = NULL;

		BOOL   fConnected = FALSE;
		DWORD  dwThreadId = 0;

		bool exited = false;
		_started = true;
		_isRunning = true;

		// The main loop creates an instance of the named pipe and 
		// then waits for a client to connect to it. When the client 
		// connects, a thread is created to handle communications 
		// with that client, and this loop is free to wait for the
		// next client connect request. It is an infinite loop.
		while (_isRunning)
		{
			// Create the named pipe.
			hPipe = CreateNamedPipe(
				namedPipeName.c_str(),    // pipe name 
				PIPE_ACCESS_DUPLEX,       // read/write access 
				PIPE_TYPE_MESSAGE |       // message type pipe 
				PIPE_READMODE_MESSAGE |   // message-read mode 
				PIPE_WAIT,                // blocking mode 
				PIPE_UNLIMITED_INSTANCES, // max. instances  
				BUFSIZE,                  // output buffer size 
				BUFSIZE,                  // input buffer size 
				0,                        // client time-out 
				NULL);                    // default security attribute 

			// If the pipe is invalid.
			if (hPipe == INVALID_HANDLE_VALUE)
			{
				// Break out of the loop.
				exited = true;
				NamedPipe::_hPipe = INVALID_HANDLE_VALUE;
				break;
			}
			else
			{
				// Copy the current pipe handle.
				NamedPipe::_hPipe = hPipe;
			}

			// Wait for the client to connect; if it succeeds, 
			// the function returns a nonzero value. If the function
			// returns zero, GetLastError returns ERROR_PIPE_CONNECTED. 
			fConnected = ConnectNamedPipe(hPipe, NULL) ? TRUE : (GetLastError() == ERROR_PIPE_CONNECTED);

			// If a client has been connected.
			if (fConnected)
			{
				// Create a thread for this client. 
				hThread = CreateThread(
					NULL,						// no security attribute 
					0,							// default stack size 
					NamedPipe::InstanceThread,  // thread proc
					(LPVOID)hPipe,				// thread parameter 
					0,							// not suspended 
					&dwThreadId);				// returns thread ID 

				// If a thread has not been created.
				if (hThread == NULL)
				{
					// Break out of the loop.
					exited = true;
					break;
				}
				else
				{
					// Close the thread handle.
					CloseHandle(hThread);
				}
			}
			else
				// The client could not connect, so close the pipe. 
				CloseHandle(hPipe);
		}

		// If not exited.
		if (exited)
		{
			// If the pipe handle is valid.
			if (hPipe != INVALID_HANDLE_VALUE)
			{
				// Close the pipe handle.
				NamedPipe::_hPipe = INVALID_HANDLE_VALUE;
				CloseHandle(hPipe);
			}
		}
	}
}

/// <summary>
/// Stop the named pipe server.
/// </summary>
void NamedPipe::Stop()
{
	// Started.
	_started = false;
	_isRunning = false;

	// If the pipe handle is valid
	if (NamedPipe::_hPipe != INVALID_HANDLE_VALUE)
	{
		// Close the pipe handle.
		CloseHandle(NamedPipe::_hPipe);

		// Set the invalid pipe handle.
		NamedPipe::_hPipe = INVALID_HANDLE_VALUE;
	}
}

/// <summary>
/// Initialize the name pipe.
/// </summary>
/// <param name="namedPipeName">A state passed parameter.</param>
/// <return>The result.</return>
DWORD WINAPI NamedPipe::InstanceThread(LPVOID lpvParam)
{
	// This routine is a thread processing function to read from and reply to a client
	// via the open pipe connection passed from the main loop. Note this allows
	// the main loop to continue executing, potentially creating more threads of
	// of this procedure to run concurrently, depending on the number of incoming
	// client connections.

	HANDLE hHeap = GetProcessHeap();
	TCHAR* pchRequest = (TCHAR*)HeapAlloc(hHeap, 0, BUFSIZE*sizeof(TCHAR));
	TCHAR* pchReply = (TCHAR*)HeapAlloc(hHeap, 0, BUFSIZE*sizeof(TCHAR));

	DWORD cbBytesRead = 0, cbReplyBytes = 0, cbWritten = 0;
	BOOL fSuccess = FALSE;
	HANDLE hPipe = NULL;

	// Do some extra error checking since the app will keep running even if this
	// thread fails.
	if (lpvParam == NULL)
	{
		if (pchReply != NULL) HeapFree(hHeap, 0, pchReply);
		if (pchRequest != NULL) HeapFree(hHeap, 0, pchRequest);
		return (DWORD)-1;
	}

	if (pchRequest == NULL)
	{
		if (pchReply != NULL) HeapFree(hHeap, 0, pchReply);
		return (DWORD)-1;
	}

	if (pchReply == NULL)
	{
		if (pchRequest != NULL) HeapFree(hHeap, 0, pchRequest);
		return (DWORD)-1;
	}

	// The thread's parameter is a handle to a pipe object instance. 
	hPipe = (HANDLE)lpvParam;

	// Loop until done reading.
	while (1)
	{
		// Read client requests from the pipe. This simplistic code only allows messages
		// up to BUFSIZE characters in length.
		fSuccess = ReadFile(
			hPipe,					// handle to pipe 
			pchRequest,				// buffer to receive data 
			BUFSIZE*sizeof(TCHAR),	// size of buffer 
			&cbBytesRead,			// number of bytes read 
			NULL);					// not overlapped I/O 

		// If read failed.
		if (!fSuccess || cbBytesRead == 0)
		{
			// Could not read.
			break;
		}

		// Process the incoming message.
		GetAnswerToRequest(pchRequest, pchReply, &cbReplyBytes);

		// Write the reply to the pipe. 
		fSuccess = WriteFile(
			hPipe,        // handle to pipe 
			pchReply,     // buffer to write from 
			cbReplyBytes, // number of bytes to write 
			&cbWritten,   // number of bytes written 
			NULL);        // not overlapped I/O 

		// If write failed.
		if (!fSuccess || cbReplyBytes != cbWritten)
		{
			// Could not write.
			break;
		}
	}

	// Flush the pipe to allow the client to read the pipe's contents 
	// before disconnecting. Then disconnect the pipe, and close the 
	// handle to this pipe instance. 
	FlushFileBuffers(hPipe);
	DisconnectNamedPipe(hPipe);
	CloseHandle(hPipe);

	// Set the pipe handle copy to invalid handle.
	NamedPipe::_hPipe = INVALID_HANDLE_VALUE;

	// Clean up.
	HeapFree(hHeap, 0, pchRequest);
	HeapFree(hHeap, 0, pchReply);

	// Exit the thread.
	return 1;
}

/// <summary>
/// Get answer to request.
/// </summary>
/// <param name="pchRequest">The request message.</param>
/// <param name="pchReply">The reply message.</param>
/// <param name="pchBytes">The reply size.</param>
/// <return>The result.</return>
VOID NamedPipe::GetAnswerToRequest(LPTSTR pchRequest, LPTSTR pchReply, LPDWORD pchBytes)
{
	// This routine is a simple function to print the client request to the console
	// and populate the reply buffer with a default data string. This is where you
	// would put the actual client request processing code that runs in the context
	// of an instance thread. Keep in mind the main thread will continue to wait for
	// and receive other client connections while the instance thread is working.

	NamedPipe::_onRequestFunction(pchRequest, pchReply, pchBytes);
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