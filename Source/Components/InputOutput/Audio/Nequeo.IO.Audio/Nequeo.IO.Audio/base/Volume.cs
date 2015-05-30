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

namespace Nequeo.IO.Audio
{
    /// <summary>
    /// System volum control.
    /// </summary>
    public class Volume
    {
        /// <summary>
        /// Get the system volume (between 0 and 10).
        /// </summary>
        /// <returns>The system volume.</returns>
        public static ushort GetVolume()
        {
            int currentVolume = 0;

            // At this point, CurrVol gets assigned the volume
            WaveInterop.waveOutGetVolume(IntPtr.Zero, out currentVolume);

            // Calculate the volume
            ushort calcVol = (ushort)(currentVolume & 0x0000ffff);

            // Get the volume on a scale of 1 to 10 (to fit the trackbar)
            return (ushort)(calcVol / (ushort.MaxValue / (ushort)10));
        }

        /// <summary>
        /// Set the system volume (between 0 and 10).
        /// </summary>
        /// <param name="level">The volume (between 0 and 10)</param>
        public static void SetVolume(ushort level)
        {
            // Calculate the volume that's being set
            int newVolume = ((ushort.MaxValue / 10) * level);

            // Set the same volume for both the left and the right channels
            int newVolumeAllChannels = (((int)newVolume & 0x0000ffff) | ((int)newVolume << 16));

            // Set the volume
            WaveInterop.waveOutSetVolume(IntPtr.Zero, newVolumeAllChannels);
        }
    }
}
