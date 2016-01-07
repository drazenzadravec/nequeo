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
    /// This structure contains parameters for OnIncomingSubscribe() callback.
    /// </summary>
    public class OnIncomingSubscribeParam
    {
        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        public StatusCode Code { get; set; }

        /// <summary>
        /// Gets or sets the sender URI.
        /// </summary>
        public string FromUri { get; set; }

        /// <summary>
        /// Gets or sets the incoming INVITE request.
        /// </summary>
        public SipRxData RxData { get; set; }

        /// <summary>
        /// Gets or sets the reason phrase to respond to the request.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets additional data to be sent with the response, if any.
        /// </summary>
        public SipTxOption TxOption { get; set; }
    }

    /// <summary>
    /// This structure describes an incoming SIP message. It corresponds to the
    /// rx data structure in SIP library.
    /// </summary>
    public class SipTxOption
    {
        /// <summary>
        /// Gets or sets MIME type of the message body, if application specifies the messageBody
        /// in this structure.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets sip headers.
        /// </summary>
        public SipHeaderVector Headers { get; set; }

        /// <summary>
        /// Gets or sets Optional message body to be added to the message, only when the
        /// message doesn't have a body.
        /// </summary>
        public string MsgBody { get; set; }

        /// <summary>
        /// Gets or sets content type of the multipart body. If application wants to send
        /// multipart message bodies, it puts the parts in multipartParts and set
        /// the content type in multipartContentType.If the message already
        /// contains a body, the body will be added to the multipart bodies.
        /// </summary>
        public SipMediaType MultipartContentType { get; set; }

        /// <summary>
        /// Gets or sets Array of multipart parts. If application wants to send multipart
        /// message bodies, it puts the parts in parts and set the content
        /// type in multipart_ctype.If the message already contains a body,
        /// the body will be added to the multipart bodies.
        /// </summary>
        public SipMultipartPartVector MultipartParts { get; set; }

        /// <summary>
        /// Gets or sets optional remote target URI (i.e. Target header). If empty (""), the
        /// target will be set to the remote URI(To header). At the moment this
        /// field is only used when sending initial INVITE and MESSAGE requests.
        /// </summary>
        public string TargetUri { get; set; }
    }

    /// <summary>
    /// Sip headers.
    /// </summary>
    public class SipHeaderVector
    {
        /// <summary>
        /// Gets or sets the total sip header count.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the sip headers.
        /// </summary>
        public SipHeader[] SipHeaders { get; set; }
    }

    /// <summary>
    /// Sip header.
    /// </summary>
    public class SipHeader
    {
        /// <summary>
        /// Gets or sets the header name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the header value.
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// SIP media type containing type and subtype. For example, for
    /// "application/sdp", the type is "application" and the subtype is "sdp".
    /// </summary>
    public class SipMediaType
    {
        /// <summary>
        /// Gets or sets the media subtype.
        /// </summary>
        public string SubType { get; set; }

        /// <summary>
        /// Gets or sets the media type.
        /// </summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// Sip multipart vector.
    /// </summary>
    public class SipMultipartPartVector
    {
        /// <summary>
        /// Gets or sets the total sip parts count.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the sip parts.
        /// </summary>
        public SipMultipartPart[] SipMultipartParts { get; set; }
    }

    /// <summary>
    /// This describes each multipart part.
    /// </summary>
    public class SipMultipartPart
    {
        /// <summary>
        /// Gets or sets the body part of tthis multipart part.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the MIME type of the body part of this multipart part.
        /// </summary>
        public SipMediaType ContentType { get; set; }

        /// <summary>
        /// Gets or sets optional headers to be put in this multipart part.
        /// </summary>
        public SipHeaderVector Headers { get; set; }
    }
}
