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

namespace Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages
{
	/// <summary>
	/// The types of authorizations that a client can use to obtain
	/// a refresh token and/or an access token.
	/// </summary>
	internal enum GrantType {
		/// <summary>
		/// The client is providing the authorization code previously obtained from an end user authorization response.
		/// </summary>
		AuthorizationCode,

		/// <summary>
		/// The client is providing the end user's username and password to the authorization server.
		/// </summary>
		Password,

		/// <summary>
		/// The client is providing an assertion it obtained from another source.
		/// </summary>
		Assertion,

		/// <summary>
		/// The client is providing a refresh token.
		/// </summary>
		RefreshToken,

		/// <summary>
		/// No authorization to access a user's data has been given.  The client is requesting
		/// an access token authorized for its own private data.  This fits the classic OAuth 1.0(a) "2-legged OAuth" scenario.
		/// </summary>
		/// <remarks>
		/// When requesting an access token using the none access grant type (no access grant is included), the client is requesting access to the protected resources under its control, or those of another resource owner which has been previously arranged with the authorization server (the method of which is beyond the scope of this specification).
		/// </remarks>
		ClientCredentials,
	}
}
