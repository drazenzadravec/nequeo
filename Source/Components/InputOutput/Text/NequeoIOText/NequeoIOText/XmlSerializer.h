/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          XmlSerializer.h
*  Purpose :       XmlSerializer header.
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

#ifndef _XMLSERIALIZER_H
#define _XMLSERIALIZER_H

#include "GlobalText.h"

#include "Base\Array.h"
#include "Base\Allocator.h"
#include "Base\Outcome.h"

#include <tinyxml2.h>

namespace Nequeo {
	namespace IO {
		namespace XML
		{
			/**
			* Converts escaped xml text back to the original characters (e.g. < ! > = etc...)
			*/
			Nequeo::String DecodeEscapedXmlText(const Nequeo::String& textToDecode);

			class XmlDocument;

			/**
			* Node in an Xml Document
			*/
			class XmlNode
			{
			public:
				/**
				* copies node and document over.
				*/
				XmlNode(const XmlNode& other);
				/**
				* copies node and document over.
				*/
				XmlNode& operator=(const XmlNode& other);
				/**
				* Get the name of the node.
				*/
				const Nequeo::String GetName() const;
				/**
				* Set the name of the node.
				*/
				void SetName(const Nequeo::String& name);
				/**
				* Get Value of an attribute specified by name.
				*/
				const Nequeo::String GetAttributeValue(const Nequeo::String& name) const;
				/**
				* Set an attribute at name to value
				*/
				void SetAttributeValue(const Nequeo::String& name, const Nequeo::String& value);
				/**
				* Get the inner text of the node (potentially includes other nodes)
				*/
				Nequeo::String GetText() const;
				/**
				* Set the inner text of the node
				*/
				void SetText(const Nequeo::String& textValue);
				/**
				* returns true if this node has another sibling.
				*/
				bool HasNextNode() const;
				/**
				* returns the next sibling.
				*/
				XmlNode NextNode() const;
				/**
				* returns the next sibling that matches node name.
				*/
				XmlNode NextNode(const char* name) const;
				/**
				* returns the next sibling that matches node name.
				*/
				XmlNode NextNode(const Nequeo::String& name) const;
				/**
				* return the first child node of this node.
				*/
				XmlNode FirstChild() const;
				/**
				* returns the first child node of this node that has name.
				*/
				XmlNode FirstChild(const char* name) const;
				/**
				* returns the first child node of this node that has name.
				*/
				XmlNode FirstChild(const Nequeo::String& name) const;
				/**
				* returns true if this node has child nodes.
				*/
				bool HasChildren() const;
				/**
				* returns the parent of this node.
				*/
				XmlNode Parent() const;
				/**
				* Creates a new child element to this with name
				*/
				XmlNode CreateChildElement(const Nequeo::String& name);
				/**
				* Creates a new child element to this with name
				*/
				XmlNode CreateSiblingElement(const Nequeo::String& name);
				/**
				* If current node is valid.
				*/
				bool IsNull();

			private:
				XmlNode(tinyxml2::XMLNode* node, const XmlDocument& document) :
					m_node(node), m_doc(&document)
				{
				}

				//we do not own these.... I just had to change it from ref because the compiler was
				//confused about which assignment operator to call. Do not... I repeat... do not delete
				//these pointers in your destructor.
				tinyxml2::XMLNode* m_node;
				const XmlDocument* m_doc;

				friend class XmlDocument;
			};

			/**
			* Container for Xml Document as a whole. All nodes have a reference to their parent document. Any changes
			* you make to the nodes will be reflected here.
			*/
			class XmlDocument
			{
			public:
				/**
				* move document memory
				*/
				XmlDocument(XmlDocument&& doc);
				XmlDocument(const XmlDocument& other) = delete;

				~XmlDocument();

				/**
				* Get root element of the document
				*/
				XmlNode GetRootElement() const;
				/**
				* Convert entire document to string. Use this if you for example, want to save the document to a file.
				*/
				Nequeo::String ConvertToString() const;
				/**
				* Returns true if the call to CreateFromXml* was successful, otherwise false.
				* if this returns false, you can call GetErrorMessage() to see details.
				*/
				bool WasParseSuccessful() const;
				/**
				* Returns the error message if the call to CreateFromXml* failed.
				*/
				Nequeo::String GetErrorMessage() const;
				/**
				* Parses the stream into an XMLDocument
				*/
				static XmlDocument CreateFromXmlStream(Nequeo::IOStream&);
				/**
				* Parses the string into an XMLDocument
				*/
				static XmlDocument CreateFromXmlString(const Nequeo::String&);
				/**
				* Creates an empty document with root node name
				*/
				static XmlDocument CreateWithRootNode(const Nequeo::String&);

			private:
				XmlDocument();

				tinyxml2::XMLDocument* m_doc;

				friend class XmlNode;
			};
		}
	}
}
#endif
