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
            this.serviceInstallerPop3Ssl = new System.ServiceProcess.ServiceInstaller();
            this.serviceInstallerPop3Tls = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller
            // 
            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;
            // 
            // serviceInstallerPop3Ssl
            // 
            this.serviceInstallerPop3Ssl.Description = "This service controls all nequeo secure socket layer proxy pop3 servers. This ser" +
    "vice contains proxy servers to a pop3 server, all data sent to a pop3 client is " +
    "secure.";
            this.serviceInstallerPop3Ssl.DisplayName = "Nequeo Secure Proxy Pop3 Service";
            this.serviceInstallerPop3Ssl.ServiceName = "Pop3SslProxy";
            this.serviceInstallerPop3Ssl.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // serviceInstallerPop3Tls
            // 
            this.serviceInstallerPop3Tls.Description = "This service controls all nequeo transport layer security start command proxy pop" +
    "3 servers. This service contains proxy servers to a pop3 server, all data sent t" +
    "o a pop3 client is secure.";
            this.serviceInstallerPop3Tls.DisplayName = "Nequeo Transport Layer Security Start Command Proxy Pop3 Service";
            this.serviceInstallerPop3Tls.ServiceName = "Pop3TlsProxy";
            this.serviceInstallerPop3Tls.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller,
            this.serviceInstallerPop3Ssl,
            this.serviceInstallerPop3Tls});

		}

		#endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstallerPop3Ssl;
        private System.ServiceProcess.ServiceInstaller serviceInstallerPop3Tls;
	}
}