/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          Account.cpp
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

#include "stdafx.h"

#include "Account.h"
#include "Contact.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	Sip account.
///	</summary>
Account::Account() :
	_accountConnection(nullptr), _disposed(false), _created(false), 
	_accountCallback(new AccountCallback()), _connectionMapper(new ConnectionMapper())
{
	_contacts = gcnew List<Contact^>();
}

///	<summary>
///	Sip account.
///	</summary>
/// <param name="accountConnection">Account connection configuration.</param>
Account::Account(AccountConnection^ accountConnection) : 
	_disposed(false), _created(false),
	_accountCallback(new AccountCallback()), _connectionMapper(new ConnectionMapper())
{
	_accountConnection = accountConnection;
	_contacts = gcnew List<Contact^>();
}

///	<summary>
///	Sip account.
///	</summary>
Account::~Account()
{
	if (!_disposed)
	{
		// Cleanup the native classes.
		this->!Account();

		_disposed = true;

		if (_contacts != nullptr)
		{
			// Dispose of the contact.
			for (int i = 0; i < _contacts->Count; i++)
			{
				try
				{
					// Dispose of the contact.
					delete _contacts[i];
				}
				catch (const std::exception&) {}
			}

			_contacts->Clear();
			_contacts = nullptr;
		}

		_gchOnIncomingCall.Free();
		_gchOnIncomingSubscribe.Free();
		_gchOnInstantMessage.Free();
		_gchOnInstantMessageStatus.Free();
		_gchOnMwiInfo.Free();
		_gchOnTypingIndication.Free();
		_gchOnRegStarted.Free();
		_gchOnRegState.Free();
	}
}

///	<summary>
///	Sip account finalizer.
///	</summary>
Account::!Account()
{
	if (!_disposed)
	{
		// If the callback has been created.
		if (_accountCallback != nullptr)
		{
			// Cleanup the native classes.
			delete _accountCallback;
			_accountCallback = nullptr;
		}

		// If the mapper has been created.
		if (_connectionMapper != nullptr)
		{
			// Cleanup the native classes.
			delete _connectionMapper;
			_connectionMapper = nullptr;
		}
	}
}

// Get the create account error message.
String^ Account::CreateAccount()
{
	return "Please create the account first.";
}

// Get the create account connection error message.
String^ Account::CreateAccountConnection()
{
	return "Please create the account connection configuration first.";
}

///	<summary>
///	Gets the account connection configuration.
///	</summary>
AccountConnection^ Account::AccountConnConfig::get()
{
	return _accountConnection;
}

///	<summary>
///	Sets the account connection configuration.
///	</summary>
void Account::AccountConnConfig::set(AccountConnection^ value)
{
	_accountConnection = value;
}

/// <summary>
/// Get the account callback reference.
/// </summary>
AccountCallback& Account::GetAccountCallback()
{
	return *_accountCallback;
}

///	<summary>
///	Gets the contacts.
///	</summary>
List<Contact^>^ Account::Contacts::get()
{
	return _contacts;
}

///	<summary>
///	Set account connection mapping.
///	</summary>
/// <param name="accountConnection">Account connection configuration.</param>
/// <param name="connectionMapper">Account connection mapping configuration.</param>
void Account::SetConnectionMappings(AccountConnection^ accountConnection, ConnectionMapper& connectionMapper)
{
	std::string accountName;
	MarshalString(accountConnection->AccountName, accountName);
	connectionMapper.SetAccountName(accountName);

	std::string spHost;
	MarshalString(accountConnection->SpHost, spHost);
	connectionMapper.SetSpHost(spHost);

	connectionMapper.SetIceEnabled(accountConnection->IceEnabled);
	connectionMapper.SetNoIceRtcp(accountConnection->NoIceRtcp);
	connectionMapper.SetVideoRateControlBandwidth(accountConnection->VideoRateControlBandwidth);
	connectionMapper.SetVideoAutoTransmit(accountConnection->VideoAutoTransmit);
	connectionMapper.SetVideoAutoShow(accountConnection->VideoAutoShow);

	connectionMapper.SetIsDefault(accountConnection->IsDefault);
	connectionMapper.SetSpPort(accountConnection->SpPort);
	connectionMapper.SetPriority(accountConnection->Priority);
	connectionMapper.SetDropCallsOnFail(accountConnection->DropCallsOnFail);
	connectionMapper.SetRegisterOnAdd(accountConnection ->RegisterOnAdd);
	connectionMapper.SetRetryIntervalSec(accountConnection->RetryIntervalSec);
	connectionMapper.SetTimeoutSec(accountConnection->TimeoutSec);
	connectionMapper.SetFirstRetryIntervalSec(accountConnection->FirstRetryIntervalSec);
	connectionMapper.SetUnregWaitSec(accountConnection->UnregWaitSec);
	connectionMapper.SetDelayBeforeRefreshSec(accountConnection->DelayBeforeRefreshSec);

	connectionMapper.SetTimerMinSESec(accountConnection->TimerMinSESec);
	connectionMapper.SetTimerSessExpiresSec(accountConnection->TimerSessExpiresSec);

	connectionMapper.SetIPv6Use(accountConnection->IPv6Use);
	connectionMapper.SetSRTPUse(accountConnection->SRTPUse);
	connectionMapper.SetSRTPSecureSignaling(accountConnection->SRTPSecureSignaling);

	connectionMapper.SetMediaTransportPort(accountConnection->MediaTransportPort);
	connectionMapper.SetMediaTransportPortRange(accountConnection->MediaTransportPortRange);

	connectionMapper.SetMessageWaitingIndication(accountConnection->MessageWaitingIndication);
	connectionMapper.SetMWIExpirationSec(accountConnection->MWIExpirationSec);

	connectionMapper.SetPublishEnabled(accountConnection->PublishEnabled);
	connectionMapper.SetPublishQueue(accountConnection->PublishQueue);
	connectionMapper.SetPublishShutdownWaitMsec(accountConnection->PublishShutdownWaitMsec);

	// Auth cred info vector.
	pj::AuthCredInfoVector authCredInfoVector;

	// If credetials exist.
	if (accountConnection->AuthCredentials != nullptr && accountConnection->AuthCredentials->AuthCredentials->Length > 0)
	{
		// For each credetial.
		for (int i = 0; i < accountConnection->AuthCredentials->AuthCredentials->Length; i++)
		{
			// Create the credetial.
			AuthCredInfo^ current = (AuthCredInfo^)(accountConnection->AuthCredentials->AuthCredentials[i]);

			std::string userName;
			MarshalString(current->Username, userName);

			std::string data;
			MarshalString(current->Data, data);

			std::string realm;
			MarshalString(current->Realm, realm);

			std::string schema;
			MarshalString(current->Scheme, schema);

			pj::AuthCredInfo authCredInfo(schema, realm, userName, current->DataType, data);

			// Add the auth cred info to the list.
			authCredInfoVector.push_back(authCredInfo);
		}
	}

	// Add the credentials.
	connectionMapper.SetAuthCredentials(authCredInfoVector);
}

///	<summary>
///	MarshalString
///	</summary>
/// <param name="s">The string.</param>
/// <param name="os">The native string.</param>
void Account::MarshalString(String^ s, std::string& os)
{
	if (!String::IsNullOrEmpty(s))
	{
		using namespace Runtime::InteropServices;
		const char* chars = (const char*)(Marshal::StringToHGlobalAnsi(s)).ToPointer();
		os = chars;
		Marshal::FreeHGlobal(IntPtr((void*)chars));
	}
}

///	<summary>
///	MarshalString
///	</summary>
/// <param name="s">The string.</param>
/// <param name="os">The native string.</param>
void Account::MarshalString(String^ s, std::wstring& os)
{
	if (!String::IsNullOrEmpty(s))
	{
		using namespace Runtime::InteropServices;
		const wchar_t* chars = (const wchar_t*)(Marshal::StringToHGlobalUni(s)).ToPointer();
		os = chars;
		Marshal::FreeHGlobal(IntPtr((void*)chars));
	}
}

/// <summary>
/// Create the account.
/// </summary>
void Account::Create()
{
	// If an account connection exists.
	if (_accountConnection != nullptr)
	{
		// If not created.
		if (!_created)
		{
			// Assign the handler and allocate memory.
			OnIncomingCallback^ onIncomingCallback = gcnew OnIncomingCallback(this, &Account::OnIncomingCall_Handler);
			_gchOnIncomingCall = GCHandle::Alloc(onIncomingCallback);

			// Get a CLS compliant pointer from our delegate
			IntPtr ipIncomingCall = Marshal::GetFunctionPointerForDelegate(onIncomingCallback);

			// Cast the pointer to the proper function ptr signature.
			OnIncomingCall_Function onIncomingCallFunction = static_cast<OnIncomingCall_Function>(ipIncomingCall.ToPointer());



			// Assign the handler and allocate memory.
			OnIncomingSubscribeCallback^ onIncomingSubscribeCallback = gcnew OnIncomingSubscribeCallback(this, &Account::OnIncomingSubscribe_Handler);
			_gchOnIncomingSubscribe = GCHandle::Alloc(onIncomingSubscribeCallback);

			// Get a CLS compliant pointer from our delegate
			IntPtr ipIncomingSubscribe = Marshal::GetFunctionPointerForDelegate(onIncomingSubscribeCallback);

			// Cast the pointer to the proper function ptr signature.
			OnIncomingSubscribe_Function onIncomingSubscribeFunction = static_cast<OnIncomingSubscribe_Function>(ipIncomingSubscribe.ToPointer());



			// Assign the handler and allocate memory.
			OnInstantMessageCallback^ onInstantMessageCallback = gcnew OnInstantMessageCallback(this, &Account::OnInstantMessage_Handler);
			_gchOnInstantMessage = GCHandle::Alloc(onInstantMessageCallback);

			// Get a CLS compliant pointer from our delegate
			IntPtr ipInstantMessage = Marshal::GetFunctionPointerForDelegate(onInstantMessageCallback);

			// Cast the pointer to the proper function ptr signature.
			OnInstantMessage_Function onInstantMessageFunction = static_cast<OnInstantMessage_Function>(ipInstantMessage.ToPointer());



			// Assign the handler and allocate memory.
			OnInstantMessageStatusCallback^ onInstantMessageStatusCallback = gcnew OnInstantMessageStatusCallback(this, &Account::OnInstantMessageStatus_Handler);
			_gchOnInstantMessageStatus = GCHandle::Alloc(onInstantMessageStatusCallback);

			// Get a CLS compliant pointer from our delegate
			IntPtr ipInstantMessageStatus = Marshal::GetFunctionPointerForDelegate(onInstantMessageStatusCallback);

			// Cast the pointer to the proper function ptr signature.
			OnInstantMessageStatus_Function onInstantMessageStatusFunction = static_cast<OnInstantMessageStatus_Function>(ipInstantMessageStatus.ToPointer());



			// Assign the handler and allocate memory.
			OnMwiInfoCallback^ onMwiInfoCallback = gcnew OnMwiInfoCallback(this, &Account::OnMwiInfo_Handler);
			_gchOnMwiInfo = GCHandle::Alloc(onMwiInfoCallback);

			// Get a CLS compliant pointer from our delegate
			IntPtr ipMwiInfo = Marshal::GetFunctionPointerForDelegate(onMwiInfoCallback);

			// Cast the pointer to the proper function ptr signature.
			OnMwiInfo_Function onMwiInfoFunction = static_cast<OnMwiInfo_Function>(ipMwiInfo.ToPointer());



			// Assign the handler and allocate memory.
			OnTypingIndicationCallback^ onTypingIndicationCallback = gcnew OnTypingIndicationCallback(this, &Account::OnTypingIndication_Handler);
			_gchOnTypingIndication = GCHandle::Alloc(onTypingIndicationCallback);

			// Get a CLS compliant pointer from our delegate
			IntPtr ipTypingIndication = Marshal::GetFunctionPointerForDelegate(onTypingIndicationCallback);

			// Cast the pointer to the proper function ptr signature.
			OnTypingIndication_Function onTypingIndicationFunction = static_cast<OnTypingIndication_Function>(ipTypingIndication.ToPointer());



			// Assign the handler and allocate memory.
			OnRegStartedCallback^ onRegStartedCallback = gcnew OnRegStartedCallback(this, &Account::OnRegStarted_Handler);
			_gchOnRegStarted = GCHandle::Alloc(onRegStartedCallback);

			// Get a CLS compliant pointer from our delegate
			IntPtr ipRegStarted = Marshal::GetFunctionPointerForDelegate(onRegStartedCallback);

			// Cast the pointer to the proper function ptr signature.
			OnRegStarted_Function onRegStartedFunction = static_cast<OnRegStarted_Function>(ipRegStarted.ToPointer());



			// Assign the handler and allocate memory.
			OnRegStateCallback^ onRegStateCallback = gcnew OnRegStateCallback(this, &Account::OnRegState_Handler);
			_gchOnRegState = GCHandle::Alloc(onRegStateCallback);

			// Get a CLS compliant pointer from our delegate
			IntPtr ipRegState = Marshal::GetFunctionPointerForDelegate(onRegStateCallback);

			// Cast the pointer to the proper function ptr signature.
			OnRegState_Function onRegStateFunction = static_cast<OnRegState_Function>(ipRegState.ToPointer());



			// Set the on RegState native function handler.
			_accountCallback->Set_OnRegState_Function(onRegStateFunction);

			// Set the on RegStarted native function handler.
			_accountCallback->Set_OnRegStarted_Function(onRegStartedFunction);

			// Set the on TypingIndication native function handler.
			_accountCallback->Set_OnTypingIndication_Function(onTypingIndicationFunction);

			// Set the on MwiInfo native function handler.
			_accountCallback->Set_OnMwiInfo_Function(onMwiInfoFunction);

			// Set the on InstantMessage native function handler.
			_accountCallback->Set_OnInstantMessageStatus_Function(onInstantMessageStatusFunction);

			// Set the on instant message native function handler.
			_accountCallback->Set_OnInstantMessage_Function(onInstantMessageFunction);

			// Set the on incoming subscribe native function handler.
			_accountCallback->Set_OnIncomingSubscribe_Function(onIncomingSubscribeFunction);

			// Set the on incoming call native function handler.
			_accountCallback->Set_OnIncomingCall_Function(onIncomingCallFunction);

			// Inistalise all account configuration.
			SetConnectionMappings(_accountConnection, *_connectionMapper);
			_accountCallback->Initialise(*_connectionMapper);

			// Creation complete.
			_created = true;
		}
	}
	else
	{
		throw gcnew Exception(CreateAccountConnection());
	}
}

/// <summary>
/// Update registration or perform unregistration. Application normally
/// only needs to call this function if it wants to manually update the
/// registration or to unregister from the server.
/// </summary>
/// <param name="renew">If False, this will start unregistration process.</param>
void Account::Registration(bool renew)
{
	// If account created.
	if (_created)
	{
		// Register or unregister the account.
		_accountCallback->setRegistration(renew);
	}
	else
	{
		throw gcnew Exception(CreateAccount());
	}
}

/// <summary>
/// Get the account ID or index associated with this account.
/// </summary>
/// <returns>The account ID or index.</returns>
int Account::GetAccountId()
{
	// If account created.
	if (_created)
	{
		// Get account ID.
		return _accountCallback->getId();
	}
	else
	{
		throw gcnew Exception(CreateAccount());
	}
}

/// <summary>
/// Is the account still valid.
/// </summary>
/// <returns>True if valid: els false.</returns>
bool Account::IsValid()
{
	// If account created.
	if (_created)
	{
		// Is account valid.
		return _accountCallback->isValid();
	}
	else
	{
		throw gcnew Exception(CreateAccount());
	}
}

/// <summary>
/// Get the account info.
/// </summary>
/// <returns>The account info.</returns>
AccountInfo^ Account::GetAccountInfo()
{
	// If account created.
	if (_created)
	{
		// Get the account info.
		AccountInfo^ accountInfo = gcnew AccountInfo();
		pj::AccountInfo pjAccountInfo = _accountCallback->getInfo();

		// Assign the properties.
		accountInfo->Id = pjAccountInfo.id;
		accountInfo->IsDefault = pjAccountInfo.isDefault;
		accountInfo->OnlineStatus = pjAccountInfo.onlineStatus;
		accountInfo->OnlineStatusText = gcnew String(pjAccountInfo.onlineStatusText.c_str());
		accountInfo->RegExpiresSec = pjAccountInfo.regExpiresSec;
		accountInfo->RegIsActive = pjAccountInfo.regIsActive;
		accountInfo->RegIsConfigured = pjAccountInfo.regIsConfigured;
		accountInfo->RegLastErr = pjAccountInfo.regLastErr;
		accountInfo->RegStatus = _connectionMapper->GetStatusCodeEx(pjAccountInfo.regStatus);
		accountInfo->RegStatusText = gcnew String(pjAccountInfo.regStatusText.c_str());
		accountInfo->Uri = gcnew String(pjAccountInfo.uri.c_str());

		// Return the account info.
		return accountInfo;
	}
	else
		throw gcnew Exception(CreateAccount());
}

/// <summary>
/// Set the online status.
/// </summary>
/// <param name="presenceState">The presence state.</param>
void Account::SetOnlineStatus(PresenceState^ presenceState)
{
	// If account created.
	if (_created)
	{
		// Set the online state.
		pj::PresenceStatus pjPresenceStatus;

		std::string note;
		MarshalString(presenceState->Note, note);

		std::string rpidId;
		MarshalString(presenceState->RpidId, rpidId);

		std::string statusText;
		MarshalString(presenceState->StatusText, statusText);

		// Set the presence.
		pjPresenceStatus.note = note;
		pjPresenceStatus.rpidId = rpidId;
		pjPresenceStatus.statusText = statusText;
		pjPresenceStatus.activity = _connectionMapper->GetActivityEx(presenceState->Activity);
		pjPresenceStatus.status = _connectionMapper->GetBuddyStatusEx(presenceState->Status);

		// Set the online status.
		_accountCallback->setOnlineStatus(pjPresenceStatus);
	}
	else
		throw gcnew Exception(CreateAccount());
}

/// <summary>
/// Get the media manager.
/// </summary>
/// <returns>The media manager.</returns>
MediaManager^ Account::GetMediaManager()
{
	// Get the audio device manager.
	pj::AudDevManager& pjAudDevManager = _accountCallback->GetAudioDevManager();
	pj::VidDevManager& pjVidDevManager = _accountCallback->GetVideoDevManager();
	pj::AccountVideoConfig& pjAccountVideoConfig = _accountCallback->GetAccountVideoConfig();
	MediaManager^ mediaManager = gcnew MediaManager(pjAudDevManager, pjVidDevManager, pjAccountVideoConfig);
	return mediaManager;
}

/// <summary>
/// Get all supported codecs in the system.
/// </summary>
/// <returns>The supported codecs in the system.</returns>
array<CodecInfo^>^ Account::GetAudioCodecInfo()
{
	// If account created.
	if (_created)
	{
		List<CodecInfo^>^ codecList = gcnew List<CodecInfo^>();
		const pj::CodecInfoVector& codecs = _accountCallback->GetAudioCodecInfo();

		// Get the vector size.
		size_t vectorSize = codecs.size();

		// If devices exist.
		if (vectorSize > 0)
		{
			// For each code found.
			for (int i = 0; i < vectorSize; i++)
			{
				CodecInfo^ codec = gcnew CodecInfo();
				codec->CodecId = gcnew String(codecs[i]->codecId.c_str());
				codec->Description = gcnew String(codecs[i]->desc.c_str());
				codec->Priority = codecs[i]->priority;
				codecList->Add(codec);
			}
		}

		// Return the code list.
		return codecList->ToArray();
	}
	else
		throw gcnew Exception(CreateAccount());
}

/// <summary>
/// Get all supported video codecs in the system.
/// </summary>
/// <returns>The supported video codecs in the system.</returns>
array<CodecInfo^>^ Account::GetVideoCodecInfo()
{
	// If account created.
	if (_created)
	{
		List<CodecInfo^>^ codecList = gcnew List<CodecInfo^>();
		const pj::CodecInfoVector& codecs = _accountCallback->GetVideoCodecInfo();

		// Get the vector size.
		size_t vectorSize = codecs.size();

		// If devices exist.
		if (vectorSize > 0)
		{
			// For each code found.
			for (int i = 0; i < vectorSize; i++)
			{
				CodecInfo^ codec = gcnew CodecInfo();
				codec->CodecId = gcnew String(codecs[i]->codecId.c_str());
				codec->Description = gcnew String(codecs[i]->desc.c_str());
				codec->Priority = codecs[i]->priority;
				codecList->Add(codec);
			}
		}

		// Return the code list.
		return codecList->ToArray();
	}
	else
		throw gcnew Exception(CreateAccount());
}

/// <summary>
/// Add audio media device to the application.
/// </summary>
/// <param name="audioMedia">The audio media device.</param>
void Account::AddAudioCaptureDevice(AudioMedia^ audioMedia)
{
	_accountCallback->AddAudioMedia(audioMedia->GetAudioMedia());
}

/// <summary>
/// Add audio media device to the application.
/// </summary>
/// <param name="audioMedia">The audio media device.</param>
void Account::AddAudioPlaybackDevice(AudioMedia^ audioMedia)
{
	_accountCallback->AddAudioMedia(audioMedia->GetAudioMedia());
}

/// <summary>
/// Get the number of active media ports.
/// </summary>
/// <returns>The number of active ports.</returns>
unsigned Account::MediaActivePorts()
{
	// If account created.
	if (_created)
	{
		return _accountCallback->MediaActivePorts();
	}
	else
		throw gcnew Exception(CreateAccount());
}

/// <summary>
/// Get all contacts.
/// </summary>
/// <returns>A contact array.</returns>
array<Contact^>^ Account::GetAllContacts()
{
	// If account created.
	if (_created)
	{
		// Get all buddies.
		List<Contact^>^ contacts = gcnew List<Contact^>();
		const pj::BuddyVector& buddies = _accountCallback->enumBuddies();
		
		// Get the vector size.
		size_t vectorSize = buddies.size();

		// If devices exist.
		if (vectorSize > 0)
		{
			// For each code found.
			for (int i = 0; i < vectorSize; i++)
			{
				pj::Buddy* buddy = buddies[i];
				pj::BuddyInfo buddyInfo = buddy->getInfo();
				String^ buddyUri = gcnew String(buddyInfo.uri.c_str());

				// Find the matching buddy.
				for (int j = 0; j < _contacts->Count; j++)
				{
					// Find the buddy.
					if (_contacts[j]->ContactUri == buddyUri)
					{
						// Add the contact.
						contacts->Add(_contacts[j]);
						break;
					}
				}
			}
		}

		// Return the contact list.
		return contacts->ToArray();
	}
	else
		throw gcnew Exception(CreateAccount());
}

/// <summary>
/// Remove the contact from the list.
/// </summary>
/// <param name="contact">The contact to remove.</param>
void Account::RemoveContact(Contact^ contact)
{
	// If account created.
	if (_created)
	{
		const pj::BuddyVector& buddies = _accountCallback->enumBuddies();

		// Get the vector size.
		size_t vectorSize = buddies.size();

		// If devices exist.
		if (vectorSize > 0)
		{
			// For each code found.
			for (int i = 0; i < vectorSize; i++)
			{
				pj::Buddy* buddy = buddies[i];
				pj::BuddyInfo buddyInfo = buddy->getInfo();
				String^ buddyUri = gcnew String(buddyInfo.uri.c_str());

				// Find the buddy.
				if (contact->ContactUri == buddyUri)
				{
					try
					{
						// Remove the buddy.
						_accountCallback->removeBuddy(buddy);
					}
					catch (const std::exception&) {}
				}

				// Find the matching buddy.
				for (int j = 0; j < _contacts->Count; j++)
				{
					// Find the buddy.
					if (_contacts[j]->ContactUri == buddyUri)
					{
						try
						{
							// Remove the contact.
							delete _contacts[i];
							_contacts->Remove(contact);
						}
						catch (const std::exception&) {}
						break;
					}
				}
			}
		}
	}
	else
		throw gcnew Exception(CreateAccount());
}

/// <summary>
/// Find the contact.
/// </summary>
/// <param name="uri">The contact unique uri.</param>
/// <returns>The contact.</returns>
Contact^ Account::FindContact(String^ uri)
{
	// If account created.
	if (_created)
	{
		Contact^ contact;

		std::string uriBuddy;
		MarshalString(uri, uriBuddy);

		// Find the buddy.
		pj::Buddy* buddy = _accountCallback->findBuddy(uriBuddy);

		// Find the matching buddy.
		for (int j = 0; j < _contacts->Count; j++)
		{
			// Find the buddy.
			if (_contacts[j]->ContactUri == uri)
			{
				try
				{
					// Get the contact.
					contact = _contacts[j];
				}
				catch (const std::exception&) {}
				break;
			}
		}

		// Return the contact.
		return contact;
	}
	else
		throw gcnew Exception(CreateAccount());
}

///	<summary>
///	On incoming call function callback.
///	</summary>
/// <param name="prm">The on incoming call parameters.</param>
void Account::OnIncomingCall_Handler(pj::OnIncomingCallParam &prm)
{
	// Convert the type.
	OnIncomingCallParam^ param = gcnew OnIncomingCallParam();
	param->RxData = gcnew SipRxData();

	param->CallId = prm.callId;
	param->RxData->Info = gcnew String(prm.rdata.info.c_str());
	param->RxData->SrcAddress = gcnew String(prm.rdata.srcAddress.c_str());
	param->RxData->WholeMsg = gcnew String(prm.rdata.wholeMsg.c_str());

	// Call the event handler.
	OnIncomingCall(this, param);
}

///	<summary>
///	On incoming subscribe function callback.
///	</summary>
/// <param name="prm">The on incoming call parameters.</param>
void Account::OnIncomingSubscribe_Handler(pj::OnIncomingSubscribeParam &prm)
{
	// Convert the type.
	OnIncomingSubscribeParam^ param = gcnew OnIncomingSubscribeParam();
	param->RxData = gcnew SipRxData();
	param->TxOption = gcnew SipTxOption();
	param->TxOption->MultipartContentType = gcnew SipMediaType();

	param->Code = _connectionMapper->GetStatusCodeEx(prm.code);
	param->FromUri = gcnew String(prm.fromUri.c_str());
	param->Reason = gcnew String(prm.reason.c_str());

	param->TxOption->ContentType = gcnew String(prm.txOption.contentType.c_str());
	param->TxOption->MsgBody = gcnew String(prm.txOption.msgBody.c_str());
	param->TxOption->TargetUri = gcnew String(prm.txOption.targetUri.c_str());
	param->TxOption->MultipartContentType->SubType = gcnew String(prm.txOption.multipartContentType.subType.c_str());
	param->TxOption->MultipartContentType->Type = gcnew String(prm.txOption.multipartContentType.type.c_str());

	param->RxData->Info = gcnew String(prm.rdata.info.c_str());
	param->RxData->SrcAddress = gcnew String(prm.rdata.srcAddress.c_str());
	param->RxData->WholeMsg = gcnew String(prm.rdata.wholeMsg.c_str());

	// Get the sip headers.
	pj::SipHeaderVector sipHeaders = prm.txOption.headers;

	// Get the vector size.
	size_t vectorSize = sipHeaders.size();
	param->TxOption->Headers = gcnew array<SipHeader^>((int)vectorSize);

	// If devices exist.
	if (vectorSize > 0)
	{
		// For each code found.
		for (int i = 0; i < vectorSize; i++)
		{
			SipHeader^ sipHeader = gcnew SipHeader();
			sipHeader->Name = gcnew String(sipHeaders[i].hName.c_str());
			sipHeader->Value = gcnew String(sipHeaders[i].hValue.c_str());
			param->TxOption->Headers[i] = sipHeader;
		}
	}

	// Call the event handler.
	OnIncomingSubscribe(this, param);
}

///	<summary>
///	On Instant Message function callback.
///	</summary>
/// <param name="prm">The on incoming call parameters.</param>
void Account::OnInstantMessage_Handler(pj::OnInstantMessageParam &prm)
{
	// Convert the type.
	OnInstantMessageParam^ param = gcnew OnInstantMessageParam();
	param->RxData = gcnew SipRxData();

	param->ContactUri = gcnew String(prm.contactUri.c_str());
	param->ContentType = gcnew String(prm.contentType.c_str());
	param->FromUri = gcnew String(prm.fromUri.c_str());
	param->MsgBody = gcnew String(prm.msgBody.c_str());
	param->ToUri = gcnew String(prm.toUri.c_str());

	param->RxData->Info = gcnew String(prm.rdata.info.c_str());
	param->RxData->SrcAddress = gcnew String(prm.rdata.srcAddress.c_str());
	param->RxData->WholeMsg = gcnew String(prm.rdata.wholeMsg.c_str());

	// Call the event handler.
	OnInstantMessage(this, param);
}

///	<summary>
///	On Instant Message Status function callback.
///	</summary>
/// <param name="prm">The on incoming call parameters.</param>
void Account::OnInstantMessageStatus_Handler(pj::OnInstantMessageStatusParam &prm)
{
	// Convert the type.
	OnInstantMessageStatusParam^ param = gcnew OnInstantMessageStatusParam();
	param->RxData = gcnew SipRxData();

	param->Code = _connectionMapper->GetStatusCodeEx(prm.code);
	param->Reason = gcnew String(prm.reason.c_str());
	param->MsgBody = gcnew String(prm.msgBody.c_str());
	param->ToUri = gcnew String(prm.toUri.c_str());

	param->RxData->Info = gcnew String(prm.rdata.info.c_str());
	param->RxData->SrcAddress = gcnew String(prm.rdata.srcAddress.c_str());
	param->RxData->WholeMsg = gcnew String(prm.rdata.wholeMsg.c_str());

	// Call the event handler.
	OnInstantMessageStatus(this, param);
}

///	<summary>
///	On Mwi Info function callback.
///	</summary>
/// <param name="prm">The on incoming call parameters.</param>
void Account::OnMwiInfo_Handler(pj::OnMwiInfoParam &prm)
{
	// Convert the type.
	OnMwiInfoParam^ param = gcnew OnMwiInfoParam();
	param->RxData = gcnew SipRxData();

	param->State = _connectionMapper->GetSubscriptionStateEx(prm.state);

	param->RxData->Info = gcnew String(prm.rdata.info.c_str());
	param->RxData->SrcAddress = gcnew String(prm.rdata.srcAddress.c_str());
	param->RxData->WholeMsg = gcnew String(prm.rdata.wholeMsg.c_str());

	// Call the event handler.
	OnMwiInfo(this, param);
}

///	<summary>
///	On Typing Indication function callback.
///	</summary>
/// <param name="prm">The on incoming call parameters.</param>
void Account::OnTypingIndication_Handler(pj::OnTypingIndicationParam &prm)
{
	// Convert the type.
	OnTypingIndicationParam^ param = gcnew OnTypingIndicationParam();
	param->RxData = gcnew SipRxData();

	param->ContactUri = gcnew String(prm.contactUri.c_str());
	param->FromUri = gcnew String(prm.fromUri.c_str());
	param->IsTyping = prm.isTyping;
	param->ToUri = gcnew String(prm.toUri.c_str());

	param->RxData->Info = gcnew String(prm.rdata.info.c_str());
	param->RxData->SrcAddress = gcnew String(prm.rdata.srcAddress.c_str());
	param->RxData->WholeMsg = gcnew String(prm.rdata.wholeMsg.c_str());

	// Call the event handler.
	OnTypingIndication(this, param);
}

///	<summary>
///	On Reg Started function callback.
///	</summary>
/// <param name="prm">The on incoming call parameters.</param>
void Account::OnRegStarted_Handler(pj::OnRegStartedParam &prm)
{
	// Convert the type.
	OnRegStartedParam^ param = gcnew OnRegStartedParam();
	param->Renew = prm.renew;

	// Call the event handler.
	OnRegStarted(this, param);
}

///	<summary>
///	On Reg State function callback.
///	</summary>
/// <param name="prm">The on incoming call parameters.</param>
void Account::OnRegState_Handler(pj::OnRegStateParam &prm)
{
	// Convert the type.
	OnRegStateParam^ param = gcnew OnRegStateParam();
	param->RxData = gcnew SipRxData();

	param->Code = _connectionMapper->GetStatusCodeEx(prm.code);
	param->Expiration = prm.expiration;
	param->Reason = gcnew String(prm.reason.c_str());
	param->Status = prm.status;

	param->RxData->Info = gcnew String(prm.rdata.info.c_str());
	param->RxData->SrcAddress = gcnew String(prm.rdata.srcAddress.c_str());
	param->RxData->WholeMsg = gcnew String(prm.rdata.wholeMsg.c_str());

	// Call the event handler.
	OnRegState(this, param);
}