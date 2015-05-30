/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
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
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Nequeo.Maintenance.File
{
    /// <summary>
    /// File retention maintenance helper.
    /// </summary>
	public class Retention
	{
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly Retention Instance = new Retention();

        /// <summary>
        /// Static constructor
        /// </summary>
        static Retention() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public Retention()
        {
        }

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="sourceDirectoryPath">The source directory to look for files to delete.</param>
        public Retention(string sourceDirectoryPath)
        {
            if (String.IsNullOrEmpty(sourceDirectoryPath)) throw new ArgumentNullException("sourceDirectoryPath");
      
            _sourceDirectoryPath = sourceDirectoryPath;
        }

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="lengthToKeepLogFile">The length of time to keep the
        /// file, the value passed is in days.</param>
        /// <param name="sourceDirectoryPath">The source directory to look for files to delete.</param>
        public Retention(double lengthToKeepLogFile, string sourceDirectoryPath)
        {
            if (String.IsNullOrEmpty(sourceDirectoryPath)) throw new ArgumentNullException("sourceDirectoryPath");
            if (lengthToKeepLogFile < (double)0.0) throw new ArgumentOutOfRangeException("lengthToKeepLogFile");

            _lengthToKeepLogFile = lengthToKeepLogFile;
            _sourceDirectoryPath = sourceDirectoryPath;
        }

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="lengthToKeepLogFile">The length of time to keep the
        /// file, the value passed is in days.</param>
        /// <param name="sourceDirectoryPath">The source directory to look for files to delete.</param>
        /// <param name="filePattern">The file pattern to look for files to delete.</param>
        public Retention(double lengthToKeepLogFile, string sourceDirectoryPath, string filePattern)
        {
            if (String.IsNullOrEmpty(sourceDirectoryPath)) throw new ArgumentNullException("sourceDirectoryPath");
            if (lengthToKeepLogFile < (double)0.0) throw new ArgumentOutOfRangeException("lengthToKeepLogFile");
            if (String.IsNullOrEmpty(filePattern)) throw new ArgumentNullException("filePattern");

            _lengthToKeepLogFile = lengthToKeepLogFile;
            _sourceDirectoryPath = sourceDirectoryPath;
            _filePattern = filePattern;
        }

        #endregion

        #region Private Fields
        private string _filePattern = "*.*";
        private string _sourceDirectoryPath = string.Empty;
        private double _lengthToKeepLogFile = 40.0;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the length of time to keep the
        /// file, the value passed is in days.
        /// </summary>
        public double LengthToKeepLogFile
        {
            get { return _lengthToKeepLogFile; }
            set { _lengthToKeepLogFile = value; }
        }

        /// <summary>
        /// Gets sets, the source directory path where deletion begins.
        /// </summary>
        public string SourceDirectoryPath
        {
            get { return _sourceDirectoryPath; }
            set { _sourceDirectoryPath = value; }
        }

        /// <summary>
        /// Gets sets, the file pattern filter.
        /// </summary>
        public string FilePattern
        {
            get { return _filePattern; }
            set { _filePattern = value; }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Start deleting files from the directory that are older then the LengthToKeepLogFile.
        /// </summary>
        public void StartDelete()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(_sourceDirectoryPath)) 
                throw new ArgumentNullException("sourceDirectoryPath");

            DeleteFilesWithinTimeSpan(_lengthToKeepLogFile, GetFiles(_sourceDirectoryPath, _filePattern));
        }

        /// <summary>
        /// Start deleting asyncronously files from the directory that are older then the LengthToKeepLogFile.
        /// </summary>
        /// <param name="deleteCallback">The delete callback method.</param>
        /// <exception cref="System.Exception">The SourceDirectoryPath must be valid.</exception>
        public void StartDeleteAsync(Action<bool> deleteCallback)
        {
            // Make sure that the source directory exist.
            if (String.IsNullOrEmpty(_sourceDirectoryPath))
                throw new ArgumentNullException("sourceDirectoryPath");

            if (deleteCallback == null)
                throw new ArgumentNullException("deleteCallback");

            try
            {
                // Start a new thread if available.
                ThreadPool.QueueUserWorkItem(new WaitCallback(StartDeleteAsyncEx), deleteCallback);
                Thread.Sleep(20);
            }
            catch { }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Start deleting asyncronously file from the directory that are older then the LengthToKeepLogFile.
        /// </summary>
        /// <param name="stateInfo">The state object.</param>
        private void StartDeleteAsyncEx(Object stateInfo)
        {
            Action<bool> deleteCallback = null;
            try
            {
                deleteCallback = (Action<bool>)stateInfo;
                DeleteFilesWithinTimeSpan(_lengthToKeepLogFile, GetFiles(_sourceDirectoryPath, _filePattern));
                deleteCallback(true);
            }
            catch 
            {
                if (deleteCallback != null)
                    deleteCallback(false); 
            }
        }

        /// <summary>
        /// Get the collection of all log files within the source folder.
        /// </summary>
        /// <param name="sourceFolder">The source directory where all the files are located.</param>
        /// <param name="filePattern">The file pattern where all the files are located.</param>
        /// <returns>The collection of file information from the processing
        /// log folder.</returns>
        private List<FileInfo> GetFiles(string sourceFolder, string filePattern)
        {
            try
            {
                // Get the collection of directories.
                string[] directories = System.IO.Directory.GetDirectories(sourceFolder, "*.*", SearchOption.AllDirectories);

                // Create a new instance of the array list.
                List<FileInfo> fileList = new List<FileInfo>();
                FileInfo fileInfo = null;
                string[] files = null;

                // For each file in the root directory 
                // add the file info object.
                files = System.IO.Directory.GetFiles(sourceFolder, filePattern, SearchOption.TopDirectoryOnly);

                // Make sure that some files exists.
                if (files.Length > 0)
                {
                    // for each file within the root directory.
                    for (int i = 0; i < files.Length; i++)
                    {
                        try
                        {
                            // Get the current file information. 
                            // Add to the list collection.
                            fileInfo = new FileInfo(files[i].Trim());
                            fileList.Add(fileInfo);
                        }
                        catch { }
                    }
                }

                // If directories exists then collect files.
                if (directories.Length > 0)
                {
                    // For file in each sub directory, return the
                    // complete file information for the current file.
                    for (int i = 0; i < directories.Length; i++)
                    {
                        try
                        {
                            // Get all the file within the
                            // current sub directory.
                            files = null;
                            files = System.IO.Directory.GetFiles(directories[i].Trim(), filePattern, SearchOption.TopDirectoryOnly);

                            // Make sure that some files exists.
                            if (files.Length > 0)
                            {
                                // for each file within the sub directory.
                                for (int j = 0; j < files.Length; j++)
                                {
                                    try
                                    {
                                        // Get the current file information. 
                                        // Add to the list collection.
                                        fileInfo = new FileInfo(files[j].Trim());
                                        fileList.Add(fileInfo);
                                    }
                                    catch { }
                                }
                            }
                            else
                            {
                                try
                                {
                                    // Delete the directory if it is empty.
                                    System.IO.Directory.Delete(directories[i].Trim());
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                }

                // Return the complete list.
                // file list.
                return fileList;
            }
            catch (System.Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                throw;
            }
        }

        /// <summary>
        /// Delete all files within the directory that are older than the deletion point
        /// time span, each file that is older than the deletion point is deleted from
        /// the specified folder.
        /// </summary>
        /// <param name="deletionPoint">The time span to subtract to start deleting.</param>
        /// <param name="fileList">The file information list to maintain.</param>
        private void DeleteFilesWithinTimeSpan(double deletionPoint, List<FileInfo> fileList)
        {
            try
            {
                if (fileList != null)
                    if (fileList.Count > 0)
                    {
                        // Create a new time span for the deletion point
                        TimeSpan timeSpan = new TimeSpan(Convert.ToInt32(deletionPoint), 0, 0, 0, 0);

                        DateTime modifiedTime;
                        DateTime backDate;

                        // For each file found delete if necessary.
                        foreach (FileInfo oFileInfo in fileList)
                        {
                            try
                            {
                                // Subtract the create deletion time span.
                                backDate = DateTime.Today.Subtract(timeSpan);
                                modifiedTime = oFileInfo.LastWriteTime;

                                // If the back date is greater than the creation date then
                                // delete the log file.
                                if (modifiedTime < backDate)
                                    if (System.IO.File.Exists(oFileInfo.FullName.ToString()))
                                        System.IO.File.Delete(oFileInfo.FullName.ToString());
                            }
                            catch { }
                        }
                    }
            }
            catch (System.Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                throw;
            }
        }
        #endregion
	}
}
