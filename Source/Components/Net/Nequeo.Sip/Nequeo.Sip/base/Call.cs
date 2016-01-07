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
    /// 
    /// </summary>
    public class Call
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="callId"></param>
        public Call(Account account, int callId)
        {
            _callId = callId;
            _account = account;
            _pjCallCallback = new CallCallback(account.PjAccount, callId);
        }

        private int _callId = 0;
        private Account _account = null;
        private CallCallback _pjCallCallback = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="digits"></param>
        public void DialDigits(string digits)
        {
            //pjsua2.CallInfo info = _pjCallCallback.getInfo();
            bool bb = _pjCallCallback.hasMedia();
            int jj = _pjCallCallback.vidGetStreamIdx();
            

            //pjsua2.Media medai = _pjCallCallback.getMedia(_callId);
            

            pjsua2.Call ll = CallCallback.lookup(_callId);

            //pjsua2.CallOpParam par = new pjsua2.CallOpParam(true);
            //_pjCallCallback.reinvite(par);

            _pjCallCallback.dialDtmf(digits);
        }

        /// <summary>
        /// 
        /// </summary>
        public void MakeCall()
        {
            pjsua2.CallOpParam callParam = new pjsua2.CallOpParam(true);
            _pjCallCallback.makeCall("sip:0282794795@58.96.1.2", callParam);
        }

        public string dump(bool with_media, string indent)
        {
            return _pjCallCallback.dump(with_media, indent);
        }

        /// <summary>
        /// 
        /// </summary>
        internal class CallCallback : pjsua2.Call
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="account"></param>
            public CallCallback(pjsua2.Account account) : base(account)
            {

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="account"></param>
            /// <param name="callId"></param>
            public CallCallback(pjsua2.Account account, int callId) : base(account, callId)
            {

            }
        }
    }
}
