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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.ComponentModel.Composition;

using Nequeo.Model.Conversion;
using Nequeo.Conversion.Common;
using Nequeo.Maintenance.File;
using Nequeo.Exceptions;
using Nequeo.Data.LinqToXml.Format;

namespace Nequeo.Conversion
{
    /// <summary>
    /// Xml conversion wrapper.
    /// </summary>
    [Export(typeof(IConvert))]
    [Export(typeof(IConvertXmlExtender))]
    public class FixedXmlFormat : IConvert, IConvertXmlExtender
    {
        #region Constructors

        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly FixedXmlFormat Instance = new FixedXmlFormat();

        /// <summary>
        /// Static constructor
        /// </summary>
        static FixedXmlFormat() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public FixedXmlFormat()
        {
        }

        #endregion

        #region Private Fields

        private const int BUFFER_SIZE = 0x2000;

        private bool _oneFilePerRow = false;
        private string _fileExt = ".xml";
        private DateTime _currentDate;
        private string _currentXmlWriteFileNameWithoutExt = string.Empty;
        private string _currentIndexFileName = string.Empty;

        private List<long> _failedIrsDocumentExport = new List<long>();
        private List<PathDetailModel> _pathDetailModels = new List<PathDetailModel>();
        private List<ExportBatchModel> _exportBatchList = new List<ExportBatchModel>();
        private List<string> _currentWriteErrors = new List<string>();

        private long _indexFileCount = -1;
        private long _numRecordsRead = 0;
        private long _numRecordsWriiten = 0;
        private string _currentDateTimeFolderName = string.Empty;

        private string _path = string.Empty;
        private string _fileNameWithoutExt = string.Empty;

        #endregion

        #region Public Events

        /// <summary>
        /// On row write/read event error.
        /// </summary>
        public event System.EventHandler<ConversionArgs> OnRowError;

        /// <summary>
        /// On write/read complete.
        /// </summary>
        public event System.EventHandler OnProcessComplete;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets sets, should only one row be placed in each xml file.
        /// </summary>
        /// <remarks>Default value 'false'.</remarks>
        public bool OneRowPerXmlFile
        {
            get { return _oneFilePerRow; }
            set { _oneFilePerRow = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// File maintenance, deletes file that are older that the time specified.
        /// </summary>
        public virtual void FileMaintenance()
        {
            try
            {
                // Validate the archive directory configuration.
                ValidateSetting(Conversion.Properties.Settings.Default.ArchivePath, "ArchivePath");
                ValidateSetting(Conversion.Properties.Settings.Default.LengthOfTimeToKeepArchiveFiles, "LengthOfTimeToKeepArchiveFiles");

                double lengthOfTimeToKeepArchiveFiles = Conversion.Properties.Settings.Default.LengthOfTimeToKeepArchiveFiles;
                string _archivePath = Conversion.Properties.Settings.Default.ArchivePath.TrimEnd('\\') + "\\";

                FileMaintenance(_archivePath, lengthOfTimeToKeepArchiveFiles);
            }
            catch { }
        }

        /// <summary>
        /// File maintenance, deletes file that are older that the time specified.
        /// </summary>
        /// <param name="lengthOfTimeToKeepArchiveFiles">The length in days to keep arcived files.</param>
        public virtual void FileMaintenance(double lengthOfTimeToKeepArchiveFiles)
        {
            try
            {
                // Validate the archive directory configuration.
                ValidateSetting(Conversion.Properties.Settings.Default.ArchivePath, "ArchivePath");
                string _archivePath = Conversion.Properties.Settings.Default.ArchivePath.TrimEnd('\\') + "\\";

                FileMaintenance(_archivePath, lengthOfTimeToKeepArchiveFiles);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// File maintenance, deletes file that are older that the time specified.
        /// </summary>
        /// <param name="sourceDirectoryPath">The dource directory path.</param>
        /// <param name="lengthOfTimeToKeepArchiveFiles">The length in days to keep arcived files.</param>
        public virtual void FileMaintenance(string sourceDirectoryPath, double lengthOfTimeToKeepArchiveFiles)
        {
            try
            {
                // Start a new maintenance task,
                // start deleting old files.
                Retention retention = new Retention(lengthOfTimeToKeepArchiveFiles, sourceDirectoryPath);
                retention.StartDelete();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets all the irs documents that failed to write.
        /// </summary>
        /// <returns>The list of failures; else empty collection.</returns>
        public virtual Int64[] GetAllWriteFailedIrsDocuments()
        {
            return _failedIrsDocumentExport.ToArray();
        }

        /// <summary>
        /// Gets the final conversion path after processing has completed.
        /// </summary>
        /// <returns>The final conversion path.</returns>
        public virtual string GetFinalConversionPath()
        {
            return Conversion.Properties.Settings.Default.ArchivePath.TrimEnd('\\') + "\\" + _currentDateTimeFolderName + "\\";
        }

        /// <summary>
        /// Gets the list of read errors if any.
        /// </summary>
        /// <returns>The list of errors; else empty collection.</returns>
        public virtual string[] GetReadErrors()
        {
            return new string[0];
        }

        /// <summary>
        /// Get the number of records read.
        /// </summary>
        /// <returns>The number of records read.</returns>
        public virtual long GetRecordsRead()
        {
            return _numRecordsRead;
        }

        /// <summary>
        /// Get the number of records written.
        /// </summary>
        /// <returns>The number of records written.</returns>
        public virtual long GetRecordsWritten()
        {
            return _numRecordsWriiten;
        }

        /// <summary>
        /// Gets the returned data from the conversion write process
        /// </summary>
        /// <returns>The list of returned data.</returns>
        public virtual List<ExportBatchModel> GetWriteData()
        {
            return _exportBatchList;
        }

        /// <summary>
        /// Gets the list of write errors if any.
        /// </summary>
        /// <returns>The list of errors; else empty collection.</returns>
        public virtual string[] GetWriteErrors()
        {
            return _currentWriteErrors.ToArray();
        }

        /// <summary>
        /// Before a read operation execute this method.
        /// </summary>
        /// <returns>True if the execution was successful else false.</returns>
        public virtual bool OnReadBegin()
        {
            return false;
        }

        /// <summary>
        /// When a read operation from another process has completed execute this method.
        /// </summary>
        /// <param name="processResult">The process result from the preceeding operation.</param>
        /// <param name="errors">The list of errors if any.</param>
        /// <param name="recordsRead">The number of records read.</param>
        /// <returns>True if the execution was successful else false.</returns>
        public virtual bool OnReadComplete(bool processResult, String[] errors, long recordsRead)
        {
            return false;
        }

        /// <summary>
        /// After a read operation execute this method.
        /// </summary>
        /// <returns>True if the execution was successful else false.</returns>
        public virtual bool OnReadEnd()
        {
            return false;
        }

        /// <summary>
        /// Before a write operation execute this method.
        /// </summary>
        /// <returns>True if the execution was successful else false.</returns>
        public virtual bool OnWriteBegin()
        {
            try
            {
                // Perform file maintenence.
                FileMaintenance();

                _currentDate = DateTime.Now;
                _currentDateTimeFolderName = _currentDate.ToString("dd-MM-yy");

                // If the directory does not exist then create it.
                if (!Directory.Exists(GetFinalConversionPath()))
                    Directory.CreateDirectory(GetFinalConversionPath());

                return true;
            }
            catch (Exception ex)
            {
                // Add the error message.
                _currentWriteErrors.Add(
                    "Type : FixedXmlFormat |\r\n" +
                    "Member : OnWriteBegin |\r\n" +
                    "Message : " + ex.Message + "|\r\n" +
                    "Stack : " + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// The final conversion path extension if any
        /// </summary>
        /// <returns>The final conversion extension name.</returns>
        public virtual string GetFinalConversionPathExtension()
        {
            return _currentDateTimeFolderName;
        }

        /// <summary>
        /// When a write operation from another process has completed execute this method.
        /// </summary>
        /// <param name="processResult">The process result from the preceeding operation.</param>
        /// <param name="errors">The list of errors if any.</param>
        /// <param name="recordsWritten">The number of records wriiten.</param>
        /// <returns>True if the execution was successful else false.</returns>
        public virtual bool OnWriteComplete(bool processResult, String[] errors, long recordsWritten)
        {
            return false;
        }

        /// <summary>
        /// After a write operation execute this method.
        /// </summary>
        /// <returns>True if the execution was successful else false.</returns>
        public virtual bool OnWriteEnd()
        {
            bool ret = true;
            bool retBase = true;

            // For each path model get the files that should
            // be copied to the final location.
            foreach (PathDetailModel model in _pathDetailModels)
            {
                try
                {
                    // Copy and delete the process files.
                    ret = CopyAndDeleteProcessedFiles(model.FolderName, model.FileName);
                    if (!ret) retBase = false;
                }
                catch (Exception ex)
                {
                    // Add the error message.
                    _currentWriteErrors.Add(
                        "Type : FixedXmlFormat |\r\n" +
                        "Member : OnWriteEnd |\r\n" +
                        "Message : " + ex.Message + "|\r\n" +
                        "Stack : " + ex.StackTrace);

                    // Return the result.
                    retBase = false;
                }
            }

            // The control file has been created.
            return retBase;
        }

        /// <summary>
        /// Read the data from the file.
        /// </summary>
        /// <returns>The collection of data.</returns>
        /// <exception cref="System.ArgumentNullException">Parameter is null</exception>
        public virtual Object Read()
        {
            return null;
        }

        /// <summary>
        /// Write the data from the transform model.
        /// </summary>
        /// <param name="data">The data to be written.</param>
        /// <returns>True is all data has been written else false.</returns>
        /// <exception cref="System.ArgumentNullException">Array is null</exception>
        /// <exception cref="System.ArgumentNullException">Parameter is null</exception>
        /// <exception cref="System.IndexOutOfRangeException">Array contains no data.</exception>
        public virtual bool Write(object data)
        {
            // Make sure that all params are valid.
            if (data == null)
                throw new ArgumentNullException("data");

            bool ret = false;

            try
            {
                ValidateSetting(Conversion.Properties.Settings.Default.OutputXmlPath, "OutputCsvPath");

                // Assign configuration values.
                OneRowPerXmlFile = Conversion.Properties.Settings.Default.OneRowPerXmlFile;

                _path = System.IO.Path.GetDirectoryName(Conversion.Properties.Settings.Default.OutputXmlPath);
                _fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(Conversion.Properties.Settings.Default.OutputXmlPath);
                _fileExt = System.IO.Path.GetExtension(Conversion.Properties.Settings.Default.OutputXmlPath);

            }
            catch (EmptyStringException esex)
            {
                // Add the error message.
                _currentWriteErrors.Add(
                    "Type : FixedXmlFormat |\r\n" +
                    "Member : Write |\r\n" +
                    "Message : " + esex.Message + "|\r\n" +
                    "Stack : " + esex.StackTrace);
                return false;
            }

            // Create the current file name.
            _currentXmlWriteFileNameWithoutExt =
                _path + "\\" + _fileNameWithoutExt + "_" +
                _currentDate.ToString("dd") + _currentDate.ToString("MM") + _currentDate.ToString("yyyy") + "_" +
                _currentDate.ToString("HH") + _currentDate.ToString("mm") + _currentDate.ToString("ss");

            // Determine the type of the object that is passed.
            if (data is List<TransformModel[]>)
            {
                // Write transform data.
                List<TransformModel[]> dataCol = (List<TransformModel[]>)data;
                return WriteTransformModelCollection(dataCol);
            }
            else if (data is TransformXmlModel)
            {
                // Write transform xml data.
                TransformXmlModel dataCol = (TransformXmlModel)data;
                return WriteTransformXmlModelCollection(dataCol);
            }
            else
                return ret;
        }

        /// <summary>
        /// Write the data to the file.
        /// </summary>
        /// <param name="data">The data to be written.</param>
        /// <param name="fileName">The file name and path where the data is to be written.</param>
        /// <returns>True is all data has been written else false.</returns>
        public virtual bool Write(TransformXmlModel data, string fileName)
        {
            // Make sure that all params are valid.
            if (data == null)
                throw new ArgumentNullException("data");
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");
            if (data.Nodes.Count() < 1)
                return false;

            bool ret = false;
            try
            {
                // If the directory does not exist then create it.
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(fileName)))
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileName));

                // If one file per record is to be written.
                if (_oneFilePerRow)
                    throw new NotImplementedException();
                else
                {
                    // Write the data to the xml file.
                    ret = WriteToXmlStream(data, fileName);
                    _numRecordsWriiten = data.Nodes.Count();
                }
            }
            catch (Exception ex)
            {
                // Add the error message.
                _currentWriteErrors.Add(
                    "Type : FixedXmlFormat |\r\n" +
                    "Member : Write |\r\n" +
                    "Message : " + ex.Message + "|\r\n" +
                    "Stack : " + ex.StackTrace);
                ret = false;
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Write the data to the file.
        /// </summary>
        /// <param name="node">The xml node to create.</param>
        /// <param name="data">The data specific to the current node.</param>
        /// <param name="fileName">The file name to write to.</param>
        /// <param name="version">The xml version.</param>
        /// <param name="encoding">The xml encoding</param>
        /// <param name="standalone">The xml standalone</param>
        /// <returns>True is all data has been written else false.</returns>
        public virtual bool Write(XElement node, TransformModel data, string fileName, string version, string encoding, string standalone)
        {
            // Make sure that all params are valid.
            if (node == null)
                throw new ArgumentNullException("node");
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            bool ret = false;
            try
            {
                // If the directory does not exist then create it.
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(fileName)))
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileName));

                // Write the data to the xml file.
                ret = WriteToXmlStream(node, data, fileName, version, encoding, standalone);
                _numRecordsWriiten = _indexFileCount + 1;
            }
            catch (Exception ex)
            {
                // Add the error message.
                _currentWriteErrors.Add(
                    "Type : FixedXmlFormat |\r\n" +
                    "Member : Write |\r\n" +
                    "Message : " + ex.Message + "|\r\n" +
                    "Stack : " + ex.StackTrace);
                ret = false;
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Write the data to the file.
        /// </summary>
        /// <param name="data">The data to be written.</param>
        /// <param name="fileName">The file name and path where the data is to be written.</param>
        /// <returns>True is all data has been written else false.</returns>
        public virtual bool Write(List<TransformModel[]> data, string fileName)
        {
            // Make sure that all params are valid.
            if (data == null)
                throw new ArgumentNullException("data");
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");
            if (data.Count < 1)
                return false;

            bool ret = false;
            FileStream fileStream = null;
            try
            {
                // If the directory does not exist then create it.
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(fileName)))
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileName));

                // If one file per record is to be written.
                if (_oneFilePerRow)
                    throw new NotImplementedException();
                else
                {
                    // Create a new file stream from the file path.
                    fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, BUFFER_SIZE, true);
                    ret = Write(data, fileStream);
                }
            }
            finally
            {
                // Close the stream.
                if (fileStream != null)
                    fileStream.Close();
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Write the data to the file stream.
        /// </summary>
        /// <param name="data">The data to be written.</param>
        /// <param name="fileStream">The file stream where the data is to be written.</param>
        /// <returns>True is all data has been written else false.</returns>
        /// <exception cref="System.ArgumentNullException">Array is null</exception>
        /// <exception cref="System.ArgumentNullException">Stream is null</exception>
        /// <exception cref="System.IndexOutOfRangeException">Array contains no data.</exception>
        public virtual bool Write(List<TransformModel[]> data, FileStream fileStream)
        {
            // Make sure that all params are valid.
            if (data == null)
                throw new ArgumentNullException("data");
            if (fileStream == null)
                throw new ArgumentNullException("fileStream");
            if (data.Count < 1)
                return false;

            bool ret = false;

            // If one file per record is to be written.
            if (_oneFilePerRow)
                throw new NotImplementedException();
            else
            {
                _currentIndexFileName = fileStream.Name;

                // Write the data to the underlying stream.
                ret = WriteToStream(data, fileStream);
            }

            // All data has been written.
            return ret;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Write the transform model type collection.
        /// </summary>
        /// <param name="data">The transform model collection data.</param>
        /// <returns>True is all data has been written else false.</returns>
        private bool WriteTransformXmlModelCollection(TransformXmlModel data)
        {
            bool ret = false;
            bool baseResult = true;

            // If no data exists then return false.
            if (data.Nodes == null || data.Nodes.Count() < 1)
                return false;

            // If one file per record is to be written.
            if (_oneFilePerRow)
            {
                // Cast the data collection.
                XElement[] nodes = data.Nodes;
                int nodeIndex = -1;

                // For each data collection in the list
                // write the data to the underlying stream.
                foreach (XElement item in nodes)
                {
                    nodeIndex++;

                    // Get the current element data.
                    TransformModel elementData = data.Data[nodeIndex];
                    string specificFileName = string.Empty;

                    // Increment the count.
                    _indexFileCount++;

                    try
                    {
                        // Get a specific file if it exists.
                        specificFileName = data.FileNames[nodeIndex];

                        // Create the current file name.
                        _currentXmlWriteFileNameWithoutExt = _path + "\\" + specificFileName;
                    }
                    catch { specificFileName = string.Empty; }

                    try
                    {
                        // Get the current suffix if any
                        string sufixFileName = (String.IsNullOrEmpty(elementData.SufixFileName) ? string.Empty : "_" + elementData.SufixFileName);

                        if (String.IsNullOrEmpty(specificFileName))
                            // Get the current index filename.
                            _currentIndexFileName = _currentXmlWriteFileNameWithoutExt + "_" + _indexFileCount.ToString() + sufixFileName + _fileExt;
                        else
                            // Get the specific file name passed for the xml.
                            _currentIndexFileName = _currentXmlWriteFileNameWithoutExt + _fileExt;

                        // If the directory does not exist then create it.
                        if (!Directory.Exists(System.IO.Path.GetDirectoryName(_currentIndexFileName)))
                            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_currentIndexFileName));

                        // Write the data to the xml file.
                        ret = Write(item, elementData, _currentIndexFileName, data.Version, data.Encoding, data.Standalone);
                        if (!ret) baseResult = false;

                        // Create the control file name.
                        if (String.IsNullOrEmpty(specificFileName))
                        {
                            if (ret) ret = CreateControlFiles(_currentXmlWriteFileNameWithoutExt + "_" + _indexFileCount.ToString() + sufixFileName + ".ctl");
                        }
                        else
                        {
                            if (ret) ret = CreateControlFiles(_currentXmlWriteFileNameWithoutExt + ".ctl");
                        }

                        if (!ret) baseResult = false;

                        // Create the control file name.
                        if (String.IsNullOrEmpty(specificFileName))
                        {
                            // Create the final location model
                            if (ret)
                                _pathDetailModels.Add(
                                    new PathDetailModel()
                                    {
                                        FolderName = _currentDateTimeFolderName,
                                        FileName = _currentXmlWriteFileNameWithoutExt + "_" + _indexFileCount.ToString() + sufixFileName
                                    });
                        }
                        else
                        {
                            // Create the final location model
                            if (ret)
                                _pathDetailModels.Add(
                                    new PathDetailModel()
                                    {
                                        FolderName = _currentDateTimeFolderName,
                                        FileName = _currentXmlWriteFileNameWithoutExt
                                    });
                        }

                        if (!ret) baseResult = false;
                    }
                    catch (Exception ex)
                    {
                        ret = false;
                        baseResult = false;

                        // Add the error message.
                        _currentWriteErrors.Add(
                            "Type : FixedXmlFormat |\r\n" +
                            "Member : WriteTransformXmlModelCollection |\r\n" +
                            "Message : " + ex.Message + "|\r\n" +
                            "Stack : " + ex.StackTrace);

                        // If the event has been attached, then
                        // send an error to the client.
                        if (OnRowError != null)
                            OnRowError(this, new ConversionArgs(_indexFileCount, ex.Message));
                    }
                }

                // Return the result.
                ret = baseResult;
                return ret;
            }
            else
            {
                // Attempt to get the first item array.
                TransformModel[] firstItem = data.Data;
                string sufixFileName = string.Empty;
                if (firstItem.Count() > 0)
                    sufixFileName = (String.IsNullOrEmpty(firstItem.First().SufixFileName) ? string.Empty : "_" + firstItem.First().SufixFileName);

                // Curent index file name without extension.
                _currentIndexFileName = _currentXmlWriteFileNameWithoutExt + sufixFileName + _fileExt;

                // Write the data to the file.
                ret = Write((TransformXmlModel)data, _currentIndexFileName);
                if (!ret) baseResult = false;

                // Create the control file name.
                if (ret) ret = CreateControlFiles(_currentXmlWriteFileNameWithoutExt + sufixFileName + ".ctl");
                if (!ret) baseResult = false;

                // Create the final location model
                if (ret)
                    _pathDetailModels.Add(
                        new PathDetailModel()
                        {
                            FolderName = _currentDateTimeFolderName,
                            FileName = _currentXmlWriteFileNameWithoutExt + sufixFileName
                        });

                ret = baseResult;
                return ret;
            }
        }

        /// <summary>
        /// Write the transform model type collection.
        /// </summary>
        /// <param name="data">The transform model collection data.</param>
        /// <returns>True is all data has been written else false.</returns>
        private bool WriteTransformModelCollection(List<TransformModel[]> data)
        {
            bool ret = false;
            bool baseResult = true;

            // If no data exists then return false.
            if (data.Count < 1)
                return false;

            // If one file per record is to be written.
            if (_oneFilePerRow)
            {
                FileStream fileStream = null;

                // For each data collection in the list
                // write the data to the underlying stream.
                foreach (TransformModel[] items in data)
                {
                    // Increment the count.
                    _indexFileCount++;

                    try
                    {
                        // Get the current suffix if any
                        string sufixFileName = (String.IsNullOrEmpty(items[0].SufixFileName) ? string.Empty : "_" + items[0].SufixFileName);

                        // Get the current index filename.
                        _currentIndexFileName = _currentXmlWriteFileNameWithoutExt + "_" + _indexFileCount.ToString() + sufixFileName + _fileExt;

                        // If the directory does not exist then create it.
                        if (!Directory.Exists(System.IO.Path.GetDirectoryName(_currentIndexFileName)))
                            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_currentIndexFileName));

                        // Create a new file stream from the file path.
                        fileStream = new FileStream(_currentIndexFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, BUFFER_SIZE, true);
                        ret = WriteToStream(items, fileStream);
                        fileStream.Close();
                        if (!ret) baseResult = false;

                        // Create the control file name.
                        if (ret) ret = CreateControlFiles(_currentXmlWriteFileNameWithoutExt + "_" + _indexFileCount.ToString() + sufixFileName + ".ctl");
                        if (!ret) baseResult = false;

                        // Create the final location model
                        if (ret)
                            _pathDetailModels.Add(
                                new PathDetailModel()
                                {
                                    FolderName = _currentDateTimeFolderName,
                                    FileName = _currentXmlWriteFileNameWithoutExt + "_" + _indexFileCount.ToString() + sufixFileName
                                });

                        if (!ret) baseResult = false;
                    }
                    catch (Exception ex)
                    {
                        ret = false;

                        // Add the error message.
                        _currentWriteErrors.Add(
                            "Type : FixedXmlFormat |\r\n" +
                            "Member : WriteTransformModelCollection |\r\n" +
                            "Message : " + ex.Message + "|\r\n" +
                            "Stack : " + ex.StackTrace);

                        // If the event has been attached, then
                        // send an error to the client.
                        if (OnRowError != null)
                            OnRowError(this, new ConversionArgs(_indexFileCount, ex.Message));
                    }
                    finally
                    {
                        // Close the stream.
                        if (fileStream != null)
                            fileStream.Close();
                    }
                }

                // Return the result.
                ret = baseResult;
                return ret;
            }
            else
            {
                // Attempt to get the first item array.
                TransformModel[] firstItem = data.First();
                string sufixFileName = string.Empty;
                if (firstItem.Count() > 0)
                    sufixFileName = (String.IsNullOrEmpty(firstItem.First().SufixFileName) ? string.Empty : "_" + firstItem.First().SufixFileName);

                // Curent index file name without extension.
                _currentIndexFileName = _currentXmlWriteFileNameWithoutExt + sufixFileName + _fileExt;

                // Write the data to the file.
                ret = Write((List<TransformModel[]>)data, _currentIndexFileName);
                if (!ret) baseResult = false;

                // Create the control file name.
                if (ret) ret = CreateControlFiles(_currentXmlWriteFileNameWithoutExt + sufixFileName + ".ctl");
                if (!ret) baseResult = false;

                // Create the final location model
                if (ret)
                    _pathDetailModels.Add(
                        new PathDetailModel()
                        {
                            FolderName = _currentDateTimeFolderName,
                            FileName = _currentXmlWriteFileNameWithoutExt + sufixFileName
                        });

                ret = baseResult;
                return ret;
            }
        }

        /// <summary>
        /// Write the data to the underlying stream.
        /// </summary>
        /// <param name="data">The data to be written.</param>
        /// <param name="fileName">The file name and path where the data is to be written.</param>
        /// <returns>True is all data has been written else false.</returns>
        private bool WriteToXmlStream(TransformXmlModel data, string fileName)
        {
            // For each data model in the collection
            // attempt to create the line of data.
            foreach (TransformModel item in data.Data)
            {
                ExportBatchModel exportBatchModel = new ExportBatchModel();
                try
                {
                    // Add the export batch information to the collection.
                    exportBatchModel.Level1ID = (long)item.Level1ID;
                    exportBatchModel.Level2ID = (long)item.Level2ID;
                    exportBatchModel.Level3ID = (long)item.Level3ID;
                    exportBatchModel.ExportName = fileName;
                    exportBatchModel.Data = item.Data;
                    exportBatchModel.Value = item.Value;
                    exportBatchModel.ItemName = item.ValueName;
                }
                catch { }

                // Add the export data.
                _exportBatchList.Add(exportBatchModel);
            }

            // Write the data to the xml file.
            XmlTransformModel xmlFormatter = new XmlTransformModel();
            return xmlFormatter.CreateDocument(data, fileName);
        }

        /// <summary>
        /// Write the data to the underlying stream.
        /// </summary>
        /// <param name="node">The xml node to create.</param>
        /// <param name="data">The data specific to the current node.</param>
        /// <param name="fileName">The file name to write to.</param>
        /// <param name="version">The xml version.</param>
        /// <param name="encoding">The xml encoding</param>
        /// <param name="standalone">The xml standalone</param>
        /// <returns>True is all data has been written else false.</returns>
        private bool WriteToXmlStream(XElement node, TransformModel data, string fileName, string version, string encoding, string standalone)
        {
            ExportBatchModel exportBatchModel = new ExportBatchModel();
            try
            {
                // Add the export batch information to the collection.
                exportBatchModel.Level1ID = (long)data.Level1ID;
                exportBatchModel.Level2ID = (long)data.Level2ID;
                exportBatchModel.Level3ID = (long)data.Level3ID;
                exportBatchModel.ExportName = fileName;
                exportBatchModel.Data = data.Data;
                exportBatchModel.Value = data.Value;
                exportBatchModel.ItemName = data.ValueName;
            }
            catch { }

            // Add the export data.
            _exportBatchList.Add(exportBatchModel);

            // Write the data to the xml file.
            XmlTransformModel xmlFormatter = new XmlTransformModel();
            return xmlFormatter.CreateDocument(node, fileName, version, encoding, standalone);
        }

        /// <summary>
        /// Write the data to the underlying stream.
        /// </summary>
        /// <param name="data">The data to be written.</param>
        /// <param name="stream">The stream to write to.</param>
        /// <returns>True is all data has been written else false.</returns>
        private bool WriteToStream(TransformModel[] data, Stream stream)
        {
            bool ret = true;
            long _indexTotalRows = -1;

            // For each data model in the collection
            // attempt to create the line of data.
            foreach (TransformModel item in data)
            {
                ExportBatchModel exportBatchModel = new ExportBatchModel();
                try
                {
                    // Add the export batch information to the collection.
                    exportBatchModel.Level1ID = (long)item.Level1ID;
                    exportBatchModel.Level2ID = (long)item.Level2ID;
                    exportBatchModel.Level3ID = (long)1;
                    exportBatchModel.ExportName = _currentIndexFileName;
                    exportBatchModel.Data = item.Data;
                    exportBatchModel.Value = item.Value;
                    exportBatchModel.ItemName = item.ValueName;
                }
                catch { }

                // Add the export data.
                _exportBatchList.Add(exportBatchModel);
            }

            try
            {
                // Close the stream at this point.
                stream.Close();

                // Write the data to the xml file.
                XmlTransformModel xmlFormatter = new XmlTransformModel();
                ret = xmlFormatter.CreateDocument(new List<TransformModel[]>() { data }, _currentIndexFileName);
            }
            catch (Exception ex)
            {
                // Add the error message.
                _currentWriteErrors.Add(
                    "Type : FixedXmlFormat |\r\n" +
                    "Member : WriteToStream |\r\n" +
                    "Message : " + ex.Message + "|\r\n" +
                    "Stack : " + ex.StackTrace);
                ret = false;
            }

            // If the event has been attached, then
            // send a complete to the client.
            if (OnProcessComplete != null)
                OnProcessComplete(this, new EventArgs());

            // Return the result.
            _numRecordsWriiten = (long)(_indexTotalRows + 1);
            return ret;
        }

        /// <summary>
        /// Write the data to the underlying stream.
        /// </summary>
        /// <param name="data">The data to be written.</param>
        /// <param name="stream">The stream to write to.</param>
        /// <returns>True is all data has been written else false.</returns>
        private bool WriteToStream(List<TransformModel[]> data, Stream stream)
        {
            bool ret = true;
            long index = -1;

            // For each data collection in the list
            // write the data to the underlying stream.
            foreach (TransformModel[] items in data)
            {
                index++;

                // For each data model in the collection
                // attempt to create the line of data.
                foreach (TransformModel item in items)
                {
                    ExportBatchModel exportBatchModel = new ExportBatchModel();
                    try
                    {
                        // Add the export batch information to the collection.
                        exportBatchModel.Level1ID = (long)item.Level1ID;
                        exportBatchModel.Level2ID = (long)item.Level2ID;
                        exportBatchModel.Level3ID = (long)data.Count;
                        exportBatchModel.ExportName = _currentIndexFileName;
                        exportBatchModel.Data = item.Data;
                        exportBatchModel.Value = item.Value;
                        exportBatchModel.ItemName = item.ValueName;
                    }
                    catch { }

                    // Add the export data.
                    _exportBatchList.Add(exportBatchModel);
                }
            }

            try
            {
                // Close the stream at this point.
                stream.Close();

                // Write the data to the xml file.
                XmlTransformModel xmlFormatter = new XmlTransformModel();
                ret = xmlFormatter.CreateDocument(data, _currentIndexFileName);
            }
            catch (Exception ex)
            {
                // Add the error message.
                _currentWriteErrors.Add(
                    "Type : FixedXmlFormat |\r\n" +
                    "Member : WriteToStream |\r\n" +
                    "Message : " + ex.Message + "|\r\n" +
                    "Stack : " + ex.StackTrace);
                ret = false;
            }

            // If the event has been attached, then
            // send a complete to the client.
            if (OnProcessComplete != null)
                OnProcessComplete(this, new EventArgs());

            // Return the result.
            _numRecordsWriiten = (long)(index + 1);
            return ret;
        }

        /// <summary>
        /// Create the control files.
        /// </summary>
        /// <param name="controlFileName">The control file name.</param>
        private bool CreateControlFiles(string controlFileName)
        {
            FileStream stream = null;

            try
            {
                // If the directory does not exist then create it.
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(controlFileName)))
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(controlFileName));

                // Attempt to create the control file.
                stream = System.IO.File.Create(controlFileName);
                stream.Close();

                // Return the result.
                return true;
            }
            catch (Exception ex)
            {
                // Add the error message.
                _currentWriteErrors.Add(
                    "Type : FixedXmlFormat |\r\n" +
                    "Member : CreateControlFiles |\r\n" +
                    "Message : " + ex.Message + "|\r\n" +
                    "Stack : " + ex.StackTrace);

                // Return the result.
                return false;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        /// <summary>
        /// Copy and delete the processed files.
        /// </summary>
        /// <param name="currentDateTimeFolderName">The current date time folder name.</param>
        /// <param name="fileNameWithoutExt">The current file name without extension.</param>
        /// <returns>True if successful;else false.</returns>
        private bool CopyAndDeleteProcessedFiles(string currentDateTimeFolderName, string fileNameWithoutExt)
        {
            bool ret = true;
            try
            {
                bool copyCsv = true;
                bool copyCtl = true;

                ValidateSetting(Conversion.Properties.Settings.Default.ArchivePath, "ArchivePath");

                // If the directory does not exist then create it.
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(Conversion.Properties.Settings.Default.ArchivePath.TrimEnd('\\') + "\\" + currentDateTimeFolderName + "\\")))
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Conversion.Properties.Settings.Default.ArchivePath.TrimEnd('\\') + "\\" + currentDateTimeFolderName + "\\"));

                try
                {
                    // Copy the xml file to the archive location.
                    File.Copy(fileNameWithoutExt + _fileExt,
                        Conversion.Properties.Settings.Default.ArchivePath.TrimEnd('\\') + "\\" + currentDateTimeFolderName + "\\" +
                        System.IO.Path.GetFileNameWithoutExtension(fileNameWithoutExt + _fileExt) + _fileExt, true);
                }
                catch (Exception ex)
                {
                    // Add the error message.
                    _currentWriteErrors.Add(
                        "Type : FixedXmlFormat |\r\n" +
                        "Member : CopyAndDeleteProcessedFiles |\r\n" +
                        "Message : " + ex.Message + "|\r\n" +
                        "Stack : " + ex.StackTrace);
                    copyCsv = false;
                    ret = true;
                }

                try
                {
                    // Copy the control file to the archive location.
                    File.Copy(fileNameWithoutExt + ".ctl",
                        Conversion.Properties.Settings.Default.ArchivePath.TrimEnd('\\') + "\\" + currentDateTimeFolderName + "\\" +
                        System.IO.Path.GetFileNameWithoutExtension(fileNameWithoutExt + ".ctl") + ".ctl", true);
                }
                catch (Exception ex)
                {
                    // Add the error message.
                    _currentWriteErrors.Add(
                        "Type : FixedXmlFormat |\r\n" +
                        "Member : CopyAndDeleteProcessedFiles |\r\n" +
                        "Message : " + ex.Message + "|\r\n" +
                        "Stack : " + ex.StackTrace);
                    copyCtl = false;
                    ret = true;
                }

                try
                {
                    if (copyCsv)
                        // Delete the csv file from the output location.
                        File.Delete(fileNameWithoutExt + _fileExt);
                }
                catch (Exception ex)
                {
                    // Add the error message.
                    _currentWriteErrors.Add(
                        "Type : FixedXmlFormat |\r\n" +
                        "Member : CopyAndDeleteProcessedFiles |\r\n" +
                        "Message : " + ex.Message + "|\r\n" +
                        "Stack : " + ex.StackTrace);
                    ret = true;
                }

                try
                {
                    if (copyCtl)
                        // Delete the control file from the output location.
                        File.Delete(fileNameWithoutExt + ".ctl");
                }
                catch (Exception ex)
                {
                    // Add the error message.
                    _currentWriteErrors.Add(
                        "Type : FixedXmlFormat |\r\n" +
                        "Member : CopyAndDeleteProcessedFiles |\r\n" +
                        "Message : " + ex.Message + "|\r\n" +
                        "Stack : " + ex.StackTrace);
                    ret = true;
                }

                // Return the result.
                return ret;
            }
            catch (EmptyStringException esex)
            {
                // Add the error message.
                _currentWriteErrors.Add(
                    "Type : FixedXmlFormat |\r\n" +
                    "Member : CopyAndDeleteProcessedFiles |\r\n" +
                    "Message : " + esex.Message + "|\r\n" +
                    "Stack : " + esex.StackTrace);
                return false;
            }
            catch
            {
                // Return the result.
                return false;
            }
        }

        /// <summary>
        /// Validate the input data.
        /// </summary>
        /// <param name="value">The object value.</param>
        /// <param name="memberName">The name of the member.</param>
        /// <returns>The object value</returns>
        /// <exception cref="System.Exception"></exception>
        private object ValidateSetting(object value, string memberName)
        {
            switch (value.GetType().FullName.ToLower())
            {
                case "system.string":
                case "string":
                    if (String.IsNullOrEmpty(value.ToString()))
                        throw new EmptyStringException(memberName + " " + Nequeo.Conversion.Properties.Resources.NoValue);
                    else
                        return value;

                case "system.int16":
                case "short":
                case "system.int64":
                case "long":
                case "system.int32":
                case "int":
                    if (String.IsNullOrEmpty(value.ToString()))
                        throw new EmptyStringException(memberName + " " + Nequeo.Conversion.Properties.Resources.NoValue);
                    else
                    {
                        long number;
                        bool ret = long.TryParse(value.ToString(), out number);
                        if (!ret)
                            throw new System.ArithmeticException(memberName + " " + Nequeo.Conversion.Properties.Resources.NotNumber);
                        else
                            return value;
                    }

                case "system.boolean":
                case "bool":
                    if (String.IsNullOrEmpty(value.ToString()))
                        throw new EmptyStringException(memberName + " " + Nequeo.Conversion.Properties.Resources.NoValue);
                    else
                    {
                        bool number;
                        bool ret = bool.TryParse(value.ToString(), out number);
                        if (!ret)
                            throw new System.ArithmeticException(memberName + " " + Nequeo.Conversion.Properties.Resources.NotBoolean);
                        else
                            return value;
                    }

                default:
                    return value;
            }
        }

        #endregion
    }
}
