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
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Nequeo.IO.Compression
{
    /// <summary>
    /// Compress data.
    /// </summary>
    public class Compresss
    {
        /// <summary>
        /// Unzips a zipped stream to the specified stream
        /// </summary>
        /// <param name="unZipStream">The stream where the unzipped data will be written to.</param>
        /// <param name="zipStream">The zipped stream to un-zip.</param>
        /// <param name="compressionType">The type of compression to use.</param>
        public static void Decompress(Stream unZipStream, Stream zipStream, CompressionAlgorithmStreaming compressionType)
        {
            switch (compressionType)
            {
                case CompressionAlgorithmStreaming.GZip:
                    GZipStream.Decompress(zipStream, unZipStream);
                    break;
                case CompressionAlgorithmStreaming.ZLib:
                    ZlibStream.Decompress(zipStream, unZipStream);
                    break;
            }
        }

        /// <summary>
        /// Zips the specified un-zipped stream.
        /// </summary>
        /// <param name="unZipStream">The stream to zip.</param>
        /// <param name="zipStream">The stream that will contain the zipper data.</param>
        /// <param name="compressionType">The type of compression to use.</param>
        public static void Compress(Stream unZipStream, Stream zipStream, CompressionAlgorithmStreaming compressionType)
        {
            switch (compressionType)
            {
                case CompressionAlgorithmStreaming.GZip:
                    GZipStream.Compress(unZipStream, zipStream);
                    break;
                case CompressionAlgorithmStreaming.ZLib:
                    ZlibStream.Compress(unZipStream, zipStream);
                    break;
            }
        }

        /// <summary>
        /// Zips the specified un-zipped stream.
        /// </summary>
        /// <param name="unZipArray">The stream to zip.</param>
        /// <param name="compressionType">The type of compression to use.</param>
        public static byte[] Compress(byte[] unZipArray, CompressionAlgorithmStreaming compressionType)
        {
            using (MemoryStream unZipStream = new MemoryStream(unZipArray))
            using (MemoryStream zipStream = new MemoryStream())
            {
                switch (compressionType)
                {
                    case CompressionAlgorithmStreaming.GZip:
                        GZipStream.Compress(unZipStream, zipStream);
                        break;
                    case CompressionAlgorithmStreaming.ZLib:
                        ZlibStream.Compress(unZipStream, zipStream);
                        break;
                }

                zipStream.Close();
                return zipStream.ToArray();
            }
        }

        /// <summary>
        /// Unzips a zipped stream to the specified stream
        /// </summary>
        /// <param name="zipArray">The zipped stream to un-zip.</param>
        /// <param name="compressionType">The type of compression to use.</param>
        public static byte[] Decompress(byte[] zipArray, CompressionAlgorithmStreaming compressionType)
        {
            using (MemoryStream unZipStream = new MemoryStream())
            using (MemoryStream zipStream = new MemoryStream(zipArray))
            {
                switch (compressionType)
                {
                    case CompressionAlgorithmStreaming.GZip:
                        GZipStream.Decompress(zipStream, unZipStream);
                        break;
                    case CompressionAlgorithmStreaming.ZLib:
                        ZlibStream.Decompress(zipStream, unZipStream);
                        break;
                }

                unZipStream.Close();
                return unZipStream.ToArray();
            }
        }
    }
}
