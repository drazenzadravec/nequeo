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

namespace Nequeo.Model.Language
{
    /// <summary>
    /// Language translate collection model.
    /// </summary>
    public class LanguageTranslateCollection
    {
        /// <summary>
        /// Gets or sets the data model.
        /// </summary>
        public Data Data { get; set; }
    }

    /// <summary>
    /// The data model.
    /// </summary>
    public class Data
    {
        /// <summary>
        /// Gets or sets the array of translations.
        /// </summary>
        public TranslationEx[] Translations { get; set; }

        /// <summary>
        /// Gets or sets the array of language codes.
        /// </summary>
        public LanguageEx[] Languages { get; set; }

        /// <summary>
        /// Gets or sets the detected language code array.
        /// </summary>
        public DetectedLanguageEx[][] Detections { get; set; }
    }

    /// <summary>
    /// Translation model.
    /// </summary>
    public partial class TranslationEx
    {
        /// <summary>
        /// Gets or sets the translated text.
        /// </summary>
        public string TranslatedText { get; set; }

        /// <summary>
        /// Gets or sets the detected source langauge code.
        /// </summary>
        public string DetectedSourceLanguage { get; set; }
    }

    /// <summary>
    /// Language model.
    /// </summary>
    public partial class LanguageEx
    {
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the language name.
        /// </summary>
        public string Name { get; set; }

    }

    /// <summary>
    /// Detected language model.
    /// </summary>
    public partial class DetectedLanguageEx
    {
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the is reliable indicator.
        /// </summary>
        public bool IsReliable { get; set; }

        /// <summary>
        /// Gets or sets the confidence value.
        /// </summary>
        public double Confidence { get; set; }

    }
}
