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
    /// Represents a package of compressed files in the zip archive format.
    /// </summary>
    public class FileSystem
    {
        /// <summary>
        /// Unzips a zipped file to the specified location.
        /// </summary>
        /// <param name="zipFilename">The zipped file to un-zip.</param>
        /// <param name="unZipDirectorPath">The directory where to un-zip the file.</param>
        public static void Decompress(string zipFilename, string unZipDirectorPath)
        {
            ZipFile.ExtractToDirectory(zipFilename, unZipDirectorPath);
        }

        /// <summary>
        /// Unzips the archive entries to the specified location.
        /// </summary>
        /// <param name="archiveEntries">The collection of archive entries.</param>
        /// <param name="unZipDirectorPath">The directory where to un-zip the archive entries.</param>
        public static void Decompress(List<ArchiveEntry> archiveEntries, string unZipDirectorPath)
        {
            // Get the directory path.
            string directory = Path.GetDirectoryName(unZipDirectorPath).TrimEnd('\\') + "\\";

            // Create the relative directory if
            // it does not exist
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // For each entry.
            foreach (ArchiveEntry entry in archiveEntries)
            {
                // Get the stream.
                using (Stream stream = entry.Stream)
                {
                    // Create a new file stream.
                    using (FileStream writer = new FileStream(directory + entry.FullName, FileMode.Create, FileAccess.Write))
                    {
                        // Write the entry data to the file.
                        stream.CopyTo(writer);
                    }
                }
            }
        }

        /// <summary>
        /// Unzips the archive entry to the specified location.
        /// </summary>
        /// <param name="archiveEntry">The archive entry.</param>
        /// <param name="unZipDirectorPath">The directory where to un-zip the archive entry.</param>
        public static void Decompress(ArchiveEntry archiveEntry, string unZipDirectorPath)
        {
            // Get the directory path.
            string directory = Path.GetDirectoryName(unZipDirectorPath).TrimEnd('\\') + "\\";

            // Create the relative directory if
            // it does not exist
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Get the stream.
            using (Stream stream = archiveEntry.Stream)
            {
                // Create a new file stream.
                using (FileStream writer = new FileStream(directory + archiveEntry.FullName, FileMode.Create, FileAccess.Write))
                {
                    // Write the entry data to the file.
                    stream.CopyTo(writer);
                }
            }
        }

        /// <summary>
        /// Unzips the stream to the specified location.
        /// </summary>
        /// <param name="zipStream">The stream that contains zipped data,</param>
        /// <param name="unZipDirectorPath">The directory where to un-zip the stream.</param>
        public static void Decompress(Stream zipStream, string unZipDirectorPath)
        {
            Archive archive = new Archive();

            // Decompress the stream data.
            List<ArchiveEntry> archiveEntries = archive.GetEntries(zipStream);
            Decompress(archiveEntries, unZipDirectorPath);
        }

        /// <summary>
        /// Zips the specified directory files to a zip file.
        /// </summary>
        /// <param name="zipFilename">The filename and path of the zip file to create.</param>
        /// <param name="zipDirectorPath">The directory of the files to zip.</param>
        public static void Compress(string zipFilename, string zipDirectorPath)
        {
            Compress(zipFilename, zipDirectorPath, null);
        }

        /// <summary>
        /// Zips the specified directory files to a zip file.
        /// </summary>
        /// <param name="zipFilename">The filename and path of the zip file to create.</param>
        /// <param name="zipDirectorPath">The directory of the files to zip.</param>
        /// <param name="pattern">The directory search pattern.</param>
        /// <param name="searchOption">The directory search option.</param>
        public static void Compress(string zipFilename, string zipDirectorPath, string pattern, SearchOption searchOption = SearchOption.AllDirectories)
        {
            // If no pattern exists.
            if (String.IsNullOrEmpty(pattern))
            {
                // Create the zip file from the directory.
                ZipFile.CreateFromDirectory(zipDirectorPath, zipFilename);
            }
            else
            {
                // Get the collection of files in the directory
                string[] files = Directory.GetFiles(zipDirectorPath.TrimEnd('\\') + "\\", pattern, searchOption);

                // Create a new archive.
                using (ZipArchive archive = ZipFile.Open(zipFilename, ZipArchiveMode.Update))
                {
                    // For each file found in the directory.
                    foreach (string file in files)
                    {
                        // Get the relative info
                        string relativePath = Path.GetDirectoryName(file).TrimEnd('\\') + "\\";
                        relativePath = relativePath.Replace(zipDirectorPath.TrimEnd('\\') + "\\", "");

                        // Add the file to the archive.
                        archive.CreateEntryFromFile(file, relativePath + Path.GetFileName(file));
                    }
                } 
            }
        }

        /// <summary>
        /// Zips the specified directory files to a zip stream.
        /// </summary>
        /// <param name="zipDirectorPath">The directory of the files to zip.</param>
        /// <param name="searchOption">The directory search option.</param>
        /// <param name="pattern">The directory search pattern.</param>
        /// <returns>The zipped stream.</returns>
        public static Stream Compress(string zipDirectorPath, SearchOption searchOption = SearchOption.AllDirectories, string pattern = "*.*")
        {
            MemoryStream stream = new MemoryStream();
            
            // Get the collection of files in the directory
            string[] files = Directory.GetFiles(zipDirectorPath.TrimEnd('\\') + "\\", pattern, searchOption);

            // Open the archive.
            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
            {
                // For each file found in the directory.
                foreach (string file in files)
                {
                    // Get the relative info
                    string relativePath = Path.GetDirectoryName(file).TrimEnd('\\') + "\\";
                    relativePath = relativePath.Replace(zipDirectorPath.TrimEnd('\\') + "\\", "");

                    // Create an empty entry.
                    ZipArchiveEntry entry = archive.CreateEntry(relativePath + Path.GetFileName(file));

                    // Open the file stream.
                    using (Stream writer = entry.Open())
                    using (FileStream reader = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        // Copy the file data to the zip archive.
                        reader.CopyTo(writer);
                    }
                }
            }

            // Return the stream.
            return stream;
        }
    }
}
