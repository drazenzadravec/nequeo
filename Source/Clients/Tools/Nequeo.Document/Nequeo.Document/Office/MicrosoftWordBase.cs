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
using Word = Microsoft.Office.Interop.Word;

namespace Nequeo.Document.Office
{
    /// <summary>
    /// Microsoft Word base class provider.
    /// </summary>
    public abstract class MicrosoftWordBase : Nequeo.Runtime.DisposableBase, IDisposable
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="isVisible">Should the document be shown when creating.</param>
        public MicrosoftWordBase(bool isVisible)
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

        private Word._Application _application = null;
        private Word._Document _document = null;

        /// <summary>
        /// Gets the word application.
        /// </summary>
        protected Word._Application Application
        {
            get { return _application; }
            set { _application = value; }
        }

        /// <summary>
        /// Gets the word document.
        /// </summary>
        protected Word._Document Document
        {
            get { return _document; }
            set { _document = value; }
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
        /// Print the current document to the default printer.
        /// </summary>
        protected void PrintDocument()
        {
            _document.PrintOut(
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing);
        }

        /// <summary>
        /// Print the current document to the printer else to the default printer.
        /// </summary>
        /// <param name="printFileName">The file name to print to.</param>
        /// <param name="printerToUse">The printer name to print to.</param>
        /// <param name="printToFile">Should document be printed to a file.</param>
        /// <param name="appendToFile">Should the current data be appended to the print file</param>
        protected void PrintDocument(
            string printFileName,
            string printerToUse = "",
            bool printToFile = true,
            bool appendToFile = false)
        {
            if (!string.IsNullOrEmpty(printerToUse))
                _application.ActivePrinter = printerToUse;

            object refPrintToFile = printToFile;
            object refPrintFileName = printFileName;
            object refAppend = appendToFile;

            _document.PrintOut(
                ref refAppend,
                ref _refMissing,
                ref _refMissing,
                ref refPrintFileName,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref refPrintToFile,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing);
        }

        /// <summary>
        /// Close the current document
        /// </summary>
        /// <param name="saveChanges">Should the changes be saved to the document.</param>
        protected void CloseDocument(bool saveChanges = false)
        {
            if (_document != null)
            {
                object refSaveChanges = saveChanges;
                // Close the document
                _document.Close(
                    ref refSaveChanges,
                    ref _refMissing,
                    ref _refMissing);
            }
            _document = null;
        }

        /// <summary>
        /// Quit the current application
        /// </summary>
        /// <param name="saveChanges">Should the changes be saved to the document.</param>
        protected void QuitApplication(bool saveChanges = false)
        {
            if (_application != null)
            {
                object refSaveChanges = saveChanges;
                // Quit the application.
                _application.Quit(
                    ref refSaveChanges,
                    ref _refMissing,
                    ref _refMissing);
            }
            _application = null;
        }

        /// <summary>
        /// Saves the current document.
        /// </summary>
        /// <param name="location">The location where the document is to be saved.</param>
        /// <param name="format">The file format to save to.</param>
        protected void SaveDocument(string location, WordFileFormat format = WordFileFormat.Original)
        {
            // Get the save file format for the current document.
            object paramExportFormat = GetWordFormat(format);

            // Save the document
            _document.SaveAs(
                location,
                paramExportFormat,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing,
                ref _refMissing);
        }

        /// <summary>
        /// Create a new document with or without the template.
        /// </summary>
        /// <param name="templateLocation">The document template location.</param>
        protected void CreateDocument(string templateLocation = "")
        {
            _templateLocation = templateLocation;

            // Create the new word instance.
            _application = new Word.Application();
            _application.Visible = _isVisible;

            // Create a new document with or without the template
            if (String.IsNullOrEmpty(templateLocation))
                _document = _application.Documents.Add(
                    ref _refMissing,
                    ref _refMissing,
                    ref _refMissing,
                    ref _refMissing);
            else
                _document = _application.Documents.Add(
                    ref _templateLocation,
                    ref _refMissing,
                    ref _refMissing,
                    ref _refMissing);
        }

        /// <summary>
        /// Create a new template contract, no data is inserted.
        /// </summary>
        /// <param name="location">The location where the workbook is to be saved.</param>
        /// <param name="templateLocation">The workbook template location.</param>
        /// <param name="format">The file format to save to.</param>
        protected void CreateTemplate(string location, string templateLocation = "", WordFileFormat format = WordFileFormat.Original)
        {
            try
            {
                // Create a new contract
                CreateDocument(templateLocation);

                // Save the contract
                SaveDocument(location, format);
                CloseDocument();
                QuitApplication();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Document != null)
                    Document = null;

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
        /// Set the text value for the specified bookmark.
        /// </summary>
        /// <param name="name">The name of the book mark.</param>
        /// <param name="text">The text value to assign.</param>
        protected void SetBookmarkText(string name, string text)
        {
            object bookMark = name;
            _document.Bookmarks.get_Item(ref bookMark).Range.Text = text;
        }

        /// <summary>
        /// Get the text value for the specified bookmark.
        /// </summary>
        /// <param name="name">The name of the book mark.</param>
        /// <returns>The value in the specified bookmark.</returns>
        protected object GetBookmarkText(string name)
        {
            object bookMark = name;
            object retValue = _document.Bookmarks.get_Item(ref bookMark).Range.Text;
            return retValue;
        }

        /// <summary>
        /// Sets the footer text.
        /// </summary>
        /// <param name="text">The text to add to the footer</param>
        protected void SetFooterText(string text)
        {
            // For each primary footer add the text.
            foreach (Microsoft.Office.Interop.Word.Section section in _document.Sections)
                section.Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Text = text;
        }

        /// <summary>
        /// Sets the header text.
        /// </summary>
        /// <param name="text">The text to add to the header</param>
        protected void SetHeaderText(string text)
        {
            // For each primary footer add the text.
            foreach (Microsoft.Office.Interop.Word.Section section in _document.Sections)
                section.Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Text = text;
        }

        /// <summary>
        /// Set the style for the specified bookmark.
        /// </summary>
        /// <param name="name">The name of the book mark.</param>
        /// <param name="style">The style to set.</param>
        protected void SetBookmarkStyle(string name, string style)
        {
            object bookMark = name;
            object styleName = style;
            _document.Bookmarks.get_Item(ref bookMark).Range.set_Style(ref styleName);
        }

        /// <summary>
        /// Read the collection of Word row column attributes properties 
        /// from the instance that contains the Word row column attributes.
        /// </summary>
        /// <typeparam name="T">The type that contains the Word row column attributes.</typeparam>
        /// <param name="instance">The instance that contains the Word row column attributes.</param>
        /// <returns>The collection of attribute values.</returns>
        protected WordPropertyInfoModel[] ReadWordColumnAttributes<T>(T instance) where T : class
        {
            List<WordPropertyInfoModel> info = new List<WordPropertyInfoModel>();

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
                        if (attribute is WordColumnAttribute)
                        {
                            // Cast the current attribute.
                            WordColumnAttribute att = (WordColumnAttribute)attribute;
                            info.Add(
                                new WordPropertyInfoModel()
                                {
                                    PropertyType = property.PropertyType,
                                    PropertyName = property.Name,
                                    PropertyValue = property.GetValue(instance, null),
                                    Name = att.Name
                                });
                        }
                    }
                }
                catch { }

            // Return the collection of ExcelRowColumnAttributes.
            return info.ToArray();
        }

        /// <summary>
        /// Completely exit the application and release all resources.
        /// </summary>
        /// <param name="saveChanges">Should the changes be saved to the document.</param>
        protected void ExitAndReleaseApplication(bool saveChanges = false)
        {
            if (Document != null)
            {
                CloseDocument(saveChanges);
                Document = null;
            }

            if (Application != null)
            {
                QuitApplication(saveChanges);
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
        /// Get the word file format.
        /// </summary>
        /// <param name="format">The file format to save to.</param>
        /// <returns>The word file format</returns>
        private object GetWordFormat(WordFileFormat format = WordFileFormat.Original)
        {
            switch (format)
            {
                case WordFileFormat.HTML:
                    return Word.WdSaveFormat.wdFormatHTML;

                case WordFileFormat.XML:
                    return Word.WdSaveFormat.wdFormatXML;

                case WordFileFormat.RTF:
                    return Word.WdSaveFormat.wdFormatRTF;

                case WordFileFormat.PDF:
                    return Word.WdSaveFormat.wdFormatPDF;

                default:
                    return Word.WdSaveFormat.wdFormatDocument;
            }
        }
    }
}

