/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Nequeo.IO.Audio.Wave;

namespace Nequeo.IO.Audio.Compression
{
    /// <summary>
    /// 
    /// </summary>
    internal enum AcmStreamSizeFlags
    {
        /// <summary>
        /// ACM_STREAMSIZEF_SOURCE
        /// </summary>
        Source = 0x00000000,
        /// <summary>
        /// ACM_STREAMSIZEF_DESTINATION
        /// </summary>
        Destination = 0x00000001
    }
}
