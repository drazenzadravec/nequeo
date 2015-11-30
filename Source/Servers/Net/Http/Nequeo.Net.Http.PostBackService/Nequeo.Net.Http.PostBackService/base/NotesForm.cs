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

namespace Nequeo.Net.Http.PostBackService
{
    /// <summary>
    /// Notes web form service.
    /// </summary>
    [Logging(Handler.WriteTo.EventLog, Handler.LogType.Error)]
    public class NotesForm : HttpServiceBase, Nequeo.Net.Http.IHttpServerContext
	{
        private string _assemblyName = "Nequeo.Net.Http.PostBackService";
        private string _baseNameSpace = "Nequeo.Net.Http.PostBackService.Pages.NotesService";
        private string _baseUploadPath = null;
        private string _notesFormDirectory = null;
        private int _maxBaseContentSize = 10000000;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NotesForm()
            : base("NotesFormService")
        {
            // Get the current path of this assembly.
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            Helper.ApplicationName = assembly.Location;

            _baseUploadPath = Helper.BaseDownloadPath().TrimEnd('\\') + "\\";
            _notesFormDirectory = Helper.NotesFormServiceBasePath().TrimEnd('\\') + "\\";
            _maxBaseContentSize = Int32.Parse(Helper.MaxBaseUploadContentSize());
        }

        /// <summary>
        /// Gets or sets the service assembly name.
        /// </summary>
        public override string AssemblyName
        {
            get { return _assemblyName; }
            set { _assemblyName = value; }
        }

        /// <summary>
        /// Gets or sets the base upload path.
        /// </summary>
        public override string BaseUploadPath
        {
            get { return _baseUploadPath; }
            set { _baseUploadPath = value; }
        }

        /// <summary>
        /// Gets or sets the base service namespace.
        /// </summary>
        public override string BaseNameSpace
        {
            get { return _baseNameSpace; }
            set { _baseNameSpace = value; }
        }

        /// <summary>
        /// Gets or sets the base service path
        /// </summary>
        public override string BaseServicePath
        {
            get { return _notesFormDirectory; }
            set { _notesFormDirectory = value; }
        }

        /// <summary>
        /// Gets or sets the base maximun post back content length
        /// </summary>
        public override int MaxBaseContentSize
        {
            get { return _maxBaseContentSize; }
            set { _maxBaseContentSize = value; }
        }

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
            try
            {
                base.HttpServiceProcessRequest(context);
            }
            catch (Exception ex)
            {
                LogHandler.WriteTypeMessage(
                    ex.Message,
                    MethodInfo.GetCurrentMethod(),
                    Helper.EventApplicationName());
            }
        }
    }
}
