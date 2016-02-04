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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nequeo.VoIP.Sip.UI
{
    /// <summary>
    /// VoIP control.
    /// </summary>
    public partial class VoIPControl : UserControl
    {
        /// <summary>
        /// VoIP control.
        /// </summary>
        public VoIPControl()
        {
            InitializeComponent();
            Init();
        }

        private bool _registered = false;
        private bool _disposed = false;
        private Nequeo.VoIP.Sip.VoIPCall _voipCall = null;

        /// <summary>
        /// Dispose of the unmanaged resources.
        /// </summary>
        public void DisposeCall()
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (_voipCall != null)
                    _voipCall.Dispose();

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _voipCall = null;
            }
        }

        /// <summary>
        /// Initialise.
        /// </summary>
        private void Init()
        {
            _voipCall = new VoIPCall();
            _voipCall.OnIncomingCall += Voipcall_OnIncomingCall;
            _voipCall.OnInstantMessage += Voipcall_OnInstantMessage;
            _voipCall.OnRegStarted += Voipcall_OnRegStarted;
            _voipCall.OnRegState += Voipcall_OnRegState;
        }

        /// <summary>
        /// Create.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCreate_Click(object sender, EventArgs e)
        {
            // Assign credentials.
            _voipCall.VoIPManager.AccountConnection.AccountName = textBoxAccountName.Text;
            _voipCall.VoIPManager.AccountConnection.SpHost = textBoxHost.Text;

            // Create the credentials.
            Net.Sip.AuthCredInfo[] AuthCredentials = new Net.Sip.AuthCredInfo[] 
                { new Net.Sip.AuthCredInfo(textBoxUsername.Text, textBoxPassword.Text) };
            _voipCall.VoIPManager.AccountConnection.AuthenticateCredentials =
                new Net.Sip.AuthenticateCredentials() { AuthCredentials = AuthCredentials };
            
            // Create.
            _voipCall.Create();

            // Enable.
            buttonCreate.Enabled = false;
            buttonSettings.Enabled = true;
            buttonRegister.Enabled = false;
        }

        /// <summary>
        /// Register.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRegister_Click(object sender, EventArgs e)
        {
            // Register.
            _voipCall.Registration(!_registered);
        }

        /// <summary>
        /// On registration state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Voipcall_OnRegState(object sender, Nequeo.VoIP.Sip.Param.OnRegStateParam e)
        {
            UISync.Execute(() =>
            {
                labelRegistationStatusState.Text = e.Reason + ". " + e.Info;

                // Check the registration state.
                if (e.Code != Net.Sip.StatusCode.SC_OK)
                {
                    // Enable.
                    buttonCreate.Enabled = true;
                    buttonSettings.Enabled = false;
                    buttonRegister.Enabled = false;
                    _registered = false;

                    // Dispose of the unmanaged resources.
                    DisposeCall();
                }
            });
            
        }

        /// <summary>
        /// On registration started.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Voipcall_OnRegStarted(object sender, Nequeo.VoIP.Sip.Param.OnRegStartedParam e)
        {
            if (e.Renew)
            {
                labelRegistationStatus.Text = "Registation Renewed : ";
                _registered = true;
            }
            else
            {
                // No Registration.
                labelRegistationStatus.Text = "Registation : ";
                labelRegistationStatusState.Text = "Not Registered";
                _registered = false;
            }
        }

        /// <summary>
        /// On instant message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Voipcall_OnInstantMessage(object sender, Nequeo.VoIP.Sip.Param.OnInstantMessageParam e)
        {

        }

        /// <summary>
        /// On incoming call.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Voipcall_OnIncomingCall(object sender, Nequeo.VoIP.Sip.Param.OnIncomingCallParam e)
        {
            // Ask the used to answer incomming call.
            DialogResult result = MessageBox.Show(this, "Source : " + e.SrcAddress + "\r\n" + e.WholeMsg, 
                "Answer Incomming Call?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Answer.
                e.AnswerCall = true;
            }
            else
            {
                // Hangup.
                e.AnswerCall = false;
            }
        }

        /// <summary>
        /// Account name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxAccountName_TextChanged(object sender, EventArgs e)
        {
            if (textBoxAccountName.Text == null)
                textBoxAccountName.Text = string.Empty;

        }

        /// <summary>
        /// Host.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxHost_TextChanged(object sender, EventArgs e)
        {
            if (textBoxHost.Text == null)
                textBoxHost.Text = string.Empty;
        }

        /// <summary>
        /// Username.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxUsername_TextChanged(object sender, EventArgs e)
        {
            if (textBoxUsername.Text == null)
                textBoxUsername.Text = string.Empty;
        }

        /// <summary>
        /// Password.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            if (textBoxPassword.Text == null)
                textBoxPassword.Text = string.Empty;
        }

        /// <summary>
        /// Settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSettings_Click(object sender, EventArgs e)
        {
            UI.Settings settings = new Settings(_voipCall);
            settings.ShowDialog(this);

            // Enable.
            buttonCreate.Enabled = false;
            buttonSettings.Enabled = false;
            buttonRegister.Enabled = true;
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VoIPControl_Load(object sender, EventArgs e)
        {
            UISync.Init(this);
            base.Disposed += VoIPControl_Disposed;
        }

        /// <summary>
        /// Disposed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VoIPControl_Disposed(object sender, EventArgs e)
        {
            // Dispose of the unmanaged resources.
            DisposeCall();
        }

        /// <summary>
        /// Update the time elasped.
        /// </summary>
        private class UISync
        {
            private static ISynchronizeInvoke Sync;

            /// <summary>
            /// Initialisation
            /// </summary>
            /// <param name="sync">The initialisation sync.</param>
            public static void Init(ISynchronizeInvoke sync)
            {
                Sync = sync;
            }

            /// <summary>
            /// Execute the action.
            /// </summary>
            /// <param name="action">The action to perfoem.</param>
            public static void Execute(Action action)
            {
                Sync.BeginInvoke(action, null);
            }
        }

        
    }
}
