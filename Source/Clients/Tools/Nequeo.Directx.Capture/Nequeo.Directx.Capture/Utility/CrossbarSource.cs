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
    /// Represents a physical connector or source on an 
    /// audio/video device. This class is used on filters that
    /// support the IAMCrossbar interface such as TV Tuners.
    /// </summary>
    public class CrossbarSource : Source
    {
        /// <summary>
        /// This class cannot be created directly.
        /// </summary>
        /// <param name="crossbar">The cross bar</param>
        /// <param name="outputPin">The output Pin</param>
        /// <param name="inputPin">The input Pin</param>
        /// <param name="connectorType">The connection type.</param>
        internal CrossbarSource(IAMCrossbar crossbar, int outputPin, int inputPin, PhysicalConnectorType connectorType)
        {
            this.Crossbar = crossbar;
            this.OutputPin = outputPin;
            this.InputPin = inputPin;
            this.ConnectorType = connectorType;
            this.Name = GetName(connectorType);
        }

        internal IAMCrossbar Crossbar;			            // crossbar filter (COM object)
        internal int OutputPin;			                    // output pin number on the crossbar
        internal int InputPin;			                    // input pin number on the crossbar
        internal PhysicalConnectorType ConnectorType;		// type of the connector

        /// <summary>
        /// Gets or sets; enabled or disable this source.
        /// </summary>
        public override bool Enabled
        {
            get
            {
                int i;
                if (Crossbar.get_IsRoutedTo(OutputPin, out i) == 0)
                    if (InputPin == i)
                        return (true);
                return (false);
            }

            set
            {
                if (value)
                {
                    // Enable this route
                    int hr = this.Crossbar.Route(this.OutputPin, this.InputPin);
                    if (hr < 0)
                        Marshal.ThrowExceptionForHR(hr);
                }
                else
                {
                    // Disable this route by routing the output
                    // pin to input pin -1
                    int hr = this.Crossbar.Route(this.OutputPin, -1);
                    if (hr < 0)
                        Marshal.ThrowExceptionForHR(hr);
                }
            }
        }

        /// <summary>
        /// Retrieve the friendly name of a connectorType.
        /// </summary>
        /// <param name="connectorType">The connection type</param>
        /// <returns>The string value connection type.</returns>
        private string GetName(PhysicalConnectorType connectorType)
        {
            string name;
            switch (connectorType)
            {
                case PhysicalConnectorType.Video_Tuner: name = "Video Tuner"; break;
                case PhysicalConnectorType.Video_Composite: name = "Video Composite"; break;
                case PhysicalConnectorType.Video_SVideo: name = "Video S-Video"; break;
                case PhysicalConnectorType.Video_RGB: name = "Video RGB"; break;
                case PhysicalConnectorType.Video_YRYBY: name = "Video YRYBY"; break;
                case PhysicalConnectorType.Video_SerialDigital: name = "Video Serial Digital"; break;
                case PhysicalConnectorType.Video_ParallelDigital: name = "Video Parallel Digital"; break;
                case PhysicalConnectorType.Video_SCSI: name = "Video SCSI"; break;
                case PhysicalConnectorType.Video_AUX: name = "Video AUX"; break;
                case PhysicalConnectorType.Video_1394: name = "Video Firewire"; break;
                case PhysicalConnectorType.Video_USB: name = "Video USB"; break;
                case PhysicalConnectorType.Video_VideoDecoder: name = "Video Decoder"; break;
                case PhysicalConnectorType.Video_VideoEncoder: name = "Video Encoder"; break;
                case PhysicalConnectorType.Video_SCART: name = "Video SCART"; break;

                case PhysicalConnectorType.Audio_Tuner: name = "Audio Tuner"; break;
                case PhysicalConnectorType.Audio_Line: name = "Audio Line In"; break;
                case PhysicalConnectorType.Audio_Mic: name = "Audio Mic"; break;
                case PhysicalConnectorType.Audio_AESDigital: name = "Audio AES Digital"; break;
                case PhysicalConnectorType.Audio_SPDIFDigital: name = "Audio SPDIF Digital"; break;
                case PhysicalConnectorType.Audio_SCSI: name = "Audio SCSI"; break;
                case PhysicalConnectorType.Audio_AUX: name = "Audio AUX"; break;
                case PhysicalConnectorType.Audio_1394: name = "Audio Firewire"; break;
                case PhysicalConnectorType.Audio_USB: name = "Audio USB"; break;
                case PhysicalConnectorType.Audio_AudioDecoder: name = "Audio Decoder"; break;

                default: name = "Unknown Connector"; break;
            }
            return (name);
        }

        /// <summary>
        /// Release unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (Crossbar != null)
                Marshal.ReleaseComObject(Crossbar);
            Crossbar = null;
            base.Dispose();
        }
    }
}
