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
	using System.Diagnostics;
	using System.Web;
	using System.Web.Security;

	/// <summary>
	/// Helper methods for setting and retrieving a custom forms authentication ticket for delegation protocols.
	/// </summary>
	internal static class OpenAuthAuthenticationTicketHelper {
		
		/// <summary>
		/// The open auth cookie token.
		/// </summary>
		private const string OpenAuthCookieToken = "OAuth";

		/// <summary>
		/// Checks whether the specified HTTP request comes from an authenticated user.
		/// </summary>
		/// <param name="context">
		/// The context.
		/// </param>
		/// <returns>True if the reuest is authenticated; false otherwise.</returns>
		public static bool IsValidAuthenticationTicket(HttpContextBase context) {
			HttpCookie cookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];
			if (cookie == null) {
				return false;
			}

			string encryptedCookieData = cookie.Value;
			if (string.IsNullOrEmpty(encryptedCookieData)) {
				return false;
			}

			try {
				FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(encryptedCookieData);
				return authTicket != null && !authTicket.Expired && authTicket.UserData == OpenAuthCookieToken;
			} catch (ArgumentException) {
				return false;
			}
		}

		/// <summary>
		/// Adds an authentication cookie to the user agent in the next HTTP response.
		/// </summary>
		/// <param name="context">
		/// The context.
		/// </param>
		/// <param name="userName">
		/// The user name.
		/// </param>
		/// <param name="createPersistentCookie">
		/// A value indicating whether the cookie should persist across sessions.
		/// </param>
		public static void SetAuthenticationTicket(HttpContextBase context, string userName, bool createPersistentCookie) {
			if (!context.Request.IsSecureConnection && FormsAuthentication.RequireSSL) {
				throw new HttpException(WebResources.ConnectionNotSecure);
			}

			HttpCookie cookie = GetAuthCookie(userName, createPersistentCookie);
			context.Response.Cookies.Add(cookie);
		}

		/// <summary>
		/// Creates an HTTP authentication cookie.
		/// </summary>
		/// <param name="userName">
		/// The user name.
		/// </param>
		/// <param name="createPersistentCookie">
		/// A value indicating whether the cookie should last across sessions.
		/// </param>
		/// <returns>An authentication cookie.</returns>
		private static HttpCookie GetAuthCookie(string userName, bool createPersistentCookie) {

			var ticket = new FormsAuthenticationTicket(
				/* version */
				2,
				userName,
				DateTime.Now,
				DateTime.Now.Add(FormsAuthentication.Timeout),
				createPersistentCookie,
				OpenAuthCookieToken,
				FormsAuthentication.FormsCookiePath);

			string encryptedTicket = FormsAuthentication.Encrypt(ticket);
			if (encryptedTicket == null || encryptedTicket.Length < 1) {
				throw new HttpException(WebResources.FailedToEncryptTicket);
			}

			var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket) {
				HttpOnly = true,
				Path = FormsAuthentication.FormsCookiePath,
				Secure = FormsAuthentication.RequireSSL
			};

			if (FormsAuthentication.CookieDomain != null) {
				cookie.Domain = FormsAuthentication.CookieDomain;
			}

			if (ticket.IsPersistent) {
				cookie.Expires = ticket.Expiration;
			}

			return cookie;
		}
	}
}
