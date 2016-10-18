using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nequeo.Forms.Security.OpenSsl.Client
{
    /// <summary>
    /// Main form
    /// </summary>
    public partial class Main : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Main()
        {
            InitializeComponent();
        }

        private Dictionary<int, bool> _validCreateCertificate = new Dictionary<int, bool>();
        private Dictionary<int, bool> _validCreatePieCertificate = new Dictionary<int, bool>();
        private Dictionary<int, bool> _validCreateRequestCertificate = new Dictionary<int, bool>();
        private Dictionary<int, bool> _validCreateSignCertificate = new Dictionary<int, bool>();
        private Dictionary<int, bool> _validCreateCACertificate = new Dictionary<int, bool>();
        private Dictionary<int, bool> _validCreateRemoveCertificate = new Dictionary<int, bool>();
        private Dictionary<int, bool> _validExtractPublicCertificate = new Dictionary<int, bool>();
        private Dictionary<int, bool> _validExtractPrivateCertificate = new Dictionary<int, bool>();
        private Dictionary<int, bool> _validRevokeUpdateDatabaseCertificate = new Dictionary<int, bool>();
        private Dictionary<int, bool> _validRevokeListCertificate = new Dictionary<int, bool>();
        private Dictionary<int, bool> _validRevokeCertificateListCertificate = new Dictionary<int, bool>();
        private Dictionary<int, bool> _validGenPublicPrivateKeyPairCertificate = new Dictionary<int, bool>();
        private Dictionary<int, bool> _validPersonalCertificate = new Dictionary<int, bool>();

        private Nequeo.Cryptography.Signing.SignatureAlgorithm _signatureAlgorithm = Cryptography.Signing.SignatureAlgorithm.SHA512withRSA;

        /// <summary>
        /// Form load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Load(object sender, EventArgs e)
        {
            // Assign each certificate manditary variable
            _validCreateCertificate.Add(countryName.GetHashCode(), false);
            _validCreateCertificate.Add(state.GetHashCode(), false);
            _validCreateCertificate.Add(locationName.GetHashCode(), false);
            _validCreateCertificate.Add(organisationName.GetHashCode(), false);
            _validCreateCertificate.Add(organisationUnitName.GetHashCode(), false);
            _validCreateCertificate.Add(commonName.GetHashCode(), false);
            _validCreateCertificate.Add(emailAddress.GetHashCode(), false);
            //_validCreateCertificate.Add(certificatePassPhrase.GetHashCode(), false);
            _validCreateCertificate.Add(certificateAuthorityPassPhrase.GetHashCode(), false);
            _validCreateCertificate.Add(certificateFilePath.GetHashCode(), false);

            // Assign each PIE certificate manditary variable
            _validCreatePieCertificate.Add(countryName.GetHashCode(), false);
            _validCreatePieCertificate.Add(state.GetHashCode(), false);
            _validCreatePieCertificate.Add(locationName.GetHashCode(), false);
            _validCreatePieCertificate.Add(organisationName.GetHashCode(), false);
            _validCreatePieCertificate.Add(organisationUnitName.GetHashCode(), false);
            _validCreatePieCertificate.Add(commonName.GetHashCode(), false);
            _validCreatePieCertificate.Add(emailAddress.GetHashCode(), false);
            //_validCreatePieCertificate.Add(certificatePassPhrase.GetHashCode(), false);
            _validCreatePieCertificate.Add(certificateAuthorityPassPhrase.GetHashCode(), false);
            //_validCreatePieCertificate.Add(exportPassPhrase.GetHashCode(), false);
            _validCreatePieCertificate.Add(certificateFilePath.GetHashCode(), false);

            // Assign each certificate manditary variable
            _validCreateRequestCertificate.Add(textRequestCountryName.GetHashCode(), false);
            _validCreateRequestCertificate.Add(textRequestState.GetHashCode(), false);
            _validCreateRequestCertificate.Add(textRequestLocationName.GetHashCode(), false);
            _validCreateRequestCertificate.Add(textRequestOrganisationName.GetHashCode(), false);
            _validCreateRequestCertificate.Add(textRequestOrganisationUnitName.GetHashCode(), false);
            _validCreateRequestCertificate.Add(textRequestCommonName.GetHashCode(), false);
            _validCreateRequestCertificate.Add(textRequestEmailAddress.GetHashCode(), false);
           // _validCreateRequestCertificate.Add(textRequestCertificatePassword.GetHashCode(), false);
            _validCreateRequestCertificate.Add(textRequestCertificateFilePath.GetHashCode(), false);

            // Assign each certificate manditary variable
            _validCreateSignCertificate.Add(textSignCertificateAuthorityPassword.GetHashCode(), false);
            _validCreateSignCertificate.Add(textSignCertificateRequestFilePath.GetHashCode(), false);
            _validCreateSignCertificate.Add(textSignCertificateFilePath.GetHashCode(), false);

            // Assign each certificate manditary variable
            _validCreateCACertificate.Add(textCACountryName.GetHashCode(), false);
            _validCreateCACertificate.Add(textCAState.GetHashCode(), false);
            _validCreateCACertificate.Add(textCALocationName.GetHashCode(), false);
            _validCreateCACertificate.Add(textCAOrganisationName.GetHashCode(), false);
            _validCreateCACertificate.Add(textCAOrganisationUnitName.GetHashCode(), false);
            _validCreateCACertificate.Add(textCACommonName.GetHashCode(), false);
            _validCreateCACertificate.Add(textCAEmailAddress.GetHashCode(), false);
            _validCreateCACertificate.Add(textCACertificatePassword.GetHashCode(), false);
            _validCreateCACertificate.Add(textCACertificatePath.GetHashCode(), false);
            _validCreateCACertificate.Add(textCACertificatePrivateKeyPath.GetHashCode(), false);
            _validCreateCACertificate.Add(textCADays.GetHashCode(), false);

            // Assign each certificate manditary variable
            _validCreateRemoveCertificate.Add(textRemoveCertificatePassword.GetHashCode(), false);
            _validCreateRemoveCertificate.Add(textRemoveCertificatePath.GetHashCode(), false);
            _validCreateRemoveCertificate.Add(textRemoveCertificateDecryptedPath.GetHashCode(), false);

            //_validExtractPublicCertificate.Add(txtExtractExportPassword.GetHashCode(), false);
            //_validExtractPublicCertificate.Add(txtExtractCertificatePassword.GetHashCode(), false);
            _validExtractPublicCertificate.Add(txtExtractCertificatePath.GetHashCode(), false);
            _validExtractPublicCertificate.Add(txtExtractPublicKeyPath.GetHashCode(), false);

            //_validExtractPrivateCertificate.Add(txtExtractExportPassword.GetHashCode(), false);
            //_validExtractPrivateCertificate.Add(txtExtractCertificatePassword.GetHashCode(), false);
            _validExtractPrivateCertificate.Add(txtExtractCertificatePath.GetHashCode(), false);
            _validExtractPrivateCertificate.Add(txtExtractPrivateKeyPath.GetHashCode(), false);

            _validRevokeUpdateDatabaseCertificate.Add(txtRevokeCertificateAuthPassword.GetHashCode(), false);

            _validRevokeListCertificate.Add(txtRevokeCertificateAuthPassword.GetHashCode(), false);
            _validRevokeListCertificate.Add(txtRevokeCertificatePath.GetHashCode(), false);

            _validRevokeCertificateListCertificate.Add(txtRevokeCertificateAuthPassword.GetHashCode(), false);
            _validRevokeCertificateListCertificate.Add(txtRevokeCertificateListPath.GetHashCode(), false);
            _validRevokeCertificateListCertificate.Add(txtRevokeListPath.GetHashCode(), false);

            _validGenPublicPrivateKeyPairCertificate.Add(txtGenPublicPrivatePairSize.GetHashCode(), true);
            _validGenPublicPrivateKeyPairCertificate.Add(txtGenPublicPrivatePairPrivateKey.GetHashCode(), false);
            _validGenPublicPrivateKeyPairCertificate.Add(txtGenPublicPrivatePairSizePublicKey.GetHashCode(), false);

            _validPersonalCertificate.Add(piCountryName.GetHashCode(), false);
            _validPersonalCertificate.Add(piState.GetHashCode(), false);
            _validPersonalCertificate.Add(piLocationName.GetHashCode(), false);
            _validPersonalCertificate.Add(piOrganidationName.GetHashCode(), false);
            _validPersonalCertificate.Add(piOrganidationUnitName.GetHashCode(), false);
            _validPersonalCertificate.Add(piCommonName.GetHashCode(), false);
            _validPersonalCertificate.Add(piEmailAddress.GetHashCode(), false);
            //_validPersonalCertificate.Add(piPassword.GetHashCode(), false);
            _validPersonalCertificate.Add(piSignerPrivateFilePath.GetHashCode(), false);
            _validPersonalCertificate.Add(piCertificateFilePath.GetHashCode(), false);
            _validPersonalCertificate.Add(psCountryName.GetHashCode(), false);
            _validPersonalCertificate.Add(psState.GetHashCode(), false);
            _validPersonalCertificate.Add(psLocationName.GetHashCode(), false);
            _validPersonalCertificate.Add(psOrganisationName.GetHashCode(), false);
            _validPersonalCertificate.Add(psOrganisationUnitName.GetHashCode(), false);
            _validPersonalCertificate.Add(psCommonName.GetHashCode(), false);
            _validPersonalCertificate.Add(psEmailAddress.GetHashCode(), false);
            _validPersonalCertificate.Add(psPassword.GetHashCode(), false);
            _validPersonalCertificate.Add(psCertificatePath.GetHashCode(), false);
            _validPersonalCertificate.Add(psPIEPath.GetHashCode(), false);
            _validPersonalCertificate.Add(psValidDays.GetHashCode(), true);
            _validPersonalCertificate.Add(psKeySize.GetHashCode(), true);

            rsaCryptography.OnError += new Threading.EventHandler<Nequeo.Custom.ClientCommandArgs>(rsaCryptography_OnError);
            rsaCryptography.OnComplete += new Threading.EventHandler<Custom.ClientCommandArgs>(rsaCryptography_OnComplete);

            Nequeo.Cryptography.Openssl.Subject personalIssuer = Nequeo.Cryptography.Openssl.Subject.GetNequeoPtyLtdCA();
            piCountryName.Text = personalIssuer.CountryName;
            piState.Text = personalIssuer.State;
            piLocationName.Text = personalIssuer.LocationName;
            piOrganidationName.Text = personalIssuer.OrganisationName;
            piOrganidationUnitName.Text = personalIssuer.OrganisationUnitName;
            piCommonName.Text = personalIssuer.CommonName;
            piEmailAddress.Text = personalIssuer.EmailAddress;

            piSignerPrivateFilePath.Text = Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLCAPrivateKey;
            piCertificateFilePath.Text = Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLCACertificate;
            psSignatureAlgorithm.SelectedIndex = 4;
        }

        /// <summary>
        /// RSA Cryptography complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e1"></param>
        private void rsaCryptography_OnComplete(object sender, Custom.ClientCommandArgs e1)
        {
            MessageBox.Show(e1.Data, "Certificate Cryptography Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// RSA Cryptography error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e1"></param>
        private void rsaCryptography_OnError(object sender, Nequeo.Custom.ClientCommandArgs e1)
        {
            MessageBox.Show(e1.Data, "Certificate Cryptography Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Select file name event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createCertificateFileName_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            saveFileDialogMain.Filter = "Certificate Files (*.pem)|*.pem";

            // Get the file name selected.
            if (saveFileDialogMain.ShowDialog() == DialogResult.OK)
                certificateFilePath.Text = saveFileDialogMain.FileName;
        }

        /// <summary>
        /// Create the new certificate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateCertificate_Click(object sender, EventArgs e)
        {
            // Create a new instance of the openssl certificate implementation
            Nequeo.Security.OpenSsl.CertificateConfiguration certificate = new Nequeo.Security.OpenSsl.CertificateConfiguration();

            // If multi domain checked then create a single instance certificate
            // else create a new multi domain certificate.
            if (!checkBoxUseMultiDomain.Checked)
                certificate.CreateCertificate(CreateSubject(), certificatePassPhrase.Text, certificateAuthorityPassPhrase.Text, certificateFilePath.Text);
            else
                certificate.CreateCertificateMultiDomain(CreateSubject(), certificatePassPhrase.Text, certificateAuthorityPassPhrase.Text, certificateFilePath.Text);
        }

        /// <summary>
        /// Create the new PIE certificate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCreatePIECertificate_Click(object sender, EventArgs e)
        {
            // Create a new instance of the openssl certificate implementation
            Nequeo.Security.OpenSsl.CertificateConfiguration certificate = new Nequeo.Security.OpenSsl.CertificateConfiguration();

            // If multi domain checked then create a single instance certificate
            // else create a new multi domain certificate.
            if (!checkBoxUseMultiDomain.Checked)
                certificate.CreatePIECertificate(CreateSubject(), certificatePassPhrase.Text, certificateAuthorityPassPhrase.Text, exportPassPhrase.Text, certificateFilePath.Text);
            else
                certificate.CreatePIECertificateMultiDomain(CreateSubject(), certificatePassPhrase.Text, certificateAuthorityPassPhrase.Text, exportPassPhrase.Text, certificateFilePath.Text);
        }

        /// <summary>
        /// Country name text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void countryName_TextChanged(object sender, EventArgs e)
        {
            CreateCertificateContainsText((TextBox)sender);
            CreatePieCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// State text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void state_TextChanged(object sender, EventArgs e)
        {
            CreateCertificateContainsText((TextBox)sender);
            CreatePieCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Location name text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void locationName_TextChanged(object sender, EventArgs e)
        {
            CreateCertificateContainsText((TextBox)sender);
            CreatePieCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Organisation name text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void organisationName_TextChanged(object sender, EventArgs e)
        {
            CreateCertificateContainsText((TextBox)sender);
            CreatePieCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Organisation unit name text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void organisationUnitName_TextChanged(object sender, EventArgs e)
        {
            CreateCertificateContainsText((TextBox)sender);
            CreatePieCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Common name text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void commonName_TextChanged(object sender, EventArgs e)
        {
            CreateCertificateContainsText((TextBox)sender);
            CreatePieCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Email address text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void emailAddress_TextChanged(object sender, EventArgs e)
        {
            CreateCertificateContainsText((TextBox)sender);
            CreatePieCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Certificate password text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void certificatePassPhrase_TextChanged(object sender, EventArgs e)
        {
            CreateCertificateContainsText((TextBox)sender);
            CreatePieCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Certificate authority password text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void certificateAuthorityPassPhrase_TextChanged(object sender, EventArgs e)
        {
            CreateCertificateContainsText((TextBox)sender);
            CreatePieCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Export PIE password text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportPassPhrase_TextChanged(object sender, EventArgs e)
        {
            CreatePieCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Certificate path and file name text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void certificateFilePath_TextChanged(object sender, EventArgs e)
        {
            CreateCertificateContainsText((TextBox)sender);
            CreatePieCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Checked multi domain changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxUseMultiDomain_CheckedChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Create the subject container from the input data.
        /// </summary>
        /// <returns>The certificate subject</returns>
        private Nequeo.Security.OpenSsl.Container.Subject CreateSubject()
        {
            Nequeo.Security.OpenSsl.Container.Subject subject =
                new Nequeo.Security.OpenSsl.Container.Subject(
                    countryName.Text,
                    state.Text,
                    locationName.Text,
                    organisationName.Text,
                    organisationUnitName.Text,
                    commonName.Text,
                    emailAddress.Text
                );

            // Return the new subject
            return subject;
        }

        /// <summary>
        /// Create the subject container from the input data.
        /// </summary>
        /// <returns>The certificate subject</returns>
        private Nequeo.Security.OpenSsl.Container.Subject RequestSubject()
        {
            Nequeo.Security.OpenSsl.Container.Subject subject =
                new Nequeo.Security.OpenSsl.Container.Subject(
                    textRequestCountryName.Text,
                    textRequestState.Text,
                    textRequestLocationName.Text,
                    textRequestOrganisationName.Text,
                    textRequestOrganisationUnitName.Text,
                    textRequestCommonName.Text,
                    textRequestEmailAddress.Text
                );

            // Return the new subject
            return subject;
        }

        /// <summary>
        /// Create the subject container from the input data.
        /// </summary>
        /// <returns>The certificate subject</returns>
        private Nequeo.Security.OpenSsl.Container.Subject CertificateAuthoritySubject()
        {
            Nequeo.Security.OpenSsl.Container.Subject subject =
                new Nequeo.Security.OpenSsl.Container.Subject(
                    textCACountryName.Text,
                    textCAState.Text,
                    textCALocationName.Text,
                    textCAOrganisationName.Text,
                    textCAOrganisationUnitName.Text,
                    textCACommonName.Text,
                    textCAEmailAddress.Text
                );

            // Return the new subject
            return subject;
        }

        /// <summary>
        /// Enable or disable the request certificate button accoring to valid input data.
        /// </summary>
        /// <param name="textBox">The current text box instance</param>
        private void RevokeListCertificateContainsText(TextBox textBox)
        {
            bool isValid = true;

            // If no text has been add then invalid.
            if (String.IsNullOrEmpty(textBox.Text))
                isValid = false;

            // Add the current validation item.
            if (!_validRevokeCertificateListCertificate.Keys.Contains(textBox.GetHashCode()))
                _validRevokeCertificateListCertificate.Add(textBox.GetHashCode(), isValid);
            else
                _validRevokeCertificateListCertificate[textBox.GetHashCode()] = isValid;

            // Find all invalid items, if no invalid items then
            // enable operation controls.
            IEnumerable<bool> inValid = _validRevokeCertificateListCertificate.Values.Where(u => (u == false));
            if ((inValid.Count() < 1))
                btnRevokeList.Enabled = true;
            else
                btnRevokeList.Enabled = false;
        }

        /// <summary>
        /// Enable or disable the request certificate button accoring to valid input data.
        /// </summary>
        /// <param name="textBox">The current text box instance</param>
        private void RevokeCertificateContainsText(TextBox textBox)
        {
            bool isValid = true;

            // If no text has been add then invalid.
            if (String.IsNullOrEmpty(textBox.Text))
                isValid = false;

            // Add the current validation item.
            if (!_validRevokeListCertificate.Keys.Contains(textBox.GetHashCode()))
                _validRevokeListCertificate.Add(textBox.GetHashCode(), isValid);
            else
                _validRevokeListCertificate[textBox.GetHashCode()] = isValid;

            // Find all invalid items, if no invalid items then
            // enable operation controls.
            IEnumerable<bool> inValid = _validRevokeListCertificate.Values.Where(u => (u == false));
            if ((inValid.Count() < 1))
                btnRevokeCertificate.Enabled = true;
            else
                btnRevokeCertificate.Enabled = false;
        }

        /// <summary>
        /// Enable or disable the request certificate button accoring to valid input data.
        /// </summary>
        /// <param name="textBox">The current text box instance</param>
        private void RevokeUpdateDatabaseCertificateContainsText(TextBox textBox)
        {
            bool isValid = true;

            // If no text has been add then invalid.
            if (String.IsNullOrEmpty(textBox.Text))
                isValid = false;

            // Add the current validation item.
            if (!_validRevokeUpdateDatabaseCertificate.Keys.Contains(textBox.GetHashCode()))
                _validRevokeUpdateDatabaseCertificate.Add(textBox.GetHashCode(), isValid);
            else
                _validRevokeUpdateDatabaseCertificate[textBox.GetHashCode()] = isValid;

            // Find all invalid items, if no invalid items then
            // enable operation controls.
            IEnumerable<bool> inValid = _validRevokeUpdateDatabaseCertificate.Values.Where(u => (u == false));
            if ((inValid.Count() < 1))
                btnRevokeUpdateDatabase.Enabled = true;
            else
                btnRevokeUpdateDatabase.Enabled = false;
        }

        /// <summary>
        /// Enable or disable the create certificate button accoring to valid input data.
        /// </summary>
        /// <param name="textBox">The current text box instance</param>
        private void CreateCertificateContainsText(TextBox textBox)
        {
            bool isValid = true;

            // If no text has been add then invalid.
            if (String.IsNullOrEmpty(textBox.Text))
                isValid = false;

            // Add the current validation item.
            if (!_validCreateCertificate.Keys.Contains(textBox.GetHashCode()))
                _validCreateCertificate.Add(textBox.GetHashCode(), isValid);
            else
                _validCreateCertificate[textBox.GetHashCode()] = isValid;

            // Find all invalid items, if no invalid items then
            // enable operation controls.
            IEnumerable<bool> inValid = _validCreateCertificate.Values.Where(u => (u == false));
            if ((inValid.Count() < 1))
                btnCreateCertificate.Enabled = true;
            else
                btnCreateCertificate.Enabled = false;
        }

        /// <summary>
        /// Enable or disable the create PIE certificate button accoring to valid input data.
        /// </summary>
        /// <param name="textBox">The current text box instance</param>
        private void CreatePieCertificateContainsText(TextBox textBox)
        {
            bool isValid = true;

            // If no text has been add then invalid.
            if (String.IsNullOrEmpty(textBox.Text))
                isValid = false;

            // Add the current validation item.
            if (!_validCreatePieCertificate.Keys.Contains(textBox.GetHashCode()))
                _validCreatePieCertificate.Add(textBox.GetHashCode(), isValid);
            else
                _validCreatePieCertificate[textBox.GetHashCode()] = isValid;

            // Find all invalid items, if no invalid items then
            // enable operation controls.
            IEnumerable<bool> inValid = _validCreatePieCertificate.Values.Where(u => (u == false));
            if ((inValid.Count() < 1))
                buttonCreatePIECertificate.Enabled = true;
            else
                buttonCreatePIECertificate.Enabled = false;
        }

        /// <summary>
        /// Enable or disable the create PIE certificate button accoring to valid input data.
        /// </summary>
        /// <param name="textBox">The current text box instance</param>
        private void ExtractPublicKeyCertificateContainsText(TextBox textBox)
        {
            bool isValid = true;

            // If no text has been add then invalid.
            if (String.IsNullOrEmpty(textBox.Text))
                isValid = false;

            // Add the current validation item.
            if (!_validExtractPublicCertificate.Keys.Contains(textBox.GetHashCode()))
                _validExtractPublicCertificate.Add(textBox.GetHashCode(), isValid);
            else
                _validExtractPublicCertificate[textBox.GetHashCode()] = isValid;

            // Find all invalid items, if no invalid items then
            // enable operation controls.
            IEnumerable<bool> inValid = _validExtractPublicCertificate.Values.Where(u => (u == false));
            if ((inValid.Count() < 1))
                btnExtractPublicKey.Enabled = true;
            else
                btnExtractPublicKey.Enabled = false;
        }

        /// <summary>
        /// Enable or disable the create PIE certificate button accoring to valid input data.
        /// </summary>
        /// <param name="textBox">The current text box instance</param>
        private void ExtractPrivateKeyCertificateContainsText(TextBox textBox)
        {
            bool isValid = true;

            // If no text has been add then invalid.
            if (String.IsNullOrEmpty(textBox.Text))
                isValid = false;

            // Add the current validation item.
            if (!_validExtractPrivateCertificate.Keys.Contains(textBox.GetHashCode()))
                _validExtractPrivateCertificate.Add(textBox.GetHashCode(), isValid);
            else
                _validExtractPrivateCertificate[textBox.GetHashCode()] = isValid;

            // Find all invalid items, if no invalid items then
            // enable operation controls.
            IEnumerable<bool> inValid = _validExtractPrivateCertificate.Values.Where(u => (u == false));
            if ((inValid.Count() < 1))
                btnExtractPrivateKey.Enabled = true;
            else
                btnExtractPrivateKey.Enabled = false;
        }

        /// <summary>
        /// Enable or disable the request certificate button accoring to valid input data.
        /// </summary>
        /// <param name="textBox">The current text box instance</param>
        private void CreateRequestCertificateContainsText(TextBox textBox)
        {
            bool isValid = true;

            // If no text has been add then invalid.
            if (String.IsNullOrEmpty(textBox.Text))
                isValid = false;

            // Add the current validation item.
            if (!_validCreateRequestCertificate.Keys.Contains(textBox.GetHashCode()))
                _validCreateRequestCertificate.Add(textBox.GetHashCode(), isValid);
            else
                _validCreateRequestCertificate[textBox.GetHashCode()] = isValid;

            // Find all invalid items, if no invalid items then
            // enable operation controls.
            IEnumerable<bool> inValid = _validCreateRequestCertificate.Values.Where(u => (u == false));
            if ((inValid.Count() < 1))
                btnRequestCertificate.Enabled = true;
            else
                btnRequestCertificate.Enabled = false;
        }

        /// <summary>
        /// Enable or disable the request certificate button accoring to valid input data.
        /// </summary>
        /// <param name="textBox">The current text box instance</param>
        private void CreateSignCertificateContainsText(TextBox textBox)
        {
            bool isValid = true;

            // If no text has been add then invalid.
            if (String.IsNullOrEmpty(textBox.Text))
                isValid = false;

            // Add the current validation item.
            if (!_validCreateSignCertificate.Keys.Contains(textBox.GetHashCode()))
                _validCreateSignCertificate.Add(textBox.GetHashCode(), isValid);
            else
                _validCreateSignCertificate[textBox.GetHashCode()] = isValid;

            // Find all invalid items, if no invalid items then
            // enable operation controls.
            IEnumerable<bool> inValid = _validCreateSignCertificate.Values.Where(u => (u == false));
            if ((inValid.Count() < 1))
                btnSignCertificate.Enabled = true;
            else
                btnSignCertificate.Enabled = false;
        }

        /// <summary>
        /// Enable or disable the request certificate button accoring to valid input data.
        /// </summary>
        /// <param name="textBox">The current text box instance</param>
        private void CreateCACertificateContainsText(TextBox textBox)
        {
            bool isValid = true;

            // If no text has been add then invalid.
            if (String.IsNullOrEmpty(textBox.Text))
                isValid = false;

            // Add the current validation item.
            if (!_validCreateCACertificate.Keys.Contains(textBox.GetHashCode()))
                _validCreateCACertificate.Add(textBox.GetHashCode(), isValid);
            else
                _validCreateCACertificate[textBox.GetHashCode()] = isValid;

            // Find all invalid items, if no invalid items then
            // enable operation controls.
            IEnumerable<bool> inValid = _validCreateCACertificate.Values.Where(u => (u == false));
            if ((inValid.Count() < 1))
                btnCACreateCertificateAuthority.Enabled = true;
            else
                btnCACreateCertificateAuthority.Enabled = false;
        }

        /// <summary>
        /// Enable or disable the request certificate button accoring to valid input data.
        /// </summary>
        /// <param name="textBox">The current text box instance</param>
        private void CreateRemoveCertificateContainsText(TextBox textBox)
        {
            bool isValid = true;

            // If no text has been add then invalid.
            if (String.IsNullOrEmpty(textBox.Text))
                isValid = false;

            // Add the current validation item.
            if (!_validCreateRemoveCertificate.Keys.Contains(textBox.GetHashCode()))
                _validCreateRemoveCertificate.Add(textBox.GetHashCode(), isValid);
            else
                _validCreateRemoveCertificate[textBox.GetHashCode()] = isValid;

            // Find all invalid items, if no invalid items then
            // enable operation controls.
            IEnumerable<bool> inValid = _validCreateRemoveCertificate.Values.Where(u => (u == false));
            if ((inValid.Count() < 1))
                btnRemovePassword.Enabled = true;
            else
                btnRemovePassword.Enabled = false;
        }

        /// <summary>
        /// Select file name event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void requestCertificateFilePath_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            saveFileDialogMain.Filter = "Certificate Request Files (*.crt *.cer *.pem)|*.crt;*.cer;*.pem";

            // Get the file name selected.
            if (saveFileDialogMain.ShowDialog() == DialogResult.OK)
                textRequestCertificateFilePath.Text = saveFileDialogMain.FileName;
        }

        /// <summary>
        /// Create the new certificate request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRequestCertificate_Click(object sender, EventArgs e)
        {
            // Create a new instance of the openssl certificate implementation
            Nequeo.Security.OpenSsl.CertificateConfiguration certificate = new Nequeo.Security.OpenSsl.CertificateConfiguration();

            // If multi domain checked then create a single instance certificate
            // else create a new multi domain certificate.
            if (!checkBoxRequestMultDomain.Checked)
                certificate.RequestCertificate(RequestSubject(), textRequestCertificatePassword.Text, textRequestCertificateFilePath.Text);
            else
                certificate.RequestCertificateMultiDomain(RequestSubject(), textRequestCertificatePassword.Text, textRequestCertificateFilePath.Text);
        }

        /// <summary>
        /// Certificate path and file name text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textRequestCertificateFilePath_TextChanged(object sender, EventArgs e)
        {
            CreateRequestCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Country name text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textRequestCountryName_TextChanged(object sender, EventArgs e)
        {
            CreateRequestCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// State text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textRequestState_TextChanged(object sender, EventArgs e)
        {
            CreateRequestCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Location name text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textRequestLocationName_TextChanged(object sender, EventArgs e)
        {
            CreateRequestCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Orgainisation name text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textRequestOrganisationName_TextChanged(object sender, EventArgs e)
        {
            CreateRequestCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Orgination unit name text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textRequestOrganisationUnitName_TextChanged(object sender, EventArgs e)
        {
            CreateRequestCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Common name text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textRequestCommonName_TextChanged(object sender, EventArgs e)
        {
            CreateRequestCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Email address text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textRequestEmailAddress_TextChanged(object sender, EventArgs e)
        {
            CreateRequestCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Certificate password text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textRequestCertificatePassword_TextChanged(object sender, EventArgs e)
        {
            CreateRequestCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Select file name event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSignCertificateFilePath_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            saveFileDialogMain.Filter = "Certificate Files (*.crt *.cer *.pem)|*.crt;*.cer;*.pem";

            // Get the file name selected.
            if (saveFileDialogMain.ShowDialog() == DialogResult.OK)
                textSignCertificateFilePath.Text = saveFileDialogMain.FileName;
        }

        /// <summary>
        /// Select file name event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSignCertificateRequestFilePath_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            openFileDialogMain.Filter = "Certificate Request Files (*.crt *.cer *.pem)|*.crt;*.cer;*.pem";

            // Get the file name selected.
            if (openFileDialogMain.ShowDialog() == DialogResult.OK)
                textSignCertificateRequestFilePath.Text = openFileDialogMain.FileName;
        }

        /// <summary>
        /// Certificate authority password change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textSignCertificateAuthorityPassword_TextChanged(object sender, EventArgs e)
        {
            CreateSignCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Certificate request file path change event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textSignCertificateRequestFilePath_TextChanged(object sender, EventArgs e)
        {
            CreateSignCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Certificate file path change event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textSignCertificateFilePath_TextChanged(object sender, EventArgs e)
        {
            CreateSignCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Signs the new certificate request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSignCertificate_Click(object sender, EventArgs e)
        {
            // Create a new instance of the openssl certificate implementation
            Nequeo.Security.OpenSsl.CertificateConfiguration certificate = new Nequeo.Security.OpenSsl.CertificateConfiguration();

            // If multi domain checked then create a single instance certificate
            // else create a new multi domain certificate.
            if (!checkBoxSignMultiDomian.Checked)
                certificate.SignCertificate(textSignCertificateAuthorityPassword.Text, textSignCertificateRequestFilePath.Text, textSignCertificateFilePath.Text);
            else
                certificate.SignCertificateMultiDomain(textSignCertificateAuthorityPassword.Text, textSignCertificateRequestFilePath.Text, textSignCertificateFilePath.Text);
        }

        /// <summary>
        /// Create a new certificate authority
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCACreateCertificateAuthority_Click(object sender, EventArgs e)
        {
            // Create a new instance of the openssl certificate implementation
            Nequeo.Security.OpenSsl.CertificateConfiguration certificate = new Nequeo.Security.OpenSsl.CertificateConfiguration();

            // Create the new certificate authority
            certificate.CreateCertificateAuthority(
                CertificateAuthoritySubject(), 
                textCACertificatePassword.Text, 
                textCACertificatePrivateKeyPath.Text, 
                textCACertificatePath.Text, Int32.Parse(textCADays.Text));
        }

        /// <summary>
        /// Select the certificate authority path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCASelectCertificatePath_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            saveFileDialogMain.Filter = "Certificate Files (*.crt *.cer *.pem)|*.crt;*.cer;*.pem";

            // Get the file name selected.
            if (saveFileDialogMain.ShowDialog() == DialogResult.OK)
                textCACertificatePath.Text = saveFileDialogMain.FileName;
        }

        /// <summary>
        /// Select the certificate authory private key path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCASelectCertificatePrivateKeyPath_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            saveFileDialogMain.Filter = "Certificate Files (*.crt *.cer *.pem)|*.crt;*.cer;*.pem";

            // Get the file name selected.
            if (saveFileDialogMain.ShowDialog() == DialogResult.OK)
                textCACertificatePrivateKeyPath.Text = saveFileDialogMain.FileName;
        }

        /// <summary>
        /// Country name text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textCACountryName_TextChanged(object sender, EventArgs e)
        {
            CreateCACertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// State text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textCAState_TextChanged(object sender, EventArgs e)
        {
            CreateCACertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Location name text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textCALocationName_TextChanged(object sender, EventArgs e)
        {
            CreateCACertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Organisation name text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textCAOrganisationName_TextChanged(object sender, EventArgs e)
        {
            CreateCACertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Organisation unit name text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textCAOrganisationUnitName_TextChanged(object sender, EventArgs e)
        {
            CreateCACertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Common name text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textCACommonName_TextChanged(object sender, EventArgs e)
        {
            CreateCACertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Email address text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textCAEmailAddress_TextChanged(object sender, EventArgs e)
        {
            CreateCACertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Certificate password text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textCACertificatePassword_TextChanged(object sender, EventArgs e)
        {
            CreateCACertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Certificate path text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textCACertificatePath_TextChanged(object sender, EventArgs e)
        {
            CreateCACertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Certificate private key path text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textCACertificatePrivateKeyPath_TextChanged(object sender, EventArgs e)
        {
            CreateCACertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Certificate valid number of days text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textCADays_TextChanged(object sender, EventArgs e)
        {
            CreateCACertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Certificate valid number key down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textCADays_KeyDown(object sender, KeyEventArgs e)
        {
            // If the key pressed is a valid number
            // then allow the key to be visible
            // else do not display the key.
            if ((e.KeyValue > 47 && e.KeyValue < 58) ||
                (e.KeyValue > 95 && e.KeyValue < 106))
                e.SuppressKeyPress = false;
            else
                e.SuppressKeyPress = true;

            // If the back space or delete
            // key is pressed then allow.
            if (e.KeyCode == Keys.Back ||
                e.KeyCode == Keys.Delete ||
                e.KeyCode == Keys.Left ||
                e.KeyCode == Keys.Right)

                e.SuppressKeyPress = false;
        }

        /// <summary>
        /// Remove certificate password text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textRemoveCertificatePassword_TextChanged(object sender, EventArgs e)
        {
            CreateRemoveCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Remove certificate password path text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textRemoveCertificatePath_TextChanged(object sender, EventArgs e)
        {
            CreateRemoveCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Remove certificate password decrypted path text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textRemoveCertificateDecryptedPath_TextChanged(object sender, EventArgs e)
        {
            CreateRemoveCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Remove certificate password.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemovePassword_Click(object sender, EventArgs e)
        {
            // Create a new instance of the openssl certificate implementation
            Nequeo.Security.OpenSsl.CertificateConfiguration certificate = new Nequeo.Security.OpenSsl.CertificateConfiguration();

            // Remove certificate password.
            certificate.RemoveCertificatePassword(
                textRemoveCertificatePassword.Text,
                textRemoveCertificatePath.Text,
                textRemoveCertificateDecryptedPath.Text);
        }

        /// <summary>
        /// Remove certificate password path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveCertificatePath_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            openFileDialogMain.Filter = "Certificate Files (*.crt *.cer *.pem)|*.crt;*.cer;*.pem";

            // Get the file name selected.
            if (openFileDialogMain.ShowDialog() == DialogResult.OK)
                textRemoveCertificatePath.Text = openFileDialogMain.FileName;
        }

        /// <summary>
        /// Remove certificate password decrypted path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveCertificateDecryptedPath_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            saveFileDialogMain.Filter = "Certificate Files (*.crt *.cer *.pem)|*.crt;*.cer;*.pem";

            // Get the file name selected.
            if (saveFileDialogMain.ShowDialog() == DialogResult.OK)
                textRemoveCertificateDecryptedPath.Text = saveFileDialogMain.FileName;
        }

        /// <summary>
        /// Certificate details path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCertificateDetailsPath_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            openFileDialogMain.Filter = "Personal Information Exchange Certificate Files (*.pfx *.p12)|*.pfx;*.p12|Certificate Files (*.crt *.cer *.pem)|*.crt;*.cer;*.pem";

            // Get the file name selected.
            if (openFileDialogMain.ShowDialog() == DialogResult.OK)
                txtCertificateDetailsPath.Text = openFileDialogMain.FileName;
        }

        /// <summary>
        /// Load the certificate details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCertificateDetailsLoad_Click(object sender, EventArgs e)
        {
            X509Certificate2 certificate = null;

            try
            {
                // Get the certificate.
                if (String.IsNullOrEmpty(txtCertificateDetailsPassword.Text))
                    certificate = Nequeo.Security.X509Certificate2Store.GetCertificate(txtCertificateDetailsPath.Text);
                else
                    certificate = Nequeo.Security.X509Certificate2Store.GetCertificate(txtCertificateDetailsPath.Text, txtCertificateDetailsPassword.Text);

                // Write the certificate data.
                richTextBoxCertificateDetails.Text = String.Format
                    (
                        "Issued To : {0}\r\n\r\n" +
                        "Issued By : {1}\r\n\r\n" +
                        "Friendly Name : {2}\r\n\r\n" +
                        "Serial# : {3}\r\n\r\n" +
                        "From : {4}, To : {5}\r\n\r\n" +
                        "Key Algorithm : {6}, Params : {7}\r\n\r\n" +
                        "Format : {8}\r\n\r\n" +
                        "Version : {9}\r\n\r\n" +
                        "Signature Algorithm Name : {10}\r\n\r\n" +
                        "Has Private Key : {11}\r\n\r\n" +
                        "Certificate Hash (Thumbprint) : {12}\r\n\r\n" +
                        "Public Key Name : {13}\r\n\r\n" +
                        "Public Key : \r\n{14}\r\n\r\n" +
                        "Private Key : \r\n{15}",
                        certificate.Subject,
                        certificate.Issuer,
                        certificate.FriendlyName,
                        certificate.GetSerialNumberString(),
                        certificate.GetEffectiveDateString(), certificate.GetExpirationDateString(),
                        certificate.GetKeyAlgorithm(), certificate.GetKeyAlgorithmParametersString(),
                        certificate.GetFormat(),
                        certificate.Version.ToString(),
                        certificate.SignatureAlgorithm.FriendlyName,
                        certificate.HasPrivateKey.ToString(),
                        certificate.GetCertHashString(),
                        certificate.PublicKey.Oid.FriendlyName,
                        certificate.GetPublicKeyString(),
                        (certificate.HasPrivateKey ? certificate.PrivateKey.ToXmlString(false) : "None")
                    );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Certificate Details", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Certificate details path changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCertificateDetailsPath_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtCertificateDetailsPath.Text.Trim()))
            {
                btnCertificateDetailsLoad.Enabled = false;
                btnCertificateDetailsView.Enabled = false;
            }
            else
            {
                btnCertificateDetailsLoad.Enabled = true;
                btnCertificateDetailsView.Enabled = true;
            }
        }

        /// <summary>
        /// Extract certificate key text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtExtractCertificatePassword_TextChanged(object sender, EventArgs e)
        {
            ExtractPublicKeyCertificateContainsText((TextBox)sender);
            ExtractPrivateKeyCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Extract certificate key text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtExtractExportPassword_TextChanged(object sender, EventArgs e)
        {
            ExtractPublicKeyCertificateContainsText((TextBox)sender);
            ExtractPrivateKeyCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Extract certificate key text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtExtractCertificatePath_TextChanged(object sender, EventArgs e)
        {
            ExtractPublicKeyCertificateContainsText((TextBox)sender);
            ExtractPrivateKeyCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Extract certificate key text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtExtractPublicKeyPath_TextChanged(object sender, EventArgs e)
        {
            ExtractPublicKeyCertificateContainsText((TextBox)sender);
            ExtractPrivateKeyCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Extract certificate key text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtExtractPrivateKeyPath_TextChanged(object sender, EventArgs e)
        {
            ExtractPublicKeyCertificateContainsText((TextBox)sender);
            ExtractPrivateKeyCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Extract certificate path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExtractCertificatePath_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            openFileDialogMain.Filter = "Personal Information Exchange Certificate Files (*.pfx *.p12)|*.pfx;*.p12|Certificate Files (*.crt *.cer *.pem)|*.crt;*.cer;*.pem";

            // Get the file name selected.
            if (openFileDialogMain.ShowDialog() == DialogResult.OK)
                txtExtractCertificatePath.Text = openFileDialogMain.FileName;
        }

        /// <summary>
        /// Extract public key location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExtractPublicKeyPath_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            saveFileDialogMain.Filter = "Txt Files (*.txt)|*.txt|All Files (*.*)|*.*";

            // Get the file name selected.
            if (saveFileDialogMain.ShowDialog() == DialogResult.OK)
                txtExtractPublicKeyPath.Text = saveFileDialogMain.FileName;
        }

        /// <summary>
        /// Extract private key location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExtractPrivateKeyPath_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            saveFileDialogMain.Filter = "Txt Files (*.txt)|*.txt|All Files (*.*)|*.*";

            // Get the file name selected.
            if (saveFileDialogMain.ShowDialog() == DialogResult.OK)
                txtExtractPrivateKeyPath.Text = saveFileDialogMain.FileName;
        }

        /// <summary>
        /// Extract public key.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExtractPublicKey_Click(object sender, EventArgs e)
        {
            // Create a new instance of the openssl certificate implementation
            Nequeo.Security.OpenSsl.CertificateConfiguration certificate = new Nequeo.Security.OpenSsl.CertificateConfiguration();

            // Create the new certificate authority
            certificate.ExtractCertificatePublicKey(
                txtExtractCertificatePassword.Text, 
                txtExtractCertificatePath.Text, 
                txtExtractPublicKeyPath.Text);
        }

        /// <summary>
        /// Extract private key.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExtractPrivateKey_Click(object sender, EventArgs e)
        {
            // Create a new instance of the openssl certificate implementation
            Nequeo.Security.OpenSsl.CertificateConfiguration certificate = new Nequeo.Security.OpenSsl.CertificateConfiguration();

            // Create the new certificate authority
            certificate.ExtractCertificatePrivateKey(
                txtExtractCertificatePassword.Text, 
                txtExtractExportPassword.Text, 
                txtExtractCertificatePath.Text, 
                txtExtractPrivateKeyPath.Text);
        }

        /// <summary>
        /// Revoke certificate CA password changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtRevokeCertificateAuthPassword_TextChanged(object sender, EventArgs e)
        {
            RevokeListCertificateContainsText((TextBox)sender);
            RevokeCertificateContainsText((TextBox)sender);
            RevokeUpdateDatabaseCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Revoke certifcate path changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtRevokeCertificatePath_TextChanged(object sender, EventArgs e)
        {
            RevokeCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Revoke certificate list changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtRevokeCertificateListPath_TextChanged(object sender, EventArgs e)
        {
            RevokeListCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Revoke list changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtRevokeListPath_TextChanged(object sender, EventArgs e)
        {
            RevokeListCertificateContainsText((TextBox)sender);
        }

        /// <summary>
        /// Revoke certificate path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevokeCertificatePath_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            openFileDialogMain.Filter = "Certificate Files (*.pem)|*.pem";

            // Get the file name selected.
            if (openFileDialogMain.ShowDialog() == DialogResult.OK)
                txtRevokeCertificatePath.Text = openFileDialogMain.FileName;
        }

        /// <summary>
        /// Revoke certificate list path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevokeCertificateListPath_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            saveFileDialogMain.Filter = "Certificate Files (*.pem)|*.pem";

            // Get the file name selected.
            if (saveFileDialogMain.ShowDialog() == DialogResult.OK)
                txtRevokeCertificateListPath.Text = saveFileDialogMain.FileName;
        }

        /// <summary>
        /// Revoke list path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevokeListPath_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            saveFileDialogMain.Filter = "Certificate Revoke List Files (*.crl)|*.crl";

            // Get the file name selected.
            if (saveFileDialogMain.ShowDialog() == DialogResult.OK)
                txtRevokeListPath.Text = saveFileDialogMain.FileName;
        }

        /// <summary>
        /// Revoke certificate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevokeCertificate_Click(object sender, EventArgs e)
        {
            // Create a new instance of the openssl certificate implementation
            Nequeo.Security.OpenSsl.CertificateConfiguration certificate = new Nequeo.Security.OpenSsl.CertificateConfiguration();

            // Create the new certificate authority
            certificate.RevokeCertificate(txtRevokeCertificateAuthPassword.Text, txtRevokeCertificatePath.Text);
        }

        /// <summary>
        /// Revoke list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevokeList_Click(object sender, EventArgs e)
        {
            // Create a new instance of the openssl certificate implementation
            Nequeo.Security.OpenSsl.CertificateConfiguration certificate = new Nequeo.Security.OpenSsl.CertificateConfiguration();

            // Create the new certificate authority
            certificate.CreateRevokationList(txtRevokeCertificateAuthPassword.Text, txtRevokeCertificateListPath.Text, txtRevokeListPath.Text);
        }

        /// <summary>
        /// Revoke update database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevokeUpdateDatabase_Click(object sender, EventArgs e)
        {
            // Create a new instance of the openssl certificate implementation
            Nequeo.Security.OpenSsl.CertificateConfiguration certificate = new Nequeo.Security.OpenSsl.CertificateConfiguration();

            // Create the new certificate authority
            certificate.UpdateDatabaseCertificate(txtRevokeCertificateAuthPassword.Text);
        }

        /// <summary>
        /// View the certificate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCertificateDetailsView_Click(object sender, EventArgs e)
        {
            X509Certificate2 certificate = null;

            try
            {
                // Get the certificate.
                if (String.IsNullOrEmpty(txtCertificateDetailsPassword.Text))
                    certificate = Nequeo.Security.X509Certificate2Store.GetCertificate(txtCertificateDetailsPath.Text);
                else
                    certificate = Nequeo.Security.X509Certificate2Store.GetCertificate(txtCertificateDetailsPath.Text, txtCertificateDetailsPassword.Text);

                // Displays a dialog box that contains the properties of an X.509 certificate
                // and its associated certificate chain using a handle to a parent window.
                Nequeo.Security.X509Certificate2Store.DisplayCertificate(certificate);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Certificate Details", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Private key path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenPublicPrivatePairPrivateKey_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            saveFileDialogMain.Filter = "Certificate Files (*.pem)|*.pem|All Files (*.*)|*.*";

            // Get the file name selected.
            if (saveFileDialogMain.ShowDialog() == DialogResult.OK)
                txtGenPublicPrivatePairPrivateKey.Text = saveFileDialogMain.FileName;
        }

        /// <summary>
        /// Public key path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenPublicPrivatePairPublicKey_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            saveFileDialogMain.Filter = "Certificate Files (*.pem)|*.pem|All Files (*.*)|*.*";

            // Get the file name selected.
            if (saveFileDialogMain.ShowDialog() == DialogResult.OK)
                txtGenPublicPrivatePairSizePublicKey.Text = saveFileDialogMain.FileName;
        }

        /// <summary>
        /// Text change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGenPublicPrivatePairSize_TextChanged(object sender, EventArgs e)
        {
            GeneratePublicPrivateKeyPair((TextBox)sender);
        }

        /// <summary>
        /// Text change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGenPublicPrivatePairPrivateKey_TextChanged(object sender, EventArgs e)
        {
            GeneratePublicPrivateKeyPair((TextBox)sender);
        }

        /// <summary>
        /// Text change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGenPublicPrivatePairSizePublicKey_TextChanged(object sender, EventArgs e)
        {
            GeneratePublicPrivateKeyPair((TextBox)sender);
        }

        /// <summary>
        /// Generate a public private key pair.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenPublicPrivatePairPublicGo_Click(object sender, EventArgs e)
        {
            // Create a new instance of the openssl certificate implementation
            Nequeo.Security.OpenSsl.CertificateConfiguration certificate = new Nequeo.Security.OpenSsl.CertificateConfiguration();

            try
            {
                // Create the public private key pair
                certificate.CreatePublicPrivateKeyPair(txtGenPublicPrivatePairSizePublicKey.Text, txtGenPublicPrivatePairPrivateKey.Text, Int32.Parse(txtGenPublicPrivatePairSize.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Public Private Key Pair", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Enable or disable the generate public private key pair to valid input data.
        /// </summary>
        /// <param name="textBox">The current text box instance</param>
        private void GeneratePublicPrivateKeyPair(TextBox textBox)
        {
            bool isValid = true;

            // If no text has been add then invalid.
            if (String.IsNullOrEmpty(textBox.Text))
                isValid = false;

            // Add the current validation item.
            if (!_validGenPublicPrivateKeyPairCertificate.Keys.Contains(textBox.GetHashCode()))
                _validGenPublicPrivateKeyPairCertificate.Add(textBox.GetHashCode(), isValid);
            else
                _validGenPublicPrivateKeyPairCertificate[textBox.GetHashCode()] = isValid;

            // Find all invalid items, if no invalid items then
            // enable operation controls.
            IEnumerable<bool> inValid = _validGenPublicPrivateKeyPairCertificate.Values.Where(u => (u == false));
            if ((inValid.Count() < 1))
                btnGenPublicPrivatePairPublicGo.Enabled = true;
            else
                btnGenPublicPrivatePairPublicGo.Enabled = false;
        }

        /// <summary>
        /// Enable or disable the generate personal certificate to valid input data.
        /// </summary>
        /// <param name="textBox">The current text box instance</param>
        private void GeneratePersonalCertificate(TextBox textBox)
        {
            bool isValid = true;

            // If no text has been add then invalid.
            if (String.IsNullOrEmpty(textBox.Text))
                isValid = false;

            // Add the current validation item.
            if (!_validPersonalCertificate.Keys.Contains(textBox.GetHashCode()))
                _validPersonalCertificate.Add(textBox.GetHashCode(), isValid);
            else
                _validPersonalCertificate[textBox.GetHashCode()] = isValid;

            // Find all invalid items, if no invalid items then
            // enable operation controls.
            IEnumerable<bool> inValid = _validPersonalCertificate.Values.Where(u => (u == false));
            if ((inValid.Count() < 1))
                psGenCertificate.Enabled = true;
            else
                psGenCertificate.Enabled = false;
        }

        /// <summary>
        /// Personal issuer singer private key file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void piSignerPrivateKeySelect_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            openFileDialogMain.Filter = "Signer Private Key Files (*.pem)|*.pem";

            // Get the file name selected.
            if (openFileDialogMain.ShowDialog() == DialogResult.OK)
                piSignerPrivateFilePath.Text = openFileDialogMain.FileName;
        }

        /// <summary>
        /// Personal issuer singer certificate file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void piSignerCertificateSelect_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            openFileDialogMain.Filter = "Signer Certificate Files (*.pem)|*.pem";

            // Get the file name selected.
            if (openFileDialogMain.ShowDialog() == DialogResult.OK)
                piCertificateFilePath.Text = openFileDialogMain.FileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void piCountryName_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void piState_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void piLocationName_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void piOrganidationName_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void piOrganidationUnitName_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void piCommonName_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void piEmailAddress_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void piPassword_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void piSignerPrivateFilePath_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void piCertificateFilePath_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void psCertificatePathSelect_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            saveFileDialogMain.Filter = "Certificate Files (*.crt *.cer *.pem)|*.crt;*.cer;*.pem";

            // Get the file name selected.
            if (saveFileDialogMain.ShowDialog() == DialogResult.OK)
                psCertificatePath.Text = saveFileDialogMain.FileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void psPIEPathSelect_Click(object sender, EventArgs e)
        {
            // Set the certificate filter.
            saveFileDialogMain.Filter = "PIE Certificate Files (*.p12 *.pfx)|*.p12;*.pfx";

            // Get the file name selected.
            if (saveFileDialogMain.ShowDialog() == DialogResult.OK)
                psPIEPath.Text = saveFileDialogMain.FileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void psCountryName_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void psState_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void psLocationName_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void psOrganisationName_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void psOrganisationUnitName_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void psCommonName_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void psEmailAddress_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void psPassword_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void psCertificatePath_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void psPIEPath_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void psValidDays_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void psKeySize_TextChanged(object sender, EventArgs e)
        {
            GeneratePersonalCertificate((TextBox)sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void psValidDays_KeyDown(object sender, KeyEventArgs e)
        {
            // If the key pressed is a valid number
            // then allow the key to be visible
            // else do not display the key.
            if ((e.KeyValue > 47 && e.KeyValue < 58) ||
                (e.KeyValue > 95 && e.KeyValue < 106))
                e.SuppressKeyPress = false;
            else
                e.SuppressKeyPress = true;

            // If the back space or delete
            // key is pressed then allow.
            if (e.KeyCode == Keys.Back ||
                e.KeyCode == Keys.Delete ||
                e.KeyCode == Keys.Left ||
                e.KeyCode == Keys.Right)

                e.SuppressKeyPress = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void psKeySize_KeyDown(object sender, KeyEventArgs e)
        {
            // If the key pressed is a valid number
            // then allow the key to be visible
            // else do not display the key.
            if ((e.KeyValue > 47 && e.KeyValue < 58) ||
                (e.KeyValue > 95 && e.KeyValue < 106))
                e.SuppressKeyPress = false;
            else
                e.SuppressKeyPress = true;

            // If the back space or delete
            // key is pressed then allow.
            if (e.KeyCode == Keys.Back ||
                e.KeyCode == Keys.Delete ||
                e.KeyCode == Keys.Left ||
                e.KeyCode == Keys.Right)

                e.SuppressKeyPress = false;
        }

        /// <summary>
        /// Selected index changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void psSignatureAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Select the signature algorithm.
            switch (psSignatureAlgorithm.SelectedIndex)
            {
                case 0:
                    _signatureAlgorithm = Cryptography.Signing.SignatureAlgorithm.SHA1withRSA;
                    break;
                case 1:
                    _signatureAlgorithm = Cryptography.Signing.SignatureAlgorithm.SHA224withRSA;
                    break;
                case 2:
                    _signatureAlgorithm = Cryptography.Signing.SignatureAlgorithm.SHA256withRSA;
                    break;
                case 3:
                    _signatureAlgorithm = Cryptography.Signing.SignatureAlgorithm.SHA384withRSA;
                    break;
                case 4:
                default:
                    _signatureAlgorithm = Cryptography.Signing.SignatureAlgorithm.SHA512withRSA;
                    break;
            }
        }

        /// <summary>
        /// Create the personal certificate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void psGenCertificate_Click(object sender, EventArgs e)
        {
            System.IO.StreamReader caPublicReader = null;
            System.IO.StreamReader caPrivateReader = null;

            System.IO.FileStream certificateStream = null;
            System.IO.FileStream pfxStream = null;

            try
            {
                // Get the signer RSA public and private key data.
                caPublicReader = new System.IO.StreamReader(piCertificateFilePath.Text);
                caPrivateReader = new System.IO.StreamReader(piSignerPrivateFilePath.Text);
                RSACryptoServiceProvider caPublicSigner = new Nequeo.Cryptography.Openssl.CertificateAuthorityKey().PublicKeyProvider(caPublicReader);
                RSACryptoServiceProvider caPrivateSigner = new Nequeo.Cryptography.Openssl.CertificateAuthorityKey().PrivateKeyProvider(caPrivateReader, piPassword.Text);
            
                // Get the issuer.
                Nequeo.Cryptography.Openssl.Subject issuer = new Nequeo.Cryptography.Openssl.Subject(
                    piCountryName.Text,
                    piState.Text,
                    piLocationName.Text,
                    piOrganidationName.Text,
                    piOrganidationUnitName.Text,
                    piCommonName.Text,
                    piEmailAddress.Text);

                // Get the subject
                Nequeo.Cryptography.Openssl.Subject subject = new Nequeo.Cryptography.Openssl.Subject(
                    psCountryName.Text,
                    psState.Text,
                    psLocationName.Text,
                    psOrganisationName.Text,
                    psOrganisationUnitName.Text,
                    psCommonName.Text,
                    psEmailAddress.Text);

                // Get a random serial number.
                string serialNumberValue = new Nequeo.Invention.NumberGenerator().Random(15);
                long serialNumber = Int64.Parse(serialNumberValue);

                // Create the certificate.
                Nequeo.Cryptography.Openssl.Certificate cert = new Nequeo.Cryptography.Openssl.Certificate();
                X509Certificate2 cert2 = cert.Generate(caPrivateSigner, caPublicSigner, serialNumber,
                    issuer,
                    subject,
                    DateTime.UtcNow.AddDays(-1),
                    DateTime.UtcNow.AddDays(double.Parse(psValidDays.Text)),
                    strength: Int32.Parse(psKeySize.Text), 
                    signatureAlgorithm: _signatureAlgorithm
                );

                // Save the certificates.
                pfxStream = System.IO.File.Create(psPIEPath.Text);
                certificateStream = System.IO.File.Create(psCertificatePath.Text);

                // Write the data.
                byte[] data = cert2.Export(X509ContentType.Cert);
                certificateStream.Write(data, 0, data.Length);
                cert.ExportCertificatePkcs(pfxStream, psPassword.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Certificate Cryptography Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (caPublicReader != null)
                    caPublicReader.Close();

                if (caPrivateReader != null)
                    caPrivateReader.Close();

                if (certificateStream != null)
                    certificateStream.Close();

                if (pfxStream != null)
                    pfxStream.Close();
            }
        }
    }
}
