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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Web.Hosting;
using System.Web;

using Nequeo.Handler;
using Nequeo.Composite.Configuration;

namespace Nequeo.Service.Transfer
{
    /// <summary>
    /// Stream HTTP context handler.
    /// </summary>
    public class StreamFileQueryHandler : IHttpHandler
    {
        private int _timeout = 30000;

        /// <summary>
        /// Gets or sets the transfer time.
        /// </summary>
        public int Timeout { get { return _timeout; } set { _timeout = value; } }

        /// <summary>
        /// Return resuse state option
        /// </summary>
        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        /// <summary>
        /// Process request method.
        /// </summary>
        /// <param name="context">The current http context.</param>
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                bool directiveFound = false;

                // Get the base path directory
                string baseDirectoryPath = Nequeo.Service.Properties.Settings.Default.BaseDirectoryPath.TrimEnd('\\') + "\\";

                // Get the collection of querys.
                NameValueCollection query = context.Request.QueryString;
                if (query == null)
                    throw new Exception("No directive has been set");

                // Is there a file 'F' directive
                if (query["F"] != null)
                {
                    directiveFound = true;

                    // Get the file directive.
                    string fileName = query["F"].ToString();

                    // Does the file exist.
                    if (File.Exists(baseDirectoryPath + fileName))
                    {
                        // Assign the content response handler
                        context.Response.ContentType = "application/" + Path.GetExtension(baseDirectoryPath + fileName).TrimStart(new char[] { '.' });
                        context.Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                        context.Response.WriteFile(baseDirectoryPath + fileName, true);
                        context.Response.Flush();
                    }
                    else
                        throw new Exception("The file '" + fileName + "' does not exist.");
                }

                // Is there a file 'D' delete directive
                if (query["D"] != null)
                {
                    directiveFound = true;

                    // Get the file directive.
                    string fileName = query["D"].ToString();

                    // Does the file exist.
                    if (System.IO.File.Exists(baseDirectoryPath + fileName))
                        System.IO.File.Delete(baseDirectoryPath + fileName);

                }

                // Is there a list 'L' directive
                if (query["L"] != null)
                {
                    directiveFound = true;

                    // Get all the files in the directory
                    string[] files = Directory.GetFiles(baseDirectoryPath);
                    StringBuilder fileList = new StringBuilder();

                    // Create the list of files.
                    foreach (string item in files)
                        fileList.Append(Path.GetFileName(item) + "\r\n");

                    // Respond with the list of files.
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(fileList.ToString());
                    context.Response.Flush();
                }

                if (!directiveFound)
                    throw new Exception("No directive has been set");
            }
            catch (Exception ex)
            {
                try
                {
                    // Respond with the error.
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("ERROR: " + ex.Message);
                    context.Response.Flush();
                }
                catch { }
            }
        }
    }
}
