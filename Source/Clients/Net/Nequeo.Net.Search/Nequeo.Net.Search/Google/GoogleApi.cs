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
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Search.Google
{
    /// <summary>
    /// Google api search.
    /// </summary>
    public sealed class Api
    {
        /// <summary>
        /// Google api search.
        /// </summary>
        public Api()
        {
            // Gets the google translator service URI.
            _service = new Uri(Nequeo.Net.Search.Properties.Settings.Default.GoogleSearchServiceURI);
        }

        /// <summary>
        /// Google api search.
        /// </summary>
        /// <param name="service">An absolute URI that identifies the root of a data service.</param>
        public Api(Uri service)
        {
            _service = service;
        }

        private Uri _service = null;
        private NetworkCredential _credentials = null;
        private ApiContainer _container = null;

        /// <summary>
        /// Gets or sets the translator service.
        /// </summary>
        public Uri Service
        {
            get { return _service; }
            set { _service = value; }
        }

        /// <summary>
        /// Gets or sets the network credentials used to access the service (the api key for the username and password).
        /// </summary>
        public NetworkCredential Credentials
        {
            get { return _credentials; }
            set { _credentials = value; }
        }

        /// <summary>
        /// Compose the query.
        /// </summary>
        /// <param name="query">Google search query Sample Values : xbox</param>
        /// <param name="cx">The custom search engine ID to scope this search query.</param>
        /// <returns>The array of search results.</returns>
        public SearchResult Composite(String query, String cx)
        {
            Initialise();

            // Get the translation list from the service.
            var dataServiceQuery = _container.Composite(query, cx);
            return dataServiceQuery;
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
        public SearchResult Composite(String query, String cx = null, String c2coff = null, String cr = null, String cref = null, String dateRestrict = null, String exactTerms = null, 
            String excludeTerms = null, String fileType = null, String filter = null, String gl = null, String googlehost = null, String highRange = null, String hl = null, String hq = null, 
            String imgColorType = null, String imgDominantColor = null, String imgSize = null, String imgType = null, String linkSite = null, String lowRange = null, String lr = null, 
            int? num = null, String orTerms = null, String relatedSite = null, String rights = null, String safe = null, String searchType = null, String siteSearch = null,
            String siteSearchFilter = null, String sort = null, int? start = null, String fields = null)
        {
            Initialise();

            // Get the translation list from the service.
            var dataServiceQuery = _container.Composite(
                query, cx, c2coff, cr, cref, dateRestrict, exactTerms, excludeTerms, fileType, filter, gl, googlehost, highRange, hl, hq, imgColorType, imgDominantColor,
                imgSize, imgType, linkSite, lowRange, lr, num, orTerms, relatedSite, rights, safe, searchType, siteSearch, siteSearchFilter, sort, start, fields);
            return dataServiceQuery;
        }

        /// <summary>
        /// Initialise.
        /// </summary>
        private void Initialise()
        {
            // If the container needs to be translated.
            if (_container == null)
            {
                // Create the container.
                _container = new ApiContainer(_service, _credentials.UserName);
            }
        }
    }
}
