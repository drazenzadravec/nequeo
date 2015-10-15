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
using System.Data.Services.Client;

namespace Nequeo.Net.Search.Google
{
    /// <summary>
    /// Google search service query container.
    /// </summary>
    internal class ApiContainer
    {
        /// <summary>
        /// Google api language service query container.
        /// </summary>
        /// <param name="serviceRoot">An absolute URI that identifies the root of a data service.</param>
        /// <param name="apiKey">The api key used to access this service.</param>
        public ApiContainer(Uri serviceRoot, string apiKey)
        {
            _apiKey = apiKey;
            _serviceRoot = serviceRoot;
        }

        private string _apiKey = null;
        private Uri _serviceRoot = null;

        /// <summary>
        /// Compose the query.
        /// </summary>
        /// <param name="query">Google search query Sample Values : xbox</param>
        /// <returns>The array of search results.</returns>
        public SearchResult Composite(String query)
        {
            return Composite(query, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        /// Compose the query.
        /// </summary>
        /// <param name="query">Google search query Sample Values : xbox</param>
        /// <param name="cx">The custom search engine ID to scope this search query.</param>
        /// <returns>The array of search results.</returns>
        public SearchResult Composite(String query, String cx)
        {
            if ((cx == null))
            {
                throw new System.ArgumentNullException("cx", "Cx value cannot be null");
            }

            return Composite(query, cx, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        /// Compose the query.
        /// </summary>
        /// <param name="query">Google search query Sample Values : xbox</param>
        /// <param name="cx">The custom search engine ID to scope this search query.</param>
        /// <param name="c2coff">Turns off the translation between zh-CN and zh-TW.</param>
        /// <param name="cr">Country restrict(s).</param>
        /// <param name="cref">The URL of a linked custom search engine.</param>
        /// <param name="dateRestrict">Specifies all search results are from a time period.</param>
        /// <param name="exactTerms">Identifies a phrase that all documents in the search results must contain.</param>
        /// <param name="excludeTerms">Identifies a word or phrase that should not appear in any documents in the search results.</param>
        /// <param name="fileType">Returns images of a specified type. Some of the allowed values are: bmp, gif, png, jpg, svg, pdf, ...</param>
        /// <param name="filter">Controls turning on or off the duplicate content filter.</param>
        /// <param name="gl">Geolocation of end user.</param>
        /// <param name="googlehost">The local Google domain to use to perform the search.</param>
        /// <param name="highRange">Creates a range in form as_nlo value..as_nhi value and attempts to append it to query.</param>
        /// <param name="hl">Sets the user interface language.</param>
        /// <param name="hq">Appends the extra query terms to the query.</param>
        /// <param name="imgColorType">Returns black and white, grayscale, or color images: mono, gray, and color.</param>
        /// <param name="imgDominantColor">Returns images of a specific dominant color: yellow, green, teal, blue, purple, pink, white, gray, black and brown.</param>
        /// <param name="imgSize">Returns images of a specified size, where size can be one of: icon, small, medium, large, xlarge, xxlarge, and huge.</param>
        /// <param name="imgType">Returns images of a type, which can be one of: clipart, face, lineart, news, and photo.</param>
        /// <param name="linkSite">Specifies that all search results should contain a link to a particular URL.</param>
        /// <param name="lowRange">Creates a range in form as_nlo value..as_nhi value and attempts to append it to query.</param>
        /// <param name="lr">The language restriction for the search results.</param>
        /// <param name="num">Number of search results to return.</param>
        /// <param name="orTerms">Provides additional search terms to check for in a document, where each document in the search results must contain at least one of the additional search terms.</param>
        /// <param name="relatedSite">Specifies that all search results should be pages that are related to the specified URL.</param>
        /// <param name="rights">Filters based on licensing. Supported values include: cc_publicdomain, cc_attribute, cc_sharealike, cc_noncommercial, cc_nonderived and combinations of these.</param>
        /// <param name="safe">Search safety level.</param>
        /// <param name="searchType">Specifies the search type: image.</param>
        /// <param name="siteSearch">Specifies all search results should be pages from a given site.</param>
        /// <param name="siteSearchFilter">Controls whether to include or exclude results from the site named in the as_sitesearch parameter.</param>
        /// <param name="sort">The sort expression to apply to the results.</param>
        /// <param name="start">The index of the first result to return.</param>
        /// <param name="fields">Selector specifying which fields to include in a partial response (e.g. kind,searchInformation(formattedTotalResults,searchTime),url/template).</param>
        /// <returns>The array of search results.</returns>
        public SearchResult Composite(String query, String cx, String c2coff, String cr, String cref, String dateRestrict, String exactTerms, String excludeTerms, String fileType,
            String filter, String gl, String googlehost, String highRange, String hl, String hq, String imgColorType, String imgDominantColor, String imgSize, String imgType,
            String linkSite, String lowRange, String lr, int? num, String orTerms, String relatedSite, String rights, String safe, String searchType, String siteSearch,
            String siteSearchFilter, String sort, int? start, String fields)
        {
            string queryString = "";

            if ((query == null))
            {
                throw new System.ArgumentNullException("query", "Query value cannot be null");
            }

            if ((query != null))
            {
                queryString += "&q=" + System.Uri.EscapeDataString(query);
            }
            if ((cx != null))
            {
                queryString += "&cx=" + System.Uri.EscapeDataString(cx);
            }
            if ((c2coff != null))
            {
                queryString += "&c2coff=" + System.Uri.EscapeDataString(c2coff);
            }
            if ((cr != null))
            {
                queryString += "&cr=" + System.Uri.EscapeDataString(cr);
            }
            if ((cref != null))
            {
                queryString += "&cref=" + System.Uri.EscapeDataString(cref);
            }
            if ((dateRestrict != null))
            {
                queryString += "&dateRestrict=" + System.Uri.EscapeDataString(dateRestrict);
            }
            if ((exactTerms != null))
            {
                queryString += "&exactTerms=" + System.Uri.EscapeDataString(exactTerms);
            }
            if ((excludeTerms != null))
            {
                queryString += "&excludeTerms=" + System.Uri.EscapeDataString(excludeTerms);
            }
            if ((fileType != null))
            {
                queryString += "&fileType=" + System.Uri.EscapeDataString(fileType);
            }
            if ((filter != null))
            {
                queryString += "&filter=" + System.Uri.EscapeDataString(filter);
            }
            if ((gl != null))
            {
                queryString += "&gl=" + System.Uri.EscapeDataString(gl);
            }
            if ((googlehost != null))
            {
                queryString += "&googlehost=" + System.Uri.EscapeDataString(googlehost);
            }
            if ((highRange != null))
            {
                queryString += "&highRange=" + System.Uri.EscapeDataString(highRange);
            }
            if ((hl != null))
            {
                queryString += "&hl=" + System.Uri.EscapeDataString(hl);
            }
            if ((hq != null))
            {
                queryString += "&hq=" + System.Uri.EscapeDataString(hq);
            }
            if ((imgColorType != null))
            {
                queryString += "&imgColorType=" + System.Uri.EscapeDataString(imgColorType);
            }
            if ((imgDominantColor != null))
            {
                queryString += "&imgDominantColor=" + System.Uri.EscapeDataString(imgDominantColor);
            }
            if ((imgSize != null))
            {
                queryString += "&imgSize=" + System.Uri.EscapeDataString(imgSize);
            }
            if ((imgType != null))
            {
                queryString += "&imgType=" + System.Uri.EscapeDataString(imgType);
            }
            if ((linkSite != null))
            {
                queryString += "&linkSite=" + System.Uri.EscapeDataString(linkSite);
            }
            if ((lowRange != null))
            {
                queryString += "&lowRange=" + System.Uri.EscapeDataString(lowRange);
            }
            if ((lr != null))
            {
                queryString += "&lr=" + System.Uri.EscapeDataString(lr);
            }
            if ((num != null))
            {
                queryString += "&num=" + System.Uri.EscapeDataString(num.Value.ToString());
            }
            if ((orTerms != null))
            {
                queryString += "&orTerms=" + System.Uri.EscapeDataString(orTerms);
            }
            if ((relatedSite != null))
            {
                queryString += "&relatedSite=" + System.Uri.EscapeDataString(relatedSite);
            }
            if ((rights != null))
            {
                queryString += "&rights=" + System.Uri.EscapeDataString(rights);
            }
            if ((safe != null))
            {
                queryString += "&safe=" + System.Uri.EscapeDataString(safe);
            }
            if ((searchType != null))
            {
                queryString += "&searchType=" + System.Uri.EscapeDataString(searchType);
            }
            if ((siteSearch != null))
            {
                queryString += "&siteSearch=" + System.Uri.EscapeDataString(siteSearch);
            }
            if ((siteSearchFilter != null))
            {
                queryString += "&siteSearchFilter=" + System.Uri.EscapeDataString(siteSearchFilter);
            }
            if ((sort != null))
            {
                queryString += "&sort=" + System.Uri.EscapeDataString(sort);
            }
            if ((start != null))
            {
                queryString += "&start=" + System.Uri.EscapeDataString(start.Value.ToString());
            }
            if ((fields != null))
            {
                queryString += "&fields=" + System.Uri.EscapeDataString(fields);
            }
            if ((_apiKey != null))
            {
                queryString += "&key=" + _apiKey;
            }

            // Process the request.
            return ProcessRequest<SearchResult>("", queryString);
        }

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="serviceName">The service name to call.</param>
        /// <param name="query">The query to apply.</param>
        /// <returns>The returned type.</returns>
        private T ProcessRequest<T>(string serviceName, string query)
        {
            // Construct the URI.
            Uri constructedServiceUri = new Uri(_serviceRoot.AbsoluteUri.TrimEnd(new char[] { '/' }) + "/" + serviceName + "?" + query.TrimStart(new char[] { '&' }));
            return Nequeo.Net.HttpJsonClient.Request<T>(constructedServiceUri);
        }
    }
}
