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

using System.Diagnostics;

namespace Nequeo.Web.Mvc.Extended.UI
{
    /// <summary>
    /// Wrap the script for the jQuery ready/unload events.
    /// </summary>
    public class ScriptWrapper : ScriptWrapperBase
    {
        /// <summary>
        /// Gets the on page load start.
        /// </summary>
        /// <value>The on page load start.</value>
        public override string OnPageLoadStart
        {
            get { return "jQuery(document).ready(function(){"; }
        }

        /// <summary>
        /// Gets the on page load end.
        /// </summary>
        /// <value>The on page load end.</value>
        public override string OnPageLoadEnd
        {
            get { return "});"; }
        }

        /// <summary>
        /// Gets the on page unload start.
        /// </summary>
        /// <value>The on page unload start.</value>
        public override string OnPageUnloadStart
        {
            get { return "jQuery(window).unload(function(){"; }
        }

        /// <summary>
        /// Gets the on page unload end.
        /// </summary>
        /// <value>The on page unload end.</value>
        public override string OnPageUnloadEnd
        {
            get { return "});"; }
        }
    }
}