/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nequeo.Web.Download.Common
{
    /// <summary>
    /// Helper class.
    /// </summary>
    internal class Helper
    {
        /// <summary>
        /// Static token provider.
        /// </summary>
        public static TokenProvider TokenProvider = new TokenProvider();

        /// <summary>
        /// Download File Base Path.
        /// </summary>
        public static string DownloadBasePath = Nequeo.Web.Download.Properties.Settings.Default.DownloadFileBasePath;

        /// <summary>
        /// The physical path of the application.
        /// </summary>
        public static string PhysicalPath = string.Empty;

        /// <summary>
        /// The application service name.
        /// </summary>
        public static string ServiceName = "DownloadWebManagerServer";

        /// <summary>
        /// Get the application download path.
        /// </summary>
        /// <returns>The base application path.</returns>
        public static string GetDownloadPath()
        {
            return PhysicalPath.TrimEnd(new char[] { '\\' }) + "\\" + DownloadBasePath + "\\";
        }

        /// <summary>
        /// Get the relative path name.
        /// </summary>
        /// <param name="path">The full path the update.</param>
        /// <returns>The relative path.</returns>
        public static string GetRelativePath(string path)
        {
            return path.Replace(GetDownloadPath().TrimEnd(new char[] { '\\' }), "");
        }

        /// <summary>
        /// Get the full path of the first file found.
        /// </summary>
        /// <param name="filename">The file name to find.</param>
        /// <param name="directory">The directory where the file is stored.</param>
        /// <returns>The full path.</returns>
        public static string GetDownloadFile(string filename, string directory)
        {
            string[] files = null;
            try
            {
                if (String.IsNullOrEmpty(directory))
                    directory = "";

                // Get the current folder.
                string folder = Helper.GetDownloadPath().TrimEnd(new char[] { '\\' }) + "\\" +
                    directory.Replace("/", "\\").TrimStart(new char[] { '\\' }).TrimEnd(new char[] { '\\' }) + (directory.Length > 0 ? "\\" : "");

                // Get all files with the same name.
                files = System.IO.Directory.GetFiles(
                    folder, filename, System.IO.SearchOption.AllDirectories);

                // Return the first file found.
                return files[0];
            }
            catch (Exception ex)
            {
                // Throw file not found exception.
                throw new Nequeo.Exceptions.InvalidPathException("File not found.", ex);
            }
        }

        /// <summary>
        /// Get the full path and name of the file to create.
        /// </summary>
        /// <param name="filename">The file name to create.</param>
        /// <param name="directory">The directory to store the file.</param>
        /// <returns>The full path and name of the file.</returns>
        public static string GetUploadFile(string filename, string directory)
        {
            string[] files = null;
            try
            {
                if (String.IsNullOrEmpty(directory))
                    directory = "";

                // Create the path.
                string path = Helper.GetDownloadPath().TrimEnd(new char[] { '\\' }) + "\\" +
                    directory.Replace("/", "\\").TrimStart(new char[] { '\\' }).TrimEnd(new char[] { '\\' }) + (directory.Length > 0 ? "\\" : "");
                string pathFilename = path + (filename.ToLower().Contains(".partial") ? filename.ToLower().Replace(".partial", "") : filename);

                // If the directory does not exist
                // create the directory.
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                // Get all files with the same name.
                files = System.IO.Directory.GetFiles(path, filename, System.IO.SearchOption.AllDirectories);

                // If not null.
                if (files != null)
                {
                    // If file already exists.
                    if (files.Length > 0)
                        throw new Exception();
                    else
                        return pathFilename;
                }
                else
                    return pathFilename;
            }
            catch (Exception ex)
            {
                // Throw file not found exception.
                throw new Nequeo.Exceptions.InvalidPathException("File already exists.", ex);
            }
        }

        /// <summary>
        /// Get the file name and path.
        /// </summary>
        /// <param name="path">The file name and path.</param>
        /// <returns>The path and file name;</returns>
        public static string GetFilePath(string path)
        {
            string file = null;
            try
            {
                // Get all files with the same name.
                file = Helper.GetDownloadPath().TrimEnd(new char[] { '\\' }) + "\\" +
                    path.Replace("/", "\\").TrimStart(new char[] { '\\' }).TrimEnd(new char[] { '\\' });

                // If the directory does not exist
                // create the directory.
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(file)))
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(file));

                // Return the path.
                return file;
            }
            catch (Exception ex)
            {
                // Throw file not found exception.
                throw new Nequeo.Exceptions.InvalidPathException("File not found.", ex);
            }
        }

        /// <summary>
        /// Get the file name and path.
        /// </summary>
        /// <param name="filename">The file name to create.</param>
        /// <param name="directory">The directory to store the file.</param>
        /// <returns>The path and file name;</returns>
        public static string GetFilePath(string filename, string directory)
        {
            string[] files = null;
            try
            {
                if (String.IsNullOrEmpty(directory))
                    directory = "";

                // Get the current folder.
                string folder = Helper.GetDownloadPath().TrimEnd(new char[] { '\\' }) + "\\" +
                    directory.Replace("/", "\\").TrimStart(new char[] { '\\' }).TrimEnd(new char[] { '\\' }) + (directory.Length > 0 ? "\\" : "");

                // Get all files with the same name.
                files = System.IO.Directory.GetFiles(folder,
                    filename, System.IO.SearchOption.AllDirectories);

                // Return the first file found.
                return files[0];
            }
            catch (Exception ex)
            {
                // Throw file not found exception.
                throw new Nequeo.Exceptions.InvalidPathException("File not found.", ex);
            }
        }

        /// <summary>
        /// Create a new directory.
        /// </summary>
        /// <param name="directory">The full path of the directory to create.</param>
        /// <returns>True if create else false.</returns>
        public static bool CreateDirectory(string directory)
        {
            try
            {
                if (String.IsNullOrEmpty(directory))
                    directory = "";

                // Get the directory.
                string folder = Helper.GetDownloadPath().TrimEnd(new char[] { '\\' }) + "\\" +
                    directory.Replace("/", "\\").TrimStart(new char[] { '\\' }).TrimEnd(new char[] { '\\' }) + (directory.Length > 0 ? "\\" : "");

                // If the directory does not exist
                // create the directory.
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(folder)))
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(folder));

                return true;
            }
            catch (Exception ex)
            {
                // Throw directory not found exception.
                throw new Nequeo.Exceptions.InvalidPathException("Directory could not be created.", ex);
            }
        }

        /// <summary>
        /// Get the directory path.
        /// </summary>
        /// <param name="directory">The current directory.</param>
        /// <returns>The directory full path.</returns>
        public static string GetDirectoryPath(string directory)
        {
            string folder = null;
            try
            {
                if (String.IsNullOrEmpty(directory))
                    directory = "";

                // Get the directory.
                folder = Helper.GetDownloadPath().TrimEnd(new char[] { '\\' }) + "\\" +
                    directory.Replace("/", "\\").TrimStart(new char[] { '\\' }).TrimEnd(new char[] { '\\' }) + (directory.Length > 0 ? "\\" : "");

                // Return the path.
                return System.IO.Path.GetDirectoryName(folder);
            }
            catch (Exception ex)
            {
                // Throw directory not found exception.
                throw new Nequeo.Exceptions.InvalidPathException("Directory not found.", ex);
            }
        }

        /// <summary>
        /// Get the list of files for the unique identifier.
        /// </summary>
        /// <param name="directory">The directory to get the list from.</param>
        /// <returns>The list of file names.</returns>
        public static string[] GetFileList(string directory)
        {
            string[] files = null;
            try
            {
                if (String.IsNullOrEmpty(directory))
                    directory = "";

                // Get the current folder.
                string folder = Helper.GetDownloadPath().TrimEnd(new char[] { '\\' }) + "\\" +
                    directory.Replace("/", "\\").TrimStart(new char[] { '\\' }).TrimEnd(new char[] { '\\' }) + (directory.Length > 0 ? "\\" : "");
                    
                // Get all files with the same name.
                string[] paths = System.IO.Directory.GetFiles(
                    folder, "*.*", System.IO.SearchOption.AllDirectories);

                // If files have been found.
                if (paths != null && paths.Length > 0)
                {
                    files = new string[paths.Length];
                    for (int i = 0; i < files.Length; i++)
                    {
                        // Get the relative paths and file names.
                        files[i] = paths[i].Replace(Helper.GetDownloadPath(), "");
                    }
                }
                else
                    throw new Exception();

                // Return the files.
                return files;
            }
            catch (Exception ex)
            {
                // Throw file not found exception.
                throw new Nequeo.Exceptions.InvalidPathException("Internal error.", ex);
            }
        }
    }
}