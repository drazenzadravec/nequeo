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
        private bool audioComplete = false;

        /// <summary>
        /// Initializes a new instance of StoppedEventArgs
        /// </summary>
        /// <param name="exception">An exception to report (null if no exception)</param>
        /// <param name="audioComplete">An indicator specifying the audio has completed.</param>
        public StoppedEventArgs(Exception exception = null, bool audioComplete = false)
        {
            this.exception = exception;
            this.audioComplete = audioComplete;
        }

        /// <summary>
        /// Gets an indicator specifying the audio has completed.
        /// </summary>
        public bool AudioComplete
        {
            get { return audioComplete; }
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
