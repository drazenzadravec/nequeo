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
    /// This enumeration describes list of media events.
    /// </summary>
    public enum MediaEventType
    {
        /// <summary>
        /// No event.
        /// </summary>
        PJMEDIA_EVENT_NONE = 0,
        /// <summary>
        /// Media format has changed event.
        /// </summary>
        PJMEDIA_EVENT_FMT_CHANGED = 1179468616,
        /// <summary>
        /// Video keyframe has just been decoded event.
        /// </summary>
        PJMEDIA_EVENT_KEYFRAME_FOUND = 1229345350,
        /// <summary>
        /// Video decoding error due to missing keyframe event.
        /// </summary>
        PJMEDIA_EVENT_KEYFRAME_MISSING = 1229345357,
        /// <summary>
        /// Mouse button has been pressed event.
        /// </summary>
        PJMEDIA_EVENT_MOUSE_BTN_DOWN = 1297302606,
        /// <summary>
        /// Video orientation has been changed event.
        /// </summary>
        PJMEDIA_EVENT_ORIENT_CHANGED = 1330794068,
        /// <summary>
        /// Video window is being closed.
        /// </summary>
        PJMEDIA_EVENT_WND_CLOSING = 1464746828,
        /// <summary>
        /// Video window has been closed event.
        /// </summary>
        PJMEDIA_EVENT_WND_CLOSED = 1464746831,
        /// <summary>
        /// Video window has been resized event.
        /// </summary>
        PJMEDIA_EVENT_WND_RESIZED = 1464750682
    }
}
