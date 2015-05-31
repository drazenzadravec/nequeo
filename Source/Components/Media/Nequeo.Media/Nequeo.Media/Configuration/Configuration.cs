/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Data;
using System.Configuration;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Reflection;

namespace Nequeo.Media.Configuration
{
    #region Media Configuration
    /// <summary>
    /// Class that contains the default host data
    /// within the configuration file.
    /// </summary>
    public class MediaSection : ConfigurationSection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MediaSection()
        {
        }

        /// <summary>
        /// Gets sets, the video section attributes.
        /// </summary>
        [ConfigurationProperty("Video")]
        public VideoElement VideoSection
        {
            get { return (VideoElement)this["Video"]; }
            set { this["Video"] = value; }
        }

        /// <summary>
        /// Gets sets, the audio section attributes.
        /// </summary>
        [ConfigurationProperty("Audio")]
        public AudioElement AudioSection
        {
            get { return (AudioElement)this["Audio"]; }
            set { this["Audio"] = value; }
        }

        /// <summary>
        /// Reads XML from the configuration file.
        /// </summary>
        /// <param name="reader">The System.Xml.XmlReader object, 
        /// which reads from the configuration file.</param>
        protected override void DeserializeSection(XmlReader reader)
        {
            base.DeserializeSection(reader);
        }

        /// <summary>
        /// Creates an XML string containing an unmerged view of the 
        /// System.Configuration.ConfigurationSection
        ///  object as a single section to write to a file.
        /// </summary>
        /// <param name="parentElement">The System.Configuration.ConfigurationElement 
        /// instance to use as the parent when performing the un-merge.</param>
        /// <param name="name">The name of the section to create.</param>
        /// <param name="saveMode">The System.Configuration.ConfigurationSaveMode 
        /// instance to use when writing to a string.</param>
        /// <returns>An XML string containing an unmerged view of the 
        /// System.Configuration.ConfigurationSection object.</returns>
        protected override string SerializeSection(
            ConfigurationElement parentElement,
            string name, ConfigurationSaveMode saveMode)
        {
            return base.SerializeSection(parentElement, name, saveMode);
        }
    }

    /// <summary>
    /// Class that contains all the video attributes.
    /// </summary>
    public class VideoElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public VideoElement()
        {
        }

        /// <summary>
        /// Constructor with video attributes
        /// </summary>
        /// <param name="useDefault">The use default attribute.</param>
        /// <param name="frameRate">The frame rate attribute.</param>
        /// <param name="frameSizeWidth">The frame size width attribute.</param>
        /// <param name="frameSizeHeight">The frame size height attribute.</param>
        public VideoElement(bool useDefault, double frameRate, int frameSizeWidth, int frameSizeHeight)
        {
            UseDefault = useDefault;
            FrameRate = frameRate;
            FrameSizeWidth = frameSizeWidth;
            FrameSizeHeight = frameSizeHeight;
        }

        /// <summary>
        /// Gets sets, the useDefault attribute.
        /// </summary>
        [ConfigurationProperty("useDefault", DefaultValue = true, IsRequired = true)]
        public bool UseDefault
        {
            get { return (bool)this["useDefault"]; }
            set { this["useDefault"] = value; }
        }

        /// <summary>
        /// Gets sets, the frameRate attribute.
        /// </summary>
        [ConfigurationProperty("frameRate", DefaultValue = 30.0, IsRequired = true)]
        public double FrameRate
        {
            get { return (double)this["frameRate"]; }
            set { this["frameRate"] = value; }
        }

        /// <summary>
        /// Gets sets, the frameSizeWidth attribute.
        /// </summary>
        [ConfigurationProperty("frameSizeWidth", DefaultValue = 640, IsRequired = true)]
        public int FrameSizeWidth
        {
            get { return (int)this["frameSizeWidth"]; }
            set { this["frameSizeWidth"] = value; }
        }

        /// <summary>
        /// Gets sets, the frameSizeHeight attribute.
        /// </summary>
        [ConfigurationProperty("frameSizeHeight", DefaultValue = 480, IsRequired = true)]
        public int FrameSizeHeight
        {
            get { return (int)this["frameSizeHeight"]; }
            set { this["frameSizeHeight"] = value; }
        }
    }

    /// <summary>
    /// Class that contains all the audio attributes.
    /// </summary>
    public class AudioElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public AudioElement()
        {
        }

        /// <summary>
        /// Constructor with audio attributes
        /// </summary>
        /// <param name="useDefault">The use default attribute.</param>
        /// <param name="channels">The channels attribute.</param>
        /// <param name="samplingRate">The sampling rate attribute.</param>
        /// <param name="sampleSize">The sample size attribute.</param>
        public AudioElement(bool useDefault, short channels, int samplingRate, short sampleSize)
        {
            UseDefault = useDefault;
            Channels = channels;
            SamplingRate = samplingRate;
            SampleSize = sampleSize;
        }

        /// <summary>
        /// Gets sets, the useDefault attribute.
        /// </summary>
        [ConfigurationProperty("useDefault", DefaultValue = true, IsRequired = true)]
        public bool UseDefault
        {
            get { return (bool)this["useDefault"]; }
            set { this["useDefault"] = value; }
        }

        /// <summary>
        /// Gets sets, the channels attribute.
        /// </summary>
        [ConfigurationProperty("channels", DefaultValue = 2, IsRequired = true)]
        public short Channels
        {
            get { return (short)this["channels"]; }
            set { this["channels"] = value; }
        }

        /// <summary>
        /// Gets sets, the samplingRate attribute.
        /// </summary>
        [ConfigurationProperty("samplingRate", DefaultValue = 44100, IsRequired = true)]
        public int SamplingRate
        {
            get { return (int)this["samplingRate"]; }
            set { this["samplingRate"] = value; }
        }

        /// <summary>
        /// Gets sets, the sampleSize attribute.
        /// </summary>
        [ConfigurationProperty("sampleSize", DefaultValue = 16, IsRequired = true)]
        public short SampleSize
        {
            get { return (short)this["sampleSize"]; }
            set { this["sampleSize"] = value; }
        }
    }
    #endregion

    /// <summary>
    /// Configuration reader
    /// </summary>
    public class Reader
    {
        /// <summary>
        /// Get the audio details.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The audio details.</returns>
        public AudioDetails GetAudio(string section = "NequeoMediaGroup/MediaSection")
        {
            AudioDetails audioDetails = new AudioDetails();

            try
            {
                // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Create a new default host type
                // an load the values from the configuration
                // file into the default host type.
                MediaSection mediaSection =
                    (MediaSection)System.Configuration.ConfigurationManager.GetSection(section);

                // Make sure the section is defined.
                if (mediaSection == null)
                    throw new Exception("Configuration section has not been defined.");

                // Get the audio element.
                AudioElement audioElement = mediaSection.AudioSection;
                if (audioElement == null)
                    throw new Exception("Configuration element Audio has not been defined.");

                // Assign the audio.
                audioDetails.Channels = audioElement.Channels;
                audioDetails.SampleSize = audioElement.SampleSize;
                audioDetails.SamplingRate = audioElement.SamplingRate;
                audioDetails.UseDefault = audioElement.UseDefault;
            }
            catch (Exception)
            {
                throw;
            }

            // Return the details.
            return audioDetails;
        }

        /// <summary>
        /// Get the video details.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The video details.</returns>
        public VideoDetails GetVideo(string section = "NequeoMediaGroup/MediaSection")
        {
            VideoDetails videoDetails = new VideoDetails();

            try
            {
                // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Create a new default host type
                // an load the values from the configuration
                // file into the default host type.
                MediaSection mediaSection =
                    (MediaSection)System.Configuration.ConfigurationManager.GetSection(section);

                // Make sure the section is defined.
                if (mediaSection == null)
                    throw new Exception("Configuration section has not been defined.");

                // Get the video element.
                VideoElement videoElement = mediaSection.VideoSection;
                if (videoElement == null)
                    throw new Exception("Configuration element Video has not been defined.");

                // Assign the audio.
                videoDetails.FrameRate = videoElement.FrameRate;
                videoDetails.FrameSizeHeight = videoElement.FrameSizeHeight;
                videoDetails.FrameSizeWidth = videoElement.FrameSizeWidth;
                videoDetails.UseDefault = videoElement.UseDefault;
            }
            catch (Exception)
            {
                throw;
            }

            // Return the details.
            return videoDetails;
        }
    }
}
