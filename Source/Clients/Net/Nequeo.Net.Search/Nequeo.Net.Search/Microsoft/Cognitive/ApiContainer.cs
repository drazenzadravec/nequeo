/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
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

namespace Nequeo.Net.Search.Microsoft.Cognitive
{
    /// <summary>
    /// Microsoft search service query container.
    /// </summary>
    internal class ApiContainer
    {
        /// <summary>
        /// Microsoft api search service query container.
        /// </summary>
        /// <param name="serviceRoot">An absolute URI that identifies the root of a data service.</param>
        /// <param name="apiKey">The api key used to access this service.</param>
        /// <param name="clientIP">The client IP address sending the request.</param>
        /// <param name="clientID">The client ID sending the request.</param>
        /// <param name="userAgent">The request user agent</param>
        public ApiContainer(Uri serviceRoot, string apiKey, string clientIP = null, string clientID = null, string userAgent = null)
        {
            _apiKey = apiKey;
            _serviceRoot = serviceRoot;
            _clientIP = clientIP;
            _clientID = clientID;
            _userAgent = userAgent;
        }

        private string _apiKey = null;
        private Uri _serviceRoot = null;
        private string _clientIP = null;
        private string _clientID = null;
        private string _userAgent = null;

        /// <summary>
        /// Web search.
        /// </summary>
        /// <param name="query">The user's search query string.</param>
        /// <param name="count">The number of search results to return in the response. The actual number delivered may be less than requested.</param>
        /// <param name="skip">The zero-based offset that indicates the number of search results to skip before returning results.</param>
        /// <param name="mkt">
        /// The market where the results come from. Typically, this is the country where the user is making the request from; however, it could be a different country if the user is not located in a country where Bing delivers results. The market must be in the form {language code}-{country code}. For example, en-US. 
        /// Full list of supported markets: 
        /// es-AR,en-AU,de-AT,nl-BE,fr-BE,pt-BR,en-CA,fr-CA,es-CL,da-DK,fi-FI,fr-FR,de-DE,zh-HK,en-IN,en-ID,en-IE,it-IT,ja-JP,ko-KR,en-MY,es-MX,nl-NL,en-NZ,no-NO,zh-CN,pl-PL,pt-PT,en-PH,ru-RU,ar-SA,en-ZA,es-ES,sv-SE,fr-CH,de-CH,zh-TW,tr-TR,en-GB,en-US,es-US
        /// </param>
        /// <param name="safeSearch">A filter used to filter results for adult content.
        /// Moderate, Strict, Off
        /// </param>
        /// <param name="responseFilter">
        /// A comma-delimited list of answers to include in the response. If you do not specify this parameter, the response will include all search answers for which there's data. 
        /// The following are some of the possible filter values.If the API that you want to filter is not listed below, refer to the API's reference documentation for the filter value. 
        /// Computation, Images, News, RelatedSearches, SpellSuggestions, TimeZone, Videos, Webpages</param>
        /// <param name="textDecorations">A Boolean value that determines whether query terms in the response strings will include hit highlighting characters. If true, the strings will include highlighting characters; otherwise, false. The default is false.</param>
        /// <param name="textFormat">The type of formatting to apply to displayable strings.
        /// For hit highlighting(see textDecorations), the format determines the highlighting characters to use in the response strings.The following are the possible values.
        /// Raw—Use special Unicode characters, E000 and E001, to highlight the query terms.For information about processing strings with the embedded Unicode characters, see Hit Highlighting.
        /// HTML—Use HTML tags to highlight the query terms.
        /// The default is Raw.
        /// To enable or disable hit highlighting, set the textDecorations query parameter.
        /// For displayable strings that contain escapable HTML characters such as &lt;, &gt;, if textFormat is set to HTML, the characters will be escaped as appropriate (for example, will be escaped to &lt;). If you do not require hit highlighting, you may specify textFormat without specifying textDecorations.
        /// </param>
        /// <param name="setLang">The language to use for user interface strings. Specify the language using the ISO 639-1 2-letter language code. For example, the language code for English is EN. The default is EN (English).</param>
        /// <param name="freshness">Filter search results by age. Age refers to the date and time that the webpage was discovered by Bing. The following are the possible filter values.
        /// Day — Return webpages discovered within the last 24 hours
        /// Week — Return webpages discovered within the last 7 days
        /// Month — Return webpages discovered within the last 30 days
        /// </param>
        /// <param name="countryCode">A 2-character country code of the country where the results come from. For a list of possible values, see Market Codes.
        /// <seealso cref="https://msdn.microsoft.com/en-us/library/dn783426.aspx"/>
        /// </param>
        /// <returns>The search result.</returns>
        public byte[] Web(string query, long? count, long? skip, string mkt, string safeSearch, string responseFilter, bool? textDecorations, string textFormat, string setLang, string freshness, string countryCode)
        {
            string queryString = "";

            if ((query == null))
            {
                throw new System.ArgumentNullException(nameof(query), "Query value cannot be null.");
            }

            if ((query != null))
            {
                queryString += "&q=" + System.Uri.EscapeDataString(query);
            }
            if ((count != null))
            {
                queryString += "&count=" + System.Uri.EscapeDataString(count.Value.ToString());
            }
            if ((skip != null))
            {
                queryString += "&offset=" + System.Uri.EscapeDataString(skip.Value.ToString());
            }
            if ((mkt != null))
            {
                queryString += "&mkt=" + System.Uri.EscapeDataString(mkt);
            }
            if ((safeSearch != null))
            {
                queryString += "&safeSearch=" + System.Uri.EscapeDataString(safeSearch);
            }
            if ((responseFilter != null))
            {
                queryString += "&responseFilter=" + System.Uri.EscapeDataString(responseFilter);
            }
            if ((textDecorations != null))
            {
                queryString += "&textDecorations=" + System.Uri.EscapeDataString(textDecorations.Value.ToString());
            }
            if ((textFormat != null))
            {
                queryString += "&textFormat=" + System.Uri.EscapeDataString(textFormat);
            }
            if ((setLang != null))
            {
                queryString += "&setLang=" + System.Uri.EscapeDataString(setLang);
            }
            if ((freshness != null))
            {
                queryString += "&freshness=" + System.Uri.EscapeDataString(freshness);
            }
            if ((countryCode != null))
            {
                queryString += "&cc=" + System.Uri.EscapeDataString(countryCode);
            }

            // Return the result.
            return ProcessRequest("", queryString);
        }

        /// <summary>
        /// Web search.
        /// </summary>
        /// <param name="query">The user's search query string.</param>
        /// <param name="count">The number of search results to return in the response. The actual number delivered may be less than requested.</param>
        /// <param name="skip">The zero-based offset that indicates the number of search results to skip before returning results.</param>
        /// <param name="mkt">
        /// The market where the results come from. Typically, this is the country where the user is making the request from; however, it could be a different country if the user is not located in a country where Bing delivers results. The market must be in the form {language code}-{country code}. For example, en-US. 
        /// Full list of supported markets: 
        /// es-AR,en-AU,de-AT,nl-BE,fr-BE,pt-BR,en-CA,fr-CA,es-CL,da-DK,fi-FI,fr-FR,de-DE,zh-HK,en-IN,en-ID,en-IE,it-IT,ja-JP,ko-KR,en-MY,es-MX,nl-NL,en-NZ,no-NO,zh-CN,pl-PL,pt-PT,en-PH,ru-RU,ar-SA,en-ZA,es-ES,sv-SE,fr-CH,de-CH,zh-TW,tr-TR,en-GB,en-US,es-US
        /// </param>
        /// <param name="safeSearch">A filter used to filter results for adult content.
        /// Moderate, Strict, Off
        /// </param>
        /// <param name="responseFilter">
        /// A comma-delimited list of answers to include in the response. If you do not specify this parameter, the response will include all search answers for which there's data. 
        /// The following are some of the possible filter values.If the API that you want to filter is not listed below, refer to the API's reference documentation for the filter value. 
        /// Computation, Images, News, RelatedSearches, SpellSuggestions, TimeZone, Videos, Webpages</param>
        /// <param name="textDecorations">A Boolean value that determines whether query terms in the response strings will include hit highlighting characters. If true, the strings will include highlighting characters; otherwise, false. The default is false.</param>
        /// <param name="textFormat">The type of formatting to apply to displayable strings.
        /// For hit highlighting(see textDecorations), the format determines the highlighting characters to use in the response strings.The following are the possible values.
        /// Raw—Use special Unicode characters, E000 and E001, to highlight the query terms.For information about processing strings with the embedded Unicode characters, see Hit Highlighting.
        /// HTML—Use HTML tags to highlight the query terms.
        /// The default is Raw.
        /// To enable or disable hit highlighting, set the textDecorations query parameter.
        /// For displayable strings that contain escapable HTML characters such as &lt;, &gt;, if textFormat is set to HTML, the characters will be escaped as appropriate (for example, will be escaped to &lt;). If you do not require hit highlighting, you may specify textFormat without specifying textDecorations.
        /// </param>
        /// <param name="setLang">The language to use for user interface strings. Specify the language using the ISO 639-1 2-letter language code. For example, the language code for English is EN. The default is EN (English).</param>
        /// <param name="freshness">Filter search results by age. Age refers to the date and time that the webpage was discovered by Bing. The following are the possible filter values.
        /// Day — Return webpages discovered within the last 24 hours
        /// Week — Return webpages discovered within the last 7 days
        /// Month — Return webpages discovered within the last 30 days
        /// </param>
        /// <param name="countryCode">A 2-character country code of the country where the results come from. For a list of possible values, see Market Codes.
        /// <seealso cref="https://msdn.microsoft.com/en-us/library/dn783426.aspx"/>
        /// </param>
        /// <returns>The search result.</returns>
        public Web.CognitiveSearchResults WebEx(string query, long? count, long? skip, string mkt, string safeSearch, string responseFilter, bool? textDecorations, string textFormat, string setLang, string freshness, string countryCode)
        {
            string queryString = "";

            if ((query == null))
            {
                throw new System.ArgumentNullException(nameof(query), "Query value cannot be null.");
            }

            if ((query != null))
            {
                queryString += "&q=" + System.Uri.EscapeDataString(query);
            }
            if ((count != null))
            {
                queryString += "&count=" + System.Uri.EscapeDataString(count.Value.ToString());
            }
            if ((skip != null))
            {
                queryString += "&offset=" + System.Uri.EscapeDataString(skip.Value.ToString());
            }
            if ((mkt != null))
            {
                queryString += "&mkt=" + System.Uri.EscapeDataString(mkt);
            }
            if ((safeSearch != null))
            {
                queryString += "&safeSearch=" + System.Uri.EscapeDataString(safeSearch);
            }
            if ((responseFilter != null))
            {
                queryString += "&responseFilter=" + System.Uri.EscapeDataString(responseFilter);
            }
            if ((textDecorations != null))
            {
                queryString += "&textDecorations=" + System.Uri.EscapeDataString(textDecorations.Value.ToString());
            }
            if ((textFormat != null))
            {
                queryString += "&textFormat=" + System.Uri.EscapeDataString(textFormat);
            }
            if ((setLang != null))
            {
                queryString += "&setLang=" + System.Uri.EscapeDataString(setLang);
            }
            if ((freshness != null))
            {
                queryString += "&freshness=" + System.Uri.EscapeDataString(freshness);
            }
            if ((countryCode != null))
            {
                queryString += "&cc=" + System.Uri.EscapeDataString(countryCode);
            }

            // Return the result.
            return ProcessRequest<Web.CognitiveSearchResults>("", queryString);
        }

        /// <summary>
        /// Video search.
        /// </summary>
        /// <param name="query">The user's search query string.</param>
        /// <param name="count">The number of search results to return in the response. The actual number delivered may be less than requested.</param>
        /// <param name="skip">The zero-based offset that indicates the number of search results to skip before returning results.</param>
        /// <param name="mkt">
        /// The market where the results come from. Typically, this is the country where the user is making the request from; however, it could be a different country if the user is not located in a country where Bing delivers results. The market must be in the form {language code}-{country code}. For example, en-US. 
        /// Full list of supported markets: 
        /// es-AR,en-AU,de-AT,nl-BE,fr-BE,pt-BR,en-CA,fr-CA,es-CL,da-DK,fi-FI,fr-FR,de-DE,zh-HK,en-IN,en-ID,en-IE,it-IT,ja-JP,ko-KR,en-MY,es-MX,nl-NL,en-NZ,no-NO,zh-CN,pl-PL,pt-PT,en-PH,ru-RU,ar-SA,en-ZA,es-ES,sv-SE,fr-CH,de-CH,zh-TW,tr-TR,en-GB,en-US,es-US
        /// </param>
        /// <param name="safeSearch">A filter used to filter results for adult content.
        /// Moderate, Strict, Off
        /// </param>
        /// <param name="textDecorations">A Boolean value that determines whether query terms in the response strings will include hit highlighting characters. If true, the strings will include highlighting characters; otherwise, false. The default is false.</param>
        /// <param name="textFormat">The type of formatting to apply to displayable strings.
        /// For hit highlighting(see textDecorations), the format determines the highlighting characters to use in the response strings.The following are the possible values.
        /// Raw—Use special Unicode characters, E000 and E001, to highlight the query terms.For information about processing strings with the embedded Unicode characters, see Hit Highlighting.
        /// HTML—Use HTML tags to highlight the query terms.
        /// The default is Raw.
        /// To enable or disable hit highlighting, set the textDecorations query parameter.
        /// For displayable strings that contain escapable HTML characters such as &lt;, &gt;, if textFormat is set to HTML, the characters will be escaped as appropriate (for example, will be escaped to &lt;). If you do not require hit highlighting, you may specify textFormat without specifying textDecorations.
        /// </param>
        /// <param name="setLang">The language to use for user interface strings. Specify the language using the ISO 639-1 2-letter language code. For example, the language code for English is EN. The default is EN (English).</param>
        /// <param name="freshness">Filter search results by age. Age refers to the date and time that the webpage was discovered by Bing. The following are the possible filter values.
        /// Day — Return webpages discovered within the last 24 hours
        /// Week — Return webpages discovered within the last 7 days
        /// Month — Return webpages discovered within the last 30 days
        /// </param>
        /// <param name="countryCode">A 2-character country code of the country where the results come from. For a list of possible values, see Market Codes.
        /// <seealso cref="https://msdn.microsoft.com/en-us/library/dn783426.aspx"/>
        /// </param>
        /// <param name="id">An ID that uniquely identifies a video. The Video object's videoId field contains the ID that you set this parameter to. 
        /// For the /videos/search endpoint, you can use this parameter to ensure that the specified video is the first video in the videos list.</param>
        /// <param name="modulesRequested">A comma-delimited list of one or more insights to request. (Note that when you URL encode the query string, the commas will change to %2C.) The following are the possible case-insensitive values.
        /// All—Return all insights, if available.
        /// RelatedVideos—Return a list of videos that are similar to the video specified by the id query parameter.
        /// VideoResult—Return the video that you're requesting insights of (this is the video that you set the id query parameter to in your insights request).</param>
        /// <returns>The search result.</returns>
        public byte[] Video(string query, long? count, long? skip, string mkt, string safeSearch, bool? textDecorations, string textFormat, string setLang, string freshness, string countryCode, string id, string modulesRequested)
        {
            string queryString = "";

            if ((query == null))
            {
                throw new System.ArgumentNullException(nameof(query), "Query value cannot be null.");
            }

            if ((query != null))
            {
                queryString += "&q=" + System.Uri.EscapeDataString(query);
            }
            if ((count != null))
            {
                queryString += "&count=" + System.Uri.EscapeDataString(count.Value.ToString());
            }
            if ((skip != null))
            {
                queryString += "&offset=" + System.Uri.EscapeDataString(skip.Value.ToString());
            }
            if ((mkt != null))
            {
                queryString += "&mkt=" + System.Uri.EscapeDataString(mkt);
            }
            if ((safeSearch != null))
            {
                queryString += "&safeSearch=" + System.Uri.EscapeDataString(safeSearch);
            }
            if ((textDecorations != null))
            {
                queryString += "&textDecorations=" + System.Uri.EscapeDataString(textDecorations.Value.ToString());
            }
            if ((textFormat != null))
            {
                queryString += "&textFormat=" + System.Uri.EscapeDataString(textFormat);
            }
            if ((setLang != null))
            {
                queryString += "&setLang=" + System.Uri.EscapeDataString(setLang);
            }
            if ((freshness != null))
            {
                queryString += "&freshness=" + System.Uri.EscapeDataString(freshness);
            }
            if ((countryCode != null))
            {
                queryString += "&cc=" + System.Uri.EscapeDataString(countryCode);
            }
            if ((id != null))
            {
                queryString += "&id=" + System.Uri.EscapeDataString(id);
            }
            if ((modulesRequested != null))
            {
                queryString += "&modulesRequested=" + System.Uri.EscapeDataString(modulesRequested);
            }

            // Return the result.
            return ProcessRequest("", queryString);
        }

        /// <summary>
        /// Video search.
        /// </summary>
        /// <param name="query">The user's search query string.</param>
        /// <param name="count">The number of search results to return in the response. The actual number delivered may be less than requested.</param>
        /// <param name="skip">The zero-based offset that indicates the number of search results to skip before returning results.</param>
        /// <param name="mkt">
        /// The market where the results come from. Typically, this is the country where the user is making the request from; however, it could be a different country if the user is not located in a country where Bing delivers results. The market must be in the form {language code}-{country code}. For example, en-US. 
        /// Full list of supported markets: 
        /// es-AR,en-AU,de-AT,nl-BE,fr-BE,pt-BR,en-CA,fr-CA,es-CL,da-DK,fi-FI,fr-FR,de-DE,zh-HK,en-IN,en-ID,en-IE,it-IT,ja-JP,ko-KR,en-MY,es-MX,nl-NL,en-NZ,no-NO,zh-CN,pl-PL,pt-PT,en-PH,ru-RU,ar-SA,en-ZA,es-ES,sv-SE,fr-CH,de-CH,zh-TW,tr-TR,en-GB,en-US,es-US
        /// </param>
        /// <param name="safeSearch">A filter used to filter results for adult content.
        /// Moderate, Strict, Off
        /// </param>
        /// <param name="textDecorations">A Boolean value that determines whether query terms in the response strings will include hit highlighting characters. If true, the strings will include highlighting characters; otherwise, false. The default is false.</param>
        /// <param name="textFormat">The type of formatting to apply to displayable strings.
        /// For hit highlighting(see textDecorations), the format determines the highlighting characters to use in the response strings.The following are the possible values.
        /// Raw—Use special Unicode characters, E000 and E001, to highlight the query terms.For information about processing strings with the embedded Unicode characters, see Hit Highlighting.
        /// HTML—Use HTML tags to highlight the query terms.
        /// The default is Raw.
        /// To enable or disable hit highlighting, set the textDecorations query parameter.
        /// For displayable strings that contain escapable HTML characters such as &lt;, &gt;, if textFormat is set to HTML, the characters will be escaped as appropriate (for example, will be escaped to &lt;). If you do not require hit highlighting, you may specify textFormat without specifying textDecorations.
        /// </param>
        /// <param name="setLang">The language to use for user interface strings. Specify the language using the ISO 639-1 2-letter language code. For example, the language code for English is EN. The default is EN (English).</param>
        /// <param name="freshness">Filter search results by age. Age refers to the date and time that the webpage was discovered by Bing. The following are the possible filter values.
        /// Day — Return webpages discovered within the last 24 hours
        /// Week — Return webpages discovered within the last 7 days
        /// Month — Return webpages discovered within the last 30 days
        /// </param>
        /// <param name="countryCode">A 2-character country code of the country where the results come from. For a list of possible values, see Market Codes.
        /// <seealso cref="https://msdn.microsoft.com/en-us/library/dn783426.aspx"/>
        /// </param>
        /// <param name="id">An ID that uniquely identifies a video. The Video object's videoId field contains the ID that you set this parameter to. 
        /// For the /videos/search endpoint, you can use this parameter to ensure that the specified video is the first video in the videos list.</param>
        /// <param name="modulesRequested">A comma-delimited list of one or more insights to request. (Note that when you URL encode the query string, the commas will change to %2C.) The following are the possible case-insensitive values.
        /// All—Return all insights, if available.
        /// RelatedVideos—Return a list of videos that are similar to the video specified by the id query parameter.
        /// VideoResult—Return the video that you're requesting insights of (this is the video that you set the id query parameter to in your insights request).</param>
        /// <returns>The search result.</returns>
        public Video.CognitiveVideoSearchResults VideoEx(string query, long? count, long? skip, string mkt, string safeSearch, bool? textDecorations, string textFormat, string setLang, string freshness, string countryCode, string id, string modulesRequested)
        {
            string queryString = "";

            if ((query == null))
            {
                throw new System.ArgumentNullException(nameof(query), "Query value cannot be null.");
            }

            if ((query != null))
            {
                queryString += "&q=" + System.Uri.EscapeDataString(query);
            }
            if ((count != null))
            {
                queryString += "&count=" + System.Uri.EscapeDataString(count.Value.ToString());
            }
            if ((skip != null))
            {
                queryString += "&offset=" + System.Uri.EscapeDataString(skip.Value.ToString());
            }
            if ((mkt != null))
            {
                queryString += "&mkt=" + System.Uri.EscapeDataString(mkt);
            }
            if ((safeSearch != null))
            {
                queryString += "&safeSearch=" + System.Uri.EscapeDataString(safeSearch);
            }
            if ((textDecorations != null))
            {
                queryString += "&textDecorations=" + System.Uri.EscapeDataString(textDecorations.Value.ToString());
            }
            if ((textFormat != null))
            {
                queryString += "&textFormat=" + System.Uri.EscapeDataString(textFormat);
            }
            if ((setLang != null))
            {
                queryString += "&setLang=" + System.Uri.EscapeDataString(setLang);
            }
            if ((freshness != null))
            {
                queryString += "&freshness=" + System.Uri.EscapeDataString(freshness);
            }
            if ((countryCode != null))
            {
                queryString += "&cc=" + System.Uri.EscapeDataString(countryCode);
            }
            if ((id != null))
            {
                queryString += "&id=" + System.Uri.EscapeDataString(id);
            }
            if ((modulesRequested != null))
            {
                queryString += "&modulesRequested=" + System.Uri.EscapeDataString(modulesRequested);
            }

            // Return the result.
            return ProcessRequest<Video.CognitiveVideoSearchResults>("", queryString);
        }

        /// <summary>
        /// News search.
        /// </summary>
        /// <param name="query">The user's search query string.</param>
        /// <param name="count">The number of search results to return in the response. The actual number delivered may be less than requested.</param>
        /// <param name="skip">The zero-based offset that indicates the number of search results to skip before returning results.</param>
        /// <param name="mkt">
        /// The market where the results come from. Typically, this is the country where the user is making the request from; however, it could be a different country if the user is not located in a country where Bing delivers results. The market must be in the form {language code}-{country code}. For example, en-US. 
        /// Full list of supported markets: 
        /// es-AR,en-AU,de-AT,nl-BE,fr-BE,pt-BR,en-CA,fr-CA,es-CL,da-DK,fi-FI,fr-FR,de-DE,zh-HK,en-IN,en-ID,en-IE,it-IT,ja-JP,ko-KR,en-MY,es-MX,nl-NL,en-NZ,no-NO,zh-CN,pl-PL,pt-PT,en-PH,ru-RU,ar-SA,en-ZA,es-ES,sv-SE,fr-CH,de-CH,zh-TW,tr-TR,en-GB,en-US,es-US
        /// </param>
        /// <param name="safeSearch">A filter used to filter results for adult content.
        /// Moderate, Strict, Off
        /// </param>
        /// <param name="textDecorations">A Boolean value that determines whether query terms in the response strings will include hit highlighting characters. If true, the strings will include highlighting characters; otherwise, false. The default is false.</param>
        /// <param name="textFormat">The type of formatting to apply to displayable strings.
        /// For hit highlighting(see textDecorations), the format determines the highlighting characters to use in the response strings.The following are the possible values.
        /// Raw—Use special Unicode characters, E000 and E001, to highlight the query terms.For information about processing strings with the embedded Unicode characters, see Hit Highlighting.
        /// HTML—Use HTML tags to highlight the query terms.
        /// The default is Raw.
        /// To enable or disable hit highlighting, set the textDecorations query parameter.
        /// For displayable strings that contain escapable HTML characters such as &lt;, &gt;, if textFormat is set to HTML, the characters will be escaped as appropriate (for example, will be escaped to &lt;). If you do not require hit highlighting, you may specify textFormat without specifying textDecorations.
        /// </param>
        /// <param name="setLang">The language to use for user interface strings. Specify the language using the ISO 639-1 2-letter language code. For example, the language code for English is EN. The default is EN (English).</param>
        /// <param name="freshness">Filter search results by age. Age refers to the date and time that the webpage was discovered by Bing. The following are the possible filter values.
        /// Day — Return webpages discovered within the last 24 hours
        /// Week — Return webpages discovered within the last 7 days
        /// Month — Return webpages discovered within the last 30 days
        /// </param>
        /// <param name="countryCode">A 2-character country code of the country where the results come from. For a list of possible values, see Market Codes.
        /// <see cref="https://msdn.microsoft.com/en-us/library/dn783426.aspx"/>
        /// </param>
        /// <param name="category">The category of articles to return. For example, Sports articles or Entertainment articles. 
        /// <seealso cref="https://msdn.microsoft.com/en-us/library/dn760793.aspx#categoriesbymarket"/>
        /// </param>
        /// <param name="headlineCount">The number of headline articles and clusters to return. The default is 12. 
        /// Specify this parameter only if you do not specify the category parameter; this parameter is ignored if you specify the category parameter.
        /// </param>
        /// <param name="originalImg">A Boolean value that determines whether the image's contentUrl contains a URL that points to a thumbnail of the original article's image or the image itself. 
        /// If the article includes an image and this parameter is set to true, the image's contentUrl property will contain a URL that you may use to download the original image from the publisher's website. Otherwise, if this parameter is false, the image's contentUrl and thumbnailUrl URLs will both point to the same thumbnail image. 
        /// The default is false.
        /// </param>
        /// <returns>The search result.</returns>
        public byte[] News(string query, long? count, long? skip, string mkt, string safeSearch, bool? textDecorations, string textFormat, string setLang, string freshness, string countryCode, string category, long? headlineCount, bool? originalImg)
        {
            string queryString = "";

            if ((query == null))
            {
                throw new System.ArgumentNullException(nameof(query), "Query value cannot be null.");
            }

            if ((query != null))
            {
                queryString += "&q=" + System.Uri.EscapeDataString(query);
            }
            if ((count != null))
            {
                queryString += "&count=" + System.Uri.EscapeDataString(count.Value.ToString());
            }
            if ((skip != null))
            {
                queryString += "&offset=" + System.Uri.EscapeDataString(skip.Value.ToString());
            }
            if ((mkt != null))
            {
                queryString += "&mkt=" + System.Uri.EscapeDataString(mkt);
            }
            if ((safeSearch != null))
            {
                queryString += "&safeSearch=" + System.Uri.EscapeDataString(safeSearch);
            }
            if ((textDecorations != null))
            {
                queryString += "&textDecorations=" + System.Uri.EscapeDataString(textDecorations.Value.ToString());
            }
            if ((textFormat != null))
            {
                queryString += "&textFormat=" + System.Uri.EscapeDataString(textFormat);
            }
            if ((setLang != null))
            {
                queryString += "&setLang=" + System.Uri.EscapeDataString(setLang);
            }
            if ((freshness != null))
            {
                queryString += "&freshness=" + System.Uri.EscapeDataString(freshness);
            }
            if ((countryCode != null))
            {
                queryString += "&cc=" + System.Uri.EscapeDataString(countryCode);
            }
            if ((category != null))
            {
                queryString += "&category=" + System.Uri.EscapeDataString(category);
            }
            if ((headlineCount != null))
            {
                queryString += "&headlineCount=" + System.Uri.EscapeDataString(headlineCount.Value.ToString());
            }
            if ((originalImg != null))
            {
                queryString += "&originalImg=" + System.Uri.EscapeDataString(originalImg.Value.ToString());
            }

            // Return the result.
            return ProcessRequest("", queryString);
        }

        /// <summary>
        /// News search.
        /// </summary>
        /// <param name="query">The user's search query string.</param>
        /// <param name="count">The number of search results to return in the response. The actual number delivered may be less than requested.</param>
        /// <param name="skip">The zero-based offset that indicates the number of search results to skip before returning results.</param>
        /// <param name="mkt">
        /// The market where the results come from. Typically, this is the country where the user is making the request from; however, it could be a different country if the user is not located in a country where Bing delivers results. The market must be in the form {language code}-{country code}. For example, en-US. 
        /// Full list of supported markets: 
        /// es-AR,en-AU,de-AT,nl-BE,fr-BE,pt-BR,en-CA,fr-CA,es-CL,da-DK,fi-FI,fr-FR,de-DE,zh-HK,en-IN,en-ID,en-IE,it-IT,ja-JP,ko-KR,en-MY,es-MX,nl-NL,en-NZ,no-NO,zh-CN,pl-PL,pt-PT,en-PH,ru-RU,ar-SA,en-ZA,es-ES,sv-SE,fr-CH,de-CH,zh-TW,tr-TR,en-GB,en-US,es-US
        /// </param>
        /// <param name="safeSearch">A filter used to filter results for adult content.
        /// Moderate, Strict, Off
        /// </param>
        /// <param name="textDecorations">A Boolean value that determines whether query terms in the response strings will include hit highlighting characters. If true, the strings will include highlighting characters; otherwise, false. The default is false.</param>
        /// <param name="textFormat">The type of formatting to apply to displayable strings.
        /// For hit highlighting(see textDecorations), the format determines the highlighting characters to use in the response strings.The following are the possible values.
        /// Raw—Use special Unicode characters, E000 and E001, to highlight the query terms.For information about processing strings with the embedded Unicode characters, see Hit Highlighting.
        /// HTML—Use HTML tags to highlight the query terms.
        /// The default is Raw.
        /// To enable or disable hit highlighting, set the textDecorations query parameter.
        /// For displayable strings that contain escapable HTML characters such as&lt;, &gt;, if textFormat is set to HTML, the characters will be escaped as appropriate (for example, will be escaped to &lt;). If you do not require hit highlighting, you may specify textFormat without specifying textDecorations.
        /// </param>
        /// <param name="setLang">The language to use for user interface strings. Specify the language using the ISO 639-1 2-letter language code. For example, the language code for English is EN. The default is EN (English).</param>
        /// <param name="freshness">Filter search results by age. Age refers to the date and time that the webpage was discovered by Bing. The following are the possible filter values.
        /// Day — Return webpages discovered within the last 24 hours
        /// Week — Return webpages discovered within the last 7 days
        /// Month — Return webpages discovered within the last 30 days
        /// </param>
        /// <param name="countryCode">A 2-character country code of the country where the results come from. For a list of possible values, see Market Codes.
        /// <seealso cref="https://msdn.microsoft.com/en-us/library/dn783426.aspx"/>
        /// </param>
        /// <param name="category">The category of articles to return. For example, Sports articles or Entertainment articles. 
        /// <seealso cref="https://msdn.microsoft.com/en-us/library/dn760793.aspx#categoriesbymarket"/>
        /// </param>
        /// <param name="headlineCount">The number of headline articles and clusters to return. The default is 12. 
        /// Specify this parameter only if you do not specify the category parameter; this parameter is ignored if you specify the category parameter.
        /// </param>
        /// <param name="originalImg">A Boolean value that determines whether the image's contentUrl contains a URL that points to a thumbnail of the original article's image or the image itself. 
        /// If the article includes an image and this parameter is set to true, the image's contentUrl property will contain a URL that you may use to download the original image from the publisher's website. Otherwise, if this parameter is false, the image's contentUrl and thumbnailUrl URLs will both point to the same thumbnail image. 
        /// The default is false.
        /// </param>
        /// <returns>The search result.</returns>
        public News.CognitiveNewsSearchResults NewsEx(string query, long? count, long? skip, string mkt, string safeSearch, bool? textDecorations, string textFormat, string setLang, string freshness, string countryCode, string category, long? headlineCount, bool? originalImg)
        {
            string queryString = "";

            if ((query == null))
            {
                throw new System.ArgumentNullException(nameof(query), "Query value cannot be null.");
            }

            if ((query != null))
            {
                queryString += "&q=" + System.Uri.EscapeDataString(query);
            }
            if ((count != null))
            {
                queryString += "&count=" + System.Uri.EscapeDataString(count.Value.ToString());
            }
            if ((skip != null))
            {
                queryString += "&offset=" + System.Uri.EscapeDataString(skip.Value.ToString());
            }
            if ((mkt != null))
            {
                queryString += "&mkt=" + System.Uri.EscapeDataString(mkt);
            }
            if ((safeSearch != null))
            {
                queryString += "&safeSearch=" + System.Uri.EscapeDataString(safeSearch);
            }
            if ((textDecorations != null))
            {
                queryString += "&textDecorations=" + System.Uri.EscapeDataString(textDecorations.Value.ToString());
            }
            if ((textFormat != null))
            {
                queryString += "&textFormat=" + System.Uri.EscapeDataString(textFormat);
            }
            if ((setLang != null))
            {
                queryString += "&setLang=" + System.Uri.EscapeDataString(setLang);
            }
            if ((freshness != null))
            {
                queryString += "&freshness=" + System.Uri.EscapeDataString(freshness);
            }
            if ((countryCode != null))
            {
                queryString += "&cc=" + System.Uri.EscapeDataString(countryCode);
            }
            if ((category != null))
            {
                queryString += "&category=" + System.Uri.EscapeDataString(category);
            }
            if ((headlineCount != null))
            {
                queryString += "&headlineCount=" + System.Uri.EscapeDataString(headlineCount.Value.ToString());
            }
            if ((originalImg != null))
            {
                queryString += "&originalImg=" + System.Uri.EscapeDataString(originalImg.Value.ToString());
            }

            // Return the result.
            return ProcessRequest<News.CognitiveNewsSearchResults>("", queryString);
        }

        /// <summary>
        /// Images search.
        /// </summary>
        /// <param name="query">The user's search query string.</param>
        /// <param name="count">The number of search results to return in the response. The actual number delivered may be less than requested.</param>
        /// <param name="skip">The zero-based offset that indicates the number of search results to skip before returning results.</param>
        /// <param name="mkt">
        /// The market where the results come from. Typically, this is the country where the user is making the request from; however, it could be a different country if the user is not located in a country where Bing delivers results. The market must be in the form {language code}-{country code}. For example, en-US. 
        /// Full list of supported markets: 
        /// es-AR,en-AU,de-AT,nl-BE,fr-BE,pt-BR,en-CA,fr-CA,es-CL,da-DK,fi-FI,fr-FR,de-DE,zh-HK,en-IN,en-ID,en-IE,it-IT,ja-JP,ko-KR,en-MY,es-MX,nl-NL,en-NZ,no-NO,zh-CN,pl-PL,pt-PT,en-PH,ru-RU,ar-SA,en-ZA,es-ES,sv-SE,fr-CH,de-CH,zh-TW,tr-TR,en-GB,en-US,es-US
        /// </param>
        /// <param name="safeSearch">A filter used to filter results for adult content.
        /// Moderate, Strict, Off
        /// </param>
        /// <param name="setLang">The language to use for user interface strings. Specify the language using the ISO 639-1 2-letter language code. For example, the language code for English is EN. The default is EN (English).</param>
        /// <param name="freshness">Filter search results by age. Age refers to the date and time that the webpage was discovered by Bing. The following are the possible filter values.
        /// Day — Return webpages discovered within the last 24 hours
        /// Week — Return webpages discovered within the last 7 days
        /// Month — Return webpages discovered within the last 30 days
        /// </param>
        /// <param name="countryCode">A 2-character country code of the country where the results come from. For a list of possible values, see Market Codes.
        /// <seealso cref="https://msdn.microsoft.com/en-us/library/dn783426.aspx"/>
        /// </param>
        /// <param name="cab">The bottom coordinate of the region to crop. 
        /// The coordinate is a fractional value of the original image's height and is measured from the top, left corner of the image. Specify the coordinate as a value from 0.0 through 1.0. 
        /// </param>
        /// <param name="cal">The left coordinate of the region to crop. 
        /// The coordinate is a fraction of the original image's width and is measured from the top, left corner of the image. Specify the coordinate as a value from 0.0 through 1.0. 
        /// </param>
        /// <param name="car">The right coordinate of the region to crop. 
        /// The coordinate is a fraction of the original image's width and is measured from the top, left corner of the image. Specify the coordinate as a value from 0.0 through 1.0. 
        /// </param>
        /// <param name="cat">The top coordinate of the region to crop. 
        /// The coordinate is a fraction of the original image's height and is measured from the top, left corner of the image. Specify the coordinate as a value from 0.0 through 1.0. 
        /// </param>
        /// <param name="ct">The crop type to use to crop the image based on the coordinates specified in the cal, cat, car, and cab parameters.
        /// The following are the possible values.
        /// 0—Rectangular (default)
        /// </param>
        /// <param name="id">An ID that uniquely identifies an image. You can use this parameter to ensure that the specified image is the first image in the images list. The Image object's imageId field contains the ID that you would set this parameter to. 
        /// </param>
        /// <param name="imgUrl">A URL to an image that you want to get insights of. This is an alternative to specifying the image by using the insightsToken parameter.
        /// To specify the image, you may use either the imgUrl parameter in a GET request or place the binary of the image in the body of a POST request.
        /// The maximum image size is 1 MB.
        /// </param>
        /// <param name="insightsToken">The token from a previous Image API call (see imageInsightsToken). Specify this parameter to get additional information about an image, such as a caption or shopping source. 
        /// For a list of the additional information that you can get, <seealso cref="https://msdn.microsoft.com/en-us/library/dn760791.aspx#imageinsightstoken"/> query parameter.
        /// This parameter is supported only by the Image API; do not specify this parameter when calling the Trending Images API or the Search API.
        /// </param>
        /// <param name="modulesRequested">
        /// A comma-delimited list of one or more insights to request. (Note that when you URL encode the query string, the commas will change to %2C.) The following are the possible case-insensitive values.
        /// All—Return all insights, if available, except RecognizedEntities.
        /// Annotations—Provides characteristics of the type of content found in the image. For example, if the image is of a person, the annotations might indicate the person's gender and the type of clothes they're wearing.
        /// BRQ—Best representative query.This is a query string that best describes the image. 
        /// Caption—A caption that provides information about the image. If the caption contains entities, the response may include links to images of those entities.
        /// Collections—A list of related images.
        /// Recipes—A list of recipes for cooking the food shown in the images.
        /// PagesIncluding—A list of webpages that include the image.
        /// RecognizedEntities—A list of entities (people) that were recognized in the image.
        /// RelatedSearches—A list of related searches that were made by others.
        /// ShoppingSources—A list of merchants where you can buy related offerings.
        /// SimilarImages—A list of images that are visually similar to the original image.
        /// SimilarProducts—A list of images that contain a product that is similar to a product found in the original image.
        /// </param>
        /// <param name="aspect">Filter images by aspect ratio. The following are the possible filter values.
        /// Square — Return images with standard aspect ratio
        /// Wide — Return images with wide screen aspect ratio
        /// Tall — Return images with tall aspect ratio
        /// All — Do not filter by aspect.Specifying this value is the same as not specifying the aspect parameter.
        /// </param>
        /// <param name="color">Filter images by color. The following are the possible filter values.
        /// ColorOnly — Return color images
        /// Monochrome — Return black and white images
        /// Black, Blue, Brown, Gray, Green, Orange, Pink, Purple, Red, Teal, White, Yellow
        /// </param>
        /// <param name="imageContent">Filter images by content. The following are the possible filter values.
        /// Face — Return images that show only a person's face
        /// Portrait — Return images that show only a person's head and shoulders
        /// </param>
        /// <param name="imageType">Filter images by image type. The following are the possible filter values.
        /// AnimatedGif — Return only animated GIFs 
        /// Clipart — Return only clip art images
        /// Line — Return only line drawings
        /// Photo — Return only photographs(excluding line drawings, animated Gifs, and clip art)
        /// Shopping — Return only images that contain items where Bing knows of a merchant that is selling the items.
        /// </param>
        /// <param name="license">Filter images by the type of license applied to the image. The following are the possible filter values.
        /// Public — Return images where the creator has waived their exclusive rights, to the fullest extent allowed by law.
        /// Share — Return images that may be shared with others. Changing or editing the image might not be allowed.Also, modifying, sharing, and using the image for commercial purposes might not be allowed.Typically, this option returns the most images.
        /// ShareCommercially — Return images that may be shared with others for personal or commercial purposes. Changing or editing the image might not be allowed.
        /// Modify — Return images that may be modified, shared, and used. Changing or editing the image might not be allowed.Modifying, sharing, and using the image for commercial purposes might not be allowed.
        /// ModifyCommercially — Return images that may be modified, shared, and used for personal or commercial purposes. Typically, this option returns the fewest images.
        /// All — Do not filter by license type. Specifying this value is the same as not specifying the license parameter.
        /// </param>
        /// <param name="size">Filter images by size. The following are the possible filter values.
        /// Small — Return images that are less than 200x200 pixels
        /// Medium — Return images that are greater than or equal to 200x200 pixels but less than 500x500 pixels
        /// Large — Return images that are 500x500 pixels or larger
        /// Wallpaper — Return wallpaper images.
        /// All — Do not filter by size.Specifying this value is the same as not specifying the size parameter.
        /// You may use this parameter in conjunction with the height or width parameters.For example, you may use height and size to request small images that are at least 150 pixels tall.
        /// </param>
        /// <param name="height">Filter images that have the specified height, in pixels. 
        /// This filter may be used in conjunction with the size filter.For example, return small images that have a height of 150 pixels.
        /// </param>
        /// <param name="width">Filter images that have the specified width, in pixels. 
        /// This filter may be used in conjunction with the size filter.For example, return small images that have a width of 150 pixels.
        /// </param>
        /// <returns>The search result.</returns>
        public byte[] Images(string query, long? count, long? skip, string mkt, string safeSearch, 
            string setLang, string freshness, string countryCode, 
            float? cab, float? cal, float? car, float? cat, uint? ct, string id, string imgUrl, 
            string insightsToken, string modulesRequested, string aspect, string color,
            string imageContent, string imageType, string license, string size, uint? height, uint? width)
        {
            string queryString = "";

            if ((query == null))
            {
                throw new System.ArgumentNullException(nameof(query), "Query value cannot be null.");
            }

            if ((query != null))
            {
                queryString += "&q=" + System.Uri.EscapeDataString(query);
            }
            if ((count != null))
            {
                queryString += "&count=" + System.Uri.EscapeDataString(count.Value.ToString());
            }
            if ((skip != null))
            {
                queryString += "&offset=" + System.Uri.EscapeDataString(skip.Value.ToString());
            }
            if ((mkt != null))
            {
                queryString += "&mkt=" + System.Uri.EscapeDataString(mkt);
            }
            if ((safeSearch != null))
            {
                queryString += "&safeSearch=" + System.Uri.EscapeDataString(safeSearch);
            }
            if ((setLang != null))
            {
                queryString += "&setLang=" + System.Uri.EscapeDataString(setLang);
            }
            if ((freshness != null))
            {
                queryString += "&freshness=" + System.Uri.EscapeDataString(freshness);
            }
            if ((countryCode != null))
            {
                queryString += "&cc=" + System.Uri.EscapeDataString(countryCode);
            }
            if ((cab != null))
            {
                queryString += "&cab=" + System.Uri.EscapeDataString(cab.Value.ToString());
            }
            if ((cal != null))
            {
                queryString += "&cal=" + System.Uri.EscapeDataString(cal.Value.ToString());
            }
            if ((car != null))
            {
                queryString += "&car=" + System.Uri.EscapeDataString(car.Value.ToString());
            }
            if ((cat != null))
            {
                queryString += "&cat=" + System.Uri.EscapeDataString(cat.Value.ToString());
            }
            if ((ct != null))
            {
                queryString += "&ct=" + System.Uri.EscapeDataString(ct.Value.ToString());
            }
            if ((id != null))
            {
                queryString += "&id=" + System.Uri.EscapeDataString(id);
            }
            if ((imgUrl != null))
            {
                queryString += "&imgUrl=" + System.Uri.EscapeDataString(imgUrl);
            }
            if ((insightsToken != null))
            {
                queryString += "&insightsToken=" + System.Uri.EscapeDataString(insightsToken);
            }
            if ((modulesRequested != null))
            {
                queryString += "&modulesRequested=" + System.Uri.EscapeDataString(modulesRequested);
            }
            if ((aspect != null))
            {
                queryString += "&aspect=" + System.Uri.EscapeDataString(aspect);
            }
            if ((color != null))
            {
                queryString += "&color=" + System.Uri.EscapeDataString(color);
            }
            if ((imageContent != null))
            {
                queryString += "&imageContent=" + System.Uri.EscapeDataString(imageContent);
            }
            if ((imageType != null))
            {
                queryString += "&imageType=" + System.Uri.EscapeDataString(imageType);
            }
            if ((license != null))
            {
                queryString += "&license=" + System.Uri.EscapeDataString(license);
            }
            if ((size != null))
            {
                queryString += "&size=" + System.Uri.EscapeDataString(size);
            }
            if ((height != null))
            {
                queryString += "&height=" + System.Uri.EscapeDataString(height.Value.ToString());
            }
            if ((width != null))
            {
                queryString += "&width=" + System.Uri.EscapeDataString(width.Value.ToString());
            }

            // Return the result.
            return ProcessRequest("", queryString);
        }

        /// <summary>
        /// Images search.
        /// </summary>
        /// <param name="query">The user's search query string.</param>
        /// <param name="count">The number of search results to return in the response. The actual number delivered may be less than requested.</param>
        /// <param name="skip">The zero-based offset that indicates the number of search results to skip before returning results.</param>
        /// <param name="mkt">
        /// The market where the results come from. Typically, this is the country where the user is making the request from; however, it could be a different country if the user is not located in a country where Bing delivers results. The market must be in the form {language code}-{country code}. For example, en-US. 
        /// Full list of supported markets: 
        /// es-AR,en-AU,de-AT,nl-BE,fr-BE,pt-BR,en-CA,fr-CA,es-CL,da-DK,fi-FI,fr-FR,de-DE,zh-HK,en-IN,en-ID,en-IE,it-IT,ja-JP,ko-KR,en-MY,es-MX,nl-NL,en-NZ,no-NO,zh-CN,pl-PL,pt-PT,en-PH,ru-RU,ar-SA,en-ZA,es-ES,sv-SE,fr-CH,de-CH,zh-TW,tr-TR,en-GB,en-US,es-US
        /// </param>
        /// <param name="safeSearch">A filter used to filter results for adult content.
        /// Moderate, Strict, Off
        /// </param>
        /// <param name="setLang">The language to use for user interface strings. Specify the language using the ISO 639-1 2-letter language code. For example, the language code for English is EN. The default is EN (English).</param>
        /// <param name="freshness">Filter search results by age. Age refers to the date and time that the webpage was discovered by Bing. The following are the possible filter values.
        /// Day — Return webpages discovered within the last 24 hours
        /// Week — Return webpages discovered within the last 7 days
        /// Month — Return webpages discovered within the last 30 days
        /// </param>
        /// <param name="countryCode">A 2-character country code of the country where the results come from. For a list of possible values, see Market Codes.
        /// <seealso cref="https://msdn.microsoft.com/en-us/library/dn783426.aspx"/>
        /// </param>
        /// <param name="cab">The bottom coordinate of the region to crop. 
        /// The coordinate is a fractional value of the original image's height and is measured from the top, left corner of the image. Specify the coordinate as a value from 0.0 through 1.0. 
        /// </param>
        /// <param name="cal">The left coordinate of the region to crop. 
        /// The coordinate is a fraction of the original image's width and is measured from the top, left corner of the image. Specify the coordinate as a value from 0.0 through 1.0. 
        /// </param>
        /// <param name="car">The right coordinate of the region to crop. 
        /// The coordinate is a fraction of the original image's width and is measured from the top, left corner of the image. Specify the coordinate as a value from 0.0 through 1.0. 
        /// </param>
        /// <param name="cat">The top coordinate of the region to crop. 
        /// The coordinate is a fraction of the original image's height and is measured from the top, left corner of the image. Specify the coordinate as a value from 0.0 through 1.0. 
        /// </param>
        /// <param name="ct">The crop type to use to crop the image based on the coordinates specified in the cal, cat, car, and cab parameters.
        /// The following are the possible values.
        /// 0—Rectangular (default)
        /// </param>
        /// <param name="id">An ID that uniquely identifies an image. You can use this parameter to ensure that the specified image is the first image in the images list. The Image object's imageId field contains the ID that you would set this parameter to. 
        /// </param>
        /// <param name="imgUrl">A URL to an image that you want to get insights of. This is an alternative to specifying the image by using the insightsToken parameter.
        /// To specify the image, you may use either the imgUrl parameter in a GET request or place the binary of the image in the body of a POST request.
        /// The maximum image size is 1 MB.
        /// </param>
        /// <param name="insightsToken">The token from a previous Image API call (see imageInsightsToken). Specify this parameter to get additional information about an image, such as a caption or shopping source. 
        /// For a list of the additional information that you can get, <seealso cref="https://msdn.microsoft.com/en-us/library/dn760791.aspx#imageinsightstoken"/> query parameter.
        /// This parameter is supported only by the Image API; do not specify this parameter when calling the Trending Images API or the Search API.
        /// </param>
        /// <param name="modulesRequested">
        /// A comma-delimited list of one or more insights to request. (Note that when you URL encode the query string, the commas will change to %2C.) The following are the possible case-insensitive values.
        /// All—Return all insights, if available, except RecognizedEntities.
        /// Annotations—Provides characteristics of the type of content found in the image. For example, if the image is of a person, the annotations might indicate the person's gender and the type of clothes they're wearing.
        /// BRQ—Best representative query.This is a query string that best describes the image. 
        /// Caption—A caption that provides information about the image. If the caption contains entities, the response may include links to images of those entities.
        /// Collections—A list of related images.
        /// Recipes—A list of recipes for cooking the food shown in the images.
        /// PagesIncluding—A list of webpages that include the image.
        /// RecognizedEntities—A list of entities (people) that were recognized in the image.
        /// RelatedSearches—A list of related searches that were made by others.
        /// ShoppingSources—A list of merchants where you can buy related offerings.
        /// SimilarImages—A list of images that are visually similar to the original image.
        /// SimilarProducts—A list of images that contain a product that is similar to a product found in the original image.
        /// </param>
        /// <param name="aspect">Filter images by aspect ratio. The following are the possible filter values.
        /// Square — Return images with standard aspect ratio
        /// Wide — Return images with wide screen aspect ratio
        /// Tall — Return images with tall aspect ratio
        /// All — Do not filter by aspect.Specifying this value is the same as not specifying the aspect parameter.
        /// </param>
        /// <param name="color">Filter images by color. The following are the possible filter values.
        /// ColorOnly — Return color images
        /// Monochrome — Return black and white images
        /// Black, Blue, Brown, Gray, Green, Orange, Pink, Purple, Red, Teal, White, Yellow
        /// </param>
        /// <param name="imageContent">Filter images by content. The following are the possible filter values.
        /// Face — Return images that show only a person's face
        /// Portrait — Return images that show only a person's head and shoulders
        /// </param>
        /// <param name="imageType">Filter images by image type. The following are the possible filter values.
        /// AnimatedGif — Return only animated GIFs 
        /// Clipart — Return only clip art images
        /// Line — Return only line drawings
        /// Photo — Return only photographs(excluding line drawings, animated Gifs, and clip art)
        /// Shopping — Return only images that contain items where Bing knows of a merchant that is selling the items.
        /// </param>
        /// <param name="license">Filter images by the type of license applied to the image. The following are the possible filter values.
        /// Public — Return images where the creator has waived their exclusive rights, to the fullest extent allowed by law.
        /// Share — Return images that may be shared with others. Changing or editing the image might not be allowed.Also, modifying, sharing, and using the image for commercial purposes might not be allowed.Typically, this option returns the most images.
        /// ShareCommercially — Return images that may be shared with others for personal or commercial purposes. Changing or editing the image might not be allowed.
        /// Modify — Return images that may be modified, shared, and used. Changing or editing the image might not be allowed.Modifying, sharing, and using the image for commercial purposes might not be allowed.
        /// ModifyCommercially — Return images that may be modified, shared, and used for personal or commercial purposes. Typically, this option returns the fewest images.
        /// All — Do not filter by license type. Specifying this value is the same as not specifying the license parameter.
        /// </param>
        /// <param name="size">Filter images by size. The following are the possible filter values.
        /// Small — Return images that are less than 200x200 pixels
        /// Medium — Return images that are greater than or equal to 200x200 pixels but less than 500x500 pixels
        /// Large — Return images that are 500x500 pixels or larger
        /// Wallpaper — Return wallpaper images.
        /// All — Do not filter by size.Specifying this value is the same as not specifying the size parameter.
        /// You may use this parameter in conjunction with the height or width parameters.For example, you may use height and size to request small images that are at least 150 pixels tall.
        /// </param>
        /// <param name="height">Filter images that have the specified height, in pixels. 
        /// This filter may be used in conjunction with the size filter.For example, return small images that have a height of 150 pixels.
        /// </param>
        /// <param name="width">Filter images that have the specified width, in pixels. 
        /// This filter may be used in conjunction with the size filter.For example, return small images that have a width of 150 pixels.
        /// </param>
        /// <returns>The search result.</returns>
        public Images.CognitiveImagesSearchResults ImagesEx(
            string query, long? count, long? skip, string mkt, string safeSearch,
            string setLang, string freshness, string countryCode,
            float? cab, float? cal, float? car, float? cat, uint? ct, string id, string imgUrl,
            string insightsToken, string modulesRequested, string aspect, string color,
            string imageContent, string imageType, string license, string size, uint? height, uint? width)
        {
            string queryString = "";

            if ((query == null))
            {
                throw new System.ArgumentNullException(nameof(query), "Query value cannot be null.");
            }

            if ((query != null))
            {
                queryString += "&q=" + System.Uri.EscapeDataString(query);
            }
            if ((count != null))
            {
                queryString += "&count=" + System.Uri.EscapeDataString(count.Value.ToString());
            }
            if ((skip != null))
            {
                queryString += "&offset=" + System.Uri.EscapeDataString(skip.Value.ToString());
            }
            if ((mkt != null))
            {
                queryString += "&mkt=" + System.Uri.EscapeDataString(mkt);
            }
            if ((safeSearch != null))
            {
                queryString += "&safeSearch=" + System.Uri.EscapeDataString(safeSearch);
            }
            if ((setLang != null))
            {
                queryString += "&setLang=" + System.Uri.EscapeDataString(setLang);
            }
            if ((freshness != null))
            {
                queryString += "&freshness=" + System.Uri.EscapeDataString(freshness);
            }
            if ((countryCode != null))
            {
                queryString += "&cc=" + System.Uri.EscapeDataString(countryCode);
            }
            if ((cab != null))
            {
                queryString += "&cab=" + System.Uri.EscapeDataString(cab.Value.ToString());
            }
            if ((cal != null))
            {
                queryString += "&cal=" + System.Uri.EscapeDataString(cal.Value.ToString());
            }
            if ((car != null))
            {
                queryString += "&car=" + System.Uri.EscapeDataString(car.Value.ToString());
            }
            if ((cat != null))
            {
                queryString += "&cat=" + System.Uri.EscapeDataString(cat.Value.ToString());
            }
            if ((ct != null))
            {
                queryString += "&ct=" + System.Uri.EscapeDataString(ct.Value.ToString());
            }
            if ((id != null))
            {
                queryString += "&id=" + System.Uri.EscapeDataString(id);
            }
            if ((imgUrl != null))
            {
                queryString += "&imgUrl=" + System.Uri.EscapeDataString(imgUrl);
            }
            if ((insightsToken != null))
            {
                queryString += "&insightsToken=" + System.Uri.EscapeDataString(insightsToken);
            }
            if ((modulesRequested != null))
            {
                queryString += "&modulesRequested=" + System.Uri.EscapeDataString(modulesRequested);
            }
            if ((aspect != null))
            {
                queryString += "&aspect=" + System.Uri.EscapeDataString(aspect);
            }
            if ((color != null))
            {
                queryString += "&color=" + System.Uri.EscapeDataString(color);
            }
            if ((imageContent != null))
            {
                queryString += "&imageContent=" + System.Uri.EscapeDataString(imageContent);
            }
            if ((imageType != null))
            {
                queryString += "&imageType=" + System.Uri.EscapeDataString(imageType);
            }
            if ((license != null))
            {
                queryString += "&license=" + System.Uri.EscapeDataString(license);
            }
            if ((size != null))
            {
                queryString += "&size=" + System.Uri.EscapeDataString(size);
            }
            if ((height != null))
            {
                queryString += "&height=" + System.Uri.EscapeDataString(height.Value.ToString());
            }
            if ((width != null))
            {
                queryString += "&width=" + System.Uri.EscapeDataString(width.Value.ToString());
            }

            // Return the result.
            return ProcessRequest<Images.CognitiveImagesSearchResults>("", queryString);
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
            Uri constructedServiceUri = new Uri(_serviceRoot.AbsoluteUri.TrimEnd(new char[] { '/' }) + (String.IsNullOrEmpty(serviceName) ? "" : "") + "?" + query.TrimStart(new char[] { '&' }));
            Net.NetRequest request = new NetRequest();
            request.Method = "GET";
            request.AddHeader("Ocp-Apim-Subscription-Key", _apiKey);

            if ((_clientIP != null))
            {
                request.AddHeader("X-Search-ClientIP", _clientIP);
            }
            if ((_clientID != null))
            {
                request.AddHeader("X-MSEdge-ClientID", _clientID);
            }
            if ((_userAgent != null))
            {
                request.AddHeader("User-Agent", _userAgent);
            }

            // Return the result.
            return Nequeo.Net.HttpJsonClient.Request<T>(constructedServiceUri, request);
        }

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <param name="serviceName">The service name to call.</param>
        /// <param name="query">The query to apply.</param>
        /// <returns>The returned type.</returns>
        private byte[] ProcessRequest(string serviceName, string query)
        {
            Uri constructedServiceUri = new Uri(_serviceRoot.AbsoluteUri.TrimEnd(new char[] { '/' }) + (String.IsNullOrEmpty(serviceName) ? "" : "") + "?" + query.TrimStart(new char[] { '&' }));
            Net.NetRequest request = new NetRequest();
            request.Method = "GET";
            request.AddHeader("Ocp-Apim-Subscription-Key", _apiKey);

            if ((_clientIP != null))
            {
                request.AddHeader("X-Search-ClientIP", _clientIP);
            }
            if ((_clientID != null))
            {
                request.AddHeader("X-MSEdge-ClientID", _clientID);
            }
            if ((_userAgent != null))
            {
                request.AddHeader("User-Agent", _userAgent);
            }

            // Return the result.
            return Nequeo.Net.HttpDataClient.Request(constructedServiceUri, request);
        }
    }
}
