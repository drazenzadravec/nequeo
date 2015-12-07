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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lucene.Net;
using Lucene.Net.Analysis;
using Lucene.Net.Util;
using Lucene.Net.Util.Automaton;
using Lucene.Net.Analysis.Tokenattributes;

namespace Nequeo.Search.Engine.Analyzer
{
    /// <summary>
    /// Simple unicode term attribute.
    /// </summary>
    internal class SimpleUnicodeTermAttribute : CharTermAttribute, IOffsetAttribute, IPositionIncrementAttribute
    {
        private int _startOffset;
        private int _endOffset;
        private int _positionIncrement = 1;
        private readonly System.Text.Encoding _charset = System.Text.Encoding.Unicode;

        /// <summary>
        /// Override fill bytes by refernce.
        /// </summary>
        public override void FillBytesRef()
        {
            BytesRef bytes = BytesRef;
            var utf16 = _charset.GetBytes(_charset.ToString());
            bytes.Bytes = utf16;
            bytes.Offset = 0;
            bytes.Length = utf16.Length;
        }

        /// <summary>
        /// Get the end offset.
        /// </summary>
        /// <returns>Return the end offset.</returns>
        public int EndOffset()
        {
            return _endOffset;
        }

        /// <summary>
        /// Set the offset.
        /// </summary>
        /// <param name="startOffset">Start offset.</param>
        /// <param name="endOffset">End offset.</param>
        public void SetOffset(int startOffset, int endOffset)
        {
            if (startOffset < 0 || endOffset < startOffset)
            {
                throw new System.ArgumentException("startOffset must be non-negative, and endOffset must be >= startOffset, " + "startOffset=" + startOffset + ",endOffset=" + endOffset);
            }

            _startOffset = startOffset;
            _endOffset = endOffset;
        }

        /// <summary>
        /// Get the start offset.
        /// </summary>
        /// <returns>Return the start offset.</returns>
        public int StartOffset()
        {
            return _startOffset;
        }

        /// <summary>
        /// Reset the offset.
        /// </summary>
        public override void Clear()
        {
            _startOffset = 0;
            _endOffset = 0;
            _positionIncrement = 1;
        }

        /// <summary>
        /// Eqauls override.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(object other)
        {
            if (other == this)
            {
                return true;
            }

            if (other is SimpleUnicodeTermAttribute)
            {
                SimpleUnicodeTermAttribute o = (SimpleUnicodeTermAttribute)other;
                return o._startOffset == _startOffset && o._endOffset == _endOffset;
            }

            return false;
        }

        /// <summary>
        /// Get the hash code.
        /// </summary>
        /// <returns>The code.</returns>
        public override int GetHashCode()
        {
            int code = _startOffset;
            code = code * 31 + _endOffset;
            return code;
        }

        /// <summary>
        /// Copy the offset.
        /// </summary>
        /// <param name="target">The target.</param>
        public override void CopyTo(Lucene.Net.Util.Attribute target)
        {
            SimpleUnicodeTermAttribute t = (SimpleUnicodeTermAttribute)target;
            t.SetOffset(_startOffset, _endOffset);
        }

        /// <summary>
        /// Gets or sets the position incrememt.
        /// </summary>
        public int PositionIncrement
        {
            set
            {
                if (value < 0)
                {
                    throw new System.ArgumentException("Increment must be zero or greater: got " + value);
                }
                _positionIncrement = value;
            }
            get
            {
                return _positionIncrement;
            }
        }
    }
}
