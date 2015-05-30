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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Drawing.Design;
using System.Web.Compilation;
using System.Collections.ObjectModel;
using System.Web.Script.Serialization;

using Nequeo.Web.Common;

namespace Nequeo.Web
{
    /// <summary>
    /// Ajax implementation manger, contains members to access ajax functionality
    /// </summary>
    public sealed class AjaxManager
    {
        private static readonly Dictionary<Type, List<ResourceEntry>> _cache = new Dictionary<Type, List<ResourceEntry>>();
        private static readonly Dictionary<Type, IList<string>> _cssCache = new Dictionary<Type, IList<string>>();
        private static readonly object _sync = new object();

        /// <summary>
        /// Register the current control for post back ajax events
        /// </summary>
        /// <param name="scriptManagerID">The current script manager id to to register against</param>
        /// <param name="control">The control to register post back events</param>
        public static void RegisterPostBackControl(String scriptManagerID, System.Web.UI.Control control)
        {
            // Get the script manager and register the
            // control with the script manager.
            System.Web.UI.ScriptManager scriptManager = GetScriptManager(scriptManagerID, control.Page);
            if (scriptManager != null)
                scriptManager.RegisterPostBackControl(control);
        }

        /// <summary>
        /// Get the script manager that is contained within the current page.
        /// </summary>
        /// <param name="scriptManagerID">The script manager to find in the current page.</param>
        /// <param name="page">The page that contains the script manager.</param>
        /// <returns>The script manager reference else null.</returns>
        public static System.Web.UI.ScriptManager GetScriptManager(String scriptManagerID, System.Web.UI.Page page)
        {
            try
            {
                // Try find the script manager on the page.
                return ((System.Web.UI.ScriptManager)page.FindControl(scriptManagerID));
            }
            catch { return null; }
        }

        /// <summary>
        /// IsEnabled can be used to determine if AJAX has been enabled already as we
        /// only need one Script Manager per page.
        /// </summary>
        /// <returns>True if the script manager exists else false.</returns>
        public static bool IsEnabled()
        {
            if (HttpContext.Current.Items["System.Web.UI.ScriptManager"] == null)
                return false;
            else
                return (bool)HttpContext.Current.Items["System.Web.UI.ScriptManager"];
        }

        /// <summary>
        /// RegisterScriptManager must be used by developers to instruct 
        /// the framework that AJAX is required on the page.
        /// </summary>
        public static void RegisterScriptManager()
        {
            if (!IsEnabled())
                HttpContext.Current.Items.Add("System.Web.UI.ScriptManager", true);
        }

        /// <summary>
        /// Create a new script manager as the first control on the page form.
        /// </summary>
        /// <param name="page">The page to add the script manager to.</param>
        /// <returns>The script manager id.</returns>
        public static string AddScriptManager(System.Web.UI.Page page)
        {
            // Make sure the page reference exists.
            if(page == null) throw new ArgumentNullException("page");

            // Get the existing script manager.
            string scriptManagerID = "ScriptManager99";
            System.Web.UI.ScriptManager scriptManager = GetScriptManager(scriptManagerID, page);

            // If a script manager does not exist
            if (scriptManager == null)
            {
                // Create a new script manager.
                using (scriptManager = new ScriptManager() 
                    { 
                        ID = scriptManagerID, 
                        EnableScriptGlobalization = true, 
                        EnableScriptLocalization = true ,
                    }
                )
                {
                    // Add the script manager to the first
                    // control on the form.
                    page.Form.Controls.AddAt(0, scriptManager);

                    // Add an indicator for the current session
                    // that the script manager has been added.
                    if (HttpContext.Current.Items["System.Web.UI.ScriptManager"] == null)
                        HttpContext.Current.Items.Add("System.Web.UI.ScriptManager", true);

                    // Return the script manager id.
                    return scriptManager.ID;
                }
            }
            else
                // Return the script manager id.
                return scriptManager.ID;
        }

        /// <summary>
        /// Create a new script manager as the first control on the page form.
        /// </summary>
        /// <param name="page">The page to add the script manager to.</param>
        /// <param name="scripts">The scripts associated with the script manager to.</param>
        /// <param name="services">The services associated with the script manager to.</param>
        /// <returns>The script manager id.</returns>
        public static string AddScriptManager(System.Web.UI.Page page, List<ScriptReference> scripts, List<ServiceReference> services)
        {
            // Make sure the page reference exists.
            if (page == null) throw new ArgumentNullException("page");

            // Get the existing script manager.
            string scriptManagerID = "ScriptManager99";
            System.Web.UI.ScriptManager scriptManager = GetScriptManager(scriptManagerID, page);

            // If a script manager does not exist
            if (scriptManager == null)
            {
                // Create a new script manager.
                using (scriptManager = new ScriptManager()
                {
                    ID = scriptManagerID,
                    EnableScriptGlobalization = true,
                    EnableScriptLocalization = true,
                })
                {
                    // Add each script
                    if (scripts != null)
                        foreach (ScriptReference item in scripts)
                            scriptManager.Scripts.Add(item);

                    // Add each service
                    if (services != null)
                        foreach (ServiceReference item in services)
                            scriptManager.Services.Add(item);
                    
                    // Add the script manager to the first
                    // control on the form.
                    page.Form.Controls.AddAt(0, scriptManager);

                    // Add an indicator for the current session
                    // that the script manager has been added.
                    if (HttpContext.Current.Items["System.Web.UI.ScriptManager"] == null)
                        HttpContext.Current.Items.Add("System.Web.UI.ScriptManager", true);

                    // Return the script manager id.
                    return scriptManager.ID;
                }
            }
            else
                // Return the script manager id.
                return scriptManager.ID;
        }

        /// <summary>
        /// Remove the script manager from the page form control.
        /// </summary>
        /// <param name="page">The page to remove the script manager from.</param>
        public static void RemoveScriptManager(System.Web.UI.Page page)
        {
            // Make sure the page reference exists.
            if (page == null) throw new ArgumentNullException("page");

            // Get the existing script manager.
            string scriptManagerID = "ScriptManager99";

            // Has the script manager been created.
            if (IsEnabled())
            {
                // Find the script manager control on the page.
                // Remove the script manager from the page.
                Control scriptManager = page.FindControl(scriptManagerID);
                if (scriptManager != null)
                    page.Form.Controls.Remove(scriptManager);
            }
        }

        /// <summary>
        /// Wrap a control within a update panel control
        /// </summary>
        /// <param name="control">The control to wrap.</param>
        /// <param name="includeProgress">Include a progress control.</param>
        /// <param name="progressImageUrl">The progress control image Url.</param>
        /// <returns>The update panel control.</returns>
        public static Control WrapControlInUpdatePanel(Control control, bool includeProgress, string progressImageUrl)
        {
            // Make sure the page reference exists.
            if (control == null) throw new ArgumentNullException("control");

            // Create a new update panel control
            UpdatePanel updatePanel = new UpdatePanel();
            updatePanel.ID = control.ID + "_UpdatePanel";
            updatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional;

            // Assign the content template container.
            Control contentTemplateContainer = updatePanel.ContentTemplateContainer;

            // For each child control within the control
            for (int i = 0; i < control.Parent.Controls.Count; i++)
            {
                // If the control is the control
                if (control.Parent.Controls[i].ID == control.ID)
                {
                    // Add the control to the update panel.
                    control.Parent.Controls.AddAt(i, updatePanel);
                    contentTemplateContainer.Controls.Add(control);
                    break;
                }
            }

            // Add a progress control.
            if (includeProgress)
            {
                // Create a new image control
                System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
                image.ImageUrl = progressImageUrl;
                image.AlternateText = "ProgressBar";

                // Create a new update panel control add the
                // image control to the progress template.
                UpdateProgress updateProgress = new UpdateProgress();
                updateProgress.AssociatedUpdatePanelID = updatePanel.ID;
                updateProgress.ID = updatePanel.ID + "_ProgressBar";
                updateProgress.ProgressTemplate = new LiteralTemplate(image);

                // Add the update progress control to the template container.
                contentTemplateContainer.Controls.Add(updateProgress);
            }

            // Return the update panel control.
            return updatePanel;
        }

        /// <summary>
        /// Register's the Css references for this control
        /// </summary>
        /// <param name="control">The control to register the css resource for.</param>
        /// <param name="cssReferences">The collection of css reference objects.</param>
        public static void RegisterCssReferences(Control control, string[] cssReferences)
        {
            // Make sure the page reference exists.
            if (control == null) throw new ArgumentNullException("control");

            // Add the link to the page header instead of inside the body which is not xhtml compliant
            HtmlHead header = control.Page.Header;

            // For each css refrence.
            foreach (string styleSheet in AjaxManager.GetCssReferences(control, cssReferences))
            {
                if (header == null)
                {
                    // It would be nice to add the required header here, but it's too late in the page
                    // lifecycle to be modifying the Page.Controls collection - throw an informative
                    // exception instead and let the page author make the simple change.
                    throw new NotSupportedException("This page is missing a HtmlHead control which is required for " +
                        "the CSS stylesheet link that is being added. Please add <head runat=\"server\" />.");
                }

                bool addIt = true;  

                // looking for this stylesheet already in the header
                foreach (Control c in header.Controls)
                {
                    HtmlLink link = c as HtmlLink;
                    if (link != null && styleSheet.Equals(link.Href, StringComparison.OrdinalIgnoreCase))
                    {
                        addIt = false;
                        break;
                    }
                }

                // If true.
                if (addIt)
                {
                    HtmlLink link = new HtmlLink();
                    link.Href = styleSheet;
                    link.Attributes.Add("type", "text/css");
                    link.Attributes.Add("rel", "stylesheet");
                    header.Controls.Add(link);

                    // ASP.NET AJAX doesn't currently send a new head element down during an async postback,
                    // so we do the same thing on the client by registering the appropriate script for after
                    // the update. A HiddenField is used to prevent multiple registrations.
                    ScriptManager scriptManager = ScriptManager.GetCurrent(control.Page);
                    if (scriptManager == null)
                        throw new InvalidOperationException("A ScriptManager is required on the page to use ASP.NET AJAX Script Components.");

                    // If the hiden field has not been created.
                    if (scriptManager.IsInAsyncPostBack && (control.Page.Request.Form["__NequeoDataCalendarCssLoaded"] == null))
                    {
                        // Create the hidden css block of data.
                        ScriptManager.RegisterClientScriptBlock(control, control.GetType(), "RegisterCssReferences",
                            "var head = document.getElementsByTagName('HEAD')[0];" +
                            "if (head) {" +
                                "var linkElement = document.createElement('link');" +
                                "linkElement.type = 'text/css';" +
                                "linkElement.rel = 'stylesheet';" +
                                "linkElement.href = '" + styleSheet + "';" +
                                "head.appendChild(linkElement);" +
                            "}", true);

                        // Register the hidden field containg the css data.
                        ScriptManager.RegisterHiddenField(control, "__NequeoDataCalendarCssLoaded", "");
                    }
                }
            }
        }

        /// <summary>
        /// Gets the embedded css file references for a type
        /// </summary>
        /// <param name="control">The control to register the css resource for.</param>
        /// <param name="cssReferences">The collection of css reference objects.</param>
        /// <returns>The collection of css refrences.</returns>
        public static IEnumerable<string> GetCssReferences(Control control, string[] cssReferences)
        {
            // Make sure the page reference exists.
            if (control == null) throw new ArgumentNullException("control");

            return GetCssReferences(control, control.GetType(), new Stack<Type>(), cssReferences);
        }

        /// <summary>
        /// Gets the css references for a type and walks the type's dependencies with circular-reference checking
        /// </summary>
        /// <param name="control">The control to register the css resource for.</param>
        /// <param name="type">The current component rtype.</param>
        /// <param name="typeReferenceStack">The type stack refrence.</param>
        /// <param name="cssReferences">The collection of css reference objects.</param>
        /// <returns>The collection of css refrences.</returns>
        private static IEnumerable<string> GetCssReferences(Control control, Type type, Stack<Type> typeReferenceStack, string[] cssReferences)
        {
            // Verify no circular references
            if (typeReferenceStack.Contains(type))
                throw new InvalidOperationException("Circular reference detected.");

            // Look for a cached set of references
            IList<string> references;
            if (_cssCache.TryGetValue(type, out references))
                return references;

            // Track this type to prevent circular references
            typeReferenceStack.Push(type);
            try
            {
                // Lock the current thread
                lock (_sync)
                {
                    // double-checked lock
                    if (_cssCache.TryGetValue(type, out references))
                        return references;

                    // build the reference list
                    List<string> referenceList = new List<string>();

                    // Get the client script resource values for this type
                    List<ResourceEntry> entries = new List<ResourceEntry>();
                    int order = 0;

                    // Start at the top level type and iterate throught the
                    // base type of each type until the object base type.
                    for (Type current = type; current != null && current != typeof(object); current = current.BaseType)
                    {
                        string[] attrs = cssReferences;
                        order -= attrs.Length;

                        // For each css reference add to the entry collection.
                        foreach (string attr in attrs)
                            entries.Add(new ResourceEntry(attr, current, order ));
                    }

                    // Sort the collection of css refrences
                    entries.Sort(
                        delegate(ResourceEntry l, ResourceEntry r) 
                        { 
                            return l.Order.CompareTo(r.Order); 
                        }
                    );

                    // For each css entry add to the refrence list.
                    foreach (ResourceEntry entry in entries)
                        referenceList.Add(control.Page.ClientScript.GetWebResourceUrl(entry.ComponentType, entry.ResourcePath));

                    // Remove duplicates from reference list
                    Dictionary<string, object> cookies = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    List<string> newReferenceList = new List<string>();

                    // For each css reference.
                    foreach (string refr in referenceList)
                    {
                        if (cookies.ContainsKey(refr))
                            continue;

                        cookies.Add(refr, null);
                        newReferenceList.Add(refr);
                    }

                    // Create a readonly dictionary to hold the values
                    references = new ReadOnlyCollection<string>(newReferenceList);

                    // Cache the reference
                    _cssCache.Add(type, references);

                    // return the list
                    return references;
                }
            }
            finally
            {
                // Remove the type as further requests will get the cached reference
                typeReferenceStack.Pop();
            }
        }

        /// <summary>
        /// Resource entry structure.
        /// </summary>
        private struct ResourceEntry
        {
            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="componentType">The component type.</param>
            /// <param name="order">The order.</param>
            public ResourceEntry(string path, Type componentType, int order)
            {
                ResourcePath = path;
                ComponentType = componentType;
                Order = order;
            }

            public string ResourcePath;
            public Type ComponentType;
            public int Order;

            /// <summary>
            /// Gets, the assembly name.
            /// </summary>
            private string AssemblyName
            {
                get { return ComponentType == null ? "" : ComponentType.Assembly.FullName; }
            }

            /// <summary>
            /// Get the to script reference.
            /// </summary>
            /// <returns>The script reference.</returns>
            public ScriptReference ToScriptReference()
            {
                ScriptReference refr = new ScriptReference();
                refr.Assembly = AssemblyName;
                refr.Name = ResourcePath;
                return refr;
            }

            /// <summary>
            /// Equals override.
            /// </summary>
            /// <param name="obj">The object to match with this object.</param>
            /// <returns>True if equal.</returns>
            public override bool Equals(object obj)
            {
                ResourceEntry other = (ResourceEntry)obj;
                return ResourcePath.Equals(other.ResourcePath, StringComparison.OrdinalIgnoreCase)
                       && AssemblyName.Equals(other.AssemblyName, StringComparison.OrdinalIgnoreCase);
            }

            /// <summary>
            /// The static equal operator.
            /// </summary>
            /// <param name="obj1">The first object to match</param>
            /// <param name="obj2">The second object to match</param>
            /// <returns>True if equal.</returns>
            public static bool operator ==(ResourceEntry obj1, ResourceEntry obj2)
            {
                return obj1.Equals(obj2);
            }

            /// <summary>
            /// The static not equal operator.
            /// </summary>
            /// <param name="obj1">The first object to match</param>
            /// <param name="obj2">The second object to match</param>
            /// <returns>True if not equal.</returns>
            public static bool operator !=(ResourceEntry obj1, ResourceEntry obj2)
            {
                return !obj1.Equals(obj2);
            }

            /// <summary>
            /// Get the hash code override.
            /// </summary>
            /// <returns>The hash code value.</returns>
            public override int GetHashCode()
            {
                return AssemblyName.GetHashCode() ^ ResourcePath.GetHashCode();
            }
        }
    }
}
