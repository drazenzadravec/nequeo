/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WriteByteAsyncCallback.h
*  Purpose :       WriteByteAsyncCallback class.
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

#ifndef _WRITEBYTEASYNCCALLBACK_H
#define _WRITEBYTEASYNCCALLBACK_H

#include "MediaGlobal.h"
#include "WriteByteContainer.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Providers the base for a media foundation write byte stream async callback.
			/// </summary>
			class WriteByteAsyncCallback : public IMFAsyncCallback
			{
			public:
				/// <summary>
				/// Constructor for the current class.
				/// </summary>
				/// <param name="byteStream">The byte stream to invoke write implementation on.</param>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API WriteByteAsyncCallback(IMFByteStream* byteStream);

				/// <summary>
				/// This destructor. Call release to cleanup resources.
				/// </summary>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API virtual ~WriteByteAsyncCallback();

				/// <summary>
				/// Add a new player ref item.
				/// </summary>
				/// <returns>The result.</returns>
				STDMETHODIMP_(ULONG) AddRef();

				/// <summary>
				/// Release this player resources.
				/// </summary>
				/// <returns>The result.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(ULONG) Release();

				/// <summary>
				/// Get the player reference for the reference id.
				/// </summary>
				/// <param name="iid">The player reference id.</param>
				/// <param name="ppv">The current player reference.</param>
				/// <returns>The result of the operation.</returns>
				virtual STDMETHODIMP QueryInterface(REFIID iid, void** ppv);

				/// <summary>
				/// Provides configuration information to the dispatching thread for a callback.
				/// </summary>
				/// <param name="pdwFlags">Receives a flag indicating the behavior of the callback object's IMFAsyncCallback::Invoke method. 
				/// The following values are defined. The default value is zero. [in]</param>
				/// <param name="pdwQueue">Receives the identifier of the work queue on which the callback is dispatched. [out]</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP GetParameters(DWORD* pdwFlags, DWORD* pdwQueue)
				{
					// Implementation of this method is optional.
					return E_NOTIMPL;
				}

				/// <summary>
				/// Called when an asynchronous operation is completed.
				/// </summary>
				/// <param name="pAsyncResult">Pointer to the IMFAsyncResult interface. Pass this pointer to the asynchronous End... method to complete the asynchronous call.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP Invoke(IMFAsyncResult* pAsyncResult);

			private:
				bool					_disposed;
				long                    _nRefCount;			// Reference count.

				IMFByteStream			*_byteStream;
				CRITICAL_SECTION        _critsec;
			};
		}
	}
}
#endif