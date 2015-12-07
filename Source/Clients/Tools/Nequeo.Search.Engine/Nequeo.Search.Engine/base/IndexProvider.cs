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

namespace Nequeo.Search.Engine
{
    /// <summary>
    /// Full-Text search engine index provider, used to create and manage documents.
    /// </summary>
    public class IndexProvider
    {
        /// <summary>
        /// Full-Text search engine index provider, used to create and manage documents.
        /// </summary>
        public IndexProvider()
        { }

        /// <summary>
        /// Create a new index store within the specified directory.
        /// </summary>
        /// <param name="directoryIndexInfo">The directory infomation where the index files are to be placed.</param>
        public void CreateIndex(DirectoryInfo directoryIndexInfo)
        {
            Lucene.Net.Index.IndexWriter writer = null;
            Lucene.Net.Store.Directory directory = null;

            try
            {
                // Create the analyzer.
                SimpleAnalyzer simpleAnalyzer = new Analyzer.SimpleAnalyzer();
                StandardAnalyzer standardAnalyzer = new Analyzer.StandardAnalyzer(simpleAnalyzer);

                // Create the index writer.
                directory = FSDirectory.Open(directoryIndexInfo);
                IndexWriterConfig indexConfig = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, standardAnalyzer);
                indexConfig.SetOpenMode(IndexWriterConfig.OpenMode_e.CREATE);

                // Create the new index.
                writer = new IndexWriter(directory, indexConfig);

                // Commit the index.
                writer.Commit();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();

                if (directory != null)
                    directory.Dispose();
            }
        }

        /// <summary>
        /// Create a new facet store within the specified directory.
        /// </summary>
        /// <param name="directoryFacetInfo">The directory infomation where the facet files are to be placed.</param>
        public void CreateMultiFacetIndex(DirectoryInfo directoryFacetInfo)
        {
            DirectoryTaxonomyWriter facetWriter = null;
            Lucene.Net.Store.Directory directoryFacet = null;

            try
            {
                // Create the facet writer.
                directoryFacet = FSDirectory.Open(directoryFacetInfo);
                facetWriter = new DirectoryTaxonomyWriter(directoryFacet, IndexWriterConfig.OpenMode_e.CREATE);

                // Commit the index.
                facetWriter.Commit();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (facetWriter != null)
                    facetWriter.Dispose();

                if (directoryFacet != null)
                    directoryFacet.Dispose();
            }
        }

        /// <summary>
        /// Add documents to the existing index.
        /// </summary>
        /// <param name="directoryIndexInfo">The directory infomation where the index files are located.</param>
        /// <param name="directoryFacetInfo">The directory infomation where the facet files are to be placed.</param>
        /// <param name="facetData">The complete facet information used to build the index information.</param>
        public void AddMultiFacetDocuments(DirectoryInfo directoryIndexInfo, DirectoryInfo directoryFacetInfo, FacetData facetData)
        {
            Lucene.Net.Index.IndexWriter writer = null;
            DirectoryTaxonomyWriter facetWriter = null;

            Lucene.Net.Store.Directory directory = null;
            Lucene.Net.Store.Directory directoryFacet = null;

            try
            {
                if (facetData != null)
                {
                    // Create the analyzer.
                    SimpleAnalyzer simpleAnalyzer = new Analyzer.SimpleAnalyzer();
                    StandardAnalyzer standardAnalyzer = new Analyzer.StandardAnalyzer(simpleAnalyzer);

                    // Create the index writer.
                    directory = FSDirectory.Open(directoryIndexInfo);
                    IndexWriterConfig indexConfig = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, standardAnalyzer);
                    indexConfig.SetOpenMode(IndexWriterConfig.OpenMode_e.APPEND);

                    // Open existing or create new.
                    writer = new IndexWriter(directory, indexConfig);

                    // Create the facet writer.
                    directoryFacet = FSDirectory.Open(directoryFacetInfo);
                    facetWriter = new DirectoryTaxonomyWriter(directoryFacet, IndexWriterConfig.OpenMode_e.APPEND);

                    // Create the facet filter.
                    FacetFilter filter = new FacetFilter();
                    filter.AddDocuments(writer, facetWriter, facetData);

                    // Commit the index.
                    writer.Commit();
                    facetWriter.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();

                if (facetWriter != null)
                    facetWriter.Dispose();

                if (directory != null)
                    directory.Dispose();

                if (directoryFacet != null)
                    directoryFacet.Dispose();
            }
        }

        /// <summary>
        /// Add documents to the existing index.
        /// </summary>
        /// <param name="directoryIndexInfo">The directory infomation where the index files are located.</param>
        /// <param name="directoryInfo">The directory information where all the files that are to be added are located.</param>
        /// <param name="documents">The supported documents search filter, used to indicate what files are to be added.</param>
        public void AddDocuments(DirectoryInfo directoryIndexInfo, DirectoryInfo directoryInfo, SupportedDocumentExtension documents)
        {
            Lucene.Net.Index.IndexWriter writer = null;
            Lucene.Net.Store.Directory directory = null;

            try
            {
                if (documents != null)
                {
                    // Create the analyzer.
                    SimpleAnalyzer simpleAnalyzer = new Analyzer.SimpleAnalyzer();
                    StandardAnalyzer standardAnalyzer = new Analyzer.StandardAnalyzer(simpleAnalyzer);

                    // Create the index writer.
                    directory = FSDirectory.Open(directoryIndexInfo);
                    IndexWriterConfig indexConfig = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, standardAnalyzer);
                    indexConfig.SetOpenMode(IndexWriterConfig.OpenMode_e.APPEND);

                    // Open existing or create new.
                    writer = new IndexWriter(directory, indexConfig);

                    // Create the directory filter.
                    DirectoryFilter filter = new DirectoryFilter();
                    filter.AddDocuments(writer, directoryInfo, documents);

                    // Commit the index.
                    writer.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();

                if (directory != null)
                    directory.Dispose();
            }
        }

        /// <summary>
        /// Remove facet documents from the existing index.
        /// </summary>
        /// <param name="directoryIndexInfo">The directory infomation where the index files are located.</param>
        /// <param name="directoryFacetInfo">The directory infomation where the facet files are to be placed.</param>
        /// <param name="textNames">The array of names for text data.</param>
        /// <param name="filePaths">The array of full paths (without root 'C:\'. e.g. 'temp/http/index.html') for file documents.</param>
        public void RemoveMultiFacetDocuments(DirectoryInfo directoryIndexInfo, DirectoryInfo directoryFacetInfo, string[] textNames, string[] filePaths)
        {
            Lucene.Net.Index.IndexWriter writer = null;
            DirectoryTaxonomyWriter facetWriter = null;

            Lucene.Net.Store.Directory directory = null;
            Lucene.Net.Store.Directory directoryFacet = null;

            try
            {
                // Create the analyzer.
                SimpleAnalyzer simpleAnalyzer = new Analyzer.SimpleAnalyzer();
                StandardAnalyzer standardAnalyzer = new Analyzer.StandardAnalyzer(simpleAnalyzer);

                // Create the index writer.
                directory = FSDirectory.Open(directoryIndexInfo);
                IndexWriterConfig indexConfig = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, standardAnalyzer);
                indexConfig.SetOpenMode(IndexWriterConfig.OpenMode_e.APPEND);

                // Open existing or create new.
                writer = new IndexWriter(directory, indexConfig);

                // Create the facet writer.
                directoryFacet = FSDirectory.Open(directoryFacetInfo);
                facetWriter = new DirectoryTaxonomyWriter(directoryFacet, IndexWriterConfig.OpenMode_e.APPEND);

                // Create the delet query.
                FacetFilter filter = new FacetFilter();
                Query[] queries = filter.RemoveDocuments(textNames, filePaths);
                writer.DeleteDocuments(queries);

                // Commit the index.
                writer.Commit();
                facetWriter.Commit();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();

                if (directory != null)
                    directory.Dispose();
            }
        }

        /// <summary>
        /// Remove documents from the existing index.
        /// </summary>
        /// <param name="directoryIndexInfo">The directory infomation where the index files are located.</param>
        /// <param name="directoryInfo">The top level relative directory information where all the files that are to be removed are located.</param>
        /// <param name="files">The array of all files that are to be removed relative to the directory info.</param>
        /// <param name="documents">The supported documents search filter, used to indicate what files are to be removed.</param>
        public void RemoveDocuments(DirectoryInfo directoryIndexInfo, DirectoryInfo directoryInfo, string[] files, SupportedDocumentExtension documents)
        {
            Lucene.Net.Index.IndexWriter writer = null;
            Lucene.Net.Store.Directory directory = null;

            try
            {
                if (documents != null)
                {
                    // Create the analyzer.
                    SimpleAnalyzer simpleAnalyzer = new Analyzer.SimpleAnalyzer();
                    StandardAnalyzer standardAnalyzer = new Analyzer.StandardAnalyzer(simpleAnalyzer);

                    // Create the index writer.
                    directory = FSDirectory.Open(directoryIndexInfo);
                    IndexWriterConfig indexConfig = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, standardAnalyzer);
                    indexConfig.SetOpenMode(IndexWriterConfig.OpenMode_e.APPEND);

                    // Open existing or create new.
                    writer = new IndexWriter(directory, indexConfig);

                    // Create the directory filter.
                    DirectoryFilter filter = new DirectoryFilter();
                    Query[] queries = filter.RemoveDocuments(directoryInfo, files, documents);
                    writer.DeleteDocuments(queries);

                    // Commit the index.
                    writer.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();

                if (directory != null)
                    directory.Dispose();
            }
        }

        /// <summary>
        /// Add text to the existing index.
        /// </summary>
        /// <param name="directoryIndexInfo">The directory infomation where the index files are located.</param>
        /// <param name="addTextData">The text data to add.</param>
        public void AddText(DirectoryInfo directoryIndexInfo, AddTextData[] addTextData)
        {
            Lucene.Net.Index.IndexWriter writer = null;
            Lucene.Net.Store.Directory directory = null;
            long totalTextLength = 0;
            long maxTextLengthBeforeCommit = 30000000L;

            try
            {
                // If text exists.
                if (addTextData != null && addTextData.Length > 0)
                {
                    // Create the analyzer.
                    SimpleAnalyzer simpleAnalyzer = new Analyzer.SimpleAnalyzer();
                    StandardAnalyzer standardAnalyzer = new Analyzer.StandardAnalyzer(simpleAnalyzer);

                    // Create the index writer.
                    directory = FSDirectory.Open(directoryIndexInfo);
                    IndexWriterConfig indexConfig = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, standardAnalyzer);
                    indexConfig.SetOpenMode(IndexWriterConfig.OpenMode_e.APPEND);

                    // Open existing or create new.
                    writer = new IndexWriter(directory, indexConfig);

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
                    foreach (AddTextData data in addTextData)
                    {
                        // Should the data be stored.
                        completeFieldType.Stored = data.StoreText;

                        // Create the document.
                        Lucene.Net.Documents.Document document = new Lucene.Net.Documents.Document();
                        Lucene.Net.Documents.Field textName = new Field("textname", data.Name.ToLower(), nameFieldType);
                        Lucene.Net.Documents.Field textComplete = new Field("textcomplete", data.Text.ToLower(), completeFieldType);
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
                                    Lucene.Net.Documents.Field textData = new Field("text", word, textFieldType);
                                    document.Add(textData);
                                }
                            }
                        }

                        // Add the document.
                        writer.AddDocument(document.Fields);

                        // Commit after a set number of documents.
                        totalTextLength += (long)data.Text.Length;
                        if (totalTextLength > maxTextLengthBeforeCommit)
                        {
                            // Commit the index.
                            writer.Commit();
                            totalTextLength = 0;
                        }
                    }

                    // Commit the index.
                    writer.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();

                if (directory != null)
                    directory.Dispose();
            }
        }

        /// <summary>
        /// Remove text from the existing index.
        /// </summary>
        /// <param name="directoryIndexInfo">The directory infomation where the index files are located.</param>
        /// <param name="names">An array of unique names for the text.</param>
        public void RemoveText(DirectoryInfo directoryIndexInfo, string[] names)
        {
            Lucene.Net.Index.IndexWriter writer = null;
            Lucene.Net.Store.Directory directory = null;

            try
            {
                // If exists.
                if (names != null && names.Length > 0)
                {
                    // Create the analyzer.
                    SimpleAnalyzer simpleAnalyzer = new Analyzer.SimpleAnalyzer();
                    StandardAnalyzer standardAnalyzer = new Analyzer.StandardAnalyzer(simpleAnalyzer);

                    // Create the index writer.
                    directory = FSDirectory.Open(directoryIndexInfo);
                    IndexWriterConfig indexConfig = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, standardAnalyzer);
                    indexConfig.SetOpenMode(IndexWriterConfig.OpenMode_e.APPEND);

                    // Open existing or create new.
                    writer = new IndexWriter(directory, indexConfig);

                    // Create the query.
                    List<Query> queries = new List<Query>();

                    // For each name.
                    foreach (string name in names)
                    {
                        // Create the query.
                        BooleanQuery query = new BooleanQuery();
                        query.Add(new TermQuery(new Term("textname", name.ToLower())), BooleanClause.Occur.MUST);

                        // Add the query.
                        queries.Add(query);
                    }
                    
                    // Delete the text.
                    writer.DeleteDocuments(queries.ToArray());

                    // Commit the index.
                    writer.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();

                if (directory != null)
                    directory.Dispose();
            }
        }
    }
}
