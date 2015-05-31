using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nequeo.Media.UI
{
    /// <summary>
    /// Update time call back delegate.
    /// </summary>
    /// <param name="result">Result of operation.</param>
    internal delegate void UpdateTimeCallBack(bool result);

    /// <summary>
    /// Media player control.
    /// </summary>
    public partial class MediaPlayerControl : UserControl
    {
        /// <summary>
        /// Media player control.
        /// </summary>
        public MediaPlayerControl()
        {
            InitializeComponent();
        }

        private Nequeo.Media.MediaPlayer _player = null;
        private System.Timers.Timer _timer = null;
        private double _duration = 0.0;
        private bool _mute = false;
        private ushort _volume = 0;

        /// <summary>
        /// Closes the media player and releases all resources.
        /// </summary>
        public void CloseMedia()
        {
            if (_player != null)
            {
                // Close the media player.
                _player.Stop();
                _player.Close();

                if (_timer != null)
                {
                    // Stop the timer.
                    _timer.Enabled = false;
                    _timer.Dispose();
                }
            }

            _player = null;
            _timer = null;
        }

        /// <summary>
        /// Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaPlayerControl_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Open the media.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOpen_Click(object sender, EventArgs e)
        {
            if (_player == null)
            {
                // Create the player.
                _player = new Nequeo.Media.MediaPlayer();

                // Set the certificate filter.
                openFileDialogMain.Filter = "Media Files (*.nva)|*.nva";

                // Get the file name selected.
                if (openFileDialogMain.ShowDialog() == DialogResult.OK)
                {
                    _timer = new System.Timers.Timer(2000);
                    _timer.Elapsed += _timer_Elapsed;

                    // Open the media file.
                    _player.Open(openFileDialogMain.FileName);

                    // Get the duration.
                    _duration = _player.Duration.TotalSeconds;
                    labelDuration.Text = TimeSpan.FromSeconds(_duration).ToString().Substring(0, 8);

                    // Enable controls.
                    EnabledControls(true);
                }
                else
                {
                    _player.Close();
                    _player = null;
                }
            }
        }

        /// <summary>
        /// Timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                UpdateTime(true);
            }
            catch { }
        }

        /// <summary>
        /// Close the media.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClose_Click(object sender, EventArgs e)
        {
            if (_player != null)
            {
                labelTime.Text = "00:00:00";
                labelDuration.Text = "00:00:00";
                EnabledOnCloseControls(false);

                // Close the media player.
                _player.Close();

                if (_timer != null)
                {
                    // Stop the timer.
                    _timer.Enabled = false;
                    _timer.Dispose();
                }
            }

            _player = null;
            _timer = null;
        }

        /// <summary>
        /// Play.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if (_player != null)
            {
                _player.Play();
                EnabledOnPlayControls(true);
            }
        }

        /// <summary>
        /// Stop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (_player != null)
            {
                EnabledOnStopControls(true);
            }
        }

        /// <summary>
        /// Pause.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPause_Click(object sender, EventArgs e)
        {
            if (_player != null)
            {
                _player.Pause();
                EnabledOnPauseControls(true);
            }
        }

        /// <summary>
        /// Mute.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMute_Click(object sender, EventArgs e)
        {
            if (_player != null)
            {
                if (!_mute)
                {
                    _volume = _player.Volume;
                    _player.Volume = 0;
                }
                else
                    _player.Volume = _volume;

                EnabledOnMuteControls(true);
            }
        }

        /// <summary>
        /// Enable or disable controls.
        /// </summary>
        /// <param name="enabled">Enabled.</param>
        private void EnabledControls(bool enabled)
        {
            _mute = false;
            buttonOpen.Enabled = !enabled;
            buttonClose.Enabled = enabled;
            buttonPlay.Enabled = enabled;
        }

        /// <summary>
        /// Enable or disable controls.
        /// </summary>
        /// <param name="enabled">Enabled.</param>
        private void EnabledOnCloseControls(bool enabled)
        {
            // Stop the timer.
            _timer.Enabled = false;

            buttonOpen.Enabled = !enabled;
            buttonClose.Enabled = enabled;
            buttonPlay.Enabled = enabled;
            buttonStop.Enabled = enabled;
            buttonPause.Enabled = enabled;
            buttonMute.Enabled = enabled;
        }

        /// <summary>
        /// Enable or disable controls.
        /// </summary>
        /// <param name="enabled">Enabled.</param>
        private void EnabledOnPlayControls(bool enabled)
        {
            // Only raise the event the first time Interval elapses set AutoReset = False
            // Start the timer, start collecting file logging information.
            _timer.AutoReset = true;
            _timer.Enabled = true;

            buttonPlay.Enabled = !enabled;
            buttonStop.Enabled = enabled;
            buttonPause.Enabled = enabled;
            buttonMute.Enabled = enabled;
        }

        /// <summary>
        /// Enable or disable controls.
        /// </summary>
        /// <param name="enabled">Enabled.</param>
        private void EnabledOnPauseControls(bool enabled)
        {
            // Stop the timer.
            _timer.Enabled = false;

            buttonPlay.Enabled = enabled;
            buttonStop.Enabled = enabled;
            buttonPause.Enabled = !enabled;
            buttonMute.Enabled = enabled;
        }

        /// <summary>
        /// Enable or disable controls.
        /// </summary>
        /// <param name="enabled">Enabled.</param>
        private void EnabledOnStopControls(bool enabled)
        {
            _player.Stop();
            labelTime.Text = "00:00:00";

            // Stop the timer.
            _timer.Enabled = false;

            buttonPlay.Enabled = enabled;
            buttonStop.Enabled = !enabled;
            buttonPause.Enabled = !enabled;
            buttonMute.Enabled = !enabled;
        }

        /// <summary>
        /// Enable or disable controls.
        /// </summary>
        /// <param name="enabled">Enabled.</param>
        private void EnabledOnMuteControls(bool enabled)
        {
            if (!_mute)
            {
                _mute = true;
                buttonMute.Text = "Mute Off";
            }
            else
            {
                _mute = false;
                buttonMute.Text = "Mute";
            }
        }

        /// <summary>
        /// Update the time elasped.
        /// </summary>
        /// <param name="result">The result.</param>
        private void UpdateTime(bool result)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.InvokeRequired)
            {
                // Create a new delegate.
                UpdateTimeCallBack update = new UpdateTimeCallBack(UpdateTime);

                // Execute the delegate on the current control.
                this.Invoke(update, new object[] { result });
            }
            else
            {
                if (_player != null)
                {
                    // Get the current position.
                    double current = _player.Position.TotalSeconds;

                    if (current < _duration)
                    {
                        // Update the time.
                        labelTime.Text = TimeSpan.FromSeconds(current).ToString().Substring(0, 8);
                    }
                    else
                    {
                        // Stop the timer.
                        labelTime.Text = "00:00:00";
                        EnabledOnStopControls(true);
                    }
                }
            }
        }
    }
}
