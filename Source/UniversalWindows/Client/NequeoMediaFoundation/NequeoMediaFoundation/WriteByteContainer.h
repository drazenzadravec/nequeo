/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WriteByteContainer.h
*  Purpose :       WriteByteContainer class.
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

#ifndef _WRITEBYTECONTAINER_H
#define _WRITEBYTECONTAINER_H

#include "MediaGlobal.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Providers the base for a media foundation write byte stream.
			/// </summary>
			class WriteByteContainer : public IUnknown
			{
			public:
				/// <summary>
				/// Constructor for the current class.
				/// </summary>
				/// <param name="pb">Pointer to a buffer containing the data to write.</param>
				/// <param name="cb">Size of the buffer in bytes.</param>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API WriteByteContainer(const BYTE *pb, ULONG cb);

				/// <summary>
				/// This destructor. Call release to cleanup resources.
				/// </summary>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API virtual ~WriteByteContainer();

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

				const BYTE *_pb;
				ULONG _cb;
				ULONG _pcbWritten;
				IMFAsyncCallback* _writeCallback;

			private:
				bool					_disposed;
				long                    _nRefCount;			// Reference count.

			};
		}
	}
}
#endif