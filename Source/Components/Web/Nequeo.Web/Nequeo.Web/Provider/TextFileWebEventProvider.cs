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
using System.Web.Management;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System.Web.Hosting;
using System.IO;
using System.Security.Permissions;
using System.Web;

using Nequeo.Handler.Global;

namespace Nequeo.Web.Provider
{
    /// <summary>
    /// Text file web event provider
    /// </summary>
    [FileIOPermission(SecurityAction.Demand, Write = "true")]
    public sealed class TextFileWebEventProvider : WebEventProvider
    {
        #region Private Fields
        private string _LogFileName;
        #endregion

        #region Abstract Method Overrides
        /// <summary>
        /// Initialise the new session state.
        /// </summary>
        /// <param name="name">The name of the session state.</param>
        /// <param name="config">The name value collection.</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            // Verify that config isn't null
            if (config == null)
                throw new ArgumentNullException("config");

            // Assign the provider a default name if it doesn't have one
            if (String.IsNullOrEmpty(name))
                name = "TextFileWebEventProvider";

            // Add a default "description" attribute to config if the
            // attribute doesn’t exist or is empty
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Text file Web event provider");
            }

            // Call the base class's Initialize method
            base.Initialize(name, config);

            // Initialize _LogFileName and make sure the path is app-relative
            string path = config["logFileName"];

            if (String.IsNullOrEmpty(path))
                throw new ProviderException("Missing logFileName attribute");

            if (!VirtualPathUtility.IsAppRelative(path))
                throw new ArgumentException("logFileName must be app-relative");

            string fullyQualifiedPath = VirtualPathUtility.Combine
                (VirtualPathUtility.AppendTrailingSlash(HttpRuntime.AppDomainAppVirtualPath), path);

            _LogFileName = HostingEnvironment.MapPath(fullyQualifiedPath);
            config.Remove("logFileName");

            // Make sure we have permission to write to the log file
            // throw an exception if we don't
            FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.Write |
                FileIOPermissionAccess.Append, _LogFileName);
            permission.Demand();

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                string attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                    throw new ProviderException("Unrecognized attribute: " + attr);
            }
        }

        /// <summary>
        /// Processes the event passed to the provider.
        /// </summary>
        /// <param name="raisedEvent">The System.Web.Management.WebBaseEvent object to process.</param>
        public override void ProcessEvent(WebBaseEvent raisedEvent)
        {
            // Write an entry to the log file
            LogEntry(FormatEntry(raisedEvent));
        }

        /// <summary>
        /// Moves the events from the provider's buffer into the event log.
        /// </summary>
        public override void Flush() { }

        /// <summary>
        /// Performs tasks associated with shutting down the provider.
        /// </summary>
        public override void Shutdown() { }
        #endregion

        #region Private Methods
        /// <summary>
        /// Format the eb base event data.
        /// </summary>
        /// <param name="e">The web base event that has occured.</param>
        /// <returns>The web base event string format.</returns>
        private string FormatEntry(WebBaseEvent e)
        {
            return String.Format("{0}\t{1}\t{2} (Event Code: {3})",
                e.EventTime, e.GetType().ToString(), e.Message, e.EventCode);
        }

        /// <summary>
        /// Write to the log file.
        /// </summary>
        /// <param name="entry">The data to write to the log file.</param>
        private void LogEntry(string entry)
        {
            StreamWriter writer = null;

            try
            {
                // Write the log data to the file.
                writer = new StreamWriter(_LogFileName, true);
                writer.WriteLine(entry);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }
        #endregion
    }
}
