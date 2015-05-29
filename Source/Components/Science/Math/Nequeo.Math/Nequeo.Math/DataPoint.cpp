/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          DataPoint.cpp
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

#include "DataPoint.h"

///	<summary>
///	Construct the data point.
///	</summary>
/// <param name='x'>The x data point.</param>
/// <param name='y'>The y data point.</param>
Nequeo::Math::DataPoint::DataPoint(double x, double y) : m_disposed(true)
{
	m_x = x;
	m_y = y;
	m_z = (double)0.0;
	m_disposed = false;
}

///	<summary>
///	Construct the data point.
///	</summary>
/// <param name='x'>The x data point.</param>
/// <param name='y'>The y data point.</param>
/// <param name='z'>The z data point.</param>
Nequeo::Math::DataPoint::DataPoint(double x, double y, double z) : m_disposed(true)
{
	m_x = x;
	m_y = y;
	m_z = z;
	m_disposed = false;
}

///	<summary>
///	Deconstruct the data point.
///	</summary>
Nequeo::Math::DataPoint::~DataPoint()
{
	// If not disposed.
    if (!m_disposed)
    {
        m_disposed = true;
    }
}

///	<summary>
///	Gets the X data point.
///	</summary>
double Nequeo::Math::DataPoint::X::get()
{
    return m_x;
}

///	<summary>
///	Sets the X data point.
///	</summary>
/// <param name='value'>The x data point value.</param>
void Nequeo::Math::DataPoint::X::set(double value)
{
    m_x = value;
}

///	<summary>
///	Gets the Y data point.
///	</summary>
double Nequeo::Math::DataPoint::Y::get()
{
    return m_y;
}

///	<summary>
///	Sets the Y data point.
///	</summary>
/// <param name='value'>The y data point value.</param>
void Nequeo::Math::DataPoint::Y::set(double value)
{
    m_y = value;
}

///	<summary>
///	Gets the Z data point.
///	</summary>
double Nequeo::Math::DataPoint::Z::get()
{
    return m_z;
}

///	<summary>
///	Sets the Z data point.
///	</summary>
/// <param name='value'>The y data point value.</param>
void Nequeo::Math::DataPoint::Z::set(double value)
{
    m_z = value;
}