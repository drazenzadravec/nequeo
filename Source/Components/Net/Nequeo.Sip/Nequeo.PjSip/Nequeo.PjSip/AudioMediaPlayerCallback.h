/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          AudioMediaPlayerCallback.h
*  Purpose :       SIP Audio MediaPlayer Callbackclass.
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

#ifndef _AUDIOMEDIAPLAYERCALLBACK_H
#define _AUDIOMEDIAPLAYERCALLBACK_H

#include "stdafx.h"

#include "pjsua2\media.hpp"
#include "pjsua2.hpp"

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			typedef bool(*OnPlayerEndOfFile_Function)();

			///	<summary>
			///	Audio media player callbacks.
			///	</summary>
			class AudioMediaPlayerCallback : public pj::AudioMediaPlayer
			{
			public:
				///	<summary>
				///	Audio media player callbacks.
				///	</summary>
				AudioMediaPlayerCallback();

				///	<summary>
				///	Audio media player callbacks.
				///	</summary>
				virtual ~AudioMediaPlayerCallback();

				///	<summary>
				///	Register a callback to be called when the file player reading has
				/// reached the end of file, or when the file reading has reached the
				/// end of file of the last file for a playlist.If the file or playlist
				/// is set to play repeatedly, then the callback will be called multiple
				/// times.
				///	</summary>
				/// <returns>If the callback returns false, the playback
				/// will stop.Note that if application destroys
				/// the player in the callback, it must return
				/// false here.</returns>
				bool onEof();

				///	<summary>
				///	Set the on player end of file function callback.
				///	</summary>
				/// <param name="onPlayerEndOfFileCallBack">The on player end of file function callback.</param>
				void Set_OnPlayerEndOfFile_Function(OnPlayerEndOfFile_Function onPlayerEndOfFileCallBack);

			private:
				bool _disposed;

				OnPlayerEndOfFile_Function _onPlayerEndOfFile_function_internal;
			};
		}
	}
}
#endif