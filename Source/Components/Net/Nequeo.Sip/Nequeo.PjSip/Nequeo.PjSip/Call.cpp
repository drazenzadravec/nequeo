/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          Call.cpp
*  Purpose :       SIP Call class.
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

#include "Call.h"
#include "AudioMedia.h"
#include "VideoMedia.h"
#include "ApplicationMedia.h"
#include "UnknownMedia.h"
#include "NoMedia.h"

#include "OnCallStateParam.h"
#include "OnCallMediaStateParam.h"
#include "OnCallTsxStateParam.h"

#include "pjsua2\call.hpp"
#include "pjsua-lib\pjsua_internal.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// Sip call.
/// </summary>
/// <param name="account">The Sip account.</param>
/// <param name="callId">An index call id (0 - 3).</param>
Call::Call(Account^ account, int callId) : 
	_callCallback(new CallCallback(account->GetAccountCallback(), callId)), 
	_callMapper(new CallMapper()), _disposed(false)
{
	// Create the call.
	Create();
}

///	<summary>
///	Sip call.
///	</summary>
Call::~Call()
{
	if (!_disposed)
	{
		// Cleanup the native classes.
		this->!Call();

		_disposed = true;

		_gchOnCallState.Free();
		_gchOnCallMediaState.Free();
		_gchOnCallTsxState.Free();
	}
}

///	<summary>
///	Sip call.
///	</summary>
Call::!Call()
{
	if (!_disposed)
	{
		// If the callback has been created.
		if (_callCallback != nullptr)
		{
			// Cleanup the native classes.
			delete _callCallback;
			_callCallback = nullptr;
		}

		// If the mapper has been created.
		if (_callMapper != nullptr)
		{
			// Cleanup the native classes.
			delete _callMapper;
			_callMapper = nullptr;
		}
	}
}

/// <summary>
/// Get the call callback reference.
/// </summary>
CallCallback& Call::GetCallCallback()
{
	return *_callCallback;
}

/// <summary>
/// Create the call.
/// </summary>
void Call::Create()
{
	// Assign the handler and allocate memory.
	OnCallStateCallback^ onCallStateCallback = gcnew OnCallStateCallback(this, &Call::OnCallState_Handler);
	_gchOnCallState = GCHandle::Alloc(onCallStateCallback);

	// Get a CLS compliant pointer from our delegate
	IntPtr iponCallState = Marshal::GetFunctionPointerForDelegate(onCallStateCallback);

	// Cast the pointer to the proper function ptr signature.
	OnCallState_Function onCallStateFunction = static_cast<OnCallState_Function>(iponCallState.ToPointer());




	// Assign the handler and allocate memory.
	OnCallMediaStateCallback^ onCallMediaStateCallback = gcnew OnCallMediaStateCallback(this, &Call::OnCallMediaState_Handler);
	_gchOnCallMediaState = GCHandle::Alloc(onCallMediaStateCallback);

	// Get a CLS compliant pointer from our delegate
	IntPtr iponCallMediaState = Marshal::GetFunctionPointerForDelegate(onCallMediaStateCallback);

	// Cast the pointer to the proper function ptr signature.
	OnCallMediaState_Function onCallMediaStateFunction = static_cast<OnCallMediaState_Function>(iponCallMediaState.ToPointer());




	// Assign the handler and allocate memory.
	OnCallTsxStateCallback^ onCallTsxStateCallback = gcnew OnCallTsxStateCallback(this, &Call::OnCallTsxState_Handler);
	_gchOnCallTsxState = GCHandle::Alloc(onCallTsxStateCallback);

	// Get a CLS compliant pointer from our delegate
	IntPtr iponCallTsxState = Marshal::GetFunctionPointerForDelegate(onCallTsxStateCallback);

	// Cast the pointer to the proper function ptr signature.
	OnCallTsxState_Function onCallTsxStateFunction = static_cast<OnCallTsxState_Function>(iponCallTsxState.ToPointer());




	// Set the native function handler.
	_callCallback->Set_OnCallState_Function(onCallStateFunction);

	// Set the native function handler.
	_callCallback->Set_OnCallMediaState_Function(onCallMediaStateFunction);

	// Set the native function handler.
	_callCallback->Set_OnCallTsxState_Function(onCallTsxStateFunction);
}

///	<summary>
///	Get detail information about this call.
///	</summary>
/// <returns>Call information.</returns>
CallInfo^ Call::GetInfo()
{
	CallInfo^ callInfo = gcnew CallInfo();
	pj::CallInfo info = _callCallback->getInfo();

	callInfo->AccountId = info.accId;
	callInfo->CallIdString = gcnew String(info.callIdString.c_str());
	callInfo->Id = info.id;
	callInfo->LastReason = gcnew String(info.lastReason.c_str());
	callInfo->LocalContact = gcnew String(info.localContact.c_str());
	callInfo->LocalUri = gcnew String(info.localUri.c_str());
	callInfo->RemAudioCount = info.remAudioCount;
	callInfo->RemOfferer = info.remOfferer;
	callInfo->RemoteContact = gcnew String(info.remoteContact.c_str());
	callInfo->RemoteUri = gcnew String(info.remoteUri.c_str());
	callInfo->RemVideoCount = info.remVideoCount;
	callInfo->StateText = gcnew String(info.stateText.c_str());
	callInfo->LastStatusCode = ConnectionMapper::GetStatusCodeEx(info.lastStatusCode);
	callInfo->Role = CallMapper::GetCallRoleEx(info.role);
	callInfo->State = CallMapper::GetInviteSessionStateEx(info.state);

	callInfo->ConnectDuration = gcnew TimeVal();
	callInfo->TotalDuration = gcnew TimeVal();
	callInfo->ConnectDuration->Milliseconds = info.connectDuration.msec;
	callInfo->ConnectDuration->Seconds = info.connectDuration.sec;
	callInfo->TotalDuration->Milliseconds = info.totalDuration.msec;
	callInfo->TotalDuration->Seconds = info.totalDuration.sec;

	callInfo->Setting = gcnew CallSetting();
	callInfo->Setting->AudioCount = info.setting.audioCount;
	callInfo->Setting->Flag = (CallFlag)info.setting.flag;
	callInfo->Setting->VideoCount = info.setting.videoCount;
	callInfo->Setting->ReqKeyframeMethod = (VidReqKeyframeMethod)info.setting.reqKeyframeMethod;

	size_t mediaSize = info.media.size();
	size_t provMediaSize = info.provMedia.size();

	if (mediaSize > 0)
	{
		callInfo->Media = gcnew array<CallMediaInfo^>((int)mediaSize);
		for (int i = 0; i < mediaSize; i++)
		{
			CallMediaInfo^ mediaInfo = gcnew CallMediaInfo();
			mediaInfo->AudioConfSlot = info.media[i].audioConfSlot;
			mediaInfo->Direction = CallMapper::GetMediaDirectionEx(info.media[i].dir);
			mediaInfo->Index = info.media[i].index;
			mediaInfo->VideoCapDev = info.media[i].videoCapDev;
			mediaInfo->VideoIncomingWindowId = info.media[i].videoIncomingWindowId;
			mediaInfo->Status = CallMapper::GetCallMediaStatusEx(info.media[i].status);
			mediaInfo->Type = MediaFormat::GetMediaTypeEx(info.media[i].type);
			callInfo->Media[i] = mediaInfo;
		}
	}

	if (provMediaSize > 0)
	{
		callInfo->ProvMedia = gcnew array<CallMediaInfo^>((int)provMediaSize);
		for (int i = 0; i < provMediaSize; i++)
		{
			CallMediaInfo^ mediaInfo = gcnew CallMediaInfo();
			mediaInfo->AudioConfSlot = info.provMedia[i].audioConfSlot;
			mediaInfo->Direction = CallMapper::GetMediaDirectionEx(info.provMedia[i].dir);
			mediaInfo->Index = info.provMedia[i].index;
			mediaInfo->VideoCapDev = info.provMedia[i].videoCapDev;
			mediaInfo->VideoIncomingWindowId = info.provMedia[i].videoIncomingWindowId;
			mediaInfo->Status = CallMapper::GetCallMediaStatusEx(info.provMedia[i].status);
			mediaInfo->Type = MediaFormat::GetMediaTypeEx(info.provMedia[i].type);
			callInfo->ProvMedia[i] = mediaInfo;
		}
	}

	// Return the call info.
	return callInfo;
}

///	<summary>
///	Check if this call has active INVITE session and the INVITE
/// session has not been disconnected.
///	</summary>
/// <returns>True if the call is active; else false.</returns>
bool Call::IsActive()
{
	return _callCallback->isActive();
}

///	<summary>
///	Get PJSUA-LIB call ID or index associated with this call.
///	</summary>
/// <returns>The current call id.</returns>
int Call::GetId()
{
	return _callCallback->getId();
}

///	<summary>
///	Check if call has an active media session.
///	</summary>
/// <returns>True if the call has media; else false.</returns>
bool Call::HasMedia()
{
	return _callCallback->hasMedia();
}

///	<summary>
///	Get media for the specified media index.
///	</summary>
/// <param name="medIdx">The media index.</param>
/// <returns>The media.</returns>
MediaBase^ Call::GetMedia(unsigned medIdx)
{
	MediaBase^ mdiaBase = nullptr;
	pj::AudioMedia* audioMedia = nullptr;
	pjmedia_type mediaType = pjmedia_type::PJMEDIA_TYPE_NONE;

	// Get the media.
	pj::Media* media = _callCallback->getMedia(medIdx);

	// If media not null.
	if (media != nullptr)
	{
		// Get the media type.
		mediaType = media->getType();

		// Select the correct media type.
		switch (mediaType)
		{
		case PJMEDIA_TYPE_AUDIO:
			audioMedia = static_cast<pj::AudioMedia*>(media);
			mdiaBase = gcnew AudioMedia(*audioMedia);
			break;

		case PJMEDIA_TYPE_VIDEO:
			mdiaBase = gcnew VideoMedia();
			break;

		case PJMEDIA_TYPE_APPLICATION:
			mdiaBase = gcnew ApplicationMedia();
			break;

		case PJMEDIA_TYPE_UNKNOWN:
			mdiaBase = gcnew UnknownMedia();
			break;

		case PJMEDIA_TYPE_NONE:
		default:
			mdiaBase = gcnew NoMedia();
			break;
		}
	}
	
	// Return the media;
	return mdiaBase;
}

///	<summary>
///	Get media for the specified media index.
///	</summary>
/// <param name="htype">The header type (pjsip_hdr_e) to be checked, which
/// value may be :
/// HeaderType::PJSIP_H_ACCEPT
/// HeaderType::PJSIP_H_ALLOW
/// HeaderType::PJSIP_H_SUPPORTED</param>
/// <param name="hname">If htype specifies HeaderType::PJSIP_H_OTHER, then the header
/// name must be supplied in this argument.Otherwise
/// the value must be set to empty string("").</param>
/// <param name="token">The capability token to check. For example, if 
/// htype is HeaderType::PJSIP_H_ALLOW, then token specifies the
/// method names; if htype is HeaderType::PJSIP_H_SUPPORTED, then
/// token specifies the extension names such as "100rel".</param>
/// <returns>If the specified capability is explicitly supported.</returns>
DialogCapStatus Call::RemoteHasCap(HeaderType htype, String^ hname, String^ token)
{
	std::string hnameN;
	MarshalString(hname, hnameN);

	std::string tokenN;
	MarshalString(token, tokenN);

	// Get the dialog cap status.
	pjsip_dialog_cap_status capStatus = _callCallback->remoteHasCap((int)htype, hnameN, tokenN);
	return CallMapper::GetDialogCapStatusEx(capStatus);
}

///	<summary>
///	Get the NAT type of remote's endpoint. This is a proprietary feature
/// of PJSUA - LIB which sends its NAT type in the SDP when natTypeInSdp
/// is set in UaConfig. This function can only be called after SDP has been received from remote,
/// which means for incoming call, this function can be called as soon as
/// call is received as long as incoming call contains SDP, and for outgoing
/// call, this function can be called only after SDP is received(normally in
/// 200 OK response to INVITE).As a general case, application should call
/// this function after or in onCallMediaState() callback.
///	</summary>
/// <returns>The NAT type.</returns>
StunNatType Call::GetRemNatType()
{
	pj_stun_nat_type stunNat = _callCallback->getRemNatType();
	return CallMapper::GetStunNatTypeEx(stunNat);
}

///	<summary>
///	Make outgoing call to the specified URI.
///	</summary>
/// <param name="uri">URI to be put in the To header (normally is the same as the target URI).</param>
/// <param name="callOpParam">Optional call setting.</param>
void Call::MakeCall(String^ uri, CallOpParam^ callOpParam)
{
	std::string dst_uri;
	MarshalString(uri, dst_uri);

	pj::CallOpParam prm;
	GetCallOpParam(callOpParam, prm);

	// Make the call.
	_callCallback->makeCall(dst_uri, prm);
}

///	<summary>
///	Send response to incoming INVITE request with call setting param.
/// Depending on the status code specified as parameter, this function may
/// send provisional response, establish the call, or terminate the call.
/// Notes about call setting:
/// If call setting is changed in the subsequent call to this function,
/// only the first call setting supplied will applied.So normally
/// application will not supply call setting before getting confirmation
/// from the user.
/// If no call setting is supplied when SDP has to be sent, i.e: answer
/// with status code 183 or 2xx, the default call setting will be used,
/// check CallSetting for its default values.
///	</summary>
/// <param name="callOpParam">Optional call setting. callOpParam.statusCode Status code, (100-699).</param>
void Call::Answer(CallOpParam^ callOpParam)
{
	pj::CallOpParam prm;
	GetCallOpParam(callOpParam, prm);

	// Answer the call.
	_callCallback->answer(prm);
}

///	<summary>
///	Hangup call by using method that is appropriate according to the
/// call state.This function is different than answering the call with
/// 3xx - 6xx response(with answer()), in that this function
/// will hangup the call regardless of the state and role of the call,
/// while answer() only works with incoming calls on EARLY state.
///	</summary>
/// <param name="callOpParam">Optional call setting. incoming call. If the value is zero, "603/Decline" will be sent.</param>
void Call::Hangup(CallOpParam^ callOpParam)
{
	pj::CallOpParam prm;
	GetCallOpParam(callOpParam, prm);

	// Hangup the call.
	_callCallback->hangup(prm);
}

///	<summary>
///	Put the specified call on hold. This will send re-INVITE with the
/// appropriate SDP to inform remote that the call is being put on hold.
/// The final status of the request itself will be reported on the
/// onCallMediaState() callback, which inform the application that
/// the media state of the call has changed.
///	</summary>
/// <param name="callOpParam">Optional call setting. 
/// prm.options   Bitmask of pjsua_call_flag constants. Currently,
/// only the flag PJSUA_CALL_UPDATE_CONTACT can be used.</param>
void Call::SetHold(CallOpParam^ callOpParam)
{
	pj::CallOpParam prm;
	GetCallOpParam(callOpParam, prm);

	// Hold the call.
	_callCallback->setHold(prm);
}

///	<summary>
///	Send re-INVITE.
/// The final status of the request itself will be reported on the
/// onCallMediaState() callback, which inform the application that
/// the media state of the call has changed.
///	</summary>
/// <param name="callOpParam">Optional call setting. 
/// callOpParam.opt.flag  Bitmask of pjsua_call_flag constants. Specifying
/// PJSUA_CALL_UNHOLD here will release call hold.</param>
void Call::Reinvite(CallOpParam^ callOpParam)
{
	pj::CallOpParam prm;
	GetCallOpParam(callOpParam, prm);

	// Reinvite the call.
	_callCallback->reinvite(prm);
}

///	<summary>
///	Send UPDATE request.
///	</summary>
/// <param name="callOpParam">Optional call setting.</param>
void Call::Update(CallOpParam^ callOpParam)
{
	pj::CallOpParam prm;
	GetCallOpParam(callOpParam, prm);

	// Update the call.
	_callCallback->update(prm);
}

///	<summary>
///	Initiate call transfer to the specified address. This function will send
/// REFER request to instruct remote call party to initiate a new INVITE
/// session to the specified destination / target.
/// If application is interested to monitor the successfulness and
/// the progress of the transfer request, it can implement
/// onCallTransferStatus() callback which will report the progress
/// of the call transfer request.
///	</summary>
/// <param name="destination">The URI of new target to be contacted. The URI may be
/// in name address or addr - spec format.</param>
/// <param name="callOpParam">Optional call setting.</param>
void Call::Transfer(String^ destination, CallOpParam^ callOpParam)
{
	std::string dst_uri;
	MarshalString(destination, dst_uri);

	pj::CallOpParam prm;
	GetCallOpParam(callOpParam, prm);

	// Transfer the call.
	_callCallback->xfer(dst_uri, prm);
}

///	<summary>
///	Initiate call transfer to the specified address. This function will send
/// REFER request to instruct remote call party to initiate a new INVITE
/// session to the specified destination / target.
/// If application is interested to monitor the successfulness and
/// the progress of the transfer request, it can implement
/// onCallTransferStatus() callback which will report the progress
/// of the call transfer request.
///	</summary>
/// <param name="destination">The call to be replaced.</param>
/// <param name="callOpParam">Optional call setting. callOpParam.options Application may specify
/// PJSUA_XFER_NO_REQUIRE_REPLACES to suppress the
/// inclusion of "Require: replaces" in
/// the outgoing INVITE request created by the REFER request.</param>
void Call::TransferReplaces(Call^ destination, CallOpParam^ callOpParam)
{
	pj::CallOpParam prm;
	GetCallOpParam(callOpParam, prm);

	// Get the call refence.
	CallCallback& call = this->GetCallCallback();

	// Transfer replace the call.
	_callCallback->xferReplaces(call, prm);
}

///	<summary>
///	Accept or reject redirection response. Application MUST call this
/// function after it signaled PJSIP_REDIRECT_PENDING in the
/// onCallRedirected() callback,
/// to notify the call whether to accept or reject the redirection
/// to the current target.Application can use the combination of
/// PJSIP_REDIRECT_PENDING command in onCallRedirected() callback and
/// this function to ask for user permission before redirecting the call.
/// Note that if the application chooses to reject or stop redirection(by
/// using PJSIP_REDIRECT_REJECT or PJSIP_REDIRECT_STOP respectively), the
/// call disconnection callback will be called before this function returns.
/// And if the application rejects the target, the \a onCallRedirected()
/// callback may also be called before this function returns if there is
/// another target to try.
///	</summary>
/// <param name="redirectResponse">
/// Redirection operation to be applied to the current
/// target.The semantic of this argument is similar
/// to the description in the onCallRedirected()
/// callback, except that the PJSIP_REDIRECT_PENDING is
/// not accepted here.</param>
void Call::ProcessRedirect(RedirectResponseType redirectResponse)
{
	pjsip_redirect_op cmd = CallMapper::GetRedirectResponseTypeEx(redirectResponse);
	_callCallback->processRedirect(cmd);
}

///	<summary>
///	Send DTMF digits to remote using RFC 2833 payload formats.
///	</summary>
/// <param name="digits">DTMF string digits to be sent.</param>
void Call::DialDtmf(String^ digits)
{
	std::string digitsN;
	MarshalString(digits, digitsN);

	//***********************
	//pjsua_data* hh = pjsua_get_var();
	//pjsua_call *call = &hh->calls[_callCallback->getId()];
	//**********************

	// Send DTMF digits to remote using RFC 2833 payload formats.
	_callCallback->dialDtmf(digitsN);
}

/// <summary>
/// Send instant messaging inside INVITE session.
/// </summary>
/// <param name="sendInstantMessageParam">Sending instant message parameter.</param>
void Call::SendInstantMessage(SendInstantMessageParam^ sendInstantMessageParam)
{
	std::string content;
	MarshalString(sendInstantMessageParam->Content, content);

	std::string contentType;
	MarshalString(sendInstantMessageParam->ContentType, contentType);

	pj::SendInstantMessageParam prm;
	prm.content = content;
	prm.contentType = contentType;

	if (sendInstantMessageParam->TxOption != nullptr)
	{
		std::string txContentType;
		MarshalString(sendInstantMessageParam->TxOption->ContentType, txContentType);

		std::string txMsgBody;
		MarshalString(sendInstantMessageParam->TxOption->MsgBody, txMsgBody);

		std::string txTargetUri;
		MarshalString(sendInstantMessageParam->TxOption->TargetUri, txTargetUri);

		prm.txOption.contentType = txContentType;
		prm.txOption.msgBody = txMsgBody;
		prm.txOption.targetUri = txTargetUri;

		if (sendInstantMessageParam->TxOption->MultipartContentType != nullptr)
		{
			std::string txSubType;
			MarshalString(sendInstantMessageParam->TxOption->MultipartContentType->SubType, txSubType);

			std::string txType;
			MarshalString(sendInstantMessageParam->TxOption->MultipartContentType->Type, txType);

			prm.txOption.multipartContentType.subType = txSubType;
			prm.txOption.multipartContentType.type = txType;
		}

		// If call settings.
		if (sendInstantMessageParam->TxOption->Headers != nullptr)
		{
			pj::SipHeaderVector headers;

			// For each header
			for (int i = 0; i < sendInstantMessageParam->TxOption->Headers->Length; i++)
			{
				// Get the current header.
				SipHeader^ sipHeader = (SipHeader^)(sendInstantMessageParam->TxOption->Headers[i]);

				std::string name;
				MarshalString(sipHeader->Name, name);

				std::string value;
				MarshalString(sipHeader->Value, value);

				pj::SipHeader header;
				header.hName = name;
				header.hValue = value;

				// Add the sip header.
				headers.push_back(header);
			}

			// Ass the headers.
			prm.txOption.headers = headers;
		}
	}

	// Send instant messaging outside dialog.
	_callCallback->sendInstantMessage(prm);
}

/// <summary>
/// Send IM typing indication inside INVITE session.
/// </summary>
/// <param name="sendTypingIndicationParam">Sending instant message parameter.</param>
void Call::SendTypingIndication(SendTypingIndicationParam^ sendTypingIndicationParam)
{
	pj::SendTypingIndicationParam prm;
	prm.isTyping = sendTypingIndicationParam->IsTyping;

	if (sendTypingIndicationParam->TxOption != nullptr)
	{
		std::string txContentType;
		MarshalString(sendTypingIndicationParam->TxOption->ContentType, txContentType);

		std::string txMsgBody;
		MarshalString(sendTypingIndicationParam->TxOption->MsgBody, txMsgBody);

		std::string txTargetUri;
		MarshalString(sendTypingIndicationParam->TxOption->TargetUri, txTargetUri);

		prm.txOption.contentType = txContentType;
		prm.txOption.msgBody = txMsgBody;
		prm.txOption.targetUri = txTargetUri;

		if (sendTypingIndicationParam->TxOption->MultipartContentType != nullptr)
		{
			std::string txSubType;
			MarshalString(sendTypingIndicationParam->TxOption->MultipartContentType->SubType, txSubType);

			std::string txType;
			MarshalString(sendTypingIndicationParam->TxOption->MultipartContentType->Type, txType);

			prm.txOption.multipartContentType.subType = txSubType;
			prm.txOption.multipartContentType.type = txType;
		}

		// If call settings.
		if (sendTypingIndicationParam->TxOption->Headers != nullptr)
		{
			pj::SipHeaderVector headers;

			// For each header
			for (int i = 0; i < sendTypingIndicationParam->TxOption->Headers->Length; i++)
			{
				// Get the current header.
				SipHeader^ sipHeader = (SipHeader^)(sendTypingIndicationParam->TxOption->Headers[i]);

				std::string name;
				MarshalString(sipHeader->Name, name);

				std::string value;
				MarshalString(sipHeader->Value, value);

				pj::SipHeader header;
				header.hName = name;
				header.hValue = value;

				// Add the sip header.
				headers.push_back(header);
			}

			// Ass the headers.
			prm.txOption.headers = headers;
		}
	}

	// Send IM typing indication inside INVITE session.
	_callCallback->sendTypingIndication(prm);
}

/// <summary>
/// Send arbitrary request with the call. This is useful for example to send
/// INFO request.Note that application should not use this function to send
/// requests which would change the invite session's state, such as
/// re - INVITE, UPDATE, PRACK, and BYE.
/// </summary>
/// <param name="callSendRequestParam">Sending call request parameter.</param>
void Call::SendRequest(CallSendRequestParam^ callSendRequestParam)
{
	std::string method;
	MarshalString(callSendRequestParam->Method, method);

	pj::CallSendRequestParam prm;
	prm.method = method;

	if (callSendRequestParam->TxOption != nullptr)
	{
		std::string txContentType;
		MarshalString(callSendRequestParam->TxOption->ContentType, txContentType);

		std::string txMsgBody;
		MarshalString(callSendRequestParam->TxOption->MsgBody, txMsgBody);

		std::string txTargetUri;
		MarshalString(callSendRequestParam->TxOption->TargetUri, txTargetUri);

		prm.txOption.contentType = txContentType;
		prm.txOption.msgBody = txMsgBody;
		prm.txOption.targetUri = txTargetUri;

		if (callSendRequestParam->TxOption->MultipartContentType != nullptr)
		{
			std::string txSubType;
			MarshalString(callSendRequestParam->TxOption->MultipartContentType->SubType, txSubType);

			std::string txType;
			MarshalString(callSendRequestParam->TxOption->MultipartContentType->Type, txType);

			prm.txOption.multipartContentType.subType = txSubType;
			prm.txOption.multipartContentType.type = txType;
		}

		// If call settings.
		if (callSendRequestParam->TxOption->Headers != nullptr)
		{
			pj::SipHeaderVector headers;

			// For each header
			for (int i = 0; i < callSendRequestParam->TxOption->Headers->Length; i++)
			{
				// Get the current header.
				SipHeader^ sipHeader = (SipHeader^)(callSendRequestParam->TxOption->Headers[i]);

				std::string name;
				MarshalString(sipHeader->Name, name);

				std::string value;
				MarshalString(sipHeader->Value, value);

				pj::SipHeader header;
				header.hName = name;
				header.hValue = value;

				// Add the sip header.
				headers.push_back(header);
			}

			// Ass the headers.
			prm.txOption.headers = headers;
		}
	}

	// Send arbitrary request with the call.
	_callCallback->sendRequest(prm);
}

/// <summary>
/// Get the media stream index of the default video stream in the call.
/// Typically this will just retrieve the stream index of the first
/// activated video stream in the call.If none is active, it will return
/// the first inactive video stream.
/// </summary>
/// <returns>The media stream index or -1 if no video stream is present in the call.</returns>
int Call::GetVideoStreamIndex()
{
	return _callCallback->vidGetStreamIdx();
}

/// <summary>
/// Determine if video stream for the specified call is currently running
/// (i.e.has been created, started, and not being paused) for the specified
/// direction.
/// </summary>
/// <param name="mediaIndex">Media stream index, or -1 to specify default video media.</param>
/// <param name="mediaDirection">The direction to be checked.</param>
/// <returns>True if stream is currently running for the specified direction.</returns>
bool Call::VideoStreamIsRunning(int mediaIndex, MediaDirection mediaDirection)
{
	pjmedia_dir dir = CallMapper::GetMediaDirectionEx(mediaDirection);
	return _callCallback->vidStreamIsRunning(mediaIndex, dir);
}

/// <summary>
/// Add, remove, modify, and/or manipulate video media stream for the
/// specified call. This may trigger a re - INVITE or UPDATE to be sent
/// for the call.
/// </summary>
/// <param name="videoStreamOperation">The video stream operation to be performed.</param>
/// <param name="callSetVideoStreamParam">The parameters for the video stream operation.</param>
void Call::SetVideoStream(VideoStreamOperation videoStreamOperation, CallSetVideoStreamParam^ callSetVideoStreamParam)
{
	pj::CallVidSetStreamParam callVideo;

	if (callSetVideoStreamParam != nullptr)
	{
		callVideo.capDev = callSetVideoStreamParam->CaptureDevice;
		callVideo.medIdx = callSetVideoStreamParam->MediaIndex;
		callVideo.dir = CallMapper::GetMediaDirectionEx(callSetVideoStreamParam->Direction);
	}

	pjsua_call_vid_strm_op op = CallMapper::GetVideoStreamOperationEx(videoStreamOperation);
	_callCallback->vidSetStream(op, callVideo);
}

/// <summary>
/// Get media stream info for the specified media index.
/// </summary>
/// <param name="mediaIndex">Media stream index.</param>
/// <returns>The stream info.</returns>
MediaStreamInfo^ Call::GetStreamInfo(unsigned mediaIndex)
{
	MediaStreamInfo^ mediaStreamInfo = gcnew MediaStreamInfo();
	pj::StreamInfo info = _callCallback->getStreamInfo(mediaIndex);

	mediaStreamInfo->CodecClockRate = info.codecClockRate;
	mediaStreamInfo->CodecName = gcnew String(info.codecName.c_str());
	mediaStreamInfo->Direction = CallMapper::GetMediaDirectionEx(info.dir);
	mediaStreamInfo->RemoteRtcpAddress = gcnew String(info.remoteRtcpAddress.c_str());
	mediaStreamInfo->RemoteRtpAddress = gcnew String(info.remoteRtpAddress.c_str());
	mediaStreamInfo->RxPayloadType = info.rxPt;
	mediaStreamInfo->TxPayloadType = info.txPt;
	mediaStreamInfo->Type = MediaFormat::GetMediaTypeEx(info.type);
	mediaStreamInfo->TransportProtocol = CallMapper::GetMediaTransportProtocolEx(info.proto);

	// Return the stream info.
	return mediaStreamInfo;
}

/// <summary>
/// Get media stream statistic for the specified media index.
/// </summary>
/// <param name="mediaIndex">Media stream index.</param>
/// <returns>The stream statistic.</returns>
MediaStreamStat^ Call::GetStreamStat(unsigned mediaIndex)
{
	MediaStreamStat^ mediaStreamStat = gcnew MediaStreamStat();
	pj::StreamStat info = _callCallback->getStreamStat(mediaIndex);


	// Return the stream statistics.
	return mediaStreamStat;
}

/// <summary>
/// Get media transport info for the specified media index.
/// </summary>
/// <param name="mediaIndex">Media stream index.</param>
/// <returns>The transport info.</returns>
MediaTransportInfo^ Call::GetMedTransportInfo(unsigned mediaIndex)
{
	MediaTransportInfo^ mediaTransportInfo = gcnew MediaTransportInfo();
	pj::MediaTransportInfo info = _callCallback->getMedTransportInfo(mediaIndex);

	mediaTransportInfo->RemoteRtcpName = gcnew String(info.srcRtcpName.c_str());
	mediaTransportInfo->RemoteRtpName = gcnew String(info.srcRtpName.c_str());

	// Return the transport info.
	return mediaTransportInfo;
}

/// <summary>
/// Dump call and media statistics to string.
/// </summary>
/// <param name="withMedia">True to include media information too.</param>
/// <param name="indent">Spaces for left indentation.</param>
/// <returns>Call dump and media statistics string.</returns>
String^ Call::Dump(bool withMedia, String^ indent)
{
	std::string indentN;
	MarshalString(indent, indentN);

	// Dump call and media statistics to string.
	std::string dump = _callCallback->dump(withMedia, indentN);
	return gcnew String(dump.c_str());
}

///	<summary>
///	MarshalString
///	</summary>
/// <param name="s">The string.</param>
/// <param name="os">The native string.</param>
void Call::MarshalString(String^ s, std::string& os)
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
void Call::MarshalString(String^ s, std::wstring& os)
{
	if (!String::IsNullOrEmpty(s))
	{
		using namespace Runtime::InteropServices;
		const wchar_t* chars = (const wchar_t*)(Marshal::StringToHGlobalUni(s)).ToPointer();
		os = chars;
		Marshal::FreeHGlobal(IntPtr((void*)chars));
	}
}

///	<summary>
///	Get the pj call options.
///	</summary>
/// <param name="callOpParam">Optional call setting.</param>
/// <param name="callOpParam">Optional pj call setting.</param>
void Call::GetCallOpParam(CallOpParam^ callOpParam, pj::CallOpParam& prm)
{
	// If call settings.
	if (callOpParam->Setting != nullptr)
	{
		prm.opt.audioCount = callOpParam->Setting->AudioCount;
		prm.opt.flag = (unsigned)callOpParam->Setting->Flag;
		prm.opt.reqKeyframeMethod = (unsigned)callOpParam->Setting->ReqKeyframeMethod;
		prm.opt.videoCount = callOpParam->Setting->VideoCount;
	}

	std::string reason;
	MarshalString(callOpParam->Reason, reason);

	prm.options = callOpParam->Options;
	prm.reason = reason;
	prm.statusCode = ConnectionMapper::GetStatusCodeEx(callOpParam->Code);

	// If call settings.
	if (callOpParam->TxOption != nullptr)
	{
		std::string contentType;
		MarshalString(callOpParam->TxOption->ContentType, contentType);

		std::string msgBody;
		MarshalString(callOpParam->TxOption->MsgBody, msgBody);

		std::string targetUri;
		MarshalString(callOpParam->TxOption->TargetUri, targetUri);

		prm.txOption.contentType = contentType;
		prm.txOption.msgBody = msgBody;
		prm.txOption.targetUri = targetUri;

		// If call settings.
		if (callOpParam->TxOption->MultipartContentType != nullptr)
		{
			std::string subType;
			MarshalString(callOpParam->TxOption->MultipartContentType->SubType, subType);

			std::string type;
			MarshalString(callOpParam->TxOption->MultipartContentType->Type, type);

			prm.txOption.multipartContentType.subType = subType;
			prm.txOption.multipartContentType.type = type;
		}

		// If call settings.
		if (callOpParam->TxOption->Headers != nullptr)
		{
			pj::SipHeaderVector headers;

			// For each header
			for (int i = 0; i < callOpParam->TxOption->Headers->Length; i++)
			{
				// Get the current header.
				SipHeader^ sipHeader = (SipHeader^)(callOpParam->TxOption->Headers[i]);

				std::string name;
				MarshalString(sipHeader->Name, name);

				std::string value;
				MarshalString(sipHeader->Value, value);

				pj::SipHeader header;
				header.hName = name;
				header.hValue = value;

				// Add the sip header.
				headers.push_back(header);
			}

			// Ass the headers.
			prm.txOption.headers = headers;
		}
	}
}

///	<summary>
///	On Call State function callback.
///	</summary>
/// <param name="prm">The on incoming call parameters.</param>
void Call::OnCallState_Handler(pj::OnCallStateParam &prm)
{
	// Convert the type.
	OnCallStateParam^ param = gcnew OnCallStateParam();
	param->CurrentCall = this;
	param->Info = GetInfo();
	param->EventType = CallMapper::GetSipEventTypeEx(prm.e.type);

	// Call the event handler.
	OnCallState(this, param);
}

///	<summary>
///	On Call Media State function callback.
///	</summary>
/// <param name="prm">The on incoming call parameters.</param>
void Call::OnCallMediaState_Handler(pj::OnCallMediaStateParam &prm)
{
	// Convert the type.
	OnCallMediaStateParam^ param = gcnew OnCallMediaStateParam();
	param->CurrentCall = this;
	param->Info = GetInfo();

	// Call the event handler.
	OnCallMediaState(this, param);
}

///	<summary>
///	On Call Tsx State function callback.
///	</summary>
/// <param name="prm">The on incoming call parameters.</param>
void Call::OnCallTsxState_Handler(pj::OnCallTsxStateParam &prm)
{
	// Convert the type.
	OnCallTsxStateParam^ param = gcnew OnCallTsxStateParam();
	param->CurrentCall = this;
	param->Info = GetInfo();
	param->EventType = CallMapper::GetSipEventTypeEx(prm.e.type);

	// Call the event handler.
	OnCallTsxState(this, param);
}