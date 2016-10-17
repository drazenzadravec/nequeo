/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaSize.cpp
*  Purpose :       SIP MediaSize class.
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

#include "MediaSize.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// Representation of media size.
/// </summary>
MediaSize::MediaSize() : _disposed(false)
{
}

///	<summary>
///	Representation of media size.
///	</summary>
MediaSize::~MediaSize()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Gets or sets the width.
/// </summary>
unsigned MediaSize::Width::get()
{
	return _width;
}

/// <summary>
/// Gets or sets the width.
/// </summary>
void MediaSize::Width::set(unsigned value)
{
	_width = value;
}

/// <summary>
/// Gets or sets the height.
/// </summary>
unsigned MediaSize::Height::get()
{
	return _height;
}

/// <summary>
/// Gets or sets the height.
/// </summary>
void MediaSize::Height::set(unsigned value)
{
	_height = value;
}