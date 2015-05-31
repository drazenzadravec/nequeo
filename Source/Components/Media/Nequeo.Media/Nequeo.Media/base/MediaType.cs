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
using System.IO;
using System.Drawing;

namespace Nequeo.Media
{
    /// <summary>
    /// Possible media active types.
    /// </summary>
    [Flags]
    [Serializable]
    public enum MediaActiveType : int
    {
        /// <summary>
        /// Video
        /// </summary>
        Video = 1,
        /// <summary>
        /// Audio
        /// </summary>
        Audio = 2,
    }

    /// <summary>
    /// Possible capture media format types
    /// </summary>
    [Serializable]
    public enum MediaFormatType
    {
        /// <summary>
        /// Avi media format video and audio (.avi file extension);
        /// </summary>
        Avi,
        /// <summary>
        /// Wmv media format video and audio (.wmv file extension).
        /// </summary>
        Wmv,
        /// <summary>
        /// Wav media format audio only (.wav file extension).
        /// </summary>
        Wav,
        /// <summary>
        /// Wma media format audio only (.wma file extension).
        /// </summary>
        Wma,
        /// <summary>
        /// Mpeg media format video and audio (.mpeg file extension).
        /// </summary>
        Mpeg,
        /// <summary>
        /// Mpeg media format audio only (.mpeg file extension).
        /// </summary>
        MpegAudio,
    }

    /// <summary>
    /// Possible image capture format types.
    /// </summary>
    [Serializable]
    public enum ImageCaptureType
    {
        /// <summary>
        /// Bitmap image.
        /// </summary>
        Bmp,
        /// <summary>
        /// Jpeg image.
        /// </summary>
        Jpg,
    }

    /// <summary>
    /// Possible sound capture format types.
    /// </summary>
    [Serializable]
    public enum SoundCaptureType
    {
        /// <summary>
        /// The raw PCM data.
        /// </summary>
        Pcm,
        /// <summary>
        /// Wave sound.
        /// </summary>
        Wav,
    }

    /// <summary>
    /// Decode encode the image and sound types.
    /// </summary>
    internal class Helper
    {
        /// <summary>
        /// Get the image type.
        /// </summary>
        /// <param name="imageType">The image type.</param>
        /// <returns>The image type index.</returns>
        public static int GetImageTypeInt32(ImageCaptureType imageType)
        {
            // Select the image type.
            switch(imageType)
            {
                case ImageCaptureType.Jpg:
                    return 1;
                case ImageCaptureType.Bmp:
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Get the sound type.
        /// </summary>
        /// <param name="soundType">The sound type.</param>
        /// <returns>The sound type index.</returns>
        public static int GetSoundTypeInt32(SoundCaptureType soundType)
        {
            // Select the sound type.
            switch (soundType)
            {
                case SoundCaptureType.Pcm:
                    return 0;
                case SoundCaptureType.Wav:
                default:
                    return 1;
            }
        }

        /// <summary>
        /// Get the image type.
        /// </summary>
        /// <param name="imageIndex">The image type.</param>
        /// <returns>The image type.</returns>
        public static ImageCaptureType GetImageType(int imageIndex)
        {
            switch(imageIndex)
            {
                case 1:
                    return ImageCaptureType.Jpg;
                case 0:
                default:
                    return ImageCaptureType.Bmp;
            }
        }

        /// <summary>
        /// Get the sound type.
        /// </summary>
        /// <param name="soundIndex">The sound type.</param>
        /// <returns>The sound type.</returns>
        public static SoundCaptureType GetSoundType(int soundIndex)
        {
            switch (soundIndex)
            {
                case 0:
                    return SoundCaptureType.Pcm;
                case 1:
                default:
                    return SoundCaptureType.Wav;
            }
        }
    }
}
