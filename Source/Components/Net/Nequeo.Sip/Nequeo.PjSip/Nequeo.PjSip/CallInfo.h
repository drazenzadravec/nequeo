/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallInfo.h
*  Purpose :       SIP CallInfo class.
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

#ifndef _CALLINFO_H
#define _CALLINFO_H

#include "stdafx.h"

#include "CallRole.h"
#include "CallSetting.h"
#include "InviteSessionState.h"
#include "StatusCode.h"
#include "CallMediaInfo.h"
#include "TimeVal.h"

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
			///	<summary>
			///	Call information. Application can query the call information by calling Call::getInfo().
			///	</summary>
			public ref class CallInfo sealed
			{
			public:
				///	<summary>
				///	Call information. Application can query the call information by calling Call::getInfo().
				///	</summary>
				CallInfo();

				///	<summary>
				///	Call information. Application can query the call information by calling Call::getInfo().
				///	</summary>
				~CallInfo();

				///	<summary>
				///	Gets or sets the call id.
				///	</summary>
				property int Id
				{
					int get();
					void set(int value);
				}

				///	<summary>
				///	Gets or sets the account id.
				///	</summary>
				property int AccountId
				{
					int get();
					void set(int value);
				}

				///	<summary>
				///	Gets or sets the call role (UAC == caller).
				///	</summary>
				property CallRole Role
				{
					CallRole get();
					void set(CallRole value);
				}

				///	<summary>
				///	Gets or sets the local uri.
				///	</summary>
				property String^ LocalUri
				{
					String^ get();
					void set(String^ value);
				}

				///	<summary>
				///	Gets or sets the local contact.
				///	</summary>
				property String^ LocalContact
				{
					String^ get();
					void set(String^ value);
				}

				///	<summary>
				///	Gets or sets the remote uri.
				///	</summary>
				property String^ RemoteUri
				{
					String^ get();
					void set(String^ value);
				}

				///	<summary>
				///	Gets or sets the remote contact.
				///	</summary>
				property String^ RemoteContact
				{
					String^ get();
					void set(String^ value);
				}

				///	<summary>
				///	Gets or sets the dialog Call-ID string.
				///	</summary>
				property String^ CallIdString
				{
					String^ get();
					void set(String^ value);
				}

				///	<summary>
				///	Gets or sets the call setting.
				///	</summary>
				property CallSetting^ Setting
				{
					CallSetting^ get();
					void set(CallSetting^ value);
				}

				///	<summary>
				///	Gets or sets the call state.
				///	</summary>
				property InviteSessionState State
				{
					InviteSessionState get();
					void set(InviteSessionState value);
				}

				///	<summary>
				///	Gets or sets the text describing the state.
				///	</summary>
				property String^ StateText
				{
					String^ get();
					void set(String^ value);
				}

				///	<summary>
				///	Gets or sets the last status code heard, which can be used as cause code.
				///	</summary>
				property StatusCode LastStatusCode
				{
					StatusCode get();
					void set(StatusCode value);
				}

				///	<summary>
				///	Gets or sets the reason phrase describing the last status.
				///	</summary>
				property String^ LastReason
				{
					String^ get();
					void set(String^ value);
				}

				///	<summary>
				///	Gets or sets the array of active media information.
				///	</summary>
				property array<CallMediaInfo^>^ Media
				{
					array<CallMediaInfo^>^ get();
					void set(array<CallMediaInfo^>^ value);
				}

				///	<summary>
				///	Gets or sets the Array of provisional media information. This contains the media info
				/// in the provisioning state, that is when the media session is being
				/// created / updated(SDP offer / answer is on progress).
				///	</summary>
				property array<CallMediaInfo^>^ ProvMedia
				{
					array<CallMediaInfo^>^ get();
					void set(array<CallMediaInfo^>^ value);
				}

				///	<summary>
				///	Gets or sets the Up-to-date call connected duration (zero when call is not established).
				///	</summary>
				property TimeVal^ ConnectDuration
				{
					TimeVal^ get();
					void set(TimeVal^ value);
				}

				///	<summary>
				///	Gets or sets the total call duration, including set-up time.
				///	</summary>
				property TimeVal^ TotalDuration
				{
					TimeVal^ get();
					void set(TimeVal^ value);
				}

				///	<summary>
				///	Gets or sets the flag if remote was SDP offerer.
				///	</summary>
				property bool RemOfferer
				{
					bool get();
					void set(bool value);
				}

				///	<summary>
				///	Gets or sets the number of audio streams offered by remote.
				///	</summary>
				property unsigned RemAudioCount
				{
					unsigned get();
					void set(unsigned value);
				}

				///	<summary>
				///	Gets or sets the number of video streams offered by remote.
				///	</summary>
				property unsigned RemVideoCount
				{
					unsigned get();
					void set(unsigned value);
				}

			private:
				bool _disposed;
				
				int _id;
				int _accountId;
				CallRole _role;
				String^ _localUri;
				String^ _localContact;
				String^ _remoteUri;
				String^ _remoteContact;
				String^ _callIdString;
				CallSetting^ _setting;
				InviteSessionState _state;
				String^ _stateText;
				StatusCode _lastStatusCode;
				String^ _lastReason;
				array<CallMediaInfo^>^ _media;
				array<CallMediaInfo^>^ _provMedia;
				TimeVal^ _connectDuration;
				TimeVal^ _totalDuration;
				bool _remOfferer;
				unsigned _remAudioCount;
				unsigned _remVideoCount;
			};
		}
	}
}
#endif