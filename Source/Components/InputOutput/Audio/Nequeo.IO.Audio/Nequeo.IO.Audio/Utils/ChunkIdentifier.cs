/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.IO.Audio.Utils
{
    /// <summary>
    /// Chunk Identifier helpers
    /// </summary>
    internal class ChunkIdentifier
    {
        /// <summary>
        /// Chunk identifier to Int32 (replaces mmioStringToFOURCC)
        /// </summary>
        /// <param name="s">four character chunk identifier</param>
        /// <returns>Chunk identifier as int 32</returns>
        public static int ChunkIdentifierToInt32(string s)
        {
            if (s.Length != 4) throw new ArgumentException("Must be a four character string");
            var bytes = Encoding.UTF8.GetBytes(s);
            if (bytes.Length != 4) throw new ArgumentException("Must encode to exactly four bytes");
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// Chunck identifier to string
        /// </summary>
        /// <param name="s">The Int32 identifier.</param>
        /// <returns>Chunk identifier as string</returns>
        public static string ChunkIdentifierToString(int s)
        {
            byte[] bytes = BitConverter.GetBytes(s);
            return BitConverter.ToString(bytes);
        }
    }
}
