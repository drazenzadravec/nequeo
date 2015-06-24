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

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace Nequeo.IO.Compression
{
    /// <summary>
    /// Zip and unzip compression class
    /// </summary>
    public class Zip
    {
        #region Public Static Methods

        /// <summary>
        /// Unzips a zipped file to the specified location
        /// </summary>
        /// <param name="zipFilename">The zipped file to un-zip.</param>
        /// <param name="unZipDirectorPath">The directory where to un-zip the file.</param>
        public static void Decompress(string zipFilename, string unZipDirectorPath)
        {
            Decompress(zipFilename, unZipDirectorPath, string.Empty);
        }

        /// <summary>
        /// Unzips a zipped file to the specified location
        /// </summary>
        /// <param name="zipFilename">The zipped file to un-zip.</param>
        /// <param name="unZipDirectorPath">The directory where to un-zip the file.</param>
        /// <param name="decryptionPasswordPhrase">The password phrase used to decrypt the compression file.</param>
        public static void Decompress(string zipFilename, string unZipDirectorPath, string decryptionPasswordPhrase)
        {
            // Create a new zip input stream
            using (ZipInputStream stream = new ZipInputStream(System.IO.File.OpenRead(zipFilename)))
            {
                // If the zip file should be encryped.
                if (!String.IsNullOrEmpty(decryptionPasswordPhrase))
                    stream.Password = decryptionPasswordPhrase;

                ZipEntry entry;

                // Iterate through the file within the zip file
                while ((entry = stream.GetNextEntry()) != null)
                {
                    // Get the relative path of the file
                    // and the name of the file to unzip
                    string fileName = unZipDirectorPath.TrimEnd('\\') + "\\" + entry.Name.TrimStart('/').Replace("/", "\\").TrimEnd('\\');
                    string directory = Path.GetDirectoryName(fileName).TrimEnd('\\') + "\\";

                    // Create the relative directory if
                    // it does not exist
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    // If a file exits then unzip the file
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        // Create a new file stream to write
                        // the unzipped data to the file.
                        using (FileStream fileStream = System.IO.File.Create(fileName))
                        {
                            int size = 2048;

                            // Create a memory buffer
                            byte[] data = new byte[2048];

                            // Write the zipped data to the unzipped file
                            while (true)
                            {
                                // Read the zipped data
                                size = stream.Read(data, 0, data.Length);

                                if (size > 0)
                                    // Wtite the unzipped data to the file.
                                    fileStream.Write(data, 0, size);
                                else
                                    break;
                            }
                        }
                    }
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
            Compress(zipFilename, zipDirectorPath, pattern, SearchOption.AllDirectories, extensionRegularExpression, filesToInclude, String.Empty);
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
            // Get the collection of files in the directory
            string[] files = Directory.GetFiles(zipDirectorPath.TrimEnd('\\') + "\\", pattern, searchOption);

            // Create a new zip output stream
            using (ZipOutputStream stream = new ZipOutputStream(File.Create(zipFilename)))
            {
                // If the zip file should be encryped.
                if (!String.IsNullOrEmpty(encryptionPasswordPhrase))
                    stream.Password = encryptionPasswordPhrase;

                // 0 - store only to 9 - means best compression
                stream.SetLevel(9);

                // Create a memory buffer
                byte[] buffer = new byte[4096];

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

                        // Using GetFileName makes the result compatible
                        // as the resulting path is not absolute.
                        ZipEntry entry = new ZipEntry((!String.IsNullOrEmpty(relativePath) ? relativePath.TrimEnd('/') + "/" : "") + Path.GetFileName(file));

                        // Assign the zip entry attributes
                        entry.DateTime = DateTime.Now;
                        stream.PutNextEntry(entry);

                        // Create a new file stream to read the
                        // contens of the file
                        using (FileStream fileStream = File.OpenRead(file))
                        {
                            // Using a fixed size buffer here makes no noticeable difference for output
                            // but keeps a lid on memory usage.
                            int sourceBytes;
                            do
                            {
                                // Read the file and erite to the zip file.
                                sourceBytes = fileStream.Read(buffer, 0, buffer.Length);
                                stream.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                }

                // Finish is important to ensure trailing information for a Zip file is appended.
                // Without this the created file would be invalid.
                stream.Finish();

                // Close is important to wrap things up and unlock the file.
                stream.Close();
            }
        }

        #endregion
    }
}
