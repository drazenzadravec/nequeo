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
    internal class UploadFile : HttpPageBase
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
        }

        /// <summary>
        /// The pre-process event.
        /// </summary>
        /// <param name="pageContext">The page content.</param>
        public override void OnPreProcess(HttpPageContext pageContext)
        {
            // Get the download file directory.
            base.UploadDirectory = Helper.BaseDownloadPath().TrimEnd('\\') + "\\";
        }
    }
}
