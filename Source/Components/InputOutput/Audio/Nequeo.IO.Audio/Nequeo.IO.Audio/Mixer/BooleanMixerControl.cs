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
    /// Boolean mixer control
    /// </summary>
    internal class BooleanMixerControl : MixerControl 
	{
		private MixerInterop.MIXERCONTROLDETAILS_BOOLEAN boolDetails;

        internal BooleanMixerControl(MixerInterop.MIXERCONTROL mixerControl, IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels) 
		{
			this.mixerControl = mixerControl;
            this.mixerHandle = mixerHandle;
            this.mixerHandleType = mixerHandleType;
			this.nChannels = nChannels;
			this.mixerControlDetails = new MixerInterop.MIXERCONTROLDETAILS();
			
			GetControlDetails();

		}
		
		/// <summary>
		/// Gets the details for this control
		/// </summary>
		/// <param name="pDetails">memory pointer</param>
		protected override void GetDetails(IntPtr pDetails) 
		{
			boolDetails = (MixerInterop.MIXERCONTROLDETAILS_BOOLEAN) Marshal.PtrToStructure(pDetails,typeof(MixerInterop.MIXERCONTROLDETAILS_BOOLEAN));
		}
		
		/// <summary>
		/// The current value of the control
		/// </summary>
		public bool Value 
		{
			get 
			{
				GetControlDetails(); // make sure we have the latest value
				return (boolDetails.fValue == 1);
			}
			set 
			{
                boolDetails.fValue = (value) ? 1 : 0;
                mixerControlDetails.paDetails = Marshal.AllocHGlobal(Marshal.SizeOf(boolDetails));
                Marshal.StructureToPtr(boolDetails, mixerControlDetails.paDetails, false);
                MmException.Try(MixerInterop.mixerSetControlDetails(mixerHandle, ref mixerControlDetails, MixerFlags.Value | mixerHandleType), "mixerSetControlDetails");
                Marshal.FreeHGlobal(mixerControlDetails.paDetails);
			}
		}		
	}
}
