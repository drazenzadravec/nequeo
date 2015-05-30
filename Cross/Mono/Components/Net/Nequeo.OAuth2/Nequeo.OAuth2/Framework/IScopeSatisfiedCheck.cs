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

namespace Nequeo.Net.OAuth2.Framework
{
	using System.Collections.Generic;

	/// <summary>
	/// An extensibility point that allows authorization servers and resource servers to customize how scopes may be considered
	/// supersets of each other.
	/// </summary>
	/// <remarks>
	/// Implementations must be thread-safe.
	/// </remarks>
	public interface IScopeSatisfiedCheck {
		/// <summary>
		/// Checks whether the granted scope is a superset of the required scope.
		/// </summary>
		/// <param name="requiredScope">The set of strings that the resource server demands in an access token's scope in order to complete some operation.</param>
		/// <param name="grantedScope">The set of strings that define the scope within an access token that the client is authorized to.</param>
		/// <returns><c>true</c> if <paramref name="grantedScope"/> is a superset of <paramref name="requiredScope"/> to allow the request to proceed; <c>false</c> otherwise.</returns>
		/// <remarks>
		/// The default reasonable implementation of this is:
		/// <code>
		///     return <paramref name="grantedScope"/>.IsSupersetOf(<paramref name="requiredScope"/>);
		/// </code>
		/// <para>In some advanced cases it may not be so simple.  One case is that there may be a string that aggregates the capabilities of several others
		/// in order to simplify common scenarios.  For example, the scope "ReadAll" may represent the same authorization as "ReadProfile", "ReadEmail", and 
		/// "ReadFriends".
		/// </para>
		/// <para>Great care should be taken in implementing this method as this is a critical security module for the authorization and resource servers.</para>
		/// </remarks>
		bool IsScopeSatisfied(HashSet<string> requiredScope, HashSet<string> grantedScope);
	}
}
