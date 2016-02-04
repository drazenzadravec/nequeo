/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Threading.Tasks;

namespace Nequeo.Net.Sip
{
    /// <summary>
    /// These enumerations specify the action to be performed to a redirect response.
    /// </summary>
    public enum RedirectResponseType
    {
        /// <summary>
        /// Reject the redirection to the current target. The UAC will
        /// select the next target from the target set if exists.
        /// </summary>
        PJSIP_REDIRECT_REJECT,
        /// <summary>
        /// Accept the redirection to the current target. The INVITE request
        /// will be resent to the current target.
        /// </summary>
        PJSIP_REDIRECT_ACCEPT,
        /// <summary>
        /// Accept the redirection to the current target and replace the To
        /// header in the INVITE request with the current target. The INVITE
        /// request will be resent to the current target.
        /// </summary>
        PJSIP_REDIRECT_ACCEPT_REPLACE,
        /// <summary>
        /// Defer the redirection decision, for example to request permission
        /// from the end user.
        /// </summary>
        PJSIP_REDIRECT_PENDING,
        /// <summary>
        /// Stop the whole redirection process altogether. This will cause
        /// the invite session to be disconnected.
        /// </summary>
        PJSIP_REDIRECT_STOP
    }
}
