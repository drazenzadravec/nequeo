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
    /// Compress data static type.
    /// </summary>
    public class CompressData
    {
        /// <summary>
        /// Unzips a zipped file to the specified location
        /// </summary>
        /// <param name="zipFilename">The zipped file to un-zip.</param>
        /// <param name="unZipDirectorPath">The directory where to un-zip the file.</param>
        /// <param name="compressionType">The type of compression to use.</param>
        public static void Decompress(string zipFilename, string unZipDirectorPath, CompressionType compressionType)
        {
            switch (compressionType)
            {
                case CompressionType.Zip:
                    Zip.Decompress(zipFilename, unZipDirectorPath);
                    break;
                case CompressionType.Gzip:
                    Gzip.Decompress(zipFilename, unZipDirectorPath);
                    break;
                case CompressionType.Bzip2:
                    Bzip2.Decompress(zipFilename, unZipDirectorPath);
                    break;
                case CompressionType.SevenZip:
                    SevenZip.Decompress(zipFilename, unZipDirectorPath);
                    break;
                case CompressionType.Zlib:
                    FileSystem.Decompress(zipFilename, unZipDirectorPath);
                    break;
            }
        }

        /// <summary>
        /// Zips the specified directory files to a zip file.
        /// </summary>
        /// <param name="zipFilename">The filename and path of the zip file to create.</param>
        /// <param name="zipDirectorPath">The directory of the files to zip.</param>
        /// <param name="compressionType">The type of compression to use.</param>
        public static void Compress(string zipFilename, string zipDirectorPath, CompressionType compressionType)
        {
            switch (compressionType)
            {
                case CompressionType.Zip:
                    Zip.Compress(zipFilename, zipDirectorPath);
                    break;
                case CompressionType.Gzip:
                    Gzip.Compress(zipFilename, zipDirectorPath);
                    break;
                case CompressionType.Bzip2:
                    Bzip2.Compress(zipFilename, zipDirectorPath);
                    break;
                case CompressionType.SevenZip:
                    SevenZip.Compress(zipFilename, zipDirectorPath);
                    break;
                case CompressionType.Zlib:
                    FileSystem.Compress(zipFilename, zipDirectorPath);
                    break;
            }
        }

        /// <summary>
        /// Perform the compression operation.
        /// </summary>
        /// <param name="zipFilename">The filename and path of the zip file to create when compressing; The zipped file to un-zip when decompressing.</param>
        /// <param name="zipDirectorPath">The directory of the files to zip when compressing; The directory where to un-zip the file when decompressing.</param>
        /// <param name="compressionType">The type of compression to use.</param>
        /// <param name="compressionDirectionType">The direction of compression</param>
        public static void Operation(string zipFilename, string zipDirectorPath, CompressionType compressionType, CompressionDirectionType compressionDirectionType)
        {
            switch (compressionDirectionType)
            {
                case CompressionDirectionType.Compress:
                    CompressData.Compress(zipFilename, zipDirectorPath, compressionType);
                    break;
                case CompressionDirectionType.Decompress:
                    CompressData.Decompress(zipFilename, zipDirectorPath, compressionType);
                    break;
            }
        }
    }
}
