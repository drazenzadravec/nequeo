using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace Nequeo.Net.Download
{
    /// <summary>
    /// ProjectInstaller
    /// </summary>
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        /// <summary>
        /// ProjectInstaller
        /// </summary>
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}
