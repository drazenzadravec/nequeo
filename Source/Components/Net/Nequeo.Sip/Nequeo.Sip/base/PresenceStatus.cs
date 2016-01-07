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
    /// Presence status.
    /// </summary>
    public class PresenceStatus
    {
        /// <summary>
        /// Presence status.
        /// </summary>
        public PresenceStatus() { }

        /// <summary>
        /// Gets or sets the activity type.
        /// </summary>
        public RpidActivity Activity { get; set; }

        /// <summary>
        /// Gets or sets the optional text describing the person/element.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the optional RPID ID string.
        /// </summary>
        public string RpidId { get; set; }

        /// <summary>
        /// Gets or sets the contact's online status.
        /// </summary>
        public ContactStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the text to describe contact's online status.
        /// </summary>
        public string StatusText { get; set; }

        /// <summary>
        /// Get the contact status.
        /// </summary>
        /// <param name="status">The current contact status.</param>
        /// <returns>The contact status.</returns>
        internal static ContactStatus GetContactStatusEx(pjsua2.pjsua_buddy_status status)
        {
            // Select the status.
            switch (status)
            {
                case pjsua2.pjsua_buddy_status.PJSUA_BUDDY_STATUS_OFFLINE:
                    return ContactStatus.OFFLINE;
                case pjsua2.pjsua_buddy_status.PJSUA_BUDDY_STATUS_ONLINE:
                    return ContactStatus.ONLINE;
                default:
                    return ContactStatus.UNKNOWN;
            }
        }

        /// <summary>
        /// Get the contact status.
        /// </summary>
        /// <param name="status">The current contact status.</param>
        /// <returns>The contact status.</returns>
        internal static pjsua2.pjsua_buddy_status GetContactStatus(ContactStatus status)
        {
            // Select the status.
            switch (status)
            {
                case ContactStatus.OFFLINE:
                    return pjsua2.pjsua_buddy_status.PJSUA_BUDDY_STATUS_OFFLINE;
                case ContactStatus.ONLINE:
                    return pjsua2.pjsua_buddy_status.PJSUA_BUDDY_STATUS_ONLINE;
                default:
                    return pjsua2.pjsua_buddy_status.PJSUA_BUDDY_STATUS_UNKNOWN;
            }
        }

        /// <summary>
        /// Get the activity.
        /// </summary>
        /// <param name="activity">The current activity.</param>
        /// <returns>The activity.</returns>
        internal static RpidActivity GetActivityEx(pjsua2.pjrpid_activity activity)
        {
            // Select the activity.
            switch (activity)
            {
                case pjsua2.pjrpid_activity.PJRPID_ACTIVITY_AWAY:
                    return RpidActivity.AWAY;
                case pjsua2.pjrpid_activity.PJRPID_ACTIVITY_BUSY:
                    return RpidActivity.BUSY;
                default:
                    return RpidActivity.UNKNOWN;
            }
        }

        /// <summary>
        /// Get the activity.
        /// </summary>
        /// <param name="activity">The current activity.</param>
        /// <returns>The activity.</returns>
        internal static pjsua2.pjrpid_activity GetActivity(RpidActivity activity)
        {
            // Select the activity.
            switch (activity)
            {
                case RpidActivity.AWAY:
                    return pjsua2.pjrpid_activity.PJRPID_ACTIVITY_AWAY;
                case RpidActivity.BUSY:
                    return pjsua2.pjrpid_activity.PJRPID_ACTIVITY_BUSY;
                default:
                    return pjsua2.pjrpid_activity.PJRPID_ACTIVITY_UNKNOWN;
            }
        }
    }
}
