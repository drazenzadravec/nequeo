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
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Script.jquery.dataTables.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Script.jquery.json-2.2.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Script.DataTable.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Image.back_disabled.jpg", "image/jpg")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Image.back_enabled.jpg", "image/jpg")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Image.forward_disabled.jpg", "image/jpg")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Image.forward_enabled.jpg", "image/jpg")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Image.sort_asc.png", "image/png")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Image.sort_desc.png", "image/png")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Image.sort_both.png", "image/png")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Image.sort_asc_disabled.png", "image/png")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Image.sort_desc_disabled.png", "image/png")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Style.demo_page.css", "text/css", PerformSubstitution = true)]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Style.demo_table.css", "text/css", PerformSubstitution = true)]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Style.demo_table_jui.css", "text/css", PerformSubstitution = true)]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Style.jquery-ui-smooth.css", "text/css", PerformSubstitution = true)]

namespace Nequeo.Web.UI.ScriptControl
{
    /// <summary>
    /// Data table web service script control.
    /// </summary>
    [ToolboxBitmap(typeof(DataTableService))]
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:DataTableService runat=server></{0}:DataTableService>")]
    [Description("Nequeo Web Data Table Service")]
    [DisplayName("Data Table Service")]
    [Designer(typeof(DataTableDesigner))]
    public class DataTableService : System.Web.UI.ScriptControl, INamingContainer
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataTableService() { }

        #endregion

        #region Private Fields
        private string _targetControlID = string.Empty;
        private string _connectionStringExtensionName = string.Empty;
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
        /// Gets sets, the configuration connection string extension name, used to connect to the database.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The configuration connection string extension name, used to connect to the database.")]
        public string ConnectionStringExtensionName
        {
            get { return _connectionStringExtensionName; }
            set { _connectionStringExtensionName = value; }
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
                new string[] { 
                    "Nequeo.Web.UI.ScriptControl.Style.demo_page.css",
                    "Nequeo.Web.UI.ScriptControl.Style.demo_table.css",
                    "Nequeo.Web.UI.ScriptControl.Style.demo_table_jui.css",
                    "Nequeo.Web.UI.ScriptControl.Style.jquery-ui-smooth.css"
                });
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
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("Nequeo.Web.UI.ScriptControl.DataTableServiceControl", this.ClientID);
            descriptor.AddProperty("targetControlID", this.TargetControlID);
            descriptor.AddProperty("connectionStringExtensionName", this.ConnectionStringExtensionName);
            descriptor.AddProperty("cssClass", this.CssClass);
            descriptor.AddProperty("urlAction", this.UrlAction);

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
                    Name = "Nequeo.Web.UI.ScriptControl.Script.jquery.json-2.2.js" },
                new ScriptReference() { 
                    Assembly = this.GetType().Assembly.FullName,
                    Name = "Nequeo.Web.UI.ScriptControl.Script.jquery.dataTables.js" },
                new ScriptReference() { 
                    Assembly = this.GetType().Assembly.FullName, 
                    Name = "Nequeo.Web.UI.ScriptControl.Script.DataTable.js" 
                    /* Use following when client-side script isn't embedded in custom control:
                    Path = this.ResolveClientUrl("~/DataTable.js")*/ }
            };

            // Return the collection of references.
            return references;
        }
        #endregion
    }
}
