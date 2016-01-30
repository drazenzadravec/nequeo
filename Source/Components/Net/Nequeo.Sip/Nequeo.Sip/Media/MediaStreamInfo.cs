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
    /// Media stream info.
    /// </summary>
    public class MediaStreamInfo
    {
        /// <summary>
        /// Media stream info.
        /// </summary>
        public MediaStreamInfo() { }

        /// <summary>
        /// Gets or sets the media type.
        /// </summary>
        public MediaType Type { get; set; }

        /// <summary>
        /// Gets or sets the transport protocol (RTP/AVP, etc.)
        /// </summary>
        public MediaTransportProtocol TransportProtocol { get; set; }

        /// <summary>
        /// Gets or sets the media direction.
        /// </summary>
        public MediaDirection Direction { get; set; }

        /// <summary>
        /// Gets or sets the remote RTP address.
        /// </summary>
        public string RemoteRtpAddress { get; set; }

        /// <summary>
        /// Gets or sets the optional remote RTCP address.
        /// </summary>
        public string RemoteRtcpAddress { get; set; }

        /// <summary>
        /// Gets or sets the outgoing codec payload type.
        /// </summary>
        public uint TxPayloadType { get; set; }

        /// <summary>
        /// Gets or sets the incoming codec payload type.
        /// </summary>
        public uint RxPayloadType { get; set; }

        /// <summary>
        /// Gets or sets the codec name.
        /// </summary>
        public string CodecName { get; set; }

        /// <summary>
        /// Gets or sets the codec clock rate.
        /// </summary>
        public uint CodecClockRate { get; set; }
    }
}
