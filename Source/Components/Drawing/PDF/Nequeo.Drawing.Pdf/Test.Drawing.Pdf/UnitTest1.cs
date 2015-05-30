using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Leadtools;
using Leadtools.Codecs;
using Leadtools.Internal;

namespace Test.Drawing.Pdf
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            try
            {
                // Get the unlock key
                RasterSupport.Unlock(RasterSupportType.PdfSave, "SgAQ-BQ4A-AAAW-AIMA");

                // Startup the codecs
                RasterCodecs.Startup();
                using (RasterCodecs codecs = new RasterCodecs())
                {
                    // Load the image file into the image type.
                    using (System.Drawing.Image srcImage = System.Drawing.Image.FromFile(@"C:\Users\PC\Pictures\Gun\DSC01888.JPG"))
                    using (RasterImage image = new RasterImage(srcImage))
                    {
                        codecs.Options.Jpeg.Save.QualityFactor = 99;
                        codecs.Options.Jpeg.Save.SaveWithStamp = false;
                        codecs.Options.Jpeg.Save.StampBitsPerPixel = 0;
                        codecs.Options.Jpeg.Save.StampWidth = image.Width;
                        codecs.Options.Jpeg.Save.StampHeight = image.Height;

                        // Set the raster image format.
                        codecs.Options.Pdf.Save.SavePdfA = true;
                        RasterImageFormat rasterImageFormat = RasterImageFormat.RasPdfJpeg;

                        codecs.Save(image, @"C:\Users\PC\Documents\Drazen Resume\I_certify_that_this_is_a_true_copy.pdf", rasterImageFormat, 0);
                    }
                }
            }
            finally
            {
                // Shutdown the codecs
                RasterCodecs.Shutdown();
            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            
        }
    }
}
