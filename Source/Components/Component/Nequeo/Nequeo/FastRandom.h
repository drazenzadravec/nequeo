/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          FastRandom.h
*  Purpose :       Fast Random definition header.
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

#ifndef _FASTRANDOM_H
#define _FASTRANDOM_H

#include "Global.h"

namespace Nequeo
{
	/// <summary>
	/// A fast random number generator. Uses linear congruential method.
	/// </summary>
	class FastRandom
	{
	public:
		/// <summary>
		/// A fast random number generator. Uses linear congruential method.
		/// </summary>
		FastRandom(size_t seed);
		virtual ~FastRandom();

		/// <summary>
		/// Get a random number.
		/// </summary>
		unsigned short Get();

		/// <summary>
		/// Get a random number for the given seed; update the seed for next use.
		/// </summary>
		unsigned short Get(size_t& seed);
		
	private:
		bool _disposed;

		size_t _x;
		size_t _a;

		static const unsigned Primes[];
		size_t GetPrime(size_t seed);
	};
}
#endif