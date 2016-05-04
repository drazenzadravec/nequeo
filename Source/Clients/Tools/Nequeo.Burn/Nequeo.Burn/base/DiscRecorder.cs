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
    /// Disc recoder details.
    /// </summary>
    public class DiscRecorder
    {
        private IDiscFormat2Data _mediaImage = null;
        private IDiscRecorder2 _recorder = null;
        private Action<string> _currentActionHandler = null;

        /// <summary>
        /// Gets or sets the progress action handler.
        /// </summary>
        public Action<string> ProgressAction
        {
            get { return _currentActionHandler; }
            set { _currentActionHandler = value; }
        }

        /// <summary>
        /// Gets or sets the index of the recorder.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the unique system recorder name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get or sets the unique volume name (this is not a drive letter).
        /// </summary>
        public string VolumeName { get; set; }

        /// <summary>
        /// Gets or sets the unique ID used to initialize the recorder.
        /// </summary>
        public string ActiveDiscRecorder { get; set; }

        /// <summary>
        /// Gets or sets the vendor ID in the device's INQUIRY data.
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// Gets or sets the Product ID in the device's INQUIRY data.
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets drive letters and NTFS mount points to access the recorder.
        /// </summary>
        public string[] VolumePathNames { get; set; }

        /// <summary>
        /// Gets or sets a list of all feature (IMAPI_FEATURE_PAGE_TYPE) pages with 'current' bit set to true.
        /// </summary>
        public int[] CurrentFeaturePages { get; set; }

        /// <summary>
        /// Gets or sets a list of all feature (IMAPI_FEATURE_PAGE_TYPE) pages supported by the device.
        /// </summary>
        public int[] SupportedFeaturePages { get; set; }

        /// <summary>
        /// Gets or sets a list of all profiles (IMAPI_PROFILE_TYPE) supported by the device.
        /// </summary>
        public int[] SupportedProfiles { get; set; }

        /// <summary>
        /// Gets or sets a list of all profiles (IMAPI_PROFILE_TYPE) with 'currentP' bit set to true.
        /// </summary>
        public int[] CurrentProfiles { get; set; }

        /// <summary>
        /// Gets or sets a list of all MODE PAGES (IMAPI_MODE_PAGE_TYPE) supported by the device.
        /// </summary>
        public int[] SupportedModePages { get; set; }

        /// <summary>
        /// Gets or sets supported media types (IMAPI_MEDIA_PHYSICAL_TYPE).
        /// </summary>
        public int[] SupportedMediaTypes { get; set; }

        /// <summary>
        /// Gets or sets an array of the write speeds supported for the 
        /// attached disc recorder and current media.
        /// </summary>
        public uint[] SupportedWriteSpeeds { get; set; }

        /// <summary>
        /// Gets or sets determines if the recorder object supports the given format.
        /// </summary>
        public bool IsRecorderSupported { get; set; }

        /// <summary>
        /// Gets or sets determines if the current media in a supported recorder object 
        /// supports the given format.
        /// </summary>
        public bool IsCurrentMediaSupported { get; set; }

        /// <summary>
        /// Gets or sets the state (usability) of the current media (IMAPI_FORMAT2_DATA_MEDIA_STATE).
        /// </summary>
        public int CurrentMediaStatus { get; set; }

        /// <summary>
        /// Gets or sets the current physical media type (IMAPI_MEDIA_PHYSICAL_TYPE).
        /// </summary>
        public int CurrentPhysicalMediaType { get; set; }

        /// <summary>
        /// Gets or sets the disc recorder.
        /// </summary>
        public IDiscRecorder2 Recorder 
        {
            get { return _recorder; }
            set { _recorder = value; }
        }

        /// <summary>
        /// Gets or sets the disc format media.
        /// </summary>
        public IDiscFormat2Data MediaImage
        {
            get { return _mediaImage; }
            set { _mediaImage = value; }
        }

        /// <summary>
        /// Displays the string of the specified IMAPI_FORMAT2_DATA_MEDIA_STATE.
        /// </summary>
        /// <param name="mediaStatus">The IMAPI_FORMAT2_DATA_MEDIA_STATE to display.</param>
        /// <returns>The media status text.</returns>
        public string MediaStatus(IMAPI_FORMAT2_DATA_MEDIA_STATE mediaStatus)
        {
            switch (mediaStatus)
            {
                case IMAPI_FORMAT2_DATA_MEDIA_STATE.IMAPI_FORMAT2_DATA_MEDIA_STATE_OVERWRITE_ONLY:
                    return "Currently, only overwriting is supported.";

                case IMAPI_FORMAT2_DATA_MEDIA_STATE.IMAPI_FORMAT2_DATA_MEDIA_STATE_APPENDABLE:
                    return "Media is currently appendable.";

                case IMAPI_FORMAT2_DATA_MEDIA_STATE.IMAPI_FORMAT2_DATA_MEDIA_STATE_FINAL_SESSION:
                    return "Media is in final writing session.";

                case IMAPI_FORMAT2_DATA_MEDIA_STATE.IMAPI_FORMAT2_DATA_MEDIA_STATE_DAMAGED:
                    return "Media is damaged.";

                case IMAPI_FORMAT2_DATA_MEDIA_STATE.IMAPI_FORMAT2_DATA_MEDIA_STATE_UNKNOWN:
                default:
                    return "Media state is unknown.";
            }
        }

        /// <summary>
        /// Displays the string of the specified IMAPI_MEDIA_PHYSICAL_TYPE.
        /// </summary>
        /// <param name="mediaType">The IMAPI_MEDIA_PHYSICAL_TYPE to display.</param>
        /// <returns>The media type text.</returns>
        public string GetMediaType(IMAPI_MEDIA_PHYSICAL_TYPE mediaType)
        {
            switch (mediaType)
            {
                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_UNKNOWN:
                    return "Empty device or an unknown disc type.";
                    
                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_CDROM:
                    return "CD-ROM";
                    
                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_CDR:
                    return "CD-R";
                    
                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_CDRW:
                        return "CD-RW";
                   
                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDROM:
                        return "Read-only DVD drive and/or disc";
                  
                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDRAM:
                        return "DVD-RAM";
                   
                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSR:
                        return "DVD+R";
               
                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSRW:
                        return "DVD+RW";
                   
                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSR_DUALLAYER:
                        return "DVD+R Dual Layer media";
                    
                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDDASHR:
                        return "DVD-R";
                 
                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDDASHRW:
                        return "DVD-RW";
                    
                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDDASHR_DUALLAYER:
                        return "DVD-R Dual Layer media";
             
                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DISK:
                        return "Randomly-writable, hardware-defect managed media type that reports the \"Disc\" profile as current.";
                   
                default:
                        return "Error!: MediaPhysicalType";
            }
        }

        /// <summary>
        /// Event handler - Progress updates when writing data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args">Argument of type IDiscFormat2DataEventArgs</param>
        internal void DiscFormat2Data_ProgressUpdate(
            [In, MarshalAs(UnmanagedType.IDispatch)] object sender, 
            [In, MarshalAs(UnmanagedType.IDispatch)] object args)
        {
            IDiscFormat2DataEventArgs progress = args as IDiscFormat2DataEventArgs;

            // If an progress action handler exists.
            if (_currentActionHandler != null)
            {
                String timeStatus = String.Format("Time: {0} / {1} ({2})",
                                    progress.ElapsedTime,
                                    progress.TotalTime,
                                    progress.ElapsedTime / progress.TotalTime);

                // Get the current action.
                switch (progress.CurrentAction)
                {
                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_VALIDATING_MEDIA:
                        {
                            _currentActionHandler("Validating media.");
                        } break;
                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_FORMATTING_MEDIA:
                        {
                            _currentActionHandler("Formatting media.");
                        } break;
                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_INITIALIZING_HARDWARE:
                        {
                            _currentActionHandler("Initializing Hardware.");
                        } break;
                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_CALIBRATING_POWER:
                        {
                            _currentActionHandler("Calibrating Power (OPC).");
                        } break;
                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_WRITING_DATA:
                        {
                            int totalSectors = progress.SectorCount;
                            int writtenSectors = progress.LastWrittenLba - progress.StartLba;
                            int percentDone = writtenSectors / totalSectors;
                            _currentActionHandler("Progress: " + percentDone.ToString() + " - ");
                        } break;
                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_FINALIZATION:
                        {
                            _currentActionHandler("Finishing the writing.");
                        } break;
                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_COMPLETED:
                        {
                            _currentActionHandler("Completed the burn.");
                        } break;
                    default:
                        {
                            _currentActionHandler("Error!!!! Unknown Action: " + progress.CurrentAction.ToString("X"));
                        } break;
                }
                _currentActionHandler(timeStatus);
            }
        }
    }
}
