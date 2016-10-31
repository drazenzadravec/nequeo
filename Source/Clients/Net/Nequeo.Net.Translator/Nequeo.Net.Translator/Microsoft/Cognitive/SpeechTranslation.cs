/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
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
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Translator.Microsoft.Cognitive
{
    /// <summary>
    /// Speech translation result model.
    /// </summary>
    public class SpeechTranslation
    {
        /// <summary>
        /// String constant to identify the type of result. The value is partial for partial results.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// String identifier assigned to the recognition result.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Recognized text in the source language.
        /// </summary>
        public string Recognition { get; set; }

        /// <summary>
        /// Recognized text translated in the target language.
        /// </summary>
        public string Translation { get; set; }

        /// <summary>
        /// Byte offset of the start of the recognition. The offset is relative to the beginning of the stream.
        /// </summary>
        public long AudioStreamPosition { get; set; }

        /// <summary>
        /// Size in bytes of the recognition.
        /// </summary>
        public long AudioSizeBytes { get; set; }

        /// <summary>
        /// Time offset of the start of the recognition in ticks (1 tick = 100 nanoseconds). The offset is relative to the beginning of streaming.
        /// </summary>
        public long AudioTimeOffset { get; set; }

        /// <summary>
        /// Duration in ticks (100 nanoseconds) of the recognition.
        /// </summary>
        public long AudioTimeSize { get; set; }
    }
}
