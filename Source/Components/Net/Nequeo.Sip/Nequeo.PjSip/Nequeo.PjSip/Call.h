/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          Call.h
*  Purpose :       SIP Call class.
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

#ifndef _CALL_H
#define _CALL_H

#include "stdafx.h"

#include "Account.h"
#include "CallCallback.h"
#include "CallMapper.h"
#include "Media.h"
#include "DialogCapStatus.h"
#include "HeaderType.h"
#include "StunNatType.h"
#include "CallOpParam.h"
#include "CallInfo.h" 
#include "RedirectResponseType.h"
#include "SendInstantMessage.h"
#include "SendTypingIndication.h"
#include "CallSendRequest.h"
#include "MediaDirection.h"
#include "VideoStreamOperation.h"
#include "CallSetVideoStream.h"
#include "MediaStreamInfo.h"
#include "MediaStreamStat.h"
#include "MediaTransportInfo.h"
#include "SipEventType.h"
#include "MediaEvent.h"
#include "MediaEventData.h"
#include "MediaFmtChangedEvent.h"
#include "MediaTransportState.h"
#include "VideoWindow.h"

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
			ref class OnCallStateParam;
			ref class OnCallMediaStateParam;
			ref class OnCallTsxStateParam;
			ref class OnCallSdpCreatedParam;
			ref class OnStreamCreatedParam;
			ref class OnStreamDestroyedParam;
			ref class OnDtmfDigitParam;
			ref class OnCallTransferRequestParam;
			ref class OnCallTransferStatusParam;
			ref class OnCallReplaceRequestParam;
			ref class OnCallReplacedParam;
			ref class OnCallRxOfferParam;
			ref class OnInstantMessageParam;
			ref class OnInstantMessageStatusParam;
			ref class OnTypingIndicationParam;
			ref class OnCallRedirectedParam;
			ref class OnCallMediaTransportStateParam;
			ref class OnCallMediaEventParam;
			ref class OnCreateMediaTransportParam;

			delegate void OnCallStateCallback(pj::OnCallStateParam&);
			delegate void OnCallMediaStateCallback(pj::OnCallMediaStateParam&);
			delegate void OnCallTsxStateCallback(pj::OnCallTsxStateParam&);
			delegate void OnCallSdpCreatedCallback(pj::OnCallSdpCreatedParam&);
			delegate void OnStreamCreatedCallback(pj::OnStreamCreatedParam&);
			delegate void OnStreamDestroyedCallback(pj::OnStreamDestroyedParam&);
			delegate void OnDtmfDigitCallback(pj::OnDtmfDigitParam&);
			delegate void OnCallTransferRequestCallback(pj::OnCallTransferRequestParam&);
			delegate void OnCallTransferStatusCallback(pj::OnCallTransferStatusParam&);
			delegate void OnCallReplaceRequestCallback(pj::OnCallReplaceRequestParam&);
			delegate void OnCallReplacedCallback(pj::OnCallReplacedParam&);
			delegate void OnCallRxOfferCallback(pj::OnCallRxOfferParam&);
			delegate void OnCallInstantMessageCallback(pj::OnInstantMessageParam&);
			delegate void OnCallInstantMessageStatusCallback(pj::OnInstantMessageStatusParam&);
			delegate void OnCallTypingIndicationCallback(pj::OnTypingIndicationParam&);
			delegate pjsip_redirect_op OnCallRedirectedCallback(pj::OnCallRedirectedParam&);
			delegate void OnCallMediaTransportStateCallback(pj::OnCallMediaTransportStateParam&);
			delegate void OnCallMediaEventCallback(pj::OnCallMediaEventParam&);
			delegate void OnCreateMediaTransportCallback(pj::OnCreateMediaTransportParam&);

			/// <summary>
			/// Sip call.
			/// </summary>
			public ref class Call sealed
			{
			public:
				/// <summary>
				/// Sip call.
				/// </summary>
				/// <param name="account">The Sip account.</param>
				/// <param name="callId">An index call id (0 - 3).</param>
				Call(Account^ account, int callId);

				///	<summary>
				///	Sip call.
				///	</summary>
				~Call();

				///	<summary>
				///	Sip call.
				///	</summary>
				!Call();

				/// <summary>
				/// Notify application when call state has changed.
				/// Application may then query the call info to get the
				/// detail call states by calling getInfo() function.
				/// </summary>
				event System::EventHandler<OnCallStateParam^>^ OnCallState;

				/// <summary>
				/// Notify application when media state in the call has changed.
				/// Normal application would need to implement this callback, e.g.
				/// to connect the call's media to sound device. When ICE is used,
				/// this callback will also be called to report ICE negotiation failure.
				/// </summary>
				event System::EventHandler<OnCallMediaStateParam^>^ OnCallMediaState;

				/// <summary>
				/// This is a general notification callback which is called whenever
				/// a transaction within the call has changed state.Application can
				/// implement this callback for example to monitor the state of
				/// outgoing requests, or to answer unhandled incoming requests
				/// (such as INFO) with a final response.
				/// </summary>
				event System::EventHandler<OnCallTsxStateParam^>^ OnCallTsxState;

				/// <summary>
				/// Notify application when a call has just created a local SDP (for
				/// initial or subsequent SDP offer / answer).Application can implement
				/// this callback to modify the SDP, before it is being sent and / or
				/// negotiated with remote SDP, for example to apply per account / call
				/// basis codecs priority or to add custom / proprietary SDP attributes.
				/// </summary>
				event System::EventHandler<OnCallSdpCreatedParam^>^ OnCallSdpCreated;

				/// <summary>
				/// Notify application when media session is created and before it is
				/// registered to the conference bridge.Application may return different
				/// media port if it has added media processing port to the stream.This
				/// media port then will be added to the conference bridge instead.
				/// </summary>
				event System::EventHandler<OnStreamCreatedParam^>^ OnStreamCreated;

				/// <summary>
				/// Notify application when media session has been unregistered from the
				/// conference bridge and about to be destroyed.
				/// </summary>
				event System::EventHandler<OnStreamDestroyedParam^>^ OnStreamDestroyed;

				/// <summary>
				/// Notify application upon incoming DTMF digits.
				/// </summary>
				event System::EventHandler<OnDtmfDigitParam^>^ OnDtmfDigit;

				/// <summary>
				/// Notify application on call being transferred (i.e. REFER is received).
				/// Application can decide to accept / reject transfer request
				/// by setting the code(default is 202).When this callback
				/// is not implemented, the default behavior is to accept the
				/// transfer.
				/// </summary>
				event System::EventHandler<OnCallTransferRequestParam^>^ OnCallTransferRequest;

				/// <summary>
				/// Notify application of the status of previously sent call
				/// transfer request.Application can monitor the status of the
				/// call transfer request, for example to decide whether to
				/// terminate existing call.
				/// </summary>
				event System::EventHandler<OnCallTransferStatusParam^>^ OnCallTransferStatus;

				/// <summary>
				/// Notify application about incoming INVITE with Replaces header.
				/// Application may reject the request by setting non - 2xx code.
				/// </summary>
				event System::EventHandler<OnCallReplaceRequestParam^>^ OnCallReplaceRequest;

				/// <summary>
				/// Notify application that an existing call has been replaced with
				/// a new call.This happens when PJSUA - API receives incoming INVITE
				/// request with Replaces header.
				/// After this callback is called, normally PJSUA - API will disconnect
				/// this call and establish a new call \a newCallId.
				/// </summary>
				event System::EventHandler<OnCallReplacedParam^>^ OnCallReplaced;

				/// <summary>
				/// Notify application when call has received new offer from remote
				/// (i.e.re - INVITE / UPDATE with SDP is received).Application can
				/// decide to accept / reject the offer by setting the code(default
				/// is 200).If the offer is accepted, application can update the
				/// call setting to be applied in the answer.When this callback is
				/// not implemented, the default behavior is to accept the offer using
				/// current call setting.
				/// </summary>
				event System::EventHandler<OnCallRxOfferParam^>^ OnCallRxOffer;

				/// <summary>
				/// Notify application on incoming MESSAGE request.
				/// </summary>
				event System::EventHandler<OnInstantMessageParam^>^ OnInstantMessage;

				/// <summary>
				/// Notify application about the delivery status of outgoing MESSAGE request.
				/// </summary>
				event System::EventHandler<OnInstantMessageStatusParam^>^ OnInstantMessageStatus;

				/// <summary>
				/// Notify application about typing indication.
				/// </summary>
				event System::EventHandler<OnTypingIndicationParam^>^ OnTypingIndication;

				/// <summary>
				/// This callback is called when the call is about to resend the
				/// INVITE request to the specified target, following the previously
				/// received redirection response.
				/// Application may accept the redirection to the specified target,
				/// reject this target only and make the session continue to try the next
				/// target in the list if such target exists, stop the whole
				/// redirection process altogether and cause the session to be
				/// disconnected, or defer the decision to ask for user confirmation.
				/// This callback is optional,
				/// the default behavior is to NOT follow the redirection response.
				/// </summary>
				event System::EventHandler<OnCallRedirectedParam^>^ OnCallRedirected;

				/// <summary>
				/// This callback is called when media transport state is changed.
				/// </summary>
				event System::EventHandler<OnCallMediaTransportStateParam^>^ OnCallMediaTransportState;

				/// <summary>
				/// Notification about media events such as video notifications. This
				/// callback will most likely be called from media threads, thus
				/// application must not perform heavy processing in this callback.
				/// Especially, application must not destroy the call or media in this
				/// callback.If application needs to perform more complex tasks to
				/// handle the event, it should post the task to another thread.
				/// </summary>
				event System::EventHandler<OnCallMediaEventParam^>^ OnCallMediaEvent;

				/// <summary>
				/// This callback can be used by application to implement custom media
				/// transport adapter for the call, or to replace the media transport
				/// with something completely new altogether.
				/// This callback is called when a new call is created.The library has
				/// created a media transport for the call, and it is provided as the
				/// \a mediaTp argument of this callback.The callback may change it
				/// with the instance of media transport to be used by the call.
				/// </summary>
				event System::EventHandler<OnCreateMediaTransportParam^>^ OnCreateMediaTransport;

				///	<summary>
				///	Get detail information about this call.
				///	</summary>
				/// <returns>Call information.</returns>
				CallInfo^ GetInfo();

				///	<summary>
				///	Check if this call has active INVITE session and the INVITE
				/// session has not been disconnected.
				///	</summary>
				/// <returns>True if the call is active; else false.</returns>
				bool IsActive();

				///	<summary>
				///	Get PJSUA-LIB call ID or index associated with this call.
				///	</summary>
				/// <returns>The current call id.</returns>
				int GetId();

				///	<summary>
				///	Check if call has an active media session.
				///	</summary>
				/// <returns>True if the call has media; else false.</returns>
				bool HasMedia();

				///	<summary>
				///	Check if remote peer support the specified capability.
				///	</summary>
				/// <param name="medIdx">The media index.</param>
				/// <returns>The media.</returns>
				MediaBase^ GetMedia(unsigned medIdx);

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
				DialogCapStatus RemoteHasCap(HeaderType htype, String^ hname, String^ token);

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
				StunNatType GetRemNatType();

				///	<summary>
				///	Make outgoing call to the specified URI.
				///	</summary>
				/// <param name="uri">URI to be put in the To header (normally is the same as the target URI).</param>
				/// <param name="callOpParam">Optional call setting.</param>
				void MakeCall(String^ uri, CallOpParam^ callOpParam);

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
				void Answer(CallOpParam^ callOpParam);

				///	<summary>
				///	Hangup call by using method that is appropriate according to the
				/// call state.This function is different than answering the call with
				/// 3xx - 6xx response(with answer()), in that this function
				/// will hangup the call regardless of the state and role of the call,
				/// while answer() only works with incoming calls on EARLY state.
				///	</summary>
				/// <param name="callOpParam">Optional call setting. Incoming call. If the value is zero, "603/Decline" will be sent.</param>
				void Hangup(CallOpParam^ callOpParam);

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
				void SetHold(CallOpParam^ callOpParam);

				///	<summary>
				///	Send re-INVITE.
				/// The final status of the request itself will be reported on the
				/// onCallMediaState() callback, which inform the application that
				/// the media state of the call has changed.
				///	</summary>
				/// <param name="callOpParam">Optional call setting. 
				/// callOpParam.opt.flag  Bitmask of pjsua_call_flag constants. Specifying
				/// PJSUA_CALL_UNHOLD here will release call hold.</param>
				void Reinvite(CallOpParam^ callOpParam);

				///	<summary>
				///	Send UPDATE request.
				///	</summary>
				/// <param name="callOpParam">Optional call setting.</param>
				void Update(CallOpParam^ callOpParam);

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
				void Transfer(String^ destination, CallOpParam^ callOpParam);

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
				void TransferReplaces(Call^ destination, CallOpParam^ callOpParam);

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
				void ProcessRedirect(RedirectResponseType redirectResponse);

				///	<summary>
				///	Send DTMF digits to remote using RFC 2833 payload formats.
				///	</summary>
				/// <param name="digits">DTMF string digits to be sent.</param>
				void DialDtmf(String^ digits);

				/// <summary>
				/// Send instant messaging inside INVITE session.
				/// </summary>
				/// <param name="sendInstantMessageParam">Sending instant message parameter.</param>
				void SendInstantMessage(SendInstantMessageParam^ sendInstantMessageParam);

				/// <summary>
				/// Send IM typing indication inside INVITE session.
				/// </summary>
				/// <param name="sendTypingIndicationParam">Sending instant message parameter.</param>
				void SendTypingIndication(SendTypingIndicationParam^ sendTypingIndicationParam);

				/// <summary>
				/// Send arbitrary request with the call. This is useful for example to send
				/// INFO request.Note that application should not use this function to send
				/// requests which would change the invite session's state, such as
				/// re - INVITE, UPDATE, PRACK, and BYE.
				/// </summary>
				/// <param name="callSendRequestParam">Sending call request parameter.</param>
				void SendRequest(CallSendRequestParam^ callSendRequestParam);

				/// <summary>
				/// Dump call and media statistics to string.
				/// </summary>
				/// <param name="withMedia">True to include media information too.</param>
				/// <param name="indent">Spaces for left indentation.</param>
				/// <returns>Call dump and media statistics string.</returns>
				String^ Dump(bool withMedia, String^ indent);

				/// <summary>
				/// Get the media stream index of the default video stream in the call.
				/// Typically this will just retrieve the stream index of the first
				/// activated video stream in the call.If none is active, it will return
				/// the first inactive video stream.
				/// </summary>
				/// <returns>The media stream index or -1 if no video stream is present in the call.</returns>
				int GetVideoStreamIndex();

				/// <summary>
				/// Determine if video stream for the specified call is currently running
				/// (i.e.has been created, started, and not being paused) for the specified
				/// direction.
				/// </summary>
				/// <param name="mediaIndex">Media stream index, or -1 to specify default video media.</param>
				/// <param name="mediaDirection">The direction to be checked.</param>
				/// <returns>True if stream is currently running for the specified direction.</returns>
				bool VideoStreamIsRunning(int mediaIndex, MediaDirection mediaDirection);

				/// <summary>
				/// Add, remove, modify, and/or manipulate video media stream for the
				/// specified call. This may trigger a re - INVITE or UPDATE to be sent
				/// for the call.
				/// </summary>
				/// <param name="videoStreamOperation">The video stream operation to be performed.</param>
				/// <param name="callSetVideoStreamParam">The parameters for the video stream operation.</param>
				void SetVideoStream(VideoStreamOperation videoStreamOperation, CallSetVideoStreamParam^ callSetVideoStreamParam);

				/// <summary>
				/// Get media stream info for the specified media index.
				/// </summary>
				/// <param name="mediaIndex">Media stream index.</param>
				/// <returns>The stream info.</returns>
				MediaStreamInfo^ GetStreamInfo(unsigned mediaIndex);

				/// <summary>
				/// Get media stream statistic for the specified media index.
				/// </summary>
				/// <param name="mediaIndex">Media stream index.</param>
				/// <returns>The stream statistic.</returns>
				MediaStreamStat^ GetStreamStat(unsigned mediaIndex);

				/// <summary>
				/// Get media transport info for the specified media index.
				/// </summary>
				/// <param name="mediaIndex">Media stream index.</param>
				/// <returns>The transport info.</returns>
				MediaTransportInfo^ GetMedTransportInfo(unsigned mediaIndex);

			internal:
					/// <summary>
					/// Get the call callback reference.
					/// </summary>
					CallCallback& GetCallCallback();

			private:
				bool _disposed;

				CallCallback* _callCallback;
				CallMapper* _callMapper;
				
				void MarshalString(String^ s, std::string& os);
				void MarshalString(String^ s, std::wstring& os);

				/// <summary>
				/// Create the call.
				/// </summary>
				void Create();

				///	<summary>
				///	Get the pj call options.
				///	</summary>
				/// <param name="callOpParam">Optional call setting.</param>
				/// <param name="callOpParam">Optional pj call setting.</param>
				void GetCallOpParam(CallOpParam^ callOpParam, pj::CallOpParam& prm);

				GCHandle _gchOnCallState;
				void OnCallState_Handler(pj::OnCallStateParam &prm);

				GCHandle _gchOnCallMediaState;
				void OnCallMediaState_Handler(pj::OnCallMediaStateParam &prm);

				GCHandle _gchOnCallTsxState;
				void OnCallTsxState_Handler(pj::OnCallTsxStateParam &prm);
				
				GCHandle _gchOnCallSdpCreated;
				void OnCallSdpCreated_Handler(pj::OnCallSdpCreatedParam &prm);

				GCHandle _gchOnStreamCreated;
				void OnStreamCreated_Handler(pj::OnStreamCreatedParam &prm);

				GCHandle _gchOnStreamDestroyed;
				void OnStreamDestroyed_Handler(pj::OnStreamDestroyedParam &prm);

				GCHandle _gchOnDtmfDigit;
				void OnDtmfDigit_Handler(pj::OnDtmfDigitParam &prm);

				GCHandle _gchOnCallTransferRequest;
				void OnCallTransferRequest_Handler(pj::OnCallTransferRequestParam &prm);

				GCHandle _gchOnCallTransferStatus;
				void OnCallTransferStatus_Handler(pj::OnCallTransferStatusParam &prm);

				GCHandle _gchOnCallReplaceRequest;
				void OnCallReplaceRequest_Handler(pj::OnCallReplaceRequestParam &prm);

				GCHandle _gchOnCallReplaced;
				void OnCallReplaced_Handler(pj::OnCallReplacedParam &prm);

				GCHandle _gchOnCallRxOffer;
				void OnCallRxOffer_Handler(pj::OnCallRxOfferParam &prm);

				GCHandle _gchOnInstantMessage;
				void OnInstantMessage_Handler(pj::OnInstantMessageParam &prm);

				GCHandle _gchOnInstantMessageStatus;
				void OnInstantMessageStatus_Handler(pj::OnInstantMessageStatusParam &prm);

				GCHandle _gchOnTypingIndication;
				void OnTypingIndication_Handler(pj::OnTypingIndicationParam &prm);

				GCHandle _gchOnCallRedirected;
				pjsip_redirect_op OnCallRedirected_Handler(pj::OnCallRedirectedParam &prm);

				GCHandle _gchOnCallMediaTransportState;
				void OnCallMediaTransportState_Handler(pj::OnCallMediaTransportStateParam &prm);

				GCHandle _gchOnCallMediaEvent;
				void OnCallMediaEvent_Handler(pj::OnCallMediaEventParam &prm);

				GCHandle _gchOnCreateMediaTransport;
				void OnCreateMediaTransport_Handler(pj::OnCreateMediaTransportParam &prm);
			};
		}
	}
}
#endif