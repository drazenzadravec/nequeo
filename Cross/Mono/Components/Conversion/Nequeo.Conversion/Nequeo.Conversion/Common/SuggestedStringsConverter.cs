/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
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
using System.Linq;
using System.Text;

namespace Nequeo.Conversion.Common
{
	using System;
	using System.Collections;
	using System.ComponentModel.Design.Serialization;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	/// A type that generates suggested strings for Intellisense,
	/// but doesn't actually convert between strings and other types.
	/// </summary>
	[ContractClass(typeof(SuggestedStringsConverterContract))]
	public abstract class SuggestedStringsConverter : ConverterBase<string> {
		/// <summary>
		/// Initializes a new instance of the <see cref="SuggestedStringsConverter"/> class.
		/// </summary>
		protected SuggestedStringsConverter() {
		}

		/// <summary>
		/// Gets the type to reflect over for the well known values.
		/// </summary>
		[Pure]
		protected abstract Type WellKnownValuesType { get; }

		/// <summary>
		/// Gets the values of public static fields and properties on a given type.
		/// </summary>
		/// <param name="type">The type to reflect over.</param>
		/// <returns>A collection of values.</returns>
		internal static ICollection GetStandardValuesForCacheShared(Type type) {
			Contract.Ensures(Contract.Result<ICollection>() != null);

			var fields = from field in type.GetFields(BindingFlags.Static | BindingFlags.Public)
						 select field.GetValue(null);
			var properties = from prop in type.GetProperties(BindingFlags.Static | BindingFlags.Public)
							 select prop.GetValue(null, null);
			return fields.Concat(properties).ToArray();
		}

		/// <summary>
		/// Converts a value from its string representation to its strongly-typed object.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The strongly-typed object.</returns>
		[Pure]
		protected override string ConvertFrom(string value) {
			return value;
		}

		/// <summary>
		/// Creates the reflection instructions for recreating an instance later.
		/// </summary>
		/// <param name="value">The value to recreate later.</param>
		/// <returns>
		/// The description of how to recreate an instance.
		/// </returns>
		[Pure]
		protected override InstanceDescriptor CreateFrom(string value) {
			// No implementation necessary since we're only dealing with strings.
			throw new NotImplementedException();
		}

		/// <summary>
		/// Converts the strongly-typed value to a string.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>The string representation of the object.</returns>
		[Pure]
		protected override string ConvertToString(string value) {
			return value;
		}

		/// <summary>
		/// Gets the standard values to suggest with Intellisense in the designer.
		/// </summary>
		/// <returns>A collection of the standard values.</returns>
		[Pure]
		protected override ICollection GetStandardValuesForCache() {
			return GetStandardValuesForCacheShared(this.WellKnownValuesType);
		}
	}
}
