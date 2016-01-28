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
    /// Contact information.
    /// </summary>
    public class ContactInfo
    {
        /// <summary>
        /// Contact information.
        /// </summary>
        public ContactInfo() { }

        /// <summary>
        /// Gets or sets the full URI of the contact.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the contact info, only available when presence subscription has
        /// been established to the buddy.
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// Gets or sets a flag to indicate that we should monitor the presence information for
        /// this buddy(normally yes, unless explicitly disabled).
        /// </summary>
        public bool PresMonitorEnabled { get; set; }

        /// <summary>
        /// Gets or sets If PresMonitorEnabled is true, this specifies the last state of
        /// the presence subscription. If presence subscription session is currently
        /// active, the value will be EVSUB_STATE_ACTIVE.If presence
        /// subscription request has been rejected, the value will be
        /// EVSUB_STATE_TERMINATED, and the termination reason will be
        /// specified in SubTermReason.
        /// </summary>
        public SubscriptionState SubState { get; set; }

        /// <summary>
        /// Gets or sets the representation of subscription state.
        /// </summary>
        public string SubStateName { get; set; }

        /// <summary>
        /// Gets or sets the specifies the last presence subscription termination code. This would
        /// return the last status of the SUBSCRIBE request.If the subscription
        /// is terminated with NOTIFY by the server, this value will be set to
        /// 200, and subscription termination reason will be given in the
        /// SubTermReason field.
        /// </summary>
        public StatusCode SubTermCode { get; set; }

        /// <summary>
        /// Gets or sets the Specifies the last presence subscription termination reason. If 
        /// presence subscription is currently active, the value will be empty.
        /// </summary>
        public string SubTermReason { get; set; }

        /// <summary>
        /// Gets or sets the presence status.
        /// </summary>
        public PresenceStatus PresenceStatus { get; set; }
    }
}
