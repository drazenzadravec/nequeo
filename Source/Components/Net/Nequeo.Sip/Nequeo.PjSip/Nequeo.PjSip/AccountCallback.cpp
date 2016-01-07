/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          AccountCallback.cpp
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

#include "stdafx.h"

#include "AccountCallback.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	Account callbacks.
///	</summary>
AccountCallback::AccountCallback() :
	_disposed(false)
{
	_endpoint = std::make_unique<pj::Endpoint>();
	_epConfig = std::make_unique<pj::EpConfig>();

	_accountConfig = std::make_unique<pj::AccountConfig>();
	_accountRegConfig = std::make_unique<pj::AccountRegConfig>();
	_accountSipConfig = std::make_unique<pj::AccountSipConfig>();
	_accountCallConfig = std::make_unique<pj::AccountCallConfig>();
	_accountMediaConfig = std::make_unique<pj::AccountMediaConfig>();
	_accountMwiConfig = std::make_unique<pj::AccountMwiConfig>();
	_accountPresConfig = std::make_unique<pj::AccountPresConfig>();
	_transportConfig = std::make_unique<pj::TransportConfig>();

	_transportConfig_UDP = std::make_unique<pj::TransportConfig>();
	_transportConfig_UDP6 = std::make_unique<pj::TransportConfig>();
	_transportConfig_TCP = std::make_unique<pj::TransportConfig>();
	_transportConfig_TCP6 = std::make_unique<pj::TransportConfig>();
}

///	<summary>
///	Account callbacks.
///	</summary>
AccountCallback::~AccountCallback()
{
	if (!_disposed)
	{
		_disposed = true;

		try
		{
			_endpoint->libStopWorkerThreads();
		}
		catch (const std::exception&) {}

		try
		{
			_endpoint->libDestroy();
		}
		catch (const std::exception&) {}
	}
}

///	<summary>
///	Initialise all setting.
///	</summary>
/// <param name="mapper">Account connection mapper.</param>
void AccountCallback::Initialise(ConnectionMapper& mapper)
{
	// Create endpoint data.
	_endpoint->libCreate();
	_endpoint->libInit(*(_epConfig.get()));

	// Create the client transport.
	_endpoint->transportCreate(pjsip_transport_type_e::PJSIP_TRANSPORT_UDP, *(_transportConfig_UDP.get()));
	_endpoint->transportCreate(pjsip_transport_type_e::PJSIP_TRANSPORT_UDP6, *(_transportConfig_UDP6.get()));
	_endpoint->transportCreate(pjsip_transport_type_e::PJSIP_TRANSPORT_TCP, *(_transportConfig_TCP.get()));
	_endpoint->transportCreate(pjsip_transport_type_e::PJSIP_TRANSPORT_TCP6, *(_transportConfig_TCP6.get()));

	// Start.
	_endpoint->libStart();

	// Set the account options.
	_accountConfig->idUri = "sip:" + mapper.GetAccountName() + "@" + mapper.GetSpHost() + ":" + std::to_string(mapper.GetSpPort());
	_accountConfig->priority = mapper.GetPriority();

	// Set the registration options.
	_accountRegConfig->registrarUri = "sip:" + mapper.GetSpHost() + ":" + std::to_string(mapper.GetSpPort());
	_accountRegConfig->dropCallsOnFail = mapper.GetDropCallsOnFail();
	_accountRegConfig->registerOnAdd = mapper.GetRegisterOnAdd();
	_accountRegConfig->retryIntervalSec = mapper.GetRetryIntervalSec();
	_accountRegConfig->timeoutSec = mapper.GetTimeoutSec();
	_accountRegConfig->firstRetryIntervalSec = mapper.GetFirstRetryIntervalSec();
	_accountRegConfig->unregWaitMsec = mapper.GetUnregWaitSec();
	_accountRegConfig->delayBeforeRefreshSec = mapper.GetDelayBeforeRefreshSec();

	// Set the media options.
	_transportConfig->port = mapper.GetMediaTransportPort();
	_transportConfig->portRange = mapper.GetMediaTransportPortRange();
	_accountMediaConfig->ipv6Use = mapper.GetIPv6UseEx(mapper.GetIPv6Use());
	_accountMediaConfig->srtpUse = mapper.GetSrtpUseEx(mapper.GetSRTPUse());
	_accountMediaConfig->srtpSecureSignaling = mapper.GetSRTPSecureSignalingEx(mapper.GetSRTPSecureSignaling());
	_accountMediaConfig->transportConfig = *(_transportConfig.get());

	// Set the sip options.
	_accountSipConfig->authCreds = mapper.GetAuthCredentials();

	// Set the call options.
	_accountCallConfig->holdType = pjsua_call_hold_type::PJSUA_CALL_HOLD_TYPE_RFC3264;
	_accountCallConfig->prackUse = pjsua_100rel_use::PJSUA_100REL_NOT_USED;
	_accountCallConfig->timerUse = pjsua_sip_timer_use::PJSUA_SIP_TIMER_OPTIONAL;
	_accountCallConfig->timerMinSESec = mapper.GetTimerMinSESec();
	_accountCallConfig->timerSessExpiresSec = mapper.GetTimerSessExpiresSec();

	// Set the message waiting indicatoin options.
	_accountMwiConfig->enabled = mapper.GetMessageWaitingIndication();
	_accountMwiConfig->expirationSec = mapper.GetMWIExpirationSec();

	// Set the presence options.
	_accountPresConfig->publishEnabled = mapper.GetPublishEnabled();
	_accountPresConfig->publishQueue = mapper.GetPublishQueue();
	_accountPresConfig->publishShutdownWaitMsec = mapper.GetPublishShutdownWaitMsec();

	// Assign the account config.
	_accountConfig->regConfig = *(_accountRegConfig.get());
	_accountConfig->sipConfig = *(_accountSipConfig.get());
	_accountConfig->callConfig = *(_accountCallConfig.get());
	_accountConfig->mediaConfig = *(_accountMediaConfig.get());
	_accountConfig->mwiConfig = *(_accountMwiConfig.get());
	_accountConfig->presConfig = *(_accountPresConfig.get());

	// Create the account.
	create(*(_accountConfig.get()), true);
}

/// <summary>
/// Get the audio deveice manager.
/// </summary>
/// <returns>The audio device manager.</returns>
pj::AudDevManager& AccountCallback::GetAudDevManager()
{
	return _endpoint->audDevManager();
}

/// <summary>
/// Get the number of active media ports.
/// </summary>
/// <returns>The number of active ports.</returns>
unsigned AccountCallback::MediaActivePorts()
{
	return _endpoint->mediaActivePorts();
}

/// <summary>
/// Get all supported codecs in the system.
/// </summary>
/// <returns>The supported codecs in the system.</returns>
const pj::CodecInfoVector& AccountCallback::GetCodecInfo()
{
	return _endpoint->codecEnum();
}

/// <summary>
/// Add audio media device to the application.
/// </summary>
/// <param name="audioMedia">The audio media device.</param>
void AccountCallback::AddAudioMedia(pj::AudioMedia& audioMedia)
{
	_endpoint->mediaAdd(audioMedia);
}

///	<summary>
///	Set the on incoming call function callback.
///	</summary>
/// <param name="onIncomingCallBack">The on incoming call function callback.</param>
void AccountCallback::Set_OnIncomingCall_Function(OnIncomingCall_Function onIncomingCallBack)
{
	_onIncomingCall_function_internal = onIncomingCallBack;
}

///	<summary>
///	Set the on Incoming Subscribe function callback.
///	</summary>
/// <param name="onIncomingSubscribeCallBack">The on Incoming Subscribe function callback.</param>
void AccountCallback::Set_OnIncomingSubscribe_Function(OnIncomingSubscribe_Function onIncomingSubscribeCallBack)
{
	_onIncomingSubscribe_function_internal = onIncomingSubscribeCallBack;
}

///	<summary>
///	Set the on Instant Message function callback.
///	</summary>
/// <param name="onInstantMessageCallBack">The on Instant Message function callback.</param>
void AccountCallback::Set_OnInstantMessage_Function(OnInstantMessage_Function onInstantMessageCallBack)
{
	_onInstantMessage_function_internal = onInstantMessageCallBack;
}

///	<summary>
///	Set the on Instant Message Status function callback.
///	</summary>
/// <param name="onInstantMessageStatusCallBack">The on Instant Message Status function callback.</param>
void AccountCallback::Set_OnInstantMessageStatus_Function(OnInstantMessageStatus_Function onInstantMessageStatusCallBack)
{
	_onInstantMessageStatus_function_internal = onInstantMessageStatusCallBack;
}

///	<summary>
///	Set the on Mwi Info function callback.
///	</summary>
/// <param name="onMwiInfoCallBack">The on Mwi Info function callback.</param>
void AccountCallback::Set_OnMwiInfo_Function(OnMwiInfo_Function onMwiInfoCallBack)
{
	_onMwiInfo_function_internal = onMwiInfoCallBack;
}

///	<summary>
///	Set the on Typing Indication function callback.
///	</summary>
/// <param name="onTypingIndicationCallBack">The on Typing Indication function callback.</param>
void AccountCallback::Set_OnTypingIndication_Function(OnTypingIndication_Function onTypingIndicationCallBack)
{
	_onTypingIndication_function_internal = onTypingIndicationCallBack;
}

///	<summary>
///	Set the on Reg Started function callback.
///	</summary>
/// <param name="onRegStartedCallBack">The on Reg Started function callback.</param>
void AccountCallback::Set_OnRegStarted_Function(OnRegStarted_Function onRegStartedCallBack)
{
	_onRegStarted_function_internal = onRegStartedCallBack;
}

///	<summary>
///	Set the on Reg State function callback.
///	</summary>
/// <param name="onRegStateCallBack">The on Reg State function callback.</param>
void AccountCallback::Set_OnRegState_Function(OnRegState_Function onRegStateCallBack)
{
	_onRegState_function_internal = onRegStateCallBack;
}

///	<summary>
///	Notify application on incoming call.
///	</summary>
/// <param name="prm">Callback parameter.</param>
void AccountCallback::onIncomingCall(pj::OnIncomingCallParam &prm)
{
	_onIncomingCall_function_internal(prm);
}

/// <summary>
/// Notify application when registration or unregistration has been
/// initiated. Note that this only notifies the initial registration
/// and unregistration. Once registration session is active, subsequent
/// refresh will not cause this callback to be called.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void AccountCallback::onRegStarted(pj::OnRegStartedParam &prm)
{
	_onRegStarted_function_internal(prm);
}

/// <summary>
/// Notify application when registration status has changed.
/// Application may then query the account info to get the
/// registration details.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void AccountCallback::onRegState(pj::OnRegStateParam &prm)
{
	_onRegState_function_internal(prm);
}

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
void AccountCallback::onIncomingSubscribe(pj::OnIncomingSubscribeParam &prm)
{
	_onIncomingSubscribe_function_internal(prm);
}

/// <summary>
/// Notify application on incoming instant message or pager (i.e. MESSAGE
/// request) that was received outside call context.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void AccountCallback::onInstantMessage(pj::OnInstantMessageParam &prm)
{
	_onInstantMessage_function_internal(prm);
}

/// <summary>
/// Notify application about the delivery status of outgoing pager/instant
/// message(i.e.MESSAGE) request.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void AccountCallback::onInstantMessageStatus(pj::OnInstantMessageStatusParam &prm)
{
	_onInstantMessageStatus_function_internal(prm);
}

/// <summary>
/// Notify application about typing indication.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void AccountCallback::onTypingIndication(pj::OnTypingIndicationParam &prm)
{
	_onTypingIndication_function_internal(prm);
}

/// <summary>
/// Notification about MWI (Message Waiting Indication) status change.
/// This callback can be called upon the status change of the
/// SUBSCRIBE request(for example, 202/Accepted to SUBSCRIBE is received)
/// or when a NOTIFY reqeust is received.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void AccountCallback::onMwiInfo(pj::OnMwiInfoParam &prm)
{
	_onMwiInfo_function_internal(prm);
}