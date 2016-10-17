/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnNatCheckStunServersCompleteParam.cpp
*  Purpose :       SIP OnNatCheckStunServersCompleteParam class.
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

#include "OnNatCheckStunServersCompleteParam.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	The Endpoint has finished performing STUN server
/// checking that is initiated when calling libInit(), or by
/// calling natCheckStunServers().
///	</summary>
OnNatCheckStunServersCompleteParam::OnNatCheckStunServersCompleteParam()
{
}

/// <summary>
/// Gets or sets the status of the detection process.
/// </summary>
int OnNatCheckStunServersCompleteParam::Status::get()
{
	return _status;
}

/// <summary>
/// Gets or sets the status of the detection process.
/// </summary>
void OnNatCheckStunServersCompleteParam::Status::set(int value)
{
	_status = value;
}

/// <summary>
/// Gets or sets the server name that yields successful result. This will only
/// contain value if status is successful.
/// </summary>
String^ OnNatCheckStunServersCompleteParam::Name::get()
{
	return _name;
}

/// <summary>
/// Gets or sets the server name that yields successful result. This will only
/// contain value if status is successful.
/// </summary>
void OnNatCheckStunServersCompleteParam::Name::set(String^ value)
{
	_name = value;
}

/// <summary>
/// Gets or sets the server IP address and port in "IP:port" format. This will only
/// contain value if status is successful.
/// </summary>
String^ OnNatCheckStunServersCompleteParam::SocketAddress::get()
{
	return _socketAddress;
}

/// <summary>
/// Gets or sets the server IP address and port in "IP:port" format. This will only
/// contain value if status is successful.
/// </summary>
void OnNatCheckStunServersCompleteParam::SocketAddress::set(String^ value)
{
	_socketAddress = value;
}