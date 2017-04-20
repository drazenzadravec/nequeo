/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2017 http://www.nequeo.com.au/
*
*  File :          Expression.cpp
*  Purpose :       Expression class.
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

#include "ExpressionSB.h"

#include <sbml/SBMLTypes.h>

using namespace Nequeo::Math::SbML;

///	<summary>
///	Expression.
///	</summary>
Expression::Expression() :
	_disposed(false)
{
}

///	<summary>
///	Expression.
///	</summary>
Expression::~Expression()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	Get equation from MathML xml.
///	</summary>
/// <param name="mathML">The MathML xml.</param>
/// <returns>The equation.</returns>
std::string Expression::FromMathML(const std::string& mathML)
{
	char* result;
	ASTNode_t* math;
	
	math = readMathMLFromString(mathML.c_str());
	result = SBML_formulaToString(math);
	
	ASTNode_free(math);

	// If passed.
	if (result != NULL)
	{
		return std::string(result);
	}
	else
	{
		return std::string("");
	}
}

///	<summary>
///	Get MathML xml from equation.
///	</summary>
/// <param name="equation">The math equation.</param>
/// <returns>The MathML xml.</returns>
std::string Expression::ToMathML(const std::string& equation)
{
	char* result;
	ASTNode* math = SBML_parseFormula(equation.c_str());
	result = writeMathMLToString(math);
	
	ASTNode_free(math);

	// If passed.
	if (result != NULL)
	{
		return std::string(result);
	}
	else
	{
		return std::string("");
	}
}