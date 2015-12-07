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
using Lucene.Net.Documents;

using Nequeo.Html;
using Nequeo.Drawing.Pdf;
using Nequeo.Search.Engine.Analyzer;
using Nequeo.Search.Engine.Filter;
using Nequeo.IO;

namespace Nequeo.Search.Engine.Filter
{
    /// <summary>
    /// Directory filter.
    /// </summary>
    internal class DirectoryFilter
    {
        /// <summary>
        /// Directory filter.
        /// </summary>
        public DirectoryFilter() { }

        /// <summary>
        /// Add documents.
        /// </summary>
        /// <param name="writer">The index writer.</param>
        /// <param name="directoryInfo">The directory information where all the files that are to be added are located.</param>
        /// <param name="documents">The supported documents search filter, used to indicate what files are to be added.</param>
        public void AddDocuments(Lucene.Net.Index.IndexWriter writer, DirectoryInfo directoryInfo, SupportedDocumentExtension documents)
        {
            Nequeo.IO.Directory directory = new Nequeo.IO.Directory();
            
            // Select the document format filter.
            // If html has been selected.
            if (documents.SupportedDocuments.HasFlag(SupportedDocuments.Html))
            {
                // Create the html filter.
                HtmlFilter htmlFilter = new HtmlFilter();
                string[] files = directory.GetFiles(directoryInfo.FullName, documents.GetFormattedSearchPatterns(SupportedDocuments.Html));
                htmlFilter.AddDocuments(writer, directoryInfo, files, documents);
            }

            // If pdf has been selected.
            if (documents.SupportedDocuments.HasFlag(SupportedDocuments.Pdf))
            {
                // Create the pdf filter.
                PdfFilter pdfFilter = new PdfFilter();
                string[] files = directory.GetFiles(directoryInfo.FullName, documents.GetFormattedSearchPatterns(SupportedDocuments.Pdf));
                pdfFilter.AddDocuments(writer, directoryInfo, files, documents);
            }

            // If rtf has been selected.
            if (documents.SupportedDocuments.HasFlag(SupportedDocuments.Rtf))
            {
                // Create the rtf filter.
                RtfFilter rtfFilter = new RtfFilter();
                string[] files = directory.GetFiles(directoryInfo.FullName, documents.GetFormattedSearchPatterns(SupportedDocuments.Rtf));
                rtfFilter.AddDocuments(writer, directoryInfo, files, documents);
            }

            // If txt has been selected.
            if (documents.SupportedDocuments.HasFlag(SupportedDocuments.Txt))
            {
                // Create the txt filter.
                TxtFilter txtFilter = new TxtFilter();
                string[] files = directory.GetFiles(directoryInfo.FullName, documents.GetFormattedSearchPatterns(SupportedDocuments.Txt));
                txtFilter.AddDocuments(writer, directoryInfo, files, documents);
            }

            // If xml has been selected.
            if (documents.SupportedDocuments.HasFlag(SupportedDocuments.Xml))
            {
                // Create the xml filter.
                XmlFilter xmlFilter = new XmlFilter();
                string[] files = directory.GetFiles(directoryInfo.FullName, documents.GetFormattedSearchPatterns(SupportedDocuments.Xml));
                xmlFilter.AddDocuments(writer, directoryInfo, files, documents);
            }

            // If docx has been selected.
            if (documents.SupportedDocuments.HasFlag(SupportedDocuments.Docx))
            {
                // Create the docx filter.
                MSDocFilter docxFilter = new MSDocFilter();
                string[] files = directory.GetFiles(directoryInfo.FullName, documents.GetFormattedSearchPatterns(SupportedDocuments.Docx));
                docxFilter.AddDocuments(writer, directoryInfo, files, documents);
            }
        }

        /// <summary>
        /// Remove documents from the existing index.
        /// </summary>
        /// <param name="directoryInfo">The top level relative directory information where all the files that are to be removed are located.</param>
        /// <param name="files">The array of all files that are to be removed relative to the directory info.</param>
        /// <param name="documents">The supported documents search filter, used to indicate what files are to be removed.</param>
        /// <returns>The array of queries that indicate which documents are to be removed.</returns>
        public Query[] RemoveDocuments(DirectoryInfo directoryInfo, string[] files, SupportedDocumentExtension documents)
        {
            List<Query> queries = new List<Query>();
            
            // Create the query for each documents that need to be removed.
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                string document = file.Replace(directoryInfo.Root.FullName, "").ToLower().Replace("\\", "/");

                // Create the query.
                BooleanQuery query = new BooleanQuery();
                query.Add(new TermQuery(new Term("path", document)), BooleanClause.Occur.MUST);

                // Add the query.
                queries.Add(query);
            }

            // Return the list of queries.
            return queries.ToArray();
        }
    }
}
