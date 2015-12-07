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
    ///  Specify the frequency of the TV tuner.
    /// </summary>
    public enum TVTunerInputType
    {
        /// <summary>
        /// Cable frequency
        /// </summary>
        Cable,
        /// <summary>
        /// Antenna frequency
        /// </summary>
        Antenna
    }

    /// <summary>
    /// Control and query a hardware TV Tuner.
    /// </summary>
    public class Tuner : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        protected IAMTVTuner _tvTuner = null;

        /// <summary>
        /// Control and query a hardware TV Tuner.
        /// </summary>
        /// <param name="tuner"></param>
        public Tuner(IAMTVTuner tuner)
        {
            _tvTuner = tuner;
        }

        /// <summary>
        /// Added for TVFineTune.cs
        /// </summary>
        internal Tuner()
        {
        }

        /// <summary>
        ///  Get or set the TV Tuner channel.
        /// </summary>
        public int Channel
        {
            get
            {
                int channel;
                AMTunerSubChannel v, a;
                int hr = _tvTuner.get_Channel(out channel, out v, out a);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
                return (channel);
            }

            set
            {
                int hr = _tvTuner.put_Channel(value, AMTunerSubChannel.Default, AMTunerSubChannel.Default);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
            }
        }

        /// <summary>
        ///  Get or set the tuner frequency (cable or antenna).
        /// </summary>
        public TunerInputType InputType
        {
            get
            {
                TunerInputType t;
                int hr = _tvTuner.get_InputType(0, out t);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
                return ((TunerInputType)t);
            }
            set
            {
                TunerInputType t = (TunerInputType)value;
                int hr = _tvTuner.put_InputType(0, t);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
            }
        }

        /// <summary>
        /// Indicates whether a signal is present on the current channel.
        /// If the signal strength cannot be determined, a NotSupportedException
        /// is thrown.
        /// </summary>
        public bool SignalPresent
        {
            get
            {
                AMTunerSignalStrength sig;
                int hr = _tvTuner.SignalPresent(out sig);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
                if (sig == AMTunerSignalStrength.HasNoSignalStrength) throw new NotSupportedException("Signal strength not available.");
                return (sig == AMTunerSignalStrength.SignalPresent);
            }
        }

        /// <summary>
        /// get minimum and maximum channels
        /// </summary>
        public int[] ChannelMinMax
        {
            get
            {
                int min;
                int max;
                int hr = _tvTuner.ChannelMinMax(out min, out max);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
                int[] myArray = new int[] { min, max };
                return myArray;
            }
        }

        /// <summary>
        /// useful for checking purposes
        /// </summary>
        public int GetVideoFrequency
        {
            get
            {
                int theFreq;
                int hr = _tvTuner.get_VideoFrequency(out theFreq);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
                return theFreq;
            }
        }

        /// <summary>
        /// not that useful, but...
        /// </summary>
        public int GetAudioFrequency
        {
            get
            {
                int theFreq;
                int hr = _tvTuner.get_AudioFrequency(out theFreq);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
                return theFreq;
            }
        }

        /// <summary>
        /// set this to your country code. Frequency Overrides should be set to this code
        /// </summary>
        public int TuningSpace
        {
            get
            {
                int tspace;
                int hr = _tvTuner.get_TuningSpace(out tspace);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
                return tspace;
            }
            set
            {
                int tspace = value;
                int hr = _tvTuner.put_TuningSpace(tspace);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
            }
        }

        /// <summary>
        /// Audio mode structure
        /// </summary>
        public struct AvAudioModes
        {
            /// <summary> Default audio mode available flag </summary>
            public bool Default;
            /// <summary> TV audio mode available flag </summary>
            public bool TV;
            /// <summary> FMRadio audio mode available flag </summary>
            public bool FMRadio;
            /// <summary> AMRadio audio mode available flag </summary>
            public bool AMRadio;
            /// <summary> Dss audio mode available flag </summary>
            public bool Dss;

            /// <summary> Scan audio modes and set appropriate flags </summary>
            public AvAudioModes(bool Default, bool TV, bool FMRadio, bool AMRadio, bool Dss)
            {
                this.Default = Default;
                this.TV = TV;
                this.FMRadio = FMRadio;
                this.AMRadio = AMRadio;
                this.Dss = Dss;
            }
        }

        /// 
        /// Retrieves or sets the current mode on a multifunction tuner.
        /// 
        public AMTunerModeType AudioMode
        {
            get
            {
                AMTunerModeType AudioMode;
                int hr = _tvTuner.get_Mode(out AudioMode);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
                return ((AMTunerModeType)AudioMode);
            }
            set
            {
                AMTunerModeType AudioMode = value;
                int hr = _tvTuner.put_Mode(AudioMode);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
            }
        }


        /// 
        /// Retrieves the tuner's supported modes.
        /// 
        public AvAudioModes AvailableAudioModes
        {
            get
            {
                AMTunerModeType AudioMode;
                int hr = _tvTuner.GetAvailableModes(out AudioMode);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
                AvAudioModes AvModes;

                if ((int)AudioMode == (int)AMTunerModeType.TV)
                {
                    AvModes = new AvAudioModes(true, true, false, false, false);
                }
                else if ((int)AudioMode == (int)AMTunerModeType.TV + (int)AMTunerModeType.AMRadio)
                {
                    AvModes = new AvAudioModes(true, true, false, true, false);
                }
                else if ((int)AudioMode == (int)AMTunerModeType.TV + (int)AMTunerModeType.FMRadio)
                {
                    AvModes = new AvAudioModes(true, true, true, false, false);
                }
                else if ((int)AudioMode == (int)AMTunerModeType.TV + (int)AMTunerModeType.Dss)
                {
                    AvModes = new AvAudioModes(true, true, false, false, true);
                }
                else if ((int)AudioMode == (int)AMTunerModeType.TV + (int)AMTunerModeType.AMRadio + (int)AMTunerModeType.FMRadio)
                {
                    AvModes = new AvAudioModes(true, true, true, true, false);
                }
                else if ((int)AudioMode == (int)AMTunerModeType.TV + (int)AMTunerModeType.AMRadio + (int)AMTunerModeType.Dss)
                {
                    AvModes = new AvAudioModes(true, true, false, true, true);
                }
                else if ((int)AudioMode == (int)AMTunerModeType.TV + (int)AMTunerModeType.FMRadio + (int)AMTunerModeType.Dss)
                {
                    AvModes = new AvAudioModes(true, true, true, false, true);
                }
                else if ((int)AudioMode == (int)AMTunerModeType.TV + (int)AMTunerModeType.AMRadio + (int)AMTunerModeType.FMRadio + (int)AMTunerModeType.Dss)
                {
                    AvModes = new AvAudioModes(true, true, true, true, true);
                }
                else
                {
                    AvModes = new AvAudioModes(false, false, false, false, false);
                }

                return (AvModes);
            }
        }
        // End of code based on code written by dauboro

        // New code written by Brian Low, dec 2003
        /// <summary>
        ///  Get or set the country code. Use the country code to set default frequency mappings.
        /// </summary>
        /// <remarks>
        /// Below is a sample of available country codes:
        /// <list type="bullet">
        ///   <item>1 - US</item>
        /// </list>
        /// For a full list of country codes, see the DirectX 9.0 
        /// documentation topic "Country/Region Assignments"
        /// </remarks>
        public int CountryCode
        {
            get
            {
                int c;
                int hr = _tvTuner.get_CountryCode(out c);
                if (hr < 0) Marshal.ThrowExceptionForHR(hr);
                return (c);
            }
            set
            {
                int hr = _tvTuner.put_CountryCode(value);
                if (hr < 0) Marshal.ThrowExceptionForHR(hr);
            }
        }

        /// <summary>
        ///  Determines if the tuner can tune to a particular channel.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///  An automated scan to find available channels:
        ///  <list type="number">
        ///   <item>Use <see cref="ChannelMinMax"/> to determine 
        ///			the range of available channels.</item>
        ///   <item>For each channel, call ChannelAvailable. If this method returns false, do not 
        ///			display the channel to the user. If this method returns true, it 
        ///			will have found the exact frequency for the channel.</item>
        ///	  <item>If ChannelAvailable is finding too many channels with just noise then 
        ///			check the <see cref="SignalPresent"/> property after calling ChannelAvailable. 
        ///			If SignalPresent is true, then the channel is most likely a valid, viewable
        ///			channel. However this risks missing viewable channels with moderate noise.
        ///			See <see cref="SignalPresent"/> for more information on locking on to 
        ///			a channel.</item>
        ///  </list>
        ///  </para>
        ///  
        ///  <para>
        ///  It is no longer required to perform a scan for each chanel's exact 
        ///  frequency. The tuner automatically finds the exact frequency each 
        ///  time the channel is changed. </para>
        ///  
        ///  <para>
        ///  This method correctly uses frequency-overrides. As described in
        ///  the DirectX SDK topic "Collecting Fine-Tuning Information", this method
        ///  does not use the IAMTVTuner.AutoTune() method. Instead it uses the
        ///  suggested put_Channel() method. </para>
        /// </remarks>
        /// <param name="channel">TV channel number</param>
        /// <returns>True if the channel's frequence was found, false otherwise.</returns>
        public bool ChannelAvailable(int channel)
        {
            int hr = _tvTuner.put_Channel(channel, AMTunerSubChannel.Default, AMTunerSubChannel.Default);
            if (hr < 0) Marshal.ThrowExceptionForHR(hr);
            return (hr == 0);
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                if (_tvTuner != null)
                    Marshal.ReleaseComObject(_tvTuner); 

                _tvTuner = null;

                // Note disposing has been done.
                _disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Tuner()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
