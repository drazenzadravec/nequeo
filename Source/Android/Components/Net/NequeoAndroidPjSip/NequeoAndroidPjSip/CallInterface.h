/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          CallInterface.h
*  Purpose :       SIP Call Interface class.
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

#include "AccountInterface.h"

#include <pjsua2\call.hpp>

namespace Nequeo {
	namespace Net {
		namespace Android {
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
				class CallInterface : public pj::Call
				{
				public:
					///	<summary>
					///	Call callbacks.
					///	</summary>
					/// <param name="account">The Sip account.</param>
					/// <param name="callID">An index call id (0 - 3).</param>
					CallInterface(AccountInterface& account, int callID);

					///	<summary>
					///	Call callbacks.
					///	</summary>
					virtual ~CallInterface();
				};
			}
		}
	}
}
