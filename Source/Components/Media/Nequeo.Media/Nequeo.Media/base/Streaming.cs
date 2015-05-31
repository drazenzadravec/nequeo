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
    /// Media streaming encoder and decoder.
    /// </summary>
    public class Streaming
    {
        /// <summary>
        /// Media streaming encoder and decoder.
        /// </summary>
        public Streaming()
        {
        }

        /// <summary>
        /// The global Nequeo.Media format.
        /// </summary>
        public static readonly int MediaFormat = 26071970;

        /// <summary>
        /// Encode the media data.
        /// </summary>
        /// <param name="videoAudio">The video and audio content.</param>
        /// <returns>The encoded media data.</returns>
        public byte[] Encode(VideoAudioModel videoAudio)
        {
            byte[] data = null;

            VideoModel? video = videoAudio.Video;
            AudioModel? audio = videoAudio.Audio;

            MemoryStream memoryStream = null;
            BinaryWriter binaryWriter = null;

            try
            {
                // Create the encoding stream.
                memoryStream = new MemoryStream();

                // Create a new binary reader from the stream
                // set the starting position at the begining
                binaryWriter = new BinaryWriter(memoryStream);
                binaryWriter.BaseStream.Seek(0, SeekOrigin.Begin);

                // Write the media format.
                if (videoAudio.MediaFormat > 0)
                    binaryWriter.Write(videoAudio.MediaFormat);
                else
                    binaryWriter.Write(Nequeo.Media.Streaming.MediaFormat);

                // If there is video data.
                if (video != null && video.Value.Header.ContainsVideo)
                {
                    // Write each pice of binary data to the stream.
                    binaryWriter.Write(video.Value.Header.ContainsVideo);
                    binaryWriter.Write(video.Value.Header.FrameRate);
                    binaryWriter.Write(video.Value.Header.FrameSizeWidth);
                    binaryWriter.Write(video.Value.Header.FrameSizeHeight);
                    binaryWriter.Write(Helper.GetImageTypeInt32(video.Value.Header.ImageType));
                    binaryWriter.Write(CompressionAlgorithmHelper.GetAlgorithmInt32(video.Value.Header.CompressionAlgorithm));
                    binaryWriter.Write(video.Value.Header.Duration);
                    binaryWriter.Write((ushort)0);
                    binaryWriter.Write(video.Value.Video.Length);

                    // Get the images in the video.
                    for (int v = 0; v < video.Value.Video.Length; v++)
                    {
                        binaryWriter.Write(video.Value.Video[v].Data.Length);
                        binaryWriter.Write(video.Value.Video[v].Data);
                    }
                }
                else
                {
                    // Let the stream know there is no video data.
                    binaryWriter.Write(false);
                }

                // If there is audio data.
                if (audio != null && audio.Value.Header.ContainsAudio)
                {
                    // Write the audio data.
                    binaryWriter.Write(audio.Value.Header.ContainsAudio);
                    binaryWriter.Write(audio.Value.Header.Channels);
                    binaryWriter.Write(audio.Value.Header.SamplingRate);
                    binaryWriter.Write(audio.Value.Header.SampleSize);
                    binaryWriter.Write(Helper.GetSoundTypeInt32(audio.Value.Header.SoundType));
                    binaryWriter.Write(CompressionAlgorithmHelper.GetAlgorithmInt32(audio.Value.Header.CompressionAlgorithm));
                    binaryWriter.Write(audio.Value.Header.Duration);
                    binaryWriter.Write((ushort)1);
                    binaryWriter.Write(audio.Value.Audio.Length);

                    // Get the sounds in the audio.
                    for (int s = 0; s < audio.Value.Audio.Length; s++)
                    {
                        binaryWriter.Write(audio.Value.Audio[s].StartAtFrameIndex);
                        binaryWriter.Write(audio.Value.Audio[s].Data.Length);
                        binaryWriter.Write(audio.Value.Audio[s].Data);
                    }
                }
                else
                {
                    // Let the stream know there is no audio data.
                    binaryWriter.Write(false);
                }

                // Assign the data.
                data = memoryStream.ToArray();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (binaryWriter != null)
                    binaryWriter.Close();

                if (memoryStream != null)
                    memoryStream.Close();
            }

            // Return the data.
            return data;
        }

        /// <summary>
        /// Encode the raw media data.
        /// </summary>
        /// <param name="videoAudio">The video and audio content.</param>
        /// <returns>The encoded media data.</returns>
        public byte[] EncodeRaw(VideoAudio[] videoAudio)
        {
            byte[] data = null;

            MemoryStream memoryStream = null;
            BinaryWriter binaryWriter = null;

            try
            {
                // Create the encoding stream.
                memoryStream = new MemoryStream();

                // Create a new binary reader from the stream
                // set the starting position at the begining
                binaryWriter = new BinaryWriter(memoryStream);
                binaryWriter.BaseStream.Seek(0, SeekOrigin.Begin);

                // For each video and audio collection.
                for (int z = 0; z < videoAudio.Length; z++)
                {
                    // Get the current video and audo collection.
                    ImageModel[] video = videoAudio[z].Video;
                    SoundModel[] audio = videoAudio[z].Audio;

                    // If there is video data.
                    if (video != null && video.Length > 0)
                    {
                        // Write each pice of binary data to the stream.
                        binaryWriter.Write((ushort)0);
                        binaryWriter.Write(video.Length);

                        // Get the images in the video.
                        for (int v = 0; v < video.Length; v++)
                        {
                            binaryWriter.Write(video[v].Data.Length);
                            binaryWriter.Write(video[v].Data);
                        }
                    }

                    // If there is audio data.
                    if (audio != null && audio.Length > 0)
                    {
                        // Write the audio data.
                        binaryWriter.Write((ushort)1);
                        binaryWriter.Write(audio.Length);

                        // Get the sounds in the audio.
                        for (int s = 0; s < audio.Length; s++)
                        {
                            binaryWriter.Write(audio[s].StartAtFrameIndex);
                            binaryWriter.Write(audio[s].Data.Length);
                            binaryWriter.Write(audio[s].Data);
                        }
                    }
                }

                // Assign the data.
                data = memoryStream.ToArray();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (binaryWriter != null)
                    binaryWriter.Close();

                if (memoryStream != null)
                    memoryStream.Close();
            }

            // Return the data.
            return data;
        }

        /// <summary>
        /// Encode the header media data.
        /// </summary>
        /// <param name="videoAudio">The video and audio header content.</param>
        /// <returns>The encoded media data.</returns>
        public byte[] EncodeHeader(VideoAudioHeader videoAudio)
        {
            byte[] data = null;

            VideoHeader? video = videoAudio.Video;
            AudioHeader? audio = videoAudio.Audio;

            MemoryStream memoryStream = null;
            BinaryWriter binaryWriter = null;

            try
            {
                // Create the encoding stream.
                memoryStream = new MemoryStream();

                // Create a new binary reader from the stream
                // set the starting position at the begining
                binaryWriter = new BinaryWriter(memoryStream);
                binaryWriter.BaseStream.Seek(0, SeekOrigin.Begin);

                // Write the media format.
                if (videoAudio.MediaFormat > 0)
                    binaryWriter.Write(videoAudio.MediaFormat);
                else
                    binaryWriter.Write(Nequeo.Media.Streaming.MediaFormat);

                // If there is video data.
                if (video != null && video.Value.ContainsVideo)
                {
                    // Write each pice of binary data to the stream.
                    binaryWriter.Write(video.Value.ContainsVideo);
                    binaryWriter.Write(video.Value.FrameRate);
                    binaryWriter.Write(video.Value.FrameSizeWidth);
                    binaryWriter.Write(video.Value.FrameSizeHeight);
                    binaryWriter.Write(Helper.GetImageTypeInt32(video.Value.ImageType));
                    binaryWriter.Write(CompressionAlgorithmHelper.GetAlgorithmInt32(video.Value.CompressionAlgorithm));
                    binaryWriter.Write(video.Value.Duration);
                }
                else
                {
                    // Let the stream know there is no video data.
                    binaryWriter.Write(false);
                }

                // If there is audio data.
                if (audio != null && audio.Value.ContainsAudio)
                {
                    // Write the audio data.
                    binaryWriter.Write(audio.Value.ContainsAudio);
                    binaryWriter.Write(audio.Value.Channels);
                    binaryWriter.Write(audio.Value.SamplingRate);
                    binaryWriter.Write(audio.Value.SampleSize);
                    binaryWriter.Write(Helper.GetSoundTypeInt32(audio.Value.SoundType));
                    binaryWriter.Write(CompressionAlgorithmHelper.GetAlgorithmInt32(audio.Value.CompressionAlgorithm));
                    binaryWriter.Write(audio.Value.Duration);
                }
                else
                {
                    // Let the stream know there is no audio data.
                    binaryWriter.Write(false);
                }

                // Assign the data.
                data = memoryStream.ToArray();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (binaryWriter != null)
                    binaryWriter.Close();

                if (memoryStream != null)
                    memoryStream.Close();
            }

            // Return the data.
            return data;
        }

        /// <summary>
        /// Decode the media data.
        /// </summary>
        /// <param name="encodedData">The encoded media data.</param>
        /// <returns>The decoded media data.</returns>
        public VideoAudioModel Decode(byte[] encodedData)
        {
            VideoModel video = new VideoModel();
            AudioModel audio = new AudioModel();
            VideoHeader videoHeader = new VideoHeader();
            AudioHeader audioHeader = new AudioHeader();
            VideoAudioModel videoAudio = new VideoAudioModel();

            MemoryStream memoryStream = null;
            BinaryReader binaryReader = null;

            try
            {
                // Load the encoded data into the memory stream.
                memoryStream = new MemoryStream(encodedData);

                // Create a new binary reader from the stream
                // set the starting position at the begining
                binaryReader = new BinaryReader(memoryStream);
                binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);

                // Read the media format.
                videoAudio.MediaFormat = binaryReader.ReadInt32();

                // Is there video data.
                videoHeader.ContainsVideo = binaryReader.ReadBoolean();

                // If there is video data.
                if (videoHeader.ContainsVideo)
                {
                    // Read each pice of binary data.
                    videoHeader.FrameRate = binaryReader.ReadDouble();
                    videoHeader.FrameSizeWidth = binaryReader.ReadInt32();
                    videoHeader.FrameSizeHeight = binaryReader.ReadInt32();
                    videoHeader.ImageType = Helper.GetImageType(binaryReader.ReadInt32());
                    videoHeader.CompressionAlgorithm = CompressionAlgorithmHelper.GetAlgorithm(binaryReader.ReadInt32());
                    videoHeader.Duration = binaryReader.ReadDouble();
                    ushort mediaVideoType = binaryReader.ReadUInt16();
                    video.ImageCount = binaryReader.ReadInt32();

                    // Create the image collection.
                    ImageModel[] images = new ImageModel[video.ImageCount];

                    // Get the images in the video.
                    for (int v = 0; v < video.ImageCount; v++)
                    {
                        // Create the image.
                        ImageModel image = new ImageModel();
                        image.Size = binaryReader.ReadInt32();
                        image.Data = binaryReader.ReadBytes(image.Size);

                        // Assign this image.
                        images[v] = image;
                    }

                    // Assign the image collection.
                    video.Video = images;
                }

                // Assign the video header.
                video.Header = videoHeader;

                // Is there audio data.
                audioHeader.ContainsAudio = binaryReader.ReadBoolean();

                // If there is audio data.
                if (audioHeader.ContainsAudio)
                {
                    // Read the audio data.
                    audioHeader.Channels = binaryReader.ReadInt16();
                    audioHeader.SamplingRate = binaryReader.ReadInt32();
                    audioHeader.SampleSize = binaryReader.ReadInt16();
                    audioHeader.SoundType = Helper.GetSoundType(binaryReader.ReadInt32());
                    audioHeader.CompressionAlgorithm = CompressionAlgorithmHelper.GetAlgorithm(binaryReader.ReadInt32());
                    audioHeader.Duration = binaryReader.ReadDouble();
                    ushort mediaAudioType = binaryReader.ReadUInt16();
                    audio.SoundCount = binaryReader.ReadInt32();

                    // Create the sound collection.
                    SoundModel[] sounds = new SoundModel[audio.SoundCount];

                    // Get the sounds in the audio.
                    for (int s = 0; s < audio.SoundCount; s++)
                    {
                        // Create the sound.
                        SoundModel sound = new SoundModel();
                        sound.StartAtFrameIndex = binaryReader.ReadInt32();
                        sound.Size = binaryReader.ReadInt32();
                        sound.Data = binaryReader.ReadBytes(sound.Size);

                        // Assign this sound.
                        sounds[s] = sound;
                    }

                    // Assign the sound collection.
                    audio.Audio = sounds;
                }

                // Assign the audio header.
                audio.Header = audioHeader;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (binaryReader != null)
                    binaryReader.Close();

                if (memoryStream != null)
                    memoryStream.Close();
            }

            // Assign the video and audio data.
            videoAudio.Video = video;
            videoAudio.Audio = audio;

            // Return the video and audio data.
            return videoAudio;
        }

        /// <summary>
        /// Decode the raw media data.
        /// </summary>
        /// <param name="encodedData">The encoded media data.</param>
        /// <returns>The decoded media data.</returns>
        public VideoAudio[] DecodeRaw(byte[] encodedData)
        {
            ImageModel[] video = null;
            SoundModel[] audio = null;
            List<VideoAudio> videoAudios = new List<VideoAudio>();

            MemoryStream memoryStream = null;
            BinaryReader binaryReader = null;

            try
            {
                // Load the encoded data into the memory stream.
                memoryStream = new MemoryStream(encodedData);

                // Create a new binary reader from the stream
                // set the starting position at the begining
                binaryReader = new BinaryReader(memoryStream);
                binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);

                // While data exists.
                while (binaryReader.PeekChar() > -1)
                {
                    // Video and audio.
                    VideoAudio videoAudio = new VideoAudio();

                    // Read the block type video or audio.
                    ushort mediaType = binaryReader.ReadUInt16();
                    int count = binaryReader.ReadInt32();

                    // Read the block of data.
                    switch (mediaType)
                    {
                        case 0:
                            // Video block.
                            video = new ImageModel[count];

                            // Get the images in the video.
                            for (int v = 0; v < count; v++)
                            {
                                video[v].Size = binaryReader.ReadInt32();
                                video[v].Data = binaryReader.ReadBytes(video[v].Size);
                            }
                            break;

                        case 1:
                            // Audio block.
                            audio = new SoundModel[count];

                            // Get the sounds in the audio.
                            for (int s = 0; s < count; s++)
                            {
                                audio[s].StartAtFrameIndex = binaryReader.ReadInt32();
                                audio[s].Size = binaryReader.ReadInt32();
                                audio[s].Data = binaryReader.ReadBytes(audio[s].Size);
                            }
                            break;
                    }

                    // Assign the video and audio data.
                    videoAudio.Video = video;
                    videoAudio.Audio = audio;

                    // Add the video and audio.
                    videoAudios.Add(videoAudio);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (binaryReader != null)
                    binaryReader.Close();

                if (memoryStream != null)
                    memoryStream.Close();
            }

            // Return the video and audio data.
            return videoAudios.ToArray();
        }

        /// <summary>
        /// Decode the header media data.
        /// </summary>
        /// <param name="encodedData">The encoded media data.</param>
        /// <returns>The decoded media data.</returns>
        public VideoAudioHeader DecodeHeader(byte[] encodedData)
        {
            VideoHeader videoHeader = new VideoHeader();
            AudioHeader audioHeader = new AudioHeader();
            VideoAudioHeader videoAudio = new VideoAudioHeader();

            MemoryStream memoryStream = null;
            BinaryReader binaryReader = null;

            try
            {
                // Load the encoded data into the memory stream.
                memoryStream = new MemoryStream(encodedData);

                // Create a new binary reader from the stream
                // set the starting position at the begining
                binaryReader = new BinaryReader(memoryStream);
                binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);

                // Read the media format.
                videoAudio.MediaFormat = binaryReader.ReadInt32();

                // Is there video data.
                videoHeader.ContainsVideo = binaryReader.ReadBoolean();

                // If there is video data.
                if (videoHeader.ContainsVideo)
                {
                    // Read each pice of binary data.
                    videoHeader.FrameRate = binaryReader.ReadDouble();
                    videoHeader.FrameSizeWidth = binaryReader.ReadInt32();
                    videoHeader.FrameSizeHeight = binaryReader.ReadInt32();
                    videoHeader.ImageType = Helper.GetImageType(binaryReader.ReadInt32());
                    videoHeader.CompressionAlgorithm = CompressionAlgorithmHelper.GetAlgorithm(binaryReader.ReadInt32());
                    videoHeader.Duration = binaryReader.ReadDouble();
                }

                // Is there audio data.
                audioHeader.ContainsAudio = binaryReader.ReadBoolean();

                // If there is audio data.
                if (audioHeader.ContainsAudio)
                {
                    // Read the audio data.
                    audioHeader.Channels = binaryReader.ReadInt16();
                    audioHeader.SamplingRate = binaryReader.ReadInt32();
                    audioHeader.SampleSize = binaryReader.ReadInt16();
                    audioHeader.SoundType = Helper.GetSoundType(binaryReader.ReadInt32());
                    audioHeader.CompressionAlgorithm = CompressionAlgorithmHelper.GetAlgorithm(binaryReader.ReadInt32());
                    audioHeader.Duration = binaryReader.ReadDouble();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (binaryReader != null)
                    binaryReader.Close();

                if (memoryStream != null)
                    memoryStream.Close();
            }

            // Assign the video and audio data.
            videoAudio.Video = videoHeader;
            videoAudio.Audio = audioHeader;

            // Return the video and audio data.
            return videoAudio;
        }
    }
}
