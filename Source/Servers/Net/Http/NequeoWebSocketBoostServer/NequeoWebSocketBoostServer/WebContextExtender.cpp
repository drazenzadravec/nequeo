/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WebContextExtender.cpp
*  Purpose :       WebSocket web context class.
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

#include <iostream>
#include <boost/asio.hpp>
#include <boost/date_time/posix_time/posix_time.hpp>

namespace Nequeo {
	namespace Net {
		namespace WebSocket
		{
			/// <summary>
			/// Web context extender.
			/// </summary>
			class WebContextExtender
			{
			public:
				/// <summary>
				/// Web context extender.
				/// </summary>
				WebContextExtender() :
					_disposed(false), _initialised(false), _hasAccessExpiryBeenInit(false)
				{
				}

				/// <summary>
				/// Web context extender.
				/// </summary>
				~WebContextExtender()
				{
					if (!_disposed)
					{
						_disposed = true;
						_initialised = false;
						_hasAccessExpiryBeenInit = false;
					}
				}

				///	<summary>
				///	Init the access expiry.
				///	</summary>
				void InitAccessExpiry()
				{
					_hasAccessExpiryBeenInit = true;
				}

				///	<summary>
				///	Has the access expiry been set.
				///	</summary>
				bool HasAccessExpiryBeenInit()
				{
					return _hasAccessExpiryBeenInit;
				}
			
			private:
				bool _disposed;
				bool _initialised;
				bool _hasAccessExpiryBeenInit;
			};
		}
	}
}