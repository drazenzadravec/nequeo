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
	using System.Collections;

	/// <summary>
	/// An enumerator that always generates zero elements.
	/// </summary>
    public class EmptyEnumerator : IEnumerator
    {
		/// <summary>
		/// The singleton instance of this empty enumerator.
		/// </summary>
        public static readonly EmptyEnumerator Instance = new EmptyEnumerator();

		/// <summary>
		/// Prevents a default instance of the <see cref="EmptyEnumerator"/> class from being created.
		/// </summary>
		private EmptyEnumerator() {
		}

		#region IEnumerator Members

		/// <summary>
		/// Gets the current element in the collection.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The current element in the collection.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">
		/// The enumerator is positioned before the first element of the collection or after the last element.
		/// </exception>
		public object Current {
			get { return null; }
		}

		/// <summary>
		/// Advances the enumerator to the next element of the collection.
		/// </summary>
		/// <returns>
		/// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">
		/// The collection was modified after the enumerator was created.
		/// </exception>
		public bool MoveNext() {
			return false;
		}

		/// <summary>
		/// Sets the enumerator to its initial position, which is before the first element in the collection.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">
		/// The collection was modified after the enumerator was created.
		/// </exception>
		public void Reset() {
		}

		#endregion
	}
}
