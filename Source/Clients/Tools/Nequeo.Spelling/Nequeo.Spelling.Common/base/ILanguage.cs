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

using NHunspell;

namespace Nequeo.Spelling
{
    /// <summary>
    /// Dictionary language interface.
    /// </summary>
    public interface ILanguage
    {
        /// <summary>
        /// Is the spelling of the word correct.
        /// </summary>
        /// <param name="word">The word to spell.</param>
        /// <returns>True if correct; else false.</returns>
        bool Spell(string word);

        /// <summary>
        /// Get suggestions for the word.
        /// </summary>
        /// <param name="word">The word to find suggestions for.</param>
        /// <returns>The list of suggestions.</returns>
        List<string> Suggest(string word);

        /// <summary>
        /// Get morphs for the word.
        /// </summary>
        /// <param name="word">The word to analyse.</param>
        /// <returns>The list of morphs.</returns>
        List<string> Analyze(string word);

        /// <summary>
        /// Gets the word stems for the specified word.
        /// </summary>
        /// <param name="word">The word to find stems for.</param>
        /// <returns>The list of stems.</returns>
        List<string> Stem(string word);

        /// <summary>
        /// Generates the specified word by a sample.
        /// </summary>
        /// <param name="word">The word to generate.</param>
        /// <param name="samples">The sample word.</param>
        /// <returns>The list of words.</returns>
        List<string> Generate(string word, string samples);

        /// <summary>
        /// Hyphenate the word.
        /// </summary>
        /// <param name="word">The word to hyphenate.</param>
        /// <returns>The hyphenated result.</returns>
        HyphenatedResult Hyphenate(string word);

        /// <summary>
        /// Get the meaning and synonyms.
        /// </summary>
        /// <param name="word">The word to get the meaning of.</param>
        /// <returns>The thesaurus result.</returns>
        ThesaurusResult Thesaurus(string word);

    }
}
