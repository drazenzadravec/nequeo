/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          HttpServer.h
*  Purpose :       Http web server class.
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
#include "GlobalNetServer.h"

#include "NequeoHttpBoostServer\MultiWebServer.h"

namespace Nequeo {
	namespace Net {
		namespace Http
		{
			/// <summary>
			/// Http web server.
			/// </summary>
			class EXPORT_NEQUEO_NET_SERVER_API HttpServer
			{
			public:
				/// <summary>
				/// Http web server.
				/// </summary>
				/// <param name="path">The root folder path (this is where all files are located).</param>
				HttpServer(const std::string& path);

				/// <summary>
				/// Http web server.
				/// </summary>
				~HttpServer();

				/// <summary>
				/// Set the muilti server contatiner list.
				/// </summary>
				/// <param name="path">The muilti server contatiner list.</param>
				inline void SetMultiServerContainer(const std::vector<MultiServerContainer>& containers)
				{
					_containers = containers;
				}

				/// <summary>
				/// Is initialise.
				/// </summary>
				/// <return>True if initialise; else false.</return>
				inline bool IsInitialise() const
				{
					return _initialised;
				}

				/// <summary>
				/// Initialise the servers.
				/// </summary>
				void Initialise();

				///	<summary>
				///	Start the servers.
				///	</summary>
				void Start();

				///	<summary>
				///	Stop the servers.
				///	</summary>
				void Stop();

			private:
				bool _disposed;
				bool _initialised;

				std::string _path;
				std::shared_ptr<MultiWebServer> _servers;
				std::vector<MultiServerContainer> _containers;
			};
		}
	}
}