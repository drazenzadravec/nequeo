/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          AccountCallback.h
*  Purpose :       SIP Account Callback class.
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

#ifndef _ACCOUNTCALLBACK_H
#define _ACCOUNTCALLBACK_H

#include "stdafx.h"

#include "ConnectionMapper.h"
#include "TransportType.h"

#include "pjsua2\account.hpp"
#include "pjsua2\endpoint.hpp"

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			typedef void(*OnIncomingCall_Function)(pj::OnIncomingCallParam&);
			typedef void(*OnIncomingSubscribe_Function)(pj::OnIncomingSubscribeParam&);
			typedef void(*OnInstantMessage_Function)(pj::OnInstantMessageParam&);
			typedef void(*OnInstantMessageStatus_Function)(pj::OnInstantMessageStatusParam&);
			typedef void(*OnMwiInfo_Function)(pj::OnMwiInfoParam&);
			typedef void(*OnTypingIndication_Function)(pj::OnTypingIndicationParam&);
			typedef void(*OnRegStarted_Function)(pj::OnRegStartedParam&);
			typedef void(*OnRegState_Function)(pj::OnRegStateParam&);

			///	<summary>
			///	Account callbacks.
			///	</summary>
			class AccountCallback : public pj::Account
			{
			public:
				///	<summary>
				///	Account callbacks.
				///	</summary>
				AccountCallback();

				///	<summary>
				///	Account callbacks.
				///	</summary>
				virtual ~AccountCallback();

				///	<summary>
				///	Initialise all setting.
				///	</summary>
				/// <param name="mapper">Account connection mapper.</param>
				void Initialise(ConnectionMapper& mapper);

				/// <summary>
				/// Get the account video configration.
				/// </summary>
				/// <returns>The account video configuration.</returns>
				pj::AccountVideoConfig& GetAccountVideoConfig();

				///	<summary>
				///	Notify application on incoming call.
				///	</summary>
				/// <param name="prm">Callback parameter.</param>
				void onIncomingCall(pj::OnIncomingCallParam &prm);

				/// <summary>
				/// Notify application when registration or unregistration has been
				/// initiated. Note that this only notifies the initial registration
				/// and unregistration. Once registration session is active, subsequent
				/// refresh will not cause this callback to be called.
				/// </summary>
				/// <param name="prm">Callback parameter.</param>
				void onRegStarted(pj::OnRegStartedParam &prm);

				/// <summary>
				/// Notify application when registration status has changed.
				/// Application may then query the account info to get the
				/// registration details.
				/// </summary>
				/// <param name="prm">Callback parameter.</param>
				void onRegState(pj::OnRegStateParam &prm);

				/// <summary>
				/// Notification when incoming SUBSCRIBE request is received. Application
				/// may use this callback to authorize the incoming subscribe request
				/// (e.g.ask user permission if the request should be granted).
				///
				/// If this callback is not implemented, all incoming presence subscription
				/// requests will be accepted.
				///
				/// If this callback is implemented, application has several choices on
				/// what to do with the incoming request:
				///	- it may reject the request immediately by specifying non-200 class
				///    final response in the IncomingSubscribeParam.code parameter.
				///  - it may immediately accept the request by specifying 200 as the
				/// IncomingSubscribeParam.code parameter. This is the default value if
				///	  application doesn't set any value to the IncomingSubscribeParam.code
				///	  parameter.In this case, the library will automatically send NOTIFY
				///	  request upon returning from this callback.
				///  - it may delay the processing of the request, for example to request
				/// user permission whether to accept or reject the request. In this
				///    case, the application MUST set the IncomingSubscribeParam.code
				/// argument to 202, then IMMEDIATELY calls presNotify() with
				/// state PJSIP_EVSUB_STATE_PENDING and later calls presNotify()
				///    again to accept or reject the subscription request.
				///
				/// Any IncomingSubscribeParam.code other than 200 and 202 will be treated
				/// as 200.
				///
				/// Application MUST return from this callback immediately (e.g.it must
				/// not block in this callback while waiting for user confirmation).
				/// </summary>
				/// <param name="prm">Callback parameter.</param>
				void onIncomingSubscribe(pj::OnIncomingSubscribeParam &prm);

				/// <summary>
				/// Notify application on incoming instant message or pager (i.e. MESSAGE
				/// request) that was received outside call context.
				/// </summary>
				/// <param name="prm">Callback parameter.</param>
				void onInstantMessage(pj::OnInstantMessageParam &prm);

				/// <summary>
				/// Notify application about the delivery status of outgoing pager/instant
				/// message(i.e.MESSAGE) request.
				/// </summary>
				/// <param name="prm">Callback parameter.</param>
				void onInstantMessageStatus(pj::OnInstantMessageStatusParam &prm);

				/// <summary>
				/// Notify application about typing indication.
				/// </summary>
				/// <param name="prm">Callback parameter.</param>
				void onTypingIndication(pj::OnTypingIndicationParam &prm);

				/// <summary>
				/// Notification about MWI (Message Waiting Indication) status change.
				/// This callback can be called upon the status change of the
				/// SUBSCRIBE request(for example, 202/Accepted to SUBSCRIBE is received)
				/// or when a NOTIFY reqeust is received.
				/// </summary>
				/// <param name="prm">Callback parameter.</param>
				void onMwiInfo(pj::OnMwiInfoParam &prm);

				///	<summary>
				///	Set the on incoming call function callback.
				///	</summary>
				/// <param name="onIncomingCallBack">The on incoming call function callback.</param>
				void Set_OnIncomingCall_Function(OnIncomingCall_Function onIncomingCallBack);

				///	<summary>
				///	Set the on Incoming Subscribe function callback.
				///	</summary>
				/// <param name="onIncomingSubscribeCallBack">The on Incoming Subscribe function callback.</param>
				void Set_OnIncomingSubscribe_Function(OnIncomingSubscribe_Function onIncomingSubscribeCallBack);

				///	<summary>
				///	Set the on Instant Message function callback.
				///	</summary>
				/// <param name="onInstantMessageCallBack">The on Instant Message function callback.</param>
				void Set_OnInstantMessage_Function(OnInstantMessage_Function onInstantMessageCallBack);

				///	<summary>
				///	Set the on Instant Message Status function callback.
				///	</summary>
				/// <param name="onInstantMessageStatusCallBack">The on Instant Message Status function callback.</param>
				void Set_OnInstantMessageStatus_Function(OnInstantMessageStatus_Function onInstantMessageStatusCallBack);

				///	<summary>
				///	Set the on Mwi Info function callback.
				///	</summary>
				/// <param name="onMwiInfoCallBack">The on Mwi Info function callback.</param>
				void Set_OnMwiInfo_Function(OnMwiInfo_Function onMwiInfoCallBack);

				///	<summary>
				///	Set the on Typing Indication function callback.
				///	</summary>
				/// <param name="onTypingIndicationCallBack">The on Typing Indication function callback.</param>
				void Set_OnTypingIndication_Function(OnTypingIndication_Function onTypingIndicationCallBack);

				///	<summary>
				///	Set the on Reg Started function callback.
				///	</summary>
				/// <param name="onRegStartedCallBack">The on Reg Started function callback.</param>
				void Set_OnRegStarted_Function(OnRegStarted_Function onRegStartedCallBack);

				///	<summary>
				///	Set the on Reg State function callback.
				///	</summary>
				/// <param name="onRegStateCallBack">The on Reg State function callback.</param>
				void Set_OnRegState_Function(OnRegState_Function onRegStateCallBack);

			private:
				bool _disposed;

				std::unique_ptr<pj::AccountConfig> _accountConfig;
				std::unique_ptr<pj::AccountRegConfig> _accountRegConfig;
				std::unique_ptr<pj::AccountSipConfig> _accountSipConfig;
				std::unique_ptr<pj::AccountCallConfig> _accountCallConfig;
				std::unique_ptr<pj::AccountMediaConfig> _accountMediaConfig;
				std::unique_ptr<pj::AccountMwiConfig> _accountMwiConfig;
				std::unique_ptr<pj::AccountPresConfig> _accountPresConfig;
				std::unique_ptr<pj::TransportConfig> _transportConfig;
				std::unique_ptr<pj::AccountNatConfig> _accountNatConfig;
				std::unique_ptr<pj::AccountVideoConfig> _accountVideoConfig;

				OnIncomingCall_Function _onIncomingCall_function_internal;
				OnIncomingSubscribe_Function _onIncomingSubscribe_function_internal;
				OnInstantMessage_Function _onInstantMessage_function_internal;
				OnInstantMessageStatus_Function _onInstantMessageStatus_function_internal;
				OnMwiInfo_Function _onMwiInfo_function_internal;
				OnTypingIndication_Function _onTypingIndication_function_internal;
				OnRegStarted_Function _onRegStarted_function_internal;
				OnRegState_Function _onRegState_function_internal;

			};
		}
	}
}
#endif