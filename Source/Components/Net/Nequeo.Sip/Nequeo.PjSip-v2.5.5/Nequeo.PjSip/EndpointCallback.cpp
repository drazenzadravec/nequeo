/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          EndpointCallback.cpp
*  Purpose :       SIP Endpoint Callback class.
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

#include "EndpointCallback.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	Endpoint callbacks.
///	</summary>
EndpointCallback::EndpointCallback() :
	_disposed(false), _created(false)
{
	_epConfig = std::make_unique<pj::EpConfig>();

	_transportConfig_UDP = std::make_unique<pj::TransportConfig>();
	_transportConfig_UDP6 = std::make_unique<pj::TransportConfig>();
	_transportConfig_TCP = std::make_unique<pj::TransportConfig>();
	_transportConfig_TCP6 = std::make_unique<pj::TransportConfig>();
	_transportConfig_TLS = std::make_unique<pj::TransportConfig>();
	_transportConfig_TLS6 = std::make_unique<pj::TransportConfig>();
}

///	<summary>
///	Endpoint callbacks.
///	</summary>
EndpointCallback::~EndpointCallback()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Start the application.
/// </summary>
/// <param name="useIPv6">Use IPv6.</param>
/// <param name="transportType">The transport type flags.</param>
void EndpointCallback::Initialise(pjsua_ipv6_use useIPv6, TransportType transportType)
{
	// If not created.
	if (!_created)
	{
		// Create endpoint data.
		this->libCreate();
		this->libInit(*(_epConfig.get()));

		// Setup TLS.
		_transportConfig_TLS->tlsConfig.method = pjsip_ssl_method::PJSIP_TLSV1_2_METHOD;
		_transportConfig_TLS->tlsConfig.verifyServer = false;
		_transportConfig_TLS->tlsConfig.verifyClient = false;

		// If IPv6 is enabled.
		if (useIPv6 == pjsua_ipv6_use::PJSUA_IPV6_ENABLED)
		{
			_transportConfig_TLS6->tlsConfig.method = pjsip_ssl_method::PJSIP_TLSV1_2_METHOD;
			_transportConfig_TLS6->tlsConfig.verifyServer = false;
			_transportConfig_TLS6->tlsConfig.verifyClient = false;
		}

		// If UDP transport.
		if ((TransportType::UDP & transportType) == TransportType::UDP)
		{
			// Create the client transport.
			int traID_UDP = this->transportCreate(pjsip_transport_type_e::PJSIP_TRANSPORT_UDP, *(_transportConfig_UDP.get()));
			_transportIDs.push_back(traID_UDP);

			// If IPv6 is enabled.
			if (useIPv6 == pjsua_ipv6_use::PJSUA_IPV6_ENABLED)
			{
				int traID_UDP6 = this->transportCreate(pjsip_transport_type_e::PJSIP_TRANSPORT_UDP6, *(_transportConfig_UDP6.get()));
				_transportIDs.push_back(traID_UDP6);
			}
		}

		// If TCP transport.
		if ((TransportType::TCP & transportType) == TransportType::TCP)
		{
			int traID_TCP = this->transportCreate(pjsip_transport_type_e::PJSIP_TRANSPORT_TCP, *(_transportConfig_TCP.get()));
			_transportIDs.push_back(traID_TCP);

			// If IPv6 is enabled.
			if (useIPv6 == pjsua_ipv6_use::PJSUA_IPV6_ENABLED)
			{
				int traID_TCP6 = this->transportCreate(pjsip_transport_type_e::PJSIP_TRANSPORT_TCP6, *(_transportConfig_TCP6.get()));
				_transportIDs.push_back(traID_TCP6);
			}
		}

		// If TLS transport.
		if ((TransportType::TLS & transportType) == TransportType::TLS)
		{
			// Has not been implemented must change pjlib.config.PJ_HAS_SSL_SOCK = 1
			// then recompile the pjproject and copy all the libs then then recomiple this project.
			int traID_TLS = this->transportCreate(pjsip_transport_type_e::PJSIP_TRANSPORT_TLS, *(_transportConfig_TLS.get()));
			_transportIDs.push_back(traID_TLS);

			// If IPv6 is enabled.
			if (useIPv6 == pjsua_ipv6_use::PJSUA_IPV6_ENABLED)
			{
				int traID_TLS6 = this->transportCreate(pjsip_transport_type_e::PJSIP_TRANSPORT_TLS6, *(_transportConfig_TLS6.get()));
				_transportIDs.push_back(traID_TLS6);
			}
		}

		// Start the end point.
		this->libStart();
		_created = true;
	}
}

/// <summary>
/// Get the list of transport ids.
/// </summary>
/// <returns>The list of transport ids.</returns>
const std::vector<int> EndpointCallback::GetTransportIdList()
{
	return _transportIDs;
}

/// <summary>
/// Get the audio deveice manager.
/// </summary>
/// <returns>The audio device manager.</returns>
pj::AudDevManager& EndpointCallback::GetAudioDevManager()
{
	return this->audDevManager();
}

/// <summary>
/// Get the video deveice manager.
/// </summary>
/// <returns>The video device manager.</returns>
pj::VidDevManager& EndpointCallback::GetVideoDevManager()
{
	return this->vidDevManager();
}

/// <summary>
/// Get the number of active media ports.
/// </summary>
/// <returns>The number of active ports.</returns>
unsigned EndpointCallback::MediaActivePorts()
{
	return this->mediaActivePorts();
}

/// <summary>
/// Get all supported audio codecs in the system.
/// </summary>
/// <returns>The supported audio codecs in the system.</returns>
const pj::CodecInfoVector& EndpointCallback::GetAudioCodecInfo()
{
	return this->codecEnum();
}

/// <summary>
/// Get all supported video codecs in the system.
/// </summary>
/// <returns>The supported video codecs in the system.</returns>
const pj::CodecInfoVector& EndpointCallback::GetVideoCodecInfo()
{
	return this->videoCodecEnum();
}

///	<summary>
///	Change audio codec priority.
///	</summary>
/// <param name="codecID">which is a string that uniquely identify
///	the codec(such as "speex/8000").</param>
/// <param name="priority">Codec priority, 0-255, where zero means to disable
///	the codec.</param>
void EndpointCallback::SetPriorityAudioCodec(const std::string &codecID, unsigned char priority)
{
	this->codecSetPriority(codecID, priority);
}

///	<summary>
///	Change video codec priority.
///	</summary>
/// <param name="codecID">Codec ID, which is a string that uniquely identify
///	the codec(such as "H263/90000"). Please see pjsua
/// manual or pjmedia codec reference for details.</param>
/// <param name="priority">Codec priority, 0-255, where zero means to disable
///	the codec.</param>
void EndpointCallback::SetPriorityVideoCodec(const std::string &codecID, unsigned char priority)
{
	this->videoCodecSetPriority(codecID, priority);
}

/// <summary>
/// Add audio media device to the application.
/// </summary>
/// <param name="audioMedia">The audio media device.</param>
void EndpointCallback::AddAudioMedia(pj::AudioMedia& audioMedia)
{
	this->mediaAdd(audioMedia);
}

///	<summary>
///	Callback when the Endpoint has finished performing NAT type
/// detection that is initiated with natDetectType().
///	</summary>
/// <param name="prm">Callback parameter.</param>
void EndpointCallback::onNatDetectionComplete(const pj::OnNatDetectionCompleteParam &prm)
{
	_onNatDetectionComplete_function_internal(prm);
}

///	<summary>
///	Callback when the Endpoint has finished performing STUN server
/// checking that is initiated when calling libInit(), or by
/// calling natCheckStunServers().
///	</summary>
/// <param name="prm">Callback parameter.</param>
void EndpointCallback::onNatCheckStunServersComplete(const pj::OnNatCheckStunServersCompleteParam &prm)
{
	_onNatCheckStunServersComplete_function_internal(prm);
}

///	<summary>
///	This callback is called when transport state has changed.
///	</summary>
/// <param name="prm">Callback parameter.</param>
void EndpointCallback::onTransportState(const pj::OnTransportStateParam &prm)
{
	_onTransportState_function_internal(prm);
}

///	<summary>
///	Callback when a timer has fired. The timer was scheduled by
/// utilTimerSchedule().
///	</summary>
/// <param name="prm">Callback parameter.</param>
void EndpointCallback::onTimer(const pj::OnTimerParam &prm)
{
	_onTimer_function_internal(prm);
}

///	<summary>
///	This callback can be used by application to override the account
/// to be used to handle an incoming message.Initially, the account to
/// be used will be calculated automatically by the library.This initial
/// account will be used if application does not implement this callback,
/// or application sets an invalid account upon returning from this
/// callback.
/// Note that currently the incoming messages requiring account assignment
/// are INVITE, MESSAGE, SUBSCRIBE, and unsolicited NOTIFY.This callback
/// may be called before the callback of the SIP event itself, i.e:
/// incoming call, pager, subscription, or unsolicited - event.
///	</summary>
/// <param name="prm">Callback parameter.</param>
void EndpointCallback::onSelectAccount(pj::OnSelectAccountParam &prm)
{
	_onSelectAccount_function_internal(prm);
}

///	<summary>
///	Set the on NatDetectionComplete function callback.
///	</summary>
/// <param name="onNatDetectionCompleteCallBack">The on NatDetectionComplete function callback.</param>
void EndpointCallback::Set_OnNatDetectionComplete_Function(OnNatDetectionComplete_Function onNatDetectionCompleteCallBack)
{
	_onNatDetectionComplete_function_internal = onNatDetectionCompleteCallBack;
}

///	<summary>
///	Set the on NatCheckStunServersComplete function callback.
///	</summary>
/// <param name="onNatCheckStunServersCompleteCallBack">The on NatCheckStunServersComplete function callback.</param>
void EndpointCallback::Set_OnNatCheckStunServersComplete_Function(OnNatCheckStunServersComplete_Function onNatCheckStunServersCompleteCallBack)
{
	_onNatCheckStunServersComplete_function_internal = onNatCheckStunServersCompleteCallBack;
}

///	<summary>
///	Set the on TransportState function callback.
///	</summary>
/// <param name="onTransportStateCallBack">The on TransportState function callback.</param>
void EndpointCallback::Set_OnTransportState_Function(OnTransportState_Function onTransportStateCallBack)
{
	_onTransportState_function_internal = onTransportStateCallBack;
}

///	<summary>
///	Set the on Timer function callback.
///	</summary>
/// <param name="onTimerCallBack">The on Timer function callback.</param>
void EndpointCallback::Set_OnTimer_Function(OnTimer_Function onTimerCallBack)
{
	_onTimer_function_internal = onTimerCallBack;
}

///	<summary>
///	Set the on SelectAccount function callback.
///	</summary>
/// <param name="onSelectAccountCallBack">The on SelectAccount function callback.</param>
void EndpointCallback::Set_OnSelectAccount_Function(OnSelectAccount_Function onSelectAccountCallBack)
{
	_onSelectAccount_function_internal = onSelectAccountCallBack;
}

/// <summary>
/// Get TransportState.
/// </summary>
/// <param name="transportState">The current TransportState.</param>
/// <returns>The TransportState.</returns>
TransportState EndpointCallback::GetTransportStateEx(pjsip_transport_state transportState)
{
	switch (transportState)
	{
	case PJSIP_TP_STATE_CONNECTED:
		return TransportState::PJSIP_TP_STATE_CONNECTED;
	case PJSIP_TP_STATE_DISCONNECTED:
		return TransportState::PJSIP_TP_STATE_DISCONNECTED;
	case PJSIP_TP_STATE_SHUTDOWN:
		return TransportState::PJSIP_TP_STATE_SHUTDOWN;
	case PJSIP_TP_STATE_DESTROY:
		return TransportState::PJSIP_TP_STATE_DESTROY;
	default:
		return TransportState::PJSIP_TP_STATE_DESTROY;
	}
}

/// <summary>
/// Get StunNatType.
/// </summary>
/// <param name="stunNatType">The current StunNatType.</param>
/// <returns>The StunNatType.</returns>
StunNatType EndpointCallback::GetStunNatTypeEx(pj_stun_nat_type stunNatType)
{
	switch (stunNatType)
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