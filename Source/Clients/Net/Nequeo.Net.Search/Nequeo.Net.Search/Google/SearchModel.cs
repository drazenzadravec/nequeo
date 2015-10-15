/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Threading.Tasks;

namespace Nequeo.Net.Search.Google
{
    /// <summary>
    /// Search Result.
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string Kind { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UrlModel Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Query Queries { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Context Context { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SearchInformation SearchInformation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Item[] Items { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ErrorModel Error { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Error[] Errors { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Error
    {
        /// <summary>
        /// 
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LocationType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Location { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UrlModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Template { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Query
    {
        /// <summary>
        /// 
        /// </summary>
        public NextPage[] NextPage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Request[] Request { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NextPage
    {
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TotalResults { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SearchTerms { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string InputEncoding { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OutputEncoding { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Safe { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CX { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Request
    {
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TotalResults { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SearchTerms { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string InputEncoding { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OutputEncoding { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Safe { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CX { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Context
    {
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SearchInformation
    {
        /// <summary>
        /// 
        /// </summary>
        public double SearchTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FormattedSearchTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TotalResults { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FormattedTotalResults { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Item
    {
        /// <summary>
        /// 
        /// </summary>
        public string Kind { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string HtmlTitle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DisplayLink { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Snippet { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string HtmlSnippet { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CacheId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FormattedUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string HtmlFormattedUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PageMap PageMap { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PageMap
    {
        /// <summary>
        /// 
        /// </summary>
        public Cse_Image[] Cse_Image { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public VideoObject[] VideoObject { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Person[] Person { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Cse_Thumbnail[] Cse_Thumbnail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object[] Metatags { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ImageObject[] ImageObject { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Cse_Image
    {
        /// <summary>
        /// 
        /// </summary>
        public string Src { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class VideoObject
    {
        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Person
    {
        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Cse_Thumbnail
    {
        /// <summary>
        /// 
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Height { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Src { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ImageObject
    {
        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Height { get; set; }
    }
}