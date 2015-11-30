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
        /// Get the syslog facility value.
        /// </summary>
        /// <param name="priority">The syslog priority.</param>
        /// <returns>The syslog facility.</returns>
        public static short GetFacility(short priority)
        {
            return (short)(priority >> 3);
        }

        /// <summary>
        /// Get the syslog priority value.
        /// </summary>
        /// <param name="facility">The syslog facility.</param>
        /// <param name="severity">The syslog severity.</param>
        /// <returns>The syslog priority.</returns>
        public static short GetPriority(short facility, short severity)
        {
            return (short)((facility * 8) + severity);
        }

        /// <summary>
        /// Get Syslog facility level.
        /// </summary>
        /// <param name="level">The Syslog facility level.</param>
        /// <returns>The Syslog facility level.</returns>
        public static FacilityLevel GetFacilityLevel(short level)
        {
            switch (level)
            {
                case 0:
                    return FacilityLevel.Kernel;
                case 2:
                    return FacilityLevel.Mail;
                case 3:
                    return FacilityLevel.Daemon;
                case 4:
                    return FacilityLevel.Authorisation;
                case 5:
                    return FacilityLevel.Internally;
                case 6:
                    return FacilityLevel.Printer;
                case 7:
                    return FacilityLevel.News;
                case 8:
                    return FacilityLevel.UUCP;
                case 9:
                    return FacilityLevel.Clock;
                case 10:
                    return FacilityLevel.Security;
                case 11:
                    return FacilityLevel.FTP;
                case 12:
                    return FacilityLevel.NTP;
                case 13:
                    return FacilityLevel.Audit;
                case 14:
                    return FacilityLevel.Alert;
                case 15:
                    return FacilityLevel.ClockDaemon;
                case 16:
                    return FacilityLevel.Local0;
                case 17:
                    return FacilityLevel.Local1;
                case 18:
                    return FacilityLevel.Local2;
                case 19:
                    return FacilityLevel.Local3;
                case 20:
                    return FacilityLevel.Local4;
                case 21:
                    return FacilityLevel.Local5;
                case 22:
                    return FacilityLevel.Local6;
                case 23:
                    return FacilityLevel.Local7;
                case 1:
                default:
                    return FacilityLevel.User;
            }
        }
    }

    /// <summary>
    /// Syslog facility level.
    /// </summary>
    public enum FacilityLevel : short
    {
        /// <summary>
        /// Kernel messages.
        /// </summary>
        Kernel = 0,
        /// <summary>
        /// User-level messages.
        /// </summary>
        User = 1,
        /// <summary>
        /// Mail system.
        /// </summary>
        Mail = 2,
        /// <summary>
        /// System daemons.
        /// </summary>
        Daemon = 3,
        /// <summary>
        /// Authorization messages.
        /// </summary>
        Authorisation = 4,
        /// <summary>
        /// Messages generated internally by syslog server.
        /// </summary>
        Internally = 5,
        /// <summary>
        /// Line printer subsystem.
        /// </summary>
        Printer = 6,
        /// <summary>
        /// Network news subsystem.
        /// </summary>
        News = 7,
        /// <summary>
        /// UUCP subsystem.
        /// </summary>
        UUCP = 8,
        /// <summary>
        /// Clock message.
        /// </summary>
        Clock = 9,
        /// <summary>
        /// Security messages.
        /// </summary>
        Security = 10,
        /// <summary>
        /// FTP daemon.
        /// </summary>
        FTP = 11,
        /// <summary>
        /// NTP subsystem.
        /// </summary>
        NTP = 12,
        /// <summary>
        /// Log audit.
        /// </summary>
        Audit = 13,
        /// <summary>
        /// Log alert.
        /// </summary>
        Alert = 14,
        /// <summary>
        /// clock daemon.
        /// </summary>
        ClockDaemon = 15,
        /// <summary>
        /// Local use.
        /// </summary>
        Local0 = 16,
        /// <summary>
        /// Local use.
        /// </summary>
        Local1 = 17,
        /// <summary>
        /// Local use.
        /// </summary>
        Local2 = 18,
        /// <summary>
        /// Local use.
        /// </summary>
        Local3 = 19,
        /// <summary>
        /// Local use.
        /// </summary>
        Local4 = 20,
        /// <summary>
        /// Local use.
        /// </summary>
        Local5 = 21,
        /// <summary>
        /// Local use.
        /// </summary>
        Local6 = 22,
        /// <summary>
        /// Local use.
        /// </summary>
        Local7 = 23,
    }
}
