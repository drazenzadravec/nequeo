/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          XmlElement.h
 *  Purpose :       Serialisation
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

#include "stdafx.h"

using namespace System;
using namespace System::IO;
using namespace System::Numerics;
using namespace System::Xml;
using namespace System::Xml::Serialization;
using namespace System::Runtime::Serialization;

namespace Nequeo 
{
	namespace Serialisation 
	{
		namespace Factor 
		{
			///	<summary>
			///	Generate the xml element from the stream.
			///	</summary>
			public ref class XmlElement sealed
			{
				public:
					// Constructors
					XmlElement();
					virtual ~XmlElement();

				private:
					// Fields
					bool m_disposed;
				
			};

			///	<summary>
			///	The class definition of a simple XML Element class.
			///	</summary>
			class XmlElementFactor
			{
				public:
					XmlElementFactor();

					void setElementName(const std::string& inName);
					void setAttribute(const std::string& inAttributeName, const std::string& inAttributeValue);
					void addSubElement(const XmlElementFactor* inElement);

					// Setting a text node will override any nested elements.
					void setTextNode(const std::string& inValue);

					friend std::ostream& operator<<(std::ostream& outStream, const XmlElementFactor& inElem);

				protected:
					void writeToStream(std::ostream& outStream, int inIndentLevel = 0) const;
					void indentStream(std::ostream& outStream, int inIndentLevel) const;

				private:
					std::string mElementName;
					std::map<std::string, std::string> mAttributes;
					std::vector<const XmlElementFactor*> mSubElements;
					std::string mTextNode;

			};
		}
	}
}