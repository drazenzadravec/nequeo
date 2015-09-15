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

namespace Nequeo.Net.Core.Messaging.Reflection
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	/// <summary>
	/// Allows a custom class or struct to be serializable between itself and a string representation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class DefaultEncoderAttribute : Attribute
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultEncoderAttribute"/> class.
		/// </summary>
		/// <param name="converterType">The <see cref="IMessagePartEncoder"/> implementing type to use for serializing this type.</param>
		public DefaultEncoderAttribute(Type converterType) {
			Requires.NotNull(converterType, "converterType");
			Requires.True(typeof(IMessagePartEncoder).IsAssignableFrom(converterType), "Argument must be a type that implements {0}.", typeof(IMessagePartEncoder).Name);
			this.Encoder = (IMessagePartEncoder)Activator.CreateInstance(converterType);
		}

		/// <summary>
		/// Gets the default encoder to use for the declaring class.
		/// </summary>
		public IMessagePartEncoder Encoder { get; private set; }
	}
}
