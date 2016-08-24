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
using System.IO;
using System.Xml.Linq;
using System.Reflection;
using System.Security.Permissions;

using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;

namespace Nequeo.Document.OpenXml
{
    /// <summary>
    /// Open Xml excel processing base class provider.
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public abstract class ExcelSpreadsheetBase
    {
        /// <summary>
        /// The word processing namespace.
        /// </summary>
        protected const string nameSpace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

        /// <summary>
        /// The footer and header relationships
        /// </summary>
        protected const string nameSpaceRelationships = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

        private string _templateLocation = string.Empty;
        private string _createLocation = string.Empty;
        private SpreadsheetDocument _document = null;
        private WorkbookPart _workBookPart = null;
        private List<WorksheetPart> _worksheetParts = null;
        private Sheets _sheets = null;
        private XmlDocument _xDocument = null;
        private XmlNamespaceManager _xmlNamespaceManager = null;
        private XmlNamespaceManager _xmlNamespaceManagerRelationships = null;

        /// <summary>
        /// Gets the template location.
        /// </summary>
        protected string TemplateLocation
        {
            get { return _templateLocation.ToString(); }
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
        /// Gets the excel processing document.
        /// </summary>
        protected SpreadsheetDocument Document
        {
            get { return _document; }
            set { _document = value; }
        }

        /// <summary>
        /// Gets the main workbook part.
        /// </summary>
        protected WorkbookPart WorkbookPart
        {
            get { return _workBookPart; }
            set { _workBookPart = value; }
        }

        /// <summary>
        /// Gets the workbook sheets.
        /// </summary>
        protected Sheets Sheets
        {
            get { return _sheets; }
            set { _sheets = value; }
        }

        /// <summary>
        /// Gets the main worksheet part.
        /// </summary>
        protected List<WorksheetPart> WorksheetParts
        {
            get { return _worksheetParts; }
            set { _worksheetParts = value; }
        }

        /// <summary>
        /// Gets the xml document reference.
        /// </summary>
        protected XmlDocument XmlDocument
        {
            get { return _xDocument; }
            set { _xDocument = value; }
        }

        /// <summary>
        /// Gets the xml namespace reference.
        /// </summary>
        protected XmlNamespaceManager XmlNamespaceManager
        {
            get { return _xmlNamespaceManager; }
            set { _xmlNamespaceManager = value; }
        }

        /// <summary>
        /// Open a document with the template.
        /// </summary>
        /// <param name="templateLocation">The document template location.</param>
        /// <param name="createLocation">The document creation location.</param>
        protected void OpenDocument(string templateLocation, string createLocation)
        {
            _templateLocation = templateLocation;
            _createLocation = createLocation;

            // Open the document.
            System.IO.File.Copy(templateLocation, createLocation, true);
            _document = SpreadsheetDocument.Open(createLocation, true);
            _workBookPart = _document.WorkbookPart;
            _worksheetParts = new List<WorksheetPart>(_workBookPart.WorksheetParts);
            _sheets = _workBookPart.Workbook.Sheets;

            NameTable nt = new NameTable();
            _xmlNamespaceManager = new XmlNamespaceManager(nt);
            _xmlNamespaceManager.AddNamespace("x", nameSpace);

            NameTable ntr = new NameTable();
            _xmlNamespaceManagerRelationships = new XmlNamespaceManager(ntr);
            _xmlNamespaceManagerRelationships.AddNamespace("r", nameSpaceRelationships);

            // Get the document part from the package.
            _xDocument = new XmlDocument();
            // Load the XML in the part into an XmlDocument instance.
            _xDocument.Load(_workBookPart.GetStream());
        }

        /// <summary>
        /// Create a new documnent.
        /// </summary>
        /// <param name="createLocation">The location and path to create the document.</param>
        protected void CreateDocument(string createLocation)
        {
            _templateLocation = createLocation;
            _createLocation = createLocation;

            // Create the new doucument
            _document = SpreadsheetDocument.Create(createLocation, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            _workBookPart = _document.AddWorkbookPart();
            _workBookPart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            _worksheetParts = new List<WorksheetPart>();
            _worksheetParts.Add(_workBookPart.AddNewPart<WorksheetPart>());
            _worksheetParts[0].Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            _sheets = _workBookPart.Workbook.AppendChild<Sheets>(new Sheets());

            NameTable nt = new NameTable();
            _xmlNamespaceManager = new XmlNamespaceManager(nt);
            _xmlNamespaceManager.AddNamespace("w", nameSpace);

            NameTable ntr = new NameTable();
            _xmlNamespaceManagerRelationships = new XmlNamespaceManager(ntr);
            _xmlNamespaceManagerRelationships.AddNamespace("r", nameSpaceRelationships);

            // Get the document part from the package.
            _xDocument = new XmlDocument();
            // Load the XML in the part into an XmlDocument instance.
            _xDocument.Load(_workBookPart.GetStream());
        }

        /// <summary>
        /// Add a new work sheet to the workbook of the first work sheet part (index 0: WorksheetParts[0]).
        /// </summary>
        /// <param name="sheetId">Sheet Tab Id.</param>
        /// <param name="name">Sheet Name.</param>
        protected void AddSheet(UInt32 sheetId, string name)
        {
            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() 
            { 
                Id = _workBookPart.GetIdOfPart(_worksheetParts[0]),
                SheetId = sheetId,
                Name = name 
            };

            // Apend the new sheet.
            _sheets.Append(sheet);
        }

        /// <summary>
        /// Get the xml document for the open xml part.
        /// </summary>
        /// <param name="part">The openxml part to get.</param>
        /// <returns>The xml document</returns>
        protected XDocument GetXDocument(OpenXmlPart part)
        {
            XDocument xdoc = part.Annotation<XDocument>();
            if (xdoc != null)
                return xdoc;

            using (StreamReader streamReader = new StreamReader(part.GetStream()))
            using (XmlReader xmlReader = XmlReader.Create(streamReader))
                xdoc = XDocument.Load(xmlReader);

            part.AddAnnotation(xdoc);
            return xdoc;
        }

        /// <summary>
        /// Save the xml document to the openxml part.
        /// </summary>
        /// <param name="part">The openxml part to save</param>
        protected void SaveXDocument(OpenXmlPart part)
        {
            XDocument xdoc = GetXDocument(part);
            if (xdoc != null)
            {
                // Serialize the XDocument object back to the package.
                using (XmlWriter xmlWriter = XmlWriter.Create(part.GetStream(FileMode.Create, FileAccess.ReadWrite)))
                    xdoc.Save(xmlWriter);
            }
        }

        /// <summary>
        /// Close the current document
        /// </summary>
        protected void CloseDocument()
        {
            if (_document != null)
                _document.Close();

            _document = null;
        }

        /// <summary>
        /// Deletes the create location file, created when opening the document.
        /// </summary>
        protected void DeleteCreateLocationFile()
        {
            if (System.IO.File.Exists(_createLocation))
                System.IO.File.Delete(_createLocation);
        }

        /// <summary>
        /// Saves the current document.
        /// </summary>
        /// <param name="location">The location where the document is to be saved.</param>
        protected void SaveDocument(string location)
        {
            // Save the document to it-self
            _xDocument.Save(_workBookPart.GetStream(System.IO.FileMode.Create, FileAccess.ReadWrite));
        }

        /// <summary>
        /// Save the current workbook and all worksheets within the workbook.
        /// </summary>
        protected void SaveWorkbook()
        {
            // Save the document to it-self
            _workBookPart.Workbook.Save();
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
    }
}
