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

using Lucene.Net;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Search;
using Lucene.Net.Util;
using Lucene.Net.Documents;
using Lucene.Net.Facet;
using Lucene.Net.Facet.Taxonomy.Directory;

using Nequeo.Html;
using Nequeo.Extension;
using Nequeo.Drawing.Pdf;
using Nequeo.Search.Engine.Analyzer;
using Nequeo.Search.Engine.Filter;
using Nequeo.IO;

namespace Nequeo.Search.Engine.Filter
{
    /// <summary>
    /// Facet filter.
    /// </summary>
    internal class FacetFilter
    {
        /// <summary>
        /// Facet filter.
        /// </summary>
        public FacetFilter() { }

        /// <summary>
        /// Add documents.
        /// </summary>
        /// <param name="writer">The index writer.</param>
        /// <param name="facetWriter">The facet index writer.</param>
        /// <param name="facetData">The complete facet information used to build the index information.</param>
        public void AddDocuments(Lucene.Net.Index.IndexWriter writer, DirectoryTaxonomyWriter facetWriter, FacetData facetData)
        {
            // Build the facet configuration information.
            FacetsConfig config = new FacetsConfig();

            // Builder hierarchicals.
            if(facetData.Hierarchicals != null && facetData.Hierarchicals.Length > 0)
            {
                // Add the config.
                foreach (FacetData.Hierarchical item in facetData.Hierarchicals)
                    config.SetHierarchical(item.DimensionName, item.IsHierarchical);
            }

            // Builder index fields.
            if (facetData.IndexFields != null && facetData.IndexFields.Length > 0)
            {
                // Add the config.
                foreach (FacetData.IndexField item in facetData.IndexFields)
                    config.SetIndexFieldName(item.DimensionName, item.IndexFieldName);
            }

            // Builder multi values.
            if (facetData.MultiValues != null && facetData.MultiValues.Length > 0)
            {
                // Add the config.
                foreach (FacetData.MultiValued item in facetData.MultiValues)
                    config.SetMultiValued(item.DimensionName, item.IsMultiValue);
            }

            // Builder require dimension counts.
            if (facetData.RequireDimensionCounts != null && facetData.RequireDimensionCounts.Length > 0)
            {
                // Add the config.
                foreach (FacetData.RequireDimensionCount item in facetData.RequireDimensionCounts)
                    config.SetRequireDimCount(item.DimensionName, item.IsAccurateCountsRequired);
            }

            // Add text data.
            if(facetData.TextFacetFields.Count > 0)
            {
                // Add the text.
                AddText(writer, facetWriter, facetData.TextFacetFields, config);
            }

            // Add file data.
            if (facetData.FileFacetFields.Count > 0)
            {
                // Add the file.
                AddFile(writer, facetWriter, facetData.FileFacetFields, config);
            }
        }

        /// <summary>
        /// Add text to the existing index.
        /// </summary>
        /// <param name="writer">The index writer.</param>
        /// <param name="facetWriter">The facet index writer.</param>
        /// <param name="addFileData">The file data to add.</param>
        /// <param name="config">The facet configuration information.</param>
        public void AddFile(Lucene.Net.Index.IndexWriter writer, DirectoryTaxonomyWriter facetWriter, Dictionary<FacetField, FileFacetModel> addFileData, FacetsConfig config)
        {
            Nequeo.IO.Directory directory = new Nequeo.IO.Directory();

            // For each file facet.
            foreach (KeyValuePair<FacetField, FileFacetModel> item in addFileData)
            {
                // Select the document format filter.
                // If html has been selected.
                if (item.Value.Documents.SupportedDocuments.HasFlag(SupportedDocuments.Html))
                {
                    // Create the html filter.
                    HtmlFilter htmlFilter = new HtmlFilter();
                    string[] files = directory.GetFiles(item.Value.DirectoryInfo.FullName, item.Value.Documents.GetFormattedSearchPatterns(SupportedDocuments.Html));
                    htmlFilter.AddDocuments(writer, facetWriter, item.Value.DirectoryInfo, files, item.Value.Documents, item.Key, config);
                }

                // If pdf has been selected.
                if (item.Value.Documents.SupportedDocuments.HasFlag(SupportedDocuments.Pdf))
                {
                    // Create the pdf filter.
                    PdfFilter pdfFilter = new PdfFilter();
                    string[] files = directory.GetFiles(item.Value.DirectoryInfo.FullName, item.Value.Documents.GetFormattedSearchPatterns(SupportedDocuments.Pdf));
                    pdfFilter.AddDocuments(writer, facetWriter, item.Value.DirectoryInfo, files, item.Value.Documents, item.Key, config);
                }

                // If rtf has been selected.
                if (item.Value.Documents.SupportedDocuments.HasFlag(SupportedDocuments.Rtf))
                {
                    // Create the rtf filter.
                    RtfFilter rtfFilter = new RtfFilter();
                    string[] files = directory.GetFiles(item.Value.DirectoryInfo.FullName, item.Value.Documents.GetFormattedSearchPatterns(SupportedDocuments.Rtf));
                    rtfFilter.AddDocuments(writer, facetWriter, item.Value.DirectoryInfo, files, item.Value.Documents, item.Key, config);
                }

                // If txt has been selected.
                if (item.Value.Documents.SupportedDocuments.HasFlag(SupportedDocuments.Txt))
                {
                    // Create the txt filter.
                    TxtFilter txtFilter = new TxtFilter();
                    string[] files = directory.GetFiles(item.Value.DirectoryInfo.FullName, item.Value.Documents.GetFormattedSearchPatterns(SupportedDocuments.Txt));
                    txtFilter.AddDocuments(writer, facetWriter, item.Value.DirectoryInfo, files, item.Value.Documents, item.Key, config);
                }

                // If xml has been selected.
                if (item.Value.Documents.SupportedDocuments.HasFlag(SupportedDocuments.Xml))
                {
                    // Create the xml filter.
                    XmlFilter xmlFilter = new XmlFilter();
                    string[] files = directory.GetFiles(item.Value.DirectoryInfo.FullName, item.Value.Documents.GetFormattedSearchPatterns(SupportedDocuments.Xml));
                    xmlFilter.AddDocuments(writer, facetWriter, item.Value.DirectoryInfo, files, item.Value.Documents, item.Key, config);
                }

                // If docx has been selected.
                if (item.Value.Documents.SupportedDocuments.HasFlag(SupportedDocuments.Docx))
                {
                    // Create the docx filter.
                    MSDocFilter docxFilter = new MSDocFilter();
                    string[] files = directory.GetFiles(item.Value.DirectoryInfo.FullName, item.Value.Documents.GetFormattedSearchPatterns(SupportedDocuments.Docx));
                    docxFilter.AddDocuments(writer, facetWriter, item.Value.DirectoryInfo, files, item.Value.Documents, item.Key, config);
                }
            }
        }

        /// <summary>
        /// Add text to the existing index.
        /// </summary>
        /// <param name="writer">The index writer.</param>
        /// <param name="facetWriter">The facet index writer.</param>
        /// <param name="addTextData">The text data to add.</param>
        /// <param name="config">The facet configuration information.</param>
        public void AddText(Lucene.Net.Index.IndexWriter writer, DirectoryTaxonomyWriter facetWriter, Dictionary<FacetField, AddTextData[]> addTextData, FacetsConfig config)
        {
            long totalTextLength = 0;
            long maxTextLengthBeforeCommit = 30000000L;

            // For each text facet.
            foreach (KeyValuePair<FacetField, AddTextData[]> item in addTextData)
            {
                // If text exists.
                if (item.Value != null && item.Value.Length > 0)
                {
                    // Add the text.
                    FieldType nameFieldType = new Lucene.Net.Documents.FieldType()
                    {
                        Indexed = true,
                        Tokenized = false,
                        Stored = true,
                        IndexOptions = FieldInfo.IndexOptions.DOCS_AND_FREQS_AND_POSITIONS,
                    };

                    // Add the text.
                    FieldType completeFieldType = new Lucene.Net.Documents.FieldType()
                    {
                        Indexed = true,
                        Tokenized = false,
                        Stored = true,
                        IndexOptions = FieldInfo.IndexOptions.DOCS_AND_FREQS_AND_POSITIONS,
                    };

                    // Add the text.
                    FieldType textFieldType = new Lucene.Net.Documents.FieldType()
                    {
                        Indexed = true,
                        Tokenized = false,
                        Stored = false,
                        IndexOptions = FieldInfo.IndexOptions.DOCS_AND_FREQS_AND_POSITIONS,
                    };

                    // For each text.
                    foreach (AddTextData data in item.Value)
                    {
                        // Should the data be stored.
                        completeFieldType.Stored = data.StoreText;

                        // Create the document.
                        Lucene.Net.Documents.Document document = new Lucene.Net.Documents.Document();
                        Lucene.Net.Documents.Field textName = new Field("textname", data.Name.ToLower(), nameFieldType);
                        Lucene.Net.Documents.Field textComplete = new Field("textcomplete", data.Text.ToLower(), completeFieldType);

                        document.Add(item.Key);
                        document.Add(textName);
                        document.Add(textComplete);

                        // Split the white spaces from the text.
                        string[] words = data.Text.Words();

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
                                    Lucene.Net.Documents.Field textData = new Field("facetcontent", word, textFieldType);
                                    document.Add(textData);
                                }
                            }
                        }

                        // Add the document.
                        writer.AddDocument(config.Build(facetWriter, document));

                        // Commit after a set number of documents.
                        totalTextLength += (long)data.Text.Length;
                        if (totalTextLength > maxTextLengthBeforeCommit)
                        {
                            // Commit the index.
                            writer.Commit();
                            facetWriter.Commit();
                            totalTextLength = 0;
                        }
                    }
                }
            }

            // Commit the index.
            writer.Commit();
            facetWriter.Commit();
        }

        /// <summary>
        /// Remove facet documents from the existing index.
        /// </summary>
        /// <param name="textNames">The array of names for text data.</param>
        /// <param name="filePaths">The array of full paths (without root 'C:\'. e.g. 'temp/http/index.html') for file documents.</param>
        /// <returns>The array of queries that indicate which documents are to be removed.</returns>
        public Query[] RemoveDocuments(string[] textNames, string[] filePaths)
        {
            List<Query> queries = new List<Query>();

            // Create the query for each documents that need to be removed.
            for (int i = 0; i < textNames.Length; i++)
            {
                // Create the query.
                BooleanQuery query = new BooleanQuery();
                query.Add(new TermQuery(new Term("textname", textNames[i].ToLower())), BooleanClause.Occur.MUST);

                // Add the query.
                queries.Add(query);
            }

            // Create the query for each documents that need to be removed.
            for (int i = 0; i < filePaths.Length; i++)
            {
                // Create the query.
                BooleanQuery query = new BooleanQuery();
                query.Add(new TermQuery(new Term("path", filePaths[i].ToLower())), BooleanClause.Occur.MUST);

                // Add the query.
                queries.Add(query);
            }

            // Return the list of queries.
            return queries.ToArray();
        }
    }
}
