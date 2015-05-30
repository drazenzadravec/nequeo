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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Security.Permissions;
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
using System.Reflection;
using System.Threading;

namespace Nequeo.Web.UI.ScriptControl.Design
{
    /// <summary>
    /// Item list service designer.
    /// </summary>
    [PermissionSet(System.Security.Permissions.SecurityAction.InheritanceDemand, Name = "FullTrust")]
    [PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public class ItemListServiceDesigner : ControlDesigner
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ItemListServiceDesigner() { }

        private DesignerActionListCollection _actionLists = null;

        /// <summary>
        /// Initialise the designer.
        /// </summary>
        /// <param name="component">The current component.</param>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            // Turn on template editing.
            //SetViewFlags(ViewFlags.TemplateEditing, true);
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
        /// Do not allow direct resizing of the control
        /// </summary>
        public override bool AllowResize
        {
            get { return false; }
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
            private ItemListServiceDesigner _parent;
            private DesignerActionItemCollection _items;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="parent">The parent designer.</param>
            public ActionList(ItemListServiceDesigner parent)
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
                    _items.Add(new DesignerActionHeaderItem("Appearance"));
                    _items.Add(new DesignerActionPropertyItem(
                        "ItemTitle", "Item Title", "Appearance",
                        "Sets the Item Title for the service."));
                    _items.Add(new DesignerActionHeaderItem("Behavior"));
                    _items.Add(new DesignerActionPropertyItem(
                        "TargetControlID", "Target Control ID", "Behavior",
                        "The ID of the control that the extender is associated with."));
                    _items.Add(new DesignerActionPropertyItem(
                        "TitleTargetControlID", "Title Target Control ID", "Behavior",
                        "The ID of the control that the title items are displayed within."));
                    _items.Add(new DesignerActionPropertyItem(
                        "ListTargetControlID", "List Target Control ID", "Behavior",
                        "The ID of the control that the list items are displayed within."));
                    _items.Add(new DesignerActionPropertyItem(
                        "ServicePath", "Service Path", "Behavior",
                        "Sets the web Service Path."));
                    _items.Add(new DesignerActionPropertyItem(
                        "ServiceMethod", "Service Method", "Behavior",
                        "Sets the web Service Method."));

                }
                return _items;
            }

            /// <summary>
            /// Gets sets, the Target Control ID.
            /// </summary>
            public String TargetControlID
            {
                get
                {
                    // Get a reference to the parent designer's associated control
                    ItemListService ctl = (ItemListService)_parent.Component;
                    return ctl.TargetControlID;
                }
                set
                {
                    // Get a reference to the parent designer's associated control
                    ItemListService ctl = (ItemListService)_parent.Component;

                    // Get a reference to the control's RenderUrl property
                    PropertyDescriptor propDesc = TypeDescriptor.GetProperties(ctl)["TargetControlID"];

                    // Toggle the property value
                    propDesc.SetValue(ctl, value);
                }
            }

            /// <summary>
            /// Gets sets, the Title Target Control ID.
            /// </summary>
            public String TitleTargetControlID
            {
                get
                {
                    // Get a reference to the parent designer's associated control
                    ItemListService ctl = (ItemListService)_parent.Component;
                    return ctl.TitleTargetControlID;
                }
                set
                {
                    // Get a reference to the parent designer's associated control
                    ItemListService ctl = (ItemListService)_parent.Component;

                    // Get a reference to the control's RenderUrl property
                    PropertyDescriptor propDesc = TypeDescriptor.GetProperties(ctl)["TitleTargetControlID"];

                    // Toggle the property value
                    propDesc.SetValue(ctl, value);
                }
            }

            /// <summary>
            /// Gets sets, the List Target Contro lID.
            /// </summary>
            public String ListTargetControlID
            {
                get
                {
                    // Get a reference to the parent designer's associated control
                    ItemListService ctl = (ItemListService)_parent.Component;
                    return ctl.ListTargetControlID;
                }
                set
                {
                    // Get a reference to the parent designer's associated control
                    ItemListService ctl = (ItemListService)_parent.Component;

                    // Get a reference to the control's RenderUrl property
                    PropertyDescriptor propDesc = TypeDescriptor.GetProperties(ctl)["ListTargetControlID"];

                    // Toggle the property value
                    propDesc.SetValue(ctl, value);
                }
            }

            /// <summary>
            /// Gets sets, the item title.
            /// </summary>
            public String ItemTitle
            {
                get
                {
                    // Get a reference to the parent designer's associated control
                    ItemListService ctl = (ItemListService)_parent.Component;
                    return ctl.ItemTitle;
                }
                set
                {
                    // Get a reference to the parent designer's associated control
                    ItemListService ctl = (ItemListService)_parent.Component;

                    // Get a reference to the control's RenderUrl property
                    PropertyDescriptor propDesc = TypeDescriptor.GetProperties(ctl)["ItemTitle"];

                    // Toggle the property value
                    propDesc.SetValue(ctl, value);
                }
            }

            /// <summary>
            /// Gets sets, the Service Path.
            /// </summary>
            public String ServicePath
            {
                get
                {
                    // Get a reference to the parent designer's associated control
                    ItemListService ctl = (ItemListService)_parent.Component;
                    return ctl.ServicePath;
                }
                set
                {
                    // Get a reference to the parent designer's associated control
                    ItemListService ctl = (ItemListService)_parent.Component;

                    // Get a reference to the control's RenderUrl property
                    PropertyDescriptor propDesc = TypeDescriptor.GetProperties(ctl)["ServicePath"];

                    // Toggle the property value
                    propDesc.SetValue(ctl, value);
                }
            }

            /// <summary>
            /// Gets sets, the Service Method.
            /// </summary>
            public String ServiceMethod
            {
                get
                {
                    // Get a reference to the parent designer's associated control
                    ItemListService ctl = (ItemListService)_parent.Component;
                    return ctl.ServiceMethod;
                }
                set
                {
                    // Get a reference to the parent designer's associated control
                    ItemListService ctl = (ItemListService)_parent.Component;

                    // Get a reference to the control's RenderUrl property
                    PropertyDescriptor propDesc = TypeDescriptor.GetProperties(ctl)["ServiceMethod"];

                    // Toggle the property value
                    propDesc.SetValue(ctl, value);
                }
            }
        }
    }
}
