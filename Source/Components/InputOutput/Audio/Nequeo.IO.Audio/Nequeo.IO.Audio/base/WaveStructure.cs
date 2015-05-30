/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Nequeo.IO.Audio
{
    /// <summary>
    /// Contains the WAVE header structure.
    /// </summary>
	public struct WaveStructure
	{
        /// <summary>
        /// Creates the default values of the wave structure.
        /// </summary>
        /// <param name="channels">The number of channels '1' - mono, '2' - stereo.</param>
        /// <param name="sampleRate">Sample rate typical values are 44100, 11025 etc.</param>
        /// <param name="bitsPerSample">The number of bits per sample; typical values are 8, 16 (default 16)</param>
        /// <param name="soundData">The actual sound data the raw PCM data.</param>
        /// <returns>The wave structure for the current data.</returns>
        public static WaveStructure CreateDefaultStructure(Int16 channels, Int32 sampleRate, Int16 bitsPerSample, byte[] soundData)
        {
            WaveStructure wave = new WaveStructure();

            // Set all the default values.
            wave.ChunckID = 1179011410;
            wave.FileSize = (soundData.Length + 36);
            wave.RiffType = 1163280727;
            wave.FmtID = 544501094;
            wave.FmtSize = 16;
            wave.FmtCode = 1;
            wave.BitsPerSample = bitsPerSample;
            wave.Channels = channels;
            wave.SampleRate = sampleRate;
            wave.FmtAverageByteRate = (Int32)((sampleRate * channels) * (wave.BitsPerSample / 8));
            wave.FmtBlockAlign = (Int16)(channels * (wave.BitsPerSample / 8));
            wave.DataID = 1635017060;
            wave.DataSize = soundData.Length;
            wave.SoundData = soundData;

            // return the wave structure.
            return wave;
        }

        /// <summary>
        /// Creates the default values of the wave structure.
        /// </summary>
        /// <param name="channels">The number of channels '1' - mono, '2' - stereo.</param>
        /// <param name="sampleRate">Sample rate typical values are 44100, 11025 etc.</param>
        /// <param name="soundData">The actual sound data the raw PCM data.</param>
        /// <returns>The wave structure for the current data.</returns>
        public static WaveStructure CreateDefaultStructure(Int16 channels, Int32 sampleRate, byte[] soundData)
        {
            WaveStructure wave = new WaveStructure();

            // Set all the default values.
            wave.ChunckID = 1179011410;
            wave.FileSize = (soundData.Length + 36);
            wave.RiffType = 1163280727;
            wave.FmtID = 544501094;
            wave.FmtSize = 16;
            wave.FmtCode = 1;
            wave.BitsPerSample = 16;
            wave.Channels = channels;
            wave.SampleRate = sampleRate;
            wave.FmtAverageByteRate = (Int32)((sampleRate * channels) * (wave.BitsPerSample / 8));
            wave.FmtBlockAlign = (Int16)(channels * (wave.BitsPerSample / 8));
            wave.DataID = 1635017060;
            wave.DataSize = soundData.Length;
            wave.SoundData = soundData;

            // return the wave structure.
            return wave;
        }

        /// <summary>
        /// Creates the default mono values of the wave structure.
        /// </summary>
        /// <param name="sampleRate">Sample rate typical values are 44100, 11025 etc.</param>
        /// <param name="soundData">The actual sound data the raw PCM data.</param>
        /// <returns>The wave structure for the current data.</returns>
        public static WaveStructure CreateDefaultMonoStructure(Int32 sampleRate, byte[] soundData)
        {
            WaveStructure wave = new WaveStructure();

            // Set all the default values.
            wave.ChunckID = 1179011410;
            wave.FileSize = (soundData.Length + 36);
            wave.RiffType = 1163280727;
            wave.FmtID = 544501094;
            wave.FmtSize = 16;
            wave.FmtCode = 1;
            wave.BitsPerSample = 16;
            wave.Channels = 1;
            wave.SampleRate = sampleRate;
            wave.FmtAverageByteRate = (Int32)((sampleRate * 1) * (wave.BitsPerSample / 8));
            wave.FmtBlockAlign = (Int16)(1 * (wave.BitsPerSample / 8));
            wave.DataID = 1635017060;
            wave.DataSize = soundData.Length;
            wave.SoundData = soundData;

            // return the wave structure.
            return wave;
        }

        /// <summary>
        /// Creates the default values of the wave structure.
        /// </summary>
        /// <param name="sampleRate">Sample rate typical values are 44100, 11025 etc.</param>
        /// <param name="soundData">The actual sound data the raw PCM data.</param>
        /// <returns>The wave structure for the current data.</returns>
        public static WaveStructure CreateDefaultStereoStructure(Int32 sampleRate, byte[] soundData)
        {
            WaveStructure wave = new WaveStructure();

            // Set all the default values.
            wave.ChunckID = 1179011410;
            wave.FileSize = (soundData.Length + 36);
            wave.RiffType = 1163280727;
            wave.FmtID = 544501094;
            wave.FmtSize = 16;
            wave.FmtCode = 1;
            wave.BitsPerSample = 16;
            wave.Channels = 2;
            wave.SampleRate = sampleRate;
            wave.FmtAverageByteRate = (Int32)((sampleRate * 2) * (wave.BitsPerSample / 8));
            wave.FmtBlockAlign = (Int16)(2 * (wave.BitsPerSample / 8));
            wave.DataID = 1635017060;
            wave.DataSize = soundData.Length;
            wave.SoundData = soundData;

            // return the wave structure.
            return wave;
        }

        /// <summary>
        /// Contains the letters 'RIFF' (1179011410) as an int.
        /// </summary>
        public Int32 ChunckID;
        /// <summary>
        /// The file size (chunk size) of the complete file.
        /// </summary>
        public Int32 FileSize;
        /// <summary>
        /// The RIFF type (format) contains the letters 'WAVE' (1163280727) as an int.
        /// </summary>
        public Int32 RiffType;
        /// <summary>
        /// Format id (sub chunk id) contains the letters 'fmt ' (544501094) as an int.
        /// </summary>
        public Int32 FmtID;
        /// <summary>
        /// Format size (sub chunk id size) (default PCM = 16) if this equals 18 then more data should be read at
        /// FmtExtraSize, FmtExtraData.
        /// </summary>
        public Int32 FmtSize;
        /// <summary>
        /// Format code (audio format) PCM/Uncompressed = 1 for WAV audio files.
        /// </summary>
        /// <remarks>
        /// 0 - Unknown,
        /// 1 - PCM/uncompressed,
        /// 2 - Microsoft ADPCM,
        /// 6 - ITU G.711 a-law,
        /// 7 - ITU G.711 Âµ-law,
        /// 17 - IMA ADPCM,
        /// 20 - ITU G.723 ADPCM (Yamaha),
        /// 49 - GSM 6.10,
        /// 64 - ITU G.721 ADPCM,
        /// 80 - MPEG
        /// </remarks>
        public Int16 FmtCode;
        /// <summary>
        /// Number of channels '1' - mono, '2' - stereo.
        /// </summary>
        public Int16 Channels;
        /// <summary>
        /// Sample rate typical values are 44100, 11025 etc.
        /// </summary>
        public Int32 SampleRate;
        /// <summary>
        /// The format average byte rate. Number of bytes per second that are used for all data. This value equals SampleRate * Channels * (BitsPerSample/8)
        /// </summary>
        public Int32 FmtAverageByteRate;
        /// <summary>
        /// Format block align. Also known as BytesPerSample and equals Channels * (BitsPerSample/8).
        /// </summary>
        public Int16 FmtBlockAlign;
        /// <summary>
        /// Bit depth (bits per sample). Typical values are 8, 16 (default 16).
        /// </summary>
        public Int16 BitsPerSample;
        /// <summary>
        /// Extra format size indicator, if this is PCM = 16 for FmtSize then does not exist, ignore.
        /// </summary>
        public Int16 FmtExtraSize;
        /// <summary>
        /// Extra format data; the array size is equal to the FmtExtraSize. Ignore if PCM = 16.
        /// </summary>
        public byte[] FmtExtraData;
        /// <summary>
        /// Contains the letters 'data' (1635017060) as an int.
        /// </summary>
        public Int32 DataID;
        /// <summary>
        /// The size of the actual sound data; without the header information; This equals NumSamples * Channels * (BitsPerSample/8).
        /// </summary>
        public Int32 DataSize;
        /// <summary>
        /// The actual sound data.
        /// </summary>
        public byte[] SoundData;
	}
}
