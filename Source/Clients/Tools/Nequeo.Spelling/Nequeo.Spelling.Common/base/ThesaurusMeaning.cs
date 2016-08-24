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
    /// Holds a meaning and its synonyms.
    /// </summary>
    public class ThesaurusMeaning
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThesaurusMeaning"/> class.
        /// </summary>
        /// <param name="description">
        /// The meaning description. 
        /// </param>
        /// <param name="synonyms">
        /// The synonyms for this meaning. 
        /// </param>
        public ThesaurusMeaning(string description, List<string> synonyms)
        {
            _description = description;
            _synonyms = synonyms;
        }

        /// <summary>
        ///   The description.
        /// </summary>
        private readonly string _description;

        /// <summary>
        ///   The synonyms.
        /// </summary>
        private readonly List<string> _synonyms;

        /// <summary>
        /// Gets the description of the meaning.
        /// </summary>
        /// <value> The description. </value>
        public string Description
        {
            get
            {
                return _description;
            }
        }

        /// <summary>
        /// Gets the synonyms of the meaning.
        /// </summary>
        /// <value> The synonyms. </value>
        public List<string> Synonyms
        {
            get
            {
                return _synonyms;
            }
        }
    }
}
