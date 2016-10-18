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

namespace Nequeo.Net.Http2.Protocol
{
    /*
    +-+-------------------------------------------------------------+
    |R|              Window Size Increment (31)                     |
    +-+-------------------------------------------------------------+
    */


    /// <summary>
    /// Window update class
    /// See spec: http://tools.ietf.org/html/draft-ietf-httpbis-http2-04#section-6.9
    /// </summary>
    internal class WindowUpdateFrame : Frame
    {
        /// <summary>
        /// Window update class
        /// </summary>
        /// <param name="preamble">The incoming frame.</param>
        public WindowUpdateFrame(Frame preamble)
            : base(preamble)
        {
        }

        /// <summary>
        /// Window update class
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="delta">The delta.</param>
        public WindowUpdateFrame(int id, int delta)
            : base(new byte[InitialFrameSize])
        {
            StreamId = id;
            FrameType = OpCodeFrame.Window_Update;
            PayloadLength = InitialFrameSize - Constants.FramePreambleSize; // 8
            Delta = delta;
        }

        private const int InitialFrameSize = 12;

        /// <summary>
        /// Gets or sets the delta.
        /// </summary>
        public int Delta
        {
            get { return FrameHelper.Get31BitsAt(Buffer, Constants.FramePreambleSize); }
            set { FrameHelper.Set31BitsAt(Buffer, Constants.FramePreambleSize, value); }
        }
    }
}
