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

namespace Nequeo.VoIP.PjSip.UI
{
    /// <summary>
    /// Transfer.
    /// </summary>
    public partial class TransferList : Form
    {
        /// <summary>
        /// Transfer.
        /// </summary>
        /// <param name="contacts">The contact list.</param>
        /// <param name="contactsView">The contacts list view.</param>
        /// <param name="imageListSmall">The image list.</param>
        public TransferList(Data.contacts contacts, ListView contactsView, ImageList imageListSmall)
        {
            InitializeComponent();

            _contacts = contacts;
            _contactsView = contactsView;
            _imageListSmall = imageListSmall;
        }

        private bool _selected = false;
        private string _selectedNumber = "";

        private Data.contacts _contacts = null;
        private ListView _contactsView = null;
        private ImageList _imageListSmall = null;

        /// <summary>
        /// Gets an indicator specifying if a contact has been selected.
        /// </summary>
        public bool ContactSelected
        {
            get { return _selected; }
        }

        /// <summary>
        /// Gets the selected number.
        /// </summary>
        public string ContactNumber
        {
            get { return _selectedNumber; }
        }

        /// <summary>
        /// Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TransferList_Load(object sender, EventArgs e)
        {
            // Copy the image list.
            for (int i = 1; i < _imageListSmall.Images.Count; i++)
                imageListSmall.Images.Add(_imageListSmall.Images[i]);

            // For each group.
            foreach (ListViewGroup group in _contactsView.Groups)
            {
                // Add the group.
                listViewTransfer.Groups.Add(group.Name, group.Header);
            }

            // Add each contact.
            foreach (ListViewItem item in _contactsView.Items)
            {
                // Create a new list item.
                ListViewItem viewItem = new ListViewItem(item.Text, item.ImageIndex);
                viewItem.Name = item.Name;
                viewItem.Group = listViewTransfer.Groups[item.Group.Name];

                // Add the item to the list.
                listViewTransfer.Items.Add(viewItem);
            }

            try
            {
                // Make collapsible.
                listViewTransfer.SetGroupState(Nequeo.Forms.UI.Extender.ListViewGroupState.Collapsible);
            }
            catch { }
        }

        /// <summary>
        /// Closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TransferList_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        /// <summary>
        /// OK transfer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            _selected = true;
            Close();
        }

        /// <summary>
        /// Cancel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            _selected = false;
            _selectedNumber = "";
            Close();
        }

        /// <summary>
        /// Transfer list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewTransfer_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if items selected.
            if (listViewTransfer.SelectedItems.Count > 0)
            {
                // Clear the number list.
                listViewNumber.Items.Clear();

                // Add all the numbers.
                string name = listViewTransfer.SelectedItems[0].Name;
                Data.contactsContact contact = _contacts.contact.First(u => u.sipAccount == name);

                // Add the sip account.
                listViewNumber.Items.Add(name, name, 0);

                // For each number.
                foreach (string number in contact.numbers)
                {
                    try
                    {
                        // Split the name an number.
                        string[] data = number.Split(new char[] { '|' });

                        // Add the contacts.
                        listViewNumber.Items.Add(data[1], data[0] + " " + data[1], 0);
                    }
                    catch { }
                }
            }
            else
            {
                buttonOK.Enabled = false;
            }
        }

        /// <summary>
        /// Number list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if items selected.
            if (listViewNumber.SelectedItems.Count > 0)
            {
                buttonOK.Enabled = true;

                // Add all the numbers.
                string name = listViewNumber.SelectedItems[0].Name;
                _selectedNumber = name;
            }
            else
            {
                buttonOK.Enabled = false;
            }
        }
    }
}
