/*  Company :       Nequeo Pty Ltd, http://www.nequeo.net.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.net.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.IO.Compression
{
    /// <summary>
    /// Compression algorithm for streaming.
    /// </summary>
    public enum CompressionAlgorithmStreaming
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,
        /// <summary>
        /// ZLib.
        /// </summary>
        ZLib = 1,
        /// <summary>
        /// GZip.
        /// </summary>
        GZip = 2,
    }

    /// <summary>
    /// Compression algorithm.
    /// </summary>
    public enum CompressionAlgorithm
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,
        /// <summary>
        /// Zip.
        /// </summary>
        Zip = 1,
        /// <summary>
        /// ZLib.
        /// </summary>
        ZLib = 2,
        /// <summary>
        /// BZip2.
        /// </summary>
        BZip2 = 3,
        /// <summary>
        /// GZip.
        /// </summary>
        GZip = 4,
    }

    /// <summary>
    /// Compression algorithm conversion helper.
    /// </summary>
    public class CompressionAlgorithmHelper
    {
        /// <summary>
        /// Get the compression algorithm conversion.
        /// </summary>
        /// <param name="compressionAlgorithm">The compression algorithm.</param>
        /// <returns>The compression algorithm index.</returns>
        public static int GetAlgorithmInt32(CompressionAlgorithmStreaming compressionAlgorithm)
        {
            switch(compressionAlgorithm)
            {
                case CompressionAlgorithmStreaming.GZip:
                    return 2;
                case CompressionAlgorithmStreaming.ZLib:
                    return 1;
                case CompressionAlgorithmStreaming.None:
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Get the compression algorithm conversion.
        /// </summary>
        /// <param name="compressionAlgorithmIndex">The compression algorithm index.</param>
        /// <returns>The compression algorithm.</returns>
        public static CompressionAlgorithmStreaming GetAlgorithm(int compressionAlgorithmIndex)
        {
            switch(compressionAlgorithmIndex)
            {
                case 1:
                    return CompressionAlgorithmStreaming.ZLib;
                case 2:
                    return CompressionAlgorithmStreaming.GZip;
                case 0:
                default:
                    return CompressionAlgorithmStreaming.None;
            }
        }
    }
}
