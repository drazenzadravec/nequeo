using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.InteropServices;

using EnvDTE;
using VSLangProj;
using VSLangProj2;
using VSOLE = Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Designer.Interfaces;

namespace Nequeo.CustomTool.CodeGenerator.Common
{
    /// <summary>
    /// Base code generator with site implementation
    /// </summary>
    public abstract class BaseCodeGeneratorWithSite : BaseCodeGenerator, VSOLE.IObjectWithSite
    {
        private object site = null;
        private CodeDomProvider codeDomProvider = null;
        private ServiceProvider serviceProvider = null;

        /// <summary>
        /// GetSite method of IOleObjectWithSite
        /// </summary>
        /// <param name="riid">interface to get</param>
        /// <param name="ppvSite">IntPtr in which to stuff return value</param>
        void VSOLE.IObjectWithSite.GetSite(ref Guid riid, out IntPtr ppvSite)
        {
            if (site == null)
            {
                throw new COMException("object is not sited", VSConstants.E_FAIL);
            }

            IntPtr pUnknownPointer = Marshal.GetIUnknownForObject(site);
            IntPtr intPointer = IntPtr.Zero;
            Marshal.QueryInterface(pUnknownPointer, ref riid, out intPointer);

            if (intPointer == IntPtr.Zero)
            {
                throw new COMException("site does not support requested interface", VSConstants.E_NOINTERFACE);
            }

            ppvSite = intPointer;
        }

        /// <summary>
        /// SetSite method of IOleObjectWithSite
        /// </summary>
        /// <param name="pUnkSite">site for this object to use</param>
        void VSOLE.IObjectWithSite.SetSite(object pUnkSite)
        {
            site = pUnkSite;
            codeDomProvider = null;
            serviceProvider = null;
        }

        /// <summary>
        /// Demand-creates a ServiceProvider
        /// </summary>
        private ServiceProvider SiteServiceProvider
        {
            get
            {
                if (serviceProvider == null)
                {
                    serviceProvider = new ServiceProvider(site as VSOLE.IServiceProvider);
                    Debug.Assert(serviceProvider != null, "Unable to get ServiceProvider from site object.");
                }
                return serviceProvider;
            }
        }

        /// <summary>
        /// Method to get a service by its GUID
        /// </summary>
        /// <param name="serviceGuid">GUID of service to retrieve</param>
        /// <returns>An object that implements the requested service</returns>
        protected object GetService(Guid serviceGuid)
        {
            return SiteServiceProvider.GetService(serviceGuid);
        }

        /// <summary>
        /// Method to get a service by its Type
        /// </summary>
        /// <param name="serviceType">Type of service to retrieve</param>
        /// <returns>An object that implements the requested service</returns>
        protected object GetService(Type serviceType)
        {
            return SiteServiceProvider.GetService(serviceType);
        }

        /// <summary>
        /// Returns a CodeDomProvider object for the language of the project containing
        /// the project item the generator was called on
        /// </summary>
        /// <returns>A CodeDomProvider object</returns>
        protected virtual CodeDomProvider GetCodeProvider()
        {
            if (codeDomProvider == null)
            {
                //Query for IVSMDCodeDomProvider/SVSMDCodeDomProvider for this project type
                IVSMDCodeDomProvider provider = GetService(typeof(Microsoft.VisualStudio.Shell.Interop.SVSMDCodeDomProvider)) as IVSMDCodeDomProvider;
                if (provider != null)
                {
                    codeDomProvider = provider.CodeDomProvider as CodeDomProvider;
                }
                else
                {
                    //In the case where no language specific CodeDom is available, fall back to CSharp
                    codeDomProvider = CodeDomProvider.CreateProvider("CSharp");
                }
            }
            return codeDomProvider;
        }

        /// <summary>
        /// Gets the default extension of the output file from the CodeDomProvider
        /// </summary>  
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            CodeDomProvider codeDom = GetCodeProvider();
            Debug.Assert(codeDom != null, "CodeDomProvider is NULL.");
            string extension = codeDom.FileExtension;
            if (extension != null && extension.Length > 0)
            {
                extension = "." + extension.TrimStart(".".ToCharArray());
            }
            return extension;
        }

        /// <summary>
        /// Returns the EnvDTE.ProjectItem object that corresponds to the project item the code 
        /// generator was called on
        /// </summary>
        /// <returns>The EnvDTE.ProjectItem of the project item the code generator was called on</returns>
        protected ProjectItem GetProjectItem()
        {
            object p = GetService(typeof(ProjectItem));
            Debug.Assert(p != null, "Unable to get Project Item.");
            return (ProjectItem)p;
        }

        /// <summary>
        /// Returns the EnvDTE.Project object of the project containing the project item the code 
        /// generator was called on
        /// </summary>
        /// <returns>
        /// The EnvDTE.Project object of the project containing the project item the code generator was called on
        /// </returns>
        protected Project GetProject()
        {
            return GetProjectItem().ContainingProject;
        }

        /// <summary>
        /// Returns the VSLangProj.VSProjectItem object that corresponds to the project item the code 
        /// generator was called on
        /// </summary>
        /// <returns>The VSLangProj.VSProjectItem of the project item the code generator was called on</returns>
        protected VSProjectItem GetVSProjectItem()
        {
            return (VSProjectItem)GetProjectItem().Object;
        }

        /// <summary>
        /// Returns the VSLangProj.VSProject object of the project containing the project item the code 
        /// generator was called on
        /// </summary>
        /// <returns>
        /// The VSLangProj.VSProject object of the project containing the project item 
        /// the code generator was called on
        /// </returns>
        protected VSProject GetVSProject()
        {
            return (VSProject)GetProject().Object;
        }
    }
}
