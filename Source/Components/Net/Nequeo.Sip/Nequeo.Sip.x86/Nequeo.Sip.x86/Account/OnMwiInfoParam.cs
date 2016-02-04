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
    /// Parameters for OnMwiInfo() account callback.
    /// </summary>
    public class OnMwiInfoParam
    {
        /// <summary>
        /// Gets or sets the incoming response that causes this callback to be called.
        /// If the transaction fails because of time out or transport error,
        /// the content will be empty.
        /// </summary>
        public SipRxData RxData { get; set; }

        /// <summary>
        /// Gets or sets the MWI subscription state.
        /// </summary>
        public SubscriptionState State { get; set; }

        /// <summary>
        /// Get subscription state.
        /// </summary>
        /// <param name="subscriptionState">The current subscription state.</param>
        /// <returns>The subscription state.</returns>
        internal static pjsua2.pjsip_evsub_state GetSubscriptionStateEx(SubscriptionState subscriptionState)
        {
            // Select the state.
            switch (subscriptionState)
            {
                case SubscriptionState.EVSUB_STATE_ACCEPTED:
                    return pjsua2.pjsip_evsub_state.PJSIP_EVSUB_STATE_ACCEPTED;
                case SubscriptionState.EVSUB_STATE_ACTIVE:
                    return pjsua2.pjsip_evsub_state.PJSIP_EVSUB_STATE_ACTIVE;
                case SubscriptionState.EVSUB_STATE_NULL:
                    return pjsua2.pjsip_evsub_state.PJSIP_EVSUB_STATE_NULL;
                case SubscriptionState.EVSUB_STATE_PENDING:
                    return pjsua2.pjsip_evsub_state.PJSIP_EVSUB_STATE_PENDING;
                case SubscriptionState.EVSUB_STATE_SENT:
                    return pjsua2.pjsip_evsub_state.PJSIP_EVSUB_STATE_SENT;
                case SubscriptionState.EVSUB_STATE_TERMINATED:
                    return pjsua2.pjsip_evsub_state.PJSIP_EVSUB_STATE_TERMINATED;
                default:
                    return pjsua2.pjsip_evsub_state.PJSIP_EVSUB_STATE_UNKNOWN;
            }
        }

        /// <summary>
        /// Get subscription state.
        /// </summary>
        /// <param name="subscriptionState">The current subscription state.</param>
        /// <returns>The subscription state.</returns>
        internal static SubscriptionState GetSubscriptionState(pjsua2.pjsip_evsub_state subscriptionState)
        {
            // Select the state.
            switch (subscriptionState)
            {
                case pjsua2.pjsip_evsub_state.PJSIP_EVSUB_STATE_ACCEPTED:
                    return SubscriptionState.EVSUB_STATE_ACCEPTED;
                case pjsua2.pjsip_evsub_state.PJSIP_EVSUB_STATE_ACTIVE:
                    return SubscriptionState.EVSUB_STATE_ACTIVE;
                case pjsua2.pjsip_evsub_state.PJSIP_EVSUB_STATE_NULL:
                    return SubscriptionState.EVSUB_STATE_NULL;
                case pjsua2.pjsip_evsub_state.PJSIP_EVSUB_STATE_PENDING:
                    return SubscriptionState.EVSUB_STATE_PENDING;
                case pjsua2.pjsip_evsub_state.PJSIP_EVSUB_STATE_SENT:
                    return SubscriptionState.EVSUB_STATE_SENT;
                case pjsua2.pjsip_evsub_state.PJSIP_EVSUB_STATE_TERMINATED:
                    return SubscriptionState.EVSUB_STATE_TERMINATED;
                default:
                    return SubscriptionState.EVSUB_STATE_UNKNOWN;
            }
        }
    }
}
