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
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Security.Permissions;

using Lucene.Net;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Search;
using Lucene.Net.Documents;
using Lucene.Net.Facet;
using Lucene.Net.Facet.Taxonomy.Directory;

using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;

using Nequeo.Html;
using Nequeo.Drawing.Pdf;
using Nequeo.Search.Engine.Analyzer;
using Nequeo.Search.Engine.Filter;
using Nequeo.IO;
using Nequeo.Extension;

namespace Nequeo.Search.Engine.Filter
{
    /// <summary>
    /// Microsoft word document filter.
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class MSDocFilter
    {
        /// <summary>
        /// Microsoft word document filter.
        /// </summary>
        public MSDocFilter() { }

        /// <summary>
        /// The word processing namespace.
        /// </summary>
        private const string nameSpace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        /// <summary>
        /// The footer and header relationships
        /// </summary>
        private const string nameSpaceRelationships = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

        private WordprocessingDocument _document = null;
        private XmlDocument _xDocument = null;

        /// <summary>
        /// Gets the word processing document.
        /// </summary>
        private WordprocessingDocument Document
        {
            get { return _document; }
            set { _document = value; }
        }

        /// <summary>
        /// Gets the xml document reference.
        /// </summary>
        private XmlDocument XmlDocument
        {
            get { return _xDocument; }
            set { _xDocument = value; }
        }

        /// <summary>
        /// Add documents.
        /// </summary>
        /// <param name="writer">The index writer.</param>
        /// <param name="directoryInfo">The directory information where all the files that are to be added are located.</param>
        /// <param name="files">The list of files that are to be added.</param>
        /// <param name="documents">The supported documents search filter, used to indicate what files are to be added.</param>
        public void AddDocuments(Lucene.Net.Index.IndexWriter writer, DirectoryInfo directoryInfo, string[] files, SupportedDocumentExtension documents)
        {
            FieldType pathFieldType = new Lucene.Net.Documents.FieldType()
            {
                Indexed = true,
                Tokenized = false,
                Stored = true,
                IndexOptions = FieldInfo.IndexOptions.DOCS_AND_FREQS_AND_POSITIONS,
            };
            FieldType contentFieldType = new Lucene.Net.Documents.FieldType()
            {
                Indexed = true,
                Tokenized = documents.TokenizeContent,
                Stored = documents.StoreContent,
                IndexOptions = FieldInfo.IndexOptions.DOCS_AND_FREQS_AND_POSITIONS,
            };

            // For each file.
            for (int i = 0; i < files.Length; i++)
            {
                // If the file exists
                if (File.Exists(files[i]))
                {
                    Lucene.Net.Documents.Document document = new Lucene.Net.Documents.Document();

                    try
                    {
                        FileInfo fileInfo = new FileInfo(files[i]);
                        string file = files[i].Replace(directoryInfo.Root.FullName, "").ToLower();

                        Lucene.Net.Documents.Field path = new Field("path", file.ToLower().Replace("\\", "/"), pathFieldType);
                        Lucene.Net.Documents.Field modified = new Field("modified", fileInfo.LastWriteTime.ToShortDateString() + " " + fileInfo.LastWriteTime.ToShortTimeString(), pathFieldType);

                        // Add the fields.
                        document.Add(path);
                        document.Add(modified);

                        // Create the stream reader.
                        OpenDocument(files[i]);
                        string content = Nequeo.Xml.Document.ExtractContent(_xDocument);

                        // If content exists.
                        if (!String.IsNullOrEmpty(content))
                        {
                            // Split the white spaces from the text.
                            string[] words = content.Words();

                            // If words exist.
                            if (words != null && words.Length > 0)
                            {
                                // Add the query for each word.
                                for (int j = 0; j < words.Length; j++)
                                {
                                    // Format the word.
                                    string word = words[j].ToLower().RemovePunctuationFromStartAndEnd();

                                    // If a word exists.
                                    if (!String.IsNullOrEmpty(word))
                                    {
                                        Lucene.Net.Documents.Field contentField = new Field("content", word, contentFieldType);
                                        document.Add(contentField);
                                    }
                                }
                            }
                        }

                        // Add the document.
                        writer.AddDocument(document.Fields);
                        _document.Close();

                        // Commit after a set number of documents.
                        documents.TotalDocumentSize += fileInfo.Length;
                        if (documents.TotalDocumentSize > documents.MaxDocumentSizePerCommit)
                        {
                            writer.Commit();
                            documents.TotalDocumentSize = 0;
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        CloseDocument();
                    }
                }
            }
        }

        /// <summary>
        /// Add documents.
        /// </summary>
        /// <param name="writer">The index writer.</param>
        /// <param name="facetWriter">The facet index writer.</param>
        /// <param name="directoryInfo">The directory information where all the files that are to be added are located.</param>
        /// <param name="files">The list of files that are to be added.</param>
        /// <param name="documents">The supported documents search filter, used to indicate what files are to be added.</param>
        /// <param name="facetField">The facet field information.</param>
        /// <param name="config">The facet configuration information.</param>
        public void AddDocuments(Lucene.Net.Index.IndexWriter writer, DirectoryTaxonomyWriter facetWriter,
            DirectoryInfo directoryInfo, string[] files, SupportedDocumentExtension documents, FacetField facetField, FacetsConfig config)
        {
            FieldType pathFieldType = new Lucene.Net.Documents.FieldType()
            {
                Indexed = true,
                Tokenized = false,
                Stored = true,
                IndexOptions = FieldInfo.IndexOptions.DOCS_AND_FREQS_AND_POSITIONS,
            };
            FieldType contentFieldType = new Lucene.Net.Documents.FieldType()
            {
                Indexed = true,
                Tokenized = documents.TokenizeContent,
                Stored = documents.StoreContent,
                IndexOptions = FieldInfo.IndexOptions.DOCS_AND_FREQS_AND_POSITIONS,
            };

            // For each file.
            for (int i = 0; i < files.Length; i++)
            {
                // If the file exists
                if (File.Exists(files[i]))
                {
                    Lucene.Net.Documents.Document document = new Lucene.Net.Documents.Document();

                    try
                    {
                        FileInfo fileInfo = new FileInfo(files[i]);
                        string file = files[i].Replace(directoryInfo.Root.FullName, "").ToLower();

                        Lucene.Net.Documents.Field path = new Field("path", file.ToLower().Replace("\\", "/"), pathFieldType);
                        Lucene.Net.Documents.Field modified = new Field("modified", fileInfo.LastWriteTime.ToShortDateString() + " " + fileInfo.LastWriteTime.ToShortTimeString(), pathFieldType);

                        // Add the fields.
                        document.Add(facetField);
                        document.Add(path);
                        document.Add(modified);

                        // Create the stream reader.
                        OpenDocument(files[i]);
                        string content = Nequeo.Xml.Document.ExtractContent(_xDocument);

                        // If content exists.
                        if (!String.IsNullOrEmpty(content))
                        {
                            // Split the white spaces from the text.
                            string[] words = content.Words();

                            // If words exist.
                            if (words != null && words.Length > 0)
                            {
                                // Add the query for each word.
                                for (int j = 0; j < words.Length; j++)
                                {
                                    // Format the word.
                                    string word = words[j].ToLower().RemovePunctuationFromStartAndEnd();

                                    // If a word exists.
                                    if (!String.IsNullOrEmpty(word))
                                    {
                                        Lucene.Net.Documents.Field contentField = new Field("facetcontent", word, contentFieldType);
                                        document.Add(contentField);
                                    }
                                }
                            }
                        }

                        // Add the document.
                        writer.AddDocument(config.Build(facetWriter, document));
                        _document.Close();

                        // Commit after a set number of documents.
                        documents.TotalDocumentSize += fileInfo.Length;
                        if (documents.TotalDocumentSize > documents.MaxDocumentSizePerCommit)
                        {
                            // Commit the index.
                            writer.Commit();
                            facetWriter.Commit();
                            documents.TotalDocumentSize = 0;
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        CloseDocument();
                    }
                }
            }
        }

        /// <summary>
        /// Open a document with the template.
        /// </summary>
        /// <param name="path">The document path.</param>
        private void OpenDocument(string path)
        {
            // Open the document.
            _document = WordprocessingDocument.Open(path, true);

            // Get the document part from the package.
            _xDocument = new XmlDocument();
            _xDocument.Load(_document.MainDocumentPart.GetStream());
        }

        /// <summary>
        /// Close the document.
        /// </summary>
        private void CloseDocument()
        {
            if (_document != null)
                _document.Dispose();

            _document = null;
            _xDocument = null;
        }
    }
}
