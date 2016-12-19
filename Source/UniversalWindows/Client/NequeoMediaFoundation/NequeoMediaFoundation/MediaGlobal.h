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

#include "pch.h"

#include <stdio.h>
#include <stdlib.h>
#include <tchar.h>
#include <string>
#include <thread>

#ifdef NEQUEOMEDIAFOUNDATION_EXPORTS
#define EXPORT_NEQUEO_MEDIA_FOUNDATION_API __declspec(dllexport) 
#else
#define EXPORT_NEQUEO_MEDIA_FOUNDATION_API __declspec(dllimport) 
#endif

using namespace std;

#define CMD_PENDING      0x01
#define CMD_PENDING_SEEK 0x02
#define CMD_PENDING_RATE 0x04

// {2715279F-4139-4ba0-9CB1-B351F1B58A4A}
static const GUID AudioSessionVolumeCtx = { 0x2715279f, 0x4139, 0x4ba0, { 0x9c, 0xb1, 0xb3, 0x51, 0xf1, 0xb5, 0x8a, 0x4a } };

// SAFE_RELEASE macro.
// Releases a COM pointer if the pointer is not NULL, and sets the pointer to NULL.
#ifndef SAFE_RELEASE
#define SAFE_RELEASE(x) if (x != NULL) { x->Release(); x = NULL; }
#endif


// SAFE_DELETE macro.
// Deletes a pointer allocated with new.
#ifndef SAFE_DELETE
#define SAFE_DELETE(x) if (x != NULL) { delete x; x = NULL; }
#endif

// SAFE_ARRAY_DELETE macro.
// Deletes an array allocated with new [].
#ifndef SAFE_ARRAY_DELETE
#define SAFE_ARRAY_DELETE(x) if (x != NULL) { delete [] x; x = NULL; }
#endif

// Safe release.
template <class T> void SafeRelease(T **ppT)
{
	if (*ppT)
	{
		(*ppT)->Release();
		*ppT = NULL;
	}
}

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
		}
	}
}
#endif