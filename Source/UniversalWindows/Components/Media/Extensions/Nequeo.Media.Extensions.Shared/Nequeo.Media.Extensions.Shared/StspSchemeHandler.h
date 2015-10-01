/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :
*  Purpose :
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

namespace Nequeo { namespace Media { namespace Communication {

class CSchemeHandler
    : public Microsoft::WRL::RuntimeClass<
           Microsoft::WRL::RuntimeClassFlags< Microsoft::WRL::RuntimeClassType::WinRtClassicComMix >, 
           ABI::Windows::Media::IMediaExtension,
           IMFSchemeHandler >
{
    InspectableClass(L"Microsoft.Samples.SimpleCommunication.StspSchemeHandler",BaseTrust)

public:
    CSchemeHandler(void);
    ~CSchemeHandler(void);

    // IMediaExtension
    IFACEMETHOD (SetProperties) (ABI::Windows::Foundation::Collections::IPropertySet *pConfiguration);

    // IMFSchemeHandler
    IFACEMETHOD (BeginCreateObject) ( 
            _In_ LPCWSTR pwszURL,
            _In_ DWORD dwFlags,
            _In_ IPropertyStore *pProps,
            _COM_Outptr_opt_  IUnknown **ppIUnknownCancelCookie,
            _In_ IMFAsyncCallback *pCallback,
            _In_ IUnknown *punkState);
        
    IFACEMETHOD (EndCreateObject) ( 
            _In_ IMFAsyncResult *pResult,
            _Out_  MF_OBJECT_TYPE *pObjectType,
            _Out_  IUnknown **ppObject);
        
    IFACEMETHOD (CancelObjectCreation) ( 
            _In_ IUnknown *pIUnknownCancelCookie);
};

}}} // namespace Microsoft::Samples::SimpleCommunication
