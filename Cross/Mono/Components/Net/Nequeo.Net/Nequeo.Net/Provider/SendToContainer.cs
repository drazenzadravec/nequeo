/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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

namespace Nequeo.Net.Provider
{
    /// <summary>
    /// Send to clients container.
    /// </summary>
    public class SendToContainer
    {
        /// <summary>
        /// Gets or sets the data to send.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets the single server contexts.
        /// </summary>
        public Nequeo.Net.Provider.ISingleContextBase[] SingleServerContexts { get; set; }

        /// <summary>
        /// Gets or sets the server contexts.
        /// </summary>
        public Nequeo.Net.Sockets.IServerContext[] ServerContexts { get; set; }

        /// <summary>
        /// Gets or sets the server contexts.
        /// </summary>
        public Nequeo.Net.Sockets.IUdpServerContext[] UdpServerContexts { get; set; }

        /// <summary>
        /// Gets or sets the member contexts.
        /// </summary>
        public Nequeo.Net.IMemberContext[] MemberContexts { get; set; }

    }
}
