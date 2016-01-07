/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          Contact.cpp
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

#include "stdafx.h"

#include "Contact.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// Sip account contact.
/// </summary>
/// <param name="account">The Sip account.</param>
/// <param name="contactConnection">The Sip contact connection configuration.</param>
Contact::Contact(Account^ account, ContactConnection^ contactConnection) :
	_disposed(false), _created(false), _contactCallback(new ContactCallback()), _contactMapper(new ContactMapper())
{
	_account = account;
	_contactConnection = contactConnection;
	_contactUri = _contactConnection->Uri;
	_account->Contacts->Add(this);
}

///	<summary>
///	Contact information.
///	</summary>
Contact::~Contact()
{
	if (!_disposed)
	{
		// Cleanup the native classes.
		this->!Contact();

		_disposed = true;

		_gchOnContactState.Free();
	}
}

///	<summary>
///	Sip account finalizer.
///	</summary>
Contact::!Contact()
{
	if (!_disposed)
	{
		// If the callback has been created.
		if (_contactCallback != nullptr)
		{
			// Cleanup the native classes.
			delete _contactCallback;
			_contactCallback = nullptr;
		}

		// If the mapper has been created.
		if (_contactMapper != nullptr)
		{
			// Cleanup the native classes.
			delete _contactMapper;
			_contactMapper = nullptr;
		}
	}
}

///	<summary>
///	Gets the contact details.
///	</summary>
ContactConnection^ Contact::ContactConn::get()
{
	return _contactConnection;
}

// Get the create contact error message.
String^ Contact::CreateContact()
{
	return "Please create the contact first.";
}

// Get the create contact connection error message.
String^ Contact::CreateContactConnection()
{
	return "Please create the contact connection configuration first.";
}

/// <summary>
/// Create the contact.
/// </summary>
void Contact::Create()
{
	// If an account and a contact connection exists.
	if (_account != nullptr && _contactConnection != nullptr)
	{
		// If not created.
		if (!_created)
		{
			// Assign the handler and allocate memory.
			OnContactStateCallback^ onContactStateCallback = gcnew OnContactStateCallback(this, &Contact::OnContactState_Handler);
			_gchOnContactState = GCHandle::Alloc(onContactStateCallback);

			// Get a CLS compliant pointer from our delegate
			IntPtr ipContactState = Marshal::GetFunctionPointerForDelegate(onContactStateCallback);

			// Cast the pointer to the proper function ptr signature.
			OnBuddyState_Function onContactStateFunction = static_cast<OnBuddyState_Function>(ipContactState.ToPointer());

			// Set the on buddy state native function handler.
			_contactCallback->Set_OnBuddyState_Function(onContactStateFunction);

			// Inistalise all account configuration.
			SetConnectionMappings(_contactConnection, *_contactMapper);
			_contactCallback->Create(_account->GetAccountCallback(), *_contactMapper);

			// Creation complete.
			_created = true;
		}
	}
	else
	{
		throw gcnew Exception(CreateContactConnection());
	}
}

/// <summary>
/// Get detailed contact info.
/// </summary>
/// <returns>The contact info.</returns>
ContactInfo^ Contact::GetInfo()
{
	// If account created.
	if (_created)
	{
		// Convert the type.
		ContactInfo^ contactInfo = gcnew ContactInfo();
		PresenceState^ status = gcnew PresenceState();
		pj::BuddyInfo info = _contactCallback->getInfo();

		status->Activity = ConnectionMapper::GetActivityEx(info.presStatus.activity);
		status->Note = gcnew String(info.presStatus.note.c_str());
		status->RpidId = gcnew String(info.presStatus.rpidId.c_str());
		status->Status = ConnectionMapper::GetBuddyStatusEx(info.presStatus.status);
		status->StatusText = gcnew String(info.presStatus.statusText.c_str());

		contactInfo->Info = gcnew String(info.contact.c_str());
		contactInfo->PresenceStatus = status;
		contactInfo->PresMonitorEnabled = info.presMonitorEnabled;
		contactInfo->SubState = ConnectionMapper::GetSubscriptionStateEx(info.subState);
		contactInfo->SubStateName = gcnew String(info.subStateName.c_str());
		contactInfo->SubTermCode = ConnectionMapper::GetStatusCodeEx(info.subTermCode);
		contactInfo->SubTermReason = gcnew String(info.subTermReason.c_str());
		contactInfo->Uri = gcnew String(info.uri.c_str());

		// Return the contact info.
		return contactInfo;
	}
	else
	{
		throw gcnew Exception(CreateContact());
	}
}

/// <summary>
/// Is the contact still valid.
/// </summary>
/// <returns>True if valid: else false.</returns>
bool Contact::IsValid()
{
	// If account created.
	if (_created)
	{
		// Is contact valid.
		return _contactCallback->isValid();
	}
	else
	{
		throw gcnew Exception(CreateContact());
	}
}

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
void Contact::UpdatePresence()
{
	// If account created.
	if (_created)
	{
		// Is contact valid.
		_contactCallback->updatePresence();
	}
	else
	{
		throw gcnew Exception(CreateContact());
	}
}

/// <summary>
/// Enable or disable contact's presence monitoring. Once contact's presence is
/// subscribed, application will be informed about contact's presence status
/// changed via OnContactState() callback.
/// </summary>
/// <param name="subscribe">Specify true to activate presence subscription.</param>
void Contact::SubscribePresence(bool subscribe)
{
	// If account created.
	if (_created)
	{
		// Is contact valid.
		_contactCallback->subscribePresence(subscribe);
	}
	else
	{
		throw gcnew Exception(CreateContact());
	}
}

/// <summary>
/// Send instant messaging outside dialog, using this contact's specified
/// account for route set and authentication.
/// </summary>
/// <param name="sendInstantMessageParam">Sending instant message parameter.</param>
void Contact::SendInstantMessage(SendInstantMessageParam^ sendInstantMessageParam)
{
	// If account created.
	if (_created)
	{
		std::string content = "";
		MarshalString(sendInstantMessageParam->Content, content);

		std::string contentType = "";
		MarshalString(sendInstantMessageParam->ContentType, contentType);

		pj::SendInstantMessageParam prm;
		prm.content = content;
		prm.contentType = contentType;

		if (sendInstantMessageParam->TxOption != nullptr)
		{
			std::string txContentType = "";
			MarshalString(sendInstantMessageParam->TxOption->ContentType, txContentType);

			std::string txMsgBody = "";
			MarshalString(sendInstantMessageParam->TxOption->MsgBody, txMsgBody);

			std::string txTargetUri = "";
			MarshalString(sendInstantMessageParam->TxOption->TargetUri, txTargetUri);

			prm.txOption.contentType = txContentType;
			prm.txOption.msgBody = txMsgBody;
			prm.txOption.targetUri = txTargetUri;

			if (sendInstantMessageParam->TxOption->MultipartContentType != nullptr)
			{
				std::string txSubType = "";
				MarshalString(sendInstantMessageParam->TxOption->MultipartContentType->SubType, txSubType);

				std::string txType = "";
				MarshalString(sendInstantMessageParam->TxOption->MultipartContentType->Type, txType);

				prm.txOption.multipartContentType.subType = txSubType;
				prm.txOption.multipartContentType.type = txType;
			}
		}

		// Send message.
		_contactCallback->sendInstantMessage(prm);
	}
	else
	{
		throw gcnew Exception(CreateContact());
	}
}

/// <summary>
/// Send typing indication outside dialog.
/// </summary>
/// <param name="sendTypingIndicationParam">Sending instant message parameter.</param>
void Contact::SendTypingIndication(SendTypingIndicationParam^ sendTypingIndicationParam)
{
	// If account created.
	if (_created)
	{
		pj::SendTypingIndicationParam prm;
		prm.isTyping = sendTypingIndicationParam->IsTyping;

		if (sendTypingIndicationParam->TxOption != nullptr)
		{
			std::string txContentType = "";
			MarshalString(sendTypingIndicationParam->TxOption->ContentType, txContentType);

			std::string txMsgBody = "";
			MarshalString(sendTypingIndicationParam->TxOption->MsgBody, txMsgBody);

			std::string txTargetUri = "";
			MarshalString(sendTypingIndicationParam->TxOption->TargetUri, txTargetUri);

			prm.txOption.contentType = txContentType;
			prm.txOption.msgBody = txMsgBody;
			prm.txOption.targetUri = txTargetUri;

			if (sendTypingIndicationParam->TxOption->MultipartContentType != nullptr)
			{
				std::string txSubType = "";
				MarshalString(sendTypingIndicationParam->TxOption->MultipartContentType->SubType, txSubType);

				std::string txType = "";
				MarshalString(sendTypingIndicationParam->TxOption->MultipartContentType->Type, txType);

				prm.txOption.multipartContentType.subType = txSubType;
				prm.txOption.multipartContentType.type = txType;
			}
		}

		// Send message.
		_contactCallback->sendTypingIndication(prm);
	}
	else
	{
		throw gcnew Exception(CreateContact());
	}
}

/// <summary>
/// Get the contact callback reference.
/// </summary>
ContactCallback& Contact::GetContactCallback()
{
	return *_contactCallback;
}

///	<summary>
///	Gets the contact uri.
///	</summary>
String^ Contact::ContactUri::get()
{
	return _contactUri;
}

///	<summary>
///	Set contact connection mapping.
///	</summary>
/// <param name="contactConnection">Contact connection configuration.</param>
/// <param name="contactMapper">Contact connection mapping configuration.</param>
void Contact::SetConnectionMappings(ContactConnection^ contactConnection, ContactMapper& contactMapper)
{
	std::string uri = "";
	MarshalString(contactConnection->Uri, uri);
	contactMapper.SetUri(uri);

	contactMapper.SetSubscribe(contactConnection->Subscribe);
}

///	<summary>
///	MarshalString
///	</summary>
/// <param name="s">The string.</param>
/// <param name="os">The native string.</param>
void Contact::MarshalString(String^ s, std::string& os)
{
	using namespace Runtime::InteropServices;
	const char* chars = (const char*)(Marshal::StringToHGlobalAnsi(s)).ToPointer();
	os = chars;
	Marshal::FreeHGlobal(IntPtr((void*)chars));
}

///	<summary>
///	MarshalString
///	</summary>
/// <param name="s">The string.</param>
/// <param name="os">The native string.</param>
void Contact::MarshalString(String^ s, std::wstring& os)
{
	using namespace Runtime::InteropServices;
	const wchar_t* chars = (const wchar_t*)(Marshal::StringToHGlobalUni(s)).ToPointer();
	os = chars;
	Marshal::FreeHGlobal(IntPtr((void*)chars));
}

///	<summary>
///	On Contact State function callback.
///	</summary>
void Contact::OnContactState_Handler()
{
	// Convert the type.
	ContactInfo^ param = gcnew ContactInfo();
	PresenceState^ status = gcnew PresenceState();
	pj::BuddyInfo info = _contactCallback->getInfo();

	status->Activity = ConnectionMapper::GetActivityEx(info.presStatus.activity);
	status->Note = gcnew String(info.presStatus.note.c_str());
	status->RpidId = gcnew String(info.presStatus.rpidId.c_str());
	status->Status = ConnectionMapper::GetBuddyStatusEx(info.presStatus.status);
	status->StatusText = gcnew String(info.presStatus.statusText.c_str());

	param->Info = gcnew String(info.contact.c_str());
	param->PresenceStatus = status;
	param->PresMonitorEnabled = info.presMonitorEnabled;
	param->SubState = ConnectionMapper::GetSubscriptionStateEx(info.subState);
	param->SubStateName = gcnew String(info.subStateName.c_str());
	param->SubTermCode = ConnectionMapper::GetStatusCodeEx(info.subTermCode);
	param->SubTermReason = gcnew String(info.subTermReason.c_str());
	param->Uri = gcnew String(info.uri.c_str());

	// Call the event handler.
	OnContactState(this, param);
}