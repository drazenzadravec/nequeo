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
    /// Sip call.
    /// </summary>
    public class Call : IDisposable
    {
        /// <summary>
        /// Sip call.
        /// </summary>
        /// <param name="account">The Sip account.</param>
        /// <param name="callId">An index call id (0 - 3).</param>
        public Call(Account account, int callId)
        {
            _callId = callId;
            _account = account;
            _pjCall = new CallCallback(account.PjAccount, callId);
        }

        private int _callId = 0;
        private Account _account = null;
        private CallCallback _pjCall = null;

        /// <summary>
        /// Gets the internal pj call.
        /// </summary>
        internal CallCallback PjCall
        {
            get { return _pjCall; }
        }

        ///	<summary>
        ///	Get detail information about this call.
        ///	</summary>
        /// <returns>Call information.</returns>
        public CallInfo GetInfo()
        {
            CallInfo callInfo = new CallInfo();

            try
            {
                pjsua2.CallInfo info = _pjCall.getInfo();

                callInfo.AccountId = info.accId;
                callInfo.CallIdString = info.callIdString;
                callInfo.Id = info.id;
                callInfo.LastReason = info.lastReason;
                callInfo.LocalContact = info.localContact;
                callInfo.LocalUri = info.localUri;
                callInfo.RemAudioCount = info.remAudioCount;
                callInfo.RemOfferer = info.remOfferer;
                callInfo.RemoteContact = info.remoteContact;
                callInfo.RemoteUri = info.remoteUri;
                callInfo.RemVideoCount = info.remVideoCount;
                callInfo.StateText = info.stateText;
                callInfo.LastStatusCode = AccountInfo.GetStatusCodeEx(info.lastStatusCode);
                callInfo.Role = CallInfo.GetCallRoleEx(info.role);
                callInfo.State = CallInfo.GetInviteSessionStateEx(info.state);

                callInfo.ConnectDuration = new TimeVal();
                callInfo.TotalDuration = new TimeVal();
                callInfo.ConnectDuration.Milliseconds = info.connectDuration.msec;
                callInfo.ConnectDuration.Seconds = info.connectDuration.sec;
                callInfo.TotalDuration.Milliseconds = info.totalDuration.msec;
                callInfo.TotalDuration.Seconds = info.totalDuration.sec;

                callInfo.Setting = new CallSetting();
                callInfo.Setting.AudioCount = info.setting.audioCount;
                callInfo.Setting.Flag = (CallFlag)info.setting.flag;
                callInfo.Setting.VideoCount = info.setting.videoCount;
                callInfo.Setting.ReqKeyframeMethod = (VidReqKeyframeMethod)info.setting.reqKeyframeMethod;

                if (info.media != null)
                {
                    callInfo.Media = new CallMediaInfo[info.media.Count];
                    for (int i = 0; i < info.media.Count; i++)
                    {
                        CallMediaInfo mediaInfo = new CallMediaInfo();
                        mediaInfo.AudioConfSlot = info.media[i].audioConfSlot;
                        mediaInfo.Direction = CallInfo.GetMediaDirectionEx(info.media[i].dir);
                        mediaInfo.Index = info.media[i].index;
                        mediaInfo.VideoCapDev = info.media[i].videoCapDev;
                        mediaInfo.VideoIncomingWindowId = info.media[i].videoIncomingWindowId;
                        mediaInfo.Status = CallInfo.GetCallMediaStatusEx(info.media[i].status);
                        mediaInfo.Type = MediaFormat.GetMediaTypeEx(info.media[i].type);
                        callInfo.Media[i] = mediaInfo;
                    }
                }

                if (info.provMedia != null)
                {
                    callInfo.ProvMedia = new CallMediaInfo[info.provMedia.Count];
                    for (int i = 0; i < info.provMedia.Count; i++)
                    {
                        CallMediaInfo mediaInfo = new CallMediaInfo();
                        mediaInfo.AudioConfSlot = info.provMedia[i].audioConfSlot;
                        mediaInfo.Direction = CallInfo.GetMediaDirectionEx(info.provMedia[i].dir);
                        mediaInfo.Index = info.provMedia[i].index;
                        mediaInfo.VideoCapDev = info.provMedia[i].videoCapDev;
                        mediaInfo.VideoIncomingWindowId = info.provMedia[i].videoIncomingWindowId;
                        mediaInfo.Status = CallInfo.GetCallMediaStatusEx(info.provMedia[i].status);
                        mediaInfo.Type = MediaFormat.GetMediaTypeEx(info.provMedia[i].type);
                        callInfo.ProvMedia[i] = mediaInfo;
                    }
                }
            }
            catch (Exception)
            {
                // Some error.
                callInfo = null;
            }

            // Return the call info.
            return callInfo;
        }

        ///	<summary>
        ///	Check if this call has active INVITE session and the INVITE
        /// session has not been disconnected.
        ///	</summary>
        /// <returns>True if the call is active; else false.</returns>
        public bool IsActive()
        {
            return _pjCall.isActive();
        }

        ///	<summary>
        ///	Get PJSUA-LIB call ID or index associated with this call.
        ///	</summary>
        /// <returns>The current call id.</returns>
        public int GetId()
        {
            return _pjCall.getId();
        }

        ///	<summary>
        ///	Check if call has an active media session.
        ///	</summary>
        /// <returns>True if the call has media; else false.</returns>
        public bool HasMedia()
        {
            return _pjCall.hasMedia();
        }

        ///	<summary>
        ///	Check if remote peer support the specified capability.
        ///	</summary>
        /// <param name="medIdx">The media index.</param>
        /// <returns>The media.</returns>
        public MediaBase GetMedia(uint medIdx)
        {
            MediaBase mdiaBase = null;
            pjsua2.AudioMedia audioMedia = null;
            pjsua2.pjmedia_type mediaType = pjsua2.pjmedia_type.PJMEDIA_TYPE_NONE;

            // Get the media.
            pjsua2.Media media = _pjCall.getMedia(medIdx);

            // If media not null.
            if (media != null)
            {
                // Get the media type.
                mediaType = media.getType();

                // Select the correct media type.
                switch (mediaType)
                {
                    case pjsua2.pjmedia_type.PJMEDIA_TYPE_AUDIO:
                        audioMedia = (pjsua2.AudioMedia)media;
                        mdiaBase = new AudioMedia(audioMedia);
                        break;

                    case pjsua2.pjmedia_type.PJMEDIA_TYPE_VIDEO:
                        mdiaBase = new VideoMedia();
                        break;

                    case pjsua2.pjmedia_type.PJMEDIA_TYPE_APPLICATION:
                        mdiaBase = new ApplicationMedia();
                        break;

                    case pjsua2.pjmedia_type.PJMEDIA_TYPE_UNKNOWN:
                        mdiaBase = new UnknownMedia();
                        break;

                    case pjsua2.pjmedia_type.PJMEDIA_TYPE_NONE:
                    default:
                        mdiaBase = new NoMedia();
                        break;
                }
            }

            // Return the media;
            return mdiaBase;
        }

        ///	<summary>
        ///	Get media for the specified media index.
        ///	</summary>
        /// <param name="htype">The header type (pjsip_hdr_e) to be checked, which
        /// value may be :
        /// HeaderType::PJSIP_H_ACCEPT
        /// HeaderType::PJSIP_H_ALLOW
        /// HeaderType::PJSIP_H_SUPPORTED</param>
        /// <param name="hname">If htype specifies HeaderType::PJSIP_H_OTHER, then the header
        /// name must be supplied in this argument.Otherwise
        /// the value must be set to empty string("").</param>
        /// <param name="token">The capability token to check. For example, if 
        /// htype is HeaderType::PJSIP_H_ALLOW, then token specifies the
        /// method names; if htype is HeaderType::PJSIP_H_SUPPORTED, then
        /// token specifies the extension names such as "100rel".</param>
        /// <returns>If the specified capability is explicitly supported.</returns>
        public DialogCapStatus RemoteHasCap(HeaderType htype, string hname, string token)
        {
            pjsua2.pjsip_dialog_cap_status dialogCapStatus = _pjCall.remoteHasCap((int)htype, hname, token);
            return CallInfo.GetDialogCapStatusEx(dialogCapStatus);
        }

        ///	<summary>
        ///	Get the NAT type of remote's endpoint. This is a proprietary feature
        /// of PJSUA - LIB which sends its NAT type in the SDP when natTypeInSdp
        /// is set in UaConfig. This function can only be called after SDP has been received from remote,
        /// which means for incoming call, this function can be called as soon as
        /// call is received as long as incoming call contains SDP, and for outgoing
        /// call, this function can be called only after SDP is received(normally in
        /// 200 OK response to INVITE).As a general case, application should call
        /// this function after or in onCallMediaState() callback.
        ///	</summary>
        /// <returns>The NAT type.</returns>
        public StunNatType GetRemNatType()
        {
            pjsua2.pj_stun_nat_type stunNatType = _pjCall.getRemNatType();
            return CallInfo.GetStunNatTypeEx(stunNatType);
        }

        ///	<summary>
        ///	Make outgoing call to the specified URI.
        ///	</summary>
        /// <param name="uri">URI to be put in the To header (normally is the same as the target URI).</param>
        /// <param name="callOpParam">Optional call setting.</param>
        public void MakeCall(string uri, CallOpParam callOpParam)
        {
            pjsua2.CallOpParam prm = new pjsua2.CallOpParam(callOpParam.UseDefaultCallSetting);
            GetCallOpParam(callOpParam, prm);

            // Make the call.
            _pjCall.makeCall(uri, prm);
        }

        ///	<summary>
        ///	Send response to incoming INVITE request with call setting param.
        /// Depending on the status code specified as parameter, this function may
        /// send provisional response, establish the call, or terminate the call.
        /// Notes about call setting:
        ///  -if call setting is changed in the subsequent call to this function,
        ///   only the first call setting supplied will applied.So normally
        ///    application will not supply call setting before getting confirmation
        ///    from the user.
        ///  -if no call setting is supplied when SDP has to be sent, i.e: answer
        ///    with status code 183 or 2xx, the default call setting will be used,
        ///check CallSetting for its default values.
        ///	</summary>
        /// <param name="callOpParam">Optional call setting. callOpParam.statusCode Status code, (100-699).</param>
        public void Answer(CallOpParam callOpParam)
        {
            pjsua2.CallOpParam prm = new pjsua2.CallOpParam(callOpParam.UseDefaultCallSetting);
            GetCallOpParam(callOpParam, prm);

            // Answer the call.
            _pjCall.answer(prm);
        }

        ///	<summary>
        ///	Hangup call by using method that is appropriate according to the
        /// call state.This function is different than answering the call with
        /// 3xx - 6xx response(with answer()), in that this function
        /// will hangup the call regardless of the state and role of the call,
        /// while answer() only works with incoming calls on EARLY state.
        ///	</summary>
        /// <param name="callOpParam">Optional call setting. Incoming call. If the value is zero, "603/Decline" will be sent.</param>
        public void Hangup(CallOpParam callOpParam)
        {
            pjsua2.CallOpParam prm = new pjsua2.CallOpParam(callOpParam.UseDefaultCallSetting);
            GetCallOpParam(callOpParam, prm);

            // Hangup the call.
            _pjCall.hangup(prm);
        }

        ///	<summary>
        ///	Put the specified call on hold. This will send re-INVITE with the
        /// appropriate SDP to inform remote that the call is being put on hold.
        /// The final status of the request itself will be reported on the
        /// onCallMediaState() callback, which inform the application that
        /// the media state of the call has changed.
        ///	</summary>
        /// <param name="callOpParam">Optional call setting. 
        /// callOpParam.options   Bitmask of pjsua_call_flag constants. Currently,
        /// only the flag PJSUA_CALL_UPDATE_CONTACT can be used.</param>
        public void SetHold(CallOpParam callOpParam)
        {
            pjsua2.CallOpParam prm = new pjsua2.CallOpParam(callOpParam.UseDefaultCallSetting);
            GetCallOpParam(callOpParam, prm);

            // Hold the call.
            _pjCall.setHold(prm);
        }

        ///	<summary>
        ///	Send re-INVITE.
        /// The final status of the request itself will be reported on the
        /// onCallMediaState() callback, which inform the application that
        /// the media state of the call has changed.
        ///	</summary>
        /// <param name="callOpParam">Optional call setting. 
        /// callOpParam.opt.flag  Bitmask of pjsua_call_flag constants. Specifying
        /// PJSUA_CALL_UNHOLD here will release call hold.</param>
        public void Reinvite(CallOpParam callOpParam)
        {
            pjsua2.CallOpParam prm = new pjsua2.CallOpParam(callOpParam.UseDefaultCallSetting);
            GetCallOpParam(callOpParam, prm);

            // Reinvite the call.
            _pjCall.reinvite(prm);
        }

        ///	<summary>
        ///	Send UPDATE request.
        ///	</summary>
        /// <param name="callOpParam">Optional call setting.</param>
        public void Update(CallOpParam callOpParam)
        {
            pjsua2.CallOpParam prm = new pjsua2.CallOpParam(callOpParam.UseDefaultCallSetting);
            GetCallOpParam(callOpParam, prm);

            // Update the call.
            _pjCall.update(prm);
        }

        ///	<summary>
        ///	Initiate call transfer to the specified address. This function will send
        /// REFER request to instruct remote call party to initiate a new INVITE
        /// session to the specified destination / target.
        /// If application is interested to monitor the successfulness and
        /// the progress of the transfer request, it can implement
        /// onCallTransferStatus() callback which will report the progress
        /// of the call transfer request.
        ///	</summary>
        /// <param name="destination">The URI of new target to be contacted. The URI may be
        /// in name address or addr - spec format.</param>
        /// <param name="callOpParam">Optional call setting.</param>
        public void Transfer(string destination, CallOpParam callOpParam)
        {
            pjsua2.CallOpParam prm = new pjsua2.CallOpParam(callOpParam.UseDefaultCallSetting);
            GetCallOpParam(callOpParam, prm);

            // Transfer the call.
            _pjCall.xfer(destination, prm);
        }

        ///	<summary>
        ///	Initiate call transfer to the specified address. This function will send
        /// REFER request to instruct remote call party to initiate a new INVITE
        /// session to the specified destination / target.
        /// If application is interested to monitor the successfulness and
        /// the progress of the transfer request, it can implement
        /// onCallTransferStatus() callback which will report the progress
        /// of the call transfer request.
        ///	</summary>
        /// <param name="destination">The call to be replaced.</param>
        /// <param name="callOpParam">Optional call setting. callOpParam.options Application may specify
        /// PJSUA_XFER_NO_REQUIRE_REPLACES to suppress the
        /// inclusion of "Require: replaces" in
        /// the outgoing INVITE request created by the REFER request.</param>
        public void TransferReplaces(Call destination, CallOpParam callOpParam)
        {
            pjsua2.CallOpParam prm = new pjsua2.CallOpParam(callOpParam.UseDefaultCallSetting);
            GetCallOpParam(callOpParam, prm);

            // Get the call reference.
            CallCallback call = destination.PjCall;

            // Transfer replace the call.
            _pjCall.xferReplaces(call, prm);
        }

        ///	<summary>
        ///	Accept or reject redirection response. Application MUST call this
        /// function after it signaled PJSIP_REDIRECT_PENDING in the
        /// onCallRedirected() callback,
        /// to notify the call whether to accept or reject the redirection
        /// to the current target.Application can use the combination of
        /// PJSIP_REDIRECT_PENDING command in onCallRedirected() callback and
        /// this function to ask for user permission before redirecting the call.
        /// Note that if the application chooses to reject or stop redirection(by
        /// using PJSIP_REDIRECT_REJECT or PJSIP_REDIRECT_STOP respectively), the
        /// call disconnection callback will be called before this function returns.
        /// And if the application rejects the target, the \a onCallRedirected()
        /// callback may also be called before this function returns if there is
        /// another target to try.
        ///	</summary>
        /// <param name="redirectResponse">
        /// Redirection operation to be applied to the current
        /// target.The semantic of this argument is similar
        /// to the description in the onCallRedirected()
        /// callback, except that the PJSIP_REDIRECT_PENDING is
        /// not accepted here.</param>
        public void ProcessRedirect(RedirectResponseType redirectResponse)
        {
            pjsua2.pjsip_redirect_op cmd = CallInfo.GetRedirectResponseType(redirectResponse);
            _pjCall.processRedirect(cmd);
        }

        ///	<summary>
        ///	Send DTMF digits to remote using RFC 2833 payload formats.
        ///	</summary>
        /// <param name="digits">DTMF string digits to be sent.</param>
        public void DialDtmf(string digits)
        {
            // Send DTMF digits to remote using RFC 2833 payload formats.
            _pjCall.dialDtmf(digits);
        }

        /// <summary>
        /// Send instant messaging inside INVITE session.
        /// </summary>
        /// <param name="sendInstantMessageParam">Sending instant message parameter.</param>
        public void SendInstantMessage(SendInstantMessage sendInstantMessageParam)
        {
            pjsua2.SendInstantMessageParam prm = new pjsua2.SendInstantMessageParam();
            prm.content = sendInstantMessageParam.Content;
            prm.contentType = sendInstantMessageParam.ContentType;

            if (sendInstantMessageParam.TxOption != null)
            {
                prm.txOption.contentType = sendInstantMessageParam.TxOption.ContentType;
                prm.txOption.msgBody = sendInstantMessageParam.TxOption.MsgBody;
                prm.txOption.targetUri = sendInstantMessageParam.TxOption.TargetUri;

                if (sendInstantMessageParam.TxOption.Headers != null && sendInstantMessageParam.TxOption.Headers.SipHeaders != null)
                {
                    prm.txOption.headers = new pjsua2.SipHeaderVector();
                    for (int i = 0; i < sendInstantMessageParam.TxOption.Headers.Count; i++)
                    {
                        pjsua2.SipHeader header = new pjsua2.SipHeader();
                        header.hName = sendInstantMessageParam.TxOption.Headers.SipHeaders[i].Name;
                        header.hValue = sendInstantMessageParam.TxOption.Headers.SipHeaders[i].Value;
                        prm.txOption.headers.Add(header);
                    }
                }

                if (sendInstantMessageParam.TxOption.MultipartContentType != null)
                {
                    prm.txOption.multipartContentType = new pjsua2.SipMediaType();
                    prm.txOption.multipartContentType.subType = sendInstantMessageParam.TxOption.MultipartContentType.SubType;
                    prm.txOption.multipartContentType.type = sendInstantMessageParam.TxOption.MultipartContentType.Type;
                }

                if (sendInstantMessageParam.TxOption.MultipartParts != null && sendInstantMessageParam.TxOption.MultipartParts.SipMultipartParts != null)
                {
                    prm.txOption.multipartParts = new pjsua2.SipMultipartPartVector();
                    for (int i = 0; i < sendInstantMessageParam.TxOption.MultipartParts.Count; i++)
                    {
                        pjsua2.SipMultipartPart mulPart = new pjsua2.SipMultipartPart();
                        mulPart.body = sendInstantMessageParam.TxOption.MultipartParts.SipMultipartParts[i].Body;

                        SipMediaType mediaType = sendInstantMessageParam.TxOption.MultipartParts.SipMultipartParts[i].ContentType;
                        mulPart.contentType = new pjsua2.SipMediaType();
                        mulPart.contentType.subType = mediaType.SubType;
                        mulPart.contentType.type = mediaType.Type;

                        if (sendInstantMessageParam.TxOption.MultipartParts.SipMultipartParts[i].Headers != null &&
                            sendInstantMessageParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.SipHeaders != null)
                        {
                            mulPart.headers = new pjsua2.SipHeaderVector();
                            for (int j = 0; j < sendInstantMessageParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.Count; j++)
                            {
                                pjsua2.SipHeader header = new pjsua2.SipHeader();
                                header.hName = sendInstantMessageParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.SipHeaders[j].Name;
                                header.hValue = sendInstantMessageParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.SipHeaders[j].Value;
                                mulPart.headers.Add(header);
                            }
                        }

                        prm.txOption.multipartParts.Add(mulPart);
                    }
                }
            }

            // Send message.
            _pjCall.sendInstantMessage(prm);
        }

        /// <summary>
        /// Send IM typing indication inside INVITE session.
        /// </summary>
        /// <param name="sendTypingIndicationParam">Sending instant message parameter.</param>
        public void SendTypingIndication(SendTypingIndication sendTypingIndicationParam)
        {
            pjsua2.SendTypingIndicationParam prm = new pjsua2.SendTypingIndicationParam();
            prm.isTyping = sendTypingIndicationParam.IsTyping;
            if (sendTypingIndicationParam.TxOption != null)
            {
                prm.txOption.contentType = sendTypingIndicationParam.TxOption.ContentType;
                prm.txOption.msgBody = sendTypingIndicationParam.TxOption.MsgBody;
                prm.txOption.targetUri = sendTypingIndicationParam.TxOption.TargetUri;

                if (sendTypingIndicationParam.TxOption.Headers != null && sendTypingIndicationParam.TxOption.Headers.SipHeaders != null)
                {
                    prm.txOption.headers = new pjsua2.SipHeaderVector();
                    for (int i = 0; i < sendTypingIndicationParam.TxOption.Headers.Count; i++)
                    {
                        pjsua2.SipHeader header = new pjsua2.SipHeader();
                        header.hName = sendTypingIndicationParam.TxOption.Headers.SipHeaders[i].Name;
                        header.hValue = sendTypingIndicationParam.TxOption.Headers.SipHeaders[i].Value;
                        prm.txOption.headers.Add(header);
                    }
                }

                if (sendTypingIndicationParam.TxOption.MultipartContentType != null)
                {
                    prm.txOption.multipartContentType = new pjsua2.SipMediaType();
                    prm.txOption.multipartContentType.subType = sendTypingIndicationParam.TxOption.MultipartContentType.SubType;
                    prm.txOption.multipartContentType.type = sendTypingIndicationParam.TxOption.MultipartContentType.Type;
                }

                if (sendTypingIndicationParam.TxOption.MultipartParts != null && sendTypingIndicationParam.TxOption.MultipartParts.SipMultipartParts != null)
                {
                    prm.txOption.multipartParts = new pjsua2.SipMultipartPartVector();
                    for (int i = 0; i < sendTypingIndicationParam.TxOption.MultipartParts.Count; i++)
                    {
                        pjsua2.SipMultipartPart mulPart = new pjsua2.SipMultipartPart();
                        mulPart.body = sendTypingIndicationParam.TxOption.MultipartParts.SipMultipartParts[i].Body;

                        SipMediaType mediaType = sendTypingIndicationParam.TxOption.MultipartParts.SipMultipartParts[i].ContentType;
                        mulPart.contentType = new pjsua2.SipMediaType();
                        mulPart.contentType.subType = mediaType.SubType;
                        mulPart.contentType.type = mediaType.Type;

                        if (sendTypingIndicationParam.TxOption.MultipartParts.SipMultipartParts[i].Headers != null &&
                            sendTypingIndicationParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.SipHeaders != null)
                        {
                            mulPart.headers = new pjsua2.SipHeaderVector();
                            for (int j = 0; j < sendTypingIndicationParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.Count; j++)
                            {
                                pjsua2.SipHeader header = new pjsua2.SipHeader();
                                header.hName = sendTypingIndicationParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.SipHeaders[j].Name;
                                header.hValue = sendTypingIndicationParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.SipHeaders[j].Value;
                                mulPart.headers.Add(header);
                            }
                        }

                        prm.txOption.multipartParts.Add(mulPart);
                    }
                }
            }

            // Send typing.
            _pjCall.sendTypingIndication(prm);
        }

        ///	<summary>
        ///	Get the pj call options.
        ///	</summary>
        /// <param name="callOpParam">Optional call setting.</param>
        /// <param name="prm">Optional pj call setting.</param>
        private void GetCallOpParam(CallOpParam callOpParam, pjsua2.CallOpParam prm)
        {
            // If call settings.
            if (callOpParam.Setting != null)
            {
                prm.opt = new pjsua2.CallSetting();
                prm.opt.audioCount = callOpParam.Setting.AudioCount;
                prm.opt.flag = (uint)callOpParam.Setting.Flag;
                prm.opt.reqKeyframeMethod = (uint)callOpParam.Setting.ReqKeyframeMethod;
                prm.opt.videoCount = callOpParam.Setting.VideoCount;
            }

            prm.options = callOpParam.Options;
            prm.reason = callOpParam.Reason;
            prm.statusCode = AccountInfo.GetStatusCode(callOpParam.Code);

            // If call settings.
            if (callOpParam.TxOption != null)
            {
                prm.txOption = new pjsua2.SipTxOption();
                prm.txOption.contentType = callOpParam.TxOption.ContentType;
                prm.txOption.msgBody = callOpParam.TxOption.MsgBody;
                prm.txOption.targetUri = callOpParam.TxOption.TargetUri;

                // If call settings.
                if (callOpParam.TxOption.MultipartContentType != null)
                {
                    prm.txOption.multipartContentType = new pjsua2.SipMediaType();
                    prm.txOption.multipartContentType.subType = callOpParam.TxOption.MultipartContentType.SubType;
                    prm.txOption.multipartContentType.type = callOpParam.TxOption.MultipartContentType.Type;
                }

                // If call settings.
                if (callOpParam.TxOption.Headers != null)
                {
                    pjsua2.SipHeaderVector headers = new pjsua2.SipHeaderVector();

                    // For each header
                    for (int i = 0; i < callOpParam.TxOption.Headers.Count; i++)
                    {
                        // Get the current header.
                        SipHeader sipHeader = callOpParam.TxOption.Headers.SipHeaders[i];

                        // Add the sip header.
                        pjsua2.SipHeader header = new pjsua2.SipHeader();
                        header.hName = sipHeader.Name;
                        header.hValue = sipHeader.Value;
                        headers.Add(header);
                    }

                    // Ass the headers.
                    prm.txOption.headers = headers;
                }
            }
        }

        /// <summary>
        /// Call callbacks.
        /// </summary>
        internal class CallCallback : pjsua2.Call
        {
            /// <summary>
            /// Call callbacks.
            /// </summary>
            /// <param name="account">The Sip account.</param>
            public CallCallback(pjsua2.Account account) : base(account) { }

            /// <summary>
            /// Call callbacks.
            /// </summary>
            /// <param name="account">The Sip account.</param>
            /// <param name="callId">An index call id (0 - 3).</param>
            public CallCallback(pjsua2.Account account, int callId) : base(account, callId) { }

            private bool _disposed = false;

            /// <summary>
            /// Notify application when call state has changed.
            /// Application may then query the call info to get the
            /// detail call states by calling getInfo() function.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onCallState(pjsua2.OnCallStateParam prm)
            {
                
            }

            /// <summary>
            /// Notify application when media state in the call has changed.
            /// Normal application would need to implement this callback, e.g.
            /// to connect the call's media to sound device. When ICE is used,
            /// this callback will also be called to report ICE negotiation failure.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onCallMediaState(pjsua2.OnCallMediaStateParam prm)
            {
                
            }

            /// <summary>
            /// This is a general notification callback which is called whenever
            /// a transaction within the call has changed state.Application can
            /// implement this callback for example to monitor the state of
            /// outgoing requests, or to answer unhandled incoming requests
            /// (such as INFO) with a final response.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onCallTsxState(pjsua2.OnCallTsxStateParam prm)
            {
                
            }

            /// <summary>
            /// Notify application when a call has just created a local SDP (for
            /// initial or subsequent SDP offer / answer).Application can implement
            /// this callback to modify the SDP, before it is being sent and / or
            /// negotiated with remote SDP, for example to apply per account / call
            /// basis codecs priority or to add custom / proprietary SDP attributes.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onCallSdpCreated(pjsua2.OnCallSdpCreatedParam prm)
            {
                
            }

            /// <summary>
            /// Notify application when media session is created and before it is
            /// registered to the conference bridge.Application may return different
            /// media port if it has added media processing port to the stream.This
            /// media port then will be added to the conference bridge instead.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onStreamCreated(pjsua2.OnStreamCreatedParam prm)
            {
                
            }

            /// <summary>
            /// Notify application when media session has been unregistered from the
            /// conference bridge and about to be destroyed.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onStreamDestroyed(pjsua2.OnStreamDestroyedParam prm)
            {
                
            }

            /// <summary>
            /// Notify application upon incoming DTMF digits.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onDtmfDigit(pjsua2.OnDtmfDigitParam prm)
            {
                
            }

            /// <summary>
            /// Notify application on call being transferred (i.e. REFER is received).
            /// Application can decide to accept / reject transfer request
            /// by setting the code(default is 202).When this callback
            /// is not implemented, the default behavior is to accept the
            /// transfer.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onCallTransferRequest(pjsua2.OnCallTransferRequestParam prm)
            {
                
            }

            /// <summary>
            /// Notify application of the status of previously sent call
            /// transfer request.Application can monitor the status of the
            /// call transfer request, for example to decide whether to
            /// terminate existing call.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onCallTransferStatus(pjsua2.OnCallTransferStatusParam prm)
            {
                
            }

            /// <summary>
            /// Notify application about incoming INVITE with Replaces header.
            /// Application may reject the request by setting non - 2xx code.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onCallReplaceRequest(pjsua2.OnCallReplaceRequestParam prm)
            {
                
            }

            /// <summary>
            /// Notify application that an existing call has been replaced with
            /// a new call.This happens when PJSUA - API receives incoming INVITE
            /// request with Replaces header.
            /// After this callback is called, normally PJSUA - API will disconnect
            /// this call and establish a new call newCallId.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onCallReplaced(pjsua2.OnCallReplacedParam prm)
            {
                
            }

            /// <summary>
            /// Notify application when call has received new offer from remote
            /// (i.e.re - INVITE / UPDATE with SDP is received).Application can
            /// decide to accept / reject the offer by setting the code(default
            /// is 200).If the offer is accepted, application can update the
            /// call setting to be applied in the answer.When this callback is
            /// not implemented, the default behavior is to accept the offer using
            /// current call setting.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onCallRxOffer(pjsua2.OnCallRxOfferParam prm)
            {
                
            }

            /// <summary>
            /// Notify application on incoming MESSAGE request.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onInstantMessage(pjsua2.OnInstantMessageParam prm)
            {
                
            }

            /// <summary>
            /// Notify application about the delivery status of outgoing MESSAGE request.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onInstantMessageStatus(pjsua2.OnInstantMessageStatusParam prm)
            {
                
            }

            /// <summary>
            /// Notify application about typing indication.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onTypingIndication(pjsua2.OnTypingIndicationParam prm)
            {
                
            }

            /// <summary>
            /// This callback is called when the call is about to resend the
            /// INVITE request to the specified target, following the previously
            /// received redirection response.
            ///
            /// Application may accept the redirection to the specified target,
            /// reject this target only and make the session continue to try the next
            /// target in the list if such target exists, stop the whole
            /// redirection process altogether and cause the session to be
            /// disconnected, or defer the decision to ask for user confirmation.
            ///
            /// This callback is optional, the default behavior is to NOT follow the redirection response.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            /// <returns>Redirection options.</returns>
            public override pjsua2.pjsip_redirect_op onCallRedirected(pjsua2.OnCallRedirectedParam prm)
            {
                return pjsua2.pjsip_redirect_op.PJSIP_REDIRECT_STOP;
            }

            /// <summary>
            /// This callback is called when media transport state is changed.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onCallMediaTransportState(pjsua2.OnCallMediaTransportStateParam prm)
            {
                
            }

            /// <summary>
            /// Notification about media events such as video notifications. This
            /// callback will most likely be called from media threads, thus
            /// application must not perform heavy processing in this callback.
            /// Especially, application must not destroy the call or media in this
            /// callback.If application needs to perform more complex tasks to
            /// handle the event, it should post the task to another thread.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onCallMediaEvent(pjsua2.OnCallMediaEventParam prm)
            {
                
            }

            /// <summary>
            /// This callback can be used by application to implement custom media
            /// transport adapter for the call, or to replace the media transport
            /// with something completely new altogether.
            ///
            /// This callback is called when a new call is created.The library has
            /// created a media transport for the call, and it is provided as the
            /// mediaTp argument of this callback.The callback may change it
            /// with the instance of media transport to be used by the call.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onCreateMediaTransport(pjsua2.OnCreateMediaTransportParam prm)
            {
                
            }

            /// <summary>
            /// Dispose.
            /// </summary>
            public override void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;

                    base.Dispose();
                }
            }
        }

        #region Dispose Object Methods

        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_pjCall != null)
                        _pjCall.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _pjCall = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Call()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
