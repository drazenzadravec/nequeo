namespace Nequeo
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
            this.serviceInstallerSmtpSsl = new System.ServiceProcess.ServiceInstaller();
            this.serviceInstallerSmtpTsl = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller
            // 
            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;
            this.serviceProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstaller1_AfterInstall);
            // 
            // serviceInstallerSmtpSsl
            // 
            this.serviceInstallerSmtpSsl.Description = "This service controls all nequeo secure socket layer proxy smtp servers. This ser" +
    "vice contains proxy servers to a smtp server, all data sent to a smtp client is " +
    "secure.";
            this.serviceInstallerSmtpSsl.DisplayName = "Nequeo Secure Proxy Smtp Service";
            this.serviceInstallerSmtpSsl.ServiceName = "SmtpSslProxy";
            this.serviceInstallerSmtpSsl.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // serviceInstallerSmtpTsl
            // 
            this.serviceInstallerSmtpTsl.Description = "This service controls all nequeo transport layer security start command proxy smt" +
    "p servers. This service contains proxy servers to a smtp server, all data sent t" +
    "o a smtp client is secure.";
            this.serviceInstallerSmtpTsl.DisplayName = "Nequeo Transport Layer Security Start Command Proxy Smtp Service";
            this.serviceInstallerSmtpTsl.ServiceName = "SmtpTlsProxy";
            this.serviceInstallerSmtpTsl.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller,
            this.serviceInstallerSmtpSsl,
            this.serviceInstallerSmtpTsl});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstallerSmtpSsl;
        private System.ServiceProcess.ServiceInstaller serviceInstallerSmtpTsl;
    }
}