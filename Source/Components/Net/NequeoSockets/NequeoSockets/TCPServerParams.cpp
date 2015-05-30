/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TCPServerParams.cpp
*  Purpose :       TCPServerParams class.
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

#include "TCPServerParams.h"

namespace Nequeo {
	namespace Net {
		namespace Provider
		{
			TCPServerParams::TCPServerParams() :
				_threadIdleTime(10000000),
				_maxThreads(0),
				_maxQueued(64),
				_threadPriority(Nequeo::Threading::PRIO_NORMAL)
			{
			}


			TCPServerParams::~TCPServerParams()
			{
			}


			void TCPServerParams::setThreadIdleTime(const Nequeo::Timespan& milliseconds)
			{
				_threadIdleTime = milliseconds;
			}


			void TCPServerParams::setMaxThreads(int count)
			{
				_maxThreads = count;
			}


			void TCPServerParams::setMaxQueued(int count)
			{
				_maxQueued = count;
			}


			void TCPServerParams::setThreadPriority(Nequeo::Threading::Priority prio)
			{
				_threadPriority = prio;
			}
		}
	}
}