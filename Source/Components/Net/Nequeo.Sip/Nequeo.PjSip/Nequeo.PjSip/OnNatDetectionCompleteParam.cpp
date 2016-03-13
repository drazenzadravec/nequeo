/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnNatDetectionCompleteParam.cpp
*  Purpose :       SIP OnNatDetectionCompleteParam class.
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

#include "OnNatDetectionCompleteParam.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	The Endpoint has finished performing NAT type
/// detection that is initiated.
///	</summary>
OnNatDetectionCompleteParam::OnNatDetectionCompleteParam()
{
}

/// <summary>
/// Gets or sets the status of the detection process.
/// </summary>
int OnNatDetectionCompleteParam::Status::get()
{
	return _status;
}

/// <summary>
/// Gets or sets the status of the detection process.
/// </summary>
void OnNatDetectionCompleteParam::Status::set(int value)
{
	_status = value;
}

/// <summary>
/// Gets or sets the text describing the status, if the status is not PJ_SUCCESS.
/// </summary>
String^ OnNatDetectionCompleteParam::Reason::get()
{
	return _reason;
}

/// <summary>
/// Gets or sets the text describing the status, if the status is not PJ_SUCCESS.
/// </summary>
void OnNatDetectionCompleteParam::Reason::set(String^ value)
{
	_reason = value;
}

/// <summary>
/// Gets or sets this contains the NAT type as detected by the detection procedure.
/// </summary>
StunNatType OnNatDetectionCompleteParam::NatType::get()
{
	return _natType;
}

/// <summary>
/// Gets or sets this contains the NAT type as detected by the detection procedure.
/// </summary>
void OnNatDetectionCompleteParam::NatType::set(StunNatType value)
{
	_natType = value;
}

/// <summary>
/// Gets or sets the text describing that NAT type.
/// </summary>
String^ OnNatDetectionCompleteParam::NatTypeName::get()
{
	return _natTypeName;
}

/// <summary>
/// Gets or sets the text describing that NAT type.
/// </summary>
void OnNatDetectionCompleteParam::NatTypeName::set(String^ value)
{
	_natTypeName = value;
}