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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.IO;
using System.Net;
using System.Xml.Linq;

using Nequeo.Net.OAuth.Storage;
using Nequeo.Net.OAuth.Framework;
using Nequeo.Net.OAuth.Framework.Signing;
using Nequeo.Net.OAuth.Framework.Utility;
using Nequeo.Net.OAuth.Provider.Inspectors;

namespace Nequeo.Net.OAuth.Consumer.Session
{
    /// <summary>
    /// Consumer Request Extensions
    /// </summary>
    public static class ConsumerRequestExtensions
    {
        /// <summary>
        /// Set the GET consumer request method.
        /// </summary>
        /// <param name="request">The consumer request source.</param>
        /// <returns>The consumer request.</returns>
        public static IConsumerRequest Get(this IConsumerRequest request)
        {
            return request.ForMethod("GET");
        }

        /// <summary>
        /// Set the DELETE consumer request method.
        /// </summary>
        /// <param name="request">The consumer request source.</param>
        /// <returns>The consumer request.</returns>
        public static IConsumerRequest Delete(this IConsumerRequest request)
        {
            return request.ForMethod("DELETE");
        }

        /// <summary>
        /// Set the PUT consumer request method.
        /// </summary>
        /// <param name="request">The consumer request source.</param>
        /// <returns>The consumer request.</returns>
        public static IConsumerRequest Put(this IConsumerRequest request)
        {
            return request.ForMethod("PUT");
        }

        /// <summary>
        /// Set the POST consumer request method.
        /// </summary>
        /// <param name="request">The consumer request source.</param>
        /// <returns>The consumer request.</returns>
        public static IConsumerRequest Post(this IConsumerRequest request)
        {
            return request.ForMethod("POST");
        }

        /// <summary>
        /// Apply parameters
        /// </summary>
        /// <param name="destination">The destination name value list</param>
        /// <param name="anonymousClass">The anonymous class.</param>
        static void ApplyParameters(NameValueCollection destination, object anonymousClass)
        {
            ApplyParameters(destination, new ReflectionBasedDictionaryAdapter(anonymousClass));
        }

        /// <summary>
        /// Apply parameters
        /// </summary>
        /// <param name="destination">The destination name value list</param>
        /// <param name="additions">The dictionary class.</param>
        static void ApplyParameters(NameValueCollection destination, IDictionary additions)
        {
            if (additions == null) throw new ArgumentNullException("additions");

            foreach (string parameter in additions.Keys)
            {
                destination[parameter] = Convert.ToString(additions[parameter]);
            }
        }

        /// <summary>
        /// From method
        /// </summary>
        /// <param name="request">The consumer request.</param>
        /// <param name="method">The methos</param>
        /// <returns>The consumer request.</returns>
        public static IConsumerRequest ForMethod(this IConsumerRequest request, string method)
        {
            request.Context.RequestMethod = method;
            return request;
        }

        /// <summary>
        /// For Uri
        /// </summary>
        /// <param name="request">The consumer request.</param>
        /// <param name="uri">The uri</param>
        /// <returns>The consumer request.</returns>
        public static IConsumerRequest ForUri(this IConsumerRequest request, Uri uri)
        {
            request.Context.RawUri = uri;
            return request;
        }

        /// <summary>
        /// For Uri
        /// </summary>
        /// <param name="request">The consumer request.</param>
        /// <param name="url">The url</param>
        /// <returns>The consumer request.</returns>
        public static IConsumerRequest ForUrl(this IConsumerRequest request, string url)
        {
            request.Context.RawUri = new Uri(url);
            return request;
        }

        /// <summary>
        /// With Form Parameters
        /// </summary>
        /// <param name="request">The consumer request.</param>
        /// <param name="dictionary">The dictionary</param>
        /// <returns>The consumer request.</returns>
        public static IConsumerRequest WithFormParameters(this IConsumerRequest request, IDictionary dictionary)
        {
            ApplyParameters(request.Context.FormEncodedParameters, dictionary);
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="acceptsType"></param>
        /// <returns></returns>
        public static IConsumerRequest WithAcceptHeader(this IConsumerRequest request, string acceptsType)
        {
            request.AcceptsType = acceptsType;
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        public static IConsumerRequest WithBody(this IConsumerRequest request, string requestBody)
        {
            request.RequestBody = requestBody;
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="anonymousClass"></param>
        /// <returns></returns>
        public static IConsumerRequest WithFormParameters(this IConsumerRequest request, object anonymousClass)
        {
            ApplyParameters(request.Context.FormEncodedParameters, anonymousClass);
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static IConsumerRequest WithQueryParameters(this IConsumerRequest request, IDictionary dictionary)
        {
            ApplyParameters(request.Context.QueryParameters, dictionary);
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="anonymousClass"></param>
        /// <returns></returns>
        public static IConsumerRequest WithQueryParameters(this IConsumerRequest request, object anonymousClass)
        {
            ApplyParameters(request.Context.QueryParameters, anonymousClass);
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static IConsumerRequest WithCookies(this IConsumerRequest request, IDictionary dictionary)
        {
            ApplyParameters(request.Context.Cookies, dictionary);
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="anonymousClass"></param>
        /// <returns></returns>
        public static IConsumerRequest WithCookies(this IConsumerRequest request, object anonymousClass)
        {
            ApplyParameters(request.Context.Cookies, anonymousClass);
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static IConsumerRequest WithHeaders(this IConsumerRequest request, IDictionary dictionary)
        {
            ApplyParameters(request.Context.Headers, dictionary);
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="anonymousClass"></param>
        /// <returns></returns>
        public static IConsumerRequest WithHeaders(this IConsumerRequest request, object anonymousClass)
        {
            ApplyParameters(request.Context.Headers, anonymousClass);
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="alteration"></param>
        /// <returns></returns>
        public static IConsumerRequest AlterContext(this IConsumerRequest request, Action<IOAuthContext> alteration)
        {
            alteration(request.Context);
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string ReadBody(this IConsumerRequest request)
        {
            HttpWebResponse response = request.ToWebResponse();

            return response.ReadToEnd();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="selectFunc"></param>
        /// <returns></returns>
        public static T Select<T>(this IConsumerRequest request, Func<NameValueCollection, T> selectFunc)
        {
            try
            {
                return selectFunc(request.ToBodyParameters());
            }
            catch (ArgumentNullException argumentException)
            {
                if (argumentException.Message.Contains("Value cannot be null.\r\nParameter name: str"))
                {
                    throw Error.ExperiencingIssueWithCreatingUriDueToMissingAppConfig(argumentException);
                }

                throw Error.FailedToParseResponse(request.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static IConsumerRequest IncludeRequestBodyHash(this IConsumerRequest request)
        {
            request.Context.IncludeOAuthRequestBodyHashInSignature = true;
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="rawContent"></param>
        /// <param name="encoding"></param>
        /// <param name="addHash"></param>
        /// <returns></returns>
        public static IConsumerRequest WithRawContent(this IConsumerRequest request, string rawContent, Encoding encoding, bool addHash)
        {
            return WithRawContent(request, encoding.GetBytes(rawContent), addHash);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="rawContent"></param>
        /// <param name="addHash"></param>
        /// <returns></returns>
        public static IConsumerRequest WithRawContent(this IConsumerRequest request, byte[] rawContent, bool addHash)
        {
            request.Context.RawContent = rawContent;
            request.Context.IncludeOAuthRequestBodyHashInSignature = addHash;
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="rawContent"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static IConsumerRequest WithRawContent(this IConsumerRequest request, string rawContent, Encoding encoding)
        {
            return WithRawContent(request, encoding.GetBytes(rawContent));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="rawContent"></param>
        /// <returns></returns>
        public static IConsumerRequest WithRawContent(this IConsumerRequest request, byte[] rawContent)
        {
            request.Context.RawContent = rawContent;
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="rawContentType"></param>
        /// <returns></returns>
        public static IConsumerRequest WithRawContentType(this IConsumerRequest request, string rawContentType)
        {
            request.Context.RawContentType = rawContentType;
            return request;
        }
    }
}
