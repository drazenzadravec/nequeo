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

namespace Nequeo.VoIP.Sip.UI
{
    /// <summary>
    /// Instant message.
    /// </summary>
    public partial class InstantMessage : Form
    {
        /// <summary>
        /// Instant message.
        /// </summary>
        /// <param name="voipCall">VoIP call.</param>
        /// <param name="contacts">List of contacts.</param>
        public InstantMessage(Nequeo.VoIP.Sip.VoIPCall voipCall, ListView contacts)
        {
            InitializeComponent();
            _voipCall = voipCall;
            _contacts = contacts;
        }

        private Nequeo.VoIP.Sip.VoIPCall _voipCall = null;
        private Nequeo.Net.Sip.Contact _contact = null;
        private ListView _contacts = null;

        /// <summary>
        /// Form is closing.
        /// </summary>
        public event System.EventHandler OnInstantMessageClosing;

        /// <summary>
        /// Incoming message.
        /// </summary>
        /// <param name="message">The instant message.</param>
        public void Message(Nequeo.VoIP.Sip.Param.OnInstantMessageParam message)
        {
            // Get the contact.
            ListViewItem item = _contacts.Items[message.From];

            // If found.
            if (item != null)
            {
                DateTime date = DateTime.Now;
                richTextBoxMessage.Text += date.ToLongDateString() + " " + date.ToLongTimeString() + "\r\n" +
                    "\t" + "From : \t" + item.Text + "\r\n" +
                    "\t" + "Message : " + message.MsgBody + "\r\n\r\n";
            }
            else
            {
                DateTime date = DateTime.Now;
                richTextBoxMessage.Text += date.ToLongDateString() + " " + date.ToLongTimeString() + "\r\n" +
                    "\t" + "From : \t" + message.From + "\r\n" +
                    "\t" + "Message : " + message.MsgBody + "\r\n\r\n";
            }

            // Scroll to the end.
            richTextBoxMessage.SelectionStart = richTextBoxMessage.Text.Length;
            richTextBoxMessage.ScrollToCaret();
        }

        /// <summary>
        /// Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InstantMessage_Load(object sender, EventArgs e)
        {
            // Add each contact.
            foreach (ListViewItem item in _contacts.Items)
            {
                // Add the item to the list.
                listViewMessage.Items.Add(item.Name, item.Text, 0);
            }
        }

        /// <summary>
        /// Send.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (_contact != null)
            {
                try
                {
                    DateTime date = DateTime.Now;
                    richTextBoxMessage.Text += date.ToLongDateString() + " " + date.ToLongTimeString() + "\r\n" +
                    "\t" + "From : \t" + "You" + "\r\n" +
                    "\t" + "Message : " + textBoxSendMesssage.Text + "\r\n\r\n";

                    // Send the message.
                    Nequeo.Net.Sip.SendInstantMessage message = new Net.Sip.SendInstantMessage();
                    message.Content = textBoxSendMesssage.Text;
                    message.ContentType = "text/plain";
                    _contact.SendInstantMessage(message);
                }
                catch (Exception)
                {
                    // Ask the used to answer incomming call.
                    DialogResult result = MessageBox.Show(this, "Unable to send the message because of an internal error.",
                        "Send Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Scroll to the end.
                richTextBoxMessage.SelectionStart = richTextBoxMessage.Text.Length;
                richTextBoxMessage.ScrollToCaret();
            }
        }

        /// <summary>
        /// Send message changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxSendMesssage_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxSendMesssage.Text))
                EnableDisableCheck();
            else
                EnableDisable(false);
        }

        /// <summary>
        /// Closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InstantMessage_FormClosing(object sender, FormClosingEventArgs e)
        {
            OnInstantMessageClosing?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewMessage_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if items selected.
            if (listViewMessage.SelectedItems.Count > 0)
            {
                EnableDisableCheck();
                foreach (ListViewItem item in listViewMessage.SelectedItems)
                {
                    // Who to send to.
                    labelSendToValue.Text = item.Text;

                    try
                    {
                        // Find the contact.
                        _contact = _voipCall.VoIPManager.FindContact(item.Name);
                    }
                    catch { }
                    break;
                }
            }
            else
            {
                // Who to send to.
                labelSendToValue.Text = "";
                EnableDisable(false);
            }
        }

        /// <summary>
        /// Check if valid.
        /// </summary>
        private void EnableDisableCheck()
        {
            if (listViewMessage.SelectedItems.Count > 0 && !String.IsNullOrEmpty(textBoxSendMesssage.Text))
                EnableDisable(true);
            else
                EnableDisable(false);
        }

        /// <summary>
        /// Enable or disable.
        /// </summary>
        /// <param name="enable"></param>
        private void EnableDisable(bool enable)
        {
            buttonSend.Enabled = enable;
        }
    }
}
