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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Http2.Protocol
{
    /*
    +-+-------------------------------------------------------------+
    |E|                  Stream Dependency (31)                     |
    +-+-------------+-----------------------------------------------+
    |   Weight (8)  |
    +-+-------------+
    */


    /// <summary>
    /// PRIORITY frame class
    /// see 12 -> 6.3
    /// </summary>
    internal class PriorityFrame : Frame
    {
        /// <summary>
        /// PRIORITY frame class
        /// </summary>
        /// <param name="preamble">The incoming frame.</param>
        public PriorityFrame(Frame preamble)
            : base(preamble)
        {
        }

        /// <summary>
        /// PRIORITY frame class
        /// </summary>
        /// <param name="streamId">Stream id.</param>
        /// <param name="streamDependency">Stream dependency.</param>
        /// <param name="isExclusive">Is exclusive.</param>
        /// <param name="weight">Weight.</param>
        public PriorityFrame(int streamId, int streamDependency, bool isExclusive, byte weight)
        {
            Contract.Assert(streamId != 0);

            // construct frame
            Buffer = new byte[Constants.FramePreambleSize + DependencyLength + WeightLength];

            StreamId = streamId;
            FrameType = OpCodeFrame.Priority;
            Exclusive = isExclusive;
            StreamDependency = streamDependency;
            Weight = weight;
        }

        // 4 bytes Stream Dependency field
        private const int DependencyLength = 4;

        // 1 byte Weight field
        private const int WeightLength = 1;

        /// <summary>
        /// Gets or sets is exclusive.
        /// </summary>
        public bool Exclusive
        {
            get
            {
                return FrameHelper.GetBit(Buffer[Constants.FramePreambleSize], 7);
            }
            set
            {
                FrameHelper.SetBit(ref Buffer[Constants.FramePreambleSize], value, 7);
            }
        }

        /// <summary>
        /// Gets or sets stream dependency.
        /// </summary>
        public int StreamDependency
        {
            get
            {
                return FrameHelper.Get31BitsAt(Buffer, Constants.FramePreambleSize);
            }
            set
            {
                FrameHelper.Set31BitsAt(Buffer, Constants.FramePreambleSize, value);
            }
        }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        public byte Weight
        {
            get
            {
                return Buffer[Constants.FramePreambleSize + DependencyLength];
            }
            set
            {
                Buffer[Constants.FramePreambleSize + DependencyLength] = value;
            }
        }
    }
}
