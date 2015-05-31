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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Cryptography;
using Nequeo.Threading;
using Nequeo.Custom;

namespace Nequeo.Forms.UI.Security
{
    /// <summary>
    /// RSA cryptography control
    /// </summary>
    public partial class RsaCryptography : UserControl
    {
        /// <summary>
        /// RSA cryptography control
        /// </summary>
        public RsaCryptography()
        {
            InitializeComponent();
        }

        private AdvancedRSA _asyncCrypt = null;
        private CryptographyType _cryto = CryptographyType.None;
        private bool _executingCrypt = false;

        /// <summary>
        /// On command return error event.
        /// </summary>
        public event Nequeo.Threading.EventHandler<ClientCommandArgs> OnError;

        /// <summary>
        /// On command return complete event.
        /// </summary>
        public event Nequeo.Threading.EventHandler<ClientCommandArgs> OnComplete;

        /// <summary>
        /// Control loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RsaCryptography_Load(object sender, EventArgs e)
        {
            // Fill the cyptography combo box.
            cboOperation.Items.Add("None");
            cboOperation.Items.Add("Encrypt");
            cboOperation.Items.Add("Decrypt");
            cboOperation.SelectedIndex = 0;
        }

        /// <summary>
        /// Selected index change for cryptography.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected combo box index.
            switch (cboOperation.SelectedIndex)
            {
                case 0:
                    // No operation specified.
                    _cryto = CryptographyType.None;
                    lblEncryptLocalFile.Text = "Encrypted File";
                    lblDecryptLocalFile.Text = "Decrypted File";
                    break;

                case 1:
                    // Encrypt operation specified.
                    _cryto = CryptographyType.Encrypt;
                    lblEncryptLocalFile.Text = "Encrypted File";
                    lblDecryptLocalFile.Text = "File To Encrypt";
                    break;

                case 2:
                    // Decrypt operation specified.
                    _cryto = CryptographyType.Decrypt;
                    lblEncryptLocalFile.Text = "File To Decrypt";
                    lblDecryptLocalFile.Text = "Decrypted File";
                    break;
            }

            // Get the selected combo box index.
            switch (cboOperation.SelectedIndex)
            {
                case 0:
                    // Disable the execute control button.
                    btnCryptExecute.Enabled = false;
                    btnEncryptLocalFile.Enabled = false;
                    btnDecryptLocalFile.Enabled = false;
                    btnCertificatePath.Enabled = false;
                    break;

                default:
                    // Enable the execute control button.
                    btnCryptExecute.Enabled = true;
                    btnEncryptLocalFile.Enabled = true;
                    btnDecryptLocalFile.Enabled = true;
                    btnCertificatePath.Enabled = true;
                    break;
            }
        }

        /// <summary>
        /// On encrypt file clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEncryptLocalFile_Click(object sender, EventArgs e)
        {
            switch (_cryto)
            {
                case CryptographyType.Encrypt:
                    // Encrypted file.
                    saveFileDialogMain.Filter = "All files (*.*)|*.*";
                    saveFileDialogMain.RestoreDirectory = true;

                    // Get the file name selected.
                    if (saveFileDialogMain.ShowDialog() == DialogResult.OK)
                        txtEncryptLocalFile.Text = saveFileDialogMain.FileName;
                    break;

                case CryptographyType.Decrypt:
                    // File to encrypt.
                    openFileDialogMain.InitialDirectory = "c:\\";
                    openFileDialogMain.Filter = "All files (*.*)|*.*";
                    openFileDialogMain.RestoreDirectory = true;

                    // Get the file name selected.
                    if (openFileDialogMain.ShowDialog() == DialogResult.OK)
                        txtEncryptLocalFile.Text = openFileDialogMain.FileName;
                    break;
            }
        }

        /// <summary>
        /// On decrypt file clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDecryptLocalFile_Click(object sender, EventArgs e)
        {
            switch (_cryto)
            {
                case CryptographyType.Encrypt:
                    // File to decrypt
                    openFileDialogMain.InitialDirectory = "c:\\";
                    openFileDialogMain.Filter = "All files (*.*)|*.*";
                    openFileDialogMain.RestoreDirectory = true;

                    // Get the file name selected.
                    if (openFileDialogMain.ShowDialog() == DialogResult.OK)
                        txtDecryptedLocalFile.Text = openFileDialogMain.FileName;
                    break;

                case CryptographyType.Decrypt:
                    // Decrypted file.
                    saveFileDialogMain.Filter = "All files (*.*)|*.*";
                    saveFileDialogMain.RestoreDirectory = true;

                    // Get the file name selected.
                    if (saveFileDialogMain.ShowDialog() == DialogResult.OK)
                        txtDecryptedLocalFile.Text = saveFileDialogMain.FileName;
                    break;
            }
        }

        /// <summary>
        /// On certificate file clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCertificatePath_Click(object sender, EventArgs e)
        {
            // File to decrypt
            openFileDialogMain.InitialDirectory = "c:\\";
            openFileDialogMain.Filter = "Personal Information Exchange Certificate Files (*.pfx *.p12)|*.pfx;*.p12|Certificate Files (*.crt *.cer *.pem)|*.crt;*.cer;*.pem|All files (*.*)|*.*";
            openFileDialogMain.RestoreDirectory = true;

            // Get the file name selected.
            if (openFileDialogMain.ShowDialog() == DialogResult.OK)
                txtCertificatePath.Text = openFileDialogMain.FileName;
        }

        /// <summary>
        /// On execute clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCryptExecute_Click(object sender, EventArgs e)
        {
            // Create a new async advanced cryptography
            // class, for file encryption.
            _asyncCrypt = new AdvancedRSA();

            // Set the execute stop button.
            ExecuteStopCrypt();

            try
            {
                // If no decryption file set
                // then throw and exception.
                if (String.IsNullOrEmpty(txtDecryptedLocalFile.Text.Trim()))
                    throw new Exception("Decrypt local file not specified");

                // If no decryption file set
                // then throw and exception.
                if (String.IsNullOrEmpty(txtEncryptLocalFile.Text.Trim()))
                    throw new Exception("Encrypt local file not specified");

                // If no certificate file set
                // then throw and exception.
                if (String.IsNullOrEmpty(txtCertificatePath.Text.Trim()))
                    throw new Exception("Certificate file not specified");

                // Get the cryptography operation.
                switch (_cryto)
                {
                    case CryptographyType.Encrypt:
                        // Encrypt the current file to the specified file.
                        // If no password has been specified then encrypt
                        // with no password.
                        if (String.IsNullOrEmpty(txtCertificatePassword.Text.Trim()))
                            _asyncCrypt.AdvancedRSAThreadContext.Execute<bool>(
                                u => u.Encrypt(txtDecryptedLocalFile.Text, txtEncryptLocalFile.Text, txtCertificatePath.Text),
                                "Encrypt", v => EncryptFile(v), null);
                        else
                            // Encrypt with the password specified.
                            _asyncCrypt.AdvancedRSAThreadContext.Execute<bool>(
                                u => u.Encrypt(txtDecryptedLocalFile.Text, txtEncryptLocalFile.Text, txtCertificatePath.Text, txtCertificatePassword.Text),
                                "Encrypt", v => EncryptFile(v), null);
                        break;

                    case CryptographyType.Decrypt:
                        // Decrypt the current file to the specified file.
                        // If no password has been specified then decrypt
                        // with no password.
                        if (String.IsNullOrEmpty(txtCertificatePassword.Text.Trim()))
                            _asyncCrypt.AdvancedRSAThreadContext.Execute<bool>(
                                u => u.Decrypt(txtDecryptedLocalFile.Text, txtEncryptLocalFile.Text, txtCertificatePath.Text),
                                "Decrypt", v => DecryptFile(v), null);
                        else
                            // Decrypt with the password specified.
                            _asyncCrypt.AdvancedRSAThreadContext.Execute<bool>(
                                u => u.Decrypt(txtDecryptedLocalFile.Text, txtEncryptLocalFile.Text, txtCertificatePath.Text, txtCertificatePassword.Text),
                                "Decrypt", v => DecryptFile(v), null);
                        break;
                }
            }
            catch (Exception ex)
            {
                // Set the execution stop controls.
                ExecuteStopCrypt();

                // On error trigger the
                // error event handler.
                if (OnError != null)
                    OnError(this, new ClientCommandArgs("ERROR", ex.Message, 000));
            }
        }

        /// <summary>
        /// Set the execute or stop controls.
        /// </summary>
        private void ExecuteStopCrypt()
        {
            // If currently executing an operation
            // then set executing to false and the
            // controls to executing.
            if (_executingCrypt)
            {
                _executingCrypt = false;
                btnCryptExecute.Text = "Execute";
            }
            // Else set to true and stop.
            else
            {
                _executingCrypt = true;
                btnCryptExecute.Text = "Stop";
            }
        }

        /// <summary>
        /// This method handles the encrypt asynchronus result
        /// when a file encryption is requested.
        /// </summary>
        /// <param name="result">The current asynchronus result.</param>
        private void EncryptFile(Nequeo.Threading.AsyncOperationResult<object> result)
        {
            // Get the async state object.
            bool ret = (bool)result.Result;

            try
            {
                // Set the initial message to decryption
                // failed.
                string message = "Encryption failed, the password maybe " +
                    "incorrect or the data can not be encrypted";

                // If decryption succeeded.
                if (ret)
                    message = "Encryption succeeded";

                // Set the controls.
                ExecuteStopCrypt();

                // If decryption succeeded.
                if (ret)
                {
                    // On decryption complete.
                    if (OnComplete != null)
                        OnComplete(this, new ClientCommandArgs("DATA", message, 000));
                }
                else
                {
                    if (_asyncCrypt.ExceptionAdvancedRSA != null)
                        throw new Exception(message, _asyncCrypt.ExceptionAdvancedRSA);
                }
            }
            catch (Exception ex)
            {
                // On error trigger the
                // error event handler.
                if (OnError != null)
                    OnError(this, new ClientCommandArgs("ERROR", ex.Message, 000));
            }
        }

        /// <summary>
        /// This method handles the decrypt asynchronus result
        /// when a file decryption is requested.
        /// </summary>
        /// <param name="result">The current asynchronus result.</param>
        private void DecryptFile(Nequeo.Threading.AsyncOperationResult<object> result)
        {
            // Get the async state object.
            bool ret = (bool)result.Result;

            try
            {
                // Set the initial message to decryption
                // failed.
                string message = "Decryption failed, the password maybe " +
                    "incorrect or the data can not be decrypted";

                // If decryption succeeded.
                if (ret)
                    message = "Decryption succeeded";

                // Set the controls.
                ExecuteStopCrypt();

                // If decryption succeeded.
                if (ret)
                {
                    // On decryption complete.
                    if (OnComplete != null)
                        OnComplete(this, new ClientCommandArgs("DATA", message, 000));
                }
                else
                {
                    if (_asyncCrypt.ExceptionAdvancedRSA != null)
                        throw new Exception(message, _asyncCrypt.ExceptionAdvancedRSA);
                }
            }
            catch (Exception ex)
            {
                // On error trigger the
                // error event handler.
                if (OnError != null)
                    OnError(this, new ClientCommandArgs("ERROR", ex.Message, 000));
            }
        }
    }
}
