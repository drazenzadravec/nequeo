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
using System.Text;
using System.Security;
using System.Net.Mail;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

using Nequeo.Handler.Common;
using Nequeo.Handler.Configuration;
using Nequeo.ComponentModel.Composition;

namespace Nequeo.Handler
{
    /// <summary>
    /// This class contains <![CDATA[ <methods><fields><properties> ]]> for asynchronous error and process handling.
    /// </summary>
    public sealed class AsyncLogHandler
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public AsyncLogHandler()
        {
        }
        #endregion

        #region Private Fields
        private Action _actionHandler = null;
        private Action<ITypeHandlerBase> _action = null;
        private ITypeHandlerBase _logHandler = default(ITypeHandlerBase);
        #endregion

        #region Public Events
        /// <summary>
        /// The async execute complete event.
        /// </summary>
        public static event EventHandler AsyncExecuteComplete;

        #endregion

        #region Public Static Methods
        /// <summary>
        /// Start a new log execute action.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <param name="actionName">The unique action name; passed to the object sender of the AsyncExecuteComplete handler.</param>
        public static async void Execute(Action<ITypeHandlerBase> action, object actionName = null)
        {
            AsyncLogHandler handler = new AsyncLogHandler();
            handler._logHandler = new LogHandler("","");
            handler._action = action;

            // Start the action asynchronously
            Task data = Task.Factory.FromAsync(handler.BeginAction(), handler.EndAction);
            await data;

            if (AsyncExecuteComplete != null)
                AsyncExecuteComplete(actionName, new EventArgs());
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Begin async action
        /// </summary>
        /// <returns>The async result</returns>
        private IAsyncResult BeginAction()
        {
            if (_actionHandler == null)
                _actionHandler = new Action(AsyncAction);

            // Begin the async call.
            return _actionHandler.BeginInvoke(null, null);
        }

        /// <summary>
        /// End async action
        /// </summary>
        /// <param name="ar">The async result</param>
        private void EndAction(IAsyncResult ar)
        {
            if (_actionHandler == null)
                throw new System.InvalidOperationException("End of asynchronous" +
                    " operation attempted when one has not begun.");

            // Use the AsyncResult to complete that async operation.
            _actionHandler.EndInvoke(ar);
        }

        /// <summary>
        /// Execute the action.
        /// </summary>
        private void AsyncAction()
        {
            _action(_logHandler);
        }

        #endregion
    }

    /// <summary>
    /// This class contains <![CDATA[ <methods><fields><properties> ]]> for error and process handling.
    /// </summary>
    [Export(typeof(IHandler))]
    [Export(typeof(ITypeHandler))]
    [Export(typeof(ITypeHandlerBase))]
    [ContentMetadata(Name = "LogHandler")]
    public class LogHandler : ServiceWebApplicationInformation, IDisposable, IHandler
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="assemblyName">The assembly name when logging.</param>
        /// <param name="typeNamespace">The type namespace when logging.</param>
        public LogHandler(string assemblyName, string typeNamespace)
            : base(null, null, 0)
        {
            // Set the application name
            _applicationName = assemblyName;
            _eventName = typeNamespace;
        }
        #endregion

        #region Private Fields
        /// <summary>
        /// The application name.
        /// </summary>
        private string _applicationName = "Nequeo";
        /// <summary>
        /// The event namespace.
        /// </summary>
        private string _eventName = "Nequeo.Handler";
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool disposed = false;
        /// <summary>
        /// The erite to files thread lock.
        /// </summary>
        private WriteToLogFileThread _logFileThread = new WriteToLogFileThread();
        /// <summary>
        /// The exception message field.
        /// </summary>
        private string _exceptionMessage = String.Empty;
        /// <summary>
        /// The process message field.
        /// </summary>
        private string _processMessage = String.Empty;
        /// <summary>
        /// The file name and path for errors.
        /// </summary>
        private string _fileNameAndPathError = String.Empty;
        /// <summary>
        /// The file name and path for processing.
        /// </summary>
        private string _fileNameAndPathProcessing = String.Empty;
        /// <summary>
        /// Get set, this property holds the override location, when
        /// writing to an error/process log location.
        /// </summary>
        private WriteTo _overrideLocation = Nequeo.Handler.WriteTo.Default;
        #endregion

        #region Protected Properties
        /// <summary>
        /// Get, this property holds the current exception.
        /// </summary>
        protected string ExceptionMessage
        {
            get { return _exceptionMessage; }
        }

        /// <summary>
        /// Get, this property holds the current process.
        /// </summary>
        protected string ProcessMessage
        {
            get { return _processMessage; }
        }

        /// <summary>
        /// Get set, this property holds the override location, when
        /// writing to an error/process log location.
        /// </summary>
        protected WriteTo OverrideWriteLocation
        {
            get { return _overrideLocation; }
            set { _overrideLocation = value; }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Get set, this property holds the error log file path.
        /// </summary>
        public string LogErrorFilePath
        {
            get { return _fileNameAndPathError; }
            set { _fileNameAndPathError = value; }
        }

        /// <summary>
        /// Get set, this property holds the process log file path.
        /// </summary>
        public string LogProcessFilePath
        {
            get { return _fileNameAndPathProcessing; }
            set { _fileNameAndPathProcessing = value; }
        }

        /// <summary>
        /// Gets, the application settings reader class.
        /// </summary>
        public System.Configuration.AppSettingsReader AppSettingsReader
        {
            get { return new System.Configuration.AppSettingsReader(); }
        }

        /// <summary>
        /// Gets, the connection strings section reader class.
        /// </summary>
        public System.Configuration.ConnectionStringSettingsCollection ConnectionStringsReader
        {
            get { return System.Configuration.ConfigurationManager.ConnectionStrings; }
        }

        /// <summary>
        /// Gets, the base handler configuration section reader class.
        /// </summary>
        public Configuration.BaseHandlerElement BaseHandlerConfigurationReader
        {
            get { return Nequeo.Handler.Configuration.BaseHandlerConfigurationManager.BaseHandlerElement(); }
        }
        #endregion

        #region Public Events
        /// <summary>
        /// This event occurs when an error occurs 
        /// attempting to process data.
        /// </summary>
        public event Nequeo.Threading.EventHandler<ErrorMessageArgs> OnError;
        #endregion

        #region Public Methods
        /// <summary>
        /// Get the last error that has occurred.
        /// </summary>
        /// <returns>The last error that occured.</returns>
        public virtual string GetLastErrorDescription()
        {
            return _exceptionMessage;
        }

        /// <summary>
        /// Get the last process that has occurred.
        /// </summary>
        /// <returns>The last process that occured.</returns>
        public virtual string GetLastProcessDescription()
        {
            return _processMessage;
        }

        /// <summary>
        /// Get the web application virtual path.
        /// </summary>
        /// <returns>The web application virtual path.</returns>
        public virtual string GetWebApplicationVirtualPath()
        {
            return base.GetApplicationVirtualPath();
        }

        /// <summary>
        /// Get the web application physical path.
        /// </summary>
        /// <returns>The web application physical path.</returns>
        public virtual string GetWebApplicationPath()
        {
            return base.GetApplicationPath();
        }

        /// <summary>
        /// Method, writes to the selected error/process reporting location.
        /// </summary>
        /// <param name="className">String, the class that the error occurred in.</param>
        /// <param name="member">String, the member that the error occurred in.</param>
        /// <param name="message">String, the error message to write.</param>
        /// <param name="lineNumber">Integer, the line number of the error.</param>
        /// <param name="writeTo">WriteTo, write log to location.</param>
        /// <param name="logType">Enum, the log file type.</param>
        public virtual void Write(string className, string member, string message, int lineNumber,
            WriteTo writeTo, LogType logType)
        {
            // Make sure the page reference exists.
            if (className == null) throw new ArgumentNullException("className");
            if (member == null) throw new ArgumentNullException("member");
            if (message == null) throw new ArgumentNullException("message");

            this.WriteErrorEx(className, member, message, lineNumber, 0, writeTo, logType);
        }

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
        public virtual void Write(string className, string member, string message, int lineNumber,
            int code, WriteTo writeTo, LogType logType)
        {
            // Make sure the page reference exists.
            if (className == null) throw new ArgumentNullException("className");
            if (member == null) throw new ArgumentNullException("method");
            if (message == null) throw new ArgumentNullException("message");

            this.WriteErrorEx(className, member, message, lineNumber, code, writeTo, logType);
        }

        /// <summary>
        /// Write a message to the selected location from the configuration file.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="member">The current member information.</param>
        /// <param name="eventApplicationName">The custom event application name.</param>
        [Export("WriteMemberType")]
        public virtual void WriteType(string message, MemberInfo member, string eventApplicationName)
        {
            // Make sure the page reference exists.
            if (message == null) throw new ArgumentNullException("message");
            if (member == null) throw new ArgumentNullException("member");
            if (eventApplicationName == null) throw new ArgumentNullException("eventApplicationName");

            bool foundLoggingAttr = false;
            List<Object> attributes = new List<object>();
            string configurationName = string.Empty;
            WriteTo writeTo = Nequeo.Handler.WriteTo.None;
            LogType logType = Nequeo.Handler.LogType.Error;

            try
            {
                // Get the attributes.
                attributes.AddRange(member.GetCustomAttributes(false));
                attributes.AddRange(member.DeclaringType.GetCustomAttributes(false));

                // If we are using any action handler then
                // go deeper into the declaring type to
                // get the actual declaring type.
                if (member.DeclaringType.DeclaringType != null)
                    attributes.AddRange(member.DeclaringType.DeclaringType.GetCustomAttributes(false));

                if (attributes.Count() < 1)
                    throw new System.Exception("No 'Nequeo.Handler.LoggingAttribute' has been applied to the member.");

                // For each attribute on each property
                // in the type.
                foreach (object attribute in attributes)
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is Nequeo.Handler.LoggingAttribute)
                    {
                        // Cast the current attribute.
                        Nequeo.Handler.LoggingAttribute att =
                            (Nequeo.Handler.LoggingAttribute)attribute;

                        configurationName = att.ConfigurationName;
                        writeTo = att.WriteTo;
                        logType = att.LogType;
                        foundLoggingAttr = true;
                        break;
                    }
                }

                // Make sure that the logging attribite has been assigned.
                if (!foundLoggingAttr)
                    throw new System.Exception("No 'Nequeo.Handler.LoggingAttribute' has been applied to the member.");

                try
                {
                    // If a configuration file has been specified.
                    if (!String.IsNullOrEmpty(configurationName))
                    {
                        // Get the configuration section
                        BaseTypeHandlerElement[] typeHandler = BaseTypeHandlerConfigurationManager.BaseTypeHandlerElements();
                        if (typeHandler != null)
                        {
                            if (typeHandler.Length > 0)
                            {
                                // Find the first name and write to the location.
                                BaseTypeHandlerElement item = typeHandler.First(u => u.Name == configurationName);
                                Type typeNamespace = Nequeo.Reflection.TypeAccessor.GetType(item.TypeName);

                                LogHandler error = new LogHandler(item.AssemblyName, typeNamespace.FullName);
                                error._applicationName = item.AssemblyName;
                                error.Write(typeNamespace.Name, item.TypeMemberName, message, 0, 0, item.WriteTo, item.LogType);
                            }
                        }
                    }
                    else
                    {
                        // If we are using any action handler then
                        // go deeper into the declaring type to
                        // get the actual declaring type.
                        if (member.DeclaringType.DeclaringType != null)
                        {
                            LogHandler error = new LogHandler(
                                (String.IsNullOrEmpty(eventApplicationName) ?
                                    member.Module.Assembly.GetName().Name : eventApplicationName),
                                    member.DeclaringType.Namespace);
                            error.Write(member.DeclaringType.DeclaringType.Name, member.Name, message, 0, 0, writeTo, logType);
                        }
                        else
                        {
                            LogHandler error = new LogHandler(
                                (String.IsNullOrEmpty(eventApplicationName) ?
                                    member.Module.Assembly.GetName().Name : eventApplicationName),
                                    member.DeclaringType.Namespace);
                            error.Write(member.DeclaringType.Name, member.Name, message, 0, 0, writeTo, logType);
                        }
                    }
                }
                catch { }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Write a message to the selected location from the configuration file.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="configurationName">The BaseTypeHandlerDefaultSection Extension name.</param>
        /// <param name="eventApplicationName">The custom event application name.</param>
        [Export("WriteConfigurationType")]
        public virtual void WriteType(string message, string configurationName, string eventApplicationName)
        {
            // Make sure the page reference exists.
            if (configurationName == null) throw new ArgumentNullException("configurationName");
            if (message == null) throw new ArgumentNullException("message");
            if (eventApplicationName == null) throw new ArgumentNullException("eventApplicationName");

            try
            {
                // Get the configuration section
                BaseTypeHandlerElement[] typeHandler = BaseTypeHandlerConfigurationManager.BaseTypeHandlerElements();
                if (typeHandler != null)
                {
                    if (typeHandler.Length > 0)
                    {
                        // Find the first name.
                        BaseTypeHandlerElement item = typeHandler.First(u => u.Name == configurationName);
                        Type typeNamespace = Nequeo.Reflection.TypeAccessor.GetType(item.TypeName);

                        // Write the message.
                        LogHandler error = new LogHandler(
                            (String.IsNullOrEmpty(eventApplicationName) ?
                                item.AssemblyName : eventApplicationName),
                            typeNamespace.FullName);
                        error.Write(typeNamespace.Name, item.TypeMemberName, message, 0, 0, item.WriteTo, item.LogType);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Write a message to the selected location from the configuration file.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="member">The current member information.</param>
        public virtual void WriteType(string message, MemberInfo member)
        {
            // Make sure the page reference exists.
            if (message == null) throw new ArgumentNullException("message");
            if (member == null) throw new ArgumentNullException("member");

            bool foundLoggingAttr = false;
            List<Object> attributes = new List<object>();
            string configurationName = string.Empty;
            WriteTo writeTo = Nequeo.Handler.WriteTo.None;
            LogType logType = Nequeo.Handler.LogType.Error;

            try
            {
                // Get the attributes.
                attributes.AddRange(member.GetCustomAttributes(false));
                attributes.AddRange(member.DeclaringType.GetCustomAttributes(false));
                
                if (attributes.Count() < 1)
                    throw new System.Exception("No 'Nequeo.Handler.LoggingAttribute' has been applied to the member.");

                // For each attribute on each property
                // in the type.
                foreach (object attribute in attributes)
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is Nequeo.Handler.LoggingAttribute)
                    {
                        // Cast the current attribute.
                        Nequeo.Handler.LoggingAttribute att =
                            (Nequeo.Handler.LoggingAttribute)attribute;

                        configurationName = att.ConfigurationName;
                        writeTo = att.WriteTo;
                        logType = att.LogType;
                        foundLoggingAttr = true;
                        break;
                    }
                }

                // Make sure that the logging attribite has been assigned.
                if (!foundLoggingAttr)
                    throw new System.Exception("No 'Nequeo.Handler.LoggingAttribute' has been applied to the member.");

                try
                {
                    // If a configuration file has been specified.
                    if (!String.IsNullOrEmpty(configurationName))
                    {
                        // Get the configuration section
                        BaseTypeHandlerElement[] typeHandler = BaseTypeHandlerConfigurationManager.BaseTypeHandlerElements();
                        if (typeHandler != null)
                        {
                            if (typeHandler.Length > 0)
                            {
                                // Find the first name and write to the location.
                                BaseTypeHandlerElement item = typeHandler.First(u => u.Name == configurationName);
                                Type typeNamespace = Nequeo.Reflection.TypeAccessor.GetType(item.TypeName);
                                this.WriteErrorEx(typeNamespace.Name, item.TypeMemberName, message, 0, 0, item.WriteTo, item.LogType);
                            }
                        }
                    }
                    else
                        this.WriteErrorEx(member.DeclaringType.Name, member.Name, message, 0, 0, writeTo, logType);
                }
                catch { }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Write a message to the selected location from the configuration file.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="configurationName">The BaseTypeHandlerDefaultSection Extension name.</param>
        public virtual void WriteType(string message, string configurationName)
        {
            // Make sure the page reference exists.
            if (configurationName == null) throw new ArgumentNullException("configurationName");
            if (message == null) throw new ArgumentNullException("message");

            try
            {
                // Get the configuration section
                BaseTypeHandlerElement[] typeHandler = BaseTypeHandlerConfigurationManager.BaseTypeHandlerElements();
                if (typeHandler != null)
                {
                    if (typeHandler.Length > 0)
                    {
                        // Find the first name and write to the location.
                        BaseTypeHandlerElement item = typeHandler.First(u => u.Name == configurationName);
                        Type typeNamespace = Nequeo.Reflection.TypeAccessor.GetType(item.TypeName);
                        this.WriteErrorEx(typeNamespace.Name, item.TypeMemberName, message, 0, 0, item.WriteTo, item.LogType);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Write a message to the selected location from the configuration file.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="configurationName">The BaseTypeHandlerDefaultSection Extension name.</param>
        /// <param name="code">A custom code identifier.</param>
        public virtual void WriteType(string message, string configurationName, int code)
        {
            // Make sure the page reference exists.
            if (configurationName == null) throw new ArgumentNullException("configurationName");
            if (message == null) throw new ArgumentNullException("message");

            try
            {
                // Get the configuration section
                BaseTypeHandlerElement[] typeHandler = BaseTypeHandlerConfigurationManager.BaseTypeHandlerElements();
                if (typeHandler != null)
                {
                    if (typeHandler.Length > 0)
                    {
                        // Find the first name and write to the location.
                        BaseTypeHandlerElement item = typeHandler.First(u => u.Name == configurationName);
                        Type typeNamespace = Nequeo.Reflection.TypeAccessor.GetType(item.TypeName);
                        this.WriteErrorEx(typeNamespace.Name, item.TypeMemberName, message, 0, code, item.WriteTo, item.LogType);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Write the message to the event log.
        /// </summary>
        /// <param name="message">String, the error message to write.</param>
        public virtual void WriteToEventLog(string message)
        {
            // Make sure the page reference exists.
            if (message == null) throw new ArgumentNullException("message");

            try
            {
                _exceptionMessage = message;

                // Get the current machine name.
                string machineName = System.Environment.MachineName.ToString();
                using (EventLog eventLog = new EventLog(_applicationName, machineName, _eventName))
                {
                    // Write event to the event log.
                    eventLog.WriteEntry(message, System.Diagnostics.EventLogEntryType.Information);
                }
            }
            catch (System.Exception e)
            { this.GeneralError("Write to event log exception : " + e.Message, message); }
        }

        /// <summary>
        /// Write the message to the event log.
        /// </summary>
        /// <param name="applicationName">The application name when logging.</param>
        /// <param name="eventNamespace">The event namespace when logging.</param>
        /// <param name="className">String, the class that the error occurred in.</param>
        /// <param name="method">String, the method that the error occurred in.</param>
        /// <param name="message">String, the error message to write.</param>
        /// <param name="lineNumber">Integer, the line number of the error.</param>
        /// <param name="code">Integer, the error/processing code for the operation.</param>
        /// <param name="writeTo">WriteTo, write log to location.</param>
        /// <param name="logType">Enum, the log file type.</param>
        public static void WriteMessage(string applicationName, string eventNamespace, 
            string className, string method, string message, int lineNumber,
            int code, WriteTo writeTo, LogType logType)
        {
            // Make sure the page reference exists.
            if (applicationName == null) throw new ArgumentNullException("applicationName");
            if (eventNamespace == null) throw new ArgumentNullException("eventNamespace");
            if (className == null) throw new ArgumentNullException("className");
            if (method == null) throw new ArgumentNullException("method");
            if (message == null) throw new ArgumentNullException("message");

            LogHandler error = new LogHandler(applicationName, eventNamespace);
            error.Write(className, method, message, lineNumber, code, writeTo, logType);
        }

        /// <summary>
        /// Write a message to the selected location from the configuration file.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="member">The current member information.</param>
        public static void WriteTypeMessage(string message, MemberInfo member)
        {
            // Make sure the page reference exists.
            if (message == null) throw new ArgumentNullException("message");
            if (member == null) throw new ArgumentNullException("member");

            bool foundLoggingAttr = false;
            List<Object> attributes = new List<object>();
            string configurationName = string.Empty;
            WriteTo writeTo = Nequeo.Handler.WriteTo.None;
            LogType logType = Nequeo.Handler.LogType.Error;

            try
            {
                // Get the attributes.
                attributes.AddRange(member.GetCustomAttributes(false));
                attributes.AddRange(member.DeclaringType.GetCustomAttributes(false));

                if (attributes.Count() < 1)
                    throw new System.Exception("No 'Nequeo.Handler.LoggingAttribute' has been applied to the member.");

                // For each attribute on each property
                // in the type.
                foreach (object attribute in attributes)
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is Nequeo.Handler.LoggingAttribute)
                    {
                        // Cast the current attribute.
                        Nequeo.Handler.LoggingAttribute att =
                            (Nequeo.Handler.LoggingAttribute)attribute;

                        configurationName = att.ConfigurationName;
                        writeTo = att.WriteTo;
                        logType = att.LogType;
                        foundLoggingAttr = true;
                        break;
                    }
                }

                // Make sure that the logging attribite has been assigned.
                if(!foundLoggingAttr)
                    throw new System.Exception("No 'Nequeo.Handler.LoggingAttribute' has been applied to the member.");

                try
                {
                    // If a configuration file has been specified.
                    if (!String.IsNullOrEmpty(configurationName))
                    {
                        // Get the configuration section
                        BaseTypeHandlerElement[] typeHandler = BaseTypeHandlerConfigurationManager.BaseTypeHandlerElements();
                        if (typeHandler != null)
                        {
                            if (typeHandler.Length > 0)
                            {
                                // Find the first name and write to the location.
                                BaseTypeHandlerElement item = typeHandler.First(u => u.Name == configurationName);
                                Type typeNamespace = Nequeo.Reflection.TypeAccessor.GetType(item.TypeName);

                                LogHandler error = new LogHandler(item.AssemblyName, typeNamespace.FullName);
                                error.Write(typeNamespace.Name, item.TypeMemberName, message, 0, 0, item.WriteTo, item.LogType);
                            }
                        }
                    }
                    else
                    {
                        LogHandler error = new LogHandler(member.Module.Assembly.FullName, member.DeclaringType.Namespace);
                        error.Write(member.DeclaringType.Name, member.Name, message, 0, 0, writeTo, logType);
                    }
                }
                catch { }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Write a message to the selected location from the configuration file.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="member">The current member information.</param>
        /// <param name="eventApplicationName">The custom event application name.</param>
        public static void WriteTypeMessage(string message, MemberInfo member, string eventApplicationName = "")
        {
            // Make sure the page reference exists.
            if (message == null) throw new ArgumentNullException("message");
            if (member == null) throw new ArgumentNullException("member");

            bool foundLoggingAttr = false;
            List<Object> attributes = new List<object>();
            string configurationName = string.Empty;
            WriteTo writeTo = Nequeo.Handler.WriteTo.None;
            LogType logType = Nequeo.Handler.LogType.Error;

            try
            {
                // Get the attributes.
                attributes.AddRange(member.GetCustomAttributes(false));
                attributes.AddRange(member.DeclaringType.GetCustomAttributes(false));

                if (attributes.Count() < 1)
                    throw new System.Exception("No 'Nequeo.Handler.LoggingAttribute' has been applied to the member.");

                // For each attribute on each property
                // in the type.
                foreach (object attribute in attributes)
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is Nequeo.Handler.LoggingAttribute)
                    {
                        // Cast the current attribute.
                        Nequeo.Handler.LoggingAttribute att =
                            (Nequeo.Handler.LoggingAttribute)attribute;

                        configurationName = att.ConfigurationName;
                        writeTo = att.WriteTo;
                        logType = att.LogType;
                        foundLoggingAttr = true;
                        break;
                    }
                }

                // Make sure that the logging attribite has been assigned.
                if (!foundLoggingAttr)
                    throw new System.Exception("No 'Nequeo.Handler.LoggingAttribute' has been applied to the member.");

                try
                {
                    // If a configuration file has been specified.
                    if (!String.IsNullOrEmpty(configurationName))
                    {
                        // Get the configuration section
                        BaseTypeHandlerElement[] typeHandler = BaseTypeHandlerConfigurationManager.BaseTypeHandlerElements();
                        if (typeHandler != null)
                        {
                            if (typeHandler.Length > 0)
                            {
                                // Find the first name and write to the location.
                                BaseTypeHandlerElement item = typeHandler.First(u => u.Name == configurationName);
                                Type typeNamespace = Nequeo.Reflection.TypeAccessor.GetType(item.TypeName);

                                LogHandler error = new LogHandler(item.AssemblyName, typeNamespace.FullName);
                                error._applicationName = item.AssemblyName;
                                error.Write(typeNamespace.Name, item.TypeMemberName, message, 0, 0, item.WriteTo, item.LogType);
                            }
                        }
                    }
                    else
                    {
                        LogHandler error = new LogHandler(
                            (String.IsNullOrEmpty(eventApplicationName) ?
                                member.Module.Assembly.GetName().Name : eventApplicationName),
                            member.DeclaringType.Namespace);
                        error.Write(member.DeclaringType.Name, member.Name, message, 0, 0, writeTo, logType);
                    }
                }
                catch { }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Write a message to the selected location from the configuration file.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="configurationName">The BaseTypeHandlerDefaultSection Extension name.</param>
        public static void WriteTypeMessage(string message, string configurationName)
        {
            // Make sure the page reference exists.
            if (configurationName == null) throw new ArgumentNullException("configurationName");
            if (message == null) throw new ArgumentNullException("message");

            try
            {
                // Get the configuration section
                BaseTypeHandlerElement[] typeHandler = BaseTypeHandlerConfigurationManager.BaseTypeHandlerElements();
                if (typeHandler != null)
                {
                    if (typeHandler.Length > 0)
                    {
                        // Find the first name.
                        BaseTypeHandlerElement item = typeHandler.First(u => u.Name == configurationName);
                        Type typeNamespace = Nequeo.Reflection.TypeAccessor.GetType(item.TypeName);

                        // Write the message.
                        LogHandler error = new LogHandler(item.AssemblyName, typeNamespace.FullName);
                        error.Write(typeNamespace.Name, item.TypeMemberName, message, 0, 0, item.WriteTo, item.LogType);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Write a message to the selected location from the configuration file.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="configurationName">The BaseTypeHandlerDefaultSection Extension name.</param>
        /// <param name="code">A custom code identifier.</param>
        public static void WriteTypeMessage(string message, string configurationName, int code)
        {
            // Make sure the page reference exists.
            if (configurationName == null) throw new ArgumentNullException("configurationName");
            if (message == null) throw new ArgumentNullException("message");

            try
            {
                // Get the configuration section
                BaseTypeHandlerElement[] typeHandler = BaseTypeHandlerConfigurationManager.BaseTypeHandlerElements();
                if (typeHandler != null)
                {
                    if (typeHandler.Length > 0)
                    {
                        // Find the first name.
                        BaseTypeHandlerElement item = typeHandler.First(u => u.Name == configurationName);
                        Type typeNamespace = Nequeo.Reflection.TypeAccessor.GetType(item.TypeName);

                        // Write the message.
                        LogHandler error = new LogHandler(item.AssemblyName, typeNamespace.FullName);
                        error.Write(typeNamespace.Name, item.TypeMemberName, message, 0, code, item.WriteTo, item.LogType);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Write a message to the application tab of the event log.
        /// </summary>
        /// <param name="applicationName">The application name when logging.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="entryType">The entry type to log.</param>
        public static void WriteMessageToApplicationEventLog(
            string applicationName, string message, System.Diagnostics.EventLogEntryType entryType)
        {
            // Make sure the page reference exists.
            if (applicationName == null) throw new ArgumentNullException("applicationName");
            if (message == null) throw new ArgumentNullException("message");

            try
            {
                // Get the current machine name.
                string machineName = System.Environment.MachineName.ToString();
                using (EventLog eventLog = new EventLog("Application", machineName, applicationName))
                {
                    // Write event to the event log.
                    eventLog.WriteEntry(message, entryType);
                }
            }
            catch { }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Method, writes to the selected error reporting location.
        /// </summary>
        /// <param name="className">String, the class that the error occurred in.</param>
        /// <param name="method">String, the method that the error occurred in.</param>
        /// <param name="message">String, the error message to write.</param>
        /// <param name="lineNumber">Integer, the line number of the error.</param>
        /// <param name="code">Integer, the error/processing code for the operation.</param>
        /// <param name="writeTo">WriteTo, write log to location.</param>
        /// <param name="logType">Enum, the log file type.</param>
        private void WriteErrorEx(string className, string method, string message, int lineNumber,
            int code, WriteTo writeTo, LogType logType)
        {
            WriteTo writeToLocation = writeTo;

            // If the default location has changed.
            if (_overrideLocation != Nequeo.Handler.WriteTo.Default)
                writeToLocation = _overrideLocation;

            // Where should the message be eritten to.
            switch (writeToLocation)
            {
                case Nequeo.Handler.WriteTo.EventLog:
                    // Write a message to the specified location.
                    this.WriteErrorToEventLog(className, method, message, lineNumber, code, logType);
                    break;

                case Nequeo.Handler.WriteTo.File:
                    // Write the exception stack with all contained information to the specified location.
                    this.WriteErrorToFile(className, method, message, lineNumber, code, logType);
                    break;

                case Nequeo.Handler.WriteTo.FileAndEventLog:
                    // Write the exception stack with all contained information to the specified location.
                    this.WriteErrorToEventLog(className, method, message, lineNumber, code, logType);
                    this.WriteErrorToFile(className, method, message, lineNumber, code, logType);
                    break;

                case Nequeo.Handler.WriteTo.None:
                    // Return the current process message
                    if (logType == Nequeo.Handler.LogType.Process)
                        _processMessage = message;
                        
                    break;

                default:
                    // No message index was specified.
                    _exceptionMessage = "Class: Handler, Method: WriteErrorEx. The specified WriteTo enum does not exist.\n";
                    break;
            }

            // If the current log type is an error.
            if (logType == Nequeo.Handler.LogType.Error)
            {
                _exceptionMessage = message;

                // Make sure than a receiver instance of the
                // event delegate handler was created.
                if (OnError != null)
                    // Fire the the event through the ErrorMessageHandler delegate.
                    OnError(this, new ErrorMessageArgs(message));
            }
        }

        /// <summary>
        /// Write the message to the event log.
        /// </summary>
        /// <param name="className">String, the class that the error occurred in.</param>
        /// <param name="method">String, the method that the error occurred in.</param>
        /// <param name="message">String, the error message to write.</param>
        /// <param name="lineNumber">Integer, the line number of the error.</param>
        /// <param name="code">Integer, the error/processing code for the operation.</param>
        /// <param name="logType">Enum, the log file type.</param>
        private void WriteErrorToEventLog(string className, string method,
            string message, int lineNumber, int code, LogType logType)
        {
            string errorMessage = string.Empty;

            try
            {
                errorMessage = DateTime.Now.ToString() + ",\r\n Application: " + _applicationName +
                    ",\r\n Event Namespace: " + _eventName + ",\r\n Class: " + className +
                    ",\r\n Member: " + method + ",\r\n Message: " + message + ",\r\n LineNumber : " + lineNumber.ToString() +
                    ",\r\n Code : " + code.ToString();

                // Get the current machine name.
                string machineName = System.Environment.MachineName.ToString();
                using (EventLog eventLog = new EventLog(_applicationName, machineName, _eventName))
                {
                    // Get the current message type.
                    switch (logType)
                    {
                        case Nequeo.Handler.LogType.Error:
                            // Write event to the event log.
                            eventLog.WriteEntry(errorMessage, System.Diagnostics.EventLogEntryType.Error);
                            break;
                        case Nequeo.Handler.LogType.Process:
                            // Write event to the event log.
                            eventLog.WriteEntry(errorMessage, System.Diagnostics.EventLogEntryType.Information);
                            break;
                    }
                }
            }
            catch (System.Exception e)
            { this.GeneralError("Write to event log exception : " + e.Message, errorMessage); }
        }

        /// <summary>
        /// Method, write the error to the file.
        /// </summary>
        /// <param name="className">String, the class that the error occurred in.</param>
        /// <param name="method">String, the method that the error occurred in.</param>
        /// <param name="message">String, the error message to write.</param>
        /// <param name="lineNumber">Integer, the line number of the error.</param>
        /// <param name="code">Integer, the error/processing code for the operation.</param>
        /// <param name="logType">Enum, the log file type.</param>
        /// <remarks></remarks>
        private void WriteErrorToFile(string className, string method, 
            string message, int lineNumber, int code, LogType logType)
        {
            string errorMessage = DateTime.Now.ToString() + ", Application: " + _applicationName +
                    ", Event Namespace: " + _eventName + ", Class: " + className +
                    ", Member: " + method + ", Message: " + message + ", LineNumber : " + lineNumber.ToString() +
                    ", Code : " + code.ToString();

            // If the log file has not been set
            if (_fileNameAndPathError == string.Empty)
            {
                BaseHandlerElement handlerReader = BaseHandlerConfigurationReader;
                if (handlerReader != null)
                {
                    _exceptionMessage = "File name and path not set in; Property : LogErrorFilePath. set this property before applying";
                    this.GeneralError("Write to file exception : File name and path not set in; Property : LogErrorFilePath", errorMessage);

                    if(handlerReader.ErrorLogFilePathIsRelative)
                        _fileNameAndPathError = "\\" + handlerReader.ErrorLogFilePath + "\\Error.log";
                    else
                        _fileNameAndPathError = handlerReader.ErrorLogFilePath + "\\Error.log";
                }
                else
                {
                    _exceptionMessage = "File name and path not set in; Property : LogErrorFilePath. set this property before applying";
                    this.GeneralError("Write to file exception : File name and path not set in; Property : LogErrorFilePath", errorMessage);
                    _fileNameAndPathError = "\\Nequeo\\Error.log";
                }
            }

            // If the log file has not been set
            if (_fileNameAndPathProcessing == string.Empty)
            {
                BaseHandlerElement handlerReader = BaseHandlerConfigurationReader;
                if (handlerReader != null)
                {
                    _processMessage = "File name and path not set in; Property : LogProcessFilePath. set this property before applying";
                    this.GeneralError("Write to file exception : File name and path not set in; Property : LogProcessFilePath", errorMessage);

                    if (handlerReader.ProcessLogFilePathIsRelative)
                        _fileNameAndPathProcessing = "\\" + handlerReader.ProcessLogFilePath + "\\Process.log";
                    else
                        _fileNameAndPathProcessing = handlerReader.ProcessLogFilePath + "\\Process.log";
                }
                else
                {
                    _processMessage = "File name and path not set in; Property : LogProcessFilePath. set this property before applying";
                    this.GeneralError("Write to file exception : File name and path not set in; Property : LogProcessFilePath", errorMessage);
                    _fileNameAndPathProcessing = "\\Nequeo\\Process.log";
                }
            }

            // lock ensure that no other threads try to 
            // use the stream at the same time.
            lock (_logFileThread)
            {
                long errorMaxFileSize = (long)1;
                long processMaxFileSize = (long)1;
                BaseHandlerElement handlerReader = BaseHandlerConfigurationReader;
                if (handlerReader != null)
                {
                    errorMaxFileSize = Convert.ToInt64(handlerReader.ErrorLogFileMaxSize);
                    processMaxFileSize = Convert.ToInt64(handlerReader.ProcessLogFileMaxSize);
                }
                
                switch (logType)
                {
                    case Nequeo.Handler.LogType.Error:
                        _logFileThread.WriteMessageToLogFile(errorMessage, this.LogErrorFilePath, errorMaxFileSize);
                        break;
                    case Nequeo.Handler.LogType.Process:
                        _logFileThread.WriteMessageToLogFile(errorMessage, this.LogProcessFilePath, processMaxFileSize);
                        break;
                }
            }
        }

        /// <summary>
        /// Write the base level error message to the event log.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="originalMessage">The original message that was sent.</param>
        private void GeneralError(string message, string originalMessage)
        {
            try
            {
                // Get the current machine name.
                string machineName = System.Environment.MachineName.ToString();
                using (EventLog eventLog = new EventLog("Application", machineName, _applicationName))
                {
                    // Write event to the event log.
                    eventLog.WriteEntry(message + " OriginalMessage : " + originalMessage,
                        System.Diagnostics.EventLogEntryType.Information);
                }
            }
            catch { }
        }
        #endregion

        #region Dispose Object Methods.
        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
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
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _logFileThread = null;

                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~LogHandler()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion

    }

    /// <summary>
    /// Offline threaded class when writting to
    /// a file containing a message.
    /// </summary>
    internal class WriteToLogFileThread
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public WriteToLogFileThread()
        {
        }
        #endregion

        #region Private Fields
        private string _applicationName = "Nequeo";
        private string _eventName = "Nequeo.Handler";
        #endregion

        #region Private Methods
        /// <summary>
        /// Write the message to the event log.
        /// </summary>
        /// <param name="className">String, the class that the error occurred in.</param>
        /// <param name="method">String, the method that the error occurred in.</param>
        /// <param name="message">String, the error message to write.</param>
        /// <param name="lineNumber">Integer, the line number of the error.</param>
        private void WriteErrorToEventLog(string className, string method, string message, int lineNumber)
        {
            string errorMessage = string.Empty;
            try
            {
                errorMessage = DateTime.Now.ToString() + ", Application: " + _applicationName +
                    ", Event Namespace: " + _eventName + ", Class: " + className +
                    ", Method: " + method + ", Message: " + message + ", LineNumber : " + lineNumber.ToString();

                // Get the current machine name.
                string machineName = System.Environment.MachineName.ToString();
                using (EventLog eventLog = new EventLog(_applicationName, machineName, _eventName))
                {
                    // Write event to the event log.
                    eventLog.WriteEntry(errorMessage, System.Diagnostics.EventLogEntryType.Information);
                }
            }
            catch { }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Write the specified message to the file.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="file">The name of the file.</param>
        /// <param name="maxFileSize">The maximun file size before creating a new file.</param>
        public void WriteMessageToLogFile(string message, string file, long maxFileSize)
        {
            string sFolderPath = string.Empty;

            // Check the current log file size
            // if the log file size is larger
            // than 1 MB then rename the current
            // log file start a new log file.
            try
            {
                // Extract the directory path.
                sFolderPath = System.IO.Path.GetDirectoryName(file);

                // If the file exists.
                if (File.Exists(file))
                {
                    // Get the file size.
                    FileInfo oFileInfo = new FileInfo(file);
                    long lFileSize = oFileInfo.Length;

                    // If the maximum file size has been reached.
                    if (lFileSize > (maxFileSize * ((long)(1000000))))
                    {
                        try
                        {
                            // Get the date and time, this will 
                            // be added to the file name when
                            // the file is renamed.
                            string sDate = System.DateTime.Now.Day.ToString() + "-" +
                                System.DateTime.Now.Month.ToString() + "-" + System.DateTime.Now.Year.ToString();
                            string dTime = System.DateTime.Now.Hour.ToString() + "-" +
                                System.DateTime.Now.Minute.ToString() + "-" + System.DateTime.Now.Second.ToString();

                            // Moving the file to the
                            // same folder is the same as
                            // renaming the file. 
                            File.Move(file, sFolderPath + "\\" + System.IO.Path.GetFileNameWithoutExtension(file) +
                                "_" + sDate + "_" + dTime + ".log");
                        }
                        catch { }
                    }
                }
            }
            catch (System.Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                // Write the error to the specified location.
                StackTrace st = new StackTrace(e, true);
                WriteErrorToEventLog("WriteToLogFileThread", "WriteMessageToLogFile Move File",
                    "Write to file exception : " + e.Message + " " + message,
                    st.GetFrame(0).GetFileLineNumber());
            }

            // Write the message to the current
            // log file. Write error message
            // if the message can not be written
            // to the log file.
            FileStream fsData = null;
            StreamWriter swWriter = null;

            try
            {
                // If the directory does not exists create it.
                if (!Directory.Exists(sFolderPath))
                    Directory.CreateDirectory(sFolderPath);

                // Create new instances of the file management.
                using (fsData = new FileStream(file, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (swWriter = new StreamWriter(fsData))
                {
                    // Write the current message to the log file.
                    // add a carriage return and line feed.
                    swWriter.WriteLine(DateTime.Now.ToShortDateString() + " " +
                        DateTime.Now.ToShortTimeString() + ", " + message + (char)13 + (char)10);
                    swWriter.Flush();

                    swWriter.Close();
                    fsData.Close();
                }
            }
            catch (System.Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                // Write the error to the specified location.
                StackTrace st = new StackTrace(e, true);
                WriteErrorToEventLog("WriteToLogFileThread", "WriteMessageToLogFile",
                    "Write to file exception : " + e.Message + " " + message,
                    st.GetFrame(0).GetFileLineNumber());
            }
            finally
            {
                // Close and clean up.
                if (swWriter != null)
                    swWriter.Close();

                if (fsData != null)
                    fsData.Close();
            }
        }
        #endregion
    }
}
