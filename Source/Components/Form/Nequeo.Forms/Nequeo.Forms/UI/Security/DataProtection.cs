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
using System.Security.Principal;
using System.IO;

using Nequeo.Invention;
using Nequeo.Extension;
using Nequeo.Security;
using Nequeo.Cryptography;

namespace Nequeo.Forms.UI.Security
{
    /// <summary>
    /// Data protection control.
    /// </summary>
    public partial class DataProtection : UserControl
    {
        /// <summary>
        /// Data protection control.
        /// </summary>
        public DataProtection()
        {
            InitializeComponent();
        }

        private string _currentFile = null;
        private string _currentPassword = null;
        private bool _loggedIn = false;
        private string _cryptoKey = string.Empty;

        /// <summary>
        /// Sets the Crytography Key.
        /// </summary>
        public string CrytographyKey
        {
            set { _cryptoKey = value; }
        }

        /// <summary>
        /// Application is closing handler.
        /// </summary>
        /// <param name="e">Provides data for the System.Windows.Forms.Form.FormClosing event.</param>
        public void ApplicationClosing(FormClosingEventArgs e)
        {
            SaveData(e);
        }

        /// <summary>
        /// On load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataProtection_Load(object sender, EventArgs e)
        {
            // Get the current login details.
            WindowsIdentity identity = WindowsIdentity.GetCurrent();

            // Get the length of the domain.
            // Get the starting point do not
            // include the domain.
            int length = Environment.MachineName.Length + 1;
            int startIndex = identity.Name.IndexOf(Environment.MachineName) + length;

            // Get the user name of the
            // current account.
            txtUsername.Text = identity.Name.Substring(startIndex).Replace("\\", "");
        }

        /// <summary>
        /// Cancel the operation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtUserPassword.Text = "";
            txtPassword.Text = "";
            btnLogin.Text = "Authenticate";
            _loggedIn = false;
        }

        /// <summary>
        /// Authenticate the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            LoginClient();
        }

        /// <summary>
        /// Validate the current user.
        /// </summary>
        private void LoginClient()
        {
            try
            {
                bool ret = false;
                string username = txtUsername.Text.Trim();
                string password = txtUserPassword.Text.Trim();

                // Create a new instance of the windows authentication class.
                using (WindowsAuthentication auth = new WindowsAuthentication())
                {
                    // Attach to the on authenticate
                    // event through an anonymous
                    // delegate.
                    auth.OnAuthenticate += delegate(Object s, AuthenticateArgs ae)
                    {
                        // if the user has been authenticated
                        // then the IsAuthenticated property
                        // will return true else false.
                        // Assign the ret value.
                        ret = ae.IsAuthenticated;
                    };

                    // Attempt to authenticate the user.
                    bool result = auth.AuthenticateUser(username, Environment.MachineName, password);

                    // If the user was not authenticated.
                    if (!ret)
                        MessageBox.Show("Username and or password are incorrect.");
                    else
                        LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Save the data.
        /// </summary>
        /// <param name="e">The closing state.</param>
        /// <param name="closing">Is the application closing.</param>
        private void SaveData(FormClosingEventArgs e, bool closing = true)
        {
            // Get the current password.
            _currentPassword = txtPassword.Text.Trim();

            // If no file password has been supplied.
            if (String.IsNullOrEmpty(_currentPassword) && (_loggedIn))
            {
                // Cancel the close operation.
                if (closing)
                    e.Cancel = true;

                MessageBox.Show("A valid file password must be entered.");
            }
            else if (_loggedIn)
            {
                FileStream file = null;

                try
                {
                    // If data exists
                    if (richTextBoxData.Text.Length > 0)
                    {
                        // Create a new file stream
                        // truncate the file.
                        file = new FileStream(_currentFile, FileMode.Truncate,
                             FileAccess.Write, FileShare.ReadWrite);

                        // Get the data in the data test.
                        string decryptedData = richTextBoxData.Rtf;
                        byte[] decryptedDataBytes = Encoding.Default.GetBytes(decryptedData);

                        // Create a new cryptography instance.
                        using (AdvancedAES cryto = new AdvancedAES())
                        {
                            // Encrypt the data.
                            Byte[] encryptedData = cryto.EncryptToMemory(decryptedDataBytes, _currentPassword);

                            // if using the key.
                            if (!String.IsNullOrEmpty(_cryptoKey))
                            {
                                byte[] encrytedInt = new byte[0].Combine(encryptedData);
                                encryptedData = cryto.EncryptToMemory(encrytedInt, _cryptoKey);
                            }

                            // Write the data to the file.
                            file.Write(encryptedData, 0, encryptedData.Length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);

                    if (closing)
                        e.Cancel = true;
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
            }
        }

        /// <summary>
        /// Load the encrypted data.
        /// </summary>
        private void LoadData()
        {
            // Get the current user.
            _currentPassword = txtPassword.Text.Trim();
            _currentFile = txtUsername.Text.Trim().ToLower() + ".txt";

            FileStream file = null;

            try
            {
                // if the file dose not
                // exist then create it.
                if (!File.Exists(_currentFile))
                {
                    file = File.Create(_currentFile);
                    file.Close();

                    // Enable the data container.
                    richTextBoxData.Enabled = true;
                    toolStripRichText.Enabled = true;
                    btnLogin.Text = "Re-Load";

                    // User has logged in.
                    _loggedIn = true;
                    btnCancel.Enabled = false;
                }
                else
                {
                    // Open the file.
                    file = new FileStream(_currentFile, FileMode.Open,
                         FileAccess.Read, FileShare.ReadWrite);

                    // If the file contains data.
                    if (file.Length > 0)
                    {
                        string decryptedData = "";

                        // Create a new cryptography instance.
                        using (AdvancedAES cryto = new AdvancedAES())
                        {
                            // Read the file data into
                            // the byte array.
                            Byte[] array = new byte[file.Length];
                            file.Read(array, 0, array.Length);

                            // if using the key.
                            if (!String.IsNullOrEmpty(_cryptoKey))
                            {
                                // Decrypt with key.
                                byte[] decryptedInt = cryto.DecryptFromMemory(array, _cryptoKey);
                                array = new byte[0].Combine(decryptedInt);
                            }

                            // Decrypt the data.
                            byte[] decryptedDataBytes = cryto.DecryptFromMemory(array, _currentPassword);
                            decryptedData = Encoding.Default.GetString(decryptedDataBytes);
                        }

                        // Was the data decrypted.
                        if (String.IsNullOrEmpty(decryptedData))
                            MessageBox.Show("Incorrect file password.");
                        else
                        {
                            // Enable the data container.
                            richTextBoxData.Enabled = true;
                            toolStripRichText.Enabled = true;
                            btnLogin.Text = "Re-Load";

                            // User has logged in.
                            _loggedIn = true;
                            btnCancel.Enabled = false;

                            // Load the data into the rich text.
                            richTextBoxData.Rtf = decryptedData;
                        }
                    }
                    else
                    {
                        // Enable the data container.
                        richTextBoxData.Enabled = true;
                        toolStripRichText.Enabled = true;
                        btnLogin.Text = "Re-Load";

                        // User has logged in.
                        _loggedIn = true;
                        btnCancel.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtUsername.Text) ||
                String.IsNullOrEmpty(txtPassword.Text) ||
                String.IsNullOrEmpty(txtUserPassword.Text))
            {
                btnLogin.Enabled = false;
            }
            else
            {
                btnLogin.Enabled = true;
            }
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtUserPassword_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtUsername.Text) ||
                String.IsNullOrEmpty(txtPassword.Text) ||
                String.IsNullOrEmpty(txtUserPassword.Text))
            {
                btnLogin.Enabled = false;
            }
            else
            {
                btnLogin.Enabled = true;
            }
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtUsername.Text) ||
                String.IsNullOrEmpty(txtPassword.Text) ||
                String.IsNullOrEmpty(txtUserPassword.Text))
            {
                btnLogin.Enabled = false;
            }
            else
            {
                btnLogin.Enabled = true;
            }
        }

        /// <summary>
        /// Password key press.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                if (btnLogin.Enabled)
                    LoginClient();
        }

        /// <summary>
        /// Link clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void richTextBoxData_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        /// <summary>
        /// Redo action.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonRedo_Click(object sender, EventArgs e)
        {
            richTextBoxData.Redo();
        }

        /// <summary>
        /// Undo action.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonUndo_Click(object sender, EventArgs e)
        {
            richTextBoxData.Undo();
        }

        /// <summary>
        /// Copy text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonCopy_Click(object sender, EventArgs e)
        {
            richTextBoxData.Copy();
        }

        /// <summary>
        /// Cut context.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonCut_Click(object sender, EventArgs e)
        {
            richTextBoxData.Cut();
        }

        /// <summary>
        /// Paste context.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonPaste_Click(object sender, EventArgs e)
        {
            richTextBoxData.Paste();
        }

        /// <summary>
        /// Zoom In.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonZoomIn_Click(object sender, EventArgs e)
        {
            try
            {
                // If the zoom factor is not too big
                // then zoom in more.
                if (richTextBoxData.ZoomFactor < 64)
                    richTextBoxData.ZoomFactor = richTextBoxData.ZoomFactor + 0.5f;
            }
            catch { }
        }

        /// <summary>
        /// Zoom Out.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonZoomOut_Click(object sender, EventArgs e)
        {
            try
            {
                // If the zoom factor is not too small
                // then zoom out more.
                if (richTextBoxData.ZoomFactor > (1 / 64))
                    richTextBoxData.ZoomFactor = richTextBoxData.ZoomFactor - 0.5f;
            }
            catch { }
        }

        /// <summary>
        /// Select All.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonSelectAll_Click(object sender, EventArgs e)
        {
            richTextBoxData.SelectAll();
        }

        /// <summary>
        /// Save the data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            SaveData(null, false);
        }
    }
}
