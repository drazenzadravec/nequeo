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
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;

using Nequeo.Web.UI.ScriptControl.Design;
using Nequeo.ComponentModel;

//Client-side script file embedded in assembly
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Script.jquery.ajax.upload.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Script.FileUpload.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Image.uploadButton.png", "image/png")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Style.uploadStyle.css", "text/css", PerformSubstitution = true)]

namespace Nequeo.Web.UI.ScriptControl
{
    /// <summary>
    /// File upload web service script control.
    /// </summary>
    [ToolboxBitmap(typeof(FileUpload))]
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:FileUpload runat=server></{0}:FileUpload>")]
    [Description("Nequeo Web File Upload")]
    [DisplayName("File Upload")]
    [Designer(typeof(FileUploadDesigner))]
    public class FileUpload : System.Web.UI.ScriptControl, INamingContainer
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public FileUpload() { }

        #endregion

        #region Private Fields
        private string _targetControlID = string.Empty;
        private string _responseControlID = string.Empty;
        private string _errorControlID = string.Empty;

        private string _fileExtensionFilter = string.Empty;
        private string _urlAction = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the view state text value.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }
            set { ViewState["Text"] = value; }
        }

        /// <summary>
        /// Gets sets, the ID of the target control id.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The ID of the target control id.")]
        [Editor(typeof(AllWebControlsEditor), typeof(UITypeEditor))]
        public string TargetControlID
        {
            get { return _targetControlID; }
            set { _targetControlID = value; }
        }

        /// <summary>
        /// Gets sets, the ID of the response message control.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The ID of the response message control.")]
        [Editor(typeof(AllWebControlsEditor), typeof(UITypeEditor))]
        public string ResponseControlID
        {
            get { return _responseControlID; }
            set { _responseControlID = value; }
        }

        /// <summary>
        /// Gets sets, the ID of the error message control.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The ID of the error message control.")]
        [Editor(typeof(AllWebControlsEditor), typeof(UITypeEditor))]
        public string ErrorControlID
        {
            get { return _errorControlID; }
            set { _errorControlID = value; }
        }

        /// <summary>
        /// Gets sets, the file extenstion filter.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The file extenstion filter.")]
        public string FileExtensionFilter
        {
            get { return _fileExtensionFilter; }
            set { _fileExtensionFilter = value; }
        }

        /// <summary>
        /// Gets sets, the URL action reference.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The URL action reference.")]
        public string UrlAction
        {
            get { return _urlAction; }
            set { _urlAction = value; }
        }
        #endregion

        #region Protected Render Methods
        /// <summary>
        /// When the control loads.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AjaxManager.RegisterCssReferences(this,
                new string[] { "Nequeo.Web.UI.ScriptControl.Style.uploadStyle.css" });
        }

        /// <summary>
        /// Render the control contents to the current page.
        /// </summary>
        /// <param name="writer">The html text writer.</param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            try
            {
                // If a target control has not been specified then
                // add an internal DIV control.
                if (String.IsNullOrEmpty(_targetControlID))
                {
                    // Adds HTML attributes and styles that need to be rendered to the specified
                    // System.Web.UI.HtmlTextWriterTag. This method is used primarily by control
                    // developers.
                    AddAttributesToRender(writer);

                    // Add the table attributes.
                    writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID + "_Upload");

                    // If no css style has been set then apply the internal css.
                    if (String.IsNullOrEmpty(this.CssClass))
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "uploadButton");
                    else
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, this.CssClass);

                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.Write("Upload");

                    // End the field table tag
                    writer.RenderEndTag();
                }
            }
            catch { }
        }

        ///// <summary>
        ///// Before rendering data.
        ///// </summary>
        ///// <param name="e"></param>
        //protected override void OnPreRender(EventArgs e)
        //{
        //    // If not in design mode
        //    if (!this.DesignMode)
        //    {
        //        // Get the script manager for the current page.
        //        _scriptManager = ScriptManager.GetCurrent(Page);

        //        // If no script manager has been add to the
        //        // current page the thrown an execption.
        //        if (_scriptManager == null)
        //            throw new HttpException("A ScriptManager control is required in the page.");

        //        // Register the script controls for this control.
        //        _scriptManager.RegisterScriptControl(this);
        //    }

        //    base.OnPreRender(e);
        //}

        ///// <summary>
        ///// When the control starts to render data.
        ///// </summary>
        ///// <param name="writer">Write the html text.</param>
        //protected override void Render(HtmlTextWriter writer)
        //{
        //    // If not in design mode
        //    if (!this.DesignMode)
        //    {
        //        // Register the script descriptors for this control.
        //        _scriptManager.RegisterScriptDescriptors(this);
        //    }
        //    base.Render(writer);
        //}
        #endregion

        #region Protected Script Methods
        /// <summary>
        /// Get the collection of script descriptors.
        /// </summary>
        /// <returns>The collection of script descriptors</returns>
        protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("Nequeo.Web.UI.ScriptControl.FileUploadControl", this.ClientID);

            // If no target control has been specified.
            if (String.IsNullOrEmpty(_targetControlID))
                descriptor.AddProperty("targetControlID", this.ClientID + "_Upload");
            else
                descriptor.AddProperty("targetControlID", this.TargetControlID);

            descriptor.AddProperty("urlAction", this.UrlAction);
            descriptor.AddProperty("cssClass", this.CssClass);

            if(!String.IsNullOrEmpty(this.ResponseControlID))
                descriptor.AddProperty("responseControlID", this.ResponseControlID);

            if (!String.IsNullOrEmpty(this.ErrorControlID))
                descriptor.AddProperty("errorControlID", this.ErrorControlID);

            if (!String.IsNullOrEmpty(this.FileExtensionFilter))
                descriptor.AddProperty("fileExtensionFilter", this.FileExtensionFilter);

            ScriptDescriptor[] descriptors = new ScriptDescriptor[] { descriptor };

            // Return the collection of descriptors.
            return descriptors;
        }

        /// <summary>
        /// Get the collection of script references.
        /// </summary>
        /// <returns>The collection of script references.</returns>
        protected override IEnumerable<ScriptReference> GetScriptReferences()
        {
            ScriptReference[] references = new ScriptReference[]
            {
                new ScriptReference() { 
                    Assembly = this.GetType().Assembly.FullName,
                    Name = "Nequeo.Web.UI.ScriptControl.Script.jquery.ajax.upload.js" },
                new ScriptReference() { 
                    Assembly = this.GetType().Assembly.FullName, 
                    Name = "Nequeo.Web.UI.ScriptControl.Script.FileUpload.js" 
                    /* Use following when client-side script isn't embedded in custom control:
                    Path = this.ResolveClientUrl("~/FileUpload.js")*/ }
            };

            // Return the collection of references.
            return references;
        }
        #endregion
    }
}
