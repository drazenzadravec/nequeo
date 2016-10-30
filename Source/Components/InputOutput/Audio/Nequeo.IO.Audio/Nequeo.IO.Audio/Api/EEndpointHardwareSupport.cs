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
    [Flags]
    public enum EEndpointHardwareSupport
    {
        /// <summary>
        /// 
        /// </summary>
        Volume = 0x00000001,
        /// <summary>
        /// 
        /// </summary>
        Mute   = 0x00000002,
        /// <summary>
        /// 
        /// </summary>
        Meter  = 0x00000004
    }
}
