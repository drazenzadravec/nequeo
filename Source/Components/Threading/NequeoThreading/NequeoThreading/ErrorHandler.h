/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          ErrorHandler.h
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

#pragma once

#ifndef _ERRORHANDLER_H
#define _ERRORHANDLER_H

#include "GlobalThreading.h"

#include "Exceptions\Exception.h"
#include "Mutex.h"

namespace Nequeo {
	namespace Threading
	{
		/// This is the base class for thread error handlers.
		///
		/// An unhandled exception that causes a thread to terminate is usually
		/// silently ignored, since the class library cannot do anything meaningful
		/// about it.
		/// 
		/// The Thread class provides the possibility to register a
		/// global ErrorHandler that is invoked whenever a thread has
		/// been terminated by an unhandled exception.
		/// The ErrorHandler must be derived from this class and can
		/// provide implementations of all three exception() overloads.
		///
		/// The ErrorHandler is always invoked within the context of
		/// the offending thread.
		class ErrorHandler
		{
		public:
			ErrorHandler();
			/// Creates the ErrorHandler.

			virtual ~ErrorHandler();
			/// Destroys the ErrorHandler.

			virtual void exception(const Exception& exc);
			/// Called when a Poco::Exception (or a subclass)
			/// caused the thread to terminate.
			///
			/// This method should not throw any exception - it would
			/// be silently ignored.
			///
			/// The default implementation just breaks into the debugger.

			virtual void exception(const std::exception& exc);
			/// Called when a std::exception (or a subclass)
			/// caused the thread to terminate.		
			///
			/// This method should not throw any exception - it would
			/// be silently ignored.
			///
			/// The default implementation just breaks into the debugger.

			virtual void exception();
			/// Called when an exception that is neither a
			/// Poco::Exception nor a std::exception caused
			/// the thread to terminate.
			///
			/// This method should not throw any exception - it would
			/// be silently ignored.
			///
			/// The default implementation just breaks into the debugger.

			static void handle(const Exception& exc);
			/// Invokes the currently registered ErrorHandler.

			static void handle(const std::exception& exc);
			/// Invokes the currently registered ErrorHandler.

			static void handle();
			/// Invokes the currently registered ErrorHandler.

			static ErrorHandler* set(ErrorHandler* pHandler);
			/// Registers the given handler as the current error handler.
			///
			/// Returns the previously registered handler.

			static ErrorHandler* get();
			/// Returns a pointer to the currently registered
			/// ErrorHandler.

		protected:
			static ErrorHandler* defaultHandler();
			/// Returns the default ErrorHandler.	

		private:
			static ErrorHandler* _pHandler;
			static FastMutex     _mutex;
		};


		//
		// inlines
		//
		inline ErrorHandler* ErrorHandler::get()
		{
			return _pHandler;
		}
	}
}
#endif
