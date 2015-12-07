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
    /// Summary description for FineTune.
    /// 
    /// This file adds fine tuning functionality to the Tuner class.
    /// Upon creation of the class the TV tuner will be initialized.
    /// </summary>
    public class TVFineTune : Tuner, IDisposable
    {
        internal TVFineTune(IAMTVTuner tuner, int countryCode, int tuningSpace)
        {
            _tvTuner = tuner;

            // Initialize the TV tuner
            this.InputType = TunerInputType.Cable;
            this.CountryCode = countryCode;
            this.TuningSpace = tuningSpace;
            this.AudioMode = AMTunerModeType.TV;

            // Minimum and maximum frequencies of TV tuner. These values are country
            // and TV tuner dependent! Real frequencies can be found via checking the
            // frequencies corresponding with the minimum and maximum channel numbers.
            // But becareful, some TV tuners return incorrect values.
            // For US the maximum frequency is usually 801MHz, for European countries
            // the maximum frequency is usually 863MHz

            // Try to find the maximum and the minimum TV tuning frequency
            int lastChan = this.Channel;
            int max = this.ChannelMinMax[1];
            if (lastChan < max)
            {
                int chan = max;
                for (int i = 0; (i < 30) && (max != this.Channel); i++)
                {
                    max = chan;
                    this.Channel = chan;
                    chan--;
                    this.maxFrequency = this.GetVideoFrequency;
                }
            }
            this.Channel = max;
            this.maxFrequency = this.GetVideoFrequency;

            int min = this.ChannelMinMax[0];
            this.Channel = min;
            this.minFrequency = this.GetVideoFrequency;

            // Default choice used for selecting my favorit channel (at 567MHz,
            // this frequency maps to Windows channel number 212. The Windows
            // default choice is usually channel 4 (48Mhz)
            this.Channel = lastChan;
            //this.Channel = 212;
        }

        internal static readonly Guid PROPSETID_TUNER = new Guid(0x6a2e0605, 0x28e4, 0x11d0, 0xa1, 0x8c, 0x00, 0xa0, 0xc9, 0x11, 0x89, 0x56);

        /// <summary>
        /// KSPROPERTY with Guid and additional data
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct KSPROPERTY
        {
            // size Guid is long + 2 short + 8 byte = 4 longs
            Guid Set;
            [MarshalAs(UnmanagedType.U4)]
            int Id;
            [MarshalAs(UnmanagedType.U4)]
            int Flags;
        }
        // KSIDENTIFIER, *PKSIDENTIFIER;

        /// <summary>
        /// Tuner property values specifying the property that can be read
        /// (or written)
        /// </summary>
        public enum KSPROPERTY_TUNER
        {
            /// <summary> R  -overall device capabilities </summary>
            TUNER_CAPS,
            /// <summary> R  -capabilities in this mode </summary>
            TUNER_MODE_CAPS,
            /// <summary> RW -set a mode (TV, FM, AM, DSS) </summary>
            TUNER_MODE,
            /// <summary> R  -get TV standard (only if TV mode) </summary>
            TUNER_STANDARD,
            /// <summary> RW -set/get frequency </summary>
            TUNER_FREQUENCY,
            /// <summary> RW -select an input </summary>
            TUNER_INPUT,
            /// <summary> R  -tuning status </summary>
            TUNER_STATUS,
            /// <summary> R O-Medium for IF or Transport Pin </summary>
            TUNER_IF_MEDIUM
        }

        // Describes how the device tunes.  Only one of these flags may be set
        // in KSPROPERTY_TUNER_MODE_CAPS_S.Strategy

        /// <summary>
        /// Describe how the driver should attempt to tune:
        /// EXACT:   just go to the frequency specified (no fine tuning)
        /// FINE:    (slow) do an exhaustive search for the best signal
        /// COARSE:  (fast) use larger frequency jumps to just determine if any signal
        /// </summary>
        public enum KS_TUNER_TUNING_FLAGS
        {
            /// <summary> No fine tuning </summary>
            TUNING_EXACT = 1,
            /// <summary> Fine grained search </summary>
            TUNING_FINE,
            /// <summary> Coarse search </summary>
            TUNING_COARSE,
        }

        /// <summary>
        /// KSPROPERTY tuner frequency data structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct KSPROPERTY_TUNERFREQUENCY
        {
            /// <summary> Hz </summary>
            [MarshalAs(UnmanagedType.U4)]
            public int Frequency;
            /// <summary> Hz (last known good) </summary>
            [MarshalAs(UnmanagedType.U4)]
            public int LastFrequency;
            /// <summary> KS_TUNER_TUNING_FLAGS </summary>
            [MarshalAs(UnmanagedType.U4)]
            public int TuningFlags;
            /// <summary> DSS </summary>
            [MarshalAs(UnmanagedType.U4)]
            public int VideoSubChannel;
            /// <summary> DSS </summary>
            [MarshalAs(UnmanagedType.U4)]
            public int AudioSubChannel;
            /// <summary> Channel number </summary>
            [MarshalAs(UnmanagedType.U4)]
            public int Channel;
            /// <summary> Country number </summary>
            [MarshalAs(UnmanagedType.U4)]
            public int Country;
            /// <summary> Undocumented or error ... </summary>
            [MarshalAs(UnmanagedType.U4)]
            public int Dummy;
            // Dummy added to get a succesful return of the Get, Set function
        }

        /// <summary>
        /// KSPROPERTY tuner frequency structure including the tuner frequency
        /// data structure.
        /// Size is 6 + 7 (+ 1 dummy) ints
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct KSPROPERTY_TUNER_FREQUENCY_S
        {
            /// <summary> Property Guid </summary>
            public KSPROPERTY Property;
            /// <summary> Tuner frequency data structure </summary>
            public KSPROPERTY_TUNERFREQUENCY Instance;
        }
        /// <summary>
        /// DShowErr enumerations
        /// </summary>
        public enum DshowError : long
        {
            // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/directshow/htm/errorandsuccesscodes.asp
            // HRESULT Values Specific to DirectShow
            /// <summary> </summary>
            VFW_NO_ERROR = 0L,
            /// <summary> </summary>
            VFW_E_NO_INTERFACE = 0x80040215,
        }

        /// <summary>
        /// Set broadcast frequency using the IKsProperySet interface.
        /// </summary>
        /// <param name="Freq"></param>
        /// <returns></returns>
        public int SetFrequency(int Freq)
        {
            int hr;
            IKsPropertySet pKs = _tvTuner as IKsPropertySet;

            DirectShowLib.KSPropertySupport dwSupported = new KSPropertySupport();
            DshowError errorCode = DshowError.VFW_NO_ERROR;

            // Use IKsProperySet interface (interface for Vfw like property
            // window) for getting/setting tuner specific information.
            // Check first if the Property is supported.
            if (pKs == null)
            {
                errorCode = DshowError.VFW_E_NO_INTERFACE;
                return (int)errorCode;
            }

            // Use IKsProperySet interface (interface for Vfw like propery
            // window) for getting and setting tuner specific information
            // like the real broadcast frequency.
            hr = pKs.QuerySupported(
                PROPSETID_TUNER,
                (int)KSPROPERTY_TUNER.TUNER_FREQUENCY,
                out dwSupported);
            if (hr == 0)
            {
                if (((dwSupported & DirectShowLib.KSPropertySupport.Get) == DirectShowLib.KSPropertySupport.Get) &&
                    ((dwSupported & DirectShowLib.KSPropertySupport.Set) == DirectShowLib.KSPropertySupport.Set) &
                    (Freq >= this.minFrequency && Freq <= this.maxFrequency))
                {
                    // Create and prepare data structures
                    KSPROPERTY_TUNER_FREQUENCY_S Frequency = new KSPROPERTY_TUNER_FREQUENCY_S();
                    IntPtr freqData = Marshal.AllocCoTaskMem(Marshal.SizeOf(Frequency));
                    IntPtr instData = Marshal.AllocCoTaskMem(Marshal.SizeOf(Frequency.Instance));
                    int cbBytes = 0;

                    // Convert the data
                    Marshal.StructureToPtr(Frequency, freqData, true);
                    Marshal.StructureToPtr(Frequency.Instance, instData, true);

                    hr = pKs.Get(
                        PROPSETID_TUNER,
                        (int)KSPROPERTY_TUNER.TUNER_FREQUENCY,
                        instData,
                        Marshal.SizeOf(Frequency.Instance),
                        freqData,
                        Marshal.SizeOf(Frequency),
                        out cbBytes);
                    if (hr == 0)
                    {
                        // Specify the TV broadcast frequency and tuning flag
                        Frequency.Instance.Frequency = Freq;
                        Frequency.Instance.TuningFlags = (int)KS_TUNER_TUNING_FLAGS.TUNING_EXACT;

                        // Convert the data
                        Marshal.StructureToPtr(Frequency, freqData, true);
                        Marshal.StructureToPtr(Frequency.Instance, instData, true);

                        // Now change the broadcast frequency
                        hr = pKs.Set(
                            PROPSETID_TUNER,
                            (int)KSPROPERTY_TUNER.TUNER_FREQUENCY,
                            instData,
                            Marshal.SizeOf(Frequency.Instance),
                            freqData,
                            Marshal.SizeOf(Frequency));
                        if (hr < 0)
                        {
                            errorCode = (DshowError)hr;
                        }
                    }
                    else
                    {
                        errorCode = (DshowError)hr;
                    }

                    if (freqData != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(freqData);
                    }
                    if (instData != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(instData);
                    }
                }
            }
            else
            {	// QuerySupported
                errorCode = (DshowError)hr;
            }

            return (int)errorCode;
        }

        private int minFrequency;
        private int maxFrequency;

        /// <summary>
        /// Maximum TV tuning frequency
        /// </summary>
        public int MaxFrequency
        {
            get
            {
                return maxFrequency;
            }
        }

        /// <summary>
        /// Minimum TV tuning frequency
        /// </summary>
        public int MinFrequency
        {
            get
            {
                return minFrequency;
            }
        }

        //#if NEWCODE
        /// <summary>
        /// IAMTVAudio property
        /// </summary>
        protected IAMTVAudio tvAudio = null;

        /// <summary>
        /// Access to TV audio property
        /// </summary>
        public IAMTVAudio TvAudio
        {
            get { return tvAudio; }
            set
            {
                tvAudio = value;
            }
        }
        //#endif		

        // ---------------- Public Methods ---------------

        /// <summary>
        /// Dispose Tuner property
        /// </summary>
        new public void Dispose()
        {
            if (_tvTuner != null)
            {
                Marshal.ReleaseComObject(_tvTuner);
                _tvTuner = null;
            }

            //#if NEWCODE
            if (tvAudio != null)
            {
                Marshal.ReleaseComObject(tvAudio);
                tvAudio = null;
            }
            //#endif		
        }
    }
}

