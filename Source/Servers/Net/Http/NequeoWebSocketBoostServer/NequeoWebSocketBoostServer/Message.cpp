/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Message.cpp
*  Purpose :       WebSocket message container.
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

#include "Message.h"

using namespace Nequeo::Net::WebSocket;

///	<summary>
///	WebSocket message container.
///	</summary>
Message::Message() :
	_disposed(false)
{
}

///	<summary>
///	WebSocket message container.
///	</summary>
Message::~Message()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	Get the message.
///	</summary>
///	<return>The message.</return>
std::string Message::Get()
{
	std::stringstream ss;
	ss << Received;
	return ss.str();
}

/// <summary>
/// Send message.
/// </summary>
/// <param name="messageType">The message type.</param>
/// <param name="message">The message to send.</param>
void Message::Send(MessageType messageType, std::streambuf* message)
{
	if (OnSend)
		// Send the message.
		OnSend(messageType, message);
}