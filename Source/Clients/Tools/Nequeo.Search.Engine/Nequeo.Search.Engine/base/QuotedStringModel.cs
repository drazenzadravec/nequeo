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

namespace Nequeo.Search.Engine
{
    /// <summary>
    /// Quoted string model.
    /// </summary>
    internal class QuotedStringModel
    {
        /// <summary>
        /// Gets or sets the quoted indicator.
        /// </summary>
        public bool Quoted { get; set; }

        /// <summary>
        /// Gets or sets the quoted text.
        /// </summary>
        public string QuotedText { get; set; }
    }

    /// <summary>
    /// File facet data model.
    /// </summary>
    internal class FileFacetModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryInfo">The directory information where all the files that are to be added are located.</param>
        /// <param name="documents">The supported documents search filter, used to indicate what files are to be added for the dimension and path.</param>
        public FileFacetModel(DirectoryInfo directoryInfo, SupportedDocumentExtension documents)
        {
            DirectoryInfo = directoryInfo;
            Documents = documents;
        }

        /// <summary>
        /// Gets or sets the directory information where all the files that are to be added are located.
        /// </summary>
        public DirectoryInfo DirectoryInfo { get; set; }

        /// <summary>
        /// Gets or sets the supported documents search filter, used to indicate what files are to be added for the dimension and path.
        /// </summary>
        public SupportedDocumentExtension Documents { get; set; }
    }

    /// <summary>
    /// Facet path result.
    /// </summary>
    public class FacetPathResult
    {
        /// <summary>
        /// Facet path.
        /// </summary>
        /// <param name="dimensionName">The dimension name.</param>
        /// <param name="number">The number of items found for this path.</param>
        /// <param name="path">The facet paths for the dimension.</param>
        public FacetPathResult(string dimensionName, float number, params string[] path)
        {
            DimensionName = dimensionName;
            Number = number;
            Path = path;
        }

        /// <summary>
        /// Gets or sets the dimension name.
        /// </summary>
        public string DimensionName { get; set; }

        /// <summary>
        /// Gets or sets the number of items found for this path.
        /// </summary>
        public float Number { get; set; }

        /// <summary>
        /// Gets or sets the facet paths for the dimension.
        /// </summary>
        public string[] Path { get; set; }
    }
}
