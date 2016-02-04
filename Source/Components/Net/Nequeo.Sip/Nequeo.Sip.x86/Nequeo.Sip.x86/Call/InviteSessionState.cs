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
    /// Describes invite session state.
    /// </summary>
    public enum InviteSessionState
    {
        ///	<summary>
        /// Before INVITE is sent or received.
        ///	</summary>
        PJSIP_INV_STATE_NULL,
        ///	<summary>
        /// After INVITE is sent.
        ///	</summary>
        PJSIP_INV_STATE_CALLING,
        ///	<summary>
        /// After INVITE is received.
        ///	</summary>
        PJSIP_INV_STATE_INCOMING,
        ///	<summary>
        /// After response with To tag.
        ///	</summary>
        PJSIP_INV_STATE_EARLY,
        ///	<summary>
        /// After 2xx is sent/received.
        ///	</summary>
        PJSIP_INV_STATE_CONNECTING,
        ///	<summary>
        /// After ACK is sent/received.
        ///	</summary>
        PJSIP_INV_STATE_CONFIRMED,
        ///	<summary>
        /// Session is terminated.
        ///	</summary>
        PJSIP_INV_STATE_DISCONNECTED,
    }
}
