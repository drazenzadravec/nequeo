/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          ErrorHandler.cpp
*  Purpose :       ErrorHandler class.
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

#include "ErrorHandler.h"
#include "SingletonHolder.h"

namespace Nequeo {
	namespace Threading
	{
		ErrorHandler* ErrorHandler::_pHandler = ErrorHandler::defaultHandler();
		FastMutex ErrorHandler::_mutex;


		ErrorHandler::ErrorHandler()
		{
		}


		ErrorHandler::~ErrorHandler()
		{
		}


		void ErrorHandler::exception(const Exception& exc)
		{
		}


		void ErrorHandler::exception(const std::exception& exc)
		{
		}


		void ErrorHandler::exception()
		{
		}


		void ErrorHandler::handle(const Exception& exc)
		{
			FastMutex::ScopedLock lock(_mutex);
			try
			{
				_pHandler->exception(exc);
			}
			catch (...)
			{
			}
		}


		void ErrorHandler::handle(const std::exception& exc)
		{
			FastMutex::ScopedLock lock(_mutex);
			try
			{
				_pHandler->exception(exc);
			}
			catch (...)
			{
			}
		}


		void ErrorHandler::handle()
		{
			FastMutex::ScopedLock lock(_mutex);
			try
			{
				_pHandler->exception();
			}
			catch (...)
			{
			}
		}


		ErrorHandler* ErrorHandler::set(ErrorHandler* pHandler)
		{
			if (pHandler != nullptr)
			{
				FastMutex::ScopedLock lock(_mutex);
				ErrorHandler* pOld = _pHandler;
				_pHandler = pHandler;
				return pOld;
			}
		}


		ErrorHandler* ErrorHandler::defaultHandler()
		{
			// NOTE: Since this is called to initialize the static _pHandler
			// variable, sh has to be a local static, otherwise we run
			// into static initialization order issues.
			static SingletonHolder<ErrorHandler> sh;
			return sh.get();
		}
	}
}