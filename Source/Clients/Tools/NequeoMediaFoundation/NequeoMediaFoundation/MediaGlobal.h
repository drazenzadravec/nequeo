/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaGlobal.h
*  Purpose :       MediaGlobal class.
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

#ifndef _MEDIAGLOBAL_H
#define _MEDIAGLOBAL_H

#include "stdafx.h"

#include <stdio.h>
#include <stdlib.h>
#include <tchar.h>
#include <string>

using namespace std;

// Private window message to notify the application of playback events.
const UINT WM_APP_NOTIFY = WM_APP + 1;   // wparam = state

// Private window message to notify the application when an error occurs.
const UINT WM_APP_ERROR = WM_APP + 2;    // wparam = HRESULT

// SAFE_RELEASE macro.
// Releases a COM pointer if the pointer is not NULL, and sets the pointer to NULL.
#ifndef SAFE_RELEASE
#define SAFE_RELEASE(x) if (x) { x->Release(); x = NULL; }
#endif


// SAFE_DELETE macro.
// Deletes a pointer allocated with new.
#ifndef SAFE_DELETE
#define SAFE_DELETE(x) if (x) { delete x; x = NULL; }
#endif

// SAFE_ARRAY_DELETE macro.
// Deletes an array allocated with new [].
#ifndef SAFE_ARRAY_DELETE
#define SAFE_ARRAY_DELETE(x) if (x) { delete [] x; x = NULL; }
#endif

// IMPORTANT: No function here can return a NULL pointer - caller assumes
// the return value is a valid null-terminated string. You should only
// use these functions for debugging purposes.

// Event type name macro.
#define NAME(x) case x: return L#x

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Media Foundation event names (subset).
			/// </summary>
			/// <param name="met">The media event type.</param>
			/// <returns>The string name.</returns>
			inline const WCHAR* EventName(MediaEventType met)
			{
				switch (met)
				{
					NAME(MEError);
					NAME(MEExtendedType);
					NAME(MESessionTopologySet);
					NAME(MESessionTopologiesCleared);
					NAME(MESessionStarted);
					NAME(MESessionPaused);
					NAME(MESessionStopped);
					NAME(MESessionClosed);
					NAME(MESessionEnded);
					NAME(MESessionRateChanged);
					NAME(MESessionScrubSampleComplete);
					NAME(MESessionCapabilitiesChanged);
					NAME(MESessionTopologyStatus);
					NAME(MESessionNotifyPresentationTime);
					NAME(MENewPresentation);
					NAME(MELicenseAcquisitionStart);
					NAME(MELicenseAcquisitionCompleted);
					NAME(MEIndividualizationStart);
					NAME(MEIndividualizationCompleted);
					NAME(MEEnablerProgress);
					NAME(MEEnablerCompleted);
					NAME(MEPolicyError);
					NAME(MEPolicyReport);
					NAME(MEBufferingStarted);
					NAME(MEBufferingStopped);
					NAME(MEConnectStart);
					NAME(MEConnectEnd);
					NAME(MEReconnectStart);
					NAME(MEReconnectEnd);
					NAME(MEAudioSessionNameChanged);
					NAME(MEAudioSessionVolumeChanged);
					NAME(MEAudioSessionDeviceRemoved);
					NAME(MEAudioSessionServerShutdown);
					NAME(MEAudioSessionGroupingParamChanged);
					NAME(MEAudioSessionIconChanged);
					NAME(MEPolicyChanged);
					NAME(MEContentProtectionMessage);
					NAME(MEPolicySet);

				default:
					return L"Unknown event type";
				}
			}

			/// <summary>
			/// Names of VARIANT data types. 
			/// </summary>
			/// <param name="prop">The property variant.</param>
			/// <returns>The string name.</returns>
			inline const WCHAR* VariantTypeName(const PROPVARIANT& prop)
			{
				switch (prop.vt & VT_TYPEMASK)
				{
					NAME(VT_EMPTY);
					NAME(VT_NULL);
					NAME(VT_I2);
					NAME(VT_I4);
					NAME(VT_R4);
					NAME(VT_R8);
					NAME(VT_CY);
					NAME(VT_DATE);
					NAME(VT_BSTR);
					NAME(VT_DISPATCH);
					NAME(VT_ERROR);
					NAME(VT_BOOL);
					NAME(VT_VARIANT);
					NAME(VT_UNKNOWN);
					NAME(VT_DECIMAL);
					NAME(VT_I1);
					NAME(VT_UI1);
					NAME(VT_UI2);
					NAME(VT_UI4);
					NAME(VT_I8);
					NAME(VT_UI8);
					NAME(VT_INT);
					NAME(VT_UINT);
					NAME(VT_VOID);
					NAME(VT_HRESULT);
					NAME(VT_PTR);
					NAME(VT_SAFEARRAY);
					NAME(VT_CARRAY);
					NAME(VT_USERDEFINED);
					NAME(VT_LPSTR);
					NAME(VT_LPWSTR);
					NAME(VT_RECORD);
					NAME(VT_INT_PTR);
					NAME(VT_UINT_PTR);
					NAME(VT_FILETIME);
					NAME(VT_BLOB);
					NAME(VT_STREAM);
					NAME(VT_STORAGE);
					NAME(VT_STREAMED_OBJECT);
					NAME(VT_STORED_OBJECT);
					NAME(VT_BLOB_OBJECT);
					NAME(VT_CF);
					NAME(VT_CLSID);
					NAME(VT_VERSIONED_STREAM);
				default:
					return L"Unknown VARIANT type";
				}
			}

			/// <summary>
			/// Topology Node Type Name.
			/// </summary>
			/// <param name="nodeType">The node type.</param>
			/// <returns>The string name.</returns>
			inline const WCHAR* TopologyNodeTypeName(MF_TOPOLOGY_TYPE nodeType)
			{
				switch (nodeType)
				{
					NAME(MF_TOPOLOGY_OUTPUT_NODE);
					NAME(MF_TOPOLOGY_SOURCESTREAM_NODE);
					NAME(MF_TOPOLOGY_TRANSFORM_NODE);
					NAME(MF_TOPOLOGY_TEE_NODE);
				default:
					return L"Unknown node type";
				}
			}
		}
	}
}
#endif