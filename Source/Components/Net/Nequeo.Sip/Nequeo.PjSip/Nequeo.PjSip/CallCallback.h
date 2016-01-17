/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallCallback.h
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

#pragma once

#ifndef _CALLCALLBACK_H
#define _CALLCALLBACK_H

#include "stdafx.h"

#include "AccountCallback.h"

#include "pjsua2\call.hpp"
#include "pjsua2\presence.hpp"
#include "pjsua2\account.hpp"
#include "pjsua2\endpoint.hpp"

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			typedef void(*OnCallState_Function)(pj::OnCallStateParam&);
			typedef void(*OnCallMediaState_Function)(pj::OnCallMediaStateParam&);
			typedef void(*OnCallTsxState_Function)(pj::OnCallTsxStateParam&);
			typedef void(*OnCallSdpCreated_Function)(pj::OnCallSdpCreatedParam&);
			typedef void(*OnStreamCreated_Function)(pj::OnStreamCreatedParam&);
			typedef void(*OnStreamDestroyed_Function)(pj::OnStreamDestroyedParam&);
			typedef void(*OnDtmfDigit_Function)(pj::OnDtmfDigitParam&);
			typedef void(*OnCallTransferRequest_Function)(pj::OnCallTransferRequestParam&);
			typedef void(*OnCallTransferStatus_Function)(pj::OnCallTransferStatusParam&);
			typedef void(*OnCallReplaceRequest_Function)(pj::OnCallReplaceRequestParam&);
			typedef void(*OnCallReplaced_Function)(pj::OnCallReplacedParam&);
			typedef void(*OnCallRxOffer_Function)(pj::OnCallRxOfferParam&);
			typedef void(*OnInstantMessage_Function)(pj::OnInstantMessageParam&);
			typedef void(*OnInstantMessageStatus_Function)(pj::OnInstantMessageStatusParam&);
			typedef void(*OnTypingIndication_Function)(pj::OnTypingIndicationParam&);
			typedef pjsip_redirect_op(*OnCallRedirected_Function)(pj::OnCallRedirectedParam&);
			typedef void(*OnCallMediaTransportState_Function)(pj::OnCallMediaTransportStateParam&);
			typedef void(*OnCallMediaEvent_Function)(pj::OnCallMediaEventParam&);
			typedef void(*OnCreateMediaTransport_Function)(pj::OnCreateMediaTransportParam&);

			///	<summary>
			///	Call callbacks.
			///	</summary>
			class CallCallback : public pj::Call
			{
			public:
				///	<summary>
				///	Call callbacks.
				///	</summary>
				/// <param name="account">The Sip account.</param>
				/// <param name="callId">An index call id (0 - 3).</param>
				CallCallback(AccountCallback& account, int callId);

				///	<summary>
				///	Call callbacks.
				///	</summary>
				virtual ~CallCallback();

				/// <summary>
				/// Notify application when call state has changed.
				/// Application may then query the call info to get the
				/// detail call states by calling getInfo() function.
				/// </summary>
				/// <param name="prm">Callback parameter.</param>
				void onCallState(pj::OnCallStateParam &prm);

				/// <summary>
				/// Notify application when media state in the call has changed.
				/// Normal application would need to implement this callback, e.g.
				/// to connect the call's media to sound device. When ICE is used,
				/// this callback will also be called to report ICE negotiation failure.
				/// </summary>
				/// <param name="prm">Callback parameter.</param>
				void onCallMediaState(pj::OnCallMediaStateParam &prm);

				/// <summary>
				/// This is a general notification callback which is called whenever
				/// a transaction within the call has changed state.Application can
				/// implement this callback for example to monitor the state of
				/// outgoing requests, or to answer unhandled incoming requests
				/// (such as INFO) with a final response.
				/// </summary>
				/// <param name="prm">Callback parameter.</param>
				void onCallTsxState(pj::OnCallTsxStateParam &prm);

				///	<summary>
				///	Set the on call state function callback.
				///	</summary>
				/// <param name="onCallMediaStateCallBack">The on call state function callback.</param>
				void Set_OnCallState_Function(OnCallState_Function onCallStateCallBack);

				///	<summary>
				///	Set the on call media state function callback.
				///	</summary>
				/// <param name="onCallMediaStateCallBack">The on call media state function callback.</param>
				void Set_OnCallMediaState_Function(OnCallMediaState_Function onCallMediaStateCallBack);

				///	<summary>
				///	Set the on call tsx state function callback.
				///	</summary>
				/// <param name="onCallTsxStateCallBack">The on call tsx state function callback.</param>
				void Set_OnCallTsxState_Function(OnCallTsxState_Function onCallTsxStateCallBack);

			private:
				bool _disposed;

				OnCallState_Function _onCallState_function_internal;
				OnCallMediaState_Function _onCallMediaState_function_internal;
				OnCallTsxState_Function _onCallTsxState_function_internal;
				OnCallSdpCreated_Function _onCallSdpCreated_function_internal;
				OnStreamCreated_Function _onStreamCreated_function_internal;
				OnStreamDestroyed_Function _onStreamDestroyed_function_internal;
				OnDtmfDigit_Function _onDtmfDigit_function_internal;
				OnCallTransferRequest_Function _onCallTransferRequest_function_internal;
				OnCallTransferStatus_Function _onCallTransferStatus_function_internal;
				OnCallReplaceRequest_Function _onCallReplaceRequest_function_internal;
				OnCallReplaced_Function _onCallReplaced_function_internal;
				OnCallRxOffer_Function _onCallRxOffer_function_internal;
				OnInstantMessage_Function _onInstantMessage_function_internal;
				OnInstantMessageStatus_Function _onInstantMessageStatus_function_internal;
				OnTypingIndication_Function _onTypingIndication_function_internal;
				OnCallRedirected_Function _onCallRedirected_function_internal;
				OnCallMediaTransportState_Function _onCallMediaTransportState_function_internal;
				OnCallMediaEvent_Function _onCallMediaEvent_function_internal;
				OnCreateMediaTransport_Function _onCreateMediaTransport_function_internal;
			};
		}
	}
}
#endif