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
using System.Drawing;

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
    /// Capabilities of the video device such as 
    /// min/max frame size and frame rate.
    /// </summary>
    public class VideoCapabilities
    {
        /// <summary>
        /// Native size of the incoming video signal. This is the largest signal the filter can digitize with every pixel remaining unique.
        /// </summary>
        public Size InputSize;

        /// <summary>
        /// Minimum supported frame size.
        /// </summary>
        public Size MinFrameSize;

        /// <summary>
        /// Maximum supported frame size.
        /// </summary>
        public Size MaxFrameSize;

        /// <summary>
        /// Granularity of the output width. This value specifies the increments that are valid between MinFrameSize and MaxFrameSize.
        /// </summary>
        public int FrameSizeGranularityX;

        /// <summary>
        /// Granularity of the output height. This value specifies the increments that are valid between MinFrameSize and MaxFrameSize.
        /// </summary>
        public int FrameSizeGranularityY;

        /// <summary>
        /// Minimum supported frame rate.
        /// </summary>
        public double MinFrameRate;

        /// <summary>
        /// Maximum supported frame rate.
        /// </summary>
        public double MaxFrameRate;

        /// <summary>
        /// Retrieve capabilities of a video device
        /// </summary>
        /// <param name="videoStreamConfig">The video stream configuration.</param>
        internal VideoCapabilities(IAMStreamConfig videoStreamConfig)
        {
            if (videoStreamConfig == null)
                throw new ArgumentNullException("videoStreamConfig");

            AMMediaType mediaType = null;
            VideoStreamConfigCaps caps = null;
            IntPtr pCaps = IntPtr.Zero;
            
            try
            {
                // Ensure this device reports capabilities
                int c, size;
                int hr = videoStreamConfig.GetNumberOfCapabilities(out c, out size);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
                if (c <= 0)
                    throw new NotSupportedException("This video device does not report capabilities.");
                if (size > Marshal.SizeOf(typeof(VideoStreamConfigCaps)))
                    throw new NotSupportedException("Unable to retrieve video device capabilities. This video device requires a larger VideoStreamConfigCaps structure.");

                // Alloc memory for structure
                pCaps = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(VideoStreamConfigCaps)));

                // Retrieve first (and hopefully only) capabilities struct
                hr = videoStreamConfig.GetStreamCaps(0, out mediaType, pCaps);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);

                // Convert pointers to managed structures
                caps = (VideoStreamConfigCaps)Marshal.PtrToStructure(pCaps, typeof(VideoStreamConfigCaps));

                // Extract info
                InputSize = caps.InputSize;
                MinFrameSize = caps.MinOutputSize;
                MaxFrameSize = caps.MaxOutputSize;
                FrameSizeGranularityX = caps.OutputGranularityX;
                FrameSizeGranularityY = caps.OutputGranularityY;
                MinFrameRate = (double)10000000 / caps.MaxFrameInterval;
                MaxFrameRate = (double)10000000 / caps.MinFrameInterval;
            }
            finally
            {
                if (pCaps != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(pCaps); pCaps = IntPtr.Zero;
                if (mediaType != null)
                    DsUtils.FreeAMMediaType(mediaType); mediaType = null;
            }
        }
    }
}
