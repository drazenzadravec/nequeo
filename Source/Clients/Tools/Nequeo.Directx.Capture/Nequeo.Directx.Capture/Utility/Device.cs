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
    /// Represents a DirectShow filter (e.g. video capture device, compression codec).
    /// </summary>
    public class Device 
    {
		/// <summary>
        /// Create a new filter from its moniker
		/// </summary>
        /// <param name="dsDevice"></param>
        public Device(DsDevice dsDevice)
		{
            _dsDevice = dsDevice;
		}

        private DsDevice _dsDevice;

        /// <summary>
        /// Gets the DS Device.
        /// </summary>
        public DsDevice DsDevice
        {
            get { return _dsDevice; }
        }

        /// <summary>
        /// Retrieve the a moniker's display name (i.e. it's unique string)
        /// </summary>
        /// <param name="moniker">The moniker interface.</param>
        /// <returns>The moniker string value.</returns>
        private string GetMonikerString(IMoniker moniker)
        {
            string s;
            moniker.GetDisplayName(null, null, out s);
            return (s);
        }

        /// <summary>
        /// Retrieve the human-readable name of the filter
        /// </summary>
        /// <param name="moniker">The moniker interface.</param>
        /// <returns>The name of the device.</returns>
        private string GetName(IMoniker moniker)
        {
            object bagObj = null;
            IPropertyBag bag = null;
            try
            {
                Guid bagId = typeof(IPropertyBag).GUID;
                moniker.BindToStorage(null, null, ref bagId, out bagObj);
                bag = (IPropertyBag)bagObj;
                object val = "";
                int hr = bag.Read("FriendlyName", out val, null);
                if (hr != 0)
                    Marshal.ThrowExceptionForHR(hr);
                string ret = val as string;
                if ((ret == null) || (ret.Length < 1))
                    throw new NotImplementedException("Device FriendlyName");
                return (ret);
            }
            catch (Exception)
            {
                return ("");
            }
            finally
            {
                bag = null;
                if (bagObj != null)
                    Marshal.ReleaseComObject(bagObj); bagObj = null;
            }
        }
    }
}
