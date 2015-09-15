/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          XmlElement.cpp
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

#include "stdafx.h"

#include "XmlElement.h"

///	<summary>
///	Construct the xml element.
///	</summary>
Nequeo::Serialisation::Factor::XmlElement::XmlElement() : m_disposed(true)
{
	m_disposed = false;
}

///	<summary>
///	Deconstruct the xml element.
///	</summary>
Nequeo::Serialisation::Factor::XmlElement::~XmlElement()
{
	// If not disposed.
    if (!m_disposed)
    {
        m_disposed = true;
    }
}

Nequeo::Serialisation::Factor::XmlElementFactor::XmlElementFactor() : mElementName("unnamed")
{
}

void Nequeo::Serialisation::Factor::XmlElementFactor::setElementName(const string& inName)
{
	mElementName = inName;
}

void Nequeo::Serialisation::Factor::XmlElementFactor::setAttribute(const string& inAttributeName, const string& inAttributeValue)
{
	// Set the key/value pair, replacing the existing one if it exists.
	mAttributes[inAttributeName] = inAttributeValue;
}

void Nequeo::Serialisation::Factor::XmlElementFactor::addSubElement(const XmlElementFactor* inElement)
{
	// Add the new element to the vector of subelements.
	mSubElements.push_back(inElement);
}

void Nequeo::Serialisation::Factor::XmlElementFactor::setTextNode(const string& inValue)
{
	mTextNode = inValue;
}

ostream& Nequeo::Serialisation::Factor::operator<<(ostream& outStream, const Nequeo::Serialisation::Factor::XmlElementFactor& inElem)
{
	inElem.writeToStream(outStream);
	return (outStream);
}

void Nequeo::Serialisation::Factor::XmlElementFactor::writeToStream(ostream& outStream, int inIndentLevel) const
{
	indentStream(outStream, inIndentLevel);
	outStream << "<" << mElementName;

	// Output any attributes.
	for (map<string, string>::const_iterator it = mAttributes.begin(); it != mAttributes.end(); ++it) 
	{
		outStream << " " << it->first << "=\"" << it->second << "\"";
	}

	// Close the start tag.
	outStream << ">";

	if (mTextNode != "") 
	{
		// If there’s a text node, output it.
		outStream << mTextNode;
	} 
	else 
	{
		outStream << endl;

		// Call writeToStream at inIndentLevel+1 for any subelements.
		for (vector<const XmlElementFactor*>::const_iterator it = mSubElements.begin(); it != mSubElements.end(); ++it) 
		{
			(*it)->writeToStream(outStream, inIndentLevel + 1);
		}

		indentStream(outStream, inIndentLevel);
	}

	// Write the close tag.
	outStream << "</" << mElementName << ">" << endl;
}

void Nequeo::Serialisation::Factor::XmlElementFactor::indentStream(ostream& outStream, int inIndentLevel) const
{
	for (int i = 0; i < inIndentLevel; i++) 
	{
		outStream << "\t";
	}
}