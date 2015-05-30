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
    internal enum AcmStreamHeaderStatusFlags
    {
        /// <summary>
        /// ACMSTREAMHEADER_STATUSF_DONE
        /// </summary>
        Done = 0x00010000,
        /// <summary>
        /// ACMSTREAMHEADER_STATUSF_PREPARED
        /// </summary>
        Prepared = 0x00020000,
        /// <summary>
        /// ACMSTREAMHEADER_STATUSF_INQUEUE
        /// </summary>
        InQueue = 0x00100000,
    }
}
