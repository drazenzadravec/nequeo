namespace Nequeo.Net.FileTransfer
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
            this.serviceInstallerFileTransfer = new System.ServiceProcess.ServiceInstaller();
            this.serviceInstallerFileTransferSsl = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller
            // 
            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;
            // 
            // serviceInstallerFileTransfer
            // 
            this.serviceInstallerFileTransfer.Description = "This service controls all nequeo file transfer servers. This service contains cus" +
    "tom servers for transfering files.";
            this.serviceInstallerFileTransfer.DisplayName = "Nequeo File Transfer Service";
            this.serviceInstallerFileTransfer.ServiceName = "FileTransfer";
            this.serviceInstallerFileTransfer.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // serviceInstallerFileTransferSsl
            // 
            this.serviceInstallerFileTransferSsl.Description = "This service controls all nequeo secure socket layer file transfer servers. This " +
    "service contains custom servers for transfering files.";
            this.serviceInstallerFileTransferSsl.DisplayName = "Nequeo Secure File Transfer Service";
            this.serviceInstallerFileTransferSsl.ServiceName = "FileTransferSsl";
            this.serviceInstallerFileTransferSsl.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller,
            this.serviceInstallerFileTransfer,
            this.serviceInstallerFileTransferSsl});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstallerFileTransfer;
        private System.ServiceProcess.ServiceInstaller serviceInstallerFileTransferSsl;
    }
}