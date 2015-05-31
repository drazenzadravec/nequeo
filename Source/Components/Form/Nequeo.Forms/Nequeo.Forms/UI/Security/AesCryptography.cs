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
using System.Windows.Forms;

using Nequeo.Cryptography;
using Nequeo.Threading;
using Nequeo.Custom;

namespace Nequeo.Forms.UI.Security
{
    /// <summary>
    /// AES cryptography control
    /// </summary>
    public partial class AesCryptography : UserControl
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public AesCryptography()
        {
            InitializeComponent();
        }

        private volatile string _errorMessage = string.Empty;
        private CryptographyType _cryto = CryptographyType.None;
        private bool _executingCrypt = false;
        private string _cryptoKey = string.Empty;

        /// <summary>
        /// Sets the Crytography Key.
        /// </summary>
        public string CrytographyKey
        {
            set { _cryptoKey = value; }
        }

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
        private void AesCryptography_Load(object sender, EventArgs e)
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
                    break;

                default:
                    // Enable the execute control button.
                    btnCryptExecute.Enabled = true;
                    btnEncryptLocalFile.Enabled = true;
                    btnDecryptLocalFile.Enabled = true;
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
        /// On execute clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCryptExecute_Click(object sender, EventArgs e)
        {
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

                if (!String.IsNullOrEmpty(_cryptoKey))
                    CryptExecute(txtDecryptedLocalFile.Text, txtEncryptLocalFile.Text, txtCryptPassword.Text.Trim());
                else
                    CryptExecuteEx(txtDecryptedLocalFile.Text, txtEncryptLocalFile.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Cryto execute.
        /// </summary>
        /// <param name="decryptFile">The decrypt file.</param>
        /// <param name="encryptFile">The encrypt file.</param>
        /// <param name="password">The password file.</param>
        private async void CryptExecute(string decryptFile, string encryptFile, string password = "")
        {
            // Set the execute stop button.
            ExecuteStopCrypt();

            // Start a new task.
            await Nequeo.Threading.AsyncOperationResult<bool>.RunTask(() =>
            {
                string decryptFileInt = "";
                string encryptFileInt = "";
                bool ret = false;

                try
                {
                    // Create a new AES cryto.
                    using (AdvancedAES aes = new AdvancedAES())
                    {
                        decryptFileInt = decryptFile.Trim() + ".partial";
                        encryptFileInt = encryptFile.Trim() + ".partial";

                        // Get the cryptography operation.
                        switch (_cryto)
                        {
                            case CryptographyType.Encrypt:
                                // Cryto with the password.
                                if (String.IsNullOrEmpty(password))
                                    ret = aes.EncryptFile(decryptFile, encryptFileInt);
                                else
                                    ret = aes.EncryptFile(decryptFile, encryptFileInt, password);

                                // Encrypt with key.
                                if (ret)
                                    ret = aes.EncryptFile(encryptFileInt.Trim(), encryptFile.Trim(), _cryptoKey);

                                // Encryption complete.
                                SetEncrypting(ret);
                                break;

                            case CryptographyType.Decrypt:
                                // Decrypt with key.
                                ret = aes.DecryptFile(decryptFileInt.Trim(), encryptFile.Trim(), _cryptoKey);

                                // Cryto with the password.
                                if (ret)
                                {
                                    if (String.IsNullOrEmpty(password))
                                        ret = aes.DecryptFile(decryptFile, decryptFileInt);
                                    else
                                        ret = aes.DecryptFile(decryptFile, decryptFileInt, password);
                                }

                                // Decryption complete.
                                SetDecrypting(ret);
                                break;
                        }
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
                finally
                {
                    try
                    {
                        // Delete the encrypted partial file.
                        if (System.IO.File.Exists(encryptFileInt))
                            System.IO.File.Delete(encryptFileInt);
                    }
                    catch { }

                    try
                    {
                        // Delete the decrypted partial file.
                        if (System.IO.File.Exists(decryptFileInt))
                            System.IO.File.Delete(decryptFileInt);
                    }
                    catch { }
                }
            });
        }

        /// <summary>
        /// Cryto execute.
        /// </summary>
        /// <param name="decryptFile">The decrypt file.</param>
        /// <param name="encryptFile">The encrypt file.</param>
        private void CryptExecuteEx(string decryptFile, string encryptFile)
        {
            // Create a new async advanced cryptography
            // class, for file encryption.
            AsynchronousAdvancedAES asyncCrypt = new AsynchronousAdvancedAES();

            // Set the execute stop button.
            ExecuteStopCrypt();

            try
            {
                // Get the cryptography operation.
                switch (_cryto)
                {
                    case CryptographyType.Encrypt:
                        // Encrypt the current file to the specified file.
                        // If no password has been specified then encrypt
                        // with no password.
                        if (String.IsNullOrEmpty(txtCryptPassword.Text.Trim()))
                            asyncCrypt.BeginEncryptFile(decryptFile.Trim(), encryptFile.Trim(),
                                new AsyncCallback(EncryptFile), asyncCrypt);
                        else
                            // Encrypt with the password specified.
                            asyncCrypt.BeginEncryptFile(decryptFile.Trim(), encryptFile.Trim(),
                                txtCryptPassword.Text.Trim(), new AsyncCallback(EncryptFile), asyncCrypt);
                        break;

                    case CryptographyType.Decrypt:
                        // Decrypt the current file to the specified file.
                        // If no password has been specified then decrypt
                        // with no password.
                        if (String.IsNullOrEmpty(txtCryptPassword.Text.Trim()))
                            asyncCrypt.BeginDecryptFile(decryptFile.Trim(), encryptFile.Trim(),
                                new AsyncCallback(DecryptFile), asyncCrypt);
                        else
                            // Decrypt with the password specified.
                            asyncCrypt.BeginDecryptFile(decryptFile.Trim(), encryptFile.Trim(),
                                txtCryptPassword.Text.Trim(), new AsyncCallback(DecryptFile), asyncCrypt);
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
                ExecuteStopCryptEx("Execute");
            }
            // Else set to true and stop.
            else
            {
                _executingCrypt = true;
                ExecuteStopCryptEx("Stop");
            }
        }

        /// <summary>
        /// Execute stop result setter.
        /// this method handles cross-threading calls.
        /// </summary>
        /// <param name="result">The encryption result.</param>
        private void ExecuteStopCryptEx(string result)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.btnCryptExecute.InvokeRequired)
            {
                // Create a new delegate.
                CrytopExecuteCallBack decrypt = new CrytopExecuteCallBack(ExecuteStopCryptEx);

                // Execute the delegate on the current control.
                this.Invoke(decrypt, new object[] { result });
            }
            else
            {
                // Set the controls.
                // If currently executing an operation
                // then set executing to false and the
                // controls to executing.
                if (_executingCrypt)
                {
                    btnCryptExecute.Text = result;
                }
                // Else set to true and stop.
                else
                {
                    btnCryptExecute.Text = result;
                }
            }
        }

        /// <summary>
        /// This method handles the decrypt asynchronus result
        /// when a file decryption is requested.
        /// </summary>
        /// <param name="ar">The current asynchronus result.</param>
        private void DecryptFile(IAsyncResult ar)
        {
            // Get the async state object.
            AsynchronousAdvancedAES state = (AsynchronousAdvancedAES)ar.AsyncState;
            bool decrypt = false;

            try
            {
                // End the decrypt async and set
                // the cross-thread result.
                decrypt = state.EndDecryptFile(ar);
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
            SetDecrypting(decrypt);
        }

        /// <summary>
        /// This method handles the encrypt asynchronus result
        /// when a file encryption is requested.
        /// </summary>
        /// <param name="ar">The current asynchronus result.</param>
        private void EncryptFile(IAsyncResult ar)
        {
            // Get the async state object.
            AsynchronousAdvancedAES state = (AsynchronousAdvancedAES)ar.AsyncState;
            bool encrypt = false;

            try
            {
                // End the encrypt async and set
                // the cross-thread result.
                encrypt = state.EndEncryptFile(ar);
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
            SetEncrypting(encrypt);
        }

        /// <summary>
        /// Decryption result setter.
        /// this method handles cross-threading calls.
        /// </summary>
        /// <param name="result">The encryption result.</param>
        private void SetDecrypting(bool result)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.btnCryptExecute.InvokeRequired)
            {
                // Create a new delegate.
                DecryptingCallBack decrypt = new DecryptingCallBack(SetDecrypting);

                // Execute the delegate on the current control.
                this.Invoke(decrypt, new object[] { result });
            }
            else
            {
                // Set the initial message to decryption
                // failed.
                string message = "Decryption failed, the password maybe " +
                    "incorrect or the data can not be decrypted. " + _errorMessage;

                // If decryption succeeded.
                if (result)
                    message = "Decryption succeeded";

                // Set the controls.
                ExecuteStopCrypt();

                // If decryption succeeded.
                if (result)
                {
                    // On decryption complete.
                    if (OnComplete != null)
                        OnComplete(this, new ClientCommandArgs("DATA", message, 000));
                }
                else
                {
                    // On error trigger the
                    // error event handler.
                    if (OnError != null)
                        OnError(this, new ClientCommandArgs("DATA", message, 000));
                }
            }
        }

        /// <summary>
        /// Encryption result setter.
        /// this method handles cross-threading calls.
        /// </summary>
        /// <param name="result">The encryption result.</param>
        private void SetEncrypting(bool result)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.btnCryptExecute.InvokeRequired)
            {
                // Create a new delegate.
                EncryptingCallBack encrypt = new EncryptingCallBack(SetEncrypting);

                // Execute the delegate on the current control.
                this.Invoke(encrypt, new object[] { result });
            }
            else
            {
                // Set the initial message to encryption
                // failed.
                string message = "Encryption failed, the password maybe " +
                    "incorrect or the data can not be encrypted. " + _errorMessage;

                // If encryption succeeded.
                if (result)
                    message = "Encryption succeeded";

                // Set the controls.
                ExecuteStopCrypt();

                // If encryption succeeded.
                if (result)
                {
                    // On encryption complete.
                    if (OnComplete != null)
                        OnComplete(this, new ClientCommandArgs("DATA", message, 000));
                }
                else
                {
                    // On error trigger the
                    // error event handler.
                    if (OnError != null)
                        OnError(this, new ClientCommandArgs("DATA", message, 000));
                }
            }
        }
    }
}
