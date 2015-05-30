/*  Company :       Nequeo Pty Ltd, http://www.nequeo.net.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2011 http://www.nequeo.net.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Leadtools;
using Leadtools.Codecs;
using Leadtools.Internal;

using Nequeo.Exceptions;
using Nequeo.Drawing;
using Nequeo.Drawing.Pdf.Common;

namespace Nequeo.Drawing.Pdf
{
    /// <summary>
    /// Image to PDF conversion.
    /// </summary>
    public class Image : IPdfConvert
	{
        #region Constructors

        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly Image Instance = new Image();

        /// <summary>
        /// Static constructor
        /// </summary>
        static Image() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public Image()
        {
        }

        #endregion

        #region Private Fields

        private string _documentUnlockKey = "SgAQ-BQ4A-AAAW-AIMA";
        private int _pdfDocumentQualityFactor = 99;
        private PdfSaveType _saveType = PdfSaveType.Pdf14;
        private int _pdfDocumentBitPerPixel = 24;
        private ConversionType _conversionType = ConversionType.Jpeg;
        private string _pdfDocumentPath = string.Empty;
        private string _imageDocumentPath = string.Empty;
        private string _leadToolResPath = string.Empty;
        private List<string> _currentErrors = new List<string>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets sets, the image convserion type.
        /// </summary>
        public ConversionType ConversionType
        {
            get { return _conversionType; }
            set { _conversionType = value; }
        }

        /// <summary>
        /// Gets sets, the pdf save type.
        /// </summary>
        public PdfSaveType PdfSaveType
        {
            get { return _saveType; }
            set { _saveType = value; }
        }

        /// <summary>
        /// Gets sets, the pdf document quality factor.
        /// </summary>
        public int PdfDocumentQualityFactor
        {
            get { return _pdfDocumentQualityFactor; }
            set { _pdfDocumentQualityFactor = value; }
        }

        /// <summary>
        /// Gets sets, the pdf document bits per pixel.
        /// </summary>
        public int PdfDocumentBitPerPixel
        {
            get { return _pdfDocumentBitPerPixel; }
            set { _pdfDocumentBitPerPixel = value; }
        }

        /// <summary>
        /// Gets sets, the document unlock key.
        /// </summary>
        public string DocumentUnlockKey
        {
            get { return _documentUnlockKey; }
            set { _documentUnlockKey = value; }
        }

        /// <summary>
        /// Gets sets, the pdf document path.
        /// </summary>
        public string PdfDocumentPath
        {
            get { return _pdfDocumentPath; }
            set { _pdfDocumentPath = value; }
        }

        /// <summary>
        /// Gets sets, the image document path.
        /// </summary>
        public string ImageDocumentPath
        {
            get { return _imageDocumentPath; }
            set { _imageDocumentPath = value; }
        }

        /// <summary>
        /// Gets sets, the lead tool ressource path.
        /// </summary>
        public string LeadToolResPath
        {
            get { return _leadToolResPath; }
            set { _leadToolResPath = value; }
        }

        #endregion

        #region Public Events

        /// <summary>
        /// On save all image event error.
        /// </summary>
        public event System.EventHandler<ConversionArgs> OnError;

        /// <summary>
        /// On write/read complete.
        /// </summary>
        public event System.EventHandler OnComplete;

        #endregion

        #region Public Methods

        /// <summary>
        /// Convert a image to a PDF file.
        /// </summary>
        /// <param name="imageSource">The image source file name and path.</param>
        /// <param name="pdfDestination">The PDF destination file name and path.</param>
        /// <returns>True if the conversion was successful; else false.</returns>
        public virtual bool ConvertImageToPdf(string imageSource, string pdfDestination)
        {
            _pdfDocumentPath = pdfDestination;
            _imageDocumentPath = imageSource;

            // Attempt to convert the image.
            return ConvertImage();
        }

        /// <summary>
        /// Converts images into a PDF file.
        /// </summary>
        /// <param name="imageSource">The image source files name and path.</param>
        /// <param name="pdfDestination">The PDF destination file name and path.</param>
        /// <returns>True if the conversion was successful; else false.</returns>
        public virtual bool ConvertImageToPdf(string[] imageSource, string pdfDestination)
        {
            _pdfDocumentPath = pdfDestination;

            // Attempt to convert the image.
            return ConvertImage(imageSource);
        }

        /// <summary>
        /// Convert a JPEG image to a PDF file.
        /// </summary>
        /// <param name="jpegSource">The JPEG source file name and path.</param>
        /// <param name="pdfDestination">The PDF destination file name and path.</param>
        /// <returns>True if the conversion was successful; else false.</returns>
        public virtual bool ConvertJpegToPdf(string jpegSource, string pdfDestination)
        {
            _pdfDocumentPath = pdfDestination;
            _imageDocumentPath = jpegSource;

            // Attempt to convert the image.
            return ConvertImage();
        }

        /// <summary>
        /// Converts JPEG images into a PDF file.
        /// </summary>
        /// <param name="jpegSource">The JPEG source files name and path.</param>
        /// <param name="pdfDestination">The PDF destination file name and path.</param>
        /// <returns>True if the conversion was successful; else false.</returns>
        public virtual bool ConvertJpegToPdf(string[] jpegSource, string pdfDestination)
        {
            _pdfDocumentPath = pdfDestination;

            // Attempt to convert the image.
            return ConvertImage(jpegSource);
        }

        /// <summary>
        /// Convert a TIFF image to a PDF file.
        /// </summary>
        /// <param name="tiffSource">The TIFF source file name and path.</param>
        /// <param name="pdfDestination">The PDF destination file name and path.</param>
        /// <returns>True if the conversion was successful; else false.</returns>
        public virtual bool ConvertTiffToPdf(string tiffSource, string pdfDestination)
        {
            _pdfDocumentPath = pdfDestination;
            _imageDocumentPath = tiffSource;

            // Attempt to convert the image.
            return ConvertImage();
        }

        /// <summary>
        /// Converts TIFF images into a PDF file.
        /// </summary>
        /// <param name="tiffSource">The TIFF source files name and path.</param>
        /// <param name="pdfDestination">The PDF destination file name and path.</param>
        /// <returns>True if the conversion was successful; else false.</returns>
        public virtual bool ConvertTiffToPdf(string[] tiffSource, string pdfDestination)
        {
            _pdfDocumentPath = pdfDestination;

            // Attempt to convert the image.
            return ConvertImage(tiffSource);
        }

        /// <summary>
        /// Save the image file to a Pdf file.
        /// </summary>
        /// <param name="sourceFile">The source file name to convert from.</param>
        /// <param name="destinationFile">The destination PDF file name.</param>
        /// <param name="conversionType">The type of image to convert to PDF</param>
        /// <returns>True if the conversion was successful; else false.</returns>
        public virtual bool Save(string sourceFile, string destinationFile, ConversionType conversionType)
        {
            try
            {
                // Assign each configuration value to the property.
                _documentUnlockKey = Nequeo.Drawing.Pdf.Properties.Settings.Default.DocumentUnlockKey;
                _pdfDocumentQualityFactor = Nequeo.Drawing.Pdf.Properties.Settings.Default.PdfDocumentQualityFactor;
                _saveType = Nequeo.Drawing.Pdf.Properties.Settings.Default.PdfDocumentProfile;
                _pdfDocumentBitPerPixel = Nequeo.Drawing.Pdf.Properties.Settings.Default.PdfDocumentBitPerPixel;
                _conversionType = conversionType;
                _pdfDocumentPath = destinationFile;
                _imageDocumentPath = sourceFile;
                _leadToolResPath = Nequeo.Drawing.Pdf.Properties.Settings.Default.LeadToolResPath;

                // Attempt to convert the image.
                return ConvertImage();
            }
            catch (System.Exception ex)
            {
                // Add the error message.
                _currentErrors.Add(
                    "Type : Image |\r\n" +
                    "Member : Save |\r\n" +
                    "Message : " + ex.Message + "|\r\n" +
                    "Stack : " + ex.StackTrace);

                // If the event has been attached, then
                // send an error to the client.
                if (OnError != null)
                    OnError(this, new ConversionArgs(1, ex.Message));

                return false;
            }
        }

        /// <summary>
        /// Save the image file to a Pdf file.
        /// </summary>
        /// <param name="sourceFile">The source file name to convert from.</param>
        /// <param name="destinationFile">The destination PDF file name.</param>
        /// <returns>True if the conversion was successful; else false.</returns>
        public virtual bool Save(string sourceFile, string destinationFile)
        {
            try
            {
                // Assign each configuration value to the property.
                _documentUnlockKey = Nequeo.Drawing.Pdf.Properties.Settings.Default.DocumentUnlockKey;
                _pdfDocumentQualityFactor = Nequeo.Drawing.Pdf.Properties.Settings.Default.PdfDocumentQualityFactor;
                _saveType = Nequeo.Drawing.Pdf.Properties.Settings.Default.PdfDocumentProfile;
                _pdfDocumentBitPerPixel = Nequeo.Drawing.Pdf.Properties.Settings.Default.PdfDocumentBitPerPixel;
                _conversionType = Nequeo.Drawing.Pdf.Properties.Settings.Default.ConversionType;
                _pdfDocumentPath = destinationFile;
                _imageDocumentPath = sourceFile;
                _leadToolResPath = Nequeo.Drawing.Pdf.Properties.Settings.Default.LeadToolResPath;

                // Attempt to convert the image.
                return ConvertImage();
            }
            catch (System.Exception ex)
            {
                // Add the error message.
                _currentErrors.Add(
                    "Type : Image |\r\n" +
                    "Member : Save |\r\n" +
                    "Message : " + ex.Message + "|\r\n" +
                    "Stack : " + ex.StackTrace);

                // If the event has been attached, then
                // send an error to the client.
                if (OnError != null)
                    OnError(this, new ConversionArgs(1, ex.Message));

                return false;
            }
        }

        /// <summary>
        /// Save the image to PDF, using the configuration data.
        /// </summary>
        /// <returns>True if the conversion was successful; else false.</returns>
        public virtual bool Save()
        {
            try
            {
                // Assign each configuration value to the property.
                _documentUnlockKey = Nequeo.Drawing.Pdf.Properties.Settings.Default.DocumentUnlockKey;
                _pdfDocumentQualityFactor = Nequeo.Drawing.Pdf.Properties.Settings.Default.PdfDocumentQualityFactor;
                _saveType = Nequeo.Drawing.Pdf.Properties.Settings.Default.PdfDocumentProfile;
                _pdfDocumentBitPerPixel = Nequeo.Drawing.Pdf.Properties.Settings.Default.PdfDocumentBitPerPixel;
                _conversionType = Nequeo.Drawing.Pdf.Properties.Settings.Default.ConversionType;
                _pdfDocumentPath = Nequeo.Drawing.Pdf.Properties.Settings.Default.PdfDocumentPath;
                _imageDocumentPath = Nequeo.Drawing.Pdf.Properties.Settings.Default.ImageDocumentPath;
                _leadToolResPath = Nequeo.Drawing.Pdf.Properties.Settings.Default.LeadToolResPath;

                // Attempt to convert the image.
                return ConvertImage();
            }
            catch (System.Exception ex)
            {
                // Add the error message.
                _currentErrors.Add(
                    "Type : Image |\r\n" +
                    "Member : Save |\r\n" +
                    "Message : " + ex.Message + "|\r\n" +
                    "Stack : " + ex.StackTrace);

                // If the event has been attached, then
                // send an error to the client.
                if (OnError != null)
                    OnError(this, new ConversionArgs(1, ex.Message));

                return false;
            }
        }

        /// <summary>
        /// Gets the list of errors if any.
        /// </summary>
        /// <returns>The list of errors; else empty collection.</returns>
        public virtual string[] GetErrors()
        {
            return _currentErrors.ToArray();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Convert the current image to a PDF document.
        /// </summary>
        /// <returns>True if the conversion was successful; else false.</returns>
        private bool ConvertImage()
        {
            try
            {
                // Get the unlock key
                RasterSupport.Unlock(RasterSupportType.PdfSave, _documentUnlockKey);

                // Startup the codecs
                RasterCodecs.Startup();
                using (RasterCodecs codecs = new RasterCodecs())
                {
                    // Load the image file into the image type.
                    using (System.Drawing.Image srcImage = System.Drawing.Image.FromFile(_imageDocumentPath))
                    using (RasterImage image = new RasterImage(srcImage))
                    {
                        codecs.Options.Jpeg.Save.QualityFactor = _pdfDocumentQualityFactor;
                        codecs.Options.Jpeg.Save.SaveWithStamp = false;
                        codecs.Options.Jpeg.Save.StampBitsPerPixel = 0;
                        codecs.Options.Jpeg.Save.StampWidth = image.Width;
                        codecs.Options.Jpeg.Save.StampHeight = image.Height;

                        // Set the raster image format.
                        RasterImageFormat rasterImageFormat = RasterImageFormat.RasPdfJpeg411;

                        // Set the pdf save type.
                        switch (_saveType)
                        {
                            case PdfSaveType.PdfA:
                                codecs.Options.Pdf.Save.SavePdfA = true;
                                rasterImageFormat = RasterImageFormat.RasPdfJpeg;
                                break;

                            case PdfSaveType.Pdf14:
                                codecs.Options.Pdf.Save.SavePdfv14 = true;
                                rasterImageFormat = RasterImageFormat.RasPdfJpeg411;
                                break;

                            case PdfSaveType.Pdf15:
                                codecs.Options.Pdf.Save.SavePdfv15 = true;
                                rasterImageFormat = RasterImageFormat.RasPdfJpeg422;
                                break;
                        }

                        // If the load tool resource path has been specified.
                        if (!String.IsNullOrEmpty(_leadToolResPath))
                            codecs.Options.Pdf.InitialPath = _leadToolResPath + "Lib;" + _leadToolResPath + "Fonts;" + _leadToolResPath + "Resource";

                        // Attempt to save the image.
                        codecs.Save(image, _pdfDocumentPath, rasterImageFormat, _pdfDocumentBitPerPixel);
                    }
                }

                return true;
            }
            finally
            {
                // Shutdown the codecs
                RasterCodecs.Shutdown();

                // If the event has been attached, then
                // send a complete to the client.
                if (OnComplete != null)
                    OnComplete(this, new EventArgs());
            }
        }

        /// <summary>
        /// Converts all the image file into one PDF file.
        /// </summary>
        /// <param name="imagePath">The collection of image files to combine.</param>
        /// <returns>True if the conversion was successful; else false.</returns>
        private bool ConvertImage(string[] imagePath)
        {
            try
            {
                // Get the unlock key
                RasterSupport.Unlock(RasterSupportType.PdfSave, _documentUnlockKey);

                // Startup the codecs
                RasterCodecs.Startup();
                using (RasterCodecs codecs = new RasterCodecs())
                {
                    // Assign the codec options
                    codecs.Options.Jpeg.Save.QualityFactor = _pdfDocumentQualityFactor;
                    codecs.Options.Jpeg.Save.SaveWithStamp = false;
                    codecs.Options.Jpeg.Save.StampBitsPerPixel = 0;

                    // Set the raster image format.
                    RasterImageFormat rasterImageFormat = RasterImageFormat.RasPdfJpeg411;

                    // Set the pdf save type.
                    switch (_saveType)
                    {
                        case PdfSaveType.PdfA:
                            codecs.Options.Pdf.Save.SavePdfA = true;
                            rasterImageFormat = RasterImageFormat.RasPdfJpeg;
                            break;

                        case PdfSaveType.Pdf14:
                            codecs.Options.Pdf.Save.SavePdfv14 = true;
                            rasterImageFormat = RasterImageFormat.RasPdfJpeg411;
                            break;

                        case PdfSaveType.Pdf15:
                            codecs.Options.Pdf.Save.SavePdfv15 = true;
                            rasterImageFormat = RasterImageFormat.RasPdfJpeg422;
                            break;
                    }

                    // If the load tool resource path has been specified.
                    if (!String.IsNullOrEmpty(_leadToolResPath))
                        codecs.Options.Pdf.InitialPath = _leadToolResPath + "Lib;" + _leadToolResPath + "Fonts;" + _leadToolResPath + "Resource";

                    // For each image file found append into the PDF document.
                    for (int i = 0; i < imagePath.Length; i++)
                    {
                        // Load the image file into the image type.
                        using (System.Drawing.Image srcImage = System.Drawing.Image.FromFile(imagePath[i]))
                        using (RasterImage image = new RasterImage(srcImage))
                        {
                            if (i > 0)
                                // Append each image file to the PDF document.
                                codecs.Save(image, _pdfDocumentPath, rasterImageFormat, _pdfDocumentBitPerPixel, 1, 1, 1, CodecsSavePageMode.Append);
                            else
                                // Create or overwrite the PDF document and
                                // place to first image at the top of the PDF.
                                codecs.Save(image, _pdfDocumentPath, rasterImageFormat, _pdfDocumentBitPerPixel, 1, 1, 1, CodecsSavePageMode.Overwrite);
                        }
                    }
                }
            
                // Return the result.
                return true;
            }
            finally
            {
                // Shutdown the codecs
                RasterCodecs.Shutdown();

                // If the event has been attached, then
                // send a complete to the client.
                if (OnComplete != null)
                    OnComplete(this, new EventArgs());
            }
        }

        /// <summary>
        /// Validate the input data.
        /// </summary>
        /// <param name="value">The object value.</param>
        /// <param name="memberName">The name of the member.</param>
        /// <returns>The object value</returns>
        /// <exception cref="System.Exception"></exception>
        private object ValidateSetting(object value, string memberName)
        {
            switch (value.GetType().FullName.ToLower())
            {
                case "system.string":
                case "string":
                    if (String.IsNullOrEmpty(value.ToString()))
                        throw new EmptyStringException(memberName + " " + Nequeo.Drawing.Pdf.Properties.Resources.NoValue);
                    else
                        return value;

                case "system.int16":
                case "short":
                case "system.int64":
                case "long":
                case "system.int32":
                case "int":
                    if (String.IsNullOrEmpty(value.ToString()))
                        throw new EmptyStringException(memberName + " " + Nequeo.Drawing.Pdf.Properties.Resources.NoValue);
                    else
                    {
                        long number;
                        bool ret = long.TryParse(value.ToString(), out number);
                        if (!ret)
                            throw new System.ArithmeticException(memberName + " " + Nequeo.Drawing.Pdf.Properties.Resources.NotNumber);
                        else
                            return value;
                    }

                case "system.boolean":
                case "bool":
                    if (String.IsNullOrEmpty(value.ToString()))
                        throw new EmptyStringException(memberName + " " + Nequeo.Drawing.Pdf.Properties.Resources.NoValue);
                    else
                    {
                        bool number;
                        bool ret = bool.TryParse(value.ToString(), out number);
                        if (!ret)
                            throw new System.ArithmeticException(memberName + " " + Nequeo.Drawing.Pdf.Properties.Resources.NotBoolean);
                        else
                            return value;
                    }

                default:
                    return value;
            }
        }

        #endregion
	}
}
