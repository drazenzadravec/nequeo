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
using System.Diagnostics;
using System.Reflection;

namespace Nequeo.Web.Mvc.Extended.WebAsset
{
    /// <summary>
    /// Contains default web asset settings.
    /// </summary>
    public static class WebAssetDefaultSettings
    {
        private static string _styleSheetFilesPath = "~/Content";
        private static string _scriptFilesPath = "~/Scripts";
        private static float _cacheDurationInDays = 365f;
        private static string _version = new AssemblyName(typeof(WebAssetDefaultSettings).Assembly.FullName).Version.ToString(3);
        private static bool _compress = true;

        /// <summary>
        /// Gets or sets the style sheet files path. Path must be a virtual path.
        /// </summary>
        public static string StyleSheetFilesPath
        {
            get { return _styleSheetFilesPath; }
            set { _styleSheetFilesPath = value; }
        }

        /// <summary>
        /// Gets or sets the script files path. Path must be a virtual path.
        /// </summary>
        public static string ScriptFilesPath
        {
            get { return _scriptFilesPath; }
            set { _scriptFilesPath = value; }
        }

        /// <summary>
        /// Gets or sets the cache duration in days.
        /// </summary>
        public static float CacheDurationInDays
        {
            get { return _cacheDurationInDays; }
            set { _cacheDurationInDays = value; }
        }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public static string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether assets should be served as compressed.
        /// </summary>
        /// <value>True if compress; otherwise, false.</value>
        public static bool Compress
        {
            get { return _compress; }
            set { _compress = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether assets shoule be combined.
        /// </summary>
        /// <value>True if combined; otherwise, false.</value>
        public static bool Combined { get; set; }
    }
}
