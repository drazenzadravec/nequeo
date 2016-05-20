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
    /// Computes the <see cref="T:Nequeo.Cryptography.Sha3.SHA3" /> hash for the input data.
	/// </summary>
    [ComVisible(true)]
    public abstract class SHA3 : HashAlgorithm
    {
        /// <summary>
        /// Computes the <see cref="T:Nequeo.Cryptography.Sha3.SHA3" /> hash for the input data.
        /// </summary>
        static SHA3()
        {
            CryptoConfig.AddAlgorithm(typeof(SHA3Managed), "SHA3", "SHA3Managed", "SHA-3", "Nequeo.Cryptography.Sha3.SHA3");
            CryptoConfig.AddOID("0.9.2.0", "SHA3", "SHA3Managed", "SHA-3", "Nequeo.Cryptography.Sha3.SHA3");
        }

        /// <summary>
        /// Initializes a new instance of <see cref="T:Nequeo.Cryptography.Sha3.SHA3" />.
        /// </summary>
        protected SHA3()
        {
            base.HashSizeValue = 512;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="T:Nequeo.Cryptography.Sha3.SHA3" />.
        /// </summary>
        /// <param name="hashBitLength">The hash bit length (224, 256, 384, 512).</param>
        protected SHA3(int hashBitLength)
        {
            if (hashBitLength != 224 && hashBitLength != 256 && hashBitLength != 384 && hashBitLength != 512)
                throw new ArgumentException("hashBitLength must be 224, 256, 384, or 512", "hashBitLength");
            Initialize();
            HashSizeValue = hashBitLength;
            switch (hashBitLength)
            {
                case 224:
                    KeccakR = 1152;
                    break;
                case 256:
                    KeccakR = 1088;
                    break;
                case 384:
                    KeccakR = 832;
                    break;
                case 512:
                    KeccakR = 576;
                    break;
            }
            RoundConstants = new ulong[]
            {
                0x0000000000000001UL,
                0x0000000000008082UL,
                0x800000000000808aUL,
                0x8000000080008000UL,
                0x000000000000808bUL,
                0x0000000080000001UL,
                0x8000000080008081UL,
                0x8000000000008009UL,
                0x000000000000008aUL,
                0x0000000000000088UL,
                0x0000000080008009UL,
                0x000000008000000aUL,
                0x000000008000808bUL,
                0x800000000000008bUL,
                0x8000000000008089UL,
                0x8000000000008003UL,
                0x8000000000008002UL,
                0x8000000000000080UL,
                0x000000000000800aUL,
                0x800000008000000aUL,
                0x8000000080008081UL,
                0x8000000000008080UL,
                0x0000000080000001UL,
                0x8000000080008008UL
            };
        }

        /// <summary>Creates an instance of the default implementation of <see cref="T:Nequeo.Cryptography.Sha3.SHA3" />.</summary>
        /// <returns>A new instance of <see cref="T:Nequeo.Cryptography.Sha3.SHA3" />.</returns>
        /// <exception cref="T:System.Reflection.TargetInvocationException">The algorithm was used with Federal Information Processing Standards (FIPS) mode enabled, but is not FIPS compatible.</exception>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        /// </PermissionSet>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public static new SHA3 Create()
        {
            return Create("Nequeo.Cryptography.Sha3.SHA3");
        }

        /// <summary>Creates an instance of a specified implementation of <see cref="T:Nequeo.Cryptography.Sha3.SHA3" />.</summary>
        /// <returns>A new instance of <see cref="T:Nequeo.Cryptography.Sha3.SHA3" /> using the specified implementation.</returns>
        /// <param name="hashName">The name of the specific implementation of <see cref="T:Nequeo.Cryptography.Sha3.SHA3" /> to be used. </param>
        /// <exception cref="T:System.Reflection.TargetInvocationException">The algorithm described by the <paramref name="hashName" /> parameter was used with Federal Information Processing Standards (FIPS) mode enabled, but is not FIPS compatible.</exception>
        public static new SHA3 Create(string hashName)
        {
            return (SHA3)CryptoConfig.CreateFromName(hashName);
        }

        #region Hash Algorithm Members
        /// <summary>
        /// Hash core.
        /// </summary>
        /// <param name="array">The array of bytes.</param>
        /// <param name="ibStart">The start bit.</param>
        /// <param name="cbSize">The end bit.</param>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (ibStart < 0)
                throw new ArgumentOutOfRangeException("ibStart");
            if (cbSize > array.Length)
                throw new ArgumentOutOfRangeException("cbSize");
            if (ibStart + cbSize > array.Length)
                throw new ArgumentOutOfRangeException("ibStart or cbSize");
        }

        /// <summary>
        /// Get the hash final array.
        /// </summary>
        /// <returns>The hash final array.</returns>
        protected override byte[] HashFinal()
        {
            return this.Hash;
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public override void Initialize()
        {
            buffLength = 0;
            state = new ulong[5 * 5];//1600 bits
            HashValue = null;
        }
        #endregion

        #region Implementation
        internal const int KeccakB = 1600;
        internal const int KeccakNumberOfRounds = 24;
        internal const int KeccakLaneSizeInBits = 8 * 8;
        internal readonly ulong[] RoundConstants;
        internal ulong[] state;
        internal byte[] buffer;
        internal int buffLength;

        //protected new byte[] HashValue;
        //protected new int HashSizeValue;
        internal int keccakR;

        /// <summary>
        /// Gets or sets the KR value.
        /// </summary>
        internal int KeccakR
        {
            get
            {
                return keccakR;
            }
            set
            {
                keccakR = value;
            }
        }

        /// <summary>
        /// Gets the byte size.
        /// </summary>
        public int SizeInBytes
        {
            get
            {
                return KeccakR / 8;
            }
        }

        /// <summary>
        /// Gets the hash byte length.
        /// </summary>
        public int HashByteLength
        {
            get
            {
                return HashSizeValue / 8;
            }
        }

        /// <summary>
        /// Gets the can reuse transform.
        /// </summary>
        public override bool CanReuseTransform
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// ROL
        /// </summary>
        /// <param name="a">A value.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The ROL value.</returns>
        protected ulong ROL(ulong a, int offset)
        {
            return (((a) << ((offset) % KeccakLaneSizeInBits)) ^ ((a) >> (KeccakLaneSizeInBits - ((offset) % KeccakLaneSizeInBits))));
        }

        /// <summary>
        /// Add to buffer.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        protected void AddToBuffer(byte[] array, ref int offset, ref int count)
        {
            int amount = Math.Min(count, buffer.Length - buffLength);
            Buffer.BlockCopy(array, offset, buffer, buffLength, amount);
            offset += amount;
            buffLength += amount;
            count -= amount;
        }

        /// <summary>
        /// Gets the hash array.
        /// </summary>
        public override byte[] Hash
        {
            get
            {
                return HashValue;
            }
        }

        /// <summary>
        /// Gets the hash size.
        /// </summary>
        public override int HashSize
        {
            get
            {
                return HashSizeValue;
            }
        }
        #endregion

    }
}
