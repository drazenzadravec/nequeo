/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          AudioMedia.cpp
*  Purpose :       SIP AudioMedia class.
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

#include "AudioMedia.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// Audio media.
/// </summary>
/// <param name="pjAudioMedia">The pj audio media.</param>
AudioMedia::AudioMedia(pj::AudioMedia& pjAudioMedia) : _disposed(false), _pjAudioMedia(pjAudioMedia)
{
}

///	<summary>
///	Sip media manager deconstructor.
///	</summary>
AudioMedia::~AudioMedia()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Get port id.
/// </summary>
/// <returns>The port id.</returns>
int AudioMedia::GetPortId()
{
	return _pjAudioMedia.getPortId();
}

/// <summary>
/// Get the pj audio media.
/// </summary>
/// <returns>The pj audio media.</returns>
pj::AudioMedia& AudioMedia::GetAudioMedia()
{
	return _pjAudioMedia;
}