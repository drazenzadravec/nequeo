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
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;

namespace Nequeo.Document.Office
{
    /// <summary>
    /// Microsoft Excel base class provider.
    /// </summary>
    public abstract class MicrosoftExcelBase : Nequeo.Runtime.DisposableBase, IDisposable
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="isVisible">Should the worksheet be shown when creating.</param>
        public MicrosoftExcelBase(bool isVisible)
        {
            _isVisible = isVisible;
        }

        private bool _isVisible = false;
        private object _templateLocation = string.Empty;
        private string _createLocation = string.Empty;

        /// <summary>
        /// The reference default missing value.
        /// </summary>
        protected object _refMissing = System.Reflection.Missing.Value;

        private Excel._Application _application = null;
        private Excel._Workbook _workbook = null;
        private Excel.Workbooks _workbooks = null;
        private Excel.Sheets _sheets = null;
        private Excel._Worksheet _worksheet = null;
        private Excel.Range _workBookCellRange = null;

        /// <summary>
        /// Gets the excel range.
        /// </summary>
        protected Excel.Range WorkBookRange
        {
            get { return _workBookCellRange; }
            set { _workBookCellRange = value; }
        }

        /// <summary>
        /// Gets the excel application.
        /// </summary>
        protected Excel._Application Application
        {
            get { return _application; }
            set { _application = value; }
        }

        /// <summary>
        /// Gets the workbook
        /// </summary>
        protected Excel._Workbook Workbook
        {
            get { return _workbook; }
            set { _workbook = value; }
        }

        /// <summary>
        /// Gets the workbooks
        /// </summary>
        protected Excel.Workbooks Workbooks
        {
            get { return _workbooks; }
            set { _workbooks = value; }
        }

        /// <summary>
        /// Gets the sheets
        /// </summary>
        protected Excel.Sheets Sheets
        {
            get { return _sheets; }
            set { _sheets = value; }
        }

        /// <summary>
        /// Gets the worksheet
        /// </summary>
        protected Excel._Worksheet Worksheet
        {
            get { return _worksheet; }
            set { _worksheet = value; }
        }

        /// <summary>
        /// Gets the template location.
        /// </summary>
        protected string TemplateLocation
        {
            get { return _templateLocation.ToString(); }
        }

        /// <summary>
        /// Gets the missing value place holder.
        /// </summary>
        protected object RefMissingValue
        {
            get { return _refMissing; }
        }

        /// <summary>
        /// Gets sets the loaction where the document will be saved.
        /// </summary>
        protected string CreateLocation
        {
            get { return _createLocation; }
            set { _createLocation = value; }
        }

        /// <summary>
        /// Print the current workbook to the printer else to the default printer.
        /// </summary>
        /// <param name="printerToUse">the printer name to print the document to.</param>
        protected void PrintWorkbook(string printerToUse = "")
        {
            _workbook.PrintOut(
                _refMissing,
                _refMissing,
                _refMissing,
                _refMissing,
                _refMissing,
                _refMissing,
                _refMissing,
                _refMissing);
        }

        /// <summary>
        /// Print the current document to the printer else to the default printer.
        /// </summary>
        /// <param name="printFileName">The file name to print to.</param>
        /// <param name="printerToUse">The printer name to print to.</param>
        /// <param name="printToFile">Should document be printed to a file.</param>
        protected void PrintWorkbook(
            string printFileName,
            string printerToUse = "",
            bool printToFile = true)
        {
            if (!string.IsNullOrEmpty(printerToUse))
                _application.ActivePrinter = printerToUse;

            object refPrintToFile = printToFile;
            object refPrintFileName = printFileName;

            _workbook.PrintOut(
                _refMissing,
                _refMissing,
                _refMissing,
                _refMissing,
                _refMissing,
                refPrintToFile,
                _refMissing,
                refPrintFileName);
        }

        /// <summary>
        /// Close the current workbook
        /// </summary>
        /// <param name="saveChanges">Should the changes be saved to the document.</param>
        protected void CloseWorkbook(bool saveChanges = false)
        {
            if (_workbook != null)
            {
                object refSaveChanges = saveChanges;
                // Close the document
                // Close the workbook
                _workbook.Close(
                    refSaveChanges,
                    _refMissing,
                    _refMissing);
            }

            if (_worksheet != null)
                _worksheet = null;

            if (_sheets != null)
                _sheets = null;

            if (_workbooks != null)
                _workbooks.Close();

            _workbook = null;
            _workbooks = null;
        }

        /// <summary>
        /// Quit the current application
        /// </summary>
        protected void QuitApplication()
        {
            if (_application != null)
                _application.Quit();

            _application = null;
        }

        /// <summary>
        /// Saves the current workbook.
        /// </summary>
        /// <param name="location">The location where the workbook is to be saved.</param>
        /// <param name="format">The file format to save to.</param>
        protected void SaveWorkbook(string location, ExcelFileFormat format = ExcelFileFormat.Original)
        {
            // Get the save file format for the current document.
            object paramExportFormat = GetExcelFormat(format);

            if (paramExportFormat is Excel.XlFixedFormatType)
            {
                // Export the workbook
                _workbook.ExportAsFixedFormat(
                    (Excel.XlFixedFormatType)paramExportFormat,
                    location,
                    _refMissing,
                    _refMissing,
                    _refMissing,
                    _refMissing,
                    _refMissing,
                    _refMissing,
                    _refMissing);
            }
            else
            {
                // Save the workbook
                _workbook.SaveAs(
                    location,
                    paramExportFormat,
                    _refMissing,
                    _refMissing,
                    _refMissing,
                    _refMissing,
                    Excel.XlSaveAsAccessMode.xlNoChange,
                    _refMissing,
                    _refMissing,
                    _refMissing,
                    _refMissing,
                    _refMissing);
            }
        }

        /// <summary>
        /// Create a new workbook with or without the template.
        /// </summary>
        /// <param name="templateLocation">The workbook template location.</param>
        protected void CreateWorkbook(string templateLocation = "")
        {
            _templateLocation = templateLocation;

            // Create the new excel instance.
            _application = new Excel.Application();
            _application.Visible = _isVisible;
            _workbooks = _application.Workbooks;

            // Create a new workbook with or without the template
            if (String.IsNullOrEmpty(templateLocation))
                _workbook = _workbooks.Add(_refMissing);
            else
                _workbook = _workbooks.Add(_templateLocation);

            // Create the worksheets and worksheet
            _sheets = _workbook.Worksheets;
            _worksheet = (Excel._Worksheet)_sheets.get_Item(1);
        }

        /// <summary>
        /// Set the text value for the specified worksheet cell.
        /// </summary>
        /// <param name="row">The cell row.</param>
        /// <param name="column">The cell column</param>
        /// <param name="text">The text to assign.</param>
        protected void SetWorksheetCell(int row, int column, string text)
        {
            _workBookCellRange.set_Item(row, column, text);
        }

        /// <summary>
        /// Set the range that this interface can work with, within this worksheet.
        /// </summary>
        /// <param name="cellFrom">The cell from range.</param>
        /// <param name="cellTo">The cell to range</param>
        protected void SetWorksheetRange(string cellFrom, string cellTo)
        {
            _workBookCellRange = _worksheet.get_Range(cellFrom, cellTo);
        }

        /// <summary>
        /// Get the text value for the specified worksheet cell.
        /// </summary>
        /// <param name="row">The cell row.</param>
        /// <param name="column">The cell column</param>
        /// <returns>The value in the cell.</returns>
        protected object GetWorksheetCell(int row, int column)
        {
            dynamic retValue = _workBookCellRange.get_Item(row, column);
            return retValue;
        }

        /// <summary>
        /// Read the collection of Excel row column attributes properties 
        /// from the instance that contains the Excel row column attributes.
        /// </summary>
        /// <typeparam name="T">The type that contains the Excel row column attributes.</typeparam>
        /// <param name="instance">The instance that contains the Excel row column attributes.</param>
        /// <returns>The collection of attribute values.</returns>
        protected ExcelPropertyInfoModel[] ReadExcelRowColumnAttributes<T>(T instance) where T : class
        {
            List<ExcelPropertyInfoModel> info = new List<ExcelPropertyInfoModel>();

            // Get all the properties within the contract model.
            PropertyInfo[] properties = Nequeo.Reflection.TypeAccessor.GetProperties<T>();

            // For each property found assign
            // the data to the contract.
            foreach (PropertyInfo property in properties)
                try
                {
                    // For each attribute on each property
                    // in the type.
                    foreach (object attribute in property.GetCustomAttributes(true))
                    {
                        // If the attribute is the
                        // excel row column attribute.
                        if (attribute is ExcelRowColumnAttribute)
                        {
                            // Cast the current attribute.
                            ExcelRowColumnAttribute att = (ExcelRowColumnAttribute)attribute;
                            info.Add(
                                new ExcelPropertyInfoModel()
                                {
                                    PropertyType = property.PropertyType,
                                    PropertyName = property.Name,
                                    PropertyValue = property.GetValue(instance, null),
                                    Name = att.Name,
                                    Column = att.Column,
                                    Row = att.Row
                                });
                        }
                    }
                }
                catch { }

            // Return the collection of ExcelRowColumnAttributes.
            return info.ToArray();
        }

        /// <summary>
        /// Create a new template contract, no data is inserted.
        /// </summary>
        /// <param name="location">The location where the workbook is to be saved.</param>
        /// <param name="templateLocation">The workbook template location.</param>
        /// <param name="format">The file format to save to.</param>
        protected void CreateTemplate(string location, string templateLocation = "", ExcelFileFormat format = ExcelFileFormat.Original)
        {
            try
            {
                // Create a new contract
                CreateWorkbook(templateLocation);

                // Save the contract
                SaveWorkbook(location, format);
                CloseWorkbook();
                QuitApplication();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Workbook != null)
                    Workbook = null;

                if (Application != null)
                    Application = null;

                try
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                catch { }
            }
        }

        /// <summary>
        /// Completely exit the application and release all resources.
        /// </summary>
        /// <param name="saveChanges">Should the changes be saved to the document.</param>
        protected void ExitAndReleaseApplication(bool saveChanges = false)
        {
            if (Workbook != null)
            {
                CloseWorkbook(saveChanges);
                Workbook = null;
            }

            if (Application != null)
            {
                QuitApplication();
                Application = null;
            }

            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch { }
        }

        /// <summary>
        /// Get the excel file format.
        /// </summary>
        /// <param name="format">The file format to save to.</param>
        /// <returns>The excel file format</returns>
        private object GetExcelFormat(ExcelFileFormat format = ExcelFileFormat.Original)
        {
            switch (format)
            {
                case ExcelFileFormat.HTML:
                    return Excel.XlFileFormat.xlHtml;

                case ExcelFileFormat.XML:
                    return Excel.XlFileFormat.xlXMLSpreadsheet;

                case ExcelFileFormat.CSV:
                    return Excel.XlFileFormat.xlCSV;

                case ExcelFileFormat.PDF:
                    return Excel.XlFixedFormatType.xlTypePDF;

                default:
                    return Excel.XlFileFormat.xlWorkbookNormal;
            }
        }
    }
}
