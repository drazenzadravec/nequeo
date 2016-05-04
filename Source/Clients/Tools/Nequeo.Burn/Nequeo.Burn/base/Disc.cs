using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

using Nequeo.Burn.Imapi2.Interop;

namespace Nequeo.Burn.Imapi2
{
    /// <summary>
    /// Disc burning provider.
    /// </summary>
    public class Disc
    {
        /// <summary>
        /// Disc burning provider.
        /// </summary>
        public Disc()
        {
            // Create a DiscMaster2 object to connect to CD/DVD drives.
            _discMaster = new MsftDiscMaster2();

            // Set the recorder list.
            SetRecoderList();
        }

        private List<DiscRecorder> _discRecoders = null;
        private IDiscMaster2 _discMaster = null;

        /// <summary>
        /// Burns data files to disc in a single session using files from a 
        /// single directory tree.
        /// </summary>
        /// <param name="recorder">Burning device.</param>
        /// <param name="path">Directory of files to burn.</param>
        /// <param name="clientName">The friendly name of the client (used to determine recorder reservation conflicts).</param>
        public void BurnDirectory(DiscRecorder recorder, string path, string clientName = "IMAPI Client Record Name")
        {
            if (!recorder.MediaImage.IsRecorderSupported(recorder.Recorder))
                throw new Exception("The recorder is not supported");

            if (!recorder.MediaImage.IsCurrentMediaSupported(recorder.Recorder))
                throw new Exception("The current media is not supported");

            // Set the client name.
            recorder.MediaImage.ClientName = clientName;

            // Create an image stream for a specified directory.
            // Create a new file system image and retrieve the root directory
            IFileSystemImage fsi = new MsftFileSystemImage();

            // Set the media size
            fsi.FreeMediaBlocks = recorder.MediaImage.FreeSectorsOnMedia;

            // Use legacy ISO 9660 Format
            fsi.FileSystemsToCreate = FsiFileSystems.FsiFileSystemISO9660;

            // Add the directory to the disc file system
            IFsiDirectoryItem dir = fsi.Root;
            dir.AddTree(path, false);

            // Write the progress.
            if(recorder.ProgressAction != null)
                recorder.ProgressAction("Writing content to disc.");

            // Create an image from the file system.
            // Data stream sent to the burning device
            IFileSystemImageResult result = fsi.CreateResultImage();
            IStream stream = result.ImageStream;

            DiscFormat2Data_Events progress = recorder.MediaImage as DiscFormat2Data_Events;
            progress.Update += new DiscFormat2Data_EventsHandler(recorder.DiscFormat2Data_ProgressUpdate);

            // Write the image stream to disc using the specified recorder.
            recorder.MediaImage.Write(stream);   // Burn the stream to disc
            progress.Update -= new DiscFormat2Data_EventsHandler(recorder.DiscFormat2Data_ProgressUpdate);

            // Write the progress.
            if (recorder.ProgressAction != null)
                recorder.ProgressAction("Finished writing content.");
        }

        /// <summary>
        /// Get the list of disc recorders.
        /// </summary>
        /// <returns>The list of recorders</returns>
        public DiscRecorder[] GetRecorders()
        {
            return _discRecoders.ToArray();
        }

        /// <summary>
        /// Set the recorder list.
        /// </summary>
        private void SetRecoderList()
        {
            // If recorders exist.
            if (_discMaster.Count > 0)
            {
                int count = 0;

                // Create the list of recorders.
                _discRecoders = new List<DiscRecorder>();

                // For each recirder found.
                foreach (string device in _discMaster)
                {
                    // Initialize the DiscRecorder object for the specified burning device.
                    IDiscRecorder2 recorder = new MsftDiscRecorder2();
                    recorder.InitializeDiscRecorder(_discMaster[count]);

                    // Get the media image.
                    IDiscFormat2Data mediaImage = new MsftDiscFormat2Data();
                    mediaImage.Recorder = recorder;
                    
                    // Create a new recorder.
                    DiscRecorder rec = new DiscRecorder()
                    {
                        Index = count,
                        Name = device,
                        Recorder = recorder,
                        MediaImage = mediaImage,
                        VolumeName = recorder.VolumeName,
                        ProductId = recorder.ProductId,
                        VendorId = recorder.VendorId,
                        ActiveDiscRecorder = recorder.ActiveDiscRecorder,
                        VolumePathNames = recorder.VolumePathNames,
                        CurrentFeaturePages = recorder.CurrentFeaturePages,
                        CurrentProfiles = recorder.CurrentProfiles,
                        SupportedFeaturePages = recorder.SupportedFeaturePages,
                        SupportedProfiles = recorder.SupportedProfiles,
                        SupportedModePages = recorder.SupportedModePages,
                        IsRecorderSupported = mediaImage.IsRecorderSupported(recorder),
                        IsCurrentMediaSupported = mediaImage.IsCurrentMediaSupported(recorder),
                        SupportedMediaTypes = mediaImage.SupportedMediaTypes
                    };

                    // Add the recorder to the list.
                    _discRecoders.Add(rec);
                    count++;
                }
            }
        }
    }
}
