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
using System.IO;
using System.Text.RegularExpressions;

using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Tar;

namespace Nequeo.IO.Compression
{
    /// <summary>
    /// Gzip unix compression class
    /// </summary>
    public class Gzip
    {
        #region Public Static Methods

        /// <summary>
        /// Unzips a zipped file to the specified location
        /// </summary>
        /// <param name="zipFilename">The zipped file to un-zip.</param>
        /// <param name="unZipDirectorPath">The directory where to un-zip the file.</param>
        public static void Decompress(string zipFilename, string unZipDirectorPath)
        {
            // Create the relative directory if
            // it does not exist
            if (!Directory.Exists(unZipDirectorPath.TrimEnd('\\') + "\\"))
                Directory.CreateDirectory(unZipDirectorPath.TrimEnd('\\') + "\\");

            // Create all the streams
            using (Stream zipStream = File.OpenRead(zipFilename))
            using (GZipInputStream stream = new GZipInputStream(zipStream))
            using (TarArchive archive = TarArchive.CreateInputTarArchive(stream, TarBuffer.DefaultBlockFactor))
            {
                // Unzip the zip file to the directory
                archive.ExtractContents(unZipDirectorPath);

                // CLose all the streams.
                archive.Close();
                stream.Close();
                zipStream.Close();
            }
        }

        /// <summary>
        /// Zips the specified directory files to a zip file.
        /// </summary>
        /// <param name="zipFilename">The filename and path of the zip file to create.</param>
        /// <param name="zipDirectorPath">The directory of the files to zip.</param>
        public static void Compress(string zipFilename, string zipDirectorPath)
        {
            Compress(zipFilename, zipDirectorPath, "*.*", SearchOption.AllDirectories, string.Empty);
        }

        /// <summary>
        /// Zips the specified directory files to a zip file.
        /// </summary>
        /// <param name="zipFilename">The filename and path of the zip file to create.</param>
        /// <param name="zipDirectorPath">The directory of the files to zip.</param>
        /// <param name="pattern">The directory search pattern.</param>
        public static void Compress(string zipFilename, string zipDirectorPath, string pattern)
        {
            Compress(zipFilename, zipDirectorPath, pattern, SearchOption.AllDirectories, string.Empty);
        }

        /// <summary>
        /// Zips the specified directory files to a zip file.
        /// </summary>
        /// <param name="zipFilename">The filename and path of the zip file to create.</param>
        /// <param name="zipDirectorPath">The directory of the files to zip.</param>
        /// <param name="pattern">The directory search pattern.</param>
        /// <param name="searchOption">The directory search option.</param>
        /// <param name="extensionRegularExpression">The regular expression for excluding files from being compressed.</param>
        /// <remarks>Extension Regular Expression should be in the form 'jpg|JPG|gif|GIF|doc|DOC|pdf|PDF'</remarks>
        public static void Compress(string zipFilename, string zipDirectorPath, string pattern, SearchOption searchOption, string extensionRegularExpression)
        {
            Compress(zipFilename, zipDirectorPath, pattern, SearchOption.AllDirectories, extensionRegularExpression, null);
        }

        /// <summary>
        /// Zips the specified directory files to a zip file.
        /// </summary>
        /// <param name="zipFilename">The filename and path of the zip file to create.</param>
        /// <param name="zipDirectorPath">The directory of the files to zip.</param>
        /// <param name="pattern">The directory search pattern.</param>
        /// <param name="searchOption">The directory search option.</param>
        /// <param name="extensionRegularExpression">The regular expression for excluding files from being compressed.</param>
        /// <param name="filesToInclude">The list of files that are only to be compressed.</param>
        /// <remarks>Extension Regular Expression should be in the form 'jpg|JPG|gif|GIF|doc|DOC|pdf|PDF'</remarks>
        public static void Compress(string zipFilename, string zipDirectorPath, string pattern,
            SearchOption searchOption, string extensionRegularExpression, List<string> filesToInclude)
        {
            // Get the collection of files in the directory
            string[] files = Directory.GetFiles(zipDirectorPath.TrimEnd('\\') + "\\", pattern, searchOption);

            // Create all the streams
            using (Stream zipStream = File.Create(zipFilename))
            using (GZipOutputStream stream = new GZipOutputStream(zipStream))
            using (TarArchive archive = TarArchive.CreateOutputTarArchive(stream, TarBuffer.DefaultBlockFactor))
            {
                // Assign the archive properties
                archive.SetKeepOldFiles(false);
                archive.AsciiTranslate = true;
                archive.SetUserInfo(0, "anonymous", 0, "None");

                // For each file found
                foreach (string file in files)
                {
                    bool excludeFile = false;

                    // If a regular expression has been supplied.
                    if (!String.IsNullOrEmpty(extensionRegularExpression))
                        excludeFile = Regex.IsMatch(file.Trim(), @".*\.(" + extensionRegularExpression + @")$");

                    // Find all files that need to be included.
                    if (filesToInclude != null && !excludeFile)
                    {
                        // Should the current file be included.
                        IEnumerable<string> includeFiles = filesToInclude.Where(u => u.ToLower() == file.ToLower());
                        if (includeFiles.Count() > 0)
                            excludeFile = false;
                        else
                            excludeFile = true;
                    }

                    // If file should not be excluded
                    if (!excludeFile)
                    {
                        // Get the relative info
                        string relativePath = Path.GetDirectoryName(file).TrimEnd('\\') + "\\";
                        relativePath = relativePath.Replace(zipDirectorPath.TrimEnd('\\') + "\\", "").Replace("\\", "/");

                        // Get the file entry and set the relative
                        // path as the name of the entry.
                        TarEntry entry = TarEntry.CreateEntryFromFile(file);
                        entry.Name = (!String.IsNullOrEmpty(relativePath) ? relativePath.TrimEnd('/') + "/" : "") + Path.GetFileName(file);

                        // Write to the zip file.
                        archive.WriteEntry(entry, true);
                    }
                }

                // CLose all the streams.
                archive.Close();
                stream.Close();
                zipStream.Close();
            }
        }

        #endregion
    }
}
