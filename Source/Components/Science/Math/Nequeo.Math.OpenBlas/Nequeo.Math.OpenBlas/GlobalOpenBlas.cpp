/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          GlobalOpenBlas.cpp
*  Purpose :       Global OpenBlas.
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
#include "GlobalOpenBlas.h"

/// <summary>
/// Array of type T unique pointer.
/// </summary>
template <typename T> using array_ptr = std::unique_ptr<T[]>;

/// <summary>
/// Create a new array of type T.
/// </summary>
/// <param name="size">The size of the array.</param>
template<typename T>
inline array_ptr<T> array_new(const int size)
{
	return array_ptr<T>(new T[size]);
}

/// <summary>
/// Clone the array type T.
/// </summary>
/// <param name="size">The size of the array.</param>
/// <param name="array">The array to clone.</param>
template<typename T>
inline array_ptr<T> array_clone(const int size, const T* array)
{
	auto clone = array_new<T>(size);
	memcpy(clone.get(), array, size * sizeof(T));
	return clone;
}

/// <summary>
/// Shift ipvi down.
/// </summary>
/// <param name="m">The size of the array.</param>
/// <param name="ipiv">The array to shift down.</param>
inline void shift_ipiv_down(int m, int ipiv[])
{
	for (auto i = 0; i < m; ++i)
	{
		ipiv[i] -= 1;
	}
}

/// <summary>
/// Shift ipvi up.
/// </summary>
/// <param name="m">The size of the array.</param>
/// <param name="ipiv">The array to shift up.</param>
inline void shift_ipiv_up(int m, int ipiv[])
{
	for (auto i = 0; i < m; ++i)
	{
		ipiv[i] += 1;
	}
}

/// <summary>
/// Clone the array type T.
/// </summary>
/// <param name="m">The size of the array.</param>
/// <param name="n">The size of the array.</param>
/// <param name="a">The array to clone.</param>
template<typename T>
inline T* Clone(const int m, const int n, const T* a)
{
	auto clone = new T[m*n];
	memcpy(clone, a, m*n * sizeof(T));
	return clone;
}

/// <summary>
/// Copy b array to x array.
/// </summary>
/// <param name="m">The size of the array.</param>
/// <param name="n">The size of the array.</param>
/// <param name="bn">The size of the array.</param>
/// <param name="b">The b array to copy from.</param>
/// <param name="x">The x array to copy to.</param>
template<typename T>
inline void copyBtoX(int m, int n, int bn, T b[], T x[])
{
	for (auto i = 0; i < n; ++i)
	{
		for (auto j = 0; j < bn; ++j)
		{
			x[j * n + i] = b[j * m + i];
		}
	}
}
