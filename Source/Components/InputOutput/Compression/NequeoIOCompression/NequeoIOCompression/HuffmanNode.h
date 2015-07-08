/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          HuffmanNode.h
*  Purpose :       HuffmanNode class.
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

#ifndef _HUFFMANNODE_H
#define _HUFFMANNODE_H

#include "GlobalIOCompression.h"

namespace Nequeo {
	namespace IO {
		namespace Compression
		{
			/// <summary>
			/// Base node interface.
			/// </summary>
			class INode
			{
			public:
				/// <summary>
				/// The destructor.
				/// </summary>
				virtual ~INode() {}

				const int _f;

			protected:
				/// <summary>
				///  Base node interface.
				/// </summary>
				/// <param name="f">The value (010001 etc) of the node.</param>
				INode(int f) : _f(f) {}
			};

			/// <summary>
			/// Internal parent node.
			/// </summary>
			class InternalNode : public INode
			{
			public:
				/// <summary>
				/// Internal parent node.
				/// </summary>
				/// <param name="c0">The left node.</param>
				/// <param name="c1">The right node.</param>
				InternalNode(INode* c0, INode* c1) : INode(c0->_f + c1->_f), _left(c0), _right(c1) {}

				/// <summary>
				/// The destructor.
				/// </summary>
				~InternalNode()
				{
					delete _left;
					delete _right;
				}

				INode* const _left;
				INode* const _right;
			};

			/// <summary>
			/// Leaf parent node.
			/// </summary>
			class LeafNode : public INode
			{
			public:
				/// <summary>
				/// Leaf parent node.
				/// </summary>
				/// <param name="f">The value (010001 etc) of the node.</param>
				/// <param name="c">The charactor.</param>
				LeafNode(int f, char c) : INode(f), _c(c) {}

				const char _c;
			};

			/// <summary>
			/// Node comparer structure.
			/// </summary>
			struct NodeCmp
			{
				/// <summary>
				/// Function call operator.
				/// </summary>
				/// <param name="lhs">The left node.</param>
				/// <param name="rhs">The right node.</param>
				/// <returns>True if left node is greator then right node; else false.</returns>
				bool operator()(const INode* lhs, const INode* rhs) const
				{
					return lhs->_f > rhs->_f;
				}
			};

			/// <summary>
			/// Huffman node.
			/// </summary>
			class Node
			{
			public:
				/// <summary>
				/// Huffman node.
				/// </summary>
				/// <param name="value">The value of the node.</param>
				/// <param name="parent">The parent node.</param>
				/// <param name="left">The left node.</param>
				/// <param name="right">The right node.</param>
				Node(bool value, Node* parent, Node* left = nullptr, Node* right = nullptr);

				/// <summary>
				/// This destructor.
				/// </summary>
				~Node();

				/// <summary>
				/// Gets or sets the value.
				/// </summary>
				bool getValue();
				void setValue(bool value);

				/// <summary>
				/// Gets or sets the parent.
				/// </summary>
				Node* getParent();
				void setParent(Node* parent);

				/// <summary>
				/// Gets or sets the left.
				/// </summary>
				Node* getLeft();
				void setLeft(Node* left);

				/// <summary>
				/// Gets or sets the right.
				/// </summary>
				Node* getRight();
				void setRight(Node* right);

			private:
				bool _disposed;

				bool _value;
				Node* _left;
				Node* _right;
				Node* _parent;
			};
		}
	}
}
#endif