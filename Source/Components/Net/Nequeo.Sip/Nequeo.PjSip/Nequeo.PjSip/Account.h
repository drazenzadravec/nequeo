/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          Account.h
*  Purpose :       SIP Account class.
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

#ifndef _ACCOUNT_H
#define _ACCOUNT_H

#include "stdafx.h"

#include "AccountConnection.h"
#include "AccountCallback.h"
#include "AccountInfo.h"
#include "PresenceState.h"
#include "MediaManager.h"
#include "AudioMedia.h"
#include "CodecInfo.h"

#include "ContactCallback.h"
#include "ContactConnection.h"
#include "ContactInfo.h"
#include "ContactMapper.h"
#include "ConnectionMapper.h"
#include "SendInstantMessage.h"
#include "SendTypingIndication.h"

#include "OnIncomingCallParam.h"
#include "OnIncomingSubscribeParam.h"
#include "OnInstantMessageParam.h"
#include "OnInstantMessageStatusParam.h"
#include "OnMwiInfoParam.h"
#include "OnRegStartedParam.h"
#include "OnRegStateParam.h"
#include "OnTypingIndicationParam.h"

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
			delegate void OnIncomingCallback(pj::OnIncomingCallParam&);
			delegate void OnIncomingSubscribeCallback(pj::OnIncomingSubscribeParam&);
			delegate void OnInstantMessageCallback(pj::OnInstantMessageParam&);
			delegate void OnInstantMessageStatusCallback(pj::OnInstantMessageStatusParam&);
			delegate void OnMwiInfoCallback(pj::OnMwiInfoParam&);
			delegate void OnTypingIndicationCallback(pj::OnTypingIndicationParam&);
			delegate void OnRegStartedCallback(pj::OnRegStartedParam&);
			delegate void OnRegStateCallback(pj::OnRegStateParam&);

			ref class Contact;

			///	<summary>
			///	Sip account.
			///	</summary>
			public ref class Account sealed
			{
			public:
				///	<summary>
				///	Sip account.
				///	</summary>
				Account();

				///	<summary>
				///	Sip account.
				///	</summary>
				/// <param name="accountConnection">Account connection configuration.</param>
				Account(AccountConnection^ accountConnection);

				///	<summary>
				///	Sip account deconstructor.
				///	</summary>
				~Account();

				///	<summary>
				///	Sip account finalizer.
				///	</summary>
				!Account();

				/// <summary>
				/// Notify application on incoming call.
				/// </summary>
				event System::EventHandler<OnIncomingCallParam^>^ OnIncomingCall;

				/// <summary>
				/// Notification when incoming SUBSCRIBE request is received.
				/// </summary>
				event System::EventHandler<OnIncomingSubscribeParam^>^ OnIncomingSubscribe;

				/// <summary>
				/// Notify application on incoming instant message or pager (i.e. MESSAGE
				/// request) that was received outside call context.
				/// </summary>
				event System::EventHandler<OnInstantMessageParam^>^ OnInstantMessage;

				/// <summary>
				/// Notify application about the delivery status of outgoing pager/instant
				/// message(i.e.MESSAGE) request.
				/// </summary>
				event System::EventHandler<OnInstantMessageStatusParam^>^ OnInstantMessageStatus;

				/// <summary>
				/// Notification about MWI (Message Waiting Indication) status change.
				/// </summary>
				event System::EventHandler<OnMwiInfoParam^>^ OnMwiInfo;

				/// <summary>
				/// Notify application about typing indication.
				/// </summary>
				event System::EventHandler<OnTypingIndicationParam^>^ OnTypingIndication;

				/// <summary>
				/// Notify application when registration or unregistration has been initiated.
				/// </summary>
				event System::EventHandler<OnRegStartedParam^>^ OnRegStarted;

				/// <summary>
				/// Notify application when registration status has changed.
				/// </summary>
				event System::EventHandler<OnRegStateParam^>^ OnRegState;

				///	<summary>
				///	Gets or sets the account connection configuration.
				///	</summary>
				property AccountConnection^ AccountConnConfig
				{
					AccountConnection^ get();
					void set(AccountConnection^ value);
				}

				/// <summary>
				/// Create the account.
				/// </summary>
				void Create();

				/// <summary>
				/// Update registration or perform unregistration. Application normally
				/// only needs to call this function if it wants to manually update the
				/// registration or to unregister from the server.
				/// </summary>
				/// <param name="renew">If False, this will start unregistration process.</param>
				void Registration(bool renew);

				/// <summary>
				/// Get the account ID or index associated with this account.
				/// </summary>
				/// <returns>The account ID or index.</returns>
				int GetAccountId();

				/// <summary>
				/// Is the account still valid.
				/// </summary>
				/// <returns>True if valid: else false.</returns>
				bool IsValid();

				/// <summary>
				/// Get the account info.
				/// </summary>
				/// <returns>The account info.</returns>
				AccountInfo^ GetAccountInfo();

				/// <summary>
				/// Set the online status.
				/// </summary>
				/// <param name="presenceState">The presence state.</param>
				void SetOnlineStatus(PresenceState^ presenceState);

				/// <summary>
				/// Get the media manager.
				/// </summary>
				/// <returns>The media manager.</returns>
				MediaManager^ GetMediaManager();

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
				/// Get all contacts.
				/// </summary>
				/// <returns>A contact array.</returns>
				array<Contact^>^ GetAllContacts();

				/// <summary>
				/// Remove the contact from the list.
				/// </summary>
				/// <param name="contact">The contact to remove.</param>
				void RemoveContact(Contact^ contact);

				/// <summary>
				/// Find the contact.
				/// </summary>
				/// <param name="uri">The contact unique uri.</param>
				/// <returns>The contact.</returns>
				Contact^ FindContact(String^ uri);

			internal:
				/// <summary>
				/// Get the account callback reference.
				/// </summary>
				AccountCallback& GetAccountCallback();

				///	<summary>
				///	Gets the contacts.
				///	</summary>
				property List<Contact^>^ Contacts
				{
					List<Contact^>^ get();
				}

			private:
				AccountConnection^ _accountConnection;
				AccountCallback* _accountCallback;
				ConnectionMapper* _connectionMapper;

				List<Contact^>^ _contacts;

				bool _disposed;
				bool _created;

				String^ CreateAccount();
				String^ CreateAccountConnection();

				///	<summary>
				///	Set account connection mapping.
				///	</summary>
				/// <param name="accountConnection">Account connection configuration.</param>
				/// <param name="connectionMapper">Account connection mapping configuration.</param>
				void SetConnectionMappings(AccountConnection^ accountConnection, ConnectionMapper& connectionMapper);

				void MarshalString(String^ s, std::string& os);
				void MarshalString(String^ s, std::wstring& os);

				GCHandle _gchOnIncomingCall;
				void OnIncomingCall_Handler(pj::OnIncomingCallParam &prm);

				GCHandle _gchOnIncomingSubscribe;
				void OnIncomingSubscribe_Handler(pj::OnIncomingSubscribeParam &prm);

				GCHandle _gchOnInstantMessage;
				void OnInstantMessage_Handler(pj::OnInstantMessageParam &prm);
				
				GCHandle _gchOnInstantMessageStatus;
				void OnInstantMessageStatus_Handler(pj::OnInstantMessageStatusParam &prm);

				GCHandle _gchOnMwiInfo;
				void OnMwiInfo_Handler(pj::OnMwiInfoParam &prm);

				GCHandle _gchOnTypingIndication;
				void OnTypingIndication_Handler(pj::OnTypingIndicationParam &prm);

				GCHandle _gchOnRegStarted;
				void OnRegStarted_Handler(pj::OnRegStartedParam &prm);

				GCHandle _gchOnRegState;
				void OnRegState_Handler(pj::OnRegStateParam &prm);
			};
		}
	}
}

#endif