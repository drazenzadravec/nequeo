/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallCallback.cpp
*  Purpose :       SIP Call Callback class.
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

#include "CallCallback.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	Call callbacks.
///	</summary>
/// <param name="account">The Sip account.</param>
/// <param name="callId">An index call id (0 - 3).</param>
CallCallback::CallCallback(AccountCallback& account, int callId) : pj::Call(account, callId),
	_disposed(false)
{
}

///	<summary>
///	Contact callbacks.
///	</summary>
CallCallback::~CallCallback()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Notify application when call state has changed.
/// Application may then query the call info to get the
/// detail call states by calling getInfo() function.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void CallCallback::onCallState(pj::OnCallStateParam &prm)
{
	_onCallState_function_internal(prm);
}

/// <summary>
/// Notify application when media state in the call has changed.
/// Normal application would need to implement this callback, e.g.
/// to connect the call's media to sound device. When ICE is used,
/// this callback will also be called to report ICE negotiation failure.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void CallCallback::onCallMediaState(pj::OnCallMediaStateParam &prm)
{
	_onCallMediaState_function_internal(prm);
}

/// <summary>
/// This is a general notification callback which is called whenever
/// a transaction within the call has changed state.Application can
/// implement this callback for example to monitor the state of
/// outgoing requests, or to answer unhandled incoming requests
/// (such as INFO) with a final response.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void CallCallback::onCallTsxState(pj::OnCallTsxStateParam &prm)
{
	_onCallTsxState_function_internal(prm);
}

/// <summary>
/// Notify application when a call has just created a local SDP (for
/// initial or subsequent SDP offer / answer).Application can implement
/// this callback to modify the SDP, before it is being sent and / or
/// negotiated with remote SDP, for example to apply per account / call
/// basis codecs priority or to add custom / proprietary SDP attributes.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void CallCallback::onCallSdpCreated(pj::OnCallSdpCreatedParam &prm)
{
	_onCallSdpCreated_function_internal(prm);
}

/// <summary>
/// Notify application when media session is created and before it is
/// registered to the conference bridge.Application may return different
/// media port if it has added media processing port to the stream.This
/// media port then will be added to the conference bridge instead.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void CallCallback::onStreamCreated(pj::OnStreamCreatedParam &prm)
{
	_onStreamCreated_function_internal(prm);
}

/// <summary>
/// Notify application when media session has been unregistered from the
/// conference bridge and about to be destroyed.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void CallCallback::onStreamDestroyed(pj::OnStreamDestroyedParam &prm)
{
	_onStreamDestroyed_function_internal(prm);
}

/// <summary>
/// Notify application upon incoming DTMF digits.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void CallCallback::onDtmfDigit(pj::OnDtmfDigitParam &prm)
{
	_onDtmfDigit_function_internal(prm);
}

/// <summary>
/// Notify application on call being transferred (i.e. REFER is received).
/// Application can decide to accept / reject transfer request
/// by setting the code(default is 202).When this callback
/// is not implemented, the default behavior is to accept the
/// transfer.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void CallCallback::onCallTransferRequest(pj::OnCallTransferRequestParam &prm)
{
	_onCallTransferRequest_function_internal(prm);
}

/// <summary>
/// Notify application of the status of previously sent call
/// transfer request.Application can monitor the status of the
/// call transfer request, for example to decide whether to
/// terminate existing call.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void CallCallback::onCallTransferStatus(pj::OnCallTransferStatusParam &prm)
{
	_onCallTransferStatus_function_internal(prm);
}

/// <summary>
/// Notify application about incoming INVITE with Replaces header.
/// Application may reject the request by setting non - 2xx code.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void CallCallback::onCallReplaceRequest(pj::OnCallReplaceRequestParam &prm)
{
	_onCallReplaceRequest_function_internal(prm);
}

/// <summary>
/// Notify application that an existing call has been replaced with
/// a new call.This happens when PJSUA - API receives incoming INVITE
/// request with Replaces header.
/// After this callback is called, normally PJSUA - API will disconnect
/// this call and establish a new call newCallId.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void CallCallback::onCallReplaced(pj::OnCallReplacedParam &prm)
{
	_onCallReplaced_function_internal(prm);
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
void CallCallback::onCallRxOffer(pj::OnCallRxOfferParam &prm)
{
	_onCallRxOffer_function_internal(prm);
}

/// <summary>
/// Notify application on incoming MESSAGE request.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void CallCallback::onInstantMessage(pj::OnInstantMessageParam &prm)
{
	_onInstantMessage_function_internal(prm);
}

/// <summary>
/// Notify application about the delivery status of outgoing MESSAGE request.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void CallCallback::onInstantMessageStatus(pj::OnInstantMessageStatusParam &prm)
{
	_onInstantMessageStatus_function_internal(prm);
}

/// <summary>
/// Notify application about typing indication.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void CallCallback::onTypingIndication(pj::OnTypingIndicationParam &prm)
{
	_onTypingIndication_function_internal(prm);
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
pjsip_redirect_op CallCallback::onCallRedirected(pj::OnCallRedirectedParam &prm)
{
	return _onCallRedirected_function_internal(prm);
}

/// <summary>
/// This callback is called when media transport state is changed.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void CallCallback::onCallMediaTransportState(pj::OnCallMediaTransportStateParam &prm)
{
	_onCallMediaTransportState_function_internal(prm);
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
void CallCallback::onCallMediaEvent(pj::OnCallMediaEventParam &prm)
{
	_onCallMediaEvent_function_internal(prm);
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
void CallCallback::onCreateMediaTransport(pj::OnCreateMediaTransportParam &prm)
{
	_onCreateMediaTransport_function_internal(prm);
}

///	<summary>
///	Set the on call state function callback.
///	</summary>
/// <param name="onCallStateCallBack">The on call state function callback.</param>
void CallCallback::Set_OnCallState_Function(OnCallState_Function onCallStateCallBack)
{
	_onCallState_function_internal = onCallStateCallBack;
}

///	<summary>
///	Set the on call media state function callback.
///	</summary>
/// <param name="onCallMediaStateCallBack">The on call media state function callback.</param>
void CallCallback::Set_OnCallMediaState_Function(OnCallMediaState_Function onCallMediaStateCallBack)
{
	_onCallMediaState_function_internal = onCallMediaStateCallBack;
}

///	<summary>
///	Set the on call tsx state function callback.
///	</summary>
/// <param name="onCallTsxStateCallBack">The on call tsx state function callback.</param>
void CallCallback::Set_OnCallTsxState_Function(OnCallTsxState_Function onCallTsxStateCallBack)
{
	_onCallTsxState_function_internal = onCallTsxStateCallBack;
}

///	<summary>
///	Set the CallSdpCreated function callback.
///	</summary>
/// <param name="onCallSdpCreatedCallBack">The CallSdpCreated function callback.</param>
void CallCallback::Set_OnCallSdpCreated_Function(OnCallSdpCreated_Function onCallSdpCreatedCallBack)
{
	_onCallSdpCreated_function_internal = onCallSdpCreatedCallBack;
}

///	<summary>
///	Set the StreamCreated function callback.
///	</summary>
/// <param name="onStreamCreatedCallBack">The StreamCreated function callback.</param>
void CallCallback::Set_OnStreamCreated_Function(OnStreamCreated_Function onStreamCreatedCallBack)
{
	_onStreamCreated_function_internal = onStreamCreatedCallBack;
}

///	<summary>
///	Set the StreamDestroyed function callback.
///	</summary>
/// <param name="onStreamDestroyedCallBack">The StreamDestroyed function callback.</param>
void CallCallback::Set_OnStreamDestroyed_Function(OnStreamDestroyed_Function onStreamDestroyedCallBack)
{
	_onStreamDestroyed_function_internal = onStreamDestroyedCallBack;
}

///	<summary>
///	Set the DtmfDigit function callback.
///	</summary>
/// <param name="onDtmfDigitCallBack">The DtmfDigit function callback.</param>
void CallCallback::Set_OnDtmfDigit_Function(OnDtmfDigit_Function onDtmfDigitCallBack)
{
	_onDtmfDigit_function_internal = onDtmfDigitCallBack;
}

///	<summary>
///	Set the CallTransferRequest function callback.
///	</summary>
/// <param name="onCallTransferRequestCallBack">The CallTransferRequest function callback.</param>
void CallCallback::Set_OnCallTransferRequest_Function(OnCallTransferRequest_Function onCallTransferRequestCallBack)
{
	_onCallTransferRequest_function_internal = onCallTransferRequestCallBack;
}

///	<summary>
///	Set the CallTransferStatus function callback.
///	</summary>
/// <param name="onCallTransferStatusCallBack">The CallTransferStatus function callback.</param>
void CallCallback::Set_OnCallTransferStatus_Function(OnCallTransferStatus_Function onCallTransferStatusCallBack)
{
	_onCallTransferStatus_function_internal = onCallTransferStatusCallBack;
}

///	<summary>
///	Set the CallReplaceRequest function callback.
///	</summary>
/// <param name="onCallReplaceRequestCallBack">The CallReplaceRequest function callback.</param>
void CallCallback::Set_OnCallReplaceRequest_Function(OnCallReplaceRequest_Function onCallReplaceRequestCallBack)
{
	_onCallReplaceRequest_function_internal = onCallReplaceRequestCallBack;
}

///	<summary>
///	Set the CallReplaced function callback.
///	</summary>
/// <param name="onCallReplacedCallBack">The CallReplaced function callback.</param>
void CallCallback::Set_OnCallReplaced_Function(OnCallReplaced_Function onCallReplacedCallBack)
{
	_onCallReplaced_function_internal = onCallReplacedCallBack;
}

///	<summary>
///	Set the CallRxOffer function callback.
///	</summary>
/// <param name="onCallRxOfferCallBack">The CallRxOffer function callback.</param>
void CallCallback::Set_OnCallRxOffer_Function(OnCallRxOffer_Function onCallRxOfferCallBack)
{
	_onCallRxOffer_function_internal = onCallRxOfferCallBack;
}

///	<summary>
///	Set the InstantMessage function callback.
///	</summary>
/// <param name="onInstantMessageCallBack">The InstantMessage function callback.</param>
void CallCallback::Set_OnInstantMessage_Function(OnInstantMessage_Function onInstantMessageCallBack)
{
	_onInstantMessage_function_internal = onInstantMessageCallBack;
}

///	<summary>
///	Set the InstantMessageStatus function callback.
///	</summary>
/// <param name="onInstantMessageStatusCallBack">The InstantMessageStatus function callback.</param>
void CallCallback::Set_OnInstantMessageStatus_Function(OnInstantMessageStatus_Function onInstantMessageStatusCallBack)
{
	_onInstantMessageStatus_function_internal = onInstantMessageStatusCallBack;
}

///	<summary>
///	Set the TypingIndication function callback.
///	</summary>
/// <param name="onTypingIndicationCallBack">The TypingIndication function callback.</param>
void CallCallback::Set_OnTypingIndication_Function(OnTypingIndication_Function onTypingIndicationCallBack)
{
	_onTypingIndication_function_internal = onTypingIndicationCallBack;
}

///	<summary>
///	Set the CallRedirected function callback.
///	</summary>
/// <param name="onCallRedirectedCallBack">The CallRedirected function callback.</param>
void CallCallback::Set_OnCallRedirected_Function(OnCallRedirected_Function onCallRedirectedCallBack)
{
	_onCallRedirected_function_internal = onCallRedirectedCallBack;
}

///	<summary>
///	Set the CallMediaTransportState function callback.
///	</summary>
/// <param name="onCallMediaTransportStateCallBack">The CallMediaTransportState function callback.</param>
void CallCallback::Set_OnCallMediaTransportState_Function(OnCallMediaTransportState_Function onCallMediaTransportStateCallBack)
{
	_onCallMediaTransportState_function_internal = onCallMediaTransportStateCallBack;
}

///	<summary>
///	Set the CallMediaEvent function callback.
///	</summary>
/// <param name="onCallMediaEventCallBack">The CallMediaEvent function callback.</param>
void CallCallback::Set_OnCallMediaEvent_Function(OnCallMediaEvent_Function onCallMediaEventCallBack)
{
	_onCallMediaEvent_function_internal = onCallMediaEventCallBack;
}

///	<summary>
///	Set the CreateMediaTransport function callback.
///	</summary>
/// <param name="onCreateMediaTransportCallBack">The CreateMediaTransport function callback.</param>
void CallCallback::Set_OnCreateMediaTransport_Function(OnCreateMediaTransport_Function onCreateMediaTransportCallBack)
{
	_onCreateMediaTransport_function_internal = onCreateMediaTransportCallBack;
}