/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Nequeo.IO.Audio.Api
{
    /// <summary>
    /// 
    /// </summary>
    public struct PropertyKey
    {
        /// <summary>
        /// Format ID
        /// </summary>
        public Guid formatId;
        /// <summary>
        /// Property ID
        /// </summary>
        public int propertyId;
        /// <summary>
        /// <param name="formatId"></param>
        /// <param name="propertyId"></param>
        /// </summary>
        public PropertyKey(Guid formatId, int propertyId)
        {
            this.formatId = formatId;
            this.propertyId = propertyId;
        }
    }

    /// <summary>
    /// Property Keys
    /// </summary>
    internal static class PropertyKeys
    {
        /// <summary>
        /// PKEY_DeviceInterface_FriendlyName
        /// </summary>
        public static readonly PropertyKey PKEY_DeviceInterface_FriendlyName = new PropertyKey(new Guid(0x026e516e, unchecked((short)0xb814), 0x414b, 0x83, 0xcd, 0x85, 0x6d, 0x6f, 0xef, 0x48, 0x22), 2);
        /// <summary>
        /// PKEY_AudioEndpoint_FormFactor
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEndpoint_FormFactor = new PropertyKey(new Guid(0x1da5d803, unchecked((short)0xd492), 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e), 0);
        /// <summary>
        /// PKEY_AudioEndpoint_ControlPanelPageProvider
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEndpoint_ControlPanelPageProvider = new PropertyKey(new Guid(0x1da5d803, unchecked((short)0xd492), 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e), 1);
        /// <summary>
        /// PKEY_AudioEndpoint_Association
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEndpoint_Association = new PropertyKey(new Guid(0x1da5d803, unchecked((short)0xd492), 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e), 2);
        /// <summary>
        /// PKEY_AudioEndpoint_PhysicalSpeakers
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEndpoint_PhysicalSpeakers = new PropertyKey(new Guid(0x1da5d803, unchecked((short)0xd492), 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e), 3);
        /// <summary>
        /// PKEY_AudioEndpoint_GUID
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEndpoint_GUID = new PropertyKey(new Guid(0x1da5d803, unchecked((short)0xd492), 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e), 4);
        /// <summary>
        /// PKEY_AudioEndpoint_Disable_SysFx 
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEndpoint_Disable_SysFx = new PropertyKey(new Guid(0x1da5d803, unchecked((short)0xd492), 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e), 5);
        /// <summary>
        /// PKEY_AudioEndpoint_FullRangeSpeakers 
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEndpoint_FullRangeSpeakers = new PropertyKey(new Guid(0x1da5d803, unchecked((short)0xd492), 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e), 6);
        /// <summary>
        /// PKEY_AudioEndpoint_Supports_EventDriven_Mode 
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEndpoint_Supports_EventDriven_Mode = new PropertyKey(new Guid(0x1da5d803, unchecked((short)0xd492), 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e), 7);
        /// <summary>
        /// PKEY_AudioEndpoint_JackSubType
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEndpoint_JackSubType = new PropertyKey(new Guid(0x1da5d803, unchecked((short)0xd492), 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e), 8);
        /// <summary>
        /// PKEY_AudioEngine_DeviceFormat 
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEngine_DeviceFormat = new PropertyKey(new Guid(unchecked((int)0xf19f064d), 0x82c, 0x4e27, 0xbc, 0x73, 0x68, 0x82, 0xa1, 0xbb, 0x8e, 0x4c), 0);
        /// <summary>
        /// PKEY_AudioEngine_OEMFormat
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEngine_OEMFormat = new PropertyKey(new Guid(unchecked((int)0xe4870e26), 0x3cc5, 0x4cd2, 0xba, 0x46, 0xca, 0xa, 0x9a, 0x70, 0xed, 0x4), 3);
        /// <summary>
        /// PKEY _Devie_FriendlyName
        /// </summary>
        public static readonly PropertyKey PKEY_Device_FriendlyName = new PropertyKey(new Guid(unchecked((int)0xa45c254e), unchecked((short)0xdf1c), 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 14);
    }
}
