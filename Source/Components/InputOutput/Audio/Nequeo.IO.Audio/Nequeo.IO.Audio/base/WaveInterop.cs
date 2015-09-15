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

namespace Nequeo.IO.Audio
{
    /// <summary>
    /// Wave interop provider.
    /// </summary>
    internal class WaveInterop
    {
        /// <summary>
        /// Wavw callback
        /// </summary>
        /// <param name="hWaveOut"></param>
        /// <param name="message"></param>
        /// <param name="dwInstance"></param>
        /// <param name="wavhdr"></param>
        /// <param name="dwReserved"></param>
        public delegate void WaveCallback(IntPtr hWaveOut, WaveMessage message, IntPtr dwInstance, WaveHeader wavhdr, IntPtr dwReserved);

        /// <summary>
        /// Get the system volum.
        /// </summary>
        /// <param name="hwo"></param>
        /// <param name="dwVolume"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutGetVolume(IntPtr hwo, out int dwVolume);

        /// <summary>
        /// Set the system volume.
        /// </summary>
        /// <param name="hwo"></param>
        /// <param name="dwVolume"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutSetVolume(IntPtr hwo, int dwVolume);

        /// <summary>
        /// Get the number of audio devices.
        /// </summary>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern Int32 waveOutGetNumDevs();

        /// <summary>
        /// Get the device capabilities
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="waveOutCaps"></param>
        /// <param name="waveOutCapsSize"></param>
        /// <returns></returns>
        [DllImport("winmm.dll", CharSet = CharSet.Auto)]
        public static extern MmResult waveOutGetDevCaps(IntPtr deviceID, out WaveDevice waveOutCaps, int waveOutCapsSize);

        /// <summary>
        /// Get the currrent position in the stream.
        /// </summary>
        /// <param name="hWaveOut"></param>
        /// <param name="mmTime"></param>
        /// <param name="uSize"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutGetPosition(IntPtr hWaveOut, out MmTime mmTime, int uSize);

        /// <summary>
        /// Prepare headers.
        /// </summary>
        /// <param name="hWaveOut"></param>
        /// <param name="lpWaveOutHdr"></param>
        /// <param name="uSize"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutPrepareHeader(IntPtr hWaveOut, WaveHeader lpWaveOutHdr, int uSize);

        /// <summary>
        /// Unprepare headers.
        /// </summary>
        /// <param name="hWaveOut"></param>
        /// <param name="lpWaveOutHdr"></param>
        /// <param name="uSize"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutUnprepareHeader(IntPtr hWaveOut, WaveHeader lpWaveOutHdr, int uSize);

        /// <summary>
        /// Write the wave data.
        /// </summary>
        /// <param name="hWaveOut"></param>
        /// <param name="lpWaveOutHdr"></param>
        /// <param name="uSize"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutWrite(IntPtr hWaveOut, WaveHeader lpWaveOutHdr, int uSize);

        /// <summary>
        /// Reset the wave.
        /// </summary>
        /// <param name="hWaveOut"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutReset(IntPtr hWaveOut);

        /// <summary>
        /// Close the wave.
        /// </summary>
        /// <param name="hWaveOut"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutClose(IntPtr hWaveOut);

        /// <summary>
        /// Pause the wave.
        /// </summary>
        /// <param name="hWaveOut"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutPause(IntPtr hWaveOut);

        /// <summary>
        /// Restart the wave.
        /// </summary>
        /// <param name="hWaveOut"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutRestart(IntPtr hWaveOut);

        /// <summary>
        /// Open out.
        /// </summary>
        /// <param name="hWaveOut"></param>
        /// <param name="uDeviceID"></param>
        /// <param name="lpFormat"></param>
        /// <param name="dwCallback"></param>
        /// <param name="dwInstance"></param>
        /// <param name="dwFlags"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutOpen(out IntPtr hWaveOut, IntPtr uDeviceID, WaveFormatProvider lpFormat, WaveCallback dwCallback, IntPtr dwInstance, WaveInOutOpenFlags dwFlags);

        /// <summary>
        /// Open out window.
        /// </summary>
        /// <param name="hWaveOut"></param>
        /// <param name="uDeviceID"></param>
        /// <param name="lpFormat"></param>
        /// <param name="callbackWindowHandle"></param>
        /// <param name="dwInstance"></param>
        /// <param name="dwFlags"></param>
        /// <returns></returns>
        [DllImport("winmm.dll", EntryPoint = "waveOutOpen")]
        public static extern MmResult waveOutOpenWindow(out IntPtr hWaveOut, IntPtr uDeviceID, WaveFormatProvider lpFormat, IntPtr callbackWindowHandle, IntPtr dwInstance, WaveInOutOpenFlags dwFlags);

        /// <summary>
        /// Open in.
        /// </summary>
        /// <param name="hWaveIn"></param>
        /// <param name="uDeviceID"></param>
        /// <param name="lpFormat"></param>
        /// <param name="dwCallback"></param>
        /// <param name="dwInstance"></param>
        /// <param name="dwFlags"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern MmResult waveInOpen(out IntPtr hWaveIn, IntPtr uDeviceID, WaveFormatProvider lpFormat, WaveCallback dwCallback, IntPtr dwInstance, WaveInOutOpenFlags dwFlags);

        /// <summary>
        /// Open in window.
        /// </summary>
        /// <param name="hWaveIn"></param>
        /// <param name="uDeviceID"></param>
        /// <param name="lpFormat"></param>
        /// <param name="callbackWindowHandle"></param>
        /// <param name="dwInstance"></param>
        /// <param name="dwFlags"></param>
        /// <returns></returns>
        [DllImport("winmm.dll", EntryPoint = "waveInOpen")]
        public static extern MmResult waveInOpenWindow(out IntPtr hWaveIn, IntPtr uDeviceID, WaveFormatProvider lpFormat, IntPtr callbackWindowHandle, IntPtr dwInstance, WaveInOutOpenFlags dwFlags);

    }
}
