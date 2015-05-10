/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Exception.h
*  Purpose :       Exception class.
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

#ifndef _EXCEPTION_H
#define _EXCEPTION_H

#include "GlobalException.h"

namespace Nequeo 
{
	/// This is the base class for all exceptions defined
	/// in the Nequeo class library.
	class Exception : public std::exception
	{
	public:
		Exception(const std::string& msg, int code = 0);
		/// Creates an exception.

		Exception(const std::string& msg, const std::string& arg, int code = 0);
		/// Creates an exception.

		Exception(const std::string& msg, const Exception& nested, int code = 0);
		/// Creates an exception and stores a clone
		/// of the nested exception.

		Exception(const Exception& exc);
		/// Copy constructor.

		~Exception() throw();
		/// Destroys the exception and deletes the nested exception.

		Exception& operator = (const Exception& exc);
		/// Assignment operator.

		virtual const char* name() const throw();
		/// Returns a static string describing the exception.

		virtual const char* className() const throw();
		/// Returns the name of the exception class.

		virtual const char* what() const throw();
		/// Returns a static string describing the exception.
		///
		/// Same as name(), but for compatibility with std::exception.

		const Exception* nested() const;
		/// Returns a pointer to the nested exception, or
		/// null if no nested exception exists.

		const std::string& message() const;
		/// Returns the message text.

		int code() const;
		/// Returns the exception code if defined.

		std::string displayText() const;
		/// Returns a string consisting of the
		/// message name and the message text.

		virtual Exception* clone() const;
		/// Creates an exact copy of the exception.
		///
		/// The copy can later be thrown again by
		/// invoking rethrow() on it.

		virtual void rethrow() const;
		/// (Re)Throws the exception.
		///
		/// This is useful for temporarily storing a
		/// copy of an exception (see clone()), then
		/// throwing it again.

	protected:
		Exception(int code = 0);
		/// Standard constructor.

		void message(const std::string& msg);
		/// Sets the message for the exception.

		void extendedMessage(const std::string& arg);
		/// Sets the extended message for the exception.

	private:
		std::string _msg;
		Exception*  _pNested;
		int			_code;
	};
}
#endif