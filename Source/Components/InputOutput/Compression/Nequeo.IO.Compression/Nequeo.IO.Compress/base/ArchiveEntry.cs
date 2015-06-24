/*  Company :       Nequeo Pty Ltd, http://www.nequeo.net.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2013 http://www.nequeo.net.au/
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
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace Nequeo.IO.Compression
{
    /// <summary>
    /// Represents a compressed file/data within a zip archive.
    /// </summary>
    public class ArchiveEntry
    {
        /// <summary>
        /// Gets the stream.
        /// </summary>
        public Stream Stream { get; internal set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        public string FullName { get; internal set; }

        /// <summary>
        /// Gets the entry length.
        /// </summary>
        public long Length { get; internal set; }

        /// <summary>
        /// Gets the entry compression length.
        /// </summary>
        public long CompressionLength { get; internal set; }

        /// <summary>
        /// Gets the last time the archive was changed.
        /// </summary>
        public DateTimeOffset LastWriteTime { get; internal set; }
    }
}
