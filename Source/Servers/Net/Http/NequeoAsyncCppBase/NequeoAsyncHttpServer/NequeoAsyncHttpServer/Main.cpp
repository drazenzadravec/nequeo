// Abstract:
// 
//     This sample demonstrates how to create a simple HTTP server using the
//     HTTP API, v2. It does this using the system thread pool. 
// 
//     Threads within the thread pool receives I/O completions from the 
//     specified HTTPAPI request queue. They process these by calling the 
//     callback function according to the I/O context. As an example, we
//     send back an HTTP response to the specified HTTP request. If the request
//     was valid, the response will include the content of a file as the entity
//     body.
// 
//     Once compiled, to use this sample you would:
//
//     httpasyncserverapp <Url> <ServerDirectory>
//
//     where:
//
//     <Url>             is the Url base this sample will listen for.
//     <ServerDirectory> is the local directory to map incoming requested Url
//                       to locally.
//

#include "stdafx.h"

//
// Routine Description:
// 
//     Step by step: 
//          - checks the number of command line parameters,
//          - initializes the http server, if failed uninitializes the http server, 
//          - initializes http server Io completion object, if failed 
//            uninitializes it and uninitializes the http server,
//          - starts http server, if failed stops http server, uninitializes,
//            Io completion object object and uninitializes the http server.
//
//     Cleans-up upon user input. The clean up process consists of:
//
//          - uninitializes the http server,
//          - uninitializes Io completion object,
//          - uninitializes the http server. 
//
// Arguments:
//     argc - Contains the count of arguments that follow in argv. 
// 
//     argv - An array of null-terminated strings representing command-line 
//            Expected:
//              argv[1] - is the Url base this sample will listen for.
//              argv[2] - is the local directory to map incoming requested Url
//                        to locally.
//
// Return Value:
// 
//     Exit code.
// 

DWORD wmain(
            DWORD argc, 
            WCHAR **argv
            )
{
    SERVER_CONTEXT ServerContext;

    ZeroMemory(&ServerContext, sizeof(SERVER_CONTEXT));

    if (argc != 3)
	{
		printf("\n");
		printf("No arguments have been provided.\n");
		printf("<Url> <ServerDirectory>\n");
		printf("\n");
		printf("<Url> is the Url base this sample will listen for.\n");
		printf("<ServerDirectory> is the local directory to map incoming requested Url to locally.\n");
		return FALSE;
	}

    if (wcslen(argv[1]) > MAX_STR_SIZE ||
        wcslen(argv[2]) > MAX_STR_SIZE)
        return FALSE;

    if (!InitializeHttpServer(argv[1], argv[2], &ServerContext))
        goto CleanServer;

    if (!InitializeServerIo(&ServerContext))
        goto CleanIo;

    if (!StartServer(&ServerContext))
        goto StopServer;

    printf("HTTP server is running.\n");
    printf("Press any key to stop.\n");

    // Waiting for the user command.

    _getch();

StopServer:
    StopServer(&ServerContext);

CleanIo:
    UninitializeServerIo(&ServerContext);

CleanServer:
    UninitializeHttpServer(&ServerContext);

    printf("Done.\n");

    return 0;
}