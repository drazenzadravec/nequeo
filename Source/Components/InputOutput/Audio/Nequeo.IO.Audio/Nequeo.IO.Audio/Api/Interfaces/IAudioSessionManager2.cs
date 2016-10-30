/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Nequeo.IO.Audio.Api.Interfaces
{
    [Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IAudioSessionManager
    {
        /// <summary>
        /// Retrieves an audio session control.
        /// </summary>
        /// <param name="sessionId">A new or existing session ID.</param>
        /// <param name="streamFlags">Audio session flags.</param>
        /// <param name="sessionControl">Receives an <see cref="IAudioSessionControl"/> interface for the audio session.</param>
        /// <returns>An HRESULT code indicating whether the operation succeeded of failed.</returns>
        [PreserveSig]
        int GetAudioSessionControl(
            [In, Optional] [MarshalAs(UnmanagedType.LPStruct)] Guid sessionId,
            [In] [MarshalAs(UnmanagedType.U4)] UInt32 streamFlags,
            [Out] [MarshalAs(UnmanagedType.Interface)] out IAudioSessionControl sessionControl);

        /// <summary>
        /// Retrieves a simple audio volume control.
        /// </summary>
        /// <param name="sessionId">A new or existing session ID.</param>
        /// <param name="streamFlags">Audio session flags.</param>
        /// <param name="audioVolume">Receives an <see cref="ISimpleAudioVolume"/> interface for the audio session.</param>
        /// <returns>An HRESULT code indicating whether the operation succeeded of failed.</returns>
        [PreserveSig]
        int GetSimpleAudioVolume(
            [In, Optional] [MarshalAs(UnmanagedType.LPStruct)] Guid sessionId,
            [In] [MarshalAs(UnmanagedType.U4)] UInt32 streamFlags,
            [Out] [MarshalAs(UnmanagedType.Interface)] out ISimpleAudioVolume audioVolume);

        [PreserveSig]
        int GetSessionEnumerator(out IAudioSessionEnumerator SessionEnum);
        [PreserveSig]
        int RegisterSessionNotification( IntPtr IAudioSessionNotification );
        [PreserveSig]
        int UnregisterSessionNotification( IntPtr IAudioSessionNotification );
        [PreserveSig]
        int RegisterDuckNotification( string sessionID, IntPtr IAudioVolumeDuckNotification);
        [PreserveSig]
        int UnregisterDuckNotification(IntPtr IAudioVolumeDuckNotification);
    };
}
