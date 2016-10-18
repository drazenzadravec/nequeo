using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Nequeo.Media;

namespace Nequeo.Directx.UI
{
    /// <summary>
    /// Media capture control.
    /// </summary>
    public partial class CaptureControl : UserControl
    {
        /// <summary>
        /// Media capture control.
        /// </summary>
        public CaptureControl()
        {
            InitializeComponent();
        }

        private Nequeo.Directx.Capture _capture = null;
        private Nequeo.Directx.Utility.DeviceCollection _videoDevices = null;
        private Nequeo.Directx.Utility.DeviceCollection _audioDevices = null;
        private Nequeo.Directx.Utility.DeviceCollection _videoDevicesCompress = null;
        private Nequeo.Directx.Utility.DeviceCollection _audioDevicesCompress = null;
        private Nequeo.Directx.Utility.Device _videoDevice = null;
        private Nequeo.Directx.Utility.Device _audioDevice = null;
        private Nequeo.Directx.Utility.Device _videoDeviceCompress = null;
        private Nequeo.Directx.Utility.Device _audioDeviceCompress = null;
        private Nequeo.Directx.ImageCapture _imageCapture = null;
        private Nequeo.Directx.SoundCapture _soundCapture = null;

        private long _imageStartingIndex = 0;
        private string _imageFileExtension = null;
        private string _imageFileName = null;
        private string _imageFilePath = null;

        private long _soundStartingIndex = 0;
        private string _soundFileExtension = null;
        private string _soundFileName = null;
        private string _soundFilePath = null;

        private System.Timers.Timer _timer = null;
        private bool _start = false;
        private bool _properties = false;
        private bool _preview = false;
        private bool _isVideo = false;
        private bool _isAudio = false;

        private Dictionary<int, bool> _validImage = new Dictionary<int, bool>();
        private Dictionary<int, bool> _validSound = new Dictionary<int, bool>();
        private Dictionary<int, bool> _validImageSound = new Dictionary<int, bool>();

        private Dictionary<string, short> _audioChannels = new Dictionary<string, short>();
        private Dictionary<string, int> _audioSamplingRate = new Dictionary<string, int>();
        private Dictionary<string, short> _audioSampleSize = new Dictionary<string, short>();
        private Dictionary<string, double> _videoFrameRate = new Dictionary<string, double>();
        private Dictionary<string, Size> _videoFrameSize = new Dictionary<string, Size>();

        /// <summary>
        /// Closes the media player and releases all resources.
        /// </summary>
        public void CloseMedia()
        {
            try
            {
                if (_capture != null)
                {
                    // Close the capture engine.
                    _capture.Stop();
                    _capture.Dispose();

                    // Clenaup image. 
                    if (_imageCapture != null)
                    {
                        _imageCapture.Stop();
                        _imageCapture.Dispose();
                    }

                    // Clenaup image. 
                    if (_soundCapture != null)
                    {
                        _soundCapture.Stop();
                        _soundCapture.Dispose();
                    }

                    if (_timer != null)
                    {
                        // Stop the timer.
                        _timer.Enabled = false;
                        _timer.Dispose();
                    }
                }
            }
            catch 
            {
                try
                {
                    // If the engine has been created.
                    if (_capture != null)
                        _capture.Dispose();
                }
                catch { }
            }

            _imageCapture = null;
            _soundCapture = null;
            _capture = null;
            _timer = null;
        }

        /// <summary>
        /// Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CaptureControl_Load(object sender, EventArgs e)
        {
            comboBoxVideoDevice.SelectedIndex = 0;
            comboBoxAudioDevice.SelectedIndex = 0;
            comboBoxVideoDeviceCompress.SelectedIndex = 0;
            comboBoxAudioDeviceCompress.SelectedIndex = 0;

            _validImage.Add(textBoxCaptureImageToFile.GetHashCode(), false);
            _validImage.Add(integerTextBoxCaptureImageToFile.GetHashCode(), true);

            _validSound.Add(textBoxCaptureSoundToFile.GetHashCode(), false);
            _validSound.Add(integerTextBoxCaptureSoundToFile.GetHashCode(), true);

            _validImageSound.Add(textBoxCaptureImageToFile.GetHashCode(), false);
            _validImageSound.Add(integerTextBoxCaptureImageToFile.GetHashCode(), true);
            _validImageSound.Add(textBoxCaptureSoundToFile.GetHashCode(), false);
            _validImageSound.Add(integerTextBoxCaptureSoundToFile.GetHashCode(), true);

            _audioChannels.Add("Mono", 1);
            _audioChannels.Add("Stereo", 2);
            foreach (KeyValuePair<string, short> item in _audioChannels)
                comboBoxAudioChannels.Items.Add(item.Key);

            _audioSamplingRate.Add("8 kHz", 8000);
            _audioSamplingRate.Add("11.025 kHz", 11025);
            _audioSamplingRate.Add("22.05 kHz", 22050);
            _audioSamplingRate.Add("32 kHz", 32000);
            _audioSamplingRate.Add("44.1 kHz", 44100);
            _audioSamplingRate.Add("48 kHz", 48000);
            foreach (KeyValuePair<string, int> item in _audioSamplingRate)
                comboBoxAudioSamplingRate.Items.Add(item.Key);

            _audioSampleSize.Add("8 bit", 8);
            _audioSampleSize.Add("16 bit", 16);
            foreach (KeyValuePair<string, short> item in _audioSampleSize)
                comboBoxAudioSampleSizes.Items.Add(item.Key);

            _videoFrameRate.Add("15 fps", 15000);
            _videoFrameRate.Add("24 fps (Film)", 24000);
            _videoFrameRate.Add("25 fps (PAL)", 25000);
            _videoFrameRate.Add("29.997 fps (NTSC)", 29997);
            _videoFrameRate.Add("30 fps (~NTSC)", 30000);
            _videoFrameRate.Add("59.994 fps (2xNTSC)", 59994);
            foreach (KeyValuePair<string, double> item in _videoFrameRate)
                comboBoxVideoFrameRate.Items.Add(item.Key);

            _videoFrameSize.Add("160 x 120", new Size(160, 120));
            _videoFrameSize.Add("320 x 240", new Size(320, 240));
            _videoFrameSize.Add("352 x 288", new Size(352, 288));
            _videoFrameSize.Add("640 x 480", new Size(640, 480));
            _videoFrameSize.Add("720 x 480", new Size(720, 480));
            _videoFrameSize.Add("768 x 576", new Size(768, 576));
            _videoFrameSize.Add("1024 x 768", new Size(1024, 768));
            foreach (KeyValuePair<string, Size> item in _videoFrameSize)
                comboBoxVideoFrameSize.Items.Add(item.Key);

            comboBoxVideoFrameRate.SelectedIndex = 4;
            comboBoxVideoFrameSize.SelectedIndex = 4;
            comboBoxAudioChannels.SelectedIndex = 1;
            comboBoxAudioSamplingRate.SelectedIndex = 4;
            comboBoxAudioSampleSizes.SelectedIndex = 1;
        }

        /// <summary>
        /// Load all devices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLoadDevice_Click(object sender, EventArgs e)
        {
            // Get all devices.
            Utility.Devices devices = new Utility.Devices();
            _videoDevices = devices.VideoInputDevices;
            _audioDevices = devices.AudioInputDevices;
            _videoDevicesCompress = devices.VideoCompressors;
            _audioDevicesCompress = devices.AudioCompressors;

            comboBoxVideoDevice.Items.Clear();
            comboBoxAudioDevice.Items.Clear();
            comboBoxVideoDeviceCompress.Items.Clear();
            comboBoxAudioDeviceCompress.Items.Clear();

            // Add default items.
            comboBoxVideoDevice.Items.Add("None");
            comboBoxAudioDevice.Items.Add("None");
            comboBoxVideoDeviceCompress.Items.Add("None");
            comboBoxAudioDeviceCompress.Items.Add("None");
            comboBoxVideoDevice.SelectedIndex = 0;
            comboBoxAudioDevice.SelectedIndex = 0;
            comboBoxVideoDeviceCompress.SelectedIndex = 0;
            comboBoxAudioDeviceCompress.SelectedIndex = 0;

            // Add all video devices.
            foreach(Utility.Device device in _videoDevices)
            {
                // Add the device.
                comboBoxVideoDevice.Items.Add(device.DsDevice.Name);
            }

            // Add all audio devices.
            foreach (Utility.Device device in _audioDevices)
            {
                // Add the device.
                comboBoxAudioDevice.Items.Add(device.DsDevice.Name);
            }

            // Add all video compressors.
            foreach (Utility.Device device in _videoDevicesCompress)
            {
                // Add the device.
                comboBoxVideoDeviceCompress.Items.Add(device.DsDevice.Name);
            }

            // Add all audio compressors.
            foreach (Utility.Device device in _audioDevicesCompress)
            {
                // Add the device.
                comboBoxAudioDeviceCompress.Items.Add(device.DsDevice.Name);
            }
        }

        /// <summary>
        /// Video device changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxVideoDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBoxVideoDevice.SelectedIndex > 0)
            {
                // Enable controls.
                panelCapture.Enabled = true;
                panelVideo.Enabled = true;

                // Get the selected device.
                _videoDevice = _videoDevices[comboBoxVideoDevice.SelectedIndex - 1];
            }
            else
            {
                // Enable controls.
                EnablePanelControls();
            }

            // Check what is used.
            CheckVideoAndAudio();
        }

        /// <summary>
        /// Audio device changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxAudioDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxAudioDevice.SelectedIndex > 0)
            {
                // Enable controls.
                panelCapture.Enabled = true;
                panelAudio.Enabled = true;

                // Get the selected device.
                _audioDevice = _audioDevices[comboBoxAudioDevice.SelectedIndex - 1];
            }
            else
            {
                // Enable controls.
                EnablePanelControls();
            }

            // Check what is used.
            CheckVideoAndAudio();
        }

        /// <summary>
        /// Video compressor changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxVideoDeviceCompress_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxVideoDeviceCompress.SelectedIndex > 0)
            {
                // Get the selected device.
                _videoDeviceCompress = _videoDevicesCompress[comboBoxVideoDeviceCompress.SelectedIndex - 1];
            }
            else
                _videoDeviceCompress = null;
        }

        /// <summary>
        /// Audio compressor changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxAudioDeviceCompress_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxAudioDeviceCompress.SelectedIndex > 0)
            {
                // Get the selected device.
                _audioDeviceCompress = _audioDevicesCompress[comboBoxAudioDeviceCompress.SelectedIndex - 1];
            }
            else
                _audioDeviceCompress = null;
        }

        /// <summary>
        /// Check what is used.
        /// </summary>
        private void CheckVideoAndAudio()
        {
            // If using video.
            if (comboBoxVideoDevice.SelectedIndex > 0)
                _isVideo = true;
            else
                _isVideo = false;

            // If using audio.
            if (comboBoxAudioDevice.SelectedIndex > 0)
                _isAudio = true;
            else
                _isAudio = false;

            // If a video or audio device exists then enable preview.
            if (_isVideo || _isAudio)
                buttonStartPreview.Enabled = true;
            else
                buttonStartPreview.Enabled = false;
        }

        /// <summary>
        /// Check to see if any checked box has been checked.
        /// </summary>
        /// <param name="textBox">The current text box instance.</param>
        private void CheckIfToFileHasBeenChecked(TextBox textBox = null)
        {
            // If capture is checked
            if (checkBoxCatureToFile.CheckState == CheckState.Checked)
            {
                // Check capture to file.
                if (!String.IsNullOrEmpty(textBoxCaptureToFile.Text))
                    buttonStartCapture.Enabled = true;
                else
                    buttonStartCapture.Enabled = false;
            }
            else
            {
                // If image and sound is checked
                if ((checkBoxCaptureImageToFile.CheckState == CheckState.Checked) &&
                    (checkBoxCaptureSoundToFile.CheckState == CheckState.Checked))
                {
                    // Check image and sound to file.
                    ValidImageSound(textBox);

                    // Check image and sound to file.
                    ValidImage(textBox);

                    // Check image and sound to file.
                    ValidSound(textBox);
                }
                else if (checkBoxCaptureImageToFile.CheckState == CheckState.Checked)
                {
                    // Check image and sound to file.
                    ValidImage(textBox);
                }
                else if (checkBoxCaptureSoundToFile.CheckState == CheckState.Checked)
                {
                    // Check image and sound to file.
                    ValidSound(textBox);
                }
            }

            // If nothing has been checked.
            if ((checkBoxCatureToFile.CheckState == CheckState.Unchecked) &&
                    (checkBoxCaptureImageToFile.CheckState == CheckState.Unchecked) &&
                    (checkBoxCaptureSoundToFile.CheckState == CheckState.Unchecked))
            {
                buttonStartCapture.Enabled = false;
            }
        }

        /// <summary>
        /// Enable or disable the valid input data.
        /// </summary>
        /// <param name="textBox">The current text box instance.</param>
        private void ValidImage(TextBox textBox = null)
        {
            bool isValid = true;

            if (textBox != null)
            {
                // If no text has been add then invalid.
                if (String.IsNullOrEmpty(textBox.Text))
                    isValid = false;

                // Add the current validation item.
                if (!_validImage.Keys.Contains(textBox.GetHashCode()))
                    _validImage.Add(textBox.GetHashCode(), isValid);
                else
                    _validImage[textBox.GetHashCode()] = isValid;
            }

            // Find all invalid items, if no invalid items then
            // enable operation controls.
            IEnumerable<bool> inValid = _validImage.Values.Where(u => (u == false));
            if ((inValid.Count() < 1))
                buttonStartCapture.Enabled = true;
            else
                buttonStartCapture.Enabled = false;
        }

        /// <summary>
        /// Enable or disable the valid input data.
        /// </summary>
        /// <param name="textBox">The current text box instance</param>
        private void ValidSound(TextBox textBox = null)
        {
            bool isValid = true;

            if (textBox != null)
            {
                // If no text has been add then invalid.
                if (String.IsNullOrEmpty(textBox.Text))
                    isValid = false;

                // Add the current validation item.
                if (!_validSound.Keys.Contains(textBox.GetHashCode()))
                    _validSound.Add(textBox.GetHashCode(), isValid);
                else
                    _validSound[textBox.GetHashCode()] = isValid;
            }

            // Find all invalid items, if no invalid items then
            // enable operation controls.
            IEnumerable<bool> inValid = _validSound.Values.Where(u => (u == false));
            if ((inValid.Count() < 1))
                buttonStartCapture.Enabled = true;
            else
                buttonStartCapture.Enabled = false;
        }

        /// <summary>
        /// Enable or disable the valid input data.
        /// </summary>
        /// <param name="textBox">The current text box instance</param>
        private void ValidImageSound(TextBox textBox = null)
        {
            bool isValid = true;

            if (textBox != null)
            {
                // If no text has been add then invalid.
                if (String.IsNullOrEmpty(textBox.Text))
                    isValid = false;

                // Add the current validation item.
                if (!_validImageSound.Keys.Contains(textBox.GetHashCode()))
                    _validImageSound.Add(textBox.GetHashCode(), isValid);
                else
                    _validImageSound[textBox.GetHashCode()] = isValid;
            }

            // Find all invalid items, if no invalid items then
            // enable operation controls.
            IEnumerable<bool> inValid = _validImageSound.Values.Where(u => (u == false));
            if ((inValid.Count() < 1))
                buttonStartCapture.Enabled = true;
            else
                buttonStartCapture.Enabled = false;
        }

        /// <summary>
        /// Enable panel controls.
        /// </summary>
        private void EnablePanelControls()
        {
            if ((comboBoxVideoDevice.SelectedIndex <= 0) && (comboBoxAudioDevice.SelectedIndex <= 0))
            {
                panelCapture.Enabled = false;
                panelVideo.Enabled = false;
                panelAudio.Enabled = false;

                checkBoxCaptureImageToFile.Enabled = false;
                checkBoxCaptureImageToFile.Checked = false;
                checkBoxCaptureImageToFile.CheckState = CheckState.Unchecked;

                checkBoxCaptureSoundToFile.Enabled = false;
                checkBoxCaptureSoundToFile.Checked = false;
                checkBoxCaptureSoundToFile.CheckState = CheckState.Unchecked;
            }
            else
            {
                if (comboBoxVideoDevice.SelectedIndex <= 0)
                {
                    panelVideo.Enabled = false;

                    checkBoxCaptureImageToFile.Enabled = false;
                    checkBoxCaptureImageToFile.Checked = false;
                    checkBoxCaptureImageToFile.CheckState = CheckState.Unchecked;
                }

                if (comboBoxAudioDevice.SelectedIndex <= 0)
                {
                    panelAudio.Enabled = false;

                    checkBoxCaptureSoundToFile.Enabled = false;
                    checkBoxCaptureSoundToFile.Checked = false;
                    checkBoxCaptureSoundToFile.CheckState = CheckState.Unchecked;
                }
            }
        }

        /// <summary>
        /// Checked changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxCatureToFile_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCatureToFile.CheckState == CheckState.Checked)
            {
                textBoxCaptureToFile.Enabled = true;
                buttonCaptureToFile.Enabled = true;

                checkBoxCaptureImageToFile.Enabled = false;
                checkBoxCaptureImageToFile.Checked = false;
                checkBoxCaptureImageToFile.CheckState = CheckState.Unchecked;

                checkBoxCaptureSoundToFile.Enabled = false;
                checkBoxCaptureSoundToFile.Checked = false;
                checkBoxCaptureSoundToFile.CheckState = CheckState.Unchecked;
            }
            else
            {
                textBoxCaptureToFile.Enabled = false;
                buttonCaptureToFile.Enabled = false;

                // If a video device has been selected.
                if (_isVideo)
                    checkBoxCaptureImageToFile.Enabled = true;

                // If a audio device has been selected.
                if (_isAudio)
                    checkBoxCaptureSoundToFile.Enabled = true;
            }

            // Check checked.
            CheckIfToFileHasBeenChecked();
        }

        /// <summary>
        /// Checked changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxCaptureImageToFile_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCaptureImageToFile.CheckState == CheckState.Checked)
            {
                textBoxCaptureImageToFile.Enabled = true;
                buttonCaptureImageToFile.Enabled = true;
                integerTextBoxCaptureImageToFile.Enabled = true;
            }
            else if (checkBoxCaptureImageToFile.CheckState == CheckState.Unchecked)
            {
                textBoxCaptureImageToFile.Enabled = false;
                buttonCaptureImageToFile.Enabled = false;
                integerTextBoxCaptureImageToFile.Enabled = false;
            }

            // Check checked.
            CheckIfToFileHasBeenChecked();
        }

        /// <summary>
        /// Checked changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxCaptureSoundToFile_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCaptureSoundToFile.CheckState == CheckState.Checked)
            {
                textBoxCaptureSoundToFile.Enabled = true;
                buttonCaptureSoundToFile.Enabled = true;
                integerTextBoxCaptureSoundToFile.Enabled = true;
            }
            else if (checkBoxCaptureSoundToFile.CheckState == CheckState.Unchecked)
            {
                textBoxCaptureSoundToFile.Enabled = false;
                buttonCaptureSoundToFile.Enabled = false;
                integerTextBoxCaptureSoundToFile.Enabled = false;
            }

            // Check checked.
            CheckIfToFileHasBeenChecked();
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxCaptureToFile_TextChanged(object sender, EventArgs e)
        {
            // Check checked.
            CheckIfToFileHasBeenChecked((TextBox)sender);
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxCaptureImageToFile_TextChanged(object sender, EventArgs e)
        {
            // Check checked.
            CheckIfToFileHasBeenChecked((TextBox)sender);
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void integerTextBoxCaptureImageToFile_TextChanged(object sender, EventArgs e)
        {
            // Check checked.
            CheckIfToFileHasBeenChecked((TextBox)sender);
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxCaptureSoundToFile_TextChanged(object sender, EventArgs e)
        {
            // Check checked.
            CheckIfToFileHasBeenChecked((TextBox)sender);
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void integerTextBoxCaptureSoundToFile_TextChanged(object sender, EventArgs e)
        {
            // Check checked.
            CheckIfToFileHasBeenChecked((TextBox)sender);
        }

        /// <summary>
        /// On clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCaptureToFile_Click(object sender, EventArgs e)
        {
            if (_isVideo && _isAudio)
                saveFileDialogMain.Filter = "Video Files (*.wmv *.avi)|*.wmv;*.avi";
            else if(_isVideo)
                saveFileDialogMain.Filter = "Video Files (*.wmv *.avi)|*.wmv;*.avi";
            else if(_isAudio)
                saveFileDialogMain.Filter = "Audio Files (*.wav)|*.wav";

            saveFileDialogMain.RestoreDirectory = true;

            // Get the file name selected.
            if (saveFileDialogMain.ShowDialog() == DialogResult.OK)
                textBoxCaptureToFile.Text = saveFileDialogMain.FileName;
        }

        /// <summary>
        /// On clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCaptureImageToFile_Click(object sender, EventArgs e)
        {
            saveFileDialogMain.Filter = "Image Files (*.jpg; *.bmp)|*.jpg; *.bmp";
            saveFileDialogMain.RestoreDirectory = true;

            // Get the file name selected.
            if (saveFileDialogMain.ShowDialog() == DialogResult.OK)
                textBoxCaptureImageToFile.Text = saveFileDialogMain.FileName;
        }

        /// <summary>
        /// On clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCaptureSoundToFile_Click(object sender, EventArgs e)
        {
            saveFileDialogMain.Filter = "Sound Files (*.wav; *.pcm)|*.wav; *.pcm";
            saveFileDialogMain.RestoreDirectory = true;

            // Get the file name selected.
            if (saveFileDialogMain.ShowDialog() == DialogResult.OK)
                textBoxCaptureSoundToFile.Text = saveFileDialogMain.FileName;
        }

        /// <summary>
        /// On clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStartPreview_Click(object sender, EventArgs e)
        {
            try
            {
                // Create and configure the capture engine.
                CreateAndConfigureCaptureEngine();

                // If the engine has been created.
                if (_capture != null)
                {
                    if (!_preview)
                    {
                        _preview = true;
                        buttonStartPreview.Text = "Preview Off";
                        _capture.PreviewWindow = this.panelMain;
                    }
                    else
                    {
                        _preview = false;
                        buttonStartPreview.Text = "Preview";

                        // Clenup.
                        _capture.Stop();
                        _capture.Dispose();
                        _capture = null;
                    }
                }
            }
            catch (Exception ex)
            {
                // Display error.
                MessageBox.Show(ex.Message, "Capture", MessageBoxButtons.OK, MessageBoxIcon.Error);

                try
                {
                    // If the engine has been created.
                    if (_capture != null)
                        _capture.Dispose();
                }
                catch { }
                _capture = null;
            }
        }

        /// <summary>
        /// On Start.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStartCapture_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_start)
                {
                    // Create and configure the capture engine.
                    CreateAndConfigureCaptureEngine();

                    // If the engine has been created.
                    if (_capture != null)
                    {
                        // Start the capture.
                        if (checkBoxCatureToFile.CheckState == CheckState.Checked)
                        {
                            // Get the file extension.
                            string fileExtension = System.IO.Path.GetExtension(textBoxCaptureToFile.Text);

                            // Select the extension.
                            switch (fileExtension.ToLower())
                            {
                                case ".wav":
                                    // Audio.
                                    _capture.StartToFile(textBoxCaptureToFile.Text, MediaFormatType.Wav);
                                    break;

                                case ".avi":
                                    // Video and audio.
                                    _capture.StartToFile(textBoxCaptureToFile.Text, MediaFormatType.Avi);
                                    break;

                                default:
                                case ".wmv":
                                    // Video and audio.
                                    _capture.StartToFile(textBoxCaptureToFile.Text, MediaFormatType.Wmv);
                                    break;
                            }
                        }
                        else if ((checkBoxCaptureImageToFile.CheckState == CheckState.Checked) &&
                            (checkBoxCaptureSoundToFile.CheckState == CheckState.Checked))
                        {
                            // Start sample capture.
                            _capture.StartSnapshotImageSound();

                            // Create the samplers.
                            ImageSampler();
                            SoundSampler();
                        }
                        else if (checkBoxCaptureImageToFile.CheckState == CheckState.Checked)
                        {
                            // Start sample capture.
                            _capture.StartSnapshotImage();

                            // Create the samplers.
                            ImageSampler();
                        }
                        else if (checkBoxCaptureSoundToFile.CheckState == CheckState.Checked)
                        {
                            // Start sample capture.
                            _capture.StartSnapshotSound();

                            // Create the samplers.
                            SoundSampler();
                        }
                        
                        // Started.
                        _start = true;
                        buttonStartCapture.Text = "Stop";

                        // Process has started.
                        _preview = false;
                        buttonStartPreview.Text = "Preview";
                        buttonStartPreview.Enabled = false;
                    }
                }
                else
                {
                    _start = false;
                    buttonStartCapture.Text = "Start";

                    // If the engine has been created.
                    if (_capture != null)
                    {
                        _capture.Stop();
                        _capture.Dispose();

                        // Clenaup image. 
                        if (_imageCapture != null)
                        {
                            _imageCapture.Stop();
                            _imageCapture.Dispose();
                        }

                        // Clenaup image. 
                        if (_soundCapture != null)
                        {
                            _soundCapture.Stop();
                            _soundCapture.Dispose();
                        }

                        _imageCapture = null;
                        _soundCapture = null;
                        _capture = null;
                    }

                    // Process has stopped.
                    _preview = false;
                    buttonStartPreview.Text = "Preview";
                    buttonStartPreview.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                // Display error.
                MessageBox.Show(ex.Message, "Capture", MessageBoxButtons.OK, MessageBoxIcon.Error);

                try
                {
                    // If the engine has been created.
                    if (_capture != null)
                        _capture.Dispose();
                }
                catch { }

                _imageCapture = null;
                _soundCapture = null;
                _capture = null;
            }
        }

        /// <summary>
        /// Create and configure the capture engine.
        /// </summary>
        private void CreateAndConfigureCaptureEngine()
        {
            // Create the capture engine.
            if (_capture == null && (_videoDevice != null || _audioDevice != null))
                _capture = new Capture(_videoDevice, _audioDevice);

            // If the engine has been created.
            if (_capture != null)
            {
                // Build the graph.
                if (!_capture.Cued)
                    _capture.Cue();

                // Add video compressors
                if (_videoDeviceCompress != null)
                    _capture.VideoCompressor = _videoDeviceCompress;

                // Add audio compressors
                if (_audioDeviceCompress != null)
                    _capture.AudioCompressor = _audioDeviceCompress;

                // Get the video and audio sources details.
                Nequeo.Directx.Utility.SourceCollection videoSources = _capture.VideoSources;
                Nequeo.Directx.Utility.SourceCollection audioSources = _capture.AudioSources;

                // If not looking at properties.
                if (!_properties)
                {
                    // If and video device has been selected.
                    if (_isVideo && checkBoxVideoDefaultSettings.CheckState == CheckState.Unchecked)
                    {
                        // Video Frame Rate.
                        if (comboBoxVideoFrameRate.SelectedIndex > -1)
                            _capture.FrameRate = VideoFrameRate();

                        // Video Frame Size.
                        if (comboBoxVideoFrameSize.SelectedIndex > -1)
                            _capture.FrameSize = VideoFrameSize();
                    }

                    // If and audio device has been selected.
                    if (_isAudio && checkBoxAudioUseDefaultSettings.CheckState == CheckState.Unchecked)
                    {
                        // Audio channels
                        if (comboBoxAudioChannels.SelectedIndex > -1)
                            _capture.AudioChannels = AudioChannels();

                        // Audio Sampling Rate.
                        if (comboBoxAudioSamplingRate.SelectedIndex > -1)
                            _capture.AudioSamplingRate = AudioSamplingRate();

                        // Audio Sample Size.
                        if (comboBoxAudioSampleSizes.SelectedIndex > -1)
                            _capture.AudioSampleSize = AudioSampleSize();
                    }
                }
            }
        }

        /// <summary>
        /// Get the selected video frame rate.
        /// </summary>
        /// <returns>The frmae rate.</returns>
        private double VideoFrameRate()
        {
            // Get the selected frame rate.
            string frameRate = comboBoxVideoFrameRate.SelectedItem.ToString();
            IEnumerable<KeyValuePair<string, double>> items = _videoFrameRate.Where(u => u.Key == frameRate);
            if (items != null && items.Count() > 0)
                return items.First().Value;
            else
                // 30 fps (~NTSC).
                return 30000;
        }

        /// <summary>
        /// Get the selected video frame size.
        /// </summary>
        /// <returns>The frmae size.</returns>
        private Size VideoFrameSize()
        {
            // Get the selected frame size.
            string frameSize = comboBoxVideoFrameSize.SelectedItem.ToString();
            IEnumerable<KeyValuePair<string, Size>> items = _videoFrameSize.Where(u => u.Key == frameSize);
            if (items != null && items.Count() > 0)
                return items.First().Value;
            else
                // 640 x 480.
                return new Size(640, 480);
        }

        /// <summary>
        /// Get the selected audio channel.
        /// </summary>
        /// <returns>The channels.</returns>
        private short AudioChannels()
        {
            // Get the selected channel.
            string channels = comboBoxAudioChannels.SelectedItem.ToString();
            IEnumerable<KeyValuePair<string, short>> items = _audioChannels.Where(u => u.Key == channels);
            if (items != null && items.Count() > 0)
                return items.First().Value;
            else
                // Stereo.
                return 2;
        }

        /// <summary>
        /// Get the selected sampling rate.
        /// </summary>
        /// <returns>The sampling rate.</returns>
        private int AudioSamplingRate()
        {
            // Get the selected sampling rate.
            string samplingRate = comboBoxAudioSamplingRate.SelectedItem.ToString();
            IEnumerable<KeyValuePair<string, int>> items = _audioSamplingRate.Where(u => u.Key == samplingRate);
            if (items != null && items.Count() > 0)
                return items.First().Value;
            else
                // 44.1 kHz.
                return 44100;
        }

        /// <summary>
        /// Get the selected sample size.
        /// </summary>
        /// <returns>The sample size.</returns>
        private short AudioSampleSize()
        {
            // Get the selected sample sizes.
            string sampleSizes = comboBoxAudioSampleSizes.SelectedItem.ToString();
            IEnumerable<KeyValuePair<string, short>> items = _audioSampleSize.Where(u => u.Key == sampleSizes);
            if (items != null && items.Count() > 0)
                return items.First().Value;
            else
                // 16 bit.
                return 16;
        }

        /// <summary>
        /// Create the image sampler.
        /// </summary>
        private void ImageSampler()
        {
            _imageFileExtension = System.IO.Path.GetExtension(textBoxCaptureImageToFile.Text);
            _imageFileName = System.IO.Path.GetFileNameWithoutExtension(textBoxCaptureImageToFile.Text);
            _imageFilePath = System.IO.Path.GetDirectoryName(textBoxCaptureImageToFile.Text).TrimEnd(new char[] { '\\' }) + "\\";
            _imageStartingIndex = Int64.Parse(integerTextBoxCaptureImageToFile.Text);

            // Get the capture sampler.
            _imageCapture = new ImageCapture(_capture.CaptureImageSample);

            // Select the extension.
            switch (_imageFileExtension.ToLower())
            {
                case ".bmp":
                    // Set the method that is used for sampling.
                    _imageCapture.ImageCaptureThreadContext.Execute(a => a.StartContinuous(u => GetImageData(u), ImageCaptureType.Bmp));
                    break;

                default:
                case ".jpg":
                    // Set the method that is used for sampling.
                    _imageCapture.ImageCaptureThreadContext.Execute(a => a.StartContinuous(u => GetImageData(u), ImageCaptureType.Jpg));
                    break;
            }
        }

        /// <summary>
        /// Create the sound sampler.
        /// </summary>
        private void SoundSampler()
        {
            _soundFileExtension = System.IO.Path.GetExtension(textBoxCaptureSoundToFile.Text);
            _soundFileName = System.IO.Path.GetFileNameWithoutExtension(textBoxCaptureSoundToFile.Text);
            _soundFilePath = System.IO.Path.GetDirectoryName(textBoxCaptureSoundToFile.Text).TrimEnd(new char[] { '\\' }) + "\\";
            _soundStartingIndex = Int64.Parse(integerTextBoxCaptureSoundToFile.Text);

            // Get the capture sampler.
            _soundCapture = new SoundCapture(_capture.CaptureSoundSample);

            // Select the extension.
            switch (_soundFileExtension.ToLower())
            {
                case ".pcm":
                    // Set the method that is used for sampling.
                    _soundCapture.SoundCaptureThreadContext.Execute(a => a.StartContinuous(u => GetSoundData(u), SoundCaptureType.Pcm));
                    break;

                default:
                case ".wav":
                    // Set the method that is used for sampling.
                    _soundCapture.SoundCaptureThreadContext.Execute(a => a.StartContinuous(u => GetSoundData(u), SoundCaptureType.Wav));
                    break;
            }
        }

        /// <summary>
        /// Get image sample.
        /// </summary>
        /// <param name="stream">The stream containg the image data.</param>
        private void GetImageData(System.IO.MemoryStream stream)
        {
            System.IO.FileStream file = null;
            try
            {
                // Write the data to the file.
                file = new System.IO.FileStream(_imageFilePath + _imageFileName + "_" + _imageStartingIndex.ToString() + _imageFileExtension, System.IO.FileMode.Create);
                byte[] buffer = stream.ToArray();
                file.Write(buffer, 0, buffer.Length);
                file.Flush();
            }
            catch { }
            finally
            {
                if (file != null)
                    file.Close();
            }

            // Increment.
            _imageStartingIndex++;
        }

        /// <summary>
        /// Get sound sample.
        /// </summary>
        /// <param name="stream">The stream containg the sound data.</param>
        private void GetSoundData(System.IO.MemoryStream stream)
        {
            System.IO.FileStream file = null;
            try
            {
                // Write the data to the file.
                file = new System.IO.FileStream(_soundFilePath + _soundFileName + "_" + _soundStartingIndex.ToString() + _soundFileExtension, System.IO.FileMode.Create);
                byte[] buffer = stream.ToArray();
                file.Write(buffer, 0, buffer.Length);
                file.Flush();
            }
            catch { }
            finally
            {
                if (file != null)
                    file.Close();
            }

            // Increment.
            _soundStartingIndex++;
        }

        /// <summary>
        /// Capture properties.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCaptureProperties_Click(object sender, EventArgs e)
        {
            try
            {
                _properties = true;

                // Create and configure the capture engine.
                CreateAndConfigureCaptureEngine();

                // If the engine has been created.
                if (_capture != null)
                {
                    StringBuilder builder = new StringBuilder();

                    // Get all video data.
                    if(_capture.VideoDevice != null)
                    {
                        builder.Append("\r\nVideo:" + "\r\n");
                        builder.Append("\tName: " + _capture.VideoDevice.DsDevice.Name + "\r\n");
                        builder.Append("\tDevice Path: " + _capture.VideoDevice.DsDevice.DevicePath + "\r\n");
                        builder.Append("\tClassID: " + _capture.VideoDevice.DsDevice.ClassID.ToString() + "\r\n");

                        if(_capture.VideoCompressor != null)
                        {
                            builder.Append("\r\n\tVideo Compressor:" + "\r\n");
                            builder.Append("\t\tName: " + _capture.VideoCompressor.DsDevice.Name + "\r\n");
                            builder.Append("\t\tDevice Path: " + _capture.VideoCompressor.DsDevice.DevicePath + "\r\n");
                            builder.Append("\t\tClassID: " + _capture.VideoCompressor.DsDevice.ClassID.ToString() + "\r\n");
                        }

                        // Get all video capabilities.
                        Nequeo.Directx.Utility.VideoCapabilities videoCapabilities = _capture.VideoCaps;
                        if(videoCapabilities != null)
                        {
                            builder.Append("\r\n\tVideo Capabilities:" + "\r\n");
                            builder.Append("\t\tFrame Size Granularity X: " + videoCapabilities.FrameSizeGranularityX.ToString() + "\r\n");
                            builder.Append("\t\tFrame Size Granularity Y: " + videoCapabilities.FrameSizeGranularityY.ToString() + "\r\n");
                            builder.Append("\t\tMaximum Frame Rate: " + videoCapabilities.MaxFrameRate.ToString() + "\r\n");
                            builder.Append("\t\tMaximum Frame Size: " + videoCapabilities.MaxFrameSize.ToString() + "\r\n");
                            builder.Append("\t\tMinimum Frame Rate: " + videoCapabilities.MinFrameRate.ToString() + "\r\n");
                            builder.Append("\t\tMinimum Frame Size: " + videoCapabilities.MinFrameSize.ToString() + "\r\n");
                        }
                    }

                    // Get all audio data.
                    if (_capture.AudioDevice != null)
                    {
                        builder.Append("\r\nAudio:" + "\r\n");
                        builder.Append("\tName: " + _capture.AudioDevice.DsDevice.Name + "\r\n");
                        builder.Append("\tDevice Path: " + _capture.AudioDevice.DsDevice.DevicePath + "\r\n");
                        builder.Append("\tClassID: " + _capture.AudioDevice.DsDevice.ClassID.ToString() + "\r\n");

                        if (_capture.AudioCompressor != null)
                        {
                            builder.Append("\r\n\tAudio Compressor:" + "\r\n");
                            builder.Append("\t\tName: " + _capture.AudioCompressor.DsDevice.Name + "\r\n");
                            builder.Append("\t\tDevice Path: " + _capture.AudioCompressor.DsDevice.DevicePath + "\r\n");
                            builder.Append("\t\tClassID: " + _capture.AudioCompressor.DsDevice.ClassID.ToString() + "\r\n");
                        }

                        // Get all video capabilities.
                        Nequeo.Directx.Utility.AudioCapabilities audioCapabilities = _capture.AudioCaps;
                        if (audioCapabilities != null)
                        {
                            builder.Append("\r\n\tAudio Capabilities:" + "\r\n");
                            builder.Append("\t\tChannels Granularity: " + audioCapabilities.ChannelsGranularity.ToString() + "\r\n");
                            builder.Append("\t\tMaximum Channels: " + audioCapabilities.MaximumChannels.ToString() + "\r\n");
                            builder.Append("\t\tMaximum Sample Size: " + audioCapabilities.MaximumSampleSize.ToString() + "\r\n");
                            builder.Append("\t\tMaximum Sampling Rate: " + audioCapabilities.MaximumSamplingRate.ToString() + "\r\n");
                            builder.Append("\t\tMinimum Channels: " + audioCapabilities.MinimumChannels.ToString() + "\r\n");
                            builder.Append("\t\tMinimum Sample Size: " + audioCapabilities.MinimumSampleSize.ToString() + "\r\n");
                            builder.Append("\t\tMinimum Sampling Rate: " + audioCapabilities.MinimumSamplingRate.ToString() + "\r\n");
                            builder.Append("\t\tSample Size Granularity: " + audioCapabilities.SampleSizeGranularity.ToString() + "\r\n");
                            builder.Append("\t\tSampling Rate Granularity: " + audioCapabilities.SamplingRateGranularity.ToString() + "\r\n");
                        }
                    }

                    // Get the video and audio sources details.
                    Nequeo.Directx.Utility.SourceCollection videoSources = _capture.VideoSources;
                    Nequeo.Directx.Utility.SourceCollection audioSources = _capture.AudioSources;

                    // Get the collection of items.
                    Nequeo.Directx.Utility.PropertyPageCollection propColl = _capture.PropertyPages;
                    for (int i = 0; i < propColl.Count; i++)
                        builder.Append("\r\nProperty Name : " + propColl[i].Name + "\r\n");

                    // Show the details window.
                    Nequeo.Directx.UI.DetailsWindow detailsWindow = new DetailsWindow();
                    detailsWindow.SetDetailData(builder.ToString().Trim());
                    detailsWindow.ShowDialog(this);

                    // If the engine has been created.
                    if (_capture != null)
                    {
                        _capture.Stop();
                        _capture.Dispose();
                        _capture = null;
                    }
                }
            }
            catch (Exception ex)
            {
                // Display error.
                MessageBox.Show(ex.Message, "Capture", MessageBoxButtons.OK, MessageBoxIcon.Error);

                try
                {
                    // If the engine has been created.
                    if (_capture != null)
                        _capture.Dispose();
                }
                catch { }
                _capture = null;
            }
            _properties = false;
        }

        /// <summary>
        /// Selected index changed. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxAudioChannels_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Selected index changed. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxAudioSamplingRate_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Selected index changed. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxAudioSampleSizes_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxAudioChannelsName_TextChanged(object sender, EventArgs e)
        {
            if ((String.IsNullOrEmpty(textBoxAudioChannelsName.Text)) || (String.IsNullOrEmpty(textBoxAudioChannelsValue.Text)))
                buttonAudioChannelsAdd.Enabled = false;
            else
                buttonAudioChannelsAdd.Enabled = true;
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxAudioChannelsValue_TextChanged(object sender, EventArgs e)
        {
            if ((String.IsNullOrEmpty(textBoxAudioChannelsName.Text)) || (String.IsNullOrEmpty(textBoxAudioChannelsValue.Text)))
                buttonAudioChannelsAdd.Enabled = false;
            else
                buttonAudioChannelsAdd.Enabled = true;
        }

        /// <summary>
        /// Add channels.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAudioChannelsAdd_Click(object sender, EventArgs e)
        {
            string channelsName = textBoxAudioChannelsName.Text;
            short channelsValue = short.Parse(textBoxAudioChannelsValue.Text);
            if (!_audioChannels.ContainsKey(channelsName))
            {
                // Add the new item.
                _audioChannels.Add(channelsName, channelsValue);
                comboBoxAudioChannels.Items.Add(channelsName);
            }
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxAudioSamplingRateName_TextChanged(object sender, EventArgs e)
        {
            if ((String.IsNullOrEmpty(textBoxAudioSamplingRateName.Text)) || (String.IsNullOrEmpty(textBoxAudioSamplingRateValue.Text)))
                buttonAudioSamplingRateAdd.Enabled = false;
            else
                buttonAudioSamplingRateAdd.Enabled = true;
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxAudioSamplingRateValue_TextChanged(object sender, EventArgs e)
        {
            if ((String.IsNullOrEmpty(textBoxAudioSamplingRateName.Text)) || (String.IsNullOrEmpty(textBoxAudioSamplingRateValue.Text)))
                buttonAudioSamplingRateAdd.Enabled = false;
            else
                buttonAudioSamplingRateAdd.Enabled = true;
        }

        /// <summary>
        /// Add sampling rate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAudioSamplingRateAdd_Click(object sender, EventArgs e)
        {
            string samplingRateName = textBoxAudioSamplingRateName.Text;
            int samplingRateValue = int.Parse(textBoxAudioSamplingRateValue.Text);
            if (!_audioSamplingRate.ContainsKey(samplingRateName))
            {
                // Add the new item.
                _audioSamplingRate.Add(samplingRateName, samplingRateValue);
                comboBoxAudioSamplingRate.Items.Add(samplingRateName);
            }
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxAudioSampleSizeName_TextChanged(object sender, EventArgs e)
        {
            if ((String.IsNullOrEmpty(textBoxAudioSampleSizeName.Text)) || (String.IsNullOrEmpty(textBoxAudioSampleSizeValue.Text)))
                buttonAudioSampleSizeAdd.Enabled = false;
            else
                buttonAudioSampleSizeAdd.Enabled = true;
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxAudioSampleSizeValue_TextChanged(object sender, EventArgs e)
        {
            if ((String.IsNullOrEmpty(textBoxAudioSampleSizeName.Text)) || (String.IsNullOrEmpty(textBoxAudioSampleSizeValue.Text)))
                buttonAudioSampleSizeAdd.Enabled = false;
            else
                buttonAudioSampleSizeAdd.Enabled = true;
        }

        /// <summary>
        /// Add sample size.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAudioSampleSizeAdd_Click(object sender, EventArgs e)
        {
            string sampleSizeName = textBoxAudioSampleSizeName.Text;
            short sampleSizeValue = short.Parse(textBoxAudioSampleSizeValue.Text);
            if (!_audioSampleSize.ContainsKey(sampleSizeName))
            {
                // Add the new item.
                _audioSampleSize.Add(sampleSizeName, sampleSizeValue);
                comboBoxAudioSampleSizes.Items.Add(sampleSizeName);
            }
        }

        /// <summary>
        /// Selected index changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxVideoFrameRate_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Selected index changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxVideoFrameSize_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxVideoFrameRateName_TextChanged(object sender, EventArgs e)
        {
            if ((String.IsNullOrEmpty(textBoxVideoFrameRateName.Text)) || (String.IsNullOrEmpty(textBoxVideoFrameRateValue.Text)))
                buttonVideoFrameRateAdd.Enabled = false;
            else
                buttonVideoFrameRateAdd.Enabled = true;
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxVideoFrameRateValue_TextChanged(object sender, EventArgs e)
        {
            if ((String.IsNullOrEmpty(textBoxVideoFrameRateName.Text)) || (String.IsNullOrEmpty(textBoxVideoFrameRateValue.Text)))
                buttonVideoFrameRateAdd.Enabled = false;
            else
                buttonVideoFrameRateAdd.Enabled = true;
        }

        /// <summary>
        /// Add frame rate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonVideoFrameRateAdd_Click(object sender, EventArgs e)
        {
            string frameRateName = textBoxVideoFrameRateName.Text;
            double frameRateValue = double.Parse(textBoxVideoFrameRateValue.Text);
            if (!_audioSampleSize.ContainsKey(frameRateName))
            {
                // Add the new item.
                _videoFrameRate.Add(frameRateName, frameRateValue);
                comboBoxVideoFrameRate.Items.Add(frameRateName);
            }
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxVideoFrameSizeName_TextChanged(object sender, EventArgs e)
        {
            if ((String.IsNullOrEmpty(textBoxVideoFrameSizeName.Text)) ||
                (String.IsNullOrEmpty(textBoxVideoFrameSizeWidth.Text)) ||
                (String.IsNullOrEmpty(textBoxVideoFrameSizeHeight.Text)))

                buttonVideoFrameSizeAdd.Enabled = false;
            else
                buttonVideoFrameSizeAdd.Enabled = true;
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxVideoFrameSizeWidth_TextChanged(object sender, EventArgs e)
        {
            if ((String.IsNullOrEmpty(textBoxVideoFrameSizeName.Text)) ||
                (String.IsNullOrEmpty(textBoxVideoFrameSizeWidth.Text)) ||
                (String.IsNullOrEmpty(textBoxVideoFrameSizeHeight.Text)))

                buttonVideoFrameSizeAdd.Enabled = false;
            else
                buttonVideoFrameSizeAdd.Enabled = true;
        }

        /// <summary>
        /// Text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxVideoFrameSizeHeight_TextChanged(object sender, EventArgs e)
        {
            if ((String.IsNullOrEmpty(textBoxVideoFrameSizeName.Text)) ||
                (String.IsNullOrEmpty(textBoxVideoFrameSizeWidth.Text)) ||
                (String.IsNullOrEmpty(textBoxVideoFrameSizeHeight.Text)))

                buttonVideoFrameSizeAdd.Enabled = false;
            else
                buttonVideoFrameSizeAdd.Enabled = true;
        }

        /// <summary>
        /// Add frame size.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonVideoFrameSizeAdd_Click(object sender, EventArgs e)
        {
            string frameSizeName = textBoxVideoFrameSizeName.Text;
            int frameSizeWidth = int.Parse(textBoxVideoFrameSizeWidth.Text);
            int frameSizeHeigth = int.Parse(textBoxVideoFrameSizeHeight.Text);
            if (!_audioSampleSize.ContainsKey(frameSizeName))
            {
                // Add the new item.
                _videoFrameSize.Add(frameSizeName, new Size(frameSizeWidth, frameSizeHeigth));
                comboBoxVideoFrameSize.Items.Add(frameSizeName);
            }
        }
    }
}
