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

namespace Nequeo.Net.Search.Microsoft
{
    /// <summary>
    /// Azure data market search query.
    /// </summary>
    public sealed class AzureDataMarket
    {
        /// <summary>
        /// Azure data market search query.
        /// </summary>
        public AzureDataMarket()
        {
            // Gets the microsoft translator service URI.
            _service = new Uri(Nequeo.Net.Search.Properties.Settings.Default.MicrosoftSearchServiceURI);
        }

        /// <summary>
        /// Azure data market search query.
        /// </summary>
        /// <param name="service">An absolute URI that identifies the root of a data service.</param>
        public AzureDataMarket(Uri service)
        {
            _service = service;
        }

        private Uri _service = null;
        private NetworkCredential _credentials = null;
        private AzureDataMarketContainer _container = null;

        /// <summary>
        /// Gets or sets the translator service.
        /// </summary>
        public Uri Service
        {
            get { return _service; }
            set { _service = value; }
        }

        /// <summary>
        /// Gets or sets the network credentials used to access the service (the account key for the username and password).
        /// </summary>
        public NetworkCredential Credentials
        {
            get { return _credentials; }
            set { _credentials = value; }
        }

        /// <summary>
        /// Compose a new search.
        /// </summary>
        /// <param name="Sources">Bing search sources Sample Values : web+image+video+news+spell</param>
        /// <param name="Query">Bing search query Sample Values : xbox</param>
        /// <param name="Options">Specifies options for this request for all Sources. Valid values are: DisableLocationDetection, EnableHighlighting. Sample Values : EnableHighlighting</param>
        /// <param name="WebSearchOptions">Specify options for a request to the Web SourceType. Valid values are: DisableHostCollapsing, DisableQueryAlterations. Sample Values : DisableQueryAlterations</param>
        /// <param name="Market">Market. Note: Not all Sources support all markets. Sample Values : en-US</param>
        /// <param name="Adult">Adult setting is used for filtering sexually explicit content Sample Values : Moderate</param>
        /// <param name="Latitude">Latitude Sample Values : 47.603450</param>
        /// <param name="Longitude">Longitude Sample Values : -122.329696</param>
        /// <param name="WebFileType">File extensions to return Sample Values : XLS</param>
        /// <param name="ImageFilters">Array of strings that filter the response the API sends based on size, aspect, color, style, face or any combination thereof. Valid values are: Size:Small, Size:Medium, Size:Large, Size:Width:[Width], Size:Height:[Height], Aspect:Square, Aspect:Wide, Aspect:Tall, Color:Color, Color:Monochrome, Style:Photo, Style:Graphics, Face:Face, Face:Portrait, Face:Other. Sample Values : Size:Small+Aspect:Square</param>
        /// <param name="VideoFilters">Array of strings that filter the response the API sends based on size, aspect, color, style, face or any combination thereof. Valid values are: Duration:Short, Duration:Medium, Duration:Long, Aspect:Standard, Aspect:Widescreen, Resolution:Low, Resolution:Medium, Resolution:High. Sample Values : Duration:Short+Resolution:High</param>
        /// <param name="VideoSortBy">The sort order of results returned Sample Values : Date</param>
        /// <param name="NewsLocationOverride">Overrides Bing location detection. This parameter is only applicable in en-US market. Sample Values : US.WA</param>
        /// <param name="NewsCategory">The category of news for which to provide results Sample Values : rt_Business</param>
        /// <param name="NewsSortBy">The sort order of results returned Sample Values : Date</param>
        /// <param name="Skip">The index of the first result to return (the number to skip for next set of data).</param>
        /// <returns>The array of search results.</returns>
        public ExpandableSearchResult[] Composite(String Sources, String Query, String Options = null, String WebSearchOptions = null, 
            String Market = null, String Adult = null, Double? Latitude = null, Double? Longitude = null, String WebFileType = null, String ImageFilters = null, 
            String VideoFilters = null, String VideoSortBy = null, String NewsLocationOverride = null, String NewsCategory = null, String NewsSortBy = null, Int64? Skip = null)
        {
            Initialise();
            var result = _container.Composite(Sources, Query, Options, WebSearchOptions, Market, Adult, Latitude, Longitude, 
                WebFileType, ImageFilters, VideoFilters, VideoSortBy, NewsLocationOverride, NewsCategory, NewsSortBy, Skip);
            return result.Execute().ToArray();
        }

        /// <summary>
        /// Compose a new web search.
        /// </summary>
        /// <param name="Query">Bing search query Sample Values : xbox</param>
        /// <param name="Options">Specifies options for this request for all Sources. Valid values are: DisableLocationDetection, EnableHighlighting. Sample Values : EnableHighlighting</param>
        /// <param name="WebSearchOptions">Specify options for a request to the Web SourceType. Valid values are: DisableHostCollapsing, DisableQueryAlterations. Sample Values : DisableQueryAlterations</param>
        /// <param name="Market">Market. Note: Not all Sources support all markets. Sample Values : en-US</param>
        /// <param name="Adult">Adult setting is used for filtering sexually explicit content Sample Values : Moderate</param>
        /// <param name="Latitude">Latitude Sample Values : 47.603450</param>
        /// <param name="Longitude">Longitude Sample Values : -122.329696</param>
        /// <param name="WebFileType">File extensions to return Sample Values : XLS</param>
        /// <param name="Skip">The index of the first result to return (the number to skip for next set of data).</param>
        /// <returns>The array of search results.</returns>
        public WebResult[] Web(String Query, String Options = null, String WebSearchOptions = null, String Market = null, 
            String Adult = null, Double? Latitude = null, Double? Longitude = null, String WebFileType = null, Int64? Skip = null)
        {
            Initialise();
            var result = _container.Web(Query, Options, WebSearchOptions, Market, Adult, Latitude, Longitude, WebFileType, Skip);
            return result.Execute().ToArray();
        }

        /// <summary>
        /// Compose a new image search.
        /// </summary>
        /// <param name="Query">Bing search query Sample Values : xbox</param>
        /// <param name="Options">Specifies options for this request for all Sources. Valid values are: DisableLocationDetection, EnableHighlighting. Sample Values : EnableHighlighting</param>
        /// <param name="Market">Market. Note: Not all Sources support all markets. Sample Values : en-US</param>
        /// <param name="Adult">Adult setting is used for filtering sexually explicit content Sample Values : Moderate</param>
        /// <param name="Latitude">Latitude Sample Values : 47.603450</param>
        /// <param name="Longitude">Longitude Sample Values : -122.329696</param>
        /// <param name="ImageFilters">Array of strings that filter the response the API sends based on size, aspect, color, style, face or any combination thereof. Valid values are: Size:Small, Size:Medium, Size:Large, Size:Width:[Width], Size:Height:[Height], Aspect:Square, Aspect:Wide, Aspect:Tall, Color:Color, Color:Monochrome, Style:Photo, Style:Graphics, Face:Face, Face:Portrait, Face:Other. Sample Values : Size:Small+Aspect:Square</param>
        /// <param name="Skip">The index of the first result to return (the number to skip for next set of data).</param>
        /// <returns>The array of search results.</returns>
        public ImageResult[] Image(String Query, String Options = null, String Market = null, String Adult = null, Double? Latitude = null, 
            Double? Longitude = null, String ImageFilters = null, Int64? Skip = null)
        {
            Initialise();
            var result = _container.Image(Query, Options, Market, Adult, Latitude, Longitude, ImageFilters, Skip);
            return result.Execute().ToArray();
        }

        /// <summary>
        /// Compose a new video search.
        /// </summary>
        /// <param name="Query">Bing search query Sample Values : xbox</param>
        /// <param name="Options">Specifies options for this request for all Sources. Valid values are: DisableLocationDetection, EnableHighlighting. Sample Values : EnableHighlighting</param>
        /// <param name="Market">Market. Note: Not all Sources support all markets. Sample Values : en-US</param>
        /// <param name="Adult">Adult setting is used for filtering sexually explicit content Sample Values : Moderate</param>
        /// <param name="Latitude">Latitude Sample Values : 47.603450</param>
        /// <param name="Longitude">Longitude Sample Values : -122.329696</param>
        /// <param name="VideoFilters">Array of strings that filter the response the API sends based on size, aspect, color, style, face or any combination thereof. Valid values are: Duration:Short, Duration:Medium, Duration:Long, Aspect:Standard, Aspect:Widescreen, Resolution:Low, Resolution:Medium, Resolution:High. Sample Values : Duration:Short+Resolution:High</param>
        /// <param name="VideoSortBy">The sort order of results returned Sample Values : Date</param>
        /// <param name="Skip">The index of the first result to return (the number to skip for next set of data).</param>
        /// <returns>The array of search results.</returns>
        public VideoResult[] Video(String Query, String Options = null, String Market = null, String Adult = null, Double? Latitude = null, Double? Longitude = null, 
            String VideoFilters = null, String VideoSortBy = null, Int64? Skip = null)
        {
            Initialise();
            var result = _container.Video(Query, Options, Market, Adult, Latitude, Longitude, VideoFilters, VideoSortBy, Skip);
            return result.Execute().ToArray();
        }

        /// <summary>
        /// Compose a new news search.
        /// </summary>
        /// <param name="Query">Bing search query Sample Values : xbox</param>
        /// <param name="Options">Specifies options for this request for all Sources. Valid values are: DisableLocationDetection, EnableHighlighting. Sample Values : EnableHighlighting</param>
        /// <param name="Market">Market. Note: Not all Sources support all markets. Sample Values : en-US</param>
        /// <param name="Adult">Adult setting is used for filtering sexually explicit content Sample Values : Moderate</param>
        /// <param name="Latitude">Latitude Sample Values : 47.603450</param>
        /// <param name="Longitude">Longitude Sample Values : -122.329696</param>
        /// <param name="NewsLocationOverride">Overrides Bing location detection. This parameter is only applicable in en-US market. Sample Values : US.WA</param>
        /// <param name="NewsCategory">The category of news for which to provide results Sample Values : rt_Business</param>
        /// <param name="NewsSortBy">The sort order of results returned Sample Values : Date</param>
        /// <param name="Skip">The index of the first result to return (the number to skip for next set of data).</param>
        /// <returns>The array of search results.</returns>
        public NewsResult[] News(String Query, String Options = null, String Market = null, String Adult = null, Double? Latitude = null, Double? Longitude = null, 
            String NewsLocationOverride = null, String NewsCategory = null, String NewsSortBy = null, Int64? Skip = null)
        {
            Initialise();
            var result = _container.News(Query, Options, Market, Adult, Latitude, Longitude, NewsLocationOverride, NewsCategory, NewsSortBy, Skip);
            return result.Execute().ToArray();
        }

        /// <summary>
        /// Compose a new related search.
        /// </summary>
        /// <param name="Query">Bing search query Sample Values : xbox</param>
        /// <param name="Options">Specifies options for this request for all Sources. Valid values are: DisableLocationDetection, EnableHighlighting. Sample Values : EnableHighlighting</param>
        /// <param name="Market">Market. Note: Not all Sources support all markets. Sample Values : en-US</param>
        /// <param name="Adult">Adult setting is used for filtering sexually explicit content Sample Values : Moderate</param>
        /// <param name="Latitude">Latitude Sample Values : 47.603450</param>
        /// <param name="Longitude">Longitude Sample Values : -122.329696</param>
        /// <param name="Skip">The index of the first result to return (the number to skip for next set of data).</param>
        /// <returns>The array of search results.</returns>
        public RelatedSearchResult[] RelatedSearch(String Query, String Options = null, String Market = null, 
            String Adult = null, Double? Latitude = null, Double? Longitude = null, Int64? Skip = null)
        {
            Initialise();
            var result = _container.RelatedSearch(Query, Options, Market, Adult, Latitude, Longitude, Skip);
            return result.Execute().ToArray();
        }

        /// <summary>
        /// Compose a new spelling suggestions search.
        /// </summary>
        /// <param name="Query">Bing search query Sample Values : xblox</param>
        /// <param name="Options">Specifies options for this request for all Sources. Valid values are: DisableLocationDetection, EnableHighlighting. Sample Values : EnableHighlighting</param>
        /// <param name="Market">Market. Note: Not all Sources support all markets. Sample Values : en-US</param>
        /// <param name="Adult">Adult setting is used for filtering sexually explicit content Sample Values : Moderate</param>
        /// <param name="Latitude">Latitude Sample Values : 47.603450</param>
        /// <param name="Longitude">Longitude Sample Values : -122.329696</param>
        /// <param name="Skip">The index of the first result to return (the number to skip for next set of data).</param>
        /// <returns>The array of search results.</returns>
        public SpellResult[] SpellingSuggestions(String Query, String Options = null, String Market = null, String Adult = null, 
            Double? Latitude = null, Double? Longitude = null, Int64? Skip = null)
        {
            Initialise();
            var result = _container.SpellingSuggestions(Query, Options, Market, Adult, Latitude, Longitude, Skip);
            return result.Execute().ToArray();
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
                _container = new AzureDataMarketContainer(_service);
                _container.Credentials = _credentials;
            }
        }
    }
}
