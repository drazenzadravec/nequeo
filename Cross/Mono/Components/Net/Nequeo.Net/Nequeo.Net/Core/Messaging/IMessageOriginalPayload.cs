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
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Text;

	/// <summary>
	/// An interface that appears on messages that need to retain a description of
	/// what their literal payload was when they were deserialized.
	/// </summary>
	[ContractClass(typeof(IMessageOriginalPayloadContract))]
	public interface IMessageOriginalPayload {
		/// <summary>
		/// Gets or sets the original message parts, before any normalization or default values were assigned.
		/// </summary>
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "By design")]
		IDictionary<string, string> OriginalPayload { get; set; }
	}

	/// <summary>
	/// Code contract for the <see cref="IMessageOriginalPayload"/> interface.
	/// </summary>
	[ContractClassFor(typeof(IMessageOriginalPayload))]
    public abstract class IMessageOriginalPayloadContract : IMessageOriginalPayload
    {
		/// <summary>
		/// Gets or sets the original message parts, before any normalization or default values were assigned.
		/// </summary>
        public IDictionary<string, string> OriginalPayload
        {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
	}
}
