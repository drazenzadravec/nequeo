/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          EndpointCallback.h
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

#pragma once

#ifndef _ENDPOINTCALLBACK_H
#define _ENDPOINTCALLBACK_H

#include "stdafx.h"

#include "TransportType.h"
#include "TransportState.h"
#include "StunNatType.h"
#include "Configuration.h"

#include "pjsua2.hpp"
#include "pjsua2\endpoint.hpp"

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			typedef void(*OnNatDetectionComplete_Function)(const pj::OnNatDetectionCompleteParam&);
			typedef void(*OnNatCheckStunServersComplete_Function)(const pj::OnNatCheckStunServersCompleteParam&);
			typedef void(*OnTransportState_Function)(const pj::OnTransportStateParam&);
			typedef void(*OnTimer_Function)(const pj::OnTimerParam&);
			typedef void(*OnSelectAccount_Function)(pj::OnSelectAccountParam&);

			///	<summary>
			///	Endpoint callbacks.
			///	</summary>
			class EndpointCallback : public pj::Endpoint
			{
			public:
				///	<summary>
				///	Endpoint callbacks.
				///	</summary>
				EndpointCallback();

				///	<summary>
				///	Endpoint callbacks.
				///	</summary>
				virtual ~EndpointCallback();

				/// <summary>
				/// Start the application.
				/// </summary>
				/// <param name="useIPv6">Use IPv6.</param>
				/// <param name="transportType">The transport type flags.</param>
				void Initialise(pjsua_ipv6_use useIPv6, TransportType transportType);

				/// <summary>
				/// Start the application.
				/// </summary>
				/// <param name="useIPv6">Use IPv6.</param>
				/// <param name="transportType">The transport type flags.</param>
				/// <param name="configuration">Endpoint configuration.</param>
				void Initialise(pjsua_ipv6_use useIPv6, TransportType transportType, const EndPointConfiguration& configuration);

				/// <summary>
				/// Get the audio deveice manager.
				/// </summary>
				/// <returns>The audio device manager.</returns>
				pj::AudDevManager& GetAudioDevManager();

				/// <summary>
				/// Get the video deveice manager.
				/// </summary>
				/// <returns>The video device manager.</returns>
				pj::VidDevManager& GetVideoDevManager();

				/// <summary>
				/// Get the number of active media ports.
				/// </summary>
				/// <returns>The number of active ports.</returns>
				unsigned MediaActivePorts();

				/// <summary>
				/// Get all supported audio codecs in the system.
				/// </summary>
				/// <returns>The supported audio codecs in the system.</returns>
				const pj::CodecInfoVector& GetAudioCodecInfo();

				/// <summary>
				/// Get all supported video codecs in the system.
				/// </summary>
				/// <returns>The supported video codecs in the system.</returns>
				const pj::CodecInfoVector& GetVideoCodecInfo();

				/// <summary>
				/// Get the list of transport ids.
				/// </summary>
				/// <returns>The list of transport ids.</returns>
				const std::vector<int> GetTransportIdList();

				///	<summary>
				///	Change audio codec priority.
				///	</summary>
				/// <param name="codecID">which is a string that uniquely identify
				///	the codec(such as "speex/8000").</param>
				/// <param name="priority">Codec priority, 0-255, where zero means to disable
				///	the codec.</param>
				void SetPriorityAudioCodec(const std::string &codecID, unsigned char priority);

				///	<summary>
				///	Change video codec priority.
				///	</summary>
				/// <param name="codecID">Codec ID, which is a string that uniquely identify
				///	the codec(such as "H263/90000"). Please see pjsua
				/// manual or pjmedia codec reference for details.</param>
				/// <param name="priority">Codec priority, 0-255, where zero means to disable
				///	the codec.</param>
				void SetPriorityVideoCodec(const std::string &codecID, unsigned char priority);

				/// <summary>
				/// Add audio media device to the application.
				/// </summary>
				/// <param name="audioMedia">The audio media device.</param>
				void AddAudioMedia(pj::AudioMedia& audioMedia);

				///	<summary>
				///	Callback when the Endpoint has finished performing NAT type
				/// detection that is initiated with natDetectType().
				///	</summary>
				/// <param name="prm">Callback parameter.</param>
				void onNatDetectionComplete(const pj::OnNatDetectionCompleteParam &prm);

				///	<summary>
				///	Callback when the Endpoint has finished performing STUN server
				/// checking that is initiated when calling libInit(), or by
				/// calling natCheckStunServers().
				///	</summary>
				/// <param name="prm">Callback parameter.</param>
				void onNatCheckStunServersComplete(const pj::OnNatCheckStunServersCompleteParam &prm);

				///	<summary>
				///	This callback is called when transport state has changed.
				///	</summary>
				/// <param name="prm">Callback parameter.</param>
				void onTransportState(const pj::OnTransportStateParam &prm);

				///	<summary>
				///	Callback when a timer has fired. The timer was scheduled by
				/// utilTimerSchedule().
				///	</summary>
				/// <param name="prm">Callback parameter.</param>
				void onTimer(const pj::OnTimerParam &prm);

				///	<summary>
				///	This callback can be used by application to override the account
				/// to be used to handle an incoming message. Initially, the account to
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
				void onSelectAccount(pj::OnSelectAccountParam &prm);

				///	<summary>
				///	Set the on NatDetectionComplete function callback.
				///	</summary>
				/// <param name="onNatDetectionCompleteCallBack">The on NatDetectionComplete function callback.</param>
				void Set_OnNatDetectionComplete_Function(OnNatDetectionComplete_Function onNatDetectionCompleteCallBack);

				///	<summary>
				///	Set the on NatCheckStunServersComplete function callback.
				///	</summary>
				/// <param name="onNatCheckStunServersCompleteCallBack">The on NatCheckStunServersComplete function callback.</param>
				void Set_OnNatCheckStunServersComplete_Function(OnNatCheckStunServersComplete_Function onNatCheckStunServersCompleteCallBack);

				///	<summary>
				///	Set the on TransportState function callback.
				///	</summary>
				/// <param name="onTransportStateCallBack">The on TransportState function callback.</param>
				void Set_OnTransportState_Function(OnTransportState_Function onTransportStateCallBack);

				///	<summary>
				///	Set the on Timer function callback.
				///	</summary>
				/// <param name="onTimerCallBack">The on Timer function callback.</param>
				void Set_OnTimer_Function(OnTimer_Function onTimerCallBack);

				///	<summary>
				///	Set the on SelectAccount function callback.
				///	</summary>
				/// <param name="onSelectAccountCallBack">The on SelectAccount function callback.</param>
				void Set_OnSelectAccount_Function(OnSelectAccount_Function onSelectAccountCallBack);

				/// <summary>
				/// Get TransportState.
				/// </summary>
				/// <param name="transportState">The current TransportState.</param>
				/// <returns>The TransportState.</returns>
				static TransportState GetTransportStateEx(pjsip_transport_state transportState);

				/// <summary>
				/// Get StunNatType.
				/// </summary>
				/// <param name="stunNatType">The current StunNatType.</param>
				/// <returns>The StunNatType.</returns>
				static StunNatType GetStunNatTypeEx(pj_stun_nat_type stunNatType);

			private:
				bool _disposed;
				bool _created;
				std::vector<int> _transportIDs;

				std::unique_ptr<pj::EpConfig> _epConfig;

				std::unique_ptr<pj::TransportConfig> _transportConfig_UDP;
				std::unique_ptr<pj::TransportConfig> _transportConfig_UDP6;
				std::unique_ptr<pj::TransportConfig> _transportConfig_TCP;
				std::unique_ptr<pj::TransportConfig> _transportConfig_TCP6;
				std::unique_ptr<pj::TransportConfig> _transportConfig_TLS;
				std::unique_ptr<pj::TransportConfig> _transportConfig_TLS6;

				OnNatDetectionComplete_Function _onNatDetectionComplete_function_internal;
				OnNatCheckStunServersComplete_Function _onNatCheckStunServersComplete_function_internal;
				OnTransportState_Function _onTransportState_function_internal;
				OnTimer_Function _onTimer_function_internal;
				OnSelectAccount_Function _onSelectAccount_function_internal;

			};
		}
	}
}
#endif