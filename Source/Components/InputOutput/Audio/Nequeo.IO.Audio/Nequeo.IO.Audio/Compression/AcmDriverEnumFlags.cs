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
    internal enum AcmDriverEnumFlags
    {
        /// <summary>
        /// ACM_DRIVERENUMF_NOLOCAL, Only global drivers should be included in the enumeration
        /// </summary>
        NoLocal = 0x40000000,
        /// <summary>
        /// ACM_DRIVERENUMF_DISABLED, Disabled ACM drivers should be included in the enumeration
        /// </summary>
        Disabled = unchecked((int)0x80000000),
    }
}
