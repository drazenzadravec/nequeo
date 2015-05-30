/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
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
using System.IO;
using System.Text.RegularExpressions;

namespace Nequeo.IO.Stream.Spam
{
    /// <summary>
    /// Contains the list of text for controlling words that are and are not spam.
    /// </summary>
    public class TextControlList
    {
        /// <summary>
		/// Public constructor.
		/// </summary>
		public TextControlList()
		{
		}

		/// <summary>
		/// Populate it with text from the supplied reader
		/// </summary>
		/// <param name="reader"></param>
		public TextControlList(TextReader reader)
		{
			LoadFromReader(reader);
		}

		/// <summary>
		/// Populate it with the contents of the supplied file
		/// </summary>
		/// <param name="filepath"></param>
        public TextControlList(string filepath)
		{
			LoadFromFile(filepath);
		}

        /// <summary>
        /// Regex pattern for words that don't start with a number
        /// </summary>
        public const string TokenPattern = @"([a-zA-Z]\w+)\W*";
        private SortedDictionary<string, int> _tokens = new SortedDictionary<string, int>();

        /// <summary>
        /// Gets a sorted list of all the words that show up in the text, along with counts of how many times they appear.
        /// </summary>
        public SortedDictionary<string, int> Tokens
        {
            get { return _tokens; }
        }

        /// <summary>
        /// Populate with text from a file.
        /// </summary>
        /// <param name="filepath">The file path</param>
        public void LoadFromFile(string filepath)
        {
            LoadFromReader(new StreamReader(filepath));
        }

        /// <summary>
        /// Loads words from the specified TextReader into the collection.
        /// Doesn't initialize the collection, so it can be called from
        /// a loop if needed.
        /// </summary>
        /// <param name="reader">The text reader used to load the words</param>
        public void LoadFromReader(TextReader reader)
        {
            Regex re = new Regex(TokenPattern, RegexOptions.Compiled);
            string line;
            while (null != (line = reader.ReadLine()))
            {
                Match m = re.Match(line);
                while (m.Success)
                {
                    string token = m.Groups[1].Value;
                    AddToken(token);
                    m = m.NextMatch();
                }
            }
        }

        /// <summary>
        /// Stick a word into the list, incrementing its count if it's already there.
        /// </summary>
        /// <param name="rawPhrase">The phrase or word to add.</param>
        public void AddToken(string rawPhrase)
        {
            if (!_tokens.ContainsKey(rawPhrase))
                _tokens.Add(rawPhrase, 1);
            else
                _tokens[rawPhrase]++;
        }
    }
}
