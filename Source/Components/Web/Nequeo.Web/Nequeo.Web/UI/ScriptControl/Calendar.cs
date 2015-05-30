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

using Nequeo.Web.Common;
using Nequeo.ComponentModel;

//Client-side script file embedded in assembly
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Script.BaseScripts.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Script.Common.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Script.DateTime.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Script.Threading.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Script.Timer.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Script.Animations.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Script.AnimationBehavior.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Script.PopupBehavior.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Script.CalendarBehavior.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Style.Calendar.css", "text/css", PerformSubstitution = true)]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Image.arrow-left.gif", "image/gif")]
[assembly: System.Web.UI.WebResource("Nequeo.Web.UI.ScriptControl.Image.arrow-right.gif", "image/gif")]

namespace Nequeo.Web.UI.ScriptControl
{
    /// <summary>
    /// Calendar web service script control.
    /// </summary>
    [ToolboxBitmap(typeof(Calendar))]
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:Calendar runat=server></{0}:Calendar>")]
    [Description("Nequeo Web Calendar Control")]
    [DisplayName("Calendar Control")]
    [TargetControlType(typeof(TextBox))]
    [Designer(typeof(Nequeo.Web.UI.ScriptControl.Design.CalendarDesigner))]
    public class Calendar : ExtenderControl
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Calendar() { }

        #endregion

        #region Private Fields
        private string _cssClass = "ajaxNequeoCalendar";
        private string _format = "d";
        private bool _enabledOnClient = true;
        private bool _animated = true;
        private FirstDayOfWeek _firstDayOfWeek = FirstDayOfWeek.Default;

        private string _popupButtonID = string.Empty;
        private CalendarPosition _popupPosition = CalendarPosition.BottomLeft;
        private DateTime? _selectedDate = null;

        private string _onClientDateSelectionChanged = string.Empty;

        //private ScriptManager _scriptManager = null;
        //private System.Web.UI.Control _targetControl = null;
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
        /// Gets sets, the style class.
        /// </summary>
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Appearance")]
        [Description("The style class.")]
        [CssClassProperty()]
        public string CssClass
        {
            get { return _cssClass; }
            set { _cssClass = value; }
        }

        /// <summary>
        /// Gets sets, the date format.
        /// </summary>
        [DefaultValue("d")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Appearance")]
        [Description("The date format.")]
        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }

        /// <summary>
        /// Gets sets, is the control enabled on the client.
        /// </summary>
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Behavior")]
        [Description("Is the control enabled on the client.")]
        public bool EnabledOnClient
        {
            get { return _enabledOnClient; }
            set { _enabledOnClient = value; }
        }

        /// <summary>
        /// Gets sets, is the calendar animated.
        /// </summary>
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Behavior")]
        [Description("Is the calendar animated.")]
        public bool Animated
        {
            get { return _animated; }
            set { _animated = value; }
        }

        /// <summary>
        /// Gets sets, the first dat of the week displayed.
        /// </summary>
        [DefaultValue(FirstDayOfWeek.Default)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Appearance")]
        [Description("The first dat of the week displayed.")]
        public FirstDayOfWeek FirstDayOfWeek
        {
            get { return _firstDayOfWeek; }
            set { _firstDayOfWeek = value; }
        }

        /// <summary>
        /// Gets sets, the ID of the button control that the extender is associated with.
        /// </summary>
        [IDReferenceProperty]
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The ID of the control that the extender is associated with.")]
        [Editor(typeof(AllWebControlsEditor), typeof(UITypeEditor))]
        public string PopupButtonID
        {
            get { return _popupButtonID; }
            set { _popupButtonID = value; }
        }

        /// <summary>
        /// Gets sets, indicates where you want the calendar displayed, bottom or top of the textbox.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Behavior")]
        [DefaultValue(CalendarPosition.BottomLeft)]
        [Description("Indicates where you want the calendar displayed, bottom or top of the textbox.")]
        public CalendarPosition PopupPosition
        {
            get { return _popupPosition; }
            set { _popupPosition = value; }
        }

        /// <summary>
        /// Gets sets, the initial date to display.
        /// </summary>
        [DefaultValue(null)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Appearance")]
        [Description("The initial date to display.")]
        public DateTime? SelectedDate
        {
            get { return _selectedDate; }
            set { _selectedDate = value; }
        }

        /// <summary>
        /// Gets sets, the event handler when the date changes.
        /// </summary>
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Appearance")]
        [Description("The event handler when the date changes.")]
        public string OnClientDateSelectionChanged
        {
            get { return _onClientDateSelectionChanged; }
            set { _onClientDateSelectionChanged = value; }
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
                new string[] { "Nequeo.Web.UI.ScriptControl.Style.Calendar.css" });
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

        //        // Register the extender with the scipt manager.
        //        System.Web.UI.Control targetControl = Page.FindControl(_targetControlID);

        //        if (targetControl != null)
        //            _scriptManager.RegisterExtenderControl(this, targetControl);
        //        else
        //          _scriptManager.RegisterExtenderControl(this, _targetControl);
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
        /// <param name="targetControl">The ID of the control that the extender is associated with.</param>
        /// <returns>The collection of script descriptors</returns>
        protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors(System.Web.UI.Control targetControl)
        {
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("Nequeo.Web.UI.ScriptControl.CalendarBehavior", targetControl.ClientID);
            descriptor.AddProperty("cssClass", this.CssClass);
            descriptor.AddProperty("format", this.Format);
            descriptor.AddProperty("enabled", this.EnabledOnClient);
            descriptor.AddProperty("animated", this.Animated);
            descriptor.AddProperty("firstDayOfWeek", this.FirstDayOfWeek);
            descriptor.AddProperty("targetControlID", this.TargetControlID);
            descriptor.AddProperty("selectedDate", this.SelectedDate);
            descriptor.AddProperty("popupPosition", this.PopupPosition);

            // Add the "PopupButtonID" control reference.
            if (!String.IsNullOrEmpty(_popupButtonID))
            {
                System.Web.UI.Control control = Page.FindControl(this.PopupButtonID);
                if(control != null)
                    descriptor.AddElementProperty("button", (string)control.ClientID);
            }

            // Add the "OnClientDateSelectionChanged" event handler.
            if (!String.IsNullOrEmpty(_onClientDateSelectionChanged))
                descriptor.AddEvent("dateSelectionChanged", this.OnClientDateSelectionChanged);
            
            // Add the descripter to the collection.
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
                    Name = "Nequeo.Web.UI.ScriptControl.Script.BaseScripts.js" },
                new ScriptReference() { 
                    Assembly = this.GetType().Assembly.FullName,
                    Name = "Nequeor.Web.UI.ScriptControl.Script.Common.js" },
                new ScriptReference() { 
                    Assembly = this.GetType().Assembly.FullName,
                    Name = "Nequeo.Web.UI.ScriptControl.Script.DateTime.js" },
                new ScriptReference() { 
                    Assembly = this.GetType().Assembly.FullName,
                    Name = "Nequeo.Web.UI.ScriptControl.Script.Threading.js" },
                new ScriptReference() { 
                    Assembly = this.GetType().Assembly.FullName,
                    Name = "Nequeo.Web.UI.ScriptControl.Script.Timer.js" },
                new ScriptReference() { 
                    Assembly = this.GetType().Assembly.FullName,
                    Name = "Nequeo.Web.UI.ScriptControl.Script.Animations.js" },
                new ScriptReference() { 
                    Assembly = this.GetType().Assembly.FullName,
                    Name = "Nequeo.Web.UI.ScriptControl.Script.AnimationBehavior.js" },
                new ScriptReference() { 
                    Assembly = this.GetType().Assembly.FullName,
                    Name = "Nequeo.Web.UI.ScriptControl.Script.PopupBehavior.js" },
                new ScriptReference() { 
                    Assembly = this.GetType().Assembly.FullName, 
                    Name = "Nequeo.Web.UI.ScriptControl.Script.CalendarBehavior.js" 
                    /* Use following when client-side script isn't embedded in custom control:
                    Path = this.ResolveClientUrl("~/CalendarBehavior.js")*/ }
            };

            // Return the collection of references.
            return references;
        }
        #endregion
    }
}
