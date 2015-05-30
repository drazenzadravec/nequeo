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

namespace Nequeo.Net.OAuth2.Storage.Basic
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

    using Nequeo.Net.Core;
    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.Core.Messaging.Bindings;

	/// <summary>
	/// An in-memory nonce store.  Useful for single-server web applications.
	/// NOT for web farms.
	/// </summary>
	public class NonceMemoryStore : INonceStore {
		/// <summary>
		/// How frequently we should take time to clear out old nonces.
		/// </summary>
		private const int AutoCleaningFrequency = 10;

		/// <summary>
		/// The maximum age a message can be before it is discarded.
		/// </summary>
		/// <remarks>
		/// This is useful for knowing how long used nonces must be retained.
		/// </remarks>
		private readonly TimeSpan maximumMessageAge;

		/// <summary>
		/// A list of the consumed nonces.
		/// </summary>
		private readonly SortedDictionary<DateTime, List<string>> usedNonces = new SortedDictionary<DateTime, List<string>>();

		/// <summary>
		/// A lock object used around accesses to the <see cref="usedNonces"/> field.
		/// </summary>
		private object nonceLock = new object();

		/// <summary>
		/// Where we're currently at in our periodic nonce cleaning cycle.
		/// </summary>
		private int nonceClearingCounter;

		/// <summary>
		/// Initializes a new instance of the <see cref="NonceMemoryStore"/> class.
		/// </summary>
		/// <param name="maximumMessageAge">The maximum age a message can be before it is discarded.</param>
        public NonceMemoryStore(TimeSpan maximumMessageAge)
        {
			this.maximumMessageAge = maximumMessageAge;
		}

		/// <summary>
		/// Stores a given nonce and timestamp.
		/// </summary>
		/// <param name="context">The context, or namespace, within which the <paramref name="nonce"/> must be unique.</param>
		/// <param name="nonce">A series of random characters.</param>
		/// <param name="timestamp">The timestamp that together with the nonce string make it unique.
		/// The timestamp may also be used by the data store to clear out old nonces.</param>
		/// <returns>
		/// True if the nonce+timestamp (combination) was not previously in the database.
		/// False if the nonce was stored previously with the same timestamp.
		/// </returns>
		/// <remarks>
		/// The nonce must be stored for no less than the maximum time window a message may
		/// be processed within before being discarded as an expired message.
		/// If the binding element is applicable to your channel, this expiration window
		/// is retrieved or set using the property.
		/// </remarks>
		public bool StoreNonce(DateTime timestamp, string nonce, string context) {
			if (timestamp.ToUniversalTimeSafe() + this.maximumMessageAge < DateTime.UtcNow) {
				// The expiration binding element should have taken care of this, but perhaps
				// it's at the boundary case.  We should fail just to be safe.
				return false;
			}

			// We just concatenate the context with the nonce to form a complete, namespace-protected nonce.
			string completeNonce = context + "\0" + nonce;

			lock (this.nonceLock) {
				List<string> nonces;
				if (!this.usedNonces.TryGetValue(timestamp, out nonces)) {
					this.usedNonces[timestamp] = nonces = new List<string>(4);
				}

				if (nonces.Contains(completeNonce)) {
					return false;
				}

				nonces.Add(completeNonce);

				// Clear expired nonces if it's time to take a moment to do that.
				// Unchecked so that this can int overflow without an exception.
				unchecked {
					this.nonceClearingCounter++;
				}
				if (this.nonceClearingCounter % AutoCleaningFrequency == 0) {
					this.ClearExpiredNonces();
				}

				return true;
			}
		}

        /// <summary>
        /// Genrate a new nonce.
        /// </summary>
        /// <param name="size">The size of the nonce.</param>
        /// <returns>The nonce data.</returns>
        public string GenerateNonce(int size = 30)
        {
            Nequeo.Invention.TokenGenerator token = new Invention.TokenGenerator();
            return token.Random(size);
        }

		/// <summary>
		/// Clears consumed nonces from the cache that are so old they would be
		/// rejected if replayed because it is expired.
		/// </summary>
		public void ClearExpiredNonces() {
			lock (this.nonceLock) {
				var oldNonceLists = this.usedNonces.Keys.Where(time => time.ToUniversalTimeSafe() + this.maximumMessageAge < DateTime.UtcNow).ToList();
				foreach (DateTime time in oldNonceLists) {
					this.usedNonces.Remove(time);
				}

				// Reset the auto-clean counter so that if this method was called externally
				// we don't auto-clean right away.
				this.nonceClearingCounter = 0;
			}
		}
	}
}
