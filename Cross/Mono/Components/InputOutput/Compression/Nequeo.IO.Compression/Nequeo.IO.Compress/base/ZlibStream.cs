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
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace Nequeo.IO.Compression
{
    /// <summary>
    /// Provides methods and properties for compressing and decompressing streams by using the Deflate algorithm (zlib library).
    /// </summary>
    public class ZlibStream
    {
        #region Public Static Methods

        /// <summary>
        /// Unzips a zipped file to the specified location
        /// </summary>
        /// <param name="zipFilename">The zipped file to un-zip.</param>
        /// <param name="unZipFilename">The file name where the unzipped file will be written to.</param>
        public static void Decompress(string zipFilename, string unZipFilename)
        {
            // Create all the streams
            using (Stream zipStream = File.OpenRead(zipFilename))
            using (Stream unzipStream = File.Create(unZipFilename))
            using (DeflateStream stream = new DeflateStream(zipStream, CompressionMode.Decompress))
            {
                // Decompress the data.
                stream.CopyTo(unzipStream);

                // Cleanup the resources.
                stream.Close();
                zipStream.Close();
                unzipStream.Close();
            }
        }

        /// <summary>
        /// Zips the specified un-zipped file.
        /// </summary>
        /// <param name="unZipFilename">The file to zip.</param>
        /// <param name="zipFilename">The file that will contain the zipper data.</param>
        public static void Compress(string unZipFilename, string zipFilename)
        {
            // Create all the streams
            using (Stream zipStream = File.Create(zipFilename))
            using (Stream unzipStream = File.OpenRead(unZipFilename))
            using (DeflateStream stream = new DeflateStream(zipStream, CompressionMode.Compress))
            {
                // Compress the data.
                unzipStream.CopyTo(stream);

                // Cleanup the resources.
                stream.Close();
                zipStream.Close();
                unzipStream.Close();
            }
        }

        /// <summary>
        /// Unzips a zipped stream to the specified stream
        /// </summary>
        /// <param name="zipStream">The zipped stream to un-zip.</param>
        /// <param name="unZipStream">The stream where the unzipped data will be written to.</param>
        /// <param name="zipStreamPosition">Set the zip stream position.</param>
        public static void Decompress(MemoryStream zipStream, MemoryStream unZipStream, long zipStreamPosition = 0)
        {
            // Set the position.
            zipStream.Position = zipStreamPosition;

            // Create all the streams
            using (DeflateStream stream = new DeflateStream(zipStream, CompressionMode.Decompress, true))
            {
                // Decompress the data.
                stream.CopyTo(unZipStream);

                // Cleanup the resources.
                stream.Close();
            }
        }

        /// <summary>
        /// Zips the specified un-zipped stream.
        /// </summary>
        /// <param name="unZipStream">The stream to zip.</param>
        /// <param name="zipStream">The stream that will contain the zipper data.</param>
        /// <param name="zipStreamPosition">Set the zip stream position.</param>
        public static void Compress(MemoryStream unZipStream, MemoryStream zipStream, long zipStreamPosition = 0)
        {
            // Set the position.
            unZipStream.Position = zipStreamPosition;

            // Create all the streams
            using (DeflateStream stream = new DeflateStream(zipStream, CompressionMode.Compress, true))
            {
                // Compress the data.
                unZipStream.CopyTo(stream);

                // Cleanup the resources.
                stream.Close();
            }
        }

        /// <summary>
        /// Unzips a zipped stream to the specified stream
        /// </summary>
        /// <param name="zipStream">The zipped stream to un-zip.</param>
        /// <param name="unZipStream">The stream where the unzipped data will be written to.</param>
        /// <param name="zipStreamPosition">Set the zip stream position.</param>
        public static void Decompress(Stream zipStream, Stream unZipStream, long zipStreamPosition = 0)
        {
            // Set the position.
            zipStream.Position = zipStreamPosition;

            // Create all the streams
            using (DeflateStream stream = new DeflateStream(zipStream, CompressionMode.Decompress, true))
            {
                // Decompress the data.
                stream.CopyTo(unZipStream);

                // Cleanup the resources.
                stream.Close();
            }
        }

        /// <summary>
        /// Zips the specified un-zipped stream.
        /// </summary>
        /// <param name="unZipStream">The stream to zip.</param>
        /// <param name="zipStream">The stream that will contain the zipper data.</param>
        /// <param name="zipStreamPosition">Set the zip stream position.</param>
        public static void Compress(Stream unZipStream, Stream zipStream, long zipStreamPosition = 0)
        {
            // Set the position.
            unZipStream.Position = zipStreamPosition;

            // Create all the streams
            using (DeflateStream stream = new DeflateStream(zipStream, CompressionMode.Compress, true))
            {
                // Compress the data.
                unZipStream.CopyTo(stream);

                // Cleanup the resources.
                stream.Close();
            }
        }
        #endregion
    }
}
