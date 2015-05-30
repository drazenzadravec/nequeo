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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel;
using System.Windows.Forms.Design;
using System.Xml.Serialization;

using Nequeo.ComponentModel.Design;

namespace Nequeo.ComponentModel
{
    /// <summary>
    /// Connection type modal form designer
    /// </summary>
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public class ConnectionTypeModalEditor : System.Drawing.Design.UITypeEditor
    {
        /// <summary>
        /// Defualt constructor
        /// </summary>
        public ConnectionTypeModalEditor()
        {
        }

        private IWindowsFormsEditorService edSvc = null;

        /// <summary>
        /// Get the editoring type style.
        /// </summary>
        /// <param name="context">The current type context.</param>
        /// <returns>The type editor display style.</returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        /// Apply value type settings.
        /// </summary>
        /// <param name="context">The current type context.</param>
        /// <param name="provider">The current service provider.</param>
        /// <param name="value">The value sent by the client</param>
        /// <returns>The value returned by the service.</returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            ConnectionTypeModel newValue = null;

            if (context != null
                && context.Instance != null
                && provider != null)
            {
                // Get the windows service editor
                edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null)
                {
                    // Create a new desginer type.
                    ConnectionTypeFormDesigner design = new ConnectionTypeFormDesigner();

                    // If the value sent is not null
                    if (value != null)
                    {
                        // If the value sent is of type connection type model
                        // else throw an exception.
                        if (value is ConnectionTypeModel)
                            design.ConnectionTypeModel = (ConnectionTypeModel)value;
                        else
                            throw new TypeLoadException("Value must be of type 'Nequeo.ComponentModel.ConnectionTypeModel'");
                    }

                    // Show the designer and assign the
                    // set values.
                    edSvc.ShowDialog(design);
                    newValue = design.ConnectionTypeModel;
                }
            }
            return newValue;
        }
    }

    /// <summary>
    /// Connection type modal drop down designer
    /// </summary>
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public class ConnectionTypeDropDownEditor : System.Drawing.Design.UITypeEditor
    {
        /// <summary>
        /// Defualt constructor
        /// </summary>
        public ConnectionTypeDropDownEditor()
        {
        }

        private IWindowsFormsEditorService edSvc = null;

        /// <summary>
        /// Get the editoring type style.
        /// </summary>
        /// <param name="context">The current type context.</param>
        /// <returns>The type editor display style.</returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        /// <summary>
        /// Apply value type settings.
        /// </summary>
        /// <param name="context">The current type context.</param>
        /// <param name="provider">The current service provider.</param>
        /// <param name="value">The value sent by the client</param>
        /// <returns>The value returned by the service.</returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            ConnectionTypeModel newValue = null;

            if (context != null
                && context.Instance != null
                && provider != null)
            {
                // Get the windows service editor
                edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null)
                {
                    // Create a new desginer type.
                    ConnectionTypeControlDesigner design = new ConnectionTypeControlDesigner();

                    // If the value sent is not null
                    if (value != null)
                    {
                        // If the value sent is of type connection type model
                        // else throw an exception.
                        if (value is ConnectionTypeModel)
                            design.ConnectionTypeModel = (ConnectionTypeModel)value;
                        else
                            throw new TypeLoadException("Value must be of type 'Nequeo.ComponentModel.ConnectionTypeModel'");
                    }

                    // Show the designer and assign the
                    // set values.
                    edSvc.DropDownControl(design);
                    newValue = design.ConnectionTypeModel;
                }
            }
            return newValue;
        }

        /// <summary>
        /// Gets, can the drop down control be resied.
        /// </summary>
        public override bool IsDropDownResizable
        {
            get { return false; }
        }
    }
}
