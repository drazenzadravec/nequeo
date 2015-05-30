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
    /// Flags for use with acmDriverAdd
    /// </summary>
    internal enum AcmDriverAddFlags
    {
        // also ACM_DRIVERADDF_TYPEMASK   = 0x00000007;

        /// <summary>
        /// ACM_DRIVERADDF_LOCAL
        /// </summary>
        Local = 0,
        /// <summary>
        /// ACM_DRIVERADDF_GLOBAL
        /// </summary>
        Global = 8,
        /// <summary>
        /// ACM_DRIVERADDF_FUNCTION
        /// </summary>
        Function = 3,
        /// <summary>
        /// ACM_DRIVERADDF_NOTIFYHWND
        /// </summary>
        NotifyWindowHandle = 4,
    }
}
