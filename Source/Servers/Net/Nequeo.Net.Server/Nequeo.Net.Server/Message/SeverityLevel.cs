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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Net.Data;

namespace Nequeo.Net.Message
{
    /// <summary>
    /// Syslog server utilty helper.
    /// </summary>
    public partial class SyslogUtility
    {
        /// <summary>
        /// Get the syslog severity value.
        /// </summary>
        /// <param name="priority">The syslog priority.</param>
        /// <returns>The syslog severity.</returns>
        public static short GetSeverity(short priority)
        {
            return (short)(priority - (GetFacility(priority) * 8));
        }

        /// <summary>
        /// Get the list of all possible priorities.
        /// </summary>
        /// <returns>The list of all priorities.</returns>
        public static short[] GetPriorities()
        {
            List<short> items = new List<short>();

            items.Add(GetPriority(0, 0));
            items.Add(GetPriority(0, 1));
            items.Add(GetPriority(0, 2));
            items.Add(GetPriority(0, 3));
            items.Add(GetPriority(0, 4));
            items.Add(GetPriority(0, 5));
            items.Add(GetPriority(0, 6));
            items.Add(GetPriority(0, 7));

            items.Add(GetPriority(1, 0));
            items.Add(GetPriority(1, 1));
            items.Add(GetPriority(1, 2));
            items.Add(GetPriority(1, 3));
            items.Add(GetPriority(1, 4));
            items.Add(GetPriority(1, 5));
            items.Add(GetPriority(1, 6));
            items.Add(GetPriority(1, 7));

            items.Add(GetPriority(2, 0));
            items.Add(GetPriority(2, 1));
            items.Add(GetPriority(2, 2));
            items.Add(GetPriority(2, 3));
            items.Add(GetPriority(2, 4));
            items.Add(GetPriority(2, 5));
            items.Add(GetPriority(2, 6));
            items.Add(GetPriority(2, 7));

            items.Add(GetPriority(3, 0));
            items.Add(GetPriority(3, 1));
            items.Add(GetPriority(3, 2));
            items.Add(GetPriority(3, 3));
            items.Add(GetPriority(3, 4));
            items.Add(GetPriority(3, 5));
            items.Add(GetPriority(3, 6));
            items.Add(GetPriority(3, 7));

            items.Add(GetPriority(4, 0));
            items.Add(GetPriority(4, 1));
            items.Add(GetPriority(4, 2));
            items.Add(GetPriority(4, 3));
            items.Add(GetPriority(4, 4));
            items.Add(GetPriority(4, 5));
            items.Add(GetPriority(4, 6));
            items.Add(GetPriority(4, 7));

            items.Add(GetPriority(5, 0));
            items.Add(GetPriority(5, 1));
            items.Add(GetPriority(5, 2));
            items.Add(GetPriority(5, 3));
            items.Add(GetPriority(5, 4));
            items.Add(GetPriority(5, 5));
            items.Add(GetPriority(5, 6));
            items.Add(GetPriority(5, 7));

            items.Add(GetPriority(6, 0));
            items.Add(GetPriority(6, 1));
            items.Add(GetPriority(6, 2));
            items.Add(GetPriority(6, 3));
            items.Add(GetPriority(6, 4));
            items.Add(GetPriority(6, 5));
            items.Add(GetPriority(6, 6));
            items.Add(GetPriority(6, 7));

            items.Add(GetPriority(7, 0));
            items.Add(GetPriority(7, 1));
            items.Add(GetPriority(7, 2));
            items.Add(GetPriority(7, 3));
            items.Add(GetPriority(7, 4));
            items.Add(GetPriority(7, 5));
            items.Add(GetPriority(7, 6));
            items.Add(GetPriority(7, 7));

            items.Add(GetPriority(8, 0));
            items.Add(GetPriority(8, 1));
            items.Add(GetPriority(8, 2));
            items.Add(GetPriority(8, 3));
            items.Add(GetPriority(8, 4));
            items.Add(GetPriority(8, 5));
            items.Add(GetPriority(8, 6));
            items.Add(GetPriority(8, 7));

            items.Add(GetPriority(9, 0));
            items.Add(GetPriority(9, 1));
            items.Add(GetPriority(9, 2));
            items.Add(GetPriority(9, 3));
            items.Add(GetPriority(9, 4));
            items.Add(GetPriority(9, 5));
            items.Add(GetPriority(9, 6));
            items.Add(GetPriority(9, 7));

            items.Add(GetPriority(10, 0));
            items.Add(GetPriority(10, 1));
            items.Add(GetPriority(10, 2));
            items.Add(GetPriority(10, 3));
            items.Add(GetPriority(10, 4));
            items.Add(GetPriority(10, 5));
            items.Add(GetPriority(10, 6));
            items.Add(GetPriority(10, 7));

            items.Add(GetPriority(11, 0));
            items.Add(GetPriority(11, 1));
            items.Add(GetPriority(11, 2));
            items.Add(GetPriority(11, 3));
            items.Add(GetPriority(11, 4));
            items.Add(GetPriority(11, 5));
            items.Add(GetPriority(11, 6));
            items.Add(GetPriority(11, 7));

            items.Add(GetPriority(12, 0));
            items.Add(GetPriority(12, 1));
            items.Add(GetPriority(12, 2));
            items.Add(GetPriority(12, 3));
            items.Add(GetPriority(12, 4));
            items.Add(GetPriority(12, 5));
            items.Add(GetPriority(12, 6));
            items.Add(GetPriority(12, 7));

            items.Add(GetPriority(13, 0));
            items.Add(GetPriority(13, 1));
            items.Add(GetPriority(13, 2));
            items.Add(GetPriority(13, 3));
            items.Add(GetPriority(13, 4));
            items.Add(GetPriority(13, 5));
            items.Add(GetPriority(13, 6));
            items.Add(GetPriority(13, 7));

            items.Add(GetPriority(14, 0));
            items.Add(GetPriority(14, 1));
            items.Add(GetPriority(14, 2));
            items.Add(GetPriority(14, 3));
            items.Add(GetPriority(14, 4));
            items.Add(GetPriority(14, 5));
            items.Add(GetPriority(14, 6));
            items.Add(GetPriority(14, 7));

            items.Add(GetPriority(15, 0));
            items.Add(GetPriority(15, 1));
            items.Add(GetPriority(15, 2));
            items.Add(GetPriority(15, 3));
            items.Add(GetPriority(15, 4));
            items.Add(GetPriority(15, 5));
            items.Add(GetPriority(15, 6));
            items.Add(GetPriority(15, 7));

            items.Add(GetPriority(16, 0));
            items.Add(GetPriority(16, 1));
            items.Add(GetPriority(16, 2));
            items.Add(GetPriority(16, 3));
            items.Add(GetPriority(16, 4));
            items.Add(GetPriority(16, 5));
            items.Add(GetPriority(16, 6));
            items.Add(GetPriority(16, 7));

            items.Add(GetPriority(17, 0));
            items.Add(GetPriority(17, 1));
            items.Add(GetPriority(17, 2));
            items.Add(GetPriority(17, 3));
            items.Add(GetPriority(17, 4));
            items.Add(GetPriority(17, 5));
            items.Add(GetPriority(17, 6));
            items.Add(GetPriority(17, 7));

            items.Add(GetPriority(18, 0));
            items.Add(GetPriority(18, 1));
            items.Add(GetPriority(18, 2));
            items.Add(GetPriority(18, 3));
            items.Add(GetPriority(18, 4));
            items.Add(GetPriority(18, 5));
            items.Add(GetPriority(18, 6));
            items.Add(GetPriority(18, 7));

            items.Add(GetPriority(19, 0));
            items.Add(GetPriority(19, 1));
            items.Add(GetPriority(19, 2));
            items.Add(GetPriority(19, 3));
            items.Add(GetPriority(19, 4));
            items.Add(GetPriority(19, 5));
            items.Add(GetPriority(19, 6));
            items.Add(GetPriority(19, 7));

            items.Add(GetPriority(20, 0));
            items.Add(GetPriority(20, 1));
            items.Add(GetPriority(20, 2));
            items.Add(GetPriority(20, 3));
            items.Add(GetPriority(20, 4));
            items.Add(GetPriority(20, 5));
            items.Add(GetPriority(20, 6));
            items.Add(GetPriority(20, 7));

            items.Add(GetPriority(21, 0));
            items.Add(GetPriority(21, 1));
            items.Add(GetPriority(21, 2));
            items.Add(GetPriority(21, 3));
            items.Add(GetPriority(21, 4));
            items.Add(GetPriority(21, 5));
            items.Add(GetPriority(21, 6));
            items.Add(GetPriority(21, 7));

            items.Add(GetPriority(22, 0));
            items.Add(GetPriority(22, 1));
            items.Add(GetPriority(22, 2));
            items.Add(GetPriority(22, 3));
            items.Add(GetPriority(22, 4));
            items.Add(GetPriority(22, 5));
            items.Add(GetPriority(22, 6));
            items.Add(GetPriority(22, 7));

            items.Add(GetPriority(23, 0));
            items.Add(GetPriority(23, 1));
            items.Add(GetPriority(23, 2));
            items.Add(GetPriority(23, 3));
            items.Add(GetPriority(23, 4));
            items.Add(GetPriority(23, 5));
            items.Add(GetPriority(23, 6));
            items.Add(GetPriority(23, 7));

            // Return
            return items.ToArray();
        }

        /// <summary>
        /// Get Syslog severity level.
        /// </summary>
        /// <param name="level">The Syslog severity level.</param>
        /// <returns>The Syslog severity level.</returns>
        public static SeverityLevel GetSeverityLevel(short level)
        {
            switch(level)
            {
                case 0:
                    return SeverityLevel.Emergency;
                case 1:
                    return SeverityLevel.Alert;
                case 2:
                    return SeverityLevel.Critical;
                case 3:
                    return SeverityLevel.Error;
                case 4:
                    return SeverityLevel.Warning;
                case 5:
                    return SeverityLevel.Notice;
                case 7:
                    return SeverityLevel.Debug;
                case 6:
                default:
                    return SeverityLevel.Information;
            }
        }
    }

    /// <summary>
    /// Syslog severity level.
    /// </summary>
    public enum SeverityLevel : short
    {
        /// <summary>
        /// System is unusable. A "panic" condition usually affecting multiple apps/servers/sites. At this level it would usually notify all tech staff on call.
        /// </summary>
        Emergency = 0,
        /// <summary>
        /// Action must be taken immediately. Should be corrected immediately, therefore notify staff who can fix the problem. An example would be the loss of a primary ISP connection.
        /// </summary>
        Alert = 1,
        /// <summary>
        /// Critical conditions. Should be corrected immediately, but indicates failure in a secondary system, an example is a loss of a backup ISP connection.
        /// </summary>
        Critical = 2,
        /// <summary>
        /// Error conditions. Non-urgent failures, these should be relayed to developers or admins; each item must be resolved within a given time.
        /// </summary>
        Error = 3,
        /// <summary>
        /// Warning conditions. Warning messages, not an error, but indication that an error will occur if action is not taken, e.g. file system 85% full - each item must be resolved within a given time.
        /// </summary>
        Warning = 4,
        /// <summary>
        /// Normal but significant condition. Events that are unusual but not error conditions - might be summarized in an email to developers or admins to spot potential problems - no immediate action required.
        /// </summary>
        Notice = 5,
        /// <summary>
        /// Information messages. Normal operational messages - may be harvested for reporting, measuring throughput, etc. - no action required.
        /// </summary>
        Information = 6,
        /// <summary>
        /// Debug-level messages. Info useful to developers for debugging the application, not useful during operations.
        /// </summary>
        Debug = 7,
    }
}
