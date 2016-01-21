/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaEvent.cpp
*  Purpose :       SIP MediaEvent class.
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

#include "MediaEvent.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	This structure describes a media event.
///	</summary>
MediaEvent::MediaEvent()
{
}

/// <summary>
/// Gets or sets the Additional data/parameters about the event. The type of data
/// will be specific to the event type being reported.
/// </summary>
MediaEventData^ MediaEvent::Data::get()
{
	return _data;
}

/// <summary>
/// Gets or sets the Additional data/parameters about the event. The type of data
/// will be specific to the event type being reported.
/// </summary>
void MediaEvent::Data::set(MediaEventData^ value)
{
	_data = value;
}

/// <summary>
/// Gets or sets the event type.
/// </summary>
MediaEventType MediaEvent::Type::get()
{
	return _type;
}

/// <summary>
/// Gets or sets the event type.
/// </summary>
void MediaEvent::Type::set(MediaEventType value)
{
	_type = value;
}