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

namespace Nequeo.Net.Core.Messaging.Bindings
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Text;
    using Nequeo.Net.Core.Messaging;

	/// <summary>
	/// A cryptographic key and metadata concerning it.
	/// </summary>
	public class CryptoKey {
		/// <summary>
		/// Backing field for the <see cref="Key"/> property.
		/// </summary>
		private readonly byte[] key;

		/// <summary>
		/// Backing field for the <see cref="ExpiresUtc"/> property.
		/// </summary>
		private readonly DateTime expiresUtc;

		/// <summary>
		/// Initializes a new instance of the <see cref="CryptoKey"/> class.
		/// </summary>
		/// <param name="key">The cryptographic key.</param>
		/// <param name="expiresUtc">The expires UTC.</param>
		public CryptoKey(byte[] key, DateTime expiresUtc) {
			Requires.NotNull(key, "key");
			Requires.True(expiresUtc.Kind == DateTimeKind.Utc, "expiresUtc");
			this.key = key;
			this.expiresUtc = expiresUtc;
		}

		/// <summary>
		/// Gets the key.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "It's a buffer")]
		public byte[] Key {
			get {
				Contract.Ensures(Contract.Result<byte[]>() != null);
				return this.key;
			}
		}

		/// <summary>
		/// Gets the expiration date of this key (UTC time).
		/// </summary>
		public DateTime ExpiresUtc {
			get {
				Contract.Ensures(Contract.Result<DateTime>().Kind == DateTimeKind.Utc);
				return this.expiresUtc;
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="T:System.NullReferenceException">
		/// The <paramref name="obj"/> parameter is null.
		/// </exception>
		public override bool Equals(object obj) {
			var other = obj as CryptoKey;
			if (other == null) {
				return false;
			}

			return this.ExpiresUtc == other.ExpiresUtc
				&& MessagingUtilities.AreEquivalent(this.Key, other.Key);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode() {
			return this.ExpiresUtc.GetHashCode();
		}
	}
}
