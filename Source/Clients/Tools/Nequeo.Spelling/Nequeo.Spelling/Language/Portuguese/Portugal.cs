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

namespace Nequeo.Spelling.Language.Portuguese
{
    /// <summary>
    /// Portugal portuguese dictionary.
    /// </summary>
    public sealed class Portugal : ILanguage, IDisposable
    {
        /// <summary>
        /// Portugal portuguese dictionary.
        /// </summary>
        public Portugal()
        {
            // Load the data.
            _hyphen = new Hyphen(Nequeo.Spelling.Properties.Resources.hyph_pt_PT);
            _thesaurus = new MyThes(Nequeo.Spelling.Properties.Resources.th_pt_PT);
            _hunspell = new Hunspell(Nequeo.Spelling.Properties.Resources.pt_PT, Nequeo.Spelling.Properties.Resources.pt_PT1);
        }

        private Hyphen _hyphen = null;
        private MyThes _thesaurus = null;
        private Hunspell _hunspell = null;

        /// <summary>
        /// Is the spelling of the word correct.
        /// </summary>
        /// <param name="word">The word to spell.</param>
        /// <returns>True if correct; else false.</returns>
        public bool Spell(string word)
        {
            return _hunspell.Spell(word);
        }

        /// <summary>
        /// Get suggestions for the word.
        /// </summary>
        /// <param name="word">The word to find suggestions for.</param>
        /// <returns>The list of suggestions.</returns>
        public List<string> Suggest(string word)
        {
            return _hunspell.Suggest(word);
        }

        /// <summary>
        /// Get morphs for the word.
        /// </summary>
        /// <param name="word">The word to analyse.</param>
        /// <returns>The list of morphs.</returns>
        public List<string> Analyze(string word)
        {
            return _hunspell.Analyze(word);
        }

        /// <summary>
        /// Gets the word stems for the specified word.
        /// </summary>
        /// <param name="word">The word to find stems for.</param>
        /// <returns>The list of stems.</returns>
        public List<string> Stem(string word)
        {
            return _hunspell.Stem(word);
        }

        /// <summary>
        /// Generates the specified word by a sample.
        /// </summary>
        /// <param name="word">The word to generate.</param>
        /// <param name="samples">The sample word.</param>
        /// <returns>The list of words.</returns>
        public List<string> Generate(string word, string samples)
        {
            return _hunspell.Generate(word, samples);
        }

        /// <summary>
        /// Hyphenate the word.
        /// </summary>
        /// <param name="word">The word to hyphenate.</param>
        /// <returns>The hyphenated result.</returns>
        public HyphenatedResult Hyphenate(string word)
        {
            if (_hyphen != null)
            {
                // Get the hyphenation.
                HyphenResult result = _hyphen.Hyphenate(word);
                HyphenatedResult hyResult = new HyphenatedResult(
                    result.HyphenatedWord,
                    result.HyphenationPoints,
                    result.HyphenationReplacements,
                    result.HyphenationPositions,
                    result.HyphenationCuts);

                // Return the result.
                return hyResult;
            }
            else
                throw new NotImplementedException();
        }

        /// <summary>
        /// Get the meaning and synonyms.
        /// </summary>
        /// <param name="word">The word to get the meaning of.</param>
        /// <returns>The thesaurus result.</returns>
        public ThesaurusResult Thesaurus(string word)
        {
            if (_thesaurus != null)
            {
                // Get the meaning.
                ThesResult result = _thesaurus.Lookup(word, _hunspell);
                List<ThesaurusMeaning> meanings = new List<ThesaurusMeaning>();

                // For each meaning found.
                foreach (ThesMeaning meaning in result.Meanings)
                {
                    // Add the meaning.
                    meanings.Add(new ThesaurusMeaning(meaning.Description, meaning.Synonyms));
                }

                // Create the result.
                ThesaurusResult theResult = new ThesaurusResult(meanings, result.IsGenerated);

                // Return the result.
                return theResult;
            }
            else
                throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            // uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    if (_hyphen != null)
                        _hyphen.Dispose();

                    if (_hunspell != null)
                        _hunspell.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                _hyphen = null;
                _hunspell = null;
                _thesaurus = null;
                _disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Portugal()
        {
            // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }
        #endregion
    }
}
