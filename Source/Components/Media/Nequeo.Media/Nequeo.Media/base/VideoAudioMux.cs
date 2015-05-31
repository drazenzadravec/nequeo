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
using System.IO;
using System.Threading.Tasks;

using Nequeo.IO.Compression;

namespace Nequeo.Media
{
    /// <summary>
    /// Video and audio mux encoder decoder marker.
    /// </summary>
    public interface IVideoAudioMux { }

    /// <summary>
    /// Video audio encoder decoder.
    /// </summary>
    public class VideoAudioMux : IVideoAudioMux
    {
        /// <summary>
        /// Video audio encoder decoder.
        /// </summary>
        /// <param name="binaryReader">The stream where video and audio data is read from.</param>
        public VideoAudioMux(BinaryReader binaryReader)
        {
            _binaryReader = binaryReader;
        }

        /// <summary>
        /// Video audio encoder decoder.
        /// </summary>
        /// <param name="binaryWriter">The stream where video and audio data is written to.</param>
        public VideoAudioMux(BinaryWriter binaryWriter)
        {
            _binaryWriter = binaryWriter;
        }

        private BinaryReader _binaryReader = null;
        private BinaryWriter _binaryWriter = null;

        private double _videoDuration = 0.0;
        private double _audioDuration = 0.0;

        private double _videoFrameRate = 0.0;

        private long _totalVideoFrames = 0;
        private long _totalAudioFrames = 0;

        /// <summary>
        /// Gets the video duration (seconds).
        /// </summary>
        public double VideoDuration
        {
            get { return _videoDuration; }
        }

        /// <summary>
        /// Gets the audio duration (seconds).
        /// </summary>
        public double AudioDuration
        {
            get { return _audioDuration; }
        }

        /// <summary>
        /// Gets the total number of video frames written.
        /// </summary>
        public long TotalVideoFrames
        {
            get { return _totalVideoFrames; }
        }

        /// <summary>
        /// Gets the total number of audio frames written.
        /// </summary>
        public long TotalAudioFrames
        {
            get { return _totalAudioFrames; }
        }

        /// <summary>
        /// Write the video and audio headers.
        /// </summary>
        /// <param name="header">The video and audo header information.</param>
        public void WriteHeader(VideoAudioHeader header)
        {
            VideoHeader? video = header.Video;
            AudioHeader? audio = header.Audio;

            try
            {
                // Create a new binary reader from the stream
                // set the starting position at the begining
                _binaryWriter.BaseStream.Seek(0, SeekOrigin.Begin);

                // Write the media format.
                if (header.MediaFormat > 0)
                    _binaryWriter.Write(header.MediaFormat);
                else
                    _binaryWriter.Write(Nequeo.Media.Streaming.MediaFormat);

                // If there is video data.
                if (video != null && video.Value.ContainsVideo)
                {
                    // Write each pice of binary data to the stream.
                    _binaryWriter.Write(video.Value.ContainsVideo);
                    _binaryWriter.Write(video.Value.FrameRate);
                    _binaryWriter.Write(video.Value.FrameSizeWidth);
                    _binaryWriter.Write(video.Value.FrameSizeHeight);
                    _binaryWriter.Write(Helper.GetImageTypeInt32(video.Value.ImageType));
                    _binaryWriter.Write(CompressionAlgorithmHelper.GetAlgorithmInt32(video.Value.CompressionAlgorithm));
                    _binaryWriter.Write(video.Value.Duration);

                    // Get the frame rate.
                    _videoFrameRate = video.Value.FrameRate;
                    _videoDuration = video.Value.Duration;
                }
                else
                {
                    // Let the stream know there is no video data.
                    _binaryWriter.Write(false);
                }

                // If there is audio data.
                if (audio != null && audio.Value.ContainsAudio)
                {
                    // Write each pice of binary data to the stream.
                    _binaryWriter.Write(audio.Value.ContainsAudio);
                    _binaryWriter.Write(audio.Value.Channels);
                    _binaryWriter.Write(audio.Value.SamplingRate);
                    _binaryWriter.Write(audio.Value.SampleSize);
                    _binaryWriter.Write(Helper.GetSoundTypeInt32(audio.Value.SoundType));
                    _binaryWriter.Write(CompressionAlgorithmHelper.GetAlgorithmInt32(audio.Value.CompressionAlgorithm));
                    _binaryWriter.Write(audio.Value.Duration);

                    // Get the frame rate.
                    _audioDuration = audio.Value.Duration;
                }
                else
                {
                    // Let the stream know there is no audio data.
                    _binaryWriter.Write(false);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Read the video and audio headers.
        /// </summary>
        /// <returns>The video and audo header information.</returns>
        public VideoAudioHeader ReadHeader()
        {
            VideoHeader videoHeader = new VideoHeader();
            AudioHeader audioHeader = new AudioHeader();
            VideoAudioHeader header = new VideoAudioHeader();

            try
            {
                // Create a new binary reader from the stream
                // set the starting position at the begining
                _binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);

                // Read the media format.
                header.MediaFormat = _binaryReader.ReadInt32();

                // Is there video data.
                videoHeader.ContainsVideo = _binaryReader.ReadBoolean();

                // If there is video data.
                if (videoHeader.ContainsVideo)
                {
                    // Read each pice of binary data.
                    videoHeader.FrameRate = _binaryReader.ReadDouble();
                    videoHeader.FrameSizeWidth = _binaryReader.ReadInt32();
                    videoHeader.FrameSizeHeight = _binaryReader.ReadInt32();
                    videoHeader.ImageType = Helper.GetImageType(_binaryReader.ReadInt32());
                    videoHeader.CompressionAlgorithm = CompressionAlgorithmHelper.GetAlgorithm(_binaryReader.ReadInt32());
                    videoHeader.Duration = _binaryReader.ReadDouble();

                    // Get the frame rate.
                    _videoFrameRate = videoHeader.FrameRate;
                    _videoDuration = videoHeader.Duration;
                }

                // Assign the video header.
                header.Video = videoHeader;

                // Is there audio data.
                audioHeader.ContainsAudio = _binaryReader.ReadBoolean();

                // If there is audio data.
                if (audioHeader.ContainsAudio)
                {
                    // Read the audio data.
                    audioHeader.Channels = _binaryReader.ReadInt16();
                    audioHeader.SamplingRate = _binaryReader.ReadInt32();
                    audioHeader.SampleSize = _binaryReader.ReadInt16();
                    audioHeader.SoundType = Helper.GetSoundType(_binaryReader.ReadInt32());
                    audioHeader.CompressionAlgorithm = CompressionAlgorithmHelper.GetAlgorithm(_binaryReader.ReadInt32());
                    audioHeader.Duration = _binaryReader.ReadDouble();

                    // Get the frame rate.
                    _audioDuration = audioHeader.Duration;
                }

                // Assign the audio header.
                header.Audio = audioHeader;
            }
            catch (Exception)
            {
                throw;
            }

            // Return the video and audio header.
            return header;
        }

        /// <summary>
        /// Write a block of video and audio data.
        /// </summary>
        /// <param name="block">The block of video and audio data to write.</param>
        public void Write(VideoAudio block)
        {
            try
            {
                // Write video data.
                if (block.Video != null)
                {
                    // Write video data.
                    _binaryWriter.Write((ushort)0);
                    _binaryWriter.Write(block.Video.Length);

                    // Get the images in the video.
                    for (int v = 0; v < block.Video.Length; v++)
                    {
                        _binaryWriter.Write(block.Video[v].Data.Length);
                        _binaryWriter.Write(block.Video[v].Data);
                    }

                    // Calculate the video frame count and duration.
                    _totalVideoFrames += (long)block.Video.Length;
                    _videoDuration = (_videoFrameRate > 0.0 ? (_totalVideoFrames / _videoFrameRate) : 0.0);
                }

                // Write audio data.
                if (block.Audio != null)
                {
                    // Write audio data.
                    _binaryWriter.Write((ushort)1);
                    _binaryWriter.Write(block.Audio.Length);

                    // Get the sounds in the audio.
                    for (int s = 0; s < block.Audio.Length; s++)
                    {
                        _binaryWriter.Write(block.Audio[s].StartAtFrameIndex);
                        _binaryWriter.Write(block.Audio[s].Data.Length);
                        _binaryWriter.Write(block.Audio[s].Data);
                    }

                    // Calculate the audio frame count and duration.
                    _totalAudioFrames += (long)block.Audio.Length;
                    _audioDuration = (double)_totalAudioFrames;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Read a block of video and audio data.
        /// </summary>
        /// <returns>The block of video and audio data read.</returns>
        public VideoAudio Read()
        {
            ImageModel[] video = null;
            SoundModel[] audio = null;
            VideoAudio block = new VideoAudio();

            try
            {
                // If data exists.
                if (_binaryReader.PeekChar() > -1)
                {
                    // Read the block type video or audio.
                    ushort mediaType = _binaryReader.ReadUInt16();
                    int count = _binaryReader.ReadInt32();

                    // Read the block of data.
                    switch (mediaType)
                    {
                        case 0:
                            // Video block.
                            video = new ImageModel[count];

                            // Get the images in the video.
                            for (int v = 0; v < count; v++)
                            {
                                video[v].Size = _binaryReader.ReadInt32();
                                video[v].Data = _binaryReader.ReadBytes(video[v].Size);
                            }

                            // Calculate the video frame count and duration.
                            _totalVideoFrames += (long)count;
                            break;

                        case 1:
                            // Audio block.
                            audio = new SoundModel[count];

                            // Get the sounds in the audio.
                            for (int s = 0; s < count; s++)
                            {
                                audio[s].StartAtFrameIndex = _binaryReader.ReadInt32();
                                audio[s].Size = _binaryReader.ReadInt32();
                                audio[s].Data = _binaryReader.ReadBytes(audio[s].Size);
                            }

                            // Calculate the audio frame count and duration.
                            _totalAudioFrames += (long)count;
                            break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            // Assign the video and audio data.
            block.Video = video;
            block.Audio = audio;

            // Return the video audio block.
            return block;
        }
    }
}
