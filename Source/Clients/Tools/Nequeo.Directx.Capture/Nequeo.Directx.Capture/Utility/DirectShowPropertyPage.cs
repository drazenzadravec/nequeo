/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
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
#endregion

using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using System.Linq;
using System.Text;

using DirectShowLib;
using DirectShowLib.BDA;
using DirectShowLib.DES;
using DirectShowLib.DMO;
using DirectShowLib.Dvd;
using DirectShowLib.MultimediaStreaming;
using DirectShowLib.SBE;

namespace Nequeo.Directx.Utility
{
    /// <summary>
    /// Property pages for a DirectShow filter (e.g. hardware device). These
    /// property pages do not support persisting their settings. 
    /// </summary>
    public class DirectShowPropertyPage : PropertyPage
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="specifyPropertyPages">The specidy property page.</param>
        public DirectShowPropertyPage(string name, ISpecifyPropertyPages specifyPropertyPages)
        {
            Name = name;
            SupportsPersisting = false;
            this.specifyPropertyPages = specifyPropertyPages;
        }

        /// <summary>
        /// Create a property frame.
        /// </summary>
        /// <param name="hwndOwner"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="lpszCaption"></param>
        /// <param name="cObjects"></param>
        /// <param name="ppUnk"></param>
        /// <param name="cPages"></param>
        /// <param name="pPageClsID"></param>
        /// <param name="lcid"></param>
        /// <param name="dwReserved"></param>
        /// <param name="pvReserved"></param>
        /// <returns></returns>
        [DllImport("olepro32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int OleCreatePropertyFrame(
            IntPtr hwndOwner, int x, int y,
            string lpszCaption, int cObjects,
            [In, MarshalAs(UnmanagedType.Interface)] ref object ppUnk,
            int cPages, IntPtr pPageClsID, int lcid, int dwReserved, IntPtr pvReserved);

        /// <summary>
        /// COM ISpecifyPropertyPages interface.
        /// </summary>
        protected ISpecifyPropertyPages specifyPropertyPages;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        public override void Show(Control owner)
        {
            DsCAUUID cauuid = new DsCAUUID();
            try
            {
                int hr = specifyPropertyPages.GetPages(out cauuid);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);

                object o = specifyPropertyPages;
                hr = OleCreatePropertyFrame(owner.Handle, 30, 30, null, 1,
                    ref o, cauuid.cElems, cauuid.pElems, 0, 0, IntPtr.Zero);
            }
            finally
            {
                if (cauuid.pElems != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(cauuid.pElems);
            }
        }

        /// <summary>
        /// Release unmanaged resources.
        /// </summary>
        public new void Dispose()
        {
            if (specifyPropertyPages != null)
                Marshal.ReleaseComObject(specifyPropertyPages); specifyPropertyPages = null;
        }
    }
}
