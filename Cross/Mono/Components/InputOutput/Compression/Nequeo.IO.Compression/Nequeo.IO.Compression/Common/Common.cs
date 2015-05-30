/*  Company :       Nequeo Pty Ltd, http://www.nequeo.net.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2011 http://www.nequeo.net.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nequeo.IO.Compression
{
    /// <summary>
    /// The compression direction type
    /// </summary>
    public enum CompressionDirectionType
    {
        /// <summary>
        /// Compress all files.
        /// </summary>
        Compress = 0,
        /// <summary>
        /// Decompress the file.
        /// </summary>
        Decompress = 1
    }

    /// <summary>
    /// The compression type.
    /// </summary>
    public enum CompressionType
    {
        /// <summary>
        /// Windows Zip protocol.
        /// </summary>
        Zip = 0,
        /// <summary>
        /// Linx/Unix Gzip protocol.
        /// </summary>
        Gzip = 1,
        /// <summary>
        /// Linx/Unix Bzip2 protocol.
        /// </summary>
        Bzip2 = 2,
        /// <summary>
        /// 7 zip protocol.
        /// </summary>
        SevenZip = 3,
        /// <summary>
        /// Zlib deflate algorithm.
        /// </summary>
        Zlib = 4,
    }
}
