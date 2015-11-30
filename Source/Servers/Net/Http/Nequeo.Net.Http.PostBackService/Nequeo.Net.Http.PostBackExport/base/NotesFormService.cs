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
using System.Security;
using System.Security.Policy;
using System.Security.Principal;
using System.Runtime.Remoting;

using Nequeo.Handler;
using Nequeo.ComponentModel.Composition;
using Nequeo.Net.Http.PostBackExport.Common;

namespace Nequeo.Net.Http.PostBackExport
{
    /// <summary>
    /// Notes web form service.
    /// </summary>
    [Export(typeof(Nequeo.Net.Http.IHttpServerContext))]
    [ContentMetadata(Name = "NotesFormService", Index = 0, Description = "Notes web form service.")]
    [Logging(Handler.WriteTo.EventLog, Handler.LogType.Error)]
    public class NotesForm : Nequeo.Net.Http.IHttpServerContext
	{
        /// <summary>
        /// Default constructor.
        /// </summary>
        public NotesForm()
        {
            // Get the current path of this assembly.
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            Helper.ApplicationName = assembly.Location;

            _serviceBasePath = Helper.ServiceBasePath().TrimEnd('\\') + "\\";
        }

        private string _serviceBasePath = null;

        /// <summary>
        /// Create a new instance of this type
        /// </summary>
        /// <returns>A new instance of this type.</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Process the http server request.
        /// </summary>
        /// <param name="context">Provides access to the request and response objects.</param>
        public void ProcessHttpRequest(IHttpContext context)
        {
            // Create a new note form instance
            // and execute the process http request.
            using (var composite = new Nequeo.Net.Http.PostBackService.NotesForm())
            {
                composite.ProcessHttpRequest(context);
            }

            //AppDomain domain = null;
            //Nequeo.Net.Http.IHttpServerContext notesForm = null;

            //try
            //{
            //    domain = AppDomain.CreateDomain(Guid.NewGuid().ToString());
            //}
            //catch (Exception ex)
            //{
            //    LogHandler.WriteTypeMessage(
            //        "1 " + ex.Message,
            //        MethodInfo.GetCurrentMethod(),
            //        Helper.EventApplicationName());
            //}

            //try
            //{
            //    notesForm = (Nequeo.Net.Http.IHttpServerContext)domain.CreateInstanceFromAndUnwrap(
            //        _serviceBasePath + "Nequeo.Net.Http.PostBackService.dll", "Nequeo.Net.Http.PostBackService.NotesForm");
            //}
            //catch (Exception ex)
            //{
            //    LogHandler.WriteTypeMessage(
            //        "2 " + ex.Message,
            //        MethodInfo.GetCurrentMethod(),
            //        Helper.EventApplicationName());
            //}

            //try
            //{
            //    IHttpContext contextEx =  (IHttpContext)context.Clone();
            //    notesForm.ProcessHttpRequest(contextEx);
            //}
            //catch (Exception ex)
            //{
            //    LogHandler.WriteTypeMessage(
            //        ex.Message,
            //        MethodInfo.GetCurrentMethod(),
            //        Helper.EventApplicationName());
            //}

            //AppDomain.Unload(domain);
        }
    }
}
