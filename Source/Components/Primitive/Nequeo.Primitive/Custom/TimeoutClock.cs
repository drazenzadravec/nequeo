/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
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

namespace Nequeo.Custom
{
    /// <summary>
    /// Timeout control, non threaded clock.
    /// </summary>
    public class TimeoutClock
    {
        /// <summary>
        /// Timeout control, non threaded clock.
        /// </summary>
        /// <param name="timeout">The timeout in milliseconds; -1 wait indefinitely.</param>
        /// <exception cref="System.Exception">The timeout is negative.</exception>
        public TimeoutClock(int timeout)
        {
            // Assign the timeout and start the clock.
            _timeout = timeout;
            Start();
        }

        private DateTime _initialTime;
        private int _timeout = 0;

        /// <summary>
        /// Reset the clock.
        /// </summary>
        public void Reset()
        {
            Start();
        }

        /// <summary>
        /// Has the clock timed out.
        /// </summary>
        /// <returns>True is the timeout has beem reached; else false.</returns>
        public bool IsComplete()
        {
            bool complete = false;

            // If the timeout is positive.
            if (_timeout > -1)
            {
                // Get the current time, subtract the initial time from the current time
                // to get the difference and assign the time span from the timeout.
                DateTime now = DateTime.Now;
                TimeSpan lapsed = now.Subtract(_initialTime);
                TimeSpan timeout = new TimeSpan(0, 0, 0, 0, _timeout);

                // If the lapsed time is greater than then timeout span
                // than the timeout has been reached.
                if (lapsed.TotalMilliseconds >= timeout.TotalMilliseconds)
                    complete = true;
            }

            // Return true if timeout has been reached.
            return complete;
        }

        /// <summary>
        /// Start the clock.
        /// </summary>
        private void Start()
        {
            // Set the current time.
            _initialTime = DateTime.Now;
        }
    }
}
