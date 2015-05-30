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
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nequeo.Handler
{
    /// <summary>
    /// Exception handler interface.
    /// </summary>
    public interface ITypeHandlerBase
    {
        /// <summary>
        /// Write a message to the selected location from the configuration file.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="member">The current member information.</param>
        /// <param name="eventApplicationName">The custom event application name.</param>
        void WriteType(string message, MemberInfo member, string eventApplicationName);

        /// <summary>
        /// Write a message to the selected location from the configuration file.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="configurationName">The BaseTypeHandlerDefaultSection Extension name.</param>
        /// <param name="eventApplicationName">The custom event application name.</param>
        void WriteType(string message, string configurationName, string eventApplicationName);
    }

    /// <summary>
    /// Exception handler interface.
    /// </summary>
    public interface ITypeHandler : ITypeHandlerBase
    {
        /// <summary>
        /// Write a message to the selected location from the configuration file.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="member">The current member information.</param>
        void WriteType(string message, MemberInfo member);

        /// <summary>
        /// Write a message to the selected location from the configuration file.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="configurationName">The BaseTypeHandlerDefaultSection Extension name.</param>
        void WriteType(string message, string configurationName);

        /// <summary>
        /// Write a message to the selected location from the configuration file.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="configurationName">The BaseTypeHandlerDefaultSection Extension name.</param>
        /// <param name="code">A custom code identifier.</param>
        void WriteType(string message, string configurationName, int code);
    }

    /// <summary>
    /// Exception handler interface.
    /// </summary>
    public interface IHandler : ITypeHandler
    {
        /// <summary>
        /// Method, writes to the selected error reporting location.
        /// </summary>
        /// <param name="className">String, the class that the error occurred in.</param>
        /// <param name="member">String, the member that the error occurred in.</param>
        /// <param name="message">String, the error message to write.</param>
        /// <param name="lineNumber">Integer, the line number of the error.</param>
        /// <param name="writeTo">WriteTo, write log to location.</param>
        /// <param name="logType">Enum, the log file type.</param>
        void Write(string className, string member, string message, int lineNumber,
            WriteTo writeTo, LogType logType);

        /// <summary>
        /// Method, writes to the selected error/process reporting location.
        /// </summary>
        /// <param name="className">String, the class that the error occurred in.</param>
        /// <param name="member">String, the member that the error occurred in.</param>
        /// <param name="message">String, the error message to write.</param>
        /// <param name="lineNumber">Integer, the line number of the error.</param>
        /// <param name="code">Integer, the error/processing code for the operation.</param>
        /// <param name="writeTo">WriteTo, write log to location.</param>
        /// <param name="logType">Enum, the log file type.</param>
        void Write(string className, string member, string message, int lineNumber, int code,
            WriteTo writeTo, LogType logType);

        /// <summary>
        /// Get the last error that has occurred.
        /// </summary>
        /// <returns>The last error that occured.</returns>
        string GetLastErrorDescription();
        
        /// <summary>
        /// Get the last process that has occurred.
        /// </summary>
        /// <returns>The last process that occured.</returns>
        string GetLastProcessDescription();
        
        /// <summary>
        /// Write the message to the event log.
        /// </summary>
        /// <param name="sMessage">String, the error message to write.</param>
        void WriteToEventLog(string sMessage);

        /// <summary>
        /// This property holds the error log file path.
        /// </summary>
        string LogErrorFilePath { get; set; }

        /// <summary>
        /// This property holds the process log file path.
        /// </summary>
        string LogProcessFilePath { get; set; }

        /// <summary>
        /// The application settings reader class.
        /// </summary>
        System.Configuration.AppSettingsReader AppSettingsReader { get; }

        /// <summary>
        /// Gets, the connection strings section reader class.
        /// </summary>
        System.Configuration.ConnectionStringSettingsCollection ConnectionStringsReader { get; }

        /// <summary>
        /// Gets, the base handler configuration section reader class.
        /// </summary>
        Nequeo.Handler.Configuration.BaseHandlerElement BaseHandlerConfigurationReader { get; }

        /// <summary>
        /// Get the web application virtual path.
        /// </summary>
        /// <returns>The web application virtual path.</returns>
        string GetWebApplicationVirtualPath();

        /// <summary>
        /// Get the web application physical path.
        /// </summary>
        /// <returns>The web application physical path.</returns>
        string GetWebApplicationPath();

    }
}
