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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Configuration;

namespace Nequeo.Net.Http.PostBackService.Common
{
    /// <summary>
    /// Internal helper
    /// </summary>
    internal class Helper
    {
        /// <summary>
        /// The application path.
        /// </summary>
        public static string ApplicationName = @"C:\Temp\Assembly.dll";

        /// <summary>
        /// Application event source name when logging to the event log.
        /// </summary>
        /// <returns>Application event source name when logging to the event log.</returns>
        public static string EventApplicationName()
        {
            return GetSettingsValue("EventApplicationName");
        }

        /// <summary>
        /// Base Download Path
        /// </summary>
        /// <returns>Base Download Path</returns>
        public static string BaseDownloadPath()
        {
            return GetSettingsValue("BaseDownloadPath");
        }

        /// <summary>
        /// MaxBaseUploadContentSize
        /// </summary>
        /// <returns>MaxBaseUploadContentSize</returns>
        public static string MaxBaseUploadContentSize()
        {
            return GetSettingsValue("MaxBaseUploadContentSize");
        }

        /// <summary>
        /// NotesFormServiceBasePath
        /// </summary>
        /// <returns>NotesFormServiceBasePath</returns>
        public static string NotesFormServiceBasePath()
        {
            return GetSettingsValue("NotesFormServiceBasePath");
        }

        /// <summary>
        /// Get the specific string value.
        /// </summary>
        /// <param name="elementKey">The element key.</param>
        /// <returns>The string value.</returns>
        private static string GetSettingsValue(string elementKey)
        {
            // Get setting data.
            string valueString = LoadConfigurationFile().Settings.Get(elementKey).Value.ValueXml.InnerText;

            // Return the event application name
            return valueString;
        }

        /// <summary>
        /// Load the configuration file and the client settings section.
        /// </summary>
        /// <returns>The client settings section.</returns>
        private static System.Configuration.ClientSettingsSection LoadConfigurationFile()
        {
            // Return the client section.
            return Nequeo.Configuration.Manager.GetClientSettings(ApplicationName, "applicationSettings/Nequeo.Net.Http.PostBackService.Properties.Settings");
        }
    }
}
