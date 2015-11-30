/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          HttpServer.cpp
 *  Purpose :       The http server control class.
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

#include "HttpServer.h"


// Define the server context.
SERVER_CONTEXT m_ServerContext;

///	<summary>
///	Construct the http server.
///	</summary>
/// <param name='urlBaseAddress'>The Url base this sample will listen for. The URL must not have a query element.</param>
/// <param name='localBaseDirectory'>The local directory to map incoming requested Url to. Note that this path should not include a trailing slash.</param>
/// <exception cref="System::ArgumentNullException">Thrown when the urlBaseAddress parameter is missing.</exception>
/// <exception cref="System::ArgumentNullException">Thrown when the localBaseDirectory parameter is missing.</exception>
Nequeo::Net::Server::HttpServer::HttpServer(String^ urlBaseAddress, String^ localBaseDirectory) : _disposed(false)
{
	// Make sure the url base address is set.
	if (System::String::IsNullOrEmpty(urlBaseAddress))
        throw gcnew ArgumentNullException("urlBaseAddress");

	// Make sure the local base directory is set.
	if (System::String::IsNullOrEmpty(localBaseDirectory))
        throw gcnew ArgumentNullException("localBaseDirectory");

	// Assign the base server values.
	_urlBaseAddress = urlBaseAddress;
	_localBaseDirectory = localBaseDirectory;

	// Assign the control fields
	_disposed = false;
	_Initialized = false;
	_Started = false;
}

///	<summary>
///	Deconstruct the http server.
///	</summary>
Nequeo::Net::Server::HttpServer::~HttpServer()
{
	// If not disposed.
    if (!_disposed)
    {
		try
		{
			// Stop the server.
			Stop();
		}
		catch(Exception^ ex) {}
		finally {}

		try
		{
			// Un-Initialize the server.
			UninitializeServerIo(&m_ServerContext);
			_Initialized = false;
		}
		catch(Exception^ ex) {}
		finally {}

		try
		{
			// Un-Initialize the server.
			UninitializeHttpServer(&m_ServerContext);
			_Initialized = false;
		}
		catch(Exception^ ex) {}
		finally {}

        _disposed = true;
    }
}

///	<summary>
///	Initialize the http server.
///	</summary>
///	<returns>True if the server has been initialized: else false.</returns>
bool Nequeo::Net::Server::HttpServer::Initialize()
{
	// If the server has not been initialised.
	if(!_Initialized)
	{
		_Started = false;

		// Initialize the memory stack for the
		// server context handler.
		ZeroMemory(&m_ServerContext, sizeof(SERVER_CONTEXT));

		// Cast the CLR string to C type WCHAR pointer
		// for the url base address string value.
		pin_ptr<const wchar_t> pinnedUrlBaseAddress = PtrToStringChars(_urlBaseAddress);
		PWCHAR urlBaseAddress = const_cast<PWCHAR>(pinnedUrlBaseAddress);

		// Cast the CLR string to C type WCHAR pointer
		// for the local base directory string value.
		pin_ptr<const wchar_t> pinnedLocalBaseDirectory = PtrToStringChars(_localBaseDirectory);
		PWCHAR localBaseDirectory = const_cast<PWCHAR>(pinnedLocalBaseDirectory);

		// Initialize the server.
		if (InitializeHttpServer(urlBaseAddress, localBaseDirectory, &m_ServerContext))
		{
			// Initialize the server.
			if (InitializeServerIo(&m_ServerContext))
			{
				_Initialized = true;
			}
			else
			{
				// Un-Initialize the server.
				UninitializeServerIo(&m_ServerContext);
				UninitializeHttpServer(&m_ServerContext);
				_Initialized = false;
			}
		}
		else
		{
			// Un-Initialize the server.
			UninitializeHttpServer(&m_ServerContext);
			_Initialized = false;
		}
	}

	// Return the current initialize state.
	return _Initialized;
}

///	<summary>
///	Start the http server.
///	</summary>
///	<returns>True if the server has been started: else false.</returns>
/// <exception cref="System::Exception">Thrown when the server has not been initialised.</exception>
bool Nequeo::Net::Server::HttpServer::Start()
{
	// If the server has been not been started.
	if(!_Started)
	{
		// If the server has been initialised.
		if(_Initialized)
		{
			// Start the server listening.
			StartServer(&m_ServerContext);
			_Started = true;
		}
		else
		{
			// Throw exception if not initialised.
			throw gcnew Exception("Initialize the http server before starting.");
		}
	}

	// Return the current start state.
	return _Started;
}

///	<summary>
///	Stop the http server.
///	</summary>
///	<returns>True if the server has been stopped: else false.</returns>
bool Nequeo::Net::Server::HttpServer::Stop()
{
	// If the server has been started.
	if(_Started)
	{
		_Started = false;

		// If the server has been initialised.
		if(_Initialized)
		{
			// Stop the server.
			StopServer(&m_ServerContext);
		}
	}

	return true;
}