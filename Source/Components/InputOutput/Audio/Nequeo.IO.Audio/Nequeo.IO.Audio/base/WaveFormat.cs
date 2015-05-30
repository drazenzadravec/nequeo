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
    /// Reads and writes Wave or Wav formatted sound files.
    /// </summary>
    public class WaveFormat : IDisposable
	{
        /// <summary>
        /// Wave sound file formatter.
        /// </summary>
        public WaveFormat()
        {
        }

        private System.Media.SoundPlayer _soundPlayer = null;

        /// <summary>
        /// Read the structure and data of the .wav file.
        /// </summary>
        /// <param name="filePath">The full path and file name of the .wav file.</param>
        /// <returns>The complete structure and sound data of the .wav file.</returns>
        public WaveStructure Read(string filePath)
        {
            if (String.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");

            WaveStructure wave = new WaveStructure();
            FileStream fileStream = null;
            BinaryReader binaryReader = null;

            try
            {
                // Create a new file stream for the file.
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                // Create a new binary reader from the file stream
                // set the starting position at the begining
                binaryReader = new BinaryReader(fileStream);
                binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);

                // Read each pice of binary data from the file.
                wave.ChunckID = binaryReader.ReadInt32();
                wave.FileSize = binaryReader.ReadInt32();
                wave.RiffType = binaryReader.ReadInt32();
                wave.FmtID = binaryReader.ReadInt32();
                wave.FmtSize = binaryReader.ReadInt32();
                wave.FmtCode = binaryReader.ReadInt16();
                wave.Channels = binaryReader.ReadInt16();
                wave.SampleRate = binaryReader.ReadInt32();
                wave.FmtAverageByteRate = binaryReader.ReadInt32();
                wave.FmtBlockAlign = binaryReader.ReadInt16();
                wave.BitsPerSample = binaryReader.ReadInt16();

                // If the format size is not '16' then read additional
                // data from the wave file.
                if (wave.FmtSize != 16)
                {
                    // Read any extra values
                    wave.FmtExtraSize = binaryReader.ReadInt16();
                    wave.FmtExtraData = binaryReader.ReadBytes(wave.FmtExtraSize);
                }

                // Read the 'data' indicator and the actual data size value.
                wave.DataID = binaryReader.ReadInt32();
                wave.DataSize = binaryReader.ReadInt32();

                // From the data size value read the actual sound data.
                wave.SoundData = binaryReader.ReadBytes(wave.DataSize);

                // Close the streams
                binaryReader.Close();
                fileStream.Close();

                // Return the wave structure and data.
                return wave;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (binaryReader != null)
                    binaryReader.Close();

                if (fileStream != null)
                    fileStream.Close();
            }
        }

        /// <summary>
        /// Read the structure and data of the .wav file.
        /// </summary>
        /// <param name="stream">The stream to read the data from.</param>
        /// <returns>The complete structure and sound data of the .wav file.</returns>
        public WaveStructure Read(Stream stream)
        {
            WaveStructure wave = new WaveStructure();
            BinaryReader binaryReader = null;

            try
            {
                // Create a new binary reader from the file stream
                // set the starting position at the begining
                binaryReader = new BinaryReader(stream);
                binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);

                // Read each pice of binary data from the file.
                wave.ChunckID = binaryReader.ReadInt32();
                wave.FileSize = binaryReader.ReadInt32();
                wave.RiffType = binaryReader.ReadInt32();
                wave.FmtID = binaryReader.ReadInt32();
                wave.FmtSize = binaryReader.ReadInt32();
                wave.FmtCode = binaryReader.ReadInt16();
                wave.Channels = binaryReader.ReadInt16();
                wave.SampleRate = binaryReader.ReadInt32();
                wave.FmtAverageByteRate = binaryReader.ReadInt32();
                wave.FmtBlockAlign = binaryReader.ReadInt16();
                wave.BitsPerSample = binaryReader.ReadInt16();

                // If the format size is not '16' then read additional
                // data from the wave file.
                if (wave.FmtSize != 16)
                {
                    // Read any extra values
                    wave.FmtExtraSize = binaryReader.ReadInt16();
                    wave.FmtExtraData = binaryReader.ReadBytes(wave.FmtExtraSize);
                }

                // Read the 'data' indicator and the actual data size value.
                wave.DataID = binaryReader.ReadInt32();
                wave.DataSize = binaryReader.ReadInt32();

                // From the data size value read the actual sound data.
                wave.SoundData = binaryReader.ReadBytes(wave.DataSize);

                // Return the wave structure and data.
                return wave;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Write the structure and data to the .wav file.
        /// </summary>
        /// <param name="filePath">The full path and file name of the .wav file.</param>
        /// <param name="waveHeaderStructure">The complete structure and sound data of the .wav file.</param>
        /// <returns>True if the write was succesfull; else false.</returns>
        public bool Write(string filePath, WaveStructure waveHeaderStructure)
        {
            if (String.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");

            FileStream fileStream = null;
            BinaryWriter binaryWriter = null;

            try
            {
                // Create a new file stream for the file.
                fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write);

                // Create a new binary reader from the file stream
                // set the starting position at the begining
                binaryWriter = new BinaryWriter(fileStream);
                binaryWriter.BaseStream.Seek(0, SeekOrigin.Begin);

                // Write each pice of binary data to the file.
                binaryWriter.Write(waveHeaderStructure.ChunckID);
                binaryWriter.Write(waveHeaderStructure.FileSize);
                binaryWriter.Write(waveHeaderStructure.RiffType);
                binaryWriter.Write(waveHeaderStructure.FmtID);
                binaryWriter.Write(waveHeaderStructure.FmtSize);
                binaryWriter.Write(waveHeaderStructure.FmtCode);
                binaryWriter.Write(waveHeaderStructure.Channels);
                binaryWriter.Write(waveHeaderStructure.SampleRate);
                binaryWriter.Write(waveHeaderStructure.FmtAverageByteRate);
                binaryWriter.Write(waveHeaderStructure.FmtBlockAlign);
                binaryWriter.Write(waveHeaderStructure.BitsPerSample);

                // If the format size is not '16' then write additional
                // data to the wave file.
                if (waveHeaderStructure.FmtSize != 16)
                {
                    binaryWriter.Write(waveHeaderStructure.FmtExtraSize);
                    binaryWriter.Write(waveHeaderStructure.FmtExtraData);
                }

                // Write the 'data' indicator and the actual data size value.
                binaryWriter.Write(waveHeaderStructure.DataID);
                binaryWriter.Write(waveHeaderStructure.DataSize);

                // From the data size value write the actual sound data.
                binaryWriter.Write(waveHeaderStructure.SoundData);

                // Close the streams
                binaryWriter.Flush();
                binaryWriter.Close();
                fileStream.Close();

                // The write operation completed.
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (binaryWriter != null)
                    binaryWriter.Close();

                if (fileStream != null)
                    fileStream.Close();
            }
        }

        /// <summary>
        /// Write the structure and data to the .wav file.
        /// </summary>
        /// <param name="stream">The stream to write the data to.</param>
        /// <param name="waveHeaderStructure">The complete structure and sound data of the .wav file.</param>
        /// <returns>True if the write was succesfull; else false.</returns>
        public bool Write(Stream stream, WaveStructure waveHeaderStructure)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            BinaryWriter binaryWriter = null;

            try
            {
                // Create a new binary reader from the file stream
                // set the starting position at the begining
                binaryWriter = new BinaryWriter(stream);
                binaryWriter.BaseStream.Seek(0, SeekOrigin.Begin);

                // Write each pice of binary data to the file.
                binaryWriter.Write(waveHeaderStructure.ChunckID);
                binaryWriter.Write(waveHeaderStructure.FileSize);
                binaryWriter.Write(waveHeaderStructure.RiffType);
                binaryWriter.Write(waveHeaderStructure.FmtID);
                binaryWriter.Write(waveHeaderStructure.FmtSize);
                binaryWriter.Write(waveHeaderStructure.FmtCode);
                binaryWriter.Write(waveHeaderStructure.Channels);
                binaryWriter.Write(waveHeaderStructure.SampleRate);
                binaryWriter.Write(waveHeaderStructure.FmtAverageByteRate);
                binaryWriter.Write(waveHeaderStructure.FmtBlockAlign);
                binaryWriter.Write(waveHeaderStructure.BitsPerSample);

                // If the format size is not '16' then write additional
                // data to the wave file.
                if (waveHeaderStructure.FmtSize != 16)
                {
                    binaryWriter.Write(waveHeaderStructure.FmtExtraSize);
                    binaryWriter.Write(waveHeaderStructure.FmtExtraData);
                }

                // Write the 'data' indicator and the actual data size value.
                binaryWriter.Write(waveHeaderStructure.DataID);
                binaryWriter.Write(waveHeaderStructure.DataSize);

                // From the data size value write the actual sound data.
                binaryWriter.Write(waveHeaderStructure.SoundData);

                // Close the streams
                binaryWriter.Flush();
                
                // The write operation completed.
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Play the wave sound file asynchronously.
        /// </summary>
        /// <param name="filePath">The full path and file name of the .wav file.</param>
        public void Play(string filePath)
        {
            if (String.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");

            _soundPlayer = new System.Media.SoundPlayer(filePath);
            _soundPlayer.Play();
        }

        /// <summary>
        /// Play the wave sound stream asynchronously.
        /// </summary>
        /// <param name="stream">The stream .wav format to play.</param>
        public void Play(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            _soundPlayer = new System.Media.SoundPlayer(stream);
            _soundPlayer.Play();
        }

        /// <summary>
        /// Play the wave sound file synchronously.
        /// </summary>
        /// <param name="filePath">The full path and file name of the .wav file.</param>
        public void PlaySync(string filePath)
        {
            if (String.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");

            _soundPlayer = new System.Media.SoundPlayer(filePath);
            _soundPlayer.PlaySync();
        }

        /// <summary>
        /// Play the wave sound stream synchronously.
        /// </summary>
        /// <param name="stream">The stream .wav format to play.</param>
        public void PlaySync(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            _soundPlayer = new System.Media.SoundPlayer(stream);
            _soundPlayer.PlaySync();
        }

        /// <summary>
        /// Stops playback of the sound if playback is occurring.
        /// </summary>
        /// <param name="immediate">Whether to stop playing immediately, or to break out of the loop region and
        /// play the release. Specify true to stop playing immediately, or false to break
        /// out of the loop region and play the release phase (the remainder of the sound).</param>
        public void Stop(bool immediate = true)
        {
            if (_soundPlayer != null)
                _soundPlayer.Stop();
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_soundPlayer != null)
                        _soundPlayer.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _soundPlayer = null;

                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~WaveFormat()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
	}
}
