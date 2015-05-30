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
using System.Text.RegularExpressions;
using System.IO;
using System.ComponentModel.Composition;

using Nequeo.Model.Conversion;
using Nequeo.Conversion.Common;
using Nequeo.Maintenance.File;
using Nequeo.Exceptions;

namespace Nequeo.Conversion
{
    /// <summary>
    /// CSV conversion wrapper.
    /// </summary>
    [Export(typeof(IConvert))]
    [Export(typeof(IConvertExtender))]
    public class FixedCsvFormat : IConvert, IConvertExtender
    {
        #region Constructor

        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly FixedCsvFormat Instance = new FixedCsvFormat();

        /// <summary>
        /// Static constructor
        /// </summary>
        static FixedCsvFormat() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public FixedCsvFormat()
        {
        }

        #endregion

        #region Private Fields

        private const int BUFFER_SIZE = 0x2000;

        private string _columnDelimiter = "|";
        private string _rowDelimiter = "\r\n";
        private bool _includeHeaders = true;
        private bool _includeCategoryID = false;
        private List<string> _currentWriteErrors = new List<string>();
        private long _numRecordsRead = 0;
        private long _numRecordsWriiten = 0;
        private string _currentCsvWriteFileNameWithoutExt = string.Empty;

        private string _fileExt = ".csv";
        private List<ExportBatchModel> _exportBatchList = new List<ExportBatchModel>();
        private List<PathDetailModel> _pathDetailModels = new List<PathDetailModel>();
        private DateTime _currentDate;
        private bool _oneFilePerRow = false;
        private long _indexTotalRows = -1;
        private long _indexFileCount = -1;
        private List<long> _failedIrsDocumentExport = new List<long>();
        private string _currentIndexFileName = string.Empty;
        private string _currentDateTimeFolderName = string.Empty;

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
        /// Gets sets, the column delimiter
        /// </summary>
        /// <remarks>Default value '|' (comma).</remarks>
        public string ColumnDelimiter
        {
            get { return _columnDelimiter; }
            set { _columnDelimiter = value; }
        }

        /// <summary>
        /// Gets sets, the row delimiter
        /// </summary>
        /// <remarks>Default value '\r\n' (CR - LF).</remarks>
        public string RowDelimiter
        {
            get { return _rowDelimiter; }
            set { _rowDelimiter = value; }
        }

        /// <summary>
        /// Gets sets, should column headers be included at the begining.
        /// </summary>
        /// <remarks>Default value 'true'.</remarks>
        public bool IncludeHeaders
        {
            get { return _includeHeaders; }
            set { _includeHeaders = value; }
        }

        /// <summary>
        /// Gets sets, should the category id be included as the first column.
        /// </summary>
        /// <remarks>Default value 'false'.</remarks>
        public bool IncludeCategoryID
        {
            get { return _includeCategoryID; }
            set { _includeCategoryID = value; }
        }

        /// <summary>
        /// Gets sets, should only one row be placed in each csv file.
        /// </summary>
        /// <remarks>Default value 'false'.</remarks>
        public bool OneRowPerCsvFile
        {
            get { return _oneFilePerRow; }
            set { _oneFilePerRow = value; }
        }

        #endregion

        /// <summary>
        /// Split the csv text.
        /// </summary>
        /// <param name="text">The text to split.</param>
        /// <returns>The collection of split items.</returns>
        public static string[] Split(string text)
        {
            return Regex.Split(text, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
        }

        /// <summary>
        /// Split the csv text items.
        /// </summary>
        /// <param name="textItems">The collection of text to split.</param>
        /// <returns>The collection of split items.</returns>
        public static List<string[]> Split(string[] textItems)
        {
            List<string[]> lines = new List<string[]>();

            // For each text items
            for (int i = 0; i < textItems.Length; i++)
            {
                // Add the line.
                lines.Add(FixedCsvFormat.Split(textItems[i]));
            }

            // Return the list of fields.
            return lines;
        }

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
        /// Write the data from the transform model.
        /// </summary>
        /// <param name="data">The data to be written.</param>
        /// <returns>True is all data has been written else false.</returns>
        /// <exception cref="System.ArgumentNullException">Array is null</exception>
        /// <exception cref="System.ArgumentNullException">Parameter is null</exception>
        /// <exception cref="System.IndexOutOfRangeException">Array contains no data.</exception>
        /// <exception cref="Nequeo.Exceptions.EmptyStringException">Empty string exception validation.</exception>
        public virtual bool Write(Object data)
        {
            // Make sure that all params are valid.
            if (data == null)
                throw new ArgumentNullException("data");

            bool ret = false;
            bool baseResult = true;

            string path = string.Empty;
            string fileNameWithoutExt = string.Empty;

            try
            {
                ValidateSetting(Conversion.Properties.Settings.Default.OutputCsvPath, "OutputCsvPath");

                // Assign configuration values.
                ColumnDelimiter = Conversion.Properties.Settings.Default.ColumnDelimiter;
                RowDelimiter = Conversion.Properties.Settings.Default.RowDelimiter.Replace("\\r", "\r").Replace("\\n", "\n");
                IncludeHeaders = Conversion.Properties.Settings.Default.IncludeHeaders;
                IncludeCategoryID = Conversion.Properties.Settings.Default.IncludeCategoryID;
                OneRowPerCsvFile = Conversion.Properties.Settings.Default.OneRowPerCsvFile;

                path = System.IO.Path.GetDirectoryName(Conversion.Properties.Settings.Default.OutputCsvPath);
                fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(Conversion.Properties.Settings.Default.OutputCsvPath);
                _fileExt = System.IO.Path.GetExtension(Conversion.Properties.Settings.Default.OutputCsvPath);

            }
            catch (EmptyStringException esex)
            {
                // Add the error message.
                _currentWriteErrors.Add(
                    "Type : FixedCsvFormat |\r\n" +
                    "Member : Write |\r\n" +
                    "Message : " + esex.Message + "|\r\n" +
                    "Stack : " + esex.StackTrace);
                return false;
            }

            // Create the current file name.
            _currentCsvWriteFileNameWithoutExt =
                path + "\\" + fileNameWithoutExt + "_" +
                _currentDate.ToString("dd") + _currentDate.ToString("MM") + _currentDate.ToString("yyyy") + "_" +
                _currentDate.ToString("HH") + _currentDate.ToString("mm") + _currentDate.ToString("ss");

            // Determine the type of the object that is passed.
            if (data is List<TransformModel[]>)
            {
                // If one file per record is to be written.
                if (_oneFilePerRow)
                {
                    FileStream fileStream = null;
                    string currentInternalCSvFileName = _currentCsvWriteFileNameWithoutExt;

                    // Cast the data collection.
                    List<TransformModel[]> dataCol = (List<TransformModel[]>)data;

                    // For each data collection in the list
                    // write the data to the underlying stream.
                    foreach (TransformModel[] items in dataCol)
                    {

                        // Increment the count.
                        _indexFileCount++;

                        try
                        {
                            // Get the current suffix if any
                            string sufixFileName = (String.IsNullOrEmpty(items[0].SufixFileName) ? string.Empty : "_" + items[0].SufixFileName);
                            currentInternalCSvFileName = _currentCsvWriteFileNameWithoutExt + "_" + _indexFileCount.ToString();

                            if (!String.IsNullOrEmpty(items[0].SpecificFileName))
                                currentInternalCSvFileName = path + "\\" + items[0].SpecificFileName;

                            // Get the current index filename.
                            _currentIndexFileName = currentInternalCSvFileName + sufixFileName + _fileExt;

                            // If the directory does not exist then create it.
                            if (!Directory.Exists(System.IO.Path.GetDirectoryName(_currentIndexFileName)))
                                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_currentIndexFileName));

                            // Create a new file stream from the file path.
                            fileStream = new FileStream(_currentIndexFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, BUFFER_SIZE, true);
                            ret = WriteToStream(items, fileStream);
                            fileStream.Close();
                            if (!ret) baseResult = false;

                            // Create the control file name.
                            if (ret) ret = CreateControlFiles(currentInternalCSvFileName + sufixFileName + ".ctl");
                            if (!ret) baseResult = false;

                            // Create the final location model
                            if (ret)
                                _pathDetailModels.Add(
                                    new PathDetailModel()
                                    {
                                        FolderName = _currentDateTimeFolderName,
                                        FileName = currentInternalCSvFileName + sufixFileName
                                    });
                        }
                        catch (Exception ex)
                        {
                            ret = false;

                            // Add the error message.
                            _currentWriteErrors.Add(
                                "Type : FixedCsvFormat |\r\n" +
                                "Member : Write |\r\n" +
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
                    // Cast the data collection.
                    List<TransformModel[]> dataCol = (List<TransformModel[]>)data;
                    if (dataCol.Count < 1)
                        return false;

                    string currentInternalCSvFileName = _currentCsvWriteFileNameWithoutExt;

                    // Attempt to get the first item array.
                    TransformModel[] firstItem = dataCol.First();
                    string sufixFileName = string.Empty;
                    if (firstItem.Count() > 0)
                        sufixFileName = (String.IsNullOrEmpty(firstItem.First().SufixFileName) ? string.Empty : "_" + firstItem.First().SufixFileName);

                    currentInternalCSvFileName = _currentCsvWriteFileNameWithoutExt;

                    if (!String.IsNullOrEmpty(firstItem.First().SpecificFileName))
                        currentInternalCSvFileName = path + "\\" + firstItem.First().SpecificFileName;

                    // Curent index file name without extension.
                    _currentIndexFileName = currentInternalCSvFileName + sufixFileName + _fileExt;

                    // Write the data to the file.
                    ret = Write(dataCol, _currentIndexFileName);
                    if (!ret) baseResult = false;

                    // Create the control file name.
                    if (ret) ret = CreateControlFiles(currentInternalCSvFileName + sufixFileName + ".ctl");
                    if (!ret) baseResult = false;

                    // Create the final location model
                    if (ret)
                        _pathDetailModels.Add(
                            new PathDetailModel()
                            {
                                FolderName = _currentDateTimeFolderName,
                                FileName = currentInternalCSvFileName + sufixFileName
                            });

                    ret = baseResult;
                    return ret;
                }
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
        /// <exception cref="System.ArgumentNullException">Array is null</exception>
        /// <exception cref="System.ArgumentNullException">Parameter is null</exception>
        /// <exception cref="System.IndexOutOfRangeException">Array contains no data.</exception>
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
                // Write the data to the underlying stream.
                ret = WriteToStream(data, fileStream);
                fileStream.Flush();
            }

            // All data has been written.
            return ret;
        }

        /// <summary>
        /// Write the data to the memory stream.
        /// </summary>
        /// <param name="data">The data to be written.</param>
        /// <param name="memoryStream">The memory stream where the data is to be written.</param>
        /// <returns>True is all data has been written else false.</returns>
        /// <exception cref="System.ArgumentNullException">Array is null</exception>
        /// <exception cref="System.ArgumentNullException">Stream is null</exception>
        /// <exception cref="System.IndexOutOfRangeException">Array contains no data.</exception>
        public virtual bool Write(List<TransformModel[]> data, MemoryStream memoryStream)
        {
            // Make sure that all params are valid.
            if (data == null)
                throw new ArgumentNullException("data");
            if (memoryStream == null)
                throw new ArgumentNullException("memoryStream");
            if (data.Count < 1)
                return false;

            bool ret = false;

            // If one file per record is to be written.
            if (_oneFilePerRow)
                throw new NotImplementedException();
            else
            {
                // Write the data to the underlying stream.
                ret = WriteToStream(data, memoryStream);
                memoryStream.Flush();
            }

            // All data has been written.
            return ret;
        }

        /// <summary>
        /// Read all the data from the file.
        /// </summary>
        /// <param name="fileName">The file name and path where the data is to be read from.</param>
        /// <returns>The collection of data.</returns>
        /// <exception cref="System.ArgumentNullException">Parameter is null</exception>
        public virtual List<TransformModel[]> ReadAllLines(string fileName)
        {
            // Make sure that all params are valid.
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            List<TransformModel[]> rows = new List<TransformModel[]>();

            // Read all the lines in the file.
            String[] fileContent = System.IO.File.ReadAllLines(fileName);

            // Get the column names if they exists.
            string[] columnHeaders = null;
            if (_includeHeaders)
                columnHeaders = fileContent[0].Split(_columnDelimiter.ToCharArray(), StringSplitOptions.None);
            else
            {
                // Estimate the column count from the first line of data.
                int columnCount = fileContent[0].Split(_columnDelimiter.ToCharArray(), StringSplitOptions.None).Length;
                columnHeaders = new string[columnCount];

                // Give the columns default names.
                for (int k = 0; k < columnCount; k++)
                    columnHeaders[k] = "Column" + k.ToString();
            }

            // For each row in the file get the data for each column.
            int startIndex = _includeHeaders ? 1 : 0;
            for (int i = startIndex; i < fileContent.Length; i++)
            {
                List<TransformModel> columnData = new List<TransformModel>();

                try
                {
                    // Get the current row of column data.
                    string[] data = fileContent[i].Split(_columnDelimiter.ToCharArray(), StringSplitOptions.None);

                    int startIndexRow = _includeCategoryID ? 1 : 0;
                    for (int j = startIndexRow; j < data.Length; j++)
                    {
                        // Create a new data model column data type.
                        TransformModel item = new TransformModel()
                        {
                            CategoryID = _includeCategoryID ? data[0] : String.Empty,
                            ValueName = columnHeaders[j],
                            Value = data[j],
                            ValueFormat = string.Empty,
                            ValueType = typeof(System.Object)
                        };

                        // Add the current column to the collection
                        // of columns in the current row.
                        columnData.Add(item);
                    }

                    // Add the current row to the collection of rows.
                    rows.Add(columnData.ToArray());
                }
                catch (Exception ex)
                {
                    // If the event has been attached, then
                    // send an error to the client.
                    if (OnRowError != null)
                        OnRowError(this, new ConversionArgs(i, ex.Message));
                }
            }

            // If the event has been attached, then
            // send a complete to the client.
            if (OnProcessComplete != null)
                OnProcessComplete(this, new EventArgs());

            // Return the collection of items.
            _numRecordsRead = (long)rows.Count;
            return rows;
        }

        /// <summary>
        /// Read the data from the file.
        /// </summary>
        /// <returns>The collection of data.</returns>
        /// <exception cref="System.ArgumentNullException">Parameter is null</exception>
        public virtual Object Read()
        {
            // Assign configuration values.
            ColumnDelimiter = Conversion.Properties.Settings.Default.ColumnDelimiter;
            RowDelimiter = Conversion.Properties.Settings.Default.RowDelimiter.Replace("\\r", "\r").Replace("\\n", "\n");
            IncludeHeaders = Conversion.Properties.Settings.Default.IncludeHeaders;
            IncludeCategoryID = Conversion.Properties.Settings.Default.IncludeCategoryID;
            OneRowPerCsvFile = Conversion.Properties.Settings.Default.OneRowPerCsvFile;

            // Get the type of object to return.
            Type typeToRead = Reflection.TypeAccessor.GetType(Conversion.Properties.Settings.Default.ReadType);

            // Determine the type of object to return
            if (typeToRead == typeof(List<TransformModel[]>))
                // Read the data from the file.
                return Read(Conversion.Properties.Settings.Default.InputCsvPath);
            else
                return null;
        }

        /// <summary>
        /// Read the data from the file.
        /// </summary>
        /// <param name="fileName">The file name and path where the data is to be read from.</param>
        /// <returns>The collection of data.</returns>
        /// <exception cref="System.ArgumentNullException">Parameter is null</exception>
        public virtual List<TransformModel[]> Read(string fileName)
        {
            // Make sure that all parm are valid.
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            List<TransformModel[]> ret = null;
            FileStream fileStream = null;
            try
            {
                // Create a new file stream from the file path.
                fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None, BUFFER_SIZE, true);
                ret = Read(fileStream);
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
        /// Read the data from the file stream.
        /// </summary>
        /// <param name="fileStream">The file stream where the data is to be read from.</param>
        /// <returns>The collection of data.</returns>
        /// <exception cref="System.ArgumentNullException">Stream is null</exception>
        public virtual List<TransformModel[]> Read(FileStream fileStream)
        {
            // Make sure that all parm are valid.
            if (fileStream == null)
                throw new ArgumentNullException("fileStream");

            // Write the data to the underlying stream.
            return ReadFromStream(fileStream);
        }

        /// <summary>
        /// Read the data from the memory stream.
        /// </summary>
        /// <param name="memoryStream">The memory stream where the data is to be read from.</param>
        /// <returns>The collection of data.</returns>
        /// <exception cref="System.ArgumentNullException">Stream is null</exception>
        public virtual List<TransformModel[]> Read(MemoryStream memoryStream)
        {
            // Make sure that all parm are valid.
            if (memoryStream == null)
                throw new ArgumentNullException("memoryStream");

            // Write the data to the underlying stream.
            return ReadFromStream(memoryStream);
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
                    "Type : FixedCsvFormat |\r\n" +
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
        /// Gets the list of write errors if any.
        /// </summary>
        /// <returns>The list of errors; else empty collection.</returns>
        public virtual string[] GetWriteErrors()
        {
            return _currentWriteErrors.ToArray();
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
                        "Type : FixedCsvFormat |\r\n" +
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
        /// After a read operation execute this method.
        /// </summary>
        /// <returns>True if the execution was successful else false.</returns>
        public virtual bool OnReadEnd()
        {
            return false;
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
        /// Gets the returned data from the conversion write process
        /// </summary>
        /// <returns>The list of returned data.</returns>
        public virtual List<ExportBatchModel> GetWriteData()
        {
            return _exportBatchList;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Write the data to the underlying stream.
        /// </summary>
        /// <param name="data">The data to be written.</param>
        /// <param name="stream">The stream to write to.</param>
        /// <returns>True is all data has been written else false.</returns>
        private bool WriteToStream(TransformModel[] data, Stream stream)
        {
            bool ret = true;
            StringBuilder builder = new StringBuilder();

            // If the stream contains no data and headers
            // are to be written.
            if (stream.Length == 0 && _includeHeaders)
            {
                // Get the first data model and attempt to read
                // the column names.
                TransformModel[] items = data;

                // For each item in the collection create the column line.
                foreach (TransformModel item in items)
                    builder.Append(item.ValueName + _columnDelimiter);

                // Get the array of column data and write the
                // data to the underlying stream.
                byte[] headerData = Encoding.Default.GetBytes(builder.ToString().TrimEnd(_columnDelimiter.ToCharArray()) + _rowDelimiter);
                stream.Write(headerData, 0, headerData.Length);
            }

            _indexTotalRows++;
            builder = new StringBuilder();

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

                try
                {
                    // Create the row of data.
                    builder.Append(
                        (_includeCategoryID ? item.CategoryID + _columnDelimiter : String.Empty) +
                            DataType.GetFormattedValue(item.ValueType, item.Value, item.ValueFormat,
                                Conversion.Properties.Settings.Default.DateTimeDataFormat).Replace(_columnDelimiter, "") + _columnDelimiter);
                }
                catch (Exception ex)
                {
                    ret = false;

                    // Create the row of data.
                    builder.Append(_columnDelimiter);

                    // Add the error message.
                    _currentWriteErrors.Add(
                        "Type : FixedCsvFormat |\r\n" +
                        "Member : WriteToStream |\r\n" +
                        "Message : " + ex.Message + "|\r\n" +
                        "Stack : " + ex.StackTrace);

                    _failedIrsDocumentExport.Add((long)item.Level2ID);

                    // If the event has been attached, then
                    // send an error to the client.
                    if (OnRowError != null)
                        OnRowError(this, new ConversionArgs(_indexTotalRows, ex.Message));
                }
            }

            // Get the array of row data and write the
            // data to the underlying stream.
            string currentRow = builder.ToString();
            byte[] rowData = Encoding.Default.GetBytes(currentRow.Substring(0, currentRow.Length - 1) + _rowDelimiter);
            stream.Write(rowData, 0, rowData.Length);
            stream.Flush();

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
            StringBuilder builder = new StringBuilder();

            // If the stream contains no data and headers
            // are to be written.
            if (stream.Length == 0 && _includeHeaders)
            {
                // Get the first data model and attempt to read
                // the column names.
                TransformModel[] items = data[0];

                // For each item in the collection create the column line.
                foreach (TransformModel item in items)
                    builder.Append(item.ValueName + _columnDelimiter);

                // Get the array of column data and write the
                // data to the underlying stream.
                byte[] headerData = Encoding.Default.GetBytes(builder.ToString().TrimEnd(_columnDelimiter.ToCharArray()) + _rowDelimiter);
                stream.Write(headerData, 0, headerData.Length);
            }

            // For each data collection in the list
            // write the data to the underlying stream.
            foreach (TransformModel[] items in data)
            {
                index++;
                builder = new StringBuilder();

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

                    try
                    {
                        // Create the row of data.
                        builder.Append(
                            (_includeCategoryID ? item.CategoryID + _columnDelimiter : String.Empty) +
                                DataType.GetFormattedValue(item.ValueType, item.Value, item.ValueFormat,
                                    Conversion.Properties.Settings.Default.DateTimeDataFormat).Replace(_columnDelimiter, "") + _columnDelimiter);
                    }
                    catch (Exception ex)
                    {
                        ret = false;

                        // Create the row of data.
                        builder.Append(_columnDelimiter);

                        // Add the error message.
                        _currentWriteErrors.Add(
                            "Type : FixedCsvFormat |\r\n" +
                            "Member : WriteToStream |\r\n" +
                            "Message : " + ex.Message + "|\r\n" +
                            "Stack : " + ex.StackTrace);

                        _failedIrsDocumentExport.Add((long)item.Level2ID);

                        // If the event has been attached, then
                        // send an error to the client.
                        if (OnRowError != null)
                            OnRowError(this, new ConversionArgs(index, ex.Message));
                    }
                }

                // Get the array of row data and write the
                // data to the underlying stream.
                string currentRow = builder.ToString();
                byte[] rowData = Encoding.Default.GetBytes(currentRow.Substring(0, currentRow.Length - 1) + _rowDelimiter);
                stream.Write(rowData, 0, rowData.Length);
                stream.Flush();
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
        /// Read the data from the underlying stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The collection of data.</returns>
        private List<TransformModel[]> ReadFromStream(Stream stream)
        {
            List<TransformModel[]> rows = new List<TransformModel[]>();

            // Attempt to read all the data from the stream,
            // encode the data to a string.
            byte[] data = new byte[Convert.ToInt32(stream.Length)];
            int bytesRead = stream.Read(data, 0, Convert.ToInt32(stream.Length));
            string items = Encoding.Default.GetString(data);

            // Collect all the rows into a collection.
            string[] rowData = items.Split(_rowDelimiter.ToCharArray(), StringSplitOptions.None);

            // Get the column names if they exists.
            string[] columnHeaders = null;
            if (_includeHeaders)
                columnHeaders = rowData[0].Split(_columnDelimiter.ToCharArray(), StringSplitOptions.None);
            else
            {
                // Estimate the column count from the first line of data.
                int columnCount = rowData[0].Split(_columnDelimiter.ToCharArray(), StringSplitOptions.None).Length;
                columnHeaders = new string[columnCount];

                // Give the columns default names.
                for (int k = 0; k < columnCount; k++)
                    columnHeaders[k] = "Column" + k.ToString();
            }

            // For each row found create the data model.
            int startIndex = _includeHeaders ? 1 : 0;
            for (int i = startIndex; i < rowData.Length; i++)
            {
                // If the data row contains data then continue.
                if (!String.IsNullOrEmpty(rowData[i]))
                {
                    List<TransformModel> columnData = new List<TransformModel>();

                    // Return the collection of columns for
                    // the curent row.
                    string[] columns = rowData[i].Split(_columnDelimiter.ToCharArray(), StringSplitOptions.None);

                    int startIndexRow = _includeCategoryID ? 1 : 0;
                    for (int j = startIndexRow; j < columns.Length; j++)
                    {
                        // Create a new data model column data type.
                        TransformModel item = new TransformModel()
                        {
                            CategoryID = _includeCategoryID ? columns[0] : String.Empty,
                            ValueName = columnHeaders[j],
                            Value = columns[j],
                            ValueFormat = string.Empty,
                            ValueType = typeof(System.Object)
                        };

                        // Add the current column to the collection
                        // of columns in the current row.
                        columnData.Add(item);
                    }

                    // Add the curent row to the collection of rows.
                    rows.Add(columnData.ToArray());
                }
            }

            _numRecordsRead = (long)rows.Count;
            return rows;
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
                    "Type : FixedCsvFormat |\r\n" +
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
        /// <exception cref="Nequeo.Exceptions.EmptyStringException">Empty string exception validation.</exception>
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
                    // Copy the csv file to the archive location.
                    File.Copy(fileNameWithoutExt + _fileExt,
                        Conversion.Properties.Settings.Default.ArchivePath.TrimEnd('\\') + "\\" + currentDateTimeFolderName + "\\" +
                        System.IO.Path.GetFileNameWithoutExtension(fileNameWithoutExt + _fileExt) + _fileExt, true);
                }
                catch (Exception ex)
                {
                    // Add the error message.
                    _currentWriteErrors.Add(
                        "Type : FixedCsvFormat |\r\n" +
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
                        "Type : FixedCsvFormat |\r\n" +
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
                        "Type : FixedCsvFormat |\r\n" +
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
                        "Type : FixedCsvFormat |\r\n" +
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
                    "Type : FixedCsvFormat |\r\n" +
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

                case "system.double":
                case "double":
                    if (String.IsNullOrEmpty(value.ToString()))
                        throw new EmptyStringException(memberName + " " + Nequeo.Conversion.Properties.Resources.NoValue);
                    else
                    {
                        double number;
                        bool ret = double.TryParse(value.ToString(), out number);
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
