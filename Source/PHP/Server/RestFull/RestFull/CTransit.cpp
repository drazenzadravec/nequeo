/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          CTransit.h
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

#include "CTransit.h"

///	<summary>
///	Construct the transit.
///	</summary>
CTransit::CTransit() : m_disposed(false)
{
}

///	<summary>
///	Deconstruct the transit.
///	</summary>
CTransit::~CTransit()
{
	// If not disposed.
    if (!m_disposed)
    {
        m_disposed = true;
    }
}

/// <summary>
/// Web invoke request.
/// </summary>
/// <param name="request">The query request.</param>
/// <param name="rawData">The raw posted data.</param>
char* CTransit::WebInvoke(const char* request, const char* rawData)
{
	return "";
}

/// <summary>
/// Web invoke request.
/// </summary>
/// <param name="request">The query request.</param>
/// <param name="rawData">The raw posted data.</param>
/// <returns>The result.</returns>
char* CTransit::WebInvokeRequest(const char* request, const char* rawData)
{
	return "";
}

/// <summary>
/// Web invoke request.
/// </summary>
/// <param name="request">The query request.</param>
/// <returns>The result.</returns>
char* CTransit::WebInvokeRequestGet(const char* request)
{
	return "";
}