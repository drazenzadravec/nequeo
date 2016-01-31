/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaFormat.cpp
*  Purpose :       SIP MediaFormat class.
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

#include "MediaFormat.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// This structure contains all the information needed to completely describe a media.
/// </summary>
MediaFormat::MediaFormat()
{
}

/// <summary>
/// Gets or sets the media format id.
/// </summary>
unsigned MediaFormat::Id::get()
{
	return _id;
}

/// <summary>
/// Gets or sets the media format id.
/// </summary>
void MediaFormat::Id::set(unsigned value)
{
	_id = value;
}

/// <summary>
/// Gets or sets the media type.
/// </summary>
MediaType MediaFormat::Type::get()
{
	return _type;
}

/// <summary>
/// Gets or sets the media type.
/// </summary>
void MediaFormat::Type::set(MediaType value)
{
	_type = value;
}

/// <summary>
/// Get the media type.
/// </summary>
/// <param name="mediaType">The current media type.</param>
/// <returns>The media type.</returns>
MediaType MediaFormat::GetMediaTypeEx(pjmedia_type mediaType)
{
	switch (mediaType)
	{
	case PJMEDIA_TYPE_NONE:
		return MediaType::PJMEDIA_TYPE_NONE;
	case PJMEDIA_TYPE_AUDIO:
		return MediaType::PJMEDIA_TYPE_AUDIO;
	case PJMEDIA_TYPE_VIDEO:
		return MediaType::PJMEDIA_TYPE_VIDEO;
	case PJMEDIA_TYPE_APPLICATION:
		return MediaType::PJMEDIA_TYPE_APPLICATION;
	case PJMEDIA_TYPE_UNKNOWN:
		return MediaType::PJMEDIA_TYPE_UNKNOWN;
	default:
		return MediaType::PJMEDIA_TYPE_NONE;
	}
}

/// <summary>
/// Get the media type.
/// </summary>
/// <param name="mediaType">The current media type.</param>
/// <returns>The media type.</returns>
pjmedia_type MediaFormat::GetMediaType(MediaType mediaType)
{
	switch (mediaType)
	{
	case Nequeo::Net::PjSip::MediaType::PJMEDIA_TYPE_NONE:
		return pjmedia_type::PJMEDIA_TYPE_NONE;
	case Nequeo::Net::PjSip::MediaType::PJMEDIA_TYPE_AUDIO:
		return pjmedia_type::PJMEDIA_TYPE_AUDIO;
	case Nequeo::Net::PjSip::MediaType::PJMEDIA_TYPE_VIDEO:
		return pjmedia_type::PJMEDIA_TYPE_VIDEO;
	case Nequeo::Net::PjSip::MediaType::PJMEDIA_TYPE_APPLICATION:
		return pjmedia_type::PJMEDIA_TYPE_APPLICATION;
	case Nequeo::Net::PjSip::MediaType::PJMEDIA_TYPE_UNKNOWN:
		return pjmedia_type::PJMEDIA_TYPE_UNKNOWN;
	default:
		return pjmedia_type::PJMEDIA_TYPE_NONE;
	}
}