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

namespace Nequeo.Net.Search.Microsoft
{
    /// <summary>
    /// Azure data market search service query container.
    /// </summary>
    internal class AzureDataMarketContainer : Nequeo.Net.ServiceModel.DataServiceClient
    {
        /// <summary>
        /// Azure data market service query container.
        /// </summary>
        /// <param name="serviceRoot">An absolute URI that identifies the root of a data service.</param>
        public AzureDataMarketContainer(Uri serviceRoot) :
                base(serviceRoot)
        {
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
        public DataServiceQuery<ExpandableSearchResult> Composite(String Sources, String Query, String Options, String WebSearchOptions, String Market, 
            String Adult, Double? Latitude, Double? Longitude, String WebFileType, String ImageFilters, String VideoFilters, String VideoSortBy, 
            String NewsLocationOverride, String NewsCategory, String NewsSortBy, Int64? Skip)
        {
            if ((Sources == null))
            {
                throw new System.ArgumentNullException("Sources", "Sources value cannot be null");
            }
            if ((Query == null))
            {
                throw new System.ArgumentNullException("Query", "Query value cannot be null");
            }
            DataServiceQuery<ExpandableSearchResult> query;
            query = base.CreateQuery<ExpandableSearchResult>("Composite");
            if ((Sources != null))
            {
                query = query.AddQueryOption("Sources", string.Concat("\'", System.Uri.EscapeDataString(Sources), "\'"));
            }
            if ((Query != null))
            {
                query = query.AddQueryOption("Query", string.Concat("\'", System.Uri.EscapeDataString(Query), "\'"));
            }
            if ((Options != null))
            {
                query = query.AddQueryOption("Options", string.Concat("\'", System.Uri.EscapeDataString(Options), "\'"));
            }
            if ((WebSearchOptions != null))
            {
                query = query.AddQueryOption("WebSearchOptions", string.Concat("\'", System.Uri.EscapeDataString(WebSearchOptions), "\'"));
            }
            if ((Market != null))
            {
                query = query.AddQueryOption("Market", string.Concat("\'", System.Uri.EscapeDataString(Market), "\'"));
            }
            if ((Adult != null))
            {
                query = query.AddQueryOption("Adult", string.Concat("\'", System.Uri.EscapeDataString(Adult), "\'"));
            }
            if (((Latitude != null)
                        && (Latitude.HasValue == true)))
            {
                query = query.AddQueryOption("Latitude", Latitude.Value);
            }
            if (((Longitude != null)
                        && (Longitude.HasValue == true)))
            {
                query = query.AddQueryOption("Longitude", Longitude.Value);
            }
            if ((WebFileType != null))
            {
                query = query.AddQueryOption("WebFileType", string.Concat("\'", System.Uri.EscapeDataString(WebFileType), "\'"));
            }
            if ((ImageFilters != null))
            {
                query = query.AddQueryOption("ImageFilters", string.Concat("\'", System.Uri.EscapeDataString(ImageFilters), "\'"));
            }
            if ((VideoFilters != null))
            {
                query = query.AddQueryOption("VideoFilters", string.Concat("\'", System.Uri.EscapeDataString(VideoFilters), "\'"));
            }
            if ((VideoSortBy != null))
            {
                query = query.AddQueryOption("VideoSortBy", string.Concat("\'", System.Uri.EscapeDataString(VideoSortBy), "\'"));
            }
            if ((NewsLocationOverride != null))
            {
                query = query.AddQueryOption("NewsLocationOverride", string.Concat("\'", System.Uri.EscapeDataString(NewsLocationOverride), "\'"));
            }
            if ((NewsCategory != null))
            {
                query = query.AddQueryOption("NewsCategory", string.Concat("\'", System.Uri.EscapeDataString(NewsCategory), "\'"));
            }
            if ((NewsSortBy != null))
            {
                query = query.AddQueryOption("NewsSortBy", string.Concat("\'", System.Uri.EscapeDataString(NewsSortBy), "\'"));
            }
            if ((Skip != null))
            {
                query = query.AddQueryOption("$skip", Skip);
            }
            return query;
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
        public DataServiceQuery<WebResult> Web(String Query, String Options, String WebSearchOptions, String Market, 
            String Adult, Double? Latitude, Double? Longitude, String WebFileType, Int64? Skip)
        {
            if ((Query == null))
            {
                throw new System.ArgumentNullException("Query", "Query value cannot be null");
            }
            DataServiceQuery<WebResult> query;
            query = base.CreateQuery<WebResult>("Web");
            if ((Query != null))
            {
                query = query.AddQueryOption("Query", string.Concat("\'", System.Uri.EscapeDataString(Query), "\'"));
            }
            if ((Options != null))
            {
                query = query.AddQueryOption("Options", string.Concat("\'", System.Uri.EscapeDataString(Options), "\'"));
            }
            if ((WebSearchOptions != null))
            {
                query = query.AddQueryOption("WebSearchOptions", string.Concat("\'", System.Uri.EscapeDataString(WebSearchOptions), "\'"));
            }
            if ((Market != null))
            {
                query = query.AddQueryOption("Market", string.Concat("\'", System.Uri.EscapeDataString(Market), "\'"));
            }
            if ((Adult != null))
            {
                query = query.AddQueryOption("Adult", string.Concat("\'", System.Uri.EscapeDataString(Adult), "\'"));
            }
            if (((Latitude != null)
                        && (Latitude.HasValue == true)))
            {
                query = query.AddQueryOption("Latitude", Latitude.Value);
            }
            if (((Longitude != null)
                        && (Longitude.HasValue == true)))
            {
                query = query.AddQueryOption("Longitude", Longitude.Value);
            }
            if ((WebFileType != null))
            {
                query = query.AddQueryOption("WebFileType", string.Concat("\'", System.Uri.EscapeDataString(WebFileType), "\'"));
            }
            if ((Skip != null))
            {
                query = query.AddQueryOption("$skip", Skip);
            }
            return query;
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
        public DataServiceQuery<ImageResult> Image(String Query, String Options, String Market, String Adult, Double? Latitude, Double? Longitude, String ImageFilters, Int64? Skip)
        {
            if ((Query == null))
            {
                throw new System.ArgumentNullException("Query", "Query value cannot be null");
            }
            DataServiceQuery<ImageResult> query;
            query = base.CreateQuery<ImageResult>("Image");
            if ((Query != null))
            {
                query = query.AddQueryOption("Query", string.Concat("\'", System.Uri.EscapeDataString(Query), "\'"));
            }
            if ((Options != null))
            {
                query = query.AddQueryOption("Options", string.Concat("\'", System.Uri.EscapeDataString(Options), "\'"));
            }
            if ((Market != null))
            {
                query = query.AddQueryOption("Market", string.Concat("\'", System.Uri.EscapeDataString(Market), "\'"));
            }
            if ((Adult != null))
            {
                query = query.AddQueryOption("Adult", string.Concat("\'", System.Uri.EscapeDataString(Adult), "\'"));
            }
            if (((Latitude != null)
                        && (Latitude.HasValue == true)))
            {
                query = query.AddQueryOption("Latitude", Latitude.Value);
            }
            if (((Longitude != null)
                        && (Longitude.HasValue == true)))
            {
                query = query.AddQueryOption("Longitude", Longitude.Value);
            }
            if ((ImageFilters != null))
            {
                query = query.AddQueryOption("ImageFilters", string.Concat("\'", System.Uri.EscapeDataString(ImageFilters), "\'"));
            }
            if ((Skip != null))
            {
                query = query.AddQueryOption("$skip", Skip);
            }
            return query;
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
        public DataServiceQuery<VideoResult> Video(String Query, String Options, String Market, String Adult, Double? Latitude, Double? Longitude, String VideoFilters, String VideoSortBy, Int64? Skip)
        {
            if ((Query == null))
            {
                throw new System.ArgumentNullException("Query", "Query value cannot be null");
            }
            DataServiceQuery<VideoResult> query;
            query = base.CreateQuery<VideoResult>("Video");
            if ((Query != null))
            {
                query = query.AddQueryOption("Query", string.Concat("\'", System.Uri.EscapeDataString(Query), "\'"));
            }
            if ((Options != null))
            {
                query = query.AddQueryOption("Options", string.Concat("\'", System.Uri.EscapeDataString(Options), "\'"));
            }
            if ((Market != null))
            {
                query = query.AddQueryOption("Market", string.Concat("\'", System.Uri.EscapeDataString(Market), "\'"));
            }
            if ((Adult != null))
            {
                query = query.AddQueryOption("Adult", string.Concat("\'", System.Uri.EscapeDataString(Adult), "\'"));
            }
            if (((Latitude != null)
                        && (Latitude.HasValue == true)))
            {
                query = query.AddQueryOption("Latitude", Latitude.Value);
            }
            if (((Longitude != null)
                        && (Longitude.HasValue == true)))
            {
                query = query.AddQueryOption("Longitude", Longitude.Value);
            }
            if ((VideoFilters != null))
            {
                query = query.AddQueryOption("VideoFilters", string.Concat("\'", System.Uri.EscapeDataString(VideoFilters), "\'"));
            }
            if ((VideoSortBy != null))
            {
                query = query.AddQueryOption("VideoSortBy", string.Concat("\'", System.Uri.EscapeDataString(VideoSortBy), "\'"));
            }
            if ((Skip != null))
            {
                query = query.AddQueryOption("$skip", Skip);
            }
            return query;
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
        public DataServiceQuery<NewsResult> News(String Query, String Options, String Market, String Adult, Double? Latitude, Double? Longitude, 
            String NewsLocationOverride, String NewsCategory, String NewsSortBy, Int64? Skip)
        {
            if ((Query == null))
            {
                throw new System.ArgumentNullException("Query", "Query value cannot be null");
            }
            DataServiceQuery<NewsResult> query;
            query = base.CreateQuery<NewsResult>("News");
            if ((Query != null))
            {
                query = query.AddQueryOption("Query", string.Concat("\'", System.Uri.EscapeDataString(Query), "\'"));
            }
            if ((Options != null))
            {
                query = query.AddQueryOption("Options", string.Concat("\'", System.Uri.EscapeDataString(Options), "\'"));
            }
            if ((Market != null))
            {
                query = query.AddQueryOption("Market", string.Concat("\'", System.Uri.EscapeDataString(Market), "\'"));
            }
            if ((Adult != null))
            {
                query = query.AddQueryOption("Adult", string.Concat("\'", System.Uri.EscapeDataString(Adult), "\'"));
            }
            if (((Latitude != null)
                        && (Latitude.HasValue == true)))
            {
                query = query.AddQueryOption("Latitude", Latitude.Value);
            }
            if (((Longitude != null)
                        && (Longitude.HasValue == true)))
            {
                query = query.AddQueryOption("Longitude", Longitude.Value);
            }
            if ((NewsLocationOverride != null))
            {
                query = query.AddQueryOption("NewsLocationOverride", string.Concat("\'", System.Uri.EscapeDataString(NewsLocationOverride), "\'"));
            }
            if ((NewsCategory != null))
            {
                query = query.AddQueryOption("NewsCategory", string.Concat("\'", System.Uri.EscapeDataString(NewsCategory), "\'"));
            }
            if ((NewsSortBy != null))
            {
                query = query.AddQueryOption("NewsSortBy", string.Concat("\'", System.Uri.EscapeDataString(NewsSortBy), "\'"));
            }
            if ((Skip != null))
            {
                query = query.AddQueryOption("$skip", Skip);
            }
            return query;
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
        public DataServiceQuery<RelatedSearchResult> RelatedSearch(String Query, String Options, String Market, String Adult, Double? Latitude, Double? Longitude, Int64? Skip)
        {
            if ((Query == null))
            {
                throw new System.ArgumentNullException("Query", "Query value cannot be null");
            }
            DataServiceQuery<RelatedSearchResult> query;
            query = base.CreateQuery<RelatedSearchResult>("RelatedSearch");
            if ((Query != null))
            {
                query = query.AddQueryOption("Query", string.Concat("\'", System.Uri.EscapeDataString(Query), "\'"));
            }
            if ((Options != null))
            {
                query = query.AddQueryOption("Options", string.Concat("\'", System.Uri.EscapeDataString(Options), "\'"));
            }
            if ((Market != null))
            {
                query = query.AddQueryOption("Market", string.Concat("\'", System.Uri.EscapeDataString(Market), "\'"));
            }
            if ((Adult != null))
            {
                query = query.AddQueryOption("Adult", string.Concat("\'", System.Uri.EscapeDataString(Adult), "\'"));
            }
            if (((Latitude != null)
                        && (Latitude.HasValue == true)))
            {
                query = query.AddQueryOption("Latitude", Latitude.Value);
            }
            if (((Longitude != null)
                        && (Longitude.HasValue == true)))
            {
                query = query.AddQueryOption("Longitude", Longitude.Value);
            }
            if ((Skip != null))
            {
                query = query.AddQueryOption("$skip", Skip);
            }
            return query;
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
        public DataServiceQuery<SpellResult> SpellingSuggestions(String Query, String Options, String Market, String Adult, Double? Latitude, Double? Longitude, Int64? Skip)
        {
            if ((Query == null))
            {
                throw new System.ArgumentNullException("Query", "Query value cannot be null");
            }
            DataServiceQuery<SpellResult> query;
            query = base.CreateQuery<SpellResult>("SpellingSuggestions");
            if ((Query != null))
            {
                query = query.AddQueryOption("Query", string.Concat("\'", System.Uri.EscapeDataString(Query), "\'"));
            }
            if ((Options != null))
            {
                query = query.AddQueryOption("Options", string.Concat("\'", System.Uri.EscapeDataString(Options), "\'"));
            }
            if ((Market != null))
            {
                query = query.AddQueryOption("Market", string.Concat("\'", System.Uri.EscapeDataString(Market), "\'"));
            }
            if ((Adult != null))
            {
                query = query.AddQueryOption("Adult", string.Concat("\'", System.Uri.EscapeDataString(Adult), "\'"));
            }
            if (((Latitude != null)
                        && (Latitude.HasValue == true)))
            {
                query = query.AddQueryOption("Latitude", Latitude.Value);
            }
            if (((Longitude != null)
                        && (Longitude.HasValue == true)))
            {
                query = query.AddQueryOption("Longitude", Longitude.Value);
            }
            if ((Skip != null))
            {
                query = query.AddQueryOption("$skip", Skip);
            }
            return query;
        }
    }
}
