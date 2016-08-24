/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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
using System.Threading;
using System.Threading.Tasks;

using Nequeo.Invention;
using Nequeo.Serialisation;
using Nequeo.Maintenance.File;
using Nequeo.Maintenance.Timing;
using Nequeo.Maintenance.Data;

namespace Nequeo.Maintenance.File
{
    /// <summary>
    /// Deletes log files from specifed directories at regular intervals.
    /// </summary>
    public class LogFile : IDisposable
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public LogFile()
        {
        }

        private IntervalControl _timer = null;

        /// <summary>
        /// Start the process.
        /// </summary>
        public void Start()
        {
            try
            {
                string xmlValidationMessage = string.Empty;

                // Get the xml file location and
                // the xsd file schema.
                string xml = Nequeo.Maintenance.Properties.Settings.Default.LogFileListXmlPath;
                string xsd = Nequeo.Maintenance.Properties.Resources.LogFileList;

                // Validate the filter xml file.
                if (!Validation.IsXmlValidEx(xsd, xml, out xmlValidationMessage))
                    throw new Exception("Xml validation. " + xmlValidationMessage);

                // Deserialise the xml file into
                // the log directory list object
                GeneralSerialisation serial = new GeneralSerialisation();
                LogDirectoryList directoryList = ((LogDirectoryList)serial.Deserialise(typeof(LogDirectoryList), xml));

                // Start the deletion process.
                _timer = new IntervalControl(directoryList.Interval);
                _timer.Start(u => DeleteLogFiles(u), directoryList);
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();
            }
        }

        /// <summary>
        /// Stop the process.
        /// </summary>
        public void Stop()
        {
            try
            {
                // Stop the delation process.
                _timer.Stop();
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();
            }
        }

        /// <summary>
        /// Delete the files within the directory.
        /// </summary>
        /// <param name="state">The state data</param>
        private void DeleteLogFiles(Object state)
        {
            LogDirectoryList directoryList = (LogDirectoryList)state;

            try
            {
                // For each directory found
                foreach (LogDirectoryListDirectory item in directoryList.Directories)
                {
                    try
                    {
                        // Create a new retension deletion file object.
                        // Find all the files in each directory that need
                        // to be deleted. Start the deletion of the files.
                        Retention retention = new Retention(item.fileRetension, item.Value, item.filePattern);
                        retention.StartDelete();
                    }
                    catch (Exception e)
                    {
                        // Detect a thread abort exception.
                        if (e is ThreadAbortException)
                            Thread.ResetAbort();
                    }
                }
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();
            }
        }

        #region Dispose Object Methods

        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_timer != null)
                        _timer.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _timer = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~LogFile()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
