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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;

using Nequeo.Web.UI.ScriptControl.Design;
using Nequeo.ComponentModel;

//Client-side script file embedded in assembly
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Script.MenuListService.js", "text/javascript")]

namespace Nequeo.Web.UI.ScriptControl
{
    /// <summary>
    /// Menu list web service script control.
    /// </summary>
    [ToolboxBitmap(typeof(MenuListService))]
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:MenuListService runat=server></{0}:MenuListService>")]
    [Description("Nequeo Web Menu List Service")]
    [DisplayName("Menu List Service")]
    [Designer(typeof(MenuListServiceDesigner))]
    public class MenuListService : System.Web.UI.ScriptControl
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MenuListService() { }

        #endregion

        #region Private Fields

        private string _menuTitle = string.Empty;
        private string _menuTitleCssClass = string.Empty;
        private string _menuListCssClass = string.Empty;

        private string _servicePath = string.Empty;
        private string _serviceMethod = string.Empty;

        private string _listTargetControlID = string.Empty;
        private string _titleTargetControlID = string.Empty;

        //private ScriptManager _scriptManager = null;
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
        /// Gets sets, the menu title.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("The menu title.")]
        public string MenuTitle
        {
            get { return _menuTitle; }
            set { _menuTitle = value; }
        }

        /// <summary>
        /// Gets sets, the menu title level css class.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("The menu title level css class.")]
        [CssClassProperty]
        public string MenuTitleCssClass
        {
            get { return _menuTitleCssClass; }
            set { _menuTitleCssClass = value; }
        }

        /// <summary>
        /// Gets sets, the menu list css class.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("The menu list css class.")]
        [CssClassProperty]
        public string MenuListCssClass
        {
            get { return _menuListCssClass; }
            set { _menuListCssClass = value; }
        }

        /// <summary>
        /// Gets sets, the ID of the control that the list items are displayed within.
        /// </summary>
        [IDReferenceProperty]
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The ID of the control that the list items are displayed within.")]
        [Editor(typeof(AllWebControlsEditor), typeof(UITypeEditor))]
        public string ListTargetControlID
        {
            get { return _listTargetControlID; }
            set { _listTargetControlID = value; }
        }

        /// <summary>
        /// Gets sets, the ID of the control that the title items are displayed within.
        /// </summary>
        [IDReferenceProperty]
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The ID of the control that the title items are displayed within.")]
        [Editor(typeof(AllWebControlsEditor), typeof(UITypeEditor))]
        public string TitleTargetControlID
        {
            get { return _titleTargetControlID; }
            set { _titleTargetControlID = value; }
        }

        /// <summary>
        /// Gets sets, the service path.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Behavior")]
        [Description("The service path.")]
        public string ServicePath
        {
            get { return _servicePath; }
            set { _servicePath = value; }
        }

        /// <summary>
        /// Gets sets, the service method name.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Behavior")]
        [Description("The service method name.")]
        public string ServiceMethod
        {
            get { return _serviceMethod; }
            set { _serviceMethod = value; }
        }
        #endregion

        #region Protected Render Methods
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
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("Nequeo.Web.UI.ScriptControl.MenuListClientControl", this.ClientID);
            descriptor.AddProperty("MenuTitle", this.MenuTitle);
            descriptor.AddProperty("MenuTitleCssClass", this.MenuTitleCssClass);
            descriptor.AddProperty("MenuListCssClass", this.MenuListCssClass);
            descriptor.AddProperty("ServicePath", this.ServicePath);
            descriptor.AddProperty("ServiceMethod", this.ServiceMethod);
            descriptor.AddProperty("ListTargetControlID", this.ListTargetControlID);
            descriptor.AddProperty("TitleTargetControlID", this.TitleTargetControlID);

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
                    Name = "Nequeo.Web.UI.ScriptControl.Script.MenuListService.js" 
                    /* Use following when client-side script isn't embedded in custom control:
                    Path = this.ResolveClientUrl("~/MenuListService.js")*/ }
            };

            // Return the collection of references.
            return references;
        }
        #endregion
    }
}
