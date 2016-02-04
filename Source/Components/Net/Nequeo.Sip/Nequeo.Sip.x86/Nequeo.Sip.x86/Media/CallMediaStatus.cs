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
    /// This enumeration specifies the media status of a call, and it's part
    /// of pjsua_call_info structure.
    /// </summary>
    public enum CallMediaStatus
    {
        ///	<summary>
        /// Call currently has no media, or the media is not used.
        ///	</summary>
        PJSUA_CALL_MEDIA_NONE,

        ///	<summary>
        /// The media is active
        ///	</summary>
        PJSUA_CALL_MEDIA_ACTIVE,

        ///	<summary>
        /// The media is currently put on hold by local endpoint
        ///	</summary>
        PJSUA_CALL_MEDIA_LOCAL_HOLD,

        ///	<summary>
        /// The media is currently put on hold by remote endpoint
        ///	</summary>
        PJSUA_CALL_MEDIA_REMOTE_HOLD,

        ///	<summary>
        /// The media has reported error (e.g. ICE negotiation)
        ///	</summary>
        PJSUA_CALL_MEDIA_ERROR
    }
}
