/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Outcome.h
*  Purpose :       Outcome class.
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

#ifndef _OUTCOME_H
#define _OUTCOME_H

#include "Global.h"
#include <utility>

namespace Nequeo
{
	/**
	* Template class representing the outcome of making a request.  It will contain
	* either a successful result or the failure error.  The caller must check
	* whether the outcome of the request was a success before attempting to access
	*  the result or the error.
	*/
	template<typename R, typename E> // Result, Error
	class Outcome
	{
	public:

		Outcome() : success(false)
		{
		} // Default constructor
		Outcome(const R& r) : result(r), success(true)
		{
		} // Result copy constructor
		Outcome(const E& e) : error(e), success(false)
		{
		} // Error copy constructor
		Outcome(R&& r) : result(std::forward<R>(r)), success(true)
		{
		} // Result move constructor
		Outcome(E&& e) : error(std::forward<E>(e)), success(false)
		{
		} // Error move constructor
		Outcome(const Outcome& o) :
			result(o.result),
			error(o.error),
			success(o.success)
		{
		}

		Outcome& operator=(const Outcome& o)
		{
			if (this != &o)
			{
				result = o.result;
				error = o.error;
				success = o.success;
			}

			return *this;
		}

		Outcome(Outcome&& o) : // Required to force Move Constructor
			result(std::move(o.result)),
			error(std::move(o.error)),
			success(o.success)
		{
		}

		Outcome& operator=(Outcome&& o)
		{
			if (this != &o)
			{
				result = std::move(o.result);
				error = std::move(o.error);
				success = o.success;
			}

			return *this;
		}

		inline const R& GetResult() const
		{
			return result;
		}

		inline R& GetResult()
		{
			return result;
		}

		/**
		* casts the underlying result to an r-value so that caller can't take ownership of underlying resources.
		* this is necessary when streams are involved.
		*/
		inline R&& GetResultWithOwnership()
		{
			return std::move(result);
		}

		inline const E& GetError() const
		{
			return error;
		}

		inline bool IsSuccess() const
		{
			return this->success;
		}

	private:
		R result;
		E error;
		bool success;
	};
}
#endif