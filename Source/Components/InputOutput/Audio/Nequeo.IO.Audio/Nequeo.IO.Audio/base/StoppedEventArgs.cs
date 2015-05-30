/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Nequeo.IO.Audio
{
    /// <summary>
    /// Stopped Event Args
    /// </summary>
    public class StoppedEventArgs : EventArgs
    {
        private readonly Exception exception;

        /// <summary>
        /// Initializes a new instance of StoppedEventArgs
        /// </summary>
        /// <param name="exception">An exception to report (null if no exception)</param>
        public StoppedEventArgs(Exception exception = null)
        {
            this.exception = exception;
        }

        /// <summary>
        /// An exception. Will be null if the playback or record operation stopped
        /// </summary>
        public Exception Exception 
        { 
            get { return exception; } 
        }
    }
}
