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
    [Guid("DD79923C-0599-45e0-B8B6-C8DF7DB6E796"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioPeakMeter
    {
         int GetChannelCount( out int pcChannels);
         int GetLevel( int Channel, out float level);
    }
}
