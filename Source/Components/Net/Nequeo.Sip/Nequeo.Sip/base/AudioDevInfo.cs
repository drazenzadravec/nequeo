/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Sip
{
    /// <summary>
    /// Audio device info.
    /// </summary>
    public class AudioDeviceInfo
    {
        /// <summary>
        /// Audio device info.
        /// </summary>
        public AudioDeviceInfo() { }

        /// <summary>
        /// Gets or sets the device name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get or sets the maximum number of input channels supported by this device. If the
        /// value is zero, the device does not support input operation (i.e. it is a playback only device).
        /// </summary>
        public uint InputCount { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of output channels supported by this device. If the
        /// value is zero, the device does not support output operation(i.e. it is an input only device).
        /// </summary>
        public uint OutputCount { get; set; }

        /// <summary>
        /// Gets or sets the default sampling rate.
        /// </summary>
        public uint DefaultSamplesPerSec { get; set; }

        /// <summary>
        /// Gets or sets the underlying driver name.
        /// </summary>
        public string Driver { get; set; }

        /// <summary>
        /// Gets or sets the device capabilities, as bitmask combination.
        /// </summary>
        public uint Caps { get; set; }

        /// <summary>
        /// Gets or sets the supported audio device routes, as bitmask combination
        /// The value may be zero if the device does not support audio routing.
        /// </summary>
        public uint Routes { get; set; }

        /// <summary>
        /// Gets or sets the array of media formats.
        /// </summary>
        public MediaFormat[] MediaFormats { get; set; }
    }
}
