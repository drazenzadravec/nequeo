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
using System.Data;
using System.Security.Permissions;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Web.Compilation;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

using Nequeo.Handler;
using Nequeo.Data.Configuration;
using Nequeo.Web.Configuration;

namespace Nequeo.Web.HttpHandler
{
    /// <summary>
    /// File upload handler allows the upload of one-to-many file uploads.
    /// </summary>
    public class FileUpload : IHttpHandler
    {
        /// <summary>
        /// Set which http handler should return data.
        /// </summary>
        public object HttpHandlerType = null;

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the IHttpHandler interface.
        /// </summary>
        /// <param name="context">An HttpContext object that provides references to the 
        /// intrinsic server objects (for example, Request, Response, Session, and Server)
        /// used to service HTTP requests.</param>
        [Logging(Nequeo.Handler.WriteTo.EventLog, Nequeo.Handler.LogType.Error)]
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                int numberOfFilesSaves = 0;

                // Get the collection of http handler configuration extensions
                HttpHandlerUploadExtensionElement[] httpCollection = HttpHandlerUploadConfigurationManager.HttpHandlerExtensionElements();
                if (httpCollection != null)
                {
                    // If http extensions exist
                    if (httpCollection.Count() > 0)
                    {
                        // For each configuration
                        foreach (HttpHandlerUploadExtensionElement item in httpCollection)
                        {
                            // Get the current http handler type
                            // and create a instance of the type.
                            Type httpHandlerType = BuildManager.GetType(item.HttpHandlerTypeName, true, true);
                            object httpHandlerTypeInstance = Activator.CreateInstance(httpHandlerType);

                            if (HttpHandlerType == null)
                                HttpHandlerType = this;

                            if (HttpHandlerType != null)
                            {
                                if (HttpHandlerType.GetType().FullName.ToLower() == httpHandlerTypeInstance.GetType().FullName.ToLower())
                                {
                                    // Get the number of files that a being uploaded.
                                    HttpFileCollection files = context.Request.Files;
                                    if (files != null)
                                    {
                                        // For each file found.
                                        for(int i = 0; i < files.Count; i++)
                                        {
                                            try
                                            {
                                                // Get the current file and file name
                                                // attempt to save the file to the upload path.
                                                HttpPostedFile file = context.Request.Files[i];
                                                string fileName = System.IO.Path.GetFileName(file.FileName);
                                                file.SaveAs(item.UploadPath + "\\" + fileName);

                                                // Respond with the file being saved.
                                                context.Response.ContentType = "text/plain";
                                                context.Response.Write("File: {" + fileName + "} saved successfully");

                                                // Increment by one.
                                                numberOfFilesSaves++;
                                            }
                                            catch (Exception ex) { context.AddError(ex); }
                                        }
                                    }
                                }
                            }
                            else break;
                        }
                    }
                }

                // If files have been uploaded.
                if (numberOfFilesSaves > 0)
                {
                    try
                    {
                        // Respond with the number of files saved.
                        context.Response.ContentType = "text/plain";
                        context.Response.Write(numberOfFilesSaves.ToString() + " file(s) have been saved !");
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        // Respond with no files have been saved.
                        context.Response.ContentType = "text/plain";
                        context.Response.Write("No files have been saved !");
                    }
                    catch { }
                }
            }
            catch (Exception ex) 
            { 
                try
                {
                    // Respond with the error.
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("Error: " + ex.Message);
                }
                catch { }

                context.AddError(ex);
                LogHandler.WriteTypeMessage(ex.Message, typeof(FileUpload).GetMethod("ProcessRequest"));
            }
        }

        /// <summary>
        /// Gets a value indicating whether another request can use the IHttpHandler instance.
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }
    }
}
