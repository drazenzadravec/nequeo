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

using SevenZip;

namespace Nequeo.IO.Compression
{
    /// <summary>
    /// 7 zip and unzip compression class
    /// </summary>
    public class SevenZip
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

            // Create a new extract archive stream.
            using (SevenZipExtractor archive = new SevenZipExtractor(zipFilename))
            {
                // For each file found in the
                // archive extract the file
                for (int i = 0; i < archive.ArchiveFileData.Count; i++)
                {
                    // Extract the current file.
                    archive.ExtractFiles(unZipDirectorPath, archive.ArchiveFileData[i].Index);
                }
            }
        }

        /// <summary>
        /// Unzips a zipped file to the specified location
        /// </summary>
        /// <param name="zipFilename">The zipped file to un-zip.</param>
        /// <param name="unZipDirectorPath">The directory where to un-zip the file.</param>
        /// <param name="decryptionPasswordPhrase">The password phrase used to decrypt the compression file.</param>
        public static void Decompress(string zipFilename, string unZipDirectorPath, string decryptionPasswordPhrase)
        {
            // Create the relative directory if
            // it does not exist
            if (!Directory.Exists(unZipDirectorPath.TrimEnd('\\') + "\\"))
                Directory.CreateDirectory(unZipDirectorPath.TrimEnd('\\') + "\\");

            // Create a new extract archive stream.
            using (SevenZipExtractor archive = new SevenZipExtractor(zipFilename, decryptionPasswordPhrase))
            {
                // For each file found in the
                // archive extract the file
                for (int i = 0; i < archive.ArchiveFileData.Count; i++)
                {
                    // Extract the current file.
                    archive.ExtractFiles(unZipDirectorPath, archive.ArchiveFileData[i].Index);
                }
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
            // Create a new archive stream.
            SevenZipCompressor archive = new SevenZipCompressor();
            
            bool recursive = false;

            // Get the search option
            switch (searchOption)
            {
                case SearchOption.AllDirectories:
                    recursive = true;
                    break;
                case SearchOption.TopDirectoryOnly:
                    recursive = false;
                    break;
            }

            // Assign options and attempt to compress
            // the directory.
            archive.ScanOnlyWritable = false;
            archive.CompressDirectoryExtension(zipDirectorPath, zipFilename, pattern, recursive, extensionRegularExpression, filesToInclude);
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
        /// <param name="encryptionPasswordPhrase">The password phrase used to encrypt the compression file.</param>
        /// <remarks>Extension Regular Expression should be in the form 'jpg|JPG|gif|GIF|doc|DOC|pdf|PDF'</remarks>
        public static void Compress(string zipFilename, string zipDirectorPath, string pattern,
            SearchOption searchOption, string extensionRegularExpression, List<string> filesToInclude, string encryptionPasswordPhrase)
        {
            // Create a new archive stream.
            SevenZipCompressor archive = new SevenZipCompressor(encryptionPasswordPhrase);

            bool recursive = false;

            // Get the search option
            switch (searchOption)
            {
                case SearchOption.AllDirectories:
                    recursive = true;
                    break;
                case SearchOption.TopDirectoryOnly:
                    recursive = false;
                    break;
            }

            // Assign options and attempt to compress
            // the directory.
            archive.ScanOnlyWritable = false;
            archive.CompressDirectoryExtension(zipDirectorPath, zipFilename, pattern, recursive, extensionRegularExpression, filesToInclude);
        }

        #endregion
    }
}
