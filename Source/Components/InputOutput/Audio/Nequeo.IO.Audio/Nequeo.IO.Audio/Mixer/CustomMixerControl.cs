/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Runtime.InteropServices;

namespace Nequeo.IO.Audio.Mixer
{
    /// <summary>
    /// Custom Mixer control
    /// </summary>
    internal class CustomMixerControl : MixerControl 
	{
        internal CustomMixerControl(MixerInterop.MIXERCONTROL mixerControl, IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels) 
		{
			this.mixerControl = mixerControl;
            this.mixerHandle = mixerHandle;
            this.mixerHandleType = mixerHandleType;
			this.nChannels = nChannels;
			this.mixerControlDetails = new MixerInterop.MIXERCONTROLDETAILS();			
			GetControlDetails();
		}

		/// <summary>
		/// Get the data for this custom control
		/// </summary>
		/// <param name="pDetails">pointer to memory to receive data</param>
		protected override void GetDetails(IntPtr pDetails)
		{
		}

		// TODO: provide a way of getting / setting data
	}
}
