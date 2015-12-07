using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Nequeo.Media.Vlc.Players;
using Nequeo.Media.Vlc.Media;
using Nequeo.Media.Vlc.Events;

namespace Nequeo.Media.Vlc.UI
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

        private IMediaPlayerFactory _player = null;
        private IDiskPlayer _disk = null;
        private IMedia _media = null;

        private bool _mute = false;
        private bool _hasClosed = false;

        /// <summary>
        /// Closes the media player and releases all resources.
        /// </summary>
        public void CloseMedia()
        {
            if (_player != null)
            {
                // Close the media player.
                _disk.Stop();
                _disk.Dispose();
                _player.Dispose();
                _media.Dispose();
            }

            _player = null;
            _disk = null;
            _media = null;
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
        /// Open selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOpen_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (buttonOpen.SelectedIndex > -1)
            {
                if (_player == null)
                {
                    string mediaString = "";

                    // Open file.
                    if (buttonOpen.SelectedIndex == 0)
                    {
                        // Set the certificate filter.
                        openFileDialogMain.Filter = "All Files (*.*)|*.*";

                        // Get the file name selected.
                        if (openFileDialogMain.ShowDialog() == DialogResult.OK)
                        {
                            mediaString = openFileDialogMain.FileName;
                        }
                    }

                    // Open folder.
                    if (buttonOpen.SelectedIndex == 1)
                    {
                        // Get the file name selected.
                        if (folderBrowserDialogMain.ShowDialog() == DialogResult.OK)
                        {
                            mediaString = folderBrowserDialogMain.SelectedPath;
                        }
                    }

                    // Open Network.
                    if (buttonOpen.SelectedIndex == 2)
                    {
                        // Open the input box.
                        InputBox input = new InputBox();
                        input.ShowDialog(this);

                        // Get the URL.
                        if (input.InputType == InputType.OK)
                        {
                            mediaString = input.InputValue;
                        }
                    }

                    // Load the media file.
                    LoadMedia(mediaString, buttonOpen.SelectedIndex);
                }
            }
        }

        /// <summary>
        /// player position changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _disk_PlayerPositionChanged(object sender, MediaPlayerPositionChanged e)
        {
            if (_player != null)
                UISync.Execute(() => trackBarMain.Value = (int)(e.NewPosition * 100));
        }

        /// <summary>
        /// Time changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _disk_TimeChanged(object sender, MediaPlayerTimeChanged e)
        {
            if (_player != null)
                // Async executeion.
                UISync.Execute(() => labelTime.Text = TimeSpan.FromSeconds(e.NewTime / 1000).ToString().Substring(0, 8));
        }

        /// <summary>
        /// Duration Changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _media_DurationChanged(object sender, MediaDurationChange e)
        {
            if (_player != null)
                UISync.Execute(() => labelDuration.Text = TimeSpan.FromSeconds(e.NewDuration / 1000).ToString().Substring(0, 8));
        }

        /// <summary>
        /// Media ended.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _disk_MediaEnded(object sender, EventArgs e)
        {
            if (_player != null)
            {
                UISync.Execute(() =>
                    {
                        trackBarMain.Enabled = false;
                        trackBarMain.Value = 0;
                        labelTime.Text = "00:00:00";
                        EnabledOnStopControls(true);
                    });
            }
        }

        /// <summary>
        /// Update the time elasped.
        /// </summary>
        /// <param name="result">The result.</param>
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

        /// <summary>
        /// Close.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClose_Click(object sender, EventArgs e)
        {
            // Close the player.
            ClosePlayer();
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
                _disk.Play();
                EnabledOnPlayControls(true);
            }
        }

        /// <summary>
        /// Stop.
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
                _disk.Pause();
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
                _disk.ToggleMute();
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
            trackBarMain.Enabled = true;
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
            // If the media has not been closed.
            if (!_hasClosed)
            {
                if (_disk != null)
                    _disk.Stop();

                trackBarMain.Enabled = false;
                trackBarMain.Value = 0;
                labelTime.Text = "00:00:00";

                buttonPlay.Enabled = enabled;
                buttonStop.Enabled = !enabled;
                buttonPause.Enabled = !enabled;
                buttonMute.Enabled = !enabled;
            }
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
        /// Track bar scroll.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBarMain_Scroll(object sender, EventArgs e)
        {
            if (_disk != null)
                _disk.Position = (float)trackBarMain.Value / 100;
        }

        /// <summary>
        /// Mouse down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBarMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (_disk != null)
                _disk.Pause();
        }

        /// <summary>
        /// Mouse up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBarMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (_disk != null)
                _disk.Play();
        }

        /// <summary>
        /// Media player drap drop.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaPlayerControl_DragDrop(object sender, DragEventArgs e)
        {
            // Handle FileDrop data.
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Assign the file names to a string array, in 
                // case the user has selected multiple files.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Load the first file.
                if (files != null && files.Length > 0)
                {
                    // Load the media file.
                    LoadMedia(files[0], 0);
                }
            }
        }

        /// <summary>
        /// Media player drap enter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaPlayerControl_DragEnter(object sender, DragEventArgs e)
        {
            // If the data is a file display the copy cursor.
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// Close the player.
        /// </summary>
        private void ClosePlayer()
        {
            if (_player != null)
            {
                trackBarMain.Enabled = false;
                trackBarMain.Value = 0;
                labelTime.Text = "00:00:00";
                labelDuration.Text = "00:00:00";
                EnabledOnCloseControls(false);

                // Close the media player.
                _disk.Stop();
                _disk.Dispose();
                _player.Dispose();
                _media.Dispose();
            }

            _player = null;
            _disk = null;
            _media = null;
            _hasClosed = true;
            this.AllowDrop = true;
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
                    // Create the player.
                    _player = new MediaPlayerFactory(true);
                    _disk = _player.CreatePlayer<IDiskPlayer>();
                    _disk.WindowHandle = this.panelMediaDisplay.Handle;

                    _disk.Events.PlayerPositionChanged += new EventHandler<MediaPlayerPositionChanged>(_disk_PlayerPositionChanged);
                    _disk.Events.TimeChanged += new EventHandler<MediaPlayerTimeChanged>(_disk_TimeChanged);
                    _disk.Events.MediaEnded += new EventHandler(_disk_MediaEnded);
                    _disk.Events.PlayerStopped += new EventHandler(_disk_MediaEnded);

                    // The initialisation sync.
                    UISync.Init(this);

                    // Open the correct media type.
                    switch (mediaIndex)
                    {
                        case 0:
                        case 1:
                            // Open the media file.
                            _media = _player.CreateMedia<IMediaFromFile>(mediaString);
                            break;

                        case 2:
                        default:
                            // Open the media file.
                            _media = _player.CreateMedia<IMedia>(mediaString);
                            break;
                    }

                    // Get the duration changed event.
                    _media.Events.DurationChanged += new EventHandler<MediaDurationChange>(_media_DurationChanged);

                    // Open the media.
                    _disk.Open(_media);
                    _media.Parse(true);

                    // Enable controls.
                    EnabledControls(true);
                    _hasClosed = false;
                    this.AllowDrop = false;
                }
                catch { }
            }
        }
    }
}
