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
using System.Net.NetworkInformation;
using System.Globalization;

namespace Nequeo.Forms.UI.Net
{
    /// <summary>
    /// Ping client user control.
    /// </summary>
    public partial class PingClient : UserControl
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public PingClient()
        {
            InitializeComponent();
            _ping = new Ping();
        }

        private Ping _ping = null;

        /// <summary>
        /// Load user constrol.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PingClient_Load(object sender, EventArgs e)
        {
            // Attach to the ping complete event.
            _ping.PingCompleted += new PingCompletedEventHandler(OnPingCompleted);
        }

        /// <summary>
        /// Send a ping request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPing_Click(object sender, EventArgs e)
        {
            try
            {
                // Select all the text in the address box.
                txtHostName.SelectAll();

                // If ip address and host name
                // has been entered.
                if (txtHostName.Text.Trim().Length != 0)
                {
                    // Disable the Send button.
                    btnPing.Enabled = false;

                    // Add the host address to the
                    // ping result details.
                    txtPingResult.Text +=
                        "Pinging " + txtHostName.Text + " . . .\r\n";

                    // Send ping request.
                    _ping.SendAsync(txtHostName.Text.Trim(), null);
                }
            }
            catch { }   
        }

        /// <summary>
        /// Stop the ping request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, EventArgs e)
        {
            btnPing.Enabled = true;

            try
            {
                // Cancell the ping request.
                _ping.SendAsyncCancel();
            }
            catch { }   
        }

        /// <summary>
        /// Ping complete event.
        /// </summary>
        /// <param name="sender">Current sender.</param>
        /// <param name="e">Event arguments.</param>
        void OnPingCompleted(object sender, PingCompletedEventArgs e)
        {
            // Check to see if an error occurred.  If no error, then display 
            // the address used and the ping time in milliseconds.
            if (e.Error == null)
            {
                // If the operation was cancelled.
                if (e.Cancelled)
                    txtPingResult.Text += "  Ping cancelled. \r\n";
                else
                {
                    // If the ping request succeded.
                    if (e.Reply.Status == IPStatus.Success)
                    {
                        // Show the result of the
                        // ping request.
                        txtPingResult.Text +=
                            "  " + e.Reply.Address.ToString() + " " +
                            e.Reply.RoundtripTime.ToString(
                            NumberFormatInfo.CurrentInfo) + "ms" + "\r\n";
                    }
                    else
                        // If the ping was not succesful
                        // then get ip status.
                        txtPingResult.Text +=
                            "  " + GetStatusString(e.Reply.Status) + "\r\n";
                }
            }
            else
            {
                // Otherwise display the error.
                txtPingResult.Text += "  Ping error.\r\n";
                MessageBox.Show("An error occurred while sending this ping. " +
                    e.Error.InnerException.Message);
            }

            // Enable the send ping button
            // when the ping was complete.
            btnPing.Enabled = true;
        }

        /// <summary>
        /// Get the ping status
        /// </summary>
        /// <param name="status">The ip status code.</param>
        /// <returns>The ping status code.</returns>
        private string GetStatusString(IPStatus status)
        {
            switch (status)
            {
                case IPStatus.Success:
                    return "Success.";
                case IPStatus.DestinationHostUnreachable:
                    return "Destination host unreachable.";
                case IPStatus.DestinationNetworkUnreachable:
                    return "Destination network unreachable.";
                case IPStatus.DestinationPortUnreachable:
                    return "Destination port unreachable.";
                case IPStatus.DestinationProtocolUnreachable:
                    return "Destination protocol unreachable.";
                case IPStatus.PacketTooBig:
                    return "Packet too big.";
                case IPStatus.TtlExpired:
                    return "TTL expired.";
                case IPStatus.ParameterProblem:
                    return "Parameter problem.";
                case IPStatus.SourceQuench:
                    return "Source quench.";
                case IPStatus.TimedOut:
                    return "Timed out.";
                default:
                    return "Ping failed.";
            }
        }

        /// <summary>
        /// Text change on host name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtHostName_TextChanged(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(txtHostName.Text))
                btnPing.Enabled = true;
        }
    }
}
