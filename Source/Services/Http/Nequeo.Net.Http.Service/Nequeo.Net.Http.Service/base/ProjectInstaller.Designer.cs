namespace Nequeo.Net.Http
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstallerHttpStaticSingleServer = new System.ServiceProcess.ServiceInstaller();
            this.serviceInstallerHttpStaticServer = new System.ServiceProcess.ServiceInstaller();
            this.serviceInstallerHttpDynamicSingleServer = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller
            // 
            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;
            // 
            // serviceInstallerHttpStaticSingleServer
            // 
            this.serviceInstallerHttpStaticSingleServer.Description = "This service is a http static content server, which only uses one processing thre" +
    "ad.";
            this.serviceInstallerHttpStaticSingleServer.DisplayName = "Nequeo Http Static Single Server";
            this.serviceInstallerHttpStaticSingleServer.ServiceName = "HttpStaticSingleServer";
            this.serviceInstallerHttpStaticSingleServer.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // serviceInstallerHttpStaticServer
            // 
            this.serviceInstallerHttpStaticServer.Description = "This service is a http static content server.";
            this.serviceInstallerHttpStaticServer.DisplayName = "Nequeo Http Static Server";
            this.serviceInstallerHttpStaticServer.ServiceName = "HttpStaticServer";
            this.serviceInstallerHttpStaticServer.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // serviceInstallerHttpDynamicSingleServer
            // 
            this.serviceInstallerHttpDynamicSingleServer.Description = "This service is a http dynamic content server, which only uses one processing thr" +
    "ead.";
            this.serviceInstallerHttpDynamicSingleServer.DisplayName = "Nequeo Http Dynamic Single Server";
            this.serviceInstallerHttpDynamicSingleServer.ServiceName = "HttpDynamicSingleServer";
            this.serviceInstallerHttpDynamicSingleServer.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller,
            this.serviceInstallerHttpStaticSingleServer,
            this.serviceInstallerHttpStaticServer,
            this.serviceInstallerHttpDynamicSingleServer});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstallerHttpStaticSingleServer;
        private System.ServiceProcess.ServiceInstaller serviceInstallerHttpStaticServer;
        private System.ServiceProcess.ServiceInstaller serviceInstallerHttpDynamicSingleServer;
    }
}