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
    /// Call media information.
    /// </summary>
    public class CallMediaInfo
    {
        /// <summary>
        /// Call media information.
        /// </summary>
        public CallMediaInfo() { }

        /// <summary>
        /// Gets or sets the media index in SDP.
        /// </summary>
        public uint Index { get; set; }

        /// <summary>
        /// Gets or sets the media type.
        /// </summary>
        public MediaType Type { get; set; }

        /// <summary>
        /// Gets or sets the media direction.
        /// </summary>
        public MediaDirection Direction { get; set; }

        /// <summary>
        /// Gets or sets the call media status.
        /// </summary>
        public CallMediaStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the conference port number for the call. Only valid if the media type is audio.
        /// </summary>
        public int AudioConfSlot { get; set; }

        ///	<summary>
        ///	Gets or sets the window id for incoming video, if any, or
        /// PJSUA_INVALID_ID. Only valid if the media type is video.
        ///	</summary>
        public int VideoIncomingWindowId { get; set; }

        ///	<summary>
        ///	Gets or sets the video capture device for outgoing transmission, if any,
        /// or PJMEDIA_VID_INVALID_DEV. Only valid if the media type is video.
        ///	</summary>
        public int VideoCapDev { get; set; }
    }
}
