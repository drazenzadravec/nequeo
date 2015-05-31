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
    /// Video audio container structure.
    /// </summary>
    [Serializable]
    public struct VideoAudioModel
    {
        /// <summary>
        /// The current media format.
        /// </summary>
        public int MediaFormat;

        /// <summary>
        /// Video data.
        /// </summary>
        public VideoModel? Video;

        /// <summary>
        /// Audio data.
        /// </summary>
        public AudioModel? Audio;
    }

    /// <summary>
    /// Video audio container structure.
    /// </summary>
    [Serializable]
    public struct VideoAudio
    {
        /// <summary>
        /// The image collection.
        /// </summary>
        public ImageModel[] Video;

        /// <summary>
        /// The sound collection.
        /// </summary>
        public SoundModel[] Audio;

    }

    /// <summary>
    /// Video audio container structure.
    /// </summary>
    [Serializable]
    public struct VideoAudioHeader
    {
        /// <summary>
        /// The current media format.
        /// </summary>
        public int MediaFormat;

        /// <summary>
        /// The video header.
        /// </summary>
        public VideoHeader? Video;

        /// <summary>
        /// The audio header.
        /// </summary>
        public AudioHeader? Audio;

    }
}
