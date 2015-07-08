/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          HuffmanCode.cpp
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

#include "stdafx.h"

#include "HuffmanCode.h"

namespace Nequeo {
	namespace IO {
		namespace Compression
		{
			/// <summary>
			/// Huffman codes.
			/// </summary>
			HuffmanCode::HuffmanCode() : _disposed(false), _bitsMapHasChanged(true)
			{
				// Load the code bit map.
				LoadCodes();

				// Build the tree.
				BuildTree();
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			HuffmanCode::~HuffmanCode()
			{
				// If not disposed.
				if (!_disposed)
				{
					// Indicate that dispose has been called.
					_disposed = true;

					if (_root != nullptr)
						delete _root;

					_eos.clear();
					_bitsMap.clear();
				}
			}

			/// <summary>
			/// Decompress the data.
			/// </summary>
			/// <param name="data">The compressed data.</param>
			/// <returns>The decompressed data.</returns>
			std::vector<byte> HuffmanCode::Decompress(std::vector<byte> data)
			{
				// Return the decompressed data.
				return GetBytes(ToBits(data));
			}

			/// <summary>
			/// Compress the data.
			/// </summary>
			/// <param name="data">The data to compress.</param>
			/// <returns>The compressed data.</returns>
			std::vector<byte> HuffmanCode::Compress(std::vector<byte> data)
			{
				std::vector<bool> huffmanEncodedMessage;

				// For each byte in bytes.
				for (vector<byte>::iterator btData = data.begin(); btData != data.end(); btData++)
				{
					// Get the byte value.
					byte byteItem = *btData;
					std::vector<bool> bits = GetBits(byteItem);

					// For each bit in bits.
					for (vector<bool>::iterator bt = bits.begin(); bt != bits.end(); bt++)
					{
						// Get the bit value.
						bool bit = *bt;

						// Add the bits.
						huffmanEncodedMessage.push_back(bit);
					}
				}

				// Adds most significant bytes of EOS.
				int temp = 8 - huffmanEncodedMessage.size() % 8;
				int numberOfBitsInPadding = temp == 8 ? 0 : temp;

				// Add the padding bits from the the EOS.
				for (int i = 0; i < numberOfBitsInPadding; i++)
				{
					// Add the bits.
					huffmanEncodedMessage.push_back(_eos[i]);
				}

				// Return the compressed data.
				return ToBytes(huffmanEncodedMessage);
			}

			/// <summary>
			/// Get the code map.
			/// </summary>
			/// <param name="value">The value to generate a code map for.</param>
			/// <returns>The code map.</returns>
			HuffmanCode::HuffCodeMap HuffmanCode::GetCode(const unsigned char* value)
			{
				// Create the frequency array of integers,
				// assign with zero.
				int frequencies[UniqueSymbols] = { 0 };
				
				// Build frequency table.
				// iterate through the char array.
				while (*value != '\0')
					++frequencies[*value++];

				// Build the tree.
				INode* root = BuildTree(frequencies);

				// Code map.
				HuffCodeMap codes;
				HuffCode code;

				// If the tree root exists.
				if (root != nullptr)
				{
					// Generate the codes.
					GenerateCodes(root, code, codes);
					delete root;
				}

				// Return the code map.
				return codes;
			}

			/// <summary>
			/// Build the tree.
			/// </summary>
			/// <param name="frequencies">The frequence array of integers function.</param>
			/// <returns>The tree node array.</returns>
			INode* HuffmanCode::BuildTree(const int frequencies[UniqueSymbols])
			{
				std::priority_queue<INode*, std::vector<INode*>, NodeCmp> trees;

				// For each symbol
				for (int i = 0; i < UniqueSymbols; ++i)
				{
					// If the current symbol is not zero.
					if (frequencies[i] != 0)
						trees.push(new LeafNode(frequencies[i], (unsigned char)i));
				}

				// While the tree size is greater than one.
				while (trees.size() > 1)
				{
					INode* childR = trees.top();
					trees.pop();

					INode* childL = trees.top();
					trees.pop();

					INode* parent = new InternalNode(childR, childL);
					trees.push(parent);
				}

				// Return the built tree node.
				return trees.top();
			}

			/// <summary>
			/// Generate the codes.
			/// </summary>
			/// <param name="node">The cuurent node.</param>
			/// <param name="prefix">The node prefix bits.</param>
			/// <param name="outCodes">The code bits for each character.</param>
			void HuffmanCode::GenerateCodes(const INode* node, const HuffCode& prefix, HuffCodeMap& outCodes)
			{
				// If the node is a leaf node.
				if (const LeafNode* lf = dynamic_cast<const LeafNode*>(node))
				{
					// Assign the bits array to the character.
					outCodes[lf->_c] = prefix;
				}
				else if (const InternalNode* in = dynamic_cast<const InternalNode*>(node))
				{
					// Loop through the tree nodes.
					HuffCode leftPrefix = prefix;
					leftPrefix.push_back(false);
					GenerateCodes(in->_left, leftPrefix, outCodes);

					HuffCode rightPrefix = prefix;
					rightPrefix.push_back(true);
					GenerateCodes(in->_right, rightPrefix, outCodes);
				}
			}

			/// <summary>
			/// Gets the size of the header code.
			/// </summary>
			int HuffmanCode::getSize()
			{
				int size = 0;

				// Get the codes.
				HuffCodeMap codes = getCode();

				// For each code.
				for (HuffCodeMap::const_iterator it = codes.begin(); it != codes.end(); ++it)
				{
					// Get the char and the code vector.
					const unsigned char charcter = it->first;
					HuffCode code = it->second;

					// Get the size as and integer.
					vector<int>::size_type currentSize = code.size();

					// Increment to count.
					size += currentSize;
				}

				// Return the size.
				return size;
			}

			/// <summary>
			/// Get the code map for headers used in http enconding.
			/// </summary>
			HuffmanCode::HuffCodeMap HuffmanCode::getCode()
			{
				// Return the code map.
				return _bitsMap;
			}

			/// <summary>
			/// Sets the code map for headers used in http enconding.
			/// </summary>
			void HuffmanCode::setCode(HuffCodeMap bitMap)
			{
				_bitsMapHasChanged = true;
				_bitsMap = bitMap;
			}

			/// <summary>
			/// Get the byte.
			/// </summary>
			/// <param name="bits">The bit array.</param>
			/// <returns>The byte.</returns>
			unsigned char HuffmanCode::GetByte(std::vector<bool> bits)
			{
				char byte;
				bool match = false;

				// Get the codes.
				HuffCodeMap codes = getCode();

				// For each code.
				for (HuffCodeMap::const_iterator it = codes.begin(); it != codes.end(); ++it)
				{
					// Get the char and the code vector.
					const unsigned char charcter = it->first;
					HuffCode code = it->second;

					// If the same size.
					if (code.size() == bits.size())
					{
						int index = 0;
						match = true;

						// For each bit in bits.
						for (vector<bool>::iterator bt = bits.begin(); bt != bits.end(); bt++)
						{
							// Get the bit value.
							bool bit = *bt;

							// If the bits are not equal.
							if (bit != code[index])
							{
								match = false;
								break;
							}

							// Increment the index.
							index++;
						}

						// If a match has been found.
						if (match)
						{
							byte = charcter;
							break;
						}
					}
				}

				// If a match has not been found.
				if (!match)
				{
					throw exception("Symbol is not present in the character set.");
				}

				// Return the byte.
				return byte;
			}

			/// <summary>
			/// Get the bits.
			/// </summary>
			/// <param name="c">The byte.</param>
			/// <returns>The bit collection.</returns>
			std::vector<bool> HuffmanCode::GetBits(unsigned char c)
			{
				std::vector<bool> bits;
				bool match = false;

				// Get the codes.
				HuffCodeMap codes = getCode();

				// For each code.
				for (HuffCodeMap::const_iterator it = codes.begin(); it != codes.end(); ++it)
				{
					// Get the char and the code vector.
					const unsigned char charcter = it->first;
					HuffCode code = it->second;

					// If the character match.
					if (charcter == c)
					{
						bits = code;
						match = true;
						break;
					}
				}

				// If a match has not been found.
				if (!match)
				{
					throw exception("Symbol is not present in the character set.");
				}

				// Return the bits.
				return bits;
			}

			/// <summary>
			/// As the Huffman encoded data doesn't always end at an octet boundary,
			/// some padding is inserted after it up to the next octet boundary. To
			/// prevent this padding to be misinterpreted as part of the string
			/// literal, the most significant bits of the EOS (end-of-string) entry
			/// in the Huffman table are used.
			/// </summary>
			/// <param name="bits">The bit array.</param>
			/// <returns>True if valid padding; else false.</returns>
			bool HuffmanCode::IsValidPadding(std::vector<bool> bits)
			{
				// If more than 7 bits.
				if (bits.size() >= 8)
				{
					return false;
				}

				int index = 0;
				bool match = true;
				HuffCode code = getEOS();

				// For each bit in bits.
				for (vector<bool>::iterator bt = bits.begin(); bt != bits.end(); bt++)
				{
					// Get the bit value.
					bool bit = *bt;

					// If not equal.
					if (bit != code[index])
					{
						match = false;
						break;
					}

					// Increment the index.
					index++;
				}

				// Return the result.
				return match;
			}

			/// <summary>
			/// Get bit.
			/// </summary>
			/// <param name="b">The byte.</param>
			/// <param name="pos">The position.</param>
			/// <returns>The bit.</returns>
			bool HuffmanCode::GetBit(byte b, byte pos)
			{
				if (pos > 7) throw invalid_argument("pos > 7");

				byte mask = (byte)(1 << pos);
				byte masked = (byte)(b & mask);
				return masked != 0;
			}

			/// <summary>
			/// Get byte.
			/// </summary>
			/// <param name="bits">The bits.</param>
			/// <param name="offset">The offset.</param>
			/// <param name="count">The count.</param>
			/// <returns>The byte.</returns>
			byte HuffmanCode::GetByte(std::vector<bool> bits, int offset, byte count)
			{
				if (count == 0) throw invalid_argument("count is 0");
				if (count > 8) throw invalid_argument("byte is 8 bits");

				byte result = 0;
				int endIndex = offset + count;
				int bitIndex = 7;

				// For each bit in bits.
				for (int i = offset; i < endIndex; i++, bitIndex--)
				{
					// If true then add to the reult.
					if (bits[i])
						result |= (byte)(1 << bitIndex);
				}

				// Return the result.
				return result;
			}

			/// <summary>
			/// Convert to bytes.
			/// </summary>
			/// <param name="bits">The bits.</param>
			/// <returns>The bytes.</returns>
			std::vector<byte> HuffmanCode::ToBytes(std::vector<bool> bits)
			{
				// Get the size as and integer.
				vector<int>::size_type size = bits.size();

				// Set the vector size.
				int elements = (size % 8 == 0 ? size / 8 : (size / 8) + 1);
				std::vector<byte> result(elements);

				int offset = 0;
				int count = 8;
				int resIndex = 0;

				// Start converting.
				while (count != 0)
				{
					// Get the current byte from the bits.
					result[resIndex] = GetByte(bits, offset, count);

					// Progress to the next byte.
					offset += count;

					// Reduce the rolling offset.
					int roffset = size - offset;

					// Get the new count.
					count = roffset >= 8 ? 8 : roffset;

					// Incrrement.
					resIndex++;
				}

				// Return the result.
				return result;
			}

			/// <summary>
			/// Convert to bits.
			/// </summary>
			/// <param name="bytes">The bytes.</param>
			/// <returns>THe bits.</returns>
			std::vector<bool> HuffmanCode::ToBits(std::vector<byte> bytes)
			{
				// Get the size as and integer.
				vector<int>::size_type size = bytes.size();

				// Set the vector size.
				int elements = size * 8;
				std::vector<bool> result(elements);

				// For each byte in bytes.
				for (int i = 0; i < size; i++)
				{
					// For each bit.
					for (int j = 0; j < 8; j++)
					{
						// Get the current bit in the byte.
						result[i * 8 + j] = GetBit(bytes[i], (byte)(7 - j));
					}
				}

				// Return the result.
				return result;
			}

			/// <summary>
			/// Get the bytes.
			/// </summary>
			/// <param name="bits">The bits.</param>
			/// <returns>The bytes.</returns>
			std::vector<byte> HuffmanCode::GetBytes(std::vector<bool> bits)
			{
				std::vector<byte> result;
				int i = 0;
				int j = 0;
				bool isEos = true;

				// Get the size as and integer.
				vector<int>::size_type size = bits.size();
				vector<int>::size_type eosSize = _eos.size();

				// While more bits exist.
				while (i < size)
				{
					j = 0;
					isEos = true;

					// Create a temp node.
					Node* temp = _root;
					std::vector<bool> symbolBits;

					// While more bits exist.
					while (i < size)
					{
						// Get the current bit.
						temp = !bits[i] ? temp->getLeft() : temp->getRight();

						// If the node is null.
						if (temp == nullptr)
							break;

						// Add the bit value.
						symbolBits.push_back(temp->getValue());
						isEos &= temp->getValue() == _eos[j];

						// Is end of string.
						if (isEos && ++j == eosSize)
						{
							// see spec 07 - > 4.1.2.  String Literal Representation
							// A Huffman encoded string literal containing the EOS entry
							// MUST be treated as a decoding error.
							throw exception("EOS contains");
						}

						// Increment the index.
						i++;
					}

					// If padding is valid.
					if (IsValidPadding(symbolBits))
						break;

					// See spec 07 -> 4.1.2.  String Literal Representation
					// A padding strictly longer than 7 bits MUST be treated as a decoding error.
					// A padding not corresponding to the most significant bits of the EOS
					// entry MUST be treated as a decoding error.

					// If padding is not valid or padding is longer than 7 bits
					// then decoding error will thrown by GetByte method 
					// since not turn recognize the symbol.
					byte symbol = GetByte(symbolBits);
					result.push_back(symbol);
				}

				// Return the result.
				return result;
			}

			/// <summary>
			/// Build the tree.
			/// </summary>
			void HuffmanCode::BuildTree()
			{
				// If a new bit map has been set.
				if (_bitsMapHasChanged)
				{
					if (_root != nullptr)
						delete _root;

					// Create a new root node.
					_root = new Node(false, nullptr);

					// Get the codes.
					HuffCodeMap codes = getCode();

					// For each code.
					for (HuffCodeMap::const_iterator it = codes.begin(); it != codes.end(); ++it)
					{
						// Get the char and the code vector.
						const unsigned char charcter = it->first;
						HuffCode code = it->second;

						// Add bits for each charactor.
						Add(code);
					}

					// Add the end of string bits.
					Add(getEOS());

					// Use existing.
					_bitsMapHasChanged = false;
				}
			}

			/// <summary>
			/// Add the bits.
			/// </summary>
			/// <param name="bits">The bits.</param>
			void HuffmanCode::Add(std::vector<bool> bits)
			{
				int index = 0;
				Node* temp = _root;

				// For each bit in bits.
				for (vector<bool>::iterator bt = bits.begin(); bt != bits.end(); bt++)
				{
					// Get the bit value.
					bool bit = *bt;

					// If false.
					if (!bit)
					{
						if (temp->getLeft() == nullptr)
							temp->setLeft(new Node(false, temp));

						temp = temp->getLeft();
					}
					else
					{
						if (temp->getRight() == nullptr)
							temp->setRight(new Node(true, temp));

						temp = temp->getRight();
					}

					// Increment the index.
					index++;
				}
			}

			/// <summary>
			/// Gets or sets the end of string code for headers used in http enconding.
			/// </summary>
			HuffmanCode::HuffCode HuffmanCode::getEOS()
			{
				return _eos;
			}

			/// <summary>
			/// Gets or sets the end of string code for headers used in http enconding.
			/// </summary>
			void HuffmanCode::setEOS(HuffCode code)
			{
				_eos = code;
			}

			/// <summary>
			/// Load the header codes.
			/// </summary>
			void HuffmanCode::LoadCodes()
			{
				// End of string.
				_eos = { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T };

				// Insert all codes.
				_bitsMap.insert({ (unsigned char)0, { T, T, T, T, T, T, T, T, T, T, F, F, F } });														//'' (0) |11111111|11000
				_bitsMap.insert({ (unsigned char)1, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, F, F } });							//'' (1) |11111111|11111111|1011000
				_bitsMap.insert({ (unsigned char)2, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, T, F } });			//'' (2) |11111111|11111111|11111110|0010
				_bitsMap.insert({ (unsigned char)3, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, T, T } });			//'' (3) |11111111|11111111|11111110|0011
				_bitsMap.insert({ (unsigned char)4, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, F, F } });			//'' (4) |11111111|11111111|11111110|0100
				_bitsMap.insert({ (unsigned char)5, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, F, T } });			//'' (5) |11111111|11111111|11111110|0101
				_bitsMap.insert({ (unsigned char)6, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, T, F } });			//'' (6) |11111111|11111111|11111110|0110
				_bitsMap.insert({ (unsigned char)7, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, T, T } });			//'' (7) |11111111|11111111|11111110|0111
				_bitsMap.insert({ (unsigned char)8, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, F, F } });			//'' (8) |11111111|11111111|11111110|1000
				_bitsMap.insert({ (unsigned char)9, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T, F } });						//'' (9) |11111111|11111111|11101010
				_bitsMap.insert({ (unsigned char)10, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F } });	//'' (10) |11111111|11111111|11111111|111100
				_bitsMap.insert({ (unsigned char)11, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, F, T } });			//'' (11) |11111111|11111111|11111110|1001
				_bitsMap.insert({ (unsigned char)12, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T, F } });			//'' (12) |11111111|11111111|11111110|1010
				_bitsMap.insert({ (unsigned char)13, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T } });	//'' (13) |11111111|11111111|11111111|111101
				_bitsMap.insert({ (unsigned char)14, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T, T } });			//'' (14) |11111111|11111111|11111110|1011
				_bitsMap.insert({ (unsigned char)15, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, F } });			//'' (15) |11111111|11111111|11111110|1100
				_bitsMap.insert({ (unsigned char)16, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, T } });			//'' (16) |11111111|11111111|11111110|1101
				_bitsMap.insert({ (unsigned char)17, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, F } });			//'' (17) |11111111|11111111|11111110|1110
				_bitsMap.insert({ (unsigned char)18, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, T } });			//'' (18) |11111111|11111111|11111110|1111
				_bitsMap.insert({ (unsigned char)19, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, F } });			//'' (19) |11111111|11111111|11111111|0000
				_bitsMap.insert({ (unsigned char)20, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, T } });			//'' (20) |11111111|11111111|11111111|0001
				_bitsMap.insert({ (unsigned char)21, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, F } });			//'' (21) |11111111|11111111|11111111|0010
				_bitsMap.insert({ (unsigned char)22, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F } });	//'' (22) |11111111|11111111|11111111|111110
				_bitsMap.insert({ (unsigned char)23, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, T } });			//'' (23) |11111111|11111111|11111111|0011
				_bitsMap.insert({ (unsigned char)24, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, F } });			//'' (24) |11111111|11111111|11111111|0100
				_bitsMap.insert({ (unsigned char)25, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T } });			//'' (25) |11111111|11111111|11111111|0101
				_bitsMap.insert({ (unsigned char)26, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F } });			//'' (26) |11111111|11111111|11111111|0110
				_bitsMap.insert({ (unsigned char)27, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T } });			//'' (27) |11111111|11111111|11111111|0111
				_bitsMap.insert({ (unsigned char)28, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F } });			//'' (28) |11111111|11111111|11111111|1000
				_bitsMap.insert({ (unsigned char)29, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T } });			//'' (29) |11111111|11111111|11111111|1001
				_bitsMap.insert({ (unsigned char)30, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F } });			//'' (30) |11111111|11111111|11111111|1010
				_bitsMap.insert({ (unsigned char)31, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T } });			//'' (31) |11111111|11111111|11111111|1011
				_bitsMap.insert({ (unsigned char)32, { F, T, F, T, F, F } });																			//' ' (32) |010100
				_bitsMap.insert({ (unsigned char)33, { T, T, T, T, T, T, T, F, F, F } });																//'!' (33) |11111110|00
				_bitsMap.insert({ (unsigned char)34, { T, T, T, T, T, T, T, F, F, T } });																//'"' (34) |11111110|01
				_bitsMap.insert({ (unsigned char)35, { T, T, T, T, T, T, T, T, T, F, T, F } });															//'#' (35) |11111111|1010
				_bitsMap.insert({ (unsigned char)36, { T, T, T, T, T, T, T, T, T, T, F, F, T } });														//'$' (36) |11111111|11001
				_bitsMap.insert({ (unsigned char)37, { F, T, F, T, F, T } });																			//'%' (37) |010101
				_bitsMap.insert({ (unsigned char)38, { T, T, T, T, T, F, F, F } });																		//'&' (38) |11111000
				_bitsMap.insert({ (unsigned char)39, { T, T, T, T, T, T, T, T, F, T, F } });															//''' (39) |11111111|010
				_bitsMap.insert({ (unsigned char)40, { T, T, T, T, T, T, T, F, T, F } });																//'(' (40) |11111110|10
				_bitsMap.insert({ (unsigned char)41, { T, T, T, T, T, T, T, F, T, T } });																//')' (41) |11111110|11
				_bitsMap.insert({ (unsigned char)42, { T, T, T, T, T, F, F, T } });																		//'*' (42) |11111001
				_bitsMap.insert({ (unsigned char)43, { T, T, T, T, T, T, T, T, F, T, T } });															//'+' (43) |11111111|011
				_bitsMap.insert({ (unsigned char)44, { T, T, T, T, T, F, T, F } });																		//',' (44) |11111010
				_bitsMap.insert({ (unsigned char)45, { F, T, F, T, T, F } });																			//'-' (45) |010110
				_bitsMap.insert({ (unsigned char)46, { F, T, F, T, T, T } });																			//'.' (46) |010111
				_bitsMap.insert({ (unsigned char)47, { F, T, T, F, F, F } });																			//'/' (47) |011000
				_bitsMap.insert({ (unsigned char)48, { F, F, F, F, F } });																				//'0' (48) |00000  
				_bitsMap.insert({ (unsigned char)49, { F, F, F, F, T } });																				//'1' (49) |00001
				_bitsMap.insert({ (unsigned char)50, { F, F, F, T, F } });																				//'2' (50) |00010
				_bitsMap.insert({ (unsigned char)51, { F, T, T, F, F, T } });																			//'3' (51) |011001
				_bitsMap.insert({ (unsigned char)52, { F, T, T, F, T, F } });																			//'4' (52) |011010
				_bitsMap.insert({ (unsigned char)53, { F, T, T, F, T, T } });																			//'5' (53) |011011
				_bitsMap.insert({ (unsigned char)54, { F, T, T, T, F, F } });																			//'6' (54) |011100
				_bitsMap.insert({ (unsigned char)55, { F, T, T, T, F, T } });																			//'7' (55) |011101
				_bitsMap.insert({ (unsigned char)56, { F, T, T, T, T, F } });																			//'8' (56) |011110
				_bitsMap.insert({ (unsigned char)57, { F, T, T, T, T, T } });																			//'9' (57) |011111
				_bitsMap.insert({ (unsigned char)58, { T, F, T, T, T, F, F } });																		//':' (58) |1011100
				_bitsMap.insert({ (unsigned char)59, { T, T, T, T, T, F, T, T } });																		//';' (59) |11111011
				_bitsMap.insert({ (unsigned char)60, { T, T, T, T, T, T, T, T, T, T, T, T, T, F, F } });												//'<' (60) |11111111|1111100
				_bitsMap.insert({ (unsigned char)61, { T, F, F, F, F, F } });																			//'=' (61) |100000
				_bitsMap.insert({ (unsigned char)62, { T, T, T, T, T, T, T, T, T, F, T, T } });															//'>' (62) |11111111|1011
				_bitsMap.insert({ (unsigned char)63, { T, T, T, T, T, T, T, T, F, F } });																//'?' (63) |11111111|00
				_bitsMap.insert({ (unsigned char)64, { T, T, T, T, T, T, T, T, T, T, F, T, F } });														//'@' (64) |11111111|11010
				_bitsMap.insert({ (unsigned char)65, { T, F, F, F, T } });																				//'A' (65) |100001
				_bitsMap.insert({ (unsigned char)66, { T, F, T, T, T, F, T } });																		//'B' (66) |1011101
				_bitsMap.insert({ (unsigned char)67, { T, F, T, T, T, T, F } });																		//'C' (67) |1011110
				_bitsMap.insert({ (unsigned char)68, { T, F, T, T, T, T, T } });																		//'D' (68) |1011111
				_bitsMap.insert({ (unsigned char)69, { T, T, F, F, F, F, F } });																		//'E' (69) |1100000
				_bitsMap.insert({ (unsigned char)70, { T, T, F, F, F, F, T } });																		//'F' (70) |1100001
				_bitsMap.insert({ (unsigned char)71, { T, T, F, F, F, T, F } });																		//'G' (71) |1100010
				_bitsMap.insert({ (unsigned char)72, { T, T, F, F, F, T, T } });																		//'H' (72) |1100011
				_bitsMap.insert({ (unsigned char)73, { T, T, F, F, T, F, F } });																		//'I' (73) |1100100
				_bitsMap.insert({ (unsigned char)74, { T, T, F, F, T, F, T } });																		//'J' (74) |1100101
				_bitsMap.insert({ (unsigned char)75, { T, T, F, F, T, T, F } });																		//'K' (75) |1100110
				_bitsMap.insert({ (unsigned char)76, { T, T, F, F, T, T, T } });																		//'L' (76) |1100111
				_bitsMap.insert({ (unsigned char)77, { T, T, F, T, F, F, F } });																		//'M' (77) |1101000
				_bitsMap.insert({ (unsigned char)78, { T, T, F, T, F, F, T } });																		//'N' (78) |1101001
				_bitsMap.insert({ (unsigned char)79, { T, T, F, T, F, T, F } });																		//'O' (79) |1101010
				_bitsMap.insert({ (unsigned char)80, { T, T, F, T, F, T, T } });																		//'P' (80) |1101011
				_bitsMap.insert({ (unsigned char)81, { T, T, F, T, T, F, F } });																		//'Q' (81) |1101100
				_bitsMap.insert({ (unsigned char)82, { T, T, F, T, T, F, T } });																		//'R' (82) |1101101
				_bitsMap.insert({ (unsigned char)83, { T, T, F, T, T, T, F } });																		//'S' (83) |1101110
				_bitsMap.insert({ (unsigned char)84, { T, T, F, T, T, T, T } });																		//'T' (84) |1101111
				_bitsMap.insert({ (unsigned char)85, { T, T, T, F, F, F, F } });																		//'U' (85) |1110000
				_bitsMap.insert({ (unsigned char)86, { T, T, T, F, F, F, T } });																		//'V' (86) |1110001
				_bitsMap.insert({ (unsigned char)87, { T, T, T, F, F, T, F } });																		//'W' (87) |1110010
				_bitsMap.insert({ (unsigned char)88, { T, T, T, T, T, T, F, F } });																		//'X' (88) |11111100
				_bitsMap.insert({ (unsigned char)89, { T, T, T, F, F, T, T } });																		//'Y' (89) |1110011
				_bitsMap.insert({ (unsigned char)90, { T, T, T, T, T, T, F, T } });																		//'Z' (90) |11111101
				_bitsMap.insert({ (unsigned char)91, { T, T, T, T, T, T, T, T, T, T, F, T, T } });														//'[' (91) |11111111|11011
				_bitsMap.insert({ (unsigned char)92, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, F } });									//'\' (92) |11111111|11111110|000
				_bitsMap.insert({ (unsigned char)93, { T, T, T, T, T, T, T, T, T, T, T, F, F } });														//']' (93) |11111111|11100
				_bitsMap.insert({ (unsigned char)94, { T, T, T, T, T, T, T, T, T, T, T, T, F, F } });													//'^' (94) |11111111|111100
				_bitsMap.insert({ (unsigned char)95, { T, F, F, F, T, F } });																			//'_' (95) |100010
				_bitsMap.insert({ (unsigned char)96, { T, T, T, T, T, T, T, T, T, T, T, T, T, F, T } });												//'`' (96) |11111111|1111101
				_bitsMap.insert({ (unsigned char)97, { F, F, F, T, T } });																				//'a' (97) |00011
				_bitsMap.insert({ (unsigned char)98, { T, F, F, F, T, T } });																			//'b' (98) |100011
				_bitsMap.insert({ (unsigned char)99, { F, F, T, F, F } });																				//'c' (99) |00100
				_bitsMap.insert({ (unsigned char)100, { T, F, F, T, F, F } });																			//'d' (100) |100100
				_bitsMap.insert({ (unsigned char)101, { F, F, T, F, T } });																				//'e' (101) |00101
				_bitsMap.insert({ (unsigned char)102, { T, F, F, T, F, T } });																			//'f' (102) |100101
				_bitsMap.insert({ (unsigned char)103, { T, F, F, T, T, F } });																			//'g' (103) |100110
				_bitsMap.insert({ (unsigned char)104, { T, F, F, T, T, T } });																			//'h' (104) |100111
				_bitsMap.insert({ (unsigned char)105, { F, F, T, T, F } });																				//'i' (105) |00110
				_bitsMap.insert({ (unsigned char)106, { T, T, T, F, T, F, F } });																		//'j' (106) |1110100
				_bitsMap.insert({ (unsigned char)107, { T, T, T, F, T, F, T } });																		//'k' (107) |1110101
				_bitsMap.insert({ (unsigned char)108, { T, F, T, F, F, F } });																			//'l' (108) |101000
				_bitsMap.insert({ (unsigned char)109, { T, F, T, F, F, T } });																			//'m' (109) |101001
				_bitsMap.insert({ (unsigned char)110, { T, F, T, F, T, F } });																			//'n' (110) |101010
				_bitsMap.insert({ (unsigned char)111, { F, F, T, T, T } });																				//'o' (111) |00111
				_bitsMap.insert({ (unsigned char)112, { T, F, T, F, T, T } });																			//'p' (112) |101011
				_bitsMap.insert({ (unsigned char)113, { T, T, T, F, T, T, F } });																		//'q' (113) |1110110
				_bitsMap.insert({ (unsigned char)114, { T, F, T, T, F, F } });																			//'r' (114) |101100
				_bitsMap.insert({ (unsigned char)115, { F, T, F, F, F } });																				//'s' (115) |01000
				_bitsMap.insert({ (unsigned char)116, { F, T, F, F, T } });																				//'t' (116) |01001
				_bitsMap.insert({ (unsigned char)117, { T, F, T, T, F, T } });																			//'u' (117) |101101
				_bitsMap.insert({ (unsigned char)118, { T, T, T, F, T, T, T } });																		//'v' (118) |1110111
				_bitsMap.insert({ (unsigned char)119, { T, T, T, T, F, F, F } });																		//'w' (119) |1111000
				_bitsMap.insert({ (unsigned char)120, { T, T, T, T, F, F, T } });																		//'x' (120) |1111001
				_bitsMap.insert({ (unsigned char)121, { T, T, T, T, F, T, F } });																		//'y' (121) |1111010
				_bitsMap.insert({ (unsigned char)122, { T, T, T, T, F, T, T } });																		//'z' (122) |1111011
				_bitsMap.insert({ (unsigned char)123, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, F } });												//'{' (123) |11111111|1111110
				_bitsMap.insert({ (unsigned char)124, { T, T, T, T, T, T, T, T, T, F, F } });															//'|' (124) |11111111|100
				_bitsMap.insert({ (unsigned char)125, { T, T, T, T, T, T, T, T, T, T, T, T, F, T } });													//'}' (125) |11111111|111101
				_bitsMap.insert({ (unsigned char)126, { T, T, T, T, T, T, T, T, T, T, T, F, T } });														//'~' (126) |11111111|11101
				_bitsMap.insert({ (unsigned char)127, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F } });		//'' (127) |11111111|11111111|11111111|1100
				_bitsMap.insert({ (unsigned char)128, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, T, F } });								//'' (128) |11111111|11111110|0110
				_bitsMap.insert({ (unsigned char)129, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, F, T, F } });							//'' (129) |11111111|11111111|010010
				_bitsMap.insert({ (unsigned char)130, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, T, T } });								//'' (130) |11111111|11111110|0111
				_bitsMap.insert({ (unsigned char)131, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, F, F } });								//'' (131) |11111111|11111110|1000
				_bitsMap.insert({ (unsigned char)132, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, F, T, T } });							//'' (132) |11111111|11111111|010011
				_bitsMap.insert({ (unsigned char)133, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T, F, F } });							//'' (133) |11111111|11111111|010100
				_bitsMap.insert({ (unsigned char)134, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T, F, T } });							//'' (134) |11111111|11111111|010101
				_bitsMap.insert({ (unsigned char)135, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, F, T } });						//'' (135) |11111111|11111111|1011001
				_bitsMap.insert({ (unsigned char)136, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T, T, F } });							//'' (136) |11111111|11111111|010110
				_bitsMap.insert({ (unsigned char)137, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, T, F } });						//'' (137) |11111111|11111111|1011010
				_bitsMap.insert({ (unsigned char)138, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, T, T } });						//'' (138) |11111111|11111111|1011011
				_bitsMap.insert({ (unsigned char)139, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, F, F } });						//'' (139) |11111111|11111111|1011100
				_bitsMap.insert({ (unsigned char)140, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, F, T } });						//'' (140) |11111111|11111111|1011101
				_bitsMap.insert({ (unsigned char)141, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, T, F } });						//'' (141) |11111111|11111111|1011110
				_bitsMap.insert({ (unsigned char)142, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T, T } });					//'' (142) |11111111|11111111|11101011
				_bitsMap.insert({ (unsigned char)143, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, T, T } });						//'' (143) |11111111|11111111|1011111
				_bitsMap.insert({ (unsigned char)144, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, F } });					//'' (144) |11111111|11111111|11101100
				_bitsMap.insert({ (unsigned char)145, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, T } });					//'' (145) |11111111|11111111|11101101
				_bitsMap.insert({ (unsigned char)146, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T, T, T } });							//'' (146) |11111111|11111111|010111
				_bitsMap.insert({ (unsigned char)147, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, F, F } });						//'' (147) |11111111|11111111|1100000
				_bitsMap.insert({ (unsigned char)148, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, F } });					//'' (148) |11111111|11111111|11101110
				_bitsMap.insert({ (unsigned char)149, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, F, T } });						//'' (149) |11111111|11111111|1100001
				_bitsMap.insert({ (unsigned char)150, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, T, F } });						//'' (150) |11111111|11111111|1100010
				_bitsMap.insert({ (unsigned char)151, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, T, T } });						//'' (151) |11111111|11111111|1100011
				_bitsMap.insert({ (unsigned char)152, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, F, F } });						//'' (152) |11111111|11111111|1100100
				_bitsMap.insert({ (unsigned char)153, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, F, F } });								//'' (153) |11111111|11111110|11100
				_bitsMap.insert({ (unsigned char)154, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, F, F } });							//'' (154) |11111111|11111111|011000
				_bitsMap.insert({ (unsigned char)155, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, F, T } });						//'' (155) |11111111|11111111|1100101
				_bitsMap.insert({ (unsigned char)156, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, F, T } });							//'' (156) |11111111|11111111|011001
				_bitsMap.insert({ (unsigned char)157, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, T, F } });						//'' (157) |11111111|11111111|1100110
				_bitsMap.insert({ (unsigned char)158, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, T, T } });						//'' (158) |11111111|11111111|1100111
				_bitsMap.insert({ (unsigned char)159, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, T } });					//'' (159) |11111111|11111111|11101111
				_bitsMap.insert({ (unsigned char)160, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, T, F } });							//'' (160) |11111111|11111111|011010
				_bitsMap.insert({ (unsigned char)161, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, F, T } });								//'' (161) |11111111|11111110|11101
				_bitsMap.insert({ (unsigned char)162, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, F, T } });								//'' (162) |11111111|11111110|1001
				_bitsMap.insert({ (unsigned char)163, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, T, T } });							//'' (163) |11111111|11111111|011011
				_bitsMap.insert({ (unsigned char)164, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, F, F } });							//'' (164) |11111111|11111111|011100
				_bitsMap.insert({ (unsigned char)165, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, F, F } });						//'' (165) |11111111|11111111|1101000
				_bitsMap.insert({ (unsigned char)166, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, F, T } });						//'' (166) |11111111|11111111|1101001
				_bitsMap.insert({ (unsigned char)167, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, T, F } });								//'' (167) |11111111|11111110|11110
				_bitsMap.insert({ (unsigned char)168, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T, F } });						//'' (168) |11111111|11111111|1101010
				_bitsMap.insert({ (unsigned char)169, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, F, T } });							//'' (169) |11111111|11111111|011101
				_bitsMap.insert({ (unsigned char)170, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, T, F } });							//'' (170) |11111111|11111111|011110
				_bitsMap.insert({ (unsigned char)171, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, F } });					//'' (171) |11111111|11111111|11110000
				_bitsMap.insert({ (unsigned char)172, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, T, T } });								//'' (172) |11111111|11111110|11111
				_bitsMap.insert({ (unsigned char)173, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, T, T } });							//'' (173) |11111111|11111111|011111
				_bitsMap.insert({ (unsigned char)174, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T, T } });						//'' (174) |11111111|11111111|1101011
				_bitsMap.insert({ (unsigned char)175, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, F } });						//'' (175) |11111111|11111111|1101100
				_bitsMap.insert({ (unsigned char)176, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, F, F } });								//'' (176) |11111111|11111111|00000
				_bitsMap.insert({ (unsigned char)177, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, F, T } });								//'' (177) |11111111|11111111|00001
				_bitsMap.insert({ (unsigned char)178, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, F, F } });							//'' (178) |11111111|11111111|100000
				_bitsMap.insert({ (unsigned char)179, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, T, F } });								//'' (179) |11111111|11111111|00010
				_bitsMap.insert({ (unsigned char)180, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, T } });						//'' (180) |11111111|11111111|1101101
				_bitsMap.insert({ (unsigned char)181, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, F, T } });							//'' (181) |11111111|11111111|100001
				_bitsMap.insert({ (unsigned char)182, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, F } });						//'' (182) |11111111|11111111|1101110
				_bitsMap.insert({ (unsigned char)183, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, T } });						//'' (183) |11111111|11111111|1101111
				_bitsMap.insert({ (unsigned char)184, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T, F } });								//'' (184) |11111111|11111110|1010
				_bitsMap.insert({ (unsigned char)185, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, T, F } });							//'' (185) |11111111|11111111|100010
				_bitsMap.insert({ (unsigned char)186, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, T, T } });							//'' (186) |11111111|11111111|100011
				_bitsMap.insert({ (unsigned char)187, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, F, F } });							//'' (187) |11111111|11111111|100100
				_bitsMap.insert({ (unsigned char)188, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, F } });						//'' (188) |11111111|11111111|1110000
				_bitsMap.insert({ (unsigned char)189, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, F, T } });							//'' (189) |11111111|11111111|100101
				_bitsMap.insert({ (unsigned char)190, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, T, F } });							//'' (190) |11111111|11111111|100110
				_bitsMap.insert({ (unsigned char)191, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, T } });						//'' (191) |11111111|11111111|1110001
				_bitsMap.insert({ (unsigned char)192, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, F, F } });				//'' (192) |11111111|11111111|11111000|00
				_bitsMap.insert({ (unsigned char)193, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, F, T } });				//'' (193) |11111111|11111111|11111000|01
				_bitsMap.insert({ (unsigned char)194, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T, T } });								//'' (194) |11111111|11111110|1011
				_bitsMap.insert({ (unsigned char)195, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, T } });									//'' (195) |11111111|11111110|001
				_bitsMap.insert({ (unsigned char)196, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, T, T } });							//'' (196) |11111111|11111111|100111
				_bitsMap.insert({ (unsigned char)197, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, F } });						//'' (197) |11111111|11111111|1110010
				_bitsMap.insert({ (unsigned char)198, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, F, F } });							//'' (198) |11111111|11111111|101000
				_bitsMap.insert({ (unsigned char)199, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, F } });					//'' (199) |11111111|11111111|11110110|0
				_bitsMap.insert({ (unsigned char)200, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, T, F } });				//'' (200) |11111111|11111111|11111000|10
				_bitsMap.insert({ (unsigned char)201, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, T, T } });				//'' (201) |11111111|11111111|11111000|11
				_bitsMap.insert({ (unsigned char)202, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, F, F } });				//'' (202) |11111111|11111111|11111001|00
				_bitsMap.insert({ (unsigned char)203, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, T, F } });			//'' (203) |11111111|11111111|11111011|110
				_bitsMap.insert({ (unsigned char)204, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, T, T } });			//'' (204) |11111111|11111111|11111011|111
				_bitsMap.insert({ (unsigned char)205, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, F, T } });				//'' (205) |11111111|11111111|11111001|01
				_bitsMap.insert({ (unsigned char)206, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, T } });					//'' (206) |11111111|11111111|11110001
				_bitsMap.insert({ (unsigned char)207, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, T } });					//'' (207) |11111111|11111111|11110110|1
				_bitsMap.insert({ (unsigned char)208, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, F } });									//'' (208) |11111111|11111110|010
				_bitsMap.insert({ (unsigned char)209, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, T, T } });								//'' (209) |11111111|11111111|00011
				_bitsMap.insert({ (unsigned char)210, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, T, F } });				//'' (210) |11111111|11111111|11111001|10
				_bitsMap.insert({ (unsigned char)211, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, F, F } });			//'' (211) |11111111|11111111|11111100|000
				_bitsMap.insert({ (unsigned char)212, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, F, T } });			//'' (212) |11111111|11111111|11111100|001
				_bitsMap.insert({ (unsigned char)213, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, T, T } });				//'' (213) |11111111|11111111|11111001|11
				_bitsMap.insert({ (unsigned char)214, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, T, F } });			//'' (214) |11111111|11111111|11111100|010
				_bitsMap.insert({ (unsigned char)215, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, F } });					//'' (215) |11111111|11111111|11110010
				_bitsMap.insert({ (unsigned char)216, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, F, F } });								//'' (216) |11111111|11111111|00100
				_bitsMap.insert({ (unsigned char)217, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, F, T } });								//'' (217) |11111111|11111111|00101
				_bitsMap.insert({ (unsigned char)218, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, F, F } });				//'' (218) |11111111|11111111|11111010|00
				_bitsMap.insert({ (unsigned char)219, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, F, T } });				//'' (219) |11111111|11111111|11111010|01
				_bitsMap.insert({ (unsigned char)220, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T } });		//'' (220) |11111111|11111111|11111111|1101
				_bitsMap.insert({ (unsigned char)221, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, T, T } });			//'' (221) |11111111|11111111|11111100|011
				_bitsMap.insert({ (unsigned char)222, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, F, F } });			//'' (222) |11111111|11111111|11111100|100
				_bitsMap.insert({ (unsigned char)223, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, F, T } });			//'' (223) |11111111|11111111|11111100|101
				_bitsMap.insert({ (unsigned char)224, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, F } });								//'' (224) |11111111|11111110|1100
				_bitsMap.insert({ (unsigned char)225, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, T } });					//'' (225) |11111111|11111111|11110011
				_bitsMap.insert({ (unsigned char)226, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, T } });								//'' (226) |11111111|11111110|1101
				_bitsMap.insert({ (unsigned char)227, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, T, F } });								//'' (227) |11111111|11111111|00110
				_bitsMap.insert({ (unsigned char)228, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, F, T } });							//'' (228) |11111111|11111111|101001
				_bitsMap.insert({ (unsigned char)229, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, T, T } });								//'' (229) |11111111|11111111|00111
				_bitsMap.insert({ (unsigned char)230, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, F, F } });								//'' (230) |11111111|11111111|01000
				_bitsMap.insert({ (unsigned char)231, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, T } });						//'' (231) |11111111|11111111|1110011
				_bitsMap.insert({ (unsigned char)232, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T, F } });							//'' (232) |11111111|11111111|101010
				_bitsMap.insert({ (unsigned char)233, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T, T } });							//'' (233) |11111111|11111111|101011
				_bitsMap.insert({ (unsigned char)234, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, F } });					//'' (234) |11111111|11111111|11110111|0
				_bitsMap.insert({ (unsigned char)235, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, T } });					//'' (235) |11111111|11111111|11110111|1
				_bitsMap.insert({ (unsigned char)236, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, F } });					//'' (236) |11111111|11111111|11110100
				_bitsMap.insert({ (unsigned char)237, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T } });					//'' (237) |11111111|11111111|11110101
				_bitsMap.insert({ (unsigned char)238, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T, F } });				//'' (238) |11111111|11111111|11111010|10
				_bitsMap.insert({ (unsigned char)239, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, F } });						//'' (239) |11111111|11111111|1110100
				_bitsMap.insert({ (unsigned char)240, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T, T } });				//'' (240) |11111111|11111111|11111010|11
				_bitsMap.insert({ (unsigned char)241, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, T, F } });			//'' (241) |11111111|11111111|11111100|110
				_bitsMap.insert({ (unsigned char)242, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, F } });				//'' (242) |11111111|11111111|11111011|00
				_bitsMap.insert({ (unsigned char)243, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, T } });				//'' (243) |11111111|11111111|11111011|01
				_bitsMap.insert({ (unsigned char)244, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, T, T, T } });			//'' (244) |11111111|11111111|11111100|111
				_bitsMap.insert({ (unsigned char)245, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, F, F } });			//'' (245) |11111111|11111111|11111101|000
				_bitsMap.insert({ (unsigned char)246, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, F, T } });			//'' (246) |11111111|11111111|11111101|001
				_bitsMap.insert({ (unsigned char)247, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T, F } });			//'' (247) |11111111|11111111|11111101|010
				_bitsMap.insert({ (unsigned char)248, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, F, T, T } });			//'' (248) |11111111|11111111|11111101|011
				_bitsMap.insert({ (unsigned char)249, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F } });		//'' (249) |11111111|11111111|11111111|1110
				_bitsMap.insert({ (unsigned char)250, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, F } });			//'' (250) |11111111|11111111|11111101|100
				_bitsMap.insert({ (unsigned char)251, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, F, T } });			//'' (251) |11111111|11111111|11111101|101
				_bitsMap.insert({ (unsigned char)252, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, F } });			//'' (252) |11111111|11111111|11111101|110
				_bitsMap.insert({ (unsigned char)253, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, T } });			//'' (253) |11111111|11111111|11111101|111
				_bitsMap.insert({ (unsigned char)254, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, F, F, F } });			//'' (254) |11111111|11111111|11111110|000
				_bitsMap.insert({ (unsigned char)255, { T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F, T, T, T, F } });				//'' (255) |11111111|11111111|11111011|10
			}
		}
	}
}