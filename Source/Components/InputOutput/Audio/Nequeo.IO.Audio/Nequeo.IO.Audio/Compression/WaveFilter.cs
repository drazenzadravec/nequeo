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
    /// Summary description for WaveFilter.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal class WaveFilter
    {
        /// <summary>
        /// cbStruct
        /// </summary>
        public int StructureSize = Marshal.SizeOf(typeof(WaveFilter));
        /// <summary>
        /// dwFilterTag
        /// </summary>
        public int FilterTag = 0;
        /// <summary>
        /// fdwFilter
        /// </summary>
        public int Filter = 0;
        /// <summary>
        /// reserved
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public int[] Reserved = null;
    }
}
