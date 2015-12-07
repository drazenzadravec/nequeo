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
    /// Capabilities of the audio device such as 
    /// min/max sampling rate and number of channels available.
    /// </summary>
    public class AudioCapabilities
    {
        /// <summary>
        /// Minimum number of audio channels.
        /// </summary>
        public int MinimumChannels;

        /// <summary>
        /// Maximum number of audio channels.
        /// </summary>
        public int MaximumChannels;

        /// <summary>
        /// Granularity of the channels. For example, channels 2 through 4, in steps of 2.
        /// </summary>
        public int ChannelsGranularity;

        /// <summary>
        /// Minimum number of bits per sample.
        /// </summary>
        public int MinimumSampleSize;

        /// <summary>
        /// Maximum number of bits per sample.
        /// </summary>
        public int MaximumSampleSize;

        /// <summary>
        /// Granularity of the bits per sample. For example, 8 bits per sample through 32 bits per sample, in steps of 8.
        /// </summary>
        public int SampleSizeGranularity;

        /// <summary>
        /// Minimum sample frequency.
        /// </summary>
        public int MinimumSamplingRate;

        /// <summary>
        /// Maximum sample frequency.
        /// </summary>
        public int MaximumSamplingRate;

        /// <summary>
        /// Granularity of the frequency. For example, 11025 Hz to 44100 Hz, in steps of 11025 Hz.
        /// </summary>
        public int SamplingRateGranularity;

        /// <summary>
        /// Retrieve capabilities of an audio device
        /// </summary>
        /// <param name="audioStreamConfig">The audio stream configuration.</param>
        internal AudioCapabilities(IAMStreamConfig audioStreamConfig)
        {
            if (audioStreamConfig == null)
                throw new ArgumentNullException("audioStreamConfig");

            AMMediaType mediaType = null;
            AudioStreamConfigCaps caps = null;
            IntPtr pCaps = IntPtr.Zero;
            
            try
            {
                // Ensure this device reports capabilities
                int c, size;
                int hr = audioStreamConfig.GetNumberOfCapabilities(out c, out size);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
                if (c <= 0)
                    throw new NotSupportedException("This audio device does not report capabilities.");
                if (size > Marshal.SizeOf(typeof(AudioStreamConfigCaps)))
                    throw new NotSupportedException("Unable to retrieve audio device capabilities. This audio device requires a larger AudioStreamConfigCaps structure.");

                // Alloc memory for structure
                pCaps = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(AudioStreamConfigCaps)));

                // Retrieve first (and hopefully only) capabilities struct
                hr = audioStreamConfig.GetStreamCaps(0, out mediaType, pCaps);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);

                // Convert pointers to managed structures
                caps = (AudioStreamConfigCaps)Marshal.PtrToStructure(pCaps, typeof(AudioStreamConfigCaps));

                // Extract info
                MinimumChannels = caps.MinimumChannels;
                MaximumChannels = caps.MaximumChannels;
                ChannelsGranularity = caps.ChannelsGranularity;
                MinimumSampleSize = caps.MinimumBitsPerSample;
                MaximumSampleSize = caps.MaximumBitsPerSample;
                SampleSizeGranularity = caps.BitsPerSampleGranularity;
                MinimumSamplingRate = caps.MinimumSampleFrequency;
                MaximumSamplingRate = caps.MaximumSampleFrequency;
                SamplingRateGranularity = caps.SampleFrequencyGranularity;

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
