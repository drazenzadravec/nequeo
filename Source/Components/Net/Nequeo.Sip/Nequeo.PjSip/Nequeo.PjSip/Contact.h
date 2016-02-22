/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          Contact.h
*  Purpose :       SIP Contact class.
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

#ifndef _CONTACT_H
#define _CONTACT_H

#include "stdafx.h"

#include "Account.h"
#include "ContactCallback.h"
#include "ContactConnection.h"
#include "ContactInfo.h"
#include "ContactMapper.h"
#include "ConnectionMapper.h"
#include "SendInstantMessage.h"
#include "SendTypingIndication.h"

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
			delegate void OnContactStateCallback();

			///	<summary>
			///	Sip account contact.
			///	</summary>
			public ref class Contact sealed
			{
			public:
				/// <summary>
				/// Sip account contact.
				/// </summary>
				/// <param name="account">The Sip account.</param>
				/// <param name="contactConnection">The Sip contact connection configuration.</param>
				Contact(Account^ account, ContactConnection^ contactConnection);

				///	<summary>
				///	Sip account contact.
				///	</summary>
				~Contact();

				///	<summary>
				///	Sip account contact finalizer.
				///	</summary>
				!Contact();

				///	<summary>
				/// Notify application when the contact state has changed.
				/// Application may then query the contact info to get the details.
				///	</summary>
				event System::EventHandler<ContactInfo^>^ OnContactState;

				///	<summary>
				///	Gets the contact details.
				///	</summary>
				property ContactConnection^ ContactConn
				{
					ContactConnection^ get();
				}

				/// <summary>
				/// Create the contact.
				/// </summary>
				void Create();

				/// <summary>
				/// Get detailed contact info.
				/// </summary>
				/// <returns>The contact info.</returns>
				ContactInfo^ GetInfo();

				/// <summary>
				/// Is the contact still valid.
				/// </summary>
				/// <returns>True if valid: else false.</returns>
				bool IsValid();

				/// <summary>
				/// Update the presence information for the contact. Although the library
				/// periodically refreshes the presence subscription for all contacts,
				/// some application may want to refresh the contact's presence subscription
				/// immediately, and in this case it can use this function to accomplish
				/// this.
				///
				/// Note that the contact's presence subscription will only be initiated
				/// if presence monitoring is enabled for the contact. See
				/// subscribePresence() for more info. Also if presence subscription for
				/// the contact is already active, this function will not do anything.
				///
				/// Once the presence subscription is activated successfully for the contact,
				/// application will be notified about the contact's presence status in the
				/// OnContactState() callback.
				/// </summary>
				void UpdatePresence();

				/// <summary>
				/// Enable or disable contact's presence monitoring. Once contact's presence is
				/// subscribed, application will be informed about contact's presence status
				/// changed via OnContactState() callback.
				/// </summary>
				/// <param name="subscribe">Specify true to activate presence subscription.</param>
				void SubscribePresence(bool subscribe);

				/// <summary>
				/// Send instant messaging outside dialog, using this contact's specified
				/// account for route set and authentication.
				/// </summary>
				/// <param name="sendInstantMessageParam">Sending instant message parameter.</param>
				void SendInstantMessage(SendInstantMessageParam^ sendInstantMessageParam);

				/// <summary>
				/// Send typing indication outside dialog.
				/// </summary>
				/// <param name="sendTypingIndicationParam">Sending instant message parameter.</param>
				void SendTypingIndication(SendTypingIndicationParam^ sendTypingIndicationParam);

			internal:
				/// <summary>
				/// Get the contact callback reference.
				/// </summary>
				ContactCallback& GetContactCallback();

				/// <summary>
				/// Gets the contact uri.
				/// </summary>
				property String^ ContactUri
				{
					String^ get();
				}

			private:
				bool _disposed;
				bool _created;

				String^ _contactUri;

				Account^ _account;
				ContactConnection^ _contactConnection;

				ContactCallback* _contactCallback;
				ContactMapper* _contactMapper;

				String^ CreateContact();
				String^ CreateContactConnection();

				///	<summary>
				///	Set contact connection mapping.
				///	</summary>
				/// <param name="contactConnection">Contact connection configuration.</param>
				/// <param name="contactMapper">Contact connection mapping configuration.</param>
				void SetConnectionMappings(ContactConnection^ contactConnection, ContactMapper& contactMapper);

				void MarshalString(String^ s, std::string& os);
				void MarshalString(String^ s, std::wstring& os);

				GCHandle _gchOnContactState;
				void OnContactState_Handler();
			};
		}
	}
}
#endif