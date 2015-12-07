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
using Lucene.Net.Support;
using Lucene.Net.Analysis;
using Lucene.Net.Util;
using Lucene.Net.Util.Automaton;
using Lucene.Net.Analysis.Tokenattributes;

namespace Nequeo.Search.Engine.Analyzer
{
    /// <summary>
    /// Read state.
    /// </summary>
    internal enum ReadState
    {
        SETREADER, // consumer set a reader input either via ctor or via reset(Reader)
        RESET, // consumer has called reset()
        INCREMENT, // consumer is consuming, has called IncrementToken() == true
        INCREMENT_FALSE, // consumer has called IncrementToken() which returned false
        END, // consumer has called end() to perform end of stream operations
        CLOSE // consumer has called close() to release any resources
    }

    /// <summary>
    /// Simple tokenizer.
    /// </summary>
    internal class SimpleTokenizer : Tokenizer
    {
        /// <summary>
        /// Simple tokenizer.
        /// </summary>
        /// <param name="input">The current text reader stream.</param>
        public SimpleTokenizer(TextReader input)
            : this(AttributeFactory.DEFAULT_ATTRIBUTE_FACTORY, input)
        {
        }

        /// <summary>
        /// Simple tokenizer.
        /// </summary>
        /// <param name="input">The current text reader stream.</param>
        /// <param name="factory">Set the attribute factory.</param>
        public SimpleTokenizer(AttributeSource.AttributeFactory factory, TextReader input)
            : base(factory, input)
        {
            this.RunAutomaton = WHITESPACE;
            this.LowerCase = true;
            this.state = this.RunAutomaton.InitialState;
            this.StreamState = ReadState.SETREADER;
            this.MaxTokenLength = DEFAULT_MAX_TOKEN_LENGTH;
            TermAtt = AddAttribute<ICharTermAttribute>();
            OffsetAtt = AddAttribute<IOffsetAttribute>();
        }

        /// <summary>
        /// Acts Similar to WhitespaceTokenizer </summary>
        public static readonly CharacterRunAutomaton WHITESPACE = new CharacterRunAutomaton(new RegExp("[^ \t\r\n]+").ToAutomaton());

        /// <summary>
        /// Acts Similar to KeywordTokenizer.
        /// TODO: Keyword returns an "empty" token for an empty reader...
        /// </summary>
        public static readonly CharacterRunAutomaton KEYWORD = new CharacterRunAutomaton(new RegExp(".*").ToAutomaton());

        /// <summary>
        /// Acts like LetterTokenizer. </summary>
        // the ugly regex below is incomplete Unicode 5.2 [:Letter:]
        public static readonly CharacterRunAutomaton SIMPLE = new CharacterRunAutomaton(new RegExp("[A-Za-zªµºÀ-ÖØ-öø-ˁ一-鿌]+").ToAutomaton());

        private readonly CharacterRunAutomaton RunAutomaton;
        private readonly bool LowerCase;
        private readonly int MaxTokenLength;
        public static readonly int DEFAULT_MAX_TOKEN_LENGTH = int.MaxValue;
        private int state;

        private readonly ICharTermAttribute TermAtt;
        private readonly IOffsetAttribute OffsetAtt;
        internal int Off = 0;

        // buffered state (previous codepoint and offset). we replay this once we
        // hit a reject state in case its permissible as the start of a new term.
        internal int BufferedCodePoint = -1; // -1 indicates empty buffer

        internal int BufferedOff = -1;

        private ReadState StreamState = ReadState.CLOSE;
        private int LastOffset = 0; // only for asserting
        private bool EnableChecks_Renamed = true;

        // evil: but we don't change the behavior with this random, we only switch up how we read
        private readonly Random Random = new Random(/*RandomizedContext.Current.Random.nextLong()*/);


        //private int offset = 0, bufferIndex = 0, dataLen = 0;
        private const int MAX_WORD_LEN = 255;
        private const int IO_BUFFER_SIZE = 4096;
        private readonly char[] ioBuffer = new char[IO_BUFFER_SIZE];


        /// <summary>
        /// Consumers use this method to advance the stream to
        /// the next token. Implementing classes must implement this method and update
        /// the appropriate with the attributes of the next
        /// token.
        /// The producer must make no assumptions about the attributes after the method
        /// has been returned: the caller may arbitrarily change it. If the producer
        /// needs to preserve the state for subsequent calls, it can use
        /// to create a copy of the current attribute state
        /// this method is called for every token of a document, so an efficient
        /// implementation is crucial for good performance. To avoid calls to,
        /// references to all that this stream uses should be
        /// retrieved during instantiation.
        /// To ensure that filters and consumers know which attributes are available,
        /// the attributes must be added during instantiation. Filters and consumers
        /// are not required to check for availability of attributes.
        /// </summary>
        /// <returns> false for end of stream; true otherwise </returns>
        public override bool IncrementToken()
        {
            if (base.input != null)
            {
                if (base.input.Peek() > -1)
                {
                    //ClearAttributes();
                    //int length = 0;
                    //int start = bufferIndex;
                    //char[] buffer = TermAtt.Buffer();
                    //while (true)
                    //{

                    //    if (bufferIndex >= dataLen)
                    //    {
                    //        offset += dataLen;
                    //        dataLen = input.Read(ioBuffer, 0, ioBuffer.Length);
                    //        if (dataLen <= 0)
                    //        {
                    //            dataLen = 0; // so next offset += dataLen won't decrement offset
                    //            if (length > 0)
                    //                break;
                    //            return false;
                    //        }
                    //        bufferIndex = 0;
                    //    }

                    //    char c = ioBuffer[bufferIndex++];

                    //    if (IsTokenChar(c))
                    //    {
                    //        // if it's a token char

                    //        if (length == 0)
                    //            // start of token
                    //            start = offset + bufferIndex - 1;
                    //        else if (length == buffer.Length)
                    //            buffer = TermAtt.ResizeBuffer(1 + length);

                    //        buffer[length++] = Normalize(c); // buffer it, normalized

                    //        if (length == MAX_WORD_LEN)
                    //            // buffer overflow!
                    //            break;
                    //    }
                    //    else if (length > 0)
                    //        // at non-Letter w/ chars
                    //        break; // return 'em
                    //}

                    //TermAtt.Length = length;
                    //OffsetAtt.SetOffset(CorrectOffset(start), CorrectOffset(start + length));
                    //return true;

                    ClearAttributes();
                    for (; ; )
                    {
                        int startOffset;
                        int cp;
                        if (BufferedCodePoint >= 0)
                        {
                            cp = BufferedCodePoint;
                            startOffset = BufferedOff;
                            BufferedCodePoint = -1;
                        }
                        else
                        {
                            startOffset = Off;
                            cp = ReadCodePoint();
                        }
                        if (cp < 0)
                        {
                            break;
                        }
                        else if (IsTokenChar(cp))
                        {
                            int endOffset;
                            do
                            {
                                char[] chars = Character.ToChars(Normalize(cp));
                                for (int i = 0; i < chars.Length; i++)
                                {
                                    TermAtt.Append(chars[i]);
                                }
                                endOffset = Off;
                                if (TermAtt.Length >= MaxTokenLength)
                                {
                                    break;
                                }
                                cp = ReadCodePoint();
                            } while (cp >= 0 && IsTokenChar(cp));

                            if (TermAtt.Length < MaxTokenLength)
                            {
                                // buffer up, in case the "rejected" char can start a new word of its own
                                BufferedCodePoint = cp;
                                BufferedOff = endOffset;
                            }
                            else
                            {
                                // otherwise, its because we hit term limit.
                                BufferedCodePoint = -1;
                            }
                            int correctedStartOffset = CorrectOffset(startOffset);
                            int correctedEndOffset = CorrectOffset(endOffset);
                            LastOffset = correctedStartOffset;
                            OffsetAtt.SetOffset(correctedStartOffset, correctedEndOffset);
                            if (state == -1 || RunAutomaton.IsAccept(state))
                            {
                                // either we hit a reject state (longest match), or end-of-text, but in an accept state
                                StreamState = ReadState.INCREMENT;
                                return true;
                            }
                        }
                    }
                    StreamState = ReadState.INCREMENT_FALSE;
                    return false;
                }
                else
                {
                    StreamState = ReadState.INCREMENT_FALSE;
                    return false;
                }
            }
            else
            {
                StreamState = ReadState.INCREMENT_FALSE;
                return false;
            }
        }

        /// <summary>
        /// Read code point.
        /// </summary>
        /// <returns>The code point.</returns>
        protected internal virtual int ReadCodePoint()
        {
            int ch = ReadChar();
            if (ch < 0)
            {
                return ch;
            }
            else
            {
                Off++;
                if (char.IsHighSurrogate((char)ch))
                {
                    int ch2 = ReadChar();
                    if (ch2 >= 0)
                    {
                        Off++;
                        return Character.ToCodePoint((char)ch, (char)ch2);
                    }
                }
                return ch;
            }
        }

        /// <summary>
        /// Read character.
        /// </summary>
        /// <returns>The character read.</returns>
        protected internal virtual int ReadChar()
        {
            switch (Random.Next(0, 10))
            {
                case 0:
                    {
                        // read(char[])
                        char[] c = new char[1];
                        int ret = input.Read(c, 0, c.Length);
                        return ret <= 0 ? -1 : c[0];
                    }
                case 1:
                    {
                        // read(char[], int, int)
                        char[] c = new char[2];
                        int ret = input.Read(c, 1, 1);
                        return ret <= 0 ? -1 : c[1];
                    }
                /* LUCENE TO-DO not sure if needed, CharBuffer not supported
                  case 2:
                  {
                    // read(CharBuffer)
                    char[] c = new char[1];
                    CharBuffer cb = CharBuffer.Wrap(c);
                    int ret = Input.Read(cb);
                    return ret < 0 ? ret : c[0];
                  }*/
                default:
                    // read()
                    return input.Read();
            }
        }

        /// <summary>
        /// Is token character.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>True if is token character.</returns>
        protected internal virtual bool IsTokenChar(int c)
        {
            if (state < 0)
            {
                state = RunAutomaton.InitialState;
            }
            state = RunAutomaton.Step(state, c);
            if (state < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Normalise the character.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>Normalised character.</returns>
        protected internal virtual int Normalize(int c)
        {
            return LowerCase ? Character.ToLowerCase(c) : c;
        }

        /// <summary>
        /// Normalise the character.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>Normalised character.</returns>
        protected internal virtual char Normalize(char c)
        {
            return Convert.ToChar(Normalize(Convert.ToInt32(c)));
        }

        /// <summary>
        /// Reset all values.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            state = RunAutomaton.InitialState;
            LastOffset = Off = 0;
            BufferedCodePoint = -1;
            StreamState = ReadState.RESET;
        }

        /// <summary>
        /// Dispose of the object.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            StreamState = ReadState.CLOSE;
        }

        /// <summary>
        /// Set the reader test point.
        /// </summary>
        /// <returns>True if set.</returns>
        internal bool SetReaderTestPoint()
        {
            StreamState = ReadState.SETREADER;
            return true;
        }

        /// <summary>
        /// End the read operation.
        /// </summary>
        public override void End()
        {
            base.End();
            int finalOffset = CorrectOffset(Off);
            OffsetAtt.SetOffset(finalOffset, finalOffset);
            StreamState = ReadState.END;

            // Do nothin for now.
            if(StreamState == ReadState.END)
            { }
        }

        /// <summary>
        /// Toggle consumer workflow checking: if your test consumes tokenstreams normally you
        /// should leave this enabled.
        /// </summary>
        public virtual bool EnableChecks
        {
            set { this.EnableChecks_Renamed = value; }
        }
    }
}
