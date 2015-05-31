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
    /// Video container structure.
    /// </summary>
    [Serializable]
    public struct VideoModel
    {
        /// <summary>
        /// The video header.
        /// </summary>
        public VideoHeader Header;

        /// <summary>
        /// The number of image models.
        /// </summary>
        public int ImageCount;

        /// <summary>
        /// The image collection.
        /// </summary>
        public ImageModel[] Video;

    }

    /// <summary>
    /// Video header container structure.
    /// </summary>
    [Serializable]
    public struct VideoHeader
    {
        /// <summary>
        /// True if video data is included; else false.
        /// </summary>
        public bool ContainsVideo;

        /// <summary>
        /// The frame rate used to capture video.
        /// </summary>
        public double FrameRate;

        /// <summary>
        /// The width frame size used to capture video.
        /// </summary>
        public int FrameSizeWidth;

        /// <summary>
        /// The height frame size used to capture video.
        /// </summary>
        public int FrameSizeHeight;

        /// <summary>
        /// The image collection type.
        /// </summary>
        public ImageCaptureType ImageType;

        /// <summary>
        /// The Compression algorithm.
        /// </summary>
        public Nequeo.IO.Compression.CompressionAlgorithmStreaming CompressionAlgorithm;

        /// <summary>
        /// The video time length.
        /// </summary>
        public double Duration;

    }

    /// <summary>
    /// Image container structure.
    /// </summary>
    [Serializable]
    public struct ImageModel
    {
        /// <summary>
        /// The size of the data collection.
        /// </summary>
        public int Size;

        /// <summary>
        /// The collection containing the image data.
        /// </summary>
        public byte[] Data;

    }

    /// <summary>
    /// Video details container structure.
    /// </summary>
    [Serializable]
    public struct VideoDetails
    {
        /// <summary>
        /// True if default video configuration is used.
        /// </summary>
        public bool UseDefault;

        /// <summary>
        /// The frame rate used to capture video.
        /// </summary>
        public double FrameRate;

        /// <summary>
        /// The width frame size used to capture video.
        /// </summary>
        public int FrameSizeWidth;

        /// <summary>
        /// The height frame size used to capture video.
        /// </summary>
        public int FrameSizeHeight;

    }
}
