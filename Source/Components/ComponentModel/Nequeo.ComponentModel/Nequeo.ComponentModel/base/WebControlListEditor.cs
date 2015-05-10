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

namespace Nequeo.ComponentModel
{
    /// <summary>
    /// Data object editor.
    /// </summary>
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public abstract class WebControlEditor : System.Drawing.Design.UITypeEditor, IDisposable
    {
        /// <summary>
        /// Defualt constructor
        /// </summary>
        /// <param name="typeOfControlToDisplay">The type of control to display in the list.</param>
        public WebControlEditor(Type typeOfControlToDisplay)
        {
            typeShow = typeOfControlToDisplay;
        }

        private IWindowsFormsEditorService edSvc = null;
        private System.Windows.Forms.ListBox listBox;
        private Type typeShow;

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
            object newValue = null;

            if (context != null
                && context.Instance != null
                && provider != null)
            {
                // Get the windows service editor
                edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null)
                {
                    // Create a new list box control.
                    listBox = new System.Windows.Forms.ListBox();
                    listBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                    listBox.SelectedIndexChanged += new EventHandler(listBox_SelectedIndexChanged);

                    // For each web control on the page.
                    foreach (System.Web.UI.Control ctrl in ((System.Web.UI.Control)context.Instance).Page.Controls)
                    {
                        // If the web control is of the type selected add
                        // the id if it exists to the collection.
                        if (ctrl.GetType().IsSubclassOf(typeShow) || ctrl.GetType().FullName == typeShow.FullName)
                            if(!String.IsNullOrEmpty(ctrl.ID))
                                listBox.Items.Add(ctrl.ID);
                    }

                    if (listBox.Items.Count > 0)
                        edSvc.DropDownControl(listBox);
                    else
                        return value;

                    // If nothing has been selected.
                    if (listBox.SelectedIndex == -1)
                        return value;

                    // Return the selected item.
                    return listBox.SelectedItem;
                }
            }
            return newValue;
        }

        /// <summary>
        /// Close the dropdowncontrol when the user has selected a value
        /// </summary>
        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (edSvc != null)
            {
                // Close the drop down when the
                // selection has changed.
                edSvc.CloseDropDown();
            }
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if(listBox != null)
                        listBox.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                listBox = null;

                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~WebControlEditor()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// Editor for selecting all Asp.Net controls
    /// </summary>
    public class AllWebControlsEditor : WebControlEditor
    {
        /// <summary>
        /// Invoke base constructor
        /// </summary>
        public AllWebControlsEditor() : base(typeof(System.Web.UI.Control)) { }
    }

    /// <summary>
    /// Editor for selecting textbox Asp.Net controls
    /// </summary>
    public class TextBoxWebControlsEditor : WebControlEditor
    {
        /// <summary>
        /// Invoke base constructor
        /// </summary>
        public TextBoxWebControlsEditor() : base(typeof(System.Web.UI.WebControls.TextBox)) { }
    }
}
