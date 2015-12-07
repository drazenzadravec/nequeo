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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lucene.Net.Facet;
using Lucene.Net.Facet.Taxonomy.Directory;

namespace Nequeo.Search.Engine
{
    /// <summary>
    /// Supported search engine documents.
    /// </summary>
    [Flags]
    public enum SupportedDocuments : int
    {
        /// <summary>
        /// Text document.
        /// </summary>
        Txt = 1,
        /// <summary>
        /// RTF document.
        /// </summary>
        Rtf = 2,
        /// <summary>
        /// PDF document.
        /// </summary>
        Pdf = 4,
        /// <summary>
        /// HTML document.
        /// </summary>
        Html = 8,
        /// <summary>
        /// Xml document.
        /// </summary>
        Xml = 16,
        /// <summary>
        /// MS word document.
        /// </summary>
        Docx = 32,
        /// <summary>
        /// All supported documents.
        /// </summary>
        All = Txt | Rtf | Pdf | Html | Xml | Docx,
    }

    /// <summary>
    /// Facet path.
    /// </summary>
    public class FacetPath
    {
        /// <summary>
        /// Facet path.
        /// </summary>
        /// <param name="dimensionName">The dimension name.</param>
        /// <param name="path">The facet paths for the dimension.</param>
        public FacetPath(string dimensionName, params string[] path)
        {
            DimensionName = dimensionName;
            Path = path;
        }

        /// <summary>
        /// Gets or sets the dimension name.
        /// </summary>
        public string DimensionName { get; set; }

        /// <summary>
        /// Gets or sets the facet paths for the dimension.
        /// </summary>
        public string[] Path { get; set; }
    }

    /// <summary>
    /// Facet data.
    /// </summary>
    public class FacetData
    {
        /// <summary>
        /// Facet data.
        /// </summary>
        public FacetData() 
        {
            _textFacetFields = new Dictionary<FacetField, AddTextData[]>();
            _fileFacetFields = new Dictionary<FacetField, FileFacetModel>();
        }

        private Dictionary<FacetField, AddTextData[]> _textFacetFields = null;
        private Dictionary<FacetField, FileFacetModel> _fileFacetFields = null;

        /// <summary>
        /// Gets the text facet fields.
        /// </summary>
        internal Dictionary<FacetField, AddTextData[]> TextFacetFields
        {
            get { return _textFacetFields; }
        }

        /// <summary>
        /// Gets the file facet fields.
        /// </summary>
        internal Dictionary<FacetField, FileFacetModel> FileFacetFields
        {
            get { return _fileFacetFields; }
        }

        /// <summary>
        /// Gets or sets the array of hierarchical items.
        /// </summary>
        public Hierarchical[] Hierarchicals { get; set; }

        /// <summary>
        /// Gets or sets the array of multi valued items.
        /// </summary>
        public MultiValued[] MultiValues { get; set; }

        /// <summary>
        /// Gets or sets the array of require dimension count items.
        /// </summary>
        public RequireDimensionCount[] RequireDimensionCounts { get; set; }

        /// <summary>
        /// Gets or sets the array of index fields.
        /// </summary>
        public IndexField[] IndexFields { get; set; }

        /// <summary>
        /// Hierarchical item (if this dimension is hierarchical (has depth > 1 paths)).
        /// </summary>
        public class Hierarchical
        {
            /// <summary>
            /// Hierarchical item.
            /// </summary>
            /// <param name="dimensionName">The dimension name.</param>
            /// <param name="isHierarchical">True if this dimension is hierarchical (has depth > 1 paths).</param>
            public Hierarchical(string dimensionName, bool isHierarchical)
            {
                DimensionName = dimensionName;
                IsHierarchical = isHierarchical;
            }

            /// <summary>
            /// Gets or sets the dimension name.
            /// </summary>
            public string DimensionName { get; set; }

            /// <summary>
            /// Gets or sets true if this dimension is hierarchical (has depth > 1 paths).
            /// </summary>
            public bool IsHierarchical { get; set; }
        }

        /// <summary>
        /// Multi valued item (if this dimension may have more than one value per document).
        /// </summary>
        public class MultiValued
        {
            /// <summary>
            /// Multi valued item.
            /// </summary>
            /// <param name="dimensionName">The dimension name.</param>
            /// <param name="isMultiValue">True if this dimension may have more than one value per document.</param>
            public MultiValued(string dimensionName, bool isMultiValue)
            {
                DimensionName = dimensionName;
                IsMultiValue = isMultiValue;
            }

            /// <summary>
            /// Gets or sets the dimension name.
            /// </summary>
            public string DimensionName { get; set; }

            /// <summary>
            /// Gets or sets true if this dimension may have more than one value per document.
            /// </summary>
            public bool IsMultiValue { get; set; }
        }

        /// <summary>
        /// Require dimension count item (if at search time you require accurate counts of the dimension, i.e. how many hits have this dimension).
        /// </summary>
        public class RequireDimensionCount
        {
            /// <summary>
            /// Require dimension count item.
            /// </summary>
            /// <param name="dimensionName">The dimension name.</param>
            /// <param name="isAccurateCountsRequired">True if at search time you require accurate counts of the dimension, i.e. how many hits have this dimension.</param>
            public RequireDimensionCount(string dimensionName, bool isAccurateCountsRequired)
            {
                DimensionName = dimensionName;
                IsAccurateCountsRequired = isAccurateCountsRequired;
            }

            /// <summary>
            /// Gets or sets the dimension name.
            /// </summary>
            public string DimensionName { get; set; }

            /// <summary>
            /// Gets or sets true if at search time you require accurate counts of the dimension, i.e. how many hits have this dimension.
            /// </summary>
            public bool IsAccurateCountsRequired { get; set; }
        }

        /// <summary>
        /// Index field item (specify which index field name should hold the ordinals for this dimension).
        /// </summary>
        public class IndexField
        {
            /// <summary>
            /// Index field item.
            /// </summary>
            /// <param name="dimensionName">The dimension name.</param>
            /// <param name="indexFieldName">The index field name.</param>
            public IndexField(string dimensionName, string indexFieldName)
            {
                DimensionName = dimensionName;
                IndexFieldName = indexFieldName;
            }

            /// <summary>
            /// Gets or sets the dimension name.
            /// </summary>
            public string DimensionName { get; set; }

            /// <summary>
            /// Gets or sets the index field name.
            /// </summary>
            public string IndexFieldName { get; set; }
        }

        /// <summary>
        /// Add a new text facet label.
        /// </summary>
        /// <param name="dimensionName">The dimension name.</param>
        /// <param name="addTextData">The text data to add for the dimension and path.</param>
        /// <param name="path">The facet paths for the dimension.</param>
        public void AddFacet(string dimensionName, AddTextData[] addTextData, params string[] path)
        {
            _textFacetFields.Add(new FacetField(dimensionName, path), addTextData);
        }

        /// <summary>
        /// Add a new text facet label.
        /// </summary>
        /// <param name="dimensionName">The dimension name.</param>
        /// <param name="directoryInfo">The directory information where all the files that are to be added are located.</param>
        /// <param name="documents">The supported documents search filter, used to indicate what files are to be added for the dimension and path.</param>
        /// <param name="path">The facet paths for the dimension.</param>
        public void AddFacet(string dimensionName, DirectoryInfo directoryInfo, SupportedDocumentExtension documents, params string[] path)
        {
            _fileFacetFields.Add(new FacetField(dimensionName, path), new FileFacetModel(directoryInfo, documents));
        }
    }

    /// <summary>
    /// Add text data.
    /// </summary>
    public class AddTextData
    {
        /// <summary>
        /// Add text document.
        /// </summary>
        /// <param name="name">A unique name for the text.</param>
        /// <param name="text">The text to add.</param>
        /// <param name="storeText">Should the text be store as well as being searchable.</param>
        public AddTextData(string name, string text, bool storeText = true)
        {
            Name = name;
            Text = text;
            StoreText = storeText;
        }

        /// <summary>
        /// Gets or set the unique name for the text.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or set the text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or set an indicator sepcifying if the text should be store as well as being searchable.
        /// </summary>
        public bool StoreText { get; set; }
    }

    /// <summary>
    /// Supported document extensions that are to be searched.
    /// </summary>
    public class SupportedDocumentExtension
    {
        /// <summary>
        /// Supported document extensions that are to be searched.
        /// </summary>
        /// <param name="supportedDocuments">The supported documents.</param>
        public SupportedDocumentExtension(SupportedDocuments supportedDocuments)
        {
            _searchPatterns = new Dictionary<Engine.SupportedDocuments, List<string>>();
            _supportedDocuments = supportedDocuments;

            // Add default extensions.
            AddDefaultExtensions();
        }

        internal long TotalDocumentSize = 0;
        private long _maxDocumentSizePerCommit = 30000000L;
        private bool _storeContent = false;
        private bool _tokenizeContent = false;
        private Dictionary<SupportedDocuments, List<string>> _searchPatterns = null;
        private SupportedDocuments _supportedDocuments = SupportedDocuments.Txt;
        private System.IO.SearchOption _searchOption = System.IO.SearchOption.AllDirectories;

        /// <summary>
        /// Gets or sets an indicator specifying if the content should be stored.
        /// </summary>
        internal bool StoreContent
        {
            get { return _storeContent; }
            set { _storeContent = value; }
        }

        /// <summary>
        /// Gets or sets an indicator specifying if the content should be tokenized.
        /// </summary>
        internal bool TokenizeContent
        {
            get { return _tokenizeContent; }
            set { _tokenizeContent = value; }
        }

        /// <summary>
        /// Gets or sets the maximum document size to add to each index (in MB Megabytes).
        /// </summary>
        public long MaxDocumentSizePerCommit
        {
            get { return _maxDocumentSizePerCommit; }
            set { _maxDocumentSizePerCommit = value; }
        }

        /// <summary>
        /// Gets or sets the supported document type.
        /// </summary>
        public SupportedDocuments SupportedDocuments 
        {
            get { return _supportedDocuments; }
            set { _supportedDocuments = value; }
        }

        /// <summary>
        /// Gets or sets whether to search the current directory, or the current directory and all subdirectories.
        /// </summary>
        public System.IO.SearchOption SearchOption
        {
            get { return _searchOption; }
            set { _searchOption = value; }
        }

        /// <summary>
        /// Remove all search patterns.
        /// </summary>
        public void RemoveAllSearchPatterns()
        {
            // If html has been selected.
            if (_supportedDocuments.HasFlag(SupportedDocuments.Html))
            {
                List<string> html = _searchPatterns[SupportedDocuments.Html];
                html.Clear();
            }

            // If pdf has been selected.
            if (_supportedDocuments.HasFlag(SupportedDocuments.Pdf))
            {
                List<string> pdf = _searchPatterns[SupportedDocuments.Pdf];
                pdf.Clear();
            }

            // If rtf has been selected.
            if (_supportedDocuments.HasFlag(SupportedDocuments.Rtf))
            {
                List<string> rtf = _searchPatterns[SupportedDocuments.Rtf];
                rtf.Clear();
            }

            // If txt has been selected.
            if (_supportedDocuments.HasFlag(SupportedDocuments.Txt))
            {
                List<string> txt = _searchPatterns[SupportedDocuments.Txt];
                txt.Clear();
            }

            // If xml has been selected.
            if (_supportedDocuments.HasFlag(SupportedDocuments.Xml))
            {
                List<string> xml = _searchPatterns[SupportedDocuments.Xml];
                xml.Clear();
            }

            // If docx has been selected.
            if (_supportedDocuments.HasFlag(SupportedDocuments.Docx))
            {
                List<string> docx = _searchPatterns[SupportedDocuments.Docx];
                docx.Clear();
            }
        }

        /// <summary>
        /// Remove a collection of search patterns for the document.
        /// </summary>
        /// <param name="supportedDocuments">The supported document format.</param>
        /// <param name="searchPatterns">The list of search patterns.</param>
        public void RemoveSearchPatterns(SupportedDocuments supportedDocuments, string[] searchPatterns = null)
        {
            // If html has been selected.
            if (supportedDocuments.HasFlag(SupportedDocuments.Html))
            {
                List<string> html = _searchPatterns[SupportedDocuments.Html];
                if (searchPatterns != null)
                {
                    foreach (string item in searchPatterns)
                        html.Remove(item);
                }
                else
                    html.Clear();
            }

            // If pdf has been selected.
            if (supportedDocuments.HasFlag(SupportedDocuments.Pdf))
            {
                List<string> pdf = _searchPatterns[SupportedDocuments.Pdf];
                if (searchPatterns != null)
                {
                    foreach (string item in searchPatterns)
                        pdf.Remove(item);
                }
                else
                    pdf.Clear();
            }

            // If rtf has been selected.
            if (supportedDocuments.HasFlag(SupportedDocuments.Rtf))
            {
                List<string> rtf = _searchPatterns[SupportedDocuments.Rtf];
                if (searchPatterns != null)
                {
                    foreach (string item in searchPatterns)
                        rtf.Remove(item);
                }
                else
                    rtf.Clear();
            }

            // If txt has been selected.
            if (supportedDocuments.HasFlag(SupportedDocuments.Txt))
            {
                List<string> txt = _searchPatterns[SupportedDocuments.Txt];
                if (searchPatterns != null)
                {
                    foreach (string item in searchPatterns)
                        txt.Remove(item);
                }
                else
                    txt.Clear();
            }

            // If xml has been selected.
            if (supportedDocuments.HasFlag(SupportedDocuments.Xml))
            {
                List<string> xml = _searchPatterns[SupportedDocuments.Xml];
                if (searchPatterns != null)
                {
                    foreach (string item in searchPatterns)
                        xml.Remove(item);
                }
                else
                    xml.Clear();
            }

            // If docx has been selected.
            if (supportedDocuments.HasFlag(SupportedDocuments.Docx))
            {
                List<string> docx = _searchPatterns[SupportedDocuments.Docx];
                if (searchPatterns != null)
                {
                    foreach (string item in searchPatterns)
                        docx.Remove(item);
                }
                else
                    docx.Clear();
            }
        }

        /// <summary>
        /// Add a collection of search patterns to the document.
        /// </summary>
        /// <param name="supportedDocuments">The supported document format.</param>
        /// <param name="searchPatterns">The list of search patterns.</param>
        public void AddSearchPatterns(SupportedDocuments supportedDocuments, string[] searchPatterns)
        {
            // If html has been selected.
            if (supportedDocuments.HasFlag(SupportedDocuments.Html))
            {
                List<string> html = _searchPatterns[SupportedDocuments.Html];
                foreach (string item in searchPatterns)
                    html.Add(item);
            }

            // If pdf has been selected.
            if (supportedDocuments.HasFlag(SupportedDocuments.Pdf))
            {
                List<string> pdf = _searchPatterns[SupportedDocuments.Pdf];
                foreach (string item in searchPatterns)
                    pdf.Add(item);
            }

            // If rtf has been selected.
            if (supportedDocuments.HasFlag(SupportedDocuments.Rtf))
            {
                List<string> rtf = _searchPatterns[SupportedDocuments.Rtf];
                foreach (string item in searchPatterns)
                    rtf.Add(item);
            }

            // If txt has been selected.
            if (supportedDocuments.HasFlag(SupportedDocuments.Txt))
            {
                List<string> txt = _searchPatterns[SupportedDocuments.Txt];
                foreach (string item in searchPatterns)
                    txt.Add(item);
            }

            // If xml has been selected.
            if (supportedDocuments.HasFlag(SupportedDocuments.Xml))
            {
                List<string> xml = _searchPatterns[SupportedDocuments.Xml];
                foreach (string item in searchPatterns)
                    xml.Add(item);
            }

            // If docx has been selected.
            if (supportedDocuments.HasFlag(SupportedDocuments.Docx))
            {
                List<string> docx = _searchPatterns[SupportedDocuments.Docx];
                foreach (string item in searchPatterns)
                    docx.Add(item);
            }
        }

        /// <summary>
        /// Get the formatted search patterns.
        /// </summary>
        /// <param name="supportedDocuments">The supported document format.</param>
        /// <returns>Semi-colon (;) seperated aearch patters.</returns>
        public string GetFormattedSearchPatterns(SupportedDocuments supportedDocuments)
        {
            StringBuilder builder = new StringBuilder();

            // If html has been selected.
            if (supportedDocuments.HasFlag(SupportedDocuments.Html))
            {
                List<string> html = _searchPatterns[SupportedDocuments.Html];
                foreach (string item in html)
                    builder.Append(item + ";");
            }

            // If pdf has been selected.
            if (supportedDocuments.HasFlag(SupportedDocuments.Pdf))
            {
                List<string> pdf = _searchPatterns[SupportedDocuments.Pdf];
                foreach (string item in pdf)
                    builder.Append(item + ";");
            }

            // If rtf has been selected.
            if (supportedDocuments.HasFlag(SupportedDocuments.Rtf))
            {
                List<string> rtf = _searchPatterns[SupportedDocuments.Rtf];
                foreach (string item in rtf)
                    builder.Append(item + ";");
            }

            // If txt has been selected.
            if (supportedDocuments.HasFlag(SupportedDocuments.Txt))
            {
                List<string> txt = _searchPatterns[SupportedDocuments.Txt];
                foreach (string item in txt)
                    builder.Append(item + ";");
            }

            // If xml has been selected.
            if (supportedDocuments.HasFlag(SupportedDocuments.Xml))
            {
                List<string> xml = _searchPatterns[SupportedDocuments.Xml];
                foreach (string item in xml)
                    builder.Append(item + ";");
            }

            // If docx has been selected.
            if (supportedDocuments.HasFlag(SupportedDocuments.Docx))
            {
                List<string> docx = _searchPatterns[SupportedDocuments.Docx];
                foreach (string item in docx)
                    builder.Append(item + ";");
            }

            // Return the pattern.
            return builder.ToString().TrimEnd(new char[] { ';' });
        }

        /// <summary>
        /// Add default extensions.
        /// </summary>
        private void AddDefaultExtensions()
        {
            List<string> html = new List<string>();
            html.Add("*.htm");
            html.Add("*.asp");
            html.Add("*.php");
            _searchPatterns.Add(Engine.SupportedDocuments.Html, html);

            List<string> pdf = new List<string>();
            pdf.Add("*.pdf");
            _searchPatterns.Add(Engine.SupportedDocuments.Pdf, pdf);

            List<string> rtf = new List<string>();
            rtf.Add("*.rtf");
            _searchPatterns.Add(Engine.SupportedDocuments.Rtf, rtf);

            List<string> txt = new List<string>();
            txt.Add("*.txt");
            txt.Add("*.text");
            _searchPatterns.Add(Engine.SupportedDocuments.Txt, txt);

            List<string> xml = new List<string>();
            xml.Add("*.xml");
            _searchPatterns.Add(Engine.SupportedDocuments.Xml, xml);

            List<string> docx = new List<string>();
            docx.Add("*.docx");
            _searchPatterns.Add(Engine.SupportedDocuments.Docx, docx);
        }
    }
}
