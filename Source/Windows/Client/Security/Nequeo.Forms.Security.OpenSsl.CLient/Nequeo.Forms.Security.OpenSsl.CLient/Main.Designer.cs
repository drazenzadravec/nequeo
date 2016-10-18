namespace Nequeo.Forms.Security.OpenSsl.Client
{
    partial class Main
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.groupBoxCreate = new System.Windows.Forms.GroupBox();
            this.labelCertificateFilePath = new System.Windows.Forms.Label();
            this.labelExportPassPhrase = new System.Windows.Forms.Label();
            this.createCertificateFileName = new System.Windows.Forms.Button();
            this.labelCertificateAuthorityPassPhrase = new System.Windows.Forms.Label();
            this.labelCertificatePassPhrase = new System.Windows.Forms.Label();
            this.checkBoxUseMultiDomain = new System.Windows.Forms.CheckBox();
            this.certificateFilePath = new System.Windows.Forms.TextBox();
            this.exportPassPhrase = new System.Windows.Forms.TextBox();
            this.certificateAuthorityPassPhrase = new System.Windows.Forms.TextBox();
            this.certificatePassPhrase = new System.Windows.Forms.TextBox();
            this.buttonCreatePIECertificate = new System.Windows.Forms.Button();
            this.labelemailAddress = new System.Windows.Forms.Label();
            this.labelcommonName = new System.Windows.Forms.Label();
            this.labelorganisationUnitName = new System.Windows.Forms.Label();
            this.labelorganisationName = new System.Windows.Forms.Label();
            this.labellocationName = new System.Windows.Forms.Label();
            this.labelstate = new System.Windows.Forms.Label();
            this.labelcountryName = new System.Windows.Forms.Label();
            this.emailAddress = new System.Windows.Forms.TextBox();
            this.commonName = new System.Windows.Forms.TextBox();
            this.organisationUnitName = new System.Windows.Forms.TextBox();
            this.organisationName = new System.Windows.Forms.TextBox();
            this.locationName = new System.Windows.Forms.TextBox();
            this.state = new System.Windows.Forms.TextBox();
            this.countryName = new System.Windows.Forms.TextBox();
            this.btnCreateCertificate = new System.Windows.Forms.Button();
            this.toolTipMain = new System.Windows.Forms.ToolTip(this.components);
            this.requestCertificateFilePath = new System.Windows.Forms.Button();
            this.btnRequestCertificate = new System.Windows.Forms.Button();
            this.btnSignCertificateRequestFilePath = new System.Windows.Forms.Button();
            this.btnSignCertificateFilePath = new System.Windows.Forms.Button();
            this.btnSignCertificate = new System.Windows.Forms.Button();
            this.btnCASelectCertificatePath = new System.Windows.Forms.Button();
            this.btnCASelectCertificatePrivateKeyPath = new System.Windows.Forms.Button();
            this.btnCACreateCertificateAuthority = new System.Windows.Forms.Button();
            this.btnRemoveCertificateDecryptedPath = new System.Windows.Forms.Button();
            this.btnRemoveCertificatePath = new System.Windows.Forms.Button();
            this.btnRemovePassword = new System.Windows.Forms.Button();
            this.btnExtractCertificatePath = new System.Windows.Forms.Button();
            this.btnRevokeCertificatePath = new System.Windows.Forms.Button();
            this.btnRevokeCertificateListPath = new System.Windows.Forms.Button();
            this.btnGenPublicPrivatePairPublicGo = new System.Windows.Forms.Button();
            this.piSignerPrivateKeySelect = new System.Windows.Forms.Button();
            this.piSignerCertificateSelect = new System.Windows.Forms.Button();
            this.psGenCertificate = new System.Windows.Forms.Button();
            this.psPIEPathSelect = new System.Windows.Forms.Button();
            this.psCertificatePathSelect = new System.Windows.Forms.Button();
            this.saveFileDialogMain = new System.Windows.Forms.SaveFileDialog();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageCreateCertificate = new System.Windows.Forms.TabPage();
            this.tabPageSignCertificate = new System.Windows.Forms.TabPage();
            this.groupBoxSign = new System.Windows.Forms.GroupBox();
            this.labelSignCertificateFilePath = new System.Windows.Forms.Label();
            this.checkBoxSignMultiDomian = new System.Windows.Forms.CheckBox();
            this.textSignCertificateFilePath = new System.Windows.Forms.TextBox();
            this.labelSignCertificateRequestFilePath = new System.Windows.Forms.Label();
            this.textSignCertificateRequestFilePath = new System.Windows.Forms.TextBox();
            this.labelSignCertificateAuthorityPassword = new System.Windows.Forms.Label();
            this.textSignCertificateAuthorityPassword = new System.Windows.Forms.TextBox();
            this.tabPageRequestCertificate = new System.Windows.Forms.TabPage();
            this.groupBoxRequest = new System.Windows.Forms.GroupBox();
            this.labelRequestCertificateFilePath = new System.Windows.Forms.Label();
            this.checkBoxRequestMultDomain = new System.Windows.Forms.CheckBox();
            this.textRequestCertificateFilePath = new System.Windows.Forms.TextBox();
            this.labelRequestCertificatePassword = new System.Windows.Forms.Label();
            this.textRequestCertificatePassword = new System.Windows.Forms.TextBox();
            this.labelRequestEmailAddress = new System.Windows.Forms.Label();
            this.labelRequestCommonName = new System.Windows.Forms.Label();
            this.labelRequestOrganisationUnitName = new System.Windows.Forms.Label();
            this.labelRequestOrganisationName = new System.Windows.Forms.Label();
            this.labelRequestLocationName = new System.Windows.Forms.Label();
            this.labelRequestState = new System.Windows.Forms.Label();
            this.labelRequestCountryName = new System.Windows.Forms.Label();
            this.textRequestEmailAddress = new System.Windows.Forms.TextBox();
            this.textRequestCommonName = new System.Windows.Forms.TextBox();
            this.textRequestOrganisationUnitName = new System.Windows.Forms.TextBox();
            this.textRequestOrganisationName = new System.Windows.Forms.TextBox();
            this.textRequestLocationName = new System.Windows.Forms.TextBox();
            this.textRequestState = new System.Windows.Forms.TextBox();
            this.textRequestCountryName = new System.Windows.Forms.TextBox();
            this.tabPageCreateCA = new System.Windows.Forms.TabPage();
            this.groupBoxCreateCA = new System.Windows.Forms.GroupBox();
            this.labelCADays = new System.Windows.Forms.Label();
            this.textCADays = new System.Windows.Forms.TextBox();
            this.textCACertificatePrivateKeyPath = new System.Windows.Forms.TextBox();
            this.labelCACertificatePrivateKeyPath = new System.Windows.Forms.Label();
            this.labelCACertificatePath = new System.Windows.Forms.Label();
            this.textCACertificatePath = new System.Windows.Forms.TextBox();
            this.labelCACertificatePassword = new System.Windows.Forms.Label();
            this.textCACertificatePassword = new System.Windows.Forms.TextBox();
            this.labelCAEmailAddress = new System.Windows.Forms.Label();
            this.labelCACommonName = new System.Windows.Forms.Label();
            this.labelCAOrganisationUnitName = new System.Windows.Forms.Label();
            this.labelCAOrganisationName = new System.Windows.Forms.Label();
            this.labelCALocationName = new System.Windows.Forms.Label();
            this.labelCAState = new System.Windows.Forms.Label();
            this.labelCACountryName = new System.Windows.Forms.Label();
            this.textCAEmailAddress = new System.Windows.Forms.TextBox();
            this.textCACommonName = new System.Windows.Forms.TextBox();
            this.textCAOrganisationUnitName = new System.Windows.Forms.TextBox();
            this.textCAOrganisationName = new System.Windows.Forms.TextBox();
            this.textCALocationName = new System.Windows.Forms.TextBox();
            this.textCAState = new System.Windows.Forms.TextBox();
            this.textCACountryName = new System.Windows.Forms.TextBox();
            this.tabPageRemovePassword = new System.Windows.Forms.TabPage();
            this.groupBoxRemovePassword = new System.Windows.Forms.GroupBox();
            this.textRemoveCertificateDecryptedPath = new System.Windows.Forms.TextBox();
            this.labelRemoveCertificateDecryptedPath = new System.Windows.Forms.Label();
            this.labelRemoveCertificatePath = new System.Windows.Forms.Label();
            this.textRemoveCertificatePath = new System.Windows.Forms.TextBox();
            this.labelRemoveCertificatePassword = new System.Windows.Forms.Label();
            this.textRemoveCertificatePassword = new System.Windows.Forms.TextBox();
            this.tabPageCertificateDetails = new System.Windows.Forms.TabPage();
            this.groupBoxCertificateDetails = new System.Windows.Forms.GroupBox();
            this.btnCertificateDetailsView = new System.Windows.Forms.Button();
            this.lblCertificateDetailsPassword = new System.Windows.Forms.Label();
            this.lblCertificateDetailsPath = new System.Windows.Forms.Label();
            this.btnCertificateDetailsLoad = new System.Windows.Forms.Button();
            this.btnCertificateDetailsPath = new System.Windows.Forms.Button();
            this.txtCertificateDetailsPassword = new System.Windows.Forms.TextBox();
            this.txtCertificateDetailsPath = new System.Windows.Forms.TextBox();
            this.richTextBoxCertificateDetails = new System.Windows.Forms.RichTextBox();
            this.tabPageCertificateCryptograph = new System.Windows.Forms.TabPage();
            this.groupBoxCertificateCryptography = new System.Windows.Forms.GroupBox();
            this.rsaCryptography = new Nequeo.Forms.UI.Security.RsaCryptography();
            this.tabPageExtractKeys = new System.Windows.Forms.TabPage();
            this.groupBoxExtractKeys = new System.Windows.Forms.GroupBox();
            this.btnExtractPublicKey = new System.Windows.Forms.Button();
            this.btnExtractPrivateKey = new System.Windows.Forms.Button();
            this.btnExtractPrivateKeyPath = new System.Windows.Forms.Button();
            this.btnExtractPublicKeyPath = new System.Windows.Forms.Button();
            this.lblExtractPrivateKeyPath = new System.Windows.Forms.Label();
            this.txtExtractPrivateKeyPath = new System.Windows.Forms.TextBox();
            this.txtExtractPublicKeyPath = new System.Windows.Forms.TextBox();
            this.lblExtractPublicKeyPath = new System.Windows.Forms.Label();
            this.lblExtractCertificatePath = new System.Windows.Forms.Label();
            this.lblExtractExportPassword = new System.Windows.Forms.Label();
            this.lblExtractCertificatePassword = new System.Windows.Forms.Label();
            this.txtExtractCertificatePath = new System.Windows.Forms.TextBox();
            this.txtExtractExportPassword = new System.Windows.Forms.TextBox();
            this.txtExtractCertificatePassword = new System.Windows.Forms.TextBox();
            this.tabPageCertificateRevoke = new System.Windows.Forms.TabPage();
            this.groupBoxRevoke = new System.Windows.Forms.GroupBox();
            this.btnRevokeUpdateDatabase = new System.Windows.Forms.Button();
            this.btnRevokeList = new System.Windows.Forms.Button();
            this.btnRevokeCertificate = new System.Windows.Forms.Button();
            this.btnRevokeListPath = new System.Windows.Forms.Button();
            this.txtRevokeListPath = new System.Windows.Forms.TextBox();
            this.lblRevokeListPath = new System.Windows.Forms.Label();
            this.lblRevokeCertificatePath = new System.Windows.Forms.Label();
            this.txtRevokeCertificatePath = new System.Windows.Forms.TextBox();
            this.lblRevokeCertificateListPath = new System.Windows.Forms.Label();
            this.txtRevokeCertificateListPath = new System.Windows.Forms.TextBox();
            this.lblRevokeCertificateAuthPassword = new System.Windows.Forms.Label();
            this.txtRevokeCertificateAuthPassword = new System.Windows.Forms.TextBox();
            this.tabPageGeneratePublicPrivateKeyPair = new System.Windows.Forms.TabPage();
            this.groupBoxGenPublicPrivatePair = new System.Windows.Forms.GroupBox();
            this.lblGenPublicPrivatePairSizePublicKey = new System.Windows.Forms.Label();
            this.lblGenPublicPrivatePairPrivateKey = new System.Windows.Forms.Label();
            this.lblGenPublicPrivatePairSize = new System.Windows.Forms.Label();
            this.btnGenPublicPrivatePairPrivateKey = new System.Windows.Forms.Button();
            this.btnGenPublicPrivatePairPublicKey = new System.Windows.Forms.Button();
            this.txtGenPublicPrivatePairSizePublicKey = new System.Windows.Forms.TextBox();
            this.txtGenPublicPrivatePairPrivateKey = new System.Windows.Forms.TextBox();
            this.txtGenPublicPrivatePairSize = new System.Windows.Forms.TextBox();
            this.tabPagePersonalCertificate = new System.Windows.Forms.TabPage();
            this.groupBoxPersonalCertificate = new System.Windows.Forms.GroupBox();
            this.tabControlPersonalCertificate = new System.Windows.Forms.TabControl();
            this.tabPagePersonalSubject = new System.Windows.Forms.TabPage();
            this.psKeySize = new System.Windows.Forms.TextBox();
            this.psValidDays = new System.Windows.Forms.TextBox();
            this.labelPSValidDays = new System.Windows.Forms.Label();
            this.labelPSKeySize = new System.Windows.Forms.Label();
            this.psPIEPath = new System.Windows.Forms.TextBox();
            this.psCertificatePath = new System.Windows.Forms.TextBox();
            this.psPassword = new System.Windows.Forms.TextBox();
            this.labelPSCertificatePassword = new System.Windows.Forms.Label();
            this.labelPSCertificateFilePath = new System.Windows.Forms.Label();
            this.labelPSPIEFilePath = new System.Windows.Forms.Label();
            this.psEmailAddress = new System.Windows.Forms.TextBox();
            this.psCommonName = new System.Windows.Forms.TextBox();
            this.psOrganisationUnitName = new System.Windows.Forms.TextBox();
            this.psOrganisationName = new System.Windows.Forms.TextBox();
            this.psLocationName = new System.Windows.Forms.TextBox();
            this.psState = new System.Windows.Forms.TextBox();
            this.psCountryName = new System.Windows.Forms.TextBox();
            this.labelPersonalSubjectEmailAddress = new System.Windows.Forms.Label();
            this.labelPersonalSubjectCommonName = new System.Windows.Forms.Label();
            this.labelPersonalSubjectOrganisationUnitName = new System.Windows.Forms.Label();
            this.labelPersonalSubjectOrganisationName = new System.Windows.Forms.Label();
            this.labelPersonalSubjectLocationName = new System.Windows.Forms.Label();
            this.labelPersonalSubjectState = new System.Windows.Forms.Label();
            this.labelPersonalSubjectCountryName = new System.Windows.Forms.Label();
            this.tabPagePersonalIssuer = new System.Windows.Forms.TabPage();
            this.piCertificateFilePath = new System.Windows.Forms.TextBox();
            this.piSignerPrivateFilePath = new System.Windows.Forms.TextBox();
            this.piPassword = new System.Windows.Forms.TextBox();
            this.labelPersonalIssuerSignerPrivateKeyPassword = new System.Windows.Forms.Label();
            this.labelPersonalIssuerSignerPrivateKeyFilePath = new System.Windows.Forms.Label();
            this.labelPersonalIssuerSignerCertificateFilePath = new System.Windows.Forms.Label();
            this.piEmailAddress = new System.Windows.Forms.TextBox();
            this.piCommonName = new System.Windows.Forms.TextBox();
            this.piOrganidationUnitName = new System.Windows.Forms.TextBox();
            this.piOrganidationName = new System.Windows.Forms.TextBox();
            this.piLocationName = new System.Windows.Forms.TextBox();
            this.piState = new System.Windows.Forms.TextBox();
            this.piCountryName = new System.Windows.Forms.TextBox();
            this.labelPersonalIssuerEmailAddress = new System.Windows.Forms.Label();
            this.labelPersonalIssuerCommonName = new System.Windows.Forms.Label();
            this.labelPersonalIssuerOrganisationUnitName = new System.Windows.Forms.Label();
            this.labelPersonalIssuerOrganisationName = new System.Windows.Forms.Label();
            this.labelPersonalIssuerLocationName = new System.Windows.Forms.Label();
            this.labelPersonalIssuerState = new System.Windows.Forms.Label();
            this.labelPersonalIssuerCountryName = new System.Windows.Forms.Label();
            this.openFileDialogMain = new System.Windows.Forms.OpenFileDialog();
            this.psSignatureAlgorithm = new System.Windows.Forms.ComboBox();
            this.labelPSSignatureAlgorithm = new System.Windows.Forms.Label();
            this.groupBoxCreate.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.tabPageCreateCertificate.SuspendLayout();
            this.tabPageSignCertificate.SuspendLayout();
            this.groupBoxSign.SuspendLayout();
            this.tabPageRequestCertificate.SuspendLayout();
            this.groupBoxRequest.SuspendLayout();
            this.tabPageCreateCA.SuspendLayout();
            this.groupBoxCreateCA.SuspendLayout();
            this.tabPageRemovePassword.SuspendLayout();
            this.groupBoxRemovePassword.SuspendLayout();
            this.tabPageCertificateDetails.SuspendLayout();
            this.groupBoxCertificateDetails.SuspendLayout();
            this.tabPageCertificateCryptograph.SuspendLayout();
            this.groupBoxCertificateCryptography.SuspendLayout();
            this.tabPageExtractKeys.SuspendLayout();
            this.groupBoxExtractKeys.SuspendLayout();
            this.tabPageCertificateRevoke.SuspendLayout();
            this.groupBoxRevoke.SuspendLayout();
            this.tabPageGeneratePublicPrivateKeyPair.SuspendLayout();
            this.groupBoxGenPublicPrivatePair.SuspendLayout();
            this.tabPagePersonalCertificate.SuspendLayout();
            this.groupBoxPersonalCertificate.SuspendLayout();
            this.tabControlPersonalCertificate.SuspendLayout();
            this.tabPagePersonalSubject.SuspendLayout();
            this.tabPagePersonalIssuer.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxCreate
            // 
            this.groupBoxCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxCreate.Controls.Add(this.labelCertificateFilePath);
            this.groupBoxCreate.Controls.Add(this.labelExportPassPhrase);
            this.groupBoxCreate.Controls.Add(this.createCertificateFileName);
            this.groupBoxCreate.Controls.Add(this.labelCertificateAuthorityPassPhrase);
            this.groupBoxCreate.Controls.Add(this.labelCertificatePassPhrase);
            this.groupBoxCreate.Controls.Add(this.checkBoxUseMultiDomain);
            this.groupBoxCreate.Controls.Add(this.certificateFilePath);
            this.groupBoxCreate.Controls.Add(this.exportPassPhrase);
            this.groupBoxCreate.Controls.Add(this.certificateAuthorityPassPhrase);
            this.groupBoxCreate.Controls.Add(this.certificatePassPhrase);
            this.groupBoxCreate.Controls.Add(this.buttonCreatePIECertificate);
            this.groupBoxCreate.Controls.Add(this.labelemailAddress);
            this.groupBoxCreate.Controls.Add(this.labelcommonName);
            this.groupBoxCreate.Controls.Add(this.labelorganisationUnitName);
            this.groupBoxCreate.Controls.Add(this.labelorganisationName);
            this.groupBoxCreate.Controls.Add(this.labellocationName);
            this.groupBoxCreate.Controls.Add(this.labelstate);
            this.groupBoxCreate.Controls.Add(this.labelcountryName);
            this.groupBoxCreate.Controls.Add(this.emailAddress);
            this.groupBoxCreate.Controls.Add(this.commonName);
            this.groupBoxCreate.Controls.Add(this.organisationUnitName);
            this.groupBoxCreate.Controls.Add(this.organisationName);
            this.groupBoxCreate.Controls.Add(this.locationName);
            this.groupBoxCreate.Controls.Add(this.state);
            this.groupBoxCreate.Controls.Add(this.countryName);
            this.groupBoxCreate.Controls.Add(this.btnCreateCertificate);
            this.groupBoxCreate.Location = new System.Drawing.Point(3, 3);
            this.groupBoxCreate.Name = "groupBoxCreate";
            this.groupBoxCreate.Size = new System.Drawing.Size(664, 443);
            this.groupBoxCreate.TabIndex = 0;
            this.groupBoxCreate.TabStop = false;
            this.groupBoxCreate.Text = "Certificate";
            // 
            // labelCertificateFilePath
            // 
            this.labelCertificateFilePath.AutoSize = true;
            this.labelCertificateFilePath.Location = new System.Drawing.Point(6, 326);
            this.labelCertificateFilePath.Name = "labelCertificateFilePath";
            this.labelCertificateFilePath.Size = new System.Drawing.Size(98, 13);
            this.labelCertificateFilePath.TabIndex = 21;
            this.labelCertificateFilePath.Text = "Certificate File Path";
            // 
            // labelExportPassPhrase
            // 
            this.labelExportPassPhrase.AutoSize = true;
            this.labelExportPassPhrase.Location = new System.Drawing.Point(6, 300);
            this.labelExportPassPhrase.Name = "labelExportPassPhrase";
            this.labelExportPassPhrase.Size = new System.Drawing.Size(106, 13);
            this.labelExportPassPhrase.TabIndex = 20;
            this.labelExportPassPhrase.Text = "PIE Export Password";
            // 
            // createCertificateFileName
            // 
            this.createCertificateFileName.Location = new System.Drawing.Point(441, 321);
            this.createCertificateFileName.Name = "createCertificateFileName";
            this.createCertificateFileName.Size = new System.Drawing.Size(27, 23);
            this.createCertificateFileName.TabIndex = 12;
            this.createCertificateFileName.Text = "...";
            this.toolTipMain.SetToolTip(this.createCertificateFileName, "Select the full path and file name of the certificate to create");
            this.createCertificateFileName.UseVisualStyleBackColor = true;
            this.createCertificateFileName.Click += new System.EventHandler(this.createCertificateFileName_Click);
            // 
            // labelCertificateAuthorityPassPhrase
            // 
            this.labelCertificateAuthorityPassPhrase.AutoSize = true;
            this.labelCertificateAuthorityPassPhrase.Location = new System.Drawing.Point(6, 274);
            this.labelCertificateAuthorityPassPhrase.Name = "labelCertificateAuthorityPassPhrase";
            this.labelCertificateAuthorityPassPhrase.Size = new System.Drawing.Size(147, 13);
            this.labelCertificateAuthorityPassPhrase.TabIndex = 19;
            this.labelCertificateAuthorityPassPhrase.Text = "Certificate Authority Password";
            // 
            // labelCertificatePassPhrase
            // 
            this.labelCertificatePassPhrase.AutoSize = true;
            this.labelCertificatePassPhrase.Location = new System.Drawing.Point(6, 248);
            this.labelCertificatePassPhrase.Name = "labelCertificatePassPhrase";
            this.labelCertificatePassPhrase.Size = new System.Drawing.Size(103, 13);
            this.labelCertificatePassPhrase.TabIndex = 18;
            this.labelCertificatePassPhrase.Text = "Certificate Password";
            // 
            // checkBoxUseMultiDomain
            // 
            this.checkBoxUseMultiDomain.AutoSize = true;
            this.checkBoxUseMultiDomain.Location = new System.Drawing.Point(160, 349);
            this.checkBoxUseMultiDomain.Name = "checkBoxUseMultiDomain";
            this.checkBoxUseMultiDomain.Size = new System.Drawing.Size(174, 17);
            this.checkBoxUseMultiDomain.TabIndex = 13;
            this.checkBoxUseMultiDomain.Text = "Use Multi Domain Configuration";
            this.checkBoxUseMultiDomain.UseVisualStyleBackColor = true;
            this.checkBoxUseMultiDomain.CheckedChanged += new System.EventHandler(this.checkBoxUseMultiDomain_CheckedChanged);
            // 
            // certificateFilePath
            // 
            this.certificateFilePath.Location = new System.Drawing.Point(160, 323);
            this.certificateFilePath.Name = "certificateFilePath";
            this.certificateFilePath.ReadOnly = true;
            this.certificateFilePath.Size = new System.Drawing.Size(275, 20);
            this.certificateFilePath.TabIndex = 11;
            this.certificateFilePath.TextChanged += new System.EventHandler(this.certificateFilePath_TextChanged);
            // 
            // exportPassPhrase
            // 
            this.exportPassPhrase.Location = new System.Drawing.Point(160, 297);
            this.exportPassPhrase.Name = "exportPassPhrase";
            this.exportPassPhrase.PasswordChar = '*';
            this.exportPassPhrase.Size = new System.Drawing.Size(153, 20);
            this.exportPassPhrase.TabIndex = 10;
            this.exportPassPhrase.TextChanged += new System.EventHandler(this.exportPassPhrase_TextChanged);
            // 
            // certificateAuthorityPassPhrase
            // 
            this.certificateAuthorityPassPhrase.Location = new System.Drawing.Point(160, 271);
            this.certificateAuthorityPassPhrase.Name = "certificateAuthorityPassPhrase";
            this.certificateAuthorityPassPhrase.PasswordChar = '*';
            this.certificateAuthorityPassPhrase.Size = new System.Drawing.Size(153, 20);
            this.certificateAuthorityPassPhrase.TabIndex = 9;
            this.certificateAuthorityPassPhrase.TextChanged += new System.EventHandler(this.certificateAuthorityPassPhrase_TextChanged);
            // 
            // certificatePassPhrase
            // 
            this.certificatePassPhrase.Location = new System.Drawing.Point(160, 245);
            this.certificatePassPhrase.Name = "certificatePassPhrase";
            this.certificatePassPhrase.PasswordChar = '*';
            this.certificatePassPhrase.Size = new System.Drawing.Size(153, 20);
            this.certificatePassPhrase.TabIndex = 8;
            this.certificatePassPhrase.TextChanged += new System.EventHandler(this.certificatePassPhrase_TextChanged);
            // 
            // buttonCreatePIECertificate
            // 
            this.buttonCreatePIECertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCreatePIECertificate.Enabled = false;
            this.buttonCreatePIECertificate.Location = new System.Drawing.Point(406, 405);
            this.buttonCreatePIECertificate.Name = "buttonCreatePIECertificate";
            this.buttonCreatePIECertificate.Size = new System.Drawing.Size(123, 23);
            this.buttonCreatePIECertificate.TabIndex = 15;
            this.buttonCreatePIECertificate.Text = "Create PIE Certificate";
            this.toolTipMain.SetToolTip(this.buttonCreatePIECertificate, "Create new request, sign and create new personal information exchange certificate" +
        ".");
            this.buttonCreatePIECertificate.UseVisualStyleBackColor = true;
            this.buttonCreatePIECertificate.Click += new System.EventHandler(this.buttonCreatePIECertificate_Click);
            // 
            // labelemailAddress
            // 
            this.labelemailAddress.AutoSize = true;
            this.labelemailAddress.Location = new System.Drawing.Point(7, 189);
            this.labelemailAddress.Name = "labelemailAddress";
            this.labelemailAddress.Size = new System.Drawing.Size(73, 13);
            this.labelemailAddress.TabIndex = 14;
            this.labelemailAddress.Text = "Email Address";
            // 
            // labelcommonName
            // 
            this.labelcommonName.AutoSize = true;
            this.labelcommonName.Location = new System.Drawing.Point(6, 163);
            this.labelcommonName.Name = "labelcommonName";
            this.labelcommonName.Size = new System.Drawing.Size(79, 13);
            this.labelcommonName.TabIndex = 13;
            this.labelcommonName.Text = "Common Name";
            // 
            // labelorganisationUnitName
            // 
            this.labelorganisationUnitName.AutoSize = true;
            this.labelorganisationUnitName.Location = new System.Drawing.Point(6, 137);
            this.labelorganisationUnitName.Name = "labelorganisationUnitName";
            this.labelorganisationUnitName.Size = new System.Drawing.Size(119, 13);
            this.labelorganisationUnitName.TabIndex = 12;
            this.labelorganisationUnitName.Text = "Organisation Unit Name";
            // 
            // labelorganisationName
            // 
            this.labelorganisationName.AutoSize = true;
            this.labelorganisationName.Location = new System.Drawing.Point(6, 111);
            this.labelorganisationName.Name = "labelorganisationName";
            this.labelorganisationName.Size = new System.Drawing.Size(97, 13);
            this.labelorganisationName.TabIndex = 11;
            this.labelorganisationName.Text = "Organisation Name";
            // 
            // labellocationName
            // 
            this.labellocationName.AutoSize = true;
            this.labellocationName.Location = new System.Drawing.Point(6, 85);
            this.labellocationName.Name = "labellocationName";
            this.labellocationName.Size = new System.Drawing.Size(79, 13);
            this.labellocationName.TabIndex = 10;
            this.labellocationName.Text = "Location Name";
            // 
            // labelstate
            // 
            this.labelstate.AutoSize = true;
            this.labelstate.Location = new System.Drawing.Point(6, 59);
            this.labelstate.Name = "labelstate";
            this.labelstate.Size = new System.Drawing.Size(32, 13);
            this.labelstate.TabIndex = 9;
            this.labelstate.Text = "State";
            // 
            // labelcountryName
            // 
            this.labelcountryName.AutoSize = true;
            this.labelcountryName.Location = new System.Drawing.Point(6, 33);
            this.labelcountryName.Name = "labelcountryName";
            this.labelcountryName.Size = new System.Drawing.Size(74, 13);
            this.labelcountryName.TabIndex = 8;
            this.labelcountryName.Text = "Country Name";
            // 
            // emailAddress
            // 
            this.emailAddress.Location = new System.Drawing.Point(160, 186);
            this.emailAddress.Name = "emailAddress";
            this.emailAddress.Size = new System.Drawing.Size(210, 20);
            this.emailAddress.TabIndex = 7;
            this.emailAddress.TextChanged += new System.EventHandler(this.emailAddress_TextChanged);
            // 
            // commonName
            // 
            this.commonName.Location = new System.Drawing.Point(160, 160);
            this.commonName.Name = "commonName";
            this.commonName.Size = new System.Drawing.Size(245, 20);
            this.commonName.TabIndex = 6;
            this.commonName.TextChanged += new System.EventHandler(this.commonName_TextChanged);
            // 
            // organisationUnitName
            // 
            this.organisationUnitName.Location = new System.Drawing.Point(160, 134);
            this.organisationUnitName.Name = "organisationUnitName";
            this.organisationUnitName.Size = new System.Drawing.Size(153, 20);
            this.organisationUnitName.TabIndex = 5;
            this.organisationUnitName.TextChanged += new System.EventHandler(this.organisationUnitName_TextChanged);
            // 
            // organisationName
            // 
            this.organisationName.Location = new System.Drawing.Point(160, 108);
            this.organisationName.Name = "organisationName";
            this.organisationName.Size = new System.Drawing.Size(180, 20);
            this.organisationName.TabIndex = 4;
            this.organisationName.TextChanged += new System.EventHandler(this.organisationName_TextChanged);
            // 
            // locationName
            // 
            this.locationName.Location = new System.Drawing.Point(160, 82);
            this.locationName.Name = "locationName";
            this.locationName.Size = new System.Drawing.Size(120, 20);
            this.locationName.TabIndex = 3;
            this.locationName.TextChanged += new System.EventHandler(this.locationName_TextChanged);
            // 
            // state
            // 
            this.state.Location = new System.Drawing.Point(160, 56);
            this.state.Name = "state";
            this.state.Size = new System.Drawing.Size(82, 20);
            this.state.TabIndex = 2;
            this.state.TextChanged += new System.EventHandler(this.state_TextChanged);
            // 
            // countryName
            // 
            this.countryName.Location = new System.Drawing.Point(160, 30);
            this.countryName.Name = "countryName";
            this.countryName.Size = new System.Drawing.Size(40, 20);
            this.countryName.TabIndex = 1;
            this.countryName.TextChanged += new System.EventHandler(this.countryName_TextChanged);
            // 
            // btnCreateCertificate
            // 
            this.btnCreateCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateCertificate.Enabled = false;
            this.btnCreateCertificate.Location = new System.Drawing.Point(535, 405);
            this.btnCreateCertificate.Name = "btnCreateCertificate";
            this.btnCreateCertificate.Size = new System.Drawing.Size(123, 23);
            this.btnCreateCertificate.TabIndex = 14;
            this.btnCreateCertificate.Text = "Create Certificate";
            this.toolTipMain.SetToolTip(this.btnCreateCertificate, "Create and sign the new certificate.");
            this.btnCreateCertificate.UseVisualStyleBackColor = true;
            this.btnCreateCertificate.Click += new System.EventHandler(this.btnCreateCertificate_Click);
            // 
            // requestCertificateFilePath
            // 
            this.requestCertificateFilePath.Location = new System.Drawing.Point(441, 269);
            this.requestCertificateFilePath.Name = "requestCertificateFilePath";
            this.requestCertificateFilePath.Size = new System.Drawing.Size(27, 23);
            this.requestCertificateFilePath.TabIndex = 10;
            this.requestCertificateFilePath.Text = "...";
            this.toolTipMain.SetToolTip(this.requestCertificateFilePath, "Select the full path and file name of the certificate to create");
            this.requestCertificateFilePath.UseVisualStyleBackColor = true;
            this.requestCertificateFilePath.Click += new System.EventHandler(this.requestCertificateFilePath_Click);
            // 
            // btnRequestCertificate
            // 
            this.btnRequestCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRequestCertificate.Enabled = false;
            this.btnRequestCertificate.Location = new System.Drawing.Point(538, 405);
            this.btnRequestCertificate.Name = "btnRequestCertificate";
            this.btnRequestCertificate.Size = new System.Drawing.Size(123, 23);
            this.btnRequestCertificate.TabIndex = 12;
            this.btnRequestCertificate.Text = "Create Certificate";
            this.toolTipMain.SetToolTip(this.btnRequestCertificate, "Create a new certificate.");
            this.btnRequestCertificate.UseVisualStyleBackColor = true;
            this.btnRequestCertificate.Click += new System.EventHandler(this.btnRequestCertificate_Click);
            // 
            // btnSignCertificateRequestFilePath
            // 
            this.btnSignCertificateRequestFilePath.Location = new System.Drawing.Point(440, 54);
            this.btnSignCertificateRequestFilePath.Name = "btnSignCertificateRequestFilePath";
            this.btnSignCertificateRequestFilePath.Size = new System.Drawing.Size(27, 23);
            this.btnSignCertificateRequestFilePath.TabIndex = 3;
            this.btnSignCertificateRequestFilePath.Text = "...";
            this.toolTipMain.SetToolTip(this.btnSignCertificateRequestFilePath, "Select the full path and file name of the certificate request to sign");
            this.btnSignCertificateRequestFilePath.UseVisualStyleBackColor = true;
            this.btnSignCertificateRequestFilePath.Click += new System.EventHandler(this.btnSignCertificateRequestFilePath_Click);
            // 
            // btnSignCertificateFilePath
            // 
            this.btnSignCertificateFilePath.Location = new System.Drawing.Point(440, 81);
            this.btnSignCertificateFilePath.Name = "btnSignCertificateFilePath";
            this.btnSignCertificateFilePath.Size = new System.Drawing.Size(27, 23);
            this.btnSignCertificateFilePath.TabIndex = 25;
            this.btnSignCertificateFilePath.Text = "...";
            this.toolTipMain.SetToolTip(this.btnSignCertificateFilePath, "Select the full path and file name of the certificate to create");
            this.btnSignCertificateFilePath.UseVisualStyleBackColor = true;
            this.btnSignCertificateFilePath.Click += new System.EventHandler(this.btnSignCertificateFilePath_Click);
            // 
            // btnSignCertificate
            // 
            this.btnSignCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSignCertificate.Enabled = false;
            this.btnSignCertificate.Location = new System.Drawing.Point(538, 405);
            this.btnSignCertificate.Name = "btnSignCertificate";
            this.btnSignCertificate.Size = new System.Drawing.Size(123, 23);
            this.btnSignCertificate.TabIndex = 28;
            this.btnSignCertificate.Text = "Sign Certificate";
            this.toolTipMain.SetToolTip(this.btnSignCertificate, "Sign the new certificate request");
            this.btnSignCertificate.UseVisualStyleBackColor = true;
            this.btnSignCertificate.Click += new System.EventHandler(this.btnSignCertificate_Click);
            // 
            // btnCASelectCertificatePath
            // 
            this.btnCASelectCertificatePath.Location = new System.Drawing.Point(441, 269);
            this.btnCASelectCertificatePath.Name = "btnCASelectCertificatePath";
            this.btnCASelectCertificatePath.Size = new System.Drawing.Size(27, 23);
            this.btnCASelectCertificatePath.TabIndex = 10;
            this.btnCASelectCertificatePath.Text = "...";
            this.toolTipMain.SetToolTip(this.btnCASelectCertificatePath, "Select the full path and file name of the certificate authority to create");
            this.btnCASelectCertificatePath.UseVisualStyleBackColor = true;
            this.btnCASelectCertificatePath.Click += new System.EventHandler(this.btnCASelectCertificatePath_Click);
            // 
            // btnCASelectCertificatePrivateKeyPath
            // 
            this.btnCASelectCertificatePrivateKeyPath.Location = new System.Drawing.Point(441, 295);
            this.btnCASelectCertificatePrivateKeyPath.Name = "btnCASelectCertificatePrivateKeyPath";
            this.btnCASelectCertificatePrivateKeyPath.Size = new System.Drawing.Size(27, 23);
            this.btnCASelectCertificatePrivateKeyPath.TabIndex = 12;
            this.btnCASelectCertificatePrivateKeyPath.Text = "...";
            this.toolTipMain.SetToolTip(this.btnCASelectCertificatePrivateKeyPath, "Select the full path and file name of the private key file certificate authority." +
        "");
            this.btnCASelectCertificatePrivateKeyPath.UseVisualStyleBackColor = true;
            this.btnCASelectCertificatePrivateKeyPath.Click += new System.EventHandler(this.btnCASelectCertificatePrivateKeyPath_Click);
            // 
            // btnCACreateCertificateAuthority
            // 
            this.btnCACreateCertificateAuthority.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCACreateCertificateAuthority.Enabled = false;
            this.btnCACreateCertificateAuthority.Location = new System.Drawing.Point(538, 405);
            this.btnCACreateCertificateAuthority.Name = "btnCACreateCertificateAuthority";
            this.btnCACreateCertificateAuthority.Size = new System.Drawing.Size(123, 23);
            this.btnCACreateCertificateAuthority.TabIndex = 13;
            this.btnCACreateCertificateAuthority.Text = "Create Certificate";
            this.toolTipMain.SetToolTip(this.btnCACreateCertificateAuthority, "Create a new certificate authority");
            this.btnCACreateCertificateAuthority.UseVisualStyleBackColor = true;
            this.btnCACreateCertificateAuthority.Click += new System.EventHandler(this.btnCACreateCertificateAuthority_Click);
            // 
            // btnRemoveCertificateDecryptedPath
            // 
            this.btnRemoveCertificateDecryptedPath.Location = new System.Drawing.Point(441, 80);
            this.btnRemoveCertificateDecryptedPath.Name = "btnRemoveCertificateDecryptedPath";
            this.btnRemoveCertificateDecryptedPath.Size = new System.Drawing.Size(27, 23);
            this.btnRemoveCertificateDecryptedPath.TabIndex = 5;
            this.btnRemoveCertificateDecryptedPath.Text = "...";
            this.toolTipMain.SetToolTip(this.btnRemoveCertificateDecryptedPath, "Select the full path and file name of the private key file certificate authority." +
        "");
            this.btnRemoveCertificateDecryptedPath.UseVisualStyleBackColor = true;
            this.btnRemoveCertificateDecryptedPath.Click += new System.EventHandler(this.btnRemoveCertificateDecryptedPath_Click);
            // 
            // btnRemoveCertificatePath
            // 
            this.btnRemoveCertificatePath.Location = new System.Drawing.Point(441, 54);
            this.btnRemoveCertificatePath.Name = "btnRemoveCertificatePath";
            this.btnRemoveCertificatePath.Size = new System.Drawing.Size(27, 23);
            this.btnRemoveCertificatePath.TabIndex = 3;
            this.btnRemoveCertificatePath.Text = "...";
            this.toolTipMain.SetToolTip(this.btnRemoveCertificatePath, "Select the full path and file name of the certificate authority to create");
            this.btnRemoveCertificatePath.UseVisualStyleBackColor = true;
            this.btnRemoveCertificatePath.Click += new System.EventHandler(this.btnRemoveCertificatePath_Click);
            // 
            // btnRemovePassword
            // 
            this.btnRemovePassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemovePassword.Enabled = false;
            this.btnRemovePassword.Location = new System.Drawing.Point(538, 405);
            this.btnRemovePassword.Name = "btnRemovePassword";
            this.btnRemovePassword.Size = new System.Drawing.Size(123, 23);
            this.btnRemovePassword.TabIndex = 6;
            this.btnRemovePassword.Text = "Remove Password";
            this.toolTipMain.SetToolTip(this.btnRemovePassword, "Remove the encryption password from the certificate");
            this.btnRemovePassword.UseVisualStyleBackColor = true;
            this.btnRemovePassword.Click += new System.EventHandler(this.btnRemovePassword_Click);
            // 
            // btnExtractCertificatePath
            // 
            this.btnExtractCertificatePath.Location = new System.Drawing.Point(441, 80);
            this.btnExtractCertificatePath.Name = "btnExtractCertificatePath";
            this.btnExtractCertificatePath.Size = new System.Drawing.Size(27, 23);
            this.btnExtractCertificatePath.TabIndex = 26;
            this.btnExtractCertificatePath.Text = "...";
            this.toolTipMain.SetToolTip(this.btnExtractCertificatePath, "Select the full path and file name of the certificate to create");
            this.btnExtractCertificatePath.UseVisualStyleBackColor = true;
            this.btnExtractCertificatePath.Click += new System.EventHandler(this.btnExtractCertificatePath_Click);
            // 
            // btnRevokeCertificatePath
            // 
            this.btnRevokeCertificatePath.Location = new System.Drawing.Point(440, 54);
            this.btnRevokeCertificatePath.Name = "btnRevokeCertificatePath";
            this.btnRevokeCertificatePath.Size = new System.Drawing.Size(27, 23);
            this.btnRevokeCertificatePath.TabIndex = 34;
            this.btnRevokeCertificatePath.Text = "...";
            this.toolTipMain.SetToolTip(this.btnRevokeCertificatePath, "Select the full path and file name of the certificate to create");
            this.btnRevokeCertificatePath.UseVisualStyleBackColor = true;
            this.btnRevokeCertificatePath.Click += new System.EventHandler(this.btnRevokeCertificatePath_Click);
            // 
            // btnRevokeCertificateListPath
            // 
            this.btnRevokeCertificateListPath.Location = new System.Drawing.Point(440, 80);
            this.btnRevokeCertificateListPath.Name = "btnRevokeCertificateListPath";
            this.btnRevokeCertificateListPath.Size = new System.Drawing.Size(27, 23);
            this.btnRevokeCertificateListPath.TabIndex = 30;
            this.btnRevokeCertificateListPath.Text = "...";
            this.toolTipMain.SetToolTip(this.btnRevokeCertificateListPath, "Select the full path and file name of the certificate request to sign");
            this.btnRevokeCertificateListPath.UseVisualStyleBackColor = true;
            this.btnRevokeCertificateListPath.Click += new System.EventHandler(this.btnRevokeCertificateListPath_Click);
            // 
            // btnGenPublicPrivatePairPublicGo
            // 
            this.btnGenPublicPrivatePairPublicGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenPublicPrivatePairPublicGo.Enabled = false;
            this.btnGenPublicPrivatePairPublicGo.Location = new System.Drawing.Point(538, 405);
            this.btnGenPublicPrivatePairPublicGo.Name = "btnGenPublicPrivatePairPublicGo";
            this.btnGenPublicPrivatePairPublicGo.Size = new System.Drawing.Size(123, 23);
            this.btnGenPublicPrivatePairPublicGo.TabIndex = 5;
            this.btnGenPublicPrivatePairPublicGo.Text = "Generate Key Pair";
            this.toolTipMain.SetToolTip(this.btnGenPublicPrivatePairPublicGo, "Generate a public private key pair.");
            this.btnGenPublicPrivatePairPublicGo.UseVisualStyleBackColor = true;
            this.btnGenPublicPrivatePairPublicGo.Click += new System.EventHandler(this.btnGenPublicPrivatePairPublicGo_Click);
            // 
            // piSignerPrivateKeySelect
            // 
            this.piSignerPrivateKeySelect.Location = new System.Drawing.Point(441, 269);
            this.piSignerPrivateKeySelect.Name = "piSignerPrivateKeySelect";
            this.piSignerPrivateKeySelect.Size = new System.Drawing.Size(27, 23);
            this.piSignerPrivateKeySelect.TabIndex = 10;
            this.piSignerPrivateKeySelect.Text = "...";
            this.toolTipMain.SetToolTip(this.piSignerPrivateKeySelect, "Select the file that contains the private key used to sign the certificate.");
            this.piSignerPrivateKeySelect.UseVisualStyleBackColor = true;
            this.piSignerPrivateKeySelect.Click += new System.EventHandler(this.piSignerPrivateKeySelect_Click);
            // 
            // piSignerCertificateSelect
            // 
            this.piSignerCertificateSelect.Location = new System.Drawing.Point(441, 295);
            this.piSignerCertificateSelect.Name = "piSignerCertificateSelect";
            this.piSignerCertificateSelect.Size = new System.Drawing.Size(27, 23);
            this.piSignerCertificateSelect.TabIndex = 12;
            this.piSignerCertificateSelect.Text = "...";
            this.toolTipMain.SetToolTip(this.piSignerCertificateSelect, "Select the file that contains the certificate used to sign the certificate.");
            this.piSignerCertificateSelect.UseVisualStyleBackColor = true;
            this.piSignerCertificateSelect.Click += new System.EventHandler(this.piSignerCertificateSelect_Click);
            // 
            // psGenCertificate
            // 
            this.psGenCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.psGenCertificate.Enabled = false;
            this.psGenCertificate.Location = new System.Drawing.Point(521, 370);
            this.psGenCertificate.Name = "psGenCertificate";
            this.psGenCertificate.Size = new System.Drawing.Size(123, 23);
            this.psGenCertificate.TabIndex = 16;
            this.psGenCertificate.Text = "Create Certificate";
            this.toolTipMain.SetToolTip(this.psGenCertificate, "Create and sign the new certificate.");
            this.psGenCertificate.UseVisualStyleBackColor = true;
            this.psGenCertificate.Click += new System.EventHandler(this.psGenCertificate_Click);
            // 
            // psPIEPathSelect
            // 
            this.psPIEPathSelect.Location = new System.Drawing.Point(441, 295);
            this.psPIEPathSelect.Name = "psPIEPathSelect";
            this.psPIEPathSelect.Size = new System.Drawing.Size(27, 23);
            this.psPIEPathSelect.TabIndex = 12;
            this.psPIEPathSelect.Text = "...";
            this.toolTipMain.SetToolTip(this.psPIEPathSelect, "Select the file that contains the certificate used to sign the certificate.");
            this.psPIEPathSelect.UseVisualStyleBackColor = true;
            this.psPIEPathSelect.Click += new System.EventHandler(this.psPIEPathSelect_Click);
            // 
            // psCertificatePathSelect
            // 
            this.psCertificatePathSelect.Location = new System.Drawing.Point(441, 269);
            this.psCertificatePathSelect.Name = "psCertificatePathSelect";
            this.psCertificatePathSelect.Size = new System.Drawing.Size(27, 23);
            this.psCertificatePathSelect.TabIndex = 10;
            this.psCertificatePathSelect.Text = "...";
            this.toolTipMain.SetToolTip(this.psCertificatePathSelect, "Select the file that contains the private key used to sign the certificate.");
            this.psCertificatePathSelect.UseVisualStyleBackColor = true;
            this.psCertificatePathSelect.Click += new System.EventHandler(this.psCertificatePathSelect_Click);
            // 
            // saveFileDialogMain
            // 
            this.saveFileDialogMain.DefaultExt = "pem";
            this.saveFileDialogMain.Filter = "Personal Information Exchange Certificate Files (*.pfx *.p12)|*.pfx;*.p12|Certifi" +
    "cate Files (*.crt *.cer *.pem)|*.crt;*.cer;*.pem";
            this.saveFileDialogMain.SupportMultiDottedExtensions = true;
            this.saveFileDialogMain.Title = "Save Certificate";
            // 
            // tabControlMain
            // 
            this.tabControlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlMain.Controls.Add(this.tabPageCreateCertificate);
            this.tabControlMain.Controls.Add(this.tabPageSignCertificate);
            this.tabControlMain.Controls.Add(this.tabPageRequestCertificate);
            this.tabControlMain.Controls.Add(this.tabPageCreateCA);
            this.tabControlMain.Controls.Add(this.tabPageRemovePassword);
            this.tabControlMain.Controls.Add(this.tabPageCertificateDetails);
            this.tabControlMain.Controls.Add(this.tabPageCertificateCryptograph);
            this.tabControlMain.Controls.Add(this.tabPageExtractKeys);
            this.tabControlMain.Controls.Add(this.tabPageCertificateRevoke);
            this.tabControlMain.Controls.Add(this.tabPageGeneratePublicPrivateKeyPair);
            this.tabControlMain.Controls.Add(this.tabPagePersonalCertificate);
            this.tabControlMain.Location = new System.Drawing.Point(12, 12);
            this.tabControlMain.Multiline = true;
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(681, 503);
            this.tabControlMain.TabIndex = 1;
            // 
            // tabPageCreateCertificate
            // 
            this.tabPageCreateCertificate.Controls.Add(this.groupBoxCreate);
            this.tabPageCreateCertificate.Location = new System.Drawing.Point(4, 40);
            this.tabPageCreateCertificate.Name = "tabPageCreateCertificate";
            this.tabPageCreateCertificate.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCreateCertificate.Size = new System.Drawing.Size(673, 449);
            this.tabPageCreateCertificate.TabIndex = 0;
            this.tabPageCreateCertificate.Text = "Create Certificate";
            this.tabPageCreateCertificate.UseVisualStyleBackColor = true;
            // 
            // tabPageSignCertificate
            // 
            this.tabPageSignCertificate.Controls.Add(this.groupBoxSign);
            this.tabPageSignCertificate.Location = new System.Drawing.Point(4, 40);
            this.tabPageSignCertificate.Name = "tabPageSignCertificate";
            this.tabPageSignCertificate.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSignCertificate.Size = new System.Drawing.Size(673, 449);
            this.tabPageSignCertificate.TabIndex = 1;
            this.tabPageSignCertificate.Text = "Sign Certificate";
            this.tabPageSignCertificate.UseVisualStyleBackColor = true;
            // 
            // groupBoxSign
            // 
            this.groupBoxSign.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSign.Controls.Add(this.btnSignCertificate);
            this.groupBoxSign.Controls.Add(this.labelSignCertificateFilePath);
            this.groupBoxSign.Controls.Add(this.btnSignCertificateFilePath);
            this.groupBoxSign.Controls.Add(this.checkBoxSignMultiDomian);
            this.groupBoxSign.Controls.Add(this.textSignCertificateFilePath);
            this.groupBoxSign.Controls.Add(this.btnSignCertificateRequestFilePath);
            this.groupBoxSign.Controls.Add(this.labelSignCertificateRequestFilePath);
            this.groupBoxSign.Controls.Add(this.textSignCertificateRequestFilePath);
            this.groupBoxSign.Controls.Add(this.labelSignCertificateAuthorityPassword);
            this.groupBoxSign.Controls.Add(this.textSignCertificateAuthorityPassword);
            this.groupBoxSign.Location = new System.Drawing.Point(3, 3);
            this.groupBoxSign.Name = "groupBoxSign";
            this.groupBoxSign.Size = new System.Drawing.Size(667, 443);
            this.groupBoxSign.TabIndex = 0;
            this.groupBoxSign.TabStop = false;
            this.groupBoxSign.Text = "Certificate";
            // 
            // labelSignCertificateFilePath
            // 
            this.labelSignCertificateFilePath.AutoSize = true;
            this.labelSignCertificateFilePath.Location = new System.Drawing.Point(6, 86);
            this.labelSignCertificateFilePath.Name = "labelSignCertificateFilePath";
            this.labelSignCertificateFilePath.Size = new System.Drawing.Size(98, 13);
            this.labelSignCertificateFilePath.TabIndex = 27;
            this.labelSignCertificateFilePath.Text = "Certificate File Path";
            // 
            // checkBoxSignMultiDomian
            // 
            this.checkBoxSignMultiDomian.AutoSize = true;
            this.checkBoxSignMultiDomian.Location = new System.Drawing.Point(159, 109);
            this.checkBoxSignMultiDomian.Name = "checkBoxSignMultiDomian";
            this.checkBoxSignMultiDomian.Size = new System.Drawing.Size(174, 17);
            this.checkBoxSignMultiDomian.TabIndex = 26;
            this.checkBoxSignMultiDomian.Text = "Use Multi Domain Configuration";
            this.checkBoxSignMultiDomian.UseVisualStyleBackColor = true;
            // 
            // textSignCertificateFilePath
            // 
            this.textSignCertificateFilePath.Location = new System.Drawing.Point(159, 83);
            this.textSignCertificateFilePath.Name = "textSignCertificateFilePath";
            this.textSignCertificateFilePath.ReadOnly = true;
            this.textSignCertificateFilePath.Size = new System.Drawing.Size(275, 20);
            this.textSignCertificateFilePath.TabIndex = 24;
            this.textSignCertificateFilePath.TextChanged += new System.EventHandler(this.textSignCertificateFilePath_TextChanged);
            // 
            // labelSignCertificateRequestFilePath
            // 
            this.labelSignCertificateRequestFilePath.AutoSize = true;
            this.labelSignCertificateRequestFilePath.Location = new System.Drawing.Point(6, 59);
            this.labelSignCertificateRequestFilePath.Name = "labelSignCertificateRequestFilePath";
            this.labelSignCertificateRequestFilePath.Size = new System.Drawing.Size(141, 13);
            this.labelSignCertificateRequestFilePath.TabIndex = 23;
            this.labelSignCertificateRequestFilePath.Text = "Certificate Request File Path";
            // 
            // textSignCertificateRequestFilePath
            // 
            this.textSignCertificateRequestFilePath.Location = new System.Drawing.Point(159, 56);
            this.textSignCertificateRequestFilePath.Name = "textSignCertificateRequestFilePath";
            this.textSignCertificateRequestFilePath.ReadOnly = true;
            this.textSignCertificateRequestFilePath.Size = new System.Drawing.Size(275, 20);
            this.textSignCertificateRequestFilePath.TabIndex = 2;
            this.textSignCertificateRequestFilePath.TextChanged += new System.EventHandler(this.textSignCertificateRequestFilePath_TextChanged);
            // 
            // labelSignCertificateAuthorityPassword
            // 
            this.labelSignCertificateAuthorityPassword.AutoSize = true;
            this.labelSignCertificateAuthorityPassword.Location = new System.Drawing.Point(6, 33);
            this.labelSignCertificateAuthorityPassword.Name = "labelSignCertificateAuthorityPassword";
            this.labelSignCertificateAuthorityPassword.Size = new System.Drawing.Size(147, 13);
            this.labelSignCertificateAuthorityPassword.TabIndex = 21;
            this.labelSignCertificateAuthorityPassword.Text = "Certificate Authority Password";
            // 
            // textSignCertificateAuthorityPassword
            // 
            this.textSignCertificateAuthorityPassword.Location = new System.Drawing.Point(159, 30);
            this.textSignCertificateAuthorityPassword.Name = "textSignCertificateAuthorityPassword";
            this.textSignCertificateAuthorityPassword.PasswordChar = '*';
            this.textSignCertificateAuthorityPassword.Size = new System.Drawing.Size(153, 20);
            this.textSignCertificateAuthorityPassword.TabIndex = 1;
            this.textSignCertificateAuthorityPassword.TextChanged += new System.EventHandler(this.textSignCertificateAuthorityPassword_TextChanged);
            // 
            // tabPageRequestCertificate
            // 
            this.tabPageRequestCertificate.Controls.Add(this.groupBoxRequest);
            this.tabPageRequestCertificate.Location = new System.Drawing.Point(4, 40);
            this.tabPageRequestCertificate.Name = "tabPageRequestCertificate";
            this.tabPageRequestCertificate.Size = new System.Drawing.Size(673, 449);
            this.tabPageRequestCertificate.TabIndex = 2;
            this.tabPageRequestCertificate.Text = "Request Certificate";
            this.tabPageRequestCertificate.UseVisualStyleBackColor = true;
            // 
            // groupBoxRequest
            // 
            this.groupBoxRequest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxRequest.Controls.Add(this.btnRequestCertificate);
            this.groupBoxRequest.Controls.Add(this.labelRequestCertificateFilePath);
            this.groupBoxRequest.Controls.Add(this.requestCertificateFilePath);
            this.groupBoxRequest.Controls.Add(this.checkBoxRequestMultDomain);
            this.groupBoxRequest.Controls.Add(this.textRequestCertificateFilePath);
            this.groupBoxRequest.Controls.Add(this.labelRequestCertificatePassword);
            this.groupBoxRequest.Controls.Add(this.textRequestCertificatePassword);
            this.groupBoxRequest.Controls.Add(this.labelRequestEmailAddress);
            this.groupBoxRequest.Controls.Add(this.labelRequestCommonName);
            this.groupBoxRequest.Controls.Add(this.labelRequestOrganisationUnitName);
            this.groupBoxRequest.Controls.Add(this.labelRequestOrganisationName);
            this.groupBoxRequest.Controls.Add(this.labelRequestLocationName);
            this.groupBoxRequest.Controls.Add(this.labelRequestState);
            this.groupBoxRequest.Controls.Add(this.labelRequestCountryName);
            this.groupBoxRequest.Controls.Add(this.textRequestEmailAddress);
            this.groupBoxRequest.Controls.Add(this.textRequestCommonName);
            this.groupBoxRequest.Controls.Add(this.textRequestOrganisationUnitName);
            this.groupBoxRequest.Controls.Add(this.textRequestOrganisationName);
            this.groupBoxRequest.Controls.Add(this.textRequestLocationName);
            this.groupBoxRequest.Controls.Add(this.textRequestState);
            this.groupBoxRequest.Controls.Add(this.textRequestCountryName);
            this.groupBoxRequest.Location = new System.Drawing.Point(3, 3);
            this.groupBoxRequest.Name = "groupBoxRequest";
            this.groupBoxRequest.Size = new System.Drawing.Size(667, 443);
            this.groupBoxRequest.TabIndex = 0;
            this.groupBoxRequest.TabStop = false;
            this.groupBoxRequest.Text = "Certificate";
            // 
            // labelRequestCertificateFilePath
            // 
            this.labelRequestCertificateFilePath.AutoSize = true;
            this.labelRequestCertificateFilePath.Location = new System.Drawing.Point(7, 274);
            this.labelRequestCertificateFilePath.Name = "labelRequestCertificateFilePath";
            this.labelRequestCertificateFilePath.Size = new System.Drawing.Size(98, 13);
            this.labelRequestCertificateFilePath.TabIndex = 34;
            this.labelRequestCertificateFilePath.Text = "Certificate File Path";
            // 
            // checkBoxRequestMultDomain
            // 
            this.checkBoxRequestMultDomain.AutoSize = true;
            this.checkBoxRequestMultDomain.Location = new System.Drawing.Point(160, 297);
            this.checkBoxRequestMultDomain.Name = "checkBoxRequestMultDomain";
            this.checkBoxRequestMultDomain.Size = new System.Drawing.Size(174, 17);
            this.checkBoxRequestMultDomain.TabIndex = 11;
            this.checkBoxRequestMultDomain.Text = "Use Multi Domain Configuration";
            this.checkBoxRequestMultDomain.UseVisualStyleBackColor = true;
            // 
            // textRequestCertificateFilePath
            // 
            this.textRequestCertificateFilePath.Location = new System.Drawing.Point(160, 271);
            this.textRequestCertificateFilePath.Name = "textRequestCertificateFilePath";
            this.textRequestCertificateFilePath.ReadOnly = true;
            this.textRequestCertificateFilePath.Size = new System.Drawing.Size(275, 20);
            this.textRequestCertificateFilePath.TabIndex = 9;
            this.textRequestCertificateFilePath.TextChanged += new System.EventHandler(this.textRequestCertificateFilePath_TextChanged);
            // 
            // labelRequestCertificatePassword
            // 
            this.labelRequestCertificatePassword.AutoSize = true;
            this.labelRequestCertificatePassword.Location = new System.Drawing.Point(6, 248);
            this.labelRequestCertificatePassword.Name = "labelRequestCertificatePassword";
            this.labelRequestCertificatePassword.Size = new System.Drawing.Size(103, 13);
            this.labelRequestCertificatePassword.TabIndex = 30;
            this.labelRequestCertificatePassword.Text = "Certificate Password";
            // 
            // textRequestCertificatePassword
            // 
            this.textRequestCertificatePassword.Location = new System.Drawing.Point(160, 245);
            this.textRequestCertificatePassword.Name = "textRequestCertificatePassword";
            this.textRequestCertificatePassword.PasswordChar = '*';
            this.textRequestCertificatePassword.Size = new System.Drawing.Size(153, 20);
            this.textRequestCertificatePassword.TabIndex = 8;
            this.textRequestCertificatePassword.TextChanged += new System.EventHandler(this.textRequestCertificatePassword_TextChanged);
            // 
            // labelRequestEmailAddress
            // 
            this.labelRequestEmailAddress.AutoSize = true;
            this.labelRequestEmailAddress.Location = new System.Drawing.Point(7, 189);
            this.labelRequestEmailAddress.Name = "labelRequestEmailAddress";
            this.labelRequestEmailAddress.Size = new System.Drawing.Size(73, 13);
            this.labelRequestEmailAddress.TabIndex = 28;
            this.labelRequestEmailAddress.Text = "Email Address";
            // 
            // labelRequestCommonName
            // 
            this.labelRequestCommonName.AutoSize = true;
            this.labelRequestCommonName.Location = new System.Drawing.Point(6, 163);
            this.labelRequestCommonName.Name = "labelRequestCommonName";
            this.labelRequestCommonName.Size = new System.Drawing.Size(79, 13);
            this.labelRequestCommonName.TabIndex = 27;
            this.labelRequestCommonName.Text = "Common Name";
            // 
            // labelRequestOrganisationUnitName
            // 
            this.labelRequestOrganisationUnitName.AutoSize = true;
            this.labelRequestOrganisationUnitName.Location = new System.Drawing.Point(6, 137);
            this.labelRequestOrganisationUnitName.Name = "labelRequestOrganisationUnitName";
            this.labelRequestOrganisationUnitName.Size = new System.Drawing.Size(119, 13);
            this.labelRequestOrganisationUnitName.TabIndex = 26;
            this.labelRequestOrganisationUnitName.Text = "Organisation Unit Name";
            // 
            // labelRequestOrganisationName
            // 
            this.labelRequestOrganisationName.AutoSize = true;
            this.labelRequestOrganisationName.Location = new System.Drawing.Point(6, 111);
            this.labelRequestOrganisationName.Name = "labelRequestOrganisationName";
            this.labelRequestOrganisationName.Size = new System.Drawing.Size(97, 13);
            this.labelRequestOrganisationName.TabIndex = 25;
            this.labelRequestOrganisationName.Text = "Organisation Name";
            // 
            // labelRequestLocationName
            // 
            this.labelRequestLocationName.AutoSize = true;
            this.labelRequestLocationName.Location = new System.Drawing.Point(6, 85);
            this.labelRequestLocationName.Name = "labelRequestLocationName";
            this.labelRequestLocationName.Size = new System.Drawing.Size(79, 13);
            this.labelRequestLocationName.TabIndex = 24;
            this.labelRequestLocationName.Text = "Location Name";
            // 
            // labelRequestState
            // 
            this.labelRequestState.AutoSize = true;
            this.labelRequestState.Location = new System.Drawing.Point(6, 59);
            this.labelRequestState.Name = "labelRequestState";
            this.labelRequestState.Size = new System.Drawing.Size(32, 13);
            this.labelRequestState.TabIndex = 23;
            this.labelRequestState.Text = "State";
            // 
            // labelRequestCountryName
            // 
            this.labelRequestCountryName.AutoSize = true;
            this.labelRequestCountryName.Location = new System.Drawing.Point(6, 33);
            this.labelRequestCountryName.Name = "labelRequestCountryName";
            this.labelRequestCountryName.Size = new System.Drawing.Size(74, 13);
            this.labelRequestCountryName.TabIndex = 22;
            this.labelRequestCountryName.Text = "Country Name";
            // 
            // textRequestEmailAddress
            // 
            this.textRequestEmailAddress.Location = new System.Drawing.Point(160, 186);
            this.textRequestEmailAddress.Name = "textRequestEmailAddress";
            this.textRequestEmailAddress.Size = new System.Drawing.Size(210, 20);
            this.textRequestEmailAddress.TabIndex = 7;
            this.textRequestEmailAddress.TextChanged += new System.EventHandler(this.textRequestEmailAddress_TextChanged);
            // 
            // textRequestCommonName
            // 
            this.textRequestCommonName.Location = new System.Drawing.Point(160, 160);
            this.textRequestCommonName.Name = "textRequestCommonName";
            this.textRequestCommonName.Size = new System.Drawing.Size(245, 20);
            this.textRequestCommonName.TabIndex = 6;
            this.textRequestCommonName.TextChanged += new System.EventHandler(this.textRequestCommonName_TextChanged);
            // 
            // textRequestOrganisationUnitName
            // 
            this.textRequestOrganisationUnitName.Location = new System.Drawing.Point(160, 134);
            this.textRequestOrganisationUnitName.Name = "textRequestOrganisationUnitName";
            this.textRequestOrganisationUnitName.Size = new System.Drawing.Size(153, 20);
            this.textRequestOrganisationUnitName.TabIndex = 5;
            this.textRequestOrganisationUnitName.TextChanged += new System.EventHandler(this.textRequestOrganisationUnitName_TextChanged);
            // 
            // textRequestOrganisationName
            // 
            this.textRequestOrganisationName.Location = new System.Drawing.Point(160, 108);
            this.textRequestOrganisationName.Name = "textRequestOrganisationName";
            this.textRequestOrganisationName.Size = new System.Drawing.Size(180, 20);
            this.textRequestOrganisationName.TabIndex = 4;
            this.textRequestOrganisationName.TextChanged += new System.EventHandler(this.textRequestOrganisationName_TextChanged);
            // 
            // textRequestLocationName
            // 
            this.textRequestLocationName.Location = new System.Drawing.Point(160, 82);
            this.textRequestLocationName.Name = "textRequestLocationName";
            this.textRequestLocationName.Size = new System.Drawing.Size(120, 20);
            this.textRequestLocationName.TabIndex = 3;
            this.textRequestLocationName.TextChanged += new System.EventHandler(this.textRequestLocationName_TextChanged);
            // 
            // textRequestState
            // 
            this.textRequestState.Location = new System.Drawing.Point(160, 56);
            this.textRequestState.Name = "textRequestState";
            this.textRequestState.Size = new System.Drawing.Size(82, 20);
            this.textRequestState.TabIndex = 2;
            this.textRequestState.TextChanged += new System.EventHandler(this.textRequestState_TextChanged);
            // 
            // textRequestCountryName
            // 
            this.textRequestCountryName.Location = new System.Drawing.Point(160, 30);
            this.textRequestCountryName.Name = "textRequestCountryName";
            this.textRequestCountryName.Size = new System.Drawing.Size(40, 20);
            this.textRequestCountryName.TabIndex = 1;
            this.textRequestCountryName.TextChanged += new System.EventHandler(this.textRequestCountryName_TextChanged);
            // 
            // tabPageCreateCA
            // 
            this.tabPageCreateCA.Controls.Add(this.groupBoxCreateCA);
            this.tabPageCreateCA.Location = new System.Drawing.Point(4, 40);
            this.tabPageCreateCA.Name = "tabPageCreateCA";
            this.tabPageCreateCA.Size = new System.Drawing.Size(673, 449);
            this.tabPageCreateCA.TabIndex = 3;
            this.tabPageCreateCA.Text = "Create Certificate Authority";
            this.tabPageCreateCA.UseVisualStyleBackColor = true;
            // 
            // groupBoxCreateCA
            // 
            this.groupBoxCreateCA.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxCreateCA.Controls.Add(this.labelCADays);
            this.groupBoxCreateCA.Controls.Add(this.textCADays);
            this.groupBoxCreateCA.Controls.Add(this.btnCACreateCertificateAuthority);
            this.groupBoxCreateCA.Controls.Add(this.btnCASelectCertificatePrivateKeyPath);
            this.groupBoxCreateCA.Controls.Add(this.textCACertificatePrivateKeyPath);
            this.groupBoxCreateCA.Controls.Add(this.labelCACertificatePrivateKeyPath);
            this.groupBoxCreateCA.Controls.Add(this.labelCACertificatePath);
            this.groupBoxCreateCA.Controls.Add(this.btnCASelectCertificatePath);
            this.groupBoxCreateCA.Controls.Add(this.textCACertificatePath);
            this.groupBoxCreateCA.Controls.Add(this.labelCACertificatePassword);
            this.groupBoxCreateCA.Controls.Add(this.textCACertificatePassword);
            this.groupBoxCreateCA.Controls.Add(this.labelCAEmailAddress);
            this.groupBoxCreateCA.Controls.Add(this.labelCACommonName);
            this.groupBoxCreateCA.Controls.Add(this.labelCAOrganisationUnitName);
            this.groupBoxCreateCA.Controls.Add(this.labelCAOrganisationName);
            this.groupBoxCreateCA.Controls.Add(this.labelCALocationName);
            this.groupBoxCreateCA.Controls.Add(this.labelCAState);
            this.groupBoxCreateCA.Controls.Add(this.labelCACountryName);
            this.groupBoxCreateCA.Controls.Add(this.textCAEmailAddress);
            this.groupBoxCreateCA.Controls.Add(this.textCACommonName);
            this.groupBoxCreateCA.Controls.Add(this.textCAOrganisationUnitName);
            this.groupBoxCreateCA.Controls.Add(this.textCAOrganisationName);
            this.groupBoxCreateCA.Controls.Add(this.textCALocationName);
            this.groupBoxCreateCA.Controls.Add(this.textCAState);
            this.groupBoxCreateCA.Controls.Add(this.textCACountryName);
            this.groupBoxCreateCA.Location = new System.Drawing.Point(3, 3);
            this.groupBoxCreateCA.Name = "groupBoxCreateCA";
            this.groupBoxCreateCA.Size = new System.Drawing.Size(667, 443);
            this.groupBoxCreateCA.TabIndex = 0;
            this.groupBoxCreateCA.TabStop = false;
            this.groupBoxCreateCA.Text = "Certificate";
            // 
            // labelCADays
            // 
            this.labelCADays.AutoSize = true;
            this.labelCADays.Location = new System.Drawing.Point(7, 326);
            this.labelCADays.Name = "labelCADays";
            this.labelCADays.Size = new System.Drawing.Size(109, 13);
            this.labelCADays.TabIndex = 56;
            this.labelCADays.Text = "Number of Valid Days";
            // 
            // textCADays
            // 
            this.textCADays.Location = new System.Drawing.Point(160, 323);
            this.textCADays.Name = "textCADays";
            this.textCADays.Size = new System.Drawing.Size(82, 20);
            this.textCADays.TabIndex = 55;
            this.textCADays.Text = "7300";
            this.textCADays.TextChanged += new System.EventHandler(this.textCADays_TextChanged);
            this.textCADays.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textCADays_KeyDown);
            // 
            // textCACertificatePrivateKeyPath
            // 
            this.textCACertificatePrivateKeyPath.Location = new System.Drawing.Point(160, 297);
            this.textCACertificatePrivateKeyPath.Name = "textCACertificatePrivateKeyPath";
            this.textCACertificatePrivateKeyPath.ReadOnly = true;
            this.textCACertificatePrivateKeyPath.Size = new System.Drawing.Size(275, 20);
            this.textCACertificatePrivateKeyPath.TabIndex = 11;
            this.textCACertificatePrivateKeyPath.TextChanged += new System.EventHandler(this.textCACertificatePrivateKeyPath_TextChanged);
            // 
            // labelCACertificatePrivateKeyPath
            // 
            this.labelCACertificatePrivateKeyPath.AutoSize = true;
            this.labelCACertificatePrivateKeyPath.Location = new System.Drawing.Point(7, 300);
            this.labelCACertificatePrivateKeyPath.Name = "labelCACertificatePrivateKeyPath";
            this.labelCACertificatePrivateKeyPath.Size = new System.Drawing.Size(155, 13);
            this.labelCACertificatePrivateKeyPath.TabIndex = 54;
            this.labelCACertificatePrivateKeyPath.Text = "Certificate Private Key File Path";
            // 
            // labelCACertificatePath
            // 
            this.labelCACertificatePath.AutoSize = true;
            this.labelCACertificatePath.Location = new System.Drawing.Point(7, 274);
            this.labelCACertificatePath.Name = "labelCACertificatePath";
            this.labelCACertificatePath.Size = new System.Drawing.Size(98, 13);
            this.labelCACertificatePath.TabIndex = 53;
            this.labelCACertificatePath.Text = "Certificate File Path";
            // 
            // textCACertificatePath
            // 
            this.textCACertificatePath.Location = new System.Drawing.Point(160, 271);
            this.textCACertificatePath.Name = "textCACertificatePath";
            this.textCACertificatePath.ReadOnly = true;
            this.textCACertificatePath.Size = new System.Drawing.Size(275, 20);
            this.textCACertificatePath.TabIndex = 9;
            this.textCACertificatePath.TextChanged += new System.EventHandler(this.textCACertificatePath_TextChanged);
            // 
            // labelCACertificatePassword
            // 
            this.labelCACertificatePassword.AutoSize = true;
            this.labelCACertificatePassword.Location = new System.Drawing.Point(6, 248);
            this.labelCACertificatePassword.Name = "labelCACertificatePassword";
            this.labelCACertificatePassword.Size = new System.Drawing.Size(103, 13);
            this.labelCACertificatePassword.TabIndex = 52;
            this.labelCACertificatePassword.Text = "Certificate Password";
            // 
            // textCACertificatePassword
            // 
            this.textCACertificatePassword.Location = new System.Drawing.Point(160, 245);
            this.textCACertificatePassword.Name = "textCACertificatePassword";
            this.textCACertificatePassword.PasswordChar = '*';
            this.textCACertificatePassword.Size = new System.Drawing.Size(153, 20);
            this.textCACertificatePassword.TabIndex = 8;
            this.textCACertificatePassword.TextChanged += new System.EventHandler(this.textCACertificatePassword_TextChanged);
            // 
            // labelCAEmailAddress
            // 
            this.labelCAEmailAddress.AutoSize = true;
            this.labelCAEmailAddress.Location = new System.Drawing.Point(7, 189);
            this.labelCAEmailAddress.Name = "labelCAEmailAddress";
            this.labelCAEmailAddress.Size = new System.Drawing.Size(73, 13);
            this.labelCAEmailAddress.TabIndex = 51;
            this.labelCAEmailAddress.Text = "Email Address";
            // 
            // labelCACommonName
            // 
            this.labelCACommonName.AutoSize = true;
            this.labelCACommonName.Location = new System.Drawing.Point(6, 163);
            this.labelCACommonName.Name = "labelCACommonName";
            this.labelCACommonName.Size = new System.Drawing.Size(79, 13);
            this.labelCACommonName.TabIndex = 50;
            this.labelCACommonName.Text = "Common Name";
            // 
            // labelCAOrganisationUnitName
            // 
            this.labelCAOrganisationUnitName.AutoSize = true;
            this.labelCAOrganisationUnitName.Location = new System.Drawing.Point(6, 137);
            this.labelCAOrganisationUnitName.Name = "labelCAOrganisationUnitName";
            this.labelCAOrganisationUnitName.Size = new System.Drawing.Size(119, 13);
            this.labelCAOrganisationUnitName.TabIndex = 49;
            this.labelCAOrganisationUnitName.Text = "Organisation Unit Name";
            // 
            // labelCAOrganisationName
            // 
            this.labelCAOrganisationName.AutoSize = true;
            this.labelCAOrganisationName.Location = new System.Drawing.Point(6, 111);
            this.labelCAOrganisationName.Name = "labelCAOrganisationName";
            this.labelCAOrganisationName.Size = new System.Drawing.Size(97, 13);
            this.labelCAOrganisationName.TabIndex = 48;
            this.labelCAOrganisationName.Text = "Organisation Name";
            // 
            // labelCALocationName
            // 
            this.labelCALocationName.AutoSize = true;
            this.labelCALocationName.Location = new System.Drawing.Point(6, 85);
            this.labelCALocationName.Name = "labelCALocationName";
            this.labelCALocationName.Size = new System.Drawing.Size(79, 13);
            this.labelCALocationName.TabIndex = 47;
            this.labelCALocationName.Text = "Location Name";
            // 
            // labelCAState
            // 
            this.labelCAState.AutoSize = true;
            this.labelCAState.Location = new System.Drawing.Point(6, 59);
            this.labelCAState.Name = "labelCAState";
            this.labelCAState.Size = new System.Drawing.Size(32, 13);
            this.labelCAState.TabIndex = 46;
            this.labelCAState.Text = "State";
            // 
            // labelCACountryName
            // 
            this.labelCACountryName.AutoSize = true;
            this.labelCACountryName.Location = new System.Drawing.Point(6, 33);
            this.labelCACountryName.Name = "labelCACountryName";
            this.labelCACountryName.Size = new System.Drawing.Size(74, 13);
            this.labelCACountryName.TabIndex = 45;
            this.labelCACountryName.Text = "Country Name";
            // 
            // textCAEmailAddress
            // 
            this.textCAEmailAddress.Location = new System.Drawing.Point(160, 186);
            this.textCAEmailAddress.Name = "textCAEmailAddress";
            this.textCAEmailAddress.Size = new System.Drawing.Size(210, 20);
            this.textCAEmailAddress.TabIndex = 7;
            this.textCAEmailAddress.TextChanged += new System.EventHandler(this.textCAEmailAddress_TextChanged);
            // 
            // textCACommonName
            // 
            this.textCACommonName.Location = new System.Drawing.Point(160, 160);
            this.textCACommonName.Name = "textCACommonName";
            this.textCACommonName.Size = new System.Drawing.Size(245, 20);
            this.textCACommonName.TabIndex = 6;
            this.textCACommonName.TextChanged += new System.EventHandler(this.textCACommonName_TextChanged);
            // 
            // textCAOrganisationUnitName
            // 
            this.textCAOrganisationUnitName.Location = new System.Drawing.Point(160, 134);
            this.textCAOrganisationUnitName.Name = "textCAOrganisationUnitName";
            this.textCAOrganisationUnitName.Size = new System.Drawing.Size(153, 20);
            this.textCAOrganisationUnitName.TabIndex = 5;
            this.textCAOrganisationUnitName.TextChanged += new System.EventHandler(this.textCAOrganisationUnitName_TextChanged);
            // 
            // textCAOrganisationName
            // 
            this.textCAOrganisationName.Location = new System.Drawing.Point(160, 108);
            this.textCAOrganisationName.Name = "textCAOrganisationName";
            this.textCAOrganisationName.Size = new System.Drawing.Size(180, 20);
            this.textCAOrganisationName.TabIndex = 4;
            this.textCAOrganisationName.TextChanged += new System.EventHandler(this.textCAOrganisationName_TextChanged);
            // 
            // textCALocationName
            // 
            this.textCALocationName.Location = new System.Drawing.Point(160, 82);
            this.textCALocationName.Name = "textCALocationName";
            this.textCALocationName.Size = new System.Drawing.Size(120, 20);
            this.textCALocationName.TabIndex = 3;
            this.textCALocationName.TextChanged += new System.EventHandler(this.textCALocationName_TextChanged);
            // 
            // textCAState
            // 
            this.textCAState.Location = new System.Drawing.Point(160, 56);
            this.textCAState.Name = "textCAState";
            this.textCAState.Size = new System.Drawing.Size(82, 20);
            this.textCAState.TabIndex = 2;
            this.textCAState.TextChanged += new System.EventHandler(this.textCAState_TextChanged);
            // 
            // textCACountryName
            // 
            this.textCACountryName.Location = new System.Drawing.Point(160, 30);
            this.textCACountryName.Name = "textCACountryName";
            this.textCACountryName.Size = new System.Drawing.Size(40, 20);
            this.textCACountryName.TabIndex = 1;
            this.textCACountryName.TextChanged += new System.EventHandler(this.textCACountryName_TextChanged);
            // 
            // tabPageRemovePassword
            // 
            this.tabPageRemovePassword.Controls.Add(this.groupBoxRemovePassword);
            this.tabPageRemovePassword.Location = new System.Drawing.Point(4, 40);
            this.tabPageRemovePassword.Name = "tabPageRemovePassword";
            this.tabPageRemovePassword.Size = new System.Drawing.Size(673, 449);
            this.tabPageRemovePassword.TabIndex = 4;
            this.tabPageRemovePassword.Text = "Remove Certificate Password";
            this.tabPageRemovePassword.UseVisualStyleBackColor = true;
            // 
            // groupBoxRemovePassword
            // 
            this.groupBoxRemovePassword.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxRemovePassword.Controls.Add(this.btnRemovePassword);
            this.groupBoxRemovePassword.Controls.Add(this.btnRemoveCertificateDecryptedPath);
            this.groupBoxRemovePassword.Controls.Add(this.textRemoveCertificateDecryptedPath);
            this.groupBoxRemovePassword.Controls.Add(this.labelRemoveCertificateDecryptedPath);
            this.groupBoxRemovePassword.Controls.Add(this.labelRemoveCertificatePath);
            this.groupBoxRemovePassword.Controls.Add(this.btnRemoveCertificatePath);
            this.groupBoxRemovePassword.Controls.Add(this.textRemoveCertificatePath);
            this.groupBoxRemovePassword.Controls.Add(this.labelRemoveCertificatePassword);
            this.groupBoxRemovePassword.Controls.Add(this.textRemoveCertificatePassword);
            this.groupBoxRemovePassword.Location = new System.Drawing.Point(3, 3);
            this.groupBoxRemovePassword.Name = "groupBoxRemovePassword";
            this.groupBoxRemovePassword.Size = new System.Drawing.Size(667, 443);
            this.groupBoxRemovePassword.TabIndex = 0;
            this.groupBoxRemovePassword.TabStop = false;
            this.groupBoxRemovePassword.Text = "Certificate";
            // 
            // textRemoveCertificateDecryptedPath
            // 
            this.textRemoveCertificateDecryptedPath.Location = new System.Drawing.Point(160, 82);
            this.textRemoveCertificateDecryptedPath.Name = "textRemoveCertificateDecryptedPath";
            this.textRemoveCertificateDecryptedPath.ReadOnly = true;
            this.textRemoveCertificateDecryptedPath.Size = new System.Drawing.Size(275, 20);
            this.textRemoveCertificateDecryptedPath.TabIndex = 4;
            this.textRemoveCertificateDecryptedPath.TextChanged += new System.EventHandler(this.textRemoveCertificateDecryptedPath_TextChanged);
            // 
            // labelRemoveCertificateDecryptedPath
            // 
            this.labelRemoveCertificateDecryptedPath.AutoSize = true;
            this.labelRemoveCertificateDecryptedPath.Location = new System.Drawing.Point(7, 85);
            this.labelRemoveCertificateDecryptedPath.Name = "labelRemoveCertificateDecryptedPath";
            this.labelRemoveCertificateDecryptedPath.Size = new System.Drawing.Size(150, 13);
            this.labelRemoveCertificateDecryptedPath.TabIndex = 62;
            this.labelRemoveCertificateDecryptedPath.Text = "Certificate Decrypted File Path";
            // 
            // labelRemoveCertificatePath
            // 
            this.labelRemoveCertificatePath.AutoSize = true;
            this.labelRemoveCertificatePath.Location = new System.Drawing.Point(7, 59);
            this.labelRemoveCertificatePath.Name = "labelRemoveCertificatePath";
            this.labelRemoveCertificatePath.Size = new System.Drawing.Size(98, 13);
            this.labelRemoveCertificatePath.TabIndex = 61;
            this.labelRemoveCertificatePath.Text = "Certificate File Path";
            // 
            // textRemoveCertificatePath
            // 
            this.textRemoveCertificatePath.Location = new System.Drawing.Point(160, 56);
            this.textRemoveCertificatePath.Name = "textRemoveCertificatePath";
            this.textRemoveCertificatePath.ReadOnly = true;
            this.textRemoveCertificatePath.Size = new System.Drawing.Size(275, 20);
            this.textRemoveCertificatePath.TabIndex = 2;
            this.textRemoveCertificatePath.TextChanged += new System.EventHandler(this.textRemoveCertificatePath_TextChanged);
            // 
            // labelRemoveCertificatePassword
            // 
            this.labelRemoveCertificatePassword.AutoSize = true;
            this.labelRemoveCertificatePassword.Location = new System.Drawing.Point(6, 33);
            this.labelRemoveCertificatePassword.Name = "labelRemoveCertificatePassword";
            this.labelRemoveCertificatePassword.Size = new System.Drawing.Size(103, 13);
            this.labelRemoveCertificatePassword.TabIndex = 60;
            this.labelRemoveCertificatePassword.Text = "Certificate Password";
            // 
            // textRemoveCertificatePassword
            // 
            this.textRemoveCertificatePassword.Location = new System.Drawing.Point(160, 30);
            this.textRemoveCertificatePassword.Name = "textRemoveCertificatePassword";
            this.textRemoveCertificatePassword.PasswordChar = '*';
            this.textRemoveCertificatePassword.Size = new System.Drawing.Size(153, 20);
            this.textRemoveCertificatePassword.TabIndex = 1;
            this.textRemoveCertificatePassword.TextChanged += new System.EventHandler(this.textRemoveCertificatePassword_TextChanged);
            // 
            // tabPageCertificateDetails
            // 
            this.tabPageCertificateDetails.Controls.Add(this.groupBoxCertificateDetails);
            this.tabPageCertificateDetails.Location = new System.Drawing.Point(4, 40);
            this.tabPageCertificateDetails.Name = "tabPageCertificateDetails";
            this.tabPageCertificateDetails.Size = new System.Drawing.Size(673, 449);
            this.tabPageCertificateDetails.TabIndex = 5;
            this.tabPageCertificateDetails.Text = "Certificate Details";
            this.tabPageCertificateDetails.UseVisualStyleBackColor = true;
            // 
            // groupBoxCertificateDetails
            // 
            this.groupBoxCertificateDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxCertificateDetails.Controls.Add(this.btnCertificateDetailsView);
            this.groupBoxCertificateDetails.Controls.Add(this.lblCertificateDetailsPassword);
            this.groupBoxCertificateDetails.Controls.Add(this.lblCertificateDetailsPath);
            this.groupBoxCertificateDetails.Controls.Add(this.btnCertificateDetailsLoad);
            this.groupBoxCertificateDetails.Controls.Add(this.btnCertificateDetailsPath);
            this.groupBoxCertificateDetails.Controls.Add(this.txtCertificateDetailsPassword);
            this.groupBoxCertificateDetails.Controls.Add(this.txtCertificateDetailsPath);
            this.groupBoxCertificateDetails.Controls.Add(this.richTextBoxCertificateDetails);
            this.groupBoxCertificateDetails.Location = new System.Drawing.Point(3, 3);
            this.groupBoxCertificateDetails.Name = "groupBoxCertificateDetails";
            this.groupBoxCertificateDetails.Size = new System.Drawing.Size(667, 443);
            this.groupBoxCertificateDetails.TabIndex = 0;
            this.groupBoxCertificateDetails.TabStop = false;
            this.groupBoxCertificateDetails.Text = "Certificate";
            // 
            // btnCertificateDetailsView
            // 
            this.btnCertificateDetailsView.Enabled = false;
            this.btnCertificateDetailsView.Location = new System.Drawing.Point(461, 46);
            this.btnCertificateDetailsView.Name = "btnCertificateDetailsView";
            this.btnCertificateDetailsView.Size = new System.Drawing.Size(75, 23);
            this.btnCertificateDetailsView.TabIndex = 7;
            this.btnCertificateDetailsView.Text = "View";
            this.btnCertificateDetailsView.UseVisualStyleBackColor = true;
            this.btnCertificateDetailsView.Click += new System.EventHandler(this.btnCertificateDetailsView_Click);
            // 
            // lblCertificateDetailsPassword
            // 
            this.lblCertificateDetailsPassword.AutoSize = true;
            this.lblCertificateDetailsPassword.Location = new System.Drawing.Point(6, 52);
            this.lblCertificateDetailsPassword.Name = "lblCertificateDetailsPassword";
            this.lblCertificateDetailsPassword.Size = new System.Drawing.Size(103, 13);
            this.lblCertificateDetailsPassword.TabIndex = 6;
            this.lblCertificateDetailsPassword.Text = "Certificate Password";
            // 
            // lblCertificateDetailsPath
            // 
            this.lblCertificateDetailsPath.AutoSize = true;
            this.lblCertificateDetailsPath.Location = new System.Drawing.Point(6, 26);
            this.lblCertificateDetailsPath.Name = "lblCertificateDetailsPath";
            this.lblCertificateDetailsPath.Size = new System.Drawing.Size(79, 13);
            this.lblCertificateDetailsPath.TabIndex = 5;
            this.lblCertificateDetailsPath.Text = "Certificate Path";
            // 
            // btnCertificateDetailsLoad
            // 
            this.btnCertificateDetailsLoad.Enabled = false;
            this.btnCertificateDetailsLoad.Location = new System.Drawing.Point(380, 46);
            this.btnCertificateDetailsLoad.Name = "btnCertificateDetailsLoad";
            this.btnCertificateDetailsLoad.Size = new System.Drawing.Size(75, 23);
            this.btnCertificateDetailsLoad.TabIndex = 4;
            this.btnCertificateDetailsLoad.Text = "Load";
            this.btnCertificateDetailsLoad.UseVisualStyleBackColor = true;
            this.btnCertificateDetailsLoad.Click += new System.EventHandler(this.btnCertificateDetailsLoad_Click);
            // 
            // btnCertificateDetailsPath
            // 
            this.btnCertificateDetailsPath.Location = new System.Drawing.Point(428, 20);
            this.btnCertificateDetailsPath.Name = "btnCertificateDetailsPath";
            this.btnCertificateDetailsPath.Size = new System.Drawing.Size(27, 23);
            this.btnCertificateDetailsPath.TabIndex = 3;
            this.btnCertificateDetailsPath.Text = "....";
            this.btnCertificateDetailsPath.UseVisualStyleBackColor = true;
            this.btnCertificateDetailsPath.Click += new System.EventHandler(this.btnCertificateDetailsPath_Click);
            // 
            // txtCertificateDetailsPassword
            // 
            this.txtCertificateDetailsPassword.Location = new System.Drawing.Point(115, 49);
            this.txtCertificateDetailsPassword.Name = "txtCertificateDetailsPassword";
            this.txtCertificateDetailsPassword.PasswordChar = '*';
            this.txtCertificateDetailsPassword.Size = new System.Drawing.Size(168, 20);
            this.txtCertificateDetailsPassword.TabIndex = 2;
            // 
            // txtCertificateDetailsPath
            // 
            this.txtCertificateDetailsPath.Location = new System.Drawing.Point(115, 22);
            this.txtCertificateDetailsPath.Name = "txtCertificateDetailsPath";
            this.txtCertificateDetailsPath.ReadOnly = true;
            this.txtCertificateDetailsPath.Size = new System.Drawing.Size(307, 20);
            this.txtCertificateDetailsPath.TabIndex = 1;
            this.txtCertificateDetailsPath.TextChanged += new System.EventHandler(this.txtCertificateDetailsPath_TextChanged);
            // 
            // richTextBoxCertificateDetails
            // 
            this.richTextBoxCertificateDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxCertificateDetails.Location = new System.Drawing.Point(6, 86);
            this.richTextBoxCertificateDetails.Name = "richTextBoxCertificateDetails";
            this.richTextBoxCertificateDetails.ReadOnly = true;
            this.richTextBoxCertificateDetails.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.richTextBoxCertificateDetails.Size = new System.Drawing.Size(655, 351);
            this.richTextBoxCertificateDetails.TabIndex = 0;
            this.richTextBoxCertificateDetails.Text = "";
            // 
            // tabPageCertificateCryptograph
            // 
            this.tabPageCertificateCryptograph.Controls.Add(this.groupBoxCertificateCryptography);
            this.tabPageCertificateCryptograph.Location = new System.Drawing.Point(4, 40);
            this.tabPageCertificateCryptograph.Name = "tabPageCertificateCryptograph";
            this.tabPageCertificateCryptograph.Size = new System.Drawing.Size(673, 449);
            this.tabPageCertificateCryptograph.TabIndex = 6;
            this.tabPageCertificateCryptograph.Text = "Certificate Cryptograph";
            this.tabPageCertificateCryptograph.UseVisualStyleBackColor = true;
            // 
            // groupBoxCertificateCryptography
            // 
            this.groupBoxCertificateCryptography.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxCertificateCryptography.Controls.Add(this.rsaCryptography);
            this.groupBoxCertificateCryptography.Location = new System.Drawing.Point(3, 3);
            this.groupBoxCertificateCryptography.Name = "groupBoxCertificateCryptography";
            this.groupBoxCertificateCryptography.Size = new System.Drawing.Size(663, 443);
            this.groupBoxCertificateCryptography.TabIndex = 1;
            this.groupBoxCertificateCryptography.TabStop = false;
            this.groupBoxCertificateCryptography.Text = "Certificate Cryptography";
            // 
            // rsaCryptography
            // 
            this.rsaCryptography.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rsaCryptography.Location = new System.Drawing.Point(6, 19);
            this.rsaCryptography.Name = "rsaCryptography";
            this.rsaCryptography.Size = new System.Drawing.Size(651, 418);
            this.rsaCryptography.TabIndex = 0;
            // 
            // tabPageExtractKeys
            // 
            this.tabPageExtractKeys.Controls.Add(this.groupBoxExtractKeys);
            this.tabPageExtractKeys.Location = new System.Drawing.Point(4, 40);
            this.tabPageExtractKeys.Name = "tabPageExtractKeys";
            this.tabPageExtractKeys.Size = new System.Drawing.Size(673, 449);
            this.tabPageExtractKeys.TabIndex = 7;
            this.tabPageExtractKeys.Text = "Extract Public and Private Keys";
            this.tabPageExtractKeys.UseVisualStyleBackColor = true;
            // 
            // groupBoxExtractKeys
            // 
            this.groupBoxExtractKeys.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxExtractKeys.Controls.Add(this.btnExtractPublicKey);
            this.groupBoxExtractKeys.Controls.Add(this.btnExtractPrivateKey);
            this.groupBoxExtractKeys.Controls.Add(this.btnExtractPrivateKeyPath);
            this.groupBoxExtractKeys.Controls.Add(this.btnExtractPublicKeyPath);
            this.groupBoxExtractKeys.Controls.Add(this.lblExtractPrivateKeyPath);
            this.groupBoxExtractKeys.Controls.Add(this.txtExtractPrivateKeyPath);
            this.groupBoxExtractKeys.Controls.Add(this.txtExtractPublicKeyPath);
            this.groupBoxExtractKeys.Controls.Add(this.lblExtractPublicKeyPath);
            this.groupBoxExtractKeys.Controls.Add(this.lblExtractCertificatePath);
            this.groupBoxExtractKeys.Controls.Add(this.lblExtractExportPassword);
            this.groupBoxExtractKeys.Controls.Add(this.btnExtractCertificatePath);
            this.groupBoxExtractKeys.Controls.Add(this.lblExtractCertificatePassword);
            this.groupBoxExtractKeys.Controls.Add(this.txtExtractCertificatePath);
            this.groupBoxExtractKeys.Controls.Add(this.txtExtractExportPassword);
            this.groupBoxExtractKeys.Controls.Add(this.txtExtractCertificatePassword);
            this.groupBoxExtractKeys.Location = new System.Drawing.Point(3, 3);
            this.groupBoxExtractKeys.Name = "groupBoxExtractKeys";
            this.groupBoxExtractKeys.Size = new System.Drawing.Size(667, 443);
            this.groupBoxExtractKeys.TabIndex = 0;
            this.groupBoxExtractKeys.TabStop = false;
            this.groupBoxExtractKeys.Text = "Extract Public and Private Keys";
            // 
            // btnExtractPublicKey
            // 
            this.btnExtractPublicKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExtractPublicKey.Enabled = false;
            this.btnExtractPublicKey.Location = new System.Drawing.Point(538, 405);
            this.btnExtractPublicKey.Name = "btnExtractPublicKey";
            this.btnExtractPublicKey.Size = new System.Drawing.Size(123, 23);
            this.btnExtractPublicKey.TabIndex = 38;
            this.btnExtractPublicKey.Text = "Extract Public Key";
            this.btnExtractPublicKey.UseVisualStyleBackColor = true;
            this.btnExtractPublicKey.Click += new System.EventHandler(this.btnExtractPublicKey_Click);
            // 
            // btnExtractPrivateKey
            // 
            this.btnExtractPrivateKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExtractPrivateKey.Enabled = false;
            this.btnExtractPrivateKey.Location = new System.Drawing.Point(409, 405);
            this.btnExtractPrivateKey.Name = "btnExtractPrivateKey";
            this.btnExtractPrivateKey.Size = new System.Drawing.Size(123, 23);
            this.btnExtractPrivateKey.TabIndex = 37;
            this.btnExtractPrivateKey.Text = "Extract Private Key";
            this.btnExtractPrivateKey.UseVisualStyleBackColor = true;
            this.btnExtractPrivateKey.Click += new System.EventHandler(this.btnExtractPrivateKey_Click);
            // 
            // btnExtractPrivateKeyPath
            // 
            this.btnExtractPrivateKeyPath.Location = new System.Drawing.Point(441, 132);
            this.btnExtractPrivateKeyPath.Name = "btnExtractPrivateKeyPath";
            this.btnExtractPrivateKeyPath.Size = new System.Drawing.Size(27, 23);
            this.btnExtractPrivateKeyPath.TabIndex = 36;
            this.btnExtractPrivateKeyPath.Text = "...";
            this.btnExtractPrivateKeyPath.UseVisualStyleBackColor = true;
            this.btnExtractPrivateKeyPath.Click += new System.EventHandler(this.btnExtractPrivateKeyPath_Click);
            // 
            // btnExtractPublicKeyPath
            // 
            this.btnExtractPublicKeyPath.Location = new System.Drawing.Point(441, 106);
            this.btnExtractPublicKeyPath.Name = "btnExtractPublicKeyPath";
            this.btnExtractPublicKeyPath.Size = new System.Drawing.Size(27, 23);
            this.btnExtractPublicKeyPath.TabIndex = 35;
            this.btnExtractPublicKeyPath.Text = "...";
            this.btnExtractPublicKeyPath.UseVisualStyleBackColor = true;
            this.btnExtractPublicKeyPath.Click += new System.EventHandler(this.btnExtractPublicKeyPath_Click);
            // 
            // lblExtractPrivateKeyPath
            // 
            this.lblExtractPrivateKeyPath.AutoSize = true;
            this.lblExtractPrivateKeyPath.Location = new System.Drawing.Point(6, 137);
            this.lblExtractPrivateKeyPath.Name = "lblExtractPrivateKeyPath";
            this.lblExtractPrivateKeyPath.Size = new System.Drawing.Size(105, 13);
            this.lblExtractPrivateKeyPath.TabIndex = 34;
            this.lblExtractPrivateKeyPath.Text = "Private Key File Path";
            // 
            // txtExtractPrivateKeyPath
            // 
            this.txtExtractPrivateKeyPath.Location = new System.Drawing.Point(160, 134);
            this.txtExtractPrivateKeyPath.Name = "txtExtractPrivateKeyPath";
            this.txtExtractPrivateKeyPath.ReadOnly = true;
            this.txtExtractPrivateKeyPath.Size = new System.Drawing.Size(275, 20);
            this.txtExtractPrivateKeyPath.TabIndex = 33;
            this.txtExtractPrivateKeyPath.TextChanged += new System.EventHandler(this.txtExtractPrivateKeyPath_TextChanged);
            // 
            // txtExtractPublicKeyPath
            // 
            this.txtExtractPublicKeyPath.Location = new System.Drawing.Point(160, 108);
            this.txtExtractPublicKeyPath.Name = "txtExtractPublicKeyPath";
            this.txtExtractPublicKeyPath.ReadOnly = true;
            this.txtExtractPublicKeyPath.Size = new System.Drawing.Size(275, 20);
            this.txtExtractPublicKeyPath.TabIndex = 32;
            this.txtExtractPublicKeyPath.TextChanged += new System.EventHandler(this.txtExtractPublicKeyPath_TextChanged);
            // 
            // lblExtractPublicKeyPath
            // 
            this.lblExtractPublicKeyPath.AutoSize = true;
            this.lblExtractPublicKeyPath.Location = new System.Drawing.Point(6, 111);
            this.lblExtractPublicKeyPath.Name = "lblExtractPublicKeyPath";
            this.lblExtractPublicKeyPath.Size = new System.Drawing.Size(101, 13);
            this.lblExtractPublicKeyPath.TabIndex = 31;
            this.lblExtractPublicKeyPath.Text = "Public Key File Path";
            // 
            // lblExtractCertificatePath
            // 
            this.lblExtractCertificatePath.AutoSize = true;
            this.lblExtractCertificatePath.Location = new System.Drawing.Point(6, 85);
            this.lblExtractCertificatePath.Name = "lblExtractCertificatePath";
            this.lblExtractCertificatePath.Size = new System.Drawing.Size(98, 13);
            this.lblExtractCertificatePath.TabIndex = 30;
            this.lblExtractCertificatePath.Text = "Certificate File Path";
            // 
            // lblExtractExportPassword
            // 
            this.lblExtractExportPassword.AutoSize = true;
            this.lblExtractExportPassword.Location = new System.Drawing.Point(6, 59);
            this.lblExtractExportPassword.Name = "lblExtractExportPassword";
            this.lblExtractExportPassword.Size = new System.Drawing.Size(86, 13);
            this.lblExtractExportPassword.TabIndex = 29;
            this.lblExtractExportPassword.Text = "Export Password";
            // 
            // lblExtractCertificatePassword
            // 
            this.lblExtractCertificatePassword.AutoSize = true;
            this.lblExtractCertificatePassword.Location = new System.Drawing.Point(6, 33);
            this.lblExtractCertificatePassword.Name = "lblExtractCertificatePassword";
            this.lblExtractCertificatePassword.Size = new System.Drawing.Size(103, 13);
            this.lblExtractCertificatePassword.TabIndex = 27;
            this.lblExtractCertificatePassword.Text = "Certificate Password";
            // 
            // txtExtractCertificatePath
            // 
            this.txtExtractCertificatePath.Location = new System.Drawing.Point(160, 82);
            this.txtExtractCertificatePath.Name = "txtExtractCertificatePath";
            this.txtExtractCertificatePath.ReadOnly = true;
            this.txtExtractCertificatePath.Size = new System.Drawing.Size(275, 20);
            this.txtExtractCertificatePath.TabIndex = 25;
            this.txtExtractCertificatePath.TextChanged += new System.EventHandler(this.txtExtractCertificatePath_TextChanged);
            // 
            // txtExtractExportPassword
            // 
            this.txtExtractExportPassword.Location = new System.Drawing.Point(160, 56);
            this.txtExtractExportPassword.Name = "txtExtractExportPassword";
            this.txtExtractExportPassword.PasswordChar = '*';
            this.txtExtractExportPassword.Size = new System.Drawing.Size(153, 20);
            this.txtExtractExportPassword.TabIndex = 24;
            this.txtExtractExportPassword.TextChanged += new System.EventHandler(this.txtExtractExportPassword_TextChanged);
            // 
            // txtExtractCertificatePassword
            // 
            this.txtExtractCertificatePassword.Location = new System.Drawing.Point(160, 30);
            this.txtExtractCertificatePassword.Name = "txtExtractCertificatePassword";
            this.txtExtractCertificatePassword.PasswordChar = '*';
            this.txtExtractCertificatePassword.Size = new System.Drawing.Size(153, 20);
            this.txtExtractCertificatePassword.TabIndex = 22;
            this.txtExtractCertificatePassword.TextChanged += new System.EventHandler(this.txtExtractCertificatePassword_TextChanged);
            // 
            // tabPageCertificateRevoke
            // 
            this.tabPageCertificateRevoke.Controls.Add(this.groupBoxRevoke);
            this.tabPageCertificateRevoke.Location = new System.Drawing.Point(4, 40);
            this.tabPageCertificateRevoke.Name = "tabPageCertificateRevoke";
            this.tabPageCertificateRevoke.Size = new System.Drawing.Size(673, 449);
            this.tabPageCertificateRevoke.TabIndex = 8;
            this.tabPageCertificateRevoke.Text = "Certificate Revokation";
            this.tabPageCertificateRevoke.UseVisualStyleBackColor = true;
            // 
            // groupBoxRevoke
            // 
            this.groupBoxRevoke.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxRevoke.Controls.Add(this.btnRevokeUpdateDatabase);
            this.groupBoxRevoke.Controls.Add(this.btnRevokeList);
            this.groupBoxRevoke.Controls.Add(this.btnRevokeCertificate);
            this.groupBoxRevoke.Controls.Add(this.btnRevokeListPath);
            this.groupBoxRevoke.Controls.Add(this.txtRevokeListPath);
            this.groupBoxRevoke.Controls.Add(this.lblRevokeListPath);
            this.groupBoxRevoke.Controls.Add(this.lblRevokeCertificatePath);
            this.groupBoxRevoke.Controls.Add(this.btnRevokeCertificatePath);
            this.groupBoxRevoke.Controls.Add(this.txtRevokeCertificatePath);
            this.groupBoxRevoke.Controls.Add(this.btnRevokeCertificateListPath);
            this.groupBoxRevoke.Controls.Add(this.lblRevokeCertificateListPath);
            this.groupBoxRevoke.Controls.Add(this.txtRevokeCertificateListPath);
            this.groupBoxRevoke.Controls.Add(this.lblRevokeCertificateAuthPassword);
            this.groupBoxRevoke.Controls.Add(this.txtRevokeCertificateAuthPassword);
            this.groupBoxRevoke.Location = new System.Drawing.Point(3, 3);
            this.groupBoxRevoke.Name = "groupBoxRevoke";
            this.groupBoxRevoke.Size = new System.Drawing.Size(667, 443);
            this.groupBoxRevoke.TabIndex = 0;
            this.groupBoxRevoke.TabStop = false;
            this.groupBoxRevoke.Text = "Revokation";
            // 
            // btnRevokeUpdateDatabase
            // 
            this.btnRevokeUpdateDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRevokeUpdateDatabase.Enabled = false;
            this.btnRevokeUpdateDatabase.Location = new System.Drawing.Point(280, 405);
            this.btnRevokeUpdateDatabase.Name = "btnRevokeUpdateDatabase";
            this.btnRevokeUpdateDatabase.Size = new System.Drawing.Size(123, 23);
            this.btnRevokeUpdateDatabase.TabIndex = 41;
            this.btnRevokeUpdateDatabase.Text = "Update Database";
            this.btnRevokeUpdateDatabase.UseVisualStyleBackColor = true;
            this.btnRevokeUpdateDatabase.Click += new System.EventHandler(this.btnRevokeUpdateDatabase_Click);
            // 
            // btnRevokeList
            // 
            this.btnRevokeList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRevokeList.Enabled = false;
            this.btnRevokeList.Location = new System.Drawing.Point(409, 405);
            this.btnRevokeList.Name = "btnRevokeList";
            this.btnRevokeList.Size = new System.Drawing.Size(123, 23);
            this.btnRevokeList.TabIndex = 40;
            this.btnRevokeList.Text = "Revoke List";
            this.btnRevokeList.UseVisualStyleBackColor = true;
            this.btnRevokeList.Click += new System.EventHandler(this.btnRevokeList_Click);
            // 
            // btnRevokeCertificate
            // 
            this.btnRevokeCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRevokeCertificate.Enabled = false;
            this.btnRevokeCertificate.Location = new System.Drawing.Point(538, 405);
            this.btnRevokeCertificate.Name = "btnRevokeCertificate";
            this.btnRevokeCertificate.Size = new System.Drawing.Size(123, 23);
            this.btnRevokeCertificate.TabIndex = 39;
            this.btnRevokeCertificate.Text = "Revoke Certificate";
            this.btnRevokeCertificate.UseVisualStyleBackColor = true;
            this.btnRevokeCertificate.Click += new System.EventHandler(this.btnRevokeCertificate_Click);
            // 
            // btnRevokeListPath
            // 
            this.btnRevokeListPath.Location = new System.Drawing.Point(440, 106);
            this.btnRevokeListPath.Name = "btnRevokeListPath";
            this.btnRevokeListPath.Size = new System.Drawing.Size(27, 23);
            this.btnRevokeListPath.TabIndex = 38;
            this.btnRevokeListPath.Text = "...";
            this.btnRevokeListPath.UseVisualStyleBackColor = true;
            this.btnRevokeListPath.Click += new System.EventHandler(this.btnRevokeListPath_Click);
            // 
            // txtRevokeListPath
            // 
            this.txtRevokeListPath.Location = new System.Drawing.Point(159, 108);
            this.txtRevokeListPath.Name = "txtRevokeListPath";
            this.txtRevokeListPath.ReadOnly = true;
            this.txtRevokeListPath.Size = new System.Drawing.Size(275, 20);
            this.txtRevokeListPath.TabIndex = 37;
            this.txtRevokeListPath.TextChanged += new System.EventHandler(this.txtRevokeListPath_TextChanged);
            // 
            // lblRevokeListPath
            // 
            this.lblRevokeListPath.AutoSize = true;
            this.lblRevokeListPath.Location = new System.Drawing.Point(6, 111);
            this.lblRevokeListPath.Name = "lblRevokeListPath";
            this.lblRevokeListPath.Size = new System.Drawing.Size(108, 13);
            this.lblRevokeListPath.TabIndex = 36;
            this.lblRevokeListPath.Text = "Revoke List File Path";
            // 
            // lblRevokeCertificatePath
            // 
            this.lblRevokeCertificatePath.AutoSize = true;
            this.lblRevokeCertificatePath.Location = new System.Drawing.Point(6, 59);
            this.lblRevokeCertificatePath.Name = "lblRevokeCertificatePath";
            this.lblRevokeCertificatePath.Size = new System.Drawing.Size(98, 13);
            this.lblRevokeCertificatePath.TabIndex = 35;
            this.lblRevokeCertificatePath.Text = "Certificate File Path";
            // 
            // txtRevokeCertificatePath
            // 
            this.txtRevokeCertificatePath.Location = new System.Drawing.Point(159, 56);
            this.txtRevokeCertificatePath.Name = "txtRevokeCertificatePath";
            this.txtRevokeCertificatePath.ReadOnly = true;
            this.txtRevokeCertificatePath.Size = new System.Drawing.Size(275, 20);
            this.txtRevokeCertificatePath.TabIndex = 33;
            this.txtRevokeCertificatePath.TextChanged += new System.EventHandler(this.txtRevokeCertificatePath_TextChanged);
            // 
            // lblRevokeCertificateListPath
            // 
            this.lblRevokeCertificateListPath.AutoSize = true;
            this.lblRevokeCertificateListPath.Location = new System.Drawing.Point(6, 85);
            this.lblRevokeCertificateListPath.Name = "lblRevokeCertificateListPath";
            this.lblRevokeCertificateListPath.Size = new System.Drawing.Size(133, 13);
            this.lblRevokeCertificateListPath.TabIndex = 32;
            this.lblRevokeCertificateListPath.Text = "Revoke Certificate List File";
            // 
            // txtRevokeCertificateListPath
            // 
            this.txtRevokeCertificateListPath.Location = new System.Drawing.Point(159, 82);
            this.txtRevokeCertificateListPath.Name = "txtRevokeCertificateListPath";
            this.txtRevokeCertificateListPath.ReadOnly = true;
            this.txtRevokeCertificateListPath.Size = new System.Drawing.Size(275, 20);
            this.txtRevokeCertificateListPath.TabIndex = 29;
            this.txtRevokeCertificateListPath.TextChanged += new System.EventHandler(this.txtRevokeCertificateListPath_TextChanged);
            // 
            // lblRevokeCertificateAuthPassword
            // 
            this.lblRevokeCertificateAuthPassword.AutoSize = true;
            this.lblRevokeCertificateAuthPassword.Location = new System.Drawing.Point(6, 33);
            this.lblRevokeCertificateAuthPassword.Name = "lblRevokeCertificateAuthPassword";
            this.lblRevokeCertificateAuthPassword.Size = new System.Drawing.Size(147, 13);
            this.lblRevokeCertificateAuthPassword.TabIndex = 31;
            this.lblRevokeCertificateAuthPassword.Text = "Certificate Authority Password";
            // 
            // txtRevokeCertificateAuthPassword
            // 
            this.txtRevokeCertificateAuthPassword.Location = new System.Drawing.Point(159, 30);
            this.txtRevokeCertificateAuthPassword.Name = "txtRevokeCertificateAuthPassword";
            this.txtRevokeCertificateAuthPassword.PasswordChar = '*';
            this.txtRevokeCertificateAuthPassword.Size = new System.Drawing.Size(153, 20);
            this.txtRevokeCertificateAuthPassword.TabIndex = 28;
            this.txtRevokeCertificateAuthPassword.TextChanged += new System.EventHandler(this.txtRevokeCertificateAuthPassword_TextChanged);
            // 
            // tabPageGeneratePublicPrivateKeyPair
            // 
            this.tabPageGeneratePublicPrivateKeyPair.Controls.Add(this.groupBoxGenPublicPrivatePair);
            this.tabPageGeneratePublicPrivateKeyPair.Location = new System.Drawing.Point(4, 40);
            this.tabPageGeneratePublicPrivateKeyPair.Name = "tabPageGeneratePublicPrivateKeyPair";
            this.tabPageGeneratePublicPrivateKeyPair.Size = new System.Drawing.Size(673, 449);
            this.tabPageGeneratePublicPrivateKeyPair.TabIndex = 9;
            this.tabPageGeneratePublicPrivateKeyPair.Text = "Generate Public Private Key Pair";
            this.tabPageGeneratePublicPrivateKeyPair.UseVisualStyleBackColor = true;
            // 
            // groupBoxGenPublicPrivatePair
            // 
            this.groupBoxGenPublicPrivatePair.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxGenPublicPrivatePair.Controls.Add(this.lblGenPublicPrivatePairSizePublicKey);
            this.groupBoxGenPublicPrivatePair.Controls.Add(this.lblGenPublicPrivatePairPrivateKey);
            this.groupBoxGenPublicPrivatePair.Controls.Add(this.lblGenPublicPrivatePairSize);
            this.groupBoxGenPublicPrivatePair.Controls.Add(this.btnGenPublicPrivatePairPublicGo);
            this.groupBoxGenPublicPrivatePair.Controls.Add(this.btnGenPublicPrivatePairPrivateKey);
            this.groupBoxGenPublicPrivatePair.Controls.Add(this.btnGenPublicPrivatePairPublicKey);
            this.groupBoxGenPublicPrivatePair.Controls.Add(this.txtGenPublicPrivatePairSizePublicKey);
            this.groupBoxGenPublicPrivatePair.Controls.Add(this.txtGenPublicPrivatePairPrivateKey);
            this.groupBoxGenPublicPrivatePair.Controls.Add(this.txtGenPublicPrivatePairSize);
            this.groupBoxGenPublicPrivatePair.Location = new System.Drawing.Point(3, 3);
            this.groupBoxGenPublicPrivatePair.Name = "groupBoxGenPublicPrivatePair";
            this.groupBoxGenPublicPrivatePair.Size = new System.Drawing.Size(667, 443);
            this.groupBoxGenPublicPrivatePair.TabIndex = 0;
            this.groupBoxGenPublicPrivatePair.TabStop = false;
            this.groupBoxGenPublicPrivatePair.Text = "Generate Public Private Key Pair";
            // 
            // lblGenPublicPrivatePairSizePublicKey
            // 
            this.lblGenPublicPrivatePairSizePublicKey.AutoSize = true;
            this.lblGenPublicPrivatePairSizePublicKey.Location = new System.Drawing.Point(6, 85);
            this.lblGenPublicPrivatePairSizePublicKey.Name = "lblGenPublicPrivatePairSizePublicKey";
            this.lblGenPublicPrivatePairSizePublicKey.Size = new System.Drawing.Size(101, 13);
            this.lblGenPublicPrivatePairSizePublicKey.TabIndex = 8;
            this.lblGenPublicPrivatePairSizePublicKey.Text = "Public Key File Path";
            // 
            // lblGenPublicPrivatePairPrivateKey
            // 
            this.lblGenPublicPrivatePairPrivateKey.AutoSize = true;
            this.lblGenPublicPrivatePairPrivateKey.Location = new System.Drawing.Point(6, 59);
            this.lblGenPublicPrivatePairPrivateKey.Name = "lblGenPublicPrivatePairPrivateKey";
            this.lblGenPublicPrivatePairPrivateKey.Size = new System.Drawing.Size(105, 13);
            this.lblGenPublicPrivatePairPrivateKey.TabIndex = 7;
            this.lblGenPublicPrivatePairPrivateKey.Text = "Private Key File Path";
            // 
            // lblGenPublicPrivatePairSize
            // 
            this.lblGenPublicPrivatePairSize.AutoSize = true;
            this.lblGenPublicPrivatePairSize.Location = new System.Drawing.Point(6, 33);
            this.lblGenPublicPrivatePairSize.Name = "lblGenPublicPrivatePairSize";
            this.lblGenPublicPrivatePairSize.Size = new System.Drawing.Size(48, 13);
            this.lblGenPublicPrivatePairSize.TabIndex = 6;
            this.lblGenPublicPrivatePairSize.Text = "Key Size";
            // 
            // btnGenPublicPrivatePairPrivateKey
            // 
            this.btnGenPublicPrivatePairPrivateKey.Location = new System.Drawing.Point(440, 54);
            this.btnGenPublicPrivatePairPrivateKey.Name = "btnGenPublicPrivatePairPrivateKey";
            this.btnGenPublicPrivatePairPrivateKey.Size = new System.Drawing.Size(27, 23);
            this.btnGenPublicPrivatePairPrivateKey.TabIndex = 2;
            this.btnGenPublicPrivatePairPrivateKey.Text = "...";
            this.btnGenPublicPrivatePairPrivateKey.UseVisualStyleBackColor = true;
            this.btnGenPublicPrivatePairPrivateKey.Click += new System.EventHandler(this.btnGenPublicPrivatePairPrivateKey_Click);
            // 
            // btnGenPublicPrivatePairPublicKey
            // 
            this.btnGenPublicPrivatePairPublicKey.Location = new System.Drawing.Point(440, 80);
            this.btnGenPublicPrivatePairPublicKey.Name = "btnGenPublicPrivatePairPublicKey";
            this.btnGenPublicPrivatePairPublicKey.Size = new System.Drawing.Size(27, 23);
            this.btnGenPublicPrivatePairPublicKey.TabIndex = 4;
            this.btnGenPublicPrivatePairPublicKey.Text = "...";
            this.btnGenPublicPrivatePairPublicKey.UseVisualStyleBackColor = true;
            this.btnGenPublicPrivatePairPublicKey.Click += new System.EventHandler(this.btnGenPublicPrivatePairPublicKey_Click);
            // 
            // txtGenPublicPrivatePairSizePublicKey
            // 
            this.txtGenPublicPrivatePairSizePublicKey.Location = new System.Drawing.Point(159, 82);
            this.txtGenPublicPrivatePairSizePublicKey.Name = "txtGenPublicPrivatePairSizePublicKey";
            this.txtGenPublicPrivatePairSizePublicKey.ReadOnly = true;
            this.txtGenPublicPrivatePairSizePublicKey.Size = new System.Drawing.Size(275, 20);
            this.txtGenPublicPrivatePairSizePublicKey.TabIndex = 3;
            this.txtGenPublicPrivatePairSizePublicKey.TextChanged += new System.EventHandler(this.txtGenPublicPrivatePairSizePublicKey_TextChanged);
            // 
            // txtGenPublicPrivatePairPrivateKey
            // 
            this.txtGenPublicPrivatePairPrivateKey.Location = new System.Drawing.Point(159, 56);
            this.txtGenPublicPrivatePairPrivateKey.Name = "txtGenPublicPrivatePairPrivateKey";
            this.txtGenPublicPrivatePairPrivateKey.ReadOnly = true;
            this.txtGenPublicPrivatePairPrivateKey.Size = new System.Drawing.Size(275, 20);
            this.txtGenPublicPrivatePairPrivateKey.TabIndex = 1;
            this.txtGenPublicPrivatePairPrivateKey.TextChanged += new System.EventHandler(this.txtGenPublicPrivatePairPrivateKey_TextChanged);
            // 
            // txtGenPublicPrivatePairSize
            // 
            this.txtGenPublicPrivatePairSize.Location = new System.Drawing.Point(159, 30);
            this.txtGenPublicPrivatePairSize.Name = "txtGenPublicPrivatePairSize";
            this.txtGenPublicPrivatePairSize.Size = new System.Drawing.Size(100, 20);
            this.txtGenPublicPrivatePairSize.TabIndex = 0;
            this.txtGenPublicPrivatePairSize.Text = "4096";
            this.txtGenPublicPrivatePairSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtGenPublicPrivatePairSize.TextChanged += new System.EventHandler(this.txtGenPublicPrivatePairSize_TextChanged);
            // 
            // tabPagePersonalCertificate
            // 
            this.tabPagePersonalCertificate.Controls.Add(this.groupBoxPersonalCertificate);
            this.tabPagePersonalCertificate.Location = new System.Drawing.Point(4, 40);
            this.tabPagePersonalCertificate.Name = "tabPagePersonalCertificate";
            this.tabPagePersonalCertificate.Size = new System.Drawing.Size(673, 459);
            this.tabPagePersonalCertificate.TabIndex = 10;
            this.tabPagePersonalCertificate.Text = "Personal Certificate";
            this.tabPagePersonalCertificate.UseVisualStyleBackColor = true;
            // 
            // groupBoxPersonalCertificate
            // 
            this.groupBoxPersonalCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxPersonalCertificate.Controls.Add(this.tabControlPersonalCertificate);
            this.groupBoxPersonalCertificate.Location = new System.Drawing.Point(3, 3);
            this.groupBoxPersonalCertificate.Name = "groupBoxPersonalCertificate";
            this.groupBoxPersonalCertificate.Size = new System.Drawing.Size(664, 453);
            this.groupBoxPersonalCertificate.TabIndex = 0;
            this.groupBoxPersonalCertificate.TabStop = false;
            this.groupBoxPersonalCertificate.Text = "Certificate";
            // 
            // tabControlPersonalCertificate
            // 
            this.tabControlPersonalCertificate.Controls.Add(this.tabPagePersonalSubject);
            this.tabControlPersonalCertificate.Controls.Add(this.tabPagePersonalIssuer);
            this.tabControlPersonalCertificate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlPersonalCertificate.Location = new System.Drawing.Point(3, 16);
            this.tabControlPersonalCertificate.Name = "tabControlPersonalCertificate";
            this.tabControlPersonalCertificate.SelectedIndex = 0;
            this.tabControlPersonalCertificate.Size = new System.Drawing.Size(658, 434);
            this.tabControlPersonalCertificate.TabIndex = 0;
            // 
            // tabPagePersonalSubject
            // 
            this.tabPagePersonalSubject.Controls.Add(this.labelPSSignatureAlgorithm);
            this.tabPagePersonalSubject.Controls.Add(this.psSignatureAlgorithm);
            this.tabPagePersonalSubject.Controls.Add(this.psKeySize);
            this.tabPagePersonalSubject.Controls.Add(this.psValidDays);
            this.tabPagePersonalSubject.Controls.Add(this.labelPSValidDays);
            this.tabPagePersonalSubject.Controls.Add(this.labelPSKeySize);
            this.tabPagePersonalSubject.Controls.Add(this.psPIEPathSelect);
            this.tabPagePersonalSubject.Controls.Add(this.psCertificatePathSelect);
            this.tabPagePersonalSubject.Controls.Add(this.psPIEPath);
            this.tabPagePersonalSubject.Controls.Add(this.psCertificatePath);
            this.tabPagePersonalSubject.Controls.Add(this.psPassword);
            this.tabPagePersonalSubject.Controls.Add(this.labelPSCertificatePassword);
            this.tabPagePersonalSubject.Controls.Add(this.labelPSCertificateFilePath);
            this.tabPagePersonalSubject.Controls.Add(this.labelPSPIEFilePath);
            this.tabPagePersonalSubject.Controls.Add(this.psEmailAddress);
            this.tabPagePersonalSubject.Controls.Add(this.psCommonName);
            this.tabPagePersonalSubject.Controls.Add(this.psOrganisationUnitName);
            this.tabPagePersonalSubject.Controls.Add(this.psOrganisationName);
            this.tabPagePersonalSubject.Controls.Add(this.psLocationName);
            this.tabPagePersonalSubject.Controls.Add(this.psState);
            this.tabPagePersonalSubject.Controls.Add(this.psCountryName);
            this.tabPagePersonalSubject.Controls.Add(this.labelPersonalSubjectEmailAddress);
            this.tabPagePersonalSubject.Controls.Add(this.labelPersonalSubjectCommonName);
            this.tabPagePersonalSubject.Controls.Add(this.labelPersonalSubjectOrganisationUnitName);
            this.tabPagePersonalSubject.Controls.Add(this.labelPersonalSubjectOrganisationName);
            this.tabPagePersonalSubject.Controls.Add(this.labelPersonalSubjectLocationName);
            this.tabPagePersonalSubject.Controls.Add(this.labelPersonalSubjectState);
            this.tabPagePersonalSubject.Controls.Add(this.labelPersonalSubjectCountryName);
            this.tabPagePersonalSubject.Controls.Add(this.psGenCertificate);
            this.tabPagePersonalSubject.Location = new System.Drawing.Point(4, 22);
            this.tabPagePersonalSubject.Name = "tabPagePersonalSubject";
            this.tabPagePersonalSubject.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePersonalSubject.Size = new System.Drawing.Size(650, 408);
            this.tabPagePersonalSubject.TabIndex = 0;
            this.tabPagePersonalSubject.Text = "Subject";
            this.tabPagePersonalSubject.UseVisualStyleBackColor = true;
            // 
            // psKeySize
            // 
            this.psKeySize.Location = new System.Drawing.Point(160, 349);
            this.psKeySize.Name = "psKeySize";
            this.psKeySize.Size = new System.Drawing.Size(100, 20);
            this.psKeySize.TabIndex = 14;
            this.psKeySize.Text = "4096";
            this.psKeySize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.psKeySize.TextChanged += new System.EventHandler(this.psKeySize_TextChanged);
            this.psKeySize.KeyDown += new System.Windows.Forms.KeyEventHandler(this.psKeySize_KeyDown);
            // 
            // psValidDays
            // 
            this.psValidDays.Location = new System.Drawing.Point(160, 323);
            this.psValidDays.Name = "psValidDays";
            this.psValidDays.Size = new System.Drawing.Size(100, 20);
            this.psValidDays.TabIndex = 13;
            this.psValidDays.Text = "1825";
            this.psValidDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.psValidDays.TextChanged += new System.EventHandler(this.psValidDays_TextChanged);
            this.psValidDays.KeyDown += new System.Windows.Forms.KeyEventHandler(this.psValidDays_KeyDown);
            // 
            // labelPSValidDays
            // 
            this.labelPSValidDays.AutoSize = true;
            this.labelPSValidDays.Location = new System.Drawing.Point(6, 326);
            this.labelPSValidDays.Name = "labelPSValidDays";
            this.labelPSValidDays.Size = new System.Drawing.Size(109, 13);
            this.labelPSValidDays.TabIndex = 29;
            this.labelPSValidDays.Text = "Number of Valid Days";
            // 
            // labelPSKeySize
            // 
            this.labelPSKeySize.AutoSize = true;
            this.labelPSKeySize.Location = new System.Drawing.Point(6, 352);
            this.labelPSKeySize.Name = "labelPSKeySize";
            this.labelPSKeySize.Size = new System.Drawing.Size(48, 13);
            this.labelPSKeySize.TabIndex = 28;
            this.labelPSKeySize.Text = "Key Size";
            // 
            // psPIEPath
            // 
            this.psPIEPath.Location = new System.Drawing.Point(160, 297);
            this.psPIEPath.Name = "psPIEPath";
            this.psPIEPath.ReadOnly = true;
            this.psPIEPath.Size = new System.Drawing.Size(275, 20);
            this.psPIEPath.TabIndex = 11;
            this.psPIEPath.TextChanged += new System.EventHandler(this.psPIEPath_TextChanged);
            // 
            // psCertificatePath
            // 
            this.psCertificatePath.Location = new System.Drawing.Point(160, 271);
            this.psCertificatePath.Name = "psCertificatePath";
            this.psCertificatePath.ReadOnly = true;
            this.psCertificatePath.Size = new System.Drawing.Size(275, 20);
            this.psCertificatePath.TabIndex = 9;
            this.psCertificatePath.TextChanged += new System.EventHandler(this.psCertificatePath_TextChanged);
            // 
            // psPassword
            // 
            this.psPassword.Location = new System.Drawing.Point(160, 245);
            this.psPassword.Name = "psPassword";
            this.psPassword.PasswordChar = '*';
            this.psPassword.Size = new System.Drawing.Size(153, 20);
            this.psPassword.TabIndex = 8;
            this.psPassword.TextChanged += new System.EventHandler(this.psPassword_TextChanged);
            // 
            // labelPSCertificatePassword
            // 
            this.labelPSCertificatePassword.AutoSize = true;
            this.labelPSCertificatePassword.Location = new System.Drawing.Point(6, 248);
            this.labelPSCertificatePassword.Name = "labelPSCertificatePassword";
            this.labelPSCertificatePassword.Size = new System.Drawing.Size(103, 13);
            this.labelPSCertificatePassword.TabIndex = 27;
            this.labelPSCertificatePassword.Text = "Certificate Password";
            // 
            // labelPSCertificateFilePath
            // 
            this.labelPSCertificateFilePath.AutoSize = true;
            this.labelPSCertificateFilePath.Location = new System.Drawing.Point(6, 274);
            this.labelPSCertificateFilePath.Name = "labelPSCertificateFilePath";
            this.labelPSCertificateFilePath.Size = new System.Drawing.Size(98, 13);
            this.labelPSCertificateFilePath.TabIndex = 25;
            this.labelPSCertificateFilePath.Text = "Certificate File Path";
            // 
            // labelPSPIEFilePath
            // 
            this.labelPSPIEFilePath.AutoSize = true;
            this.labelPSPIEFilePath.Location = new System.Drawing.Point(6, 300);
            this.labelPSPIEFilePath.Name = "labelPSPIEFilePath";
            this.labelPSPIEFilePath.Size = new System.Drawing.Size(68, 13);
            this.labelPSPIEFilePath.TabIndex = 23;
            this.labelPSPIEFilePath.Text = "PIE File Path";
            // 
            // psEmailAddress
            // 
            this.psEmailAddress.Location = new System.Drawing.Point(160, 186);
            this.psEmailAddress.Name = "psEmailAddress";
            this.psEmailAddress.Size = new System.Drawing.Size(210, 20);
            this.psEmailAddress.TabIndex = 7;
            this.psEmailAddress.TextChanged += new System.EventHandler(this.psEmailAddress_TextChanged);
            // 
            // psCommonName
            // 
            this.psCommonName.Location = new System.Drawing.Point(160, 160);
            this.psCommonName.Name = "psCommonName";
            this.psCommonName.Size = new System.Drawing.Size(245, 20);
            this.psCommonName.TabIndex = 6;
            this.psCommonName.TextChanged += new System.EventHandler(this.psCommonName_TextChanged);
            // 
            // psOrganisationUnitName
            // 
            this.psOrganisationUnitName.Location = new System.Drawing.Point(160, 134);
            this.psOrganisationUnitName.Name = "psOrganisationUnitName";
            this.psOrganisationUnitName.Size = new System.Drawing.Size(153, 20);
            this.psOrganisationUnitName.TabIndex = 5;
            this.psOrganisationUnitName.TextChanged += new System.EventHandler(this.psOrganisationUnitName_TextChanged);
            // 
            // psOrganisationName
            // 
            this.psOrganisationName.Location = new System.Drawing.Point(160, 108);
            this.psOrganisationName.Name = "psOrganisationName";
            this.psOrganisationName.Size = new System.Drawing.Size(180, 20);
            this.psOrganisationName.TabIndex = 4;
            this.psOrganisationName.TextChanged += new System.EventHandler(this.psOrganisationName_TextChanged);
            // 
            // psLocationName
            // 
            this.psLocationName.Location = new System.Drawing.Point(160, 82);
            this.psLocationName.Name = "psLocationName";
            this.psLocationName.Size = new System.Drawing.Size(120, 20);
            this.psLocationName.TabIndex = 3;
            this.psLocationName.TextChanged += new System.EventHandler(this.psLocationName_TextChanged);
            // 
            // psState
            // 
            this.psState.Location = new System.Drawing.Point(160, 56);
            this.psState.Name = "psState";
            this.psState.Size = new System.Drawing.Size(82, 20);
            this.psState.TabIndex = 2;
            this.psState.TextChanged += new System.EventHandler(this.psState_TextChanged);
            // 
            // psCountryName
            // 
            this.psCountryName.Location = new System.Drawing.Point(160, 30);
            this.psCountryName.Name = "psCountryName";
            this.psCountryName.Size = new System.Drawing.Size(40, 20);
            this.psCountryName.TabIndex = 1;
            this.psCountryName.TextChanged += new System.EventHandler(this.psCountryName_TextChanged);
            // 
            // labelPersonalSubjectEmailAddress
            // 
            this.labelPersonalSubjectEmailAddress.AutoSize = true;
            this.labelPersonalSubjectEmailAddress.Location = new System.Drawing.Point(6, 189);
            this.labelPersonalSubjectEmailAddress.Name = "labelPersonalSubjectEmailAddress";
            this.labelPersonalSubjectEmailAddress.Size = new System.Drawing.Size(73, 13);
            this.labelPersonalSubjectEmailAddress.TabIndex = 20;
            this.labelPersonalSubjectEmailAddress.Text = "Email Address";
            // 
            // labelPersonalSubjectCommonName
            // 
            this.labelPersonalSubjectCommonName.AutoSize = true;
            this.labelPersonalSubjectCommonName.Location = new System.Drawing.Point(6, 163);
            this.labelPersonalSubjectCommonName.Name = "labelPersonalSubjectCommonName";
            this.labelPersonalSubjectCommonName.Size = new System.Drawing.Size(79, 13);
            this.labelPersonalSubjectCommonName.TabIndex = 18;
            this.labelPersonalSubjectCommonName.Text = "Common Name";
            // 
            // labelPersonalSubjectOrganisationUnitName
            // 
            this.labelPersonalSubjectOrganisationUnitName.AutoSize = true;
            this.labelPersonalSubjectOrganisationUnitName.Location = new System.Drawing.Point(6, 137);
            this.labelPersonalSubjectOrganisationUnitName.Name = "labelPersonalSubjectOrganisationUnitName";
            this.labelPersonalSubjectOrganisationUnitName.Size = new System.Drawing.Size(119, 13);
            this.labelPersonalSubjectOrganisationUnitName.TabIndex = 16;
            this.labelPersonalSubjectOrganisationUnitName.Text = "Organisation Unit Name";
            // 
            // labelPersonalSubjectOrganisationName
            // 
            this.labelPersonalSubjectOrganisationName.AutoSize = true;
            this.labelPersonalSubjectOrganisationName.Location = new System.Drawing.Point(6, 111);
            this.labelPersonalSubjectOrganisationName.Name = "labelPersonalSubjectOrganisationName";
            this.labelPersonalSubjectOrganisationName.Size = new System.Drawing.Size(97, 13);
            this.labelPersonalSubjectOrganisationName.TabIndex = 14;
            this.labelPersonalSubjectOrganisationName.Text = "Organisation Name";
            // 
            // labelPersonalSubjectLocationName
            // 
            this.labelPersonalSubjectLocationName.AutoSize = true;
            this.labelPersonalSubjectLocationName.Location = new System.Drawing.Point(6, 85);
            this.labelPersonalSubjectLocationName.Name = "labelPersonalSubjectLocationName";
            this.labelPersonalSubjectLocationName.Size = new System.Drawing.Size(79, 13);
            this.labelPersonalSubjectLocationName.TabIndex = 12;
            this.labelPersonalSubjectLocationName.Text = "Location Name";
            // 
            // labelPersonalSubjectState
            // 
            this.labelPersonalSubjectState.AutoSize = true;
            this.labelPersonalSubjectState.Location = new System.Drawing.Point(6, 59);
            this.labelPersonalSubjectState.Name = "labelPersonalSubjectState";
            this.labelPersonalSubjectState.Size = new System.Drawing.Size(32, 13);
            this.labelPersonalSubjectState.TabIndex = 10;
            this.labelPersonalSubjectState.Text = "State";
            // 
            // labelPersonalSubjectCountryName
            // 
            this.labelPersonalSubjectCountryName.AutoSize = true;
            this.labelPersonalSubjectCountryName.Location = new System.Drawing.Point(6, 33);
            this.labelPersonalSubjectCountryName.Name = "labelPersonalSubjectCountryName";
            this.labelPersonalSubjectCountryName.Size = new System.Drawing.Size(74, 13);
            this.labelPersonalSubjectCountryName.TabIndex = 8;
            this.labelPersonalSubjectCountryName.Text = "Country Name";
            // 
            // tabPagePersonalIssuer
            // 
            this.tabPagePersonalIssuer.Controls.Add(this.piSignerCertificateSelect);
            this.tabPagePersonalIssuer.Controls.Add(this.piSignerPrivateKeySelect);
            this.tabPagePersonalIssuer.Controls.Add(this.piCertificateFilePath);
            this.tabPagePersonalIssuer.Controls.Add(this.piSignerPrivateFilePath);
            this.tabPagePersonalIssuer.Controls.Add(this.piPassword);
            this.tabPagePersonalIssuer.Controls.Add(this.labelPersonalIssuerSignerPrivateKeyPassword);
            this.tabPagePersonalIssuer.Controls.Add(this.labelPersonalIssuerSignerPrivateKeyFilePath);
            this.tabPagePersonalIssuer.Controls.Add(this.labelPersonalIssuerSignerCertificateFilePath);
            this.tabPagePersonalIssuer.Controls.Add(this.piEmailAddress);
            this.tabPagePersonalIssuer.Controls.Add(this.piCommonName);
            this.tabPagePersonalIssuer.Controls.Add(this.piOrganidationUnitName);
            this.tabPagePersonalIssuer.Controls.Add(this.piOrganidationName);
            this.tabPagePersonalIssuer.Controls.Add(this.piLocationName);
            this.tabPagePersonalIssuer.Controls.Add(this.piState);
            this.tabPagePersonalIssuer.Controls.Add(this.piCountryName);
            this.tabPagePersonalIssuer.Controls.Add(this.labelPersonalIssuerEmailAddress);
            this.tabPagePersonalIssuer.Controls.Add(this.labelPersonalIssuerCommonName);
            this.tabPagePersonalIssuer.Controls.Add(this.labelPersonalIssuerOrganisationUnitName);
            this.tabPagePersonalIssuer.Controls.Add(this.labelPersonalIssuerOrganisationName);
            this.tabPagePersonalIssuer.Controls.Add(this.labelPersonalIssuerLocationName);
            this.tabPagePersonalIssuer.Controls.Add(this.labelPersonalIssuerState);
            this.tabPagePersonalIssuer.Controls.Add(this.labelPersonalIssuerCountryName);
            this.tabPagePersonalIssuer.Location = new System.Drawing.Point(4, 22);
            this.tabPagePersonalIssuer.Name = "tabPagePersonalIssuer";
            this.tabPagePersonalIssuer.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePersonalIssuer.Size = new System.Drawing.Size(650, 408);
            this.tabPagePersonalIssuer.TabIndex = 1;
            this.tabPagePersonalIssuer.Text = "Issuer";
            this.tabPagePersonalIssuer.UseVisualStyleBackColor = true;
            // 
            // piCertificateFilePath
            // 
            this.piCertificateFilePath.Location = new System.Drawing.Point(160, 297);
            this.piCertificateFilePath.Name = "piCertificateFilePath";
            this.piCertificateFilePath.ReadOnly = true;
            this.piCertificateFilePath.Size = new System.Drawing.Size(275, 20);
            this.piCertificateFilePath.TabIndex = 11;
            this.piCertificateFilePath.TextChanged += new System.EventHandler(this.piCertificateFilePath_TextChanged);
            // 
            // piSignerPrivateFilePath
            // 
            this.piSignerPrivateFilePath.Location = new System.Drawing.Point(160, 271);
            this.piSignerPrivateFilePath.Name = "piSignerPrivateFilePath";
            this.piSignerPrivateFilePath.ReadOnly = true;
            this.piSignerPrivateFilePath.Size = new System.Drawing.Size(275, 20);
            this.piSignerPrivateFilePath.TabIndex = 9;
            this.piSignerPrivateFilePath.TextChanged += new System.EventHandler(this.piSignerPrivateFilePath_TextChanged);
            // 
            // piPassword
            // 
            this.piPassword.Location = new System.Drawing.Point(160, 245);
            this.piPassword.Name = "piPassword";
            this.piPassword.PasswordChar = '*';
            this.piPassword.Size = new System.Drawing.Size(153, 20);
            this.piPassword.TabIndex = 8;
            this.piPassword.TextChanged += new System.EventHandler(this.piPassword_TextChanged);
            // 
            // labelPersonalIssuerSignerPrivateKeyPassword
            // 
            this.labelPersonalIssuerSignerPrivateKeyPassword.AutoSize = true;
            this.labelPersonalIssuerSignerPrivateKeyPassword.Location = new System.Drawing.Point(6, 248);
            this.labelPersonalIssuerSignerPrivateKeyPassword.Name = "labelPersonalIssuerSignerPrivateKeyPassword";
            this.labelPersonalIssuerSignerPrivateKeyPassword.Size = new System.Drawing.Size(143, 13);
            this.labelPersonalIssuerSignerPrivateKeyPassword.TabIndex = 10;
            this.labelPersonalIssuerSignerPrivateKeyPassword.Text = "Signer Private Key Password";
            // 
            // labelPersonalIssuerSignerPrivateKeyFilePath
            // 
            this.labelPersonalIssuerSignerPrivateKeyFilePath.AutoSize = true;
            this.labelPersonalIssuerSignerPrivateKeyFilePath.Location = new System.Drawing.Point(6, 274);
            this.labelPersonalIssuerSignerPrivateKeyFilePath.Name = "labelPersonalIssuerSignerPrivateKeyFilePath";
            this.labelPersonalIssuerSignerPrivateKeyFilePath.Size = new System.Drawing.Size(138, 13);
            this.labelPersonalIssuerSignerPrivateKeyFilePath.TabIndex = 9;
            this.labelPersonalIssuerSignerPrivateKeyFilePath.Text = "Signer Private Key File Path";
            // 
            // labelPersonalIssuerSignerCertificateFilePath
            // 
            this.labelPersonalIssuerSignerCertificateFilePath.AutoSize = true;
            this.labelPersonalIssuerSignerCertificateFilePath.Location = new System.Drawing.Point(6, 300);
            this.labelPersonalIssuerSignerCertificateFilePath.Name = "labelPersonalIssuerSignerCertificateFilePath";
            this.labelPersonalIssuerSignerCertificateFilePath.Size = new System.Drawing.Size(131, 13);
            this.labelPersonalIssuerSignerCertificateFilePath.TabIndex = 8;
            this.labelPersonalIssuerSignerCertificateFilePath.Text = "Signer Certificate File Path";
            // 
            // piEmailAddress
            // 
            this.piEmailAddress.Location = new System.Drawing.Point(160, 186);
            this.piEmailAddress.Name = "piEmailAddress";
            this.piEmailAddress.Size = new System.Drawing.Size(210, 20);
            this.piEmailAddress.TabIndex = 7;
            this.piEmailAddress.TextChanged += new System.EventHandler(this.piEmailAddress_TextChanged);
            // 
            // piCommonName
            // 
            this.piCommonName.Location = new System.Drawing.Point(160, 160);
            this.piCommonName.Name = "piCommonName";
            this.piCommonName.Size = new System.Drawing.Size(245, 20);
            this.piCommonName.TabIndex = 6;
            this.piCommonName.TextChanged += new System.EventHandler(this.piCommonName_TextChanged);
            // 
            // piOrganidationUnitName
            // 
            this.piOrganidationUnitName.Location = new System.Drawing.Point(160, 134);
            this.piOrganidationUnitName.Name = "piOrganidationUnitName";
            this.piOrganidationUnitName.Size = new System.Drawing.Size(153, 20);
            this.piOrganidationUnitName.TabIndex = 5;
            this.piOrganidationUnitName.TextChanged += new System.EventHandler(this.piOrganidationUnitName_TextChanged);
            // 
            // piOrganidationName
            // 
            this.piOrganidationName.Location = new System.Drawing.Point(160, 108);
            this.piOrganidationName.Name = "piOrganidationName";
            this.piOrganidationName.Size = new System.Drawing.Size(180, 20);
            this.piOrganidationName.TabIndex = 4;
            this.piOrganidationName.TextChanged += new System.EventHandler(this.piOrganidationName_TextChanged);
            // 
            // piLocationName
            // 
            this.piLocationName.Location = new System.Drawing.Point(160, 82);
            this.piLocationName.Name = "piLocationName";
            this.piLocationName.Size = new System.Drawing.Size(120, 20);
            this.piLocationName.TabIndex = 3;
            this.piLocationName.TextChanged += new System.EventHandler(this.piLocationName_TextChanged);
            // 
            // piState
            // 
            this.piState.Location = new System.Drawing.Point(160, 56);
            this.piState.Name = "piState";
            this.piState.Size = new System.Drawing.Size(82, 20);
            this.piState.TabIndex = 2;
            this.piState.TextChanged += new System.EventHandler(this.piState_TextChanged);
            // 
            // piCountryName
            // 
            this.piCountryName.Location = new System.Drawing.Point(160, 30);
            this.piCountryName.Name = "piCountryName";
            this.piCountryName.Size = new System.Drawing.Size(40, 20);
            this.piCountryName.TabIndex = 1;
            this.piCountryName.TextChanged += new System.EventHandler(this.piCountryName_TextChanged);
            // 
            // labelPersonalIssuerEmailAddress
            // 
            this.labelPersonalIssuerEmailAddress.AutoSize = true;
            this.labelPersonalIssuerEmailAddress.Location = new System.Drawing.Point(6, 189);
            this.labelPersonalIssuerEmailAddress.Name = "labelPersonalIssuerEmailAddress";
            this.labelPersonalIssuerEmailAddress.Size = new System.Drawing.Size(73, 13);
            this.labelPersonalIssuerEmailAddress.TabIndex = 6;
            this.labelPersonalIssuerEmailAddress.Text = "Email Address";
            // 
            // labelPersonalIssuerCommonName
            // 
            this.labelPersonalIssuerCommonName.AutoSize = true;
            this.labelPersonalIssuerCommonName.Location = new System.Drawing.Point(6, 163);
            this.labelPersonalIssuerCommonName.Name = "labelPersonalIssuerCommonName";
            this.labelPersonalIssuerCommonName.Size = new System.Drawing.Size(79, 13);
            this.labelPersonalIssuerCommonName.TabIndex = 5;
            this.labelPersonalIssuerCommonName.Text = "Common Name";
            // 
            // labelPersonalIssuerOrganisationUnitName
            // 
            this.labelPersonalIssuerOrganisationUnitName.AutoSize = true;
            this.labelPersonalIssuerOrganisationUnitName.Location = new System.Drawing.Point(6, 137);
            this.labelPersonalIssuerOrganisationUnitName.Name = "labelPersonalIssuerOrganisationUnitName";
            this.labelPersonalIssuerOrganisationUnitName.Size = new System.Drawing.Size(119, 13);
            this.labelPersonalIssuerOrganisationUnitName.TabIndex = 4;
            this.labelPersonalIssuerOrganisationUnitName.Text = "Organisation Unit Name";
            // 
            // labelPersonalIssuerOrganisationName
            // 
            this.labelPersonalIssuerOrganisationName.AutoSize = true;
            this.labelPersonalIssuerOrganisationName.Location = new System.Drawing.Point(6, 111);
            this.labelPersonalIssuerOrganisationName.Name = "labelPersonalIssuerOrganisationName";
            this.labelPersonalIssuerOrganisationName.Size = new System.Drawing.Size(97, 13);
            this.labelPersonalIssuerOrganisationName.TabIndex = 3;
            this.labelPersonalIssuerOrganisationName.Text = "Organisation Name";
            // 
            // labelPersonalIssuerLocationName
            // 
            this.labelPersonalIssuerLocationName.AutoSize = true;
            this.labelPersonalIssuerLocationName.Location = new System.Drawing.Point(6, 85);
            this.labelPersonalIssuerLocationName.Name = "labelPersonalIssuerLocationName";
            this.labelPersonalIssuerLocationName.Size = new System.Drawing.Size(79, 13);
            this.labelPersonalIssuerLocationName.TabIndex = 2;
            this.labelPersonalIssuerLocationName.Text = "Location Name";
            // 
            // labelPersonalIssuerState
            // 
            this.labelPersonalIssuerState.AutoSize = true;
            this.labelPersonalIssuerState.Location = new System.Drawing.Point(6, 59);
            this.labelPersonalIssuerState.Name = "labelPersonalIssuerState";
            this.labelPersonalIssuerState.Size = new System.Drawing.Size(32, 13);
            this.labelPersonalIssuerState.TabIndex = 1;
            this.labelPersonalIssuerState.Text = "State";
            // 
            // labelPersonalIssuerCountryName
            // 
            this.labelPersonalIssuerCountryName.AutoSize = true;
            this.labelPersonalIssuerCountryName.Location = new System.Drawing.Point(6, 33);
            this.labelPersonalIssuerCountryName.Name = "labelPersonalIssuerCountryName";
            this.labelPersonalIssuerCountryName.Size = new System.Drawing.Size(74, 13);
            this.labelPersonalIssuerCountryName.TabIndex = 0;
            this.labelPersonalIssuerCountryName.Text = "Country Name";
            // 
            // openFileDialogMain
            // 
            this.openFileDialogMain.DefaultExt = "pem";
            this.openFileDialogMain.Filter = "Certificate Request Files (*.crt *.cer *.pem)|*.crt;*.cer;*.pem";
            this.openFileDialogMain.SupportMultiDottedExtensions = true;
            this.openFileDialogMain.Title = "Open Certificate";
            // 
            // psSignatureAlgorithm
            // 
            this.psSignatureAlgorithm.FormattingEnabled = true;
            this.psSignatureAlgorithm.Items.AddRange(new object[] {
            "SHA 1",
            "SHA 224",
            "SHA 256",
            "SHA 384",
            "SHA 512"});
            this.psSignatureAlgorithm.Location = new System.Drawing.Point(160, 375);
            this.psSignatureAlgorithm.Name = "psSignatureAlgorithm";
            this.psSignatureAlgorithm.Size = new System.Drawing.Size(153, 21);
            this.psSignatureAlgorithm.TabIndex = 15;
            this.psSignatureAlgorithm.SelectedIndexChanged += new System.EventHandler(this.psSignatureAlgorithm_SelectedIndexChanged);
            // 
            // labelPSSignatureAlgorithm
            // 
            this.labelPSSignatureAlgorithm.AutoSize = true;
            this.labelPSSignatureAlgorithm.Location = new System.Drawing.Point(6, 378);
            this.labelPSSignatureAlgorithm.Name = "labelPSSignatureAlgorithm";
            this.labelPSSignatureAlgorithm.Size = new System.Drawing.Size(98, 13);
            this.labelPSSignatureAlgorithm.TabIndex = 30;
            this.labelPSSignatureAlgorithm.Text = "Signature Algorithm";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 527);
            this.Controls.Add(this.tabControlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OpenSSL x509 Certificate Client";
            this.Load += new System.EventHandler(this.Main_Load);
            this.groupBoxCreate.ResumeLayout(false);
            this.groupBoxCreate.PerformLayout();
            this.tabControlMain.ResumeLayout(false);
            this.tabPageCreateCertificate.ResumeLayout(false);
            this.tabPageSignCertificate.ResumeLayout(false);
            this.groupBoxSign.ResumeLayout(false);
            this.groupBoxSign.PerformLayout();
            this.tabPageRequestCertificate.ResumeLayout(false);
            this.groupBoxRequest.ResumeLayout(false);
            this.groupBoxRequest.PerformLayout();
            this.tabPageCreateCA.ResumeLayout(false);
            this.groupBoxCreateCA.ResumeLayout(false);
            this.groupBoxCreateCA.PerformLayout();
            this.tabPageRemovePassword.ResumeLayout(false);
            this.groupBoxRemovePassword.ResumeLayout(false);
            this.groupBoxRemovePassword.PerformLayout();
            this.tabPageCertificateDetails.ResumeLayout(false);
            this.groupBoxCertificateDetails.ResumeLayout(false);
            this.groupBoxCertificateDetails.PerformLayout();
            this.tabPageCertificateCryptograph.ResumeLayout(false);
            this.groupBoxCertificateCryptography.ResumeLayout(false);
            this.tabPageExtractKeys.ResumeLayout(false);
            this.groupBoxExtractKeys.ResumeLayout(false);
            this.groupBoxExtractKeys.PerformLayout();
            this.tabPageCertificateRevoke.ResumeLayout(false);
            this.groupBoxRevoke.ResumeLayout(false);
            this.groupBoxRevoke.PerformLayout();
            this.tabPageGeneratePublicPrivateKeyPair.ResumeLayout(false);
            this.groupBoxGenPublicPrivatePair.ResumeLayout(false);
            this.groupBoxGenPublicPrivatePair.PerformLayout();
            this.tabPagePersonalCertificate.ResumeLayout(false);
            this.groupBoxPersonalCertificate.ResumeLayout(false);
            this.tabControlPersonalCertificate.ResumeLayout(false);
            this.tabPagePersonalSubject.ResumeLayout(false);
            this.tabPagePersonalSubject.PerformLayout();
            this.tabPagePersonalIssuer.ResumeLayout(false);
            this.tabPagePersonalIssuer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxCreate;
        private System.Windows.Forms.Button btnCreateCertificate;
        private System.Windows.Forms.Button buttonCreatePIECertificate;
        private System.Windows.Forms.Label labelemailAddress;
        private System.Windows.Forms.Label labelcommonName;
        private System.Windows.Forms.Label labelorganisationUnitName;
        private System.Windows.Forms.Label labelorganisationName;
        private System.Windows.Forms.Label labellocationName;
        private System.Windows.Forms.Label labelstate;
        private System.Windows.Forms.Label labelcountryName;
        private System.Windows.Forms.TextBox emailAddress;
        private System.Windows.Forms.TextBox commonName;
        private System.Windows.Forms.TextBox organisationUnitName;
        private System.Windows.Forms.TextBox organisationName;
        private System.Windows.Forms.TextBox locationName;
        private System.Windows.Forms.TextBox state;
        private System.Windows.Forms.TextBox countryName;
        private System.Windows.Forms.ToolTip toolTipMain;
        private System.Windows.Forms.TextBox exportPassPhrase;
        private System.Windows.Forms.TextBox certificateAuthorityPassPhrase;
        private System.Windows.Forms.TextBox certificatePassPhrase;
        private System.Windows.Forms.Label labelCertificateAuthorityPassPhrase;
        private System.Windows.Forms.Label labelCertificatePassPhrase;
        private System.Windows.Forms.CheckBox checkBoxUseMultiDomain;
        private System.Windows.Forms.TextBox certificateFilePath;
        private System.Windows.Forms.Button createCertificateFileName;
        private System.Windows.Forms.Label labelExportPassPhrase;
        private System.Windows.Forms.Label labelCertificateFilePath;
        private System.Windows.Forms.SaveFileDialog saveFileDialogMain;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageCreateCertificate;
        private System.Windows.Forms.TabPage tabPageSignCertificate;
        private System.Windows.Forms.TabPage tabPageRequestCertificate;
        private System.Windows.Forms.GroupBox groupBoxRequest;
        private System.Windows.Forms.GroupBox groupBoxSign;
        private System.Windows.Forms.Label labelRequestEmailAddress;
        private System.Windows.Forms.Label labelRequestCommonName;
        private System.Windows.Forms.Label labelRequestOrganisationUnitName;
        private System.Windows.Forms.Label labelRequestOrganisationName;
        private System.Windows.Forms.Label labelRequestLocationName;
        private System.Windows.Forms.Label labelRequestState;
        private System.Windows.Forms.Label labelRequestCountryName;
        private System.Windows.Forms.TextBox textRequestEmailAddress;
        private System.Windows.Forms.TextBox textRequestCommonName;
        private System.Windows.Forms.TextBox textRequestOrganisationUnitName;
        private System.Windows.Forms.TextBox textRequestOrganisationName;
        private System.Windows.Forms.TextBox textRequestLocationName;
        private System.Windows.Forms.TextBox textRequestState;
        private System.Windows.Forms.TextBox textRequestCountryName;
        private System.Windows.Forms.Label labelRequestCertificatePassword;
        private System.Windows.Forms.TextBox textRequestCertificatePassword;
        private System.Windows.Forms.Label labelRequestCertificateFilePath;
        private System.Windows.Forms.Button requestCertificateFilePath;
        private System.Windows.Forms.CheckBox checkBoxRequestMultDomain;
        private System.Windows.Forms.TextBox textRequestCertificateFilePath;
        private System.Windows.Forms.Button btnRequestCertificate;
        private System.Windows.Forms.OpenFileDialog openFileDialogMain;
        private System.Windows.Forms.Label labelSignCertificateAuthorityPassword;
        private System.Windows.Forms.TextBox textSignCertificateAuthorityPassword;
        private System.Windows.Forms.Label labelSignCertificateRequestFilePath;
        private System.Windows.Forms.TextBox textSignCertificateRequestFilePath;
        private System.Windows.Forms.Button btnSignCertificateRequestFilePath;
        private System.Windows.Forms.Button btnSignCertificate;
        private System.Windows.Forms.Label labelSignCertificateFilePath;
        private System.Windows.Forms.Button btnSignCertificateFilePath;
        private System.Windows.Forms.CheckBox checkBoxSignMultiDomian;
        private System.Windows.Forms.TextBox textSignCertificateFilePath;
        private System.Windows.Forms.TabPage tabPageCreateCA;
        private System.Windows.Forms.GroupBox groupBoxCreateCA;
        private System.Windows.Forms.TabPage tabPageRemovePassword;
        private System.Windows.Forms.GroupBox groupBoxRemovePassword;
        private System.Windows.Forms.Button btnCACreateCertificateAuthority;
        private System.Windows.Forms.Button btnCASelectCertificatePrivateKeyPath;
        private System.Windows.Forms.TextBox textCACertificatePrivateKeyPath;
        private System.Windows.Forms.Label labelCACertificatePrivateKeyPath;
        private System.Windows.Forms.Label labelCACertificatePath;
        private System.Windows.Forms.Button btnCASelectCertificatePath;
        private System.Windows.Forms.TextBox textCACertificatePath;
        private System.Windows.Forms.Label labelCACertificatePassword;
        private System.Windows.Forms.TextBox textCACertificatePassword;
        private System.Windows.Forms.Label labelCAEmailAddress;
        private System.Windows.Forms.Label labelCACommonName;
        private System.Windows.Forms.Label labelCAOrganisationUnitName;
        private System.Windows.Forms.Label labelCAOrganisationName;
        private System.Windows.Forms.Label labelCALocationName;
        private System.Windows.Forms.Label labelCAState;
        private System.Windows.Forms.Label labelCACountryName;
        private System.Windows.Forms.TextBox textCAEmailAddress;
        private System.Windows.Forms.TextBox textCACommonName;
        private System.Windows.Forms.TextBox textCAOrganisationUnitName;
        private System.Windows.Forms.TextBox textCAOrganisationName;
        private System.Windows.Forms.TextBox textCALocationName;
        private System.Windows.Forms.TextBox textCAState;
        private System.Windows.Forms.TextBox textCACountryName;
        private System.Windows.Forms.TextBox textCADays;
        private System.Windows.Forms.Label labelCADays;
        private System.Windows.Forms.Button btnRemovePassword;
        private System.Windows.Forms.Button btnRemoveCertificateDecryptedPath;
        private System.Windows.Forms.TextBox textRemoveCertificateDecryptedPath;
        private System.Windows.Forms.Label labelRemoveCertificateDecryptedPath;
        private System.Windows.Forms.Label labelRemoveCertificatePath;
        private System.Windows.Forms.Button btnRemoveCertificatePath;
        private System.Windows.Forms.TextBox textRemoveCertificatePath;
        private System.Windows.Forms.Label labelRemoveCertificatePassword;
        private System.Windows.Forms.TextBox textRemoveCertificatePassword;
        private System.Windows.Forms.TabPage tabPageCertificateDetails;
        private System.Windows.Forms.GroupBox groupBoxCertificateDetails;
        private System.Windows.Forms.TabPage tabPageCertificateCryptograph;
        private System.Windows.Forms.GroupBox groupBoxCertificateCryptography;
        private UI.Security.RsaCryptography rsaCryptography;
        private System.Windows.Forms.RichTextBox richTextBoxCertificateDetails;
        private System.Windows.Forms.Button btnCertificateDetailsLoad;
        private System.Windows.Forms.Button btnCertificateDetailsPath;
        private System.Windows.Forms.TextBox txtCertificateDetailsPassword;
        private System.Windows.Forms.TextBox txtCertificateDetailsPath;
        private System.Windows.Forms.Label lblCertificateDetailsPassword;
        private System.Windows.Forms.Label lblCertificateDetailsPath;
        private System.Windows.Forms.TabPage tabPageExtractKeys;
        private System.Windows.Forms.GroupBox groupBoxExtractKeys;
        private System.Windows.Forms.Button btnExtractPrivateKeyPath;
        private System.Windows.Forms.Button btnExtractPublicKeyPath;
        private System.Windows.Forms.Label lblExtractPrivateKeyPath;
        private System.Windows.Forms.TextBox txtExtractPrivateKeyPath;
        private System.Windows.Forms.TextBox txtExtractPublicKeyPath;
        private System.Windows.Forms.Label lblExtractPublicKeyPath;
        private System.Windows.Forms.Label lblExtractCertificatePath;
        private System.Windows.Forms.Label lblExtractExportPassword;
        private System.Windows.Forms.Button btnExtractCertificatePath;
        private System.Windows.Forms.Label lblExtractCertificatePassword;
        private System.Windows.Forms.TextBox txtExtractCertificatePath;
        private System.Windows.Forms.TextBox txtExtractExportPassword;
        private System.Windows.Forms.TextBox txtExtractCertificatePassword;
        private System.Windows.Forms.Button btnExtractPublicKey;
        private System.Windows.Forms.Button btnExtractPrivateKey;
        private System.Windows.Forms.TabPage tabPageCertificateRevoke;
        private System.Windows.Forms.GroupBox groupBoxRevoke;
        private System.Windows.Forms.Button btnRevokeUpdateDatabase;
        private System.Windows.Forms.Button btnRevokeList;
        private System.Windows.Forms.Button btnRevokeCertificate;
        private System.Windows.Forms.Button btnRevokeListPath;
        private System.Windows.Forms.TextBox txtRevokeListPath;
        private System.Windows.Forms.Label lblRevokeListPath;
        private System.Windows.Forms.Label lblRevokeCertificatePath;
        private System.Windows.Forms.Button btnRevokeCertificatePath;
        private System.Windows.Forms.TextBox txtRevokeCertificatePath;
        private System.Windows.Forms.Button btnRevokeCertificateListPath;
        private System.Windows.Forms.Label lblRevokeCertificateListPath;
        private System.Windows.Forms.TextBox txtRevokeCertificateListPath;
        private System.Windows.Forms.Label lblRevokeCertificateAuthPassword;
        private System.Windows.Forms.TextBox txtRevokeCertificateAuthPassword;
        private System.Windows.Forms.TabPage tabPageGeneratePublicPrivateKeyPair;
        private System.Windows.Forms.GroupBox groupBoxGenPublicPrivatePair;
        private System.Windows.Forms.Button btnCertificateDetailsView;
        private System.Windows.Forms.Button btnGenPublicPrivatePairPrivateKey;
        private System.Windows.Forms.Button btnGenPublicPrivatePairPublicKey;
        private System.Windows.Forms.TextBox txtGenPublicPrivatePairSizePublicKey;
        private System.Windows.Forms.TextBox txtGenPublicPrivatePairPrivateKey;
        private System.Windows.Forms.TextBox txtGenPublicPrivatePairSize;
        private System.Windows.Forms.Button btnGenPublicPrivatePairPublicGo;
        private System.Windows.Forms.Label lblGenPublicPrivatePairSizePublicKey;
        private System.Windows.Forms.Label lblGenPublicPrivatePairPrivateKey;
        private System.Windows.Forms.Label lblGenPublicPrivatePairSize;
        private System.Windows.Forms.TabPage tabPagePersonalCertificate;
        private System.Windows.Forms.GroupBox groupBoxPersonalCertificate;
        private System.Windows.Forms.TabControl tabControlPersonalCertificate;
        private System.Windows.Forms.TabPage tabPagePersonalSubject;
        private System.Windows.Forms.TabPage tabPagePersonalIssuer;
        private System.Windows.Forms.Label labelPersonalIssuerEmailAddress;
        private System.Windows.Forms.Label labelPersonalIssuerCommonName;
        private System.Windows.Forms.Label labelPersonalIssuerOrganisationUnitName;
        private System.Windows.Forms.Label labelPersonalIssuerOrganisationName;
        private System.Windows.Forms.Label labelPersonalIssuerLocationName;
        private System.Windows.Forms.Label labelPersonalIssuerState;
        private System.Windows.Forms.Label labelPersonalIssuerCountryName;
        private System.Windows.Forms.TextBox piEmailAddress;
        private System.Windows.Forms.TextBox piCommonName;
        private System.Windows.Forms.TextBox piOrganidationUnitName;
        private System.Windows.Forms.TextBox piOrganidationName;
        private System.Windows.Forms.TextBox piLocationName;
        private System.Windows.Forms.TextBox piState;
        private System.Windows.Forms.TextBox piCountryName;
        private System.Windows.Forms.Label labelPersonalIssuerSignerPrivateKeyPassword;
        private System.Windows.Forms.Label labelPersonalIssuerSignerPrivateKeyFilePath;
        private System.Windows.Forms.Label labelPersonalIssuerSignerCertificateFilePath;
        private System.Windows.Forms.TextBox piCertificateFilePath;
        private System.Windows.Forms.TextBox piSignerPrivateFilePath;
        private System.Windows.Forms.TextBox piPassword;
        private System.Windows.Forms.Button piSignerCertificateSelect;
        private System.Windows.Forms.Button piSignerPrivateKeySelect;
        private System.Windows.Forms.Button psGenCertificate;
        private System.Windows.Forms.TextBox psEmailAddress;
        private System.Windows.Forms.TextBox psCommonName;
        private System.Windows.Forms.TextBox psOrganisationUnitName;
        private System.Windows.Forms.TextBox psOrganisationName;
        private System.Windows.Forms.TextBox psLocationName;
        private System.Windows.Forms.TextBox psState;
        private System.Windows.Forms.TextBox psCountryName;
        private System.Windows.Forms.Label labelPersonalSubjectEmailAddress;
        private System.Windows.Forms.Label labelPersonalSubjectCommonName;
        private System.Windows.Forms.Label labelPersonalSubjectOrganisationUnitName;
        private System.Windows.Forms.Label labelPersonalSubjectOrganisationName;
        private System.Windows.Forms.Label labelPersonalSubjectLocationName;
        private System.Windows.Forms.Label labelPersonalSubjectState;
        private System.Windows.Forms.Label labelPersonalSubjectCountryName;
        private System.Windows.Forms.TextBox psKeySize;
        private System.Windows.Forms.TextBox psValidDays;
        private System.Windows.Forms.Label labelPSValidDays;
        private System.Windows.Forms.Label labelPSKeySize;
        private System.Windows.Forms.Button psPIEPathSelect;
        private System.Windows.Forms.Button psCertificatePathSelect;
        private System.Windows.Forms.TextBox psPIEPath;
        private System.Windows.Forms.TextBox psCertificatePath;
        private System.Windows.Forms.TextBox psPassword;
        private System.Windows.Forms.Label labelPSCertificatePassword;
        private System.Windows.Forms.Label labelPSCertificateFilePath;
        private System.Windows.Forms.Label labelPSPIEFilePath;
        private System.Windows.Forms.ComboBox psSignatureAlgorithm;
        private System.Windows.Forms.Label labelPSSignatureAlgorithm;
    }
}