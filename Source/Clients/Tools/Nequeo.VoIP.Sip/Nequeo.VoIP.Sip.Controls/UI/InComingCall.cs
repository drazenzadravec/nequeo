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
    /// In coming call.
    /// </summary>
    public partial class InComingCall : Form
    {
        /// <summary>
        /// In coming call.
        /// </summary>
        /// <param name="inComingCall">In coming call param.</param>
        /// <param name="contactName">The contact name.</param>
        public InComingCall(Nequeo.VoIP.Sip.Param.OnIncomingCallParam inComingCall, string contactName)
        {
            InitializeComponent();
            _inComingCall = inComingCall;
            _contactName = contactName;
        }

        private bool _hasAction = false;
        private string _contactName = null;
        private Nequeo.VoIP.Sip.Param.OnIncomingCallParam _inComingCall = null;

        /// <summary>
        /// Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InComingCall_Load(object sender, EventArgs e)
        {
            if (_inComingCall != null)
            {
                // Ask the used to answer incomming call.
                textBoxDetails.Text =
                    "Source : \t" + _inComingCall.SrcAddress.Trim() + "\r\n" +
                    (String.IsNullOrEmpty(_inComingCall.From.Trim()) ? "" : "From : \t" + _inComingCall.From.Trim() + "\r\n") +
                    (String.IsNullOrEmpty(_inComingCall.FromContact.Trim()) ? "" : "Contact : \t" + _inComingCall.FromContact.Trim() + "\r\n\r\n") +
                    (String.IsNullOrEmpty(_contactName) ? "" : "Contact : \t" + _contactName + "\r\n");
            }
        }

        /// <summary>
        /// Answer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAnswer_Click(object sender, EventArgs e)
        {
            _hasAction = true;
            buttonAnswer.Enabled = false;
            buttonHangup.Enabled = true;
            groupBoxDigits.Enabled = true;

            if (_inComingCall != null)
            {
                try
                {
                    _inComingCall.Call.Answer();
                }
                catch { }
            }
        }

        /// <summary>
        /// Hangup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHangup_Click(object sender, EventArgs e)
        {
            _hasAction = true;
            buttonAnswer.Enabled = false;
            buttonHangup.Enabled = false;
            groupBoxDigits.Enabled = false;

            if (_inComingCall != null)
            {
                try
                {
                    _inComingCall.Call.Hangup();
                }
                catch { }
            }
        }

        /// <summary>
        /// Closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InComingCall_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_hasAction)
            {
                if (_inComingCall != null)
                {
                    try
                    {
                        _inComingCall.Call.Hangup();
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Digit 1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOne_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("1");
                    textBoxDigits.Text += "1";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit 2.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonTwo_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("2");
                    textBoxDigits.Text += "2";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit 3.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonThree_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("3");
                    textBoxDigits.Text += "3";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit 4.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFour_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("4");
                    textBoxDigits.Text += "4";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit 5.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFive_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("5");
                    textBoxDigits.Text += "5";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit 6.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSix_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("6");
                    textBoxDigits.Text += "6";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit 7.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSeven_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("7");
                    textBoxDigits.Text += "7";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit 8.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEight_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("8");
                    textBoxDigits.Text += "8";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit 9.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonNine_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("9");
                    textBoxDigits.Text += "9";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit *.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStar_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("*");
                    textBoxDigits.Text += "*";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit 0.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonZero_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("0");
                    textBoxDigits.Text += "0";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit #.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHash_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("#");
                    textBoxDigits.Text += "#";
                }
            }
            catch (Exception) { }
        }
    }
}
