/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          HuffmanCode.h
*  Purpose :       HuffmanCode class.
*
*/

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

#pragma once

#ifndef _HUFFMANCODE_H
#define _HUFFMANCODE_H

#include "GlobalIOCompression.h"
#include "HuffmanNode.h"

namespace Nequeo {
	namespace IO {
		namespace Compression
		{
			/// <summary>
			/// Huffman codes.
			/// </summary>
			class HuffmanCode
			{
			public:
				/// <summary>
				/// Huffman codes.
				/// </summary>
				HuffmanCode();
				~HuffmanCode();

				typedef std::vector<bool> HuffCode;
				typedef std::map<unsigned char, HuffCode> HuffCodeMap;

				/// <summary>
				/// Gets or sets the code map for headers used in http enconding.
				/// </summary>
				HuffCodeMap getCode();
				void setCode(HuffCodeMap bitMap);

				/// <summary>
				/// Gets or sets the end of string code for headers used in http enconding.
				/// </summary>
				HuffCode getEOS();
				void setEOS(HuffCode code);

				/// <summary>
				/// Gets the size of the header code.
				/// </summary>
				int getSize();

				/// <summary>
				/// Gets the code map.
				/// </summary>
				/// <param name="value">The value to generate a code map for.</param>
				/// <returns>The code map.</returns>
				HuffCodeMap GetCode(const unsigned char* value);
				
				/// <summary>
				/// Gets the byte.
				/// </summary>
				/// <param name="bits">The bit array.</param>
				/// <returns>The byte.</returns>
				unsigned char GetByte(std::vector<bool> bits);

				/// <summary>
				/// Gets the bits.
				/// </summary>
				/// <param name="c">The byte.</param>
				/// <returns>The bit collection.</returns>
				std::vector<bool> GetBits(unsigned char c);

				/// <summary>
				/// As the Huffman encoded data doesn't always end at an octet boundary,
				/// some padding is inserted after it up to the next octet boundary. To
				/// prevent this padding to be misinterpreted as part of the string
				/// literal, the most significant bits of the EOS (end-of-string) entry
				/// in the Huffman table are used.
				/// </summary>
				/// <param name="bits">The bit array.</param>
				/// <returns>True if valid padding; else false.</returns>
				bool IsValidPadding(std::vector<bool> bits);

				/// <summary>
				/// Decompress the data.
				/// </summary>
				/// <param name="data">The compressed data.</param>
				/// <returns>The decompressed data.</returns>
				std::vector<byte> Decompress(std::vector<byte> data);

				/// <summary>
				/// Compress the data.
				/// </summary>
				/// <param name="data">The data to compress.</param>
				/// <returns>The compressed data.</returns>
				std::vector<byte> Compress(std::vector<byte> data);

			private:
				bool _disposed;
				bool _bitsMapHasChanged;
				HuffCodeMap _bitsMap;
				HuffCode _eos;
				Node* _root;

				/// <summary>
				/// Build the tree.
				/// </summary>
				/// <param name="frequencies">The frequence array of integers.</param>
				/// <returns>The tree node array.</returns>
				INode* BuildTree(const int frequencies[UniqueSymbols]);

				/// <summary>
				/// Build the tree.
				/// </summary>
				void BuildTree();

				/// <summary>
				/// Generate the codes.
				/// </summary>
				/// <param name="node">The cuurent node.</param>
				/// <param name="prefix">The node prefix bits.</param>
				/// <param name="outCodes">The code bits for each character.</param>
				void GenerateCodes(const INode* node, const HuffCode& prefix, HuffCodeMap& outCodes);

				/// <summary>
				/// Load the codes.
				/// </summary>
				void LoadCodes();

				/// <summary>
				/// Add the bits.
				/// </summary>
				/// <param name="bits">The bits.</param>
				void Add(std::vector<bool> bits);

				/// <summary>
				/// Get bit.
				/// </summary>
				/// <param name="b">The byte.</param>
				/// <param name="pos">The position.</param>
				/// <returns>The bit.</returns>
				bool GetBit(byte b, byte pos);

				/// <summary>
				/// Get byte.
				/// </summary>
				/// <param name="bits">The bits.</param>
				/// <param name="offset">The offset.</param>
				/// <param name="count">The count.</param>
				/// <returns>The byte.</returns>
				byte GetByte(std::vector<bool> bits, int offset, byte count);

				/// <summary>
				/// Get the bytes.
				/// </summary>
				/// <param name="bits">The bits.</param>
				/// <returns>The bytes.</returns>
				std::vector<byte> GetBytes(std::vector<bool> bits);

				/// <summary>
				/// Convert to bytes.
				/// </summary>
				/// <param name="bits">The bits.</param>
				/// <returns>The bytes.</returns>
				std::vector<byte> ToBytes(std::vector<bool> bits);

				/// <summary>
				/// Convert to bits.
				/// </summary>
				/// <param name="bytes">The bytes.</param>
				/// <returns>THe bits.</returns>
				std::vector<bool> ToBits(std::vector<byte> bytes);

			};
		}
	}
}
#endif