// PipeServerTest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include "NamedPipe.h"

using namespace Nequeo::Pipe::Server;

/// <summary>
/// Get answer to request.
/// </summary>
/// <param name="pchRequest">The request message.</param>
/// <param name="pchReply">The reply message.</param>
/// <param name="pchBytes">The reply size.</param>
/// <return>The result.</return>
VOID GetAnswerToRequest(LPTSTR pchRequest, LPTSTR pchReply, LPDWORD pchBytes);

int main()
{
	NamedPipe pipe;
	pipe.Initialize("\\\\.\\pipe\\nequeopipe", GetAnswerToRequest);
	pipe.Start();

    return 0;
}

/// <summary>
/// Get answer to request.
/// </summary>
/// <param name="pchRequest">The request message.</param>
/// <param name="pchReply">The reply message.</param>
/// <param name="pchBytes">The reply size.</param>
/// <return>The result.</return>
VOID GetAnswerToRequest(LPTSTR pchRequest, LPTSTR pchReply, LPDWORD pchBytes)
{
	// Set the reply message.
	STRSAFE_LPCWSTR LreplyMessage = L"Reply from the server. Hello";

	// Check the outgoing message to make sure it's not too long for the buffer.
	if (FAILED(StringCchCopy(pchReply, BUFSIZE, TEXT(replyMessage))))
	{
		*pchBytes = 0;
		pchReply[0] = 0;
		return;
	}

	// Get the length of bytes.
	*pchBytes = (lstrlen(pchReply) + 1)*sizeof(TCHAR);

	_tprintf(TEXT("Got %d byte message: \"%s\"\n"), *pchBytes, pchRequest);
}