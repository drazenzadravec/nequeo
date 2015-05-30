/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.IO;

using QueryParameter = System.Collections.Generic.KeyValuePair<string, string>;

using Nequeo.Cryptography.Signing;

namespace Nequeo.Net.OAuth.Framework.Utility
{
    /// <summary>
    /// URI helper.
    /// </summary>
    public static class UriUtility
    {
        const string OAuthAuthorizationHeaderStart = "OAuth";
        static readonly string[] HexEscapedUriRfc3986CharsToEscape;
        static readonly string[] QuoteCharacters = new[] { "\"", "'" };
        static readonly string[] UriRfc3986CharsToEscape = new[] { "!", "*", "'", "(", ")" };

        /// <summary>
        /// URI utility constructor.
        /// </summary>
        static UriUtility()
        {
            HexEscapedUriRfc3986CharsToEscape = UriRfc3986CharsToEscape.Select(c => Uri.HexEscape(c[0])).ToArray();
        }

        /// <summary>
        /// Escape Uri Data String Rfc3986
        /// </summary>
        /// <param name="value">The rfc value.</param>
        /// <returns>The string value.</returns>
        /// <remarks>see "http://stackoverflow.com/questions/846487/how-to-get-uri-escapedatastring-to-comply-with-rfc-3986" </remarks>
        static string EscapeUriDataStringRfc3986(string value)
        {
            var escaped = new StringBuilder(Uri.EscapeDataString(value));

            for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
            {
                escaped.Replace(UriRfc3986CharsToEscape[i], HexEscapedUriRfc3986CharsToEscape[i]);
            }

            return escaped.ToString();
        }

        /// <summary>
        /// Extracts all the parameters from the supplied encoded parameters.
        /// </summary>
        /// <param name="parameters">The paramters.</param>
        /// <returns>The list of query parameters</returns>
        public static List<QueryParameter> GetQueryParameters(string parameters)
        {
            if (parameters.StartsWith("?"))
            {
                parameters = parameters.Remove(0, 1);
            }

            var result = new List<QueryParameter>();

            if (!string.IsNullOrEmpty(parameters))
            {
                string[] p = parameters.Split('&');
                foreach (string s in p)
                {
                    if (!string.IsNullOrEmpty(s) && !s.StartsWith(Parameters.OAuthParameterPrefix) && !s.StartsWith(Parameters.XAuthParameterPrefix))
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            string[] temp = s.Split('=');
                            result.Add(new QueryParameter(temp[0], temp[1]));
                        }
                        else
                        {
                            result.Add(new QueryParameter(s, string.Empty));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Extracts all the parameters from the supplied encoded parameters.
        /// </summary>
        /// <param name="parameters">The paramters.</param>
        /// <returns>The list of query parameters</returns>
        public static List<QueryParameter> GetHeaderParameters(string parameters)
        {
            parameters = parameters.Trim();

            var result = new List<QueryParameter>();

            if (!parameters.StartsWith(OAuthAuthorizationHeaderStart, StringComparison.InvariantCultureIgnoreCase))
            {
                return result;
            }

            parameters = parameters.Substring(OAuthAuthorizationHeaderStart.Length).Trim();

            if (!string.IsNullOrEmpty(parameters))
            {
                string[] p = parameters.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in p)
                {
                    if (string.IsNullOrEmpty(s)) continue;
                    QueryParameter parameter = ParseAuthorizationHeaderKeyValuePair(s);
                    result.Add(parameter);
                }
            }

            return result;
        }

        /// <summary>
        /// Parse Authorization Header Key Value Pair
        /// </summary>
        /// <param name="keyEqualValuePair">The key value pair.</param>
        /// <returns>The query parameter</returns>
        public static QueryParameter ParseAuthorizationHeaderKeyValuePair(string keyEqualValuePair)
        {
            int indexOfEqualSign = keyEqualValuePair.IndexOf('=');

            if (indexOfEqualSign > 0)
            {
                string key = keyEqualValuePair.Substring(0, indexOfEqualSign).Trim();

                string quotedValue = keyEqualValuePair.Substring(indexOfEqualSign + 1);

                string itemValue = StripQuotes(quotedValue);

                itemValue = HttpUtility.UrlDecode(itemValue);

                return new QueryParameter(key, itemValue);
            }

            return new QueryParameter(keyEqualValuePair.Trim(), string.Empty);
        }

        /// <summary>
        /// Strip of the quatoes.
        /// </summary>
        /// <param name="quotedValue">The value with quotes.</param>
        /// <returns>The new value.</returns>
        static string StripQuotes(string quotedValue)
        {
            foreach (string quoteCharacter in QuoteCharacters)
            {
                if (quotedValue.StartsWith(quoteCharacter) && quotedValue.EndsWith(quoteCharacter) && quotedValue.Length > 1)
                {
                    return quotedValue.Substring(1, quotedValue.Length - 2);
                }
            }

            return quotedValue;
        }

        /// <summary>
        /// Encode the URI
        /// </summary>
        /// <param name="value">The value to encode.</param>
        /// <returns>The encoded value.</returns>
        public static string UrlEncode(string value)
        {
            return EscapeUriDataStringRfc3986(value);
        }

        /// <summary>
        /// Formats a set of query parameters, as per query string encoding.
        /// </summary>
        /// <param name="parameters">The collection of parameters</param>
        /// <returns>The string equivalent.</returns>
        public static string FormatQueryString(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var builder = new StringBuilder();

            if (parameters != null)
            {
                foreach (var pair in parameters)
                {
                    if (builder.Length > 0) builder.Append("&");
                    builder.Append(pair.Key).Append("=");
                    builder.Append(UrlEncode(pair.Value));
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Formats a set of query parameters, as per query string encoding.
        /// </summary>
        /// <param name="parameters">The name value collection.</param>
        /// <returns>The formatted query string.</returns>
        public static string FormatQueryString(NameValueCollection parameters)
        {
            var builder = new StringBuilder();

            if (parameters != null)
            {
                foreach (string key in parameters.Keys)
                {
                    if (builder.Length > 0) builder.Append("&");
                    builder.Append(key).Append("=");
                    builder.Append(UrlEncode(parameters[key]));
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Takes an http method, url and a set of parameters and formats them as a signature base as per the OAuth core spec.
        /// </summary>
        /// <param name="httpMethod">The http method, (GET, POST)</param>
        /// <param name="url">The URI.</param>
        /// <param name="parameters">The query parameters</param>
        /// <returns>The formatted parameters.</returns>
        public static string FormatParameters(string httpMethod, Uri url, List<QueryParameter> parameters)
        {
            string normalizedRequestParameters = NormalizeRequestParameters(parameters);

            var signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());

            signatureBase.AppendFormat("{0}&", UrlEncode(NormalizeUri(url)));
            signatureBase.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));

            return signatureBase.ToString();
        }

        /// <summary>
        /// Normalizes a sequence of key/value pair parameters as per the OAuth core specification.
        /// </summary>
        /// <param name="parameters">The collection of query parameters</param>
        /// <returns>The string of normalised parameters</returns>
        public static string NormalizeRequestParameters(IEnumerable<QueryParameter> parameters)
        {
            IEnumerable<QueryParameter> orderedParameters = parameters
                .OrderBy(x => x.Key, StringComparer.Ordinal)
                .ThenBy(x => x.Value)
                .Select(
                    x => new QueryParameter(x.Key, UrlEncode(x.Value)));

            var builder = new StringBuilder();

            foreach (var parameter in orderedParameters)
            {
                if (builder.Length > 0) builder.Append("&");

                builder.Append(parameter.Key).Append("=").Append(parameter.Value);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Normalizes a Url according to the OAuth specification (this ensures http or https on a default port is not displayed
        /// with the :XXX following the host in the url).
        /// </summary>
        /// <param name="uri">The URI</param>
        /// <returns>The normalised uri.</returns>
        public static string NormalizeUri(Uri uri)
        {
            string normalizedUrl = string.Format("{0}://{1}", uri.Scheme, uri.Host);

            if (!((uri.Scheme == "http" && uri.Port == 80) ||
                  (uri.Scheme == "https" && uri.Port == 443)))
            {
                normalizedUrl += ":" + uri.Port;
            }

            return normalizedUrl + ((uri.AbsolutePath == "/") ? "" : uri.AbsolutePath);
        }

        /// <summary>
        /// To query parameters
        /// </summary>
        /// <param name="source">The source name value collection.</param>
        /// <returns>The collection of query parameters</returns>
        public static IEnumerable<QueryParameter> ToQueryParameters(this NameValueCollection source)
        {
            foreach (string key in source.AllKeys)
            {
                yield return new QueryParameter(key, source[key]);
            }
        }

        /// <summary>
        /// To query parameters excluding token secret
        /// </summary>
        /// <param name="source">The source name value collection.</param>
        /// <returns>The collection of query parameters</returns>
        public static IEnumerable<QueryParameter> ToQueryParametersExcludingTokenSecret(this NameValueCollection source)
        {
            foreach (string key in source.AllKeys)
            {
                if (key != Parameters.OAuth_Token_Secret)
                {
                    yield return new QueryParameter(key, source[key]);
                }
            }
        }

        /// <summary>
        /// To name value collection
        /// </summary>
        /// <param name="source">The source collection of query parameters</param>
        /// <returns>The name value collection.</returns>
        public static NameValueCollection ToNameValueCollection(this IEnumerable<QueryParameter> source)
        {
            var collection = new NameValueCollection();

            foreach (var parameter in source)
            {
                collection[parameter.Key] = parameter.Value;
            }

            return collection;
        }

        /// <summary>
        /// Format token for response
        /// </summary>
        /// <param name="token">The current token context.</param>
        /// <returns>The string value of the query parameters.</returns>
        public static string FormatTokenForResponse(IToken token)
        {
            var builder = new StringBuilder();

            builder.Append(Parameters.OAuth_Token).Append("=").Append(UrlEncode(token.Token))
                .Append("&").Append(Parameters.OAuth_Token_Secret).Append("=").Append(UrlEncode(token.TokenSecret));

            return builder.ToString();
        }
    }
}
