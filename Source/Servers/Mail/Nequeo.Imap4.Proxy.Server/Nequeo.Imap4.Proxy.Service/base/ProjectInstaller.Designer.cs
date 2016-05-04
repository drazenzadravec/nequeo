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
            this.serviceInstallerImapSsl = new System.ServiceProcess.ServiceInstaller();
            this.serviceInstallerImapTls = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller
            // 
            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;
            // 
            // serviceInstallerImapSsl
            // 
            this.serviceInstallerImapSsl.Description = "This service controls all nequeo secure socket layer proxy imap4 servers. This se" +
                "rvice contains proxy servers to a imap4 server, all data sent to a imap4 client " +
                "is secure.";
            this.serviceInstallerImapSsl.DisplayName = "Nequeo Secure Proxy Imap4 Service";
            this.serviceInstallerImapSsl.ServiceName = "Imap4SslProxy";
            this.serviceInstallerImapSsl.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // serviceInstallerImapTls
            // 
            this.serviceInstallerImapTls.Description = "This service controls all nequeo transport layer security start command proxy ima" +
                "p4 servers. This service contains proxy servers to a imap4 server, all data sent" +
                " to a imap4 client is secure.";
            this.serviceInstallerImapTls.DisplayName = "Nequeo Transport Layer Security Start Command Proxy Imap4 Service";
            this.serviceInstallerImapTls.ServiceName = "Imap4TlsProxy";
            this.serviceInstallerImapTls.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller,
            this.serviceInstallerImapSsl,
            this.serviceInstallerImapTls});

		}

		#endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstallerImapSsl;
        private System.ServiceProcess.ServiceInstaller serviceInstallerImapTls;
	}
}