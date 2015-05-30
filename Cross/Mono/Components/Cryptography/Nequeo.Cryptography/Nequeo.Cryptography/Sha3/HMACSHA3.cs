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
using System.Threading;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using System.ComponentModel.Composition;

namespace Nequeo.Cryptography.Sha3
{
    /// <summary>
    /// Represents the abstract class from which all implementations of Hash-based
    /// Message Authentication Code (HMAC) must derive.
    /// </summary>
    [ComVisible(true)]
    public class HMACSHA3 : HMAC
    {

        /// <summary>
        /// Represents the abstract class from which all implementations of Hash-based
        /// Message Authentication Code (HMAC) must derive.
        /// </summary>
        static HMACSHA3()
        {
            SHA3 sha = SHA3.Create();
        }

        /// <summary>
        /// Represents the abstract class from which all implementations of Hash-based
        /// Message Authentication Code (HMAC) must derive.
        /// </summary>
        /// <param name="hashBitLength">The hash bit length.</param>
        public HMACSHA3(int hashBitLength = 512) : this(RandomNumber.Generate(0x80), hashBitLength) { }

        /// <summary>
        /// Represents the abstract class from which all implementations of Hash-based
        /// Message Authentication Code (HMAC) must derive.
        /// </summary>
        /// <param name="key">The key array.</param>
        /// <param name="hashBitLength">The hash bit length.</param>
        public HMACSHA3(byte[] key, int hashBitLength = 512)
        {
            base.HashName = "SHA3Managed";
            SetHashBitLength(hashBitLength);
            SetHMACBlockSize();
            Initialize();
            base.Key = (byte[])key.Clone();
        }

        /// <summary>
        /// Set the block size.
        /// </summary>
        private void SetHMACBlockSize()
        {
            switch (base.HashSizeValue)
            {
                case 224:
                    base.BlockSizeValue = 144;
                    break;
                case 256:
                    base.BlockSizeValue = 136;
                    break;
                case 384:
                    base.BlockSizeValue = 104;
                    break;
                case 512:
                    base.BlockSizeValue = 72;
                    break;
            }
        }

        /// <summary>
        /// Set the hash bit length.
        /// </summary>
        /// <param name="hashBitLength">The hash bit length.</param>
        private void SetHashBitLength(int hashBitLength)
        {
            if (hashBitLength != 512) throw new NotImplementedException("HMAC-SHA3 is only implemented for 512bits hashes.");
            if (hashBitLength != 224 && hashBitLength != 256 && hashBitLength != 384 && hashBitLength != 512)
                throw new ArgumentException("Hash bit length must be 224, 256, 384, or 512", "hashBitLength");
            base.HashSizeValue = hashBitLength;
        }
    }
}
