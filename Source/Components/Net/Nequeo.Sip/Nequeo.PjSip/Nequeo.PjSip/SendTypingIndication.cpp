/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          SendTypingIndication.cpp
*  Purpose :       SIP SendTypingIndication class.
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

#include "SendTypingIndication.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// This structure contains parameters for sending typing indication methods,
/// e.g: Contact::SendTypingIndication(), Call:SendTypingIndication().
/// </summary>
SendTypingIndicationParam::SendTypingIndicationParam() :
	_disposed(false)
{
}

/// <summary>
/// This structure contains parameters for sending typing indication methods,
/// e.g: Contact::SendTypingIndication(), Call:SendTypingIndication().
/// </summary>
SendTypingIndicationParam::~SendTypingIndicationParam()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	Gets or sets True to indicate to remote that local person is currently typing an IM.
///	</summary>
bool SendTypingIndicationParam::IsTyping::get()
{
	return _isTyping;
}

///	<summary>
///	Gets or sets True to indicate to remote that local person is currently typing an IM.
///	</summary>
void SendTypingIndicationParam::IsTyping::set(bool value)
{
	_isTyping = value;
}

///	<summary>
///	Gets or sets the list of headers etc to be included in outgoing request.
///	</summary>
SipTxOption^ SendTypingIndicationParam::TxOption::get()
{
	return _txOption;
}

///	<summary>
///	Gets or sets the list of headers etc to be included in outgoing request.
///	</summary>
void SendTypingIndicationParam::TxOption::set(SipTxOption^ value)
{
	_txOption = value;
}
