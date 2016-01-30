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
    /// Media stream statistic.
    /// </summary>
    public class MediaStreamStat
    {
        /// <summary>
        /// Media stream statistic.
        /// </summary>
        public MediaStreamStat() { }

        /// <summary>
        /// Gets or sets the time when session was created.
        /// </summary>
        public TimeVal Start { get; set; }

        /// <summary>
        /// Gets or sets the last TX RTP timestamp.
        /// </summary>
        public uint RtpTxLastTs { get; set; }

        /// <summary>
        /// Gets or sets the last TX RTP sequence.
        /// </summary>
        public ushort RtpTxLastSeq { get; set; }

        /// <summary>
        /// Gets or sets the Individual frame size, in bytes.
        /// </summary>
        public uint FrameSize { get; set; }

        ///	<summary>
        ///	Gets or sets the Minimum allowed prefetch, in frms. 
        ///	</summary>
        public uint MinPrefetch { get; set; }

        ///	<summary>
        ///	Gets or sets the Maximum allowed prefetch, in frms. 
        ///	</summary>
        public uint MaxPrefetch { get; set; }

        ///	<summary>
        ///	Gets or sets the Current burst level, in frames.
        ///	</summary>
        public uint Burst { get; set; }

        ///	<summary>
        ///	Gets or sets the Current prefetch value, in frames.
        ///	</summary>
        public uint Prefetch { get; set; }

        ///	<summary>
        ///	Gets or sets the Current buffer size, in frames.
        ///	</summary>
        public uint Size { get; set; }

        ///	<summary>
        ///	Gets or sets the Average delay, in ms.
        ///	</summary>
        public uint AvgDelayMsec { get; set; }

        ///	<summary>
        ///	Gets or sets the Minimum delay, in ms.
        ///	</summary>
        public uint MinDelayMsec { get; set; }

        ///	<summary>
        ///	Gets or sets the Maximum delay, in ms.
        ///	</summary>
        public uint MaxDelayMsec { get; set; }

        ///	<summary>
        ///	Gets or sets the Standard deviation of delay, in ms.
        ///	</summary>
        public uint DevDelayMsec { get; set; }

        ///	<summary>
        ///	Gets or sets the Average burst, in frames.
        ///	</summary>
        public uint AvgBurst { get; set; }

        ///	<summary>
        ///	Gets or sets the Number of lost frames.
        ///	</summary>
        public uint Lost { get; set; }

        ///	<summary>
        ///	Gets or sets the Number of discarded frames.
        ///	</summary>
        public uint Discard { get; set; }

        ///	<summary>
        ///	Gets or sets the Number of empty on GET events.
        ///	</summary>
        public uint Empty { get; set; }
    }
}
