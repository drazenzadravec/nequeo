/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          Endpoint.h
*  Purpose :       SIP Endpoint class.
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

#ifndef _ENDPOINT_H
#define _ENDPOINT_H

#include "stdafx.h"

#include "EndpointCallback.h"
#include "AudioMedia.h"
#include "CodecInfo.h"
#include "MediaManager.h"
#include "Account.h"
#include "TransportType.h"
#include "IPv6_Use.h"
#include "ConnectionMapper.h"
#include "TransportInfo.h"

#include "OnNatDetectionCompleteParam.h"
#include "OnNatCheckStunServersCompleteParam.h"
#include "OnTransportStateParam.h"
#include "OnTimerParam.h"
#include "OnSelectAccountParam.h"

#include "pjsua2.hpp"

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			delegate void OnNatDetectionCompleteCallback(const pj::OnNatDetectionCompleteParam&);
			delegate void OnNatCheckStunServersCompleteCallback(const pj::OnNatCheckStunServersCompleteParam&);
			delegate void OnTransportStateCallback(const pj::OnTransportStateParam&);
			delegate void OnTimerCallback(const pj::OnTimerParam&);
			delegate void OnSelectAccountCallback(pj::OnSelectAccountParam&);

			///	<summary>
			///	Sip endpoint.
			///	</summary>
			public ref class Endpoint sealed
			{
			public:
				///	<summary>
				///	Sip endpoint.
				///	</summary>
				Endpoint();

				///	<summary>
				///	Sip endpoint deconstructor.
				///	</summary>
				~Endpoint();

				///	<summary>
				///	Sip endpoint finalizer.
				///	</summary>
				!Endpoint();

				///	<summary>
				///	Callback when the Endpoint has finished performing NAT type
				/// detection that is initiated with natDetectType().
				///	</summary>
				event System::EventHandler<OnNatDetectionCompleteParam^>^ OnNatDetectionComplete;

				///	<summary>
				///	Callback when the Endpoint has finished performing STUN server
				/// checking that is initiated when calling libInit(), or by
				/// calling natCheckStunServers().
				///	</summary>
				event System::EventHandler<OnNatCheckStunServersCompleteParam^>^ OnNatCheckStunServersComplete;

				///	<summary>
				///	This callback is called when transport state has changed.
				///	</summary>
				event System::EventHandler<OnTransportStateParam^>^ OnTransportState;

				///	<summary>
				///	Callback when a timer has fired. The timer was scheduled by
				/// utilTimerSchedule().
				///	</summary>
				event System::EventHandler<OnTimerParam^>^ OnTimer;

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
				event System::EventHandler<OnSelectAccountParam^>^ OnSelectAccount;

				/// <summary>
				/// Start the application.
				/// </summary>
				/// <param name="useIPv6">Use IPv6.</param>
				/// <param name="transportType">The transport type flags.</param>
				void Initialise(IPv6_Use useIPv6, TransportType transportType);

				/// <summary>
				/// Get all supported audio codecs in the system.
				/// </summary>
				/// <returns>The supported audio codecs in the system.</returns>
				array<CodecInfo^>^ GetAudioCodecInfo();

				/// <summary>
				/// Get all supported video codecs in the system.
				/// </summary>
				/// <returns>The supported video codecs in the system.</returns>
				array<CodecInfo^>^ GetVideoCodecInfo();

				///	<summary>
				///	Change audio codec priority.
				///	</summary>
				/// <param name="codecID">which is a string that uniquely identify
				///	the codec(such as "speex/8000").</param>
				/// <param name="priority">Codec priority, 0-255, where zero means to disable
				///	the codec.</param>
				void SetPriorityAudioCodec(String^ codecID, byte priority);

				///	<summary>
				///	Change video codec priority.
				///	</summary>
				/// <param name="codecID">Codec ID, which is a string that uniquely identify
				///	the codec(such as "H263/90000").</param>
				/// <param name="priority">Codec priority, 0-255, where zero means to disable
				///	the codec.</param>
				void SetPriorityVideoCodec(String^ codecID, byte priority);

				/// <summary>
				/// Add audio media device to the application.
				/// </summary>
				/// <param name="audioMedia">The audio media device.</param>
				void AddAudioCaptureDevice(AudioMedia^ audioMedia);

				/// <summary>
				/// Add audio media device to the application.
				/// </summary>
				/// <param name="audioMedia">The audio media device.</param>
				void AddAudioPlaybackDevice(AudioMedia^ audioMedia);

				/// <summary>
				/// Get the number of active media ports.
				/// </summary>
				/// <returns>The number of active ports.</returns>
				unsigned MediaActivePorts();

				/// <summary>
				/// Get the media manager.
				/// </summary>
				/// <param name="account">The audio media device.</param>
				/// <returns>The media manager.</returns>
				MediaManager^ GetMediaManager(Account^ account);

				/// <summary>
				/// Stop all threads.
				/// </summary>
				void StopThreads();

				/// <summary>
				/// Cleanup all resources.
				/// </summary>
				void Destroy();

				/// <summary>
				/// Get the list of transport ids.
				/// </summary>
				/// <returns>The list of transport ids.</returns>
				array<int>^ GetTransportIdList();

				/// <summary>
				/// Get the transport information.
				/// </summary>
				/// <param name="transportID">The transport id.</param>
				/// <returns>The transport information.</returns>
				TransportInfo^ GetTransportInfo(int transportID);

			private:
				EndpointCallback* _endpointCallback;

				bool _disposed;
				bool _created;

				String^ CreateEndpoint();

				void MarshalString(String^ s, std::string& os);
				void MarshalString(String^ s, std::wstring& os);

				GCHandle _gchOnNatDetectionComplete;
				void OnNatDetectionComplete_Handler(const pj::OnNatDetectionCompleteParam &prm);

				GCHandle _gchOnNatCheckStunServersComplete;
				void OnNatCheckStunServersComplete_Handler(const pj::OnNatCheckStunServersCompleteParam &prm);

				GCHandle _gchOnTransportState;
				void OnTransportState_Handler(const pj::OnTransportStateParam &prm);

				GCHandle _gchOnTimer;
				void OnTimer_Handler(const pj::OnTimerParam &prm);

				GCHandle _gchOnSelectAccount;
				void OnSelectAccount_Handler(pj::OnSelectAccountParam &prm);

			};
		}
	}
}
#endif