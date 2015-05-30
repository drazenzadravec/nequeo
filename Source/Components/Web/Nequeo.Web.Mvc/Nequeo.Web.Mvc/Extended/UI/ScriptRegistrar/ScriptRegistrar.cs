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
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Text;

using Nequeo.Extension;
using Nequeo.Collections.Extension;
using Nequeo.Net.Http.Extension;
using Nequeo.Web.Mvc.Extended.WebAsset;
using Nequeo.Web.Mvc.Extended.Factory;

namespace Nequeo.Web.Mvc.Extended.UI
{
    /// <summary>
    /// Manages ASP.NET MVC javascript files and statements.
    /// </summary>
    public class ScriptRegistrar : IScriptableComponentContainer
    {
        internal const string jQuery = "jquery-1.4.2.min.js";
        internal const string jQueryValidation = "jquery.validate.min.js";

        /// <summary>
        /// Used to ensure that the same instance is used for the same HttpContext.
        /// </summary>
        public static readonly string Key = typeof(ScriptRegistrar).AssemblyQualifiedName;
        private static readonly IList<string> frameworkScriptFileNames = new List<string> { jQuery };
        private readonly IList<IScriptableComponent> scriptableComponents;

        private string _assetHandlerPath;
        private bool _hasRendered;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptRegistrar"/> class.
        /// </summary>
        /// <param name="scripts">The scripts.</param>
        /// <param name="scriptableComponents">The scriptable components.</param>
        /// <param name="viewContext">The view context.</param>
        /// <param name="assetItemMerger">The asset merger.</param>
        /// <param name="scriptWrapper">The script wrapper.</param>
        public ScriptRegistrar(WebAssetItemCollection scripts, IList<IScriptableComponent> scriptableComponents,
            ViewContext viewContext, IWebAssetItemMerger assetItemMerger, ScriptWrapperBase scriptWrapper)
        {
            // If the instance object is null.
            if (scripts == null) throw new System.ArgumentNullException("scripts");
            if (scriptableComponents == null) throw new System.ArgumentNullException("scriptableComponents");
            if (viewContext == null) throw new System.ArgumentNullException("viewContext");
            if (assetItemMerger == null) throw new System.ArgumentNullException("assetItemMerger");
            if (scriptWrapper == null) throw new System.ArgumentNullException("scriptWrapper");

            if (viewContext.HttpContext.Items[Key] != null)
                throw new InvalidOperationException("Only one script registrar is allowed in a single request");

            viewContext.HttpContext.Items[Key] = this;

            DefaultGroup = new WebAssetItemGroup("default", false) { DefaultPath = WebAssetDefaultSettings.ScriptFilesPath };
            Scripts = scripts;
            Scripts.Insert(0, DefaultGroup);

            this.scriptableComponents = scriptableComponents;
            ViewContext = viewContext;
            AssetMerger = assetItemMerger;
            ScriptWrapper = scriptWrapper;
            AssetHandlerPath = WebAssetHttpHandler.DefaultPath;

            OnDocumentReadyActions = new List<Action>();
            OnDocumentReadyStatements = new List<string>();
            OnWindowUnloadActions = new List<Action>();
            OnWindowUnloadStatements = new List<string>();
        }

        /// <summary>
        /// Gets the framework script file names.
        /// </summary>
        /// <value>The framework script file names.</value>
        public static IList<string> FrameworkScriptFileNames
        {
            get { return frameworkScriptFileNames; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [exclude framework scripts].
        /// </summary>
        /// <value>
        /// <c>true</c> if [exclude framework scripts]; otherwise, <c>false</c>.
        /// </value>
        public bool ExcludeFrameworkScripts
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the asset handler path. Path must be a 
        /// virtual path. The default value is set to.
        /// </summary>
        /// <value>The asset handler path.</value>
        public string AssetHandlerPath
        {
            get { return _assetHandlerPath; }
            set { _assetHandlerPath = value; }
        }

        /// <summary>
        /// Gets the default script group.
        /// </summary>
        /// <value>The default group.</value>
        public WebAssetItemGroup DefaultGroup
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable globalization].
        /// </summary>
        /// <value>True if [enable globalization]; otherwise, false.</value>
        public bool EnableGlobalization
        {
            get; set;
        }

        /// <summary>
        /// Gets the scripts that will be rendered in the view.
        /// </summary>
        /// <value>The scripts.</value>
        public WebAssetItemCollection Scripts
        {
            get; private set;
        }

        /// <summary>
        /// Gets the on document ready actions.
        /// </summary>
        /// <value>The on page load actions.</value>
        public IList<Action> OnDocumentReadyActions
        {
            get; private set;
        }

        /// <summary>
        /// Gets the on document ready statements that is used in <code>RenderAction</code>.
        /// </summary>
        /// <value>The on page load actions.</value>
        public IList<string> OnDocumentReadyStatements
        {
            get; private set;
        }

        /// <summary>
        /// Gets the on window unload actions.
        /// </summary>
        /// <value>The on page unload actions.</value>
        public IList<Action> OnWindowUnloadActions
        {
            get; private set;
        }

        /// <summary>
        /// Gets the on window unload statements.that is used in <code>RenderAction</code>.
        /// </summary>
        /// <value>The on page load actions.</value>
        public IList<string> OnWindowUnloadStatements
        {
            get; private set;
        }

        /// <summary>
        /// Gets the view context.
        /// </summary>
        /// <value>The view context.</value>
        protected ViewContext ViewContext
        {
            get; private set;
        }

        /// <summary>
        /// Gets the asset merger.
        /// </summary>
        /// <value>The asset merger.</value>
        protected IWebAssetItemMerger AssetMerger
        {
            get; private set;
        }

        /// <summary>
        /// Gets the script wrapper that is used to write the script statements.
        /// </summary>
        /// <value>The script wrapper.</value>
        protected ScriptWrapperBase ScriptWrapper
        {
            get; private set;
        }

        /// <summary>
        /// Registers the scriptable component.
        /// </summary>
        /// <param name="component">The component.</param>
        public virtual void Register(IScriptableComponent component)
        {
            // If the instance object is null.
            if (component == null) throw new System.ArgumentNullException("component");

            if (!scriptableComponents.Contains(component))
                scriptableComponents.Add(component);
        }

        /// <summary>
        /// Writes the scripts in the response.
        /// </summary>
        public void Render()
        {
            if (_hasRendered)
                throw new InvalidOperationException("You cannot call render more than once");

            if (ViewContext.HttpContext.Request.Browser.EcmaScriptVersion.Major >= 1)
                Write(ViewContext.HttpContext.Response.Output);

            _hasRendered = true;
        }

        /// <summary>
        /// Writes all script source and script statements.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected virtual void Write(TextWriter writer)
        {
            WriteScriptSources(writer);
            WriteScriptStatements(writer);
        }

        /// <summary>
        /// Write the source script files.
        /// </summary>
        /// <param name="writer"></param>
        private void WriteScriptSources(TextWriter writer)
        {
            IList<string> mergedList = new List<string>();

            bool isSecured = ViewContext.HttpContext.Request.IsSecureConnection;
            bool canCompress = ViewContext.HttpContext.Request.CanCompress();

            Action<WebAssetItemCollection> append =
                assets =>
                {
                    IList<string> result = AssetMerger.Merge("application/x-javascript", AssetHandlerPath, isSecured, canCompress, assets);

                    if (!result.IsNullOrEmpty())
                        mergedList.AddRange(result);
                };

            CopyFrameworkScriptFiles();
            CopyScriptFilesFromComponents();

            if (!Scripts.IsEmpty())
                append(Scripts);

            if (!mergedList.IsEmpty())
                foreach (string script in mergedList)
                    writer.WriteLine("<script type=\"text/javascript\" src=\"{0}\"></script>".FormatWith(script));
        }

        /// <summary>
        /// Write the script statements.
        /// </summary>
        /// <param name="writer">The text writer attachec to the Mvc view context output stream.</param>
        private void WriteScriptStatements(TextWriter writer)
        {
            StringBuilder cleanUpScripts = WriteCleanUpScripts();

            bool shouldWriteOnDocumentReady = !scriptableComponents.IsEmpty() || !OnDocumentReadyActions.IsEmpty() || !OnDocumentReadyStatements.IsEmpty();
            bool shouldWriteOnWindowUnload = !OnWindowUnloadActions.IsEmpty() || !OnWindowUnloadStatements.IsEmpty() || cleanUpScripts.Length > 0;

            if (shouldWriteOnDocumentReady || shouldWriteOnWindowUnload)
            {
                bool isFirst;
                writer.WriteLine("<script type=\"text/javascript\">{0}//<![CDATA[".FormatWith(Environment.NewLine));

                // pageLoad
                if (shouldWriteOnDocumentReady)
                {
                    writer.WriteLine(ScriptWrapper.OnPageLoadStart);

                    // globalization
                    if (EnableGlobalization && CultureInfo.CurrentCulture.Name != "en-US")
                    {
                        writer.WriteLine("if (!jQuery.nequeo) jQuery.nequeo = {};");
                        GlobalizationInfo globalizationInfo = new GlobalizationInfo(CultureInfo.CurrentCulture);
                        writer.Write("jQuery.nequeo.cultureInfo=");
                        writer.Write(new JavaScriptSerializer().Serialize(globalizationInfo.ToDictionary()));
                        writer.WriteLine(";");
                    }

                    isFirst = true;

                    foreach (IScriptableComponent component in scriptableComponents.Where(s => !s.IsSelfInitialized))
                    {
                        if (!isFirst)
                            writer.WriteLine();

                        component.WriteInitializationScript(writer);
                        isFirst = false;
                    }

                    isFirst = true;

                    foreach (Action action in OnDocumentReadyActions)
                    {
                        if (!isFirst)
                            writer.WriteLine();

                        action();
                        isFirst = false;
                    }

                    isFirst = true;

                    foreach (string statement in OnDocumentReadyStatements)
                    {
                        if (!isFirst)
                            writer.WriteLine();

                        writer.Write(statement);
                        isFirst = false;
                    }

                    writer.WriteLine(ScriptWrapper.OnPageLoadEnd);
                }

                // pageUnload
                if (shouldWriteOnWindowUnload)
                {
                    writer.WriteLine(ScriptWrapper.OnPageUnloadStart);
                    isFirst = true;

                    foreach (Action action in OnWindowUnloadActions)
                    {
                        if (!isFirst)
                            writer.WriteLine();

                        action();
                        isFirst = false;
                    }

                    isFirst = true;

                    foreach (string statement in OnWindowUnloadStatements)
                    {
                        if (!isFirst)
                            writer.WriteLine();

                        writer.Write(statement);
                        isFirst = false;
                    }

                    writer.WriteLine(cleanUpScripts.ToString()); // write clean up scripts
                    writer.WriteLine(ScriptWrapper.OnPageUnloadEnd);
                }

                writer.Write("//]]>{0}</script>".FormatWith(Environment.NewLine));
            }
        }

        /// <summary>
        /// Write the cleaup scripts.
        /// </summary>
        /// <returns>The string builder.</returns>
        private StringBuilder WriteCleanUpScripts()
        {
            bool isFirst = true;
            StringWriter cleanupWriter = new StringWriter();

            foreach (IScriptableComponent component in scriptableComponents)
            {
                if (!isFirst)
                    cleanupWriter.WriteLine();

                component.WriteCleanupScript(cleanupWriter);
                isFirst = false;
            }

            return cleanupWriter.GetStringBuilder();
        }

        /// <summary>
        /// Copy the jQuery components.
        /// </summary>
        private void CopyScriptFilesFromComponents()
        {
            foreach (IScriptableComponent component in scriptableComponents)
            {
                string assetKey = string.IsNullOrEmpty(component.AssetKey) ? "default" : component.AssetKey;
                string filesPath = component.ScriptFilesPath;

                if (assetKey.IsCaseInsensitiveEqual("default") && WebAssetDefaultSettings.ScriptFilesPath.IsCaseInsensitiveEqual(filesPath))
                    if (!DefaultGroup.DefaultPath.IsCaseInsensitiveEqual(WebAssetDefaultSettings.ScriptFilesPath))
                        filesPath = DefaultGroup.DefaultPath;

                component.ScriptFileNames.Each(source => Scripts.Add(assetKey, MvcManager.CombinePath(filesPath, source)));
            }
        }

        /// <summary>
        /// Copy the jQuery files.
        /// </summary>
        private void CopyFrameworkScriptFiles()
        {
            if (!ExcludeFrameworkScripts)
            {
                FrameworkScriptFileNames.Reverse().Each(source => 
                    DefaultGroup.Items.Insert(0, new WebAssetItem(MvcManager.CombinePath(DefaultGroup.DefaultPath, source))));
            }
        }
    }
}