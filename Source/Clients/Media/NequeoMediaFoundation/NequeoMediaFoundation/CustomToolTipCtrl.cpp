/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          CustomToolTipCtrl.cpp
*  Purpose :       CustomToolTipCtrl class.
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

#include "CustomToolTipCtrl.h"

/// <summary>
/// Begin messsage map.
/// </summary>
BEGIN_MESSAGE_MAP(Nequeo::Media::Foundation::CustomToolTipCtrl, CMFCToolTipCtrl)
	ON_NOTIFY_REFLECT(TTN_SHOW, &CustomToolTipCtrl::OnShow)
END_MESSAGE_MAP()

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Constructor for the current class.
			/// </summary>
			/// <param name="pParent">The parent window.</param>
			CustomToolTipCtrl::CustomToolTipCtrl()
			{
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			CustomToolTipCtrl::~CustomToolTipCtrl()
			{
				
			}

			void CustomToolTipCtrl::OnShow(NMHDR* pNMHDR, LRESULT* pResult)
			{
				m_nCurrID = CWnd::FromHandle((HWND)pNMHDR->idFrom)->GetDlgCtrlID();

				switch (m_nCurrID)
				{
				case IDOK:
					SetDescription(_T("OK Button description..."));
					break;

				case IDCANCEL:
					SetDescription(_T("Cancel Button description..."));
					break;

				default:
					SetDescription(_T(""));
				}

				CMFCToolTipCtrl::OnShow(pNMHDR, pResult);
			}
		}
	}
}