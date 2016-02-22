/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          TimeVal.cpp
*  Purpose :       SIP TimeVal class.
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

#include "TimeVal.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	Representation of time value.
///	</summary>
TimeVal::TimeVal() :
	_disposed(false)
{
}

///	<summary>
///	Representation of time value.
///	</summary>
TimeVal::~TimeVal()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	Gets or sets the seconds part of the time.
///	</summary>
long TimeVal::Seconds::get()
{
	return _seconds;
}

///	<summary>
///	Gets or sets the seconds part of the time.
///	</summary>
void TimeVal::Seconds::set(long value)
{
	_seconds = value;
}

///	<summary>
///	Gets or sets the milliseconds fraction of the time.
///	</summary>
long TimeVal::Milliseconds::get()
{
	return _milliseconds;
}

///	<summary>
///	Gets or sets the milliseconds fraction of the time.
///	</summary>
void TimeVal::Milliseconds::set(long value)
{
	_milliseconds = value;
}