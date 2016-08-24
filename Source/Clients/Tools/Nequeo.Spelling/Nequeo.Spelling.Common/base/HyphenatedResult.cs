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
    /// Holds the result of a hyphenation.
    /// </summary>
    public class HyphenatedResult
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="hyphenatedWord">
        /// The hyphenated word. 
        /// </param>
        /// <param name="hyphenationPoints">
        /// The hyphenation points. 
        /// </param>
        /// <param name="hyphenationRep">
        /// The hyphenation rep. 
        /// </param>
        /// <param name="hyphenationPos">
        /// The hyphenation pos. 
        /// </param>
        /// <param name="hyphenationCut">
        /// The hyphenation cut. 
        /// </param>
        public HyphenatedResult(string hyphenatedWord, byte[] hyphenationPoints, string[] hyphenationRep, int[] hyphenationPos, int[] hyphenationCut)
        {
            _word = hyphenatedWord;
            _points = hyphenationPoints;
            _rep = hyphenationRep;
            _pos = hyphenationPos;
            _cut = hyphenationCut;
        }

        /// <summary>
        ///   The cut.
        /// </summary>
        private readonly int[] _cut;

        /// <summary>
        ///   The points.
        /// </summary>
        private readonly byte[] _points;

        /*
         rep:       NULL (only standard hyph.), or replacements (hyphenation points
                    signed with `=' in replacements);
         pos:       NULL, or difference of the actual position and the beginning
                    positions of the change in input words;
         cut:       NULL, or counts of the removed characters of the original words
                    at hyphenation,

         Note: rep, pos, cut are complementary arrays to the hyphens, indexed with the
               character positions of the input word.
        */

        /// <summary>
        ///   The pos.
        /// </summary>
        private readonly int[] _pos;

        /// <summary>
        ///   The rep.
        /// </summary>
        private readonly string[] _rep;

        /// <summary>
        ///   The word.
        /// </summary>
        private readonly string _word;

        /// <summary>
        /// Gets the hyphenated word.
        /// </summary>
        /// <remarks>
        ///   The hyphentaion points are marked with a equal sign '='.
        /// </remarks>
        /// <value> The hyphenated word. </value>
        public string HyphenatedWord
        {
            get
            {
                return _word;
            }
        }

        /// <summary>
        /// Gets the hyphenation cuts.
        /// </summary>
        /// <value> The hyphenation cuts. </value>
        public int[] HyphenationCuts
        {
            get
            {
                return _cut;
            }
        }

        /// <summary>
        /// Gets the hyphenation points.
        /// </summary>
        /// <value> The hyphenation points. </value>
        public byte[] HyphenationPoints
        {
            get
            {
                return _points;
            }
        }

        /// <summary>
        /// Gets the hyphenation positions.
        /// </summary>
        /// <value> The hyphenation positions. </value>
        public int[] HyphenationPositions
        {
            get
            {
                return _pos;
            }
        }

        /// <summary>
        /// Gets the hyphenation replacements.
        /// </summary>
        /// <value> The hyphenation replacements. </value>
        public string[] HyphenationReplacements
        {
            get
            {
                return _rep;
            }
        }
    }
}
