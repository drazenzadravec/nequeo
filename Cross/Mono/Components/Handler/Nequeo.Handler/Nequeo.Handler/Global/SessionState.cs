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
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Generic;

using Nequeo.Handler;

namespace Nequeo.Handler.Global
{
    /// <summary>
    /// Class contains methods for setting the
    /// session state data storage path.
    /// </summary>
    public class SessionState : Nequeo.Handler.LogHandler, IDisposable, ISessionState
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public SessionState()
            : base(applicationName, eventNamespace)
        {
        }
        #endregion

        #region Private Constants
        private const string applicationName = "Nequeo";
        private const string eventNamespace = "Nequeo.Handler.Global";
        #endregion

        #region Private Methods
        /// <summary>
        /// Method to locate file log path within the
        /// application configuration file.
        /// </summary>
        /// <param name="specificPath">The specific path of the config file, used for web applications.</param>
        /// <returns>The full path of the logging file, else empty string.</returns>
        private string SessionStatePathEx(string specificPath)
        {
            System.Configuration.Configuration config = null;

            try
            {
                // Get the absolute configuration information
                // of the current application.
                if (string.IsNullOrEmpty(specificPath))
                    config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                else
                    config = ConfigurationManager.OpenExeConfiguration(specificPath);
            }
            catch { }

            try
            {
                if (config == null)
                {
                    string path = GetWebApplicationPath();
                    config = ConfigurationManager.OpenExeConfiguration(path);
                }
            }
            catch { }

            try
            {
                if (config != null)
                {
                    // Get the current log file path, relative
                    // or absolute path.
                    string fileLogPath = BaseHandlerConfigurationReader.SessionStatePath + "\\";

                    // Is the log file path relative to
                    // the current application.
                    bool fileLogPathIsRelative = BaseHandlerConfigurationReader.SessionStatePathIsRelative;

                    // If the log file path is relative to the application
                    // then get the absolute path to the log file.
                    if (fileLogPathIsRelative)
                        fileLogPath = System.IO.Path.GetDirectoryName(config.FilePath.ToString()) + "\\" + fileLogPath;

                    // Return the location of the
                    // logging file.
                    return fileLogPath;
                }
                return string.Empty;
            }
            catch (System.Exception ex)
            {
                LogHandler.WriteMessageToApplicationEventLog(applicationName, ex.Message, EventLogEntryType.Error);
                return string.Empty;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get the session state path.
        /// </summary>
        /// <param name="specificPath">The specific path of the config file, used for web applications.</param>
        /// <returns>The full path of the logging file.</returns>
        public virtual string SessionStatePath(string specificPath)
        {
            return this.SessionStatePathEx(specificPath);
        }

        /// <summary>
        /// Get the session state path.
        /// </summary>
        /// <param name="specificPath">The specific path of the config file, used for web applications.</param>
        /// <returns>The full path of the logging file.</returns>
        public static string GetSessionStatePath(string specificPath)
        {
            SessionState state = new SessionState();
            return state.SessionStatePath(specificPath);
        }

        /// <summary>
        /// Get the session state path for a web application.
        /// </summary>
        /// <returns>The full path of the logging file.</returns>
        public static string GetWebAppSessionStatePath()
        {
            SessionState state = new SessionState();
            return state.SessionStatePath(state.GetWebApplicationPath());
        }
        #endregion
    }

    /// <summary>
    /// Interface contains methods for setting the
    /// session state data storage path.
    /// </summary>
    public interface ISessionState
    {
        /// <summary>
        /// Get the session state path.
        /// </summary>
        /// <param name="specificPath">The specific path of the config file, used for web applications.</param>
        /// <returns>The full path of the logging file.</returns>
        string SessionStatePath(string specificPath);
    }
}
