/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Nequeo.IO.Audio.Wave;
using Nequeo.IO.Audio.Utils;

namespace Nequeo.IO.Audio
{
    /// <summary>
    /// System audio devices.
    /// </summary>
    public class Devices
    {
        /// <summary>
        /// Gets the number of Wave Out devices available in the system
        /// </summary>
        public static Int32 Count
        {
            get { return WaveInterop.waveOutGetNumDevs(); }
        }

        /// <summary>
        /// Retrieves the waveOut device.
        /// </summary>
        /// <param name="deviceIndex">The device index.</param>
        /// <returns>The WaveOut device capabilities</returns>
        public static Device GetDevice(int deviceIndex)
        {
            DeviceDetails caps = new DeviceDetails();
            int structSize = Marshal.SizeOf(caps);
            MmException.Try(WaveInterop.waveOutGetDevCaps((IntPtr)deviceIndex, out caps, structSize), "waveOutGetDevCaps");
            Device device = new Device();
            device.Details = caps;
            device.Index = deviceIndex;
            return device;
        }
    }
}
