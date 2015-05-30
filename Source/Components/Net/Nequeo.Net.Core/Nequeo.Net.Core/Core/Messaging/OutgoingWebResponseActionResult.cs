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
	using System.Diagnostics.Contracts;
	using System.Web.Mvc;
    using Nequeo.Net.Core.Messaging;

	/// <summary>
	/// An ASP.NET MVC structure to represent the response to send
	/// to the user agent when the controller has finished its work.
	/// </summary>
    public class OutgoingWebResponseActionResult : ActionResult
    {
		/// <summary>
		/// The outgoing web response to send when the ActionResult is executed.
		/// </summary>
		private readonly OutgoingWebResponse response;

		/// <summary>
		/// Initializes a new instance of the <see cref="OutgoingWebResponseActionResult"/> class.
		/// </summary>
		/// <param name="response">The response.</param>
        public OutgoingWebResponseActionResult(OutgoingWebResponse response)
        {
			Requires.NotNull(response, "response");
			this.response = response;
		}

		/// <summary>
		/// Enables processing of the result of an action method by a custom type that inherits from <see cref="T:System.Web.Mvc.ActionResult"/>.
		/// </summary>
		/// <param name="context">The context in which to set the response.</param>
		public override void ExecuteResult(ControllerContext context) {
			this.response.Respond(context.HttpContext);

			// MVC likes to muck with our response.  For example, when returning contrived 401 Unauthorized responses
			// MVC will rewrite our response and turn it into a redirect, which breaks OAuth 2 authorization server token endpoints.
			// It turns out we can prevent this unwanted behavior by flushing the response before returning from this method.
			context.HttpContext.Response.Flush();
		}
	}
}
