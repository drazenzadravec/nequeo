/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          GetTheLights.h
*  Purpose :       GetTheLights class.
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

#include "Global.h"
#include "Allocator.h"

#include <functional>
#include <atomic>

namespace Nequeo
{
	/**
	* Make initialization and cleanup of shared resources less painful.
	* If you have this problem. Create a static instance of GetTheLights,
	* have each actor call Enter the room with your callable.
	*
	* When you are finished with the shared resources call LeaveRoom(). The last caller will
	* have its callable executed.
	*/
	class GetTheLights
	{
	public:
		GetTheLights();
		void EnterRoom(std::function<void()>&&);
		void LeaveRoom(std::function<void()>&&);
	private:
		std::atomic<int> m_value;
	};
}