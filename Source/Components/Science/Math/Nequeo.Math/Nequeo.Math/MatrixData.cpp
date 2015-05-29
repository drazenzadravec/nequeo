/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          MatrixData.cpp
 *  Purpose :       
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

#include "MatrixData.h"

///	<summary>
///	Construct the matrix data.
///	</summary>
/// <param name='row'>The row of matrix data.</param>
Nequeo::Math::MatrixData::MatrixData(array<double>^ row) : m_disposed(true)
{
	m_row = row;
	m_disposed = false;
}

///	<summary>
///	Deconstruct the matrix data.
///	</summary>
Nequeo::Math::MatrixData::~MatrixData()
{
	// If not disposed.
    if (!m_disposed)
    {
        m_disposed = true;
    }
}

///	<summary>
///	Gets the Row of data.
///	</summary>
array<double>^ Nequeo::Math::MatrixData::Row::get()
{
    return m_row;
}

///	<summary>
///	Sets the Row of data.
///	</summary>
/// <param name='value'>The x data point value.</param>
void Nequeo::Math::MatrixData::Row::set(array<double>^ value)
{
    m_row = value;
}