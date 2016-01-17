/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallMapper.cpp
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

#include "stdafx.h"

#include "CallMapper.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	Call connection mapper.
///	</summary>
CallMapper::CallMapper() :
	_disposed(false)
{
}

///	<summary>
///	Call connection mapper.
///	</summary>
CallMapper::~CallMapper()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Get call role.
/// </summary>
/// <param name="callRole">The current call role.</param>
/// <returns>The call role.</returns>
CallRole CallMapper::GetCallRoleEx(pjsip_role_e callRole)
{
	switch (callRole)
	{
	case PJSIP_ROLE_UAC:
		return CallRole::PJSIP_ROLE_UAC;;
	case PJSIP_ROLE_UAS:
		return CallRole::PJSIP_ROLE_UAS;;
	default:
		return CallRole::PJSIP_ROLE_UAC;
	}
}

/// <summary>
/// Get InviteSessionState.
/// </summary>
/// <param name="inviteSessionState">The current InviteSessionState.</param>
/// <returns>The InviteSessionState.</returns>
InviteSessionState CallMapper::GetInviteSessionStateEx(pjsip_inv_state inviteSessionState)
{
	switch (inviteSessionState)
	{
	case PJSIP_INV_STATE_NULL:
		return InviteSessionState::PJSIP_INV_STATE_NULL;
	case PJSIP_INV_STATE_CALLING:
		return InviteSessionState::PJSIP_INV_STATE_CALLING;
	case PJSIP_INV_STATE_INCOMING:
		return InviteSessionState::PJSIP_INV_STATE_INCOMING;
	case PJSIP_INV_STATE_EARLY:
		return InviteSessionState::PJSIP_INV_STATE_EARLY;
	case PJSIP_INV_STATE_CONNECTING:
		return InviteSessionState::PJSIP_INV_STATE_CONNECTING;
	case PJSIP_INV_STATE_CONFIRMED:
		return InviteSessionState::PJSIP_INV_STATE_CONFIRMED;
	case PJSIP_INV_STATE_DISCONNECTED:
		return InviteSessionState::PJSIP_INV_STATE_DISCONNECTED;
	default:
		return InviteSessionState::PJSIP_INV_STATE_NULL;
	}
}

/// <summary>
/// Get MediaDirection.
/// </summary>
/// <param name="mediaDirection">The current MediaDirection.</param>
/// <returns>The MediaDirection.</returns>
MediaDirection CallMapper::GetMediaDirectionEx(pjmedia_dir mediaDirection)
{
	switch (mediaDirection)
	{
	case PJMEDIA_DIR_NONE:
		return MediaDirection::PJMEDIA_DIR_NONE;
	case PJMEDIA_DIR_ENCODING:
		return MediaDirection::PJMEDIA_DIR_ENCODING;
	case PJMEDIA_DIR_DECODING:
		return MediaDirection::PJMEDIA_DIR_DECODING;
	case PJMEDIA_DIR_ENCODING_DECODING:
		return MediaDirection::PJMEDIA_DIR_ENCODING_DECODING;
	default:
		return MediaDirection::PJMEDIA_DIR_NONE;
	}
}

/// <summary>
/// Get MediaDirection.
/// </summary>
/// <param name="mediaDirection">The current MediaDirection.</param>
/// <returns>The MediaDirection.</returns>
pjmedia_dir CallMapper::GetMediaDirectionEx(MediaDirection mediaDirection)
{
	switch (mediaDirection)
	{
	case Nequeo::Net::PjSip::MediaDirection::PJMEDIA_DIR_NONE:
		return pjmedia_dir::PJMEDIA_DIR_NONE;
	case Nequeo::Net::PjSip::MediaDirection::PJMEDIA_DIR_ENCODING:
		return pjmedia_dir::PJMEDIA_DIR_ENCODING;
	case Nequeo::Net::PjSip::MediaDirection::PJMEDIA_DIR_DECODING:
		return pjmedia_dir::PJMEDIA_DIR_DECODING;
	case Nequeo::Net::PjSip::MediaDirection::PJMEDIA_DIR_ENCODING_DECODING:
		return pjmedia_dir::PJMEDIA_DIR_ENCODING_DECODING;
	default:
		return pjmedia_dir::PJMEDIA_DIR_NONE;
	}
}

/// <summary>
/// Get CallMediaStatus.
/// </summary>
/// <param name="callMediaStatus">The current CallMediaStatus.</param>
/// <returns>The CallMediaStatus.</returns>
CallMediaStatus CallMapper::GetCallMediaStatusEx(pjsua_call_media_status callMediaStatus)
{
	switch (callMediaStatus)
	{
	case PJSUA_CALL_MEDIA_NONE:
		return CallMediaStatus::PJSUA_CALL_MEDIA_NONE;
	case PJSUA_CALL_MEDIA_ACTIVE:
		return CallMediaStatus::PJSUA_CALL_MEDIA_ACTIVE;
	case PJSUA_CALL_MEDIA_LOCAL_HOLD:
		return CallMediaStatus::PJSUA_CALL_MEDIA_LOCAL_HOLD;
	case PJSUA_CALL_MEDIA_REMOTE_HOLD:
		return CallMediaStatus::PJSUA_CALL_MEDIA_REMOTE_HOLD;
	case PJSUA_CALL_MEDIA_ERROR:
		return CallMediaStatus::PJSUA_CALL_MEDIA_ERROR;
	default:
		return CallMediaStatus::PJSUA_CALL_MEDIA_NONE;
	}
}

///	<summary>
///	Get the dialog cap status.
///	</summary>
/// <param name="capStatus">The dialog cap status.</param>
/// <returns>The dialog cap status.</returns>
DialogCapStatus CallMapper::GetDialogCapStatusEx(pjsip_dialog_cap_status capStatus)
{
	switch (capStatus)
	{
	case PJSIP_DIALOG_CAP_UNSUPPORTED:
		return DialogCapStatus::PJSIP_DIALOG_CAP_UNSUPPORTED;
	case PJSIP_DIALOG_CAP_SUPPORTED:
		return DialogCapStatus::PJSIP_DIALOG_CAP_SUPPORTED;
	case PJSIP_DIALOG_CAP_UNKNOWN:
		return DialogCapStatus::PJSIP_DIALOG_CAP_UNKNOWN;
	default:
		return DialogCapStatus::PJSIP_DIALOG_CAP_UNKNOWN;
	}
}

///	<summary>
///	Get the stun nat type.
///	</summary>
/// <param name="natType">The stun nat type.</param>
/// <returns>The stun nat type.</returns>
StunNatType CallMapper::GetStunNatTypeEx(pj_stun_nat_type natType)
{
	switch (natType)
	{
	case PJ_STUN_NAT_TYPE_UNKNOWN:
		return StunNatType::PJ_STUN_NAT_TYPE_UNKNOWN;
	case PJ_STUN_NAT_TYPE_ERR_UNKNOWN:
		return StunNatType::PJ_STUN_NAT_TYPE_ERR_UNKNOWN;
	case PJ_STUN_NAT_TYPE_OPEN:
		return StunNatType::PJ_STUN_NAT_TYPE_OPEN;
	case PJ_STUN_NAT_TYPE_BLOCKED:
		return StunNatType::PJ_STUN_NAT_TYPE_BLOCKED;
	case PJ_STUN_NAT_TYPE_SYMMETRIC_UDP:
		return StunNatType::PJ_STUN_NAT_TYPE_SYMMETRIC_UDP;
	case PJ_STUN_NAT_TYPE_FULL_CONE:
		return StunNatType::PJ_STUN_NAT_TYPE_FULL_CONE;
	case PJ_STUN_NAT_TYPE_SYMMETRIC:
		return StunNatType::PJ_STUN_NAT_TYPE_SYMMETRIC;
	case PJ_STUN_NAT_TYPE_RESTRICTED:
		return StunNatType::PJ_STUN_NAT_TYPE_RESTRICTED;
	case PJ_STUN_NAT_TYPE_PORT_RESTRICTED:
		return StunNatType::PJ_STUN_NAT_TYPE_PORT_RESTRICTED;
	default:
		return StunNatType::PJ_STUN_NAT_TYPE_UNKNOWN;
	}
}

///	<summary>
///	Get the RedirectResponseType.
///	</summary>
/// <param name="redirectResponseType">The RedirectResponseType.</param>
/// <returns>The RedirectResponseType.</returns>
pjsip_redirect_op CallMapper::GetRedirectResponseTypeEx(RedirectResponseType redirectResponseType)
{
	switch (redirectResponseType)
	{
	case Nequeo::Net::PjSip::RedirectResponseType::PJSIP_REDIRECT_REJECT:
		return pjsip_redirect_op::PJSIP_REDIRECT_REJECT;
	case Nequeo::Net::PjSip::RedirectResponseType::PJSIP_REDIRECT_ACCEPT:
		return pjsip_redirect_op::PJSIP_REDIRECT_ACCEPT;
	case Nequeo::Net::PjSip::RedirectResponseType::PJSIP_REDIRECT_ACCEPT_REPLACE:
		return pjsip_redirect_op::PJSIP_REDIRECT_ACCEPT_REPLACE;
	case Nequeo::Net::PjSip::RedirectResponseType::PJSIP_REDIRECT_PENDING:
		return pjsip_redirect_op::PJSIP_REDIRECT_PENDING;
	case Nequeo::Net::PjSip::RedirectResponseType::PJSIP_REDIRECT_STOP:
		return pjsip_redirect_op::PJSIP_REDIRECT_STOP;
	default:
		return pjsip_redirect_op::PJSIP_REDIRECT_REJECT;
	}
}

///	<summary>
///	Get the VideoStreamOperation.
///	</summary>
/// <param name="videoStreamOperation">The VideoStreamOperation.</param>
/// <returns>The VideoStreamOperation.</returns>
pjsua_call_vid_strm_op CallMapper::GetVideoStreamOperationEx(VideoStreamOperation videoStreamOperation)
{
	switch (videoStreamOperation)
	{
	case Nequeo::Net::PjSip::VideoStreamOperation::PJSUA_CALL_VID_STRM_NO_OP:
		return pjsua_call_vid_strm_op::PJSUA_CALL_VID_STRM_NO_OP;
	case Nequeo::Net::PjSip::VideoStreamOperation::PJSUA_CALL_VID_STRM_ADD:
		return pjsua_call_vid_strm_op::PJSUA_CALL_VID_STRM_ADD;
	case Nequeo::Net::PjSip::VideoStreamOperation::PJSUA_CALL_VID_STRM_REMOVE:
		return pjsua_call_vid_strm_op::PJSUA_CALL_VID_STRM_REMOVE;
	case Nequeo::Net::PjSip::VideoStreamOperation::PJSUA_CALL_VID_STRM_CHANGE_DIR:
		return pjsua_call_vid_strm_op::PJSUA_CALL_VID_STRM_CHANGE_DIR;
	case Nequeo::Net::PjSip::VideoStreamOperation::PJSUA_CALL_VID_STRM_CHANGE_CAP_DEV:
		return pjsua_call_vid_strm_op::PJSUA_CALL_VID_STRM_CHANGE_CAP_DEV;
	case Nequeo::Net::PjSip::VideoStreamOperation::PJSUA_CALL_VID_STRM_START_TRANSMIT:
		return pjsua_call_vid_strm_op::PJSUA_CALL_VID_STRM_START_TRANSMIT;
	case Nequeo::Net::PjSip::VideoStreamOperation::PJSUA_CALL_VID_STRM_STOP_TRANSMIT:
		return pjsua_call_vid_strm_op::PJSUA_CALL_VID_STRM_STOP_TRANSMIT;
	case Nequeo::Net::PjSip::VideoStreamOperation::PJSUA_CALL_VID_STRM_SEND_KEYFRAME:
		return pjsua_call_vid_strm_op::PJSUA_CALL_VID_STRM_SEND_KEYFRAME;
	default:
		return pjsua_call_vid_strm_op::PJSUA_CALL_VID_STRM_NO_OP;
	}
}

///	<summary>
///	Get the MediaTransportProtocol.
///	</summary>
/// <param name="mediaTransportProtocol">The MediaTransportProtocol.</param>
/// <returns>The MediaTransportProtocol.</returns>
MediaTransportProtocol CallMapper::GetMediaTransportProtocolEx(pjmedia_tp_proto mediaTransportProtocol)
{
	switch (mediaTransportProtocol)
	{
	case PJMEDIA_TP_PROTO_NONE:
		return MediaTransportProtocol::PJMEDIA_TP_PROTO_NONE;
	case PJMEDIA_TP_PROTO_RTP_AVP:
		return MediaTransportProtocol::PJMEDIA_TP_PROTO_RTP_AVP;
	case PJMEDIA_TP_PROTO_RTP_SAVP:
		return MediaTransportProtocol::PJMEDIA_TP_PROTO_RTP_SAVP;
	case PJMEDIA_TP_PROTO_UNKNOWN:
		return MediaTransportProtocol::PJMEDIA_TP_PROTO_UNKNOWN;
	default:
		return MediaTransportProtocol::PJMEDIA_TP_PROTO_NONE;
	}
}

///	<summary>
///	Get the SipEventType.
///	</summary>
/// <param name="sipEventType">The SipEventType.</param>
/// <returns>The SipEventType.</returns>
SipEventType CallMapper::GetSipEventTypeEx(pjsip_event_id_e sipEventType)
{
	switch (sipEventType)
	{
	case PJSIP_EVENT_UNKNOWN:
		return SipEventType::PJSIP_EVENT_UNKNOWN;
	case PJSIP_EVENT_TIMER:
		return SipEventType::PJSIP_EVENT_TIMER;
	case PJSIP_EVENT_TX_MSG:
		return SipEventType::PJSIP_EVENT_TX_MSG;
	case PJSIP_EVENT_RX_MSG:
		return SipEventType::PJSIP_EVENT_RX_MSG;
	case PJSIP_EVENT_TRANSPORT_ERROR:
		return SipEventType::PJSIP_EVENT_TRANSPORT_ERROR;
	case PJSIP_EVENT_TSX_STATE:
		return SipEventType::PJSIP_EVENT_TSX_STATE;
	case PJSIP_EVENT_USER:
		return SipEventType::PJSIP_EVENT_USER;
	default:
		return SipEventType::PJSIP_EVENT_UNKNOWN;
	}
}