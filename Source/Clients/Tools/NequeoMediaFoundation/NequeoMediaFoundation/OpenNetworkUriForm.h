/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OpenNetworkUriForm.h
*  Purpose :       OpenNetworkUriForm class.
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

#ifndef _OPENNETWORKURIFORM_H
#define _OPENNETWORKURIFORM_H

#include "MediaGlobal.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Open network URI form.
			/// </summary>
			class OpenNetworkUriForm : public CDialog
			{
				/// <summary>
				/// Open network URI window enum.
				/// </summary>
				enum
				{
					/// <summary>
					/// The id of the network URI window.
					/// </summary>
					IDD = IDD_OPENNETWORKURI
				};

			public:
				/// <summary>
				/// Constructor for the current class.
				/// </summary>
				/// <param name="pParent">The parent window.</param>
				OpenNetworkUriForm(CWnd* pParent = NULL);

				/// <summary>
				/// This destructor.
				/// </summary>
				~OpenNetworkUriForm();

				/// <summary>
				/// Gets the network URL.
				/// </summary>
				/// <returns>The URL text.</returns>
				string GetNetworkURL() const;

			protected:
				/// <summary>
				/// Data exchange.
				/// </summary>
				/// <param name="pDX">Data exchange instance.</param>
				virtual void DoDataExchange(CDataExchange* pDX);

			private:
				bool _disposed;
				string _networkURL;

				// Window controls
				CToolTipCtrl* _toolTip;
				CEdit _textURL;

			public:
				/// <summary>
				/// Declare the message map.
				/// </summary>
				DECLARE_MESSAGE_MAP()
				afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
				afx_msg void OnBnClickedCancel();
				afx_msg void OnBnClickedOk();
				afx_msg void OnEnChangeEditNetworUuri();
				afx_msg void OnShowWindow(BOOL bShow, UINT nStatus);
				
			};
		}
	}
}
#endif