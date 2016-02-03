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

namespace Nequeo.Html
{
    /// <summary>
    /// Represents a parsing error found during document parsing.
    /// </summary>
    public class HtmlParseError
    {
        #region Fields

        private HtmlParseErrorCode _code;
        private int _line;
        private int _linePosition;
        private string _reason;
        private string _sourceText;
        private int _streamPosition;

        #endregion

        #region Constructors

        internal HtmlParseError(
            HtmlParseErrorCode code,
            int line,
            int linePosition,
            int streamPosition,
            string sourceText,
            string reason)
        {
            _code = code;
            _line = line;
            _linePosition = linePosition;
            _streamPosition = streamPosition;
            _sourceText = sourceText;
            _reason = reason;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the type of error.
        /// </summary>
        public HtmlParseErrorCode Code
        {
            get { return _code; }
        }

        /// <summary>
        /// Gets the line number of this error in the document.
        /// </summary>
        public int Line
        {
            get { return _line; }
        }

        /// <summary>
        /// Gets the column number of this error in the document.
        /// </summary>
        public int LinePosition
        {
            get { return _linePosition; }
        }

        /// <summary>
        /// Gets a description for the error.
        /// </summary>
        public string Reason
        {
            get { return _reason; }
        }

        /// <summary>
        /// Gets the the full text of the line containing the error.
        /// </summary>
        public string SourceText
        {
            get { return _sourceText; }
        }

        /// <summary>
        /// Gets the absolute stream position of this error in the document, relative to the start of the document.
        /// </summary>
        public int StreamPosition
        {
            get { return _streamPosition; }
        }

        #endregion
    }
}