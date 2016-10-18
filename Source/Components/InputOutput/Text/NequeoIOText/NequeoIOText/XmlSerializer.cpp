/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          XmlSerializer.cpp
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

#include "stdafx.h"

#include "XmlSerializer.h"
#include "Base\StringUtils.h"

#include <utility>
#include <algorithm>
#include <iostream>

using namespace tinyxml2;

namespace Nequeo {
	namespace IO {
		namespace XML
		{
			Nequeo::String Nequeo::IO::XML::DecodeEscapedXmlText(const Nequeo::String& textToDecode)
			{
				Nequeo::String decodedString = textToDecode;
				StringUtils::Replace(decodedString, "&quot;", "\"");
				StringUtils::Replace(decodedString, "&apos;", "'");
				StringUtils::Replace(decodedString, "&lt;", "<");
				StringUtils::Replace(decodedString, "&gt;", ">");
				StringUtils::Replace(decodedString, "&amp;;", "&");

				return decodedString;
			}

			XmlNode::XmlNode(const XmlNode& other) : m_node(other.m_node), m_doc(other.m_doc)
			{
			}

			XmlNode& XmlNode::operator=(const XmlNode& other)
			{
				if (this == &other)
				{
					return *this;
				}

				m_node = other.m_node;
				m_doc = other.m_doc;

				return *this;
			}

			const Nequeo::String XmlNode::GetName() const
			{
				return m_node->Value();
			}

			void XmlNode::SetName(const Nequeo::String& name)
			{
				m_node->SetValue(name.c_str(), false);
			}

			const Nequeo::String XmlNode::GetAttributeValue(const Nequeo::String& name) const
			{
				return m_node->ToElement()->Attribute(name.c_str(), nullptr);
			}

			void XmlNode::SetAttributeValue(const Nequeo::String& name, const Nequeo::String& value)
			{
				m_node->ToElement()->SetAttribute(name.c_str(), value.c_str());
			}

			bool XmlNode::HasNextNode() const
			{
				return m_node->NextSibling() != nullptr;
			}

			XmlNode XmlNode::NextNode() const
			{
				return XmlNode(m_node->NextSiblingElement(), *m_doc);
			}

			XmlNode XmlNode::NextNode(const char* name) const
			{
				return XmlNode(m_node->NextSiblingElement(name), *m_doc);
			}

			XmlNode XmlNode::NextNode(const Nequeo::String& name) const
			{
				return NextNode(name.c_str());
			}

			XmlNode XmlNode::FirstChild() const
			{
				return XmlNode(m_node->FirstChildElement(), *m_doc);
			}

			XmlNode XmlNode::FirstChild(const char* name) const
			{
				return XmlNode(m_node->FirstChildElement(name), *m_doc);
			}

			XmlNode XmlNode::FirstChild(const Nequeo::String& name) const
			{
				return FirstChild(name.c_str());
			}

			bool XmlNode::HasChildren() const
			{
				return !m_node->NoChildren();
			}

			XmlNode XmlNode::Parent() const
			{
				return XmlNode(m_node->Parent()->ToElement(), *m_doc);
			}

			Nequeo::String XmlNode::GetText() const
			{
				if (m_node != nullptr)
				{
					tinyxml2::XMLPrinter printer;
					tinyxml2::XMLNode* node = m_node->FirstChild();
					while (node != nullptr)
					{
						node->Accept(&printer);
						node = node->NextSibling();
					}

					return printer.CStr();
				}

				return "";
			}

			void XmlNode::SetText(const Nequeo::String& textValue)
			{
				if (m_node != nullptr)
				{
					tinyxml2::XMLText* text = m_doc->m_doc->NewText(textValue.c_str());
					m_node->InsertEndChild(text);
				}
			}

			XmlNode XmlNode::CreateChildElement(const Nequeo::String& name)
			{
				tinyxml2::XMLElement* element = m_doc->m_doc->NewElement(name.c_str());
				return XmlNode(m_node->InsertEndChild(element), *m_doc);
			}

			XmlNode XmlNode::CreateSiblingElement(const Nequeo::String& name)
			{
				tinyxml2::XMLElement* element = m_doc->m_doc->NewElement(name.c_str());
				return XmlNode(m_node->Parent()->InsertEndChild(element), *m_doc);
			}

			bool XmlNode::IsNull()
			{
				return m_node == nullptr;
			}

			static const char* XML_SERIALIZER_ALLOCATION_TAG = "XmlDocument";

			XmlDocument::XmlDocument()
			{
				m_doc = Nequeo::New<tinyxml2::XMLDocument>(XML_SERIALIZER_ALLOCATION_TAG, true, tinyxml2::Whitespace::PRESERVE_WHITESPACE);
			}

			XmlDocument::XmlDocument(XmlDocument&& doc) : m_doc{ std::move(doc.m_doc) } // take the innards
			{
				doc.m_doc = nullptr; // leave nothing behind
			}

			XmlDocument::~XmlDocument()
			{
				Nequeo::Delete(m_doc);
			}

			XmlNode XmlDocument::GetRootElement() const
			{
				return XmlNode(m_doc->FirstChildElement(), *this);
			}

			bool XmlDocument::WasParseSuccessful() const
			{
				return !m_doc->Error();
			}

			Nequeo::String XmlDocument::GetErrorMessage() const
			{
				return !WasParseSuccessful() ? m_doc->ErrorName() : "";
			}

			Nequeo::String XmlDocument::ConvertToString() const
			{
				tinyxml2::XMLPrinter printer;
				printer.PushHeader(false, true);
				m_doc->Accept(&printer);

				return printer.CStr();
			}

			XmlDocument XmlDocument::CreateFromXmlStream(Nequeo::IOStream& xmlStream)
			{
				Nequeo::String xmlString((Nequeo::IStreamBufIterator(xmlStream)), Nequeo::IStreamBufIterator());
				return CreateFromXmlString(xmlString);
			}

			XmlDocument XmlDocument::CreateFromXmlString(const Nequeo::String& xmlText)
			{
				XmlDocument xmlDocument;
				xmlDocument.m_doc->Parse(xmlText.c_str(), xmlText.size());
				return xmlDocument;
			}

			XmlDocument XmlDocument::CreateWithRootNode(const Nequeo::String& rootNodeName)
			{
				XmlDocument xmlDocument;
				tinyxml2::XMLElement* rootNode = xmlDocument.m_doc->NewElement(rootNodeName.c_str());
				xmlDocument.m_doc->LinkEndChild(rootNode);

				return xmlDocument;
			}
		}
	}
}