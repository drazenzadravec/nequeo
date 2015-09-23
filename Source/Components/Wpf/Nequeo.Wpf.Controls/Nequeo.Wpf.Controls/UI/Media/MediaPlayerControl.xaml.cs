/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nequeo.Wpf.UI.Media
{
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

        private System.Timers.Timer _timer = null;
        private double _duration = 0.0;
        private bool _mute = false;
        private volatile bool _isDispatcher = false;

        /// <summary>
        /// Closes the media player and releases all resources.
        /// </summary>
        public void CloseMedia()
        {
            if (mediaElement != null)
            {
                // Close the media player.
                mediaElement.Stop();
                mediaElement.Close();

                if (_timer != null)
                {
                    // Stop the timer.
                    _timer.Enabled = false;
                    _timer.Dispose();
                }
            }
            _timer = null;
        }

        /// <summary>
        /// On loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //SelectedOpenIndex(comboBox.SelectedIndex);
        }

        /// <summary>
        /// Combo box item selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)sender;
            SelectedOpenIndex(Int32.Parse(item.Tag.ToString()));
        }

        /// <summary>
        /// Media opened.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            // Set the slider.
            slider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            slider.IsEnabled = mediaElement.IsLoaded;
            slider.Value = slider.Minimum;

            // Get the duration.
            _duration = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            labelDuration.Content = TimeSpan.FromSeconds(_duration).ToString().Substring(0, 8);
        }

        /// <summary>
        /// Media ended.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            EnabledOnStopControls(true);
        }

        /// <summary>
        /// Selected open index.
        /// </summary>
        /// <param name="mediaIndex">The media type index.</param>
        private void SelectedOpenIndex(int mediaIndex)
        {
            if (mediaIndex > -1)
            {
                string mediaString = "";
                Microsoft.Win32.OpenFileDialog dlg = null;

                // Open file.
                if (mediaIndex == 0)
                {
                    dlg = new Microsoft.Win32.OpenFileDialog();
                    dlg.Filter =
                        "Video Files (*.avi; *.wmv; *.qt; *.mov; *.mpg; *.mp4; *.mpeg; *.m1v)|*.avi; *.wmv; *.qt; *.mov; *.mpg; *.mp4; *.mpeg; *.m1v|" +
                        "Audio files (*.wav; *.wma; *.mpa; *.mp2; *.mp3; *.au; *.aif; *.aiff; *.snd)|*.wav; *.wma; *.mpa; *.mp2; *.mp3; *.au; *.aif; *.aiff; *.snd|" +
                        "All Files (*.*)|*.*";

                    // If the used has selected OK.
                    if ((dlg != null) && (dlg.ShowDialog() == true))
                    {
                        // Load the media file.
                        mediaString = dlg.FileName;
                        LoadMedia(mediaString, mediaIndex);
                    }
                }

                // Open netwok location.
                if (mediaIndex == 1)
                {
                    InputBox network = new InputBox();
                    network.ShowDialog();

                    // Get the URL.
                    if (network.InputType == DialogResult.OK)
                    {
                        // Load the media file.
                        mediaString = network.InputValue;
                        LoadMedia(mediaString, mediaIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Load the media file.
        /// </summary>
        /// <param name="mediaString">The media file to load.</param>
        /// <param name="mediaIndex">The media index type to load.</param>
        private void LoadMedia(string mediaString, int mediaIndex)
        {
            // If a media string has been selected.
            if (!String.IsNullOrEmpty(mediaString))
            {
                try
                {
                    _timer = new System.Timers.Timer(2000);
                    _timer.Elapsed += _timer_Elapsed;

                    // Set the source of the media to play.
                    mediaElement.Source = new Uri(mediaString);

                    // Enable controls.
                    EnabledControls(true);
                }
                catch
                {
                    if (_timer != null)
                    {
                        // Stop the timer.
                        _timer.Enabled = false;
                        _timer.Dispose();
                    }
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
        /// Close.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            // Cose the media player.
            ClosePlayer();
        }

        /// <summary>
        /// Play.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Play();
            EnabledOnPlayControls(true);
        }

        /// <summary>
        /// Stop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            EnabledOnStopControls(true);
        }

        /// <summary>
        /// Pause.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Pause();
            EnabledOnPauseControls(true);
        }

        /// <summary>
        /// Mute.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMute_Click(object sender, RoutedEventArgs e)
        {
            EnabledOnMuteControls(true);
        }

        /// <summary>
        /// Slider value changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // if in dispatch mode.
            if (!_isDispatcher)
            {
                // Get the current value.
                int sliderValue = (int)slider.Value;

                // If not minimum value.
                if (sliderValue > slider.Minimum)
                {
                    mediaElement.Pause();
                    EnabledOnPauseControls(true);

                    // Overloaded constructor takes the arguments days, hours, minutes, seconds, miniseconds.
                    // Create a TimeSpan with miliseconds equal to the slider value.
                    TimeSpan ts = new TimeSpan(0, 0, 0, 0, sliderValue);
                    mediaElement.Position = ts;
                }
            }
        }

        /// <summary>
        /// Slider mouse down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // if in dispatch mode.
            if (!_isDispatcher)
            {
                mediaElement.Pause();
                EnabledOnPauseControls(true);
            }
        }

        /// <summary>
        /// Update the time elasped.
        /// </summary>
        /// <param name="result">The result.</param>
        private void UpdateTime(bool result)
        {   
            // Invoke the dispatcher on this thread for
            // the position timer.
            this.Dispatcher.Invoke((Action)(() =>
            {
                try
                {
                    // Get the current position.
                    TimeSpan currentPosition = mediaElement.Position;
                    double current = currentPosition.TotalSeconds;
                    labelTime.Content = TimeSpan.FromSeconds(current).ToString().Substring(0, 8);

                    // Change value
                    _isDispatcher = true;
                }
                catch { }
                _isDispatcher = false;

            }));
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
                mediaElement.IsMuted = _mute;
                btnMute.Content = "Mute Off";
            }
            else
            {
                _mute = false;
                mediaElement.IsMuted = _mute;
                btnMute.Content = "Mute";
            }
        }

        /// <summary>
        /// Enable or disable controls.
        /// </summary>
        /// <param name="enabled">Enabled.</param>
        private void EnabledControls(bool enabled)
        {
            _mute = false;
            comboBox.IsEnabled = !enabled;
            btnClose.IsEnabled = enabled;
            btnPlay.IsEnabled = enabled;
        }

        /// <summary>
        /// Close the player.
        /// </summary>
        private void ClosePlayer()
        {
            if (mediaElement != null)
            {
                labelTime.Content = "00:00:00";
                labelDuration.Content = "00:00:00";
                EnabledOnCloseControls(false);

                // Close the media player.
                mediaElement.Close();
                
                if (_timer != null)
                {
                    // Stop the timer.
                    _timer.Enabled = false;
                    _timer.Dispose();
                }
            }
        }

        /// <summary>
        /// Enable or disable controls.
        /// </summary>
        /// <param name="enabled">Enabled.</param>
        private void EnabledOnCloseControls(bool enabled)
        {
            // Stop the timer.
            _timer.Enabled = false;

            comboBox.IsEnabled = !enabled;
            btnClose.IsEnabled = enabled;
            btnPlay.IsEnabled = enabled;
            btnStop.IsEnabled = enabled;
            btnPause.IsEnabled = enabled;
            btnMute.IsEnabled = enabled;
            slider.IsEnabled = enabled;
            slider.Value = slider.Minimum;
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

            btnPlay.IsEnabled = !enabled;
            btnStop.IsEnabled = enabled;
            btnPause.IsEnabled = enabled;
            btnMute.IsEnabled = enabled;
            slider.IsEnabled = enabled;
        }

        /// <summary>
        /// Enable or disable controls.
        /// </summary>
        /// <param name="enabled">Enabled.</param>
        private void EnabledOnPauseControls(bool enabled)
        {
            // Stop the timer.
            _timer.Enabled = false;

            btnPlay.IsEnabled = enabled;
            btnStop.IsEnabled = enabled;
            btnPause.IsEnabled = !enabled;
            btnMute.IsEnabled = enabled;
        }

        /// <summary>
        /// Enable or disable controls.
        /// </summary>
        /// <param name="enabled">Enabled.</param>
        private void EnabledOnStopControls(bool enabled)
        {
            mediaElement.Stop();
            labelTime.Content = "00:00:00";

            // Stop the timer.
            _timer.Enabled = false;

            btnPlay.IsEnabled = enabled;
            btnStop.IsEnabled = !enabled;
            btnPause.IsEnabled = !enabled;
            btnMute.IsEnabled = !enabled;
            slider.IsEnabled = !enabled;
            slider.Value = slider.Minimum;

            // Overloaded constructor takes the arguments days, hours, minutes, seconds, miniseconds.
            // Create a TimeSpan with miliseconds equal to the slider value.
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, 0);
            mediaElement.Position = ts;
        }
    }
}
