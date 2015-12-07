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

namespace Nequeo.Search.Engine
{
    /// <summary>
    /// Facet document model container.
    /// </summary>
    public class FacetDocument
    {
        /// <summary>
        /// Gets or sets the total number of hits.
        /// </summary>
        public long TotalHits { get; set; }

        /// <summary>
        /// Gets or sets the maximum score value encountered. Note that in case scores are not tracked.
        /// </summary>
        public float MaxScore { get; set; }

        /// <summary>
        /// Gets or sets the facet path result.
        /// </summary>
        public FacetPathResult[] FacetPathResults { get; set; }

        /// <summary>
        /// Gets or sets the text data results.
        /// </summary>
        public TextDataResult[] TextDataResults { get; set; }

        /// <summary>
        /// Gets or sets the file document results.
        /// </summary>
        public FileDocumentResult[] FileDocumentResults { get; set; }
    }

    /// <summary>
    /// Text data model container.
    /// </summary>
    public class TextData
    {
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        public TextDataResult[] Results { get; set; }

        /// <summary>
        /// Gets or sets the maximum score value encountered. Note that in case scores are not tracked.
        /// </summary>
        public float MaxScore { get; set; }

        /// <summary>
        /// Gets or sets the total number of hits.
        /// </summary>
        public long TotalHits { get; set; }
    }

    /// <summary>
    /// Text data model container.
    /// </summary>
    public class TextDataResult
    {
        /// <summary>
        /// Gets or set the text name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or set the text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the score of this document for the query.
        /// </summary>
        public float Score { get; set; }

        /// <summary>
        /// Gets or sets the internal document identifier.
        /// </summary>
        public int Doc { get; set; }
    }

    /// <summary>
    /// File document model container.
    /// </summary>
    public class FileDocument
    {
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        public FileDocumentResult[] Results { get; set; }

        /// <summary>
        /// Gets or sets the maximum score value encountered. Note that in case scores are not tracked.
        /// </summary>
        public float MaxScore { get; set; }

        /// <summary>
        /// Gets or sets the total number of hits.
        /// </summary>
        public long TotalHits { get; set; }
    }

    /// <summary>
    /// File document model container.
    /// </summary>
    public class FileDocumentResult
    {
        /// <summary>
        /// Gets or set the path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or set the modified data.
        /// </summary>
        public string Modified { get; set; }

        /// <summary>
        /// Gets or sets the score of this document for the query.
        /// </summary>
        public float Score { get; set; }

        /// <summary>
        /// Gets or sets the internal document identifier.
        /// </summary>
        public int Doc { get; set; }
    }
}
