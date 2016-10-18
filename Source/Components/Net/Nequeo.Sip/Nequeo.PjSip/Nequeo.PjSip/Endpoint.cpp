/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          Endpoint.cpp
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

#include "stdafx.h"

#include "Endpoint.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	Sip endpoint.
///	</summary>
Endpoint::Endpoint() :
	_disposed(false), _created(false), _endpointCallback(new EndpointCallback())
{
}

///	<summary>
///	Sip endpoint.
///	</summary>
Endpoint::~Endpoint()
{
	if (!_disposed)
	{
		// Cleanup the native classes.
		this->!Endpoint();

		_disposed = true;

		_gchOnNatDetectionComplete.Free();
		_gchOnNatCheckStunServersComplete.Free();
		_gchOnTransportState.Free();
		_gchOnTimer.Free();
		_gchOnSelectAccount.Free();
	}
}

///	<summary>
///	Sip endpoint finalizer.
///	</summary>
Endpoint::!Endpoint()
{
	if (!_disposed)
	{
		// If the callback has been created.
		if (_endpointCallback != nullptr)
		{
			// Cleanup the native classes.
			delete _endpointCallback;
			_endpointCallback = nullptr;
		}
	}
}

// Get the create endpoint error message.
String^ Endpoint::CreateEndpoint()
{
	return "Please create the endpoint first.";
}

///	<summary>
///	MarshalString
///	</summary>
/// <param name="s">The string.</param>
/// <param name="os">The native string.</param>
void Endpoint::MarshalString(String^ s, std::string& os)
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
void Endpoint::MarshalString(String^ s, std::wstring& os)
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
/// Start the application.
/// </summary>
/// <param name="useIPv6">Use IPv6.</param>
/// <param name="transportType">The transport type flags.</param>
void Endpoint::Initialise(IPv6_Use useIPv6, TransportType transportType)
{
	// If endpoint not created.
	if (!_created)
	{
		pjsua_ipv6_use ipv6 = ConnectionMapper::GetIPv6UseEx(useIPv6);
		_endpointCallback->Initialise(ipv6, transportType);


		// Assign the handler and allocate memory.
		OnNatCheckStunServersCompleteCallback^ onNatCheckStunServersCompleteCallback = gcnew OnNatCheckStunServersCompleteCallback(this, &Endpoint::OnNatCheckStunServersComplete_Handler);
		_gchOnNatCheckStunServersComplete = GCHandle::Alloc(onNatCheckStunServersCompleteCallback);

		// Get a CLS compliant pointer from our delegate
		IntPtr ipNatCheckStunServersComplete = Marshal::GetFunctionPointerForDelegate(onNatCheckStunServersCompleteCallback);

		// Cast the pointer to the proper function ptr signature.
		OnNatCheckStunServersComplete_Function onNatCheckStunServersCompleteFunction = static_cast<OnNatCheckStunServersComplete_Function>(ipNatCheckStunServersComplete.ToPointer());



		// Assign the handler and allocate memory.
		OnNatDetectionCompleteCallback^ onNatDetectionCompleteCallback = gcnew OnNatDetectionCompleteCallback(this, &Endpoint::OnNatDetectionComplete_Handler);
		_gchOnNatDetectionComplete = GCHandle::Alloc(onNatDetectionCompleteCallback);

		// Get a CLS compliant pointer from our delegate
		IntPtr ipNatDetectionComplete = Marshal::GetFunctionPointerForDelegate(onNatDetectionCompleteCallback);

		// Cast the pointer to the proper function ptr signature.
		OnNatDetectionComplete_Function onNatDetectionCompleteFunction = static_cast<OnNatDetectionComplete_Function>(ipNatDetectionComplete.ToPointer());



		// Assign the handler and allocate memory.
		OnSelectAccountCallback^ onSelectAccountCallback = gcnew OnSelectAccountCallback(this, &Endpoint::OnSelectAccount_Handler);
		_gchOnSelectAccount = GCHandle::Alloc(onSelectAccountCallback);

		// Get a CLS compliant pointer from our delegate
		IntPtr ipSelectAccount = Marshal::GetFunctionPointerForDelegate(onSelectAccountCallback);

		// Cast the pointer to the proper function ptr signature.
		OnSelectAccount_Function onSelectAccountFunction = static_cast<OnSelectAccount_Function>(ipSelectAccount.ToPointer());



		// Assign the handler and allocate memory.
		OnTimerCallback^ onTimerCallback = gcnew OnTimerCallback(this, &Endpoint::OnTimer_Handler);
		_gchOnTimer = GCHandle::Alloc(onTimerCallback);

		// Get a CLS compliant pointer from our delegate
		IntPtr ipTimer = Marshal::GetFunctionPointerForDelegate(onTimerCallback);

		// Cast the pointer to the proper function ptr signature.
		OnTimer_Function onTimerFunction = static_cast<OnTimer_Function>(ipTimer.ToPointer());



		// Assign the handler and allocate memory.
		OnTransportStateCallback^ onTransportStateCallback = gcnew OnTransportStateCallback(this, &Endpoint::OnTransportState_Handler);
		_gchOnTransportState = GCHandle::Alloc(onTransportStateCallback);

		// Get a CLS compliant pointer from our delegate
		IntPtr ipTransportState = Marshal::GetFunctionPointerForDelegate(onTransportStateCallback);

		// Cast the pointer to the proper function ptr signature.
		OnTransportState_Function onTransportStateFunction = static_cast<OnTransportState_Function>(ipTransportState.ToPointer());



		// Set the on NatCheckStunServers native function handler.
		_endpointCallback->Set_OnNatCheckStunServersComplete_Function(onNatCheckStunServersCompleteFunction);

		// Set the on NatDetectionComplete native function handler.
		_endpointCallback->Set_OnNatDetectionComplete_Function(onNatDetectionCompleteFunction);

		// Set the on SelectAccount native function handler.
		_endpointCallback->Set_OnSelectAccount_Function(onSelectAccountFunction);

		// Set the on Timer native function handler.
		_endpointCallback->Set_OnTimer_Function(onTimerFunction);

		// Set the on TransportState native function handler.
		_endpointCallback->Set_OnTransportState_Function(onTransportStateFunction);

		// Created.
		_created = true;
	}
}

/// <summary>
/// Get all supported codecs in the system.
/// </summary>
/// <returns>The supported codecs in the system.</returns>
array<CodecInfo^>^ Endpoint::GetAudioCodecInfo()
{
	// If endpoint created.
	if (_created)
	{
		List<CodecInfo^>^ codecList = gcnew List<CodecInfo^>();
		const pj::CodecInfoVector& codecs = _endpointCallback->GetAudioCodecInfo();

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
		throw gcnew Exception(CreateEndpoint());
}

/// <summary>
/// Get all supported video codecs in the system.
/// </summary>
/// <returns>The supported video codecs in the system.</returns>
array<CodecInfo^>^ Endpoint::GetVideoCodecInfo()
{
	// If endpoint created.
	if (_created)
	{
		List<CodecInfo^>^ codecList = gcnew List<CodecInfo^>();
		const pj::CodecInfoVector& codecs = _endpointCallback->GetVideoCodecInfo();

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
		throw gcnew Exception(CreateEndpoint());
}

/// <summary>
/// Get the list of transport ids.
/// </summary>
/// <returns>The list of transport ids.</returns>
array<int>^ Endpoint::GetTransportIdList()
{
	// If endpoint created.
	if (_created)
	{
		List<int>^ transList = gcnew List<int>();
		const std::vector<int> transIDs = _endpointCallback->GetTransportIdList();

		// Get the vector size.
		size_t vectorSize = transIDs.size();

		// If devices exist.
		if (vectorSize > 0)
		{
			// For each code found.
			for (int i = 0; i < vectorSize; i++)
			{
				int transportID = transIDs[i];
				transList->Add(transportID);
			}
		}

		// Return the transport list.
		return transList->ToArray();
	}
	else
		throw gcnew Exception(CreateEndpoint());
}

/// <summary>
/// Get the transport information.
/// </summary>
/// <param name="transportID">The transport id.</param>
/// <returns>The transport information.</returns>
TransportInfo^ Endpoint::GetTransportInfo(int transportID)
{
	// If endpoint created.
	if (_created)
	{
		TransportInfo^ info = gcnew TransportInfo();

		// Get the info.
		pj::TransportInfo pjInfo = _endpointCallback->transportGetInfo(transportID);
		info->Flags = pjInfo.flags;
		info->Info = gcnew String(pjInfo.info.c_str());
		info->LocalAddress = gcnew String(pjInfo.localAddress.c_str());
		info->LocalName = gcnew String(pjInfo.localName.c_str());
		info->TransportId = pjInfo.id;
		info->TypeName = gcnew String(pjInfo.typeName.c_str());
		info->UsageCount = pjInfo.usageCount;

		String^ protocol = "IPv4";
		TransportType transportType = TransportType::UDP;
		pjsip_transport_type_e type = pjInfo.type;
		switch (type)
		{
		case PJSIP_TRANSPORT_IPV6:
			protocol = "IPv6";
			transportType = TransportType::UDP;
			break;
		case PJSIP_TRANSPORT_UDP6:
			protocol = "IPv6";
			transportType = TransportType::UDP;
			break;
		case PJSIP_TRANSPORT_TCP6:
			protocol = "IPv6";
			transportType = TransportType::TCP;
			break;
		case PJSIP_TRANSPORT_TLS6:
			protocol = "IPv6";
			transportType = TransportType::TLS;
			break;
		case PJSIP_TRANSPORT_UDP:
			transportType = TransportType::UDP;
			break;
		case PJSIP_TRANSPORT_TCP:
			transportType = TransportType::TCP;
			break;
		case PJSIP_TRANSPORT_TLS:
			transportType = TransportType::TLS;
			break;
		case PJSIP_TRANSPORT_UNSPECIFIED:
		case PJSIP_TRANSPORT_SCTP:
		case PJSIP_TRANSPORT_LOOP:
		case PJSIP_TRANSPORT_LOOP_DGRAM:
		case PJSIP_TRANSPORT_START_OTHER:
		default:
			transportType = TransportType::UDP;
			break;
		}

		info->Transport = transportType;
		info->Protocol = protocol;

		// Return the info.
		return info;
	}
	else
		throw gcnew Exception(CreateEndpoint());
}

///	<summary>
///	Change audio codec priority.
///	</summary>
/// <param name="codecID">which is a string that uniquely identify
///	the codec(such as "speex/8000").</param>
/// <param name="priority">Codec priority, 0-255, where zero means to disable
///	the codec.</param>
void Endpoint::SetPriorityAudioCodec(String^ codecID, byte priority)
{
	std::string codecIDNative;
	MarshalString(codecID, codecIDNative);

	// If endpoint created.
	if (_created)
	{
		_endpointCallback->SetPriorityAudioCodec(codecIDNative, priority);
	}
	else
		throw gcnew Exception(CreateEndpoint());
}

///	<summary>
///	Change video codec priority.
///	</summary>
/// <param name="codecID">Codec ID, which is a string that uniquely identify
///	the codec(such as "H263/90000").</param>
/// <param name="priority">Codec priority, 0-255, where zero means to disable
///	the codec.</param>
void Endpoint::SetPriorityVideoCodec(String^ codecID, byte priority)
{
	std::string codecIDNative;
	MarshalString(codecID, codecIDNative);

	// If endpoint created.
	if (_created)
	{
		_endpointCallback->SetPriorityVideoCodec(codecIDNative, priority);
	}
	else
		throw gcnew Exception(CreateEndpoint());
}

/// <summary>
/// Add audio media device to the application.
/// </summary>
/// <param name="audioMedia">The audio media device.</param>
void Endpoint::AddAudioCaptureDevice(AudioMedia^ audioMedia)
{
	// If endpoint created.
	if (_created)
	{
		_endpointCallback->AddAudioMedia(audioMedia->GetAudioMedia());
	}
	else
		throw gcnew Exception(CreateEndpoint());
}

/// <summary>
/// Add audio media device to the application.
/// </summary>
/// <param name="audioMedia">The audio media device.</param>
void Endpoint::AddAudioPlaybackDevice(AudioMedia^ audioMedia)
{
	// If endpoint created.
	if (_created)
	{
		_endpointCallback->AddAudioMedia(audioMedia->GetAudioMedia());
	}
	else
		throw gcnew Exception(CreateEndpoint());
}

/// <summary>
/// Get the number of active media ports.
/// </summary>
/// <returns>The number of active ports.</returns>
unsigned Endpoint::MediaActivePorts()
{
	// If endpoint created.
	if (_created)
	{
		return _endpointCallback->MediaActivePorts();
	}
	else
		throw gcnew Exception(CreateEndpoint());
}

/// <summary>
/// Get the media manager.
/// </summary>
/// <param name="account">The audio media device.</param>
/// <returns>The media manager.</returns>
MediaManager^ Endpoint::GetMediaManager(Account^ account)
{
	// If endpoint created.
	if (_created)
	{
		// Get the audio device manager.
		pj::AudDevManager& pjAudDevManager = _endpointCallback->GetAudioDevManager();
		pj::VidDevManager& pjVidDevManager = _endpointCallback->GetVideoDevManager();
		pj::AccountVideoConfig& pjAccountVideoConfig = account->GetAccountCallback().GetAccountVideoConfig();
		MediaManager^ mediaManager = gcnew MediaManager(pjAudDevManager, pjVidDevManager, pjAccountVideoConfig);
		return mediaManager;
	}
	else
		throw gcnew Exception(CreateEndpoint());
}

/// <summary>
/// Stop all threads.
/// </summary>
void Endpoint::StopThreads()
{
	// If endpoint created.
	if (_created)
	{
		_endpointCallback->libStopWorkerThreads();
	}
	else
		throw gcnew Exception(CreateEndpoint());
}

/// <summary>
/// Cleanup all resources.
/// </summary>
void Endpoint::Destroy()
{
	// If endpoint created.
	if (_created)
	{
		_endpointCallback->libDestroy();
	}
	else
		throw gcnew Exception(CreateEndpoint());
}

///	<summary>
///	On NatDetectionComplete function callback.
///	</summary>
/// <param name="prm">The on incoming call parameters.</param>
void Endpoint::OnNatDetectionComplete_Handler(const pj::OnNatDetectionCompleteParam &prm)
{
	// Convert the type.
	OnNatDetectionCompleteParam^ param = gcnew OnNatDetectionCompleteParam();

	param->NatType = EndpointCallback::GetStunNatTypeEx(prm.natType);
	param->NatTypeName = gcnew String(prm.natTypeName.c_str());
	param->Reason = gcnew String(prm.reason.c_str());
	param->Status = prm.status;

	// Call the event handler.
	OnNatDetectionComplete(this, param);
}

///	<summary>
///	On NatCheckStunServersComplete function callback.
///	</summary>
/// <param name="prm">The on incoming call parameters.</param>
void Endpoint::OnNatCheckStunServersComplete_Handler(const pj::OnNatCheckStunServersCompleteParam &prm)
{
	// Convert the type.
	OnNatCheckStunServersCompleteParam^ param = gcnew OnNatCheckStunServersCompleteParam();

	param->Name = gcnew String(prm.name.c_str());
	param->SocketAddress = gcnew String(prm.addr.c_str());
	param->Status = prm.status;

	// Call the event handler.
	OnNatCheckStunServersComplete(this, param);
}

///	<summary>
///	On TransportState function callback.
///	</summary>
/// <param name="prm">The on incoming call parameters.</param>
void Endpoint::OnTransportState_Handler(const pj::OnTransportStateParam &prm)
{
	// Convert the type.
	OnTransportStateParam^ param = gcnew OnTransportStateParam();

	param->LastError = prm.lastError;
	param->State = EndpointCallback::GetTransportStateEx(prm.state);

	// Call the event handler.
	OnTransportState(this, param);
}

///	<summary>
///	On Timer function callback.
///	</summary>
/// <param name="prm">The on incoming call parameters.</param>
void Endpoint::OnTimer_Handler(const pj::OnTimerParam &prm)
{
	// Convert the type.
	OnTimerParam^ param = gcnew OnTimerParam();
	
	param->MiliisecondDelay = prm.msecDelay;

	// Call the event handler.
	OnTimer(this, param);
}

///	<summary>
///	On SelectAccount function callback.
///	</summary>
/// <param name="prm">The on incoming call parameters.</param>
void Endpoint::OnSelectAccount_Handler(pj::OnSelectAccountParam &prm)
{
	// Convert the type.
	OnSelectAccountParam^ param = gcnew OnSelectAccountParam();
	param->RxData = gcnew SipRxData();

	param->AccountIndex = prm.accountIndex;

	param->RxData->Info = gcnew String(prm.rdata.info.c_str());
	param->RxData->SrcAddress = gcnew String(prm.rdata.srcAddress.c_str());
	param->RxData->WholeMsg = gcnew String(prm.rdata.wholeMsg.c_str());

	// Call the event handler.
	OnSelectAccount(this, param);
}