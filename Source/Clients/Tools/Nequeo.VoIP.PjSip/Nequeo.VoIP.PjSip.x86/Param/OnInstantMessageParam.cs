/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
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

namespace Nequeo.VoIP.PjSip.Param
{
    /// <summary>
    /// Notify application on incoming instant message or pager (i.e. MESSAGE
    /// request) that was received outside call context.
    /// </summary>
    public class OnInstantMessageParam
    {
        /// <summary>
        /// Gets or sets a short info string describing the request, which normally contains the request method and its CSeq.
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// Gets or sets the source address of the message.
        /// </summary>
        public string SrcAddress { get; set; }

        /// <summary>
        /// Gets or sets the whole message data as a string, containing both the header section and message body section.
        /// </summary>
        public string WholeMsg { get; set; }

        /// <summary>
        /// Gets or sets the contact URI of the sender.
        /// </summary>
        public string ContactUri { get; set; }

        /// <summary>
        /// Gets or sets the MIME type of the message body.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the sender from URI.
        /// </summary>
        public string FromUri { get; set; }

        /// <summary>
        /// Gets or sets the message body.
        /// </summary>
        public string MsgBody { get; set; }

        /// <summary>
        /// Gets or sets the to URI of the request.
        /// </summary>
        public string ToUri { get; set; }
    }
}
