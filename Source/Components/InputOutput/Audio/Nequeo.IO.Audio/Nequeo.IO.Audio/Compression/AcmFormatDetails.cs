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
    /// ACMFORMATDETAILS
    /// http://msdn.microsoft.com/en-us/library/dd742913%28VS.85%29.aspx
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
    internal struct AcmFormatDetails
    {
        /// <summary>
        /// DWORD cbStruct; 
        /// </summary>
        public int structSize;
        /// <summary>
        /// DWORD dwFormatIndex; 
        /// </summary>
        public int formatIndex;
        /// <summary>
        /// DWORD dwFormatTag; 
        /// </summary>
        public int formatTag;
        /// <summary>
        /// DWORD fdwSupport; 
        /// </summary>
        public AcmDriverDetailsSupportFlags supportFlags;
        /// <summary>
        /// LPWAVEFORMATEX pwfx; 
        /// </summary>    
        public IntPtr waveFormatPointer;
        /// <summary>
        /// DWORD cbwfx; 
        /// </summary>
        public int waveFormatByteSize;
        /// <summary>
        /// TCHAR szFormat[ACMFORMATDETAILS_FORMAT_CHARS];
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = FormatDescriptionChars)]
        public string formatDescription;

        /// <summary>
        /// ACMFORMATDETAILS_FORMAT_CHARS
        /// </summary>
        public const int FormatDescriptionChars = 128;
    }
}
