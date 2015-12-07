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
    ///  The property page to configure a Video for Windows compliant
    ///  compression codec. Most compressors support this property page
    ///  rather than a DirectShow property page. Also, most compressors
    ///  do not support the IAMVideoCompression interface so this
    ///  property page is the only method to configure a compressor. 
    /// </summary>
    public class VfwCompressorPropertyPage : PropertyPage
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name compress dialog.</param>
        /// <param name="compressDialogs"></param>
        public VfwCompressorPropertyPage(string name, IAMVfwCompressDialogs compressDialogs)
        {
            Name = name;
            SupportsPersisting = true;
            this.vfwCompressDialogs = compressDialogs;
        }

        /// <summary>
        /// Compress dialog.
        /// </summary>
        protected IAMVfwCompressDialogs vfwCompressDialogs = null;

        /// <summary> 
        ///  Get or set the state of the property page. This is used to save
        ///  and restore the user's choices without redisplaying the property page.
        ///  This property will be null if unable to retrieve the property page's
        ///  state.
        /// </summary>
        /// <remarks>
        ///  After showing this property page, read and store the value of 
        ///  this property. At a later time, the user's choices can be 
        ///  reloaded by setting this property with the value stored earlier. 
        ///  Note that some property pages, after setting this property, 
        ///  will not reflect the new state. However, the filter will use the
        ///  new settings.
        /// </remarks>
        public override byte[] State
        {
            get
            {
                byte[] data = null;
                int size = 0;

                int hr = vfwCompressDialogs.GetState(System.IntPtr.Zero, ref size);
                if ((hr == 0) && (size > 0))
                {
                    data = new byte[size];

                    // Allocate the memory space.
                    IntPtr unmanagedPointer = Marshal.AllocHGlobal(data.Length);

                    // Load the data into the memory space.
                    hr = vfwCompressDialogs.GetState(unmanagedPointer, ref size);

                    // Copy the memory data to the data byte array.
                    Marshal.Copy(unmanagedPointer, data, 0, size);
                    Marshal.FreeHGlobal(unmanagedPointer);

                    if (hr != 0) data = null;
                }
                return (data);
            }
            set
            {
                byte[] data = value;
                int size = data.Length;

                // Allocate the memory space.
                IntPtr unmanagedPointer = Marshal.AllocHGlobal(data.Length);
                Marshal.Copy(data, 0, unmanagedPointer, size);

                int hr = vfwCompressDialogs.SetState(unmanagedPointer, value.Length);
                Marshal.FreeHGlobal(unmanagedPointer);

                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
            }
        }

        /// <summary> 
        ///  Show the property page. Some property pages cannot be displayed 
        ///  while previewing and/or capturing. 
        /// </summary>
        public override void Show(Control owner)
        {
            vfwCompressDialogs.ShowDialog(VfwCompressDialogs.Config, owner.Handle);
        }
    }
}
