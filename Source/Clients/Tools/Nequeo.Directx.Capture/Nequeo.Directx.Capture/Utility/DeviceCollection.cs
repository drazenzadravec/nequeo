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
    ///	A collection of Filter objects (DirectShow filters).
    ///	This is used by the <see cref="Capture"/> class to provide
    ///	lists of capture devices and compression filters. This class
    ///	cannot be created directly.
    /// </summary>
    public class DeviceCollection : CollectionBase
    {
        /// <summary>
        /// Populate the collection with a list of filters from a particular category.
        /// </summary>
        /// <param name="category">The Filter Category.</param>
        public DeviceCollection(Guid category)
		{
            GetDevices(category);
		}

        /// <summary>
        /// Populate the InnerList with a list of filters from a particular category
        /// </summary>
        /// <param name="category">The Filter Category.</param>
        private void GetDevices(Guid category)
        {
            DsDevice[] deviceList;

            // Get the collection of devices
            deviceList = DsDevice.GetDevicesOfCat(category);

            if(deviceList != null)
                if(deviceList.Count() > 0)
                    foreach (DsDevice item in deviceList)
                    {
                        // Add the filter
                        Device filter = new Device(item);
                        InnerList.Add(filter);
                    }
        }

        /// <summary>
        /// Get the filter at the specified index.
        /// </summary>
        /// <param name="index">The current index of the device.</param>
        /// <returns>The selected device.</returns>
        public Device this[int index]
        {
            get { return ((Device)InnerList[index]); }
        }
    }
}
