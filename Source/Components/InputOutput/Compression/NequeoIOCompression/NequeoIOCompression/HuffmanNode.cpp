/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          HuffmanNode.cpp
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

#include "stdafx.h"

#include "HuffmanNode.h"

namespace Nequeo {
	namespace IO {
		namespace Compression
		{
			/// <summary>
			/// Huffman node.
			/// </summary>
			/// <param name="value">The value of the node.</param>
			/// <param name="parent">The parent node.</param>
			/// <param name="left">The left node.</param>
			/// <param name="right">The right node.</param>
			Node::Node(bool value, Node* parent, Node* left, Node* right) : _value(value), _parent(parent), _left(left), _right(right), _disposed(false)
			{
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			Node::~Node()
			{
				// If not disposed.
				if (!_disposed)
				{
					// Indicate that dispose has been called.
					_disposed = true;

					if (_left != nullptr)
						delete _left;

					if (_right != nullptr)
						delete _right;

					if (_parent != nullptr)
						delete _parent;
				}
			}

			/// <summary>
			/// Gets or sets the value.
			/// </summary>
			bool Node::getValue()
			{
				return _value;
			}

			/// <summary>
			/// Gets or sets the value.
			/// </summary>
			void Node::setValue(bool value)
			{
				_value = value;
			}

			/// <summary>
			/// Gets or sets the parent.
			/// </summary>
			Node* Node::getParent()
			{
				return _parent;
			}

			/// <summary>
			/// Gets or sets the parent.
			/// </summary>
			void Node::setParent(Node* parent)
			{
				if (_parent != nullptr)
					delete _parent;

				_parent = parent;
			}

			/// <summary>
			/// Gets or sets the left.
			/// </summary>
			Node* Node::getLeft()
			{
				return _left;
			}

			/// <summary>
			/// Gets or sets the left.
			/// </summary>
			void Node::setLeft(Node* left)
			{
				if (_left != nullptr)
					delete _left;

				_left = left;
			}

			/// <summary>
			/// Gets or sets the right.
			/// </summary>
			Node* Node::getRight()
			{
				return _right;
			}

			/// <summary>
			/// Gets or sets the right.
			/// </summary>
			void Node::setRight(Node* right)
			{
				if (_right != nullptr)
					delete _right;

				_right = right;
			}
		}
	}
}