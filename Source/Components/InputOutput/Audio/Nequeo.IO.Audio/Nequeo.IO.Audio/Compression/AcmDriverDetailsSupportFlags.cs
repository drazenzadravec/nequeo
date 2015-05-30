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
    /// Flags indicating what support a particular ACM driver has
    /// </summary>
    [Flags]
    internal enum AcmDriverDetailsSupportFlags
    {
        /// <summary>ACMDRIVERDETAILS_SUPPORTF_CODEC - Codec</summary>
        Codec = 0x00000001,
        /// <summary>ACMDRIVERDETAILS_SUPPORTF_CONVERTER - Converter</summary>
        Converter = 0x00000002,
        /// <summary>ACMDRIVERDETAILS_SUPPORTF_FILTER - Filter</summary>
        Filter = 0x00000004,
        /// <summary>ACMDRIVERDETAILS_SUPPORTF_HARDWARE - Hardware</summary>
        Hardware = 0x00000008,
        /// <summary>ACMDRIVERDETAILS_SUPPORTF_ASYNC - Async</summary>
        Async = 0x00000010,
        /// <summary>ACMDRIVERDETAILS_SUPPORTF_LOCAL - Local</summary>
        Local = 0x40000000,
        /// <summary>ACMDRIVERDETAILS_SUPPORTF_DISABLED - Disabled</summary>
        Disabled = unchecked((int)0x80000000),
    }
}
