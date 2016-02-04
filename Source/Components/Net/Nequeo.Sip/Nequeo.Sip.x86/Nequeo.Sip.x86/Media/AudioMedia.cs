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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Sip
{
    /// <summary>
    /// Audio media.
    /// </summary>
    public class AudioMedia : MediaBase
    {
        /// <summary>
        /// Audio media.
        /// </summary>
        /// <param name="pjAudioMedia">The pj audio media.</param>
        internal AudioMedia(pjsua2.AudioMedia pjAudioMedia) : base(MediaType.PJMEDIA_TYPE_AUDIO)
        {
            _pjAudioMedia = pjAudioMedia;
        }

        private int _id;
        private pjsua2.AudioMedia _pjAudioMedia = null;

        /// <summary>
        /// Gets or sets the pj audio media.
        /// </summary>
        internal pjsua2.AudioMedia PjAudioMedia
        {
            get { return _pjAudioMedia; }
            set { _pjAudioMedia = value; }
        }

        /// <summary>
        /// Gets or sets the conference port Id.
        /// </summary>
        protected int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Get information about the specified conference port.
        /// </summary>
        /// <returns>The conference port.</returns>
        public ConfPortInfo GetPortInfo()
        {
            // Get the config
            pjsua2.ConfPortInfo info = _pjAudioMedia.getPortInfo();

            ConfPortInfo confPortInfo = new ConfPortInfo();
            confPortInfo.Format = new MediaFormatAudio();
            confPortInfo.Format.AvgBps = info.format.avgBps;
            confPortInfo.Format.BitsPerSample = info.format.bitsPerSample;
            confPortInfo.Format.ChannelCount = info.format.channelCount;
            confPortInfo.Format.ClockRate = info.format.clockRate;
            confPortInfo.Format.FrameTimeUsec = info.format.frameTimeUsec;
            confPortInfo.Format.Id = info.format.id;
            confPortInfo.Format.MaxBps = info.format.maxBps;
            confPortInfo.Format.Type = MediaFormat.GetMediaTypeEx(info.format.type);

            confPortInfo.Name = info.name;
            confPortInfo.PortId = info.portId;
            confPortInfo.RxLevelAdj = info.rxLevelAdj;
            confPortInfo.TxLevelAdj = info.txLevelAdj;

            List<int> listeners = new List<int>();

            // For each code found.
            for (int i = 0; i < info.listeners.Count; i++)
            {
                // Get the port number.
                listeners.Add(info.listeners[i]);
            }

            confPortInfo.Listeners = listeners.ToArray();

            // Return the config port info.
            return confPortInfo;
        }

        /// <summary>
        /// Get information about the specified conference port.
        /// </summary>
        /// <param name="portId">The port id.</param>
        /// <returns>The conference port.</returns>
        public static ConfPortInfo GetPortInfoFromId(int portId)
        {
            // Get the config
            pjsua2.ConfPortInfo info = pjsua2.AudioMedia.getPortInfoFromId(portId);

            ConfPortInfo confPortInfo = new ConfPortInfo();
            confPortInfo.Format = new MediaFormatAudio();
            confPortInfo.Format.AvgBps = info.format.avgBps;
            confPortInfo.Format.BitsPerSample = info.format.bitsPerSample;
            confPortInfo.Format.ChannelCount = info.format.channelCount;
            confPortInfo.Format.ClockRate = info.format.clockRate;
            confPortInfo.Format.FrameTimeUsec = info.format.frameTimeUsec;
            confPortInfo.Format.Id = info.format.id;
            confPortInfo.Format.MaxBps = info.format.maxBps;
            confPortInfo.Format.Type = MediaFormat.GetMediaTypeEx(info.format.type);

            confPortInfo.Name = info.name;
            confPortInfo.PortId = info.portId;
            confPortInfo.RxLevelAdj = info.rxLevelAdj;
            confPortInfo.TxLevelAdj = info.txLevelAdj;

            List<int> listeners = new List<int>();

            // For each code found.
            for (int i = 0; i < info.listeners.Count; i++)
            {
                // Get the port number.
                listeners.Add(info.listeners[i]);
            }

            confPortInfo.Listeners = listeners.ToArray();

            // Return the config port info.
            return confPortInfo;
        }

        /// <summary>
        /// Get port id.
        /// </summary>
        /// <returns>The port id.</returns>
        public int GetPortId()
        {
            return _pjAudioMedia.getPortId();
        }

        /// <summary>
        /// Establish unidirectional media flow to sink. This media port
        /// will act as a source, and it may transmit to multiple destinations / sink.
        /// And if multiple sources are transmitting to the same sink, the media
        /// will be mixed together.Source and sink may refer to the same Media,
        /// effectively looping the media.
        ///
        /// If bidirectional media flow is desired, application needs to call
        /// this method twice, with the second one called from the opposite source
        /// media.
        /// </summary>
        /// <param name="sink">The destination media.</param>
        public void StartTransmit(AudioMedia sink)
        {
            pjsua2.AudioMedia media = sink.PjAudioMedia;
            _pjAudioMedia.startTransmit(media);
        }

        /// <summary>
        /// Stop media flow to destination/sink port.
        /// </summary>
        /// <param name="sink">The destination media.</param>
        public void StopTransmit(AudioMedia sink)
        {
            pjsua2.AudioMedia media = sink.PjAudioMedia;
            _pjAudioMedia.stopTransmit(media);
        }

        /// <summary>
        /// Adjust the signal level to be transmitted from the bridge to this
        /// media port by making it louder or quieter.
        /// </summary>
        /// <param name="level">Signal level adjustment. Value 1.0 means no level 
        /// adjustment, while value 0 means to mute the port.</param>
        public void AdjustRxLevel(float level)
        {
            _pjAudioMedia.adjustRxLevel(level);
        }

        /// <summary>
        /// Adjust the signal level to be received from this media port (to
        /// the bridge) by making it louder or quieter.
        /// </summary>
        /// <param name="level">Signal level adjustment. Value 1.0 means no level 
        /// adjustment, while value 0 means to mute the port.</param>
        public void AdjustTxLevel(float level)
        {
            _pjAudioMedia.adjustTxLevel(level);
        }

        /// <summary>
        /// Get the last received signal level.
        /// </summary>
        /// <returns>Signal level in percent.</returns>
        public uint GetRxLevel()
        {
            return _pjAudioMedia.getRxLevel();
        }

        /// <summary>
        /// Get the last transmitted signal level.
        /// </summary>
        /// <returns>Signal level in percent.</returns>
        public uint GetTxLevel()
        {
            return _pjAudioMedia.getTxLevel();
        }
    }
}
