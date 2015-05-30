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

namespace Nequeo.Net.OAuth2.Framework.Utility
{
	using System;
	using System.Text.RegularExpressions;
	using System.Web;

    using Nequeo.Net.Core.Messaging;

	/// <summary>
	/// The uri helper.
	/// </summary>
	internal static class UriHelper {
		/// <summary>
		/// The attach query string parameter.
		/// </summary>
		/// <param name="url">
		/// The url.
		/// </param>
		/// <param name="parameterName">
		/// The parameter name.
		/// </param>
		/// <param name="parameterValue">
		/// The parameter value.
		/// </param>
		/// <returns>An absolute URI.</returns>
		public static Uri AttachQueryStringParameter(this Uri url, string parameterName, string parameterValue) {
			UriBuilder builder = new UriBuilder(url);
			string query = builder.Query;
			if (query.Length > 1) {
				// remove the '?' character in front of the query string
				query = query.Substring(1);
			}

			string parameterPrefix = parameterName + "=";

			string encodedParameterValue = Uri.EscapeDataString(parameterValue);

			string newQuery = Regex.Replace(query, parameterPrefix + "[^\\&]*", parameterPrefix + encodedParameterValue);
			if (newQuery == query) {
				if (newQuery.Length > 0) {
					newQuery += "&";
				}

				newQuery = newQuery + parameterPrefix + encodedParameterValue;
			}

			builder.Query = newQuery;

			return builder.Uri;
		}

		/// <summary>
		/// Converts an app-relative url, e.g. ~/Content/Return.cshtml, to a full-blown url, e.g. http://mysite.com/Content/Return.cshtml
		/// </summary>
		/// <param name="returnUrl">
		/// The return URL. 
		/// </param>
		/// <param name="context">
		/// The context.
		/// </param>
		/// <returns>An absolute URI.</returns>
		public static Uri ConvertToAbsoluteUri(string returnUrl, HttpContextBase context) {
			if (Uri.IsWellFormedUriString(returnUrl, UriKind.Absolute)) {
				return new Uri(returnUrl, UriKind.Absolute);
			}

			if (!VirtualPathUtility.IsAbsolute(returnUrl)) {
				returnUrl = VirtualPathUtility.ToAbsolute(returnUrl);
			}

			Uri publicUrl = context.Request.GetPublicFacingUrl();
			return new Uri(publicUrl, returnUrl);
		}
	}
}
