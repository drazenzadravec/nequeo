/*  Company :       Nequeo Pty Ltd, http://www.nequeo.net.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2013 http://www.nequeo.net.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.IO.Compression.Huffman
{
    /// <summary>
    /// The btree.
    /// </summary>
    internal class BitTree
    {
        /// <summary>
        /// The btree.
        /// </summary>
        /// <param name="table"></param>
        public BitTree(CodesTable table)
        {
            _table = table;
            _eos = CodesTable.Eos;
            _root = new Node(false, null);
            BuildTree(table);
        }

        private Node _root;
        private CodesTable _table;
        private readonly bool[] _eos;

        /// <summary>
        /// Build the tree.
        /// </summary>
        /// <param name="table">The code table.</param>
        private void BuildTree(CodesTable table)
        {
            foreach (var bits in table.HuffmanTable.Keys)
            {
                Add(bits);
            }

            Add(CodesTable.Eos);
        }

        /// <summary>
        /// Add the bits.
        /// </summary>
        /// <param name="bits">The bits.</param>
        private void Add(bool[] bits)
        {
            if (bits == null)
                throw new ArgumentNullException("bits is null");

            Node temp = _root;

            for (int i = 0; i < bits.Length; i++)
            {
                bool bit = bits[i];
                if (!bit)
                {
                    if (temp.Left == null)
                        temp.Left = new Node(false, temp);

                    temp = temp.Left;
                }
                else
                {
                    if (temp.Right == null)
                        temp.Right = new Node(true, temp);

                    temp = temp.Right;
                }
            }
        }

        /// <summary>
        /// Get the bytes.
        /// </summary>
        /// <param name="bits">The bits.</param>
        /// <returns>The bytes.</returns>
        public byte[] GetBytes(bool[] bits)
        {
            if (bits == null)
                throw new ArgumentNullException("bits is null");

            byte[] result = null;
            using (var stream = new MemoryStream())
            {
                int i = 0;

                while (i < bits.Length)
                {
                    Node temp = _root;
                    var symbolBits = new List<bool>();

                    bool isEos = true;

                    int j = 0;
                    while (i < bits.Length)
                    {
                        temp = !bits[i] ? temp.Left : temp.Right;

                        if (temp == null)
                            break;

                        symbolBits.Add(temp.Value);
                        isEos &= temp.Value == _eos[j];

                        if (isEos && ++j == _eos.Length)
                        {
                            // see spec 07 - > 4.1.2.  String Literal Representation
                            // A Huffman encoded string literal containing the EOS entry
                            // MUST be treated as a decoding error.
                            throw new Exception("EOS contains");
                        }

                        i++;
                    }

                    if (IsValidPadding(symbolBits))
                        break;

                    // See spec 07 -> 4.1.2.  String Literal Representation
                    // A padding strictly longer than 7 bits MUST be treated as a decoding error.
                    // A padding not corresponding to the most significant bits of the EOS
                    // entry MUST be treated as a decoding error.

                    // If padding is not valid or padding is longer than 7 bits
                    // then decoding error will thrown by GetByte method 
                    // since not turn recognize the symbol.
                    var symbol = _table.GetByte(symbolBits);
                    stream.WriteByte(symbol);
                }

                result = new byte[stream.Position];
                Buffer.BlockCopy(stream.GetBuffer(), 0, result, 0, result.Length);
                return result;
            }
        }

        // See spec 07 -> 4.1.2.  String Literal Representation
        // As the Huffman encoded data doesn't always end at an octet boundary,
        // some padding is inserted after it up to the next octet boundary.  To
        // prevent this padding to be misinterpreted as part of the string
        // literal, the most significant bits of the EOS (end-of-string) entry
        // in the Huffman table are used.
        private bool IsValidPadding(List<bool> symbolBits)
        {
            if (symbolBits.Count >= 8)
            {
                return false;
            }

            for (int i = 0; i < symbolBits.Count; i++)
            {
                if (symbolBits[i] != CodesTable.Eos[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Node class.
        /// </summary>
        private class Node
        {
            /// <summary>
            /// Gets the value.
            /// </summary>
            public bool Value { get; private set; }

            /// <summary>
            /// Gets or sets the left.
            /// </summary>
            public Node Left { get; set; }

            /// <summary>
            /// Gets or sets the right.
            /// </summary>
            public Node Right { get; set; }

            /// <summary>
            /// Gets the parent.
            /// </summary>
            public Node Parent { get; private set; }

            /// <summary>
            /// Node.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="parent">The parent.</param>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            public Node(bool value, Node parent, Node left = null, Node right = null)
            {
                Value = value;
                Left = left;
                Right = right;
                Parent = parent;
            }
        }
    }
}
