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


namespace Nequeo.Net.Core.Messaging
{
	using System;
	using System.Collections.ObjectModel;
	using System.Diagnostics.Contracts;

	/// <summary>
	/// A KeyedCollection whose item -&gt; key transform is provided via a delegate
	/// to its constructor, and null items are disallowed.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TItem">The type of the item.</typeparam>
	[Serializable]
    public class KeyedCollectionDelegate<TKey, TItem> : KeyedCollection<TKey, TItem>
    {
		/// <summary>
		/// The delegate that returns a key for the given item.
		/// </summary>
		private Func<TItem, TKey> getKeyForItemDelegate;

		/// <summary>
		/// Initializes a new instance of the KeyedCollectionDelegate class.
		/// </summary>
		/// <param name="getKeyForItemDelegate">The delegate that gets the key for a given item.</param>
        public KeyedCollectionDelegate(Func<TItem, TKey> getKeyForItemDelegate)
        {
			Requires.NotNull(getKeyForItemDelegate, "getKeyForItemDelegate");

			this.getKeyForItemDelegate = getKeyForItemDelegate;
		}

		/// <summary>
		/// When implemented in a derived class, extracts the key from the specified element.
		/// </summary>
		/// <param name="item">The element from which to extract the key.</param>
		/// <returns>The key for the specified element.</returns>
		protected override TKey GetKeyForItem(TItem item) {
			ErrorUtilities.VerifyArgumentNotNull(item, "item"); // null items not supported.
			return this.getKeyForItemDelegate(item);
		}
	}
}
