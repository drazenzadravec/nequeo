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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Lucene.Net;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Search;
using Lucene.Net.Util;
using Lucene.Net.Documents;
using Lucene.Net.Facet;
using Lucene.Net.Facet.Taxonomy;
using Lucene.Net.Facet.Taxonomy.Directory;

using Nequeo.Extension;
using Nequeo.Search.Engine.Analyzer;

namespace Nequeo.Search.Engine
{
    /// <summary>
    /// Full-Text search engine search provider, used to search indexed documents.
    /// </summary>
    public class SearchProvider : IDisposable
    {
        /// <summary>
        /// Full-Text search engine search provider, used to search indexed documents.
        /// </summary>
        /// <param name="directoryIndexInfo">The directory infomation where the index files are located.</param>
        public SearchProvider(DirectoryInfo directoryIndexInfo)
        { 
            try
            {
                // Create the index reader.
                Lucene.Net.Store.Directory directory = FSDirectory.Open(directoryIndexInfo);
                _reader = Lucene.Net.Index.DirectoryReader.Open(directory);
            }
            catch (Exception)
            {
                if (_reader != null)
                    _reader.Dispose();

                throw;
            }
        }

        /// <summary>
        /// Full-Text search engine search provider, used to search indexed documents.
        /// </summary>
        /// <param name="directoryIndexInfos">The array directory infomation where the index files are located.</param>
        public SearchProvider(DirectoryInfo[] directoryIndexInfos)
        {
            try
            {
                List<Lucene.Net.Index.IndexReader> readers = new List<IndexReader>();

                // For each directory.
                foreach (DirectoryInfo item in directoryIndexInfos)
                {
                    // Create the index reader.
                    Lucene.Net.Store.Directory directory = FSDirectory.Open(item);
                    Lucene.Net.Index.IndexReader reader = Lucene.Net.Index.DirectoryReader.Open(directory);
                    readers.Add(reader);
                }

                // Create the multiple index readers.
                _reader = new Lucene.Net.Index.MultiReader(readers.ToArray(), true);
            }
            catch (Exception)
            {
                if (_reader != null)
                    _reader.Dispose();

                throw;
            }
        }

        /// <summary>
        /// Search engine search provider, used to search indexed documents.
        /// </summary>
        /// <param name="directoryIndexInfo">The directory infomation where the index files are located.</param>
        /// <param name="directoryFacetInfo">The directory infomation where the facet files are to be placed.</param>
        public SearchProvider(DirectoryInfo directoryIndexInfo, DirectoryInfo directoryFacetInfo)
        {
            try
            {
                // Create the index reader.
                Lucene.Net.Store.Directory directory = FSDirectory.Open(directoryIndexInfo);
                _reader = Lucene.Net.Index.DirectoryReader.Open(directory);

                // Create the facet reader.
                Lucene.Net.Store.Directory directoryFacet = FSDirectory.Open(directoryFacetInfo);
                _facetReader = new DirectoryTaxonomyReader(directoryFacet);
            }
            catch (Exception)
            {
                if (_reader != null)
                    _reader.Dispose();

                if (_facetReader != null)
                    _facetReader.Dispose();

                throw;
            }
        }

        /// <summary>
        /// Search engine search provider, used to search indexed documents.
        /// </summary>
        /// <param name="directoryIndexInfos">The array directory infomation where the index files are located.</param>
        /// <param name="directoryFacetInfo">The directory infomation where the facet files are to be placed.</param>
        public SearchProvider(DirectoryInfo[] directoryIndexInfos, DirectoryInfo directoryFacetInfo)
        {
            try
            {
                List<Lucene.Net.Index.IndexReader> readers = new List<IndexReader>();

                // For each directory.
                foreach (DirectoryInfo item in directoryIndexInfos)
                {
                    // Create the index reader.
                    Lucene.Net.Store.Directory directory = FSDirectory.Open(item);
                    Lucene.Net.Index.IndexReader reader = Lucene.Net.Index.DirectoryReader.Open(directory);
                    readers.Add(reader);
                }

                // Create the multiple index readers.
                _reader = new Lucene.Net.Index.MultiReader(readers.ToArray(), true);

                // Create the facet reader.
                Lucene.Net.Store.Directory directoryFacet = FSDirectory.Open(directoryFacetInfo);
                _facetReader = new DirectoryTaxonomyReader(directoryFacet);
            }
            catch (Exception)
            {
                if (_reader != null)
                    _reader.Dispose();

                if (_facetReader != null)
                    _facetReader.Dispose();

                throw;
            }
        }

        private Lucene.Net.Index.IndexReader _reader = null;
        private DirectoryTaxonomyReader _facetReader = null;

        /// <summary>
        /// Search text in the existing index.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        /// <param name="numberToReturn">The maximum number of documents to return.</param>
        /// <returns>The text data.</returns>
        /// <remarks>Search for text that is stored along with the index data.
        /// Use wildcard chars ('*', '?', '\'), logical ('AND', 'OR'), Quoted exact phrase ("search this")</remarks>
        public Nequeo.Search.Engine.TextData SearchText(string text, int numberToReturn = Int32.MaxValue)
        {
            Nequeo.Search.Engine.TextData documents = new TextData();
            documents.TotalHits = 0;

            try
            {
                // If text exists.
                if (!String.IsNullOrEmpty(text))
                {
                    // Load the searcher.
                    Lucene.Net.Search.IndexSearcher searcher = new Lucene.Net.Search.IndexSearcher(_reader);
                    string searchFieldName = "text";
                    Query query = null;
                    TopDocs results = null;

                    // Get bytes
                    char[] textArray = text.ToCharArray();

                    // Search logical.
                    if (text.Contains("AND") || text.Contains("OR"))
                    {
                        // Create the query.
                        query = CreateLogicalQuery(text, searchFieldName);
                    }
                    else if (textArray[0].Equals('"') && textArray[textArray.Length - 1].Equals('"'))
                    {
                        // Create the query.
                        query = CreateQuotedQuery(new string(textArray, 1, textArray.Length - 2), searchFieldName);
                    }
                    else
                    {
                        // Create the query.
                        query = CreateBoolenQuery(text, BooleanClause.Occur.SHOULD, searchFieldName);
                    }

                    // Search.
                    if (query != null)
                        results = searcher.Search(query, numberToReturn);

                    // Get the total number of results that was asked for.
                    int totalResult = ((results.ScoreDocs != null && results.ScoreDocs.Length > 0) ? results.ScoreDocs.Length : 0);

                    // If result found.
                    if (results != null && results.TotalHits > 0)
                    {
                        List<TextDataResult> textDataResults = new List<TextDataResult>();

                        // For each document found.
                        for (int i = 0; i < totalResult; i++)
                        {
                            TextDataResult document = new TextDataResult();
                            int docID = results.ScoreDocs[i].Doc;
                            Lucene.Net.Documents.Document doc = searcher.Doc(docID);

                            // Get the data for each field.
                            IndexableField[] textNameFields = doc.GetFields("textname");

                            // Assign the data to the text document.
                            document.Name = textNameFields.Length > 0 ? textNameFields[0].StringValue : null;
                            document.Score = results.ScoreDocs[i].Score;
                            document.Doc = docID;

                            try
                            {
                                // Do not know if the text was stored.
                                IndexableField[] textValueFields = doc.GetFields("textcomplete");
                                document.Text = textValueFields.Length > 0 ? textValueFields[0].StringValue : null;
                            }
                            catch { }

                            // Add the document.
                            textDataResults.Add(document);
                        }

                        // Assign
                        documents.TotalHits = results.TotalHits;
                        documents.MaxScore = results.MaxScore;
                        documents.Results = textDataResults.ToArray();
                    }
                }

                // Return the documents.
                return documents;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Search file content in the existing index.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        /// <param name="numberToReturn">The maximum number of documents to return.</param>
        /// <returns>The file document.</returns>
        /// <remarks>Use wildcard chars ('*', '?', '\'), logical ('AND', 'OR'), Quoted exact phrase ("search this").</remarks>
        public Nequeo.Search.Engine.FileDocument SearchDocument(string text, int numberToReturn = Int32.MaxValue)
        {
            Nequeo.Search.Engine.FileDocument documents = new FileDocument();
            documents.TotalHits = 0;

            try
            {
                // If text exists.
                if (!String.IsNullOrEmpty(text))
                {
                    // Load the searcher.
                    Lucene.Net.Search.IndexSearcher searcher = new Lucene.Net.Search.IndexSearcher(_reader);
                    string searchFieldName = "content";
                    Query query = null;
                    TopDocs results = null;
                    
                    // Get bytes
                    char[] textArray = text.ToCharArray();

                    // Search logical.
                    if (text.Contains("AND") || text.Contains("OR"))
                    {
                        // Create the query.
                        query = CreateLogicalQuery(text, searchFieldName);
                    }
                    else if (textArray[0].Equals('"') && textArray[textArray.Length - 1].Equals('"'))
                    {
                        // Create the query.
                        query = CreateQuotedQuery(new string(textArray, 1, textArray.Length - 2), searchFieldName);
                    }
                    else
                    {
                        // Create the query.
                        query = CreateBoolenQuery(text, BooleanClause.Occur.SHOULD, searchFieldName);
                    }

                    // Search.
                    if (query != null)
                        results = searcher.Search(query, numberToReturn);

                    // Get the total number of results that was asked for.
                    int totalResult = ((results.ScoreDocs != null && results.ScoreDocs.Length > 0) ? results.ScoreDocs.Length : 0);

                    // If result found.
                    if (results != null && results.TotalHits > 0)
                    {
                        List<FileDocumentResult> fileDocResults = new List<FileDocumentResult>();

                        // For each document found.
                        for (int i = 0; i < totalResult; i++)
                        {
                            FileDocumentResult document = new FileDocumentResult();
                            int docID = results.ScoreDocs[i].Doc;
                            Lucene.Net.Documents.Document doc = searcher.Doc(docID);

                            // Get the data for each field.
                            IndexableField[] pathNameFields = doc.GetFields("path");
                            IndexableField[] modifiedNameFields = doc.GetFields("modified");

                            // Assign the data to the path document.
                            document.Path = pathNameFields.Length > 0 ? pathNameFields[0].StringValue : null;
                            document.Modified = modifiedNameFields.Length > 0 ? modifiedNameFields[0].StringValue : null;
                            document.Score = results.ScoreDocs[i].Score;
                            document.Doc = docID;

                            // Add the document.
                            fileDocResults.Add(document);
                        }

                        // Assign
                        documents.TotalHits = results.TotalHits;
                        documents.MaxScore = results.MaxScore;
                        documents.Results = fileDocResults.ToArray();
                    }
                }

                // Return the documents.
                return documents;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Search facet content in the existing index.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        /// <param name="indexFields">The array of index fields to search in.</param>
        /// <param name="facetPaths">The array of facet paths to perform a drill down search on.</param>
        /// <param name="numberToReturn">The maximum number of documents to return.</param>
        /// <returns>The facet document.</returns>
        /// <remarks>Use wildcard chars ('*', '?', '\'), logical ('AND', 'OR'), Quoted exact phrase ("search this").</remarks>
        public Nequeo.Search.Engine.FacetDocument SearchFacetDocument(string text, FacetData.IndexField[] indexFields, FacetPath[] facetPaths, int numberToReturn = Int32.MaxValue)
        {
            Nequeo.Search.Engine.FacetDocument documents = new FacetDocument();
            documents.TotalHits = 0;

            try
            {
                // If text exists.
                if (!String.IsNullOrEmpty(text))
                {
                    // Load the searcher.
                    Lucene.Net.Search.IndexSearcher searcher = new Lucene.Net.Search.IndexSearcher(_reader);
                    string searchFieldName = "facetcontent";
                    Query query = null;
                    DrillDownQuery queryFacet = null;
                    TopDocs results = null;

                    // Build the facet configuration information.
                    FacetsConfig config = new FacetsConfig();

                    // Add the config.
                    foreach (FacetData.IndexField item in indexFields)
                        config.SetIndexFieldName(item.DimensionName, item.IndexFieldName);

                    // Get bytes
                    char[] textArray = text.ToCharArray();

                    // Search logical.
                    if (text.Contains("AND") || text.Contains("OR"))
                    {
                        // Create the query.
                        query = CreateLogicalQuery(text, searchFieldName);
                    }
                    else if (textArray[0].Equals('"') && textArray[textArray.Length - 1].Equals('"'))
                    {
                        // Create the query.
                        query = CreateQuotedQuery(new string(textArray, 1, textArray.Length - 2), searchFieldName);
                    }
                    else
                    {
                        // Create the query.
                        query = CreateBoolenQuery(text, BooleanClause.Occur.SHOULD, searchFieldName);
                    }

                    // Create the facet query.
                    queryFacet = new DrillDownQuery(config, query);
                    foreach (FacetPath facetPath in facetPaths)
                    {
                        // Add the path.
                        queryFacet.Add(facetPath.DimensionName, facetPath.Path);
                    }

                    // The collector.
                    FacetsCollector collector = new FacetsCollector();

                    // Search.
                    if (queryFacet != null)
                        results = FacetsCollector.Search(searcher, queryFacet, numberToReturn, collector);

                    // Get the total number of results that was asked for.
                    int totalResult = ((results.ScoreDocs != null && results.ScoreDocs.Length > 0) ? results.ScoreDocs.Length : 0);

                    // If result found.
                    if (results != null && results.TotalHits > 0)
                    {
                        List<TextDataResult> textDataResults = new List<TextDataResult>();
                        List<FileDocumentResult> fileDocResults = new List<FileDocumentResult>();

                        List<FacetPathResult> facetPathResults = new List<FacetPathResult>();
                        IDictionary<string, Facets> facetsMap = new Dictionary<string, Facets>();

                        // Add the facet count.
                        foreach (FacetData.IndexField item in indexFields)
                        {
                            // Add the facet for each index field.
                            facetsMap[item.DimensionName] = GetTaxonomyFacetCounts(_facetReader, config, collector, item.IndexFieldName);
                        }

                        // Create the multi facet list.
                        foreach (FacetPath facetPath in facetPaths)
                        {
                            try
                            {
                                // Add the facets.
                                Facets facets = facetsMap.First(u => u.Key.ToLower().Contains(facetPath.DimensionName.ToLower())).Value;
                                float number = facets.GetSpecificValue(facetPath.DimensionName, facetPath.Path);
                                
                                // Add the path.
                                facetPathResults.Add(new FacetPathResult(facetPath.DimensionName, number, facetPath.Path));
                            }
                            catch { }
                        }
                        
                        // For each document found.
                        for (int i = 0; i < totalResult; i++)
                        {
                            FileDocumentResult fileDocument = null;
                            TextDataResult textData = null;

                            int docID = results.ScoreDocs[i].Doc;
                            Lucene.Net.Documents.Document doc = searcher.Doc(docID);

                            try
                            {
                                // Get the data for each field.
                                IndexableField[] textNameFields = doc.GetFields("textname");

                                // If this field exists then text data.
                                if (textNameFields.Length > 0)
                                {
                                    // Assign the data to the text document.
                                    textData = new TextDataResult();
                                    textData.Name = textNameFields.Length > 0 ? textNameFields[0].StringValue : null;
                                    textData.Score = results.ScoreDocs[i].Score;
                                    textData.Doc = docID;

                                    // Do not know if the text was stored.
                                    IndexableField[] textValueFields = doc.GetFields("textcomplete");
                                    textData.Text = textValueFields.Length > 0 ? textValueFields[0].StringValue : null;
                                }
                            }
                            catch { }

                            // If text data exists then add.
                            if (textData != null)
                                textDataResults.Add(textData);

                            try
                            {
                                // Get the data for each field.
                                IndexableField[] pathNameFields = doc.GetFields("path");
                                IndexableField[] modifiedNameFields = doc.GetFields("modified");

                                // If this field exists then file document.
                                if (pathNameFields.Length > 0)
                                {
                                    // Assign the data to the path document.
                                    fileDocument = new FileDocumentResult();
                                    fileDocument.Path = pathNameFields.Length > 0 ? pathNameFields[0].StringValue : null;
                                    fileDocument.Modified = modifiedNameFields.Length > 0 ? modifiedNameFields[0].StringValue : null;
                                    fileDocument.Score = results.ScoreDocs[i].Score;
                                    fileDocument.Doc = docID;
                                }
                            }
                            catch { }

                            // If file data exists then add.
                            if (fileDocument != null)
                                fileDocResults.Add(fileDocument);
                        }

                        // Assign the facet document values.
                        documents.MaxScore = results.MaxScore;
                        documents.TotalHits = results.TotalHits;
                        documents.FacetPathResults = facetPathResults.ToArray();
                        documents.TextDataResults = textDataResults.ToArray();
                        documents.FileDocumentResults = fileDocResults.ToArray();
                    }
                }

                // Return the documents.
                return documents;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the taxonomy facet caounts
        /// </summary>
        /// <param name="taxoReader">The taxonomy reader.</param>
        /// <param name="config">The facet configuration.</param>
        /// <param name="collector">The result collector.</param>
        /// <param name="indexFieldName">The index field name.</param>
        /// <returns>The facets.</returns>
        private Facets GetTaxonomyFacetCounts(TaxonomyReader taxoReader, FacetsConfig config, FacetsCollector collector, string indexFieldName)
        {
            Facets facets  = new FastTaxonomyFacetCounts(indexFieldName, taxoReader, config, collector);
            return facets;
        }

        /// <summary>
        /// Does the word contain a wildcard.
        /// </summary>
        /// <param name="word">The word to examine.</param>
        /// <returns>True if a wild card exists; else false.</returns>
        private bool ContainsWildcard(string word)
        {
            bool found = false;

            // For each char in the word.
            foreach (char character in word)
            {
                if (character.Equals('*') || character.Equals('?') || character.Equals('\\'))
                    found = true;
            }

            // Return found value.
            return found;
        }

        /// <summary>
        /// Find all the quoted text.
        /// </summary>
        /// <param name="text">The text to search.</param>
        /// <returns>The quoted text list; the last string item in the collection is un-quoted text.</returns>
        private QuotedStringModel[] FindQuotedText(string text)
        {
            bool startFound = false;
            bool endFound = false;

            List<QuotedStringModel> searchData = new List<QuotedStringModel>();
            List<char> unQuotedChars = new List<char>();
            List<char> quotedChars = null;

            // For each char in the word.
            foreach (char character in text)
            {
                // If is quoted char.
                if (character.Equals('"'))
                {
                    // If start was found now the end has been found.
                    if (startFound)
                        endFound = true;
                    else
                        quotedChars = new List<char>();

                    // Start has been found.
                    startFound = true;
                }

                // Start adding the chars.
                if (startFound && !endFound)
                {
                    // Add the char.
                    quotedChars.Add(character);
                }

                // If not in quotes.
                if (!startFound && !endFound)
                {
                    // Add unquoted chars.
                    unQuotedChars.Add(character);
                }

                // If start has been found and end.
                if(startFound && endFound)
                {
                    // Reset.
                    char[] inQuoteChars = quotedChars.ToArray();
                    string quoted = new string(inQuoteChars, 1, inQuoteChars.Length - 1);
                    startFound = false;
                    endFound = false;

                    // This id quoted.
                    searchData.Add(new QuotedStringModel() { Quoted = true, QuotedText = quoted });
                }
            }

            // Add the unquoted chars.
            string unquoted = new string(unQuotedChars.ToArray());
            searchData.Add(new QuotedStringModel() { Quoted = false, QuotedText = unquoted });

            // Return the quoted text.
            return searchData.ToArray();
        }

        /// <summary>
        /// Create the logical query.
        /// </summary>
        /// <param name="text">The complete text.</param>
        /// <param name="searchFieldName">The name of the field to search.</param>
        /// <returns>The query result.</returns>
        private BooleanQuery CreateLogicalQuery(string text, string searchFieldName)
        {
            BooleanQuery query = null;

            // Split by AND
            string[] add = text.Split(new string[] { "AND" }, StringSplitOptions.RemoveEmptyEntries);
            if (add != null && add.Length > 1)
            {
                // Create the query.
                query = new BooleanQuery();
                List<Query> queries = new List<Query>();

                // For each AND.
                foreach (string item in add)
                {
                    // Look for OR's
                    string[] or = item.Split(new string[] { "OR" }, StringSplitOptions.RemoveEmptyEntries);
                    if (or != null && or.Length > 1)
                    {
                        // Combine the text.
                        string combineText = String.Join(" ", or);

                        // Create the query.
                        queries.Add(CreateBoolenQuery(combineText, BooleanClause.Occur.SHOULD, searchFieldName));
                    }
                    else
                    {
                        // All AND's
                        queries.Add(CreateBoolenQuery(item, BooleanClause.Occur.SHOULD, searchFieldName));
                    }
                }

                // For each query,add to the BooleanQuery.
                foreach (Query item in queries)
                {
                    // Add the query.
                    query.Add(item, BooleanClause.Occur.MUST);
                }
            }
            else
            {
                // No AND's look for OR's
                string[] or = text.Split(new string[] { "OR" }, StringSplitOptions.RemoveEmptyEntries);
                if (or != null && or.Length > 1)
                {
                    // Combine the text.
                    string combineText = String.Join(" ", or);

                    // Create the query.
                    query = CreateBoolenQuery(combineText, BooleanClause.Occur.SHOULD, searchFieldName);
                }
            }

            // Return the query.
            return query;
        }

        /// <summary>
        /// Create the quoted query.
        /// </summary>
        /// <param name="text">The quoted text.</param>
        /// <param name="searchFieldName">The name of the field to search.</param>
        /// <returns>The query result.</returns>
        private PhraseQuery CreateQuotedQuery(string text, string searchFieldName)
        {
            // Create the query.
            PhraseQuery query = new PhraseQuery();
            query.Slop = 2;

            // Quoted search exact phase.
            string[] words = text.Words();
            for (int i = 0; i < words.Length; i++)
            {
                // Add the query.
                query.Add(new Term(searchFieldName, words[i].Trim().ToLower()));
            }

            // Return the query.
            return query;
        }

        /// <summary>
        /// Create the complete query.
        /// </summary>
        /// <param name="text">The complete text.</param>
        /// <param name="clause">The term clause value.</param>
        /// <param name="searchFieldName">The name of the field to search.</param>
        /// <returns>The query result.</returns>
        private BooleanQuery CreateBoolenQuery(string text, BooleanClause.Occur clause, string searchFieldName)
        {
            // Create the query.
            BooleanQuery query = new BooleanQuery();

            // Get quoted chars if any.
            QuotedStringModel[] quoted = FindQuotedText(text);

            // If data exists.
            if (quoted != null && quoted.Length > 0)
            {
                // For each item.
                foreach (QuotedStringModel item in quoted)
                {
                    // If is quoted text.
                    if (item.Quoted)
                    {
                        // Add the query.
                        query.Add(CreateQuotedQuery(item.QuotedText, searchFieldName), BooleanClause.Occur.MUST);
                    }
                    else
                    {
                        // Do a complete search.
                        string[] words = item.QuotedText.Words();
                        for (int i = 0; i < words.Length; i++)
                        {
                            // If a wild card char exists.
                            if (ContainsWildcard(words[i]))
                            {
                                // Add the query.
                                query.Add(new WildcardQuery(new Term(searchFieldName, words[i].Trim().ToLower())), clause);
                            }
                            else
                            {
                                // Add the query.
                                query.Add(new TermQuery(new Term(searchFieldName, words[i].Trim().ToLower())), clause);
                            }
                        }
                    }
                }
            }

            // Return the query.
            return query;
        }

        #region Dispose Object Methods

        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_reader != null)
                        _reader.Dispose();

                    if (_facetReader != null)
                        _facetReader.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _reader = null;
                _facetReader = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~SearchProvider()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
