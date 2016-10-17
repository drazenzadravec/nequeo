/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Nequeo.Extension;

namespace Nequeo.VoIP.Sip.UI
{
    /// <summary>
    /// Contact info.
    /// </summary>
    public partial class ContactInfo : Form
    {
        /// <summary>
        /// Contact info.
        /// </summary>
        /// <param name="voipCall">VoIP call.</param>
        /// <param name="contactKey">The contact key.</param>
        /// <param name="adding">True if adding false if updating.</param>
        /// <param name="contacts">The contact list.</param>
        /// <param name="contactsView">The contact list.</param>
        public ContactInfo(Nequeo.VoIP.Sip.VoIPCall voipCall, string contactKey, 
            bool adding, Data.contacts contacts, ListView contactsView)
        {
            InitializeComponent();
            _voipCall = voipCall;
            _contactKey = contactKey;
            _adding = adding;
            _contacts = contacts;
            _contactsView = contactsView;
        }

        private bool _adding = true;
        private string _contactKey = null;
        private Nequeo.VoIP.Sip.VoIPCall _voipCall = null;
        private Data.contacts _contacts = null;
        private List<string> _numbers = null;
        private ListView _contactsView = null;

        private bool _newContact = false;
        private bool _presenecState = false;
        private string _sipAccount = null;
        private string _name = null;
        private string _group = null;
        private string _picture = null;

        /// <summary>
        /// Gets an indicator specifying if a new contact has been created.
        /// </summary>
        public bool NewContact
        {
            get { return _newContact; }
        }

        /// <summary>
        /// Gets the presenece state.
        /// </summary>
        public bool PresenecState
        {
            get { return _presenecState; }
        }

        /// <summary>
        /// Gets the sip account.
        /// </summary>
        public string SipAccount
        {
            get { return _sipAccount; }
        }

        /// <summary>
        /// Gets the contact name.
        /// </summary>
        public string ContactName
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the contact group.
        /// </summary>
        public string ContactGroup
        {
            get { return _group; }
        }

        /// <summary>
        /// Gets the contact picture.
        /// </summary>
        public string ContactPicture
        {
            get { return _picture; }
        }

        /// <summary>
        /// OK.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            // If updating.
            if (!_adding)
            {
                if (_contacts != null)
                {
                    // Get the contact.
                    Data.contactsContact contact = _contacts.contact.First(u => u.sipAccount == _contactKey);
                    contact.name = textBoxName.Text;
                    contact.presenceState = checkBoxPresenceState.Checked;
                    contact.group = comboBoxGroup.Text;
                    contact.numbers = _numbers.ToArray();
                    contact.picture = textBoxPicture.Text;
                    _name = contact.name;
                    _group = contact.group;
                    _presenecState = contact.presenceState;
                    _picture = contact.picture;
                }
            }
            else
            {
                if (_contacts != null)
                {
                    // Load the current list. 
                    List<Data.contactsContact> contacts = new List<Data.contactsContact>(_contacts.contact);
                    Data.contactsContact contact = new Data.contactsContact()
                    {
                        name = textBoxName.Text,
                        sipAccount = textBoxSipAccount.Text,
                        presenceState = checkBoxPresenceState.Checked,
                        numbers = _numbers.ToArray(),
                        group = comboBoxGroup.Text,
                        picture = textBoxPicture.Text,
                    };

                    // Add the new contact.
                    contacts.Add(contact);
                    _contacts.contact = contacts.ToArray();

                    // New contact.
                    _newContact = true;
                    _presenecState = contact.presenceState;
                    _sipAccount = contact.sipAccount;
                    _name = contact.name;
                    _group = contact.group;
                    _picture = contact.picture;
                }
            }
            Close();
        }

        /// <summary>
        /// Cancel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Add number.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            UI.NumberInfo numberInfo = new NumberInfo();
            numberInfo.ShowDialog(this);

            // Add the number.
            try
            {
                // If the number is valid.
                if (numberInfo.CreatedNumber)
                {
                    // Add the number.
                    listViewNumbers.Items.Add(numberInfo.NumberName + "|" + numberInfo.Number,
                        numberInfo.NumberName + " " + numberInfo.Number, 0);

                    // Add to the list.
                    _numbers.Add(numberInfo.NumberName + "|" + numberInfo.Number);
                }
            }
            catch { }
        }

        /// <summary>
        /// Delete number.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            string contactName = "";
            string contactKey = "";

            // Add each contact.
            foreach (ListViewItem item in listViewNumbers.SelectedItems)
            {
                // Get the name.
                contactName = item.Text;
                contactKey = item.Name;
                break;
            }

            // If a key has been selected.
            if (!String.IsNullOrEmpty(contactKey))
            {
                // Ask the used to answer incomming call.
                DialogResult result = MessageBox.Show(this, "Are you sure you wish to delete number " + contactName + ".",
                    "Delete Number", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // If delete.
                if (result == DialogResult.Yes)
                {
                    // Remove the item.
                    listViewNumbers.Items.RemoveByKey(contactKey);

                    try
                    {
                        // Remove from contact file.
                        Data.contactsContact contact = _contacts.contact.First(u => u.sipAccount == _contactKey);
                        string number = contact.numbers.First(u => u == contactKey);
                        contact.numbers = contact.numbers.Remove(u => u.Equals(number));
                        _numbers.Remove(number);
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Numbers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewNumbers_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if items selected.
            if (listViewNumbers.SelectedItems.Count > 0)
            {
                buttonDelete.Enabled = true;
            }
            else
            {
                buttonDelete.Enabled = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxName.Text) && 
                !String.IsNullOrEmpty(textBoxSipAccount.Text) &&
                comboBoxGroup.SelectedIndex >= 0)
            {
                buttonOk.Enabled = true;
                buttonAdd.Enabled = true;
                listViewNumbers.Enabled = true;
            }
            else
            {
                buttonOk.Enabled = false;
                buttonAdd.Enabled = false;
                listViewNumbers.Enabled = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxSipAccount_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxName.Text) && 
                !String.IsNullOrEmpty(textBoxSipAccount.Text) &&
                comboBoxGroup.SelectedIndex >= 0)
            {
                buttonOk.Enabled = true;
                buttonAdd.Enabled = true;
                listViewNumbers.Enabled = true;
            }
            else
            {
                buttonOk.Enabled = false;
                buttonAdd.Enabled = false;
                listViewNumbers.Enabled = false;
            }
        }

        /// <summary>
        /// Enter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupBoxContact_Enter(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContactInfo_Load(object sender, EventArgs e)
        {
            // Numbers list.
            _numbers = new List<string>();

            // Load the group list.
            if (_contactsView != null)
            {
                // For each group.
                foreach (ListViewGroup group in _contactsView.Groups)
                {
                    // Add the group.
                    comboBoxGroup.Items.Add(group.Header);
                }
            }

            // If updating.
            if (!_adding)
            {
                if (_contacts != null)
                {
                    // Get the contact.
                    Data.contactsContact contact = _contacts.contact.First(u => u.sipAccount == _contactKey);
                    textBoxName.Text = contact.name;
                    textBoxSipAccount.Text = contact.sipAccount;
                    checkBoxPresenceState.Checked = contact.presenceState;
                    comboBoxGroup.SelectedIndex = comboBoxGroup.Items.IndexOf(contact.group);
                    textBoxPicture.Text = contact.picture;
                    textBoxSipAccount.ReadOnly = true;

                    // For each number.
                    foreach (string number in contact.numbers)
                    {
                        // Split the name an number.
                        string[] data = number.Split(new char[] { '|' });
                        _numbers.Add(number);

                        try
                        {
                            // Add the contacts.
                            listViewNumbers.Items.Add(number, data[0] + " " + data[1], 0);
                        }
                        catch { }
                    }
                }
            }
        }

        /// <summary>
        /// Group.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxName.Text) &&
                !String.IsNullOrEmpty(textBoxSipAccount.Text) &&
                comboBoxGroup.SelectedIndex >= 0)
            {
                buttonOk.Enabled = true;
                buttonAdd.Enabled = true;
                listViewNumbers.Enabled = true;
            }
            else
            {
                buttonOk.Enabled = false;
                buttonAdd.Enabled = false;
                listViewNumbers.Enabled = false;
            }
        }

        /// <summary>
        /// Open picture.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPicture_Click(object sender, EventArgs e)
        {
            // Set the import filter.
            openFileDialog.Filter = "All image files (*.bmp, *.gif, *.jpg, *.jpeg, *.png, *.ico)|*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.ico";

            // Get the file name selected.
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxPicture.Text = openFileDialog.FileName;
            }
        }
    }
}

