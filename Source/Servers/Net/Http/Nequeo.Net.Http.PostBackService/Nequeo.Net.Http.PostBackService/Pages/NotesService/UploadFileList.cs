/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using System.Net;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

using Nequeo.Handler;
using Nequeo.ComponentModel.Composition;
using Nequeo.Net.Http.PostBackService.Common;
using Nequeo.Net.Http.Common;

namespace Nequeo.Net.Http.PostBackService.Pages.NotesService
{
    /// <summary>
    /// Notes page implementation.
    /// </summary>
    [Logging(Handler.WriteTo.EventLog, Handler.LogType.Error)]
    internal class UploadFileList : HttpPageBase
    {
        /// <summary>
        /// The page is initialised.
        /// </summary>
        /// <param name="pageContext">The page content.</param>
        public override void OnInit(HttpPageContext pageContext)
        {
        }

        /// <summary>
        /// The page is loading.
        /// </summary>
        /// <param name="pageContext">The page content.</param>
        public override void OnLoad(HttpPageContext pageContext)
        {
            bool deleteFiles = true;
            bool deleteDirectories = true;

            // Get the download file directory.
            string directoryQuery = "";
            if (base.Request.QueryString != null)
            {
                if (!string.IsNullOrEmpty(base.Request.QueryString["directory"]))
                    directoryQuery = base.Request.QueryString["directory"];

                // If the delete file query exists
                if (!String.IsNullOrEmpty(base.Request.QueryString["deletefile"]))
                {
                    // Get the file to delete path.
                    string fileNameToDelete = base.UploadDirectory + base.Request.QueryString["deletefile"].Replace("/", "\\");

                    // If the file exists then delete the file.
                    if (System.IO.File.Exists(fileNameToDelete))
                        System.IO.File.Delete(fileNameToDelete);
                }

                // If the delete directory query exists
                if (!String.IsNullOrEmpty(base.Request.QueryString["deletedirectory"]))
                {
                    // Get the directory to delete path.
                    string directoryToDelete = base.UploadDirectory + base.Request.QueryString["deletedirectory"].Replace("/", "\\").TrimStart('\\') + "\\";

                    // If the directory exists then delete the directory.
                    if (System.IO.Directory.Exists(directoryToDelete))
                        System.IO.Directory.Delete(directoryToDelete, true);
                }
            }
                
            // Get the file system html.
            base.AlternativeContent = HttpResponseContent.UploadFileList(base.Response, base.UploadDirectory,
                System.IO.Path.GetFileName(base.UrlFilePath), directoryQuery, deleteFiles, deleteDirectories);
        }

        /// <summary>
        /// The pre-process event.
        /// </summary>
        /// <param name="pageContext">The page content.</param>
        public override void OnPreProcess(HttpPageContext pageContext)
        {
            // Get the download file directory.
            base.UploadDirectory = Helper.BaseDownloadPath().TrimEnd('\\') + "\\";
            pageContext.ProcessOnPostBack = false;
        }
    }
}
