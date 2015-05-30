/*  Company :       Nequeo Pty Ltd, http://www.nequeo.net.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2013 http://www.nequeo.net.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.IO.Compression.Huffman
{
    /// <summary>
    /// Huffman compression processor algorithm.
    /// </summary>
    public class Processor
    {
        /// <summary>
        /// Huffman compression processor algorithm.
        /// </summary>
        public Processor()
        {
            _table = new CodesTable();
            _tree = new BitTree(_table);
        }

        private BitTree _tree;
        private CodesTable _table;

        /// <summary>
        /// Compress the data.
        /// </summary>
        /// <param name="data">The data to compress.</param>
        /// <returns>The compressed data.</returns>
        public byte[] Compress(byte[] data)
        {
            var huffmanEncodedMessage = new List<bool>();

            foreach (var bt in data)
            {
                huffmanEncodedMessage.AddRange(_table.GetBits(bt));
            }

            // Adds most significant bytes of EOS
            int temp = 8 - huffmanEncodedMessage.Count % 8;
            int numberOfBitsInPadding = temp == 8 ? 0 : temp;

            for (int i = 0; i < numberOfBitsInPadding; i++)
            {
                huffmanEncodedMessage.Add(CodesTable.Eos[i]);
            }

            return BinaryConverter.ToBytes(huffmanEncodedMessage);
        }

        /// <summary>
        /// Decompress the data.
        /// </summary>
        /// <param name="compressed">The compressed data.</param>
        /// <returns>The decompressed data.</returns>
        public byte[] Decompress(byte[] compressed)
        {
            var bits = BinaryConverter.ToBits(compressed);
            return _tree.GetBytes(bits);
        }
    }
}
