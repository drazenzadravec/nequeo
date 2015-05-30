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

namespace Nequeo.Handler
{
    /// <summary>
    /// Logging attribute.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property | AttributeTargets.Class)]
    public sealed class LoggingAttribute : System.Attribute
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="writeTo">Where the message should be written to.</param>
        /// <param name="logType">The type of logging that is to be applied.</param>
        public LoggingAttribute(WriteTo writeTo, LogType logType)
        {
            _writeTo = writeTo;
            _logType = logType;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="configurationName">The BaseTypeHandlerDefaultSection Extension name.</param>
        public LoggingAttribute(string configurationName)
        {
            _configurationName = configurationName;
        }
        #endregion

        #region Private Fields
        private string _configurationName = string.Empty;
        private WriteTo _writeTo = Nequeo.Handler.WriteTo.None;
        private LogType _logType = Nequeo.Handler.LogType.Error;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the configuration extension name.
        /// </summary>
        public string ConfigurationName
        {
            get { return _configurationName; }
            set { _configurationName = value; }
        }

        /// <summary>
        /// Gets sets, the message should be written to.
        /// </summary>
        public WriteTo WriteTo
        {
            get { return _writeTo; }
            set { _writeTo = value; }
        }

        /// <summary>
        /// Gets sets, the type of logging that is to be applied.
        /// </summary>
        public LogType LogType
        {
            get { return _logType; }
            set { _logType = value; }
        }
        #endregion
    }

    /// <summary>
    /// This enum holds the error message write to location.
    /// </summary>
    public enum WriteTo
    {
        /// <summary>
        /// Writes to the default pre-defined location.
        /// </summary>
        Default = -1,
        /// <summary>
        /// Event log.
        /// </summary>
        EventLog = 0,
        /// <summary>
        /// File.
        /// </summary>
        File = 1,
        /// <summary>
        /// File and Event Log.
        /// </summary>
        FileAndEventLog = 2,
        /// <summary>
        /// No location.
        /// </summary>
        None = 3,
    }

    /// <summary>
    /// This enum holds the status level of a message.
    /// </summary>
    public enum StatusLevel
    {
        /// <summary>
        /// No location.
        /// </summary>
        None = -1,
        /// <summary>
        /// Status level high.
        /// </summary>
        High = 0,
        /// <summary>
        /// Status level medium high.
        /// </summary>
        MediumHigh = 1,
        /// <summary>
        /// Status level medium.
        /// </summary>
        Medium = 2,
        /// <summary>
        /// Status level medium low.
        /// </summary>
        MediumLow = 3,
        /// <summary>
        /// Status level low.
        /// </summary>
        Low = 4,
    }

    /// <summary>
    /// This enum holds the log type when writting to
    /// a log file.
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// Error log type.
        /// </summary>
        Error = 0,
        /// <summary>
        /// Processing log type.
        /// </summary>
        Process = 1,
    }
}
