/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Sip
{
    /// <summary>
    /// Call information. Application can query the call information by calling Call::getInfo().
    /// </summary>
    public class CallInfo
    {
        /// <summary>
        /// Call information. Application can query the call information by calling Call::getInfo().
        /// </summary>
        public CallInfo() { }

        ///	<summary>
        ///	Gets or sets the call id.
        ///	</summary>
        public int Id { get; set; }

        ///	<summary>
        ///	Gets or sets the account id.
        ///	</summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the call role (UAC == caller).
        /// </summary>
        public CallRole Role { get; set; }

        /// <summary>
        /// Gets or sets the local uri.
        /// </summary>
        public string LocalUri { get; set; }

        /// <summary>
        /// Gets or sets the local contact.
        /// </summary>
        public string LocalContact { get; set; }

        /// <summary>
        /// Gets or sets the remote uri.
        /// </summary>
        public string RemoteUri { get; set; }

        /// <summary>
        /// Gets or sets the remote contact.
        /// </summary>
        public string RemoteContact { get; set; }

        /// <summary>
        /// Gets or sets the dialog Call-ID string.
        /// </summary>
        public string CallIdString { get; set; }

        /// <summary>
        /// Gets or sets the call setting.
        /// </summary>
        public CallSetting Setting { get; set; }

        ///	<summary>
        ///	Gets or sets the call state.
        ///	</summary>
        public InviteSessionState State { get; set; }

        /// <summary>
        /// Gets or sets the text describing the state.
        /// </summary>
        public string StateText { get; set; }

        ///	<summary>
        ///	Gets or sets the last status code heard, which can be used as cause code.
        ///	</summary>
        public StatusCode LastStatusCode { get; set; }

        /// <summary>
        /// Gets or sets the reason phrase describing the last status.
        /// </summary>
        public string LastReason { get; set; }

        /// <summary>
        /// Gets or sets the array of active media information.
        /// </summary>
        public CallMediaInfo[] Media { get; set; }

        ///	<summary>
        ///	Gets or sets the Array of provisional media information. This contains the media info
        /// in the provisioning state, that is when the media session is being
        /// created / updated(SDP offer / answer is on progress).
        ///	</summary>
        public CallMediaInfo[] ProvMedia { get; set; }

        /// <summary>
        /// Gets or sets the Up-to-date call connected duration (zero when call is not established).
        /// </summary>
        public TimeVal ConnectDuration { get; set; }

        /// <summary>
        /// Gets or sets the total call duration, including set-up time.
        /// </summary>
        public TimeVal TotalDuration { get; set; }

        /// <summary>
        /// Gets or sets the flag if remote was SDP offerer.
        /// </summary>
        public bool RemOfferer { get; set; }

        /// <summary>
        /// Gets or sets the number of audio streams offered by remote.
        /// </summary>
        public uint RemAudioCount { get; set; }

        /// <summary>
        /// Gets or sets the number of video streams offered by remote.
        /// </summary>
        public uint RemVideoCount { get; set; }

        /// <summary>
        /// Get CallRole.
        /// </summary>
        /// <param name="callRole">The current CallRole.</param>
        /// <returns>The CallRole.</returns>
        internal static CallRole GetCallRoleEx(pjsua2.pjsip_role_e callRole)
        {
            // Select the srtp signaling.
            switch (callRole)
            {
                case pjsua2.pjsip_role_e.PJSIP_ROLE_UAC:
                    return CallRole.PJSIP_ROLE_UAC;
                case pjsua2.pjsip_role_e.PJSIP_ROLE_UAS:
                    return CallRole.PJSIP_ROLE_UAS;
                default:
                    return CallRole.PJSIP_ROLE_UAC;
            }
        }

        /// <summary>
        /// Get InviteSessionState.
        /// </summary>
        /// <param name="inviteSessionState">The current InviteSessionState.</param>
        /// <returns>The InviteSessionState.</returns>
        internal static InviteSessionState GetInviteSessionStateEx(pjsua2.pjsip_inv_state inviteSessionState)
        {
            // Select the srtp signaling.
            switch (inviteSessionState)
            {
                case pjsua2.pjsip_inv_state.PJSIP_INV_STATE_CALLING:
                    return InviteSessionState.PJSIP_INV_STATE_CALLING;
                case pjsua2.pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED:
                    return InviteSessionState.PJSIP_INV_STATE_CONFIRMED;
                case pjsua2.pjsip_inv_state.PJSIP_INV_STATE_CONNECTING:
                    return InviteSessionState.PJSIP_INV_STATE_CONNECTING;
                case pjsua2.pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED:
                    return InviteSessionState.PJSIP_INV_STATE_DISCONNECTED;
                case pjsua2.pjsip_inv_state.PJSIP_INV_STATE_EARLY:
                    return InviteSessionState.PJSIP_INV_STATE_EARLY;
                case pjsua2.pjsip_inv_state.PJSIP_INV_STATE_INCOMING:
                    return InviteSessionState.PJSIP_INV_STATE_INCOMING;
                case pjsua2.pjsip_inv_state.PJSIP_INV_STATE_NULL:
                    return InviteSessionState.PJSIP_INV_STATE_NULL;
                default:
                    return InviteSessionState.PJSIP_INV_STATE_NULL;
            }
        }

        /// <summary>
        /// Get MediaDirection.
        /// </summary>
        /// <param name="mediaDirection">The current MediaDirection.</param>
        /// <returns>The MediaDirection.</returns>
        internal static MediaDirection GetMediaDirectionEx(pjsua2.pjmedia_dir mediaDirection)
        {
            // Select the srtp signaling.
            switch (mediaDirection)
            {
                case pjsua2.pjmedia_dir.PJMEDIA_DIR_DECODING:
                    return MediaDirection.PJMEDIA_DIR_DECODING;
                case pjsua2.pjmedia_dir.PJMEDIA_DIR_ENCODING:
                    return MediaDirection.PJMEDIA_DIR_ENCODING;
                case pjsua2.pjmedia_dir.PJMEDIA_DIR_ENCODING_DECODING:
                    return MediaDirection.PJMEDIA_DIR_ENCODING_DECODING;
                case pjsua2.pjmedia_dir.PJMEDIA_DIR_NONE:
                    return MediaDirection.PJMEDIA_DIR_NONE;
                default:
                    return MediaDirection.PJMEDIA_DIR_NONE;
            }
        }

        /// <summary>
        /// Get MediaDirection.
        /// </summary>
        /// <param name="mediaDirection">The current MediaDirection.</param>
        /// <returns>The MediaDirection.</returns>
        internal static pjsua2.pjmedia_dir GetMediaDirection(MediaDirection mediaDirection)
        {
            // Select the srtp signaling.
            switch (mediaDirection)
            {
                case MediaDirection.PJMEDIA_DIR_DECODING:
                    return pjsua2.pjmedia_dir.PJMEDIA_DIR_DECODING;
                case MediaDirection.PJMEDIA_DIR_ENCODING:
                    return pjsua2.pjmedia_dir.PJMEDIA_DIR_ENCODING;
                case MediaDirection.PJMEDIA_DIR_ENCODING_DECODING:
                    return pjsua2.pjmedia_dir.PJMEDIA_DIR_ENCODING_DECODING;
                case MediaDirection.PJMEDIA_DIR_NONE:
                    return pjsua2.pjmedia_dir.PJMEDIA_DIR_NONE;
                default:
                    return pjsua2.pjmedia_dir.PJMEDIA_DIR_NONE;
            }
        }

        /// <summary>
        /// Get CallMediaStatus.
        /// </summary>
        /// <param name="callMediaStatus">The current CallMediaStatus.</param>
        /// <returns>The CallMediaStatus.</returns>
        internal static CallMediaStatus GetCallMediaStatusEx(pjsua2.pjsua_call_media_status callMediaStatus)
        {
            // Select the srtp signaling.
            switch (callMediaStatus)
            {
                case pjsua2.pjsua_call_media_status.PJSUA_CALL_MEDIA_ACTIVE:
                    return CallMediaStatus.PJSUA_CALL_MEDIA_ACTIVE;
                case pjsua2.pjsua_call_media_status.PJSUA_CALL_MEDIA_ERROR:
                    return CallMediaStatus.PJSUA_CALL_MEDIA_ERROR;
                case pjsua2.pjsua_call_media_status.PJSUA_CALL_MEDIA_LOCAL_HOLD:
                    return CallMediaStatus.PJSUA_CALL_MEDIA_LOCAL_HOLD;
                case pjsua2.pjsua_call_media_status.PJSUA_CALL_MEDIA_NONE:
                    return CallMediaStatus.PJSUA_CALL_MEDIA_NONE;
                case pjsua2.pjsua_call_media_status.PJSUA_CALL_MEDIA_REMOTE_HOLD:
                    return CallMediaStatus.PJSUA_CALL_MEDIA_REMOTE_HOLD;
                default:
                    return CallMediaStatus.PJSUA_CALL_MEDIA_NONE;
            }
        }

        /// <summary>
        /// Get DialogCapStatus.
        /// </summary>
        /// <param name="dialogCapStatus">The current DialogCapStatus.</param>
        /// <returns>The DialogCapStatus.</returns>
        internal static DialogCapStatus GetDialogCapStatusEx(pjsua2.pjsip_dialog_cap_status dialogCapStatus)
        {
            // Select the srtp signaling.
            switch (dialogCapStatus)
            {
                case pjsua2.pjsip_dialog_cap_status.PJSIP_DIALOG_CAP_SUPPORTED:
                    return DialogCapStatus.PJSIP_DIALOG_CAP_SUPPORTED;
                case pjsua2.pjsip_dialog_cap_status.PJSIP_DIALOG_CAP_UNKNOWN:
                    return DialogCapStatus.PJSIP_DIALOG_CAP_UNKNOWN;
                case pjsua2.pjsip_dialog_cap_status.PJSIP_DIALOG_CAP_UNSUPPORTED:
                    return DialogCapStatus.PJSIP_DIALOG_CAP_UNSUPPORTED;
                default:
                    return DialogCapStatus.PJSIP_DIALOG_CAP_UNKNOWN;
            }
        }

        /// <summary>
        /// Get StunNatType.
        /// </summary>
        /// <param name="stunNatType">The current StunNatType.</param>
        /// <returns>The StunNatType.</returns>
        internal static StunNatType GetStunNatTypeEx(pjsua2.pj_stun_nat_type stunNatType)
        {
            // Select the srtp signaling.
            switch (stunNatType)
            {
                case pjsua2.pj_stun_nat_type.PJ_STUN_NAT_TYPE_BLOCKED:
                    return StunNatType.PJ_STUN_NAT_TYPE_BLOCKED;
                case pjsua2.pj_stun_nat_type.PJ_STUN_NAT_TYPE_ERR_UNKNOWN:
                    return StunNatType.PJ_STUN_NAT_TYPE_ERR_UNKNOWN;
                case pjsua2.pj_stun_nat_type.PJ_STUN_NAT_TYPE_FULL_CONE:
                    return StunNatType.PJ_STUN_NAT_TYPE_FULL_CONE;
                case pjsua2.pj_stun_nat_type.PJ_STUN_NAT_TYPE_OPEN:
                    return StunNatType.PJ_STUN_NAT_TYPE_OPEN;
                case pjsua2.pj_stun_nat_type.PJ_STUN_NAT_TYPE_PORT_RESTRICTED:
                    return StunNatType.PJ_STUN_NAT_TYPE_PORT_RESTRICTED;
                case pjsua2.pj_stun_nat_type.PJ_STUN_NAT_TYPE_RESTRICTED:
                    return StunNatType.PJ_STUN_NAT_TYPE_RESTRICTED;
                case pjsua2.pj_stun_nat_type.PJ_STUN_NAT_TYPE_SYMMETRIC:
                    return StunNatType.PJ_STUN_NAT_TYPE_SYMMETRIC;
                case pjsua2.pj_stun_nat_type.PJ_STUN_NAT_TYPE_SYMMETRIC_UDP:
                    return StunNatType.PJ_STUN_NAT_TYPE_SYMMETRIC_UDP;
                case pjsua2.pj_stun_nat_type.PJ_STUN_NAT_TYPE_UNKNOWN:
                    return StunNatType.PJ_STUN_NAT_TYPE_UNKNOWN;
                default:
                    return StunNatType.PJ_STUN_NAT_TYPE_UNKNOWN;
            }
        }

        /// <summary>
        /// Get RedirectResponseType.
        /// </summary>
        /// <param name="redirectResponseType">The current RedirectResponseType.</param>
        /// <returns>The RedirectResponseType.</returns>
        internal static pjsua2.pjsip_redirect_op GetRedirectResponseType(RedirectResponseType redirectResponseType)
        {
            // Select the srtp signaling.
            switch (redirectResponseType)
            {
                case RedirectResponseType.PJSIP_REDIRECT_ACCEPT:
                    return pjsua2.pjsip_redirect_op.PJSIP_REDIRECT_ACCEPT;
                case RedirectResponseType.PJSIP_REDIRECT_ACCEPT_REPLACE:
                    return pjsua2.pjsip_redirect_op.PJSIP_REDIRECT_ACCEPT_REPLACE;
                case RedirectResponseType.PJSIP_REDIRECT_PENDING:
                    return pjsua2.pjsip_redirect_op.PJSIP_REDIRECT_PENDING;
                case RedirectResponseType.PJSIP_REDIRECT_REJECT:
                    return pjsua2.pjsip_redirect_op.PJSIP_REDIRECT_REJECT;
                case RedirectResponseType.PJSIP_REDIRECT_STOP:
                    return pjsua2.pjsip_redirect_op.PJSIP_REDIRECT_STOP;
                default:
                    return pjsua2.pjsip_redirect_op.PJSIP_REDIRECT_STOP;
            }
        }

        /// <summary>
        /// Get VideoStreamOperation.
        /// </summary>
        /// <param name="videoStreamOperation">The current VideoStreamOperation.</param>
        /// <returns>The VideoStreamOperation.</returns>
        internal static pjsua2.pjsua_call_vid_strm_op GetVideoStreamOperation(VideoStreamOperation videoStreamOperation)
        {
            // Select the srtp signaling.
            switch (videoStreamOperation)
            {
                case VideoStreamOperation.PJSUA_CALL_VID_STRM_ADD:
                    return pjsua2.pjsua_call_vid_strm_op.PJSUA_CALL_VID_STRM_ADD;
                case VideoStreamOperation.PJSUA_CALL_VID_STRM_CHANGE_CAP_DEV:
                    return pjsua2.pjsua_call_vid_strm_op.PJSUA_CALL_VID_STRM_CHANGE_CAP_DEV;
                case VideoStreamOperation.PJSUA_CALL_VID_STRM_CHANGE_DIR:
                    return pjsua2.pjsua_call_vid_strm_op.PJSUA_CALL_VID_STRM_CHANGE_DIR;
                case VideoStreamOperation.PJSUA_CALL_VID_STRM_NO_OP:
                    return pjsua2.pjsua_call_vid_strm_op.PJSUA_CALL_VID_STRM_NO_OP;
                case VideoStreamOperation.PJSUA_CALL_VID_STRM_REMOVE:
                    return pjsua2.pjsua_call_vid_strm_op.PJSUA_CALL_VID_STRM_REMOVE;
                case VideoStreamOperation.PJSUA_CALL_VID_STRM_SEND_KEYFRAME:
                    return pjsua2.pjsua_call_vid_strm_op.PJSUA_CALL_VID_STRM_SEND_KEYFRAME;
                case VideoStreamOperation.PJSUA_CALL_VID_STRM_START_TRANSMIT:
                    return pjsua2.pjsua_call_vid_strm_op.PJSUA_CALL_VID_STRM_START_TRANSMIT;
                case VideoStreamOperation.PJSUA_CALL_VID_STRM_STOP_TRANSMIT:
                    return pjsua2.pjsua_call_vid_strm_op.PJSUA_CALL_VID_STRM_STOP_TRANSMIT;
                default:
                    return pjsua2.pjsua_call_vid_strm_op.PJSUA_CALL_VID_STRM_NO_OP;
            }
        }

        /// <summary>
        /// Get MediaTransportProtocol.
        /// </summary>
        /// <param name="mediaTransportProtocol">The current MediaTransportProtocol.</param>
        /// <returns>The MediaTransportProtocol.</returns>
        internal static MediaTransportProtocol GetMediaTransportProtocolEx(pjsua2.pjmedia_tp_proto mediaTransportProtocol)
        {
            // Select the srtp signaling.
            switch (mediaTransportProtocol)
            {
                case pjsua2.pjmedia_tp_proto.PJMEDIA_TP_PROTO_NONE:
                    return MediaTransportProtocol.PJMEDIA_TP_PROTO_NONE;
                case pjsua2.pjmedia_tp_proto.PJMEDIA_TP_PROTO_RTP_AVP:
                    return MediaTransportProtocol.PJMEDIA_TP_PROTO_RTP_AVP;
                case pjsua2.pjmedia_tp_proto.PJMEDIA_TP_PROTO_RTP_SAVP:
                    return MediaTransportProtocol.PJMEDIA_TP_PROTO_RTP_SAVP;
                case pjsua2.pjmedia_tp_proto.PJMEDIA_TP_PROTO_UNKNOWN:
                    return MediaTransportProtocol.PJMEDIA_TP_PROTO_UNKNOWN;
                default:
                    return MediaTransportProtocol.PJMEDIA_TP_PROTO_NONE;
            }
        }

        /// <summary>
        /// Get SipEventType.
        /// </summary>
        /// <param name="sipEventType">The current SipEventType.</param>
        /// <returns>The SipEventType.</returns>
        internal static SipEventType GetSipEventTypeEx(pjsua2.pjsip_event_id_e sipEventType)
        {
            // Select the srtp signaling.
            switch (sipEventType)
            {
                case pjsua2.pjsip_event_id_e.PJSIP_EVENT_RX_MSG:
                    return SipEventType.PJSIP_EVENT_RX_MSG;
                case pjsua2.pjsip_event_id_e.PJSIP_EVENT_TIMER:
                    return SipEventType.PJSIP_EVENT_TIMER;
                case pjsua2.pjsip_event_id_e.PJSIP_EVENT_TRANSPORT_ERROR:
                    return SipEventType.PJSIP_EVENT_TRANSPORT_ERROR;
                case pjsua2.pjsip_event_id_e.PJSIP_EVENT_TSX_STATE:
                    return SipEventType.PJSIP_EVENT_TSX_STATE;
                case pjsua2.pjsip_event_id_e.PJSIP_EVENT_TX_MSG:
                    return SipEventType.PJSIP_EVENT_TX_MSG;
                case pjsua2.pjsip_event_id_e.PJSIP_EVENT_UNKNOWN:
                    return SipEventType.PJSIP_EVENT_UNKNOWN;
                case pjsua2.pjsip_event_id_e.PJSIP_EVENT_USER:
                    return SipEventType.PJSIP_EVENT_USER;
                default:
                    return SipEventType.PJSIP_EVENT_UNKNOWN;
            }
        }

        /// <summary>
        /// Get MediaTransportState.
        /// </summary>
        /// <param name="mediaTransportState">The current MediaTransportState.</param>
        /// <returns>The MediaTransportState.</returns>
        internal static MediaTransportState GetMediaTransportStateEx(pjsua2.pjsua_med_tp_st mediaTransportState)
        {
            // Select the srtp signaling.
            switch (mediaTransportState)
            {
                case pjsua2.pjsua_med_tp_st.PJSUA_MED_TP_CREATING:
                    return MediaTransportState.PJSUA_MED_TP_CREATING;
                case pjsua2.pjsua_med_tp_st.PJSUA_MED_TP_DISABLED:
                    return MediaTransportState.PJSUA_MED_TP_DISABLED;
                case pjsua2.pjsua_med_tp_st.PJSUA_MED_TP_IDLE:
                    return MediaTransportState.PJSUA_MED_TP_IDLE;
                case pjsua2.pjsua_med_tp_st.PJSUA_MED_TP_INIT:
                    return MediaTransportState.PJSUA_MED_TP_INIT;
                case pjsua2.pjsua_med_tp_st.PJSUA_MED_TP_NULL:
                    return MediaTransportState.PJSUA_MED_TP_NULL;
                case pjsua2.pjsua_med_tp_st.PJSUA_MED_TP_RUNNING:
                    return MediaTransportState.PJSUA_MED_TP_RUNNING;
                default:
                    return MediaTransportState.PJSUA_MED_TP_NULL;
            }
        }

        /// <summary>
        /// Get MediaEventType.
        /// </summary>
        /// <param name="mediaEventType">The current MediaEventType.</param>
        /// <returns>The MediaEventType.</returns>
        internal static MediaEventType GetMediaEventTypeEx(pjsua2.pjmedia_event_type mediaEventType)
        {
            // Select the srtp signaling.
            switch (mediaEventType)
            {
                case pjsua2.pjmedia_event_type.PJMEDIA_EVENT_FMT_CHANGED:
                    return MediaEventType.PJMEDIA_EVENT_FMT_CHANGED;
                case pjsua2.pjmedia_event_type.PJMEDIA_EVENT_KEYFRAME_FOUND:
                    return MediaEventType.PJMEDIA_EVENT_KEYFRAME_FOUND;
                case pjsua2.pjmedia_event_type.PJMEDIA_EVENT_KEYFRAME_MISSING:
                    return MediaEventType.PJMEDIA_EVENT_KEYFRAME_MISSING;
                case pjsua2.pjmedia_event_type.PJMEDIA_EVENT_MOUSE_BTN_DOWN:
                    return MediaEventType.PJMEDIA_EVENT_MOUSE_BTN_DOWN;
                case pjsua2.pjmedia_event_type.PJMEDIA_EVENT_NONE:
                    return MediaEventType.PJMEDIA_EVENT_NONE;
                case pjsua2.pjmedia_event_type.PJMEDIA_EVENT_ORIENT_CHANGED:
                    return MediaEventType.PJMEDIA_EVENT_ORIENT_CHANGED;
                case pjsua2.pjmedia_event_type.PJMEDIA_EVENT_WND_CLOSED:
                    return MediaEventType.PJMEDIA_EVENT_WND_CLOSED;
                case pjsua2.pjmedia_event_type.PJMEDIA_EVENT_WND_CLOSING:
                    return MediaEventType.PJMEDIA_EVENT_WND_CLOSING;
                case pjsua2.pjmedia_event_type.PJMEDIA_EVENT_WND_RESIZED:
                    return MediaEventType.PJMEDIA_EVENT_WND_RESIZED;
                default:
                    return MediaEventType.PJMEDIA_EVENT_NONE;
            }
        }
    }
}
