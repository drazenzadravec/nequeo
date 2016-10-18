/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          FastRandom.cpp
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

#include "stdafx.h"

#include "FastRandom.h"

using namespace Nequeo;

const unsigned FastRandom::Primes[] = 
{
	0x9e3779b1, 0xffe6cc59, 0x2109f6dd, 0x43977ab5,
	0xba5703f5, 0xb495a877, 0xe1626741, 0x79695e6b,
	0xbc98c09f, 0xd5bee2b3, 0x287488f9, 0x3af18231,
	0x9677cd4d, 0xbe3a6929, 0xadc6a877, 0xdcf0674b,
	0xbe4d6fe9, 0x5f15e201, 0x99afc3fd, 0xf3f16801,
	0xe222cfff, 0x24ba5fdb, 0x0620452d, 0x79f149e3,
	0xc8b93f49, 0x972702cd, 0xb07dd827, 0x6c97d5ed,
	0x085a3d61, 0x46eb5ea7, 0x3d9910ed, 0x2e687b5b,
	0x29609227, 0x6eb081f1, 0x0954c4e1, 0x9d114db9,
	0x542acfa9, 0xb3e6bd7b, 0x0742d917, 0xe9f3ffa7,
	0x54581edb, 0xf2480f45, 0x0bb9288f, 0xef1affc7,
	0x85fa0ca7, 0x3ccc14db, 0xe6baf34b, 0x343377f7,
	0x5ca19031, 0xe6d9293b, 0xf0a9f391, 0x5d2e980b,
	0xfc411073, 0xc3749363, 0xb892d829, 0x3549366b,
	0x629750ad, 0xb98294e5, 0x892d9483, 0xc235baf3,
	0x3d2402a3, 0x6bdef3c9, 0xbec333cd, 0x40c9520f
};

///	<summary>
///	A fast random number generator. Uses linear congruential method.
///	</summary>
FastRandom::FastRandom(size_t seed) : _disposed(false)
{
	_x = seed * GetPrime(seed);
	_a = GetPrime(_x);
}

///	<summary>
///	A fast random number generator. Uses linear congruential method.
///	</summary>
FastRandom::~FastRandom()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Get a random number.
/// </summary>
unsigned short FastRandom::Get()
{
	return Get(_x);
}

/// <summary>
/// Get a random number for the given seed; update the seed for next use.
/// </summary>
unsigned short FastRandom::Get(size_t& seed)
{
	unsigned short r = (unsigned short)(seed >> 16);
	seed = (seed * _a) + 1;
	return r;
}

/// <summary>
/// Get a random number.
/// </summary>
size_t FastRandom::GetPrime(size_t seed)
{
	return Primes[seed % (sizeof(Primes) / sizeof(Primes[0]))];
}