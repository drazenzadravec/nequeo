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

using Nequeo.Net.Sip;

namespace Nequeo.VoIP.Sip.Param
{
    /// <summary>
    /// Call information.
    /// </summary>
    public class CallParam
    {
        /// <summary>
        /// Call information.
        /// </summary>
        /// <param name="call">The current call.</param>
        internal CallParam(Call call)
        {
            _call = call;
        }

        private Call _call = null;

        /// <summary>
        /// Hangup the current call.
        /// </summary>
        public void Hangup()
        {
            // Create the call settings.
            CallSetting setting = new CallSetting(true);
            CallOpParam parm = new CallOpParam(true);
            setting.AudioCount = 1;
            parm.Setting = setting;
            parm.Code = StatusCode.SC_BUSY_HERE;

            // Hangup the call.
            _call.Hangup(parm);
        }
    }
}
