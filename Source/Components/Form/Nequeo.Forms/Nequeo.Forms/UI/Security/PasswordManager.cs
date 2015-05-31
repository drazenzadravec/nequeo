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
using Nequeo.Serialisation;

namespace Nequeo.Forms.UI.Security
{
    /// <summary>
    /// Password manager.
    /// </summary>
    public partial class PasswordManager : UserControl
    {
        /// <summary>
        /// Password manager.
        /// </summary>
        public PasswordManager()
        {
            InitializeComponent();
        }

        private string _currentFile = null;
        private string _currentPassword = null;
        private bool _loggedIn = false;
        private Nequeo.Forms.UI.Security.Data.passwordManager _manager = null;
        private List<Nequeo.Forms.UI.Security.Data.passwordManagerItem> _items = null;
        private Nequeo.Forms.UI.Security.Data.passwordManagerItem _currentItem = null;
        private bool _showPassword = true;
        private bool _sorted = false;
        private string _cryptoKey = string.Empty;
        private short _currentFocus = -1;

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
        private void PasswordManager_Load(object sender, EventArgs e)
        {
            _items = new List<Data.passwordManagerItem>();
        }

        /// <summary>
        /// Save the data.
        /// </summary>
        /// <param name="e">The closing state.</param>
        /// <param name="closing">Is the application closing.</param>
        private void SaveData(FormClosingEventArgs e, bool closing = true)
        {
            // Get the current user.
            _currentPassword = txtPasswordFile.Text.Trim();
            _currentFile = txtFile.Text.Trim();

            // If no file password has been supplied.
            if ((String.IsNullOrEmpty(_currentPassword) || String.IsNullOrEmpty(_currentFile)) && (_loggedIn))
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
                    if (_manager != null)
                    {
                        // Assign all the current items.
                        _manager.item = _items.ToArray();

                        // Create a new file stream
                        // truncate the file.
                        file = new FileStream(_currentFile, FileMode.Truncate,
                             FileAccess.Write, FileShare.ReadWrite);

                        // Create a new cryptography instance.
                        using (AdvancedAES cryto = new AdvancedAES())
                        {
                            // Deserialise the xml file.
                            GeneralSerialisation serial = new GeneralSerialisation();
                            byte[] decryptedData = serial.Serialise(_manager, typeof(Nequeo.Forms.UI.Security.Data.passwordManager));

                            // Encrypt the data.
                            Byte[] encryptedData = cryto.EncryptToMemory(decryptedData, _currentPassword);

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
            // Disable the group box.
            groupBoxItem.Enabled = false;
            _items.Clear();
            _manager = null;

            // Get the current user.
            _currentPassword = txtPasswordFile.Text.Trim();
            _currentFile = txtFile.Text.Trim();

            FileStream file = null;

            try
            {
                // Remove all current items in the list.
                if (listBoxPasswordName.Items.Count > 0)
                    listBoxPasswordName.Items.Clear();

                // Open the file.
                file = new FileStream(_currentFile, FileMode.Open,
                     FileAccess.Read, FileShare.ReadWrite);

                // If the file contains data.
                if (file.Length > 0)
                {
                    byte[] decryptedData = null;

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
                        decryptedData = cryto.DecryptFromMemory(array, _currentPassword);
                    }

                    // Was the data decrypted.
                    if (decryptedData == null)
                        MessageBox.Show("Incorrect file password.");
                    else
                    {
                        // Load the xml data.
                        // Deserialise the xml file.
                        GeneralSerialisation serial = new GeneralSerialisation();
                        _manager = ((Nequeo.Forms.UI.Security.Data.passwordManager)serial.Deserialise(typeof(Nequeo.Forms.UI.Security.Data.passwordManager), decryptedData));

                        // Create an empty manager.
                        if (_manager == null)
                            _manager = new Data.passwordManager();

                        // If items exist.
                        if (_manager != null && _manager.item != null)
                        {
                            // Add the items to the list.
                            _items.AddRange(_manager.item);

                            // For each item found.
                            foreach (Nequeo.Forms.UI.Security.Data.passwordManagerItem item in _manager.item)
                            {
                                // Load the names into the list box.
                                listBoxPasswordName.Items.Add(item.name);
                            }

                            // If no items exist.
                            if (listBoxPasswordName.Items.Count > 0)
                                btnRemoveItem.Enabled = true;
                        }

                        // Enable the data container.
                        listBoxPasswordName.Enabled = true;
                        btnAddItem.Enabled = true;
                        
                        // User has logged in.
                        _loggedIn = true;
                        btnFile.Enabled = false;
                        btnAuthenticate.Text = "Re-Load";
                    }
                }
                else
                {
                    // Create an empty manager.
                    _manager = new Data.passwordManager();

                    // Enable the data container.
                    listBoxPasswordName.Enabled = true;
                    btnAddItem.Enabled = true;

                    // User has logged in.
                    _loggedIn = true;
                    btnFile.Enabled = false;
                    btnAuthenticate.Text = "Re-Load";
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
        /// Open an existing encypted file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFile_Click(object sender, EventArgs e)
        {
            openFileDialogMain.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";

            // Get the file name selected.
            if (openFileDialogMain.ShowDialog() == DialogResult.OK)
                txtFile.Text = openFileDialogMain.FileName;
        }

        /// <summary>
        /// Authenticate the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAuthenticate_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFile_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtFile.Text) ||
                String.IsNullOrEmpty(txtPasswordFile.Text))
            {
                btnAuthenticate.Enabled = false;
            }
            else
            {
                btnAuthenticate.Enabled = true;
            }
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPasswordFile_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtFile.Text) ||
                String.IsNullOrEmpty(txtPasswordFile.Text))
            {
                btnAuthenticate.Enabled = false;
            }
            else
            {
                btnAuthenticate.Enabled = true;
            }
        }

        /// <summary>
        /// Add an item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddItem_Click(object sender, EventArgs e)
        {
            UI.Input input = new UI.Input();
            input.ShowDialog(this);

            // If ok then add.
            if (input.InputType == InputType.OK)
            {
                // A value must exist.
                if (!String.IsNullOrEmpty(input.InputValue))
                {
                    // Does the name already exist.
                    int count = _items.Count(u => u.name == input.InputValue);

                    // If already exists.
                    if (count > 0)
                    {
                        MessageBox.Show("The item '" + input.InputValue + "' already exists!", "Input");
                    }
                    else
                    {
                        try
                        {
                            listBoxPasswordName.Items.Add(input.InputValue);

                            // Add a new item.
                            Data.passwordManagerItem newItem = new Data.passwordManagerItem();
                            newItem.name = input.InputValue;
                            _items.Add(newItem);

                            // Assign the current item user.
                            _currentItem = newItem;

                            // If no items exist.
                            if (listBoxPasswordName.Items.Count > 0 )
                                btnRemoveItem.Enabled = true;
                        }
                        catch { }
                    }
                }
            }
        }

        /// <summary>
        /// Remove the item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            // Assign the values.
            if (_manager != null)
            {
                // If a real item has been selected.
                if (listBoxPasswordName.SelectedIndex > -1)
                {
                    // If items exist.
                    if (listBoxPasswordName.Items.Count > 0)
                    {
                        // Make sure the user wants to delete the item.
                        if (MessageBox.Show(this, "Are you sure you wish to delete item '" + listBoxPasswordName.SelectedItem.ToString() + "'?", 
                            "Delete", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            try
                            {
                                // Remove the selected item.
                                listBoxPasswordName.Items.Remove(listBoxPasswordName.SelectedItem);
                                _items.Remove(_currentItem);

                                groupBoxItem.Text = string.Empty;
                                txtUsername.Text = string.Empty;
                                txtPassword.Text = string.Empty;
                                txtEmail.Text = string.Empty;
                                txtWebsite.Text = string.Empty;
                                txtDetails.Rtf = string.Empty;

                                // Disable the group.
                                groupBoxItem.Enabled = false;

                                // If no items exist.
                                if (listBoxPasswordName.Items.Count < 1)
                                    btnRemoveItem.Enabled = false;
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// When the item has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxPasswordName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Assign the values.
            if (_manager != null)
            {
                // If a real item has been selected.
                if (listBoxPasswordName.SelectedIndex > -1)
                {
                    // If items exist.
                    if (listBoxPasswordName.Items.Count > 0)
                    {
                        try
                        {
                            // Find the selected item.
                            _currentItem = _items.First(u => u.name == listBoxPasswordName.SelectedItem.ToString());
                            groupBoxItem.Enabled = true;

                            // Assign the data.
                            AssignData();
                        }
                        catch { }
                    }
                }
            }
        }

        /// <summary>
        /// Assign the data.
        /// </summary>
        private void AssignData()
        {
            // If an item has been selected.
            if (_currentItem != null)
            {
                // Assign the values.
                groupBoxItem.Text = (String.IsNullOrEmpty(_currentItem.name) ? "" : _currentItem.name);
                txtUsername.Text = (String.IsNullOrEmpty(_currentItem.username) ? "" : _currentItem.username);
                txtPassword.Text = (String.IsNullOrEmpty(_currentItem.password) ? "" : _currentItem.password);
                txtEmail.Text = (String.IsNullOrEmpty(_currentItem.email) ? "" : _currentItem.email);
                txtWebsite.Text = (String.IsNullOrEmpty(_currentItem.website) ? "" : _currentItem.website);

                // If details exist.
                if (_currentItem.details != null)
                {
                    byte[] byteBase64 = Nequeo.Conversion.Context.HexStringToByteArray(_currentItem.details);
                    txtDetails.Rtf = Encoding.Default.GetString(byteBase64);
                }
                else
                {
                    txtDetails.Rtf = "";
                }
            }
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            // If an item has been selected.
            if (_currentItem != null)
            {
                // Assign the values.
                _currentItem.username = txtUsername.Text;
            }
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            // If an item has been selected.
            if (_currentItem != null)
            {
                // Assign the values.
                _currentItem.password = txtPassword.Text;
            }
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            // If an item has been selected.
            if (_currentItem != null)
            {
                // Assign the values.
                _currentItem.email = txtEmail.Text;
            }
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtWebsite_TextChanged(object sender, EventArgs e)
        {
            // If an item has been selected.
            if (_currentItem != null)
            {
                // Assign the values.
                _currentItem.website = txtWebsite.Text;
            }
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDetails_TextChanged(object sender, EventArgs e)
        {
            // If an item has been selected.
            if (_currentItem != null)
            {
                // Assign the values.
                byte[] dataBase64 = Encoding.Default.GetBytes(txtDetails.Rtf);
                _currentItem.details = Nequeo.Conversion.Context.ByteArrayToHexString(dataBase64);
            }
        }

        /// <summary>
        /// Key pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPasswordFile_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                if (btnAuthenticate.Enabled)
                    LoadData();
        }

        /// <summary>
        /// Link clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDetails_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(e.LinkText);
            }
            catch { }
        }

        /// <summary>
        /// Email click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblEmail_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtEmail.Text))
            {
                try
                {
                    System.Diagnostics.Process.Start("mailto:" + txtEmail.Text);
                }
                catch { }
            }
        }

        /// <summary>
        /// Website click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblWebsite_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtWebsite.Text))
            {
                try
                {
                    System.Diagnostics.Process.Start(txtWebsite.Text);
                }
                catch { }
            }
        }

        /// <summary>
        /// Show the password.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowPassword_Click(object sender, EventArgs e)
        {
            if (_showPassword)
            {
                _showPassword = false;
                txtPassword.PasswordChar = '\0';
                btnShowPassword.Text = "Hide";
            }
            else
            {
                _showPassword = true;
                txtPassword.PasswordChar = '*';
                btnShowPassword.Text = "Show";
            }
        }

        /// <summary>
        /// Transfer credentials.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTransferPassword_Click(object sender, EventArgs e)
        {
            UI.Transfer transfer = new UI.Transfer();
            transfer.richTextBoxTransfer.Text = txtUsername.Text.Trim() + "\r\n" + txtPassword.Text.Trim();
            transfer.ShowDialog(this);
        }

        /// <summary>
        /// Change account name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeAccountNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If a real item has been selected.
            if (listBoxPasswordName.SelectedIndex > -1)
            {
                UI.Input input = new UI.Input();
                input.ShowDialog(this);

                // If ok then add.
                if (input.InputType == InputType.OK)
                {
                    // A value must exist.
                    if (!String.IsNullOrEmpty(input.InputValue))
                    {
                        // Does the name already exist.
                        int count = _items.Count(u => u.name == input.InputValue);

                        // If already exists.
                        if (count > 0)
                        {
                            MessageBox.Show("The item '" + input.InputValue + "' already exists!", "Input");
                        }
                        else
                        {
                            try
                            {
                                // Get the current item name.
                                // Remove the current item.
                                string itemName = listBoxPasswordName.SelectedItem.ToString();
                                listBoxPasswordName.Items.Remove(itemName);

                                // Add the new item.
                                listBoxPasswordName.Items.Add(input.InputValue);
                                _currentItem.name = input.InputValue;
                                groupBoxItem.Text = input.InputValue;
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sort by name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sortByNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_sorted)
            {
                listBoxPasswordName.Sorted = true;
                _sorted = true;
            }
            else
            {
                listBoxPasswordName.Sorted = false;
                _sorted = false;
            }
        }

        /// <summary>
        /// Display the keyboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnKeyboard_Click(object sender, EventArgs e)
        {
            // Create the keyboard form.
            Control.KeyboardForm keyboard = new Control.KeyboardForm();
            keyboard.UserKeyPressed += new Control.KeyboardDelegate(keyboard_UserKeyPressed);

            // Show the keyboard.
            keyboard.Show();
        }

        /// <summary>
        /// User pressed a key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keyboard_UserKeyPressed(object sender, Control.KeyboardEventArgs e)
        {
            switch (_currentFocus)
            {
                case 0:
                    txtPasswordFile.Focus();
                    // Send the key pressed to the current focued control.
                    SendKeys.Send(e.KeyboardKeyPressed);
                    break;
                case 1:
                    txtPassword.Focus();
                    // Send the key pressed to the current focued control.
                    SendKeys.Send(e.KeyboardKeyPressed);
                    break;
            }
        }

        /// <summary>
        /// When active.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPasswordFile_Enter(object sender, EventArgs e)
        {
            _currentFocus = 0;
        }

        /// <summary>
        /// When inactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPasswordFile_Leave(object sender, EventArgs e)
        {
            _currentFocus = -1;
        }

        /// <summary>
        /// When active.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPassword_Enter(object sender, EventArgs e)
        {
            _currentFocus = 1;
        }

        /// <summary>
        /// When inactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPassword_Leave(object sender, EventArgs e)
        {
            _currentFocus = -1;
        }
    }
}
