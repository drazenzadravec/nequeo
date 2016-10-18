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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nequeo.Threading.Tasks
{
    /// <summary>
    /// Post complete event arguments.
    /// </summary>
    public class PostCompletedEventArgs : EventArgs
    {
        private SendOrPostCallback callback;

        private object state;

        private Exception error;

        /// <summary>
        /// Post complete event arguments.
        /// </summary>
        /// <param name="callback">The call back.</param>
        /// <param name="state">The state.</param>
        /// <param name="error">The error message.</param>
        public PostCompletedEventArgs(SendOrPostCallback callback, object state, Exception error)
        {
            this.callback = callback;
            this.state = state;
            this.error = error;
        }

        /// <summary>
        /// Gets the send or post callback.
        /// </summary>
        public SendOrPostCallback Callback
        {
            get
            {
                return callback;
            }
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public object State
        {
            get
            {
                return state;
            }
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public Exception Error
        {
            get
            {
                return error;
            }
        }
    }
}

