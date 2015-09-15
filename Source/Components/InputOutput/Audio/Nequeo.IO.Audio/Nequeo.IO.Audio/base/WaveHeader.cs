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
    /// WaveHeader interop structure (WAVEHDR)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class WaveHeader
    {
        /// <summary>pointer to locked data buffer (lpData)</summary>
        public IntPtr dataBuffer;
        /// <summary>length of data buffer (dwBufferLength)</summary>
        public int bufferLength;
        /// <summary>used for input only (dwBytesRecorded)</summary>
        public int bytesRecorded;
        /// <summary>for client's use (dwUser)</summary>
        public IntPtr userData;
        /// <summary>assorted flags (dwFlags)</summary>
        public WaveHeaderFlags flags;
        /// <summary>loop control counter (dwLoops)</summary>
        public int loops;
        /// <summary>PWaveHdr, reserved for driver (lpNext)</summary>
        public IntPtr next;
        /// <summary>reserved for driver</summary>
        public IntPtr reserved;
    }

    /// <summary>
    /// Wave Header Flags enumeration
    /// </summary>
    [Flags]
    public enum WaveHeaderFlags
    {
        /// <summary>
        /// WHDR_BEGINLOOP
        /// This buffer is the first buffer in a loop.  This flag is used only with output buffers.
        /// </summary>
        BeginLoop = 0x00000004,
        /// <summary>
        /// WHDR_DONE
        /// Set by the device driver to indicate that it is finished with the buffer and is returning it to the application.
        /// </summary>
        Done = 0x00000001,
        /// <summary>
        /// WHDR_ENDLOOP
        /// This buffer is the last buffer in a loop.  This flag is used only with output buffers.
        /// </summary>
        EndLoop = 0x00000008,
        /// <summary>
        /// WHDR_INQUEUE
        /// Set by Windows to indicate that the buffer is queued for playback.
        /// </summary>
        InQueue = 0x00000010,
        /// <summary>
        /// WHDR_PREPARED
        /// Set by Windows to indicate that the buffer has been prepared with the waveInPrepareHeader or waveOutPrepareHeader function.
        /// </summary>
        Prepared = 0x00000002
    }

    /// <summary>
    /// Wave message type.
    /// </summary>
    public enum WaveMessage
    {
        /// <summary>
        /// WIM_OPEN
        /// </summary>
        WaveInOpen = 0x3BE,
        /// <summary>
        /// WIM_CLOSE
        /// </summary>
        WaveInClose = 0x3BF,
        /// <summary>
        /// WIM_DATA
        /// </summary>
        WaveInData = 0x3C0,

        /// <summary>
        /// WOM_CLOSE
        /// </summary>
        WaveOutClose = 0x3BC,
        /// <summary>
        /// WOM_DONE
        /// </summary>
        WaveOutDone = 0x3BD,
        /// <summary>
        /// WOM_OPEN
        /// </summary>
        WaveOutOpen = 0x3BB
    }

    /// <summary>
    /// Wave in out open flag.
    /// </summary>
    [Flags]
    internal enum WaveInOutOpenFlags
    {
        /// <summary>
        /// CALLBACK_NULL
        /// No callback
        /// </summary>
        CallbackNull = 0,
        /// <summary>
        /// CALLBACK_FUNCTION
        /// dwCallback is a FARPROC 
        /// </summary>
        CallbackFunction = 0x30000,
        /// <summary>
        /// CALLBACK_EVENT
        /// dwCallback is an EVENT handle 
        /// </summary>
        CallbackEvent = 0x50000,
        /// <summary>
        /// CALLBACK_WINDOW
        /// dwCallback is a HWND 
        /// </summary>
        CallbackWindow = 0x10000,
        /// <summary>
        /// CALLBACK_THREAD
        /// callback is a thread ID 
        /// </summary>
        CallbackThread = 0x20000,
        /*
        WAVE_FORMAT_QUERY = 1,
        WAVE_MAPPED = 4,
        WAVE_FORMAT_DIRECT = 8*/
    }
}
