/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallMapper.h
*  Purpose :       SIP Call Mapper class.
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

#ifndef _CALLMAPPER_H
#define _CALLMAPPER_H

#include "stdafx.h"

#include "CallRole.h"
#include "InviteSessionState.h"
#include "CallFlag.h"
#include "MediaDirection.h"
#include "CallMediaStatus.h"
#include "VidReqKeyframeMethod.h"
#include "DialogCapStatus.h"
#include "StunNatType.h"
#include "RedirectResponseType.h"
#include "VideoStreamOperation.h"
#include "MediaTransportProtocol.h"
#include "SipEventType.h"
#include "MediaEventType.h"
#include "MediaTransportState.h"

#include "pjsua2.hpp"

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			///	<summary>
			///	Call connection mapper.
			///	</summary>
			class CallMapper
			{
			public:
				///	<summary>
				///	Call connection mapper.
				///	</summary>
				CallMapper();

				///	<summary>
				///	Call connection mapper.
				///	</summary>
				~CallMapper();

				/// <summary>
				/// Get call role.
				/// </summary>
				/// <param name="callRole">The current call role.</param>
				/// <returns>The call role.</returns>
				static CallRole GetCallRoleEx(pjsip_role_e callRole);

				/// <summary>
				/// Get InviteSessionState.
				/// </summary>
				/// <param name="inviteSessionState">The current InviteSessionState.</param>
				/// <returns>The InviteSessionState.</returns>
				static InviteSessionState GetInviteSessionStateEx(pjsip_inv_state inviteSessionState);

				/// <summary>
				/// Get MediaDirection.
				/// </summary>
				/// <param name="mediaDirection">The current MediaDirection.</param>
				/// <returns>The MediaDirection.</returns>
				static MediaDirection GetMediaDirectionEx(pjmedia_dir mediaDirection);

				/// <summary>
				/// Get MediaDirection.
				/// </summary>
				/// <param name="mediaDirection">The current MediaDirection.</param>
				/// <returns>The MediaDirection.</returns>
				static pjmedia_dir GetMediaDirectionEx(MediaDirection mediaDirection);

				/// <summary>
				/// Get CallMediaStatus.
				/// </summary>
				/// <param name="callMediaStatus">The current CallMediaStatus.</param>
				/// <returns>The CallMediaStatus.</returns>
				static CallMediaStatus GetCallMediaStatusEx(pjsua_call_media_status callMediaStatus);

				///	<summary>
				///	Get the dialog cap status.
				///	</summary>
				/// <param name="capStatus">The dialog cap status.</param>
				/// <returns>The dialog cap status.</returns>
				static DialogCapStatus GetDialogCapStatusEx(pjsip_dialog_cap_status capStatus);

				///	<summary>
				///	Get the stun nat type.
				///	</summary>
				/// <param name="natType">The stun nat type.</param>
				/// <returns>The stun nat type.</returns>
				static StunNatType GetStunNatTypeEx(pj_stun_nat_type natType);

				///	<summary>
				///	Get the RedirectResponseType.
				///	</summary>
				/// <param name="redirectResponseType">The RedirectResponseType.</param>
				/// <returns>The RedirectResponseType.</returns>
				static pjsip_redirect_op GetRedirectResponseTypeEx(RedirectResponseType redirectResponseType);

				///	<summary>
				///	Get the VideoStreamOperation.
				///	</summary>
				/// <param name="videoStreamOperation">The VideoStreamOperation.</param>
				/// <returns>The VideoStreamOperation.</returns>
				static pjsua_call_vid_strm_op GetVideoStreamOperationEx(VideoStreamOperation videoStreamOperation);

				///	<summary>
				///	Get the MediaTransportProtocol.
				///	</summary>
				/// <param name="mediaTransportProtocol">The MediaTransportProtocol.</param>
				/// <returns>The MediaTransportProtocol.</returns>
				static MediaTransportProtocol GetMediaTransportProtocolEx(pjmedia_tp_proto mediaTransportProtocol);

				///	<summary>
				///	Get the SipEventType.
				///	</summary>
				/// <param name="sipEventType">The SipEventType.</param>
				/// <returns>The SipEventType.</returns>
				static SipEventType GetSipEventTypeEx(pjsip_event_id_e sipEventType);

				///	<summary>
				///	Get the MediaEventType.
				///	</summary>
				/// <param name="mediaEventType">The MediaEventType.</param>
				/// <returns>The MediaEventType.</returns>
				static MediaEventType GetMediaEventTypeEx(pjmedia_event_type mediaEventType);

				///	<summary>
				///	Get the MediaTransportState.
				///	</summary>
				/// <param name="mediaTransportState">The MediaTransportState.</param>
				/// <returns>The MediaTransportState.</returns>
				static MediaTransportState GetMediaTransportStateEx(pjsua_med_tp_st mediaTransportState);

			private:
				bool _disposed;

			};
		}
	}
}
#endif