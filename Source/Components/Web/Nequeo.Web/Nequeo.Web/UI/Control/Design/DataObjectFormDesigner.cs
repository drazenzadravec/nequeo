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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;

namespace Nequeo.Web.UI.Control.Design
{
    /// <summary>
    /// Data object form designer.
    /// </summary>
    [PermissionSet(System.Security.Permissions.SecurityAction.InheritanceDemand, Name = "FullTrust")]
    [PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public class DataObjectFormDesigner : ControlDesigner
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DataObjectFormDesigner() { }

        private DesignerActionListCollection _actionLists = null;
        private TemplateGroupCollection col = null;

        /// <summary>
        /// Initialise the designer.
        /// </summary>
        /// <param name="component">The current component.</param>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            // Turn on template editing.
            SetViewFlags(ViewFlags.TemplateEditing, true);
        }

        /// <summary>
        /// Gets a collection of template groups, each containing one or more template
        /// definitions.
        /// </summary>
        public override TemplateGroupCollection TemplateGroups
        {
            get
            {
                if (col == null)
                {
                    // Get the base collection
                    col = base.TemplateGroups;

                    // Create variables
                    TemplateGroup tempGroup;
                    TemplateDefinition tempDef;
                    DataObjectForm ctl;

                    // Get reference to the component as DataObjectForm
                    ctl = (DataObjectForm)Component;

                    // Create a TemplateGroup
                    tempGroup = new TemplateGroup("Templates");

                    // Create a TemplateDefinition
                    tempDef = new TemplateDefinition(this, "Item Template",
                        ctl, "ItemTemplate", true);

                    // Add the TemplateDefinition to the TemplateGroup
                    tempGroup.AddTemplateDefinition(tempDef);

                    // Add the TemplateGroup to the TemplateGroupCollection
                    col.Add(tempGroup);
                }
                return col;
            }
        }

        /// <summary>
        /// Get the design time html.
        /// </summary>
        /// <returns>Html</returns>
        public override string GetDesignTimeHtml()
        {
            return CreatePlaceHolderDesignTimeHtml("Click here and use " +
                "the task menu to edit the control.");
        }

        /// <summary>
        /// Do not allow direct resizing of the control unless in template mode.
        /// </summary>
        public override bool AllowResize
        {
            get
            {
                if (this.InTemplateMode)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Return a custom ActionList collection
        /// </summary>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists == null)
                {
                    _actionLists = new DesignerActionListCollection();
                    _actionLists.AddRange(base.ActionLists);

                    // Add a custom DesignerActionList
                    _actionLists.Add(new ActionList(this));
                }
                return _actionLists;
            }
        }

        /// <summary>
        /// Custom action collection.
        /// </summary>
        public class ActionList : DesignerActionList
        {
            private DataObjectFormDesigner _parent;
            private DesignerActionItemCollection _items;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="parent">The parent designer.</param>
            public ActionList(DataObjectFormDesigner parent)
                : base(parent.Component)
            {
                _parent = parent;
            }

            /// <summary>
            /// Create the ActionItem collection and add one command
            /// </summary>
            /// <returns>Action designer collection</returns>
            public override DesignerActionItemCollection GetSortedActionItems()
            {
                if (_items == null)
                {
                    _items = new DesignerActionItemCollection();
                    _items.Add(new DesignerActionHeaderItem("Behavior"));
                    _items.Add(new DesignerActionMethodItem(this, "ToggleAllowDelete", "Toggle Allow Delete", "Behavior", true));
                    _items.Add(new DesignerActionMethodItem(this, "ToggleAllowUpdate", "Toggle Allow Update", "Behavior", true));
                    _items.Add(new DesignerActionMethodItem(this, "ToggleAllowInsert", "Toggle Allow Insert", "Behavior", true));
                    _items.Add(new DesignerActionMethodItem(this, "ToggleIncludeTypeExpressValidation", "Toggle Include Type Express Validation", "Behavior", true));
                    _items.Add(new DesignerActionMethodItem(this, "ToggleIncludeRequiredValidation", "Toggle Include Required Validation", "Behavior", true));
                }
                return _items;
            }

            /// <summary>
            /// Toggle the allow delete property.
            /// </summary>
            private void ToggleAllowDelete()
            {
                // Get a reference to the parent designer's associated control
                DataObjectForm ctl = (DataObjectForm)_parent.Component;

                // Get a reference to the control's LargeText property
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(ctl)["AllowDelete"];

                // Get the current value of the property
                bool v = (bool)propDesc.GetValue(ctl);

                // Toggle the property value
                propDesc.SetValue(ctl, !v);
            }

            /// <summary>
            /// Toggle the allow update property.
            /// </summary>
            private void ToggleAllowUpdate()
            {
                // Get a reference to the parent designer's associated control
                DataObjectForm ctl = (DataObjectForm)_parent.Component;

                // Get a reference to the control's LargeText property
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(ctl)["AllowUpdate"];

                // Get the current value of the property
                bool v = (bool)propDesc.GetValue(ctl);

                // Toggle the property value
                propDesc.SetValue(ctl, !v);
            }

            /// <summary>
            /// Toggle the allow insert property.
            /// </summary>
            private void ToggleAllowInsert()
            {
                // Get a reference to the parent designer's associated control
                DataObjectForm ctl = (DataObjectForm)_parent.Component;

                // Get a reference to the control's LargeText property
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(ctl)["AllowInsert"];

                // Get the current value of the property
                bool v = (bool)propDesc.GetValue(ctl);

                // Toggle the property value
                propDesc.SetValue(ctl, !v);
            }

            /// <summary>
            /// Toggle the allow insert property.
            /// </summary>
            private void ToggleIncludeTypeExpressValidation()
            {
                // Get a reference to the parent designer's associated control
                DataObjectForm ctl = (DataObjectForm)_parent.Component;

                // Get a reference to the control's LargeText property
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(ctl)["IncludeTypeExpressValidation"];

                // Get the current value of the property
                bool v = (bool)propDesc.GetValue(ctl);

                // Toggle the property value
                propDesc.SetValue(ctl, !v);
            }

            /// <summary>
            /// Toggle the allow insert property.
            /// </summary>
            private void ToggleIncludeRequiredValidation()
            {
                // Get a reference to the parent designer's associated control
                DataObjectForm ctl = (DataObjectForm)_parent.Component;

                // Get a reference to the control's LargeText property
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(ctl)["IncludeRequiredValidation"];

                // Get the current value of the property
                bool v = (bool)propDesc.GetValue(ctl);

                // Toggle the property value
                propDesc.SetValue(ctl, !v);
            }
        }
    }
}
