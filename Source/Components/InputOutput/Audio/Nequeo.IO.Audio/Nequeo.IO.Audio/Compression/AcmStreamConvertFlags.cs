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
    [Flags]
    internal enum AcmStreamConvertFlags
    {
        /// <summary>
        /// ACM_STREAMCONVERTF_BLOCKALIGN
        /// </summary>
        BlockAlign = 0x00000004,
        /// <summary>
        /// ACM_STREAMCONVERTF_START
        /// </summary>
        Start = 0x00000010,
        /// <summary>
        /// ACM_STREAMCONVERTF_END
        /// </summary>
        End = 0x00000020,
    }
}
