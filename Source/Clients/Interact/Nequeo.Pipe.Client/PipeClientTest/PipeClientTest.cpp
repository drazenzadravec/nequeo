// PipeClientTest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include "NamedPipe.h"

using namespace Nequeo::Pipe::Client;

VOID GetReply(TCHAR pchReply[BUFSIZE], DWORD pchBytes);

int main()
{
	NamedPipe pipe;
	pipe.Initialize("\\\\.\\pipe\\nequeopipe", GetReply);
	pipe.Send(TEXT("Hello pipe server."));

    return 0;
}

VOID GetReply(TCHAR pchReply[BUFSIZE], DWORD pchBytes)
{
	_tprintf(TEXT("\"%s\"\n"), pchReply);
}