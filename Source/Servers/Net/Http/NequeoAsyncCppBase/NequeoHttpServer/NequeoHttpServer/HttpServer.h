/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          HttpServer.h
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

#pragma once

#include "stdafx.h"

using namespace System;
using namespace System::IO;

namespace Nequeo
{
	namespace Net
	{
		namespace Server
		{
			///	<summary>
			///	Http server provider interface.
			///	</summary>
			public interface class IHttpServer
			{
				public:
					///	<summary>
					///	Start the http server.
					///	</summary>
					///	<returns>True if the server has been started: else false.</returns>
					/// <exception cref="System::Exception">Thrown when the server has not been initialised.</exception>
					bool Start();

					///	<summary>
					///	Stop the http server.
					///	</summary>
					///	<returns>True if the server has been stopped: else false.</returns>
					bool Stop();

					///	<summary>
					///	Initialize the http server.
					///	</summary>
					///	<returns>True if the server has been initialized: else false.</returns>
					bool Initialize();
			};

			///	<summary>
			///	Http server provider.
			///	</summary>
			public ref class HttpServer sealed : public IHttpServer
			{
				public:
					// Constructors
					HttpServer(String^ urlBaseAddress, String^ localBaseDirectory);
					virtual ~HttpServer();

					// Methods
					virtual bool Start();
					virtual bool Stop();
					virtual bool Initialize();

				private:
					// Fields
					bool _disposed;
					String^ _urlBaseAddress;
					String^ _localBaseDirectory;
					bool _Initialized;
					bool _Started;
			
			};
		}
	}
}
