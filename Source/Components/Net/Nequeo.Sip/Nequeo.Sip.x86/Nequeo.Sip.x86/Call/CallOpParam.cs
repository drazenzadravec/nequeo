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
    ///	<summary>
    ///	Call options.
    ///	</summary>
    public class CallOpParam
    {
        /// <summary>
        /// Call options.
        /// </summary>
        public CallOpParam() { }

        /// <summary>
        /// Call options.
        /// </summary>
        /// <param name="useDefaultCallSetting">Use default call settings. 
        /// Setting useDefaultCallSetting to true will initialize opt with default
        /// call setting values.</param>
        public CallOpParam(bool useDefaultCallSetting)
        {
            _useDefaultCallSetting = useDefaultCallSetting;
        }

        private bool _useDefaultCallSetting = false;

        /// <summary>
        /// Gets the use default call settings
        /// </summary>
        internal bool UseDefaultCallSetting
        {
            get { return _useDefaultCallSetting; }
        }

        ///	<summary>
        ///	Gets or sets the call settings.
        ///	</summary>
        public CallSetting Setting { get; set; }

        ///	<summary>
        ///	Gets or sets the options.
        ///	</summary>
        public uint Options { get; set; }

        ///	<summary>
        ///	Gets or sets the reason phrase.
        ///	</summary>
        public string Reason { get; set; }

        ///	<summary>
        ///	Gets or sets the status code.
        ///	</summary>
        public StatusCode Code { get; set; }

        ///	<summary>
        ///	Gets or sets the list of headers etc to be added to outgoing response message.
        /// Note that this message data will be persistent in all next
        /// answers / responses for this INVITE request.
        ///	</summary>
        public SipTxOption TxOption { get; set; }
    }
}
