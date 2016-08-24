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

using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;

namespace Nequeo.Document.OpenXml
{
    /// <summary>
    /// Open Xml word processing base class provider.
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public abstract class WordProcessingBase
    {
        /// <summary>
        /// The word processing namespace.
        /// </summary>
        protected const string nameSpace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        /// <summary>
        /// The footer and header relationships
        /// </summary>
        protected const string nameSpaceRelationships = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

        private string _templateLocation = string.Empty;
        private string _createLocation = string.Empty;
        private WordprocessingDocument _document = null;
        private MainDocumentPart _documentPart = null;
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
        /// Gets the word processing document.
        /// </summary>
        protected WordprocessingDocument Document
        {
            get { return _document; }
            set { _document = value; }
        }

        /// <summary>
        /// Gets the main document part.
        /// </summary>
        protected MainDocumentPart DocumentPart
        {
            get { return _documentPart; }
            set { _documentPart = value; }
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

            System.IO.File.Copy(templateLocation, createLocation, true);
            _document = WordprocessingDocument.Open(createLocation, true);
            _documentPart = _document.MainDocumentPart;

            NameTable nt = new NameTable();
            _xmlNamespaceManager = new XmlNamespaceManager(nt);
            _xmlNamespaceManager.AddNamespace("w", nameSpace);

            NameTable ntr = new NameTable();
            _xmlNamespaceManagerRelationships = new XmlNamespaceManager(ntr);
            _xmlNamespaceManagerRelationships.AddNamespace("r", nameSpaceRelationships);

            // Get the document part from the package.
            _xDocument = new XmlDocument();
            // Load the XML in the part into an XmlDocument instance.
            _xDocument.Load(_documentPart.GetStream());

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
            _document = WordprocessingDocument.Create(createLocation, WordprocessingDocumentType.Document);

            // Add a main document part. 
            _documentPart = _document.AddMainDocumentPart();
            // Create the document structure and add some text.
            _documentPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();

            NameTable nt = new NameTable();
            _xmlNamespaceManager = new XmlNamespaceManager(nt);
            _xmlNamespaceManager.AddNamespace("w", nameSpace);

            NameTable ntr = new NameTable();
            _xmlNamespaceManagerRelationships = new XmlNamespaceManager(ntr);
            _xmlNamespaceManagerRelationships.AddNamespace("r", nameSpaceRelationships);

            // Get the document part from the package.
            _xDocument = new XmlDocument();
            // Load the XML in the part into an XmlDocument instance.
            _xDocument.Load(_documentPart.GetStream());
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
            _xDocument.Save(_documentPart.GetStream(System.IO.FileMode.Create, FileAccess.ReadWrite));
        }

        /// <summary>
        /// Set the text value for the specified bookmark.
        /// </summary>
        /// <param name="bookmarkNode">The the bookmark node to create the text for.</param>
        /// <param name="text">The text value to assign.</param>
        protected void SetBookmarkText(OpenXmlElement bookmarkNode, string text)
        {
            // Add the run element.
            Run runEle = new Run(
                new Text(text));

            // Insert the xml nodes under the bookmark
            bookmarkNode.Parent.InsertAfter(runEle, bookmarkNode);
        }

        /// <summary>
        /// Set the text value and the font for the specified bookmark.
        /// </summary>
        /// <param name="bookmarkNode">The the bookmark node to create the text for.</param>
        /// <param name="text">The text value to assign.</param>
        /// <param name="font">The font node for the bookmark text.</param>
        protected void SetBookmarkText(OpenXmlElement bookmarkNode, string text, OpenXmlElement font)
        {
            // Add the run element.
            Run runEle = new Run(font, new Text(text));

            // Insert the xml nodes under the bookmark
            bookmarkNode.Parent.InsertAfter(runEle, bookmarkNode);
        }

        /// <summary>
        /// Creates the font for the specified book mark.
        /// </summary>
        /// <param name="fontName">The name of the font: e.g. "Microsoft Sans Serif"</param>
        /// <param name="fontSize">The size of the font: e.g. "16"</param>
        /// <param name="languageCountryCode">The langauage country code: e.g. "en-AU"</param>
        /// <returns>The font node for the book mark.</returns>
        protected OpenXmlElement GetBookmarkFontNode(string fontName, int fontSize, string languageCountryCode)
        {
            return new RunProperties(
                    new RunFonts() { Ascii = fontName, HighAnsi = fontName, ComplexScript = fontName },
                    new NoProof() { Val = DocumentFormat.OpenXml.OnOffValue.FromBoolean(false) },
                    new FontSize() { Val = fontSize.ToString() },
                    new FontSizeComplexScript() { Val = fontSize.ToString() },
                    new Languages() { Val = languageCountryCode });
        }

        /// <summary>
        /// Does the parent paragraph contain the end of the bookmark.
        /// </summary>
        /// <param name="parent">The parent paragraph to search within</param>
        /// <param name="bookmarkID">The start bookmark id.</param>
        /// <returns>True if the end of the bookmark has been found.</returns>
        protected bool IsEndBookmark(Paragraph parent, string bookmarkID)
        {
            try
            {
                BookmarkEnd bookmarkEnd = parent.Descendants<BookmarkEnd>().
                    First(u => u.Id.ToString() == bookmarkID);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Set the text to vanish
        /// </summary>
        /// <param name="elements">The collection of elements to vanish</param>
        protected void SetTextToVanish(IEnumerable<RunProperties> elements)
        {
            foreach (RunProperties item in elements)
                SetTextToVanish(item);
        }

        /// <summary>
        /// Set the text to vanish
        /// </summary>
        /// <param name="element">The element to vanish.</param>
        protected void SetTextToVanish(RunProperties element)
        {
            element.AppendChild<Vanish>(new Vanish());
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
    }
}
