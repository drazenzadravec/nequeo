/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          VideoStreamOperation.h
*  Purpose :       SIP VideoStreamOperation class.
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

#ifndef _VIDEOSTREAMOPERATION_H
#define _VIDEOSTREAMOPERATION_H

#include "stdafx.h"

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
			/// This enumeration represents video stream operation on a call.
			/// </summary>
			public enum class VideoStreamOperation : unsigned
			{
				/// <summary>
				/// No operation
				/// </summary>
				PJSUA_CALL_VID_STRM_NO_OP,
				/// <summary>
				/// Add a new video stream. This will add a new m=video line to
				/// the media, regardless of whether existing video is/are present
				/// or not.  This will cause re-INVITE or UPDATE to be sent to remote
				/// party.
				/// </summary>
				PJSUA_CALL_VID_STRM_ADD,
				/// <summary>
				/// Remove/disable an existing video stream. This will
				/// cause re-INVITE or UPDATE to be sent to remote party.
				/// </summary>
				PJSUA_CALL_VID_STRM_REMOVE,
				/// <summary>
				/// Change direction of a video stream. This operation can be used
				/// to activate or deactivate an existing video media. This will
				/// cause re-INVITE or UPDATE to be sent to remote party.
				/// </summary>
				PJSUA_CALL_VID_STRM_CHANGE_DIR,
				/// <summary>
				/// Change capture device of a video stream.  This will not send
				/// re-INVITE or UPDATE to remote party.
				/// </summary>
				PJSUA_CALL_VID_STRM_CHANGE_CAP_DEV,
				/// <summary>
				/// Start transmitting video stream. This will cause previously
				/// stopped stream to start transmitting again. Note that no
				/// re-INVITE/UPDATE is to be transmitted to remote since this
				/// operation only operates on local stream.
				/// </summary>
				PJSUA_CALL_VID_STRM_START_TRANSMIT,
				/// <summary>
				/// Stop transmitting video stream. This will cause the stream to
				/// be paused in TX direction, causing it to stop sending any video
				/// packets. No re-INVITE/UPDATE is to be transmitted to remote
				/// with this operation.
				/// </summary>
				PJSUA_CALL_VID_STRM_STOP_TRANSMIT,
				/// <summary>
				/// Send keyframe in the video stream. This will force the stream to
				/// generate and send video keyframe as soon as possible. No
				/// re-INVITE/UPDATE is to be transmitted to remote with this operation.
				/// </summary>
				PJSUA_CALL_VID_STRM_SEND_KEYFRAME
			};
		}
	}
}
#endif