/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          AudioMedia.h
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

#pragma once

#ifndef _AUDIOMEDIA_H
#define _AUDIOMEDIA_H

#include "stdafx.h"

#include "Media.h"
#include "ConfPortInfo.h"
#include "CallAudioMedia.h"

#include "pjsua2\media.hpp"
#include "pjsua2.hpp"

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			/// <summary>
			/// Audio media.
			/// </summary>
			public ref class AudioMedia : public MediaBase
			{
			public:
				///	<summary>
				///	Audio media. deconstructor.
				///	</summary>
				virtual ~AudioMedia();

				/// <summary>
				/// Get information about the specified conference port.
				/// </summary>
				/// <returns>The conference port.</returns>
				ConfPortInfo^ GetPortInfo();

				/// <summary>
				/// Get port id.
				/// </summary>
				/// <returns>The port id.</returns>
				int GetPortId();

				/// <summary>
				/// Get information about the specified conference port.
				/// </summary>
				/// <param name="portId">The port id.</param>
				/// <returns>The conference port.</returns>
				static ConfPortInfo^ GetPortInfoFromId(int portId);

				/// <summary>
				/// Establish unidirectional media flow to sink. This media port
				/// will act as a source, and it may transmit to multiple destinations / sink.
				/// And if multiple sources are transmitting to the same sink, the media
				/// will be mixed together.Source and sink may refer to the same Media,
				/// effectively looping the media.
				///
				/// If bidirectional media flow is desired, application needs to call
				/// this method twice, with the second one called from the opposite source
				/// media.
				/// </summary>
				/// <param name="sink">The destination media.</param>
				void StartTransmit(AudioMedia^ sink);

				/// <summary>
				/// Stop media flow to destination/sink port.
				/// </summary>
				/// <param name="sink">The destination media.</param>
				void StopTransmit(AudioMedia^ sink);

				/// <summary>
				/// Adjust the signal level to be transmitted from the bridge to this
				/// media port by making it louder or quieter.
				/// </summary>
				/// <param name="level">Signal level adjustment. Value 1.0 means no level 
				/// adjustment, while value 0 means to mute the port.</param>
				void AdjustRxLevel(float level);

				/// <summary>
				/// Adjust the signal level to be received from this media port (to
				/// the bridge) by making it louder or quieter.
				/// </summary>
				/// <param name="level">Signal level adjustment. Value 1.0 means no level 
				/// adjustment, while value 0 means to mute the port.</param>
				void AdjustTxLevel(float level);

				/// <summary>
				/// Get the last received signal level.
				/// </summary>
				/// <returns>Signal level in percent.</returns>
				unsigned GetRxLevel();

				/// <summary>
				/// Get the last transmitted signal level.
				/// </summary>
				/// <returns>Signal level in percent.</returns>
				unsigned GetTxLevel();

				/// <summary>
				/// Typecast from base class MediaBase.
				/// </summary>
				/// <param name="media">The object to be downcasted.</param>
				/// <returns>The object as AudioMedia instance.</returns>
				static AudioMedia^ TypecastFromMedia(MediaBase^ media);

			protected:
				/// <summary>
				/// Gets or sets the conference port Id.
				/// </summary>
				property int Id
				{
					int get();
					void set(int value);
				}

			internal:
				/// <summary>
				/// Audio media.
				/// </summary>
				/// <param name="pjAudioMedia">The pj audio media.</param>
				AudioMedia(pj::AudioMedia& pjAudioMedia);

				/// <summary>
				/// Get the pj audio media.
				/// </summary>
				/// <returns>The pj audio media.</returns>
				pj::AudioMedia& GetAudioMedia();

			private:
				bool _disposed;
				pj::AudioMedia& _pjAudioMedia;
				int _id;
			};
		}
	}
}
#endif